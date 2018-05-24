using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class MainMenuQuestDetail : MainMenuSeq
{
    [SerializeField]
    private static float PanelAnimationDuration = 0.2f;


    private QuestDetailBG m_QuestDetailBG = null;
    private QuestDetailInfo m_QuestDetailInfo = null;
    private QuestDetailMessage m_QuestDetailMessage = null;
    private QuestDetailMission m_QuestDetailMission = null;

    private MasterDataAreaCategory m_MasterDataAreaCategory = null;
    private MasterDataArea m_MasterDataArea = null;
    // TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
    //private MasterDataQuest m_MasterDataQuest = null;
    private MasterDataQuest2 m_MasterDataQuest2 = null;
    private MainMenuQuestSelect.AreaAmendParam m_AreaAmendParam = null;

    private QuestDetailModel m_model = new QuestDetailModel();
    private bool m_bReturnHome = false;
    private bool m_bGetMissionData = false;

    void Awake()
    {
        var canvas = GetComponentInChildren<Canvas>();
        Debug.Assert(canvas != null, "canvas not found.");
        m_BackEventPageUpdate = true;
        m_bGetMissionData = false;
    }

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

        if (m_bReturnHome)
        {
            if (MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false))
            {
                m_bReturnHome = false;
            }
        }
    }

    //ページ初期化処理
    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //戻るボタン抑制
        SetSuspendReturn(true);

        if (m_QuestDetailBG == null)
        {
            m_QuestDetailBG = m_CanvasObj.GetComponentInChildren<QuestDetailBG>();
            m_QuestDetailBG.Load(MainMenuParam.m_QuestSelectAreaCateID);
            m_QuestDetailBG.SetPositionAjustStatusBar(new Vector2(0, -25), new Vector2(0, -215));
        }
        if (m_QuestDetailInfo == null)
        {
            m_QuestDetailInfo = m_CanvasObj.GetComponentInChildren<QuestDetailInfo>();
            m_QuestDetailInfo.SetModel(m_model);
        }
        if (m_QuestDetailMessage == null)
        {
            m_QuestDetailMessage = m_CanvasObj.GetComponentInChildren<QuestDetailMessage>();
            m_QuestDetailMessage.SetModel(m_model);
        }
        if (m_QuestDetailMission == null)
        {
            m_QuestDetailMission = m_CanvasObj.GetComponentInChildren<QuestDetailMission>();
            //m_QuestDetailMission.SetPosition(new Vector2(0, 270), new Vector2(0, -546));
            m_QuestDetailMission.SetModel(m_model);
        }
        if (m_QuestDetailInfo.tab == null)
        {
            m_QuestDetailInfo.tab = m_CanvasObj.GetComponentInChildren<QuestDetailTab>();
            m_QuestDetailInfo.tab.m_IsReady = false;
        }

        m_MasterDataAreaCategory = MasterFinder<MasterDataAreaCategory>.Instance.Find((int)MainMenuParam.m_QuestSelectAreaCateID);
        m_MasterDataArea = MasterFinder<MasterDataArea>.Instance.Find((int)MainMenuParam.m_QuestSelectAreaID);
        m_AreaAmendParam = MainMenuUtil.CreateAreaParamAmend(m_MasterDataArea);
        if (MainMenuUtil.IsRenewQuest(m_MasterDataAreaCategory))
        {
            // TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
            //m_MasterDataQuest = null;
            m_MasterDataQuest2 = MasterDataUtil.GetQuest2ParamFromID(MainMenuParam.m_QuestSelectMissionID);
            updateQuest2Detail();
        }

        m_bReturnHome = false;

        SetUpAppearAnimation();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.QUEST;

#if BUILD_TYPE_DEBUG
        if (DebugOption.Instance.disalbeDebugMenu == false)
        {
            // デバッグ用メニューの生成
            QuestDetailDebugMenu debugMenu = QuestDetailDebugMenu.Create(m_CanvasObj.transform);
            if (debugMenu != null)
            {
                debugMenu.m_QuestDetailBG = m_QuestDetailBG;
                debugMenu.m_InitCharaID = m_MasterDataQuest2.boss_chara_id;
                debugMenu.SetPositionAjustStatusBar(new Vector2(0, -66), new Vector2(0, -132));
            }
        }
