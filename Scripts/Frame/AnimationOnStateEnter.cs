using UnityEngine;

public class AnimationOnStateEnter : StateMachineBehaviour
{
    [SerializeField] public float speed = 1f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = speed;
    }
}
