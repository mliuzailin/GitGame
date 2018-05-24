using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;

public class UnitStatusPanel : MenuPartsBase
{
    public Toggle togglePremium = null;
    public RectTransform expBar = null;

    public Action<bool> DidSelectPremium = delegate { };

    M4uProperty<string> charaName = new M4uProperty<string>();
    public string CharaName { get { return charaName.Value; } set { charaName.Value = value; } }

    M4uProperty<string> charaNo = new M4uProperty<string>();
    public string CharaNo { get { return charaNo.Value; } set { charaNo.Value = value; } }

    M4uProperty<uint> rarity = new M4uProperty<uint>();
    public uint Rarity { get { return rarity.Value; } set { rarity.Value = value; } }

    M4uProperty<string> raceLabel = new M4uProperty<string>();
    public string RaceLabel { get { return raceLabel.Value; } set { raceLabel.Value = value; } }
    //M4uProperty<string> race = new M4uProperty<string>();
    //public string Race { get { return race.Value; } set { race.Value = value; } }

    M4uProperty<Sprite> raceImage = new M4uProperty<Sprite>();
    public Sprite RaceImage
    {
        get { return raceImage.Value; }
        set
        {
            raceImage.Value = value;
            IsActiveRace = (value != null);
        }
    }

    M4uProperty<bool> isActiveRace = new M4uProperty<bool>(false);
    bool IsActiveRace { get { return isActiveRace.Value; } set { isActiveRace.Value = value; } }

    M4uProperty<Sprite> subRaceImage = new M4uProperty<Sprite>();
    public Sprite SubRaceImage
    {
        get { return subRaceImage.Value; }
        set
        {
            subRaceImage.Value = value;
            IsActiveSubRace = (value != null);
        }
    }

    M4uProperty<bool> isActiveSubRace = new M4uProperty<bool>(false);
    bool IsActiveSubRace { get { return isActiveSubRace.Value; } set { isActiveSubRace.Value = value; } }

    M4uProperty<string> attributeLabel = new M4uProperty<string>();
    public string AttributeLabel { get { return attributeLabel.Value; } set { attributeLabel.Value = value; } }

    M4uProperty<Sprite> attributeImage = new M4uProperty<Sprite>();
    public Sprite AttributeImage
    {
        get { return attributeImage.Value; }
        set
        {
            attributeImage.Value = value;
            IsActiveAttribute = (value != null);
        }
    }

    M4uProperty<Color> attributeImageColor = new M4uProperty<Color>(Color.white);
    public Color AttributeImageColor { get { return attributeImageColor.Value; } set { attributeImageColor.Value = value; } }

    M4uProperty<bool> isActiveAttribute = new M4uProperty<bool>();
    public bool IsActiveAttribute { get { return isActiveAttribute.Value; } set { isActiveAttribute.Value = value; } }

    M4uProperty<bool> isViewPremiumButton = new M4uProperty<bool>();
    public bool IsViewPremiumButton { get { return isViewPremiumButton.Value; } set { isViewPremiumButton.Value = value; } }

    private float ExpWidthMax = 186;
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

    // ASLV
    M4uProperty<string> aslvLabel = new M4uProperty<string>();
    public string AslvLabel { get { return aslvLabel.Value; } set { aslvLabel.Value = value; } }

    M4uProperty<string> aslv = new M4uProperty<string>();
    public string Aslv { get { return aslv.Value; } set { aslv.Value = value; } }

