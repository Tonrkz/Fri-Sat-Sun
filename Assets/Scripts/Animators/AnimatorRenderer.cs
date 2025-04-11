using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class AnimatorRenderer : MonoBehaviour {
    [Header("References")]
    [SerializeField] public Animator animator; // Reference to the animator

    [Header("Animation State Names")]
    public readonly string BUILDING = "Building"; // The building animation state name
    public readonly string IDLE = "Idle"; // The idle animation state name
    public readonly string WALK = "Walk"; // The walk animation state name
    public readonly string UPGRADE = "Upgrade"; // The upgrade animation state name
    public readonly string ACTIVATE = "Activate"; // The run animation state name
    public readonly string ATTACK = "Attack"; // The attack animation state name
    public readonly string HURT = "Hurt"; // The attack animation state name
    public readonly string DEAD = "Dead"; // The dead animation state name

    [Header("Debug")]
    string currentAnimation; // The current animation that is playing

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string animationName, Single crossfadeDuration = 0.2f, Single playingSpeed = 1f, Single delay = 0f) {
        if (delay > 0) {
            StartCoroutine(Wait());
            return;
        }

        Validate();
        return;

        IEnumerator Wait() {
            yield return new WaitForSeconds(delay - crossfadeDuration);
            Validate();
        }

        void Validate() {
            if(animationName == "") {
                animator.CrossFade(IDLE, 0);
                animator.speed = playingSpeed;
            }
            else if (animationName != currentAnimation) {
                animator.CrossFade(animationName, crossfadeDuration);
                animator.speed = playingSpeed;
            }
        }

    }

    public void FlipSprite(bool flip) {
        GetComponent<SpriteRenderer>().flipX = flip;
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
        else if (transform.parent.gameObject.CompareTag("Demon") || transform.parent.gameObject.CompareTag("Phantom")) {
            StartCoroutine(GetComponentInParent<ADemons>().Dead());
        }
    }

    public void AnimNotifyToIdle() {
        PlayAnimation(IDLE);
    }
}
