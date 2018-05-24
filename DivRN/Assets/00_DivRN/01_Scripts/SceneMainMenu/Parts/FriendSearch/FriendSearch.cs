/**
 *  @file   FriendSearch.cs
 *  @brief  フレンド検索
 *  @author Developer
 *  @date   2016/11/21
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using M4u;
using TMPro;

public class FriendSearch : MenuPartsBase
{
    [SerializeField]
    private GameObject m_decisionButtonRoot;
    [SerializeField]
    private GameObject m_searchButtonRoot;
    [SerializeField]
    private TMP_InputField m_inputField;

    private static readonly string DecisionButtonPrefabPath = "Prefab/FriendSearch/FriendSearchDecisionButton";
    private static readonly string SearchButtonPrefabPath = "Prefab/FriendSearch/FriendSearchButton";


    M4uProperty<string> selfIDText = new M4uProperty<string>();
    public string SelfIDText
    {
        get
        {
            return selfIDText.Value;
        }
        set
        {
            selfIDText.Value = value;
        }
    }
    public Action<string> DidSearchFriend = delegate { };

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_inputField.enabled = false;

        SetUpButtons();
    }

    // Use this for initialization
    void Start()
    {
        m_inputField.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSelectSearchButton()
    {
        DidSearchFriend(m_inputField.text);
    }

    public void resetInputField()
    {
        m_inputField.text = "";
    }



    private void SetUpButtons()
    {
        var decisionButtonModel = new ButtonModel();
        ButtonView
            .Attach<ButtonView>(DecisionButtonPrefabPath, m_decisionButtonRoot)
            .SetModel<ButtonModel>(decisionButtonModel);
        decisionButtonModel.OnClicked += () =>
        {
            OnSelectSearchButton();
        };

        var searchButtonModel = new ButtonModel();
        ButtonView
            .Attach<ButtonView>(SearchButtonPrefabPath, m_searchButtonRoot)
            .SetModel<ButtonModel>(searchButtonModel);
        searchButtonModel.OnClicked += () =>
        {
            OnSelectSearchButton();
        };


        // TODO : 演出を入れるならその場所に移動
        decisionButtonModel.Appear();
        decisionButtonModel.SkipAppearing();
        searchButtonModel.Appear();
        searchButtonModel.SkipAppearing();
    }
}