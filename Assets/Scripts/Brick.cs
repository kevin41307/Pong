using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Brick : MonoBehaviour, IPooled<Brick>, ISeatTicket
{
    public event Action<int> OnLetMeFreeed;
    public event Action<Brick, BrickColorType, int> OnLetMeMigrated;
    public event Action<Brick, MonoBehaviour> OnLetMeBoomed;
    public event Action<ISeatTicket> OnLetSeatReleased;
    public event Action<CompensationInfo> OnBrickBreaked;

    //public bool migration = false;
    public int poolID { get; set; }
    public ObjectPooler<Brick> pool { get; set; }

    //Init fields params
    protected BrickType brickType;
    public BrickColorType m_BrickColorType { set; get; }
    public Color m_Color { protected set; get; }
    public float Durability { set; get; }
    protected float durabilityMax;
    public SeatOrder seatOrder { get; set; }
    public int seatStartID { get; set; }
    public int seatCount { get; set; }

    protected int MigrateMultiplier { set; get; }

    protected SpriteRenderer sr;
    protected CrackLineDecalSystem crackLineDecalSystem;
    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        MigrateMultiplier = 1;
    }

    protected virtual void OnEnable()
    {
        ApplyColorTypeParameter();
        if (crackLineDecalSystem == null)
            crackLineDecalSystem = new CrackLineDecalSystem(sr, m_BrickColorType);
        crackLineDecalSystem.Apply(0);
    }
    public abstract void Break(CompensationInfo info);

    /// <summary>
    /// usually call by manager, break it but dont invoke any typed behavior
    /// </summary>
    public void SimpleBreak(CompensationInfo info)
    {
        if (info.dontNeedCompensate == false)
            ItemOnWorldManager.Instance.ItemIncreased(info);
        pool.Free(this);
        LetSeatReleased();
        BrickBreaked(info);
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
    public virtual void ChangeDurability(float value)
    {
        Durability = value;
        durabilityMax = Durability;
    }
    public virtual void ChangeMigrateMultiplier(int n)
    {
        ChangeDurability(n);
        MigrateMultiplier = (int)Durability;
    }
    protected void LetMeFree()
    {
        OnLetMeFreeed?.Invoke(poolID);
    }
    protected void LetMeMigrate()
    {
        OnLetMeMigrated?.Invoke(this, m_BrickColorType, MigrateMultiplier);
    }
    protected void LetMeBoom(MonoBehaviour breaker)
    {
        OnLetMeBoomed?.Invoke(this, breaker);
    }
    protected void LetSeatReleased()
    {
        OnLetSeatReleased?.Invoke(this);
    }

    protected void BrickBreaked(CompensationInfo info)
    {
        OnBrickBreaked?.Invoke(info);
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
    Blue,
    Green,
    None

}
public class CrackLineDecalSystem
{
    BrickColorType m_brickColorType;
    MaterialInventory materialInventory;
    SpriteRenderer sr;
    public CrackLineDecalSystem(SpriteRenderer _sr, BrickColorType brickColorType)
    {
        materialInventory = MaterialInventory.Instance;
        m_brickColorType = brickColorType;
        sr = _sr;
    }
    public void Apply(int intensity)
    {
        sr.material = materialInventory.GetMaterialInstance(m_brickColorType, intensity);
    }
}