using System;
using UnityEngine;

public class HotScriptAdapter : MonoBehaviour
{
    public string targetClass;
    public object targetObj;

    public CacheInfo[] cacheInfos;

    [Serializable]
    public struct CacheInfo
    {
        public string fieldName;
        public string typeName;

        public int intValue;
        public float floatValue;
        public Vector3 vector3Value;
        public string stringValue;
        public Component componentValue;

        public object value
        {
            get
            {
                if (typeName == typeof(string).FullName)
                    return stringValue;
                if (typeName == typeof(Vector3).FullName)
                    return vector3Value;
                if (typeName == typeof(float).FullName)
                    return floatValue;
                if (typeName == typeof(int).FullName)
                    return intValue;
                return componentValue;
            }
        }
    }

    private void Awake()
    {
        InvokeMethod(nameof(Awake));
    }

    void Start()
    {
        InvokeMethod(nameof(Start));
    }

    void Update()
    {
        InvokeMethod(nameof(Update));
    }

    private void OnGUI()
    {
        InvokeMethod(nameof(OnGUI));
    }

    private void OnDestroy()
    {
        InvokeMethod(nameof(OnDestroy));
    }

    private void InvokeMethod(string name)
    {
        ILRuntimeInit.Invoke("HotProject.HotScriptManager", name, this);
    }
}
