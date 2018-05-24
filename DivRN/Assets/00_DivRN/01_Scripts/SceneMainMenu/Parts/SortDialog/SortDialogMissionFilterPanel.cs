using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using System;

public class SortDialogMissionFilterPanel : MenuPartsBase
{
    M4uProperty<string> drawFilterTitleText = new M4uProperty<string>();
    public string DrawFilterTitleText { get { return drawFilterTitleText.Value; } set { drawFilterTitleText.Value = value; } }

    M4uProperty<string> receiveFilterTitleText = new M4uProperty<string>();
    public string ReceiveFilterTitleText { get { return receiveFilterTitleText.Value; } set { receiveFilterTitleText.Value = value; } }

    M4uProperty<string> resetButtonText = new M4uProperty<string>();
    public string ResetButtonText { get { return resetButtonText.Value; } set { resetButtonText.Value = value; } }

    M4uProperty<List<SortDialogTextButtonListContext>> drawFilterButtons = new M4uProperty<List<SortDialogTextButtonListContext>>(new List<SortDialogTextButtonListContext>());
    public List<SortDialogTextButtonListContext> DrawFilterButtons { get { return drawFilterButtons.Value; } set { drawFilterButtons.Value = value; } }

    M4uProperty<List<SortDialogTextButtonListContext>> receiveFilterButtons = new M4uProperty<List<SortDialogTextButtonListContext>>(new List<SortDialogTextButtonListContext>());
    public List<SortDialogTextButtonListContext> ReceiveFilterButtons { get { return receiveFilterButtons.Value; } set { receiveFilterButtons.Value = value; } }

    SortDialog m_SortDialog;

    float m_InitHeight;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_InitHeight = GetComponent<RectTransform>().rect.height;
    }

    // Use this for initialization
    void Start()
    {
        m_SortDialog = GetComponentInParent<SortDialog>();

        string titleColor = GameTextUtil.GetText("title_Color");
        DrawFilterTitleText = string.Format(titleColor, GameTextUtil.GetText("filter_title1"));
        ReceiveFilterTitleText = string.Format(titleColor, GameTextUtil.GetText("filter_button_allget"));
        ResetButtonText = GameTextUtil.GetText("filter_text51");

        SetUpDrawFilterList();
        SetUpReceiveFilterList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetUpDrawFilterList()
    {
        DrawFilterButtons.Clear();
        AddDrawFilterData(MasterDataDefineLabel.AchievementFilterType.ALL);
        AddDrawFilterData(MasterDataDefineLabel.AchievementFilterType.NOT_ACHIEVED);
        AddDrawFilterData(MasterDataDefineLabel.AchievementFilterType.UNACQUIRED);
    }

    void AddDrawFilterData(MasterDataDefineLabel.AchievementFilterType filterType)
    {
        SortDialogTextButtonListContext filterButton = new SortDialogTextButtonListContext();
        filterButton.AchievementFilterType = filterType;
        filterButton.OffNameText = GameTextUtil.GetMissonDrawFilterText(filterType);
        filterButton.OnNameText = string.Format(GameTextUtil.GetText("filter_choice"), GameTextUtil.GetMissonDrawFilterText(filterType));
        filterButton.OffTextColor = ColorUtil.COLOR_WHITE;
        filterButton.OnTextColor = ColorUtil.COLOR_YELLOW;
        filterButton.DidSelectItem = OnClickDrawFilterButton;
        DrawFilterButtons.Add(filterButton);
    }

    public void UpdateDrawFilter(MasterDataDefineLabel.AchievementFilterType filterType)
    {
        if (DrawFilterButtons != null)
        {
            for (int i = 0; i < DrawFilterButtons.Count; i++)
            {
                DrawFilterButtons[i].IsSelect = (filterType == DrawFilterButtons[i].AchievementFilterType);
            }
        }
    }

    void SetUpReceiveFilterList()
    {
        ReceiveFilterButtons.Clear();
        //AddReceiveFilterData(MasterDataDefineLabel.AchievementReceiveType.NONE);
        AddReceiveFilterData(MasterDataDefineLabel.AchievementReceiveType.UNIT);
        AddReceiveFilterData(MasterDataDefineLabel.AchievementReceiveType.COIN);
        AddReceiveFilterData(MasterDataDefineLabel.AchievementReceiveType.CHIP);
        AddReceiveFilterData(MasterDataDefineLabel.AchievementReceiveType.OTHER);
    }

    void AddReceiveFilterData(MasterDataDefineLabel.AchievementReceiveType filterType)
    {
        SortDialogTextButtonListContext filterButton = new SortDialogTextButtonListContext();
        filterButton.AchievementReceiveType = filterType;
        filterButton.OffNameText = GameTextUtil.GetMissonReceiveFilterText(filterType);
        filterButton.OnNameText = string.Format(GameTextUtil.GetText("filter_choice"), GameTextUtil.GetMissonReceiveFilterText(filterType));
        filterButton.OffTextColor = ColorUtil.COLOR_WHITE;
        filterButton.OnTextColor = ColorUtil.COLOR_YELLOW;
        filterButton.DidSelectItem = OnClickReceiveFilterButton;
        ReceiveFilterButtons.Add(filterButton);
    }

    public void UpdateReceiveFilter(MasterDataDefineLabel.AchievementReceiveType filterType)
    {
        if (ReceiveFilterButtons != null)
        {
            for (int i = 0; i < ReceiveFilterButtons.Count; i++)
            {
                ReceiveFilterButtons[i].IsSelect = (filterType == ReceiveFilterButtons[i].AchievementReceiveType);
            }
        }
    }

    public void OnClickResetButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (m_SortDialog.m_MissionFilterData == null)
        {
            m_SortDialog.m_MissionFilterData = new SortDialog.MissionFilterInfo();
        }
        m_SortDialog.m_MissionFilterData.InitParam();

        UpdateDrawFilter(m_SortDialog.m_MissionFilterData.m_filter_type);
        UpdateReceiveFilter(m_SortDialog.m_MissionFilterData.m_receive_type);

    }

    void OnClickDrawFilterButton(SortDialogTextButtonListContext filterButton)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (m_SortDialog != null && m_SortDialog.m_MissionFilterData != null)
        {
            m_SortDialog.m_MissionFilterData.m_filter_type = filterButton.AchievementFilterType;
            UpdateDrawFilter(filterButton.AchievementFilterType);
        }
    }

    void OnClickReceiveFilterButton(SortDialogTextButtonListContext filterButton)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (m_SortDialog != null && m_SortDialog.m_MissionFilterData != null)
        {
            m_SortDialog.m_MissionFilterData.m_receive_type = filterButton.AchievementReceiveType;
            UpdateReceiveFilter(filterButton.AchievementReceiveType);
        }
    }

    void OnChangedFilterButtons()
    {
        if (m_SortDialog == null)
        {
            m_SortDialog = GetComponentInParent<SortDialog>();
        }
        if (m_SortDialog != null && m_SortDialog.m_MissionFilterData != null)
        {
            UpdateDrawFilter(m_SortDialog.m_MissionFilterData.m_filter_type);
            UpdateReceiveFilter(m_SortDialog.m_MissionFilterData.m_receive_type);
        }
    }
}
