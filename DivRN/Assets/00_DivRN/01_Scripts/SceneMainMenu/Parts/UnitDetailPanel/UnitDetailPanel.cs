using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;

public class UnitDetailPanel : MenuPartsBase
{
    public Image CharaImage = null;
    public Image ShadowImage = null;
    public UnitEvolveTouchArea TouchArea = null;
    public MasterDataParamChara charaMaster = null;
    public MasterDataLimitOver limitOverMaster = null;

    public System.Action<UnitDetailInfo.ToggleType, bool> selectTogglr = null;
    public System.Action selectSkill = null;
    public System.Action selectLink = null;
    public System.Action selectLoupe = null;
    public System.Action selectFavorite = null;

    private readonly float ExpWidthMax = 296;

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

    private M4uProperty<string> skillLabel = new M4uProperty<string>();
    public string SkillLabel { get { return skillLabel.Value; } set { skillLabel.Value = value; } }

    private M4uProperty<string> linkLabel = new M4uProperty<string>();
    public string LinkLabel { get { return linkLabel.Value; } set { linkLabel.Value = value; } }

    private M4uProperty<string> maxLabel = new M4uProperty<string>();
    public string MaxLabel { get { return maxLabel.Value; } set { maxLabel.Value = value; } }

    private M4uProperty<Sprite> charaSprite = new M4uProperty<Sprite>();
    public Sprite CharaSprite { get { return charaSprite.Value; } set { charaSprite.Value = value; } }

    private M4uProperty<Material> charaMaterial = new M4uProperty<Material>();
    public Material CharaMaterial { get { return charaMaterial.Value; } set { charaMaterial.Value = value; } }

    private M4uProperty<Sprite> shadowSprite = new M4uProperty<Sprite>();
    public Sprite ShadowSprite { get { return shadowSprite.Value; } set { shadowSprite.Value = value; } }

    private M4uProperty<Material> shadowMaterial = new M4uProperty<Material>();
    public Material ShadowMaterial { get { return shadowMaterial.Value; } set { shadowMaterial.Value = value; } }

    private M4uProperty<float> charaSpriteWidth = new M4uProperty<float>();
    public float CharaSpriteWidth { get { return charaSpriteWidth.Value; } set { charaSpriteWidth.Value = value; } }

    private M4uProperty<float> charaSpriteHeight = new M4uProperty<float>();
    public float CharaSpriteHeight { get { return charaSpriteHeight.Value; } set { charaSpriteHeight.Value = value; } }

    private M4uProperty<string> expLabel = new M4uProperty<string>();
    public string ExpLabel { get { return expLabel.Value; } set { expLabel.Value = value; } }

    private M4uProperty<float> nowExpWidth = new M4uProperty<float>();
    public float NowExpWidth { get { return nowExpWidth.Value; } set { nowExpWidth.Value = value; } }

    public float ExpRate { get { return (NowExpWidth / ExpWidthMax); } set { NowExpWidth = ExpWidthMax * value; } }

    M4uProperty<string> nextExpLabel = new M4uProperty<string>();
    public string NextExpLabel { get { return nextExpLabel.Value; } set { nextExpLabel.Value = value; } }

    M4uProperty<int> nextExp = new M4uProperty<int>();
    public int NextExp { get { return nextExp.Value; } set { nextExp.Value = value; } }

    private M4uProperty<string> profileLabel = new M4uProperty<string>();
    public string ProfileLabel { get { return profileLabel.Value; } set { profileLabel.Value = value; } }

    M4uProperty<string> profileMessage = new M4uProperty<string>();
    public string ProfileMessage { get { return profileMessage.Value; } set { profileMessage.Value = value; } }

    M4uProperty<bool> isExpActive = new M4uProperty<bool>();
    public bool IsExpActive { get { return isExpActive.Value; } set { isExpActive.Value = value; } }

    private M4uProperty<bool> isViewMaterialPanel = new M4uProperty<bool>();
    public bool IsViewMaterialPanel { get { return isViewMaterialPanel.Value; } set { isViewMaterialPanel.Value = value; } }

