using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class UserInput : MonoBehaviour
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

    protected Vector2 m_Movement;
    protected Vector3 m_PointerWorldPosition;
    protected bool m_Jump;
    protected bool m_Fire1;
    protected bool m_Pause;
    PlayerInputActions inputActions;

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
        inputActions.Player.Fire1.canceled += Fire1_canceled;

        inputActions.Player.Move.performed += Move_performed;
        inputActions.Player.Move.canceled += Move_canceled;

        inputActions.Player.PrimaryFingerPosition.performed += PrimaryFingerPosition_performed;

        inputActions.Player.PrimaryTouchContact.started += TouchPress_started;
        inputActions.Player.PrimaryTouchContact.canceled += TouchPress_canceled;
        mainCamera = Camera.main;
    }
    private void PrimaryFingerPosition_performed(InputAction.CallbackContext obj)
    {     
        pointerVector = Helpers.ScreenToWorld2D(mainCamera, obj.ReadValue<Vector2>());
        m_PointerWorldPosition = pointerVector;
    }

    private void Fire1_started(InputAction.CallbackContext obj)
    {
        m_Fire1 = true;
    }

    private void Fire1_canceled(InputAction.CallbackContext obj)
    {
        m_Fire1 = false;
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
        Debug.Log("Touch started " + inputActions.Player.PrimaryFingerPosition.ReadValue<Vector2>());
    }

    private void TouchPress_canceled(InputAction.CallbackContext context)
    {
        if (OnEndTouch != null) OnEndTouch(Helpers.ScreenToWorld2D(mainCamera, inputActions.Player.PrimaryFingerPosition.ReadValue<Vector2>()), (float)context.startTime);
        Debug.Log("Touch ended ");
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



    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Y))
        {
            UseInputSystem = !UseInputSystem;
        }
#endif
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
