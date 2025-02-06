using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class GoblinDemonBehavior : MonoBehaviour, IDemons, IAttackables {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;
    [SerializeField] DemonsMovement movement;

    [Header("Attributes")]
    [SerializeField] Enum_GoblinDemonState state = Enum_GoblinDemonState.Walk;
    [SerializeField] float hitPoint = 100;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField]  Single startWalkSpeed = 1.5f;
    public Single StartWalkSpeed { get => startWalkSpeed; set => startWalkSpeed = value; }
    [HideInInspector] Single walkSpeed = 1.5f;
    public Single WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    [SerializeField] internal Single acceptableRadius = 0.33f;
    [SerializeField] Single damage = 10;
    public Single Damage { get => damage; set => damage = value; }
    [SerializeField] internal Single sightRange = 1.5f;
    [SerializeField] Single attackSpeed = 1;
    public Single AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    [SerializeField] Single attackCooldown = 1;
    public Single AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    [SerializeField] Single attackRange = 1f;
    public Single AttackRange { get => attackRange; set => attackRange = value; }

    [Header("Debug")]
    Enum_DemonTypes demonType = Enum_DemonTypes.Goblin;
    public Enum_DemonTypes DemonType { get => demonType; set => demonType = value; }
    GameObject attackTarget;
    public GameObject AttackTarget { get => attackTarget; set => attackTarget = value; }
    float lastCalculateTime;
    [SerializeField] float delayCalculateTime = 0.2f;
    LayerMask SoldierLayer;

    void Start() {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<DemonsMovement>();
        WalkSpeed = StartWalkSpeed;
        SoldierLayer = LayerMask.GetMask("Soldier");
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
                if (attackTarget != null) {
                    if (Vector3.Distance(transform.position, attackTarget.transform.position) <= AttackRange) {
                        state = Enum_GoblinDemonState.Attack;
                    }
                }
                if (Vector3.Distance(transform.position, movement.walkTarget.transform.position) <= acceptableRadius) {
                    movement.walkTarget = movement.GetNextWalkTarget();
                }
                CheckForTarget();
                break;
            case Enum_GoblinDemonState.Attack:
                if (attackTarget.GetComponent<ISoldiers>().HitPoint <= 0 || attackTarget.gameObject.IsDestroyed()) {
                    attackTarget = null;
                    state = Enum_GoblinDemonState.Walk;
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
                if (attackTarget != null) {
                    Move(attackTarget.transform.position);
                    return;
                }
                Move(movement.walkTarget.transform.position);
                break;
            case Enum_GoblinDemonState.Attack:
                //Play Attack Animation
                //Deal Damage
                try {
                    Attack(attackTarget);
                }
                catch {
                    attackTarget = null;
                    state = Enum_GoblinDemonState.Walk;
                }
                break;
            case Enum_GoblinDemonState.Dead:
                Dead();
                break;
            default:
                break;
        }
        if (HitPoint <= 0) {
            state = Enum_GoblinDemonState.Dead;
        }
    }

    public void Attack(GameObject target) {
        target.gameObject.GetComponent<ISoldiers>().TakeDamage(Damage * Time.fixedDeltaTime * 5); // Don't forget to fix this
    }

    public void AttackDown(Single atkDownPercent) {
        throw new NotImplementedException();
    }

    public void Dead() {
        Destroy(gameObject);
    }

    public void Move(Vector3 position) {
        rb.MovePosition(Vector3.MoveTowards(transform.position, position, WalkSpeed * Time.fixedDeltaTime));
    }

    public void TakeDamage(Single damage) {
        HitPoint -= damage;
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
