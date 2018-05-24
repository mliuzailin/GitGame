/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	CharaParty.cs
	@brief	キャラパーティ
	@author Developer
	@date 	2012/10/08

	情報の保持にのみ特化。
	[ メインメニュー , インゲーム ]の各シーンでの使用を想定してゲーム要素は組み込まない
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
	@brief	キャラパーティ
*/
//----------------------------------------------------------------------------
[Serializable]
public class CharaParty
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public int[] m_PartyTotalAtk = new int[(int)MasterDataDefineLabel.ElementType.MAX]; //!< パーティー総合情報：攻撃パラメータ
    public BattleSceneUtil.MultiInt m_HPBase = null;                                        //!< パーティー総合情報：体力最大値（基本値）
    public BattleSceneUtil.MultiInt m_HPMax = null;                                         //!< パーティー総合情報：体力最大値（スキルで補正された値）
    public BattleSceneUtil.MultiInt m_HPCurrent = null;                                 //!< パーティー総合情報：体力（現在値）
    public int m_PartyTotalSP = 0;                                          //!< パーティー総合情報：スタミナ
    public int m_PartyTotalSPMax = 20;                                          //!< パーティー総合情報：スタミナ最大値
    public int m_PartyTotalCost = 0;                                            //!< パーティー総合情報：総合コスト
    public double m_PartyTotalCharm = 0;                                            //!< パーティー総合情報：総合魅力
    public bool m_PartySetupOK = false;                                     //!< パーティー総合情報：セットアップ済み
    public BattleSceneUtil.MultiAilment m_Ailments = null;  // パーティ状態異常
    private CharaOnce[] m_PlayerPartyChara = null;

    private BattleSceneUtil.MultiInt m_DispHpDamageReg = null;  // パーティダメージ表示値（実際に減った値ではなく減らそうとした値）
    private BattleSceneUtil.MultiInt m_DispHpDamage = null; // パーティダメージ表示値（実際に減った値ではなく減らそうとした値）
    private BattleSceneUtil.MultiInt m_DispHpDamageTargetReg = null;    // パーティダメージ表示対象
    private BattleSceneUtil.MultiInt m_DispHpDamageTarget = null;   // パーティダメージ表示対象
    private BattleSceneUtil.MultiInt m_DispHpRecoveryReg = null;    // パーティ回復表示値（実際に回復した値ではなく回復しようとした値）
    private BattleSceneUtil.MultiInt m_DispHpRecovery = null;   // パーティ回復表示値（実際に回復した値ではなく回復しようとした値）
    private MasterDataDefineLabel.ElementType m_DispHpDamageRegElement = MasterDataDefineLabel.ElementType.NONE;
    private BattleSceneUtil.MultiInt m_DispHp = null;   // パーティのＨＰ表示値
    private float m_DispHpWait = 0.0f;

    public BattleSceneUtil.MultiInt m_Hate = null;  // 現在のヘイト値
    public BattleSceneUtil.MultiInt m_Hate_ProvokeTurn = null;  // 挑発の残りターン数
    public BattleSceneUtil.MultiInt m_Hate_ProvokeOrder = null; // 挑発をした順番（最後に発動した挑発のみが有効）
    public BattleSceneUtil.MultiInt m_Hate_1TurnDamage = new BattleSceneUtil.MultiInt();    //1ターンで与えたダメージの総計（パーティメンバー別）（ヘイト算出用）
    public BattleSceneUtil.MultiInt m_Hate_1TurnHeal = new BattleSceneUtil.MultiInt();  //1ターンで回復されたＨＰの総計（パーティメンバー別）（ヘイト算出用）

    private GlobalDefine.PartyCharaIndex m_GeneralPartyMember = GlobalDefine.PartyCharaIndex.LEADER;    // 汎用回復などの発動者

    public BattleHero m_BattleHero = new BattleHero(0, 0);

#if BUILD_TYPE_DEBUG
    public BattleSceneUtil.MultiInt m_DebugNoDeadFlag = new BattleSceneUtil.MultiInt();
