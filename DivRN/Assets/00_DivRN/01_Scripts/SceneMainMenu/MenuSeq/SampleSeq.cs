using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メインメニューの基本コード
/// </summary>
public class SampleSeq : MainMenuSeq
{

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }
}
