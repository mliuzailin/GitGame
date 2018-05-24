using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataListItem : ListItem<ItemDataContext>
{
    public void OnSelectItem()
    {
        Context.DidSelectItem(Context);
    }
}
