using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GreenBrick : Brick
{
    public TextMeshProUGUI multiplierText;
    public override void ApplyColorTypeParameter()
    {
        brickType = BrickType.Breakable;
        m_BrickColorType = BrickColorType.Green;
        transform.localScale = new Vector3(0.5f, 1, 1);
        Durability = 2f;
        MigrateMultiplier = (int)Durability;
    }

    public override void Break(CompensationInfo info)
    {
        if (info.dontNeedCompensate == false)
            ItemOnWorldManager.Instance.ItemIncreased(info);
        pool.Free(this);
        LetSeatReleased();
        LetMeMigrate();
    }

    public override void ChangeMigrateMultiplier(int n)
    {
        base.ChangeMigrateMultiplier(n);
        multiplierText.text = "x" + MigrateMultiplier.ToString();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Durability -= 1f;
        if (Durability <= 0)
        {
            CompensationInfo newInfo = new CompensationInfo { breaker = collision.gameObject.GetComponent<MonoBehaviour>(), brick = this };
            Break(newInfo);
        }
        else if (Durability <= durabilityMax * 0.2f)
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
