using UnityEngine;
using UnityEngine.AI;

public class AllyUnit : MonoBehaviour
{
    private NavMeshAgent agent;

    private Vector3 target;
    private Vector3 center;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        UpdateMove();
    }

    public void UpdateMove()
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }
        if (agent.pathPending)
        {
            return;
        }

        if(transform.position == target)
        {
            agent.isStopped = true;
        }
    }

    public void SetTarget(Vector3 targetSocket, Vector3 targetCenter)
    {
        target = targetSocket;
        center = targetCenter;

        agent.SetDestination(target);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(center, 1f);
    }

}
