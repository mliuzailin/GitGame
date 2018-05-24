using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using ServerDataDefine;

public class LoginBonus : MenuPartsBase
{
    /// <summary></summary>
    public Action<MAINMENU_SEQ> CloseAction = null;
    public MAINMENU_SEQ NextSequence { get; private set; }

    /// <summary>ログインボーナスの表示があったかどうか</summary>
    bool m_IsExcecLoginBonus = false;
    /// <summary>現在のログインボーナスのインデックス</summary>
    uint m_CurrentPeriodIndex = 0;
    PacketStructPeriodLogin m_CurrentLoginPeriodData;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    void Start()
    {

    }

    /// <summary>
    /// ログインボーナスに出すアイテムが存在するか
    /// </summary>
    /// <returns></returns>
    public static bool isCheck()
    {
#if UNITY_EDITOR && BUILD_TYPE_DEBUG
        //return true;
#endif

        // 月間ログインボーナスのチェック
        if (IsExistMonthlyLogin(UserDataAdmin.Instance.m_StructLoginMonthly) == true)
        {
            return true;
        }

        // 特別ログインボーナスのチェック
        if (IsExistPeriodLogin(UserDataAdmin.Instance.m_StructPeriodLogin) == true)
        {
            return true;
        }

        return false;
    }

    public static bool IsExistMonthlyLogin(PacketStructLoginMonthly login_monthly)
    {
        bool isExist = (login_monthly.monthly_list != null && login_monthly.login_list != null);
        return isExist;
    }

    public static bool IsExistPeriodLogin(PacketStructPeriodLogin[] period_login_array)
    {
        bool isExist = (period_login_array.IsNullOrEmpty() == false);
        return isExist;
    }

    // SceneStart後に実行
    public bool PostSceneStart()
    {
        //--------------------------------------------
        // 初期化
        //--------------------------------------------
        m_CurrentPeriodIndex = 0;
        LoginBonusFSM.Instance.SendFsmEvent("DO_START");
        return true;
    }

    // プレゼントID → アイテムID×個数
    private string PresentText(int[][] present_ids)
    {
        string result = "";
        foreach (int[] ids in present_ids)
        {
            foreach (int id in ids)
            {
                var present = MasterFinder<MasterDataPresent>.Instance.Find(id);
                if (present == null)
                {
#if true
                    // プレゼントがみつかりません
                    result += string.Format("[MasterDataに該当プレセントがありません ID{0}]\n", id);
#endif
                }
                else
                {
                    // プレゼント内容文字列
                    result += "\n" + string.Format(GameTextUtil.GetText("pp4q_content_2"), MasterDataUtil.GetPresentName(present), MasterDataUtil.GetPresentCount(present));
                }
            }
        }
        return result;
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    #region Play Maker Method
    void OnWait()
    {

    }

    /// <summary>
    /// 月間ログインボーナスの表示
    /// </summary>
    void OnStartMonthlyLogin()
    {
        PacketStructLoginMonthly loginMonthlyData = UserDataAdmin.Instance.m_StructLoginMonthly;
        if (IsExistMonthlyLogin(loginMonthlyData) == false)
        {
#if BUILD_TYPE_DEBUG
            DateTime todayTime;
            if (loginMonthlyData != null && loginMonthlyData.login_date > 0)
            {
                int nYear = (int)(loginMonthlyData.login_date / 100 / 100);
                int nMonth = (int)(loginMonthlyData.login_date / 100) % 100;
                int nDay = (int)(loginMonthlyData.login_date) % 100;

                todayTime = new DateTime(nYear, nMonth, nDay, 0, 0, 0);
            }
            else
            {
                todayTime = TimeManager.Instance.m_TimeNow;
            }
            string messageText = todayTime.ToString("yyyy年MM月dd日") + "の通常ログインボーナスのデータが取得できませんでした。\n"
                                + "\n"
                                + "このダイアログは、通常ログインボーナスの他にログインボーナスがある場合に表示されます。\n"
                                + "\n"
                                + "プランナーさんにマスターデータ設定が\n間違っていないか確認しください。\n"
                                + "\n不明な場合はクライアントプログラマに報告してください。";

            Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
            newDialog.SetDialogText(DialogTextType.Title, "No LoginMonthlyData");
            newDialog.SetDialogText(DialogTextType.MainText, messageText);
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button7");
            newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
            {
                LoginBonusFSM.Instance.SendFsmNextEvent();
            });
            newDialog.Show();
#else
            LoginBonusFSM.Instance.SendFsmNextEvent();
#endif
            return;
        }

        m_IsExcecLoginBonus = true;

        LoginBonusDialog dialog = LoginBonusDialog.Create();

        AndroidBackKeyManager.Instance.StackPush(gameObject, () =>
        {
            dialog.OnClickBGPanel();
        });

        dialog.SetUpMonthlyLoginList(loginMonthlyData);
        dialog.Show();
        dialog.CloseAction = () =>
        {
            AndroidBackKeyManager.Instance.StackPop(gameObject);
            LoginBonusFSM.Instance.SendFsmNextEvent();
        };
    }

    /// <summary>
    /// 特別ログインボーナスがあるかチェックする
    /// </summary>
    void OnIsExistPeriodLogin()
    {
        PacketStructPeriodLogin[] loginPeriodData = UserDataAdmin.Instance.m_StructPeriodLogin;
        if (IsExistPeriodLogin(loginPeriodData) == false)
        {
            LoginBonusFSM.Instance.SendFsmNegativeEvent();
            return;
        }

        if (m_CurrentPeriodIndex >= loginPeriodData.Length)
        {
            // 表示するイベントがなくなった
            LoginBonusFSM.Instance.SendFsmNegativeEvent();
        }
        else
        {
            // 表示の間がないので遅延させる
            StartCoroutine(DelayMethod(0.3f, () =>
            {
                // 特別ログインボーナスの表示
                LoginBonusFSM.Instance.SendFsmPositiveEvent();
            }));
        }
    }

