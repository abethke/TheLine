using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraResizer))]
public class CameraResizerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Resize Orthographic Camera"))
        {
            CameraResizer resizer = target as CameraResizer;
            resizer.Resize();
        }
    }
}
