using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ガチャ関連：ガチャ詳細アサイン
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataGachaAssign : Master
{
    public MasterDataDefineLabel.BoolType active { get; set; }              //!< データ使用フラグ	※このデータは一部領域を使いまわす。Excelの行ごとに無効化してデータを作るのでフラグを持つ

    public uint gacha_id { get; set; }           //!< ガチャID							※[ MasterDataGacha.fix_id	]と一致
    public uint assign_chara_id { get; set; }    //!< アサインキャラ情報：出現キャラ		※[ MasterDataParamChara.fix_id	]と一致
    public uint assign_rate { get; set; }        //!< アサインキャラ情報：出現確率
    public uint assign_level { get; set; }       //!< アサインキャラ情報：出現レベル


    public uint plus_pow { get; set; }           //!< プラス値確率：攻撃
    public uint plus_hp { get; set; }            //!< プラス値確率：体力
    public uint plus_none { get; set; }          //!< プラス値確率：無し

};
