using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BricksManager : MonoBehaviourSingleton<BricksManager>
{
    public Collider2D m_ConstructionArea1;
    public Collider2D m_ConstructionArea2;

    public Brick prefab_RedBrick;
    protected Brick m_LoadedRedBrick = null;
    public Brick loadedRedBrick
    {
        get { return m_LoadedRedBrick; }
    }
    protected ObjectPooler<Brick> m_RedBrickPool;
    private Brick[] redBrickInstances;


    public Brick prefab_HintBrick;
    protected Brick m_LoadedHintBrick = null;
    public Brick loadedHintBrick
    {
        get { return m_LoadedHintBrick; }
    }
    protected ObjectPooler<Brick> m_HintBrickPool;
    private Brick[] hintBrickInstances;


    public Brick prefab_WhiteBrickPrefab;
    protected Brick m_LoadedWhiteBrick = null;
    public Brick loadedWhiteBrick
    {
        get { return m_LoadedWhiteBrick; }
    }
    private ObjectPooler<Brick> m_WhiteBrickPool;
    private Brick[] whiteBrickInstances;

    private const int k_ConstructionSize = 50;
    private Vector3 newLocalSize;
    private Collider2D[] boomCheckResults = new Collider2D[10];

    public struct SeatData
    {
        public Vector2 position;
        public bool isFree;
        public int bID;
        public BrickColorType brickColorType;

    }
    public SeatData[] primarySeatDatas = new SeatData[k_ConstructionSize*2];
    public SeatData[] secondarySeatDatas = new SeatData[k_ConstructionSize*2];
    float thickness = 0.1f;
    public float offset = -0.4f;

    int construction1LastIndex = 20;
    int construction2LastIndex = 20;
    int lastMigratableTargetIndexP1;
    int lastMigratableTargetIndexP2;
    int avaliableSpaceSizeP1;
    int avaliableSpaceSizeP2;


    private void Start()
    {
        //Initialize Pool
        m_WhiteBrickPool = new ObjectPooler<Brick>();
        m_WhiteBrickPool.Initialize(k_ConstructionSize, prefab_WhiteBrickPrefab);
        whiteBrickInstances = m_WhiteBrickPool.instances;

        m_HintBrickPool = new ObjectPooler<Brick>();
        m_HintBrickPool.Initialize(2, prefab_HintBrick);
        hintBrickInstances = m_HintBrickPool.instances;

        m_RedBrickPool = new ObjectPooler<Brick>();
        m_RedBrickPool.Initialize(20, prefab_RedBrick);
        redBrickInstances = m_RedBrickPool.instances;


        newLocalSize.Set(0.5f, 1, 1);

        lastMigratableTargetIndexP1 = construction1LastIndex;
        lastMigratableTargetIndexP2 = construction2LastIndex;

        Construction_P1();
        Construction_P2();
        SubscribeAll();
    }

    private void Update()
    {
    }
    private void SubscribeAll()
    {
        for (int i = 0; i < whiteBrickInstances.Length; i++)
        {
            whiteBrickInstances[i].OnLetMeMigrated += MigrateNewBrick;
            //whiteBrickInstances[i].OnLetMeFreeed += Free;

        }
        for (int i = 0; i < redBrickInstances.Length; i++)
        {
            redBrickInstances[i].OnLetMeMigrated += MigrateNewBrick;
            //redBrickInstances[i].OnLetMeBoomed += BoomBrick;

        }

    }
    void Construction_P1()
    {
        Vector2 size = m_ConstructionArea1.bounds.max - m_ConstructionArea1.bounds.min;
        float width = newLocalSize.x;
        float height = newLocalSize.y;

        int stepX = (int)(size.x / width);
        int stepY = (int)(size.y / height);
        avaliableSpaceSizeP1 = stepX * stepY;
        Vector2 startPos = (Vector2)m_ConstructionArea1.bounds.max + Vector2.down * (0.525f + (-0.025f * stepY));// dont konw the rule
        Vector2 pos;
        for (int x = 0; x < stepX; x++) //Fill avaliable positions
        {
            for (int y = 0; y < stepY; y++)
            {
                int index = x * stepY + y;
                //if (index >= k_ConstructionSize) break;
                pos = startPos + (Vector2.left * x * width * (1 + thickness * height) + Vector2.down * y * height * (1 + thickness * width));

                primarySeatDatas[index].position = pos;
                primarySeatDatas[index].isFree = true;
                primarySeatDatas[index].bID = -1;
                primarySeatDatas[index].brickColorType = BrickColorType.None;
            }
        }

        for (int i = 0; i < construction1LastIndex; i++) //Fill bricks
        {
            PlaceNewBrick(ref primarySeatDatas[i], BrickColorType.White);
        }
    }

    private void Construction_P2()
    {
        Vector2 size = m_ConstructionArea2.bounds.max - m_ConstructionArea2.bounds.min;

        float width = newLocalSize.x;
        float height = newLocalSize.y;

        int stepX = (int)(size.x / width);
        int stepY = (int)(size.y / height);

        avaliableSpaceSizeP2 = stepX * stepY;
        Vector2 startPos = (Vector2)m_ConstructionArea2.bounds.min + Vector2.up * (0.525f + ( -0.025f * stepY )); 
        Vector2 pos;
        
        for (int x = 0; x < stepX; x++)
        {
            for (int y = 0; y < stepY; y++)
            {
                int index = x * stepY + y;
                //if (index >= k_ConstructionSize) break;
                pos = startPos + (Vector2.right * x * width * (1 + thickness * height) + Vector2.up * y * height * (1 + thickness * width));

                secondarySeatDatas[index].position = pos;
                secondarySeatDatas[index].isFree = true;
                secondarySeatDatas[index].bID = -1;
                secondarySeatDatas[index].brickColorType = BrickColorType.None;

            }
        }

        for (int i = 0; i < construction2LastIndex; i++)
        {
            PlaceNewBrick(ref secondarySeatDatas[i], BrickColorType.White);
            /*
            whiteBrickInstances[i+construction1LastIndex].transform.position = secondarySeatDatas[i].position;
            whiteBrickInstances[i+construction1LastIndex].transform.localScale = newLocalSize;
            secondarySeatDatas[i].bID = whiteBrickInstances[i + construction1LastIndex].poolID;
            whiteBrickInstances[i + construction1LastIndex].gameObject.SetActive(true);
            */
        }
    }


    private void MigrateNewBrick(int brickID, BrickColorType previousBrickColorType)
    {
        bool find = false;
        for (int i = 0; i < avaliableSpaceSizeP1; i++) // find brick in primary seatData list, if finded, migrate it to secondary. 
        {
            if (primarySeatDatas[i].bID == brickID && primarySeatDatas[i].brickColorType == previousBrickColorType) // use brick poolID to trace brick gameobject
            {
                for (int j = lastMigratableTargetIndexP2; j < avaliableSpaceSizeP2; j++)
                {
                    if (secondarySeatDatas[j].isFree)
                    {
                        //Place new brick to pre-defined position            
                        PlaceNewBrick(ref secondarySeatDatas[j], hintBrickInstances[0].BrickColorType);
                        ReleaseSeat(ref primarySeatDatas[i]);

                        //Place hint brick to next pre-defined position
                        lastMigratableTargetIndexP2 = j + 1;
                        PlaceHintBrick(secondarySeatDatas[j + 1], 0);
                        find = true;
                        break;
                    }
                }
                if (find) break;
            }
        }
        //if (find) return;
        for (int i = 0; i < avaliableSpaceSizeP2; i++)
        {
            if (secondarySeatDatas[i].bID == brickID && secondarySeatDatas[i].brickColorType == previousBrickColorType)
            {
                for (int j = lastMigratableTargetIndexP1; j < avaliableSpaceSizeP1; j++)
                {
                    if (primarySeatDatas[j].isFree)
                    {
                        PlaceNewBrick(ref primarySeatDatas[j], hintBrickInstances[1].BrickColorType);
                        ReleaseSeat(ref secondarySeatDatas[i]);

                        //Place hint brick to next pre-defined position
                        lastMigratableTargetIndexP1 = j + 1;
                        PlaceHintBrick(primarySeatDatas[j + 1], 1);
                        find = true;
                        break;
                    }

                }
                if (find) break;
            }
        }
        if(find == false)
        {
            for (int i = 0; i < primarySeatDatas.Length; i++)
            {
                Debug.Log("primarySeatDatas[ + " + i + "].bID" + primarySeatDatas[i].bID + " brickColorType" + primarySeatDatas[i].brickColorType);
            }
            for (int i = 0; i < secondarySeatDatas.Length; i++)
            {
                Debug.Log("secondarySeatDatas[ + " + i + "].bID" + secondarySeatDatas[i].bID + " brickColorType" + secondarySeatDatas[i].brickColorType);
            }
            Debug.LogError("NotFind" + brickID + previousBrickColorType);
        }
    }



    //List<int> boomChainID = new List<int>();
    private void BoomBrick(int brickID)
    {
        Queue<int> boomChain = new Queue<int>();
        List<int> boomChainChecked = new List<int>();

        boomChain.Enqueue(brickID);
        while (boomChain.Count > 0)
        {

            #region Debug....
            /*
string ch1 = string.Empty;
foreach (var item in boomChain)
{
    ch1 += item.ToString() + " ";
}
Debug.Log("boomChain" + ch1);

string ch2 = string.Empty;
foreach (var item in boomChainChecked)
{
    ch2 += item.ToString() + " ";
}
Debug.Log("boomChainChecked" + ch2);
            */
            #endregion

            int currentPoolID = boomChain.Dequeue();

            boomChainChecked.Add(currentPoolID);
            int instanceID = -1;
            for (int i = 0; i < redBrickInstances.Length; i++)
            {
                if (redBrickInstances[i].poolID == currentPoolID)
                {
                    instanceID = i;
                    break;
                }
            }

            if (instanceID == -1) break;

            Vector2 topLeft = (Vector2)redBrickInstances[instanceID].transform.position + Vector2.left * newLocalSize.x + Vector2.up * newLocalSize.y;
            Vector2 bottomRight = (Vector2)redBrickInstances[instanceID].transform.position + Vector2.right * newLocalSize.x + Vector2.down * newLocalSize.y;

            int hits = Physics2D.OverlapAreaNonAlloc(topLeft, bottomRight, boomCheckResults, 1 << LayerMask.NameToLayer("Collider"));
            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    if (boomCheckResults[i].name.Contains("RedBrick"))
                    {
                        string[] str = boomCheckResults[i].name.Split(' ');
                        if (!string.IsNullOrEmpty(str[1]))
                        {
                            if (int.TryParse(str[1], out int nextRedBrickID))
                            {
                                if (boomChainChecked.Contains(nextRedBrickID) == false && boomChain.Contains(nextRedBrickID) == false)
                                {
                                    boomChain.Enqueue(nextRedBrickID);

                                }
                            }
                        }

                    }
                }
            }
            Array.Clear(boomCheckResults, 0, hits);
        }
    }

    private void Free(int brickID)
    {
        for (int i = 0; i < whiteBrickInstances.Length; i++)
        {
            if(whiteBrickInstances[i].poolID == brickID)
            {
                whiteBrickInstances[i].OnLetMeMigrated -= MigrateNewBrick;
                whiteBrickInstances[i].OnLetMeFreeed -= Free;
                m_WhiteBrickPool.Free(whiteBrickInstances[i]);
                break;
            }
        }
    }

    private void ReleaseSeat(ref SeatData seatData)
    {
        seatData.bID = -1;
        seatData.isFree = true;
        seatData.brickColorType = BrickColorType.None;
    }

    Brick GetNewBrick(BrickColorType brickColorType)
    {
        Brick temp = null;
        switch (brickColorType)
        {
            case BrickColorType.White:
                temp = m_WhiteBrickPool.GetNew();
                break;
            case BrickColorType.Red:
                temp = m_RedBrickPool.GetNew();
                break;
            default:
                Debug.Log("BrickColorType not valid or =None!");
                break;
        }
        if( temp != null)
            temp.transform.localScale = newLocalSize;
        return temp;
    }

    void PlaceNewBrick(ref SeatData seatData, BrickColorType brickColorType)
    {
        Brick temp = GetNewBrick(brickColorType);
        if(temp != null)
        {
            temp.transform.position = seatData.position;        
            seatData.bID = temp.poolID;
            seatData.brickColorType = brickColorType;
        }
        else
        {
            Debug.Log("BrickColorType not valid!");
        }
    }
    void PlaceBrick(Brick brick, ref SeatData seatData, BrickColorType brickColorType)
    {
        brick.transform.position = seatData.position;
        seatData.bID = brick.poolID;
        seatData.brickColorType = brickColorType;
    }

    private void PlaceHintBrick(SeatData seatData, int instanceID)
    {
        hintBrickInstances[instanceID].transform.position = seatData.position;
        if(hintBrickInstances[instanceID].gameObject.activeSelf == false)
        {
            hintBrickInstances[instanceID].gameObject.SetActive(true);
        }

        Color nextColor = GetRandomColor();
        hintBrickInstances[instanceID].ChangeColor(nextColor);

    }

    private Color GetRandomColor()
    {
        float randomNum = UnityEngine.Random.Range(0, 3f);
        Color nextColor = Color.white;
        if (randomNum > 2f)
            nextColor = Color.red;
        else if (randomNum <= 2f && randomNum > 1f)
            nextColor = Color.blue;
        else
            nextColor = Color.green;
        return nextColor;
    }



}

