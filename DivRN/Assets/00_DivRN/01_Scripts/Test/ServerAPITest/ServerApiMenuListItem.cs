using UnityEngine;
using System.Collections;

public class ServerApiMenuListItem : ListItem<ServerApiMenuItem> {

    public ServerApiMenuItem ApiMenuItem {
        get {
            return (ServerApiMenuItem)Context;
        }
    }


	public void OnSelect()
	{
		ApiMenuItem.DelSelectSubMenu(ApiMenuItem.api);
	}
}
