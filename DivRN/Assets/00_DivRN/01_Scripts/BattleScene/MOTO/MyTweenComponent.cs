using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DOTween の動作が不安定になる場合があるので独自に似た機能を作ってみた（一部機能のみ実装）
/// </summary>
public class MyTween
{
    public enum EaseType
    {
        LINEAR,
        IN_QUAD,
        OUT_QUAD,
        IN_OUT_QUAD,
    }

    private Transform m_OwnerTransform;
    private float m_Duration;
    private float m_WaitTime;
    private float m_Timer;
    private EaseType m_EaseType = EaseType.OUT_QUAD;
    private Vector3 m_StartLocalPosition;
    private Quaternion m_StartLocalRotation;
    private Vector3 m_StartLocalScale;
    private Vector3 m_GoalLocalPosition;
    private Quaternion m_GoalLocalRotation;
    private Vector3 m_GoalLocalScale;

    public MyTween(Transform owner_transform)
    {
        m_OwnerTransform = owner_transform;
        m_Duration = 0.0f;
    }

    public void tweenToParent(Transform parent_transform, float duration, EaseType ease_type = EaseType.OUT_QUAD, float wait_time = 0.0f)
    {
        m_Duration = duration;
        m_WaitTime = wait_time;
        if (duration > 0.0f)
        {
            m_Timer = 0.0f;
            m_EaseType = ease_type;
            m_OwnerTransform.SetParent(parent_transform, true);
            m_StartLocalPosition = m_OwnerTransform.localPosition;
            m_StartLocalRotation = m_OwnerTransform.localRotation;
            m_StartLocalScale = m_OwnerTransform.localScale;

            m_GoalLocalPosition = Vector3.zero;
            m_GoalLocalRotation = Quaternion.identity;
            m_GoalLocalScale = Vector3.one;
        }
        else if (m_WaitTime <= 0.0f)
        {
            m_OwnerTransform.SetParent(parent_transform, false);

            m_OwnerTransform.localPosition = Vector3.zero;
            m_OwnerTransform.localRotation = Quaternion.identity;
            m_OwnerTransform.localScale = Vector3.one;
        }
    }

    public void tween(Vector3 local_position, Quaternion local_rotation, Vector3 local_scale, float duration, EaseType ease_type = EaseType.OUT_QUAD, float wait_time = 0.0f)
    {
        m_Duration = duration;
        m_WaitTime = wait_time;
        if (duration > 0.0f)
        {
            m_Timer = 0.0f;
            m_EaseType = ease_type;
            m_StartLocalPosition = m_OwnerTransform.localPosition;
            m_StartLocalRotation = m_OwnerTransform.localRotation;
            m_StartLocalScale = m_OwnerTransform.localScale;

            m_GoalLocalPosition = local_position;
            m_GoalLocalRotation = local_rotation;
            m_GoalLocalScale = local_scale;
        }
        else if (m_WaitTime <= 0.0f)
        {
            m_OwnerTransform.localPosition = local_position;
            m_OwnerTransform.localRotation = local_rotation;
            m_OwnerTransform.localScale = local_scale;
        }
    }

    public void stopTween(bool is_complete = false)
    {
        if (m_Duration > 0.0f || m_WaitTime > 0.0f)
        {
            if (is_complete)
            {
                m_OwnerTransform.localPosition = m_GoalLocalPosition;
                m_OwnerTransform.localRotation = m_GoalLocalRotation;
                m_OwnerTransform.localScale = m_GoalLocalScale;
            }
            m_Duration = 0.0f;
            m_WaitTime = 0.0f;
        }
    }

