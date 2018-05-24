using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitCatalogLine : M4uContextMonoBehaviour
{
    M4uProperty<List<UnitCatalogItemContext>> itemList = new M4uProperty<List<UnitCatalogItemContext>>();
    public List<UnitCatalogItemContext> ItemList { get { return itemList.Value; } set { itemList.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        ItemList = new List<UnitCatalogItemContext>();
    }
}
