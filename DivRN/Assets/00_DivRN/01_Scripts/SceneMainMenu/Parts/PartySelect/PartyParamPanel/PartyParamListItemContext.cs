/**
 *  @file   PartyParamListItemContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/13
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using System;

public class PartyParamListItemContext : M4uContext
{
    private PartyParamListItemModel m_model = null;
    public PartyParamListItemContext(PartyParamListItemModel model)
    {
        m_model = model;
    }
    public PartyParamListItemModel model { get { return m_model; } }

    public Action SelectLinkAction = delegate { };

    /// <summary>パーティ情報</summary>
    public CharaParty PartyInfo = new CharaParty();

    public Toggle Toggle = null;
    M4uProperty<string> nameText = new M4uProperty<string>();
    /// <summary>パーティ名</summary>
    public string NameText
    {
        get
        {
            return nameText.Value;
        }
        set
        {
            nameText.Value = value;
        }
    }

    M4uProperty<string> hitPointText = new M4uProperty<string>();
    /// <summary>HP</summary>
    public string HitPointText
    {
        get
        {
            return hitPointText.Value;
        }
        set
        {
            hitPointText.Value = value;
        }
    }

    M4uProperty<string> costText = new M4uProperty<string>();
    /// <summary>コスト</summary>
    public string CostText
    {
        get
        {
            return costText.Value;
        }
        set
        {
            costText.Value = value;
        }
    }

    M4uProperty<string> charmText = new M4uProperty<string>();
    /// <summary>チャーム</summary>
    public string CharmText
    {
        get
        {
            return charmText.Value;
        }
        set
        {
            charmText.Value = value;
        }
    }

    M4uProperty<List<UnitSkillAtPartyContext>> skills = new M4uProperty<List<UnitSkillAtPartyContext>>(new List<UnitSkillAtPartyContext>());
    /// <summary>スキルリスト</summary>
    public List<UnitSkillAtPartyContext> Skills
    {
        get
        {
            return skills.Value;
        }
        set
        {
            skills.Value = value;
        }
    }

    M4uProperty<List<PartyMemberUnitContext>> units = new M4uProperty<List<PartyMemberUnitContext>>(new List<PartyMemberUnitContext>());
    /// <summary>ユニットアイコン</summary>
    public List<PartyMemberUnitContext> Units
    {
        get
        {
            return units.Value;
        }
        set
        {
            units.Value = value;
        }
    }

    M4uProperty<Texture2D> heroImage = new M4uProperty<Texture2D>();
    /// <summary>主人公の画像</summary>
    public Texture2D HeroImage
    {
        get
        {
            return heroImage.Value;
        }
        set
        {
            heroImage.Value = value;
            IsActiveHeroImage = (value != null);
        }
    }

    M4uProperty<Texture2D> heroImage_mask = new M4uProperty<Texture2D>();
    /// <summary>主人公の画像</summary>
    public Texture2D HeroImage_mask
    {
        get
        {
            return heroImage_mask.Value;
        }
        set
        {
            heroImage_mask.Value = value;
        }
    }

    M4uProperty<bool> isActiveHeroImage = new M4uProperty<bool>();
    /// <summary>主人公画像の表示・非表示</summary>
    public bool IsActiveHeroImage
    {
        get
        {
            return isActiveHeroImage.Value;
        }
        set
        {
            isActiveHeroImage.Value = value;
        }
    }

    List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> UnitList { get { return unitList; } set { unitList = value; } }

    void OnChangedUnitList()
    {
        // 表示順を逆にする
        for (int i = 0; i < UnitList.Count; ++i)
        {
            UnitList[i].transform.SetAsFirstSibling();
        }
    }

}
