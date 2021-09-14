using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadierGrenade : Projectile
{
    public enum ShotType
    {
        HIGHEST_SHOT,
        LOWEST_SPEED,
        MOST_DIRECT
    }
    public ShotType shotType;
    public float projectileSpeed;
    public int damageAmount = 1;
    public LayerMask damageMask;
    public float explosionRadius;
    public float explosionTimer;
    public ParticleSystem explosionVFX;
    [Tooltip("Will the explosion VFX play where the grenade explode or on the closest ground")]
    public bool vfxOnGround = false;

    //public RandomAudioPlayer explosionPlayer;
    //public RandomAudioPlayer bouncePlayer;

    protected float m_SinceFired;

    protected RangeWeapon m_Shooter;
    protected Rigidbody m_RigidBody;
    protected ParticleSystem m_VFXInstance;
    protected int m_EnvironmentLayer = -1;

    protected static Collider[] m_ExplosionHitCache = new Collider[32];
    public bool receiving2DCollisionEvent;

    private void Awake()
    {
        m_EnvironmentLayer = 1 << LayerMask.NameToLayer("Wall");

        m_RigidBody = GetComponent<Rigidbody>();
        m_RigidBody.detectCollisions = false;

        m_VFXInstance = Instantiate(explosionVFX);
        m_VFXInstance.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        m_RigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        m_RigidBody.isKinematic = true;
        m_SinceFired = 0.0f;
    }
    public override void Shot(Vector3 target, RangeWeapon shooter)
    {
        m_RigidBody.isKinematic = false;

        m_Shooter = shooter;


        m_RigidBody.velocity = GetVelocity(target);
        if(receiving2DCollisionEvent)
            m_RigidBody.AddRelativeTorque(Vector3.forward * -300.0f);
        else
            m_RigidBody.AddRelativeTorque(Vector3.right * -5500.0f);

        m_RigidBody.detectCollisions = false;

        
        if(receiving2DCollisionEvent) transform.right = target - transform.position;
        else
            transform.forward = target - transform.position;
    }
    private void FixedUpdate()
    {
        m_SinceFired += Time.deltaTime;

        if (m_SinceFired > 0.2f)
        {
            //we only enable collision after half a second to get it time to clear the grenadier body 
            m_RigidBody.detectCollisions = true;
        }

        if (explosionTimer > 0 && m_SinceFired > explosionTimer)
        {
            Explosion();
        }
    }
    public virtual void Explosion()
    {
        /*
        if (explosionPlayer)
        {
            explosionPlayer.transform.SetParent(null);
            explosionPlayer.PlayRandomClip();
        }
        */
        int count = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, m_ExplosionHitCache,
            damageMask.value);

        //apply damege

        pool.Free(this);

        Vector3 playPosition = transform.position;
        Vector3 playNormal = Vector3.up;
        if (vfxOnGround)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f, m_EnvironmentLayer))
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

    protected virtual void OnCollisionEnter(Collision other)
    {
        /*
        if (bouncePlayer != null)
            bouncePlayer.PlayRandomClip();
        */ 
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private Vector3 GetVelocity(Vector3 target)
    {
        Vector3 velocity = Vector3.zero;
        Vector3 toTarget = target - transform.position;

        // Set up the terms we need to solve the quadratic equations.
        float gSquared = Physics.gravity.sqrMagnitude;
        float b = projectileSpeed * projectileSpeed + Vector3.Dot(toTarget, Physics.gravity);
        float discriminant = b * b - gSquared * toTarget.sqrMagnitude;

        // Check whether the target is reachable at max speed or less.
        if (discriminant < 0)
        {
            // Debug.Log("Can't reach");

            velocity = toTarget;
            velocity.y = 0;
            velocity.Normalize();
            velocity.y = 0.7f;

            Debug.DrawRay(transform.position, velocity * 3.0f, Color.blue);

            velocity *= projectileSpeed;
            return velocity;
        }

        float discRoot = Mathf.Sqrt(discriminant);

        // Highest shot with the given max speed:
        float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

        // Most direct shot with the given max speed:
        float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

        // Lowest-speed arc available:
        float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

        float T = 0;
        // choose T_max, T_min, or some T in-between like T_lowEnergy
        switch (shotType)
        {
            case ShotType.HIGHEST_SHOT:
                T = T_max;
                break;
            case ShotType.LOWEST_SPEED:
                T = T_lowEnergy;
                break;
            case ShotType.MOST_DIRECT:
                T = T_min;
                break;
            default:
                break;
        }


        // Convert from time-to-hit to a launch velocity:
        velocity = toTarget / T - Physics.gravity * T / 2f;

        return velocity;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
#endif
}
