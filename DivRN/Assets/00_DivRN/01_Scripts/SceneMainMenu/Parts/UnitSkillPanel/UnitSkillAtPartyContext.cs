using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitSkillAtPartyContext : UnitSkillContext
{
    public UnitSkillAtPartyContext(ListItemModel listItemModel = null)
    {
        m_model = listItemModel;
    }

    private ListItemModel m_model;
    public ListItemModel model { get { return m_model; } }
}
