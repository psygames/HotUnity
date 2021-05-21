using HotUnity;
using UnityEngine;

namespace HotProject
{
    /// <summary>
    /// 基本测试
    /// </summary>
    public class TestBasic : HotScript
    {
        public string testText;
        public Vector3 testVec3;
        public override void Awake()
        {
            Debug.Log(testText);
        }
    }
}
