using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class HomeMenuUnitInfo : ButtonView
{
    public static readonly string PrefabPath = "Prefab/MainMenu/Parts/Home/HomeMenuUnitInfo";

    private ButtonModel m_model = null;


    public static HomeMenuUnitInfo Attach(GameObject parent)
    {
        return ButtonView.Attach<HomeMenuUnitInfo>(PrefabPath, parent);
    }


    public HomeMenuUnitInfo SetModel(ButtonModel model)
    {
        m_model = model;

        base.SetModel<ButtonModel>(m_model);

        return this;
    }

    void Awake()
    {
        AppearAnimationName = "mainmenu_home_side_menu_appear";
        DefaultAnimationName = "mainmenu_home_unit_info_loop";
    }
}
