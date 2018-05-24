/**
 *  @file   MissonListItemContext.cs
 *  @brief  クエストのミッションリストアイテムのM4uContext
 *  @author Developer
 *  @date   2016/11/14
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using M4u;

public class MissonListItemContext : M4uContext
{
    /// <summary>
    /// アイテムを選択したときのアクション
    /// </summary>
    public Action<MissonListItemContext> DidSelectItem = delegate { };

    /// <summary>
    /// アイコン画像
    /// </summary>
    M4uProperty<Sprite> icon = new M4uProperty<Sprite>();
    public Sprite Icon
    {
        get
        {
            return icon.Value;
        }
        set
        {
            icon.Value = value;
        }
    }

    /// <summary>
    /// Newの状態かどうか
    /// </summary>
    M4uProperty<bool> isActiveNew = new M4uProperty<bool>();
    public bool IsActiveNew
    {
        get
        {
            return isActiveNew.Value;
        }
        set
        {
            IconColor = (value) ? Color.white : ColorUtil.COLOR_GRAY;
            ContentTextColor = (value) ? ColorUtil.COLOR_WHITE : ColorUtil.COLOR_GRAY;
            DetailTextColor = (value) ? ColorUtil.COLOR_LIGHT_BLUE : ColorUtil.COLOR_GRAY;

            isActiveNew.Value = value;
        }
    }

    /// <summary>
    /// 説明文
    /// </summary>
    M4uProperty<string> contentText = new M4uProperty<string>();
    public string ContentText
    {
        get
        {
            return contentText.Value;
        }
        set
        {
            contentText.Value = value;
        }
    }

    /// <summary>
    /// 取得アイテム名
    /// </summary>
    M4uProperty<string> detailText = new M4uProperty<string>();
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

    /// <summary>
    /// アイコンの色
    /// </summary>
    M4uProperty<Color> iconColor = new M4uProperty<Color>(Color.white);
    public Color IconColor
    {
        get
        {
            return iconColor.Value;
        }
        set
        {
            iconColor.Value = value;
        }
    }

    /// <summary>
    /// 説明文の色
    /// </summary>
    M4uProperty<Color> contentTextColor = new M4uProperty<Color>(ColorUtil.COLOR_WHITE);
    public Color ContentTextColor
    {
        get
        {
            return contentTextColor.Value;
        }
        set
        {
            contentTextColor.Value = value;
        }
    }

    /// <summary>
    /// 取得アイテム名の色
    /// </summary>
    M4uProperty<Color> detailTextColor = new M4uProperty<Color>(ColorUtil.COLOR_LIGHT_BLUE);
    public Color DetailTextColor
    {
        get
        {
            return detailTextColor.Value;
        }
        set
        {
            detailTextColor.Value = value;
        }
    }
}
