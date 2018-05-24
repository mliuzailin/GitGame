/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	CharaOnce.cs
	@brief	キャラ単体
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
	@brief	キャラ単体
*/
//----------------------------------------------------------------------------
[Serializable]
public class CharaOnce
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public MasterDataParamChara m_CharaMasterDataParam = null;      //!< キャラ基本情報：
    public bool m_bHasCharaMasterDataParam = false; //!< 非nullキャッシュ：m_CharaMasterDataParam。

    public int m_CharaLevel;                    //!< キャラステータス：キャラレベル
    public int m_CharaLBSLv;                    //!< キャラステータス：LBSレベル
    public int m_CharaPow;                      //!< キャラステータス：攻撃
    public int m_CharaDef;                      //!< キャラステータス：防御
    public int m_CharaHP;                       //!< キャラステータス：体力
    public int m_CharaPlusPow;                  //!< キャラステータス：プラス値：攻撃力
    public int m_CharaPlusDef;                  //!< キャラステータス：プラス値：防御力
    public int m_CharaPlusHP;                   //!< キャラステータス：プラス値：体力
    public int m_CharaLimitBreak;               //!< キャラステータス：リミットブレイクスキル
    public int m_CharaLimitOver;                //!< キャラステータス：限界突破
    public double m_CharaCharm;                 //!< キャラステータス：魅力

    // @add Developer 2015/09/03 ver300
    public CharaLinkParam m_LinkParam = new CharaLinkParam();   //!< キャラステータス：リンク情報

    public GlobalDefine.PartyCharaIndex m_PartyCharaIndex;	//!< CharaParty内での順番

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    public MasterDataDefineLabel.KindType kind
    {
        get
        {
            if (!m_bHasCharaMasterDataParam)
            {
                return MasterDataDefineLabel.KindType.NONE;
            }

            return m_CharaMasterDataParam.kind;
        }
    }


    public MasterDataDefineLabel.KindType kind_sub
    {
        get
        {
            if (!m_bHasCharaMasterDataParam)
            {
                return MasterDataDefineLabel.KindType.NONE;
            }

            return m_CharaMasterDataParam.sub_kind;
        }
    }



    //----------------------------------------------------------------------------
    /*!
		@brief	キャラ情報セットアップ：ID指定
		@param	uint	unCharaID		キャラID
		@param	int		nLevel			キャラレベル
		@param	int		lbsLv			リミットブレイクスキルレベル
		@param	int		nLoLevel		限界突破レベル
		@param	int		nPlusPow		プラス値：攻撃
		@param	int		nPlusHP			プラス値：体力
		@param	uint	unLinkID		リンク：キャラID
		@param	int		nLinkLv			リンク：キャラレベル
		@param	int		nLinkPlusPow	リンク：プラス値：攻撃
		@param	int		nLinkPlusHP		リンク：プラス値：体力
		@param	int		nLinkPoint		リンク：ポイント
		@param	int		nLinkLoLevel	リンク：限界突破レベル
	*/
    //----------------------------------------------------------------------------
    public bool CharaSetupFromID(uint unCharaID, int nLevel, int lbsLv, int nLoLevel, int nPlusPow, int nPlusHP,
                                  uint unLinkID, int nLinkLv, int nLinkPlusPow, int nLinkPlusHP, int nLinkPoint, int nLinkLoLevel)
    {
        MasterDataParamChara cCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(unCharaID);
        if (cCharaMaster == null)
        {
            Debug.LogError("CharaSetup Error! - " + unCharaID);
            return false;
        }

        // リンク中の場合
        if (unLinkID != 0)
        {
            m_LinkParam.Setup(unLinkID, nLinkLv, nLinkPlusPow, nLinkPlusHP, nLinkPoint, nLinkLoLevel);
        }
        else
        {
            m_LinkParam.Setup();
        }

        return CharaSetupFromParam(cCharaMaster, nLevel, lbsLv, nPlusPow, nPlusHP, nLoLevel);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キャラ情報セットアップ：ID指定
		@param	uint	unCharaID		キャラID
		@param	int		nLevel			キャラレベル
	*/
    //----------------------------------------------------------------------------
    public bool CharaSetupFromID(uint unCharaID, int nLevel)
    {
        MasterDataParamChara cCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(unCharaID);
        if (cCharaMaster == null)
        {
            Debug.LogError("CharaSetup Error! - " + unCharaID);
            return false;
        }
        return CharaSetupFromParam(cCharaMaster, nLevel);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キャラ情報セットアップ：マスターデータ指定
		@param	MasterDataParamChara	cCharaMaster	キャラマスター
		@param	int						nLevel			キャラレベル
		@param	int						lbsLv			リミットブレイクスキルレベル
		@param	int						nLoLevel		限界突破レベル
		@param	int						nPlusPow		プラス値：攻撃
		@param	int						nPlusHP			プラス値：体力
		@param	uint					unLinkID		リンク：キャラID
		@param	int						nLinkLv			リンク：キャラレベル
		@param	int						nLinkPlusPow	リンク：プラス値：攻撃
		@param	int						nLinkPlusHP		リンク：プラス値：体力
		@param	int						nLinkPoint		リンク：ポイント
		@param	int						nLinkLoLevel	リンク：限界突破レベル
	*/
    //----------------------------------------------------------------------------
    public bool CharaSetupFromParam(MasterDataParamChara cCharaMaster, int nLevel, int lbsLv, int nLoLevel, int nPlusPow, int nPlusHP,
                                     uint unLinkID, int nLinkLv, int nLinkPlusPow, int nLinkPlusHP, int nLinkPoint, int nLinkLoLevel)
    {

        // リンク中の場合
        if (unLinkID != 0)
        {
            m_LinkParam.Setup(unLinkID, nLinkLv, nLinkPlusPow, nLinkPlusHP, nLinkPoint, nLinkLoLevel);
        }
        else
        {
            m_LinkParam.Setup();
        }

        return CharaSetupFromParam(cCharaMaster, nLevel, lbsLv, nPlusPow, nPlusHP, nLoLevel);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キャラ情報セットアップ：マスターデータ指定
		@param	MasterDataParamChara	cMasterData		キャラマスター
		@param	int						nLevel			キャラレベル
	*/
    //----------------------------------------------------------------------------
    public bool CharaSetupFromParam(MasterDataParamChara cMasterData, int nLevel)
    {
        m_CharaMasterDataParam = cMasterData;
        m_bHasCharaMasterDataParam = (null != m_CharaMasterDataParam);

        if (m_CharaMasterDataParam == null)
        {
            Debug.LogError("CharaSetup Error! - InstanceNone!! ");
            return false;
        }

        m_CharaLevel = nLevel;
        m_CharaLBSLv = 0;
        m_CharaPow = CharaUtil.GetStatusValue(m_CharaMasterDataParam, nLevel, CharaUtil.VALUE.POW);
        m_CharaDef = CharaUtil.GetStatusValue(m_CharaMasterDataParam, nLevel, CharaUtil.VALUE.DEF);
        m_CharaHP = CharaUtil.GetStatusValue(m_CharaMasterDataParam, nLevel, CharaUtil.VALUE.HP);
        m_CharaLimitBreak = 0;
        m_CharaPlusPow = 0;
        m_CharaPlusDef = 0;
        m_CharaPlusHP = 0;

        // @add Developer 2015/09/03 ver300
        // リンク用変数の初期化
        m_LinkParam.m_CharaID = 0;
        m_LinkParam.m_CharaLv = 0;
        m_LinkParam.m_CharaPlusPow = 0;
        m_LinkParam.m_CharaPlusHP = 0;
        m_LinkParam.m_CharaLinkPoint = 0;

        if (m_CharaPow > GlobalDefine.VALUE_MAX_POW)
        {
            m_CharaPow = GlobalDefine.VALUE_MAX_POW;
        }

        if (m_CharaHP > GlobalDefine.VALUE_MAX_HP)
        {
            m_CharaHP = GlobalDefine.VALUE_MAX_HP;
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キャラ情報セットアップ：マスターデータ指定
		@param	MasterDataParamChara	cMasterData		キャラマスター
		@param	int						nLevel			キャラレベル
		@param	int						lbsLv			リミットブレイクスキルレベル
		@param	int						nPlusPow		プラス値：攻撃
		@param	int						nPlusHP			プラス値：体力
		@param	int						nLOLevel		プラス値：限界突破値
		@note
	*/
    //----------------------------------------------------------------------------
    public bool CharaSetupFromParam(MasterDataParamChara cMasterData, int nLevel, int lbsLv, int nPlusPow, int nPlusHP, int nLOLevel)
    {
        m_CharaMasterDataParam = cMasterData;
        m_bHasCharaMasterDataParam = (null != m_CharaMasterDataParam);

        if (m_CharaMasterDataParam == null)
        {
            Debug.LogError("CharaSetup Error! - InstanceNone!! ");
            return false;
        }

        // @change Developer 2015/09/03 ver300
        #region ==== 通常処理 ====
        int nPlusValuePow = 0;
        int nPlusValueHP = 0;

        m_CharaLevel = nLevel;
        m_CharaLBSLv = lbsLv;

        float fLimitOverHP = 0;
        float fLimitOverATK = 0;

        #region ==== スキルレベルまるめ処理 ====
        MasterDataSkillLimitBreak cSkillLimitBreak = BattleParam.m_MasterDataCache.useSkillLimitBreak(cMasterData.skill_limitbreak);
        if (cSkillLimitBreak != null)
        {
            if (lbsLv > cSkillLimitBreak.level_max)
            {
                m_CharaLBSLv = cSkillLimitBreak.level_max;
            }
        }
        #endregion

        #region ==== リミットオーバーまるめ処理 ====
        MasterDataLimitOver _masterMainLO = MasterFinder<MasterDataLimitOver>.Instance.Find((int)cMasterData.limit_over_type);
        if (_masterMainLO != null)
        {
            if (nLOLevel > _masterMainLO.limit_over_max)
            {
                nLOLevel = _masterMainLO.limit_over_max;
            }
        }
        #endregion

        m_CharaPow = CharaUtil.GetStatusValue(m_CharaMasterDataParam, nLevel, CharaUtil.VALUE.POW);
        m_CharaDef = CharaUtil.GetStatusValue(m_CharaMasterDataParam, nLevel, CharaUtil.VALUE.DEF);
        m_CharaHP = CharaUtil.GetStatusValue(m_CharaMasterDataParam, nLevel, CharaUtil.VALUE.HP);
        m_CharaLimitBreak = 0;
        m_CharaPlusPow = nPlusPow;
        m_CharaPlusDef = 0;
        m_CharaPlusHP = nPlusHP;
        m_CharaLimitOver = nLOLevel;
        m_CharaCharm = CharaLimitOver.GetParamCharm((uint)nLOLevel, cMasterData.limit_over_type);
        // レベルMAXなら限界突破の値を追加

        float fLimitOverAddHp = CharaLimitOver.GetParam((uint)nLOLevel, cMasterData.limit_over_type, (int)CharaLimitOver.EGET.ePARAM_HP);
        float fLimitOverAddAtk = CharaLimitOver.GetParam((uint)nLOLevel, cMasterData.limit_over_type, (int)CharaLimitOver.EGET.ePARAM_ATK);


        fLimitOverHP = m_CharaHP * (fLimitOverAddHp / 100);
        fLimitOverATK = m_CharaPow * (fLimitOverAddAtk / 100);

        // プラス値を算出
        nPlusValuePow = m_CharaPlusPow * GlobalDefine.PLUS_RATE_POW;
        nPlusValueHP = m_CharaPlusHP * GlobalDefine.PLUS_RATE_HP;
        #endregion

        // @add Developer 2015/09/03 ver300
        #region ==== リンクシステム処理 ====
        int nElemValuePow = 0;
        int nElemValueHP = 0;
        int nRaceValuePow = 0;
        int nRaceValueHP = 0;
        int nLinkPlusValuePow = 0;
        int nLinkPlusValueHP = 0;
        double nLinkCharm = 0;
        // リンク中の場合
        //MasterDataParamChara cLinkCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(m_LinkParam.m_CharaID);
        MasterDataParamChara cLinkCharaMaster = m_LinkParam.m_cCharaMasterDataParam;
        if (cLinkCharaMaster != null)
        {
            float fWork = 0.0f;

            // 属性ボーナスを加算
            nElemValuePow = CharaLinkUtil.GetLinkUnitBonusElement(cLinkCharaMaster, m_LinkParam.m_CharaLv, (uint)m_LinkParam.m_CharaLOLevel, CharaUtil.VALUE.POW);
            nElemValueHP = CharaLinkUtil.GetLinkUnitBonusElement(cLinkCharaMaster, m_LinkParam.m_CharaLv, (uint)m_LinkParam.m_CharaLOLevel, CharaUtil.VALUE.HP);

            // 種族ボーナス：攻撃力の+%値を算出
            fWork = CharaLinkUtil.GetLinkUnitBonusRace(cLinkCharaMaster, cLinkCharaMaster.kind, CharaUtil.VALUE.POW);           // %値取得(メイン)
            fWork += CharaLinkUtil.GetLinkUnitBonusRace(cLinkCharaMaster, cLinkCharaMaster.sub_kind, CharaUtil.VALUE.POW);          // %値取得(サブ)
            fWork = InGameUtilBattle.GetDBRevisionValue(fWork);                                                                 // 数値変換
            nRaceValuePow = (int)InGameUtilBattle.AvoidErrorMultiple((float)m_CharaPow, fWork);                                         // 増加量

            // 種族ボーナス：体力の実値を取得
            nRaceValueHP = (int)CharaLinkUtil.GetLinkUnitBonusRace(cLinkCharaMaster, cLinkCharaMaster.kind, CharaUtil.VALUE.HP);        // 実値取得(メイン)
            nRaceValueHP += (int)CharaLinkUtil.GetLinkUnitBonusRace(cLinkCharaMaster, cLinkCharaMaster.sub_kind, CharaUtil.VALUE.HP);       // 実値取得(サブ)

            // +値の算出
            nLinkPlusValuePow = CharaLinkUtil.GetLinkUnitBonusPlus(m_LinkParam.m_CharaPlusPow, CharaUtil.VALUE.POW);
            nLinkPlusValueHP = CharaLinkUtil.GetLinkUnitBonusPlus(m_LinkParam.m_CharaPlusHP, CharaUtil.VALUE.HP);
            nLinkCharm = CharaLimitOver.GetParamCharm((uint)m_LinkParam.m_CharaLOLevel, cLinkCharaMaster.limit_over_type);
        }
        else
        {
            // リンク用変数の初期化
            m_LinkParam.Setup();
        }
        #endregion

#if BUILD_TYPE_DEBUG
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "キャラ基本情報"
            + "[" + cMasterData.name + "]"
            + " FixID:" + cMasterData.fix_id
            + " DrawID:" + cMasterData.draw_id
            + " LV:" + m_CharaLevel.ToString()
            + " SkillLV:" + m_CharaLBSLv.ToString()
            + " LimOverLv:" + m_CharaLimitOver.ToString()
            + " PlusPow:" + m_CharaPlusPow.ToString()
            + " PlusHp:" + m_CharaPlusHP.ToString()
            + " 属性:" + cMasterData.element.ToString()
            + " 種族1:" + cMasterData.kind.ToString()
            + " 種族2:" + cMasterData.sub_kind.ToString()
        );

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "  HP(" + (m_CharaHP + nPlusValueHP + nElemValueHP + nRaceValueHP + nLinkPlusValueHP + (int)fLimitOverHP).ToString() + ")"
            + " = LV(" + m_CharaHP.ToString() + ")"
            + " + PlusHP(" + nPlusValueHP.ToString() + ")"
            + " + LinkElem(" + nElemValueHP.ToString() + ")"
            + " + LinkRace(" + nRaceValueHP.ToString() + ")"
            + " + LinkPlus(" + nLinkPlusValueHP.ToString() + ")"
            + " + LimitOver(" + ((int)fLimitOverHP).ToString() + ")"
            );
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "  POW(" + (m_CharaPow + nPlusValuePow + nElemValuePow + nRaceValuePow + nLinkPlusValuePow + (int)fLimitOverATK).ToString() + ")"
            + " = LV(" + m_CharaPow.ToString() + ")"
            + " + PlusPow(" + nPlusValuePow.ToString() + ")"
            + " + LinkElem(" + nElemValuePow.ToString() + ")"
            + " + LinkRace(" + nRaceValuePow.ToString() + ")"
            + " + LinkPlus(" + nLinkPlusValuePow.ToString() + ")"
            + " + LimitOver(" + ((int)fLimitOverATK).ToString() + ")"
            );

        const int ADD_SKILL_COUNT_LIMIT = 50;	// スキル連結数の上限

        if (cMasterData.skill_leader != 0)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　リーダースキル(fixid:" + cMasterData.skill_leader.ToString(), false);
            MasterDataSkillLeader master_data = BattleParam.m_MasterDataCache.useSkillLeader(cMasterData.skill_leader);
            if (master_data == null)
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（存在しないfix_id）)");
            }
            else
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + ")[" + master_data.name + "]" + master_data.detail);
            }
        }

        if (cMasterData.skill_limitbreak != 0)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　リミブレスキル(fixid:" + cMasterData.skill_limitbreak.ToString(), false);
            MasterDataSkillLimitBreak master_data = BattleParam.m_MasterDataCache.useSkillLimitBreak(cMasterData.skill_limitbreak);
            if (master_data == null)
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（存在しないfix_id）)");
            }
            else
            {
                int loop_limit = ADD_SKILL_COUNT_LIMIT;
                int add_fix_id = master_data.add_fix_id;
                while (add_fix_id != 0)
                {
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "+" + add_fix_id.ToString(), false);
                    MasterDataSkillLimitBreak add_master_data = BattleParam.m_MasterDataCache.useSkillLimitBreak((uint)add_fix_id);
                    if (add_master_data == null)
                    {
                        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（存在しないfix_id）)");
                        break;
                    }

                    add_fix_id = add_master_data.add_fix_id;
                    loop_limit--;
                    if (loop_limit <= 0)
                    {
                        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（無限ループ）)");
                    }
                }

                DebugBattleLog.writeText(DebugBattleLog.StrOpe + ")[" + master_data.name + "]" + master_data.detail);
            }
        }

        if (cMasterData.skill_active0 != 0)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　ノーマルスキル1(fixid:" + cMasterData.skill_active0.ToString(), false);
            MasterDataSkillActive master_data = BattleParam.m_MasterDataCache.useSkillActive(cMasterData.skill_active0);
            if (master_data == null)
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（存在しないfix_id）)");
            }
            else
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + ")[" + master_data.name + "]" + master_data.detail);
            }
        }

        if (cMasterData.skill_active1 != 0)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　ノーマルスキル2(fixid:" + cMasterData.skill_active1.ToString(), false);
            MasterDataSkillActive master_data = BattleParam.m_MasterDataCache.useSkillActive(cMasterData.skill_active1);
            if (master_data == null)
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（存在しないfix_id）)");
            }
            else
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + ")[" + master_data.name + "]" + master_data.detail);
            }
        }

        if (cMasterData.skill_passive != 0)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　パッシブスキル(fixid:" + cMasterData.skill_passive.ToString(), false);
            MasterDataSkillPassive master_data = BattleParam.m_MasterDataCache.useSkillPassive(cMasterData.skill_passive);
            if (master_data == null)
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（存在しないfix_id）)");
            }
            else
            {
                int loop_limit = ADD_SKILL_COUNT_LIMIT;
                int add_fix_id = master_data.add_fix_id;
                while (add_fix_id != 0)
                {
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "+" + add_fix_id.ToString(), false);
                    MasterDataSkillPassive add_master_data = BattleParam.m_MasterDataCache.useSkillPassive((uint)add_fix_id);
                    if (add_master_data == null)
                    {
                        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（存在しないfix_id）)");
                        break;
                    }

                    add_fix_id = add_master_data.add_fix_id;
                    loop_limit--;
                    if (loop_limit <= 0)
                    {
                        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（無限ループ）)");
                    }
                }

                DebugBattleLog.writeText(DebugBattleLog.StrOpe + ")[" + master_data.name + "]" + master_data.detail);
            }
        }

        if (cLinkCharaMaster != null)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　リンクキャラ"
                + "[" + cLinkCharaMaster.name + "]"
                + " FixID:" + cLinkCharaMaster.fix_id
                + " DrawID:" + cLinkCharaMaster.draw_id
                + " LV:" + m_LinkParam.m_CharaLv.ToString()
                + " LimOverLv:" + m_LinkParam.m_CharaLOLevel.ToString()
                + " PlusPow:" + m_LinkParam.m_CharaPlusPow.ToString()
                + " PlusHp:" + m_LinkParam.m_CharaPlusHP.ToString()
                + " LinkPoint:" + m_LinkParam.m_CharaLinkPoint.ToString()
            );

            if (cLinkCharaMaster.link_skill_active != 0)
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　　リンクスキル(fixid:" + cLinkCharaMaster.link_skill_active.ToString(), false);
                MasterDataSkillActive master_data = BattleParam.m_MasterDataCache.useSkillActive(cLinkCharaMaster.link_skill_active);
                if (master_data == null)
                {
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（存在しないfix_id）)");
                }
                else
                {
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + ")[" + master_data.name + "]" + master_data.detail);
                }
            }

            if (cLinkCharaMaster.link_skill_passive != 0)
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　　リンクパッシブスキル(fixid:" + cLinkCharaMaster.link_skill_passive.ToString(), false);
                MasterDataSkillPassive master_data = BattleParam.m_MasterDataCache.useSkillPassive(cLinkCharaMaster.link_skill_passive);
                if (master_data == null)
                {
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（存在しないfix_id）)");
                }
                else
                {
                    int loop_limit = ADD_SKILL_COUNT_LIMIT;
                    int add_fix_id = master_data.add_fix_id;
                    while (add_fix_id != 0)
                    {
                        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "+" + add_fix_id.ToString(), false);
                        MasterDataSkillPassive add_master_data = BattleParam.m_MasterDataCache.useSkillPassive((uint)add_fix_id);
                        if (add_master_data == null)
                        {
                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（存在しないfix_id）)");
                            break;
                        }

                        add_fix_id = add_master_data.add_fix_id;
                        loop_limit--;
                        if (loop_limit <= 0)
                        {
                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ERROR!（無限ループ）)");
                        }
                    }

                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + ")[" + master_data.name + "]" + master_data.detail);
                }
            }
        }
