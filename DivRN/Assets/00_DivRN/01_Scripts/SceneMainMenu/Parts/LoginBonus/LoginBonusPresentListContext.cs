using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class LoginBonusPresentListContext : M4uContext
{
    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    /// <summary>アイコンイメージ</summary>
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<string> nameText = new M4uProperty<string>();
    /// <summary>名前</summary>
    public string NameText { get { return nameText.Value; } set { nameText.Value = value; } }

    M4uProperty<string> numText = new M4uProperty<string>();
    /// <summary>個数</summary>
    public string NumText { get { return numText.Value; } set { numText.Value = value; } }

    M4uProperty<string> numRate = new M4uProperty<string>();
    /// <summary>個数の単位</summary>
    public string NumRate { get { return numRate.Value; } set { numRate.Value = value; } }

    M4uProperty<bool> isViewBorder = new M4uProperty<bool>(true);
    public bool IsViewBorder { get { return isViewBorder.Value; } set { isViewBorder.Value = value; } }
}
