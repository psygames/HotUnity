using UnityEngine;

namespace HotUnity
{
    public class HotScript
    {
        public GameObject gameObject { get; private set; }
        public Transform transform => gameObject.transform;
        public string name
        {
            get { return gameObject.name; }
            set { gameObject.name = value; }
        }

        public virtual void Awake() { }
        public virtual void OnEnable() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void FixedUpdate() { }
        public virtual void OnGUI() { }
        public virtual void OnDisable() { }
        public virtual void OnDestroy() { }
    }
}
