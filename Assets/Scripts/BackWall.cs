using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackWall : MonoBehaviour
{
    public static readonly float YOffset = 19.91f;
    public static readonly float ZOffset = -94f;

    public VolatilizeVisualEffect effect;
    protected VolatilizeVisualEffect m_LoadedEffect = null;
    public VolatilizeVisualEffect loadedEffect
    {
        get { return m_LoadedEffect; }
    }

    protected ObjectPooler<VolatilizeVisualEffect> m_EffectPool;


    private void Start()
    {
        m_EffectPool = new ObjectPooler<VolatilizeVisualEffect>();
        m_EffectPool.Initialize(10, effect);
        
    }

    private void OnEnable()
    {
        Wall.onBallCollisionEnter.AddListener(Play);
    }

    public void Play(Vector3 pos)
    {
        if (m_LoadedEffect == null) LoadEffect();

        pos.x = pos.x * 40 / 18;
        Vector3 temp = pos;
        pos = transform.InverseTransformPoint(pos);
        pos.y = (temp.y > 0) ? YOffset : 0.01f;
        pos.z = ZOffset;

        m_LoadedEffect.transform.localPosition = pos;
        m_LoadedEffect.transform.SetParent(transform, false);
        
        m_LoadedEffect = null; //once shot, we don't own the projectile anymore, it does it's own life.
    }

    void LoadEffect()
    {
        if (m_LoadedEffect != null) return;
        m_LoadedEffect = m_EffectPool.GetNew(effect.expiredTime);

    }

    private void OnDisable()
    {
        Wall.onBallCollisionEnter.RemoveListener(Play);
    }

}
