using System;
using UnityEngine;

public class PhantomDemonBehavior : MonoBehaviour, IDemons {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;
    [SerializeField] DemonsMovement movement;



    [Header("Attributes")]
    [SerializeField] Enum_PhantomDemonState state = Enum_PhantomDemonState.Walk;
    [SerializeField] float hitPoint = 100;
    [SerializeField] public float HitPoint { get => hitPoint; set => hitPoint = value; }



    [Header("Movement Attributes")]
    [SerializeField] Single startWalkSpeed = 1.5f;
    public Single StartWalkSpeed { get => startWalkSpeed; set => startWalkSpeed = value; }
    Single walkSpeed = 1.5f;
    public Single WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    [SerializeField] internal Single acceptableRadius = 0.33f;



    [Header("Debug")]
    Enum_DemonTypes demonType = Enum_DemonTypes.Phantom;
    public Enum_DemonTypes DemonType { get => demonType; set => demonType = value; }
    float lastCalculateTime;
    [SerializeField] float delayCalculateTime = 0.2f;

    void Start() {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<DemonsMovement>();
        walkSpeed = startWalkSpeed;
    }

    void Update() {
        if (Time.time < lastCalculateTime + delayCalculateTime) {
            return;
        }
        lastCalculateTime = Time.time;
        switch (state) {
            case Enum_PhantomDemonState.Walk:
                if (Vector3.Distance(transform.position, movement.walkTarget.transform.position) <= acceptableRadius) {
                    movement.walkTarget = movement.GetNextWalkTarget();
                }
                break;
            case Enum_PhantomDemonState.Dead:
                break;
            default:
                break;
        }
    }

    void FixedUpdate() {
        switch (state) {
            case Enum_PhantomDemonState.Walk:
                Move(movement.walkTarget.transform.position);
                break;
            case Enum_PhantomDemonState.Dead:
                Dead();
                break;
            default:
                break;
        }
        if (HitPoint <= 0) {
            state = Enum_PhantomDemonState.Dead;
        }
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
