using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class ItemEffectContext : M4uContext
{
    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<bool> isViewIcon = new M4uProperty<bool>();
    public bool IsViewIcon { get { return isViewIcon.Value; } set { isViewIcon.Value = value; } }

    M4uProperty<string> time = new M4uProperty<string>();
    public string Time { get { return time.Value; } set { time.Value = value; } }

    public MasterDataUseItem ItemMaster = null;
}
