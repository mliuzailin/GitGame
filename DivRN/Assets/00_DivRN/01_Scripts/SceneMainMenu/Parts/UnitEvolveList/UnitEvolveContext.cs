using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitEvolveContext : M4uContext
{
    public MasterDataParamChara charaMaster = null;
    public MasterDataLimitOver limitOverMaster = null;
    public MasterDataParamCharaEvol evolMaster = null;
    public Texture2D charaTexture = null;
    public CharaOnce charaOnce = null;

    private M4uProperty<bool> isViewNamePanel = new M4uProperty<bool>();
    public bool IsViewNamePanel { get { return isViewNamePanel.Value; } set { isViewNamePanel.Value = value; } }

    private M4uProperty<bool> isViewMaterialPanel = new M4uProperty<bool>();
    public bool IsViewMaterialPanel { get { return isViewMaterialPanel.Value; } set { isViewMaterialPanel.Value = value; } }

    private M4uProperty<List<MaterialDataContext>> materialList = new M4uProperty<List<MaterialDataContext>>(new List<MaterialDataContext>());
    public List<MaterialDataContext> MaterialList { get { return materialList.Value; } set { materialList.Value = value; } }

    private M4uProperty<string> levelLabel = new M4uProperty<string>();
    public string LevelLabel { get { return levelLabel.Value; } set { levelLabel.Value = value; } }

    private M4uProperty<string> level = new M4uProperty<string>();
    public string Level { get { return level.Value; } set { level.Value = value; } }

    private M4uProperty<string> hpLabel = new M4uProperty<string>();
    public string HpLabel { get { return hpLabel.Value; } set { hpLabel.Value = value; } }

    private M4uProperty<string> hp = new M4uProperty<string>();
    public string Hp { get { return hp.Value; } set { hp.Value = value; } }

    private M4uProperty<string> hpPlus = new M4uProperty<string>();
    public string HpPlus { get { return hpPlus.Value; } set { hpPlus.Value = value; } }

    private M4uProperty<string> atkLabel = new M4uProperty<string>();
    public string AtkLabel { get { return atkLabel.Value; } set { atkLabel.Value = value; } }

    private M4uProperty<string> atk = new M4uProperty<string>();
    public string Atk { get { return atk.Value; } set { atk.Value = value; } }

    private M4uProperty<string> atkPlus = new M4uProperty<string>();
    public string AtkPlus { get { return atkPlus.Value; } set { atkPlus.Value = value; } }

    private M4uProperty<string> costLabel = new M4uProperty<string>();
    public string CostLabel { get { return costLabel.Value; } set { costLabel.Value = value; } }

    private M4uProperty<string> cost = new M4uProperty<string>();
    public string Cost { get { return cost.Value; } set { cost.Value = value; } }

    private M4uProperty<string> limitOverLabel = new M4uProperty<string>();
    public string LimitOverLabel { get { return limitOverLabel.Value; } set { limitOverLabel.Value = value; } }

    private M4uProperty<string> overLabel = new M4uProperty<string>();
    public string OverLabel { get { return overLabel.Value; } set { overLabel.Value = value; } }

    private M4uProperty<string> limitOver = new M4uProperty<string>();
    public string LimitOver { get { return limitOver.Value; } set { limitOver.Value = value; } }

    private M4uProperty<string> charmLabel = new M4uProperty<string>();
    public string CharmLabel { get { return charmLabel.Value; } set { charmLabel.Value = value; } }

    private M4uProperty<string> charm = new M4uProperty<string>();
    public string Charm { get { return charm.Value; } set { charm.Value = value; } }

    private M4uProperty<bool> isViewFloatWindow = new M4uProperty<bool>();
    public bool IsViewFloatWindow { get { return isViewFloatWindow.Value; } set { isViewFloatWindow.Value = value; } }

    private M4uProperty<string> materialValue = new M4uProperty<string>();
    public string MaterialValue { get { return materialValue.Value; } set { materialValue.Value = value; } }

    private M4uProperty<string> skillLabel = new M4uProperty<string>();
    public string SkillLabel { get { return skillLabel.Value; } set { skillLabel.Value = value; } }

    private M4uProperty<string> profileLabel = new M4uProperty<string>();
    public string ProfileLabel { get { return profileLabel.Value; } set { profileLabel.Value = value; } }

    private M4uProperty<string> profileMessage = new M4uProperty<string>();
    public string ProfileMessage { get { return profileMessage.Value; } set { profileMessage.Value = value; } }

    private M4uProperty<string> materialLabel = new M4uProperty<string>();
    public string MaterialLabel { get { return materialLabel.Value; } set { materialLabel.Value = value; } }

    private M4uProperty<float> materialLabelX = new M4uProperty<float>();
    public float MaterialLabelX { get { return materialLabelX.Value; } set { materialLabelX.Value = value; } }

    public System.Action<UnitEvolveContext> DidSelectItem = delegate { };

    public UnitEvolveContext()
    {
        LevelLabel = GameTextUtil.GetText("unit_detail_01");
        HpLabel = GameTextUtil.GetText("unit_detail_03");
        AtkLabel = GameTextUtil.GetText("unit_detail_05");
        CostLabel = GameTextUtil.GetText("unit_detail_02");
        LimitOverLabel = GameTextUtil.GetText("unit_detail_06");
        OverLabel = GameTextUtil.GetText("unit_detail_10");
        CharmLabel = GameTextUtil.GetText("unit_detail_04");
        SkillLabel = GameTextUtil.GetText("unit_detail_07");
        ProfileLabel = GameTextUtil.GetText("unit_status14");
        MaterialLabel = GameTextUtil.GetText("unit_detail_12");
    }

    public void setup(MasterDataParamChara _chara, MasterDataParamCharaEvol _evolve = null, Texture2D _charaTexture = null, bool nameView = true)
    {
        charaMaster = _chara;
        evolMaster = _evolve;
        charaTexture = _charaTexture;
        limitOverMaster = MasterFinder<MasterDataLimitOver>.Instance.Find((int)charaMaster.limit_over_type);

        IsViewNamePanel = nameView;
        IsViewMaterialPanel = false;
        IsViewFloatWindow = false;
        if (evolMaster != null)
        {
            IsViewMaterialPanel = true;

            MaterialValue = "";
            MaterialList.Clear();
            MaterialLabelX = -190;
            if (evolMaster.unit_id_parts1 != 0)
            {
                addMaterial(evolMaster.unit_id_parts1);
                MaterialLabelX += 64;
            }
            if (evolMaster.unit_id_parts2 != 0)
            {
                addMaterial(evolMaster.unit_id_parts2);
                MaterialLabelX += 64;
            }
            if (evolMaster.unit_id_parts3 != 0)
            {
                addMaterial(evolMaster.unit_id_parts3);
                MaterialLabelX += 64;
            }
            if (evolMaster.unit_id_parts4 != 0)
            {
                addMaterial(evolMaster.unit_id_parts4);
                MaterialLabelX += 64;
            }
        }

        charaOnce = new CharaOnce();
        charaOnce.CharaSetupFromID(
            charaMaster.fix_id,
            (int)charaMaster.level_max,
            (int)0,
            (int)limitOverMaster.limit_over_max,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
            );

        Level = string.Format(GameTextUtil.GetText("unit_status16"), charaOnce.m_CharaLevel, charaMaster.level_max);
        Hp = charaOnce.m_CharaHP.ToString();
        HpPlus = "";
        Atk = charaOnce.m_CharaPow.ToString();
        AtkPlus = "";
        Cost = charaMaster.party_cost.ToString();
        LimitOver = string.Format(GameTextUtil.GetText("unit_status16"), limitOverMaster.limit_over_max, limitOverMaster.limit_over_max);
        Charm = charaOnce.m_CharaCharm.ToString("F1");
        ProfileMessage = charaMaster.detail.ReplaceSpaceTag(29).NoLineBreakTag();
    }

    private float m_IconSize = 64.0f;

    private void addMaterial(uint chara_id)
    {
        uint id = (uint)MaterialList.Count;
        MasterDataParamChara materialMaster = MasterDataUtil.GetCharaParamFromID(chara_id);
        if (materialMaster == null)
        {
            return;
        }
        MaterialValue += materialMaster.name + "\n";

        var iconModel = new ListItemModel((uint)id);

        MaterialDataContext _newData = new MaterialDataContext(iconModel);
        _newData.m_Id = (int)id;
        _newData.m_CharaId = chara_id;
        _newData.IsViewIcon = true;
        UnitIconImageProvider.Instance.Get(
            chara_id,
            sprite =>
            {
                _newData.IconImage = sprite;
            });
        _newData.IconColor = new Color(1, 1, 1);

        _newData.Width = m_IconSize;
        _newData.Height = m_IconSize;
        _newData.calcScale();

        _newData.SelectImage = ResourceManager.Instance.Load("icon_square1");

        MaterialList.Add(_newData);

    }
}
