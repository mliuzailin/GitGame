using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class HomeMenuCharacterChange : ButtonView
{
    public static readonly string PrefabPath = "Prefab/MainMenu/Parts/Home/HomeMenuCharacterChange";

    private ButtonModel m_model = null;


    public static HomeMenuCharacterChange Attach(GameObject parent)
    {
        return ButtonView.Attach<HomeMenuCharacterChange>(PrefabPath, parent);
    }


    public HomeMenuCharacterChange SetModel(ButtonModel model)
    {
        m_model = model;

        base.SetModel<ButtonModel>(m_model);

        return this;
    }

    void Awake()
    {
        AppearAnimationName = "mainmenu_home_side_menu_appear";
        DefaultAnimationName = "mainmenu_home_character_change_loop";
    }
}
