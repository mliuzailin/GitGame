/**
 *  @file   SortDialogRareSwitchPanel.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/18
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M4u;

public class SortDialogRareFilterPanel : MenuPartsBase
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
        TitleText = string.Format(titleColor, GameTextUtil.GetSortDialogFilterText(MAINMENU_FILTER_TYPE.FILTER_RARE));
        SetUpFilter();
        AllOnButtonText = GameTextUtil.GetText("filter_text58");
        AllOffButtonText = GameTextUtil.GetText("filter_text57");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetUpFilter()
    {
        FilterSwitches.Clear();
        for (int i = 0; i < (int)MasterDataDefineLabel.RarityType.MAX; i++)
        {
            SortDialogSwitchListContext filterSwitch = new SortDialogSwitchListContext();
            filterSwitch.RarityType = (MasterDataDefineLabel.RarityType)i;
            filterSwitch.NameText = GameTextUtil.GetText(string.Format("filter_text{0}", 21 + i));
            filterSwitch.IsSelect = CheckSelectFilter(filterSwitch.RarityType, m_SortDialog.m_SortData);
            filterSwitch.DidSelectItem = OnClickSwitch;
            FilterSwitches.Add(filterSwitch);
        }
    }

    /// <summary>
    /// フィルタの選択状態をチェック
    /// </summary>
    /// <param name="rarityType"></param>
    /// <returns></returns>
    bool CheckSelectFilter(MasterDataDefineLabel.RarityType rarityType, LocalSaveSortInfo sortInfo)
    {
        if (sortInfo == null) { return false; }
        if (sortInfo.m_FilterRares == null || sortInfo.m_FilterRares.Length == 0) { return false; }
        return (Array.IndexOf(sortInfo.m_FilterRares, (int)rarityType) >= 0);
    }

    public void UpdateData(LocalSaveSortInfo sortInfo)
    {
        for (int i = 0; i < FilterSwitches.Count; i++)
        {
            FilterSwitches[i].IsSelect = CheckSelectFilter(FilterSwitches[i].RarityType, sortInfo);
        }
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
