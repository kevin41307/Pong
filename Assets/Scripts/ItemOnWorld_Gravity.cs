using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld_Gravity : ItemOnWorld, IPooled<ItemOnWorld_Gravity>
{
    public int poolID { get; set; }
    public ObjectPooler<ItemOnWorld_Gravity> pool { get; set; }
    const int k_ConstructionSize = 20;

    public Vector4 effectDirection;
    public float duration = 5f;
    private bool isCustomized;
    public override void ItemFree()
    {
        if (pool != null)
            pool.Free(this);
        else
        {
            gameObject.SetActive(false);
        }
        isCustomized = false;
        owner = null;
    }

    public override void PoolInitialize()
    {
        pool = new ObjectPooler<ItemOnWorld_Gravity>();
        pool.Initialize(k_ConstructionSize, this);
    }

    public void SetEffectDirection()
    {
        AvatarCard card = AvatarCard.FindSpecifiedCard(owner);
        //Debug.Log(card.avatarID);
        if (card.avatarID == 1)
        {
            effectDirection.Set(-1, 0, 0, 0);
        }
        else if (card.avatarID == 2)
        {
            effectDirection.Set(1, 0, 0, 0);
        }
    }
    public void SetEffectDirection(Vector4 newED)
    {
        effectDirection = newED;
        isCustomized = true;
    }
    public override void Use(GameObject bagOwner)
    {
        if(isCustomized == false)
            SetEffectDirection();
        //myGameEventSystem.UseGravityChangedItem(effectDirection, duration);
        MyGameEventSystem.UseGravityChangedItemEvent.Perform(effectDirection, duration);
    }

    protected override void ApplyItemTypeParameter()
    {
        itemType = ItemType.Purple;
    }
}
