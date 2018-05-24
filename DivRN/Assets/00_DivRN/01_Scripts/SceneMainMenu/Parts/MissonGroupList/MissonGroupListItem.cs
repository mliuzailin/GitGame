/**
 *  @file   MissonGroupListItem.cs
 *  @brief  クエストのミッショングループリストアイテム
 *  @author Developer
 *  @date   2016/11/14
 */

using UnityEngine;
using System.Collections;
using M4u;

public class MissonGroupListItem : ListItem<MissonGroupListItemContext>
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        Context.DidSelectItem(Context);
    }
}
