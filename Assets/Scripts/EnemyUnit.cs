using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnit : MonoBehaviour, IDamagable
{
    [SerializeField] private float arriveTolerance = 0.1f;
    [SerializeField] private float maxHealth = 100f;

    private NavMeshAgent agent;

    private Vector3 target;
    private Vector3[] wayPoints;
    private int CurrentWayIndex = 0;

    public float Health { get; private set; }
    public bool IsDead { get; private set; }

    public event Action OnDeath;

    private void OnEnable()
    {
        IsDead = false;
        Health = maxHealth;
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

    public void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        Health -= damage;
        if(Health <= 0 && !IsDead)
        {
            //Die
            OnDeath?.Invoke();
            IsDead = true;
        }
    }
}
