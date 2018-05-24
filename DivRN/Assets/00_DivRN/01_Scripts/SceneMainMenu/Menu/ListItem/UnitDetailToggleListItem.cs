using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDetailToggleListItem : ListItem<UnitDetailToggleContext>
{
    public void OnSelect()
    {
        Context.DidSelected(Context.m_Type, true);
    }
}
