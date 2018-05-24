using UnityEngine;
using System;
using System.Collections;
using M4u;

public class UnitSaleContext : M4uContext
{
    public long unique_unit_id;

    M4uProperty<Sprite> unitImage = new M4uProperty<Sprite>();
    public Sprite UnitImage { get { return unitImage.Value; } set { unitImage.Value = value; } }

    /// <summary>
    /// アイテムを選択したときのアクション
    /// </summary>
    public Action<UnitSaleContext> DidSelectItem = delegate { };
}
