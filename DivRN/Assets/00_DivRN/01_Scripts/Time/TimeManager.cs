/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	TimeManager.cs
	@brief	時間管理クラス
	@author Developer
	@date 	2012/10/08

	時間管理に特化
	期間限定クエストなど、スマホプロジェクトはリアル時間の判定を多用するが、
	それぞれが好き勝手に参照すると開発後半でタイミングバグが多発する。
	iOSはTimeクラスから現在時間を取得すると、本体設定時間を返すため意図した結果が得られないケースもある。	
 
	時間管理を切り離し、ここを経由することで処理の煩雑化を防ぐ。
	後々起動時にサーバーから基準時間を得て、それをdeltaで加算して進めていく手法を検討する
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
#if UNITY_EDITOR
using UnityEditor;
#endif

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
	@brief	時間管理クラス
*/
//----------------------------------------------------------------------------
public class TimeManager : SingletonComponent<TimeManager>
{
    public const float FPS = 30.0f;             //!< このゲームのデフォルトFPS

    public const int SYSTEM_START_DATE = 1970010100;    //!< システム開始日時

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    public DateTime m_TimeAppStart;             //!< アプリ起動タイミング：
                                                //	public	int					m_TimeAppStartYear;			//!< アプリ起動タイミング：年
                                                //	public	int					m_TimeAppStartMonth;		//!< アプリ起動タイミング：月
                                                //	public	int					m_TimeAppStartData;			//!< アプリ起動タイミング：日
                                                //	public	int					m_TimeAppStartHour;			//!< アプリ起動タイミング：時
                                                //	public	int					m_TimeAppStartMinute;		//!< アプリ起動タイミング：分
                                                //	public	int					m_TimeAppStartSecond;		//!< アプリ起動タイミング：秒

    public DateTime m_TimeNow;                  //!< 現時点タイミング：
                                                //	public	int					m_TimeNowYear;				//!< 現時点タイミング：年
                                                //	public	int					m_TimeNowMonth;				//!< 現時点タイミング：月
                                                //	public	int					m_TimeNowData;				//!< 現時点タイミング：日
                                                //	public	int					m_TimeNowHour;				//!< 現時点タイミング：時
                                                //	public	int					m_TimeNowMinute;			//!< 現時点タイミング：分
                                                //	public	int					m_TimeNowSecond;			//!< 現時点タイミング：秒

    private TimeSpan m_TimeSpan;                //!< 作業クラス：起動後タイムスパン総計
    private DateTime m_TimePrev;                //!< 作業クラス：起動後前回時間

#if BUILD_TYPE_DEBUG
    private bool m_FlgUserSetTime = false;		//!< DEBUG用ユーザ時間設定フラグ
    private DateTime m_TimeNowSv;				//!< DEBUG用ユーザ時間設定
#endif

    public float m_DeltaTotal = 0.0f;			//!< 開始以後のDeltaTime総計

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

