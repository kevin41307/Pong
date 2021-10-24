using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VolatilizeVisualEffect : MonoBehaviour, IPooled<VolatilizeVisualEffect>
{
    public int poolID { get; set; }
    public ObjectPooler<VolatilizeVisualEffect> pool { get; set; }

    [HideInInspector]
    public float m_ExpiredTime = -1f;
    public float ExpiredTime
    {
        get
        {
            if (ps == null) ps = GetComponentInChildren<ParticleSystem>();
            if (ps == null)
            {
                Debug.Log("Cannot find ParticleSystem in children!");
            }
            var psDuration = ps.main;
            m_ExpiredTime = psDuration.duration;
            return m_ExpiredTime;
        }
        set { m_ExpiredTime = value; }
    }
    protected ParticleSystem ps;
    public void Play(Vector3 pos, Vector3 normal)
    {
        transform.position = pos;
        transform.up = normal;
    }
    public void Play(Vector3 pos)
    {
        transform.position = pos;
    }
    public void Play(Vector3 localPos, Transform parent, bool worldPositionStays)
    {
        transform.localPosition = localPos;
        transform.SetParent(parent, worldPositionStays);
    }

}
