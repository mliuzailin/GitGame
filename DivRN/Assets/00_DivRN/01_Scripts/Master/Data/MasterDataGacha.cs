using UnityEngine;
using System.Collections;
using SQLite.Attribute;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ガチャ関連：ガチャ定義
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataGacha : Master
{
    public MasterDataGacha()
    {
        debug_url_img = "";
        //notification_title = "";
        //notification_body = "";
        //url_salt = "";
        grossing_present_detail = "";
    }

    [Ignore]
    public override uint tag_id
    {
        get;
        set;
    }

    public uint timing_start
    {
        get;
        set;
    } //!< ガチャタイミング：開始

    public uint timing_end
    {
        get;
        set;
    } //!< ガチャタイミング：終了

    public string name
    {
        get;
        set;
    } //!< ガチャ名称

    public string detail
    {
        get;
        set;
    } //!< ガチャ詳細：V350クライアント中使用廃止

    public string detail_2
    {
        get;
        set;
    } //!< ガチャ詳細（統合してガチャトップページに貼る用）：V350クライアント中使用廃止

    public int priority
    {
        get;
        set;
    } //!< 優先度

    public MasterDataDefineLabel.GachaType type
    {
        get;
        set;
    } //!< ガチャタイプ

    public uint price
    {
        get;
        set;
    } //!< 対価

    public uint view_count
    {
        get;
        set;
    } //!< ガチャを引ける回数 0:無制限

    public uint event_id
    {
        get;
        set;
    } //!< 出現条件イベントID	※出力して無いけどイベント固有ID

    public string hint
    {
        get;
        set;
    } //!< ガチャヒント（ガチャアイコンの左上に乗せる）	※1.0.3追加分：V350クライアント中使用廃止

    public string url_img
    {
        get;
        set;
    } //!< ガチャ詳細画像URL

    public string url_web
    {
        get;
        set;
    } //!< 外部リンクURL


    public int priority_show
    {
        get;
        set;
    } //!< 表示優先度

    //public int tab_num{get;set;}         //!< タブ番号
    public MasterDataDefineLabel.GachaTabIndex tab_num
    {
        get;
        set;
    } //!< タブ番号

    public string tab_name
    {
        get;
        set;
    } //!< タブ名称

    public int rainbow_decide
    {
        get;
        set;
    } //!< 虹確定イベント

    public string change_title
    {
        get;
        set;
    } //!< ガチャ表示名称変更

    public int change_odds
    {
        get;
        set;
    } //!< 名称変更確率

    public string caption_subtext
    {
        get;
        set;
    } //!< ヘッダーに出すサブテキスト

    public int not_have_priority
    {
        get;
        set;
    } //!< 未所持ユニット優先排出

    public MasterDataDefineLabel.BoolType box_output
    {
        get;
        set;
    } //!< BOXガチャフラグ

    public MasterDataDefineLabel.BoolType present_enable
    {
        get;
        set;
    } //!< ガチャプレゼント情報：有効化フラグ

    public uint present_group_id
    {
        get;
        set;
    } //!< ガチャプレゼント情報：付与プレゼント

    public string present_message
    {
        get;
        set;
    } //!< ガチャプレゼント情報：付与プレゼントメッセージ

    public int present_count
    {
        get;
        set;
    } //!< ガチャプレゼント情報：付与回数

    public int present_gacha_count
    {
        get;
        set;
    } //!< ガチャプレゼント情報：付与条件：連続引き回数

    public MasterDataDefineLabel.BoolType paid_tip_only
    {
        get;
        set;
    } //!< 有料チップ限定ガチャフラグ

    public MasterDataDefineLabel.PaidTipResetType reset_type
    {
        get;
        set;
    } //!< リセットタイプ

    public int reset_month
    {
        get;
        set;
    } //!< リセット時間(月)

    public int reset_day
    {
        get;
        set;
    } //!< リセット時間(日)

    public int reset_time
    {
        get;
        set;
    } //!< リセット時間(時分)

    public uint reset_week
    {
        get;
        set;
    }
    /// <summary>
    /// 以下、追加分
    /// </summary>

    public uint gacha_group_id
    {
        get;
        set;
    }

    public string debug_url_img
    {
        get;
        set;
    }

    public uint event_point_id
    {
        get;
        set;
    }

    public uint grossing_present_enable
    {
        get;
        set;
    }

    public uint grossing_present_id
    {
        get;
        set;
    }

    public string grossing_present_detail
    {
        get;
        set;
    }


    public uint cost_item_id
    {
        get;
        set;
    }//!< GachaTypeがITEM_POINTの時に紐づくアイテムのID

    public MasterDataDefineLabel.BoolType first_time_free_enable
    {
        get;
        set;
    }//!< 初回無料フラグ
};
