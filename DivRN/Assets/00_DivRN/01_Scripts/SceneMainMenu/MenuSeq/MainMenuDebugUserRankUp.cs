using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDebugUserRankUp : MainMenuSeq
{

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }

    }
}
