/**
 *  @file   TutorialDialogPointContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/01
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;

public class TutorialDialogPointContext : M4uContext
{
    /// <summary>アイテムを選択したときのアクション</summary>
    public Action<TutorialDialogPointContext> DidSelectItem = delegate { };

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
