using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ストア関連：商品情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataStoreProduct : Master
{
    public string name { get; set; }                 //!< 商品和名
    public MasterDataDefineLabel.StoreType platform { get; set; }                //!< 商品販売プラットフォーム [ 0:iOS ][ 1:Android ][ 2:ALL ]
    public string id { get; set; }                       //!< 商品ID
    public uint price { get; set; }                  //!< 商品価格
    public uint point { get; set; }                  //!< 商品内包個数

    public int active { get; set; }                  //!< データ使用フラグ
};