//whiteBrickInstances[7].transform.position = startPos;

/*
for (int x = 0; x < stepX; x++)
{
    for (int y = 0; y < stepY; y++)
    {
        int index = x * stepX + y + k_ConstructionSize / 2; // 25~49
        if (index >= k_ConstructionSize ) return;

        pos = startPos + (Vector2.right * x * width * (1 + thickness * height) + Vector2.up * y * height * (1 + thickness * width));

        whiteBrickInstances[index].transform.localScale = newLocalSize;
        whiteBrickInstances[index].transform.position = pos;
        secondarySeatDatas[index].brick = whiteBrickInstances[index];

        index -= k_ConstructionSize / 2;
        secondarySeatDatas[index].position = pos;
        secondarySeatDatas[index].isFree = false;
    }
}
*/
/*
for (int x = 0; x < stepX; x++)
{
    for (int y = 0; y < stepY; y++)
    {
        int index = x * stepX + y; // 0 ~ 24

        if (index >= k_ConstructionSize / 2) return;
        pos = startPos + (Vector2.left * x * width * (1 + thickness * height) + Vector2.down * y * height * (1 + thickness * width));

        whiteBrickInstances[index].transform.localScale = newLocalSize;
        whiteBrickInstances[index].transform.position = pos;
        primarySeatDatas[index].brick = whiteBrickInstances[index];

        primarySeatDatas[index].position = pos;
        primarySeatDatas[index].isFree = false;

    }
}
*/

