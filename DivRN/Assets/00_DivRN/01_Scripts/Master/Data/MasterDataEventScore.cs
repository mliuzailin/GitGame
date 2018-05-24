using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：イベント関連：スコアイベント
	@note	スコアイベント情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataEventScore : Master
{
    public uint event_id { get; set; }              //!< MasterDataEventのfix_id

    public string title { get; set; }               //!< タイトル

    public string banner_url { get; set; }          //!< バナー画像URL

    public int priority { get; set; }               //!< 表示順
}
