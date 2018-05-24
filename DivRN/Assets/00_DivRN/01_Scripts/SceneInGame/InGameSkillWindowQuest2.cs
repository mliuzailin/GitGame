using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerDataDefine;
using M4u;

//============================================================================
//	class
//============================================================================
//----------------------------------------------------------------------------
/*!
	@class		InGameSkillWindowQuest2
	@brief		スキルウィンドウ(新探索版)
*/
//----------------------------------------------------------------------------
public class InGameSkillWindowQuest2 : M4uContextMonoBehaviour
{
	enum eSkillWindowStep : int
	{
		NONE = 0,
		OPEN_WAIT_HERO,
		OPEN_WAIT,
		OPEN,
		CLOSE,
	};

	enum tagSpriteIndex : int
	{
		UNSELECT_SKILL = 0,
		UNSELECT_LINK,
		UNSELECT_STATUS,
		SELECT_SKILL,
		SELECT_LINK,
		SELECT_STATUS,
	};

	M4uProperty<float> window_y = new M4uProperty<float>();
	public float Window_y { get { return window_y.Value; } set { window_y.Value = value; } }

	M4uProperty<string> label_text = new M4uProperty<string>();
	public string Label_text { get { return label_text.Value; } set { label_text.Value = value; } }

	M4uProperty<string> closeText = new M4uProperty<string>();
	public string CloseText { get { return closeText.Value; } set { closeText.Value = value; } }

	public GameObject windowPanelRoot = null;
	public UnitNamePanel unitNamePanel = null;
	public UnitSkillPanel unitSkillPanel = null;
	public UnitLinkPanel unitLinkPanel = null;
	public UnitParamPanel unitParamPanel = null;
	public UnitAilmentPanel unitAilmentPanel = null;
	public HeroDetailPanel heroDetailPanel = null;
	public Sprite[] m_TagSprite = null;
	public InGameSkillWindowParty skillWindowParty = null;
	public InGameSkillWindowTag skillWindowTag = null;

	private GlobalDefine.PartyCharaIndex m_CharaIdx = GlobalDefine.PartyCharaIndex.ERROR;               //!< キャラインデックス
	private uint m_CharaId = 0;
	private bool m_Activate = false;            //!< 発動ボタンタッチ
	private eSkillWindowStep m_SkillWindowStep = eSkillWindowStep.NONE;
	private bool m_Open = false;
	private bool m_Close = false;
	public bool isClose { get { return m_Close; } }

	private Color SelectTagColor = new Color(0.33f, 0.75f, 0.88f);

	//------------------------------------------------------------------------
	/*!
		@brief		発動ボタンが押されたかどうか
		@retval		bool		[押された/押されてない]
	*/
	//------------------------------------------------------------------------
	public bool isActivate
	{
		get
		{
			return m_Activate;
		}

		set
		{
			m_Activate = value;
		}
	}

	void Awake()
	{
		gameObject.GetComponent<M4uContextRoot>().Context = this;
		unitNamePanel = setupPrefab<UnitNamePanel>("Prefab/UnitNamePanel/UnitNamePanel", windowPanelRoot);
		unitParamPanel = setupPrefab<UnitParamPanel>("Prefab/UnitParamPanel/UnitParamPanel", windowPanelRoot);
		Destroy(unitParamPanel.gameObject.GetComponent<ContentSizeFitter>());
		Destroy(unitParamPanel.gameObject.GetComponent<LayoutElement>());
		unitAilmentPanel = setupPrefab<UnitAilmentPanel>("Prefab/InGame/InGameUI/Menu/UnitAilmentPanel", windowPanelRoot);
		unitSkillPanel = setupPrefabAddLayoutElement<UnitSkillPanel>("Prefab/UnitSkillPanel/UnitSkillPanel", windowPanelRoot, 600, 520, 0, 0);
		unitLinkPanel = setupPrefabAddLayoutElement<UnitLinkPanel>("Prefab/UnitLinkPanel/UnitLinkPanel", windowPanelRoot, 600, 520, 0, 0);
		heroDetailPanel = setupPrefab<HeroDetailPanel>("Prefab/InGame/InGameUI/Menu/HeroDetailPanel", windowPanelRoot);
		skillWindowTag = setupPrefab<InGameSkillWindowTag>("Prefab/InGame/InGameUI/Menu/InGameSkillWindowTag", windowPanelRoot);
		skillWindowParty = setupPrefab<InGameSkillWindowParty>("Prefab/InGame/InGameUI/Menu/InGameSkillWindowParty", windowPanelRoot);
		CloseText = GameTextUtil.GetText("common_button6");
		skillWindowTag.m_StatusClickAction = OnStatus;
		skillWindowTag.m_LinkClickAction = OnLink;
		skillWindowTag.m_SkillClickAction = OnSkill;

		skillWindowTag.Tab_active = true;
	}


