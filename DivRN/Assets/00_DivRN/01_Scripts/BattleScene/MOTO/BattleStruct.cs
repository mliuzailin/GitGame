/*==========================================================================*/
/*==========================================================================*/
/*!
    @file	BattleSkillStruct.cs
    @brief	バトルスキル関連構造体定義
    @author Developer
    @date 	2012/11/14
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
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
    @brief	リーチパラメータクラス
*/
//----------------------------------------------------------------------------
public class BattleSkillReachInfo
{
    private MasterDataDefineLabel.ElementType[,] m_ReachInfos = null;
    private MasterDataDefineLabel.ElementType[] m_HandAreaElements = null;

    public BattleSkillReachInfo(int field_area_count, int hand_area_count)
    {
        m_ReachInfos = new MasterDataDefineLabel.ElementType[field_area_count, hand_area_count];
    }

    public void resetReachInfo(MasterDataDefineLabel.ElementType[] hand_area_elements)
    {
        for (int field_index = 0; field_index < m_ReachInfos.GetLength(0); field_index++)
        {
            for (int hand_index = 0; hand_index < m_ReachInfos.GetLength(1); hand_index++)
            {
                m_ReachInfos[field_index, hand_index] = MasterDataDefineLabel.ElementType.NONE;
            }
        }

        if (m_HandAreaElements == null && hand_area_elements != null)
        {
            m_HandAreaElements = new MasterDataDefineLabel.ElementType[hand_area_elements.Length];
        }

        for (int idx = 0; idx < m_HandAreaElements.Length; idx++)
        {
            m_HandAreaElements[idx] = hand_area_elements[idx];
        }
    }

    public void addReachInfo(int field_index, MasterDataDefineLabel.ElementType element_type)
    {
        for (int hand_index = 0; hand_index < m_ReachInfos.GetLength(1); hand_index++)
        {
            if (m_HandAreaElements[hand_index] == element_type
                && m_ReachInfos[field_index, hand_index] == MasterDataDefineLabel.ElementType.NONE)
            {
                m_ReachInfos[field_index, hand_index] = element_type;
            }
        }
    }

    public MasterDataDefineLabel.ElementType getReachInfo(int field_index, int hand_index)
    {
        return m_ReachInfos[field_index, hand_index];
    }
}

//----------------------------------------------------------------------------
/*!
    @brief	戦闘時攻撃リクエスト
    @note	現時点の場で確定しているスキルの情報保持
*/
//----------------------------------------------------------------------------
[Serializable]
public class BattleSkillReq
{
    public enum State
    {
        NONE = 0,               //!< 攻撃ステータス：
        REQUESTED = 1,              //!< 攻撃ステータス：リクエストアサイン中
        RUNNING = 2,                //!< 攻撃ステータス：攻撃実行中
        FINISH = 3,             //!< 攻撃ステータス：攻撃完遂
    }
    //----------------------------------------------------------------------------

    public State m_SkillReqState = BattleSkillReq.State.NONE;   //!< 攻撃リクエスト情報：リクエスト状態
    public GlobalDefine.PartyCharaIndex m_SkillParamCharaNum = GlobalDefine.PartyCharaIndex.LEADER;                                 //!< 攻撃リクエスト情報：発行者番号
    public int m_SkillParamFieldNum = 0;                                    //!< 攻撃リクエスト情報：フィールド番号
    public uint m_SkillParamSkillID = 0;                                    //!< 攻撃リクエスト情報：スキルID

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
        @brief	デフォルトコンストラクタ
    */
    //----------------------------------------------------------------------------
    public BattleSkillReq()
    {
        m_SkillReqState = BattleSkillReq.State.NONE;
        m_SkillParamCharaNum = 0;
        m_SkillParamFieldNum = 0;
        m_SkillParamSkillID = 0;
    }
    //----------------------------------------------------------------------------
    /*!
        @brief	コピーコンストラクタ
    */
    //----------------------------------------------------------------------------
    public BattleSkillReq(BattleSkillReq cOrigin)
    {
        m_SkillReqState = cOrigin.m_SkillReqState;
        m_SkillParamCharaNum = cOrigin.m_SkillParamCharaNum;
        m_SkillParamFieldNum = cOrigin.m_SkillParamFieldNum;
        m_SkillParamSkillID = cOrigin.m_SkillParamSkillID;
    }
}

//----------------------------------------------------------------------------
// @enum	スキルタイプ定義
//----------------------------------------------------------------------------
public enum ESKILLTYPE
{
    //	eNORMAL,							//!< ノーマルスキル
    eACTIVE,                            //!< アクティブスキル
    eLEADER,                            //!< リーダースキル
    ePASSIVE,                           //!< パッシブスキル
    eLIMITBREAK,                        //!< リミットブレイクスキル
    eBOOST,                             //!< ブーストスキル
    eLINK,                              //!< リンクスキル
    eLINKPASSIVE,                       //!< リンクパッシブ
    eENEMY,								//!< エネミースキル
}

/// <summary>
/// 復活情報
/// </summary>
public class ResurrectInfo
{
    public int m_FixCount;  // 確定復活人数
    public int m_FixSpUse;  // 確定分復活時ＳＰ消費

    public int m_AddCount;  // 追加復活人数
    public int m_AddChancePercent;  // 追加分復活確率（％）
    public int m_AddSpUse;  // 追加分復活時ＳＰ消費

    public int m_HpPercent;	// 復活時ＨＰ％
}

//----------------------------------------------------------------------------
/*!
    @brief	戦闘時スキル発動情報
    @note	「誰が、誰に向かって、何を、発動するか。ダメージ値はどれくらいか。」をまとめたクラス。
            発動直前の最終確定情報として、実際のスキルの発動に使用する
*/
//----------------------------------------------------------------------------
[Serializable]
public class BattleSkillActivity
{
    public GlobalDefine.PartyCharaIndex m_SkillParamOwnerNum = GlobalDefine.PartyCharaIndex.LEADER;             //!< スキルリクエスト情報：加害者番号（プレイヤー）
    public int m_SkillParamFieldID = 0;             //!< スキルリクエスト情報：フィールドID
    public uint m_SkillParamSkillID = 0;                //!< スキルリクエスト情報：スキルID
    public string m_SkillName = null;               //!< スキル名（表示名）

    public BattleSkillTarget[] m_SkillParamTarget = null;               //!< スキルリクエスト情報：対象

    public int m_nAttackReqNum = 0;
    public bool m_bBonusBoost = false;          //!< ブーストフラグ
    public ESKILLTYPE m_SkillType = ESKILLTYPE.eACTIVE;
    public int m_SkillIndex = 0;
    public MasterDataDefineLabel.UIEffectType m_Effect = 0;                //!< エフェクト
    public MasterDataDefineLabel.ElementType m_Element = 0;                //!< 属性
    public MasterDataDefineLabel.SkillType m_Type = 0;              //!< 目的タイプ
    [SerializeField]
    private int m_Category = 0;                //!< スキル効果
                                               //m_Category は MasterDataDefineLabel.SkillCategory か MasterDataDefineLabel.BoostSkillType のどちらかの値になります.(m_SkillType が eBOOST の時に MasterDataDefineLabel.BoostSkillType になる？)
    public MasterDataDefineLabel.SkillCategory m_Category_SkillCategory_PROPERTY { get { return (MasterDataDefineLabel.SkillCategory)m_Category; } set { m_Category = (int)value; } }
    public MasterDataDefineLabel.BoostSkillCategory m_Category_BoostSkillCategory_PROPERTY { get { return (MasterDataDefineLabel.BoostSkillCategory)m_Category; } set { m_Category = (int)value; } }
    public bool m_Skip = false;

    public int m_skill_power = 0;                                       //!< 攻撃力の割合
    public int m_skill_power_fix = 0;                                       //!< 固定ダメージ量
    public int m_skill_power_hp_rate = 0;                                       //!< 対象HPの割合

    public int m_skill_absorb = 0;                                      //!< 吸収量
    public int m_skill_kickback = 0;                                        //!< 反動ダメージ（割合）
    public int m_skill_kickback_fix = 0;                                        //!< 反動ダメージ（値指定）

    public MasterDataDefineLabel.BoolType m_skill_chk_atk_affinity = MasterDataDefineLabel.BoolType.ENABLE;       //!< 弱点チェック
    public MasterDataDefineLabel.BoolType m_skill_chk_atk_leader = MasterDataDefineLabel.BoolType.ENABLE;       //!< リーダースキルチェック
    public MasterDataDefineLabel.BoolType m_skill_chk_atk_passive = MasterDataDefineLabel.BoolType.ENABLE;       //!< パッシブスキルチェック
    public MasterDataDefineLabel.BoolType m_skill_chk_atk_ailment = MasterDataDefineLabel.BoolType.ENABLE;       //!< 状態チェック
    public MasterDataDefineLabel.BoolType m_skill_chk_atk_combo = MasterDataDefineLabel.BoolType.DISABLE;      //!< コンボレートチェック

