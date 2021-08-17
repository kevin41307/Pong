using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    Rigidbody2D rb;
    static Rigidbody2D rbExternal;
    float moveSpeed = 10f;
    Vector3 startPos;
    float ballStopTimer = 5f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rbExternal = GetComponent<Rigidbody2D>();
    }

    private void Reset()
    {
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        ballStopTimer = 5f;
        StartCoroutine(Serve());

    }
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        GameManager.onResetGame.AddListener(Reset);
        StartCoroutine(Serve());

    }

    private void FixedUpdate()
    {
        Timeout();
        //rb.AddTorque(10f * Time.deltaTime);
    }


    IEnumerator Serve()
    {
        yield return new WaitForSeconds(1f);
        Vector2 randomForce = new Vector2(Random.Range(-1f, 1f), Random.Range(-.7f, .7f)).normalized;
        rb.AddForce(randomForce * 500f);
    }
    
    void Timeout()
    {
        //Debug.Log(rb.velocity.magnitude);
        if (rb.velocity.magnitude < 1.8f)
        {
            ballStopTimer -= Time.fixedDeltaTime;
            if (ballStopTimer < 0f)
            {
                if (transform.position.x > 0f)
                {
                    if (Borderline.onPlayerGoal != null)
                    {
                        Borderline.onPlayerGoal.Invoke();
                    }
                }
                else
                {
                    if (Borderline.onComputerGoal != null)
                    {
                        Borderline.onComputerGoal.Invoke();
                    }

                }
            }
        }
        else
        {
            ballStopTimer = 3f;
        }
        
    }

    public void maxSpeed()
    {
    }
}
