using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class UnitMaterialPanel : MenuPartsBase
{
    public enum ArrowType
    {
        SIDE = 0,
        UP,
    };

    private static RectOffset sideOffset = null;
    private static RectOffset upOffset = null;

    private UnitResultBuildupModel m_model = null;

    M4uProperty<Sprite> bgImage = new M4uProperty<Sprite>();
    public Sprite BgImage { get { return bgImage.Value; } set { bgImage.Value = value; } }

    M4uProperty<List<MaterialDataContext>> materialList = new M4uProperty<List<MaterialDataContext>>();
    public List<MaterialDataContext> MaterialList { get { return materialList.Value; } set { materialList.Value = value; } }

	List<GameObject> objectList = new List<GameObject>();
	public List<GameObject> ObjectList { get { return objectList; } set { objectList = value; } }

	M4uProperty<Color> panelColor = new M4uProperty<Color>();
	public Color PanelColor { get { return panelColor.Value; } set { panelColor.Value = value; } }

	private List<ListItemModel> m_iconModels = new List<ListItemModel>();

    private HorizontalLayoutGroup LayoutGroup = null;
    private float m_IconSize = 102.0f;

    private void Awake()
    {
        if (sideOffset == null)
            sideOffset = new RectOffset(17, 8, 6, 33);
        if (upOffset == null)
            upOffset = new RectOffset(33, 8, 6, 6);

        GetComponent<M4uContextRoot>().Context = this;
        LayoutGroup = GetComponent<HorizontalLayoutGroup>();

        setup(ArrowType.SIDE);

        MaterialList = new List<MaterialDataContext>();

		PanelColor = Color.white;
    }

    public void setup(ArrowType _tyep)
    {
        Debug.Assert(sideOffset != null, "Dont Call setup() before awake().");
        Debug.Assert(upOffset != null, "Dont Call setup() before awake().");

        switch (_tyep)
        {
            case ArrowType.SIDE:
                BgImage = ResourceManager.Instance.Load("unit_material");
                LayoutGroup.padding = sideOffset;
                break;
            case ArrowType.UP:
                BgImage = ResourceManager.Instance.Load("unit_material_up");
                LayoutGroup.padding = upOffset;
                break;
        }
    }

    public void setIconSize(float _size)
    {
        m_IconSize = _size;
        m_iconModels.Clear();
    }

    public void setIconSpaceSize(float _size)
    {
        LayoutGroup.spacing = _size;
    }

    public UnitMaterialPanel SetModel(UnitResultBuildupModel model)
    {
        m_model = model;
        return this;
    }


    public void addItem(int _id, uint _unit_id, System.Action<MaterialDataContext> action = null, bool notVierw = false)
    {
        var iconModel = new ListItemModel((uint)_id);

        MaterialDataContext _newData = new MaterialDataContext(iconModel);
        _newData.m_Id = _id;
        _newData.m_CharaId = _unit_id;
        if (_unit_id == 0)
        {
            _newData.IsViewIcon = false;
        }
        else
        {
            _newData.IsViewIcon = true;
            UnitIconImageProvider.Instance.Get(
                _unit_id,
                sprite =>
                {
                    _newData.IconImage = sprite;
                },
                true);
        }
        _newData.IconColor = new Color(1, 1, 1);
		if (notVierw == true)
		{
			_newData.MaterialColor = Color.clear;
		}

		_newData.Width = m_IconSize;
        _newData.Height = m_IconSize;
        _newData.calcScale();

        _newData.SelectImage = ResourceManager.Instance.Load("icon_square1");

        iconModel.OnClicked += () =>
        {
            if (action != null) action(_newData);
        };

        MaterialList.Add(_newData);
        m_iconModels.Add(iconModel);
    }

    public void SetViewIcon(int index)
    {
        if (m_model == null
            || m_iconModels == null
            || m_iconModels.Count <= index
            || index < 0)
        {
            return;
        }

        m_model.AddMaterial(m_iconModels[index]);
    }

    public void UnsetViewIcon(int index)
    {
        if (m_model == null
            || m_iconModels == null
            || m_iconModels.Count <= index
            || m_model.LeftMaterialCount() <= index
            || index < 0)
        {
            return;
        }

        // MaterialListが自身はそのままで中身のデータだけソートしている変則的なつくりなので合わせるために最後だけ削ってつじつまを合わせる
        m_model.RemoveLastMaterial();
    }

    public void setMaterialWarning(long unique_id, bool flag)
    {
        if (unique_id == 0)
        {
            return;
        }

        MaterialDataContext data = MaterialList.Find(a => a.m_UniqueId == unique_id);
        if (data != null)
        {
            data.m_bWarning = flag;
        }
    }

    public void setEggWarning(long unique_id, bool flag)
    {
        if (unique_id == 0)
        {
            return;
        }

        MaterialDataContext data = MaterialList.Find(a => a.m_UniqueId == unique_id);
        if (data != null)
        {
            data.m_bEggWarning = flag;
        }

    }

    public void resetWarning()
    {
        for(int i=0;i< MaterialList.Count;i++)
        {
            MaterialList[i].m_bWarning = false;
            MaterialList[i].m_bEggWarning = false;
        }
    }
}
