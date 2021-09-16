using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-1)]
public class GravityEffectManager : MonoBehaviourSingleton<GravityEffectManager>
{
    public GravityRing gravityRing;

    protected GravityRing m_LoadedGravityRing = null;
    public GravityRing loadedGravityRing
    {
        get { return m_LoadedGravityRing; }
    }

    protected ObjectPooler<GravityRing> m_GravityRingPool;

    private void Start()
    {
        m_GravityRingPool = new ObjectPooler<GravityRing>();
        m_GravityRingPool.Initialize(5, gravityRing); //TODO: Ball Manager set;
    }

    public void AssignRing(Transform source)
    {
        if (m_LoadedGravityRing == null) LoadGravityRing();

        m_LoadedGravityRing.transform.SetParent(source, true); // set true to pevent transform scale lose
        m_LoadedGravityRing.transform.position = source.transform.position;

        m_LoadedGravityRing = null;
    }

    public void WithdrawRing(GravityRing ring)
    {
        ring.pool.Free(ring);
    }

    public void  LoadGravityRing()
    {
        if (m_LoadedGravityRing != null) return;

        m_LoadedGravityRing = m_GravityRingPool.GetNew();
        if(m_LoadedGravityRing == null )
        {
            m_GravityRingPool.Initialize(5, gravityRing); //TODO: Ball Manager set;
            m_LoadedGravityRing = m_GravityRingPool.GetNew();
        }
    }
}
