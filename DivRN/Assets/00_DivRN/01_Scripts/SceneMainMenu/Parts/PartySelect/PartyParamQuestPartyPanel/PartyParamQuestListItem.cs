/**
 *  @file   PartyParamQuestListItem.cs
 *  @brief
 *  @author Developer
 *  @date   2017/04/13
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using M4u;

public class PartyParamQuestListItem : ListItem<PartyParamListItemContext>
{
    [SerializeField]
    GameObject UnitListGroupRoot;
    [SerializeField]
    GameObject HPTextRoot;
    [SerializeField]
    GameObject ATKTextRoot;
    [SerializeField]
    GameObject m_linkButtonRoot;

    private static readonly string LInkButtonPrefabPath = "Prefab/PartySelectButtonPanel/PartySelectLinkButton";

    public PartySelectLinkButton m_LinkButton;

    private PartyParamListItemModel m_model = null;


    void Awake()
    {
        AppearAnimationName = "party_param_quest_list_item_appear";
        DefaultAnimationName = "party_param_quest_list_item_default";
        SetUpButtons();

        // アニメーションの初期設定
        UnitListGroupRoot.transform.localScale = new Vector3(0, 1, 1);
        HPTextRoot.SetActive(false);
        ATKTextRoot.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        m_model = Context.model;
        SetModel(m_model);

        RegisterKeyEventCallback("show_unit_list", () =>
        {
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
        });


        // ページ切り替え用トグルの設定
        Context.Toggle = GetComponent<Toggle>();
        ToggleGroup toggleGroup = GetComponentInParent<ToggleGroup>();
        if (toggleGroup != null)
        {
            Context.Toggle.group = toggleGroup;
        }


        // コールバック設定
        PartyParamQuestPartyPanel partyParam = GetComponentInParent<PartyParamQuestPartyPanel>();
        if (partyParam != null)
        {
            Context.Toggle.onValueChanged.AddListener(partyParam.OnChangedPartyParam);
        }
    }

    // Update is called once per frame
    void Update()
    {

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
