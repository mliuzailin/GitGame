using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class MainMenuDebugForceFriendUnit : MainMenuSeq
{
    private DebugForceFriendUnit m_ForceFriendUnit = null;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //ページ初期化処理
        if (m_ForceFriendUnit == null)
        {
            m_ForceFriendUnit = m_CanvasObj.GetComponentInChildren<DebugForceFriendUnit>();
            m_ForceFriendUnit.DidSetParam = SetParam;
            m_ForceFriendUnit.DidResetBase = OnBaseReset;
            m_ForceFriendUnit.DidResetLink = OnLinkReset;
            m_ForceFriendUnit.DidForce = OnForce;
        }

        m_ForceFriendUnit.IsForce = ForceFriendUnitParam.Enable;
        m_ForceFriendUnit.setupBase(ForceFriendUnitParam.BaseUnit);
        m_ForceFriendUnit.setupLink(ForceFriendUnitParam.LinkUnit);
    }

    private void SetParam(DebugForceFriendUnit.InputType _type, string _data)
    {
        string value = "";
        string value2 = "";
        switch (_type)
        {
            case DebugForceFriendUnit.InputType.BaseID:
                value = setUnitId(ref ForceFriendUnitParam.BaseUnit, _data);
                m_ForceFriendUnit.resetBaseParam(false);
                value2 = setUnitLevel(ref ForceFriendUnitParam.BaseUnit, "1");
                m_ForceFriendUnit.setInputText(DebugForceFriendUnit.InputType.BaseLevel, value2);
                break;
            case DebugForceFriendUnit.InputType.BaseLevel:
                value = setUnitLevel(ref ForceFriendUnitParam.BaseUnit, _data);
                break;
            case DebugForceFriendUnit.InputType.BaseAtkPlus:
                value = setUnitAtlPlus(ref ForceFriendUnitParam.BaseUnit, _data);
                break;
            case DebugForceFriendUnit.InputType.BaseHpPlus:
                value = setUnitHpPlus(ref ForceFriendUnitParam.BaseUnit, _data);
                break;
            case DebugForceFriendUnit.InputType.BaseSkillLevel:
                value = setUnitSkillLevel(ref ForceFriendUnitParam.BaseUnit, _data);
                break;
            case DebugForceFriendUnit.InputType.BaseLimitOver:
                value = setUnitLimitOver(ref ForceFriendUnitParam.BaseUnit, _data);
                break;
            case DebugForceFriendUnit.InputType.LinkID:
                value = setUnitId(ref ForceFriendUnitParam.LinkUnit, _data);
                m_ForceFriendUnit.resetLinkParam(false);
                value2 = setUnitLevel(ref ForceFriendUnitParam.LinkUnit, "1");
                m_ForceFriendUnit.setInputText(DebugForceFriendUnit.InputType.LinkLevel, value2);
                break;
            case DebugForceFriendUnit.InputType.LinkLevel:
                value = setUnitLevel(ref ForceFriendUnitParam.LinkUnit, _data);
                break;
            case DebugForceFriendUnit.InputType.LinkAtkPlus:
                value = setUnitAtlPlus(ref ForceFriendUnitParam.LinkUnit, _data);
                break;
            case DebugForceFriendUnit.InputType.LinkHpPlus:
                value = setUnitHpPlus(ref ForceFriendUnitParam.LinkUnit, _data);
                break;
            case DebugForceFriendUnit.InputType.LinkPoint:
                value = setUnitLinkPoint(ref ForceFriendUnitParam.LinkUnit, _data);
                break;
            case DebugForceFriendUnit.InputType.LinkLimitOver:
                value = setUnitLimitOver(ref ForceFriendUnitParam.LinkUnit, _data);
                break;
        }
        m_ForceFriendUnit.setInputText(_type, value);
        setupLink();
    }

    private string setUnitId(ref PacketStructUnit _unit, string _id)
    {
        _unit = null;
        int unit_id = _id.ToInt(0);
        if (unit_id == 0) return "";
        MasterDataParamChara master = m_ForceFriendUnit.SearchUnit(unit_id);
        if (master == null) return "";

        _unit = new PacketStructUnit();
        _unit.id = (uint)master.fix_id;
        return master.draw_id.ToString();
    }

    private string setUnitLevel(ref PacketStructUnit _unit, string _level)
    {
        if (_unit == null) return "";
        int level = _level.ToInt(0);
        MasterDataParamChara master = MasterFinder<MasterDataParamChara>.Instance.Find((int)_unit.id);
        if (master == null) return "";

        if (level < 1) level = 1;
        if (level > master.level_max) level = master.level_max;
        _unit.level = (uint)level;
        return level.ToString();
    }

    private string setUnitAtlPlus(ref PacketStructUnit _unit, string _atkPlus)
    {
        if (_unit == null) return "";

        int atkPlus = _atkPlus.ToInt(0);
        if (atkPlus > 99) atkPlus = 99;
        _unit.add_pow = (uint)atkPlus;

        return atkPlus.ToString();
    }

    private string setUnitHpPlus(ref PacketStructUnit _unit, string _hpPlus)
    {
        if (_unit == null) return "";

        int hpPlus = _hpPlus.ToInt(0);
        if (hpPlus > 99) hpPlus = 99;
        _unit.add_hp = (uint)hpPlus;

        return hpPlus.ToString();
    }

    private string setUnitSkillLevel(ref PacketStructUnit _unit, string _skillLevel)
    {
        if (_unit == null) return "";
        MasterDataParamChara master = MasterFinder<MasterDataParamChara>.Instance.Find((int)_unit.id);
        if (master == null) return "";
        if (master.skill_limitbreak == 0) return "";
        MasterDataSkillLimitBreak skillMaster = MasterFinder<MasterDataSkillLimitBreak>.Instance.Find((int)master.skill_limitbreak);
        if (skillMaster == null) return "";

        int skillLevel = _skillLevel.ToInt(0);
        if (skillLevel > skillMaster.level_max) skillLevel = skillMaster.level_max;
        _unit.limitbreak_lv = (uint)skillLevel;
        return skillLevel.ToString();
    }

    private string setUnitLimitOver(ref PacketStructUnit _unit, string _limitOver)
    {
        if (_unit == null) return "";
        MasterDataParamChara master = MasterFinder<MasterDataParamChara>.Instance.Find((int)_unit.id);
        if (master == null) return "";
        int nLimitOverMaxLevel = (int)CharaLimitOver.GetParam(0, master.limit_over_type, (int)CharaLimitOver.EGET.ePARAM_LIMITOVER_MAX);

        int limitOver = _limitOver.ToInt(0);
        if (limitOver > nLimitOverMaxLevel) limitOver = nLimitOverMaxLevel;
        _unit.limitover_lv = (uint)limitOver;

        return limitOver.ToString();
    }

    private string setUnitLinkPoint(ref PacketStructUnit _unit, string _linkPoint)
    {
        if (_unit == null) return "";

        int linkPoint = _linkPoint.ToInt(0);
        if (linkPoint > CharaLinkUtil.LINK_POINT_MAX) linkPoint = CharaLinkUtil.LINK_POINT_MAX;
        _unit.link_point = (uint)linkPoint;

        return linkPoint.ToString();
    }

    private void setupLink()
    {
        if (ForceFriendUnitParam.BaseUnit != null &&
            ForceFriendUnitParam.LinkUnit != null)
        {
            ForceFriendUnitParam.BaseUnit.link_info = (int)CHARALINK_TYPE.CHARALINK_TYPE_BASE;
            ForceFriendUnitParam.LinkUnit.link_info = (int)CHARALINK_TYPE.CHARALINK_TYPE_LINK;

            //リンクポイントは親をみるので親に設定
            ForceFriendUnitParam.BaseUnit.link_point = ForceFriendUnitParam.LinkUnit.link_point;
        }
        else if (ForceFriendUnitParam.BaseUnit != null)
        {
            ForceFriendUnitParam.BaseUnit.link_info = (int)CHARALINK_TYPE.CHARALINK_TYPE_NONE;
        }
        else if (ForceFriendUnitParam.LinkUnit != null)
        {
            ForceFriendUnitParam.LinkUnit.link_info = (int)CHARALINK_TYPE.CHARALINK_TYPE_LINK;
        }
    }

    private void OnBaseReset()
    {
        m_ForceFriendUnit.resetBaseParam();
        ForceFriendUnitParam.BaseUnit = null;
        setupLink();
    }

    private void OnLinkReset()
    {
        m_ForceFriendUnit.resetLinkParam();
        ForceFriendUnitParam.LinkUnit = null;
        setupLink();
    }

    private void OnForce()
    {
        ForceFriendUnitParam.Enable = m_ForceFriendUnit.IsForce;
    }
}
