using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Gamekit3D;

public class BalanceTypeController : PlayerControl
{
    private int getBallsCount;
    private Collider2D[] getBallsResults = new Collider2D[10];
    private Vector2 gravityDirection;
    private const float k_AbsorbRadius = 9f;
    private float chargePower;
    private bool chargeFullBuff;
    private float chargePosX;
    private Coroutine chargeFullCotoutine;
    private WaitForSeconds m_ChargeFullWait;
    private const float k_ChargeFullDuration = 0.25f;
    public bool IsChargeBtn { get; private set; }

    //State
    readonly int m_HashStartGlow = Animator.StringToHash("StartGlow");
    readonly int m_HashGlowing = Animator.StringToHash("Glowing");

    //Parameter
    readonly int m_HashChargeFull = Animator.StringToHash("ChargeFull");
    readonly int m_HashChargeBtn = Animator.StringToHash("ChargeBtn");

    protected override void Reset()
    {
        base.Reset();
    }
    protected override void Start()
    {
        base.Start();
        SceneLinkedSMB<BalanceTypeController>.Initialise(animator, this);
        SubscribeInputEvents();
        chargePosX = startPos.x - 1f;
    }


    public override void ApplyStyleParameter()
    {
        transform.position = startPos;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
        rb.mass = 10000f;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        moveableDimension = MoveableDimension.Line;
        baseCounterForce = 200f;
        m_MoveSpeedDefault = 50f;
    }

    void SubscribeInputEvents()
    {
        inputActions.Player.Fire1.started -= OnChargeStateStarted; // prevent multiple subscibe
        inputActions.Player.Fire1.started += OnChargeStateStarted;
        inputActions.Player.Fire1.canceled -= OnChargeStateExited;
        inputActions.Player.Fire1.canceled += OnChargeStateExited;
    }

    private void OnDisable()
    {
        inputActions.Player.Fire1.started -= OnChargeStateStarted; // prevent multiple subscibe
        inputActions.Player.Fire1.canceled -= OnChargeStateExited;
    }

    private void OnChargeStateStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        GetBalls();
    }
    private void OnChargeStateExited(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ReleaseBalls();
    }

    void Update()
    {
        DoCharge();
    }

    private void FixedUpdate()
    {
        DoAbsortForce();
        PrepareMove();
        DoMove();
    }
    protected override void PrepareMove()
    {
        base.PrepareMove();
        ApplyChargeMovement();
    }

    void GetBalls()
    {
        if (rb == null) return;
        getBallsCount = Physics2D.OverlapCircleNonAlloc(rb.position, k_AbsorbRadius, getBallsResults, 1 << LayerMask.NameToLayer("Collider"));
        for (int i = 0; i < getBallsCount; i++)
        {
            if (getBallsResults[i].CompareTag("ball") == false) continue;
            GravityEffectManager.Instance.AssignRing(getBallsResults[i].transform);
        }
    }

    void DoAbsortForce()
    {
        if (IsChargeBtn)
        {
            for (int i = 0; i < getBallsCount; i++)
            {
                if (getBallsResults[i].CompareTag("ball") == false) continue;
                gravityDirection = rb.position - (Vector2)getBallsResults[i].transform.position;
                getBallsResults[i].attachedRigidbody.velocity += gravityDirection.normalized * 9.81f * Mathf.Clamp(gravityDirection.magnitude * gravityDirection.magnitude * .15f, 0, 10) * Time.deltaTime;

            }
        }
    }

    void ReleaseBalls()
    {
        for (int i = 0; i < getBallsCount; i++)
        {
            if (getBallsResults[i].CompareTag("ball") == false) continue;
            //gravityRings[i].pool.Free(gravityRings[i]);

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
    private void ApplyChargeMovement()
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
            collision.collider.attachedRigidbody.AddForce(direction * (baseCounterForce * chargeMultiplier));
        }
    }

    public void EnterIdle()
    {
        animator.ResetTrigger(m_HashChargeFull);
    }

}
