using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public abstract class PlayerControl : Player
{
    //Components
    protected Rigidbody2D rb;
    protected UserInput userInput;
    protected Animator animator;
    protected SpriteRenderer sr;
    protected PlayerInputActions inputActions;
    public EnclosureArea2D moveableArea;
    public EnclosureArea2D ClippedMoveableArea { private set; get; }
    protected Collider2D m_collider;

    public delegate void TypeChanged(Player player, PlaneStyle nextStyle);
    public event TypeChanged OnTypeChanged;

    //Player Style
    protected PlaneStyle playerStyle;
    protected MoveableDimension moveableDimension;
    protected Vector2 startPos = new Vector2(-9, 0f);
    protected Vector2 nextPosition;
    protected Vector2 lastValidNextPosition = Vector2.zero;
    protected float moveSpeed;
    protected float m_MoveSpeedDefault = 50f; // OnlyChangeInPlaneStyle
    protected float baseCounterForce = 200f;
    protected Vector2 currentVelocity;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        userInput = UserInput.Instance;
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider2D>();
        
    }

    public virtual void OnEnable()
    {
        ApplyStyleParameter();
    }

    protected virtual void Reset()
    {
        transform.position = startPos;
        ResetTransformExceptPostion();
    }

    protected virtual void ResetTransformExceptPostion()
    {
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
    }

    protected virtual void Start()
    {
        moveSpeed = m_MoveSpeedDefault;
        //TODO: maybe seperate EnclosureArea2D class
        if(ClippedMoveableArea == null)
            ClippedMoveableArea = gameObject.AddComponent<EnclosureArea2D>();
        ClippedMoveableArea.UpdateBounds(moveableArea.Center, m_collider.bounds.extents, moveableArea.Extent);
        inputActions = userInput.GetPlayerInputActions();
        //subscribe event
        GameManager.OnResetGame.AddListener(Reset);

        //ApplyPlaneStyleParameter(playerStyle);

    }

    protected virtual void PrepareMove()
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

        FixUnityInputSystemProblem();
    }

    protected virtual void FixUnityInputSystemProblem()
    {
        if (userInput.PositionInput.magnitude == 0)
        {
            if (lastValidNextPosition.magnitude == 0)
                nextPosition = rb.position; // deal with first case
            else
                nextPosition = lastValidNextPosition;
            //Debug.Log(lastValidNextPosition);
        }
        else
            lastValidNextPosition = nextPosition;
    }

    protected virtual void DoMove()
    {
        //Debug.Log("1." + userInput.PositionInput + "1.nextPosition" + nextPosition);
        Vector2.SmoothDamp(rb.position, nextPosition, ref currentVelocity, Time.deltaTime, m_MoveSpeedDefault, Time.deltaTime);
        rb.velocity = currentVelocity;
    }
    void UpdateBounds() => ClippedMoveableArea.UpdateBounds(moveableArea.Center, m_collider.bounds.extents, moveableArea.Extent);


    public abstract void ApplyStyleParameter();
}


