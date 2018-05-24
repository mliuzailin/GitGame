using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class RegionContext : M4uContext
{
    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<Texture> iconImage_mask = new M4uProperty<Texture>();
    public Texture IconImage_mask { get { return iconImage_mask.Value; } set { iconImage_mask.Value = value; } }

    M4uProperty<bool> isSelect = new M4uProperty<bool>();
    public bool IsSelect { get { return isSelect.Value; } set { isSelect.Value = value; } }

    public System.Action<RegionContext> DidSelectItem = delegate { };

    public MasterDataRegion master = null;
}
