using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ゲリラボス
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataGuerrillaBoss : Master
{
    //------------------------------------------------
    // ※※※※※※※※※※※※※※※※※※※※※※
    // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
    // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
    // ※※※※※※※※※※※※※※※※※※※※※※
    //------------------------------------------------
    public MasterDataDefineLabel.BoolType active
    {
        get;
        set;
    } //!< データ使用フラグ	※このデータは一部領域を使いまわす。Excelの行ごとに無効化してデータを作るのでフラグを持つ

    public uint timing_start
    {
        get;
        set;
    } //!< イベントタイミング：開始

    public uint timing_end
    {
        get;
        set;
    } //!< イベントタイミング：終了

    public MasterDataDefineLabel.BelongType user_group
    {
        get;
        set;
    } //!< ユーザーグループ [ BELONG_NONE < user_group < BELONG_MAX ]

    public uint enemy_group_id
    {
        get;
        set;
    } //!< 敵キャラID				※[ MasterDataParamEnemyGroup.fix_id ]と一致

    public uint quest_id
    {
        get;
        set;
    } //!< 出現クエストID			※[ MasterDataQuest.fix_id	]と一致

    public uint quest_id_must
    {
        get;
        set;
    } //!< 出現条件クエストID		※[ MasterDataQuest.fix_id	]と一致

    public override string ToString()
    {
        return string.Format("[MasterDataGuerrillaBoss: active={0}, timing_start={1}, timing_end={2}, user_group={3}, enemy_group_id={4}, quest_id={5}, quest_id_must={6}]", active, timing_start, timing_end, user_group, enemy_group_id, quest_id, quest_id_must);
    }
};
