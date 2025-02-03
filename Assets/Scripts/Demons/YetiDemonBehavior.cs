using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class YetiDemonBehavior : MonoBehaviour {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    [Header("Attributes")]
    [SerializeField] Enum_YetiDemonState state = Enum_YetiDemonState.Walk;
    [SerializeField] float hitPoint = 300;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] internal Single walkSpeed = 0.33f;
    [SerializeField] internal Single acceptableRadius = 0.33f;
    [SerializeField] internal Single damage = 10;
    [SerializeField] internal Single sightRange = 1.5f;
    [SerializeField] internal Single attackSpeed = 1;
    [SerializeField] internal Single attackCooldown = 1;
    [SerializeField] internal Single attackRange = 1f;
    [SerializeField] List<GameObject> walkPath = new List<GameObject>();

    [Header("Debug")]
    GameObject attackTarget;
    GameObject walkTarget;
    float lastCalculateTime;
    [SerializeField] float delayCalculateTime = 0.25f;
    Byte currentPathIndex = 0;
    LayerMask SoldierLayer;

    void Start() {
        walkPath = DemonsNavigationManager.instance.NormalWalkPath;
        currentPathIndex = 0;
        walkTarget = walkPath[currentPathIndex];
        SoldierLayer = LayerMask.GetMask("Soldier");
    }

    void Update() {
        if (Time.time < lastCalculateTime + delayCalculateTime) {
            return;
        }
        lastCalculateTime = Time.time;
        switch (state) {
            case Enum_YetiDemonState.Idle:
                break;
            case Enum_YetiDemonState.Walk:
                if (attackTarget != null) {
                    if (Vector3.Distance(transform.position, attackTarget.transform.position) <= attackRange) {
                        state = Enum_YetiDemonState.Attack;
                    }
                }
                if (Vector3.Distance(transform.position, walkTarget.transform.position) <= acceptableRadius) {
                    walkTarget = GetNextWalkTarget();
                }
                CheckForTarget();
                break;
            case Enum_YetiDemonState.Attack:
                if (attackTarget.GetComponent<ISoldiers>().HitPoint <= 0 || attackTarget.gameObject.IsDestroyed()) {
                    attackTarget = null;
                    state = Enum_YetiDemonState.Walk;
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
                if (attackTarget != null) {
                    Move(attackTarget.transform.position);
                    return;
                }
                Move(walkTarget.transform.position);
                break;
            case Enum_YetiDemonState.Attack:
                //Play Attack Animation
                //Deal Damage
                try {
                    Attack(attackTarget);
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
        target.gameObject.GetComponent<ISoldiers>().TakeDamage(damage * Time.fixedDeltaTime * 5); // Don't forget to fix this
    }

    public void Dead() {
        Destroy(gameObject);
    }

    public void Move(Vector3 position) {
        rb.MovePosition(Vector3.MoveTowards(transform.position, position, walkSpeed * Time.fixedDeltaTime));
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

    public GameObject GetNextWalkTarget() {
        currentPathIndex++;
        if (currentPathIndex >= walkPath.Count) {
            currentPathIndex = 0;
        }
        return walkPath[currentPathIndex];
    }
}
