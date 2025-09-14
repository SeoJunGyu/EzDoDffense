using UnityEngine;
using UnityEngine.AI;

public class AllyUnit : MonoBehaviour
{
    private NavMeshAgent agent;

    private Vector3 socket; //유닛이 배치된 소켓
    public Vector3 Center { get; set; } //유닛이 있는 슬롯 중앙

    [SerializeField] private float attackInterval = 1f;
    private float attackTimer = 0f;
    private float damage = 10f;
    [SerializeField] private float range = 2f;
    [SerializeField] private LayerMask enemyMask;
    private EnemyUnit target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        UpdateMove();
        UpdateAttack();
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

    public void UpdateAttack()
    {
        attackTimer += Time.deltaTime;
        if(target != null)
        {
            if(!target.gameObject.activeSelf || 
                target.IsDead || 
                Vector3.Distance(target.transform.position, Center) > range)
            {
                target = null;
                return;
            }

            if (attackTimer > attackInterval)
            {
                target.OnDamage(damage);
                attackTimer = 0f;
            }

            return;
        }

        //타겟 발견 못했을 경우
        var hits = Physics.OverlapSphere(Center, range, enemyMask, QueryTriggerInteraction.Ignore);
        foreach(var enemyCollider in hits)
        {
            var enemy = enemyCollider.GetComponent<EnemyUnit>();
            if(enemy != null && enemy.gameObject.activeSelf && !enemy.IsDead)
            {
                target = enemy;
                break;
            }
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
        Gizmos.DrawWireSphere(Center, range);
    }

}
