using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Computer : MonoBehaviour
{
    Ball[] balls;
    Vector2 tracePoint;
    float moveSpeed = 13f;
    Rigidbody2D rb;
    Vector2 targetPos;
    Vector2 startPos;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        balls = FindObjectsOfType<Ball>();
    }

    private void Start()
    {
        startPos = transform.position;
        GameManager.onResetGame.AddListener(Reset);
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        UpdateTracePoint();
        targetPos = Vector3.MoveTowards(transform.position, tracePoint, moveSpeed * Time.fixedDeltaTime) * Vector2.up;
        if(Mathf.Abs(transform.position.y - tracePoint.y) > 0.1f)
            rb.MovePosition(targetPos);

    }

    void UpdateTracePoint()
    {
        if (balls.Length <= 0) return;
        Vector3 previousResult = tracePoint;
        Vector3 result = Vector3.zero;
        for (int i = 0; i < balls.Length; i++)
        {
            result += balls[i].transform.position;
            
        }
        result /= balls.Length;
        if (result.x < -3) return;
        tracePoint = result;
        
    }

    private void Reset()
    {
        transform.rotation = Quaternion.identity;
        transform.position = startPos;
        rb.velocity = Vector3.zero;
    }
}
