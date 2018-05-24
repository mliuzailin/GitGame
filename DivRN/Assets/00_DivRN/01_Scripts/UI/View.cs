using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;

// Viewクラス基底
public class View : M4uContextMonoBehaviour
{
    [SerializeField]
    protected GameObject m_root = null;
    [SerializeField]
    protected Animation m_animation = null;

    public static T Attach<T>(string prefabPath, GameObject parent) where T : View
    {
        var prefab = Resources.Load<GameObject>(prefabPath);
        UnityEngine.Debug.Assert(prefab != null, "The prefab " + prefabPath + " not found.");
        var entity = Instantiate<GameObject>(prefab);
        var view = entity.GetComponentInChildren<T>();
        UnityEngine.Debug.Assert(view != null, "The component View not found.");

        return view.SetParent<T>(parent);
    }

    public T SetParent<T>(GameObject parent) where T : View
    {
        if (parent == null)
            return (T)this;

        var localPosition = GetRoot().transform.localPosition;
        var localRotation = GetRoot().transform.localRotation;
        var localScale = GetRoot().transform.localScale;

        GetRoot().transform.SetParent(parent.transform);

        GetRoot().transform.localPosition = localPosition;
        GetRoot().transform.localRotation = localRotation;
        GetRoot().transform.localScale = localScale;
        return (T)this;
    }

    public void Detach()
    {
        GameObject.Destroy(GetRoot());
    }


    public GameObject GetRoot()
    {
        return m_root != null
            ? m_root
            : this.gameObject;
    }



    // ================================================= Animation =================================================

    private Dictionary<string, System.Action> m_animationFinishCallbackMap = new Dictionary<string, System.Action>();
    private Dictionary<string, System.Action> m_animationKeyEventCallbackMap = new Dictionary<string, System.Action>();
    virtual public void PlayAnimation(string animationName, System.Action finishCallback = null)
    {
        if (m_animation == null)
            return;

        m_animation.Stop();

        m_animation.cullingType = AnimationCullingType.AlwaysAnimate;
        m_animationFinishCallbackMap[animationName] = finishCallback;
        m_animation.Play(animationName, PlayMode.StopSameLayer);
    }

    // TODO : ...もうちょとイケてる感じに変える
    public void RegisterKeyEventCallback(string key, System.Action callback)
    {
        if (m_animation == null)
            return;

        m_animationKeyEventCallbackMap[key] = callback;
    }

    // animationのkey eventから呼ぶ
    virtual public void FinishAnimation(string animationName)
    {
        if (m_animationFinishCallbackMap.ContainsKey(animationName)
            && m_animationFinishCallbackMap[animationName] != null)
            m_animationFinishCallbackMap[animationName]();
    }

    // animationのkey eventから呼ぶ
    public void InvokeKeyEvent(string key)
    {
        if (m_animationKeyEventCallbackMap.ContainsKey(key)
            && m_animationKeyEventCallbackMap[key] != null)
            m_animationKeyEventCallbackMap[key]();
    }
}
