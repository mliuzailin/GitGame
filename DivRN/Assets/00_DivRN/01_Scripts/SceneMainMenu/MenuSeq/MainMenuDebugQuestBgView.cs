using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDebugQuestBGView : MainMenuSeq
{

    DebugQuestBGView m_DebugQuestBGView;

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

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        m_DebugQuestBGView = m_CanvasObj.GetComponentInChildren<DebugQuestBGView>();
        m_DebugQuestBGView.setup();
    }
}
