/**
 *  @file   PartySelectButtonPanel.cs
 *  @brief
 *  @author Developer
 *  @date   2017/02/14
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;

public class PartySelectButtonPanel : MenuPartsBase
{
    [SerializeField]
    private GameObject m_nextButtonRoot;
    [SerializeField]
    private GameObject m_memberButtonRoot;
    [SerializeField]
    private GameObject m_questMemberButtonRoot;
    [SerializeField]
    private GameObject m_autoPlayButtonRoot;

    private static readonly string NextButtonPrefabPath = "Prefab/PartySelectButtonPanel/PartySelectNextButton";
    private static readonly string MemberButtonPrefabPath = "Prefab/PartySelectButtonPanel/PartySelectMemberButton";
    private static readonly string AutoPlayButtonPrefabPath = "Prefab/PartySelectButtonPanel/PartySelectAutoPlayButton";

    /// <summary>ボタンを選択したときのアクション</summary>
    public Action NextAction = delegate { };
    public Action MemberSettingAction = delegate { };
    public Action AutoPlayAction = delegate { };

    public PartySelectMemberButton m_MemberButton;
    public ButtonModel m_MemberButtonModel;
    public PartySelectLinkButton m_LinkButton;

    M4uProperty<bool> isActiveMemberSettingButton = new M4uProperty<bool>(false);
    /// <summary>MemberSettingボタンの表示・非表示</summary>
    public bool IsActiveMemberSettingButton { get { return isActiveMemberSettingButton.Value; } set { isActiveMemberSettingButton.Value = value; } }

    M4uProperty<bool> isActiveNextButton = new M4uProperty<bool>(false);
    /// <summary>中央ボタンの表示・非表示</summary>
    public bool IsActiveNextButton { get { return isActiveNextButton.Value; } set { isActiveNextButton.Value = value; } }

    M4uProperty<string> memberSettingButtonText = new M4uProperty<string>();
    public string MemberSettingButtonText { get { return memberSettingButtonText.Value; } set { memberSettingButtonText.Value = value; } }

    M4uProperty<bool> isActiveAutoPlayOnButton = new M4uProperty<bool>(false);
    /// <summary>中央ボタンの表示・非表示</summary>
    public bool IsActiveAutoPlayOnButton { get { return isActiveAutoPlayOnButton.Value; } set { isActiveAutoPlayOnButton.Value = value; } }

    M4uProperty<bool> isActiveAutoPlayOffButton = new M4uProperty<bool>(false);
    /// <summary>中央ボタンの表示・非表示</summary>
    public bool IsActiveAutoPlayOffButton { get { return isActiveAutoPlayOffButton.Value; } set { isActiveAutoPlayOffButton.Value = value; } }

    M4uProperty<bool> isActiveAutoPlayNgButton = new M4uProperty<bool>(false);
    /// <summary>中央ボタンの表示・非表示</summary>
    public bool IsActiveAutoPlayNgButton { get { return isActiveAutoPlayNgButton.Value; } set { isActiveAutoPlayNgButton.Value = value; } }

    bool m_isSetupButtons = false;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_isSetupButtons = false;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickMemberSettingButton()
    {
        if (MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch())
        {
            return;
        }

        if (MemberSettingAction != null)
        {
            MemberSettingAction();
        }
    }

    public void OnClickNextButton()
    {
        if (MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch())
        {
            return;
        }

        if (NextAction != null)
        {
            NextAction();
        }
    }

    public void OnClickAutoPlayButton()
    {
        if (MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch())
        {
            return;
        }

        if (AutoPlayAction != null)
        {
            AutoPlayAction();
        }
    }

    /// <summary>
    /// 下のアンカーのY座標の設定
    /// </summary>
    /// <param name="posY"></param>
    public void SetBottomPositionY(float posY)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, posY);
    }

    public void SetUpButtons(bool isQuest)
    {
        if (m_isSetupButtons == true)
        {
            return;
        }
        m_isSetupButtons = true;

        var nextButtonModel = new ButtonModel();
        ButtonView
            .Attach<ButtonView>(NextButtonPrefabPath, m_nextButtonRoot)
            .SetModel<ButtonModel>(nextButtonModel);
        nextButtonModel.OnClicked += () =>
        {
            OnClickNextButton();
        };

        m_MemberButtonModel = new ButtonModel();
        m_MemberButton = ButtonView.Attach<PartySelectMemberButton>(MemberButtonPrefabPath, (isQuest ? m_questMemberButtonRoot : m_memberButtonRoot));
        m_MemberButton.SetModel(m_MemberButtonModel);
        m_MemberButtonModel.OnClicked += () =>
        {
            OnClickMemberSettingButton();
        };

        var autoPlayButtonModel = new ButtonModel();
        ButtonView
            .Attach<ButtonView>(AutoPlayButtonPrefabPath, m_autoPlayButtonRoot)
            .SetModel<ButtonModel>(autoPlayButtonModel);
        autoPlayButtonModel.OnClicked += () =>
        {
            OnClickAutoPlayButton();
        };

        // TODO : 演出を入れるならその場所に移す
        nextButtonModel.Appear();
        nextButtonModel.SkipAppearing();
        m_MemberButtonModel.Appear();
        m_MemberButtonModel.SkipAppearing();
        autoPlayButtonModel.Appear();
        autoPlayButtonModel.SkipAppearing();
    }
}