    M4uProperty<string> aslvMax = new M4uProperty<string>();
    public string AslvMax { get { return aslvMax.Value; } set { aslvMax.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        RaceLabel = GameTextUtil.GetText("unit_status2");
        AttributeLabel = GameTextUtil.GetText("unit_status3");
        IsViewPremiumButton = false;
        LevelLabel = GameTextUtil.GetText("unit_status4");
        HpLabel = GameTextUtil.GetText("unit_status5");
        AtkLabel = GameTextUtil.GetText("unit_status6");
        CostLabel = GameTextUtil.GetText("unit_status8");
        CharmLabel = GameTextUtil.GetText("unit_status9");
        AslvLabel = GameTextUtil.GetText("unit_status22");
        LoLabel = GameTextUtil.GetText("unit_status10");
        ExpLabel = GameTextUtil.GetText("unit_status11");
        NextExpLabel = "NEXT:";
        IsActiveBG = false;
        IsViewLevel = true;
        IsViewExp = false;
        reset();
        ExpWidthMax = expBar.rect.width;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setupUnit(PacketStructUnit _mainUnit, PacketStructUnit _subUnit = null, bool bFakeLink = false, bool bPink = false)
    {
        MasterDataParamChara _master = MasterFinder<MasterDataParamChara>.Instance.Find((int)_mainUnit.id);
        if (_master == null)
        {
            return;
        }
        setup(_master, _mainUnit, _subUnit, bFakeLink, bPink);
    }

    public void setup(MasterDataParamChara _master, PacketStructUnit _mainUnit, PacketStructUnit _subUnit = null, bool bFakeLink = false, bool bPink = false)
    {
        CharaOnce baseChara = new CharaOnce();
        CharaOnce prevChara = null;
        MasterDataLimitOver _masterMainLO = MasterFinder<MasterDataLimitOver>.Instance.Find((int)_master.limit_over_type);
        if (_masterMainLO == null)
        {
            return;
        }
        MasterDataSkillLimitBreak _masterMainLB = MasterFinder<MasterDataSkillLimitBreak>.Instance.Find((int)_master.skill_limitbreak);

        CharaName = _master.name;
        string noFormat = GameTextUtil.GetText("unit_status1");
        CharaNo = string.Format(noFormat, _master.draw_id);
        Rarity = (uint)_master.rare + 1;

        RaceImage = MainMenuUtil.GetTextKindSprite(_master.kind, false);
        if (_master.sub_kind != MasterDataDefineLabel.KindType.NONE)
        {
            SubRaceImage = MainMenuUtil.GetTextKindSprite(_master.sub_kind, false);
        }
        else
        {
            SubRaceImage = null;
        }

        AttributeImage = MainMenuUtil.GetTextElementSprite(_master.element);
        AttributeImageColor = ColorUtil.GetElementLabelColor(_master.element);

        if (bPink == true)
        {
            prevChara = new CharaOnce();
            prevChara.CharaSetupFromID(
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

        if (IsViewExp == true)
        {
            //-----------------------
            // 次のレベルまでの経験値を算出
            //-----------------------
            int nNowLevelExp = CharaUtil.GetStatusValue(_master, (int)_mainUnit.level, CharaUtil.VALUE.EXP);
            int nNextLevelExp = CharaUtil.GetStatusValue(_master, (int)_mainUnit.level + 1, CharaUtil.VALUE.EXP);
            int nLevelupExp = nNextLevelExp - nNowLevelExp;
            int nNextEXP = nNextLevelExp - (int)_mainUnit.exp;
            float expRatio = 0.0f;
            if (nNextEXP != 0)
            {
                expRatio = (float)(nLevelupExp - nNextEXP) / nLevelupExp;
            }
            NextExp = nNextEXP;
            ExpRate = expRatio;
        }


        setParam(baseChara, _master, _masterMainLO, _masterMainLB, _subUnit, prevChara);
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

        MasterDataSkillLimitBreak _masterMainLB = null;
        if (_masterMain.skill_limitbreak != 0)
        {
            _masterMainLB = MasterFinder<MasterDataSkillLimitBreak>.Instance.Find((int)_masterMain.skill_limitbreak);
        }


        CharaOnce baseChara = new CharaOnce();
        if (_type == StatusType.LV_1)
        {
            baseChara.CharaSetupFromID(
                unit_id,
                1,
                0,
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
                _limitBreakLevel = _masterMainLB.level_max;
            }
            baseChara.CharaSetupFromID(
                unit_id,
                _masterMain.level_max,
                _limitBreakLevel,
                _masterMainLO.limit_over_max,
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

        setParam(baseChara, _masterMain, _masterMainLO, _masterMainLB);
    }

    private void setParam(CharaOnce baseChara, MasterDataParamChara _masterMain, MasterDataLimitOver _masterMainLO, MasterDataSkillLimitBreak _masterMainLB, PacketStructUnit _subUnit = null, CharaOnce prevUnit = null)
    {
        /**
		 * メインユニット情報
		 */
        {
            string levelFormat = GameTextUtil.GetText("unit_status17");
            Level = string.Format(levelFormat, baseChara.m_CharaLevel, _masterMain.level_max);

        }
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
        Charm = baseChara.m_CharaCharm.ToString("F1");

        int limitBreakSkill = 0;
        int limitBreakSkillMax = 0;
        if (_masterMainLB != null)
        {
            limitBreakSkill = baseChara.m_CharaLBSLv + 1;
            limitBreakSkillMax = _masterMainLB.level_max + 1;
        }
        Aslv = limitBreakSkill.ToString();
        AslvMax = limitBreakSkillMax.ToString();

        Lo = baseChara.m_CharaLimitOver.ToString();
        LoMax = _masterMainLO.limit_over_max.ToString();
        if (prevUnit != null)
        {
            if (baseChara.m_CharaHP > prevUnit.m_CharaHP)
            {
                Hp = string.Format(GameTextUtil.GetText("kyouka_text1"), Hp);
            }
            if (baseChara.m_CharaPow > prevUnit.m_CharaPow)
            {
                Atk = string.Format(GameTextUtil.GetText("kyouka_text1"), Atk);
            }
            if (_cost > prevUnit.m_CharaMasterDataParam.party_cost)
            {
                Cost = string.Format(GameTextUtil.GetText("kyouka_text1"), Cost);
            }
            if (baseChara.m_CharaCharm > prevUnit.m_CharaCharm)
            {
                Charm = string.Format(GameTextUtil.GetText("kyouka_text1"), baseChara.m_CharaCharm.ToString("F1"));
            }
        }
    }

    public void reset()
    {
        CharaName = "";
        Rarity = 0;
        RaceImage = null;
        SubRaceImage = null;
        AttributeImage = null;

        Level = "";
        Hp = "";
        Atk = "";
        Cost = "";
        Charm = "";
        Aslv = "";
        AslvMax = "";
        Lo = "";
        LoMax = "";
        NextExp = 0;
        ExpRate = 0.0f;
        BuildupExpRate = 0.0f;
    }

    public void OnSelectPremium(bool bFlag)
    {
        DidSelectPremium(togglePremium.isOn);
    }
}
