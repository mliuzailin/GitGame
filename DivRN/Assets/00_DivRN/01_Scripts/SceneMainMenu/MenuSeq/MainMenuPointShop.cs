using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// メインメニューの基本コード
/// </summary>
public class MainMenuPointShop : MainMenuSeq
{
    PointShop m_PointShop = null;
    bool m_bInit = false;

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
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.SHOP;

        StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
    }

    public override bool PageSwitchEventEnableBefore(bool bBack = false)
    {
        if (!m_bInit)
        {
            if (m_PointShop == null)
            {
                m_PointShop = GetComponentInChildren<PointShop>();
                m_PointShop.SetTopAndBottomAjustStatusBar(new Vector2(1, -266));
            }

            //ページ初期化処理
            m_PointShop.SceneStart();
            m_bInit = true;

        }
        return !m_PointShop.IsReady;
    }

    public override bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        m_bInit = false;
        return base.PageSwitchEventDisableAfter(eNextMainMenuSeq);
    }
}
