using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;

public class FieldOfView : MonoBehaviour
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
    List<Vector3> viewPoints = new List<Vector3>();
    RaycastHit[] hits = new RaycastHit[1];
    Collider[] targetInViewRadius = new Collider[10];
    public MeshRenderer mr;

    private Vector3 targetRotation;

    PlayerControll player;

    private void Awake()
    {
        player = GetComponentInParent<PlayerControll>();
    }

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        //StartCoroutine("FindTargetWithDelay", .2f);
        StartCoroutine("TickRender");
        
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
        if (!player.IsChargeBtn) return;

        UpdateOrientation();
        
    }

    private void LateUpdate()
    {
        if (!player.IsChargeBtn) return;
        DrawFieldOfView();
    }

    void UpdateOrientation()
    {
        visibleTargets.Clear();
        if(Physics.OverlapSphereNonAlloc(transform.position, viewRadius, targetInViewRadius, targetMask, QueryTriggerInteraction.Ignore) > 0)
        {
            for (int i = 0; i < targetInViewRadius.Length; i++)
            {
                if (targetInViewRadius[i] == null) return;
                Vector3 dirToTarget = (targetInViewRadius[i].transform.position - transform.position).normalized;
                targetRotation.Set(transform.rotation.x, transform.rotation.y, Helpers.AngleFromDir(dirToTarget));
                transform.eulerAngles = targetRotation;
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
            float angleZ = Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetAngle, 5*Time.deltaTime);
            transform.rotation = Quaternion.Euler( new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angleZ)); 
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
    
    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        //List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        viewPoints.Clear();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle + 90, true) * viewRadius, Color.red);
            ViewCastInfo newViewCast = ViewCast(angle);
            
            if( i > 0 )
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;

                if(oldViewCast.hit != newViewCast.hit ||( oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                    //Debug.Log(oldViewCast.angle + "," + newViewCast.angle);
                }
            }
            
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }


        //Generate mesh
        int vertexCount = viewPoints.Count + 1 ;
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0, 0);
        float u;
        float v;
        for (int i = 0; i < vertexCount -1; i++)
        {
            vertices[i+1] = transform.InverseTransformPoint(viewPoints[i]);
            u = Vector3.Distance(viewPoints[i], transform.position) / viewRadius;
            //float v = Mathf.Sin(stepAngleSize * Mathf.Deg2Rad);
            v = (float) i / (viewPoints.Count-1);
            uvs[i + 1].Set(u, u * v);
            //Debug.Log("v" + v);
            //uvs[i + 1] = new Vector2(u, v);
            //Debug.Log("u" + u);
            /*
            if( i % 2 == 0)
                uvs[i + 1].Set(u, v * u);
            else
                uvs[i + 1].Set(u, 0);
            */
            //Debug.Log(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.uv = uvs;
        viewMesh.RecalculateNormals();
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
        Vector3 dir = DirFromAngle(globalAngle , true);

        //if(Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        int count = Physics.RaycastNonAlloc(transform.position, dir, hits, viewRadius, obstacleMask, QueryTriggerInteraction.Collide);
        if ( count > 0)
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
        angleInDegrees += (90 - 2*transform.eulerAngles.z);
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

}
