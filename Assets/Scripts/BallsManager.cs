using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class BallsManager : MonoBehaviourSingleton<BallsManager>
{
    public Ball prefab_BallPrefab;
    private ObjectPooler<Ball> m_BallPool;
    public ObjectPooler<Ball> ballPool { get => m_BallPool; }

    public PredictableArrow prefab_PredictableArrowPrefab;
    private ObjectPooler<PredictableArrow> m_PredictableArrowPool;

    const int poolSize = 30;

    public float coolDown = 0f;
    float startTime = 0;
    //gravity change
    Vector3 gravityDirection;
    float duration;
    bool isGravityEffecting = false;

    public struct BallWithArrow
    {
        public Ball ball;
        public PredictableArrow arrow;
    }
    List<BallWithArrow> ballWithArrows = new List<BallWithArrow>();

    private void Awake()
    {
        //Initialize Pool
        m_BallPool = new ObjectPooler<Ball>();
        m_BallPool.Initialize(poolSize, prefab_BallPrefab);
        m_PredictableArrowPool = new ObjectPooler<PredictableArrow>();
        m_PredictableArrowPool.Initialize(poolSize, prefab_PredictableArrowPrefab);
        for (int i = 0; i < m_BallPool.instances.Length; i++)
        {
            m_BallPool.instances[i].VFX_PoolInitialize();
        }
    }

    private void Start()
    {
        MyGameEventSystem.UseBallIncreasedItemEvent.performed += BallIncreased;

        CountingDownCanvas.Instance.OnCountedDown += BallIncreased;
        UserInput.Instance.inputActions.Player.Fire1.canceled += Fire1_canceled; //corruption
        MyGameEventSystem.UseGravityChangedItemEvent.performed += StartCustomGravity;
        MyGameEventSystem.UseGravityChangedItemEvent.canceled += StopCustomGravity;
        MyGameEventSystem.BallEvents.OwnerChanged.performed += CheckYourBall;

        //BallIncreased(10);
        
    }

    private void StopCustomGravity()
    {
        duration = 0f;
        gravityDirection = Vector3.zero;
        isGravityEffecting = false;
    }

    private void StartCustomGravity(Vector4 _direction, float _duration)
    {
        duration = _duration;
        gravityDirection = _direction;
        isGravityEffecting = true;
    }

    private void Fire1_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StopPredictAiming(PlayerInstanceManager.Instance.currentPlayerGameObject);
    }
    private void Update()
    {   
        for (int i = 0; i < m_PredictableArrowPool.notFreeIdx.Count; i++)
        {
            m_PredictableArrowPool.instances[m_PredictableArrowPool.notFreeIdx[i]].UpdateArrowOrientation();
            m_PredictableArrowPool.instances[m_PredictableArrowPool.notFreeIdx[i]].Follow();
        }
        for (int i = 0; i < m_BallPool.notFreeIdx.Count; i++)
        {
            m_BallPool.instances[m_BallPool.notFreeIdx[i]].Follow();
        }

    }

    private void FixedUpdate()
    {
        if(isGravityEffecting)
        {
            if (duration > 0)
            {
                for (int i = 0; i < m_BallPool.notFreeIdx.Count; i++)
                {
                    m_BallPool.instances[m_BallPool.notFreeIdx[i]].RigidBody2D.velocity += (Vector2)gravityDirection * Physics2D.gravity.magnitude * Time.fixedDeltaTime;
                }
                duration -= Time.fixedDeltaTime;
            }
            else
            {
                isGravityEffecting = false;
            }
        }
        
        for (int i = 0; i < m_BallPool.notFreeIdx.Count; i++)
        {
            m_BallPool.instances[m_BallPool.notFreeIdx[i]].Timeout();
            m_BallPool.instances[m_BallPool.notFreeIdx[i]].OverSpeed();
            m_BallPool.instances[m_BallPool.notFreeIdx[i]].RestrictMaxSpeed();
            m_BallPool.instances[m_BallPool.notFreeIdx[i]].RestrictMaxBound();
        }
        
    }

    public void BallIncreased(int count, GameObject applicant) // the start of life cycle of a ball
    {
        float startAngle = 0;
        float sign = -1;
        
        Ball newBall;
        PredictableArrow newArrow;
        BallWithArrow ballWithArrow;
        for (int i = 0; i < count; i++)
        {
            if(m_BallPool.FreeIdx.Count > 0)
                m_BallPool.instances[m_BallPool.FreeIdx.Peek()].owner = applicant;
            if (m_PredictableArrowPool.FreeIdx.Count > 0)
                m_PredictableArrowPool.instances[m_PredictableArrowPool.FreeIdx.Peek()].target = applicant.transform;

  
            newBall = m_BallPool.GetNew();
            //newBall.owner = applicant;
            
            newBall.OnDisabledTheBall -= CheckMineBall;
            newBall.OnDisabledTheBall += CheckMineBall;

            newArrow = m_PredictableArrowPool.GetNew();
            if(newBall == null)
            {
                break;
            }
            ballWithArrow = new BallWithArrow { ball = newBall, arrow = newArrow };
            ballWithArrows.Add(ballWithArrow);
            StartPredictAiming(ballWithArrow, startAngle);
            //Update StartAngle
            if (i % 2 == 0)
                startAngle += 15f;
            startAngle *= sign;
        }
        //lastloadedIndex += count;
    }

    private void CheckMineBall(GameObject applicant) 
    {
        for (int i = 0; i < m_BallPool.notFreeIdx.Count; i++)
        {
            if (m_BallPool.instances[m_BallPool.notFreeIdx[i]].owner == applicant)
            {
                return;
            }
        }
#if UNITY_EDITOR
        Debug.Log("There is no ball belong " + applicant + ", serve a new ball.");
#endif 
        CountingDownCanvas.Instance.CountingDown(applicant);
        
    }
    public void CheckYourBall(GameObject applicant)
    {
        AvatarCard card = AvatarCard.FindSpecifiedCard(applicant);
        GameObject go = null;
        if (card.avatarID == 1)
        {
            go = AvatarCard.FindSpecifiedCard(2).avatar;
        }
        else if (card.avatarID == 2)
        {
            go = AvatarCard.FindSpecifiedCard(1).avatar;
        }
        if (go == null) return;

        for (int i = 0; i < m_BallPool.notFreeIdx.Count; i++)
        {
            if (m_BallPool.instances[m_BallPool.notFreeIdx[i]].owner == go)
            {
                return;
            }
        }
#if UNITY_EDITOR
        Debug.Log("There is no ball belong " + go + ", serve a new ball.");
#endif 
        CountingDownCanvas.Instance.CountingDown(go);
    }




    IEnumerator DelayAndServe(GameObject applicant, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ballWithArrows.Clear();
        BallIncreased(1, applicant);
    }

    private void StartPredictAiming(BallWithArrow ballWithArrow, float _startAngle)
    {
        ballWithArrow.arrow.StartFollow(ballWithArrow.ball.owner.transform, _startAngle);
        ballWithArrow.ball.StartFollow(ballWithArrow.ball.owner.transform);
        /*
        ballWithArrow.ball.target = ballWithArrow.ball.owner.transform;
        ballWithArrow.ball.myCollider.enabled = false;
        ballWithArrow.ball.pauseFollow = false;
        */
    }
    public void StopPredictAiming(GameObject applicant)
    {
        List<BallWithArrow> marked = new List<BallWithArrow>();
        for (int i = 0; i < ballWithArrows.Count; i++)
        {
            if (applicant != ballWithArrows[i].ball.owner) continue;

            ballWithArrows[i].arrow.StopFollow(applicant);
            ballWithArrows[i].ball.StopFollow(applicant);
            ballWithArrows[i].ball.ServeByManager(ballWithArrows[i].arrow.transform.rotation, 15f);
            m_PredictableArrowPool.Free(ballWithArrows[i].arrow);
            marked.Add(ballWithArrows[i]);
        }
        //ballWithArrows.Clear();
        for (int i = 0; i < marked.Count; i++)
        {
            ballWithArrows.Remove(marked[i]);
        }
    }

}

public interface IOwnerShip
{
    public GameObject owner {set; get;}
}