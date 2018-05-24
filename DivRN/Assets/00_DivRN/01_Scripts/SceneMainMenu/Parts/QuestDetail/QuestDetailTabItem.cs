using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDetailTabItem : ListItem<QuestDetailTabContext>
{
    private static readonly string SelectAnimationName = "quest_detail_tab_list_item_select";

    void Awake()
    {
        AppearAnimationName = "quest_detail_tab_list_item_appear";
        DefaultAnimationName = "quest_detail_tab_list_item_loop";
    }

    void Start()
    {
        var model = Context.model;

        SetModel(model);

        model.OnSelected += () =>
        {
            PlayAnimation(SelectAnimationName, () =>
            {
                PlayAnimation(DefaultAnimationName);
            });
        };

        RegisterKeyEventCallback("next", () =>
        {
            model.ShowNext();
        });
    }

    public void OnSelectTab()
    {
        Context.DidSelectTab(Context);
    }
}
