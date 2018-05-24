/*==========================================================================*/
/*==========================================================================*/
/*!
    @file	EnemyAbility.cs
    @brief	敵特性クラス
    @author Developer
    @date 	2015/05/20
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
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
    @class	EnemyAbility
    @brief	敵特性クラス
*/
//----------------------------------------------------------------------------
static public class EnemyAbility
{
    public const int ENEMY_ABILITY_DEFAULT_MAX = (8);   // 特性最大数（デフォルト）
    public const int ENEMY_ABILITY_ADD_MAX = (8);       // 特性最大数（追加分）
    public const int ENEMY_ABILITY_MAX = ENEMY_ABILITY_DEFAULT_MAX + ENEMY_ABILITY_ADD_MAX; // 特性最大数

    public const int BIT_SKILL_NORMAL = 7;          //!< ビットスキル：ノーマル
    public const int BIT_SKILL_LEADER = 6;          //!< ビットスキル：リーダー
    public const int BIT_SKILL_PASSIVE = 5;         //!< ビットスキル：パッシブ
    public const int BIT_SKILL_LIMITBREAK = 4;          //!< ビットスキル：リンクパッシブ
    public const int BIT_SKILL_BOOST = 3;           //!< ビットスキル：ブースト
    public const int BIT_SKILL_LINK = 2;            //!< ビットスキル：リンク
    public const int BIT_SKILL_LINKPASSIVE = 1;         //!< ビットスキル：リンクパッシブ
    public const int BIT_SKILL_MAX = 8;         //!< ビットスキル：

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /// <summary>
    /// 敵リアクション情報
    /// </summary>
    public struct ReactionInfo
    {
        public MasterDataEnemyActionParam m_EnemyReactParam;    // 敵行動
        public int m_EnemyPartyIndex;   // 発動者番号
    }
    private static ReactionInfo[] m_ReactionInfos = null;
    private static int m_ReactionInfoCount = 0;


    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    static public int getReactionCount()
    {
        return m_ReactionInfoCount;
    }
    static public ReactionInfo getReaction(int index)
    {
        return m_ReactionInfos[index];
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		特性によるダメージ補正を取得
        @param[in]	BattleSkillActivity	(skillActivity)	発動スキル
        @param[in]	BattleEnemy			(enemy)			スキル対象の敵
        @return		float				(rate)			[ダメージ倍率]
        @note		HPとATKの+%値
    */
    //----------------------------------------------------------------------------
    static public float GetAbilityDamageRateEnemy(BattleEnemy enemy)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (enemy == null)
        {
            return (rate);
        }

        MasterDataEnemyAbility abilityMaster = null;
        float categoryRate = 1.0f;

        // 特性を配列化
        uint[] enemyAbility = enemy.getEnemyAbilitys();

        //--------------------------------
        // 所持特性を全チェック
        //--------------------------------
        for (int num = 0; num < enemyAbility.Length; ++num)
        {
            if (enemyAbility[num] == 0)
            {
                continue;
            }

            // 特性マスターを取得
            abilityMaster = BattleParam.m_MasterDataCache.useEnemyAbility(enemyAbility[num]);
            if (abilityMaster == null)
            {
                continue;
            }

            // 効果カテゴリによる分岐
            switch (abilityMaster.category)
            {
                case MasterDataDefineLabel.EnemyAbilityType.DMG_AILMENT:
                    categoryRate = GetAbilityAilmentDamageRateEnemy(abilityMaster, enemy.m_StatusAilmentChara);
                    break;

                case MasterDataDefineLabel.EnemyAbilityType.DMG_CRITICAL:
                case MasterDataDefineLabel.EnemyAbilityType.DMG_NORMAL:
                case MasterDataDefineLabel.EnemyAbilityType.DMG_ACTIVE:
                case MasterDataDefineLabel.EnemyAbilityType.DMG_BOOST:
                case MasterDataDefineLabel.EnemyAbilityType.DMG_COST:
                case MasterDataDefineLabel.EnemyAbilityType.DMG_COLOR:
                case MasterDataDefineLabel.EnemyAbilityType.DMG_HANDS:
                case MasterDataDefineLabel.EnemyAbilityType.DMG_RATE:
                case MasterDataDefineLabel.EnemyAbilityType.NONE:
                case MasterDataDefineLabel.EnemyAbilityType.MAX:
                default:
                    categoryRate = 1.0f;
                    break;
            }

            // 特性補正(カテゴリ補正を掛け合わせて行く)
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, categoryRate);
        }

        //--------------------------------
        // 下限チェック
        //--------------------------------
        if (rate < 0.0f)
        {
            rate = 0.0f;
        }

        return (rate);
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		特性によるダメージ補正を取得(プレイヤー)
        @param[in]	BattleSkillActivity	(skillActivity)	発動スキル
        @param[in]	BattleEnemy			(enemy)			スキル対象の敵
        @return		float				(rate)			[ダメージ倍率]
        @note		HPとATKの+%値
    */
    //----------------------------------------------------------------------------
    static public float GetAbilityDamageRatePlayer(BattleSkillActivity skillActivity, BattleEnemy enemy)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (enemy == null)
        {
            return (rate);
        }

        MasterDataEnemyAbility abilityMaster = null;
        float categoryRate = 1.0f;

        // 特性を配列化
        uint[] enemyAbility = enemy.getEnemyAbilitys();

