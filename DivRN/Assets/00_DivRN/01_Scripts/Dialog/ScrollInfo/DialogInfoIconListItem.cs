using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInfoIconListItem : ListItem<DialogInfoIconContext>
{
    public void OnSelect()
    {
        Context.DidSelectIcon(Context.m_CharaId);
    }
}
