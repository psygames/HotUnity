using System;
using UnityEngine;

namespace HotUnity
{
    public class HotScriptAdapter : MonoBehaviour
    {
        public string targetClass;

        public string[] strings;
        public bool[] bools;
        public char[] chars;
        public short[] shorts;
        public int[] ints;
        public float[] floats;
        public double[] doubles;
        public int[] enums;
        public Vector2[] vector2s;
        public Vector2Int[] vector2Ints;
        public Vector3[] vector3s;
        public Vector3Int[] vector3Ints;
        public Vector4[] vector4s;
        public RangeInt[] rangeInts;
        public Quaternion[] quaternions;
        public Rect[] rects;
        public RectInt[] rectInts;
        public Matrix4x4[] matrix4X4s;
        public Bounds[] bounds;
        public BoundsInt[] boundsInts;
        public Color[] colors;
        public Color32[] color32s;
        public Component[] components;
        public HotScriptAdapter[] adapters;

        public int[][] intss;


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

        public object targetObj { get; set; }
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
