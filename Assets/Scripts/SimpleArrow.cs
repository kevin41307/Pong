using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleArrow : MonoBehaviour
{
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        transform.right = rb.velocity.normalized;
    }
}
