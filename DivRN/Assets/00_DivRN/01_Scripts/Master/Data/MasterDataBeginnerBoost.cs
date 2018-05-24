using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：初心者ブースト
	@note	初心者ブーストパラメータ
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataBeginnerBoost : Master
{

    public MasterDataBeginnerBoost()
    {

    }

    //public int fix_id;                  //!< 
    public uint event_id { get; set; }               //!< 公開条件イベント	※出力してないけどイベント固有ID

    public int rank_min { get; set; }                //!< 適用ランク下限（以上）
    public int rank_max { get; set; }                //!< 適用ランク上限（以下）

    public int boost_exp { get; set; }               //!< ボーナス倍率：クエストの経験値倍率			※通常クエ,進化クえ
    public int boost_money { get; set; }         //!< ボーナス倍率：クエストの取得金額倍率		※通常クエ,進化クえ
    public int boost_stamina_use { get; set; }       //!< ボーナス倍率：クエストのスタミナ消費倍率	※通常クエ,進化クえ
    public int boost_build_money { get; set; }       //!< ボーナス倍率：合成の消費金額倍率			※強化,進化
    public int boost_build_great { get; set; }       //!< ボーナス倍率：合成の大成功発生確率倍率		※強化
};

