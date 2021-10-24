using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(2)]
public class Borderline : MonoBehaviour //Control the start invocation list of endGameType1 
{
    public bool isPlayer1Border;
    public GameObject owner { private set; get; }
    private void Start()
    {
        if(isPlayer1Border)
        {
            AvatarCard card = AvatarCard.FindSpecifiedCard(1);
            owner = card.avatar;
        }
        else
        {
            AvatarCard card = AvatarCard.FindSpecifiedCard(2);
            owner = card.avatar;
        }
    }
}