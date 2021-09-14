using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerControl : MonoBehaviour
{

    Camera mainCamera;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.right = ((Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition) - new Vector2( transform.position.x, transform.position.y)).normalized ;
    }
}
