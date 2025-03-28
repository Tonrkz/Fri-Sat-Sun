using DG.Tweening;
using System;
using UnityEngine;

public class PhantomDemonBehavior : MonoBehaviour, IDemons {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] AnimatorRenderer render;
    [SerializeField] DemonsMovement movement;



    [Header("Attributes")]
    [SerializeField] Enum_PhantomDemonState state = Enum_PhantomDemonState.Walk;
    [SerializeField] float hitPoint = 100;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] Single moneyOnDead = 100;
    public Single MoneyOnDead { get { return moneyOnDead; } set { moneyOnDead = value; } }




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
                render.PlayAnimation("Walk");
                if (Vector3.Distance(transform.position, movement.walkTarget.transform.position) <= acceptableRadius) {
                    movement.walkTarget = movement.GetNextWalkTarget();
                }
                break;
            case Enum_PhantomDemonState.Dead:
                render.PlayAnimation("Dead");
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
                break;
            default:
                break;
        }
        if (HitPoint <= 0) {
            state = Enum_PhantomDemonState.Dead;
        }
    }

    public void Dead() {
        DOVirtual.Float(0, 1, 1f, x => transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", x));

        if (transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.GetFloat("_Dissolve") == 1) {
            DemonsSpawnerManager.instance.OnDemonDead(this);
            //Play Dead Animation
            Destroy(gameObject);
        }
    }

    public void Move(Vector3 position) {
        rb.MovePosition(Vector3.MoveTowards(transform.position, position, walkSpeed * Time.fixedDeltaTime));
    }

    public void TakeDamage(Single damage) {
        HitPoint -= damage;
        render.PlayAnimation("Hurt");
    }
}
