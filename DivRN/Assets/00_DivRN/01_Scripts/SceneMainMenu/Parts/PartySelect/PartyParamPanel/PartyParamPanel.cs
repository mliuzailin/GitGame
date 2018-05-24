/**
 *  @file   PartyParamPanel.cs
 *  @brief
 *  @author Developer
 *  @date   2017/02/13
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using M4u;
using AsPerSpec;

public class PartyParamPanel : MenuPartsBase
{
    [SerializeField]
    private GameObject m_linkButtonRoot;

    public Action<int> OnChangedPartyParamAction = delegate { };

    M4uProperty<List<PartyParamListItemContext>> partyParams = new M4uProperty<List<PartyParamListItemContext>>(new List<PartyParamListItemContext>());
    public List<PartyParamListItemContext> PartyParams
    {
        get
        {
            return partyParams.Value;
        }
        set
        {
            partyParams.Value = value;
        }
    }

    M4uProperty<bool> isActivePrevButton = new M4uProperty<bool>();
    bool IsActivePrevButton
    {
        get
        {
            return isActivePrevButton.Value;
        }
        set
        {
            isActivePrevButton.Value = value;
        }
    }

    M4uProperty<bool> isActiveNextButton = new M4uProperty<bool>();
    bool IsActiveNextButton
    {
        get
        {
            return isActiveNextButton.Value;
        }
        set
        {
            isActiveNextButton.Value = value;
        }
    }

    M4uProperty<bool> isScrollMoving = new M4uProperty<bool>();
    public bool IsScrollMoving { get { return isScrollMoving.Value; } set { isScrollMoving.Value = value; } }

    List<GameObject> partyParamList = new List<GameObject>();
    public List<GameObject> PartyParamList { get { return partyParamList; } set { partyParamList = value; } }


    public CarouselRotator m_CarouselRotator = null;
    public CarouselToggler m_CarouselToggler = null;

    public int m_CurrentIndex = 0;
    bool m_IsChangeSnap = false;
    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_CarouselRotator = GetComponentInChildren<CarouselRotator>();
        m_CarouselToggler = GetComponentInChildren<CarouselToggler>();
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        if (IsScrollMoving != m_CarouselToggler.moving)
        {
            IsScrollMoving = m_CarouselToggler.moving;
        }
    }

    /// <summary>
    /// 任意のインデクスにリストを切り替える
    /// </summary>
    /// <param name="index"></param>
    public void ChangeParam(int index)
    {
        ScrollRect scrollRect = m_CarouselToggler.gameObject.GetComponent<ScrollRect>();
        RectTransform content;
        content = scrollRect.content;

        if (index >= 0 && index < PartyParamList.Count)
        {
            Toggle toggle = PartyParamList[index].GetComponentInChildren<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = true;
                m_CarouselToggler.CenterOnToggled();
            }
        }
    }

    /// <summary>
    /// 矢印ボタンの表示・非表示状態を変更する
    /// </summary>
    void ChangeArrowButtonActive()
    {
        int index = -1;
        for (int i = 0; i < PartyParamList.Count; i++)
        {
            Toggle toggle = PartyParamList[i].GetComponentInChildren<Toggle>();
            if (toggle == null) { continue; }

            if (toggle.isOn == true)
            {
                index = i;
                break;
            }
        }

        if (PartyParamList.Count <= 1)
        {
            // リストが1個以下のとき
            IsActiveNextButton = false;
            IsActivePrevButton = false;
        }
        else if (index == 0)
        {
            // リストが先頭の場合
            IsActiveNextButton = true;
            IsActivePrevButton = false;
        }
        else if (index == PartyParamList.Count - 1)
        {
            // リストが最後尾の場合
            IsActiveNextButton = false;
            IsActivePrevButton = true;
        }
        else
        {
            // その他の場合
            IsActiveNextButton = true;
            IsActivePrevButton = true;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public void SetUpLinkUnit()
    {
        SetUpLinkUnit(!MainMenuParam.m_PartySelectIsShowLinkUnit);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="isShow"></param>
    public void SetUpLinkUnit(bool isShow)
    {
        MainMenuParam.m_PartySelectIsShowLinkUnit = isShow;

        for (int party_count = 0; party_count < PartyParams.Count; ++party_count)
        {
            PartyParamListItem panelImtem = PartyParamList[party_count].GetComponent<PartyParamListItem>();
            if (panelImtem == null)
            {
                continue;
            }

            panelImtem.m_LinkButton.SetSelected(MainMenuParam.m_PartySelectIsShowLinkUnit);

            List<GameObject> unitListObjList = PartyParams[party_count].UnitList;
            for (int unit_count = 0; unit_count < unitListObjList.Count; ++unit_count)
            {
                PartyParamUnitListItem unitItem = unitListObjList[unit_count].GetComponent<PartyParamUnitListItem>();
                if (unitItem == null)
                {
                    continue;
                }
                unitItem.SetUpLinkUnit(MainMenuParam.m_PartySelectIsShowLinkUnit);
            }
        }
    }

    void OnChangedPartyParamList()
    {
        if (PartyParamList.Count > 0)
        {
            //ScrollContentが機能した後に実行する
            StartCoroutine(DelayScrollContent(() =>
            {
                // リストの表示位置設定
                ChangeParam(m_CurrentIndex);
            }));
        }
        else
        {
        }
    }

    /// <summary>
    /// ページが切り替わったときに呼ばれる
    /// </summary>
    /// <param name="isOn"></param>
    public void OnChangedPartyParam(bool isOn)
    {
        if (isOn == true)
        {
            var oldIndex = m_CurrentIndex;

            // ページコントロールの選択位置を変える
            for (int i = 0; i < PartyParams.Count; ++i)
            {
                if (PartyParams[i].Toggle == null)
                {
                    continue;
                }
                if (PartyParams[i].Toggle.isOn == true)
                {
                    if (i != m_CurrentIndex)
                    {
                        m_CurrentIndex = i;
                        if (OnChangedPartyParamAction != null)
                        {
                            OnChangedPartyParamAction(i);
                        }
                    }
                    break;
                }
            }

            ChangeArrowButtonActive();

            if (oldIndex != m_CurrentIndex && m_IsChangeSnap == false)
            {
                SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            }
            m_IsChangeSnap = false;
        }
    }

    /// <summary>
    /// ScrollRect.contentが機能した後に実行する
    /// </summary>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayScrollContent(Action action)
    {
        ScrollRect scrollRect = m_CarouselToggler.GetComponentInChildren<ScrollRect>();
        while (scrollRect.content.rect.width == 0 || PartyParams.Any(q => q.Toggle == null))
        {
            yield return null;
        }

        action();
    }

    public void OnClickPrevButton()
    {
        //SoundUtil.PlaySE(SEID.SE_MENU_OK);
        if (m_CarouselRotator != null && m_CarouselToggler.moving == false)
        {
            m_CarouselRotator.Step(false);
        }
    }

    public void OnClickNextButton()
    {
        //SoundUtil.PlaySE(SEID.SE_MENU_OK);
        if (m_CarouselRotator != null && m_CarouselToggler.moving == false)
        {
            m_CarouselRotator.Step(true);
        }
    }

    public void OnChangedSnapCarousel(bool isChange)
    {
        if (isChange == true)
        {
            m_IsChangeSnap = true;
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
        }
    }
}
