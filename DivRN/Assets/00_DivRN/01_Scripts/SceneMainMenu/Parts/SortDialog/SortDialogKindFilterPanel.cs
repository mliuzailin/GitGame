/**
 *  @file   SortDialogKindFilterPanel.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/20
 */


using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M4u;

public class SortDialogKindFilterPanel : MenuPartsBase
{
    M4uProperty<string> titleText = new M4uProperty<string>();
    public string TitleText
    {
        get
        {
            return titleText.Value;
        }
        set
        {
            titleText.Value = value;
        }
    }

    M4uProperty<List<SortDialogSwitchListContext>> filterSwitches = new M4uProperty<List<SortDialogSwitchListContext>>(new List<SortDialogSwitchListContext>());
    public List<SortDialogSwitchListContext> FilterSwitches
    {
        get
        {
            return filterSwitches.Value;
        }
        set
        {
            filterSwitches.Value = value;
        }
    }

    M4uProperty<string> allOnButtonText = new M4uProperty<string>();
    public string AllOnButtonText
    {
        get
        {
            return allOnButtonText.Value;
        }
        set
        {
            allOnButtonText.Value = value;
        }
    }

    M4uProperty<string> allOffButtonText = new M4uProperty<string>();
    public string AllOffButtonText
    {
        get
        {
            return allOffButtonText.Value;
        }
        set
        {
            allOffButtonText.Value = value;
        }
    }

    M4uProperty<string> includeSubKindText = new M4uProperty<string>();
    public string IncludeSubKindText
    {
        get
        {
            return includeSubKindText.Value;
        }
        set
        {
            includeSubKindText.Value = value;
        }
    }

    M4uProperty<bool> isIncludeSubKind = new M4uProperty<bool>();
    public bool IsIncludeSubKind
    {
        get
        {
            return isIncludeSubKind.Value;
        }
        set
        {
            isIncludeSubKind.Value = value;
        }
    }

    SortDialog m_SortDialog;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
        m_SortDialog = GetComponentInParent<SortDialog>();

        string titleColor = GameTextUtil.GetText("title_Color");
        TitleText = string.Format(titleColor, GameTextUtil.GetSortDialogFilterText(MAINMENU_FILTER_TYPE.FILTER_KIND));
        IncludeSubKindText = GameTextUtil.GetText("filter_text43");
        SetUpFilter();
        AllOnButtonText = GameTextUtil.GetText("filter_text58");
        AllOffButtonText = GameTextUtil.GetText("filter_text57");
        IsIncludeSubKind = (m_SortDialog.m_SortData != null) ? m_SortDialog.m_SortData.m_FilterIsIncludeKindsSub : false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetUpFilter()
    {
        FilterSwitches.Clear();
        AddFilterSwitch(MasterDataDefineLabel.KindType.HUMAN);
        AddFilterSwitch(MasterDataDefineLabel.KindType.DRAGON);
        AddFilterSwitch(MasterDataDefineLabel.KindType.GOD);
        AddFilterSwitch(MasterDataDefineLabel.KindType.DEMON);
        AddFilterSwitch(MasterDataDefineLabel.KindType.CREATURE);
        AddFilterSwitch(MasterDataDefineLabel.KindType.BEAST);
        AddFilterSwitch(MasterDataDefineLabel.KindType.MACHINE);
        AddFilterSwitch(MasterDataDefineLabel.KindType.EGG);
    }

    public void AddFilterSwitch(MasterDataDefineLabel.KindType kindType)
    {
        SortDialogSwitchListContext filterSwitch = new SortDialogSwitchListContext();
        filterSwitch.KindType = kindType;
        filterSwitch.NameImage = MainMenuUtil.GetTextKindSprite(kindType, true);
        filterSwitch.IsSelect = CheckSelectFilter(kindType, m_SortDialog.m_SortData);
        filterSwitch.DidSelectItem = OnClickSwitch;
        FilterSwitches.Add(filterSwitch);
    }

    /// <summary>
    /// フィルタの選択状態をチェック
    /// </summary>
    /// <param name="kindType"></param>
    /// <returns></returns>
    bool CheckSelectFilter(MasterDataDefineLabel.KindType kindType, LocalSaveSortInfo sortInfo)
    {
        if (sortInfo == null) { return false; }
        if (sortInfo.m_FilterKinds == null || sortInfo.m_FilterKinds.Length == 0) { return false; }
        return (Array.IndexOf(sortInfo.m_FilterKinds, (int)kindType) >= 0);
    }


    public void UpdateData(LocalSaveSortInfo sortInfo)
    {
        for (int i = 0; i < FilterSwitches.Count; i++)
        {
            FilterSwitches[i].IsSelect = CheckSelectFilter(FilterSwitches[i].KindType, sortInfo);
        }

        IsIncludeSubKind = (sortInfo != null) ? sortInfo.m_FilterIsIncludeKindsSub : false;
    }

    /// <summary>
    /// 服種族を含む切り替えたとき
    /// </summary>
    public void OnClickSubKindButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        IsIncludeSubKind = !IsIncludeSubKind;
    }

    /// <summary>
    /// スイッチを切り替えたとき
    /// </summary>
    /// <param name="sw"></param>
    public void OnClickSwitch(SortDialogSwitchListContext sw)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    /// <summary>
    /// 一括ONボタンを押したとき
    /// </summary>
    public void OnClickAllOnButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (FilterSwitches != null)
        {
            FilterSwitches.ConvertAll(v => v.IsSelect = true);
        }
    }

    /// <summary>
    /// 一括OFFボタンを押したとき
    /// </summary>
    public void OnClickAllOffButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (FilterSwitches != null)
        {
            FilterSwitches.ConvertAll(v => v.IsSelect = false);
        }
    }


}
