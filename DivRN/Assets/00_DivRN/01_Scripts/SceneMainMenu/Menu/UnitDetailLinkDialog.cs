using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using DG.Tweening;
using ServerDataDefine;

public class UnitDetailLinkDialog : M4uContextMonoBehaviour
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
    private UnitLinkPanel m_UnitLinkPanel = null;

    private void Awake()
    {
        IsViewDialog = false;

        GetComponent<M4uContextRoot>().Context = this;

        if (SceneObjReferMainMenu.HasInstance)
        {
            m_UnitLinkPanel = UnityUtil.SetupPrefab<UnitLinkPanel>(SceneObjReferMainMenu.Instance.m_UnitLinkPanel, PanelRoot);
        }
        else
        {
            m_UnitLinkPanel = UnityUtil.SetupPrefab<UnitLinkPanel>("Prefab/UnitLinkPanel/UnitLinkPanel", PanelRoot);
        }

        Window.SetPositionAjustStatusBar(new Vector2(0, -17), new Vector2(-40, -446));


        m_UnitLinkPanel.SetPosition(new Vector2(0, -2), new Vector2(-20, -60), true);

        //m_UnitLinkPanel.transform.Find("Viewport/Content").localScale = new Vector3(0.75f, 0.75f, 1.0f);
    }

    public void Show(uint _unit_id, PacketStructUnit _mainUnit, PacketStructUnit _subUnit)
    {
        IsViewDialog = true;
        PanelColor = 0;

        if (_mainUnit != null)
        {
            m_UnitLinkPanel.setupUnit(_mainUnit, _subUnit, UnitLinkPanel.LinkParamType.Link);
        }

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

        m_UnitLinkPanel.AllClear();
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
