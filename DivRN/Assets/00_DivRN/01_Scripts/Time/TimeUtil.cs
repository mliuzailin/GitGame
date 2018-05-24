/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	TimeUtil.cs
	@brief	時間関連ユーティリティ
	@author Developer
	@date 	2013/05/24
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
	@brief	時間関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class TimeUtil
{
    public const int BIT_SUNDAY = (1);          //!< ビット曜日：日
    public const int BIT_SATURDAY = (2);            //!< ビット曜日：土
    public const int BIT_FRIDAY = (3);          //!< ビット曜日：金
    public const int BIT_THURSDAY = (4);            //!< ビット曜日：木
    public const int BIT_WEDNESDAY = (5);           //!< ビット曜日：水
    public const int BIT_TUESDAY = (6);         //!< ビット曜日：火
    public const int BIT_MONDAY = (7);          //!< ビット曜日：月
    public const int BIT_DAY_OF_WEEK_MAX = (8);         //!< ビット曜日：

    public const int CONV_VAL_YEAR = (1000000);     //!< 変換格納用(YYYY/MM/DD/HH)：年
    public const int CONV_VAL_MONTH = (10000);      //!< 変換格納用(YYYY/MM/DD/HH)：月
    public const int CONV_VAL_DAY = (100);      //!< 変換格納用(YYYY/MM/DD/HH)：日

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	今回フレームのデルタタイムのデフォルト時比率を算出
	*/
    //----------------------------------------------------------------------------
    static public float GetDeltaTimeRate()
    {
        return (Time.deltaTime / (1.0f / TimeManager.FPS));
    }



    //----------------------------------------------------------------------------
    /*!
		@brief	サーバー時間をローカル時間に変換
		@note	
		@param[in]	ulong		( unServerTime			) サーバー時間
		@param[out]	DateTime	( ---------------------	) ローカル時間
	*/
    //----------------------------------------------------------------------------
    static public DateTime ConvertServerTimeToLocalTime(ulong unServerTime)
    {
        //-------------------------------
        // 1970年1月1日0時に該当する時間クラスを生成。
        // （1970年1月1日0時はUTC時間の基準）
        //
        //
        // ※東京の場合はUTC時間から9時間の時差がある
        //-------------------------------
        DateTime cTimeDate = new DateTime(1970, 1, 1, 9, 0, 0, System.DateTimeKind.Utc);

        //-------------------------------
        // 
        //-------------------------------
        cTimeDate = cTimeDate.AddSeconds(unServerTime);

        return cTimeDate;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ローカル時間をサーバー時間に変換
		@note	
		@param[in]	DateTime	( cLocalTime			) ローカル時間
		@param[out]	ulong		( ---------------------	) サーバー時間
	*/
    //----------------------------------------------------------------------------
    static public ulong ConvertLocalTimeToServerTime(DateTime cLocalTime)
    {
        //-------------------------------
        // 1970年1月1日0時を基準とする世界時間からの経過時間に変換
        //
        //
        // ※東京の場合はUTC時間から9時間の時差がある
        //-------------------------------
        TimeSpan cDeltaSpan = cLocalTime - new DateTime(1970, 1, 1, 9, 0, 0, System.DateTimeKind.Utc);

        //-------------------------------
        // 経過時間を秒数に変換して返す
        //-------------------------------
        return (ulong)cDeltaSpan.TotalSeconds;
    }


    //----------------------------------------------------------------------------
    /*
		@brief	経過秒数からサーバー時間を取得
		@param[in]	ulong		( totalSeconds		) 経過秒数
		@param[out]	DateTime	( totalSeconds		) サーバー時間
	*/
    //----------------------------------------------------------------------------
    static public DateTime ConvertTotalSecondsToServerTime(ulong totalSeconds)
    {

        DateTime cTimeDate = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        cTimeDate = cTimeDate.AddSeconds(totalSeconds);

        return cTimeDate;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	年月日時が連続したデータから時間を算出
	*/
    //----------------------------------------------------------------------------
    static public DateTime GetDateTime(uint nTiming)
    {
        int nYear = (int)(nTiming / 100 / 100 / 100);
        int nMonth = (int)(nTiming / 100 / 100) % 100;
        int nDay = (int)(nTiming / 100) % 100;
        int nHour = (int)(nTiming % 100);

        if (nHour == 24)
        {
            return new DateTime(nYear, nMonth, nDay, 23, 59, 59);
        }
        else
        {
            return new DateTime(nYear, nMonth, nDay, nHour, 0, 0);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	年月日時分が連続したデータから時間を算出
		@note	nTiming:「YYYYMMDDhhmmss」の形式
	*/
    //----------------------------------------------------------------------------
    static public DateTime GetDateTimeMin(long nTiming)
    {
        int nYear = (int)(nTiming / 100 / 100 / 100 / 100);
        int nMonth = (int)(nTiming / 100 / 100 / 100) % 100;
        int nDay = (int)(nTiming / 100 / 100) % 100;
        int nHour = (int)(nTiming / 100 % 100);
        int nMin = (int)(nTiming % 100);

        if (nHour >= 24)
        {
            return new DateTime(nYear, nMonth, nDay, 23, 59, 59);
        }
        else
        {
            return new DateTime(nYear, nMonth, nDay, nHour, nMin, 00);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	年月日時分秒が連続したデータから時間を算出
		@note	nTiming:「YYYYMMDDhhmmss」の形式
	*/
    //----------------------------------------------------------------------------
    static public DateTime GetDateTimeSec(long nTiming)
    {
        int nYear = (int)(nTiming / 100 / 100 / 100 / 100 / 100);
        int nMonth = (int)(nTiming / 100 / 100 / 100 / 100) % 100;
        int nDay = (int)(nTiming / 100 / 100 / 100) % 100;
        int nHour = (int)(nTiming / 100 / 100) % 100;
        int nMin = (int)(nTiming / 100 % 100);
        int nSec = (int)(nTiming % 100);

        if (nHour == 24)
        {
            return new DateTime(nYear, nMonth, nDay, 23, 59, 59);
        }
        else
        {
            return new DateTime(nYear, nMonth, nDay, nHour, nMin, nSec);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	年月日が連続したデータから曜日を算出
	*/
    //----------------------------------------------------------------------------
    static public int GetDayOfWeek(uint unDate)
    {
        int nYear = (int)(unDate / 100 / 100 / 100);
        int nMonth = (int)(unDate / 100 / 100) % 100;
        int nDay = (int)(unDate / 100) % 100;
        //int nHour = (int)(unDate % 100);

        DateTime cDateTime = new DateTime(nYear, nMonth, nDay);

        return (int)cDateTime.DayOfWeek;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	表示用イベント終了日取得	(v360イベントスケジュールから移動)
		@note	終了日に「2016/1/1 00時」が設定されている場合は「2015/12/31 23:59」表示
		@note	終了日に「2015/12/31 24時」が設定されている場合は「2015/12/31 23:59」表示
	*/
    //----------------------------------------------------------------------------
    static public DateTime GetShowEventEndTime(uint nTimingEnd)
    {
        int nEndYear = (int)(nTimingEnd / 100 / 100 / 100);
        int nEndMonth = (int)(nTimingEnd / 100 / 100) % 100;
        int nEndDay = (int)(nTimingEnd / 100) % 100;
        int nEndHour = (int)(nTimingEnd % 100);

        DateTime cTimeEnd = new DateTime();

        if (nEndHour < 24)
        {
            cTimeEnd = new DateTime(nEndYear, nEndMonth, nEndDay, nEndHour, 0, 59);

            // -1分した日時取得
            cTimeEnd = cTimeEnd.AddMinutes(-1);
        }
        else
        {
            //「24時」を超えた値が設定されている場合は補正
            if (nEndHour > 24)
            {
                nEndHour = 24;
            }

            //「24時」指定を「23時59分」になるように補正
            cTimeEnd = new DateTime(nEndYear, nEndMonth, nEndDay, nEndHour - 1, 59, 59);
        }

        return cTimeEnd;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	時間(DateTime)から、年月日時が連続した値を算出
		@note	
		@add	Developer 2016/07/25 v360
	*/
    //----------------------------------------------------------------------------
    static public uint GetDateTimeToValue(DateTime cDataTime)
    {
        TimeManager cTimeMgr = TimeManager.Instance;
        uint unResult = 0;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (cTimeMgr == null)
        {
            return (unResult);
        }

        //--------------------------------
        // 現在時刻を算出
        //--------------------------------
        int nTimeNow;
        nTimeNow = (cTimeMgr.m_TimeNow.Year * CONV_VAL_YEAR)
                 + (cTimeMgr.m_TimeNow.Month * CONV_VAL_MONTH)
                 + (cTimeMgr.m_TimeNow.Day * CONV_VAL_DAY)
                 + (cTimeMgr.m_TimeNow.Hour);

        unResult = (uint)nTimeNow;
        return (unResult);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	指定時間(年月日時)から、年月日時が連続した値を算出
		@note	
		@add	Developer 2016/07/25 v360
	*/
    //----------------------------------------------------------------------------
    static public uint GetDateTimeToValue(int nYear, int nMonth, int nDay, int nHour)
    {
        uint unResult = 0;

        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (nYear <= 0
        || nMonth <= 0)
        {
            // システム開始日時
            unResult = (uint)1970010100;
            return (unResult);
        }

        //--------------------------------
        // 指定時刻を算出
        //--------------------------------
        int nTimeNow;
        nTimeNow = (nYear * CONV_VAL_YEAR)
                 + (nMonth * CONV_VAL_MONTH)
                 + (nDay * CONV_VAL_DAY)
                 + (nHour);

        unResult = (uint)nTimeNow;
        return (unResult);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	曜日定義(DayOfWeek)を、自前のBit定義で取得
		@note	
		@add	Developer 2016/07/25 v360
	*/
    //----------------------------------------------------------------------------
    static public int GetDayOfWeekToBitValue(int nYear = 0, int nMonth = 0, int nDay = 0)
    {
        DayOfWeek cDayOfWeek;
        int nBitDayOfWeek;


        if (nYear != 0
        && nMonth != 0
        && nDay != 0)
        {
            DateTime cDateTime = new DateTime(nYear, nMonth, nDay);
            cDayOfWeek = cDateTime.DayOfWeek;
        }
        else
        {
            cDayOfWeek = TimeManager.Instance.m_TimeNow.DayOfWeek;
        }

        // ビット曜日変換
        switch (cDayOfWeek)
        {
            case System.DayOfWeek.Monday: nBitDayOfWeek = BIT_MONDAY; break;
            case System.DayOfWeek.Tuesday: nBitDayOfWeek = BIT_TUESDAY; break;
            case System.DayOfWeek.Wednesday: nBitDayOfWeek = BIT_WEDNESDAY; break;
            case System.DayOfWeek.Thursday: nBitDayOfWeek = BIT_THURSDAY; break;
            case System.DayOfWeek.Friday: nBitDayOfWeek = BIT_FRIDAY; break;
            case System.DayOfWeek.Saturday: nBitDayOfWeek = BIT_SATURDAY; break;
            case System.DayOfWeek.Sunday: nBitDayOfWeek = BIT_SUNDAY; break;
            default: nBitDayOfWeek = BIT_MONDAY; break;
        }

        return (nBitDayOfWeek);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ガチャの引いた曜日をBit定義で取得
		@note	
	*/
    //----------------------------------------------------------------------------
    static public int GetGachaPlayDayOfWeekToBitValue(DateTime cGachaPlayTime)
    {
        DayOfWeek cDayOfWeek;
        int nBitDayOfWeek;

        cDayOfWeek = cGachaPlayTime.DayOfWeek;

        // ビット曜日変換
        switch (cDayOfWeek)
        {
            case System.DayOfWeek.Monday: nBitDayOfWeek = BIT_MONDAY; break;
            case System.DayOfWeek.Tuesday: nBitDayOfWeek = BIT_TUESDAY; break;
            case System.DayOfWeek.Wednesday: nBitDayOfWeek = BIT_WEDNESDAY; break;
            case System.DayOfWeek.Thursday: nBitDayOfWeek = BIT_THURSDAY; break;
            case System.DayOfWeek.Friday: nBitDayOfWeek = BIT_FRIDAY; break;
            case System.DayOfWeek.Saturday: nBitDayOfWeek = BIT_SATURDAY; break;
            case System.DayOfWeek.Sunday: nBitDayOfWeek = BIT_SUNDAY; break;
            default: nBitDayOfWeek = BIT_MONDAY; break;
        }

        return (nBitDayOfWeek);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	年月日時が連続したデータから、年を取得
		@add Developer 2016/07/29 v360
	*/
    //----------------------------------------------------------------------------
    static public int GetDateTimeToYear(uint unDateTime)
    {
        int nYear = (int)(unDateTime / CONV_VAL_YEAR);
        return (nYear);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	年月日時が連続したデータから、月を取得
		@add Developer 2016/07/29 v360
	*/
    //----------------------------------------------------------------------------
    static public int GetDateTimeToMonth(uint unDateTime)
    {
        int nMonth = (int)(unDateTime / CONV_VAL_MONTH) % 100;
        return (nMonth);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	年月日時が連続したデータから、日を取得
		@add Developer 2016/07/29 v360
	*/
    //----------------------------------------------------------------------------
    static public int GetDateTimeToDay(uint unDateTime)
    {
        int nDay = (int)(unDateTime / CONV_VAL_DAY) % 100;
        return (nDay);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	年月日時が連続したデータから、時を取得
		@add Developer 2016/07/29 v360
	*/
    //----------------------------------------------------------------------------
    static public int GetDateTimeToHour(uint unDateTime)
    {
        int nHour = (int)(unDateTime % 100);
        return (nHour);
    }
};
/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
