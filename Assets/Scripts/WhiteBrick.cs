using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBrick : Brick
{
    public Sprite broken1;
    public Sprite broken2;
    public override void ApplyColorTypeParameter()
    {
        durability = 3f;
        brickType = BrickType.Breakable;
        BrickColorType = BrickColorType.White;
    }

    public override void Break()
    {
        LetMeMigrate();
        pool.Free(this);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        durability -= 1f;
        if(durability <= 0f)
        {
            Break();
        }
        else if( durability <= 1)
        {      
            sr.sprite = broken2;
        }
        else
        {
            sr.sprite = broken1;
        }
        
    }



}
