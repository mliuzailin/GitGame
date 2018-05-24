/**
 *  @file   SceneHeroSelectTest.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/20
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class SceneHeroSelectTest : SceneTest<SceneHeroSelectTest>
{
    public enum SHOW_OBJECT_TYPE
    {
        FORM,
        DETAIL,
        PREVIEW,
    }

    [SerializeField]
    GameObject m_Prefab;
    [SerializeField]
    HeroForm m_HeroForm;
    [SerializeField]
    HeroDetail m_HeroDetail;
    [SerializeField]
    HeroPreview m_HeroPreview;
    [SerializeField]
    HeroSelectButtonPanel m_ButtonPanel;

    public SHOW_OBJECT_TYPE m_ShowObjectType = SHOW_OBJECT_TYPE.FORM;
    M4uProperty<SHOW_OBJECT_TYPE> showObjectType = new M4uProperty<SHOW_OBJECT_TYPE>();
    public SHOW_OBJECT_TYPE ShowObjectType
    {
        get
        {
            return showObjectType.Value;
        }
        set
        {
            showObjectType.Value = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        GetComponent<M4uContextRoot>().Context = this;

    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        m_Prefab.SetActive(false);
        ShowObjectType = m_ShowObjectType;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnInitialized()
    {
        base.OnInitialized();

        switch (m_ShowObjectType)
        {
            case SHOW_OBJECT_TYPE.FORM:
                // データの設定
                int index = Array.FindIndex(UserDataAdmin.Instance.m_StructHeroList, v => v.unique_id == UserDataAdmin.Instance.m_StructPlayer.current_hero_id);

                m_HeroForm.SetFormDatas(HeroForm.CreateFormDatas(null, null, null), index);
                m_HeroForm.OnClickNextButtonAction = OnClickFormPreviousButton;
                m_HeroForm.OnClickPreviousButtonAction = OnClickFormNextButton;
                break;
            case SHOW_OBJECT_TYPE.DETAIL:
                m_HeroDetail.SetDetail(null, OnClickStoryItem);
                break;
            case SHOW_OBJECT_TYPE.PREVIEW:
                break;
            default:
                break;
        }

        m_ButtonPanel.SetBottomPositionY(82);
    }


    #region FORM
    void OnClickFormPreviousButton()
    {
        m_HeroForm.Step(false);
    }

    void OnClickFormNextButton()
    {
        m_HeroForm.Step(true);
    }

    #endregion

    void OnClickStoryItem(HeroStoryListItemContext story)
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
        newDialog.SetDialogText(DialogTextType.Title, story.StoryTitle);
        newDialog.SetDialogText(DialogTextType.MainText, story.ContentText);
        newDialog.Show();
    }
}
