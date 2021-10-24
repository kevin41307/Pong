using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class MaterialInventory : MonoBehaviourSingleton<MaterialInventory>
{
    static int hash_HorizontalLineCount = Shader.PropertyToID("_HorizontalLineCount");
    static int hash_VerticalLineCount = Shader.PropertyToID("_VerticalLineCount");

    private const int k_poolSize = 7;
    public Material white0x0;
    [HideInInspector]
    public Material[] whites = new Material[k_poolSize];

    public Material red0x0;
    [HideInInspector]
    public Material[] reds = new Material[k_poolSize];

    public Material green0x0;
    [HideInInspector]
    public Material[] greens = new Material[k_poolSize];

    public Material blue0x0;
    [HideInInspector]
    public Material[] blues = new Material[k_poolSize];

    private void Awake()
    {
        whites = Build(white0x0);
        reds = Build(red0x0);
        greens = Build(green0x0);
        blues = Build(blue0x0);

        /*
        for (int i = 0; i < reds.Length; i++)
        {
            Debug.Log( reds[i].GetFloat(hash_HorizontalLineCount));
        }
        */
    }

    private Material[] Build(Material original)
    {
        Material[] materials = new Material[k_poolSize];
        for (int i = 0; i < materials.Length; i++)
        {
            if (i == 0)
                materials[i] = original;
            else
            {
                materials[i] = Instantiate(original);
                materials[i].SetFloat(hash_HorizontalLineCount, i + 1);
                materials[i].SetFloat(hash_VerticalLineCount, i);

            }
        }
        return materials;
    }

    public Material GetMaterialInstance(BrickColorType brickColorType, int intensity)
    {
        if (intensity > k_poolSize) return null;

        switch (brickColorType)
        {
            case BrickColorType.White:
                return whites[intensity];
            case BrickColorType.Red:
                return reds[intensity];
            case BrickColorType.Blue:
                return blues[intensity];
            case BrickColorType.Green:
                return greens[intensity];                
            default:
#if UNITY_EDITOR
                Debug.Log("not valid brickcolortype: " + brickColorType);
#endif
                return whites[intensity];
        }
    }


}
