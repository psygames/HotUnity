using HotUnity;
using UnityEngine;
using UnityEngine.UI;

namespace HotProject
{
    /// <summary>
    /// 脚本引用测试
    /// </summary>
    public class TestRefer : HotScript
    {
        public TestBasic test;
        public Image image;
        public Button button;

        public override void Awake()
        {
            button.onClick.AddListener(() =>
            {
                Debug.Log(test.testVec3);
                image.color = Random.ColorHSV();
            });
        }
    }
}
