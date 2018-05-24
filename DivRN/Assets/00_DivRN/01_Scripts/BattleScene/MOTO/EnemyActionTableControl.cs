/*==========================================================================*/
/*	using																	*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/*==========================================================================*/
/*	class																	*/
/*==========================================================================*/
//---------------------------------------------------------------------------
/*!
    @class		EnemyActionTableControl
    @brief		敵行動テーブルコントロール
    @note		敵一体につき一コントロール持たせる
*/
//---------------------------------------------------------------------------
public class EnemyActionTableControl
{

    private readonly int UNUSE_ENEMY_ID = -1;       //!< 未使用定義ID(敵)
    public readonly int UNUSE_ACTIONTABLE_ID = -1;      //!< 未使用定義ID(行動パラメータテーブル)
    private readonly int UNUSE_ACTIONPARAM_ID = 0;      //!< 未使用定義ID(行動パラメータ)


    public int m_CurrentActionTableID;              //!< 現在のアクションテーブル
    public BattleEnemy m_BattleEnemy = null;
    public int m_ActionStep;                        //!< 進行状態
    public bool m_using;                            //!< 使用中フラグ

    public Rand m_RandomActionTable = null; //!< ランダム行動用乱数生成クラス
    public uint m_RandomSeed;                       //!< ランダム行動用シード値

    public int m_ActionSwitchParamID = 0;       //!< 行動パターン切り替わり時：行動ID

    //-----------------------------------------------------------------------
    /*!
        @brief		使用状態の取得
        @retval		bool		[使用中/未使用]
    */
    //-----------------------------------------------------------------------
    public bool isUsing()
    {

        return m_using;

    }


    //-----------------------------------------------------------------------
    /*!
        @brief		セットアップ
        @param[in]	int		(enemyID)		敵ID
    */
    //-----------------------------------------------------------------------
    public bool Setup()
    {
        //--------------------------------
        //	使用中
        //--------------------------------
        m_using = true;


        //--------------------------------
        // 乱数シード生成
        //--------------------------------
        m_RandomSeed = RandManager.GetRand();
        m_RandomActionTable = new Rand();


        //--------------------------------
        // 乱数シード設定
        //--------------------------------
        SetupRandomSeed(m_RandomSeed);


        //--------------------------------
        //	行動テーブル進行度
        //--------------------------------
        m_ActionStep = 0;


        //--------------------------------
        //	行動パターン切り替わり時：行動ID
        //--------------------------------
        m_ActionSwitchParamID = 0;


        //--------------------------------
        //	現在のテーブル
        //--------------------------------
        m_CurrentActionTableID = UNUSE_ACTIONTABLE_ID;


        return true;
    }


    //-----------------------------------------------------------------------
    /*!
        @brief		乱数シードの設定
    */
    //-----------------------------------------------------------------------
    public void SetupRandomSeed(uint unSeed)
    {


        if (m_RandomActionTable == null)
        {
            return;
        }


        //--------------------------------
        // 乱数シード設定
        //--------------------------------
        m_RandomActionTable.SetRandSeed(unSeed);
    }


    //-----------------------------------------------------------------------
    /*!
        @brief		クリア
    */
    //-----------------------------------------------------------------------
    public void Clear()
    {
        //--------------------------------
        // 使用中フラグ
        //--------------------------------
        m_using = false;


        //--------------------------------
        // 現在選択中のアクションテーブルクリア
        //--------------------------------
        m_CurrentActionTableID = UNUSE_ACTIONTABLE_ID;


        //--------------------------------
        //	行動テーブル進行度
        //--------------------------------
        m_ActionStep = 0;

        //--------------------------------
        //	行動パターン切り替わり時：行動ID
        //--------------------------------
        m_ActionSwitchParamID = 0;
    }


    //-----------------------------------------------------------------------
    /*!
        @brief		現在のテーブルパラメータを取得
    */
    //-----------------------------------------------------------------------
    public MasterDataEnemyActionTable GetCurrentActionTableParam()
    {

        //--------------------------------
        //	未定義IDを参照した場合
        //--------------------------------
        if (m_CurrentActionTableID == UNUSE_ACTIONTABLE_ID)
        {
            return null;
        }


        //--------------------------------
        //	現在の行動テーブルデータを参照する
        //--------------------------------
        MasterDataEnemyActionTable tableData = BattleParam.m_MasterDataCache.useEnemyActionTable((uint)m_CurrentActionTableID);
        if (tableData == null)
        {
            return null;
        }


        return tableData;

    }