#endif
        StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.QuestStart, MainMenuParam.m_QuestSelectMissionID));
    }

    public override bool PageSwitchEventEnableAfter()
    {
        //戻るアクション設定
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.SetMenuReturnAction(true, SelectReturn);
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

    public override void OnPageFinished()
    {
        base.OnPageFinished();
        if (m_QuestDetailInfo != null)
            m_QuestDetailInfo.Hide();

        if (m_QuestDetailBG != null)
        {
            m_QuestDetailBG.Hide();
            m_QuestDetailBG.HideChara();
        }


        if (m_model.currentType != QuestDetailModel.TabType.Info)
        {
            m_model.currentType = QuestDetailModel.TabType.Info;
            SwitchPrefabs(false);
            SwitchPrefabs(true);
            m_QuestDetailInfo.tab.Clear();
            updateQuest2Detail();
        }
    }

    public void SelectReturn()
    {
        if (!MainMenuManager.HasInstance)
        {
            return;
        }

        // エリアカテゴリIDの登録
        MainMenuParam.SetQuestSelectParam(m_MasterDataArea.area_cate_id, m_MasterDataArea.fix_id);

        // エリア移動
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT, false, false);
    }

    private void SetUpAppearAnimation()
    {
        EffectProcessor.Instance.Register("MainMenuQuestDetail",
            (System.Action finish) =>
            {
                new SerialProcess()
                    .Add((System.Action nextProcess) =>
                    {
                        m_QuestDetailBG.Show(nextProcess);
                    })
                    .Add((System.Action nextProcess) =>
                    {
                        m_QuestDetailInfo.Show(nextProcess);
                    })
                    .Add((System.Action nextProcess) =>
                    {
                        m_QuestDetailBG.ShowChara(nextProcess);
                    })
                    .Add((System.Action nextProcess) =>
                    {
                        //戻るボタン抑制解除
                        SetSuspendReturn(false);
                        m_QuestDetailInfo.tab.m_IsReady = true;
                    })
                    .Flush();

                finish();
            });
    }

    private void updateQuest2Detail()
    {
        if (m_QuestDetailBG != null)
        {
            //情報
            m_QuestDetailBG.QuestId = 1;
            m_QuestDetailBG.QuestIdLabel = "Quest.";
            m_QuestDetailBG.QuestTitle = m_MasterDataQuest2.quest_name;
            m_QuestDetailBG.AreaCategoryTitle = m_MasterDataAreaCategory.area_cate_name;
            m_QuestDetailBG.AreaTitle = m_MasterDataArea.area_name;
            m_QuestDetailBG.ButtonTitle = GameTextUtil.GetText("questinfo_button");
            m_QuestDetailBG.DidSelectButton = OnSelectButton;

            UnitIconImageProvider.Instance.Get(
                m_MasterDataQuest2.boss_chara_id,
                sprite =>
                {
                    m_QuestDetailBG.BossImage = sprite;
                });

            MasterDataParamChara _master = MasterFinder<MasterDataParamChara>.Instance.Find((int)m_MasterDataQuest2.boss_chara_id);
            m_QuestDetailBG.IconSelect = MainMenuUtil.GetElementCircleSprite(_master.element);
            m_QuestDetailBG.setupChara(m_MasterDataQuest2.boss_chara_id);
        }

        MasterDataParamChara _masterBoss = MasterFinder<MasterDataParamChara>.Instance.Find((int)m_MasterDataQuest2.boss_chara_id);
        if (m_QuestDetailInfo != null &&
            _masterBoss != null)
        {
            //詳細情報
            m_QuestDetailInfo.CountLabel = GameTextUtil.GetText("questinfo_text1");
            m_QuestDetailInfo.CountValue = string.Format(GameTextUtil.GetText("value_colorset"), m_MasterDataQuest2.battle_count);
            m_QuestDetailInfo.ExpLabel = GameTextUtil.GetText("questinfo_text3");
            m_QuestDetailInfo.CoinLabel = GameTextUtil.GetText("questinfo_text4");
            m_QuestDetailInfo.BossLabel = GameTextUtil.GetText("questinfo_text2");
            m_QuestDetailInfo.BossName = _masterBoss.name;

            {
                //詳細情報テキスト差し替え
                MasterDataQuestAppearance[] questAppearance = MasterFinder<MasterDataQuestAppearance>.Instance.SelectWhere("where area_category_id = ?", MainMenuParam.m_QuestSelectAreaCateID).ToArray();
                if (questAppearance.IsNullOrEmpty() == false)
                {
                    m_QuestDetailInfo.BossLabel = questAppearance[0].boss_text_key;
                    m_QuestDetailInfo.CountLabel = questAppearance[0].battle_text_key;
                }
            }

            //----------------------------------------
            // 初心者ブースト適用
            // 表示用の値を計算、補正値を適用
            //----------------------------------------
            uint exp = (uint)m_MasterDataQuest2.clear_exp;
            uint coin = (uint)m_MasterDataQuest2.clear_money;
            if (m_AreaAmendParam != null)
            {
                if (m_AreaAmendParam.m_QuestSelectAreaAmendEXP != 100)
                {
                    // エリア補正時
                    exp = (uint)((float)exp * (m_AreaAmendParam.m_QuestSelectAreaAmendEXP / 100.0f));
                    m_QuestDetailInfo.ExpValue = string.Format(GameTextUtil.GetText("stmina_bahutext"), exp);
                }
                else
                {
                    m_QuestDetailInfo.ExpValue = string.Format(GameTextUtil.GetText("value_colorset"), exp);
                }

                if (m_AreaAmendParam.m_QuestSelectAreaAmendCoin != 100)
                {
                    // エリア補正時
                    coin = (uint)((float)coin * (m_AreaAmendParam.m_QuestSelectAreaAmendCoin / 100.0f));
                    m_QuestDetailInfo.CoinValue = string.Format(GameTextUtil.GetText("stmina_bahutext"), coin);
                }
                else
                {
                    m_QuestDetailInfo.CoinValue = string.Format(GameTextUtil.GetText("value_colorset"), coin);
                }
            }

            MainMenuParam.m_QuestAddMoney = coin;
        }
        {
            //ミッション
            m_QuestDetailMission.Title = GameTextUtil.GetText("questinfo2_text");
            m_QuestDetailMission.Count = 0;
            m_QuestDetailMission.CountMax = 0;
        }
        {
            //タブ
            m_QuestDetailInfo.tab.DidTabChenged = (QuestDetailTabContext tab) =>
            {
                m_model.currentType = tab.m_Type;
                OnSelectTabButton();
            };


            m_QuestDetailInfo.tab.Clear();
            m_QuestDetailInfo.tab.AddTab(GameTextUtil.GetText("questinfo_text6"), QuestDetailModel.TabType.Info);
            m_QuestDetailInfo.tab.AddTab(GameTextUtil.GetText("questinfo_text7"), QuestDetailModel.TabType.Mission);

            if (m_MasterDataQuest2.quest_requirement_id != 0)
            {
                m_QuestDetailInfo.tab.AddTab(GameTextUtil.GetText("questinfo_text8"), QuestDetailModel.TabType.Rule);
            }

            if (m_MasterDataQuest2.boss_ability_1 != 0 ||
               m_MasterDataQuest2.boss_ability_2 != 0 ||
               m_MasterDataQuest2.boss_ability_3 != 0 ||
               m_MasterDataQuest2.boss_ability_4 != 0)
            {
                m_QuestDetailInfo.tab.AddTab(GameTextUtil.GetText("questinfo_text9"), QuestDetailModel.TabType.Boss);
            }
        }
        {
            UnityUtil.SetObjectEnabledOnce(m_QuestDetailInfo.gameObject, true);
            UnityUtil.SetObjectEnabledOnce(m_QuestDetailMessage.gameObject, false);
            UnityUtil.SetObjectEnabledOnce(m_QuestDetailMission.gameObject, false);
        }
    }

    private void OnSelectTabButton()
    {
        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);

        switch (m_model.currentType)
        {
            case QuestDetailModel.TabType.Info:
                SwitchPrefabs(false);
                ChangeInfoWindow(() =>
                {
                    SwitchPrefabs(true);
                    m_QuestDetailInfo.tab.ActivateButton();
                });

                MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk104f_description"));
                break;
            case QuestDetailModel.TabType.Mission:
                SwitchPrefabs(false);
                ChangeInfoWindow(() =>
                {
                    SwitchPrefabs(true);
                    if (m_bGetMissionData == false)
                    {
                        //ミッションデータ未取得
                        sendMissionGet(() =>
                        {
                            m_QuestDetailInfo.tab.ActivateButton();
                            m_bGetMissionData = true;
                        });
                    }
                    else
                    {
                        //ミッションデータ取得済み
                        m_QuestDetailInfo.tab.ActivateButton();
                    }
                });

                MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk105f_description"));
                break;
            case QuestDetailModel.TabType.Rule:
                SwitchPrefabs(false);
                ChangeInfoWindow(() =>
                {
                    m_QuestDetailMessage.Title = GameTextUtil.GetText("questinfo3_text");
#if false// TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
                    if (m_MasterDataQuest != null)
                    {
                        m_QuestDetailMessage.Message = m_MasterDataQuest.quest_requirement_text;
                    }
                    else
#endif
                    {
                        m_QuestDetailMessage.Message = m_MasterDataQuest2.quest_requirement_text;
                    }
                    SwitchPrefabs(true);

                    m_QuestDetailInfo.tab.ActivateButton();
                });

                MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk106f_description"));
                break;
            case QuestDetailModel.TabType.Boss:
                SwitchPrefabs(false);
                ChangeInfoWindow(() =>
                {
                    m_QuestDetailMessage.Title = GameTextUtil.GetText("questinfo4_text");
                    m_QuestDetailMessage.Message = MasterDataUtil.GetBossAbilityText(m_MasterDataQuest2);
                    SwitchPrefabs(true);

                    m_QuestDetailInfo.tab.ActivateButton();
                });
                MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk108f_description"));
                break;
        }
    }

    private void SwitchPrefabs(bool bFlag)
    {
        switch (m_model.currentType)
        {
            case QuestDetailModel.TabType.Info:
                if (bFlag)
                {
                    UnityUtil.SetObjectEnabledOnce(m_QuestDetailInfo.gameObject, true);
                }
                else
                {
                    UnityUtil.SetObjectEnabledOnce(m_QuestDetailMessage.gameObject, false);
                    UnityUtil.SetObjectEnabledOnce(m_QuestDetailMission.gameObject, false);
                }
                break;
            case QuestDetailModel.TabType.Mission:
                if (bFlag)
                {
                    UnityUtil.SetObjectEnabledOnce(m_QuestDetailMission.gameObject, true);
                }
                else
                {
                    UnityUtil.SetObjectEnabledOnce(m_QuestDetailInfo.gameObject, false);
                    UnityUtil.SetObjectEnabledOnce(m_QuestDetailMessage.gameObject, false);
                }
                break;
            case QuestDetailModel.TabType.Rule:
                if (bFlag)
                {
                    UnityUtil.SetObjectEnabledOnce(m_QuestDetailMessage.gameObject, true);
                }
                else
                {
                    UnityUtil.SetObjectEnabledOnce(m_QuestDetailInfo.gameObject, false);
                    UnityUtil.SetObjectEnabledOnce(m_QuestDetailMission.gameObject, false);
                }
                break;
            case QuestDetailModel.TabType.Boss:
                if (bFlag)
                {
                    UnityUtil.SetObjectEnabledOnce(m_QuestDetailMessage.gameObject, true);
                }
                else
                {
                    UnityUtil.SetObjectEnabledOnce(m_QuestDetailInfo.gameObject, false);
                    UnityUtil.SetObjectEnabledOnce(m_QuestDetailMission.gameObject, false);
                }
                break;
        }
    }

    private void ChangeInfoWindow(System.Action action)
    {
        m_QuestDetailBG.Change(
            m_model.GetBgHeight(),
            PanelAnimationDuration,
            () =>
            {
                action();
            });
    }

    private void OnSelectButton()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("select quest id=" + MainMenuParam.m_QuestSelectMissionID.ToString());
