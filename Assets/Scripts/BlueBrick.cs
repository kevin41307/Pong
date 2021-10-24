using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBrick : Brick
{
    private void OnDisable()
    {
        durabilityMax = 1f;
        seatStartID = -1;
        seatCount = -1;
    }



    public override void ApplyColorTypeParameter()
    {
        brickType = BrickType.Breakable;
        m_BrickColorType = BrickColorType.Blue;
        transform.localScale = new Vector3(0.5f, 1, 1);
        Durability = 1f;
        durabilityMax = Durability;
    }

    public override void Break(CompensationInfo info)
    {
        if (info.dontNeedCompensate == false)
            ItemOnWorldManager.Instance.ItemIncreased(info);
        LetSeatReleased();
        pool.Free(this);
        BrickBreaked(info);
        LetMeMigrate();

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Durability -= 1f;
        if (Durability <= 0)
        {
            CompensationInfo newInfo = new CompensationInfo { breaker = collision.gameObject.GetComponent<MonoBehaviour>(), brick = this };
            Break(newInfo);
        }
        else if( Durability <= durabilityMax * 0.2f )
        {
            crackLineDecalSystem.Apply(5);
        }
        else if (Durability <= durabilityMax * 0.4f)
        {
            crackLineDecalSystem.Apply(4);
        }
        else if (Durability <= durabilityMax * 0.6f)
        {
            crackLineDecalSystem.Apply(3);
        }
        else if (Durability <= durabilityMax * 0.8f)
        {
            crackLineDecalSystem.Apply(2);
        }
        else
        {
            crackLineDecalSystem.Apply(1);
        }
    }
}
