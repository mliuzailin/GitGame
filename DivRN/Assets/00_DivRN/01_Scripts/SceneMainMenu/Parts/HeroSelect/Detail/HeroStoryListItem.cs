/**
 *  @file   HeroStoryListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/20
 */

using UnityEngine;
using System.Collections;

public class HeroStoryListItem : ListItem<HeroStoryListItemContext>
{
    void Start()
    {
        SetModel(Context.model);

        // TODO : 演出あるならしかるべき場所に移動
        m_listItemModel.Appear();
        m_listItemModel.SkipAppearing();
    }
}
