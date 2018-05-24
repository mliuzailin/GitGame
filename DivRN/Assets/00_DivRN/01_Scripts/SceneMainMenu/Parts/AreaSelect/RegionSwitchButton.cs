using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionSwitchButton : ButtonView
{
    public static readonly string PrefabPath = "Prefab/AreaSelect/RegionSwitchButton";

    private ButtonModel m_model = null;


    public static RegionSwitchButton Attach(GameObject parent)
    {
        return ButtonView.Attach<RegionSwitchButton>(PrefabPath, parent);
    }


    public RegionSwitchButton SetModel(ButtonModel model)
    {
        m_model = model;

        base.SetModel<ButtonModel>(m_model);

        m_model.SkipAppearing();

        return this;
    }
}
