using UnityEngine;
using System.Collections;

public class DialogTipListItem : ListItem<DialogTipItem>
{

    public DialogTipItem DialogTipItem
    {
        get
        {
            return (DialogTipItem)Context;
        }
    }


    public void OnSelect()
    {
        DialogTipItem.DidSelectItem(DialogTipItem);
    }
}
