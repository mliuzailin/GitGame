using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDataListItem : ListItem<AreaDataContext>
{
    [SerializeField]
    private GameObject m_effectRoot;
    [SerializeField]
    private GameObject m_effect2Root;
    [SerializeField]
    private GameObject m_clearedBg;
    [SerializeField]
    private GameObject m_activeBg;

    [SerializeField]
    private Animation m_textAnimation;
    private static readonly string TextAppearAnimationName = "area_list_icon_title_appear";
    private readonly string AreaSelectBufAnimationName = "mainmenu_area_select_buf_loop";
    private readonly string AreaSelectBaseAnimationName = "mainmenu_area_select_loop";

    private AreaSelectListItemModel m_model = null;

    void Start()
    {
        m_model = Context.model;

        SetModel(m_model);

        m_model.OnUpdated += () =>
        {
            UpdateViews();
        };
        m_model.OnBeganShowingTitle += () => { ShowTitle(); };

        RegisterKeyEventCallback("next", () =>
        {
            m_model.ShowNext();
        });

        SetUpAnimations();
        UpdateViews();


        // MainMenuQuestStoryでインスタンス化終了タイミングを取れない造りなのでここで最初のアイコンアニメーション再生
        // TODO : 個別のクラスにほかのクラスインスタンスがある前提の処理を書くのはよくないのでそうならないように造りを変える
        if (m_model.index == 0)
            m_model.Appear();
    }

    public void ShowTitle()
    {
        m_textAnimation.PlayQueued(TextAppearAnimationName);
    }

    public void OnSelectArea()
    {
        base.Click();
    }


    private void SetUpAnimations()
    {
        string appearName = "area_list_icon_appear";
        string defaultName = "area_list_icon_loop";

        if (m_model.isActive)
        {
            appearName = "area_list_active_icon_appear";
            defaultName = "area_list_active_icon_loop";
        }

        if (m_model.isChallenge)
        {
            appearName = "area_list_challenge_icon_appear";
            defaultName = "area_list_challenge_icon_loop";

        }

        AppearAnimationName = appearName;
        DefaultAnimationName = defaultName;
    }

    private void UpdateViews()
    {
        m_effectRoot.SetActive(m_model.isActive);

        m_clearedBg.SetActive(!m_model.isActive);
        m_activeBg.SetActive(m_model.isActive);

        m_effect2Root.SetActive(m_model.isChallenge);
    }

    public void setBufEvent(bool sw, float startTime = 0)
    {
        if (Context.m_bufEvent == true)
        {
            if (sw == true)
            {
                m_textAnimation[AreaSelectBufAnimationName].time = startTime;
                m_textAnimation.Play(AreaSelectBufAnimationName);
            }
            else
            {
                m_textAnimation.Play(AreaSelectBaseAnimationName);
            }
        }
    }
}
