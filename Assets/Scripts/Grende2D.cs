using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grende2D : GrenadierGrenade
{

    protected static Collider2D[] m_ExplosionHit2DCache = new Collider2D[32];
    private void Start()
    {
        receiving2DCollisionEvent = true;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Collider")) return;

        Explosion();
    }

    public override void Explosion()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, m_ExplosionHit2DCache,
           damageMask.value);

        //apply damege

        for (int i = 0; i < count; i++)
        {
            if( m_ExplosionHit2DCache[i].CompareTag("ball"))
            {
                var rb = m_ExplosionHit2DCache[i].GetComponent<Rigidbody2D>();
                if(rb != null)
                    rb.AddForce(m_RigidBody.velocity.normalized * 100f);
            }
        }

        pool.Free(this);

        Vector3 playPosition = transform.position;
        Vector3 playNormal = Vector3.up;
        if (vfxOnGround)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 10.0f, m_EnvironmentLayer);

            if (hit.collider != null)
            {
                playPosition = hit.point + hit.normal * 0.1f;
                playNormal = hit.normal;
            }
        }

        m_VFXInstance.gameObject.transform.position = playPosition;
        m_VFXInstance.gameObject.transform.up = playNormal;
        m_VFXInstance.time = 0.0f;
        m_VFXInstance.gameObject.SetActive(true);
        m_VFXInstance.Play(true);
    }
}