    public MasterDataDefineLabel.BoolType m_skill_chk_def_defence = MasterDataDefineLabel.BoolType.ENABLE;       //!< 防御チェック
    public MasterDataDefineLabel.BoolType m_skill_chk_def_ailment = MasterDataDefineLabel.BoolType.ENABLE;       //!< 状態チェック
    public MasterDataDefineLabel.BoolType m_skill_chk_def_barrier = MasterDataDefineLabel.BoolType.ENABLE;      //!< 状態バリアチェック

    public MasterDataDefineLabel.TargetType m_statusAilment_target = MasterDataDefineLabel.TargetType.NONE;     //!< 状態異常効果対象
    public int[] m_statusAilment = null;                                        //!< 状態異常ID

    public bool m_bCritical = false;                                    //!< クリティカルフラグ

    public int m_nStatusAilmentDelay = 0;                                       //!< 状態異常発動遅延。※2015/08/24現在 ０：遅延無し、非０：遅延有り。将来的に遅延ターン数の指定要望が来るかも知れないのでintで保持。
    public int m_CutinID = -1;  //Developer カットインＩＤを保存



    private MasterDataSkillActive m_MasterDataSkillActive = null;
    private MasterDataSkillLimitBreak m_MasterDataSkillLimitBreak = null;
    private MasterDataSkillBoost m_MasterDataSkillBoost = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public BattleSkillActivity()
    {
    }


    /// <summary>
    /// ダメージあるかないか。 m_skill_damage_enableの代わり
    /// </summary>
    /// <returns></returns>
    public bool is_skill_damage_enable()
    {
        bool ret_val = (m_skill_power != 0)
            || (m_skill_power_fix != 0)
            || (m_skill_power_hp_rate != 0);

        return ret_val;
    }

    public void _setParam(MasterDataSkillActive master_data_skill_active)
    {
        m_MasterDataSkillActive = master_data_skill_active;
        m_MasterDataSkillLimitBreak = null;
        m_MasterDataSkillBoost = null;
    }

    public void _setParam(MasterDataSkillLimitBreak master_data_skill_limit_break)
    {
        m_MasterDataSkillActive = null;
        m_MasterDataSkillLimitBreak = master_data_skill_limit_break;
        m_MasterDataSkillBoost = null;
    }

    public void _setParam(MasterDataSkillBoost master_data_skill_boost)
    {
        m_MasterDataSkillActive = null;
        m_MasterDataSkillLimitBreak = null;
        m_MasterDataSkillBoost = master_data_skill_boost;
    }

    //SKILLPARAM_ATK_DEATH_RATE
    public int Get_ATK_DEATH_RATE()
    {
        return m_MasterDataSkillLimitBreak.Get_ATK_DEATH_RATE();
    }

    //SKILLPARAM_COST_CHANGE_PREV
    public MasterDataDefineLabel.ElementType Get_COST_CHANGE_PREV()
    {
        return m_MasterDataSkillLimitBreak.Get_COST_CHANGE_PREV();
    }

    //SKILLPARAM_COST_CHANGE_AFTER
    public MasterDataDefineLabel.ElementType Get_COST_CHANGE_AFTER()
    {
        return m_MasterDataSkillLimitBreak.Get_COST_CHANGE_AFTER();
    }

    //SKILLPARAM_SUPPORT_C_FIX_HANDCARD
    public bool Get_SUPPORT_C_FIX_HANDCARD()
    {
        return m_MasterDataSkillLimitBreak.Get_SUPPORT_C_FIX_HANDCARD();
    }

    //SKILLPARAM_SUPPORT_C_FIX_ELEM
    public MasterDataDefineLabel.ElementType Get_SUPPORT_C_FIX_ELEM()
    {
        return m_MasterDataSkillLimitBreak.Get_SUPPORT_C_FIX_ELEM();
    }

    //SKILLPARAM_SUPPORT_TELEPORT_RANDOM
    public bool Get_SUPPORT_TELEPORT_RANDOM()
    {
        return m_MasterDataSkillLimitBreak.Get_SUPPORT_TELEPORT_RANDOM();
    }
    //SKILLPARAM_SUPPORT_TELEPORT_POS_X
    public int Get_SUPPORT_TELEPORT_POS_X()
    {
        return m_MasterDataSkillLimitBreak.Get_SUPPORT_TELEPORT_POS_X();
    }

    //SKILLPARAM_SUPPORT_TELEPORT_POS_Y
    public int Get_SUPPORT_TELEPORT_POS_Y()
    {
        return m_MasterDataSkillLimitBreak.Get_SUPPORT_TELEPORT_POS_Y();
    }

    //SKILLPARAM_COST_CHANGE_ALL_ELEMENT_0
    public MasterDataDefineLabel.ElementType[] Get_COST_CHANGE_ALL_ELEMENT()
    {
        return m_MasterDataSkillLimitBreak.Get_COST_CHANGE_ALL_ELEMENT();
    }

    //SKILLPARAM_FIX_DMG_ELEM_T_ELEM
    public MasterDataDefineLabel.ElementType Get_FIX_DMG_ELEM_T_ELEM()
    {
        return m_MasterDataSkillLimitBreak.Get_FIX_DMG_ELEM_T_ELEM();
    }

    //SKILLPARAM_ATK_ELEM_TARGET_ELEM
    public MasterDataDefineLabel.ElementType Get_ATK_ELEM_TARGET_ELEM()
    {
        return m_MasterDataSkillLimitBreak.Get_ATK_ELEM_TARGET_ELEM();
    }

    //SKILLPARAM_RECOVERY_HP_VALUE
    public int Get_RECOVERY_HP_VALUE()
    {
        return m_MasterDataSkillLimitBreak.Get_RECOVERY_HP_VALUE();
    }
    //SKILLPARAM_RECOVERY_HP_VALUE_RAND
    public int Get_RECOVERY_HP_VALUE_RAND()
    {
        return m_MasterDataSkillLimitBreak.Get_RECOVERY_HP_VALUE_RAND();
    }

    //SKILLPARAM_RECOVERY_FIX_HP_VALUE
    public int Get_RECOVERY_FIX_HP_VALUE()
    {
        return m_MasterDataSkillLimitBreak.Get_RECOVERY_FIX_HP_VALUE();
    }

    //SKILLPARAM_SUPPORT_SPESTATE_CLEAR_TARGET
    public MasterDataDefineLabel.TargetType Get_SUPPORT_SPESTATE_CLEAR_TARGET()
    {
        return m_MasterDataSkillLimitBreak.Get_SUPPORT_SPESTATE_CLEAR_TARGET();
    }

    //SKILLPARAM_SUPPORT_SPESTATE_CLEAR_GOOD_BAD
    public StatusAilment.GoodOrBad Get_SUPPORT_SPESTATE_CLEAR_GOOD_BAD()
    {
        return m_MasterDataSkillLimitBreak.Get_SUPPORT_SPESTATE_CLEAR_GOOD_BAD();
    }

    //SKILLPARAM_RECOVERY_SP_VALUE
    public int Get_RECOVERY_SP_VALUE()
    {
        return m_MasterDataSkillLimitBreak.Get_RECOVERY_SP_VALUE();
    }

    //SKILLPARAM_SUPPORT_LBS_TURN_FAST_TURN
    public int Get_SUPPORT_LBS_TURN_FAST_TURN()
    {
        return m_MasterDataSkillLimitBreak.Get_SUPPORT_LBS_TURN_FAST_TURN();
    }


    //-------------------
    //BOOSTSKILL_HEAL_SP_VALUE
    public int Get_BOOSTSKILL_HEAL_SP_VALUE()
    {
        return m_MasterDataSkillBoost.Get_HEAL_SP_VALUE();
    }

    //BOOSTSKILL_HEAL_HP_RATE_VALUE
    public int Get_BOOSTSKILL_HEAL_HP_RATE_VALUE()
    {
        return m_MasterDataSkillBoost.Get_HEAL_HP_RATE_VALUE();
    }

    //BOOSTSKILL_HEAL_HP_FIX_VALUE
    public int Get_BOOSTSKILL_HEAL_HP_FIX_VALUE()
    {
        return m_MasterDataSkillBoost.Get_HEAL_HP_FIX_VALUE();
    }

