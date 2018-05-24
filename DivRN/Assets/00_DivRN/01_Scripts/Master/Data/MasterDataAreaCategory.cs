using UnityEngine;
using System.Collections;
using M4u;
using SQLite.Attribute;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：エリア関連：エリアカテゴリ情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataAreaCategory : Master
{
    //public uint fix_id{get;set;}                                    //!< 情報固有固定ID

    public string area_cate_name { get; set; }                            //!< エリアカテゴリ名称

    //public int area_cate_type{get;set;}                             //!< エリアカテゴリタイプ
    public MasterDataDefineLabel.AreaCategory area_cate_type { get; set; }//!< エリアカテゴリタイプ

    public int questlist_sort { get; set; }                               //!< クエスト図鑑表示順

    public uint region_id { get; set; }                               //!< リージョンID

    public string area_cate_detail { get; set; }                          //!< 備考

    public int btn_posx_offset { get; set; }                          //!< ボタン：位置オフセットX
    public int btn_posy_offset { get; set; }                          //!< ボタン：位置オフセットY

    public int background { get; set; }                              //!< 背景アセットバンドル番号
};
