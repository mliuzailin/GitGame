using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：テキスト定義情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataTextDefinition : Master
{
    public string text_key { get; set; }         //!< キー値
    public string text { get; set; }             //!< テキスト
};
