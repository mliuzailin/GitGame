using UnityEngine;
using System.Collections;
using System.ComponentModel;

public class MainMenuManagerFSM : FSM<MainMenuManagerFSM>
{
    public bool IsTutorialNext
    {
        get
        {
            return ActiveStateName.Equals("TutorialNext");
        }
    }

    public bool IsPageWaitLoop
    {
        get
        {
            return ActiveStateName.Equals("PageWaitLoop");
        }
    }

    public void SendEvent_ChangeSeq()
    {
        SendFsmEvent("CHANGE_SEQ");
    }
}