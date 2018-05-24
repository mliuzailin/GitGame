using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：キャラ関連：キャラ進化情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataParamCharaEvol : Master
{
    public uint timing_public { get; set; }      //!< 一般公開タイミング

    public int active { get; set; }              //!< データ使用フラグ	※このデータは一部領域を使いまわす。Excelの行ごとに無効化してデータを作るのでフラグを持つ

    public uint unit_id_pre { get; set; }        //!< ユニットID：進化元（ベース素材）	※[ MasterDataGacha.fix_id	]と一致
    public uint unit_id_after { get; set; }      //!< ユニットID：進化後					※[ MasterDataGacha.fix_id	]と一致
    public uint unit_id_parts1 { get; set; }     //!< ユニットID：素材１					※[ MasterDataGacha.fix_id	]と一致
    public uint unit_id_parts2 { get; set; }     //!< ユニットID：素材２					※[ MasterDataGacha.fix_id	]と一致
    public uint unit_id_parts3 { get; set; }     //!< ユニットID：素材３					※[ MasterDataGacha.fix_id	]と一致
    public uint unit_id_parts4 { get; set; }     //!< ユニットID：素材４					※[ MasterDataGacha.fix_id	]と一致

    public int friend_elem { get; set; }     //!< フレンド制限：属性
    public int friend_kind { get; set; }     //!< フレンド制限：種族
    public int friend_level { get; set; }        //!< フレンド制限：レベル

    public uint money { get; set; }              //!< 必要経費

    public uint quest_id { get; set; }           //!< 進化クエストID
};
