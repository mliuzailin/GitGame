/**
 *  @file   PagePointListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/21
 */

using UnityEngine;
using System.Collections;

public class PagePointListItem : ListItem<PagePointListItemContext>
{
    void Start()
    {
        SetModel(Context.model);

        // TODO : 演出あるならしかるべき場所に移動
        m_listItemModel.Appear();
        m_listItemModel.SkipAppearing();
    }
}
