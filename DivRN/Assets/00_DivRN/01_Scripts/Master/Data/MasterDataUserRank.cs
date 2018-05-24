using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ユーザー関連：ランク
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataUserRank : Master
{
    public uint exp_next { get; set; }       //!< ランクパラメータ：経験値：次までの数値
    public uint exp_next_total { get; set; } //!< ランクパラメータ：経験値：次までの合計値

    public uint stamina { get; set; }        //!< ランクパラメータ：スタミナ値
    public uint friend_max { get; set; }     //!< ランクパラメータ：フレンド保持数上限
    public uint unit_max { get; set; }       //!< ランクパラメータ：ユニット保持数上限
    public uint party_cost { get; set; }     //!< ランクパラメータ：パーティコスト上限
};

