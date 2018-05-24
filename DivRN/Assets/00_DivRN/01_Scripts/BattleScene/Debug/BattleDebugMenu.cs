using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class BattleDebugMenu : M4uContextMonoBehaviour
{
    public InputField m_HpDialogInputField = null;
    public Slider m_HpDialogSlider = null;
    public InputField m_HpDialogInputField2 = null;
    public Slider m_HpDialogSlider2 = null;

    public Sprite[] m_ElementSprites = new Sprite[(int)MasterDataDefineLabel.ElementType.MAX];

    public Dropdown m_DropdownEnemyActionEnemy = null;
    public Dropdown m_DropdownEnemyActionTable = null;

    public Slider m_SliderAutoPlayPanelPutCount = null;
    public Dropdown[] m_DropdownAutoPlaySkills = null;

#if BUILD_TYPE_DEBUG
    private enum SpeedType
    {
        UP,
        NORMAL,
        SLOW1,
        SLOW2,
        STOP,

        MAX
    }

    private enum HpButtonState
    {
        NORMAL,	// 通常
        NO_DEAD,	// 不死

        MAX
    }

    private enum BoostType
    {
        OFF,
        ON,
        KEEP,

        MAX
    }

    private string getHpButtonStateText(HpButtonState hp_button_state)
    {
        string ret_val = "";
        switch (hp_button_state)
        {
            case HpButtonState.NORMAL:
                ret_val = "通常";
                break;

            case HpButtonState.NO_DEAD:
                ret_val = "不死";
                break;
        }

        return ret_val;
    }

    private string getBuffButtonStateText(bool is_clear)
    {
        string ret_val = "";
        if (is_clear)
        {
            ret_val = "buff消去";
        }
        else
        {
            ret_val = "buff維持";
        }

        return ret_val;
    }

    private string getBoostTypeText(BoostType boost_type)
    {
        string ret_val = "";
        switch (boost_type)
        {
            case BoostType.OFF:
                ret_val = "OFF";
                break;

            case BoostType.ON:
                ret_val = "ON";
                break;

            case BoostType.KEEP:
                ret_val = "固定";
                break;
        }

        return ret_val;
    }

    // ローカル変数的に使う部分.
    M4uProperty<bool> speedButtons = new M4uProperty<bool>();

    M4uProperty<bool> buttonSpeedUp = new M4uProperty<bool>();
    M4uProperty<bool> buttonSpeedNormal = new M4uProperty<bool>();
    M4uProperty<bool> buttonSpeedSlow1 = new M4uProperty<bool>();
    M4uProperty<bool> buttonSpeedSlow2 = new M4uProperty<bool>();
    M4uProperty<bool> buttonSpeedStop = new M4uProperty<bool>();

    M4uProperty<bool> toggleSkill100 = new M4uProperty<bool>();
    M4uProperty<bool> toggleEnemyDamage1000 = new M4uProperty<bool>();

    M4uProperty<bool> buttonDebug = new M4uProperty<bool>();

    M4uProperty<MasterDataDefineLabel.ElementType> buttonHandL = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonHandLL = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonHandC = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonHandR = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonHandRR = new M4uProperty<MasterDataDefineLabel.ElementType>();

    M4uProperty<MasterDataDefineLabel.ElementType> buttonNextLL = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonNextL = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonNextC = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonNextR = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonNextRR = new M4uProperty<MasterDataDefineLabel.ElementType>();

    M4uProperty<MasterDataDefineLabel.ElementType> buttonField1LL = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField2LL = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField3LL = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField4LL = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField5LL = new M4uProperty<MasterDataDefineLabel.ElementType>();

    M4uProperty<MasterDataDefineLabel.ElementType> buttonField1L = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField2L = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField3L = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField4L = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField5L = new M4uProperty<MasterDataDefineLabel.ElementType>();

    M4uProperty<MasterDataDefineLabel.ElementType> buttonField1C = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField2C = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField3C = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField4C = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField5C = new M4uProperty<MasterDataDefineLabel.ElementType>();

    M4uProperty<MasterDataDefineLabel.ElementType> buttonField1R = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField2R = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField3R = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField4R = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField5R = new M4uProperty<MasterDataDefineLabel.ElementType>();

    M4uProperty<MasterDataDefineLabel.ElementType> buttonField1RR = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField2RR = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField3RR = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField4RR = new M4uProperty<MasterDataDefineLabel.ElementType>();
    M4uProperty<MasterDataDefineLabel.ElementType> buttonField5RR = new M4uProperty<MasterDataDefineLabel.ElementType>();

    M4uProperty<BoostType> buttonBoostLL = new M4uProperty<BoostType>();
    M4uProperty<BoostType> buttonBoostL = new M4uProperty<BoostType>();
    M4uProperty<BoostType> buttonBoostC = new M4uProperty<BoostType>();
    M4uProperty<BoostType> buttonBoostR = new M4uProperty<BoostType>();
    M4uProperty<BoostType> buttonBoostRR = new M4uProperty<BoostType>();

    M4uProperty<string> textUnitName1 = new M4uProperty<string>();
    M4uProperty<string> textUnitName2 = new M4uProperty<string>();
    M4uProperty<string> textUnitName3 = new M4uProperty<string>();
    M4uProperty<string> textUnitName4 = new M4uProperty<string>();
    M4uProperty<string> textUnitName5 = new M4uProperty<string>();

    M4uProperty<bool> buttonEnemyHp0 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHp1 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHp2 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHp3 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHp4 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHp5 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHp6 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHp7 = new M4uProperty<bool>();

    M4uProperty<bool> buttonEnemyHpSet0 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHpSet1 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHpSet2 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHpSet3 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHpSet4 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHpSet5 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHpSet6 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyHpSet7 = new M4uProperty<bool>();

    M4uProperty<HpButtonState> buttonTextUnitHpAll = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextUnitHp1 = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextUnitHp2 = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextUnitHp3 = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextUnitHp4 = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextUnitHp5 = new M4uProperty<HpButtonState>();

    M4uProperty<string> buttonTextUnitHpSet1 = new M4uProperty<string>();
    M4uProperty<string> buttonTextUnitHpSet2 = new M4uProperty<string>();
    M4uProperty<string> buttonTextUnitHpSet3 = new M4uProperty<string>();
    M4uProperty<string> buttonTextUnitHpSet4 = new M4uProperty<string>();
    M4uProperty<string> buttonTextUnitHpSet5 = new M4uProperty<string>();


    M4uProperty<bool> buttonTextUnitBuffAll = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextUnitBuff1 = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextUnitBuff2 = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextUnitBuff3 = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextUnitBuff4 = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextUnitBuff5 = new M4uProperty<bool>();

    M4uProperty<int> inputFieldHate1 = new M4uProperty<int>();
    M4uProperty<int> inputFieldHate2 = new M4uProperty<int>();
    M4uProperty<int> inputFieldHate3 = new M4uProperty<int>();
    M4uProperty<int> inputFieldHate4 = new M4uProperty<int>();
    M4uProperty<int> inputFieldHate5 = new M4uProperty<int>();

    M4uProperty<string> textEnemyName0 = new M4uProperty<string>();
    M4uProperty<string> textEnemyName1 = new M4uProperty<string>();
    M4uProperty<string> textEnemyName2 = new M4uProperty<string>();
    M4uProperty<string> textEnemyName3 = new M4uProperty<string>();
    M4uProperty<string> textEnemyName4 = new M4uProperty<string>();
    M4uProperty<string> textEnemyName5 = new M4uProperty<string>();
    M4uProperty<string> textEnemyName6 = new M4uProperty<string>();
    M4uProperty<string> textEnemyName7 = new M4uProperty<string>();

    M4uProperty<HpButtonState> buttonTextEnemyHpAll = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextEnemyHp0 = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextEnemyHp1 = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextEnemyHp2 = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextEnemyHp3 = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextEnemyHp4 = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextEnemyHp5 = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextEnemyHp6 = new M4uProperty<HpButtonState>();
    M4uProperty<HpButtonState> buttonTextEnemyHp7 = new M4uProperty<HpButtonState>();

    M4uProperty<string> buttonTextEnemyHpSet0 = new M4uProperty<string>();
    M4uProperty<string> buttonTextEnemyHpSet1 = new M4uProperty<string>();
    M4uProperty<string> buttonTextEnemyHpSet2 = new M4uProperty<string>();
    M4uProperty<string> buttonTextEnemyHpSet3 = new M4uProperty<string>();
    M4uProperty<string> buttonTextEnemyHpSet4 = new M4uProperty<string>();
    M4uProperty<string> buttonTextEnemyHpSet5 = new M4uProperty<string>();
    M4uProperty<string> buttonTextEnemyHpSet6 = new M4uProperty<string>();
    M4uProperty<string> buttonTextEnemyHpSet7 = new M4uProperty<string>();

    // 状態異常を消去するかどうかのフラグ
    M4uProperty<bool> buttonEnemyBuff0 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyBuff1 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyBuff2 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyBuff3 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyBuff4 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyBuff5 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyBuff6 = new M4uProperty<bool>();
    M4uProperty<bool> buttonEnemyBuff7 = new M4uProperty<bool>();

    M4uProperty<bool> buttonTextEnemyBuffAll = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextEnemyBuff0 = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextEnemyBuff1 = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextEnemyBuff2 = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextEnemyBuff3 = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextEnemyBuff4 = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextEnemyBuff5 = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextEnemyBuff6 = new M4uProperty<bool>();
    M4uProperty<bool> buttonTextEnemyBuff7 = new M4uProperty<bool>();


    M4uProperty<int> inputFieldBuffID = new M4uProperty<int>();
    M4uProperty<string> textBuffName = new M4uProperty<string>();
    M4uProperty<string> textBuffTurn = new M4uProperty<string>();


    M4uProperty<string> textHpDialog = new M4uProperty<string>();

    M4uProperty<string> inputFieldEnemyAbility1 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbility2 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbility3 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbility4 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbility5 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbility6 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbility7 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbility8 = new M4uProperty<string>();

    M4uProperty<string> textEnemyAbility1 = new M4uProperty<string>();
    M4uProperty<string> textEnemyAbility2 = new M4uProperty<string>();
    M4uProperty<string> textEnemyAbility3 = new M4uProperty<string>();
    M4uProperty<string> textEnemyAbility4 = new M4uProperty<string>();
    M4uProperty<string> textEnemyAbility5 = new M4uProperty<string>();
    M4uProperty<string> textEnemyAbility6 = new M4uProperty<string>();
    M4uProperty<string> textEnemyAbility7 = new M4uProperty<string>();
    M4uProperty<string> textEnemyAbility8 = new M4uProperty<string>();

    M4uProperty<Color> textColorEnemyAbility1 = new M4uProperty<Color>();
    M4uProperty<Color> textColorEnemyAbility2 = new M4uProperty<Color>();
    M4uProperty<Color> textColorEnemyAbility3 = new M4uProperty<Color>();
    M4uProperty<Color> textColorEnemyAbility4 = new M4uProperty<Color>();
    M4uProperty<Color> textColorEnemyAbility5 = new M4uProperty<Color>();
    M4uProperty<Color> textColorEnemyAbility6 = new M4uProperty<Color>();
    M4uProperty<Color> textColorEnemyAbility7 = new M4uProperty<Color>();
    M4uProperty<Color> textColorEnemyAbility8 = new M4uProperty<Color>();

    M4uProperty<string> inputFieldEnemyAbilityActionAppear = new M4uProperty<string>();
    M4uProperty<Color> inputFieldEnemyAbilityActionAppearTextColor = new M4uProperty<Color>();
    M4uProperty<string> inputFieldEnemyAbilityTableID = new M4uProperty<string>();
    M4uProperty<Color> inputFieldEnemyAbilityTableIDTextColor = new M4uProperty<Color>();

    M4uProperty<string> inputFieldEnemyAbilityAction1 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbilityAction2 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbilityAction3 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbilityAction4 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbilityAction5 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbilityAction6 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbilityAction7 = new M4uProperty<string>();
    M4uProperty<string> inputFieldEnemyAbilityAction8 = new M4uProperty<string>();

    M4uProperty<Color> inputFieldEnemyAbilityActionTextColor1 = new M4uProperty<Color>();
    M4uProperty<Color> inputFieldEnemyAbilityActionTextColor2 = new M4uProperty<Color>();
    M4uProperty<Color> inputFieldEnemyAbilityActionTextColor3 = new M4uProperty<Color>();
    M4uProperty<Color> inputFieldEnemyAbilityActionTextColor4 = new M4uProperty<Color>();
    M4uProperty<Color> inputFieldEnemyAbilityActionTextColor5 = new M4uProperty<Color>();
    M4uProperty<Color> inputFieldEnemyAbilityActionTextColor6 = new M4uProperty<Color>();
    M4uProperty<Color> inputFieldEnemyAbilityActionTextColor7 = new M4uProperty<Color>();
    M4uProperty<Color> inputFieldEnemyAbilityActionTextColor8 = new M4uProperty<Color>();

    M4uProperty<bool> panelDownloading = new M4uProperty<bool>();

    M4uProperty<string> textPanelPutCount = new M4uProperty<string>();

    // M4uコンポーネントはこちらの値を見る.
    public bool SpeedButtons { get { return speedButtons.Value; } }

    public Color ButtonSpeedUp { get { return (buttonSpeedUp.Value ? Color.black : Color.magenta); } }
    public Color ButtonSpeedNormal { get { return (buttonSpeedNormal.Value ? Color.black : Color.magenta); } }
    public Color ButtonSpeedSlow1 { get { return (buttonSpeedSlow1.Value ? Color.black : Color.magenta); } }
    public Color ButtonSpeedSlow2 { get { return (buttonSpeedSlow2.Value ? Color.black : Color.magenta); } }
    public Color ButtonSpeedStop { get { return (buttonSpeedStop.Value ? Color.black : Color.magenta); } }

    public bool ButtonDebug { get { return buttonDebug.Value; } }

    public bool ToggleSkill100 { get { return toggleSkill100.Value; } }
    public bool ToggleEnemyDamage1000 { get { return toggleEnemyDamage1000.Value; } }

    public Sprite ButtonHandLL { get { return m_ElementSprites[(int)buttonHandLL.Value]; } }
    public Sprite ButtonHandL { get { return m_ElementSprites[(int)buttonHandL.Value]; } }
    public Sprite ButtonHandC { get { return m_ElementSprites[(int)buttonHandC.Value]; } }
    public Sprite ButtonHandR { get { return m_ElementSprites[(int)buttonHandR.Value]; } }
    public Sprite ButtonHandRR { get { return m_ElementSprites[(int)buttonHandRR.Value]; } }

    public Sprite ButtonNextLL { get { return m_ElementSprites[(int)buttonNextLL.Value]; } }
    public Sprite ButtonNextL { get { return m_ElementSprites[(int)buttonNextL.Value]; } }
    public Sprite ButtonNextC { get { return m_ElementSprites[(int)buttonNextC.Value]; } }
    public Sprite ButtonNextR { get { return m_ElementSprites[(int)buttonNextR.Value]; } }
    public Sprite ButtonNextRR { get { return m_ElementSprites[(int)buttonNextRR.Value]; } }

    public Sprite ButtonField1LL { get { return m_ElementSprites[(int)buttonField1LL.Value]; } }
    public Sprite ButtonField2LL { get { return m_ElementSprites[(int)buttonField2LL.Value]; } }
    public Sprite ButtonField3LL { get { return m_ElementSprites[(int)buttonField3LL.Value]; } }
    public Sprite ButtonField4LL { get { return m_ElementSprites[(int)buttonField4LL.Value]; } }
    public Sprite ButtonField5LL { get { return m_ElementSprites[(int)buttonField5LL.Value]; } }

    public Sprite ButtonField1L { get { return m_ElementSprites[(int)buttonField1L.Value]; } }
    public Sprite ButtonField2L { get { return m_ElementSprites[(int)buttonField2L.Value]; } }
    public Sprite ButtonField3L { get { return m_ElementSprites[(int)buttonField3L.Value]; } }
    public Sprite ButtonField4L { get { return m_ElementSprites[(int)buttonField4L.Value]; } }
    public Sprite ButtonField5L { get { return m_ElementSprites[(int)buttonField5L.Value]; } }

    public Sprite ButtonField1C { get { return m_ElementSprites[(int)buttonField1C.Value]; } }
    public Sprite ButtonField2C { get { return m_ElementSprites[(int)buttonField2C.Value]; } }
    public Sprite ButtonField3C { get { return m_ElementSprites[(int)buttonField3C.Value]; } }
    public Sprite ButtonField4C { get { return m_ElementSprites[(int)buttonField4C.Value]; } }
    public Sprite ButtonField5C { get { return m_ElementSprites[(int)buttonField5C.Value]; } }

    public Sprite ButtonField1R { get { return m_ElementSprites[(int)buttonField1R.Value]; } }
    public Sprite ButtonField2R { get { return m_ElementSprites[(int)buttonField2R.Value]; } }
    public Sprite ButtonField3R { get { return m_ElementSprites[(int)buttonField3R.Value]; } }
    public Sprite ButtonField4R { get { return m_ElementSprites[(int)buttonField4R.Value]; } }
    public Sprite ButtonField5R { get { return m_ElementSprites[(int)buttonField5R.Value]; } }

    public Sprite ButtonField1RR { get { return m_ElementSprites[(int)buttonField1RR.Value]; } }
    public Sprite ButtonField2RR { get { return m_ElementSprites[(int)buttonField2RR.Value]; } }
    public Sprite ButtonField3RR { get { return m_ElementSprites[(int)buttonField3RR.Value]; } }
    public Sprite ButtonField4RR { get { return m_ElementSprites[(int)buttonField4RR.Value]; } }
    public Sprite ButtonField5RR { get { return m_ElementSprites[(int)buttonField5RR.Value]; } }

    public string ButtonBoostLL { get { return getBoostTypeText(buttonBoostLL.Value); } }
    public string ButtonBoostL { get { return getBoostTypeText(buttonBoostL.Value); } }
    public string ButtonBoostC { get { return getBoostTypeText(buttonBoostC.Value); } }
    public string ButtonBoostR { get { return getBoostTypeText(buttonBoostR.Value); } }
    public string ButtonBoostRR { get { return getBoostTypeText(buttonBoostRR.Value); } }

    public string TextUnitName1 { get { return textUnitName1.Value; } }
    public string TextUnitName2 { get { return textUnitName2.Value; } }
    public string TextUnitName3 { get { return textUnitName3.Value; } }
    public string TextUnitName4 { get { return textUnitName4.Value; } }
    public string TextUnitName5 { get { return textUnitName5.Value; } }

    public string ButtonTextUnitHpAll { get { return getHpButtonStateText(buttonTextUnitHpAll.Value); } }
    public string ButtonTextUnitHp1 { get { return getHpButtonStateText(buttonTextUnitHp1.Value); } }
    public string ButtonTextUnitHp2 { get { return getHpButtonStateText(buttonTextUnitHp2.Value); } }
    public string ButtonTextUnitHp3 { get { return getHpButtonStateText(buttonTextUnitHp3.Value); } }
    public string ButtonTextUnitHp4 { get { return getHpButtonStateText(buttonTextUnitHp4.Value); } }
    public string ButtonTextUnitHp5 { get { return getHpButtonStateText(buttonTextUnitHp5.Value); } }

    public string ButtonTextUnitHpSet1 { get { return buttonTextUnitHpSet1.Value; } }
    public string ButtonTextUnitHpSet2 { get { return buttonTextUnitHpSet2.Value; } }
    public string ButtonTextUnitHpSet3 { get { return buttonTextUnitHpSet3.Value; } }
    public string ButtonTextUnitHpSet4 { get { return buttonTextUnitHpSet4.Value; } }
    public string ButtonTextUnitHpSet5 { get { return buttonTextUnitHpSet5.Value; } }

    public string ButtonTextUnitBuffAll { get { return getBuffButtonStateText(buttonTextUnitBuffAll.Value); } }
    public string ButtonTextUnitBuff1 { get { return getBuffButtonStateText(buttonTextUnitBuff1.Value); } }
    public string ButtonTextUnitBuff2 { get { return getBuffButtonStateText(buttonTextUnitBuff2.Value); } }
    public string ButtonTextUnitBuff3 { get { return getBuffButtonStateText(buttonTextUnitBuff3.Value); } }
    public string ButtonTextUnitBuff4 { get { return getBuffButtonStateText(buttonTextUnitBuff4.Value); } }
    public string ButtonTextUnitBuff5 { get { return getBuffButtonStateText(buttonTextUnitBuff5.Value); } }

    public string InputFieldHate1 { get { return inputFieldHate1.Value.ToString(); } }
    public string InputFieldHate2 { get { return inputFieldHate2.Value.ToString(); } }
    public string InputFieldHate3 { get { return inputFieldHate3.Value.ToString(); } }
    public string InputFieldHate4 { get { return inputFieldHate4.Value.ToString(); } }
    public string InputFieldHate5 { get { return inputFieldHate5.Value.ToString(); } }

    public string TextEnemyName0 { get { return textEnemyName0.Value; } }
    public string TextEnemyName1 { get { return textEnemyName1.Value; } }
    public string TextEnemyName2 { get { return textEnemyName2.Value; } }
    public string TextEnemyName3 { get { return textEnemyName3.Value; } }
    public string TextEnemyName4 { get { return textEnemyName4.Value; } }
    public string TextEnemyName5 { get { return textEnemyName5.Value; } }
    public string TextEnemyName6 { get { return textEnemyName6.Value; } }
    public string TextEnemyName7 { get { return textEnemyName7.Value; } }

    public bool ButtonEnemyHp0 { get { return buttonEnemyHp0.Value; } }
    public bool ButtonEnemyHp1 { get { return buttonEnemyHp1.Value; } }
    public bool ButtonEnemyHp2 { get { return buttonEnemyHp2.Value; } }
    public bool ButtonEnemyHp3 { get { return buttonEnemyHp3.Value; } }
    public bool ButtonEnemyHp4 { get { return buttonEnemyHp4.Value; } }
    public bool ButtonEnemyHp5 { get { return buttonEnemyHp5.Value; } }
    public bool ButtonEnemyHp6 { get { return buttonEnemyHp6.Value; } }
    public bool ButtonEnemyHp7 { get { return buttonEnemyHp7.Value; } }

    public bool ButtonEnemyHpSet0 { get { return buttonEnemyHpSet0.Value; } }
    public bool ButtonEnemyHpSet1 { get { return buttonEnemyHpSet1.Value; } }
    public bool ButtonEnemyHpSet2 { get { return buttonEnemyHpSet2.Value; } }
    public bool ButtonEnemyHpSet3 { get { return buttonEnemyHpSet3.Value; } }
    public bool ButtonEnemyHpSet4 { get { return buttonEnemyHpSet4.Value; } }
    public bool ButtonEnemyHpSet5 { get { return buttonEnemyHpSet5.Value; } }
    public bool ButtonEnemyHpSet6 { get { return buttonEnemyHpSet6.Value; } }
    public bool ButtonEnemyHpSet7 { get { return buttonEnemyHpSet7.Value; } }

    public string ButtonTextEnemyHpAll { get { return getHpButtonStateText(buttonTextEnemyHpAll.Value); } }
    public string ButtonTextEnemyHp0 { get { return getHpButtonStateText(buttonTextEnemyHp0.Value); } }
    public string ButtonTextEnemyHp1 { get { return getHpButtonStateText(buttonTextEnemyHp1.Value); } }
    public string ButtonTextEnemyHp2 { get { return getHpButtonStateText(buttonTextEnemyHp2.Value); } }
    public string ButtonTextEnemyHp3 { get { return getHpButtonStateText(buttonTextEnemyHp3.Value); } }
    public string ButtonTextEnemyHp4 { get { return getHpButtonStateText(buttonTextEnemyHp4.Value); } }
    public string ButtonTextEnemyHp5 { get { return getHpButtonStateText(buttonTextEnemyHp5.Value); } }
    public string ButtonTextEnemyHp6 { get { return getHpButtonStateText(buttonTextEnemyHp6.Value); } }
    public string ButtonTextEnemyHp7 { get { return getHpButtonStateText(buttonTextEnemyHp7.Value); } }

    public string ButtonTextEnemyHpSet0 { get { return buttonTextEnemyHpSet0.Value; } }
    public string ButtonTextEnemyHpSet1 { get { return buttonTextEnemyHpSet1.Value; } }
    public string ButtonTextEnemyHpSet2 { get { return buttonTextEnemyHpSet2.Value; } }
    public string ButtonTextEnemyHpSet3 { get { return buttonTextEnemyHpSet3.Value; } }
    public string ButtonTextEnemyHpSet4 { get { return buttonTextEnemyHpSet4.Value; } }
    public string ButtonTextEnemyHpSet5 { get { return buttonTextEnemyHpSet5.Value; } }
    public string ButtonTextEnemyHpSet6 { get { return buttonTextEnemyHpSet6.Value; } }
    public string ButtonTextEnemyHpSet7 { get { return buttonTextEnemyHpSet7.Value; } }

    public bool ButtonEnemyBuff0 { get { return buttonEnemyBuff0.Value; } }
    public bool ButtonEnemyBuff1 { get { return buttonEnemyBuff1.Value; } }
    public bool ButtonEnemyBuff2 { get { return buttonEnemyBuff2.Value; } }
    public bool ButtonEnemyBuff3 { get { return buttonEnemyBuff3.Value; } }
    public bool ButtonEnemyBuff4 { get { return buttonEnemyBuff4.Value; } }
    public bool ButtonEnemyBuff5 { get { return buttonEnemyBuff5.Value; } }
    public bool ButtonEnemyBuff6 { get { return buttonEnemyBuff6.Value; } }
    public bool ButtonEnemyBuff7 { get { return buttonEnemyBuff7.Value; } }

    public string ButtonTextEnemyBuffAll { get { return getBuffButtonStateText(buttonTextEnemyBuffAll.Value); } }
    public string ButtonTextEnemyBuff0 { get { return getBuffButtonStateText(buttonTextEnemyBuff0.Value); } }
    public string ButtonTextEnemyBuff1 { get { return getBuffButtonStateText(buttonTextEnemyBuff1.Value); } }
    public string ButtonTextEnemyBuff2 { get { return getBuffButtonStateText(buttonTextEnemyBuff2.Value); } }
    public string ButtonTextEnemyBuff3 { get { return getBuffButtonStateText(buttonTextEnemyBuff3.Value); } }
    public string ButtonTextEnemyBuff4 { get { return getBuffButtonStateText(buttonTextEnemyBuff4.Value); } }
    public string ButtonTextEnemyBuff5 { get { return getBuffButtonStateText(buttonTextEnemyBuff5.Value); } }
    public string ButtonTextEnemyBuff6 { get { return getBuffButtonStateText(buttonTextEnemyBuff6.Value); } }
    public string ButtonTextEnemyBuff7 { get { return getBuffButtonStateText(buttonTextEnemyBuff7.Value); } }

    public string InputFieldBuffID { get { return inputFieldBuffID.Value.ToString(); } }
    public string TextBuffName { get { return textBuffName.Value; } }
    public string TextBuffTurn { get { return textBuffTurn.Value; } }

    public string TextHpDialog { get { return textHpDialog.Value; } }

    public string InputFieldEnemyAbility1 { get { return inputFieldEnemyAbility1.Value; } }
    public string InputFieldEnemyAbility2 { get { return inputFieldEnemyAbility2.Value; } }
    public string InputFieldEnemyAbility3 { get { return inputFieldEnemyAbility3.Value; } }
    public string InputFieldEnemyAbility4 { get { return inputFieldEnemyAbility4.Value; } }
    public string InputFieldEnemyAbility5 { get { return inputFieldEnemyAbility5.Value; } }
    public string InputFieldEnemyAbility6 { get { return inputFieldEnemyAbility6.Value; } }
    public string InputFieldEnemyAbility7 { get { return inputFieldEnemyAbility7.Value; } }
    public string InputFieldEnemyAbility8 { get { return inputFieldEnemyAbility8.Value; } }

    public string TextEnemyAbility1 { get { return textEnemyAbility1.Value; } }
    public string TextEnemyAbility2 { get { return textEnemyAbility2.Value; } }
    public string TextEnemyAbility3 { get { return textEnemyAbility3.Value; } }
    public string TextEnemyAbility4 { get { return textEnemyAbility4.Value; } }
    public string TextEnemyAbility5 { get { return textEnemyAbility5.Value; } }
    public string TextEnemyAbility6 { get { return textEnemyAbility6.Value; } }
    public string TextEnemyAbility7 { get { return textEnemyAbility7.Value; } }
    public string TextEnemyAbility8 { get { return textEnemyAbility8.Value; } }

    public Color TextColorEnemyAbility1 { get { return textColorEnemyAbility1.Value; } }
    public Color TextColorEnemyAbility2 { get { return textColorEnemyAbility2.Value; } }
    public Color TextColorEnemyAbility3 { get { return textColorEnemyAbility3.Value; } }
    public Color TextColorEnemyAbility4 { get { return textColorEnemyAbility4.Value; } }
    public Color TextColorEnemyAbility5 { get { return textColorEnemyAbility5.Value; } }
    public Color TextColorEnemyAbility6 { get { return textColorEnemyAbility6.Value; } }
    public Color TextColorEnemyAbility7 { get { return textColorEnemyAbility7.Value; } }
    public Color TextColorEnemyAbility8 { get { return textColorEnemyAbility8.Value; } }

    public string InputFieldEnemyAbilityActionAppear { get { return inputFieldEnemyAbilityActionAppear.Value; } }
    public Color InputFieldEnemyAbilityActionAppearTextColor { get { return inputFieldEnemyAbilityActionAppearTextColor.Value; } }
    public string InputFieldEnemyAbilityTableID { get { return inputFieldEnemyAbilityTableID.Value; } }
    public Color InputFieldEnemyAbilityTableIDTextColor { get { return inputFieldEnemyAbilityTableIDTextColor.Value; } }

    public string InputFieldEnemyAbilityAction1 { get { return inputFieldEnemyAbilityAction1.Value; } }
    public string InputFieldEnemyAbilityAction2 { get { return inputFieldEnemyAbilityAction2.Value; } }
    public string InputFieldEnemyAbilityAction3 { get { return inputFieldEnemyAbilityAction3.Value; } }
    public string InputFieldEnemyAbilityAction4 { get { return inputFieldEnemyAbilityAction4.Value; } }
    public string InputFieldEnemyAbilityAction5 { get { return inputFieldEnemyAbilityAction5.Value; } }
    public string InputFieldEnemyAbilityAction6 { get { return inputFieldEnemyAbilityAction6.Value; } }
    public string InputFieldEnemyAbilityAction7 { get { return inputFieldEnemyAbilityAction7.Value; } }
    public string InputFieldEnemyAbilityAction8 { get { return inputFieldEnemyAbilityAction8.Value; } }

    public Color InputFieldEnemyAbilityActionTextColor1 { get { return inputFieldEnemyAbilityActionTextColor1.Value; } }
    public Color InputFieldEnemyAbilityActionTextColor2 { get { return inputFieldEnemyAbilityActionTextColor2.Value; } }
    public Color InputFieldEnemyAbilityActionTextColor3 { get { return inputFieldEnemyAbilityActionTextColor3.Value; } }
    public Color InputFieldEnemyAbilityActionTextColor4 { get { return inputFieldEnemyAbilityActionTextColor4.Value; } }
    public Color InputFieldEnemyAbilityActionTextColor5 { get { return inputFieldEnemyAbilityActionTextColor5.Value; } }
    public Color InputFieldEnemyAbilityActionTextColor6 { get { return inputFieldEnemyAbilityActionTextColor6.Value; } }
    public Color InputFieldEnemyAbilityActionTextColor7 { get { return inputFieldEnemyAbilityActionTextColor7.Value; } }
    public Color InputFieldEnemyAbilityActionTextColor8 { get { return inputFieldEnemyAbilityActionTextColor8.Value; } }

    public bool PanelDownloading { get { return panelDownloading.Value; } }

    public string TextPanelPutCount { get { return textPanelPutCount.Value; } }

    private enum PanelType
    {
        NONE,
        MAIN,
        BATTLE,
        HAND,
        FIELD,
        CHARA_HP,
        ENEMY_HP,
        ENEMY_ACTION,
        BUFF,
        DISP_SETTING,
        AUTO_PLAY,

        MAX
    }

    private static string[] m_PanelNames =
    {
        "PanelNone",
        "PanelMain",
        "PanelBattle",
        "PanelHand",
        "PanelField",
        "PanelCharaHP",
        "PanelEnemyHP",
        "PanelEnemyAction",
        "PanelBuff",
        "PanelDispSetting",
        "PanelAutoPlay",
    };

    private float m_TimeScale = 1.0f;	// デバッグメニューを開く前の時間経過速度（デバッグメニューを開いている間は時間を止めている）
    private PanelType m_PanelType = PanelType.MAIN;
    private string m_HpDialog_name = null;
    private bool m_HpDialog_IsNodead = false;
    private int m_HpDialog_HpMax = 0;
    private int m_HpDialog_HpCurrent = 0;
    private M4uProperty<string> m_HpDialog_DestButtonText = null;

    private Transform m_TextEnemyInfo = null;	// 敵状態表示テキスト
    private Transform m_TextEnemyInfoMask = null;	// 敵状態表示テキスト

    private bool m_IsExecutingSetupEnemeyActionSub = false;	// ２重呼び出しをしない用

    private bool m_IsDispMask = false;
    private bool m_IsDispTime = false;
    private bool m_IsDispTurn = false;
    private bool m_IsDispEnemyInfo = false;
    private bool m_IsDispEnemyAbility = false;
    private bool m_IsDispEnemyAilment = false;
    private static bool m_IsDispDamagePlayer = false;
    private static bool m_IsDispDamageEnemy = false;
    private float m_DebugFpsTimer = 0.0f;
    private int m_DebugFpsCounter = 0;
    private int m_DebugFps = 0;

    private GameObject[] m_DebugTouchPosition = null;
    private int m_DebugTouchPositionIndex = 0;

    private enum EnemyMasterDownloadState
    {
        NONE,
        LOADING,
        READY,
        ERROR,
    }
    private EnemyMasterDownloadState m_EnemyMasterDownloadState = EnemyMasterDownloadState.NONE;

    private static BattleDebugMenu m_Instance = null;

    // Use this for initialization
    void Start()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }

        m_TextEnemyInfo = transform.Find("Canvas/PanelNone/TextInfo");
        m_TextEnemyInfoMask = transform.Find("Canvas/PanelNone/TextInfo/TextInfoMask");
        m_EnemyMasterDownloadState = EnemyMasterDownloadState.NONE;
        BattleParam.m_MasterDataCache.clearCacheEnemyAll();

        m_IsDispDamagePlayer = false;
        m_IsDispDamageEnemy = false;

        if (DebugOption.Instance.disalbeDebugMenu == true)
        {
            speedButtons.Value = false;
            buttonDebug.Value = false;
        }
        else
        {
            speedButtons.Value = true;
            buttonDebug.Value = true;
        }

        initAutoPlay();
    }

    private void OnDestroy()
    {
        if (m_Instance == this)
        {
            m_Instance = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0.0f)
        {
            m_DebugFpsTimer += Time.unscaledDeltaTime;
            m_DebugFpsCounter++;
            if (m_DebugFpsTimer >= 1.0f)
            {
                m_DebugFps = m_DebugFpsCounter;
                m_DebugFpsTimer -= 1.0f;
                m_DebugFpsCounter = 0;
            }
        }

        if (m_PanelType == PanelType.NONE
            && m_TextEnemyInfo != null
            && m_TextEnemyInfo.gameObject.IsActive()
        )
        {
            TMPro.TextMeshPro text = m_TextEnemyInfo.GetComponent<TMPro.TextMeshPro>();
            if (text != null)
            {
                string str = "";

                if (m_IsDispTime)
                {
                    str = DateTime.Now.ToString("MM/dd HH:mm:ss");
                    str += "  (UpdateFps:" + m_DebugFps.ToString() + ")\n";
                }

                if (m_IsDispTurn)
                {
                    str += "経過ターン：" + BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleTotalTurn.ToString() + "\n";
                }

                if (str != "")
                {
                    str += "\n";
                }

                if (m_IsDispEnemyInfo || m_IsDispEnemyAbility || m_IsDispEnemyAilment)
                {
                    if (BattleParam.m_EnemyParam != null)
                    {
                        for (int idx = 0; idx < BattleParam.m_EnemyParam.Length; idx++)
                        {
                            BattleEnemy battle_enemy = BattleParam.m_EnemyParam[idx];
                            if (battle_enemy != null)
                            {
                                str += "敵" + idx.ToString() + "(fixid:" + battle_enemy.getMasterDataParamEnemy().fix_id.ToString() + ")";
                                str += battle_enemy.getMasterDataParamChara().name;

                                if (m_IsDispEnemyInfo)
                                {
                                    str += "\n    HP:" + battle_enemy.m_EnemyHP.ToString() + "/" + battle_enemy.getMasterDataParamEnemy().status_hp.ToString()
                                        + "\n    HP%:" + (battle_enemy.m_EnemyHP * 100L / battle_enemy.m_EnemyHPMax).ToString() + "  敵行動判定HP%:" + calcEnemyActionSelectHpPercent(battle_enemy.m_EnemyHPMax, battle_enemy.m_EnemyHP);
                                    str += "\n    POW:" + battle_enemy.getMasterDataParamEnemy().status_pow.ToString()
                                        + "  DEF:" + battle_enemy.getMasterDataParamEnemy().status_def.ToString()
                                        + "  TURN:" + battle_enemy.getMasterDataParamEnemy().status_turn.ToString();

                                    if (battle_enemy.m_EnemyDrop != 0
                                        && BattleParam.m_BattleRequest != null && BattleParam.m_BattleRequest.m_QuestBuild != null)
                                    {
                                        ServerDataDefine.PacketStructQuest2BuildDrop drop_param = InGameUtilBattle.GetQuestBuildDrop(BattleParam.m_BattleRequest.m_QuestBuild, battle_enemy.m_EnemyDrop);
                                        if (drop_param != null)
                                        {
                                            ServerDataDefine.PacketStructQuest2BuildDrop.KindType kind_type = drop_param.getKindType();
                                            int item_id = drop_param.item_id;
                                            int item_num = drop_param.num;
                                            if (item_id != 0)
                                            {
                                                switch (kind_type)
                                                {
                                                    case ServerDataDefine.PacketStructQuest2BuildDrop.KindType.UNIT:
                                                        str += "\n        drop_unit:" + item_id.ToString();
                                                        break;

                                                    case ServerDataDefine.PacketStructQuest2BuildDrop.KindType.TICKET:
                                                        str += "\n        drop_ticket:" + item_num.ToString();
                                                        break;

                                                    case ServerDataDefine.PacketStructQuest2BuildDrop.KindType.MONEY:
                                                        str += "\n        drop_money:" + item_num.ToString();
                                                        break;

                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }

                                    EnemyActionTableControl enemy_action_table_control = battle_enemy.getEnemyActionTableControl();
                                    if (enemy_action_table_control != null)
                                    {
                                        int current_action_table_id = enemy_action_table_control.m_CurrentActionTableID;
                                        int action_step = enemy_action_table_control.m_ActionStep;

                                        MasterDataEnemyActionTable action_table = BattleParam.m_MasterDataCache.useEnemyActionTable((uint)current_action_table_id);
                                        if (action_table != null)
                                        {
                                            str += "\n    ActionTableID:" + current_action_table_id.ToString();
                                            str += "  ActionStep:" + action_step.ToString();

                                            int[] action_ids =
                                            {
                                            action_table.action_param_id1,
                                            action_table.action_param_id2,
                                            action_table.action_param_id3,
                                            action_table.action_param_id4,
                                            action_table.action_param_id5,
                                            action_table.action_param_id6,
                                            action_table.action_param_id7,
                                            action_table.action_param_id8,
                                        };

                                            MasterDataEnemyActionParam action_param = BattleParam.m_MasterDataCache.useEnemyActionParam((uint)action_ids[action_step]);
                                            if (action_param != null)
                                            {
                                                str += "\n    次のActionID:" + action_param.fix_id.ToString();
                                                str += " [" + action_param.skill_name + "]";
                                                str += "\n";
                                                str += "       ChainActionID:" + action_param.add_fix_id.ToString();
                                            }
                                        }
                                    }
                                }

                                if (m_IsDispEnemyAbility)
                                {
                                    uint[] enemy_abilitys = battle_enemy.getEnemyAbilitys();
                                    for (int ability_idx = 0; ability_idx < enemy_abilitys.Length; ability_idx++)
                                    {
                                        uint enemy_ability_fix_id = enemy_abilitys[ability_idx];
                                        MasterDataEnemyAbility master_data_enemy_ability = BattleParam.m_MasterDataCache.useEnemyAbility(enemy_ability_fix_id);
                                        if (master_data_enemy_ability != null)
                                        {
                                            str += string.Format("\n    特性{0}(fixid:{1}):{2}", ability_idx, master_data_enemy_ability.fix_id, master_data_enemy_ability.name);
                                            str += string.Format("\n      category:{0}({1})", master_data_enemy_ability.category.ToString(), (int)master_data_enemy_ability.category);
                                            str += string.Format("\n      p0:{0},p1:{1},p2:{2},p3:{3},p4:{4}",
                                                master_data_enemy_ability.param_00,
                                                master_data_enemy_ability.param_01,
                                                master_data_enemy_ability.param_02,
                                                master_data_enemy_ability.param_03,
                                                master_data_enemy_ability.param_04
                                            );
                                            str += string.Format("\n      p5:{0},p6:{1},p7:{2},p8:{3}",
                                                master_data_enemy_ability.param_05,
                                                master_data_enemy_ability.param_06,
                                                master_data_enemy_ability.param_07,
                                                master_data_enemy_ability.param_08
                                            );
                                        }
                                        else
                                        {
                                            if (enemy_ability_fix_id != 0)
                                            {
                                                str += string.Format("\n    特性{0}(fixid:{1}):ERROR! 存在しないfixidです.", ability_idx, enemy_ability_fix_id);
                                            }
                                        }
                                    }
                                }

                                if (m_IsDispEnemyAilment)
                                {
                                    for (int ailment_type_idx = 0; ailment_type_idx < (int)MasterDataDefineLabel.AilmentType.MAX; ailment_type_idx++)
                                    {
                                        MasterDataDefineLabel.AilmentType ailment_type = (MasterDataDefineLabel.AilmentType)ailment_type_idx;
                                        for (int ailment_idx = 0; ailment_idx < battle_enemy.m_StatusAilmentChara.GetAilmentCount(); ailment_idx++)
                                        {
                                            StatusAilment status_ailment = battle_enemy.m_StatusAilmentChara.GetAilment(ailment_idx);
                                            if (status_ailment != null
                                                && status_ailment.bUsed
                                                && status_ailment.nType == ailment_type
                                            )
                                            {
                                                MasterDataStatusAilmentParam master_data_status_ailment_param = BattleParam.m_MasterDataCache.useAilmentParam((uint)status_ailment.nMasterDataStatusAilmentID);
                                                if (master_data_status_ailment_param != null)
                                                {
                                                    str += string.Format("\n    状態異常(fixid:{0}):{1}", master_data_status_ailment_param.fix_id, master_data_status_ailment_param.name);
                                                    str += string.Format("\n      category:{0}({1})", master_data_status_ailment_param.category.ToString(), (int)master_data_status_ailment_param.category);
                                                    str += string.Format("\n      p1:{0},p2:{1},p3:{2},p4:{3}",
                                                        master_data_status_ailment_param.param01,
                                                        master_data_status_ailment_param.param02,
                                                        master_data_status_ailment_param.param03,
                                                        master_data_status_ailment_param.param04
                                                    );
                                                    str += string.Format("\n      p5:{0},p6:{1},p7:{2},p8:{3}",
                                                        master_data_status_ailment_param.param05,
                                                        master_data_status_ailment_param.param06,
                                                        master_data_status_ailment_param.param07,
                                                        master_data_status_ailment_param.param08
                                                    );
                                                    str += string.Format("\n      p9:{0},p10:{1},p11:{2},p12:{3}",
                                                        master_data_status_ailment_param.param09,
                                                        master_data_status_ailment_param.param10,
                                                        master_data_status_ailment_param.param11,
                                                        master_data_status_ailment_param.param12
                                                    );
                                                }
                                            }
                                        }
                                    }
                                }

                                str += "\n";
                            }
                        }
                    }
                }

                if (text.text != str)
                {
                    text.text = str;
                }
            }
        }

        updateDispTouchPosition();
    }

    private void OnEnable()
    {
        changePanel(PanelType.NONE);
        setSpeed(SpeedType.NORMAL);
    }

    private void OnDisable()
    {
        setSpeed(SpeedType.NORMAL);
    }

    private void changePanel(PanelType panel_type)
    {
        m_PanelType = panel_type;
        for (int idx = 0; idx < (int)PanelType.MAX; idx++)
        {
            Transform trans = transform.Find("Canvas/" + m_PanelNames[idx]);
            if (trans != null)
            {
                trans.gameObject.SetActive((PanelType)idx == m_PanelType);
            }
        }
        if (m_PanelType == PanelType.NONE)
        {
            Time.timeScale = m_TimeScale;
        }
    }

    private void openHpDialog(bool is_enemy, int index, M4uProperty<string> dest_button_text)
    {
        if (index >= 0)
        {
            Transform trans = transform.Find("Canvas/PanelHpSetting");
            if (trans != null)
            {
                string name = null;
                bool is_nodead = false;
                if (is_enemy)
                {
                    if (index >= 0 && index < BattleParam.m_EnemyParam.Length)
                    {
                        BattleEnemy battle_enemy = BattleParam.m_EnemyParam[index];
                        if (battle_enemy != null)
                        {
                            name = battle_enemy.getMasterDataParamChara().name;
                        }
                    }
                    is_nodead = true;
                }
                else
                {
                    CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)index, CharaParty.CharaCondition.EXIST);

                    if (chara_once != null)
                    {
                        name = chara_once.m_CharaMasterDataParam.name;
                    }
                }

                if (name != null)
                {
                    m_HpDialog_name = null;

                    m_HpDialog_IsNodead = is_nodead;
                    m_HpDialog_HpMax = getMaxHpFromHpButtonText(dest_button_text);
                    m_HpDialog_HpCurrent = getHpFromHpButtonText(dest_button_text);
                    m_HpDialog_DestButtonText = dest_button_text;

                    m_HpDialogSlider.minValue = 0.9f;
                    m_HpDialogSlider.value = 0.9f;	// ここで値変更のコールバックが呼ばれるので注意.
                    m_HpDialogSlider.maxValue = m_HpDialog_HpMax;

                    m_HpDialogSlider2.minValue = 0.0f;
                    m_HpDialogSlider2.value = 0.0f;	// ここで値変更のコールバックが呼ばれるので注意.
                    m_HpDialogSlider2.maxValue = 100.0f;

                    m_HpDialog_name = name;
                    updateDialog(m_HpDialog_HpCurrent);
                    trans.gameObject.SetActive(true);
                }
            }
        }
    }

    private void updateDialog(int hp)
    {
        if (m_HpDialog_name == null)
        {
            return;
        }
        string temp_name = m_HpDialog_name;	// コールバックから呼ばれた場合に対処するために m_HpDialog_name を null にしておく.
        m_HpDialog_name = null;

        m_HpDialog_HpCurrent = hp;
        if (m_HpDialog_HpCurrent < 0)
        {
            m_HpDialog_HpCurrent = 0;
        }
        if (m_HpDialog_HpCurrent > m_HpDialog_HpMax)
        {
            m_HpDialog_HpCurrent = m_HpDialog_HpMax;
        }

        int hp_percent = (int)(m_HpDialog_HpCurrent * 100L / m_HpDialog_HpMax);

        // 敵行動判定用ＨＰ％
        int hp_percent2 = calcEnemyActionSelectHpPercent(m_HpDialog_HpMax, m_HpDialog_HpCurrent);

        string wrk_text = temp_name.ToString() + "\n"
            + m_HpDialog_HpCurrent.ToString() + "/" + m_HpDialog_HpMax.ToString() + "\n"
            + hp_percent.ToString() + "%";

        textHpDialog.Value = wrk_text;

        m_HpDialogInputField.text = m_HpDialog_HpCurrent.ToString();
        m_HpDialogInputField2.text = hp_percent2.ToString();

        m_HpDialogSlider.value = m_HpDialog_HpCurrent;	// ここで値変更のコールバックが呼ばれるので注意.
        m_HpDialogSlider2.value = hp_percent2;	// ここで値変更のコールバックが呼ばれるので注意.

        m_HpDialog_name = temp_name;
    }

    private void updateDialogByEnemyActionHpPercent(int hp_percent)
    {
        float rate = (float)InGameUtilBattle.GetDBRevisionValue(hp_percent);
        int enemy_hp = (int)InGameUtilBattle.AvoidErrorMultiple(m_HpDialog_HpMax, rate);

        updateDialog(enemy_hp);
    }

    /// <summary>
    /// 敵行動判定用のＨＰパーセントを求める（EnemyActionTableControl 内と同じ計算をする）
    /// </summary>
    /// <param name="hp_max"></param>
    /// <param name="hp_current"></param>
    /// <returns></returns>
    private static int calcEnemyActionSelectHpPercent(int hp_max, int hp_current)
    {
        if (hp_max <= 0)
        {
            return 0;
        }

        int hp_percent = (int)(hp_current * 100L / hp_max) - 1;
        while (true)
        {
            float rate = (float)InGameUtilBattle.GetDBRevisionValue(hp_percent);
            int enemy_hp = (int)InGameUtilBattle.AvoidErrorMultiple(hp_max, rate);
            if (hp_current <= enemy_hp)
            {
                break;
            }

            hp_percent++;
        }

        if (hp_percent < 0)
        {
            hp_percent = 0;
        }
        if (hp_percent > 100)
        {
            hp_percent = 100;
        }

        return hp_percent;
    }

    private void closeHpDialog(bool is_decide)
    {
        Transform trans = transform.Find("Canvas/PanelHpSetting");
        if (trans != null)
        {
            trans.gameObject.SetActive(false);

            if (is_decide)
            {
                if (m_HpDialog_IsNodead && m_HpDialog_HpCurrent < 1)
                {
                    m_HpDialog_HpCurrent = 1;
                }
                setHpButtonText(m_HpDialog_DestButtonText, m_HpDialog_HpCurrent, m_HpDialog_HpMax);
            }

            m_HpDialog_name = null;
            m_HpDialog_DestButtonText = null;
        }
    }

    private void setHpButtonText(M4uProperty<string> dest_button_text, int current_hp, int max_hp)
    {
        dest_button_text.Value = "HP:" + current_hp.ToString() + "/" + max_hp.ToString();
    }

    private int getHpFromHpButtonText(M4uProperty<string> src_button_text)
    {
        string hp_button_text = src_button_text.Value;
        hp_button_text = hp_button_text.Replace("HP:", "");
        hp_button_text = hp_button_text.Substring(0, hp_button_text.IndexOf("/"));
        int new_hp_current = _int_Parse(hp_button_text);

        return new_hp_current;
    }
    private int getMaxHpFromHpButtonText(M4uProperty<string> src_button_text)
    {
        string hp_button_text = src_button_text.Value;
        hp_button_text = hp_button_text.Substring(hp_button_text.IndexOf("/") + 1);
        int new_hp_current = _int_Parse(hp_button_text);

        return new_hp_current;
    }


    private void setSpeed(SpeedType speed_type)
    {
        switch (speed_type)
        {
            case SpeedType.UP:
                Time.timeScale = 2.0f;
                buttonSpeedUp.Value = false;
                buttonSpeedNormal.Value = true;
                buttonSpeedSlow1.Value = true;
                buttonSpeedSlow2.Value = true;
                buttonSpeedStop.Value = true;
                break;

            case SpeedType.NORMAL:
                Time.timeScale = 1.0f;
                buttonSpeedUp.Value = true;
                buttonSpeedNormal.Value = false;
                buttonSpeedSlow1.Value = true;
                buttonSpeedSlow2.Value = true;
                buttonSpeedStop.Value = true;
                break;

            case SpeedType.SLOW1:
                Time.timeScale = 0.5f;
                buttonSpeedUp.Value = true;
                buttonSpeedNormal.Value = true;
                buttonSpeedSlow1.Value = false;
                buttonSpeedSlow2.Value = true;
                buttonSpeedStop.Value = true;
                break;

            case SpeedType.SLOW2:
                Time.timeScale = 0.25f;
                buttonSpeedUp.Value = true;
                buttonSpeedNormal.Value = true;
                buttonSpeedSlow1.Value = true;
                buttonSpeedSlow2.Value = false;
                buttonSpeedStop.Value = true;
                break;

            case SpeedType.STOP:
                Time.timeScale = 0.0f;
                buttonSpeedUp.Value = true;
                buttonSpeedNormal.Value = true;
                buttonSpeedSlow1.Value = true;
                buttonSpeedSlow2.Value = true;
                buttonSpeedStop.Value = false;
                break;
        }

        m_TimeScale = Time.timeScale;
    }

    public void OnChangeEvent(string object_name)
    {
        switch (object_name)
        {
            case "ButtonDebugView":
                if (speedButtons.Value == true)
                {
                    speedButtons.Value = false;
                    buttonDebug.Value = false;
                }
                else
                {
                    speedButtons.Value = true;
                    buttonDebug.Value = true;
                }
                break;

            case "ButtonDebug":
                Time.timeScale = 0.0f;
                changePanel(PanelType.MAIN);
                break;

            case "ButtonSpeedUp":
                setSpeed(SpeedType.UP);
                break;

            case "ButtonSpeedNormal":
                setSpeed(SpeedType.NORMAL);
                break;

            case "ButtonSpeedSlow1":
                setSpeed(SpeedType.SLOW1);
                break;

            case "ButtonSpeedSlow2":
                setSpeed(SpeedType.SLOW2);
                break;

            case "ButtonSpeedStop":
                setSpeed(SpeedType.STOP);
                break;

            case "ButtonAutoplay":
                BattleSceneManager.Instance.AutoPlay.startAutoPlay(null, true);
                break;

            case "ButtonClose":
                changePanel(PanelType.NONE);
                break;

            case "ButtonEnd":
                // デバッグメニュー終了（再表示できなくなる）
                setSpeed(SpeedType.NORMAL);
                changePanel(PanelType.NONE);
                gameObject.SetActive(false);
                GameObject.Destroy(gameObject);
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                break;

            case "ButtonCancel":
                changePanel(PanelType.MAIN);
                break;

            case "ButtonDecide":
                switch (m_PanelType)
                {
                    case PanelType.BATTLE:
                        decidePanelBattle();
                        break;

                    case PanelType.HAND:
                        decidePanelHand();
                        break;

                    case PanelType.FIELD:
                        decidePanelField();
                        break;

                    case PanelType.CHARA_HP:
                        decideUnitState();
                        break;

                    case PanelType.ENEMY_HP:
                        decideEnemyState();
                        break;

                    case PanelType.ENEMY_ACTION:
                        decideEnemyAction();
                        break;

                    case PanelType.BUFF:
                        decideBuff();
                        break;

                    case PanelType.DISP_SETTING:
                        decideDispSetting();
                        break;

                    case PanelType.AUTO_PLAY:
                        decideAutoPlay();
                        break;
                }
                changePanel(PanelType.MAIN);
                break;

            case "ButtonOpenPanelBattle":
                setupPanelBattle();
                changePanel(PanelType.BATTLE);
                break;

            case "ButtonOpenPanelHand":
                setupPanelHand();
                changePanel(PanelType.HAND);
                break;

            case "ButtonOpenPanelField":
                setupPanelField();
                changePanel(PanelType.FIELD);
                break;

            case "ButtonOpenPanelCharaHp":
                setupUnitState();
                changePanel(PanelType.CHARA_HP);
                break;

            case "ButtonOpenPanelEnemyHp":
                setupEnemyState();
                changePanel(PanelType.ENEMY_HP);
                break;

            case "ButtonOpenPanelEnemyAction":
                setupEnemyAction();
                changePanel(PanelType.ENEMY_ACTION);
                break;

            case "ButtonOpenPanelBuff":
                setupBuff();
                changePanel(PanelType.BUFF);
                break;

            case "ButtonOpenPanelDispSetting":
                setupDispSetting();
                changePanel(PanelType.DISP_SETTING);
                break;

            case "ButtonOpenPanelAutoPlay":
                setupAutoPlay();
                changePanel(PanelType.AUTO_PLAY);
                break;

            case "ButtonKillEnemyAll":
#if BUILD_TYPE_DEBUG
                if (BattleSceneManager.HasInstance)
                {
                    BattleSceneManager.Instance.PRIVATE_FIELD.DebugKillAllEnemy();
                }
                changePanel(PanelType.NONE);
#endif
                break;

            case "ButtonSpPlus":
                BattleParam.m_PlayerParty.RecoverySP(1, true, true);
                BattleSceneManager.Instance.PRIVATE_FIELD.updateResurrectInfoView();
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：SP変更（→" + BattleParam.m_PlayerParty.GetSP().ToString());
                break;

            case "ButtonSpMinus":
                BattleParam.m_PlayerParty.DamageSP(1);
                BattleSceneManager.Instance.PRIVATE_FIELD.updateResurrectInfoView();
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：SP変更（→" + BattleParam.m_PlayerParty.GetSP().ToString());
                break;

            case "ButtonHeroSkillTurn":
                // 主人公のスキルターンを消化.
                BattleParam.m_PlayerParty.m_BattleHero.addSkillTrun(1, false);

                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：主人公スキルターン消化");
                break;

            case "ButtonSkillTurnZero":
                // 主人公・パーティのスキルターンを初期化
                {
                    BattleParam.m_PlayerParty.m_BattleHero.resetSkillTurn();

                    for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
                    {
                        CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.SKILL_TURN1);
                        if (chara_once != null)
                        {
                            chara_once.ClrCharaLimitBreak();
                        }
                    }

                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：主人公・ユニットスキルターン初期化");
                }
                break;

            case "ButtonPlayerSkillTurn":
                // パーティのスキルターンを消化.
                {
                    for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
                    {
                        CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.SKILL_TURN1);
                        if (chara_once != null)
                        {
                            chara_once.AddCharaLimitBreak(1);
                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：ユニットスキルターン消化");
                        }
                    }
                }
                break;

            case "ButtonSkillTurn":
                // 主人公・パーティのスキルターンを使用可能に
                {
                    BattleParam.m_PlayerParty.m_BattleHero.addSkillTrun(99999, false);

                    for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
                    {
                        CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.SKILL_TURN1);
                        if (chara_once != null)
                        {
                            chara_once.AddCharaLimitBreak(99999);
                        }
                    }

                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：主人公・ユニット全スキルターン消化");
                }
                break;


            case "ButtonHandLL":
                buttonHandLL.Value = nextCard(buttonHandLL.Value);
                break;
            case "ButtonHandL":
                buttonHandL.Value = nextCard(buttonHandL.Value);
                break;
            case "ButtonHandC":
                buttonHandC.Value = nextCard(buttonHandC.Value);
                break;
            case "ButtonHandR":
                buttonHandR.Value = nextCard(buttonHandR.Value);
                break;
            case "ButtonHandRR":
                buttonHandRR.Value = nextCard(buttonHandRR.Value);
                break;

            case "ButtonNextLL":
                buttonNextLL.Value = nextCard(buttonNextLL.Value);
                break;
            case "ButtonNextL":
                buttonNextL.Value = nextCard(buttonNextL.Value);
                break;
            case "ButtonNextC":
                buttonNextC.Value = nextCard(buttonNextC.Value);
                break;
            case "ButtonNextR":
                buttonNextR.Value = nextCard(buttonNextR.Value);
                break;
            case "ButtonNextRR":
                buttonNextRR.Value = nextCard(buttonNextRR.Value);
                break;

            case "ButtonField1LL":
                buttonField1LL.Value = nextCard2(buttonField1LL.Value);
                break;
            case "ButtonField2LL":
                buttonField2LL.Value = nextCard2(buttonField2LL.Value);
                break;
            case "ButtonField3LL":
                buttonField3LL.Value = nextCard2(buttonField3LL.Value);
                break;
            case "ButtonField4LL":
                buttonField4LL.Value = nextCard2(buttonField4LL.Value);
                break;
            case "ButtonField5LL":
                buttonField5LL.Value = nextCard2(buttonField5LL.Value);
                break;

            case "ButtonField1L":
                buttonField1L.Value = nextCard2(buttonField1L.Value);
                break;
            case "ButtonField2L":
                buttonField2L.Value = nextCard2(buttonField2L.Value);
                break;
            case "ButtonField3L":
                buttonField3L.Value = nextCard2(buttonField3L.Value);
                break;
            case "ButtonField4L":
                buttonField4L.Value = nextCard2(buttonField4L.Value);
                break;
            case "ButtonField5L":
                buttonField5L.Value = nextCard2(buttonField5L.Value);
                break;

            case "ButtonField1C":
                buttonField1C.Value = nextCard2(buttonField1C.Value);
                break;
            case "ButtonField2C":
                buttonField2C.Value = nextCard2(buttonField2C.Value);
                break;
            case "ButtonField3C":
                buttonField3C.Value = nextCard2(buttonField3C.Value);
                break;
            case "ButtonField4C":
                buttonField4C.Value = nextCard2(buttonField4C.Value);
                break;
            case "ButtonField5C":
                buttonField5C.Value = nextCard2(buttonField5C.Value);
                break;

            case "ButtonField1R":
                buttonField1R.Value = nextCard2(buttonField1R.Value);
                break;
            case "ButtonField2R":
                buttonField2R.Value = nextCard2(buttonField2R.Value);
                break;
            case "ButtonField3R":
                buttonField3R.Value = nextCard2(buttonField3R.Value);
                break;
            case "ButtonField4R":
                buttonField4R.Value = nextCard2(buttonField4R.Value);
                break;
            case "ButtonField5R":
                buttonField5R.Value = nextCard2(buttonField5R.Value);
                break;

            case "ButtonField1RR":
                buttonField1RR.Value = nextCard2(buttonField1RR.Value);
                break;
            case "ButtonField2RR":
                buttonField2RR.Value = nextCard2(buttonField2RR.Value);
                break;
            case "ButtonField3RR":
                buttonField3RR.Value = nextCard2(buttonField3RR.Value);
                break;
            case "ButtonField4RR":
                buttonField4RR.Value = nextCard2(buttonField4RR.Value);
                break;
            case "ButtonField5RR":
                buttonField5RR.Value = nextCard2(buttonField5RR.Value);
                break;

            case "ButtonBoostLL":
                buttonBoostLL.Value = (BoostType)(((int)buttonBoostLL.Value + 1) % (int)BoostType.MAX);
                break;
            case "ButtonBoostL":
                buttonBoostL.Value = (BoostType)(((int)buttonBoostL.Value + 1) % (int)BoostType.MAX);
                break;
            case "ButtonBoostC":
                buttonBoostC.Value = (BoostType)(((int)buttonBoostC.Value + 1) % (int)BoostType.MAX);
                break;
            case "ButtonBoostR":
                buttonBoostR.Value = (BoostType)(((int)buttonBoostR.Value + 1) % (int)BoostType.MAX);
                break;
            case "ButtonBoostRR":
                buttonBoostRR.Value = (BoostType)(((int)buttonBoostRR.Value + 1) % (int)BoostType.MAX);
                break;

            case "ButtonUnitHpAll":
                {
                    buttonTextUnitHpAll.Value = (HpButtonState)(((int)(buttonTextUnitHpAll.Value + 1)) % ((int)HpButtonState.MAX));

                    buttonTextUnitHp1.Value = buttonTextUnitHpAll.Value;
                    buttonTextUnitHp2.Value = buttonTextUnitHpAll.Value;
                    buttonTextUnitHp3.Value = buttonTextUnitHpAll.Value;
                    buttonTextUnitHp4.Value = buttonTextUnitHpAll.Value;
                    buttonTextUnitHp5.Value = buttonTextUnitHpAll.Value;
                }
                break;

            case "ButtonUnitHp1":
                buttonTextUnitHp1.Value = (HpButtonState)(((int)(buttonTextUnitHp1.Value + 1)) % ((int)HpButtonState.MAX));
                break;
            case "ButtonUnitHp2":
                buttonTextUnitHp2.Value = (HpButtonState)(((int)(buttonTextUnitHp2.Value + 1)) % ((int)HpButtonState.MAX));
                break;
            case "ButtonUnitHp3":
                buttonTextUnitHp3.Value = (HpButtonState)(((int)(buttonTextUnitHp3.Value + 1)) % ((int)HpButtonState.MAX));
                break;
            case "ButtonUnitHp4":
                buttonTextUnitHp4.Value = (HpButtonState)(((int)(buttonTextUnitHp4.Value + 1)) % ((int)HpButtonState.MAX));
                break;
            case "ButtonUnitHp5":
                buttonTextUnitHp5.Value = (HpButtonState)(((int)(buttonTextUnitHp5.Value + 1)) % ((int)HpButtonState.MAX));
                break;

            case "ButtonUnitHpMaxAll":
                for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
                {
                    CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.EXIST);
                    if (chara_once != null)
                    {
                        M4uProperty<string>[] unit_hp_buttons =
                        {
                            buttonTextUnitHpSet1,
                            buttonTextUnitHpSet2,
                            buttonTextUnitHpSet3,
                            buttonTextUnitHpSet4,
                            buttonTextUnitHpSet5,
                        };

                        int hp_max = BattleParam.m_PlayerParty.m_HPMax.getValue((GlobalDefine.PartyCharaIndex)idx);
                        setHpButtonText(unit_hp_buttons[idx], hp_max, hp_max);
                    }
                }
                break;

            case "ButtonUnitDeadlyAll":
                for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
                {
                    CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.EXIST);
                    if (chara_once != null)
                    {
                        M4uProperty<string>[] unit_hp_buttons =
                        {
                            buttonTextUnitHpSet1,
                            buttonTextUnitHpSet2,
                            buttonTextUnitHpSet3,
                            buttonTextUnitHpSet4,
                            buttonTextUnitHpSet5,
                        };

                        int hp_max = BattleParam.m_PlayerParty.m_HPMax.getValue((GlobalDefine.PartyCharaIndex)idx);
                        setHpButtonText(unit_hp_buttons[idx], 1, hp_max);
                    }
                }
                break;

            case "ButtonUnitHpSet1":
                openHpDialog(false, (int)GlobalDefine.PartyCharaIndex.LEADER, buttonTextUnitHpSet1);
                break;
            case "ButtonUnitHpSet2":
                openHpDialog(false, (int)GlobalDefine.PartyCharaIndex.MOB_1, buttonTextUnitHpSet2);
                break;
            case "ButtonUnitHpSet3":
                openHpDialog(false, (int)GlobalDefine.PartyCharaIndex.MOB_2, buttonTextUnitHpSet3);
                break;
            case "ButtonUnitHpSet4":
                openHpDialog(false, (int)GlobalDefine.PartyCharaIndex.MOB_3, buttonTextUnitHpSet4);
                break;
            case "ButtonUnitHpSet5":
                openHpDialog(false, (int)GlobalDefine.PartyCharaIndex.FRIEND, buttonTextUnitHpSet5);
                break;

            case "ButtonUnitBuffAll":
                buttonTextUnitBuffAll.Value = !buttonTextUnitBuffAll.Value;

                buttonTextUnitBuff1.Value = buttonTextUnitBuffAll.Value;
                buttonTextUnitBuff2.Value = buttonTextUnitBuffAll.Value;
                buttonTextUnitBuff3.Value = buttonTextUnitBuffAll.Value;
                buttonTextUnitBuff4.Value = buttonTextUnitBuffAll.Value;
                buttonTextUnitBuff5.Value = buttonTextUnitBuffAll.Value;
                break;

            case "ButtonUnitBuff1":
                buttonTextUnitBuff1.Value = !buttonTextUnitBuff1.Value;
                break;
            case "ButtonUnitBuff2":
                buttonTextUnitBuff2.Value = !buttonTextUnitBuff2.Value;
                break;
            case "ButtonUnitBuff3":
                buttonTextUnitBuff3.Value = !buttonTextUnitBuff3.Value;
                break;
            case "ButtonUnitBuff4":
                buttonTextUnitBuff4.Value = !buttonTextUnitBuff4.Value;
                break;
            case "ButtonUnitBuff5":
                buttonTextUnitBuff5.Value = !buttonTextUnitBuff5.Value;
                break;

            case "ButtonEnemyHpAll":
            case "ButtonEnemyHp0":
            case "ButtonEnemyHp1":
            case "ButtonEnemyHp2":
            case "ButtonEnemyHp3":
            case "ButtonEnemyHp4":
            case "ButtonEnemyHp5":
            case "ButtonEnemyHp6":
            case "ButtonEnemyHp7":
                {
                    M4uProperty<HpButtonState> hp_state = null;
                    switch (object_name)
                    {
                        case "ButtonEnemyHpAll":
                            hp_state = buttonTextEnemyHpAll;
                            break;

                        case "ButtonEnemyHp0":
                            if (buttonEnemyHp0.Value)
                            {
                                hp_state = buttonTextEnemyHp0;
                            }
                            break;

                        case "ButtonEnemyHp1":
                            if (buttonEnemyHp1.Value)
                            {
                                hp_state = buttonTextEnemyHp1;
                            }
                            break;

                        case "ButtonEnemyHp2":
                            if (buttonEnemyHp2.Value)
                            {
                                hp_state = buttonTextEnemyHp2;
                            }
                            break;

                        case "ButtonEnemyHp3":
                            if (buttonEnemyHp3.Value)
                            {
                                hp_state = buttonTextEnemyHp3;
                            }
                            break;

                        case "ButtonEnemyHp4":
                            if (buttonEnemyHp4.Value)
                            {
                                hp_state = buttonTextEnemyHp4;
                            }
                            break;

                        case "ButtonEnemyHp5":
                            if (buttonEnemyHp5.Value)
                            {
                                hp_state = buttonTextEnemyHp5;
                            }
                            break;

                        case "ButtonEnemyHp6":
                            if (buttonEnemyHp6.Value)
                            {
                                hp_state = buttonTextEnemyHp6;
                            }
                            break;

                        case "ButtonEnemyHp7":
                            if (buttonEnemyHp7.Value)
                            {
                                hp_state = buttonTextEnemyHp7;
                            }
                            break;
                    }

                    if (hp_state != null)
                    {
                        hp_state.Value = (HpButtonState)(((int)(hp_state.Value + 1)) % ((int)HpButtonState.MAX));
                    }

                    if (object_name == "ButtonEnemyHpAll")
                    {
                        if (buttonEnemyHp0.Value)
                        {
                            buttonTextEnemyHp0.Value = buttonTextEnemyHpAll.Value;
                        }

                        if (buttonEnemyHp1.Value)
                        {
                            buttonTextEnemyHp1.Value = buttonTextEnemyHpAll.Value;
                        }

                        if (buttonEnemyHp2.Value)
                        {
                            buttonTextEnemyHp2.Value = buttonTextEnemyHpAll.Value;
                        }

                        if (buttonEnemyHp3.Value)
                        {
                            buttonTextEnemyHp3.Value = buttonTextEnemyHpAll.Value;
                        }

                        if (buttonEnemyHp4.Value)
                        {
                            buttonTextEnemyHp4.Value = buttonTextEnemyHpAll.Value;
                        }

                        if (buttonEnemyHp5.Value)
                        {
                            buttonTextEnemyHp5.Value = buttonTextEnemyHpAll.Value;
                        }

                        if (buttonEnemyHp6.Value)
                        {
                            buttonTextEnemyHp6.Value = buttonTextEnemyHpAll.Value;
                        }

                        if (buttonEnemyHp7.Value)
                        {
                            buttonTextEnemyHp7.Value = buttonTextEnemyHpAll.Value;
                        }
                    }
                }
                break;

            case "ButtonEnemyHpMaxAll":
                for (int idx = 0; idx < BattleParam.m_EnemyParam.Length; idx++)
                {
                    BattleEnemy battle_enemy = BattleParam.m_EnemyParam[idx];
                    if (battle_enemy.m_EnemyHP > 0)
                    {
                        M4uProperty<string>[] unit_hp_buttons =
                        {
                            buttonTextEnemyHpSet0,
                            buttonTextEnemyHpSet1,
                            buttonTextEnemyHpSet2,
                            buttonTextEnemyHpSet3,
                            buttonTextEnemyHpSet4,
                            buttonTextEnemyHpSet5,
                            buttonTextEnemyHpSet6,
                            buttonTextEnemyHpSet7,
                        };

                        setHpButtonText(unit_hp_buttons[idx], battle_enemy.m_EnemyHPMax, battle_enemy.m_EnemyHPMax);
                    }
                }
                break;

            case "ButtonEnemyDeadlyAll":
                for (int idx = 0; idx < BattleParam.m_EnemyParam.Length; idx++)
                {
                    BattleEnemy battle_enemy = BattleParam.m_EnemyParam[idx];
                    if (battle_enemy.m_EnemyHP > 0)
                    {
                        M4uProperty<string>[] unit_hp_buttons =
                        {
                            buttonTextEnemyHpSet0,
                            buttonTextEnemyHpSet1,
                            buttonTextEnemyHpSet2,
                            buttonTextEnemyHpSet3,
                            buttonTextEnemyHpSet4,
                            buttonTextEnemyHpSet5,
                            buttonTextEnemyHpSet6,
                            buttonTextEnemyHpSet7,
                        };

                        setHpButtonText(unit_hp_buttons[idx], 1, battle_enemy.m_EnemyHPMax);
                    }
                }
                break;

            case "ButtonEnemyHpSet0":
                openHpDialog(true, 0, buttonTextEnemyHpSet0);
                break;
            case "ButtonEnemyHpSet1":
                openHpDialog(true, 1, buttonTextEnemyHpSet1);
                break;
            case "ButtonEnemyHpSet2":
                openHpDialog(true, 2, buttonTextEnemyHpSet2);
                break;
            case "ButtonEnemyHpSet3":
                openHpDialog(true, 3, buttonTextEnemyHpSet3);
                break;
            case "ButtonEnemyHpSet4":
                openHpDialog(true, 4, buttonTextEnemyHpSet4);
                break;
            case "ButtonEnemyHpSet5":
                openHpDialog(true, 5, buttonTextEnemyHpSet5);
                break;
            case "ButtonEnemyHpSet6":
                openHpDialog(true, 6, buttonTextEnemyHpSet6);
                break;
            case "ButtonEnemyHpSet7":
                openHpDialog(true, 7, buttonTextEnemyHpSet7);
                break;

            case "ButtonEnemyBuffAll":
                buttonTextEnemyBuffAll.Value = !buttonTextEnemyBuffAll.Value;

                if (buttonEnemyBuff0.Value)
                {
                    buttonTextEnemyBuff0.Value = buttonTextEnemyBuffAll.Value;
                }

                if (buttonEnemyBuff1.Value)
                {
                    buttonTextEnemyBuff1.Value = buttonTextEnemyBuffAll.Value;
                }

                if (buttonEnemyBuff2.Value)
                {
                    buttonTextEnemyBuff2.Value = buttonTextEnemyBuffAll.Value;
                }

                if (buttonEnemyBuff3.Value)
                {
                    buttonTextEnemyBuff3.Value = buttonTextEnemyBuffAll.Value;
                }

                if (buttonEnemyBuff4.Value)
                {
                    buttonTextEnemyBuff4.Value = buttonTextEnemyBuffAll.Value;
                }

                if (buttonEnemyBuff5.Value)
                {
                    buttonTextEnemyBuff5.Value = buttonTextEnemyBuffAll.Value;
                }

                if (buttonEnemyBuff6.Value)
                {
                    buttonTextEnemyBuff6.Value = buttonTextEnemyBuffAll.Value;
                }

                if (buttonEnemyBuff7.Value)
                {
                    buttonTextEnemyBuff7.Value = buttonTextEnemyBuffAll.Value;
                }
                break;

            case "ButtonEnemyBuff0":
                buttonTextEnemyBuff0.Value = !buttonTextEnemyBuff0.Value;
                break;

            case "ButtonEnemyBuff1":
                buttonTextEnemyBuff1.Value = !buttonTextEnemyBuff1.Value;
                break;

            case "ButtonEnemyBuff2":
                buttonTextEnemyBuff2.Value = !buttonTextEnemyBuff2.Value;
                break;

            case "ButtonEnemyBuff3":
                buttonTextEnemyBuff3.Value = !buttonTextEnemyBuff3.Value;
                break;

            case "ButtonEnemyBuff4":
                buttonTextEnemyBuff4.Value = !buttonTextEnemyBuff4.Value;
                break;

            case "ButtonEnemyBuff5":
                buttonTextEnemyBuff5.Value = !buttonTextEnemyBuff5.Value;
                break;

            case "ButtonEnemyBuff6":
                buttonTextEnemyBuff6.Value = !buttonTextEnemyBuff6.Value;
                break;

            case "ButtonEnemyBuff7":
                buttonTextEnemyBuff7.Value = !buttonTextEnemyBuff7.Value;
                break;


            case "InputFieldBuffID":
                updateBuffInfo(0);
                break;

            case "ButtonBuffPrev":
                updateBuffInfo(-1);
                break;

            case "ButtonBuffNext":
                updateBuffInfo(1);
                break;

            case "SliderBuffTurn":
                updateBuffTurnSlider();
                break;

            case "ButtonBuffFriend":
            case "ButtonBuffEnemyAll":
            case "ButtonBuffEnemyTarget":
                if (m_CurrentMasterDataStatusAilmentParam != null)
                {
                    int ailment_id = (int)m_CurrentMasterDataStatusAilmentParam.fix_id;
                    // 発動者はリーダとして状態異常を付与
                    CharaOnce caster_chara_once = BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.LEADER, CharaParty.CharaCondition.EXIST);
                    if (caster_chara_once != null)
                    {
                        int atk = caster_chara_once.m_CharaPow;
                        switch (object_name)
                        {
                            case "ButtonBuffFriend":
                                BattleParam.m_PlayerParty.m_Ailments.AddStatusAilmentToPlayerParty(GlobalDefine.PartyCharaIndex.MAX, ailment_id, atk, BattleParam.m_PlayerParty.m_HPMax);

                                // ターン数を書き換え
                                for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
                                {
                                    CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.ALIVE);
                                    if (chara_once != null)
                                    {
                                        StatusAilmentChara ailment_chara = BattleParam.m_PlayerParty.m_Ailments.getAilment((GlobalDefine.PartyCharaIndex)idx);
                                        if (ailment_chara != null)
                                        {
                                            for (int ailment_idx = 0; ailment_idx < ailment_chara.GetAilmentCount(); ailment_idx++)
                                            {
                                                StatusAilment ailment = ailment_chara.GetAilment(ailment_idx);
                                                if (ailment != null
                                                    && ailment.nMasterDataStatusAilmentID == ailment_id
                                                    && ailment.nLife == m_CurrentMasterDataStatusAilmentParam.duration)
                                                {
                                                    ailment.nLife = m_AilmentTurn;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：パーティの状態変化を変更");
                                break;

                            case "ButtonBuffEnemyAll":
                                for (int idx = 0; idx < BattleParam.m_EnemyParam.Length; idx++)
                                {
                                    BattleEnemy battle_enemy = BattleParam.m_EnemyParam[idx];
                                    if (battle_enemy.m_EnemyHP > 0)
                                    {
                                        battle_enemy.m_StatusAilmentChara.AddStatusAilment(ailment_id, atk, battle_enemy.m_EnemyHPMax, battle_enemy.getMasterDataParamChara());
                                    }

                                    // ターン数を書き換え
                                    for (int ailment_idx = 0; ailment_idx < battle_enemy.m_StatusAilmentChara.GetAilmentCount(); ailment_idx++)
                                    {
                                        StatusAilment ailment = battle_enemy.m_StatusAilmentChara.GetAilment(ailment_idx);
                                        if (ailment != null
                                            && ailment.nMasterDataStatusAilmentID == ailment_id
                                            && ailment.nLife == m_CurrentMasterDataStatusAilmentParam.duration)
                                        {
                                            ailment.nLife = m_AilmentTurn;
                                            break;
                                        }
                                    }
                                }

                                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：敵の状態変化を変更");
                                break;

                            case "ButtonBuffEnemyTarget":
                                if (BattleParam.m_TargetEnemyCurrent >= 0 && BattleParam.m_TargetEnemyCurrent < BattleParam.m_EnemyParam.Length)
                                {
                                    BattleEnemy battle_enemy = BattleParam.m_EnemyParam[BattleParam.m_TargetEnemyCurrent];
                                    if (battle_enemy != null && battle_enemy.m_EnemyHP > 0)
                                    {
                                        battle_enemy.m_StatusAilmentChara.AddStatusAilment(ailment_id, atk, battle_enemy.m_EnemyHPMax, battle_enemy.getMasterDataParamChara());
                                    }

                                    // ターン数を書き換え
                                    for (int ailment_idx = 0; ailment_idx < battle_enemy.m_StatusAilmentChara.GetAilmentCount(); ailment_idx++)
                                    {
                                        StatusAilment ailment = battle_enemy.m_StatusAilmentChara.GetAilment(ailment_idx);
                                        if (ailment != null
                                            && ailment.nMasterDataStatusAilmentID == ailment_id
                                            && ailment.nLife == m_CurrentMasterDataStatusAilmentParam.duration)
                                        {
                                            ailment.nLife = m_AilmentTurn;
                                            break;
                                        }
                                    }
                                }

                                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：敵の状態変化を変更");
                                break;
                        }
                    }
                }
                break;

            case "ButtonClearMemory":
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                break;

            case "InputFieldHpValue":
                {
                    int hp = _int_Parse(m_HpDialogInputField.text);
                    updateDialog(hp);
                }
                break;

            case "SliderHpPercent":
                {
                    int hp = (int)m_HpDialogSlider.value;
                    updateDialog(hp);
                }
                break;

            case "InputFieldHpValue2":
                {
                    int hp_percent = _int_Parse(m_HpDialogInputField2.text);
                    updateDialogByEnemyActionHpPercent(hp_percent);
                }
                break;

            case "SliderHpPercent2":
                {
                    int hp_percent = (int)m_HpDialogSlider2.value;
                    updateDialogByEnemyActionHpPercent(hp_percent);
                }
                break;

            case "ButtonHpDialogCancel":
                closeHpDialog(false);
                break;

            case "ButtonHpDialogDecide":
                closeHpDialog(true);
                break;

            case "DropdownEnemyAction":
            case "DropdownTableIndex":
                setupEnemeyActionSub();
                break;


            case "InputFieldEnemyAbility1":
            case "InputFieldEnemyAbility2":
            case "InputFieldEnemyAbility3":
            case "InputFieldEnemyAbility4":
            case "InputFieldEnemyAbility5":
            case "InputFieldEnemyAbility6":
            case "InputFieldEnemyAbility7":
            case "InputFieldEnemyAbility8":
            case "InputFieldEnemyAbilityActionAppear":
                {
                    MasterDataParamEnemy enemy_master = getDropdownEnemyActionEnemyMaster();

                    if (enemy_master != null)
                    {
                        enemy_master.ability1 = _uint_Parse(getInputFieldValue("InputFieldEnemyAbility1"));
                        enemy_master.ability2 = _uint_Parse(getInputFieldValue("InputFieldEnemyAbility2"));
                        enemy_master.ability3 = _uint_Parse(getInputFieldValue("InputFieldEnemyAbility3"));
                        enemy_master.ability4 = _uint_Parse(getInputFieldValue("InputFieldEnemyAbility4"));
                        enemy_master.ability5 = _uint_Parse(getInputFieldValue("InputFieldEnemyAbility5"));
                        enemy_master.ability6 = _uint_Parse(getInputFieldValue("InputFieldEnemyAbility6"));
                        enemy_master.ability7 = _uint_Parse(getInputFieldValue("InputFieldEnemyAbility7"));
                        enemy_master.ability8 = _uint_Parse(getInputFieldValue("InputFieldEnemyAbility8"));
                        enemy_master.act_first = _int_Parse(getInputFieldValue("InputFieldEnemyAbilityActionAppear"));
                    }
                }
                setupEnemeyActionSub();
                break;

            case "InputFieldEnemyAbilityTableID":
                {
                    MasterDataParamEnemy enemy_master = getDropdownEnemyActionEnemyMaster();
                    if (enemy_master != null)
                    {
                        int value = _int_Parse(getInputFieldValue("InputFieldEnemyAbilityTableID"));
                        switch (m_DropdownEnemyActionTable.value)
                        {
                            case 0:
                                enemy_master.act_table1 = value;
                                break;

                            case 1:
                                enemy_master.act_table2 = value;
                                break;

                            case 2:
                                enemy_master.act_table3 = value;
                                break;

                            case 3:
                                enemy_master.act_table4 = value;
                                break;

                            case 4:
                                enemy_master.act_table5 = value;
                                break;

                            case 5:
                                enemy_master.act_table6 = value;
                                break;

                            case 6:
                                enemy_master.act_table7 = value;
                                break;

                            case 7:
                                enemy_master.act_table8 = value;
                                break;
                        }
                    }
                }
                setupEnemeyActionSub();
                break;

            case "InputFieldEnemyAbilityAction1":
            case "InputFieldEnemyAbilityAction2":
            case "InputFieldEnemyAbilityAction3":
            case "InputFieldEnemyAbilityAction4":
            case "InputFieldEnemyAbilityAction5":
            case "InputFieldEnemyAbilityAction6":
            case "InputFieldEnemyAbilityAction7":
            case "InputFieldEnemyAbilityAction8":
                {
                    int table_id = _int_Parse(getInputFieldValue("InputFieldEnemyAbilityTableID"));
                    MasterDataEnemyActionTable enemy_action_table_master = BattleParam.m_MasterDataCache.useEnemyActionTable((uint)table_id);
                    if (enemy_action_table_master != null)
                    {
                        enemy_action_table_master.action_param_id1 = _int_Parse(getInputFieldValue("InputFieldEnemyAbilityAction1"));
                        enemy_action_table_master.action_param_id2 = _int_Parse(getInputFieldValue("InputFieldEnemyAbilityAction2"));
                        enemy_action_table_master.action_param_id3 = _int_Parse(getInputFieldValue("InputFieldEnemyAbilityAction3"));
                        enemy_action_table_master.action_param_id4 = _int_Parse(getInputFieldValue("InputFieldEnemyAbilityAction4"));
                        enemy_action_table_master.action_param_id5 = _int_Parse(getInputFieldValue("InputFieldEnemyAbilityAction5"));
                        enemy_action_table_master.action_param_id6 = _int_Parse(getInputFieldValue("InputFieldEnemyAbilityAction6"));
                        enemy_action_table_master.action_param_id7 = _int_Parse(getInputFieldValue("InputFieldEnemyAbilityAction7"));
                        enemy_action_table_master.action_param_id8 = _int_Parse(getInputFieldValue("InputFieldEnemyAbilityAction8"));
                    }
                }
                setupEnemeyActionSub();
                break;

            case "SliderPanelPutCount":
                updateAutoPlaySlider();
                break;

            case "ButtonAutoPlaySkillRandom":
                randomAutoPlaySkill();
                break;

            case "ButtonAutoPlaySkillClear":
                clearAutoPlaySkill();
                break;
        }
    }

    private MasterDataDefineLabel.ElementType nextCard(MasterDataDefineLabel.ElementType element_type)
    {
        element_type++;
        if (element_type >= MasterDataDefineLabel.ElementType.MAX)
        {
            element_type = MasterDataDefineLabel.ElementType.NONE + 1;
        }

        return element_type;
    }

    private MasterDataDefineLabel.ElementType nextCard2(MasterDataDefineLabel.ElementType element_type)
    {
        element_type++;
        if (element_type >= MasterDataDefineLabel.ElementType.MAX)
        {
            element_type = MasterDataDefineLabel.ElementType.NONE;
        }

        return element_type;
    }

    private void setupPanelBattle()
    {
#if BUILD_TYPE_DEBUG
        setDropdownValue("PanelBattle/Dropdown", (int)BattleParam.m_DebugBattleStartType);
        toggleSkill100.Value = BattleParam.m_DebugForce100PercentSkill;
        toggleEnemyDamage1000.Value = BattleParam.m_DebugEnemyDamage1000;
#endif //BUILD_TYPE_DEBUG
    }

    private void decidePanelBattle()
    {
#if BUILD_TYPE_DEBUG
        BattleParam.m_DebugBattleStartType = (BattleParam.DebugBattleStartType)getDropdownValue("PanelBattle/Dropdown");
        bool is_skill_100 = getToggleValue("PanelBattle/ToggleSkill100");
        BattleParam.m_DebugForce100PercentSkill = is_skill_100;

        bool is_enemy_damage_1000 = getToggleValue("PanelBattle/ToggleEnemyDamage1000");
        BattleParam.m_DebugEnemyDamage1000 = is_enemy_damage_1000;
#endif //BUILD_TYPE_DEBUG
    }

    private void setupPanelHand()
    {
        if (BattleSceneManager.Instance != null)
        {
            buttonHandLL.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCardElement(0);
            buttonHandL.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCardElement(1);
            buttonHandC.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCardElement(2);
            buttonHandR.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCardElement(3);
            buttonHandRR.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCardElement(4);

            buttonNextLL.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_NextArea.getCardElement(0);
            buttonNextL.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_NextArea.getCardElement(1);
            buttonNextC.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_NextArea.getCardElement(2);
            buttonNextR.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_NextArea.getCardElement(3);
            buttonNextRR.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_NextArea.getCardElement(4);
        }
    }

    private void decidePanelHand()
    {
        if (BattleSceneManager.Instance != null)
        {
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCard(0).setElementType(buttonHandLL.Value, BattleScene.BattleCard.ChangeCause.NONE);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCard(1).setElementType(buttonHandL.Value, BattleScene.BattleCard.ChangeCause.NONE);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCard(2).setElementType(buttonHandC.Value, BattleScene.BattleCard.ChangeCause.NONE);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCard(3).setElementType(buttonHandR.Value, BattleScene.BattleCard.ChangeCause.NONE);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCard(4).setElementType(buttonHandRR.Value, BattleScene.BattleCard.ChangeCause.NONE);

            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_NextArea.getCard(0).setElementType(buttonNextLL.Value, BattleScene.BattleCard.ChangeCause.NONE);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_NextArea.getCard(1).setElementType(buttonNextL.Value, BattleScene.BattleCard.ChangeCause.NONE);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_NextArea.getCard(2).setElementType(buttonNextC.Value, BattleScene.BattleCard.ChangeCause.NONE);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_NextArea.getCard(3).setElementType(buttonNextR.Value, BattleScene.BattleCard.ChangeCause.NONE);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_NextArea.getCard(4).setElementType(buttonNextRR.Value, BattleScene.BattleCard.ChangeCause.NONE);

#if BUILD_TYPE_DEBUG
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_DebugNextCardFix[0] = MasterDataDefineLabel.ElementType.NONE;
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_DebugNextCardFix[1] = MasterDataDefineLabel.ElementType.NONE;
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_DebugNextCardFix[2] = MasterDataDefineLabel.ElementType.NONE;
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_DebugNextCardFix[3] = MasterDataDefineLabel.ElementType.NONE;
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_DebugNextCardFix[4] = MasterDataDefineLabel.ElementType.NONE;

            if (getToggleValue("PanelHand/ToggleNextFixLL"))
            {
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_DebugNextCardFix[0] = buttonNextLL.Value;
            }
            if (getToggleValue("PanelHand/ToggleNextFixL"))
            {
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_DebugNextCardFix[1] = buttonNextL.Value;
            }
            if (getToggleValue("PanelHand/ToggleNextFixC"))
            {
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_DebugNextCardFix[2] = buttonNextC.Value;
            }
            if (getToggleValue("PanelHand/ToggleNextFixR"))
            {
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_DebugNextCardFix[3] = buttonNextR.Value;
            }
            if (getToggleValue("PanelHand/ToggleNextFixRR"))
            {
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_DebugNextCardFix[4] = buttonNextRR.Value;
            }
#endif

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：手札・次札変更");
        }
    }

    private void setupPanelField()
    {
        if (BattleSceneManager.Instance != null)
        {
            buttonField1LL.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(0).getCardElement(0);
            buttonField2LL.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(0).getCardElement(1);
            buttonField3LL.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(0).getCardElement(2);
            buttonField4LL.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(0).getCardElement(3);
            buttonField5LL.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(0).getCardElement(4);

            buttonField1L.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(1).getCardElement(0);
            buttonField2L.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(1).getCardElement(1);
            buttonField3L.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(1).getCardElement(2);
            buttonField4L.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(1).getCardElement(3);
            buttonField5L.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(1).getCardElement(4);

            buttonField1C.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(2).getCardElement(0);
            buttonField2C.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(2).getCardElement(1);
            buttonField3C.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(2).getCardElement(2);
            buttonField4C.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(2).getCardElement(3);
            buttonField5C.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(2).getCardElement(4);

            buttonField1R.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(3).getCardElement(0);
            buttonField2R.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(3).getCardElement(1);
            buttonField3R.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(3).getCardElement(2);
            buttonField4R.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(3).getCardElement(3);
            buttonField5R.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(3).getCardElement(4);

            buttonField1RR.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(4).getCardElement(0);
            buttonField2RR.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(4).getCardElement(1);
            buttonField3RR.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(4).getCardElement(2);
            buttonField4RR.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(4).getCardElement(3);
            buttonField5RR.Value = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(4).getCardElement(4);

            if (buttonBoostLL.Value != BoostType.KEEP)
            {
                buttonBoostLL.Value = (BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[0]) ? BoostType.ON : BoostType.OFF;
            }
            if (buttonBoostL.Value != BoostType.KEEP)
            {
                buttonBoostL.Value = (BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[1]) ? BoostType.ON : BoostType.OFF;
            }
            if (buttonBoostC.Value != BoostType.KEEP)
            {
                buttonBoostC.Value = (BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[2]) ? BoostType.ON : BoostType.OFF;
            }
            if (buttonBoostR.Value != BoostType.KEEP)
            {
                buttonBoostR.Value = (BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[3]) ? BoostType.ON : BoostType.OFF;
            }
            if (buttonBoostRR.Value != BoostType.KEEP)
            {
                buttonBoostRR.Value = (BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[4]) ? BoostType.ON : BoostType.OFF;
            }
        }
    }

    private void decidePanelField()
    {
#if BUILD_TYPE_DEBUG
        if (BattleSceneManager.Instance != null)
        {
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(0).reset();
            _addFieldCard(0, buttonField1LL.Value);
            _addFieldCard(0, buttonField2LL.Value);
            _addFieldCard(0, buttonField3LL.Value);
            _addFieldCard(0, buttonField4LL.Value);
            _addFieldCard(0, buttonField5LL.Value);

            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(1).reset();
            _addFieldCard(1, buttonField1L.Value);
            _addFieldCard(1, buttonField2L.Value);
            _addFieldCard(1, buttonField3L.Value);
            _addFieldCard(1, buttonField4L.Value);
            _addFieldCard(1, buttonField5L.Value);

            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(2).reset();
            _addFieldCard(2, buttonField1C.Value);
            _addFieldCard(2, buttonField2C.Value);
            _addFieldCard(2, buttonField3C.Value);
            _addFieldCard(2, buttonField4C.Value);
            _addFieldCard(2, buttonField5C.Value);

            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(3).reset();
            _addFieldCard(3, buttonField1R.Value);
            _addFieldCard(3, buttonField2R.Value);
            _addFieldCard(3, buttonField3R.Value);
            _addFieldCard(3, buttonField4R.Value);
            _addFieldCard(3, buttonField5R.Value);

            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(4).reset();
            _addFieldCard(4, buttonField1RR.Value);
            _addFieldCard(4, buttonField2RR.Value);
            _addFieldCard(4, buttonField3RR.Value);
            _addFieldCard(4, buttonField4RR.Value);
            _addFieldCard(4, buttonField5RR.Value);

            BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostFieldDebugKeep[0] = (buttonBoostLL.Value == BoostType.KEEP);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostFieldDebugKeep[1] = (buttonBoostL.Value == BoostType.KEEP);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostFieldDebugKeep[2] = (buttonBoostC.Value == BoostType.KEEP);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostFieldDebugKeep[3] = (buttonBoostR.Value == BoostType.KEEP);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostFieldDebugKeep[4] = (buttonBoostRR.Value == BoostType.KEEP);

            BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[0] = (buttonBoostLL.Value == BoostType.ON || buttonBoostLL.Value == BoostType.KEEP);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[1] = (buttonBoostL.Value == BoostType.ON || buttonBoostL.Value == BoostType.KEEP);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[2] = (buttonBoostC.Value == BoostType.ON || buttonBoostC.Value == BoostType.KEEP);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[3] = (buttonBoostR.Value == BoostType.ON || buttonBoostR.Value == BoostType.KEEP);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[4] = (buttonBoostRR.Value == BoostType.ON || buttonBoostRR.Value == BoostType.KEEP);

            // BOOST表示を更新
            for (int idx = 0; idx < BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField.Length; idx++)
            {
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(idx).m_IsBoost = BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[idx];
            }

            BattleSceneManager.Instance.PRIVATE_FIELD.updateResurrectInfoView();

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：場札・ブースト状態を変更");
        }
#endif
    }

    private void _addFieldCard(int field_index, MasterDataDefineLabel.ElementType element_type)
    {
        if (element_type != MasterDataDefineLabel.ElementType.NONE)
        {
            BattleScene.BattleCard battle_card = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.getUnusedCard();
            if (battle_card != null)
            {
                battle_card.setElementType(element_type, BattleScene.BattleCard.ChangeCause.NONE);
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(field_index).addCard(battle_card);
            }
        }
    }

    private void setupUnitState()
    {
        if (BattleSceneManager.Instance != null)
        {
            M4uProperty<HpButtonState>[] hp_states =
            {
                buttonTextUnitHp1,
                buttonTextUnitHp2,
                buttonTextUnitHp3,
                buttonTextUnitHp4,
                buttonTextUnitHp5,
            };

            M4uProperty<string>[] unit_hp_buttons =
            {
                buttonTextUnitHpSet1,
                buttonTextUnitHpSet2,
                buttonTextUnitHpSet3,
                buttonTextUnitHpSet4,
                buttonTextUnitHpSet5,
            };

            M4uProperty<string>[] unit_names =
            {
                textUnitName1,
                textUnitName2,
                textUnitName3,
                textUnitName4,
                textUnitName5,
            };

            M4uProperty<bool>[] buff_states =
            {
                buttonTextUnitBuff1,
                buttonTextUnitBuff2,
                buttonTextUnitBuff3,
                buttonTextUnitBuff4,
                buttonTextUnitBuff5,
            };

            M4uProperty<int>[] unit_hates =
            {
                inputFieldHate1,
                inputFieldHate2,
                inputFieldHate3,
                inputFieldHate4,
                inputFieldHate5,
            };

            for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
            {
                M4uProperty<HpButtonState> hp_state = hp_states[idx];

                hp_state.Value = HpButtonState.NORMAL;
#if BUILD_TYPE_DEBUG
                if (BattleParam.m_PlayerParty.m_DebugNoDeadFlag.getValue((GlobalDefine.PartyCharaIndex)idx) > 0)
                {
                    hp_state.Value = HpButtonState.NO_DEAD;
                }
#endif

                M4uProperty<string> unit_name = unit_names[idx];
                unit_name.Value = "";
                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.EXIST);
                if (chara_once != null)
                {
                    unit_name.Value = chara_once.m_CharaMasterDataParam.name;

                    int hp_max = BattleParam.m_PlayerParty.m_HPMax.getValue((GlobalDefine.PartyCharaIndex)idx);
                    int hp_current = BattleParam.m_PlayerParty.m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx);

                    setHpButtonText(unit_hp_buttons[idx], hp_current, hp_max);
                }

                buff_states[idx].Value = false;

                int hate_value = BattleParam.m_PlayerParty.m_Hate.getValue((GlobalDefine.PartyCharaIndex)idx);
                unit_hates[idx].Value = hate_value;
            }
        }
    }

    private void decideUnitState()
    {
        if (BattleSceneManager.Instance != null)
        {
            M4uProperty<HpButtonState>[] hp_states =
            {
                buttonTextUnitHp1,
                buttonTextUnitHp2,
                buttonTextUnitHp3,
                buttonTextUnitHp4,
                buttonTextUnitHp5,
            };

            M4uProperty<string>[] unit_hp_buttons =
            {
                buttonTextUnitHpSet1,
                buttonTextUnitHpSet2,
                buttonTextUnitHpSet3,
                buttonTextUnitHpSet4,
                buttonTextUnitHpSet5,
            };

            M4uProperty<bool>[] buff_states =
            {
                buttonTextUnitBuff1,
                buttonTextUnitBuff2,
                buttonTextUnitBuff3,
                buttonTextUnitBuff4,
                buttonTextUnitBuff5,
            };

            buttonTextUnitBuffAll.Value = false;

            for (int idx = 0; idx < hp_states.Length; idx++)
            {
                if (BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.EXIST) == null)
                {
                    continue;
                }

                int new_hp_current = getHpFromHpButtonText(unit_hp_buttons[idx]);

                BattleParam.m_PlayerParty.m_HPCurrent.setValue((GlobalDefine.PartyCharaIndex)idx, new_hp_current);

                switch (hp_states[idx].Value)
                {
                    case HpButtonState.NORMAL:
#if BUILD_TYPE_DEBUG
                        BattleParam.m_PlayerParty.m_DebugNoDeadFlag.setValue((GlobalDefine.PartyCharaIndex)idx, 0);
#endif
                        break;

                    case HpButtonState.NO_DEAD:
                        if (BattleParam.m_PlayerParty.m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx) <= 0)
                        {
                            BattleParam.m_PlayerParty.m_HPCurrent.setValue((GlobalDefine.PartyCharaIndex)idx, 1);
                        }

#if BUILD_TYPE_DEBUG
                        BattleParam.m_PlayerParty.m_DebugNoDeadFlag.setValue((GlobalDefine.PartyCharaIndex)idx, 1);
#endif
                        break;
                }

                if (buff_states[idx].Value)
                {
                    BattleParam.m_PlayerParty.m_Ailments.getAilment((GlobalDefine.PartyCharaIndex)idx).DelAllStatusAilment();
                }

                {
                    string obj_path = "Canvas/PanelCharaHP/Unit" + (idx + 1).ToString() + "/InputFieldHate" + (idx + 1).ToString();
                    Transform hate_trans = transform.Find(obj_path);
                    if (hate_trans != null)
                    {
                        InputField input_field = hate_trans.GetComponent<InputField>();
                        if (input_field != null)
                        {
                            int hate_value = _int_Parse(input_field.text);
                            BattleParam.m_PlayerParty.m_Hate.setValue((GlobalDefine.PartyCharaIndex)idx, hate_value);
                        }
                    }

                }
            }
            BattleParam.m_PlayerParty.m_Ailments.updatePartyAilment();
            BattleParam.m_PlayerParty.adjustHate();

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：ユニット状態を変更");

            // BattleLogic.OnGameInput() 内の初期化処理を動作させるための物.
            BattleLogicFSM.Instance.step(BattleLogic.EBATTLE_STEP.eBATTLE_STEP_MAX);
            BattleLogicFSM.Instance.step(BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT);
        }
    }

    private void setupEnemyState()
    {
        // 敵キャラクタは死亡・復活をデバッグメニューからは変更できないようにしている（単純にHPを書き換えただけでは正常に動作しないため）

        if (BattleSceneManager.Instance != null)
        {
            M4uProperty<HpButtonState>[] hp_states =
            {
                buttonTextEnemyHp0,
                buttonTextEnemyHp1,
                buttonTextEnemyHp2,
                buttonTextEnemyHp3,
                buttonTextEnemyHp4,
                buttonTextEnemyHp5,
                buttonTextEnemyHp6,
                buttonTextEnemyHp7,
            };

            M4uProperty<string>[] unit_hp_buttons =
            {
                buttonTextEnemyHpSet0,
                buttonTextEnemyHpSet1,
                buttonTextEnemyHpSet2,
                buttonTextEnemyHpSet3,
                buttonTextEnemyHpSet4,
                buttonTextEnemyHpSet5,
                buttonTextEnemyHpSet6,
                buttonTextEnemyHpSet7,
            };

            M4uProperty<bool>[] hp_button_enables =
            {
                buttonEnemyHp0,
                buttonEnemyHp1,
                buttonEnemyHp2,
                buttonEnemyHp3,
                buttonEnemyHp4,
                buttonEnemyHp5,
                buttonEnemyHp6,
                buttonEnemyHp7,
            };

            M4uProperty<bool>[] hp_button_set_enables =
            {
                buttonEnemyHpSet0,
                buttonEnemyHpSet1,
                buttonEnemyHpSet2,
                buttonEnemyHpSet3,
                buttonEnemyHpSet4,
                buttonEnemyHpSet5,
                buttonEnemyHpSet6,
                buttonEnemyHpSet7,
            };

            M4uProperty<bool>[] buff_states =
            {
                buttonTextEnemyBuff0,
                buttonTextEnemyBuff1,
                buttonTextEnemyBuff2,
                buttonTextEnemyBuff3,
                buttonTextEnemyBuff4,
                buttonTextEnemyBuff5,
                buttonTextEnemyBuff6,
                buttonTextEnemyBuff7,
            };

            M4uProperty<bool>[] buff_button_enables =
            {
                buttonEnemyBuff0,
                buttonEnemyBuff1,
                buttonEnemyBuff2,
                buttonEnemyBuff3,
                buttonEnemyBuff4,
                buttonEnemyBuff5,
                buttonEnemyBuff6,
                buttonEnemyBuff7,
            };

            M4uProperty<string>[] enemy_names =
            {
                textEnemyName0,
                textEnemyName1,
                textEnemyName2,
                textEnemyName3,
                textEnemyName4,
                textEnemyName5,
                textEnemyName6,
                textEnemyName7,
            };

            buttonTextEnemyBuffAll.Value = false;

            for (int idx = 0; idx < hp_states.Length; idx++)
            {
                BattleEnemy battle_enemy = null;
                if (idx < BattleParam.m_EnemyParam.Length)
                {
                    battle_enemy = BattleParam.m_EnemyParam[idx];
                }

                M4uProperty<HpButtonState> hp_state = hp_states[idx];
                M4uProperty<bool> hp_button_enable = hp_button_enables[idx];
                M4uProperty<bool> hp_button_set_enable = hp_button_set_enables[idx];
                M4uProperty<bool> buff_state = buff_states[idx];
                M4uProperty<bool> buff_button_enable = buff_button_enables[idx];
                M4uProperty<string> enemy_name = enemy_names[idx];

                hp_state.Value = HpButtonState.NORMAL;
                hp_button_enable.Value = false;
                buff_state.Value = false;
                buff_button_enable.Value = false;
                enemy_name.Value = "";

                if (battle_enemy != null)
                {
                    enemy_name.Value = battle_enemy.getMasterDataParamChara().name;

                    if (battle_enemy.m_EnemyHP > 0)
                    {
                        hp_button_enable.Value = true;
                        hp_button_set_enable.Value = true;
                        buff_button_enable.Value = true;
                    }
                    else
                    {
                        hp_button_enable.Value = false;
                        hp_button_set_enable.Value = false;
                        buff_button_enable.Value = false;
                    }

                    int hp_max = battle_enemy.m_EnemyHPMax;
                    int hp_current = battle_enemy.m_EnemyHP;

                    setHpButtonText(unit_hp_buttons[idx], hp_current, hp_max);

#if BUILD_TYPE_DEBUG
                    if (battle_enemy.m_DebugNoDeadFlag)
                    {
                        hp_state.Value = HpButtonState.NO_DEAD;
                    }
#endif
                }
            }
        }
    }

    private void decideEnemyState()
    {
        if (BattleSceneManager.Instance != null)
        {
            M4uProperty<HpButtonState>[] hp_states =
            {
                buttonTextEnemyHp0,
                buttonTextEnemyHp1,
                buttonTextEnemyHp2,
                buttonTextEnemyHp3,
                buttonTextEnemyHp4,
                buttonTextEnemyHp5,
                buttonTextEnemyHp6,
                buttonTextEnemyHp7,
            };

            M4uProperty<string>[] unit_hp_buttons =
            {
                buttonTextEnemyHpSet0,
                buttonTextEnemyHpSet1,
                buttonTextEnemyHpSet2,
                buttonTextEnemyHpSet3,
                buttonTextEnemyHpSet4,
                buttonTextEnemyHpSet5,
                buttonTextEnemyHpSet6,
                buttonTextEnemyHpSet7,
            };

            M4uProperty<bool>[] buff_states =
            {
                buttonTextEnemyBuff0,
                buttonTextEnemyBuff1,
                buttonTextEnemyBuff2,
                buttonTextEnemyBuff3,
                buttonTextEnemyBuff4,
                buttonTextEnemyBuff5,
                buttonTextEnemyBuff6,
                buttonTextEnemyBuff7,
            };

            for (int idx = 0; idx < hp_states.Length; idx++)
            {
                BattleEnemy battle_enemy = null;
                if (idx < BattleParam.m_EnemyParam.Length)
                {
                    battle_enemy = BattleParam.m_EnemyParam[idx];
                }

                if (battle_enemy != null)
                {
                    if (battle_enemy.m_EnemyHP > 0)
                    {
                        int new_hp_current = getHpFromHpButtonText(unit_hp_buttons[idx]);
                        if (new_hp_current < 1)
                        {
                            new_hp_current = 1;
                        }

                        battle_enemy.m_EnemyHP = new_hp_current;

                        switch (hp_states[idx].Value)
                        {
                            case HpButtonState.NORMAL:
#if BUILD_TYPE_DEBUG
                                battle_enemy.m_DebugNoDeadFlag = false;
#endif
                                break;

                            case HpButtonState.NO_DEAD:
#if BUILD_TYPE_DEBUG
                                battle_enemy.m_DebugNoDeadFlag = true;
#endif
                                break;
                        }
                    }

                    // 状態変化クリア
                    if (buff_states[idx].Value)
                    {
                        battle_enemy.m_StatusAilmentChara.DelAllStatusAilment();
                    }
                }
            }

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "デバッグ機能実行：敵の状態を変更");
        }
    }

    private void setupEnemyAction()
    {
        // 敵カスタマイズ機能で変更される可能性があるマスターデータは MasterDataParamEnemy と MasterDataEnemyActionTable
        // MasterDataParamEnemy はクエスト開始APIで取得したもので十分。書き換えても問題ない
        // MasterDataEnemyActionTable はクエスト開始APIで取得したも以外も使用するのでDebug_GetMasterDataAll2で取得。また書き換えられる可能性がある
        // MasterDataEnemyActionParam はクエスト開始APIで取得したも以外も使用するのでDebug_GetMasterDataAll2で取得。書き換えられることはない
        // MasterDataEnemyAbility はクライアント内に持っているデータなので取得しない。書き換えられることもない

        panelDownloading.Value = true;

        // 敵関連のマスターを読み込み
        if (m_EnemyMasterDownloadState == EnemyMasterDownloadState.NONE)
        {
            m_EnemyMasterDownloadState = EnemyMasterDownloadState.LOADING;

            // マスターデータのサーバーからの取得は重いのでこのメニューが開かれるまでは行わない
            Dictionary<EMASTERDATA, uint> dict = new Dictionary<EMASTERDATA, uint>();
            dict.Add(EMASTERDATA.eMASTERDATA_ENEMY_ACTION_TABLE, 0);
            dict.Add(EMASTERDATA.eMASTERDATA_ENEMY_ACTION_PARAM, 0);
            ServerDataUtilSend.SendPacketAPI_Debug_GetMasterDataAll2(dict).
                setSuccessAction(
                    _data =>
                    {
                        ServerDataDefine.RecvMasterDataAll2Value result = _data.GetResult<ServerDataDefine.RecvMasterDataAll2>().result;
                        MasterDataEnemyActionTable[] master_enemy_action_table_array = result.master_array_enemy_action_table.upd_list;
                        MasterDataEnemyActionParam[] master_enemy_action_param_array = result.master_array_enemy_action_param.upd_list;
                        m_EnemyMasterDownloadState = EnemyMasterDownloadState.READY;
                        setupEnemyActionSub(master_enemy_action_table_array, master_enemy_action_param_array);
                    }
                ).
                setErrorAction(
                    _date =>
                    {
                        m_EnemyMasterDownloadState = EnemyMasterDownloadState.ERROR;
#if BUILD_TYPE_DEBUG
                        Debug.Log("ERROR");
#endif
                        setupEnemyActionSub(null, null);
                    }
                ).
                SendStart();
        }
        else
        {
            setupEnemyActionSub(null, null);
        }
    }

    private void setupEnemyActionSub(MasterDataEnemyActionTable[] master_enemy_action_table_array, MasterDataEnemyActionParam[] master_enemy_action_param_array)
    {
        // MasterDataEnemyActionTable はクエスト開始ＡＰＩで取得したデータをDebugGetMasterAll2で取得したデータへ上書き
        if (master_enemy_action_table_array != null)
        {
            for (int idx1 = 0; idx1 < SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild.list_e_acttable.Length; idx1++)
            {
                MasterDataEnemyActionTable enemy_action_table_master1 = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild.list_e_acttable[idx1];
                uint fix_id1 = enemy_action_table_master1.fix_id;

                for (int idx2 = 0; idx2 < master_enemy_action_table_array.Length; idx2++)
                {
                    MasterDataEnemyActionTable enemy_action_table_master2 = master_enemy_action_table_array[idx2];
                    uint fix_id2 = enemy_action_table_master2.fix_id;

                    if (fix_id1 == fix_id2)
                    {
                        master_enemy_action_table_array[idx2] = enemy_action_table_master1;
                        break;
                    }
                }
            }
        }

        if (master_enemy_action_table_array != null)
        {
            BattleParam.m_MasterDataCache.setDebugAllDataEnemyActionTable(master_enemy_action_table_array);
        }

        if (master_enemy_action_param_array != null)
        {
            BattleParam.m_MasterDataCache.setDebugAllDataEnemyActionParam(master_enemy_action_param_array);
        }

        if (m_DropdownEnemyActionEnemy != null)
        {
            m_DropdownEnemyActionEnemy.ClearOptions();

            List<string> enemys = new List<string>();

            // 現在のクエストの敵
            if (SceneGoesParam.HasInstance)
            {
                for (int idx = 0; idx < SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild.list_e_param.Length; idx++)
                {
                    MasterDataParamEnemy enemy_master = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild.list_e_param[idx];
                    if (enemy_master != null)
                    {
                        MasterDataParamChara chara_master = MasterFinder<MasterDataParamChara>.Instance.Find((int)enemy_master.chara_id);
                        if (chara_master != null)
                        {
                            string text = enemy_master.fix_id.ToString() + ":" + chara_master.name;
                            enemys.Add(text);
                        }
                    }
                }
            }

            m_DropdownEnemyActionEnemy.AddOptions(enemys);
            m_DropdownEnemyActionEnemy.value = 0;
            m_DropdownEnemyActionTable.value = 0;

            panelDownloading.Value = false;

            setupEnemeyActionSub();
        }
    }


    private void decideEnemyAction()
    {

    }

    private MasterDataParamEnemy getDropdownEnemyActionEnemyMaster()
    {
        MasterDataParamEnemy enemy_master = null;

        string dropdown_option_text = m_DropdownEnemyActionEnemy.options[m_DropdownEnemyActionEnemy.value].text;
        if (dropdown_option_text != null)
        {
            string enemy_fix_id_text = dropdown_option_text.Substring(0, dropdown_option_text.IndexOf(":"));
            uint enemy_fix_id = _uint_Parse(enemy_fix_id_text);

            enemy_master = BattleParam.m_MasterDataCache.useEnemyParam(enemy_fix_id);
        }

        return enemy_master;
    }

    private void setupEnemeyActionSub()
    {
        if (m_IsExecutingSetupEnemeyActionSub)
        {
            return;
        }
        m_IsExecutingSetupEnemeyActionSub = true;

        MasterDataParamEnemy enemy_master = getDropdownEnemyActionEnemyMaster();
        if (enemy_master != null)
        {
            inputFieldEnemyAbility1.Value = enemy_master.ability1.ToString();
            if (enemy_master.ability1 != 0)
            {
                MasterDataEnemyAbility enemy_ability = BattleParam.m_MasterDataCache.useEnemyAbility(enemy_master.ability1);
                if (enemy_ability != null)
                {
                    textColorEnemyAbility1.Value = Color.black;
                    textEnemyAbility1.Value = enemy_ability.name;
                }
                else
                {
                    textColorEnemyAbility1.Value = Color.red;
                    textEnemyAbility1.Value = "";
                }
            }
            else
            {
                textColorEnemyAbility1.Value = Color.blue;
                textEnemyAbility1.Value = "";
            }

            inputFieldEnemyAbility2.Value = enemy_master.ability2.ToString();
            if (enemy_master.ability2 != 0)
            {
                MasterDataEnemyAbility enemy_ability = BattleParam.m_MasterDataCache.useEnemyAbility(enemy_master.ability2);
                if (enemy_ability != null)
                {
                    textColorEnemyAbility2.Value = Color.black;
                    textEnemyAbility2.Value = enemy_ability.name;
                }
                else
                {
                    textColorEnemyAbility2.Value = Color.red;
                    textEnemyAbility2.Value = "";
                }
            }
            else
            {
                textColorEnemyAbility2.Value = Color.blue;
                textEnemyAbility2.Value = "";
            }

            inputFieldEnemyAbility3.Value = enemy_master.ability3.ToString();
            if (enemy_master.ability3 != 0)
            {
                MasterDataEnemyAbility enemy_ability = BattleParam.m_MasterDataCache.useEnemyAbility(enemy_master.ability3);
                if (enemy_ability != null)
                {
                    textColorEnemyAbility3.Value = Color.black;
                    textEnemyAbility3.Value = enemy_ability.name;
                }
                else
                {
                    textColorEnemyAbility3.Value = Color.red;
                    textEnemyAbility3.Value = "";
                }
            }
            else
            {
                textColorEnemyAbility3.Value = Color.blue;
                textEnemyAbility3.Value = "";
            }

            inputFieldEnemyAbility4.Value = enemy_master.ability4.ToString();
            if (enemy_master.ability4 != 0)
            {
                MasterDataEnemyAbility enemy_ability = BattleParam.m_MasterDataCache.useEnemyAbility(enemy_master.ability4);
                if (enemy_ability != null)
                {
                    textColorEnemyAbility4.Value = Color.black;
                    textEnemyAbility4.Value = enemy_ability.name;
                }
                else
                {
                    textColorEnemyAbility4.Value = Color.red;
                    textEnemyAbility4.Value = "";
                }
            }
            else
            {
                textColorEnemyAbility4.Value = Color.blue;
                textEnemyAbility4.Value = "";
            }

            inputFieldEnemyAbility5.Value = enemy_master.ability5.ToString();
            if (enemy_master.ability5 != 0)
            {
                MasterDataEnemyAbility enemy_ability = BattleParam.m_MasterDataCache.useEnemyAbility(enemy_master.ability5);
                if (enemy_ability != null)
                {
                    textColorEnemyAbility5.Value = Color.black;
                    textEnemyAbility5.Value = enemy_ability.name;
                }
                else
                {
                    textColorEnemyAbility5.Value = Color.red;
                    textEnemyAbility5.Value = "";
                }
            }
            else
            {
                textColorEnemyAbility5.Value = Color.blue;
                textEnemyAbility5.Value = "";
            }

            inputFieldEnemyAbility6.Value = enemy_master.ability6.ToString();
            if (enemy_master.ability6 != 0)
            {
                MasterDataEnemyAbility enemy_ability = BattleParam.m_MasterDataCache.useEnemyAbility(enemy_master.ability6);
                if (enemy_ability != null)
                {
                    textColorEnemyAbility6.Value = Color.black;
                    textEnemyAbility6.Value = enemy_ability.name;
                }
                else
                {
                    textColorEnemyAbility6.Value = Color.red;
                    textEnemyAbility6.Value = "";
                }
            }
            else
            {
                textColorEnemyAbility6.Value = Color.blue;
                textEnemyAbility6.Value = "";
            }

            inputFieldEnemyAbility7.Value = enemy_master.ability7.ToString();
            if (enemy_master.ability7 != 0)
            {
                MasterDataEnemyAbility enemy_ability = BattleParam.m_MasterDataCache.useEnemyAbility(enemy_master.ability7);
                if (enemy_ability != null)
                {
                    textColorEnemyAbility7.Value = Color.black;
                    textEnemyAbility7.Value = enemy_ability.name;
                }
                else
                {
                    textColorEnemyAbility7.Value = Color.red;
                    textEnemyAbility7.Value = "";
                }
            }
            else
            {
                textColorEnemyAbility7.Value = Color.blue;
                textEnemyAbility7.Value = "";
            }

            inputFieldEnemyAbility8.Value = enemy_master.ability8.ToString();
            if (enemy_master.ability8 != 0)
            {
                MasterDataEnemyAbility enemy_ability = BattleParam.m_MasterDataCache.useEnemyAbility(enemy_master.ability8);
                if (enemy_ability != null)
                {
                    textColorEnemyAbility8.Value = Color.black;
                    textEnemyAbility8.Value = enemy_ability.name;
                }
                else
                {
                    textColorEnemyAbility8.Value = Color.red;
                    textEnemyAbility8.Value = "";
                }
            }
            else
            {
                textColorEnemyAbility8.Value = Color.blue;
                textEnemyAbility8.Value = "";
            }

            inputFieldEnemyAbilityActionAppear.Value = enemy_master.act_first.ToString();
            if (enemy_master.act_first != 0)
            {
                MasterDataEnemyActionParam enemy_action_param = BattleParam.m_MasterDataCache.useEnemyActionParam((uint)enemy_master.act_first);
                if (enemy_action_param != null)
                {
                    inputFieldEnemyAbilityActionAppearTextColor.Value = Color.black;
                }
                else
                {
                    inputFieldEnemyAbilityActionAppearTextColor.Value = Color.red;
                }
            }
            else
            {
                inputFieldEnemyAbilityActionAppearTextColor.Value = Color.blue;
            }

            int[] table_indexs =
            {
                enemy_master.act_table1,
                enemy_master.act_table2,
                enemy_master.act_table3,
                enemy_master.act_table4,
                enemy_master.act_table5,
                enemy_master.act_table6,
                enemy_master.act_table7,
                enemy_master.act_table8,
            };
            int action_table_id = table_indexs[m_DropdownEnemyActionTable.value];
            if (action_table_id != 0)
            {
                inputFieldEnemyAbilityTableID.Value = action_table_id.ToString();

                MasterDataEnemyActionTable action_table_master = BattleParam.m_MasterDataCache.useEnemyActionTable((uint)action_table_id);

                if (action_table_master != null)
                {
                    inputFieldEnemyAbilityTableIDTextColor.Value = Color.black;

                    inputFieldEnemyAbilityAction1.Value = action_table_master.action_param_id1.ToString();
                    inputFieldEnemyAbilityAction2.Value = action_table_master.action_param_id2.ToString();
                    inputFieldEnemyAbilityAction3.Value = action_table_master.action_param_id3.ToString();
                    inputFieldEnemyAbilityAction4.Value = action_table_master.action_param_id4.ToString();
                    inputFieldEnemyAbilityAction5.Value = action_table_master.action_param_id5.ToString();
                    inputFieldEnemyAbilityAction6.Value = action_table_master.action_param_id6.ToString();
                    inputFieldEnemyAbilityAction7.Value = action_table_master.action_param_id7.ToString();
                    inputFieldEnemyAbilityAction8.Value = action_table_master.action_param_id8.ToString();
                }
                else
                {
                    inputFieldEnemyAbilityTableIDTextColor.Value = Color.red;

                    inputFieldEnemyAbilityAction1.Value = "0";
                    inputFieldEnemyAbilityAction2.Value = "0";
                    inputFieldEnemyAbilityAction3.Value = "0";
                    inputFieldEnemyAbilityAction4.Value = "0";
                    inputFieldEnemyAbilityAction5.Value = "0";
                    inputFieldEnemyAbilityAction6.Value = "0";
                    inputFieldEnemyAbilityAction7.Value = "0";
                    inputFieldEnemyAbilityAction8.Value = "0";
                }
            }
            else
            {
                inputFieldEnemyAbilityTableID.Value = "0";
                inputFieldEnemyAbilityTableIDTextColor.Value = Color.blue;

                inputFieldEnemyAbilityAction1.Value = "0";
                inputFieldEnemyAbilityAction2.Value = "0";
                inputFieldEnemyAbilityAction3.Value = "0";
                inputFieldEnemyAbilityAction4.Value = "0";
                inputFieldEnemyAbilityAction5.Value = "0";
                inputFieldEnemyAbilityAction6.Value = "0";
                inputFieldEnemyAbilityAction7.Value = "0";
                inputFieldEnemyAbilityAction8.Value = "0";
            }


            M4uProperty<string>[] input_field_enemy_ability_actions =
            {
                inputFieldEnemyAbilityAction1,
                inputFieldEnemyAbilityAction2,
                inputFieldEnemyAbilityAction3,
                inputFieldEnemyAbilityAction4,
                inputFieldEnemyAbilityAction5,
                inputFieldEnemyAbilityAction6,
                inputFieldEnemyAbilityAction7,
                inputFieldEnemyAbilityAction8,
            };

            M4uProperty<Color>[] input_field_enemy_ability_action_text_colors =
            {
                inputFieldEnemyAbilityActionTextColor1,
                inputFieldEnemyAbilityActionTextColor2,
                inputFieldEnemyAbilityActionTextColor3,
                inputFieldEnemyAbilityActionTextColor4,
                inputFieldEnemyAbilityActionTextColor5,
                inputFieldEnemyAbilityActionTextColor6,
                inputFieldEnemyAbilityActionTextColor7,
                inputFieldEnemyAbilityActionTextColor8,
            };

            for (int idx = 0; idx < input_field_enemy_ability_actions.Length; idx++)
            {
                uint action_id = _uint_Parse(input_field_enemy_ability_actions[idx].Value);
                Color color;
                if (action_id == 0)
                {
                    color = Color.blue;
                }
                else
                {
                    MasterDataEnemyActionParam enemy_action_param_master = BattleParam.m_MasterDataCache.useEnemyActionParam(action_id);
                    if (enemy_action_param_master != null)
                    {
                        color = Color.black;
                    }
                    else
                    {
                        color = Color.red;
                    }
                }

                input_field_enemy_ability_action_text_colors[idx].Value = color;
            }
        }
        m_IsExecutingSetupEnemeyActionSub = false;
    }

    private void setupBuff()
    {
        updateBuffInfo(0);
    }

    private void decideBuff()
    {

    }

    private void setupDispSetting()
    {
    }

    private void decideDispSetting()
    {
        if (m_TextEnemyInfo != null)
        {
            m_IsDispMask = getToggleValue("PanelDispSetting/ToggleDispMask");
            m_IsDispTime = getToggleValue("PanelDispSetting/ToggleDispTime");
            m_IsDispTurn = getToggleValue("PanelDispSetting/ToggleDispTurn");
            m_IsDispEnemyInfo = getToggleValue("PanelDispSetting/ToggleDispInfo");
            m_IsDispEnemyAbility = getToggleValue("PanelDispSetting/ToggleDispEnemyAbility");
            m_IsDispEnemyAilment = getToggleValue("PanelDispSetting/ToggleDispEnemyAilment");

            bool is_active = m_IsDispTime || m_IsDispTurn || m_IsDispEnemyInfo || m_IsDispEnemyAbility || m_IsDispEnemyAilment;
            m_TextEnemyInfo.gameObject.SetActive(is_active);

            if (m_TextEnemyInfoMask != null)
            {
                m_TextEnemyInfoMask.gameObject.SetActive(m_IsDispMask);
            }
        }

        m_IsDispDamagePlayer = getToggleValue("PanelDispSetting/ToggleDispDamagePlayer");
        m_IsDispDamageEnemy = getToggleValue("PanelDispSetting/ToggleDispDamageEnemy");

        speedButtons.Value = getToggleValue("PanelDispSetting/ToggleSpeedButton");

        initDispTouchPosition(getToggleValue("PanelDispSetting/ToggleDispTouchPosition"));
    }

    private const int AUTO_PLAY_PUT_COUNT_DEFAULT = 9;
    private int m_AutoPlayPutCount = AUTO_PLAY_PUT_COUNT_DEFAULT;
    private MasterDataDefineLabel.AutoPlaySkillType[][] m_AutoPlaySkills = null;
    private const string DEBUG_AUTO_PLAY_KEY = "DbgAutoplay_";

    private void initAutoPlay()
    {
        m_AutoPlayPutCount = AUTO_PLAY_PUT_COUNT_DEFAULT;

        if (m_DropdownAutoPlaySkills.IsNullOrEmpty() == false)
        {
            List<string> skill_types = new List<string>();
            for (MasterDataDefineLabel.AutoPlaySkillType skill_idx = MasterDataDefineLabel.AutoPlaySkillType.NONE; skill_idx < MasterDataDefineLabel.AutoPlaySkillType.MAX; skill_idx++)
            {
                string skill_text = MasterDataDefineLabel.Debug_AutoPlaySkillTypeNames[(int)skill_idx];
                skill_types.Add(skill_text);
            }

            for (int idx = 0; idx < m_DropdownAutoPlaySkills.Length; idx++)
            {
                Dropdown dropdown = m_DropdownAutoPlaySkills[idx];
                if (dropdown != null)
                {
                    dropdown.ClearOptions();
                    dropdown.AddOptions(skill_types);
                }
            }

            m_AutoPlaySkills = new MasterDataDefineLabel.AutoPlaySkillType[(int)GlobalDefine.PartyCharaIndex.MAX][];
            for (int member_idx = 0; member_idx < (int)GlobalDefine.PartyCharaIndex.MAX; member_idx++)
            {
                m_AutoPlaySkills[member_idx] = new MasterDataDefineLabel.AutoPlaySkillType[m_DropdownAutoPlaySkills.Length / (int)GlobalDefine.PartyCharaIndex.MAX];
            }
        }

        if (m_AutoPlaySkills != null)
        {
            m_AutoPlayPutCount = PlayerPrefs.GetInt(DEBUG_AUTO_PLAY_KEY + "Num", m_AutoPlayPutCount);

            int dropdown_idx = 0;
            for (int member_idx = 0; member_idx < (int)GlobalDefine.PartyCharaIndex.MAX; member_idx++)
            {
                MasterDataDefineLabel.AutoPlaySkillType[] skills = m_AutoPlaySkills[member_idx];
                for (int skill_idx = 0; skill_idx < skills.Length; skill_idx++)
                {
                    MasterDataDefineLabel.AutoPlaySkillType skill_type = skills[skill_idx];
                    skill_type = (MasterDataDefineLabel.AutoPlaySkillType)PlayerPrefs.GetInt(DEBUG_AUTO_PLAY_KEY + dropdown_idx.ToString(), (int)skill_type);
                    skills[skill_idx] = skill_type;

                    dropdown_idx++;
                }
            }
        }
    }

    private void setupAutoPlay()
    {
        if (m_AutoPlaySkills != null)
        {
            m_SliderAutoPlayPanelPutCount.value = m_AutoPlayPutCount;

            int dropdown_idx = 0;
            for (int member_idx = 0; member_idx < (int)GlobalDefine.PartyCharaIndex.MAX; member_idx++)
            {
                MasterDataDefineLabel.AutoPlaySkillType[] skills = m_AutoPlaySkills[member_idx];
                for (int skill_idx = 0; skill_idx < skills.Length; skill_idx++)
                {
                    MasterDataDefineLabel.AutoPlaySkillType skill_type = skills[skill_idx];

                    Dropdown dropdown = m_DropdownAutoPlaySkills[dropdown_idx];
                    dropdown_idx++;
                    if (dropdown != null)
                    {
                        dropdown.value = (int)skill_type;
                    }
                }
            }

            updateAutoPlaySlider();
        }
    }

    private void updateAutoPlaySlider()
    {
        if (m_SliderAutoPlayPanelPutCount != null)
        {
            int panel_put_count = (int)m_SliderAutoPlayPanelPutCount.value;
            if (panel_put_count > 0)
            {
                textPanelPutCount.Value = "配置枚数\n" + panel_put_count.ToString();
            }
            else
            {
                textPanelPutCount.Value = "配置枚数\n初期値";
            }
        }
    }

    private void decideAutoPlay()
    {
        if (m_AutoPlaySkills != null)
        {
            m_AutoPlayPutCount = (int)m_SliderAutoPlayPanelPutCount.value;
            PlayerPrefs.SetInt(DEBUG_AUTO_PLAY_KEY + "Num", m_AutoPlayPutCount);

            int dropdown_idx = 0;
            for (int member_idx = 0; member_idx < (int)GlobalDefine.PartyCharaIndex.MAX; member_idx++)
            {
                MasterDataDefineLabel.AutoPlaySkillType[] skills = m_AutoPlaySkills[member_idx];
                for (int skill_idx = 0; skill_idx < skills.Length; skill_idx++)
                {
                    MasterDataDefineLabel.AutoPlaySkillType skill_type = MasterDataDefineLabel.AutoPlaySkillType.NONE;
                    Dropdown dropdown = m_DropdownAutoPlaySkills[dropdown_idx];
                    dropdown_idx++;
                    if (dropdown != null)
                    {
                        skill_type = (MasterDataDefineLabel.AutoPlaySkillType)dropdown.value;
                    }
                    skills[skill_idx] = skill_type;

                    PlayerPrefs.SetInt(DEBUG_AUTO_PLAY_KEY + (dropdown_idx - 1).ToString(), (int)skill_type);
                }
            }
        }
    }

    private void randomAutoPlaySkill()
    {
        if (m_AutoPlaySkills != null)
        {
            int dropdown_idx = 0;
            for (int member_idx = 0; member_idx < (int)GlobalDefine.PartyCharaIndex.MAX; member_idx++)
            {
                MasterDataDefineLabel.AutoPlaySkillType[] skills = m_AutoPlaySkills[member_idx];
                int skill_num = UnityEngine.Random.Range(1, skills.Length + 1);
                for (int skill_idx = 0; skill_idx < skills.Length; skill_idx++)
                {
                    MasterDataDefineLabel.AutoPlaySkillType skill_type = MasterDataDefineLabel.AutoPlaySkillType.NONE;
                    if (skill_idx < skill_num)
                    {
                        skill_type = (MasterDataDefineLabel.AutoPlaySkillType)UnityEngine.Random.Range(1, (int)MasterDataDefineLabel.AutoPlaySkillType.MAX);
                    }

                    Dropdown dropdown = m_DropdownAutoPlaySkills[dropdown_idx];
                    dropdown_idx++;
                    if (dropdown != null)
                    {
                        dropdown.value = (int)skill_type;
                    }
                }
            }
        }
    }

    private void clearAutoPlaySkill()
    {
        if (m_AutoPlaySkills != null)
        {
            m_SliderAutoPlayPanelPutCount.value = AUTO_PLAY_PUT_COUNT_DEFAULT;

            int dropdown_idx = 0;
            for (int member_idx = 0; member_idx < (int)GlobalDefine.PartyCharaIndex.MAX; member_idx++)
            {
                MasterDataDefineLabel.AutoPlaySkillType[] skills = m_AutoPlaySkills[member_idx];
                for (int skill_idx = 0; skill_idx < skills.Length; skill_idx++)
                {
                    MasterDataDefineLabel.AutoPlaySkillType skill_type = MasterDataDefineLabel.AutoPlaySkillType.NONE;

                    Dropdown dropdown = m_DropdownAutoPlaySkills[dropdown_idx];
                    dropdown_idx++;
                    if (dropdown != null)
                    {
                        dropdown.value = (int)skill_type;
                    }
                }
            }
        }
    }

    public static int getAutoPlayPanelPutCount()
    {
        int ret_val = AUTO_PLAY_PUT_COUNT_DEFAULT;

        if (m_Instance != null)
        {
            ret_val = m_Instance.m_AutoPlayPutCount;
            ret_val = Mathf.Min(ret_val, 25);
            ret_val = Mathf.Max(ret_val, 1);
        }

        return ret_val;
    }

    public static MasterDataDefineLabel.AutoPlaySkillType[] getAutoPlaySkill(GlobalDefine.PartyCharaIndex member_index)
    {
        if (m_Instance != null)
        {
            if (m_Instance.m_AutoPlaySkills != null)
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER
                    && member_index < GlobalDefine.PartyCharaIndex.MAX
                )
                {
                    return m_Instance.m_AutoPlaySkills[(int)member_index];
                }
            }
        }

        return null;
    }


    private bool getToggleValue(string toggle_name)
    {
        bool is_on = false;

        Toggle toggle = findComponent<Toggle>(toggle_name);
        if (toggle != null)
        {
            is_on = toggle.isOn;
        }

        return is_on;
    }

    private void setToggleValue(string toggle_name, bool is_on)
    {
        Toggle toggle = findComponent<Toggle>(toggle_name);
        if (toggle != null)
        {
            toggle.isOn = is_on;
        }
    }

    private int getDropdownValue(string dropdown_name)
    {
        int value = 0;

        Dropdown dropdown = findComponent<Dropdown>(dropdown_name);
        if (dropdown != null)
        {
            value = dropdown.value;
        }

        return value;
    }

    private void setDropdownValue(string dropdown_name, int value)
    {
        Dropdown dropdown = findComponent<Dropdown>(dropdown_name);
        if (dropdown != null)
        {
            dropdown.value = value;
        }
    }

    private string getInputFieldValue(string input_field_name)
    {
        string ret_val = null;

        InputField input_field = findComponent<InputField>(input_field_name);
        if (input_field != null)
        {
            ret_val = input_field.text;
        }

        return ret_val;
    }

    private T findComponent<T>(string game_object_name) where T : Component
    {
        T ret_val = default(T);

        T[] components = transform.GetComponentsInChildren<T>();
        foreach (T component in components)
        {
            string full_path = component.gameObject.FullPath();
            int index = full_path.LastIndexOf(game_object_name);
            if (index >= 0)
            {
                if (full_path.Length - game_object_name.Length == index)
                {
                    ret_val = component;
                    break;
                }
            }
        }

        return ret_val;
    }

    private MasterDataStatusAilmentParam[] m_MasterDataStatusAilmentParams = null;
    private MasterDataStatusAilmentParam m_CurrentMasterDataStatusAilmentParam = null;
    private int m_AilmentTurn = 0;
    private void updateBuffInfo(int change)
    {
        Transform trans = transform.Find("Canvas/PanelBuff/InputFieldBuffID");
        if (trans != null)
        {
            InputField input_field = trans.GetComponent<InputField>();
            if (input_field != null)
            {
                if (m_MasterDataStatusAilmentParams == null)
                {
                    m_MasterDataStatusAilmentParams = new MasterDataStatusAilmentParam[0];	// 次のGetAll()を失敗したときに何回もGetAll()しにいかなようにするためにnullでなくす.

                    m_MasterDataStatusAilmentParams = MasterFinder<MasterDataStatusAilmentParam>.Instance.GetAll();
                    if (m_MasterDataStatusAilmentParams != null)
                    {
                        List<MasterDataStatusAilmentParam> aaa = new List<MasterDataStatusAilmentParam>(m_MasterDataStatusAilmentParams);
                        aaa.Sort((a, b) => (int)a.fix_id - (int)b.fix_id);
                        m_MasterDataStatusAilmentParams = aaa.ToArray();
                    }
                }

                if (m_MasterDataStatusAilmentParams.Length > 0)
                {
                    int buff_fix_id = 0;
                    if (input_field.text != "")
                    {
                        buff_fix_id = _int_Parse(input_field.text);
                    }
                    buff_fix_id = buff_fix_id + change;
                    if (m_CurrentMasterDataStatusAilmentParam != null && m_CurrentMasterDataStatusAilmentParam.fix_id != buff_fix_id)
                    {
                        m_CurrentMasterDataStatusAilmentParam = null;
                    }

                    if (m_CurrentMasterDataStatusAilmentParam == null)
                    {
                        if (change >= 0)
                        {
                            for (int idx = 0; idx < m_MasterDataStatusAilmentParams.Length; idx++)
                            {
                                if (m_MasterDataStatusAilmentParams[idx].fix_id >= buff_fix_id)
                                {
                                    m_CurrentMasterDataStatusAilmentParam = m_MasterDataStatusAilmentParams[idx];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int idx = m_MasterDataStatusAilmentParams.Length - 1; idx >= 0; idx--)
                            {
                                if (m_MasterDataStatusAilmentParams[idx].fix_id <= buff_fix_id)
                                {
                                    m_CurrentMasterDataStatusAilmentParam = m_MasterDataStatusAilmentParams[idx];
                                    break;
                                }
                            }
                        }
                    }

                    if (m_CurrentMasterDataStatusAilmentParam == null)
                    {
                        if (buff_fix_id > m_MasterDataStatusAilmentParams[m_MasterDataStatusAilmentParams.Length - 1].fix_id)
                        {
                            m_CurrentMasterDataStatusAilmentParam = m_MasterDataStatusAilmentParams[0];
                        }
                        else
                        if (buff_fix_id < m_MasterDataStatusAilmentParams[0].fix_id)
                        {
                            m_CurrentMasterDataStatusAilmentParam = m_MasterDataStatusAilmentParams[m_MasterDataStatusAilmentParams.Length - 1];
                        }
                    }

                    if (m_CurrentMasterDataStatusAilmentParam != null)
                    {
                        inputFieldBuffID.Value = (int)m_CurrentMasterDataStatusAilmentParam.fix_id;
                        textBuffName.Value = m_CurrentMasterDataStatusAilmentParam.name + "[" + m_CurrentMasterDataStatusAilmentParam.category.ToString() + "]";
                    }

                    updateBuffTurnSlider();
                }
            }
        }
    }

    private void updateBuffTurnSlider()
    {
        if (m_CurrentMasterDataStatusAilmentParam != null)
        {
            Transform trans = transform.Find("Canvas/PanelBuff/SliderBuffTurn");
            if (trans != null)
            {
                Slider slider = trans.GetComponent<Slider>();
                if (slider != null)
                {
                    float slider_value = slider.value;

                    int turn_max = m_CurrentMasterDataStatusAilmentParam.duration;
                    m_AilmentTurn = (int)(slider_value * turn_max) + 1;
                    if (m_AilmentTurn > turn_max)
                    {
                        m_AilmentTurn = turn_max;
                    }

                    textBuffTurn.Value = "ターン数：" + m_AilmentTurn.ToString() + "/" + turn_max.ToString();
                }
            }
        }
    }

    public static void DispDamagePlayer(BattleEnemy enemy, BattleSceneUtil.MultiInt damage_value, BattleSceneUtil.MultiInt damage_target)
    {
        if (m_IsDispDamagePlayer && enemy != null)
        {
            string damage_text = "[";
            for (int idx = 0; idx < damage_value.getMemberCount(); idx++)
            {
                if (idx > 0)
                {
                    damage_text += "/";
                }

                if (damage_target.getValue((GlobalDefine.PartyCharaIndex)idx) > 0)
                {
                    damage_text += damage_value.getValue((GlobalDefine.PartyCharaIndex)idx).ToString();
                }
                else
                {
                    damage_text += "-";
                }
            }
            damage_text += "]";

            DebugLogger.StatAdd("<color=#00BFFF>DAMAGE_PLAYER_" + enemy.getName() + "\n" + damage_text + "</color>");
        }
    }

    public static void DispDamageEnemy(BattleSkillActivity activity, BattleEnemy target, int damage)
    {
        if (m_IsDispDamageEnemy && activity != null && target != null)
        {
            string skillName = activity.getMainText();
            string enemyName = target.getName();

            switch (activity.m_SkillType)
            {
                case ESKILLTYPE.eACTIVE:
                    break;

                case ESKILLTYPE.eLEADER:
                    break;

                case ESKILLTYPE.eLIMITBREAK:
                    break;

                case ESKILLTYPE.ePASSIVE:
                    break;

                case ESKILLTYPE.eBOOST:
                default:
                    skillName = "BOOSTSKILL";
                    break;
            }

            DebugLogger.StatAdd("<color=#ff0000>DAMAGE_" + enemyName + "_" + skillName + "\n" + damage.ToString() + "</color>");
        }
    }

#endif //BUILD_TYPE_DEBUG

    private uint _uint_Parse(string text)
    {
        uint ret_val = 0;
        try
        {
            ret_val = uint.Parse(text);
        }
        catch (Exception)
        {

        }

        return ret_val;
    }

    private int _int_Parse(string text)
    {
        int ret_val = 0;
        try
        {
            ret_val = int.Parse(text);
        }
        catch (Exception)
        {

        }

        return ret_val;
    }

    /// <summary>
    /// タッチ位置表示機能初期化処理
    /// </summary>
    /// <param name="is_enable"></param>
    private void initDispTouchPosition(bool is_enable)
    {
#if BUILD_TYPE_DEBUG
        if (is_enable)
        {
            if (m_DebugTouchPosition == null)
            {
                m_DebugTouchPosition = new GameObject[64];
                m_DebugTouchPositionIndex = 0;

                Sprite sprite = m_ElementSprites[(int)MasterDataDefineLabel.ElementType.NAUGHT];

                for (int idx = 0; idx < m_DebugTouchPosition.Length; idx++)
                {
                    GameObject obj = new GameObject("MOUSE_POINT");
                    obj.transform.SetParent(transform, false);
                    obj.layer = gameObject.layer;
                    obj.transform.localScale = Vector3.one * 0.004f;
                    SpriteRenderer sprite_renderer = obj.AddComponent<SpriteRenderer>();
                    if (sprite_renderer != null)
                    {
                        sprite_renderer.sprite = sprite;
                        sprite_renderer.sortingOrder = 100;
                    }
                    obj.SetActive(false);

                    m_DebugTouchPosition[idx] = obj;
                }
            }
        }
        else
        {
            if (m_DebugTouchPosition != null)
            {
                for (int idx = 0; idx < m_DebugTouchPosition.Length; idx++)
                {
                    GameObject obj = m_DebugTouchPosition[idx];
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
                m_DebugTouchPosition = null;
            }
        }
#endif
    }

    /// <summary>
    /// タッチ位置表示機能更新処理
    /// </summary>
    private void updateDispTouchPosition()
    {
#if BUILD_TYPE_DEBUG
        if (m_DebugTouchPosition != null)
        {
            if (BattleTouchInput.Instance.isTouchTriger())
            {
                m_DebugTouchPosition[m_DebugTouchPositionIndex].transform.position = BattleTouchInput.Instance.getWorldPosition(0.5f);
                m_DebugTouchPosition[m_DebugTouchPositionIndex].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 45.0f);
                m_DebugTouchPosition[m_DebugTouchPositionIndex].SetActive(true);
            }
            else if (BattleTouchInput.Instance.isTouchRelease())
            {
                m_DebugTouchPosition[m_DebugTouchPositionIndex].transform.position = BattleTouchInput.Instance.getWorldPosition(0.5f);
                m_DebugTouchPosition[m_DebugTouchPositionIndex].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                m_DebugTouchPosition[m_DebugTouchPositionIndex].SetActive(true);
            }
            else if (BattleTouchInput.Instance.isTouching())
            {
                m_DebugTouchPosition[m_DebugTouchPositionIndex].transform.position = BattleTouchInput.Instance.getWorldPosition(1.5f);
                m_DebugTouchPosition[m_DebugTouchPositionIndex].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                m_DebugTouchPosition[m_DebugTouchPositionIndex].SetActive(true);
            }
            else
            {
                m_DebugTouchPosition[m_DebugTouchPositionIndex].SetActive(false);
            }

            m_DebugTouchPositionIndex = (m_DebugTouchPositionIndex + 1) % m_DebugTouchPosition.Length;
        }
#endif
    }
}