/*
//store neibor brick index
primarySeatDatas[index].topLeft = index + stepY;
primarySeatDatas[index].top = index - 1;
primarySeatDatas[index].topRight = index - stepY - 2;
primarySeatDatas[index].middleLeft = index + stepY + 1;
primarySeatDatas[index].middle = index;
primarySeatDatas[index].middleRight = index - stepY - 1;
primarySeatDatas[index].bottomLeft = index + stepY + 2;
primarySeatDatas[index].bottom = index + 1;
primarySeatDatas[index].bottomRight = index - stepY;

if( index <= stepY)
{
    if(index == 0)
    {
        primarySeatDatas[index].topRight = -1;
        primarySeatDatas[index].middleRight = stepY;
        primarySeatDatas[index].bottomRight = stepY - 1;
    }
}

*/
/*
//neiborBrick seat index;
// topleft top top right
// 4 middle 6
// 7 bottom 9
public int topLeft;
public int top;
public int topRight;
public int middleLeft;
public int middle;
public int middleRight;
public int bottomLeft;
public int bottom;
public int bottomRight;  
*/

/*

    private void MigrateBrick(int brickID)
    {
        int instanceID = -1;
        Brick currentInstance;

        Debug.Log("sss");
        if (hintBrickInstances[0].m_Color == Color.red)
        {
            currentInstance = GetNewBrick(BrickColorType.Red);
        }
        else if (hintBrickInstances[0].m_Color == Color.white)
        {
            currentInstance = GetNewBrick(BrickColorType.White);
        }
        else
        {
            currentInstance = GetNewBrick(BrickColorType.White);
        }

        /*
        for (int i = 0; i < whiteBrickInstances.Length; i++) // find brick instance in whiteBrickInstances[]
        {
            if (whiteBrickInstances[i].poolID == brickID)
            {
                instanceID = i;
                break;
            }
        }
        */


