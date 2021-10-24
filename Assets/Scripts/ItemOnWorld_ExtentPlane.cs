using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld_ExtentPlane : ItemOnWorld, IPooled<ItemOnWorld_ExtentPlane>
{
    public int poolID { get; set; }
    public ObjectPooler<ItemOnWorld_ExtentPlane> pool { get; set; }
    const int k_ConstructionSize = 30;

    public override void ItemFree()
    {
        if (pool != null)
            pool.Free(this);
        else
        {
            gameObject.SetActive(false);
        }
        owner = null;
    }

    /*
public ItemOnWorld_BigBall prefab_GigBallPrefab;
private ObjectPooler<ItemOnWorld_BigBall> m_WhiteBrickPool;
private ItemOnWorld_BigBall[] whiteBrickInstances;
*/
    public override void Use(GameObject bagOwner)
    {
        //myGameEventSystem.UseExtentedPlaneItem(bagOwner);
        MyGameEventSystem.UseExtentedPlaneItemEvent.Perform(bagOwner);
    }

    protected override void ApplyItemTypeParameter()
    {
        itemType = ItemType.Blue;
    }

    public override void PoolInitialize()
    {
        pool = new ObjectPooler<ItemOnWorld_ExtentPlane>();
        pool.Initialize(k_ConstructionSize, this);
    }
}
