using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メインメニューの基本コード
/// </summary>
public class MainMenuLoginBonus : MainMenuSeq
{
    private LoginBonus m_LoginBonus = null;
    private bool m_bStart = false;

    // Use this for initialization
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

        if (!m_bStart)
        {
            m_bStart = true;

            m_LoginBonus.PostSceneStart();
            m_LoginBonus.CloseAction = OnClose;
        }
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //ページ初期化処理
        if (m_LoginBonus == null)
        {
            m_LoginBonus = GetComponentInChildren<LoginBonus>();
            m_LoginBonus.SetSizeParfect(new Vector2(0, 0));
        }

        m_bStart = false;
    }

    private void OnClose(MAINMENU_SEQ nextseq)
    {
        MainMenuManager.Instance.AddSwitchRequest(nextseq, false, false);
    }
}
