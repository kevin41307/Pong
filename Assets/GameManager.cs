using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    public static UnityEvent onPressAnyKeyToContinue = new UnityEvent();
    public static UnityEvent onResetGame = new UnityEvent();

    bool isListenAnyKeyDown = false;


    private void Start()
    {
        Borderline.onPlayerGoal.AddListener(() =>
        {
            GamePause();
            PressAnyKeyToContinue();
        });
        Borderline.onComputerGoal.AddListener(() =>
        {
            GamePause();
            PressAnyKeyToContinue();
        });
    }
    private void Update()
    {
        if (isListenAnyKeyDown)
            PressAnyKeyToContinue();
    }
    void GamePause()
    {
        Time.timeScale = 0f;
        isListenAnyKeyDown = true;
    }

    void PressAnyKeyToContinue()
    {
        if (Input.anyKeyDown)
        {
            if (onPressAnyKeyToContinue != null)
                onPressAnyKeyToContinue.Invoke();
            isListenAnyKeyDown = false;
            ResetGame();
        }
    }

    void ResetGame()
    {
        if(onResetGame != null)
        {
            onResetGame.Invoke();
        }
        Time.timeScale = 1f;
    }

}