    /// <summary>
    /// 特別ログインボーナスの表示
    /// </summary>
    void OnStartPeriodLogin()
    {
        m_CurrentLoginPeriodData = UserDataAdmin.Instance.m_StructPeriodLogin[m_CurrentPeriodIndex];
        if (m_CurrentLoginPeriodData == null)
        {
            LoginBonusFSM.Instance.SendFsmNextEvent();
        }
        else
        {
            m_IsExcecLoginBonus = true;

            LoginBonusDialog dialog = LoginBonusDialog.Create();

            AndroidBackKeyManager.Instance.StackPush(gameObject, () =>
            {
                dialog.OnClickBGPanel();
            });

            dialog.SetUpPeriodLoginList(m_CurrentLoginPeriodData);
            dialog.LoadResources(() =>
            {
                dialog.Show();
            });
            dialog.CloseAction = () =>
            {
                AndroidBackKeyManager.Instance.StackPop(gameObject);
                LoginBonusFSM.Instance.SendFsmNextEvent();
            };
        }

        ++m_CurrentPeriodIndex;
    }

    /// <summary>
    /// その他アイテムの表示
    /// </summary>
    void OnStartOthersItem()
    {
        // 本文
        List<string> mainTextList = new List<string>();

        // その他の入手アイテム
        long[] othersIDList = UserDataAdmin.Instance.m_StructOthersPresent.serial_numbers;
        List<MasterDataPresent> messageList = new List<MasterDataPresent>();
        List<MasterDataPresent> othersList = new List<MasterDataPresent>();

        if (othersIDList != null)
        {
            for (int i = 0; i < othersIDList.Length; i++)
            {
                PacketStructPresent tmp = UserDataAdmin.Instance.SearchPresent(othersIDList[i]);
                MasterDataPresent presentMaster = MasterDataUtil.ConvertStructPresentToMasterData(tmp);
                if (presentMaster != null)
                {
                    switch (presentMaster.present_type)
                    {
                        case MasterDataDefineLabel.PresentType.MSG:
                        case MasterDataDefineLabel.PresentType.NOTICE:
                            messageList.Add(presentMaster);
                            break;
                        default:
                            othersList.Add(presentMaster);
                            break;
                    }
                }
            }
        }

        // その他の入手アイテムが1個以上ある場合は表示する
        if (0 < othersList.Count || 0 < MainMenuParam.m_LoginFriendPointGet)
        {
            // タイトル
            string othreText = string.Format(GameTextUtil.GetText("pp4q_title_3"));

            // プレゼントの初回表示
            if (0 < othersList.Count)
            {
                // アイテム
                foreach (var presentMaster in othersList)
                {
                    string text = string.Format(GameTextUtil.GetText("pp4q_content_2"),
                        MasterDataUtil.GetPresentName(presentMaster),
                        MasterDataUtil.GetPresentCount(presentMaster));

                    othreText += "\n" + text;
                }
            }

            // 運営からのお知らせ
            if (messageList != null && messageList.Count() > 0)
            {
                var presentMaster = messageList.First();
                string text = string.Format(GameTextUtil.GetText("pp4q_content_2"),
                    MasterDataUtil.GetPresentName(presentMaster),
                    messageList.Count());

                othreText += "\n" + text;
            }

            // 助っ人経由で入手した友情ポイント
            if (0 < MainMenuParam.m_LoginFriendPointGet)
            {
                // {0}人の助っ人をしました。友情ポイント×｛1｝
                string text = string.Format(GameTextUtil.GetText("pp4q_content_1"),
                MainMenuParam.m_LoginFriendHelpCt,     //!< ログイン情報：フレンド：助っ人として助けた人数
                MainMenuParam.m_LoginFriendPointGet);    //!< ログイン情報：フレンド：フレンドポイント：今回取得分
                othreText += "\n" + text;
            }

            mainTextList.Add(othreText);
        }

        if (mainTextList.IsNullOrEmpty() == true)
        {
            LoginBonusFSM.Instance.SendFsmNextEvent();
        }
        else
        {
            // 本文の連結
            string MainText = string.Join("\n\n", mainTextList.ToArray());

            // ダイアログ設定
            Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
            newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("pp4q_title_0"));
            newDialog.SetTextAlignment(DialogTextType.MainText, TMPro.TextAlignmentOptions.Center);
            newDialog.SetDialogText(DialogTextType.MainText, MainText);
            newDialog.SetDialogText(DialogTextType.OKText, GameTextUtil.GetText("common_button1"));
            newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
            {
                LoginBonusFSM.Instance.SendFsmNextEvent();
            }));

            // ダイアログ表示
            newDialog.Show();
        }
    }

    void OnSetBeginnerBoost()
    {
        var boost = MasterDataUtil.GetMasterDataBeginnerBoost();
        if (boost == null)
        {
            // ホーム画面へ遷移
            NextSequence = MAINMENU_SEQ.SEQ_HOME_MENU;
        }
        else
        {
            if (m_IsExcecLoginBonus == false)
            {
                // ホーム画面へ遷移
                NextSequence = MAINMENU_SEQ.SEQ_HOME_MENU;
            }
            else
            {
                // 初心者ブーストへ遷移
                NextSequence = MAINMENU_SEQ.SEQ_BEGINNER_BOOST;
            }
        }

        LoginBonusFSM.Instance.SendFsmNextEvent();
    }

    void OnClose()
    {
        CloseAction(NextSequence);
    }
    #endregion


}