    //BOOSTSKILL_HAND_CNG_PANEL_LEFT
    public MasterDataDefineLabel.ElementType[] Get_BOOSTSKILL_HAND_CNG_PANEL()
    {
        return m_MasterDataSkillBoost.Get_HAND_CNG_PANEL();
    }

    //BOOSTSKILL_HAND_CNG_ELEM_NAUGHT
    public MasterDataDefineLabel.ElementType[] Get_BOOSTSKILL_HAND_CNG_ELEM()
    {
        return m_MasterDataSkillBoost.Get_HAND_CNG_ELEM();
    }

    //BOOSTSKILL_LBS_TURN_REDUCE_VALUE
    public int Get_BOOSTSKILL_LBS_TURN_REDUCE_VALUE()
    {
        return m_MasterDataSkillBoost.Get_LBS_TURN_REDUCE_VALUE();
    }

    //BOOSTSKILL_LBS_TURN_REDUCE_TARGET
    public MasterDataDefineLabel.TargetType Get_BOOSTSKILL_LBS_TURN_REDUCE_TARGET()
    {
        return m_MasterDataSkillBoost.Get_LBS_TURN_REDUCE_TARGET();
    }

    public BattleFieldPanelChangeParam GetBattleFieldPanelChangeParam()
    {
        if (m_SkillType == ESKILLTYPE.eBOOST)
        {
            return m_MasterDataSkillBoost.GetBattleFieldPanelChangeParam();
        }
        else
        {
            if (m_MasterDataSkillLimitBreak != null)
            {
                return m_MasterDataSkillLimitBreak.GetBattleFieldPanelChangeParam();
            }
        }

        return null;
    }

    public MasterDataSkillActive getMasterDataSkillActive()
    {
        return m_MasterDataSkillActive;
    }

    public MasterDataSkillLimitBreak getMasterDataSkillLimitBreak()
    {
        return m_MasterDataSkillLimitBreak;
    }

    /// <summary>
    /// 常駐アクティブスキルかどうか
    /// </summary>
    /// <returns></returns>
    public bool isActiveSkillAlways()
    {
        bool ret_val = (m_SkillType == ESKILLTYPE.eACTIVE
            && m_MasterDataSkillActive.always == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public ResurrectInfo getResurrectInfo()
    {
        ResurrectInfo ret_val = null;

        switch (m_SkillType)
        {
            case ESKILLTYPE.eACTIVE:
                if (m_MasterDataSkillActive != null)
                {
                    ret_val = m_MasterDataSkillActive.getResurrectInfo();
                }
                break;

            case ESKILLTYPE.eBOOST:
                if (m_MasterDataSkillBoost != null)
                {
                    ret_val = m_MasterDataSkillBoost.getResurrectInfo();
                }
                break;

            case ESKILLTYPE.eLIMITBREAK:
                if (m_MasterDataSkillLimitBreak != null)
                {
                    ret_val = m_MasterDataSkillLimitBreak.getResurrectInfo();
                }
                break;
        }

        return ret_val;
    }

    /// <summary>
    /// ヘイト増減値取得
    /// </summary>
    /// <returns></returns>
    public int getHateValue()
    {
        int ret_val = 0;
        switch (m_SkillType)
        {
            case ESKILLTYPE.eACTIVE:
                if (m_MasterDataSkillActive != null)
                {
                    ret_val = m_MasterDataSkillActive.hate_value;
                }
                break;

            case ESKILLTYPE.eBOOST:
                if (m_MasterDataSkillBoost != null)
                {
                    ret_val = m_MasterDataSkillBoost.hate_value;
                }
                break;

            case ESKILLTYPE.eLIMITBREAK:
                if (m_MasterDataSkillLimitBreak != null)
                {
                    ret_val = m_MasterDataSkillLimitBreak.hate_value;
                }
                break;
        }

        return ret_val;
    }

    /// <summary>
    /// 挑発ターン数取得
    /// </summary>
    /// <returns></returns>
    public int getProvokeTurn()
    {
        int ret_val = 0;
        switch (m_SkillType)
        {
            case ESKILLTYPE.eLIMITBREAK:
                if (m_MasterDataSkillLimitBreak != null)
                {
                    ret_val = m_MasterDataSkillLimitBreak.getProvokeTurn();
                }
                break;
        }

        return ret_val;
    }


    //------------------------------------------------------------------------
    //	@brief		スキル情報取得：メインテキストを取得
    //------------------------------------------------------------------------
    public string getMainText()
    {
        if (m_SkillName != null)
        {
            return m_SkillName;
        }

        uint skill_id = m_SkillParamSkillID;
        m_SkillName = "";
        MasterDataSkillLeader skillLeader = null;
        MasterDataSkillPassive skillPassive = null;
        MasterDataSkillLimitBreak skillLimitBreak = null;
        MasterDataSkillActive skillActive = null;

        switch (m_SkillType)
        {
            case ESKILLTYPE.eLEADER:
                // リーダースキルの処理
                skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(skill_id);
                if (skillLeader != null)
                {
                    m_SkillName = skillLeader.name;
                }
                break;

            case ESKILLTYPE.ePASSIVE:
                // パッシブスキルの処理
                skillPassive = BattleParam.m_MasterDataCache.useSkillPassive(skill_id);
                if (skillPassive != null)
                {
                    m_SkillName = skillPassive.name;
                }
                break;

            case ESKILLTYPE.eLIMITBREAK:
                // リミットブレイクスキルの処理
                skillLimitBreak = BattleParam.m_MasterDataCache.useSkillLimitBreak(skill_id);
                if (skillLimitBreak != null)
                {
                    m_SkillName = skillLimitBreak.name;
                }
                break;

            case ESKILLTYPE.eACTIVE:
                // アクティブスキルの処理
                skillActive = BattleParam.m_MasterDataCache.useSkillActive(skill_id);
                if (skillActive != null)
                {
                    m_SkillName = skillActive.name;
                }
                break;

            case ESKILLTYPE.eBOOST:
                // ブーストスキルの処理
                // ブーストスキル名は BattleSkillActivity を作る側で設定する必要がある。
                break;

            case ESKILLTYPE.eLINK:
                // リンクスキルの処理
                skillActive = BattleParam.m_MasterDataCache.useSkillActive(skill_id);
                if (skillActive != null)
                {
                    m_SkillName = skillActive.Get_skill_link_name();
                }
                break;

            case ESKILLTYPE.eLINKPASSIVE:
                // リンクパッシブの処理
                skillPassive = BattleParam.m_MasterDataCache.useSkillPassive(skill_id);
                if (skillPassive != null)
                {
                    m_SkillName = skillPassive.name;
                }
                break;

            default:
                break;
        }

        return m_SkillName;
    }


    //------------------------------------------------------------------------
    //	@brief		スキル情報取得：サブテキストを取得
    //------------------------------------------------------------------------
    public string getSubText()
    {

        string sub_text = "";

        switch (m_SkillType)
        {
            case ESKILLTYPE.eACTIVE:
                {
                    switch (m_Type)
                    {
                        case MasterDataDefineLabel.SkillType.ATK_ALL:
                        case MasterDataDefineLabel.SkillType.ATK_ONCE:
                            // 攻撃力等の数値表示
                            {
                                // キャラデータ取得
                                CharaOnce cChara = null;
                                if (BattleParam.isInitilaizedBattle()
                                && BattleParam.m_PlayerParty != null)
                                {
                                    GlobalDefine.PartyCharaIndex nOwnerNum = GlobalDefine.PartyCharaIndex.ERROR;
                                    if ((m_SkillParamOwnerNum >= 0) &&
                                         ((int)m_SkillParamOwnerNum < BattleParam.m_PlayerParty.getPartyMemberMaxCount()))
                                    {
                                        nOwnerNum = m_SkillParamOwnerNum;
                                    }
                                    else
                                    {
                                        nOwnerNum = 0;
                                    }

                                    cChara = BattleParam.m_PlayerParty.getPartyMember(nOwnerNum, CharaParty.CharaCondition.EXIST);
                                }

                                // 攻撃力表示
                                int damage = 0;
                                if (cChara != null)
                                {
                                    damage = InGameUtilBattle.GetDamageValue(BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.LEADER, CharaParty.CharaCondition.SKILL_LEADER),
                                                                        BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.FRIEND, CharaParty.CharaCondition.SKILL_LEADER),
                                                                        cChara, null, null, false,
                                                                        BattleParam.m_PlayerParty.m_Ailments.getAilment(m_SkillParamOwnerNum), 0,
                                                                        this);
                                    // activity.m_Element,
                                    // activity.m_Category,
                                    // activity.m_Param );
                                }
                                sub_text = "ATK " + damage.ToString();
                            }
                            break;

                        case MasterDataDefineLabel.SkillType.HEAL:
                            // 回復量表示
                            {
                                BattleSceneUtil.MultiInt nTotal = new BattleSceneUtil.MultiInt();
                                if (m_SkillParamTarget != null)
                                {
                                    for (int n = 0; n < m_SkillParamTarget.Length; n++)
                                    {
                                        nTotal.addValue(GlobalDefine.PartyCharaIndex.MAX, m_SkillParamTarget[n].m_SkillValueToPlayer);
                                    }
                                }
                                sub_text = "HEAL " + nTotal.getValue(GlobalDefine.PartyCharaIndex.MAX).ToString();
                            }
                            break;

                        case MasterDataDefineLabel.SkillType.SUPPORT:
                            // 補助効果表示
                            break;

                        case MasterDataDefineLabel.SkillType.NONE:

                        default:
                            // 無し
                            Debug.LogError("SKILL_TYPE_UNKNOWN");
                            break;
                    }
                }
                break;

            case ESKILLTYPE.eLEADER:
            case ESKILLTYPE.ePASSIVE:
            case ESKILLTYPE.eLIMITBREAK:
            case ESKILLTYPE.eBOOST:
            case ESKILLTYPE.eLINK:
            case ESKILLTYPE.eLINKPASSIVE:
            default:
                break;
        }

        return sub_text;
    }
}



