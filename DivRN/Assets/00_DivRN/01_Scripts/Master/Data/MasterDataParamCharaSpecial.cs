using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：キャラ関連：キャラ特効情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataParamCharaSpecial : Master
{
    public int hp { get; set; }     //!< HP補正

    public int atk { get; set; }    //!< ATK補正

    public int score { get; set; }  //!< スコア補正
}
