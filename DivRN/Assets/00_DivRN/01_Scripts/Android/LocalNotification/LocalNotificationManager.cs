#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define LOCAL_NOTIFICATION_EDITOR
#elif UNITY_ANDROID
#define LOCAL_NOTIFICATION_ANDROID
#elif UNITY_IPHONE
#define LOCAL_NOTIFICATION_IOS
#endif


using UnityEngine;
#if LOCAL_NOTIFICATION_IOS
using UnityEngine.iOS;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;


//--------------------------------------------------------------------------------
//	class
//--------------------------------------------------------------------------------
//--------------------------------------------------------------------------------
/*!
	@brief		ローカル通知ユーティリティー
*/
//--------------------------------------------------------------------------------
public class LocalNotificationUtil
{

    // ステータスバー表示の関係で、
    // UserDataAdmin.Instance.m_StructPlayer.stamina_nowを、
    // 更新できないので、↓変数に保存する
    static public uint m_StaminaNow = 0;

#if LOCAL_NOTIFICATION_EDITOR
    //----------------------------------------------------------------------------
    //	@brief		アプリケーションフォーカス時イベント処理
    //----------------------------------------------------------------------------
    public static void OnAppFocus(bool focusStatus)
    {

        if (focusStatus == true)
        {
            CancelNotification();
        }
        else
        {
            RegisterNotification();
        }

    }
#endif


    //----------------------------------------------------------------------------
    //	@brief		アプリ一時停止時イベント処理
    //----------------------------------------------------------------------------
    public static void OnAppPause(bool pause)
    {

        if (pause == true)
        {
            RegisterNotification();
        }
        else
        {
            CancelNotification();
        }

    }


    // //----------------------------------------------------------------------------
    // //	@brief		アプリ起動時
    // //----------------------------------------------------------------------------
    // public static void OnAppStart() {
    // 	PQDMLocalNotification.CancelAllNotifications();
    // }


    //----------------------------------------------------------------------------
    //	@brief		ローカル通知登録破棄
    //----------------------------------------------------------------------------
    public static void CancelNotification()
    {
        // マスターから登録、スタミナ通知など
        PQDMLocalNotification.CancelAllNotifications();
    }


