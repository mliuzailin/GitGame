/*==========================================================================*/
/*==========================================================================*/
/*!
    @file	SceneGoesParamToQuest2Build.cs
    @brief	シーン間のパラメータ受け渡しクラス：ゲームメインへの受け渡し用：サーバー側で構築したフロア構成情報
    @author Developer
    @date 	2014/03/14
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using ServerDataDefine;

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
    @brief	受け渡し情報：メインメニュー→ゲームメイン
    @note	サーバー側で構築したフロア構成情報
*/
//----------------------------------------------------------------------------
public class SceneGoesParamToQuest2Build
{
    public PacketStructQuest2Build m_QuestBuild = null;      //!< サーバー側で求めたクエスト構成情報


    //---------------------------------------------------------------------
    /*!
        @brief		敵マスターデータの取得
        @param[in]	uint		fix_id
    */
    //---------------------------------------------------------------------
    public MasterDataParamEnemy GetEnemyParamFromID(uint unID)
    {
        MasterDataParamEnemy[] enemyparam_list = m_QuestBuild.list_e_param;
        if (unID == 0)
        {
            return null;
        }

        if (enemyparam_list == null)
        {
            return null;
        }


        for (int i = 0; i < enemyparam_list.Length; i++)
        {
            if (enemyparam_list[i] == null)
            {
                continue;
            }

            if (enemyparam_list[i].fix_id != unID)
            {
                continue;
            }

            return enemyparam_list[i];
        }

        return null;
    }

    //---------------------------------------------------------------------
    /*!
        @brief		敵行動テーブル取得
        @param[in]	uint		fix_id
    */
    //---------------------------------------------------------------------
    public MasterDataEnemyActionTable GetEnemyActionTableFromID(int unID)
    {
        if (unID == 0)
        {
            return null;
        }

        MasterDataEnemyActionTable[] enemy_action_table = m_QuestBuild.list_e_acttable;
        if (enemy_action_table == null)
        {
            return null;
        }

        for (int i = 0; i < enemy_action_table.Length; i++)
        {
            if (enemy_action_table[i] == null)
            {
                continue;
            }

            if (enemy_action_table[i].fix_id != unID)
            {
                continue;
            }

            return enemy_action_table[i];
        }

        return null;
    }


    //---------------------------------------------------------------------
    /*!
        @brief		敵行動パラメータ取得
        @param[in]	uint		fix_id
    */
    //---------------------------------------------------------------------
    public MasterDataEnemyActionParam GetEnemyActionParamFromID(int unID)
    {
        if (unID == 0)
        {
            return null;
        }

        MasterDataEnemyActionParam[] enemy_action_param = m_QuestBuild.list_e_actparam;
        if (enemy_action_param == null)
        {
            return null;
        }

        for (int i = 0; i < enemy_action_param.Length; i++)
        {
            if (enemy_action_param[i] == null)
            {
                continue;
            }

            if (enemy_action_param[i].fix_id != unID)
            {
                continue;
            }

            return enemy_action_param[i];
        }


        return null;
    }


};


