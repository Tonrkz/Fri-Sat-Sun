using DG.Tweening;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class YetiDemonBehavior : MonoBehaviour, IDemons, IAttackables {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] AnimatorRenderer render;
    [SerializeField] DemonsMovement movement;



    [Header("Attributes")]
    [SerializeField] Enum_YetiDemonState state = Enum_YetiDemonState.Walk;
    [SerializeField] float hitPoint = 300;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] Single moneyOnDead = 120;
    public Single MoneyOnDead { get { return moneyOnDead; } set { moneyOnDead = value; } }




    [Header("Movement Attributes")]
    [SerializeField] Single startWalkSpeed = 1.5f;
    public Single StartWalkSpeed { get => startWalkSpeed; set => startWalkSpeed = value; }
    Single walkSpeed;
    public Single WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    [SerializeField] internal Single acceptableRadius = 0.75f;



    [Header("Attack Attributes")]
    [SerializeField] Single startDamage = 20;
    public Single StartDamage { get => startDamage; set => startDamage = value; }
    Single damage;
    public Single Damage { get => damage; set => damage = value; }
    [SerializeField] internal Single sightRange = 1.5f;
    [SerializeField] Single attackSpeed = 1;
    public Single AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    [SerializeField] Single attackCooldown = 1;
    public Single AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    [SerializeField] Single attackRange = 1f;
    public Single AttackRange { get => attackRange; set => attackRange = value; }



    [Header("Debug")]
    Enum_DemonTypes demonType = Enum_DemonTypes.Yeti;
    public Enum_DemonTypes DemonType { get => demonType; set => demonType = value; }
    GameObject attackTarget;
    public GameObject AttackTarget { get => attackTarget; set => attackTarget = value; }
    float lastCalculateTime;
    float lastAttackTime;
    [SerializeField] float delayCalculateTime = 0.2f;
    LayerMask SoldierLayer;

    void Start() {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<DemonsMovement>();
        WalkSpeed = StartWalkSpeed;
        Damage = StartDamage;
        SoldierLayer = LayerMask.GetMask("Soldier");
    }

    void Update() {
        if (Time.time < lastCalculateTime + delayCalculateTime) {
            return;
        }
        lastCalculateTime = Time.time;
        switch (state) {
            case Enum_YetiDemonState.Idle:
                render.PlayAnimation("Idle");
                break;
            case Enum_YetiDemonState.Walk:
                render.PlayAnimation("Walk");
                if (attackTarget != null) {
                    if (Vector3.Distance(transform.position, attackTarget.transform.position) <= AttackRange) {
                        state = Enum_YetiDemonState.Attack;
                    }
                }
                if (Vector3.Distance(transform.position, movement.walkTarget.transform.position) <= acceptableRadius) {
                    movement.walkTarget = movement.GetNextWalkTarget();
                }
                CheckForTarget();
                break;
            case Enum_YetiDemonState.Attack:
                if (attackTarget.gameObject.IsDestroyed() || attackTarget.GetComponent<ISoldiers>().HitPoint <= 0) {
                    attackTarget = null;
                    state = Enum_YetiDemonState.Walk;
                }
                break;
            case Enum_YetiDemonState.Dead:
                render.PlayAnimation("Dead");
                break;
            default:
                break;
        }
    }

    void FixedUpdate() {
        switch (state) {
            case Enum_YetiDemonState.Idle:
                return;
            case Enum_YetiDemonState.Walk:
                if (attackTarget != null) {
                    Move(attackTarget.transform.position);
                    return;
                }
                Move(movement.walkTarget.transform.position);
                break;
            case Enum_YetiDemonState.Attack:
                // Change to walking if attack target is out of reach
                if (attackTarget.gameObject.IsDestroyed() || attackTarget.GetComponent<ISoldiers>().HitPoint <= 0 || Vector3.Distance(transform.position, attackTarget.transform.position) > attackRange) {
                    attackTarget = null;
                    state = Enum_YetiDemonState.Walk;
                    return;
                }

                //Play Attack Animation
                //Deal Damage
                try {
                    if (Time.time > lastAttackTime + AttackCooldown) {
                        render.PlayAnimation("Attack");
                    }
                }
                catch {
                    attackTarget = null;
                    state = Enum_YetiDemonState.Walk;
                }
                break;
            case Enum_YetiDemonState.Dead:
                Dead();
                break;
            default:
                break;
        }
        if (HitPoint <= 0) {
            state = Enum_YetiDemonState.Dead;
        }
    }

    public void Attack(GameObject target) {
        Debug.Log($"{gameObject} Attack");
        lastAttackTime = Time.time;

        Single knockbackForce = 2f;

        // Calculate knockback direction
        Vector3 knockbackDirection = attackTarget.transform.position - transform.position;
        knockbackDirection.Normalize();
        knockbackDirection *= knockbackForce;

        target.gameObject.GetComponent<ISoldiers>().AddKnockback(knockbackDirection * knockbackForce);
        target.gameObject.GetComponent<ISoldiers>().TakeDamage(Damage); // Don't forget to fix this
    }

    public IEnumerator AttackDown(Single atkDownPercent) {
        Damage = StartDamage * (1 - atkDownPercent);
        yield return null;
    }

    public IEnumerator ResetAttack() {
        Damage = StartDamage;
        yield return null;
    }

    public IEnumerator Dead() {
        DOVirtual.Float(0, 1, 1f, x => transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", x));

        yield return new WaitForSeconds(1.2f);

        if (transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.GetFloat("_Dissolve") >= 1) {
            DemonsSpawnerManager.instance.OnDemonDead(this);
            //Play Dead Animation
            Destroy(gameObject);
        }
    }

    public void Move(Vector3 position) {
        rb.MovePosition(Vector3.MoveTowards(transform.position, position, WalkSpeed * Time.fixedDeltaTime));
    }

    public void TakeDamage(Single damage) {
        HitPoint -= damage;
    }

    public void AddKnockback(Vector3 knockback) {
        // Add a knockback
        rb.AddForce(knockback, ForceMode.Impulse);
        render.PlayAnimation(render.HURT, 0, 1);
    }

    public void CheckForTarget() {
        Collider[] collides = Physics.OverlapSphere(transform.position, sightRange, SoldierLayer);
        foreach (var item in collides) {
            if (item.CompareTag("Soldier")) {
                attackTarget = item.gameObject;
                break;
            }
        }
    }
}
