/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	CharaUtil.cs
	@brief	キャラ関連ユーティリティ
	@author Developer
	@date 	2012/12/05
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
	@brief	キャラ関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class CharaUtil
{
    public enum VALUE : int
    {
        POW = 0,                            //!< キャラステータス値：攻撃
        DEF = 1,                            //!< キャラステータス値：防御
        HP = 2,                         //!< キャラステータス値：体力
        EXP = 3,                            //!< キャラステータス値：レベルアップに必要な経験値
        EXP_PARTS = 4,                          //!< キャラステータス値：素材になる時の経験値
        SALE = 5,                           //!< キャラステータス値：売却価格
        CRT = 6,                            //!< キャラステータス値：クリティカル威力
        BST_PANEL = 7,                          //!< キャラステータス値：ブーストパネル威力
    }
    //	public	const	int		VALUE_POW		= 0;	//!< キャラステータス値：攻撃
    //	public	const	int		VALUE_DEF		= 1;	//!< キャラステータス値：防御
    //	public	const	int		VALUE_HP		= 2;	//!< キャラステータス値：体力
    //	public	const	int		VALUE_EXP		= 3;	//!< キャラステータス値：レベルアップに必要な経験値
    //	public	const	int		VALUE_EXP_PARTS	= 4;	//!< キャラステータス値：素材になる時の経験値
    //	public	const	int		VALUE_SALE		= 5;	//!< キャラステータス値：売却価格
    //	public	const	int		VALUE_CRT		= 6;	//!< キャラステータス値：クリティカル威力
    //	public	const	int		VALUE_BST_PANEL	= 7;	//!< キャラステータス値：ブーストパネル威力

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	ステータス値取得
	*/
    //----------------------------------------------------------------------------
    static public int GetStatusValue(MasterDataParamChara cMaster, int nLevel, CharaUtil.VALUE nValueType)
    {
        //--------------------------------
        // 要素のmax,minを求める
        //--------------------------------
        int nValueMin = 0;
        int nValueMax = 0;
        MasterDataDefineLabel.CurveType nCurveType = MasterDataDefineLabel.CurveType.BALANCE_A;
        bool bServerCal = false;
        switch (nValueType)
        {
            case CharaUtil.VALUE.POW: nValueMin = cMaster.base_attack_min; nValueMax = cMaster.base_attack_max; nCurveType = cMaster.base_attack_curve; bServerCal = false; break;
            case CharaUtil.VALUE.DEF: nValueMin = cMaster.base_defense_min; nValueMax = cMaster.base_defense_max; nCurveType = cMaster.base_defense_curve; bServerCal = false; break;
            case CharaUtil.VALUE.HP: nValueMin = cMaster.base_hp_min; nValueMax = cMaster.base_hp_max; nCurveType = cMaster.base_hp_curve; bServerCal = false; break;
            case CharaUtil.VALUE.EXP: nValueMin = 0; nValueMax = cMaster.exp_total; nCurveType = cMaster.exp_total_curve; bServerCal = true; break;
            case CharaUtil.VALUE.EXP_PARTS: nValueMin = cMaster.blend_exp_min; nValueMax = cMaster.blend_exp_max; nCurveType = cMaster.blend_exp_curve; bServerCal = true; break;
            case CharaUtil.VALUE.SALE: nValueMin = cMaster.sales_min; nValueMax = cMaster.sales_max; nCurveType = cMaster.sales_curve; bServerCal = true; break;
            default: Debug.LogError("Status Type NG!!"); return 1;
        }

        //--------------------------------
        // レベルの最低最大が一致する場合は
        // 補間計算をせずに最低値を返す
        //--------------------------------
        if (nValueMin == nValueMax)
        {
            return nValueMin;
        }

        //--------------------------------
        // 既に最大レベルなら最大値を返す
        //--------------------------------
        if (nLevel >= cMaster.level_max)
        {
            return nValueMax;
        }

        //--------------------------------
        // 要素のレベル達成割合を求める
        //--------------------------------
        /*
				float fLevelRate = 1.0f;
				if( nLevel < cMaster.level_min )
				{
					Debug.LogError( "Level Range NG!!" );
					fLevelRate = 0.0f;
				}
				else
				if( cMaster.level_max > cMaster.level_min )
				{
					fLevelRate = 1.0f - ( (float)( cMaster.level_max - nLevel ) / (float)( cMaster.level_max - cMaster.level_min ) );
				}
		*/

        //--------------------------------
        // 成長曲線タイプでべき乗係数を変化
        //--------------------------------
        //float fPowerRate = 1.0f;
        //switch( nCurveType )
        //{
        //	case MasterDataDefineLabel.CURVE_NONE:		fPowerRate = 1.0f;		break;		// 補間タイプ： - 
        //	case MasterDataDefineLabel.CURVE_1_A:		fPowerRate = 1.0f;		break;		// 補間タイプ：バランス：A
        //	case MasterDataDefineLabel.CURVE_1_B:		fPowerRate = 1.0f;		break;		// 補間タイプ：バランス：B
        //	case MasterDataDefineLabel.CURVE_1_C:		fPowerRate = 2.5f;		break;		// 補間タイプ：バランス：C
        //	case MasterDataDefineLabel.CURVE_2_A:		fPowerRate = 0.7f;		break;		// 補間タイプ：早熟：A
        //	case MasterDataDefineLabel.CURVE_2_B:		fPowerRate = 0.7f;		break;		// 補間タイプ：早熟：B
        //	case MasterDataDefineLabel.CURVE_2_C:		fPowerRate = 0.7f;		break;		// 補間タイプ：早熟：C
        //	case MasterDataDefineLabel.CURVE_3_A:		fPowerRate = 1.5f;		break;		// 補間タイプ：大器晩成：A
        //	case MasterDataDefineLabel.CURVE_3_B:		fPowerRate = 1.5f;		break;		// 補間タイプ：大器晩成：B
        //	case MasterDataDefineLabel.CURVE_3_C:		fPowerRate = 1.5f;		break;		// 補間タイプ：大器晩成：C
        //	default:	Debug.LogError( "CurveType Error! - " + nCurveType );	break;		// 補間タイプ：__
        //}

        //--------------------------------
        // 補間で数値算出
        //--------------------------------
        int nTotalLevelMax = cMaster.level_max - cMaster.level_min;
        int nTotalLevelNow = nLevel - cMaster.level_min;
        int nResult = 0;

        //--------------------------------
        // サーバでも計算している項目は、
        // サーバ数値を正として同じになるように計算する
        // @change Developer 2016/06/15 v342：計算誤差修正
        //--------------------------------
        if (bServerCal == true)
        {
            #region ==== サーバでも計算する項目 ====
            //--------------------------------
            // double用の成長曲線タイプを取得
            // 上記で成長曲線タイプを取得した変数を(double)キャストしても計算が狂う
            //--------------------------------
            double dPowerRate = 1.0;
            switch (nCurveType)
            {
                case MasterDataDefineLabel.CurveType.NONE: dPowerRate = 1.0; break;     // 補間タイプ： - 
                case MasterDataDefineLabel.CurveType.BALANCE_A: dPowerRate = 1.0; break;        // 補間タイプ：バランス：A
                case MasterDataDefineLabel.CurveType.BALANCE_B: dPowerRate = 1.0; break;        // 補間タイプ：バランス：B
                case MasterDataDefineLabel.CurveType.BALANCE_C: dPowerRate = 2.5; break;        // 補間タイプ：バランス：C
                case MasterDataDefineLabel.CurveType.PRECOCITY_A: dPowerRate = 0.7; break;      // 補間タイプ：早熟：A
                case MasterDataDefineLabel.CurveType.PRECOCITY_B: dPowerRate = 0.7; break;      // 補間タイプ：早熟：B
                case MasterDataDefineLabel.CurveType.PRECOCITY_C: dPowerRate = 0.7; break;      // 補間タイプ：早熟：C
                case MasterDataDefineLabel.CurveType.LATE_DEVELOPMENT_A: dPowerRate = 1.5; break;       // 補間タイプ：大器晩成：A
                case MasterDataDefineLabel.CurveType.LATE_DEVELOPMENT_B: dPowerRate = 1.5; break;       // 補間タイプ：大器晩成：B
                case MasterDataDefineLabel.CurveType.LATE_DEVELOPMENT_C: dPowerRate = 1.5; break;       // 補間タイプ：大器晩成：C
                default: Debug.LogError("CurveType Error! - " + nCurveType); break;     // 補間タイプ：__
            }

            nResult = (int)(nValueMin + (nValueMax - nValueMin) * Math.Exp(Math.Log((double)nTotalLevelNow / (double)nTotalLevelMax) * dPowerRate));
            #endregion
        }
        else
        {
            #region ==== クライアントのみ計算する項目 ====
            //--------------------------------
            // 成長曲線タイプでべき乗係数を変化
            //--------------------------------
            float fPowerRate = 1.0f;
            switch (nCurveType)
            {
                case MasterDataDefineLabel.CurveType.NONE: fPowerRate = 1.0f; break;        // 補間タイプ： - 
                case MasterDataDefineLabel.CurveType.BALANCE_A: fPowerRate = 1.0f; break;       // 補間タイプ：バランス：A
                case MasterDataDefineLabel.CurveType.BALANCE_B: fPowerRate = 1.0f; break;       // 補間タイプ：バランス：B
                case MasterDataDefineLabel.CurveType.BALANCE_C: fPowerRate = 2.5f; break;       // 補間タイプ：バランス：C
                case MasterDataDefineLabel.CurveType.PRECOCITY_A: fPowerRate = 0.7f; break;     // 補間タイプ：早熟：A
                case MasterDataDefineLabel.CurveType.PRECOCITY_B: fPowerRate = 0.7f; break;     // 補間タイプ：早熟：B
                case MasterDataDefineLabel.CurveType.PRECOCITY_C: fPowerRate = 0.7f; break;     // 補間タイプ：早熟：C
                case MasterDataDefineLabel.CurveType.LATE_DEVELOPMENT_A: fPowerRate = 1.5f; break;      // 補間タイプ：大器晩成：A
                case MasterDataDefineLabel.CurveType.LATE_DEVELOPMENT_B: fPowerRate = 1.5f; break;      // 補間タイプ：大器晩成：B
                case MasterDataDefineLabel.CurveType.LATE_DEVELOPMENT_C: fPowerRate = 1.5f; break;      // 補間タイプ：大器晩成：C
                default: Debug.LogError("CurveType Error! - " + nCurveType); break;     // 補間タイプ：__
            }

            nResult = (int)(nValueMin + (nValueMax - nValueMin) * Mathf.Pow((float)nTotalLevelNow / (float)nTotalLevelMax, fPowerRate));
            #endregion
        }

        return nResult;
        //		return (int)(nValueMin + ( nValueMax - nValueMin ) * Mathf.Pow( (float)nTotalLevelNow / (float)nTotalLevelMax , fPowerRate ));
        //		return (int)(nValueMin + ( nValueMax - nValueMin ) * Mathf.Pow( (float)( nLevel - 1 ) / (float)( cMaster.level_max - 1 ) , fPowerRate ));
    }

    static public void setupCharaParty(ref CharaParty _party, PacketStructUnit[] unitList, PacketStructUnit[] linkUnitList = null)
    {
        if (_party == null || unitList == null)
        {
            return;
        }

        CharaOnce[] acCharaList = new CharaOnce[(int)GlobalDefine.PartyCharaIndex.MAX];

        for (int j = 0; j < unitList.Length; j++)
        {
            if (unitList[j] == null)
                continue;
            PacketStructUnit cBaseUnit = unitList[j];

            // リンクの情報
            int nLinkLimitOverLV = 0;
            uint nLinkId = 0;
            int nLinkLevel = 0;
            int nLinkPlusPow = 0;
            int nLinkPlusHp = 0;

            PacketStructUnit cLinkUnit;
            if (linkUnitList != null && linkUnitList.IsRange(j))
            {
                cLinkUnit = linkUnitList[j];
            }
            else
            {
                cLinkUnit = CharaLinkUtil.GetLinkUnit(cBaseUnit.link_unique_id);
            }

            if (cLinkUnit != null)
            {
                nLinkId = cLinkUnit.id;
                nLinkLevel = (int)cLinkUnit.level;
                nLinkPlusPow = (int)cLinkUnit.add_pow;
                nLinkPlusHp = (int)cLinkUnit.add_hp;
                nLinkLimitOverLV = (int)cLinkUnit.limitover_lv;
            }


            acCharaList[j] = new CharaOnce();
            acCharaList[j].CharaSetupFromID(cBaseUnit.id, (int)cBaseUnit.level, (int)cBaseUnit.limitbreak_lv, (int)cBaseUnit.limitover_lv, (int)cBaseUnit.add_pow, (int)cBaseUnit.add_hp,
                                                nLinkId, nLinkLevel, nLinkPlusPow, nLinkPlusHp, (int)cBaseUnit.link_point, nLinkLimitOverLV);
        }

        _party.PartySetupMenu(acCharaList, false);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	経験値量からレベルを算出
	*/
    //----------------------------------------------------------------------------
    static public int GetLevelFromExp(MasterDataParamChara cMaster, int nExp)
    {
        if (cMaster == null)
            return 1;

        //--------------------------------
        // 最大レベルまで補間しつつ必要経験値を求め、
        // 必要経験値に達していないならその直前のレベルを概要レベルとして返す
        //--------------------------------
        int nPrevLevel = 1;
        for (int i = 0; i < cMaster.level_max; i++)
        {
            // ※レベルは 1～Max まで
            int nChkLevel = i + 1;
            int nChkLevelExp = GetStatusValue(cMaster, nChkLevel, CharaUtil.VALUE.EXP);

            if (nExp < nChkLevelExp)
            {
                return nPrevLevel;
            }

            nPrevLevel = nChkLevel;
        }

        return cMaster.level_max;
    }

};

/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
