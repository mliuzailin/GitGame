/**
 *  @file   PartyParamUnitListItem.cs
 *  @brief
 *  @author Developer
 *  @date   2017/02/13
 */

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PartyParamUnitListItem : ListItem<PartyMemberUnitContext>
{
    const string m_LinkUnitAnimKey = "unit_link_anim_key";

    [SerializeField]
    private Animation m_statusAimation;

    [SerializeField]
    GameObject UnitIconRoot;
    [SerializeField]
    GameObject HPRoot;
    [SerializeField]
    GameObject ATKRoot;
    [SerializeField]
    GameObject SkillRoot;
    [SerializeField]
    GameObject LinkUnitIconRoot;

    private static readonly string AppearStatusAnimationName = "party_param_unit_list_item_status_appear";

    private bool m_bShowedStatus = false;
    public bool isShowedStatus { get { return m_bShowedStatus; } }

    void Awake()
    {
        AppearAnimationName = "party_param_unit_list_item_appear";
        DefaultAnimationName = "party_param_unit_list_item_loop";
        UnitIconRoot.SetActive(false);
        HPRoot.SetActive(false);
        ATKRoot.SetActive(false);
        SkillRoot.SetActive(false);
        LinkUnitIconRoot.transform.localScale = new Vector3(1, 0, 1);
    }

    void Start()
    {
        var model = Context.model;
        SetModel(model);

        model.OnShowedStatus += () =>
        {
            m_statusAimation.Stop();
            m_statusAimation.cullingType = AnimationCullingType.AlwaysAnimate;
            // 同じフレーム中にたくさんPlayQueued()を呼ぶと動かなくなるのでPlay()で代用
            m_statusAimation.Play(AppearStatusAnimationName, PlayMode.StopSameLayer);
            m_bShowedStatus = true;
        };

        RegisterKeyEventCallback("next", () => { model.ShowNext(); });
    }

    void OnDestroy()
    {
        DOTween.Kill(m_LinkUnitAnimKey + this.GetInstanceID());
    }

    public void OnClick()
    {
        if (MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch())
            return;

        base.Click();
    }

    public void OnLongPress()
    {
        if (MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch())
            return;

        base.LongPress();
    }

    /// <summary>
    /// リンクユニットの表示・非表示
    /// </summary>
    /// <param name="isShow"></param>
    public void SetUpLinkUnit(bool isShow)
    {
        DOTween.Kill(m_LinkUnitAnimKey + this.GetInstanceID());
        if (isShow)
        {
            LinkUnitIconRoot.transform.localScale = new Vector3(1, 0, 1);
            LinkUnitIconRoot.transform
                .DOScaleY(1, 0.1f)
                .SetId(m_LinkUnitAnimKey + this.GetInstanceID())
                .OnComplete(() =>
                {

                });
        }
        else
        {
            LinkUnitIconRoot.transform.localScale = new Vector3(1, 0, 1);
        }

    }
}
