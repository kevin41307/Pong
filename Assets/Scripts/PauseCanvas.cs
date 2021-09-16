using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvas : MonoBehaviour
{
    public Button pauseButton;
    public GameObject pausePanel;
    public Button strikeTypeButton;
    public Button balanceTypeButton;
    public Button accuracyTypeButton;
    public Button casualTypeButton;

    public PlayerControll player; //TODO: get play instance from gm etc...
    private bool pausePanelActive;

    private void Awake()
    {
        pausePanelActive = false;
        pauseButton.onClick.AddListener( () => ActivePausePanel(!pausePanelActive));
        strikeTypeButton.onClick.AddListener(BecomeStrikeType);
        balanceTypeButton.onClick.AddListener(BecomeBalanceType);
        accuracyTypeButton.onClick.AddListener(BecomeAccuracyType);
        casualTypeButton.onClick.AddListener(BecomeCasualType);
    }
    void ActivePausePanel(bool _active)
    {
        pausePanel.SetActive(_active);
        pausePanelActive = !pausePanelActive;

    }


    void BecomeStrikeType()
    {
        player.ApplyPlaneStyleParameter(PlaneStyle.Strike);
    }

    void BecomeBalanceType()
    {
        player.ApplyPlaneStyleParameter(PlaneStyle.Balance);
    }
    void BecomeAccuracyType()
    {
        player.ApplyPlaneStyleParameter(PlaneStyle.Accuracy);
    }
    void BecomeCasualType()
    {
        player.ApplyPlaneStyleParameter(PlaneStyle.Casual);
    }
}
