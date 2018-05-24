using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;

public class UnitParamPanel : MenuPartsBase
{
    private readonly float ExpWidthMax = 466;
    public enum StatusType
    {
        LV_1,
        LV_MAX,
    }

    //Level
    M4uProperty<bool> isViewLevel = new M4uProperty<bool>();
    public bool IsViewLevel { get { return isViewLevel.Value; } set { isViewLevel.Value = value; } }

    M4uProperty<string> levelLabel = new M4uProperty<string>();
    public string LevelLabel { get { return levelLabel.Value; } set { levelLabel.Value = value; } }

    M4uProperty<string> level = new M4uProperty<string>();
    public string Level { get { return level.Value; } set { level.Value = value; } }

    //HP
    M4uProperty<string> hpLabel = new M4uProperty<string>();
    public string HpLabel { get { return hpLabel.Value; } set { hpLabel.Value = value; } }

    M4uProperty<string> hp = new M4uProperty<string>();
    public string Hp { get { return hp.Value; } set { hp.Value = value; } }

    //ATK
    M4uProperty<string> atkLabel = new M4uProperty<string>();
    public string AtkLabel { get { return atkLabel.Value; } set { atkLabel.Value = value; } }

    M4uProperty<string> atk = new M4uProperty<string>();
    public string Atk { get { return atk.Value; } set { atk.Value = value; } }

    //Cost
    M4uProperty<string> costLabel = new M4uProperty<string>();
    public string CostLabel { get { return costLabel.Value; } set { costLabel.Value = value; } }

    M4uProperty<string> cost = new M4uProperty<string>();
    public string Cost { get { return cost.Value; } set { cost.Value = value; } }

    //Charm
    M4uProperty<string> charmLabel = new M4uProperty<string>();
    public string CharmLabel { get { return charmLabel.Value; } set { charmLabel.Value = value; } }

    M4uProperty<string> charm = new M4uProperty<string>();
    public string Charm { get { return charm.Value; } set { charm.Value = value; } }

    //LO
    M4uProperty<string> loLabel = new M4uProperty<string>();
    public string LoLabel { get { return loLabel.Value; } set { loLabel.Value = value; } }

    M4uProperty<string> lo = new M4uProperty<string>();
    public string Lo { get { return lo.Value; } set { lo.Value = value; } }

    M4uProperty<string> loMax = new M4uProperty<string>();
    public string LoMax { get { return loMax.Value; } set { loMax.Value = value; } }

    //Exp
    M4uProperty<bool> isViewExp = new M4uProperty<bool>();
    public bool IsViewExp { get { return isViewExp.Value; } set { isViewExp.Value = value; } }

    M4uProperty<string> expLabel = new M4uProperty<string>();
    public string ExpLabel { get { return expLabel.Value; } set { expLabel.Value = value; } }

    private M4uProperty<float> nowExpWidth = new M4uProperty<float>();
    public float NowExpWidth { get { return nowExpWidth.Value; } set { nowExpWidth.Value = value; } }

    public float ExpRate { get { return (NowExpWidth / ExpWidthMax); } set { NowExpWidth = ExpWidthMax * value; } }

    private M4uProperty<float> buildupExpWidth = new M4uProperty<float>();
    public float BuildupExpWidth { get { return buildupExpWidth.Value; } set { buildupExpWidth.Value = value; } }

    public float BuildupExpRate { get { return (BuildupExpWidth / ExpWidthMax); } set { BuildupExpWidth = ExpWidthMax * value; } }

    //NextExp
    M4uProperty<string> nextExpLabel = new M4uProperty<string>();
    public string NextExpLabel { get { return nextExpLabel.Value; } set { nextExpLabel.Value = value; } }

    M4uProperty<int> nextExp = new M4uProperty<int>();
    public int NextExp { get { return nextExp.Value; } set { nextExp.Value = value; } }

    //BG
    M4uProperty<bool> isActiveBG = new M4uProperty<bool>();
    public bool IsActiveBG { get { return isActiveBG.Value; } set { isActiveBG.Value = value; } }

    private void Awake()
    {
        LevelLabel = GameTextUtil.GetText("unit_status4");
        HpLabel = GameTextUtil.GetText("unit_status5");
        AtkLabel = GameTextUtil.GetText("unit_status6");
        CostLabel = GameTextUtil.GetText("unit_status8");
        CharmLabel = GameTextUtil.GetText("unit_status9");
        LoLabel = GameTextUtil.GetText("unit_status10");
        ExpLabel = GameTextUtil.GetText("unit_status11");
        NextExpLabel = "NEXT:";
        IsActiveBG = false;
        IsViewLevel = true;
        IsViewExp = false;
        GetComponent<M4uContextRoot>().Context = this;
    }

