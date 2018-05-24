using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：期間限定イベント
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataEventBase : Master
{
    public MasterDataDefineLabel.BoolType active { get; set; }              //!< データ使用フラグ	※このデータは一部領域を使いまわす。Excelの行ごとに無効化してデータを作るのでフラグを持つ

    public uint period_type { get; set; }        //!< 期間指定タイプ
    public uint cycle_date_type { get; set; }    //!< サイクル：開催日指定
    public uint cycle_timing_start { get; set; } //!< サイクル：イベント開始時間
    public uint cycle_active_hour { get; set; }  //!< サイクル：イベント有効時間

    public uint timing_start { get; set; }       //!< イベントタイミング：開始
    public uint timing_end { get; set; }         //!< イベントタイミング：終了
    public uint receiving_end { get; set; }      //!< 獲得できる期間

    public int user_group { get; set; }          //!< ユーザーグループ [ BELONG_NONE < user_group < BELONG_MAX ]
    public uint event_id { get; set; }           //!< イベントラベル		※出力して無いけどイベント固有ID

    public string notification_title { get; set; }   //!< 通知文言：タイトル
    public string notification_body { get; set; }    //!< 通知文言：本文
    public MasterDataDefineLabel.NotificationType notification_type { get; set; }  //!< 通知文言：タイプ

    public MasterDataDefineLabel.BoolType event_schedule_show { get; set; }    //!< イベントスケジュール表示有無

    public uint timing_public { get; set; }                  //!< 一般公開タイミング
};
