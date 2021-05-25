using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HotUnity.Editor
{
    public static class HotFieldGUI
    {
        public static Type hotScriptType;

        public static HotScriptAdapter.HotInfo EditorDrawField(Type type, HotScriptAdapter.HotInfo info)
        {

            /*
            var title = Helper.ToTitle(info.fieldName);
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
                info.componentValue = (Component)EditorGUILayout.ObjectField(title, info.componentValue, type, true);
            }
            else if (type.IsClass && hotScriptType.IsAssignableFrom(type))
            {
                var tempComp = (HotScriptAdapter)EditorGUILayout.ObjectField(title,
                    info.componentValue, typeof(HotScriptAdapter), true);
                if (tempComp != null && (tempComp).targetClass == type.FullName)
                {
                    info.componentValue = tempComp;
                }
                else
                {
                    info.componentValue = null;
                }
            }
            */
            return info;
        }


        public static void RuntimeDrawField(FieldInfo fieldInfo, object obj)
        {
            var title = Helper.ToTitle(fieldInfo.Name);
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
}
