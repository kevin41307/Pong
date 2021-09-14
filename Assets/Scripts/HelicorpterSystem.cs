using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicorpterSystem : MonoBehaviour
{
    public float engineForce =1000f;
    float rotationForce = 50f;
    private Rigidbody2D rb;

    public Transform fan;
    public float maxFanSpeed = 2000;

    private float currentFanSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        fan.Rotate(0, 0, currentFanSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.up * engineForce * Time.deltaTime);
            currentFanSpeed = Mathf.Lerp(currentFanSpeed, maxFanSpeed, Time.deltaTime);
        }
        else
        {
            currentFanSpeed = Mathf.Lerp(currentFanSpeed, 0, Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationForce * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationForce * Time.deltaTime);
        }

    }
}
