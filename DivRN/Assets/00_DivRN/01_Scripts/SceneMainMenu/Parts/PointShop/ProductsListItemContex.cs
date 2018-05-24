using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;


public class ProductsListItemContex : M4uContext
{
    public static readonly string DefaultSpriteName = "divine1";

    // 商品ボタン識別ID
    public int Id { get; set; }

    // データ
    public MasterDataPointShopProduct Product { get; set; }

    // 販売価格
    protected M4uProperty<int> price = new M4uProperty<int>(0);
    public int Price
    {
        get { return price.Value; }
        set
        {
            price.Value = value;
            PriceText = string.Format(GameTextUtil.GetText("pointshop_display1"), string.Format("{0:#,0}", value));
        }
    }

    M4uProperty<string> priceText = new M4uProperty<string>();
    public string PriceText { get { return priceText.Value; } set { priceText.Value = value; } }

    // 商品名称
    protected M4uProperty<string> nameText = new M4uProperty<string>("");
    public string NameText { get { return nameText.Value; } set { nameText.Value = value; } }

    // 商品イメージ
    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<string> balloonMessageText = new M4uProperty<string>("");
    /// <summary>吹き出しメッセージ</summary>
    public string BalloonMessageText { get { return balloonMessageText.Value; } set { balloonMessageText.Value = value; } }

    private ListItemModel m_model = null;
    public ListItemModel model { get { return m_model; } }

    // ボタンフィードバック
    public void OnClickedButton()
    {
        if (Product.product_type == MasterDataDefineLabel.PointShopType.UNIT)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            // カタログ表示
            MainMenuManager.Instance.OpenUnitDetailInfoCatalog((uint)Product.product_param2);
        }
    }

    public ProductsListItemContex(
        int id,
        MasterDataPointShopProduct product,
        ListItemModel model /* TODO : 機能に関するものはListItemModelに移動 */)
    {
        // ボタンID
        Id = id;

        // 販売データ
        Product = product;

        // 商品名
        NameText = Product.product_name;

        // 販売価格 通貨単位はunity上で付与
        Price = Product.price;

        //残り時間
        if (product.timing_end > 0)
        {
            DateTime cTimeEnd = TimeUtil.GetDateTime(product.timing_end);
            TimeSpan cCountDown = cTimeEnd - TimeManager.Instance.m_TimeNow;
            BalloonMessageText = GameTextUtil.GetRemainStr(cCountDown, GameTextUtil.GetText("general_time_02"));
        }


        // アイコン設定
        this.image(sprite => { IconImage = sprite; });

        m_model = model;
    }

    // アイコンのSpriteを返却 MasterDataDefineLabel.PointShopType版
    // いずれ他の物と統合すべき
    private void image(System.Action<Sprite> callback)
    {
        // アイコン設定
        switch (Product.product_type)
        {
            case MasterDataDefineLabel.PointShopType.NONE:
                callback(ResourceManager.Instance.Load(DefaultSpriteName));
                break;

            case MasterDataDefineLabel.PointShopType.MONEY:
                callback(ResourceManager.Instance.Load("coin_icon", ResourceType.Common));
                break;

            case MasterDataDefineLabel.PointShopType.FP:
                callback(ResourceManager.Instance.Load("friend_point_icon", ResourceType.Common));
                break;

            case MasterDataDefineLabel.PointShopType.UNIT:
            case MasterDataDefineLabel.PointShopType.UNIT_EVOL:
            case MasterDataDefineLabel.PointShopType.UNIT_BUILDUP:
            case MasterDataDefineLabel.PointShopType.UNIT_LINK:
                UnitIconImageProvider.Instance.Get(
                    (uint)Product.product_param2,
                    sprite =>
                    {
                        callback(sprite);
                    });
                break;

            case MasterDataDefineLabel.PointShopType.TICKET:
                callback(ResourceManager.Instance.Load("casino_ticket", ResourceType.Common));
                break;

            case MasterDataDefineLabel.PointShopType.SCRATCH:
                callback(ResourceManager.Instance.Load(DefaultSpriteName));
                break;

            case MasterDataDefineLabel.PointShopType.ITEM:
                {
                    MasterDataUtil.GetItemIcon(
                        (uint)Product.product_param1,
                        sprite =>
                        {
                            callback(sprite);
                        });
                }
                break;

            case MasterDataDefineLabel.PointShopType.LIMITOVER:
                callback(ResourceManager.Instance.Load("mm_limit_over"));
                break;

            case MasterDataDefineLabel.PointShopType.QUEST_KEY:
                callback(ResourceManager.Instance.Load("mm_quest_key"));
                break;

            default:
                callback(ResourceManager.Instance.Load(DefaultSpriteName));
                break;
        }
    }
}
