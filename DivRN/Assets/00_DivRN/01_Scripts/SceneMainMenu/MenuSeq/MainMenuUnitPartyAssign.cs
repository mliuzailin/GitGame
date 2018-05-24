using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;

public class MainMenuUnitPartyAssign : MainMenuSeq
{
    /// <summary>パーティメンバー一覧</summary>
    PartyMemberUnitGroup m_PartyMemberUnitGroup = null;
    /// <summary>ユニット一覧</summary>
    UnitGridComplex m_UnitGrid = null;
    ExpandWindow m_ExpandWindow = null;
    /// <summary>パーティステータス</summary>
    PartyMemberStatusPanel m_PartyMemberStatusPanel = null;
    UnitPartyAssignButtonPanel m_UnitPartyAssignButtonPanel = null;

    /// <summary>現在選択されているパーティ番号</summary>
    int m_CurrentSelectPartyIndex = 0;
    /// <summary>選択しているユニット</summary>
    GlobalDefine.PartyCharaIndex m_SelectPartyCharaIndex = GlobalDefine.PartyCharaIndex.ERROR;

    /// <summary>パーティメンバーが変更されたかどうか</summary>
    private bool m_IsPartyMemberChange = false;
    /// <summary>変更確認ダイアログを表示したかどうか</summary>
    private bool m_IsShowChangeConfirmation = false;
    private bool m_IsFinishChangeConfirmation = false;

    private bool m_isDisabeleEvent = false;

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

        if (ChkUserDataUpdate())
        {
            CreateUnitGrid();
        }
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        AndroidBackKeyManager.Instance.StackPush(m_CanvasObj.gameObject, OnSelectReturn);

        //--------------------------------
        //	オブジェクトの取得
        //--------------------------------
        m_PartyMemberUnitGroup = m_CanvasObj.GetComponentInChildren<PartyMemberUnitGroup>();
        m_PartyMemberUnitGroup.SetPositionAjustStatusBar(new Vector2(0, -132));
        m_PartyMemberUnitGroup.SetSizeParfect(new Vector2(0, 100));

        m_UnitGrid = m_CanvasObj.GetComponentInChildren<UnitGridComplex>(); //ユニットグリッド取得
        m_UnitGrid.SetPositionAjustStatusBar(new Vector2(0, -15), new Vector2(-48, -355));
        m_UnitGrid.AttchUnitGrid<UnitGridView>(UnitGridView.Create());

        m_ExpandWindow = m_CanvasObj.GetComponentInChildren<ExpandWindow>();
        m_ExpandWindow.SetPositionAjustStatusBar(new Vector2(0, -232));
        m_ExpandWindow.DidSelectButton = SelectWindowButton;

        m_PartyMemberStatusPanel = m_CanvasObj.GetComponentInChildren<PartyMemberStatusPanel>();

        m_UnitPartyAssignButtonPanel = m_CanvasObj.GetComponentInChildren<UnitPartyAssignButtonPanel>();

        m_SelectPartyCharaIndex = GlobalDefine.PartyCharaIndex.ERROR;
        m_IsPartyMemberChange = false;

        //カレントパーティ設定
        m_CurrentSelectPartyIndex = UserDataAdmin.Instance.m_StructPlayer.unit_party_current;

        CreatePartyMemberUnitGroup(); // パーティメンバーの作成
        CreateUnitGrid(); // ユニットリストの作成
        CreatePatyStatusPanel();
        ChangeUnitGridMark();

        if (m_UnitPartyAssignButtonPanel != null)
        {
            m_UnitPartyAssignButtonPanel.ClickExecuteButton = OnClickDecision;
            m_UnitPartyAssignButtonPanel.ClickReturnButton = OnSelectReturn;
        }

        m_ExpandWindow.Close(true);

