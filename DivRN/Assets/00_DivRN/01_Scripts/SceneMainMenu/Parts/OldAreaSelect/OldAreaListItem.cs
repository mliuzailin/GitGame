using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldAreaListItem : ListItem<OldAreaContext>
{

    public void OnSelectArea()
    {
        Context.DidSelectArea(Context.m_AreaId);
    }
}
