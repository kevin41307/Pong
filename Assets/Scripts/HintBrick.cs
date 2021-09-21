using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintBrick : Brick
{
    public override void ApplyColorTypeParameter()
    {
        brickType = BrickType.Penetrateable;
        durability = 0f;
        BrickColorType = BrickColorType.White;
    }

    public override void ChangeColor(Color _color)
    {
        base.ChangeColor(_color);
        if (_color == Color.red)
        {
            BrickColorType = BrickColorType.Red;
        }
        else if (_color == Color.white)
        {
            BrickColorType = BrickColorType.White;
        }
        else
        {
            BrickColorType = BrickColorType.White;
        }
    }



    public override void Break()
    {
        //None
    }

}
