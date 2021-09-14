using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class FieldOfViewJobVersion : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public float meshResolution;
    public MeshFilter viewMeshFilter;
    public int edgeResolveIterations;
    public float edgeDstThreshold;
    Mesh viewMesh;
    RaycastHit[] hits = new RaycastHit[1];
    Collider[] targetInViewRadius = new Collider[10];
    public MeshRenderer mr;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        //StartCoroutine("FindTargetWithDelay", .2f);
        //StartCoroutine("TickRender");
    }

    IEnumerator TickRender()
    {
        float m_Time = 0f;
        while (true)
        {
            m_Time += Time.fixedDeltaTime * Time.deltaTime * Time.timeScale;
            mr.material.SetFloat("_Tick", m_Time);
            yield return new WaitForFixedUpdate();
        }

    }


    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();

        }
    }
    private void Update()
    {
        //UpdateOrientation();

    }

    private void LateUpdate()
    {
       DrawFieldOfView();
    }



    void UpdateOrientation()
    {
        visibleTargets.Clear();
        if (Physics.OverlapSphereNonAlloc(transform.position, viewRadius, targetInViewRadius, targetMask, QueryTriggerInteraction.Ignore) > 0)
        {
            for (int i = 0; i < targetInViewRadius.Length; i++)
            {
                if (targetInViewRadius[i] == null) return;
                Transform target = targetInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                float targetAngle = Vector3.SignedAngle(transform.right, dirToTarget, Vector3.forward);
                //if (Mathf.Abs(targetAngle) < .5f) return;
                float angleZ = Mathf.Lerp(0, targetAngle, 5 * Time.deltaTime);
                //Debug.Log("targetAngle" + targetAngle);
                //Debug.Log("angleZ" + angleZ);
                //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, targetAngle);
                //transform.localRotation *= Quaternion.AngleAxis(angleZ * Time.deltaTime, Vector3.forward);
                transform.eulerAngles += new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angleZ);
            }
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform target = targetInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float targetAngle = Vector3.SignedAngle(transform.right, dirToTarget, transform.forward);
            float angleZ = Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetAngle, 2 * Time.deltaTime);
            transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angleZ));
            Debug.Log(targetAngle);
            if (Vector3.SignedAngle(transform.right, dirToTarget, transform.forward) < viewAngle / 2)//Vector3.Angle(transform.forward...
            {
            }
        }
        /*
        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform target = targetInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            //Debug.Log(targetInViewRadius[i].name);

            if (Vector3.Angle(transform.right, dirToTarget) < viewAngle / 2)//Vector3.Angle(transform.forward...
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                visibleTargets.Add(target);

                if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))//not
                {
                    visibleTargets.Add(target);
                }

            }
        }
        */
    }
    //List<Vector3> viewPoints = new List<Vector3>();

    [ReadOnly] NativeArray<RaycastCommand> commands;
    [ReadOnly] NativeArray<RaycastHit> results;
    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        commands = new NativeArray<RaycastCommand>(stepCount, Allocator.TempJob);
        results = new NativeArray<RaycastHit>(stepCount, Allocator.TempJob);


        for (int i = 0; i < commands.Length; i++)
        {
            float angle = transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            commands[i] = new RaycastCommand(transform.position, DirFromAngle(angle, true), viewRadius, obstacleMask, 1);

        }
        JobHandle jobHandle = RaycastCommand.ScheduleBatch(commands, results, 128);
        jobHandle.Complete();
        
        for (int i = 0; i < results.Length; i++)
        {


            Debug.DrawLine(transform.position, results[i].point, Color.red) ;

        }

        commands.Dispose();
        results.Dispose();
        


    }




    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);

        //if(Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        int count = Physics.RaycastNonAlloc(transform.position, dir, hits, viewRadius, obstacleMask, QueryTriggerInteraction.Ignore);
        if (count > 0)
        {
            return new ViewCastInfo(true, hits[0].point, hits[0].distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        angleInDegrees += (90 - 2 * transform.eulerAngles.z);
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;

        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }


    public struct MyRaycastInfo
    {
        public float3 origin;
        public float3 direction;
        public RaycastHit[] hits;
        public float viewRadius;
        public int mask;
        public float globalAngle;
    }


    public struct RaycastJob : IJobParallelFor
    {
        [ReadOnly] public  NativeArray<MyRaycastInfo> info;
        public NativeArray<ViewCastInfo> castResult;

        public void Execute(int index)
        {
            if (Physics.RaycastNonAlloc(info[index].origin, info[index].direction, info[index].hits, info[index].viewRadius, info[index].mask, QueryTriggerInteraction.Ignore) > 0)
            {
                castResult[index] = new ViewCastInfo(true, info[index].hits[0].point, info[index].hits[0].distance, info[index].globalAngle);
            }
            else
            {

                castResult[index] = new ViewCastInfo(false, info[index].origin + info[index].direction * info[index].viewRadius, info[index].viewRadius, info[index].globalAngle);
            }
        }
    }

}
