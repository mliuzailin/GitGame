using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionListItem : ListItem<RegionContext>
{
    public void OnSelect()
    {
        Context.DidSelectItem(Context);
    }
}
