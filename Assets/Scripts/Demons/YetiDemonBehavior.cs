using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class YetiDemonBehavior : ADemons, IAttackables, IDamagable {
    [Header("Attributes")]
    [SerializeField] Enum_YetiDemonState state = Enum_YetiDemonState.Walk;
    [SerializeField] float hitPoint = 300;
    [SerializeField] Single moneyOnDead = 120;




    [Header("Movement Attributes")]
    [SerializeField] Single startWalkSpeed = 1.5f;



    [Header("Attack Attributes")]
    [SerializeField] Single startDamage = 20;
    public Single StartDamage { get => startDamage; set => startDamage = value; }
    public Single Damage { get; set; }
    [SerializeField] internal Single sightRange = 1.5f;
    [SerializeField] Single attackSpeed = 1;
    public Single AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    [SerializeField] Single attackCooldown = 1;
    public Single AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    [SerializeField] Single attackRange = 1f;
    public Single AttackRange { get => attackRange; set => attackRange = value; }



    [Header("Debug")]
    Enum_DemonTypes demonType = Enum_DemonTypes.Yeti;
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
        ChangeDemonState(Enum_YetiDemonState.Walk);
        render.PlayAnimation(render.WALK, 0, WalkSpeed);
    }

    void Update() {
        if (Time.time < lastCalculateTime + delayCalculateTime) {
            return;
        }

        lastCalculateTime = Time.time;

        switch (state) {
            case Enum_YetiDemonState.Idle:
                render.PlayAnimation(render.IDLE);
                break;
            case Enum_YetiDemonState.Walk:
                render.PlayAnimation(render.WALK);
                if (AttackTarget != null) {
                    if (Vector3.Distance(transform.position, AttackTarget.transform.position) <= AttackRange) {
                        ChangeDemonState(Enum_YetiDemonState.Attack);
                        return;
                    }
                }
                if (Vector3.Distance(transform.position, movement.walkTarget.transform.position) <= acceptableRadius) {
                    movement.walkTarget = movement.GetNextWalkTarget();
                }
                CheckForTarget();
                break;
            case Enum_YetiDemonState.Attack:
                if (AttackTarget.gameObject.IsDestroyed() || AttackTarget.GetComponent<IDamagable>().HitPoint <= 0) {
                    AttackTarget = null;
                    ChangeDemonState(Enum_YetiDemonState.Walk);
                }
                break;
            case Enum_YetiDemonState.Dead:
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
                if (AttackTarget != null) {
                    Move(AttackTarget.transform.position);
                    return;
                }
                Move(movement.walkTarget.transform.position);
                break;
            case Enum_YetiDemonState.Attack:
                // Change to walking if attack target is out of reach
                if (AttackTarget.gameObject.IsDestroyed() || AttackTarget.GetComponent<IDamagable>().HitPoint <= 0 || Vector3.Distance(transform.position, AttackTarget.transform.position) > attackRange) {
                    AttackTarget = null;
                    ChangeDemonState(Enum_YetiDemonState.Walk);
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
                    ChangeDemonState(Enum_YetiDemonState.Walk);
                }
                break;
            case Enum_YetiDemonState.Dead:
                break;
            default:
                break;
        }
        if (HitPoint <= 0) {
            ChangeDemonState(Enum_YetiDemonState.Dead);
        }
    }

    public override void ChangeDemonState(Enum newState) {
        if (state == (Enum_YetiDemonState)newState || state == Enum_YetiDemonState.Dead) {
            return;
        }

        state = (Enum_YetiDemonState)newState;

        switch (state) {
            case Enum_YetiDemonState.Idle:
                render.PlayAnimation(render.IDLE, 0);
                break;
            case Enum_YetiDemonState.Walk:
                render.PlayAnimation(render.WALK, 0, WalkSpeed);
                break;
            case Enum_YetiDemonState.Hurt:
                render.PlayAnimation(render.HURT, 0);
                break;
            case Enum_YetiDemonState.Attack:
                break;
            case Enum_YetiDemonState.Dead:
                // Disabled Hitbox
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<SphereCollider>().enabled = false;
                GetComponent<CapsuleCollider>().enabled = false;

                // Play Animation
                render.PlayAnimation(render.DEAD, 0);
                break;
            default:
                break;
        }
    }

    public override void AddKnockback(Vector3 knockback) {
        base.AddKnockback(knockback);

        StartCoroutine(WaitForHurtAnimation());

        IEnumerator WaitForHurtAnimation() {
            ChangeDemonState(Enum_YetiDemonState.Hurt);
            yield return new WaitForSeconds(render.animator.GetCurrentAnimatorStateInfo(0).length);
            ChangeDemonState(Enum_YetiDemonState.Walk);
        }
    }

    public void Attack(GameObject target) {
        Debug.Log($"{gameObject} Attack");
        lastAttackTime = Time.time;

        Single knockbackForce = 2f;

        // Calculate knockback direction
        Vector3 knockbackDirection = AttackTarget.transform.position - transform.position;
        knockbackDirection.Normalize();
        knockbackDirection *= knockbackForce;

        target.gameObject.GetComponent<IDamagable>().AddKnockback(knockbackDirection * knockbackForce);
        target.gameObject.GetComponent<IDamagable>().TakeDamage(Damage); // Don't forget to fix this
    }

    public IEnumerator AttackDown(Single atkDownPercent) {
        Damage = StartDamage * (1 - atkDownPercent);
        yield return null;
    }

    public IEnumerator ResetAttack() {
        Damage = StartDamage;
        yield return null;
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
