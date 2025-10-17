using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;

public class EnemyUnit : MonoBehaviour, IDamagable, ISkillTarget
{
    [SerializeField] private float arriveTolerance = 0.1f;

    [SerializeField] private float maxHealth = 100f;
    
    private float deffense = 1f;
    private EnemyTypes defType;
    public EnemyData Data { get; private set; }

    private float beforeDeffense = 0;
    private float beforeSpeed = 0;
    private float moveSpeed = 0f;

    private Vector3 target;
    private Vector3[] wayPoints;
    private int CurrentWayIndex = 0;

    public Slider healthSlider;
    public Canvas canvas;

    public float Health { get; private set; }
    public bool IsDead { get; private set; }

    public bool IsTargetable => gameObject.activeInHierarchy;

    [SerializeField] private Transform effectAnchor;
    public Transform EffectAnchor => effectAnchor;

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
    //public Dictionary<long, ParticleSystem> ActiveBuffParticle = new Dictionary<long, ParticleSystem>();

    public string UnitName { get; private set; }

    private bool paused;

    private void OnEnable()
    {
        IsDead = false;
        Health = maxHealth;

        healthSlider.gameObject.SetActive(true);
        UpdateHealthBar();

        canvas.gameObject.SetActive(false);

        initialRotation = healthSlider.transform.rotation;

        ActiveBuffValue.Clear();
        //ActiveBuffParticle.Clear();
        particles.Clear();

        paused = false;
    }

    private void OnDisable()
    {
        ActiveBuffValue.Clear();
        //ActiveBuffParticle.Clear();
        particles.Clear();

        OnDeath = null;
        OnDisableUnit = null;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        target = transform.position;
    }

    private void Update()
    {
        RailMoveUpdate();
    }

    private void LateUpdate()
    {
        healthSlider.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    public void RailMoveUpdate()
    {
        if (paused)
        {
            return;
        }
        if(wayPoints == null || wayPoints.Length == 0)
        {
            return;
        }

        float remaining = moveSpeed * Time.deltaTime;
        Vector3 pos = transform.position;

        int safety = 0;
        while(remaining > 0f && safety++ < 8)
        {
            Vector3 to = wayPoints[CurrentWayIndex];
            Vector3 delta = to - pos;
            float dist = delta.magnitude;

            //코너
            if(dist <= arriveTolerance)
            {
                pos = to;

                int next = (CurrentWayIndex + 1) % wayPoints.Length;

                Vector3 dir = wayPoints[next] - to;
                if(dir.sqrMagnitude > 0.0001f)
                {
                    transform.rotation = Quaternion.LookRotation(dir);
                }

                CurrentWayIndex = next;
                continue;
            }

            float step = Mathf.Min(remaining, dist);
            pos += (delta / dist) * step;
            remaining -= step;
        }

        transform.position = pos;
    }

    public void SetTarget(Vector3[] wayPoint)
    {
        wayPoints = wayPoint;

        CurrentWayIndex = 0;

        if(wayPoints == null || wayPoints.Length == 0)
        {
            return;
        }

        Vector3 dir = wayPoints[0] - transform.position;
        if(dir.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
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
        }
        else if(attackType == Data.Advangage)
        {
            Health -= baseDamage * adventageDamageRate;
        }
        else if(attackType == Data.Disadvangage)
        {
            Health -= baseDamage * disAdventageDamageRate;
        }
        else
        {
            Health -= baseDamage;
        }

        UpdateHealthBar();

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
                    if(gem >= 3)
                    {
                        Variables.Gold += gem * 100 + 100;
                    }
                    else
                    {
                        Variables.Gold += gem * 100;
                    }

                        Variables.Stage++;
                }

                transform.localScale = Vector3.one;

                Vector3 pos = new Vector3(0f, 2f, 0f);
                canvas.transform.localPosition = pos;

                Variables.Boss = null;

                Handheld.Vibrate();
            }

            if(Variables.SelectedEnemy == this)
            {
                Variables.SelectedEnemy = null;
            }

            AudioManager.Instance.PlayDead(transform.position);

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
        moveSpeed = data.Move_Speed;
        CurrentWayIndex = 0;
        Data = data;