#endif

    [System.Serializable]
    public class BattleAchive
    {
        public enum AchieveType
        {
            PANEL_1_SKILL,
            PANEL_2_SKILL,
            PANEL_3_SKILL,
            PANEL_4_SKILL,
            PANEL_5_SKILL,
            COMBO_5,
            COMBO_10,
            FULL_FIELD,
            BOOST,

            MAX
        }

        public bool m_Panel1Skill = false;
        public bool m_Panel2Skill = false;
        public bool m_Panel3Skill = false;
        public bool m_Panel4Skill = false;
        public bool m_Panel5Skill = false;
        public bool m_Combo5 = false;
        public bool m_Combo10 = false;
        public bool m_FullField = false;
        public bool m_Boost = false;

        public void resetAll()
        {
            m_Panel1Skill = false;
            m_Panel2Skill = false;
            m_Panel3Skill = false;
            m_Panel4Skill = false;
            m_Panel5Skill = false;
            m_Combo5 = false;
            m_Combo10 = false;
            m_FullField = false;
            m_Boost = false;
        }

        public bool isAchieved(AchieveType achieve_type)
        {
            switch (achieve_type)
            {
                case AchieveType.PANEL_1_SKILL:
                    return m_Panel1Skill;

                case AchieveType.PANEL_2_SKILL:
                    return m_Panel2Skill;

                case AchieveType.PANEL_3_SKILL:
                    return m_Panel3Skill;

                case AchieveType.PANEL_4_SKILL:
                    return m_Panel4Skill;

                case AchieveType.PANEL_5_SKILL:
                    return m_Panel5Skill;

                case AchieveType.COMBO_5:
                    return m_Combo5;

                case AchieveType.COMBO_10:
                    return m_Combo10;

                case AchieveType.FULL_FIELD:
                    return m_FullField;

                case AchieveType.BOOST:
                    return m_Boost;
            }

            return false;
        }
    }
    public BattleAchive m_BattleAchive = new BattleAchive();

    // パーティメンバー枠サイズ
    public int getPartyMemberMaxCount()
    {
        return (int)GlobalDefine.PartyCharaIndex.MAX;
    }

    public enum CharaCondition
    {
        EXIST,  //存在している（生死を問わない）
        ALIVE,  //存在して生存している
        DEAD,   //存在して死亡している

        SKILL_LEADER,   //リーダースキル所持判定対象
        SKILL_PASSIVE,  //パッシブスキル所持判定対象
        SKILL_ACTIVE,   //アクティブ（ノーマル）スキル所持判定対象
        SKILL_LIMITBREAK,   //リミットブレイク（アクティブ）スキル所持判定対象

        SKILL_TURN1,    //リミットブレイクスキルのターンの増減対象（死亡していても変化）
        SKILL_TURN2,    //リミットブレイクスキルのターンの増減対象（死亡していると変化しない）

        HATE,   //ヘイト増減対象
    }

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	パーティー情報セットアップ
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	パーティー情報セットアップ
	*/
    //----------------------------------------------------------------------------
    public bool PartySetup(CharaOnce[] acPartyChara, bool is_kobetsu_hp)
    {
        BattleParam.setKobetsuMode(is_kobetsu_hp);
        m_PlayerPartyChara = acPartyChara;
        //----------------------------------------
        // 情報構築前クリア
        //----------------------------------------
        {
            //			for( int i = 0;i < m_PartyTotalAtk.Length;i++ )
            //			{
            //				m_PartyTotalAtk[i] = 0;
            //			}
            //			m_HPBase = new BattleSceneUtil.MultiInt(0);
            //			m_HPMax = new BattleSceneUtil.MultiInt(0);
            m_HPCurrent = new BattleSceneUtil.MultiInt(GlobalDefine.VALUE_MAX_HP);
            m_Hate = new BattleSceneUtil.MultiInt(0);
            m_Hate_ProvokeTurn = new BattleSceneUtil.MultiInt(0);
            m_Hate_ProvokeOrder = new BattleSceneUtil.MultiInt(0);
            m_PartyTotalSP = 0;
            m_PartyTotalSPMax = 0;
            m_PartyTotalCost = 0;
            m_PartyTotalCharm = 0;
            m_DispHpDamageReg = new BattleSceneUtil.MultiInt(0);
            m_DispHpDamage = new BattleSceneUtil.MultiInt(0);
            m_DispHpDamageTargetReg = new BattleSceneUtil.MultiInt(0);
            m_DispHpDamageTarget = new BattleSceneUtil.MultiInt(0);
            m_DispHpRecoveryReg = new BattleSceneUtil.MultiInt(0);
            m_DispHpRecovery = new BattleSceneUtil.MultiInt(0);
            m_DispHp = new BattleSceneUtil.MultiInt(m_HPCurrent);
            m_DispHpWait = 0.0f;
        }

        //----------------------------------------
        // キャラ情報からパーティ情報を構築
        //----------------------------------------
        for (int i = 0; i < m_PlayerPartyChara.Length; i++)
        {
            if (m_PlayerPartyChara[i] == null)
                continue;
            if (m_PlayerPartyChara[i].m_CharaMasterDataParam == null)
                continue;

            m_PartyTotalCost += m_PlayerPartyChara[i].m_CharaMasterDataParam.party_cost;

            // リンクユニット分のコストを加算
            if (m_PlayerPartyChara[i].m_LinkParam.m_CharaID != 0)
            {
                m_PartyTotalCost += CharaLinkUtil.GetLinkUnitCost(m_PlayerPartyChara[i].m_LinkParam.m_CharaID);
            }

            // 魅力値加算
            m_PartyTotalCharm += CharaLimitOver.GetParamCharm((uint)m_PlayerPartyChara[i].m_CharaLimitOver, m_PlayerPartyChara[i].m_CharaMasterDataParam.limit_over_type);

        }
        m_PartyTotalSPMax = m_PartyTotalSP;

        calcPartyHP();

        //--------------------------------------------------------------------
        // InGameの遊びの検証用パターン実装
        //--------------------------------------------------------------------
        switch (ResidentParam.m_OptionSPRecovery)
        {
            case 0:
            case 1:
            default:
                m_PartyTotalSPMax = 20;
                m_PartyTotalSP = 10;
                break;

            case 2:
                m_PartyTotalSPMax = 14;
                m_PartyTotalSP = 14;
                break;
        }

        switch (ResidentParam.m_OptionSPPattern)
        {
            case 1:
                m_PartyTotalSPMax = 29;
                m_PartyTotalSP = 29;
                break;

            default:
                break;
        }

        m_PartySetupOK = true;

        m_Ailments = new BattleSceneUtil.MultiAilment();

        setupHero();

        return true;
    }

    public bool PartySetupMenu(CharaOnce[] acPartyChara, bool is_kobetsu_hp)
    {
        BattleParam.setKobetsuMode(is_kobetsu_hp);
        m_PlayerPartyChara = acPartyChara;
        //----------------------------------------
        // 情報構築前クリア
        //----------------------------------------
        {
            for (int i = 0; i < m_PartyTotalAtk.Length; i++)
            {
                m_PartyTotalAtk[i] = 0;
            }
            m_HPBase = new BattleSceneUtil.MultiInt(0);
            m_HPMax = new BattleSceneUtil.MultiInt(0);
            m_HPCurrent = new BattleSceneUtil.MultiInt(0);
            m_Hate = new BattleSceneUtil.MultiInt(0);
            m_Hate_ProvokeTurn = new BattleSceneUtil.MultiInt(0);
            m_Hate_ProvokeOrder = new BattleSceneUtil.MultiInt(0);
            m_PartyTotalSP = 0;
            m_PartyTotalSPMax = 0;
            m_PartyTotalCost = 0;
            m_PartyTotalCharm = 0;
            m_DispHpDamageReg = new BattleSceneUtil.MultiInt(0);
            m_DispHpDamage = new BattleSceneUtil.MultiInt(0);
            m_DispHpDamageTargetReg = new BattleSceneUtil.MultiInt(0);
            m_DispHpDamageTarget = new BattleSceneUtil.MultiInt(0);
            m_DispHpRecoveryReg = new BattleSceneUtil.MultiInt(0);
            m_DispHpRecovery = new BattleSceneUtil.MultiInt(0);
            m_DispHp = new BattleSceneUtil.MultiInt(m_HPCurrent);
            m_DispHpWait = 0.0f;
        }

        //----------------------------------------
        // キャラ情報からパーティ情報を構築
        //----------------------------------------
        for (int i = 0; i < m_PlayerPartyChara.Length; i++)
        {
            if (m_PlayerPartyChara[i] == null)
                continue;
            if (m_PlayerPartyChara[i].m_CharaMasterDataParam == null)
                continue;

            MasterDataDefineLabel.ElementType nElementType = m_PlayerPartyChara[i].m_CharaMasterDataParam.element;
            if (nElementType < MasterDataDefineLabel.ElementType.MAX)
            {
                m_PartyTotalAtk[(int)nElementType] += m_PlayerPartyChara[i].m_CharaPow;
            }
            else
            {
                Debug.LogError("Element Type Over!!! - " + m_PlayerPartyChara[i].m_CharaMasterDataParam.name);
            }

            //----------------------------------------------------------------
            // 体力総計に加算
            //----------------------------------------------------------------
            m_HPBase.addValue((GlobalDefine.PartyCharaIndex)i, m_PlayerPartyChara[i].m_CharaHP);
            m_PartyTotalCost += m_PlayerPartyChara[i].m_CharaMasterDataParam.party_cost;

            // リンクユニット分のコストを加算
            if (m_PlayerPartyChara[i].m_LinkParam.m_CharaID != 0)
            {
                m_PartyTotalCost += CharaLinkUtil.GetLinkUnitCost(m_PlayerPartyChara[i].m_LinkParam.m_CharaID);
            }

            // 魅力値加算
            m_PartyTotalCharm += m_PlayerPartyChara[i].m_CharaCharm;
        }
        m_HPMax.setValue(GlobalDefine.PartyCharaIndex.MAX, m_HPBase);
        m_HPCurrent.setValue(GlobalDefine.PartyCharaIndex.MAX, m_HPMax);
        m_DispHp.setValue(GlobalDefine.PartyCharaIndex.MAX, m_HPCurrent);
        m_PartyTotalSPMax = m_PartyTotalSP;
        m_PartySetupOK = true;

        m_Ailments = new BattleSceneUtil.MultiAilment();

        setupHero();

        return true;
    }

    public CharaOnce getPartyMember(GlobalDefine.PartyCharaIndex party_chara_index, CharaCondition chara_condition)
    {
        CharaOnce ret_val = null;
        if (party_chara_index >= GlobalDefine.PartyCharaIndex.LEADER && party_chara_index < GlobalDefine.PartyCharaIndex.MAX)
        {
            CharaOnce chara_once = m_PlayerPartyChara[(int)party_chara_index];
            if (chara_once != null && chara_once.m_CharaMasterDataParam != null)
            {
                int hp = m_HPCurrent.getValue(party_chara_index);
                switch (chara_condition)
                {
                    case CharaCondition.EXIST:
                        ret_val = chara_once;
                        break;

                    case CharaCondition.ALIVE:
                        if (hp > 0)
                        {
                            ret_val = chara_once;
                        }
                        break;

                    case CharaCondition.DEAD:
                        if (hp <= 0)
                        {
                            ret_val = chara_once;
                        }
                        break;

                    case CharaCondition.SKILL_LEADER:
                    case CharaCondition.SKILL_PASSIVE:
                    case CharaCondition.SKILL_TURN1:
                    case CharaCondition.HATE:
                        //死んでいてもスキルの効果あり
                        ret_val = chara_once;
                        break;

                    case CharaCondition.SKILL_ACTIVE:
                    case CharaCondition.SKILL_LIMITBREAK:
                    case CharaCondition.SKILL_TURN2:
                        //死んでいるとスキルの効果なし
                        if (hp > 0)
                        {
                            ret_val = chara_once;
                        }
                        break;
                }
            }
        }

        return ret_val;
    }

    // 指定条件に合ったパーティメンバーの配列を取得
    public CharaOnce[] getPartyMembers(CharaCondition chara_condition)
    {
        CharaOnce[] temp_array = new CharaOnce[getPartyMemberMaxCount()];
        int count = 0;
        for (int idx = 0; idx < temp_array.Length; idx++)
        {
            CharaOnce chara_once = getPartyMember((GlobalDefine.PartyCharaIndex)idx, chara_condition);
            if (chara_once != null)
            {
                temp_array[count] = chara_once;
                count++;
            }
        }

        if (count == temp_array.Length)
        {
            return temp_array;
        }

        CharaOnce[] ret_val = new CharaOnce[count];
        for (int idx = 0; idx < ret_val.Length; idx++)
        {
            ret_val[idx] = temp_array[idx];
        }
        return ret_val;
    }

    /// <summary>
    /// 汎用回復など発動者を取得
    /// </summary>
    /// <returns></returns>
    public GlobalDefine.PartyCharaIndex getGeneralPartyMember()
    {
        return m_GeneralPartyMember;
    }

    /// <summary>
    /// 汎用回復など発動者を更新
    /// </summary>
    public void updateGeneralPartyMember()
    {
        m_GeneralPartyMember = GlobalDefine.PartyCharaIndex.LEADER;
        for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
        {
            if (m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx) > 0)
            {
                m_GeneralPartyMember = (GlobalDefine.PartyCharaIndex)idx;
                break;
            }
        }
    }

    public void calcPartyHP()
    {
        for (int i = 0; i < m_PartyTotalAtk.Length; i++)
        {
            m_PartyTotalAtk[i] = 0;
        }
        m_HPBase = new BattleSceneUtil.MultiInt(0);
        m_HPMax = new BattleSceneUtil.MultiInt(0);

        //----------------------------------------
        // キャラ情報からパーティ情報を構築
        //----------------------------------------
        for (int i = 0; i < m_PlayerPartyChara.Length; i++)
        {
            CharaOnce chara_once = getPartyMember((GlobalDefine.PartyCharaIndex)i, CharaCondition.EXIST);
            if (chara_once == null)
            {
                continue;
            }

            MasterDataDefineLabel.ElementType nElementType = chara_once.m_CharaMasterDataParam.element;
            if (nElementType < MasterDataDefineLabel.ElementType.MAX)
            {
                m_PartyTotalAtk[(int)nElementType] += chara_once.m_CharaPow;
            }
            else
            {
                Debug.LogError("Element Type Over!!! - " + chara_once.m_CharaMasterDataParam.name);
            }

            float leaderskill_hp_rate = 1.0f;

            int debug_leader_hp_percent = 100;
            int debug_friend_hp_percent = 100;

            //----------------------------------------------------------------
            // リーダースキルによる体力増減
            //----------------------------------------------------------------
            if (getPartyMember(GlobalDefine.PartyCharaIndex.LEADER, CharaCondition.SKILL_LEADER) != null)
            {
                //				leaderskill_hp_rate *= GetLaeaderSkillHPRate( m_PlayerPartyChara[ GlobalDefine.PARTY_CHARA_LEADER ].m_CharaMasterDataParam.skill_leader, chara_once );
                leaderskill_hp_rate *= InGameUtilBattle.GetLeaderSkillHPRate(m_PlayerPartyChara, GlobalDefine.PartyCharaIndex.LEADER, chara_once);
                debug_leader_hp_percent = (int)(InGameUtilBattle.GetLeaderSkillHPRate(m_PlayerPartyChara, GlobalDefine.PartyCharaIndex.LEADER, chara_once) * 100.0f);
            }

            // フレンドは登録状況で発動するかどうかを判定
            if (getPartyMember(GlobalDefine.PartyCharaIndex.FRIEND, CharaCondition.SKILL_LEADER) != null)
            {
                //				leaderskill_hp_rate *= GetLaeaderSkillHPRate( m_PlayerPartyChara[ GlobalDefine.PARTY_CHARA_FRIEND ].m_CharaMasterDataParam.skill_leader, chara_once );
                leaderskill_hp_rate *= InGameUtilBattle.GetLeaderSkillHPRate(m_PlayerPartyChara, GlobalDefine.PartyCharaIndex.FRIEND, chara_once);
                debug_friend_hp_percent = (int)(InGameUtilBattle.GetLeaderSkillHPRate(m_PlayerPartyChara, GlobalDefine.PartyCharaIndex.FRIEND, chara_once) * 100.0f);
            }

            //----------------------------------------------------------------
            // 体力総計に加算
            //----------------------------------------------------------------
            m_HPBase.addValue((GlobalDefine.PartyCharaIndex)i, chara_once.m_CharaHP);
            m_HPMax.addValue((GlobalDefine.PartyCharaIndex)i, (int)(chara_once.m_CharaHP * leaderskill_hp_rate));

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "パーティ基本値計算:" + chara_once.m_CharaMasterDataParam.name);
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "  HP:" + m_HPMax.getValue((GlobalDefine.PartyCharaIndex)i).ToString()
                + " = " + chara_once.m_CharaHP.ToString() + "(Base)"
                + " x" + debug_leader_hp_percent.ToString() + "%(LeaderSkill)"
                + " x" + debug_friend_hp_percent.ToString() + "%(FriendSkill)"
            );
        }

        m_HPCurrent.minValue(GlobalDefine.PartyCharaIndex.MAX, m_HPMax);
        m_DispHp.setValue(GlobalDefine.PartyCharaIndex.MAX, m_HPCurrent);
    }

    /// <summary>
    /// ヒーローの情報を設定
    /// </summary>
    /// <param name="hero_level"></param>
    /// <param name="limit_break_skill_id"></param>
    public void setHero(int hero_level, uint limit_break_skill_id)
    {
        m_BattleHero = new BattleHero(hero_level, limit_break_skill_id);
    }

    private void setupHero()
    {
        // ヒーロースキルを設定
        int hero_level = 1;
        int hero_skill_id = 0;
        if (UserDataAdmin.HasInstance)
        {
            ServerDataDefine.PacketStructHero current_struct_hero = UserDataAdmin.Instance.getCurrentHero();
            if (current_struct_hero != null)
            {
                hero_level = current_struct_hero.level;
                hero_skill_id = current_struct_hero.current_skill_id;
            }
        }

        setHero(hero_level, (uint)hero_skill_id);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		危険モードか否か取得
		@retval		bool		[危険モード/通常モード]
	*/
    //----------------------------------------------------------------------------
    public bool ChkDangerMode()
    {
        if (m_HPCurrent != null && m_HPMax != null)
        {
            float fHPRate = ((float)m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MAX) / (float)m_HPMax.getValue(GlobalDefine.PartyCharaIndex.MAX));
            if (fHPRate < InGameDefine.CAUTION_DANGER_RATE)
            {
                return true;
            }
        }
        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	コンティニュー回復
	*/
    //----------------------------------------------------------------------------
    public void ContinueRecovery()
    {
        Resurrect(new BattleSceneUtil.MultiInt(1));
        RecoveryHP(m_HPMax, true, true);

        m_PartyTotalSP = m_PartyTotalSPMax;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	体力変動：回復
	*/
    //----------------------------------------------------------------------------
    public void RecoveryHP(BattleSceneUtil.MultiInt nHP, bool bDrawEnable, bool is_alive_only)
    {
        BattleSceneUtil.MultiInt before_hp = new BattleSceneUtil.MultiInt(m_HPCurrent); // 増減前のHP

        BattleSceneUtil.MultiInt recov_value = new BattleSceneUtil.MultiInt(nHP);
        if (is_alive_only)
        {
            BattleSceneUtil.MultiInt alive_member = new BattleSceneUtil.MultiInt(m_HPCurrent);
            alive_member.minValue(GlobalDefine.PartyCharaIndex.MAX, 1);

            recov_value.mulValue(GlobalDefine.PartyCharaIndex.MAX, alive_member);
        }

        m_HPCurrent.addValue(GlobalDefine.PartyCharaIndex.MAX, recov_value);
        m_HPCurrent.minValue(GlobalDefine.PartyCharaIndex.MAX, m_HPMax);
        m_Hate_1TurnHeal.addValue(GlobalDefine.PartyCharaIndex.MAX, recov_value);

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "プレイヤーHP回復"
            + " LEADER:" + recov_value.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString() + "(" + before_hp.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString() + "→" + m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString() + ")"
            + " MOB_1:" + recov_value.getValue(GlobalDefine.PartyCharaIndex.MOB_1).ToString() + "(" + before_hp.getValue(GlobalDefine.PartyCharaIndex.MOB_1).ToString() + "→" + m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MOB_1).ToString() + ")"
            + " MOB_2:" + recov_value.getValue(GlobalDefine.PartyCharaIndex.MOB_2).ToString() + "(" + before_hp.getValue(GlobalDefine.PartyCharaIndex.MOB_2).ToString() + "→" + m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MOB_2).ToString() + ")"
            + " MOB_3:" + recov_value.getValue(GlobalDefine.PartyCharaIndex.MOB_3).ToString() + "(" + before_hp.getValue(GlobalDefine.PartyCharaIndex.MOB_3).ToString() + "→" + m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MOB_3).ToString() + ")"
            + " FRIEND:" + recov_value.getValue(GlobalDefine.PartyCharaIndex.FRIEND).ToString() + "(" + before_hp.getValue(GlobalDefine.PartyCharaIndex.FRIEND).ToString() + "→" + m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.FRIEND).ToString() + ")"
        );

        //----------------------------------------
        // 回復量表示発行
        //----------------------------------------
        if (bDrawEnable)
        {
            //実際に回復した値ではなく回復しようとした値を表示
            m_DispHpRecoveryReg.addValue(GlobalDefine.PartyCharaIndex.MAX, recov_value);
        }
    }

    /// <summary>
    /// 蘇生（ＨＰ回復量はＨＰ最大値に対する割合）
    /// </summary>
    /// <param name="hp_percents">蘇生後のＨＰ割合、１％の時はＨＰ１で蘇生</param>
    private void Resurrect(BattleSceneUtil.MultiInt hp_percents)
    {
        if (BattleParam.IsKobetsuHP)
        {
            BattleSceneUtil.MultiInt recov_value = new BattleSceneUtil.MultiInt(0);

            for (int idx = 0; idx < recov_value.getMemberCount(); idx++)
            {
                int hp_percent = hp_percents.getValue((GlobalDefine.PartyCharaIndex)idx);
                if (hp_percent > 0)
                {
                    int current_hp = m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx);
                    int max_hp = m_HPMax.getValue((GlobalDefine.PartyCharaIndex)idx);
                    if (current_hp <= 0
                        && max_hp > 0
                    )
                    {
                        int hp = 1;
                        if (hp_percent > 0)
                        {
                            float hp_rate = InGameUtilBattle.GetDBRevisionValue(hp_percent);
                            hp = (int)InGameUtilBattle.AvoidErrorMultiple(max_hp, hp_rate);
                        }

                        if (hp < 1)
                        {
                            hp = 1;
                        }

                        recov_value.setValue((GlobalDefine.PartyCharaIndex)idx, hp);

                        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "パーティメンバー復活！！ " + ((GlobalDefine.PartyCharaIndex)idx).ToString() + "  HP:0→" + hp.ToString());
                    }
                }
            }
            m_HPCurrent.addValue(GlobalDefine.PartyCharaIndex.MAX, recov_value);
            m_DispHpRecoveryReg.addValue(GlobalDefine.PartyCharaIndex.MAX, recov_value);
            calcPartyHP();
        }
        else
        {
            m_HPCurrent.setValue(GlobalDefine.PartyCharaIndex.MAX, 1);
            m_DispHpRecoveryReg.addValue(GlobalDefine.PartyCharaIndex.MAX, 1);
        }

        // 復活したキャラにパーティ状態異常をコピー
        m_Ailments.updatePartyAilment();
    }

    /// <summary>
    /// パーティメンバー復活
    /// </summary>
    /// <param name="count">復活人数</param>
    /// <param name="rate_percent">復活確率％</param>
    /// <param name="hp_percent">復活時ＨＰ％</param>
    /// <param name="sp_use">復活時ＳＰ消費量</param>
    public void Resurrect(int count, int rate_percent, int hp_percent, int sp_use)
    {
        if (count > 0 && rate_percent > 0 && hp_percent > 0)
        {
            BattleSceneUtil.MultiInt resuc_hp_percent = new BattleSceneUtil.MultiInt(0);
            CharaOnce[] dead_party_menbers = getPartyMembers(CharaParty.CharaCondition.DEAD);
            int target_count = Mathf.Min(count, dead_party_menbers.Length);

            CharaOnce[] resurrection_targets = new CharaOnce[target_count];
            int dead_member_count = dead_party_menbers.Length;

            // 対象者をランダムな順番に並び替え
            for (int idx = 0; idx < target_count; idx++)
            {
                int member_index = (int)RandManager.GetRand(0, (uint)dead_member_count);
                resurrection_targets[idx] = dead_party_menbers[member_index];

                dead_member_count--;
                dead_party_menbers[member_index] = dead_party_menbers[dead_member_count];
            }

            for (int idx = 0; idx < resurrection_targets.Length; idx++)
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "復活判定 対象:" + resurrection_targets[idx].m_PartyCharaIndex.ToString()
                    + " 復活確率:" + rate_percent.ToString()
                    + " 消費SP:" + sp_use
                , false);
                int cur_sp = BattleParam.m_PlayerParty.GetSP();
                if (cur_sp < sp_use)
                {
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + " ==> SP不足により復活せず.");
                    break;
                }

                if (BattleSceneUtil.checkChancePercentSkill(rate_percent))
                {
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + " ==> 復活.");

                    CharaOnce chara_once = resurrection_targets[idx];
                    resuc_hp_percent.maxValue(chara_once.m_PartyCharaIndex, hp_percent);

                    BattleParam.m_PlayerParty.DamageSP(sp_use);
                }
                else
                {
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + " ==> 確率により復活せず.");
                }
            }

            Resurrect(resuc_hp_percent);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	体力変動：ダメージ
		@param[in]		int		(nHP)		ダメージ量
		@param[in]		bool	(nodead)	死なないフラグ
		@param[in]		bool	(disp)		ダメージ表示あり/なし
	*/
    //----------------------------------------------------------------------------
    public void DamageHP(BattleSceneUtil.MultiInt nHP, BattleSceneUtil.MultiInt damage_target, bool nodead, bool is_disp, MasterDataDefineLabel.ElementType disp_element_type)
    {
        BattleSceneUtil.MultiInt before_hp = new BattleSceneUtil.MultiInt(m_HPCurrent); // 増減前のHP

        BattleSceneUtil.MultiInt alive_member = new BattleSceneUtil.MultiInt(m_HPCurrent);
        alive_member.minValue(GlobalDefine.PartyCharaIndex.MAX, 1);

        BattleSceneUtil.MultiInt damage_value = new BattleSceneUtil.MultiInt(nHP);
        damage_value.mulValue(GlobalDefine.PartyCharaIndex.MAX, alive_member);
        damage_target.mulValue(GlobalDefine.PartyCharaIndex.MAX, alive_member);

        // 実績を集計
        for (int idx = 0; idx < damage_value.getMemberCount(); idx++)
        {
            int val = damage_value.getValue((GlobalDefine.PartyCharaIndex)idx);
            BattleParam.m_AchievementTotalingInBattle.damagePlayer(val);
        }

        m_HPCurrent.subValue(GlobalDefine.PartyCharaIndex.MAX, damage_value);
        m_HPCurrent.maxValue(GlobalDefine.PartyCharaIndex.MAX, 0);

        if (nodead)
        {
            m_HPCurrent.maxValue(GlobalDefine.PartyCharaIndex.MAX, alive_member);
        }

#if BUILD_TYPE_DEBUG
        if (BattleParam.IsKobetsuHP)
        {
            for (int idx = 0; idx < alive_member.getMemberCount(); idx++)
            {
                // 今死亡したか調べる
                if (alive_member.getValue((GlobalDefine.PartyCharaIndex)idx) > 0
                    && m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx) <= 0)
                {
                    if (m_DebugNoDeadFlag.getValue((GlobalDefine.PartyCharaIndex)idx) > 0)
                    {
                        // 不死なので復活
                        m_HPCurrent.setValue((GlobalDefine.PartyCharaIndex)idx, 1);
                    }
                }
            }
        }
