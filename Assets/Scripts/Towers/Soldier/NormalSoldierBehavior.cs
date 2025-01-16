using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NormalSoldierBehavior : MonoBehaviour {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    [Header("Attributes")]
    [SerializeField] Enum_NormalSoldierState state = Enum_NormalSoldierState.Initiate;
    [SerializeField] internal float hitPoint = 100;
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

    private void Start() {
        towerRange = baseTower.GetComponent<CampfireScript>().range;
    }

    private void FixedUpdate() {
        switch (state) {
            case Enum_NormalSoldierState.Initiate:
                if (baseTower != null) {
                    RaycastHit[] hits = Physics.SphereCastAll(baseTower.transform.position, towerRange, Vector3.up, towerRange, 6);
                    foreach (var hit in hits) {
                        if (hit.collider.CompareTag("Path")) {
                            walkPosition = hit.transform.position;
                            rb.MovePosition(Vector3.MoveTowards(transform.position, walkPosition, walkSpeed * Time.fixedDeltaTime));
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
                rb.MovePosition(Vector3.MoveTowards(transform.position, attackTarget.transform.position, walkSpeed * Time.fixedDeltaTime));
                if (Vector3.Distance(transform.position, attackTarget.transform.position) <= attackRange) {
                    state = Enum_NormalSoldierState.Attack;
                }
                break;
            case Enum_NormalSoldierState.Attack:
                //Play Attack Animation
                //Deal Damage
                attackTarget.GetComponent<IDemons>().TakeDamage(damage * Time.deltaTime);
                if (attackTarget.GetComponent<IDemons>().HitPoint <= 0) {
                    state = Enum_NormalSoldierState.Idle;
                }
                break;
            case Enum_NormalSoldierState.Die:
                Destroy(gameObject);
                break;
            default:
                break;
        }
        if (hitPoint <= 0) {
            state = Enum_NormalSoldierState.Die;
        }
    }

    void CheckForTarget() {
        Collider[] collides = Physics.OverlapSphere(transform.position, attackRange);
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
}
