using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：エネミー関連：同時出現グループ
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataEnemyGroup : Master
{
    //public uint fix_id;                 //!< 情報固有固定ID

    //public int fix;                 //!< エネミーグループ：完全固定フラグ
    public MasterDataDefineLabel.BoolType fix { get; set; }                 //!< エネミーグループ：完全固定フラグ

    public int num_min { get; set; }             //!< エネミーグループ：キャラ数下限
    public int num_max { get; set; }             //!< エネミーグループ：キャラ数上限

    public uint enemy_id_1 { get; set; }             //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_2 { get; set; }             //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_3 { get; set; }             //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_4 { get; set; }             //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_5 { get; set; }             //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_6 { get; set; }             //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_7 { get; set; }             //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_8 { get; set; }             //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_9 { get; set; }             //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_10 { get; set; }            //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_11 { get; set; }            //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_12 { get; set; }            //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_13 { get; set; }            //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_14 { get; set; }            //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_15 { get; set; }            //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_16 { get; set; }            //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_17 { get; set; }            //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_18 { get; set; }            //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_19 { get; set; }            //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致
    public uint enemy_id_20 { get; set; }            //!< エネミーグループ：選択肢キャラ		※[ MasterDataParamEnemy.fix_id	]と一致

    public uint chain_id { get; set; }               //!< 連鎖するエネミーグループ			※1.0.6で機能追加
    public int chain_turn_offset { get; set; }       //!< 連鎖した際のターンオフセット		※1.0.6で機能追加
    public int drop_type { get; set; }               //!< 敵のタイプ毎にドロップタイプを変更 ※2.4.0で機能追加

    public int evol_direct_type { get; set; }        //!< 進化演出タイプ

    //クライアントでは使用しない	public int hate_id{get;set;}        //!< ヘイトID

    public int bgm_id { get; set; }                //!< BGM ID
};
