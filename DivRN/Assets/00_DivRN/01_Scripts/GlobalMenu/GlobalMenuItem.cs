using UnityEngine;
using System.Collections;
using System;
using M4u;

public class GlobalMenuItem : M4uContext
{
    private GlobalMenuListItemModel m_model = null;
    public GlobalMenuListItemModel model { get { return m_model; } }

    public GlobalMenuItem(GlobalMenuButtonType type, Sprite title, Sprite icon, Action<GlobalMenuButtonType> action, bool bFlag = false)
    {
        Type = type;
        Title = title;
        Icon = icon;
        DelSelectDialogMenu = action;
        IsActiveFlag = bFlag;
    }

    public GlobalMenuItem SetModel(GlobalMenuListItemModel model)
    {
        m_model = model;
        return this;
    }


    M4uProperty<Sprite> title = new M4uProperty<Sprite>();
    public Sprite Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<Sprite> icon = new M4uProperty<Sprite>();
    public Sprite Icon { get { return icon.Value; } set { icon.Value = value; } }

    M4uProperty<bool> isActiveFlag = new M4uProperty<bool>();
    public bool IsActiveFlag { get { return isActiveFlag.Value; } set { isActiveFlag.Value = value; } }

    public GlobalMenuButtonType Type;

    public Action<GlobalMenuButtonType> DelSelectDialogMenu = delegate { };
}
