using UnityEngine;
using UnityEngine.AI;

public class AllyUnit : MonoBehaviour
{
    private NavMeshAgent agent;

    private Vector3 target;

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

    public void SetTarget(Vector3 targetSocket)
    {
        target = targetSocket;
        agent.SetDestination(target);
    }

}
