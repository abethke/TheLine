using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LayoutElementByPercent))]
public class LayoutElementByPercentInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LayoutElementByPercent layout = target as LayoutElementByPercent;
        if (GUILayout.Button("Set Layout Element Values"))
        {
            layout.SetCalculatedValues();
            EditorUtility.SetDirty(target);
        }
    }
}
