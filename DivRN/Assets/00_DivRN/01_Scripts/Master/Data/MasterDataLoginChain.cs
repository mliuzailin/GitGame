using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ログインボーナス関連：連続ログイン
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataLoginChain : Master
{
    public int login_ct { get; set; }            //!< 連続ログインボーナスパラメータ：ログイン回数
    public string message { get; set; }          //!< 連続ログインボーナスパラメータ：表示テキスト
    public int acquire_money { get; set; }       //!< 連続ログインボーナスパラメータ：プレゼント：お金
    public int acquire_fp { get; set; }          //!< 連続ログインボーナスパラメータ：プレゼント：フレンドポイント
    public int acquire_stone { get; set; }       //!< 連続ログインボーナスパラメータ：プレゼント：魔法石
};
