using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class MainMenuWebViewShowChk
{
    private static Dictionary<uint, uint> VALUES = null;
    const uint rank_max = 99999;

    public enum PopupWebViewType : int
    {
        None = 0,
        QuestStart,
        QuestClear,
        Mission,
    };

    public static void ResetCache()
    {
        VALUES = null;
    }

    public static bool GetViewCheck(uint fixid)
    {
        if (VALUES == null)
        {
            VALUES = LocalSaveUtilToInstallFolder.LoadWebviewCheck();
        }

        bool value = false;

        if (VALUES.ContainsKey(fixid) == true)
        {
            uint checkDay = VALUES[fixid];
            uint nowDay = TimeManager.Instance.ConvertDay(TimeManager.Instance.m_TimeNow);
            if (checkDay > nowDay)
            {
                value = true;
            }
        }

        return value;
    }

    public static void SetViewCheck(uint fixid, bool check)
    {
        if (VALUES == null)
        {
            VALUES = LocalSaveUtilToInstallFolder.LoadWebviewCheck();
        }

        if (check == true)
        {
            VALUES[fixid] = TimeManager.Instance.ConvertDay(TimeManager.Instance.m_TimeNow.AddDays(1));
        }
        else
        {
            VALUES.Remove(fixid);
        }

        LocalSaveUtilToInstallFolder.SaveWebviewCheck(VALUES);
    }

    private static void WebViewDayCheck()
    {
        if (VALUES.IsNullOrEmpty() == true)
        {
            return;
        }

        uint nowDay = TimeManager.Instance.ConvertDay(TimeManager.Instance.m_TimeNow);
        List<uint> dellist = new List<uint>();
        dellist.Clear();

        foreach (KeyValuePair<uint, uint> dic in VALUES)
        {
            if (dic.Value <= nowDay)
            {
                dellist.Add(dic.Key);
            }
        }

        foreach (uint key in dellist)
        {
            VALUES.Remove(key);
        }
    }

    public static List<uint> GetPopupWebViewList(PopupWebViewType type, uint id)
    {
        List<uint> missionList = new List<uint>();
        MasterDataWebView[] masters = MasterFinder<MasterDataWebView>.Instance.GetAll();

        foreach (MasterDataWebView master in masters)
        {
            if (master.webview_type != (int)type)
            {
                if (type == PopupWebViewType.QuestClear)
                {
                    if (master.webview_type != (int)PopupWebViewType.Mission)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }

            if (master.webview_type == (int)PopupWebViewType.Mission)
            {
                ServerDataDefine.PacketAchievement achievement = ResidentParam.GetAchievementClear(master.webview_param_1);
                if (achievement == null)
                {
                    continue;
                }
            }
            else
            {
                if (master.webview_param_1 != id)
                {
                    continue;
                }
            }

            if (master.timing_start != 0)
            {
                if (TimeManager.Instance.CheckWithinTime(master.timing_start, master.timing_end) == false)
                {
                    continue;
                }
            }

            uint rank_upper_limit = master.webview_param_3;
            if (rank_upper_limit == 0)
            {
                rank_upper_limit = rank_max;
            }

            if (UserDataAdmin.Instance.m_StructPlayer.rank < master.webview_param_2
            || UserDataAdmin.Instance.m_StructPlayer.rank > rank_upper_limit)
            {
                continue;
            }

            if (GetViewCheck(master.fix_id) == true)
            {
                continue;
            }
            missionList.Add(master.fix_id);
        }
        return missionList;
    }

    public static IEnumerator PopupWebViewStart(PopupWebViewType type,
                                                uint id = 0,
                                                Action callback = null,
                                                Action closeAction = null)
    {
        if (TutorialManager.IsExists)
        {
            if (callback != null)
            {
                callback();
            }
        }
        else
        {
            WebViewDayCheck();

            List<uint> webViewList = MainMenuWebViewShowChk.GetPopupWebViewList(type, id);
            WaitForSeconds wait = new WaitForSeconds(0.5f);
            int opencount = 0;
            for (int i = 0; i < webViewList.Count; ++i)
            {
                MasterDataWebView master = MasterFinder<MasterDataWebView>.Instance.Find((int)webViewList[i]);
                if (master != null)
                {
                    WebView webView = WebView.OpenWebView(master.url_web,
                                                          closeAction,
                                                          master.fix_id);
                    while (webView.isOpen == true)
                    {
                        yield return null;
                    }

                    ResidentParam.DelAchievementClear(master.webview_param_1);
                    opencount++;
                }

                yield return wait;
            }

            if (callback != null)
            {
                callback();
            }

            if (opencount <= 0 &&
                closeAction != null)
            {
                closeAction();
            }
        }
    }

}
