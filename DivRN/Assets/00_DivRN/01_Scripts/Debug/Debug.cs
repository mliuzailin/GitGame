using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
#else

public static class Debug
{
    static public void Break()
    {
        if (IsEnable())
            UnityEngine.Debug.Break();
    }

    static public void Log(object message)
    {
        if (IsEnable())
        {
            InternalLog(message);
        }
    }

    static public void LogAssertion(string t)
    {
        if (IsEnable())
        {
            InternalLog(t);
        }
    }
    static public void Assert(bool t)
    {
        if (IsEnable())
        {
            InternalLog(t);
        }

        UnityEngine.Debug.Assert(t);
    }
    public static void Assert(bool condition, Object context)
    {
#if BUILD_TYPE_DEBUG
        UnityEngine.Debug.Assert(condition, context);
#endif
    }
    public static void Assert(bool condition, object message)
    {
#if BUILD_TYPE_DEBUG
        UnityEngine.Debug.Assert(condition, message);
#endif
    }
    public static void Assert(bool condition, object message, Object context)
    {
#if BUILD_TYPE_DEBUG
        UnityEngine.Debug.Assert(condition, message, context);
#endif
    }

    static public void LogException(System.Exception e)
    {
        if (IsEnable())
        {
#if BUILD_TYPE_DEBUG
            UnityEngine.Debug.Log(e.Message);
#endif
        }
    }

    static public void Log(object message, Object context)
    {
        if (IsEnable())
        {
            InternalLog(message + " " + context.ToString());
        }
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
        if (IsEnable())
        {
#if BUILD_TYPE_DEBUG
            UnityEngine.Debug.LogErrorFormat(format, args);
#endif
        }
    }

    public static void LogErrorFormat(Object context, string format, params object[] args)
    {

        if (IsEnable())
        {
#if BUILD_TYPE_DEBUG
            UnityEngine.Debug.LogErrorFormat(context, format, args);
#endif
        }
    }

    static public void LogWarningFormat(object message, string context)
    {
        if (IsEnable())
        {
            InternalLog(message);
        }
    }

    static public void LogWarningFormat(object message, Object context)
    {
        if (IsEnable())
        {
            InternalLog(message);
        }
    }

    static public void LogWarning(object message)
    {
        if (IsEnable())
        {
            InternalLog(message);
        }
    }

    static public void LogWarning(object message, Object context)
    {
        if (IsEnable())
        {
            InternalLog(message);
        }
    }

    static public void LogError(object message)
    {
        if (IsEnable())
        {
            InternalLog(message);
        }
    }

    static public void LogError(object message, Object context)
    {
        if (IsEnable())
        {
            InternalLog(message);
        }
    }


    static public void DrawRay(Vector3 start, Vector3 end, Color color)
    {
        DrawRay(start, end, color, 0.0f, true);
    }

    static public void DrawRay(Vector3 start, Vector3 end, Color color, float duration)
    {
        DrawRay(start, end, color, duration, true);
    }

    static public void DrawRay(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
    {
        if (IsEnable())
        {
#if BUILD_TYPE_DEBUG
            UnityEngine.Debug.DrawRay(start, end, color, duration, depthTest);
#endif
        }
    }

    static public void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        DrawLine(start, end, color, 0.0f, true);
    }

    static public void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
        DrawLine(start, end, color, duration, true);
    }

    static public void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
    {
        if (IsEnable())
        {
#if BUILD_TYPE_DEBUG
           UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
#endif
        }
    }


    static public void LogFormat(string message, params object[] args)
    {
#if BUILD_TYPE_DEBUG
        UnityEngine.Debug.LogFormat(message, args);
#endif
    }


    static private void InternalLog(object message)
    {
#if BUILD_TYPE_DEBUG
        UnityEngine.Debug.Log(message);
#endif
    }

    static bool IsEnable()
    {
#if UNITY_EDITOR && BUILD_TYPE_DEBUG
        if (DebugOption.Instance == null)
        {
            return true;
        }
        return DebugOption.Instance.enableLog;
#else
        return false;
#endif
    }
}


#endif
