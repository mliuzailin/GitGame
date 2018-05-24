/**
 *  @file   SortDialogFavoriteSortPanel.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/21
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M4u;

public class SortDialogFavoriteSortPanel : MenuPartsBase
{
    public Action<SortDialogFavoriteSortListContext> OnClickFavoriteSortItemButtonAction;

    M4uProperty<List<SortDialogFavoriteSortListContext>> sortCells = new M4uProperty<List<SortDialogFavoriteSortListContext>>(new List<SortDialogFavoriteSortListContext>());
    public List<SortDialogFavoriteSortListContext> SortCells
    {
        get
        {
            return sortCells.Value;
        }
        set
        {
            sortCells.Value = value;
        }
    }

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

    M4uProperty<string> usePriorityText = new M4uProperty<string>();
    /// <summary>使用中優先のテキスト</summary>
    public string UsePriorityText
    {
        get
        {
            return usePriorityText.Value;
        }
        set
        {
            usePriorityText.Value = value;
        }
    }

    M4uProperty<bool> isOnUsePriority = new M4uProperty<bool>(true);
    /// <summary>使用中優先かどうか</summary>
    public bool IsOnUsePriority
    {
        get
        {
            return isOnUsePriority.Value;
        }
        set
        {
            isOnUsePriority.Value = value;
        }
    }

    M4uProperty<string> clearButtonText = new M4uProperty<string>();
    /// <summary>クリアボタンのテキスト</summary>
    public string ClearButtonText
    {
        get
        {
            return clearButtonText.Value;
        }
        set
        {
            clearButtonText.Value = value;
        }
    }

    M4uProperty<bool> isActiveUsePriority = new M4uProperty<bool>(true);
    /// <summary>使用中優先を表示するかどうか</summary>
    public bool IsActiveUsePriority { get { return isActiveUsePriority.Value; } set { isActiveUsePriority.Value = value; } }

    SortDialog m_SortDialog;
    public int SelectIndex;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
        m_SortDialog = GetComponentInParent<SortDialog>();

        string titleColor = GameTextUtil.GetText("title_Color");
        TitleText = string.Format(titleColor, GameTextUtil.GetText("filter_title4"));
        UsePriorityText = GameTextUtil.GetText("filter_text50");
        ClearButtonText = GameTextUtil.GetText("filter_text51");
        SetUpSortList();
        UpdateData(m_SortDialog.m_SortData);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetUpSortList()
    {
        SortCells.Clear();
        for (int i = 0; i < 5; i++)
        {
            SortDialogFavoriteSortListContext favoriteSort = new SortDialogFavoriteSortListContext();
            string textkey = string.Format("filter_text{0}", i + 44);
            favoriteSort.PriorityText = GameTextUtil.GetText(textkey);
            favoriteSort.DetailFilterTextColor = ColorUtil.COLOR_YELLOW;
            favoriteSort.OnClickFilterAction = OnClickFavoriteSortItemButton;
            favoriteSort.OnClickOrderAction = OnClickFavoriteSortOrderButton;
            favoriteSort.SetUpOderText();
            SortCells.Add(favoriteSort);
        }
    }

    public void SetSortType(MAINMENU_SORT_SEQ sortType, int index)
    {
        if (SortCells.IsRange(index))
        {
            SortCells[index].SortType = sortType;
            SortCells[index].DetailFilterText = string.Format(GameTextUtil.GetText("filter_choice"), GameTextUtil.GetSortDialogSortText(SortCells[index].SortType));

            if (SortCells[index].SortType == MAINMENU_SORT_SEQ.SEQ_INIT)
            {
                SortCells[index].IsAscOrder = true;
            }
        }
    }

    public void UpdateData(LocalSaveSortInfo sortInfo)
    {
        if (sortInfo != null)
        {
            for (int i = 0; i < SortCells.Count; i++)
            {
                MAINMENU_SORT_SEQ sortType = MAINMENU_SORT_SEQ.SEQ_INIT;
                bool isAscOrder = true;
                if (sortInfo != null && sortInfo.m_FavoriteSorts != null)
                {
                    if (sortInfo.m_FavoriteSorts.Length > i)
                    {
                        sortType = (MAINMENU_SORT_SEQ)sortInfo.m_FavoriteSorts[i].m_SortType;
                        isAscOrder = sortInfo.m_FavoriteSorts[i].m_IsAscOrder;
                    }
                }

                SetSortType(sortType, i);
                SortCells[i].IsAscOrder = isAscOrder;
            }

            IsOnUsePriority = sortInfo.m_FavoriteSortIsUsePriority;
        }
    }

    /// <summary>
    /// 使用中優先を切り替えたとき
    /// </summary>
    public void OnClickUsePriorityButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        IsOnUsePriority = !IsOnUsePriority;
    }

    /// <summary>
    /// 初期状態に戻すボタンを押したとき
    /// </summary>
    public void OnClickClearButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        string detailFilterText = string.Format(GameTextUtil.GetText("filter_choice"), GameTextUtil.GetSortDialogSortText(MAINMENU_SORT_SEQ.SEQ_INIT));
        for (int i = 0; i < SortCells.Count; i++)
        {
            SortDialogFavoriteSortListContext favoriteSort = SortCells[i];
            favoriteSort.SortType = MAINMENU_SORT_SEQ.SEQ_INIT;
            favoriteSort.DetailFilterText = detailFilterText;
            favoriteSort.IsAscOrder = true;
        }

        IsOnUsePriority = false;
    }

    /// <summary>
    /// ソート項目を押したとき
    /// </summary>
    /// <param name="favoriteSort"></param>
    void OnClickFavoriteSortItemButton(SortDialogFavoriteSortListContext favoriteSort)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        SelectIndex = SortCells.IndexOf(favoriteSort);
        if (OnClickFavoriteSortItemButtonAction != null)
        {
            OnClickFavoriteSortItemButtonAction(favoriteSort);
        }
    }

    /// <summary>
    /// 昇順・降順ボタンを押したとき
    /// </summary>
    /// <param name="favoriteSort"></param>
    void OnClickFavoriteSortOrderButton(SortDialogFavoriteSortListContext favoriteSort)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

    }
}
