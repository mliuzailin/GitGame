/**
 *  @file   DebugReplacePartyUnitListItemContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/13
 */

using UnityEngine;
using System.Collections;
using ServerDataDefine;
using M4u;

public class DebugReplacePartyUnitListItemContext : M4uContext
{

    M4uProperty<string> baseUnitName = new M4uProperty<string>("????");
    /// <summary>
    /// ベースユニット名
    /// </summary>
    public string BaseUnitName
    {
        get
        {
            return baseUnitName.Value;
        }
        set
        {
            baseUnitName.Value = value;
        }
    }

    M4uProperty<string> linkUnitName = new M4uProperty<string>("????");
    /// <summary>
    /// リンクユニット名
    /// </summary>
    public string LinkUnitName
    {
        get
        {
            return linkUnitName.Value;
        }
        set
        {
            linkUnitName.Value = value;
        }
    }

    M4uProperty<Sprite> baseUnitIcon = new M4uProperty<Sprite>();
    /// <summary>
    /// ベースユニットのアイコン
    /// </summary>
    public Sprite BaseUnitIcon
    {
        get
        {
            return baseUnitIcon.Value;
        }
        set
        {
            baseUnitIcon.Value = value;
        }
    }

    M4uProperty<Sprite> linkUnitIcon = new M4uProperty<Sprite>();
    /// <summary>
    /// リンクユニットのアイコン
    /// </summary>
    public Sprite LinkUnitIcon
    {
        get
        {
            return linkUnitIcon.Value;
        }
        set
        {
            linkUnitIcon.Value = value;
        }
    }

    /// <summary>ベースユニットデータ</summary>
    public PacketStructUnit BaseUnitData = new PacketStructUnit();

    /// <summary>リンクユニットデータ</summary>
    public PacketStructUnit LinkUnitData = new PacketStructUnit();

    public DebugReplacePartyUnitListItemContext()
    {
        UnitIconImageProvider.Instance.GetEmpty(sprite =>
        {
            linkUnitIcon.Value = baseUnitIcon.Value = sprite;
        });
    }
}
