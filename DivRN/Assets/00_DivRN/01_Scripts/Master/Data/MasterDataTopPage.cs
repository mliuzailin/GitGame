using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：トップページ
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataTopPage : Master
{
    public uint timing_public { get; set; }              //!< 一般公開タイミング

    public uint timing_start { get; set; }               //!< イベントタイミング：開始
    public uint timing_end { get; set; }                 //!< イベントタイミング：終了

    public string img_bg { get; set; }                       //!< 画像リソース名：背景

    public string banner_img { get; set; }                   //!< バナー：画像リソース名
    public uint banner_gacha_id { get; set; }            //!< バナー：ガチャID
    public uint banner_quest_id { get; set; }            //!< バナー：クエストID
    public MasterDataDefineLabel.TopPageJump banner_shop_type { get; set; }           //!< バナー：ショップID
    public MasterDataDefineLabel.AchievementCategory banner_achievement_type { get; set; }    //!< バナー：アチーブメントID
    public uint banner_achievement_group_id { get; set; }//!< バナー：アチーブメントグループID

    public string a_cap { get; set; }                        //!< テキストエリア０：タイトル
    public string a_txt { get; set; }                        //!< テキストエリア０：本文
    public uint a_gacha_id { get; set; }                 //!< テキストエリア０：ガチャID
    public uint a_quest_id { get; set; }                 //!< テキストエリア０：クエストID
    public MasterDataDefineLabel.TopPageJump a_shop_type { get; set; }                //!< テキストエリア０：ショップID
    public MasterDataDefineLabel.AchievementCategory a_achievement_type { get; set; }         //!< テキストエリア０：アチーブメントID
    public uint a_achievement_group_id { get; set; }     //!< テキストエリア０：アチーブメントグループID

    public string b_cap { get; set; }                        //!< テキストエリア１：タイトル
    public string b_txt { get; set; }                        //!< テキストエリア１：本文
    public uint b_gacha_id { get; set; }                 //!< テキストエリア１：ガチャID
    public uint b_quest_id { get; set; }                 //!< テキストエリア１：クエストID
    public MasterDataDefineLabel.TopPageJump b_shop_type { get; set; }                //!< テキストエリア１：ショップID
    public MasterDataDefineLabel.AchievementCategory b_achievement_type { get; set; }         //!< テキストエリア１：アチーブメントID
    public uint b_achievement_group_id { get; set; }     //!< テキストエリア１：アチーブメントグループID
}

