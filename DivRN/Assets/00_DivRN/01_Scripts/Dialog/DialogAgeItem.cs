using UnityEngine;
using System;
using System.Collections;
using M4u;


public class DialogAgeItem : M4uContext
{

    public DialogAgeItem(string label, string limit, DialogButtonEventType type)
    {
        Age_label = label;
        buttonType = type;
    }

    M4uProperty<string> age_label = new M4uProperty<string>();
    public string Age_label { get { return age_label.Value; } set { age_label.Value = value; } }

    public DialogButtonEventType buttonType = DialogButtonEventType.NONE;

    public System.Action<DialogButtonEventType> DidSelectItem = delegate { };

}
