/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	SystemUtil.cs
	@brief	システム関連ユーティリティ
	@author Developer
	@date 	2013/08/06
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
	@brief	システム関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class SystemUtil
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	端末依存のデフォルトクオリティレベル取得
	*/
    //----------------------------------------------------------------------------
    static public int GetQualityLevel()
    {
        //--------------------------------
        // Unityの実行時生成時にクオリティを固定化して端末確認したいときがあるので、
        // 定義したら強制的にクオリティを設定するよう対応
        //--------------------------------
#if QUALITY_FIX_1_1
		return GlobalDefine.QUALITY_1_1;
#endif
#if QUALITY_FIX_1_2
		return GlobalDefine.QUALITY_1_2;
#endif
#if QUALITY_FIX_1_4
		return GlobalDefine.QUALITY_1_4;
#endif

        //--------------------------------
        // クオリティレベルを求める
        //--------------------------------
        int nQualityLevel = GlobalDefine.QUALITY_1_2;
#if UNITY_IPHONE
		//--------------------------------
		// iOSは機種タイプから判別
		//--------------------------------
		switch( UnityEngine.iOS.Device.generation )
		{
			case UnityEngine.iOS.DeviceGeneration.Unknown:			nQualityLevel = GlobalDefine.QUALITY_1_2;	break;
			case UnityEngine.iOS.DeviceGeneration.iPhone:			nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 元々1/4指定。ただ1/4のとこに1/2のやつ入れてたので実質変化無し
			case UnityEngine.iOS.DeviceGeneration.iPhone3G:			nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 元々1/4指定。ただ1/4のとこに1/2のやつ入れてたので実質変化無し
			case UnityEngine.iOS.DeviceGeneration.iPhone3GS:		nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 元々1/4指定。ただ1/4のとこに1/2のやつ入れてたので実質変化無し
			case UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen:	nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 元々1/4指定。ただ1/4のとこに1/2のやつ入れてたので実質変化無し
			case UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen:	nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 元々1/4指定。ただ1/4のとこに1/2のやつ入れてたので実質変化無し
			case UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen:	nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 元々1/4指定。ただ1/4のとこに1/2のやつ入れてたので実質変化無し
			case UnityEngine.iOS.DeviceGeneration.iPad1Gen:			nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 元々1/4指定。ただ1/4のとこに1/2のやつ入れてたので実質変化無し
			case UnityEngine.iOS.DeviceGeneration.iPhone4:			nQualityLevel = GlobalDefine.QUALITY_1_2;	break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen:	nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 元々1/4指定。ただ1/4のとこに1/2のやつ入れてたので実質変化無し
			case UnityEngine.iOS.DeviceGeneration.iPad2Gen:			nQualityLevel = GlobalDefine.QUALITY_1_2;	break;
			case UnityEngine.iOS.DeviceGeneration.iPhone4S:			nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 1090からデフォルトでは高にならないようにする
			case UnityEngine.iOS.DeviceGeneration.iPad3Gen:			nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 1090からデフォルトでは高にならないようにする
			case UnityEngine.iOS.DeviceGeneration.iPhone5:			nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 1090からデフォルトでは高にならないようにする
			case UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen:	nQualityLevel = GlobalDefine.QUALITY_1_2;	break;
			case UnityEngine.iOS.DeviceGeneration.iPadMini1Gen:		nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 1090からデフォルトでは高にならないようにする
			case UnityEngine.iOS.DeviceGeneration.iPad4Gen:			nQualityLevel = GlobalDefine.QUALITY_1_2;	break;		// 1090からデフォルトでは高にならないようにする
			case UnityEngine.iOS.DeviceGeneration.iPhoneUnknown:	nQualityLevel = GlobalDefine.QUALITY_1_2;	break;
			case UnityEngine.iOS.DeviceGeneration.iPadUnknown:		nQualityLevel = GlobalDefine.QUALITY_1_2;	break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouchUnknown:	nQualityLevel = GlobalDefine.QUALITY_1_2;	break;
		}

#if BUILD_TYPE_DEBUG
		Debug.Log( "Generation Quality - " + UnityEngine.iOS.Device.generation + " , " + nQualityLevel );
#endif
#endif

#if UNITY_ANDROID
        //--------------------------------
        // Androidは機種有りすぎて判別付かない。
        // クオリティが高いほどメモリを食うんでGPUサイズで分岐。
        //--------------------------------
        int nMemorySize = SystemInfo.systemMemorySize;
        if (nMemorySize < 200) { nQualityLevel = GlobalDefine.QUALITY_1_4; }
        else if (nMemorySize < 800) { nQualityLevel = GlobalDefine.QUALITY_1_2; }
        //		else{								nQualityLevel = GlobalDefine.QUALITY_1_1;	}
        else { nQualityLevel = GlobalDefine.QUALITY_1_2; }  // 1090からデフォルトでは高にならないようにする
#if true
#if BUILD_TYPE_DEBUG
        Debug.Log("Memory Quality - " + nMemorySize + "(MB) , GPU:" + SystemInfo.graphicsMemorySize + "(MB) ,  ... " + nQualityLevel);
#endif
#endif

#endif
        return nQualityLevel;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	端末依存の負荷軽減有無判定
	*/
    //----------------------------------------------------------------------------
    static public bool GetLoadReductionMode()
    {
        //--------------------------------
        // 負荷軽減の有無を求める
        //--------------------------------
#if UNITY_IPHONE
		//--------------------------------
		// iOSは機種タイプから判別
		//--------------------------------
		bool bLoadReductionMode = false;
		switch( UnityEngine.iOS.Device.generation )
		{
			case UnityEngine.iOS.DeviceGeneration.Unknown:				bLoadReductionMode = false;		break;
			case UnityEngine.iOS.DeviceGeneration.iPhone:				bLoadReductionMode = true;		break;
			case UnityEngine.iOS.DeviceGeneration.iPhone3G:				bLoadReductionMode = true;		break;
			case UnityEngine.iOS.DeviceGeneration.iPhone3GS:			bLoadReductionMode = true;		break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen:		bLoadReductionMode = true;		break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen:		bLoadReductionMode = true;		break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen:		bLoadReductionMode = true;		break;
			case UnityEngine.iOS.DeviceGeneration.iPad1Gen:				bLoadReductionMode = true;		break;
			case UnityEngine.iOS.DeviceGeneration.iPhone4:				bLoadReductionMode = true;		break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen:		bLoadReductionMode = true;		break;
			case UnityEngine.iOS.DeviceGeneration.iPad2Gen:				bLoadReductionMode = false;		break;
			case UnityEngine.iOS.DeviceGeneration.iPhone4S:				bLoadReductionMode = false;		break;
			case UnityEngine.iOS.DeviceGeneration.iPad3Gen:				bLoadReductionMode = false;		break;
			case UnityEngine.iOS.DeviceGeneration.iPhone5:				bLoadReductionMode = false;		break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen:		bLoadReductionMode = false;		break;
			case UnityEngine.iOS.DeviceGeneration.iPadMini1Gen:			bLoadReductionMode = false;		break;
			case UnityEngine.iOS.DeviceGeneration.iPad4Gen:				bLoadReductionMode = false;		break;
			case UnityEngine.iOS.DeviceGeneration.iPhoneUnknown:		bLoadReductionMode = false;		break;
			case UnityEngine.iOS.DeviceGeneration.iPadUnknown:			bLoadReductionMode = false;		break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouchUnknown:		bLoadReductionMode = false;		break;
		}
		return bLoadReductionMode;
#elif UNITY_ANDROID
        //--------------------------------
        // Androidは機種有りすぎて判別付かない。
        // 
        // OSが古いほど性能が低いと仮定して一定ランク以下は負荷軽減モードを適用
        //--------------------------------
        bool bLoadReductionMode = false;

        string strOSVersion = SystemInfo.operatingSystem;
        if (strOSVersion.IndexOf("Android OS 1.0") >= 0) { bLoadReductionMode = true; }
        else if (strOSVersion.IndexOf("Android OS 1.1") >= 0) { bLoadReductionMode = true; }
        else if (strOSVersion.IndexOf("Android OS 1.5") >= 0) { bLoadReductionMode = true; }
        else if (strOSVersion.IndexOf("Android OS 1.6") >= 0) { bLoadReductionMode = true; }
        else if (strOSVersion.IndexOf("Android OS 2.0") >= 0) { bLoadReductionMode = true; }
        else if (strOSVersion.IndexOf("Android OS 2.1") >= 0) { bLoadReductionMode = true; }
        else if (strOSVersion.IndexOf("Android OS 2.2") >= 0) { bLoadReductionMode = true; }
        else if (strOSVersion.IndexOf("Android OS 2.3") >= 0) { bLoadReductionMode = true; }
        else if (strOSVersion.IndexOf("Android OS 3.0") >= 0) { bLoadReductionMode = false; }
        else if (strOSVersion.IndexOf("Android OS 3.1") >= 0) { bLoadReductionMode = false; }
        else if (strOSVersion.IndexOf("Android OS 3.2") >= 0) { bLoadReductionMode = false; }
        else if (strOSVersion.IndexOf("Android OS 4.0") >= 0) { bLoadReductionMode = false; }
        else if (strOSVersion.IndexOf("Android OS 4.1") >= 0) { bLoadReductionMode = false; }
        else if (strOSVersion.IndexOf("Android OS 4.2") >= 0) { bLoadReductionMode = false; }
        else if (strOSVersion.IndexOf("Android OS 4.3") >= 0) { bLoadReductionMode = false; }
        else { bLoadReductionMode = false; }

#if BUILD_TYPE_DEBUG
        Debug.Log("OS Version :" + strOSVersion + " , LoadReduction:" + bLoadReductionMode);
#endif
        return bLoadReductionMode;
#else
		return false;
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ユーザーをグループ分けした番号を取得
	*/
    //----------------------------------------------------------------------------
    static public MasterDataDefineLabel.BelongType GetUserGroup()
    {
        if (UserDataAdmin.Instance == null
        || UserDataAdmin.Instance.m_StructPlayer == null
        || UserDataAdmin.Instance.m_StructPlayer.user == null
        ) return MasterDataDefineLabel.BelongType.A;

        switch ((UserDataAdmin.Instance.m_StructPlayer.user.user_id) % 5)
        {
            case 0: return MasterDataDefineLabel.BelongType.A;
            case 1: return MasterDataDefineLabel.BelongType.B;
            case 2: return MasterDataDefineLabel.BelongType.C;
            case 3: return MasterDataDefineLabel.BelongType.D;
            case 4: return MasterDataDefineLabel.BelongType.E;
        }
        return MasterDataDefineLabel.BelongType.A;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ストア処理を解禁するか否か
		@note	スペックが低くてストアを使うとハングする可能性の高い端末での課金阻止対応用
	*/
    //----------------------------------------------------------------------------
    static public bool GetStoreOpen()
    {
        //--------------------------------
        // 機種別のストア解禁判定
        //--------------------------------
        bool bStoreActive = true;

#if UNITY_IPHONE
		//--------------------------------
		// iOSは機種タイプから判別
		//--------------------------------
		switch( UnityEngine.iOS.Device.generation )
		{
			case UnityEngine.iOS.DeviceGeneration.Unknown:			bStoreActive = true;	break;
			case UnityEngine.iOS.DeviceGeneration.iPhone:			bStoreActive = false;	break;
			case UnityEngine.iOS.DeviceGeneration.iPhone3G:			bStoreActive = false;	break;
			case UnityEngine.iOS.DeviceGeneration.iPhone3GS:		bStoreActive = false;	break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen:	bStoreActive = false;	break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen:	bStoreActive = false;	break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen:	bStoreActive = false;	break;
			case UnityEngine.iOS.DeviceGeneration.iPad1Gen:			bStoreActive = false;	break;
			case UnityEngine.iOS.DeviceGeneration.iPhone4:			bStoreActive = true;	break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen:	bStoreActive = false;	break;
			case UnityEngine.iOS.DeviceGeneration.iPad2Gen:			bStoreActive = true;	break;
			case UnityEngine.iOS.DeviceGeneration.iPhone4S:			bStoreActive = true;	break;
			case UnityEngine.iOS.DeviceGeneration.iPad3Gen:			bStoreActive = true;	break;
			case UnityEngine.iOS.DeviceGeneration.iPhone5:			bStoreActive = true;	break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen:	bStoreActive = true;	break;
			case UnityEngine.iOS.DeviceGeneration.iPadMini1Gen:		bStoreActive = true;	break;
			case UnityEngine.iOS.DeviceGeneration.iPad4Gen:			bStoreActive = true;	break;
			case UnityEngine.iOS.DeviceGeneration.iPhoneUnknown:	bStoreActive = true;	break;
			case UnityEngine.iOS.DeviceGeneration.iPadUnknown:		bStoreActive = true;	break;
			case UnityEngine.iOS.DeviceGeneration.iPodTouchUnknown:	bStoreActive = true;	break;
		}
#endif

        return bStoreActive;
    }

};

/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
