using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GravityEffectListener : MonoBehaviour
{
    Rigidbody rb;
    float timer = 0f;
    bool isImpacting = false;
    Vector4 direction;
    public float effectForce = 20f;
    public float resumedForce = 30f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        MyGameEventSystem.UseGravityChangedItemEvent.performed += ApplyGravity;
        MyGameEventSystem.UseGravityChangedItemEvent.canceled += CancelGravity;

    }

    private void CancelGravity()
    {
        isImpacting = false;
        timer = 0f;
    }

    private void ApplyGravity(Vector4 _direction, float _duration)
    {
        direction = _direction;
        timer = _duration;
        isImpacting = true;

    }

    private void FixedUpdate()
    {
        if(isImpacting)
        {
            if (timer > 0f)
            {
                rb.AddForce(direction * effectForce * Physics.gravity.magnitude * Time.fixedDeltaTime, ForceMode.Acceleration);
                rb.AddRelativeTorque(Vector3.forward * Physics.gravity.magnitude * 1.5f * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
            else
            {
                isImpacting = false;
            }
            timer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce( Physics.gravity * resumedForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }
}
