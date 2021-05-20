using HotUnity;
using UnityEngine;
using UnityEngine.UI;

namespace HotProject
{
    public class Test2 : HotScript
    {
        public Test test;
        public string test2Text;
        public Image image;
        public Button button;

        public override void Awake()
        {
            button.onClick.AddListener(() =>
            {
                Debug.Log(test.testText);
                Debug.Log(test.testVec3);
                Debug.Log(test2Text);
                image.color = Random.ColorHSV();
            });
        }
    }
}
