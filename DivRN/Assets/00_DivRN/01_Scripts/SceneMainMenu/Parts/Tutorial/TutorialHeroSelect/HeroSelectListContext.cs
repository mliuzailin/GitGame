/**
 *  @file   HeroSelectListContext.cs
 *  @brief
 *  @author Developer
 *  @date   2017/04/24
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;

public class HeroSelectListContext : M4uContext
{
    [SerializeField]
    private Animation m_SelectHeroAnim;

    M4uProperty<List<PartyMemberUnitContext>> units = new M4uProperty<List<PartyMemberUnitContext>>(new List<PartyMemberUnitContext>());
    /// <summary>ユニットアイコン</summary>
    public List<PartyMemberUnitContext> Units { get { return units.Value; } set { units.Value = value; } }

    List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> UnitList { get { return unitList; } set { unitList = value; } }

    M4uProperty<string> tutorial_text = new M4uProperty<string>();
    public string Tutorial_text { get { return tutorial_text.Value; } set { tutorial_text.Value = value; } }

    M4uProperty<string> profile_text = new M4uProperty<string>();
    public string Profile_text { get { return profile_text.Value; } set { profile_text.Value = value; } }

    M4uProperty<List<UnitSkillContext>> skillList = new M4uProperty<List<UnitSkillContext>>(new List<UnitSkillContext>());
    public List<UnitSkillContext> SkillList { get { return skillList.Value; } set { skillList.Value = value; } }

    List<GameObject> skillListDatas = new List<GameObject>();
    public List<GameObject> SkillListDatas { get { return skillListDatas; } set { skillListDatas = value; } }

    void OnChangedUnitList()
    {
        // 表示順を逆にする
        for (int i = 0; i < UnitList.Count; ++i)
        {
            UnitList[i].transform.SetAsFirstSibling();
        }
    }

    public void CreatePartyParam(int party_id, MainMenuTutorialHeroSelect heroSelect)
    {
        MasterDataDefaultParty masterParty = MasterFinder<MasterDataDefaultParty>.Instance.Find(party_id);

        //-------------------------
        // ユニット設定
        //-------------------------
        List<PartyMemberUnitContext> unitList = new List<PartyMemberUnitContext>();
        int unitDataIndex = 0;
        for (int n = 0; n < 4; ++n)
        {
            uint unit_id = 0;
            uint unit_level = 0;
            switch (n)
            {
                case 0:
                    unit_id = masterParty.party_chara0_id;
                    unit_level = masterParty.party_chara0_level;
                    break;
                case 1:
                    unit_id = masterParty.party_chara1_id;
                    unit_level = masterParty.party_chara1_level;
                    break;
                case 2:
                    unit_id = masterParty.party_chara2_id;
                    unit_level = masterParty.party_chara2_level;
                    break;
                case 3:
                    unit_id = masterParty.party_chara3_id;
                    unit_level = masterParty.party_chara3_level;
                    break;
                default:
                    break;
            }

            var unitDataModel = new PartyMemberUnitListItemModel((uint)unitDataIndex++);
            PartyMemberUnitContext unit = new PartyMemberUnitContext(unitDataModel);

            unit.IsActiveStatus = true;
            if (unit_id != 0)
            {
                PacketStructUnit unitData = new PacketStructUnit();
                unitData.id = unit_id;
                unitData.level = unit_level;

                if (unitData != null)
                {
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
                    unit.UnitImage = ResourceManager.Instance.Load("icon_empty", ResourceType.Menu);
                }

                unit.UnitData = unitData;
                SetUnitData(ref unit, unitData);

                unitDataModel.OnLongPressed += () =>
                {
                    OnLongPressParamUnit(unit, heroSelect);
                };
            }
            else
            {
                unit.UnitImage = ResourceManager.Instance.Load("icon_empty", ResourceType.Menu);
            }

            unitDataModel.OnShowedNext += () =>
            {
                unitDataModel.ShowStatus();
            };

            unitList.Add(unit);
        }

        unitList[0].PartyCharaIndex = GlobalDefine.PartyCharaIndex.LEADER;
        unitList[1].PartyCharaIndex = GlobalDefine.PartyCharaIndex.MOB_1;
        unitList[2].PartyCharaIndex = GlobalDefine.PartyCharaIndex.MOB_2;
        unitList[3].PartyCharaIndex = GlobalDefine.PartyCharaIndex.MOB_3;

        Units = unitList;
    }

    public void CreateSkillList(MasterDataHero master)
    {
        UnitSkillContext unit = new UnitSkillContext();
        unit.setupHeroSkill((uint)master.default_skill_id, 0, 0, true);
        SkillList.Add(unit);
    }

    public void SetUnitData(ref PartyMemberUnitContext unit, PacketStructUnit unitData)
    {
        if (unit == null) { return; }
        if (unitData == null || unitData.id == 0) { return; }
        MasterDataParamChara charMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)unitData.id);

        if (charMaster == null) { return; }
        if (charMaster.skill_active0 > 0)
        {
            MasterDataSkillActive skill1 = MasterFinder<MasterDataSkillActive>.Instance.Find((int)charMaster.skill_active0);
            if (skill1 != null)
            {
                unit.IsActiveSkill1Empty = false;
                unit.Skill1Cost1 = MainMenuUtil.GetSkillElementIcon(skill1.cost1);
                unit.Skill1Cost2 = MainMenuUtil.GetSkillElementIcon(skill1.cost2);
                unit.Skill1Cost3 = MainMenuUtil.GetSkillElementIcon(skill1.cost3);
                unit.Skill1Cost4 = MainMenuUtil.GetSkillElementIcon(skill1.cost4);
                unit.Skill1Cost5 = MainMenuUtil.GetSkillElementIcon(skill1.cost5);
                unit.Skill1Color = MainMenuUtil.GetSkillElementColor("S1", skill1.skill_element);
            }
        }

        if (charMaster.skill_active1 > 0)
        {
            MasterDataSkillActive skill2 = MasterFinder<MasterDataSkillActive>.Instance.Find((int)charMaster.skill_active1);
            if (skill2 != null)
            {
                unit.IsActiveSkill2Empty = false;
                unit.Skill2Cost1 = MainMenuUtil.GetSkillElementIcon(skill2.cost1);
                unit.Skill2Cost2 = MainMenuUtil.GetSkillElementIcon(skill2.cost2);
                unit.Skill2Cost3 = MainMenuUtil.GetSkillElementIcon(skill2.cost3);
                unit.Skill2Cost4 = MainMenuUtil.GetSkillElementIcon(skill2.cost4);
                unit.Skill2Cost5 = MainMenuUtil.GetSkillElementIcon(skill2.cost5);
                unit.Skill2Color = MainMenuUtil.GetSkillElementColor("S2", skill2.skill_element);
            }
        }

        unit.ParamText = (unitData.level >= charMaster.level_max) ? GameTextUtil.GetText("uniticon_flag1")
                                    : string.Format(GameTextUtil.GetText("uniticon_flag2"), unitData.level); // レベル

        uint plusPoint = unitData.add_hp + unitData.add_pow; // プラス値の計算
        if (plusPoint != 0)
        {
            unit.ParamText += string.Format(GameTextUtil.GetText("uniticon_flag3"), plusPoint);
        }

        unit.LinkIcon = MainMenuUtil.GetLinkMark(unitData, null); // リンクアイコン
        unit.OutSideCircleImage = MainMenuUtil.GetElementCircleSprite(charMaster.element);
        SetUpCharaData(ref unit, unitData, CharaLinkUtil.GetLinkUnit(unitData.link_unique_id), false);
    }

    void SetUpCharaData(ref PartyMemberUnitContext item, PacketStructUnit _mainUnit, PacketStructUnit _subUnit, bool dispCharm)
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

        item.HpText = baseChara.m_CharaHP.ToString();
        item.AtkText = baseChara.m_CharaPow.ToString();
    }

    /// <summary>
    /// パーティのユニットを長押ししたとき
    /// </summary>
    /// <param name="_unit"></param>
    void OnLongPressParamUnit(PartyMemberUnitContext _unit, MainMenuTutorialHeroSelect heroSelect)
    {
        if (heroSelect.isSelect)
        {
            return;
        }

        if (heroSelect.isDecision)
        {
            return;
        }

        if (heroSelect.isfinishDecision)
        {
            return;
        }

        if (_unit.UnitData != null &&
            _unit.UnitData.id > 0 &&
            MainMenuManager.HasInstance)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            MainMenuManager.Instance.OpenUnitDetailInfoPlayerTutorial(_unit.UnitData, false);
        }
    }
}
