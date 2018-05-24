using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class UnitDetailToggleContext : M4uContext
{
    public UnitDetailInfo.ToggleType m_Type = UnitDetailInfo.ToggleType.None;

    public System.Action<UnitDetailInfo.ToggleType, bool> DidSelected = delegate { };

    M4uProperty<bool> flag = new M4uProperty<bool>();
    public bool Flag { get { return flag.Value; } set { flag.Value = value; } }

    M4uProperty<float> posX = new M4uProperty<float>();
    public float PosX { get { return posX.Value; } set { posX.Value = value; } }

    M4uProperty<float> posY = new M4uProperty<float>();
    public float PosY { get { return posY.Value; } set { posY.Value = value; } }

    M4uProperty<Sprite> onSprite = new M4uProperty<Sprite>();
    public Sprite OnSprite { get { return onSprite.Value; } set { onSprite.Value = value; } }

    M4uProperty<Sprite> offSprite = new M4uProperty<Sprite>();
    public Sprite OffSprite { get { return offSprite.Value; } set { offSprite.Value = value; } }

    M4uProperty<Sprite> textImage = new M4uProperty<Sprite>();
    public Sprite TextImage { get { return textImage.Value; } set { textImage.Value = value; } }

    M4uProperty<float> onY = new M4uProperty<float>();
    public float OnY { get { return onY.Value; } set { onY.Value = value; } }

    M4uProperty<float> offY = new M4uProperty<float>();
    public float OffY { get { return offY.Value; } set { offY.Value = value; } }

    public UnitDetailToggleContext(UnitDetailInfo.ToggleType _type, Vector2 _pos, string _spr_name, System.Action<UnitDetailInfo.ToggleType, bool> _action, float _touch_y)
    {
        Flag = false;
        m_Type = _type;
        PosX = _pos.x;
        PosY = _pos.y;
        DidSelected = _action;
        OnY = _touch_y;
        OffY = _touch_y;

        setupSprite(_spr_name);
    }

    public void setupSprite(string _spr_name)
    {
        OnSprite = ResourceManager.Instance.Load("btn_" + _spr_name + "_down");
        OffSprite = ResourceManager.Instance.Load("btn_" + _spr_name);
        TextImage = ResourceManager.Instance.Load("txt_" + _spr_name);
    }
}
