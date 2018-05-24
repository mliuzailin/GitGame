using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitResultButton : MenuPartsBase
{
    M4uProperty<bool> isActiveButton1 = new M4uProperty<bool>();
    public bool IsActiveButton1 { get { return isActiveButton1.Value; } set { isActiveButton1.Value = value; } }

    M4uProperty<string> button1Text = new M4uProperty<string>();
    public string Button1Text { get { return button1Text.Value; } set { button1Text.Value = value; } }

    M4uProperty<bool> isActiveButton2 = new M4uProperty<bool>();
    public bool IsActiveButton2 { get { return isActiveButton2.Value; } set { isActiveButton2.Value = value; } }

    M4uProperty<string> button2Text = new M4uProperty<string>();
    public string Button2Text { get { return button2Text.Value; } set { button2Text.Value = value; } }

    M4uProperty<bool> isActiveSkip = new M4uProperty<bool>();
    public bool IsActiveSkip { get { return isActiveSkip.Value; } set { isActiveSkip.Value = value; } }

    public System.Action DidSelectButton1 = delegate { };

    public System.Action DidSelectButton2 = delegate { };

    public System.Action DidSelectSkip = delegate { };

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    public void OnSelectButton1()
    {
        DidSelectButton1();
    }

    public void OnSelectButton2()
    {
        DidSelectButton2();
    }

    public void OnSelectSkip()
    {
        DidSelectSkip();
    }
}
