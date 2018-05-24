/**
 *  @file   SortDialogSwitchListContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/17
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;

public class SortDialogSwitchListContext : M4uContext
{
    public Action<SortDialogSwitchListContext> DidSelectItem = delegate { };

    public MasterDataDefineLabel.RarityType RarityType;
    public MasterDataDefineLabel.ElementType ElementType;
    public MasterDataDefineLabel.KindType KindType;

    M4uProperty<string> nameText = new M4uProperty<string>("");
    /// <summary>名前</summary>
    public string NameText { get { return nameText.Value; } set { nameText.Value = value; } }

    M4uProperty<bool> isSelect = new M4uProperty<bool>();
    /// <summary>選択状態</summary>
    public bool IsSelect { get { return isSelect.Value; } set { isSelect.Value = value; } }


    M4uProperty<Sprite> nameImage = new M4uProperty<Sprite>();
    /// <summary>名前の画像</summary>
    public Sprite NameImage
    {
        get
        {
            return nameImage.Value;
        }
        set
        {
            nameImage.Value = value;
            IsActiveNameImage = (value != null);
        }
    }

    M4uProperty<bool> isActiveNameImage = new M4uProperty<bool>(false);
    /// <summary>名前の画像の表示・非表示</summary>
    public bool IsActiveNameImage { get { return isActiveNameImage.Value; } set { isActiveNameImage.Value = value; } }

}
