using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Borderline : MonoBehaviour
{
    public static event System.Action OnPlayerGoaled;
    public static event System.Action OnComputerGoaled;

    public bool isPlayerBorder;
    public static void DecideWinner(Winner who)
    {
        switch (who)
        {
            case Winner.Player:
                OnPlayerGoaled?.Invoke();
                break;
            case Winner.Computer:
                OnComputerGoaled?.Invoke();
                break;
            default:
                OnPlayerGoaled?.Invoke();
                break;
        }
    }

    private void OnDestroy()
    {
        OnPlayerGoaled = null;
        OnComputerGoaled = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ball"))
        {
            if (isPlayerBorder)
            {
                if (OnComputerGoaled != null)
                    OnComputerGoaled.Invoke();
            }
            else
            {
                if (OnPlayerGoaled != null)
                    OnPlayerGoaled.Invoke();
            }

        }
    }

}

public enum Winner
{
    Player,
    Computer
}