using System;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;
using DG.Tweening;
using TMPro;

public class GoblinDemonBehavior : ADemons, IAttackables {
    [Header("Demon Attributes")]
    [SerializeField] Enum_GoblinDemonState state = Enum_GoblinDemonState.Walk;
    [SerializeField] float hitPoint = 60;
    [SerializeField] Single moneyOnDead = 25;



    [Header("Movement Attributes")]
    [SerializeField] Single startWalkSpeed = 1f;



    [Header("Attack Attributes")]
    [SerializeField] Single startDamage = 30;
    public Single StartDamage { get => startDamage; set => startDamage = value; }
    public Single Damage { get; set; }
    [SerializeField] internal Single sightRange = 1.5f;
    [SerializeField] Single attackSpeed = 1;
    public Single AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    [SerializeField] Single attackCooldown = 1.25f;
    public Single AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    [SerializeField] Single attackRange = 1.3f;
    public Single AttackRange { get => attackRange; set => attackRange = value; }



    [Header("Debug")]
    Enum_DemonTypes demonType = Enum_DemonTypes.Goblin;
    public GameObject AttackTarget { get; set; }
    float lastAttackTime;
    LayerMask SoldierLayer;



    void Start() {
        // Get the references
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<DemonsMovement>();
        SoldierLayer = LayerMask.GetMask("Soldier");

        // Set ADemons properties
        StartWalkSpeed = startWalkSpeed;
        WalkSpeed = StartWalkSpeed;
        HitPoint = hitPoint;
        MoneyOnDead = moneyOnDead;
        DemonType = demonType;

        // Set IAttackables properties
        Damage = StartDamage;

        // Set the initial state
        ChangeDemonState(Enum_GoblinDemonState.Walk);
        render.PlayAnimation(render.WALK, 0, WalkSpeed);
    }

    void Update() {
        if (Time.time < lastCalculateTime + delayCalculateTime) {
            return;
        }
        lastCalculateTime = Time.time;
        switch (state) {
            case Enum_GoblinDemonState.Idle:
                break;
            case Enum_GoblinDemonState.Walk:
                if (AttackTarget != null) {
                    if (Vector3.Distance(transform.position, AttackTarget.transform.position) <= AttackRange) {
                        ChangeDemonState(Enum_GoblinDemonState.Attack);
                        return;
                    }
                }
                if (Vector3.Distance(transform.position, movement.walkTarget.transform.position) <= acceptableRadius) {
                    movement.walkTarget = movement.GetNextWalkTarget();
                }
                CheckForTarget();
                break;
            case Enum_GoblinDemonState.Attack:
                if (AttackTarget.gameObject.IsDestroyed() || AttackTarget.GetComponent<IDamagable>().HitPoint <= 0) {
                    AttackTarget = null;
                    ChangeDemonState(Enum_GoblinDemonState.Walk);
                }
                break;
            case Enum_GoblinDemonState.Dead:
                break;
            default:
                break;
        }
    }

    void FixedUpdate() {
        switch (state) {
            case Enum_GoblinDemonState.Idle:
                return;
            case Enum_GoblinDemonState.Walk:
                if (AttackTarget != null) {
                    Move(AttackTarget.transform.position);
                    return;
                }
                Move(movement.walkTarget.transform.position);
                break;
            case Enum_GoblinDemonState.Attack:
                // Change to walking if attack target is out of reach
                if (AttackTarget.gameObject.IsDestroyed() || AttackTarget.GetComponent<IDamagable>().HitPoint <= 0 || Vector3.Distance(transform.position, AttackTarget.transform.position) > attackRange) {
                    AttackTarget = null;
                    ChangeDemonState(Enum_GoblinDemonState.Walk);
                    return;
                }

                //Play Attack Animation
                //Deal Damage
                try {
                    if (Time.time > lastAttackTime + AttackCooldown) {
                        render.PlayAnimation(render.ATTACK);
                    }
                }
                catch {
                    AttackTarget = null;
                    ChangeDemonState(Enum_GoblinDemonState.Walk);
                }
                break;
            case Enum_GoblinDemonState.Dead:
                break;
            default:
                break;
        }
        if (HitPoint <= 0) {
            ChangeDemonState(Enum_GoblinDemonState.Dead);
        }
    }

    public override void ChangeDemonState(Enum newState) {
        if (state == (Enum_GoblinDemonState)newState || state == Enum_GoblinDemonState.Dead) {
            return;
        }

        state = (Enum_GoblinDemonState)newState;

        switch (state) {
            case Enum_GoblinDemonState.Idle:
                render.PlayAnimation(render.IDLE, 0);
                break;
            case Enum_GoblinDemonState.Walk:
                render.PlayAnimation(render.WALK, 0, WalkSpeed);
                break;
            case Enum_GoblinDemonState.Hurt:
                render.PlayAnimation(render.HURT, 0);
                break;
            case Enum_GoblinDemonState.Attack:
                break;
            case Enum_GoblinDemonState.Dead:
                // Disabled Hitbox
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<SphereCollider>().enabled = false;
                GetComponent<CapsuleCollider>().enabled = false;

                // Play Animation
                render.PlayAnimation(render.DEAD, 0);
                break;
            default:
                ChangeDemonState(Enum_GoblinDemonState.Walk);
                break;
        }
    }

    public void Attack(GameObject target) {
        lastAttackTime = Time.time;

        Single knockbackForce = 1.5f;

        // Calculate knockback direction
        Vector3 knockbackDirection = AttackTarget.transform.position - transform.position;
        knockbackDirection.Normalize();
        knockbackDirection *= knockbackForce;

        target.gameObject.GetComponent<IDamagable>().AddKnockback(knockbackDirection * knockbackForce);
        target.gameObject.GetComponent<IDamagable>().TakeDamage(Damage); // Don't forget to fix this
        Debug.Log($"{gameObject} Attack");
    }

    public IEnumerator AttackDown(Single atkDownPercent) {
        Damage = StartDamage * (1 - atkDownPercent);
        yield return null;
    }

    public IEnumerator ResetAttack() {
        Damage = StartDamage;
        yield return null;
    }

    public override void AddKnockback(Vector3 knockback) {
        base.AddKnockback(knockback);

        StartCoroutine(WaitForHurtAnimation());

        IEnumerator WaitForHurtAnimation() {
            ChangeDemonState(Enum_GoblinDemonState.Hurt);
            yield return new WaitForSeconds(render.animator.GetCurrentAnimatorStateInfo(0).length);
            ChangeDemonState(Enum_GoblinDemonState.Walk);
        }
    }

    public void CheckForTarget() {
        Collider[] collides = Physics.OverlapSphere(transform.position, sightRange, SoldierLayer);
        foreach (var item in collides) {
            if (item.CompareTag("Soldier")) {
                AttackTarget = item.gameObject;
                break;
            }
        }
    }
}
