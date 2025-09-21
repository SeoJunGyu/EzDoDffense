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

    public void SetSkillEffect()
    {
        if (SingleSkill != null)
        {
            skill1 = SingleSkill.SkillParticle;
            var particleSkill1 = Instantiate(skill1, transform);
            OnDestroyParticle += () => Destroy(particleSkill1);
        }

        if(MultiSkill != null)
        {
            skill2 = MultiSkill.SkillParticle;
            var particleSkill2 = Instantiate(skill2, transform);
            OnDestroyParticle += () => Destroy(particleSkill2);
        }
    }
}
