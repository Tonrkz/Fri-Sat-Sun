using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class PhantomDemonBehavior : ADemons {
    [Header("Attributes")]
    [SerializeField] Enum_PhantomDemonState state = Enum_PhantomDemonState.Walk;
    [SerializeField] float hitPoint = 100;
    [SerializeField] Single moneyOnDead = 100;




    [Header("Movement Attributes")]
    [SerializeField] Single startWalkSpeed = 1.5f;



    [Header("Debug")]
    Enum_DemonTypes demonType = Enum_DemonTypes.Phantom;

    void Start() {
        // Get the references
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<DemonsMovement>();

        // Set ADemons properties
        StartWalkSpeed = startWalkSpeed;
        WalkSpeed = StartWalkSpeed;
        HitPoint = hitPoint;
        MoneyOnDead = moneyOnDead;
        DemonType = demonType;

        // Set the initial state
        ChangeDemonState(Enum_PhantomDemonState.Walk);
        render.PlayAnimation(render.WALK, 0, WalkSpeed);
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
            ChangeDemonState(Enum_PhantomDemonState.Dead);
        }
    }

    public override void ChangeDemonState(Enum newState) {
        // Check if the new state is the same as the current state or dead
        if (state == (Enum_PhantomDemonState)newState || state == Enum_PhantomDemonState.Dead) {
            return;
        }

        // Change the state
        state = (Enum_PhantomDemonState)newState;

        // Set the animation
        switch (state) {
            case Enum_PhantomDemonState.Walk:
                render.PlayAnimation(render.WALK, 0, WalkSpeed);
                break;
            case Enum_PhantomDemonState.Hurt:
                render.PlayAnimation(render.HURT, 0);
                break;
            case Enum_PhantomDemonState.Dead:
                // Disabled Hitbox
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<SphereCollider>().enabled = false;
                GetComponent<CapsuleCollider>().enabled = false;

                // Play Animation
                render.PlayAnimation(render.DEAD, 0);
                break;
            default:
                break;
        }
    }

    public override void AddKnockback(Vector3 knockback) {
        base.AddKnockback(knockback);

        StartCoroutine(WaitForHurtAnimation());

        IEnumerator WaitForHurtAnimation() {
            ChangeDemonState(Enum_PhantomDemonState.Hurt);
            yield return new WaitForSeconds(render.animator.GetCurrentAnimatorStateInfo(0).length);
            ChangeDemonState(Enum_PhantomDemonState.Walk);
        }
    }
}
