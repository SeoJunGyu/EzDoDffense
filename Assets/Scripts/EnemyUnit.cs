using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyUnit : MonoBehaviour, IDamagable, ISkillTarget
{
    [SerializeField] private float arriveTolerance = 0.1f;

    [SerializeField] private float maxHealth = 100f;
    private NavMeshAgent agent;
    private float deffense = 1f;
    private EnemyTypes defType;
    public EnemyData Data { get; private set; }

    private float beforeDeffense = 0;
    private float beforeSpeed = 0;
    private float multipliedDamage = 0f;

    private Vector3 target;
    private Vector3[] wayPoints;
    private int CurrentWayIndex = 0;

    public Slider healthSlider;
    public Canvas canvas;

    public float Health { get; private set; }
    public bool IsDead { get; private set; }

    public bool IsTargetable => gameObject.activeInHierarchy;

    private Transform effectAnchor;
    public Transform EffectAnchor => transform;

    private List<long> particles = new List<long>();
    public List<long> ActiveParticle { get => particles; }

    public event Action OnDeath;

    [SerializeField] private float adventageDamageRate = 1.2f;
    [SerializeField] private float disAdventageDamageRate = 0.8f;
    [SerializeField] private int addGold = 5;

    private Quaternion initialRotation;

    private Animator animator;

    private HashSet<long> activeDebuff = new HashSet<long>();

    private void OnEnable()
    {
        IsDead = false;
        Health = maxHealth;

        healthSlider.gameObject.SetActive(true);
        UpdateHealthBar();

        canvas.gameObject.SetActive(false);

        initialRotation = healthSlider.transform.rotation;
    }

    private void OnDisable()
    {
        activeDebuff.Clear();

        OnDeath = null;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        target = transform.position;
    }

    

    private void Update()
    {
        UpdateTrace();
    }

    private void LateUpdate()
    {
        healthSlider.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    public void UpdateTrace()
    {
        if(wayPoints == null || wayPoints.Length == 0)
        {
            return;
        }
        if (!agent.isOnNavMesh)
        {
            return;
        }
        if (agent.pathPending)
        {
            return;
        }

        if(agent.remainingDistance <= agent.stoppingDistance + arriveTolerance)
        {
            CurrentWayIndex = (CurrentWayIndex + 1) % wayPoints.Length;
            target = wayPoints[CurrentWayIndex];
            agent.SetDestination(target);
        }
    }

    public void SetTarget(Vector3[] wayPoint)
    {
        wayPoints = wayPoint;
        if(wayPoints == null || wayPoints.Length == 0)
        {
            return;
        }
        if (!agent.isOnNavMesh)
        {
            return;
        }

        target = wayPoints[CurrentWayIndex];
        agent.SetDestination(target);
    }

    public void OnDamage(float damage, AttackTypes attackType)
    {
        if(Health < 0)
        {
            return;
        }

        canvas.gameObject.SetActive(true);

        var baseDamage = damage * 100 / (100 + deffense);//기본 데미지

        if(Data.Advangage == AttackTypes.None) //영웅 장갑
        {
            Health -= baseDamage * disAdventageDamageRate;
            UpdateHealthBar();
        }
        else if(attackType == Data.Advangage)
        {
            Health -= baseDamage * adventageDamageRate;
            UpdateHealthBar();
        }
        else if(attackType == Data.Disadvangage)
        {
            Health -= baseDamage * disAdventageDamageRate;
            UpdateHealthBar();
        }
        else
        {
            Health -= baseDamage;
            UpdateHealthBar();
        }

        if (Health <= 0 && !IsDead)
        {
            //Die
            Variables.EnemyTotalCount--;

            Health = 0;
            Variables.Gold += addGold;
            canvas.gameObject.SetActive(false);

            IsDead = true;

            healthSlider.gameObject.SetActive(false);

            if(Variables.Boss == this)
            {
                if(Variables.Stage != 100)
                {
                    int gem = Variables.Stage <= 40 ? 1 :
                        Variables.Stage <= 70 ? 2 :
                        3;

                    Variables.Gem += gem;
                    Variables.Gold += gem * 100;
                    Variables.Stage++;
                }

                Variables.Boss = null;
            }

            if(Variables.SelectedEnemy == this)
            {
                Variables.SelectedEnemy = null;
            }

            OnDeath?.Invoke();
        }
    }

    public void UpdateHealthBar() => healthSlider.value = Health / maxHealth;

    public void Setup(EnemyData data)
    {
        maxHealth = data.Unit_HP;
        deffense = data.Unit_DEF;
        defType = (EnemyTypes)data.Unit_DEF_TYPE;
        agent.speed = data.Move_Speed;
        CurrentWayIndex = 0;
        Data = data;
    }

    public void ApplySingleSkill(SingleSkillData data, AllyUnit caster, ParticleSystem particle)
    {
        switch (data.Skill_Effect)
        {
            case 3:
                if (!activeDebuff.Add(data.Skill_ID))
                {
                    return;
                }
                beforeDeffense = deffense;
                deffense = deffense * (1 - data.Skill_Effect_Value / 100f);
                particles.Add(data.Skill_ID);

                Debug.Log($"방어력 변화 : {data.Skill_Name} / {beforeDeffense} / {deffense}");
                break;
            case 4:
                if (!activeDebuff.Add(data.Skill_ID))
                {
                    return;
                }
                beforeSpeed = agent.speed;
                agent.speed = agent.speed * (1 - data.Skill_Effect_Value / 100f);
                particles.Add(data.Skill_ID);

                Debug.Log($"이동속도 변화 : {data.Skill_Name} / {beforeSpeed} / {agent.speed}");
                break;
            case 5:
                var damage = caster.Damage * (data.Skill_Effect_Value / 100f);
                OnDamage(damage, caster.UnitType);
                particles.Add(data.Skill_ID);

                Debug.Log($"데미지 변화 : {data.Skill_Name} / {caster.Damage} / {damage}");
                break;
        }
    }

    public void ApplyMultiSkill(MultiSkillData data, AllyUnit caster, ParticleSystem particle)
    {
        switch (data.Skill_Effect_1)
        {
            case 5:
                var damage = caster.Damage * (data.Skill_Effect_Value_1 / 100f);
                OnDamage(damage, caster.UnitType);
                particles.Add(data.Skill_ID);

                Debug.Log($"데미지 변화 : {data.Skill_Name} / {caster.Damage} / {damage}");
                break;
        }

        switch (data.Skill_Effect_2)
        {
            case 3:
                if (!gameObject.activeSelf || IsDead)
                {
                    return;
                }
                if (!activeDebuff.Add(data.Skill_ID))
                {
                    return;
                }
                beforeDeffense = deffense;
                deffense = deffense * (1 - data.Skill_Effect_Value_2 / 100f);
                particles.Add(data.Skill_ID);

                StartCoroutine(DeffenseCoroutine(data.Skill_ID, data.Skill_Duration_2));

                Debug.Log($"방어력 변화 : {data.Skill_Name} / {beforeDeffense} / {deffense}");
                break;
            case 4:
                if(!gameObject.activeSelf || IsDead)
                {
                    return;
                }
                if (!activeDebuff.Add(data.Skill_ID))
                {
                    return;
                }

                beforeSpeed = agent.speed;
                agent.speed = agent.speed * (1 - data.Skill_Effect_Value_2 / 100f);
                particles.Add(data.Skill_ID);

                StartCoroutine(SpeedCoroutine(data.Skill_ID, data.Skill_Duration_2));

                Debug.Log($"이동속도 변화 : {data.Skill_Name} / {beforeSpeed} / {agent.speed}");
                break;
            case 6:
                if (!CanControlAgent)
                {
                    break;
                }
                if (!activeDebuff.Add(data.Skill_ID))
                {
                    return;
                }

                agent.isStopped = true;
                if(animator != null)
                {
                    animator.speed = 0f;
                }
                StartCoroutine(StunCoroutine(data.Skill_ID, data.Skill_Duration_2));

                Debug.Log($"기절 {data.Skill_Name} / {data.Skill_Duration_2}초");
                break;
        }
    }

    private bool CanControlAgent => agent != null && agent.enabled && gameObject.activeSelf && agent.isOnNavMesh;

    private IEnumerator StunCoroutine(long id, float duration)
    {
        yield return new WaitForSeconds(duration);

        agent.isStopped = false;
        if (animator != null) animator.speed = 1f; // 원래 값 복원

        activeDebuff.Remove(id);
    }

    private IEnumerator SpeedCoroutine(long id, float duration)
    {
        Debug.Log($"이동속도 지속시간 {duration}");
        yield return new WaitForSeconds(duration);
        agent.speed = beforeSpeed;
        Debug.Log($"이동속도 되돌아옴");
        activeDebuff.Remove(id);
    }

    private IEnumerator DeffenseCoroutine(long id, float duration)
    {
        Debug.Log($"이동속도 지속시간 {duration}");
        yield return new WaitForSeconds(duration);
        deffense = beforeDeffense;
        Debug.Log($"이동속도 되돌아옴");
        activeDebuff.Remove(id);
    }
}
