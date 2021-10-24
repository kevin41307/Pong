using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyInput : MonoBehaviour
{

    //Components
    ItemBag itemBag;

    //Input Parameters
    [HideInInspector]
    public bool playerControllerInputBlocked;
    protected bool m_ExternalInputBlocked;
    protected Vector2 m_PointerWorldPosition;
    BallsManager ballsManager;
    Rigidbody2D rb;
    //Parameters
    ObjectPooler<Ball> ballPool;
    float predictFrameTime = 12f;
    Vector3 closestTarget;
    Coroutine delayUseItemCoroutine;
    Coroutine startCheckBagCoroutine;
    bool updated = false;
    Vector2 ballPosition = Vector2.zero;
    Vector2 ballVelocity;
    System.Action<int, GameObject> CountedDownAction;
    Ball lockOnTarget;
    float scanTimer = 0.1f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        itemBag = GetComponent<ItemBag>();
        itemBag.isPlayerUsing = false;
        ballsManager = BallsManager.Instance;
        ballPool = ballsManager.ballPool;
        closestTarget.Set(-11f, 0, 0);
    }

    private void Start()
    {
        if (startCheckBagCoroutine != null) StopCoroutine(startCheckBagCoroutine);
        startCheckBagCoroutine = StartCoroutine(StartCheckBag());
        CountedDownAction += (_a, _b) => StartCoroutine(StartSimulateServeBallClick());
        CountingDownCanvas.Instance.OnCountedDown += CountedDownAction;
    }


    public Vector3 PositionInput
    {
        get
        {
            if (playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_PointerWorldPosition;
        }
    }

    public void Scan()
    {
        if (ballPool.notFreeIdx.Count <= 0) return;
        closestTarget.Set(-11f, 0, 0);
        updated = false;
        int targetIndex = 0;
        Ball currentBall = null;
        for (int i = 0; i < ballPool.notFreeIdx.Count; i++)
        {
            currentBall = null;
            if(lockOnTarget != null )
            {
                currentBall = lockOnTarget;
            }
            else
            {
                currentBall = ballPool.instances[ballPool.notFreeIdx[i]];
            }

            ballVelocity = currentBall.RigidBody2D.velocity;
            ballPosition = currentBall.RigidBody2D.position;
            if (ballPosition.x > closestTarget.x
                && Helpers.IsRightDirection(ballVelocity)
                && !Helpers.IsCloseDirection(rb.position - ballPosition, ballVelocity))
            {
                closestTarget = ballPosition + ballVelocity * predictFrameTime * Time.fixedDeltaTime;
                targetIndex = i;
                updated = true;
            }
            if (lockOnTarget != null) break;
        }

        if (updated)
        {
            //Debug.Log(Vector2.Distance(m_PointerWorldPosition, closestTarget));
            if (Vector2.Distance(m_PointerWorldPosition, closestTarget) > 1.5f)
                m_PointerWorldPosition = closestTarget;
            if(lockOnTarget != null )
            {
                lockOnTarget = ballPool.instances[targetIndex];
                lockOnTarget.OnDisabledTheBall += ClearTarget;
            }
        }
    }

    private void ClearTarget(GameObject obj)
    {
        lockOnTarget = null;
    }

    private void FixedUpdate()
    {
        scanTimer -= Time.fixedDeltaTime;
        if( scanTimer < 0)
        {
            Scan();
            scanTimer = 0.1f;
        }
    }

    IEnumerator StartCheckBag()
    {
        while (true)
        {
            CheckBag();
            yield return new WaitForSeconds(1.1f);
        }
    }

    IEnumerator StartSimulateServeBallClick()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 1f));

        BallsManager.Instance.StopPredictAiming(this.gameObject);
    }
    private void CheckBag()
    {
        if (itemBag.handedItemList.Count <= 0) return;
        if(itemBag.handedItemList.Exists(x => x.itemType == ItemType.Green))
        {
            StartCoroutine(StartSimulateServeBallClick());
        }
        itemBag.UseItems();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("ball"))
        {
            if (collision.transform == lockOnTarget)
            {
                lockOnTarget.OnDisabledTheBall -= ClearTarget;
                lockOnTarget = null;
            }
        }
    }


}
