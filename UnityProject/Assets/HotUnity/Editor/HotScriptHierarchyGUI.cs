using UnityEditor;
using UnityEngine;

namespace HotUnity.Editor
{
    [InitializeOnLoad]
    public class HotScriptHierarchyGUI
    {
        static HotScriptHierarchyGUI()
        {
            // if you don't want hierarchy gui, remove this line.
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
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
