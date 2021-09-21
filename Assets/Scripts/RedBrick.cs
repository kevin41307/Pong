using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBrick : Brick
{
    public bool startBoom = false;
    public override void ApplyColorTypeParameter()
    {
        brickType = BrickType.Breakable;
        durability = 1f;
        BrickColorType = BrickColorType.Red;
    }

    public override void Break()
    {
        //LetMeBoom();
        LetMeMigrate();
        pool.Free(this);
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && startBoom)
        {
            Break();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Break(); 
    } 
}
