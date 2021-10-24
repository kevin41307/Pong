using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBrick : Brick
{
    /*
   public bool startBoom = false;

   protected Collider2D ccollider2D;

   protected override void Awake()
   {
       base.Awake();
       ccollider2D = GetComponent<Collider2D>();
   }


   private void OnDisable()
   {
       ccollider2D.enabled = false;
   }
   */
    public override void ApplyColorTypeParameter()
    {
        brickType = BrickType.Breakable;
        Durability = 1f;
        m_BrickColorType = BrickColorType.Red;
    }

    public override void Break(CompensationInfo info)
    {
        if (info.dontNeedCompensate == false)
            ItemOnWorldManager.Instance.ItemIncreased(info);
        LetMeBoom(info.breaker);
        pool.Free(this);
        LetSeatReleased();
        LetMeMigrate();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CompensationInfo newInfo = new CompensationInfo { breaker = collision.gameObject.GetComponent<MonoBehaviour>(), brick = this };
        Break(newInfo);
    } 
}
