/**
 *  @file   PartyParamListItem.cs
 *  @brief
 *  @author Developer
 *  @date   2017/04/13
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using M4u;

public class PartyParamListItem : ListItem<PartyParamListItemContext>
{
    [SerializeField]
    GameObject m_linkButtonRoot;

    private static readonly string LInkButtonPrefabPath = "Prefab/PartySelectButtonPanel/PartySelectLinkButton";

    public PartySelectLinkButton m_LinkButton;

    private PartyParamListItemModel m_model = null;
    private bool m_bShow = false;
    private bool m_bEvent = false;

    void Awake()
    {
        AppearAnimationName = "party_param_list_item_appear";
        DefaultAnimationName = "party_param_list_item_default";

        SetUpButtons();
    }


    // Use this for initialization
    void Start()
    {
        m_model = Context.model;
        SetModel(m_model);

        RegisterKeyEventCallback("show_unit_list", () =>
        {
            m_bEvent = true;
        });


        // ページ切り替え用トグルの設定
        Context.Toggle = GetComponent<Toggle>();
        ToggleGroup toggleGroup = GetComponentInParent<ToggleGroup>();
        if (toggleGroup != null)
        {
            Context.Toggle.group = toggleGroup;
        }


        // コールバック設定
        PartyParamPanel partyParam = GetComponentInParent<PartyParamPanel>();
        if (partyParam != null)
        {
            Context.Toggle.onValueChanged.AddListener(partyParam.OnChangedPartyParam);
        }
    }

    private void Update()
    {
        if (m_bEvent == false) return;
        if (m_bShow == true) return;
        if (m_model == null) return;

        if (m_model.isUnitAppearingBegan() == true)
        {
            m_bShow = true;
            new SerialProcess()
                .Add((System.Action next) =>
                {
                    m_model.ShowUnits(next);
                })
                .Add((System.Action next) =>
                {
                    m_model.ShowSkills(next);
                })
                .Flush();
        }
    }

    void SetUpButtons()
    {
        var linkButtonModel = new ButtonModel();
        m_LinkButton = ButtonView.Attach<PartySelectLinkButton>(LInkButtonPrefabPath, m_linkButtonRoot);
        m_LinkButton.SetModel<ButtonModel>(linkButtonModel);
        linkButtonModel.OnClicked += () =>
        {
            OnClickLinkButton();
        };
        linkButtonModel.Appear();
        linkButtonModel.SkipAppearing();
    }

    /// <summary>
    /// リンクボタンを押したとき
    /// </summary>
    public void OnClickLinkButton()
    {
        if (MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch())
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (Context.SelectLinkAction != null)
        {
            Context.SelectLinkAction();
        }
    }
}
