using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubTabListItem : ListItem<SubTabContext>
{
    void Start()
    {
        SetModel(Context.model);

        // TODO : 演出あるならしかるべき場所に移動
        m_listItemModel.Appear();
        m_listItemModel.SkipAppearing();
    }

    public void OnSelectTab()
    {
        if (MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch())
            return;

        base.Click();
    }
}
