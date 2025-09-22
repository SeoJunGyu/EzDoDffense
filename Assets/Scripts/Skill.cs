using UnityEngine;

public class Skill : MonoBehaviour
{


    public void TraceTarget(GameObject target)
    {
        transform.position = target.transform.position;
    }
}
