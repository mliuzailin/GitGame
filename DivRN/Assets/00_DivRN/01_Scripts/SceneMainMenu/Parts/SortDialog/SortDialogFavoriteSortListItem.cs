/**
 *  @file   SortDialogFavoriteSortListItem.cs
 *  @brief  お好みソートアイテム
 *  @author Developer
 *  @date   2017/04/20
 */

using UnityEngine;
using System.Collections;

public class SortDialogFavoriteSortListItem : ListItem<SortDialogFavoriteSortListContext>
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// ソートボタンを押したとき
    /// </summary>
    public void OnClickAscOrderButton()
    {
        Context.IsAscOrder = true;
        if (Context.OnClickOrderAction != null)
        {
            Context.OnClickOrderAction(Context);
        }
    }

    /// <summary>
    /// ソートボタンを押したとき
    /// </summary>
    public void OnClickDescOrderButton()
    {
        Context.IsAscOrder = false;
        if (Context.OnClickOrderAction != null)
        {
            Context.OnClickOrderAction(Context);
        }
    }

    /// <summary>
    /// フィルターテキストボタンを押したとき
    /// </summary>
    public void OnClickFilterButton()
    {
        if (Context.OnClickFilterAction != null)
        {
            Context.OnClickFilterAction(Context);
        }
    }


}
