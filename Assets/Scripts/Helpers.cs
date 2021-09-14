using UnityEngine;

public class Helpers : MonoBehaviour
{
    public static Vector3 ScreenToWorld2D(Camera camera , Vector3 position)
    {
        position.z = camera.nearClipPlane;
        return camera.ScreenToWorldPoint(position);
    }

    public static float AngleFromDir(Vector2 direction)
    {
        direction.Normalize();
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
