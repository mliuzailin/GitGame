using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class ScratchLineUpLine : M4uContextMonoBehaviour
{

    M4uProperty<List<LineUpListItemContex>> itemList = new M4uProperty<List<LineUpListItemContex>>();
    public List<LineUpListItemContex> ItemList { get { return itemList.Value; } set { itemList.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        ItemList = new List<LineUpListItemContex>();
    }
}
