using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;

public class DialogUnitInfo : M4uContextMonoBehaviour
{
	public ScrollRect LinkScrollRect = null;
	public RectTransform ScrollViewRect = null;
	public RectTransform LeaderSkillRect = null;
	public RectTransform LinkUnitInfoRect = null;

	private float ScrollBarViewHeight;

	public class BaseUnitInfo
    {
        M4uProperty<string> name = new M4uProperty<string>();
        public string Name { get { return name.Value; } set { name.Value = value; } }

        M4uProperty<Sprite> icon = new M4uProperty<Sprite>();
        public Sprite Icon { get { return icon.Value; } set { icon.Value = value; } }

        M4uProperty<int> rarity = new M4uProperty<int>();
        public int Rarity { get { return rarity.Value; } set { rarity.Value = value; } }

        M4uProperty<Sprite> mainRace = new M4uProperty<Sprite>();
        public Sprite MainRace { get { return mainRace.Value; } set { mainRace.Value = value; } }

        M4uProperty<Sprite> subRace = new M4uProperty<Sprite>();
        public Sprite SubRace { get { return subRace.Value; } set { subRace.Value = value; } }

        M4uProperty<bool> isViewSubRace = new M4uProperty<bool>();
        public bool IsViewSubRace { get { return isViewSubRace.Value; } set { isViewSubRace.Value = value; } }

        M4uProperty<Sprite> element = new M4uProperty<Sprite>();
        public Sprite Element { get { return element.Value; } set { element.Value = value; } }

        M4uProperty<Color> elementColor = new M4uProperty<Color>();
        public Color ElementColor { get { return elementColor.Value; } set { elementColor.Value = value; } }

        M4uProperty<string> lv = new M4uProperty<string>();
        public string Lv { get { return lv.Value; } set { lv.Value = value; } }

        M4uProperty<string> slv = new M4uProperty<string>();
        public string Slv { get { return slv.Value; } set { slv.Value = value; } }

        M4uProperty<string> hp = new M4uProperty<string>();
        public string Hp { get { return hp.Value; } set { hp.Value = value; } }

        M4uProperty<string> atk = new M4uProperty<string>();
        public string Atk { get { return atk.Value; } set { atk.Value = value; } }

        M4uProperty<string> charm = new M4uProperty<string>();
        public string Charm { get { return charm.Value; } set { charm.Value = value; } }

        M4uProperty<List<UnitSkillContext>> leaderSkill = new M4uProperty<List<UnitSkillContext>>(new List<UnitSkillContext>());
        public List<UnitSkillContext> LeaderSkill { get { return leaderSkill.Value; } set { leaderSkill.Value = value; } }

		List<GameObject> leaderSkillList = new List<GameObject>();
		public List<GameObject> LeaderSkillList { get { return leaderSkillList; } set { leaderSkillList = value; } }

	}
	public class LinkUnitInfo
    {
        M4uProperty<bool> isActiveLinkUnit = new M4uProperty<bool>();
        public bool IsActiveLinkUnit { get { return isActiveLinkUnit.Value; } set { isActiveLinkUnit.Value = value; } }

        M4uProperty<string> name = new M4uProperty<string>();
        public string Name { get { return name.Value; } set { name.Value = value; } }

        M4uProperty<Sprite> icon = new M4uProperty<Sprite>();
        public Sprite Icon { get { return icon.Value; } set { icon.Value = value; } }

        M4uProperty<string> lv = new M4uProperty<string>();
        public string Lv { get { return lv.Value; } set { lv.Value = value; } }

        M4uProperty<string> linkSkillName = new M4uProperty<string>();
        public string LinkSkillName { get { return linkSkillName.Value; } set { linkSkillName.Value = value; } }

        M4uProperty<string> linkSkillMessage = new M4uProperty<string>();
        public string LinkSkillMessage { get { return linkSkillMessage.Value; } set { linkSkillMessage.Value = value; } }

        M4uProperty<string> linkSkillRate = new M4uProperty<string>();
        public string LinkSkillRate { get { return linkSkillRate.Value; } set { linkSkillRate.Value = value; } }

        M4uProperty<string> linkPassiveName = new M4uProperty<string>();
        public string LinkPassiveName { get { return linkPassiveName.Value; } set { linkPassiveName.Value = value; } }

        M4uProperty<string> linkPassiveMessage = new M4uProperty<string>();
        public string LinkPassiveMessage { get { return linkPassiveMessage.Value; } set { linkPassiveMessage.Value = value; } }

