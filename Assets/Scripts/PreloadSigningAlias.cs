#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;


[InitializeOnLoad]
public class PreloadSigningAlias
{

    static PreloadSigningAlias()
    {
        PlayerSettings.Android.keystorePass = "12345678";
        PlayerSettings.Android.keyaliasName = "kogaine";
        PlayerSettings.Android.keyaliasPass = "12345678";
    }

}

#endif