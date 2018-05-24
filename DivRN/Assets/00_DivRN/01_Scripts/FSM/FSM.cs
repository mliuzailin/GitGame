using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using HutongGames.PlayMaker;
using UniExtensions;

public abstract class FSM<T> : SingletonComponent<T>
    where T : Component
{
    [SerializeField]
    private PlayMakerFSM fsm;


    public virtual PlayMakerFSM FsmComponent
    {
        get
        {
            if (fsm == null)
            {
                fsm = GetComponent<PlayMakerFSM>();
            }
            return fsm;
        }
    }

    public void Reset()
    {
        FsmComponent.Reset();
    }

    public string ActiveStateName
    {
        get
        {
            return FsmComponent.ActiveStateName;
        }
    }

    public void SendFsmEventWithInt(string eventName, string key, int value)
    {
        SetFsmInt(key, value);
        SendFsmEvent(eventName);
    }

    public void SendFsmEventWithIntAndVector3(string eventName, string key1, int value1, string key2, Vector3 value2)
    {
        SetFsmInt(key1, value1);
        SetFsmVector3(key2, value2);
        SendFsmEvent(eventName);
    }

    public void SendFsmEventWithVector3(string eventName, string key, Vector3 value)
    {
        SetFsmVector3(key, value);
        SendFsmEvent(eventName);
    }

    //	public void SendFsmEventWithVector2 (string eventName,string key,Vector2 value)
    //	{
    //		SetFsmVector2(key,value);
    //		SendFsmEvent(eventName);
    //	}

    public void SendFsmEventWithObject(string eventName, string key, UnityEngine.Object value)
    {
        SetFsmObject(key, value);
        SendFsmEvent(eventName);
    }

    public bool GetFsmBool(string key)
    {
        return FsmComponent.FsmVariables.GetFsmBool(key).Value;
    }

    public void SetFsmBool(string key, bool value)
    {
        FsmComponent.FsmVariables.GetFsmBool(key).Value = value;
    }


    public string GetFsmString(string key)
    {
        return FsmComponent.FsmVariables.GetFsmString(key).Value;
    }

    public int GetFsmInt(string key)
    {
        return FsmComponent.FsmVariables.GetFsmInt(key).Value;
    }


    public virtual void SendFsmNextEvent(float delay = 0)
    {
        string beforeActievStateName = ActiveStateName;
        SendFsmEvent("DO_NEXT", delay);
#if BUILD_TYPE_DEBUG
        //        Debug.Log("[FSM]CALL SendFsmNextEvent name:" + gameObject.name + "beforeState:" + beforeActievStateName + " afterState:" + ActiveStateName);
#endif
    }

    public void SendFsmEvent_FailQuit()
    {
        SendFsmEvent("FAIL_QUIT");
    }

    public void SendFsmEvent_FailRetry()
    {
        SendFsmEvent("FAIL_RETRY");
    }

    public void SendFsmEvent_Success()
    {
        SendFsmEvent("SUCCESS");
    }

    public void SendFsmApplicationFocus()
    {
        SendFsmEvent("APPLICATION_FOCUS");
    }


    public void SendFsmPositiveEvent(float delay = 0)
    {
        SendFsmEvent("DO_POSITIVE", delay);
    }

    public void SendFsmNegativeEvent(float delay = 0)
    {
        SendFsmEvent("DO_NEGATIVE", delay);
    }


    public void SetFsmString(string key, string value)
    {
        FsmComponent.FsmVariables.GetFsmString(key).Value = value;
    }

    public void SetFsmInt(string key, int value)
    {
        FsmComponent.FsmVariables.GetFsmInt(key).Value = value;
    }

    public void SetFsmObject(string key, UnityEngine.Object value)
    {
        FsmComponent.FsmVariables.GetFsmObject(key).Value = value;
    }

    public void SetFsmVector2(string key, Vector2 value)
    {
        FsmComponent.FsmVariables.GetFsmVector2(key).Value = value;
    }

    public void SetFsmVector3(string key, Vector3 value)
    {
        FsmComponent.FsmVariables.GetFsmVector3(key).Value = value;
    }

    public virtual void SendFsmEvent(string eventName, float delay = 0)
    {
        Action action = () =>
        {
            string beforeActievStateName = ActiveStateName;
            FsmComponent.SendEvent(eventName);
#if BUILD_TYPE_DEBUG
            Debug.Log("[FSM]CALL SendFsmEvent goName:" + name + " event:" + eventName + " before:" + beforeActievStateName + " after:" + ActiveStateName);
#endif
        };

        if (delay == 0)
        {
            action();
            return;
        }
        this.ExecuteLater(delay, action);
    }
}
