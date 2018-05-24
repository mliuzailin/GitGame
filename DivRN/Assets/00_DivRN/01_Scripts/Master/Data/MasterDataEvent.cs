﻿using UnityEngine;
using System.Collections;
using SQLite.Attribute;

[System.Serializable]
public class MasterDataEvent : Master
{

    public MasterDataEvent()
    {
        banner_img_url = "";
        head_text = "";
        head_titel_1 = "";
        head_titel_2 = "";
        //notification_body = "";
        //notification_title = "";
        //work_note = "";

    }

    [Ignore]
    public override uint tag_id
    {
        get;
        set;
    }

    public MasterDataDefineLabel.BoolType active
    {
        get;
        set;
    } //!< データ使用フラグ	※このデータは一部領域を使いまわす。Excelの行ごとに無効化してデータを作るのでフラグを持つ

    public MasterDataDefineLabel.PeriodType period_type
    {
        get;
        set;
    } //!< 期間指定タイプ

    public uint cycle_date_type
    {
        get;
        set;
    } //!< サイクル：開催日指定

    public uint cycle_timing_start
    {
        get;
        set;
    } //!< サイクル：イベント開始時間

    public uint cycle_active_hour
    {
        get;
        set;
    } //!< サイクル：イベント有効時間

    public uint timing_start
    {
        get;
        set;
    } //!< イベントタイミング：開始

    public uint timing_end
    {
        get;
        set;
    } //!< イベントタイミング：終了

    public uint receiving_end
    {
        get;
        set;
    } //!< 獲得できる期間

    public MasterDataDefineLabel.BelongType user_group
    {
        get;
        set;
    } //!< ユーザーグループ [ BELONG_NONE < user_group < BELONG_MAX ]

    public uint event_id
    {
        get;
        set;
    } //!< イベントラベル		※出力して無いけどイベント固有ID

    public MasterDataDefineLabel.BoolType event_schedule_show
    {
        get;
        set;
    } //!< イベントスケジュール表示有無

    public int area_id
    {
        get;
        set;
    }

    public string banner_img_url
    {
        get;
        set;
    }

    public string head_text
    {
        get;
        set;
    }

    public string head_titel_1
    {
        get;
        set;
    }

    public string head_titel_2
    {
        get;
        set;
    }

    //public string notification_body{ get; set; }
    //public string notification_title { get; set; }
    //public string notification_type { get; set; }
    //public string work_note { get; set; }
    //public uint timing_public { get; set; }
};
