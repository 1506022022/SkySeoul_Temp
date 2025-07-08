using UnityEngine;

public class AnimatorClipToggle : StateMachineBehaviour
{
    [SerializeField] bool toggle;
    public string property;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        toggle = !toggle;
        if (!string.IsNullOrEmpty(property)) animator.SetBool(property, toggle);
    }
}