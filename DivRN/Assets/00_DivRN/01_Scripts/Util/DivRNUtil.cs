using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class DivRNUtil
{

    public static void ShowRetryDialog(Action action)
    {
        bool lastInputlock = false;
        if (TutorialManager.IsExists)
        {
            lastInputlock = TutorialManager.Instance.LastInputLock;
            TutorialManager.Instance.InputLock(false);
        }

        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "ERROR_RETRY_LIMIT_TITLE");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "ERROR_RETRY_LIMIT");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
            action();

            if (TutorialManager.IsExists)
            {
                TutorialManager.Instance.InputLock(lastInputlock);
            }
        }));
        newDialog.Show();
    }

    public static void ShowQuitApplicationDialog()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        string title = String.Format(GameTextUtil.GetText("error_epplication_quit_title"), 0);
        newDialog.SetDialogText(DialogTextType.Title, title);
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "error_epplication_quit_content");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
            Application.Quit();
        }));
        newDialog.Show();
    }

    public static string UnitIdToString(uint id)
    {
        if (id < 10000)
        {
            return string.Format("{0:D4}", id);
        }

        if (id < 100000)
        {
            return string.Format("{0:D5}", id);
        }
        return null;
    }

    public static string UnitIdToStringMin(uint id)
    {
        if (id < 1000)
        {
            return string.Format("{0:D3}", id);
        }

        if (id < 10000)
        {
            return string.Format("{0:D4}", id);
        }

        if (id < 100000)
        {
            return string.Format("{0:D5}", id);
        }
        return null;
    }

    public static string DiffTimeLabel(DateTime date_time, DateTime now_time)
    {
        int totalMinutes = Convert.ToInt32(date_time.Subtract(now_time).TotalMinutes);
        if (totalMinutes < 0)
        {
            return "0分";
        }

        if (totalMinutes < 60)
        {
            return totalMinutes + "分";
        }
        if (totalMinutes < 60 * 24)
        {
            return (totalMinutes / 60) + "時間";
        }
        return (totalMinutes / 60 / 24) + "日";
    }

    /// <summary>
    /// 受け取り期限の表示
    /// </summary>
    /// <returns></returns>
    public static string DiffTimePresentLabel(DateTime date_time, DateTime now_time)
    {
        // 日付情報から残り時間を所定のフォーマットで返却する
        string result;

        // 現在時刻との差分
        TimeSpan ts = date_time.Subtract(now_time);

        // 期間内
        if (TimeSpan.Zero < ts)
        {
            if (1 <= ts.Days)
            {
                // 期間まで、残り1日（24時間）以上
                result = string.Format(GameTextUtil.GetText("common_duedate"), ts.Days);
            }
            else
            {
                // 期間まで、残り1日（24時間）未満
                result = string.Format(GameTextUtil.GetText("common_duetime"), string.Format("{0:D2}:{1:D2}", ts.Hours, ts.Minutes));
            }
        }
        else
        {
            // 期限切れ
            result = GameTextUtil.GetText("common_duetimeout");
        }

        return result;
    }

    /// <summary>
    /// 挑戦期限の表示
    /// </summary>
    /// <returns></returns>
    public static string DiffTimeMissonLabel(DateTime date_time, DateTime now_time)
    {
        // 日付情報から残り時間を所定のフォーマットで返却する
        string result;

        // 現在時刻との差分
        TimeSpan ts = date_time.Subtract(now_time);

        // 期間内
        if (TimeSpan.Zero < ts)
        {
            if (1 <= ts.Days)
            {
                // 期間まで、残り1日（24時間）以上
                result = string.Format(GameTextUtil.GetText("common_datetime"), ts.Days);
            }
            else
            {
                // 期間まで、残り1日（24時間）未満
                result = string.Format(GameTextUtil.GetText("common_timecount"), string.Format("{0:D2}:{1:D2}", ts.Hours, ts.Minutes));
            }
        }
        else
        {
            // 期限切れ
            result = string.Format(GameTextUtil.GetText("common_timecount"), "00:00");
        }

        return result;
    }

    private static Dictionary<string, string> TableNameCache = new Dictionary<string, string>();

    public static void ResetTableName()
    {
        TableNameCache = new Dictionary<string, string>();
    }

    public static string GetTableName(string str)
    {
        if (TableNameCache.ContainsKey(str))
        {
            return TableNameCache[str];
        }

        string result = str.SnakeCase().Replace("master_data_", "") + "_master";

        if (result.StartsWith("param_"))
        {
            result = result.Replace("param_", "");
        }

        switch (result)
        {
            case "quest2_master":
                result = "renew_quest_master";
                break;
            case "status_ailment_param_master":
                result = "status_ailment_master";
                break;
            case "use_item_master":
                result = "item_master";
                break;
            case "quest_requirement_master":
                result = "quest_requirement";
                break;
            case "skill_limit_break_master":
                result = "skill_limitbreak_master";
                break;
            case "quest_2_master":
                result = "renew_quest_master";
                break;
            default:
                break;
        }

        TableNameCache[str] = result;

        return result;
    }
}
