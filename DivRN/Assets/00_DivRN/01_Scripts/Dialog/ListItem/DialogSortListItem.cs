/**
 *  @file   DialogSortListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/16
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using M4u;

public class DialogSortListItem : ListItem<DialogSortItem>
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
        Context.DelSelectDialogSort(Context.SortType);
    }
}
