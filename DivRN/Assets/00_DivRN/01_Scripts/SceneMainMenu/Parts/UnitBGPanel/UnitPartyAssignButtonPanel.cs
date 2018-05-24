/**
 *  @file   UnitPartyAssignButtonPanel.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/05/16
 */

using UnityEngine;
using System.Collections;
using M4u;
using System;

public class UnitPartyAssignButtonPanel : MenuPartsBase
{
    [SerializeField]
    private GameObject m_executeButtonRoot;
    [SerializeField]
    private GameObject m_returnButtonRoot;

    private static readonly string ExecuteButtonPrefabPath = "Prefab/UnitBGPanel/UnitPartyAssignExecuteButton";
    private static readonly string ReturnButtonPrefabPath = "Prefab/UnitBGPanel/UnitPanelReturnButton";

    public Action ClickExecuteButton = delegate { };
    public Action ClickReturnButton = delegate { };

    M4uProperty<bool> enableExecButton = new M4uProperty<bool>(true);
    public bool EnableExecButton { get { return enableExecButton.Value; } set { enableExecButton.Value = value; } }

    M4uProperty<bool> isViewReturnButton = new M4uProperty<bool>(true);
    public bool IsViewReturnButton { get { return isViewReturnButton.Value; } set { isViewReturnButton.Value = value; } }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        SetUpButtons();
    }

    // Use this for initialization
    void Start()
    {
        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.fitTopAndBottom(GetComponent<RectTransform>());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickExecButton()
    {
        if (ClickExecuteButton != null)
        {
            ClickExecuteButton();
        }
    }

    public void OnClickReturnButton()
    {
        if (ClickReturnButton != null)
        {
            ClickReturnButton();
        }
    }

    void SetUpButtons()
    {
        var executeButtonModel = new ButtonModel();
        var returnButtonModel = new ButtonModel();

        ButtonView
            .Attach<ButtonView>(ExecuteButtonPrefabPath, m_executeButtonRoot)
            .SetModel<ButtonModel>(executeButtonModel);

        executeButtonModel.OnClicked += () =>
        {
            OnClickExecButton();
        };

        ButtonView
            .Attach<ButtonView>(ReturnButtonPrefabPath, m_returnButtonRoot)
            .SetModel<ButtonModel>(returnButtonModel);

        returnButtonModel.OnClicked += () =>
        {
            OnClickReturnButton();
        };

        // TODO : 演出を入れるならそこに移動
        executeButtonModel.Appear();
        executeButtonModel.SkipAppearing();
        returnButtonModel.Appear();
        returnButtonModel.SkipAppearing();
    }
}
