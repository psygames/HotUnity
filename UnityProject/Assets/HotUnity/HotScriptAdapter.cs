using System;
using UnityEngine;

namespace HotUnity
{
    public class HotScriptAdapter : MonoBehaviour
    {
        public string targetClass;
        public object targetObj { get; set; }

        public HotInfo[] infos;

        private void Awake()
        {
            InvokeMethod(nameof(Awake));
        }

        private void Start()
        {
            InvokeMethod(nameof(Start));
        }

        private void Update()
        {
            InvokeMethod(nameof(Update));
        }

        private void LateUpdate()
        {
            InvokeMethod(nameof(LateUpdate));
        }

        private void FixedUpdate()
        {
            InvokeMethod(nameof(FixedUpdate));
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
            ILRuntimeInit.Invoke("HotUnity.HotScriptManager", name, this);
        }


        // HOT FIELD INFO
        [Serializable]
        public struct HotInfo
        {
            [SerializeField]
            private string _fieldName;
            [SerializeField]
            private string _fieldType;

            public string fieldName
            {
                get { return _fieldName; }
                set { _fieldName = value; }
            }
            public string fieldType
            {
                get { return _fieldType; }
                set { _fieldType = value; }
            }

            public string string_;
            public bool bool_;
            public char char_;
            public short short_;
            public int int_;
            public float float_;
            public double double_;
            public Vector2 vector2_;
            public Vector2Int vector2Int_;
            public Vector3 vector3_;
            public Vector3Int vector3Int_;
            public Vector4 vector4_;
            public RangeInt rangeInt_;
            public Quaternion quaternion_;
            public Rect rect_;
            public RectInt rectInt_;
            public Matrix4x4 matrix4X4_;
            public Bounds bound_;
            public BoundsInt boundsInt_;
            public Color color_;
            public Color32 color32_;
            public GameObject gameObject_;
            public Component component_;
            public HotScriptAdapter adapter_;

            public string[] strings;
            public bool[] bools;
            public char[] chars;
            public short[] shorts;
            public int[] ints;
            public float[] floats;
            public double[] doubles;
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
            public GameObject[] gameObjects;
            public Component[] components;
            public HotScriptAdapter[] adapters;
        }
    }
}
