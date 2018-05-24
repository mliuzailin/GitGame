/// <summary>
/// プレゼントアイコン
/// </summary>

using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class LoginBonusRecordListItem : ListItem<LoginBonusRecordListContext>
{
    static readonly string m_countFadeKey = "count_fade";

    [SerializeField]
    GameObject m_effectRoot;
    [SerializeField]
    float m_DurationSeconds = 1.0f;
    [SerializeField]
    Ease m_EaseType = Ease.InCubic;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnDestroy()
    {
        DOTween.Kill(m_countFadeKey);
    }


    public void OnClick()
    {
        if (Context.DidSelectItem != null)
        {
            Context.DidSelectItem(Context);
        }
    }

    public void SetUpCheckAnim(Action callback)
    {
        PlayAnimation("login_bonus_record_item_check", () =>
        {
            PlayAnimation("login_bonus_record_item_default", () =>
            {
                if (Context.IsToday == true)
                {
                    SetLoopCountFade();
                }
                if (callback != null)
                {
                    callback();
                }
            });
        });
    }

    void SetLoopCountFade()
    {
        DOTween.Kill(m_countFadeKey);

        Context.CountColor = Color.white;
        DOTween.ToAlpha(() => Context.CountColor, (x) => Context.CountColor = x, 0.0f, m_DurationSeconds).SetEase(m_EaseType).SetLoops(-1, LoopType.Yoyo).SetId(m_countFadeKey);
    }
}
