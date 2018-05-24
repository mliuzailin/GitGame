using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ガチャ関連：ガチャグループ
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataGachaGroup : Master
{

    public uint timing_public { get; set; }               //!< 公開日

    public uint sale_period_start { get; set; }           //!< 販売開始日
    public uint sale_period_end { get; set; }             //!< 販売終了日

    public string event_detail { get; set; }                //!< イベント詳細
    public string guideline_text_key { get; set; }          //!< ガイドラインテキストキー
}
