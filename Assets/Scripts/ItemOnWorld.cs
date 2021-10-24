using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemOnWorld : Item, IOwnerShip
{
    //Components
    protected BoxCollider2D m_Collider;
    protected MyGameEventSystem myGameEventSystem;
    //Parameters
    [SerializeField]
    protected Vector3 gravityDirection;
    protected float moveSpeed = 4f;
    public GameObject owner { get; set; }

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        m_Collider = GetComponent<BoxCollider2D>();
        m_Collider.isTrigger = true;
        myGameEventSystem = MyGameEventSystem.Instance;
        ApplyItemTypeParameter();
    }
    private void OnDisable()
    {
        gravityDirection = Vector3.zero;
    }

    protected virtual void Update()
    {
        transform.position += gravityDirection * moveSpeed * Time.deltaTime;
    }
    public void SetGravityDirection(Vector3 direction)
    {
        gravityDirection = direction;
    }
    public abstract void PoolInitialize();
    public abstract void Use(GameObject bagOwner);
    public abstract void ItemFree();
    public virtual void Keep(GameObject handler) // not return this item to pool, deactivate it. Item will get free when it be used;
    {
        owner = handler;
        this.gameObject.SetActive(false);
    }
    protected abstract void ApplyItemTypeParameter();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Player") || collision.name.Contains("Computer"))
        {
            //Keep();
            ItemBag bag = collision.gameObject.GetComponent<ItemBag>();
            if (bag != null)
            {
                //owner = bag.gameObject;
                bag.PutIn(this);
            }
        }
        else if( collision.CompareTag("BorderLine"))
        {
            ItemFree();
        }
    }

}