    //----------------------------------------------------------------------------
    //	@brief		ローカル通知登録
    //----------------------------------------------------------------------------
    private static void RegisterNotification()
    {

        //------------------------------------------------------------------------
        //	ローカル通知の有効無効
        //------------------------------------------------------------------------
        bool enable_notification = false;

        if (LocalSaveManager.Instance == null)
        {
            return;
        }


        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();

        enable_notification = (cOption.m_OptionNotification == (int)LocalSaveDefine.OptionNotification.ON);

        if (enable_notification == false)
        {
            return;
        }

        if (TimeManager.Instance == null)
        {
            return;
        }

        //------------------------------------------------------------------------
        //	ローカル通知の登録
        //------------------------------------------------------------------------
        int delay = 0;
        bool bSeisekiden = false;

        MasterDataNotification[] notification_param_array = null;
        if (MasterDataUtil.IsMasterDataNotification())
        {
            notification_param_array = MasterDataUtil.GetMasterDataNotification();
        }

        MasterDataNotification notification_param;

        if (notification_param_array.IsNullOrEmpty() == false)
        {

            // パッチテキスト（通知表示しない期間）取得 v360
            string sCancelS = Patcher.Instance.GetLocalNotificationCancelStart();
            string sCancelE = Patcher.Instance.GetLocalNotificationCancelEnd();

            // 通知文言があるやつ
            for (uint i = 0; i < notification_param_array.Length; i++)
            {

                notification_param = notification_param_array[i];
                if (notification_param == null)
                {
                    continue;
                }

                string d = notification_param.timing_start.ToString() + @"00";
                // string f = "yyyyMMddHHmm";
                string f = "yyyyMMddHHmm";
                DateTime dt = DateTime.ParseExact(d, f, null);


                TimeSpan ts = dt.Subtract(TimeManager.Instance.m_TimeNow);
                if (ts.TotalSeconds < 0)
                {
                    // 期限を過ぎたデータは消す MasterDataEvent.fix_idと同じ
                    LocalSaveManager.Instance.RemoveFuncNotificationRequest(notification_param.fix_id);

                    continue;
                }

                //プッシュ通知のタイプが設定でOFFになっているなら弾く
                if ((notification_param.notification_type == MasterDataDefineLabel.NotificationType.EVENT ||
                    notification_param.notification_type == MasterDataDefineLabel.NotificationType.CASINO ||
                    notification_param.notification_type == MasterDataDefineLabel.NotificationType.SEISEKIDEN)
                    && cOption.m_NotificationEvent == (int)LocalSaveDefine.OptionNotificationEvent.OFF)
                {
                    continue;
                }
                if (notification_param.notification_type == MasterDataDefineLabel.NotificationType.SEISEKIDEN)
                {
                    //すでに聖石殿の通知をしていたらその後の聖石殿の通知は弾く
                    if (bSeisekiden == true)
                    {
                        continue;
                    }
                }

                if (sCancelS.IsNOTNullOrEmpty() && sCancelE.IsNOTNullOrEmpty())
                {
                    //イベント開始タイミングがプッシュ通知を表示しない期間内ならはじくv360
                    if (sCancelS.Length == f.Length && sCancelE.Length == f.Length)
                    {
                        //パッチ登録テキストはYYYYMMDDhhmm形式のためそのままdatetimeに変換
                        DateTime dtCancelS = DateTime.ParseExact(sCancelS, f, null);
                        DateTime dtCancelE = DateTime.ParseExact(sCancelE, f, null);
                        if ((dtCancelS != null && dtCancelS <= dt)
                        && (dtCancelE != null && dtCancelE > dt))
                        {
                            // はじく
                            continue;
                        }
                    }
                }


                delay = (int)ts.TotalSeconds;

                string notification_title = notification_param.notification_title;
                string notification_body = notification_param.notification_body;

                if (notification_param.type != (int)ServerDataDefine.NOTIFICATION_TYPE.GACHA)
                {
                    // イベント通知フラグをチェックする
                    LocalSaveEventNotification cNotification = LocalSaveManager.Instance.CheckFuncNotificationRequest(notification_param.fix_id);
                    if (cNotification != null)
                    {
                        // データが登録されている場合
                        if (cNotification.m_Push == false)
                        {
                            // 通知しないならそのまま戻る
                            continue;
                        }
                        else
                        {
                            // 通知する場合で、テキストデータが無い場合。
                            // 初期値を入れる
                            if (notification_body == null
                            || notification_body.Length <= 0)
                            {
                                MasterDataEvent cEventData = MasterDataUtil.GetMasterDataEventFromFixID(cNotification.m_FixID);
                                if (cEventData != null)
                                {
                                    MasterDataArea cAreaData = MasterDataUtil.GetAreaParamFromEventID(cEventData.event_id);
                                    if (cAreaData != null)
                                    {
                                        // 紐づいているエリアを見る
                                        notification_title = GameTextUtil.GetText("LOCAL_NOTIFICATION_REQUEST_TITLE");
                                        notification_body = string.Format(GameTextUtil.GetText("LOCAL_NOTIFICATION_REQUEST_MESSAGE"), cAreaData.area_name);
                                    }
                                    // 情報が無ければ飛ばさない
                                }
                            }
                        }
                    }
                }
                // テキストデータが存在するものだけ通知に出す
                if (notification_body != null
                && notification_body.Length > 0)
                {
                    PQDMLocalNotification.SendNotification(notification_title,
                                                            notification_body,
                                                            delay);
                    if (notification_param.notification_type == MasterDataDefineLabel.NotificationType.SEISEKIDEN)
                    {
                        bSeisekiden = true;
                    }
                }
            }
            // 通知情報を保存する
            LocalSaveManager.Instance.SaveFuncNotificationRequest();
        }

        #region ==== スタミナMAX ====
        PacketStructPlayer cPlayer = UserDataAdmin.Instance.m_StructPlayer;
        // スタミナが減少チェック
        // 通知設定チェック
        if (cPlayer != null
        && m_StaminaNow < cPlayer.stamina_max
        && cOption.m_NotificationStaminaMax == (int)LocalSaveDefine.OptionNotificationStaminaMax.ON)
        {
            //----------------------------------------
            // スタミナ回復シミュレート
            // 回復に関連する時間系データを算出
            //----------------------------------------
            DateTime cRecoveryTime = TimeUtil.ConvertServerTimeToLocalTime(cPlayer.stamina_recover);    // 開始時刻取得
            uint unRecoveryValue = cPlayer.stamina_max - m_StaminaNow;                                  // スタミナ回復量取得

            // 終了時刻算出
            cRecoveryTime = cRecoveryTime.AddSeconds(GlobalDefine.STAMINA_RECOVERY_SEC * unRecoveryValue);

            //----------------------------------------
            // ローカル通知設定
            //----------------------------------------
            TimeSpan ts = cRecoveryTime.Subtract(TimeManager.Instance.m_TimeNow);
            if (ts.TotalSeconds >= 0)
            {
                delay = (int)ts.TotalSeconds;
                PQDMLocalNotification.SendNotification(UnityUtil.GetText("LOCAL_NOTIFICATION_STAMINA_TITLE"),
                                                        UnityUtil.GetText("LOCAL_NOTIFICATION_STAMINA_MESSAGE"),
                                                        delay);
            }
        }
        #endregion


    }


} // class LocalNotificationUtil



