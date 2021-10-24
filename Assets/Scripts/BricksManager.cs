using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BricksManager : MonoBehaviourSingleton<BricksManager>
{
    public Collider2D m_ConstructionArea1;
    public Collider2D m_ConstructionArea2;

    public Brick prefab_RedBrick;
    protected ObjectPooler<Brick> m_RedBrickPool;
    private Brick[] redBrickInstances;

    public Brick prefab_BlueBrick;
    protected ObjectPooler<Brick> m_BlueBrickPool;
    private Brick[] blueBrickInstances;

    public Brick prefab_GreenBrick;
    protected ObjectPooler<Brick> m_GreenBrickPool;
    private Brick[] greenBrickInstances;

    public Brick prefab_HintBrick;
    protected ObjectPooler<Brick> m_HintBrickPool;
    private Brick[] hintBrickInstances;

    public Brick prefab_WhiteBrickPrefab;
    private ObjectPooler<Brick> m_WhiteBrickPool;
    private Brick[] whiteBrickInstances;

    private const int k_ConstructionSize = 60;
    private Vector3 newLocalSize;
    private Collider2D[] boomCheckResults = new Collider2D[10];
    private int greenBrickUseCount = 0;
    public struct SeatData
    {
        public bool isFree;
        public int bID;
        public BrickColorType brickColorType;

        public Vector2 position;

        // only change in first 
        public int row;
        public int column;
        public int seatNumber;
        public SeatOrder seatOrder;
        
        public void ApplyOffset(Vector2 offset)
        {
            position += offset;
        }
    }
    SeatData nullSeat = new SeatData {
        bID = -1,
        isFree = false,
        brickColorType = BrickColorType.None,
        position = -Vector2.one,
        seatNumber = -1,
        row = -1,
        column = -1
    };
    public SeatData[] primarySeatDatas = new SeatData[k_ConstructionSize * 2];
    public SeatData[] secondarySeatDatas = new SeatData[k_ConstructionSize * 2];
    float thickness = 0.1f;
    //public float offset = -0.4f;

    int construction1LastIndex = 27;
    int construction2LastIndex = 27;
    int lastMigratableTargetIndexP1;
    int lastMigratableTargetIndexP2;
    int avaliableSpaceSizeP1;
    int avaliableSpaceSizeP2;
    int XEdgeCount = 0;
    int YEdgeCount = 0;

    //public Vector2 pushOffset;
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
        m_RedBrickPool.Initialize(40, prefab_RedBrick);
        redBrickInstances = m_RedBrickPool.instances;

        m_BlueBrickPool = new ObjectPooler<Brick>();
        m_BlueBrickPool.Initialize(30, prefab_BlueBrick);
        blueBrickInstances = m_BlueBrickPool.instances;

        m_GreenBrickPool = new ObjectPooler<Brick>();
        m_GreenBrickPool.Initialize(40, prefab_GreenBrick);
        greenBrickInstances = m_GreenBrickPool.instances;

        greenBrickUseCount = 0;
        newLocalSize.Set(0.5f, 1, 1);

        lastMigratableTargetIndexP1 = construction1LastIndex;
        lastMigratableTargetIndexP2 = construction2LastIndex;

        Construction_P1();
        Construction_P2();
        SubscribeAll();

        RestrictArea.OnHasSetMoveableArea += PushLevel;
    }

    private void SubscribeAll() //TODO: Clear invocation list in brick when free.
    {
        for (int i = 0; i < whiteBrickInstances.Length; i++)
        {
            whiteBrickInstances[i].OnLetMeMigrated += MigrateNewBrick;
            whiteBrickInstances[i].OnLetSeatReleased += ReleaseSeat;
        }
        for (int i = 0; i < redBrickInstances.Length; i++)
        {
            redBrickInstances[i].OnLetMeMigrated += MigrateNewBrick;
            redBrickInstances[i].OnLetMeBoomed += BoomBrick;
            redBrickInstances[i].OnLetSeatReleased += ReleaseSeat;
        }
        for (int i = 0; i < blueBrickInstances.Length; i++)
        {
            blueBrickInstances[i].OnLetMeMigrated += MigrateNewBrick;
            blueBrickInstances[i].OnLetSeatReleased += ReleaseSeat;
        }
        for (int i = 0; i < greenBrickInstances.Length; i++)
        {
            greenBrickInstances[i].OnLetMeMigrated += MigrateNewBrick;
            greenBrickInstances[i].OnLetSeatReleased += ReleaseSeat;
        }


    }

    void Construction_P1()
    {
        Vector2 size = m_ConstructionArea1.bounds.max - m_ConstructionArea1.bounds.min;
        float width = newLocalSize.x;
        float height = newLocalSize.y;

        int stepX = (int)(size.x / width);
        int stepY = (int)(size.y / height);
        XEdgeCount = stepX;
        YEdgeCount = stepY;
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
                primarySeatDatas[index].row = x;
                primarySeatDatas[index].column = y;
                primarySeatDatas[index].seatNumber = index;
                primarySeatDatas[index].seatOrder = SeatOrder.Primary;
            }
        }
        /*
        for (int i = 0; i < construction1LastIndex; i++) //Fill bricks
        {
            if (i == 3 || i == 5 || i == 6 || i == 10 || i == 14 || i == 18) PlaceNewBrick(ref primarySeatDatas[i], BrickColorType.White);
            else
                PlaceNewBrick(ref primarySeatDatas[i], BrickColorType.Blue);
        }
        */
        for (int i = 0; i < construction1LastIndex; i++)
        {
            PlaceNewBrick(ref primarySeatDatas[i], BrickColorType.White);
            /*
            whiteBrickInstances[i+construction1LastIndex].transform.position = secondarySeatDatas[i].position;
            whiteBrickInstances[i+construction1LastIndex].transform.localScale = newLocalSize;
            secondarySeatDatas[i].bID = whiteBrickInstances[i + construction1LastIndex].poolID;
            whiteBrickInstances[i + construction1LastIndex].gameObject.SetActive(true);
            */
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
        Vector2 startPos = (Vector2)m_ConstructionArea2.bounds.min + Vector2.up * (0.525f + (-0.025f * stepY));
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
                secondarySeatDatas[index].row = x;
                secondarySeatDatas[index].column = y;
                secondarySeatDatas[index].seatNumber = index;
                secondarySeatDatas[index].seatOrder = SeatOrder.Secondary;
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
    private void ReBuildSeatPosition(Vector2 pivot, Vector2 size, ref SeatData[] seatDatas, Vector2 right, Vector2 up)
    {
        float width = newLocalSize.x;
        float height = newLocalSize.y;

        int stepX = (int)(size.x / width);
        int stepY = (int)(size.y / height);
        Vector2 startPos = pivot + up * (0.525f + (-0.025f * stepY));// dont konw the rule
        Vector2 pos;
        for (int x = 0; x < stepX; x++) //Fill avaliable positions
        {
            for (int y = 0; y < stepY; y++)
            {
                int index = x * stepY + y;
                //pos = startPos + (Vector2.right * x * width * (1 + thickness * height) + Vector2.up * y * height * (1 + thickness * width));
                pos = CaculateSeatPosition(startPos, x, y, width, height, thickness, right, up);
                seatDatas[index].position = pos;
            }
        }
    }
    private Vector2 CaculateSeatPosition(Vector2 startPos, int x, int y, float width, float height, float thickness, Vector2 right, Vector2 up)
        => startPos + (right * x * width * (1 + thickness * height) + up * y * height * (1 + thickness * width));
    private void PushLevel(float distance)
    {
        Vector3 offset = new Vector3(distance, 0, 0);
        PushBrickPosition(m_WhiteBrickPool, offset);
        PushBrickPosition(m_RedBrickPool, offset);
        PushBrickPosition(m_BlueBrickPool, offset);
        PushBrickPosition(m_GreenBrickPool, offset);
        PushBrickPosition(m_HintBrickPool, offset);
        
        ApplySeatOffset(ref primarySeatDatas, offset);
        ApplySeatOffset(ref secondarySeatDatas, offset);
        PlaceHintBrick(FindNextFreeSeat(primarySeatDatas, lastMigratableTargetIndexP1, avaliableSpaceSizeP1), 0);
        PlaceHintBrick(FindNextFreeSeat(secondarySeatDatas, lastMigratableTargetIndexP2, avaliableSpaceSizeP2), 1);
    }
    private void ApplySeatOffset(ref SeatData[] seatDatas, Vector2 offset)
    {
        if (seatDatas == null) return;
        for (int i = 0; i < seatDatas.Length; i++)
        {
            seatDatas[i].ApplyOffset(offset);
        }
    }

    private void PushBrickPosition(ObjectPooler<Brick> targetPool, Vector3 offset)
    {
        for (int i = 0; i < targetPool.notFreeIdx.Count; i++)
        {
            targetPool.instances[targetPool.notFreeIdx[i]].transform.position += (Vector3)offset;
        }
    }
    private void MigrateNewBrick(Brick breakedBrick, BrickColorType previousBrickColorType, int count)
    {
        if (CheckLastBrick()) return;
        SeatOrder previousSeatOrder = breakedBrick.seatOrder;
        if (previousSeatOrder == SeatOrder.Primary)
        {
            for (int i = 0; i < count; i++)
            {
                
                SeatData nextSeat = FindNextFreeSeat(secondarySeatDatas, lastMigratableTargetIndexP2, avaliableSpaceSizeP2, "Ball", "Ball");
                if(nextSeat.seatNumber != -1)
                {
                    PlaceNewBrick(ref secondarySeatDatas[nextSeat.seatNumber], hintBrickInstances[1].m_BrickColorType);
                    lastMigratableTargetIndexP2 = nextSeat.seatNumber + 1;
                    PlaceHintBrick(FindNextFreeSeat(secondarySeatDatas, lastMigratableTargetIndexP2, avaliableSpaceSizeP2), 1);
                }
                /*
                for (int j = lastMigratableTargetIndexP2; j < avaliableSpaceSizeP2; j++)
                {
                    if (secondarySeatDatas[j].isFree)
                    {
                        if (Helpers.DetectObstacle(secondarySeatDatas[j].position + Vector2.up * newLocalSize.y * 0.6f + Vector2.left * newLocalSize.x * 0.6f,
                                                    secondarySeatDatas[j].position + Vector2.down * newLocalSize.y * 0.6f + Vector2.right * newLocalSize.x * 0.6f
                                                    , "Ball", "Ball"))
                        {
                            continue;
                        }
                        //Place new brick to pre-defined seat            
                        PlaceNewBrick(ref secondarySeatDatas[j], hintBrickInstances[1].m_BrickColorType);

                        //Place hint brick to next pre-defined seat
                        lastMigratableTargetIndexP2 = j + 1;
                        //PlaceHintBrick(secondarySeatDatas[j + 1], 1);

                        PlaceHintBrick(FindNextFreeSeat(secondarySeatDatas, lastMigratableTargetIndexP2, avaliableSpaceSizeP2), 1);
                        find = true;
                        break;
                    }
                }
                 */
            }
            UpdateHintBrickColor(1);
        }
        else if (previousSeatOrder == SeatOrder.Secondary)
        {
            for (int i = 0; i < count; i++)
            {
               
                SeatData nextSeat = FindNextFreeSeat(primarySeatDatas, lastMigratableTargetIndexP1, avaliableSpaceSizeP1, "Ball", "Ball");
                if (nextSeat.seatNumber != -1)
                {
                    PlaceNewBrick(ref primarySeatDatas[nextSeat.seatNumber], hintBrickInstances[0].m_BrickColorType);
                    lastMigratableTargetIndexP1 = nextSeat.seatNumber + 1;
                    PlaceHintBrick(FindNextFreeSeat(primarySeatDatas, lastMigratableTargetIndexP1, avaliableSpaceSizeP1), 0);
                }

                /*
               for (int j = lastMigratableTargetIndexP1; j < avaliableSpaceSizeP1; j++)
               {
                   if (primarySeatDatas[j].isFree)
                   {
                       if (Helpers.DetectObstacle(primarySeatDatas[j].position + Vector2.up * newLocalSize.y * 0.6f + Vector2.left * newLocalSize.x * 0.6f,
                           primarySeatDatas[j].position + Vector2.down * newLocalSize.y * 0.6f + Vector2.right * newLocalSize.x * 0.6f
                           , "Ball", "Ball"))
                       {
                           continue;
                       }
                       PlaceNewBrick(ref primarySeatDatas[j], hintBrickInstances[0].m_BrickColorType);

                       //Place hint brick to next pre-defined position
                       lastMigratableTargetIndexP1 = j + 1;
                       //PlaceHintBrick(primarySeatDatas[j + 1], 0);
                       PlaceHintBrick(FindNextFreeSeat(primarySeatDatas, lastMigratableTargetIndexP1, avaliableSpaceSizeP1), 0);
                       find = true;
                       break;
                   }
               }
               */
            }
            UpdateHintBrickColor(0);
        }
    }
    //List<int> boomChainID = new List<int>();
    private void BoomBrick(Brick firstBrick, MonoBehaviour breaker)
    {
        Queue<int> boomChain = new Queue<int>();
        List<int> boomChainChecked = new List<int>();

        boomChain.Enqueue(firstBrick.poolID);
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
            if (m_RedBrickPool.FreeIdx.Contains(instanceID)) return;

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

        /*
        string ch2 = string.Empty;
        foreach (var item in boomChainChecked)
        {
            ch2 += item.ToString() + " ";
        }
        Debug.Log("boomChainChecked" + ch2);
        */
        foreach (var index in boomChainChecked)
        {
            if(index != firstBrick.poolID)
                m_RedBrickPool.instances[index].SimpleBreak(new CompensationInfo { breaker = breaker, brick = m_RedBrickPool.instances[index] });
        }
        boomChainChecked.Clear();
    }
    private void ConnectionCheckStepByStep(Brick firstBrick)
    {
        //int column = (int) ()
        SeatData seat = FindSeat(firstBrick.poolID, BrickColorType.Blue);
        /*
        Debug.Log("seatNumber" + seat.seatNumber);
        Debug.Log("row" + seat.row);
        Debug.Log("column" + seat.column);
        */
        List<int> connectedBlueBrickList = new List<int>();
        SeatData[] seatDatas = null;
        if (seat.seatOrder == SeatOrder.Primary)
        {
            seatDatas = primarySeatDatas;
        }
        else if (seat.seatOrder == SeatOrder.Secondary)
        {
            seatDatas = secondarySeatDatas;
        }

        int index = 0;
        int offset = seat.row * YEdgeCount;
        for (int i = 0; i < YEdgeCount; i++)
        {
            index = i + offset;
            //Debug.Log("i " + i + "color " + seatDatas[i].brickColorType);

            if (seatDatas[index].brickColorType == BrickColorType.Blue)
            {
                if (connectedBlueBrickList.Count == 0)
                    connectedBlueBrickList.Add(index);
                else if (connectedBlueBrickList[connectedBlueBrickList.Count - 1] + 1 == index)
                {
                    connectedBlueBrickList.Add(index);
                }
            }
            else
            {
                
                if (connectedBlueBrickList.Count > 1)
                {
                    //Debug.Log("a "  + seatDatas[i].brickColorType);
                    PlaceConnectedBrick(ref seatDatas, connectedBlueBrickList);
                    //Debug.Log("b " + seatDatas[i].brickColorType);
                }
                else if (connectedBlueBrickList.Count == 1)
                {
                    //PlaceConnectedBrick(ref seatDatas, connectedBlueBrickList);
                    firstBrick.seatStartID = connectedBlueBrickList[0];
                    firstBrick.seatCount = 1;
                    firstBrick.seatOrder = seat.seatOrder;
                    //Debug.Log("seat.seatOrder" + seat.seatOrder);
                }
                connectedBlueBrickList.Clear();
            }
        }
        if (connectedBlueBrickList.Count > 1)
        {
            PlaceConnectedBrick(ref seatDatas, connectedBlueBrickList);
        }
        else if (connectedBlueBrickList.Count == 1)
        {
            firstBrick.seatStartID = connectedBlueBrickList[0];
            firstBrick.seatCount = 1;
            firstBrick.seatOrder = seat.seatOrder;
        }
        connectedBlueBrickList.Clear();

    }
    private void ConnectionCheck()
    {
        List<int> connectedBlueBrickList = new List<int>();
        for (int x = 0; x < XEdgeCount; x++) //Fill avaliable positions
        {

            for (int y = 0; y < YEdgeCount; y++)
            {
                int index = x * YEdgeCount + y;
                //if (index >= k_ConstructionSize) break;
                //Debug.Log("x" + x + "y" + y + "index" + index);
                if (primarySeatDatas[index].brickColorType == BrickColorType.Blue)
                {
                    if (connectedBlueBrickList.Count == 0)
                        connectedBlueBrickList.Add(index);
                    else if (connectedBlueBrickList[connectedBlueBrickList.Count - 1] + 1 == index)
                    {
                        connectedBlueBrickList.Add(index);
                    }
                }
                else
                {
                    if (connectedBlueBrickList.Count > 1)
                    {
                        /*
                        string str = string.Empty;
                        foreach (var item in connectedBlueBrickList)
                        {
                            str += item + " ";
                        }
                        Debug.Log(str);
                        Debug.Log("out");
                        
                        Brick newBrick = GetNewBrick(BrickColorType.Blue);             
                        Vector2 newPosition = AverageSeatPosition(primarySeatDatas, connectedBlueBrickList[0], connectedBlueBrickList[connectedBlueBrickList.Count - 1]);
                        newBrick.transform.position = newPosition;
                        newBrick.transform.localScale = new Vector3(newBrick.transform.localScale.x, connectedBlueBrickList.Count + ((connectedBlueBrickList.Count - 1 )* 0.05f), newBrick.transform.localScale.z);
                        Debug.Log(connectedBlueBrickList.Count);
                        */
                        PlaceConnectedBrick(ref primarySeatDatas, connectedBlueBrickList);
                    }
                    connectedBlueBrickList.Clear();
                }

            }

            if (connectedBlueBrickList.Count > 1)
            {
                /*
                string str = string.Empty;
                foreach (var item in connectedBlueBrickList)
                {
                    str += item + " ";
                }
                Debug.Log(str);
                Debug.Log("out");
                Brick newBrick = GetNewBrick(BrickColorType.Blue);
                Vector2 newPosition = AverageSeatPosition(primarySeatDatas, connectedBlueBrickList[0], connectedBlueBrickList[connectedBlueBrickList.Count - 1]);
                newBrick.transform.position = newPosition;
                newBrick.transform.localScale = new Vector3(newBrick.transform.localScale.x, connectedBlueBrickList.Count + ((connectedBlueBrickList.Count - 1) * 0.05f), newBrick.transform.localScale.z);
                Debug.Log(connectedBlueBrickList.Count);
                */

                PlaceConnectedBrick(ref primarySeatDatas, connectedBlueBrickList);

            }
            connectedBlueBrickList.Clear();
            /*
            if (blueBrickList.Count > 0)
            {
                string str = string.Empty;
                foreach (var item in blueBrickList)
                {
                    str += item + " ";
                }
                Debug.Log(str);
            }
            
            
            
            */
            //blueBrickList.Clear();
        }
    }
    private void PlaceConnectedBrick(ref SeatData[] seats, List<int> connectedList)
    {
        for (int i = connectedList[0]; i <= connectedList[connectedList.Count - 1]; i++)
        {
            m_BlueBrickPool.Free(m_BlueBrickPool.instances[seats[i].bID]); // bID is eqaulity to poolID           
            //ReleaseSeat(ref seats[i]);
            //Debug.Log("i" + i);

        }
        Brick newBrick = GetNewBrick(BrickColorType.Blue);
        Vector2 newPosition = AverageSeatPosition(seats, connectedList[0], connectedList[connectedList.Count - 1]);
        newBrick.transform.position = newPosition;
        newBrick.transform.localScale = new Vector3(newBrick.transform.localScale.x, connectedList.Count + ((connectedList.Count - 1) * 0.05f), newBrick.transform.localScale.z);
        newBrick.ChangeDurability(connectedList.Count);
        //newBrick.ChangeMigrateMultiplier(connectedList.Count);
        newBrick.seatOrder = seats[connectedList[0]].seatOrder;
        newBrick.seatStartID = connectedList[0];
        newBrick.seatCount = connectedList.Count;


        for (int i = connectedList[0]; i <= connectedList[connectedList.Count - 1]; i++)
        {
            PlaceBrick(newBrick, newPosition, ref seats[i], BrickColorType.Blue);
            /*
            seats[i].bID = newBrick.poolID;
            seats[i].isFree = false;
            seats[i].brickColorType = BrickColorType.Blue;
            */
        }

    }
    private Vector2 AverageSeatPosition(in SeatData[] seats, int startIdx, int endIdx)
    {
        Vector2 newPostion = Vector2.zero;
        for (int i = startIdx; i <= endIdx; i++)
        {
            newPostion += seats[i].position;
        }
        newPostion /= ((endIdx - startIdx) + 1);

        return newPostion;
    }

    private void ReleaseSeat(ISeatTicket ticket)
    {
        /*
        if(ticket.seatStartID == -1 )
            Debug.LogError("seatOrder" + ticket.seatOrder + "seatStartID" + ticket.seatStartID + "seatCount" + ticket.seatCount);
        else
            Debug.LogWarning("seatOrder" + ticket.seatOrder + "seatStartID" + ticket.seatStartID + "seatCount" + ticket.seatCount);
        */
        if (ticket.seatOrder == SeatOrder.Primary)
        {
            for (int i = ticket.seatStartID; i < ticket.seatStartID + ticket.seatCount; i++)
            {
                ReleaseSeat(ref primarySeatDatas[i]);
            }
            
        }
        else if(ticket.seatOrder == SeatOrder.Secondary)
        {
            for (int i = ticket.seatStartID; i < ticket.seatStartID + ticket.seatCount; i++)
            {
                ReleaseSeat(ref secondarySeatDatas[i]);
            }
        }

    }
    private void ReleaseSeat(ref SeatData seatData)
    {
        seatData.bID = -1;
        seatData.isFree = true;
        seatData.brickColorType = BrickColorType.None;
    }

    private SeatData FindSeat(int _bID, BrickColorType brickColor)
    {
        for (int i = 0; i < avaliableSpaceSizeP1; i++)
        {
            if (primarySeatDatas[i].bID == _bID && primarySeatDatas[i].brickColorType == brickColor)
            {
                return primarySeatDatas[i];
            }
        }
        for (int i = 0; i < avaliableSpaceSizeP2; i++)
        {
            if (secondarySeatDatas[i].bID == _bID && secondarySeatDatas[i].brickColorType == brickColor)
            {
                return secondarySeatDatas[i];
            }
        }
        Debug.Log("cannot find brick in avaliable seat");
        return nullSeat;
    }
    /// <summary>
    /// struct type will return copy.
    /// </summary>
    private SeatData FindNextFreeSeat(in SeatData[] seatDatas, int startIndex, int avaliableSpaceSize) 
    {
        
        if (seatDatas[startIndex].isFree) return seatDatas[startIndex];
        for (int i = startIndex + 1; i != startIndex; i = (i+1) % avaliableSpaceSize)
        {
            if (seatDatas[i].isFree) return seatDatas[i];
        }
        return nullSeat;
    }
    private SeatData FindNextFreeSeat(in SeatData[] seatDatas, int startIndex, int avaliableSpaceSize, string obstableLayer, string obstableName)
    {

        if (seatDatas[startIndex].isFree) return seatDatas[startIndex];
        for (int i = startIndex + 1; i != startIndex; i = (i + 1) % avaliableSpaceSize)
        {
            if (Helpers.DetectObstacle(seatDatas[i].position + Vector2.up * newLocalSize.y * 0.6f + Vector2.left * newLocalSize.x * 0.6f,
                            seatDatas[i].position + Vector2.down * newLocalSize.y * 0.6f + Vector2.right * newLocalSize.x * 0.6f
                            , obstableLayer, obstableName))
            {
                continue;
            }
            if (seatDatas[i].isFree) return seatDatas[i];
        }
        return nullSeat;
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
            case BrickColorType.Blue:
                temp = m_BlueBrickPool.GetNew();
                break;
            case BrickColorType.Green:
                temp = m_GreenBrickPool.GetNew();
                break;
            default:
                Debug.Log("BrickColorType not valid or = None!");
                break;
        }
        if( temp != null)
        {
            temp.transform.localScale = newLocalSize;
        }
        return temp;
    }

    void PlaceNewBrick(ref SeatData seatData, BrickColorType brickColorType)
    {
        Brick temp = GetNewBrick(brickColorType); //Call brick.OnEnable
        if(temp != null)
        {
            temp.transform.position = seatData.position;        
            seatData.bID = temp.poolID;
            seatData.brickColorType = brickColorType;
            seatData.isFree = false;
            temp.seatOrder = seatData.seatOrder;
            temp.seatCount = 1;
            temp.seatStartID = seatData.seatNumber;

            //do difference brick function check
            if (brickColorType == BrickColorType.Blue)
            {
                ConnectionCheckStepByStep(temp);
            }
            if (brickColorType == BrickColorType.Green)
            {
                temp.ChangeMigrateMultiplier(UnityEngine.Random.Range(2, 5));
            }
        }
        else
        {
            Debug.Log("BrickColorType " + brickColorType.ToString() + " not valid or pool empty!");
        }
    }

    void PlaceBrick(Brick brick, Vector3 position, ref SeatData seatData, BrickColorType brickColorType)
    {
        brick.transform.position = position;
        seatData.bID = brick.poolID;
        seatData.isFree = false;
        seatData.brickColorType = brickColorType;
        
    }

    private void PlaceHintBrick(in SeatData seatData, int instanceID)
    {
        hintBrickInstances[instanceID].transform.position = seatData.position;
        if(hintBrickInstances[instanceID].gameObject.activeSelf == false)
        {
            hintBrickInstances[instanceID].gameObject.SetActive(true);
        }
    }

    private void UpdateHintBrickColor(int instanceID)
    {
        Color nextColor = GetRandomColor();
        hintBrickInstances[instanceID].ChangeColor(nextColor);
    }

    private bool CheckLastBrick()
    {
        if (m_WhiteBrickPool.notFreeIdx.Count + m_RedBrickPool.notFreeIdx.Count + m_GreenBrickPool.notFreeIdx.Count + m_BlueBrickPool.notFreeIdx.Count <= 1)
        {
            return true;
        }
        else
        { 
            return false;
        }
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
        {
            if (greenBrickUseCount < 40)
                nextColor = Color.green;
            else
                nextColor = Color.red;
            greenBrickUseCount++;
        }
        return nextColor;
    }

    private Color ColorTypeToColor(BrickColorType brickColorType)
    {
        switch (brickColorType)
        {
            case BrickColorType.White:
                return Color.white;
            case BrickColorType.Red:
                return Color.red;
            case BrickColorType.Blue:
                return Color.blue;
            case BrickColorType.Green:
                return Color.green;
            case BrickColorType.None:
                return Color.gray;
            default:
                Debug.Log("Invalid color type" + brickColorType);
                return Color.black;
        }
    }

    private void PrintSeatDatas()
    {
        for (int i = 0; i < primarySeatDatas.Length; i++)
        {
            Debug.Log("primarySeatDatas[ + " + i + "].bID" + primarySeatDatas[i].bID + " brickColorType" + primarySeatDatas[i].brickColorType);
        }
        for (int i = 0; i < secondarySeatDatas.Length; i++)
        {
            Debug.Log("secondarySeatDatas[ + " + i + "].bID" + secondarySeatDatas[i].bID + " brickColorType" + secondarySeatDatas[i].brickColorType);
        }
    }

    private void OnDrawGizmos()
    {
        Color c;
        Vector3 scale = new Vector3(newLocalSize.x, newLocalSize.y, 0.1f);
        for (int i = 0; i < primarySeatDatas.Length; i++)
        {
            c = ColorTypeToColor(primarySeatDatas[i].brickColorType);
            c.a = 0.5f;
            Gizmos.color = c;
            Gizmos.DrawCube(primarySeatDatas[i].position, scale);
            if(primarySeatDatas[i].isFree)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(primarySeatDatas[i].position, 0.2f);
            }


        }
        for (int i = 0; i < secondarySeatDatas.Length; i++)
        {
            c = ColorTypeToColor(secondarySeatDatas[i].brickColorType);
            c.a = 0.5f;
            Gizmos.color = c;
            Gizmos.DrawCube(secondarySeatDatas[i].position, scale);
            if (secondarySeatDatas[i].isFree)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(secondarySeatDatas[i].position, 0.2f);
            }
        }
    }

}

public enum SeatOrder
{
    None,
    Primary,
    Secondary
}

public interface ISeatTicket
{
    public SeatOrder seatOrder { set; get; }
    public int seatStartID { set; get; }
    public int seatCount { set; get; }
}