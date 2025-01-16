using System;
using System.Reflection.Emit;
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
    Single towerRange;
    GameObject attackTarget;
    Vector3 walkPosition;
    LayerMask DemonAndSoldierLayer;
    LayerMask PathLayer;


    private void Start() {
        if (baseTower != null) {
            towerRange = baseTower.GetComponent<CampfireScript>().range;
        }
        DemonAndSoldierLayer = LayerMask.GetMask("DemonAndSoldier");
        PathLayer = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate() {
        switch (state) {
            case Enum_NormalSoldierState.Initiate:
                if (baseTower != null) {
                    RaycastHit[] hits = Physics.SphereCastAll(baseTower.transform.position, towerRange, Vector3.up, towerRange, PathLayer);
                    foreach (var hit in hits) {
                        if (hit.collider.CompareTag("Path")) {
                            walkPosition = hit.transform.position;
                            Move(walkPosition);
                            state = Enum_NormalSoldierState.Idle;
                            break;
                        }
                    }
                }
                break;
            case Enum_NormalSoldierState.Idle:
                CheckForTarget();
                return;
            case Enum_NormalSoldierState.Engage:
                Move(attackTarget.transform.position);
                if (Vector3.Distance(transform.position, attackTarget.transform.position) <= attackRange) {
                    state = Enum_NormalSoldierState.Attack;
                }
                break;
            case Enum_NormalSoldierState.Attack:
                //Play Attack Animation
                //Deal Damage
                Attack(attackTarget);
                if (attackTarget.GetComponent<IDemons>().HitPoint <= 0) {
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
        Collider[] collides = Physics.OverlapSphere(transform.position, sightRange, DemonAndSoldierLayer);
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
