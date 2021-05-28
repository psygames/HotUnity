using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HotUnity.Editor
{
    [InitializeOnLoad]
    public class AddScriptButtonHook
    {
        static AddScriptButtonHook()
        {
            HookInspectorWindow();
        }

        static void HookInspectorWindow()
        {
            Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            MethodInfo miTarget = type.GetMethod("DrawEditors", BindingFlags.Instance | BindingFlags.NonPublic);

            type = typeof(AddScriptButtonHook);
            MethodInfo miReplacement = type.GetMethod(nameof(NewOnGUI), BindingFlags.Static | BindingFlags.NonPublic);

            MethodInfo miProxy = type.GetMethod(nameof(ProxyOnGUI), BindingFlags.Static | BindingFlags.NonPublic);

            var hook = new MethodHook(miTarget, miReplacement, miProxy);
            hook.Install();
        }

        static void ProxyOnGUI(UnityEditor.Editor[] editors)
        {

        }

        static void NewOnGUI(UnityEditor.Editor[] editors)
        {
            ProxyOnGUI(editors);
            OnInspectorGUI();
        }

        private static void OnInspectorGUI()
        {
            if (Application.isPlaying) return;
            if (Selection.activeTransform == null) return;
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var content = EditorGUIUtility.TrTextContent("Add HotScript");
            var rect = GUILayoutUtility.GetRect(content, "AC Button");
            if (EditorGUI.DropdownButton(rect, content, FocusType.Passive, "AC Button"))
            {
                AddScriptWindow.Show(rect, OnAddScript, OnFilterScript);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            rect.y -= 10;
            rect.xMin = 0;
            rect.width = EditorGUIUtility.currentViewWidth;
            rect.height = 1;
            EditorGUI.DrawRect(rect, Helper.splitLineColor);

            EditorGUILayout.Space();
        }

        private static void OnAddScript(string script)
        {
            var targetObj = Selection.activeTransform.gameObject;
            var comp = Undo.AddComponent<HotScriptAdapter>(targetObj);
            comp.targetClass = script;
        }

        private static string[] OnFilterScript(string search)
        {
            var baseTypeName = "HotUnity.HotScript";
            var baseType = Helper.hotAssembly.GetType(baseTypeName).ReflectionType;
            var list = new List<string>();
            foreach (var kv in Helper.hotAssembly.LoadedTypes)
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