//----------------------------------------------------------------------------
/*!
    @brief	戦闘時スキル対象情報
    @note	「誰に対してダメージ値はどれくらいか。」をまとめたクラス。
            発動直前の最終確定情報として、実際のスキルの発動に使用する
*/
//----------------------------------------------------------------------------
[Serializable]
public class BattleSkillTarget
{
    public enum TargetType
    {
        PLAYER = 0,
        ENEMY = 1
    }

    public TargetType m_TargetType = TargetType.PLAYER;             //!< スキルリクエスト情報：対象タイプ
    public int m_TargetNum = 0;                             //!< スキルリクエスト情報：対象番号（ターゲットが敵の場合に使用）
    public int m_SkillValueToEnemy = 0;                             //!< スキルリクエスト情報：効果値（ターゲットが敵の場合に使用）
    public BattleSceneUtil.MultiInt m_SkillValueToPlayer = new BattleSceneUtil.MultiInt();  //!< スキルリクエスト情報：効果値（ターゲットがプレイヤーパーティの場合に使用）


    //------------------------------------------------------------------------
    /*!
        @brief	デフォルトコンストラクタ。
    */
    //------------------------------------------------------------------------
    public BattleSkillTarget()
    {
        m_TargetType = 0;
        m_TargetNum = 0;
        m_SkillValueToEnemy = 0;
        m_SkillValueToPlayer.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
    }

    //------------------------------------------------------------------------
    /*!
        @brief	コピーコンストラクタ。
    */
    //------------------------------------------------------------------------
    public BattleSkillTarget(BattleSkillTarget cOrigin)
    {
        m_TargetType = cOrigin.m_TargetType;
        m_TargetNum = cOrigin.m_TargetNum;
        m_SkillValueToEnemy = cOrigin.m_SkillValueToEnemy;
        m_SkillValueToPlayer.setValue(GlobalDefine.PartyCharaIndex.MAX, cOrigin.m_SkillValueToPlayer);
    }
}




//----------------------------------------------------------------------------
/*!
    @brief	スキル発動リクエストパラメータ
*/
//----------------------------------------------------------------------------
public class SkillRequestParam
{
    private BattleSkillActivity[] m_SkillActivityArray = null;
    private int m_SkillActivityCount = 0;   // スキルリクエスト登録数.
    private int m_Progress = 0; // スキル実行時カウンタ
    private static int m_LastAttackEnemyIndex = -1; // プレイヤーが最後に攻撃した敵

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="max_skill_request">スキルリクエストの最大数</param>
    public SkillRequestParam(int max_skill_request)
    {
        m_SkillActivityArray = new BattleSkillActivity[max_skill_request];
        m_SkillActivityCount = 0;
        m_Progress = 0;
    }

    /// <summary>
    /// 登録されているスキルリクエストの数を返す
    /// </summary>
    /// <returns></returns>
    public int getRequestCount()
    {
        return m_SkillActivityCount;
    }

    public BattleSkillActivity getSkillRequestByIndex(int index)
    {
        return m_SkillActivityArray[index];
    }

    /// <summary>
    /// リクエストを全消去
    /// </summary>
    public void clearRequest()
    {
        for (int idx = 0; idx < m_SkillActivityCount; idx++)
        {
            m_SkillActivityArray[idx] = null;
        }
        m_SkillActivityCount = 0;
        m_Progress = 0;
    }

