using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class SortDialogElementListContext : M4uContext
{
    public Action<SortDialogElementListContext> DidSelectItem = delegate { };

    public MasterDataDefineLabel.ElementType ElementType;

    M4uProperty<bool> isSelect = new M4uProperty<bool>();
    public bool IsSelect { get { return isSelect.Value; } set { isSelect.Value = value; } }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }
}
