using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class EventSchedule : MenuPartsBase
{
    public class EventScheduleInfo
    {
        public MasterDataEvent m_MasterDataEvent = null;        //!< イベントマスターデータ

        public string m_AreaName = "";      //!< エリアマスターデータ：エリア名
        public string m_AreaUrl = "";       //!< エリアマスターデータ：URL
    }

    // カラーパレット
    // "#E03F6CFF", "#FFBF00FF", "#82B312FF"
    public Color[] IconColor;
    // "#FFA6C1C0", "#F1E990C0", "#BDF291C0"
    public Color[] FrameColor;

    // ボタン画像
    //public Sprite[] RecordButtonImage;

    M4uProperty<string> scheduleTab1 = new M4uProperty<string>("");
    M4uProperty<string> scheduleTab2 = new M4uProperty<string>("");
    public string ScheduleTab1 { get { return scheduleTab1.Value; } set { scheduleTab1.Value = value; } }
    public string ScheduleTab2 { get { return scheduleTab2.Value; } set { scheduleTab2.Value = value; } }

    // レコードの追加先
    M4uProperty<List<EventRecordListItemContex>> records0 = new M4uProperty<List<EventRecordListItemContex>>(new List<EventRecordListItemContex>());
    public List<EventRecordListItemContex> Records0 { get { return records0.Value; } set { records0.Value = value; } }

    M4uProperty<int> recordCount0 = new M4uProperty<int>();
    public int RecordCount0 { get { return recordCount0.Value; } set { recordCount0.Value = value; } }

    M4uProperty<List<EventRecordListItemContex>> records1 = new M4uProperty<List<EventRecordListItemContex>>(new List<EventRecordListItemContex>());
    public List<EventRecordListItemContex> Records1 { get { return records1.Value; } set { records1.Value = value; } }

    M4uProperty<int> recordCount1 = new M4uProperty<int>();
    public int RecordCount1 { get { return recordCount1.Value; } set { recordCount1.Value = value; } }

    M4uProperty<bool> isViewContents = new M4uProperty<bool>(true);
    /// <summary>イベントリストの表示・非表示</summary>
    public bool IsViewContents { get { return isViewContents.Value; } set { isViewContents.Value = value; } }

    // ダイアログ
    public GameObject DialogInfo;
    public GameObject DialogInfoRoot;

    // ページ切り替え
    M4uProperty<int> viewPage = new M4uProperty<int>(0);
    public int ViewPage { get { return viewPage.Value; } set { viewPage.Value = value; } }
    M4uProperty<int> pageCount = new M4uProperty<int>(0);
    public int PageCount { get { return pageCount.Value; } set { pageCount.Value = value; } }
    M4uProperty<string> pageText = new M4uProperty<string>("");
    public string PageText { get { return pageText.Value; } set { pageText.Value = value; } }

    // ページ数表示
    M4uProperty<bool> pageTextView = new M4uProperty<bool>(false);
    M4uProperty<bool> pageLeftView = new M4uProperty<bool>(false);
    M4uProperty<bool> pageRightView = new M4uProperty<bool>(false);
    public bool PageTextView { get { return pageTextView.Value; } set { pageTextView.Value = value; } }
    public bool PageLeftView { get { return pageLeftView.Value; } set { pageLeftView.Value = value; } }
    public bool PageRightView { get { return pageRightView.Value; } set { pageRightView.Value = value; } }

    //
    M4uProperty<string> emptyLabel = new M4uProperty<string>();
    public string EmptyLabel { get { return emptyLabel.Value; } set { emptyLabel.Value = value; } }

    public enum Category : int
    {
        None = -1,
        Active = 0,
        Furture = 1,
    }

    [SerializeField]
    private Toggle[] m_toggles;

    private Category m_Category = Category.Active;
    private GameObject m_Dialog = null;


    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    void Start()
    {
    }

    public void PostSceneStart()
    {
        AndroidBackKeyManager.Instance.StackPush(gameObject, OnClickedBackButton);
        // ナビゲーションバー
        ScheduleTab1 = GameTextUtil.GetText("schedule_tab1");
        ScheduleTab2 = GameTextUtil.GetText("schedule_tab2");

        EmptyLabel = GameTextUtil.GetText("common_not_list");

        IsViewContents = true;

        //---------------------------------------------
        // 開催中レコード
        //---------------------------------------------
        Records0.Clear();
        List<EventScheduleInfo> activeEventInfoList = GetEvetntItemList(Category.Active);
        activeEventInfoList.Sort(SortExec);
        foreach (EventScheduleInfo item in activeEventInfoList)
        {
            AddRecord(Category.Active, item);
        }
        RecordCount0 = Records0.Count;

        //---------------------------------------------
        // 開催予定レコード
        //---------------------------------------------
        Records1.Clear();
        List<EventScheduleInfo> furtureEventInfoList = GetEvetntItemList(Category.Furture);
        furtureEventInfoList.Sort(SortExec);
        foreach (EventScheduleInfo item in furtureEventInfoList)
        {
            AddRecord(Category.Furture, item);
        }
        RecordCount1 = Records1.Count;

        //---------------------------------------------
        // データがある場合は詳細ダイアログを表示する
        //---------------------------------------------
        if (MainMenuParam.m_DialogEventScheduleData != null)
        {
            EventRecordListItemContex record = Records0.Find((v) => v.FixId == MainMenuParam.m_DialogEventScheduleData.fix_id);
            if (record != null)
            {
                EventScheduleDialog eventDialog = CreateDetailView(record);
                eventDialog.IsViewJumpButton = false;
            }
            else
            {
#if BUILD_TYPE_DEBUG
                string messageText = "イベントがありませんでした。\nMasterDataEvent::fix_id:" + MainMenuParam.m_DialogEventScheduleData.fix_id;
                Dialog dloalog = DialogManager.Open1B_Direct("デバッグ ダイアログ", messageText, "common_button7", true, true)
                    .SetOkEvent(() =>
                    {
                    });
#endif
            }
            MainMenuParam.m_DialogEventScheduleData = null;
        }
    }

    /// <summary>
    /// 表示するイベント情報を取得する
    /// </summary>
    List<EventScheduleInfo> GetEvetntItemList(Category category)
    {
        List<EventScheduleInfo> eventItemList = new List<EventScheduleInfo>();

        MasterDataArea[] areaMasterArray = MasterFinder<MasterDataArea>.Instance.GetAll();
        for (int i = 0; i < areaMasterArray.Length; ++i)
        {
            MasterDataArea cAreaMasterData = areaMasterArray[i];

            // イベント情報取得
            MasterDataEvent cMasterDataEvent = null;
            if (category == Category.Active)
            {
                //開催中ページの場合、有効なイベント情報取得
                cMasterDataEvent = MasterDataUtil.GetMasterDataEventFromID(cAreaMasterData.event_id);
            }
            else
            {
                //開催予定ページの場合、開始日が未来日のイベント情報取得
                cMasterDataEvent = MasterDataUtil.GetMasterDataFurtureEventFromID(cAreaMasterData.event_id);
            }

            //--------------------------------
            //表示対象かをチェック
            //--------------------------------
            if (cMasterDataEvent == null) { continue; }

            uint unFixID = 0;
            uint unTimingStart = 0;
            uint unTimingEnd = 0;
            MasterDataDefineLabel.BoolType unScheduleShow = MasterDataDefineLabel.BoolType.NONE;

            //--------------------------------
            // 期間指定タイプによる分岐
            // @add Developer 2016/07/29 v360
            //--------------------------------
            switch (cMasterDataEvent.period_type)
            {
                // 指定(従来通り)
                default:
                case MasterDataDefineLabel.PeriodType.DESIGNATION:
                    unFixID = cMasterDataEvent.fix_id;
                    unTimingStart = cMasterDataEvent.timing_start;
                    unTimingEnd = cMasterDataEvent.timing_end;
                    unScheduleShow = cMasterDataEvent.event_schedule_show;
                    break;

                // サイクル
                case MasterDataDefineLabel.PeriodType.CYCLE:
                    if (TimeEventManager.Instance == null)
                    {
                        continue;
                    }

                    // エリアの表示期間のカウントダウンを算出
                    CycleParam cCycleParam;
                    if (category == Category.Active)
                    {
                        cCycleParam = TimeEventManager.Instance.GetEventCycleParam(cMasterDataEvent.event_id);
                    }
                    else
                    {
                        cCycleParam = TimeEventManager.Instance.GetEventCycleFurtureParam(cMasterDataEvent.event_id);
                    }
                    if (cCycleParam == null)
                    {
                        continue;
                    }

                    unFixID = cCycleParam.fixID;
                    unTimingStart = cCycleParam.timingStart;
                    unTimingEnd = cCycleParam.timingEnd;
                    unScheduleShow = cCycleParam.schedule;
                    break;
            }
            if (unScheduleShow != MasterDataDefineLabel.BoolType.ENABLE)
            {
                continue;
            }

            // 開催中ページの場合
            if (category == Category.Active)
            {
                //--------------------------------
                // イベント期間判定
                //--------------------------------
                bool bCheckWithinTime = TimeManager.Instance.CheckWithinTime(unTimingStart, unTimingEnd);
                if (bCheckWithinTime == false)
                {
                    continue;
                }
            }
            // 開催予定ページの場合開始時間が現在日の次の日までを表示する
            else if (category == Category.Furture)
            {
                // 時間を考慮しない開始日を取得
                int nYear = TimeUtil.GetDateTimeToYear(unTimingStart);
                int nMonth = TimeUtil.GetDateTimeToMonth(unTimingStart);
                int nDay = TimeUtil.GetDateTimeToDay(unTimingStart);
                DateTime cTimeStartDay = new DateTime(nYear, nMonth, nDay, 0, 0, 0);

                // 時間を考慮しない現在日を取得
                DateTime cTmpTimeNow = TimeManager.Instance.m_TimeNow;
                DateTime cTimeNowDay = new DateTime(cTmpTimeNow.Year, cTmpTimeNow.Month, cTmpTimeNow.Day, 0, 0, 0);
                // 現在日から開催日までのTimeSpanを取得
                TimeSpan timeSpan = cTimeStartDay - cTimeNowDay;
                // 開催日まで一日よりある場合は登録しない
                if (timeSpan.Days > 1)
                {
                    continue;
                }
            }

            EventScheduleInfo cEventScheduleInfo = new EventScheduleInfo();
            //--------------------------------
            // マスターの情報をそのまま使うと、
            // サイクルなどの処理で困るため、一部改変してスケジュールのマスタとする
            //--------------------------------
            cEventScheduleInfo.m_MasterDataEvent = new MasterDataEvent();
            cEventScheduleInfo.m_MasterDataEvent.fix_id = unFixID;
            cEventScheduleInfo.m_MasterDataEvent.active = cMasterDataEvent.active;
            cEventScheduleInfo.m_MasterDataEvent.period_type = cMasterDataEvent.period_type;
            cEventScheduleInfo.m_MasterDataEvent.cycle_date_type = cMasterDataEvent.cycle_date_type;
            cEventScheduleInfo.m_MasterDataEvent.cycle_timing_start = cMasterDataEvent.cycle_timing_start;
            cEventScheduleInfo.m_MasterDataEvent.cycle_active_hour = cMasterDataEvent.cycle_active_hour;
            cEventScheduleInfo.m_MasterDataEvent.timing_start = unTimingStart;
            cEventScheduleInfo.m_MasterDataEvent.timing_end = unTimingEnd;
            cEventScheduleInfo.m_MasterDataEvent.user_group = cMasterDataEvent.user_group;
            cEventScheduleInfo.m_MasterDataEvent.event_id = cMasterDataEvent.event_id;
            cEventScheduleInfo.m_MasterDataEvent.event_schedule_show = unScheduleShow;
            cEventScheduleInfo.m_MasterDataEvent.area_id = (int)cAreaMasterData.fix_id;
            cEventScheduleInfo.m_AreaName = cAreaMasterData.area_name;
            cEventScheduleInfo.m_AreaUrl = cAreaMasterData.area_url;

            //リストに追加
            eventItemList.Add(cEventScheduleInfo);
        }

        return eventItemList;
    }


    /// <summary>
    /// ソート処理：イベントリスト
    /// </summary>
    /// <param name="cEventA"></param>
    /// <param name="cEventB"></param>
    /// <returns></returns>
    static public int SortExec(EventScheduleInfo cEventA, EventScheduleInfo cEventB)
    {
        //開始日時で比較
        if (cEventA.m_MasterDataEvent.timing_start < cEventB.m_MasterDataEvent.timing_start)
        {
            return -1;
        }

        if (cEventA.m_MasterDataEvent.timing_start > cEventB.m_MasterDataEvent.timing_start)
        {
            return 1;
        }

        //終了日時で比較
        if (cEventA.m_MasterDataEvent.timing_end < cEventB.m_MasterDataEvent.timing_end)
        {
            return -1;
        }

        if (cEventA.m_MasterDataEvent.timing_end > cEventB.m_MasterDataEvent.timing_end)
        {
            return 1;
        }

        //期間が同じだったらエリアIDで比較
        if (cEventA.m_MasterDataEvent.area_id < cEventB.m_MasterDataEvent.area_id)
        {
            return -1;
        }

        if (cEventA.m_MasterDataEvent.area_id > cEventB.m_MasterDataEvent.area_id)
        {
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// レコードの追加
    /// </summary>
    private void AddRecord(Category category, EventScheduleInfo data)
    {
        var contex = new EventRecordListItemContex();

        contex.FixId = (int)data.m_MasterDataEvent.fix_id;
        contex.EventId = (int)data.m_MasterDataEvent.event_id;

        // アイコンイメージの設定

        //クエスト情報
        contex.questType = MasterDataDefineLabel.QuestType.NONE;
        //通常クエスト
        MasterDataQuest2 lastQuestMaster = MasterFinder<MasterDataQuest2>.Instance.SelectFirstWhere("where area_id = ? AND boss_chara_id > 0 AND story = 0 ORDER BY fix_id DESC"
                                                                                                , data.m_MasterDataEvent.area_id);
        if (lastQuestMaster != null)
        {
            contex.questType = MasterDataDefineLabel.QuestType.NORMAL;
        }
        else
        {
            //成長ボスクエスト
            lastQuestMaster = MasterFinder<MasterDataChallengeQuest>.Instance.SelectFirstWhere("where area_id = ? AND boss_chara_id > 0 AND story = 0 ORDER BY fix_id DESC"
                                                                                                            , data.m_MasterDataEvent.area_id);
            if (lastQuestMaster != null)
            {
                contex.questType = MasterDataDefineLabel.QuestType.CHALLENGE;

                //タイトル差し替え
                setupChallengeTitle(data);
            }
        }

        if (lastQuestMaster == null)
        {
            // ボスが居ない場合の画像
            contex.IconImage = ResourceManager.Instance.Load("divine1", ResourceType.Common);

#if BUILD_TYPE_DEBUG
            DialogManager.Open1B_Direct("EventSchedule AddRecord",
                                        "renew_quest_masterのarea_idに下記の\nidが含まれていません。\nプランナーさんに画面を見せるか\n画面キャプチャーして報告してください。\n\n" +
                                        "area_id: " + data.m_MasterDataEvent.area_id,
                                        "common_button7", true, true).
            SetOkEvent(() =>
            {
            });
#endif
            Debug.LogError("[none x.boss_chara_id > 0]: " + data.m_MasterDataEvent.area_id);
        }
        else
        {
            UnitIconImageProvider.Instance.Get(
            lastQuestMaster.boss_chara_id,
            sprite =>
            {
                contex.IconImage = sprite;
            });
        }

        // 予約済みアイコンの設定
        if (category == Category.Furture)
        {
            LocalSaveEventNotification cEventNotification = LocalSaveManager.Instance.CheckFuncNotificationRequest((uint)data.m_MasterDataEvent.fix_id);
            if (cEventNotification != null)
            {
                contex.IsReserveBadge = cEventNotification.m_Push;
            }
        }

        // タイトル（エリア名称）表示
        contex.CaptionText01 = data.m_AreaName;

        // 枠色変更
        contex.IconColor = this.IconColor[(int)category];
        contex.FrameColor = this.FrameColor[(int)category];

        // ボタン挙動
        contex.DidSelectItem += OnClickedRecordButton;

        // レコードの追加先の指定
        switch (category)
        {
            case Category.Active:
                // 日付表示
                contex.CaptionText02 = data.m_MasterDataEvent.timing_end != 0 ?
                    string.Format(GameTextUtil.GetText("schedule_held_text"), TimingFormat(data.m_MasterDataEvent.timing_end, true)) : "";
                Records0.Add(contex);
                break;
            case Category.Furture:
                contex.CaptionText02 = data.m_MasterDataEvent.timing_start != 0 ?
                    string.Format(GameTextUtil.GetText("schedule_plans_text"), TimingFormat(data.m_MasterDataEvent.timing_start)) : "";
                Records1.Add(contex);
                break;
            default:
                return;
        }
    }

    // 確認ダイアログボックス表示
    private EventScheduleDialog CreateDetailView(EventRecordListItemContex contex)
    {
        m_Dialog = Instantiate(DialogInfo) as GameObject;
        m_Dialog.transform.SetParent(DialogInfoRoot.transform, false);
        bool future = (m_Category == Category.Furture) ? true : false;
        EventScheduleDialog eventDialog = m_Dialog.GetComponent<EventScheduleDialog>();
        eventDialog.Create(contex, future);
        IsViewContents = false;

        return eventDialog;
    }

    private void closeDialog()
    {
        if (m_Dialog == null)
        {
            return;
        }

        //--------------------------------------
        // 予約アイコンの更新
        //--------------------------------------
        EventScheduleDialog dialog = m_Dialog.GetComponent<EventScheduleDialog>();
        if (dialog != null)
        {
            if (dialog.IsFuture)
            {
                EventRecordListItemContex item = Records1.Find((x) => x.FixId == dialog.FixId);
                if (item != null)
                {
                    item.IsReserveBadge = dialog.IsOnNotif;
                }
            }
        }

        DestroyObject(m_Dialog);
        m_Dialog = null;
        IsViewContents = true;
    }

    // 戻るボタンのフィードバック
    public void OnClickedBackButton()
    {
        if (m_Dialog != null)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
            closeDialog();
        }
        else
        {
            AndroidBackKeyManager.Instance.StackPop(gameObject);
            GlobalMenu.GetGlobalMainMenu().OnPushReturn();
        }
    }

    // ナビゲーションボタンのフィードバック
    public void OnClickedNaviButton(int _category)
    {
        if (!m_toggles[_category].isOn
            || m_Category == (Category)_category)
            return;

        m_Category = (Category)_category;
        closeDialog();

        SoundManager.Instance.PlaySE(SEID.SE_MM_A03_TAB);
    }

    // 詳細ボタンのフィードバック
    public void OnClickedRecordButton(EventRecordListItemContex context)
    {
        // 詳細画面の表示
        CreateDetailView(context);

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    // 時刻表示の書式
    public static string TimingFormat(uint timing, bool subtract = false)
    {
        DateTime _time = TimeUtil.GetDateTime(timing);
        if (subtract == true)
        {
            _time = _time.SubtractAMinute();
        }
        var result = timing != 0 ?
            _time.ToString("yyyy/MM/dd HH:mm") : "";

        return result;
    }

    // ページ切り替え
    public void UpdatePage(int inc = 0)
    {
        int recordsCapacity = 30;
        PageCount = (Records0.Count + recordsCapacity - 1) / recordsCapacity;

        // ページ切り替えが有効の場合
        int nextPage = ViewPage + inc;
        if (0 <= nextPage && nextPage < PageCount)
        {
            ViewPage = nextPage;

            PageTextView = 1 < PageCount ? true : false;
            PageLeftView = 0 < nextPage ? true : false;
            PageRightView = nextPage < PageCount - 1 ? true : false;
        }

        for (int i = 0; i < Records0.Count; i++)
        {
            if (ViewPage * recordsCapacity <= i &&
                i < (ViewPage + 1) * recordsCapacity)
            {
                // 表示する
                Records0[i].Active = true;
            }
            else
            {
                // 非表示に
                Records0[i].Active = false;
            }
        }

        PageText = string.Format("{0}/{1}", ViewPage + 1, PageCount);
    }

    private void setupChallengeTitle(EventScheduleInfo data)
    {
        //開催中のイベントからチェック
        List<MasterDataChallengeEvent> challengeEvents = MasterDataUtil.GetActiveChallengeEvent();
        if (challengeEvents != null &&
            challengeEvents.Count > 0)
        {
            MasterDataChallengeEvent challengeEvent = challengeEvents.Where(a => a.event_id == data.m_MasterDataEvent.event_id).FirstOrDefault();
            if (challengeEvent != null)
            {
                data.m_AreaName = challengeEvent.title;
                return;
            }
        }

        //ないときは開催前
        challengeEvents = MasterFinder<MasterDataChallengeEvent>.Instance.GetAll().Where(a => a.event_id == data.m_MasterDataEvent.event_id).ToList();
        if (challengeEvents != null &&
            challengeEvents.Count > 0)
        {
            challengeEvents.Sort((a, b) => (int)a.timing_start - (int)b.timing_start);
            for (int i = 0; i < challengeEvents.Count; i++)
            {
                if (data.m_MasterDataEvent.timing_start < challengeEvents[i].timing_start)
                {
                    data.m_AreaName = challengeEvents[i].title;
                    return;
                }
            }
        }

        return;
    }
}