using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

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
    [SerializeField] internal bool startCanSeePhantom = false;
    bool canSeePhantom;



    [Header("Debug")]
    internal GameObject baseTower;
    public GameObject BaseTower { get => baseTower; set => baseTower = value; }
    Single towerRange;
    float lastCalculateTime;
    [SerializeField] float delayCalculateTime = 0.2f;
    [SerializeField] GroundScript occupiedPath;
    GameObject attackTarget;
    Vector3 walkPosition;
    LayerMask DemonLayer;
    LayerMask PathLayer;


    void Start() {
        canSeePhantom = startCanSeePhantom;
        if (baseTower != null) {
            towerRange = baseTower.GetComponent<IActivatables>().TowerRange;
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
        if (baseTower.gameObject.IsDestroyed()) {
            state = Enum_NormalSoldierState.Die;
        }
        switch (state) {
            case Enum_NormalSoldierState.Initiate:
                if (Vector3.Distance(transform.position, walkPosition) <= acceptableRadius) {
                    state = Enum_NormalSoldierState.Idle;
                }
                if (baseTower != null && walkPosition == new Vector3()) {
                    if (!FindWalkPosition()) {
                        state = Enum_NormalSoldierState.Die;
                    }
                }
                StartCoroutine(CheckForTarget());
                break;
            case Enum_NormalSoldierState.Idle:
                StartCoroutine(CheckForTarget());
                if (Vector3.Distance(transform.position, walkPosition) > 2f) {
                    // Don't forget to fix this
                    state = Enum_NormalSoldierState.Initiate;
                }
                break;
            case Enum_NormalSoldierState.Engage:
                if (Vector3.Distance(transform.position, attackTarget.transform.position) <= attackRange) {
                    state = Enum_NormalSoldierState.Attack;
                }
                break;
            case Enum_NormalSoldierState.Attack:
                if (attackTarget.GetComponent<IDemons>().HitPoint <= 0 || attackTarget.gameObject.IsDestroyed()) {
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
        if (HitPoint <= 0) {
            state = Enum_NormalSoldierState.Die;
        }
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
                    state = Enum_NormalSoldierState.Initiate;
                }
                break;
            case Enum_NormalSoldierState.Die:
                StartCoroutine(Die());
                break;
            default:
                break;
        }
    }

    bool FindWalkPosition() {
        RaycastHit[] hits = Physics.SphereCastAll(baseTower.transform.position, towerRange, Vector3.up, towerRange, PathLayer);
        foreach (var hit in hits) {
            if (hit.collider.CompareTag("Path")) {
                if (hit.collider.GetComponent<GroundScript>().hasTower) {
                    continue;
                }
                occupiedPath = hit.collider.GetComponent<GroundScript>();
                occupiedPath.hasTower = true;
                occupiedPath.tower = gameObject;
                walkPosition = hit.transform.position + Vector3.up * 2f;
                return true;
            }
        }
        return false;
    }

    IEnumerator CheckForTarget() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange, DemonLayer);
        if (canSeePhantom) {
            foreach (Collider collider in colliders) {
                if (collider.CompareTag("Phantom")) {
                    attackTarget = collider.gameObject;
                    state = Enum_NormalSoldierState.Engage;
                    break;
                }
            }
        }

        if (attackTarget == null && colliders.Length > 0) {
            if (colliders[0].CompareTag("Demon")) {
                attackTarget = colliders[0].gameObject;
                state = Enum_NormalSoldierState.Engage;
            }
        }
        yield return null;
    }

    public IEnumerator SetCanSeePhantom(bool canSee) {
        canSeePhantom = canSee;
        yield return null;
    }

    public IEnumerator ResetCanSeePhantom() {
        canSeePhantom = startCanSeePhantom;
        yield return null;
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

    public IEnumerator Die() {
        yield return new WaitForEndOfFrame();
        if (occupiedPath != null) {
            occupiedPath.hasTower = false;
            occupiedPath.tower = null;
        }
        Destroy(gameObject);
    }
    public void Move(Vector3 position) {
        rb.MovePosition(Vector3.MoveTowards(transform.position, position, walkSpeed * Time.fixedDeltaTime));
    }

    public void TakeDamage(Single damage) {
        HitPoint -= damage;
    }

}
