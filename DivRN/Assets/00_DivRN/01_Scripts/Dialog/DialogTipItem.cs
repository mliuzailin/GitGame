using UnityEngine;
using System;
using System.Collections;
using M4u;


public class DialogTipItem : M4uContext
{

    public DialogTipItem(int _stone_num, int _stone_price, string _event_msg, string _caption_msg, DialogButtonEventType _type)
    {
        InitParam();
        Stone_num = string.Format(GameTextUtil.GetText("sh119q_content3"), _stone_num);
        Stone_price = string.Format(GameTextUtil.GetText("sh119q_content5"), _stone_price);
        Event_msg = _event_msg;
        if (_event_msg != "")
        {
            Event_active = true;
        }
        Caption_msg = _caption_msg;
        if (_caption_msg != "")
        {
            Caption_active = true;
        }
        buttonType = _type;
    }

    public DialogTipItem(StoreProduct _product, DialogButtonEventType _type)
    {
        InitParam();
        product = _product;
        Stone_num = string.Format(GameTextUtil.GetText("sh119q_content3"), _product.product_num);
        Stone_price = _product.product_price_format;

        Event_active = false;
        if (_product.event_text != null && _product.event_text != "")
        {
            // 作成したテキストを連結したものを表示する
            string eventText = string.Format(_product.event_text, GameTextUtil.GetRemainStr(_product.remaining_time, GameTextUtil.GetText("general_time_01")), string.Format("残り{0}回", _product.event_chip_count));
            Event_msg = eventText;
            Event_active = true;
        }
        Caption_active = false;
        if (_product.event_caption != null && _product.event_caption != "")
        {
            Caption_msg = _product.event_caption;
            Caption_active = true;
        }
        buttonType = _type;
    }

    private void InitParam()
    {
        Stone_num_rate = GameTextUtil.GetText("sh119q_content4");
        Stone_price_rate = GameTextUtil.GetText("sh119q_content6");
    }

    M4uProperty<string> stone_num = new M4uProperty<string>();
    public string Stone_num { get { return stone_num.Value; } set { stone_num.Value = value; } }

    M4uProperty<string> stone_num_rate = new M4uProperty<string>();
    /// <summary>チップの単位テキスト</summary>
    public string Stone_num_rate { get { return stone_num_rate.Value; } set { stone_num_rate.Value = value; } }

    M4uProperty<string> stone_price = new M4uProperty<string>();
    public string Stone_price { get { return stone_price.Value; } set { stone_price.Value = value; } }

    M4uProperty<string> stone_price_rate = new M4uProperty<string>();
    /// <summary>金額の単位テキスト</summary>
    public string Stone_price_rate { get { return stone_price_rate.Value; } set { stone_price_rate.Value = value; } }

    M4uProperty<string> event_msg = new M4uProperty<string>();
    public string Event_msg { get { return event_msg.Value; } set { event_msg.Value = value; } }

    M4uProperty<string> caption_msg = new M4uProperty<string>();
    public string Caption_msg { get { return caption_msg.Value; } set { caption_msg.Value = value; } }

    M4uProperty<bool> event_active = new M4uProperty<bool>();
    public bool Event_active { get { return event_active.Value; } set { event_active.Value = value; } }

    M4uProperty<bool> caption_active = new M4uProperty<bool>();
    public bool Caption_active { get { return caption_active.Value; } set { caption_active.Value = value; } }

    public DialogButtonEventType buttonType = DialogButtonEventType.NONE;

    public System.Action<DialogTipItem> DidSelectItem = delegate { };

    public StoreProduct product = null;

}
