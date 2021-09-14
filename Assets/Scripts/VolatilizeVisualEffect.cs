using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VolatilizeVisualEffect : MonoBehaviour, IPooled<VolatilizeVisualEffect>
{
    public int poolID { get; set; }
    public ObjectPooler<VolatilizeVisualEffect> pool { get; set; }

    [HideInInspector]
    public float m_ExpiredTime = -1f;

    public abstract float expiredTime { get; set; }
}
