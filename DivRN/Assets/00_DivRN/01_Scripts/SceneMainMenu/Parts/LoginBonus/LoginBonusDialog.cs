/// <summary>
///
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;
using DG.Tweening;

public class LoginBonusDialog : MenuPartsBase
{
    private static readonly string AppearAnimationName = "login_bonus_dialog_appear";
    private static readonly string DefaultAnimationName = "login_bonus_dialog_default";

    [SerializeField]
    GameObject m_PanelRoot;
    [SerializeField]
    GameObject m_DetailRoot;
    [SerializeField]
    public LoginBonusRecordPanel m_RecordPanel;
    [SerializeField]
    GameObject m_Mask;
    [SerializeField]
    Image m_CharaImage;
    [SerializeField]
    GameObject m_TextBG;
    [SerializeField]
    GameObject m_TextRoot;
    [SerializeField]
    MenuPartsBase m_Window;

    public LoginBonusDetailPanel m_DetailPanel;

    public Action CloseAction;

    PacketStructLoginMonthly m_LoginMonthlyData;
    PacketStructPeriodLogin m_LoginPeriodData;

    M4uProperty<string> titleText = new M4uProperty<string>();
    public string TitleText { get { return titleText.Value; } set { titleText.Value = value; } }

    M4uProperty<string> startDateText = new M4uProperty<string>();
    public string StartDateText { get { return startDateText.Value; } set { startDateText.Value = value; } }

    M4uProperty<string> endDateText = new M4uProperty<string>();
    public string EndDateText { get { return endDateText.Value; } set { endDateText.Value = value; } }

    M4uProperty<Sprite> charaImage = new M4uProperty<Sprite>();
    /// <summary>背景のキャラ画像</summary>
    public Sprite CharaImage
    {
        get { return charaImage.Value; }
        set
        {
            charaImage.Value = value;
            IsViewCharaImage = (value != null);
        }
    }

    M4uProperty<bool> isViewCharaImage = new M4uProperty<bool>();
    /// <summary>背景のキャラ画像の表示・非表示</summary>
    public bool IsViewCharaImage { get { return isViewCharaImage.Value; } set { isViewCharaImage.Value = value; } }

    bool isShow = false;
    bool isClosing = false;

    int m_LoadingCount = 0;

    public static LoginBonusDialog Create()
    {
        GameObject _tmpObj = Resources.Load<GameObject>("Prefab/LoginBonus/LoginBonusDialog");
        if (_tmpObj == null) return null;
        GameObject _newObj = Instantiate(_tmpObj);
        if (_newObj == null) return null;
        UnityUtil.SetObjectEnabledOnce(_newObj, true);
        LoginBonusDialog dialog = _newObj.GetComponent<LoginBonusDialog>();

        return dialog;
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        isShow = false;
        m_PanelRoot.transform.localScale = new Vector3(1, 0, 1);
        ScrollRect scroll = m_RecordPanel.GetComponentInChildren<ScrollRect>();
        scroll.gameObject.transform.localScale = new Vector3(1, 0, 1);
        m_Window.SetPositionAjustStatusBar(Vector2.zero);

        // 表示の一瞬アニメーションしていない間があるので、ここで設定
        m_CharaImage.gameObject.SetActive(false);
        m_TextBG.gameObject.SetActive(false);
        m_TextRoot.gameObject.SetActive(false);
    }


