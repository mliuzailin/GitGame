/**
 *  @file   UnitGridLineListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/05/31
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class UnitGridLineListItem : MenuPartsBase
{

    M4uProperty<List<UnitGridContext>> items = new M4uProperty<List<UnitGridContext>>(new List<UnitGridContext>());
    public List<UnitGridContext> Items
    {
        get
        {
            return items.Value;
        }
        set
        {
            items.Value = value;
        }
    }

    public List<GameObject> ItemObjects = null;
    private int m_StartIndex = 0;
    public int StartIndex { get { return m_StartIndex; } set { m_StartIndex = value; } }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        Items = new List<UnitGridContext>();
        ItemObjects = new List<GameObject>();
    }
}
