/**
 *  @file   MainMenuHeroSelect.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/21
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;

public class MainMenuHeroSelect : MainMenuSeq
{
    /// <summary>主人公リスト</summary>
    HeroForm m_HeroForm = null;
    /// <summary>下部ボタン</summary>
    HeroSelectButtonPanel m_HeroSelectButtonPanel = null;
    /// <summary>API送信中かどうか</summary>
    private bool m_IsSendApi = false;

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

    public override bool PageSwitchEventDisableBefore()
    {
        base.PageSwitchEventDisableBefore();
        return false;
    }

    public override bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        base.PageSwitchEventDisableAfter(eNextMainMenuSeq);

        if (m_IsSendApi)
        {
            return true;
        }

        return false;

    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //--------------------------------
        // オブジェクトの取得
        //--------------------------------
        m_HeroForm = m_CanvasObj.GetComponentInChildren<HeroForm>();
        m_HeroForm.SetPositionAjustStatusBar(new Vector2(0, -20), new Vector2(0, -240));
        m_HeroSelectButtonPanel = m_CanvasObj.GetComponentInChildren<HeroSelectButtonPanel>();
        m_HeroSelectButtonPanel.SetSizeParfect(new Vector2(0, 78));

        m_HeroSelectButtonPanel.NextAction = OnClickNextButton;
        m_HeroSelectButtonPanel.IsActiveReturn = false;

        UnityUtil.SetObjectEnabledOnce(m_HeroSelectButtonPanel.gameObject, false);

        // データの設定
        int index;
        if (MainMenuParam.m_HeroSelectReturn)
        {
            index = MainMenuParam.m_HeroCurrentInex;
            MainMenuParam.m_HeroSelectReturn = false;
        }
        else
        {
            index = Array.FindIndex(UserDataAdmin.Instance.m_StructHeroList, v => v.unique_id == UserDataAdmin.Instance.m_StructPlayer.current_hero_id);
        }

        HeroForm.CreateFormDatas(OnClickFaceImage, (v) =>
        {
            UnityUtil.SetObjectLayer(gameObject, LayerMask.NameToLayer("DRAW_CLIP"));
            m_HeroForm.SetFormDatas(v, index);
        },
        () =>
        {
            UnityUtil.SetObjectLayer(gameObject, LayerMask.NameToLayer("GUI"));
            UnityUtil.SetObjectEnabledOnce(m_HeroSelectButtonPanel.gameObject, true);
        });

        m_HeroForm.OnClickNextButtonAction = OnClickFormNextButton;
        m_HeroForm.OnClickPreviousButtonAction = OnClickFormPreviousButton;
        m_HeroForm.OnClickDecisionButtonAction = OnClickDecisionButton;

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.HOME;
    }

    /// <summary>
    /// 戻るボタンを押したとき
    /// </summary>
    void OnClickFormPreviousButton()
    {
        if (m_HeroForm.m_CarouselRotator != null && m_HeroForm.m_CarouselToggler.moving == false)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            m_HeroForm.Step(false);
        }
    }

    /// <summary>
    /// 次ボタンを押したとき
    /// </summary>
    void OnClickFormNextButton()
    {
        if (m_HeroForm.m_CarouselRotator != null && m_HeroForm.m_CarouselToggler.moving == false)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            m_HeroForm.Step(true);
        }
    }

    /// <summary>
    /// 詳細ボタンが押されたとき
    /// </summary>
    void OnClickNextButton()
    {
        if (m_HeroForm.m_CarouselToggler.moving) { return; }

        // データ表示画面に移る
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        MainMenuParam.m_HeroCurrentInex = m_HeroForm.m_CurrentIndex;
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HERO_DETAIL, false, false);
    }

    /// <summary>
    /// 顔画像が押されたとき
    /// </summary>
    void OnClickFaceImage(HeroFormListContext form)
    {
        //	キャラ表示画面に移る
        //MainMenuParam.m_HeroCurrentInex = m_HeroForm.m_CurrentIndex;
        //MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HERO_PREVIEW, false, false);
    }

    /// <summary>
    /// 決定ボタンが押されたとき
    /// </summary>
    void OnClickDecisionButton()
    {
        if (m_HeroForm.m_CarouselToggler.moving) { return; }
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        SendSetCurrentHero();
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false);
    }

    #region API通信
    /// <summary>
    /// API送信：主人公選択設定
    /// </summary>
    void SendSetCurrentHero()
    {
        if (UserDataAdmin.Instance.m_StructHeroList != null)
        {
            if (UserDataAdmin.Instance.m_StructHeroList.Length > m_HeroForm.m_CurrentIndex)
            {
                PacketStructHero heroData = UserDataAdmin.Instance.m_StructHeroList[m_HeroForm.m_CurrentIndex];
                if (heroData != null)
                {
                    ServerDataUtilSend.SendPacketAPI_SetCurrentHero(heroData.hero_id, heroData.unique_id).setSuccessAction(_data =>
                    {
                        UserDataAdmin.Instance.m_StructHeroList = _data.GetResult<RecvSetCurrentHero>().result.hero_list;
                        UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvSetCurrentHero>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                        m_IsSendApi = false;
                    }).setErrorAction(_data =>
                    {
                        m_IsSendApi = false;
                    }).SendStart();

                    m_IsSendApi = true;
                }
            }
        }
    }
    #endregion
}
