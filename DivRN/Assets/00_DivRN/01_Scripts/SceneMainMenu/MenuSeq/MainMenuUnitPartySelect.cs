/**
 *  @file   MainMenuUnitPartySelect.cs
 *  @brief
 *  @author Developer
 *  @date   2017/02/15
 */

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using ServerDataDefine;

public class MainMenuUnitPartySelect : MainMenuSeq
{

    /// <summary>パーティ選択</summary>
    PartySelectGroup m_PartySelectGroup = null;
    /// <summary>パーティの詳細</summary>
    PartyParamPanel m_PartyParamPanel = null;
    /// <summary>下部ボタン</summary>
    PartySelectButtonPanel m_PartySelectButtonPanel = null;
    /// <summary>API送信中かどうか</summary>
    bool m_IsSendApi;
    /// <summary>API送信したかどうか</summary>
    bool m_IsRequestedApi;

    int m_PartyAssignLength = -1;

    //DG0-2521 リロード時のパーティ選択保護
    static readonly int DefaultUnitPartyCurrent = -1;
    static int m_UnitPartyCurrent = DefaultUnitPartyCurrent;

    private List<PartyParamListItemModel> m_partyPanels = new List<PartyParamListItemModel>();

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

        m_IsRequestedApi = false;

        //--------------------------------
        //	オブジェクトの取得
        //--------------------------------
        m_PartySelectGroup = m_CanvasObj.GetComponentInChildren<PartySelectGroup>();
        m_PartySelectGroup.SetPositionAjustStatusBar(new Vector2(0, -132));
        m_PartySelectGroup.SetSizeParfect(new Vector2(0, 128));

        m_PartyParamPanel = m_CanvasObj.GetComponentInChildren<PartyParamPanel>();
        m_PartyParamPanel.SetPositionAjustStatusBar(new Vector2(0, -280));

        m_PartySelectButtonPanel = m_CanvasObj.GetComponentInChildren<PartySelectButtonPanel>();
        m_PartySelectButtonPanel.SetPositionAjustStatusBar(new Vector2(0, -714));

        // パーティ一覧の描画
        if (m_UnitPartyCurrent == DefaultUnitPartyCurrent)
        {
            m_UnitPartyCurrent = UserDataAdmin.Instance.m_StructPlayer.unit_party_current;
        }

        CreatePartyList();
        CreatePartyParam();
        m_PartyAssignLength = UserDataAdmin.Instance.m_StructPartyAssign.Length;

        // 下部ボタン
        m_PartySelectButtonPanel.IsActiveMemberSettingButton = true;
        m_PartySelectButtonPanel.MemberSettingButtonText = GameTextUtil.GetText("party_button");
        m_PartySelectButtonPanel.MemberSettingAction = OnSelectMovePartyAssign;
        m_PartySelectButtonPanel.SetUpButtons(false);

        SetUpAppearAnimation();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.UNIT;

