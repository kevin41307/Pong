using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PinchDetection : MonoBehaviour
{
    private UserInput userInput;
    private PlayerInputActions inputActions;
    private Coroutine zoomCoroutine;
    private Transform cameraTransform;
    [SerializeField]
    private float cameraSpeed = 4f;

    private void Awake()
    {
        userInput = GetComponent<UserInput>();
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        inputActions = userInput.GetPlayerInputActions();
        inputActions.Player.SencondaryTouchContact.started += _ => ZoomStart();
        inputActions.Player.SencondaryTouchContact.canceled += _ => ZoomEnd();
    }
    private void ZoomStart()
    {
        zoomCoroutine = StartCoroutine(ZoomDetection());
    }
    private void ZoomEnd()
    {
        StopCoroutine(zoomCoroutine);
    }

    IEnumerator ZoomDetection()
    {
        float previousDistance = 0f, distance = 0f;
        while (true)
        {
            distance = Vector2.Distance(inputActions.Player.PrimaryFingerPosition.ReadValue<Vector2>(), inputActions.Player.SencondaryFingerPosition.ReadValue<Vector2>());
            Debug.Log(inputActions.Player.SencondaryFingerPosition.ReadValue<Vector2>());
            //Detection
            //Zoom Out
            //TODO: threshold, accuracy
            if( distance > previousDistance )
            {
                Vector3 targetPosition = cameraTransform.position;
                targetPosition.z -= 1;
                cameraTransform.position = Vector3.Slerp(cameraTransform.position, targetPosition, Time.deltaTime * cameraSpeed);

            }
            else if( distance < previousDistance)
            {
                Vector3 targetPosition = cameraTransform.position;
                targetPosition.z += 1;
                cameraTransform.position = Vector3.Slerp(cameraTransform.position, targetPosition, Time.deltaTime * cameraSpeed);
            }
            // keep track of previous distance for next loop
            previousDistance = distance;
            yield return null;
        }
    }

}
