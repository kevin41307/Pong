using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasualTypeController : PlayerControl
{
    public override void ApplyStyleParameter()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
        rb.mass = 100f;
        rb.drag = 0f;
        rb.angularDrag = 0.01f;
        rb.constraints = RigidbodyConstraints2D.None;
        moveableDimension = MoveableDimension.Face;
        baseCounterForce = 200f;
        m_MoveSpeedDefault = 50f;
    }

    private void FixedUpdate()
    {
        PrepareMove();
        DoMove();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("ball"))
        {
            Vector2 direction = new Vector2(1f, 0f);

            collision.collider.attachedRigidbody.AddForce(direction * (baseCounterForce));
        }
    }

}
