/**
 *  @file   StoryViewFSM.cs
 *  @brief
 *  @author Developer
 *  @date   2017/02/06
 */

using UnityEngine;
using System.Collections;
using System;

public class StoryViewFSM : FSM<StoryViewFSM>
{

    public void SendFsmSkipEvent()
    {
        SendFsmEvent("SKIP");
    }

    public void SendFsmErrorEvent()
    {
        SendFsmEvent("ERROR");
    }

    public override void SendFsmEvent(string eventName, float delay = 0)
    {
        // 一時停止を確認してからイベントを送る
        StartCoroutine(PauseWait(() =>
        {
            base.SendFsmEvent(eventName, delay);
        }));
    }

    IEnumerator PauseWait(Action finishAction)
    {
        while (IsPause())
        {
            yield return null;
        }
        if (finishAction != null)
        {
            finishAction();
        }
    }

    public bool IsPause()
    {
        return Dialog.HasDialog();
    }
}
