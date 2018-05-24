using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogMenuListItem : ListItem<DialogMenuItem>
{

    public DialogMenuItem DialogMenuItem
    {
        get
        {
            return (DialogMenuItem)Context;
        }
    }

    void Start()
    {
        if (DialogMenuItem.Title_active == false)
        {
            LayoutElement layoutElement = gameObject.GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                layoutElement.preferredHeight = 60;
            }
        }
    }

    public void OnSelect()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("SelectDialogMenu:" + DialogMenuItem.Button_title);
#endif
        DialogMenuItem.DelSelectDialogMenu(DialogMenuItem);
        DialogMenuItem.DidSelectAction();
    }
}
