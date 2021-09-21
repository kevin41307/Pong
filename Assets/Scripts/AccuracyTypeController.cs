using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyTypeController : PlayerControl
{
    public SniperX sniperX;
    public override void ApplyStyleParameter()
    {
        ResetTransformExceptPostion();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.mass = 10000f;
        rb.drag = 100f;
        rb.angularDrag = 100f;
        rb.constraints = RigidbodyConstraints2D.None;
        moveableDimension = MoveableDimension.Point;
        baseCounterForce = 400f;
        m_MoveSpeedDefault = 10000f;
        sniperX.enabled = true;
    }
    public void Sniper_Start()
    {
        sniperX.OnAimed += Sniper_Aiming;
        m_collider.enabled = false;
        SetNextPosition(userInput.PositionInput);
        DoMoveImmediately(nextPosition);
    }
    public void Sniper_Aiming()
    {
        //Debug.Log(sniper.Direction);
        rb.MoveRotation(Helpers.AngleFromDir(sniperX.Direction));
        SetNextPosition(sniperX.StartPosition - sniperX.Direction.normalized * Mathf.Clamp(sniperX.Direction.magnitude, 0.1f, 1f));
        DoMoveImmediately(nextPosition);
    }
    public void Sniper_End()
    {
        m_collider.enabled = true;
        sniperX.OnAimed -= Sniper_Aiming;
    }
    void SetNextPosition(Vector2 position)
    {
        nextPosition.Set(Mathf.Clamp(position.x, ClippedMoveableArea.Min.x, ClippedMoveableArea.Max.x), Mathf.Clamp(position.y, ClippedMoveableArea.Min.y, ClippedMoveableArea.Max.y));
    }
    void DoMoveImmediately(Vector3 position)
    {
        rb.MovePosition(position);
    }
}
