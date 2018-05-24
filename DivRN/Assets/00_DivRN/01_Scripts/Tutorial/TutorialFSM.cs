using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFSM : FSM<TutorialFSM>
{
    public void SendEvent_FinishStory()
    {
        SendFsmEvent("FINISH_STORY");
    }
    public void SendEvent_FinishBattle()
    {
        SendFsmEvent("FINISH_BATTLE");
    }

    public void SendEvent_FinishQuestResult()
    {
        SendFsmEvent("FINISH_QUEST_RESULT");
    }
}
