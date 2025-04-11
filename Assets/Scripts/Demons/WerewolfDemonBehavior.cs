using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using TMPro;

public class WerewolfDemonBehavior : ADemons {
    [Header("Attributes")]
    [SerializeField] Enum_WerewolfDemonState state = Enum_WerewolfDemonState.Walk;
    [SerializeField] float hitPoint = 100;
    [SerializeField] Single moneyOnDead = 70;



    [Header("Movement Attributes")]
    [SerializeField] Single startWalkSpeed = 1.5f;



    [Header("Debug")]
    Enum_DemonTypes demonType = Enum_DemonTypes.Werewolf;



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
        ChangeDemonState(Enum_WerewolfDemonState.Walk);
        render.PlayAnimation(render.WALK, 0, WalkSpeed);
    }

    void Update() {
        if (Time.time < lastCalculateTime + delayCalculateTime) {
            return;
        }
        lastCalculateTime = Time.time;
        switch (state) {
            case Enum_WerewolfDemonState.Walk:
                if (Vector3.Distance(transform.position, movement.walkTarget.transform.position) <= acceptableRadius) {
                    movement.walkTarget = movement.GetNextWalkTarget();
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
                Move(movement.walkTarget.transform.position);
                break;
            case Enum_WerewolfDemonState.Dead:
                // Disabled Hitbox
                GetComponent<Rigidbody>().Sleep();
                GetComponent<SphereCollider>().enabled = false;
                GetComponent<CapsuleCollider>().excludeLayers = LayerMask.GetMask("Soldier");

                // Play Animation
                render.PlayAnimation(render.DEAD);
                break;
            default:
                break;
        }
        if (HitPoint <= 0) {
            state = Enum_WerewolfDemonState.Dead;
        }
    }

    public override void ChangeDemonState(Enum newState) {
        // Check if the new state is the same as the current state or dead
        if (state == (Enum_WerewolfDemonState)newState || state == Enum_WerewolfDemonState.Dead) {
            return;
        }

        // Change the state
        state = (Enum_WerewolfDemonState)newState;


        switch (state) {
            case Enum_WerewolfDemonState.Walk:
                render.PlayAnimation(render.WALK, 0, WalkSpeed);
                break;
            case Enum_WerewolfDemonState.Hurt:
                render.PlayAnimation(render.HURT, 0);
                break;
            case Enum_WerewolfDemonState.Dead:
                render.PlayAnimation(render.DEAD, 0);
                break;
            default:
                break;
        }
    }

    public override void AddKnockback(Vector3 knockback) {
        // Add a knockback
        rb.AddForce(knockback, ForceMode.Impulse);
        StartCoroutine(WaitForHurtAnimation());

        IEnumerator WaitForHurtAnimation() {
            ChangeDemonState(Enum_WerewolfDemonState.Hurt);
            yield return new WaitForSeconds(render.animator.GetCurrentAnimatorStateInfo(0).length);
            ChangeDemonState(Enum_WerewolfDemonState.Walk);
        }
    }
}
