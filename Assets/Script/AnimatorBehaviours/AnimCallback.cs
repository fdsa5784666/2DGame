using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCallback : StateMachineBehaviour
{
    public event Action OnComplete;

    // 动画播完、离开这个状态时自动调用
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnComplete?.Invoke();
    }

}