        m_TimePrev = DateTime.Now;
        m_TimeSpan = new TimeSpan();

#if true
        //-------------------------
        // とりあえずサーバー整備されるまではローカル時間を参照。
        // 本来は、サーバーから初回通信時に時間を貰って、それを基準として時間を割り出す。
        //-------------------------
        m_TimeAppStart = DateTime.Now;
        //-------------------------
#endif

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	DEBUG用時間設定
	*/
    //----------------------------------------------------------------------------
    public void setTimeNowForDebug(DateTime dt)
    {
#if BUILD_TYPE_DEBUG
        m_TimeNowSv = dt;
        // m_TimeNow = m_TimeNowSv;

        ulong svrtime = TimeUtil.ConvertLocalTimeToServerTime(m_TimeNowSv);
        m_FlgUserSetTime = false;
        SetupServerTime(svrtime);
        m_FlgUserSetTime = true;

#endif
    }

#if BUILD_TYPE_DEBUG
    //----------------------------------------------------------------------------
    /*!
		@brief	DEBUG用時間取得
	*/
    //----------------------------------------------------------------------------
    public DateTime getTimeNowForDebug()
    {
        DateTime result;
        result = m_TimeNowSv;
        // result = m_TimeNow;
        return result;
    }
#endif




    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public void Update()
    {
        //--------------------------------
        // UI関連の操作のための統一時間
        //--------------------------------
        m_DeltaTotal += Time.deltaTime;

        //-------------------------
        // 起動後の経過時間を求める
        //-------------------------
        DateTime cTimeNow = DateTime.Now;
        m_TimeSpan += (cTimeNow - m_TimePrev);
        m_TimePrev = cTimeNow;

        //-------------------------
        // 現在時間を求める
        // 
        // DataTime.Nowで取れる時間情報は、本体設定時間のためそのまま参照できない。
        // サーバーから基準時間を取得し、それに経過時間を足し合わせて現在時間を求める。
        //-------------------------
        m_TimeNow = m_TimeAppStart + m_TimeSpan;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	サーバー時間設定
	*/
    //----------------------------------------------------------------------------
    public void SetupServerTime(ulong unServerTime)
    {
#if BUILD_TYPE_DEBUG
        if (m_FlgUserSetTime)
        {
            return;
        }
#endif

        //-------------------------
        // サーバー時間を基準時間として設定
        //-------------------------
        DateTime cServerTime = TimeUtil.ConvertServerTimeToLocalTime(unServerTime);

        //-------------------------
        // サーバー時間を設定。
        // 基準時間が更新されることで、経過時間情報も初期化される
        //-------------------------
        m_TimeAppStart = cServerTime;
        m_TimePrev = DateTime.Now;
        m_TimeSpan = new TimeSpan();
        m_TimeNow = m_TimeAppStart;
#if UNITY_EDITOR

#endif
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	時間内チェック
	*/
    //----------------------------------------------------------------------------
    public bool CheckWithinTime(
        int nStartYear
    , int nStartMonth
    , int nStartDay
    , int nStartHour
    , int nEndYear
    , int nEndMonth
    , int nEndDay
    , int nEndHour
    )
    {
        //-------------------------
        // 期間開始前チェック
        //-------------------------
        //-------------------------
        // 時間が24を超える場合、
        // 時間が生成できずにハングする。
        //
        // 対応はデータ側で気を付けてもらうとして、
        // 最低限のセーフティのため24時間で制限をかけておく
        //-------------------------
        if (nStartHour > 24)
        {
            Debug.LogError("Time Safety - " + nStartHour + " ->" + 24);

            DateTime cTimeStart = new DateTime(nStartYear, nStartMonth, nStartDay, 23, 59, 59);
            if (cTimeStart > m_TimeNow)
            {
                return false;
            }
        }
        else
        {
            DateTime cTimeStart = new DateTime(nStartYear, nStartMonth, nStartDay, nStartHour, 0, 0);
            if (cTimeStart > m_TimeNow)
            {
#if BUILD_TYPE_DEBUG
                //				Debug.Log( cTimeStart.ToString("start:yyyy-MM-dd,HH:mm:ss" ) );
#endif
                return false;
            }
        }
        //-------------------------
        // @add Developer 2016/07/25 ver360
        // 終了期間が0だった場合、無期限として扱うように設定
        //-------------------------
        if (nEndYear + nEndMonth + nEndDay + nEndHour > 0)
        {
            //-------------------------
            // 期間終了済みチェック
            //  「24時」指定とかだと、実質「23時59分59秒」になるように補正して判定
            //-------------------------
            if (nEndHour == 0)
            {
                DateTime cTimeEnd = new DateTime(nEndYear, nEndMonth, nEndDay, nEndHour, 0, 0);
                if (cTimeEnd < m_TimeNow)
                {
#if BUILD_TYPE_DEBUG
                    //				Debug.Log( cTimeEnd.ToString("end:yyyy-MM-dd,HH:mm:ss" ) );
#endif
                    return false;
                }
            }
            else
            {
                //-------------------------
                // 時間が24を超える場合、
                // 時間が生成できずにハングする。
                //
                // 対応はデータ側で気を付けてもらうとして、
                // 最低限のセーフティのため24時間で制限をかけておく
                //-------------------------
                if (nEndHour > 24)
                {
                    nEndHour = 24;
                }

                DateTime cTimeEnd = new DateTime(nEndYear, nEndMonth, nEndDay, nEndHour - 1, 59, 59);
                if (cTimeEnd < m_TimeNow)
                {
#if BUILD_TYPE_DEBUG
                    //				Debug.Log( cTimeEnd.ToString("end:yyyy-MM-dd,HH:mm:ss" ) );
#endif
                    return false;
                }
            }
        }
        //-------------------------
        // 期間中！
        //-------------------------
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	時間外チェック（未来日時）
	*/
    //----------------------------------------------------------------------------
    public bool CheckFurtureTime(
        int nStartYear
    , int nStartMonth
    , int nStartDay
    , int nStartHour
    , int nEndYear
    , int nEndMonth
    , int nEndDay
    , int nEndHour
    )
    {
        //-------------------------
        // 期間開始前チェック
        //-------------------------
        //-------------------------
        // 時間が24を超える場合、
        // 時間が生成できずにハングする。
        //
        // 対応はデータ側で気を付けてもらうとして、
        // 最低限のセーフティのため24時間で制限をかけておく
        //-------------------------
        if (nStartHour > 24)
        {
            Debug.LogError("Time Safety - " + nStartHour + " ->" + 24);

            DateTime cTimeStart = new DateTime(nStartYear, nStartMonth, nStartDay, 23, 59, 59);
            if (cTimeStart <= m_TimeNow)
            {
                //時間内の場合
                return false;
            }
        }
        else
        {
            DateTime cTimeStart = new DateTime(nStartYear, nStartMonth, nStartDay, nStartHour, 0, 0);
            if (cTimeStart <= m_TimeNow)
            {
                //時間内の場合
                return false;
            }
        }

        //-------------------------
        // @add Developer 2016/08/12 ver360
        // 終了期間が0だった場合、無期限として扱うように設定
        //-------------------------
        if (nEndYear + nEndMonth + nEndDay + nEndHour > 0)
        {
            //-------------------------
            // 期間終了済みチェック
            //  「24時」指定とかだと、実質「23時59分59秒」になるように補正して判定
            //-------------------------
            if (nEndHour == 0)
            {
                DateTime cTimeEnd = new DateTime(nEndYear, nEndMonth, nEndDay, nEndHour, 0, 0);
                if (cTimeEnd < m_TimeNow)
                {
                    return false;
                }
            }
            else
            {
                //-------------------------
                // 時間が24を超える場合、
                // 時間が生成できずにハングする。
                //
                // 対応はデータ側で気を付けてもらうとして、
                // 最低限のセーフティのため24時間で制限をかけておく
                //-------------------------
                if (nEndHour > 24)
                {
                    nEndHour = 24;
                }

                DateTime cTimeEnd = new DateTime(nEndYear, nEndMonth, nEndDay, nEndHour - 1, 59, 59);
                if (cTimeEnd < m_TimeNow)
                {
                    return false;
                }
            }
        }

        //-------------------------
        // 未来！
        //-------------------------
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	時間内チェック v350追加
				 終了期限のみの指定時用
	*/
    //----------------------------------------------------------------------------
    public bool CheckWithinTime(uint unTimingEnd)
    {
        return CheckWithinTime(SYSTEM_START_DATE, unTimingEnd);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	時間内チェック
	*/
    //----------------------------------------------------------------------------
    public bool CheckWithinTime(
        uint nTimingStart
    , uint nTimingEnd
    )
    {
        int nStartYear = (int)(nTimingStart / 100 / 100 / 100);
        int nStartMonth = (int)(nTimingStart / 100 / 100) % 100;
        int nStartDay = (int)(nTimingStart / 100) % 100;
        int nStartHour = (int)(nTimingStart % 100);
        int nEndYear = (int)(nTimingEnd / 100 / 100 / 100);
        int nEndMonth = (int)(nTimingEnd / 100 / 100) % 100;
        int nEndDay = (int)(nTimingEnd / 100) % 100;
        int nEndHour = (int)(nTimingEnd % 100);

        return CheckWithinTime(nStartYear, nStartMonth, nStartDay, nStartHour, nEndYear, nEndMonth, nEndDay, nEndHour);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	時間外チェック（未来日時）
	*/
    //----------------------------------------------------------------------------
    public bool CheckFurtureTime(
        uint nTimingStart
    , uint nTimingEnd
    )
    {
        int nStartYear = (int)(nTimingStart / 100 / 100 / 100);
        int nStartMonth = (int)(nTimingStart / 100 / 100) % 100;
        int nStartDay = (int)(nTimingStart / 100) % 100;
        int nStartHour = (int)(nTimingStart % 100);
        int nEndYear = (int)(nTimingEnd / 100 / 100 / 100);
        int nEndMonth = (int)(nTimingEnd / 100 / 100) % 100;
        int nEndDay = (int)(nTimingEnd / 100) % 100;
        int nEndHour = (int)(nTimingEnd % 100);

        return CheckFurtureTime(nStartYear, nStartMonth, nStartDay, nStartHour, nEndYear, nEndMonth, nEndDay, nEndHour);
    }

    public uint ConvertDay(DateTime day)
    {
        uint ret = 0;
        ret = (uint)(day.Year * 10000 + day.Month * 100 + day.Day);
        return ret;
    }

    public int ChangeTimeHour()
    {
        // タイトル戻しの時間を取得
        string strChangeDateTime = Patcher.Instance.GetChangeDateTime();

        // タイトル戻しの時間を設定
        int nChangeTimeHour = 0;

        if (strChangeDateTime != "")
        {
            nChangeTimeHour = int.Parse(strChangeDateTime);
        }

        return nChangeTimeHour;
    }

#if UNITY_EDITOR
    public long TimeEdit = 0;
    [CustomEditor(typeof(TimeManager))]
    public class TimeManagerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TimeManager timeManager = (TimeManager)target;

            EditorGUI.BeginDisabledGroup(false);

            EditorGUILayout.TextField("アプリ時間", timeManager.m_TimeNow.ToString());

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Update"))
            {
                DateTime time = TimeUtil.GetDateTimeMin(timeManager.TimeEdit);
                timeManager.setTimeNowForDebug(time);
            }
        }
    }
#endif
}

