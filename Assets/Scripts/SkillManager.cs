using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    public static List<KeyValuePair<long, ParticleSystem>> particles = new List<KeyValuePair<long, ParticleSystem>>();

    private void Awake()
    {
        Instance = this;
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

        return UnityEngine.Random.Range(0, 100) <= chance;
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
        var list = CheckSkillTarget(caster, caster.SingleSkill);
        foreach(var target in list)
        {
            target.ApplySingleSkill(caster.SingleSkill, caster);
        }
    }

    public void ExecuteMultiSkill(AllyUnit caster)
    {
        var list = CheckSkillTarget(caster, caster.MultiSkill);
        foreach (var target in list)
        {
            target.ApplyMultiSkill(caster.MultiSkill, caster);
        }
    }

    public ParticleSystem CheckActive(SingleSkillData data)
    {
        foreach(var pair in particles)
        {
            if (pair.Key == data.Skill_ID && !pair.Value.gameObject.activeSelf)
            {
                return pair.Value;
            }
        }

        var particle = Instantiate(data.SkillParticle, transform);
        particles.Add(new KeyValuePair<long, ParticleSystem>(data.Skill_ID, particle));

        return null;
    }

    public ParticleSystem CheckActive(MultiSkillData data)
    {
        foreach (var pair in particles)
        {
            if (pair.Key == data.Skill_ID && !pair.Value.gameObject.activeSelf)
            {
                return pair.Value;
            }
        }

        var particle = Instantiate(data.SkillParticle, transform);
        particles.Add(new KeyValuePair<long, ParticleSystem>(data.Skill_ID, particle));

        return null;
    }
}
