using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：リンクシステム
	@note	リンクシステムパラメータ
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataLinkSystem : Master
{
    public MasterDataDefineLabel.ElementType elem { get; set; }            //!< 基本情報：属性
    public MasterDataDefineLabel.KindType kind { get; set; }            //!< 基本情報：種族
    public MasterDataDefineLabel.RarityType rare { get; set; }            //!< 基本情報：レア度
    public int hp { get; set; }              //!< リンクボーナス：体力
    public int atk { get; set; }         //!< リンクボーナス：攻撃力
    public int crt { get; set; }         //!< リンクボーナス：クリティカル威力
    public int bst { get; set; }         //!< リンクボーナス：BOOSTパネル威力
};
