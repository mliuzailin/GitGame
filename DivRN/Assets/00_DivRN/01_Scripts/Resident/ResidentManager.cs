/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	ResidentManager.cs
	@brief	常駐物管理クラス
	@author Developer
	@date 	2012/10/04

	ResidentManagerはSceneResidentによって生成される。
	シーン管理の処理により、SceneResidentが読み込まれる前に存在する可能性のあるクラスはSceneManager関連のものに制限される。
	
	ここで各マネージャの生成破棄を管理することで、ゲーム中のインスタンスの存在を保証する
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
	@brief	常駐物管理クラス
*/
//----------------------------------------------------------------------------
public class ResidentManager : SingletonComponent<ResidentManager>
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	各常駐マネージャのインスタンス生成保持
		@note	インスタンスの保持は行うが更新関数呼び出し等は行わない。生成破棄のみを保証する
	*/
    //----------------------------------------------------------------------------
#if BUILD_TYPE_DEBUG
#if DEBUG_LOG
	[HideInInspector]	private	MemoryLogManager			m_ManagerMemoryLog				= null;
#endif // DEBUG_LOG
#endif

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();

        //--------------------------------
        //	回転制御許可設定
        //--------------------------------
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;

#if true
        //--------------------------------
        // ビルド情報をログ出力。
        // 
        // マスター版で焼いた際にログが出なくて、
        // それぞれのシンボル情報の確証が得にくいので情報として出しておく
        //--------------------------------
        string strApplicationSymbolData = "BuildParam - ";

#if UNITY_IPHONE
		strApplicationSymbolData += " Platform:iOS , ";
#elif UNITY_ANDROID
        strApplicationSymbolData += " Platform:Android , ";
#else
		strApplicationSymbolData += " Platform:????? , ";
#endif

#if BUILD_TYPE_DEBUG
        strApplicationSymbolData += " BuildType:debug , ";
#else
		strApplicationSymbolData += "";
#endif

#if STORE_SIMULATION
        strApplicationSymbolData += " Store:disable , ";
#else
		strApplicationSymbolData += " Store:enable , ";
#endif

#if _MASTER_BUILD
		strApplicationSymbolData += " Master:On , ";
#else
        strApplicationSymbolData += " Master:Off , ";
#endif

        UnityEngine.Debug.Log(strApplicationSymbolData);
#endif

#if BUILD_TYPE_DEBUG && UNITY_EDITOR
        //--------------------------------
        // 
        //--------------------------------
        Application.runInBackground = true;
#endif // #if BUILD_TYPE_DEBUG && UNITY_EDITOR


        //--------------------------------
        // staticパラメータのクリア
        //--------------------------------
        ResidentParam.ParamReset();

        //--------------------------------
        // マルチタッチ無効
        //--------------------------------
        Input.multiTouchEnabled = false;

        //--------------------------------
        // 負荷軽減モードの適用分岐
        //--------------------------------
        if (SystemUtil.GetLoadReductionMode() == true)
        {
            ResidentParam.m_OptionWorkLoadLevel = 1;
        }
        else
        {
            ResidentParam.m_OptionWorkLoadLevel = 0;
        }

#if BUILD_TYPE_DEBUG
#if DEBUG_LOG
		m_ManagerMemoryLog		= MemoryLogManager.getInstance();
#endif // DEBUG_LOG
#endif

        //--------------------------------
        // ローカルのセーブ情報をユーザークラスに譲渡するためロード実行
        //--------------------------------
        if (LocalSaveManager.Instance != null)
        {
            // 2015/07/15 k_iida V280 招待機能先送りに伴い封印する V290復活予定.
            //			LocalSaveStorage.Instance.InitLocalSaveStorage( "DivineGate" );
            LocalSaveManager.SaveVersionSafety();
            LocalSaveManager.Instance.LoadFuncUUID();
            LocalSaveManager.Instance.LoadFuncLocalData();
            //			LocalSaveManager.Instance.LoadFuncRestore();
        }
        else
        {
            Debug.LogError("LocalSaveManager None!");
        }

        //--------------------------------
        // Prime31 PlayGameServices 初期化処理
        //--------------------------------
        PlayGameServiceUtil.InitPlayGameService();

        //--------------------------------
        // サーバープッシュ通知初期化
        // @add Developer 2016/10/31
        //--------------------------------
        RemoteNotificationManager.Init();
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：インスタンス制御関連：インスタンス破棄時に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		アプリケーションポーズ時のイベント処理
	*/
    //----------------------------------------------------------------------------
    void OnApplicationPause(bool pause)
    {
        LocalNotificationUtil.OnAppPause(pause);
    }

#if UNITY_EDITOR
    //----------------------------------------------------------------------------
    /*!
		@brief		アプリケーションフォーカス時のイベント処理
	*/
    //----------------------------------------------------------------------------
    void OnApplicationFocus(bool focus)
    {
        LocalNotificationUtil.OnAppFocus(focus);
    }
#endif // #if UNITY_EDITOR
}

