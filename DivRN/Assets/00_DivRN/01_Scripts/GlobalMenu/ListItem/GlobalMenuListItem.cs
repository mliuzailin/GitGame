using UnityEngine;
using System.Collections;

public class GlobalMenuListItem : ListItem<GlobalMenuItem>
{
    private static readonly string DisappearWithSelectedAnimationName = "gloabal_menu_list_item_disappear_with_selected";

    void Awake()
    {
        AppearAnimationName = "gloabal_menu_list_item_appear";
        DefaultAnimationName = "gloabal_menu_list_item_loop";
        ClickAnimationName = "gloabal_menu_list_item_clicked";
        DisappearAnimationName = "gloabal_menu_list_item_disappear";
    }

    void Start()
    {
        var model = Context.model;

        model.OnClosedWithSelected += () =>
        {
            PlayAnimation(DisappearWithSelectedAnimationName, () =>
            {
                model.FinishDisappearingAnimation();
            });
        };

        SetModel(model);
    }


    new public void Click()
    {
        if (!m_listItemModel.isReady
            || ButtonBlocker.Instance.IsActive()
            || MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch())
            return;

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        base.Click();
    }
}
