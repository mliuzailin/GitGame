using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeMenuMission : ButtonView
{

    public static readonly string PrefabPath = "Prefab/MainMenu/Parts/Home/HomeMenuMission";

    private ButtonModel m_model = null;


    public static HomeMenuMission Attach(GameObject parent)
    {
        return ButtonView.Attach<HomeMenuMission>(PrefabPath, parent);
    }


    public HomeMenuMission SetModel(ButtonModel model)
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
