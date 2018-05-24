using UnityEngine;
using System;
using System.Linq;
using UniExtensions;

public class Overlay<T> : SingletonComponent<T>
    where T : Component
{

    public void Parent(Transform childT)
    {
        childT.transform.parent = transform;
    }

    protected Action closeAction;


    public static T Create()
    {
        if (IsExists && typeof(T) != typeof(Dialog))
        {
            return GameObject.FindObjectOfType<T>().GetComponent<T>();
        }

        string prefabName = typeof(T).Name;
        GameObject prefab = null;
        if (prefab == null)
        {
            string prefabPath = "Prefab/Overlay/" + prefabName;
            prefab = Resources.Load(prefabPath) as GameObject;
        }
#if BUILD_TYPE_DEBUG
        Debug.Log("PREFAB_NAME:" + prefabName);
#endif
        GameObject go = Instantiate(prefab) as GameObject;
        go.transform.localScale = Vector3.one;
        T result = go.GetComponent<T>();
        return result;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    { 
        base.Start();
    }

    public virtual void Show(Action closeAction)
    {
        Show(0, closeAction);
    }

    public virtual void Show(float delay = 0, Action closeAction = null)
    {
        if (closeAction != null)
        {
            this.closeAction = closeAction;
        }

        if (delay > 0)
        {
            Hide(delay);
        }
    }

    protected virtual void OnEnable()
    { }

    protected virtual void OnDisable()
    { }

    protected virtual void Initialize()
    { }

    public virtual void Hide()
    {
        Hide(0);
    }

    public static void Destroy()
    {
        if (!IsExists)
        {
            return;
        }
        GameObject.Destroy(Instance.gameObject);
    }


    public virtual void Hide(float delay)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL HIDE:" + delay + " GO:" + gameObject.name);
#endif

        this.ExecuteLater(
            delay,
            () =>
            {
//                Debug.LogError("HIDE:" + closeAction);
                if (closeAction != null)
                {
                    closeAction();
//                    Debug.LogError("CLOSE");
                    closeAction = null;
                }

                Destroy(gameObject);
            });
    }
}
