using UnityEngine;

public abstract class TransformFollower : MonoBehaviour
{
    public Transform target;
    public abstract void Follow();
}

public interface ITransformFollower
{
    public Transform target { set; get; }
    public Vector3 followOffset { set; get; }
    public bool pauseFollow { set; get; }
    public abstract void Follow();
}