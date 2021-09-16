using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
public class SettleCanvas : MonoBehaviour
{
    string massage;
    public GameObject settleCanvas;
    public TextMeshProUGUI settleText;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnPressAnyKeyToContinue.AddListener(HideMsg);
        Borderline.OnComputerGoaled += () => { ShowMsg("Computer wins!"); };
        Borderline.OnPlayerGoaled += () => { ShowMsg("Player wins!"); };

    }

    void ShowMsg(string msg)
    {
        settleCanvas.SetActive(true);
        settleText.text = msg;
    }
    void HideMsg()
    {
        settleText.text = "";
        settleCanvas.SetActive(false);
    }
}
