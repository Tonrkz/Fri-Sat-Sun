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
    public readonly string DEAD = "Dead"; // The dead animation state name

    [Header("Debug")]
    string currentAnimation; // The current animation that is playing

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string animationName, Single crossfadeDuration = 0.2f) {
        if (animationName != currentAnimation) {
            animator.CrossFade(animationName, crossfadeDuration);
        }
    }
}
