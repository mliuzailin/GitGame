/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	RemoteNotificationManager.cs
	@brief	サーバー通知
	@author Developer
	@date 	2016/10/03
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
	@brief	サーバー通知
*/
//----------------------------------------------------------------------------
public static class RemoteNotificationManager
{
    /*==========================================================================*/
    /*		define																*/
    /*==========================================================================*/
    // Android用
#if UNITY_ANDROID
    private const string GOE_GCM_JAVA_CLASS = ("jp.example.goe.gcm");                        //!< gcmプラグイン：ルート
    private const string GCM_CLASS_RECEPTION = (GOE_GCM_JAVA_CLASS + ".GOEUnityGCM");       //!< gcmプラグイン：クラス名：窓口
    private const string GCM_CLASS_CALLBACK = (GOE_GCM_JAVA_CLASS + ".CallbacksToUnity");   //!< gcmプラグイン：クラス名：コールバック
    private const string GCM_INSTANCE = ("instance");                                   //!< gcmプラグイン：関数：インスタンス
    private const string GCM_INITIALIZE = ("initialize");                               //!< gcmプラグイン：関数：初期化
    private const string GCM_REQ_REGISTER_ID = ("registerGCM");                             //!< gcmプラグイン：関数：RegistrationIDリクエスト
#if _MASTER_BUILD
	public	const string SENDER_ID			  = ( "" );								//!< SenderID(プロジェクト番号：Androidで通知サーバーに必要なパラメーター)
#else
    public const string SENDER_ID_DEBUG = ("");                             //!< SenderID：debug版(プロジェクト番号：Androidで通知サーバーに必要なパラメーター)
#endif // _MASTER_BUILD

    // iOS用
#elif UNITY_IOS
#endif// UNITY_ANDROID or UNITY_IOS

    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス
	*/
    //----------------------------------------------------------------------------
    private const int SEQ_NONE = 0;         //!< シーケンス：処理なし
    private const int SEQ_REQ_DEVICE_ID = 1;            //!< シーケンス：デバイスIDリクエスト
    private const int SEQ_GET_DEVICE_ID = 2;            //!< シーケンス：デバイスID取得
    private const int SEQ_REQ_SEND = 3;         //!< シーケンス：送信リクエスト
    private const int SEQ_FINISH = 4;           //!< シーケンス：終了処理
    private const int SEQ_ERROR = 5;            //!< シーケンス：エラー対応

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    // Android用
#if UNITY_ANDROID
    private static AndroidJavaObject m_Plugin;          //!< java objectのinstance

    // iOS用
#elif UNITY_IOS
#endif// UNITY_ANDROID or UNITY_IOS

