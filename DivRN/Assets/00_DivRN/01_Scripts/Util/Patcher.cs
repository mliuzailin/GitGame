using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class Patcher : SingletonComponent<Patcher>
{
    // サーバーとリソースの参照先切り替えアプリのバージョン（Platformごと）
    public bool isNextVersion()
    {
#if BUILD_TYPE_DEBUG
        if (DebugOption.Instance.noneNextVersion)
        {
            return false;
        }
#endif

        string next = GetStr("APP_NEXT_VERSION_" + NOUtil.Platform.ToUpper());

        if (next.IsNullOrEmpty())
        {
            return false;
        }

        if (next != GlobalDefine.StrVersion())
        {
            return false;
        }

        return true;
    }

    // プレイを許容するアプリのバージョン（Platformごと）
    public bool IsCompitableVersion(string v)
    {
#if UNITY_EDITOR && BUILD_TYPE_DEBUG 
        if (DebugOption.Instance.patcherDO.alwaysCompitableAppVersion)
        {
            return true;
        }
#endif
#if UNITY_EDITOR
        List<string> list = GetAppVersionMulti();
        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log("GetAppVersionMulti: " + i + ":" + list[i] + " / " + v);
        }
#endif
        return GetAppVersionMulti().Contains(v);
    }

    // アプリの許容バージョンリスト（Platformごと）
    public List<string> GetAppVersionMulti()
    {
        return GetListStr("APP_VERSION_PATCH_MULTI_" + NOUtil.Platform.ToUpper());
    }

    // プライバシーポリシーのバージョン（Platformごと）
    public string GetPrivacyPolicy()
    {
        return GetStr("PRIVACYPOLICY_" + NOUtil.Platform.ToUpper());
    }

    // 利用規約のバージョン（Platformごと）
    public string GetPolicy()
    {
        return GetStr("USERPOLICY_" + NOUtil.Platform.ToUpper());
    }

    // パッチファイル更新カウンター（Platformごと）
    public int GetUpdateCounter()
    {
        return GetNum("DATA_UPDATE_COUNTER_" + NOUtil.Platform.ToUpper());
    }

    // プライバシーポリシーページURL
    public string GetPolicyUrl()
    {
        return GetStr("POLICY_URL");
    }

    // 利用規約ページURL
    public string GetAgreementUrl()
    {
        return GetStr("AGREEMENT_URL");
    }

    // アプリ内ブラウザで表示するURL設定
    public List<string> GetInBrowserUrls()
    {
        if (GetStr("IN_BROWSERL_URLS") != null)
        {
            return GetListStr("IN_BROWSERL_URLS");
        }
        else
        {
            return new List<string>();
        }
    }

    // 日に一度、タイトルに戻す時間を設定
    public string GetChangeDateTime()
    {
        return GetStr("CHANGE_DATE_TIME");
    }

    // 何回画面切り替えを行ったらプレゼント取得を行うか
    public int GetPresentRequestWaitNum()
    {
        int num = GetNum("PRESENT_REQUEST_WAIT_NUM");

        if (num == 0)
        {
            num = 50;
        }

        return num;
    }

    // ODINへの送信を止める
    public bool DisableOdin()
    {
        return GetBool("DISABLE_ODIN_SYSTEM");
    }

    // ディスクの空き容量警告（新規ユーザー）
    public int GetDiskSizeLimitNewUser()
    {
        int num = GetNum("DISK_SIZE_LIMIT_NEW_USER");

        if (num == 0)
        {
            num = 80;
        }

        return num;
    }

    // ディスクの空き容量警告（既存ユーザー）
    public int GetDiskSizeLimitExistingUser()
    {
        int num = GetNum("DISK_SIZE_LIMIT_EXISTING_USER");

        if (num == 0)
        {
            num = 20;
        }

        return num;
    }

    // ローカル通知無効化:登録
    public bool GetLocalNotificationRegisterDisable()
    {
        return GetBool("LOCAL_NOTIFICATION_REGISTER_DISABLE");
    }

    // ローカル通知無効化:キャンセル
    public bool GetLocalNotificationCancelDisable()
    {
        return GetBool("LOCAL_NOTIFICATION_CANCEL_DISABLE");
    }

    // ローカル通知期間:開始
    public string GetLocalNotificationCancelStart()
    {
        return GetStr("LOCAL_NOTIFICATION_CANCEL_START");
    }

    // ローカル通知期間:終了
    public string GetLocalNotificationCancelEnd()
    {
        return GetStr("LOCAL_NOTIFICATION_CANCEL_END");
    }


    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    private Dictionary<string, string> dict = new Dictionary<string, string>();

    private string URL
    {
        get
        {
            return GlobalDefine.GetPatchUrl();
        }
    }


    public bool IsLoaded
    {
        get
        {
            return dict.Count > 0;
        }
    }

    public void clear()
    {
        if (dict != null)
        {
            dict.Clear();
        }
    }

    public void Load(Action finishAction, Action<string> failAction)
    {
        StartCoroutine(_Load(finishAction, failAction));
    }

    private IEnumerator _Load(Action finishAction, Action<string> failAction)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("PATCH URL:" + URL);
#endif

        WWW www = new WWW(URL);
        yield return www;

#if UNITY_EDITOR && BUILD_TYPE_DEBUG 
        if (DebugOption.Instance.patcherDO.alwaysFail)
        {
            failAction(www.error);
            yield break;
        }
#endif

        if (www.error.IsNOTNullOrEmpty() || www.text.IsNullOrEmpty())
        {
            failAction(www.error);
            yield break;
        }

        dict.Clear();

        string text = www.text;

#if BUILD_TYPE_DEBUG
        Debug.Log("PATCH TEXT:" + text);
