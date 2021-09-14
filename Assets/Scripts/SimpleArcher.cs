using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleArcher : SimplePlayerControl
{

    public GameObject prefab_Arrow;
    public GameObject prefab_Dot;

    public float shootForce = 100f;

    public int pathIterationTimes;
    Vector2[] pos;
    Transform[] dotPool;

    protected override void Start()
    {
        base.Start();
        CreateDotPool();
    }



    protected override void Update()
    {
        base.Update();
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        DrawPredictPath();
    }

    void CreateDotPool()
    {
        pos = new Vector2[pathIterationTimes];
        dotPool = new Transform[pathIterationTimes];
        for (int i = 0; i < dotPool.Length; i++)
        {
            dotPool[i] = Instantiate(prefab_Dot, null).transform;
            dotPool[i].SetPositionAndRotation(new Vector3(20, 20, 0), Quaternion.identity);
        }
    }

    void Shoot()
    {
        Rigidbody2D arrowRb = Instantiate(prefab_Arrow).GetComponent<Rigidbody2D>();
        arrowRb.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        
        arrowRb.velocity = (transform.right * shootForce);
    }

    void DrawPredictPath()
    {
        for (int i = 0; i < dotPool.Length; i++)
        {
            float t = (float)i/ dotPool.Length;
            dotPool[i].position = (Vector2)transform.position + (Vector2)transform.right * shootForce * t + 0.5f * Physics2D.gravity * t * t;
        }

    }


}
