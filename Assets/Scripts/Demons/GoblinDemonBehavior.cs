using System;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;
using DG.Tweening;

public class GoblinDemonBehavior : MonoBehaviour, IDemons, IAttackables {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] AnimatorRenderer render;
    [SerializeField] DemonsMovement movement;



    [Header("Demon Attributes")]
    [SerializeField] Enum_GoblinDemonState state = Enum_GoblinDemonState.Walk;
    [SerializeField] float hitPoint = 100;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] Single moneyOnDead = 20;
    public Single MoneyOnDead { get { return moneyOnDead; } set { moneyOnDead = value; } }



    [Header("Movement Attributes")]
    [SerializeField] Single startWalkSpeed = 1.5f;
    public Single StartWalkSpeed { get => startWalkSpeed; set => startWalkSpeed = value; }
    Single walkSpeed;
    public Single WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    [SerializeField] internal Single acceptableRadius = 0.33f;



    [Header("Attack Attributes")]
    [SerializeField] Single startDamage = 10;
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
    Enum_DemonTypes demonType = Enum_DemonTypes.Goblin;
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
                try {
                    if (attackTarget.gameObject.IsDestroyed() || attackTarget.GetComponent<ISoldiers>().HitPoint <= 0) {
                        attackTarget = null;
                        state = Enum_GoblinDemonState.Walk;
                    }
                }
                catch {
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
                render.PlayAnimation("Idle");
                return;
            case Enum_GoblinDemonState.Walk:
                render.PlayAnimation("Walk");
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
                    if (Time.time > lastAttackTime + AttackCooldown) {
                        render.PlayAnimation("Attack");
                        lastCalculateTime = Time.time;
                    }
                }
                catch {
                    attackTarget = null;
                    state = Enum_GoblinDemonState.Walk;
                }
                break;
            case Enum_GoblinDemonState.Dead:
                render.PlayAnimation("Dead");
                break;
            default:
                break;
        }
        if (HitPoint <= 0) {
            state = Enum_GoblinDemonState.Dead;
        }
    }

    public void Attack(GameObject target) {
        lastAttackTime = Time.time;
        target.gameObject.GetComponent<ISoldiers>().TakeDamage(Damage); // Don't forget to fix this
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

    public void Dead() {
        DOVirtual.Float(0, 1, 1f, x => transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", x));

        if (transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.GetFloat("_Dissolve") == 1) {
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
        render.PlayAnimation("Hurt");
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
