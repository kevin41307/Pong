using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{

    public Transform backWallTop;
    public Transform backWallBottom;

    MeshRenderer topMeshRender;
    MeshRenderer bottomMeshRender;
    public MeshRenderer fluidWallTopMeshRender;
    public MeshRenderer fluidWallBottomMeshRender;

    Material topMat;
    Material bottomMat;
    Material fluidWallTopMat;
    Material fluidWallBottomMat;

    readonly int hashVisible = Shader.PropertyToID("Visible_");
    readonly int hashSpeed = Shader.PropertyToID("_Speed");
    float speed = 0.25f;

    Coroutine changedFlowCoroutine;
    Vector4 topTargetVec;
    Vector4 bottomTargetVec;
    int iteration = 16;


    private void Awake()
    {
        topMeshRender = backWallTop.GetComponent<MeshRenderer>();
        topMat = topMeshRender.material;
        bottomMeshRender = backWallBottom.GetComponent<MeshRenderer>();
        bottomMat = bottomMeshRender.material;

        fluidWallBottomMat = fluidWallBottomMeshRender.material;
        fluidWallTopMat = fluidWallTopMeshRender.material;

        //MyGameEventSystem.Instance.OnUseGravityChangedItem += ChangeFlow;
        MyGameEventSystem.UseGravityChangedItemEvent.performed += ChangeFlow;
        MyGameEventSystem.UseGravityChangedItemEvent.canceled += StopChangeFLow;
        MyGameEventSystem.UsePortalItemEvent.performed += OpenFluidField;
        MyGameEventSystem.UsePortalItemEvent.canceled += CloseFluidField;
    }

    private void StopChangeFLow()
    {
        if (changedFlowCoroutine != null)
        {
            StopCoroutine(changedFlowCoroutine);
            changedFlowCoroutine = null;
            SetFlow(Vector4.zero, Vector4.zero);
        }
    }

    private void CloseFluidField()
    {
        fluidWallTopMat.SetFloat(hashVisible, 0);
        fluidWallBottomMat.SetFloat(hashVisible, 0);
    }

    private void OpenFluidField(GameObject obj)
    {
        fluidWallTopMat.SetFloat("Visible_", 1);
        fluidWallBottomMat.SetFloat("Visible_", 1);
    }


    private void Start()
    {
        
    }

    private void ChangeFlow(Vector4 direction, float duration)
    {
        if (changedFlowCoroutine != null)
        {
            StopCoroutine(changedFlowCoroutine);
        }

        if (direction.x > 0)
        {
            changedFlowCoroutine = StartCoroutine(StartChangedFlow(RightFlow, HoleFLow, duration));
        }
        else if(direction.x < 0 )
        {
            changedFlowCoroutine = StartCoroutine(StartChangedFlow(LeftFlow, HoleFLow, duration));
        }

        if(direction.w > 0 ) // TODO: W component control clockwise and couter clockwise
        {
            changedFlowCoroutine = StartCoroutine(StartChangedFlow(ClockwiseFlow, HoleFLow, duration));
        } 
        else if(direction.w < 0)
        {
            changedFlowCoroutine = StartCoroutine(StartChangedFlow(CounterClockwiseFlow, HoleFLow, duration));
        }

    }

    // Start is called before the first frame update

    public void ClockwiseFlow()
    {
        topTargetVec = Vector2.right * speed;
        bottomTargetVec = Vector2.left * speed;
    }

    public void CounterClockwiseFlow()
    {
        topTargetVec = Vector2.left * speed;
        bottomTargetVec = Vector2.right * speed;
    }

    public void RightFlow()
    {
        topTargetVec = Vector2.right * speed;
        bottomTargetVec = Vector2.right * speed;
    }
    public void LeftFlow()
    {
        topTargetVec = Vector2.left * speed;
        bottomTargetVec = Vector2.left * speed;
    }

    public void HoleFLow()
    {
        topTargetVec = Vector2.down * speed;
        bottomTargetVec = Vector2.down * speed;
    }

    public void SetFlow(Vector4 top, Vector4 bottom)
    {
        topMat.SetVector(hashSpeed, top);
        bottomMat.SetVector(hashSpeed, bottom);
    }


    IEnumerator StartChangedFlow(System.Action from, System.Action to, float duration)
    {
        from();
        Vector4 topCurrent = topMat.GetVector(hashSpeed);
        Vector4 bottomCurrent = bottomMat.GetVector(hashSpeed);
        Vector4 topDivided = (topTargetVec - topCurrent) / iteration;
        Vector4 bottomDivided = (bottomTargetVec - bottomCurrent) / iteration;

        for (int i = 1; i <= iteration; i++)
        {
            SetFlow(topCurrent + i * topDivided, bottomCurrent + i * bottomDivided);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(duration - iteration * Time.fixedDeltaTime);
        to();
        topCurrent = topMat.GetVector(hashSpeed);
        bottomCurrent = bottomMat.GetVector(hashSpeed);
        topDivided = (topTargetVec - topCurrent) / iteration;
        bottomDivided = (bottomTargetVec - bottomCurrent) / iteration;

        for (int i = 1; i <= iteration; i++)
        {
            SetFlow(topCurrent + i * topDivided, bottomCurrent + i * bottomDivided);
            yield return new WaitForFixedUpdate();
        }
        changedFlowCoroutine = null;
    }
    
}
