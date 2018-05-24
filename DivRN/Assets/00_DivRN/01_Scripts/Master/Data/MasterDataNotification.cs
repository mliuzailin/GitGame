using UnityEngine;
using System.Collections;
using SQLite.Attribute;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ：実体：ローカル通知
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataNotification : Master
{
    [Ignore]
    public override uint tag_id
    {
        get;
        set;
    }

    public uint type
    {
        get;
        set;
    } //!< 情報固有固定ID// なし:0 イベント:1  ガチャ:2
      // ServerDataDefine.NOTIFICATION_TYPE

    public uint timing_start
    {
        get;
        set;
    } //!< イベントタイミング：開始

    public string notification_title
    {
        get;
        set;
    } //!< 通知文言：タイトル

    public string notification_body
    {
        get;
        set;
    } //!< 通知文言：本文

    public MasterDataDefineLabel.NotificationType notification_type
    {
        get;
        set;
    } //!< 通知文言：タイプ

    public uint event_schedule_show
    {
        get;
        set;
    } //!< イベントスケジュール表示有無
}

///}
