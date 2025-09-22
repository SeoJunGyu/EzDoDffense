using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    private Dictionary<long, Queue<ParticleSystem>> particles = new Dictionary<long, Queue<ParticleSystem>>();

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

        return UnityEngine.Random.Range(0, 100) < chance;
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
                {
                    caster.findSkillTarget.RemoveAll(e => e == null || !e.gameObject.activeSelf || e.IsDead);

                    foreach(var target in caster.findSkillTarget)
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
        var data = DataTableManager.SingleSkillTable.Get(caster.Skill1);
        var list = CheckSkillTarget(caster, DataTableManager.SingleSkillTable.Get(caster.Skill1));
        foreach(var target in list)
        {
            if (target.ActiveParticle.Contains(data.Skill_ID))
            {
                continue;
            }

            //var particle = CheckActive(data);
            //particle.transform.SetParent(target.EffectAnchor);
            //particle.transform.localPosition = Vector3.zero;
            //particle.gameObject.SetActive(true);
            //particle.Play();

            target.ApplySingleSkill(data, caster, null);
            target.ActiveParticle.Add(data.Skill_ID);
        }
    }

    public void ExecuteMultiSkill(AllyUnit caster)
    {
        var data = DataTableManager.MultiSkillTable.Get(caster.Skill2);
        var list = CheckSkillTarget(caster, data);
        foreach (var target in list)
        {
            target.ApplyMultiSkill(data, caster, null);
        }
    }

    public ParticleSystem CheckActive(SingleSkillData data)
    {
        if(!particles.TryGetValue(data.Skill_ID, out var q))
        {
            q = new Queue<ParticleSystem>();
            particles.Add(data.Skill_ID, q);
        }

        while(q.Count > 0)
        {
            var candidate = q.Dequeue();
            if(candidate != null)
            {
                candidate.gameObject.SetActive(false);
                return candidate;
            }
        }

        //없는경우
        var inst = Instantiate(data.SkillParticle, transform);
        inst.gameObject.SetActive(false);
        q.Enqueue(inst);
        return inst;
    }

    public ParticleSystem CheckActive(MultiSkillData data)
    {
        if (!particles.TryGetValue(data.Skill_ID, out var q))
        {
            q = new Queue<ParticleSystem>();
            particles.Add(data.Skill_ID, q);
        }

        while (q.Count > 0)
        {
            var candidate = q.Dequeue();
            if (candidate != null)
            {
                candidate.gameObject.SetActive(false);
                return candidate;
            }
        }

        //없는경우
        var inst = Instantiate(data.SkillParticle, transform);
        inst.gameObject.SetActive(false);
        q.Enqueue(inst);
        return inst;
    }
}
