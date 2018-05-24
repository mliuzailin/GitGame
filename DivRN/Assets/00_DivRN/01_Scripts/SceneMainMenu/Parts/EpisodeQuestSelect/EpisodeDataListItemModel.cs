using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EpisodeDataListItemModel : ListItemModel
{
    new public event EventHandler OnUpdated;
    public event EventHandler OnShowedNext;
    public event EventHandler OnShowedArrow;
    public event EventHandler OnHideArrow;

    // MainMenuQuestSelectでインスタンス化終了タイミングを取れない造りなのでイベント発行
    // TODO : MainMenuSeq系を整理したら消す
    public event EventHandler OnViewInstantidated;
    public void ViewInstantidated() { if (OnViewInstantidated != null) OnViewInstantidated(); }

    protected bool m_isSelected = false;


    public EpisodeDataListItemModel(uint index) : base(index)
    {
        m_index = index;

        base.OnUpdated += () =>
        {
            if (OnUpdated != null)
                OnUpdated();
        };
    }

    // 次のボタンを順番に表示する
    public void ShowNext()
    {
        if (OnShowedNext != null)
            OnShowedNext();
    }

    // 選択中を示す矢印の表示アニメーションを再生する
    public void ShowArrow()
    {
        if (OnShowedArrow != null)
            OnShowedArrow();
    }

    // 選択中を示す矢印をけす
    public void HideArrow()
    {
        if (OnHideArrow != null)
            OnHideArrow();
    }


    public bool isSelected
    {
        get { return m_isSelected; }
        set
        {
            m_isSelected = value;

            if (OnUpdated != null)
                OnUpdated();
        }
    }
}
