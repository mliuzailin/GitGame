/**
 *  @file   SortDialogChoiceButtonListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/17
 */

using UnityEngine;
using System.Collections;
using M4u;

public class SortDialogTextButtonListItem : ListItem<SortDialogTextButtonListContext>
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        if (Context.DidSelectItem != null)
        {
            Context.DidSelectItem(Context);
        }
    }

}