	//------------------------------------------------------------------------
	/*!
		@brief		更新前処理
	*/
	//------------------------------------------------------------------------
	protected void Start()
	{
		Window_y = 960;
		m_Close = false;
	}

	//------------------------------------------------------------------------
	/*!
		@brief		更新処理
	*/
	//------------------------------------------------------------------------
	void Update()
	{
		switch (m_SkillWindowStep)
		{
			case eSkillWindowStep.OPEN_WAIT_HERO:
				{
					if (Time.deltaTime < 0.1f)
					{
						initHero();
						skillWindowParty.setup(GlobalDefine.PartyCharaIndex.HERO, changePartyUnit);
						skillWindowParty.m_ClickAction = changePartyUnit;
						m_SkillWindowStep = eSkillWindowStep.OPEN_WAIT;
					}
				}
				break;
			case eSkillWindowStep.OPEN_WAIT:
				{
					if (Time.deltaTime < 0.1f)
					{
						m_SkillWindowStep = eSkillWindowStep.OPEN;
					}
				}
				break;
			case eSkillWindowStep.OPEN:
				{
					Window_y -= (3840 * Time.deltaTime);
					if (Window_y <= 0)
					{
						Window_y = 0;
						m_Open = true;
						m_SkillWindowStep = eSkillWindowStep.NONE;
					}
				}
				break;
			case eSkillWindowStep.CLOSE:
				{
					Window_y += (3840 * Time.deltaTime);
					if (Window_y >= 960)
					{
						m_SkillWindowStep = eSkillWindowStep.NONE;
						m_Close = true;
					}
				}
				break;
			default:
				break;
		}
	}

	//------------------------------------------------------------------------
	/*!
		@brief		更新処理
	*/
	//------------------------------------------------------------------------
	public void Open(GlobalDefine.PartyCharaIndex charaIdx)
	{
		if (charaIdx == GlobalDefine.PartyCharaIndex.ERROR)
		{
			return;
		}
		SoundUtil.PlaySE(SEID.SE_BATLE_UI_OPEN);

		m_CharaIdx = charaIdx;
		m_CharaId = InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_CharaMasterDataParam.fix_id;

		initPanel();

		setupStatusWindow();

		skillWindowParty.setup(m_CharaIdx,changePartyUnit);
		skillWindowParty.m_ClickAction = changePartyUnit;

		m_SkillWindowStep = eSkillWindowStep.OPEN_WAIT;
	}

