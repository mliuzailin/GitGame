using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
/*!
	@brief		マスターデータ実体：イベントスコア
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataScoreEvent : Master
{
    public uint event_id { get; set; }          //!< イベントID
    public uint timing_start { get; set; }      //!< スコアの報酬が受け取れる期間（開始）
    public uint receiving_end { get; set; }     //!< スコアの報酬が受け取れる期間（終了）

    public string title { get; set; }           //!< イベント名

    public uint image_present_id { get; set; }  //!< イベントアイコン用プレゼントID

    public string banner_url { get; set; }      //!< バナーURL

    public int priority { get; set; }           //!< 表示優先度

    public uint area_category_id { get; set; }  //!< 対象エリア
}
