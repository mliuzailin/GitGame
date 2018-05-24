using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class DialogIconItem : M4uContext
{
    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<string> paramValue = new M4uProperty<string>();
    public string ParamValue { get { return paramValue.Value; } set { paramValue.Value = value; } }

    public DialogIconItem()
    {
        IconImage = null;
        ParamValue = "";
    }
}
