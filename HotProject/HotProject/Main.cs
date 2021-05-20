using UnityEngine;

namespace HotProject
{
    public class Main
    {
        public static void Start()
        {
            var obj = (GameObject)Resources.Load("Test");
            var parent = GameObject.Find("Canvas").transform;
            Object.Instantiate(obj).transform.SetParent(parent, false);
        }
    }
}