    // Use this for initialization
    void Start()
    {
        m_Mask.SetActive(true);

        m_RecordPanel.CloseAction = Hide;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// リソースのロード開始
    /// </summary>
    /// <param name="finishAction"></param>
    public void LoadResources(Action finishAction = null)
    {
        string url = string.Format(GlobalDefine.GetMasterResourcesUrl()
                                , UserDataAdmin.Instance.m_PeriodLoginResourceRoot
                                , m_LoginPeriodData.resource_store_name
                                , "");

        m_LoadingCount = 0;

        GetResource(url + "background.png",
            (Sprite sprite) =>
            {
                CharaImage = sprite;
            },
            () =>
            {
                CharaImage = null;
            });

        StartCoroutine(LoopCheckLoad(finishAction));
    }

    void GetResource(string url, Action<Sprite> sucess, Action error)
    {
        ++m_LoadingCount;
        WebResource.Instance.GetSprite(url,
            (Sprite sprite) =>
            {
                if (sucess != null)
                {
                    sucess(sprite);
                }
                --m_LoadingCount;
            }
            ,
            () =>
            {
                if (error != null)
                {
                    error();
                }
                --m_LoadingCount;
            });
    }

    /// <summary>
    /// リソースのロード中
    /// </summary>
    /// <param name="finishAction"></param>
    /// <returns></returns>
    IEnumerator LoopCheckLoad(Action finishAction)
    {
        while (m_LoadingCount > 0)
        {
            yield return null;
        }

        if (finishAction != null)
        {
            finishAction();
        }
    }

    /// <summary>
    /// 月間ログインボーナスリストの作成
    /// </summary>
    /// <param name="loginMonthlyData"></param>
    public void SetUpMonthlyLoginList(PacketStructLoginMonthly loginMonthlyData)
    {
        MasterDataLoginMonthlyConverted[] loginMonthlyMasterArray = loginMonthlyData.monthly_list;
        uint today; // 今日の日付
        if (loginMonthlyData != null && loginMonthlyData.login_date > 0)
        {
            today = loginMonthlyData.login_date;
        }
        else
        {
            today = (uint)(TimeManager.Instance.m_TimeNow.ToString("yyyyMMdd") + "00").ToLong(0);  // 端末時間
        }

        int loginIndex = 0;
        for (int i = 0; i < loginMonthlyMasterArray.Length; ++i)
        {
            if (loginMonthlyMasterArray[i] == null)
            {
                continue;
            }

            // 今日のデータ
            if ((int)loginMonthlyMasterArray[i].date == today)
            {
                loginIndex = i;
            }
        }

        if (loginMonthlyMasterArray.IsNullOrEmpty() == false)
        {
            //------------------------------------------------
            //開始日
            //------------------------------------------------
            uint startMonthDate = loginMonthlyMasterArray[0].date;

            int nStartYear = (int)(startMonthDate / 100 / 100);
            int nStartMonth = (int)(startMonthDate / 100) % 100;
            int nStartDay = (int)(startMonthDate) % 100;

            if (startMonthDate.ToString().Length > 8)
            {
                // TODO:Ymd形式未対応サーバー用  後で削除
                nStartYear = TimeUtil.GetDateTimeToYear(startMonthDate);
                nStartMonth = TimeUtil.GetDateTimeToMonth(startMonthDate);
                nStartDay = TimeUtil.GetDateTimeToDay(startMonthDate);
            }

            //------------------------------------------------
            //終了日
            //------------------------------------------------
            uint endMonthDate = loginMonthlyMasterArray[loginMonthlyMasterArray.Length - 1].date;

            int nEndYear = (int)(endMonthDate / 100 / 100);
            int nEndMonth = (int)(endMonthDate / 100) % 100;
            int nEndDay = (int)(endMonthDate) % 100;

            if (endMonthDate.ToString().Length > 8)
            {
                // TODO:Ymd形式未対応サーバー用 後で削除
                nEndYear = TimeUtil.GetDateTimeToYear(endMonthDate);
                nEndMonth = TimeUtil.GetDateTimeToMonth(endMonthDate);
                nEndDay = TimeUtil.GetDateTimeToDay(endMonthDate);
            }

            TitleText = GameTextUtil.GetText("loginbonus_normal_title");
            StartDateText = string.Format(GameTextUtil.GetText("loginbonus_period_start"), nStartYear, nStartMonth, nStartDay);
            StartDateText += GameTextUtil.GetText("loginbonus_period_tilde");

            EndDateText = string.Format(GameTextUtil.GetText("loginbonus_period_end"), nEndYear, nEndMonth, nEndDay);

            m_RecordPanel.DetailText = loginMonthlyMasterArray[loginIndex].message;
        }
        else
        {
#if BUILD_TYPE_DEBUG
            // 検証ユーザーの時にユーザー変更で一般ユーザーになるとTimeManager.Instance.m_TimeNowが前のままで、データがおかしくなる
            string messageText = "通常ログインボーナスのデータが取得できませんでした。\n"
                               + "\n"
                               + "端末時間とサーバー時間がずれている可能性があります。\n"
                               + "\n"
                               + "管理ツールからユーザーのステータスを「開発ユーザー」に変更するか、\n"
                               + "端末時間を現在の時間に修正してアクセスしてください。";

            Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
            newDialog.SetDialogText(DialogTextType.Title, "No LoginMonthlyData");
            newDialog.SetDialogText(DialogTextType.MainText, messageText);
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button7");
            newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
            {
            });
            newDialog.Show();
#endif
        }

        m_RecordPanel.IsViewRecordList = true;
        m_RecordPanel.m_LoginBonusIndex = (uint)loginIndex;

        for (int i = 0; i < loginMonthlyMasterArray.Length; ++i)
        {
            MasterDataLoginMonthlyConverted loginMonthlyMasterData = loginMonthlyMasterArray[i];


            LoginBonusRecordListContext record = new LoginBonusRecordListContext();

            record.present_ids = loginMonthlyMasterData.present_ids;
            record.message = loginMonthlyMasterData.message;
            record.date_count = loginMonthlyMasterData.date % 100;
            if (loginMonthlyMasterData.date.ToString().Length > 8)
            {
                // TODO:Ymd形式未対応サーバー用 後で削除
                record.date_count = (loginMonthlyMasterData.date / 100) % 100;
            }

            if (loginMonthlyMasterData.present_ids.IsNullOrEmpty() == false)
            {
                int presentID = loginMonthlyMasterData.present_ids[0];
                MainMenuUtil.GetPresentIcon(MasterDataUtil.GetPresentParamFromID((uint)presentID), sprite => {
                    record.IconImage = sprite;
                });
            }
            else
            {
                MainMenuUtil.GetPresentIcon(null, sprite => {
                    record.IconImage = sprite;
                });
            }

            record.SetMonthlyLoginState(loginMonthlyMasterData.date, loginMonthlyData.login_list, loginMonthlyMasterArray[loginIndex].date);

            record.DidSelectItem = OnSelectRecord;
            m_RecordPanel.Records.Add(record);
        }

#if BUILD_TYPE_DEBUG
        DebugDialogCheckNoPresent(m_RecordPanel.Records);
#endif
    }

