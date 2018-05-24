using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using ServerDataDefine;

public class UnitSkillLinkListItem : M4uContextMonoBehaviour
{
	private static readonly string EmptyStr = "-";

	/// <summary>
	/// リンクボーナス
	/// </summary>
	M4uProperty<Sprite> linkBonusBG = new M4uProperty<Sprite>();
	public Sprite LinkBonusBG { get { return linkBonusBG.Value; } set { linkBonusBG.Value = value; } }

	M4uProperty<Sprite> linkBonusImage = new M4uProperty<Sprite>();
	public Sprite LinkBonusImage { get { return linkBonusImage.Value; } set { linkBonusImage.Value = value; } }

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

	/// <summary>
	/// 種族ボーナス
	/// </summary>
	M4uProperty<Sprite> raceBonusBG = new M4uProperty<Sprite>();
	public Sprite RaceBonusBG { get { return raceBonusBG.Value; } set { raceBonusBG.Value = value; } }

	M4uProperty<Sprite> raceBonusImage = new M4uProperty<Sprite>();
	public Sprite RaceBonusImage { get { return raceBonusImage.Value; } set { raceBonusImage.Value = value; } }

	M4uProperty<string> raceName = new M4uProperty<string>();
	public string RaceName { get { return raceName.Value; } set { raceName.Value = value; } }

	M4uProperty<string> raceBonusMessage = new M4uProperty<string>();
	public string RaceBonusMessage { get { return raceBonusMessage.Value; } set { raceBonusMessage.Value = value; } }

	/// <summary>
	/// リンクスキル
	/// </summary>
	M4uProperty<Sprite> linkSkillBG = new M4uProperty<Sprite>();
	public Sprite LinkSkillBG { get { return linkSkillBG.Value; } set { linkSkillBG.Value = value; } }

	M4uProperty<Sprite> linkSkillImage = new M4uProperty<Sprite>();
	public Sprite LinkSkillImage { get { return linkSkillImage.Value; } set { linkSkillImage.Value = value; } }

	M4uProperty<string> linkSkillName = new M4uProperty<string>();
	public string LinkSkillName { get { return linkSkillName.Value; } set { linkSkillName.Value = value; } }

	M4uProperty<string> linkSkillMessage = new M4uProperty<string>();
	public string LinkSkillMessage { get { return linkSkillMessage.Value; } set { linkSkillMessage.Value = value; } }

	M4uProperty<string> linkSkillRate = new M4uProperty<string>();
	public string LinkSkillRate { get { return linkSkillRate.Value; } set { linkSkillRate.Value = value; } }

	/// <summary>
	/// リンクパッシブ
	/// </summary>
	M4uProperty<Sprite> linkPassiveBG = new M4uProperty<Sprite>();
	public Sprite LinkPassiveBG { get { return linkPassiveBG.Value; } set { linkPassiveBG.Value = value; } }

	M4uProperty<Sprite> linkPassiveImage = new M4uProperty<Sprite>();
	public Sprite LinkPassiveImage { get { return linkPassiveImage.Value; } set { linkPassiveImage.Value = value; } }

	M4uProperty<string> linkPassiveName = new M4uProperty<string>();
	public string LinkPassiveName { get { return linkPassiveName.Value; } set { linkPassiveName.Value = value; } }

	M4uProperty<string> linkPassiveMessage = new M4uProperty<string>();
	public string LinkPassiveMessage { get { return linkPassiveMessage.Value; } set { linkPassiveMessage.Value = value; } }

