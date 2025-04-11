using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NormalSoldierBehavior : MonoBehaviour, ISoldiers, IDamagable {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] AnimatorRenderer render;
    [SerializeField] GameObject floatingTextPrefab;



    [Header("Attributes")]
    [SerializeField] Enum_NormalSoldierState state;
    public float HitPoint { get; set; } = 100;
    public Single WalkSpeed { get; set; }
    public Single AcceptableRadius { get; set; }
    public Single Damage { get; set; }
    public Single SightRange { get; set; }
    public Single AttackSpeed { get; set; }
    public Single AttackCooldown { get; set; }
    public Single AttackRange { get; set; }
    public bool StartCanSeePhantom { get; set; }
    public bool CanSeePhantom { get; set; }



    [Header("Debug")]
    [SerializeField] float delayCalculateTime = 0.2f;
    [SerializeField] GroundScript occupiedPath;
    internal GameObject baseTower;
    public GameObject BaseTower { get => baseTower; set => baseTower = value; }
    Single towerRange;
    float lastCalculateTime;
    float lastAttackTime;
    internal GameObject attackTarget;
    Vector3 walkPosition;
    LayerMask DemonLayer;
    LayerMask PathLayer;


    void Start() {
        CanSeePhantom = StartCanSeePhantom;
        if (baseTower != null) {
            towerRange = baseTower.GetComponent<IActivatables>().TowerRange;
        }
        DemonLayer = LayerMask.GetMask("Demon");
        PathLayer = LayerMask.GetMask("Ground");
        lastCalculateTime = Time.time;

        ChangeState(Enum_NormalSoldierState.Initiate);
    }

    void Update() {
        if (Time.time < lastCalculateTime + delayCalculateTime) {
            return;
        }
        lastCalculateTime = Time.time;
        if (baseTower.gameObject.IsDestroyed()) {
            ChangeState(Enum_NormalSoldierState.Die);
        }
        if (HitPoint <= 0) {
            ChangeState(Enum_NormalSoldierState.Die);
            return;
        }
        switch (state) {
            case Enum_NormalSoldierState.Initiate:
                if (Vector3.Distance(transform.position, walkPosition) <= AcceptableRadius) {
                    ChangeState(Enum_NormalSoldierState.Idle);
                }
                StartCoroutine(CheckForTarget());
                break;
            case Enum_NormalSoldierState.Idle:
                StartCoroutine(CheckForTarget());
                if (Vector3.Distance(transform.position, walkPosition) > 1.25f) {
                    // Don't forget to fix this
                    ChangeState(Enum_NormalSoldierState.Initiate);
                }
                break;
            case Enum_NormalSoldierState.Engage:
                if (attackTarget.GetComponent<IDamagable>().HitPoint <= 0 || attackTarget.gameObject.IsDestroyed() || Vector3.Distance(transform.position, attackTarget.transform.position) > SightRange) {
                    attackTarget = null;
                    ChangeState(Enum_NormalSoldierState.Initiate);
                    return;
                }

                if (Vector3.Distance(transform.position, attackTarget.transform.position) <= AttackRange) {
                    ChangeState(Enum_NormalSoldierState.Attack);
                }
                break;
            case Enum_NormalSoldierState.Attack:
                if (attackTarget.GetComponent<IDamagable>().HitPoint <= 0 || attackTarget.gameObject.IsDestroyed() || Vector3.Distance(transform.position, attackTarget.transform.position) > AttackRange) {
                    attackTarget = null;
                    ChangeState(Enum_NormalSoldierState.Initiate);
                    return;
                }

                if (Time.time > lastAttackTime + AttackCooldown) {
                    render.PlayAnimation(render.ATTACK, 0.1f);
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
                try {
                    Move(attackTarget.transform.position);
                }
                catch {
                    attackTarget = null;
                    ChangeState(Enum_NormalSoldierState.Initiate);
                }
                break;
            case Enum_NormalSoldierState.Attack:
                break;
            case Enum_NormalSoldierState.Die:
                break;
            default:
                break;
        }
    }

    public void ChangeState(Enum_NormalSoldierState newState) {
        state = newState;
        switch (state) {
            case Enum_NormalSoldierState.Initiate:
                // Find Walk Position
                if (baseTower != null && walkPosition == new Vector3()) {
                    if (!FindWalkPosition()) {
                        Destroy(gameObject);
                    }
                }
                // Play Walk Animation
                render.PlayAnimation(render.WALK, 0, WalkSpeed);
                break;
            case Enum_NormalSoldierState.Idle:
                // Play Idle Animation
                render.PlayAnimation(render.IDLE, 0);
                break;
            case Enum_NormalSoldierState.Engage:
                // Play Walk Animation
                render.PlayAnimation(render.WALK, 0, WalkSpeed);
                break;
            case Enum_NormalSoldierState.Hurt:
                // Play Hurt Animation
                render.PlayAnimation(render.HURT, 0);
                break;
            case Enum_NormalSoldierState.Attack:
                // Play Idle Animation
                render.PlayAnimation(render.IDLE, 0);
                break;
            case Enum_NormalSoldierState.Die:
                // Disabled Hitbox
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<SphereCollider>().enabled = false;
                GetComponent<CapsuleCollider>().enabled = false;

                // Play Animation
                render.PlayAnimation(render.DEAD);
                break;
            default:
                ChangeState(Enum_NormalSoldierState.Initiate);
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
        if (attackTarget != null) {
            ChangeState(Enum_NormalSoldierState.Engage);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, SightRange, DemonLayer);
        if (CanSeePhantom) {
            foreach (Collider collider in colliders) {
                if (collider.CompareTag("Phantom")) {
                    attackTarget = collider.gameObject;
                    ChangeState(Enum_NormalSoldierState.Engage);
                    break;
                }
            }
        }

        if (attackTarget == null && colliders.Length > 0) {
            if (colliders[0].CompareTag("Demon")) {
                attackTarget = colliders[0].gameObject;
                ChangeState(Enum_NormalSoldierState.Engage);
            }
        }
        yield return null;
    }

    public IEnumerator SetCanSeePhantom(bool canSee) {
        CanSeePhantom = canSee;
        yield return null;
    }

    public IEnumerator ResetCanSeePhantom() {
        CanSeePhantom = StartCanSeePhantom;
        yield return null;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(walkPosition, AcceptableRadius);
    }

    public void Attack(GameObject target) {
        lastAttackTime = Time.time;

        Single knockbackForce = 3f;

        // Calculate knockback direction
        Vector3 knockbackDirection = attackTarget.transform.position - transform.position;
        knockbackDirection.Normalize();

        target.GetComponent<IDamagable>().AddKnockback(knockbackDirection * knockbackForce);
        target.GetComponent<IDamagable>().TakeDamage(Damage * GlobalAttributeMultipliers.SoldierDamageMultiplier);  // Don't forget to fix this
        Debug.Log($"{gameObject} Attack");
    }

    public IEnumerator Die() {
        yield return new WaitForEndOfFrame();
        if (occupiedPath != null) {
            occupiedPath.hasTower = false;
            occupiedPath.tower = null;
        }
        Destroy(gameObject);
    }

    Vector3 FindDirection(Vector3 target) {
        Vector3 direction = (target - transform.position).normalized;

        // Flip sprite based on direction
        if (direction.x < 0) {
            render.FlipSprite(true);
        }
        else {
            render.FlipSprite(false);
        }

        return direction;
    }

    public void Move(Vector3 position) {
        rb.MovePosition(transform.position + FindDirection(position) * WalkSpeed * Time.fixedDeltaTime);
    }

    public void TakeDamage(Single damage) {
        HitPoint -= damage;
        ShowFloatingText();

        void ShowFloatingText() {
            if (HitPoint > 0 && floatingTextPrefab) {
                var floatingText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
                floatingText.GetComponent<TextMeshPro>().SetText(((int)HitPoint).ToString());
            }
        }
    }

    public void AddKnockback(Vector3 knockback) {
        // Add a knockback
        if (HitPoint > 0) {
            rb.AddForce(knockback, ForceMode.Impulse);
        }

        StartCoroutine(WaitForHurtAnimation());

        IEnumerator WaitForHurtAnimation() {
            ChangeState(Enum_NormalSoldierState.Hurt);
            yield return new WaitForSeconds(render.animator.GetCurrentAnimatorStateInfo(0).length);
            ChangeState(Enum_NormalSoldierState.Initiate);
        }
    }
}