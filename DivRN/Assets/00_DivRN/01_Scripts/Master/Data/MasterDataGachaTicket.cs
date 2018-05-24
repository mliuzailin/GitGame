using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief		マスターデータ実体：ガチャチケット
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataGachaTicket : Master
{
    //public uint fix_id{get;set;}             //!< 情報固有固定ID

    public uint timing_public { get; set; }      //!< 一般公開タイミング

    public int gacha_ticket_id { get; set; } //!< ガチャチケット定義の該当番号

    public uint gacha_id { get; set; }           //!< ガチャ定義ID

    public uint gacha_ct { get; set; }           //!< ガチャ連続回数

    public string gacha_tk_name { get; set; }        //!< ガチャチケット名
    public string gacha_tk_msg { get; set; }     //!< ガチャ詳細メッセージ
}
