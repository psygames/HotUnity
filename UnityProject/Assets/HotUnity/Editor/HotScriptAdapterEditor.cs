using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace HotUnity.Editor
{
    [CustomEditor(typeof(HotScriptAdapter))]
    public class HotScriptAdapterEditor : UnityEditor.Editor
    {
        private ILRuntime.Runtime.Enviorment.AppDomain hotAssembly => assemblyLoader.appdomain;
        private HotAssemblyLoader assemblyLoader;
        private new HotScriptAdapter target => serializedObject.targetObject as HotScriptAdapter;

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
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y -= rect.height;

            // icon
            rect.x = 16;
            rect.xMax = 32;
            EditorGUI.DrawRect(rect, Helper.backgroudColor);
            GUI.DrawTexture(rect, Helper.scriptIcon);

            // title
            rect.x = 48;
            rect.xMax = EditorGUIUtility.currentViewWidth - 96;
            EditorGUI.DrawRect(rect, Helper.backgroudColor);
            var className = target.targetClass;
            if (className.LastIndexOf('.') != -1)
            {
                className = className.Substring(className.LastIndexOf('.') + 1);
            }
            string header = $"{className} (HotScript)";
            EditorGUI.LabelField(rect, header, EditorStyles.boldLabel);
        }

        public override void OnInspectorGUI()
        {
            OnHeaderGUI();

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            bool isEditorMode = prefabStage != null || !Application.isPlaying;

            var type = hotAssembly.GetType(target.targetClass)?.ReflectionType;
            if (type == null)
            {
                target.targetClass = EditorGUILayout.TextField("Target Class", target.targetClass);
                EditorGUILayout.HelpBox($"Target Class Not Found: {target.targetClass}, You can fix it manually.",
                    MessageType.Error);
                return;
            }

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (isEditorMode)
            {
                EditorDraw(fields);
            }
            else
            {
                RuntimeDraw(fields);
            }
        }

        private void RuntimeDraw(FieldInfo[] fields)
        {
            foreach (var f in fields)
            {
                HotFieldGUI.RuntimeDrawField(f, target.targetObj);
            }
        }

        private void EditorDraw(FieldInfo[] fields)
        {
            target.infos = target.infos ?? new HotScriptAdapter.HotInfo[0];
            var infos = new List<HotScriptAdapter.HotInfo>(target.infos);
            bool isSizeChanged = false;
            foreach (var f in fields)
            {
                var index = infos.FindIndex(a =>
                {
                    return a.fieldName == f.Name &&
                    a.fieldType == f.FieldType.FullName;
                });

                if (index == -1)
                {
                    var info = new HotScriptAdapter.HotInfo();
                    info.fieldName = f.Name;
                    info.fieldType = f.FieldType.FullName;
                    infos.Add(info);
                    isSizeChanged = true;
                }
            }
            for (int i = infos.Count - 1; i >= 0; i--)
            {
                if (!fields.Any(a => a.Name == infos[i].fieldName &&
                a.FieldType.FullName == infos[i].fieldType))
                {
                    isSizeChanged = true;
                    infos.RemoveAt(i);
                }
            }

            if (isSizeChanged)
            {
                target.infos = infos.ToArray();
                EditorUtility.SetDirty(target);
                return;
            }

            bool isChanged = false;
            foreach (var f in fields)
            {
                var index = infos.FindIndex(a =>
                {
                    return a.fieldName == f.Name &&
                    a.fieldType == f.FieldType.FullName;
                });

                if (index != -1)
                {
                    isChanged |= EditorDrawField(f, index);
                }
            }

            if (isChanged)
            {
                EditorUtility.SetDirty(target);
            }
        }

        private static Dictionary<string, bool> foldouts = new Dictionary<string, bool>();
        private T[] EditorDrawArray<T>(string name, T[] array, Type elementType) where T : UnityEngine.Object
        {
            if (array == null) array = (T[])Array.CreateInstance(typeof(T), 0);
            var _foldoutName = target.GetInstanceID() + name;
            if (!foldouts.ContainsKey(_foldoutName)) foldouts[_foldoutName] = false;
            foldouts[_foldoutName] = EditorGUILayout.Foldout(foldouts[_foldoutName], name);
            if (foldouts[_foldoutName])
            {
                EditorGUI.indentLevel++;
                var newLength = EditorGUILayout.DelayedIntField("Size", array.Length);
                newLength = Mathf.Clamp(newLength, 0, 65535);
                if (newLength != array.Length)
                {
                    var newArray = (T[])Array.CreateInstance(typeof(T), newLength);
                    Array.Copy(array, newArray, Mathf.Min(array.Length, newLength));
                    array = newArray;
                }
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = (T)EditorDrawObject($"Element {i}", array[i], elementType);
                }
                EditorGUI.indentLevel--;
            }
            return array;
        }

        private UnityEngine.Object EditorDrawObject(string title, UnityEngine.Object obj, Type objType, string realTypeName = null)
        {
            if (objType == typeof(HotScriptAdapter))
            {
                Rect rect;
                try { rect = EditorGUILayout.GetControlRect(); }
                catch { return obj; }
                var _obj = EditorGUI.ObjectField(rect, title, obj, objType, true);
                var w = EditorGUIUtility.labelWidth;
                rect.xMin += w + 1;
                rect.xMax -= 19;
                rect.yMin += 1;
                rect.yMax -= 1;
                EditorGUI.DrawRect(rect, Helper.objFieldColor);
                var iconRect = rect;
                iconRect.x += 1;
                iconRect.width = 14;
                GUI.DrawTexture(iconRect, Helper.scriptIcon);
                var labelRect = rect;
                labelRect.xMin += 16;
                labelRect.yMin -= 1;
                labelRect.yMax += 1;
                var objName = obj ? obj.name : "None";
                var labelName = $"{objName} ({realTypeName})";
                EditorGUI.LabelField(labelRect, labelName);
                return _obj;
            }
            else
            {
                try
                {
                    return EditorGUILayout.ObjectField(title, obj, objType, true);
                }
                catch { return obj; }
            }
        }

        private bool EditorDrawField(FieldInfo field, int fieldIndex)
        {
            EditorGUI.BeginChangeCheck();
            var hotScriptType = hotAssembly.GetType("HotUnity.HotScript").ReflectionType;
            var info = target.infos[fieldIndex];
            bool isSerializedProperty = false;

            if (hotScriptType.IsAssignableFrom(field.FieldType))
            {
                info.adapter_ = (HotScriptAdapter)EditorDrawObject(Helper.ToTitle(field.Name),
                    info.adapter_, typeof(HotScriptAdapter), field.FieldType.Name);
            }
            else if (typeof(GameObject).IsAssignableFrom(field.FieldType))
            {
                info.gameObject_ = (GameObject)EditorDrawObject(Helper.ToTitle(field.Name),
                    info.gameObject_, field.FieldType);
            }
            else if (typeof(Component).IsAssignableFrom(field.FieldType))
            {
                info.component_ = (Component)EditorDrawObject(Helper.ToTitle(field.Name),
                    info.component_, field.FieldType);
            }
            else if (field.FieldType.IsArray
               && hotScriptType.IsAssignableFrom(field.FieldType.GetElementType()))
            {
                info.adapters = EditorDrawArray(
                    Helper.ToTitle(field.Name),
                    info.adapters, typeof(HotScriptAdapter));
            }
            else if (field.FieldType.IsArray
                && typeof(GameObject).IsAssignableFrom(field.FieldType.GetElementType()))
            {
                info.gameObjects = EditorDrawArray(
                    Helper.ToTitle(field.Name),
                    info.gameObjects, field.FieldType.GetElementType());
            }
            else if (field.FieldType.IsArray
                && typeof(Component).IsAssignableFrom(field.FieldType.GetElementType()))
            {
                info.components = EditorDrawArray(
                    Helper.ToTitle(field.Name),
                    info.components, field.FieldType.GetElementType());
            }
            else
            {
                isSerializedProperty = true;
                var infoField = typeof(HotScriptAdapter.HotInfo)
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(a => a.FieldType.FullName == field.FieldType.FullName);
                var infoProp = serializedObject.FindProperty(nameof(HotScriptAdapter.infos)).
                    GetArrayElementAtIndex(fieldIndex).FindPropertyRelative(infoField.Name);
                try
                {
                    EditorGUILayout.PropertyField(infoProp, new GUIContent(Helper.ToTitle(field.Name)));
                }
                catch { return EditorGUI.EndChangeCheck(); }
            }

            bool changed = EditorGUI.EndChangeCheck();
            if (changed)
            {
                if (isSerializedProperty)
                    serializedObject.ApplyModifiedProperties();
                else
                    target.infos[fieldIndex] = info;
            }
            return changed;
        }
    }
}
