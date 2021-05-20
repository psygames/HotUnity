using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject testPrefab;

    private bool isInit = false;
    void Update()
    {
        if (isInit)
            return;
        if (ILRuntimeInit.isInitialized)
            Init();
    }

    void Init()
    {
        isInit = true;
        Instantiate(testPrefab).transform.SetParent(transform, false);
    }
}
