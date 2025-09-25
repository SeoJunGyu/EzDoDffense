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

    public event Action OnDeath;
    public event Action OnDisableUnit;

    [SerializeField] private float adventageDamageRate = 1.2f;
    [SerializeField] private float disAdventageDamageRate = 0.8f;
    [SerializeField] private int addGold = 5;

    private Quaternion initialRotation;

    private Animator animator;

    private List<long> particles = new List<long>();
    public List<long> ActiveParticle => particles;

    public Dictionary<long, float> ActiveBuffValue = new Dictionary<long, float>();
    public Dictionary<long, ParticleSystem> ActiveBuffParticle = new Dictionary<long, ParticleSystem>();

    public string UnitName { get; private set; }

    private void OnEnable()
    {
        IsDead = false;
        Health = maxHealth;

        healthSlider.gameObject.SetActive(true);
        UpdateHealthBar();

        canvas.gameObject.SetActive(false);

        initialRotation = healthSlider.transform.rotation;

        ActiveBuffValue.Clear();
        ActiveBuffParticle.Clear();
        particles.Clear();
    }

    private void OnDisable()
    {
        particles.Clear();

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

            AudioManager.Instance.PlayDead();

            OnDisableUnit?.Invoke();

            OnDeath?.Invoke();
        }
    }

    public void UpdateHealthBar() => healthSlider.value = Health / maxHealth;

    public void Setup(EnemyData data)
    {
        UnitName = data.Unit_Name;
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
                if (particles.Contains(data.Skill_ID))
                {
                    break;
                }
                particles.Add(data.Skill_ID);
                beforeDeffense = deffense;
                var deffenseDebuffValue = 1 - data.Skill_Effect_Value / 100f;
                deffense = deffense * deffenseDebuffValue;

                if (!ActiveBuffValue.ContainsKey(data.Skill_ID))
                {
                    ActiveBuffValue.Add(data.Skill_ID, deffenseDebuffValue);
                    ActiveBuffParticle.Add(data.Skill_ID, particle);
                }
                else
                {
                    ActiveBuffValue[data.Skill_ID] = deffenseDebuffValue;
                    ActiveBuffParticle[data.Skill_ID] = particle;
                }

                OnDisableUnit += () => SkillManager.Instance.ReturnParticle(data.Skill_ID, particle);

                break;
            case 4:
                if (particles.Contains(data.Skill_ID))
                {
                    break;
                }
                particles.Add(data.Skill_ID);
                beforeSpeed = agent.speed;
                var atkSpeedDebuffValue = 1 - data.Skill_Effect_Value / 100f;
                agent.speed = agent.speed * atkSpeedDebuffValue;

                if (!ActiveBuffValue.ContainsKey(data.Skill_ID))
                {
                    ActiveBuffValue.Add(data.Skill_ID, atkSpeedDebuffValue);
                    ActiveBuffParticle.Add(data.Skill_ID, particle);
                }
                else
                {
                    ActiveBuffValue[data.Skill_ID] = atkSpeedDebuffValue;
                    ActiveBuffParticle[data.Skill_ID] = particle;
                }

                OnDisableUnit += () => SkillManager.Instance.ReturnParticle(data.Skill_ID, particle);

                break;
            case 5:
                var damage = caster.Damage * (data.Skill_Effect_Value / 100f);
                OnDamage(damage, caster.UnitType);
                SkillManager.Instance.StartCoroutine(SkillManager.Instance.ReturnWhenDead(data.Skill_ID, particle));

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
                SkillManager.Instance.StartCoroutine(SkillManager.Instance.ReturnWhenDead(data.Skill_ID, particle));

                break;
        }

        switch (data.Skill_Effect_2)
        {
            case 3:
                if (!gameObject.activeSelf || IsDead)
                {
                    break;
                }
                if (particles.Contains(data.Skill_ID))
                {
                    break;
                }
                particles.Add(data.Skill_ID);
                beforeDeffense = deffense;
                deffense = deffense * (1 - data.Skill_Effect_Value_2 / 100f);
                SkillManager.Instance.StartCoroutine(SkillManager.Instance.ReturnWhenDead(data.Skill_ID, particle));

                StartCoroutine(DeffenseCoroutine(data.Skill_ID, data.Skill_Duration_2));

                break;
            case 4:
                if(!gameObject.activeSelf || IsDead)
                {
                    break;
                }
                if (particles.Contains(data.Skill_ID))
                {
                    break;
                }
                particles.Add(data.Skill_ID);
                beforeSpeed = agent.speed;
                agent.speed = agent.speed * (1 - data.Skill_Effect_Value_2 / 100f);
                SkillManager.Instance.StartCoroutine(SkillManager.Instance.ReturnWhenDead(data.Skill_ID, particle));

                StartCoroutine(SpeedCoroutine(data.Skill_ID, data.Skill_Duration_2));

                break;
            case 6:
                if (!CanControlAgent)
                {
                    break;
                }
                if (particles.Contains(data.Skill_ID))
                {
                    break;
                }
                particles.Add(data.Skill_ID);
                agent.isStopped = true;
                if(animator != null)
                {
                    animator.speed = 0f;
                }
                SkillManager.Instance.StartCoroutine(SkillManager.Instance.ReturnWhenDead(data.Skill_ID, particle));
                StartCoroutine(StunCoroutine(data.Skill_ID, data.Skill_Duration_2));

                break;
        }
    }

    private bool CanControlAgent => agent != null && agent.enabled && gameObject.activeSelf && agent.isOnNavMesh;

    private IEnumerator StunCoroutine(long id, float duration)
    {
        yield return new WaitForSeconds(duration);

        agent.isStopped = false;
        if (animator != null) animator.speed = 1f; // 원래 값 복원

        particles.Remove(id);
    }

    private IEnumerator SpeedCoroutine(long id, float duration)
    {
        yield return new WaitForSeconds(duration);
        agent.speed = beforeSpeed;
        particles.Remove(id);
    }

    private IEnumerator DeffenseCoroutine(long id, float duration)
    {
        yield return new WaitForSeconds(duration);
        deffense = beforeDeffense;
        particles.Remove(id);
    }

    public void ResetBuffValue(long skillId, int Skill_Effect)
    {
        if (!ActiveBuffValue.TryGetValue(skillId, out var mult))
        {
            return;
        }
        if (mult == 0f || float.IsNaN(mult))
        {
            return;
        }

        ActiveBuffParticle.TryGetValue(skillId, out var par);

        ActiveBuffValue.Remove(skillId);
        ActiveBuffParticle.Remove(skillId);

        switch (Skill_Effect)
        {
            case 3:
                deffense = deffense / mult;
                break;
            case 4:
                agent.speed = agent.speed / mult;
                break;
        }

        if (par != null)
        {
            SkillManager.Instance.ReturnParticle(skillId, par);
        }

        particles.Remove(skillId);
    }
}
