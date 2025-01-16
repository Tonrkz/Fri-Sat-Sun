using NUnit.Framework;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Generic;
using UnityEngine;
using Unity.Behavior;
using Unity.VisualScripting;

public class NormalDemonBehavior : MonoBehaviour, IDemons {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    [Header("Attributes")]
    [SerializeField] Enum_NormalDemonState state = Enum_NormalDemonState.Walk;
    [SerializeField] float hitPoint = 100;
    [SerializeField] Single walkSpeed = 1;
    [SerializeField] Single acceptableRadius = 0.33f;
    [SerializeField] Single damage = 10;
    [SerializeField] Single attackSpeed = 1;
    [SerializeField] Single range = 0.75f;
    [SerializeField] List<GameObject> walkPath = new List<GameObject>();

    [Header("Debug")]
    GameObject attackTarget;
    GameObject walkTarget;
    Byte currentPathIndex = 0;

    void Start() {
        currentPathIndex = 0;
        walkTarget = walkPath[currentPathIndex];
    }

    void FixedUpdate() {
        switch (state) {
            case Enum_NormalDemonState.Walk:
                Move();
                break;
            case Enum_NormalDemonState.Attack:
                break;
            case Enum_NormalDemonState.Die:
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }

    public void Attack() {
        throw new System.NotImplementedException();
    }

    public void Die() {
        throw new System.NotImplementedException();
    }

    public void Move() {
        rb.MovePosition(Vector3.MoveTowards(transform.position, walkTarget.transform.position, walkSpeed * Time.deltaTime));
        if (Vector3.Distance(transform.position, walkTarget.transform.position) <= acceptableRadius) {
            walkTarget = GetNextWalkTarget();
        }
    }

    GameObject GetNextWalkTarget() {
        currentPathIndex++;
        if (currentPathIndex >= walkPath.Count) {
            currentPathIndex = 0;
        }
        return walkPath[currentPathIndex];
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
        Gizmos.color = Color.green;
        foreach (var item in walkPath) {
            Gizmos.DrawWireSphere(item.transform.position, acceptableRadius);
        }
    }
}
