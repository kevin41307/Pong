using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;
public class Ball : MonoBehaviour
{
    //Components
    Animator animator;

    public Rigidbody2D RigidBody2D { private set; get; }
    public TrailRenderer trailRenderer;
    public SpriteRenderer spriteRenderer;

    private Vector3 startPos;
    private float moveSpeed = 10f;
    private float directionToZero;
    private float dot;
    private float ballStopTimer = 5f;
    private float sinceServeTimer = 0f;
    private float boundCheckTimer;
    private static readonly float m_MaxDistance = 100f;
    private Coroutine ServeCoroutine;

    private const float k_DefaultBallMaxVelocity = 80f;
    public float MaxBallVelocity { get; } = 45f;
    private float m_NormalBallVelocity = 17f;

    private Gradient trailGradient;
    public Gradient overSpeedGradient;

    //prevent ball pass through bound but not detected.
    const float k_restrictMaxBoundTimer = 1f;

    // Parameter
    static readonly int m_HashCollisionEnter = Animator.StringToHash("CollisionEnter");
    static readonly int m_HashDot = Animator.StringToHash("Dot");
    static readonly int m_HashSpeedMultiplier = Animator.StringToHash("SpeedMultiplier");
    

    private void Awake()
    {
        RigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Reset()
    {
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
        RigidBody2D.velocity = Vector3.zero;
        RigidBody2D.angularVelocity = 0;
        ballStopTimer = 5f;
        sinceServeTimer = 0f;
        boundCheckTimer = 0f;
        if (ServeCoroutine != null) StopCoroutine(ServeCoroutine);
        ServeCoroutine = StartCoroutine(Serve());

    }
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        StartCoroutine(Serve());
        //subscribe event
        GameManager.OnResetGame.RemoveListener(Reset);
        GameManager.OnResetGame.AddListener(Reset);
        SceneLinkedSMB<Ball>.Initialise(animator, this);
        trailGradient = trailRenderer.colorGradient;
        boundCheckTimer = 0f;


    }

    private void FixedUpdate()
    {
        Timeout();
        OverSpeed();
    }
    
    void LateUpdate()
    {

        RestrictMaxBound();
#if UNITY_EDITOR
        //Debug.Log(rb.velocity.magnitude);
#endif
        RestrictMaxSpeed();
    }

    IEnumerator Serve()
    {
        yield return new WaitForSeconds(1.5f);
        if(RigidBody2D.simulated)
        {
            Vector2 randomForce = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.02f, 0.02f)).normalized;
            //rb.AddForce(randomForce * 500f);
            //RigidBody2D.velocity = randomForce * MaxBallVelocity;
            PlayImpactAnimation();
        }
    }
    
    void Timeout()
    {
        if (RigidBody2D.velocity.magnitude < 1.8f)
        {
            ballStopTimer -= Time.fixedDeltaTime;
            if (ballStopTimer < 0f)
            {
                if (transform.position.x > 0f)
                {
                    Borderline.DecideWinner(Winner.Player);
                }
                else
                {
                    Borderline.DecideWinner(Winner.Computer);
                }
            }
        }
        else
        {
            ballStopTimer = 3f;
        }
        
    }

    void RestrictMaxSpeed()
    {   
        RigidBody2D.velocity = RigidBody2D.velocity.normalized * Mathf.Min(k_DefaultBallMaxVelocity , RigidBody2D.velocity.magnitude);
    }


    public void RestrictMaxBound()
    {
        boundCheckTimer += Time.deltaTime;

        if( boundCheckTimer > k_restrictMaxBoundTimer)
        {
            directionToZero = (RigidBody2D.position - Vector2.zero).magnitude;
            if (directionToZero * directionToZero > m_MaxDistance * m_MaxDistance)
            {
                if(transform.position.x >= 0)
                {
                    Borderline.DecideWinner(Winner.Player);
                }
                else
                {
                    Borderline.DecideWinner(Winner.Computer);
                }
            }
            boundCheckTimer = 0f;
        }
    }

    void OverSpeed()
    {
        if(RigidBody2D.velocity.magnitude > 29f)
        {
            if (trailRenderer.colorGradient != overSpeedGradient)
                trailRenderer.colorGradient = overSpeedGradient;
            RigidBody2D.drag = (RigidBody2D.velocity.magnitude - m_NormalBallVelocity) * 0.12f + 0.5f;
        }
        else if(RigidBody2D.velocity.magnitude > 23f)
        {
            if(trailRenderer.colorGradient != overSpeedGradient)
                trailRenderer.colorGradient = overSpeedGradient;
            RigidBody2D.drag = 0.5f;
        }
        else if(RigidBody2D.velocity.magnitude > m_NormalBallVelocity)
        {
            if (trailRenderer.colorGradient != overSpeedGradient)
                trailRenderer.colorGradient = overSpeedGradient;
            RigidBody2D.drag = 0.1f;
        }
        else
        {
            if (trailRenderer.colorGradient != trailGradient)
                trailRenderer.colorGradient = trailGradient;
            RigidBody2D.drag = 1e-05f;
        }

    }


    public void EnterIdle()
    {
        animator.ResetTrigger(m_HashCollisionEnter);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        dot = Mathf.Abs(Vector2.Dot(collision.contacts[0].normal, Vector2.up));
        PlayImpactAnimation();
    }

    public void PlayImpactAnimation()
    {
        RigidBody2D.angularVelocity = 0;

        animator.SetFloat(m_HashDot, dot);
        animator.SetFloat(m_HashSpeedMultiplier, 1f + RigidBody2D.velocity.magnitude / MaxBallVelocity);
        animator.SetTrigger(m_HashCollisionEnter);
    }
    
}
