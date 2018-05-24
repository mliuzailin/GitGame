/**
 *  @file   SortDialogFavoriteSortListContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/20
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;

public class SortDialogFavoriteSortListContext : M4uContext
{
    public Action<SortDialogFavoriteSortListContext> OnClickOrderAction = delegate { };
    public Action<SortDialogFavoriteSortListContext> OnClickFilterAction = delegate { };

    public MAINMENU_SORT_SEQ SortType;

    M4uProperty<string> priorityText = new M4uProperty<string>();
    /// <summary>優先順のテキスト</summary>
    public string PriorityText
    {
        get
        {
            return priorityText.Value;
        }
        set
        {
            priorityText.Value = value;
        }
    }

    M4uProperty<string> detailFilterText = new M4uProperty<string>();
    /// <summary>フィルター内容テキスト</summary>
    public string DetailFilterText
    {
        get
        {
            return detailFilterText.Value;
        }
        set
        {
            detailFilterText.Value = value;
        }
    }

    M4uProperty<Color> detailFilterTextColor = new M4uProperty<Color>(ColorUtil.COLOR_YELLOW);
    /// <summary>フィルター内容テキストカラー</summary>
    public Color DetailFilterTextColor
    {
        get
        {
            return detailFilterTextColor.Value;
        }
        set
        {
            detailFilterTextColor.Value = value;
        }
    }

    M4uProperty<bool> isAscOrder = new M4uProperty<bool>(true);
    /// <summary>昇順かどうか</summary>
    public bool IsAscOrder
    {
        get
        {
            return isAscOrder.Value;
        }
        set
        {
            isAscOrder.Value = value;
        }
    }

    M4uProperty<string> onAscOrderText = new M4uProperty<string>();
    public string OnAscOrderText
    {
        get
        {
            return onAscOrderText.Value;
        }
        set
        {
            onAscOrderText.Value = value;
        }
    }

    M4uProperty<string> offAscOrderText = new M4uProperty<string>();
    public string OffAscOrderText
    {
        get
        {
            return offAscOrderText.Value;
        }
        set
        {
            offAscOrderText.Value = value;
        }
    }

    M4uProperty<Color> onAscOrderTextColor = new M4uProperty<Color>();
    public Color OnAscOrderTextColor
    {
        get
        {
            return onAscOrderTextColor.Value;
        }
        set
        {
            onAscOrderTextColor.Value = value;
        }
    }

    M4uProperty<Color> offAscOrderTextColor = new M4uProperty<Color>();
    public Color OffAscOrderTextColor
    {
        get
        {
            return offAscOrderTextColor.Value;
        }
        set
        {
            offAscOrderTextColor.Value = value;
        }
    }

    M4uProperty<string> onDescOrderText = new M4uProperty<string>();
    public string OnDescOrderText
    {
        get
        {
            return onDescOrderText.Value;
        }
        set
        {
            onDescOrderText.Value = value;
        }
    }

    M4uProperty<string> offDescOrderText = new M4uProperty<string>();
    public string OffDescOrderText
    {
        get
        {
            return offDescOrderText.Value;
        }
        set
        {
            offDescOrderText.Value = value;
        }
    }

    M4uProperty<Color> onDescOrderTextColor = new M4uProperty<Color>();
    public Color OnDescOrderTextColor
    {
        get
        {
            return onDescOrderTextColor.Value;
        }
        set
        {
            onDescOrderTextColor.Value = value;
        }
    }

    M4uProperty<Color> offDescOrderTextColor = new M4uProperty<Color>();
    public Color OffDescOrderTextColor
    {
        get
        {
            return offDescOrderTextColor.Value;
        }
        set
        {
            offDescOrderTextColor.Value = value;
        }
    }

    public void SetUpOderText()
    {
        OnAscOrderText = string.Format(GameTextUtil.GetText("filter_choice"), GameTextUtil.GetText("filter_text19"));
        OffAscOrderText = GameTextUtil.GetText("filter_text19");
        OnDescOrderText = string.Format(GameTextUtil.GetText("filter_choice"), GameTextUtil.GetText("filter_text20"));
        OffDescOrderText = GameTextUtil.GetText("filter_text20");
        OnAscOrderTextColor = ColorUtil.COLOR_YELLOW;
        OffAscOrderTextColor = ColorUtil.COLOR_WHITE;
        OnDescOrderTextColor = ColorUtil.COLOR_YELLOW;
        OffDescOrderTextColor = ColorUtil.COLOR_WHITE;
    }
}
