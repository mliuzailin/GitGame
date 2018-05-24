/**
 *  @file   PartyMemberUnitContext.cs
 *  @brief
 *  @author Developer
 *  @date   2017/02/14
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;
using ServerDataDefine;

public class PartyMemberUnitContext : M4uContext
{
    public PartyMemberUnitContext(PartyMemberUnitListItemModel listItemModel)
    {
        m_model = listItemModel;
    }

    private PartyMemberUnitListItemModel m_model;
    public PartyMemberUnitListItemModel model { get { return m_model; } }


    /// <summary>アイテムを選択したときのアクション</summary>
    public Action<PartyMemberUnitContext> DidSelectItem = delegate { };
    /// <summary>アイテムを選択したときのアクション</summary>
    public Action<PartyMemberUnitContext> DidLongPressItem = delegate { };


    /// <summary>ユニット単体情報</summary>
    public PacketStructUnit UnitData = new PacketStructUnit();
    public MasterDataParamChara CharaMaster = null;
    public MasterDataParamChara LinkCharaMaster = null;

    /// <summary>リンクユニット単体情報</summary>
    public PacketStructUnit LinkUnitData = new PacketStructUnit();

    M4uProperty<GlobalDefine.PartyCharaIndex> partyCharaIndex = new M4uProperty<GlobalDefine.PartyCharaIndex>();
    /// <summary>パーティキャラタイプ</summary>
    public GlobalDefine.PartyCharaIndex PartyCharaIndex
    {
        get
        {
            return partyCharaIndex.Value;
        }
        set
        {
            partyCharaIndex.Value = value;
        }
    }

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

    M4uProperty<Sprite> unitImage = new M4uProperty<Sprite>();
    /// <summary>ユニット画像</summary>
    public Sprite UnitImage { get { return unitImage.Value; } set { unitImage.Value = value; } }

    M4uProperty<Sprite> linkUnitImage = new M4uProperty<Sprite>();
    /// <summary>リンクユニット画像</summary>
    public Sprite LinkUnitImage { get { return linkUnitImage.Value; } set { linkUnitImage.Value = value; } }

    M4uProperty<Sprite> outSideCircleImage = new M4uProperty<Sprite>();
    /// <summary>外側のサークルの画像</summary>
    public Sprite OutSideCircleImage { get { return outSideCircleImage.Value; } set { outSideCircleImage.Value = value; } }

    M4uProperty<Sprite> linkOutSideCircleImage = new M4uProperty<Sprite>();
    /// <summary>リンクユニットの外側のサークルの画像</summary>
    public Sprite LinkOutSideCircleImage { get { return linkOutSideCircleImage.Value; } set { linkOutSideCircleImage.Value = value; } }

    M4uProperty<bool> isEmptyLinkUnit = new M4uProperty<bool>(true);
    public bool IsEmptyLinkUnit { get { return isEmptyLinkUnit.Value; } set { isEmptyLinkUnit.Value = value; } }

    M4uProperty<bool> isEnalbeSelect = new M4uProperty<bool>(true);
    /// <summary>選択できるかどうか</summary>
    public bool IsEnalbeSelect
    {
        get { return isEnalbeSelect.Value; }
        set
        {
            isEnalbeSelect.Value = value;
            m_model.isEnabled = value;
        }
    }

    M4uProperty<bool> isSelect = new M4uProperty<bool>();
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

    M4uProperty<string> paramText = new M4uProperty<string>();
    /// <summary>ユニットアイコンの下部テキスト</summary>
    public string ParamText
    {
        get
        {
            return paramText.Value;
        }
        set
        {
            paramText.Value = value;
        }
    }

    M4uProperty<bool> isActiveParamText = new M4uProperty<bool>(false);
    public bool IsActiveParamText
    {
        get
        {
            return isActiveParamText.Value;
        }
        set
        {
            isActiveParamText.Value = value;
        }
    }

    M4uProperty<string> hpText = new M4uProperty<string>("");
    /// <summary>HPのテキスト</summary>
    public string HpText
    {
        get
        {
            return hpText.Value;
        }
        set
        {
            hpText.Value = value;
        }
    }

    M4uProperty<string> atkText = new M4uProperty<string>("");
    /// <summary>攻撃力のテキスト</summary>
    public string AtkText
    {
        get
        {
            return atkText.Value;
        }
        set
        {
            atkText.Value = value;
        }
    }

    M4uProperty<bool> isActiveStatus = new M4uProperty<bool>(false);
    /// <summary>ステータスの表示・非表示</summary>
    public bool IsActiveStatus
    {
        get
        {
            return isActiveStatus.Value;
        }
        set
        {
            isActiveStatus.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill1Cost1 = new M4uProperty<bool>();
    private bool IsActiveSkill1Cost1
    {
        get
        {
            return isActiveSkill1Cost1.Value;
        }
        set
        {
            isActiveSkill1Cost1.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill1Cost2 = new M4uProperty<bool>();
    private bool IsActiveSkill1Cost2
    {
        get
        {
            return isActiveSkill1Cost2.Value;
        }
        set
        {
            isActiveSkill1Cost2.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill1Cost3 = new M4uProperty<bool>();
    private bool IsActiveSkill1Cost3
    {
        get
        {
            return isActiveSkill1Cost3.Value;
        }
        set
        {
            isActiveSkill1Cost3.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill1Cost4 = new M4uProperty<bool>();
    private bool IsActiveSkill1Cost4
    {
        get
        {
            return isActiveSkill1Cost4.Value;
        }
        set
        {
            isActiveSkill1Cost4.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill1Cost5 = new M4uProperty<bool>();
    private bool IsActiveSkill1Cost5
    {
        get
        {
            return isActiveSkill1Cost5.Value;
        }
        set
        {
            isActiveSkill1Cost5.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill2Cost1 = new M4uProperty<bool>();
    private bool IsActiveSkill2Cost1
    {
        get
        {
            return isActiveSkill2Cost1.Value;
        }
        set
        {
            isActiveSkill2Cost1.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill2Cost2 = new M4uProperty<bool>();
    private bool IsActiveSkill2Cost2
    {
        get
        {
            return isActiveSkill2Cost2.Value;
        }
        set
        {
            isActiveSkill2Cost2.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill2Cost3 = new M4uProperty<bool>();
    private bool IsActiveSkill2Cost3
    {
        get
        {
            return isActiveSkill2Cost3.Value;
        }
        set
        {
            isActiveSkill2Cost3.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill2Cost4 = new M4uProperty<bool>();
    private bool IsActiveSkill2Cost4
    {
        get
        {
            return isActiveSkill2Cost4.Value;
        }
        set
        {
            isActiveSkill2Cost4.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill2Cost5 = new M4uProperty<bool>();
    private bool IsActiveSkill2Cost5
    {
        get
        {
            return isActiveSkill2Cost5.Value;
        }
        set
        {
            isActiveSkill2Cost5.Value = value;
        }
    }

    M4uProperty<Sprite> skill1Cost1 = new M4uProperty<Sprite>();
    public Sprite Skill1Cost1
    {
        get
        {
            return skill1Cost1.Value;
        }
        set
        {
            skill1Cost1.Value = value;
            IsActiveSkill1Cost1 = (value != null);
        }
    }

    M4uProperty<Sprite> skill1Cost2 = new M4uProperty<Sprite>();
    public Sprite Skill1Cost2
    {
        get
        {
            return skill1Cost2.Value;
        }
        set
        {
            skill1Cost2.Value = value;
            IsActiveSkill1Cost2 = (value != null);
        }
    }

    M4uProperty<Sprite> skill1Cost3 = new M4uProperty<Sprite>();
    public Sprite Skill1Cost3
    {
        get
        {
            return skill1Cost3.Value;
        }
        set
        {
            skill1Cost3.Value = value;
            IsActiveSkill1Cost3 = (value != null);
        }
    }

    M4uProperty<Sprite> skill1Cost4 = new M4uProperty<Sprite>();
    public Sprite Skill1Cost4
    {
        get
        {
            return skill1Cost4.Value;
        }
        set
        {
            skill1Cost4.Value = value;
            IsActiveSkill1Cost4 = (value != null);
        }
    }

    M4uProperty<Sprite> skill1Cost5 = new M4uProperty<Sprite>();
    public Sprite Skill1Cost5
    {
        get
        {
            return skill1Cost5.Value;
        }
        set
        {
            skill1Cost5.Value = value;
            IsActiveSkill1Cost5 = (value != null);
        }
    }

    M4uProperty<Sprite> skill2Cost1 = new M4uProperty<Sprite>();
    public Sprite Skill2Cost1
    {
        get
        {
            return skill2Cost1.Value;
        }
        set
        {
            skill2Cost1.Value = value;
            IsActiveSkill2Cost1 = (value != null);
        }
    }

    M4uProperty<Sprite> skill2Cost2 = new M4uProperty<Sprite>();
    public Sprite Skill2Cost2
    {
        get
        {
            return skill2Cost2.Value;
        }
        set
        {
            skill2Cost2.Value = value;
            IsActiveSkill2Cost2 = (value != null);
        }
    }

    M4uProperty<Sprite> skill2Cost3 = new M4uProperty<Sprite>();
    public Sprite Skill2Cost3
    {
        get
        {
            return skill2Cost3.Value;
        }
        set
        {
            skill2Cost3.Value = value;
            IsActiveSkill2Cost3 = (value != null);
        }
    }

    M4uProperty<Sprite> skill2Cost4 = new M4uProperty<Sprite>();
    public Sprite Skill2Cost4
    {
        get
        {
            return skill2Cost4.Value;
        }
        set
        {
            skill2Cost4.Value = value;
            IsActiveSkill2Cost4 = (value != null);
        }
    }

    M4uProperty<Sprite> skill2Cost5 = new M4uProperty<Sprite>();
    public Sprite Skill2Cost5
    {
        get
        {
            return skill2Cost5.Value;
        }
        set
        {
            skill2Cost5.Value = value;
            IsActiveSkill2Cost5 = (value != null);
        }
    }


    M4uProperty<bool> isActiveSkill1Empty = new M4uProperty<bool>(true);
    public bool IsActiveSkill1Empty
    {
        get
        {
            return isActiveSkill1Empty.Value;
        }
        set
        {
            isActiveSkill1Empty.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill2Empty = new M4uProperty<bool>(true);
    public bool IsActiveSkill2Empty
    {
        get
        {
            return isActiveSkill2Empty.Value;
        }
        set
        {
            isActiveSkill2Empty.Value = value;
        }
    }

    M4uProperty<bool> isActiveFixFlag = new M4uProperty<bool>(false);
    /// <summary>固定パーティのフラグ表示・非表示</summary>
    public bool IsActiveFixFlag { get { return isActiveFixFlag.Value; } set { isActiveFixFlag.Value = value; } }

    M4uProperty<Sprite> iconSelect = new M4uProperty<Sprite>();
    public Sprite IconSelect { get { return iconSelect.Value; } set { iconSelect.Value = value; } }

    M4uProperty<bool> isElement = new M4uProperty<bool>(true);
    /// <summary>選択できるかどうか</summary>
    public bool IsElement
    {
        get { return isElement.Value; }
        set
        {
            isElement.Value = value;
        }
    }

    M4uProperty<Sprite> skill1Color = new M4uProperty<Sprite>();
    public Sprite Skill1Color
    {
        get
        {
            return skill1Color.Value;
        }
        set
        {
            skill1Color.Value = value;
        }
    }

    M4uProperty<Sprite> skill2Color = new M4uProperty<Sprite>();
    public Sprite Skill2Color
    {
        get
        {
            return skill2Color.Value;
        }
        set
        {
            skill2Color.Value = value;
        }
    }

}
