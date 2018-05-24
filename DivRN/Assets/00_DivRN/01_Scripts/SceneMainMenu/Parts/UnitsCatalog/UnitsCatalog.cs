using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitsCatalog : M4uContextMonoBehaviour
{
    // TitleText
    M4uProperty<string> titleText = new M4uProperty<string>();
    public string TitleText { get { return titleText.Value; } set { titleText.Value = value; } }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    void Start()
    {
    }


}
