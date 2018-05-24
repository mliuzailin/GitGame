using UnityEngine;
using System.Collections;
using SQLite.Attribute;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ログインボーナス関連：プレゼントグループ
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataPresentGroup : Master
{
    [Ignore]
    public override uint tag_id { get; set; }

    public MasterDataDefineLabel.BoolType active { get; set; }              //!< データ使用フラグ	※このデータは一部領域を使いまわす。Excelの行ごとに無効化してデータを作るのでフラグを持つ

    public uint group_id { get; set; }           //!< プレゼントグループ定義：グループID
    public uint present_id { get; set; }         //!< プレゼントグループ定義：プレゼントID（MasterDataPresent.fix_id）
    public int present_num { get; set; }     //!< プレゼントグループ定義：

};
