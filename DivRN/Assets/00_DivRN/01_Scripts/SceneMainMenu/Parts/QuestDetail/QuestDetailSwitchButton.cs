using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDetailSwitchButton : ButtonView
{
    private static readonly string SwitchButtonPrefabPath = "Prefab/QuestDetail/QuestDetailSwitchButton";

    public static QuestDetailSwitchButton Attach(GameObject parent)
    {
        return ButtonView.Attach<QuestDetailSwitchButton>(SwitchButtonPrefabPath, parent);
    }


    void Awake()
    {
        AppearAnimationName = "quest_detail_switch_button_appear";
        DefaultAnimationName = "quest_detail_switch_button_loop";
    }
}
