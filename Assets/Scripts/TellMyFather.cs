using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TellMyFather : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (transform.parent != null)
            transform.parent.SendMessage("OnCollisionEnter2D", collision, SendMessageOptions.DontRequireReceiver);
    }
}
