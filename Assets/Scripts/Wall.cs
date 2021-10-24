using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Wall : MonoBehaviour
{
    public static UnityEvent<Vector3> OnBallHitted = new UnityEvent<Vector3>();

    public VolatilizeVisualEffect portalPS;
    protected ObjectPooler<VolatilizeVisualEffect> m_EffectPool;

    private void Start()
    {
        m_EffectPool = new ObjectPooler<VolatilizeVisualEffect>();
        m_EffectPool.Initialize(20, portalPS);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("ball"))
        {
            //Debug.Log("ballIn " + collision.GetContact(0).point);
            if(OnBallHitted != null)
            {
                OnBallHitted.Invoke(collision.GetContact(0).point);
            }
            if(MyGameEventSystem.UsePortalItemEvent.isPortalEnabled)
            {
                //Play(collision.GetContact(0).point);
                m_EffectPool.GetNew(portalPS.ExpiredTime)?.Play(collision.GetContact(0).point);

            }
        }
    }
}