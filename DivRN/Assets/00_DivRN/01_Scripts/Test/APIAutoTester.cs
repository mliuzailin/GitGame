using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class APIAutoTester : SingletonComponent<APIAutoTester>
{
    private Action finishAction;

    private Queue<SERVER_API> unsendServerAPIQueue;
    private SERVER_API currentServerAPI;

    public void Play(Action _finishAction)
    {
        this.finishAction = _finishAction;
        APIAutoTesterFSM.Instance.SendFsmNextEvent();
    }

    void OnReady()
    {
        List<SERVER_API> list = Enum.GetValues(typeof(SERVER_API)).Cast<SERVER_API>().ToList();
        unsendServerAPIQueue = new Queue<SERVER_API>(list);
    }

    void OnSendAPI()
    {
        currentServerAPI = unsendServerAPIQueue.Dequeue();

        SceneServerAPITest.Instance.OnSendApi(currentServerAPI);

    }

    void OnExistsUnsendAPI()
    {
        if (unsendServerAPIQueue.Count > 0)
        {
            APIAutoTesterFSM.Instance.SendFsmPositiveEvent();
            return;
        }
        APIAutoTesterFSM.Instance.SendFsmNegativeEvent();
    }

    void OnFinish()
    {
        if (finishAction != null)
        {
            finishAction();
            finishAction = null;
        }
    }
}
