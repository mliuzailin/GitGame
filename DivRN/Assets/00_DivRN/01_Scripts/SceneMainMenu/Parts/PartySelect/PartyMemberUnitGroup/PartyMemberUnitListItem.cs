/**
 *  @file   PartyMemberUnitListItem.cs
 *  @brief
 *  @author Developer
 *  @date   2017/02/14
 */

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PartyMemberUnitListItem : ListItem<PartyMemberUnitContext>
{
    const string m_LinkUnitAnimKey = "unit_link_anim_key";

    [SerializeField]
    GameObject LinkUnitIconRoot;

    void Awake()
    {
        AppearAnimationName = "party_member_unit_list_item_appear";
        DefaultAnimationName = "party_member_unit_list_item_loop";
        LinkUnitIconRoot.transform.localScale = new Vector3(1, 0, 1);
    }

    void Start()
    {
        SetUpLinkUnit(MainMenuParam.m_PartySelectIsShowLinkUnit);
    }

    public void OnClick()
    {
        Context.DidSelectItem(Context);
    }

    public void OnLongPress()
    {
        Context.DidLongPressItem(Context);
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
