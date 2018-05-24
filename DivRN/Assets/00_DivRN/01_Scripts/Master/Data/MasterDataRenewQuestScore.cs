using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：クエスト関連：クエストスコア情報
	@note	クエストスコア情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataRenewQuestScore : Master
{
    public int base_score { get; set; }             //!< 基礎スコア

    public int play_score_rate { get; set; }        //!< プレイスコア補正

    public int turn_penalty { get; set; }           //!< ターンペナルティ
}
