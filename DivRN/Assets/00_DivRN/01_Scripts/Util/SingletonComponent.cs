using UnityEngine;
using System.Collections;
using M4u;

public abstract class SingletonComponent<T> : M4uContextMonoBehaviour
    where T : Component
{
    static T instance = null;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    public static bool HasInstance
    {
        get
        {
            return (instance != null);
        }
    }

    protected virtual void Awake()
    {
        instance = GetComponent<T>();
#if BUILD_TYPE_DEBUG
        Debug.Log("Awake:" + typeof(T).ToString());
#endif
    }

    protected virtual void Start()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("Start:" + typeof(T).ToString());
#endif
    }

    protected virtual void OnDestroy()
    {
        instance = null;
    }

    public static bool IsExists
    {
        get
        {
            return (instance != null);
        }
    }
}