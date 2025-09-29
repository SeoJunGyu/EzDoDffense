using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    private Dictionary<long, Queue<ParticleSystem>> particles = new Dictionary<long, Queue<ParticleSystem>>();
    private Dictionary<long, SingleSkillData> singleDatas = new Dictionary<long, SingleSkillData>();
    private Dictionary<long, MultiSkillData> multiDatas = new Dictionary<long, MultiSkillData>();

    public Dictionary<long, AudioClip> sounds = new Dictionary<long, AudioClip>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var data = DataTableManager.SingleSkillTable.Get(313020511);
            CheckActive(data).Play();
        }
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    //발동 확률 -> 스킬 대상 확인 -> 스킬 범위 확인 -> 스킬 적용 가능 여부 확인 -> 수치 적용 -> 스킬 지속시간 부여 -> 이펙트 플레이
    private static bool IsValid(ISkillTarget target)
    {
        return target != null && target.IsTargetable;
    }

    //발동 확률 확인
    private bool CheckSkillRandom(int chance)
    {
        if (chance >= 100)
        {
            return true;
        }
        if(chance <= 0)
        {
            return false;
        }

        var rnd = UnityEngine.Random.Range(0, 100);
        Debug.Log($"{rnd}");
        return rnd < chance;
    }

    //발동 대상 확인
    private List<ISkillTarget> CheckSkillTarget(AllyUnit caster, SingleSkillData data)
    {
        //1 - 자기자신 / 2 - 단일적군 / 3 - 범위적군 / 4 - 아군전체 / 5 - 적군전체
        var list = new List<ISkillTarget>();

        switch (data.Skill_Target)
        {
            case 1:
                if (IsValid(caster))
                {
                    list.Add(caster);
                }
                break;
            case 2:
                if (IsValid(caster?.Target))
                {
                    list.Add(caster.Target);
                }
                break;
            case 3:
                foreach (var target in caster.findSkillTarget)
                {
                    if (IsValid(target))
                    {
                        list.Add(target);
                    }
                }
                break;
            case 4:
                foreach (var a in PlacementManager.Instance.GetAllyUnits)
                {
                    if (IsValid(a))
                    {
                        list.Add(a);
                    }
                }
                break;
            case 5:
                foreach (var e in EnemySpawner.Instance.GetEnemies)
                {
                    if (IsValid(e))
                    {
                        list.Add(e);
                    }
                }
                break;
        }

        return list;
    }

    private List<ISkillTarget> CheckSkillTarget(AllyUnit caster, MultiSkillData data)
    {
        //1 - 자기자신 / 2 - 단일적군 / 3 - 범위적군 / 4 - 아군전체 / 5 - 적군전체
        var list = new List<ISkillTarget>();

        switch (data.Skill_Target)
        {
            case 1:
                if (IsValid(caster))
                {
                    list.Add(caster);
                }
                break;
            case 2:
                if (IsValid(caster?.Target))
                {
                    list.Add(caster.Target);
                }
                break;
            case 3:
                {
                    caster.findSkillTarget.RemoveAll(e => e == null || !e.gameObject.activeSelf || e.IsDead);

                    foreach (var target in caster.findSkillTarget)
                    {
                        if (IsValid(target))
                        {
                            list.Add(target);
                        }
                    }

                    break;
                }
            case 4:
                foreach (var a in PlacementManager.Instance.GetAllyUnits)
                {
                    if (IsValid(a))
                    {
                        list.Add(a);
                    }
                }
                break;
            case 5:
                foreach (var e in EnemySpawner.Instance.GetEnemies)
                {
                    if (IsValid(e))
                    {
                        list.Add(e);
                    }
                }
                break;
        }

        return list;
    }

    public void ExecuteSingleSkill(AllyUnit caster)
    {
        var data = GetSingleData(caster.Skill1);
        var list = CheckSkillTarget(caster, data);
        if (CheckSkillRandom(data.Skill_Random))
        {
            foreach (var target in list)
            {
                if (!IsValid(target))
                {
                    continue;
                }
                if (target.ActiveParticle.Contains(data.Skill_ID))
                {
                    continue;
                }

                var particle = CheckActive(data);
                if(data.Skill_Target == 2)
                {
                    particle.transform.SetParent(caster.EffectAnchor);
                    particle.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    particle.transform.SetParent(target.EffectAnchor);
                }
                particle.transform.localPosition = Vector3.zero;
                particle.gameObject.SetActive(true);
                particle.Play();

                if(data.Skill_Target != 5)
                {
                    AudioManager.Instance.PlaySkill(data.SkillAudio, target.EffectAnchor.position);
                }

                target.ApplySingleSkill(data, caster, particle);
            }
        }
    }

    public void ExecuteMultiSkill(AllyUnit caster)
    {
        var data = GetMultiData(caster.Skill2);
        var list = CheckSkillTarget(caster, data);
        if (CheckSkillRandom(data.Skill_Random))
        {
            foreach (var target in list)
            {
                if (!IsValid(target))
                {
                    continue;
                }
                if (target.ActiveParticle.Contains(data.Skill_ID))
                {
                    continue;
                }

                var particle = CheckActive(data);
                particle.transform.SetParent(target.EffectAnchor);
                particle.transform.localPosition = Vector3.zero;
                particle.gameObject.SetActive(true);
                particle.Play();

                AudioManager.Instance.PlaySkill(data.SkillAudio, target.EffectAnchor.position);

                target.ApplyMultiSkill(data, caster, particle);
            }
        }
    }

    public ParticleSystem CheckActive(SingleSkillData data)
    {
        if(!particles.TryGetValue(data.Skill_ID, out var q))
        {
            q = new Queue<ParticleSystem>();
            particles.Add(data.Skill_ID, q);
        }

        ParticleSystem ps = null;

        if(q.Count > 0)
        {
            ps = q.Dequeue();
        }
        else
        {
            ps = Instantiate(data.SkillParticle, transform);
        }

        if(ps == null)
        {
            ps = Instantiate(data.SkillParticle, transform);
        }
        ps.gameObject.SetActive(false);

        return ps;
    }

    public ParticleSystem CheckActive(MultiSkillData data)
    {
        if (!particles.TryGetValue(data.Skill_ID, out var q))
        {
            q = new Queue<ParticleSystem>();
            particles.Add(data.Skill_ID, q);
        }

        ParticleSystem ps = null;

        if (q.Count > 0)
        {
            ps = q.Dequeue();
        }
        else
        {
            ps = Instantiate(data.SkillParticle, transform);
        }

        ps.gameObject.SetActive(false);

        return ps;
    }

    public void ReturnParticle(long skillId, ParticleSystem ps)
    {
        if(ps == null)
        {
            return;
        }

        var p = ps.transform.parent;
        if(p != null && !p.gameObject.activeSelf)
        {
            StartCoroutine(DeferReturn(skillId, ps));
            return;
        }

        DoReturn(skillId, ps);
    }

    public void ReturnActiveEffect(SingleSkillData singleData)
    {
        if(singleData.Skill_Target == 4)
        {
            foreach (var a in PlacementManager.Instance.GetAllyUnits)
            { 
                if (!a.gameObject.activeSelf)
                {
                    continue;
                }

                if (a.ActiveBuffValue.ContainsKey(singleData.Skill_ID))
                {
                    a.ResetBuffValue(singleData.Skill_ID, singleData.Skill_Effect);
                }
            }
        }
        else
        {
            foreach (var a in EnemySpawner.Instance.GetEnemies)
            {
                if (!a.gameObject.activeSelf)
                {
                    continue;
                }

                if (a.ActiveBuffValue.ContainsKey(singleData.Skill_ID))
                {
                    a.ResetBuffValue(singleData.Skill_ID, singleData.Skill_Effect);
                }
            }
        }
    }

    public IEnumerator DeferReturn(long skillId, ParticleSystem ps)
    {
        yield return null;

        DoReturn(skillId, ps);
    }

    public IEnumerator ReturnWhenDead(long skillId, ParticleSystem ps)
    {
        yield return new WaitWhile(() => ps != null && ps.IsAlive(true));

        DoReturn(skillId, ps);
    }

    private void DoReturn(long skillId, ParticleSystem ps)
    {
        ps.transform.localScale = Vector3.one;
        ps.transform.SetParent(transform);
        ps.gameObject.SetActive(false);
        if (particles.TryGetValue(skillId, out var q))
        {
            if (!q.Contains(ps))
            {
                q.Enqueue(ps);
            }
        }
    }

    public SingleSkillData GetSingleData(long id)
    {
        if (singleDatas.ContainsKey(id))
        {
            return singleDatas[id];
        }

        var data = DataTableManager.SingleSkillTable.Get(id);
        singleDatas.Add(id, data);
        return data;
    }

    public MultiSkillData GetMultiData(long id)
    {
        if (multiDatas.ContainsKey(id))
        {
            return multiDatas[id];
        }

        var data = DataTableManager.MultiSkillTable.Get(id);
        multiDatas.Add(id, data);
        return data;
    }

    public AudioClip GetSingleSkillSound(SingleSkillData data)
    {
        if (sounds.ContainsKey(data.Skill_ID))
        {
            return sounds[data.Skill_ID];
        }

        sounds.Add(data.Skill_ID, data.SkillAudio);
        return data.SkillAudio;
    }

    public AudioClip GetMultiSkillSound(MultiSkillData data)
    {
        if (sounds.ContainsKey(data.Skill_ID))
        {
            return sounds[data.Skill_ID];
        }

        sounds.Add(data.Skill_ID, data.SkillAudio);
        return data.SkillAudio;
    }
}
