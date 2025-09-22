using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillTarget
{
    public bool IsTargetable { get; }
    public Transform EffectAnchor { get; }
    public void ApplySingleSkill(SingleSkillData data, AllyUnit caster, ParticleSystem particle);
    public void ApplyMultiSkill(MultiSkillData data, AllyUnit caster, ParticleSystem particle);
    public List<long> ActiveParticle { get; }
}
