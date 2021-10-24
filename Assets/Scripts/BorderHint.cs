using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class BorderHint : MonoBehaviour
{
    SpriteRenderer sr;
    GameObject player;
    Color startColor;
    Color currentColor;
    public bool isPlayer1;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
        currentColor = startColor;
    }

    private void Start()
    {
        player = (isPlayer1) ? AvatarCard.FindSpecifiedCard(1).avatar : AvatarCard.FindSpecifiedCard(2).avatar;
    }

    private void Update()
    {
        sr.color = new Color(startColor.r, startColor.g, startColor.b, startColor.a * (2.5f - Mathf.Clamp(Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).magnitude, 0.3f, 2.5f)));
    }

}
