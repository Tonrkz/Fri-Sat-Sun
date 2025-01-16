using System;
using UnityEngine;

public class NormalSoldierBehavior : MonoBehaviour {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    [Header("Attributes")]
    [SerializeField] Enum_NormalSoldierState state = Enum_NormalSoldierState.Walk;
    [SerializeField] float hitPoint = 100;
    [SerializeField] Single walkSpeed = 1;
    [SerializeField] Single acceptableRadius = 0.33f;
    [SerializeField] Single damage = 10;
    [SerializeField] Single attackSpeed = 1;
    [SerializeField] Single attackRange = 0.75f;

    [Header("Debug")]
    GameObject attackTarget;
    Vector3 walkPosition;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
