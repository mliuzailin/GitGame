using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class MaterialDataContext : M4uContext
{
    public MaterialDataContext(ListItemModel model)
    {
        m_model = model;
		MaterialColor = Color.white;
    }

    private ListItemModel m_model = null;
    public ListItemModel model { get { return m_model; } }


    private readonly float DEFAULT_SIZE = 102.0f;


    public int m_Id = 0;
    public uint m_CharaId = 0;
    public long m_UniqueId = 0;
    public bool m_bGray = false;
    public bool m_bWarning = false;
    public bool m_bEggWarning = false;

    M4uProperty<float> width = new M4uProperty<float>();
    public float Width { get { return width.Value; } set { width.Value = value; } }

    M4uProperty<float> height = new M4uProperty<float>();
    public float Height { get { return height.Value; } set { height.Value = value; } }

    M4uProperty<float> scaleX = new M4uProperty<float>();
    public float ScaleX { get { return scaleX.Value; } set { scaleX.Value = value; } }

    M4uProperty<float> scaleY = new M4uProperty<float>();
    public float ScaleY { get { return scaleY.Value; } set { scaleY.Value = value; } }

    M4uProperty<bool> isViewIcon = new M4uProperty<bool>();
    public bool IsViewIcon { get { return isViewIcon.Value; } set { isViewIcon.Value = value; } }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<Color> iconColor = new M4uProperty<Color>();
    public Color IconColor { get { return iconColor.Value; } set { iconColor.Value = value; } }

    M4uProperty<Sprite> selectImage = new M4uProperty<Sprite>();
    public Sprite SelectImage { get { return selectImage.Value; } set { selectImage.Value = value; } }

	M4uProperty<Color> materialColor = new M4uProperty<Color>();
	public Color MaterialColor { get { return materialColor.Value; } set { materialColor.Value = value; } }

	public System.Action<MaterialDataContext> DidSelectItem = delegate { };

    public void calcScale()
    {
        float _scale = Width / DEFAULT_SIZE;
        ScaleX = _scale;
        ScaleY = _scale;
    }

    public void reset()
    {
        IsViewIcon = false;
        IconImage = null;
        m_UniqueId = 0;
    }

    public void setUnit(long unique_id)
    {
        m_UniqueId = unique_id;
        if (m_UniqueId != 0)
        {
            IconColor = new Color(1.0f, 1.0f, 1.0f);
        }
        else
        {
            IconColor = new Color(0.3f, 0.3f, 0.3f);
        }
    }
}