	private void Awake()
	{
		LvLabel = GameTextUtil.GetText("unit_status4");
		HpLabel = GameTextUtil.GetText("unit_status5");
		AtkLabel = GameTextUtil.GetText("unit_status6");
		CostLabel = GameTextUtil.GetText("unit_status8");
		LinkLabel = GameTextUtil.GetText("unit_linkstatus3");
		CharmLabel = GameTextUtil.GetText("unit_status9");

		GetComponent<M4uContextRoot>().Context = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setupUnit(uint unit_id, PacketStructUnit _mainUnit, PacketStructUnit _subUnit)
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

		CHARALINK_TYPE linkType = CHARALINK_TYPE.CHARALINK_TYPE_NONE;

		CharaOnce m_CharaParam = new CharaOnce();
		if (_mainUnit == null)
		{
			m_CharaParam.CharaSetupFromID(
				unit_id,
				_masterMain.level_max,
				0,
				_masterMainLO.limit_over_max,
				0,
				0,
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
			PacketStructUnit _parentUnit = null;
			if (_mainUnit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK)
			{
				_parentUnit = UserDataAdmin.Instance.SearchChara(_mainUnit.link_unique_id);
			}
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
		}
		switch (linkType)
		{
			case CHARALINK_TYPE.CHARALINK_TYPE_BASE:
			case CHARALINK_TYPE.CHARALINK_TYPE_NONE:
				SetLinkEffectInfo(m_CharaParam, _masterMain, false);
				setActiveGray(true);
				break;
			case CHARALINK_TYPE.CHARALINK_TYPE_LINK:
				SetLinkEffectInfo(m_CharaParam, _masterMain, true);
				setActiveGray(false);
				break;
		}
	}

	private void SetLinkEffectInfo(CharaOnce cCharaOnce, MasterDataParamChara cCharaMasterData, bool bLink)
	{
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
			Hp = string.Format(GameTextUtil.GetText("unit_linkstatus1"), nHp, nPlusHp);
		}
		else
		{
			Hp = nHp.ToString();
		}

		if (nPlusPow > 0)
		{
			Atk = string.Format(GameTextUtil.GetText("unit_linkstatus1"), nPow, nPlusPow);
		}
		else
		{
			Atk = nPow.ToString();
		}

		Charm = nCharm.ToString("F1");

		//------------------------------------------------------------
		// RACE BONUS
		//------------------------------------------------------------
		// 種族
		//RaceName = GameTextUtil.GetKindToText(cCharaOnce.kind, cCharaOnce.kind_sub);
		RaceName = GameTextUtil.GetKindToText(cCharaMasterData.kind, cCharaMasterData.sub_kind);

		// 説明文
		RaceBonusMessage = CharaLinkUtil.GetLinkBonusRaceText(cCharaMasterData);

		SetLinkEffectSkill(cCharaMasterData, nLinkPoint);
	}

	public void SetLinkEffectSkill(MasterDataParamChara cCharaMasterData, uint nLinkPoint)
	{
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
			LinkSkillName = cSkillParamLinkActive.skill_link_name;

			// 説明文
			LinkSkillMessage = cSkillParamLinkActive.skill_link_detail;

			//発動率
			float fSkillOdds = CharaLinkUtil.GetLinkSkillOdds(cSkillParamLinkActive, (int)nLinkPoint) * 0.01f;
			LinkSkillRate = string.Format(GameTextUtil.GetText("unit_linkstatus2"), fSkillOdds.ToString("F1"));
		}
		else
		{
			// スキル名
			LinkSkillName = EmptyStr;
			// 説明文
			LinkSkillMessage = EmptyStr;
			// 発動率
			LinkSkillRate = "";
		}

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
			LinkPassiveName = cSkillParamLinkPassive.name;

			// 説明文
			LinkPassiveMessage = cSkillParamLinkPassive.detail;
		}
		else
		{
			// スキル名
			LinkPassiveName = EmptyStr;

			// 説明文
			LinkPassiveMessage = EmptyStr;
		}
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

		LinkBonusImage = ResourceManager.Instance.Load(linkBonusPath, ResourceType.Common);
		RaceBonusImage = ResourceManager.Instance.Load(raceBonusPath, ResourceType.Common);
		LinkSkillImage = ResourceManager.Instance.Load(linkSkillPath, ResourceType.Common);
		LinkPassiveImage = ResourceManager.Instance.Load(linkPassivePath, ResourceType.Common);

		LinkBonusBG = ResourceManager.Instance.Load(linkBonusBGPath, ResourceType.Common);
		RaceBonusBG = ResourceManager.Instance.Load(raceBonusBGPath, ResourceType.Common);
		LinkSkillBG = ResourceManager.Instance.Load(linkSkillBGPath, ResourceType.Common);
		LinkPassiveBG = ResourceManager.Instance.Load(linkPassiveBGPath, ResourceType.Common);
	}
}
