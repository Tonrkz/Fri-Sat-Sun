using DG.Tweening;
using System;
using UnityEngine;

public class AnimatorRenderer : MonoBehaviour {
    [Header("References")]
    [SerializeField] Animator animator; // Reference to the animator

    [Header("Animation State Names")]
    public readonly string BUILDING = "Building"; // The building animation state name
    public readonly string IDLE = "Idle"; // The idle animation state name
    public readonly string WALK = "Walk"; // The walk animation state name
    public readonly string ACTIVATE = "Activate"; // The run animation state name
    public readonly string ATTACK = "Attack"; // The attack animation state name
    public readonly string HURT = "Hurt"; // The attack animation state name
    public readonly string DEAD = "Dead"; // The dead animation state name

    [Header("Debug")]
    string currentAnimation; // The current animation that is playing

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string animationName, Single crossfadeDuration = 0.2f, Single playingSpeed = 1f) {
        if (animationName != currentAnimation && currentAnimation != HURT) {
            animator.CrossFade(animationName, crossfadeDuration);
            animator.speed = playingSpeed;
        }
    }

    public void AnimNotifyOnDestroyTower() {
        // Notify the tower that the animation is done
        GetComponentInParent<ATowers>().Dead();
    }

    public void AnimNotifyOnAttack() {
        // Notify the tower that the animation is done
        if (transform.parent.gameObject.CompareTag("Soldier")) {
            GetComponentInParent<ISoldiers>().Attack(GetComponentInParent<NormalSoldierBehavior>().attackTarget);
        }
        else if (transform.parent.gameObject.CompareTag("Demon")) {
            GetComponentInParent<IAttackables>().Attack(GetComponentInParent<IAttackables>().AttackTarget);
        }
    }

    public void AnimNotifyOnDead() {
        // Notify the tower that the animation is done
        if (transform.parent.gameObject.CompareTag("Soldier")) {
            StartCoroutine(GetComponentInParent<ISoldiers>().Die());
        }
        else if (transform.parent.gameObject.CompareTag("Demon")) {
            StartCoroutine(GetComponentInParent<IDemons>().Dead());
        }
    }

    public void AnimNotifyToIdle() {
        PlayAnimation(IDLE);
    }
}
