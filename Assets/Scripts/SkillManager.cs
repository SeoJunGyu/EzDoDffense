using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public SkillAgent skillBase;

    private List<SkillAgent> particles = new List<SkillAgent>();

    public SkillAgent CreateSkillBase(long singleSkillId, long multiSkillId)
    {
        SkillAgent Base = null;
        foreach(var skill in particles)
        {
            if (!skill.gameObject.activeSelf)
            {
                Base = skill;
                Base.transform.position = transform.position;
                Base.transform.rotation = transform.rotation;
                break;
            }
        }

        if(Base == null)
        {
            Base = Instantiate(skillBase);
            particles.Add(Base);
        }

        Base.SingleSkill = DataTableManager.SingleSkillTable.Get(singleSkillId);
        if(multiSkillId != 0)
        {
            Base.MultiSkill = DataTableManager.MultiSkillTable.Get(multiSkillId);
        }

        Base.SetSkillEffect();

        return Base;
    }
}
