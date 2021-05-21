using UnityEditor;
using UnityEngine;

namespace HotUnity.Editor
{
    [InitializeOnLoad]
    public class FixAddScriptButton
    {
        static FixAddScriptButton()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static AddScriptButton addScriptTool;

        static void OnSelectionChanged()
        {
            if (Selection.activeTransform)
            {
                addScriptTool = Selection.activeTransform.GetComponent<AddScriptButton>();
                if (!addScriptTool)
                {
                    addScriptTool = Selection.activeTransform.gameObject.AddComponent<AddScriptButton>();
                }
            }
            else
            {
                if (addScriptTool)
                {
                    GameObject.DestroyImmediate(addScriptTool);
                }
            }
        }

        static void HierarchyWindowItemOnGUI(int instanceId, Rect selectionRect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (gameObject && gameObject.GetComponent<HotScriptAdapter>() != null)
            {
                Rect rect = new Rect(selectionRect.x + selectionRect.width - 16f, selectionRect.y, 16f, 16f);
                GUI.DrawTexture(rect, Helper.scriptIcon);
            }
        }
    }
}
