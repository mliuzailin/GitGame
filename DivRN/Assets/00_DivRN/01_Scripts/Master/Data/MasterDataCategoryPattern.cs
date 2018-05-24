using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：クエスト関連：階層内ランダムカテゴリ分布パターン
	@note	階層毎に指定。「この階層でのパネルの中身の割合」を指定するテーブルデータ
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataCategoryPattern : Master
{
    //public uint fix_id;         //!< 情報固有固定ID

    public int cate0 { get; set; }           //!< 出現割合：カラ
    public int cate1 { get; set; }           //!< 出現割合：敵キャラ
    public int cate2 { get; set; }           //!< 出現割合：アイテム
    public int cate3 { get; set; }           //!< 出現割合：トラップ
    public int cate4 { get; set; }           //!< 出現割合：SP回復
    public int cate5 { get; set; }           //!< 出現割合：鍵の数
};

