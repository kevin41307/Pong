using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Computer : MonoBehaviour
{
    Ball[] balls;
    Vector2 tracePoint;
    Rigidbody2D rb;
    Vector2 targetPos;
    Vector2 startPos;

    float moveSpeed = 20f;
    Vector2 currentVelocity;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        balls = FindObjectsOfType<Ball>();
    }

    private void Start()
    {
        startPos = transform.position;
        GameManager.OnResetGame.AddListener(Reset);
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        UpdateTracePoint();
        Vector2.SmoothDamp(rb.position * Vector2.up, tracePoint * Vector2.up, ref currentVelocity, 2f * Time.fixedDeltaTime, moveSpeed, Time.fixedDeltaTime);
        rb.velocity = currentVelocity;

        /*
        targetPos = Vector3.MoveTowards(rb.position, tracePoint, moveSpeed * Time.fixedDeltaTime) * Vector2.up;
        float speed = (tracePoint.y - rb.position.y) / Time.fixedDeltaTime;
        if (Mathf.Abs(rb.position.y - tracePoint.y) > 0.1f)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(0, speed), Time.fixedDeltaTime);
            Debug.Log(rb.velocity);
            //rb.MovePosition(targetPos);
        }
        */
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
        if (result.x < -6) return;
        tracePoint = result;
        
    }

    private void Reset()
    {
        transform.rotation = Quaternion.identity;
        transform.position = startPos;
        rb.velocity = Vector3.zero;
    }
}
