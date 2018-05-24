/**
 *  @file   DebugGetUnitSelectPanel.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/05/25
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;

public class DebugGetUnitSelectPanel : MenuPartsBase
{
    public Action<uint> ClickSearchAddButtonAction = delegate { };
    public Action ClickFixButtonAction = delegate { };
    public Action ClickResetButtonAction = delegate { };

    M4uProperty<List<DebugGetUnitListItemContext>> units = new M4uProperty<List<DebugGetUnitListItemContext>>(new List<DebugGetUnitListItemContext>());
    public List<DebugGetUnitListItemContext> Units { get { return units.Value; } set { units.Value = value; } }


    M4uProperty<Color> iDTextColor = new M4uProperty<Color>();
    public Color IDTextColor { get { return iDTextColor.Value; } set { iDTextColor.Value = value; } }

    M4uProperty<Color> noTextColor = new M4uProperty<Color>();
    public Color NoTextColor { get { return noTextColor.Value; } set { noTextColor.Value = value; } }

    /// <summary>デバッグ用のユニット情報</summary>
    public PacketStructUnitGetDebug m_UnitGetData = new PacketStructUnitGetDebug();

    /// <summary>検索したユニットの情報</summary>
    public MasterDataParamChara m_SearchCharaMaster = null;

    /// <summary>リンク素材を含めるかどうか</summary>
    public bool IsLinkMaterial
    {
        get
        {
            if (m_LinkMaterialToggle == null) { return false; }
            return m_LinkMaterialToggle.isOn;
        }
        set
        {
            if (m_LinkMaterialToggle != null)
            {
                m_LinkMaterialToggle.isOn = value;
            }
        }
    }
    /// <summary>進化素材を含めるかどうか</summary>
    public bool IsEvolMaterial
    {
        get
        {
            if (m_EvolMaterialToggle == null) { return false; }
            return m_EvolMaterialToggle.isOn;
        }
        set
        {
            if (m_EvolMaterialToggle != null)
            {
                m_EvolMaterialToggle.isOn = value;
            }
        }
    }

    /// <summary>追加ユニット数を10倍にするかどうか</summary>
    public bool IsMultiplyUnit
    {
        get
        {
            if (m_MultiplyUnitToggle == null) { return false; }
            return m_MultiplyUnitToggle.isOn;
        }
        set
        {
            if (m_MultiplyUnitToggle != null)
            {
                m_MultiplyUnitToggle.isOn = value;
            }
        }
    }

    [SerializeField]
    InputField m_IdInputField;
    [SerializeField]
    InputField m_NoInputField;
    [SerializeField]
    InputField m_LevelInputField;
    [SerializeField]
    InputField m_SkillLevelInputField;
    [SerializeField]
    InputField m_LimitOverLevelInputField;
    [SerializeField]
    InputField m_AddInputField;

    [SerializeField]
    Toggle m_LinkMaterialToggle;
    [SerializeField]
    Toggle m_EvolMaterialToggle;
    [SerializeField]
    Toggle m_MultiplyUnitToggle;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetParam()
    {
        for (int i = 0; i < Units.Count; i++)
        {
            int index = i;
            var unit = Units[i];

            UnitIconImageProvider.Instance.Reset(unit.unitID);

            unit.unitID = 0;
            unit.UnitName = "";

            UnitIconImageProvider.Instance.GetEmpty(sprite =>
            {
                unit.UnitImage = sprite;
            });
        }
        m_IdInputField.text = "";
        m_NoInputField.text = "";
        m_LevelInputField.text = "99";
        m_SkillLevelInputField.text = "99";
        m_LimitOverLevelInputField.text = "0";
        m_AddInputField.text = "0";

        IDTextColor = Color.black;
        NoTextColor = Color.black;

        m_UnitGetData = new PacketStructUnitGetDebug();
        m_UnitGetData.level = 99;
        m_UnitGetData.limitbreak_lv = 99;
        m_UnitGetData.limitover_lv = 0;
        m_UnitGetData.add_pow = 0;
        m_UnitGetData.add_hp = 0;
        IsLinkMaterial = false;
        IsEvolMaterial = false;
        IsMultiplyUnit = false;

        m_SearchCharaMaster = null;

    }

    public void AddUnit(uint unit_id)
    {
        if (unit_id == 0) { return; }

        DebugGetUnitListItemContext unit = Units.Find((v) => v.unitID == 0);
        if (unit == null) { return; }
        MasterDataParamChara charaMaster = MasterDataUtil.GetCharaParamFromID(unit_id);
        if (charaMaster == null) { return; }
        unit.unitID = unit_id;
        unit.UnitName = string.Format("ID:{0}", charaMaster.draw_id);

        UnitIconImageProvider.Instance.Get(
            unit_id,
            sprite =>
            {
                unit.UnitImage = sprite;
            },
            true);
    }

    public void DeleteUnit(DebugGetUnitListItemContext unit)
    {
        unit.unitID = 0;
        unit.UnitName = "";

        UnitIconImageProvider.Instance.GetEmpty(sprite =>
        {
            unit.UnitImage = sprite;
        });
    }

    #region Button
    public void OnClickSearchAddButton()
    {
        uint unit_id = (m_SearchCharaMaster != null) ? m_SearchCharaMaster.fix_id : 0;

        if (ClickSearchAddButtonAction != null)
        {
            ClickSearchAddButtonAction(unit_id);
        }
    }

    public void OnClickFixButton()
    {
        if (ClickFixButtonAction != null)
        {
            ClickFixButtonAction();
        }
    }

    public void OnClickResetButton()
    {
        if (ClickResetButtonAction != null)
        {
            ClickResetButtonAction();
        }
    }
    #endregion

    #region InputField
    public void OnEndEditID(string value)
    {
        uint unit_id = 0;
        uint.TryParse(value, out unit_id);

        m_SearchCharaMaster = MasterDataUtil.GetCharaParamFromID(unit_id); // キャラクターのマスターデータを取得

        string noText = "";
        if (m_SearchCharaMaster != null)
        {
            noText = m_SearchCharaMaster.draw_id.ToString();
            IDTextColor = Color.black;
            NoTextColor = Color.black;
        }
        else
        {
            IDTextColor = Color.red;
            NoTextColor = Color.red;
        }

        m_NoInputField.text = noText;
    }

    public void OnEndEditNo(string value)
    {
        uint unit_no = 0;
        uint.TryParse(value, out unit_no);

        m_SearchCharaMaster = MasterDataUtil.GetCharaParamFromDrawID(unit_no); // キャラクターのマスターデータを取得

        string idText = "";
        if (m_SearchCharaMaster != null)
        {
            idText = m_SearchCharaMaster.fix_id.ToString();
            IDTextColor = Color.black;
            NoTextColor = Color.black;
        }
        else
        {
            IDTextColor = Color.red;
            NoTextColor = Color.red;
        }

        m_IdInputField.text = idText;
    }

    public void OnEndEditLevel(string value)
    {
        uint.TryParse(value, out m_UnitGetData.level);
    }

    public void OnEndEditSkillLevel(string value)
    {
        uint.TryParse(value, out m_UnitGetData.limitbreak_lv);
    }

    public void OnEndEditLimitOverLevel(string value)
    {
        uint.TryParse(value, out m_UnitGetData.limitover_lv);
    }

    public void OnEndEditAdd(string value)
    {
        uint.TryParse(value, out m_UnitGetData.add_pow);
        uint.TryParse(value, out m_UnitGetData.add_hp);
    }
    #endregion
}
