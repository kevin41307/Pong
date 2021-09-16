using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityRing : MonoBehaviour, IPooled<GravityRing>
{
    public int poolID { get; set; }
    public ObjectPooler<GravityRing> pool { get; set; }

    public virtual void Start()
    {
        if (UserInput.Instance.inputActions != null)
            UserInput.Instance.inputActions.Player.Fire1.canceled += FreeItself;
    }

    public virtual void OnDestroy()
    {
        if (UserInput.Instance.inputActions != null)
            UserInput.Instance.inputActions.Player.Fire1.canceled -= FreeItself;
    }

    private void FreeItself(InputAction.CallbackContext obj)
    {
        transform.SetParent(null, true);
        pool.Free(this);
    }

}
