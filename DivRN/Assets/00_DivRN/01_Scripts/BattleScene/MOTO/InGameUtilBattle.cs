/*==========================================================================*/
/*==========================================================================*/
/*!
    @file	InGameUtilBattle.cs
    @brief	InGameUtil からバトルシーン側が使用するものを抜き出したもの。
            InGameUtilと被っている関数などがあるがその調整は後で考える。
    @author Developer
    @date 	2012/10/09
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
    @class		InGameUtil
    @brief		インゲーム関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class InGameUtilBattle
{
    const float DEF_INGAME_DB_RATE = 0.01f;
    const float DEF_INGAME_AVOID_ERROR = 0.0000001f;

    public const int VALUE_DMG_MAX_PLYAER = (99999999);//!< 最大ダメージ値：プレイヤー
    public const int VALUE_DMG_MAX_ENEMY = (999999999);//!< 最大ダメージ値：エネミー
    public const int VALUE_ATK_ALL_HIGH = (180);		//!< エフェクトしきい値：全体：大
    public const int VALUE_ATK_ONCE_MID = (160);		//!< エフェクトしきい値：単体：中
    public const int VALUE_ATK_ONCE_HIGH = (230);		//!< エフェクトしきい値：単体：大
    public const int VALUE_HEAL_MID = (30);		//!< エフェクトしきい値：回復：中
    public const int VALUE_HEAL_HIGH = (60);		//!< エフェクトしきい値：回復：大
    public const int VALUE_HEAL_MAX = (100);		//!< エフェクトしきい値：回復：最大
    public const int SKILL_EFFECT_LOW = (0);		//!< エフェクトタイプ：小
    public const int SKILL_EFFECT_MID = (1);		//!< エフェクトタイプ：中
    public const int SKILL_EFFECT_HIGH = (2);		//!< エフェクトタイプ：大
    public const int SKILL_EFFECT_MAX = (3);		//!< エフェクトタイプ：最大

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    static private float[][] m_SkillElementRate = null;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
        @brief			スキルによるダメージ実値
        @param[in]		CharaOnce					(cLeader)			リーダー
        @param[in]		CharaOnce					(cFriend)			フレンド
        @param[in]		CharaOnce					(cChara)			攻撃者
        @param[in]		CharaOnce					(cTarget)			対象者
        @param[in]		bool						(bBoost)			ブースト
        @param[in]		int							(nTargetHP)			攻撃対象のHP
        @param[in]		int							(nAilmentID)		状態異常管理ID
        @param[in]		int							(nSkillCount)		スキル使用回数
        @param[in]		int							(nSkillElem)		スキル属性
        @param[in]		int							(category)			スキルカテゴリ
        @param[in]		int[]						(param)				スキル発動パラメータ
        @return			int							[スキルによるダメージ値]
    */
    //----------------------------------------------------------------------------
    static public int GetDamageValue(CharaOnce cLeader,
                                        CharaOnce cFriend,
                                        CharaOnce cChara,
                                        CharaOnce cTarget,
                                        BattleEnemy targetParam,
                                        bool bBoost,
                                        StatusAilmentChara nAilmentID,
                                        int nSkillCount,
                                        BattleSkillActivity skillActivity)
    {

        if (skillActivity.m_SkillParamOwnerNum == GlobalDefine.PartyCharaIndex.HERO)
        {
            return GetHeroDamageValue(cTarget, targetParam, skillActivity);
        }

        //------------------------------
        // キャラがいないと相性判定等できない。
        // とりあえずログ出してテキトーな数値を返す
        //------------------------------
        if (cChara == null)
        {
            Debug.LogError("DamageValue , Owner Is None!");
            return 0;
        }


        float fCharaPow = 0.0f;
        float fElementRate = 1.0f;
        float leaderPow = 1.0f;
        float passivePow = 1.0f;
        float fLinkPassivePow = 1.0f;
        float fBoostRate = 1.0f;
        float fAilmentRate = 1.0f;
        float fCriticalRate = InGameDefine.CRITICAL_RATE;

        MasterDataDefineLabel.ElementType nSkillElem = skillActivity.m_Element;
        MasterDataDefineLabel.SkillCategory category = skillActivity.m_Category_SkillCategory_PROPERTY;

        uint unLinkUnitID = 0;
        float fLinkBonusBst = 0.0f;

        //------------------------------
        // 攻撃力
        //------------------------------
        fCharaPow = cChara.m_CharaPow;

        //------------------------------
        // リンクボーナス
        //------------------------------
        // @add Developer 2015/09/03 ver300
        if (cChara.m_LinkParam != null)
        {
            unLinkUnitID = cChara.m_LinkParam.m_CharaID;
        }
        #region ==== リンク：種族ボーナス処理 ====
        if (skillActivity.m_SkillType == ESKILLTYPE.eACTIVE
        || skillActivity.m_SkillType == ESKILLTYPE.eLINK)
        {
            MasterDataParamChara cCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(unLinkUnitID);
            if (cCharaMaster != null)
            {
                float fWork = 0.0f;

                // クリティカル威力アップ
                fWork = CharaLinkUtil.GetLinkUnitBonusRace(cCharaMaster, cCharaMaster.kind, CharaUtil.VALUE.CRT);		// %値取得(メイン)
                fWork += CharaLinkUtil.GetLinkUnitBonusRace(cCharaMaster, cCharaMaster.sub_kind, CharaUtil.VALUE.CRT);		// %値取得(サブ)
                fWork = GetDBRevisionValue(fWork);																			// 数値変換
                fCriticalRate += fWork;

                // ブーストパネル威力アップ
                fLinkBonusBst = CharaLinkUtil.GetLinkUnitBonusRace(cCharaMaster, cCharaMaster.kind, CharaUtil.VALUE.BST_PANEL);	// %値取得(メイン)
                fLinkBonusBst += CharaLinkUtil.GetLinkUnitBonusRace(cCharaMaster, cCharaMaster.sub_kind, CharaUtil.VALUE.BST_PANEL);	// %値取得(サブ)
                fLinkBonusBst = GetDBRevisionValue(fLinkBonusBst);																	// 数値変換
            }
        }
        #endregion

        //------------------------------
        // クリティカル(キャラの攻撃力*固定倍率)
        //------------------------------
        if (skillActivity.m_bCritical == true)
        {
            fCharaPow *= fCriticalRate;

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　キャラ攻撃力:" + fCharaPow.ToString("F2")
                + " = " + ((int)cChara.m_CharaPow).ToString() + "(キャラ基本攻撃力)"
                + " x" + InGameUtilBattle.getDebugPercentString(fCriticalRate) + "%(クリティカル補正)"
            );

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　　クリティカル補正:" + InGameUtilBattle.getDebugPercentString(fCriticalRate) + "%"
                + " = " + InGameUtilBattle.getDebugPercentString(InGameDefine.CRITICAL_RATE) + "%(基本)"
                + " + " + InGameUtilBattle.getDebugPercentString(fCriticalRate - InGameDefine.CRITICAL_RATE) + "%(リンクボーナス)"
            );
        }
        else
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　キャラ攻撃力:" + ((int)fCharaPow).ToString()
                + " = " + ((int)cChara.m_CharaPow).ToString() + "(キャラ基本攻撃力)"
            );
        }

        //------------------------------
        // 属性倍率
        //------------------------------
        if (cTarget != null)
        {
            fElementRate = GetSkillElementRate(nSkillElem, cTarget.m_CharaMasterDataParam.element);
        }

        //------------------------------
        // ブースト
        //------------------------------
        fBoostRate = (bBoost == true) ? InGameDefine.BOOST_RATE_MAX + fLinkBonusBst : InGameDefine.BOOST_RATE_MIN;


        //------------------------------
        // リーダースキル補正(とりあえずver)
        //------------------------------
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex.LEADER, cChara));
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex.FRIEND, cChara));
        //		leaderPow = AvoidErrorMultiple( leaderPow, GetLeaderSkillDamageUPColor( GlobalDefine.PARTY_CHARA_LEADER ) );
        //		leaderPow = AvoidErrorMultiple( leaderPow, GetLeaderSkillDamageUPColor( GlobalDefine.PARTY_CHARA_FRIEND ) );
        //		leaderPow = AvoidErrorMultiple( leaderPow, GetLeaderSkillDamageUPHands( GlobalDefine.PARTY_CHARA_LEADER ) );
        //		leaderPow = AvoidErrorMultiple( leaderPow, GetLeaderSkillDamageUPHands( GlobalDefine.PARTY_CHARA_FRIEND ) );
        // アクティブ(リミットブレイク)以外の場合チェックを行う(発動条件が、ノーマルスキルに影響する)
        if (skillActivity.m_SkillType != ESKILLTYPE.eLIMITBREAK)
        {
            leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRateCondition(GlobalDefine.PartyCharaIndex.LEADER));
            leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRateCondition(GlobalDefine.PartyCharaIndex.FRIEND));
        }


        //------------------------------
        // パッシブスキル補正(とりあえずver)
        //------------------------------
        GlobalDefine.PartyCharaIndex owner = GlobalDefine.PartyCharaIndex.MAX;	// ダミー（未使用）
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkSkillDamageRate(cChara));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkKindKillerAtk(cChara));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkDamageUPFloorPanel(ref owner));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkDamageUPElemNum(ref owner));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkDamageUPHandsNum(ref owner));

        //------------------------------
        // リンクパッシブスキル補正
        //------------------------------
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkSkillDamageRate(cChara, ESKILLTYPE.eLINKPASSIVE));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkKindKillerAtk(cChara, ESKILLTYPE.eLINKPASSIVE));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkDamageUPFloorPanel(ref owner, ESKILLTYPE.eLINKPASSIVE, unLinkUnitID));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkDamageUPElemNum(ref owner, ESKILLTYPE.eLINKPASSIVE, unLinkUnitID));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkDamageUPHandsNum(ref owner, ESKILLTYPE.eLINKPASSIVE, unLinkUnitID));

        //------------------------------
        // 状態異常によるダメージ増減
        //------------------------------
        // 攻撃力倍率
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, nAilmentID.GetOffenceRate());
        // 属性攻撃力倍率
        // ※属性エンハンス系、属性逆エンハンス系の計算処理。属性参照はユニット単位で行うこと！
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, nAilmentID.GetOffenceElementRate(cChara.m_CharaMasterDataParam.element));
        // 種族攻撃力倍率(メイン)
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, nAilmentID.GetOffenceKindRate(cChara.kind));
        // 種族攻撃力倍率(サブ)
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, nAilmentID.GetOffenceKindRate(cChara.kind_sub));


        //------------------------------
        // スキルコンボ数
        //------------------------------
        float fSkillCountRate = GetSkillCountRate(nSkillCount);

        int damage = 0;
        float temp = 0.0f;
        float work = 1.0f;

        //	攻撃力倍率
        if (skillActivity.m_skill_power != 0)
        {
            work = 1.0f;
            work = GetDBRevisionValue(skillActivity.m_skill_power);
            temp += AvoidErrorMultiple(fCharaPow, work);
        }

        //	固定値
        if (skillActivity.m_skill_power_fix != 0)
        {
            temp += skillActivity.m_skill_power_fix;
        }

        //	HP割合
        if (skillActivity.m_skill_power_hp_rate != 0)
        {
            work = 1.0f;
            work = GetDBRevisionValue(skillActivity.m_skill_power_hp_rate);
            temp += AvoidErrorMultiple(targetParam.m_EnemyHP, work);
        }

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　スキル基本威力:" + temp.ToString("F2") + " ="
            + ((skillActivity.m_skill_power != 0) ? " + (" + fCharaPow.ToString("F2") + "(キャラ攻撃力)" : "")
            + ((skillActivity.m_skill_power != 0) ? " x" + skillActivity.m_skill_power.ToString() + "%(スキル攻撃力倍率))" : "")
            + ((skillActivity.m_skill_power_fix != 0) ? " + " + skillActivity.m_skill_power_fix.ToString() + "(スキル固定値)" : "")
            + ((skillActivity.m_skill_power_hp_rate != 0) ? " + (" + targetParam.m_EnemyHP.ToString() + "(敵ＨＰ)" : "")
            + ((skillActivity.m_skill_power_hp_rate != 0) ? " x" + skillActivity.m_skill_power_hp_rate.ToString() + "%(スキルＨＰ割合))" : "")
        );


        //	ブースト補正
        temp = AvoidErrorMultiple(temp, fBoostRate);

        //	弱点属性補正
        if (skillActivity.m_skill_chk_atk_affinity != MasterDataDefineLabel.BoolType.DISABLE)
        {
            temp = AvoidErrorMultiple(temp, fElementRate);
        }

        //	リーダースキル補正
        if (skillActivity.m_skill_chk_atk_leader != MasterDataDefineLabel.BoolType.DISABLE)
        {
            temp = AvoidErrorMultiple(temp, leaderPow);
        }

        //	パッシブスキル補正
        if (skillActivity.m_skill_chk_atk_passive != MasterDataDefineLabel.BoolType.DISABLE)
        {
            temp = AvoidErrorMultiple(temp, passivePow);

            // リンクパッシブスキル補正
            // @add Developer 2015/09/08 ver300
            temp = AvoidErrorMultiple(temp, fLinkPassivePow);
        }

        //	状態変化補正
        if (skillActivity.m_skill_chk_atk_ailment != MasterDataDefineLabel.BoolType.DISABLE)
        {
            temp = AvoidErrorMultiple(temp, fAilmentRate);
        }

        //	状態変化補正
        if (skillActivity.m_skill_chk_atk_combo != MasterDataDefineLabel.BoolType.DISABLE)
        {
            temp = AvoidErrorMultiple(temp, fSkillCountRate);
        }

        // 敵特性による補正
        float fEnemyAbilityRate = EnemyAbility.GetAbilityDamageRatePlayer(skillActivity, targetParam);
        temp = AvoidErrorMultiple(temp, fEnemyAbilityRate);

        damage = (int)temp;

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　スキル威力:" + damage.ToString() + " = スキル基本威力"
            + " x" + InGameUtilBattle.getDebugPercentString(fBoostRate) + "%(ブースト補正)"
            + " x" + ((skillActivity.m_skill_chk_atk_affinity != MasterDataDefineLabel.BoolType.DISABLE) ? InGameUtilBattle.getDebugPercentString(fElementRate) + "%" : "100%") + "(弱点属性補正)"
            + " x" + ((skillActivity.m_skill_chk_atk_leader != MasterDataDefineLabel.BoolType.DISABLE) ? InGameUtilBattle.getDebugPercentString(leaderPow) + "%" : "100%") + "(リーダースキル補正)"
            + " x" + ((skillActivity.m_skill_chk_atk_passive != MasterDataDefineLabel.BoolType.DISABLE) ? InGameUtilBattle.getDebugPercentString(passivePow) + "%" : "100%") + "(パッシブスキル補正)"
            + " x" + ((skillActivity.m_skill_chk_atk_passive != MasterDataDefineLabel.BoolType.DISABLE) ? InGameUtilBattle.getDebugPercentString(fLinkPassivePow) + "%" : "100%") + "(リンクパッシブスキル補正)"
            + " x" + ((skillActivity.m_skill_chk_atk_ailment != MasterDataDefineLabel.BoolType.DISABLE) ? InGameUtilBattle.getDebugPercentString(fAilmentRate) + "%" : "100%") + "(状態変化補正)"
            + " x" + ((skillActivity.m_skill_chk_atk_combo != MasterDataDefineLabel.BoolType.DISABLE) ? InGameUtilBattle.getDebugPercentString(fSkillCountRate) + "%" : "100%") + "(コンボ数補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(fEnemyAbilityRate) + "%(敵特性による補正)"
        );

        return damage;
    }

    /// <summary>
    /// 発動者がヒーローの場合のダメージ計算
    /// </summary>
    /// <param name="targetParam"></param>
    /// <param name="skillActivity"></param>
    /// <returns></returns>
    static int GetHeroDamageValue(CharaOnce cTarget, BattleEnemy targetParam, BattleSkillActivity skillActivity)
    {
        int damage = 0;

        //	攻撃力倍率
        if (skillActivity.m_skill_power != 0)
        {
            damage += (int)AvoidErrorMultiple(BattleParam.m_PlayerParty.m_BattleHero.getAtk(), GetDBRevisionValue(skillActivity.m_skill_power));
        }

        //	固定値
        if (skillActivity.m_skill_power_fix != 0)
        {
            damage += skillActivity.m_skill_power_fix;
        }

        //	HP割合
        if (skillActivity.m_skill_power_hp_rate != 0)
        {
            float work = GetDBRevisionValue(skillActivity.m_skill_power_hp_rate);
            damage += (int)AvoidErrorMultiple(targetParam.m_EnemyHP, work);
        }

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "  スキル威力:" + damage.ToString()
            + " = " + ((int)AvoidErrorMultiple(BattleParam.m_PlayerParty.m_BattleHero.getAtk(), GetDBRevisionValue(skillActivity.m_skill_power))).ToString() + "(" + BattleParam.m_PlayerParty.m_BattleHero.getAtk().ToString() + "(主人公攻撃力) x " + skillActivity.m_skill_power.ToString() + "%(割合威力)"
            + " + " + skillActivity.m_skill_power_fix.ToString() + "(固定威力)"
            + " + " + ((int)AvoidErrorMultiple(targetParam.m_EnemyHP, GetDBRevisionValue(skillActivity.m_skill_power_hp_rate))).ToString() + "("
            + targetParam.m_EnemyHP.ToString() + "(敵HP) x " + skillActivity.m_skill_power_hp_rate.ToString() + "%(スキル基本割合))"
        );

        //------------------------------
        // 属性倍率
        //------------------------------
        //	弱点属性補正
        if (skillActivity.m_skill_chk_atk_affinity != MasterDataDefineLabel.BoolType.DISABLE)
        {
            if (cTarget != null)
            {
                float fElementRate = GetSkillElementRate(skillActivity.m_Element, cTarget.m_CharaMasterDataParam.element);
                damage = (int)AvoidErrorMultiple(damage, fElementRate);
            }
        }

        return damage;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		リミットブレイクスキル発動情報作成
        @param[out]	BattleSkillActivity[]		(skillActivity)		発動情報出力変数
        @param[in]	int							(charaIdx)			発動キャラパーティインデックス
        @param[in]	int							(fieldID)			発動フィールドのID
        @param[in]	uint						(skillID)			スキルID
    */
    //----------------------------------------------------------------------------
    static public BattleSkillActivity ActivityLimitBreakSkill(GlobalDefine.PartyCharaIndex charaIdx, int fieldID, uint skillID)
    {
        MasterDataSkillLimitBreak param = BattleParam.m_MasterDataCache.useSkillLimitBreak(skillID);
        if (param == null)
        {
            return null;
        }


        //--------------------------------
        //	スキル発動情報
        //--------------------------------
        BattleSkillActivity activity = new BattleSkillActivity();
        activity.m_SkillParamOwnerNum = charaIdx;
        activity.m_SkillParamFieldID = InGameDefine.SELECT_NONE;
        activity.m_SkillParamSkillID = skillID;
        activity.m_Effect = param.skill_effect;
        activity.m_Type = param.skill_type;
        activity.m_Element = param.skill_elem;
        activity.m_SkillType = ESKILLTYPE.eLIMITBREAK;
        activity.m_Category_SkillCategory_PROPERTY = param.skill_cate;
        activity.m_SkillParamTarget = null;

        activity.m_skill_power = param.skill_power;
        activity.m_skill_power_fix = param.skill_power_fix;
        activity.m_skill_power_hp_rate = param.skill_power_hp_rate;

        activity.m_skill_absorb = param.skill_absorb;
        activity.m_skill_kickback = param.skill_kickback;
        activity.m_skill_kickback_fix = param.skill_kickback_fix;

        activity.m_skill_chk_atk_affinity = param.skill_chk_atk_affinity;
        activity.m_skill_chk_atk_leader = param.skill_chk_atk_leader;
        activity.m_skill_chk_atk_passive = param.skill_chk_atk_passive;
        activity.m_skill_chk_atk_ailment = param.skill_chk_atk_ailment;
        activity.m_skill_chk_atk_combo = MasterDataDefineLabel.BoolType.DISABLE;			// コンボレートはのらない

        activity.m_skill_chk_def_defence = param.skill_chk_def_defence;
        activity.m_skill_chk_def_ailment = param.skill_chk_def_ailment;
        activity.m_skill_chk_def_barrier = param.skill_chk_def_barrier;

        activity.m_statusAilment_target = param.status_ailment_target;
        activity.m_statusAilment = new int[] {   param.status_ailment1,
                                                            param.status_ailment2,
                                                            param.status_ailment3,
                                                            param.status_ailment4 };

        activity._setParam(param);

        return activity;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル発動情報作成
        @param[in]	int							(charaIdx)			キャラパーティインデックス
        @return		BattleSkillActivity[]		(activitySkill)		発動情報出力変数
        @param[in]	ESKILLTYPE					(skillType)			スキルタイプ
        @chenge		Developer 2015/09/15 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public BattleSkillActivity ActivityPassiveSkill(GlobalDefine.PartyCharaIndex charaIdx, ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE)
    {

        //------------------------------
        //	指定キャラのパッシブ情報を取得
        //------------------------------
        CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember(charaIdx, CharaParty.CharaCondition.SKILL_PASSIVE);
        if (chara == null)
        {
            return null;
        }

        if (!chara.m_bHasCharaMasterDataParam)
        {
            return null;
        }


        MasterDataSkillPassive param = null;
        //------------------------------
        // スキルタイプによる分岐
        //------------------------------
        switch (skillType)
        {
            //------------------------------
            // パッシブスキル
            //------------------------------
            case ESKILLTYPE.ePASSIVE:
                param = BattleParam.m_MasterDataCache.useSkillPassive(chara.m_CharaMasterDataParam.skill_passive);
                break;

            //------------------------------
            // リンクパッシブ
            //------------------------------
            case ESKILLTYPE.eLINKPASSIVE:
                {
                    if (chara.m_LinkParam == null)
                    {
                        return null;
                    }

                    MasterDataParamChara linkCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(chara.m_LinkParam.m_CharaID);
                    if (linkCharaMaster == null)
                    {
                        return null;
                    }

                    param = BattleParam.m_MasterDataCache.useSkillPassive(linkCharaMaster.link_skill_passive);
                    break;
                }
        }

        if (param == null)
        {
            return null;
        }


        //------------------------------
        //	発動情報作成
        //------------------------------
        BattleSkillActivity activity = new BattleSkillActivity();
        if (activity == null)
        {
            return null;
        }

        // TODO:DB拡張
        // ・対象情報がDBに含まれていないので、スキルの種類から対象を判別
        // ・カテゴリ情報がDBに含まれていないので、スキルの種類からカテゴリを判別
        // ・エフェクト情報がDBに含まれていないので、スキルの種類からエフェクトを判別
        // ・最後までパラメータを汎用化してくれなかったのでバラバラに判別
        // ・回復カウンターだけエフェクト指定がない
        MasterDataDefineLabel.SkillType type = MasterDataDefineLabel.SkillType.NONE;
        MasterDataDefineLabel.SkillCategory cate = MasterDataDefineLabel.SkillCategory.NONE;
        MasterDataDefineLabel.ElementType elem = MasterDataDefineLabel.ElementType.NONE;
        MasterDataDefineLabel.UIEffectType effect = 0;

        if (param.Is_skill_powup_kind_active())
        {

            //------------------------------
            // 補助
            //------------------------------
            type = MasterDataDefineLabel.SkillType.SUPPORT;
            cate = MasterDataDefineLabel.SkillCategory.NONE;

        }
        else if (param.Is_skill_counter_atk_active())
        {

            //------------------------------
            // カウンター
            //------------------------------
            type = MasterDataDefineLabel.SkillType.ATK_ONCE;
            cate = MasterDataDefineLabel.SkillCategory.ATK;
            elem = param.skill_counter_atk_element;
            effect = param.skill_counter_atk_effect;

        }
        else if (param.Is_skill_damage_recovery_active())
        {

            //------------------------------
            // ダメージを受けた時に回復
            //------------------------------
            type = MasterDataDefineLabel.SkillType.HEAL;
            cate = MasterDataDefineLabel.SkillCategory.NONE;
            elem = MasterDataDefineLabel.ElementType.HEAL;
            effect = MasterDataDefineLabel.UIEffectType.HEAL_1;

        }
        else if (param.Get_FullLifeAttackUpInfo() != null)
        {

            //------------------------------
            // HP満タン時に攻撃力アップ
            //------------------------------
            type = MasterDataDefineLabel.SkillType.SUPPORT;
            cate = MasterDataDefineLabel.SkillCategory.NONE;

        }
        else if (param.Get_DyingAttackUpInfo() != null)
        {

            //------------------------------
            // 瀕死時に攻撃力アップ
            //------------------------------
            type = MasterDataDefineLabel.SkillType.SUPPORT;
            cate = MasterDataDefineLabel.SkillCategory.NONE;
        }

        activity.m_SkillParamOwnerNum = charaIdx;
        activity.m_SkillParamFieldID = InGameDefine.SELECT_NONE;
        activity.m_SkillParamSkillID = param.fix_id;
        activity.m_Effect = effect;
        activity.m_Type = type;
        activity.m_Element = elem;
        activity.m_SkillType = skillType;
        activity.m_Category_SkillCategory_PROPERTY = cate;
        activity.m_SkillParamTarget = null;


        return activity;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		回復スキルのターゲット情報の作成処理
        @param[out]	ref BattleSkillActivity		(activity)		回復スキルの発動情報
        @param[in]	bool						(boost)			ブーストフラグ
        @param[in]	int							(category)		スキルカテゴリ
        @param[in]	int[]						(param)			スキルパラメータ
    */
    //----------------------------------------------------------------------------
    static public void CreateHealTarget(ref BattleSkillActivity activity, bool boost)
    {

        //------------------------------
        // エラーチェック
        //------------------------------
        if (activity.m_SkillParamTarget == null)
        {
            activity.m_SkillParamTarget = new BattleSkillTarget[1];
            if (activity.m_SkillParamTarget == null)
            {
                return;
            }
        }

        if (activity.m_SkillParamTarget[0] == null)
        {
            activity.m_SkillParamTarget[0] = new BattleSkillTarget();
            if (activity.m_SkillParamTarget[0] == null)
            {
                return;
            }
        }


        activity.m_SkillParamTarget[0].m_TargetType = BattleSkillTarget.TargetType.PLAYER;
        activity.m_SkillParamTarget[0].m_SkillValueToPlayer.setValue(GlobalDefine.PartyCharaIndex.MAX,
            GetHealValue(BattleParam.m_PlayerParty, boost, activity).toMultiInt());

#if UNITY_EDITOR
        Debug.Log(activity.m_SkillParamTarget[0].m_SkillValueToPlayer);
#endif // #if UNITY_EDITOR

    }


    //----------------------------------------------------------------------------
    /*!
        @brief		プレイヤーを対象としたターゲット情報の作成処理
        @param[in]	ref BattleSkillActivity		(activity)		スキルの発動情報
    */
    //----------------------------------------------------------------------------
    public static void CreateTargetPlayer(ref BattleSkillActivity activity)
    {

        //------------------------------
        // エラーチェック
        //------------------------------
        if (activity.m_SkillParamTarget == null)
        {
            activity.m_SkillParamTarget = new BattleSkillTarget[1];
            if (activity.m_SkillParamTarget == null)
            {
                return;
            }
        }

        if (activity.m_SkillParamTarget[0] == null)
        {
            activity.m_SkillParamTarget[0] = new BattleSkillTarget();
            if (activity.m_SkillParamTarget[0] == null)
            {
                return;
            }
        }


        activity.m_SkillParamTarget[0].m_TargetType = BattleSkillTarget.TargetType.PLAYER;
        activity.m_SkillParamTarget[0].m_SkillValueToPlayer.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);

    }


    //----------------------------------------------------------------------------
    /*!
        @brief			回復量取得[リミットブレイクスキル等で使用]	<static>
        @param[in]		CharaParty		(party)			キャラパーティ
        @param[in]		bool			(bBoost)		ブーストパネル
        @param[in]		int				(category)		スキルカテゴリ
        @param[in]		int[]			(param)			スキルパラメータ
        @return			float			[回復量]
    */
    //----------------------------------------------------------------------------
    static public BattleSceneUtil.MultiFloat GetHealValue(CharaParty party, bool bBoost, BattleSkillActivity activity)
    {

        BattleSceneUtil.MultiFloat heal = new BattleSceneUtil.MultiFloat(0.0f);

        bool bChkAilment = false;

        //------------------------------
        // リンクボーナス(回復ノーマルスキルの場合)
        //------------------------------
        // @add Developer 2015/09/18 ver300
        float fLinkBonusBst = 0.0f;
        #region ==== リンク：種族ボーナス処理 ====
        if (activity.m_SkillType == ESKILLTYPE.eACTIVE)
        {
            uint unLinkUnitID = CharaLinkUtil.GetLinkUnitID(activity.m_SkillParamOwnerNum);
            MasterDataParamChara cCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(unLinkUnitID);
            if (cCharaMaster != null)
            {
                // ブーストパネル威力アップ
                fLinkBonusBst = CharaLinkUtil.GetLinkUnitBonusRace(cCharaMaster, cCharaMaster.kind, CharaUtil.VALUE.BST_PANEL);	// %値取得(メイン)
                fLinkBonusBst += CharaLinkUtil.GetLinkUnitBonusRace(cCharaMaster, cCharaMaster.sub_kind, CharaUtil.VALUE.BST_PANEL);	// %値取得(サブ)
                fLinkBonusBst = GetDBRevisionValue(fLinkBonusBst);																	// 数値変換
            }
        }
        #endregion

        //--------------------------------
        // スキルタイプによって処理を分岐
        //--------------------------------
        switch (activity.m_SkillType)
        {
            default:
            case ESKILLTYPE.eLIMITBREAK:
                //--------------------------------
                // 効果によって回復量計算処理を分岐
                //--------------------------------
                switch (activity.m_Category_SkillCategory_PROPERTY)
                {
                    default:
                    case MasterDataDefineLabel.SkillCategory.RECOVERY_HP_RATE:
                        //------------------------------
                        // 割合回復
                        //------------------------------
                        {
                            float rate;
                            // スキルタイプによる分岐
                            switch (activity.m_SkillType)
                            {
                                case ESKILLTYPE.eACTIVE:
                                case ESKILLTYPE.eLINK:
                                    // スキル発動判定時に、必要な処理は終えている
                                    rate = GetDBRevisionValue(activity.m_skill_power);
                                    break;

                                default:
                                    rate = GetDBRevisionValue(activity.Get_RECOVERY_HP_VALUE());
                                    break;
                            }
                            heal = GetHealValue(party, rate, bBoost, fLinkBonusBst, bChkAilment);
                        }
                        break;

                    case MasterDataDefineLabel.SkillCategory.RECOVERY_HP:
                        //------------------------------
                        // 固定回復
                        //------------------------------
                        {
                            float value = (float)activity.Get_RECOVERY_FIX_HP_VALUE();

                            int alive_count = 0;
                            for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
                            {
                                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.ALIVE);
                                if (chara_once != null)
                                {
                                    alive_count++;
                                }
                            }
                            if (alive_count > 0)
                            {
                                value = (value + alive_count - 1) / alive_count;    // 端数切り上げ
                            }

                            heal.setValue(GlobalDefine.PartyCharaIndex.MAX, value);
                        }
                        break;
                }
                break;

            case ESKILLTYPE.eBOOST:
                //--------------------------------
                // 効果によって回復量計算処理を分岐
                //--------------------------------
                switch (activity.m_Category_BoostSkillCategory_PROPERTY)
                {
                    default:
                    case MasterDataDefineLabel.BoostSkillCategory.HEAL_HP_RATE:
                        //------------------------------
                        // 割合回復
                        //------------------------------
                        float rate = GetDBRevisionValue(activity.Get_BOOSTSKILL_HEAL_HP_RATE_VALUE());

                        heal = GetHealValue(party, rate, false, 0.0f, bChkAilment);

                        break;

                    case MasterDataDefineLabel.BoostSkillCategory.HEAL_HP_FIX:
                        //------------------------------
                        // 固定回復
                        //------------------------------
                        float value = (float)activity.Get_BOOSTSKILL_HEAL_HP_FIX_VALUE();

                        int alive_count = 0;
                        for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
                        {
                            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.ALIVE);
                            if (chara_once != null)
                            {
                                alive_count++;
                            }
                        }
                        if (alive_count > 0)
                        {
                            value = (value + alive_count - 1) / alive_count;    // 端数切り上げ
                        }

                        heal.setValue(GlobalDefine.PartyCharaIndex.MAX, value);

                        break;
                }
                break;
        }

        //----------------------------------------
        // 状態異常確認
        // @add Developer 2016/05/30 v350 回復不可[全]対応
        //----------------------------------------
        // 状態異常：回復不可[全]の場合
        if (activity.m_SkillType == ESKILLTYPE.eLIMITBREAK
            && activity.m_Category_SkillCategory_PROPERTY == MasterDataDefineLabel.SkillCategory.SUPPORT_ABSTATE_CLEAR
        )
        {
            // リミブレスキルでSUPPORT_ABSTATE_CLEAR効果がある時は回復不可[全]の効果は無視する（回復の前に状態異常がクリアされていることにするらしい）
        }
        else
        {
            for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
            {
                StatusAilmentChara ailmnet_chara = party.m_Ailments.getAilment((GlobalDefine.PartyCharaIndex)idx);
                if (ailmnet_chara != null
                    && ailmnet_chara.GetNonRecoveryAll()
                )
                {
                    // 0.0fを返す
                    heal.setValue((GlobalDefine.PartyCharaIndex)idx, 0.0f);
                }
            }
        }

        return heal;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief			回復量計算[通常スキル等で使用]	<static>
        @param[in]		CharaParty		(party)		キャラパーティクラス
        @param[in]		float			(rate)		割合
        @param[in]		bool			(boost)		ブースト
        @param[in]		float			(fLinkBonusBst)		リンク：ブーストボーナス値
        @param[in]		bool			(bChkAilment)		状態異常チェック
        @return			float			[回復量]
    */
    //----------------------------------------------------------------------------
    static public BattleSceneUtil.MultiFloat GetHealValue(CharaParty party, float rate, bool boost, float fLinkBonusBst = 0.0f, bool bChkAilment = true)
    {

        BattleSceneUtil.MultiFloat heal = new BattleSceneUtil.MultiFloat();

        if (party == null)
        {
            return heal;
        }

        float boost_rate = (boost == true) ? InGameDefine.BOOST_RATE_MAX + fLinkBonusBst : InGameDefine.BOOST_RATE_MIN;


        //------------------------------
        // 基本回復量=パーティ最大HP＊回復効果量＊BOOST
        //------------------------------
        heal.setValue(GlobalDefine.PartyCharaIndex.MAX, party.m_HPMax.toMultiFloat());
        heal.mulValue(GlobalDefine.PartyCharaIndex.MAX, rate);
        heal.mulValue(GlobalDefine.PartyCharaIndex.MAX, boost_rate);

        //----------------------------------------
        // 状態異常確認
        // @add		Developer 2016/05/30 v350 回復不可[全]対応
        // @note	上記関数：GetHealValueを通っている場合、
        //			二重チェックになるが、この関数単体で処理している箇所もある(リーダーや、パッシブ)
        //----------------------------------------
        // 例外対応：リミブレ(状態異常全クリア&HP回復)の場合は、通常通り処理する
        // 上記の場合のみ、falseを引数に渡して対応
        if (bChkAilment == true)
        {
            // 状態異常：回復不可[全]の場合
            for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
            {
                StatusAilmentChara ailmnet_chara = party.m_Ailments.getAilment((GlobalDefine.PartyCharaIndex)idx);
                if (ailmnet_chara != null
                    && ailmnet_chara.GetNonRecoveryAll()
                )
                {
                    // 0.0fを返す
                    heal.setValue((GlobalDefine.PartyCharaIndex)idx, 0.0f);
                }
            }
        }

        return heal;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		ダメージ軽減						<static>
        @param[in]	int				(damage)			ダメージ量
        @param[in]	int				(elem)				ダメージ属性
        @param[in]	int				(ailmentID)			状態異常管理ID
        @param[in]	int				(defence)			防御力
        @param[in]	bool			(ignoreDef)			防御無視
        @param[in]	bool			(ignoreDefAilment)	状態異常防御無視
        @param[in]	bool			(ignoreDefBarrier)	状態異常バリア無視
        @return		int				[軽減後のダメージ量]
        @note		Developer 2016/04/06 v340 バリア無視対応
    */
    //----------------------------------------------------------------------------
    static public BattleSceneUtil.MultiInt DamageReduce(BattleSceneUtil.MultiInt damage, MasterDataDefineLabel.ElementType elem, BattleSceneUtil.MultiAilment ailmentID/*, int defence, bool ignoreDef*/, bool ignoreDefAilment, bool ignoreDefBarrier)
    {
        BattleSceneUtil.MultiInt ret_val = new BattleSceneUtil.MultiInt(damage);
        for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
        {
            int wrk_damage = DamageReduce(damage.getValue((GlobalDefine.PartyCharaIndex)idx), elem, ailmentID.getAilment((GlobalDefine.PartyCharaIndex)idx), 0, true, ignoreDefAilment, ignoreDefBarrier);
            ret_val.setValue((GlobalDefine.PartyCharaIndex)idx, wrk_damage);
        }

        return ret_val;
    }
    static public int DamageReduce(int damage, MasterDataDefineLabel.ElementType elem, StatusAilmentChara ailmentID, int defence, bool ignoreDef, bool ignoreDefAilment, bool ignoreDefBarrier)
    {

        float defenceRate = 1.0f;

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　ダメージ軽減計算： " + damage.ToString() + "(スキル威力)", false);

        //------------------------------
        // 状態異常防御計算(状態異常防御無視の場合は処理しない)
        // @change Developer 2016/04/06 v340 バリア無視対応、軽減と増加の変数を分離
        //------------------------------
        if (ignoreDefAilment == false)
        {
            //------------------------------
            // 状態異常バリア計算(状態異常バリア無視の場合は処理しない)
            // @add	 Developer 2016/04/06 v340 バリア無視対応
            //------------------------------
            if (ignoreDefBarrier == false)
            {
                //------------------------------
                // 被ダメ軽減倍率
                //------------------------------
                defenceRate = ailmentID.GetDefenceRate();
                damage = (int)AvoidErrorMultiple(damage, defenceRate);

                DebugBattleLog.writeText(DebugBattleLog.StrOpe + " x" + InGameUtilBattle.getDebugPercentString(defenceRate) + "%(被ダメ軽減バフ)", false);

                //------------------------------
                // 指定属性からの被ダメ軽減
                //------------------------------
                defenceRate = ailmentID.GetDeffenceElementRate(elem);
                damage = (int)AvoidErrorMultiple(damage, defenceRate);

                DebugBattleLog.writeText(DebugBattleLog.StrOpe + " x" + InGameUtilBattle.getDebugPercentString(defenceRate) + "%(属性軽減バフ)", false);
            }
            else
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + " x100%(バリア無視攻撃)", false);
            }

            //------------------------------
            // 被ダメ増加倍率
            //------------------------------
            defenceRate = ailmentID.GetDefenceRateGain();
            damage = (int)AvoidErrorMultiple(damage, defenceRate);
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + " x" + InGameUtilBattle.getDebugPercentString(defenceRate) + "%(被ダメ増加バフ)", false);

            //------------------------------
            // 指定属性からの被ダメ増加
            //------------------------------
            defenceRate = ailmentID.GetDeffenceElementRateGain(elem);
            damage = (int)AvoidErrorMultiple(damage, defenceRate);
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + " x" + InGameUtilBattle.getDebugPercentString(defenceRate) + "%(属性増加バフ)", false);
        }
        else
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + " x100%(状態異常無視攻撃)", false);
        }

        //------------------------------
        // 防御力計算(防御力無視の場合は処理しない)
        //------------------------------
        if (ignoreDef == false)
        {
            float weekRate = ailmentID.GetDefenceWeekRate();
            damage -= (int)AvoidErrorMultiple(defence, weekRate);
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + " - " + ((int)AvoidErrorMultiple(defence, weekRate)).ToString()
                + "(防御力:"
                + defence.ToString() + "(基本)"
                + "x" + InGameUtilBattle.getDebugPercentString(defenceRate) + "%(属性)"
                + ")"
            );
        }
        else
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + " - 0(防御力無視攻撃)");
        }

        //------------------------------
        // 下限チェック
        //------------------------------
        if (damage <= 0)
        {
            damage = 0;
        }

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　軽減後ダメージ:" + damage.ToString());

        return damage;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		敵に特定種族がいる時のパワーアップするパッシブスキル		<static>
        @param[in]	CharaOnce			(cChara)		攻撃者
        @param[in]	ESKILLTYPE			(skillType)		スキルタイプ
        @return		float				(atkPow)		[ステータスの倍率]
        @chenge		Developer 2015/09/14 ver300	リンクパッシブ対応
    */
    //------------------------------------------------------------------------
    static public float PassiveChkKindKillerAtk(CharaOnce cChara, ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE)
    {
        float atkPow = 1.0f;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (cChara == null
        || !cChara.m_bHasCharaMasterDataParam)
        {
            return (atkPow);
        }


        MasterDataSkillPassive skillPassive = null;
        //------------------------------
        // スキルタイプによる分岐
        //------------------------------
        switch (skillType)
        {
            //------------------------------
            // パッシブスキル
            //------------------------------
            case ESKILLTYPE.ePASSIVE:
                skillPassive = BattleParam.m_MasterDataCache.useSkillPassive(cChara.m_CharaMasterDataParam.skill_passive);
                break;
            //------------------------------
            // リンクパッシブ
            //------------------------------
            case ESKILLTYPE.eLINKPASSIVE:
                {
                    if (cChara.m_LinkParam == null)
                    {
                        return (atkPow);
                    }

                    MasterDataParamChara linkCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(cChara.m_LinkParam.m_CharaID);
                    if (linkCharaMaster == null)
                    {
                        return (atkPow);
                    }

                    skillPassive = BattleParam.m_MasterDataCache.useSkillPassive(linkCharaMaster.link_skill_passive);
                    break;
                }
        }

        if (skillPassive == null)
        {
            return (atkPow);
        }

        //------------------------------
        // 敵に特定種族のモンスターがいれば能力アップ
        //------------------------------
        atkPow = PassiveKindKillerAtk(skillPassive);

        return (atkPow);
    }

    //------------------------------------------------------------------------
    /*!
        @brief		敵に特定種族がいる時のパワーアップするパッシブスキル		<static>
        @param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
        @return		float					(atkPow)		[ステータスの倍率]
        @add		Developer 2015/09/14 ver300 根本の処理部分を分離
    */
    //------------------------------------------------------------------------
    static private float PassiveKindKillerAtk(MasterDataSkillPassive passiveParam)
    {
        float atkPow = 1.0f;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (BattleSceneManager.Instance == null)
        {
            return (atkPow);
        }


        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null
        || passiveParam.Is_skill_powup_kind_active() == false)
        {
            return (atkPow);
        }

        //------------------------------
        // 全ての敵をチェック
        //------------------------------
        BattleEnemy[] cTarget = BattleParam.m_EnemyParam;
        for (int num = 0; num < cTarget.Length; ++num)
        {
            if (cTarget[num] == null
            || cTarget[num].getMasterDataParamChara() == null)
            {
                continue;
            }

            //------------------------------
            // 敵死亡チェック
            //------------------------------
            if (cTarget[num].isDead() == true)
            {
                continue;
            }

            //------------------------------
            // 指定種族かどうかチェック
            //------------------------------
            if (cTarget[num].getKind() != passiveParam.skill_powup_kind_type
            && cTarget[num].getKindSub() != passiveParam.skill_powup_kind_type)
            {
                continue;
            }

            //------------------------------
            // 該当する場合はパワーアップ
            //------------------------------
            MasterDataDefineLabel.StatusType nStatus = passiveParam.skill_powup_kind_status;
            float fPow = passiveParam.skill_powup_kind_rate;

            if (nStatus == MasterDataDefineLabel.StatusType.ATK)
            {
                fPow = GetDBRevisionValue(fPow);
                atkPow = AvoidErrorMultiple(atkPow, fPow);
            }
            break;
        }

        return (atkPow);
    }


    //------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによるブースト抽選回数増加
        @return		int				[ブースト抽選回数]
        @chenge		Developer 2015/09/12 ver300	リンクパッシブ対応
    */
    //------------------------------------------------------------------------
    static public int PassiveChkAddBoostChance()
    {
        int temp_chance = 0;

        //----------------------------------------
        // 最も有効な値を取得
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //----------------------------------------
        //--------------------------------
        // パーティ全員の効果を比較して一番有効なものを適用する
        //--------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < (int)GlobalDefine.PartyCharaIndex.MAX; ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null)
            {
                continue;
            }

            //------------------------------
            // パッシブスキルパラメータ取得
            //------------------------------
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //--------------------------------
            // 有効無効(該当スキルかチェック)
            //--------------------------------
            if (passiveParam.Is_skill_boost_chance_active() == false)
            {
                continue;
            }

            //--------------------------------
            // ブースト抽選回数
            //--------------------------------
            if (temp_chance > passiveParam.skill_boost_chance_count)
            {
                continue;
            }

            temp_chance = passiveParam.skill_boost_chance_count;
        }
        #endregion

        //--------------------------------
        // リンクキャラ全員の効果も比較して一番有効なものを適用する
        //--------------------------------
        #region ==== リンクパッシブ処理 ====
        for (int num = 0; num < (int)GlobalDefine.PartyCharaIndex.MAX; ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null
            || chara_once.m_LinkParam == null)
            {
                continue;
            }

            // リンクキャラ情報取得
            MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
            if (charaParam == null)
            {
                continue;
            }

            //------------------------------
            // リンクパッシブ取得
            //------------------------------
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //--------------------------------
            // 有効無効(該当スキルかチェック)
            //--------------------------------
            if (passiveParam.Is_skill_boost_chance_active() == false)
            {
                continue;
            }

            //--------------------------------
            // ブースト抽選回数
            //--------------------------------
            if (temp_chance > passiveParam.skill_boost_chance_count)
            {
                continue;
            }

            temp_chance = passiveParam.skill_boost_chance_count;
        }
        #endregion

        return (temp_chance);
    }

    //------------------------------------------------------------------------
    /*!
        @brief		追撃ダメージ取得	<static>
        @param[in]	CharaOnce		(cLeader)			リーダー		※未使用 @Developer 2015/07/28 ver290
        @param[in]	CharaOnce		(cFriend)			フレンド		※未使用 @Developer 2015/07/28 ver290
        @param[in]	CharaOnce		(chara)				攻撃者
        @param[in]	CharaOnce		(target)			対象者
        @param[in]	int				(nAilmentID)		状態異常管理ID
        @return		int				[追撃ダメージ値]
    */
    //------------------------------------------------------------------------
    static public int GetDamageFollow(CharaOnce cLeader, CharaOnce cFriend,
                                       CharaOnce chara, CharaOnce target,
                                       //StatusAilmentChara nAilmentID ) {
                                       BattleEnemy enemyParam,
                                       BattleSkillActivity skillActivity)
    {
        //----------------------------------------
        // キャラ情報無し
        //----------------------------------------
        if (chara == null || target == null)
        {
            return 0;
        }


        //----------------------------------------
        // 追撃スキル倍率
        //----------------------------------------
        float skillRate = 1.0f;
        if (!chara.m_bHasCharaMasterDataParam)
        {
            return 0;
        }


        MasterDataSkillLeader skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(chara.m_CharaMasterDataParam.skill_leader);
        if (skillLeader == null)
        {
            return 0;
        }

        skillRate = GetDBRevisionValue(skillLeader.skill_follow_atk_rate);

        //----------------------------------------
        // リーダースキル補正
        //----------------------------------------
        float leaderPow = 1.0f;
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex.LEADER, chara));
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex.FRIEND, chara));
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRateCondition(GlobalDefine.PartyCharaIndex.LEADER));
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRateCondition(GlobalDefine.PartyCharaIndex.FRIEND));

        //----------------------------------------
        // パッシブスキル補正
        //----------------------------------------
        GlobalDefine.PartyCharaIndex owner = GlobalDefine.PartyCharaIndex.MAX;	// ダミー（未使用）
        float passivePow = 1.0f;
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkSkillDamageRate(chara));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkKindKillerAtk(chara));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkDamageUPFloorPanel(ref owner));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkDamageUPElemNum(ref owner));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkDamageUPHandsNum(ref owner));

        //----------------------------------------
        // リンクパッシブスキル補正
        //----------------------------------------
        float fLinkPassivePow = 1.0f;
        uint unLinkUnitID = 0;
        if (chara.m_LinkParam != null)
        {
            unLinkUnitID = chara.m_LinkParam.m_CharaID;
        }
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkSkillDamageRate(chara, ESKILLTYPE.eLINKPASSIVE));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkKindKillerAtk(chara, ESKILLTYPE.eLINKPASSIVE));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkDamageUPFloorPanel(ref owner, ESKILLTYPE.eLINKPASSIVE, unLinkUnitID));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkDamageUPElemNum(ref owner, ESKILLTYPE.eLINKPASSIVE, unLinkUnitID));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkDamageUPHandsNum(ref owner, ESKILLTYPE.eLINKPASSIVE, unLinkUnitID));

        //----------------------------------------
        // 状態異常によるダメージ増減
        //----------------------------------------
        StatusAilmentChara nAilmentID = BattleParam.m_PlayerParty.m_Ailments.getAilment(chara.m_PartyCharaIndex);
        //		float fAilmentRate = StatusAilmentUtil.GetOffenceRate( nAilmentID );
        float fAilmentRate = 1.0f;
        // 攻撃力倍率
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, nAilmentID.GetOffenceRate());
        // 属性攻撃力倍率
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, nAilmentID.GetOffenceElementRate(chara.m_CharaMasterDataParam.element));
        // 種族攻撃力倍率(メイン)
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, nAilmentID.GetOffenceKindRate(chara.kind));
        // 種族攻撃力倍率(サブ)
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, nAilmentID.GetOffenceKindRate(chara.kind_sub));

        //----------------------------------------
        // v390 Developer add
        // 敵特性による補正の適用
        // 今回追加した「ダメ補正：指定スキル」以外の特性は後続の処理ではじかれてる
        //----------------------------------------
        float fEnemyAbilityRate = 1.0f;
        fEnemyAbilityRate = EnemyAbility.GetAbilityDamageRatePlayer(skillActivity, enemyParam);

        //----------------------------------------
        // 攻撃力
        //----------------------------------------
        float fCharaPow = chara.m_CharaPow;

        //----------------------------------------
        // 属性倍率
        //----------------------------------------
        float fElementRate = GetSkillElementRate(skillLeader.skill_follow_atk_element, target.m_CharaMasterDataParam.element);

        //----------------------------------------
        // コンボボーナス
        //----------------------------------------
        //		float fSkillCountRate = GetSkillCountRate( skillCount );

        //----------------------------------------
        // ダメージ計算
        //----------------------------------------
        float damageValue = 1.0f;
        damageValue = AvoidErrorMultiple(damageValue, fCharaPow);
        damageValue = AvoidErrorMultiple(damageValue, skillRate);
        damageValue = AvoidErrorMultiple(damageValue, fElementRate);
        damageValue = AvoidErrorMultiple(damageValue, leaderPow);
        damageValue = AvoidErrorMultiple(damageValue, passivePow);
        damageValue = AvoidErrorMultiple(damageValue, fLinkPassivePow);
        damageValue = AvoidErrorMultiple(damageValue, fAilmentRate);
        damageValue = AvoidErrorMultiple(damageValue, fEnemyAbilityRate); //v390 Developer add

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "追撃ダメージ値:" + ((int)damageValue).ToString()
            + " = " + ((int)fCharaPow).ToString() + "(キャラ基本攻撃力)"
            + " x" + InGameUtilBattle.getDebugPercentString(skillRate) + "%(スキル威力)"
            + " x" + InGameUtilBattle.getDebugPercentString(fElementRate) + "%(属性補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(leaderPow) + "%(リーダースキル補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(passivePow) + "%(パッシブスキル補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(fLinkPassivePow) + "%(リンクパッシブスキル補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(fAilmentRate) + "%(状態異常補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(fEnemyAbilityRate) + "%(敵特性補正)"
        );

        return (int)damageValue;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		リーダースキル：ダメージ補正系：常時発動
        @param[in]	MasterDataSkillLeader	(skillLeader)	リーダースキルマスター
        @param[in]	CharaOnce				(chara)			攻撃者
        @retval		float	[効果値]
        @note		ダメージが発生する場合、チェックする必要がある
    */
    //------------------------------------------------------------------------
    static private float ChkLeaderSkillDamageRateUsually(MasterDataSkillLeader skillLeader, CharaOnce atkChara)
    {
        float rate = 1.0f;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (skillLeader == null
        || atkChara == null)
        {
            return (rate);
        }


        //------------------------------
        // スキルによる分岐(処理内容は、GetLeaderSkillDamageRateより抜粋)
        //------------------------------
        if (skillLeader.Is_skill_powup_elem_active())
        {
            #region ==== 属性別攻撃力倍率 ====
            if (skillLeader.Is_skill_powup_elem_active()
            && skillLeader.skill_powup_elem_type == atkChara.m_CharaMasterDataParam.element)
            {
                if (skillLeader.skill_powup_elem_status == MasterDataDefineLabel.StatusType.ATK
                || skillLeader.skill_powup_elem_status == MasterDataDefineLabel.StatusType.ATK_AND_HP)
                {
                    float val = GetDBRevisionValue(skillLeader.skill_powup_elem_rate);
                    rate = AvoidErrorMultiple(rate, val);
                }
            }
            #endregion
        }
        else if (skillLeader.Is_skill_powup_kind_active())
        {
            #region ==== 種族別攻撃力倍率 ====
            if (skillLeader.Is_skill_powup_kind_active()
            && skillLeader.skill_powup_kind_type == atkChara.kind
            || skillLeader.skill_powup_kind_type == atkChara.kind_sub)
            {
                if (skillLeader.skill_powup_kind_status == MasterDataDefineLabel.StatusType.ATK
                || skillLeader.skill_powup_kind_status == MasterDataDefineLabel.StatusType.ATK_AND_HP)
                {
                    float val = GetDBRevisionValue(skillLeader.skill_powup_kind_rate);
                    rate = AvoidErrorMultiple(rate, val);
                }
            }
            #endregion
        }
        else if (skillLeader.Is_skill_hpfull_powup_active())
        {
            #region ==== 体力最大時攻撃力倍化 ====
            if (skillLeader.Is_skill_hpfull_powup_active())
            {
                BattleSceneUtil.MultiFloat val = BattleSceneUtil.getLifeRatePow(1.0f, 1.0f, GetDBRevisionValue(skillLeader.skill_hpfull_powup_rate));
                rate = AvoidErrorMultiple(rate, val.getValue(atkChara.m_PartyCharaIndex));
            }
            #endregion
        }
        else if (skillLeader.Is_skill_hpdown_powup_active())
        {
            #region ==== 体力減少時攻撃力倍化 ====
            if (skillLeader.Is_skill_hpdown_powup_active())
            {
                BattleSceneUtil.MultiFloat val = BattleSceneUtil.getLifeRatePow(0.0f, GetDBRevisionValue(skillLeader.skill_hpdown_powup_border), GetDBRevisionValue(skillLeader.skill_hpdown_powup_rate));
                rate = AvoidErrorMultiple(rate, val.getValue(atkChara.m_PartyCharaIndex));
            }
            #endregion
        }
        else if (skillLeader.skill_mekuri_powup_active == MasterDataDefineLabel.BoolType.ENABLE)
        {
        }
        else if (skillLeader.skill_type == MasterDataDefineLabel.LeaderSkillCategory.HP_ATK_POWUP)
        {
            #region ==== 指定属性、指定種族の攻撃力倍化 ====
            bool skill_result;

            if (skillLeader.Get_HP_ATK_POWUP_AND_CHK())
            {
                skill_result = true;
                //----------------------------------------
                //	ANDチェック
                //----------------------------------------
                //	種族チェック
                MasterDataDefineLabel.KindType[] HP_ATK_POWUP_KIND_array = skillLeader.GetArray_HP_ATK_POWUP_KIND();
                for (int i = 0; i < HP_ATK_POWUP_KIND_array.Length; i++)
                {
                    MasterDataDefineLabel.KindType kind_type = HP_ATK_POWUP_KIND_array[i];
                    if (kind_type == MasterDataDefineLabel.KindType.NONE)
                    {
                        continue;
                    }

                    if (kind_type == atkChara.kind
                    || kind_type == atkChara.kind_sub)
                    {
                        continue;
                    }

                    skill_result = false;
                    break;
                }

                //	属性チェック
                MasterDataDefineLabel.ElementType[] HP_ATK_POWUP_ELEM_array = skillLeader.GetArray_HP_ATK_POWUP_ELEM();
                for (int i = 0; i < HP_ATK_POWUP_ELEM_array.Length; i++)
                {
                    MasterDataDefineLabel.ElementType element_type = HP_ATK_POWUP_ELEM_array[i];
                    if (element_type == MasterDataDefineLabel.ElementType.NONE)
                    {
                        continue;
                    }

                    if (element_type == atkChara.m_CharaMasterDataParam.element)
                    {
                        continue;
                    }

                    skill_result = false;
                    break;
                }


            }
            else
            {
                skill_result = false;
                //----------------------------------------
                //	ORチェック
                //----------------------------------------
                //	種族チェック
                MasterDataDefineLabel.KindType[] HP_ATK_POWUP_KIND_array = skillLeader.GetArray_HP_ATK_POWUP_KIND();
                for (int i = 0; i < HP_ATK_POWUP_KIND_array.Length; i++)
                {
                    MasterDataDefineLabel.KindType kind_type = HP_ATK_POWUP_KIND_array[i];
                    if (kind_type == MasterDataDefineLabel.KindType.NONE)
                    {
                        continue;
                    }

                    if (kind_type != atkChara.kind
                    && kind_type != atkChara.kind_sub)
                    {
                        continue;
                    }

                    skill_result = true;
                    break;
                }

                //	属性チェック
                MasterDataDefineLabel.ElementType[] HP_ATK_POWUP_ELEM_array = skillLeader.GetArray_HP_ATK_POWUP_ELEM();
                for (int i = 0; i < HP_ATK_POWUP_ELEM_array.Length; i++)
                {
                    MasterDataDefineLabel.ElementType element_type = HP_ATK_POWUP_ELEM_array[i];
                    if (element_type == MasterDataDefineLabel.ElementType.NONE)
                    {
                        continue;
                    }

                    if (element_type != atkChara.m_CharaMasterDataParam.element)
                    {
                        continue;
                    }

                    skill_result = true;
                    break;
                }

            }

            //	種族チェック、属性チェックを通過したら効果を適用
            if (skill_result == true)
            {
                float val = GetDBRevisionValue(skillLeader.Get_HP_ATK_POWUP_ATK());
                rate = AvoidErrorMultiple(rate, val);
            }
            #endregion
        }

        return (rate);
    }


    //------------------------------------------------------------------------
    /*!
        @brief		リーダースキル：ダメージ補正系：条件発動
        @param[in]	MasterDataSkillLeader	(skillLeader)	リーダースキルマスター
        @retval		float	[効果値]
        @note		アクティブスキル以外の場合、チェックする必要がある
    */
    //------------------------------------------------------------------------
    static public float ChkLeaderSkillDamageRateCondition(MasterDataSkillLeader skillLeader)
    {
        BattleSceneManager battleMgr = BattleSceneManager.Instance;
        float rate = 1.0f;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (battleMgr == null
        || skillLeader == null)
        {
            return (rate);
        }


        //------------------------------
        // スキルによる分岐
        //------------------------------
        if (skillLeader.Is_skill_damageup_color_active())
        {
            #region ==== 指定色以上のスキルを成立させて攻撃すると、ダメージが指定倍率分UP ====
            int count = battleMgr.getActiveSkillElementCount();

            if (count < skillLeader.skill_damageup_color_count)
            {
                return (rate);
            }

            // DBから倍率を取得して倍率を計算
            float val = GetDBRevisionValue(skillLeader.skill_damageup_color_rate);
            rate = AvoidErrorMultiple(rate, val);
            #endregion
        }
        else if (skillLeader.Is_skill_damageup_hands_active())
        {
            #region ==== 指定HANDS以上でスキルを成立させて攻撃すると、ダメージが指定倍率分UP ====
            int count = battleMgr.getActiveSkillComboCount();
            if (count < skillLeader.skill_damageup_hands_count)
            {
                return (rate);
            }

            // DBから倍率を取得して倍率を計算
            float val = GetDBRevisionValue(skillLeader.skill_damageup_hands_rate);
            rate = AvoidErrorMultiple(rate, val);
            #endregion
        }
        else if (skillLeader.skill_type == MasterDataDefineLabel.LeaderSkillCategory.DMGUP_CONDITION_COST)
        {
            // ダメ補正：コスト条件
            float val = GetLeaderSkillDamageUPConditionCost(skillLeader);
            rate = AvoidErrorMultiple(rate, val);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief			リーダースキルによる体力倍率取得
        @param[in]		uint		(id)		リーダースキルid
        @param[in]		CharaOnce	(chara)		対象ユニット
    */
    //----------------------------------------------------------------------------
    public static float GetLeaderSkillHPRate(CharaOnce[] acParty, GlobalDefine.PartyCharaIndex charaIdx, CharaOnce cTarget)
    {
        float rate = 1.0f;

        if (acParty == null)
        {
            return rate;
        }

        if (cTarget == null)
        {
            return rate;
        }

        if (charaIdx < GlobalDefine.PartyCharaIndex.LEADER
        || charaIdx >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return rate;
        }

        // フレンドであればフレンド登録状態のチェック
        if (InGameUtil.ChkFriendRegisterStatus(charaIdx) == false)
        {
            return rate;
        }

        if (acParty[(int)charaIdx] == null)
        {
            return rate;
        }

        if (!acParty[(int)charaIdx].m_bHasCharaMasterDataParam)
        {
            return rate;
        }

        uint skillID = acParty[(int)charaIdx].m_CharaMasterDataParam.skill_leader;
        MasterDataSkillLeader skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(skillID);
        if (skillLeader == null)
        {
            return rate;
        }

        //------------------------------
        // スキル検索：リーダースキル複合化
        // @Developer 2015/07/31 ver290
        //------------------------------
        for (; skillLeader != null;)
        {
            // 該当属性の体力アップ
            if (skillLeader.Is_skill_powup_elem_active()
            && skillLeader.skill_powup_elem_type == cTarget.m_CharaMasterDataParam.element)
            {
                if (skillLeader.skill_powup_elem_status == MasterDataDefineLabel.StatusType.HP
                || skillLeader.skill_powup_elem_status == MasterDataDefineLabel.StatusType.ATK_AND_HP)
                {
                    rate *= skillLeader.skill_powup_elem_rate * 0.01f;
                }
            }

            // 該当種族の体力アップ
            if (skillLeader.Is_skill_powup_kind_active()
            && skillLeader.skill_powup_kind_type == cTarget.kind
            || skillLeader.skill_powup_kind_type == cTarget.kind_sub)
            {
                if (skillLeader.skill_powup_kind_status == MasterDataDefineLabel.StatusType.HP
                || skillLeader.skill_powup_kind_status == MasterDataDefineLabel.StatusType.ATK_AND_HP)
                {
                    rate *= (skillLeader.skill_powup_kind_rate * 0.01f);
                }
            }


            //	指定属性、指定種族の体力アップ
            if (skillLeader.skill_type == MasterDataDefineLabel.LeaderSkillCategory.HP_ATK_POWUP)
            {

                bool skill_result;

                if (skillLeader.Get_HP_ATK_POWUP_AND_CHK())
                {

                    skill_result = true;
                    //----------------------------------------
                    //	ANDチェック
                    //----------------------------------------
                    //	種族チェック
                    MasterDataDefineLabel.KindType[] HP_ATK_POWUP_KIND_array = skillLeader.GetArray_HP_ATK_POWUP_KIND();
                    for (int i = 0; i < HP_ATK_POWUP_KIND_array.Length; i++)
                    {
                        MasterDataDefineLabel.KindType kind_type = HP_ATK_POWUP_KIND_array[i];

                        if (kind_type == MasterDataDefineLabel.KindType.NONE)
                        {
                            continue;
                        }

                        if (kind_type == cTarget.kind
                        || kind_type == cTarget.kind_sub)
                        {
                            continue;
                        }

                        skill_result = false;
                        break;
                    }

                    //	属性チェック
                    MasterDataDefineLabel.ElementType[] HP_ATK_POWUP_ELEM_array = skillLeader.GetArray_HP_ATK_POWUP_ELEM();
                    for (int i = 0; i < HP_ATK_POWUP_ELEM_array.Length; i++)
                    {
                        MasterDataDefineLabel.ElementType element_type = HP_ATK_POWUP_ELEM_array[i];

                        if (element_type == MasterDataDefineLabel.ElementType.NONE)
                        {
                            continue;
                        }

                        if (element_type == cTarget.m_CharaMasterDataParam.element)
                        {
                            continue;
                        }

                        skill_result = false;
                        break;
                    }


                }
                else
                {

                    skill_result = false;
                    //----------------------------------------
                    //	ORチェック
                    //----------------------------------------
                    //	種族チェック
                    MasterDataDefineLabel.KindType[] HP_ATK_POWUP_KIND_array = skillLeader.GetArray_HP_ATK_POWUP_KIND();
                    for (int i = 0; i < HP_ATK_POWUP_KIND_array.Length; i++)
                    {
                        MasterDataDefineLabel.KindType kind_type = HP_ATK_POWUP_KIND_array[i];

                        if (kind_type == MasterDataDefineLabel.KindType.NONE)
                        {
                            continue;
                        }

                        if (kind_type != cTarget.kind
                        && kind_type != cTarget.kind_sub)
                        {
                            continue;
                        }

                        skill_result = true;
                        break;
                    }

                    //	属性チェック
                    MasterDataDefineLabel.ElementType[] HP_ATK_POWUP_ELEM_array = skillLeader.GetArray_HP_ATK_POWUP_ELEM();
                    for (int i = 0; i < HP_ATK_POWUP_ELEM_array.Length; i++)
                    {
                        MasterDataDefineLabel.ElementType element_type = HP_ATK_POWUP_ELEM_array[i];

                        if (element_type == MasterDataDefineLabel.ElementType.NONE)
                        {
                            continue;
                        }

                        if (element_type != cTarget.m_CharaMasterDataParam.element)
                        {
                            continue;
                        }

                        skill_result = true;
                        break;
                    }

                }

                //	種族チェック、属性チェックを通過したら効果を適用
                if (skill_result == true)
                {
                    float val = InGameUtilBattle.GetDBRevisionValue(skillLeader.Get_HP_ATK_POWUP_HP());
                    rate = InGameUtilBattle.AvoidErrorMultiple(val, rate);
                }

            }


            //------------------------------
            // 追加リーダースキルを取得
            //------------------------------
            skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(skillLeader.add_fix_id);
        }

#if BUILD_TYPE_DEBUG && UNITY_EDITOR && OUTPUT_INGAME_LOG

    Debug.LogError( "-------------------------------------------------" );
    Debug.LogError( "LeaderSkill_" + "hp_rate_" + rate );

#endif // BUILD_TYPE_DEBUG && UNITY_EDITOR

        return rate;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		スキル効果によるダメージ増減倍率の取得		<static>
        @param[in]	int				(charaIdx)	スキル所持者インデックス
        @param[in]	CharaOnce		(chara)		攻撃者
        @return		float			[ダメージ倍率]
        @note		InGame関連以外からの呼出しは行わないでください。
    */
    //------------------------------------------------------------------------
    static public float GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex charaIdx, CharaOnce chara)
    {
        float rate = 1.0f;

        if (BattleParam.m_PlayerParty == null)
        {
            return rate;
        }

        if (charaIdx < 0 || charaIdx >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return rate;
        }

        // フレンドであればフレンド登録状態のチェック
        if (InGameUtil.ChkFriendRegisterStatus(charaIdx) == false)
        {
            return rate;
        }

        CharaOnce leader = BattleParam.m_PlayerParty.getPartyMember(charaIdx, CharaParty.CharaCondition.SKILL_LEADER);
        if (leader == null)
        {
            return rate;
        }

        MasterDataParamChara charaParam = leader.m_CharaMasterDataParam;
        if (charaParam == null)
        {
            return rate;
        }

        MasterDataSkillLeader skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(charaParam.skill_leader);

        //------------------------------
        // スキル検索：リーダースキル複合化
        // @Developer 2015/07/31 ver290
        //------------------------------
        float val = 1.0f;
        for (; skillLeader != null;)
        {
            // このチェック関数は、下記の処理をまとめたもの
            val = ChkLeaderSkillDamageRateUsually(skillLeader, chara);
            rate = AvoidErrorMultiple(rate, val);

            //------------------------------
            // 追加リーダースキルを取得
            //------------------------------
            skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(skillLeader.add_fix_id);
        }

#if BUILD_TYPE_DEBUG && UNITY_EDITOR && OUTPUT_INGAME_LOG

    Debug.LogError( "-------------------------------------------------" );
    Debug.LogError( "LeaderSkill_" + "hp_rate_" + rate );

#endif // BUILD_TYPE_DEBUG && UNITY_EDITOR

        return rate;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		リーダースキル効果によるダメージ増減倍率の取得：条件発動	<static>
        @param[in]	int		(charaIdx)	スキル所持者インデックス
        @return		float	[ダメージ倍率]
        @note		InGame関連以外からの呼出しは行わないでください。
    */
    //------------------------------------------------------------------------
    static public float GetLeaderSkillDamageRateCondition(GlobalDefine.PartyCharaIndex charaIdx)
    {
        float rate = 1.0f;

        if (BattleParam.m_PlayerParty == null)
        {
            return (rate);
        }

        if (charaIdx < GlobalDefine.PartyCharaIndex.LEADER
        || charaIdx > GlobalDefine.PartyCharaIndex.FRIEND)
        {
            return (rate);
        }

        // フレンドであればフレンド登録状態のチェック(廃止された処理だが、既存処理の流れに合わす)
        if (InGameUtil.ChkFriendRegisterStatus(charaIdx) == false)
        {
            return (rate);
        }

        CharaOnce leader = BattleParam.m_PlayerParty.getPartyMember(charaIdx, CharaParty.CharaCondition.SKILL_LEADER);
        if (leader == null)
        {
            return (rate);
        }

        MasterDataParamChara charaParam = leader.m_CharaMasterDataParam;
        if (charaParam == null)
        {
            return (rate);
        }


        //------------------------------
        // スキル検索
        //------------------------------
        MasterDataSkillLeader skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(charaParam.skill_leader);
        float val = 1.0f;
        for (; skillLeader != null;)
        {
            val = ChkLeaderSkillDamageRateCondition(skillLeader);
            rate = AvoidErrorMultiple(rate, val);

            //------------------------------
            // 追加リーダースキルを取得
            //------------------------------
            skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(skillLeader.add_fix_id);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		リーダースキルによるダメージ軽減倍率の取得		<static>
        @param[in]	int				(charaIdx)	リーダースキル所持者インデックス
        @param[in]	int				(element)	攻撃属性
        @param[in]	int				(enemyElem)	敵属性
        @return		float			[ダメージ軽減倍率]
        @note		InGame関連以外からの呼出しは行わないでください。
    */
    //----------------------------------------------------------------------------
    static public float GetLeaderSkillDamageDecrease(GlobalDefine.PartyCharaIndex leader_charaIdx, MasterDataDefineLabel.ElementType element, MasterDataDefineLabel.ElementType enemyElem, GlobalDefine.PartyCharaIndex target_player)
    {
        float rate = 1.0f;

        //------------------------------
        // エラーチェック
        //------------------------------
        #region ==== nullチェックや、範囲チェック ====
        if (leader_charaIdx < GlobalDefine.PartyCharaIndex.LEADER
        || leader_charaIdx >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return rate;
        }

        // フレンドであればフレンド登録状態のチェック
        if (InGameUtil.ChkFriendRegisterStatus(leader_charaIdx) == false)
        {
            return rate;
        }

        CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember(leader_charaIdx, CharaParty.CharaCondition.SKILL_LEADER);
        if (chara == null)
        {
            return rate;
        }

        MasterDataParamChara charaParam = chara.m_CharaMasterDataParam;
        if (charaParam == null)
        {
            return rate;
        }
        #endregion

        MasterDataSkillLeader skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(charaParam.skill_leader);
        if (skillLeader == null)
        {
            return rate;
        }
        //------------------------------
        // スキルによる分岐
        //------------------------------
        if (skillLeader.Is_skill_decline_dmg_active())
        {
            #region ==== 攻撃属性でのダメージ減衰 ====
            if (skillLeader.skill_decline_dmg_element == element
            || skillLeader.skill_decline_dmg_element == MasterDataDefineLabel.ElementType.NONE)
            {
                float val = GetDBRevisionValue(skillLeader.skill_decline_dmg_rate);
                rate = AvoidErrorMultiple(rate, val);
            }
            #endregion
        }
        else if (skillLeader.Is_skill_hpfull_guard_active())
        {
            #region ==== 体力最大時ダメージ減衰 ====
            if (BattleParam.m_PlayerParty.m_HPCurrent.getValue(target_player) == BattleParam.m_PlayerParty.m_HPMax.getValue(target_player))
            {
                float val = GetDBRevisionValue(skillLeader.skill_hpfull_guard_rate);
                rate = AvoidErrorMultiple(rate, val);
            }
            #endregion
        }
        else if (skillLeader.skill_type == MasterDataDefineLabel.LeaderSkillCategory.DMG_ENEMY_ELEM)
        {
            // 被ダメ補正：エネミー属性
            float val = GetLeaderSkillDamageEnemyElement(skillLeader, enemyElem);
            rate = AvoidErrorMultiple(rate, val);
        }

        return rate;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		リーダースキルによるカウントダウン増減
        @param[in]	int		(charaIdx)		キャラID
        @return		float	[カウントダウン増減数]
    */
    //----------------------------------------------------------------------------
    static public float GetLeaderSkillCountDown(GlobalDefine.PartyCharaIndex charaIdx)
    {
        float countDown = 0.0f;

        if (charaIdx < GlobalDefine.PartyCharaIndex.LEADER
        || charaIdx >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return countDown;
        }

        // フレンドであればフレンド登録状態のチェック
        if (InGameUtil.ChkFriendRegisterStatus(charaIdx) == false)
        {
            return countDown;
        }

        CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember(charaIdx, CharaParty.CharaCondition.SKILL_LEADER);
        if (chara == null)
        {
            return countDown;
        }

        MasterDataParamChara charaParam = chara.m_CharaMasterDataParam;
        if (charaParam == null)
        {
            return countDown;
        }

        MasterDataSkillLeader skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(charaParam.skill_leader);
        if (skillLeader == null)
        {
            return countDown;
        }

        //----------------------------------------
        // カウントダウン増減が有効であれば変化量を保存
        //----------------------------------------
        if (skillLeader.Is_skill_quick_time_active())
        {
            countDown = skillLeader.skill_quick_time_second;
        }
        return countDown;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		リーダースキルによる戦闘中の回復量取得
        @param[in]	int		(charaIdx)		キャラID
        @return		int		[回復倍率取得]
    */
    //----------------------------------------------------------------------------
    static public float GetLeaderSkillHealBattle(GlobalDefine.PartyCharaIndex charaIdx)
    {
        float rate = 0.0f;

        if (charaIdx < GlobalDefine.PartyCharaIndex.LEADER
        || charaIdx >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return rate;
        }

        // フレンドであればフレンド登録状態のチェック
        if (InGameUtil.ChkFriendRegisterStatus(charaIdx) == false)
        {
            return rate;
        }

        CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember(charaIdx, CharaParty.CharaCondition.SKILL_LEADER);
        if (chara == null)
        {
            return rate;
        }

        MasterDataParamChara charaParam = chara.m_CharaMasterDataParam;
        if (charaParam == null)
        {
            return rate;
        }

        MasterDataSkillLeader skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(charaParam.skill_leader);
        if (skillLeader == null)
        {
            return rate;
        }

        // 割合回復
        if (skillLeader.Is_skill_recovery_battle_active())
        {
            rate = GetDBRevisionValue(skillLeader.skill_recovery_battle_rate);
        }

        return rate;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		ふんばりチェック				<static>
        @param[in]	int		(charaIdx)		発動キャラインデックス
        @param[in]	int		(nowhp)			パーティの現在体力
        @param[in]	int		(maxhp)			パーティの最大体力
        @param[in]	int		(hp)			ダメージを受ける前の体力
        @retval		bool	[ふんばり/死]
    */
    //----------------------------------------------------------------------------
    static public BattleSceneUtil.MultiInt GetLeaderSkillFunbari(GlobalDefine.PartyCharaIndex charaIdx, BattleSceneUtil.MultiInt nowhp, BattleSceneUtil.MultiInt maxhp, BattleSceneUtil.MultiInt before_hp)
    {
        BattleSceneUtil.MultiInt ret_val = new BattleSceneUtil.MultiInt();

        if (charaIdx < 0 || charaIdx >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return ret_val;
        }

        // フレンドであればフレンド登録状態のチェック
        if (InGameUtil.ChkFriendRegisterStatus(charaIdx) == false)
        {
            return ret_val;
        }

        CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember(charaIdx, CharaParty.CharaCondition.SKILL_LEADER);
        if (chara == null)
        {
            return ret_val;
        }

        if (before_hp.getValue(charaIdx) <= 0)
        {
            return ret_val;
        }

        MasterDataParamChara charaParam = chara.m_CharaMasterDataParam;
        if (charaParam == null)
        {
            return ret_val;
        }

        MasterDataSkillLeader skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(charaParam.skill_leader);
        if (skillLeader == null)
        {
            return ret_val;
        }

        if (skillLeader.Is_skill_funbari_active() == false)
        {
            return ret_val;
        }

        float rate = GetDBRevisionValue(skillLeader.skill_funbari_border);
        for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
        {
            if (before_hp.getValue((GlobalDefine.PartyCharaIndex)idx) >= (int)AvoidErrorMultiple(maxhp.getValue((GlobalDefine.PartyCharaIndex)idx), rate))
            {
                ret_val.setValue((GlobalDefine.PartyCharaIndex)idx, 1);
            }
        }

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "即死回避効果"
            + " (LEADERスキル:" + charaIdx.ToString() + ")"
            + "(HP" + InGameUtilBattle.getDebugPercentString(rate) + "%以上):"
            + ((ret_val.getValue(GlobalDefine.PartyCharaIndex.LEADER) > 0) ? "LEADER " : "")
            + ((ret_val.getValue(GlobalDefine.PartyCharaIndex.MOB_1) > 0) ? "MOB_1 " : "")
            + ((ret_val.getValue(GlobalDefine.PartyCharaIndex.MOB_2) > 0) ? "MOB_2 " : "")
            + ((ret_val.getValue(GlobalDefine.PartyCharaIndex.MOB_3) > 0) ? "MOB_3 " : "")
            + ((ret_val.getValue(GlobalDefine.PartyCharaIndex.FRIEND) > 0) ? "FRIEND " : "")
        );

        return ret_val;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		指定属性の手札を指定属性に変換する		<static>
        @param[in]	int			(charaIdx)				発動キャラインデックス
        @param[in]	int			(element_hand)			変換元手札
        @return		int			[変換後]
    */
    //----------------------------------------------------------------------------
    static public MasterDataDefineLabel.ElementType GetLeaderSkillTransformCard(GlobalDefine.PartyCharaIndex charaIdx, MasterDataDefineLabel.ElementType element_hand)
    {
        MasterDataDefineLabel.ElementType element = element_hand;

        if (charaIdx < 0 || charaIdx >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return element;
        }

        // フレンドであればフレンド登録状態のチェック
        if (InGameUtil.ChkFriendRegisterStatus(charaIdx) == false)
        {
            return element;
        }

        // スキルの所持者を取得
        CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember(charaIdx, CharaParty.CharaCondition.SKILL_LEADER);
        if (chara == null)
        {
            return element;
        }

        MasterDataParamChara charaParam = chara.m_CharaMasterDataParam;
        if (charaParam == null)
        {
            return element;
        }

        // スキル情報の取得
        MasterDataSkillLeader skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(charaParam.skill_leader);
        if (skillLeader == null)
        {
            return element;
        }

        if (skillLeader.Is_skill_transform_card_active() == false)
        {
            return element;
        }

        // スキルの発動条件判定
        if (skillLeader.skill_transform_card_root == element)
        {
            // 条件に該当するなら、設定された属性に変換
            element = skillLeader.skill_transform_card_dest;
        }

        return element;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		リーダースキル：ダメ補正：コスト条件		<static>
        @param[in]	MasterDataSkillLeader	(skillLeader)	リーダースキルマスター
        @retval		float	[倍率]
    */
    //------------------------------------------------------------------------
    static private float GetLeaderSkillDamageUPConditionCost(MasterDataSkillLeader skillLeader)
    {
        BattleSceneManager battleMgr = BattleSceneManager.Instance;
        float rate = 1.0f;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (battleMgr == null
        || skillLeader == null)
        {
            return (rate);
        }
        MasterDataDefineLabel.SkillCostType[] DMGUP_CONDI_COST_COST_array = skillLeader.GetArray_DMGUP_CONDI_COST_COST();
        MasterDataDefineLabel.ConditionType[] DMGUP_CONDI_COST_COND_array = skillLeader.GetArray_DMGUP_CONDI_COST_COND();

        //----------------------------------------
        // ダメ補正：コスト条件(属性指定＆パネル数指定)処理
        //----------------------------------------

        //------------------------------
        // 判定用変数準備
        //------------------------------
        bool[] chkAndOr = new bool[DMGUP_CONDI_COST_COND_array.Length];
        int orCnt = 0;

        // スキル重複チェック用[発動スキルID, 発動数]：ノーマルスキルは、1キャラで最大2所持(汎用回復などもあるため、多めに確保)
        uint[] skillIDArray = new uint[(int)GlobalDefine.PartyCharaIndex.MAX * 3];
        int[] skillActiveNum = new int[(int)GlobalDefine.PartyCharaIndex.MAX * 3];		// 同スキルの発動数

        // 初期化
        for (int num = 0; num < chkAndOr.Length; ++num)
        {
            chkAndOr[num] = false;
        }

        //----------------------------------------
        // 発動スキルを全チェック：重複スキルは省く
        //----------------------------------------
        int skillCnt = 0;
        bool entry = false;
        int active_skill_count = battleMgr.PRIVATE_FIELD.getFormedActiveSkillCount();
        for (int skillNum = 0; skillNum < active_skill_count; ++skillNum)
        {
            // ノーマルスキルチェック
            uint active_skill_id = battleMgr.PRIVATE_FIELD.getFormedActiveSkillID(skillNum);
            if (active_skill_id == 0)
            {
                continue;
            }

            MasterDataSkillActive skill_active_master = BattleParam.m_MasterDataCache.useSkillActive(active_skill_id);
            if (skill_active_master == null
                || skill_active_master.isAlwaysResurrectSkill()
            )
            {
                continue;
            }

            // 登録済みかチェック
            for (int chkNum = 0; chkNum < skillIDArray.Length; ++chkNum)
            {
                if (skillIDArray[chkNum] == 0)
                {
                    entry = true;
                    break;
                }
                else if (skillIDArray[chkNum] == active_skill_id)
                {
                    // 発動数加算
                    ++skillActiveNum[chkNum];

                    entry = false;
                    break;
                }
            }

            // 登録判定
            if (entry == false)
            {
                continue;
            }

            // 発動数加算
            ++skillActiveNum[skillCnt];

            // スキルIDを登録
            skillIDArray[skillCnt] = active_skill_id;
            ++skillCnt;
        }


        //----------------------------------------
        // 発動スキルを全チェック
        //----------------------------------------
        for (int skillNum = 0; skillNum < skillCnt; ++skillNum)
        {
            // コスト条件チェック
            for (int num = 0; num < DMGUP_CONDI_COST_COST_array.Length; ++num)
            {
                MasterDataDefineLabel.SkillCostType skill_cost_type = DMGUP_CONDI_COST_COST_array[num];
                MasterDataDefineLabel.ConditionType condition_type = DMGUP_CONDI_COST_COND_array[num];
                // 設定チェック
                if (skill_cost_type == MasterDataDefineLabel.SkillCostType.NONE
                || condition_type == MasterDataDefineLabel.ConditionType.NONE)
                {
                    continue;
                }

                // 条件達成済みなら次へ
                if (chkAndOr[num] == true)
                {
                    continue;
                }

                // コストチェック
                if (CheckSkillCost(skillIDArray[skillNum], skill_cost_type) == false)
                {
                    continue;
                }

                // 条件分岐
                switch (condition_type)
                {
                    case MasterDataDefineLabel.ConditionType.AND:
                    case MasterDataDefineLabel.ConditionType.OR:
                        --skillActiveNum[skillNum];	// 発動数減算
                        chkAndOr[num] = true;
                        break;
                    case MasterDataDefineLabel.ConditionType.BAN:
                        return (rate);

                    case MasterDataDefineLabel.ConditionType.NONE:
                    case MasterDataDefineLabel.ConditionType.MAX:	// 2016/10/11 Developer CATEGORY_MAX だったのを Condition.MAX へ変更.
                    default:
                        break;
                }

                // 次以降の条件検索(ここまでこれるなら、ANDかOR)
                for (int numNext = num + 1; numNext < DMGUP_CONDI_COST_COST_array.Length; ++numNext)
                {
                    // 発動数が残っていない場合
                    if (skillActiveNum[skillNum] <= 0)
                    {
                        break;
                    }

                    // 同じコスト条件ではない場合
                    if (DMGUP_CONDI_COST_COST_array[numNext] != skill_cost_type)
                    {
                        continue;
                    }

                    --skillActiveNum[skillNum];	// 発動数減算
                    chkAndOr[numNext] = true;
                }

                // 条件を満たせた場合、次のスキルへ(1スキル、1条件)
                break;
            }

        }


        //----------------------------------------
        // 発動チェック
        //----------------------------------------
        for (int num = 0; num < DMGUP_CONDI_COST_COND_array.Length; ++num)
        {
            MasterDataDefineLabel.ConditionType condition_type = DMGUP_CONDI_COST_COND_array[num];
            switch (condition_type)
            {
                // AND条件を満たせていない場合は、発動不可
                case MasterDataDefineLabel.ConditionType.AND:
                    if (chkAndOr[num] == true)
                    {
                        continue;
                    }

                    return (rate);

                // OR条件をカウントする(重複達成対策)
                case MasterDataDefineLabel.ConditionType.OR:
                    if (chkAndOr[num] == true)
                    {
                        ++orCnt;
                    }
                    break;
            }
        }

        // ここでOR条件を満たしていた場合、発動
        if (orCnt >= skillLeader.Get_DMGUP_CONDI_COST_OR_NUM())
        {
            float val = GetDBRevisionValue(skillLeader.Get_DMGUP_CONDI_COST_RATE());
            rate = AvoidErrorMultiple(rate, val);
        }

        return (rate);
    }

    //------------------------------------------------------------------------
    /*!
        @brief		コストチェック	<static>
        @param[in]	uint		(skillID)		ノーマルスキルID
        @param[in]	int			(cost)			コスト条件
        @retval		bool	[条件可/条件不可]
    */
    //------------------------------------------------------------------------
    private static bool CheckSkillCost(uint skillID, MasterDataDefineLabel.SkillCostType cost)
    {
        bool result = false;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (skillID == 0)
        {
            return (result);
        }

        // ノーマルスキルマスターを取得
        MasterDataSkillActive normalMaster = BattleParam.m_MasterDataCache.useSkillActive(skillID);
        if (normalMaster == null)
        {
            return (result);
        }


        //------------------------------
        // 判定要素を取得
        //------------------------------
        MasterDataDefineLabel.ElementType skillElem = normalMaster.skill_element;
        MasterDataDefineLabel.ElementType[] skillCost = {
                            normalMaster.cost1,
                            normalMaster.cost2,
                            normalMaster.cost3,
                            normalMaster.cost4,
                            normalMaster.cost5,
                          };

        //------------------------------
        // コスト数をカウント
        //------------------------------
        int costNum = 0;
        for (int num = 0; num < skillCost.Length; ++num)
        {
            // 無、炎、水、光、闇、風、回復以外は、カウントしない
            if (skillCost[num] < MasterDataDefineLabel.ElementType.NAUGHT
            || skillCost[num] > MasterDataDefineLabel.ElementType.HEAL)
            {
                continue;
            }

            ++costNum;
        }


        //------------------------------
        // コスト条件分岐
        //------------------------------
        switch (cost)
        {
            case MasterDataDefineLabel.SkillCostType.FIRE: if (skillElem == MasterDataDefineLabel.ElementType.FIRE) { result = true; } break;		// 炎属性
            case MasterDataDefineLabel.SkillCostType.WATER: if (skillElem == MasterDataDefineLabel.ElementType.WATER) { result = true; } break;		// 水属性
            case MasterDataDefineLabel.SkillCostType.WIND: if (skillElem == MasterDataDefineLabel.ElementType.WIND) { result = true; } break;		// 風属性
            case MasterDataDefineLabel.SkillCostType.LIGHT: if (skillElem == MasterDataDefineLabel.ElementType.LIGHT) { result = true; } break;		// 光属性
            case MasterDataDefineLabel.SkillCostType.DARK: if (skillElem == MasterDataDefineLabel.ElementType.DARK) { result = true; } break;		// 闇属性
            case MasterDataDefineLabel.SkillCostType.NAUGHT: if (skillElem == MasterDataDefineLabel.ElementType.NAUGHT) { result = true; } break;		// 無属性
            case MasterDataDefineLabel.SkillCostType.HEAL: if (skillElem == MasterDataDefineLabel.ElementType.HEAL) { result = true; } break;		// 回復属性(汎用回復)
            case MasterDataDefineLabel.SkillCostType.PANEL_1: if (costNum == 1 && skillElem != MasterDataDefineLabel.ElementType.HEAL) { result = true; } break;		// 1パネル(回復属性は除く)
            case MasterDataDefineLabel.SkillCostType.PANEL_2: if (costNum == 2 && skillElem != MasterDataDefineLabel.ElementType.HEAL) { result = true; } break;		// 2パネル(回復属性は除く)
            case MasterDataDefineLabel.SkillCostType.PANEL_3: if (costNum == 3 && skillElem != MasterDataDefineLabel.ElementType.HEAL) { result = true; } break;		// 3パネル(回復属性は除く)
            case MasterDataDefineLabel.SkillCostType.PANEL_4: if (costNum == 4 && skillElem != MasterDataDefineLabel.ElementType.HEAL) { result = true; } break;		// 4パネル(回復属性は除く)
            case MasterDataDefineLabel.SkillCostType.PANEL_5: if (costNum == 5 && skillElem != MasterDataDefineLabel.ElementType.HEAL) { result = true; } break;		// 5パネル(回復属性は除く)
            case MasterDataDefineLabel.SkillCostType.FIRE_1: if (costNum == 1 && skillElem == MasterDataDefineLabel.ElementType.FIRE) { result = true; } break;		// 炎属性の1パネルNS
            case MasterDataDefineLabel.SkillCostType.FIRE_2: if (costNum == 2 && skillElem == MasterDataDefineLabel.ElementType.FIRE) { result = true; } break;		// 炎属性の2パネルNS
            case MasterDataDefineLabel.SkillCostType.FIRE_3: if (costNum == 3 && skillElem == MasterDataDefineLabel.ElementType.FIRE) { result = true; } break;		// 炎属性の3パネルNS
            case MasterDataDefineLabel.SkillCostType.FIRE_4: if (costNum == 4 && skillElem == MasterDataDefineLabel.ElementType.FIRE) { result = true; } break;		// 炎属性の4パネルNS
            case MasterDataDefineLabel.SkillCostType.FIRE_5: if (costNum == 5 && skillElem == MasterDataDefineLabel.ElementType.FIRE) { result = true; } break;		// 炎属性の5パネルNS
            case MasterDataDefineLabel.SkillCostType.WATER_1: if (costNum == 1 && skillElem == MasterDataDefineLabel.ElementType.WATER) { result = true; } break;		// 水属性の1パネルNS
            case MasterDataDefineLabel.SkillCostType.WATER_2: if (costNum == 2 && skillElem == MasterDataDefineLabel.ElementType.WATER) { result = true; } break;		// 水属性の2パネルNS
            case MasterDataDefineLabel.SkillCostType.WATER_3: if (costNum == 3 && skillElem == MasterDataDefineLabel.ElementType.WATER) { result = true; } break;		// 水属性の3パネルNS
            case MasterDataDefineLabel.SkillCostType.WATER_4: if (costNum == 4 && skillElem == MasterDataDefineLabel.ElementType.WATER) { result = true; } break;		// 水属性の4パネルNS
            case MasterDataDefineLabel.SkillCostType.WATER_5: if (costNum == 5 && skillElem == MasterDataDefineLabel.ElementType.WATER) { result = true; } break;		// 水属性の5パネルNS
            case MasterDataDefineLabel.SkillCostType.WIND_1: if (costNum == 1 && skillElem == MasterDataDefineLabel.ElementType.WIND) { result = true; } break;		// 風属性の1パネルNS
            case MasterDataDefineLabel.SkillCostType.WIND_2: if (costNum == 2 && skillElem == MasterDataDefineLabel.ElementType.WIND) { result = true; } break;		// 風属性の2パネルNS
            case MasterDataDefineLabel.SkillCostType.WIND_3: if (costNum == 3 && skillElem == MasterDataDefineLabel.ElementType.WIND) { result = true; } break;		// 風属性の3パネルNS
            case MasterDataDefineLabel.SkillCostType.WIND_4: if (costNum == 4 && skillElem == MasterDataDefineLabel.ElementType.WIND) { result = true; } break;		// 風属性の4パネルNS
            case MasterDataDefineLabel.SkillCostType.WIND_5: if (costNum == 5 && skillElem == MasterDataDefineLabel.ElementType.WIND) { result = true; } break;		// 風属性の5パネルNS
            case MasterDataDefineLabel.SkillCostType.LIGHT_1: if (costNum == 1 && skillElem == MasterDataDefineLabel.ElementType.LIGHT) { result = true; } break;		// 光属性の1パネルNS
            case MasterDataDefineLabel.SkillCostType.LIGHT_2: if (costNum == 2 && skillElem == MasterDataDefineLabel.ElementType.LIGHT) { result = true; } break;		// 光属性の2パネルNS
            case MasterDataDefineLabel.SkillCostType.LIGHT_3: if (costNum == 3 && skillElem == MasterDataDefineLabel.ElementType.LIGHT) { result = true; } break;		// 光属性の3パネルNS
            case MasterDataDefineLabel.SkillCostType.LIGHT_4: if (costNum == 4 && skillElem == MasterDataDefineLabel.ElementType.LIGHT) { result = true; } break;		// 光属性の4パネルNS
            case MasterDataDefineLabel.SkillCostType.LIGHT_5: if (costNum == 5 && skillElem == MasterDataDefineLabel.ElementType.LIGHT) { result = true; } break;		// 光属性の5パネルNS
            case MasterDataDefineLabel.SkillCostType.DARK_1: if (costNum == 1 && skillElem == MasterDataDefineLabel.ElementType.DARK) { result = true; } break;		// 闇属性の1パネルNS
            case MasterDataDefineLabel.SkillCostType.DARK_2: if (costNum == 2 && skillElem == MasterDataDefineLabel.ElementType.DARK) { result = true; } break;		// 闇属性の2パネルNS
            case MasterDataDefineLabel.SkillCostType.DARK_3: if (costNum == 3 && skillElem == MasterDataDefineLabel.ElementType.DARK) { result = true; } break;		// 闇属性の3パネルNS
            case MasterDataDefineLabel.SkillCostType.DARK_4: if (costNum == 4 && skillElem == MasterDataDefineLabel.ElementType.DARK) { result = true; } break;		// 闇属性の4パネルNS
            case MasterDataDefineLabel.SkillCostType.DARK_5: if (costNum == 5 && skillElem == MasterDataDefineLabel.ElementType.DARK) { result = true; } break;		// 闇属性の5パネルNS
            case MasterDataDefineLabel.SkillCostType.NAUGHT_1: if (costNum == 1 && skillElem == MasterDataDefineLabel.ElementType.NAUGHT) { result = true; } break;		// 無属性の1パネルNS
            case MasterDataDefineLabel.SkillCostType.NAUGHT_2: if (costNum == 2 && skillElem == MasterDataDefineLabel.ElementType.NAUGHT) { result = true; } break;		// 無属性の2パネルNS
            case MasterDataDefineLabel.SkillCostType.NAUGHT_3: if (costNum == 3 && skillElem == MasterDataDefineLabel.ElementType.NAUGHT) { result = true; } break;		// 無属性の3パネルNS
            case MasterDataDefineLabel.SkillCostType.NAUGHT_4: if (costNum == 4 && skillElem == MasterDataDefineLabel.ElementType.NAUGHT) { result = true; } break;		// 無属性の4パネルNS
            case MasterDataDefineLabel.SkillCostType.NAUGHT_5: if (costNum == 5 && skillElem == MasterDataDefineLabel.ElementType.NAUGHT) { result = true; } break;		// 無属性の5パネルNS
            case MasterDataDefineLabel.SkillCostType.HEAL_1: if (costNum == 1 && skillElem == MasterDataDefineLabel.ElementType.HEAL) { result = true; } break;		// 回復属性の1パネル(汎用回復)
            case MasterDataDefineLabel.SkillCostType.HEAL_2: if (costNum == 2 && skillElem == MasterDataDefineLabel.ElementType.HEAL) { result = true; } break;		// 回復属性の2パネル(汎用回復)
            case MasterDataDefineLabel.SkillCostType.HEAL_3: if (costNum == 3 && skillElem == MasterDataDefineLabel.ElementType.HEAL) { result = true; } break;		// 回復属性の3パネル(汎用回復)
            case MasterDataDefineLabel.SkillCostType.HEAL_4: if (costNum == 4 && skillElem == MasterDataDefineLabel.ElementType.HEAL) { result = true; } break;		// 回復属性の4パネル(汎用回復)
            case MasterDataDefineLabel.SkillCostType.HEAL_5: if (costNum == 5 && skillElem == MasterDataDefineLabel.ElementType.HEAL) { result = true; } break;		// 回復属性の5パネル(汎用回復)

            case MasterDataDefineLabel.SkillCostType.NONE:
            case MasterDataDefineLabel.SkillCostType.MAX:
            default:
                break;
        }

        return (result);
    }

    //------------------------------------------------------------------------
    /*!
        @brief		リーダースキル：被ダメ補正：エネミー属性	<static>
        @param[in]	MasterDataSkillLeader	(skillLeader)	リーダースキルマスター
        @param[in]	int						(enemyElem	)	エネミー属性
        @retval		float	[倍率]
    */
    //------------------------------------------------------------------------
    static private float GetLeaderSkillDamageEnemyElement(MasterDataSkillLeader skillLeader, MasterDataDefineLabel.ElementType enemyElem)
    {
        float rate = 1.0f;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (skillLeader == null)
        {
            return (rate);
        }


        //----------------------------------------
        // ダメ補正：エネミー属性(敵攻撃ダメージ)処理
        //----------------------------------------

        //------------------------------
        // 判定する属性を取得
        //------------------------------
        MasterDataDefineLabel.ElementType chkElem = skillLeader.Get_DMG_ENEMY_ELEM_ELEMENT();

        //------------------------------
        // スキル発動判定
        // 属性指定無しの場合は全属性扱い
        //------------------------------
        if (chkElem != enemyElem
        && chkElem != MasterDataDefineLabel.ElementType.NONE)
        {
            return (rate);
        }

        //------------------------------
        // スキル発動
        //------------------------------
        float val = GetDBRevisionValue(skillLeader.Get_DMG_ENEMY_ELEM_RATE());
        rate = AvoidErrorMultiple(rate, val);

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		回復カウンター			<static>
        @param[in]	BattleSkillActivity[]		(activityArray)		1パッシブスキル発動情報格納配列
        @param[in]	int							(charaIdx)			キャラID
        @param[in]	int							(damage)			ダメージ量
        @param[in]	int							(enemyID)			敵ID
        @param[in]	ESKILLTYPE					(skillType)			スキルタイプ
        @return		bool						(result)			[成功/失敗]
        @change		Developer 2015/09/15 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public bool PassiveChkSkillCounterHeal(SkillRequestParam skill_request, GlobalDefine.PartyCharaIndex charaIdx, int damage, int enemyID, ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE)
    {
        BattleSceneManager battleMgr = BattleSceneManager.Instance;
        bool result = false;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (battleMgr == null
        || skill_request == null)
        {
            return (result);
        }

        if (BattleParam.m_PlayerParty == null
        || BattleParam.m_EnemyParam == null)
        {
            return (result);
        }

        if (BattleParam.m_PlayerParty.m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MAX) <= 0)
        {
            return (result);
        }


        // 対象情報取得
        BattleEnemy cTarget = BattleParam.m_EnemyParam[enemyID];
        if (cTarget == null
        || cTarget.getMasterDataParamChara() == null)
        {
            return (result);
        }

        // キャラ情報取得
        if (charaIdx < 0 || charaIdx >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return (result);
        }

        CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember(charaIdx, CharaParty.CharaCondition.SKILL_PASSIVE);
        if (chara == null
        || !chara.m_bHasCharaMasterDataParam)
        {
            return (result);
        }

        // パッシブスキル情報取得
        MasterDataSkillPassive skillParam = null;
        //------------------------------
        // スキルタイプによる分岐
        //------------------------------
        switch (skillType)
        {
            //------------------------------
            // パッシブスキル
            //------------------------------
            case ESKILLTYPE.ePASSIVE:
                skillParam = BattleParam.m_MasterDataCache.useSkillPassive(chara.m_CharaMasterDataParam.skill_passive);
                break;

            //------------------------------
            // リンクパッシブ
            //------------------------------
            case ESKILLTYPE.eLINKPASSIVE:
                {
                    if (chara.m_LinkParam == null)
                    {
                        return (result);
                    }

                    MasterDataParamChara linkCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(chara.m_LinkParam.m_CharaID);
                    if (linkCharaMaster == null)
                    {
                        return (result);
                    }

                    skillParam = BattleParam.m_MasterDataCache.useSkillPassive(linkCharaMaster.link_skill_passive);
                    break;
                }
        }

        if (skillParam == null)
        {
            return (result);
        }

        BattleSkillActivity passive_skill_counter_heal = PassiveSkillCounterHeal(skillParam, charaIdx, skillType);
        if (passive_skill_counter_heal != null)
        {
            skill_request.addSkillRequest(passive_skill_counter_heal);
            result = true;
        }


        return (result);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		回復カウンター			<static>
        @param[in]	MasterDataSkillPassive		(passiveParam)		スキルマスター
        @param[in]	int							(charaIdx)			キャラID
        @param[in]	ESKILLTYPE					(skillType)			スキルタイプ
        @return		BattleSkillActivity			(resultActivity)	[発動スキル情報]
        @add		Developer 2015/09/15 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static private BattleSkillActivity PassiveSkillCounterHeal(MasterDataSkillPassive passiveParam, GlobalDefine.PartyCharaIndex charaIdx, ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE)
    {
        BattleSkillActivity resultActivity = null;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null
        || passiveParam.Is_skill_damage_recovery_active() == false)
        {
            return (resultActivity);
        }

        if (BattleSceneUtil.checkChancePercentSkill(passiveParam.skill_damage_recovery_odds) == false)
        {
            return (resultActivity);
        }

        // パッシブスキル発動情報作成
        BattleSkillActivity activity = ActivityPassiveSkill(charaIdx, skillType);
        if (activity == null)
        {
            return (resultActivity);
        }

        // ターゲット情報作成
        BattleSkillTarget[] targetArray = new BattleSkillTarget[1];
        if (targetArray == null)
        {
            return (resultActivity);
        }

        targetArray[0] = new BattleSkillTarget();
        if (targetArray[0] == null)
        {
            return (resultActivity);
        }

        float rate = GetDBRevisionValue(passiveParam.skill_damage_recovery_rate);
        BattleSceneUtil.MultiFloat healVal = GetHealValue(BattleParam.m_PlayerParty, rate, false);
        targetArray[0].m_TargetType = BattleSkillTarget.TargetType.PLAYER;
        targetArray[0].m_SkillValueToPlayer.setValue(GlobalDefine.PartyCharaIndex.MAX, healVal.toMultiInt());

        activity.m_SkillParamTarget = targetArray;
        resultActivity = activity;

        return (resultActivity);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：カウンターチェック	<static>
        @param[in]	BattleSkillActivity[]		(activityArray)		パッシブスキル発動情報格納配列
        @param[in]	int							(charaIdx)			キャラID
        @param[in]	int							(damage)			ダメージ量
        @param[in]	int							(enemyID)			敵ID
        @param[in]	ESKILLTYPE					(skillType)			スキルタイプ
        @return		bool						(result)			[成功/失敗]
        @change		Developer 2015/09/15 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public bool PassiveChkSkillCounter(SkillRequestParam skill_request, GlobalDefine.PartyCharaIndex charaIdx, int damage, int enemyID, ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE)
    {
        BattleSceneManager battleMgr = BattleSceneManager.Instance;
        bool result = false;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (battleMgr == null
        || skill_request == null)
        {
            return (result);
        }

        if (BattleParam.m_EnemyParam == null)
        {
            return (result);
        }


        // 対象情報取得
        BattleEnemy cTarget = BattleParam.m_EnemyParam[enemyID];
        if (cTarget == null
        || cTarget.getMasterDataParamChara() == null)
        {
            return (result);
        }

        // キャラ情報取得
        if (charaIdx < 0 || charaIdx >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return (result);
        }

        CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember(charaIdx, CharaParty.CharaCondition.SKILL_PASSIVE);
        if (chara == null
        || !chara.m_bHasCharaMasterDataParam)
        {
            return (result);
        }

        BattleSkillActivity passive_skill_counter = PassiveSkillCounter(chara, cTarget, charaIdx, enemyID, damage, skillType);
        if (passive_skill_counter != null)
        {
            skill_request.addSkillRequest(passive_skill_counter);
            result = true;
        }

        return (result);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：カウンター	<static>
        @param[in]	CharaOnce					(chara)				発動者情報(charaIdxから求められるが、渡してもらう形)
        @param[in]	BattleEnemy					(targetEnemy)		対象情報(enemyIdxから求められるが、渡してもらう形)
        @param[in]	int							(charaIdx)			パーティキャラ番号
        @param[in]	int							(enemyIdx)			敵番号
        @param[in]	int							(damage)			ダメージ量
        @param[in]	ESKILLTYPE					(skillType)			スキルタイプ
        @return		BattleSkillActivity			(resultActivity)	[発動スキル情報]
        @add		Developer 2015/09/15 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static private BattleSkillActivity PassiveSkillCounter(CharaOnce chara, BattleEnemy targetEnemy, GlobalDefine.PartyCharaIndex charaIdx, int enemyIdx, int damage, ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE)
    {
        BattleSceneManager battleMgr = BattleSceneManager.Instance;
        BattleSkillActivity resultActivity = null;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (chara == null
        || !chara.m_bHasCharaMasterDataParam
        || targetEnemy == null
        || targetEnemy.getMasterDataParamChara() == null)
        {
            return (resultActivity);
        }


        // パッシブスキル情報取得
        MasterDataSkillPassive passiveParam = null;
        //------------------------------
        // スキルタイプによる分岐
        //------------------------------
        switch (skillType)
        {
            //------------------------------
            // パッシブスキル
            //------------------------------
            case ESKILLTYPE.ePASSIVE:
                passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara.m_CharaMasterDataParam.skill_passive);
                break;

            //------------------------------
            // リンクパッシブ
            //------------------------------
            case ESKILLTYPE.eLINKPASSIVE:
                {
                    if (chara.m_LinkParam == null)
                    {
                        return (resultActivity);
                    }

                    MasterDataParamChara linkCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(chara.m_LinkParam.m_CharaID);
                    if (linkCharaMaster == null)
                    {
                        return (resultActivity);
                    }

                    passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(linkCharaMaster.link_skill_passive);
                    break;
                }
        }

        if (passiveParam == null)
        {
            return (resultActivity);
        }

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null
        || passiveParam.Is_skill_counter_atk_active() == false)
        {
            return (resultActivity);
        }

        // 確率判定
        if (BattleSceneUtil.checkChancePercentSkill(passiveParam.skill_counter_atk_odds) == false)
        {
            return (resultActivity);
        }

        // パッシブスキル発動情報作成
        BattleSkillActivity activity = ActivityPassiveSkill(charaIdx, skillType);
        if (activity == null)
        {
            return (resultActivity);
        }

        // ターゲット情報作成
        BattleSkillTarget[] targetArray = new BattleSkillTarget[1];
        if (targetArray == null)
        {
            return (resultActivity);
        }

        targetArray[0] = new BattleSkillTarget();
        if (targetArray[0] == null)
        {
            return (resultActivity);
        }

        //------------------------------
        // 属性倍率
        //------------------------------
        float fElementRate = GetSkillElementRate(activity.m_Element, targetEnemy.getMasterDataParamChara().element);

        //------------------------------
        // リーダースキル補正(とりあえずver)
        //------------------------------
        float leaderPow = 1.0f;
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex.LEADER, chara));
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex.FRIEND, chara));
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRateCondition(GlobalDefine.PartyCharaIndex.LEADER));
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRateCondition(GlobalDefine.PartyCharaIndex.FRIEND));

        //------------------------------
        // パッシブスキル補正(とりあえずver)
        //------------------------------
        GlobalDefine.PartyCharaIndex owner = GlobalDefine.PartyCharaIndex.MAX;	// ダミー（未使用）
        float passivePow = 1.0f;
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkSkillDamageRate(chara));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkKindKillerAtk(chara));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkDamageUPFloorPanel(ref owner));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkDamageUPElemNum(ref owner));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkDamageUPHandsNum(ref owner));

        //----------------------------------------
        // リンクパッシブスキル補正
        //----------------------------------------
        float fLinkPassivePow = 1.0f;
        uint unLinkUnitID = 0;
        if (chara.m_LinkParam != null)
        {
            unLinkUnitID = chara.m_LinkParam.m_CharaID;
        }
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkSkillDamageRate(chara, ESKILLTYPE.eLINKPASSIVE));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkKindKillerAtk(chara, ESKILLTYPE.eLINKPASSIVE));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkDamageUPFloorPanel(ref owner, ESKILLTYPE.eLINKPASSIVE, unLinkUnitID));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkDamageUPElemNum(ref owner, ESKILLTYPE.eLINKPASSIVE, unLinkUnitID));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkDamageUPHandsNum(ref owner, ESKILLTYPE.eLINKPASSIVE, unLinkUnitID));

        //------------------------------
        // 状態異常によるダメージ増減
        //------------------------------
        float fAilmentRate = 1.0f;
        // 攻撃力倍率
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, BattleParam.m_PlayerParty.m_Ailments.getAilment(charaIdx).GetOffenceRate());
        // 属性攻撃力倍率
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, BattleParam.m_PlayerParty.m_Ailments.getAilment(charaIdx).GetOffenceElementRate(chara.m_CharaMasterDataParam.element));
        // 種族攻撃力倍率(メイン)
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, BattleParam.m_PlayerParty.m_Ailments.getAilment(charaIdx).GetOffenceKindRate(chara.kind));
        // 種族攻撃力倍率(サブ)
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, BattleParam.m_PlayerParty.m_Ailments.getAilment(charaIdx).GetOffenceKindRate(chara.kind_sub));

        //----------------------------------------
        // v390 Developer add
        // 敵特性による補正の適用
        //----------------------------------------
        float fEnemyAbilityRate = 1.0f;
        fEnemyAbilityRate = EnemyAbility.GetAbilityDamageRatePlayer(activity, targetEnemy);

        // ダメージ量算出
        float damageScale = GetDBRevisionValue(passiveParam.skill_counter_atk_scale);
        float damageValue = AvoidErrorMultiple((float)damage, damageScale);
        damageValue = AvoidErrorMultiple((float)damageValue, fElementRate);
        damageValue = AvoidErrorMultiple((float)damageValue, leaderPow);
        damageValue = AvoidErrorMultiple((float)damageValue, passivePow);
        damageValue = AvoidErrorMultiple((float)damageValue, fLinkPassivePow);
        damageValue = AvoidErrorMultiple((float)damageValue, fAilmentRate);
        damageValue = AvoidErrorMultiple((float)damageValue, fEnemyAbilityRate); //v390 Developer add

