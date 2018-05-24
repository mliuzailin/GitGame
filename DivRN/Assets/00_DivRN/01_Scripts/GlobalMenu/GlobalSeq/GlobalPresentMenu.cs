using ServerDataDefine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPresentMenu : GlobalMenuSeq
{
    private Present m_Present = null;

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
        if (m_Present == null)
        {
            m_Present = GetComponentInChildren<Present>();
            m_Present.SetPositionAjustStatusBar(new Vector2(0, -6), new Vector2(-20, -351));
        }

        // データの更新
        m_Present.initPresent();

        // シーンの最後に呼び出す
        //m_Present.PostSceneStart();
    }

}
