/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	CharaLinkUtil.cs
	@brief	リンクシステムクラス
	@author Developer
	@date 	2015/02/12
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
	@brief	リンクシステムクラス
*/
//----------------------------------------------------------------------------
static public class CharaLinkUtil
{
    private const float RATE_COST = (0.2f); // 属性ボーナス：コスト補正値
    private const float RATE_LEVEL = (0.03f);   // 属性ボーナス：レベル補正値
    private const int LINK_BONUS_KIND = (6);    // 種族ボーナス開始の添え字番号
    private const int LINK_BONUS_KIND_SUB = (62);   // 副種族ボーナス開始の添え字番号
    public const int LINK_POINT_MAX = (10000);  // リンクポイント最大値
    public const uint SKILL_LINK_ODDS_MAX = (10000);    // リンクスキル発動率最大値

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：リンクユニットの取得
		@param[in]	PacketStructUnit	(cBaseUnit)			ベースユニット
		@return		PacketStructUnit	[リンクユニット]
		@note		自分のユニットリストから、リンクユニットを取得
	*/
    //----------------------------------------------------------------------------
    static public PacketStructUnit GetLinkUnit(PacketStructUnit cBaseUnit)
    {
        UserDataAdmin cUserData = UserDataAdmin.Instance;
        PacketStructUnit cLinkUnit = null;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (cUserData == null
        || cBaseUnit == null)
        {
            return (cLinkUnit);
        }


        cLinkUnit = cUserData.SearchChara(cBaseUnit.link_unique_id);

        return (cLinkUnit);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：リンクユニットの取得(ユニークID指定)
		@param[in]	long				(lLinkUnitUniqueID)		リンクユニット：ユニークID
		@return		PacketStructUnit	[リンクユニット]
		@note		自分のユニットリストから、リンクユニットを取得
	*/
    //----------------------------------------------------------------------------
    static public PacketStructUnit GetLinkUnit(long lLinkUnitUniqueID)
    {
        UserDataAdmin cUserData = UserDataAdmin.Instance;
        PacketStructUnit cLinkUnit = null;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (cUserData == null)
        {
            return (cLinkUnit);
        }


        cLinkUnit = cUserData.SearchChara(lLinkUnitUniqueID);

        return (cLinkUnit);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：リンクユニットIDの取得
		@param[in]	int		(nPartyCharaIdx)	パーティキャラ番号
		@return		uint	[キャラID]
		@note		パーティキャラから、リンクユニットのキャラIDを取得
	*/
    //----------------------------------------------------------------------------
    static public uint GetLinkUnitID(GlobalDefine.PartyCharaIndex nPartyCharaIdx)
    {
        uint unResult = 0;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember(nPartyCharaIdx, CharaParty.CharaCondition.EXIST);
        if (chara_once == null)
        {
            return (unResult);
        }

        unResult = chara_once.m_LinkParam.m_CharaID;

        return (unResult);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：リンクユニットIDの取得(キャラカットイン用に特化)
		@param[in]	BattleSkillActivity	(cActivity)	戦闘時スキル発動情報
		@return		uint				[キャラID]
		@note		パーティキャラから、リンクユニットのキャラIDを取得
	*/
    //----------------------------------------------------------------------------
    static public uint GetCutinLinkUnitID(BattleSkillActivity cActivity)
    {
        uint unResult = 0;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (cActivity == null)
        {
            return (unResult);
        }

        //--------------------------------
        // スキルタイプによる分岐
        //--------------------------------
        switch (cActivity.m_SkillType)
        {
            case ESKILLTYPE.eLINK:
            case ESKILLTYPE.eLINKPASSIVE:
                unResult = GetLinkUnitID(cActivity.m_SkillParamOwnerNum);
                break;

            case ESKILLTYPE.eACTIVE:
            case ESKILLTYPE.eLEADER:
            case ESKILLTYPE.ePASSIVE:
            case ESKILLTYPE.eLIMITBREAK:
            case ESKILLTYPE.eBOOST:
            default:
                break;
        }

        return (unResult);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：リンクユニットのパーティコスト
		@param[in]	PacketStructUnit	(cBaseUnit)			ベースユニット
		@return		int					[パーティコスト]
		@note		リンクユニットのパーティコストを取得
	*/
    //----------------------------------------------------------------------------
    static public int GetLinkUnitCost(PacketStructUnit cBaseUnit)
    {
        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (cBaseUnit == null)
        {
            return (0);
        }

        PacketStructUnit cLinkUnit = GetLinkUnit(cBaseUnit);
        if (cLinkUnit == null)
        {
            return (0);
        }
        MasterDataParamChara cCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(cLinkUnit.id);
        if (cCharaMaster == null)
        {
            return (0);
        }


        return (cCharaMaster.party_cost);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：リンクユニットのパーティコスト
		@param[in]	uint	(unLinkUnitID)		リンクユニット：キャラID
		@return		int		[ユニットコスト]
		@note		リンクユニットのパーティコストを取得
	*/
    //----------------------------------------------------------------------------
    static public int GetLinkUnitCost(uint unLinkUnitID)
    {
        if (unLinkUnitID == 0)
        {
            return (0);
        }

        MasterDataParamChara cCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(unLinkUnitID);
        if (cCharaMaster == null)
        {
            return (0);
        }


        return (cCharaMaster.party_cost);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：リンクスキル発動率：キャラマスター指定
		@param[in]	MasterDataParamChara	(cCharaMaster)	キャラマスター
		@param[in]	int						(nLinkPoint)	リンクポイント
		@return		uint					[リンクスキル発動率]
		@note		リンクスキル発動率を取得
	*/
    //----------------------------------------------------------------------------
    static public uint GetLinkSkillOdds(MasterDataParamChara cCharaMaster, int nLinkPoint)
    {
        uint unResult = 0;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (cCharaMaster == null)
        {
            Debug.LogError("MasterDataParamChara Is None!");
            return (unResult);
        }

        // ノーマルスキルマスターを取得
        MasterDataSkillActive cNormalMaster = BattleParam.m_MasterDataCache.useSkillActive(cCharaMaster.link_skill_active);
        if (cNormalMaster == null)
        {
            return (unResult);
        }

        // リンクスキル発動率
        unResult = GetLinkSkillOdds(cNormalMaster, nLinkPoint);


        return (unResult);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：リンクスキル発動率：ノーマルマスター指定
		@param[in]	MasterDataSkillActive	(cNormalMaster)	ノーマルマスター
		@param[in]	int						(nLinkPoint)	リンクポイント
		@return		uint					[リンクスキル発動率]
		@note		リンクスキル発動率を取得
	*/
    //----------------------------------------------------------------------------
    static public uint GetLinkSkillOdds(MasterDataSkillActive cNormalMaster, int nLinkPoint)
    {
        uint unResult = 0;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (cNormalMaster == null)
        {
            return (unResult);
        }

        // ベース発動率を取得
        float fBaseOdds = (float)cNormalMaster.skill_link_odds;

        // リンクポイントによる発動率を算出
        float fLinkOdds = 1.0f + ((float)nLinkPoint * 0.0001f);

        // リンクスキル発動率を算出(リンクスキル発動率 = ベース発動率 × { 1 + (リンク度/10000) })
        unResult = (uint)(fBaseOdds * fLinkOdds);
        if (unResult > SKILL_LINK_ODDS_MAX)
        {
            unResult = SKILL_LINK_ODDS_MAX;
        }

        return (unResult);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：属性ボーナス：キャラID指定
		@param[in]	uint	(unLinkUnitID)	リンクユニット：キャラID
		@param[in]	int		(nLevel)		ユニットレベル
		@param[in]	int		(nLoLevel)		限界突破レベル
		@param[in]	int		(nBonusType)	ボーナスタイプ
		@return		int		(nResult)		[+加算値]
		@note		HPとATKのボーナス値		※引数でまとめて受け取った方がいいかも
	*/
    //----------------------------------------------------------------------------
    static public int GetLinkUnitBonusElement(uint unLinkUnitID, int nLevel, uint nLoLevel, CharaUtil.VALUE nBonusType)
    {
        int nResult = 0;

        // キャラマスターを取得
        MasterDataParamChara cCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(unLinkUnitID);
        if (cCharaMaster == null)
        {
            return (nResult);
        }

        // 属性ボーナス
        nResult = GetLinkUnitBonusElement(cCharaMaster, nLevel, nLoLevel, nBonusType);

        return (nResult);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：属性ボーナス：キャラマスター指定
		@param[in]	MasterDataParamChara	(cCharaMaster)	キャラマスター
		@param[in]	int						(nLevel)		ユニットレベル
		@param[in]	int						(nLoLevel)		限界突破レベル
		@param[in]	int						(nBonusType)	ボーナスタイプ
		@return		int						(nResult)		[+加算値]
		@note		HPとATKのボーナス値		※引数でまとめて受け取った方がいいかも
	*/
    //----------------------------------------------------------------------------
    static public int GetLinkUnitBonusElement(MasterDataParamChara cCharaMaster, int nLevel, uint nLoLevel, CharaUtil.VALUE nBonusType)
    {
        MasterDataLinkSystem[] acLinkMaster = MasterFinder<MasterDataLinkSystem>.Instance.GetAll();
        int nResult = 0;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (acLinkMaster == null
        || cCharaMaster == null)
        {
            Debug.LogError("MasterDataLinkSystem or MasterDataParamChara Is None!");
            return (nResult);
        }

        int nLimitOverCost = (int)CharaLimitOver.GetParam(nLoLevel, cCharaMaster.limit_over_type, (int)CharaLimitOver.EGET.ePARAM_COST);

        //--------------------------------
        // 基礎値
        //--------------------------------
        float fBaseValue = 1.0f;
        float fCostValue = InGameUtilBattle.AvoidErrorMultiple((float)cCharaMaster.party_cost + nLimitOverCost, RATE_COST);
        float fRareValue = ((int)cCharaMaster.rare + 1) * ((int)cCharaMaster.rare + 1);


        fBaseValue += fCostValue + fRareValue;


        //--------------------------------
        // 属性値
        //--------------------------------
        int nLinkNum = 0;
        float fElemValue = 0.0f;


        // ボーナス定義の確定
        for (int num = 0; num < LINK_BONUS_KIND; ++num)
        {
            // ボーナス定義の属性と、リンクユニットの属性が等しい場合
            if (acLinkMaster[num].elem == cCharaMaster.element)
            {
                nLinkNum = num;
                break;
            }
        }

        // ボーナス値の種類による分岐
        switch (nBonusType)
        {
            case CharaUtil.VALUE.POW: fElemValue = InGameUtilBattle.GetDBRevisionValue((float)acLinkMaster[nLinkNum].atk); break;
            case CharaUtil.VALUE.HP: fElemValue = InGameUtilBattle.GetDBRevisionValue((float)acLinkMaster[nLinkNum].hp); break;
        }


        //--------------------------------
        // レベル補正
        //--------------------------------
        float fLevelCorrection = 1.0f;
        float fLevelValue = InGameUtilBattle.AvoidErrorMultiple((float)nLevel, RATE_LEVEL);


        fLevelCorrection += fLevelValue;


        //--------------------------------
        // 最終結果：属性ボーナス値(基礎値 × 属性値 × Lv補正)※端数切捨て
        //--------------------------------
        float fWork = 0.0f;

        fWork = InGameUtilBattle.AvoidErrorMultiple(fBaseValue, fElemValue);
        fWork = InGameUtilBattle.AvoidErrorMultiple(fWork, fLevelCorrection);
        nResult = (int)fWork;

        return (nResult);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：種族ボーナス：キャラID指定
		@param[in]	uint	(unLinkUnitID)	リンクユニット：キャラID
		@param[in]	int		(nBonusRace)	ボーナス種族
		@param[in]	int		(nBonusType)	ボーナスタイプ
		@return		float	(fResult)		[+%値]
		@note		種族・レア度毎のボーナス値を取得	※引数でまとめて受け取った方がいいかも
					この関数自体使わないかも(現在未使用)	2015/09/14 ver300
	*/
    //----------------------------------------------------------------------------
    static public float GetLinkUnitBonusRace(uint unLinkUnitID, MasterDataDefineLabel.KindType nBonusRace, CharaUtil.VALUE nBonusType)
    {
        float fResult = 0.0f;

        // キャラマスターを取得
        MasterDataParamChara cCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(unLinkUnitID);
        if (cCharaMaster == null)
        {
            return (fResult);
        }

        // 種族ボーナス
        fResult = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, nBonusType);

        return (fResult);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：種族ボーナス：キャラマスター指定
		@param[in]	MasterDataParamChara	(cCharaMaster)	リンクユニット：キャラマスター
		@param[in]	int						(nBonusRace)	ボーナス種族
		@param[in]	int						(nBonusType)	ボーナスタイプ
		@return		float					(fResult)		[+%値]
		@note		種族・レア度毎のボーナス値を取得	※引数でまとめて受け取った方がいいかも
	*/
    //----------------------------------------------------------------------------
    static public float GetLinkUnitBonusRace(MasterDataParamChara cCharaMaster, MasterDataDefineLabel.KindType nBonusRace, CharaUtil.VALUE nBonusType)
    {
        MasterDataLinkSystem[] acLinkMaster = MasterFinder<MasterDataLinkSystem>.Instance.GetAll();
        float fResult = 0.0f;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (acLinkMaster == null
        || cCharaMaster == null)
        {
            Debug.LogError("MasterDataLinkSystem or MasterDataParamChara Is None!");
            return (fResult);
        }

        // ボーナス種族が設定されていない場合
        if (nBonusRace <= MasterDataDefineLabel.KindType.NONE
        || nBonusRace >= MasterDataDefineLabel.KindType.MAX)
        {
            return (fResult);
        }

        //--------------------------------
        // 副種族持ちかチェック
        //--------------------------------
        MasterDataDefineLabel.KindType[] anCharaKind = new MasterDataDefineLabel.KindType[2]{ cCharaMaster.kind,
                                                                                                cCharaMaster.sub_kind };
        int nStartNum = 0;
        int nEndNum = 0;

        // 副種族が設定されていない場合
        if (anCharaKind[1] <= MasterDataDefineLabel.KindType.NONE
        || anCharaKind[1] >= MasterDataDefineLabel.KindType.MAX)
        {
            nStartNum = LINK_BONUS_KIND;
            nEndNum = LINK_BONUS_KIND_SUB;
        }
        else
        {
            nStartNum = LINK_BONUS_KIND_SUB;
            nEndNum = acLinkMaster.Length;
        }

        //--------------------------------
        // ボーナス定義の確定
        //--------------------------------
        int nLinkIdx = 0;
        for (int num = nStartNum; num < nEndNum; ++num)
        {
            // ボーナス定義の種族とレア度が、リンクユニットと等しい場合
            if (acLinkMaster[num].kind == nBonusRace
            && acLinkMaster[num].rare == cCharaMaster.rare)
            {
                nLinkIdx = num;
                break;
            }
        }

        //--------------------------------
        // ボーナス値の種類による分岐
        // ※HPは実値のため、そのまま取得する
        //--------------------------------
        switch (nBonusType)
        {
            case CharaUtil.VALUE.POW: fResult = InGameUtilBattle.GetDBRevisionValue((float)acLinkMaster[nLinkIdx].atk); break;
            case CharaUtil.VALUE.HP: fResult = (float)acLinkMaster[nLinkIdx].hp; break;
            case CharaUtil.VALUE.CRT: fResult = InGameUtilBattle.GetDBRevisionValue((float)acLinkMaster[nLinkIdx].crt); break;
            case CharaUtil.VALUE.BST_PANEL: fResult = InGameUtilBattle.GetDBRevisionValue((float)acLinkMaster[nLinkIdx].bst); break;
        }

        return (fResult);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：リンクユニットの種族ボーナステキスト
		@param[in]	uint	(unLinkUnitID)	リンクユニット：キャラマスター
		@return		string	(strRaceMsgKey)	[対応したゲームテキスト]
		@note
	*/
    //----------------------------------------------------------------------------
    static public string GetLinkBonusRaceText(MasterDataParamChara cCharaMaster)
    {
        string strRaceBonus = "";

        if (cCharaMaster != null)
        {
            MasterDataDefineLabel.KindType[] anKind = new MasterDataDefineLabel.KindType[2]{ cCharaMaster.kind,
                                                                                                   cCharaMaster.sub_kind };

            for (int num = 0; num < anKind.Length; ++num)
            {
                //--------------------------------
                // 副種族チェック
                //--------------------------------
                if (num != 0)
                {
                    if (anKind[num] <= MasterDataDefineLabel.KindType.NONE
                    || anKind[num] >= MasterDataDefineLabel.KindType.MAX)
                    {
                        break;
                    }

                    // 改行コードを追加
                    strRaceBonus += "\r\n";
                }

                //--------------------------------
                // テキスト構築
                //--------------------------------
                strRaceBonus += GetLinkBonusRaceText(cCharaMaster, anKind[num]);
            }
        }

        return (strRaceBonus);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：リンクユニットの種族ボーナステキスト
		@param[in]	uint	(unLinkUnitID)	リンクユニット：キャラマスター
		@param[in]	int		(nBonusRace)	ボーナス種族
		@return		string	(strRaceMsgKey)	[対応したゲームテキスト]
		@note
	*/
    //----------------------------------------------------------------------------
    static public string GetLinkBonusRaceText(MasterDataParamChara cCharaMaster, MasterDataDefineLabel.KindType nBonusRace)
    {
        string strRaceBonus = "";

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (cCharaMaster == null)
        {
            Debug.LogError("GameTextManager or MasterDataParamChara Is None!");
            return (strRaceBonus);
        }

        string strKindKey = "";
        string strBonusVal1 = "";
        string strBonusVal2 = "";

        //--------------------------------
        // 種族による分岐
        //--------------------------------
        switch (nBonusRace)
        {
            case MasterDataDefineLabel.KindType.HUMAN:
                strBonusVal1 = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, CharaUtil.VALUE.HP).ToString("F0");
                strBonusVal2 = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, CharaUtil.VALUE.POW).ToString("F1");
                strKindKey = "LINK_BONUS_HUMAN";
                break;
            case MasterDataDefineLabel.KindType.DRAGON:
                strBonusVal1 = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, CharaUtil.VALUE.BST_PANEL).ToString("F1");
                strKindKey = "LINK_BONUS_DRAGON";
                break;
            case MasterDataDefineLabel.KindType.GOD:
                strBonusVal1 = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, CharaUtil.VALUE.CRT).ToString("F1");
                strKindKey = "LINK_BONUS_GOD";
                break;
            case MasterDataDefineLabel.KindType.DEMON:
                strBonusVal1 = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, CharaUtil.VALUE.POW).ToString("F1");
                strKindKey = "LINK_BONUS_DEMON";
                break;
            case MasterDataDefineLabel.KindType.CREATURE:
                strBonusVal1 = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, CharaUtil.VALUE.HP).ToString("F0");
                strKindKey = "LINK_BONUS_CREATURE";
                break;
            case MasterDataDefineLabel.KindType.BEAST:
                strBonusVal1 = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, CharaUtil.VALUE.HP).ToString("F0");
                strBonusVal2 = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, CharaUtil.VALUE.CRT).ToString("F1");
                strKindKey = "LINK_BONUS_BEAST";
                break;
            case MasterDataDefineLabel.KindType.MACHINE:
                strBonusVal1 = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, CharaUtil.VALUE.POW).ToString("F1");
                strBonusVal2 = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, CharaUtil.VALUE.BST_PANEL).ToString("F1");
                strKindKey = "LINK_BONUS_MACHINE";
                break;
            case MasterDataDefineLabel.KindType.EGG:
                strBonusVal1 = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, CharaUtil.VALUE.CRT).ToString("F1");
                strBonusVal2 = GetLinkUnitBonusRace(cCharaMaster, nBonusRace, CharaUtil.VALUE.BST_PANEL).ToString("F1");
                strKindKey = "LINK_BONUS_EGG";
                break;
            default:
                Debug.LogError("Unit Kind Error!: kind:" + nBonusRace);
                break;
        }

        //--------------------------------
        // テキスト構築
        //--------------------------------
        strRaceBonus = string.Format(UnityUtil.GetText(strKindKey), strBonusVal1, strBonusVal2);

        return (strRaceBonus);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リンクシステム：リンクユニットの+値ボーナス
		@param[in]	int		(nPlusValue)	プラス値
		@param[in]	int		(nBonusType)	ボーナスタイプ
		@return		int		(nResult)		[+加算値]
		@note		HPとATKの+値ボーナス	※引数でまとめて受け取った方がいいかも
	*/
    //----------------------------------------------------------------------------
    static public int GetLinkUnitBonusPlus(int nPlusValue, CharaUtil.VALUE nBonusType)
    {
        int nResult = 0;

        //--------------------------------
        // ボーナス値の種類による分岐
        //--------------------------------
        switch (nBonusType)
        {
            case CharaUtil.VALUE.POW: nResult = nPlusValue * GlobalDefine.PLUS_LINK_RATE_POW; break;
            case CharaUtil.VALUE.HP: nResult = nPlusValue * GlobalDefine.PLUS_LINK_RATE_HP; break;
        }

        return (nResult);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ダイアログ表示情報更新
		@param[in]	PacketStructUnit	cUnitData	ユニット情報
		@return		ユニット情報から設定したスプライト名情報が帰る
					nullが入ってきた場合はメインがnullの状態はほぼ無いはずなので、
					リンクユニットがnonの状態のスプライト名を返す
	*/
    //----------------------------------------------------------------------------
    static public string GetLinkIconSpriteName(PacketStructUnit cUnitData)
    {
        string retVal = "mm_link_";
        // nullが入ってきた場合リンクユニットがnonの状態のスプライト名を返す
        if (cUnitData == null)
        {
            retVal += "link_non";
            return retVal;
        }
        // リンク情報によって分岐
        if (cUnitData.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK)
        {
            // リンクユニットの場合
            retVal += "link_";
        }
        else
        {
            // ベースユニットの場合
            // リンクをしていないユニットもベース扱いで返す
            retVal += "base_";
        }
        // ユニットの属性によって分岐
        MasterDataParamChara cCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(cUnitData.id);
        if (cCharaMaster == null)
        {
            // nullならnonをつけておく
            retVal += "non";
        }
        else
        {
            // 属性により振り分け
            switch (cCharaMaster.element)
            {
                case MasterDataDefineLabel.ElementType.NAUGHT:  //!< 属性：無
                    retVal += "naught";
                    break;
                case MasterDataDefineLabel.ElementType.FIRE:    //!< 属性：炎
                    retVal += "fire";
                    break;
                case MasterDataDefineLabel.ElementType.WATER:   //!< 属性：水
                    retVal += "water";
                    break;
                case MasterDataDefineLabel.ElementType.LIGHT:   //!< 属性：光
                    retVal += "light";
                    break;
                case MasterDataDefineLabel.ElementType.DARK:    //!< 属性：闇
                    retVal += "dark";
                    break;
                case MasterDataDefineLabel.ElementType.WIND:    //!< 属性：風
                    retVal += "wind";
                    break;
                default:
                    retVal += "non";
                    break;
            }
        }
        return retVal;
    }
}
///////////////////////////////////////EOF///////////////////////////////////////