#if UNITY_EDITOR
        Debug.Log("damage=" + damage + " ElemRate=" + fElementRate + " LeaderPow=" + leaderPow + " PassivePow=" + passivePow + "LinkPassivePow=" + fLinkPassivePow + " ailmentRate=" + fAilmentRate);
#endif // #if UNITY_EDITOR

        // ターゲット情報作成
        targetArray[0].m_TargetType = BattleSkillTarget.TargetType.ENEMY;
        targetArray[0].m_TargetNum = enemyIdx;
        targetArray[0].m_SkillValueToEnemy = (int)damageValue;

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "カウンター値:" + ((int)damageValue).ToString()
            + " = " + ((int)damage).ToString() + "(ダメージ値)"
            + " x" + InGameUtilBattle.getDebugPercentString(damageScale) + "%(スキル威力)"
            + " x" + InGameUtilBattle.getDebugPercentString(fElementRate) + "%(属性補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(leaderPow) + "%(リーダースキル補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(passivePow) + "%(パッシブスキル補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(fLinkPassivePow) + "%(リンクパッシブスキル補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(fAilmentRate) + "%(状態異常補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(fEnemyAbilityRate) + "%(敵特性補正)"
        );


        // ターゲット情報設定
        activity.m_SkillParamTarget = targetArray;

        resultActivity = activity;

        return (resultActivity);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによるダメージ増減倍率の取得		<static>
        @param[in]	CharaOnce		(chara)		攻撃者
        @param[in]	CharaOnce		(nHeal)		回復スキルによるライフ上乗せ量。m_PartyTotalHPにこの値を加算した値を現在ライフとして扱う。
        @param[in]	ESKILLTYPE		(skillType)	スキルタイプ
        @return		float			[ダメージ倍率]
        @change		Developer 2015/09/12 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public float PassiveChkSkillDamageRate(CharaOnce chara, ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE)
    {
        float rate = 1.0f;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (chara == null
        || !chara.m_bHasCharaMasterDataParam)
        {
            return (rate);
        }

        CharaParty party = BattleParam.m_PlayerParty;
        if (party == null)
        {
            return (rate);
        }

        MasterDataSkillPassive skillPassive = null;
        //------------------------------
        // スキルタイプによる分岐
        //------------------------------
        switch (skillType)
        {
            //------------------------------
            // パッシブスキル
            //------------------------------
            case ESKILLTYPE.ePASSIVE:
                skillPassive = BattleParam.m_MasterDataCache.useSkillPassive(chara.m_CharaMasterDataParam.skill_passive);
                break;
            //------------------------------
            // リンクパッシブ
            //------------------------------
            case ESKILLTYPE.eLINKPASSIVE:
                {
                    if (chara.m_LinkParam == null)
                    {
                        return (rate);
                    }

                    MasterDataParamChara linkCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(chara.m_LinkParam.m_CharaID);
                    if (linkCharaMaster == null)
                    {
                        return (rate);
                    }

                    skillPassive = BattleParam.m_MasterDataCache.useSkillPassive(linkCharaMaster.link_skill_passive);
                    break;
                }
        }

        if (skillPassive == null)
        {
            return (rate);
        }

        //------------------------------
        // HPが最大時攻撃力アップ
        //------------------------------
        float fullLifeVal = PassiveSkillDamageRateFullLife(skillPassive, party).getValue(chara.m_PartyCharaIndex);
        rate = AvoidErrorMultiple(rate, fullLifeVal);

        //------------------------------
        // 瀕死時に攻撃力アップ
        //------------------------------
        float dyingVal = PassiveSkillDamageRateDying(skillPassive, party).getValue(chara.m_PartyCharaIndex);
        rate = AvoidErrorMultiple(rate, dyingVal);

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：HPが最大時攻撃力アップ		<static>
        @param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
        @param[in]	CharaParty				(party)			パーティ情報
        @param[in]	CharaOnce				(nHeal)			回復スキルによるライフ上乗せ量。m_PartyTotalHPにこの値を加算した値を現在ライフとして扱う。
        @return		float					(rate)			[ダメージ倍率]
        @add		Developer 2015/09/12 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static private BattleSceneUtil.MultiFloat PassiveSkillDamageRateFullLife(MasterDataSkillPassive passiveParam, CharaParty party)
    {
        BattleSceneUtil.MultiFloat rate = new BattleSceneUtil.MultiFloat(1.0f);

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null)
        {
            return (rate);
        }

        MasterDataSkillPassive.FullLifeAttacUpkInfo full_life_attack_up_info = passiveParam.Get_FullLifeAttackUpInfo();
        if (full_life_attack_up_info == null)
        {
            return (rate);
        }

        BattleSceneUtil.MultiFloat val = BattleSceneUtil.getLifeRatePow(1.0f, 1.0f, GetDBRevisionValue(full_life_attack_up_info.m_AttackPercent));
        rate.mulValue(GlobalDefine.PartyCharaIndex.MAX, val);

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：瀕死時に攻撃力アップ		<static>
        @param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
        @param[in]	CharaParty				(party)			パーティ情報
        @param[in]	CharaOnce				(nHeal)			回復スキルによるライフ上乗せ量。m_PartyTotalHPにこの値を加算した値を現在ライフとして扱う。
        @return		float					(rate)			[ダメージ倍率]
        @add		Developer 2015/09/12 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static private BattleSceneUtil.MultiFloat PassiveSkillDamageRateDying(MasterDataSkillPassive passiveParam, CharaParty party)
    {
        BattleSceneUtil.MultiFloat rate = new BattleSceneUtil.MultiFloat(1.0f);

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null)
        {
            return (rate);
        }

        MasterDataSkillPassive.DyingAttackUpInfo dying_attack_up_info = passiveParam.Get_DyingAttackUpInfo();
        if (dying_attack_up_info == null)
        {
            return (rate);
        }

        BattleSceneUtil.MultiFloat val = BattleSceneUtil.getLifeRatePow(0.0f, GetDBRevisionValue(dying_attack_up_info.m_HpBorder), GetDBRevisionValue(dying_attack_up_info.m_AttackPercent));
        rate.mulValue(GlobalDefine.PartyCharaIndex.MAX, val);

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによるダメージ倍率の取得			<static>
        @return		float		[ダメージ倍率]
        @note		属性＆種族の軽減値の累積値で計算する
    */
    //----------------------------------------------------------------------------
    static public BattleSceneUtil.MultiFloat PassiveChkDamageReduce(MasterDataDefineLabel.ElementType elem, MasterDataDefineLabel.KindType kind, MasterDataDefineLabel.KindType kind_sub, BattleSceneUtil.MultiFloat damage)
    {

        float reduce_elem = 0.0f;
        float reduce_kind = 0.0f;
        float reduce_kind_sub = 0.0f;
        float reduce_handBarrier = 0.0f;
        BattleSceneUtil.MultiFloat reduce_fullHP = new BattleSceneUtil.MultiFloat(0.0f);
        BattleSceneUtil.MultiFloat reduce_rate = new BattleSceneUtil.MultiFloat(1.0f);

        {
            //------------------------------
            //	各軽減値を集計
            //------------------------------
            // 属性の軽減値
            reduce_elem = PassiveChkDamageReduceElem(elem);
#if UNITY_EDITOR && OUTPUT_INGAME_LOG_SKILL_PASSIVE
            Debug.LogError( "PassiveChkDamageReduceElem " + reduce_elem );
#endif // #if UNITY_EDITOR && OUTPUT_INGAME_LOG_SKILL_PASSIVE


            // 種族の軽減値(メイン/サブ)
            reduce_kind = PassiveChkDamageReduceKind(kind, kind_sub);
#if UNITY_EDITOR && OUTPUT_INGAME_LOG_SKILL_PASSIVE
            Debug.LogError( "PassiveChkDamageReduceKind " + reduce_kind );
#endif // #if UNITY_EDITOR && OUTPUT_INGAME_LOG_SKILL_PASSIVE


            //	手札バリア
            if (damage.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0)
            {
                reduce_handBarrier = PassiveChkHandCardBarrier();
            }
#if UNITY_EDITOR && OUTPUT_INGAME_LOG_SKILL_PASSIVE
            Debug.LogError( "PassiveChkHandCardBarrier " + reduce_handBarrier );
#endif // #if UNITY_EDITOR && OUTPUT_INGAME_LOG_SKILL_PASSIVE

            // 体力最大時ダメージ軽減
            if (damage.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0)
            {
                ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE;             // ダミー（未使用）
                GlobalDefine.PartyCharaIndex owner = GlobalDefine.PartyCharaIndex.MAX;	// ダミー（未使用）
                reduce_fullHP = PassiveChkDamageReduceFullHP(BattleParam.m_PlayerParty.m_HPCurrent, ref owner, ref skillType);
            }
#if UNITY_EDITOR && OUTPUT_INGAME_LOG_SKILL_PASSIVE
            Debug.LogError( "PassiveChkDamageReduceFullHP " + reduce_fullHP );
#endif // #if UNITY_EDITOR && OUTPUT_INGAME_LOG_SKILL_PASSIVE

            reduce_rate.subValue(GlobalDefine.PartyCharaIndex.MAX, reduce_elem);
            reduce_rate.subValue(GlobalDefine.PartyCharaIndex.MAX, reduce_kind);
            reduce_rate.subValue(GlobalDefine.PartyCharaIndex.MAX, reduce_kind_sub);
            reduce_rate.subValue(GlobalDefine.PartyCharaIndex.MAX, reduce_handBarrier);
            reduce_rate.subValue(GlobalDefine.PartyCharaIndex.MAX, reduce_fullHP);
            //reduce_rateはダメージ増加スキルもあるので１以上を許容する
            reduce_rate.maxValue(GlobalDefine.PartyCharaIndex.MAX, new BattleSceneUtil.MultiFloat(0.0f));

#if BUILD_TYPE_DEBUG
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "パッシブスキルダメージ軽減率:" + reduce_rate.getDebugString(GlobalDefine.PartyCharaIndex.MAX, 100) + "%"
                + " = 100 - " + InGameUtilBattle.getDebugPercentString(reduce_elem) + "(敵属性補正)"
                + " - " + InGameUtilBattle.getDebugPercentString(reduce_kind) + "(敵種族１補正)"
                + " - " + InGameUtilBattle.getDebugPercentString(reduce_kind_sub) + "(敵種族２補正)"
                + " - " + InGameUtilBattle.getDebugPercentString(reduce_handBarrier) + "(手札バリア補正)"
                + " - " + reduce_fullHP.getDebugString(GlobalDefine.PartyCharaIndex.MAX, 100) + "(ＨＰ最大時補正)"
            );
#endif
        }

        return reduce_rate;
    }



    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルよる属性ダメージ増減倍率の取得		<static>
        @param[in]	int			(elem)			属性
        @return		float		[ダメージ倍率]
        @change		Developer 2015/09/14 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static private float PassiveChkDamageReduceElem(MasterDataDefineLabel.ElementType elem)
    {
        float damage_rate = 0.0f;

        //----------------------------------------
        // 合計値を取得
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //----------------------------------------
        //------------------------------
        // パッシブスキル：全キャラチェック
        //------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            //------------------------------
            // エラーチェック
            //------------------------------
            if (chara_once == null)
            {
                continue;
            }

            // パッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // 特定種族のダメージを軽減
            //------------------------------
            // 軽減率を足していく
            damage_rate += PassiveDamageReduceElem(passiveParam, elem);
        }
        #endregion

        //------------------------------
        // リンクパッシブ：全リンクキャラチェック
        //------------------------------
        #region ==== リンクパッシブ処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            //------------------------------
            // エラーチェック
            //------------------------------
            if (chara_once == null
            || chara_once.m_LinkParam == null)
            {
                continue;
            }

            // リンクキャラ情報取得
            MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
            if (charaParam == null)
            {
                continue;
            }

            // リンクパッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // 特定属性のダメージを軽減
            //------------------------------
            // 軽減率を足していく
            damage_rate += PassiveDamageReduceElem(passiveParam, elem);
        }
        #endregion

        return (damage_rate);
    }



    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルよる属性ダメージ増減倍率の取得		<static>
        @param[in]	MasterDataSkillPassive	(passiveParam)		スキルマスター
        @param[in]	int						(elem)				属性
        @return		float					(damage_rate)		[ダメージ倍率]
        @add		Developer 2015/09/14 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static private float PassiveDamageReduceElem(MasterDataSkillPassive passiveParam, MasterDataDefineLabel.ElementType elem)
    {
        float damage_rate = 0.0f;
        int percentage_max = 100;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null)
        {
            return (damage_rate);
        }

        MasterDataSkillPassive.DeclineElementDamageInfo decline_element_damage_info = passiveParam.Get_DeclineElementDamageInfo(elem);
        if (decline_element_damage_info == null)
        {
            return (damage_rate);
        }

        //------------------------------
        // ダメージ軽減率を取得
        //------------------------------
        damage_rate = GetDBRevisionValue(percentage_max - decline_element_damage_info.m_DamagePercent);

        return (damage_rate);
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルよる種族ダメージ増減倍率の取得		<static>
        @param[in]	int			(kind)			種族
        @param[in]	int			(kind_sub)		副種族
        @return		float		[ダメージ倍率]
        @change		Developer 2015/09/14 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static private float PassiveChkDamageReduceKind(MasterDataDefineLabel.KindType kind, MasterDataDefineLabel.KindType kind_sub)
    {
        float damage_rate = 0.0f;

        //----------------------------------------
        // 合計値を取得
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //----------------------------------------
        //------------------------------
        // パッシブスキル：全キャラチェック
        //------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            //------------------------------
            // エラーチェック
            //------------------------------
            if (chara_once == null)
            {
                continue;
            }

            // パッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // 特定種族からのダメージを軽減
            //------------------------------
            // 軽減率を足していく
            damage_rate += PassiveDamageReduceKind(passiveParam, kind, kind_sub);
        }
        #endregion

        //------------------------------
        // リンクパッシブ：全リンクキャラチェック
        //------------------------------
        #region ==== リンクパッシブ処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            //------------------------------
            // エラーチェック
            //------------------------------
            if (chara_once == null
            || chara_once.m_LinkParam == null)
            {
                continue;
            }

            // リンクキャラ情報取得
            MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
            if (charaParam == null)
            {
                continue;
            }

            // リンクパッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // 特定属性のダメージを軽減
            //------------------------------
            // 軽減率を足していく
            damage_rate += PassiveDamageReduceKind(passiveParam, kind, kind_sub);
        }
        #endregion

        //------------------------------
        //	100％軽減を超えないようにクリップ
        //------------------------------
        //if ( damage_rate > rate_max ) {
        //    damage_rate = rate_max;
        //}

        return (damage_rate);
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルよる種族ダメージ増減倍率の取得		<static>
        @param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
        @param[in]	int						(kind)			種族
        @param[in]	int						(kind_sub)		副種族
        @return		float					(damage_rate)	[ダメージ倍率]
        @add		Developer 2015/09/14 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static private float PassiveDamageReduceKind(MasterDataSkillPassive passiveParam, MasterDataDefineLabel.KindType kind, MasterDataDefineLabel.KindType kind_sub)
    {
        float damage_rate = 0.0f;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null)
        {
            return (damage_rate);
        }

        MasterDataSkillPassive.DeclineKindDamageInfo decline_kind_damage_info_main = passiveParam.Get_DeclineKindDamageInfo(kind);
        MasterDataSkillPassive.DeclineKindDamageInfo decline_kind_damage_info_sub = passiveParam.Get_DeclineKindDamageInfo(kind_sub);

        MasterDataSkillPassive.DeclineKindDamageInfo decline_kind_damage_info = null;
        if (decline_kind_damage_info_main != null
            && decline_kind_damage_info_sub != null
        )
        {
            // 効果値が良い方（ダメージ軽減量が多い）を選ぶ
            if (decline_kind_damage_info_main.m_DamagePercent <= decline_kind_damage_info_sub.m_DamagePercent)
            {
                decline_kind_damage_info = decline_kind_damage_info_main;
            }
            else
            {
                decline_kind_damage_info = decline_kind_damage_info_sub;
            }
        }
        else if (decline_kind_damage_info_main != null)
        {
            decline_kind_damage_info = decline_kind_damage_info_main;
        }
        else if (decline_kind_damage_info_sub != null)
        {
            decline_kind_damage_info = decline_kind_damage_info_sub;
        }
        else
        {
            return (damage_rate);
        }

        //------------------------------
        // ダメージ軽減率を取得
        //------------------------------
        damage_rate = GetDBRevisionValue(100 - decline_kind_damage_info.m_DamagePercent);

        return (damage_rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによる手札バリア
        @change		Developer 2015/09/12 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    public static float PassiveChkHandCardBarrier()
    {
        BattleSceneManager balleMgr = BattleSceneManager.Instance;
        float percentage = 0.0f;

        bool[] handTransform = new bool[BattleLogic.BATTLE_FIELD_MAX];

        //------------------------------
        // 手札変換情報を初期化
        //------------------------------
        for (int num = 0; num < handTransform.Length; ++num)
        {
            handTransform[num] = false;
        }

        //----------------------------------------
        // 合計値を取得
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //----------------------------------------
        //------------------------------
        // パッシブスキル：全キャラチェック
        //------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            //------------------------------
            // エラーチェック
            //------------------------------
            if (chara_once == null)
            {
                continue;
            }

            // パッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // 手札バリア
            //------------------------------
            // 軽減率を足していく
            percentage += PassiveHandCardBarrier(passiveParam, ref handTransform);
        }
        #endregion

        //------------------------------
        // リンクパッシブ：全リンクキャラチェック
        //------------------------------
        #region ==== リンクパッシブ処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            //------------------------------
            // エラーチェック
            //------------------------------
            if (chara_once == null
            || chara_once.m_LinkParam == null)
            {
                continue;
            }

            // リンクキャラ情報取得
            MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
            if (charaParam == null)
            {
                continue;
            }

            // リンクパッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // 手札バリア
            //------------------------------
            // 軽減率を足していく
            percentage += PassiveHandCardBarrier(passiveParam, ref handTransform);
        }
        #endregion


        BattleScene.BattleCard tempCardParam;
        //------------------------------
        // 手札変化(いずれリクエスト式にしよう)
        //------------------------------
        for (int num = 0; num < balleMgr.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCardMaxCount(); ++num)
        {
            tempCardParam = balleMgr.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCard(num);
            if (tempCardParam == null)
            {
                continue;
            }

            if (handTransform[num] == false)
            {
                continue;
            }

            balleMgr.PRIVATE_FIELD.ChangeCard(ref tempCardParam);

            // 手札変化エフェクト
            balleMgr.PRIVATE_FIELD.m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.HAND_CARD_AREA, num, BattleScene._BattleCardManager.EffectInfo.EffectType.CARD_DESTROY);
        }

        return percentage;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによる手札バリア
        @param[in]	MasterDataSkillPassive	(passiveParam)		スキルマスター
        @param[out]	ref bool[]				(handTransform)		手札変換情報
        @add		Developer 2015/09/12 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    private static float PassiveHandCardBarrier(MasterDataSkillPassive passiveParam, ref bool[] handTransform)
    {
        float percentage = 0.0f;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null
        || passiveParam.skill_type != MasterDataDefineLabel.PassiveSkillCategory.HANDS_BARRIER)
        {
            return (percentage);
        }

        // パッシブスキルの指定属性
        MasterDataDefineLabel.ElementType elem = passiveParam.Get_HANDS_BARRIER_ELEM();

        for (int num = 0; num < BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCardMaxCount(); ++num)
        {
            // エラーチェック
            if (BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCard(num) == null)
            {
                continue;
            }

            // 属性が一致しない場合は終了
            if (elem != MasterDataDefineLabel.ElementType.NONE)
            {
                if (elem != BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCard(num).getElementType())
                {
                    continue;
                }
            }

            // 手札バリア発動条件達成
            // 軽減率を足していく
            percentage += 1.0f - GetDBRevisionValue(passiveParam.Get_HANDS_BARRIER_RATE());

            handTransform[num] = true;
        }

        return (percentage);
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによる手札の出現属性バランス調整
        @param[in]	int		(elem)		属性
        @return		int		[指定属性の出現率調整値]
        @change		Developer 2015/09/16 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    public static float PassiveChkHandCardChanceUPElement(MasterDataDefineLabel.ElementType elem)
    {
        float val = 1.0f;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (elem < MasterDataDefineLabel.ElementType.NONE
        || elem >= MasterDataDefineLabel.ElementType.MAX)
        {
            return (val);
        }

        float rate = 1.0f;

        //int[] element_value = new int[ MasterDataDefineLabel.ELEMENT_MAX ];


        //----------------------------------------
        // 合計値を取得
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //----------------------------------------
        //------------------------------
        // パッシブスキル：全キャラチェック
        //------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null)
            {
                continue;
            }

            // パッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // 各属性の出現率調整
            //------------------------------
            rate = PassiveHandCardChanceUPElement(passiveParam, elem);

            // 指定属性の値の総計
            val = AvoidErrorMultiple(val, rate);
        }
        #endregion

        //------------------------------
        // リンクパッシブ：全リンクキャラチェック
        //------------------------------
        #region ==== リンクパッシブ処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null
            || chara_once.m_LinkParam == null)
            {
                continue;
            }

            // リンクキャラ情報取得
            MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
            if (charaParam == null)
            {
                continue;
            }

            // リンクパッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // 各属性の出現率調整
            //------------------------------
            rate = PassiveHandCardChanceUPElement(passiveParam, elem);

            // 指定属性の値の総計
            val = AvoidErrorMultiple(val, rate);
        }
        #endregion

        return (val);
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによる手札の出現属性バランス調整
        @param[in]	MasterDataSkillPassive	(passiveParam)		スキルマスター
        @param[in]	int						(elem)				属性
        @return		int						(rate)				[指定属性の出現率調整値]
        @add		Developer 2015/09/16 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    private static float PassiveHandCardChanceUPElement(MasterDataSkillPassive passiveParam, MasterDataDefineLabel.ElementType elem)
    {
        float rate = 1.0f;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null
        || passiveParam.skill_type != MasterDataDefineLabel.PassiveSkillCategory.HANDCARD_CHANCE_UP)
        {
            return (rate);
        }

        //------------------------------
        // 各属性の出現率調整値を計算
        //------------------------------
        // 属性毎の値(%)
        int element_value = passiveParam.Get_CARD_CHANCE_UP(elem);

        rate = GetDBRevisionValue(element_value);

        return (rate);
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによる指定HANDS時でHP回復
        @param[in]	int		(handsCount)	ハンズ数
        @change		Developer 2015/09/15 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    public static void PassiveChkHandsHealHP(ref SkillRequestParam dest_skill_request, int handsCount)
    {
        int nRecovelyValue = 0;
        float nRecovelyRate = 0.0f;
        BattleSkillActivity last_skill_activity = null;

        // スキルカットイン用にスキル情報を生成（スキルカットインを表示したいだけなのでスキル効果は何も発揮しません。ただし最後のカットインには回復の効果を付加しています）
        for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.SKILL_PASSIVE);
            if (chara_once != null)
            {
                // パッシブスキル
                MasterDataSkillPassive passive_param = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
                if (passive_param != null)
                {
                    if (PassiveHandsHealHP(passive_param, handsCount, ref nRecovelyValue, ref nRecovelyRate))
                    {
                        BattleSkillActivity skill_activity = new BattleSkillActivity();
                        skill_activity.m_SkillParamOwnerNum = (GlobalDefine.PartyCharaIndex)idx;
                        skill_activity.m_SkillParamFieldID = -1;
                        skill_activity.m_SkillParamSkillID = passive_param.fix_id;
                        skill_activity.m_SkillType = ESKILLTYPE.ePASSIVE;
                        skill_activity.m_Effect = MasterDataDefineLabel.UIEffectType.NONE;
                        skill_activity.m_Element = MasterDataDefineLabel.ElementType.HEAL;	// スキルカットインの下地は回復属性に
                        skill_activity.m_Type = MasterDataDefineLabel.SkillType.HEAL;
                        {
                            skill_activity.m_SkillParamTarget = new BattleSkillTarget[1];
                            skill_activity.m_SkillParamTarget[0] = new BattleSkillTarget();
                            skill_activity.m_SkillParamTarget[0].m_TargetType = BattleSkillTarget.TargetType.PLAYER;
                            skill_activity.m_SkillParamTarget[0].m_SkillValueToPlayer.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
                        }
                        dest_skill_request.addSkillRequest(skill_activity);

                        last_skill_activity = skill_activity;
                    }
                }

                // リンクパッシブスキル
                if (chara_once.m_LinkParam != null)
                {
                    MasterDataParamChara link_chara = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
                    if (link_chara != null)
                    {
                        MasterDataSkillPassive link_passive_param = BattleParam.m_MasterDataCache.useSkillPassive(link_chara.link_skill_passive);
                        if (link_passive_param != null)
                        {
                            if (PassiveHandsHealHP(link_passive_param, handsCount, ref nRecovelyValue, ref nRecovelyRate))
                            {
                                BattleSkillActivity skill_activity = new BattleSkillActivity();
                                skill_activity.m_SkillParamOwnerNum = (GlobalDefine.PartyCharaIndex)idx;
                                skill_activity.m_SkillParamFieldID = -1;
                                skill_activity.m_SkillParamSkillID = link_passive_param.fix_id;
                                skill_activity.m_SkillType = ESKILLTYPE.eLINKPASSIVE;
                                skill_activity.m_Effect = MasterDataDefineLabel.UIEffectType.NONE;
                                skill_activity.m_Element = MasterDataDefineLabel.ElementType.HEAL;	// スキルカットインの下地は回復属性に
                                skill_activity.m_Type = MasterDataDefineLabel.SkillType.HEAL;
                                {
                                    skill_activity.m_SkillParamTarget = new BattleSkillTarget[1];
                                    skill_activity.m_SkillParamTarget[0] = new BattleSkillTarget();
                                    skill_activity.m_SkillParamTarget[0].m_TargetType = BattleSkillTarget.TargetType.PLAYER;
                                    skill_activity.m_SkillParamTarget[0].m_SkillValueToPlayer.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
                                }
                                dest_skill_request.addSkillRequest(skill_activity);

                                last_skill_activity = skill_activity;
                            }
                        }
                    }
                }
            }
        }

        // 最後のカットインに回復量（合計値）の情報を付加
        if (last_skill_activity != null)
        {
            BattleSceneUtil.MultiInt recovely_value = new BattleSceneUtil.MultiInt();
            {
                //----------------------------------------------------------
                // 総回復量を計算
                //----------------------------------------------------------
                CharaParty party = BattleParam.m_PlayerParty;
                float rate = GetDBRevisionValue(nRecovelyRate);

                recovely_value.setValue(GlobalDefine.PartyCharaIndex.MAX, nRecovelyValue);

                // 割合値から回復量を計算したものを加算
                recovely_value.addValue(GlobalDefine.PartyCharaIndex.MAX, GetHealValue(party, rate, false).toMultiInt());
            }

            last_skill_activity.m_SkillParamTarget[0].m_SkillValueToPlayer.setValue(GlobalDefine.PartyCharaIndex.MAX, recovely_value);
        }
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによる指定HANDS時でHP回復
        @param[in]	MasterDataSkillPassive	(passiveParam)		スキルマスター
        @param[in]	int						(handsCount)		ハンズ数
        @param[out]	ref int					(nRecovelyValue)	回復量：固定値
        @param[out]	ref float				(nRecovelyRate)		回復量：割合値
        @add		Developer 2015/09/15 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static public bool PassiveHandsHealHP(MasterDataSkillPassive passiveParam, int handsCount, ref int nRecovelyValue, ref float nRecovelyRate)
    {
        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null)
        {
            return false;
        }

        MasterDataSkillPassive.HandsHealInfo hands_heal_info = passiveParam.Get_HandsHealInfo();
        if (hands_heal_info == null)
        {
            return false;
        }

        //------------------------------
        // 効果適用条件チェック
        //------------------------------
        if (hands_heal_info.m_IsOver)
        {
            // 指定HANDS数以上
            if (handsCount < hands_heal_info.m_HandsCount)
            {
                return false;
            }
        }
        else
        {
            // 指定HANDS数と一致
            if (handsCount != hands_heal_info.m_HandsCount)
            {
                return false;
            }
        }

        // 割合値を加算
        nRecovelyRate += hands_heal_info.m_HealValuePercent;

        // 固定値を加算
        nRecovelyValue += hands_heal_info.m_HealValueFix;

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによるカウントダウン増減
        @return		float	[カウントダウン増減数]
        @change		Developer 2015/09/12 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public float PassiveChkCountDown()
    {
        float countDown = 0.0f;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (BattleParam.isActiveBattle() == false)
        {
            return (countDown);
        }

        //----------------------------------------
        // 合計値を取得
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //----------------------------------------
        //------------------------------
        // パッシブスキル：全キャラチェック
        //------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null)
            {
                continue;
            }

            // パッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // カウントダウン増減
            //------------------------------
            // 付加秒数を加算
            countDown += PassiveCountDown(passiveParam);
        }
        #endregion

        //------------------------------
        // リンクパッシブ：全リンクキャラチェック
        //------------------------------
        #region ==== リンクパッシブ処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null
            || chara_once.m_LinkParam == null)
            {
                continue;
            }

            // リンクキャラ情報取得
            MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
            if (charaParam == null)
            {
                continue;
            }

            // リンクパッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // カウントダウン増減
            //------------------------------
            // 付加秒数を加算
            countDown += PassiveCountDown(passiveParam);
        }
        #endregion

        return (countDown);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによるカウントダウン増減
        @param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
        @return		float	[カウントダウン増減数]
        @add		Developer 2015/09/12 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static private float PassiveCountDown(MasterDataSkillPassive passiveParam)
    {
        float countDown = 0.0f;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null)
        {
            return (countDown);
        }

        MasterDataSkillPassive.CountDownTimeInfo count_down_time_info = passiveParam.Get_CountDownTimeInfo();
        if (count_down_time_info == null)
        {
            return (countDown);
        }

        // 付加秒数を取得
        countDown = count_down_time_info.m_Second;

        return (countDown);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによるHANDS時のRate数増減
        @return		float	[Rate増減数]
        @change		Developer 2015/09/12 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public float PassiveChkHandsRate()
    {
        float rate = 0.0f;

        //----------------------------------------
        // 合計値を取得
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //----------------------------------------
        //------------------------------
        // パッシブスキル：全キャラチェック
        //------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null)
            {
                continue;
            }

            // パッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // Rate数増減
            //------------------------------
            // Rate数を加算
            rate += PassiveHandsRate(passiveParam);
        }
        #endregion

        //------------------------------
        // リンクパッシブ：全リンクキャラチェック
        //------------------------------
        #region ==== リンクパッシブ処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null
            || chara_once.m_LinkParam == null)
            {
                continue;
            }

            // リンクキャラ情報取得
            MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
            if (charaParam == null)
            {
                continue;
            }

            // リンクパッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // Rate数増減
            //------------------------------
            // Rate数を加算
            rate += PassiveHandsRate(passiveParam);
        }
        #endregion

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキルによるHANDS時のRate数増減
        @param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
        @return		float	[Rate増減数]
        @add		Developer 2015/09/12 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static private float PassiveHandsRate(MasterDataSkillPassive passiveParam)
    {
        float rate = 0.0f;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null
        || passiveParam.skill_type != MasterDataDefineLabel.PassiveSkillCategory.HANDS_RATE)
        {
            return (rate);
        }

        // Rate数を取得
        rate = GetDBRevisionValue(passiveParam.Get_HANDS_RATE_VALUE());

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：ふんばりチェック	<static>
        @param[in]	int				(nowhp)			パーティの現在体力
        @param[in]	int				(maxhp)			パーティの最大体力
        @param[in]	int				(hp)			ダメージを受ける前の体力
        @param[out]	ref int			(charaID)		発動キャラ
        @param[out]	ref uint		(passiveID)		発動スキル
        @param[in]	ref ESKILLTYPE	(skillType)		スキルタイプ
        @retval		bool	[ふんばり/死]
        @change		Developer 2015/09/15 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public BattleSceneUtil.MultiInt PassiveChkFunbari(BattleSceneUtil.MultiInt nowhp, BattleSceneUtil.MultiInt maxhp, BattleSceneUtil.MultiInt before_hp, ref GlobalDefine.PartyCharaIndex charaID, ref uint passiveID, ref ESKILLTYPE skillType)
    {
        BattleSceneUtil.MultiInt result = new BattleSceneUtil.MultiInt();
        float chkOverlap = 10.0f;		// 重複防止用に大きめの値を初期値とする
        float tempRate = 0.0f;
        bool funbari = false;


        //----------------------------------------
        // 最も有効な値を取得
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //----------------------------------------
        //------------------------------
        // パッシブスキル：全キャラチェック
        //------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null)
            {
                continue;
            }

            // パッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //----------------------------------------
            // ふんばり
            //----------------------------------------
            tempRate = PassiveFunbari(passiveParam);

            //----------------------------------------
            // しきい値選定(低い方が発動しやすい)：重複対策
            //----------------------------------------
            if (chkOverlap <= tempRate
            || tempRate <= 0.0f)
            {
                continue;
            }

            // スキルタイプを設定
            skillType = ESKILLTYPE.ePASSIVE;

            // 判定前情報を設定
            chkOverlap = tempRate;				// しきい値
            charaID = (GlobalDefine.PartyCharaIndex)num;					// キャラ番号
            passiveID = passiveParam.fix_id;	// スキル番号
            funbari = true;					// 判定可能フラグ
        }
        #endregion

        //------------------------------
        // リンクパッシブ：全キャラチェック
        //------------------------------
        #region ==== リンクパッシブ処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null
            || chara_once.m_LinkParam == null)
            {
                continue;
            }

            // リンクキャラ情報取得
            MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
            if (charaParam == null)
            {
                continue;
            }

            // リンクパッシブ取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //----------------------------------------
            // ふんばり
            //----------------------------------------
            tempRate = PassiveFunbari(passiveParam);

            //----------------------------------------
            // しきい値選定(低い方が発動しやすい)：重複対策
            //----------------------------------------
            if (chkOverlap <= tempRate
            || tempRate <= 0.0f)
            {
                continue;
            }

            // スキルタイプを設定
            skillType = ESKILLTYPE.eLINKPASSIVE;

            // 判定前情報を設定
            chkOverlap = tempRate;				// しきい値
            charaID = (GlobalDefine.PartyCharaIndex)num;					// キャラ番号
            passiveID = passiveParam.fix_id;	// スキル番号
            funbari = true;					// 判定可能フラグ
        }
        #endregion

        // 判定前情報が設定できていない場合
        if (funbari == false)
        {
            return (result);
        }

        // ふんばり判定(被ダメージ前のHPが、最大HP * しきい値より低い場合、発動しない)
        for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
        {
            if (before_hp.getValue((GlobalDefine.PartyCharaIndex)idx) >= (int)AvoidErrorMultiple(maxhp.getValue((GlobalDefine.PartyCharaIndex)idx), chkOverlap))
            {
                result.setValue((GlobalDefine.PartyCharaIndex)idx, 1);
            }
        }

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "即死回避効果"
            + " (" + skillType.ToString() + "スキル:"
            + charaID.ToString() + ")"
            + "(HP" + InGameUtilBattle.getDebugPercentString(chkOverlap) + "%以上):"
            + ((result.getValue(GlobalDefine.PartyCharaIndex.LEADER) > 0) ? "LEADER " : "")
            + ((result.getValue(GlobalDefine.PartyCharaIndex.MOB_1) > 0) ? "MOB_1 " : "")
            + ((result.getValue(GlobalDefine.PartyCharaIndex.MOB_2) > 0) ? "MOB_2 " : "")
            + ((result.getValue(GlobalDefine.PartyCharaIndex.MOB_3) > 0) ? "MOB_3 " : "")
            + ((result.getValue(GlobalDefine.PartyCharaIndex.FRIEND) > 0) ? "FRIEND " : "")
        );

        return (result);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：ふんばり	<static>
        @param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
        @retval		float					(result)		[ふんばり/死]
        @add		Developer 2015/09/15 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static public float PassiveFunbari(MasterDataSkillPassive passiveParam)
    {
        float result = 10.0f;		// 重複防止用に大きめの値を初期値とする

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null)
        {
            return (result);
        }

        MasterDataSkillPassive.FunbariInfo funbari_info = passiveParam.Get_FunbariInfo();
        if (funbari_info == null)
        {
            return (result);
        }

        //----------------------------------------
        // しきい値選定(低い方が発動しやすい)：重複対策
        //----------------------------------------
        result = GetDBRevisionValue(funbari_info.m_HpBorder);

        return (result);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：体力最大時のダメージ軽減倍率	<static>
        @param[in]	int				(playerHP)			判定したいときのHP
        @param[out]	ref int			(owner)				スキル発動者
        @param[in]	ref ESKILLTYPE	(skillType)			スキルタイプ
        @return		float			[ダメージ軽減倍率]
        @note		InGame関連以外からの呼出しは行わないでください。
        @change		Developer 2015/09/15 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public BattleSceneUtil.MultiFloat PassiveChkDamageReduceFullHP(BattleSceneUtil.MultiInt playerHP, ref GlobalDefine.PartyCharaIndex owner, ref ESKILLTYPE skillType)
    {
        BattleSceneManager battleMgr = BattleSceneManager.Instance;
        BattleSceneUtil.MultiFloat rate = new BattleSceneUtil.MultiFloat(0.0f);

        //------------------------------
        // エラーチェック
        //------------------------------
        if (BattleParam.isActiveBattle() == false)
        {
            return (rate);
        }

        //----------------------------------------
        // 体力最大時判定
        //----------------------------------------
        BattleSceneUtil.MultiFloat hp_max = new BattleSceneUtil.MultiFloat(0.0f);
        for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.ALIVE);
            if (chara_once != null)
            {
                if (playerHP.getValue((GlobalDefine.PartyCharaIndex)idx) == BattleParam.m_PlayerParty.m_HPMax.getValue((GlobalDefine.PartyCharaIndex)idx))
                {
                    hp_max.setValue((GlobalDefine.PartyCharaIndex)idx, 1.0f);
                    break;
                }
            }
        }
        if (hp_max.getValue(GlobalDefine.PartyCharaIndex.MAX) == 0.0f)
        {
            return (rate);
        }

        float tempRate = 0.0f;
        float chkOverlap = 0.0f;
        bool skillFlag = false;

        //----------------------------------------
        // 最も有効な値を取得
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //----------------------------------------
        //------------------------------
        // パッシブスキル：全キャラチェック
        //------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null)
            {
                continue;
            }

            // キャラ情報取得
            MasterDataParamChara charaParam = chara_once.m_CharaMasterDataParam;
            if (charaParam == null)
            {
                continue;
            }

            // パッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // 該当スキルかどうかチェック
            //------------------------------
            if (passiveParam.skill_type != MasterDataDefineLabel.PassiveSkillCategory.GUARD_FULL_HP)
            {
                continue;
            }

            //----------------------------------------
            // 体力最大時のダメージ軽減倍率
            //----------------------------------------
            tempRate = PassiveDamageReduceFullHP(passiveParam);

            // 発動が1回目の場合
            if (skillFlag == false)
            {
                // スキル発動フラグON
                skillFlag = true;
                chkOverlap = tempRate;
                owner = (GlobalDefine.PartyCharaIndex)num;
                skillType = ESKILLTYPE.ePASSIVE;
            }

            //----------------------------------------
            // 軽減倍率選定：重複対策
            //----------------------------------------
            if (chkOverlap >= tempRate)
            {
                continue;
            }

            chkOverlap = tempRate;
            owner = (GlobalDefine.PartyCharaIndex)num;
            skillType = ESKILLTYPE.ePASSIVE;
        }
        #endregion

        //------------------------------
        // リンクパッシブ：全リンクキャラチェック
        //------------------------------
        #region ==== リンクパッシブ処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null
            || chara_once.m_LinkParam == null)
            {
                continue;
            }

            // リンクキャラ情報取得
            MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
            if (charaParam == null)
            {
                continue;
            }

            // リンクパッシブ取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // 該当スキルかどうかチェック
            //------------------------------
            if (passiveParam.skill_type != MasterDataDefineLabel.PassiveSkillCategory.GUARD_FULL_HP)
            {
                continue;
            }

            //----------------------------------------
            // 体力最大時のダメージ軽減倍率
            //----------------------------------------
            tempRate = PassiveDamageReduceFullHP(passiveParam);

            // 発動が1回目の場合
            if (skillFlag == false)
            {
                // スキル発動フラグON
                skillFlag = true;
                chkOverlap = tempRate;
                owner = (GlobalDefine.PartyCharaIndex)num;
                skillType = ESKILLTYPE.eLINKPASSIVE;
            }

            //----------------------------------------
            // 軽減倍率選定：重複対策
            //----------------------------------------
            if (chkOverlap >= tempRate)
            {
                continue;
            }

            chkOverlap = tempRate;
            owner = (GlobalDefine.PartyCharaIndex)num;
            skillType = ESKILLTYPE.eLINKPASSIVE;
        }
        #endregion

        //------------------------------
        // スキルが発動していた場合
        //------------------------------
        if (skillFlag == true)
        {
            rate.setValue(GlobalDefine.PartyCharaIndex.MAX, hp_max);
            rate.mulValue(GlobalDefine.PartyCharaIndex.MAX, chkOverlap);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：体力最大時のダメージ軽減倍率	<static>
        @param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
        @return		float					(rate)			[ダメージ軽減倍率]
        @note		InGame関連以外からの呼出しは行わないでください。
        @add		Developer 2015/09/11 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static private float PassiveDamageReduceFullHP(MasterDataSkillPassive passiveParam)
    {
        float rate = 0.0f;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null
        || passiveParam.skill_type != MasterDataDefineLabel.PassiveSkillCategory.GUARD_FULL_HP)
        {
            return (rate);
        }

        // 補正倍率を取得
        rate = 1.0f - GetDBRevisionValue(passiveParam.Get_GUARD_FULL_HP_RATE());

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：バトルリジェネ	<static>
        @param[out]	ref int			(owner)			スキル発動者
        @return		float			[回復：割合]
        @note		InGame関連以外からの呼出しは行わないでください。
        @change		Developer 2015/09/11 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public float PassiveChkHealHPBattle(ref GlobalDefine.PartyCharaIndex owner)
    {
        float rate = 0.0f;

        float tempRate = 0.0f;

        //----------------------------------------
        // 最も有効な値を取得
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //----------------------------------------
        //------------------------------
        // パッシブスキル：全キャラチェック
        //------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null)
            {
                continue;
            }

            // パッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            // 発動者を仮設定(フロア開始時カットイン用：初期値でなければよい)
            if (owner == GlobalDefine.PartyCharaIndex.MAX)
            {
                owner = (GlobalDefine.PartyCharaIndex)num;
            }

            //----------------------------------------
            // バトルリジェネ
            //----------------------------------------
            tempRate = PassiveHealHPBattle(passiveParam);

            //----------------------------------------
            // バトル時回復選定(割合)：重複対策
            //----------------------------------------
            if (rate >= tempRate)
            {
                continue;
            }

            rate = tempRate;
            owner = (GlobalDefine.PartyCharaIndex)num;
        }
        #endregion

        //------------------------------
        // リンクパッシブ：全リンクキャラチェック
        //------------------------------
        #region ==== リンクパッシブ処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null
            || chara_once.m_LinkParam == null)
            {
                continue;
            }

            // リンクキャラ情報取得
            MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
            if (charaParam == null)
            {
                continue;
            }

            // リンクパッシブ取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            // 発動者を仮設定(フロア開始時カットイン用：初期値でなければよい)
            if (owner == GlobalDefine.PartyCharaIndex.MAX)
            {
                owner = (GlobalDefine.PartyCharaIndex)num;
            }

            //----------------------------------------
            // バトルリジェネ
            //----------------------------------------
            tempRate = PassiveHealHPBattle(passiveParam);

            //----------------------------------------
            // バトル時回復選定(割合)：重複対策
            //----------------------------------------
            if (rate >= tempRate)
            {
                continue;
            }

            rate = tempRate;
            owner = (GlobalDefine.PartyCharaIndex)num;
        }
        #endregion

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：バトルリジェネ	<static>
        @param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
        @return		float					(rate)			[回復：割合]
        @note		InGame関連以外からの呼出しは行わないでください。
                    Developer 2015/09/11 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static public float PassiveHealHPBattle(MasterDataSkillPassive passiveParam)
    {
        float rate = 0.0f;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null
        || passiveParam.skill_type != MasterDataDefineLabel.PassiveSkillCategory.HEAL_HP_BATTLE)
        {
            return (rate);
        }

        //----------------------------------------
        // 割合取得
        //----------------------------------------
        rate = GetDBRevisionValue(passiveParam.Get_HEAL_HP_BATTLE_RATE());

        return (rate);
    }

    //------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：[追撃]発動チェック		<static>
        @param[in]	ESKILLTYPE		(skillType)		スキルタイプ
        @retval		bool	[発動/発動しない]
        @change		Developer 2015/09/11 ver300	リンクパッシブ対応
    */
    //------------------------------------------------------------------------
    public static bool PassiveChkFollowAttack(ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE)
    {
        bool result = false;

        //------------------------------
        //	エラーチェック
        //------------------------------
        if (BattleParam.isActiveBattle() == false)
        {
            return (result);
        }

        //------------------------------
        // スキルタイプによる分岐
        //------------------------------
        switch (skillType)
        {
            //------------------------------
            // パッシブスキル：全キャラチェック
            //------------------------------
            #region ==== パッシブスキル処理 ====
            case ESKILLTYPE.ePASSIVE:
                {
                    for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
                    {
                        CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
                        // メンバーが設定されているかチェック
                        if (chara_once == null)
                        {
                            continue;
                        }

                        // パッシブスキル取得
                        MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
                        if (passiveParam == null)
                        {
                            continue;
                        }

                        // 該当スキルかどうかチェック
                        MasterDataSkillPassive.FollowAttackInfo follow_attack_info = passiveParam.Get_FollowAttackInfo();
                        if (follow_attack_info == null)
                        {
                            continue;
                        }

                        result = true;
                        break;
                    }
                    break;
                }
            #endregion

            //------------------------------
            // リンクパッシブ：全リンクキャラチェック
            //------------------------------
            #region ==== リンクパッシブ処理 ====
            case ESKILLTYPE.eLINKPASSIVE:
                {
                    for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
                    {
                        CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
                        // メンバーが設定されているかチェック
                        if (chara_once == null
                        || chara_once.m_LinkParam == null)
                        {
                            continue;
                        }

                        // リンクキャラ情報取得
                        MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
                        if (charaParam == null)
                        {
                            continue;
                        }

                        // リンクパッシブ取得
                        MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
                        if (passiveParam == null)
                        {
                            continue;
                        }

                        // 該当スキルかどうかチェック
                        MasterDataSkillPassive.FollowAttackInfo follow_attack_info = passiveParam.Get_FollowAttackInfo();
                        if (follow_attack_info == null)
                        {
                            continue;
                        }

                        result = true;
                        break;
                    }
                    break;
                }
                #endregion
        }

        return (result);
    }

    //------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：追撃ダメージ取得	<static>
        @param[in]	CharaOnce		(chara)				攻撃者
        @param[in]	CharaOnce		(target)			対象者
        @param[in]	int				(nAilmentID)		状態異常管理ID
        @param[in]	ESKILLTYPE		(skillType)			スキルタイプ
        @return		int				[追撃ダメージ値]
    */
    //------------------------------------------------------------------------
    static public int GetPassiveDamageFollow(CharaOnce chara, CharaOnce target,
        BattleEnemy enemyParam,
        BattleSkillActivity skillActivity)
    {
        //----------------------------------------
        // キャラ情報無し
        //----------------------------------------
        if (chara == null || target == null)
        {
            return 0;
        }

        //----------------------------------------
        // 追撃スキル倍率
        //----------------------------------------
        float skillRate = 1.0f;
        if (!chara.m_bHasCharaMasterDataParam)
        {
            return 0;
        }

        MasterDataSkillPassive passiveParam = null;
        //------------------------------
        // スキルタイプによる分岐
        //------------------------------
        switch (skillActivity.m_SkillType)
        {
            //------------------------------
            // パッシブスキル
            //------------------------------
            case ESKILLTYPE.ePASSIVE:
                {
                    // パッシブスキル取得
                    passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara.m_CharaMasterDataParam.skill_passive);
                    break;
                }
            //------------------------------
            // リンクパッシブ
            //------------------------------
            case ESKILLTYPE.eLINKPASSIVE:
                {
                    if (chara.m_LinkParam == null)
                    {
                        return 0;
                    }

                    MasterDataParamChara linkCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(chara.m_LinkParam.m_CharaID);
                    if (linkCharaMaster == null)
                    {
                        return 0;
                    }

                    passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(linkCharaMaster.link_skill_passive);
                    break;
                }
        }

        //	該当スキルかどうかチェック
        if (passiveParam == null)
        {
            return 0;
        }

        MasterDataSkillPassive.FollowAttackInfo follow_attack_info = passiveParam.Get_FollowAttackInfo();
        if (follow_attack_info == null)
        {
            return 0;
        }

        // 倍率を取得
        skillRate = GetDBRevisionValue(follow_attack_info.m_AttackPercent);


        //----------------------------------------
        // リーダースキル補正
        //----------------------------------------
        float leaderPow = 1.0f;
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex.LEADER, chara));
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex.FRIEND, chara));
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRateCondition(GlobalDefine.PartyCharaIndex.LEADER));
        leaderPow = AvoidErrorMultiple(leaderPow, GetLeaderSkillDamageRateCondition(GlobalDefine.PartyCharaIndex.FRIEND));

        //----------------------------------------
        // パッシブスキル補正
        //----------------------------------------
        GlobalDefine.PartyCharaIndex owner = GlobalDefine.PartyCharaIndex.MAX;	// ダミー（未使用）
        float passivePow = 1.0f;
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkSkillDamageRate(chara));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkKindKillerAtk(chara));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkDamageUPFloorPanel(ref owner));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkDamageUPElemNum(ref owner));
        passivePow = AvoidErrorMultiple(passivePow, PassiveChkDamageUPHandsNum(ref owner));

        //----------------------------------------
        // リンクパッシブスキル補正
        //----------------------------------------
        float fLinkPassivePow = 1.0f;
        uint unLinkUnitID = 0;
        if (chara.m_LinkParam != null)
        {
            unLinkUnitID = chara.m_LinkParam.m_CharaID;
        }
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkSkillDamageRate(chara, ESKILLTYPE.eLINKPASSIVE));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkKindKillerAtk(chara, ESKILLTYPE.eLINKPASSIVE));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkDamageUPFloorPanel(ref owner, ESKILLTYPE.eLINKPASSIVE, unLinkUnitID));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkDamageUPElemNum(ref owner, ESKILLTYPE.eLINKPASSIVE, unLinkUnitID));
        fLinkPassivePow = AvoidErrorMultiple(fLinkPassivePow, PassiveChkDamageUPHandsNum(ref owner, ESKILLTYPE.eLINKPASSIVE, unLinkUnitID));

        //----------------------------------------
        // 状態異常によるダメージ増減
        //----------------------------------------
        StatusAilmentChara nAilmentID = BattleParam.m_PlayerParty.m_Ailments.getAilment(chara.m_PartyCharaIndex);
        float fAilmentRate = 1.0f;
        // 攻撃力倍率
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, nAilmentID.GetOffenceRate());
        // 属性攻撃力倍率
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, nAilmentID.GetOffenceElementRate(chara.m_CharaMasterDataParam.element));
        // 種族攻撃力倍率(メイン)
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, nAilmentID.GetOffenceKindRate(chara.kind));
        // 種族攻撃力倍率(サブ)
        fAilmentRate = AvoidErrorMultiple(fAilmentRate, nAilmentID.GetOffenceKindRate(chara.kind_sub));

        //----------------------------------------
        // v390 Developer add
        // 敵特性による補正の適用
        // 今回追加した「ダメ補正：指定スキル」以外の特性は後続の処理ではじかれてる
        //----------------------------------------
        float fEnemyAbilityRate = 1.0f;
        fEnemyAbilityRate = EnemyAbility.GetAbilityDamageRatePlayer(skillActivity, enemyParam);

        //----------------------------------------
        // 攻撃力
        //----------------------------------------
        float fCharaPow = chara.m_CharaPow;

        //----------------------------------------
        // 属性倍率
        //----------------------------------------
        float fElementRate = GetSkillElementRate(follow_attack_info.m_ElementType, target.m_CharaMasterDataParam.element);

        //----------------------------------------
        // コンボボーナス
        //----------------------------------------
        //		float fSkillCountRate = GetSkillCountRate( skillCount );

        //----------------------------------------
        // ダメージ計算
        //----------------------------------------
        float damageValue = 1.0f;
        damageValue = AvoidErrorMultiple(damageValue, fCharaPow);
        damageValue = AvoidErrorMultiple(damageValue, skillRate);
        damageValue = AvoidErrorMultiple(damageValue, fElementRate);
        damageValue = AvoidErrorMultiple(damageValue, leaderPow);
        damageValue = AvoidErrorMultiple(damageValue, passivePow);
        damageValue = AvoidErrorMultiple(damageValue, fLinkPassivePow);
        damageValue = AvoidErrorMultiple(damageValue, fAilmentRate);
        damageValue = AvoidErrorMultiple(damageValue, fEnemyAbilityRate); //v390 Developer add

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "パッシブスキル：追撃ダメージ値:" + ((int)damageValue).ToString()
            + " = " + ((int)fCharaPow).ToString() + "(キャラ基本攻撃力)"
            + " x" + InGameUtilBattle.getDebugPercentString(skillRate) + "%(スキル威力)"
            + " x" + InGameUtilBattle.getDebugPercentString(fElementRate) + "%(属性補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(leaderPow) + "%(リーダースキル補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(passivePow) + "%(パッシブスキル補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(fLinkPassivePow) + "%(リンクパッシブスキル補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(fAilmentRate) + "%(状態異常補正)"
            + " x" + InGameUtilBattle.getDebugPercentString(fEnemyAbilityRate) + "%(敵特性補正)"
        );

        return ((int)damageValue);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：指定属性の手札を指定属性に変換	<static>
        @param[in]	int			(element_hand)			変換元手札
        @param[out]	ref int		(owner)					スキル発動者
        @return		int			[変換後]
        @change		Developer 2015/09/11 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public MasterDataDefineLabel.ElementType PassiveChkChangeHandElem(MasterDataDefineLabel.ElementType element_hand, ref GlobalDefine.PartyCharaIndex owner)
    {
        MasterDataDefineLabel.ElementType element = element_hand;

        bool skillFlag = false;

        //----------------------------------------
        // 最初の発動情報を取得
        // ※パッシブとリンクパッシブは、切り分けておく
        //----------------------------------------
        //------------------------------
        // パッシブスキル：全キャラチェック
        //------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            // メンバーが設定されているかチェック
            if (chara_once == null)
            {
                continue;
            }

            // パッシブスキル取得
            MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //----------------------------------------
            // 指定属性の手札を指定属性に変換
            //----------------------------------------
            element = PassiveChangeHandElem(passiveParam, element, ref skillFlag);

            //----------------------------------------
            // 重複対策(パッシブで該当スキルが発動できるのは、1ユニット：パーティ番号で若いユニットを優先)
            //----------------------------------------
            if (skillFlag == true)
            {
                owner = (GlobalDefine.PartyCharaIndex)num;
                break;
            }
        }
        #endregion

        //------------------------------
        // リンクパッシブ：全リンクキャラチェック
        //------------------------------
        #region ==== リンクパッシブ処理 ====
        // パッシブで発動していない場合
        if (skillFlag == false)
        {
            for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
            {
                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
                // メンバーが設定されているかチェック
                if (chara_once == null
                || chara_once.m_LinkParam == null)
                {
                    continue;
                }

                // リンクキャラ情報取得
                MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
                if (charaParam == null)
                {
                    continue;
                }

                // リンクパッシブ取得
                MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
                if (passiveParam == null)
                {
                    continue;
                }

                // 発動者を仮設定(フロア開始時カットイン用：初期値でなければよい)
                if (owner == GlobalDefine.PartyCharaIndex.MAX)
                {
                    owner = (GlobalDefine.PartyCharaIndex)num;
                }

                //----------------------------------------
                // 指定属性の手札を指定属性に変換
                //----------------------------------------
                element = PassiveChangeHandElem(passiveParam, element, ref skillFlag);

                //----------------------------------------
                // 重複対策(パッシブで該当スキルが発動できるのは、1ユニット：パーティ番号で若いユニットを優先)
                //----------------------------------------
                if (skillFlag == true)
                {
                    owner = (GlobalDefine.PartyCharaIndex)num;
                    break;
                }
            }
        }
        #endregion

        return (element);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：指定属性の手札を指定属性に変換	<static>
        @param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
        @param[in]	int						(element_hand)	変換元手札
        @param[out]	ref int					(owner)			スキル発動者
        @return		int						(element)		[変換後]
        @note		Developer 2015/09/08 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static private MasterDataDefineLabel.ElementType PassiveChangeHandElem(MasterDataSkillPassive passiveParam, MasterDataDefineLabel.ElementType element_hand, ref bool skillFlag)
    {
        MasterDataDefineLabel.ElementType element = element_hand;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null
        || passiveParam.skill_type != MasterDataDefineLabel.PassiveSkillCategory.CNG_HAND_ELEM)
        {
            return (element);
        }

        // スキルの発動条件判定(指定属性の場合)
        if (element == passiveParam.Get_CNG_HAND_ELEM_ROOT())
        {
            // 設定された属性に変換
            element = passiveParam.Get_CNG_HAND_ELEM_DEST();
        }
        skillFlag = true;

        return (element);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：フロアパネルめくった枚数に応じてダメージUP	<static>
        @param[out]	ref int			(owner)			スキル発動者
        @param[in]	ESKILLTYPE		(skillType)		スキルタイプ
        @param[in]	uint			(linkUnitID)	リンクユニットID
        @return		float			[ダメージ倍率]
        @note		カットイン演出未確定のため表示されません(2015/05/22)
                    CheckPrePassiveSkill内で、カットイン判定から間引いています。
                    現在、演出時のスキル発動者は、適当に決められています。
                    (2015/06/23)
                    v280カットイン実装
        @change		Developer 2015/09/11 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public float PassiveChkDamageUPFloorPanel(ref GlobalDefine.PartyCharaIndex owner, ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE, uint linkUnitID = 0)
    {
        float rate = 1.0f;
        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：指定色以上で攻撃した際にダメージUP
        @param[out]	ref int			(owner)			スキル発動者
        @param[in]	ESKILLTYPE		(skillType)		スキルタイプ
        @param[in]	uint			(linkUnitID)	リンクユニットID
        @return		float		[攻撃力倍率]
        @note
        @change		Developer 2015/09/11 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public float PassiveChkDamageUPElemNum(ref GlobalDefine.PartyCharaIndex owner, ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE, uint linkUnitID = 0)
    {
        float rate = 1.0f;
        float chkOverlap = 0.0f;
        bool skillFlag = false;

        //------------------------------
        // 属性色をカウント
        //------------------------------
        int elemCnt = BattleSceneManager.Instance.getActiveSkillElementCount();

        //------------------------------
        // スキルタイプによる分岐
        //------------------------------
        switch (skillType)
        {
            //------------------------------
            // パッシブスキル：全キャラチェック
            //------------------------------
            #region ==== パッシブスキル処理 ====
            case ESKILLTYPE.ePASSIVE:
                {
                    float tempRate = 0.0f;

                    //------------------------------
                    // 全てのキャラのパッシブスキルをチェック
                    //------------------------------
                    for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
                    {
                        CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
                        // メンバーが設定されているかチェック
                        if (chara_once == null)
                        {
                            continue;
                        }

                        // パッシブスキル取得
                        MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
                        if (passiveParam == null)
                        {
                            continue;
                        }

                        // 発動者を仮設定(フロア開始時カットイン用：初期値でなければよい)
                        if (owner == GlobalDefine.PartyCharaIndex.MAX)
                        {
                            owner = (GlobalDefine.PartyCharaIndex)num;
                        }

                        //----------------------------------------
                        // 指定色以上で攻撃した際にダメージUP
                        //----------------------------------------
                        tempRate = PassiveDamageUPElemNum(passiveParam, elemCnt, ref skillFlag);

                        //----------------------------------------
                        // 倍率選定：重複対策
                        //----------------------------------------
                        if (chkOverlap >= tempRate)
                        {
                            continue;
                        }

                        chkOverlap = tempRate;
                        owner = (GlobalDefine.PartyCharaIndex)num;
                    }
                    break;
                }
            #endregion

            //------------------------------
            // リンクパッシブ：リンクユニット単体チェック
            //------------------------------
            #region ==== リンクパッシブ処理 ====
            case ESKILLTYPE.eLINKPASSIVE:
                {
                    // リンクキャラ情報取得
                    MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(linkUnitID);
                    if (charaParam == null)
                    {
                        return (rate);
                    }

                    // リンクパッシブ取得
                    MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
                    if (passiveParam == null)
                    {
                        return (rate);
                    }

                    //----------------------------------------
                    // 指定色以上で攻撃した際にダメージUP
                    //----------------------------------------
                    chkOverlap = PassiveDamageUPElemNum(passiveParam, elemCnt, ref skillFlag);
                }
                break;
                #endregion
        }


        //------------------------------
        // バトル中にスキル発動した場合
        //------------------------------
        if (BattleParam.isActiveBattle() == true
        && skillFlag == true)
        {
            // 戻り値に代入：誤差回避用計算
            rate = AvoidErrorMultiple(rate, chkOverlap);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：指定色以上で攻撃した際にダメージUP
        @param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
        @param[in]	int						(elemCnt)		属性数
        @param[out]	ref bool				(skillFlag)		スキル発動有無
        @return		float		[攻撃力倍率]
        @note		Developer 2015/09/11 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static private float PassiveDamageUPElemNum(MasterDataSkillPassive passiveParam, int elemCnt, ref bool skillFlag)
    {
        float rate = 0.0f;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null
        || passiveParam.skill_type != MasterDataDefineLabel.PassiveSkillCategory.DMGUP_ELEM_NUM)
        {
            return (rate);
        }

        // 属性数の判定(カットイン演出のため、バトル中のみ判定する)
        if (BattleParam.isActiveBattle() == true
        && elemCnt < passiveParam.Get_DMGUP_ELEM_NUM_CNT())
        {
            return (rate);
        }

        //----------------------------------------
        // 倍率取得
        //----------------------------------------
        rate = GetDBRevisionValue(passiveParam.Get_DMGUP_ELEM_NUM_SCALE());
        skillFlag = true;

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：指定HANDS以上で攻撃した際にダメージUP
        @param[out]	ref int			(owner)			スキル発動者
        @param[in]	ESKILLTYPE		(skillType)		スキルタイプ
        @param[in]	uint			(linkUnitID)	リンクユニットID
        @return		float		[攻撃力倍率]
        @change		Developer 2015/09/11 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    static public float PassiveChkDamageUPHandsNum(ref GlobalDefine.PartyCharaIndex owner, ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE, uint linkUnitID = 0)
    {
        float rate = 1.0f;

        float chkOverlap = 0.0f;
        bool skillFlag = false;

        //------------------------------
        // HANDS数を取得
        //------------------------------
        int handNum = BattleSceneManager.Instance.getActiveSkillComboCount();

        //------------------------------
        // スキルタイプによる分岐
        //------------------------------
        switch (skillType)
        {
            //------------------------------
            // パッシブスキル：全キャラチェック
            //------------------------------
            #region ==== パッシブスキル処理 ====
            case ESKILLTYPE.ePASSIVE:
                {
                    float tempRate = 0.0f;

                    //------------------------------
                    // 全てのキャラのパッシブスキルをチェック
                    //------------------------------
                    for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
                    {
                        CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
                        // メンバーが設定されているかチェック
                        if (chara_once == null)
                        {
                            continue;
                        }

                        // パッシブスキル取得
                        MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
                        if (passiveParam == null)
                        {
                            continue;
                        }

                        // 発動者を仮設定(フロア開始時カットイン用：初期値でなければよい)
                        if (owner == GlobalDefine.PartyCharaIndex.MAX)
                        {
                            owner = (GlobalDefine.PartyCharaIndex)num;
                        }

                        //----------------------------------------
                        // 指定HANDS以上で攻撃した際にダメージUP
                        //----------------------------------------
                        tempRate = PassiveDamageUPHandsNum(passiveParam, handNum, ref skillFlag);

                        //----------------------------------------
                        // 倍率選定：重複対策
                        //----------------------------------------
                        if (chkOverlap >= tempRate)
                        {
                            continue;
                        }

                        chkOverlap = tempRate;
                        owner = (GlobalDefine.PartyCharaIndex)num;
                    }
                    break;
                }
            #endregion

            //------------------------------
            // リンクパッシブ：リンクユニット単体チェック
            //------------------------------
            #region ==== リンクパッシブ処理 ====
            case ESKILLTYPE.eLINKPASSIVE:
                {
                    MasterDataParamChara charaParam = BattleParam.m_MasterDataCache.useCharaParam(linkUnitID);
                    if (charaParam == null)
                    {
                        return (rate);
                    }

                    // リンクパッシブ取得
                    MasterDataSkillPassive passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
                    if (passiveParam == null)
                    {
                        return (rate);
                    }

                    //----------------------------------------
                    // 指定色以上で攻撃した際にダメージUP
                    //----------------------------------------
                    chkOverlap = PassiveDamageUPHandsNum(passiveParam, handNum, ref skillFlag);

                    // 最初の発動者を仮設定(フロア開始時カットイン用)
                    //if( owner == GlobalDefine.PARTY_CHARA_MAX )
                    //{
                    //	owner = num;
                    //}
                }
                break;
                #endregion
        }


        //------------------------------
        // バトル中にスキル発動した場合
        //------------------------------
        if (BattleParam.isActiveBattle() == true
        && skillFlag == true)
        {
            // 戻り値に代入：誤差回避用計算
            rate = AvoidErrorMultiple(rate, chkOverlap);
        }

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル：指定HANDS以上で攻撃した際にダメージUP
        @param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
        @param[in]	int						(handNum)		HANDS数
        @param[out]	ref bool				(skillFlag)		スキル発動有無
        @return		float		[攻撃力倍率]
        @note		Developer 2015/09/11 ver300 根本の処理部分を分離
    */
    //----------------------------------------------------------------------------
    static private float PassiveDamageUPHandsNum(MasterDataSkillPassive passiveParam, int handNum, ref bool skillFlag)
    {
        float rate = 0.0f;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null)
        {
            return (rate);
        }

        MasterDataSkillPassive.HandsAttackUpInfo hands_attack_up_info = passiveParam.Get_HandsAttackUpInfo();
        if (hands_attack_up_info == null)
        {
            return (rate);
        }

        // HANDS数の判定(カットイン演出のため、バトル中のみ判定する)
        if (BattleParam.isActiveBattle() == true
        && handNum < hands_attack_up_info.m_HandsCount)
        {
            return (rate);
        }

        //----------------------------------------
        // 倍率取得
        //----------------------------------------
        rate = GetDBRevisionValue(hands_attack_up_info.m_AttackPercent);
        skillFlag = true;

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		スキル効果の相性倍率				<static>
        @param[in]	int		(nElementSkill)		スキル属性
        @param[in]	int		(nElementTarget)	対象属性
        @return		float	[属性相性によるダメージ倍率]
    */
    //----------------------------------------------------------------------------
    static public float GetSkillElementRate(MasterDataDefineLabel.ElementType nElementSkill, MasterDataDefineLabel.ElementType nElementTarget)
    {
        //------------------------------
        // 情報構築前なら情報構築
        //------------------------------
        if (m_SkillElementRate == null)
        {
            m_SkillElementRate = new float[(int)MasterDataDefineLabel.ElementType.MAX][];
            for (int i = 0; i < m_SkillElementRate.Length; i++)
            {
                m_SkillElementRate[i] = new float[(int)MasterDataDefineLabel.ElementType.MAX];
                for (int j = 0; j < m_SkillElementRate[i].Length; j++)
                {
                    m_SkillElementRate[i][j] = GlobalDefine.ELEMENTRATE_NORMAL;
                }
            }

            //------------------------------
            // m_SkillElementRate[ 攻撃属性 ][ 被害者属性 ] = 倍率
            //------------------------------
            m_SkillElementRate[(int)MasterDataDefineLabel.ElementType.FIRE][(int)MasterDataDefineLabel.ElementType.WATER] = GlobalDefine.ELEMENTRATE_GUARD;
            m_SkillElementRate[(int)MasterDataDefineLabel.ElementType.FIRE][(int)MasterDataDefineLabel.ElementType.WIND] = GlobalDefine.ELEMENTRATE_WEEK;
            m_SkillElementRate[(int)MasterDataDefineLabel.ElementType.WATER][(int)MasterDataDefineLabel.ElementType.WIND] = GlobalDefine.ELEMENTRATE_GUARD;
            m_SkillElementRate[(int)MasterDataDefineLabel.ElementType.WATER][(int)MasterDataDefineLabel.ElementType.FIRE] = GlobalDefine.ELEMENTRATE_WEEK;
            m_SkillElementRate[(int)MasterDataDefineLabel.ElementType.WIND][(int)MasterDataDefineLabel.ElementType.FIRE] = GlobalDefine.ELEMENTRATE_GUARD;
            m_SkillElementRate[(int)MasterDataDefineLabel.ElementType.WIND][(int)MasterDataDefineLabel.ElementType.WATER] = GlobalDefine.ELEMENTRATE_WEEK;
            m_SkillElementRate[(int)MasterDataDefineLabel.ElementType.LIGHT][(int)MasterDataDefineLabel.ElementType.DARK] = GlobalDefine.ELEMENTRATE_WEEK;
            m_SkillElementRate[(int)MasterDataDefineLabel.ElementType.DARK][(int)MasterDataDefineLabel.ElementType.LIGHT] = GlobalDefine.ELEMENTRATE_WEEK;
        }

        //------------------------------
        // スキル相性倍率を返す
        //------------------------------
        return m_SkillElementRate[(int)nElementSkill][(int)nElementTarget];
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		スキル発動回数での効果倍率
        @param[in]	int		(nSkillCount)		スキル発動回数
        @return		float	[効果倍率]
    */
    //----------------------------------------------------------------------------
    static public float GetSkillCountRate(int nSkillCount)
    {
        int skillcnt = (nSkillCount - 1);
        if (skillcnt < 0)
        {
            skillcnt = 0;
        }

        //----------------------------------------
        // パッシブスキルによるRate数の増減
        //----------------------------------------
        float fHandsRate = InGameDefine.HANDS_RATE_DEFAULT;

        fHandsRate += PassiveChkHandsRate();

        // Rate数の下限上限のチェック
        if (fHandsRate < InGameDefine.HANDS_RATE_MIN)
        {
            fHandsRate = InGameDefine.HANDS_RATE_MIN;

            // 誤差対策(ここに入った時点で、小数点が代入されるので補正必須)
            fHandsRate += DEF_INGAME_AVOID_ERROR;
        }
        else if (fHandsRate > InGameDefine.HANDS_RATE_MAX)
        {
            fHandsRate = InGameDefine.HANDS_RATE_MAX;
        }

        return 1.0f + (skillcnt * fHandsRate);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		属性相性取得			<static>
        @param[in]	int					(elem)				属性
        @param[in]	int					(targetElem)		対象属性
        @return		EDAMAGE_TYPE		[属性相性]
    */
    //----------------------------------------------------------------------------
    static public EDAMAGE_TYPE GetSkillElementAffinity(MasterDataDefineLabel.ElementType elem, MasterDataDefineLabel.ElementType targetElem)
    {
        //----------------------------------------
        // 属性倍率
        //----------------------------------------
        float fAffinityRate = GetSkillElementRate(elem, targetElem);

        //----------------------------------------
        // 倍率から属性を特定
        //----------------------------------------
        EDAMAGE_TYPE eDamageType = EDAMAGE_TYPE.eDAMAGE_TYPE_NORMAL;
        if (fAffinityRate < GlobalDefine.ELEMENTRATE_NORMAL)
        {
            eDamageType = EDAMAGE_TYPE.eDAMAGE_TYPE_GUARD;
        }
        else if (fAffinityRate > GlobalDefine.ELEMENTRATE_NORMAL)
        {
            eDamageType = EDAMAGE_TYPE.eDAMAGE_TYPE_WEEK;
        }

        return eDamageType;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		弱点属性取得
        @param[in]	int		(elem)		属性
        @return		int		[弱点属性]
    */
    //----------------------------------------------------------------------------
    static public MasterDataDefineLabel.ElementType GetElementAffinity(MasterDataDefineLabel.ElementType elem)
    {
        switch (elem)
        {
            case MasterDataDefineLabel.ElementType.FIRE:
                return MasterDataDefineLabel.ElementType.WATER;
            case MasterDataDefineLabel.ElementType.WATER:
                return MasterDataDefineLabel.ElementType.WIND;
            case MasterDataDefineLabel.ElementType.WIND:
                return MasterDataDefineLabel.ElementType.FIRE;
            case MasterDataDefineLabel.ElementType.LIGHT:
                return MasterDataDefineLabel.ElementType.DARK;
            case MasterDataDefineLabel.ElementType.DARK:
                return MasterDataDefineLabel.ElementType.LIGHT;

            case MasterDataDefineLabel.ElementType.NAUGHT:
            case MasterDataDefineLabel.ElementType.HEAL:
            case MasterDataDefineLabel.ElementType.NONE:
                break;
        }

        // なし
        return MasterDataDefineLabel.ElementType.NONE;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		アクティブスキルSE再生			<static>
        @param[in]	int			(type)			スキルタイプ
        @param[in]	int			(element)		スキル属性
    */
    //------------------------------------------------------------------------
    static public void PlayActiveSkillSE(MasterDataDefineLabel.SkillType type, MasterDataDefineLabel.ElementType element)
    {
        switch (type)
        {
            case MasterDataDefineLabel.SkillType.ATK_ONCE:
            case MasterDataDefineLabel.SkillType.ATK_ALL:
                // 攻撃
                switch (element)
                {
                    case MasterDataDefineLabel.ElementType.FIRE:
                        // 炎
                        SoundUtil.PlaySE(SEID.SE_BATTLE_ATTACK_FIRE);
                        break;
                    case MasterDataDefineLabel.ElementType.WATER:
                        // 水
                        SoundUtil.PlaySE(SEID.SE_BATTLE_ATTACK_WATER);
                        break;
                    case MasterDataDefineLabel.ElementType.WIND:
                        // 風
                        SoundUtil.PlaySE(SEID.SE_BATTLE_ATTACK_WIND);
                        break;
                    case MasterDataDefineLabel.ElementType.NAUGHT:
                        // 無
                        SoundUtil.PlaySE(SEID.SE_BATTLE_ATTACK_NAUGHT);
                        break;
                    case MasterDataDefineLabel.ElementType.LIGHT:
                        // 光
                        SoundUtil.PlaySE(SEID.SE_BATTLE_ATTACK_LIGHT);
                        break;
                    case MasterDataDefineLabel.ElementType.DARK:
                        // 闇
                        SoundUtil.PlaySE(SEID.SE_BATTLE_ATTACK_DARK);
                        break;

                    case MasterDataDefineLabel.ElementType.HEAL:
                        // 回復
                        SoundUtil.PlaySE(SEID.SE_BATTLE_SKILL_HEAL);
                        break;

                    case MasterDataDefineLabel.ElementType.NONE:
                    default:
                        // 実行
                        SoundUtil.PlaySE(SEID.SE_BATLE_SKILL_EXEC);
                        break;
                }
                break;

            case MasterDataDefineLabel.SkillType.CURSE_ONCE:
            case MasterDataDefineLabel.SkillType.CURSE_ALL:
                SoundUtil.PlaySE(SEID.SE_BATTLE_DEBUFF);
                break;

            case MasterDataDefineLabel.SkillType.HEAL:
            case MasterDataDefineLabel.SkillType.HEAL_SP:
            case MasterDataDefineLabel.SkillType.RESURRECT:
                SoundUtil.PlaySE(SEID.SE_BATTLE_SKILL_HEAL);
                break;

            case MasterDataDefineLabel.SkillType.SUPPORT:
                SoundUtil.PlaySE(SEID.SE_BATTLE_BUFF);
                break;

            default:
                break;
        }
    }

    //------------------------------------------------------------------------
    /*!
        @brief		リーダースキル[追撃]発動チェック		<static>
        @param[in]	int		(nCharaID)		キャラクターID
        @retval		bool	[発動/発動しない]
    */
    //------------------------------------------------------------------------
    public static bool CheckLaeaderSkill(GlobalDefine.PartyCharaIndex nCharaID)
    {
        if (nCharaID < 0 || nCharaID >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return false;
        }

        // フレンドであればフレンド登録状態のチェック
        if (InGameUtil.ChkFriendRegisterStatus(nCharaID) == false)
        {
            return false;
        }

        CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember(nCharaID, CharaParty.CharaCondition.SKILL_LEADER);
        if (chara == null)
        {
            return false;
        }

        if (!chara.m_bHasCharaMasterDataParam)
        {
            return false;
        }

        MasterDataSkillLeader skill = BattleParam.m_MasterDataCache.useSkillLeader(chara.m_CharaMasterDataParam.skill_leader);
        if (skill == null)
        {
            return false;
        }

        if (skill.Is_skill_follow_atk_active())
        {
            return true;
        }

        return false;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		指定都合状態異常の全クリア：指定都合
        @param[in]	BattleSkillActivity		(activity)		スキル発動情報
    */
    //------------------------------------------------------------------------
    static public void ClearAilmentStatusCondition(BattleSkillActivity activity)
    {
        //------------------------------
        //	エラーチェック
        //------------------------------
        if (activity == null
        || BattleParam.m_PlayerParty == null
        || BattleParam.isActiveBattle() == false)
        {
            return;
        }

        switch (activity.Get_SUPPORT_SPESTATE_CLEAR_TARGET())
        {
            case MasterDataDefineLabel.TargetType.SELF:		// 自分
            case MasterDataDefineLabel.TargetType.FRIEND:	// 使用者の味方全員
                // プレイヤーの指定都合状態異常をクリア
                BattleParam.m_PlayerParty.m_Ailments.DelStatusAilmentCondition(activity.Get_SUPPORT_SPESTATE_CLEAR_GOOD_BAD());
                break;

            case MasterDataDefineLabel.TargetType.OTHER:	// 相手
            case MasterDataDefineLabel.TargetType.ENEMY:	// 使用者の敵全員
            case MasterDataDefineLabel.TargetType.ENE_N_1:
            case MasterDataDefineLabel.TargetType.ENE_1N_1:
            case MasterDataDefineLabel.TargetType.ENE_R_N:
            case MasterDataDefineLabel.TargetType.ENE_1_N:
                if (BattleParam.m_EnemyParam == null)
                {
                    break;
                }

                // 敵全サーチ
                for (int num = 0; num < BattleParam.m_EnemyParam.Length; ++num)
                {
                    if (BattleParam.m_EnemyParam[num] == null)
                    {
                        continue;
                    }

                    // 敵の指定都合状態異常をクリア
                    BattleParam.m_EnemyParam[num].m_StatusAilmentChara.DelStatusAilmentCondition(activity.Get_SUPPORT_SPESTATE_CLEAR_GOOD_BAD());
                }
                break;

            case MasterDataDefineLabel.TargetType.ALL:      // 全員
                                                            // プレイヤーの指定都合状態異常をクリア
                BattleParam.m_PlayerParty.m_Ailments.DelStatusAilmentCondition(activity.Get_SUPPORT_SPESTATE_CLEAR_GOOD_BAD());

                if (BattleParam.m_EnemyParam == null)
                {
                    break;
                }

                // 敵全サーチ
                for (int num = 0; num < BattleParam.m_EnemyParam.Length; ++num)
                {
                    if (BattleParam.m_EnemyParam[num] == null)
                    {
                        continue;
                    }

                    // 敵の指定都合状態異常をクリア
                    BattleParam.m_EnemyParam[num].m_StatusAilmentChara.DelStatusAilmentCondition(activity.Get_SUPPORT_SPESTATE_CLEAR_GOOD_BAD());
                }
                break;
        }
    }

    //------------------------------------------------------------------------
    /*!
        @brief		状態異常全クリア&HP回復スキル
        @param[in]	BattleSkillActivity		(activity)		スキル発動情報
    */
    //------------------------------------------------------------------------
    static public void ClearAilmentStatusSkillProc(BattleSkillActivity activity)
    {
        if (activity == null)
        {
            return;
        }
        if (BattleParam.m_PlayerParty == null)
        {
            return;
        }

        //----------------------------
        // HP回復
        //----------------------------
        if (activity.m_Type == MasterDataDefineLabel.SkillType.HEAL)
        {	// 回復タイプなら
            BattleSkillTarget[] target = activity.m_SkillParamTarget;
            if (target == null)
            {
                return;
            }
            if (target[0] == null)
            {
                return;
            }
            // HP回復
            BattleParam.m_PlayerParty.RecoveryHP(target[0].m_SkillValueToPlayer, true, true);
        }
        // 状態異常全クリア
        BattleParam.m_PlayerParty.m_Ailments.DelAllStatusAilment(GlobalDefine.PartyCharaIndex.MAX);
    }

    //------------------------------------------------------------------------
    /*!
        @brief		HP回復スキル
        @param[in]	BattleSkillActivity		(activity)		スキル発動情報
    */
    //------------------------------------------------------------------------
    static public void HealHPSkillProc(BattleSkillActivity activity)
    {
        if (activity == null)
        {
            return;
        }

        if (BattleParam.m_PlayerParty == null)
        {
            return;
        }

        //--------------------------------
        // HP回復
        //--------------------------------
        if (activity.m_Type == MasterDataDefineLabel.SkillType.HEAL)
        {
            BattleSkillTarget[] target = activity.m_SkillParamTarget;
            if (target == null)
            {
                return;
            }

            if (target[0] == null)
            {
                return;
            }

            BattleParam.m_PlayerParty.RecoveryHP(target[0].m_SkillValueToPlayer, true, true);
        }
    }

    //------------------------------------------------------------------------
    /*!
        @brief		SP回復スキル
        @param[in]	BattleSkillActivity		(activity)		スキル発動情報
    */
    //------------------------------------------------------------------------
    static public void HealSPSkillProc(BattleSkillActivity activity)
    {
        if (activity == null)
        {
            return;
        }

        if (BattleParam.m_PlayerParty == null)
        {
            return;
        }

        //--------------------------------
        // スキルタイプによる処理の分岐
        //--------------------------------
        switch (activity.m_SkillType)
        {
            default:
            case ESKILLTYPE.eLIMITBREAK:
                //--------------------------------
                // SP回復
                //--------------------------------
                if (activity.m_Category_SkillCategory_PROPERTY == MasterDataDefineLabel.SkillCategory.RECOVERY_SP)
                {
                    int sp = activity.Get_RECOVERY_SP_VALUE();
                    BattleParam.m_PlayerParty.RecoverySP(sp, false, false);
                }
                break;

            case ESKILLTYPE.eBOOST:
                //--------------------------------
                // SP回復
                //--------------------------------
                if (activity.m_Category_BoostSkillCategory_PROPERTY == MasterDataDefineLabel.BoostSkillCategory.HEAL_SP)
                {
                    int sp = activity.Get_BOOSTSKILL_HEAL_SP_VALUE();
                    BattleParam.m_PlayerParty.RecoverySP(sp, false, false);
                }
                break;
        }
    }


    //------------------------------------------------------------------------
    /*!
        @brief		LBS必要ターン数の短縮
        @param[in]	BattleSkillActivity		(activity)		スキル発動情報
        @note		※一旦いままでのつくりで作る
    */
    //------------------------------------------------------------------------
    static public void LBSCostSkillProc(BattleSkillActivity activity)
    {
        if (activity == null)
        {
            return;
        }

        // スキルパラメータ取得
        GlobalDefine.PartyCharaIndex userID = activity.m_SkillParamOwnerNum;
        int turn = activity.Get_SUPPORT_LBS_TURN_FAST_TURN();

        CharaOnce chara;
        for (int i = 0; i < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); i++)
        {
            // 使用者自身は効果適用外
            if (userID == (GlobalDefine.PartyCharaIndex)i)
            {
                continue;
            }

            // エラーチェック
            chara = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)i, CharaParty.CharaCondition.SKILL_LIMITBREAK);
            if (chara == null)
            {
                continue;
            }

            // LBS必要ターン数短縮
            chara.AddCharaLimitBreak(turn);

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "  LBSターン数短縮(" + chara.m_PartyCharaIndex.ToString() + "):" + turn.ToString() + "(→" + chara.GetTrunToLimitBreak() + ")");
        }

    }

    //----------------------------------------------------------------------------
    /*!
        @brief		DBから来た値に補正値を掛けて返す
        @return		float		[補正後の値]
    */
    //----------------------------------------------------------------------------
    public static float GetDBRevisionValue(float v)
    {
        return AvoidErrorMultiple(v, DEF_INGAME_DB_RATE);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		誤差回避用掛け算
        @param[in]	float		(val)		元の値
        @param[in]	float		(pow)		倍率
    */
    //----------------------------------------------------------------------------
    public static float AvoidErrorMultiple(float v, float pow)
    {
        float ret = (v * pow);
        if (ret != 0.0f)
        {
            ret += DEF_INGAME_AVOID_ERROR;
        }

        return ret;
    }

    /// <summary>
    /// 整数に戻した時に切り捨てで値が変わってしまわないように補正しておく
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static float AvoidError(ref float val)
    {
        const float AVOID_ERROR_CHECK_SCALE = 10000.0f;
        int i1 = (int)(val * AVOID_ERROR_CHECK_SCALE);	// 切り捨て
        int ir = (int)(val * AVOID_ERROR_CHECK_SCALE + 0.5f);	// 四捨五入

        // 切り捨てと四捨五入で値が違うときは補正
        if (i1 != ir)
        {
            for (int idx = 1; idx <= 4; idx++)
            {
                float f2 = val + DEF_INGAME_AVOID_ERROR * idx;
                int i2 = (int)(f2 * AVOID_ERROR_CHECK_SCALE);

                if (i1 != i2)
                {
                    val = f2;
                    break;
                }
            }
        }

        return val;
    }

    public static string getDebugPercentString(float rate)
    {
        int percent = (int)((rate + DEF_INGAME_AVOID_ERROR) * 100);
        string ret_val = percent.ToString();
        return ret_val;
    }

    //------------------------------------------------------------------------
    //	@brief		クエスト構築情報からドロップ情報を検索
    //	@param[in]	PacketStructQuestBuild		(quest_build_param)		クエスト情報
    //	@param[in]	int							(unique_id)				ドロップユニークID
    //------------------------------------------------------------------------
    public static PacketStructQuest2BuildDrop GetQuestBuildDrop(PacketStructQuest2Build quest_build_param, int unique_id)
    {

        //----------------------------------------
        //	エラーチェック
        //----------------------------------------
        if (quest_build_param == null
        || quest_build_param.list_drop == null)
        {
            return null;
        }

        if (unique_id == 0)
        {
            return null;
        }


        PacketStructQuest2BuildDrop drop_param = null;
        PacketStructQuest2BuildDrop drop_param_temp = null;
        for (int i = 0; i < quest_build_param.list_drop.Length; i++)
        {

            //----------------------------------------
            //	エラーチェック
            //----------------------------------------
            drop_param_temp = quest_build_param.list_drop[i];
            if (drop_param_temp == null)
            {
                continue;
            }


            //----------------------------------------
            //	ユニークID検索
            //----------------------------------------
            if (drop_param_temp.unique_id != unique_id)
            {
                continue;
            }


            drop_param = drop_param_temp;
            break;
        }


        return drop_param;

    }

    //----------------------------------------------------------------------------
    /*!
        @brief		スキル：場にパネルを配置	<static>
        @param[in]	uint				(unSkillID)			スキルID
        @param[in]	ESKILLTYPE			(eSkillType)		スキルタイプ
        @param[in]	bool				(bBtlStart)			戦闘開始時かどうか
        @return		bool				(bResult)			成否
        @note
    */
    //----------------------------------------------------------------------------
    static public bool SkillBattlefieldPanel(uint unSkillID, ESKILLTYPE eSkillType, bool bBtlStart)
    {
        bool bResult = false;
        //------------------------------
        // エラーチェック
        //------------------------------
        if (unSkillID == 0)
        {
            return (bResult);
        }

        // スキル効果を取得
        switch (eSkillType)
        {
            case ESKILLTYPE.eLEADER:
                {
                    MasterDataSkillLeader cLeaderMaster = BattleParam.m_MasterDataCache.useSkillLeader(unSkillID);
                    while (cLeaderMaster != null)
                    {
                        if (bBtlStart
                            || cLeaderMaster.Get_BFPNL_BTLSTART() == false)
                        {
                            // 場にパネルを配置
                            MasterDataDefineLabel.LeaderSkillCategory nCategory = cLeaderMaster.skill_type;
                            bResult = SkillBattlefieldPanel(nCategory, cLeaderMaster.GetBattleFieldPanelChangeParam(), eSkillType);
                        }

                        // 追加リーダースキルを取得
                        cLeaderMaster = BattleParam.m_MasterDataCache.useSkillLeader(cLeaderMaster.add_fix_id);
                    }

                    break;
                }

            case ESKILLTYPE.ePASSIVE:
            case ESKILLTYPE.eLINKPASSIVE:
                {
                    MasterDataSkillPassive cPassiveMaster = BattleParam.m_MasterDataCache.useSkillPassive(unSkillID);
                    if (cPassiveMaster == null)
                    {
                        return (bResult);
                    }

                    MasterDataDefineLabel.PassiveSkillCategory nCategory = cPassiveMaster.skill_type;
                    BattleFieldPanelChangeParam panel_change_param = cPassiveMaster.GetBattleFieldPanelChangeParam();

                    // 戦闘開始時のみの場合
                    if (panel_change_param.m_IsBattleStartOnly
                    && bBtlStart == false)
                    {
                        return (bResult);
                    }

                    // 場にパネルを配置
                    bResult = SkillBattlefieldPanel(nCategory, panel_change_param, eSkillType);
                    break;
                }

            default:
                break;
        }

        return (bResult);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		スキル：場にパネルを配置	<static>
        @param[in]	BattleSkillActivity	(cSkillActivity)	発動情報出力変数
        @return		bool				(bResult)			成否
        @note
    */
    //----------------------------------------------------------------------------
    static public bool SkillBattlefieldPanel(BattleSkillActivity cSkillActivity)
    {
        bool bResult = false;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (cSkillActivity == null)
        {
            return (bResult);
        }

        BattleFieldPanelChangeParam panel_change_param = cSkillActivity.GetBattleFieldPanelChangeParam();
        bResult = SkillBattlefieldPanel(cSkillActivity.m_Category_SkillCategory_PROPERTY,
                                         panel_change_param,
                                         cSkillActivity.m_SkillType);

        return (bResult);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		スキル：場にパネルを配置	<static>
        @param[in]	int			(nCategory)		カテゴリ
        @param[in]	int[]		(anParamList)	パラメータリスト
        @param[in]	ESKILLTYPE	(eSkillType)	スキルタイプ
        @return		bool		(bResult)		成否
        @note
    */
    //----------------------------------------------------------------------------
    static public bool SkillBattlefieldPanel(MasterDataDefineLabel.SkillCategory nCategory, BattleFieldPanelChangeParam panel_change_param, ESKILLTYPE eSkillType)
    {
        //if (eSkillType != ESKILLTYPE.eBOOST)
        {
            return _SkillBattlefieldPanel((int)nCategory, panel_change_param, eSkillType);
        }
    }
    static public bool SkillBattlefieldPanel(MasterDataDefineLabel.BoostSkillCategory nCategory, BattleFieldPanelChangeParam panel_change_param, ESKILLTYPE eSkillType)
    {
        if (eSkillType == ESKILLTYPE.eBOOST)
        {
            return _SkillBattlefieldPanel((int)nCategory, panel_change_param, eSkillType);
        }
        return false;
    }
    static public bool SkillBattlefieldPanel(MasterDataDefineLabel.EnemySkillCategory nCategory, BattleFieldPanelChangeParam panel_change_param, ESKILLTYPE eSkillType)
    {
        if (eSkillType == ESKILLTYPE.eENEMY)
        {
            return _SkillBattlefieldPanel((int)nCategory, panel_change_param, eSkillType);
        }
        return false;
    }
    static public bool SkillBattlefieldPanel(MasterDataDefineLabel.LeaderSkillCategory nCategory, BattleFieldPanelChangeParam panel_change_param, ESKILLTYPE eSkillType)
    {
        if (eSkillType == ESKILLTYPE.eLEADER)
        {
            return _SkillBattlefieldPanel((int)nCategory, panel_change_param, eSkillType);
        }
        return false;
    }
    static public bool SkillBattlefieldPanel(MasterDataDefineLabel.PassiveSkillCategory nCategory, BattleFieldPanelChangeParam panel_change_param, ESKILLTYPE eSkillType)
    {
        if (eSkillType == ESKILLTYPE.ePASSIVE
        || eSkillType == ESKILLTYPE.eLINKPASSIVE
        )
        {
            return _SkillBattlefieldPanel((int)nCategory, panel_change_param, eSkillType);
        }
        return false;
    }
    static private bool _SkillBattlefieldPanel(int nCategory, BattleFieldPanelChangeParam panel_change_param, ESKILLTYPE eSkillType)
    {
        bool bResult = false;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (panel_change_param == null)
        {
            return (bResult);
        }


        //------------------------------
        // スキルタイプによる分岐：要素の取得
        //------------------------------
        int[] anPanelNum = new int[BattleLogic.BATTLE_FIELD_COST_MAX];	// パネル数
        for (int idx = 0; idx < anPanelNum.Length; idx++)
        {
            anPanelNum[idx] = panel_change_param.getPanelNum(idx);
        }
        switch (eSkillType)
        {
            #region ==== リーダースキル処理 ====
            case ESKILLTYPE.eLEADER:
                {
                    // 該当スキルかチェック
                    if ((MasterDataDefineLabel.LeaderSkillCategory)nCategory != MasterDataDefineLabel.LeaderSkillCategory.BATTLEFIELD_PANEL)
                    {
                        return (bResult);
                    }

                    // 発動確率判定
                    if (BattleSceneUtil.checkChancePercentSkill(panel_change_param.m_Odds) == false)
                    {
                        return (bResult);
                    }

                    bResult = true;
                    break;
                }
            #endregion
            #region ==== パッシブ/リンクパッシブスキル処理 ====
            case ESKILLTYPE.ePASSIVE:
            case ESKILLTYPE.eLINKPASSIVE:
                {
                    // 該当スキルかチェック
                    if ((MasterDataDefineLabel.PassiveSkillCategory)nCategory != MasterDataDefineLabel.PassiveSkillCategory.BATTLEFIELD_PANEL)
                    {
                        return (bResult);
                    }

                    // 発動確率判定
                    if (BattleSceneUtil.checkChancePercentSkill(panel_change_param.m_Odds) == false)
                    {
                        return (bResult);
                    }

                    bResult = true;
                    break;
                }
            #endregion
            #region ==== リミットブレイク処理 ====
            case ESKILLTYPE.eLIMITBREAK:
                // 該当スキルかチェック
                if ((MasterDataDefineLabel.SkillCategory)nCategory != MasterDataDefineLabel.SkillCategory.SUPPORT_BATTLEFIELD_PANEL)
                {
                    return (bResult);
                }

                bResult = true;
                break;
            #endregion
            #region ==== ブーストスキル処理 ====
            case ESKILLTYPE.eBOOST:
                // 該当スキルかチェック
                if ((MasterDataDefineLabel.BoostSkillCategory)nCategory != MasterDataDefineLabel.BoostSkillCategory.BATTLEFIELD_PANEL)
                {
                    return (bResult);
                }

                bResult = true;
                break;
            #endregion
            #region ==== エネミースキル処理 ====
            case ESKILLTYPE.eENEMY:
                // 該当スキルかチェック
                if ((MasterDataDefineLabel.EnemySkillCategory)nCategory != MasterDataDefineLabel.EnemySkillCategory.BATTLEFIELD_PANEL)
                {
                    return (bResult);
                }

                bResult = true;
                break;
            #endregion
            default:
                break;
        }

        //------------------------------
        // ランダム内容の取得：ビットフラグ
        //------------------------------
        MasterDataDefineLabel.ElementType[] anRandPanel = null;
        int nRandPanelMax = (int)MasterDataDefineLabel.PanelType.MAX - 3;			// ランダムパネル数：※NONE定義分、ランダム定義2種分「-3」する
        uint unRandCnt = 0;

        // ランダム内容が設定されている場合
        if (panel_change_param.m_RandContent != 0)
        {
            // データ順：炎水風光闇無回邪 →下位bit
            // ※NONE定義分、ランダム定義分、ランダム(1枚毎)定義分「-3」する
            bool[] abChkPanel = new bool[nRandPanelMax];
            int nBitFlag = 0;
            for (int bit = 0; bit < nRandPanelMax; ++bit)
            {
                nBitFlag = 1 << bit;

                // フラグチェック
                if ((panel_change_param.m_RandContent & nBitFlag) == 0)
                {
                    continue;
                }

                abChkPanel[bit] = true;
                ++unRandCnt;
            }

            //------------------------------
            // ランダム内容を配列化
            //------------------------------
            int nPanelIdx = 0;

            anRandPanel = new MasterDataDefineLabel.ElementType[unRandCnt];
            for (int chkNum = 0; chkNum < nRandPanelMax; ++chkNum)
            {
                // フラグチェック
                if (abChkPanel[chkNum] == false)
                {
                    continue;
                }

                #region ==== 配列化処理 ====
                // ※MAX定義分「-1」する
                switch (chkNum)
                {
                    // 炎
                    case MasterDataDefineLabel.PanelType.MAX - MasterDataDefineLabel.PanelType.FIRE - 1:
                        anRandPanel[nPanelIdx] = MasterDataDefineLabel.ElementType.FIRE;
                        break;

                    // 水
                    case MasterDataDefineLabel.PanelType.MAX - MasterDataDefineLabel.PanelType.WATER - 1:
                        anRandPanel[nPanelIdx] = MasterDataDefineLabel.ElementType.WATER;
                        break;

                    // 風
                    case MasterDataDefineLabel.PanelType.MAX - MasterDataDefineLabel.PanelType.WIND - 1:
                        anRandPanel[nPanelIdx] = MasterDataDefineLabel.ElementType.WIND;
                        break;

                    // 光
                    case MasterDataDefineLabel.PanelType.MAX - MasterDataDefineLabel.PanelType.LIGHT - 1:
                        anRandPanel[nPanelIdx] = MasterDataDefineLabel.ElementType.LIGHT;
                        break;

                    // 闇
                    case MasterDataDefineLabel.PanelType.MAX - MasterDataDefineLabel.PanelType.DARK - 1:
                        anRandPanel[nPanelIdx] = MasterDataDefineLabel.ElementType.DARK;
                        break;

                    // 無
                    case MasterDataDefineLabel.PanelType.MAX - MasterDataDefineLabel.PanelType.NAUGHT - 1:
                        anRandPanel[nPanelIdx] = MasterDataDefineLabel.ElementType.NAUGHT;
                        break;

                    // 回復
                    case MasterDataDefineLabel.PanelType.MAX - MasterDataDefineLabel.PanelType.HEAL - 1:
                        anRandPanel[nPanelIdx] = MasterDataDefineLabel.ElementType.HEAL;
                        break;

                    // 邪魔(未実装)
                    case MasterDataDefineLabel.PanelType.MAX - MasterDataDefineLabel.PanelType.HIND - 1:
                        break;

                    default:
                        break;
                }
                #endregion

                ++nPanelIdx;
            }
        }

        //------------------------------
        // 事前クリア処理
        // @add Developer 2016/06/06 v350
        //------------------------------
        //BattleCardObject[]	   acTempPanel	  = null;
        if (panel_change_param.m_IsFieldClear)
        {
            //------------------------------
            // 全クリア
            //------------------------------
            for (int fieldNum = 0; fieldNum < BattleLogic.BATTLE_FIELD_MAX; ++fieldNum)
            {
                // アクセス用に保持
                BattleScene.FieldArea acTempPanel = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(fieldNum);
                if (acTempPanel == null || acTempPanel.getCardCount() <= 0)
                {
                    continue;
                }

                // 場のパネルをクリア
                acTempPanel.reset();

                // パネル変化エフェクト
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.FIELD_AREA, fieldNum, BattleScene._BattleCardManager.EffectInfo.EffectType.CARD_CHANGE);
            }
        }

        //------------------------------
        // 出現箇所による分岐
        // ※nFieldTypeの値がランダム数と等しい
        //------------------------------
        bool[] abChkField = new bool[BattleLogic.BATTLE_FIELD_MAX];
        uint unRandField = 0;
        switch (panel_change_param.m_FieldType)
        {
            //------------------------------
            // ランダム
            //------------------------------
            #region ==== ランダム処理 ====
            case MasterDataDefineLabel.FieldType.RANDOM_1:
            case MasterDataDefineLabel.FieldType.RANDOM_2:
            case MasterDataDefineLabel.FieldType.RANDOM_3:
            case MasterDataDefineLabel.FieldType.RANDOM_4:
                {
                    // 検索上限値取得
                    uint[] anRandField = new uint[BattleLogic.BATTLE_FIELD_MAX];
                    uint unSearchMax = (uint)panel_change_param.m_FieldType;
                    uint unSearchFieldNum = 0;
                    for (uint num = 0; num < BattleLogic.BATTLE_FIELD_MAX; ++num)
                    {
                        // パネル設定されていない場合、ランダム対象外
                        if (panel_change_param.getPanelType((int)num) == MasterDataDefineLabel.PanelType.NONE)
                        {
                            continue;
                        }

                        anRandField[unSearchFieldNum] = num;
                        ++unSearchFieldNum;
                    }

                    // 上限チェック
                    if (unSearchMax > unSearchFieldNum)
                    {
                        unSearchMax = unSearchFieldNum;
                    }

                    // 出現場所許可処理
                    uint unPermitField = 0;
                    for (uint num = 0; num < unSearchMax; ++num)
                    {
                        // 乱数取得
                        unRandField = RandManager.GetRand(num, unSearchFieldNum);

                        // 入れ替え
                        unPermitField = anRandField[unRandField];
                        anRandField[unRandField] = anRandField[num];
                        anRandField[num] = unPermitField;

                        // 出現許可
                        abChkField[unPermitField] = true;
                    }
                    break;
                }
            #endregion

            //------------------------------
            // 指定通り
            //------------------------------
            case MasterDataDefineLabel.FieldType.ASSIGN:
                for (int num = 0; num < BattleLogic.BATTLE_FIELD_MAX; ++num)
                {
                    abChkField[num] = true;
                }
                break;
        }

        //------------------------------
        // スキル反映処理
        //------------------------------
        bool[] abEffect = new bool[BattleLogic.BATTLE_FIELD_MAX];
        MasterDataDefineLabel.ElementType nPanelElem = MasterDataDefineLabel.ElementType.NONE;
        uint unRandPanel = 0;
        bool bRandPanelEach = false;
        for (int fieldNum = 0; fieldNum < BattleLogic.BATTLE_FIELD_MAX; ++fieldNum)
        {
            // 出現場所許可チェック
            if (abChkField[fieldNum] == false)
            {
                continue;
            }

            // アクセス用に保持
            BattleScene.FieldArea acTempPanel = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(fieldNum);
            if (acTempPanel == null)
            {
                continue;
            }

            // 場が選択された場合：エフェクトフラグON
            abEffect[fieldNum] = true;

            // ランダム(1枚毎)処理：フラグOFF
            // @add Developer 2016/06/06 v350
            bRandPanelEach = false;

            //------------------------------
            // スキル効果のパネルに変換
            //------------------------------
            switch (panel_change_param.getPanelType(fieldNum))
            {
                case MasterDataDefineLabel.PanelType.NONE:
                    continue;

                // ランダム処理：属性の決定
                case MasterDataDefineLabel.PanelType.RANDOM:
                    if (anRandPanel != null)
                    {
                        unRandPanel = RandManager.GetRand((uint)0, unRandCnt);
                        nPanelElem = anRandPanel[unRandPanel];
                    }
                    else
                    {
                        nPanelElem = MasterDataDefineLabel.ElementType.NONE;
                    }
                    break;

                // ランダム(1枚毎)処理：フラグON
                // @add Developer 2016/06/06 v350
                case MasterDataDefineLabel.PanelType.RANDOM_EACH:
                    bRandPanelEach = true;
                    break;

                case MasterDataDefineLabel.PanelType.FIRE: nPanelElem = MasterDataDefineLabel.ElementType.FIRE; break;
                case MasterDataDefineLabel.PanelType.WATER: nPanelElem = MasterDataDefineLabel.ElementType.WATER; break;
                case MasterDataDefineLabel.PanelType.WIND: nPanelElem = MasterDataDefineLabel.ElementType.WIND; break;
                case MasterDataDefineLabel.PanelType.LIGHT: nPanelElem = MasterDataDefineLabel.ElementType.LIGHT; break;
                case MasterDataDefineLabel.PanelType.DARK: nPanelElem = MasterDataDefineLabel.ElementType.DARK; break;
                case MasterDataDefineLabel.PanelType.NAUGHT: nPanelElem = MasterDataDefineLabel.ElementType.NAUGHT; break;
                case MasterDataDefineLabel.PanelType.HEAL: nPanelElem = MasterDataDefineLabel.ElementType.HEAL; break;
                // 邪魔(未実装)
                case MasterDataDefineLabel.PanelType.HIND: nPanelElem = MasterDataDefineLabel.ElementType.NONE; break;
                default:
                    break;
            }

            //------------------------------
            // 置換の場合
            //------------------------------
            #region ==== パネルクリア ====
            if (panel_change_param.m_MethodType == MasterDataDefineLabel.MethodType.REPLACE)
            {
                // 該当する場のパネルをクリア
                acTempPanel.reset();
            }
            #endregion

            //------------------------------
            // 場にパネルを配置
            //------------------------------
            #region ==== パネル配置 ====
            for (int num = 0; num < BattleLogic.BATTLE_FIELD_COST_MAX; ++num)
            {
                // 指定枚数チェック
                if (anPanelNum[fieldNum] <= 0)
                {
                    break;
                }

                // 空きチェック
                if (acTempPanel.isFull())
                {
                    continue;
                }

                BattleScene.BattleCard FreePanel = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.getUnusedCard();
                if (FreePanel == null)
                {
#if UNITY_EDITOR
                    Debug.LogError("FreeCard Is None!");
#endif // #if UNITY_EDITOR
                    continue;
                }

                //------------------------------
                // ランダム(1枚毎)処理：属性の決定
                // @add Developer 2016/06/06 v350
                //------------------------------
                if (bRandPanelEach == true)
                {
                    if (anRandPanel != null)
                    {
                        unRandPanel = RandManager.GetRand((uint)0, unRandCnt);
                        nPanelElem = anRandPanel[unRandPanel];
                    }
                    else
                    {
                        nPanelElem = MasterDataDefineLabel.ElementType.NONE;
                    }
                }

                // パネル設定変更＆デザイン変更
                FreePanel.setElementType(nPanelElem, BattleScene.BattleCard.ChangeCause.NONE);

                //acTempPanel[	num ] = FreePanel;
                acTempPanel.addCard(FreePanel);

                // 指定枚数カウントダウン
                --anPanelNum[fieldNum];
            }

            // パネル変化エフェクト
            // @change Developer 2016/06/09 v350：全クリアが行われている場合、エフェクトは一度出したので出さない
            if (panel_change_param.m_IsFieldClear == false
            && abEffect[fieldNum] == true)
            {
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.FIELD_AREA, fieldNum, BattleScene._BattleCardManager.EffectInfo.EffectType.CARD_CHANGE);
            }
            #endregion
        }

        //------------------------------
        // 場の最終的な空き状態をチェック
        // ※上記処理内で確認したいが、煩雑化するため分離
        //------------------------------
        bool bFieldMax = true;
        for (int fieldNum = 0; fieldNum < BattleLogic.BATTLE_FIELD_MAX; ++fieldNum)
        {
            // 場が満杯の場合
            if (BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(fieldNum).isFull())
            {
                continue;
            }

            bFieldMax = false;
            break;
        }

        //------------------------------
        // 全ての場が最大値の場合
        //------------------------------
        #region ==== 1枚ランダム削除 ====
        if (bFieldMax == true)
        {
            unRandField = RandManager.GetRand((uint)0, BattleLogic.BATTLE_FIELD_MAX);
            unRandPanel = RandManager.GetRand((uint)0, BattleLogic.BATTLE_FIELD_COST_MAX);

            BattleScene.BattleCard battle_card = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea((int)unRandField).getCard((int)unRandPanel);
            BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.removeCard(battle_card, true);
        }
        #endregion

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "パネル変換スキル発動  " + eSkillType.ToString());
        DebugBattleLog.outputCard(BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager);

        return (bResult);
    }

    //---------------------------------------------------------------------
    /*!
        @brief		スキルエフェクト設定
        @param[in]	ref int		(nEffect)	エフェクトタイプ
        @param[in]	int			(nType)		スキルタイプ
        @param[in]	int			(nValue)	スキル威力
        @note		効果値に対応したエフェクトに切り替える
    */
    //---------------------------------------------------------------------
    static public MasterDataDefineLabel.UIEffectType SetSkillEffectToValue(MasterDataDefineLabel.UIEffectType nEffect, MasterDataDefineLabel.SkillType nType, int nValue)
    {
        //----------------------------------------
        // タイプ別エフェクトの種類判定
        //----------------------------------------
        #region ==== エフェクト種類判定 ====
        int nEffectType = -1;
        switch (nType)
        {
            //-------------------------------
            // 全体
            //-------------------------------
            case MasterDataDefineLabel.SkillType.ATK_ALL:
                if (nValue >= VALUE_ATK_ALL_HIGH)
                {
                    nEffectType = SKILL_EFFECT_HIGH;
                }
                else
                {
                    nEffectType = SKILL_EFFECT_LOW;
                }
                break;

            //-------------------------------
            // 単体
            //-------------------------------
            case MasterDataDefineLabel.SkillType.ATK_ONCE:
                if (nValue >= VALUE_ATK_ONCE_HIGH)
                {
                    nEffectType = SKILL_EFFECT_HIGH;
                }
                else if (nValue >= VALUE_ATK_ONCE_MID)
                {
                    nEffectType = SKILL_EFFECT_MID;
                }
                else
                {
                    nEffectType = SKILL_EFFECT_LOW;
                }
                break;

            //-------------------------------
            // 回復
            //-------------------------------
            case MasterDataDefineLabel.SkillType.HEAL:
                if (nValue >= VALUE_HEAL_MAX)
                {
                    nEffectType = SKILL_EFFECT_MAX;
                }
                else if (nValue >= VALUE_HEAL_HIGH)
                {
                    nEffectType = SKILL_EFFECT_HIGH;
                }
                else if (nValue >= VALUE_HEAL_MID)
                {
                    nEffectType = SKILL_EFFECT_MID;
                }
                else
                {
                    nEffectType = SKILL_EFFECT_LOW;
                }
                break;
        }
        #endregion

        //----------------------------------------
        // エフェクト切り替え
        //----------------------------------------
        #region ==== エフェクト設定 ====
        switch (nEffect)
        {
            //-------------------------------
            // 無
            //-------------------------------
            case MasterDataDefineLabel.UIEffectType.NAUGHT_MA_00:	// 全体：物理：弱
            case MasterDataDefineLabel.UIEffectType.NAUGHT_MA_01:	// 全体：物理：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.NAUGHT_MA_00; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.NAUGHT_MA_01; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.NAUGHT_MM_00:	// 全体：魔法：弱
            case MasterDataDefineLabel.UIEffectType.NAUGHT_MM_01:	// 全体：魔法：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.NAUGHT_MM_00; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.NAUGHT_MM_01; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.NAUGHT_SA_00:	// 単体：物理：弱
            case MasterDataDefineLabel.UIEffectType.NAUGHT_SA_01:	// 単体：物理：中
            case MasterDataDefineLabel.UIEffectType.NAUGHT_SA_02:	// 単体：物理：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.NAUGHT_SA_00; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.NAUGHT_SA_01; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.NAUGHT_SA_02; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.NAUGHT_SM_00:	// 単体：魔法：弱
            case MasterDataDefineLabel.UIEffectType.NAUGHT_SM_01:	// 単体：魔法：中
            case MasterDataDefineLabel.UIEffectType.NAUGHT_SM_02:	// 単体：魔法：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.NAUGHT_SM_00; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.NAUGHT_SM_01; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.NAUGHT_SM_02; break;
                }
                break;

            //-------------------------------
            // 炎
            //-------------------------------
            case MasterDataDefineLabel.UIEffectType.FIRE_MA_00:		// 全体：物理：弱
            case MasterDataDefineLabel.UIEffectType.FIRE_MA_01:		// 全体：物理：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.FIRE_MA_00; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.FIRE_MA_01; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.FIRE_MM_00:		// 全体：魔法：弱
            case MasterDataDefineLabel.UIEffectType.FIRE_MM_01:		// 全体：魔法：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.FIRE_MM_00; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.FIRE_MM_01; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.FIRE_SA_00:		// 単体：物理：弱
            case MasterDataDefineLabel.UIEffectType.FIRE_SA_01:		// 単体：物理：中
            case MasterDataDefineLabel.UIEffectType.FIRE_SA_02:		// 単体：物理：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.FIRE_SA_00; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.FIRE_SA_01; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.FIRE_SA_02; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.FIRE_SM_00:		// 単体：魔法：弱
            case MasterDataDefineLabel.UIEffectType.FIRE_SM_01:		// 単体：魔法：中
            case MasterDataDefineLabel.UIEffectType.FIRE_SM_02:		// 単体：魔法：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.FIRE_SM_00; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.FIRE_SM_01; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.FIRE_SM_02; break;
                }
                break;

            //-------------------------------
            // 水
            //-------------------------------
            case MasterDataDefineLabel.UIEffectType.WATER_MA_00:	// 全体：物理：弱
            case MasterDataDefineLabel.UIEffectType.WATER_MA_01:	// 全体：物理：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.WATER_MA_00; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.WATER_MA_01; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.WATER_MM_00:	// 全体：魔法：弱
            case MasterDataDefineLabel.UIEffectType.WATER_MM_01:	// 全体：魔法：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.WATER_MM_00; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.WATER_MM_01; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.WATER_SA_00:	// 単体：物理：弱
            case MasterDataDefineLabel.UIEffectType.WATER_SA_01:	// 単体：物理：中
            case MasterDataDefineLabel.UIEffectType.WATER_SA_02:	// 単体：物理：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.WATER_SA_00; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.WATER_SA_01; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.WATER_SA_02; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.WATER_SM_00:	// 単体：魔法：弱
            case MasterDataDefineLabel.UIEffectType.WATER_SM_01:	// 単体：魔法：中
            case MasterDataDefineLabel.UIEffectType.WATER_SM_02:	// 単体：魔法：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.WATER_SM_00; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.WATER_SM_01; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.WATER_SM_02; break;
                }
                break;

            //-------------------------------
            // 風
            //-------------------------------
            case MasterDataDefineLabel.UIEffectType.WIND_MA_00:		// 全体：物理：弱
            case MasterDataDefineLabel.UIEffectType.WIND_MA_01:		// 全体：物理：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.WIND_MA_00; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.WIND_MA_01; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.WIND_MM_00:		// 全体：魔法：弱
            case MasterDataDefineLabel.UIEffectType.WIND_MM_01:		// 全体：魔法：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.WIND_MM_00; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.WIND_MM_01; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.WIND_SA_00:		// 単体：物理：弱
            case MasterDataDefineLabel.UIEffectType.WIND_SA_01:		// 単体：物理：中
            case MasterDataDefineLabel.UIEffectType.WIND_SA_02:		// 単体：物理：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.WIND_SA_00; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.WIND_SA_01; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.WIND_SA_02; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.WIND_SM_00:		// 単体：魔法：弱
            case MasterDataDefineLabel.UIEffectType.WIND_SM_01:		// 単体：魔法：中
            case MasterDataDefineLabel.UIEffectType.WIND_SM_02:		// 単体：魔法：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.WIND_SM_00; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.WIND_SM_01; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.WIND_SM_02; break;
                }
                break;

            //-------------------------------
            // 光
            //-------------------------------
            case MasterDataDefineLabel.UIEffectType.LIGHT_MA_00:	// 全体：物理：弱
            case MasterDataDefineLabel.UIEffectType.LIGHT_MA_01:	// 全体：物理：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.LIGHT_MA_00; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.LIGHT_MA_01; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.LIGHT_MM_00:	// 全体：魔法：弱
            case MasterDataDefineLabel.UIEffectType.LIGHT_MM_01:	// 全体：魔法：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.LIGHT_MM_00; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.LIGHT_MM_01; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.LIGHT_SA_00:	// 単体：物理：弱
            case MasterDataDefineLabel.UIEffectType.LIGHT_SA_01:	// 単体：物理：中
            case MasterDataDefineLabel.UIEffectType.LIGHT_SA_02:	// 単体：物理：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.LIGHT_SA_00; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.LIGHT_SA_01; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.LIGHT_SA_02; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.LIGHT_SM_00:	// 単体：魔法：弱
            case MasterDataDefineLabel.UIEffectType.LIGHT_SM_01:	// 単体：魔法：中
            case MasterDataDefineLabel.UIEffectType.LIGHT_SM_02:	// 単体：魔法：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.LIGHT_SM_00; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.LIGHT_SM_01; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.LIGHT_SM_02; break;
                }
                break;

            //-------------------------------
            // 闇
            //-------------------------------
            case MasterDataDefineLabel.UIEffectType.DARK_MA_00:		// 全体：物理：弱
            case MasterDataDefineLabel.UIEffectType.DARK_MA_01:		// 全体：物理：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.DARK_MA_00; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.DARK_MA_01; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.DARK_MM_00:		// 全体：魔法：弱
            case MasterDataDefineLabel.UIEffectType.DARK_MM_01:		// 全体：魔法：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.DARK_MM_00; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.DARK_MM_01; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.DARK_SA_00:		// 単体：物理：弱
            case MasterDataDefineLabel.UIEffectType.DARK_SA_01:		// 単体：物理：中
            case MasterDataDefineLabel.UIEffectType.DARK_SA_02:		// 単体：物理：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.DARK_SA_00; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.DARK_SA_01; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.DARK_SA_02; break;
                }
                break;
            case MasterDataDefineLabel.UIEffectType.DARK_SM_00:		// 単体：魔法：弱
            case MasterDataDefineLabel.UIEffectType.DARK_SM_01:		// 単体：魔法：中
            case MasterDataDefineLabel.UIEffectType.DARK_SM_02:		// 単体：魔法：強
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.DARK_SM_00; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.DARK_SM_01; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.DARK_SM_02; break;
                }
                break;

            //-------------------------------
            // 回復
            //-------------------------------
            case MasterDataDefineLabel.UIEffectType.HEAL_1:			// 回復：小
            case MasterDataDefineLabel.UIEffectType.HEAL_2:			// 回復：中
            case MasterDataDefineLabel.UIEffectType.HEAL_3:			// 回復：大
            case MasterDataDefineLabel.UIEffectType.HEAL_4:			// 回復：完全回復
                switch (nEffectType)
                {
                    case SKILL_EFFECT_LOW: nEffect = MasterDataDefineLabel.UIEffectType.HEAL_1; break;
                    case SKILL_EFFECT_MID: nEffect = MasterDataDefineLabel.UIEffectType.HEAL_2; break;
                    case SKILL_EFFECT_HIGH: nEffect = MasterDataDefineLabel.UIEffectType.HEAL_3; break;
                    case SKILL_EFFECT_MAX: nEffect = MasterDataDefineLabel.UIEffectType.HEAL_4; break;
                }
                break;
        }
        #endregion

        return nEffect;
    }


    //---------------------------------------------------------------------
    /*!
        @brief		アクティブ(ノーマル)スキル：特性処理
        @param[in]	MasterDataSkillActive	(cSkillActiveParam)	アクティブスキルマスター
        @param[in]	int						(nSkillNum)			スキル発動数
        @note
    */
    //---------------------------------------------------------------------
    static public bool ChkSkillAbility(MasterDataSkillActive cSkillActiveParam, int nSkillNum)
    {
        bool bResult = false;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (cSkillActiveParam == null)
        {
            return (bResult);
        }


        //--------------------------------
        // 特性による分岐
        //--------------------------------
        switch (cSkillActiveParam.ability)
        {
            case MasterDataDefineLabel.ActiveAbilityType.AILMENT: bResult = ChkSkillAbilityAilment(cSkillActiveParam); break;	// 条件：状態異常
            case MasterDataDefineLabel.ActiveAbilityType.HANDS: bResult = ChkSkillAbilityHands(cSkillActiveParam, nSkillNum); break;	// 条件：HANDS数
            case MasterDataDefineLabel.ActiveAbilityType.RATE: bResult = ChkSkillAbilityRate(cSkillActiveParam, nSkillNum); break;  // 条件：Rate数
                                                                                                                                    //			case MasterDataDefineLabel.ACTIVE_ABILITY_PARTY_HP:	bResult = ChkSkillAbilityPartyHP( ref anParamList			 );	break;	// 条件：パーティHP(未実装)
            default:
                break;
        }

        return (bResult);
    }

    //---------------------------------------------------------------------
    /*!
        @brief		アクティブ(ノーマル)スキル：特性タイプ：状態異常
        @param[in]	ref int[]	(anParamList)	特性汎用パラメータ配列
        @note
    */
    //---------------------------------------------------------------------
    static private bool ChkSkillAbilityAilment(MasterDataSkillActive master_data_skill_active)
    {
        bool bResult = false;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (BattleParam.isInitilaizedBattle() == false)
        {
            return (bResult);
        }

        StatusAilmentChara cAilmentPlayer = null;
        StatusAilmentChara[] cAilmentEnemy = null;


        //--------------------------------
        // 状態異常の判定対象による分岐
        //--------------------------------
        switch (master_data_skill_active.Get_AILMENT_TARGET())
        {
            //--------------------------------
            // プレイヤーの状態異常を見る場合
            //--------------------------------
            case MasterDataDefineLabel.TargetType.SELF:
            case MasterDataDefineLabel.TargetType.FRIEND:
            case MasterDataDefineLabel.TargetType.SELF_OTHER_FRIEND:
            case MasterDataDefineLabel.TargetType.SELF_OTHER_ALL:
                // 状態異常管理を取得
                //if( BattleParam.m_PlayerParty.m_StatusAilmentChara.GetOwnerType() == StatusAilmentChara.OwnerType.PLAYER )
                {
                    cAilmentPlayer = BattleParam.m_PlayerParty.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX);
                }
                break;

            //--------------------------------
            // エネミーの状態異常を見る場合
            //--------------------------------
            case MasterDataDefineLabel.TargetType.OTHER:
            case MasterDataDefineLabel.TargetType.ENEMY:
            case MasterDataDefineLabel.TargetType.ENE_N_1:
            case MasterDataDefineLabel.TargetType.ENE_1N_1:
            case MasterDataDefineLabel.TargetType.ENE_R_N:
            case MasterDataDefineLabel.TargetType.ENE_1_N:
                {
                    // 全エネミーの状態異常を取得
                    BattleEnemy cEnemyParam = null;

                    cAilmentEnemy = new StatusAilmentChara[BattleParam.m_EnemyParam.Length];
                    for (int num = 0; num < BattleParam.m_EnemyParam.Length; ++num)
                    {
                        cEnemyParam = BattleParam.m_EnemyParam[num];
                        if (cEnemyParam == null
                        || cEnemyParam.isShow() == false
                        || cEnemyParam.isDead() == true)
                        {
                            continue;
                        }

                        // 状態異常管理を取得
                        if (cEnemyParam.m_StatusAilmentChara.GetOwnerType() == StatusAilmentChara.OwnerType.ENEMY)
                        {
                            cAilmentEnemy[num] = cEnemyParam.m_StatusAilmentChara;
                        }
                    }
                    break;
                }

            //--------------------------------
            // 両者の状態異常を見る場合
            //--------------------------------
            case MasterDataDefineLabel.TargetType.ALL:
                {
                    // プレイヤーの状態異常管理を取得
                    //if( BattleParam.m_PlayerParty.m_StatusAilmentChara.GetOwnerType() == StatusAilmentChara.OwnerType.PLAYER )
                    {
                        cAilmentPlayer = BattleParam.m_PlayerParty.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX);
                    }

                    // 全エネミーの状態異常を取得
                    BattleEnemy cEnemyParam = null;

                    cAilmentEnemy = new StatusAilmentChara[BattleParam.m_EnemyParam.Length];
                    for (int num = 0; num < BattleParam.m_EnemyParam.Length; ++num)
                    {
                        cEnemyParam = BattleParam.m_EnemyParam[num];
                        if (cEnemyParam == null
                        || cEnemyParam.isShow() == false
                        || cEnemyParam.isDead() == true)
                        {
                            continue;
                        }

                        // 状態異常管理を取得
                        if (cEnemyParam.m_StatusAilmentChara.GetOwnerType() == StatusAilmentChara.OwnerType.ENEMY)
                        {
                            cAilmentEnemy[num] = cEnemyParam.m_StatusAilmentChara;
                        }
                    }
                    break;
                }

            //--------------------------------
            // 何もしない場合
            //--------------------------------
            case MasterDataDefineLabel.TargetType.NONE:
            default:
                return (bResult);
        }


        //--------------------------------
        // 指定した状態異常の検索(プレイヤー)
        //--------------------------------
        if (cAilmentPlayer != null)
        {
            MasterDataDefineLabel.AilmentGroup nGroupNum = MasterDataDefineLabel.AilmentGroup.NONE;
            for (int num = 0; num < (int)MasterDataDefineLabel.AilmentType.MAX; ++num)
            {
                if (cAilmentPlayer.IsHavingAilment((MasterDataDefineLabel.AilmentType)num) == false)
                {
                    continue;
                }

                // 状態異常グループを取得
                nGroupNum = StatusAilment.getAilmentGroupFromAilmentType((MasterDataDefineLabel.AilmentType)num);

                // 該当するグループの場合
                if (nGroupNum == master_data_skill_active.Get_AILMENT_GROUP_TYPE1())
                {
                    bResult = true;
                    break;
                }
            }
        }
        //--------------------------------
        // 指定した状態異常の検索(エネミー)
        //--------------------------------
        if (cAilmentEnemy != null
        && bResult == false)
        {
            MasterDataDefineLabel.AilmentGroup nGroupNum = MasterDataDefineLabel.AilmentGroup.NONE;
            for (int idx = 0; idx < cAilmentEnemy.Length; ++idx)
            {
                if (cAilmentEnemy[idx] == null)
                {
                    continue;
                }

                for (int num = 0; num < (int)MasterDataDefineLabel.AilmentType.MAX; ++num)
                {
                    if (cAilmentEnemy[idx].IsHavingAilment((MasterDataDefineLabel.AilmentType)num) == false)
                    {
                        continue;
                    }

                    // 状態異常グループを取得
                    nGroupNum = StatusAilment.getAilmentGroupFromAilmentType((MasterDataDefineLabel.AilmentType)num);

                    // 該当するグループの場合
                    if (nGroupNum == master_data_skill_active.Get_AILMENT_GROUP_TYPE1())
                    {
                        bResult = true;
                        break;
                    }
                }
            }
        }
        return (bResult);
    }

    //---------------------------------------------------------------------
    /*!
        @brief		アクティブ(ノーマル)スキル：特性タイプ：HANDS数
        @param[in]	ref int[]	(anParamList)	特性汎用パラメータ配列
        @param[in]		int		(nSkillNum)		スキル発動数
        @note
    */
    //---------------------------------------------------------------------
    static private bool ChkSkillAbilityHands(MasterDataSkillActive master_data_skill_active, int nSkillNum)
    {
        bool bResult = false;

        //--------------------------------
        // 判定値を取得
        //--------------------------------
        int nChkMin = master_data_skill_active.Get_HANDS_NUM_MIN();
        int nChkMax = master_data_skill_active.Get_HANDS_NUM_MAX();


        //--------------------------------
        // 最小値より、最大値が大きい場合(範囲内処理)
        //--------------------------------
        if (nChkMin < nChkMax)
        {
            //--------------------------------
            // 判定：最小値以上、最大値未満の場合
            //--------------------------------
            if (nSkillNum >= nChkMin
            && nSkillNum < nChkMax)
            {
                bResult = true;
            }
        }
        //--------------------------------
        // 最小値より、最大値が大きくない場合(上限無し処理)
        //--------------------------------
        else
        {
            //--------------------------------
            // 判定：最小値以上の場合
            //--------------------------------
            if (nSkillNum >= nChkMin)
            {
                bResult = true;
            }
        }

        return (bResult);
    }

    //---------------------------------------------------------------------
    /*!
        @brief		アクティブ(ノーマル)スキル：特性タイプ：Rate数
        @param[in]	ref int[]	(anParamList)	特性汎用パラメータ配列
        @param[in]		int		(nSkillNum)		スキル発動数
        @note
    */
    //---------------------------------------------------------------------
    static private bool ChkSkillAbilityRate(MasterDataSkillActive master_data_skill_active, int nSkillNum)
    {
        bool bResult = false;

        //------------------------------
        // Rate値を取得
        // ※floatだと正確に判定できないので、intに直す
        //------------------------------
        int nSkillRate = (int)(GetSkillCountRate(nSkillNum) * 100.0f);

        //--------------------------------
        // 判定値を取得
        //--------------------------------
        int nChkMin = master_data_skill_active.Get_RATE_NUM_MIN();
        int nChkMax = master_data_skill_active.Get_RATE_NUM_MAX();


        //--------------------------------
        // 最小値より、最大値が大きい場合(範囲内処理)
        //--------------------------------
        if (nChkMin < nChkMax)
        {
            //--------------------------------
            // 判定：最小値以上、最大値未満の場合
            //--------------------------------
            if (nSkillRate >= nChkMin
            && nSkillRate < nChkMax)
            {
                bResult = true;
            }
        }
        //--------------------------------
        // 最小値より、最大値が大きくない場合(上限無し処理)
        //--------------------------------
        else
        {
            //--------------------------------
            // 判定：最小値以上の場合
            //--------------------------------
            if (nSkillRate >= nChkMin)
            {
                bResult = true;
            }
        }

        return (bResult);
    }
}; // class InGameUtil

/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
