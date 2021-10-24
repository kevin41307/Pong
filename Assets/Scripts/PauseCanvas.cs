using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class PauseCanvas : MonoBehaviourSingleton<PauseCanvas>
{
    public Button pauseButton;
    public GameObject pausePanel;
    public Button strikeTypeButton;
    public Button balanceTypeButton;
    public Button accuracyTypeButton;
    public Button casualTypeButton;

    public PlayerControl player; //TODO: get play instance from gm etc...
    private bool pausePanelActive;

    private void Awake()
    {
        pausePanelActive = false;
        pauseButton.onClick.AddListener( () => ActivePausePanel(!pausePanelActive));
        strikeTypeButton.onClick.AddListener(BecomeStrikeType);
        balanceTypeButton.onClick.AddListener(BecomeBalanceType);
        accuracyTypeButton.onClick.AddListener(BecomeAccuracyType);
        casualTypeButton.onClick.AddListener(BecomeCasualType);
        PlayerInstanceManager.Instance.OnInstantiatePlayer += SetCurrentPlayerGameObject;
    }
    private void Start()
    {
    }
    private void OnDestroy()
    {
        if(PlayerInstanceManager.Instance != null)
            PlayerInstanceManager.Instance.OnInstantiatePlayer -= SetCurrentPlayerGameObject;
    }


    void ActivePausePanel(bool _active)
    {
        pausePanel.SetActive(_active);
        pausePanelActive = !pausePanelActive;

    }
    public void SetCurrentPlayerGameObject(int id, GameObject go)
    {
        player = go.GetComponent<PlayerControl>();
    }

    void BecomeStrikeType()
    {
        PlayerInstanceManager.Instance.NewPlayerAtLastTransform(PlaneStyle.Strike);
    }

    void BecomeBalanceType()
    {
        PlayerInstanceManager.Instance.NewPlayerAtLastTransform(PlaneStyle.Balance);
    }
    void BecomeAccuracyType()
    {
        PlayerInstanceManager.Instance.NewPlayerAtLastTransform(PlaneStyle.Accuracy);
    }
    void BecomeCasualType()
    {
        PlayerInstanceManager.Instance.NewPlayerAtLastTransform(PlaneStyle.Casual);
    }
}
