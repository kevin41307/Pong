using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld_IncreaseBall : ItemOnWorld, IPooled<ItemOnWorld_IncreaseBall>
{
    int count = 1;
    public int poolID { get; set; }
    public ObjectPooler<ItemOnWorld_IncreaseBall> pool { get; set; }

    const int k_ConstructionSize = 30;

    public override void ItemFree()
    {
        if(pool != null)
        {
            pool.Free(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
        owner = null;
    }

    public override void Use(GameObject bagOwner)
    {
        //myGameEventSystem.UseBallIncreasedItem(count, bagOwner);
        //myGameEventSystem.AcumulatedBalls(count, bagOwner);
        MyGameEventSystem.AcumulatedBallEvent.Perform(count, bagOwner);

    }
    protected override void ApplyItemTypeParameter()
    {
        itemType = ItemType.Green;
    }

    public override void PoolInitialize()
    {
        pool = new ObjectPooler<ItemOnWorld_IncreaseBall>();
        pool.Initialize(k_ConstructionSize, this);
    }
}
