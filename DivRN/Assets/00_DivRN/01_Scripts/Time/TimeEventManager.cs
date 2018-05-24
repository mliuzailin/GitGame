/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	TimeEventManager.cs
	@brief	期間限定関連の情報保持クラス
	@author Developer
	@date 	2013/01/09

	期間限定イベントなどの、発生状況保持に特化。
	毎フレ判定を回してどのイベントが発生しているかを判断し続ける。

	期間限定クエストや、期間限定ガチャ、ステータス変動、etc...
	それぞれの処理中でイベント開催中か判断するのが面倒なのでここで一括で判断して他に開示する
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
//
//	@class		CycleParam
//	@brief		サイクルイベント用パラメータ
//
//----------------------------------------------------------------------------
public class CycleParam
{
    private uint m_FixID;           //!< イベントマスタ：情報固有固定ID
    private uint m_TimingStart;     //!< イベントタイミング：開始
    private uint m_TimingEnd;       //!< イベントタイミング：終了
    private MasterDataDefineLabel.BoolType m_Schedule;      //!< イベントスケジュール表示有無

    //------------------------------------------------------------------------
    //	@brief		イベントタイミング：開始
    //------------------------------------------------------------------------
    public uint fixID
    {
        get
        {
            return m_FixID;
        }
    }

    //------------------------------------------------------------------------
    //	@brief		イベントタイミング：開始
    //------------------------------------------------------------------------
    public uint timingStart
    {
        get
        {
            return m_TimingStart;
        }
    }

    //------------------------------------------------------------------------
    //	@brief		イベントタイミング：終了
    //------------------------------------------------------------------------
    public uint timingEnd
    {
        get
        {
            return m_TimingEnd;
        }
    }

    //------------------------------------------------------------------------
    //	@brief		イベントスケジュール表示有無
    //------------------------------------------------------------------------
    public MasterDataDefineLabel.BoolType schedule
    {
        get
        {
            return m_Schedule;
        }
    }

    //------------------------------------------------------------------------
    //	@brief		パラメータリセット
    //------------------------------------------------------------------------
    public void Reset()
    {
        m_FixID = 0;                    //!< イベントマスタ：情報固有固定ID
        m_TimingStart = 0;                  //!< イベントタイミング：開始
        m_TimingEnd = 0;                    //!< イベントタイミング：終了
        m_Schedule = 0;                 //!< イベントスケジュール表示有無
    }

    //------------------------------------------------------------------------
    //	@brief		パラメータセットアップ
    //	@change Developer 2015/09/01 ver300
    //------------------------------------------------------------------------
    public void Setup(uint unFixID, uint unTimingStart, uint unTimingEnd, MasterDataDefineLabel.BoolType unSchedule)
    {
        m_FixID = unFixID;          //!< イベントマスタ：情報固有固定ID
        m_TimingStart = unTimingStart;      //!< イベントタイミング：開始
        m_TimingEnd = unTimingEnd;      //!< イベントタイミング：終了
        m_Schedule = unSchedule;            //!< イベントスケジュール表示有無
    }


}; // class CycleParam

//----------------------------------------------------------------------------
/*!
	@brief	期間限定関連の情報保持クラス
*/
//----------------------------------------------------------------------------
public class TimeEventManager : SingletonComponent<TimeEventManager>
{
    const int EVENT_MAX = 1024;
    const int MONTH_MAX = 12;           //!< 月：最大数
    const int DAY_TIME_MAX = 24;            //!< 日：最大時間

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public CycleParam[] m_TimeEventCycleParam = new CycleParam[EVENT_MAX];  //!< 現在有効なサイクルイベント情報
    public CycleParam[] m_TimeEventCycleFurtureParam = new CycleParam[EVENT_MAX];   //!< 未来有効のサイクルイベント情報
    public uint[] m_TimeEventActiveID = null;           //!< 現在有効なイベントリスト
    public uint[] m_TimeEventFurtureID = null;          //!< 未来有効のイベントリスト
    public uint m_TimeEventActiveInputed = 0;                               //!< 現在有効なイベントリスト入力総数
    public uint m_TimeEventFurtureInputed = 0;                              //!< 未来有効のイベントリスト入力総数

