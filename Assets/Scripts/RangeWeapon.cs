using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : MonoBehaviour
{
    public Vector3 muzzleOffset;
    public Projectile projectile;

    protected Projectile m_LoadedProjectile = null;
    public Projectile loadedProjectile
    {
        get { return m_LoadedProjectile; }
    }

    protected ObjectPooler<Projectile> m_ProjectilePool;

    private void Start()
    {
        m_ProjectilePool = new ObjectPooler<Projectile>();
        m_ProjectilePool.Initialize(5, projectile);
    }


    private void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.A))
        {
            Attack(new Vector3(0, 0, 0));
            //Debug.Log("aa");
        }
#endif
    }

    void Attack(Vector3 target)
    {
        AttackProjectile(target);
    }

    void LoadProjectile()
    {
        if (m_LoadedProjectile != null)
            return;

        m_LoadedProjectile = m_ProjectilePool.GetNew();
        if(m_LoadedProjectile == null)
        {
            // Says pool is empty
            m_ProjectilePool.Initialize(5, projectile);
            m_LoadedProjectile = m_ProjectilePool.GetNew();
        }
        m_LoadedProjectile.transform.SetParent(transform, false);
        m_LoadedProjectile.transform.localPosition = muzzleOffset;
        //Debug.Log(m_LoadedProjectile.transform.localPosition);
        m_LoadedProjectile.transform.localRotation = Quaternion.identity;


    }

    void AttackProjectile(Vector3 target)
    {
        if (m_LoadedProjectile == null) LoadProjectile();

        m_LoadedProjectile.transform.SetParent(null, false);
        m_LoadedProjectile.transform.position = transform.position + muzzleOffset;

        m_LoadedProjectile.Shot(target, this);
        m_LoadedProjectile = null; //once shot, we don't own the projectile anymore, it does it's own life.
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 worldOffset = transform.TransformPoint(muzzleOffset);
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawLine(worldOffset + Vector3.up * 0.4f, worldOffset + Vector3.down * 0.4f);
        UnityEditor.Handles.DrawLine(worldOffset + Vector3.forward * 0.4f, worldOffset + Vector3.back * 0.4f);
    }
#endif

}