    /// <summary>
    /// スキルリクエストを追加
    /// </summary>
    /// <param name="skill_activity"></param>
    /// <returns></returns>
    public bool addSkillRequest(BattleSkillActivity skill_activity)
    {
        if (skill_activity != null)
        {
            if (m_SkillActivityCount < m_SkillActivityArray.Length)
            {
                m_SkillActivityArray[m_SkillActivityCount] = skill_activity;
                m_SkillActivityCount++;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// スキルリクエストを追加
    /// </summary>
    /// <param name="activity_array"></param>
    public void addSkillRequest(BattleSkillActivity[] activity_array, int count)
    {
        if (activity_array != null)
        {
            for (int idx = 0; idx < count; idx++)
            {
                if (activity_array[idx] != null)
                {
                    addSkillRequest(activity_array[idx]);
                }
            }
        }
    }

    /// <summary>
    /// スキルリクエストを追加
    /// </summary>
    /// <param name="skill_request_param"></param>
    public void addSkillRequest(SkillRequestParam skill_request_param)
    {
        if (skill_request_param != null)
        {
            addSkillRequest(skill_request_param.m_SkillActivityArray, skill_request_param.m_SkillActivityCount);
        }
    }

    public int getCurrentProgress()
    {
        return m_Progress;
    }

    public int getSkillActivityCount()
    {
        return m_SkillActivityCount;
    }

    public BattleSkillActivity getCurrentSkillRequest()
    {
        if (m_Progress < m_SkillActivityCount)
        {
            return m_SkillActivityArray[m_Progress];
        }
        return null;
    }

    public void nextProgress()
    {
        m_Progress++;
    }

    /// <summary>
    /// スキルのソート優先順位
    /// </summary>
    private static MasterDataDefineLabel.SkillType[] anSortPriority =   {
                                    MasterDataDefineLabel.SkillType.CURSE_ONCE	//0
                                ,   MasterDataDefineLabel.SkillType.CURSE_ALL	//1
                                ,   MasterDataDefineLabel.SkillType.SUPPORT	//2
                                ,   MasterDataDefineLabel.SkillType.HEAL	//3 汎用回復
                                ,   MasterDataDefineLabel.SkillType.RESURRECT	//4 復活（復活は汎用回復の後、回復の前）
                                ,   MasterDataDefineLabel.SkillType.HEAL	//5 回復
                                ,   MasterDataDefineLabel.SkillType.HEAL_SP	//6
                                ,   MasterDataDefineLabel.SkillType.ATK_ALL	//7
                                ,   MasterDataDefineLabel.SkillType.ATK_ONCE	//8
                                };

    public static bool filterSkill(MasterDataSkillActive master_data_skill_active, SkillFilterType filter_type)
    {
        if (master_data_skill_active != null)
        {
            if (filter_type == SkillFilterType.ALL)
            {
                return true;
            }

            bool is_first_half = false; // 前半スキルかどうか
            if (master_data_skill_active.skill_type == MasterDataDefineLabel.SkillType.CURSE_ONCE
                || master_data_skill_active.skill_type == MasterDataDefineLabel.SkillType.CURSE_ALL
                || master_data_skill_active.skill_type == MasterDataDefineLabel.SkillType.SUPPORT
                || master_data_skill_active.skill_type == MasterDataDefineLabel.SkillType.RESURRECT
            )
            {
                is_first_half = true;
            }
            else if (master_data_skill_active.skill_type == MasterDataDefineLabel.SkillType.HEAL)
            {
                if (master_data_skill_active.always == MasterDataDefineLabel.BoolType.ENABLE)
                {
                    is_first_half = true;
                }
            }

            if (filter_type == SkillFilterType.FIRST_HALF && is_first_half)
            {
                return true;
            }

            if (filter_type == SkillFilterType.LAST_HALF && is_first_half == false)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// スキルが対象優先度かどうかを調べる
    /// </summary>
    /// <param name="activity"></param>
    /// <param name="priority_index"></param>
    /// <returns></returns>
    private static bool isPriority(BattleSkillActivity wrk_activity, int priority_index)
    {
        MasterDataDefineLabel.SkillType skill_type = anSortPriority[priority_index];
        if (skill_type != MasterDataDefineLabel.SkillType.HEAL)
        {
            if (wrk_activity.m_Type == skill_type)
            {
                return true;
            }
            return false;
        }
        else
        {
            // スキルタイプ回復の時の区別（スキルタイプを増やすべきだがとりあえず）
            bool is_match_cond = false;
            switch (priority_index)
            {
                case 3: // 汎用回復
                    is_match_cond = (wrk_activity.m_Type == MasterDataDefineLabel.SkillType.HEAL
                        && wrk_activity.isActiveSkillAlways());
                    break;

                case 5: // 回復
                    is_match_cond = (wrk_activity.m_Type == MasterDataDefineLabel.SkillType.HEAL
                        && wrk_activity.isActiveSkillAlways() == false);
                    break;
            }
            return is_match_cond;
        }
    }

    public enum SkillFilterType
    {
        ALL,        // 全部
        FIRST_HALF, // 前半のみを抽出（復活スキルまで）
        LAST_HALF,  // 後半のみを抽出（復活スキルの後から）
    }

    /// <summary>
    /// m_SkillActivityArray から各タイミングごとのスキルを抽出しソート.
    /// </summary>
    /// <param name="filter_type"></param>
    public void sortSkillRequest(SkillFilterType filter_type)
    {
        BattleSkillActivity[] src_activity_array = new BattleSkillActivity[m_SkillActivityCount];
        for (int idx = 0; idx < m_SkillActivityCount; idx++)
        {
            src_activity_array[idx] = m_SkillActivityArray[idx];
        }

        BattleSkillActivity[] activity_array = new BattleSkillActivity[m_SkillActivityCount];
        int activity_array_index = 0;
        BattleSkillActivity[] link_skill_activity_array = new BattleSkillActivity[SkillLink.SKILL_LINK_MAX];
        int link_skill_activity_array_index = 0;

        // スキルをスキル優先順位・パーティメンバー順に並び替える。またアクティブスキルとリンクスキルへ振り分ける.
        for (int priority_idx = 0; priority_idx < anSortPriority.Length; priority_idx++)
        {
            BattleSkillActivity[] wrk_activity_array = _selectActivityBySkillType(priority_idx, src_activity_array);
            for (int idx = 0; idx < wrk_activity_array.Length; idx++)
            {
                BattleSkillActivity wrk_activity = wrk_activity_array[idx];

                if (wrk_activity.m_SkillType != ESKILLTYPE.eLINK)
                {
                    activity_array[activity_array_index] = wrk_activity;
                    activity_array_index++;
                }
                else
                {
                    link_skill_activity_array[link_skill_activity_array_index] = wrk_activity;
                    link_skill_activity_array_index++;
                }
            }
        }

        // @add Developer 2015/08/31 ver300
        // リンクスキルをアクティブスキル配列の適切な位置へ挿入（各ユニット最後のスキルの後ろにくっつける）
        for (int link_idx = 0; link_idx < link_skill_activity_array_index; link_idx++)
        {
            BattleSkillActivity wrk_link_skill_activity = link_skill_activity_array[link_idx];
            // 末尾から発動者毎の最終スキルを検索
            for (int idx = activity_array_index - 1; idx >= 0; idx--)
            {
                BattleSkillActivity wrk_activity = activity_array[idx];

                // 最終スキルの後に、リンクスキルを差込み
                if (wrk_activity.m_SkillParamOwnerNum == wrk_link_skill_activity.m_SkillParamOwnerNum)
                {
                    for (int idx2 = activity_array_index - 1; idx2 > idx; idx2--)
                    {
                        activity_array[idx2 + 1] = activity_array[idx2];
                    }

                    activity_array[idx + 1] = wrk_link_skill_activity;

                    activity_array_index++;
                    break;
                }
            }
        }

        for (int idx = 0; idx < m_SkillActivityCount; idx++)
        {
            m_SkillActivityArray[idx] = null;
        }
        m_SkillActivityCount = 0;

        // ソート結果を書き戻し
        if (filter_type != SkillFilterType.ALL)
        {
            // スキルを抽出（一度すべてのスキルをソートしてからでないとリンクスキルの発動タイミングがおかしくなる場合があるのでこのタイミングで抽出）

            // 後半スキルの先頭を検索
            int last_half_top_index = activity_array_index;
            for (int idx = 0; idx < activity_array_index; idx++)
            {
                BattleSkillActivity wrk_activity = activity_array[idx];
                if (wrk_activity.m_SkillType != ESKILLTYPE.eLINK)
                {
                    if (isPriority(wrk_activity, 5)
                        || isPriority(wrk_activity, 6)
                        || isPriority(wrk_activity, 7)
                        || isPriority(wrk_activity, 8)
                    )
                    {
                        last_half_top_index = idx;
                        break;
                    }
                }
            }

            if (filter_type == SkillFilterType.FIRST_HALF)
            {
                // 前半スキルを書き出し
                for (int idx = 0; idx < last_half_top_index; idx++)
                {
                    m_SkillActivityArray[m_SkillActivityCount] = activity_array[idx];
                    m_SkillActivityCount++;
                }
            }
            else
            {
                // 後半スキルを書き出し
                for (int idx = last_half_top_index; idx < activity_array_index; idx++)
                {
                    m_SkillActivityArray[m_SkillActivityCount] = activity_array[idx];
                    m_SkillActivityCount++;
                }
            }
        }
        else
        {
            // 全スキルを書き出し
            for (int idx = 0; idx < activity_array_index; idx++)
            {
                m_SkillActivityArray[m_SkillActivityCount] = activity_array[idx];
                m_SkillActivityCount++;
            }
        }

        //Developer 以前はここで全アクティビティの攻撃対象などを決定していたが( calcAttackValue() )、スキル発動タイミングで行うように変更
        //Developer この変更により、BattleSkillActivity.m_nHealTmp が不要になったので削除（BattleSkillActivity.m_nHealTmp の処理内容は過去バージョン(Ver3代)を参照してください）
    }

    /// <summary>
    /// アクティブスキルの中から指定優先順位のスキルを抜き出しパーティメンバー順にソートしたものを返す。
    /// </summary>
    /// <param name="skill_priority_index"></param>
    /// <param name="src_activity_array"></param>
    /// <returns></returns>
    private static BattleSkillActivity[] _selectActivityBySkillType(int skill_priority_index, BattleSkillActivity[] src_activity_array)
    {
        BattleSkillActivity[] wrk_activity_array = new BattleSkillActivity[src_activity_array.Length];
        int wrk_activity_index = 0;

        for (int idx = 0; idx < src_activity_array.Length; idx++)
        {
            BattleSkillActivity wrk_activity = src_activity_array[idx];
            if (wrk_activity == null)
            {
                continue;
            }

            if (isPriority(wrk_activity, skill_priority_index) == false)
            {
                continue;
            }

            wrk_activity_array[wrk_activity_index] = wrk_activity;
            wrk_activity_index++;
        }

        BattleSkillActivity[] ret_val = new BattleSkillActivity[wrk_activity_index];
        int ret_val_index = 0;

        // パーティメンバー順のソートもしておく
        // 汎用回復・ヒーロースキルは先に
        {
            for (int idx = 0; idx < wrk_activity_index; idx++)
            {
                BattleSkillActivity wrk_activity = wrk_activity_array[idx];

                // 発動者を取得
                GlobalDefine.PartyCharaIndex ownerNum = wrk_activity.m_SkillParamOwnerNum;

                // 発動者とキャラ番号が同じ場合
                if ((ownerNum == GlobalDefine.PartyCharaIndex.GENERAL/* && wrk_activity.m_Type == MasterDataDefineLabel.SkillType.HEAL*/)
                    || ownerNum == GlobalDefine.PartyCharaIndex.HERO)
                {
                    ret_val[ret_val_index] = wrk_activity;
                    ret_val_index++;

                    wrk_activity_array[idx] = null; // ソート済みは削除(重複防止)
                }
            }
        }

        // パーティ順にスキルを格納
        for (int member_idx = 0; member_idx < (int)GlobalDefine.PartyCharaIndex.MAX; member_idx++)
        {
            for (int idx = 0; idx < wrk_activity_index; idx++)
            {
                BattleSkillActivity wrk_activity = wrk_activity_array[idx];
                if (wrk_activity == null)
                {
                    continue;
                }

                // 発動者を取得
                GlobalDefine.PartyCharaIndex ownerNum = wrk_activity.m_SkillParamOwnerNum;

                // 発動者とキャラ番号が同じ場合
                if (ownerNum == (GlobalDefine.PartyCharaIndex)member_idx)
                {
                    ret_val[ret_val_index] = wrk_activity;
                    ret_val_index++;

                    wrk_activity_array[idx] = null; // ソート済みは削除(重複防止)
                }
            }
        }

        return ret_val;
    }

    /// <summary>
    /// 最後に攻撃した敵をリセット
    /// </summary>
    public static void resetAttackTarget()
    {
        m_LastAttackEnemyIndex = -1;
    }

    /// <summary>
    /// アクティビティの攻撃力などを計算
    /// 現在のプレイヤーパーティの状況・敵パーティの状況を考慮して計算される.
    /// </summary>
    /// <param name="enemyParam"></param>
    /// <param name="combo_count"></param>
    /// <param name="boostField"></param>
    public static void calcAttackValue(BattleSkillActivity dest_skill_activity,
                                    int combo_count,
                                    bool[] boostField)
    {
        // スキル発動情報の起動順にダメージを反映。
        CharaOnce wrk_target_chara = new CharaOnce();
        {
            //-------------------------------
            // スキル発動者を取得
            //-------------------------------
            CharaOnce cOwnerChara = null;
            if ((dest_skill_activity.m_SkillParamOwnerNum < GlobalDefine.PartyCharaIndex.MAX)
            && (dest_skill_activity.m_SkillParamOwnerNum >= 0))
            {
                cOwnerChara = BattleParam.m_PlayerParty.getPartyMember(dest_skill_activity.m_SkillParamOwnerNum, CharaParty.CharaCondition.EXIST);
            }

            //-------------------------------
            //	ブースト効果の判定
            //-------------------------------
            bool bonusBoost = false;
            if (dest_skill_activity.m_SkillParamFieldID != InGameDefine.SELECT_NONE)
            {

                int fieldID = dest_skill_activity.m_SkillParamFieldID;
                if (fieldID >= 0 && fieldID < boostField.Length)
                {
                    bonusBoost = (boostField[fieldID] == true) ? true : false;
                }
            }

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル発動[" + dest_skill_activity.m_SkillType.ToString() + ":" + dest_skill_activity.getMainText() + "]"
                + " 発動者:" + dest_skill_activity.m_SkillParamOwnerNum.ToString()
                + " 場番号:" + dest_skill_activity.m_SkillParamFieldID.ToString()
                + (bonusBoost ? "(BOOST)" : "")
                + " 効果種類:" + dest_skill_activity.m_Type.ToString()
            );

            //-------------------------------
            //	攻撃対象の選択と、
            //	スキル効果タイプに応じたダメージ計算処理を行う。
            //-------------------------------
            switch (dest_skill_activity.m_Type)
            {
                case MasterDataDefineLabel.SkillType.ATK_ONCE:
                    //-------------------------------
                    //	スキルタイプ：単体攻撃
                    //-------------------------------
                    {
                        int nWeakChara = -1;

                        //-------------------------------
                        //	対象リストの選定
                        //-------------------------------
                        bool[] targetArray = new bool[BattleParam.m_EnemyParam.Length];
                        for (int n = 0; n < BattleParam.m_EnemyParam.Length; n++)
                        {

                            targetArray[n] = false;

                            if (BattleParam.m_EnemyParam[n] == null)
                            {
                                continue;
                            }

                            // 死亡チェック
                            if (BattleParam.m_EnemyParam[n].isDead() == true)
                            {
                                continue;
                            }

                            // スキルタイプによる分岐
                            switch (dest_skill_activity.m_SkillType)
                            {
                                case ESKILLTYPE.eBOOST:
                                    switch (dest_skill_activity.m_Category_BoostSkillCategory_PROPERTY)
                                    {
                                        default:
                                            targetArray[n] = true;
                                            break;
                                    }

                                    break;

                                default:
                                    // カテゴリによる条件分岐
                                    switch (dest_skill_activity.m_Category_SkillCategory_PROPERTY)
                                    {
                                        case MasterDataDefineLabel.SkillCategory.DAMAGE_ELEM:
                                            {
                                                // 該当属性だった場合、リストに追加
                                                MasterDataDefineLabel.ElementType targetElem = dest_skill_activity.Get_FIX_DMG_ELEM_T_ELEM();
                                                targetArray[n] = (BattleParam.m_EnemyParam[n].getMasterDataParamChara().element == targetElem) ? true : false;
                                            }
                                            break;

                                        case MasterDataDefineLabel.SkillCategory.ATK_ELEM:
                                            {
                                                // 該当属性だった場合、リストに追加
                                                MasterDataDefineLabel.ElementType targetElem = dest_skill_activity.Get_ATK_ELEM_TARGET_ELEM();
                                                targetArray[n] = (BattleParam.m_EnemyParam[n].getMasterDataParamChara().element == targetElem) ? true : false;
                                            }
                                            break;

                                        default:
                                            targetArray[n] = true;
                                            break;
                                    }

                                    break;
                            }
                        }

                        //-------------------------------
                        //	選定された対象リストの中からの条件分岐
                        //-------------------------------

                        //-------------------------------
                        //	ターゲッティングしてる相手を対象とする
                        //-------------------------------
                        if (nWeakChara == -1)
                        {
                            for (int nEnemyAccess = 0; nEnemyAccess < targetArray.Length; nEnemyAccess++)
                            {
                                if (targetArray[nEnemyAccess] == false)
                                {
                                    continue;
                                }

                                if (BattleParam.m_TargetEnemyCurrent == nEnemyAccess)
                                {
                                    nWeakChara = nEnemyAccess;
                                }
                            }
                        }


                        //-------------------------------
                        //	残り体力を考慮し、攻撃対象を決定する
                        //-------------------------------
                        if (nWeakChara == -1)
                        {

                            int min_hp_index = -1;
                            int min_hp = int.MaxValue;

                            for (int nEnemyAccess = 0; nEnemyAccess < targetArray.Length; nEnemyAccess++)
                            {
                                if (targetArray[nEnemyAccess] == false
                                || BattleParam.m_EnemyParam[nEnemyAccess] == null)
                                {
                                    continue;
                                }

                                if (BattleParam.m_EnemyParam[nEnemyAccess].m_EnemyHPMax <= 0)
                                {
                                    continue;
                                }

                                int wrk_hp = BattleParam.m_EnemyParam[nEnemyAccess].m_EnemyHP;
                                if (wrk_hp <= 0)
                                {
                                    continue;
                                }

                                if (wrk_hp == BattleParam.m_EnemyParam[nEnemyAccess].m_EnemyHPMax)
                                {
                                    continue;
                                }


                                if (wrk_hp < min_hp)
                                {
                                    min_hp_index = nEnemyAccess;
                                    min_hp = wrk_hp;
                                }
                            }

                            if (min_hp_index != -1)
                            {
                                nWeakChara = min_hp_index;
                            }
                        }

                        //-------------------------------
                        //	弱点属性な相手を優先し、攻撃対象を設定
                        //-------------------------------
                        if (nWeakChara == -1)
                        {
                            for (int nEnemyAccess = 0; nEnemyAccess < targetArray.Length; nEnemyAccess++)
                            {
                                if (targetArray[nEnemyAccess] == false
                                || BattleParam.m_EnemyParam[nEnemyAccess] == null)
                                {
                                    continue;
                                }

                                if (BattleParam.m_EnemyParam[nEnemyAccess].m_EnemyHP <= 0)
                                {
                                    continue;
                                }

                                MasterDataDefineLabel.ElementType nEnemyElement = BattleParam.m_EnemyParam[nEnemyAccess].getMasterDataParamChara().element;
                                if (InGameUtilBattle.GetSkillElementRate(dest_skill_activity.m_Element, nEnemyElement) <= 1.0f)
                                {
                                    continue;
                                }

                                nWeakChara = nEnemyAccess;
                                break;
                            }
                        }

                        //-------------------------------
                        //	普通にダメージ通る相手を攻撃対象に設定
                        //-------------------------------
                        if (nWeakChara == -1)
                        {

                            for (int nEnemyAccess = 0; nEnemyAccess < targetArray.Length; nEnemyAccess++)
                            {
                                if (targetArray[nEnemyAccess] == false
                                || BattleParam.m_EnemyParam[nEnemyAccess] == null)
                                {
                                    continue;
                                }

                                if (BattleParam.m_EnemyParam[nEnemyAccess].m_EnemyHP <= 0)
                                {
                                    continue;
                                }

                                MasterDataDefineLabel.ElementType nEnemyElement = BattleParam.m_EnemyParam[nEnemyAccess].getMasterDataParamChara().element;
                                if (InGameUtilBattle.GetSkillElementRate(dest_skill_activity.m_Element, nEnemyElement) <= 0.5f)
                                {
                                    continue;
                                }

                                nWeakChara = nEnemyAccess;
                                break;
                            }

                        }


#if true //Developer TODO:この判定は生存している中の先頭キャラが必ず選択され、これ以降の判定がされない。
                        //-------------------------------
                        //	軽減されるけど体力残ってる相手で妥協
                        //-------------------------------
                        if (nWeakChara == -1)
                        {

                            for (int nEnemyAccess = 0; nEnemyAccess < targetArray.Length; nEnemyAccess++)
                            {
                                if (targetArray[nEnemyAccess] == false
                                || BattleParam.m_EnemyParam[nEnemyAccess] == null)
                                {
                                    continue;
                                }

                                if (BattleParam.m_EnemyParam[nEnemyAccess].m_EnemyHP <= 0)
                                {
                                    continue;
                                }

                                nWeakChara = nEnemyAccess;
                                break;
                            }

                        }
#endif


                        //-------------------------------
                        //	攻撃力の高いやつを優先
                        //-------------------------------
                        if (nWeakChara == -1)
                        {
                            int highAtkChara = -1;
                            float highAtk = 0;

                            for (int nEnemyAccess = 0; nEnemyAccess < targetArray.Length; nEnemyAccess++)
                            {

                                if (targetArray[nEnemyAccess] == false
                                || BattleParam.m_EnemyParam[nEnemyAccess] == null)
                                {
                                    continue;
                                }

                                if (BattleParam.m_EnemyParam[nEnemyAccess].m_EnemyHP <= 0)
                                {
                                    continue;
                                }

                                if (highAtkChara == -1
                                || highAtk > BattleParam.m_EnemyParam[nEnemyAccess].m_EnemyAttack)
                                {
                                    highAtkChara = nEnemyAccess;
                                    highAtk = BattleParam.m_EnemyParam[nEnemyAccess].m_EnemyAttack;
                                }
                            }

                            if (highAtkChara != -1)
                            {
                                nWeakChara = highAtkChara;
                            }
                        }


                        //-------------------------------
                        //	前回死んでなかった相手で妥協
                        //-------------------------------
                        if (nWeakChara == -1)
                        {

                            for (int nEnemyAccess = 0; nEnemyAccess < targetArray.Length; nEnemyAccess++)
                            {
                                if (targetArray[nEnemyAccess] == false
                                || BattleParam.m_EnemyParam[nEnemyAccess] == null)
                                {
                                    continue;
                                }

                                if (BattleParam.m_EnemyParam[nEnemyAccess].m_EnemyHP <= 0)
                                {
                                    continue;
                                }

                                nWeakChara = nEnemyAccess;
                                break;
                            }

                        }

                        //-------------------------------
                        //	攻撃対象が決まっていない場合、
                        //	最後に攻撃した相手を対象として設定
                        //	※それすらいない状況の場合は、強制的に先頭の敵を対象とする
                        //-------------------------------
                        if (nWeakChara == -1)
                        {
                            if (m_LastAttackEnemyIndex == -1)
                            {

                                // 弱点キャラがいないかつ、最後に殴ったキャラがいない場合、
                                // NULLじゃないやつを探す
                                for (int n = 0; n < BattleParam.m_EnemyParam.Length; n++)
                                {
                                    if (BattleParam.m_EnemyParam[n] == null)
                                    {
                                        continue;
                                    }

                                    // 死亡している場合
                                    if (BattleParam.m_EnemyParam[n].isDead() == true)
                                    {
                                        // 既に非表示化されていれば処理しない
                                        if (!BattleParam.m_EnemyParam[n].isShow())
                                        {
                                            continue;
                                        }
                                    }

                                    m_LastAttackEnemyIndex = n;
                                    break;
                                }

                                // それでもいないならあきらめる
                                if (m_LastAttackEnemyIndex == -1)
                                {
                                    break;
                                }
                            }

                            nWeakChara = m_LastAttackEnemyIndex;
                            dest_skill_activity.m_Skip = true;
                        }

                        //-------------------------------
                        //	最後に攻撃した相手として記録しておく
                        //-------------------------------
                        m_LastAttackEnemyIndex = nWeakChara;


                        //-------------------------------
                        //	ターゲット情報の作成とダメージ量の算出
                        //-------------------------------
                        wrk_target_chara.CharaSetupFromParamEnemy(BattleParam.m_EnemyParam[nWeakChara].getMasterDataParamEnemy());

                        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　攻撃対象:敵" + nWeakChara.ToString());

                        //-------------------------------
                        //	スキル発動情報の作成
                        //-------------------------------
                        dest_skill_activity.m_SkillParamTarget = new BattleSkillTarget[1];
                        dest_skill_activity.m_SkillParamTarget[0] = new BattleSkillTarget();
                        dest_skill_activity.m_SkillParamTarget[0].m_TargetType = BattleSkillTarget.TargetType.ENEMY;
                        dest_skill_activity.m_SkillParamTarget[0].m_TargetNum = nWeakChara;
                        dest_skill_activity.m_SkillParamTarget[0].m_SkillValueToEnemy = InGameUtilBattle.GetDamageValue(BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.LEADER, CharaParty.CharaCondition.SKILL_LEADER),
                                                                                                             BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.FRIEND, CharaParty.CharaCondition.SKILL_LEADER),
                                                                                                             cOwnerChara, wrk_target_chara,
                                                                                                             BattleParam.m_EnemyParam[nWeakChara],
                                                                                                             bonusBoost,
                                                                                                             BattleParam.m_PlayerParty.m_Ailments.getAilment(dest_skill_activity.m_SkillParamOwnerNum),
                                                                                                             combo_count,
                                                                                                             dest_skill_activity);
                    }
                    break;

                case MasterDataDefineLabel.SkillType.ATK_ALL:
                    //-------------------------------
                    // スキルタイプ：全体攻撃
                    //-------------------------------
                    {
                        bool allDead = true;

                        dest_skill_activity.m_SkillParamTarget = new BattleSkillTarget[BattleParam.m_EnemyParam.Length];
                        for (int nCt = 0; nCt < BattleParam.m_EnemyParam.Length; nCt++)
                        {

                            //-------------------------------
                            //	エラーチェック
                            //-------------------------------
                            if (BattleParam.m_EnemyParam[nCt] == null)
                            {
                                continue;
                            }


                            //-------------------------------
                            //	敵の生存チェックを行う。
                            //-------------------------------
                            if (BattleParam.m_EnemyParam[nCt].isDead() == false)
                            {
                                allDead = false;
                                m_LastAttackEnemyIndex = nCt;
                            }

                            //-------------------------------
                            //	対象のキャラ情報を作成
                            //-------------------------------
                            wrk_target_chara.CharaSetupFromParamEnemy(BattleParam.m_EnemyParam[nCt].getMasterDataParamEnemy());

                            //-------------------------------
                            //	スキルタイプによる分岐
                            //	@change Developer 2016/06/03 v350
                            //	@note	スキルタイプ毎のカテゴリ仕様分岐
                            //-------------------------------
                            switch (dest_skill_activity.m_SkillType)
                            {
                                case ESKILLTYPE.eLIMITBREAK:
                                    if (dest_skill_activity.m_Category_SkillCategory_PROPERTY == MasterDataDefineLabel.SkillCategory.DAMAGE_ELEM)
                                    {
                                        //-------------------------------
                                        // 固定ダメージ
                                        // 対象属性以外はターゲットにしない
                                        //-------------------------------
                                        MasterDataDefineLabel.ElementType targetElem = dest_skill_activity.Get_FIX_DMG_ELEM_T_ELEM();
                                        if (BattleParam.m_EnemyParam[nCt].getMasterDataParamChara().element != targetElem)
                                        {
                                            continue;
                                        }

                                    }
                                    else if (dest_skill_activity.m_Category_SkillCategory_PROPERTY == MasterDataDefineLabel.SkillCategory.ATK_ELEM)
                                    {
                                        //-------------------------------
                                        // 倍率ダメージ
                                        // 対象属性以外はターゲットにしない
                                        //-------------------------------
                                        MasterDataDefineLabel.ElementType targetElem = dest_skill_activity.Get_ATK_ELEM_TARGET_ELEM();
                                        if (BattleParam.m_EnemyParam[nCt].getMasterDataParamChara().element != targetElem)
                                        {
                                            continue;
                                        }

                                    }
                                    break;

                                default:
                                    break;
                            }

                            //-------------------------------
                            //	スキル発動情報の作成
                            //-------------------------------
                            dest_skill_activity.m_SkillParamTarget[nCt] = new BattleSkillTarget();
                            if (dest_skill_activity == null)
                            {
                                continue;
                            }

                            //-------------------------------
                            //	攻撃対象がいない場合は強制的に先頭キャラを対象とする
                            //-------------------------------
                            if (m_LastAttackEnemyIndex == -1)
                            {

                                // NULLじゃないやつを探す
                                for (int n = 0; n < BattleParam.m_EnemyParam.Length; n++)
                                {
                                    if (BattleParam.m_EnemyParam[n] == null)
                                    {
                                        continue;
                                    }

                                    // 死亡している場合
                                    if (BattleParam.m_EnemyParam[n].isDead() == true)
                                    {
                                        // 既に非表示化されていれば処理しない
                                        if (!BattleParam.m_EnemyParam[n].isShow())
                                        {
                                            continue;
                                        }
                                    }

                                    m_LastAttackEnemyIndex = n;
                                    break;
                                }

                            }

                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　攻撃対象:敵" + nCt.ToString());

                            dest_skill_activity.m_SkillParamTarget[nCt].m_TargetType = BattleSkillTarget.TargetType.ENEMY;
                            dest_skill_activity.m_SkillParamTarget[nCt].m_TargetNum = nCt;
                            dest_skill_activity.m_SkillParamTarget[nCt].m_SkillValueToEnemy = InGameUtilBattle.GetDamageValue(BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.LEADER, CharaParty.CharaCondition.SKILL_LEADER),
                                                                                                                     BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.FRIEND, CharaParty.CharaCondition.SKILL_LEADER),
                                                                                                                     cOwnerChara, wrk_target_chara,
                                                                                                                     BattleParam.m_EnemyParam[nCt],
                                                                                                                     bonusBoost,
                                                                                                                     BattleParam.m_PlayerParty.m_Ailments.getAilment(dest_skill_activity.m_SkillParamOwnerNum),
                                                                                                                     combo_count,
                                                                                                                     dest_skill_activity);
                        }

                        // 演出スキップ
                        if (allDead == true)
                        {
                            dest_skill_activity.m_Skip = true;
                        }
                    }
                    break;

                case MasterDataDefineLabel.SkillType.HEAL:
                case MasterDataDefineLabel.SkillType.RESURRECT:
                    //-------------------------------
                    // スキルタイプ：回復
                    //-------------------------------
                    {
                        InGameUtilBattle.CreateHealTarget(ref dest_skill_activity, bonusBoost);
                    }
                    break;

                case MasterDataDefineLabel.SkillType.SUPPORT:
                case MasterDataDefineLabel.SkillType.HEAL_SP:
                case MasterDataDefineLabel.SkillType.CURSE_ONCE:
                    //-------------------------------
                    //	補助、回復、弱体
                    //-------------------------------
                    {
                        // ターゲットは内部で処理されているため、とりあえずダミーターゲットを作成してみる
                        InGameUtilBattle.CreateTargetPlayer(ref dest_skill_activity);
                    }
                    break;

                case MasterDataDefineLabel.SkillType.CURSE_ALL:
                    //-------------------------------
                    //	全体弱体
                    //-------------------------------
                    {
                        // 敵全体をターゲットとして情報作成
                        dest_skill_activity.m_SkillParamTarget = new BattleSkillTarget[BattleParam.m_EnemyParam.Length];
                        for (int j = 0; j < BattleParam.m_EnemyParam.Length; j++)
                        {
                            if (BattleParam.m_EnemyParam[j] == null)
                            {
                                continue;
                            }

                            if (BattleParam.m_EnemyParam[j].isDead() == true)
                            {
                                continue;
                            }


                            BattleSkillTarget target = new BattleSkillTarget();
                            target.m_TargetType = BattleSkillTarget.TargetType.ENEMY;
                            target.m_TargetNum = j;
                            target.m_SkillValueToEnemy = 0;
                            dest_skill_activity.m_SkillParamTarget[j] = target;
                        }
                    }
                    break;

                default:
                    break;
            }

            dest_skill_activity.m_bBonusBoost = bonusBoost;
            dest_skill_activity.m_nAttackReqNum = combo_count;
        }
    }
} // class SkillRequestParam

