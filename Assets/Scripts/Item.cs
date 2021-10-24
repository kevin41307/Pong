using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemType itemType { protected set; get; }
    protected SpriteRenderer sr;

    public static Color DecideColor(ItemType itemType)
    {
        Color newColor = default;
        switch (itemType)
        {
            case ItemType.Red:
                newColor = Color.red;
                break;
            case ItemType.Green:
                newColor = Color.green;
                break;
            case ItemType.Blue:
                newColor = Color.blue;
                break;
            case ItemType.Purple:
                newColor = new Color(.9f, 0, .9f, 1f);
                break;
            case ItemType.Yellow:
                newColor = new Color(1f, 1f, 0, 1f);
                break;
            case ItemType.Cyan:
                newColor = new Color(0f, .9f, .9f, 1f);
                break;
            default:
                newColor = Color.white;
                break;
        }
        return newColor;
    }
}
public enum ItemType
{
    Red,
    Green,
    Blue,
    Yellow,
    Purple,
    Cyan
}

