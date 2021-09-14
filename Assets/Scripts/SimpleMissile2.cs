using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMissile2 : MonoBehaviour
{
    public Transform target; 
    public float Speed;
    public float Acceleration;

    Rigidbody2D rb;

    public float RotationControl;

    float MovY, MovX = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        MovY = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        Vector2 direction = transform.position - target.position;
        direction.Normalize();
        float cross = Vector3.Cross(direction, transform.right).z;
        rb.angularVelocity = cross * RotationControl;

        Vector2 Vel = transform.right * (MovX * Acceleration);
        rb.AddForce(Vel);

        float thrustForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.down)) * 2.0f;

        Vector2 relForce = Vector2.up * thrustForce;

        rb.AddForce(rb.GetRelativeVector(relForce));


        if (rb.velocity.magnitude > Speed)
        {
            rb.velocity = rb.velocity.normalized * Speed;
        }
    }
}