//--------------------------------------------------------------------------------
/*!
	@class		PQDMLocalNotification
	@brief		ローカル通知クラス
*/
//--------------------------------------------------------------------------------
public class PQDMLocalNotification
{

    //----------------------------------------------------------------------------
    //	@brief		ローカル通知設定
    //	@param[in]	string		(title)		通知文言：タイトル
    //	@param[in]	string		(text)		通知文言：本文
    //	@param[in]	int			(delay)		現在時刻から表示するまでの時間(秒)
    //----------------------------------------------------------------------------
    public static void SendNotification(string title, string text, int delay)
    {
        if (Patcher.Instance.GetLocalNotificationRegisterDisable())
        {
            return;
        }

        if (TimeManager.Instance == null)
        {
            return;
        }

#if LOCAL_NOTIFICATION_EDITOR
#if DEBUG_LOG
		// Editor
		Debug.Log( "SetNotification: " + title + " / text: " + text + " / delay: " + delay);
#endif
#elif LOCAL_NOTIFICATION_ANDROID
		string package = Application.identifier;
		string activity = "com.onevcat.uniwebview.AndroidPlugin";
		// Android
		ELANManager.SendNotification( title, text, package, activity, delay );
#elif LOCAL_NOTIFICATION_IOS
		// iOS(現在時刻とりなおして、ディレイ時間を足してるので若干登録時間に誤差がでるかも。気になるレベルではないはず)
		LocalNotification notification = new LocalNotification();
		if ( notification != null )
		{
			// サーバー時間を加味して通知時間を変更している
			ulong totalSeconds = TimeUtil.ConvertLocalTimeToServerTime( TimeManager.Instance.m_TimeNow.AddSeconds( delay ) );
			DateTime td = TimeUtil.ConvertTotalSecondsToServerTime( totalSeconds );

			notification.applicationIconBadgeNumber = -1;
			notification.fireDate  = td;
			notification.alertBody = text;
			NotificationServices.ScheduleLocalNotification( notification );
		}
#endif
    }


    //----------------------------------------------------------------------------
    //	@brief		ローカル通知のキャンセル
    //----------------------------------------------------------------------------
    public static void CancelAllNotifications()
    {

        if (Patcher.Instance.GetLocalNotificationCancelDisable())
        {
            return;
        }

#if LOCAL_NOTIFICATION_EDITOR
#if DEBUG_LOG
		// Editor
		Debug.Log( "CancelAllNotification" );
#endif
#elif LOCAL_NOTIFICATION_ANDROID
		// Android
		ELANManager.CancelAllNotifications();
#elif LOCAL_NOTIFICATION_IOS
		// iOS
		if ( NotificationServices.localNotificationCount >= 0 ) {
			LocalNotification notification = new LocalNotification();
			if ( notification != null ) {
				notification.applicationIconBadgeNumber = -1;
				NotificationServices.PresentLocalNotificationNow( notification );

				NotificationServices.CancelAllLocalNotifications();
				NotificationServices.ClearLocalNotifications();
			}
		}
#endif
    }


} // class PQDMLocalNotification


