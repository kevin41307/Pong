using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MobileDebug : MonoBehaviourSingletonPersistent<MobileDebug>
{
    public TextMeshProUGUI textMeshProUGUI;
    public string Buffer0 { set; get; }


    public void ShowMessage(string msg)
    {
        textMeshProUGUI.text = msg + Buffer0;
    }


}