#endif

        try
        {
            foreach (object obj in MiniJSON.Json.Deserialize(text) as List<object>)
            {
                Dictionary<string, object> d = obj as Dictionary<string, object>;
                dict.Add(d["key"].ToString(), d["param"].ToString());
            }
        }
        catch (Exception e)
        {
            failAction("Patchファイルのフォーマットがおかしいようです");
            yield break;
        }

        finishAction();
    }

    private List<string> GetListStr(string key)
    {
        return GetStr(key).Replace("[", "").Replace("]", "").Split(',').ToList();
    }

    private string GetStr(string key)
    {
        if (dict != null &&
            dict.ContainsKey(key))
        {
            return dict[key];
        }
        return null;
    }

    private int GetNum(string key)
    {
        return GetStr(key).ToInt(0);
    }

    private bool GetBool(string key)
    {
        return GetNum(key) == 1;
    }

    public void IsUpdateMoveTitle(Action updateAction, Action nextAction)
    {
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.PATCH);
        }

        Load(
        () =>
        {
            if (LoadingManager.Instance != null)
            {
                LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.PATCH);
            }

            if (LocalSaveManagerRN.Instance.PatcherCounter < GetUpdateCounter())
            {
#if BUILD_TYPE_DEBUG 
                Debug.Log("Update Counter Change!!! Goto Title!!");
#endif
                DialogManager.Open1B("ERROR_MSG_UPDATE_TITLE", "ERROR_MSG_UPDATE", "common_button1", true, true).
                SetDialogEvent(DialogButtonEventType.OK, () =>
                {

                    if (updateAction != null)
                    {
                        updateAction();
                    }

                    LocalSaveManagerRN.Instance.PatcherCounter = GetUpdateCounter();
                    LocalSaveManagerRN.Instance.Save();

                    SceneCommon.Instance.GameToTitle();
                }).
                DisableCancelButton();
            }
            else
            {
                if (nextAction != null)
                {
                    nextAction();
                }
            }
        },
        (error) =>
        {
            if (LoadingManager.Instance != null)
            {
                LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.PATCH);
            }

            Debug.LogError("ERROR:" + error);
            DivRNUtil.ShowRetryDialog(
            () =>
            {
                IsUpdateMoveTitle(updateAction, nextAction);
            });
        });
    }

#if BUILD_TYPE_DEBUG
    // パッチのフォーマットチェック
    public string checkFormat()
    {

        string errors = "";

        try
        {
            isNextVersion();
        }
        catch (Exception e)
        {
            errors += "「サーバーとリソースの参照先切り替えアプリのバージョン（Platformごと）」\n";
        }

        try
        {
            IsCompitableVersion("0");
        }
        catch (Exception e)
        {
            errors += "「プレイを許容するアプリのバージョン（Platformごと）」\n";
        }

        try
        {
            GetPrivacyPolicy();
        }
        catch (Exception e)
        {
            errors += "「プライバシーポリシーのバージョン（Platformごと）」\n";
        }

        try
        {
            GetPolicy();
        }
        catch (Exception e)
        {
            errors += "「利用規約のバージョン（Platformごと）」\n";
        }

        try
        {
            GetUpdateCounter();
        }
        catch (Exception e)
        {
            errors += "「パッチファイル更新カウンター（Platformごと）」\n";
        }

        try
        {
            GetPolicyUrl();
        }
        catch (Exception e)
        {
            errors += "「プライバシーポリシーページURL」\n";
        }

        try
        {
            GetAgreementUrl();
        }
        catch (Exception e)
        {
            errors += "「利用規約ページURL」\n";
        }

        try
        {
            GetInBrowserUrls();
        }
        catch (Exception e)
        {
            errors += "「アプリ内ブラウザで表示するURL設定」\n";
        }

        try
        {
            GetChangeDateTime();
        }
        catch (Exception e)
        {
            errors += "「日に一度、タイトルに戻す時間を設定」\n";
        }

        try
        {
            GetPresentRequestWaitNum();
        }
        catch (Exception e)
        {
            errors += "「何回画面切り替えを行ったらプレゼント取得を行うか」\n";
        }

        try
        {
            DisableOdin();
        }
        catch (Exception e)
        {
            errors += "「ODINへの送信を止める」\n";
        }

        try
        {
            GetDiskSizeLimitNewUser();
        }
        catch (Exception e)
        {
            errors += "「ディスクの空き容量警告（新規ユーザー）」\n";
        }

        try
        {
            GetDiskSizeLimitExistingUser();
        }
        catch (Exception e)
        {
            errors += "「ディスクの空き容量警告（既存ユーザー）」\n";
        }

        try
        {
            GetLocalNotificationRegisterDisable();
        }
        catch (Exception e)
        {
            errors += "「ローカル通知無効化:登録」\n";
        }

        try
        {
            GetLocalNotificationCancelDisable();
        }
        catch (Exception e)
        {
            errors += "「ローカル通知無効化:キャンセル」\n";
        }

        try
        {
            GetLocalNotificationCancelStart();
        }
        catch (Exception e)
        {
            errors += "「ローカル通知期間:開始」\n";
        }

        try
        {
            GetLocalNotificationCancelEnd();
        }
        catch (Exception e)
        {
            errors += "「ローカル通知期間:終了」\n";
        }

        if (errors.Length > 0)
        {
            errors += string.Format("\n\n{0}プラットフォームの設定に問題があるようです\n" +
                                    "patchファイルを修正してアップロードしてください\n" +
                                    "設定詳細は「Patchファイル詳細仕様書」を確認してください",
                                     NOUtil.Platform);
        }

        return errors;
    }
#endif
}
