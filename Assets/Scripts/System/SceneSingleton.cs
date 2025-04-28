using UnityEngine;

public abstract class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected virtual void Awake()
    {
        if (InstanceRegister.Get<T>() != null)
        {
            Destroy(gameObject);
            return;
        }
        InstanceRegister.Add(this as T);
    }

    protected virtual void OnDestroy()
    {
        InstanceRegister.Remove<T>();
    }
}
