using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventScheduleMenu : GlobalMenuSeq
{
    private EventSchedule m_EventSchedule = null;

    protected override void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }

    protected override void PageSwitchSetting(bool bActive, bool bBack)
    {
        base.PageSwitchSetting(bActive, bBack);

        //--------------------------------
        // 以下は有効になったタイミングでの処理なので、
        // フェードアウト指示の場合にはスルー
        //--------------------------------
        if (bActive == false) { return; }

        //--------------------------------
        // 戻り処理の場合は再構築スルー
        //--------------------------------
        if (bBack == true) { return; }

        //ページ初期化処理
        if (m_EventSchedule == null)
        {
            m_EventSchedule = GetComponentInChildren<EventSchedule>();
            m_EventSchedule.SetPositionAjustStatusBar(new Vector2(0, -6), new Vector2(-20, -351));

        }
        // シーンの最後に呼び出す
        m_EventSchedule.PostSceneStart();
    }
}
