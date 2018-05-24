/**
 *  @file   DebugGetUnitListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/14
 */

using UnityEngine;
using System.Collections;

public class DebugGetUnitListItem : ListItem<DebugGetUnitListItemContext>
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnLongPress()
    {
        if (Context.LongPressAction != null)
        {
            Context.LongPressAction(Context);
        }
    }

    public void OnClick()
    {
        if (Context.ClickAction != null)
        {
            Context.ClickAction(Context);
        }
    }

}
