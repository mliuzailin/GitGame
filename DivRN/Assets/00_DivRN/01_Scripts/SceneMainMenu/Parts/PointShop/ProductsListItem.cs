using UnityEngine;
using System.Collections;

public class ProductsListItem : ListItem<ProductsListItemContex>
{
    void Start()
    {
        SetModel(Context.model);

        // TODO : 演出あるならしかるべき場所に移動
        m_listItemModel.Appear();
        m_listItemModel.SkipAppearing();
    }

    public void OnLongPress()
    {
        m_listItemModel.LongPress();
    }
}
