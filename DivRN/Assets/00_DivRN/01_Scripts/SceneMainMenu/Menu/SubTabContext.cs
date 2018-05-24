using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class SubTabContext : M4uContext
{
    private ListItemModel m_model;
    public ListItemModel model { get { return m_model; } }

    public SubTabContext(ListItemModel listItemModel)
    {
        m_model = listItemModel;
    }

    public MAINMENU_SEQ m_MenuSeq = MAINMENU_SEQ.SEQ_NONE;

    public bool IsSelect { get; private set; }

    /// <summary>リスト判別用のID</summary>
    public int Index
    {
        get
        {
            return (int)model.index;
        }
    }

    M4uProperty<float> rotationZ = new M4uProperty<float>();
    private float RotationZ { get { return rotationZ.Value; } set { rotationZ.Value = value; } }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage
    {
        get { return iconImage.Value; }
        set
        {
            iconImage.Value = value;
            IsViewIconImage = (value != null);
        }
    }

    M4uProperty<bool> isViewIconImage = new M4uProperty<bool>();
    private bool IsViewIconImage { get { return isViewIconImage.Value; } set { isViewIconImage.Value = value; } }

    M4uProperty<Sprite> txtImage = new M4uProperty<Sprite>();
    public Sprite TxtImage { get { return txtImage.Value; } set { txtImage.Value = value; } }

    M4uProperty<bool> isViewTxtImage = new M4uProperty<bool>();
    public bool IsViewTxtImage { get { return isViewTxtImage.Value; } set { isViewTxtImage.Value = value; } }

    M4uProperty<string> tabName = new M4uProperty<string>();
    public string TabName { get { return tabName.Value; } set { tabName.Value = value; } }

    M4uProperty<int> alertCount = new M4uProperty<int>();
    public int AlertCount { get { return alertCount.Value; } set { alertCount.Value = value; } }

    M4uProperty<bool> isViewFlag = new M4uProperty<bool>();
    public bool IsViewFlag { get { return isViewFlag.Value; } set { isViewFlag.Value = value; } }

    public System.Action<SubTabContext> DidSelectTab = delegate { };

    public MasterDataGacha GachaMaster { get; set; }

    public void setFlag(bool bFlag)
    {
        if (bFlag)
        {
            IsSelect = true;
            RotationZ = -45.0f;
        }
        else
        {
            IsSelect = false;
            RotationZ = 0.0f;
        }
    }
}
