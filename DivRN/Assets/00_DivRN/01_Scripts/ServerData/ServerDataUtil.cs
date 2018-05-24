/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	ServerDataUtil.cs
	@brief	サーバー通信関連ユーティリティ
	@author Developer
	@date 	2013/05/28
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

using LitJson;

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
	@brief	サーバー通信関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class ServerDataUtil
{
    //新クエストは100000以降
    //const uint NewQuestIdStart = 100000;

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	クエストクリアフラグを付け足し
	*/
    //----------------------------------------------------------------------------
    static public bool AddBitFlag(ref byte[] aunBitFlag, uint unSaveID)
    {
        //--------------------------------
        // クエスト番号からセーブ領域算出
        //--------------------------------
        uint unAccessNum = unSaveID / 8;
        byte byAccessBF = (byte)(unSaveID % 8);

        //--------------------------------
        // 領域が足りないなら追加
        //--------------------------------
        if (aunBitFlag == null
        || aunBitFlag.Length <= unAccessNum
        )
        {
            Debug.LogError("BitFlag Buffer Over! - " + unSaveID);

            byte[] aunQuestClearBFNew = new byte[unAccessNum + 1];
            for (int i = 0; i < aunQuestClearBFNew.Length; i++)
            {
                aunQuestClearBFNew[i] = 0;
            }
            if (aunBitFlag != null)
            {
                for (int i = 0; i < aunBitFlag.Length; i++)
                {
                    aunQuestClearBFNew[i] = aunBitFlag[i];
                }
            }
            aunBitFlag = aunQuestClearBFNew;
        }

        //--------------------------------
        // ビットフラグを追加設定
        //--------------------------------
        if ((aunBitFlag[unAccessNum] & (byte)(1 << byAccessBF)) != 0)
        {
            //--------------------------------
            // すでにフラグが立っているなら変化無し
            //--------------------------------
            return false;
        }
        aunBitFlag[unAccessNum] += (byte)(1 << byAccessBF);
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aunBitFlag"></param>
    /// <param name="unSaveID"></param>
    /// <returns></returns>
    static public bool RemoveRenewBitFlag(ref byte[] aunBitFlag, uint unSaveID)
    {
        //新クエストは100000以降になる予定なので引いておく
        if (unSaveID >= GlobalDefine.RENEW_QUEST_OFFSET)
        {
            unSaveID -= GlobalDefine.RENEW_QUEST_OFFSET;
        }

        return RemoveBitFlag(ref aunBitFlag, unSaveID);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ビットフラグを削除
	*/
    //----------------------------------------------------------------------------
    static public bool RemoveBitFlag(ref byte[] aunBitFlag, uint unSaveID)
    {
        //--------------------------------
        // クエスト番号からセーブ領域算出
        //--------------------------------
        uint unAccessNum = unSaveID / 8;
        byte byAccessBF = (byte)(unSaveID % 8);

        //--------------------------------
        // 領域が足りないなら追加
        //--------------------------------
        if (aunBitFlag == null
        || aunBitFlag.Length <= unAccessNum
        )
        {
            Debug.LogError("BitFlag Buffer Over! - " + unSaveID);

            byte[] aunQuestClearBFNew = new byte[unAccessNum + 1];
            for (int i = 0; i < aunQuestClearBFNew.Length; i++)
            {
                aunQuestClearBFNew[i] = 0;
            }
            if (aunBitFlag != null)
            {
                for (int i = 0; i < aunBitFlag.Length; i++)
                {
                    aunQuestClearBFNew[i] = aunBitFlag[i];
                }
            }
            aunBitFlag = aunQuestClearBFNew;
        }

        //--------------------------------
        // ビットフラグを追加設定
        //--------------------------------
        if ((aunBitFlag[unAccessNum] & (byte)(1 << byAccessBF)) == 0)
        {
            //--------------------------------
            // すでにフラグが立っているなら変化無し
            //--------------------------------
            return false;
        }
        aunBitFlag[unAccessNum] -= (byte)(1 << byAccessBF);
        return true;
    }


    /// <summary>
    /// ビットフラグのONOFFチェック(新クエスト用)
    /// </summary>
    /// <param name="aunBitFlag"></param>
    /// <param name="unSaveID"></param>
    /// <returns></returns>
    static public bool ChkRenewBitFlag(ref byte[] aunBitFlag, uint unSaveID)
    {
        //新クエストは100000以降になる予定なので引いておく
        if (unSaveID >= GlobalDefine.RENEW_QUEST_OFFSET)
        {
            unSaveID -= GlobalDefine.RENEW_QUEST_OFFSET;
        }

        return ChkBitFlag(ref aunBitFlag, unSaveID);
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ビットフラグのONOFFチェック
	*/
    //----------------------------------------------------------------------------
    static public bool ChkBitFlag(ref byte[] aunBitFlag, uint unSaveID)
    {
        //--------------------------------
        // クエスト番号からセーブ領域算出
        //--------------------------------
        uint unAccessNum = unSaveID / 8;
        byte byAccessBF = (byte)(unSaveID % 8);

        //--------------------------------
        // 領域が足りないならフラグ無し
        //--------------------------------
        if (aunBitFlag == null
        || aunBitFlag.Length <= unAccessNum
        )
        {
            Debug.LogError("BitFlag Buffer Over! - " + unSaveID);
            return false;
        }

        //--------------------------------
        // ビットフラグが立ってるかチェック
        //--------------------------------
        return ((aunBitFlag[unAccessNum] & (byte)(1 << byAccessBF)) != 0);
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ビットフラグの数取得
	*/
    //----------------------------------------------------------------------------
    static public uint GetBitFlagCt(ref byte[] aunBitFlag)
    {
        //--------------------------------
        // 領域が足りないならフラグ無し
        //--------------------------------
        if (aunBitFlag == null)
        {
            return 0;
        }

        //--------------------------------
        // ビットフラグが立っている数を算出
        //--------------------------------
        uint unTotalFlagCt = 0;
        for (int i = 0; i < aunBitFlag.Length; i++)
        {
            if ((aunBitFlag[i] & (byte)(1 << 0)) != 0) { unTotalFlagCt++; }
            if ((aunBitFlag[i] & (byte)(1 << 1)) != 0) { unTotalFlagCt++; }
            if ((aunBitFlag[i] & (byte)(1 << 2)) != 0) { unTotalFlagCt++; }
            if ((aunBitFlag[i] & (byte)(1 << 3)) != 0) { unTotalFlagCt++; }
            if ((aunBitFlag[i] & (byte)(1 << 4)) != 0) { unTotalFlagCt++; }
            if ((aunBitFlag[i] & (byte)(1 << 5)) != 0) { unTotalFlagCt++; }
            if ((aunBitFlag[i] & (byte)(1 << 6)) != 0) { unTotalFlagCt++; }
            if ((aunBitFlag[i] & (byte)(1 << 7)) != 0) { unTotalFlagCt++; }
        }
        return unTotalFlagCt;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	
	*/
    //----------------------------------------------------------------------------
    static public int GetPacketUnitCost(PacketStructUnit cPacketUnit)
    {
        if (cPacketUnit == null)
        {
            return 0;
        }

        MasterDataParamChara cCharaParam = MasterDataUtil.GetCharaParamFromID(cPacketUnit.id);
        if (cCharaParam == null)
        {
            return 0;
        }

        return cCharaParam.party_cost;
    }

    static public string GetServerName()
    {
        string strName = "";
#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
        string strServerAddress = LocalSaveManager.Instance.LoadFuncServerAddressIP();
        strName = GetServerName(strServerAddress);
#endif
        return strName;
    }

    static public string GetServerName(string strServerAddress)
    {
        string strName = "";
#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
        switch (strServerAddress)
        {
            case GlobalDefine.API_TEST_ADDRESS_DEVELOP_0_GOE:
                strName = "EXANPLE_DEVELOP_0";
                break;
            case GlobalDefine.API_TEST_ADDRESS_DEVELOP_1_NEW_GOE:
                strName = "EXANPLE_DEVELOP_1";
                break;
            case GlobalDefine.API_TEST_ADDRESS_DEVELOP_1_IP_GOE:
                strName = "EXANPLE_DEVELOP_1_IP";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_0_NEW_GOE:
                strName = "EXANPLE_STAGING_0";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_0_IP_GOE:
                strName = "EXANPLE_STAGING_0_IP";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_1_NEW_GOE:
                strName = "EXANPLE_STAGING_1";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_1_IP_GOE:
                strName = "EXANPLE_STAGING_1_IP";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_2a_GOE:
                strName = "EXANPLE_STAGING_2a";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_2b_GOE:
                strName = "EXANPLE_STAGING_2b";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_2c_GOE:
                strName = "EXANPLE_STAGING_2c";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_3a_GOE:
                strName = "EXANPLE_STAGING_3a";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_3b_GOE:
                strName = "EXANPLE_STAGING_3b";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_3c_GOE:
                strName = "EXANPLE_STAGING_3c";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE:
                strName = "EXANPLE_REVIEW";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_REVIEW_IP_GOE:
                strName = "EXANPLE_REVIEW_IP";
                break;
            case GlobalDefine.API_TEST_ADDRESS_ONLINE:
                strName = "EXANPLE_PROD";
                break;
            case GlobalDefine.API_TEST_ADDRESS_STAGING_REHEARSAL_GOE:
                strName = "EXANPLE_REHEARSAL";
                break;
            case GlobalDefine.API_TEST_ADDRESS_LOCAL_GOE:
                strName = "EXANPLE_LOCAL";
                break;
        }
#endif
        return strName;
    }
}



/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
