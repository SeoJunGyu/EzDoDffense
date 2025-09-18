using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyUnit : MonoBehaviour, IDamagable
{
    [SerializeField] private float arriveTolerance = 0.1f;

    [SerializeField] private float maxHealth = 100f;
    private NavMeshAgent agent;
    private int deffense = 1;
    private EnemyTypes defType;
    public EnemyData Data { get; private set; }

    private Vector3 target;
    private Vector3[] wayPoints;
    private int CurrentWayIndex = 0;

    public Slider healthSlider;
    public Canvas canvas;

    public float Health { get; private set; }
    public bool IsDead { get; private set; }

    public event Action OnDeath;

    [SerializeField] private float adventageDamageRate = 1.2f;
    [SerializeField] private float disAdventageDamageRate = 0.8f;
    [SerializeField] private int addGold = 5;

    private void OnEnable()
    {
        IsDead = false;
        Health = maxHealth;

        healthSlider.gameObject.SetActive(true);
        UpdateHealthBar();

        canvas.gameObject.SetActive(false);
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
            Debug.Log($"유리 상성 -> {damage} -> {damage * adventageDamageRate}");
            UpdateHealthBar();
        }
        else if(attackType == Data.Disadvangage)
        {
            Health -= baseDamage * disAdventageDamageRate;
            Debug.Log($"불리 상성 -> {damage} -> {damage * disAdventageDamageRate}");
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
}
