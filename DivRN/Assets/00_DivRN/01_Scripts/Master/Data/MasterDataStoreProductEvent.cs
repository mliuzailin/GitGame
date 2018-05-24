using UnityEngine;
using System.Collections;
using System;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ストア関連：イベント商品情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataStoreProductEvent : Master
{
    public MasterDataDefineLabel.BoolType active { get; set; }                  //!<
    public uint timing_start { get; set; }           //!< 期間開始
    public uint timing_st_m { get; set; }            //!< 期間開始 分
    public uint timing_end { get; set; }             //!< 期間終了
    public uint timing_ed_m { get; set; }            //!< 期間終了 分

    public string item_before { get; set; }          //!< 置き換え前アイテム
    public string item_after { get; set; }               //!< 置き換え後アイテム
    public uint event_id { get; set; }               //!< イベントID

    public int add_chip { get; set; }                //!< 初回加算枚数
    public int event_chip_count { get; set; }        //!< イベントでのチップ購入回数
    public MasterDataDefineLabel.StoreType platform { get; set; }                //!< 商品販売プラットフォーム [ 0:iOS ][ 1:Android ][ 2:ALL ]	 (MasterDataDefineLabel.STORE_IOS～)
    public int count_draw { get; set; }              //!< 残り時間表示有無											 (MasterDataDefineLabel.BOOL_NONE～)
    public int buy_count_draw { get; set; }          //!< 残り購入回数表示有無										 (MasterDataDefineLabel.BOOL_NONE～)

    public MasterDataDefineLabel.EventType kind { get; set; }                   //!< イベントタイプ [ 0:通常イベント ][ 1:初回購入イベント ][ 2:プレゼントつきイベント ]
    public string event_text { get; set; }               //!< イベントテキスト(下)

    // V320プレゼントつきイベント対応
    public uint present_group_id { get; set; }       //!< 購入時付与プレゼント ※MasterDataPresenGroup.group_idと一致　※[ 0 ～ 999,999,999 ]
    public string present_message { get; set; }      //!< 購入時付与プレゼントメッセージ ※プレゼントが付与される場合に表示される文言

    public string event_caption_text { get; set; }       //!< イベントテキスト(上)

    public override string ToString()
    {
        return string.Format(
            "fix_id ={0}  active={1}  timing_start={2} timing_end={3} item_before={4} item_after={5} event_id={6} add_chip={7} platform={8} count_draw={9} kind={10} event_text={11}"
            , fix_id
            , active
            , timing_start
            , timing_end

            , item_before
            , item_after
            , event_id

            , add_chip
            , platform
            , count_draw

            , kind
            , event_text
        );
    }

};

