using System;
using UnityEngine;

public class ExitStateBehavior : StateMachineBehaviour
{
    public string parameterNameOfExitState;
    public string parameterValueOfExitState;
    AnimatorControllerParameterType? chachedType;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (string.IsNullOrEmpty(parameterNameOfExitState) || string.IsNullOrEmpty(parameterValueOfExitState)) return;

        if (chachedType == null) SearchType(animator);
        if (chachedType == null) return;

        switch (chachedType)
        {
            case AnimatorControllerParameterType.Int:
                if (int.TryParse(parameterValueOfExitState, out int intValue))
                    animator.SetInteger(parameterNameOfExitState, intValue);
                else
                    Debug.LogWarning($"[StateBehavior] Invalid int value: {parameterValueOfExitState}");
                break;

            case AnimatorControllerParameterType.Float:
                if (float.TryParse(parameterValueOfExitState, out float floatValue))
                    animator.SetFloat(parameterNameOfExitState, floatValue);
                else
                    Debug.LogWarning($"[StateBehavior] Invalid float value: {parameterValueOfExitState}");
                break;

            case AnimatorControllerParameterType.Bool:
                if (bool.TryParse(parameterValueOfExitState, out bool boolValue))
                    animator.SetBool(parameterNameOfExitState, boolValue);
                else
                    Debug.LogWarning($"[StateBehavior] Invalid bool value: {parameterValueOfExitState}");
                break;

            case AnimatorControllerParameterType.Trigger:
                bool.TryParse(parameterNameOfExitState, out bool value);
                if (value)
                    animator.SetTrigger(parameterNameOfExitState);
                else
                    animator.ResetTrigger(parameterNameOfExitState);
                break;

            default:
                Debug.LogWarning($"[StateBehavior] Unsupported parameter type: {chachedType}");
                break;
        }
    }

    private void SearchType(Animator animator)
    {
        foreach (var param in animator.parameters)
        {
            if (param.name == parameterNameOfExitState)
            {
                chachedType = param.type;
                break;
            }
        }
    }

}