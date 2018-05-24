using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：イベント関連：特効イベント
	@note	特効イベント情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataEventSpecial : Master
{
    public uint event_id { get; set; }  //!< MasterDataEventのfix_id

    public MasterDataDefineLabel.SpecialEffectType effect_type { get; set; }    //!< 特効効果範囲タイプ

    public int effect_param { get; set; }   //!< 特効効果範囲パラメータ

    public uint chara_id { get; set; }  //!< MasterDataParamCharaのfix_id

    public uint chara_special_id { get; set; }  //!< MasterDataParamCharaSpecialのfix_id
}
