using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleFSM : FSM<TitleFSM>
{

    public void SendFsmEvent_DoError()
    {
        SendFsmEvent("DO_ERROR");
    }
}