/*
Debug.Log("instanceID" + instanceID);
Debug.Log("instancePoolID" + whiteBrickInstances[instanceID].poolID);

if (instanceID == -1) Debug.Log("instanceID nUll");
else
    Debug.Log("Search1");
*/
/*
bool find = false;
for (int i = 0; i < avaliableSpaceSizeP1; i++) // find brick in primary seatData list, if finded, migrate it to secondary. 
{
    if (primarySeatDatas[i].bID == brickID) // use brick poolID to trace brick gameobject
    {
        for (int j = lastMigratableTargetIndexP2; j < avaliableSpaceSizeP2; j++)
        {
            if (secondarySeatDatas[j].isFree)
            {
                //Place white brick to pre-defined position                       
                whiteBrickInstances[instanceID].transform.position = secondarySeatDatas[j].position;
                secondarySeatDatas[j].isFree = false;
                secondarySeatDatas[j].bID = whiteBrickInstances[instanceID].poolID;
                primarySeatDatas[i].bID = -1;
                primarySeatDatas[i].isFree = true;
                lastMigratableTargetIndexP2 = j + 1;

                //Place hint brick to next pre-defined position
                lastMigratableTargetIndexP2 = j + 1;
                PlaceHintBrick(secondarySeatDatas[j + 1], 0);
                find = true;
                break;
            }
        }
        if (find) break;
    }

}
if (find) return;
for (int i = 0; i < avaliableSpaceSizeP2; i++)
{
    if (secondarySeatDatas[i].bID == brickID)
    {
        for (int j = lastMigratableTargetIndexP1; j < avaliableSpaceSizeP1; j++)
        {
            if (primarySeatDatas[j].isFree)
            {
                whiteBrickInstances[instanceID].transform.position = primarySeatDatas[j].position;
                primarySeatDatas[j].isFree = false;
                primarySeatDatas[j].bID = whiteBrickInstances[instanceID].poolID;
                secondarySeatDatas[i].bID = -1;
                secondarySeatDatas[i].isFree = true;
                lastMigratableTargetIndexP1 = j + 1;

                //Place hint brick to next pre-defined position
                PlaceHintBrick(primarySeatDatas[j + 1], 1);
                find = true;
                break;
            }

        }
        if (find) break;
    }
}

if (find == false)
{
    Debug.Log("Not Find!");
    
    whiteBrickInstances[instanceID].gameObject.SetActive(false);
    whiteBrickInstances[instanceID].pool.Free(whiteBrickInstances[instanceID]);
    
    m_WhiteBrickPool.Free(whiteBrickInstances[instanceID]);
}
    }

*/

/*
newBrickInstance.transform.position = primarySeatDatas[j].position;
primarySeatDatas[j].isFree = false;
primarySeatDatas[j].bID = newBrickInstance.poolID;
*/
/*
newBrickInstance.transform.position = secondarySeatDatas[j].position;
secondarySeatDatas[j].isFree = false;
secondarySeatDatas[j].bID = newBrickInstance.poolID;
*/