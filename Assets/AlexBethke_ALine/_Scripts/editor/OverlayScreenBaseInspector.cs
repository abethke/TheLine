using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ALine
{
    [CustomEditor(typeof(OverlayScreenBase), true)]
    public class OverlayScreenBaseInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            OverlayScreenBase screen = target as OverlayScreenBase;
            if (GUILayout.Button("Build UI Fade Lists"))
            {
                screen.imagesToFade = screen.GetComponentsInChildren<Image>();
                screen.textToFade = screen.GetComponentsInChildren<TMP_Text>();
                EditorUtility.SetDirty(target);
            }
        }
    }
}