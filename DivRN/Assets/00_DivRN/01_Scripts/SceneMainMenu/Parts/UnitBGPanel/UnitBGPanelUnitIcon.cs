/**
 *  @file   UnitBGPanelUnitIcon.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/08/02
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;

public class UnitBGPanelUnitIcon : MenuPartsBase
{
    public uint chara_fix_id = 0;

    M4uProperty<bool> isActiveIcon = new M4uProperty<bool>();
    /// <summary>ユニットアイコンの表示・非表示</summary>
    public bool IsActiveIcon { get { return isActiveIcon.Value; } set { isActiveIcon.Value = value; } }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    /// <summary>ユニットアイコン</summary>
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<Color> iconColor = new M4uProperty<Color>(new Color(1, 1, 1, 1));
    /// <summary>ユニットアイコンの色</summary>
    public Color IconColor { get { return iconColor.Value; } set { iconColor.Value = value; } }

    M4uProperty<uint> charaNo = new M4uProperty<uint>();
    /// <summary>ユニットナンバー</summary>
    public uint CharaNo { get { return charaNo.Value; } set { charaNo.Value = value; } }

    M4uProperty<Sprite> linkIcon = new M4uProperty<Sprite>();
    /// <summary>リンクアイコン</summary>
    public Sprite LinkIcon
    {
        get { return linkIcon.Value; }
        set
        {
            IsActiveLinkIcon = (value != null);
            linkIcon.Value = value;
        }
    }

    M4uProperty<bool> isActiveLinkIcon = new M4uProperty<bool>(false);
    /// <summary>リンクアイコンの表示・非表示</summary>
    private bool IsActiveLinkIcon { get { return isActiveLinkIcon.Value; } set { isActiveLinkIcon.Value = value; } }

    M4uProperty<Sprite> attribute_circle = new M4uProperty<Sprite>();
    /// <summary>ユニットアイコン</summary>
    public Sprite Attribute_circle { get { return attribute_circle.Value; } set { attribute_circle.Value = value; } }

    public System.Action DidSelectIcon = delegate { };
    public System.Action DidSelectIconLongpress = delegate { };

    /// <summary>
    /// オブジェクトの生成
    /// </summary>
    /// <param name="_transform"></param>
    /// <returns></returns>
    static public UnitBGPanelUnitIcon Create(Transform _transform)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefab/UnitBGPanel/UnitBGPanelUnitIcon");
        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab);
            if (obj != null)
            {
                obj.transform.SetParent(_transform, false);
                UnitBGPanelUnitIcon unitIcon = obj.GetComponent<UnitBGPanelUnitIcon>();
                unitIcon.SetPosition(Vector2.zero);
                return unitIcon;
            }
        }
        return null;
    }

    void Awake()
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

    public void OnClick()
    {
        if (ButtonBlocker.Instance.IsActive())
        {
            return;
        }

        if (DidSelectIcon != null)
        {
            DidSelectIcon();
        }
    }

    public void OnLongPress()
    {
        if (ButtonBlocker.Instance.IsActive())
        {
            return;
        }

        if (DidSelectIconLongpress != null)
        {
            DidSelectIconLongpress();
        }
    }
}