    private M4uProperty<List<MaterialDataContext>> materialList = new M4uProperty<List<MaterialDataContext>>(new List<MaterialDataContext>());
    public List<MaterialDataContext> MaterialList { get { return materialList.Value; } set { materialList.Value = value; } }

    private M4uProperty<bool> isViewFloatWindow = new M4uProperty<bool>();
    public bool IsViewFloatWindow { get { return isViewFloatWindow.Value; } set { isViewFloatWindow.Value = value; } }

    private M4uProperty<string> materialValue = new M4uProperty<string>();
    public string MaterialValue { get { return materialValue.Value; } set { materialValue.Value = value; } }

    private M4uProperty<string> materialLabel = new M4uProperty<string>();
    public string MaterialLabel { get { return materialLabel.Value; } set { materialLabel.Value = value; } }

    private M4uProperty<string> evolveLabel = new M4uProperty<string>();
    public string EvolveLabel { get { return evolveLabel.Value; } set { evolveLabel.Value = value; } }

    private M4uProperty<float> materialLabelX = new M4uProperty<float>();
    public float MaterialLabelX { get { return materialLabelX.Value; } set { materialLabelX.Value = value; } }

    M4uProperty<string> charaCountText = new M4uProperty<string>();
    public string CharaCountText { get { return charaCountText.Value; } set { charaCountText.Value = value; } }

    M4uProperty<bool> isViewCharaCount = new M4uProperty<bool>(true);
    /// <summary>所持数の表示・非表示</summary>
    public bool IsViewCharaCount { get { return isViewCharaCount.Value; } set { isViewCharaCount.Value = value; } }

    private M4uProperty<bool> isViewFavorite = new M4uProperty<bool>();
    public bool IsViewFavorite { get { return isViewFavorite.Value; } set { isViewFavorite.Value = value; } }

    private M4uProperty<bool> isFavorite = new M4uProperty<bool>();
    public bool IsFavorite { get { return isFavorite.Value; } set { isFavorite.Value = value; } }

    private M4uProperty<Color> linkButtonColor = new M4uProperty<Color>();
    public Color LinkButtonColor { get { return linkButtonColor.Value; } set { linkButtonColor.Value = value; } }

    private uint m_UnitId;
    private PacketStructUnit m_mainUnit = null;
    private PacketStructUnit m_subUnit = null;
    private bool lvMax = false;
    private bool linkButtonEnable = true;

    private void Awake()
    {
        LevelLabel = GameTextUtil.GetText("unit_detail_01");
        HpLabel = GameTextUtil.GetText("unit_detail_03");
        AtkLabel = GameTextUtil.GetText("unit_detail_05");
        CostLabel = GameTextUtil.GetText("unit_detail_02");
        LimitOverLabel = GameTextUtil.GetText("unit_detail_06");
        OverLabel = GameTextUtil.GetText("unit_detail_10");
        CharmLabel = GameTextUtil.GetText("unit_detail_04");
        SkillLabel = GameTextUtil.GetText("unit_detail_07");
        LinkLabel = GameTextUtil.GetText("unit_detail_08");
        MaxLabel = GameTextUtil.GetText("unit_detail_09");
        ExpLabel = GameTextUtil.GetText("unit_status11");
        NextExpLabel = "NEXT:";
        ProfileLabel = GameTextUtil.GetText("unit_status14");
        MaterialLabel = GameTextUtil.GetText("unit_detail_12");
        EvolveLabel = GameTextUtil.GetText("unit_detail_11");
        GetComponent<M4uContextRoot>().Context = this;
        CharaCountText = "";
        lvMax = false;
        linkButtonEnable = true;
        LinkButtonColor = Color.white;
    }

