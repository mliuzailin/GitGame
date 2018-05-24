/**
 *  @file   PartySelectGroupUnitContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/14
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;

public class PartySelectGroupUnitContext : M4uContext
{
    public PartySelectGroupUnitContext(PartySelectGroupUnitListItemModel listItemModel)
    {
        m_model = listItemModel;
    }

    private PartySelectGroupUnitListItemModel m_model;
    public PartySelectGroupUnitListItemModel model { get { return m_model; } }

    /// <summary>アイテムを選択したときのアクション</summary>
    public Action<PartySelectGroupUnitContext> DidSelectItem = delegate { };

    /// <summary>リスト判別用のID</summary>
    public int Index;

    /// <summary>ユニット単体情報配列</summary>
    public PacketStructUnit[] PartyData = new PacketStructUnit[(int)GlobalDefine.PartyCharaIndex.MAX - 1];
    /// <summary>ユニット単体情報配列</summary>
    public PacketStructUnit[] PartyLinkData = new PacketStructUnit[(int)GlobalDefine.PartyCharaIndex.MAX - 1];
    /// <summary>クエスト制限による固定ユニットかどうか</summary>
    public bool[] IsPartyFix = new bool[(int)GlobalDefine.PartyCharaIndex.MAX];

    M4uProperty<bool> isLowerScreen = new M4uProperty<bool>(false);
    /// <summary>低解像度かどうか</summary>
    public bool IsLowerScreen { get { return isLowerScreen.Value; } set { isLowerScreen.Value = value; } }

    M4uProperty<Sprite> unitImage = new M4uProperty<Sprite>();
    /// <summary>ユニット画像</summary>
    public Sprite UnitImage { get { return unitImage.Value; } set { unitImage.Value = value; } }

    M4uProperty<bool> isSelect = new M4uProperty<bool>();
    /// <summary>選択状態かどうか</summary>
    public bool IsSelect { get { return isSelect.Value; } set { m_model.isSelected = isSelect.Value = value; } }

    M4uProperty<string> nameText = new M4uProperty<string>();
    /// <summary>名前</summary>
    public string NameText { get { return nameText.Value; } set { nameText.Value = value; } }

    M4uProperty<Sprite> linkIcon = new M4uProperty<Sprite>();
    /// <summary>リンクアイコン</summary>
    public Sprite LinkIcon
    {
        get { return linkIcon.Value; }
        set
        {
            IsActiveLinkIcon = (value != null);
            linkIcon.Value = value;
        }
    }

    M4uProperty<bool> isActiveLinkIcon = new M4uProperty<bool>(false);
    /// <summary>リンクアイコンの表示・非表示</summary>
    public bool IsActiveLinkIcon { get { return isActiveLinkIcon.Value; } set { isActiveLinkIcon.Value = value; } }

	M4uProperty<Sprite> iconSelect = new M4uProperty<Sprite>();
	public Sprite IconSelect { get { return iconSelect.Value; } set { iconSelect.Value = value; } }


}
