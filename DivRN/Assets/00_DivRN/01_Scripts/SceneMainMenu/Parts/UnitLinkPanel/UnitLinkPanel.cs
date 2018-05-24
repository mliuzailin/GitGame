using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;

public class UnitLinkPanel : MenuPartsBase
{
    private static readonly string EmptyStr = "-";
    public enum LinkParamType
    {
        Link,
        LinkEffect,
    };

    public enum LinkStatusType
    {
        LV_1,
        LV_MAX,
    }

    /// <summary>
    /// 
    /// </summary>
    M4uProperty<bool> isViewLinkStatus = new M4uProperty<bool>();
    public bool IsViewLinkStatus { get { return isViewLinkStatus.Value; } set { isViewLinkStatus.Value = value; } }

    M4uProperty<Sprite> iconSelect = new M4uProperty<Sprite>();
    public Sprite IconSelect { get { return iconSelect.Value; } set { iconSelect.Value = value; } }

    M4uProperty<Sprite> iconSelectLeft = new M4uProperty<Sprite>();
    public Sprite IconSelectLeft { get { return iconSelectLeft.Value; } set { iconSelectLeft.Value = value; } }

    M4uProperty<Sprite> iconSelectRight = new M4uProperty<Sprite>();
    public Sprite IconSelectRight { get { return iconSelectRight.Value; } set { iconSelectRight.Value = value; } }


    /// <summary>
    /// 
    /// </summary>
    M4uProperty<bool> isViewNone = new M4uProperty<bool>();
    public bool IsViewNone { get { return isViewNone.Value; } set { isViewNone.Value = value; } }

    M4uProperty<string> noneValue = new M4uProperty<string>();
    public string NoneValue { get { return noneValue.Value; } set { noneValue.Value = value; } }

    /// <summary>
    /// 
    /// </summary>
    M4uProperty<bool> isViewParent = new M4uProperty<bool>();
    public bool IsViewParent { get { return isViewParent.Value; } set { isViewParent.Value = value; } }

    M4uProperty<Sprite> iconLeftImage = new M4uProperty<Sprite>();
    public Sprite IconLeftImage { get { return iconLeftImage.Value; } set { iconLeftImage.Value = value; } }

    M4uProperty<Sprite> iconRightImage = new M4uProperty<Sprite>();
    public Sprite IconRightImage { get { return iconRightImage.Value; } set { iconRightImage.Value = value; } }

    M4uProperty<string> parentText = new M4uProperty<string>();
    public string ParentText { get { return parentText.Value; } set { parentText.Value = value; } }

    M4uProperty<bool> isSkillOnly = new M4uProperty<bool>();
    public bool IsSkillOnly { get { return isSkillOnly.Value; } set { isSkillOnly.Value = value; } }

	M4uProperty<List<UnitSkillContext>> linkList = new M4uProperty<List<UnitSkillContext>>();
	public List<UnitSkillContext> LinkList { get { return linkList.Value; } set { linkList.Value = value; } }

	List<GameObject> linkItemList = new List<GameObject>();
	public List<GameObject> LinkItemList { get { return linkItemList; } set { linkItemList = value; } }

	private CharaOnce m_CharaParam = null;
    private CharaOnce m_CharaLOParam = null;
	private UnitSkillContext m_LinkBonus = null;
	private UnitSkillContext m_RaceBonus = null;
	private UnitSkillContext m_LinkSkill = null;
	private UnitSkillContext m_LinkPassive = null;

	private void Awake()
    {
        IsViewLinkStatus = true;
        IsViewNone = false;

		GetComponent<M4uContextRoot>().Context = this;
		LinkList = new List<UnitSkillContext>();
	}

