using UnityEngine;
using System.Collections.Generic;
using System;

public class Helpers : MonoBehaviour
{
    public static Vector3 ScreenToWorld2D(Camera camera, Vector3 position)
    {
        position.z = camera.nearClipPlane;
        return camera.ScreenToWorldPoint(position);
    }

    public static float AngleFromDir(Vector2 direction)
    {
        direction.Normalize();
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    public static Vector2 DirFromAngle(float angle)
    {
        Vector2 vec2 = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
        vec2.Normalize();
        return vec2;
    }

    public static bool IsRightDirection(Vector2 direction)
    {
        return Vector2.Dot(Vector2.right, direction.normalized) > 0.01f;
    }
    public static bool IsCloseDirection(Vector2 d1, Vector2 d2)
    {
        return Vector2.Dot(d1.normalized, d2.normalized) > 0.998f;
    }

    static Collider2D[] results = new Collider2D[8];
    public static bool DetectObstacle(Vector2 pointA, Vector2 pointB, string obstacleLayer, string obstacleName)
    {
        Array.Clear(results, 0, results.Length);
        int hits = Physics2D.OverlapAreaNonAlloc(pointA, pointB, results, 1 << LayerMask.NameToLayer(obstacleLayer));
        if (hits > 0)
        {
            for (int i = 0; i < hits; i++)
            {
                if (results[i].name.Contains(obstacleName))
                {
                    return true;
                }

            }
            return false;
        }
        else
            return false;
    }
}

public class Helpers<T> where T : MonoBehaviour
{
    public static void DebugInOneLine(List<T> newList)
    {
        string str = string.Empty;
        foreach (var e in newList)
        {
            str += e.name + " ";

        }
        Debug.Log(str);
    }
}
