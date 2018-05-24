using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeMenuScoreInfo : ButtonView
{

    public static readonly string PrefabPath = "Prefab/MainMenu/Parts/Home/HomeMenuScoreInfo";

    private ButtonModel m_model = null;


    public static HomeMenuScoreInfo Attach(GameObject parent)
    {
        return ButtonView.Attach<HomeMenuScoreInfo>(PrefabPath, parent);
    }


    public HomeMenuScoreInfo SetModel(ButtonModel model)
    {
        m_model = model;

        base.SetModel<ButtonModel>(m_model);

        return this;
    }
    void Awake()
    {
        AppearAnimationName = "mainmenu_home_tab_appear";
        DefaultAnimationName = "mainmenu_home_tab_loop";
    }
}
