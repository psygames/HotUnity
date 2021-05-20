using System;
using UnityEngine;

namespace HotUnity
{
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
        }

        void Awake()
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

        void OnGUI()
        {
            InvokeMethod(nameof(OnGUI));
        }

        void OnDestroy()
        {
            InvokeMethod(nameof(OnDestroy));
        }

        void InvokeMethod(string name)
        {
            ILRuntimeInit.Invoke("HotUnity.HotScriptManager", name, this);
        }
    }
}
