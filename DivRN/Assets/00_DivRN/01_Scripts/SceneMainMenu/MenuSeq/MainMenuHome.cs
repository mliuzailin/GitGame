using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;

public class MainMenuHome : MainMenuSeq
{
    private HomeMenu m_HomeMenu = null;
    private MenuBanner m_MenuBanner = null;
    private List<MasterDataScoreEvent> scoreEventList = null;
    private List<MasterDataChallengeEvent> challengeEventList = null;
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        MainMenuManager.Instance.EnableBackKey();
    }

    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }

    private bool m_bStartApi = false;
    private bool m_bEndApi = false;
    private RecvGetTopicInfoValue m_RecvData = null;
    private bool m_bStartLoadImg = false;
    private bool m_bEndLoadImg = false;
    private Sprite m_HeroImage;
    private Texture m_HeroImage_mask;
    private ScoreEventDialog m_ScoreDialog = null;

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        if (m_HomeMenu == null)
        {
            m_HomeMenu = m_CanvasObj.GetComponentInChildren<HomeMenu>();
            m_HomeMenu.SetPositionAjustStatusBar(new Vector2(0, -25), new Vector2(0, -215));
            m_HomeMenu.DidSelectStory = changeQuestStory;
            m_HomeMenu.DidSelectMaster = changeMaster;
            m_HomeMenu.DidSelectChallenge = changeChallengeSelect;

            m_HomeMenu.DidSelectGoodInfo = openGoodInfo;
            m_HomeMenu.DidSelectBossInfo = openBossInfo;
            m_HomeMenu.DidSelectUnitInfo = openUnitInfo;
            m_HomeMenu.DidSelectScoreInfo = openScoreInfo;
            m_HomeMenu.DidSelectMission = openMission;
            m_HomeMenu.DidSelectPresent = openPresent;

        }
        m_HomeMenu.Initialize();
        m_HomeMenu.HeroImage = m_HeroImage;
        m_HomeMenu.HeroImage_mask = m_HeroImage_mask;

        if (m_MenuBanner == null)
        {
            m_MenuBanner = m_CanvasObj.GetComponentInChildren<MenuBanner>();
            m_MenuBanner.SetPositionAjustStatusBar(new Vector2(-125, -218));
            m_MenuBanner.bannerSetup();
        }

        //イベント開催中フラグ
        bool bFlag = false;
        if (m_RecvData != null && m_RecvData.pickup != null)
        {
            PacketStructPickup _info = m_RecvData.pickup;
            if (_info.text != null)
            {
                for (int i = 0; i < _info.text.Length; i++)
                {
                    if (!ChkTiming(_info.text[i].timing_start, _info.text[i].timing_end))
                    {
                        continue;
                    }

                    if (_info.text[i].event_flag != 0)
                    {
                        bFlag = true;
                    }
                }
            }

        }
        m_HomeMenu.IsActiveEventFlag = bFlag;

        //スコアイベントチェック
        m_HomeMenu.IsViewScoreInfo = false;
        scoreEventList = MasterDataUtil.GetActiveScoreEvent();
        if (scoreEventList.Count != 0)
        {
            m_HomeMenu.IsViewScoreInfo = true;
        }

        //成長ボスイベントチェック
        m_HomeMenu.IsViewChallenge = false;
        challengeEventList = MasterDataUtil.GetActiveChallengeEvent();
        if (challengeEventList.Count != 0)
        {
            MasterDataAreaCategory areaCategory = MasterDataUtil.GetChallengeAreaCategoryMaster(challengeEventList[0].event_id);
            if (areaCategory != null)
            {
                m_HomeMenu.ChallengeButton.LoadIcon(areaCategory.fix_id);
                m_HomeMenu.IsViewChallenge = true;
            }
        }

        //
        updateFlag();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.HOME;

        StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
    }

    public void changeQuestStory()
    {
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_STORY, false, false);
        }
    }

    public void changeMaster()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HERO_FORM, false, false);
        }
    }

    public void openGoodInfo()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (m_RecvData == null ||
            m_RecvData.pickup == null
            )
        {
            return;
        }
        PacketStructPickup _info = m_RecvData.pickup;
        Dialog _newDialog = Dialog.Create(DialogType.DialogScrollInfo);
        _newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("otoku_dialog_title"));
        if (_info.image != null)
        {
            for (int i = 0; i < _info.image.Length; i++)
            {
                if (!ChkTiming(_info.image[i].timing_start, _info.image[i].timing_end))
                {
                    continue;
                }

                _newDialog.AddScrollInfoImage(String.Format("{0}/{1}", GlobalDefine.GetBaseBannerUrl(), _info.image[i].url));
                break;
            }
        }
        if (_info.text != null)
        {
            for (int i = 0; i < _info.text.Length; i++)
            {
                if (!ChkTiming(_info.text[i].timing_start, _info.text[i].timing_end))
                {
                    continue;
                }

                _newDialog.AddScrollInfoText(_info.text[i].message);
            }
        }
        _newDialog.Show();
    }

    public void openBossInfo()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (m_RecvData == null ||
            m_RecvData.guerrilla == null
            )
        {
            return;
        }
        PacketStructGuerrilla _info = m_RecvData.guerrilla;

        Dialog _newDialog = Dialog.Create(DialogType.DialogScrollInfo);
        _newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("gerira_dialog_title"));
        if (_info.image != null)
        {
            for (int i = 0; i < _info.image.Length; i++)
            {
                if (!ChkTiming(_info.image[i].timing_start, _info.image[i].timing_end))
                {
                    continue;
                }

                _newDialog.AddScrollInfoImage(String.Format("{0}/{1}", GlobalDefine.GetBaseBannerUrl(), _info.image[i].url));
                break;
            }
        }
        if (_info.text != null)
        {
            for (int i = 0; i < _info.text.Length; i++)
            {
                if (!ChkTiming(_info.text[i].timing_start, _info.text[i].timing_end))
                {
                    continue;
                }
                _newDialog.AddScrollInfoText(_info.text[i].message);
            }
        }
        _newDialog.Show();
    }

    private void openUnitInfo()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        if (m_RecvData == null ||
            m_RecvData.new_unit == null
            )
        {
            return;
        }
        PacketStructNewUnit _info = m_RecvData.new_unit;
        Dialog _newDialog = Dialog.Create(DialogType.DialogScrollInfo);
        _newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("newunit_dialog_title"));
        if (_info.image != null)
        {
            for (int i = 0; i < _info.image.Length; i++)
            {
                if (!ChkTiming(_info.image[i].timing_start, _info.image[i].timing_end))
                {
                    continue;
                }

                _newDialog.AddScrollInfoImage(String.Format("{0}/{1}", GlobalDefine.GetBaseBannerUrl(), _info.image[i].url));

                break;
            }
        }
        if (_info.icon != null)
        {
            for (int i = 0; i < _info.icon.Length; i++)
            {
                if (!ChkTiming(_info.icon[i].timing_start, _info.icon[i].timing_end))
                {
                    continue;
                }
                _newDialog.AddScrollInfoIconList(_info.icon[i].title, _info.icon[i].ids);
            }
        }
        _newDialog.Show();

    }
    private void openScoreInfo()
    {
        if (m_ScoreDialog != null ||
             ServerApi.IsExists)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        m_ScoreDialog = ScoreEventDialog.Create(SceneObjReferMainMenu.Instance.m_MainMenuGroupCamera.GetComponent<Camera>());

        int[] event_ids = new int[scoreEventList.Count];
        for (int i = 0; i < scoreEventList.Count; i++)
        {
            event_ids[i] = (int)scoreEventList[i].event_id;
        }
        ServerDataUtilSend.SendPacketAPI_GetUserScoreInfo(event_ids)
        .setSuccessAction((data) =>
        {
            RecvGetUserScoreInfo scoreInfo = data.GetResult<RecvGetUserScoreInfo>();
            if (scoreInfo != null &&
                scoreInfo.result != null &&
                scoreInfo.result.score_infos != null)
            {

                for (int i = 0; i < scoreInfo.result.score_infos.Length; i++)
                {
                    PacketStructUserScoreInfo Info = scoreInfo.result.score_infos[i];
                    if (Info == null)
                    {
                        continue;
                    }

                    MasterDataScoreEvent scoreEventMaster = scoreEventList.Find((m) => m.event_id == Info.event_id);
                    if (scoreEventMaster == null)
                    {
                        continue;
                    }

                    m_ScoreDialog.addScoreInfo(Info, scoreEventMaster);
                }


                bool isTutorial = (LocalSaveManagerRN.Instance.GetIsShowTutorialDialog(TutorialDialog.FLAG_TYPE.SCORE) == false);
                if (isTutorial)
                {
#if BUILD_TYPE_DEBUG
                    Debug.LogError(string.Format("チュートリアルを表示する FLAG_TYPE:{0}", TutorialDialog.FLAG_TYPE.SCORE.ToString()));
#endif
                    TutorialDialog.Create().SetTutorialType(TutorialDialog.FLAG_TYPE.SCORE).Show(() =>
                    {
                        isTutorial = false;
                    });
                }

                m_HomeMenu.IsViewScoreInfo = false;
                m_ScoreDialog.Show(() =>
                {
                    m_ScoreDialog = null;
                    m_HomeMenu.IsViewScoreInfo = true;
                });
            }
            else
            {
                DestroyObject(m_ScoreDialog.gameObject);
                m_ScoreDialog = null;

                Dialog errorDialog = Dialog.Create(DialogType.DialogOK);
                errorDialog.SetDialogText(DialogTextType.Title, "警告");
                errorDialog.SetDialogText(DialogTextType.MainText, "スコアイベントは開催されていません。");
                errorDialog.SetDialogText(DialogTextType.OKText, "閉じる");
                errorDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
                {
                    m_HomeMenu.IsViewScoreInfo = false;
                });
                errorDialog.Show();
            }
        })
        .setErrorAction((data) =>
        {
            DestroyObject(m_ScoreDialog.gameObject);
            m_ScoreDialog = null;
        })
        .SendStart();
    }

    private void openMission()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        MainMenuHeader header = MainMenuManager.Instance.Header;
        header.OpenGlobalMenu(GLOBALMENU_SEQ.MISSION);
    }

    private void openPresent()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        MainMenuHeader header = MainMenuManager.Instance.Header;
        header.OpenGlobalMenu(GLOBALMENU_SEQ.PRESENT);
    }

    private void changeChallengeSelect()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        if (MainMenuManager.HasInstance)
        {
            MainMenuParam.SetChallengeSelectParam(0);
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_CHALLENGE_SELECT, false, false);
        }
    }

    public override bool PageSwitchEventEnableBefore(bool bBack = false)
    {
        bool bEnable = base.PageSwitchEventEnableBefore();
        // アセットバンドルの読み込み
        if (!m_bStartLoadImg)
        {
            m_bStartLoadImg = true;
            uint currentHeroID = MasterDataUtil.GetCurrentHeroID();
            string assetname = string.Format("tex_hero_perform_l_{0:D4}", currentHeroID);
            AssetBundler.Create().Set(string.Format("hero_{0:D4}", currentHeroID), assetname, (o) =>
            {
                Texture2D texture = o.GetTexture2D(assetname, TextureWrapMode.Clamp);
                if (texture != null)
                {
                    m_HeroImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    m_HeroImage_mask = o.GetTexture(assetname + "_mask", TextureWrapMode.Clamp);
                }
                if (m_HomeMenu != null)
                {
                    m_HomeMenu.HeroImage = m_HeroImage;
                    m_HomeMenu.HeroImage_mask = m_HeroImage_mask;
                }
                m_bEndLoadImg = true;
            }, (s) =>
            {
                m_bEndLoadImg = true;
            }).Load();
        }

#if false//トピック情報を表示するボタンは削除されたので情報取得APIもコメントアウトする。
        if (!m_bStartApi)
        {
            ServerDataUtilSend.SendPacketAPI_GetTopicInfo()
            .setSuccessAction(_data =>
            {
                m_RecvData = _data.GetResult<RecvGetTopicInfo>().result.Clone<RecvGetTopicInfoValue>();
                m_bEndApi = true;
            })
            .SendStart();
            m_bStartApi = true;
        }
        bEnable = !m_bEndApi && !m_bEndLoadImg;
#else
        bEnable = !m_bEndLoadImg;
#endif
        return bEnable;
    }

    public override bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        m_bStartLoadImg = false;
        m_bEndLoadImg = false;
        m_HeroImage = null;
        m_HeroImage_mask = null;

        MainMenuParam.m_BannerLastIndexHome = m_MenuBanner.banner.carouselRotator.CurrentIndex;

        if (m_ScoreDialog != null)
        {
            m_ScoreDialog.Hide();
        }

        m_bStartApi = false;
        m_bEndApi = false;
        m_RecvData = null;

        return base.PageSwitchEventDisableAfter(eNextMainMenuSeq);
    }

    private bool ChkTiming(uint _start, uint _end)
    {
        bool bTimeCheck = false;


        if (_start == 0 && _end == 0)
        {
            bTimeCheck = true;
        }
        else if (_start == 0)
        {
            //----------------------------------------
            // 期間内チェック
            // 終了期限のみの指定時用
            //----------------------------------------
            bTimeCheck = TimeManager.Instance.CheckWithinTime(_end);
        }
        else
        {
            //----------------------------------------
            // 期間内チェック
            //----------------------------------------
            bTimeCheck = TimeManager.Instance.CheckWithinTime(_start, _end);
        }
        return bTimeCheck;
    }

    private void updateFlag()
    {
        bool bPresent = UserDataAdmin.Instance.GetUserFlag(UserDataAdmin.UserFlagType.GlobalPresent);

        bool bMission = false;
        if (UserDataAdmin.Instance.GetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionDaily) ||
            UserDataAdmin.Instance.GetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionEvent) ||
            UserDataAdmin.Instance.GetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionNormal))
        {
            bMission = true;
        }

        setFlag(bPresent, bMission);
    }

    public void setFlag(bool bPresent, bool bMission)
    {
        m_HomeMenu.IsActivePresentFlag = bPresent;
        m_HomeMenu.IsActiveMissionFlag = bMission;
    }
}
