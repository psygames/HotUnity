using UnityEditor;
using UnityEngine;

namespace HotUnity.Editor
{
    [InitializeOnLoad]
    public class AddScriptButtonPlaceScript
    {
        static AddScriptButtonPlaceScript()
        {
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
    }
}
