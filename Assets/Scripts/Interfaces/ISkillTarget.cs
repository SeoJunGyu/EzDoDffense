using UnityEngine;

public interface ISkillTarget
{
    public bool IsTargetable { get; }
    public Transform EffectAnchor { get; }
    public void ApplySingleSkill(SingleSkillData data, AllyUnit caster);
    public void ApplyMultiSkill(MultiSkillData data, AllyUnit caster);
}
