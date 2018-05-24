using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class UnitMaterialContext : M4uContextMonoBehaviour
{
    public Action<UnitMaterialContext> DidSelectMaterial = delegate { };

    private M4uProperty<bool> isActiveButton = new M4uProperty<bool>();
    public bool IsActiveButton { get { return isActiveButton.Value; } set { isActiveButton.Value = value; } }

    private M4uProperty<bool> isViewIcon = new M4uProperty<bool>();
    public bool IsViewIcon { get { return isViewIcon.Value; } set { isViewIcon.Value = value; } }

    private M4uProperty<Sprite> icon = new M4uProperty<Sprite>();
    public Sprite Icon { get { return icon.Value; } set { icon.Value = value; } }

    private M4uProperty<Color> iconColor = new M4uProperty<Color>();
    public Color IconColor { get { return iconColor.Value; } set { iconColor.Value = value; } }

    private M4uProperty<Sprite> frame = new M4uProperty<Sprite>();
    public Sprite Frame { get { return frame.Value; } set { frame.Value = value; } }

    public uint m_UnitId = 0;
    public long m_UnitUniqueId = 0;

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSelect()
    {
        DidSelectMaterial(this);
    }
}
