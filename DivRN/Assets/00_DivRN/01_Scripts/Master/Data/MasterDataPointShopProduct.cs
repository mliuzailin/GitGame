using UnityEngine;
using System.Collections;
using System;

//----------------------------------------------------------------------------
/*!
	@brief		マスターデータ実体：ポイントショップ商品定義
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataPointShopProduct : Master
{
    public int priority { get; set; }            //!< 表示優先度：上から昇順

    public MasterDataDefineLabel.PointShopType product_type { get; set; }        //!< 商品種別：[ 0 : NONE ][ 1 : お金 ][ 2 : フレンドポイント ][ 3 : ユニット ] [ 4 : チケット ] [ 5 : スクラッチチケット ]
    public string product_name { get; set; }     //!< 商品表示名
    public int product_param1 { get; set; }      //!< 商品パラメータ1
    public int product_param2 { get; set; }      //!< 商品パラメータ2
    public int product_param3 { get; set; }      //!< 商品パラメータ3
    public int product_param4 { get; set; }      //!< 商品パラメータ4
    public int product_param5 { get; set; }      //!< 商品パラメータ5
    public int product_param6 { get; set; }      //!< 商品パラメータ6
    public int product_param7 { get; set; }      //!< 商品パラメータ7

    public int price { get; set; }               //!< 価格

    public MasterDataDefineLabel.BoolType show_limit { get; set; }          //!< 残り時間表示：[ 0 : 表示なし ][ 1 : 表示あり ]
    public uint timing_start { get; set; }       //!< 商品有効期間：開始
    public uint timing_end { get; set; }         //!< イベントタイミング：終了
}

