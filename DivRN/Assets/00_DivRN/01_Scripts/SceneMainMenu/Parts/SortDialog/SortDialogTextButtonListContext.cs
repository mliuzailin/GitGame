/**
 *  @file   SortDialogChoiceButtonListContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/17
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;

public class SortDialogTextButtonListContext : M4uContext
{
    public Action<SortDialogTextButtonListContext> DidSelectItem = delegate { };

    public MAINMENU_SORT_SEQ SortType;
    public MAINMENU_FILTER_TYPE FilterType;
    /// <summary>表示フィルタータイプ</summary>
    public MasterDataDefineLabel.AchievementFilterType AchievementFilterType;
    /// <summary>種別受け取りタイプ</summary>
    public MasterDataDefineLabel.AchievementReceiveType AchievementReceiveType;

    M4uProperty<bool> isSelect = new M4uProperty<bool>(false);
    /// <summary>選択状態かどうか</summary>
    public bool IsSelect
    {
        get
        {
            return isSelect.Value;
        }
        set
        {
            isSelect.Value = value;
        }
    }

    M4uProperty<bool> isButtonEnable = new M4uProperty<bool>(true);
    public bool IsButtonEnable
    {
        get
        {
            return isButtonEnable.Value;
        }
        set
        {
            isButtonEnable.Value = value;
        }
    }

    M4uProperty<string> onNameText = new M4uProperty<string>();
    /// <summary>テキスト</summary>
    public string OnNameText
    {
        get
        {
            return onNameText.Value;
        }
        set
        {
            onNameText.Value = value;
        }
    }

    M4uProperty<string> offNameText = new M4uProperty<string>();
    /// <summary>テキスト</summary>
    public string OffNameText
    {
        get
        {
            return offNameText.Value;
        }
        set
        {
            offNameText.Value = value;
        }
    }

    M4uProperty<Color> onTextColor = new M4uProperty<Color>(Color.white);
    /// <summary>文字色</summary>
    public Color OnTextColor
    {
        get
        {
            return onTextColor.Value;
        }
        set
        {
            onTextColor.Value = value;
        }
    }

    M4uProperty<Color> offTextColor = new M4uProperty<Color>(Color.white);
    /// <summary>文字色</summary>
    public Color OffTextColor
    {
        get
        {
            return offTextColor.Value;
        }
        set
        {
            offTextColor.Value = value;
        }
    }

}
