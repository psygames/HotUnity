﻿using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HotUnity.Editor
{
    [CustomEditor(typeof(AddScriptButton))]
    public class AddScriptButtonEditor : UnityEditor.Editor
    {
        private ILRuntime.Runtime.Enviorment.AppDomain hotAssembly => assemblyLoader.appdomain;
        private HotAssemblyLoader assemblyLoader;
        private new AddScriptButton target => serializedObject.targetObject as AddScriptButton;

        private void OnEnable()
        {
            assemblyLoader = assemblyLoader ?? new HotAssemblyLoader();
            assemblyLoader.Reloead();
        }

        private void OnDisable()
        {
            assemblyLoader?.Unload();
        }

        protected override void OnHeaderGUI()
        {
            var rect = EditorGUILayout.GetControlRect(false, 0f);
            topRect = rect;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y -= rect.height;
            rect.xMin -= 16;
            rect.xMax += 4;
            EditorGUI.DrawRect(rect, Helper.backgroudColor);

            // GUI.DrawTexture(rect, Helper.scriptIcon, ScaleMode.ScaleToFit);
            rect.yMax += 4;
            var _lastColor = GUI.skin.box.normal.textColor;
            GUI.skin.box.normal.textColor = Helper.tipsFieldColor;
            GUI.Label(rect, "在这里添加热更脚本(Add HotScript Here)", GUI.skin.box);
            GUI.skin.box.normal.textColor = _lastColor;
        }

        private Rect topRect;
        public override void OnInspectorGUI()
        {
            OnHeaderGUI();

            if (ComponentUtility.MoveComponentDown(target))
            {
                return;
            }

            if (!InternalEditorUtility.GetIsInspectorExpanded(target))
            {
                InternalEditorUtility.SetIsInspectorExpanded(target, true);
                ActiveEditorTracker.sharedTracker.ForceRebuild();
                return;
            }

            try { EditorGUILayout.GetControlRect(GUILayout.Height(30)); } catch { return; }
            var content = EditorGUIUtility.TrTextContent("Add HotScript");
            var rect = topRect;
            rect.x += (rect.width - 230) / 2 - 8;
            rect.width = 230;
            rect.height = 23;
            rect.y += 8;
            if (EditorGUI.DropdownButton(rect, content, FocusType.Passive, "AC Button"))
            {
                assemblyLoader?.Reloead();
                AddScriptWindow.Show(rect, OnAddScript, OnFilterScript);
            }
        }

        private void OnAddScript(string script)
        {
            var comp = Undo.AddComponent<HotScriptAdapter>(target.gameObject);
            comp.targetClass = script;
        }

        private string[] OnFilterScript(string search)
        {
            var baseTypeName = "HotUnity.HotScript";
            var baseType = hotAssembly.GetType(baseTypeName).ReflectionType;
            var list = new List<string>();
            foreach (var kv in hotAssembly.LoadedTypes)
            {
                if (kv.Key == baseTypeName) continue;
                if (!string.IsNullOrEmpty(search)
                    && !kv.Key.ToLower().Contains(search.ToLower())) continue;
                var type = kv.Value.ReflectionType;
                if (type.IsClass && baseType.IsAssignableFrom(type))
                {
                    list.Add(kv.Key);
                }
            }
            list.Sort();
            return list.ToArray();
        }
    }
}
