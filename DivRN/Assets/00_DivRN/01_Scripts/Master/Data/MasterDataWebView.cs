using UnityEngine;
using System.Collections;
using M4u;
using SQLite.Attribute;

//----------------------------------------------------------------------------
/*!
    @brief  マスターデータ実体：エリア関連：エリア情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataWebView : Master
{
    //public uint fix_id;                 //!< 情報固有固定ID

    public uint timing_public { get; set; }          //!< 一般公開タイミング

    public uint timing_start { get; set; }           //!< 開始タイミング
    public uint timing_end { get; set; }             //!< 終了タイミング

    public string url_web { get; set; }              //!< リンク先URL

    public uint webview_type { get; set; }           //!< webview表示条件
    public uint show_every_time { get; set; }        //!< 繰り返し表示
    public uint webview_param_1 { get; set; }        //!< webview表示処理用の汎用パラメータ ※表示判定用
    public uint webview_param_2 { get; set; }        //!< webview表示処理用の汎用パラメータ ※表示判定用
    public uint webview_param_3 { get; set; }        //!< webview表示処理用の汎用パラメータ ※表示判定用
    public uint webview_param_4 { get; set; }        //!< webview表示処理用の汎用パラメータ ※表示判定用
    public uint webview_param_5 { get; set; }        //!< webview表示処理用の汎用パラメータ ※表示判定用
    public uint webview_param_6 { get; set; }        //!< webview表示処理用の汎用パラメータ ※表示判定用
    public uint webview_param_7 { get; set; }        //!< webview表示処理用の汎用パラメータ ※表示判定用
    public uint webview_param_8 { get; set; }        //!< webview表示処理用の汎用パラメータ ※表示判定用
    public uint webview_param_9 { get; set; }        //!< webview表示処理用の汎用パラメータ ※表示判定用
    public uint webview_param_10 { get; set; }       //!< webview表示処理用の汎用パラメータ ※表示判定用
}