    private static string m_DeviceID;       //!< 端末ID(Android = registration_id, iOS = device_token) ※プラットフォームで呼び方が違う
    private static string m_SaveDeviceID;   //!< 端末ID(再取得時の比較用)
    private static int m_SeqStep;           //!< シーケンス


    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	リモート通知：初期化
		@note	プラットフォーム毎に処理を分ける
	*/
    //----------------------------------------------------------------------------
    public static void Init()
    {
        // デバッグログ
        //-----------------------
        // 共通
        //-----------------------
        m_DeviceID = "";
        m_SaveDeviceID = "";
        m_SeqStep = SEQ_GET_DEVICE_ID;

        //-----------------------
        // プラットフォーム別
        //-----------------------
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

#elif UNITY_ANDROID
		InitGCM();
#elif UNITY_IOS
		InitAPNS();
#endif// UNITY_ANDROID or UNITY_IOS
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リモート通知：開始
		@note	プラットフォーム毎に処理を分ける
	*/
    //----------------------------------------------------------------------------
    public static bool RequestDeviceID()
    {
        //-----------------------
        // 処理中の場合
        //-----------------------
        if (m_SeqStep != SEQ_NONE)
        {
            return false;
        }

        // デバイスIDリクエストへ
        m_SeqStep = SEQ_REQ_DEVICE_ID;
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リモート通知：更新
		@note	処理中はtrueを返す
	*/
    //----------------------------------------------------------------------------
    public static bool UpdateProcess()
    {
        //-----------------------
        // 処理なしの場合
        //-----------------------
        if (m_SeqStep == SEQ_NONE)
        {
            return false;
        }

        //-----------------------
        // シーケンス処理
        //-----------------------
        switch (m_SeqStep)
        {
            case SEQ_REQ_DEVICE_ID: ExecStepRequestDeviceID(); break;       //!< シーケンス：デバイスIDリクエスト
            case SEQ_GET_DEVICE_ID: ExecStepGetDeviceID(); break;       //!< シーケンス：デバイスID取得
            case SEQ_REQ_SEND: ExecStepRequestSend(); break;        //!< シーケンス：送信リクエスト
            case SEQ_FINISH: ExecStepFinish(); break;       //!< シーケンス：終了処理
            case SEQ_ERROR: ExecStepError(); break;     //!< シーケンス：エラー対応
            default: break;
        }

        return true;
    }

    // Android用
#if UNITY_ANDROID
    //----------------------------------------------------------------------------
    /*!
		@brief	GCMの初期化
		@note	GCM = Google Cloud Messaging for Androidのこと
				Android用
	*/
    //----------------------------------------------------------------------------
    private static void InitGCM()
    {
        NotificationCallBack cCallback = new NotificationCallBack();
        using (AndroidJavaClass pluginClass = new AndroidJavaClass(GCM_CLASS_RECEPTION))
        {
            //-----------------------
            // static instance取得(java objectのinstance)
            //-----------------------
            m_Plugin = pluginClass.CallStatic<AndroidJavaObject>(GCM_INSTANCE);

            //-----------------------
            // SenderID設定(Master or Debugで分岐)
            // ※本来はパッケージ名で分岐したほうがいい
            //-----------------------
            string sSenderID;
#if _MASTER_BUILD
			sSenderID = SENDER_ID;
#else
            sSenderID = SENDER_ID_DEBUG;
#endif // _MASTER_BUILD

            //-----------------------
            // 初期化
            //-----------------------
            bool bEnablePush = true;
            m_Plugin.Call(GCM_INITIALIZE, sSenderID, bEnablePush, cCallback);

            //-----------------------
            // registration idリクエスト(起動初回は、リクエストも行っておく)
            // OnRegisterPushSucceeded又は、OnRegisterPushFailedが呼ばれる
            // ※初期化と、リクエストで区分けしたい気もあるが、
            // 　1フレ余計にかかるのと、コールバックの遅さを懸念
            //-----------------------
            m_Plugin.Call(GCM_REQ_REGISTER_ID);

            // デバッグログ
#if BUILD_TYPE_DEBUG
            //			DebugLogger.StatAdd( "[DeviceID:InitGCM_OK]" );
#endif // BUILD_TYPE_DEBUG
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	registration_idリクエスト
		@note	Android用
	*/
    //----------------------------------------------------------------------------
    private static void RequestRegistrationID()
    {
        // registration idリクエスト
        // OnRegisterPushSucceeded又は、OnRegisterPushFailedが呼ばれる
        m_Plugin.Call(GCM_REQ_REGISTER_ID);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	通知コールバック・registration_id取得/失敗
		@note	Android用
	*/
    //----------------------------------------------------------------------------
    private class NotificationCallBack : AndroidJavaProxy
    {
        public NotificationCallBack() : base(GCM_CLASS_CALLBACK)
        {
        }

        //-----------------------
        // registration id取得成功時
        //-----------------------
        public void OnRegisterPushSucceeded(string sRegID)
        {
            // デバイスIDのチェック
            if (m_SaveDeviceID != ""
            && m_SaveDeviceID == sRegID)
            {
                // IDに変更がないため、処理なしへ
                m_SeqStep = SEQ_NONE;
                return;
            }

            // registration id取得
            m_DeviceID = sRegID;

            // デバッグログ
#if BUILD_TYPE_DEBUG
            Debug.Log("registration id:" + m_DeviceID);
            //			DebugLogger.StatAdd( "[DeviceID:" + m_DeviceID + "]" );
#endif // BUILD_TYPE_DEBUG
        }

        //-----------------------
        // registration id取得失敗時
        //-----------------------
        public void OnRegisterPushFailed(string sErrMsg)
        {
            // デバッグログ
#if BUILD_TYPE_DEBUG
            Debug.Log("fail to obtain registration id:" + sErrMsg);
            //			DebugLogger.StatAdd( "[DeviceID:" + sErrMsg + "]" );
#endif // BUILD_TYPE_DEBUG
        }
    }

    // iOS用
#elif UNITY_IOS
	//----------------------------------------------------------------------------
	/*!
		@brief	APN初期化・device_tokenリクエスト
		@note	APNS = Apple Push Notification Serviceのこと
				iOS用
	*/
	//----------------------------------------------------------------------------
	private static void InitAPNS()
	{
		UnityEngine.iOS.NotificationServices.RegisterForNotifications( UnityEngine.iOS.NotificationType.Alert
															   | UnityEngine.iOS.NotificationType.Badge
															   | UnityEngine.iOS.NotificationType.Sound );
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	device_token取得/失敗
		@note	いつ届くか分からない(フックできない)ため、
				Update関数で毎フレームチェックが必要
				iOS用
	*/
	//----------------------------------------------------------------------------
	private static void UpdateAPNS()
	{
		// device_tokenを取得
		byte[] abyToken = UnityEngine.iOS.NotificationServices.deviceToken;
		if( abyToken == null )
		{
			return;
		}
		
		// バイト配列の各要素の数値を等価の 16 進数文字列形式に変換
		// 連続した文字列に置き換える
		string sTempDeviceID = System.BitConverter.ToString( abyToken ).Replace( "-", "" );

		// デバイスIDのチェック
		if( m_SaveDeviceID != ""
		&&	m_SaveDeviceID == sTempDeviceID )
		{
			// IDに変更がないため、処理なしへ
			m_SeqStep = SEQ_NONE;
			return;
		}

		// device_token取得
		m_DeviceID = sTempDeviceID;

		// デバッグログ
#if BUILD_TYPE_DEBUG
		Debug.Log( "device token:" + m_DeviceID );
//		DebugLogger.StatAdd( "[DeviceID:" + m_DeviceID + "]" );
#endif // BUILD_TYPE_DEBUG
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	バッチの削除：アプリアイコンに付く数字
		@note	自動的に加減しないため、明示的に記述する
				iOS用
	*/
	//----------------------------------------------------------------------------
	public static void DeleteBatch()
	{
		UnityEngine.iOS.LocalNotification cSetCountNotification = new UnityEngine.iOS.LocalNotification();

		// 設定
		cSetCountNotification.fireDate					 = System.DateTime.Now;
		cSetCountNotification.applicationIconBadgeNumber = -1;						// 0だとバッチが消えない
		cSetCountNotification.hasAction					 = false;

		// 更新
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification( cSetCountNotification);
	}
#endif// UNITY_ANDROID or UNITY_IOS

    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス：デバイスIDリクエスト
		@note	
	*/
    //----------------------------------------------------------------------------
    private static void ExecStepRequestDeviceID()
    {
        // プラットフォーム別
#if UNITY_ANDROID && !UNITY_EDITOR
		RequestRegistrationID();
#elif UNITY_IOS
		// iOSは、リクエストがない
#endif// UNITY_ANDROID or UNITY_IOS

        // デバイスID取得へ
        m_SeqStep = SEQ_GET_DEVICE_ID;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス：デバイスID取得
		@note	
	*/
    //----------------------------------------------------------------------------
    private static void ExecStepGetDeviceID()
    {
        // プラットフォーム別
#if UNITY_ANDROID
        // Androidは、コールバック関数で取得
#elif UNITY_IOS
		UpdateAPNS();
#endif// UNITY_ANDROID or UNITY_IOS

        // デバイスIDが取得できていない場合
        if (m_DeviceID == "")
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            m_SeqStep = SEQ_NONE;
#endif// UNITY_EDITOR
            return;
        }

        // 送信リクエストへ
        m_SeqStep = SEQ_REQ_SEND;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス：送信リクエスト
		@note	
	*/
    //----------------------------------------------------------------------------
    private static void ExecStepRequestSend()
    {
        //----------------------------------------
        // APIパケットを送信リクエスト
        // ※受信する必要はない
        //----------------------------------------
        ServerDataUtilSend.SendPacketAPI_PeriodicUpdate(m_DeviceID).SendStart();

        // 処理終了へ
        m_SeqStep = SEQ_FINISH;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス：終了処理
		@note	正常に一連の処理が終了した場合に通る(IDを保存しているため)
	*/
    //----------------------------------------------------------------------------
    private static void ExecStepFinish()
    {
        m_SaveDeviceID = m_DeviceID;        // デバイスIDを保存
        m_DeviceID = "";                // 送信済みIDをクリア
        m_SeqStep = SEQ_NONE;          // 処理をしないステップに設定

        return;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス：エラー処理
		@note	
	*/
    //----------------------------------------------------------------------------
    private static void ExecStepError()
    {
        return;
    }

}
///////////////////////////////////////EOF///////////////////////////////////////