using System;
using UnityEngine;

public class SkillAgent : MonoBehaviour
{
    public SingleSkillData SingleSkill { get; set; }
    public MultiSkillData MultiSkill { get; set; }
    private ParticleSystem skill1;
    private ParticleSystem skill2;

    public event Action OnDestroyParticle;

    private void OnDisable()
    {
        OnDestroyParticle?.Invoke();

        OnDestroyParticle = null;
    }

    public void SetSingleSkillEffect()
    {
        if (SingleSkill != null)
        {
            skill1 = SingleSkill.SkillParticle;
            var particleSkill1 = Instantiate(skill1, transform);
            OnDestroyParticle += () => Destroy(particleSkill1);
        }
    }

    public void SetMultiSkillEffect()
    {
        if (MultiSkill != null)
        {
            skill2 = MultiSkill.SkillParticle;
            var particleSkill2 = Instantiate(skill2, transform);
            OnDestroyParticle += () => Destroy(particleSkill2);
        }
    }

    private bool CheckSkillRandom()
    {
        if(SingleSkill.Skill_Random >= 100)
        {
            return true;
        }

        return UnityEngine.Random.Range(0, 100) <= SingleSkill.Skill_Random;
    }

    private void CheckSkillTarget()
    {
        //1 - 자기자신 / 2 - 단일적군 / 3 - 범위적군 / 4 - 아군전체 / 5 - 적군전체
        switch (SingleSkill.Skill_Target)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
        }
    }

    public void UpdateSingleSkill()
    {
        if(SingleSkill == null)
        {
            return;
        }

        if (!CheckSkillRandom())
        {
            return;
        }



        //발동 확률 -> 스킬 대상 확인 -> 스킬 범위 확인 -> 스킬 적용 가능 여부 확인 -> 수치 적용 -> 스킬 지속시간 부여 -> 이펙트 플레이
    }
}
