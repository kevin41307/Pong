using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(1)]
public class StrikeComputer : PlayerControl
{
    DummyInput dummyInput;
    private float x;
    private float counterForceMultiplier = 1f;
    float intelligence = 10;

    public override void ApplyStyleParameter()
    {
        ResetTransformExceptPostion();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.mass = 10000f;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        moveableDimension = MoveableDimension.Face;
        baseCounterForce = 100f;
        m_MoveSpeedDefault = 40f;
    }

    private void FixedUpdate()
    {
        PrepareMove();
        DoMove();
    }

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //userInput = UserInput.Instance;
        dummyInput = GetComponent<DummyInput>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider2D>();
        if (ClippedMoveableArea == null)
            ClippedMoveableArea = gameObject.AddComponent<EnclosureArea2D>();
    }

    protected override void Start()
    {
        startScale = transform.localScale;
        moveSpeed = m_MoveSpeedDefault;
        ClippedMoveableArea.UpdateBounds(moveableArea.Center, m_collider.bounds.extents, moveableArea.Extent);

        //inputActions = userInput.GetPlayerInputActions();
        //subscribe event
        GameManager.OnResetGame.AddListener(Reset);
        AvatarCard card = new AvatarCard(this.gameObject);
    }
    protected override void PrepareMove()
    {
        switch (moveableDimension)
        {
            case MoveableDimension.Point:
                break;
            case MoveableDimension.Line:
                nextPosition.Set(rb.position.x, Mathf.Clamp(dummyInput.PositionInput.y, ClippedMoveableArea.Min.y, ClippedMoveableArea.Max.y));
                break;
            case MoveableDimension.Face:
                nextPosition.Set(Mathf.Clamp(dummyInput.PositionInput.x, ClippedMoveableArea.Min.x, ClippedMoveableArea.Max.x), Mathf.Clamp(dummyInput.PositionInput.y, ClippedMoveableArea.Min.y, ClippedMoveableArea.Max.y));
                break;
            default:
                nextPosition = rb.position;
                break;
        }

        //FixUnityInputSystemProblem();
    
    }

    protected override void FixUnityInputSystemProblem()
    {
        if (dummyInput.PositionInput.magnitude == 0) 
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

    void ApplyCounterForceMultiply()
    {
        x = (Mathf.Abs(rb.position.x - moveableArea.Min.x) / (moveableArea.Extent.x * 2) + 1.1f);

        counterForceMultiplier = x * x;
        //Debug.Log(counterForceMultiply);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("ball"))
        {
            Vector2 direction = rb.velocity.normalized;
            ApplyCounterForceMultiply();
            collision.collider.attachedRigidbody.AddForce(direction * (baseCounterForce * counterForceMultiplier));
        }
    }
}
