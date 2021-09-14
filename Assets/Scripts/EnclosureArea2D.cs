using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnclosureArea2D : MonoBehaviour
{
    public Collider2D area2D;

    private Vector2 m_Center = Vector2.zero;
    public Vector2 Center
    {
        get
        {
            if (m_Center != Vector2.zero) return m_Center;

            if (area2D != null)
            {
                m_Center = area2D.bounds.center;
            }
            else
            {
                area2D = GetComponent<Collider2D>();
                if( area2D == null )
                {
                    Debug.Log("area2D is null");
                }
                return Vector2.zero;
            }
            return m_Center;
        }

        private set
        {
            m_Center = value;
        }
    }
    private Vector2 m_Min = Vector2.zero;
    public Vector2 Min
    {
        get
        {
            if (m_Min != Vector2.zero) return m_Min;

            if (area2D != null)
            {
                m_Min = area2D.bounds.min;
            }
            else
            {
                area2D = GetComponent<Collider2D>();
                if (area2D == null)
                {
                    Debug.Log("area2D is null");
                }
                return Vector2.zero;
            }
            return m_Min;
        }

        private set
        {
            m_Min = value;
        }

    }

    private Vector2 m_Max = Vector2.zero;
    public Vector2 Max
    {
        get
        {
            if (m_Max != Vector2.zero) return m_Max;

            if (area2D != null)
            {
                m_Max = area2D.bounds.max;
            }
            else
            {
                area2D = GetComponent<Collider2D>();
                if (area2D == null)
                {
                    Debug.Log("area2D is null");
                }
                return Vector2.zero;
            }
            return m_Max;
        }

        private set
        {
            m_Max = value;
        }
    }

    private Vector2 m_Extent = Vector2.zero;
    public Vector2 Extent
    {
        get
        {
            if (m_Extent != Vector2.zero) return m_Extent;

            if (area2D != null)
            {
                m_Extent = area2D.bounds.extents;
            }
            else
            {
                area2D = GetComponent<Collider2D>();
                if (area2D == null)
                {
                    Debug.Log("area2D is null");
                }
                return Vector2.zero;
            }
            return m_Extent;
        }
        set
        {
            m_Extent = value;
        }
    }

    public bool IsContain(Vector2 position)
    {
        if (position.x > Min.x && position.x < Max.x)
        {
            if(position.y > Min.y && position.y < Max.y)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsContain(float xy, Vector2 axis)
    {
        if(axis == Vector2.up)
        {
            if (xy > Min.y && xy < Max.y)
            {
                return true;
            }
        }

        if (axis == Vector2.right)
        {
            if (xy > Min.x && xy < Max.x)
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateBounds(Vector2 center, Vector2 extentInner, Vector2 extentOutter)
    {
        Vector2 extent = extentOutter - extentInner;
        Center = center;
        Min = center - extent;
        Max = center + extent;
    }

}
