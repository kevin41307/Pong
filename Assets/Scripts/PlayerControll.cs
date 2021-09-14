using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

public class PlayerControll : MonoBehaviour
{
    //Components
    Rigidbody2D rb;
    UserInput userInput;
    Animator animator;
    private SpriteRenderer sr;
    private PlayerInputActions inputActions;
    public Sniper sniper;

    //events
    public delegate void SizeChanged();
    public event SizeChanged OnSizeChanged;
    public delegate void TypeChanged();
    public event TypeChanged OnTypeChanged;

    public EnclosureArea2D moveableArea;
    Collider2D m_collider;

    public EnclosureArea2D ClippedMoveableArea { private set; get; }

    //Player Style
    public PlaneStyle playerStyle;
    public MoveableDimension moveableDimension;

    private float vertical;
    private float m_MoveSpeedDefault = 50f; // OnlyChangeInPlaneStyle
    private float moveSpeed;
    private float speedMultiplier = 0.6f;
    private Vector2 startPos;
    private Vector2 nextPosition;
    private float m_scaleY;
    private Vector2 currentVelocity;
    private float lastRotation;  
    private const float k_AbsorbRadius = 9f;
    private int getBallsCount;
    private Collider2D[] getBallsResults = new Collider2D[10];

    public bool IsChargeBtn { get; private set; }
    private float baseCounterForce = 400f;
    private float counterForceMultiplier = 1f;
    private float chargePower;
    private bool chargeFullBuff;
    private float chargePosX;
    private Coroutine chargeFullCotoutine;
    private WaitForSeconds m_ChargeFullWait;
    private const float k_ChargeFullDuration = 0.25f;

    //State
    readonly int m_HashStartGlow = Animator.StringToHash("StartGlow");
    readonly int m_HashGlowing = Animator.StringToHash("Glowing");

    //Parameter
    readonly int m_HashChargeFull = Animator.StringToHash("ChargeFull");
    readonly int m_HashChargeBtn = Animator.StringToHash("ChargeBtn");

    protected bool IsMoveInput
    {
        get { return !Mathf.Approximately(userInput.MoveInput.y, 0f); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        userInput = GetComponent<UserInput>();
        m_ChargeFullWait = new WaitForSeconds(k_ChargeFullDuration);
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider2D>();
        

    }
    private void Reset()
    {
        transform.rotation = Quaternion.identity;
        transform.position = startPos;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        counterForceMultiplier = 1f;

        SceneLinkedSMB<PlayerControll>.Initialise(animator, this);
    }

    private void OnEnable()
    {
        OnSizeChanged += UpdateBounds;
        OnTypeChanged += ApplyPlaneStyleParameter;
    }

    private void OnDisable()
    {
        OnSizeChanged -= UpdateBounds;
        OnTypeChanged -= ApplyPlaneStyleParameter;
    }

    private void Start()
    {
        startPos.Set(-9, 0);
        chargePosX = startPos.x - 1f;
        moveSpeed = m_MoveSpeedDefault;
        m_scaleY = transform.localScale.y;
        SceneLinkedSMB<PlayerControll>.Initialise(animator, this);
        //TODO: maybe seperate EnclosureArea2D class
        ClippedMoveableArea = gameObject.AddComponent<EnclosureArea2D>();
        ClippedMoveableArea.UpdateBounds(moveableArea.Center, m_collider.bounds.extents, moveableArea.Extent);
        inputActions = userInput.GetPlayerInputActions();
        //subscribe event
        GameManager.onResetGame.AddListener(Reset);
        OnTypeChanged?.Invoke();
    }

    private void ApplyPlaneStyleParameter()
    {
        switch (playerStyle)
        {
            case PlaneStyle.Strike:
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.simulated = true;
                rb.mass = 10000f;
                rb.drag = 0f;
                rb.angularDrag = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                moveableDimension = MoveableDimension.Face;
                baseCounterForce = 400f;
                m_MoveSpeedDefault = 70f;
                sniper.enabled = false;
                inputActions.Player.Fire1.started -= OnChargeStateStarted;
                inputActions.Player.Fire1.canceled -= OnChargeStateExited;
                break;
            case PlaneStyle.Balance:
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.simulated = true;
                rb.mass = 10000f;
                rb.drag = 0f;
                rb.angularDrag = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                moveableDimension = MoveableDimension.Line;
                baseCounterForce = 200f;
                m_MoveSpeedDefault = 50f;
                sniper.enabled = false;
                inputActions.Player.Fire1.started -= OnChargeStateStarted; // prevent multiple subscibe
                inputActions.Player.Fire1.started += OnChargeStateStarted;
                inputActions.Player.Fire1.canceled -= OnChargeStateExited;
                inputActions.Player.Fire1.canceled += OnChargeStateExited;
                break;
            case PlaneStyle.Accuracy:
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.simulated = true;
                rb.mass = 10000f;
                rb.drag = 100f;
                rb.angularDrag = 100f;
                rb.constraints = RigidbodyConstraints2D.None ;
                moveableDimension = MoveableDimension.Point;
                baseCounterForce = 400f;
                m_MoveSpeedDefault = 10000f;
                sniper.enabled = true;
                inputActions.Player.Fire1.started -= OnChargeStateStarted;
                inputActions.Player.Fire1.canceled -= OnChargeStateExited;
                break;
            case PlaneStyle.Casual:
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.simulated = true;
                rb.mass = 100f;
                rb.drag = 0f;
                rb.angularDrag = 0.01f;
                rb.constraints = RigidbodyConstraints2D.None;
                moveableDimension = MoveableDimension.Face;
                baseCounterForce = 200f;
                m_MoveSpeedDefault = 50f;
                sniper.enabled = false;
                inputActions.Player.Fire1.started -= OnChargeStateStarted;
                inputActions.Player.Fire1.canceled -= OnChargeStateExited;
                break;
            default:
                Debug.Log("playerStyle is not valid :" + playerStyle);
                break;
        }

    }
    private void OnChargeStateStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        GetBalls();
    }
    private void OnChargeStateExited(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ReleaseBalls();
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        vertical = userInput.MoveInput.y;
        //Debug.Log(userInput.PositionInput);

        //Do Charge stuff
        if (playerStyle == PlaneStyle.Balance)
            DoCharge();
    }

