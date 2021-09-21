using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeTypeController : PlayerControl
{
    private float x;
    private float counterForceMultiplier = 1f;

    private void FixedUpdate()
    {
        PrepareMove();
        DoMove();
    }

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
        m_MoveSpeedDefault = 70f;
        //OnTypeChanged?.Invoke(playerStyle);
        //OnSizeChanged?.Invoke();
    }

    void SetNextPosition(Vector2 position)
    {
        nextPosition.Set(Mathf.Clamp(position.x, ClippedMoveableArea.Min.x, ClippedMoveableArea.Max.x), Mathf.Clamp(position.y, ClippedMoveableArea.Min.y, ClippedMoveableArea.Max.y));
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
            collision.collider.attachedRigidbody.AddForce(direction * (baseCounterForce * counterForceMultiplier ));
        }
    }

}

