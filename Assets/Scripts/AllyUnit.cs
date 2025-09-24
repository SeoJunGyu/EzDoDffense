using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AllyUnit : MonoBehaviour, ISkillTarget
{
    private NavMeshAgent agent;

    private Vector3 socket; //유닛이 배치된 소켓
    public Vector3 Center { get; set; } //유닛이 있는 슬롯 중앙

    [SerializeField] private float attackInterval = 1f;
    private float beforeAttackSpeed = 0f;
    private float beforeDamage = 0f;
    private float attackSpeed = 0f;
    private float attackTimer = 0f;
    private float damage = 10f;
    [SerializeField] private float range = 2f;
    private int grade;
    private AttackTypes unitType;
    private long skill1;
    private long skill2;

    public long Skill1
    {
        get
        {
            return skill1;
        }
    }

    public long Skill2
    {
        get
        {
            return skill2;
        }
    }

    public float Damage
    {
        get
        {
            return damage;
        }
    }

    public float AtkSpeed { get => attackSpeed; }

    public AttackTypes UnitType
    {
        get
        {
            return unitType;
        }
    }

    public int Grade { get => grade; }

    [SerializeField] private LayerMask enemyMask;
    private EnemyUnit target;
    public EnemyUnit Target { get => target; }

    private bool IsMove { get; set; }

    public event Action OnSynthesis;
    public event Action OnDespawned;

    private Animator animator;

    public bool IsTargetable => gameObject.activeInHierarchy;

    private Transform effectAnchor;
    public Transform EffectAnchor => transform;

    private List<long> particles = new List<long>();
    public List<long> ActiveParticle { get => particles; }

    public List<EnemyUnit> findSkillTarget = new List<EnemyUnit>();

    private HashSet<long> activeBuff = new HashSet<long>(); //유닛에 활성화된 버프

    private void OnDisable()
    {
        OnDespawned?.Invoke();

        activeBuff.Clear();

        OnSynthesis = null;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        UpdateMove();
        UpdateAttack();

        if (target)
        {
            Vector3 dir = target.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 180f * Time.deltaTime);
        }
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

        if (agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, 0f))
        {
            agent.isStopped = true;
            agent.ResetPath();
            IsMove = false;

            SafeSetBoolAnimator("IsMoved", false);
        }
    }

    public void UpdateAttack()
    {
        if (IsMove)
        {
            attackTimer = 0f;

            SafeSetBoolAnimator("IsMoved", true);
            return;
        }

        attackTimer += Time.deltaTime;
        if(target != null)
        {
            if(!target.gameObject.activeSelf || 
                target.IsDead || 
                Vector3.Distance(target.transform.position, Center) > range)
            {
                target = null;
                SafeSetBoolAnimator("IsTarget", false);

                return;
            }

            SafeSetBoolAnimator("IsTarget", true);

            if (attackTimer > attackInterval)
            {
                target.OnDamage(damage, unitType);

                if(skill1 != 0)
                {
                    SkillManager.Instance.ExecuteSingleSkill(this);
                }
                if(skill2 != 0)
                {
                    SkillManager.Instance.ExecuteMultiSkill(this);
                }

                attackTimer = 0f;
            }

            return;
        }

        var skillhits = Physics.OverlapSphere(Center, range, enemyMask, QueryTriggerInteraction.Ignore);
        foreach (var enemyCollider in skillhits)
        {
            var enemy = enemyCollider.GetComponent<EnemyUnit>();
            if (enemy != null && enemy.gameObject.activeSelf && !enemy.IsDead)
            {
                if (findSkillTarget.Contains(enemy))
                {
                    continue;
                }
                else
                {
                    findSkillTarget.Add(enemy);
                }
            }
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

        IsMove = true;

        agent.SetDestination(socket);
    }

    public void Setup(AllyData data, int gradeUpgrade, int typeUpgrade)
    {
        grade = data.Unit_Grade;
        unitType = (AttackTypes)data.Unit_Type;

        //damage = data.Unit_ATK;
        damage = data.Unit_ATK + UpgradeDB.CalcGradeUpDamage(data.Unit_ATK, grade, gradeUpgrade) + UpgradeDB.CalcTypeUpDamage(data.Unit_ATK, typeUpgrade);

        attackSpeed = data.Unit_ATK_SPD + UpgradeDB.CalcGradeUpAtkSpeed(data.Unit_ATK_SPD, grade, gradeUpgrade) + UpgradeDB.CalcTypeUpAtkSpeed(data.Unit_ATK_SPD, typeUpgrade);
        Debug.Log($"{data.Unit_ATK_SPD} / {attackSpeed}");

        attackInterval = 1f / data.Unit_ATK_SPD;
        range = data.Unit_ATK_RNG + 4f; //4f : 최소가 1f이니까
        agent.speed = data.Unit_Move_Speed;
        skill1 = data.Unit_Skill_1;
        skill2 = data.Unit_Skill_2;

        animator = GetComponentInChildren<Animator>();

    }

    public void SynthesisAfter()
    {
        animator = null;

        OnSynthesis?.Invoke(); //비주얼 모델 제거 후 프리펩 비활성화
    }

    private void SafeSetBoolAnimator(string name, bool value)
    {
        if(animator == null)
        {
            return;
        }
        if (!animator)
        {
            return;
        }
        if (!animator.isActiveAndEnabled)
        {
            return;
        }

        animator.SetBool(name, value);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Center, range);
    }

    public void ApplySingleSkill(SingleSkillData data, AllyUnit caster, ParticleSystem particle)
    {
        if (!activeBuff.Add(data.Skill_ID))
        {
            return;
        }

        switch (data.Skill_Effect)
        {
            case 1:
                beforeDamage = damage;
                damage = damage * (1 + data.Skill_Effect_Value / 100f);
                break;
            case 2:
                beforeAttackSpeed = attackSpeed;
                attackInterval = 1f / (attackSpeed * (1 + data.Skill_Effect_Value / 100f));
                break;
        }
    }

    public void ApplyMultiSkill(MultiSkillData data, AllyUnit caster, ParticleSystem particle)
    {
        return;
    }
}
