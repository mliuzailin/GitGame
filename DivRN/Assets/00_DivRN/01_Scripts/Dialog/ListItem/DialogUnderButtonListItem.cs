using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogUnderButtonListItem : ListItem<DialogUnderButtonContext>
{
    public void OnSelect()
    {
        Context.DidSelectItem();
    }
}
