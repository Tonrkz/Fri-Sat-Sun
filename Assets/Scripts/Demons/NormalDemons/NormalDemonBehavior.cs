using NUnit.Framework;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Generic;
using UnityEngine;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class NormalDemonBehavior : MonoBehaviour, IDemons {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    [Header("Attributes")]
    [SerializeField] Enum_NormalDemonState state = Enum_NormalDemonState.Walk;
    [SerializeField] float hitPoint = 100;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] internal Single walkSpeed = 1;
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
    Byte currentPathIndex = 0;
    LayerMask SoldierLayer;

    void Start() {
        currentPathIndex = 0;
        walkTarget = walkPath[currentPathIndex];
        SoldierLayer = LayerMask.GetMask("Soldier");
    }

    void FixedUpdate() {
        switch (state) {
            case Enum_NormalDemonState.Idle:
                return;
            case Enum_NormalDemonState.Walk:
                if (attackTarget != null) {
                    Move(attackTarget.transform.position);
                    if (Vector3.Distance(transform.position, attackTarget.transform.position) <= attackRange) {
                        state = Enum_NormalDemonState.Attack;
                    }
                    return;
                }
                Move(walkTarget.transform.position);
                if (Vector3.Distance(transform.position, walkTarget.transform.position) <= acceptableRadius) {
                    walkTarget = GetNextWalkTarget();
                }
                CheckForTarget();
                break;
            case Enum_NormalDemonState.Attack:
                //Play Attack Animation
                //Deal Damage
                Attack(attackTarget);
                if (attackTarget.GetComponent<ISoldiers>().HitPoint <= 0 || ReferenceEquals(attackTarget, null)) {
                    attackTarget = null;
                    state = Enum_NormalDemonState.Walk;
                }
                break;
            case Enum_NormalDemonState.Die:
                Die();
                break;
            default:
                break;
        }
        if (HitPoint <= 0) {
            state = Enum_NormalDemonState.Die;
        }
    }

    public void Attack(GameObject target) {
        target.gameObject.GetComponent<ISoldiers>().TakeDamage(damage * Time.fixedDeltaTime * 5); // Don't forget to fix this
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


    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        foreach (var item in walkPath) {
            Gizmos.DrawWireSphere(item.transform.position, acceptableRadius);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