    /// <summary>
    /// 期間ログインボーナスリストの作成
    /// </summary>
    /// <param name="loginPeriodData"></param>
    public void SetUpPeriodLoginList(PacketStructPeriodLogin loginPeriodData)
    {
        m_LoginPeriodData = loginPeriodData;
        //------------------------------------------------
        //開始日
        //------------------------------------------------
        int nStartYear = TimeUtil.GetDateTimeToYear(loginPeriodData.timing_start);
        int nStartMonth = TimeUtil.GetDateTimeToMonth(loginPeriodData.timing_start);
        int nStartDay = TimeUtil.GetDateTimeToDay(loginPeriodData.timing_start);
        //int nStartHour = TimeUtil.GetDateTimeToHour(loginPeriodData.timing_start);
        //int nStartMin = 0;//分を設定する箇所は今の所無いので、0固定

        //------------------------------------------------
        //終了日(設定日時から-1分した日時に加工)
        //------------------------------------------------
        DateTime cEndEvent = DateTime.MinValue;

        if (loginPeriodData.timing_end > 0)
        {
            cEndEvent = TimeUtil.GetShowEventEndTime(loginPeriodData.timing_end);
        }

        int nEndYear = 0;
        int nEndMonth = 0;
        int nEndDay = 0;
        int nEndHour = 0;
        int nEndMin = 0;
        if (cEndEvent != DateTime.MinValue)
        {
            nEndYear = cEndEvent.Year;
            nEndMonth = cEndEvent.Month;
            nEndDay = cEndEvent.Day;
            nEndHour = cEndEvent.Hour;
            nEndMin = cEndEvent.Minute;
        }

        TitleText = loginPeriodData.event_name;
        StartDateText = string.Format(GameTextUtil.GetText("loginbonus_period_start"), nStartYear, nStartMonth, nStartDay);
        StartDateText += GameTextUtil.GetText("loginbonus_period_tilde");

        EndDateText = "";
        if (cEndEvent != DateTime.MinValue)
        {
            EndDateText = string.Format(GameTextUtil.GetText("loginbonus_period_end"), nEndYear, nEndMonth, nEndDay);
        }

        string detailText = "";
        if (loginPeriodData.period_login != null && loginPeriodData.period_login.IsRange((int)loginPeriodData.login_count - 1))
        {
            detailText = loginPeriodData.period_login[(int)loginPeriodData.login_count - 1].message;
        }

        m_RecordPanel.IsViewRecordList = true;
        m_RecordPanel.DetailText = detailText;
        m_RecordPanel.m_LoginBonusIndex = loginPeriodData.login_count - 1;

        for (int i = 0; i < loginPeriodData.period_login.Length; ++i)
        {
            MasterDataPeriodLoginConverted periodLoginMaster = loginPeriodData.period_login[i];

            LoginBonusRecordListContext record = new LoginBonusRecordListContext();

            record.present_ids = periodLoginMaster.present_ids;
            record.message = periodLoginMaster.message;
            record.date_count = periodLoginMaster.date_count;

            if (periodLoginMaster.present_ids.IsNullOrEmpty() == false)
            {
                int presentID = periodLoginMaster.present_ids[0];
                MainMenuUtil.GetPresentIcon(MasterDataUtil.GetPresentParamFromID((uint)presentID), sprite => {
                    record.IconImage = sprite;
                });
            }
            else
            {
                MainMenuUtil.GetPresentIcon(null, sprite => {
                    record.IconImage = sprite;
                });
            }

            record.SetPeriodLoginState(periodLoginMaster.date_count, loginPeriodData.login_count);

            record.DidSelectItem = OnSelectRecord;
            m_RecordPanel.Records.Add(record);
        }

#if BUILD_TYPE_DEBUG
        DebugDialogCheckNoPresent(m_RecordPanel.Records);
#endif
    }

