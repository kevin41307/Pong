using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBag : MonoBehaviour
{
    public bool isPlayerUsing = false;

    public List<ItemOnWorld> handedItemList { protected set; get; }
    Material playerMat;
    SpriteRenderer sr;
    const int k_MaxBagCount = 10;
    int[] hashColors = new int[k_MaxBagCount];
    int nextNewballCount;
    private void Reset()
    {
        handedItemList.Clear();
    }

    private void Awake()
    {
        handedItemList = new List<ItemOnWorld>();
        sr = GetComponent<SpriteRenderer>();
        playerMat = sr.material;
        StoreProtertyID();
        UpdateHorizontalColor();
        //MyGameEventSystem.Instance.OnAcumulatedBalls += AcumulatedBalls;
        MyGameEventSystem.AcumulatedBallEvent.performed += AcumulatedBalls;
    }

    private void AcumulatedBalls(int count, GameObject applicant)
    {
        if (applicant != this.gameObject) return;
        nextNewballCount += count;
    }

    private void Start()
    {
        if(isPlayerUsing)
            UserInput.Instance.inputActions.Player.Fire1.canceled += Fire1_canceled; //race condition with BallsManager:BallIncreased() this function should be later in invocation list

    }

    private void Fire1_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        UseItems();
    }

    public void PutIn(ItemOnWorld item)
    {
        item.Keep(this.gameObject);

        if (handedItemList.Count <= k_MaxBagCount)
            Enqueue(item);
        else
        {
            Dequeue();
            Enqueue(item);
        }
        //Helpers<ItemOnWorld>.DebugInOneLine(handedItemList);
        CombineColor();
        CheckPrimaryColor();
        UpdateHorizontalColor();
    }

    public void UseItems()
    {
        List<ItemOnWorld> marked = new List<ItemOnWorld>(); //extract green item, deal it first. 
        foreach (var item in handedItemList)
        {
            if (item.itemType == ItemType.Green)
            {
                item.Use(this.gameObject);
                item.ItemFree();
                marked.Add(item);
            }
        }
        foreach (var m in marked) // remove green item; TODO: SKIP INDEX 
        {
            handedItemList.Remove(m);
        }
        marked.Clear();
        //MyGameEventSystem.Instance.UseBallIncreasedItem(nextNewballCount, this.gameObject);
        MyGameEventSystem.UseBallIncreasedItemEvent.Perform(nextNewballCount, this.gameObject);
        nextNewballCount = 0;

        foreach (var item in handedItemList)
        {
            item.Use(this.gameObject);
            item.ItemFree();
        }

        ClearBag();
    }

    private void ClearBag()
    {
        handedItemList.Clear();
        UpdateHorizontalColor();
    }

    private void CheckPrimaryColor()
    {
        if (handedItemList.Count < 3) return;
        ItemOnWorld red = null;
        ItemOnWorld green = null;
        ItemOnWorld blue = null;
        for (int i = 0; i < handedItemList.Count; i++)
        {
            switch (handedItemList[i].itemType)
            {
                case ItemType.Red:
                    red = handedItemList[i];
                    break;
                case ItemType.Green:
                    green = handedItemList[i];
                    break;
                case ItemType.Blue:
                    blue = handedItemList[i];
                    break;
                default:
#if UNITY_EDITOR
                    //Debug.Log("not primary color item type! " + handedItemList[i].itemType);
#endif
                    break;
            }
            if(red != null && green != null && blue != null)
            {
                //Debug.Log("redIdx" + redIdx + "greenIdx" + greenIdx + "blueIdx" + blueIdx);
                red.ItemFree();
                green.ItemFree();
                blue.ItemFree();
                handedItemList.Remove(red);
                handedItemList.Remove(green);
                handedItemList.Remove(blue);
                red = null;
                green = null;
                blue = null;
            }
        }
    }
    
    private void CombineColor()
    {
        if (handedItemList.Count < 2) return;
        ItemOnWorld red = null;
        ItemOnWorld green = null;
        ItemOnWorld blue = null;
        
        for (int i = handedItemList.Count-1; i > handedItemList.Count-3; i--)
        {
            switch (handedItemList[i].itemType)
            {
                case ItemType.Red:
                    red = handedItemList[i];
                    break;
                case ItemType.Green:
                    green = handedItemList[i];
                    break;
                case ItemType.Blue:
                    blue = handedItemList[i];
                    break;
                default:
                    break;
            }
        }
        if (red != null && blue != null )
        {
            //Debug.Log("redIdx" + redIdx + "greenIdx" + greenIdx + "blueIdx" + blueIdx);
            red.ItemFree();
            blue.ItemFree();
            handedItemList.Remove(red);
            handedItemList.Remove(blue);
            ItemOnWorld_Gravity item = ItemOnWorldManager.Instance.itemOnWorld_Gravity.pool.GetNew();
            if(item != null)
                PutIn(item);

        } 
        else if(red != null && green != null)
        {
            red.ItemFree();
            green.ItemFree();
            handedItemList.Remove(red);
            handedItemList.Remove(green);
            ItemOnWorld_PushLevel item = ItemOnWorldManager.Instance.itemOnWorld_PushLevel.pool.GetNew();
            if (item != null)
                PutIn(item);
        } 
        else if(green != null && blue != null)
        {
            green.ItemFree();
            blue.ItemFree();
            handedItemList.Remove(green);
            handedItemList.Remove(blue);
            ItemOnWorld_Portal item = ItemOnWorldManager.Instance.itemOnWorld_Portal.pool.GetNew();
            if (item != null)
                PutIn(item);
            //Helpers<ItemOnWorld>.DebugInOneLine(handedItemList);

        }
        else
        {
            //Debug.Log("Individual Color.");
        }
        red = null;
        green = null;
        blue = null;
    }


    private void Enqueue(ItemOnWorld item)
    {
        handedItemList.Add(item);
    }    
    private void Dequeue()
    {
        handedItemList.RemoveAt(0);
    }

    private void UpdateHorizontalColor()
    {
        for (int i = 0; i < 10; i++)
        {
            if(i < handedItemList.Count)
            {
                playerMat.SetColor(hashColors[i], Item.DecideColor(handedItemList[i].itemType));
            }
            else
            {
                playerMat.SetColor(hashColors[i], Color.white);
            }

        }
    }

    private void StoreProtertyID()
    {
        for (int i = 0; i < k_MaxBagCount; i++)
        {
            hashColors[i] = Shader.PropertyToID("_mColor" + i.ToString());
        }
    }
}
