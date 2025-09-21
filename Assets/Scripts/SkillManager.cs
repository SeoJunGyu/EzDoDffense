using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public GameObject skillBase;

    private void Awake()
    {
        var data = DataTableManager.SingleSkillTable.Get(111005411);

        Instantiate(data.SkillParticle, skillBase.transform);
    }
}
