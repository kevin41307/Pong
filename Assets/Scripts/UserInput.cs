using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-10)]
public class UserInput : MonoBehaviourSingleton<UserInput>
{
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;
    public delegate void EndTouch(Vector2 position, float time);
    public event EndTouch OnEndTouch;

    Camera mainCamera;
    public Transform cube;
    bool isInputBlocked = false;
    [HideInInspector]
    public bool playerControllerInputBlocked;
    protected bool m_ExternalInputBlocked;

    private Vector2 inputVector;
    private Vector2 pointerVector;
    private Vector2 secondPointVector;

    protected Vector2 m_Movement;
    protected Vector2 m_PointerWorldPosition;
    protected Vector2 m_SecondPointerWorldPosition;

    protected bool m_PrimaryFinger;
    protected bool m_SecondFinger;
    protected bool m_Jump;
    protected bool m_Fire1;
    protected bool m_Pause;
    [HideInInspector]
    public PlayerInputActions inputActions;

    [SerializeField]
    private bool m_useInputSystem;
    public bool UseInputSystem
    {
        get { return m_useInputSystem; }
        set
        {
            m_useInputSystem = value;
            EnableDisableInputSystem(m_useInputSystem);
        }
    }  

    public Vector2 MoveInput
    {
        get
        {
            if (playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Movement;
        }
    }

    public Vector3 PositionInput
    {
        get
        {
            if (playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_PointerWorldPosition;
        }
    }
    public Vector3 SecondPositionInput
    {
        get
        {
            if (playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_SecondPointerWorldPosition;
        }
    }


    public bool JumpInput
    {
        get { return m_Jump && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool Fire1Input
    {
        get { return m_Fire1 && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }


    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Jump.started += Jump_started;
        inputActions.Player.Jump.canceled += Jump_canceled;

        inputActions.Player.Fire1.started += Fire1_started;
        //inputActions.Player.Fire1.performed += Fire1_performed;

        inputActions.Player.Fire1.canceled += Fire1_canceled;

        inputActions.Player.Move.performed += Move_performed;
        inputActions.Player.Move.canceled += Move_canceled;

        inputActions.Player.PrimaryFingerPosition.performed += PrimaryFingerPosition_performed;
        inputActions.Player.SencondaryFingerPosition.performed += SencondaryFingerPosition_performed;
        //inputActions.Player.PrimaryTouchContact.started += TouchPress_started;
        //inputActions.Player.PrimaryTouchContact.canceled += TouchPress_canceled;
        inputActions.Player.SencondaryTouchContact.started += SencondaryTouchContact_started;
        inputActions.Player.SencondaryTouchContact.canceled += SencondaryTouchContact_canceled;
        inputActions.Player.PrimaryTouchContact.started += PrimaryTouchContact_started;
        inputActions.Player.PrimaryTouchContact.canceled += PrimaryTouchContact_canceled;


        mainCamera = Camera.main;
    }


    private void PrimaryTouchContact_started(InputAction.CallbackContext obj)
    {
        m_PrimaryFinger = true;
    }
    private void PrimaryTouchContact_canceled(InputAction.CallbackContext obj)
    {
        m_PrimaryFinger = false;
    }
    private void SencondaryTouchContact_started(InputAction.CallbackContext obj)
    {
        m_SecondFinger = true;
    }
    private void SencondaryTouchContact_canceled(InputAction.CallbackContext obj)
    {
        m_SecondFinger = false;
    }
    private void PrimaryFingerPosition_performed(InputAction.CallbackContext obj)
    {
        pointerVector = Helpers.ScreenToWorld2D(mainCamera, obj.ReadValue<Vector2>());
        if (pointerVector.x > 0) // check first finger position, if it right side dispose it //TODO : try to detect rectransform
        {
            pointerVector = Vector2.zero;
        }
        m_PointerWorldPosition = pointerVector;
        #region can run but stupid code
        /*
        pointerVector = Helpers.ScreenToWorld2D(mainCamera, obj.ReadValue<Vector2>());

        if (m_SecondFinger == false) // if only one finger and this is primary finger, check 
        {
            if (pointerVector.x > 0) // check first finger position, if it right side dispose it //TODO : try to detect rectransform
            {
                pointerVector = Vector2.zero;
            }
        }

        m_PointerWorldPosition = pointerVector;
        */
        #endregion
    }
    private void SencondaryFingerPosition_performed(InputAction.CallbackContext obj)
    {
        pointerVector = Helpers.ScreenToWorld2D(mainCamera, obj.ReadValue<Vector2>());
        if (pointerVector.x > 0)
        {
            pointerVector = Vector2.zero;
        }
        m_PointerWorldPosition = pointerVector;

        #region can run but stupid code
        /*
        if (m_PrimaryFinger == true) 
        {
            pointerVector = Helpers.ScreenToWorld2D(mainCamera, inputActions.Player.SencondaryFingerPosition.ReadValue<Vector2>());
            if (pointerVector.x > 0)
            {
                pointerVector = Vector2.zero;
            }
            m_PointerWorldPosition = pointerVector;
        }
        else if (m_PrimaryFinger == false) // if primary finger is leave
        {
            pointerVector = Helpers.ScreenToWorld2D(mainCamera, inputActions.Player.SencondaryFingerPosition.ReadValue<Vector2>());
            if (pointerVector.x > 0)
            {
                pointerVector = Vector2.zero;
            }

            m_PointerWorldPosition = pointerVector;
        }

        */
        #endregion
    }
    private void Fire1_started(InputAction.CallbackContext obj)
    {
        m_Fire1 = true;
        //Debug.Log("Fire1_started");
    }
    private void Fire1_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Fire1_performed");
    }
    private void Fire1_canceled(InputAction.CallbackContext obj)
    {
        m_Fire1 = false;
        //Debug.Log("Fire1_canceled");
    }


    private void Move_performed(InputAction.CallbackContext context)
    {

        //Debug.Log("Move_performed");
        //Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();
        inputVector.Set(inputActions.Player.Move.ReadValue<Vector2>().x, inputActions.Player.Move.ReadValue<Vector2>().y);
        m_Movement.Set(inputVector.x, inputVector.y);

    }
    private void Move_canceled(InputAction.CallbackContext context)
    {
        //Debug.Log("Move_canceled");
        //Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();
        inputVector.Set(inputActions.Player.Move.ReadValue<Vector2>().x, inputActions.Player.Move.ReadValue<Vector2>().y);
        m_Movement.Set(inputVector.x, inputVector.y);
    }

    private void Jump_started(InputAction.CallbackContext context)
    {
        m_Jump = true;
    }
    private void Jump_canceled(InputAction.CallbackContext context)
    {
        m_Jump = false;
    }

    private void TouchPress_started(InputAction.CallbackContext context)
    {
        OnStartTouch?.Invoke(Helpers.ScreenToWorld2D(mainCamera, inputActions.Player.PrimaryFingerPosition.ReadValue<Vector2>()), (float)context.startTime);
        //Debug.Log("Touch started " + inputActions.Player.PrimaryFingerPosition.ReadValue<Vector2>());
    }

    private void TouchPress_canceled(InputAction.CallbackContext context)
    {
        if (OnEndTouch != null) OnEndTouch(Helpers.ScreenToWorld2D(mainCamera, inputActions.Player.PrimaryFingerPosition.ReadValue<Vector2>()), (float)context.startTime);
        //Debug.Log("Touch ended ");
    }

    public Vector2 PrimaryPosition()
    {
        return Helpers.ScreenToWorld2D(mainCamera, inputActions.Player.PrimaryFingerPosition.ReadValue<Vector2>());
    }
    private void Start()
    {
        UseInputSystem = true;
    }

    private void OnEnable()
    {
        EnableDisableInputSystem(true);
    }

    private void OnDisable()
    {
        EnableDisableInputSystem(false);
    }

    /*
    private void FixedUpdate()
    {
        if (MobileDebug.Instance != null)
            MobileDebug.Instance.ShowMessage("1.pointerVector:" + pointerVector + " 2.m_PrimaryFinger:" + m_PrimaryFinger + " 3.m_SecondFinger:" + m_SecondFinger + " 5.m_PointerWorldPosition:" + m_PointerWorldPosition);
    }
    */
    void Update()
    {
        if(UseInputSystem)
        {
            //Debug.Log(m_Jump);
            //Debug.Log(m_Movement);
            
        }
        else
        {
            m_Movement.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            m_Jump = Input.GetButton("Jump");
            m_Fire1 = Input.GetButton("Fire1");
            //m_Pause = Input.GetButtonDown("Pause");
        }
    }

    void EnableDisableInputSystem(bool active)
    {
        if (active)
        {
            inputActions?.Player.Enable();
        }
        else
            inputActions?.Player.Disable();
    }
    public bool HaveControl()
    {
        return !m_ExternalInputBlocked;
    }

    public void ReleaseControl()
    {
        m_ExternalInputBlocked = true;
    }

    public void GainControl()
    {
        m_ExternalInputBlocked = false;
    }

    public PlayerInputActions GetPlayerInputActions()
    {
        return inputActions;
    }
}
