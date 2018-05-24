using UnityEngine;
using System;
using System.Collections;
using M4u;

public class DropIconContex : M4uContext
{

    private DropIconListItemModel m_model = null;
    public DropIconListItemModel model { get { return m_model; } }


    public Action<DropIconContex> DidSelectItem = delegate { };

    // アイテムデザイン
    M4uProperty<Sprite> itemImage = new M4uProperty<Sprite>();
    public Sprite ItemImage { get { return itemImage.Value; } set { itemImage.Value = value; } }

    //レア度イメージ
    M4uProperty<Sprite> rareImage = new M4uProperty<Sprite>();
    public Sprite RareImage { get { return rareImage.Value; } set { rareImage.Value = value; } }

    //
    M4uProperty<bool> isViewRareImage = new M4uProperty<bool>();
    public bool IsViewRareImage { get { return isViewRareImage.Value; } set { isViewRareImage.Value = value; } }

    M4uProperty<bool> isViewCaption = new M4uProperty<bool>();
    public bool IsViewCaption { get { return isViewCaption.Value; } set { isViewCaption.Value = value; } }

    // Caption
    M4uProperty<string> captionText = new M4uProperty<string>();
    public string CaptionText { get { return captionText.Value; } set { captionText.Value = value; } }

    // Count
    M4uProperty<string> countText = new M4uProperty<string>();
    public string CountText { get { return countText.Value; } set { countText.Value = value; } }

    //
    M4uProperty<bool> isViewFlag = new M4uProperty<bool>();
    public bool IsViewFlag { get { return isViewFlag.Value; } set { isViewFlag.Value = value; } }

    M4uProperty<bool> isSelectEnable = new M4uProperty<bool>(true);
    /// <summary>ボタンの選択ができるかどうか</summary>
    public bool IsSelectEnable
    {
        get { return isSelectEnable.Value; }
        set
        {
            isSelectEnable.Value = value;
            //m_model.isEnabled = value;
        }
    }

    public MasterDataParamChara m_Master = null;

    public AnimationClipGettingUnit m_GettingUnit = null;

    public bool m_bReady = false;

    public DropIconContex(MasterDataParamChara _master, bool bPlus, bool bNew, DropIconListItemModel listItemModel)
    {
        m_model = listItemModel;
        m_Master = _master;

        if (_master != null)
        {
            string _name = "";
            switch (_master.rare)
            {
                case MasterDataDefineLabel.RarityType.STAR_1: _name = "chara_icon_rare01"; break;
                case MasterDataDefineLabel.RarityType.STAR_2: _name = "chara_icon_rare02"; break;
                case MasterDataDefineLabel.RarityType.STAR_3: _name = "chara_icon_rare03"; break;
                case MasterDataDefineLabel.RarityType.STAR_4: _name = "chara_icon_rare04"; break;
                case MasterDataDefineLabel.RarityType.STAR_5: _name = "chara_icon_rare05"; break;
                case MasterDataDefineLabel.RarityType.STAR_6: _name = "chara_icon_rare06"; break;
                case MasterDataDefineLabel.RarityType.STAR_7: _name = "chara_icon_rare06"; break;
            }
            RareImage = ResourceManager.Instance.Load(_name);

            UnitIconImageProvider.Instance.Get(
                _master.fix_id,
                sprite =>
                {
                    ItemImage = sprite;
                });

            IsViewRareImage = true;
        }
        IsViewCaption = false;
        IsViewFlag = false;
        CaptionText = "";
        CountText = "";
    }
    public DropIconContex(Sprite _item, Sprite _rare, DropIconListItemModel listItemModel)
    {
        m_model = listItemModel;

        ItemImage = _item;
        RareImage = _rare;
        IsViewRareImage = true;
        CaptionText = "";
    }

    public DropIconContex( MasterDataUseItem itemMaster, Sprite _rare, DropIconListItemModel listItemModel)
    {
        m_model = listItemModel;

        MasterDataUtil.GetItemIcon(
            itemMaster,
            sprite =>
            {
                ItemImage = sprite;
            });
        RareImage = _rare;
        IsViewRareImage = true;
        CaptionText = "";
    }

    // 賞品アイコンの表示
    private Sprite spriteIcon(string spriteName, string atlasName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(atlasName);
        return System.Array.Find<Sprite>(sprites, (sprite) => sprite.name.Equals(spriteName));
    }

    /// <summary>
    /// 条件が合う場合はNewフラグを表示
    /// </summary>
    public void CheckViewFlag()
    {
        if (IsViewFlag == true) { return; }
        if (m_Master == null) { return; }

        if (ServerDataUtil.ChkBitFlag(ref MainMenuParam.m_ResultPrevUnitGetFlag, m_Master.fix_id) == true &&
            !TutorialManager.IsExists)
        {
            return;
        }
        IsViewFlag = true;
    }

}
