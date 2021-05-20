using System;
using System.Collections.Generic;
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

        private static object GetFieldValue(HotScriptAdapter.CacheInfo info)
        {
            if (info.typeName == typeof(string).FullName)
                return info.stringValue;
            if (info.typeName == typeof(Vector3).FullName)
                return info.vector3Value;
            if (info.typeName == typeof(float).FullName)
                return info.floatValue;
            if (info.typeName == typeof(int).FullName)
                return info.intValue;
            return info.componentValue;
        }


        public static void Awake(HotScriptAdapter adapter)
        {
            Type scriptType = Type.GetType(adapter.targetClass);
            HotScript behaviour = (HotScript)Activator.CreateInstance(scriptType);
            foreach (var info in adapter.cacheInfos)
            {
                var field = scriptType.GetField(info.fieldName);
                var value = GetFieldValue(info);
                if (typeof(HotScript).IsAssignableFrom(field.FieldType))
                {
                    field.SetValue(behaviour, ((HotScriptAdapter)value).targetObj);
                }
                else
                {
                    field.SetValue(behaviour, value);
                }
            }
            adapter.targetObj = behaviour;
            ins.hotScripts.Add(adapter, behaviour);
            behaviour.Awake();
        }

        public static void Start(HotScriptAdapter adapter)
        {
            ins.hotScripts[adapter].Start();
        }

        public static void Update(HotScriptAdapter adapter)
        {
            ins.hotScripts[adapter].Update();
        }

        public static void OnGUI(HotScriptAdapter adapter)
        {
            ins.hotScripts[adapter].OnGUI();
        }

        public static void OnDestroy(HotScriptAdapter adapter)
        {
            ins.hotScripts[adapter].OnDestroy();
            ins.hotScripts.Remove(adapter);
        }
    }
}