        if (MainMenuParam.m_PartyAssignPrevPage == MAINMENU_SEQ.SEQ_QUEST_SELECT_PARTY)
        {
            MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.QUEST;
        }
        else
        {
            MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.UNIT;
        }
        m_IsShowChangeConfirmation = false;
        m_IsFinishChangeConfirmation = false;
        CheckUnitSpace();
    }

    public override bool PageSwitchEventDisableBefore()
    {
        if (m_IsShowChangeConfirmation == true && m_IsFinishChangeConfirmation == false)
        {
            return true;
        }

        if (m_IsPartyMemberChange == true && m_IsShowChangeConfirmation == false)
        {
            if (TutorialManager.IsExists)
            {
                return false;
            }

            if (ServerApi.IsExists == true)
            {
                return false;
            }

            ShowChangeConfirmation(() => { });

            return true;
        }

        return base.PageSwitchEventDisableBefore();
    }

    public override bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        if (m_PartyMemberUnitGroup != null)
        {
            m_PartyMemberUnitGroup.Detach();
        }

        return base.PageSwitchEventDisableAfter(eNextMainMenuSeq);
    }

    /// <summary>
    /// パーティメンバー一覧の作成
    /// </summary>
    void CreatePartyMemberUnitGroup()
    {
        PacketStructUnit[][] partys = UserDataAdmin.Instance.m_StructPartyAssign;
        PacketStructUnit[] party = new PacketStructUnit[(int)GlobalDefine.PartyCharaIndex.MAX - 1];
        Array.Copy(partys[m_CurrentSelectPartyIndex], party, party.Length); // ユニット情報をコピー

        List<PartyMemberUnitContext> unitList = new List<PartyMemberUnitContext>();
        foreach (PacketStructUnit unitData in party)
        {
            int index = unitList.Count;
            var model = new PartyMemberUnitListItemModel((uint)index);

            PartyMemberUnitContext unit = new PartyMemberUnitContext(model);
            unit.UnitData = unitData;
            if (unitData != null)
            {
                // ユニット画像
                UnitIconImageProvider.Instance.Get(
                    unitData.id,
                    sprite =>
                    {
                        unit.UnitImage = sprite;
                    },
                    true);
                unit.LinkIcon = MainMenuUtil.GetLinkMark(unitData);
                unit.IsEnalbeSelect = true;
                MasterDataParamChara _master = MasterFinder<MasterDataParamChara>.Instance.Find((int)unitData.id);
                unit.IconSelect = MainMenuUtil.GetElementCircleSprite(_master.element);
                unit.IsElement = true;
            }
            else
            {
                unit.UnitImage = ResourceManager.Instance.Load("icon_empty2");
                unit.IsEnalbeSelect = false;
                unit.IsElement = false;
            }

            SetupLinkChara(unit);

            unit.DidSelectItem = OnSelectPartyMemberUnit;
            unit.DidLongPressItem = OnSelectUnitLongPress;
            unitList.Add(unit);
        }

        unitList[0].PartyCharaIndex = GlobalDefine.PartyCharaIndex.LEADER;
        unitList[1].PartyCharaIndex = GlobalDefine.PartyCharaIndex.MOB_1;
        unitList[2].PartyCharaIndex = GlobalDefine.PartyCharaIndex.MOB_2;
        unitList[3].PartyCharaIndex = GlobalDefine.PartyCharaIndex.MOB_3;

        m_PartyMemberUnitGroup.Units = unitList;
        m_PartyMemberUnitGroup.OnClickReleaseAction = OnUnitRelease;
        m_PartyMemberUnitGroup.OnClickDetailAction = OnDetailWindow;
        m_PartyMemberUnitGroup.OnClickLinkAction = OnSelectLink;

        CharaUtil.setupCharaParty(ref m_PartyMemberUnitGroup.PartyInfo, party); // パーティ情報の設定

        //DG0-4124 対応
        if (TutorialManager.HasInstance)
        {
            UnitIconImageProvider.Instance.Tick();
        }
    }

    /// <summary>
    /// リンクキャラの作成
    /// </summary>
    /// <param name="unit"></param>
    void SetupLinkChara(PartyMemberUnitContext unit)
    {
        if (unit != null && unit.UnitData != null)
        {
            var linkData = CharaLinkUtil.GetLinkUnit(unit.UnitData.link_unique_id);
            List<MasterDataParamChara> charaMasterList = MasterFinder<MasterDataParamChara>.Instance.FindAll();

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

                if (unit.LinkCharaMaster != null)
                {
                    unit.LinkOutSideCircleImage = MainMenuUtil.GetElementCircleSprite(unit.LinkCharaMaster.element);
                }
                return;
            }
        }
        unit.LinkOutSideCircleImage = ResourceManager.Instance.Load("icon_circle_deco", ResourceType.Common);
        unit.LinkUnitImage = ResourceManager.Instance.Load("icon_empty2", ResourceType.Menu);
        unit.IsEmptyLinkUnit = true;
    }

    /// <summary>
    /// 詳細領域
    /// </summary>
    void CreatePatyStatusPanel()
    {
        if (m_PartyMemberStatusPanel == null || m_PartyMemberUnitGroup == null)
        {
            return;
        }

        // リーダースキルのHP、攻撃力計算のためにユニット情報配列を作成しておく.
        var partyUnits = new PacketStructUnit[m_PartyMemberUnitGroup.Units.Count];
        for (var i = 0; i < partyUnits.Length; i++)
        {
            partyUnits[i] = m_PartyMemberUnitGroup.Units[i].UnitData;
        }

        m_PartyMemberStatusPanel.UnitStatusParams.Clear();
        for (int i = 0; i < m_PartyMemberUnitGroup.Units.Count; ++i)
        {
            m_PartyMemberStatusPanel.AddData(m_PartyMemberUnitGroup.Units[i].UnitData, m_PartyMemberUnitGroup.Units[i].PartyCharaIndex, partyUnits);
        }

        m_PartyMemberStatusPanel.Skills.Clear();
        UnitSkillContext skill = new UnitSkillContext();
        MasterDataParamChara charMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)m_PartyMemberUnitGroup.Units[0].UnitData.id);
        skill.setupLeaderSkill(charMaster.skill_leader);
        m_PartyMemberStatusPanel.Skills.Add(skill);
        m_PartyMemberStatusPanel.NameText = string.Format(GameTextUtil.GetText("questlast_text3"), m_CurrentSelectPartyIndex + 1); // パーティ名
        m_PartyMemberStatusPanel.SetStatusParam();

        if (m_ExpandWindow != null)
        {
            m_ExpandWindow.ViewHeightSize = m_PartyMemberStatusPanel.GetComponent<RectTransform>().rect.height;

            m_PartyMemberStatusPanel.SetParent(m_ExpandWindow.Content);
        }
        StartCoroutine(WaitSkillList());
    }

    IEnumerator WaitSkillList()
    {
        while (m_PartyMemberStatusPanel.SkillListView.sizeDelta.y == 0)
        {
            yield return null;
        }
        UnityUtil.SetObjectEnabledOnce(m_PartyMemberStatusPanel.Content, false);
        yield return null;
        UnityUtil.SetObjectEnabledOnce(m_PartyMemberStatusPanel.Content, true);
    }

    /// <summary>
    /// ユニットリストの作成
    /// </summary>
    void CreateUnitGrid()
    {
        //ユニット情報構築
        PacketStructUnit[] party = new PacketStructUnit[m_PartyMemberUnitGroup.Units.Count];
        for (int i = 0; i < m_PartyMemberUnitGroup.Units.Count; ++i)
        {
            party[i] = m_PartyMemberUnitGroup.Units[i].UnitData;
        }

        List<UnitGridContext> unitList = MainMenuUtil.MakeUnitGridContextList(party);
        if (unitList == null)
        {
            Debug.LogError("unitlist is null");
            return;
        }

        m_UnitGrid.OnClickSortButtonAction = OnClockSortButton;
        m_UnitGrid.ClickUnitAction = OnSelectUnit;
        m_UnitGrid.LongPressUnitAction = OnSelectUnitLongPress;

        m_UnitGrid.SetUpSortData(LocalSaveManager.Instance.LoadFuncSortFilterPartyForm());
        m_UnitGrid.CreateList(unitList);

        m_UnitGrid.SetupUnitSelected = SetupUintSelected;
    }

    void ChangeStatus(PacketStructUnit unitData, GlobalDefine.PartyCharaIndex partyCharaIndex, PacketStructUnit[] partyUnits)
    {
        //-------------------------------------------------
        // ステータスの変更
        //-------------------------------------------------
        if (m_PartyMemberStatusPanel != null)
        {
            m_PartyMemberStatusPanel.ChangeData(unitData, partyCharaIndex, partyUnits);
            //-------------------------------------------------
            // スキルの変更
            //-------------------------------------------------
            if (unitData != null && m_SelectPartyCharaIndex == GlobalDefine.PartyCharaIndex.LEADER)
            {
                m_PartyMemberStatusPanel.Skills.Clear();
                UnitSkillContext skill = new UnitSkillContext();
                MasterDataParamChara charMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)unitData.id);
                skill.setupLeaderSkill(charMaster.skill_leader);
                m_PartyMemberStatusPanel.Skills.Add(skill);
            }
        }
    }

    /// <summary>
    /// ユニットの選択状態を変更する
    /// </summary>
    void ChangeMemberUnitSelect(PartyMemberUnitContext unit)
    {
        m_PartyMemberUnitGroup.ChangeUnitParam(unit);
        m_SelectPartyCharaIndex = (unit != null) ? unit.PartyCharaIndex : GlobalDefine.PartyCharaIndex.ERROR;
        ChangeUnitGridMark();
    }

    /// <summary>
    /// ユニット一覧の状態を変更
    /// </summary>
    public void ChangeUnitGridMark()
    {
        for (int i = 0; i < m_UnitGrid.Units.Count; ++i)
        {
            UnitGridContext unit = m_UnitGrid.Units[i];
            SetupUintSelected(unit);
        }
        m_UnitGrid.UpdateList();
    }

    void SetupUintSelected(UnitGridContext _unit)
    {
        bool isMemberFull = true;

        // 選択関係を初期化
        _unit.IsActiveLeader = false;
        _unit.IsActivePartyAssign = false;
        _unit.IsSelectedUnit = false;
        _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.NONE;

        // パーティ内にユニットがあるか調べる
        foreach (PartyMemberUnitContext partyUnit in m_PartyMemberUnitGroup.Units)
        {
            if (partyUnit.UnitData == null)
            {
                if (isMemberFull)
                {
                    isMemberFull = false;
                }
                continue;
            }

            if (partyUnit.UnitData.unique_id == _unit.UnitData.unique_id)
            {
                _unit.IsActivePartyAssign = true;
                _unit.IsSelectedUnit = true;
                if (partyUnit.PartyCharaIndex == GlobalDefine.PartyCharaIndex.LEADER)
                {
                    _unit.IsActiveLeader = true;
                }
            }
        }

        // リーダー選択中以外でメンバーがいっぱいでパーティに入っていない場合
        if (m_SelectPartyCharaIndex != GlobalDefine.PartyCharaIndex.LEADER && isMemberFull && _unit.IsActivePartyAssign == false)
        {
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.GRAY_OUT_ENABLE_BUTTON;
        }
        // リンクユニットの場合
        if (_unit.UnitData.link_info == (int)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK)
        {
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.GRAY_OUT_ENABLE_BUTTON;
        }
    }

    /// <summary>
    /// パーティメンバー一覧から作成したユニット情報配列を返す.
    /// </summary>
    /// <returns>ユニット情報配列</returns>
    private PacketStructUnit[] GetPartyPacketStructUnits()
    {
        var partyUnits = new PacketStructUnit[m_PartyMemberUnitGroup.Units.Count];
        for (var i = 0; i < partyUnits.Length; i++)
        {
            if (m_PartyMemberUnitGroup.Units[i] == null)
            {
                continue;
            }
            partyUnits[i] = m_PartyMemberUnitGroup.Units[i].UnitData;
        }

        return partyUnits;
    }

    /// <summary>
    /// パーティユニットを変更する
    /// </summary>
    /// <param name="unit_unique_id"></param>
    public void ChangePartyUnit(long unit_unique_id)
    {
        PacketStructUnit slectUnitData = UserDataAdmin.Instance.SearchChara(unit_unique_id); // 選択したユニット
        PartyMemberUnitContext selectUnit;
        if (m_SelectPartyCharaIndex == GlobalDefine.PartyCharaIndex.ERROR)
        {
            // 選択されていない場合は空いてるパーティを選ぶ
            selectUnit = m_PartyMemberUnitGroup.Units.Find(value => value.UnitData == null);
        }
        else
        {
            selectUnit = m_PartyMemberUnitGroup.Units.Find(value => value.PartyCharaIndex == m_SelectPartyCharaIndex);
        }

        if (slectUnitData == null)
        {
            return;
        }
        if (selectUnit == null)
        {
            return;
        }

        GlobalDefine.PartyCharaIndex charaIndex = CheckPartyInUnit(unit_unique_id);
        if (charaIndex != GlobalDefine.PartyCharaIndex.ERROR)
        {
            //パーティ内で入れ替える
            if (charaIndex == GlobalDefine.PartyCharaIndex.LEADER && selectUnit.UnitData == null)
            {
                //パーティ選択枠が空で入れ替えのキャラがリーダーの場合はやめる
                return;
            }
            PartyMemberUnitContext partyUnit = m_PartyMemberUnitGroup.Units.Find(value => value.PartyCharaIndex == charaIndex);
            PacketStructUnit tmpUnitData = selectUnit.UnitData;

            selectUnit.UnitData = slectUnitData;
            partyUnit.UnitData = tmpUnitData;

            // 画像変更
            if (partyUnit.UnitData != null)
            {
                UnitIconImageProvider.Instance.Get(
                    partyUnit.UnitData.id,
                    sprite =>
                    {
                        partyUnit.UnitImage = sprite;
                    },
                    true);
                partyUnit.LinkIcon = MainMenuUtil.GetLinkMark(partyUnit.UnitData);
                partyUnit.IsEnalbeSelect = true;
                MasterDataParamChara _master = MasterFinder<MasterDataParamChara>.Instance.Find((int)partyUnit.UnitData.id);
                partyUnit.IconSelect = MainMenuUtil.GetElementCircleSprite(_master.element);
                partyUnit.IsElement = true;
            }
            else
            {
                partyUnit.UnitImage = ResourceManager.Instance.Load("icon_empty2");
                partyUnit.LinkIcon = null;
                partyUnit.IsEnalbeSelect = false;
                partyUnit.IsElement = false;
            }

            UnitIconImageProvider.Instance.Get(
                selectUnit.UnitData.id,
                sprite =>
                {
                    selectUnit.UnitImage = sprite;
                },
                true);
            selectUnit.LinkIcon = MainMenuUtil.GetLinkMark(selectUnit.UnitData);
            selectUnit.IsEnalbeSelect = true;
            MasterDataParamChara _selectMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)selectUnit.UnitData.id);
            selectUnit.IconSelect = MainMenuUtil.GetElementCircleSprite(_selectMaster.element);
            selectUnit.IsElement = true;

            SetupLinkChara(selectUnit);
            SetupLinkChara(partyUnit);

            ChangeStatus(partyUnit.UnitData, partyUnit.PartyCharaIndex, GetPartyPacketStructUnits());
        }
        else
        {
            if (selectUnit.UnitData != null)
            {
                UnitIconImageProvider.Instance.Reset(selectUnit.UnitData.id);
            }

            //入れ替え
            selectUnit.UnitData = slectUnitData;
            UnitIconImageProvider.Instance.Get(
                selectUnit.UnitData.id,
                sprite =>
                {
                    selectUnit.UnitImage = sprite;
                },
                true);
            selectUnit.LinkIcon = MainMenuUtil.GetLinkMark(selectUnit.UnitData);
            selectUnit.IsEnalbeSelect = true;
            MasterDataParamChara _selectMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)selectUnit.UnitData.id);
            selectUnit.IconSelect = MainMenuUtil.GetElementCircleSprite(_selectMaster.element);
            selectUnit.IsElement = true;

            SetupLinkChara(selectUnit);
        }

        ChangeStatus(slectUnitData, selectUnit.PartyCharaIndex, GetPartyPacketStructUnits());
        m_IsPartyMemberChange = true;
        CheckUnitSpace();
    }

    /// <summary>
    /// パーティユニットをリジェクト
    /// </summary>
    /// <param name="selectUnit"></param>
    /// <param name="isChangeUnitGridMark">trueにするとアイコンの状態が更新されない</param>
    public void RejectPartyUnit(PartyMemberUnitContext selectUnit, bool isChangeUnitGridMark = true)
    {
        if (selectUnit == null)
        {
            return;
        }

        if (selectUnit.UnitData == null)
        {
            return;
        }


        UnitIconImageProvider.Instance.Reset(selectUnit.UnitData.id);

        selectUnit.UnitData = null;
        selectUnit.UnitImage = ResourceManager.Instance.Load("icon_empty2");
        selectUnit.LinkIcon = null;
        selectUnit.IsEnalbeSelect = false;
        selectUnit.IsElement = false;

        selectUnit.LinkOutSideCircleImage = ResourceManager.Instance.Load("icon_circle_deco", ResourceType.Common);
        selectUnit.LinkUnitImage = ResourceManager.Instance.Load("icon_empty2", ResourceType.Menu);
        selectUnit.IsEmptyLinkUnit = true;

        m_IsPartyMemberChange = true;

        SetupLinkChara(selectUnit);

        ChangeStatus(selectUnit.UnitData, selectUnit.PartyCharaIndex, GetPartyPacketStructUnits());
        if (isChangeUnitGridMark)
        {
            ChangeMemberUnitSelect(null);
        }
    }

    /// <summary>
    /// 選択したユニットがパーティ内に入っているか調べる
    /// </summary>
    /// <param name="unit_unique_id"></param>
    /// <returns></returns>
    public GlobalDefine.PartyCharaIndex CheckPartyInUnit(long unit_unique_id)
    {
        foreach (PartyMemberUnitContext unit in m_PartyMemberUnitGroup.Units)
        {
            if (unit.UnitData == null)
            {
                continue;
            }
            if (unit.UnitData.unique_id == unit_unique_id)
            { // パーティにいる場合
                return unit.PartyCharaIndex;
            }
        }

        return GlobalDefine.PartyCharaIndex.ERROR;
    }

    /// <summary>
    /// パーティー変更を確定せずに遷移したときの表示
    /// </summary>
    /// <param name="finishAction"></param>
    void ShowChangeConfirmation(Action finishAction = null)
    {
        m_IsShowChangeConfirmation = true;

        if (CheckCostOver() == false)
        {
            // 確認ダイアログ
            Dialog newDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "warning_organization_title");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "warning_organization_01");
            newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
            newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
            newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
            {
                SendUnitPartyAssign((success) =>
                {
                    m_IsFinishChangeConfirmation = true;

                    if (finishAction != null)
                    {
                        finishAction();
                    }
                });
            });
            newDialog.SetDialogEvent(DialogButtonEventType.NO, () =>
            {
                m_IsFinishChangeConfirmation = true;

                if (finishAction != null)
                {
                    finishAction();
                }
            });
            newDialog.Show();
        }
        else
        {
            // コストオーバーダイアログ
            Dialog newDialogCost = Dialog.Create(DialogType.DialogOK);
            newDialogCost.SetDialogTextFromTextkey(DialogTextType.Title, "un73q_title");
            newDialogCost.SetDialogTextFromTextkey(DialogTextType.MainText, "partycost_error_01");
            newDialogCost.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialogCost.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
            {
                m_IsFinishChangeConfirmation = true;

                if (finishAction != null)
                {
                    finishAction();
                }
            }));
            newDialogCost.Show();
            SoundUtil.PlaySE(SEID.SE_MENU_NG);
        }
    }

    #region API通信
    /// <summary>
    /// API送信：ユニットパーティ編成設定
    /// </summary>
    void SendUnitPartyAssign(Action<bool> finishAction = null)
    {
        PacketStructUnit leader = m_PartyMemberUnitGroup.Units.Find(value => value.PartyCharaIndex == GlobalDefine.PartyCharaIndex.LEADER).UnitData;
        PacketStructUnit mob_1 = m_PartyMemberUnitGroup.Units.Find(value => value.PartyCharaIndex == GlobalDefine.PartyCharaIndex.MOB_1).UnitData;
        PacketStructUnit mob_2 = m_PartyMemberUnitGroup.Units.Find(value => value.PartyCharaIndex == GlobalDefine.PartyCharaIndex.MOB_2).UnitData;
        PacketStructUnit mob_3 = m_PartyMemberUnitGroup.Units.Find(value => value.PartyCharaIndex == GlobalDefine.PartyCharaIndex.MOB_3).UnitData;

        PacketStructUnit[][] partys = UserDataAdmin.Instance.m_StructPartyAssign;

        PacketStructPartyAssign[] partyAssigns = new PacketStructPartyAssign[partys.Length]; // パーティアサイン情報配列

        for (int i = 0; i < partys.Length; ++i)
        {
            partyAssigns[i] = new PacketStructPartyAssign();
            if (i == m_CurrentSelectPartyIndex)
            {
                partyAssigns[i].unit0_unique_id = leader != null ? leader.unique_id : 0;
                partyAssigns[i].unit1_unique_id = mob_1 != null ? mob_1.unique_id : 0;
                partyAssigns[i].unit2_unique_id = mob_2 != null ? mob_2.unique_id : 0;
                partyAssigns[i].unit3_unique_id = mob_3 != null ? mob_3.unique_id : 0;
            }
            else
            {
                partyAssigns[i].unit0_unique_id = partys[i][0] != null ? partys[i][0].unique_id : 0;
                partyAssigns[i].unit1_unique_id = partys[i][1] != null ? partys[i][1].unique_id : 0;
                partyAssigns[i].unit2_unique_id = partys[i][2] != null ? partys[i][2].unique_id : 0;
                partyAssigns[i].unit3_unique_id = partys[i][3] != null ? partys[i][3].unique_id : 0;
            }
        }

        ServerDataUtilSend.SendPacketAPI_UnitPartyAssign(partyAssigns, m_CurrentSelectPartyIndex)
        .setSuccessAction(_data =>
        {
            UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvUnitPartyAssign>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
            UserDataAdmin.Instance.ConvertPartyAssing();
            m_IsPartyMemberChange = false;

            // ダイアログの表示
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "un72q_title");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "un72q_content");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
            {
                if (finishAction != null)
                {
                    finishAction(true);
                }
            }));
            newDialog.Show();

        })
        .setErrorAction(_data =>
        {
            m_IsPartyMemberChange = false;

            if (finishAction != null)
            {
                finishAction(false);
            }
        })
        .SendStart();
    }
    #endregion

    #region Button OnClick
    /// <summary>
    /// パーティメンバーを選択したとき
    /// </summary>
    /// <param name="unit"></param>
    void OnSelectPartyMemberUnit(PartyMemberUnitContext unit)
    {

        //
        if (TutorialManager.IsExists)
        {
            return;
        }

        if (unit.PartyCharaIndex != GlobalDefine.PartyCharaIndex.LEADER)
        {
            if (unit.UnitData != null)
            {
                SoundUtil.PlaySE(SEID.SE_MENU_RET);
            }
            RejectPartyUnit(unit);
            ChangeMemberUnitSelect(unit);
        }
        else
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK);
            ChangeMemberUnitSelect(unit);
        }
    }

    /// <summary>
    /// パーティのユニットを長押ししたとき
    /// </summary>
    /// <param name="_unit"></param>
    public void OnSelectUnitLongPress(PartyMemberUnitContext _unit)
    {
        //
        if (TutorialManager.IsExists)
        {
            return;
        }

        //ユニット詳細画面へ
        if (_unit.UnitData != null && _unit.UnitData.id > 0 && MainMenuManager.HasInstance)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);

            UnitGridContext unitContext = m_UnitGrid.GridView.GetItem(_unit.UnitData.unique_id);
            if (unitContext != null)
            {
                openUnitDetailInfo(unitContext);
            }
        }
    }

    /// <summary>
    /// ユニット一覧のユニットを長押ししたとき
    /// </summary>
    /// <param name="_unit"></param>
    public void OnSelectUnitLongPress(UnitGridContext _unit)
    {
        //
        if (TutorialManager.IsExists)
        {
            return;
        }

        //ユニット詳細画面へ
        if (_unit.UnitData != null && _unit.UnitData.id > 0 && MainMenuManager.HasInstance)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);

            openUnitDetailInfo(_unit);
        }
    }

    /// <summary>
    /// ユニット詳細を開く
    /// </summary>
    /// <param name="_unit"></param>
    private void openUnitDetailInfo(UnitGridContext _unit)
    {
        UnitDetailInfo _info = MainMenuManager.Instance.OpenUnitDetailInfo();
        if (_info == null) return;
        m_PartyMemberUnitGroup.SetActive(false);
        PacketStructUnit _subUnit = UserDataAdmin.Instance.SearchLinkUnit(_unit.UnitData);
        _info.SetUnitFavorite(_unit.UnitData, _subUnit, _unit);
        _info.SetCloseAction(() =>
        {
            //更新データ反映
            m_UnitGrid.UpdateBaseItem(_unit);
            m_PartyMemberUnitGroup.SetActive(true);
        });
    }

    /// <summary>
    /// ユニット一覧のユニットを選択したとき
    /// </summary>
    /// <param name="_unit"></param>
    public void OnSelectUnit(UnitGridContext _unit)
    {
        //
        if (TutorialManager.IsExists)
        {
            return;
        }

        if (_unit.UnitIconType == MasterDataDefineLabel.UnitIconType.GRAY_OUT_ENABLE_BUTTON)
        {
            return;
        }

        if (_unit.IsActivePartyAssign)
        {
            if (_unit.IsActiveLeader == false
            && m_SelectPartyCharaIndex != GlobalDefine.PartyCharaIndex.LEADER)
            {
                // パーティに入っている場合リジェクト
                SoundUtil.PlaySE(SEID.SE_MENU_RET);

                foreach (PartyMemberUnitContext unit in m_PartyMemberUnitGroup.Units)
                {
                    if (unit.UnitData == null)
                    {
                        continue;
                    }
                    if (unit.UnitData.unique_id == _unit.UnitData.unique_id)
                    {
                        RejectPartyUnit(unit);
                    }
                }
                CheckUnitSpace();
            }
            else if (m_SelectPartyCharaIndex != GlobalDefine.PartyCharaIndex.ERROR)
            {
                SoundUtil.PlaySE(SEID.SE_MENU_OK);

                ChangePartyUnit(_unit.UnitData.unique_id);
            }
        }
        else if (_unit.IsActiveLeader == false)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK);

            ChangePartyUnit(_unit.UnitData.unique_id);
        }
    }

    /// <summary>
    /// パーティの全解除
    /// </summary>
    void OnUnitRelease()
    {
        //
        if (TutorialManager.IsExists)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        if (m_PartyMemberUnitGroup == null) { return; }
        for (int i = 0; i < m_PartyMemberUnitGroup.Units.Count; ++i)
        {
            PartyMemberUnitContext unit = m_PartyMemberUnitGroup.Units[i];
            if (unit.PartyCharaIndex == GlobalDefine.PartyCharaIndex.LEADER) { continue; }
            RejectPartyUnit(unit, false);
        }
        CheckUnitSpace();
    }

    /// <summary>
    /// アサインするとパーティコストを超えるかチェック
    /// </summary>
    /// <returns>true:コストオーバー false:コスト内</returns>
    bool CheckCostOver()
    {
        int nAssignedCost = 0;

        for (int i = 0; i < m_PartyMemberUnitGroup.Units.Count; ++i)
        {
            nAssignedCost += ServerDataUtil.GetPacketUnitCost(m_PartyMemberUnitGroup.Units[i].UnitData);
            // リンクユニット分のコストを加算
            nAssignedCost += CharaLinkUtil.GetLinkUnitCost(m_PartyMemberUnitGroup.Units[i].UnitData);
        }

        bool bAssignedCostOver = nAssignedCost > UserDataAdmin.Instance.m_StructPlayer.total_party;

        return bAssignedCostOver;
    }

    void ReturnPage()
    {
        MainMenuParam.m_PartySelectShowedLinkUnit = MainMenuParam.m_PartySelectIsShowLinkUnit;
        AndroidBackKeyManager.Instance.StackPop(m_CanvasObj.gameObject);
        MainMenuManager.Instance.AddSwitchRequest(MainMenuParam.m_PartyAssignPrevPage, false, true);
    }

    /// <summary>
    /// ソートダイアログを開く
    /// </summary>
    void OnClockSortButton()
    {
        //
        if (TutorialManager.IsExists)
        {
            return;
        }

        if (SortDialog.IsExists == true)
        {
            return;
        }

        SortDialog dialog = SortDialog.Create();
        dialog.SetDialogType(SortDialog.DIALOG_TYPE.UNIT);
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterPartyForm());
        dialog.OnCloseThreadAction = OnClickSortThread;
        dialog.OnCloseAction = OnClickSortCloseButton;
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    /// <summary>
    /// ソートダイアログを閉じたとき
    /// </summary>
    void OnClickSortCloseButton(LocalSaveSortInfo sortInfo)
    {
        //--------------------------------
        // データ保存
        //--------------------------------
        LocalSaveManager.Instance.SaveFuncSortFilterPartyForm(sortInfo);

        m_UnitGrid.ExecSortBuild(sortInfo);
    }

    void OnClickSortThread(LocalSaveSortInfo sortInfo)
    {
        m_UnitGrid.ExecSortOnly(sortInfo);
    }

    /// <summary>
    /// 決定ボタンが押されたとき
    /// </summary>
    void OnClickDecision()
    {
        //
        if (TutorialManager.IsExists)
        {
            return;
        }

        if (ServerApi.IsExists == true)
        {
            return;
        }

        if (CheckCostOver() == false)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            SendUnitPartyAssign((success) =>
            {
                ReturnPage();
            });
        }
        else
        {
            // コストオーバーダイアログ
            Dialog newDialogCost = Dialog.Create(DialogType.DialogOK);
            newDialogCost.SetDialogTextFromTextkey(DialogTextType.Title, "un73q_title");
            newDialogCost.SetDialogTextFromTextkey(DialogTextType.MainText, "un73q_content");
            newDialogCost.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialogCost.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() => { }));
            newDialogCost.Show();
            SoundUtil.PlaySE(SEID.SE_MENU_NG);
        }
    }

    void OnSelectReturn()
    {
        //チュートリアル中は反応しない
        if (TutorialManager.IsExists)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        ReturnPage();
    }

    #endregion

    /// <summary>
    ///
    /// </summary>
    /// <param name="bBack"></param>
    /// <returns></returns>
    public override bool PageSwitchEventEnableBefore(bool bBack = false)
    {
        //ユニットパラメータが作成されるまで待つ
        if (UserDataAdmin.Instance.m_bThreadUnitParam)
        {
            return true;
        }
        return false;
    }

    void OnDetailWindow()
    {
        if (m_ExpandWindow == null)
        {
            return;
        }
        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        if (m_ExpandWindow.isOpen == false)
        {
            m_ExpandWindow.SetBackKey(true);
            m_ExpandWindow.Open();
            m_UnitGrid.changeGridWindowSize(true, m_ExpandWindow.ViewHeightSize);
        }
        else
        {
            m_ExpandWindow.SetBackKey(false);
            m_ExpandWindow.Close();
            m_UnitGrid.changeGridWindowSize(false, m_ExpandWindow.ViewHeightSize);
        }
    }

    void SelectWindowButton()
    {
        if (m_ExpandWindow == null)
        {
            return;
        }

        if (m_ExpandWindow.isOpen == true)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
            m_ExpandWindow.Close();
            m_UnitGrid.changeGridWindowSize(false, m_ExpandWindow.ViewHeightSize);
        }
    }

    void CheckUnitSpace()
    {
        foreach (PartyMemberUnitContext unit in m_PartyMemberUnitGroup.Units)
        {
            if (unit.UnitData == null)
            {
                ChangeMemberUnitSelect(unit);
                return;
            }
        }
        ChangeMemberUnitSelect(null);
    }

    void OnSelectLink()
    {
        //ヘッダのタッチ判定の制御
        MainMenuManager.Instance.Header.SetTouchActive(!MainMenuParam.m_PartySelectIsShowLinkUnit);
    }
}