    //-----------------------------------------------------------------------
    /*!
        @brief		現在の行動パラメータを取得
    */
    //-----------------------------------------------------------------------
    public MasterDataEnemyActionParam GetCurrentActionParam()
    {

        MasterDataEnemyActionParam actionParam = null;

        //--------------------------------
        // テーブル切り替え時：行動IDがある場合
        // @add Developer v320
        //--------------------------------
        if (m_ActionSwitchParamID > UNUSE_ACTIONPARAM_ID)
        {
            // アクションパラメータのマスターデータを取得
            actionParam = BattleParam.m_MasterDataCache.useEnemyActionParam((uint)m_ActionSwitchParamID);
            return (actionParam);
        }

        //--------------------------------
        // 現在の行動テーブルデータを参照する
        //--------------------------------
        MasterDataEnemyActionTable tableData = GetCurrentActionTableParam();
        if (tableData == null)
        {
            return null;
        }


        //--------------------------------
        // 現在のアクションパラメータを取得
        //--------------------------------
        int actionStep = m_ActionStep;


        int[] actionParamIDArray = { tableData.action_param_id1,
                                     tableData.action_param_id2,
                                     tableData.action_param_id3,
                                     tableData.action_param_id4,
                                     tableData.action_param_id5,
                                     tableData.action_param_id6,
                                     tableData.action_param_id7,
                                     tableData.action_param_id8 };

        int actionParamID = actionParamIDArray[actionStep];
        if (actionParamID == UNUSE_ACTIONPARAM_ID)
        {
            return null;
        }


        //--------------------------------
        // アクションパラメータのマスターデータの取得
        //--------------------------------
        actionParam = BattleParam.m_MasterDataCache.useEnemyActionParam((uint)actionParamID);


        return actionParam;

    }


    //-----------------------------------------------------------------------
    /*!
        @brief		単発アクション[初回]があるか確認
    */
    //-----------------------------------------------------------------------
    public MasterDataEnemyActionParam GetActionFirstAttack()
    {
        if (m_BattleEnemy == null)
        {
            return null;
        }


        MasterDataParamEnemy enemyParam = m_BattleEnemy.getMasterDataParamEnemy();
        if (enemyParam == null)
        {
            return null;
        }


        //--------------------------------
        //	指定フェイズにアクションが設定されているか
        //--------------------------------
        if (enemyParam.act_first == 0)
        {
            return null;
        }

        //		return MasterDataUtil.GetMasterDataEnemyActionParam( enemyParam.act_first );
        return BattleParam.m_MasterDataCache.useEnemyActionParam((uint)enemyParam.act_first);
    }


    //-----------------------------------------------------------------------
    /*!
        @brief		アクションテーブルの切り替え判定
    */
    //-----------------------------------------------------------------------
    public void SelectActionTable(int battle_total_turn)
    {
        MasterDataEnemyActionTable enemy_action_table = searchCurrentMasterDataEnemyActionTable(battle_total_turn);

        int table_id_current = UNUSE_ACTIONTABLE_ID;
        if (enemy_action_table != null)
        {
            table_id_current = (int)enemy_action_table.fix_id;
        }

        //--------------------------------
        //	テーブルID設定
        //--------------------------------
        if (m_CurrentActionTableID != table_id_current)
        {

#if UNITY_EDITOR && OUTPUT_INGAME_LOG
        Debug.LogError( "SelectActionTable <<< " + table_id_current );
#endif // #if UNITY_EDITOR && OUTPUT_INGAME_LOG

            //--------------------------------
            //	テーブルID更新
            //--------------------------------
            m_CurrentActionTableID = table_id_current;

            //--------------------------------
            //	更新された場合は、進行状態を更新する
            //--------------------------------
            if (enemy_action_table != null)
            {

                //--------------------------------
                // テーブル切り替え時：行動IDを保存
                //--------------------------------
                m_ActionSwitchParamID = enemy_action_table.change_action_id;

                if (enemy_action_table.action_select_type == MasterDataDefineLabel.EnemyACTSelectType.RAND)
                {
                    //--------------------------------
                    //	ランダム選択の場合は抽選を行う
                    //--------------------------------
                    StepActionTable();
                }
                else
                {
                    //--------------------------------
                    //	巡回選択時は先頭に設定
                    //--------------------------------
                    m_ActionStep = 0;
                }
            }

        }


    }


