using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class DialogInfoIconList : M4uContextMonoBehaviour
{
    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<List<DialogInfoIconContext>> iconList = new M4uProperty<List<DialogInfoIconContext>>();
    public List<DialogInfoIconContext> IconList { get { return iconList.Value; } set { iconList.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        IconList = new List<DialogInfoIconContext>();
    }

    public void setup(string _title, uint[] _icon_ids, System.Action<uint> _action)
    {
        Title = _title;
        IconList.Clear();
        for (int i = 0; i < _icon_ids.Length; i++)
        {
            DialogInfoIconContext _newData = new DialogInfoIconContext();
            _newData.setup(_icon_ids[i]);
            _newData.DidSelectIcon = _action;
            IconList.Add(_newData);
        }
    }
}
