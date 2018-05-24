using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M4u;

// 2017/5/8 今後のアップデートで追加予らしい 
// 
public class HomeMenuBossInfo : ButtonView
{
    public static readonly string PrefabPath = "Prefab/MainMenu/Parts/Home/HomeMenuBossInfo";

    private ButtonModel m_model = null;


    public static HomeMenuBossInfo Attach(GameObject parent)
    {
        return ButtonView.Attach<HomeMenuBossInfo>(PrefabPath, parent);
    }


    public HomeMenuBossInfo SetModel(ButtonModel model)
    {
        m_model = model;

        base.SetModel<ButtonModel>(m_model);

        return this;
    }

    void Awake()
    {
        AppearAnimationName = "mainmenu_home_side_menu_appear";
        DefaultAnimationName = "mainmenu_home_boss_info_loop";
    }
}
