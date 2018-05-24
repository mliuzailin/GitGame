/**
 *  @file   PartyMemberStatusPanel.cs
 *  @brief
 *  @author Developer
 *  @date   2017/04/05
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;

public class PartyMemberStatusPanel : MenuPartsBase
{
    public GameObject Content = null;
    public RectTransform SkillListView = null;

    M4uProperty<string> nameText = new M4uProperty<string>();
    public string NameText
    {
        get
        {
            return nameText.Value;
        }
        set
        {
            nameText.Value = value;
        }
    }

    M4uProperty<string> costText = new M4uProperty<string>();
    /// <summary>コスト</summary>
    public string CostText
    {
        get
        {
            return costText.Value;
        }
        set
        {
            costText.Value = value;
        }
    }

    M4uProperty<string> charmText = new M4uProperty<string>();
    /// <summary>チャーム</summary>
    public string CharmText
    {
        get
        {
            return charmText.Value;
        }
        set
        {
            charmText.Value = value;
        }
    }

    M4uProperty<List<PartyMemberStatusListItemContext>> unitStatusParams = new M4uProperty<List<PartyMemberStatusListItemContext>>(new List<PartyMemberStatusListItemContext>());
    public List<PartyMemberStatusListItemContext> UnitStatusParams
    {
        get
        {
            return unitStatusParams.Value;
        }
        set
        {
            unitStatusParams.Value = value;
        }
    }

    M4uProperty<string> partyText = new M4uProperty<string>();
    public string PartyText
    {
        get
        {
            return partyText.Value;
        }
        set
        {
            partyText.Value = value;
        }
    }

    M4uProperty<List<UnitSkillContext>> skills = new M4uProperty<List<UnitSkillContext>>(new List<UnitSkillContext>());
    public List<UnitSkillContext> Skills
    {
        get
        {
            return skills.Value;
        }
        set
        {
            skills.Value = value;
        }
    }

    List<GameObject> unitStatusParamList = new List<GameObject>();
    public List<GameObject> UnitStatusParamList { get { return unitStatusParamList; } set { unitStatusParamList = value; } }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddData(PacketStructUnit unitData, GlobalDefine.PartyCharaIndex partyCharaIndex, PacketStructUnit[] partyUnits)
    {
        PartyMemberStatusListItemContext unitStatus = new PartyMemberStatusListItemContext();
        unitStatus.PartyCharaIndex = partyCharaIndex;
        SetUnitData(ref unitStatus, unitData, partyUnits);
        UnitStatusParams.Add(unitStatus);
    }

    public void ChangeData(PacketStructUnit unitData, GlobalDefine.PartyCharaIndex partyCharaIndex, PacketStructUnit[] partyUnits)
    {
        PartyMemberStatusListItemContext selectUnitStatus = UnitStatusParams.Find(value => value.PartyCharaIndex == partyCharaIndex);
        SetUnitData(ref selectUnitStatus, unitData, partyUnits);
        UpdateOtherMemberStatus(partyCharaIndex, partyUnits);
        SetStatusParam();
    }

    private void UpdateOtherMemberStatus(GlobalDefine.PartyCharaIndex changeCharaIndex, PacketStructUnit[] partyUnits)
    {
        for (var i = (int)GlobalDefine.PartyCharaIndex.LEADER; i < partyUnits.Length; i++)
        {
            var charaIndex = (GlobalDefine.PartyCharaIndex)i;
            if (changeCharaIndex == charaIndex)
            {
                continue;
            }
            var selectUnitStatus = UnitStatusParams.Find(value => value.PartyCharaIndex == charaIndex);
            SetUnitData(ref selectUnitStatus, partyUnits[i], partyUnits);
        }
    }

    public void SetUnitData(ref PartyMemberStatusListItemContext unitStatus, PacketStructUnit unitData, PacketStructUnit[] partyUnits)
    {
        if (unitStatus == null) { return; }

        MasterDataParamChara charMaster = null;
        if (unitData == null || unitData.id == 0)
        {
            unitStatus.HpText = "";
            unitStatus.AtkText = "";
            unitStatus.Cost = 0;
            unitStatus.Charm = 0;
        }
        else
        {
            charMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)unitData.id);
            SetUpCharaData(ref unitStatus, unitData, CharaLinkUtil.GetLinkUnit(unitData.link_unique_id), false, partyUnits);
        }

        MasterDataSkillActive skill1 = null;
        if (charMaster != null && charMaster.skill_active0 > 0)
        {
            skill1 = MasterFinder<MasterDataSkillActive>.Instance.Find((int)charMaster.skill_active0);
        }
        unitStatus.IsActiveSkill1Empty = (skill1 == null);
        unitStatus.Skill1Cost1 = (skill1 != null) ? MainMenuUtil.GetSkillElementIcon(skill1.cost1) : null;
        unitStatus.Skill1Cost2 = (skill1 != null) ? MainMenuUtil.GetSkillElementIcon(skill1.cost2) : null;
        unitStatus.Skill1Cost3 = (skill1 != null) ? MainMenuUtil.GetSkillElementIcon(skill1.cost3) : null;
        unitStatus.Skill1Cost4 = (skill1 != null) ? MainMenuUtil.GetSkillElementIcon(skill1.cost4) : null;
        unitStatus.Skill1Cost5 = (skill1 != null) ? MainMenuUtil.GetSkillElementIcon(skill1.cost5) : null;
        unitStatus.Skill1Color = (skill1 != null) ? MainMenuUtil.GetSkillElementColor("S1", skill1.skill_element) : null;

        MasterDataSkillActive skill2 = null;
        if (charMaster != null && charMaster.skill_active1 > 0)
        {
            skill2 = MasterFinder<MasterDataSkillActive>.Instance.Find((int)charMaster.skill_active1);
        }
        unitStatus.IsActiveSkill2Empty = (skill2 == null);
        unitStatus.Skill2Cost1 = (skill2 != null) ? MainMenuUtil.GetSkillElementIcon(skill2.cost1) : null;
        unitStatus.Skill2Cost2 = (skill2 != null) ? MainMenuUtil.GetSkillElementIcon(skill2.cost2) : null;
        unitStatus.Skill2Cost3 = (skill2 != null) ? MainMenuUtil.GetSkillElementIcon(skill2.cost3) : null;
        unitStatus.Skill2Cost4 = (skill2 != null) ? MainMenuUtil.GetSkillElementIcon(skill2.cost4) : null;
        unitStatus.Skill2Cost5 = (skill2 != null) ? MainMenuUtil.GetSkillElementIcon(skill2.cost5) : null;
        unitStatus.Skill2Color = (skill2 != null) ? MainMenuUtil.GetSkillElementColor("S2", skill2.skill_element) : null;
    }

    void SetUpCharaData(ref PartyMemberStatusListItemContext item, PacketStructUnit _mainUnit, PacketStructUnit _subUnit, bool dispCharm, PacketStructUnit[] partyUnits)
    {
        CharaOnce baseChara = new CharaOnce();

        if (_mainUnit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE &&
            _subUnit != null)
        {
            baseChara.CharaSetupFromID(
                _mainUnit.id,
                (int)_mainUnit.level,
                (int)_mainUnit.limitbreak_lv,
                (int)_mainUnit.limitover_lv,
                (int)_mainUnit.add_pow,
                (int)_mainUnit.add_hp,
                _subUnit.id,
                (int)_subUnit.level,
                (int)_subUnit.add_pow,
                (int)_subUnit.add_hp,
                (int)_mainUnit.link_point,
                (int)_subUnit.limitover_lv
                );
        }
        else
        {
            baseChara.CharaSetupFromID(
                _mainUnit.id,
                (int)_mainUnit.level,
                (int)_mainUnit.limitbreak_lv,
                (int)_mainUnit.limitover_lv,
                (int)_mainUnit.add_pow,
                (int)_mainUnit.add_hp,
                0,
                0,
                0,
                0,
                0,
                0
                );
        }

        var changeHp = (int)(baseChara.m_CharaHP * MainMenuUtil.GetLeaderSkillHpRate(baseChara, partyUnits));
        var statusText = string.Format(GameTextUtil.GetText("questlast_text4"), changeHp);
        if (changeHp > baseChara.m_CharaHP)
        {
            statusText = string.Format(GameTextUtil.GetText("questlast_text8"), changeHp);
        }
        else if (changeHp < baseChara.m_CharaHP)
        {
            statusText = string.Format(GameTextUtil.GetText("questlast_text9"), changeHp);
        }
        item.HpText = statusText;

        var changePow = (int)(baseChara.m_CharaPow * MainMenuUtil.GetLeaderSkillDamageRate(baseChara, MainMenuUtil.GetPartyCharaOnceArray(partyUnits)));
        statusText = string.Format(GameTextUtil.GetText("questlast_text4"), changePow);
        if (changePow > baseChara.m_CharaPow)
        {
            statusText = string.Format(GameTextUtil.GetText("questlast_text8"), changePow);
        }
        else if (changePow < baseChara.m_CharaPow)
        {
            statusText = string.Format(GameTextUtil.GetText("questlast_text9"), changePow);
        }
        item.AtkText = statusText;

        item.Cost = ServerDataUtil.GetPacketUnitCost(_mainUnit) + CharaLinkUtil.GetLinkUnitCost(_mainUnit);
        item.Charm = baseChara.m_CharaCharm;
    }

    public void SetStatusParam()
    {
        int nAssignedCost = 0;
        double nAssignedCharmt = 0;
        for (int i = 0; i < UnitStatusParams.Count; ++i)
        {
            nAssignedCost += UnitStatusParams[i].Cost;
            nAssignedCharmt += UnitStatusParams[i].Charm;
        }

        CostText = string.Format(GameTextUtil.GetText("questlast_text1"), nAssignedCost, UserDataAdmin.Instance.m_StructPlayer.total_party); // Cost
        CharmText = string.Format(GameTextUtil.GetText("questlast_text2"), nAssignedCharmt); // CHARM
    }

    void OnChangedUnitStatusParamList()
    {
        // 表示順を逆にする
        for (int i = 0; i < UnitStatusParamList.Count; ++i)
        {
            UnitStatusParamList[i].transform.SetAsFirstSibling();
        }
    }
}
