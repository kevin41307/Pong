using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMissile : MonoBehaviour
{

    Rigidbody2D rb;
    float rotateSpeed = 300f;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 direction = (Vector2)target.transform.position - rb.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.right).z;
        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.velocity = transform.right * 20f;
        //Debug.Log("Z" + rotateAmount);
    }
}
