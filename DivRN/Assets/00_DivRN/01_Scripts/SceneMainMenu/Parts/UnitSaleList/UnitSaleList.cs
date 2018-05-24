using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class UnitSaleList : MenuPartsBase
{
    static readonly float ICON_SIZE = 70.0f;

    private M4uProperty<List<MaterialDataContext>> unitList = new M4uProperty<List<MaterialDataContext>>();
    public List<MaterialDataContext> UnitList { get { return unitList.Value; } set { unitList.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        UnitList = new List<MaterialDataContext>();
    }


    public void addItem(int _id, uint _unit_id)
    {
        var model = new ListItemModel((uint)_id);

        MaterialDataContext _newData = new MaterialDataContext(model);
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

        _newData.Width = ICON_SIZE;
        _newData.Height = ICON_SIZE;
        _newData.calcScale();

        _newData.SelectImage = ResourceManager.Instance.Load("icon_square1");

        UnitList.Add(_newData);


        model.OnClicked += () =>
        {
            // TODO : 選んだ時の処理
        };
    }
}
