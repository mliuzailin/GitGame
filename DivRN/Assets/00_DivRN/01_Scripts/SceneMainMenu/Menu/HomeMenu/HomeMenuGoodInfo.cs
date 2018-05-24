using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class HomeMenuGoodInfo : ButtonView
{
    public static readonly string PrefabPath = "Prefab/MainMenu/Parts/Home/HomeMenuGoodInfo";

    private ButtonModel m_model = null;


    public static HomeMenuGoodInfo Attach(GameObject parent)
    {
        return ButtonView.Attach<HomeMenuGoodInfo>(PrefabPath, parent);
    }


    public HomeMenuGoodInfo SetModel(ButtonModel model)
    {
        m_model = model;

        base.SetModel<ButtonModel>(m_model);

        return this;
    }

    void Awake()
    {
        AppearAnimationName = "mainmenu_home_side_menu_appear";
        DefaultAnimationName = "mainmenu_home_good_info_loop";
    }
}
