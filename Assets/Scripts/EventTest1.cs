using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTest1 : MonoBehaviour
{
    public delegate void Player1(EventTest1 id);
    public static event Player1 OnPlayer1;
    public bool isPlayer1;
    private void OnEnable()
    {
        OnPlayer1 += EventTest1_OnPlayer1;
    }
    private void OnDisable()
    {
        OnPlayer1 -= EventTest1_OnPlayer1;
    }

    private void EventTest1_OnPlayer1(EventTest1 id)
    {
        if (id != this) return;
        Debug.Log("aaa" + transform.name);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            OnPlayer1?.Invoke(this);
        }
    }
}
