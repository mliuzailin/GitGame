using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCatalogItemListItem : ListItem<UnitCatalogItemContext>
{
    public void OnSelectItem()
    {
        Context.DidSelectItem(Context.master.fix_id);
    }

    public void OnLongPress()
    {
        Context.DidLongPressItem(Context.master.fix_id);
    }
}
