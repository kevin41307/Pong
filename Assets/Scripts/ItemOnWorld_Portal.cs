using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld_Portal : ItemOnWorld, IPooled<ItemOnWorld_Portal>
{
    public int poolID { get; set ; }
    public ObjectPooler<ItemOnWorld_Portal> pool { get; set; }
    const int k_ConstructionSize = 20;
    float duration = 7f;
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

    public override void PoolInitialize()
    {
        pool = new ObjectPooler<ItemOnWorld_Portal>();
        pool.Initialize(k_ConstructionSize, this);
    }

    public override void Use(GameObject bagOwner)
    {
        //myGameEventSystem.UsePortalItem(bagOwner, duration);
        MyGameEventSystem.UsePortalItemEvent.Perform(bagOwner, duration);
    }

    protected override void ApplyItemTypeParameter()
    {
        itemType = ItemType.Cyan;
    }
}
