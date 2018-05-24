/*==========================================================================*/
/*==========================================================================*/
/*!
    @file	SkillBoostr.cs
    @brief	ブーストスキルクラス
    @author Developer
    @date 	2014/11/07
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
    @brief	ブーストスキルクラス
*/
//----------------------------------------------------------------------------
public class SkillBoost
{

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    private SkillRequestParam m_SkillRequestBoost = new SkillRequestParam(BattleLogic.BATTLE_SKILL_TOTAL_MAX + SkillLink.SKILL_LINK_MAX);       //!< スキル発動リクエスト管理

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/

    // C#のルール：アクセサの頭文字は一般的に小文字(今後修正対応)
    public SkillRequestParam SkillRequestBoost
    {
        get
        {
            return (m_SkillRequestBoost);
        }
    }

    public int BoostReqInputNum
    {
        get
        {
            return (m_SkillRequestBoost.getRequestCount());
        }
    }

    //----------------------------------------------------------------------------
    /*!
    //	@brief		ブーストスキル発行情報リセット
    */
    //----------------------------------------------------------------------------
    public void ResetSkillRequestBoost()
    {

        if (m_SkillRequestBoost == null)
        {
            return;
        }

        m_SkillRequestBoost.clearRequest();
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：ブーストスキルによるアクションタイトル表示
    */
    //----------------------------------------------------------------------------
    public void CutinStart()
    {
        if (m_SkillRequestBoost == null
        || m_SkillRequestBoost.getRequestCount() <= 0
        )
        {
            Debug.LogError("SkillActivity Is None!");
            return;
        }

        BattleSkillCutinManager.Instance.ClrSkillCutin();
        BattleSkillCutinManager.Instance.SetSkillCutin(m_SkillRequestBoost);
        BattleSkillCutinManager.Instance.CutinStart2(true);	//ブーストスキル
    }

    /// <summary>
    /// アクティブスキルからブーストスキルの発動情報を抽出し追加
    /// </summary>
    /// <param name="active_skill_activity"></param>
    public void AddActivityBoostSkill(BattleSkillActivity active_skill_activity)
    {
        if (active_skill_activity == null
            || active_skill_activity.m_SkillType != ESKILLTYPE.eACTIVE)
        {
            return;
        }

        // ブーストパネルを未使用の場合
        if (active_skill_activity.m_bBonusBoost == false)
        {
            return;
        }

        MasterDataSkillActive skillActiveParam = active_skill_activity.getMasterDataSkillActive();
        if (skillActiveParam == null
        || skillActiveParam.skill_boost_id == 0)
        {
            return;
        }

        // ブーストスキル情報を取得
        MasterDataSkillBoost skillBoostParam = BattleParam.m_MasterDataCache.useSkillBoost(skillActiveParam.skill_boost_id);
        if (skillBoostParam == null)
        {
            return;
        }

        // ブーストスキルを構築
        BattleSkillActivity boost_skill_activity = new BattleSkillActivity();
        if (boost_skill_activity != null)
        {
            // 共通情報を設定
            boost_skill_activity.m_SkillParamOwnerNum = active_skill_activity.m_SkillParamOwnerNum;				// 発動者
            boost_skill_activity.m_SkillParamFieldID = InGameDefine.SELECT_NONE;								// フィールドID(ブーストパネルの判定にも使っている)
            boost_skill_activity.m_SkillParamSkillID = skillActiveParam.skill_boost_id;						// ブーストスキルID
            boost_skill_activity.m_SkillName = skillActiveParam.Get_skill_boost_name();				// ブーストスキル名
            boost_skill_activity.m_SkillType = ESKILLTYPE.eBOOST;									// 発動スキルの種類

            boost_skill_activity.m_Element = skillActiveParam.skill_boost_element;				// 基本情報：属性
            boost_skill_activity.m_Type = skillBoostParam.skill_type;							// 基本情報：スキルタイプ
            boost_skill_activity.m_Category_BoostSkillCategory_PROPERTY = skillBoostParam.skill_cate;							// 基本情報：効果カテゴリ
            boost_skill_activity.m_Effect = skillActiveParam.skill_boost_effect;					// 基本情報：エフェクト

            boost_skill_activity.m_skill_power = skillBoostParam.skill_power;							// 攻撃情報：攻撃力(％)
            boost_skill_activity.m_skill_power_fix = skillBoostParam.skill_power_fix;						// 攻撃情報：攻撃力(固定)
            boost_skill_activity.m_skill_power_hp_rate = skillBoostParam.skill_power_hp_rate;					// 攻撃情報：攻撃力(対象HPの割合)
            boost_skill_activity.m_skill_absorb = skillBoostParam.skill_absorb;							// 攻撃情報：吸収量(％)

            boost_skill_activity.m_skill_chk_atk_affinity = skillBoostParam.skill_chk_atk_affinity;				// 効果情報：攻撃側：属性相性チェック
            boost_skill_activity.m_skill_chk_atk_leader = skillBoostParam.skill_chk_atk_leader;					// 効果情報：攻撃側：リーダースキルチェック
            boost_skill_activity.m_skill_chk_atk_passive = skillBoostParam.skill_chk_atk_passive;				// 効果情報：攻撃側：パッシブスキルチェック
            boost_skill_activity.m_skill_chk_atk_ailment = skillBoostParam.skill_chk_atk_ailment;				// 効果情報：攻撃側：状態変化チェック
            boost_skill_activity.m_skill_chk_atk_combo = MasterDataDefineLabel.BoolType.DISABLE;					// 攻撃情報：攻撃側：コンボレートの影響

            boost_skill_activity.m_skill_chk_def_defence = skillBoostParam.skill_chk_def_defence;				// 効果情報：防御側：防御無視チェック
            boost_skill_activity.m_skill_chk_def_ailment = skillBoostParam.skill_chk_def_ailment;				// 効果情報：防御側：状態変化チェック
            boost_skill_activity.m_skill_chk_def_barrier = skillBoostParam.skill_chk_def_barrier;				// 効果情報：防御側：状態バリアチェック

            boost_skill_activity.m_statusAilment_target = skillBoostParam.status_ailment_target;				// 状態変化対象
            boost_skill_activity.m_statusAilment = new int[] {   skillBoostParam.status_ailment1,		// 状態変化1
                                                                    skillBoostParam.status_ailment2,		// 状態変化2
                                                                    skillBoostParam.status_ailment3,		// 状態変化3
                                                                    skillBoostParam.status_ailment4 };		// 状態変化4

            boost_skill_activity.m_nStatusAilmentDelay = skillBoostParam.status_ailment_delay;					// 状態変化遅延。

            // 汎用情報を設定
            boost_skill_activity._setParam(skillBoostParam);


            // リストに登録
            m_SkillRequestBoost.addSkillRequest(boost_skill_activity);
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		手札変換：パネル指定
    */
    //----------------------------------------------------------------------------
    public void ChangeHandPanel(BattleSkillActivity activity)
    {
        //------------------------------
        //	エラーチェック
        //------------------------------
        if (activity == null
        || BattleParam.isActiveBattle() == false)
        {
            return;
        }

        // バトルマネージャ取得
        BattleSceneManager battleMgr = BattleSceneManager.Instance;


        // 汎用パラメータから設定
        MasterDataDefineLabel.ElementType[] elemArray = activity.Get_BOOSTSKILL_HAND_CNG_PANEL();

        for (int i = 0; i < elemArray.Length; ++i)
        {
            BattleScene.BattleCard battle_card = battleMgr.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCard(i);

            if (battle_card == null
            || elemArray[i] == MasterDataDefineLabel.ElementType.MAX)
            {
                continue;
            }

            // 変換先が設定されている場合
            if (elemArray[i] != MasterDataDefineLabel.ElementType.NONE)
            {

                // 属性変更
                battle_card.setElementType(elemArray[i], BattleScene.BattleCard.ChangeCause.SKILL);

                //	手札変化エフェクト
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.HAND_CARD_AREA, i, BattleScene._BattleCardManager.EffectInfo.EffectType.CARD_CHANGE);
                // 設定されていない場合は、ランダム変換
            }
            else
            {

                // ランダム属性を取得
                MasterDataDefineLabel.ElementType randElement = (MasterDataDefineLabel.ElementType)RandManager.GetRand((int)MasterDataDefineLabel.ElementType.NAUGHT,
                                                            (int)MasterDataDefineLabel.ElementType.MAX);

                // 属性変更
                battle_card.setElementType(randElement, BattleScene.BattleCard.ChangeCause.SKILL);

                //	手札変化エフェクト
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.HAND_CARD_AREA, i, BattleScene._BattleCardManager.EffectInfo.EffectType.CARD_CHANGE);
            }
        }

    }

    //----------------------------------------------------------------------------
    /*!
        @brief		手札変換：属性指定
        @param[in]	BattleSkillActivity		(activity)		スキル発動情報
    */
    //----------------------------------------------------------------------------
    public void ChangeHandElement(BattleSkillActivity activity)
    {
        //------------------------------
        //	エラーチェック
        //------------------------------
        if (activity == null
        || BattleParam.isActiveBattle() == false)
        {
            return;
        }

        // バトルマネージャ取得
        BattleSceneManager battleMgr = BattleSceneManager.Instance;


        // 汎用パラメータから設定(属性ラベル定義順)
        MasterDataDefineLabel.ElementType[] elemArray = activity.Get_BOOSTSKILL_HAND_CNG_ELEM();


        MasterDataDefineLabel.ElementType cardElement;		// 手札の属性用

        //----------------------------------------
        //	手札をすべてチェック
        //----------------------------------------
        for (int i = 0; i < battleMgr.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCardMaxCount(); ++i)
        {
            BattleScene.BattleCard battle_card = battleMgr.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCard(i);

            if (battle_card == null)
            {
                continue;
            }


            // 手札の属性を取得
            cardElement = battle_card.getElementType();
            if (elemArray[(int)cardElement] == MasterDataDefineLabel.ElementType.MAX)
            {
                continue;
            }

            // 変換先が設定されている場合
            if (elemArray[(int)cardElement] != MasterDataDefineLabel.ElementType.NONE)
            {

                // 属性変更
                battle_card.setElementType(elemArray[(int)cardElement], BattleScene.BattleCard.ChangeCause.SKILL);

                //	手札変化エフェクト
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.HAND_CARD_AREA, i, BattleScene._BattleCardManager.EffectInfo.EffectType.CARD_CHANGE);

            }
            else
            {
                // 設定されていない場合は、ランダム変換

                // ランダム属性を取得
                MasterDataDefineLabel.ElementType randElement = (MasterDataDefineLabel.ElementType)RandManager.GetRand((int)MasterDataDefineLabel.ElementType.NAUGHT,
                                                            (int)MasterDataDefineLabel.ElementType.MAX);

                // 属性変更
                battle_card.setElementType(randElement, BattleScene.BattleCard.ChangeCause.SKILL);

                //	手札変化エフェクト
                BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.HAND_CARD_AREA, i, BattleScene._BattleCardManager.EffectInfo.EffectType.CARD_CHANGE);
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		フィールド変換
    */
    //----------------------------------------------------------------------------
    public void ChangeField()
    {
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		攻撃：対象属性限定
    */
    //----------------------------------------------------------------------------
    public void AttackElementTarget()
    {
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		LBS必要ターン数の短縮
        @param[in]	BattleSkillActivity		(activity)		スキル発動情報
    */
    //----------------------------------------------------------------------------
    public void ReduceLBSTurn(BattleSkillActivity activity)
    {
        if (activity == null)
        {
            return;
        }

        // スキルパラメータ取得
        GlobalDefine.PartyCharaIndex userID = activity.m_SkillParamOwnerNum;
        int turn = activity.Get_BOOSTSKILL_LBS_TURN_REDUCE_VALUE();
        MasterDataDefineLabel.TargetType target = activity.Get_BOOSTSKILL_LBS_TURN_REDUCE_TARGET();


        // 対象情報による分岐
        CharaOnce chara;
        switch (target)
        {
            // 発動者のみ
            case MasterDataDefineLabel.TargetType.SELF:
                // エラーチェック
                chara = BattleParam.m_PlayerParty.getPartyMember(userID, CharaParty.CharaCondition.SKILL_TURN1);
                if (chara == null)
                {
                    break;
                }

                // LBS必要ターン数短縮
                chara.AddCharaLimitBreak(turn);
                break;

            case MasterDataDefineLabel.TargetType.FRIEND:				// 発動者の味方全員
            case MasterDataDefineLabel.TargetType.ALL:					// 全員
            case MasterDataDefineLabel.TargetType.SELF_OTHER_FRIEND:	// 発動者以外の味方全員
            case MasterDataDefineLabel.TargetType.SELF_OTHER_ALL:		// 発動者以外の全員
                for (int num = 0; num < (int)GlobalDefine.PartyCharaIndex.MAX; ++num)
                {
                    // 発動者は効果適用外
                    if ((GlobalDefine.PartyCharaIndex)num == userID)
                    {
                        if (target == MasterDataDefineLabel.TargetType.SELF_OTHER_FRIEND
                        || target == MasterDataDefineLabel.TargetType.SELF_OTHER_ALL)
                        {
                            continue;
                        }
                    }

                    // エラーチェック
                    chara = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_TURN1);
                    if (chara == null)
                    {
                        continue;
                    }

                    // LBS必要ターン数短縮
                    chara.AddCharaLimitBreak(turn);
                }
                break;

            // 何も処理しない
            case MasterDataDefineLabel.TargetType.NONE:
            case MasterDataDefineLabel.TargetType.OTHER:
            case MasterDataDefineLabel.TargetType.ENEMY:
            case MasterDataDefineLabel.TargetType.ENE_N_1:
            case MasterDataDefineLabel.TargetType.ENE_1N_1:
            case MasterDataDefineLabel.TargetType.ENE_R_N:
            case MasterDataDefineLabel.TargetType.ENE_1_N:
            default:
                break;
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		状態異常クリア
    */
    //----------------------------------------------------------------------------
    public void ClearAbstate()
    {
    }

}
///////////////////////////////////////EOF///////////////////////////////////////