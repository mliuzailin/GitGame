using UnityEngine;
using System.Collections;

public class CircleButtonListItem : ListItem<CircleButtonListItemContex>
{
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // クリック時のフィードバック
    public void OnClicked()
    {
        Context.DidSelectItem(Context);
#if BUILD_TYPE_DEBUG
        Debug.Log("[Debug:RecordListItem - OnClicked]Id:" + Context.Id + " UnitId:" + Context.UnitId);
#endif
    }
}
