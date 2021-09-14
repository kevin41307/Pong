using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformDampFollower : TransformFollower
{

    private Vector3 currentVelocity;
    public float smoothTime = 1;
    public float maxSpeed = 160f;
    public float dampTime = .02f;
    private Vector3 lastPosition;
    private WaitForSeconds waitForSeconds;
    Coroutine updateLastPositionCoroutine;
    private void OnEnable()
    {
        waitForSeconds = new WaitForSeconds(dampTime);
        updateLastPositionCoroutine = StartCoroutine(UpdateLastPosition());
    }

    private void OnDisable()
    {
        if (updateLastPositionCoroutine != null) StopCoroutine(updateLastPositionCoroutine);
    }
    public override void Follow()
    {
        transform.position = Vector3.SmoothDamp(transform.position, lastPosition, ref currentVelocity, smoothTime * Time.deltaTime, maxSpeed);
    }

    private void FixedUpdate()
    {
        Follow();
    }

    IEnumerator UpdateLastPosition()
    {
        while (true)
        {
            lastPosition = target.position;
            yield return waitForSeconds;

        }

    }

}
