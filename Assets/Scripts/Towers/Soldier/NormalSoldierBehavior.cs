using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NormalSoldierBehavior : MonoBehaviour, ISoldiers {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    [Header("Attributes")]
    [SerializeField] Enum_NormalSoldierState state = Enum_NormalSoldierState.Initiate;
    [SerializeField] float hitPoint = 100;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] internal Single walkSpeed = 1;
    [SerializeField] internal Single acceptableRadius = 0.33f;
    [SerializeField] internal Single damage = 10;
    [SerializeField] internal Single sightRange = 1.5f;
    [SerializeField] internal Single attackSpeed = 1;
    [SerializeField] internal Single attackCooldown = 1;
    [SerializeField] internal Single attackRange = 1f;
    [SerializeField] internal bool canSeeAssassin = false;

    [Header("Debug")]
    internal GameObject baseTower;
    public GameObject BaseTower { get => baseTower; set => baseTower = value; }
    Single towerRange;
    float lastCalculateTime;
    [SerializeField] float delayCalculateTime = 0.2f;
    GameObject attackTarget;
    Vector3 walkPosition;
    LayerMask DemonLayer;
    LayerMask PathLayer;


    void Start() {
        if (baseTower != null) {
            towerRange = baseTower.GetComponent<CampfireScript>().TowerRange;
        }
        DemonLayer = LayerMask.GetMask("Demon");
        PathLayer = LayerMask.GetMask("Ground");
        lastCalculateTime = Time.time;
    }

    void Update() {
        if (Time.time < lastCalculateTime + delayCalculateTime) {
            return;
        }
        lastCalculateTime = Time.time;
        switch (state) {
            case Enum_NormalSoldierState.Initiate:
                if (Vector3.Distance(transform.position, walkPosition) <= acceptableRadius) {
                    state = Enum_NormalSoldierState.Idle;
                }
                if (baseTower != null) {
                    RaycastHit[] hits = Physics.SphereCastAll(baseTower.transform.position, towerRange, Vector3.up, towerRange, PathLayer);
                    foreach (var hit in hits) {
                        Debug.Log(hit.collider.name);
                        if (hit.collider.CompareTag("Path")) {
                            Debug.Log("Found Path");
                            walkPosition = hit.transform.position + Vector3.up;
                            break;
                        }
                    }
                }
                break;
            case Enum_NormalSoldierState.Idle:
                CheckForTarget();
                break;
            case Enum_NormalSoldierState.Engage:
                if (Vector3.Distance(transform.position, attackTarget.transform.position) <= attackRange) {
                    state = Enum_NormalSoldierState.Attack;
                }
                break;
            case Enum_NormalSoldierState.Attack:
                if (attackTarget.GetComponent<IDemons>().HitPoint <= 0 || ReferenceEquals(attackTarget, null)) {
                    attackTarget = null;
                    state = Enum_NormalSoldierState.Idle;
                }
                break;
            case Enum_NormalSoldierState.Die:
                break;
            default:
                break;
        }
    }

    void FixedUpdate() {
        switch (state) {
            case Enum_NormalSoldierState.Initiate:
                if (walkPosition != new Vector3()) {
                    Move(walkPosition);
                    return;
                }
                break;
            case Enum_NormalSoldierState.Idle:
                return;
            case Enum_NormalSoldierState.Engage:
                Move(attackTarget.transform.position);
                break;
            case Enum_NormalSoldierState.Attack:
                //Play Attack Animation
                //Deal Damage
                try {
                    Attack(attackTarget);
                }
                catch {
                    attackTarget = null;
                    state = Enum_NormalSoldierState.Idle;
                }
                break;
            case Enum_NormalSoldierState.Die:
                Die();
                break;
            default:
                break;
        }
        if (HitPoint <= 0) {
            state = Enum_NormalSoldierState.Die;
        }
    }

    void CheckForTarget() {
        Collider[] collides = Physics.OverlapSphere(transform.position, sightRange, DemonLayer);
        foreach (var item in collides) {
            if (item.CompareTag("Demon") || (item.CompareTag("Assassin") && canSeeAssassin)) {
                attackTarget = item.gameObject;
                state = Enum_NormalSoldierState.Engage;
                break;
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(walkPosition, acceptableRadius);
    }

    public void Attack(GameObject target) {
        target.GetComponent<IDemons>().TakeDamage(damage * Time.fixedDeltaTime * 3);  // Don't forget to fix this
    }

    public void Die() {
        Destroy(gameObject);
    }
    public void Move(Vector3 position) {
        rb.MovePosition(Vector3.MoveTowards(transform.position, position, walkSpeed * Time.fixedDeltaTime));
    }

    public void TakeDamage(Single damage) {
        HitPoint -= damage;
    }

}
