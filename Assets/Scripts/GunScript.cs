using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{

    Camera mainCamera;
    public GameObject prefab;
    public float bulletForce = 1500f;
    public LineRenderer lineRenderer;
    public Transform target;
    public SpringJoint2D springJoint2D;
    private void Awake()
    {
        mainCamera = Camera.main;
        lineRenderer.enabled = false;
        springJoint2D.enabled = false;
    }


    private void Update()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
        transform.right = direction;
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if(target != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, target.transform.position);

        }
    }
    void Shoot()
    {
        GameObject bullet = Instantiate(prefab, null);
        bullet.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(transform.right));
        bullet.GetComponent<Rigidbody2D>().AddForce(bulletForce * transform.right);

    }

    public void TargetHit(Transform hit)
    {
        target = hit;
        lineRenderer.enabled = true;
        springJoint2D.enabled = true;
        springJoint2D.connectedBody = target.GetComponent<Rigidbody2D>();
    }    
}