        M4uProperty<string> notMessage = new M4uProperty<string>();
        public string NotMessage { get { return notMessage.Value; } set { notMessage.Value = value; } }
    }

    M4uProperty<bool> isViewScroll = new M4uProperty<bool>();
    public bool IsViewScroll { get { return isViewScroll.Value; } set { isViewScroll.Value = value; } }

    M4uProperty<float> viewScrollAlpha = new M4uProperty<float>();
    public float ViewScrollAlpha { get { return viewScrollAlpha.Value; } set { viewScrollAlpha.Value = value; } }

    M4uProperty<List<UnitSkillContext>> skillList = new M4uProperty<List<UnitSkillContext>>();
    public List<UnitSkillContext> SkillList { get { return skillList.Value; } set { skillList.Value = value; } }

    public BaseUnitInfo baseUnitInfo = new BaseUnitInfo();
    public LinkUnitInfo linkUnitInfo = new LinkUnitInfo();
    private bool bSetup = false;
    private CharaOnce m_baseChara = null;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        IsViewScroll = true;
        ViewScrollAlpha = 0;
        SkillList = new List<UnitSkillContext>();
    }

    // Use this for initialization
    void Start()
    {
        if (!bSetup) setSample();
    }

    private void setSample()
    {
        baseUnitInfo.Name = "テストユニット";
        baseUnitInfo.MainRace = null;
        baseUnitInfo.Lv = "99";
        baseUnitInfo.Slv = "5";
        baseUnitInfo.Hp = "2580";
        baseUnitInfo.Atk = "1860";
        baseUnitInfo.Charm = "0.0";

        linkUnitInfo.IsActiveLinkUnit = false;
        linkUnitInfo.Name = "";
        linkUnitInfo.Lv = "-";
        linkUnitInfo.LinkSkillName = "-";
        linkUnitInfo.LinkSkillMessage = "";
        linkUnitInfo.LinkSkillRate = "";
        linkUnitInfo.LinkPassiveName = "-";
        linkUnitInfo.LinkPassiveMessage = "";
        linkUnitInfo.NotMessage = "";

		baseUnitInfo.LeaderSkill.Clear();
		UnitSkillContext leder = new UnitSkillContext();
		leder.setupLeaderSkill(0);
		baseUnitInfo.LeaderSkill.Add(leder);
	}

