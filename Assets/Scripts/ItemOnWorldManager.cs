using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class ItemOnWorldManager : MonoBehaviourSingleton<ItemOnWorldManager>
{
    /*
    public ObjectPooler<ItemOnWorld_IncreaseBall> increaseBallPool { get; set; }
    private ItemOnWorld_IncreaseBall[] increaseBallInstances;
    public ObjectPooler<ItemOnWorld_ExtentPlane> extentPlanePool { get; set; }
    private ItemOnWorld_ExtentPlane[] extentPlaneInstances;
    public ObjectPooler<ItemOnWorld_BigBall> bigBallPool { get; set; }
    private ItemOnWorld_BigBall[] bigBallInstances;
    */

    public ItemOnWorld_IncreaseBall itemOnWorld_IncreaseBall;
    public ItemOnWorld_ExtentPlane itemOnWorld_ExtentPlane;
    public ItemOnWorld_BigBall itemOnWorld_BigBall;
    public ItemOnWorld_PushLevel itemOnWorld_PushLevel;
    public ItemOnWorld_Gravity itemOnWorld_Gravity;
    public ItemOnWorld_Portal itemOnWorld_Portal;


    private void Awake()
    {
        /*
        increaseBallPool = new ObjectPooler<ItemOnWorld_IncreaseBall>();
        increaseBallInstances = increaseBallPool.instances;

        extentPlanePool = new ObjectPooler<ItemOnWorld_ExtentPlane>();
        extentPlaneInstances = extentPlanePool.instances;

        bigBallPool = new ObjectPooler<ItemOnWorld_BigBall>();
        bigBallInstances = bigBallPool.instances;
        */

        itemOnWorld_IncreaseBall.PoolInitialize();
        itemOnWorld_ExtentPlane.PoolInitialize();
        itemOnWorld_BigBall.PoolInitialize();
        itemOnWorld_PushLevel.PoolInitialize();
        itemOnWorld_Gravity.PoolInitialize();
        itemOnWorld_Portal.PoolInitialize();

        //ItemOnWorld item1 = itemOnWorld_Portal.pool.GetNew();
        //item1.SetGravityDirection(new Vector3(-1, 0, 0));

        for (int i = 0; i < 2 ; i++)
        {
            ItemOnWorld item = itemOnWorld_IncreaseBall.pool.GetNew();
            item.SetGravityDirection(new Vector3(-1, 0, 0));
            //ItemOnWorld item5 = itemOnWorld_Gravity.pool.GetNew();
            //item5.SetGravityDirection(new Vector3(-1, 0, 0));
            //ItemOnWorld item2 = itemOnWorld_PushLevel.pool.GetNew();
            //item2.SetGravityDirection(new Vector3(-1, 0, 0));

        }

        /*
                ItemOnWorld item = itemOnWorld_ExtentPlane.pool.GetNew();
        item.SetGravityDirection(new Vector3(-1, 0, 0));

            ItemOnWorld item1 = itemOnWorld_IncreaseBall.pool.GetNew();
            item1.SetGravityDirection(new Vector3(-1, 0, 0));

        ItemOnWorld item2 = itemOnWorld_IncreaseBall.pool.GetNew();
        item2.SetGravityDirection(new Vector3(-1, 0, 0));

        ItemOnWorld item3 = itemOnWorld_IncreaseBall.pool.GetNew();
        item3.SetGravityDirection(new Vector3(-1, 0, 0));
        */
    }

    public void ItemIncreased(CompensationInfo info)
    {
        ItemOnWorld item = null;
        bool mutation = Random.Range(0f, 10f) > 9f ? true : false;
        switch (info.brick.m_BrickColorType)
        {
            case BrickColorType.White:
                /*
                if (mutation == false)
                {
                    item = itemOnWorld_PushLevel.pool.GetNew();
                }
                else
                {
                    bool secondMutation = Random.Range(0f, 2f) > 1f ? true : false;
                    if (secondMutation)
                    {
                        item = itemOnWorld_PushLevel.pool.GetNew();
                    }
                    else
                    {
                        item = itemOnWorld_PushLevel.pool.GetNew();
                    }
                }
                */
                break;
            case BrickColorType.Red:
                if (mutation == false) item = itemOnWorld_BigBall.pool.GetNew();
                else
                {
                    bool secondMutation = Random.Range(0f, 2f) > 1f ? true : false;
                    if (secondMutation)
                        item = itemOnWorld_Gravity.pool.GetNew();
                    else
                        item = itemOnWorld_PushLevel.pool.GetNew();
                }
                break;
            case BrickColorType.Blue:
                if (mutation == false) item = itemOnWorld_ExtentPlane.pool.GetNew();
                else
                {
                    bool secondMutation = Random.Range(0f, 2f) > 1f ? true : false;
                    if (secondMutation)
                        item = itemOnWorld_Gravity.pool.GetNew();
                    else
                        item = itemOnWorld_Portal.pool.GetNew(); 
                }
                break;
            case BrickColorType.Green:
                if (mutation == false) item = itemOnWorld_IncreaseBall.pool.GetNew();
                else
                {
                    bool secondMutation = Random.Range(0f, 2f) > 1f ? true : false;
                    if (secondMutation)
                        item = itemOnWorld_PushLevel.pool.GetNew();
                    else
                        item = itemOnWorld_Portal.pool.GetNew(); 
                }
                break;
            case BrickColorType.None:
                break;
            default:
                break;
        }

        if (item == null) return;
        

        item.transform.position = info.brick.transform.position;
        IOwnerShip ownerShip = info.breaker as IOwnerShip;
        AvatarCard card = AvatarCard.FindSpecifiedCard(ownerShip.owner);
        if (card != null)
        {
            if (card.avatarID == 1)
            {
                item.SetGravityDirection(Vector2.left);
            }
            else if (card.avatarID == 2)
            {
                item.SetGravityDirection(Vector2.right);
            }
        }
    }

}

public class CompensationInfo
{
    public Brick brick;
    public MonoBehaviour breaker;
    public bool dontNeedCompensate;
}