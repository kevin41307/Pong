using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBrick : Brick
{
    /*
    protected override void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        MigrateMultiplier = 1;
        material = sr.material;
        crackLineDecalSystem = new CrackLineDecalSystem(material);
        sr.material = crackLineDecalSystem.ApplyWhite(0);
    }

    protected override void OnEnable()
    {
        ApplyColorTypeParameter();
        sr.material = crackLineDecalSystem.ApplyWhite(0);
    }
    */
    public override void ApplyColorTypeParameter()
    {
        Durability = 3f;
        brickType = BrickType.Breakable;
        m_BrickColorType = BrickColorType.White;
    }

    public override void Break(CompensationInfo info) // First call of compensated invocation list
    {
        if(info.dontNeedCompensate == false )
            ItemOnWorldManager.Instance.ItemIncreased(info);
        pool.Free(this);
        BrickBreaked(info);
        LetSeatReleased();
        LetMeMigrate();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Durability -= 1f;
        if(Durability <= 0f)
        {
            CompensationInfo newInfo = new CompensationInfo { breaker = collision.gameObject.GetComponent<MonoBehaviour>(), brick = this };
            Break(newInfo);

        }
        else if( Durability <= 1)
        {
            //sr.sprite = broken2;
            crackLineDecalSystem.Apply(3);

        }
        else
        {
            //sr.sprite = broken1;
            crackLineDecalSystem.Apply(1);
        }
        
    }



}
