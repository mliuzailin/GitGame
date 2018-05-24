using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeMenuPresent : ButtonView
{

    public static readonly string PrefabPath = "Prefab/MainMenu/Parts/Home/HomeMenuPresent";

    private ButtonModel m_model = null;


    public static HomeMenuPresent Attach(GameObject parent)
    {
        return ButtonView.Attach<HomeMenuPresent>(PrefabPath, parent);
    }


    public HomeMenuPresent SetModel(ButtonModel model)
    {
        m_model = model;

        base.SetModel<ButtonModel>(m_model);

        return this;
    }
    void Awake()
    {
        AppearAnimationName = "mainmenu_home_btn_appear";
        DefaultAnimationName = "mainmenu_home_btn_loop";
    }
}