    private void FixedUpdate()
    {
        /*
        //Charge backforce
        ApplyChargeMovement();

        //prevent border collision
        PreventBorderOverlap();

        */
        if(playerStyle == PlaneStyle.Balance)
            DoAbsortForce();

        if (!Mathf.Approximately(lastRotation, rb.rotation))
            OnSizeChanged?.Invoke();
        if (userInput.PositionInput.magnitude != 0 && playerStyle != PlaneStyle.Accuracy) // prevent unity inside bug: pointer position is initial at vector.zero
        {
            PrepareMove();
            DoMove();
        }
         
        lastRotation = rb.rotation;
    }

    void UpdateBounds() => ClippedMoveableArea.UpdateBounds(moveableArea.Center, m_collider.bounds.extents, moveableArea.Extent);

    void PrepareMove()
    {
        switch (moveableDimension)
        {
            case MoveableDimension.Point:
                break;
            case MoveableDimension.Line:
                nextPosition.Set(rb.position.x, Mathf.Clamp(userInput.PositionInput.y, ClippedMoveableArea.Min.y, ClippedMoveableArea.Max.y));
                break;
            case MoveableDimension.Face:
                nextPosition.Set(Mathf.Clamp(userInput.PositionInput.x, ClippedMoveableArea.Min.x, ClippedMoveableArea.Max.x), Mathf.Clamp(userInput.PositionInput.y, ClippedMoveableArea.Min.y, ClippedMoveableArea.Max.y));
                break;
            default:
                nextPosition = rb.position;
                break;
        }

        switch (playerStyle)
        {
            case PlaneStyle.Strike:
                break;
            case PlaneStyle.Balance:
                ApplyChargeMovement();
                break;
            case PlaneStyle.Accuracy:
                break;
            case PlaneStyle.Casual:
                break;
            default:
                break;
        }

    }
      
    public void Sniper_Start()
    {
        Sniper.OnAimed += Sniper_Aiming;
        m_collider.enabled = false;
        SetNextPosition(userInput.PositionInput);
        DoMoveImmediately(nextPosition);
    }
    public void Sniper_Aiming()
    {
        //Debug.Log(sniper.Direction);
        rb.MoveRotation(Helpers.AngleFromDir(sniper.Direction));
        SetNextPosition(sniper.StartPosition - sniper.Direction.normalized * Mathf.Clamp(sniper.Direction.magnitude, 0.1f, 1f));
        DoMoveImmediately(nextPosition);
    }
    public void Sniper_End()
    {
        m_collider.enabled = true;
        Sniper.OnAimed -= Sniper_Aiming;
    }


