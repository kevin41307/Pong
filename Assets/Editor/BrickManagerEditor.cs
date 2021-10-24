using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BricksManager))]
public class BrickManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        

        if(GUILayout.Button("PrintSeatData"))
        {
            var bm = target as BricksManager;
        }
    }
}
