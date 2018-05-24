using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemKeyListItem : ListItem<ItemKeyContext>
{
    public void OnSelectItemKey()
    {
        Context.DidSelectItemKey(Context);
    }
}
