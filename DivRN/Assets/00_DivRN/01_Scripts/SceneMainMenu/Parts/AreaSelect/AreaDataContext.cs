using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class AreaDataContext : M4uContext
{
    private AreaSelectListItemModel m_model;
    public AreaSelectListItemModel model { get { return m_model; } }

    public AreaDataContext(AreaSelectListItemModel listItemModel)
    {
        m_model = listItemModel;
    }


    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<Texture> iconImage_mask = new M4uProperty<Texture>();
    public Texture IconImage_mask { get { return iconImage_mask.Value; } set { iconImage_mask.Value = value; } }

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<float> posX = new M4uProperty<float>();
    public float PosX { get { return posX.Value; } set { posX.Value = value; } }

    M4uProperty<float> posY = new M4uProperty<float>();
    public float PosY { get { return posY.Value; } set { posY.Value = value; } }

    M4uProperty<bool> isAreaNew = new M4uProperty<bool>(false);
    /// <summary>クエストに入ったことがないか</summary>
    public bool IsAreaNew { get { return isAreaNew.Value; } set { isAreaNew.Value = value; } }

    M4uProperty<bool> isViewFlag = new M4uProperty<bool>(false);
    /// <summary>全てのクエストをクリアしているか</summary>
    public bool IsViewFlag { get { return isViewFlag.Value; } set { isViewFlag.Value = value; } }

    M4uProperty<Sprite> flagImage = new M4uProperty<Sprite>();
    public Sprite FlagImage { get { return flagImage.Value; } set { flagImage.Value = value; } }

    public uint m_AreaIndex = 0;

    public System.Action<uint> DidSelectArea = delegate { };

	public bool m_bufEvent = false;
}
