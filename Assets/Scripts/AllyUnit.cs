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
    public Dictionary<long, float> ActiveBuffValue = new Dictionary<long, float>();
    public Dictionary<long, ParticleSystem> ActiveBuffParticle = new Dictionary<long, ParticleSystem>();

    private Animator animator;

    public bool IsTargetable => gameObject.activeInHierarchy;

    private Transform effectAnchor;
    public Transform EffectAnchor => transform;

    private List<long> particles = new List<long>();
    public List<long> ActiveParticle { get => particles; }

    public List<EnemyUnit> findSkillTarget = new List<EnemyUnit>();

    private SingleSkillData singleData;
    private MultiSkillData multiData;

    private void OnEnable()
    {
        ActiveBuffValue.Clear();
        ActiveBuffParticle.Clear();
        particles.Clear();
    }

    private void OnDisable()
    {
        particles.Clear();

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

        if (skill1 != 0 && singleData.Skill_Area == 1)
        {
            SkillManager.Instance.ExecuteSingleSkill(this);
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

                if (skill1 != 0)
                {
                    SkillManager.Instance.ExecuteSingleSkill(this);
                }
                if (skill2 != 0)
                {
                    SkillManager.Instance.ExecuteMultiSkill(this);
                }

                attackTimer = 0f;
            }

            return;
        }

        var skillhits = Physics.OverlapSphere(Center, range, enemyMask, QueryTriggerInteraction.Ignore);
        var current = new List<EnemyUnit>();
        foreach (var enemyCollider in skillhits)
        {
            var enemy = enemyCollider.GetComponent<EnemyUnit>();
            if (enemy != null && enemy.gameObject.activeSelf && !enemy.IsDead)
            {
                current.Add(enemy);
                if (!findSkillTarget.Contains(enemy))
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

        for(int i = findSkillTarget.Count - 1; i >= 0; i--)
        {
            if (!current.Contains(findSkillTarget[i]))
            {
                findSkillTarget.RemoveAt(i);
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

        attackInterval = 1f / attackSpeed;
        range = data.Unit_ATK_RNG + 4f; //4f : 최소가 1f이니까
        agent.speed = data.Unit_Move_Speed;
        skill1 = data.Unit_Skill_1;
        skill2 = data.Unit_Skill_2;

        singleData = skill1 != 0 ? SkillManager.Instance.GetSingleData(skill1) : null;
        multiData = skill2 != 0 ? SkillManager.Instance.GetMultiData(skill2) : null;

        animator = GetComponentInChildren<Animator>();

    }

    public void SynthesisAfter()
    {
        animator = null;

        OnDespawned?.Invoke();

        if(singleData.Skill_Area == 1)
        {
            SkillManager.Instance.ReturnActiveEffect(singleData);
        }

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
        if (particles.Contains(data.Skill_ID))
        {
            return;
        }

        switch (data.Skill_Effect)
        {
            case 1:
                particles.Add(data.Skill_ID);
                beforeDamage = damage;
                var damageBuffValue = 1 + data.Skill_Effect_Value / 100f;
                damage = damage * damageBuffValue;

                if (!ActiveBuffValue.ContainsKey(data.Skill_ID))
                {
                    ActiveBuffValue.Add(data.Skill_ID, damageBuffValue);
                    ActiveBuffParticle.Add(data.Skill_ID, particle);
                }
                else
                {
                    ActiveBuffValue[data.Skill_ID] = damageBuffValue;
                    ActiveBuffParticle[data.Skill_ID] = particle;
                }

                OnDespawned += () => SkillManager.Instance.ReturnParticle(data.Skill_ID, particle);

                break;
            case 2:
                particles.Add(data.Skill_ID);
                beforeAttackSpeed = attackSpeed;
                var speedBuffValue = 1 + data.Skill_Effect_Value / 100f;
                attackSpeed = attackSpeed * speedBuffValue;
                attackInterval = 1f / attackSpeed;

                if (!ActiveBuffValue.ContainsKey(data.Skill_ID))
                {
                    ActiveBuffValue.Add(data.Skill_ID, speedBuffValue);
                    ActiveBuffParticle.Add(data.Skill_ID, particle);
                }
                else
                {
                    ActiveBuffValue[data.Skill_ID] = speedBuffValue;
                    ActiveBuffParticle[data.Skill_ID] = particle;
                }

                OnDespawned += () => SkillManager.Instance.ReturnParticle(data.Skill_ID, particle);

                break;
        }
    }

    public void ApplyMultiSkill(MultiSkillData data, AllyUnit caster, ParticleSystem particle)
    {
        return;
    }

    public void ResetBuffValue(long skillId, int Skill_Effect)
    {
        if(!ActiveBuffValue.TryGetValue(skillId, out var mult))
        {
            return;
        }
        if(mult == 0f || float.IsNaN(mult))
        {
            return;
        }

        ActiveBuffParticle.TryGetValue(skillId, out var par);

        ActiveBuffValue.Remove(skillId);
        ActiveBuffParticle.Remove(skillId);

        switch (Skill_Effect)
        {
            case 1:
                damage = damage / mult;
                break;
            case 2:
                attackSpeed = attackSpeed / mult;
                attackInterval = 1f / attackSpeed;
                break;
        }

        if(par != null)
        {
            SkillManager.Instance.ReturnParticle(skillId, par);
        }
        
        particles.Remove(skillId);
    }
}
