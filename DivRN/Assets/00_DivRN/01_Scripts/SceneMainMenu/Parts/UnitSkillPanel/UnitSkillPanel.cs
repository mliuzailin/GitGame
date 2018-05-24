using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;

public class UnitSkillPanel : MenuPartsBase
{
    public const int MAX_SKILL_LV = -1;

    M4uProperty<List<UnitSkillContext>> skillList = new M4uProperty<List<UnitSkillContext>>();
    public List<UnitSkillContext> SkillList { get { return skillList.Value; } set { skillList.Value = value; } }

	List<GameObject> skillItemList = new List<GameObject>();
	public List<GameObject> SkillItemList { get { return skillItemList; } set { skillItemList = value; } }

	M4uProperty<bool> isViewBG = new M4uProperty<bool>();
    public bool IsViewBG { get { return isViewBG.Value; } set { isViewBG.Value = value; } }

	private UnitSkillLinkListItem unitSkillLinkListItem = null;

	private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;

        SkillList = new List<UnitSkillContext>();
        IsViewBG = true;
    }

    public void AllClear()
    {
        SkillList.Clear();
		SkillItemList.Clear();
	}

    public void AddLeaderSkill(uint _id)
    {
        UnitSkillContext _newSkill = new UnitSkillContext();
        _newSkill.setupLeaderSkill(_id);
        SkillList.Add(_newSkill);
    }

    public void AddLimitBreakSkill(uint _id, int level)
    {
        UnitSkillContext _newSkill = new UnitSkillContext();
        _newSkill.setupLimitBreakSkill(_id, (uint)level);
        SkillList.Add(_newSkill);
    }
    public void AddActiveSkill(uint _id)
    {
        UnitSkillContext _newSkill = new UnitSkillContext();
        _newSkill.setupActiveSkill(_id);
        SkillList.Add(_newSkill);
    }
    public void AddPassiveSkill(uint _id)
    {
        UnitSkillContext _newSkill = new UnitSkillContext();
        _newSkill.setupPassiveSkill(_id);
        SkillList.Add(_newSkill);
	}

	public void AddLinkBonus(CharaOnce cCharaOnce, MasterDataParamChara cCharaMasterData)
	{
		UnitSkillContext _newSkill = new UnitSkillContext();
		_newSkill.setupLinkEffectInfo(cCharaOnce,cCharaMasterData,false);
		_newSkill.setLinkTitleImage("linkubo-nasu_gray", "skill_name_bg0");
		SkillList.Add(_newSkill);
	}

	public void AddRaceBonus(MasterDataParamChara cCharaMasterData)
	{
		UnitSkillContext _newSkill = new UnitSkillContext();
		_newSkill.setupRaceBonus(cCharaMasterData);
		_newSkill.setLinkTitleImage("RACE BONUS_gray", "skill_name_bg0");
		SkillList.Add(_newSkill);
	}

	public void AddLinkSkill(MasterDataParamChara cCharaMasterData, uint nLinkPoint)
	{
		UnitSkillContext _newSkill = new UnitSkillContext();
		_newSkill.setupLinkSkill(cCharaMasterData, nLinkPoint);
		_newSkill.setLinkTitleImage("LINK SKILL_gray", "skill_name_bg0");
		SkillList.Add(_newSkill);
	}

	public void AddLinkPassive(MasterDataParamChara cCharaMasterData)
	{
		UnitSkillContext _newSkill = new UnitSkillContext();
		_newSkill.setupLinkPassive(cCharaMasterData);
		_newSkill.setLinkTitleImage("LINK PASSIVE_gray", "skill_name_bg0");
		SkillList.Add(_newSkill);
	}

	public void setupChara(uint _unit_id, PacketStructUnit _mainUnit)
	{
		MasterDataParamChara _masterMain = MasterFinder<MasterDataParamChara>.Instance.Find((int)_unit_id);
		if (_masterMain == null)
		{
			return;
		}

		CharaOnce m_CharaParam = new CharaOnce();
		if (_mainUnit == null)
		{
			m_CharaParam.CharaSetupFromID(
				_unit_id,
				(int)1,
				(int)0,
				(int)0,
				(int)0,
				(int)0,
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

		if (_masterMain.link_enable == MasterDataDefineLabel.BoolType.ENABLE)
		{
			AddLinkBonus(m_CharaParam, _masterMain);
			AddRaceBonus(_masterMain);
			AddLinkSkill(_masterMain, 0);
			AddLinkPassive(_masterMain);
		}
	}
}
