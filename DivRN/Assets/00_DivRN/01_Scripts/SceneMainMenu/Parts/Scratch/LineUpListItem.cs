using UnityEngine;
using System.Collections;

public class LineUpListItem : ListItem<LineUpListItemContex>
{
    // クリック時のフィードバック
    public void OnClicked()
    {
        Context.DidSelectItem(Context);
    }
}