    void SetNextPosition(Vector2 position)
    {
        nextPosition.Set(Mathf.Clamp(position.x, ClippedMoveableArea.Min.x, ClippedMoveableArea.Max.x), Mathf.Clamp(position.y, ClippedMoveableArea.Min.y, ClippedMoveableArea.Max.y));
    }
    void DoMove()
    {
        Vector2.SmoothDamp(rb.position, nextPosition, ref currentVelocity, Time.deltaTime, m_MoveSpeedDefault, Time.deltaTime);
        rb.velocity = currentVelocity;
    }
    void DoMoveImmediately(Vector3 position)
    {

        rb.MovePosition(nextPosition);
    }


    void ApplyCounterForceMultiply()
    {
        float x = (Mathf.Abs(rb.position.x - moveableArea.Min.x) / (moveableArea.Extent.x * 2) + 1.33f);

        counterForceMultiplier = x * x;
        //Debug.Log(counterForceMultiply);
    }
    public void PreventBorderOverlap()
    {
        if (IsMoveInput)
        {
            if (!Physics2DHelper.EasySweepTest(transform.position, nextPosition, m_scaleY * 0.5f, 1 << LayerMask.NameToLayer("Wall")))
            {
                nextPosition.y = rb.position.y;
                moveSpeed *= speedMultiplier;
            }
            else
            {
                moveSpeed = m_MoveSpeedDefault;
            }

        }
    }

    void GetBalls()
    {
        getBallsCount = Physics2D.OverlapCircleNonAlloc(rb.position, k_AbsorbRadius, getBallsResults, 1 << LayerMask.NameToLayer("Collider"));
    }

    void DoAbsortForce()
    {
        if(IsChargeBtn)
        {
            for (int i = 0; i < getBallsCount; i++)
            {
                if (getBallsResults[i].CompareTag("ball") == false) continue;
                Vector2 gravityDirection = rb.position - (Vector2)getBallsResults[i].transform.position;
                getBallsResults[i].attachedRigidbody.velocity += gravityDirection.normalized * 9.81f * Mathf.Clamp(gravityDirection.magnitude * gravityDirection.magnitude * .15f, 0, 10) * Time.deltaTime;

            }
        }
    }

    void ReleaseBalls()
    {
        for (int i = 0; i < getBallsCount; i++)
        {
            if (getBallsResults[i].CompareTag("ball") == false) continue;
            
            Vector2 direction = (Vector2)getBallsResults[i].transform.position - rb.position;
            if (direction.magnitude > k_AbsorbRadius) continue;
            //TODO: Auto Trigger PlayImpactAnimation;
            getBallsResults[i].attachedRigidbody.velocity += direction.normalized * Mathf.Clamp(k_AbsorbRadius - direction.magnitude, 0, k_AbsorbRadius);
        }
        Array.Clear(getBallsResults, 0, getBallsCount);
    }

    void DoCharge()
    {
        IsChargeBtn = userInput.Fire1Input;
        animator.SetBool(m_HashChargeBtn, IsChargeBtn);
        if (IsChargeBtn)
        {
            chargePower = Mathf.Clamp(chargePower + 6 * Time.deltaTime, 0, 1.1f);
            if (chargePower > 1f)
            {
                animator.SetTrigger(m_HashChargeFull);
            }
        }
        else
        {
            if (chargePower > 1f)
            {
                if (chargeFullCotoutine != null) StopCoroutine(chargeFullCotoutine);
                chargeFullCotoutine = StartCoroutine(StartChargeFull());
            }
            chargePower = 0;
        }
    }
    public void ApplyChargeMovement()
    {
        if (IsChargeBtn)
        {
            nextPosition.x = Mathf.Lerp(rb.position.x, chargePosX, 12 * Time.fixedDeltaTime);
        }
        else
        {
            nextPosition.x = Mathf.Lerp(rb.position.x, startPos.x, 12 * Time.fixedDeltaTime);
        }
    }

    IEnumerator StartChargeFull()
    {
        chargeFullBuff = true;

        yield return m_ChargeFullWait;
        chargeFullBuff = false;
        chargeFullCotoutine = null;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float chargeMultiplier = (chargeFullBuff) ? 40f : 1f;
        //Debug.Log(collision.collider.tag);
        if (collision.collider.CompareTag("ball"))
        {
            Vector2 direction = new Vector2(1f, 0f);
            if(playerStyle == PlaneStyle.Strike)
            {
                ApplyCounterForceMultiply();
            }
            collision.collider.attachedRigidbody.AddForce(direction * (baseCounterForce * counterForceMultiplier * chargeMultiplier ));
        }
    }

    public void EnterIdle()
    {
        animator.ResetTrigger(m_HashChargeFull);
    }

}

public enum PlaneStyle
{
    Strike,
    Balance,
    Accuracy,
    Casual

}

public enum MoveableDimension
{
    Point,
    Line,
    Face
}
