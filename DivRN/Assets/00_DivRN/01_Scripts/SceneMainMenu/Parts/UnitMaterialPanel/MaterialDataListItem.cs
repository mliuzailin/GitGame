using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialDataListItem : ListItem<MaterialDataContext>
{
    void Awake()
    {
        DefaultAnimationName = "material_data_item_loop";
        DisappearAnimationName = "material_data_item_disappear";
    }

    void Start()
    {
        SetModel(Context.model);

        // TODO : 演出あるならしかるべき場所に移動
        m_listItemModel.Appear();
        m_listItemModel.SkipAppearing();

    }
}
