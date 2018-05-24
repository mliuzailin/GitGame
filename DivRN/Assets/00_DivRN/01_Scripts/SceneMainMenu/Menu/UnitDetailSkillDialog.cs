using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using DG.Tweening;
using ServerDataDefine;

public class UnitDetailSkillDialog : M4uContextMonoBehaviour
{
    private static readonly float FadeShowAlpha = 0.5f;
    private static readonly float FadeHideAlpha = 0.0f;
    private static readonly float WindowShowScale = 1.0f;
    private static readonly float WindowHideScale = 0.0f;
    private static readonly float AnimationTime = 0.25f;

    public MenuPartsBase Window = null;
    public GameObject ShadowPanel = null;
    public GameObject PanelRoot = null;

    private M4uProperty<bool> isViewDialog = new M4uProperty<bool>();
    public bool IsViewDialog { get { return isViewDialog.Value; } set { isViewDialog.Value = value; } }

    private M4uProperty<float> panelColor = new M4uProperty<float>();
    public float PanelColor { get { return panelColor.Value; } set { panelColor.Value = value; } }

    private bool m_Ready = false;
    private UnitSkillPanel m_UnitSkillPanel = null;
    private int skillPanelCount;

    private void Awake()
    {
        IsViewDialog = false;

        GetComponent<M4uContextRoot>().Context = this;

        if (SceneObjReferMainMenu.HasInstance)
        {
            m_UnitSkillPanel = UnityUtil.SetupPrefab<UnitSkillPanel>(SceneObjReferMainMenu.Instance.m_UnitSkillPanel, PanelRoot);
        }
        else
        {
            m_UnitSkillPanel = UnityUtil.SetupPrefab<UnitSkillPanel>("Prefab/UnitSkillPanel/UnitSkillPanel", PanelRoot);
        }

        Window.SetPositionAjustStatusBar(new Vector2(0, -17), new Vector2(-40, -446));


        m_UnitSkillPanel.SetPosition(new Vector2(0, -2), new Vector2(-20, -60), true);

        //m_UnitSkillPanel.transform.Find("Viewport/Content").localScale = new Vector3(0.75f, 0.75f, 1.0f);
    }

    public void Show(MasterDataParamChara charaMaster, PacketStructUnit _mainUnit = null, PacketStructUnit _subUnit = null)
    {
        IsViewDialog = true;
        PanelColor = 0;

        m_UnitSkillPanel.AllClear();
        m_UnitSkillPanel.AddLeaderSkill(charaMaster.skill_leader);
        if (_mainUnit == null)
        {
            m_UnitSkillPanel.AddLimitBreakSkill(charaMaster.skill_limitbreak, 0);
        }
        else
        {
            m_UnitSkillPanel.AddLimitBreakSkill(charaMaster.skill_limitbreak, (int)_mainUnit.limitbreak_lv);
        }
        m_UnitSkillPanel.AddActiveSkill(charaMaster.skill_active0);
        skillPanelCount = 3;
        if (charaMaster.skill_active1 != 0)
        {
            m_UnitSkillPanel.AddActiveSkill(charaMaster.skill_active1);
            ++skillPanelCount;
        }
        if (charaMaster.skill_passive != 0)
        {
            m_UnitSkillPanel.AddPassiveSkill(charaMaster.skill_passive);
            ++skillPanelCount;
        }
        m_UnitSkillPanel.setupChara(charaMaster.fix_id, _mainUnit);
        ShadowPanel.GetComponent<Image>().DOFade(FadeShowAlpha, AnimationTime);

        Window.transform.DOScaleY(WindowShowScale, AnimationTime).OnComplete(() =>
        {
            PanelColor = 1;
            m_Ready = true;
        });

        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを登録
            AndroidBackKeyManager.Instance.StackPush(gameObject, OnClose);
        }
    }

    public void Hide()
    {
        if (m_Ready == false)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを解除
            AndroidBackKeyManager.Instance.StackPop(gameObject);
        }

        ShadowPanel.GetComponent<Image>().DOFade(FadeHideAlpha, AnimationTime);

        m_UnitSkillPanel.AllClear();
        Window.transform.DOScaleY(WindowHideScale, AnimationTime).OnComplete(() =>
        {
            IsViewDialog = false;
            m_Ready = false;

        });

    }

    public void OnClose()
    {
        if (m_Ready == false)
        {
            return;
        }

        Hide();
    }
}