#endif

        // 被ダメージヘイト
        if (BattleParam.m_BattleRequest.isEnableHate())
        {
            BattleSceneUtil.MultiInt hate_value = new BattleSceneUtil.MultiInt(damage_value);
            hate_value.minValue(GlobalDefine.PartyCharaIndex.MAX, 1);
            hate_value.mulValue(GlobalDefine.PartyCharaIndex.MAX, BattleParam.m_BattleRequest.getDamageHate());
            m_Hate.addValue(GlobalDefine.PartyCharaIndex.MAX, hate_value);
            adjustHate();
        }

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "プレイヤー被ダメ"
            + " LEADER:" + damage_value.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString() + "(" + before_hp.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString() + "→" + m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString() + ")"
            + " MOB_1:" + damage_value.getValue(GlobalDefine.PartyCharaIndex.MOB_1).ToString() + "(" + before_hp.getValue(GlobalDefine.PartyCharaIndex.MOB_1).ToString() + "→" + m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MOB_1).ToString() + ")"
            + " MOB_2:" + damage_value.getValue(GlobalDefine.PartyCharaIndex.MOB_2).ToString() + "(" + before_hp.getValue(GlobalDefine.PartyCharaIndex.MOB_2).ToString() + "→" + m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MOB_2).ToString() + ")"
            + " MOB_3:" + damage_value.getValue(GlobalDefine.PartyCharaIndex.MOB_3).ToString() + "(" + before_hp.getValue(GlobalDefine.PartyCharaIndex.MOB_3).ToString() + "→" + m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MOB_3).ToString() + ")"
            + " FRIEND:" + damage_value.getValue(GlobalDefine.PartyCharaIndex.FRIEND).ToString() + "(" + before_hp.getValue(GlobalDefine.PartyCharaIndex.FRIEND).ToString() + "→" + m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.FRIEND).ToString() + ")"
        );


        if (is_disp == true)
        {
            //----------------------------------------
            // テキトーにダメージ表示発行
            //----------------------------------------
            //実際に減った値ではなく減らそうとした値を表示
            m_DispHpDamageReg.addValue(GlobalDefine.PartyCharaIndex.MAX, damage_value);
            m_DispHpDamageTargetReg.addValue(GlobalDefine.PartyCharaIndex.MAX, damage_target);
            m_DispHpDamageRegElement = disp_element_type;
        }

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	体力変動：スタミナ
	*/
    //----------------------------------------------------------------------------
    public void RecoverySP(int nSP, bool disp, bool draw_value)
    {
        m_PartyTotalSP += nSP;
        if (m_PartyTotalSP > m_PartyTotalSPMax)
        {
            m_PartyTotalSP = m_PartyTotalSPMax;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	体力変動：スタミナ
	*/
    //----------------------------------------------------------------------------
    public void DamageSP(int nSP)
    {
        m_PartyTotalSP -= nSP;
        if (m_PartyTotalSP < 0)
        {
            m_PartyTotalSP = 0;
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ススメポイントの取得
	*/
    //----------------------------------------------------------------------------
    public int GetSP()
    {
        return m_PartyTotalSP;
    }

    /// <summary>
    /// 敵が攻撃する対象となるプレイヤーを選ぶ（ヘイト値・挑発スキルに基づく）
    /// </summary>
    /// <param name="is_random"></param>
    /// <returns></returns>
    public GlobalDefine.PartyCharaIndex selectTargetPlayer(bool is_random)
    {
#if BUILD_TYPE_DEBUG
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "パーティHATE値:" + m_Hate.getDebugString(GlobalDefine.PartyCharaIndex.MAX));
#endif
        if (BattleParam.IsKobetsuHP == false)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "(敵→プレイヤー)ターゲット選択：全員");
            return GlobalDefine.PartyCharaIndex.MAX;
        }

        CharaOnce[] alive_members = getPartyMembers(CharaCondition.ALIVE);  //生存メンバーだけが攻撃対象候補

        if (alive_members.Length <= 0)
        {
            // 全員死亡している場合はリーダーへ攻撃してみる.
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "(敵→プレイヤー)ターゲット選択：リーダー（全員死亡時)");
            return GlobalDefine.PartyCharaIndex.LEADER;
        }

        // ランダムターゲット選択
        if (is_random)
        {
            GlobalDefine.PartyCharaIndex ret_val = alive_members[RandManager.GetRand(0, (uint)alive_members.Length)].m_PartyCharaIndex;
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "(敵→プレイヤー)ターゲット選択：" + ret_val.ToString() + "(ランダム)");
            return ret_val;
        }

        // 挑発スキルによる選択
        {
            GlobalDefine.PartyCharaIndex provoke_target = _getProvokeTarget();
            if (provoke_target != GlobalDefine.PartyCharaIndex.MAX)
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "(敵→プレイヤー)ターゲット選択：" + provoke_target.ToString() + "(挑発スキル)");
                return provoke_target;
            }
        }

        // hate値に基づくターゲット選択
        {
            int max_hate = 0;
            for (int idx = 0; idx < alive_members.Length; idx++)
            {
                GlobalDefine.PartyCharaIndex chara_index = alive_members[idx].m_PartyCharaIndex;
                int wrk_hate = m_Hate.getValue(chara_index);
                if (wrk_hate > max_hate)
                {
                    max_hate = wrk_hate;
                }
            }

            int max_hate_count = 0;
            for (int idx = 0; idx < alive_members.Length; idx++)
            {
                GlobalDefine.PartyCharaIndex chara_index = alive_members[idx].m_PartyCharaIndex;
                int wrk_hate = m_Hate.getValue(chara_index);
                if (wrk_hate == max_hate)
                {
                    alive_members[max_hate_count] = alive_members[idx];
                    max_hate_count++;
                }
            }

            //　ヘイト最大値の中からランダムで選択
            GlobalDefine.PartyCharaIndex ret_val = alive_members[RandManager.GetRand(0, (uint)max_hate_count)].m_PartyCharaIndex;
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "(敵→プレイヤー)ターゲット選択：" + ret_val.ToString() + "(ヘイト値)");
            return ret_val;
        }
    }


    /// <summary>
    /// 表示HPを取得
    /// </summary>
    /// <returns></returns>
    public BattleSceneUtil.MultiInt getDispHP()
    {
        return m_DispHp;
    }

    /// <summary>
    /// 表示ダメージ量を取得
    /// </summary>
    /// <returns></returns>
    public BattleSceneUtil.MultiInt getDispDamageValue()
    {
        return m_DispHpDamage;
    }
    public BattleSceneUtil.MultiInt getDispDamageTarget()
    {
        return m_DispHpDamageTarget;
    }
    public MasterDataDefineLabel.ElementType getDispDamageElement()
    {
        return m_DispHpDamageRegElement;
    }

    /// <summary>
    /// 表示回復量を取得
    /// </summary>
    /// <returns></returns>
    public BattleSceneUtil.MultiInt getDispRecoveryValue()
    {
        return m_DispHpRecovery;
    }

    /// <summary>
    /// 表示回復量の表示までの待ち時間を設定
    /// </summary>
    /// <param name="wait_time"></param>
    public void setDispHpWait(float wait_time)
    {
        m_DispHpWait = wait_time;
        if (m_DispHpWait <= 0.0f)
        {
            m_DispHpWait = 1.0f / 120.0f;
        }
    }

    public void updateDispHp(float delta_time)
    {
        if (m_DispHpWait <= 0.0f)
        {
            m_DispHpDamage.setValue(GlobalDefine.PartyCharaIndex.MAX, m_DispHpDamageReg);
            m_DispHpDamageTarget.setValue(GlobalDefine.PartyCharaIndex.MAX, m_DispHpDamageTargetReg);
            m_DispHpDamageTarget.minValue(GlobalDefine.PartyCharaIndex.MAX, 1);

            m_DispHpDamageReg.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
            m_DispHpDamageTargetReg.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);

            m_DispHpRecovery.setValue(GlobalDefine.PartyCharaIndex.MAX, m_DispHpRecoveryReg);
            m_DispHpRecoveryReg.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);

            m_DispHp.setValue(GlobalDefine.PartyCharaIndex.MAX, m_HPCurrent);
        }
        else
        {
            m_DispHpWait -= delta_time;
            m_DispHpDamage.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
            m_DispHpRecovery.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
        }
    }

    /// <summary>
    /// ダメージや回復の数値を表示中かどうか
    /// </summary>
    /// <returns></returns>
    public bool isShowingDamageNumber()
    {
        if (DrawDamageManager.isShowing())
        {
            return true;
        }

        if (m_DispHpDamageTargetReg.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0
            || m_DispHpDamageTarget.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0
            || m_DispHpRecoveryReg.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0
            || m_DispHpRecovery.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0
        )
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// ヘイトを初期化
    /// </summary>
    /// <param name="is_chane_battle">継続バトルかどうか（以前のヘイトを引き継ぐかどうか）</param>
    public void initHate(bool is_chain_battle)
    {
        if (BattleParam.IsKobetsuHP)
        {
            if (is_chain_battle == false)
            {
                m_Hate.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
            }

            for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
            {
                CharaOnce chara_once = getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaCondition.HATE);
                if (chara_once != null)
                {
                    int initial_value = BattleParam.m_BattleRequest.getInitialHate(chara_once.m_CharaMasterDataParam.element, chara_once.kind, chara_once.kind_sub);
                    addHate((GlobalDefine.PartyCharaIndex)idx, initial_value);
                }
            }

            adjustHate();

            // ヘイト判定パラメータを初期化
            BattleParam.m_PlayerParty.m_Hate_1TurnDamage.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
            BattleParam.m_PlayerParty.m_Hate_1TurnHeal.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
        }
    }

    /// <summary>
    /// ヘイト情報を更新
    /// </summary>
    public void updateHate()
    {
        if (BattleParam.IsKobetsuHP)
        {
            int damage_rank = 1;
            while (damage_rank < (int)GlobalDefine.PartyCharaIndex.MAX)
            {
                int damage_max = 0;
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    int wrk_damage = m_Hate_1TurnDamage.getValue((GlobalDefine.PartyCharaIndex)idx);

                    if (wrk_damage > damage_max)
                    {
                        damage_max = wrk_damage;
                    }
                }

                if (damage_max <= 0)
                {
                    break;
                }

                int wrk_rank = damage_rank;

                bool is_update = false;
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    CharaOnce chara_once = getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaCondition.HATE);
                    if (chara_once != null)
                    {
                        int wrk_damage = m_Hate_1TurnDamage.getValue((GlobalDefine.PartyCharaIndex)idx);
                        if (wrk_damage == damage_max)
                        {
                            int hate_value = BattleParam.m_BattleRequest.getGivenDamageHate(chara_once.m_CharaMasterDataParam.element, chara_once.m_CharaMasterDataParam.kind, chara_once.m_CharaMasterDataParam.sub_kind, wrk_rank);
                            addHate((GlobalDefine.PartyCharaIndex)idx, hate_value);
                            m_Hate_1TurnDamage.setValue((GlobalDefine.PartyCharaIndex)idx, 0);
                            damage_rank++;
                            is_update = true;
                        }
                    }
                }

                if (is_update == false)
                {
                    break;
                }
            }

            int heal_rank = 1;
            while (heal_rank < (int)GlobalDefine.PartyCharaIndex.MAX)
            {
                int heal_max = 0;
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    int wrk_heal = m_Hate_1TurnHeal.getValue((GlobalDefine.PartyCharaIndex)idx);

                    if (wrk_heal > heal_max)
                    {
                        heal_max = wrk_heal;
                    }
                }

                if (heal_max <= 0)
                {
                    break;
                }

                int wrk_rank = heal_rank;

                bool is_update = false;
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    CharaOnce chara_once = getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaCondition.HATE);
                    if (chara_once != null)
                    {
                        int wrk_heal = m_Hate_1TurnHeal.getValue((GlobalDefine.PartyCharaIndex)idx);

                        if (wrk_heal == heal_max)
                        {
                            int hate_value = BattleParam.m_BattleRequest.getHealHate(chara_once.m_CharaMasterDataParam.element, chara_once.m_CharaMasterDataParam.kind, chara_once.m_CharaMasterDataParam.sub_kind, wrk_rank);
                            addHate((GlobalDefine.PartyCharaIndex)idx, hate_value);
                            m_Hate_1TurnHeal.setValue((GlobalDefine.PartyCharaIndex)idx, 0);
                            heal_rank++;
                            is_update = true;
                        }
                    }
                }

                if (is_update == false)
                {
                    break;
                }
            }

            adjustHate();

            // ヘイト判定パラメータを初期化
            BattleParam.m_PlayerParty.m_Hate_1TurnDamage.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
            BattleParam.m_PlayerParty.m_Hate_1TurnHeal.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
        }
    }

    public void addHate(GlobalDefine.PartyCharaIndex member_index, int hate_value)
    {
        m_Hate.addValue(member_index, hate_value);
    }

    /// <summary>
    /// ヘイト値を適正な範囲に補正
    /// </summary>
    public void adjustHate()
    {
        const int HATE_MAX = 1000000;

        // 最小値がゼロになるように補正
        int min_value = HATE_MAX * 2;
        for (int idx = 0; idx < m_Hate.getMemberCount(); idx++)
        {
            int val = m_Hate.getValue((GlobalDefine.PartyCharaIndex)idx);
            if (val < min_value)
            {
                min_value = val;
            }
        }
        m_Hate.addValue(GlobalDefine.PartyCharaIndex.MAX, -min_value);

        // ヘイト値の上限
        for (int loop_count = 0; loop_count < 100; loop_count++)
        {
            int max_value = 0;
            for (int idx = 0; idx < m_Hate.getMemberCount(); idx++)
            {
                int val = m_Hate.getValue((GlobalDefine.PartyCharaIndex)idx);
                if (val > max_value)
                {
                    max_value = val;
                }
            }

            if (max_value > HATE_MAX)
            {
                m_Hate.mulValueF(GlobalDefine.PartyCharaIndex.MAX, 0.9f);
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 挑発スキル効果設定
    /// </summary>
    /// <param name="member_index"></param>
    /// <param name="provoke_turn">挑発継続ターン数</param>
    public void provoke(GlobalDefine.PartyCharaIndex member_index, int provoke_turn)
    {
        if (provoke_turn > 0)
        {
            int max_order = 0;
            for (int idx = 0; idx < m_Hate_ProvokeOrder.getMemberCount(); idx++)
            {
                int order = m_Hate_ProvokeOrder.getValue((GlobalDefine.PartyCharaIndex)idx);
                if (order > max_order)
                {
                    max_order = order;
                }
            }

            m_Hate_ProvokeTurn.setValue(member_index, provoke_turn);
            m_Hate_ProvokeOrder.setValue(member_index, max_order + 1);
        }
        else
        {
            m_Hate_ProvokeTurn.setValue(member_index, 0);
            m_Hate_ProvokeOrder.setValue(member_index, 0);
        }
    }

    public void updateProvokeTurn()
    {
        // 挑発スキルを更新
        if (BattleParam.IsKobetsuHP)
        {
            // ターン数を１消化
            m_Hate_ProvokeTurn.subValue(GlobalDefine.PartyCharaIndex.MAX, 1);
            m_Hate_ProvokeTurn.maxValue(GlobalDefine.PartyCharaIndex.MAX, 0);    //マイナスにならないように

            // 死亡しているメンバーのターン数をゼロクリア
            {
                BattleSceneUtil.MultiInt mask = new BattleSceneUtil.MultiInt(m_HPCurrent);
                mask.minValue(GlobalDefine.PartyCharaIndex.MAX, 1);
                m_Hate_ProvokeTurn.mulValue(GlobalDefine.PartyCharaIndex.MAX, mask);
            }

            // ターン数ゼロになったところはオーダーをゼロクリア
            {
                BattleSceneUtil.MultiInt mask = new BattleSceneUtil.MultiInt(m_Hate_ProvokeTurn);
                mask.minValue(GlobalDefine.PartyCharaIndex.MAX, 1);
                m_Hate_ProvokeOrder.mulValue(GlobalDefine.PartyCharaIndex.MAX, mask);
            }
        }
    }

    /// <summary>
    /// 挑発スキルによる現在のターゲット
    /// </summary>
    public GlobalDefine.PartyCharaIndex _getProvokeTarget()
    {
        GlobalDefine.PartyCharaIndex ret_val = GlobalDefine.PartyCharaIndex.MAX;
        if (BattleParam.IsKobetsuHP)
        {
            CharaOnce[] alive_members = getPartyMembers(CharaCondition.ALIVE);  //生存メンバーだけが攻撃対象候補
            if (alive_members.Length > 0)
            {
                // 有効な挑発を抽出（最後に発動した挑発が有効）
                int max_order = 0;
                for (int idx = 0; idx < alive_members.Length; idx++)
                {
                    GlobalDefine.PartyCharaIndex wrk_member_idx = alive_members[idx].m_PartyCharaIndex;
                    int order = m_Hate_ProvokeOrder.getValue(wrk_member_idx);
                    if (order > max_order)
                    {
                        max_order = order;
                        ret_val = (GlobalDefine.PartyCharaIndex)idx;
                    }
                }
            }
        }

        return ret_val;
    }

    /// <summary>
    /// 死亡しているメンバーの状態異常を解除
    /// </summary>
    public void clearAilmentDeadMember()
    {
        if (BattleParam.IsKobetsuHP)
        {
            CharaOnce[] dead_members = getPartyMembers(CharaParty.CharaCondition.DEAD);
            for (int idx = 0; idx < dead_members.Length; idx++)
            {
                GlobalDefine.PartyCharaIndex chara_index = dead_members[idx].m_PartyCharaIndex;
                m_Ailments.DelAllStatusAilment(chara_index);

                m_Hate_ProvokeTurn.setValue(chara_index, 0);
                m_Hate_ProvokeOrder.setValue(chara_index, 0);
            }
        }
    }

}

