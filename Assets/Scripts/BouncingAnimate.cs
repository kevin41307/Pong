using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingAnimate : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    //Param
    static readonly int m_HashCollisionEnter = Animator.StringToHash("CollisionEnter");
    static readonly int m_HashDot = Animator.StringToHash("Dot");

    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetTrigger(m_HashCollisionEnter);
    }
    private void Update()
    {
        float dot = Mathf.Abs( Vector2.Dot(rb.velocity.normalized, Vector2.up));
        animator.SetFloat(m_HashDot, dot);
    }
}