	//------------------------------------------------------------------------
	/*!
		@brief		更新処理
	*/
	//------------------------------------------------------------------------
	private void initPanel()
	{
		if (m_CharaIdx == GlobalDefine.PartyCharaIndex.ERROR)
		{
			return;
		}
		PacketStructUnit _unit = null;
		PacketStructUnit _linkunit = null;
		{
			_unit = new PacketStructUnit();
			_unit.id = m_CharaId;
			_unit.level = (uint)InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_CharaLevel;
			_unit.unique_id = 1; // ダミー
			_unit.add_pow = (uint)InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_CharaPlusPow;
			_unit.add_def = (uint)InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_CharaPlusDef;
			_unit.add_hp = (uint)InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_CharaPlusHP;
			_unit.limitbreak_lv = (uint)InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_CharaLBSLv;
			_unit.limitover_lv = (uint)InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_CharaLimitOver;
			_unit.link_info = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE;
			if (InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_LinkParam.m_CharaID != 0)
			{
				_unit.link_info = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE;
				_linkunit = new PacketStructUnit();
				_linkunit.id = InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_LinkParam.m_CharaID;
				_linkunit.level = (uint)InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_LinkParam.m_CharaLv;
				_linkunit.add_pow = (uint)InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_LinkParam.m_CharaPlusPow;
				_linkunit.add_hp = (uint)InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_LinkParam.m_CharaPlusHP;
				_unit.link_point = (uint)InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_LinkParam.m_CharaLinkPoint;
				_linkunit.limitover_lv = (uint)InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_LinkParam.m_CharaLOLevel;
				_linkunit.link_info = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK;
			}
		}
		MasterDataParamChara _master = InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_CharaMasterDataParam;

		// 名前パネル設定
		unitNamePanel.setup(_master);
		unitNamePanel.IsViewPremiumButton = false;

		// スキルパネル設定
		unitSkillPanel.AllClear();
		unitSkillPanel.AddLeaderSkill(_master.skill_leader);
		unitSkillPanel.AddLimitBreakSkill(_master.skill_limitbreak, (int)_unit.limitbreak_lv);
		unitSkillPanel.AddActiveSkill(_master.skill_active0);
		if (_master.skill_passive == 0) unitSkillPanel.AddActiveSkill(_master.skill_active1);
		if (_master.skill_passive != 0) unitSkillPanel.AddPassiveSkill(_master.skill_passive);

		// ステータスパネル設定
		CharaParty party = BattleParam.m_PlayerParty;
		CharaOnce party_unit = party.getPartyMember(m_CharaIdx, CharaParty.CharaCondition.EXIST);
		int pow = (int)((float)party_unit.m_CharaPow * InGameUtil.getCharaAttakPowScale(party_unit, BattleParam.m_PlayerParty.m_Ailments.getAilment(m_CharaIdx)));
		unitParamPanel.setupUnit(_unit, _linkunit, false, party.m_HPCurrent.getValue(m_CharaIdx), party.m_HPMax.getValue(m_CharaIdx), pow);
		unitParamPanel.IsViewExp = false;

		if (_linkunit != null)
		{
			// リンクパネル設定
			unitLinkPanel.setupUnit(_unit, _linkunit, UnitLinkPanel.LinkParamType.Link);
			skillWindowTag.Link_tag_active = true;
		}
		else
		{
			skillWindowTag.Link_tag_active = false;
		}

		// 状態異常パネル設定
		unitAilmentPanel.AllClear();
		unitAilmentPanel.setupCharaAilmentInfo(SceneModeContinuousBattle.Instance.m_PlayerParty.m_Ailments.getAilment(m_CharaIdx));

		UnityUtil.SetObjectEnabledOnce(heroDetailPanel.gameObject, false);

		Label_text = GameTextUtil.GetText("battle_infotext3");
	}

