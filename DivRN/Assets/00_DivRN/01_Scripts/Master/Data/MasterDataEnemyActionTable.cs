using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：敵行動パターン定義
	@note	敵行動パターンテーブル。１匹の敵が戦闘中にやる行動の一覧
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataEnemyActionTable : Master
{
    //public int fix_id{get;set;}                      //!<

    public uint timing_public { get; set; }              //!< 一般公開タイミング

    /*→未使用？*/
    public string change_msg { get; set; }                   //!< 起動タイミング表示メッセージ
    public int change_action_id { get; set; }            //!< 起動アクション定義IDパターン		EnemyActionParamのFixID

    public int timing_priority { get; set; }         //!< 判定優先度							※
                                                     //public int timing_type{get;set;}             //!< 起動タイミング(初回、HP割合)		（MasterDataDefineLabel.ENEMY_ACTPATTERN_SELECT_NONE）
    public MasterDataDefineLabel.EnemyACTPatternSelectType timing_type { get; set; }             //!< 起動タイミング(初回、HP割合)		（MasterDataDefineLabel.ENEMY_ACTPATTERN_SELECT_NONE）
    public int timing_param1 { get; set; }               //!< 起動タイミングパラメータ			※HP割合とかで使う
                                                         /*→未使用？*/
    public int timing_param2 { get; set; }               //!< 起動タイミングパラメータ			※HP割合とかで使う
                                                         /*→未使用？*/
    public int timing_param3 { get; set; }               //!< 起動タイミングパラメータ			※HP割合とかで使う
                                                         /*→未使用？*/
    public int timing_param4 { get; set; }               //!< 起動タイミングパラメータ			※HP割合とかで使う
                                                         /*→未使用？*/
    public int timing_param5 { get; set; }               //!< 起動タイミングパラメータ			※HP割合とかで使う
                                                         /*→未使用？*/
    public int timing_param6 { get; set; }               //!< 起動タイミングパラメータ			※HP割合とかで使う
                                                         /*→未使用？*/
    public int timing_param7 { get; set; }               //!< 起動タイミングパラメータ			※HP割合とかで使う
                                                         /*→未使用？*/
    public int timing_param8 { get; set; }               //!< 起動タイミングパラメータ			※HP割合とかで使う
                                                         //public int action_select_type{get;set;}          //!< アクション選択タイプ(巡回、ループ)	（MasterDataDefineLabel.ENEMY_ACTION_SELECT_NONE）
    public MasterDataDefineLabel.EnemyACTSelectType action_select_type { get; set; }          //!< アクション選択タイプ(巡回、ループ)	（MasterDataDefineLabel.ENEMY_ACTION_SELECT_NONE）
    public int action_param_id1 { get; set; }            //!< アクション定義IDパターン			EnemyActionParamのFixID
    public int action_param_id2 { get; set; }            //!< アクション定義IDパターン			EnemyActionParamのFixID
    public int action_param_id3 { get; set; }            //!< アクション定義IDパターン			EnemyActionParamのFixID
    public int action_param_id4 { get; set; }            //!< アクション定義IDパターン			EnemyActionParamのFixID
    public int action_param_id5 { get; set; }            //!< アクション定義IDパターン			EnemyActionParamのFixID
    public int action_param_id6 { get; set; }            //!< アクション定義IDパターン			EnemyActionParamのFixID
    public int action_param_id7 { get; set; }            //!< アクション定義IDパターン			EnemyActionParamのFixID
    public int action_param_id8 { get; set; }            //!< アクション定義IDパターン			EnemyActionParamのFixID


    public int Get_timing_type_count()
    {
        return 1;
    }

    public MasterDataDefineLabel.EnemyACTPatternSelectType Get_timing_type(int index)
    {
        MasterDataDefineLabel.EnemyACTPatternSelectType ret_val = MasterDataDefineLabel.EnemyACTPatternSelectType.NONE;

        switch (index)
        {
            case 0:
                ret_val = timing_type;
                break;
        }

        return ret_val;
    }

    public int Get_timing_type_param(int index)
    {
        int ret_val = 0;

        switch (index)
        {
            case 0:
                ret_val = timing_param1;
                break;
        }

        return ret_val;
    }
};
