using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

public class BalanceTypeIdleSMB : SceneLinkedSMB<BalanceTypeController>
{
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_MonoBehaviour == null) return;
        m_MonoBehaviour.EnterIdle();
    }
}
