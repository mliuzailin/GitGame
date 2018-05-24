using UnityEngine;
using System.Collections;
using M4u;
using SQLite.Attribute;

//----------------------------------------------------------------------------
/*!
    @brief  マスターデータ実体：エリア関連：エリア情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataArea : Master
{
    //public uint fix_id;                 //!< 情報固有固定ID

    public int view_id { get; set; }                //!< クライアント表示順

    public uint timing_public { get; set; }          //!< 一般公開タイミング

    public string area_name { get; set; }
    public string area_name_eng { get; set; }
    public string area_detail { get; set; }

    public uint area_cate_id { get; set; }           //!< エリアカテゴリID    ※[ MasterDataAreaCategory.fix_id ]と一致
    public uint questlist_sort { get; set; }         //!< クエスト図鑑表示順

    public uint event_id { get; set; }               //!< 公開条件イベント    ※出力してないけどイベント固有ID
    //public MasterDataDefineLabel.AreaType type{get;set;} //!< エリアタイプ
    public int type { get; set; }                    //!< エリアタイプ
    public uint pre_area { get; set; }               //!< クリア必須エリア    ※[ MasterDataArea.fix_id ]と一致

    public int cost0 { get; set; }                   //!< コスト枚数分布：無
    public int cost1 { get; set; }                   //!< コスト枚数分布：火
    public int cost2 { get; set; }                   //!< コスト枚数分布：水
    public int cost3 { get; set; }                   //!< コスト枚数分布：風
    public int cost4 { get; set; }                   //!< コスト枚数分布：光
    public int cost5 { get; set; }                   //!< コスト枚数分布：闇
    public int cost6 { get; set; }                   //!< コスト枚数分布：回復

    //public int area_element{get;set;}            //!< エリア属性
    public MasterDataDefineLabel.ElementType area_element { get; set; }            //!< エリア属性
    public string area_effect { get; set; }          //!< エリアエフェクト名
    public string mesh_map { get; set; }             //!< リソース：メッシュ名称：マップ
    public string mesh_door { get; set; }                //!< リソース：メッシュ名称：扉
    public string mesh_door_boss { get; set; }           //!< リソース：メッシュ名称：扉（ボス）
    public string res_map { get; set; }              //!< リソース：テクスチャ名称：マップ
    public string res_map_icon { get; set; }         //!< リソース：テクスチャ名称：マップアイコン

    public string res_icon_key { get; set; }         //!< リソース：テクスチャ：パネルアイコン：鍵
    public string res_icon_box { get; set; }         //!< リソース：テクスチャ：パネルアイコン：宝箱

    public string packname_se { get; set; }          //!< SEパック
    public string packname_bgm { get; set; }         //!< BGMパック

    public string area_url { get; set; }             //!< 概要URL

};
