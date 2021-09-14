using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Wall : MonoBehaviour
{
    public static UnityEvent<Vector3> onBallCollisionEnter = new UnityEvent<Vector3>();
    public bool wallTop = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("ball"))
        {
            //Debug.Log("ballIn " + collision.GetContact(0).point);
            if(onBallCollisionEnter != null)
            {
                onBallCollisionEnter.Invoke(collision.GetContact(0).point);
            }
        }
    }
}
