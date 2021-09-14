using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics2DHelper : MonoBehaviour
{
    public static bool Raycast(Vector2 orgin, Vector2 direction, float distance, int impactLayer)
    {
        if(Physics2D.Raycast(orgin, direction, distance, impactLayer))
        {
#if UNITY_EDITOR
            Debug.DrawLine(orgin, orgin + direction * distance, Color.green);
#endif
            return true;
        }
        else
        {
#if UNITY_EDITOR
            Debug.DrawLine(orgin, orgin + direction * distance, Color.red);
#endif
            return false;
        }
    }
    public static Vector2 EasyReachPoint(Vector2 start, Vector2 end, float offset, int impactLayer)
    {
        end.x = start.x;
        Vector2 predictDirection = end - start;
        float predictLength = predictDirection.magnitude;

        RaycastHit2D raycastHit2D = Physics2D.Raycast(start, predictDirection, predictLength + offset, impactLayer);
        if (predictLength >= raycastHit2D.distance)
        {
#if UNITY_EDITOR
            Debug.DrawLine(start, start + predictDirection.normalized * (predictLength + offset), Color.green);
#endif
            return end;
        }
        else
        {
#if UNITY_EDITOR
            Debug.DrawLine(start, start + predictDirection.normalized * (predictLength + offset), Color.red);
#endif
            return raycastHit2D.point - predictDirection.normalized * offset;
        }
    }




    public static bool EasySweepTest(Vector2 start, Vector2 end, float offset, int impactLayer)
    {
        Vector2 predictDirection = end - start;
        float predictLength = predictDirection.magnitude;

        RaycastHit2D raycastHit2D = Physics2D.Raycast(start, predictDirection, predictLength + offset, impactLayer);
        if(predictLength >= raycastHit2D.distance)
        {
#if UNITY_EDITOR
            Debug.DrawLine(start, start + predictDirection.normalized * (predictLength + offset), Color.green);
            //Debug.Log(true);
#endif
            return true;
        }
        else
        {
#if UNITY_EDITOR
            Debug.DrawLine(start, start + predictDirection.normalized * (predictLength + offset), Color.red);
            //Debug.Log(false);
#endif
            return false;
        }
    }
}
