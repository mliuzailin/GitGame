/**
 *  @file   MainMenuOthersWeb.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/06
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainMenuOthersWeb : MainMenuSeq
{
    OthersInfo m_OthersInfo;

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

        m_OthersInfo = GetComponentInChildren<OthersInfo>();
        m_OthersInfo.SetTopAndBottomAjustStatusBar(new Vector2(-8, -348));
        m_OthersInfo.LabelText = GameTextUtil.GetText("he168p_title");

        CreateInfoList();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.NONE;
    }

    void CreateInfoList()
    {
        m_OthersInfo.Infos.Clear();

        string info_url = MasterDataUtil.GetMasterDataGlobalParamTextFromID(GlobalDefine.WEB_LINK_INFORMATION);
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateWebViewWithCaptionKey("he168p_button1", info_url, OnClickInfoListItem));
        //string home_url = MasterDataUtil.GetMasterDataGlobalParamTextFromID(GlobalDefine.WEB_LINK_HP_DIVINE);
        //m_OthersInfo.Infos.Add(OthersInfoListContext.CreateWebViewWithCaptionKey("he168p_button2", home_url, OnClickInfoListItem));
        string mr_url = MasterDataUtil.GetMasterDataGlobalParamTextFromID(GlobalDefine.WEB_LINK_MR_DIVINE);
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateWebViewWithCaptionKey("he168p_button3", mr_url, OnClickInfoListItem));
    }

    void OnClickInfoListItem(OthersInfoListContext item)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        URLManager.OpenURL(item.URL);
    }

}
