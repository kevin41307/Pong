using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ScoreBoard : MonoBehaviour
{
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI computerScoreText;
    int playerScore;
    int computerScore;

    private void Start()
    {
        Borderline.OnComputerGoaled += AddComputerScore;
        Borderline.OnPlayerGoaled += AddPlayerScore;
    }

    void AddPlayerScore()
    {
        playerScore++;
        playerScoreText.text = playerScore.ToString();
    }

    void AddComputerScore()
    {
        computerScore++;
        computerScoreText.text = computerScore.ToString();

    }

}