    //-----------------------------------------------------------------------
    /*!
        @brief		アクションテーブルの切り替え時行動確認
        @add		Developer 2016/01/04 v320
    */
    //-----------------------------------------------------------------------
    public bool CheckActionSwitch(int battle_total_turn)
    {
        bool bResult = false;

        MasterDataEnemyActionTable enemy_action_table = searchCurrentMasterDataEnemyActionTable(battle_total_turn);

        int table_id_current = UNUSE_ACTIONTABLE_ID;
        if (enemy_action_table != null)
        {
            table_id_current = (int)enemy_action_table.fix_id;
        }

        //--------------------------------
        // テーブルID設定
        //--------------------------------
        if (m_CurrentActionTableID != table_id_current)
        {
            //--------------------------------
            // 更新された場合は、進行状態を更新する
            //--------------------------------
            if (enemy_action_table != null
            && enemy_action_table.change_action_id != 0)
            {
                //--------------------------------
                // 行動パターン切り替わり時行動有り
                //--------------------------------
                bResult = true;

                //--------------------------------
                // 行動テーブル進行度の決定：ここでやりたくないが、中断復帰の都合上このタイミングでやる
                //--------------------------------
                if (enemy_action_table.action_select_type == MasterDataDefineLabel.EnemyACTSelectType.RAND)
                {
                    //--------------------------------
                    // ランダム選択の場合は抽選を行う
                    //--------------------------------
                    StepActionTable();
                }
                else
                {
                    //--------------------------------
                    // 巡回選択時は先頭に設定
                    //--------------------------------
                    m_ActionStep = 0;
                }
            }
        }

        return (bResult);
    }


    //-----------------------------------------------------------------------
    /*!
        @brief		ターン数経過条件を満たしているか
    */
    //-----------------------------------------------------------------------
    private bool checkTimingTotalTurn(int battle_total_turn, int check_turn)
    {
        if (battle_total_turn < check_turn)
        {
            return false;
        }

        return true;
    }


    //-----------------------------------------------------------------------
    /*!
        @brief		HPの指定割合以上
    */
    //-----------------------------------------------------------------------
    private bool checkTimingHPRate(int check_hp)
    {
        //--------------------------------
        // HP割合をみる
        //--------------------------------
        float fRate = (float)InGameUtilBattle.GetDBRevisionValue(check_hp);
        int nEnemyHP = (int)InGameUtilBattle.AvoidErrorMultiple(m_BattleEnemy.m_EnemyHPMax, fRate);

        //--------------------------------
        //	指定割合"以上"でない物は通さない
        //--------------------------------
        if (m_BattleEnemy.m_EnemyHP < nEnemyHP)
        {
            return false;
        }

        return true;
    }


    //-----------------------------------------------------------------------
    /*!
        @brief		HPの指定割合以下
    */
    //-----------------------------------------------------------------------
    private bool checkTimingHPRateUnder(int check_hp)
    {
        //--------------------------------
        // HP割合をみる
        //--------------------------------
        float fRate = (float)InGameUtilBattle.GetDBRevisionValue(check_hp);
        int nEnemyHP = (int)InGameUtilBattle.AvoidErrorMultiple(m_BattleEnemy.m_EnemyHPMax, fRate);

        //--------------------------------
        //	指定割合"以下"でない物は通さない
        //--------------------------------
        if (m_BattleEnemy.m_EnemyHP > nEnemyHP)
        {
            return false;
        }

        return true;
    }



