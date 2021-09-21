using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Brick : MonoBehaviour, IPooled<Brick>
{
    public event Action<int> OnLetMeFreeed;
    public event Action<int, BrickColorType> OnLetMeMigrated;
    public event Action<int> OnLetMeBoomed;

    //public bool migration = false;
    public int poolID { get; set; }
    public ObjectPooler<Brick> pool { get; set; }
    public abstract void Break();

    //Init fields params
    protected BrickType brickType;
    public BrickColorType BrickColorType { set; get; }
    public Color m_Color { protected set; get; }
    protected float durability;
    protected SpriteRenderer sr;

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    protected virtual void OnEnable()
    {
        ApplyColorTypeParameter();
    }
    /*
    private void Update()
    {
        if (migration == true)
        {
            LetMeMigrate();
            migration = false;
        }
    }
    */
    public virtual void ChangeColor(Color _color)
    {
        m_Color = _color;
        sr.color = m_Color;
    }
    protected void LetMeFree()
    {
        OnLetMeFreeed?.Invoke(poolID);
    }
    protected void LetMeMigrate()
    {
        OnLetMeMigrated?.Invoke(poolID, BrickColorType);
    }
    protected void LetMeBoom()
    {
        OnLetMeBoomed?.Invoke(poolID);
    }

    public abstract void ApplyColorTypeParameter();


}

public enum BrickType
{
    Breakable,
    NonBreakable,
    Penetrateable
}

public enum BrickColorType
{
    White,
    Red,
    None

}