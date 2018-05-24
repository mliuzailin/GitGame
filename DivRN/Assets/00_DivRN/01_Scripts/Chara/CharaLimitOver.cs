/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	CharaLimitOver.cs
	@brief	限界突破
	@author Developer
	@date 	2016/02/29
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
	@brief	キャラユニット限界突破関連
*/
//----------------------------------------------------------------------------
static public class CharaLimitOver
{
    public enum EGET
    {
        ePARAM_NONE = 0,                    //!< パラメーター：なし
        ePARAM_HP,                          //!< パラメーター：HP
        ePARAM_ATK,                         //!< パラメーター：攻撃力
        ePARAM_COST,                        //!< パラメーター：コスト
        ePARAM_LIMITOVER_MAX,               //!< パラメーター：限界突破最大値
    }

    public enum RESULT_TYPE
    {
        eNone = 0,
        ePossible,                          //!< 合成可能
        eValueOver,                         //!< 限界突破の上限値を超える場合
        eValueMax,                          //!< 限界突破が元々MAXの場合
        eMaterialLimitOver,                 //!< 限界突破が発生しない組み合わせで素材ユニットが限界突破してる場合
    }

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    public static TemplateList<uint> m_EvolBaseUnitIdList = new TemplateList<uint>();

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/

    //----------------------------------------------------------------------------
    /*!
		@brief	限界突破パラメーター取得
	*/
    //----------------------------------------------------------------------------
    static public float GetParam(uint nLimitOverLevel, int nLimitOverType, int nLimitOverParam)
    {
        if (nLimitOverType <= 0)
        {
            return 0;
        }

        // nLimitOverTypeはFixIDとして値を受け取っています
        MasterDataLimitOver cLimitOverMaster = MasterDataUtil.GetLimitOverFromID(nLimitOverType);
        if (cLimitOverMaster == null)
        {
            return 0;
        }

        // 成長タイプ
        MasterDataDefineLabel.LimitOverCurveType nGrowType = cLimitOverMaster.limit_grow;

        //--------------------------------
        // 成長曲線タイプでべき乗係数を変化
        //--------------------------------
        float fRate = 1.0f;

        switch (nGrowType)
        {
            case MasterDataDefineLabel.LimitOverCurveType.NONE: fRate = 0.0f; break;
            case MasterDataDefineLabel.LimitOverCurveType.NORMAL: fRate = 1.0f; break;
            case MasterDataDefineLabel.LimitOverCurveType.PRECOCITY: fRate = 0.7f; break;
            case MasterDataDefineLabel.LimitOverCurveType.LATE_DEVELOPMENT: fRate = 1.5f; break;
        }

        int nMaxRate = 0;
        float nParamRate = 0;

        // 限界突破の最大値
        int fLimitOverMax = cLimitOverMaster.limit_over_max;

        switch (nLimitOverParam)
        {
            case (int)EGET.ePARAM_HP: // HP上昇値
                nMaxRate = cLimitOverMaster.limit_over_max_hp;

                if (nLimitOverLevel > 0)
                {
                    nParamRate = (nMaxRate * Mathf.Pow((float)nLimitOverLevel / (float)fLimitOverMax, fRate));
                }
                break;
            case (int)EGET.ePARAM_ATK: // ATK上昇値
                nMaxRate = cLimitOverMaster.limit_over_max_atk;

                if (nLimitOverLevel > 0)
                {
                    nParamRate = (nMaxRate * Mathf.Pow((float)nLimitOverLevel / (float)fLimitOverMax, fRate));
                }
                break;
            case (int)EGET.ePARAM_COST: // COST上昇値
                nMaxRate = cLimitOverMaster.limit_over_max_cost;

                if (nLimitOverLevel > 0)
                {
                    nParamRate = (int)(nMaxRate * Mathf.Pow((float)nLimitOverLevel / (float)fLimitOverMax, fRate));
                }
                break;
            case (int)EGET.ePARAM_LIMITOVER_MAX: // 限界突破最大値
                nParamRate = fLimitOverMax;
                break;

        }
        return nParamRate;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	限界突破パラメーター取得
	*/
    //----------------------------------------------------------------------------
    static public double GetParamCharm(uint nLimitOverLevel, int nLimitOverType)
    {
        if (nLimitOverType == 0)
        {
            return 0;
        }

        MasterDataLimitOver cLimitOverMaster = MasterDataUtil.GetLimitOverFromID(nLimitOverType);
        if (cLimitOverMaster == null)
        {
            return 0;
        }

        // 成長タイプ
        MasterDataDefineLabel.LimitOverCurveType nGrowType = cLimitOverMaster.limit_grow;

        // 限界突破の最大値
        int fLimitOverMax = cLimitOverMaster.limit_over_max;

        //--------------------------------
        // 成長曲線タイプでべき乗係数を変化
        //--------------------------------
        float fRate = 1.0f;

        switch (nGrowType)
        {
            case MasterDataDefineLabel.LimitOverCurveType.NONE: fRate = 0.0f; break;
            case MasterDataDefineLabel.LimitOverCurveType.NORMAL: fRate = 1.0f; break;
            case MasterDataDefineLabel.LimitOverCurveType.PRECOCITY: fRate = 0.7f; break;
            case MasterDataDefineLabel.LimitOverCurveType.LATE_DEVELOPMENT: fRate = 1.5f; break;
        }

        int nMaxRate = 0;
        double dParamRate = 0;

        nMaxRate = cLimitOverMaster.limit_over_max_charm;

        if (nLimitOverLevel > 0)
        {
            double dRate = Math.Floor(nMaxRate * Mathf.Pow((float)nLimitOverLevel / (float)fLimitOverMax, fRate) * 10);
            dParamRate = dRate / 10;

            if (dParamRate > nMaxRate)
            {
                dParamRate = nMaxRate;
            }
        }

        return dParamRate;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	売却時の限界突破ボーナス値
	*/
    //----------------------------------------------------------------------------
    static public double GetParamSaleLimitOverBouns(double dLimitOverLevel, double dLimitOverMax, uint unUnitMoney)
    {
        double dLoBouns = MasterDataUtil.GetSaleLimitOverBonus() * (dLimitOverLevel / dLimitOverMax);

        double dSaleBouns = (double)(unUnitMoney * (dLimitOverLevel + 1) * (1 + dLoBouns) - unUnitMoney);

        return dSaleBouns;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化時の限界突破ボーナス値
	*/
    //----------------------------------------------------------------------------
    static public double GetParamBuildUpLimitOverBouns(double dLimitOverLevel, double dLimitOverMax, int nDefalutUnitExp)
    {
        double dLoBouns = MasterDataUtil.GetBuildUpLimitOverBonus() * (dLimitOverLevel / dLimitOverMax);

        double dBuildUpBouns = (double)(nDefalutUnitExp * (dLimitOverLevel + 1) * (1 + dLoBouns) - nDefalutUnitExp);

        return dBuildUpBouns;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ベースユニットの進化先をリストに保持
		@note	
	*/
    //----------------------------------------------------------------------------
    static public void SetEvolBaseUnitIdList(uint unBaseUnitId)
    {
        // リストを初期化
        m_EvolBaseUnitIdList.Release();

        // 進化前を検索
        SearchEvolList(unBaseUnitId, false);

        // 進化後を検索
        SearchEvolList(unBaseUnitId, true);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	進化先のユニット情報検索
		@note	
	*/
    //----------------------------------------------------------------------------
    static public void SearchEvolList(uint unBaseUnitId, bool bAfter)
    {
        // 進化前情報を取得
        MasterDataParamCharaEvol[] cBaseUnitData = MasterDataUtil.GetCharaEvolParamFromCharaListID(unBaseUnitId, bAfter);

        bool bNewUnitId = true;

        if (cBaseUnitData != null)
        {
            for (int i = 0; i < cBaseUnitData.Length; i++)
            {
                if (cBaseUnitData[i] != null)
                {

                    uint unID = 0;

                    if (bAfter)
                    {
                        unID = cBaseUnitData[i].unit_id_after;
                    }
                    else
                    {
                        unID = cBaseUnitData[i].unit_id_pre;
                    }


                    // すでにリストに同じユニット情報があるかチェック
                    for (int j = 0; j < m_EvolBaseUnitIdList.GetLength(); j++)
                    {
                        if (m_EvolBaseUnitIdList[j] == unID)
                        {
                            bNewUnitId = false;
                            break;
                        }
                    }

                    // リストに追加されてないIDなら登録し再検索
                    if (bNewUnitId == true)
                    {
                        // 追加されてないユニット情報を追加
                        m_EvolBaseUnitIdList.Add(unID);

                        // 再検索
                        SearchEvolList(unID, bAfter);
                    }
                }
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	限界突破対応ユニットかチェック
		@note	
	*/
    //----------------------------------------------------------------------------
    static public bool CheckEvolUnit(uint unMaterialUnitId)
    {
        bool bPossibleFlg = false;

        for (int i = 0; i < m_EvolBaseUnitIdList.GetLength(); i++)
        {
            if (m_EvolBaseUnitIdList[i] == unMaterialUnitId)
            {
                bPossibleFlg = true;
            }
        }

        return bPossibleFlg;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	限界突破表示単体(強化用)
	*/
    //----------------------------------------------------------------------------
    static public void LimitOverBuildUpAnimationOnce(GameObject cObj, string sttAddText1, Color cLabelColor)
    {
        /*
                CtrUILabelSwitch cLabelSwitch = null;

                if( cObj.GetComponent< CtrUILabelSwitch >() == null )
                {
                    cLabelSwitch = cObj.AddComponent< CtrUILabelSwitch >();
                }
                else
                {
                    cLabelSwitch = cObj.GetComponent< CtrUILabelSwitch >();
                }

                cLabelSwitch.ClrText();

                cLabelSwitch.AddText( sttAddText1 );
                cLabelSwitch.SetColor( cLabelColor );
                cLabelSwitch.m_EnableResetFlg = true;
                cLabelSwitch.MessageFixUpdate();
                cLabelSwitch.ResetSwitchAnimation();
        */
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	限界突破用テキスト：タイトル
	*/
    //----------------------------------------------------------------------------
    static public string DialogTitleTextType(int nType)
    {
        string strMsg = "";
        switch (nType)
        {
            case (int)RESULT_TYPE.ePossible: strMsg = GameTextUtil.GetText("UNIT_LIMIT_OVER_POSSIBLE_TITLE"); return strMsg;        // 条件を満たしてる場合
            case (int)RESULT_TYPE.eValueOver: strMsg = GameTextUtil.GetText("UNIT_LIMIT_OVER_VALUE_OVER_TITLE"); return strMsg;     // レベルがMAX出ない場合
            case (int)RESULT_TYPE.eValueMax: strMsg = GameTextUtil.GetText("UNIT_LIMIT_OVER_VALUE_MAX_TITLE"); return strMsg;       // 限界突破の上限値を超える場合
            case (int)RESULT_TYPE.eMaterialLimitOver: strMsg = GameTextUtil.GetText("UNIT_LIMIT_OVER_MATERIAL_WARNING_TITLE"); return strMsg;       // 限界突破が発生しない組み合わせで素材ユニットが限界突破してる場合
        }
        return strMsg;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	限界突破用テキスト：本文
	*/
    //----------------------------------------------------------------------------
    static public string DialogTextType(int nType)
    {
        string strMsg = "";
        switch (nType)
        {
            case (int)RESULT_TYPE.ePossible: strMsg = GameTextUtil.GetText("UNIT_LIMIT_OVER_POSSIBLE_MSG"); return strMsg;      // 条件を満たしてる場合
            case (int)RESULT_TYPE.eValueOver: strMsg = GameTextUtil.GetText("UNIT_LIMIT_OVER_VALUE_OVER_MSG"); return strMsg;       // レベルがMAX出ない場合
            case (int)RESULT_TYPE.eValueMax: strMsg = GameTextUtil.GetText("UNIT_LIMIT_OVER_VALUE_MAX_MSG"); return strMsg;     // 限界突破の上限値を超える場合
            case (int)RESULT_TYPE.eMaterialLimitOver: strMsg = GameTextUtil.GetText("UNIT_LIMIT_OVER_MATERIAL_WARNING_MSG"); return strMsg;     // 限界突破が発生しない組み合わせで素材ユニットが限界突破してる場合
        }
        return strMsg;
    }
};

/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
