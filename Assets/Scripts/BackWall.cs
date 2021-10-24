using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackWall : MonoBehaviour
{
    public static readonly float YOffset = 19.91f;
    public static readonly float ZOffset = -47f;

    public VolatilizeVisualEffect impactRingPS;
    //protected VolatilizeVisualEffect m_LoadedEffect = null;
    protected ObjectPooler<VolatilizeVisualEffect> m_EffectPool;

    private void Start()
    {
        m_EffectPool = new ObjectPooler<VolatilizeVisualEffect>();
        m_EffectPool.Initialize(40, impactRingPS);
        
    }

    private void OnEnable()
    {
        Wall.OnBallHitted.AddListener(Play);
    }

    private void OnDisable()
    {
        Wall.OnBallHitted.RemoveListener(Play);
    }

    public void Play(Vector3 pos)
    {
        pos.x = pos.x * 40 / 18;
        Vector3 temp = pos;
        pos = transform.InverseTransformPoint(pos);
        pos.y = (temp.y > 0) ? YOffset : 0.01f;
        pos.z = ZOffset;
        if (MyGameEventSystem.UsePortalItemEvent.isPortalEnabled == false)
            m_EffectPool.GetNew(impactRingPS.ExpiredTime)?.Play(pos, this.transform, false);
    }
}
