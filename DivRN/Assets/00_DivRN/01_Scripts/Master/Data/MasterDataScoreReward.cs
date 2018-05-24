using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
/*!
	@brief		マスターデータ実体：スコア報酬
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataScoreReward : Master
{
    public uint event_id { get; set; }          //!< イベントID

    public MasterDataDefineLabel.ScoreRewardType type { get; set; }     //!< 達成タイプ

    public int score { get; set; }              //!< 達成スコア

    public uint present_group_id { get; set; }  //!< プレゼントグループID

    public string present_message { get; set; } //!< プレゼントメッセージ
}
