using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FooterSubMenuListItem : ListItem<FooterSubMenuItem>
{
    void Awake()
    {
        AppearAnimationName = "footer_sub_button_appear";
        DefaultAnimationName = "footer_sub_button_loop";
        DisappearAnimationName = "footer_sub_button_disappear";
    }

    void Start()
    {
        SetModel(Context.model);
        m_listItemModel.Start();
    }

    public void OnSelect()
    {
        Context.DelSelectSubMenu(Context);
    }

    public void OnClick()
    {
        if (MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch())
            return;

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }
}
