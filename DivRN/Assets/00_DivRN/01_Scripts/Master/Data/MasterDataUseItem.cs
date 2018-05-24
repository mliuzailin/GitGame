using UnityEngine;
using System.Collections;
using M4u;
using SQLite.Attribute;

//----------------------------------------------------------------------------
/*!
	@brief		マスターデータ実体：消費アイテム
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataUseItem : Master
{
    public string item_name { get; set; }            //!< 消費アイテム名称
    public string item_icon { get; set; }            //!< 消費アイテムアイコン
    public string item_effect { get; set; }      //!< 使用効果概要
    public string item_effect_detail { get; set; }   //!< 使用効果詳細

    public int stamina_recovery { get; set; }    //!< 効果：スタミナ回復量割合

    public int exp_amend { get; set; }           //!< 効果：経験値補正
    public int coin_amend { get; set; }          //!< 効果：コイン補正
    public int fp_amend { get; set; }            //!< 効果：友情ポイント補正
    public int link_amend { get; set; }          //!< 効果：リンクポイント補正
    public int tk_amend { get; set; }            //!< 効果：チケット補正

    public MasterDataDefineLabel.BoolType enemy_avoid { get; set; }     //!< エネミー連鎖回避有無

    public int effect_span_m { get; set; }       //!< 効果時間：分

    public MasterDataDefineLabel.BoolType quest_use { get; set; }           //!< クエスト入場時使用可否

    public uint gacha_event_id { get; set; }


}

