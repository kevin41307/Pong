#if UNITY_ANDROID
using UnityEngine;
using UnityEngine.UI;

public class LagacyJoyStickInput : MonoBehaviour
{

    Camera m_Camera;
    Touch touch;
    RectTransform rectTransform;
    Image image;
    Color invisible;
    Color visible;


    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        m_Camera = Camera.main;     
    }
    private void Start()
    {
        invisible = new Color(image.color.r, image.color.g, image.color.b, 0);
        visible = new Color(image.color.r, image.color.g, image.color.b, .75f);
    }


    private void Update()
    {
        if (Input.touchCount > 0)
        {    
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                image.color = visible;
                touch = Input.GetTouch(0);
                rectTransform.position = Input.GetTouch(0).position;
                //Debug.Log(Input.GetTouch(0).position);
            }
        }
        else
        {
            image.color = invisible;
        }
    }

}
#endif
