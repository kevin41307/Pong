using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictArea : MonoBehaviour
{
    public Collider2D constructionArea1P;
    public Collider2D constructionArea2P;
    public Collider2D moveableArea1P;
    public Collider2D moveableArea2P;

    const float halfExercisableHeight = 4.5f; // construction + move area + gap
    const float halfExercisableWidth = 11f;
    const float halfGapWidth = 0.275f;
    Vector3 pivot = Vector3.zero;

    public static float minWidth = 3;
    public static System.Action<float> OnHasSetMoveableArea;

    private void Start()
    {
        //MyGameEventSystem.Instance.OnUsePushLevelItem += SetMoveableArea;
        MyGameEventSystem.UsePushLevelItemEvent.performed += SetMoveableArea;
        SetMoveableArea(0);
    }

    private void SetMoveableArea(float offset)
    {
        //...-5...-4...-3....-2...-1...0...1...2...3...4...5...
        //...-5....b..center..a...-1..pivot.1..2...3...4...5...
        //...-5...-4...-3.....|==size==|...1...2...3...4...5...

        pivot = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);
        
        Vector3 center1P = new Vector3( ((pivot.x - halfGapWidth - constructionArea1P.bounds.size.x ) + (-halfExercisableWidth)) * 0.5f, 0f, 0f);
        Vector3 size1P = new Vector3((pivot.x - halfGapWidth - constructionArea1P.bounds.size.x) + (halfExercisableWidth) , halfExercisableHeight * 2f, 1);

        Vector2 center2P = new Vector3(((pivot.x + halfGapWidth + constructionArea2P.bounds.size.x) + halfExercisableWidth ) * 0.5f, 0f, 0f);
        Vector2 size2P = new Vector3( halfExercisableWidth - (pivot.x + halfGapWidth + constructionArea2P.bounds.size.x), halfExercisableHeight * 2f, 1);
        
        if (size1P.x < minWidth || size2P.x < minWidth)
        {
            Debug.Log("Moveable area too small, return;");
            return;
        }

        transform.position = pivot;

        moveableArea1P.transform.position = center1P;
        moveableArea1P.transform.localScale = size1P;
        moveableArea2P.transform.position = center2P;
        moveableArea2P.transform.localScale = size2P;

        EnclosureArea2D enclosure = moveableArea1P.GetComponent<EnclosureArea2D>();
        enclosure.UpdateBounds();
        EnclosureArea2D enclosure2 = moveableArea2P.GetComponent<EnclosureArea2D>();
        enclosure2.UpdateBounds();
        OnHasSetMoveableArea?.Invoke(offset);

    }
}
