using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
/*!
	@brief		マスターデータ実体： クエスト演出差し替え情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataQuestAppearance : Master
{
    //    public uint timing_public { get; set; }             //!< 一般公開日時

    public uint timing_start { get; set; }              //!< 開始日時
    public uint timing_end { get; set; }                //!< 終了日時

    public uint area_category_id { get; set; }          //!< 対象エリアカテゴリーID
    public string boss_text_key { get; set; }           //!< "BOSS"テキストキー

    public string battle_text_key { get; set; }         //!< "BATTLE"テキストキー

    public string enemy_info_text_key { get; set; }     //!< "ENEMY INFO"テキストキー

    public uint asset_bundle_id { get; set; }           //!< アセットバンドルID
}