/// <summary>
/// パネル変換パラメータ
/// </summary>
public class BattleFieldPanelChangeParam
{
    public bool m_IsBattleStartOnly;
    public int m_Odds;  //発動率
    public bool m_IsFieldClear;
    public MasterDataDefineLabel.FieldType m_FieldType;
    public MasterDataDefineLabel.MethodType m_MethodType;
    public int m_RandContent;
    public MasterDataDefineLabel.PanelType m_PnaleType0;
    public int m_PnaleNum0;
    public MasterDataDefineLabel.PanelType m_PnaleType1;
    public int m_PnaleNum1;
    public MasterDataDefineLabel.PanelType m_PnaleType2;
    public int m_PnaleNum2;
    public MasterDataDefineLabel.PanelType m_PnaleType3;
    public int m_PnaleNum3;
    public MasterDataDefineLabel.PanelType m_PnaleType4;
    public int m_PnaleNum4;

    public void init(int[] param_array)
    {
        m_IsBattleStartOnly = (MasterDataDefineLabel.BoolType)param_array[0] == MasterDataDefineLabel.BoolType.ENABLE;
        m_Odds = param_array[1];
        m_IsFieldClear = (MasterDataDefineLabel.BoolType)param_array[2] == MasterDataDefineLabel.BoolType.ENABLE;
        m_FieldType = (MasterDataDefineLabel.FieldType)param_array[3];
        m_MethodType = (MasterDataDefineLabel.MethodType)param_array[4];
        m_PnaleType0 = (MasterDataDefineLabel.PanelType)param_array[5];
        m_PnaleNum0 = param_array[6];
        m_PnaleType1 = (MasterDataDefineLabel.PanelType)param_array[7];
        m_PnaleNum1 = param_array[8];
        m_PnaleType2 = (MasterDataDefineLabel.PanelType)param_array[9];
        m_PnaleNum2 = param_array[10];
        m_PnaleType3 = (MasterDataDefineLabel.PanelType)param_array[11];
        m_PnaleNum3 = param_array[12];
        m_PnaleType4 = (MasterDataDefineLabel.PanelType)param_array[13];
        m_PnaleNum4 = param_array[14];
        m_RandContent = param_array[15];
    }

