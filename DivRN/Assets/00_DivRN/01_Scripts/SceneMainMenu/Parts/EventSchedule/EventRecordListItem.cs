using UnityEngine;
using System.Collections;

public class EventRecordListItem : ListItem<EventRecordListItemContex>
{
    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR
        gameObject.name = string.Format("fix_id:{0}", Context.FixId);
#endif
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
        //Debug.Log("[Debug:RecordListItem - OnClicked]" + Context.CaptionText01);
#endif
    }
}
