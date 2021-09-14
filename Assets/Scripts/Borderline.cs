using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Borderline : MonoBehaviour
{
    public static UnityEvent onPlayerGoal = new UnityEvent();
    public static UnityEvent onComputerGoal = new UnityEvent();

    public bool isPlayerBorder;

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ball"))
        {
            if (isPlayerBorder)
            {
                if (onComputerGoal != null)
                    onComputerGoal.Invoke();
            }
            else
            {
                if (onPlayerGoal != null)
                    onPlayerGoal.Invoke();
            }

        }
    }





}
