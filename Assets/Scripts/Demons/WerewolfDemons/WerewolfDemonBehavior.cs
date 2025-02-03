using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class WerewolfDemonBehavior : MonoBehaviour, IDemons {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    [Header("Attributes")]
    [SerializeField] Enum_WerewolfDemonState state = Enum_WerewolfDemonState.Walk;
    [SerializeField] float hitPoint = 100;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] internal Single walkSpeed = 1;
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
            case Enum_WerewolfDemonState.Walk:
                if (attackTarget != null) {
                    Move(attackTarget.transform.position);
                    return;
                }
                Move(walkTarget.transform.position);
                break;
            case Enum_WerewolfDemonState.Dead:
                Dead();
                break;
            default:
                break;
        }
        if (HitPoint <= 0) {
            state = Enum_WerewolfDemonState.Dead;
        }
    }

    public void Attack(GameObject target) {
        throw new System.NotImplementedException();
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
}