// Update is called once per frame
void Update()
    {

    }

    public void setupChara(PacketStructUnit _mainUnit, PacketStructUnit _subUnit, bool dispCharm)
    {
        m_baseChara = new CharaOnce();

        if (_mainUnit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE &&
            _subUnit != null)
        {
            m_baseChara.CharaSetupFromID(
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
            m_baseChara.CharaSetupFromID(
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
        if (m_baseChara.m_CharaMasterDataParam != null)
        {

            UnitIconImageProvider.Instance.Get(
                _mainUnit.id,
                sprite =>
                {
                    baseUnitInfo.Icon = sprite;
                });

			baseUnitInfo.LeaderSkill.Clear();
			UnitSkillContext leder = new UnitSkillContext();
			if (m_baseChara.m_CharaMasterDataParam.skill_leader != 0)
			{
				leder.setupLeaderSkill(m_baseChara.m_CharaMasterDataParam.skill_leader);
			}
			else
			{
				leder.setupLeaderSkill(0);
			}
			baseUnitInfo.LeaderSkill.Add(leder);
		}

		/**
         * サブユニット情報
         */
		if (_subUnit != null)
        {
            linkUnitInfo.IsActiveLinkUnit = true;
            MasterDataParamChara _subMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)_subUnit.id);
            if (_subMaster != null)
            {

                linkUnitInfo.Name = _subMaster.name;
                linkUnitInfo.Lv = string.Format("{0}/{1}", _subUnit.level, _subMaster.level_max);
                UnitIconImageProvider.Instance.Get(
                    _subUnit.id,
                    sprite =>
                    {
                        linkUnitInfo.Icon = sprite;
                    });
                SkillList.Clear();
                UnitSkillContext _newSkill = new UnitSkillContext();
                _newSkill.setupLinkSkill(_subMaster, 0);
                _newSkill.setLinkTitleImage("LINK SKILL", "skill_name_bg3");
                SkillList.Add(_newSkill);
                _newSkill = new UnitSkillContext();
                _newSkill.setupLinkPassive(_subMaster);
                _newSkill.setLinkTitleImage("LINK PASSIVE", "skill_name_bg4");
                SkillList.Add(_newSkill);
            }
            else
            {
#if BUILD_TYPE_DEBUG
                linkUnitInfo.Name = "<color=#FF0000>No MasterDataParamChara fix_id:" + _subUnit.id + "</color>";
#endif
            }
        }
        else
        {
            linkUnitInfo.IsActiveLinkUnit = false;
            linkUnitInfo.NotMessage = GameTextUtil.GetText("unit_linkstatus5");
        }

        bSetup = true;

    }

    public void setViewScroll()
    {
        IsViewScroll = false;
        StartCoroutine(WaitScrollContent());
    }

    IEnumerator WaitScrollContent()
    {
		while (m_baseChara == null)
		{
			yield return null;
		}
		yield return null;
        IsViewScroll = true;

		while (LeaderSkillRect.sizeDelta.y == 0)
		{
			yield return null;
		}
		ScrollBarViewHeight = ScrollViewRect.sizeDelta.y;
		float LeaderSkillHeight = LeaderSkillRect.sizeDelta.y;
		float LinkUnitInfoHeight = LinkUnitInfoRect.sizeDelta.y;
		while(LinkScrollRect.content.sizeDelta.y < (LeaderSkillHeight + LinkUnitInfoHeight))
		{
			IsViewScroll = false;
			yield return null;
			IsViewScroll = true;
			yield return null;
		}
		if ((LeaderSkillHeight + LinkUnitInfoHeight) > ScrollBarViewHeight)
			{
				while (LinkScrollRect.verticalScrollbar.IsActive() == false)
            {
				yield return null;
            }
		}
		/**
         * メインユニット情報
         */
		if (m_baseChara.m_CharaMasterDataParam != null)
        {
			baseUnitInfo.Name = m_baseChara.m_CharaMasterDataParam.name;
            baseUnitInfo.Rarity = (int)m_baseChara.m_CharaMasterDataParam.rare + 1;
            baseUnitInfo.MainRace = MainMenuUtil.GetTextKindSprite(m_baseChara.m_CharaMasterDataParam.kind, false);
            baseUnitInfo.IsViewSubRace = false;
            if (m_baseChara.m_CharaMasterDataParam.sub_kind != MasterDataDefineLabel.KindType.NONE)
            {
                baseUnitInfo.SubRace = MainMenuUtil.GetTextKindSprite(m_baseChara.m_CharaMasterDataParam.sub_kind, false);
                baseUnitInfo.IsViewSubRace = true;
            }
            baseUnitInfo.Element = MainMenuUtil.GetTextElementSprite(m_baseChara.m_CharaMasterDataParam.element);
            baseUnitInfo.ElementColor = ColorUtil.GetElementLabelColor(m_baseChara.m_CharaMasterDataParam.element);

            string levelFormat = GameTextUtil.GetText("unit_status17");
            baseUnitInfo.Lv = string.Format(levelFormat, m_baseChara.m_CharaLevel, m_baseChara.m_CharaMasterDataParam.level_max);
            MasterDataSkillLimitBreak lb_skill = MasterFinder<MasterDataSkillLimitBreak>.Instance.Find((int)m_baseChara.m_CharaMasterDataParam.skill_limitbreak);
            baseUnitInfo.Slv = "-";
            if (lb_skill != null)
            {
                baseUnitInfo.Slv = string.Format("{0}/{1}", m_baseChara.m_CharaLBSLv + 1, lb_skill.level_max + 1);
            }

            if (m_baseChara.m_CharaPlusHP != 0)
            {
                baseUnitInfo.Hp = string.Format(GameTextUtil.GetText("unit_status19"), m_baseChara.m_CharaHP, m_baseChara.m_CharaPlusHP);
            }
            else
            {
                baseUnitInfo.Hp = m_baseChara.m_CharaHP.ToString();
            }

            if (m_baseChara.m_CharaPlusPow != 0)
            {
                baseUnitInfo.Atk = string.Format(GameTextUtil.GetText("unit_status19"), m_baseChara.m_CharaPow, m_baseChara.m_CharaPlusPow);
            }
            else
            {
                baseUnitInfo.Atk = m_baseChara.m_CharaPow.ToString();
            }

            baseUnitInfo.Charm = string.Format(GameTextUtil.GetText("kyouka_text1"), m_baseChara.m_CharaCharm.ToString("F1"));
        }
        else
        {
#if BUILD_TYPE_DEBUG
            baseUnitInfo.Name = "<color=#FF0000>No MasterDataParamChara fix_id:" + m_baseChara.m_CharaMasterDataParam.fix_id + "</color>";
#endif
        }
        ViewScrollAlpha = 1;
	}
}