    public void update(float delta_time)
    {
        if (m_WaitTime > 0.0f)
        {
            m_WaitTime -= delta_time;
            if (m_WaitTime > 0.0f)
            {
                return;
            }

            m_Duration += m_WaitTime;
            m_WaitTime = 0.0f;

            if (m_Duration <= 0.0f)
            {
                m_Duration = 0.0f;

                m_OwnerTransform.localPosition = m_GoalLocalPosition;
                m_OwnerTransform.localRotation = m_GoalLocalRotation;
                m_OwnerTransform.localScale = m_GoalLocalScale;

                return;
            }
        }

        if (m_Duration > 0.0f)
        {
            m_Timer += delta_time;
            if (m_Timer < m_Duration)
            {
                float rate = m_Timer / m_Duration;

                switch (m_EaseType)
                {
                    case EaseType.LINEAR:
                    default:
                        break;

                    case EaseType.IN_QUAD:
                        rate = _rate1In(rate);
                        rate = _rate2Quad(rate);
                        rate = _rate3In(rate);
                        break;

                    case EaseType.OUT_QUAD:
                        rate = _rate1Out(rate);
                        rate = _rate2Quad(rate);
                        rate = _rate3Out(rate);
                        break;

                    case EaseType.IN_OUT_QUAD:
                        rate = _rate1InOut(rate);
                        rate = _rate2Quad(rate);
                        rate = _rate3InOut(rate);
                        break;
                }

                m_OwnerTransform.localPosition = Vector3.Lerp(m_StartLocalPosition, m_GoalLocalPosition, rate);
                m_OwnerTransform.localRotation = Quaternion.Lerp(m_StartLocalRotation, m_GoalLocalRotation, rate);
                m_OwnerTransform.localScale = Vector3.Lerp(m_StartLocalScale, m_GoalLocalScale, rate);
            }
            else
            {
                m_Duration = 0.0f;

                m_OwnerTransform.localPosition = m_GoalLocalPosition;
                m_OwnerTransform.localRotation = m_GoalLocalRotation;
                m_OwnerTransform.localScale = m_GoalLocalScale;
            }
        }
    }

    private float _rate1In(float rate)
    {
        return rate;
    }

    private float _rate3In(float rate)
    {
        return rate;
    }

    private float _rate1Out(float rate)
    {
        return 1.0f - rate;
    }

    private float _rate3Out(float rate)
    {
        return 1.0f - rate;
    }

    private float _rate1InOut(float rate)
    {
        if (rate < 0.5f)
        {
            return rate * 2.0f;
        }
        else
        {
            return (1.0f - rate * 2.0f);
        }
    }

    private float _rate3InOut(float rate)
    {
        if (rate < 0.5f)
        {
            return rate * 0.5f;
        }
        else
        {
            return (1.0f - rate * 0.5f);
        }
    }

    private float _rate2Quad(float rate)
    {
        return rate * rate;
    }
}

/// <summary>
/// DOTween のように Transform のメソッドを拡張してみる.
/// </summary>
namespace MyTweenExt
{
    public static class ExtensionMethods
    {
        public static void tweenToParent(this Transform target_transform, Transform parent_transform, float duration, MyTween.EaseType ease_type = MyTween.EaseType.OUT_QUAD)
        {
            if (target_transform != null)
            {
                MyTweenComponent my_tween = target_transform.gameObject.GetComponent<MyTweenComponent>();
                if (my_tween == null)
                {
                    my_tween = target_transform.gameObject.AddComponent<MyTweenComponent>();
                    my_tween._init(target_transform);
                }

                my_tween.tweenToParent(parent_transform, duration, ease_type);
            }
        }

        public static void tween(this Transform target_transform, Vector3 local_position, Quaternion local_rotation, Vector3 local_scale, float duration, MyTween.EaseType ease_type = MyTween.EaseType.OUT_QUAD)
        {
            if (target_transform != null)
            {
                MyTweenComponent my_tween = target_transform.gameObject.GetComponent<MyTweenComponent>();
                if (my_tween == null)
                {
                    my_tween = target_transform.gameObject.AddComponent<MyTweenComponent>();
                    my_tween._init(target_transform);
                }

                my_tween.tween(local_position, local_rotation, local_scale, duration, ease_type);
            }
        }

        public static void killTween(this Transform target_transform, bool is_complete = false)
        {
            if (target_transform != null)
            {
                MyTweenComponent my_tween = target_transform.gameObject.GetComponent<MyTweenComponent>();
                if (my_tween != null)
                {
                    my_tween.stopTween(is_complete);
                }
            }
        }
    }
}

/// <summary>
/// 拡張メソッドを動作させるためのコンポーネント
/// </summary>
public class MyTweenComponent : MonoBehaviour
{
    private MyTween m_MyTween = null;

    private void Awake()
    {
        _init(transform);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        m_MyTween.update(Time.deltaTime);
    }

    public void _init(Transform transform)
    {
        m_MyTween = new MyTween(transform);
    }

    public void tweenToParent(Transform parent_transform, float duration, MyTween.EaseType ease_type = MyTween.EaseType.OUT_QUAD)
    {
        m_MyTween.tweenToParent(parent_transform, duration, ease_type);
    }

    public void tween(Vector3 local_position, Quaternion local_rotation, Vector3 local_scale, float duration, MyTween.EaseType ease_type = MyTween.EaseType.OUT_QUAD)
    {
        m_MyTween.tween(local_position, local_rotation, local_scale, duration, ease_type);
    }

    public void stopTween(bool is_complete = false)
    {
        m_MyTween.stopTween(is_complete);
    }
}

