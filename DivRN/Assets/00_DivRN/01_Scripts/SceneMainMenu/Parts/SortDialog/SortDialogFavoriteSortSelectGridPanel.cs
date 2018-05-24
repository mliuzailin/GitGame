/**
 *  @file   SortDialogFavoriteSortSelectGridPanel.cs
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


public class SortDialogFavoriteSortSelectGridPanel : MenuPartsBase
{
    public Action<SortDialogTextButtonListContext> OnClickSortButtonAction;

    M4uProperty<List<SortDialogTextButtonListContext>> sortButtons = new M4uProperty<List<SortDialogTextButtonListContext>>(new List<SortDialogTextButtonListContext>());
    /// <summary>ソートボタン</summary>
    public List<SortDialogTextButtonListContext> SortButtons
    {
        get
        {
            return sortButtons.Value;
        }
        set
        {
            sortButtons.Value = value;
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
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// お好みソート選択項目の作成
    /// </summary>
    void SetUpSortList()
    {
        if (m_SortDialog == null)
        {
            m_SortDialog = GetComponentInParent<SortDialog>();
        }

        SortButtons.Clear();
        if (m_SortDialog.m_FavoriteSortList != null)
        {
            for (int i = 0; i < m_SortDialog.m_FavoriteSortList.Length; i++)
            {
                AddSortData(m_SortDialog.m_FavoriteSortList[i]);
            }
        }

        //----------------------------------------
        // 未指定項目の作成
        //----------------------------------------
        int num = SortButtons.Count % 3;
        if (num == 0)
        {
            AddSortData(MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MAX);
            AddSortData(MAINMENU_SORT_SEQ.SEQ_INIT);
        }
        else if (num == 1)
        {
            AddSortData(MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MAX);
            AddSortData(MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MAX);
            AddSortData(MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MAX);
            AddSortData(MAINMENU_SORT_SEQ.SEQ_INIT);
        }
        else if (num == 2)
        {
            AddSortData(MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MAX);
            AddSortData(MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MAX);
            AddSortData(MAINMENU_SORT_SEQ.SEQ_INIT);
        }
    }

    /// <summary>
    /// 指定したソートタイプを選択状態にする
    /// </summary>
    /// <param name="sortType"></param>
    public void SetUpSelected(MAINMENU_SORT_SEQ sortType, List<SortDialogFavoriteSortListContext> sortCells)
    {
        if (SortButtons.Count == 0)
        {
            SetUpSortList();
        }

        for (int i = 0; i < SortButtons.Count; ++i)
        {
            SortButtons[i].IsSelect = (sortType == SortButtons[i].SortType);

            bool enabledButton = true;
            if (SortButtons[i].IsSelect == false && sortCells != null)
            {
                for (int j = 0; j < sortCells.Count; ++j)
                {
                    if (sortCells[j].SortType == MAINMENU_SORT_SEQ.SEQ_INIT)
                    {
                        continue;
                    }

                    if (SortButtons[i].SortType == sortCells[j].SortType)
                    {
                        enabledButton = false;
                        break;
                    }
                }
            }
            SortButtons[i].IsButtonEnable = enabledButton;
        }
    }

    /// <summary>
    /// ソート項目の追加
    /// </summary>
    /// <param name="sortType"></param>
    void AddSortData(MAINMENU_SORT_SEQ sortType)
    {
        SortDialogTextButtonListContext sortButton = new SortDialogTextButtonListContext();
        sortButton.SortType = sortType;
        if (sortType != MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MAX)
        {
            sortButton.OffNameText = GameTextUtil.GetSortDialogSortText(sortType);
            sortButton.OnNameText = string.Format(GameTextUtil.GetText("filter_choice"), GameTextUtil.GetSortDialogSortText(sortType));
            sortButton.OffTextColor = ColorUtil.COLOR_WHITE;
            sortButton.OnTextColor = ColorUtil.COLOR_YELLOW;
            sortButton.DidSelectItem = OnClickSortButton;
        }
        SortButtons.Add(sortButton);
    }

    /// <summary>
    /// ボタンが押された
    /// </summary>
    /// <param name="sortButton"></param>
    void OnClickSortButton(SortDialogTextButtonListContext sortButton)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (OnClickSortButtonAction != null)
        {
            OnClickSortButtonAction(sortButton);
        }
    }

}
