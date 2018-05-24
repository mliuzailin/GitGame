/**
 *  @file   MainMenuHeroDetail.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/21
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;

public class MainMenuHeroDetail : MainMenuSeq
{
    HeroDetail m_HeroDetail = null;
    HeroSelectButtonPanel m_HeroSelectButtonPanel = null;
    UnitSkillPanel m_UnitSkillPanel = null;


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

        //--------------------------------
        // オブジェクトの取得
        //--------------------------------
        m_HeroDetail = m_CanvasObj.GetComponentInChildren<HeroDetail>();
        m_HeroDetail.SetTopAndBottomAjustStatusBar(new Vector2(-65, -131));
        m_HeroSelectButtonPanel = m_CanvasObj.GetComponentInChildren<HeroSelectButtonPanel>();
        m_HeroSelectButtonPanel.SetSizeParfect(new Vector2(0, 78));
        m_UnitSkillPanel = m_CanvasObj.GetComponentInChildren<UnitSkillPanel>();

        m_HeroSelectButtonPanel.ReturnAction = OnClickNextButton;
        m_HeroSelectButtonPanel.IsActiveNext = false;

        PacketStructHero heroData = null;
        if (MainMenuParam.m_HeroCurrentInex >= 0 && MainMenuParam.m_HeroCurrentInex < UserDataAdmin.Instance.m_StructHeroList.Length)
        {
            heroData = UserDataAdmin.Instance.m_StructHeroList[MainMenuParam.m_HeroCurrentInex];
        }
        m_HeroDetail.SetDetail(heroData, OnClickStoryItem);

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.HOME;
    }

    /// <summary>
    /// ストーリのボタンが押されたとき
    /// </summary>
    /// <param name="story"></param>
	void OnClickStoryItem(HeroStoryListItemContext story)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
        newDialog.SetDialogText(DialogTextType.Title, story.StoryTitle);
        newDialog.SetDialogText(DialogTextType.MainText, story.ContentText);
        newDialog.Show();
    }


    void OnClickNextButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        MainMenuParam.m_HeroSelectReturn = true;
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HERO_FORM, false, true);
    }

}