#endif
        var requirement_id = m_MasterDataQuest2.quest_requirement_id;
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
                UserDataAdmin.Instance.m_StructPlayer.stamina_max < m_MasterDataQuest2.consume_value * MainMenuParam.m_QuestStaminaAmend)
        {
            //--------------------------------
            // スタミナのMAX値が足りずクエストに入れない場合
            //--------------------------------
            openDialogStaminaLow();

            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
        else if (MainMenuParam.m_QuestStamina != 0 &&
                UserDataAdmin.Instance.m_StructPlayer.stamina_max >= m_MasterDataQuest2.consume_value &&
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
        else if (MainMenuParam.m_QuestTicket != 0 &&
                MainMenuParam.m_QuestTicket > UserDataAdmin.Instance.m_StructPlayer.have_ticket)
        {
            //--------------------------------
            // チケット対価不足
            //--------------------------------
            openDialogTicketLow();

            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
        else if (MainMenuParam.m_QuestKey != 0 &&
            MainMenuParam.m_QuestKey > GetQuestKeyCntFromAreaCategory(m_MasterDataAreaCategory))
        {
            //--------------------------------
            // キークエストの場合、クエストキー数チェッククエストキー不足
            //--------------------------------
            openDialogKeyLow();

            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
        else
        {
            //--------------------------------
            // 特にエラーなし。次のフローへ
            //--------------------------------
            if (MainMenuManager.HasInstance)
            {
                MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_FRIEND, false, false);
                SoundUtil.PlaySE(SEID.SE_MENU_OK);
            }
        }

    }

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

    private void openDialogKeyLow()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "quest_notkey_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "quest_notkey_content");
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
                    StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission, 0));
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

    private void sendMissionGet(System.Action callback = null)
    {
        m_QuestDetailMission.MissionList.Clear();
        ServerDataUtilSend.SendPacketAPI_GetMasterDataAchievement(4, 0, 0, MainMenuParam.m_QuestSelectMissionID)
        .setSuccessAction(_data =>
        {
            RecvMasterDataAchievementValue _result = _data.GetResult<RecvMasterDataAchievement>().result;

            MasterDataAchievementConverted[] achievementArray = _result.master_array_achievement;
            // 未達成と達成済みを分ける
            List<MasterDataAchievementConverted> notAchievedList = new List<MasterDataAchievementConverted>();
            List<MasterDataAchievementConverted> achievedList = new List<MasterDataAchievementConverted>();
            for (int i = 0; i < achievementArray.Length; ++i)
            {
                if (achievementArray[i].IsState_Achieve)
                {
                    achievedList.Add(achievementArray[i]);
                }
                else
                {
                    notAchievedList.Add(achievementArray[i]);
                }
            }

            // 未達成と達成済みを結合する
            notAchievedList.AddRange(achievedList);
            achievementArray = notAchievedList.ToArray();

            int clearCount = 0;
            for (int i = 0; i < achievementArray.Length; i++)
            {
                MasterDataAchievementConverted _master = achievementArray[i];
                QuestMissionContext newMission = new QuestMissionContext();
                newMission.Title = _master.draw_msg;
                newMission.ItemName = _master.PresentName;
                newMission.Count = (int)_master.ProgressCount;
                newMission.CountMax = (int)_master.TotalCount;
                _master.GetPresentIcon(sprite => { newMission.IconImage = sprite; });
                newMission.IsActiveLeftTime = false;
                newMission.ItemValue = (_master.PresentCount > 0) ? _master.PresentCount.ToString() : "";

                if (_master.event_id != 0)
                {
                    uint unTimingEnd = MainMenuUtil.GetEventTimingEnd(_master.event_id);

                    if (unTimingEnd != 0)
                    {
                        DateTime endTime = TimeUtil.GetDateTime(unTimingEnd);
                        DateTime nowTime = TimeManager.Instance.m_TimeNow;
                        TimeSpan leftTime = endTime - nowTime;
                        newMission.LeftValue = GameTextUtil.GetRemainStr(leftTime, GameTextUtil.GetText("general_time_01"));
                        newMission.IsActiveLeftTime = true;
                    }
                }
                if (_master.IsState_Achieve)
                {
                    newMission.Count = newMission.CountMax;
                    clearCount++;
                }
                m_QuestDetailMission.MissionList.Add(newMission);
            }
            m_QuestDetailMission.Count = clearCount;
            m_QuestDetailMission.CountMax = achievementArray.Length;

            if (callback != null)
                callback();
        })
        .setErrorAction(_data =>
        {
            if (callback != null)
                callback();
        })
        .SendStart();
    }

    public void openWarningAreaDialog()
    {
        //表示OFF
        UnityUtil.SetObjectEnabledOnce(m_CanvasObj, false);
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

    #region ==== キークエスト：対応エリアのクエストキー所持数取得 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	対応エリアのクエストキー所持数取得（v380 クエストキー合算対応）
	*/
    //----------------------------------------------------------------------------
    private uint GetQuestKeyCntFromAreaCategory(MasterDataAreaCategory _master)
    {
        uint unCnt = 0;

        if (_master == null)
        {
            return unCnt;
        }

        PacketStructQuestKey[] key_list = UserDataAdmin.Instance.m_StructPlayer.quest_key_list;

        if (key_list == null ||
            key_list.Length == 0)
        {
            return unCnt;
        }

        //----------------------------------------
        // 現在有効なキー所持数をマージ
        //----------------------------------------
        for (int iKey = 0; iKey < key_list.Length; iKey++)
        {
            if (key_list[iKey] == null
            || key_list[iKey].quest_key_id <= 0)
            {
                continue;
            }

            MasterDataQuestKey cQuestKeyMaster = MasterDataUtil.GetMasterDataQuestKeyFromID(key_list[iKey].quest_key_id);

            if (cQuestKeyMaster == null
                || cQuestKeyMaster.key_area_category_id != _master.fix_id)
            {
                continue;
            }

            //有効期限チェック
            if (cQuestKeyMaster.timing_end > 0)
            {
                bool bCheckWithinTime = TimeManager.Instance.CheckWithinTime(cQuestKeyMaster.timing_end);
                if (bCheckWithinTime == false)
                {
                    continue;
                }
            }

            unCnt += key_list[iKey].quest_key_cnt;
        }

        return unCnt;
    }
    #endregion
}
