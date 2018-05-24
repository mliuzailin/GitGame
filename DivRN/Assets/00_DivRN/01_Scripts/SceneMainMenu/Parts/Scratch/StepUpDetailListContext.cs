using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using System;

public class StepUpDetailListContext : M4uContext
{
    public Action FinishLoadImageAction;
    public ButtonModel LinUpNormalButtonModel = new ButtonModel();
    public ButtonModel LinUpRainbowButtonModel = new ButtonModel();

    public uint StepManageID = 0;
    public string Banner_url;


    M4uProperty<string> titleText = new M4uProperty<string>();
    public string TitleText { get { return titleText.Value; } set { titleText.Value = value; } }

    M4uProperty<string> lotExecText = new M4uProperty<string>();
    public string LotExecText { get { return lotExecText.Value; } set { lotExecText.Value = value; } }

    M4uProperty<string> priceText = new M4uProperty<string>();
    public string PriceText { get { return priceText.Value; } set { priceText.Value = value; } }

    M4uProperty<string> bonusText = new M4uProperty<string>();
    public string BonusText { get { return bonusText.Value; } set { bonusText.Value = value; } }

    M4uProperty<string> detailText = new M4uProperty<string>();
    public string DetailText
    {
        get { return detailText.Value; }
        set
        {
            detailText.Value = value;
            IsViewDetailLabel = (value.IsNullOrEmpty() == false);
        }
    }

    M4uProperty<bool> isStep = new M4uProperty<bool>(true);
    /// <summary>true:ステップ項目 false:TOP項目</summary>
    public bool IsStep { get { return isStep.Value; } set { isStep.Value = value; } }

    M4uProperty<bool> isViewDetailLabel = new M4uProperty<bool>();
    private bool IsViewDetailLabel { get { return isViewDetailLabel.Value; } set { isViewDetailLabel.Value = value; } }

    M4uProperty<bool> isViewBornusLabel = new M4uProperty<bool>(false);
    public bool IsViewBornusLabel { get { return isViewBornusLabel.Value; } set { isViewBornusLabel.Value = value; } }

    M4uProperty<bool> isViewLineUpNormal = new M4uProperty<bool>();
    public bool IsViewLineUpNormal { get { return isViewLineUpNormal.Value; } set { isViewLineUpNormal.Value = value; } }

    M4uProperty<bool> isViewLineUpRainbow = new M4uProperty<bool>();
    public bool IsViewLineUpRainbow { get { return isViewLineUpRainbow.Value; } set { isViewLineUpRainbow.Value = value; } }
}