    /// <summary>
    /// 詳細画面の表示
    /// </summary>
    /// <param name="periodLoginMaster"></param>
    public void OpenDetail(LoginBonusRecordListContext item)
    {
        if (item.present_ids == null)
        {
            return;
        }

        m_RecordPanel.IsViewRecordList = false;

        m_DetailPanel = LoginBonusDetailPanel.Attach(m_DetailRoot);
        m_DetailPanel.SetUpList(item);
        m_DetailPanel.CloseAction = () =>
        {
            m_RecordPanel.IsViewRecordList = true;
        };
        m_DetailPanel.Show();
    }

    void OnSelectRecord(LoginBonusRecordListContext item)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        OpenDetail(item);
    }

    public void Show(Action callback = null)
    {
        if (isShow == true)
        {
            return;
        }

        isShow = true;
        m_Mask.SetActive(true);

        //---------------------------------
        // ダイアログを開く
        //---------------------------------
        m_PanelRoot.transform.DOScaleY(1, 0.25f).OnComplete(() =>
        {
            //---------------------------------
            // アニメーション開始
            //---------------------------------
            m_RecordPanel.m_LastUpdateCount = 5;

            PlayAnimation(AppearAnimationName, () =>
            {
                m_RecordPanel.m_LastUpdateCount = 5;
                PlayAnimation(DefaultAnimationName);
                //---------------------------------
                // アイコンのアニメーション開始
                //---------------------------------
                LoginBonusRecordListItem[] recordArray = m_RecordPanel.GetComponentsInChildren<LoginBonusRecordListItem>();
                LoginBonusRecordListItem record = Array.Find(recordArray, (v) => v.Context.IsToday == true);
                if (record == null)
                {
                    m_Mask.SetActive(false);
                }
                else
                {
                    record.SetUpCheckAnim(() =>
                    {
                        OpenDetail(record.Context);
                        m_Mask.SetActive(false);
                        if (callback != null)
                        {
                            callback();
                        }
                    });
                }
            });

            RegisterKeyEventCallback("light_big_down", () =>
            {
                SoundUtil.PlaySE(SEID.SE_INGAME_QUEST_START_00);
            });
        });
    }

    public void OnClickBGPanel()
    {
        if (m_DetailPanel != null)
        {
            m_DetailPanel.Hide();
        }
        else
        {
            Hide();
        }
    }

    void Hide()
    {
        if (EnableClose() == false)
        {
            return;
        }

        isClosing = true;
        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        if (CloseAction != null)
        {
            CloseAction();
        }

        m_PanelRoot.transform.DOScaleY(0f, 0.25f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    /// <summary>
    /// ダイアログを閉じられる状態かどうか
    /// </summary>
    /// <returns></returns>
    public bool EnableClose()
    {
        if (isClosing == true
            || m_Mask.IsActive() == true)
        {

            return false;
        }

        return true;
    }

    void DebugDialogCheckNoPresent(List<LoginBonusRecordListContext> records)
    {
#if BUILD_TYPE_DEBUG
        string debugNoPresentText = "";
        foreach (var item in records)
        {
            if (item.present_ids.IsNullOrEmpty() == true)
            {
                debugNoPresentText += string.Format("\n・{0}日目", item.date_count.ToString());
            }
        }

        if (debugNoPresentText != "")
        {
            string messageText = "プレゼント情報がない日があります。\n"
                                + "\n"
                                + "プランナーさんにマスターデータ設定が\n間違っていないか確認しください。\n"
                                + "不明な場合はクライアントもしくはサーバに報告してください。\n"
                                + "\n"
                                + "以下の日にプレゼント情報がありません。"
                                + debugNoPresentText;

            Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
            newDialog.SetDialogText(DialogTextType.Title, "No PresentData");
            newDialog.SetDialogText(DialogTextType.MainText, messageText);
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button7");
            newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
            {
            });
            newDialog.Show();
        }
#endif
    }
}
