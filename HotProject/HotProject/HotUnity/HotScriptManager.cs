using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HotUnity
{
    public class HotScriptManager
    {
        private static HotScriptManager _ins = null;
        private static HotScriptManager ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new HotScriptManager();
                }
                return _ins;
            }
        }

        private Dictionary<HotScriptAdapter, HotScript> hotScripts = new Dictionary<HotScriptAdapter, HotScript>();

        private static FieldInfo[] infoFields = null;
        private static object GetFieldValue(FieldInfo field, HotScriptAdapter.HotInfo info)
        {
            if (infoFields == null)
            {
                infoFields = info.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            }
            if (typeof(HotScript).IsAssignableFrom(field.FieldType))
            {
                return info.adapter_.targetObj;
            }
            else if (typeof(GameObject).IsAssignableFrom(field.FieldType))
            {
                return info.gameObject_;
            }
            else if (typeof(Component).IsAssignableFrom(field.FieldType))
            {
                return info.component_;
            }
            else if (field.FieldType.IsArray
                && typeof(HotScript).IsAssignableFrom(field.FieldType.GetElementType()))
            {
                var array = Array.CreateInstance(field.FieldType.GetElementType(), info.adapters.Length);
                for (int i = 0; i < info.adapters.Length; i++)
                {
                    array.SetValue(info.adapters[i].targetObj, i);
                }
                return array;
            }
            else if (field.FieldType.IsArray
                && typeof(GameObject).IsAssignableFrom(field.FieldType.GetElementType()))
            {
                var array = Array.CreateInstance(field.FieldType.GetElementType(), info.gameObjects.Length);
                for (int i = 0; i < info.gameObjects.Length; i++)
                {
                    array.SetValue(info.gameObjects[i], i);
                }
                return array;
            }
            else if (field.FieldType.IsArray
                && typeof(Component).IsAssignableFrom(field.FieldType.GetElementType()))
            {
                var array = Array.CreateInstance(field.FieldType.GetElementType(), info.components.Length);
                for (int i = 0; i < info.components.Length; i++)
                {
                    array.SetValue(info.components[i], i);
                }
                return array;
            }
            foreach (var f in infoFields)
            {
                if (field.FieldType.FullName == f.FieldType.FullName)
                {
                    return f.GetValue(info);
                }
            }
            return null;
        }


        public static void Awake(HotScriptAdapter adapter)
        {
            Type scriptType = Type.GetType(adapter.targetClass);
            HotScript behaviour = (HotScript)Activator.CreateInstance(scriptType);
            typeof(HotScript).GetProperty(nameof(HotScript.gameObject)
                , BindingFlags.Instance | BindingFlags.Public)
                .SetValue(behaviour, adapter.gameObject);
            adapter.targetObj = behaviour;
            foreach (var info in adapter.infos)
            {
                var field = scriptType.GetField(info.fieldName);
                var value = GetFieldValue(field, info);
                field.SetValue(behaviour, value);
            }
            ins.hotScripts.Add(adapter, behaviour);
            behaviour.Awake();
        }

        public static void OnEnable(HotScriptAdapter adapter)
        {
            ins.hotScripts[adapter].OnEnable();
        }

        public static void Start(HotScriptAdapter adapter)
        {
            ins.hotScripts[adapter].Start();
        }

        public static void Update(HotScriptAdapter adapter)
        {
            ins.hotScripts[adapter].Update();
        }

        public static void LateUpdate(HotScriptAdapter adapter)
        {
            ins.hotScripts[adapter].LateUpdate();
        }

        public static void FixedUpdate(HotScriptAdapter adapter)
        {
            ins.hotScripts[adapter].FixedUpdate();
        }

        public static void OnGUI(HotScriptAdapter adapter)
        {
            ins.hotScripts[adapter].OnGUI();
        }

        public static void OnDisable(HotScriptAdapter adapter)
        {
            ins.hotScripts[adapter].OnDisable();
        }

        public static void OnDestroy(HotScriptAdapter adapter)
        {
            ins.hotScripts[adapter].OnDestroy();
            ins.hotScripts.Remove(adapter);
        }
    }
}
