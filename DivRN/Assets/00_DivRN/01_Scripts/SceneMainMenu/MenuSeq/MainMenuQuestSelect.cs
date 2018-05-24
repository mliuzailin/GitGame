using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class MainMenuQuestSelect : MainMenuSeq
{
    //----------------------------------------------------------------------------
    /*!
		@brief	エリア選択パラメータ
	*/
    //----------------------------------------------------------------------------
    public class AreaAmendParam
    {
        public TemplateList<MasterDataAreaAmend> m_AreaMasterDataAmendList = null;      //!< マスターデータ；エリア補正リスト

        public bool m_FlagAmendStamina = false;
        public int m_QuestSelectAreaAmendStamina = 100;     //!< 通常クエスト発行関連：エリア補正：スタミナ
        public bool m_FlagAmendEXP = false;
        public int m_QuestSelectAreaAmendEXP = 100;     //!< 通常クエスト発行関連：エリア補正：経験値
        public bool m_FlagAmendCoin = false;
        public int m_QuestSelectAreaAmendCoin = 100;        //!< 通常クエスト発行関連：エリア補正：コイン
        public bool m_FlagAmendDrop = false;
        public int m_QuestSelectAreaAmendDrop = 100;        //!< 通常クエスト発行関連：エリア補正：ドロップ率
        public bool m_FlagAmendTicket = false;
        public int m_QuestSelectAreaAmendTicket = 100;      //!< 通常クエスト発行関連：エリア補正：チケット
        public bool m_FlagAmendGuerrillaBoss = false;
        public int m_QuestSelectAreaAmendGuerrillaBoss = 100;       //!< 通常クエスト発行関連：エリア補正：ゲリラボス
        public bool m_FlagAmendLinkPoint = false;
        public int m_QuestSelectAreaAmendLinkPoint = 100;       //!< 通常クエスト発行関連：エリア補正：リンクポイント
        public bool m_FlagAmendScore = false;
        public int m_QuestSelectAreaAmendScore = 100;       //!< 通常クエスト発行関連：エリア補正：スコア

        //public bool m_QuestSelectAreaAmendFlag = false; //!< 通常クエスト発行関連：エリア補正フラグ
    }

    private EpisodeQuestSelect m_QuestSelect = null;
    private int m_UpdateLayoutCount = 0;

    private MasterDataAreaCategory m_MasterAreaCategory = null;
    private int m_SelectAreaIndex = -1;

    private Sprite m_BGSprite = null;
    private Sprite m_SelectSprite = null;
    private Sprite m_UnSelectSprite = null;

    private bool m_bReturnHome = false;

    private List<ListItemModel> m_questButtons = new List<ListItemModel>();
    private List<EpisodeDataListItemModel> m_episodeButtons = new List<EpisodeDataListItemModel>();

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        m_BGSprite = ResourceManager.Instance.Load("BG_color");
        m_SelectSprite = ResourceManager.Instance.Load("icon_circle_2", ResourceType.Common);
        m_UnSelectSprite = ResourceManager.Instance.Load("icon_circle_deco", ResourceType.Common);
    }

    public new void Update()
    {
        if (m_UpdateLayoutCount != 0)
        {
            m_UpdateLayoutCount--;
            if (m_UpdateLayoutCount < 0)
            {
                m_UpdateLayoutCount = 0;
            }

            m_QuestSelect.updateLayout();
            m_QuestSelect.checkMask();
        }

        if (PageSwitchUpdate() == false)
        {
            return;
        }

        //
        if (m_bReturnHome)
        {
            if (MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false))
            {
                m_bReturnHome = false;
            }
        }

        updateAreaFlag();
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //ページ初期化処理
        if (m_QuestSelect == null)
        {
            m_QuestSelect = m_CanvasObj.GetComponentInChildren<EpisodeQuestSelect>();
            m_QuestSelect.SetPositionAjustStatusBar(new Vector2(0, -25), new Vector2(0, -215));
        }

        m_SelectAreaIndex = -1;
        m_bReturnHome = false;

        //戻るボタン抑制
        SetSuspendReturn(true);
        SetUpButtons();
        setup();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.QUEST;

        StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
    }

    private bool setup()
    {
        m_MasterAreaCategory = MasterFinder<MasterDataAreaCategory>.Instance.Find((int)MainMenuParam.m_JmpQuestSelectAreaCateID);
        if (m_MasterAreaCategory == null)
        {
            openWarningAreaDialog();
            return false;
        }

        MainMenuParam.m_QuestSelectAreaCateID = MainMenuParam.m_JmpQuestSelectAreaCateID;

        List<MasterDataArea> masterAreaList = MasterFinder<MasterDataArea>.Instance.SelectWhere(" where area_cate_id = ? ", m_MasterAreaCategory.fix_id);
        masterAreaList.Sort((a, b) => (int)a.fix_id - (int)b.fix_id);
        MasterDataArea[] masterAreaArray = masterAreaList.ToArray();
        if (masterAreaArray == null)
        {
            openWarningAreaDialog();
            return false;
        }

        //BG
        //        m_QuestSelect.BackGroundImage = m_BGSprite;
        //AreaCategoryName
        m_QuestSelect.AreaTitle = m_MasterAreaCategory.area_cate_name;

        setupArea(masterAreaArray);

        //
        m_UpdateLayoutCount = 5;

        return true;
    }

    private void SetUpButtons()
    {
        var detailButtonFull = new ButtonModel();
        EpisodeDetailButton.Attach(m_QuestSelect.DetailButtonFullRoot).SetModel(detailButtonFull);
        detailButtonFull.OnClicked += () =>
        {
            OnClickDetailButton();
        };

        var detailButton = new ButtonModel();
        EpisodeDetailButton.Attach(m_QuestSelect.DetailButtonRoot).SetModel(detailButton);
        detailButton.OnClicked += () =>
        {
            OnClickDetailButton();
        };

        detailButtonFull.Appear();
        detailButtonFull.SkipAppearing();
        detailButton.Appear();
        detailButton.SkipAppearing();
    }

    /// <summary>
    /// エリア更新
    /// </summary>
    private void setupArea(MasterDataArea[] areaArray)
    {
        m_QuestSelect.EpisodeList.Clear();
        m_episodeButtons.Clear();
        uint _count = 0;

        for (int i = 0; i < areaArray.Length; i++)
        {
            bool hasArea = false;
            bool hasQuestCleard = true;
            bool hasQuestCompleted = true;
            bool hasQuestNew = false;
            MasterDataGuerrillaBoss guerrillaBoss = null;

            MainMenuUtil.ChkActiveArea(m_MasterAreaCategory, areaArray[i], ref hasArea, ref hasQuestCompleted, ref hasQuestCleard, ref hasQuestNew, ref guerrillaBoss);
            if (!hasArea)
            {
                continue;
            }

            uint index = _count;
            var model = new EpisodeDataListItemModel(index);
            model.OnClicked += () =>
            {
                SelectArea(index, true);
            };
            model.OnViewInstantidated += () =>
            {
                if (m_episodeButtons.Count > index + 1)
                {
                    return;
                }

                model.Appear();
            };
            model.OnShowedNext += () =>
            {
                if (index == 0)
                {
                    return;
                }

                m_episodeButtons[(int)index - 1].Appear();
            };

            EpisodeDataContext newEpisode = new EpisodeDataContext(model);
            newEpisode.master = areaArray[i];
            newEpisode.masterDataAreaCategory = m_MasterAreaCategory;
            newEpisode.amend = MainMenuUtil.CreateAreaParamAmend(newEpisode.master);
            newEpisode.m_EpisodeId = (uint)i + 1;
            newEpisode.IconImage = ResourceManager.Instance.Load("icon_city");
            newEpisode.SelectImage = m_UnSelectSprite;
            newEpisode.IsSelected = false;
            updateEpisodeFlag(newEpisode, hasQuestCleard, hasQuestCompleted, guerrillaBoss);
            updateEpsodeTime(newEpisode, newEpisode.master);
            m_QuestSelect.EpisodeList.Add(newEpisode);

            m_episodeButtons.Add(model);

            _count++;
        }

        //選択できるエリアがない
        if (m_QuestSelect.EpisodeList.Count == 0)
        {
            openWarningAreaDialog();
            return;
        }

        //指定エリア選択
        if (MainMenuParam.m_JmpQuestSelectAreaID != 0)
        {
            for (int i = 0; i < m_QuestSelect.EpisodeList.Count; i++)
            {
                if (m_QuestSelect.EpisodeList[i].master.fix_id == MainMenuParam.m_JmpQuestSelectAreaID)
                {
                    SelectArea((uint)i);
                    return;
                }
            }
        }

        //指定がないときは一番下のエリア選択
        SelectArea((uint)(m_QuestSelect.EpisodeList.Count - 1));
    }


    void updateEpisodeFlag(EpisodeDataContext newEpisode, bool hasQuestCleard, bool hasQuestCompleted, MasterDataGuerrillaBoss guerrillaBoss)
    {
        newEpisode.flagImageList.Clear();
        newEpisode.flagTextList.Clear();

        //フラグ関連
        {
            if (hasQuestCompleted)
            {
                newEpisode.flagImageList.Add(ResourceManager.Instance.Load("completed"));
                newEpisode.flagTextList.Add("");
            }
            else if (hasQuestCleard)
            {
                newEpisode.flagImageList.Add(ResourceManager.Instance.Load("clear"));
                newEpisode.flagTextList.Add("");
            }

            if (guerrillaBoss != null)
            {
                newEpisode.flagImageList.Add(ResourceManager.Instance.Load("flag_warn"));
                newEpisode.flagTextList.Add("");
            }

            if (newEpisode.amend != null)
            {
                if (newEpisode.amend.m_FlagAmendCoin)
                {
                    newEpisode.flagImageList.Add(ResourceManager.Instance.Load("flag_coin"));
                    newEpisode.flagTextList.Add(getStringFlagRate(newEpisode.amend.m_QuestSelectAreaAmendCoin));
                }
                if (newEpisode.amend.m_FlagAmendDrop)
                {
                    newEpisode.flagImageList.Add(ResourceManager.Instance.Load("flag_drop"));
                    newEpisode.flagTextList.Add(getStringFlagRate(newEpisode.amend.m_QuestSelectAreaAmendDrop));
                }
                if (newEpisode.amend.m_FlagAmendEXP)
                {
                    newEpisode.flagImageList.Add(ResourceManager.Instance.Load("flag_exp"));
                    newEpisode.flagTextList.Add(getStringFlagRate(newEpisode.amend.m_QuestSelectAreaAmendEXP));
                }
                if (newEpisode.amend.m_FlagAmendTicket)
                {
                    newEpisode.flagImageList.Add(ResourceManager.Instance.Load("flag_ticket"));
                    newEpisode.flagTextList.Add(getStringFlagRate(newEpisode.amend.m_QuestSelectAreaAmendTicket));
                }
                if (newEpisode.amend.m_FlagAmendStamina)
                {
                    newEpisode.flagImageList.Add(ResourceManager.Instance.Load("flag_stm"));
                    newEpisode.flagTextList.Add("");
                }
                if (newEpisode.amend.m_FlagAmendGuerrillaBoss)
                {
                    newEpisode.flagImageList.Add(ResourceManager.Instance.Load("flag_boss"));
                    newEpisode.flagTextList.Add("");
                }
                if (newEpisode.amend.m_FlagAmendLinkPoint)
                {
                    newEpisode.flagImageList.Add(ResourceManager.Instance.Load("flag_link"));
                    newEpisode.flagTextList.Add(getStringFlagRate(newEpisode.amend.m_QuestSelectAreaAmendLinkPoint));
                }
            }
            if (newEpisode.flagImageList.Count != 0)
            {
                newEpisode.IsActiveFlag = true;
                newEpisode.flagCounter = 0;
                newEpisode.FlagImage = newEpisode.flagImageList[newEpisode.flagCounter];
                newEpisode.FlagRate = newEpisode.flagTextList[newEpisode.flagCounter];
            }
            else
            {
                newEpisode.IsActiveFlag = false;
            }
        }

    }

    private void updateEpsodeTime(EpisodeDataContext newEpisode, MasterDataArea area)
    {
        //残り時間計算
        newEpisode.Time = "";
        if (area.event_id != 0)
        {
            uint unTimingEnd = MainMenuUtil.GetEventTimingEnd(area.event_id);

            if (unTimingEnd != 0)
            {
                DateTime endTime = TimeUtil.GetDateTime(unTimingEnd);
                DateTime nowTime = TimeManager.Instance.m_TimeNow;
                TimeSpan leftTime = endTime - nowTime;
                newEpisode.Time = GameTextUtil.GetRemainStr(leftTime, GameTextUtil.GetText("general_time_01"));
            }
        }
    }

    private void setupQuest()
    {
        m_QuestSelect.isEndShowList = false;
        m_QuestSelect.QuestList.Clear();
        m_questButtons.Clear();

        if (m_SelectAreaIndex >= m_QuestSelect.EpisodeList.Count)
        {
            return;
        }

        EpisodeDataContext episodeData = m_QuestSelect.EpisodeList[m_SelectAreaIndex];

        MasterDataArea areaMaster = episodeData.master;
        if (areaMaster == null)
        {
            return;
        }

        List<MasterDataQuest2> quest2List = MasterFinder<MasterDataQuest2>.Instance.SelectWhere("where area_id = ?", areaMaster.fix_id);
        quest2List.Sort((a, b) => (int)a.fix_id - (int)b.fix_id);
        MasterDataQuest2[] quest2Array = quest2List.ToArray();
        if (quest2Array == null)
        {
            return;
        }

        MainMenuParam.m_QuestStaminaAmend = (float)episodeData.amend.m_QuestSelectAreaAmendStamina / 100.0f;

        //クエストリスト更新
        {
            uint _count = 0;
            uint _notClearCount = 0;
            bool bSkip = false;
            for (int i = 0; i < quest2Array.Length; i++)
            {
                uint _index = _count;
                MasterDataQuest2 _masterQuest2 = quest2Array[i];

                if (_masterQuest2.active != MasterDataDefineLabel.BoolType.ENABLE)
                {
                    continue;
                }

                if (_masterQuest2.story != 0 && _notClearCount != 0)
                {
                    //未クリアクエスト以降のシナリオはスキップ
                    bSkip = true;
                }
                else if (_masterQuest2.story == 0 &&
                    !ServerDataUtil.ChkRenewBitFlag(ref UserDataAdmin.Instance.m_StructPlayer.flag_renew_quest_clear, quest2Array[i].fix_id))
                {
                    //１つめの未クリアは表示
                    if (_notClearCount != 0)
                    {
                        bSkip = true;
                    }

                    _notClearCount++;
                }

                if (bSkip)
                {
                    continue;
                }

                var model = new ListItemModel(_index);
                model.OnClicked += () =>
                {
                    SelectQuest(_index);
                };
                m_questButtons.Add(model);

                QuestDataContext newQuest = new QuestDataContext(model);
                newQuest.master = _masterQuest2;
                newQuest.area_category_id = m_MasterAreaCategory.fix_id;
                newQuest.boss = MasterDataUtil.GetGuerrillaBossParamFromQuestID(_masterQuest2.fix_id);
                newQuest.m_QuestId = _index + 1;

                if (_masterQuest2.story == 0)
                {
                    newQuest.m_QuestType = QuestDataContext.ExecType.Quest2;
                    newQuest.IconLabel = GameTextUtil.GetText("questselect_text1");
                    {
                        //詳細情報テキスト差し替え
                        MasterDataQuestAppearance[] questAppearance = MasterFinder<MasterDataQuestAppearance>.Instance.SelectWhere("where area_category_id = ?", MainMenuParam.m_QuestSelectAreaCateID).ToArray();
                        if (questAppearance.IsNullOrEmpty() == false)
                        {
                            // newQuest.IconLabel = GameTextUtil.GetText(questAppearance[0].boss_text_key);
                            // テキストキーではなく直接テキストが入っている
                            newQuest.IconLabel = questAppearance[0].boss_text_key;
                        }
                    }
                    UnitIconImageProvider.Instance.Get(
                        _masterQuest2.boss_chara_id,
                        sprite => { newQuest.IconImage = sprite; });
                }
                else
                {
                    newQuest.m_QuestType = QuestDataContext.ExecType.Event;
                    newQuest.IconLabel = GameTextUtil.GetText("questselect_text2");
                    newQuest.IconImage = ResourceManager.Instance.Load("storyicon");
                }

                string titleFormat = GameTextUtil.GetText("questselect_questname1");
                if (newQuest.boss != null)
                {
                    titleFormat = GameTextUtil.GetText("questselect_questname2");
                }

                newQuest.Title = string.Format(titleFormat, _masterQuest2.quest_name);
                newQuest.Index = _index;
                newQuest.SelectImage = m_SelectSprite;

                //                newQuest.BackGroundTexture = m_QuestSelect.BackGroundImage.texture;

                newQuest.IsActivePoint = false;
                switch (_masterQuest2.consume_type)
                {
                    case 1://スタミナ
                        newQuest.IsActivePoint = true;
                        newQuest.PointLabel = GameTextUtil.GetText("questselect_text3");
                        if (episodeData.amend.m_QuestSelectAreaAmendStamina == 100)
                        {
                            newQuest.m_Point = (uint)_masterQuest2.consume_value;
                            newQuest.Point = string.Format("{0}", _masterQuest2.consume_value);
                        }
                        else
                        {
                            uint point = (uint)((float)_masterQuest2.consume_value * ((float)episodeData.amend.m_QuestSelectAreaAmendStamina / 100.0f));
                            newQuest.m_Point = point;
                            newQuest.Point = string.Format(GameTextUtil.GetText("stmina_bahutext"), point);
                        }

                        //スコア倍率アップ
                        newQuest.AmendText = "";
                        if (_masterQuest2.story == 0 &&
                            _masterQuest2.consume_value != 0 &&
                            episodeData.amend.m_FlagAmendScore)
                        {
                            float score_rate = (float)episodeData.amend.m_QuestSelectAreaAmendScore / 100.0f;
                            newQuest.AmendText = string.Format("スコア {0:0.0}倍", score_rate);
                        }
                        break;
                    case 2://Key
                        {
                            newQuest.IsActivePoint = true;
                            newQuest.m_Point = (uint)_masterQuest2.consume_value;
                            string strFormat = GameTextUtil.GetText("questselect_text5");
                            MasterDataQuestKey _keyMaster = MasterDataUtil.GetMasterDataQuestKeyFromAreaCategoryID(m_MasterAreaCategory.fix_id);
                            if (_keyMaster != null)
                            {
                                newQuest.PointLabel = string.Format(strFormat, _keyMaster.key_name);
                            }
                            newQuest.Point = string.Format("{0}", _masterQuest2.consume_value);
                        }
                        break;
                    case 3://Ticket
                        newQuest.IsActivePoint = true;
                        newQuest.m_Point = (uint)_masterQuest2.consume_value;
                        newQuest.PointLabel = GameTextUtil.GetText("questselect_text4");
                        newQuest.Point = string.Format("{0}", _masterQuest2.consume_value);
                        break;
                }

                newQuest.SetFlag(quest2Array[i].fix_id);

                m_QuestSelect.QuestList.Add(newQuest);

                _count++;
            }

            //-------------------------------------------------
            // イベントスケジュールがあるが検索
            //-------------------------------------------------
            MasterDataEvent eventMaster = MasterDataUtil.GetMasterDataEventFromID(areaMaster.event_id);
            m_QuestSelect.m_EventMaster = eventMaster;
            if (eventMaster != null
                && eventMaster.event_schedule_show == MasterDataDefineLabel.BoolType.ENABLE)
            {
                m_QuestSelect.IsViewDetailButton = true;
            }
            else
            {
                m_QuestSelect.IsViewDetailButton = false;
            }
        }


        // View更新
        foreach (var episodeButton in m_episodeButtons)
        {
            episodeButton.HideArrow();
        }

        // インジケーターを表示
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.ASSETBUNDLE);
        }
        m_QuestSelect.Show(episodeData.masterDataAreaCategory.fix_id, () =>
        {
            foreach (var questButton in m_questButtons)
            {
                questButton.Appear();
            }

            foreach (var episodeButton in m_episodeButtons)
            {
                if (episodeButton.isSelected)
                {
                    episodeButton.ShowArrow();
                }
                else
                {
                    episodeButton.HideArrow();
                }
            }
            //戻るボタン抑制解除
            SetSuspendReturn(false);

            StartCoroutine(WaitShowQuestList(() =>
            {
                m_QuestSelect.isEndShowList = true;
            }));
        }, () =>
        {
            // インジケーターを閉じる
            if (LoadingManager.Instance != null)
            {
                LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.ASSETBUNDLE);
            }
        });
    }

    IEnumerator WaitShowQuestList(Action finishAction)
    {
        while (m_QuestSelect.QuestList.Count != m_QuestSelect.QuestObjList.Count)
        {
            yield return null;
        }
        m_QuestSelect.QuestListAlpha = 1;
        while (CheckQuestListShowAll() == false)
        {
            yield return null;
        }

        if (finishAction != null)
        {
            finishAction();
        }
    }

    bool CheckQuestListShowAll()
    {
        for (int i = 0; i < m_questButtons.Count; ++i)
        {
            if (m_questButtons[i].isReady == false)
            {
                return false;
            }

        }
        return true;
    }

    /// <summary>
    /// エリア選択
    /// </summary>
    /// <param name="area_id"></param>
    private void SelectArea(uint area_id, bool bSE = false)
    {
        if (m_SelectAreaIndex == (int)area_id)
        {
            return;
        }

        if (bSE)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK);
        }

        if (m_SelectAreaIndex != -1)
        {
            m_QuestSelect.EpisodeList[m_SelectAreaIndex].SelectImage = m_UnSelectSprite;
            m_QuestSelect.EpisodeList[m_SelectAreaIndex].IsSelected = false;
        }
        m_SelectAreaIndex = (int)area_id;

        m_QuestSelect.EpisodeList[m_SelectAreaIndex].SelectImage = m_SelectSprite;
        m_QuestSelect.EpisodeList[m_SelectAreaIndex].IsSelected = true;

        m_QuestSelect.EpisodeTitle = m_QuestSelect.EpisodeList[m_SelectAreaIndex].master.area_name;

        setupQuest();

        m_UpdateLayoutCount = 5;

        //選択保存
        MainMenuParam.SetSaveSelectNormal(m_MasterAreaCategory.fix_id, m_QuestSelect.EpisodeList[m_SelectAreaIndex].master.fix_id);
    }

    /// <summary>
    /// クエスト決定
    /// </summary>
    /// <param name="quest_id"></param>
    private void SelectQuest(uint quest_index)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        EpisodeDataContext selectArea = m_QuestSelect.EpisodeList[m_SelectAreaIndex];
        QuestDataContext selectQuest = m_QuestSelect.QuestList[(int)quest_index];

        switch (m_QuestSelect.QuestList[(int)quest_index].m_QuestType)
        {
            case QuestDataContext.ExecType.Quest:
                break;

            case QuestDataContext.ExecType.Quest2:
                {
                    {
                        MainMenuParam.m_QuestStamina = 0;
                        MainMenuParam.m_QuestKey = 0;
                        MainMenuParam.m_QuestTicket = 0;
                        switch (selectQuest.master.consume_type)
                        {
                            case 1:
                                MainMenuParam.m_QuestStamina = selectQuest.m_Point;
                                break;
                            case 2:
                                MainMenuParam.m_QuestKey = selectQuest.m_Point;
                                break;
                            case 3:
                                MainMenuParam.m_QuestTicket = selectQuest.m_Point;
                                break;
                        }

                        MainMenuParam.m_QuestAreaAmendList = selectArea.amend.m_AreaMasterDataAmendList;
                    }
                    if (MainMenuManager.HasInstance)
                    {
                        MainMenuParam.m_QuestSelectAreaID = selectArea.master.fix_id;
                        MainMenuParam.m_QuestSelectMissionID = selectQuest.master.fix_id;
                        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_DETAIL, false, false);
                    }
                }
                break;

            case QuestDataContext.ExecType.Event:
                {

                    //----------------------------------------
                    // パラメータリミットチェック
                    //----------------------------------------
                    //チェック対象：コイン、チケット、フレンドポイント
                    PRM_LIMIT_ERROR_TYPE ret = MainMenuUtil.ChkPrmLimit(1, 1, 1, 0, 0);
                    //チェック対象：消費アイテム全て
                    ret = MainMenuUtil.ChkPrmLimitItem(1, -1, ret);
                    //チェック対象：クエストキー全て
                    ret = MainMenuUtil.ChkPrmLimitQuestKey(1, -1, ret);

                    MainMenuUtil.ShowParamLimitDialog(ret, DialogType.DialogYesNo, (isPositive) =>
                    {
                        if (isPositive == true)
                        {

                            //----------------------------------------
                            // ストーリー画面の表示
                            //----------------------------------------
                            StoryView cutin = StoryView.Create();
                            cutin.SetScenario(selectQuest.master.story, selectQuest.master.fix_id);
                            cutin.SetReloadQuestListEvent(() =>
                            {
                                int hasAreaCount = 0;
                                bool hasSelectArea = false;
                                selectQuest.SetFlag(selectQuest.master.fix_id);
                                updateEpisodeListInfo(ref hasAreaCount, ref hasSelectArea);
                                // 表示しているエリアが期限切れ
                                if (hasSelectArea == false)
                                {
                                    openWarningAreaDialog();
                                }
                            });
                            cutin.Show(() =>
                            {
#if BUILD_TYPE_DEBUG
                                Debug.Log("StoryView Completed");
#endif
                            });
                        }
                    });

                }
                break;
        }
    }

    private float m_UpdateTimeCount = 0.0f;

    private void updateAreaFlag()
    {
        m_UpdateTimeCount += Time.deltaTime;
        if (m_UpdateTimeCount >= 2.0f)
        {
            m_UpdateTimeCount = 0.0f;
            for (int i = 0; i < m_QuestSelect.EpisodeList.Count; i++)
            {
                if (!m_QuestSelect.EpisodeList[i].IsActiveFlag)
                {
                    continue;
                }

                if (m_QuestSelect.EpisodeList[i].flagImageList.Count == 1)
                {
                    continue;
                }

                m_QuestSelect.EpisodeList[i].flagCounter++;
                if (m_QuestSelect.EpisodeList[i].flagCounter >= m_QuestSelect.EpisodeList[i].flagImageList.Count) m_QuestSelect.EpisodeList[i].flagCounter = 0;
                m_QuestSelect.EpisodeList[i].FlagImage = m_QuestSelect.EpisodeList[i].flagImageList[m_QuestSelect.EpisodeList[i].flagCounter];
                m_QuestSelect.EpisodeList[i].FlagRate = m_QuestSelect.EpisodeList[i].flagTextList[m_QuestSelect.EpisodeList[i].flagCounter];
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="hasAreaCount"></param>
    /// <param name="hasSelectArea"></param>
    private void updateEpisodeListInfo(ref int hasAreaCount, ref bool hasSelectArea)
    {
        hasAreaCount = 0;
        hasSelectArea = false;

        for (int i = 0; i < m_QuestSelect.EpisodeList.Count; ++i)
        {
            updateEpsodeTime(m_QuestSelect.EpisodeList[i], m_QuestSelect.EpisodeList[i].master);

            bool hasArea = false;
            bool hasQuestCleard = true;
            bool hasQuestCompleted = true;
            bool hasQuestNew = false;
            MasterDataGuerrillaBoss guerrillaBoss = null;

            MainMenuUtil.ChkActiveArea(m_MasterAreaCategory, m_QuestSelect.EpisodeList[i].master, ref hasArea, ref hasQuestCompleted, ref hasQuestCleard, ref hasQuestNew, ref guerrillaBoss);

            updateEpisodeFlag(m_QuestSelect.EpisodeList[i], hasQuestCleard, hasQuestCompleted, guerrillaBoss);

            if (hasArea == true)
            {
                if (i == m_SelectAreaIndex)
                {
                    hasSelectArea = true;
                }

                hasAreaCount++;
            }
        }
    }

    public override bool PageSwitchEventEnableBefore(bool bBack = false)
    {
        if (m_QuestSelect != null && !bBack)
        {
            m_QuestSelect.QuestList.Clear();
            m_QuestSelect.resetMask();
            m_UpdateLayoutCount = 5;
        }
        return base.PageSwitchEventEnableBefore();
    }

    public override bool PageSwitchEventEnableAfter()
    {
        //戻るアクション設定
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.SetMenuReturnAction(true, SelectReturn);
        }

        if (m_QuestSelect != null)
        {
            m_QuestSelect.UpdateUV();
        }

        return base.PageSwitchEventEnableAfter();
    }

    public override bool PageSwitchEventDisableBefore()
    {
        //戻るアクション解除
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.SetMenuReturnAction(false, null);
        }
        return base.PageSwitchEventDisableBefore();
    }

    public void SelectReturn()
    {
        if (!MainMenuManager.HasInstance)
        {
            return;
        }

        MainMenuParam.m_RegionID = m_MasterAreaCategory.region_id;
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_STORY, false, false);
    }

    /// <summary>
    /// 詳細ボタンを押したとき
    /// </summary>
    void OnClickDetailButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        MainMenuParam.m_DialogEventScheduleData = m_QuestSelect.m_EventMaster;
        MainMenuManager.Instance.Header.OpenGlobalMenu(GLOBALMENU_SEQ.EVENTSCHEDULE);
    }

    public void openWarningAreaDialog()
    {
        //表示OFF
        UnityUtil.SetObjectEnabledOnce(m_QuestSelect.gameObject, false);
        //Homeへ戻る
        Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
        _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "kk111q_title");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "kk111q_content");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        _newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
        {
            m_bReturnHome = true;
        });
        _newDialog.DisableCancelButton();
        _newDialog.Show();
    }

    private string getStringFlagRate(int rate)
    {
        string ret = "";
        if ((rate % 100) != 0)
        {
            ret = ((float)rate / 100).ToString("F1");
        }
        else
        {
            ret = (rate / 100).ToString();
        }
        return ret;
    }
}