        //--------------------------------
        // 所持特性を全チェック
        //--------------------------------
        for (int num = 0; num < enemyAbility.Length; ++num)
        {
            if (enemyAbility[num] == 0)
            {
                continue;
            }

            // 特性マスターを取得
            abilityMaster = BattleParam.m_MasterDataCache.useEnemyAbility(enemyAbility[num]);
            if (abilityMaster == null)
            {
                continue;
            }

            // 効果カテゴリによる分岐
            switch (abilityMaster.category)
            {
                case MasterDataDefineLabel.EnemyAbilityType.DMG_AILMENT:
                    categoryRate = GetAbilityAilmentDamageRatePlayer(abilityMaster, enemy.m_StatusAilmentChara, skillActivity);
                    break;
                case MasterDataDefineLabel.EnemyAbilityType.DMG_CRITICAL:
                    categoryRate = GetAbilityCriticalDamageRate(abilityMaster, skillActivity);
                    break;
                case MasterDataDefineLabel.EnemyAbilityType.DMG_NORMAL:
                    categoryRate = GetAbilityNormalDamageRate(abilityMaster, skillActivity);
                    break;
                case MasterDataDefineLabel.EnemyAbilityType.DMG_ACTIVE:
                    categoryRate = GetAbilityActiveDamageRate(abilityMaster, skillActivity);
                    break;
                case MasterDataDefineLabel.EnemyAbilityType.DMG_BOOST:
                    categoryRate = GetAbilityBoostDamageRate(abilityMaster, skillActivity);
                    break;
                case MasterDataDefineLabel.EnemyAbilityType.DMG_COST:
                    categoryRate = GetAbilityCostDamageRate(abilityMaster, skillActivity);
                    break;
                case MasterDataDefineLabel.EnemyAbilityType.DMG_COLOR:
                    categoryRate = GetAbilityColorDamageRate(abilityMaster, skillActivity);
                    break;
                case MasterDataDefineLabel.EnemyAbilityType.DMG_HANDS:
                    categoryRate = GetAbilityHandsDamageRate(abilityMaster, skillActivity);
                    break;
                case MasterDataDefineLabel.EnemyAbilityType.DMG_RATE:
                    categoryRate = GetAbilityRateDamageRate(abilityMaster, skillActivity);
                    break;
                case MasterDataDefineLabel.EnemyAbilityType.DMG_SKILL:
                    //v390 Developer add 効果カテゴリ追加：ダメージ補正スキル指定
                    categoryRate = GetAbilityTargetSkillDamageRate(abilityMaster, skillActivity);
                    break;

                case MasterDataDefineLabel.EnemyAbilityType.NONE:
                case MasterDataDefineLabel.EnemyAbilityType.MAX:
                default:
                    categoryRate = 1.0f;
                    break;
            }

            // 特性補正(カテゴリ補正を掛け合わせて行く)
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, categoryRate);
        }

        //--------------------------------
        // 下限チェック
        //--------------------------------
        if (rate < 0.0f)
        {
            rate = 0.0f;
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		状態異常有無によるダメージ補正
        @param[in]	int[]	(paramList)		パラメータリスト
        @param[in]	int		(ailmentID)		状態異常管理ID
        @return		float	(rate)			[ダメージ倍率]
        @note
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityAilmentDamageRate(MasterDataEnemyAbility master_data_enemy_ability, StatusAilmentChara ailmentID)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (ailmentID == null)
        {
            return (rate);
        }


        StatusAilmentChara ailmentCharaEnemy = null;
        StatusAilmentChara ailmentCharaPlayer = null;
        bool ailmentOn = false;
        bool ailmentOff = true;

        //--------------------------------
        // 判定対象による分岐
        //--------------------------------
        switch (master_data_enemy_ability.Get_DMG_AILMENT_TARGET())
        {
            // エネミー
            case MasterDataDefineLabel.TargetType.SELF:     // 自分
            case MasterDataDefineLabel.TargetType.FRIEND:   // 使用者の味方全員
                if (ailmentID.GetOwnerType() == StatusAilmentChara.OwnerType.ENEMY)
                {
                    // 状態異常管理を取得
                    ailmentCharaEnemy = ailmentID;
                }
                break;

            // プレイヤー
            case MasterDataDefineLabel.TargetType.OTHER:    // 相手
            case MasterDataDefineLabel.TargetType.ENEMY:    // 使用者の敵全員
            case MasterDataDefineLabel.TargetType.ENE_N_1:
            case MasterDataDefineLabel.TargetType.ENE_1N_1:
            case MasterDataDefineLabel.TargetType.ENE_R_N:
            case MasterDataDefineLabel.TargetType.ENE_1_N:
                {
                    // 状態異常管理を取得
                    ailmentCharaPlayer = BattleParam.m_PlayerParty.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX);
                }
                break;

            // どちらかに該当の状態異常がある場合
            case MasterDataDefineLabel.TargetType.ALL:      // 全員
                                                            // エネミーの状態異常管理を取得
                if (ailmentID.GetOwnerType() == StatusAilmentChara.OwnerType.ENEMY)
                {
                    ailmentCharaEnemy = ailmentID;
                }
                // プレイヤーの状態異常管理を取得
                {
                    ailmentCharaPlayer = BattleParam.m_PlayerParty.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX);
                }
                break;

            case MasterDataDefineLabel.TargetType.NONE:
            default:
                break;
        }

        //--------------------------------
        // 指定した状態異常の検索(エネミー)
        //--------------------------------
        if (ailmentCharaEnemy != null)
        {
            MasterDataDefineLabel.AilmentGroup groupNum = MasterDataDefineLabel.AilmentGroup.NONE;

            for (int num = 0; num < (int)MasterDataDefineLabel.AilmentType.MAX; ++num)
            {
                if (ailmentCharaEnemy.IsHavingAilment((MasterDataDefineLabel.AilmentType)num) == false)
                {
                    continue;
                }

                // 状態異常グループを取得
                groupNum = StatusAilment.getAilmentGroupFromAilmentType((MasterDataDefineLabel.AilmentType)num);

                // どれか1つでも該当する状態がある場合
                if (groupNum == master_data_enemy_ability.Get_DMG_AILMENT_TYPE1()
                || groupNum == master_data_enemy_ability.Get_DMG_AILMENT_TYPE2()
                || groupNum == master_data_enemy_ability.Get_DMG_AILMENT_TYPE3()
                || groupNum == master_data_enemy_ability.Get_DMG_AILMENT_TYPE4())
                {
                    ailmentOn = true;
                    ailmentOff = false;
                    break;
                }

                // 何か状態異常がかかっているので、フラグOFF
                ailmentOff = false;
            }

        }
        //--------------------------------
        // 指定した状態異常の検索(プレイヤー)
        //--------------------------------
        if (ailmentCharaPlayer != null)
        {
            MasterDataDefineLabel.AilmentGroup groupNum = MasterDataDefineLabel.AilmentGroup.NONE;

            for (int num = 0; num < (int)MasterDataDefineLabel.AilmentType.MAX; ++num)
            {
                if (ailmentCharaPlayer.IsHavingAilment((MasterDataDefineLabel.AilmentType)num) == false)
                {
                    continue;
                }

                // 状態異常グループを取得
                groupNum = StatusAilment.getAilmentGroupFromAilmentType((MasterDataDefineLabel.AilmentType)num);

                // どれか1つでも該当する状態がある場合
                if (groupNum == master_data_enemy_ability.Get_DMG_AILMENT_TYPE1()
                || groupNum == master_data_enemy_ability.Get_DMG_AILMENT_TYPE2()
                || groupNum == master_data_enemy_ability.Get_DMG_AILMENT_TYPE3()
                || groupNum == master_data_enemy_ability.Get_DMG_AILMENT_TYPE4())
                {
                    ailmentOn = true;
                    ailmentOff = false;
                    break;
                }

                // 何か状態異常がかかっているので、フラグOFF
                ailmentOff = false;
            }

        }

        //--------------------------------
        // 判定：指定した状態異常が付与されている場合
        //--------------------------------
        if (ailmentOn == true)
        {
            float ailmentOnRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_AILMENT_ON_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, ailmentOnRate);
        }
        //--------------------------------
        // 判定：状態異常が何も付与されていない場合
        //--------------------------------
        else if (ailmentOff == true)
        {
            float ailmentOffRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_AILMENT_OFF_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, ailmentOffRate);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		状態異常有無によるダメージ補正(エネミー)
        @param[in]	int[]	(paramList)		パラメータリスト
        @param[in]	int		(ailmentID)		状態異常管理ID
        @return		float	(rate)			[ダメージ倍率]
        @note
    */
    //----------------------------------------------------------------------------
    static public float GetAbilityAilmentDamageRateEnemy(MasterDataEnemyAbility master_data_enemy_ability, StatusAilmentChara ailmentID)
    {
        float rate = 1.0f;

        // 補正対象による分岐
        switch (master_data_enemy_ability.Get_DMG_AILMENT_SUBJECT())
        {
            // エネミー
            case MasterDataDefineLabel.TargetType.SELF:     // 自分
            case MasterDataDefineLabel.TargetType.FRIEND:   // 使用者の味方全員
            case MasterDataDefineLabel.TargetType.ALL:      // 全員
                rate = GetAbilityAilmentDamageRate(master_data_enemy_ability, ailmentID);
                break;

            // プレイヤー or 未設定の場合処理しない
            case MasterDataDefineLabel.TargetType.OTHER:    // 相手
            case MasterDataDefineLabel.TargetType.ENEMY:    // 使用者の敵全員
            case MasterDataDefineLabel.TargetType.ENE_N_1:
            case MasterDataDefineLabel.TargetType.ENE_1N_1:
            case MasterDataDefineLabel.TargetType.ENE_R_N:
            case MasterDataDefineLabel.TargetType.ENE_1_N:
            case MasterDataDefineLabel.TargetType.NONE:
            default:
                break;
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		状態異常有無によるダメージ補正(プレイヤー)
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	int					(ailmentID)		状態異常管理ID
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityAilmentDamageRatePlayer(MasterDataEnemyAbility master_data_enemy_ability, StatusAilmentChara ailmentID, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eACTIVE)
        {
            return (rate);
        }

        // 補正対象による分岐
        switch (master_data_enemy_ability.Get_DMG_AILMENT_SUBJECT())
        {
            // プレイヤー
            case MasterDataDefineLabel.TargetType.OTHER:    // 相手
            case MasterDataDefineLabel.TargetType.ENEMY:    // 使用者の敵全員
            case MasterDataDefineLabel.TargetType.ENE_N_1:
            case MasterDataDefineLabel.TargetType.ENE_1N_1:
            case MasterDataDefineLabel.TargetType.ENE_R_N:
            case MasterDataDefineLabel.TargetType.ENE_1_N:
            case MasterDataDefineLabel.TargetType.ALL:      // 全員
                rate = GetAbilityAilmentDamageRate(master_data_enemy_ability, ailmentID);
                break;

            // エネミー or 未設定の場合処理しない
            case MasterDataDefineLabel.TargetType.SELF:     // 自分
            case MasterDataDefineLabel.TargetType.FRIEND:   // 使用者の味方全員
            case MasterDataDefineLabel.TargetType.NONE:
            default:
                break;
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		クリティカル有無によるダメージ補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityCriticalDamageRate(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        // ノーマルとリンクスキル以外はリターン
        //--------------------------------
        if (skillActivity == null
        || (skillActivity.m_SkillType != ESKILLTYPE.eACTIVE
            && skillActivity.m_SkillType != ESKILLTYPE.eLINK)
        )
        {
            return (rate);
        }

        //--------------------------------
        // 補正対象スキルチェック
        // 今後参照スキルが増えてきたらCheckTargetSkillTypeを使ってもいいかもね
        // v400 Developer add
        //--------------------------------
        if (skillActivity.m_SkillType == ESKILLTYPE.eACTIVE
        && master_data_enemy_ability.Get_DMG_CRITICAL_NORMAL() == false)
        {
            return (rate);
        }

        if (skillActivity.m_SkillType == ESKILLTYPE.eLINK
        && master_data_enemy_ability.Get_DMG_CRITICAL_LINK() == false)
        {
            return (rate);
        }

        //--------------------------------
        // 判定：クリティカルの場合
        //--------------------------------
        if (skillActivity.m_bCritical == true)
        {
            float criticalOnRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_CRITICAL_ON_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, criticalOnRate);
        }
        else
        {
            float criticalOffRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_CRITICAL_OFF_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, criticalOffRate);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		ノーマルスキルのダメージ補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityNormalDamageRate(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eACTIVE)
        {
            return (rate);
        }

        //--------------------------------
        // タイプチェック(単体 or 全体)
        // @change Developer 2016/03/31 v330 ノーマル特性対応
        //--------------------------------
        MasterDataDefineLabel.SkillType normalType = skillActivity.m_Type;
        if (normalType != MasterDataDefineLabel.SkillType.ATK_ONCE
        && normalType != MasterDataDefineLabel.SkillType.ATK_ALL)
        {
            return (rate);
        }

        //--------------------------------
        // 判定：該当タイプの場合
        //--------------------------------
        if (normalType == master_data_enemy_ability.Get_DMG_NORMAL_TYPE())
        {
            float normalRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_NORMAL_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, normalRate);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		アクティブスキルのダメージ補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityActiveDamageRate(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eLIMITBREAK)
        {
            return (rate);
        }

        //--------------------------------
        // アクティブスキルマスターを取得
        //--------------------------------
        MasterDataSkillLimitBreak activeMaster = BattleParam.m_MasterDataCache.useSkillLimitBreak(skillActivity.m_SkillParamSkillID);
        if (activeMaster == null
        || activeMaster.Is_skill_damage_enable() == false)
        {
            return (rate);
        }


        bool chkElemental = false;      // 判定用：属性
        bool chkType = false;       // 判定用：タイプ
        bool chkCategory = false;       // 判定用：カテゴリ
        bool chkAtkRate = false;        // 判定用：攻撃力倍率
        bool chkAtkFix = false;     // 判定用：固定値
        bool chkAtkHp = false;      // 判定用：対象HP割合
        bool chkAbsorb = false;     // 判定用：吸血
        bool chkAilment = false;        // 判定用：状態異常


        //--------------------------------
        // 判定条件を取得
        //--------------------------------
        MasterDataDefineLabel.ElementType abiElement = master_data_enemy_ability.Get_DMG_ACTIVE_ELEMENT();
        MasterDataDefineLabel.SkillType abiType = master_data_enemy_ability.Get_DMG_ACTIVE_TYPE();
        MasterDataDefineLabel.SkillCategory abiCategory = master_data_enemy_ability.Get_DMG_ACTIVE_CATEGORY();
        MasterDataDefineLabel.AilmentGroup abiAilment = master_data_enemy_ability.Get_DMG_ACTIVE_AILMENT();

        //--------------------------------
        // 判定：属性
        //--------------------------------
        if (abiElement > MasterDataDefineLabel.ElementType.NONE
        && abiElement < MasterDataDefineLabel.ElementType.MAX)
        {
            if (activeMaster.skill_elem == abiElement)
            {
                chkElemental = true;
            }
            else
            {
                chkElemental = false;
            }
        }
        else
        {
            chkElemental = false;
        }

        //--------------------------------
        // 判定：タイプ
        //--------------------------------
        if (abiType > MasterDataDefineLabel.SkillType.NONE
        && abiType < MasterDataDefineLabel.SkillType.MAX)
        {
            if (activeMaster.skill_type == abiType)
            {
                chkType = true;
            }
            else
            {
                chkType = false;
            }
        }
        else
        {
            chkType = false;
        }

        //--------------------------------
        // 判定：カテゴリ
        //--------------------------------
        if (abiCategory > MasterDataDefineLabel.SkillCategory.NONE
        && abiCategory < MasterDataDefineLabel.SkillCategory.MAX)
        {
            if (activeMaster.skill_cate == abiCategory)
            {
                chkCategory = true;
            }
            else
            {
                chkCategory = false;
            }
        }
        else
        {
            chkCategory = false;
        }

        //--------------------------------
        // 判定：攻撃力倍率
        //--------------------------------
        if (master_data_enemy_ability.Get_SPECIFIC_ACTION_ATK_RATE())
        {
            if (activeMaster.skill_power > 0)
            {
                chkAtkRate = true;
            }
            else
            {
                chkAtkRate = false;
            }
        }
        else
        {
            chkAtkRate = false;
        }

        //--------------------------------
        // 判定：固定値
        //--------------------------------
        if (master_data_enemy_ability.Get_SPECIFIC_ACTION_ATK_FIX())
        {
            if (activeMaster.skill_power_fix > 0)
            {
                chkAtkFix = true;
            }
            else
            {
                chkAtkFix = false;
            }
        }
        else
        {
            chkAtkFix = false;
        }

        //--------------------------------
        // 判定：対象HP割合
        //--------------------------------
        if (master_data_enemy_ability.Get_SPECIFIC_ACTION_ATK_HP())
        {
            if (activeMaster.skill_power_hp_rate > 0)
            {
                chkAtkHp = true;
            }
            else
            {
                chkAtkHp = false;
            }
        }
        else
        {
            chkAtkHp = false;
        }

        //--------------------------------
        // 判定：吸血
        //--------------------------------
        if (master_data_enemy_ability.Get_SPECIFIC_ACTION_ABSORB())
        {
            if (activeMaster.skill_absorb > 0)
            {
                chkAbsorb = true;
            }
            else
            {
                chkAbsorb = false;
            }
        }
        else
        {
            chkAbsorb = false;
        }

        //--------------------------------
        // 判定：状態異常
        //--------------------------------
        if (abiAilment > MasterDataDefineLabel.AilmentGroup.NONE
        && abiAilment < MasterDataDefineLabel.AilmentGroup.MAX)
        {
            // 状態異常の種類を取得
            MasterDataDefineLabel.AilmentGroup groupNum1 = StatusAilment.getAilmentGroupFromAilmentID(activeMaster.status_ailment1);
            MasterDataDefineLabel.AilmentGroup groupNum2 = StatusAilment.getAilmentGroupFromAilmentID(activeMaster.status_ailment2);
            MasterDataDefineLabel.AilmentGroup groupNum3 = StatusAilment.getAilmentGroupFromAilmentID(activeMaster.status_ailment3);
            MasterDataDefineLabel.AilmentGroup groupNum4 = StatusAilment.getAilmentGroupFromAilmentID(activeMaster.status_ailment4);

            // 該当の状態異常が設定されている場合
            if (groupNum1 == abiAilment
            || groupNum2 == abiAilment
            || groupNum3 == abiAilment
            || groupNum4 == abiAilment)
            {
                chkAilment = true;
            }
            else
            {
                chkAilment = false;
            }
        }
        else
        {
            chkAilment = false;
        }


        //--------------------------------
        // 判定：設定した条件の何れかが該当する場合
        //--------------------------------
        if (chkElemental == true || chkType == true || chkCategory == true || chkAilment == true
        || chkAtkRate == true || chkAtkFix == true || chkAtkHp == true || chkAbsorb == true)
        {
            float damageRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_ACTIVE_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, damageRate);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		ブーストスキルのダメージ補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityBoostDamageRate(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eBOOST)
        {
            return (rate);
        }

        //--------------------------------
        // ブーストスキルマスターを取得
        //--------------------------------
        MasterDataSkillBoost boostMaster = BattleParam.m_MasterDataCache.useSkillBoost(skillActivity.m_SkillParamSkillID);
        if (boostMaster == null)
        {
            return (rate);
        }

        //--------------------------------
        // 判定：ダメージ系の場合
        //--------------------------------
        if (boostMaster.Is_skill_damage_enable())
        {
            float damageRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_BOOST_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, damageRate);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定コストによるダメージ補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityCostDamageRate(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eACTIVE)
        {
            return (rate);
        }


        //--------------------------------
        // タイプチェック(単体 or 全体)
        // bugweb:8372 2016/12/26 Developer 参照スキル情報修正
        //--------------------------------
        MasterDataDefineLabel.SkillType normalType = skillActivity.m_Type;
        if (normalType != MasterDataDefineLabel.SkillType.ATK_ONCE
        && normalType != MasterDataDefineLabel.SkillType.ATK_ALL)
        {
            return (rate);
        }

        //--------------------------------
        // ノーマルスキルマスターを取得
        //--------------------------------
        MasterDataSkillActive normalMaster = BattleParam.m_MasterDataCache.useSkillActive(skillActivity.m_SkillParamSkillID);
        if (normalMaster == null)
        {
            return (rate);
        }

        // 必要スキルコストを配列化
        MasterDataDefineLabel.ElementType[] skillCost = {
                            normalMaster.cost1,
                            normalMaster.cost2,
                            normalMaster.cost3,
                            normalMaster.cost4,
                            normalMaster.cost5,
                          };

        //--------------------------------
        // コスト数をカウント
        //--------------------------------
        int skillCostNum = 0;
        for (int num = 0; num < skillCost.Length; ++num)
        {
            // 無、炎、水、光、闇、風、回復以外は、カウントしない
            if (skillCost[num] < MasterDataDefineLabel.ElementType.NAUGHT
            || skillCost[num] > MasterDataDefineLabel.ElementType.HEAL)
            {
                continue;
            }

            ++skillCostNum;
        }

        //--------------------------------
        // 判定：指定コスト数以上の場合
        //--------------------------------
        if (skillCostNum >= master_data_enemy_ability.Get_DMG_COST_NUM())
        {
            float costOverRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_COST_OVER_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, costOverRate);
        }
        else
        {
            float costLessRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_COST_LESS_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, costLessRate);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定色によるダメージ補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityColorDamageRate(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        BattleSceneManager battleMgr = BattleSceneManager.Instance;
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (battleMgr == null
        || skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eACTIVE)
        {
            return (rate);
        }


        //--------------------------------
        // 属性色をカウント
        //--------------------------------
        int elemColorNum = battleMgr.getActiveSkillElementCount();

        //--------------------------------
        // 判定：指定色数以上の場合
        //--------------------------------
        if (elemColorNum >= master_data_enemy_ability.Get_DMG_COLOR_NUM())
        {
            float colorOverRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_COLOR_OVER_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, colorOverRate);
        }
        else
        {
            float colorLessRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_COLOR_LESS_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, colorLessRate);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定HANDSによるダメージ補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityHandsDamageRate(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        BattleSceneManager battleMgr = BattleSceneManager.Instance;
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (battleMgr == null
        || skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eACTIVE)
        {
            return (rate);
        }

        //--------------------------------
        // HANDS数を取得
        //--------------------------------
        int handsNum = battleMgr.getActiveSkillComboCount();

        //--------------------------------
        // 判定：指定HANDS数以上の場合
        //--------------------------------
        if (handsNum >= master_data_enemy_ability.Get_DMG_HANDS_NUM())
        {
            float handsOverRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_HANDS_OVER_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, handsOverRate);
        }
        else
        {
            float handsLessRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_HANDS_LESS_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, handsLessRate);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定Rateによるダメージ補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityRateDamageRate(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        BattleSceneManager battleMgr = BattleSceneManager.Instance;
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (battleMgr == null
        || skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eACTIVE)
        {
            return (rate);
        }

        //--------------------------------
        // Rate値を取得
        // ※floatだと正確に判定できないので、intに直す
        //--------------------------------
        int rateValue = (int)(InGameUtilBattle.GetSkillCountRate(battleMgr.getActiveSkillComboCount()) * 100.0f);

        //--------------------------------
        // 判定：指定Rate値以上の場合
        //--------------------------------
        int abilitChkRate = master_data_enemy_ability.Get_DMG_RATE_VALUE();
        if (rateValue >= abilitChkRate)
        {
            float rateOverRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_RATE_OVER_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, rateOverRate);
        }
        else
        {
            float rateLessRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_RATE_LESS_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, rateLessRate);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		敵リアクション情報初期化
        @note
    */
    //----------------------------------------------------------------------------
    static public void InitReaction()
    {
        //--------------------------------
        // 初期化
        //--------------------------------
        m_ReactionInfos = null;
        m_ReactionInfoCount = 0;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		特定アクションに対する行動
        @param[in]	uint	(skillID)	スキルID
        @return		bool	(result)	[リアクション発動：成/否]
        @note
    */
    //----------------------------------------------------------------------------
    static public bool AbilitySpecificActionReaction(uint skillID, BattleEnemy[] enemy_params)
    {
        bool result = false;

        // 各情報を取得
        MasterDataSkillLimitBreak activeMaster = BattleParam.m_MasterDataCache.useSkillLimitBreak(skillID);
        if (enemy_params == null
        || enemy_params.Length == 0
        )
        {
            return (result);
        }


        //--------------------------------
        // 初期化
        //--------------------------------
        m_ReactionInfos = new ReactionInfo[enemy_params.Length * ENEMY_ABILITY_MAX];
        m_ReactionInfoCount = 0;

        //--------------------------------
        // 戦闘中の敵を全チェック
        //--------------------------------
        for (int enemyNum = 0; enemyNum < enemy_params.Length; ++enemyNum)
        {
            BattleEnemy battle_enemy = enemy_params[enemyNum];

            // 死亡している場合
            if (battle_enemy == null
            || battle_enemy.isDead() == true)
            {
                continue;
            }

            // 特性を配列化
            uint[] enemyAbility = battle_enemy.getEnemyAbilitys();

            bool chkElemental = false;      // 判定用：属性
            bool chkType = false;       // 判定用：タイプ
            bool chkCategory = false;       // 判定用：カテゴリ
            bool chkAtkRate = false;        // 判定用：攻撃力倍率
            bool chkAtkFix = false;     // 判定用：固定値
            bool chkAtkHp = false;      // 判定用：対象HP割合
            bool chkAbsorb = false;     // 判定用：吸血
            bool chkAilment = false;        // 判定用：状態異常

            //--------------------------------
            // 所持特性を全チェック
            //--------------------------------
            for (int abiNum = 0; abiNum < enemyAbility.Length; ++abiNum)
            {
                if (enemyAbility[abiNum] == 0)
                {
                    continue;
                }

                // 特性マスターを取得
                MasterDataEnemyAbility abilityMaster = BattleParam.m_MasterDataCache.useEnemyAbility(enemyAbility[abiNum]);

                //	該当スキルかどうかチェック
                if (abilityMaster == null
                || abilityMaster.category != MasterDataDefineLabel.EnemyAbilityType.SPECIFIC_ACTION)
                {
                    continue;
                }


                // 判定条件を取得
                MasterDataDefineLabel.ElementType abiElement = abilityMaster.Get_SPECIFIC_ACTION_ELEMENT();
                MasterDataDefineLabel.SkillType abiType = abilityMaster.Get_SPECIFIC_ACTION_TYPE();
                MasterDataDefineLabel.SkillCategory abiCategory = abilityMaster.Get_SPECIFIC_ACTION_CATEGORY();
                MasterDataDefineLabel.AilmentGroup abiAilment = abilityMaster.Get_SPECIFIC_ACTION_AILMENT();

                //--------------------------------
                // 判定：属性
                //--------------------------------
                if (abiElement > MasterDataDefineLabel.ElementType.NONE
                && abiElement < MasterDataDefineLabel.ElementType.MAX)
                {
                    if (activeMaster.skill_elem == abiElement)
                    {
                        chkElemental = true;
                    }
                    else
                    {
                        chkElemental = false;
                    }
                }
                else
                {
                    chkElemental = false;
                }

                //--------------------------------
                // 判定：タイプ
                //--------------------------------
                if (abiType > MasterDataDefineLabel.SkillType.NONE
                && abiType < MasterDataDefineLabel.SkillType.MAX)
                {
                    if (activeMaster.skill_type == abiType)
                    {
                        chkType = true;
                    }
                    else
                    {
                        chkType = false;
                    }
                }
                else
                {
                    chkType = false;
                }

                //--------------------------------
                // 判定：カテゴリ
                //--------------------------------
                if (abiCategory > MasterDataDefineLabel.SkillCategory.NONE
                && abiCategory < MasterDataDefineLabel.SkillCategory.MAX)
                {
                    if (activeMaster.skill_cate == abiCategory)
                    {
                        chkCategory = true;
                    }
                    else
                    {
                        chkCategory = false;
                    }
                }
                else
                {
                    chkCategory = false;
                }

                //--------------------------------
                // 判定：攻撃力倍率
                //--------------------------------
                if (abilityMaster.Get_SPECIFIC_ACTION_ATK_RATE())
                {
                    if (activeMaster.skill_power > 0)
                    {
                        chkAtkRate = true;
                    }
                    else
                    {
                        chkAtkRate = false;
                    }
                }
                else
                {
                    chkAtkRate = false;
                }

                //--------------------------------
                // 判定：固定値
                //--------------------------------
                if (abilityMaster.Get_SPECIFIC_ACTION_ATK_FIX())
                {
                    if (activeMaster.skill_power_fix > 0)
                    {
                        chkAtkFix = true;
                    }
                    else
                    {
                        chkAtkFix = false;
                    }
                }
                else
                {
                    chkAtkFix = false;
                }

                //--------------------------------
                // 判定：対象HP割合
                //--------------------------------
                if (abilityMaster.Get_SPECIFIC_ACTION_ATK_HP())
                {
                    if (activeMaster.skill_power_hp_rate > 0)
                    {
                        chkAtkHp = true;
                    }
                    else
                    {
                        chkAtkHp = false;
                    }
                }
                else
                {
                    chkAtkHp = false;
                }

                //--------------------------------
                // 判定：吸血
                //--------------------------------
                if (abilityMaster.Get_SPECIFIC_ACTION_ABSORB())
                {
                    if (activeMaster.skill_absorb > 0)
                    {
                        chkAbsorb = true;
                    }
                    else
                    {
                        chkAbsorb = false;
                    }
                }
                else
                {
                    chkAbsorb = false;
                }

                //--------------------------------
                // 判定：状態異常
                //--------------------------------
                if (abiAilment > MasterDataDefineLabel.AilmentGroup.NONE
                && abiAilment < MasterDataDefineLabel.AilmentGroup.MAX)
                {
                    // 状態異常の種類を取得
                    MasterDataDefineLabel.AilmentGroup groupNum1 = StatusAilment.getAilmentGroupFromAilmentID(activeMaster.status_ailment1);
                    MasterDataDefineLabel.AilmentGroup groupNum2 = StatusAilment.getAilmentGroupFromAilmentID(activeMaster.status_ailment2);
                    MasterDataDefineLabel.AilmentGroup groupNum3 = StatusAilment.getAilmentGroupFromAilmentID(activeMaster.status_ailment3);
                    MasterDataDefineLabel.AilmentGroup groupNum4 = StatusAilment.getAilmentGroupFromAilmentID(activeMaster.status_ailment4);

                    // 該当の状態異常が設定されている場合
                    if (groupNum1 == abiAilment
                    || groupNum2 == abiAilment
                    || groupNum3 == abiAilment
                    || groupNum4 == abiAilment)
                    {
                        chkAilment = true;
                    }
                    else
                    {
                        chkAilment = false;
                    }
                }
                else
                {
                    chkAilment = false;
                }

                //--------------------------------
                // 敵リアクション発動：設定した条件の何れかが該当する場合
                //--------------------------------
                if (chkElemental == true || chkType == true || chkCategory == true || chkAilment == true
                || chkAtkRate == true || chkAtkFix == true || chkAtkHp == true || chkAbsorb == true)
                {
                    // 行動IDを取得
                    int actionID = abilityMaster.Get_SPECIFIC_ACTION_INVOKE();
                    if (actionID == 0)
                    {
                        continue;
                    }

                    // 発動エネミーを保存
                    m_ReactionInfos[m_ReactionInfoCount].m_EnemyPartyIndex = enemyNum;

                    // 敵行動マスターを保存
                    m_ReactionInfos[m_ReactionInfoCount].m_EnemyReactParam = BattleParam.m_MasterDataCache.useEnemyActionParam((uint)actionID);

                    m_ReactionInfoCount++;

                    result = true;
                }

            }

        }

        return (result);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		無効化
        @param[in]	MasterDataParamEnemy	(enemyMaster)	エネミーマスター
        @param[in]	int						(type)			無効化したいタイプ
        @return		bool					(result)		[無効化：成否]
        @note
    */
    //----------------------------------------------------------------------------
    static public bool ChkAbilityInvalid(BattleEnemy battle_enemy, MasterDataDefineLabel.SkillCategory type)
    {
        bool result = false;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (battle_enemy == null)
        {
            return (result);
        }


        MasterDataEnemyAbility abilityMaster = null;

        // 特性を配列化
        uint[] enemyAbility = battle_enemy.getEnemyAbilitys();

        //--------------------------------
        // 所持特性を全チェック
        //--------------------------------
        for (int num = 0; num < enemyAbility.Length; ++num)
        {
            if (enemyAbility[num] == 0)
            {
                continue;
            }

            // 特性マスターを取得
            abilityMaster = BattleParam.m_MasterDataCache.useEnemyAbility(enemyAbility[num]);
            if (abilityMaster == null)
            {
                continue;
            }

            //	該当スキルかどうかチェック
            if (abilityMaster.category != MasterDataDefineLabel.EnemyAbilityType.INVALID)
            {
                continue;
            }

            // どれか1つでも該当するタイプがある場合
            if (type == (MasterDataDefineLabel.SkillCategory)abilityMaster.Get_INVALID_TYPE1()
            || type == (MasterDataDefineLabel.SkillCategory)abilityMaster.Get_INVALID_TYPE2()
            || type == (MasterDataDefineLabel.SkillCategory)abilityMaster.Get_INVALID_TYPE3()
            || type == (MasterDataDefineLabel.SkillCategory)abilityMaster.Get_INVALID_TYPE4())
            {
                result = true;
                break;
            }

        }

        return (result);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定スキルのダメージ補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note		v390 Developer add 特性カテゴリ追加対応
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityTargetSkillDamageRate(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null)
        {
            return (rate);
        }

        //--------------------------------
        // 対象スキルがダメージ補正対象かをチェック
        //--------------------------------
        uint unSkillType = master_data_enemy_ability.Get_DMG_SKILL_TARGET();
        bool bChkRet = CheckTargetSkillType(unSkillType, skillActivity);

        //補正対象外なので終了
        if (bChkRet == false)
        {
            return rate;
        }

        //--------------------------------
        // ダメージ補正を反映
        //--------------------------------
        switch (skillActivity.m_SkillType)
        {
            case ESKILLTYPE.eACTIVE:
                rate = GetAbilityTargetSkillNormal(master_data_enemy_ability, skillActivity);
                break;
            case ESKILLTYPE.eLEADER:
                rate = GetAbilityTargetSkillLeader(master_data_enemy_ability, skillActivity);
                break;
            case ESKILLTYPE.ePASSIVE:
                rate = GetAbilityTargetSkillPassive(master_data_enemy_ability, skillActivity);
                break;
            case ESKILLTYPE.eLIMITBREAK:
                rate = GetAbilityTargetSkillActive(master_data_enemy_ability, skillActivity);
                break;
            case ESKILLTYPE.eBOOST:
                rate = GetAbilityTargetSkillBoost(master_data_enemy_ability, skillActivity);
                break;
            case ESKILLTYPE.eLINK:
                rate = GetAbilityTargetSkillLink(master_data_enemy_ability, skillActivity);
                break;
            case ESKILLTYPE.eLINKPASSIVE:
                rate = GetAbilityTargetSkillLinkPassive(master_data_enemy_ability, skillActivity);
                break;
            default:
                break;
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		補正対象スキルかのチェック
        @param[in]	uint					(unSkillType)	補正対象スキルタイプ
        @param[in]	BattleSkillActivity		(skillActivity)	発行したスキル情報
        @return		bool					(result)		[対象スキル：成否]
        @note		v390 Developer add
    */
    //----------------------------------------------------------------------------
    static private bool CheckTargetSkillType(uint unSkillType, BattleSkillActivity skillActivity)
    {
        bool result = false;

        if (unSkillType == 0
        || skillActivity == null)
        {
            return result;
        }

        //--------------------------------
        // 対象スキル
        // unSkillTypeのデータ順[8]：NS,LS,PS,AS,BS,LS,LPS,-
        //--------------------------------
        bool[] abChkBitTargetSkill = new bool[BIT_SKILL_MAX];
        int nBitFlag = 0;

        for (int bitNum = 0; bitNum < BIT_SKILL_MAX; ++bitNum)
        {
            nBitFlag = 1 << bitNum;

            //--------------------------------
            // フラグチェック
            //--------------------------------
            if ((unSkillType & nBitFlag) == 0)
            {
                continue;
            }

            abChkBitTargetSkill[bitNum] = true;
        }

        int nBitNum = 0;
        switch (skillActivity.m_SkillType)
        {
            case ESKILLTYPE.eACTIVE: nBitNum = BIT_SKILL_NORMAL; break;
            case ESKILLTYPE.eLEADER: nBitNum = BIT_SKILL_LEADER; break;
            case ESKILLTYPE.ePASSIVE: nBitNum = BIT_SKILL_PASSIVE; break;
            case ESKILLTYPE.eLIMITBREAK: nBitNum = BIT_SKILL_LIMITBREAK; break;
            case ESKILLTYPE.eBOOST: nBitNum = BIT_SKILL_BOOST; break;
            case ESKILLTYPE.eLINK: nBitNum = BIT_SKILL_LINK; break;
            case ESKILLTYPE.eLINKPASSIVE: nBitNum = BIT_SKILL_LINKPASSIVE; break;
            default: nBitNum = BIT_SKILL_MAX; break;
        }

        result = abChkBitTargetSkill[nBitNum];

        return result;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定スキルのダメージ補正：ノーマルスキル補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note		v390 Developer add
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityTargetSkillNormal(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eACTIVE)
        {
            return (rate);
        }

        //--------------------------------
        // タイプチェック(単体 or 全体)
        //--------------------------------
        MasterDataDefineLabel.SkillType normalType = skillActivity.m_Type;
        if (normalType != MasterDataDefineLabel.SkillType.ATK_ONCE
        && normalType != MasterDataDefineLabel.SkillType.ATK_ALL)
        {
            return (rate);
        }

        //--------------------------------
        // 補正値取得
        //--------------------------------
        float normalRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_SKILL_RATE());
        rate = InGameUtilBattle.AvoidErrorMultiple(rate, normalRate);

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定スキルのダメージ補正：リーダースキル補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note		v390 Developer add
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityTargetSkillLeader(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eLEADER)
        {
            return (rate);
        }

        //リーダースキル：追撃
        //ここに来るまでにマスターのチェックは済んでいる想定

        //--------------------------------
        // 判定：ダメージ系の場合
        //--------------------------------
        float damageRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_SKILL_RATE());
        rate = InGameUtilBattle.AvoidErrorMultiple(rate, damageRate);

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定スキルのダメージ補正：パッシブスキル補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note		v390 Developer add
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityTargetSkillPassive(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.ePASSIVE)
        {
            return (rate);
        }

        //パッシブスキル：追撃、カウンター
        //ここに来るまでにマスターのチェックは済んでいる想定

        //--------------------------------
        // 判定：ダメージ系の場合
        //--------------------------------
        float damageRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_SKILL_RATE());
        rate = InGameUtilBattle.AvoidErrorMultiple(rate, damageRate);

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定スキルのダメージ補正：アクティブスキル補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note		v390 Developer add
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityTargetSkillActive(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eLIMITBREAK)
        {
            return (rate);
        }

        //--------------------------------
        // アクティブスキル（リミブレスキル）マスターを取得
        //--------------------------------
        MasterDataSkillLimitBreak activeMaster = BattleParam.m_MasterDataCache.useSkillLimitBreak(skillActivity.m_SkillParamSkillID);

        if (activeMaster == null)
        {
            return (rate);
        }

        //--------------------------------
        // 判定：ダメージ系の場合
        //--------------------------------
        if (activeMaster.skill_damage_enable == MasterDataDefineLabel.BoolType.ENABLE)
        {
            float damageRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_SKILL_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, damageRate);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定スキルのダメージ補正：ブースト補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note		v390 Developer add
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityTargetSkillBoost(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eBOOST)
        {
            return (rate);
        }

        //--------------------------------
        // ブーストスキルマスターを取得
        //--------------------------------
        MasterDataSkillBoost boostMaster = BattleParam.m_MasterDataCache.useSkillBoost(skillActivity.m_SkillParamSkillID);

        if (boostMaster == null)
        {
            return (rate);
        }

        //--------------------------------
        // 判定：ダメージ系の場合
        //--------------------------------
        if (boostMaster.skill_damage_enable == MasterDataDefineLabel.BoolType.ENABLE)
        {
            float damageRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_SKILL_RATE());
            rate = InGameUtilBattle.AvoidErrorMultiple(rate, damageRate);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定スキルのダメージ補正：リンクスキル補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note		v390 Developer add
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityTargetSkillLink(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eLINK)
        {
            return (rate);
        }

        //--------------------------------
        // リンクスキルマスターを取得
        //--------------------------------
        //--------------------------------
        // タイプチェック(単体 or 全体)
        //--------------------------------
        MasterDataDefineLabel.SkillType linkType = skillActivity.m_Type;
        if (linkType != MasterDataDefineLabel.SkillType.ATK_ONCE
        && linkType != MasterDataDefineLabel.SkillType.ATK_ALL)
        {
            return (rate);
        }

        //--------------------------------
        // 判定：ダメージ系の場合
        //--------------------------------
        float linkRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_SKILL_RATE());
        rate = InGameUtilBattle.AvoidErrorMultiple(rate, linkRate);

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定スキルのダメージ補正：リンクパッシブスキル補正
        @param[in]	int[]				(paramList)		パラメータリスト
        @param[in]	BattleSkillActivity	(skillActivity)	スキル情報
        @return		float				(rate)			[ダメージ倍率]
        @note		v390 Developer add
    */
    //----------------------------------------------------------------------------
    static private float GetAbilityTargetSkillLinkPassive(MasterDataEnemyAbility master_data_enemy_ability, BattleSkillActivity skillActivity)
    {
        float rate = 1.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (skillActivity == null
        || skillActivity.m_SkillType != ESKILLTYPE.eLINKPASSIVE)
        {
            return (rate);
        }

        //リンクパッシブスキル：追撃、カウンター
        //ここに来るまでにマスターのチェックは済んでいる想定

        //--------------------------------
        // 判定：ダメージ系の場合
        //--------------------------------
        float damageRate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_DMG_SKILL_RATE());
        rate = InGameUtilBattle.AvoidErrorMultiple(rate, damageRate);

        return (rate);
    }
}
///////////////////////////////////////EOF///////////////////////////////////////