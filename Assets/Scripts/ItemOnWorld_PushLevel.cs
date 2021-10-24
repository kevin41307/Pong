using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld_PushLevel : ItemOnWorld, IPooled<ItemOnWorld_PushLevel>
{
    public int poolID { get; set; }
    public ObjectPooler<ItemOnWorld_PushLevel> pool { get; set; }
    const int k_ConstructionSize = 20;

    float distance = 1f;


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
        pool = new ObjectPooler<ItemOnWorld_PushLevel>();
        pool.Initialize(k_ConstructionSize, this);
    }

    public override void Use(GameObject pusher)
    {
        AvatarCard card = AvatarCard.FindSpecifiedCard(pusher);
        if (card.avatarID == 1)
        {
            distance = Mathf.Abs(distance);
        }
        else if (card.avatarID == 2)
        {
            distance = -Mathf.Abs(distance);
        }

        //myGameEventSystem.UsePushLevelItem(distance);
        MyGameEventSystem.UsePushLevelItemEvent.Perform(distance);
    }

    protected override void ApplyItemTypeParameter()
    {
        itemType = ItemType.Yellow;
    }

}
