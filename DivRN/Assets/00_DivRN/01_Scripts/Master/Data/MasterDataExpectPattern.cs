using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：クエスト関連：階層内期待値☆分布パターン
	@note	階層毎に指定。「この階層での期待値の分布」を指定するテーブルデータ
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataExpectPattern : Master
{
    //public uint fix_id;     //!< 情報固有固定ID

    public int level1 { get; set; }      //!< 期待値固定数：☆１
    public int level2 { get; set; }      //!< 期待値固定数：☆２
    public int level3 { get; set; }      //!< 期待値固定数：☆３
    public int level4 { get; set; }      //!< 期待値固定数：☆４
    public int level5 { get; set; }      //!< 期待値固定数：☆５
    public int level6 { get; set; }      //!< 期待値固定数：☆６
    public int level7 { get; set; }      //!< 期待値固定数：☆７（！マークで表示）
};