#endif //BUILD_TYPE_DEBUG


        // 攻撃力、体力の増加値を加算
        m_CharaPow += nPlusValuePow + nElemValuePow + nRaceValuePow + nLinkPlusValuePow + (int)fLimitOverATK;
        m_CharaHP += nPlusValueHP + nElemValueHP + nRaceValueHP + nLinkPlusValueHP + (int)fLimitOverHP;
        m_CharaCharm += nLinkCharm;

        if (m_CharaPow > GlobalDefine.VALUE_MAX_POW)
        {
            m_CharaPow = GlobalDefine.VALUE_MAX_POW;
        }

        if (m_CharaHP > GlobalDefine.VALUE_MAX_HP)
        {
            m_CharaHP = GlobalDefine.VALUE_MAX_HP;
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キャラ情報セットアップ：エネミーマスターデータ指定
		@param	MasterDataParamEnemy	cMasterData		エネミーマスター
	*/
    //----------------------------------------------------------------------------
    public bool CharaSetupFromParamEnemy(MasterDataParamEnemy cMasterData)
    {
        m_CharaMasterDataParam = BattleParam.m_MasterDataCache.useCharaParam(cMasterData.chara_id);
        m_bHasCharaMasterDataParam = (null != m_CharaMasterDataParam);

        if (m_CharaMasterDataParam == null)
        {
            Debug.LogError("CharaSetup Error! - " + cMasterData.chara_id);
            return false;
        }

        m_CharaLevel = 1;
        m_CharaLBSLv = 0;
        m_CharaPow = cMasterData.status_pow;
        m_CharaDef = cMasterData.status_def;
        m_CharaHP = cMasterData.status_hp;
        m_CharaLimitBreak = 0;
        m_CharaPlusPow = 0;
        m_CharaPlusDef = 0;
        m_CharaPlusHP = 0;

        // @add Developer 2015/09/03 ver300
        // リンク用変数の初期化
        m_LinkParam.Setup();

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リミットブレイクスキルパワーのチェック
		@return		bool		[足りる/足りない]
	*/
    //----------------------------------------------------------------------------
    public bool ChkCharaLimitBreak()
    {
        if (!m_bHasCharaMasterDataParam)
        {
            return false;
        }

        if (m_CharaMasterDataParam.skill_limitbreak == 0)
        {
            return false;
        }

        int max_turn = GetMaxTurn();
        if (max_turn < 0)
        {
            return false;
        }

        if (m_CharaLimitBreak >= max_turn)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// リミットブレイク発動可能になるまでのターン数を取得（ゼロなら発動可能）
    /// </summary>
    /// <returns></returns>
    public int GetTrunToLimitBreak()
    {
        int ret_val = 999;
        if (m_CharaMasterDataParam != null)
        {
            int max_turn = GetMaxTurn();
            if (max_turn >= 0)
            {
                ret_val = max_turn - m_CharaLimitBreak;
                if (ret_val < 0)
                {
                    ret_val = 0;
                }
            }
        }
        return ret_val;
    }

    /// <summary>
    /// このキャラのリミットブレイクターン数
    /// </summary>
    /// <returns></returns>
    public int GetMaxTurn()
    {
        MasterDataSkillLimitBreak lbsParam = BattleParam.m_MasterDataCache.useSkillLimitBreak(m_CharaMasterDataParam.skill_limitbreak);
        if (lbsParam != null)
        {
            return lbsParam.use_turn - m_CharaLBSLv;
        }
        return -1;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リミットブレイクスキルパワーの増加
		@param[in]		int		(val)		増加量
	*/
    //----------------------------------------------------------------------------
    public void AddCharaLimitBreak(int val)
    {
        if (!m_bHasCharaMasterDataParam)
        {
            return;
        }

        if (m_CharaMasterDataParam.skill_limitbreak == 0)
        {
            return;
        }

        int max_turn = GetMaxTurn();
        if (max_turn < 0)
        {
            return;
        }

        // リミットブレイクスキルパワーの必要数が最大値
        m_CharaLimitBreak += val;
        if (m_CharaLimitBreak > max_turn)
        {
            m_CharaLimitBreak = max_turn;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リミットブレイクスキルパワーの減少
	*/
    //----------------------------------------------------------------------------
    public void DelCharaLimitBreak(int val)
    {
        m_CharaLimitBreak -= val;
        if (m_CharaLimitBreak < GlobalDefine.SKILL_LIMITBREAK_MIN)
        {
            m_CharaLimitBreak = GlobalDefine.SKILL_LIMITBREAK_MIN;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リミットブレイクスキルパワーの削除
	*/
    //----------------------------------------------------------------------------
    public void ClrCharaLimitBreak()
    {
        m_CharaLimitBreak = 0;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リミットブレイクスキルパワーの使用可能状態
	*/
    //----------------------------------------------------------------------------
    public void MaxCharaLimitBreak()
    {
        if (!m_bHasCharaMasterDataParam)
        {
            return;
        }

        if (m_CharaMasterDataParam.skill_limitbreak == 0)
        {
            return;
        }

        // リミットブレイクスキルパワーのコストMAX
        m_CharaLimitBreak = GetMaxTurn();
    }

} // class CharaOnce

//----------------------------------------------------------------------------
/*!
	@brief	リンクキャラ情報
	@note
*/
//----------------------------------------------------------------------------
[Serializable]
public class CharaLinkParam
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    // @add Developer 2015/09/03 ver300
    public uint m_CharaID = 0;          //!< キャラステータス：リンク：キャラID
    public int m_CharaLv = 0;           //!< キャラステータス：リンク：キャラレベル
    public int m_CharaPlusPow = 0;          //!< キャラステータス：リンク：プラス値：攻撃力
    public int m_CharaPlusHP = 0;           //!< キャラステータス：リンク：プラス値：体力
    public int m_CharaLinkPoint = 0;            //!< キャラステータス：リンク：ポイント(リンクスキル発動率に影響)
    public int m_CharaLOLevel = 0;          //!< キャラステータス：リンク：限界突破レベル

    public MasterDataParamChara m_cCharaMasterDataParam = null;     //!< キャラステータス：リンク：リンクキャラのマスターデータ。
    public bool m_bHasCharaMasterDataParam = false; //!< 非nullキャッシュ：m_cCharaMasterDataParam。

    //----------------------------------------------------------------------------
    /*!
		@brief	リンクキャラ情報セットアップ
		@param	uint	unLinkID		リンク：キャラID
		@param	int		nLinkLv			リンク：キャラレベル
		@param	int		nLinkPlusPow	リンク：プラス値：攻撃
		@param	int		nLinkPlusHP		リンク：プラス値：体力
		@param	int		nLinkPoint		リンク：ポイント
		@param	int		nLinkLOLevel	リンク：限界突破レベル
		@note	Developer 2015/09/03 ver300
				引数を追加
	*/
    //----------------------------------------------------------------------------
    public void Setup(uint unLinkID = 0, int nLinkLv = 0, int nLinkPlusPow = 0, int nLinkPlusHP = 0, int nLinkPoint = 0, int nLinkLOLevel = 0)
    {
        m_CharaID = unLinkID;
        m_CharaLv = nLinkLv;
        m_CharaPlusPow = nLinkPlusPow;
        m_CharaPlusHP = nLinkPlusHP;
        m_CharaLinkPoint = nLinkPoint;
        m_CharaLOLevel = nLinkLOLevel;

        m_cCharaMasterDataParam = BattleParam.m_MasterDataCache.useCharaParam(m_CharaID);
        m_bHasCharaMasterDataParam = (null != m_cCharaMasterDataParam);
    }

} // class CharaLinkParam