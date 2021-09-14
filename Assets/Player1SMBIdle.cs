using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

public class Player1SMBIdle : SceneLinkedSMB<PlayerControll>
{
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.EnterIdle();

    }
}
