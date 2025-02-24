using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(Button))]
public class ButtonInspectorExtension : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Button btn = target as Button;
        if (GUILayout.Button("Rename Instance"))
        {
            string labelValue = string.Empty;
            TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
            if (label == null)
            {
                Text labelLegacy = btn.GetComponentInChildren<Text>();
                if (labelLegacy != null)
                {
                    labelValue = labelLegacy.text;
                }
            }
            else
            {
                labelValue = label.text;
            }

            btn.name = $"btn:{labelValue}";

            EditorUtility.SetDirty(btn);
        }

        if (GUILayout.Button("Size to Image Texture"))
        {
            Image img = btn.GetComponent<Image>();
            Debug.Log($"Texture size: {img.sprite.texture.width}, {img.sprite.texture.height}");

            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(img.sprite.texture.width, img.sprite.texture.height);
        }
    }
}
