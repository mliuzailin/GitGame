using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class OthersInfo : MenuPartsBase
{
    M4uProperty<string> labelText = new M4uProperty<string>();
    public string LabelText { get { return labelText.Value; } set { labelText.Value = value; } }

    M4uProperty<List<OthersInfoListContext>> infos = new M4uProperty<List<OthersInfoListContext>>(new List<OthersInfoListContext>());
    public List<OthersInfoListContext> Infos
    {
        get
        {
            return infos.Value;
        }
        set
        {
            infos.Value = value;
        }
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    void Start()
    {
    }


}
