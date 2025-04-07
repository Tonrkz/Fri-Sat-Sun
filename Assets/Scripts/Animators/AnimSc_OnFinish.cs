using UnityEngine;

public class AnimSc_OnFinish : StateMachineBehaviour {
    [SerializeField] string animation;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.GetComponent<AnimatorRenderer>().PlayAnimation(animation, 0.2f, 1, stateInfo.length);
    }
}
