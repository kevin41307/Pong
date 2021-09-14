using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(LineRenderer))]
public class Sniper : MonoBehaviour
{
    public static event System.Action OnAimed;

    //Components
    UserInput userInput;
    PlayerInputActions inputActions;
    PlayerControll player; //TODO: Add Component and init it
    public LineRenderer lineRenderer;
    Ball m_Bullet;

    //Parameter
    private float minimumDistance = .1f;
    private float maxLineLength = 3f;
    private float maximumTime = 3f;
    private float sinceAimedTime = 0f;
    public GameObject sniperVolume;
    [HideInInspector]
    public float shootVelocity= 50f;

    public Vector2 StartPosition { private set; get; }
    private Vector2 endPosition;
    private Coroutine aimCoroutine;
    private Camera mainCamera;

    private Vector2 m_direction;
    public Vector2 Direction
    {
        private set => m_direction = value;
        get {
            if (IsAiming)
                return m_direction;
            else
            {
#if UNITY_EDITOR
                Debug.Log("Not Aiming!");
#endif
                return Vector2.zero;
            }
        }
    }

    public bool IsAiming { private set; get; }
    

    private void Awake()
    {
        userInput = GetComponent<UserInput>();
        mainCamera = Camera.main;
        player = GetComponent<PlayerControll>();

    }

    private void OnEnable()
    {
        lineRenderer.enabled = false;
        IsAiming = false;
        lineRenderer.positionCount = 2;
        if( inputActions == null)
            inputActions = userInput.GetPlayerInputActions();
        inputActions.Player.Fire1.started += Fire1_started;
        inputActions.Player.Fire1.canceled += Fire1_canceled;
        /*
        userInput.OnStartTouch += DrawABow;
        userInput.OnEndTouch += ShootArrow;
        */
    }

    private void OnDisable()
    {
        /*
        userInput.OnStartTouch -= DrawABow;
        userInput.OnEndTouch -= ShootArrow;
        */
        inputActions.Player.Fire1.started -= Fire1_started;
        inputActions.Player.Fire1.started -= Fire1_canceled;

    }
    private void Start()
    {

        /*
        inputActions.Player.Fire1.started += _ => DrawABow();
        inputActions.Player.Fire1.canceled += _ => ShootArrow();
        inputActions.Player.Fire1.started += _ => { player.Sniper_Start(); };
        inputActions.Player.Fire1.canceled += _ => { player.Sniper_End(); };
        */
    }

    private void Fire1_canceled(InputAction.CallbackContext obj)
    {
        ShootArrow();
        player.Sniper_End();
    }

    private void Fire1_started(InputAction.CallbackContext obj)
    {
        DrawABow();
        player.Sniper_Start();
    }

    public void DrawABow()
    {
        Ray ray = mainCamera.ScreenPointToRay(inputActions.Player.PrimaryFingerPosition.ReadValue<Vector2>());
        RaycastHit raycastHit;
        if (!Physics.Raycast(ray, out raycastHit, 100f, 1 << LayerMask.NameToLayer("Collider"), QueryTriggerInteraction.Collide)) return; // if player dont click obj, do nothing
        if (raycastHit.collider != null && player.ClippedMoveableArea.IsContain(ray.origin))
        {
            if(raycastHit.collider.CompareTag("ball"))
            {
                m_Bullet = raycastHit.collider.GetComponentInParent<Ball>();
                m_Bullet.RigidBody2D.simulated = false;
                m_Bullet.transform.position = raycastHit.point;
                m_Bullet.RigidBody2D.velocity = Vector2.zero;
                m_Bullet.RigidBody2D.angularVelocity = 0f;

                aimCoroutine = StartCoroutine(StartAiming());
            }
        }

    }

    public void ShootArrow()
    {
        Time.timeScale = 1f;
        sniperVolume.SetActive(false);
        if (aimCoroutine != null)
            StopCoroutine(aimCoroutine);

        if(m_Bullet != null)
        {
            m_Bullet.RigidBody2D.simulated = true;
            m_Bullet.RigidBody2D.velocity = Vector2.zero;
            m_Bullet.RigidBody2D.angularVelocity = 0f;
            m_Bullet.RigidBody2D.velocity = m_Bullet.MaxBallVelocity * Direction.normalized * Mathf.Clamp(Direction.magnitude, 0f, maxLineLength) * 10f;
            m_Bullet.PlayImpactAnimation();
        }

        if(lineRenderer == true)
        {
            lineRenderer.SetPosition(0, Vector2.zero);
            lineRenderer.SetPosition(1, Vector2.zero);
            lineRenderer.enabled = false;
        }
        IsAiming = false;
        sinceAimedTime = 0f;
        Direction = Vector2.zero;
        m_Bullet = null;

    }
    IEnumerator StartAiming()
    {
        lineRenderer.enabled = true;
        IsAiming = true;
        sniperVolume.SetActive(true);
        Time.timeScale = 0.33f;
        StartPosition = userInput.PositionInput;
        lineRenderer.SetPosition(0, StartPosition);
        while (true)
        {
            endPosition = userInput.PositionInput;
            //Debug.DrawLine(startPosition, secondPosition);
            Direction = StartPosition - endPosition;
            lineRenderer.SetPosition(1, StartPosition + Direction.normalized * Mathf.Clamp(Direction.magnitude, 0f, maxLineLength));
            OnAimed?.Invoke();
            if(sinceAimedTime > maximumTime)
            {
                ShootArrow();
                break;
            }
            sinceAimedTime += Time.unscaledDeltaTime;
            yield return null;
        }

    }

}
