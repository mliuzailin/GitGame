/**
 *  @file   MainMenuQuestSelectParty.cs
 *  @brief
 *  @author Developer
 *  @date   2017/03/13
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using ServerDataDefine;

public class MainMenuQuestSelectParty : MainMenuSeq
{
    /// <summary>パーティ選択</summary>
    PartySelectGroup m_PartySelectGroup = null;
    /// <summary>選択パーティのユニットリスト</summary>
    PartyParamQuestPartyPanel m_PartyParamQuestPartyPanel = null;
    /// <summary>下部ボタン</summary>
    PartySelectButtonPanel m_PartySelectButtonPanel = null;
    /// <summary>低解像度かどうか</summary>
    bool m_IsLowerScreen;
    /// <summary>固定パーティ上書きがあるかどうか</summary>
    private bool m_IsOverrideAssign = false;

    private List<PartyParamListItemModel> m_partyPanels = new List<PartyParamListItemModel>();

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        ReplaceAssetManager.Instance.startReplaceMode(MainMenuParam.m_QuestSelectAreaCateID, ReplaceAssetReference.ChangeTimingType.MENU);
    }

    // Update is called once per frame
    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }

    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ReplaceAssetManager.Instance.endReplaceMode();
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //--------------------------------
        //	オブジェクトの取得
        //--------------------------------
        m_PartySelectGroup = m_CanvasObj.GetComponentInChildren<PartySelectGroup>();
        m_PartySelectGroup.SetPositionAjustStatusBar(new Vector2(0, -132));
        m_PartyParamQuestPartyPanel = m_CanvasObj.GetComponentInChildren<PartyParamQuestPartyPanel>();
        m_PartyParamQuestPartyPanel.SetPositionAjustStatusBar(new Vector2(0, -263));
        m_PartySelectButtonPanel = m_CanvasObj.GetComponentInChildren<PartySelectButtonPanel>();
        m_PartySelectButtonPanel.SetPositionAjustStatusBar(new Vector2(0, -908));

        m_IsOverrideAssign = false;

        SizeToFitPosition();

        // パーティ一覧の描画
        CreatePartyList();
        CreatePartyParam();

        // 下部ボタンの設定
        m_PartySelectButtonPanel.IsActiveNextButton = true;
        m_PartySelectButtonPanel.NextAction = OnClickNextButton;
        m_PartySelectButtonPanel.IsActiveMemberSettingButton = true;
        m_PartySelectButtonPanel.MemberSettingAction = OnSelectMovePartyAssign;
        m_PartySelectButtonPanel.MemberSettingButtonText = GameTextUtil.GetText("party_button");
        m_PartySelectButtonPanel.AutoPlayAction = OnClickAutoPlayButton;
        {
            bool is_ok_auto_play = false;
            MasterDataQuest2 master_data_quest2 = MasterDataUtil.GetQuest2ParamFromID(MainMenuParam.m_QuestSelectMissionID);
            if (master_data_quest2 != null)
            {
                if (master_data_quest2.enable_autoplay != MasterDataDefineLabel.BoolType.ENABLE)
                {
                    is_ok_auto_play = true;
                }
            }
            if (is_ok_auto_play)
            {
                LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
                m_PartySelectButtonPanel.IsActiveAutoPlayOnButton = (cOption.m_OptionAutoPlayEnable == (int)LocalSaveDefine.OptionAutoPlayEnable.ON);
                m_PartySelectButtonPanel.IsActiveAutoPlayOffButton = !m_PartySelectButtonPanel.IsActiveAutoPlayOnButton;
                m_PartySelectButtonPanel.IsActiveAutoPlayNgButton = false;
            }
            else
            {
                m_PartySelectButtonPanel.IsActiveAutoPlayOnButton = false;
                m_PartySelectButtonPanel.IsActiveAutoPlayOffButton = false;
                m_PartySelectButtonPanel.IsActiveAutoPlayNgButton = true;
            }
        };
        m_PartySelectButtonPanel.SetUpButtons(true);
        RectTransform rect = m_PartySelectButtonPanel.GetComponent<RectTransform>();
        rect.anchorMax = new Vector2(rect.anchorMax.x, 1);
        rect.anchorMin = new Vector2(rect.anchorMin.x, 1);
        rect.pivot = new Vector2(rect.pivot.x, 0);
        updateStateMemberSettingButton();

        // 開始バトル選択ドロップダウン
        Transform debug_select_battle_dropdown = m_PartySelectButtonPanel.transform.Find("DebugSelectBattleDropdown");
        if (debug_select_battle_dropdown != null)
        {
            debug_select_battle_dropdown.gameObject.SetActive(false);

#if BUILD_TYPE_DEBUG
            if (DebugOption.Instance.disalbeDebugMenu == false)
            {
                Dropdown dropdown = debug_select_battle_dropdown.GetComponent<Dropdown>();
                if (dropdown != null)
                {
                    uint quest_id = MainMenuParam.m_QuestSelectMissionID;
                    MasterDataQuest2 master_data_quest = MasterDataUtil.GetQuest2ParamFromID(quest_id);
                    if (master_data_quest != null)
                    {
                        int battle_count = 0;	// バトル数
                        {
                            uint[] enemy_groups = {
                                master_data_quest.enemy_group_id_1,
                                master_data_quest.enemy_group_id_2,
                                master_data_quest.enemy_group_id_3,
                                master_data_quest.enemy_group_id_4,
                                master_data_quest.enemy_group_id_5,
                                master_data_quest.enemy_group_id_6,
                                master_data_quest.enemy_group_id_7,
                                master_data_quest.enemy_group_id_8,
                                master_data_quest.enemy_group_id_9,
                                master_data_quest.enemy_group_id_10,
                                master_data_quest.enemy_group_id_11,
                                master_data_quest.enemy_group_id_12,
                                master_data_quest.enemy_group_id_13,
                                master_data_quest.enemy_group_id_14,
                                master_data_quest.enemy_group_id_15,
                                master_data_quest.enemy_group_id_16,
                                master_data_quest.enemy_group_id_17,
                                master_data_quest.enemy_group_id_18,
                                master_data_quest.enemy_group_id_19,
                            };
                            for (int idx = 0; idx < enemy_groups.Length; idx++)
                            {
                                if (enemy_groups[idx] > 0)
                                {
                                    battle_count++;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (master_data_quest.boss_group_id > 0)
                            {
                                battle_count++;
                            }

                            battle_count = Mathf.Min(battle_count, (int)master_data_quest.battle_count);
                        }

                        List<string> options = new List<string>();
                        for (int idx = 0; idx < battle_count; idx++)
                        {
                            options.Add(string.Format("Battle {0}/{1}", idx + 1, battle_count));
                        }
                        dropdown.ClearOptions();
                        dropdown.AddOptions(options);

                        debug_select_battle_dropdown.gameObject.SetActive(true);
                    }
                }
            }
#endif //BUILD_TYPE_DEBUG

            if (debug_select_battle_dropdown.gameObject.IsActive() == false)
            {
                Destroy(debug_select_battle_dropdown.gameObject);
            }
        }

        SetUpAppearAnimation();

        //----------------------------------------
        // パッチのリクエスト
        //----------------------------------------
        MainMenuManager.Instance.RequestPatchUpdate(true);

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.QUEST;

        StartCoroutine(sohwArrow(() =>
        {
            MainMenuParam.m_PartySelectIsShowLinkUnit = MainMenuParam.m_PartySelectShowedLinkUnit;
            MainMenuParam.m_PartySelectShowedLinkUnit = false;
            m_PartyParamQuestPartyPanel.SetUpLinkUnit(MainMenuParam.m_PartySelectIsShowLinkUnit);
        }));

    }

    IEnumerator sohwArrow(Action finishAction)
    {

        int unitPartyCurrent = UserDataAdmin.Instance.m_StructPlayer.unit_party_current;
        while (m_PartyParamQuestPartyPanel.PartyParamList.Count == 0)
        {
            yield return null;
        }
        PartyParamQuestListItem list = m_PartyParamQuestPartyPanel.PartyParamList[unitPartyCurrent].GetComponent<PartyParamQuestListItem>();
        if (list != null)
        {
            while (list.Context.UnitList.Count < 5)
            {
                yield return null;
            }
            PartyParamUnitListItem ulist = list.Context.UnitList[4].GetComponent<PartyParamUnitListItem>();
            if (ulist != null)
            {
                while (ulist.isShowedStatus == false)
                {
                    yield return null;
                }

                if (m_IsLowerScreen == false)
                {
                    while (m_PartySelectGroup.isSelectGroupIconSowed() == false)
                    {
                        yield return null;
                    }
                    m_PartySelectGroup.firstShowArrow();
                }
                if (finishAction != null)
                {
                    finishAction();
                }
            }
        }
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

    /// <summary>
    /// 画面サイズに合わせて表示位置を変える
    /// </summary>
    public void SizeToFitPosition()
    {
        RectTransform rect = m_CanvasObj.gameObject.GetComponent<RectTransform>();
        float height = rect.rect.height;
        if (SafeAreaControl.Instance)
        {
            height = height - SafeAreaControl.Instance.bar_height - SafeAreaControl.Instance.bottom_space_height;
        }

        if (height < 1006)
        {
            m_IsLowerScreen = true;
            m_PartySelectGroup.SetSizeParfect(new Vector2(0, 24));
            m_PartySelectGroup.BackGoundImageHeight = 27;
            m_PartyParamQuestPartyPanel.SetPositionAjustStatusBar(new Vector2(0, -170));
            m_PartySelectButtonPanel.SetPositionAjustStatusBar(new Vector2(0, -794));
        }
        else
        {
            m_IsLowerScreen = false;
            m_PartySelectGroup.SetSizeParfect(new Vector2(0, 128));
            m_PartyParamQuestPartyPanel.SetPositionAjustStatusBar(new Vector2(0, -260));
            m_PartySelectButtonPanel.SetPositionAjustStatusBar(new Vector2(0, -884));
        }

        //m_PartySelectButtonPanel.SetSizeParfect(new Vector2(0, 78));
    }

    /// <summary>
    /// パーティリストの作成
    /// </summary>
    void CreatePartyList()
    {
        PacketStructUnit[][] partys = UserDataAdmin.Instance.m_StructPartyAssign;
        m_PartySelectGroup.IsLowerScreen = m_IsLowerScreen;
        m_PartySelectGroup.ClearPartyGroups();

        List<MasterDataParamChara> charaMasterList = MasterFinder<MasterDataParamChara>.Instance.FindAll();
        var partyModels = new List<PartySelectGroupUnitListItemModel>();
        const int AnimationFirstIndex = 0;
        const int AnimationLastIndex = 4;

        for (int i = 0; i < partys.Length; ++i)
        {
            PacketStructUnit[] party = partys[i];
            int index = i;

            var model = new PartySelectGroupUnitListItemModel((uint)index);

            PartySelectGroupUnitContext partyGroup = new PartySelectGroupUnitContext(model);
            partyGroup.IsLowerScreen = m_IsLowerScreen;
            partyGroup.Index = index; // 番号の設定
            partyGroup.NameText = string.Format(GameTextUtil.GetText("questlast_tub"), index + 1); // パーティ名
            Array.Copy(party, partyGroup.PartyData, partyGroup.PartyData.Length); // ユニット情報をコピー
            for (int pt_cout = 0; pt_cout < partyGroup.PartyData.Length; ++pt_cout)
            {
                // リンクユニット情報を設定
                if (partyGroup.PartyData[pt_cout] == null) { continue; }
                partyGroup.PartyLinkData[pt_cout] = CharaLinkUtil.GetLinkUnit(partyGroup.PartyData[pt_cout].link_unique_id);
            }

            RequirementParty(partyGroup); // クエスト制限によるパーティメンバーの上書き処理

            PacketStructUnit leaderUnit = partyGroup.PartyData[(int)GlobalDefine.PartyCharaIndex.LEADER];
            if (leaderUnit != null)
            {
                // リーダーユニット画像
                UnitIconImageProvider.Instance.Get(
                    leaderUnit.id,
                    sprite =>
                    {
                        partyGroup.UnitImage = sprite;
                    });
                //partyGroup.IsActiveLinkIcon = (leaderUnit.link_info != (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE); // リンクアイコン
                MasterDataParamChara _master = charaMasterList.Find((v) => v.fix_id == leaderUnit.id);
                partyGroup.IconSelect = MainMenuUtil.GetElementCircleSprite(_master.element);
            }
            partyGroup.IsSelect = (index == UserDataAdmin.Instance.m_StructPlayer.unit_party_current);
            m_PartySelectGroup.AddData(partyGroup);
            model.OnClicked += () =>
            {
                OnSelectPartyGroup(partyGroup);
            };

            model.OnShowedNextIcon += () =>
            {
                if (index <= AnimationFirstIndex
                    || index > AnimationLastIndex)
                    return;

                partyModels[index - 1].ShowIcon();
            };
            model.OnShowedNextName += () =>
            {
                if (index >= AnimationLastIndex)
                    return;

                partyModels[index + 1].ShowName();
            };

            model.OnViewStarted += () =>
            {
                bool showName = index == AnimationFirstIndex
                                || index > AnimationLastIndex;
                bool showIcon = index == AnimationLastIndex
                                || index > AnimationLastIndex;

                if (showName)
                    model.ShowName();
                if (showIcon)
                    model.ShowIcon();
            };

            partyModels.Add(model);
        }
    }


    /// <summary>
    /// パーティ詳細情報の変更
    /// </summary>
    void CreatePartyParam()
    {
        m_PartyParamQuestPartyPanel.m_CurrentIndex = UserDataAdmin.Instance.m_StructPlayer.unit_party_current;
        m_PartyParamQuestPartyPanel.PartyParams.Clear();
        m_PartyParamQuestPartyPanel.OnChangedPartyParamAction = OnChangedPartyParam;

        m_partyPanels.Clear();

        List<MasterDataParamChara> charaMasterList = MasterFinder<MasterDataParamChara>.Instance.FindAll();
        PacketStructFriend friendData = MainMenuParam.m_QuestHelper;
        CharaOnce friendCharaOnce = new CharaOnce();
        friendCharaOnce.CharaSetupFromID((uint)friendData.unit.id
                                        , (int)friendData.unit.level
                                        , (int)friendData.unit.limitbreak_lv
                                        , (int)friendData.unit.limitover_lv
                                        , (int)friendData.unit.add_pow
                                        , (int)friendData.unit.add_hp
                                        , friendData.unit_link.id
                                        , (int)friendData.unit_link.level
                                        , (int)friendData.unit_link.add_pow
                                        , (int)friendData.unit_link.add_hp
                                        , (int)friendData.unit.link_point
                                        , (int)friendData.unit_link.limitover_lv
                                        );

        for (int i = 0; i < m_PartySelectGroup.PartyCount; ++i)
        {
            int index = i;
            var partyPanelModel = new PartyParamListItemModel((uint)index);

            PartyParamListItemContext partyParam = new PartyParamListItemContext(partyPanelModel);
            PartySelectGroupUnitContext party = m_PartySelectGroup.GetParty(i);

            m_partyPanels.Add(partyPanelModel);

            //--------------------------------------------
            // パーティ情報
            //--------------------------------------------
            if (m_PartyParamQuestPartyPanel != null)
            {
                CharaUtil.setupCharaParty(ref partyParam.PartyInfo, party.PartyData, party.PartyLinkData); // パーティ情報の設定

                partyParam.NameText = string.Format(GameTextUtil.GetText("questlast_text7"), party.Index + 1);
                partyParam.CostText = string.Format(GameTextUtil.GetText("questlast_text5"), (m_IsOverrideAssign == false) ? partyParam.PartyInfo.m_PartyTotalCost : 0
                                                                                        , UserDataAdmin.Instance.m_StructPlayer.total_party); // Cost
                partyParam.CharmText = string.Format(GameTextUtil.GetText("questlast_text6"), (partyParam.PartyInfo.m_PartyTotalCharm + friendCharaOnce.m_CharaCharm)); // CHARM
            }

            //---------------------------------------------------------------------------
            // リーダースキル、フレンドスキル参照用のパーティー情報設定
            //---------------------------------------------------------------------------
            var partyUnits = new PacketStructUnit[(int)GlobalDefine.PartyCharaIndex.MAX];
            partyUnits[(int)GlobalDefine.PartyCharaIndex.LEADER] = party.PartyData[(int)GlobalDefine.PartyCharaIndex.LEADER];
            partyUnits[(int)GlobalDefine.PartyCharaIndex.FRIEND] = MainMenuParam.m_QuestHelper.unit;

            //-------------------------
            // ユニット設定
            //-------------------------
            int unitDataIndex = 0;
            List<PartyMemberUnitContext> unitList = new List<PartyMemberUnitContext>();
            for (int pt_count = 0; pt_count < party.PartyData.Length; ++pt_count)
            {
                PacketStructUnit unitData = party.PartyData[pt_count];
                PacketStructUnit linkData = party.PartyLinkData[pt_count];

                var unitDataModel = new PartyMemberUnitListItemModel((uint)unitDataIndex++);

                PartyMemberUnitContext unit = new PartyMemberUnitContext(unitDataModel);
                unit.IsActiveStatus = true;
                unit.IsActiveParamText = true;
                if (unitData != null)
                {
                    unit.CharaMaster = charaMasterList.Find((v) => v.fix_id == unitData.id);
                    UnitIconImageProvider.Instance.Get(
                        unitData.id,
                        sprite =>
                        {
                            unit.UnitImage = sprite;
                        });
                }
                else
                {
                    unit.OutSideCircleImage = ResourceManager.Instance.Load("icon_circle_deco", ResourceType.Common);
                    unit.UnitImage = ResourceManager.Instance.Load("icon_empty2", ResourceType.Menu);
                    unit.IsEnalbeSelect = true;
                }

                if (linkData != null && linkData.id > 0)
                {
                    unit.LinkCharaMaster = charaMasterList.Find((v) => v.fix_id == linkData.id);
                    UnitIconImageProvider.Instance.Get(
                        linkData.id,
                        sprite =>
                        {
                            unit.LinkUnitImage = sprite;
                        });
                    unit.IsEmptyLinkUnit = false;
                }
                else
                {
                    unit.LinkOutSideCircleImage = ResourceManager.Instance.Load("icon_circle_deco", ResourceType.Common);
                    unit.LinkUnitImage = ResourceManager.Instance.Load("icon_empty2", ResourceType.Menu);
                    unit.IsEmptyLinkUnit = true;
                }

                unit.UnitData = unitData;
                unit.LinkUnitData = linkData;
                unit.IsActiveFixFlag = party.IsPartyFix[pt_count];
                if ((index == m_PartyParamQuestPartyPanel.m_CurrentIndex) ||
                    (index == m_PartyParamQuestPartyPanel.m_CurrentIndex - 1) ||
                    (index == m_PartyParamQuestPartyPanel.m_CurrentIndex + 1))
                {
                    MainMenuUtil.SetPartySelectUnitData(ref unit, unitData, linkData, partyUnits);
                }
                else if (unit.OutSideCircleImage == null)
                {
                    unit.OutSideCircleImage = ResourceManager.Instance.Load("icon_circle_deco", ResourceType.Common);
                }
                unitDataModel.OnClicked += () =>
                {
                    OnSelectParamUnit(unit);
                };
                unitDataModel.OnLongPressed += () =>
                {
                    OnLongPressParamUnit(unit);
                };

                unitList.Add(unit);

                partyPanelModel.AddUnit(unitDataModel);
            }

            //-------------------------
            // フレンド設定
            //-------------------------
            { // TODO : 整理
                var friendUnitModel = new PartyMemberUnitListItemModel((uint)unitDataIndex++);

                PartyMemberUnitContext friendUnit = new PartyMemberUnitContext(friendUnitModel);
                friendUnit.IsActiveStatus = true;
                friendUnit.IsActiveParamText = true;
                friendUnit.UnitData = MainMenuParam.m_QuestHelper.unit;
                friendUnit.LinkUnitData = MainMenuParam.m_QuestHelper.unit_link;
                friendUnit.IsActiveFixFlag = party.IsPartyFix[(int)GlobalDefine.PartyCharaIndex.FRIEND];
                friendUnit.CharaMaster = charaMasterList.Find((v) => v.fix_id == MainMenuParam.m_QuestHelper.unit.id);
                UnitIconImageProvider.Instance.Get(
                    MainMenuParam.m_QuestHelper.unit.id,
                    sprite =>
                    {
                        friendUnit.UnitImage = sprite;
                    });
                if (friendUnit.LinkUnitData != null && friendUnit.LinkUnitData.id > 0)
                {
                    friendUnit.LinkCharaMaster = charaMasterList.Find((v) => v.fix_id == friendUnit.LinkUnitData.id);
                    UnitIconImageProvider.Instance.Get(
                         friendUnit.LinkUnitData.id,
                        sprite =>
                        {
                            friendUnit.LinkUnitImage = sprite;
                        });
                    friendUnit.IsEmptyLinkUnit = false;
                }
                else
                {
                    friendUnit.LinkOutSideCircleImage = ResourceManager.Instance.Load("icon_circle_deco", ResourceType.Common);
                    friendUnit.LinkUnitImage = ResourceManager.Instance.Load("icon_empty2", ResourceType.Menu);
                    friendUnit.IsEmptyLinkUnit = true;
                }
                MainMenuUtil.SetPartySelectUnitData(ref friendUnit, MainMenuParam.m_QuestHelper.unit, MainMenuParam.m_QuestHelper.unit_link, partyUnits);
                friendUnitModel.OnClicked += () =>
                {
                    OnSelectParamUnit(friendUnit);
                };
                friendUnitModel.OnLongPressed += () =>
                {
                    OnLongPressParamUnit(friendUnit);
                };

                unitList.Add(friendUnit);

                partyPanelModel.AddUnit(friendUnitModel);
            }

            unitList[0].PartyCharaIndex = GlobalDefine.PartyCharaIndex.LEADER;
            unitList[1].PartyCharaIndex = GlobalDefine.PartyCharaIndex.MOB_1;
            unitList[2].PartyCharaIndex = GlobalDefine.PartyCharaIndex.MOB_2;
            unitList[3].PartyCharaIndex = GlobalDefine.PartyCharaIndex.MOB_3;
            unitList[4].PartyCharaIndex = GlobalDefine.PartyCharaIndex.FRIEND;

            partyParam.Units = unitList;

            //-------------------------
            // スキル設定
            //-------------------------
            int skillDataIndex = 0;

            List<UnitSkillAtPartyContext> skillList = new List<UnitSkillAtPartyContext>();

            // TODO : 整理
            {
                var skillDataModel = new ListItemModel((uint)skillDataIndex++);
                UnitSkillAtPartyContext leaderSkill = new UnitSkillAtPartyContext(skillDataModel);
                leaderSkill.setupLeaderSkill(unitList[0].CharaMaster.skill_leader);
                skillList.Add(leaderSkill);

                partyPanelModel.AddSkill(skillDataModel);
            }

            {
                var skillDataModel = new ListItemModel((uint)skillDataIndex++);
                UnitSkillAtPartyContext friendSkill = new UnitSkillAtPartyContext(skillDataModel);
                friendSkill.setupFriendSkill(unitList[4].CharaMaster.skill_leader);
                skillList.Add(friendSkill);

                partyPanelModel.AddSkill(skillDataModel);
            }

            partyParam.Skills = skillList;

            partyParam.SelectLinkAction = OnSelectLink;
            m_PartyParamQuestPartyPanel.PartyParams.Add(partyParam);
        }

        //--------------------------------------
        // 主人公
        //--------------------------------------
        // アセットバンドルの読み込み
        uint currentHeroID = MasterDataUtil.GetCurrentHeroID();
        AssetBundler.Create().
        Set(string.Format("hero_{0:D4}", currentHeroID),
        (o) =>
        {
            Texture2D texture = o.GetTexture2D(string.Format("tex_hero_perform_l_{0:D4}", currentHeroID), TextureWrapMode.Clamp);
            Texture2D texture_mask = o.GetTexture2D(string.Format("tex_hero_perform_l_{0:D4}_mask", currentHeroID), TextureWrapMode.Clamp);

            foreach (var paertParam in m_PartyParamQuestPartyPanel.PartyParams)
            {
                paertParam.HeroImage = texture;
                paertParam.HeroImage_mask = texture_mask;
            }
        }).Load();
    }

    /// <summary>
    /// クエスト参加条件チェック
    /// </summary>
    /// <param name="quest_id"></param>
    /// <returns>true:入れる false:入れない</returns>
    bool ChkQuestRequirement(uint quest_id, List<PartyMemberUnitContext> currentParty)
    {
        MasterDataQuest2 questMaster = MasterDataUtil.GetQuest2ParamFromID(quest_id);
        if (questMaster == null) { return false; }

        // 条件指定なし
        if (questMaster.quest_requirement_id == 0) { return true; }

        // 条件マスター取得
        MasterDataQuestRequirement questrequirement_param = MasterFinder<MasterDataQuestRequirement>.Instance.Find((int)questMaster.quest_requirement_id);
        if (questrequirement_param == null) { return false; }

        PacketStructUnit[] acUnitStruct = {
            currentParty[0].UnitData,
            currentParty[1].UnitData,
            currentParty[2].UnitData,
            currentParty[3].UnitData,
            currentParty[4].UnitData
        };

        // マスタで指定されている必須属性、必須種族の情報を取得
        //--------------------------------
        // 指定属性のリスト作成処理
        //--------------------------------
        List<MasterDataDefineLabel.ElementType> m_ElemRequireList = new List<MasterDataDefineLabel.ElementType>();
        if (questrequirement_param.elem_fire == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_ElemRequireList.Add(MasterDataDefineLabel.ElementType.FIRE);
        }
        if (questrequirement_param.elem_water == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_ElemRequireList.Add(MasterDataDefineLabel.ElementType.WATER);
        }
        if (questrequirement_param.elem_wind == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_ElemRequireList.Add(MasterDataDefineLabel.ElementType.WIND);
        }
        if (questrequirement_param.elem_light == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_ElemRequireList.Add(MasterDataDefineLabel.ElementType.LIGHT);
        }
        if (questrequirement_param.elem_dark == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_ElemRequireList.Add(MasterDataDefineLabel.ElementType.DARK);
        }
        if (questrequirement_param.elem_naught == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_ElemRequireList.Add(MasterDataDefineLabel.ElementType.NAUGHT);
        }

        //--------------------------------
        // 指定種族のリスト作成処理
        //--------------------------------
        List<MasterDataDefineLabel.KindType> m_KindRequireList = new List<MasterDataDefineLabel.KindType>();
        if (questrequirement_param.kind_human == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_KindRequireList.Add(MasterDataDefineLabel.KindType.HUMAN);
        }
        if (questrequirement_param.kind_fairy == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_KindRequireList.Add(MasterDataDefineLabel.KindType.CREATURE);
        }
        if (questrequirement_param.kind_demon == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_KindRequireList.Add(MasterDataDefineLabel.KindType.DEMON);
        }
        if (questrequirement_param.kind_dragon == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_KindRequireList.Add(MasterDataDefineLabel.KindType.DRAGON);
        }
        if (questrequirement_param.kind_machine == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_KindRequireList.Add(MasterDataDefineLabel.KindType.MACHINE);
        }
        if (questrequirement_param.kind_beast == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_KindRequireList.Add(MasterDataDefineLabel.KindType.BEAST);
        }
        if (questrequirement_param.kind_god == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_KindRequireList.Add(MasterDataDefineLabel.KindType.GOD);
        }
        if (questrequirement_param.kind_egg == MasterDataDefineLabel.BoolType.ENABLE)
        {
            m_KindRequireList.Add(MasterDataDefineLabel.KindType.EGG);
        }

        //	判定用変数群
        MasterDataParamChara paramChara = null;
        //		bool					questAble		= true;
        int numElem = 0;
        int numKind = 0;
        //		int						numUnit			= 0;
        int numParty = 0;
        int numCostTotal = 0;
        uint numLevelTotal = 0;

        int numElem_bit = 0;
        int numKind_bit = 0;

        int numElemReq_Cnt = 0;
        int numKindReq_Cnt = 0;

        //--------------------------------
        // パーティメンバー全員に対し、条件確認を行う
        //--------------------------------
        for (int i = 0; i < acUnitStruct.Length; i++)
        {
            if (acUnitStruct[i] == null)
            {
                continue;
            }


            paramChara = MasterDataUtil.GetCharaParamFromID(acUnitStruct[i].id);
            if (paramChara == null)
            {
                continue;
            }

            // パーティメンバーの数カウント用
            numParty += 1;


            // 属性数カウント用
            if (paramChara.element != MasterDataDefineLabel.ElementType.NONE)
            {
                numElem_bit |= (1 << (int)paramChara.element);
            }

            // 種族数カウント用
            if (paramChara.kind != MasterDataDefineLabel.KindType.NONE)
            {
                numKind_bit |= (1 << (int)paramChara.kind);
            }
            if (paramChara.sub_kind != MasterDataDefineLabel.KindType.NONE)
            {
                numKind_bit |= (1 << (int)paramChara.sub_kind);
            }

            //--------------------------------
            // 属性チェック
            //--------------------------------
            {
                bool chk_elem = false;
                if (questrequirement_param.elem_fire != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.element == MasterDataDefineLabel.ElementType.FIRE
                )
                {
                    if (questrequirement_param.elem_fire == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numElemReq_Cnt++;
                    }
                    chk_elem = true;
                }
                if (questrequirement_param.elem_water != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.element == MasterDataDefineLabel.ElementType.WATER
                )
                {
                    if (questrequirement_param.elem_water == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numElemReq_Cnt++;
                    }
                    chk_elem = true;
                }
                if (questrequirement_param.elem_wind != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.element == MasterDataDefineLabel.ElementType.WIND
                )
                {
                    if (questrequirement_param.elem_wind == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numElemReq_Cnt++;
                    }
                    chk_elem = true;
                }
                if (questrequirement_param.elem_light != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.element == MasterDataDefineLabel.ElementType.LIGHT
                )
                {
                    if (questrequirement_param.elem_light == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numElemReq_Cnt++;
                    }
                    chk_elem = true;
                }
                if (questrequirement_param.elem_dark != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.element == MasterDataDefineLabel.ElementType.DARK
                )
                {
                    if (questrequirement_param.elem_dark == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numElemReq_Cnt++;
                    }
                    chk_elem = true;
                }
                if (questrequirement_param.elem_naught != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.element == MasterDataDefineLabel.ElementType.NAUGHT
                )
                {
                    if (questrequirement_param.elem_naught == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numElemReq_Cnt++;
                    }
                    chk_elem = true;
                }

                if (chk_elem == false)
                {
#if BUILD_TYPE_DEBUG
                    DebugLogger.StatAdd("Elem " + "Unit_No." + (i + 1));
#endif
                    return false;
                }
            }

            //--------------------------------
            // 種族チェック
            //--------------------------------
            {
                bool chk_kind = false;
                bool chk_kind_sub = false;

                //	メイン種族チェック
                if (questrequirement_param.kind_human != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.kind == MasterDataDefineLabel.KindType.HUMAN
                )
                {
                    if (questrequirement_param.kind_human == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind = true;
                }
                if (questrequirement_param.kind_fairy != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.kind == MasterDataDefineLabel.KindType.CREATURE
                )
                {
                    if (questrequirement_param.kind_fairy == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind = true;
                }
                if (questrequirement_param.kind_demon != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.kind == MasterDataDefineLabel.KindType.DEMON
                )
                {
                    if (questrequirement_param.kind_demon == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind = true;
                }
                if (questrequirement_param.kind_dragon != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.kind == MasterDataDefineLabel.KindType.DRAGON
                )
                {
                    if (questrequirement_param.kind_dragon == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind = true;
                }
                if (questrequirement_param.kind_machine != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.kind == MasterDataDefineLabel.KindType.MACHINE
                )
                {
                    if (questrequirement_param.kind_machine == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind = true;
                }
                if (questrequirement_param.kind_beast != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.kind == MasterDataDefineLabel.KindType.BEAST
                )
                {
                    if (questrequirement_param.kind_beast == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind = true;
                }
                if (questrequirement_param.kind_god != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.kind == MasterDataDefineLabel.KindType.GOD
                )
                {
                    if (questrequirement_param.kind_god == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind = true;
                }
                if (questrequirement_param.kind_egg != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.kind == MasterDataDefineLabel.KindType.EGG
                )
                {
                    if (questrequirement_param.kind_egg == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind = true;
                }

                //	サブ種族チェック
                if (questrequirement_param.kind_human != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.sub_kind == MasterDataDefineLabel.KindType.HUMAN
                )
                {
                    if (questrequirement_param.kind_human == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind_sub = true;
                }
                if (questrequirement_param.kind_fairy != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.sub_kind == MasterDataDefineLabel.KindType.CREATURE
                )
                {
                    if (questrequirement_param.kind_fairy == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind_sub = true;
                }
                if (questrequirement_param.kind_demon != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.sub_kind == MasterDataDefineLabel.KindType.DEMON
                )
                {
                    if (questrequirement_param.kind_demon == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind_sub = true;
                }
                if (questrequirement_param.kind_dragon != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.sub_kind == MasterDataDefineLabel.KindType.DRAGON
                )
                {
                    if (questrequirement_param.kind_dragon == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind_sub = true;
                }
                if (questrequirement_param.kind_machine != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.sub_kind == MasterDataDefineLabel.KindType.MACHINE
                )
                {
                    if (questrequirement_param.kind_machine == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind_sub = true;
                }
                if (questrequirement_param.kind_beast != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.sub_kind == MasterDataDefineLabel.KindType.BEAST
                )
                {
                    if (questrequirement_param.kind_beast == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind_sub = true;
                }
                if (questrequirement_param.kind_god != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.sub_kind == MasterDataDefineLabel.KindType.GOD
                )
                {
                    if (questrequirement_param.kind_god == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind_sub = true;
                }
                if (questrequirement_param.kind_egg != MasterDataDefineLabel.BoolType.DISABLE
                && paramChara.sub_kind == MasterDataDefineLabel.KindType.EGG
                )
                {
                    if (questrequirement_param.kind_egg == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        numKindReq_Cnt++;
                    }
                    chk_kind_sub = true;
                }

                //	サブのみ指定無しがありえるため、その場合は許可する
                if (paramChara.sub_kind == MasterDataDefineLabel.KindType.NONE)
                {
                    chk_kind_sub = true;
                }


                //	メイン、サブどちらかの条件を満たしていない場合、クエスト入場不可とする
                if (chk_kind == false
                || chk_kind_sub == false)
                {
#if BUILD_TYPE_DEBUG
                    if (chk_kind == false)
                    {
                        DebugLogger.StatAdd("Kind " + "Unit_No." + (i + 1));
                    }

                    if (chk_kind_sub == false)
                    {
                        DebugLogger.StatAdd("Kind_sub " + "Unit_No." + (i + 1));
                    }
#endif
                    return false;
                }


            }

            //--------------------------------
            // 同名ユニットチェック
            //--------------------------------
            {
                if (questrequirement_param.much_name == MasterDataDefineLabel.BoolType.ENABLE)
                {
                    for (int j = 0; j < acUnitStruct.Length; j++)
                    {
                        if (j == i)
                        {
                            continue;
                        }

                        if (acUnitStruct[j] == null)
                        {
                            continue;
                        }

                        if (acUnitStruct[j].id != paramChara.fix_id)
                        {
                            continue;
                        }

#if BUILD_TYPE_DEBUG
                        DebugLogger.StatAdd("MuchName " + acUnitStruct[j].id);
#endif
                        return false;
                    }
                }
            }

            //--------------------------------
            //	ユニットレベル制限
            //--------------------------------
            if (questrequirement_param.limit_unit_lv != 0
            && questrequirement_param.limit_unit_lv < acUnitStruct[i].level)
            {
#if BUILD_TYPE_DEBUG
                DebugLogger.StatAdd("Lv " + questrequirement_param.limit_unit_lv + " < " + acUnitStruct[i].level + "Unit_No." + (i + 1));
#endif
                return false;
            }
            numLevelTotal += acUnitStruct[i].level;


            //--------------------------------
            //	レア度制限
            //--------------------------------
            if (questrequirement_param.limit_rare < paramChara.rare)
            {
#if BUILD_TYPE_DEBUG
                DebugLogger.StatAdd("Rare " + questrequirement_param.limit_rare + " < " + paramChara.rare + "Unit_No." + (i + 1));
#endif
                return false;
            }


            //--------------------------------
            //	コスト制限
            //--------------------------------
            if (questrequirement_param.limit_cost != 0
            && questrequirement_param.limit_cost < paramChara.party_cost)
            {
#if BUILD_TYPE_DEBUG
                DebugLogger.StatAdd("Cost " + questrequirement_param.limit_cost + " < " + paramChara.party_cost + " Unit_No." + (i + 1));
#endif
                return false;
            }
            numCostTotal += paramChara.party_cost;

        }

        //--------------------------------
        //	合計コスト制限
        //--------------------------------
        if (questrequirement_param.limit_cost_total != 0
        && questrequirement_param.limit_cost_total < numCostTotal)
        {
#if BUILD_TYPE_DEBUG
            DebugLogger.StatAdd(" TotalCost " + questrequirement_param.limit_cost_total + " < " + numCostTotal);
#endif
            return false;
        }


        //--------------------------------
        //	合計レベル制限
        //--------------------------------
        if (questrequirement_param.limit_unit_lv_total != 0
        && questrequirement_param.limit_unit_lv_total < numLevelTotal)
        {
#if BUILD_TYPE_DEBUG
            DebugLogger.StatAdd(" TotalLevel " + questrequirement_param.limit_unit_lv_total + " < " + numLevelTotal);
#endif
            return false;
        }

        //--------------------------------
        //	必須属性チェック
        //--------------------------------
        if (m_ElemRequireList.Count > 0)
        {
            // 必須の属性が最低１体 PTに含まれているか
            if (numElemReq_Cnt <= 0)
            {
                //必須の属性が入っていないのでNG
#if BUILD_TYPE_DEBUG
                DebugLogger.StatAdd(" ElemReqCnt " + numElemReq_Cnt);
#endif
                return false;
            }
        }

        //--------------------------------
        //	必須種族チェック
        //--------------------------------
        if (m_KindRequireList.Count > 0)
        {

            // 必須の種族が最低１体 PTに含まれているか
            if (numKindReq_Cnt <= 0)
            {
                //必須の種族が入っていないのでNG
#if BUILD_TYPE_DEBUG
                DebugLogger.StatAdd(" KindReqCnt " + numKindReq_Cnt);
#endif
                return false;
            }
        }

        //--------------------------------
        //	属性数チェック
        //--------------------------------
        if (questrequirement_param.num_elem != 0)
        {
            for (int i = 0; i < (int)MasterDataDefineLabel.ElementType.MAX; i++)
            {
                if ((numElem_bit & (1 << i)) == 0)
                {
                    continue;
                }

                numElem_bit ^= (1 << i);

                numElem += 1;
            }
            if (questrequirement_param.num_elem != numElem)
            {
#if BUILD_TYPE_DEBUG
                DebugLogger.StatAdd(" NumElem " + questrequirement_param.num_elem + " < " + numElem);
#endif
                return false;
            }
        }


        //--------------------------------
        //	種族数チェック
        //--------------------------------
        if (questrequirement_param.num_kind != 0)
        {
            for (int i = 0; i < (int)MasterDataDefineLabel.KindType.MAX; i++)
            {
                if ((numKind_bit & (1 << i)) == 0)
                {
                    continue;
                }

                numKind_bit ^= (1 << i);

                numKind += 1;
            }
            if (questrequirement_param.num_kind != numKind)
            {
#if BUILD_TYPE_DEBUG
                DebugLogger.StatAdd(" NumKind " + questrequirement_param.num_kind + " < " + numKind);
#endif
                return false;
            }
        }


        //--------------------------------
        // パーティメンバー数チェック
        //--------------------------------
        if (questrequirement_param.num_unit != 0)
        {
            if (questrequirement_param.num_unit < numParty)
            {
#if BUILD_TYPE_DEBUG
                DebugLogger.StatAdd(" NumUnit " + questrequirement_param.num_unit + " < " + numParty);
#endif
                return false;
            }
        }


        //--------------------------------
        //	ユーザーランク制限
        //--------------------------------
        if (questrequirement_param.limit_rank != 0)
        {
            if (questrequirement_param.limit_rank > UserDataAdmin.Instance.m_StructPlayer.rank)
            {
#if BUILD_TYPE_DEBUG
                DebugLogger.StatAdd(" NumRank " + questrequirement_param.limit_rank + " < " + UserDataAdmin.Instance.m_StructPlayer.rank);
#endif
                return false;
            }
        }


        //--------------------------------
        //	必須ユニットIDチェック
        //--------------------------------
        bool chk_requireunit = false;

        uint[] unitFixID = {
            questrequirement_param.require_unit_00,
            questrequirement_param.require_unit_01,
            questrequirement_param.require_unit_02,
            questrequirement_param.require_unit_03,
            questrequirement_param.require_unit_04
        };

        //	１つのユニットを複数指定された場合の判定用に、IDをコピー
        uint[] partyUnitFixID = new uint[acUnitStruct.Length];
        for (int i = 0; i < acUnitStruct.Length; i++)
        {
            if (acUnitStruct[i] == null)
            {
                continue;
            }

            partyUnitFixID[i] = acUnitStruct[i].id;
        }


        for (int i = 0; i < unitFixID.Length; i++)
        {
            //	指定されていない
            if (unitFixID[i] == 0)
            {
                continue;
            }

            chk_requireunit = false;

            //	パーティ内に指定ユニットがいるか判定
            for (int j = 0; j < partyUnitFixID.Length; j++)
            {
                if (partyUnitFixID[j] == 0)
                {
                    continue;
                }

                if (partyUnitFixID[j] == unitFixID[i])
                {
                    //	一致するユニットが居た場合は、
                    //	判定済みとして配列から消す。
                    partyUnitFixID[j] = 0;
                    chk_requireunit = true;
                    break;
                }
            }

            //	指定のユニットがいない
            if (chk_requireunit == false)
            {
#if BUILD_TYPE_DEBUG
                DebugLogger.StatAdd(" RequireUnit " + unitFixID[i]);
#endif
                return false;
            }
        }

        //	クエスト入場可能
        return true;
    }

    /// <summary>
    /// パーティコストを超えるかチェック
    /// </summary>
    /// <returns>true:コストオーバー false:コスト内</returns>
    bool CheckCostOver(List<PartyMemberUnitContext> currentParty)
    {
        bool bAssignedCostOver = false;
        int nAssignedCost = 0;

        if (m_IsOverrideAssign)
        {
            // 固定パーティ上書きがある場合
            return false;
        }

        for (int i = 0; i < currentParty.Count; ++i)
        {
            if (currentParty[i].PartyCharaIndex == GlobalDefine.PartyCharaIndex.FRIEND) { continue; }
            PacketStructUnit unitData = currentParty[i].UnitData;
            PacketStructUnit linkData = currentParty[i].LinkUnitData;
            if (unitData == null) { continue; }

            nAssignedCost += ServerDataUtil.GetPacketUnitCost(unitData);
            // リンクユニット分のコストを加算
            if (linkData != null)
            {
                nAssignedCost += CharaLinkUtil.GetLinkUnitCost(linkData.id);
            }
        }

        if (nAssignedCost > UserDataAdmin.Instance.m_StructPlayer.total_party)
        {
            bAssignedCostOver = true;
        }

        return bAssignedCostOver;
    }

    /// <summary>
    /// 変更されたパーティー番号のユニットのステータスを更新する.
    /// </summary>
    /// <param name="party">変更対象パーティー</param>
    void UpdatePartyUnitStatus(int unitPartyIndex, PartySelectGroupUnitContext party)
    {
        if (party == null)
        {
            return;
        }

        //---------------------------------------------------------------------------
        // リーダースキル、フレンドスキル参照用のパーティー情報設定
        //---------------------------------------------------------------------------
        var partyUnits = new PacketStructUnit[(int)GlobalDefine.PartyCharaIndex.MAX];
        partyUnits[(int)GlobalDefine.PartyCharaIndex.LEADER] = party.PartyData[(int)GlobalDefine.PartyCharaIndex.LEADER];
        partyUnits[(int)GlobalDefine.PartyCharaIndex.FRIEND] = MainMenuParam.m_QuestHelper.unit;

        var list = m_PartyParamQuestPartyPanel.PartyParamList[unitPartyIndex].GetComponent<PartyParamQuestListItem>();
        for (int pt_count = 0; pt_count < party.PartyData.Length; ++pt_count)
        {
            var unitData = party.PartyData[pt_count];
            var linkData = party.PartyLinkData[pt_count];

            var unit = list.Context.UnitList[pt_count].GetComponent<PartyParamUnitListItem>().Context;
            MainMenuUtil.SetPartySelectUnitData(ref unit, unitData, linkData, partyUnits);
        }
    }

    /// <summary>
    /// パーティリストを選択したとき
    /// </summary>
    /// <param name="party"></param>
    void OnSelectPartyGroup(PartySelectGroupUnitContext party)
    {
        var index = party.Index;
        // 変更されたパーティー番号のユニットのステータスを更新する.
        UpdatePartyUnitStatus(index, party);

        if (m_PartyParamQuestPartyPanel != null)
        {
            m_PartyParamQuestPartyPanel.ChangeParam(index);
        }
        m_PartySelectGroup.ChangePartyItemSelect(index); // パーティ選択状態を変更
    }

    /// <summary>
    /// パーティのユニットを選択したとき
    /// </summary>
    /// <param name="_unit"></param>
    void OnSelectParamUnit(PartyMemberUnitContext _unit)
    {
        if (_unit.PartyCharaIndex != GlobalDefine.PartyCharaIndex.FRIEND)
        {
            OnSelectMovePartyAssign();
        }
        else
        {
            // フレンド選択画面に移る
            SoundUtil.PlaySE(SEID.SE_MENU_OK);
            SelectReturn();
        }
    }

    /// <summary>
    /// パーティのユニットを長押ししたとき
    /// </summary>
    /// <param name="_unit"></param>
    void OnLongPressParamUnit(PartyMemberUnitContext _unit)
    {
        if (_unit.UnitData != null && _unit.UnitData.id > 0 && MainMenuManager.HasInstance)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);

            bool isOwnUnit = (_unit.PartyCharaIndex != GlobalDefine.PartyCharaIndex.FRIEND);
            m_PartyParamQuestPartyPanel.m_CarouselRotator.SetIndex(m_PartyParamQuestPartyPanel.m_CurrentIndex, true);
            MainMenuManager.Instance.OpenUnitDetailInfoPlayer(_unit.UnitData, _unit.LinkUnitData, true, isOwnUnit);
        }
    }

    void OnChangedPartyParam(int index)
    {
        // 移動先パーティー情報更新
        var partyIndices = new int[]
        {
            index,
            index - 1,
            index + 1
        };
        for (var i = 0; i < partyIndices.Length; i++)
        {
            if (partyIndices[i] > -1 &&
                partyIndices[i] < UserDataAdmin.Instance.m_StructPartyAssign.Length)
            {
                var party = m_PartySelectGroup.GetParty(partyIndices[i]);
                UpdatePartyUnitStatus(partyIndices[i], party);
            }
        }

        m_PartySelectGroup.ChangePartyItemSelect(index, true); // パーティ選択状態を変更
    }

    void OnClickNextButton()
    {
        if (MainMenuManager.Instance.IsPageSwitch() ||
            ServerApi.IsExists)
        {
            return;
        }

#if BUILD_TYPE_DEBUG
        // 開始バトル選択
        MainMenuParam.m_DebugBattleNumber = 0;
        if (DebugOption.Instance.disalbeDebugMenu == false)
        {
            Transform debug_select_battle_dropdown = m_PartySelectButtonPanel.transform.Find("DebugSelectBattleDropdown");
            if (debug_select_battle_dropdown != null)
            {
                if (debug_select_battle_dropdown.gameObject.IsActive())
                {
                    Dropdown dropdown = debug_select_battle_dropdown.GetComponent<Dropdown>();
                    if (dropdown != null)
                    {
                        MainMenuParam.m_DebugBattleNumber = dropdown.value;
                    }
                }
            }
        }
#endif //BUILD_TYPE_DEBUG

        int partyCurrent = m_PartyParamQuestPartyPanel.m_CurrentIndex;
        List<PartyMemberUnitContext> currentParty = m_PartyParamQuestPartyPanel.PartyParams[partyCurrent].Units;

        //----------------------------------------
        // パーティコストチェック
        //----------------------------------------
        if (CheckCostOver(currentParty))
        {
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "kk110q_title");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "kk110q_content");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
            {

            });
            newDialog.Show();
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
            return;
        }

        //----------------------------------------
        // クエスト入場条件チェック
        //----------------------------------------
        if (ChkQuestRequirement(MainMenuParam.m_QuestSelectMissionID, currentParty) == false)
        {
            //----------------------------------------
            // 入場条件満たしてなければここで終わり
            //----------------------------------------
            string requirement_text = ""; // クエスト入場条件テキスト
            MasterDataQuest2 quest_param = MasterDataUtil.GetQuest2ParamFromID(MainMenuParam.m_QuestSelectMissionID);
            if (quest_param != null)
            {
                requirement_text = quest_param.quest_requirement_text;
            }

            Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "quest_notcondition_title");
            newDialog.SetDialogText(DialogTextType.MainText, string.Format(GameTextUtil.GetText("quest_notcondition_content"), requirement_text));
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.SetTextAlignment(DialogTextType.MainText, TMPro.TextAlignmentOptions.Center);
            newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
            {

            });
            newDialog.Show();

            SoundUtil.PlaySE(SEID.SE_MENU_RET);
            return;
        }

        // 所持上限チェック
        ParamCheck(() =>
        {
            //--------------------------------
            // 次のフローへ
            //--------------------------------
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);

            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.TO_HOME);

            //カレントパーティ設定
            UserDataAdmin.Instance.m_StructPlayer.unit_party_current = partyCurrent;
            //クエスト開始
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SERVER_SEND, false, false);

            AndroidBackKeyManager.Instance.DisableBackKey();
        });
    }

    void OnClickAutoPlayButton()
    {
        if (MainMenuManager.Instance.IsPageSwitch() ||
            ServerApi.IsExists)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        bool new_value = !m_PartySelectButtonPanel.IsActiveAutoPlayOnButton;
        m_PartySelectButtonPanel.IsActiveAutoPlayOnButton = new_value;
        m_PartySelectButtonPanel.IsActiveAutoPlayOffButton = !new_value;

        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
        cOption.m_OptionAutoPlayEnable = (new_value ? (int)LocalSaveDefine.OptionAutoPlayEnable.ON : (int)LocalSaveDefine.OptionAutoPlayEnable.OFF);
    }

    public void OnSelectMovePartyAssign()
    {
        if (m_PartySelectButtonPanel.m_MemberButtonModel.isEnabled == false)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        //カレントパーティ設定
        UserDataAdmin.Instance.m_StructPlayer.unit_party_current = m_PartyParamQuestPartyPanel.m_CurrentIndex;

        // パーティユニット選択画面に移る
        MainMenuParam.m_PartyAssignPrevPage = MAINMENU_SEQ.SEQ_QUEST_SELECT_PARTY;
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_PARTY_FORM, false, false);
    }

    public void OnSelectLink()
    {
        m_PartyParamQuestPartyPanel.SetUpLinkUnit();
    }

    public void SelectReturn()
    {
        if (!MainMenuManager.HasInstance)
        {
            return;
        }

        // エリア移動
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_FRIEND, false, true);
    }

    private void SetUpAppearAnimation()
    {
        // 今のつくりだと前回の状態が残っているので
        m_PartySelectGroup.Hide();

        EffectProcessor.Instance.Register("MainMenuUnitPartySelect", (System.Action finish) =>
        {
            foreach (var partyPanel in m_partyPanels)
                partyPanel.Appear();

            m_PartySelectGroup.Show();
            finish();
        });
    }

    void RequirementParty(PartySelectGroupUnitContext partyGroup)
    {
        //--------------------------------
        //	クエスト制限によるパーティメンバーの上書き処理
        //--------------------------------
        MasterDataQuest2 quest_param = MasterDataUtil.GetQuest2ParamFromID(MainMenuParam.m_QuestSelectMissionID);
        if (quest_param != null)
        {
            MasterDataQuestRequirement requirement_param = MasterDataUtil.GetMasterDataQuestRequirementFromID(quest_param.quest_requirement_id);
            if (requirement_param != null)
            {
                uint unBaseUnitFixID = 0;
                uint party_length = (int)GlobalDefine.PartyCharaIndex.MAX;
                for (int loop_pt_idx = 0; loop_pt_idx < party_length; ++loop_pt_idx)
                {
                    GlobalDefine.PartyCharaIndex partyCharaIndex = (GlobalDefine.PartyCharaIndex)loop_pt_idx;
                    if (IsFixUnitAssign(partyCharaIndex, requirement_param) == true)
                    {
                        // ベースユニットが固定されている場合
                        switch (partyCharaIndex)
                        {
                            case GlobalDefine.PartyCharaIndex.LEADER: unBaseUnitFixID = requirement_param.fix_unit_00_id; break;
                            case GlobalDefine.PartyCharaIndex.MOB_1: unBaseUnitFixID = requirement_param.fix_unit_01_id; break;
                            case GlobalDefine.PartyCharaIndex.MOB_2: unBaseUnitFixID = requirement_param.fix_unit_02_id; break;
                            case GlobalDefine.PartyCharaIndex.MOB_3: unBaseUnitFixID = requirement_param.fix_unit_03_id; break;
                            case GlobalDefine.PartyCharaIndex.FRIEND: unBaseUnitFixID = requirement_param.fix_unit_04_id; break;
                        }
                        if (unBaseUnitFixID != 0)
                        {
                            // Fixユニット
                            partyGroup.IsPartyFix[loop_pt_idx] = true;
                        }
                        else
                        {
                            // Fixではないユニット
                            partyGroup.IsPartyFix[loop_pt_idx] = false;
                        }
                        m_IsOverrideAssign = true;
                    }
                    else
                    {
                        // Fixではないユニット
                        partyGroup.IsPartyFix[loop_pt_idx] = false;
                    }

                    // パーティ情報の改竄
                    if (partyCharaIndex != GlobalDefine.PartyCharaIndex.FRIEND)
                    {
                        FixUnitAssign(ref partyGroup.PartyData[loop_pt_idx], ref partyGroup.PartyLinkData[loop_pt_idx], partyCharaIndex, requirement_param);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 固定パーティ情報で上書きが発生するか
    /// </summary>
    private bool IsFixUnitAssign(GlobalDefine.PartyCharaIndex party_index, MasterDataQuestRequirement requirement_param)
    {
        bool is_fix_unit = false;

        MasterDataDefineLabel.BoolType fix_enable = MasterDataDefineLabel.BoolType.NONE;
        MasterDataDefineLabel.BoolType fix_link_enable = MasterDataDefineLabel.BoolType.NONE;


        switch (party_index)
        {
            default:
            case GlobalDefine.PartyCharaIndex.LEADER:
                //	情報格納：リーダー
                fix_enable = requirement_param.fix_unit_00_enable;
                fix_link_enable = requirement_param.fix_unit_00_link_enable;
                break;
            case GlobalDefine.PartyCharaIndex.MOB_1:
                //	情報格納：サブ１
                fix_enable = requirement_param.fix_unit_01_enable;
                fix_link_enable = requirement_param.fix_unit_01_link_enable;
                break;
            case GlobalDefine.PartyCharaIndex.MOB_2:
                //	情報格納：サブ２
                fix_enable = requirement_param.fix_unit_02_enable;
                fix_link_enable = requirement_param.fix_unit_02_link_enable;
                break;
            case GlobalDefine.PartyCharaIndex.MOB_3:
                //	情報格納：サブ３
                fix_enable = requirement_param.fix_unit_03_enable;
                fix_link_enable = requirement_param.fix_unit_03_link_enable;
                break;
            case GlobalDefine.PartyCharaIndex.FRIEND:
                //	情報格納：フレンド
                fix_enable = requirement_param.fix_unit_04_enable;
                fix_link_enable = requirement_param.fix_unit_04_link_enable;
                break;
        }

        // 固定判定
        if (fix_enable == MasterDataDefineLabel.BoolType.ENABLE
        || fix_link_enable == MasterDataDefineLabel.BoolType.ENABLE)
        {
            is_fix_unit = true;
        }
        else if (fix_enable == MasterDataDefineLabel.BoolType.DISABLE)
        {
            // ベースのリーダー、フレンドは「無し」にはならない
            if (party_index != GlobalDefine.PartyCharaIndex.LEADER
            && party_index != GlobalDefine.PartyCharaIndex.FRIEND)
            {
                is_fix_unit = true;
            }
        }

        if (fix_link_enable == MasterDataDefineLabel.BoolType.DISABLE)
        {
            is_fix_unit = true;
        }

        return is_fix_unit;
    }

    /// <summary>
    /// 固定パーティ情報で上書き：単体
    /// </summary>
    private bool FixUnitAssign(ref PacketStructUnit unit, ref PacketStructUnit linkUnit, GlobalDefine.PartyCharaIndex party_index, MasterDataQuestRequirement requirement_param)
    {
        bool override_assign = false;

        MasterDataDefineLabel.BoolType fix_enable = MasterDataDefineLabel.BoolType.NONE;
        uint fix_unit_id;
        int fix_unit_lv;
        int fix_unit_lv_lbs;
        int fix_unit_lv_lo;
        int fix_unit_add_hp;
        int fix_unit_add_atk;
        MasterDataDefineLabel.BoolType fix_link_enable = MasterDataDefineLabel.BoolType.NONE;
        uint fix_unit_link_id;
        int fix_unit_link_lv;
        int fix_unit_link_add_hp;
        int fix_unit_link_add_atk;
        int fix_unit_link_point;
        int fix_unit_link_lv_lo;

        switch (party_index)
        {
            default:
            case GlobalDefine.PartyCharaIndex.LEADER:
                //	情報格納：リーダー
                fix_enable = requirement_param.fix_unit_00_enable;
                fix_unit_id = requirement_param.fix_unit_00_id;
                fix_unit_lv = requirement_param.fix_unit_00_lv;
                fix_unit_lv_lbs = requirement_param.fix_unit_00_lv_lbs;
                fix_unit_lv_lo = requirement_param.fix_unit_00_lv_lo;
                fix_unit_add_hp = requirement_param.fix_unit_00_plus_hp;
                fix_unit_add_atk = requirement_param.fix_unit_00_plus_atk;
                fix_link_enable = requirement_param.fix_unit_00_link_enable;
                fix_unit_link_id = requirement_param.fix_unit_00_link_id;
                fix_unit_link_lv = requirement_param.fix_unit_00_link_lv;
                fix_unit_link_add_hp = requirement_param.fix_unit_00_link_plus_hp;
                fix_unit_link_add_atk = requirement_param.fix_unit_00_link_plus_atk;
                fix_unit_link_point = requirement_param.fix_unit_00_link_point;
                fix_unit_link_lv_lo = requirement_param.fix_unit_00_link_lv_lo;
                break;
            case GlobalDefine.PartyCharaIndex.MOB_1:
                //	情報格納：サブ１
                fix_enable = requirement_param.fix_unit_01_enable;
                fix_unit_id = requirement_param.fix_unit_01_id;
                fix_unit_lv = requirement_param.fix_unit_01_lv;
                fix_unit_lv_lbs = requirement_param.fix_unit_01_lv_lbs;
                fix_unit_lv_lo = requirement_param.fix_unit_01_lv_lo;
                fix_unit_add_hp = requirement_param.fix_unit_01_plus_hp;
                fix_unit_add_atk = requirement_param.fix_unit_01_plus_atk;
                fix_link_enable = requirement_param.fix_unit_01_link_enable;
                fix_unit_link_id = requirement_param.fix_unit_01_link_id;
                fix_unit_link_lv = requirement_param.fix_unit_01_link_lv;
                fix_unit_link_add_hp = requirement_param.fix_unit_01_link_plus_hp;
                fix_unit_link_add_atk = requirement_param.fix_unit_01_link_plus_atk;
                fix_unit_link_point = requirement_param.fix_unit_01_link_point;
                fix_unit_link_lv_lo = requirement_param.fix_unit_01_link_lv_lo;
                break;
            case GlobalDefine.PartyCharaIndex.MOB_2:
                //	情報格納：サブ２
                fix_enable = requirement_param.fix_unit_02_enable;
                fix_unit_id = requirement_param.fix_unit_02_id;
                fix_unit_lv = requirement_param.fix_unit_02_lv;
                fix_unit_lv_lbs = requirement_param.fix_unit_02_lv_lbs;
                fix_unit_lv_lo = requirement_param.fix_unit_02_lv_lo;
                fix_unit_add_hp = requirement_param.fix_unit_02_plus_hp;
                fix_unit_add_atk = requirement_param.fix_unit_02_plus_atk;
                fix_link_enable = requirement_param.fix_unit_02_link_enable;
                fix_unit_link_id = requirement_param.fix_unit_02_link_id;
                fix_unit_link_lv = requirement_param.fix_unit_02_link_lv;
                fix_unit_link_add_hp = requirement_param.fix_unit_02_link_plus_hp;
                fix_unit_link_add_atk = requirement_param.fix_unit_02_link_plus_atk;
                fix_unit_link_point = requirement_param.fix_unit_02_link_point;
                fix_unit_link_lv_lo = requirement_param.fix_unit_02_link_lv_lo;
                break;
            case GlobalDefine.PartyCharaIndex.MOB_3:
                //	情報格納：サブ３
                fix_enable = requirement_param.fix_unit_03_enable;
                fix_unit_id = requirement_param.fix_unit_03_id;
                fix_unit_lv = requirement_param.fix_unit_03_lv;
                fix_unit_lv_lbs = requirement_param.fix_unit_03_lv_lbs;
                fix_unit_lv_lo = requirement_param.fix_unit_03_lv_lo;
                fix_unit_add_hp = requirement_param.fix_unit_03_plus_hp;
                fix_unit_add_atk = requirement_param.fix_unit_03_plus_atk;
                fix_link_enable = requirement_param.fix_unit_03_link_enable;
                fix_unit_link_id = requirement_param.fix_unit_03_link_id;
                fix_unit_link_lv = requirement_param.fix_unit_03_link_lv;
                fix_unit_link_add_hp = requirement_param.fix_unit_03_link_plus_hp;
                fix_unit_link_add_atk = requirement_param.fix_unit_03_link_plus_atk;
                fix_unit_link_point = requirement_param.fix_unit_03_link_point;
                fix_unit_link_lv_lo = requirement_param.fix_unit_03_link_lv_lo;
                break;
            case GlobalDefine.PartyCharaIndex.FRIEND:
                //	情報格納：フレンド
                fix_enable = requirement_param.fix_unit_04_enable;
                fix_unit_id = requirement_param.fix_unit_04_id;
                fix_unit_lv = requirement_param.fix_unit_04_lv;
                fix_unit_lv_lbs = requirement_param.fix_unit_04_lv_lbs;
                fix_unit_lv_lo = requirement_param.fix_unit_04_lv_lo;
                fix_unit_add_hp = requirement_param.fix_unit_04_plus_hp;
                fix_unit_add_atk = requirement_param.fix_unit_04_plus_atk;
                fix_link_enable = requirement_param.fix_unit_04_link_enable;
                fix_unit_link_id = requirement_param.fix_unit_04_link_id;
                fix_unit_link_lv = requirement_param.fix_unit_04_link_lv;
                fix_unit_link_add_hp = requirement_param.fix_unit_04_link_plus_hp;
                fix_unit_link_add_atk = requirement_param.fix_unit_04_link_plus_atk;
                fix_unit_link_point = requirement_param.fix_unit_04_link_point;
                fix_unit_link_lv_lo = requirement_param.fix_unit_04_link_lv_lo;
                break;
        }


        // 元のリンクユニークIDを保存
        long lLinkUniqueID = 0;
        if (unit != null)
        {
            lLinkUniqueID = unit.link_unique_id;
        }

        // ベースユニット強制置き換え
        switch (fix_enable)
        {
            case MasterDataDefineLabel.BoolType.ENABLE:
                {
                    // 上書き
                    if (fix_unit_id != 0)
                    {
                        unit = MainMenuUtil.CreateDummyUnit(fix_unit_id,
                                                            (uint)fix_unit_lv,
                                                            (uint)fix_unit_lv_lbs,
                                                            (uint)fix_unit_lv_lo,
                                                            (uint)fix_unit_add_atk,
                                                            (uint)fix_unit_add_hp);
                    }
                    else
                    {
                        // リーダー、フレンドは空にしない
                        if (party_index != GlobalDefine.PartyCharaIndex.LEADER
                        && party_index != GlobalDefine.PartyCharaIndex.FRIEND)
                        {
                            // 空にする
                            unit = null;
                        }
                    }

                    override_assign = true;
                }
                break;

            case MasterDataDefineLabel.BoolType.DISABLE:
                {
                    // リーダー、フレンドは空にしない
                    if (party_index != GlobalDefine.PartyCharaIndex.LEADER
                    && party_index != GlobalDefine.PartyCharaIndex.FRIEND)
                    {
                        // 空にする
                        unit = null;

                        override_assign = true;
                    }
                }
                break;

            case MasterDataDefineLabel.BoolType.NONE:
                // リンクユニットのみを固定する場合
                if (fix_link_enable != MasterDataDefineLabel.BoolType.NONE)
                {
                    // パーティにベースユニットが設定されていない場合
                    if (unit == null)
                    {
                        return override_assign;
                    }

                    // 複製
                    unit = MainMenuUtil.CreateDummyUnit(unit.id, unit.level, unit.limitbreak_lv, unit.limitover_lv, unit.add_pow, unit.add_hp);
                }
                break;
        }


        // パーティにベースユニットが設定されていない場合
        if (unit == null)
        {
            return override_assign;
        }

        // リンクユニット強制置き換え
        switch (fix_link_enable)
        {
            case MasterDataDefineLabel.BoolType.ENABLE:

                if (fix_unit_link_id != 0)
                {
                    unit.link_unique_id = 1;
                    unit.link_point = (uint)fix_unit_link_point;
                    unit.link_info = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE;

                    linkUnit = MainMenuUtil.CreateDummyUnit(fix_unit_link_id,
                                                            (uint)fix_unit_link_lv,
                                                            0,//skill_level
                                                            (uint)fix_unit_link_lv_lo,
                                                            (uint)fix_unit_link_add_atk,
                                                            (uint)fix_unit_link_add_hp,
                                                             0, (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK);
                }
                else
                {
                    unit.link_unique_id = 0;
                    unit.link_point = 0;
                    unit.link_info = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE;
                    linkUnit = MainMenuUtil.CreateDummyUnit(0, 0, 0, 0, 0, 0);
                }

                override_assign = true;
                break;

            case MasterDataDefineLabel.BoolType.DISABLE:
                unit.link_unique_id = 0;
                unit.link_point = 0;
                unit.link_info = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE;

                linkUnit = MainMenuUtil.CreateDummyUnit(0, 0, 0, 0, 0, 0);

                override_assign = true;
                break;

            case MasterDataDefineLabel.BoolType.NONE:
                linkUnit = CharaLinkUtil.GetLinkUnit(lLinkUniqueID);

                // ベースユニットが固定されている場合
                if (fix_enable != MasterDataDefineLabel.BoolType.NONE)
                {
                    if (linkUnit != null)
                    {
                        unit.link_unique_id = 1;
                        unit.link_point = (uint)fix_unit_link_point;
                        unit.link_info = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE;

                        linkUnit = MainMenuUtil.CreateDummyUnit(linkUnit.id,
                                                                linkUnit.level,
                                                                0,//skill_level
                                                                linkUnit.limitover_lv,
                                                                linkUnit.add_pow,
                                                                linkUnit.add_hp,
                                                                 0, (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK);
                    }
                    else
                    {
                        unit.link_unique_id = 0;
                        unit.link_point = 0;
                        unit.link_info = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE;
                        linkUnit = MainMenuUtil.CreateDummyUnit(0, 0, 0, 0, 0, 0);
                    }
                }
                break;
        }

        return override_assign;
    }

    /// <summary>
    /// パラメータ上限チェック
    /// </summary>
    private void ParamCheck(Action nextAction)
    {
        //----------------------------------------
        // パラメータリミットチェック
        //----------------------------------------
        //選択中のフレンドのフレンドポイント取得
        uint iAddFp = MainMenuUtil.GetSelectFriendPoint(MainMenuParam.m_QuestHelper, MainMenuParam.m_QuestEventFP);
        //チェック対象：コイン、チケット、フレンドポイント
        PRM_LIMIT_ERROR_TYPE ret = MainMenuUtil.ChkPrmLimit(MainMenuParam.m_QuestAddMoney, 1, iAddFp, 0, 0);
        //チェック対象：消費アイテム全て
        ret = MainMenuUtil.ChkPrmLimitItem(1, -1, ret);
        //チェック対象：クエストキー全て
        ret = MainMenuUtil.ChkPrmLimitQuestKey(1, -1, ret);

        MainMenuUtil.ShowParamLimitDialog(ret, DialogType.DialogYesNo, (isPositive) =>
        {
            if (isPositive == true && nextAction != null)
            {
                nextAction();
            }
        });
    }

    /// <summary>
    /// パーティー編成ボタンの状態を変更
    /// </summary>
    void updateStateMemberSettingButton()
    {
        bool selected = true;
        if (m_IsOverrideAssign == true)
        {
            List<PartyMemberUnitContext> party = m_PartyParamQuestPartyPanel.PartyParams[0].Units;
            for (int i = 0; i < party.Count; ++i)
            {
                PartyMemberUnitContext unit = party[i];

                if (unit.IsActiveFixFlag == false)
                {
                    continue;
                }

                if (unit.PartyCharaIndex != GlobalDefine.PartyCharaIndex.FRIEND)
                {
                    selected = false;
                    break;
                }
            }
        }

        m_PartySelectButtonPanel.m_MemberButtonModel.isEnabled = selected;
    }
}
