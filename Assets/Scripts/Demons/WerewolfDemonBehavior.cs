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
    [SerializeField] internal Single acceptableRadius = 0.33f;
    [SerializeField] List<GameObject> walkPath = new List<GameObject>();

    [Header("Debug")]
    GameObject walkTarget;
    float lastCalculateTime;
    [SerializeField] float delayCalculateTime = 0.2f;
    Byte currentPathIndex = 0;

    void Start() {
        walkPath = DemonsNavigationManager.instance.ShortcutWalkPath;
        currentPathIndex = 0;
        walkTarget = walkPath[currentPathIndex];
    }

    void Update() {
        if (Time.time < lastCalculateTime + delayCalculateTime) {
            return;
        }
        lastCalculateTime = Time.time;
        switch (state) {
            case Enum_WerewolfDemonState.Walk:
                if (Vector3.Distance(transform.position, walkTarget.transform.position) <= acceptableRadius) {
                    walkTarget = GetNextWalkTarget();
                }
                break;
            case Enum_WerewolfDemonState.Dead:
                break;
            default:
                break;
        }
    }

    void FixedUpdate() {
        switch (state) {
            case Enum_WerewolfDemonState.Walk:
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

    GameObject GetNextWalkTarget() {
        currentPathIndex++;
        if (currentPathIndex >= walkPath.Count) {
            currentPathIndex = 0;
        }
        return walkPath[currentPathIndex];
    }
}
