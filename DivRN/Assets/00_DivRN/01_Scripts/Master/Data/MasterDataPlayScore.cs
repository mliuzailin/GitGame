using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
/*!
	@brief		マスターデータ実体：プレイスコア定義
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataPlayScore : Master
{
    public MasterDataDefineLabel.PlayScoreType score_type { get; set; }     //!< プレイスコアタイプ

    public int score_param { get; set; }                                    //!< スコア値
}
