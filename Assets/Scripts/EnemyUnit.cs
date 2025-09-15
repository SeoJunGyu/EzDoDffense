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

    private Vector3 target;
    private Vector3[] wayPoints;
    private int CurrentWayIndex = 0;

    public Slider healthSlider;

    public float Health { get; private set; }
    public bool IsDead { get; private set; }

    public event Action OnDeath;

    private void OnEnable()
    {
        IsDead = false;
        Health = maxHealth;

        healthSlider.gameObject.SetActive(true);
        UpdateHealthBar();
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

    public void OnDamage(float damage)
    {
        if(Health < 0)
        {
            return;
        }

        Health -= damage;
        UpdateHealthBar();

        if(Health <= 0 && !IsDead)
        {
            //Die
            IsDead = true;

            healthSlider.gameObject.SetActive(false);

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
    }
}
