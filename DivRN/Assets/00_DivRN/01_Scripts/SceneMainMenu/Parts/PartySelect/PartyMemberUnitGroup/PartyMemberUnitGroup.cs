/**
 *  @file   PartyMemberUnitGroup.cs
 *  @brief
 *  @author Developer
 *  @date   2017/02/14
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class PartyMemberUnitGroup : MenuPartsBase
{
    [SerializeField]
    GameObject m_linkButtonRoot;

    private static readonly string LInkButtonPrefabPath = "Prefab/PartySelectButtonPanel/PartySelectLinkButton";

    public PartySelectLinkButton m_LinkButton;

    /// <summary>パーティ情報</summary>
    public CharaParty PartyInfo = new CharaParty();

    M4uProperty<List<PartyMemberUnitContext>> units = new M4uProperty<List<PartyMemberUnitContext>>(new List<PartyMemberUnitContext>());
    /// <summary>ユニットリスト</summary>
    public List<PartyMemberUnitContext> Units
    {
        get
        {
            return units.Value;
        }
        set
        {
            units.Value = value;
        }
    }

    M4uProperty<List<GameObject>> unitList = new M4uProperty<List<GameObject>>(new List<GameObject>());
    /// <summary>ユニットアイコンオブジェクト</summary>
    public List<GameObject> UnitList
    {
        get
        {
            return unitList.Value;
        }
        set
        {
            unitList.Value = value;
        }
    }

    public Action OnClickReleaseAction = delegate { };
    public Action OnClickDetailAction = delegate { };
    public Action OnClickLinkAction = delegate { };

    CanvasGroup m_baseGroup = null;
    CanvasGroup m_myGroup = null;

    void Awake()
    {
        MainMenuParam.m_PartySelectIsShowLinkUnit = false;
        GetComponent<M4uContextRoot>().Context = this;
        SetUpButtons();
    }

    // Use this for initialization
    void Start()
    {
        m_baseGroup = gameObject.GetComponentInParent<CanvasGroup>();
        m_myGroup = gameObject.AddComponent<CanvasGroup>();
    }
    // Update is called once per frame
    void Update()
    {
        // 元CanvasのFadeに追従する
        if (m_baseGroup != null && m_myGroup != null)
        {
            m_myGroup.alpha = m_baseGroup.alpha;
        }
    }
    /// <summary>
    /// ユニットの選択状態を変更
    /// </summary>
    /// <param name="unit"></param>
    public void ChangeUnitParam(PartyMemberUnitContext unit)
    {
        for (int i = 0; i < Units.Count; ++i)
        {
            if (unit != null)
            {
                Units[i].IsSelect = (ReferenceEquals(Units[i], unit));
            }
            else
            {
                Units[i].IsSelect = false;
            }
        }
        // Canvas移動はこのタイミング
        if (SceneObjReferMainMenu.Instance.m_MainMenuRoot)
        {
            var newParent = SceneObjReferMainMenu.Instance.m_MainMenuRoot.transform.Find("UIMainMenu/TopAnchor/MainMenuHeader(Clone)/PartyMemberUnitGroupRoot");
            if (newParent == null)
            {
                newParent = GameObject.Find("PartyMemberUnitGroupRoot").transform;
            }
            if (newParent != null)
            {
                transform.SetParent(newParent);
            }
        }
    }

    void OnChangedUnitList()
    {
        // 表示順を逆にする
        for (int i = 0; i < UnitList.Count; ++i)
        {
            UnitList[i].transform.SetAsFirstSibling();
        }
    }

    public void OnClickReleaseButton()
    {
        if (OnClickReleaseAction != null)
        {
            OnClickReleaseAction();
        }
    }

    public void OnClickDetailButton()
    {
        if (OnClickDetailAction != null)
        {
            OnClickDetailAction();
        }
    }

    void SetUpButtons()
    {
        var linkButtonModel = new ButtonModel();
        m_LinkButton = ButtonView.Attach<PartySelectLinkButton>(LInkButtonPrefabPath, m_linkButtonRoot);
        m_LinkButton.SetModel<ButtonModel>(linkButtonModel);
        linkButtonModel.OnClicked += () =>
        {
            OnClickLinkButton();
        };
        linkButtonModel.Appear();
        linkButtonModel.SkipAppearing();
        m_LinkButton.SetSelected(MainMenuParam.m_PartySelectIsShowLinkUnit);
    }

    /// <summary>
    /// リンクボタンを押したとき
    /// </summary>
    public void OnClickLinkButton()
    {
        if (MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch())
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        SetUpLinkUnit();

        if (OnClickLinkAction != null)
        {
            OnClickLinkAction();
        }
    }

    public void SetUpLinkUnit()
    {
        MainMenuParam.m_PartySelectIsShowLinkUnit = !MainMenuParam.m_PartySelectIsShowLinkUnit;
        for (int i = 0; i < UnitList.Count; ++i)
        {
            var unitItem = UnitList[i].GetComponent<PartyMemberUnitListItem>();
            if (unitItem != null)
            {
                unitItem.SetUpLinkUnit(MainMenuParam.m_PartySelectIsShowLinkUnit);
            }
        }
        m_LinkButton.SetSelected(MainMenuParam.m_PartySelectIsShowLinkUnit);
    }

    public void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
        if (enable == true)
        {
            MainMenuParam.m_PartySelectIsShowLinkUnit = true;
            SetUpLinkUnit();
        }
    }
}
