using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitSkillContext : M4uContext
{
    private readonly float BASE_DEFAULT_HEIGHT = 105.0f;
    private readonly float BASE_LINK_BONUS_HEIGHT = 145.4f;
    private readonly float TITLE_DEFAULT_HEIGHT = 36.0f;
    private readonly float TITLE_DEFAULT_WIDTH = 470.7f;
    private readonly float LINK_BONUS_TITLE_WIDTH = 494;
    private readonly float RACE_BONUS_TITLE_WIDTH = 470.7f;
    private readonly float LINK_SKILL_TITLE_WIDTH = 470.7f;
    private readonly float LINK_PASSIVE_TITLE_WIDTH = 516;
    private readonly float MESSAGE_BG_HEIGHT = 84.9f;
    private readonly float LINK_MESSAGE_BG_HEIGHT = 124.5f;
    private static readonly string EmptyStr = "-";

    protected M4uProperty<float> baseHeight = new M4uProperty<float>();
    public float BaseHeight { get { return baseHeight.Value; } set { baseHeight.Value = value; } }

    protected M4uProperty<float> titleHeight = new M4uProperty<float>();
    public float TitleHeight { get { return titleHeight.Value; } set { titleHeight.Value = value; } }

    protected M4uProperty<float> titleWidth = new M4uProperty<float>();
    public float TitleWidth { get { return titleWidth.Value; } set { titleWidth.Value = value; } }

    protected M4uProperty<Sprite> titleBGImage = new M4uProperty<Sprite>();
    public Sprite TitleBGImage { get { return titleBGImage.Value; } set { titleBGImage.Value = value; } }

    //
    protected M4uProperty<Sprite> skillTitleImage = new M4uProperty<Sprite>();
    public Sprite SkillTitleImage { get { return skillTitleImage.Value; } set { skillTitleImage.Value = value; } }

    protected M4uProperty<string> skillTitleText = new M4uProperty<string>();
    public string SkillTitleText { get { return skillTitleText.Value; } set { skillTitleText.Value = value; } }

    protected M4uProperty<string> skillDetailText = new M4uProperty<string>();
    public string SkillDetailText { get { return skillDetailText.Value; } set { skillDetailText.Value = value; } }

    protected M4uProperty<float> skillTitleY = new M4uProperty<float>(0);
    public float SkillTitleY { get { return skillTitleY.Value; } set { skillTitleY.Value = value; } }

    //Element(NormalSkill)
    protected M4uProperty<bool> isViewElement = new M4uProperty<bool>();
    public bool IsViewElement { get { return isViewElement.Value; } set { isViewElement.Value = value; } }

    protected M4uProperty<List<UnitSkillElemContext>> elementList = new M4uProperty<List<UnitSkillElemContext>>();
    public List<UnitSkillElemContext> ElementList { get { return elementList.Value; } set { elementList.Value = value; } }

    //Turn(ActiveSkill)
    protected M4uProperty<bool> isViewTurn = new M4uProperty<bool>();
    public bool IsViewTurn { get { return isViewTurn.Value; } set { isViewTurn.Value = value; } }

    protected M4uProperty<string> turnLabel = new M4uProperty<string>();
    public string TurnLabel { get { return turnLabel.Value; } set { turnLabel.Value = value; } }

    protected M4uProperty<string> turnValue = new M4uProperty<string>();
    public string TurnValue { get { return turnValue.Value; } set { turnValue.Value = value; } }

    protected M4uProperty<float> messageBgH = new M4uProperty<float>();
    public float MessageBgH { get { return messageBgH.Value; } set { messageBgH.Value = value; } }

    /// <summary>アクティブスキルのラベルカラーを黒にするかどうか</summary>
    public bool IsTurnLabelBlackColor { get; private set; }

    //SkillLevel(ActiveSkill)
    protected M4uProperty<bool> isViewSkillLevel = new M4uProperty<bool>();
    public bool IsViewSkillLevel { get { return isViewSkillLevel.Value; } set { isViewSkillLevel.Value = value; } }

    protected M4uProperty<string> skillLevelText = new M4uProperty<string>();
    public string SkillLevelText { get { return skillLevelText.Value; } set { skillLevelText.Value = value; } }

    protected M4uProperty<bool> isSkillItem = new M4uProperty<bool>();
    public bool IsSkillItem { get { return isSkillItem.Value; } set { isSkillItem.Value = value; } }

    protected M4uProperty<Sprite> linkTitleImage = new M4uProperty<Sprite>();
    public Sprite LinkTitleImage { get { return linkTitleImage.Value; } set { linkTitleImage.Value = value; } }

    protected M4uProperty<string> linkTitleText = new M4uProperty<string>();
    public string LinkTitleText { get { return linkTitleText.Value; } set { linkTitleText.Value = value; } }

    protected M4uProperty<bool> isViewIcon = new M4uProperty<bool>();
    public bool IsViewIcon { get { return isViewIcon.Value; } set { isViewIcon.Value = value; } }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<string> lvLabel = new M4uProperty<string>();
    public string LvLabel { get { return lvLabel.Value; } set { lvLabel.Value = value; } }

    M4uProperty<int> lv = new M4uProperty<int>();
    public int Lv { get { return lv.Value; } set { lv.Value = value; } }

    M4uProperty<string> hpLabel = new M4uProperty<string>();
    public string HpLabel { get { return hpLabel.Value; } set { hpLabel.Value = value; } }

    M4uProperty<string> hp = new M4uProperty<string>();
    public string Hp { get { return hp.Value; } set { hp.Value = value; } }

    M4uProperty<string> atkLabel = new M4uProperty<string>();
    public string AtkLabel { get { return atkLabel.Value; } set { atkLabel.Value = value; } }

    M4uProperty<string> atk = new M4uProperty<string>();
    public string Atk { get { return atk.Value; } set { atk.Value = value; } }

    M4uProperty<string> costLabel = new M4uProperty<string>();
    public string CostLabel { get { return costLabel.Value; } set { costLabel.Value = value; } }

    M4uProperty<int> cost = new M4uProperty<int>();
    public int Cost { get { return cost.Value; } set { cost.Value = value; } }

    M4uProperty<string> linkLabel = new M4uProperty<string>();
    public string LinkLabel { get { return linkLabel.Value; } set { linkLabel.Value = value; } }

    M4uProperty<string> link = new M4uProperty<string>();
    public string Link { get { return link.Value; } set { link.Value = value; } }

    M4uProperty<string> charmLabel = new M4uProperty<string>();
    public string CharmLabel { get { return charmLabel.Value; } set { charmLabel.Value = value; } }

    M4uProperty<string> charm = new M4uProperty<string>();
    public string Charm { get { return charm.Value; } set { charm.Value = value; } }

    M4uProperty<Sprite> iconSelect = new M4uProperty<Sprite>();
    public Sprite IconSelect { get { return iconSelect.Value; } set { iconSelect.Value = value; } }

    protected M4uProperty<bool> isViewStatus = new M4uProperty<bool>();
    public bool IsViewStatus { get { return isViewStatus.Value; } set { isViewStatus.Value = value; } }

    M4uProperty<string> linkSkillRate = new M4uProperty<string>();
    public string LinkSkillRate { get { return linkSkillRate.Value; } set { linkSkillRate.Value = value; } }

    /// <summary>
    /// リーダースキル設定
    /// </summary>
    /// <param name="skill_id"></param>
    public void setupLeaderSkill(uint skill_id)
    {
        BaseHeight = BASE_DEFAULT_HEIGHT;
        TitleHeight = TITLE_DEFAULT_HEIGHT;
        TitleWidth = TITLE_DEFAULT_WIDTH;
        MessageBgH = MESSAGE_BG_HEIGHT;
        TitleBGImage = ResourceManager.Instance.Load("skill_name_bg1", ResourceType.Common);
        SkillTitleImage = ResourceManager.Instance.Load("Unit_LEADER", ResourceType.Common);
        IsViewElement = false;
        IsViewTurn = false;
        IsViewSkillLevel = false;
        IsTurnLabelBlackColor = false;
        IsSkillItem = true;
        IsViewIcon = false;
        IsViewStatus = false;

        if (skill_id != 0)
        {
            MasterDataSkillLeader _master = MasterFinder<MasterDataSkillLeader>.Instance.Find((int)skill_id);
            if (_master == null)
            {
                return;
            }

            SkillTitleText = _master.name;
            SkillDetailText = _master.detail;
        }
        else
        {
            SkillTitleText = "-";
            SkillDetailText = "-";
        }
        MessageBgResize();
    }

    /// <summary>
    /// アクティブスキル設定
    /// </summary>
    /// <param name="skill_id"></param>
    /// <param name="level"></param>
    public void setupLimitBreakSkill(uint skill_id, uint level, bool is_turn_labal_black = false)
    {
        MessageBgH = MESSAGE_BG_HEIGHT;
        TitleWidth = TITLE_DEFAULT_WIDTH;
        TitleBGImage = ResourceManager.Instance.Load("skill_name_bg2", ResourceType.Common);
        SkillTitleImage = ResourceManager.Instance.Load("ACTIVE", ResourceType.Common);
        IsTurnLabelBlackColor = is_turn_labal_black;
        IsSkillItem = true;
        IsViewIcon = false;
        IsViewStatus = false;

        if (skill_id != 0)
        {
            BaseHeight = BASE_DEFAULT_HEIGHT + 12.0f;
            TitleHeight = TITLE_DEFAULT_HEIGHT + 12.0f;
            SkillTitleY = 6;
            IsViewElement = false;
            IsViewTurn = true;
            IsViewSkillLevel = true;

            MasterDataSkillLimitBreak _master = MasterFinder<MasterDataSkillLimitBreak>.Instance.Find((int)skill_id);
            if (_master == null)
            {
                return;
            }

            SkillTitleText = _master.name;
            SkillDetailText = _master.detail;

            TurnLabel = "";
            TurnValue = string.Format(GameTextUtil.GetText("unit_skill2"), (_master.use_turn - (int)level));

            SkillLevelText = string.Format(GameTextUtil.GetText("unit_skill1"), level + 1, _master.level_max + 1);
        }
        else
        {
            BaseHeight = BASE_DEFAULT_HEIGHT;
            TitleHeight = TITLE_DEFAULT_HEIGHT;
            IsViewElement = false;
            IsViewTurn = false;
            IsViewSkillLevel = false;

            SkillTitleText = "-";
            SkillDetailText = "-";
        }
        MessageBgResize();
    }

    /// <summary>
    /// ノーマルスキル設定
    /// </summary>
    /// <param name="skill_id"></param>
    public void setupActiveSkill(uint skill_id)
    {
        BaseHeight = BASE_DEFAULT_HEIGHT;
        TitleHeight = TITLE_DEFAULT_HEIGHT;
        TitleWidth = TITLE_DEFAULT_WIDTH;
        MessageBgH = MESSAGE_BG_HEIGHT;
        TitleBGImage = ResourceManager.Instance.Load("skill_name_bg4", ResourceType.Common);
        SkillTitleImage = ResourceManager.Instance.Load("NORMAL", ResourceType.Common);
        IsTurnLabelBlackColor = false;
        IsSkillItem = true;
        IsViewIcon = false;
        IsViewStatus = false;

        if (skill_id != 0)
        {
            IsViewElement = true;
            IsViewTurn = false;
            IsViewSkillLevel = false;

            MasterDataSkillActive _master = MasterFinder<MasterDataSkillActive>.Instance.Find((int)skill_id);
            if (_master == null)
            {
                return;
            }

            SkillTitleText = _master.name;
            SkillDetailText = _master.detail;

            if (ElementList == null) ElementList = new List<UnitSkillElemContext>();
            if (_master.cost1 != MasterDataDefineLabel.ElementType.NONE) ElementList.Add(new UnitSkillElemContext(_master.cost1));
            if (_master.cost2 != MasterDataDefineLabel.ElementType.NONE) ElementList.Add(new UnitSkillElemContext(_master.cost2));
            if (_master.cost3 != MasterDataDefineLabel.ElementType.NONE) ElementList.Add(new UnitSkillElemContext(_master.cost3));
            if (_master.cost4 != MasterDataDefineLabel.ElementType.NONE) ElementList.Add(new UnitSkillElemContext(_master.cost4));
            if (_master.cost5 != MasterDataDefineLabel.ElementType.NONE) ElementList.Add(new UnitSkillElemContext(_master.cost5));
        }
        else
        {
            IsViewElement = false;
            IsViewTurn = false;
            IsViewSkillLevel = false;

            SkillTitleText = "-";
            SkillDetailText = "-";
        }
        MessageBgResize();
    }

    /// <summary>
    /// パッシブスキル設定
    /// </summary>
    /// <param name="skill_id"></param>
    public void setupPassiveSkill(uint skill_id)
    {
        BaseHeight = BASE_DEFAULT_HEIGHT;
        TitleHeight = TITLE_DEFAULT_HEIGHT;
        TitleWidth = TITLE_DEFAULT_WIDTH;
        MessageBgH = MESSAGE_BG_HEIGHT;
        TitleBGImage = ResourceManager.Instance.Load("skill_name_bg3", ResourceType.Common);
        SkillTitleImage = ResourceManager.Instance.Load("Unit_PASSIVE", ResourceType.Common);
        IsViewElement = false;
        IsViewTurn = false;
        IsViewSkillLevel = false;
        IsTurnLabelBlackColor = false;
        IsSkillItem = true;
        IsViewIcon = false;
        IsViewStatus = false;

        MasterDataSkillPassive _master = MasterFinder<MasterDataSkillPassive>.Instance.Find((int)skill_id);
        if (_master == null)
        {
            return;
        }

        SkillTitleText = _master.name;
        SkillDetailText = _master.detail;
        MessageBgResize();
    }

    /// <summary>
    /// フレンドスキル設定
    /// </summary>
    /// <param name="skill_id"></param>
    public void setupFriendSkill(uint skill_id)
    {
        BaseHeight = BASE_DEFAULT_HEIGHT;
        TitleHeight = TITLE_DEFAULT_HEIGHT;
        TitleWidth = TITLE_DEFAULT_WIDTH;
        MessageBgH = MESSAGE_BG_HEIGHT;
        TitleBGImage = ResourceManager.Instance.Load("skill_name_azure", ResourceType.Common);
        SkillTitleImage = ResourceManager.Instance.Load("Unit_FRIEND", ResourceType.Common);
        IsViewElement = false;
        IsViewTurn = false;
        IsViewSkillLevel = false;
        IsTurnLabelBlackColor = false;
        IsSkillItem = true;
        IsViewIcon = false;
        IsViewStatus = false;

        if (skill_id != 0)
        {
            MasterDataSkillLeader _master = MasterFinder<MasterDataSkillLeader>.Instance.Find((int)skill_id);
            if (_master == null)
            {
                return;
            }

            SkillTitleText = _master.name;
            SkillDetailText = _master.detail;
        }
        else
        {
            SkillTitleText = "-";
            SkillDetailText = "-";
        }
        MessageBgResize();
    }

    /// <summary>
    /// ヒーロースキル設定
    /// </summary>
    /// <param name="skill_id"></param>
    public void setupHeroSkill(uint skill_id, int hero_id = 0, int level = 0, bool is_turn_labal_black = false)
    {
        MessageBgH = MESSAGE_BG_HEIGHT;
        TitleWidth = TITLE_DEFAULT_WIDTH;
        TitleBGImage = ResourceManager.Instance.Load("skill_name_bg2", ResourceType.Common);
        SkillTitleImage = ResourceManager.Instance.Load("tutorial_select_master", ResourceType.Common);
        IsTurnLabelBlackColor = is_turn_labal_black;
        IsSkillItem = true;
        IsViewIcon = false;
        IsViewStatus = false;
        SkillTitleY = 0;

        if (skill_id != 0)
        {
            BaseHeight = BASE_DEFAULT_HEIGHT;
            TitleHeight = TITLE_DEFAULT_HEIGHT;
            IsViewElement = false;
            IsViewTurn = true;
            IsViewSkillLevel = false;

            MasterDataSkillLimitBreak _master = MasterFinder<MasterDataSkillLimitBreak>.Instance.Find((int)skill_id);
            if (_master == null)
            {
                return;
            }

            SkillTitleText = _master.name;
            SkillDetailText = _master.detail;
            if (hero_id > 0 && level > 0)
            {
                MasterDataHeroAddEffectRate _addRate = MasterFinder<MasterDataHeroAddEffectRate>.Instance.SelectWhere("where hero_id = ? and start_level <= ? and ? <= end_level", hero_id, level, level).First();
                if (_addRate != null)
                {
                    SkillDetailText += string.Format("\n" + GameTextUtil.GetText("hero_skill_activationrate"), _addRate.additional_effect_value);
                }
            }

            TurnLabel = "";

            TurnValue = string.Format(GameTextUtil.GetText("unit_skill3"), (_master.use_turn));
        }
        else
        {
            BaseHeight = BASE_DEFAULT_HEIGHT;
            TitleHeight = TITLE_DEFAULT_HEIGHT;
            IsViewElement = false;
            IsViewTurn = false;
            IsViewSkillLevel = false;

            SkillTitleText = "-";
            SkillDetailText = "-";
        }
        MessageBgResize();
        MessageBgResize();
    }

    /// <summary>
    /// リンクボーナス設定
    /// </summary>
    /// <param name="skill_id"></param>
    public void setupUnitData(CharaOnce cCharaParam, MasterDataParamChara cCharaMasterData)
    {
        BaseHeight = BASE_LINK_BONUS_HEIGHT;
        TitleHeight = TITLE_DEFAULT_HEIGHT;
        TitleWidth = LINK_BONUS_TITLE_WIDTH;
        MessageBgH = LINK_MESSAGE_BG_HEIGHT;
        setLinkTitleImage("skill_name_bg1", "linkubo-nasu");
        LvLabel = GameTextUtil.GetText("unit_status4");
        HpLabel = GameTextUtil.GetText("unit_status5");
        AtkLabel = GameTextUtil.GetText("unit_status6");
        CostLabel = GameTextUtil.GetText("unit_status8");
        LinkLabel = GameTextUtil.GetText("unit_linkstatus3");
        CharmLabel = GameTextUtil.GetText("unit_status9");
        IsSkillItem = false;
        IsViewStatus = true;
        IsViewIcon = true;


        //--------------------------------
        // 選択されているユニット情報を選定
        //--------------------------------
        uint unCharaLevel = (uint)cCharaParam.m_CharaLevel;  // キャラレベル
                                                             // +情報

        //----------------------------------------
        // リンク情報の設定 v300対応
        //----------------------------------------

        //リンクキャラの定義取得
        uint unCharaId = cCharaParam.m_LinkParam.m_CharaID;
        // リンク対象ユニット
        MasterDataParamChara cCharaParamLink = MasterDataUtil.GetCharaParamFromID(unCharaId);
        // コスト
        int nCharaCost = cCharaMasterData.party_cost;

        if (unCharaId > 0
        && cCharaParamLink != null)
        {
            //------------------------------------------------------------
            // LINK BONUS
            //------------------------------------------------------------
            //ユニット名
            LinkTitleText = cCharaParamLink.name;

            //レベル
            Lv = cCharaParam.m_LinkParam.m_CharaLv;

            //HP
            int nHp = (CharaLinkUtil.GetLinkUnitBonusElement(cCharaParamLink, cCharaParam.m_LinkParam.m_CharaLv, 0, CharaUtil.VALUE.HP) +
                                                            CharaLinkUtil.GetLinkUnitBonusPlus(cCharaParam.m_LinkParam.m_CharaPlusHP, CharaUtil.VALUE.HP));
            int nPlusHp = cCharaParam.m_LinkParam.m_CharaPlusHP;
            // ATK
            int nPow = (CharaLinkUtil.GetLinkUnitBonusElement(cCharaParamLink, cCharaParam.m_LinkParam.m_CharaLv, 0, CharaUtil.VALUE.POW) +
                                                            CharaLinkUtil.GetLinkUnitBonusPlus(cCharaParam.m_LinkParam.m_CharaPlusPow, CharaUtil.VALUE.POW));
            int nPlusPow = cCharaParam.m_LinkParam.m_CharaPlusPow;

            double nCharm = 0;

            //COST
            Cost = cCharaParamLink.party_cost;

            // リンクポイント
            float fLinkPoint = (cCharaParam.m_LinkParam.m_CharaLinkPoint) * 0.01f;
            Link = string.Format(GameTextUtil.GetText("unit_linkstatus4"), fLinkPoint);

            uint nLimitOverLevel = (uint)cCharaParam.m_LinkParam.m_CharaLOLevel;

            if (nLimitOverLevel > 0)
            {
                // 限界突破タイプ
                int nLimitOverType = cCharaParamLink.limit_over_type;

                // 限界突破後のコスト
                //Cost = (int)CharaLimitOver.GetParam(nLimitOverLevel, nLimitOverType, (int)CharaLimitOver.EGET.ePARAM_COST);

                // 限界突破後のHp
                nHp = CharaLinkUtil.GetLinkUnitBonusElement(unCharaId, Lv, nLimitOverLevel, CharaUtil.VALUE.HP) + CharaLinkUtil.GetLinkUnitBonusPlus(nPlusHp, CharaUtil.VALUE.HP);

                // 限界突破後のPow
                nPow = CharaLinkUtil.GetLinkUnitBonusElement(unCharaId, Lv, nLimitOverLevel, CharaUtil.VALUE.POW) + CharaLinkUtil.GetLinkUnitBonusPlus(nPlusPow, CharaUtil.VALUE.POW);

                // CHARM
                nCharm = CharaLimitOver.GetParamCharm(nLimitOverLevel, nLimitOverType);
            }

            //キャラアイコン
            IsViewIcon = true;
            IconSelect = MainMenuUtil.GetElementCircleSprite(cCharaParamLink.element);
            UnitIconImageProvider.Instance.Get(
                cCharaParamLink.fix_id,
                sprite =>
                {
                    IconImage = sprite;
                });

            if (nPlusHp > 0)
            {
                Hp = string.Format(GameTextUtil.GetText("unit_status19"), nHp, nPlusHp);
            }
            else
            {
                Hp = nHp.ToString();
            }

            if (nPlusPow > 0)
            {
                Atk = string.Format(GameTextUtil.GetText("unit_status19"), nPow, nPlusPow);
            }
            else
            {
                Atk = nPow.ToString();
            }

            Charm = nCharm.ToString("F1");
        }
    }

    /// <summary>
    /// リンクボーナス設定
    /// </summary>
    /// <param name="skill_id"></param>
    public uint setupLinkEffectInfo(CharaOnce cCharaOnce, MasterDataParamChara cCharaMasterData, bool bLink)
    {
        BaseHeight = BASE_LINK_BONUS_HEIGHT;
        TitleHeight = TITLE_DEFAULT_HEIGHT;
        TitleWidth = LINK_BONUS_TITLE_WIDTH;
        MessageBgH = LINK_MESSAGE_BG_HEIGHT;
        setLinkTitleImage("skill_name_bg1", "linkubo-nasu");
        LvLabel = GameTextUtil.GetText("unit_status4");
        HpLabel = GameTextUtil.GetText("unit_status5");
        AtkLabel = GameTextUtil.GetText("unit_status6");
        CostLabel = GameTextUtil.GetText("unit_status8");
        LinkLabel = GameTextUtil.GetText("unit_linkstatus3");
        CharmLabel = GameTextUtil.GetText("unit_status9");
        IsSkillItem = false;
        IsViewStatus = true;
        IsViewIcon = true;


        //
        LinkTitleText = "";

        //----------------------------------------
        // リンク効果設定
        //----------------------------------------
        //int nUnitId = (int)cCharaMasterData.fix_id;
        int nLv = cCharaOnce.m_CharaLevel;
        int nPlusHp = cCharaOnce.m_CharaPlusHP;
        int nPlusPow = cCharaOnce.m_CharaPlusPow;
        int nCost = cCharaMasterData.party_cost;
        uint unID = cCharaMasterData.fix_id;
        double nCharm = 0;

        //------------------------------------------------------------
        // LINK BONUS
        //------------------------------------------------------------
        //レベル
        Lv = nLv;

        //HP
        int nHp = (CharaLinkUtil.GetLinkUnitBonusElement(unID, nLv, 0, CharaUtil.VALUE.HP) +
                                                     CharaLinkUtil.GetLinkUnitBonusPlus(nPlusHp, CharaUtil.VALUE.HP));

        // ATK
        int nPow = (CharaLinkUtil.GetLinkUnitBonusElement(unID, nLv, 0, CharaUtil.VALUE.POW) +
                                                     CharaLinkUtil.GetLinkUnitBonusPlus(nPlusPow, CharaUtil.VALUE.POW));

        //COST
        Cost = nCost;

        //アイコン
        IsViewIcon = false;

        uint nLinkPoint = 0;

        //リンクポイント
        if (bLink)
        {
            //リンク子の場合はリンクポイントを反映
            nLinkPoint = (uint)cCharaOnce.m_LinkParam.m_CharaLinkPoint;
            float fLinkPoint = nLinkPoint * 0.01f;
            Link = string.Format(GameTextUtil.GetText("unit_linkstatus4"), fLinkPoint);
        }
        else
        {
            //ハイフン表示
            Link = EmptyStr;
        }

        uint nLimitOverLevel = (uint)cCharaOnce.m_CharaLimitOver;

        if (nLimitOverLevel > 0)
        {
            // 限界突破タイプ
            int nLimitOverType = cCharaMasterData.limit_over_type;

            // 限界突破後のコスト
            //Cost = (int)CharaLimitOver.GetParam(nLimitOverLevel, nLimitOverType, (int)CharaLimitOver.EGET.ePARAM_COST);

            // 限界突破後のHp
            nHp = CharaLinkUtil.GetLinkUnitBonusElement(unID, nLv, nLimitOverLevel, CharaUtil.VALUE.HP) + CharaLinkUtil.GetLinkUnitBonusPlus(nPlusHp, CharaUtil.VALUE.HP);

            // 限界突破後のPow
            nPow = CharaLinkUtil.GetLinkUnitBonusElement(unID, nLv, nLimitOverLevel, CharaUtil.VALUE.POW) + CharaLinkUtil.GetLinkUnitBonusPlus(nPlusPow, CharaUtil.VALUE.POW);

            // CHARM
            nCharm = CharaLimitOver.GetParamCharm(nLimitOverLevel, nLimitOverType);
        }

        if (nPlusHp > 0)
        {
            Hp = string.Format(GameTextUtil.GetText("unit_status19"), nHp, nPlusHp);
        }
        else
        {
            Hp = nHp.ToString();
        }

        if (nPlusPow > 0)
        {
            Atk = string.Format(GameTextUtil.GetText("unit_status19"), nPow, nPlusPow);
        }
        else
        {
            Atk = nPow.ToString();
        }

        Charm = nCharm.ToString("F1");

        return nLinkPoint;
    }

    /// <summary>
    /// 種族ボーナス設定
    /// </summary>
    /// <param name="skill_id"></param>
    public void setupRaceBonus(MasterDataParamChara cCharaMasterData)
    {
        BaseHeight = BASE_DEFAULT_HEIGHT;
        TitleHeight = TITLE_DEFAULT_HEIGHT;
        TitleWidth = RACE_BONUS_TITLE_WIDTH;
        MessageBgH = MESSAGE_BG_HEIGHT;
        setLinkTitleImage("skill_name_bg0", "RACE BONUS");
        IsSkillItem = false;
        IsViewStatus = false;
        IsViewIcon = false;
        if (cCharaMasterData != null)
        {
            // 種族
            LinkTitleText = GameTextUtil.GetKindToText(cCharaMasterData.kind, cCharaMasterData.sub_kind);
            // 説明文
            SkillDetailText = CharaLinkUtil.GetLinkBonusRaceText(cCharaMasterData);
        }
        else
        {
            // 説明文
            SkillDetailText = EmptyStr;
        }
        MessageBgResize();
    }

    /// <summary>
    /// リンクスキル設定
    /// </summary>
    /// <param name="skill_id"></param>
    public void setupLinkSkill(MasterDataParamChara cCharaMasterData, uint nLinkPoint)
    {
        BaseHeight = BASE_DEFAULT_HEIGHT;
        TitleHeight = TITLE_DEFAULT_HEIGHT;
        TitleWidth = LINK_SKILL_TITLE_WIDTH;
        MessageBgH = MESSAGE_BG_HEIGHT;
        setLinkTitleImage("skill_name_bg3", "LINK SKILL");
        IsSkillItem = false;
        IsViewStatus = false;
        IsViewIcon = false;
        //------------------------------------------------------------
        // LINK SKILL
        //------------------------------------------------------------
        MasterDataSkillActive cSkillParamLinkActive = null;
        if (cCharaMasterData != null)
        {
            cSkillParamLinkActive = MasterDataUtil.GetActiveSkillParamFromID(cCharaMasterData.link_skill_active);
        }
        if (null != cSkillParamLinkActive)
        {
            // スキル名
            LinkTitleText = cSkillParamLinkActive.skill_link_name;

            // 説明文
            SkillDetailText = cSkillParamLinkActive.skill_link_detail;

            //発動率
            float fSkillOdds = CharaLinkUtil.GetLinkSkillOdds(cSkillParamLinkActive, (int)nLinkPoint) * 0.01f;
            LinkSkillRate = string.Format(GameTextUtil.GetText("unit_linkstatus2"), fSkillOdds.ToString("F1"));
        }
        else
        {
            // スキル名
            LinkTitleText = EmptyStr;
            // 説明文
            SkillDetailText = EmptyStr;
            // 発動率
            LinkSkillRate = "";
        }
        MessageBgResize();
    }

    /// <summary>
    /// リンクパッシブ設定
    /// </summary>
    /// <param name="skill_id"></param>
    public void setupLinkPassive(MasterDataParamChara cCharaMasterData)
    {
        BaseHeight = BASE_DEFAULT_HEIGHT;
        TitleHeight = TITLE_DEFAULT_HEIGHT;
        TitleWidth = LINK_PASSIVE_TITLE_WIDTH;
        MessageBgH = MESSAGE_BG_HEIGHT;
        setLinkTitleImage("skill_name_bg4", "LINK PASSIVE");
        IsSkillItem = false;
        IsViewStatus = false;
        IsViewIcon = false;
        //------------------------------------------------------------
        // LINK PASSIVE
        //------------------------------------------------------------
        MasterDataSkillPassive cSkillParamLinkPassive = null;
        if (cCharaMasterData != null)
        {
            cSkillParamLinkPassive = MasterDataUtil.GetPassiveSkillParamFromID(cCharaMasterData.link_skill_passive);
        }
        if (null != cSkillParamLinkPassive)
        {
            // スキル名
            LinkTitleText = cSkillParamLinkPassive.name;

            // 説明文
            SkillDetailText = cSkillParamLinkPassive.detail;
        }
        else
        {
            // スキル名
            LinkTitleText = EmptyStr;

            // 説明文
            SkillDetailText = EmptyStr;
        }
        MessageBgResize();
    }

    private void MessageBgResize()
    {
        string[] lineText = SkillDetailText.Split('\n');
        if (lineText.Length > 3)
        {
            float resizeH = (lineText.Length - 3) * 21;
            MessageBgH += resizeH;
            BaseHeight += resizeH;
        }
    }

    public void setLinkTitleImage(string imageName, string bgName)
    {
        TitleBGImage = ResourceManager.Instance.Load(bgName, ResourceType.Common);
        LinkTitleImage = ResourceManager.Instance.Load(imageName, ResourceType.Common);
    }
}