        particles.Clear();
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
                    //ActiveBuffParticle.Add(data.Skill_ID, particle);
                }
                else
                {
                    ActiveBuffValue[data.Skill_ID] = deffenseDebuffValue;
                    //ActiveBuffParticle[data.Skill_ID] = particle;
                }

                OnDisableUnit += () => SkillManager.Instance.ReturnParticle(data.Skill_ID, particle);

                break;
            case 4:
                if (particles.Contains(data.Skill_ID))
                {
                    break;
                }
                particles.Add(data.Skill_ID);
                beforeSpeed = moveSpeed;
                var atkSpeedDebuffValue = 1 - data.Skill_Effect_Value / 100f;
                moveSpeed *= atkSpeedDebuffValue;

                if (!ActiveBuffValue.ContainsKey(data.Skill_ID))
                {
                    ActiveBuffValue.Add(data.Skill_ID, atkSpeedDebuffValue);
                    //ActiveBuffParticle.Add(data.Skill_ID, particle);
                }
                else
                {
                    ActiveBuffValue[data.Skill_ID] = atkSpeedDebuffValue;
                    //ActiveBuffParticle[data.Skill_ID] = particle;
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
        if (!gameObject.activeSelf || IsDead)
        {
            return;
        }

        var damage = caster.Damage * (data.Skill_Effect_Value_1 / 100f);
        OnDamage(damage, caster.UnitType);
        
        if (particles.Contains(data.Skill_ID))
        {
            return;
        }
        //Debug.Log($"{data.Skill_Name}");
        switch (data.Skill_Effect_2)
        {
            case 3:
                particles.Add(data.Skill_ID);
                beforeDeffense = deffense;
                deffense = deffense * (1 - data.Skill_Effect_Value_2 / 100f);
                SkillManager.Instance.StartCoroutine(SkillManager.Instance.ReturnWhenDead(data.Skill_ID, particle));

                SkillManager.Instance.StartCoroutine(DeffenseCoroutine(data.Skill_ID, data.Skill_Duration_2));

                break;
            case 4:
                particles.Add(data.Skill_ID);
                beforeSpeed = moveSpeed;
                moveSpeed *= (1 - data.Skill_Effect_Value_2 / 100f);
                SkillManager.Instance.StartCoroutine(SkillManager.Instance.ReturnWhenDead(data.Skill_ID, particle));

                SkillManager.Instance.StartCoroutine(SpeedCoroutine(data.Skill_ID, data.Skill_Duration_2));

                break;
            case 6:
                particles.Add(data.Skill_ID);
                //agent.isStopped = true;
                if(animator != null)
                {
                    animator.speed = 0f;
                }
                SkillManager.Instance.StartCoroutine(SkillManager.Instance.ReturnWhenDead(data.Skill_ID, particle));
                SkillManager.Instance.StartCoroutine(StunCoroutine(data.Skill_ID, data.Skill_Duration_2));

                break;
        }
    }

    private IEnumerator StunCoroutine(long id, float duration)
    {
        paused = true;
        if(animator != null)
        {
            animator.speed = 0f;
        }
        Debug.Log("스턴");

        yield return new WaitForSeconds(duration);

        if(this == null || !gameObject || !gameObject.activeSelf || IsDead)
        {
            yield break;
        }

        //agent.isStopped = false;
        paused = false;
        if (animator != null) animator.speed = 1f; // 원래 값 복원

        particles.Remove(id);
        Debug.Log("스턴풀림");
    }

    private IEnumerator SpeedCoroutine(long id, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (this == null || !gameObject || !gameObject.activeSelf || IsDead)
        {
            yield break;
        }

        moveSpeed = beforeSpeed;
        particles.Remove(id);
    }

    private IEnumerator DeffenseCoroutine(long id, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (this == null || !gameObject || !gameObject.activeSelf || IsDead)
        {
            yield break;
        }

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

        ActiveBuffValue.Remove(skillId);

        switch (Skill_Effect)
        {
            case 3:
                deffense = deffense / mult;
                break;
            case 4:
                moveSpeed /= mult;
                break;
        }

        particles.Remove(skillId);
    }
}
