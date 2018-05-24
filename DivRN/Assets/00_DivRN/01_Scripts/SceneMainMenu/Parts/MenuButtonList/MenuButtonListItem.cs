using UnityEngine;
using System.Collections;

public class MenuButtonListItem : ListItem<MenuButtonContext>
{
    public MenuButtonContext Item
    {
        get
        {
            return (MenuButtonContext)Context;
        }
    }

    public void OnSelect()
    {
        Item.SelectAction();

        if (MainMenuManager.HasInstance &&
            Item.Dto.switchSeqType != MAINMENU_SEQ.SEQ_NONE)
        {
            MainMenuManager.Instance.AddSwitchRequest(Item.Dto.switchSeqType, false, false);
        }

    }

}