    public void setupUnit(PacketStructUnit _mainUnit, PacketStructUnit _subUnit, LinkParamType _type)
    {
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

        CHARALINK_TYPE linkType = CHARALINK_TYPE.CHARALINK_TYPE_NONE;
        PacketStructUnit _parentUnit = null;
        if (_mainUnit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK)
        {
            _parentUnit = UserDataAdmin.Instance.SearchChara(_mainUnit.link_unique_id);
        }

        m_CharaParam = new CharaOnce();
        m_CharaLOParam = null;
		MasterDataParamChara _masterSub = null;
		if (_mainUnit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE &&
            _subUnit != null)
        {
            m_CharaParam.CharaSetupFromID(
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
            _masterSub = MasterFinder<MasterDataParamChara>.Instance.Find((int)_subUnit.id);
            if (_masterSub != null)
            {
                IconSelect = MainMenuUtil.GetElementCircleSprite(_masterSub.element);
            }
        }
        else if (_mainUnit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK &&
            _parentUnit != null)
        {
            m_CharaParam.CharaSetupFromID(
                _mainUnit.id,
                (int)_mainUnit.level,
                (int)_mainUnit.limitbreak_lv,
                (int)_mainUnit.limitover_lv,
                (int)_mainUnit.add_pow,
                (int)_mainUnit.add_hp,
                _parentUnit.id,
                (int)_parentUnit.level,
                (int)_parentUnit.add_pow,
                (int)_parentUnit.add_hp,
                (int)_parentUnit.link_point,
                (int)_parentUnit.limitover_lv
                );
        }
        else
        {
            m_CharaParam.CharaSetupFromID(
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

        linkType = (CHARALINK_TYPE)_mainUnit.link_info;
		IsViewParent = false;

		AllClear();
		switch (_type)
        {
            case LinkParamType.Link:
                switch (linkType)
                {
                    case CHARALINK_TYPE.CHARALINK_TYPE_BASE:
						AddUnitDataLink(m_CharaParam, _masterMain);
						AddRaceBonus(_masterSub);
						AddLinkSkill(_masterSub, (uint)m_CharaParam.m_LinkParam.m_CharaLinkPoint);
						AddLinkPassive(_masterSub);
						setActiveGray(false);
                        break;
                    case CHARALINK_TYPE.CHARALINK_TYPE_LINK:
                        SetLinkParent(m_CharaParam);
                        setActiveGray(false);
                        break;
                    case CHARALINK_TYPE.CHARALINK_TYPE_NONE:
                        SetLinkNone();
                        break;
                }
                break;
            case LinkParamType.LinkEffect:
                if (_masterMain.link_enable == MasterDataDefineLabel.BoolType.ENABLE)
                {
                    switch (linkType)
                    {
                        case CHARALINK_TYPE.CHARALINK_TYPE_BASE:
                        case CHARALINK_TYPE.CHARALINK_TYPE_NONE:
							AddLinkEffectInfo(m_CharaParam, _masterMain, false);
							AddRaceBonus(_masterMain);
							AddLinkSkill(_masterMain, 0);
							AddLinkPassive(_masterMain);
							setActiveGray(true);
                            break;
                        case CHARALINK_TYPE.CHARALINK_TYPE_LINK:
							AddLinkEffectInfo(m_CharaParam, _masterMain, true);
							AddRaceBonus(_masterMain);
							AddLinkSkill(_masterMain, (uint)m_CharaParam.m_LinkParam.m_CharaLinkPoint);
							AddLinkPassive(_masterMain);
							setActiveGray(false);
                            break;
                    }
                }
                else
                {
                    SetLinkDisable();
                }
                break;
        }
    }

	public void setupSkill(MasterDataParamChara _masterMain, PacketStructUnit _mainUnit = null)
	{
		AllClear();
		if (_masterMain != null)
		{
			AddLinkSkill(_masterMain, _mainUnit.link_point);
			AddLinkPassive(_masterMain);
			setActiveGray(false);
		}
		else
		{
			AddLinkSkill(null, 0);
			AddLinkPassive(null);
			setActiveGray(true);
		}
	}

	private void SetLinkNone()
    {
        IsViewLinkStatus = false;
        IsViewNone = true;
        IsViewParent = false;
        NoneValue = GameTextUtil.GetText("unit_linkstatus5");
    }

    private void SetLinkParent(CharaOnce cCharaParam)
    {
        IsViewLinkStatus = false;
        IsViewNone = false;
        IsViewParent = true;

        UnitIconImageProvider.Instance.Get(
            cCharaParam.m_CharaMasterDataParam.fix_id,
            sprite =>
            {
                IconRightImage = sprite;
            });
        IconSelectRight = MainMenuUtil.GetElementCircleSprite(cCharaParam.m_CharaMasterDataParam.element);

        UnitIconImageProvider.Instance.Get(
            cCharaParam.m_LinkParam.m_CharaID,
            sprite =>
            {
                IconLeftImage = sprite;
            });
        IconSelectLeft = MainMenuUtil.GetElementCircleSprite(cCharaParam.m_LinkParam.m_cCharaMasterDataParam.element);

        ParentText = GameTextUtil.GetText("unit_linkstatus6");
    }

    private void SetLinkDisable()
    {
        IsViewLinkStatus = false;
        IsViewNone = true;
        IsViewParent = false;
        NoneValue = GameTextUtil.GetText("unit_linkstatus7");
    }

    /// <summary>
    /// タイトル画像のグレー・カラー切り替え
    /// </summary>
    /// <param name="bFlag">true:グレー false:カラー</param>
    public void setActiveGray(bool bFlag)
    {
        string linkBonusPath = "linkubo-nasu";
        string raceBonusPath = "RACE BONUS";
        string linkSkillPath = "LINK SKILL";
        string linkPassivePath = "LINK PASSIVE";

        string linkBonusBGPath;
        string raceBonusBGPath;
        string linkSkillBGPath;
        string linkPassiveBGPath;

        if (bFlag)
        {
            string grayStr = "_gray";
            linkBonusPath += grayStr;
            raceBonusPath += grayStr;
            linkSkillPath += grayStr;
            linkPassivePath += grayStr;

            linkBonusBGPath
            = raceBonusBGPath
            = linkSkillBGPath
            = linkPassiveBGPath
            = "skill_name_bg0";
        }
        else
        {
            linkBonusBGPath = "skill_name_bg1";
            raceBonusBGPath = "skill_name_bg0";
            linkSkillBGPath = "skill_name_bg3";
            linkPassiveBGPath = "skill_name_bg4";

        }

		if (m_LinkBonus != null) m_LinkBonus.setLinkTitleImage(linkBonusPath, linkBonusBGPath);
		if (m_RaceBonus != null) m_RaceBonus.setLinkTitleImage(raceBonusPath, raceBonusBGPath);
		if (m_LinkSkill != null) m_LinkSkill.setLinkTitleImage(linkSkillPath, linkSkillBGPath);
		if (m_LinkPassive != null) m_LinkPassive.setLinkTitleImage(linkPassivePath, linkPassiveBGPath);
	}

	public void AllClear()
	{
		LinkList.Clear();
		LinkItemList.Clear();
	}

	public void AddUnitDataLink(CharaOnce cCharaParam, MasterDataParamChara cCharaMasterData)
	{
		if (m_LinkBonus == null)
		{
			m_LinkBonus = new UnitSkillContext();
		}
		m_LinkBonus.setupUnitData(cCharaParam, cCharaMasterData);
		LinkList.Add(m_LinkBonus);
	}

	public void AddLinkEffectInfo(CharaOnce cCharaOnce, MasterDataParamChara cCharaMasterData, bool bLink)
	{
		if (m_LinkBonus == null)
		{
			m_LinkBonus = new UnitSkillContext();
		}
		m_LinkBonus.setupLinkEffectInfo(cCharaOnce, cCharaMasterData, bLink);
		LinkList.Add(m_LinkBonus);
	}

	public void AddRaceBonus(MasterDataParamChara cCharaMasterData)
	{
		if (m_RaceBonus == null)
		{
			m_RaceBonus = new UnitSkillContext();
		}
		m_RaceBonus.setupRaceBonus(cCharaMasterData);
		LinkList.Add(m_RaceBonus);
	}

	public void AddLinkSkill(MasterDataParamChara cCharaMasterData, uint nLinkPoint)
	{
		if (m_LinkSkill == null)
		{
			m_LinkSkill = new UnitSkillContext();
		}
		m_LinkSkill.setupLinkSkill(cCharaMasterData, nLinkPoint);
		LinkList.Add(m_LinkSkill);
	}

	public void AddLinkPassive(MasterDataParamChara cCharaMasterData)
	{
		if (m_LinkPassive == null)
		{
			m_LinkPassive = new UnitSkillContext();
		}
		m_LinkPassive.setupLinkPassive(cCharaMasterData);
		LinkList.Add(m_LinkPassive);
	}

}
