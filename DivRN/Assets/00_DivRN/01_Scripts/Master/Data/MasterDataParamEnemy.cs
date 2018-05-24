using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：エネミー関連：パラメータ
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataParamEnemy : Master
{
    //------------------------------------------------
    // ※※※※※※※※※※※※※※※※※※※※※※
    // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
    // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
    // ※※※※※※※※※※※※※※※※※※※※※※
    //------------------------------------------------

    public uint timing_public { get; set; }      //!< 一般公開タイミング

    public uint chara_id { get; set; }           //!< エネミーパラメータ：キャラID			※[ MasterDataParamChara.fix_id	]と一致

    public int status_hp { get; set; }           //!< エネミーパラメータ：ステータス：体力
    public int status_pow { get; set; }          //!< エネミーパラメータ：ステータス：攻撃
    public int status_def { get; set; }          //!< エネミーパラメータ：ステータス：防御
    public int status_turn { get; set; }     //!< エネミーパラメータ：ステータス：ターン

    public int acquire_money { get; set; }       //!< エネミーパラメータ：撃破時固定報酬：お金
    public int acquire_exp { get; set; }     //!< エネミーパラメータ：撃破時固定報酬：経験値

    public uint drop_unit_id { get; set; }       //!< エネミーパラメータ：ドロップ：ユニット：ID		※[ MasterDataParamChara.fix_id	]と一致

    public int drop_unit_level { get; set; } //!< エネミーパラメータ：ドロップ：ユニット：レベル		※ データ隠蔽のため０しか入って来ない
    public int drop_unit_rate { get; set; }      //!< エネミーパラメータ：ドロップ：ユニット：確率		※ データ隠蔽のため０しか入って来ない
    public int drop_plus_pow { get; set; }       //!< エネミーパラメータ：ドロップ時プラス値確率：攻撃	※ データ隠蔽のため０しか入って来ない
    public int drop_plus_hp { get; set; }        //!< エネミーパラメータ：ドロップ時プラス値確率：体力	※ データ隠蔽のため０しか入って来ない
    public int drop_plus_none { get; set; }      //!< エネミーパラメータ：ドロップ時プラス値確率：無し	※ データ隠蔽のため０しか入って来ない

    public int drop_money_value { get; set; }    //!< エネミーパラメータ：ドロップ：お金：金額
    public int drop_money_rate { get; set; } //!< エネミーパラメータ：ドロップ：お金：確率

    public int act_table1 { get; set; }          //!< エネミーパラメータ：行動パターン１
    public int act_table2 { get; set; }          //!< エネミーパラメータ：行動パターン２
    public int act_table3 { get; set; }          //!< エネミーパラメータ：行動パターン３
    public int act_table4 { get; set; }          //!< エネミーパラメータ：行動パターン４
    public int act_table5 { get; set; }          //!< エネミーパラメータ：行動パターン５
    public int act_table6 { get; set; }          //!< エネミーパラメータ：行動パターン６
    public int act_table7 { get; set; }          //!< エネミーパラメータ：行動パターン７
    public int act_table8 { get; set; }          //!< エネミーパラメータ：行動パターン８

    public int act_first { get; set; }           //!< エネミーパラメータ：初回行動
    public int act_dead { get; set; }            //!< エネミーパラメータ：死亡行動

    public uint ability1 { get; set; }           //!< エネミーパラメータ：特性1		※[ MasterDataEnemyAbility.fix_id ]と一致
    public uint ability2 { get; set; }           //!< エネミーパラメータ：特性2		※[ MasterDataEnemyAbility.fix_id ]と一致
    public uint ability3 { get; set; }           //!< エネミーパラメータ：特性3		※[ MasterDataEnemyAbility.fix_id ]と一致
    public uint ability4 { get; set; }           //!< エネミーパラメータ：特性4		※[ MasterDataEnemyAbility.fix_id ]と一致
    public uint ability5 { get; set; }           //!< エネミーパラメータ：特性5		※[ MasterDataEnemyAbility.fix_id ]と一致
    public uint ability6 { get; set; }           //!< エネミーパラメータ：特性6		※[ MasterDataEnemyAbility.fix_id ]と一致
    public uint ability7 { get; set; }           //!< エネミーパラメータ：特性7		※[ MasterDataEnemyAbility.fix_id ]と一致
    public uint ability8 { get; set; }           //!< エネミーパラメータ：特性8		※[ MasterDataEnemyAbility.fix_id ]と一致

    public int posz_value { get; set; }          //!< 描画優先度

    public MasterDataDefineLabel.BoolType pos_absolute { get; set; }        //!< メッシュ表示：絶対座標フラグ
    public int posx_offset { get; set; }     //!< メッシュ表示：位置オフセットX
    public int posy_offset { get; set; }     //!< メッシュ表示：位置オフセットY

    public MasterDataDefineLabel.HPGaugeType hp_gauge_type { get; set; }       //!< HPゲージ：種類
    public int hp_posx_offset { get; set; }      //!< HPゲージ：位置オフセットX
    public int hp_posy_offset { get; set; }      //!< HPゲージ：位置オフセットY

    public int target_pos_absolute { get; set; }//!< ターゲットカーソル：絶対座標フラグ
    public int target_posx_offset { get; set; }  //!< ターゲットカーソル：位置オフセットX
    public int target_posy_offset { get; set; } //!< ターゲットカーソル：位置オフセットY


    /// <summary>
    /// コピー
    /// </summary>
    /// <param name="cSrc"></param>
    public void Copy(MasterDataParamEnemy src)
    {
        base.Copy(src);

        timing_public = src.timing_public;
        chara_id = src.chara_id;

        status_hp = src.status_hp;
        status_pow = src.status_pow;
        status_def = src.status_def;
        status_turn = src.status_turn;

        acquire_money = src.acquire_money;
        acquire_exp = src.acquire_exp;

        drop_unit_id = src.drop_unit_id;

        drop_unit_level = src.drop_unit_level;
        drop_unit_rate = src.drop_unit_rate;
        drop_plus_pow = src.drop_plus_pow;
        drop_plus_hp = src.drop_plus_hp;
        drop_plus_none = src.drop_plus_none;

        drop_money_value = src.drop_money_value;
        drop_money_rate = src.drop_money_rate;

        act_table1 = src.act_table1;
        act_table2 = src.act_table2;
        act_table3 = src.act_table3;
        act_table4 = src.act_table4;
        act_table5 = src.act_table5;
        act_table6 = src.act_table6;
        act_table7 = src.act_table7;
        act_table8 = src.act_table8;

        act_first = src.act_first;
        act_dead = src.act_dead;

        ability1 = src.ability1;
        ability2 = src.ability2;
        ability3 = src.ability3;
        ability4 = src.ability4;
        ability5 = src.ability5;
        ability6 = src.ability6;
        ability7 = src.ability7;
        ability8 = src.ability8;

        posz_value = src.posz_value;

        pos_absolute = src.pos_absolute;
        posx_offset = src.posx_offset;
        posy_offset = src.posy_offset;

        hp_gauge_type = src.hp_gauge_type;
        hp_posx_offset = src.hp_posx_offset;
        hp_posy_offset = src.hp_posy_offset;

        target_pos_absolute = src.target_pos_absolute;
        target_posx_offset = src.target_posx_offset;
        target_posy_offset = src.target_posy_offset;
    }

};
