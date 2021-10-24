using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;
public class Ball : MonoBehaviour, IPooled<Ball>, ITransformFollower, IOwnerShip
{
    public event System.Action<GameObject> OnDisabledTheBall;
    //Components
    Animator animator;
    public Rigidbody2D RigidBody2D { private set; get; }
    public Collider2D myCollider { set; get; }
    public GameObject owner { set; get; }
    public TrailRenderer trailRenderer;
    public SpriteRenderer spriteRenderer;

    private Vector3 startPos;
    private Vector3 defaultScale = new Vector3(0.4f, 0.4f, 1f) ;
    private float moveSpeed = 10f;
    private float directionToZero;
    private float dot;
    private float ballStopTimer = 5f;
    private float sinceServeTimer = 0f;
    private float boundCheckTimer;
    private static readonly float m_MaxDistance = 100f;
    private Coroutine ServeCoroutine;

    private const float k_DefaultBallMaxVelocity = 70f;
    public float MaxBallVelocity { get; } = 35f;
    public int poolID { get; set; }
    public ObjectPooler<Ball> pool { get; set; }
    public Transform target { get; set; }
    [SerializeField]
    private Vector3 m_followOffset;
    public Vector3 followOffset { get => m_followOffset; set => m_followOffset = value; }
    public bool pauseFollow { get; set; }
    private float m_NormalBallVelocity = 17f;
    private bool isServed = false;
    private bool isPortalIgnoring = false;
    private bool isPortalEnabled = false;
    private Gradient trailGradient;
    private float scaleMultiplier = 1f;

    public Gradient overSpeedGradient;

