using NUnit.Framework;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Generic;
using UnityEngine;
using Unity.Behavior;

public class NormalDemonBehaviour : MonoBehaviour, IDemons {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    [Header("Attributes")]
    [SerializeField] Enum_NormalDemonState state = Enum_NormalDemonState.Walk;
    [SerializeField] float hitPoint = 100;
    [SerializeField] Byte walkSpeed = 1;
    [SerializeField] Single acceptableRadius = 0.5f;
    [SerializeField] Byte damage = 10;
    [SerializeField] Byte attackSpeed = 1;
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
    }

    public void Attack() {
        throw new System.NotImplementedException();
    }

    public void Die() {
        throw new System.NotImplementedException();
    }

    public void Move() {
        throw new System.NotImplementedException();
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