    public void setupUnit(PacketStructUnit _mainUnit, PacketStructUnit _subUnit, bool bFakeLink = false, int battleHP = 0, int battleHPMax = 0, int battleATK = -1, bool expView = true)
    {
        CharaOnce baseChara = new CharaOnce();
        MasterDataParamChara _masterMain = MasterFinder<MasterDataParamChara>.Instance.Find((int)_mainUnit.id);
        if (_masterMain == null)
        {
            return;
        }

        MasterDataLimitOver _masterMainLO = MasterFinder<MasterDataLimitOver>.Instance.Find((int)_masterMain.limit_over_type);
        if (_masterMainLO == null)
        {
            return;
        }

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
        else if (bFakeLink && _subUnit != null)
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
                (int)0,
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

        if (expView == true)
        {
            //-----------------------
            // 次のレベルまでの経験値を算出
            //-----------------------
            int nNowLevelExp = CharaUtil.GetStatusValue(_masterMain, (int)_mainUnit.level, CharaUtil.VALUE.EXP);
            int nNextLevelExp = CharaUtil.GetStatusValue(_masterMain, (int)_mainUnit.level + 1, CharaUtil.VALUE.EXP);
            int nLevelupExp = nNextLevelExp - nNowLevelExp;
            int nNextEXP = 0;
            float expRatio = 0.0f;
            if (nLevelupExp > 0)
            {
                nNextEXP = nNextLevelExp - (int)_mainUnit.exp;
                expRatio = (float)(nLevelupExp - nNextEXP) / nLevelupExp;
            }
            IsViewExp = true;
            NextExp = nNextEXP;
            ExpRate = expRatio;
        }


        setParam(baseChara, _masterMain, _masterMainLO, _subUnit, battleHP, battleHPMax, battleATK);
    }

    public void setupChara(uint unit_id, StatusType _type, uint limitover_lv = 0, uint plus_pow = 0, uint plus_hp = 0)
    {
        MasterDataParamChara _masterMain = MasterFinder<MasterDataParamChara>.Instance.Find((int)unit_id);
        if (_masterMain == null)
        {
            return;
        }

        MasterDataLimitOver _masterMainLO = MasterFinder<MasterDataLimitOver>.Instance.Find((int)_masterMain.limit_over_type);
        if (_masterMainLO == null)
        {
            return;
        }

        CharaOnce baseChara = new CharaOnce();
        if (_type == StatusType.LV_1)
        {
            baseChara.CharaSetupFromID(
                unit_id,
                (int)1,
                (int)0,
                (int)limitover_lv,
                (int)plus_pow,
                (int)plus_hp,
                0,
                0,
                0,
                0,
                0,
                0
                );
        }
        else
        {
            int _limitBreakLevel = 0;
            if (_masterMain.skill_limitbreak != 0)
            {
                MasterDataSkillLimitBreak _masterLimitBreak = MasterFinder<MasterDataSkillLimitBreak>.Instance.Find((int)_masterMain.skill_limitbreak);
                _limitBreakLevel = _masterLimitBreak.level_max;
            }
            baseChara.CharaSetupFromID(
                unit_id,
                (int)_masterMain.level_max,
                (int)_limitBreakLevel,
                (int)_masterMainLO.limit_over_max,
                (int)GlobalDefine.PLUS_MAX,
                (int)GlobalDefine.PLUS_MAX,
                0,
                0,
                0,
                0,
                0,
                0
                );

        }

        setParam(baseChara, _masterMain, _masterMainLO);
    }

    private void setParam(CharaOnce baseChara, MasterDataParamChara _masterMain, MasterDataLimitOver _masterMainLO, PacketStructUnit _subUnit = null, int battleHP = 0, int battleHPMax = 0, int battleATK = -1)
    {
        /**
		 * メインユニット情報
		 */
        {
            string levelFormat = GameTextUtil.GetText("unit_status17");
            Level = string.Format(levelFormat, baseChara.m_CharaLevel, _masterMain.level_max);

        }
        if (battleHPMax != 0)
        {
            Hp = string.Format(GameTextUtil.GetText("unit_status_battle1"), battleHP, battleHPMax);
        }
        else
        {
            if (baseChara.m_CharaPlusHP != 0)
            {
                Hp = string.Format(GameTextUtil.GetText("unit_status19"), baseChara.m_CharaHP, baseChara.m_CharaPlusHP);
            }
            else
            {
                Hp = string.Format("{0}", baseChara.m_CharaHP);
            }
        }
        if (battleATK >= 0)
        {
            Atk = string.Format(GameTextUtil.GetText("unit_status_battle2"), battleATK);
        }
        else
        {
            if (baseChara.m_CharaPlusPow != 0)
            {
                Atk = string.Format(GameTextUtil.GetText("unit_status19"), baseChara.m_CharaPow, baseChara.m_CharaPlusPow);
            }
            else
            {
                Atk = string.Format("{0}", baseChara.m_CharaPow);
            }
        }
        int _cost = _masterMain.party_cost;
        if (_subUnit != null)
        {
            _cost += CharaLinkUtil.GetLinkUnitCost(_subUnit.id);
        }
        Cost = _cost.ToString();
        Charm = string.Format(GameTextUtil.GetText("kyouka_text1"), baseChara.m_CharaCharm.ToString("F1"));
        Lo = baseChara.m_CharaLimitOver.ToString();
        LoMax = _masterMainLO.limit_over_max.ToString();
    }

    public void reset()
    {
        Level = "";
        Hp = "";
        Atk = "";
        Cost = "";
        Charm = "";
        Lo = "";
        LoMax = "";
        NextExp = 0;
        ExpRate = 0.0f;
        BuildupExpRate = 0.0f;
    }
}
