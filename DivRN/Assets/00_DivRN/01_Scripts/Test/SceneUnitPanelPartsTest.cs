using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class SceneUnitPanelPartsTest : Scene<SceneUnitPanelPartsTest>
{
    public bool IsViewNamePanel = false;
	public UnitNamePanel unitNamePanel = null;
    public bool IsViewParamPanel = false;
    public UnitParamPanel unitParamPanel = null;
    public bool IsViewStoryPanel = false;
    public UnitStoryPanel unitStoryPanel = null;
    public bool IsViewSkillPanel = false;
    public UnitSkillPanel unitSkillPanel = null;
    public bool IsViewLinkPanel = false;
    public UnitLinkPanel unitLinkPanel = null;
    public bool IsViewMaterialPanel2 = false;
    public UnitMaterialPanel unitMaterialPanel = null;


    public uint UnitId = 0;

	private bool mbInit = false;

	protected override void Start()
	{
		base.Start();
	}

	// Update is called once per frame
	void Update () {
		if (SceneCommon.Instance.IsLoadingScene)
		{
			return;
		}
	
		if( !mbInit &&
			UserDataAdmin.Instance.m_StructPlayer != null)
		{
			PacketStructUnit _unit = UserDataAdmin.Instance.m_StructPlayer.unit_list[UnitId];
			PacketStructUnit _linkunit = UserDataAdmin.Instance.SearchChara(_unit.link_unique_id);
			MasterDataParamChara _master = MasterFinder<MasterDataParamChara>.Instance.Find((int)_unit.id);
            unitNamePanel.setup(_master);
            if (!IsViewNamePanel) UnityUtil.SetObjectEnabledOnce(unitNamePanel.gameObject, false);

            unitParamPanel.setupUnit(_unit, _linkunit);
            unitParamPanel.IsViewExp = true;
            if (!IsViewParamPanel) UnityUtil.SetObjectEnabledOnce(unitParamPanel.gameObject, false);

            unitStoryPanel.setup(_unit.id);
            if (!IsViewStoryPanel) UnityUtil.SetObjectEnabledOnce(unitStoryPanel.gameObject, false);

            unitSkillPanel.AddLeaderSkill(_master.skill_leader);
            unitSkillPanel.AddLimitBreakSkill(_master.skill_limitbreak, (int)_unit.limitbreak_lv);
            if (_master.skill_active0 != 0) unitSkillPanel.AddActiveSkill(_master.skill_active0);
            if (_master.skill_active1 != 0) unitSkillPanel.AddActiveSkill(_master.skill_active1);
            if (_master.skill_passive != 0) unitSkillPanel.AddPassiveSkill(_master.skill_passive);
            if (!IsViewSkillPanel)UnityUtil.SetObjectEnabledOnce(unitSkillPanel.gameObject, false);

            unitLinkPanel.setupUnit(_unit, _linkunit, UnitLinkPanel.LinkParamType.LinkEffect);
            if (!IsViewLinkPanel) UnityUtil.SetObjectEnabledOnce(unitLinkPanel.gameObject, false);

            unitMaterialPanel.setIconSize(80);
            unitMaterialPanel.addItem(0, 10);
            unitMaterialPanel.addItem(0, 20);
            unitMaterialPanel.addItem(0, 30);
            if (!IsViewMaterialPanel2) UnityUtil.SetObjectEnabledOnce(unitMaterialPanel.gameObject, false);

            mbInit = true;

        }
    }
}
