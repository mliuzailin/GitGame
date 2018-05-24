using UnityEngine;
using System.Collections;

public class UnitSaleListItem : ListItem<UnitSaleContext>
{

    public UnitSaleContext Unit
    {
        get
        {
            return Context;
        }
    }
    /// <summary>
    /// 選択されたとき
    /// </summary>
    public void OnSelect()
    {
        Unit.DidSelectItem(Unit);
    }
}