    public bool m_TimeEventUpdate = false;
    public DateTime m_TimeEventChkTime;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※インスタンス生成時呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();

        m_TimeEventActiveID = new uint[EVENT_MAX];
        m_TimeEventFurtureID = new uint[EVENT_MAX];

        //--------------------------------
        // 一旦情報クリア
        //--------------------------------
        for (int i = 0; i < EVENT_MAX; ++i)
        {
            m_TimeEventCycleParam[i] = new CycleParam();
            m_TimeEventCycleFurtureParam[i] = new CycleParam();

            m_TimeEventCycleParam[i].Reset();
            m_TimeEventCycleFurtureParam[i].Reset();
            m_TimeEventActiveID[i] = 0;
            m_TimeEventFurtureID[i] = 0;
        }
        m_TimeEventActiveInputed = 0;
        m_TimeEventFurtureInputed = 0;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();

        //--------------------------------
        // 初回更新時に処理が回るようにフラグを立てておく
        //--------------------------------
        m_TimeEventUpdate = true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	イベント判定の強制更新リクエスト
		@note	リクエスト発行から１フレの遅延があるので注意
	*/
    //----------------------------------------------------------------------------
    public void TimeEventUpdateRequest()
    {
        //--------------------------------
        // 処理が回るようにフラグを立てておく
        //--------------------------------
        m_TimeEventUpdate = true;

#if BUILD_TYPE_DEBUG && DEBUG_LOG
		Debug.Log( "TimeEventManager UpdateRequest!" );
//		Debug.LogWarning( "TimeEventManager UpdateRequest!" );
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public void Update()
    {
        //--------------------------------
        // 必要な各種マネージャのチェック
        //--------------------------------
        if (TimeManager.Instance == null)
        {
            //			Debug.LogError( "TimeEvent Error!" );
            return;
        }
        if (UserDataAdmin.Instance == null
        || UserDataAdmin.Instance.m_StructPlayer == null
        || UserDataAdmin.Instance.m_StructPlayer.user == null
        || UserDataAdmin.Instance.m_StructPlayer.user.user_id == 0)
        {
            return;
        }

        //--------------------------------
        // 毎フレ判定する意味がないので適当なサイクルで判定を行う
        //--------------------------------
        if (m_TimeEventUpdate == false)
        {
            //			if( m_TimeEventChkTime.Hour != TimeManager.Instance.m_TimeNow.Hour )
            if (m_TimeEventChkTime.Minute != TimeManager.Instance.m_TimeNow.Minute
            || m_TimeEventChkTime.Hour != TimeManager.Instance.m_TimeNow.Hour
            || m_TimeEventChkTime.Day != TimeManager.Instance.m_TimeNow.Day
            || m_TimeEventChkTime.Month != TimeManager.Instance.m_TimeNow.Month
            || m_TimeEventChkTime.Year != TimeManager.Instance.m_TimeNow.Year
            )
            {
                m_TimeEventUpdate = true;
            }
            else
            {
                return;
            }
        }
        m_TimeEventChkTime = TimeManager.Instance.m_TimeNow;
        m_TimeEventUpdate = false;

        //--------------------------------
        // 一旦情報クリア
        //--------------------------------
        for (int i = 0; i < EVENT_MAX; ++i)
        {
            m_TimeEventCycleParam[i].Reset();
            m_TimeEventCycleFurtureParam[i].Reset();
            m_TimeEventActiveID[i] = 0;
            m_TimeEventFurtureID[i] = 0;
        }
        m_TimeEventActiveInputed = 0;
        m_TimeEventFurtureInputed = 0;

        updateEvents();
#if UNITY_EDITOR && DEBUG_LOG
		Debug.Log( "TimeEventManager Update! - " + TimeManager.Instance.m_TimeNow );
#endif // UNITY_EDITOR
    }

    public void updateEvents()
    {
        //--------------------------------
        // 期間限定イベントのマスターデータを総当たりでチェック
        //--------------------------------
        MasterDataDefineLabel.BelongType nUserGroup = SystemUtil.GetUserGroup();
        MasterDataEvent[] eventArray = MasterFinder<MasterDataEvent>.Instance.GetAll();
        //MasterDataEvent[] eventArray = MasterFinder<MasterDataEvent>.Instance.SelectWhere(" where active = ? and event_id != 0 and ( user_group = ? OR user_group = ?) ", MasterDataDefineLabel.BoolType.ENABLE, MasterDataDefineLabel.BelongType.NONE, nUserGroup).ToArray();
        int nMasterEventMax = eventArray.Length;
        for (int num = 0; num < nMasterEventMax; ++num)
        {
            MasterDataEvent cEvent = eventArray[num];
            /*sqlite対応*/
            if (cEvent.active != MasterDataDefineLabel.BoolType.ENABLE
            || cEvent.event_id == 0)
            {
                continue;
            }

            if (cEvent.user_group != MasterDataDefineLabel.BelongType.NONE
            && cEvent.user_group != nUserGroup)
            {
                continue;
            }
            //if (cEvent.event_id == 0) continue;

            //--------------------------------
            // サイクル処理対応
            // @add Developer 2016/07/25 v360
            //--------------------------------
            // 指定(従来通り)
            if (cEvent.period_type == MasterDataDefineLabel.PeriodType.DESIGNATION)
            {
                //--------------------------------
                // イベント期間判定
                //--------------------------------
                bool bCheckWithinTime = TimeManager.Instance.CheckWithinTime(
                                                                    cEvent.timing_start
                                                                , cEvent.timing_end
                                                                );
#if BUILD_TYPE_DEBUG
                // デバッグ機能：未来のクエストも表示する（表示が重なったりするかも…）
                if (DebugOption.Instance.featureQuest
                    && bCheckWithinTime == false
                )
                {
                    DateTime date_time_start = TimeUtil.GetDateTime(cEvent.timing_start);
                    if (date_time_start.CompareTo(DateTime.Now) >= 0)
                    {
                        bCheckWithinTime = true;
                    }
                }
#endif //BUILD_TYPE_DEBUG

                if (bCheckWithinTime == false)
                {
                    continue;
                }

                //--------------------------------
                // 現在時がイベント期間内なのでフラグ立て
                //--------------------------------
                m_TimeEventActiveID[m_TimeEventActiveInputed] = cEvent.event_id;
                m_TimeEventActiveInputed++;

                if (EVENT_MAX <= m_TimeEventActiveInputed)
                {
                    Debug.LogError("EventID Buffer Over!");
                    break;
                }
            }
            // サイクル
            else if (cEvent.period_type == MasterDataDefineLabel.PeriodType.CYCLE)
            {
                //--------------------------------
                // イベント期間判定
                //--------------------------------
                if (TimeManager.Instance.CheckWithinTime(cEvent.timing_start, cEvent.timing_end) == false)
                {
                    //--------------------------------
                    // 未来日チェック
                    // スケジュール開催予定用に、サイクルデータを作成するため
                    //--------------------------------
                    if (TimeManager.Instance.CheckFurtureTime(cEvent.timing_start, cEvent.timing_end) == false)
                    {
                        continue;
                    }

                    // 各種情報を分解取得
                    int nStartYear = TimeUtil.GetDateTimeToYear(cEvent.timing_start);       // 年
                    int nStartMonth = TimeUtil.GetDateTimeToMonth(cEvent.timing_start);     // 月
                    int nStartDay = TimeUtil.GetDateTimeToDay(cEvent.timing_start);     // 日
                    DateTime cTimeStartDay = new DateTime(nStartYear, nStartMonth, nStartDay, 0, 0, 0);

                    // 時間を考慮しない現在日を取得
                    DateTime cTmpTimeNow = TimeManager.Instance.m_TimeNow;
                    DateTime cTimeNowDay = new DateTime(cTmpTimeNow.Year, cTmpTimeNow.Month, cTmpTimeNow.Day, 0, 0, 0);

                    // 現在日から開催日までのTimeSpanを取得
                    TimeSpan cTimeSpan = cTimeStartDay - cTimeNowDay;

                    // 開催日まで一日よりある場合は登録しない
                    if (cTimeSpan.Days > 1)
                    {
                        continue;
                    }
                }

                //--------------------------------
                // サイクルデータ作成
                //--------------------------------
                if (CreateEventCycle(cEvent) == false)
                {
                    break;
                }
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public bool ChkEventActive(uint unEventID)
    {
        //--------------------------------
        // 0番はイベント無視なので無条件OK
        //--------------------------------
        if (unEventID == 0)
        {
            return true;
        }

        //--------------------------------
        // 総当たりでイベントリストチェック
        //--------------------------------
        for (uint i = 0; i < m_TimeEventActiveInputed; i++)
        {
            if (m_TimeEventActiveID[i] != unEventID)
                continue;
            return true;
        }
        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	サイクル処理
		@note	サイクルイベントを算出する
		@add	Developer 2016/07/25 v360
	*/
    //----------------------------------------------------------------------------
    private bool CreateEventCycle(MasterDataEvent cEvent)
    {
        bool bResult = true;
        //--------------------------------
        // エラーチェック
        // イベント有効時間が、0の場合は処理しない
        //--------------------------------
        if (cEvent.cycle_active_hour == 0)
        {
            return (bResult);
        }


        // イベント期間用
        uint unEventStart;
        uint unEventEnd;
        uint unEventEndTemp;

        // イベントマスター
        uint unDateType = cEvent.cycle_date_type;                               // サイクル：曜日
        uint unCycleStart = cEvent.cycle_timing_start;                          // サイクル：開始時間
        uint unCycleActive = cEvent.cycle_active_hour;                          // サイクル：有効時間
        int nActiveEnd = (int)(unCycleStart + unCycleActive);               // サイクル：開始時間+有効時間

        // 処理日
        int nYearNow = TimeManager.Instance.m_TimeNow.Year;             // 現在：年
        int nMonthNow = TimeManager.Instance.m_TimeNow.Month;               // 現在：月
        int nDayNow = TimeManager.Instance.m_TimeNow.Day;                   // 現在：日

        DateTime cFirstDayOfMonth = new DateTime(nYearNow, nMonthNow, 1);               // 現在：月初め
        DateTime cLastDayOfMonth = cFirstDayOfMonth.AddMonths(1).AddDays(-1);       // 現在：月終わり


        //--------------------------------
        // サイクル曜日を取得
        // データ順：月火水木金土日空 →下位bit
        //--------------------------------
        bool[] abChkBitDayOfWeek = new bool[TimeUtil.BIT_DAY_OF_WEEK_MAX];
        int nBitFlag = 0;
        for (int bitNum = 0; bitNum < TimeUtil.BIT_DAY_OF_WEEK_MAX; ++bitNum)
        {
            nBitFlag = 1 << bitNum;

#if BUILD_TYPE_DEBUG
            // デバッグ機能：未来のクエストも表示する（表示が重なったりするかも…）
            if (DebugOption.Instance.featureQuest)
            {
                abChkBitDayOfWeek[bitNum] = true;   //全ての曜日を表示する
            }
#endif //BUILD_TYPE_DEBUG

            //--------------------------------
            // フラグチェック
            //--------------------------------
            if ((unDateType & nBitFlag) == 0)
            {
                continue;
            }

            abChkBitDayOfWeek[bitNum] = true;
        }

        //--------------------------------
        // 前後一週間をチェック
        //--------------------------------
        int nWeekMax = TimeUtil.BIT_DAY_OF_WEEK_MAX - 1;
        int nDayWeekBefore = nDayNow - nWeekMax;
        int nDayWeekAfter = nDayNow + nWeekMax;

        int nYearStart;         // 開始：年
        int nMonthStart;            // 開始：月
        int nDayStart;              // 開始：日
        int nSearchDayOfWeek;       // 前後一週間の曜日検索用

        DateTime cFirstDayOfLastMonth;  // 開始：月初め
        DateTime cLastDayOfLastMonth;   // 開始：月終わり

        for (int dayNum = nDayWeekBefore; dayNum < nDayWeekAfter; ++dayNum)
        {
            #region ==== 開始時間を算出 ====
            //--------------------------------
            // 初期化
            //--------------------------------
            nYearStart = nYearNow;
            nMonthStart = nMonthNow;
            nDayStart = dayNum;
            cLastDayOfLastMonth = cLastDayOfMonth;

            //--------------------------------
            // 月跨ぎチェック
            //--------------------------------
            // 現在：月初めより前の場合
            if (nDayStart < 1)
            {
                --nMonthStart;

                //--------------------------------
                // 年跨ぎチェック
                //--------------------------------
                // 年明けより前の場合
                if (nMonthStart < 1)
                {
                    --nYearStart;
                    nMonthStart = MONTH_MAX;
                }

                //--------------------------------
                // 開始日設定(先月日)
                //--------------------------------
                cFirstDayOfLastMonth = new DateTime(nYearStart, nMonthStart, 1);            // 開始：月初め
                cLastDayOfLastMonth = cFirstDayOfLastMonth.AddMonths(1).AddDays(-1);    // 開始：月終わり

                nDayStart += cLastDayOfLastMonth.Day;
            }
            // 現在：月終わりより後の場合
            else if (nDayStart > cLastDayOfMonth.Day)
            {
                ++nMonthStart;

                //--------------------------------
                // 年跨ぎチェック
                //--------------------------------
                // 年末より後の場合
                if (nMonthStart > MONTH_MAX)
                {
                    ++nYearStart;
                    nMonthStart = 1;
                }

                //--------------------------------
                // 開始日設定(来月日)
                //--------------------------------
                cFirstDayOfLastMonth = new DateTime(nYearStart, nMonthStart, 1);            // 開始：月初め
                cLastDayOfLastMonth = cFirstDayOfLastMonth.AddMonths(1).AddDays(-1);    // 開始：月終わり

                nDayStart -= cLastDayOfMonth.Day;
            }

            //--------------------------------
            // サイクル曜日チェック
            //--------------------------------
            nSearchDayOfWeek = TimeUtil.GetDayOfWeekToBitValue(nYearStart, nMonthStart, nDayStart);
            if (abChkBitDayOfWeek[nSearchDayOfWeek] == false)
            {
                continue;
            }

            //--------------------------------
            // 開始時間を設定
            //--------------------------------
            unEventStart = TimeUtil.GetDateTimeToValue(nYearStart, nMonthStart, nDayStart, (int)unCycleStart);

            //--------------------------------
            // 期間チェック
            // 開始タイミング前の日程は、登録しない
            //--------------------------------
            if (unEventStart < cEvent.timing_start)
            {
                continue;
            }
            #endregion

            #region ==== 終了時間を算出 ====
            //--------------------------------
            // 終了時間の日跨ぎチェック(開始時間+有効時間)
            // ※24時で、翌日の0時のため「>=」としている
            //--------------------------------
            if (nActiveEnd >= DAY_TIME_MAX)
            {
                int nAddYear = 0;                               // 終了加算：年
                int nAddMonth = 0;                              // 終了加算：月
                int nAddDay = nActiveEnd / DAY_TIME_MAX;        // 終了加算：日
                int nHourEnd = nActiveEnd % DAY_TIME_MAX;       // 終了加算：時

                //--------------------------------
                // 月跨ぎチェック(開始日+加算日)
                //--------------------------------
                int nDayEnd = nDayStart + nAddDay;
                if (nDayEnd > cLastDayOfLastMonth.Day)
                {
                    ++nAddMonth;
                    nDayEnd %= cLastDayOfLastMonth.Day;
                }

                //--------------------------------
                // 年跨ぎチェック(開催月+加算月)
                //--------------------------------
                int nMonthEnd = nMonthStart + nAddMonth;
                if (nMonthEnd > MONTH_MAX)
                {
                    ++nAddYear;
                    nMonthEnd %= MONTH_MAX;
                }

                unEventEndTemp = TimeUtil.GetDateTimeToValue((nYearStart + nAddYear),
                                                              nMonthEnd, nDayEnd, nHourEnd);
            }
            else
            {
                // 日を跨がないので、開始時間+有効時間を終了時間とする
                unEventEndTemp = unEventStart + unCycleActive;
            }

            //--------------------------------
            // 終了時間を設定
            //--------------------------------
            unEventEnd = unEventEndTemp;
            #endregion


            //--------------------------------
            // イベント期間判定(現在開催中)
            //--------------------------------
            if (TimeManager.Instance.CheckWithinTime(unEventStart, unEventEnd) == false)
            {
                //--------------------------------
                // スケジュールに表示する場合
                // 未来予定は、スケジュールでしか使用していない(v360)
                //--------------------------------
                if (cEvent.event_schedule_show == MasterDataDefineLabel.BoolType.ENABLE)
                {
                    //--------------------------------
                    // サイクルイベントの終了時間が、
                    // イベント終了タイミングを越えている場合
                    //--------------------------------
                    if (cEvent.timing_end > 0
                    && unEventEnd > cEvent.timing_end)
                    {
                        // 登録しない
                        continue;
                    }

                    //--------------------------------
                    // イベント期間判定(未来開催)
                    //--------------------------------
                    if (TimeManager.Instance.CheckFurtureTime(unEventStart, unEventEnd) == true)
                    {
                        //--------------------------------
                        // 未来有効のサイクル情報を保存
                        //--------------------------------
                        m_TimeEventCycleFurtureParam[m_TimeEventFurtureInputed].Setup(cEvent.fix_id,
                                                                                         unEventStart,
                                                                                         unEventEnd,
                                                                                         cEvent.event_schedule_show);

                        //--------------------------------
                        // 未来有効のイベントなのでフラグ立て
                        //--------------------------------
                        m_TimeEventFurtureID[m_TimeEventFurtureInputed] = cEvent.event_id;
                        ++m_TimeEventFurtureInputed;

                        if (EVENT_MAX <= m_TimeEventFurtureInputed)
                        {
                            // 末尾を消して、応急処置
                            --m_TimeEventFurtureInputed;
#if BUILD_TYPE_DEBUG
                            Debug.LogError("EventFurtureID Buffer Over!");
#endif // BUILD_TYPE_DEBUG
                        }
                    }
                }

                continue;
            }

            //--------------------------------
            // 現在有効なサイクル情報を保存
            //--------------------------------
            m_TimeEventCycleParam[m_TimeEventActiveInputed].Setup(cEvent.fix_id,
                                                                     unEventStart,
                                                                     unEventEnd,
                                                                     cEvent.event_schedule_show);

            //--------------------------------
            // 現在時がイベント期間内なのでフラグ立て
            //--------------------------------
            m_TimeEventActiveID[m_TimeEventActiveInputed] = cEvent.event_id;
            ++m_TimeEventActiveInputed;

            if (EVENT_MAX <= m_TimeEventActiveInputed)
            {
#if BUILD_TYPE_DEBUG
                Debug.LogError("EventID Buffer Over!");
#endif // BUILD_TYPE_DEBUG
                bResult = false;
                break;
            }
        }

        return (bResult);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	現在有効なサイクルイベント情報を取得
	*/
    //----------------------------------------------------------------------------
    public CycleParam GetEventCycleParam(uint unEventID)
    {
        CycleParam cCycleParam = null;

        //--------------------------------
        // 総当たりでイベントリストチェック
        // m_TimeEventCycleParamも、m_TimeEventActiveInputedで管理されている
        //--------------------------------
        for (uint num = 0; num < m_TimeEventActiveInputed; ++num)
        {
            if (m_TimeEventActiveID[num] != unEventID)
            {
                continue;
            }

            cCycleParam = m_TimeEventCycleParam[num];
            break;
        }

        return (cCycleParam);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	未来有効のサイクルイベント情報を取得
	*/
    //----------------------------------------------------------------------------
    public CycleParam GetEventCycleFurtureParam(uint unEventID)
    {
        CycleParam cCycleParam = null;
        uint unStartTemp = 0;

        //--------------------------------
        // 総当たりでイベントリストチェック
        // m_TimeEventCycleFurtureParamは、m_TimeEventFurtureInputedで管理されている
        //--------------------------------
        for (uint num = 0; num < m_TimeEventFurtureInputed; ++num)
        {
            if (m_TimeEventFurtureID[num] != unEventID)
            {
                continue;
            }

            // 最も直近のイベントを探す
            unStartTemp = m_TimeEventCycleFurtureParam[num].timingStart;
            if (cCycleParam == null
            || cCycleParam.timingStart > unStartTemp)
            {
                // 初発見時は、とりあえず取得
                // 同イベント発見時は、開始時間を比較して取得
                cCycleParam = m_TimeEventCycleFurtureParam[num];
            }
        }

        return (cCycleParam);
    }
}
