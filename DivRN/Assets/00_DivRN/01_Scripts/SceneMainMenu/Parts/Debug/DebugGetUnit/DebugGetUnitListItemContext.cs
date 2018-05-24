/**
 *  @file   DebugGetUnitListItemContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/14
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;
using ServerDataDefine;

public class DebugGetUnitListItemContext : M4uContext
{
    public Action<DebugGetUnitListItemContext> ClickAction = delegate { };
    public Action<DebugGetUnitListItemContext> LongPressAction = delegate { };

    /// <summary>デバッグ用のユニット情報</summary>
    public PacketStructUnitGetDebug UnitGetData = new PacketStructUnitGetDebug();

    public uint unitID;

    M4uProperty<string> unitName = new M4uProperty<string>("????");
    /// <summary>
    /// ユニットの名前
    /// </summary>
    public string UnitName
    {
        get
        {
            return unitName.Value;
        }
        set
        {
            unitName.Value = value;
        }
    }

    M4uProperty<Sprite> unitImage = new M4uProperty<Sprite>();
    /// <summary>
    /// ユニットのアイコン
    /// </summary>
    public Sprite UnitImage
    {
        get
        {
            return unitImage.Value;
        }
        set
        {
            unitImage.Value = value;
        }
    }

    public DebugGetUnitListItemContext()
    {
        UnitIconImageProvider.Instance.GetEmpty(sprite =>
        {
            unitImage.Value = sprite;
        });
    }
}
