using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviourSingletonPersistent<GameManager>
{
    public static UnityEvent OnPressAnyKeyToContinue = new UnityEvent();
    public static UnityEvent OnResetGame = new UnityEvent();

    bool isListenAnyKeyDown = false;

    public GameObject virtualJoystickCanvas;

    private void Start()
    {
        //Borderline.OnPlayerGoaled += EndGameType1;
        //Borderline.OnComputerGoaled += EndGameType1;

#if UNITY_ANDROID
        virtualJoystickCanvas.SetActive(true);
#endif

    }
    private void Update()
    {
        if (isListenAnyKeyDown)
            PressAnyKeyToContinue();
    }
    void GamePauseAndListenAnyKey()
    {
        Time.timeScale = 0f;
        isListenAnyKeyDown = true;
    }
    
    void EndGameType1()
    {
        GamePauseAndListenAnyKey();
        PressAnyKeyToContinue();
    }


    public void GamePause()
    {
        Time.timeScale = 0f;
    }

    public void GameResume()
    {
        Time.timeScale = 1f;
    }

    void PressAnyKeyToContinue()
    {
        if (Input.anyKeyDown)
        {
            if (OnPressAnyKeyToContinue != null)
                OnPressAnyKeyToContinue.Invoke();
            isListenAnyKeyDown = false;
            ResetGame();
        }
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
        if(OnResetGame != null)
        {
            OnResetGame.Invoke();
        }
    }

}