    // Use this for initialization
    void Start()
    {
        if (TouchArea != null)
        {
            TouchArea.DidPointerEnter = OnMaterialPointerEnter;
            TouchArea.DidPointerExit = OnMaterialPointerExit;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setup(uint unit_id, PacketStructUnit _mainUnit, PacketStructUnit _subUnit, Image Chara, Image Shadow)
    {
        lvMax = false;
        charaMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)unit_id);

        m_UnitId = unit_id;
        m_mainUnit = _mainUnit;
        m_subUnit = _subUnit;
        CharaOnce charaOnce = setParam();

        if (_mainUnit == null)
        {
            IsExpActive = false;
            linkButtonEnable = false;
            LinkButtonColor = Color.gray;
        }
        else
        {
            //-----------------------
            // 次のレベルまでの経験値を算出
            //-----------------------
            int nNowLevelExp = CharaUtil.GetStatusValue(charaMaster, (int)_mainUnit.level, CharaUtil.VALUE.EXP);
            int nNextLevelExp = CharaUtil.GetStatusValue(charaMaster, (int)_mainUnit.level + 1, CharaUtil.VALUE.EXP);
            int nLevelupExp = nNextLevelExp - nNowLevelExp;
            int nNextEXP = 0;
            float expRatio = 0.0f;
            if (nLevelupExp > 0)
            {
                nNextEXP = nNextLevelExp - (int)_mainUnit.exp;
                expRatio = (float)(nLevelupExp - nNextEXP) / nLevelupExp;
            }
            NextExp = nNextEXP;
            ExpRate = expRatio;
            IsExpActive = true;
        }

        CharaSprite = Chara.sprite;
        CharaMaterial = Chara.material;
        ShadowSprite = Shadow.sprite;
        ShadowMaterial = Shadow.material;
        CharaSpriteWidth = CharaSprite.texture.GetUnitTextureWidth();
        CharaSpriteHeight = CharaSprite.texture.GetUnitTextureHeight();

        ProfileMessage = charaMaster.detail.ReplaceSpaceTag(29).NoLineBreakTag();

        IsViewMaterialPanel = false;
        IsViewFloatWindow = false;
        MasterDataParamCharaEvol _evolAfter = MasterDataUtil.GetCharaEvolParamFromCharaID(unit_id);
        if (_evolAfter != null)
        {
            IsViewMaterialPanel = true;

            MaterialValue = "";
            MaterialList.Clear();
            MaterialLabelX = 108;
            if (_evolAfter.unit_id_parts1 != 0)
            {
                addMaterial(_evolAfter.unit_id_parts1);
                MaterialLabelX += 64;
            }
            if (_evolAfter.unit_id_parts2 != 0)
            {
                addMaterial(_evolAfter.unit_id_parts2);
                MaterialLabelX += 64;
            }
            if (_evolAfter.unit_id_parts3 != 0)
            {
                addMaterial(_evolAfter.unit_id_parts3);
                MaterialLabelX += 64;
            }
            if (_evolAfter.unit_id_parts4 != 0)
            {
                addMaterial(_evolAfter.unit_id_parts4);
                MaterialLabelX += 64;
            }
        }
        ScrollRect srect = GetComponentInChildren<ScrollRect>();
        if (srect != null)
        {
            srect.verticalNormalizedPosition = 1;
        }
    }

