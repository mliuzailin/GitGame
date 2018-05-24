using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief		マスターデータ実体：サウンド再生情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataAudioData : Master
{
    //public uint fix_id{get;set;}             //!< 情報固有固定ID
    public uint timing_public { get; set; }      //!< 一般公開タイミング

    public uint group_id { get; set; }           //!< 所属グループID
                                                 //public int ducking_disable{get;set;} //!< ダッキング無効化
    public MasterDataDefineLabel.BoolType ducking_disable { get; set; } //!< ダッキング無効化

    public string res_name { get; set; }         //!< オーディオファイル名
    public int vol_lv { get; set; }              //!< ボリュームレベル

    public uint rand_id_00 { get; set; }         //!< ランダム再生：fixID
    public uint rand_id_01 { get; set; }         //!< ランダム再生：fixID
    public uint rand_id_02 { get; set; }         //!< ランダム再生：fixID
    public uint rand_id_03 { get; set; }         //!< ランダム再生：fixID
    public uint rand_id_04 { get; set; }         //!< ランダム再生：fixID
    public uint rand_id_05 { get; set; }         //!< ランダム再生：fixID
}
