/**
 *  @file   DialogSortItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/16
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using M4u;

public class DialogSortItem : M4uContext
{
    /// <summary>ソートの種類</summary>
    public MAINMENU_SORT_SEQ SortType;

    /// <summary>アイテムを押したときのアクション</summary>
    public Action<MAINMENU_SORT_SEQ> DelSelectDialogSort = delegate { };

    M4uProperty<ColorBlock> buttonColorBlock = new M4uProperty<ColorBlock>(ColorBlock.defaultColorBlock);
    public ColorBlock ButtonColorBlock
    {
        get
        {
            return buttonColorBlock.Value;
        }
        set
        {
            buttonColorBlock.Value = value;
        }
    }

    M4uProperty<string> detailText = new M4uProperty<string>("＞デフォルト");
    /// <summary>
    /// テキスト
    /// </summary>
    public string DetailText
    {
        get
        {
            return detailText.Value;
        }
        set
        {
            detailText.Value = value;
        }
    }
}