    public VolatilizeVisualEffect hitPS;
    protected ObjectPooler<VolatilizeVisualEffect> m_hitPSPool;
    private int hitPSPoolSize = 3;
    private bool playHitVFX;
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
        myCollider = GetComponent<Collider2D>();
        startPos = transform.position;



    }

    public void Reset()
    {
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
        RigidBody2D.velocity = Vector3.zero;
        RigidBody2D.angularVelocity = 0;
        ballStopTimer = 5f;
        sinceServeTimer = 0f;
        boundCheckTimer = 0f;
        if (ServeCoroutine != null) StopCoroutine(ServeCoroutine);
        //ServeCoroutine = StartCoroutine(Serve());
    }

    private void OnEnable()
    {
        MyGameEventSystem.UseBigBallItemEvent.performed -= ChangeScale;
        MyGameEventSystem.UseBigBallItemEvent.performed += ChangeScale;
        if(owner != null)
        {
            AvatarCard card = AvatarCard.FindSpecifiedCard(owner);
            if(card.avatarID == 1)
            {
                m_followOffset.Set(0.4f, 0, 0);
            }
            else if( card.avatarID == 2 )
            {
                m_followOffset.Set(-0.4f, 0, 0);
            }
        }
        trailRenderer.enabled = true;
    }

    private void OnDisable()
    {
        MyGameEventSystem.UseBigBallItemEvent.performed -= ChangeScale;
        trailRenderer.enabled = false;
        OnDisabledTheBall?.Invoke(owner);
        owner = null;
        isServed = false;
        scaleMultiplier = 1f;
        transform.localScale = defaultScale;

    }
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(Serve());
        //subscribe event
        GameManager.OnResetGame.RemoveListener(Reset);
        GameManager.OnResetGame.AddListener(Reset);
        
        SceneLinkedSMB<Ball>.Initialise(animator, this);
        trailGradient = trailRenderer.colorGradient;
        boundCheckTimer = 0f;

        //m_hitPSPool.GetNew(hitPS.ExpiredTime).PlayPS(transform.position);
    }
    /*
    private void FixedUpdate()
    {
        Timeout();
        OverSpeed();
        //Debug.Log(owner);
    }

    void LateUpdate()
    {
        RestrictMaxBound();
#if UNITY_EDITOR
        //Debug.Log(rb.velocity.magnitude);
#endif
        RestrictMaxSpeed();
    }
    */
    public void ServeByManager(Quaternion rotation, float velocity)
    {
        isServed = true;
        transform.rotation = rotation;
        RigidBody2D.velocity = transform.right * velocity;
        StartCoroutine(EnableCollider());
    }
    IEnumerator EnableCollider()
    {
        yield return new WaitForFixedUpdate();
        myCollider.enabled = true;
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
    public void Timeout()
    {
        if (isServed == false) return;
        if (RigidBody2D.velocity.magnitude < 1f)
        {
            ballStopTimer -= Time.fixedDeltaTime;
            if (ballStopTimer < 0f)
            {
                if (transform.position.x > 0f)
                {
                    //Borderline.DecideWinner(Winner.Player);
                    MyGameEventSystem.BallEvents.Player1Goaled.Perform();
                    
                }
                else
                {
                    //Borderline.DecideWinner(Winner.Computer);
                    MyGameEventSystem.BallEvents.Player2Goaled.Perform();
                }
                pool.Free(this);
            }
        }
        else
        {
            ballStopTimer = 3f;
        }
        
    }
    public void RestrictMaxSpeed()
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
                    //Borderline.DecideWinner(Winner.Player);
                    MyGameEventSystem.BallEvents.Player1Goaled.Perform();
                }
                else
                {
                    //Borderline.DecideWinner(Winner.Computer);
                    MyGameEventSystem.BallEvents.Player2Goaled.Perform();
                }
            }
            boundCheckTimer = 0f;
        }
    }
    public void OverSpeed()
    {
        if(RigidBody2D.velocity.magnitude > 16f)
        {
            if (trailRenderer.colorGradient != overSpeedGradient)
                trailRenderer.colorGradient = overSpeedGradient;
            RigidBody2D.drag = (RigidBody2D.velocity.magnitude - m_NormalBallVelocity) * 0.12f + 0.5f;
        }
        else if(RigidBody2D.velocity.magnitude > 12f)
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

    public void VFX_PoolInitialize()
    {
        m_hitPSPool = new ObjectPooler<VolatilizeVisualEffect>();
        m_hitPSPool.Initialize(hitPSPoolSize, hitPS);
    }
    public void EnterIdle()
    {
        animator.ResetTrigger(m_HashCollisionEnter);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        dot = Mathf.Abs(Vector2.Dot(collision.contacts[0].normal, Vector2.up));
        playHitVFX = true;
        
        PlayImpactAnimation();
        if(collision.transform.CompareTag("Wall"))
        {
            if (MyGameEventSystem.UsePortalItemEvent.isPortalEnabled)
            //if(true)
            {
                playHitVFX = false;
                if (isPortalIgnoring == false && gameObject.activeInHierarchy)
                {
                    float newY = default;
                    if (transform.position.y > 0)
                    {
                        newY = -4.5f;
                    }
                    if (transform.position.y <= 0)
                    {
                        newY = 4.5f;
                    }
                    transform.position = new Vector3(transform.position.x, newY, 0);
                    isPortalIgnoring = true;
                    trailRenderer.enabled = false;
                    Vector2 velocity = RigidBody2D.velocity;
                    RigidBody2D.velocity = RigidBody2D.velocity.normalized;
                    StartCoroutine(DelayResumePortalIgnoring(velocity));
                }
            }
        }
        if(collision.transform.CompareTag("Player"))
        {
            owner = collision.gameObject;
            MyGameEventSystem.BallEvents.OwnerChanged.Perform(owner);
        }

        if(playHitVFX)
            m_hitPSPool.GetNew(hitPS.ExpiredTime)?.Play(collision.GetContact(0).point, collision.GetContact(0).normal);
        //Debug.Log("" + collision.transform.name);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("BorderLine"))
        { 
            MyGameEventSystem.BallEvents.BallIn.Perform(this ,collision.gameObject);
            pool.Free(this);

        }
    }

    IEnumerator DelayResumePortalIgnoring(Vector2 resumedVelocity)
    {
        yield return new WaitForSeconds(0.16f); // just longer than trail render time 
        isPortalIgnoring = false;
        trailRenderer.enabled = true;
        RigidBody2D.velocity = resumedVelocity;
    }

    public void PlayImpactAnimation()
    {
        RigidBody2D.angularVelocity = 0;

        animator.SetFloat(m_HashDot, dot);
        animator.SetFloat(m_HashSpeedMultiplier, 1f + RigidBody2D.velocity.magnitude / MaxBallVelocity);
        animator.SetTrigger(m_HashCollisionEnter);
    }

    public void ChangeScale(GameObject applicant, float x)
    {
        if (applicant != owner) return;
        scaleMultiplier = Mathf.Clamp(scaleMultiplier + x/4f, 1f, 4f);
        transform.localScale = defaultScale * scaleMultiplier;
    }

    public void Follow()
    {
        if (pauseFollow == true) return;
        transform.position = target.transform.position + m_followOffset;
    }

    public void StopFollow(GameObject applicant)
    {
        if (applicant != target.gameObject) return;
        //Debug.Log( transform.name +" " + owner + "/ applicant: " + applicant.transform.name);
        pauseFollow = true;
    }

    public void StartFollow(Transform _target)
    {
        target = _target;
        pauseFollow = false;
        myCollider.enabled = false;
    }
    

    
}
