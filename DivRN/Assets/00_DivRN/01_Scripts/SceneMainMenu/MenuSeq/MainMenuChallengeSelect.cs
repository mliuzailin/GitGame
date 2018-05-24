using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class MainMenuChallengeSelect : MainMenuSeq
{
    private readonly float AREA_FLAG_CHANGE_TIME = 2.0f;
    private readonly uint DISP_MIN_STAMINA = 1;

    private ChallengeSelect m_ChallengeSelect = null;
    private ChallengeQuestInfo m_ChallengeQuestInfo = null;

    //エリアフラグ関連
    private MainMenuQuestSelect.AreaAmendParam m_AreaAmendParam = null;
    private float m_AreaFlagTime = 0.0f;
    private int m_CurrentAreaFlag = 0;
    private List<Sprite> m_AreaFlagSprits = new List<Sprite>();
    private List<string> m_AreaFlagValue = new List<string>();

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

        //エリアフラグ更新
        updateAreaFlag();
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        if (m_ChallengeSelect == null)
        {
            m_ChallengeSelect = m_CanvasObj.GetComponentInChildren<ChallengeSelect>();
            m_ChallengeSelect.OnChengedBoss = OnSelectBoss;
            m_ChallengeSelect.OnSelectOk = OnSelectOk;
            m_ChallengeSelect.OnSelectReward = OnSelectReward;
            m_ChallengeSelect.OnSelectRule = OnSelectRule;
            m_ChallengeSelect.OnSelectBossAtr = OnSelectBossAtr;
            m_ChallengeSelect.OnSelectSkip = OnSelectSkip;

            m_ChallengeSelect.SetPositionAjustStatusBar(new Vector2(0, 0), new Vector2(0, 0));
        }

        if (m_ChallengeQuestInfo == null)
        {
            m_ChallengeQuestInfo = m_CanvasObj.GetComponentInChildren<ChallengeQuestInfo>();
            m_ChallengeQuestInfo.CountLabel = GameTextUtil.GetText("growth_boss_05");
            m_ChallengeQuestInfo.ExpLabel = GameTextUtil.GetText("growth_boss_06");
            m_ChallengeQuestInfo.CoinLabel = GameTextUtil.GetText("growth_boss_07");
            m_ChallengeQuestInfo.BossLabel = GameTextUtil.GetText("growth_boss_04");
            m_ChallengeQuestInfo.ScoreLabel = GameTextUtil.GetText("growth_boss_08");
        }

        if (initalize)
        {
            m_ChallengeSelect.Panel.transform.localScale = new Vector3(1, 0, 1);
            UnityUtil.SetObjectEnabledOnce(m_ChallengeQuestInfo.gameObject, false);

            sendGetUserChallenge();
        }
        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.QUEST;
    }

    public override bool PageSwitchEventEnableAfter()
    {
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.SetMenuReturnAction(true, OnSelectReturn);
        }
        return base.PageSwitchEventEnableAfter();
    }

    /// <summary>
    /// 成長ボス情報取得API呼び出し
    /// </summary>
    private void sendGetUserChallenge()
    {
        List<MasterDataChallengeEvent> eventList = MasterDataUtil.GetActiveChallengeEvent();
        if (eventList == null ||
            eventList.Count == 0)
        {
            //有効なデータがない
            openReturnHomeDialog();
            return;
        }

        int[] event_ids = new int[eventList.Count];
        for (int i = 0; i < eventList.Count; i++)
        {
            event_ids[i] = (int)eventList[i].event_id;
        }

        ServerDataUtilSend.SendPacketAPI_GetChallengeInfo(event_ids)
        .setSuccessAction((data) =>
        {
            //
            m_ChallengeSelect.ClearAll();

            RecvGetChallengeInfo challengeInfo = data.GetResult<RecvGetChallengeInfo>();
            if (challengeInfo != null &&
                challengeInfo.result != null &&
                challengeInfo.result.challenge_infos != null)
            {

                for (int i = 0; i < challengeInfo.result.challenge_infos.Length; i++)
                {
                    PacketStructChallengeInfo info = challengeInfo.result.challenge_infos[i];
                    if (info == null)
                    {
                        continue;
                    }

                    MasterDataChallengeEvent master = eventList.Find((m) => m.event_id == info.event_id);
                    if (master == null)
                    {
                        continue;
                    }

                    m_ChallengeSelect.AddEventData(master, info);
                }
            }
            if (m_ChallengeSelect.Events.Count != 0)
            {
                uint select_event_id = m_ChallengeSelect.Events[0].eventMaster.event_id;

                //
                if (MainMenuParam.m_ChallengeQuestMissionID != 0 &&
                    MasterDataUtil.GetQuestType(MainMenuParam.m_ChallengeQuestMissionID) == MasterDataDefineLabel.QuestType.CHALLENGE)
                {
                    MasterDataChallengeQuest master = (MasterDataChallengeQuest)MasterDataUtil.GetQuest2ParamFromID(MainMenuParam.m_ChallengeQuestMissionID);
                    if (master != null)
                    {
                        select_event_id = (uint)master.event_id;
                    }
                }

                m_ChallengeSelect.setup(select_event_id);
            }
            else
            {
                //有効なデータがない
                openReturnHomeDialog();
            }
        })
        .setErrorAction((data) =>
        {
            //エラー
        })
        .SendStart();
    }

    /// <summary>
    /// 成長ボス選択
    /// </summary>
    /// <param name="data"></param>
    private void OnSelectBoss(ChallengeSelect.EventData data)
    {
        //期間
        updateTerm(data);

        //エリア効果
        updateAreaAmend(data.questMaster.area_id);

        //クエスト情報更新
        updateQuestInfo(data);

        //報酬フラグ
        updateRewardFlag(data);

        //選択保存
        MainMenuParam.SetSaveSelectChallenge(data.questMaster.fix_id);

        //有効化
        UnityUtil.SetObjectEnabledOnce(m_ChallengeQuestInfo.gameObject, true);
    }

    /// <summary>
    /// 期間情報更新
    /// </summary>
    /// <param name="data"></param>
    private void updateTerm(ChallengeSelect.EventData data)
    {
        if (TimeEventManager.Instance.ChkEventActive(data.eventMaster.event_id) == true)
        {
            //開催中
            MasterDataEvent eventData = MasterDataUtil.GetMasterDataEventFromID(data.eventMaster.event_id);
            DateTime startTime = TimeUtil.GetDateTime(eventData.timing_start);
            string kikanFormat = GameTextUtil.GetText("growth_boss_01");
            if (eventData.timing_end != 0)
            {
                DateTime endTime = TimeUtil.GetDateTime(eventData.timing_end);
                endTime = endTime.SubtractAMinute();
                m_ChallengeSelect.Time = string.Format(kikanFormat, startTime.ToString("yyyy/MM/dd(HH:mm)"), endTime.ToString("yyyy/MM/dd(HH:mm)"));
            }
            else
            {
                m_ChallengeSelect.Time = string.Format(kikanFormat, startTime.ToString("yyyy/MM/dd(HH:mm)"), "");
            }

            m_ChallengeSelect.IsActiveSkipButton = true;
            m_ChallengeSelect.IsActiveOkButton = true;

            MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("challenge_quest_marquee_open"));
        }
        else
        {
            //イベント終了
            if (data.eventMaster.receiving_end != 0)
            {
                DateTime endTime = TimeUtil.GetDateTime(data.eventMaster.receiving_end);
                endTime = endTime.SubtractAMinute();
                string kikanFormat = GameTextUtil.GetText("growth_boss_02");
                m_ChallengeSelect.Time = string.Format(kikanFormat, endTime.ToString("yyyy/MM/dd(HH:mm)"));
            }
            else
            {
                m_ChallengeSelect.Time = GameTextUtil.GetText("growth_boss_03");
            }

            m_ChallengeSelect.IsActiveSkipButton = false;
            m_ChallengeSelect.IsActiveOkButton = false;

            MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("challenge_quest_marquee_end"));
        }
    }

    /// <summary>
    /// クエスト情報更新
    /// </summary>
    /// <param name="data"></param>
    private void updateQuestInfo(ChallengeSelect.EventData data)
    {
        MasterDataParamChara bossMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)data.questMaster.boss_chara_id);

        //ボスアイコン
        UnitIconImageProvider.Instance.Get(
            data.questMaster.boss_chara_id,
            (sprite) =>
            {
                m_ChallengeSelect.IconImage = sprite;
            });

        //ボス属性
        if (bossMaster != null)
        {
            m_ChallengeSelect.ElementImage = MainMenuUtil.GetElementCircleSprite(bossMaster.element);
        }

        //イベント名
        m_ChallengeSelect.Title = data.eventMaster.title;
        //ボス名
        if (bossMaster != null)
        {
            m_ChallengeQuestInfo.BossName = bossMaster.name;
        }

        //レベル
        if (data.bSkip)
        {
            m_ChallengeSelect.Level = string.Format("{0}", data.SkipLevel);

        }
        else
        {
            m_ChallengeSelect.Level = string.Format("{0}", data.info.challenge_level);
        }

        //経験値
        uint exp = (uint)data.questMaster.clear_exp;
        string expFormat = GameTextUtil.GetText("value_colorset");
        if (m_AreaAmendParam.m_QuestSelectAreaAmendEXP != 100)
        {
            // エリア補正時
            exp = (uint)((float)exp * (m_AreaAmendParam.m_QuestSelectAreaAmendEXP / 100.0f));
            expFormat = GameTextUtil.GetText("stmina_bahutext");
        }

        //コイン
        uint coin = (uint)data.questMaster.clear_money;
        string coinFormat = GameTextUtil.GetText("value_colorset");
        if (m_AreaAmendParam.m_QuestSelectAreaAmendCoin != 100)
        {
            // エリア補正時
            coin = (uint)((float)coin * (m_AreaAmendParam.m_QuestSelectAreaAmendCoin / 100.0f));
            coinFormat = GameTextUtil.GetText("stmina_bahutext");
        }

        //消費スタミナ
        uint stamina = 0;
        string staminaFormat = GameTextUtil.GetText("value_colorset");
        MainMenuParam.m_QuestStamina = 0;
        switch (data.questMaster.consume_type)
        {
            case 1://スタミナ
                m_ChallengeQuestInfo.StaminaLabel = GameTextUtil.GetText("growth_boss_09"); ;
                stamina = (uint)data.questMaster.consume_value;
                break;
            case 2://Key
                //m_ChallengeQuestInfo.StaminaLabel = "KEY";
                //MainMenuParam.m_QuestKey = (uint)data.questMaster.consume_type;
                break;
            case 3://Ticket
                //m_ChallengeQuestInfo.StaminaLabel = "TICKET";
                //MainMenuParam.m_QuestTicket = (uint)data.questMaster.consume_type;
                break;
        }
        if (m_AreaAmendParam.m_QuestSelectAreaAmendStamina != 100 &&
            stamina != 0)
        {
            // エリア補正時
            stamina = (uint)((float)stamina * ((float)m_AreaAmendParam.m_QuestSelectAreaAmendStamina / 100.0f));
            staminaFormat = GameTextUtil.GetText("stmina_bahutext");

            //最少スタミナチェック
            if (stamina <= DISP_MIN_STAMINA)
            {
                stamina = DISP_MIN_STAMINA;
            }
        }

        MainMenuParam.m_QuestStamina = stamina;

        //消費チケット(成長ボスではスキップにチケットを使用する)
        MainMenuParam.m_QuestTicket = (data.bSkip == true ? (uint)data.UseTicket : 0);

        //消費キー（成長ボスではキー消費はいまのところなし）
        MainMenuParam.m_QuestKey = 0;

        //スコア情報
        uint score = 0;
        string scoreFormat = GameTextUtil.GetText("value_colorset");
        MasterDataRenewQuestScore scoreMaster = MasterFinder<MasterDataRenewQuestScore>.Instance.Find((int)data.questMaster.quest_score_id);
        if (scoreMaster != null)
        {
            score = (uint)scoreMaster.base_score;
        }
        if (m_AreaAmendParam.m_QuestSelectAreaAmendScore != 100)
        {
            // エリア補正時
            score = (uint)((float)score * (m_AreaAmendParam.m_QuestSelectAreaAmendScore / 100.0f));
            scoreFormat = GameTextUtil.GetText("stmina_bahutext");
        }

        m_ChallengeQuestInfo.CountValue = string.Format(GameTextUtil.GetText("value_colorset"), data.questMaster.battle_count);
        m_ChallengeQuestInfo.ExpValue = string.Format(expFormat, exp);
        m_ChallengeQuestInfo.CoinValue = string.Format(coinFormat, coin);
        m_ChallengeQuestInfo.StaminaValue = string.Format(staminaFormat, stamina);
        m_ChallengeQuestInfo.ScoreValue = string.Format(scoreFormat, score);

    }

    private void updateRewardFlag(ChallengeSelect.EventData data)
    {
        if (data.info.get_list != null &&
            data.info.get_list.Length > 0)
        {
            m_ChallengeSelect.IsViewRewardFlag = true;
        }
        else
        {
            m_ChallengeSelect.IsViewRewardFlag = false;
        }
    }

    private void updateAreaAmend(uint area_id)
    {
        MasterDataArea areaMaster = MasterFinder<MasterDataArea>.Instance.Find((int)area_id);
        if (areaMaster == null)
        {
            return;
        }

        m_AreaAmendParam = MainMenuUtil.CreateAreaParamAmend(areaMaster);

        m_ChallengeSelect.IsViewFlag = false;
        m_AreaFlagTime = 0.0f;
        m_CurrentAreaFlag = 0;
        m_AreaFlagSprits.Clear();
        m_AreaFlagValue.Clear();

        if (m_AreaAmendParam.m_FlagAmendCoin)
        {
            m_AreaFlagSprits.Add(ResourceManager.Instance.Load("flag_coin"));
            m_AreaFlagValue.Add(getStringFlagRate(m_AreaAmendParam.m_QuestSelectAreaAmendCoin));
        }
        if (m_AreaAmendParam.m_FlagAmendDrop)
        {
            m_AreaFlagSprits.Add(ResourceManager.Instance.Load("flag_drop"));
            m_AreaFlagValue.Add(getStringFlagRate(m_AreaAmendParam.m_QuestSelectAreaAmendDrop));
        }
        if (m_AreaAmendParam.m_FlagAmendEXP)
        {
            m_AreaFlagSprits.Add(ResourceManager.Instance.Load("flag_exp"));
            m_AreaFlagValue.Add(getStringFlagRate(m_AreaAmendParam.m_QuestSelectAreaAmendEXP));
        }
        if (m_AreaAmendParam.m_FlagAmendTicket)
        {
            m_AreaFlagSprits.Add(ResourceManager.Instance.Load("flag_ticket"));
            m_AreaFlagValue.Add(getStringFlagRate(m_AreaAmendParam.m_QuestSelectAreaAmendTicket));
        }
        if (m_AreaAmendParam.m_FlagAmendStamina)
        {
            m_AreaFlagSprits.Add(ResourceManager.Instance.Load("flag_stm"));
            m_AreaFlagValue.Add("");
        }
        if (m_AreaAmendParam.m_FlagAmendGuerrillaBoss)
        {
            m_AreaFlagSprits.Add(ResourceManager.Instance.Load("flag_boss"));
            m_AreaFlagValue.Add("");
        }
        if (m_AreaAmendParam.m_FlagAmendLinkPoint)
        {
            m_AreaFlagSprits.Add(ResourceManager.Instance.Load("flag_link"));
            m_AreaFlagValue.Add(getStringFlagRate(m_AreaAmendParam.m_QuestSelectAreaAmendLinkPoint));
        }

        if (m_AreaFlagSprits.Count != 0)
        {
            m_ChallengeSelect.IsViewFlag = true;
            m_ChallengeSelect.FlagImage = m_AreaFlagSprits[m_CurrentAreaFlag];
            m_ChallengeSelect.FlagValue = m_AreaFlagValue[m_CurrentAreaFlag];
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="rate"></param>
    /// <returns></returns>
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

    /// <summary>
    ///
    /// </summary>
    private void updateAreaFlag()
    {
        m_AreaFlagTime += Time.deltaTime;
        if (m_AreaFlagTime >= AREA_FLAG_CHANGE_TIME)
        {
            m_AreaFlagTime = 0.0f;
            int next = m_CurrentAreaFlag + 1;
            if (next >= m_AreaFlagSprits.Count)
            {
                next = 0;
            }
            if (m_CurrentAreaFlag != next)
            {
                m_CurrentAreaFlag = next;
                m_ChallengeSelect.FlagImage = m_AreaFlagSprits[m_CurrentAreaFlag];
                m_ChallengeSelect.FlagValue = m_AreaFlagValue[m_CurrentAreaFlag];
            }
        }
    }

    /// <summary>
    /// OKボタン選択
    /// </summary>
    /// <param name="data"></param>
    private void OnSelectOk(ChallengeSelect.EventData data)
    {
        var requirement_id = data.questMaster.quest_requirement_id;
        MasterDataQuestRequirement quest_requirement = null;
        if (requirement_id != 0) quest_requirement = MasterDataUtil.GetMasterDataQuestRequirementFromID(requirement_id);

        //--------------------------------
        // ユニット所持数が上限超えてるならクエスト不可
        //--------------------------------
        if (UserDataAdmin.Instance.m_StructPlayer.total_unit < UserDataAdmin.Instance.m_StructPlayer.unit_list.Length)
        {
            openDialogUnitOver();

            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
        else if (quest_requirement != null
            && quest_requirement.limit_rank > UserDataAdmin.Instance.m_StructPlayer.rank)
        {
            //--------------------------------
            // ランク制限でクエストに入れない場合
            //--------------------------------
            openDialogRankLow();

            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
        else if (MainMenuParam.m_QuestStamina != 0 &&
                UserDataAdmin.Instance.m_StructPlayer.stamina_max < MainMenuParam.m_QuestStamina)
        {
            //--------------------------------
            // スタミナのMAX値が足りずクエストに入れない場合
            //--------------------------------
            openDialogStaminaLow();

            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
        else if (MainMenuParam.m_QuestTicket != 0 &&
                MainMenuParam.m_QuestTicket > UserDataAdmin.Instance.m_StructPlayer.have_ticket)
        {
            //--------------------------------
            // チケット対価不足
            //--------------------------------
            openDialogTicketLow();

            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
        else if (MainMenuParam.m_QuestStamina != 0 &&
                UserDataAdmin.Instance.m_StructPlayer.stamina_max >= data.questMaster.consume_value &&
                MainMenuParam.m_QuestStamina > UserDataAdmin.Instance.m_StructPlayer.stamina_now)
        {
            //--------------------------------
            // スタミナが足りない場合、スタミナ回復ダイアログを表示
            //--------------------------------

            PacketStructUseItem item = useRecoverItem();

            if (item != null)
            {
                openDialogUseItem(item);
            }
            else
            {
                //--------------------------------
                // チップによる回復ルート
                //--------------------------------

                openDialogUseStone();
            }

            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
        else
        {
            //--------------------------------
            // 特にエラーなし。次のフローへ
            //--------------------------------
            //成長ボス選択パラメータ設定
            MainMenuParam.SetChallengeSelectParam(data.questMaster.fix_id, data.bSkip, (data.bSkip ? data.SkipLevel : data.info.challenge_level));

            //成長ボスではMainMenuQuestSelectを経由しないのでAreaCategoryIDをここで設定
            MasterDataArea area = MasterDataUtil.GetAreaParamFromID(data.questMaster.area_id);
            MainMenuParam.m_QuestSelectAreaCateID = (area != null ? area.area_cate_id : 0);

            MainMenuParam.m_QuestAreaAmendList = m_AreaAmendParam.m_AreaMasterDataAmendList;
            MainMenuParam.m_QuestSelectAreaID = data.questMaster.area_id;
            MainMenuParam.m_QuestSelectMissionID = data.questMaster.fix_id;
            if (MainMenuManager.HasInstance)
            {
                MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_FRIEND, false, false);
                SoundUtil.PlaySE(SEID.SE_MENU_OK);
            }
        }
    }

    /// <summary>
    /// 報酬ボタン選択
    /// </summary>
    /// <param name="data"></param>
    private void OnSelectReward(ChallengeSelect.EventData data)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        ChallengeRewardDialog newDialog = ChallengeRewardDialog.Create(SceneObjReferMainMenu.Instance.m_MainMenuGroupCamera.GetComponent<Camera>());
        newDialog.setup(data.info);
        newDialog.Show(() =>
        {
            updateRewardFlag(data);
        });
    }

    /// <summary>
    /// レベル選択ボタン選択
    /// </summary>
    /// <param name="data"></param>
    private void OnSelectSkip(ChallengeSelect.EventData data)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        m_ChallengeSelect.IsViewSkipButton = false;
        ChallengeSkipDialog newDialog = ChallengeSkipDialog.Create(SceneObjReferMainMenu.Instance.m_MainMenuGroupCamera.GetComponent<Camera>());
        newDialog.setup(data);
        newDialog.Show(() =>
        {
            updateQuestInfo(data);
            m_ChallengeSelect.IsViewSkipButton = true;
        });
    }

    /// <summary>
    /// ルールボタン選択
    /// </summary>
    /// <param name="data"></param>
    private void OnSelectRule(ChallengeSelect.EventData data)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "growth_boss_11");
        newDialog.SetDialogText(DialogTextType.MainText, data.questMaster.quest_requirement_text);
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.EnableCancel();
        newDialog.Show();
    }

    /// <summary>
    /// 特性ボタン選択
    /// </summary>
    /// <param name="data"></param>
    private void OnSelectBossAtr(ChallengeSelect.EventData data)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "growth_boss_12");
        newDialog.SetDialogText(DialogTextType.MainText, MasterDataUtil.GetBossAbilityText(data.questMaster));
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.EnableCancel();
        newDialog.Show();

    }

    /// <summary>
    /// 戻るボタン選択
    /// </summary>
    private void OnSelectReturn()
    {
        MainMenuParam.m_RegionID = MasterDataUtil.GetRegionIDFromCategory(MasterDataDefineLabel.REGION_CATEGORY.EVENT);
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_STORY, false, false);
    }

    /// <summary>
    /// Homeへ戻るダイアログ
    /// </summary>
    private void openReturnHomeDialog()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "growth_boss_13");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "growth_boss_14");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
         {
             //HOMEへ
             MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false);
         });
        newDialog.Show();
    }

    #region ---MainMenuQuestDetailと同一関数。共通化したい---

    private void openDialogUnitOver()
    {
        if (UserDataAdmin.Instance.m_StructPlayer.extend_unit < MasterDataUtil.GetMasterDataGlobalParamFromID(GlobalDefine.GLOBALPARAMS_UNIT_MAX_EXTEND))
        {
            // ユニット所持枠購入上限内
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "kk116q_title");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "kk116q_content");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.SetDialogObjectEnabled(DialogObjectType.VerticalButtonList, true);
            newDialog.addVerticalButton(GameTextUtil.GetText("icon_button_01"), () =>
            {
                //ユニット枠拡張
                StoreDialogManager.Instance.OpenDialogUnitExtend();
            });
            newDialog.addVerticalButton(GameTextUtil.GetText("icon_button_02"), null, () =>
            {
                if (MainMenuManager.HasInstance)
                {
                    MainMenuParam.m_BuildupBaseUnitUniqueId = 0;
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_BUILDUP, false, false);
                }
            });
            newDialog.addVerticalButton(GameTextUtil.GetText("icon_button_03"), null, () =>
            {
                if (MainMenuManager.HasInstance)
                {
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_SALE, false, false);
                }
            });
            newDialog.Show();
        }
        else
        {
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "kk116q_title");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "error_response_content71");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.SetDialogObjectEnabled(DialogObjectType.VerticalButtonList, true);
            newDialog.addVerticalButton(GameTextUtil.GetText("icon_button_02"), null, () =>
            {
                if (MainMenuManager.HasInstance)
                {
                    MainMenuParam.m_BuildupBaseUnitUniqueId = 0;
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_BUILDUP, false, false);
                }
            });
            newDialog.addVerticalButton(GameTextUtil.GetText("icon_button_03"), null, () =>
            {
                if (MainMenuManager.HasInstance)
                {
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_SALE, false, false);
                }
            });
            newDialog.Show();
        }
    }

    private void openDialogRankLow()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "kk109q_title2");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "kk109q_content3");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button6");
        newDialog.Show();
    }

    private void openDialogStaminaLow()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "kk109q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "kk109q_content2");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button6");
        newDialog.Show();
    }

    private void openDialogTicketLow()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "quest_notticket_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "quest_notticket_content");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button6");
        newDialog.Show();
    }
    private PacketStructUseItem useRecoverItem()
    {
        PacketStructUseItem[] items = UserDataAdmin.Instance.m_StructPlayer.item_list;
        List<PacketStructUseItem> list = new List<PacketStructUseItem>();

        // レコード追加
        for (int i = 0; i < items.Length; i++)
        {
            PacketStructUseItem item = items[i];
            if (item.item_cnt == 0)
            {
                continue;
            }

            MasterDataUseItem itemMaster = MasterFinder<MasterDataUseItem>.Instance.Find((int)item.item_id);
            if (itemMaster == null)
            {
                continue;
            }

            if (!MasterDataUtil.ChkUseItemTypeStaminaRecovery(itemMaster))
            {
                continue;
            }

            //スタミナ系アイテム
            list.Add(item);
        }

        if (list.Count <= 0)
        {
            return null;
        }

        //[DG0-1196]fix_idの昇順ソートで使用順を決める ItemMaster.fix_id == PacketStructUseItem.item_id
        list.Sort((a, b) => (int)a.item_id - (int)b.item_id);

        return list[0];
    }

    private void openDialogUseItem(PacketStructUseItem item)
    {
        MasterDataUseItem itemMaster = MasterFinder<MasterDataUseItem>.Instance.Find((int)item.item_id);

        //アイテムで回復
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "kk109q_title");
        string text = GameTextUtil.GetText("sh132q_content4");
        string maintext = String.Format(text, itemMaster.item_name, item.item_cnt);
        // 回復後のスタミナ値を表示.
        int result_stamina = UserDataAdmin.Instance.GetUseItemStamina(itemMaster);
        maintext += "\n\n" + string.Format(GameTextUtil.GetText("sh132q_content3"), result_stamina, UserDataAdmin.Instance.m_StructPlayer.stamina_max);
        // オーバー回復になるかどうか
        if (result_stamina > UserDataAdmin.Instance.m_StructPlayer.stamina_max)
        {
            maintext += "\n\n" + GameTextUtil.GetText("sh132q_content5");
        }
        newDialog.SetDialogText(DialogTextType.MainText, maintext);
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            //スタミナ回復
            ServerDataUtilSend.SendPacketAPI_ItemUse(itemMaster.fix_id)
            .setSuccessAction(_data =>
            {
                UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvItemUse>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                DialogManager.Open1B("sh135q_title", "sh135q_content", "common_button1", true, true).
                SetOkEvent(() =>
                {
                });

            })
            .setErrorAction(_data =>
            {
            })
            .SendStart();
        });
        newDialog.Show();
    }

    private void openDialogUseStone()
    {
        //チップで回復
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "kk109q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "kk109q_content1");
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            //スタミナ回復
            StoreDialogManager.Instance.OpenDialogStaminaRecovery();
        });
        newDialog.Show();
    }

    #endregion
}