    //-----------------------------------------------------------------------
    /*!
        @brief		アクションテーブル更新
    */
    //-----------------------------------------------------------------------
    public void StepActionTable()
    {


        //--------------------------------
        // 現在の行動テーブルデータを参照する
        //--------------------------------
        MasterDataEnemyActionTable table = GetCurrentActionTableParam();
        if (table == null)
        {
            return;
        }


        int[] actionParamArray = { table.action_param_id1,
                                   table.action_param_id2,
                                   table.action_param_id3,
                                   table.action_param_id4,
                                   table.action_param_id5,
                                   table.action_param_id6,
                                   table.action_param_id7,
                                   table.action_param_id8 };

        MasterDataDefineLabel.EnemyACTSelectType selectType = table.action_select_type;



        //--------------------------------
        //	セーフティ
        //--------------------------------
        if (m_ActionStep < 0)
        {
            m_ActionStep = 0;
        }


        //--------------------------------
        // 進行状態の更新
        //--------------------------------
        switch (selectType)
        {
            case MasterDataDefineLabel.EnemyACTSelectType.RAND:
                {
                    //--------------------------------
                    // ランダムセレクト設定
                    //--------------------------------
                    int totalCount = 0;
                    int randomVal = 0;
                    int randomMix = 1;
                    int randomMax = 100;
                    int tempValue = 0;
                    int tempRate = 10;


                    for (int i = 0; i < actionParamArray.Length; i++)
                    {

                        if (actionParamArray[i] == UNUSE_ACTIONPARAM_ID)
                        {
                            break;
                        }

                        totalCount = totalCount + 1;
                    }


                    randomMix = 0;
                    randomMax = totalCount * tempRate;


                    //--------------------------------
                    // 乱数生成
                    //--------------------------------
                    randomVal = (int)m_RandomActionTable.GetRand((uint)randomMix, (uint)(randomMax + 1));

                    //--------------------------------
                    // 抽選処理
                    //--------------------------------
                    for (int i = 0; i < totalCount; i++)
                    {

                        tempValue = randomVal - ((i + 1) * tempRate);
                        if (tempValue > 0)
                        {
                            continue;
                        }

                        m_ActionStep = i;
                        break;
                    }
                }
                break;


            case MasterDataDefineLabel.EnemyACTSelectType.LOOP:
                {
                    //--------------------------------
                    // 巡回設定
                    //--------------------------------
                    m_ActionStep = m_ActionStep + 1;


                    if (m_ActionStep >= 0
                    && m_ActionStep < actionParamArray.Length)
                    {

                        //--------------------------------
                        // 未設定、最終定義まで到達した場合は、先頭に戻す
                        //--------------------------------
                        if (actionParamArray[m_ActionStep] == UNUSE_ACTIONPARAM_ID)
                        {
                            m_ActionStep = 0;
                        }

                    }
                    else
                    {

                        //--------------------------------
                        //	範囲外に到達したら先頭へ
                        //--------------------------------
                        m_ActionStep = 0;

                    }
                }
                break;

            default:
                break;
        }

    }

    /// <summary>
    /// 現在の条件を満たした敵行動テーブルを検索
    /// </summary>
    /// <param name="battle_total_turn"></param>
    /// <returns></returns>
    private MasterDataEnemyActionTable searchCurrentMasterDataEnemyActionTable(int battle_total_turn)
    {
        if (m_BattleEnemy == null)
        {
            return null;
        }

        MasterDataParamEnemy enemy_param = m_BattleEnemy.getMasterDataParamEnemy();
        if (enemy_param == null)
        {
            return null;
        }

        List<MasterDataEnemyActionTable> enemy_action_tables = new List<MasterDataEnemyActionTable>();

        {
            int[] table_id_array =
            {
                enemy_param.act_table1,
                enemy_param.act_table2,
                enemy_param.act_table3,
                enemy_param.act_table4,
                enemy_param.act_table5,
                enemy_param.act_table6,
                enemy_param.act_table7,
                enemy_param.act_table8
            };

            for (int idx = 0; idx < table_id_array.Length; idx++)
            {
                MasterDataEnemyActionTable enemy_action_table = BattleParam.m_MasterDataCache.useEnemyActionTable((uint)table_id_array[idx]);
                if (enemy_action_table != null)
                {
                    enemy_action_tables.Add(enemy_action_table);
                }
            }

            // 優先度、降順にソート
            enemy_action_tables.Sort((a, b) => b.timing_priority - a.timing_priority);
        }

        MasterDataEnemyActionTable ret_val = null;
        for (int priority_idx = 0; priority_idx < enemy_action_tables.Count; priority_idx++)
        {
            MasterDataEnemyActionTable enemy_action_table = enemy_action_tables[priority_idx];

            bool is_cond = true;
            for (int cond_idx = 0; cond_idx < enemy_action_table.Get_timing_type_count(); cond_idx++)
            {
                MasterDataDefineLabel.EnemyACTPatternSelectType timing_type = enemy_action_table.Get_timing_type(cond_idx);
                int timing_type_param = enemy_action_table.Get_timing_type_param(cond_idx);

                switch (timing_type)
                {
                    case MasterDataDefineLabel.EnemyACTPatternSelectType.HP:
                        if (checkTimingHPRate(timing_type_param) == false)
                        {
                            is_cond = false;
                        }
                        break;

                    case MasterDataDefineLabel.EnemyACTPatternSelectType.HP_UNDER:
                        if (checkTimingHPRateUnder(timing_type_param) == false)
                        {
                            is_cond = false;
                        }
                        break;

                    case MasterDataDefineLabel.EnemyACTPatternSelectType.TURN:
                        if (checkTimingTotalTurn(battle_total_turn, timing_type_param) == false)
                        {
                            is_cond = false;
                        }
                        break;
                }

                if (is_cond == false)
                {
                    break;
                }
            }

            if (is_cond)
            {
                ret_val = enemy_action_table;
                break;
            }
        }

        return ret_val;
    }

} // class ActionTableControl
