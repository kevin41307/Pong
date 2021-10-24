using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld_BigBall : ItemOnWorld, IPooled<ItemOnWorld_BigBall>
{
    public float amplifier = 1;
    public int poolID { get; set; }
    public ObjectPooler<ItemOnWorld_BigBall> pool { get; set; }
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

    public override void Use(GameObject bagOwner)
    {
        //myGameEventSystem.UseBigBallItem(bagOwner, multiplier);
        MyGameEventSystem.UseBigBallItemEvent.Perform(bagOwner, amplifier);
    }

    protected override void ApplyItemTypeParameter()
    {
        itemType = ItemType.Red;
    }

    public override void PoolInitialize()
    {
        pool = new ObjectPooler<ItemOnWorld_BigBall>();
        pool.Initialize(k_ConstructionSize, this);
    }
}