	//------------------------------------------------------------------------
	/*!
		@brief		更新処理
	*/
	//------------------------------------------------------------------------
	private void setupSkillWindow()
	{
		UnityUtil.SetObjectEnabledOnce(unitSkillPanel.gameObject, true);
		UnityUtil.SetObjectEnabledOnce(unitLinkPanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(unitParamPanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(unitAilmentPanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(heroDetailPanel.gameObject, false);

		skillWindowTag.Skill_tag = m_TagSprite[(int)tagSpriteIndex.SELECT_SKILL];
		skillWindowTag.Link_tag = m_TagSprite[(int)tagSpriteIndex.UNSELECT_LINK];
		skillWindowTag.Status_tag = m_TagSprite[(int)tagSpriteIndex.UNSELECT_STATUS];
		skillWindowTag.StatusColor = Color.white;
		skillWindowTag.LinkColor = Color.white;
		skillWindowTag.SkillColor = SelectTagColor;
	}

	//------------------------------------------------------------------------
	/*!
		@brief		更新処理
	*/
	//------------------------------------------------------------------------
	private void setupLinkWindow()
	{
		UnityUtil.SetObjectEnabledOnce(unitSkillPanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(unitLinkPanel.gameObject, true);
		UnityUtil.SetObjectEnabledOnce(unitParamPanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(unitAilmentPanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(heroDetailPanel.gameObject, false);

		skillWindowTag.Skill_tag = m_TagSprite[(int)tagSpriteIndex.UNSELECT_SKILL];
		skillWindowTag.Link_tag = m_TagSprite[(int)tagSpriteIndex.SELECT_LINK];
		skillWindowTag.Status_tag = m_TagSprite[(int)tagSpriteIndex.UNSELECT_STATUS];
		skillWindowTag.StatusColor = Color.white;
		skillWindowTag.LinkColor = SelectTagColor;
		skillWindowTag.SkillColor = Color.white;
	}

	//------------------------------------------------------------------------
	/*!
		@brief		更新処理
	*/
	//------------------------------------------------------------------------
	private void setupStatusWindow()
	{
		UnityUtil.SetObjectEnabledOnce(unitSkillPanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(unitLinkPanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(unitParamPanel.gameObject, true);
		UnityUtil.SetObjectEnabledOnce(unitAilmentPanel.gameObject, true);
		UnityUtil.SetObjectEnabledOnce(heroDetailPanel.gameObject, false);
		unitParamPanel.IsViewExp = false;

		skillWindowTag.Skill_tag = m_TagSprite[(int)tagSpriteIndex.UNSELECT_SKILL];
		skillWindowTag.Link_tag = m_TagSprite[(int)tagSpriteIndex.UNSELECT_LINK];
		skillWindowTag.Status_tag = m_TagSprite[(int)tagSpriteIndex.SELECT_STATUS];
		skillWindowTag.StatusColor = SelectTagColor;
		skillWindowTag.LinkColor = Color.white;
		skillWindowTag.SkillColor = Color.white;
	}

	//------------------------------------------------------------------------
	/*!
		@brief		更新処理
	*/
	//------------------------------------------------------------------------
	public void OnSkill()
	{
		if(UnityUtil.ChkObjectEnabled(unitSkillPanel.gameObject) == false)
		{
			SoundUtil.PlaySE(SEID.SE_MENU_OK);
			setupSkillWindow();
		}
	}

	//------------------------------------------------------------------------
	/*!
		@brief		更新処理
	*/
	//------------------------------------------------------------------------
	public void OnLink()
	{
		if (UnityUtil.ChkObjectEnabled(unitLinkPanel.gameObject) == false)
		{
			SoundUtil.PlaySE(SEID.SE_MENU_OK);
			setupLinkWindow();
		}
	}

	//------------------------------------------------------------------------
	/*!
		@brief		更新処理
	*/
	//------------------------------------------------------------------------
	public void OnStatus()
	{
		if (UnityUtil.ChkObjectEnabled(unitParamPanel.gameObject) == false)
		{
			SoundUtil.PlaySE(SEID.SE_MENU_OK);
			setupStatusWindow();
		}
	}

	//------------------------------------------------------------------------
	/*!
		@brief		更新処理
	*/
	//------------------------------------------------------------------------
	public void OnClose()
	{
		if (m_SkillWindowStep == eSkillWindowStep.NONE)
		{
			SoundUtil.PlaySE(SEID.SE_MENU_RET);
			m_SkillWindowStep = eSkillWindowStep.CLOSE;
		}
	}

	public static T setupPrefab<T>(string _prefabName, GameObject _parent)
	{
		GameObject _tmpObj = Resources.Load(_prefabName) as GameObject;
		if (_tmpObj != null)
		{
			GameObject _newObj = Instantiate(_tmpObj);
			if (_newObj != null)
			{
				_newObj.transform.SetParent(_parent.transform, false);
				return _newObj.GetComponent<T>();
			}
		}
		return default(T);
	}

	public static T setupPrefabAddLayoutElement<T>(string _prefabName, GameObject _parent, float minw, float minh, float prew, float preh)
	{
		GameObject _tmpObj = Resources.Load(_prefabName) as GameObject;
		if (_tmpObj != null)
		{
			GameObject _newObj = Instantiate(_tmpObj);
			if (_newObj != null)
			{
				_newObj.transform.SetParent(_parent.transform, false);
				LayoutElement _element = _newObj.AddComponent<LayoutElement>();
				if (minw > 0) _element.minWidth = minw;
				if (minh > 0) _element.minHeight = minh;
				if (prew > 0) _element.preferredWidth = prew;
				if (preh > 0) _element.preferredHeight = preh;
				return _newObj.GetComponent<T>();
			}
		}
		return default(T);
	}

	public static T setupPrefab<T>(GameObject _obj, GameObject _parent)
	{
		if (_obj != null)
		{
			GameObject _newObj = Instantiate(_obj);
			if (_newObj != null)
			{
				_newObj.transform.SetParent(_parent.transform, false);
				return _newObj.GetComponent<T>();
			}
		}
		return default(T);
	}

	//------------------------------------------------------------------------
	/*!
		@brief		更新処理
	*/
	//------------------------------------------------------------------------
	public void OpenHero()
	{
		SoundUtil.PlaySE(SEID.SE_BATLE_UI_OPEN);

		Open(GlobalDefine.PartyCharaIndex.LEADER);

		m_SkillWindowStep = eSkillWindowStep.OPEN_WAIT_HERO;
	}

	private void initHero()
	{
		skillWindowTag.Tab_active = false;

		UnityUtil.SetObjectEnabledOnce(unitNamePanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(unitSkillPanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(unitLinkPanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(unitParamPanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(unitAilmentPanel.gameObject, false);
		UnityUtil.SetObjectEnabledOnce(heroDetailPanel.gameObject, true);

		Label_text = GameTextUtil.GetText("battle_infotext5");

		PacketStructHero hero = null;
		int unique_id = UserDataAdmin.Instance.m_StructPlayer.current_hero_id;
		for (int i = 0; i < UserDataAdmin.Instance.m_StructHeroList.Length; ++i)
		{
			if (UserDataAdmin.Instance.m_StructHeroList[i].unique_id == unique_id)
			{
				hero = UserDataAdmin.Instance.m_StructHeroList[i];
			}
		}
		heroDetailPanel.SetDetail(hero);
		m_CharaIdx = GlobalDefine.PartyCharaIndex.HERO;
	}

	//------------------------------------------------------------------------
	/*!
		@brief		更新処理
	*/
	//------------------------------------------------------------------------
	public bool isOpened()
	{
		if (m_Open == true) return true;
		return false;
	}

	public void changePartyUnit(GlobalDefine.PartyCharaIndex index)
	{
		if (index == GlobalDefine.PartyCharaIndex.ERROR)
		{
			return;
		}
		if (m_CharaIdx == index)
		{
			return;
		}
		if (index != GlobalDefine.PartyCharaIndex.HERO)
		{
			if (InGamePlayerParty.Instance != null
			&&	InGamePlayerParty.Instance.m_PartyUnit[(int)index].IsSetUnit() == false)
			{
				return;
			}

		}
		SoundUtil.PlaySE(SEID.SE_MENU_OK);
		skillWindowParty.changeUnit(m_CharaIdx, index);
		skillWindowParty.changeHero(index == GlobalDefine.PartyCharaIndex.HERO);
		if (index != GlobalDefine.PartyCharaIndex.HERO)
		{
			if (m_CharaIdx == GlobalDefine.PartyCharaIndex.HERO)
			{
				skillWindowTag.Tab_active = true;
				UnityUtil.SetObjectEnabledOnce(unitNamePanel.gameObject, true);
				setupStatusWindow();
			}
			m_CharaId = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[(int)index].m_CharaMasterDataParam.fix_id;
			m_CharaIdx = index;
			initPanel();
			if (UnityUtil.ChkObjectEnabled(unitLinkPanel.gameObject) == true
			&& skillWindowTag.Link_tag_active == false )
			{
				setupStatusWindow();
			}
		}
		else
		{
			initHero();
		}
	}
}
