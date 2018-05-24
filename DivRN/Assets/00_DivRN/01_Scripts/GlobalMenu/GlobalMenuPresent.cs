using UnityEngine;
using System.Collections;
using System;
using M4u;

public class GlobalMenuPresent : M4uContext
{
    public GlobalMenuPresent(GlobalMenuButtonType type, string title, Sprite icon, Action<GlobalMenuButtonType> action)
    {
        Type = type;
        Title = title;
        Icon = icon;
        DelSelectDialogMenu = action;
    }

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<Sprite> icon = new M4uProperty<Sprite>();
    public Sprite Icon { get { return icon.Value; } set { icon.Value = value; } }

    public GlobalMenuButtonType Type;

    public Action<GlobalMenuButtonType> DelSelectDialogMenu = delegate { };
}
