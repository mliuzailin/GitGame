using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief		マスターデータ実体：限界突破定義
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataLimitOver : Master
{
    public int limit_over_max { get; set; }              //!< 限界突破最大値
    public int limit_over_max_hp { get; set; }           //!< 限界突破最大値ＨＰ
    public int limit_over_max_atk { get; set; }          //!< 限界突破最大値ＡＴＫ
    public int limit_over_max_cost { get; set; }     //!< 限界突破最大値ＣＯＳＴ
    public int limit_over_max_charm { get; set; }        //!< 限界突破最大値ＣＨＡＲＭ
    public MasterDataDefineLabel.LimitOverCurveType limit_grow { get; set; }                  //!< 限界突破成長タイプ
}
