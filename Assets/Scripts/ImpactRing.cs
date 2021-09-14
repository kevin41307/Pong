using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactRing : VolatilizeVisualEffect
{

    public override float expiredTime { 
        get { 
            if (ps == null) ps = GetComponentInChildren<ParticleSystem>();
            if (ps == null)
            {
                Debug.Log("Cannot find ParticleSystem in children!");
            }
            var psDuration = ps.main;
            m_ExpiredTime = psDuration.duration;
            return m_ExpiredTime; 
        } 
        set { m_ExpiredTime = value; } }

    ParticleSystem ps;
}