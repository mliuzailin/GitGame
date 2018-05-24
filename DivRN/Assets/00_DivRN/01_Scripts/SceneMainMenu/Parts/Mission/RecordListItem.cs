using UnityEngine;
using System.Collections;

public class RecordListItem : ListItem<RecordListItemContex>
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
        Debug.Log("[Debug:RecordListItem - OnClicked]" + Context.CaptionText01);
#endif
    }
}
