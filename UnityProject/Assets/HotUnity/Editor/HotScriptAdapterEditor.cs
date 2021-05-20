using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace HotUnity
{
    [CustomEditor(typeof(HotScriptAdapter))]
    public class HotScriptAdapterEditor : Editor
    {
        public ILRuntime.Runtime.Enviorment.AppDomain hotAssembly => assemblyLoader.appdomain;
        private HotAssemblyLoader assemblyLoader;

        private void OnEnable()
        {
            if (assemblyLoader == null)
                assemblyLoader = new HotAssemblyLoader();
            assemblyLoader.Reloead();
        }

        private void OnDisable()
        {
            assemblyLoader?.Unload();
        }

        protected override void OnHeaderGUI()
        {
            Debug.LogError("OnHeaderGUI");
        }

        public override void OnInspectorGUI()
        {
            return;
            var target = serializedObject.targetObject as HotScriptAdapter;

            EditorGUILayout.LabelField(ObjectNames.GetInspectorTitle(target));

            var _class = EditorGUILayout.TextField("Target Class", target.targetClass);
            if (_class != target.targetClass)
            {
                target.targetClass = _class;
                EditorUtility.SetDirty(target);
            }

            // EditorGUILayout.PropertyField(serializedObject.FindProperty("cacheInfos"));

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            bool isEditorMode = prefabStage != null || !Application.isPlaying;

            var type = hotAssembly.GetType(target.targetClass)?.ReflectionType;
            if (type == null)
            {
                EditorGUILayout.HelpBox($"Type not found: {target.targetClass}", MessageType.Error);
                return;
            }

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (isEditorMode)
            {
                target.cacheInfos = target.cacheInfos ?? new HotScriptAdapter.CacheInfo[0];
                var infos = new List<HotScriptAdapter.CacheInfo>(target.cacheInfos);
                bool changed = false;
                foreach (var f in fields)
                {
                    changed |= EditorDrawField(f, infos);
                }
                changed |= infos.Count != target.cacheInfos.Length;
                for (int i = infos.Count - 1; i >= 0; i--)
                {
                    if (!fields.Any(a => a.Name == infos[i].fieldName &&
                    a.FieldType.FullName == infos[i].typeName))
                    {
                        changed = true;
                        infos.RemoveAt(i);
                    }
                }
                if (changed)
                {
                    target.cacheInfos = infos.ToArray();
                    EditorUtility.SetDirty(target);
                }
            }
            else
            {
                foreach (var f in fields)
                {
                    RuntimeDrawField(f, target.targetObj);
                }
            }
        }

        private static string ToTitle(string name)
        {
            var sb = new StringBuilder();
            sb.Append(char.ToUpper(name[0]));
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    sb.Append(' ');
                }
                sb.Append(name[i]);
            }
            return sb.ToString();
        }

        private HotScriptAdapter.CacheInfo EditorDrawInfo(Type type, HotScriptAdapter.CacheInfo info)
        {
            var title = ToTitle(info.fieldName);
            if (info.typeName == typeof(string).FullName)
            {
                info.stringValue = EditorGUILayout.TextField(title, info.stringValue);
            }
            else if (info.typeName == typeof(Vector3).FullName)
            {
                info.vector3Value = EditorGUILayout.Vector3Field(title, info.vector3Value);
            }
            else if (type.IsClass && typeof(Component).IsAssignableFrom(type))
            {
                info.componentValue = (Component)EditorGUILayout.ObjectField(title, info.componentValue, type);
            }
            else if (type.IsClass &&
                hotAssembly.GetType("HotUnity.HotScript").
                ReflectionType.IsAssignableFrom(type))
            {
                info.componentValue = (Component)EditorGUILayout.ObjectField(title, info.componentValue, typeof(HotScriptAdapter));
            }
            return info;
        }

        private bool EditorDrawField(FieldInfo fieldInfo, List<HotScriptAdapter.CacheInfo> infos)
        {
            var index = infos.FindIndex(a =>
            {
                return a.fieldName == fieldInfo.Name &&
                a.typeName == fieldInfo.FieldType.FullName;
            });

            HotScriptAdapter.CacheInfo info;
            if (index == -1)
            {
                info = new HotScriptAdapter.CacheInfo();
                info.fieldName = fieldInfo.Name;
                info.typeName = fieldInfo.FieldType.FullName;
                infos.Add(info);
                index = infos.Count - 1;
            }
            else
            {
                info = infos[index];
            }

            info = EditorDrawInfo(fieldInfo.FieldType, info);

            if (!infos[index].Equals(info))
            {
                infos[index] = info;
                return true;
            }
            return false;
        }

        private void RuntimeDrawField(FieldInfo fieldInfo, object obj)
        {
            var title = ToTitle(fieldInfo.Name);
            if (fieldInfo.FieldType.FullName == typeof(string).FullName)
            {
                var value = EditorGUILayout.TextField(title, $"{fieldInfo.GetValue(obj)}");
                fieldInfo.SetValue(obj, value);
            }
            else if (fieldInfo.FieldType.FullName == typeof(Vector3).FullName)
            {
                var value = EditorGUILayout.Vector3Field(title, (Vector3)fieldInfo.GetValue(obj));
                fieldInfo.SetValue(obj, value);
            }
        }
    }

    public class HotAssemblyLoader
    {
        private string path => Path.Combine(Application.streamingAssetsPath, "HotProject.dll");
        public ILRuntime.Runtime.Enviorment.AppDomain appdomain;
        MemoryStream fs;

        public void Load()
        {
            appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            var bytes = File.ReadAllBytes(path);
            fs = new MemoryStream(bytes);
            try
            {
                appdomain.LoadAssembly(fs, null, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            catch
            {
                Debug.LogError("加载热更DLL失败");
            }
        }

        public void Unload()
        {
            fs.Close();
            appdomain = null;
        }

        public void Reloead()
        {
            if (appdomain != null)
            {
                Unload();
            }
            Load();
        }
    }
}
