using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintBrick : Brick
{

    protected override void OnEnable()
    {
        ApplyColorTypeParameter();
    }

    public override void ApplyColorTypeParameter()
    {
        brickType = BrickType.Penetrateable;
        Durability = 0f;
        m_BrickColorType = BrickColorType.White;
    }

    public override void ChangeColor(Color _color)
    {
        base.ChangeColor(_color);
        if (_color == Color.red)
        {
            m_BrickColorType = BrickColorType.Red;
        }
        else if (_color == Color.white)
        {
            m_BrickColorType = BrickColorType.White;
        }
        else if (_color == Color.blue)
        {
            m_BrickColorType = BrickColorType.Blue;
        }
        else if(_color == Color.green)
        {
            m_BrickColorType = BrickColorType.Green;
        }
        else
        {
            m_BrickColorType = BrickColorType.White;
            Debug.Log("Unexpected Color! return white.");
        }
    }



    public override void Break(CompensationInfo info)
    {
        //None
    }

}
