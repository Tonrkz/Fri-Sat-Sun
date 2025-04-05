using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class WerewolfDemonBehavior : MonoBehaviour, IDemons {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] AnimatorRenderer render;
    [SerializeField] DemonsMovement movement;



    [Header("Attributes")]
    [SerializeField] Enum_WerewolfDemonState state = Enum_WerewolfDemonState.Walk;
    [SerializeField] float hitPoint = 100;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] Single moneyOnDead = 70;
    public Single MoneyOnDead { get { return moneyOnDead; } set { moneyOnDead = value; } }



    [Header("Movement Attributes")]
    [SerializeField] Single startWalkSpeed = 1.5f;
    public Single StartWalkSpeed { get => startWalkSpeed; set => startWalkSpeed = value; }
    Single walkSpeed = 1.5f;
    public Single WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    [SerializeField] internal Single acceptableRadius = 0.33f;



    [Header("Debug")]
    Enum_DemonTypes demonType = Enum_DemonTypes.Werewolf;
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
            case Enum_WerewolfDemonState.Walk:
                render.PlayAnimation("Walk");
                if (Vector3.Distance(transform.position, movement.walkTarget.transform.position) <= acceptableRadius) {
                    movement.walkTarget = movement.GetNextWalkTarget();
                }
                break;
            case Enum_WerewolfDemonState.Dead:
                render.PlayAnimation("Dead");
                break;
            default:
                break;
        }
    }

    void FixedUpdate() {
        switch (state) {
            case Enum_WerewolfDemonState.Walk:
                Move(movement.walkTarget.transform.position);
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

    public IEnumerator Dead() {
        DOVirtual.Float(0, 1, 1f, x => transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", x));

        yield return new WaitForSeconds(1.2f);

        if (transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.GetFloat("_Dissolve") >= 1) {
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
    }

    public void AddKnockback(Vector3 knockback) {
        // Add a knockback
        rb.AddForce(knockback, ForceMode.Impulse);
        render.PlayAnimation(render.HURT, 0, 1);
    }
}
