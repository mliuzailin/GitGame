using UnityEngine;
using System.Collections;
using M4u;
using SQLite.Attribute;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：クエスト関連：クエスト情報
	@note	マスターデータは同一エリアで連鎖してデータが存在し、エリアIDが変化するまでを１つのエリアのクエストと見なす
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataQuest2 : Master
{

    public uint timing_public
    {
        get;
        set;
    } //!< 一般公開タイミング

    public uint area_id
    {
        get;
        set;
    } //!< エリア通し番号		※[ MasterDataArea.fix_id ]と一致

    public MasterDataDefineLabel.BoolType active
    {
        get;
        set;
    } //!< データ使用フラグ	※クエストクリア状況をセーブするのに連番化する必要があり、リザーブをデータに含めることで対応する

    /// <summary>
    /// クエスト名称
    /// </summary>
    public string quest_name
    {
        get;
        set;
    }

    /// <summary>
    /// 難易度名称
    /// </summary>
    public string difficulty_name
    {
        get;
        set;
    }

    public MasterDataDefineLabel.BoolType once
    {
        get;
        set;
    } //!< 一度クリアしたら二度と出現しないクエスト	※1.0.6で機能追加

    public MasterDataDefineLabel.BoolType enable_continue
    {
        get;
        set;
    } //!< コンティニュー有無

    public MasterDataDefineLabel.BoolType enable_friendpoint
    {
        get;
        set;
    } //!< フレンドポイント有無

    public MasterDataDefineLabel.BoolType enable_autoplay
    {
        get;
        set;
    } //!< オートプレイの有無

    public int consume_type
    {
        get;
        set;
    } //!< 消費タイプ

    public int consume_value
    {
        get;
        set;
    } //!< 消費値

    public int quest_floor_bonus_type
    {
        get;
        set;
    } //!< フロアボーナスタイプ

    public int clear_money
    {
        get;
        set;
    } //!< クリア報酬：お金

    public int clear_exp
    {
        get;
        set;
    } //!< クリア報酬：経験値

    public int clear_link_point
    {
        get;
        set;
    } //!< クリア報酬：リンクポイント

    public int clear_stone
    {
        get;
        set;
    } //!< クリア報酬：魔法石（初回限定）

    public uint clear_unit
    {
        get;
        set;
    } //!< クリア報酬：ユニット						※1.0.6で機能追加

    public uint clear_unit_lv
    {
        get;
        set;
    } //!< クリア報酬：ユニットレベル					※1.0.6で機能追加

    public string clear_unit_msg
    {
        get;
        set;
    } //!< クリア報酬：ユニット取得時文言				※1.0.6で機能追加

    public uint clear_item
    {
        get;
        set;
    } //!< クリア報酬：アイテム						※3.4.0で機能追加

    public uint clear_item_num
    {
        get;
        set;
    } //!< クリア報酬：アイテム個数					※3.4.0で機能追加

    public string clear_item_msg
    {
        get;
        set;
    } //!< クリア報酬：アイテム取得時文言				※3.4.0で機能追加

    public uint clear_key
    {
        get;
        set;
    } //!< クリア報酬：クエストキー					※3.5.0で機能追加

    public uint clear_key_num
    {
        get;
        set;
    } //!< クリア報酬：クエストキー個数				※3.5.0で機能追加

    public string clear_key_msg
    {
        get;
        set;
    } //!< クリア報酬：取得時文言						※3.5.0で機能追加

    public uint limit_money
    {
        get;
        set;
    } //!< チート対策上限：お金

    public uint limit_exp
    {
        get;
        set;
    } //!< チート対策上限：経験値

    public uint quest_requirement_id
    {
        get;
        set;
    } //!< クエスト入場条件通し番号		※[ MasterDataQuest2Requirement.fix_id ]と一致

    public string quest_requirement_text
    {
        get;
        set;
    } //!< クエスト入場条件テキスト

    public uint story
    {
        get;
        set;
    } //!< ストーリー

    public uint voice_group_id
    {
        get;
        set;
    } //!< ボイスグループID

    public string packname_voice
    {
        get;
        set;
    } //!< ボイスパック名

    public MasterDataDefineLabel.BoolType boss_icon_hide
    {
        get;
        set;
    } //!< ボスアイコンを隠す(「？」表示にする)フラグ

    public uint boss_icon_unit
    {
        get;
        set;
    } //!< ボスアイコン情報

    public uint boss_ability_1
    {
        get;
        set;
    } //!< ボス特性_1

    public uint boss_ability_2
    {
        get;
        set;
    } //!< ボス特性_2

    public uint boss_ability_3
    {
        get;
        set;
    } //!< ボス特性_3

    public uint boss_ability_4
    {
        get;
        set;
    } //!< ボス特性_4

    public uint boss_chara_id
    {
        get;
        set;
    } //!< 出現するボスID

    public uint e_chara_id_0
    {
        get;
        set;
    } //!< 出現する敵ID

    public uint e_chara_id_1
    {
        get;
        set;
    } //!< 出現する敵ID

    public uint e_chara_id_2
    {
        get;
        set;
    } //!< 出現する敵ID

    public uint e_chara_id_3
    {
        get;
        set;
    } //!< 出現する敵ID

    public uint e_chara_id_4
    {
        get;
        set;
    } //!< 出現する敵ID

    public int battle_count
    {
        get;
        set;
    } //!< クエストバトル数

    public uint enemy_group_id_1
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆１		※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_2
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆２		※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_3
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆３		※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_4
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆４		※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_5
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆５		※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_6
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆６		※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_7
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆７		※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_8
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆８		※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_9
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆９		※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_10
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆１０	※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_11
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆１１	※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_12
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆１２	※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_13
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆１３	※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_14
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆１４	※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_15
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆１５	※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_16
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆１６	※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_17
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆１７	※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_18
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆１８	※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint enemy_group_id_19
    {
        get;
        set;
    } //!< 出現エネミーグループ：☆１９	※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint call_group_id
    {
        get;
        set;
    } //!< エネミー召喚用グループ	※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint boss_group_id
    {
        get;
        set;
    } //!< ボスエネミーグループ番号	※[ MasterDataEnemyGroup.fix_id ]と一致

    public uint first_attack
    {
        get;
        set;
    } //!< ボスグループへの先制攻撃の確立

    public uint back_attack
    {
        get;
        set;
    } //!< ボスグループの不意打ち攻撃の確立

    public int background
    {
        get;
        set;
    } //!< 背景アセットバンドル番号

    public uint quest_score_id
    {
        get;
        set;
    }//!< MasterDataQuestScoreのfix_id
}
