using UnityEngine;
using UnityEngine.AI;

public class AllyUnit : MonoBehaviour
{
    private NavMeshAgent agent;

    private Vector3 socket; //¿Ø¥÷¿Ã πËƒ°µ» º“ƒœ
    public Vector3 Center { get; set; } //¿Ø¥÷¿Ã ¿÷¥¬ ΩΩ∑‘ ¡ﬂæ”

    [SerializeField] private float attackInterval = 1f;
    private float attackTimer = 0f;

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

        if(transform.position == socket)
        {
            agent.isStopped = true;
        }
    }

    public void SetTarget(Vector3 targetSocket)
    {
        socket = targetSocket;

        agent.SetDestination(socket);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Center, 1f);
    }

}
