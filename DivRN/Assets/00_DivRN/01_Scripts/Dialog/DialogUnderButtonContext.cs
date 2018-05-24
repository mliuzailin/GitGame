using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class DialogUnderButtonContext : M4uContext
{
    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    public System.Action DidSelectItem = delegate { };
}
