using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief		マスターデータ実体：共通パラメータ定義
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataGlobalParams : Master
{
    public int value { get; set; }               //!< 値

    public string text { get; set; }				//!< テキスト
}