    private CharaOnce setParam()
    {
        int mainLv;
        int mainLOLv;
        limitOverMaster = MasterFinder<MasterDataLimitOver>.Instance.Find((int)charaMaster.limit_over_type);
        CharaOnce charaOnce = new CharaOnce();
        if (m_mainUnit == null)
        {
            mainLv = 1;
            mainLOLv = 0;
            if (lvMax == true)
            {
                mainLv = charaMaster.level_max;
                mainLOLv = limitOverMaster.limit_over_max;
            }
            charaOnce.CharaSetupFromID(
                m_UnitId,
                mainLv,
                0,
                mainLOLv,
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
            mainLv = (int)m_mainUnit.level;
            mainLOLv = (int)m_mainUnit.limitover_lv;
            if (lvMax == true)
            {
                mainLv = charaMaster.level_max;
                mainLOLv = limitOverMaster.limit_over_max;
            }
            if (m_mainUnit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE &&
                m_subUnit != null)
            {
                charaOnce.CharaSetupFromID(
                    m_UnitId,
                    mainLv,
                    (int)m_mainUnit.limitbreak_lv,
                    mainLOLv,
                    (int)m_mainUnit.add_pow,
                    (int)m_mainUnit.add_hp,
                    m_subUnit.id,
                    (int)m_subUnit.level,
                    (int)m_subUnit.add_pow,
                    (int)m_subUnit.add_hp,
                    (int)m_mainUnit.link_point,
                    (int)m_subUnit.limitover_lv
                    );
            }
            else
            {
                charaOnce.CharaSetupFromID(
                    charaMaster.fix_id,
                    mainLv,
                    (int)m_mainUnit.limitbreak_lv,
                    mainLOLv,
                    (int)m_mainUnit.add_pow,
                    (int)m_mainUnit.add_hp,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                    );
            }
        }

        Level = string.Format(GameTextUtil.GetText("unit_status16"), charaOnce.m_CharaLevel, charaMaster.level_max);
        Hp = charaOnce.m_CharaHP.ToString();
        if (charaOnce.m_CharaPlusHP != 0)
        {
            Hp = string.Format(GameTextUtil.GetText("unit_status19"), charaOnce.m_CharaHP, charaOnce.m_CharaPlusHP);
        }

        Atk = charaOnce.m_CharaPow.ToString();
        if (charaOnce.m_CharaPlusPow != 0)
        {
            Atk = string.Format(GameTextUtil.GetText("unit_status19"), charaOnce.m_CharaPow, charaOnce.m_CharaPlusPow);
        }

        int _cost = charaMaster.party_cost;
        if (m_subUnit != null)
        {
            _cost += CharaLinkUtil.GetLinkUnitCost(m_subUnit.id);
        }
        Cost = _cost.ToString();

        LimitOver = string.Format(GameTextUtil.GetText("unit_status16"), charaOnce.m_CharaLimitOver, limitOverMaster.limit_over_max);

        Charm = charaOnce.m_CharaCharm.ToString("F1");

        if (lvMax == true)
        {
            Level = string.Format(GameTextUtil.GetText("kyouka_text1"), Level);
            Hp = string.Format(GameTextUtil.GetText("kyouka_text1"), Hp);
            Atk = string.Format(GameTextUtil.GetText("kyouka_text1"), Atk);
            Cost = string.Format(GameTextUtil.GetText("kyouka_text1"), Cost);
            LimitOver = string.Format(GameTextUtil.GetText("kyouka_text1"), LimitOver);
            Charm = string.Format(GameTextUtil.GetText("kyouka_text1"), Charm);
        }

        return charaOnce;
    }

    public void OnSkillTouch()
    {
        if (selectSkill != null)
        {
            selectSkill();
        }
    }

    public void OnLinkTouch()
    {
        if (linkButtonEnable == false)
        {
            return;
        }
        if (selectLink != null)
        {
            selectLink();
        }
    }

    public void OnMaxTouch()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        if (lvMax == false)
        {
            lvMax = true;
            MaxLabel = string.Format(GameTextUtil.GetText("kyouka_text1"), GameTextUtil.GetText("unit_detail_09"));
        }
        else
        {
            lvMax = false;
            MaxLabel = GameTextUtil.GetText("unit_detail_09");
        }
        setParam();
    }

    public void OnEvolveTouch()
    {
        if (selectTogglr != null)
        {
            selectTogglr(UnitDetailInfo.ToggleType.Evolve, true);
        }
    }

    public void OnLoupeTouch()
    {
        if (selectLoupe != null)
        {
            selectLoupe();
        }
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

    public void OnMaterialPointerEnter()
    {
        IsViewFloatWindow = true;
    }
    public void OnMaterialPointerExit()
    {
        IsViewFloatWindow = false;
    }

    public void OnSelectFavorite()
    {
        if (selectFavorite != null)
        {
            selectFavorite();
        }
    }
}