        StartCoroutine(sohwArrow(() =>
        {
            MainMenuParam.m_PartySelectIsShowLinkUnit = MainMenuParam.m_PartySelectShowedLinkUnit;
            MainMenuParam.m_PartySelectShowedLinkUnit = false;
            m_PartyParamPanel.SetUpLinkUnit(MainMenuParam.m_PartySelectIsShowLinkUnit);
        }));
    }

    IEnumerator sohwArrow(Action finishAction)
    {
        while (m_PartyParamPanel.PartyParamList.Count == 0)
        {
            yield return null;
        }
        PartyParamListItem list = m_PartyParamPanel.PartyParamList[m_UnitPartyCurrent].GetComponent<PartyParamListItem>();
        if (list != null)
        {
            while (list.Context.UnitList.Count < 4)
            {
                yield return null;
            }
            PartyParamUnitListItem ulist = list.Context.UnitList[3].GetComponent<PartyParamUnitListItem>();
            if (ulist != null)
            {
                while (ulist.isShowedStatus == false)
                {
                    yield return null;
                }
                while (m_PartySelectGroup.isSelectGroupIconSowed() == false)
                {
                    yield return null;
                }
                m_PartySelectGroup.firstShowArrow();
                if (finishAction != null)
                {
                    finishAction();
                }
            }
        }
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

    public override bool PageSwitchEventDisableBefore()
    {
        base.PageSwitchEventDisableBefore();

        if (m_IsRequestedApi == false)
        {
            SendUnitPartyAssign();
        }

        return false;
    }

    public override void PageUpdateStatusFromMainMenu(GLOBALMENU_SEQ seq)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("MainMenuUnitPartySelect UpdateUserStatusFromGlobalMenu:" + seq);
#endif

        //[DG0-2521] 【5.0.1対応】ユニット編成画面でミッションから「パーティ枠拡張」を受け取った後に特定操作を行うと、Odinエラーの後に進行不能となる
        switch (seq)
        {
            case GLOBALMENU_SEQ.MISSION:
                if (m_PartyAssignLength != UserDataAdmin.Instance.m_StructPartyAssign.Length)
                {
                    m_IsRequestedApi = true;
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_PARTY_SELECT, false, false);
                }
                break;
            case GLOBALMENU_SEQ.TITLE:
                m_UnitPartyCurrent = DefaultUnitPartyCurrent;
                break;
        }
    }

    /// <summary>
    /// パーティリストの作成
    /// </summary>
    void CreatePartyList()
    {
        PacketStructUnit[][] partys = UserDataAdmin.Instance.m_StructPartyAssign;
        m_PartySelectGroup.ClearPartyGroups();

        var partyModels = new List<PartySelectGroupUnitListItemModel>();
        const int AnimationFirstIndex = 0;
        const int AnimationLastIndex = 4;

        for (int i = 0; i < partys.Length; ++i)
        {
            PacketStructUnit[] party = partys[i];
            int index = i;
            var model = new PartySelectGroupUnitListItemModel((uint)index);

            PartySelectGroupUnitContext partyGroup = new PartySelectGroupUnitContext(model);
            partyGroup.Index = index; // 番号の設定
            partyGroup.NameText = string.Format(GameTextUtil.GetText("questlast_tub"), index + 1); // パーティ名
            Array.Copy(party, partyGroup.PartyData, partyGroup.PartyData.Length); // ユニット情報をコピー
            for (int pt_cout = 0; pt_cout < partyGroup.PartyData.Length; ++pt_cout)
            {
                // リンクユニット情報を設定
                if (partyGroup.PartyData[pt_cout] == null) { continue; }
                partyGroup.PartyLinkData[pt_cout] = CharaLinkUtil.GetLinkUnit(partyGroup.PartyData[pt_cout].link_unique_id);
            }

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
                MasterDataParamChara _master = MasterFinder<MasterDataParamChara>.Instance.Find((int)leaderUnit.id);
                partyGroup.IconSelect = MainMenuUtil.GetElementCircleSprite(_master.element);
            }
            partyGroup.IsSelect = (index == m_UnitPartyCurrent);
            m_PartySelectGroup.AddData(partyGroup);
            model.OnClicked += () =>
            {
                OnSelectPartyGroup(partyGroup);
            };

            model.OnShowedNextIcon += () =>
            {
                if (index <= AnimationFirstIndex ||
                    index > AnimationLastIndex)
                {
                    return;
                }

                partyModels[index - 1].ShowIcon();
            };
            model.OnShowedNextName += () =>
            {
                if (index >= AnimationLastIndex)
                {
                    return;
                }

                partyModels[index + 1].ShowName();
            };

            model.OnViewStarted += () =>
            {
                bool showName = index == AnimationFirstIndex
                                || index > AnimationLastIndex;
                bool showIcon = index == AnimationLastIndex
                                || index > AnimationLastIndex;

                if (showName)
                {
                    model.ShowName();
                }
                if (showIcon)
                {
                    model.ShowIcon();
                }
            };

            partyModels.Add(model);
        }
    }

    /// <summary>
    /// パーティ詳細情報の変更
    /// </summary>
    void CreatePartyParam()
    {
        PacketStructUnit[][] partys = UserDataAdmin.Instance.m_StructPartyAssign;
        m_PartyParamPanel.m_CurrentIndex = m_UnitPartyCurrent;
        m_PartyParamPanel.PartyParams.Clear();
        m_PartyParamPanel.OnChangedPartyParamAction = OnChangedPartyParam;
        List<MasterDataParamChara> charaMasterList = MasterFinder<MasterDataParamChara>.Instance.FindAll();
        m_partyPanels.Clear();

        for (int i = 0; i < partys.Length; ++i)
        {
            int index = i;
            var partyPanelModel = new PartyParamListItemModel((uint)index);

            PartyParamListItemContext paertParam = new PartyParamListItemContext(partyPanelModel);
            PartySelectGroupUnitContext party = m_PartySelectGroup.GetParty(i);


            m_partyPanels.Add(partyPanelModel);

            //--------------------------------------------
            // パーティ情報
            //--------------------------------------------
            if (m_PartyParamPanel != null)
            {
                CharaUtil.setupCharaParty(ref paertParam.PartyInfo, party.PartyData); // パーティ情報の設定
                paertParam.NameText = string.Format(GameTextUtil.GetText("questlast_text7"), party.Index + 1);
                paertParam.CostText = string.Format(GameTextUtil.GetText("questlast_text5"), paertParam.PartyInfo.m_PartyTotalCost
                                                                        , UserDataAdmin.Instance.m_StructPlayer.total_party); // Cost
                paertParam.CharmText = string.Format(GameTextUtil.GetText("questlast_text6"), paertParam.PartyInfo.m_PartyTotalCharm); // CHARM
            }

            //-------------------------
            // ユニット設定
            //-------------------------
            List<PartyMemberUnitContext> unitList = new List<PartyMemberUnitContext>();
            int unitDataIndex = 0;
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
                    unit.UnitData = unitData;
                    unitDataModel.OnLongPressed += () =>
                    {
                        OnLongPressParamUnit(unit);
                    };
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

                // indexがm_UnitPartyCurrentとその前後の時にアイコンとステータスを更新する.
                if ((index == m_UnitPartyCurrent) ||
                    (index == m_UnitPartyCurrent - 1) ||
                    (index == m_UnitPartyCurrent + 1))
                {
                    MainMenuUtil.SetPartySelectUnitData(ref unit, unitData, linkData, party.PartyData);
                }
                else if (unit.OutSideCircleImage == null)
                {
                    unit.OutSideCircleImage = ResourceManager.Instance.Load("icon_circle_deco", ResourceType.Common);
                }

                unitDataModel.OnClicked += () =>
                {
                    OnSelectParamUnit(unit);
                };
                unitList.Add(unit);

                partyPanelModel.AddUnit(unitDataModel);
            }

            unitList[0].PartyCharaIndex = GlobalDefine.PartyCharaIndex.LEADER;
            unitList[1].PartyCharaIndex = GlobalDefine.PartyCharaIndex.MOB_1;
            unitList[2].PartyCharaIndex = GlobalDefine.PartyCharaIndex.MOB_2;
            unitList[3].PartyCharaIndex = GlobalDefine.PartyCharaIndex.MOB_3;

            paertParam.Units = unitList;

            //-------------------------
            // スキル
            //-------------------------
            int skillDataIndex = 0;
            if (unitList[0].UnitData.id > 0)
            {
                List<UnitSkillAtPartyContext> skillList = new List<UnitSkillAtPartyContext>();

                var skillDataModel = new ListItemModel((uint)skillDataIndex++);

                UnitSkillAtPartyContext leaderSkill = new UnitSkillAtPartyContext(skillDataModel);
                leaderSkill.setupLeaderSkill(unitList[0].CharaMaster.skill_leader);
                skillList.Add(leaderSkill);

                paertParam.Skills = skillList;

                partyPanelModel.AddSkill(skillDataModel);

#if BUILD_TYPE_DEBUG
                Debug.Log("*************** UnitSkillAtPartyContext *********************");
#endif
            }

            paertParam.SelectLinkAction = OnSelectLink;
            m_PartyParamPanel.PartyParams.Add(paertParam);
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
            foreach (var paertParam in m_PartyParamPanel.PartyParams)
            {
                paertParam.HeroImage = texture;
                paertParam.HeroImage_mask = texture_mask;
            }
        }).Load();
    }

    /// <summary>
    /// API送信：ユニットパーティ編成設定
    /// </summary>
    void SendUnitPartyAssign(Action finishedSendAction = null)
    {
        Action endAction = () =>
        {
            m_UnitPartyCurrent = DefaultUnitPartyCurrent;
        };

        if (UserDataAdmin.Instance.m_StructPlayer.unit_party_current != m_PartyParamPanel.m_CurrentIndex)
        { // 選択パーティが変わっている場合
            PacketStructPartyAssign[] partyAssigns = new PacketStructPartyAssign[UserDataAdmin.Instance.m_StructPartyAssign.Length]; // パーティアサイン情報配列

            // パーティアサイン情報設定
            for (int i = 0; i < UserDataAdmin.Instance.m_StructPartyAssign.Length; ++i)
            {
                PartySelectGroupUnitContext party = m_PartySelectGroup.GetParty(i);
                partyAssigns[i] = new PacketStructPartyAssign();
                if (party != null)
                {
                    partyAssigns[i].unit0_unique_id = party.PartyData[0] != null ? party.PartyData[0].unique_id : 0;
                    partyAssigns[i].unit1_unique_id = party.PartyData[1] != null ? party.PartyData[1].unique_id : 0;
                    partyAssigns[i].unit2_unique_id = party.PartyData[2] != null ? party.PartyData[2].unique_id : 0;
                    partyAssigns[i].unit3_unique_id = party.PartyData[3] != null ? party.PartyData[3].unique_id : 0;
                }
                else
                {
                    var partyAssign = UserDataAdmin.Instance.m_StructPartyAssign;
                    partyAssigns[i].unit0_unique_id = (partyAssign[i][0] != null) ? partyAssign[i][0].unique_id : 0;
                    partyAssigns[i].unit1_unique_id = (partyAssign[i][1] != null) ? partyAssign[i][1].unique_id : 0;
                    partyAssigns[i].unit2_unique_id = (partyAssign[i][2] != null) ? partyAssign[i][2].unique_id : 0;
                    partyAssigns[i].unit3_unique_id = (partyAssign[i][3] != null) ? partyAssign[i][3].unique_id : 0;
                }
            }

            // 送信開始
            ServerDataUtilSend.SendPacketAPI_UnitPartyAssign(partyAssigns, m_PartyParamPanel.m_CurrentIndex)
            .setSuccessAction(_data =>
            {
                UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvUnitPartyAssign>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                UserDataAdmin.Instance.ConvertPartyAssing();
                m_IsSendApi = false;
                endAction();
                if (finishedSendAction != null)
                {
                    finishedSendAction();
                }
            })
            .setErrorAction(_data =>
            {
                m_IsSendApi = false;
                endAction();

                if (finishedSendAction != null)
                {
                    finishedSendAction();
                }
            })
            .SendStart();

            m_IsSendApi = true;
        }
        else
        {
            endAction();

            if (finishedSendAction != null)
            {
                finishedSendAction();
            }
        }
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

        var list = m_PartyParamPanel.PartyParamList[unitPartyIndex].GetComponent<PartyParamListItem>();
        for (int pt_count = 0; pt_count < party.PartyData.Length; ++pt_count)
        {
            var unitData = party.PartyData[pt_count];
            var linkData = party.PartyLinkData[pt_count];

            var unit = list.Context.UnitList[pt_count].GetComponent<PartyParamUnitListItem>().Context;
            MainMenuUtil.SetPartySelectUnitData(ref unit, unitData, linkData, party.PartyData);
        }
    }

    /// <summary>
    /// パーティリストを選択したとき
    /// </summary>
    /// <param name="party"></param>
    void OnSelectPartyGroup(PartySelectGroupUnitContext party)
    {
        m_UnitPartyCurrent = party.Index;
        // 変更されたパーティー番号のユニットのステータスを更新する.
        UpdatePartyUnitStatus(m_UnitPartyCurrent, party);

        if (m_PartyParamPanel != null)
        {
            m_PartyParamPanel.ChangeParam(m_UnitPartyCurrent);
        }
        m_PartySelectGroup.ChangePartyItemSelect(m_UnitPartyCurrent); // パーティ選択状態を変更
    }

    /// <summary>
    /// MemberSettingボタンが押されたとき
    /// </summary>
    void OnSelectMovePartyAssign()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        SendUnitPartyAssign(() =>
        {
            // パーティユニット選択画面に移る
            MainMenuParam.m_PartyAssignPrevPage = MAINMENU_SEQ.SEQ_UNIT_PARTY_SELECT;
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_PARTY_FORM, false, false);
        });
        m_IsRequestedApi = true;
    }

    /// <summary>
    /// パーティのユニットを長押ししたとき
    /// </summary>
    /// <param name="_unit"></param>
    void OnLongPressParamUnit(PartyMemberUnitContext _unit)
    {
        if (_unit.UnitData != null && _unit.UnitData.id > 0 && MainMenuManager.HasInstance)
        {
            m_PartyParamPanel.m_CarouselRotator.SetIndex(m_PartyParamPanel.m_CurrentIndex, true);
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            MainMenuManager.Instance.OpenUnitDetailInfoPlayer(_unit.UnitData);
        }
    }

    /// <summary>
    /// パーティのユニットを押したとき
    /// </summary>
    /// <param name="_unit"></param>
    void OnSelectParamUnit(PartyMemberUnitContext _unit)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        SendUnitPartyAssign(() =>
        {
            // パーティユニット選択画面に移る
            MainMenuParam.m_PartyAssignPrevPage = MAINMENU_SEQ.SEQ_UNIT_PARTY_SELECT;
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_PARTY_FORM, false, false);
        });
        m_IsRequestedApi = true;
    }

    void OnChangedPartyParam(int index)
    {
        m_UnitPartyCurrent = index;

        // 移動先パーティー情報更新
        var partyIndices = new int[]
        {
            m_UnitPartyCurrent,
            m_UnitPartyCurrent - 1,
            m_UnitPartyCurrent + 1
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

        m_PartySelectGroup.ChangePartyItemSelect(m_UnitPartyCurrent, true); // パーティ選択状態を変更
    }

    public void OnSelectLink()
    {
        m_PartyParamPanel.SetUpLinkUnit();
    }

    private void SetUpAppearAnimation()
    {
        // 今のつくりだと前回の状態が残っているので
        m_PartySelectGroup.Hide();

        EffectProcessor.Instance.Register("MainMenuUnitPartySelect", (System.Action finish) =>
        {
            foreach (var partyPanel in m_partyPanels)
            {
                partyPanel.Appear();
            }

            m_PartySelectGroup.Show();
            finish();
        });
    }
}
