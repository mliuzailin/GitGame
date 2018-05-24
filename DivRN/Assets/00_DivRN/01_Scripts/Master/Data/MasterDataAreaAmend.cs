using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：期間限定エリア補正
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataAreaAmend : Master
{
    //public uint fix_id;             //!< 情報固有固定ID
    //public int active;              //!< データ使用フラグ	※このデータは一部領域を使いまわす。Excelの行ごとに無効化してデータを作るのでフラグを持つ
    public MasterDataDefineLabel.BoolType active { get; set; }//!< データ使用フラグ	※このデータは一部領域を使いまわす。Excelの行ごとに無効化してデータを作るのでフラグを持つ

    public uint timing_start { get; set; }       //!< イベントタイミング：開始
    public uint timing_end { get; set; }         //!< イベントタイミング：終了

    //	public	int		start_year{get;set;}			//!< イベント開始タイミング：年
    //	public	int		start_month{get;set;}		//!< イベント開始タイミング：月
    //	public	int		start_day{get;set;}			//!< イベント開始タイミング：日
    //	public	int		start_hour{get;set;}			//!< イベント開始タイミング：時
    //	public	int		end_year{get;set;}			//!< イベント終了タイミング：年
    //	public	int		end_month{get;set;}			//!< イベント終了タイミング：月
    //	public	int		end_day{get;set;}			//!< イベント終了タイミング：日
    //	public	int		end_hour{get;set;}			//!< イベント終了タイミング：時

    //public int user_group{get;set;}          //!< ユーザーグループ [ BELONG_NONE < user_group < BELONG_MAX ]
    public MasterDataDefineLabel.BelongType user_group { get; set; }//!< ユーザーグループ [ BELONG_NONE < user_group < BELONG_MAX ]

    public uint area_id { get; set; }            //!< 補正対象エリア番号		※[ MasterDataArea.fix_id	]と一致
                                                 //public int amend{get;set;}               //!< 補正タイプ	 [ AMEND_NONE < user_group < AMEND_MAX ]
    public MasterDataDefineLabel.AmendType amend { get; set; }//!< 補正タイプ	 [ AMEND_NONE < user_group < AMEND_MAX ]
    public int amend_value { get; set; }     //!< 補正値
};
