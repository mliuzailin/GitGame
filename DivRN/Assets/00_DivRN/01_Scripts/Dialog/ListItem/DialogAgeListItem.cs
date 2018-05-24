using UnityEngine;
using System.Collections;

public class DialogAgeListItem : ListItem<DialogAgeItem>
{

    public DialogAgeItem DialogAgeItem
    {
        get
        {
            return (DialogAgeItem)Context;
        }
    }


    public void OnSelect()
    {
        DialogAgeItem.DidSelectItem(DialogAgeItem.buttonType);
    }
}
