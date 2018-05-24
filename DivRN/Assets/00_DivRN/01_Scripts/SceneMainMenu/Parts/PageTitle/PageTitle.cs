using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class PageTitle : MenuPartsBase
{
    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }
}
