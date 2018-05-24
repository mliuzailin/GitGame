using UnityEngine;
using System.Collections;
using System;

public class GlobalMenuUIView : View
{
    private static readonly string ChangeToItemUIAnimationName = "global_menu_ui_to_item";
    private static readonly string BackToTopUIAnimationName = "global_menu_ui_to_top";
    private static readonly string DisappearAnimationName = "global_menu_ui_disappear";

    public void ChangeToItem(System.Action callback = null)
    {
        var tag = "GlobalMenuUIViewChangeToItem";
        ButtonBlocker.Instance.Block(tag);
        PlayAnimation(ChangeToItemUIAnimationName, () =>
        {
            if (callback != null)
                callback();

            ButtonBlocker.Instance.Unblock(tag);
        });
    }

    public void BackToTop(System.Action callback = null)
    {
        var tag = "GlobalMenuUIViewBackToTop";
        ButtonBlocker.Instance.Block(tag);
        PlayAnimation(BackToTopUIAnimationName, () =>
        {
            if (callback != null)
                callback();

            ButtonBlocker.Instance.Unblock(tag);
        });
    }

    public void Close(System.Action callback = null)
    {
        var tag = "GlobalMenuUIViewClose";
        ButtonBlocker.Instance.Block(tag);
        PlayAnimation(DisappearAnimationName, () =>
        {
            if (callback != null)
                callback();

            ButtonBlocker.Instance.Unblock(tag);
        });
    }
}
