using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpisodeDataListItem : ListItem<EpisodeDataContext>
{

    // 選択中のエピソードボタンの位置に合わせてクエストウィンドウの吹き出しを出すのでこっちにつけておく
    [SerializeField]
    //private Animation m_arrowAnimation;
    //private static readonly string ArrowAppearAnimationName = "episode_select_button_arrow_appear";
    //private static readonly string ArrowDisappearAnimationName = "episode_select_button_arrow_disappear";

    private EpisodeDataListItemModel m_model;

    void Awake()
    {
        AppearAnimationName = "episode_select_button_appear";
        DefaultAnimationName = "episode_select_button_loop";
    }

    void Start()
    {
        m_model = Context.model;
        SetModel(m_model);

        m_model.OnShowedArrow += () =>
        {
            //m_arrowAnimation.PlayQueued(ArrowAppearAnimationName);
        };

        m_model.OnHideArrow += () =>
        {
            //m_arrowAnimation.PlayQueued(ArrowDisappearAnimationName);
        };

        RegisterKeyEventCallback("next", () =>
        {
            m_model.ShowNext();
        });

        m_model.ViewInstantidated();
    }

    public void OnSelectEpisode()
    {
        base.Click();
    }
}
