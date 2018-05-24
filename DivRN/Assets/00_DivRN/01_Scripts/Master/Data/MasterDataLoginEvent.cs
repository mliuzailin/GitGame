using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ログインボーナス関連：期間限定ログイン
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataLoginEvent : Master
{
    public int active { get; set; }              //!< データ使用フラグ	※このデータは一部領域を使いまわす。Excelの行ごとに無効化してデータを作るのでフラグを持つ

    public uint timing { get; set; }             //!< イベントタイミング：開始
    public string message { get; set; }          //!< 期間限定ログインボーナスパラメータ：表示テキスト

    public int acquire_money { get; set; }       //!< 期間限定ログインボーナスパラメータ：プレゼント：お金
    public int acquire_fp { get; set; }          //!< 期間限定ログインボーナスパラメータ：プレゼント：フレンドポイント
    public int acquire_stone { get; set; }       //!< 期間限定ログインボーナスパラメータ：プレゼント：魔法石
    public uint acquire_unit_id { get; set; }    //!< 期間限定ログインボーナスパラメータ：プレゼント：ユニット	※[ MasterDataParamChara.fix_id	]と一致
};

