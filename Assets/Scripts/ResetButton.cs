using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResetButton : MonoBehaviour
{

    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(ResetGameButton);
    }

    void ResetGameButton()
    {
        GameManager.Instance.ResetGame();
    }
}
