using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ログインボーナス関連：月間ログイン
	@name	捏造クラス専用
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataLoginMonthly : Master
{
    public MasterDataDefineLabel.BoolType active { get; set; }  //!< データ使用フラグ	※このデータは一部領域を使いまわす。Excelの行ごとに無効化してデータを作るのでフラグを持つ

    public uint date { get; set; }                              //!< イベントタイミング：開始
    public int present_group_id { get; set; }                   //!< いらんやつ

    public string message { get; set; }                         //!< ダイアログメッセージ

    public uint notification1_timing { get; set; }              //!< プッシュ通知1：時間
    public string notification1_title { get; set; }             //!< プッシュ通知1：タイトル
    public string notification1_body { get; set; }              //!< プッシュ通知1：メッセージ
    public uint notification2_timing { get; set; }              //!< プッシュ通知2：時間
    public string notification2_title { get; set; }             //!< プッシュ通知2：タイトル
    public string notification2_body { get; set; }              //!< プッシュ通知2：メッセージ
};

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ログインボーナス関連：サーバー変換後
*/
//----------------------------------------------------------------------------
public class MasterDataLoginMonthlyConverted : Master
{
    public uint date { get; set; }          //!< イベントタイミング：開始
    public int[] present_ids { get; set; }  //!< プレゼントID群
    public string message { get; set; }     //!< 自由メッセージ
};

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：期間ログインボーナス関連：サーバー変換後
*/
//----------------------------------------------------------------------------
public class MasterDataPeriodLoginConverted : Master
{
    public uint date_count { get; set; }    //!< 指定ログイン日数
    public int[] present_ids { get; set; }  //!< プレゼントID
    public string message { get; set; }     //!< ダイアログメッセージ
};
