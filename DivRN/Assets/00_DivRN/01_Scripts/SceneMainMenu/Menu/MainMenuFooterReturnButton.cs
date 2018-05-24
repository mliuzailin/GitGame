using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuFooterReturnButton : ButtonView
{

    public static readonly string PrefabPath = "Prefab/MainMenu/MainMenuFooterReturnButton";

    private ButtonModel m_model = null;


    public static MainMenuFooterReturnButton Attach(GameObject parent)
    {
        return ButtonView.Attach<MainMenuFooterReturnButton>(PrefabPath, parent);
    }


    public MainMenuFooterReturnButton SetModel(ButtonModel model)
    {
        m_model = model;

        base.SetModel<ButtonModel>(m_model);

        return this;
    }

    void Awake()
    {
    }
}
