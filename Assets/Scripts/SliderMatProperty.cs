using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderMatProperty : MonoBehaviour
{
    public string propertyName;
    public Slider slider;
    public MeshRenderer meshRenderer;
    Material material;


    private void Awake()
    {
        material = meshRenderer.material;
    }

    private void Update()
    {
        material.SetFloat(propertyName, slider.value);
    }
}