    public MasterDataDefineLabel.PanelType getPanelType(int index)
    {
        MasterDataDefineLabel.PanelType[] panel_types =
        {
            m_PnaleType0,
            m_PnaleType1,
            m_PnaleType2,
            m_PnaleType3,
            m_PnaleType4,
        };

        return panel_types[index];
    }

    public int getPanelNum(int index)
    {
        int[] panel_nums =
        {
            m_PnaleNum0,
            m_PnaleNum1,
            m_PnaleNum2,
            m_PnaleNum3,
            m_PnaleNum4,
        };

        return panel_nums[index];
    }
}

/// <summary>
/// スキルターン検証用データ
/// </summary>
[System.Serializable]
public class SkillTurnCondition
{
    [Tooltip("１枚カードスキルが発動した場合消化するスキルターン数")]
    public int m_1CardSkill = 0;    //[DVGAN-2067]ver5.0.0 一旦無効に
    [Tooltip("２枚カードスキルが発動した場合消化するスキルターン数")]
    public int m_2CardSkill = 0;    //[DVGAN-2067]ver5.0.0 一旦無効に
    [Tooltip("３枚カードスキルが発動した場合消化するスキルターン数")]
    public int m_3CardSkill = 1;
    [Tooltip("４枚カードスキルが発動した場合消化するスキルターン数")]
    public int m_4CardSkill = 1;
    [Tooltip("５枚カードスキルが発動した場合消化するスキルターン数")]
    public int m_5CardSkill = 1;

    [Tooltip("Ｎコンボ数以上になった場合１スキルターン消化")]
    public int m_ComboCount1 = 5;

    [Tooltip("Ｎコンボ数以上になった場合１スキルターン消化")]
    public int m_ComboCount2 = 10;

    [Tooltip("「FULL」になった場があった場合１スキルターン消化")]
    public bool m_FullField = false;    //[DVGAN-2067]ver5.0.0 一旦無効に

    [Tooltip("ブーストスキルが発動した場合１スキルターン消化")]
    public bool m_BoostSkill = false;   //[DVGAN-2067]ver5.0.0 一旦無効に

    [Tooltip("バトル開始時")]
    public int m_BattleStart = 0;

    [Tooltip("バトル終了時")]
    public int m_BattleEnd = 0;

    [Tooltip("敵進化時")]
    public int m_EnemyEvol = 0;

    [Tooltip("リミブレスキルによる敵全滅時")]
    public int m_LimitBreakWin = 1;
}
