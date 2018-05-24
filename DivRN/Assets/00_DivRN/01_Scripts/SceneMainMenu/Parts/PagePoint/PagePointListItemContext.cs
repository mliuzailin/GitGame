/**
 *  @file   PagePointListItemContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/21
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;

public class PagePointListItemContext : M4uContext
{
    public PagePointListItemContext(ListItemModel listItemModel)
    {
        m_model = listItemModel;
    }
    private ListItemModel m_model;
    public ListItemModel model { get { return m_model; } }

    M4uProperty<bool> isSelect = new M4uProperty<bool>(false);
    /// <summary>選択状態かどうか</summary>
    public bool IsSelect
    {
        get
        {
            return isSelect.Value;
        }
        set
        {
            isSelect.Value = value;
        }
    }

}
