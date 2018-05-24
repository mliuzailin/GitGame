using UnityEngine;
using System;
using System.Collections;
using M4u;


public class DialogMenuItem : M4uContext
{

    public DialogMenuItem(string title, string button_title, Action<DialogMenuItem> action)
    {
        Title = title;
        if (Title != "")
        {
            Title_active = true;
        }
        Button_title = button_title;
        DelSelectDialogMenu = action;
    }

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<bool> title_active = new M4uProperty<bool>();
    public bool Title_active { get { return title_active.Value; } set { title_active.Value = value; } }

    M4uProperty<string> button_title = new M4uProperty<string>();
    public string Button_title { get { return button_title.Value; } set { button_title.Value = value; } }

    public Action<DialogMenuItem> DelSelectDialogMenu = delegate { };
    public Action DidSelectAction = delegate { };

}
