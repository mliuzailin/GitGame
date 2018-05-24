using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class DateText
{
    enum Labels : int
    {
        Endless,
        Closed,
        Days,
        Hours,
        Minutes
    }

    private static string[] shortFormat = new string[]{
            "ENDLESS",
            "CLOSED",
            "あと{3}日",
            "あと{4}時間",
            "あと{5}分"
    };

    private static string[] midiumFormat = new string[]{
            "ENDLESS",
            "CLOSED",
            "{0:D4}/{1:D2}/{2:D2}",
            "{4:D2}:{5:D2}",
            "{4:D2}:{5:D2}"
    };

    private static string[] debugFormat = new string[]{
            "ENDLESS",
            "C:{1:D2}/{2:D2}",
            "D:{1:D2}/{2:D2}",
            "H:{1:D2}/{2:D2}",
            "M:{1:D2}/{2:D2}",
    };

    // 日付情報から残り時間を所定のフォーマットで返却　無制限判定なし
    public static DateTime IsDateTime(long timing)
    {
        return TimeUtil.ConvertTotalSecondsToServerTime((ulong)timing);
    }

    // 残り時間の文字列：アイコンなど、「短いスペース」で使用
    public static string DateTextDebug(long timing)
    {
        return defaultDateText(timing, debugFormat);
    }

    // 残り時間の文字列：アイコンなど、「短いスペース」で使用
    public static string DateTextShort(long timing)
    {
        return defaultDateText(timing, shortFormat);
    }

    // 残り時間の文字列：Recordなど、「標準的なスペース」で使用
    public static string DateTextMidium(long timing)
    {
        return defaultDateText(timing, midiumFormat);
    }

    // 日付情報から残り時間を所定のフォーマットで返却
    private static string defaultDateText(long timing, string[] formats)
    {
        Labels label;
        string result;

        // アイテム受領期間までの残り時間が無期限の場合
        if (timing == 1)
        {
            // 無制限
            result = formats[(int)Labels.Endless];
        }
        else
        {
            // 日付情報から残り時間を所定のフォーマットで返却
            //DateTime date = TimeUtil.GetShowEventEndTime((uint)timing);
            DateTime date = TimeUtil.ConvertTotalSecondsToServerTime((ulong)timing);

            // 現在時刻との差分
            TimeSpan ts = date - TimeManager.Instance.m_TimeNow;

            // 期間内
            if (TimeSpan.Zero < ts)
            {
                if (1 < ts.Days)
                {
                    // 期間まで、残り1日（24時間）以上
                    label = Labels.Days;
                }
                else if (1 < ts.Hours)
                {
                    // 期間まで、残り24時間未満
                    label = Labels.Hours;
                }
                else
                {
                    // 期間まで、残り1時間未満
                    label = Labels.Minutes;
                }
            }
            else
            {
                // 期限切れ
                label = Labels.Closed;
            }
            result = String.Format(formats[(int)label], date.Year, date.Month, date.Day, ts.Days, ts.Hours, ts.Minutes);
        }
        return result;
    }
}
