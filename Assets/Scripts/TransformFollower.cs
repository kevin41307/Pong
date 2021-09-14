using UnityEngine;

public abstract class TransformFollower : MonoBehaviour
{
    public Transform target;
    public abstract void Follow();
}
