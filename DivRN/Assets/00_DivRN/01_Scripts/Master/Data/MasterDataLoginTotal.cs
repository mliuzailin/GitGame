using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ログインボーナス関連：通算ログイン
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataLoginTotal : Master
{
    public int login_ct { get; set; }            //!< 通算ログインボーナスパラメータ：ログイン回数
    public string message { get; set; }          //!< 通算ログインボーナスパラメータ：表示テキスト
    public int acquire_money { get; set; }       //!< 通算ログインボーナスパラメータ：プレゼント：お金
    public int acquire_fp { get; set; }          //!< 通算ログインボーナスパラメータ：プレゼント：フレンドポイント
    public int acquire_stone { get; set; }       //!< 通算ログインボーナスパラメータ：プレゼント：魔法石
    public uint acquire_unit_id { get; set; }    //!< 通算ログインボーナスパラメータ：プレゼント：ユニット	※[ MasterDataParamChara.fix_id	]と一致
};
