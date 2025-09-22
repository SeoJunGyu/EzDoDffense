using NUnit.Framework;
using System;
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
    private int deffense = 1;
    private EnemyTypes defType;
    public EnemyData Data { get; private set; }

    private int beforeDeffense = 0;
    private int beforeSpeed = 0;
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
    public Transform EffectAnchor => effectAnchor != null ? effectAnchor : transform;

    public event Action OnDeath;

    [SerializeField] private float adventageDamageRate = 1.2f;
    [SerializeField] private float disAdventageDamageRate = 0.8f;
    [SerializeField] private int addGold = 5;

    private Quaternion initialRotation;

    public List<long> Buff = new List<long>();

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
        OnDeath = null;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        target = transform.position;
    }

    

    private void Update()
    {
        UpdateTrace();
    }

    private void LateUpdate()
    {
        healthSlider.transform.rotation = initialRotation;
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

        var baseDamage = damage * 100 / (100 + deffense);//±âº» µ¥¹ÌÁö

        if(Data.Advangage == AttackTypes.None) //¿µ¿õ Àå°©
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
                Variables.Stage++;
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

    public void ApplySingleSkill(SingleSkillData data, AllyUnit caster)
    {
        if (Buff.Contains(data.Skill_ID))
        {
            return;
        }

        switch (data.Skill_Effect)
        {
            case 3:
                beforeDeffense = deffense;
                deffense = deffense * (1 - data.Skill_Effect_Value / 100);
                Buff.Add(data.Skill_ID);
                var Deffenseskill = caster.singleSkillParticle.GetComponent<Skill>();
                Deffenseskill.TraceTarget(gameObject);
                break;
            case 4:
                beforeSpeed = (int)agent.speed;
                agent.speed = agent.speed * (1 - data.Skill_Effect_Value / 100);
                Buff.Add(data.Skill_ID);
                var Speedskill = caster.singleSkillParticle.GetComponent<Skill>();
                Speedskill.TraceTarget(gameObject);
                break;
            case 5:
                var damage = caster.Damage * (data.Skill_Effect_Value / 100);
                OnDamage(caster.Damage, caster.UnitType);
                break;
        }
    }

    public void ApplyMultiSkill(MultiSkillData data, AllyUnit caster)
    {
        throw new NotImplementedException();
    }
}
