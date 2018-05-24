/**
 *  @file   PartySelectGroup.cs
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

public class PartySelectGroup : MenuPartsBase
{

    [SerializeField]
    private RectTransform m_line;

    private static readonly string AppearAnimation = "party_select_group_appear";
    private static readonly string DefaultAnimation = "party_select_group_loop";

    M4uProperty<bool> isLowerScreen = new M4uProperty<bool>(false);
    /// <summary>低解像度かどうか</summary>
    public bool IsLowerScreen
    {
        get
        {
            return isLowerScreen.Value;
        }
        set
        {
            isLowerScreen.Value = value;
        }
    }

    M4uProperty<List<PartySelectGroupListContext>> partyGroups = new M4uProperty<List<PartySelectGroupListContext>>(new List<PartySelectGroupListContext>());
    /// <summary>パーティリスト</summary>
    List<PartySelectGroupListContext> PartyGroups
    {
        get
        {
            return partyGroups.Value;
        }
        set
        {
            partyGroups.Value = value;
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


    M4uProperty<float> backGoundImageHeight = new M4uProperty<float>(100);
    public float BackGoundImageHeight
    {
        get
        {
            return backGoundImageHeight.Value;
        }
        set
        {
            backGoundImageHeight.Value = value;
        }
    }

    M4uProperty<bool> isScrollMoving = new M4uProperty<bool>();
    public bool IsScrollMoving { get { return isScrollMoving.Value; } set { isScrollMoving.Value = value; } }

    List<GameObject> partyGroupList = new List<GameObject>();
    public List<GameObject> PartyGroupList { get { return partyGroupList; } set { partyGroupList = value; } }

    public CarouselRotator m_CarouselRotator = null;
    public CarouselToggler m_CarouselToggler = null;

    public uint PartyCount { get; private set; }

	private PartySelectGroupUnitListItem m_currentSelectGroup = null;

	void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_CarouselRotator = GetComponentInChildren<CarouselRotator>();
        m_CarouselToggler = GetComponentInChildren<CarouselToggler>();
        PartyCount = 0;
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

    public void Show()
    {
        PlayAnimation(AppearAnimation, () =>
        {
            PlayAnimation(DefaultAnimation);
        });
    }

    public void Hide()
    {
        m_line.gameObject.SetActive(false);
        m_line.transform.localScale =
            m_line.localScale = new Vector3(1, 0, 1);
    }

    public void AddData(PartySelectGroupUnitContext party)
    {
        if (PartyGroups.Count == 0)
        {
            PartySelectGroupListContext addGroup = new PartySelectGroupListContext();
            addGroup.Parties.Add(party);
            PartyGroups.Add(addGroup);
        }
        else
        {
            PartySelectGroupListContext group = PartyGroups[PartyGroups.Count - 1];
            if (group.Parties.Count < 5)
            {
                group.Parties.Add(party);
            }
            else
            {
                PartySelectGroupListContext addGroup = new PartySelectGroupListContext();
                addGroup = new PartySelectGroupListContext();
                addGroup.Parties.Add(party);
                PartyGroups.Add(addGroup);
            }
        }
        ++PartyCount;
    }

    public void ClearPartyGroups()
    {
        PartyGroups.Clear();
        PartyCount = 0;
    }

    /// <summary>
    /// 指定したインデックスのパーティ情報を取得する
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public PartySelectGroupUnitContext GetParty(int index)
    {
        for (int i = 0; i < PartyGroups.Count; ++i)
        {
            for (int j = 0; j < PartyGroups[i].Parties.Count; ++j)
            {
                PartySelectGroupUnitContext partyUnit = PartyGroups[i].Parties[j];
                if (partyUnit.Index == index)
                {
                    return partyUnit;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 矢印ボタンの表示・非表示状態を変更する
    /// </summary>
    void ChangeArrowButtonActive()
    {
        if (!IsLowerScreen)
        {
            int index = -1;
            for (int i = 0; i < PartyGroupList.Count; i++)
            {
                Toggle toggle = PartyGroupList[i].GetComponentInChildren<Toggle>();
                if (toggle == null) { continue; }

                if (toggle.isOn == true)
                {
                    index = i;
                    break;
                }
            }

            if (PartyGroupList.Count <= 1)
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
            else if (index == PartyGroupList.Count - 1)
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
        else
        {
            IsActiveNextButton = false;
            IsActivePrevButton = false;
        }
    }

    /// <summary>
    /// パーティの選択状態を変更する
    /// </summary>
    /// <param name="party"></param>
    public void ChangePartyItemSelect(int index, bool isToggleChange = false)
    {
        for (int i = 0; i < PartyGroups.Count; ++i)
        {
            for (int j = 0; j < PartyGroups[i].Parties.Count; ++j)
            {
                PartySelectGroupUnitContext partyUnit = PartyGroups[i].Parties[j];
                if (partyUnit.Index == index)
                {
                    if (isToggleChange)
                    {
                        Toggle toggle = PartyGroupList[i].GetComponentInChildren<Toggle>();
                        if (toggle != null)
                        {
                            toggle.isOn = true;
                        }
                    }
                    partyUnit.IsSelect = true;
                }
                else
                {
                    partyUnit.IsSelect = false;
                }
            }
        }

        m_CarouselToggler.CenterOnToggled();
    }

    /// <summary>
    /// ページが切り替わったときに呼ばれる
    /// </summary>
    /// <param name="isOn"></param>
    public void OnChangedPartGroup(bool isOn)
    {
        if (isOn == true)
        {
            ChangeArrowButtonActive();
        }
    }

    void OnChangedPartyGroupList()
    {
        if (PartyGroupList.Count > 0)
        {
            //ScrollContentが機能した後に実行する
            StartCoroutine(DelayScrollContent(() =>
            {
                // リストの表示位置設定
                for (int i = 0; i < PartyGroups.Count; ++i)
                {
                    for (int j = 0; j < PartyGroups[i].Parties.Count; ++j)
                    {
                        PartySelectGroupUnitContext partyUnit = PartyGroups[i].Parties[j];
                        if (partyUnit.IsSelect)
                        {
                            Toggle toggle = PartyGroupList[i].GetComponentInChildren<Toggle>();
                            if (toggle != null)
                            {
                                toggle.isOn = true;
                                m_CarouselToggler.CenterOnToggled();
                            }

                            return;
                        }
                    }
                }
            }));
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

        while (scrollRect.content.rect.width == 0 || PartyGroups.Any(q => q.Toggle == null))
        {
            yield return null;
        }

        action();
    }

    public void OnClickPrevButton()
    {
        if (m_CarouselRotator != null && m_CarouselToggler.moving == false)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            m_CarouselRotator.Step(false);
        }
    }

    public void OnClickNextButton()
    {
        if (m_CarouselRotator != null && m_CarouselToggler.moving == false)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            m_CarouselRotator.Step(true);
        }
    }

    public void OnChangedSnapCarousel(bool isChange)
    {
        if (isChange == true)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
        }
    }

	public bool isSelectGroupIconSowed()
	{
		if (m_currentSelectGroup == null)
		{
			for (int i = 0; i < PartyGroups.Count; ++i)
			{
				for (int j = 0; j < PartyGroups[i].Parties.Count; ++j)
				{
					PartySelectGroupUnitListItem partyUnit = PartyGroups[i].PartyList[j].GetComponent<PartySelectGroupUnitListItem>();
					if (partyUnit.Context.IsSelect)
					{
						m_currentSelectGroup = partyUnit;
						break;
					}
				}
			}
		}
		return m_currentSelectGroup.isIconShowFinish;
	}

	public void firstShowArrow()
	{
		m_currentSelectGroup.iconShowArrow();
	}
}
