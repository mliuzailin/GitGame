using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class EventScheduleDialog : MenuPartsBase
{
    public int EventId { get; protected set; }
    public uint AreaId { get; protected set; }
    public uint AreaCateId { get; protected set; }
    public int FixId { get; protected set; }
    public MasterDataDefineLabel.QuestType QuestType { get; protected set; }

    // メインイメージ
    M4uProperty<Sprite> mainImage = new M4uProperty<Sprite>();
    public Sprite MainImage
    {
        get { return mainImage.Value; }
        set
        {
            mainImage.Value = value;
            IsActiveMainImage = (value != null);
        }
    }

    M4uProperty<bool> isActiveMainImage = new M4uProperty<bool>();
    public bool IsActiveMainImage { get { return isActiveMainImage.Value; } set { isActiveMainImage.Value = value; } }

    // テキスト
    M4uProperty<string> dateText = new M4uProperty<string>("");
    M4uProperty<string> title0 = new M4uProperty<string>("");
    M4uProperty<string> title1 = new M4uProperty<string>("");
    M4uProperty<string> title2 = new M4uProperty<string>("");
    M4uProperty<string> overviewText = new M4uProperty<string>("");
    M4uProperty<string> button = new M4uProperty<string>();
    public string DateText { get { return dateText.Value; } set { dateText.Value = value; } }
    public string Title0 { get { return title0.Value; } set { title0.Value = value; } }
    public string Title1 { get { return title1.Value; } set { title1.Value = value; } }
    public string Title2 { get { return title2.Value; } set { title2.Value = value; } }
    public string OverviewText { get { return overviewText.Value; } set { overviewText.Value = value; } }
    public string Button { get { return button.Value; } set { button.Value = value; } }

    private string notificationTitle { get; set; }
    private string notificationBody { get; set; }
    private int notificationdelay { get; set; }

    M4uProperty<bool> isFuture = new M4uProperty<bool>();
    public bool IsFuture { get { return isFuture.Value; } set { isFuture.Value = value; } }

    M4uProperty<bool> isOnNotif = new M4uProperty<bool>(false);
    /// <summary>予約がONになっているかどうか</summary>
    public bool IsOnNotif { get { return isOnNotif.Value; } set { isOnNotif.Value = value; } }

    M4uProperty<bool> isViewJumpButton = new M4uProperty<bool>(true);
    /// <summary>イベント参加ボタンの表示・非表示</summary>
    public bool IsViewJumpButton { get { return isViewJumpButton.Value; } set { isViewJumpButton.Value = value; } }

    // 出現ユニットイメージ
    M4uProperty<List<CircleButtonListItemContex>> icons = new M4uProperty<List<CircleButtonListItemContex>>(new List<CircleButtonListItemContex>());
    public List<CircleButtonListItemContex> Icons { get { return icons.Value; } set { icons.Value = value; } }

    private int m_LastUpdateCount = 0;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        Button = GameTextUtil.GetText("schedule_button");
    }

    void Start()
    {
    }

    void LateUpdate()
    {
        if (m_LastUpdateCount != 0)
        {
            m_LastUpdateCount--;
            if (m_LastUpdateCount < 0)
            {
                m_LastUpdateCount = 0;
            }
            updateLayout();
        }
    }

    // ダイアログの初期化
    public void Create(EventRecordListItemContex contex, bool _future)
    {
        EventId = contex.EventId;
        FixId = contex.FixId;
        IsFuture = _future;
        QuestType = contex.questType;

        if (IsFuture)
        {
            LocalSaveEventNotification cEventNotification = LocalSaveManager.Instance.CheckFuncNotificationRequest((uint)FixId);
            if (cEventNotification != null)
            {
                IsOnNotif = cEventNotification.m_Push;
            }
        }

        MasterDataEvent eventMaster = MasterFinder<MasterDataEvent>.Instance.Find(FixId);
        if (eventMaster == null)
        {
#if BUILD_TYPE_DEBUG
            Debug.LogError("Eventがみつかりません fix_id: " + FixId);
#endif
            return;
        }

        // テキスト
        Title0 = "開催期間";
        Title1 = eventMaster.head_titel_1;
        Title2 = eventMaster.head_titel_2;
        OverviewText = eventMaster.head_text;

        //サイクルタイプで終了が設定されていない場合、規定文字列を表示
        if (eventMaster.period_type == MasterDataDefineLabel.PeriodType.CYCLE &&
            eventMaster.timing_end == 0)
        {
            DateText = GameTextUtil.GetText("schedule_text");
        }
        else
        {
            DateText = string.Format("{0}～{1}",
                EventSchedule.TimingFormat(eventMaster.timing_start),
                ((eventMaster.timing_end != 0 ? EventSchedule.TimingFormat(eventMaster.timing_end, true) : "")));   // イベント開催~終了まで
        }

        // メインイメージを読み込み
        MainImage = null;
        string bannerUrl = eventMaster.banner_img_url;
        if (bannerUrl.IsNullOrEmpty() == false)
        {
            bannerUrl = GlobalDefine.GetEventScheduleBannerUrl() + bannerUrl;

            WebResource.Instance.GetSprite(bannerUrl,
                        (Sprite sprite) =>
                        {
                            MainImage = sprite;
                        },
                        () => { });
        }

        // 通知情報
        var notification = MasterDataUtil.GetMasterDataNotification(eventMaster.fix_id,
                                                                    ServerDataDefine.NOTIFICATION_TYPE.EVENT);
        if (notification == null)
        {
            notificationTitle = null;
            notificationBody = null;
            notificationdelay = 0;
        }
        else
        {
            notificationTitle = notification.notification_title;
            notificationBody = notification.notification_body;
            var delay = TimeUtil.GetDateTime(notification.timing_start) - TimeManager.Instance.m_TimeNow;
            notificationdelay = delay.Milliseconds;
        }

        // デバッグ用
        //OverviewText += "\n";

        // エリア情報
        AreaId = (uint)eventMaster.area_id;
        var areaMaster = MasterFinder<MasterDataArea>.Instance.FindAll();
        var areaCateId = areaMaster.Where(x => x.fix_id == AreaId).Select(x => x.area_cate_id).ToList();
        AreaCateId = areaCateId.Count() != 0 ? areaCateId[0] : 0;

        List<uint> bossUnits = new List<uint>();
        switch (QuestType)
        {
            case MasterDataDefineLabel.QuestType.NORMAL:
                {
                    var questMaster = MasterFinder<MasterDataQuest2>.Instance.FindAll();
                    bossUnits = questMaster.Where(x => x.area_id == AreaId).Select(x => x.boss_chara_id).ToList();
                }
                break;
            case MasterDataDefineLabel.QuestType.CHALLENGE:
                {
                    var challengeMaster = MasterFinder<MasterDataChallengeQuest>.Instance.FindAll();
                    bossUnits = challengeMaster.Where(x => x.area_id == AreaId).Select(x => x.boss_chara_id).ToList();
                }
                break;
            default:
                break;
        }

        // ゲリラボスユニット(複数)
        var guerrillaUnits = new List<uint>();
        var area_id_list = new int[] { (int)AreaId };
        var serverApi = ServerDataUtilSend.SendPacketAPI_GetGuerrillaBossInfo(area_id_list);

        // ゲリラボスユニット取得成功時の振る舞い
        serverApi.setSuccessAction(_data =>
        {
            var guerrilla = _data.GetResult<ServerDataDefine.RecvGetGuerrillaBossInfo>().result.guerrilla_boss_list;
            if (guerrilla != null)
            {
                for (int i = 0; i < guerrilla.Length; i++)
                {
                    for (int j = 0; j < guerrilla[i].boss_id_list.Length; j++)
                    {
                        guerrillaUnits.Add((uint)guerrilla[i].boss_id_list[j]);
                    }
                }
            }

            // 出現ユニット
            List<uint> units = new List<uint>();

            // ボス・ゲリラボスIDの結合
            units.AddRange(bossUnits);
            units.AddRange(guerrillaUnits);

            // ソート
            units.Sort();

            // 重複を削除
            List<uint> unique = units.Distinct().ToList();

            // バトルが無いクエストはユニットが出現しないので削除（設定：UnitID=0）
            unique.RemoveAll(x => x == 0);

            // 該当ユニットの追加
            for (int i = 0; i < unique.Count; i++)
            {
                var icon = new CircleButtonListItemContex(i, unique[i]);
                icon.IsEnableSelect = false;
                Icons.Add(icon);

                // デバッグ用
                //OverviewText += i + ":" + "出現ユニットID[" + unique[i] + "]" + "\n";

                // レイアウト再構成
                m_LastUpdateCount = 5;
            }
        });

        // SendStartの失敗時の振る舞い
        serverApi.setErrorAction(_date =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("MASTER_HASH_GET:Error");
#endif
        });

        // ゲリラボス取得API
        serverApi.SendStart();
    }

    // 「戻るボタン」クリック時のフィードバック
    public void OnClickButton()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("OnClick:");
#endif

        Destroy(gameObject);
    }

    // 「予約ボタン」クリック時のフィードバック
    public void OnClickNotificationButton()
    {
        if (GlobalMenuManager.Instance.IsPageClosing() == true)
        {
            return;
        }

#if BUILD_TYPE_DEBUG
        Debug.Log("OnClick Notification:");
#endif
        IsOnNotif = !IsOnNotif;

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        LocalSaveManager.Instance.SaveFuncNotificationRequest((uint)FixId, IsOnNotif);
    }

    // 「イベント参加ボタン」クリック時のフィードバック
    public void OnClickdAreaButton()
    {
        if (GlobalMenuManager.Instance.IsPageClosing() == true)
        {
            return;
        }

#if BUILD_TYPE_DEBUG
        Debug.Log("AreaCateId:" + AreaCateId + " AreaId:" + AreaId);
#endif
        switch (QuestType)
        {
            case MasterDataDefineLabel.QuestType.NORMAL:
                {
                    // エリアカテゴリIDの登録
                    MainMenuParam.SetQuestSelectParam(AreaCateId, AreaId);

                    // エリア移動
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT, false, false, true);

                }
                break;
            case MasterDataDefineLabel.QuestType.CHALLENGE:
                {
                    MainMenuParam.SetChallengeSelectParamFromEventID((uint)EventId);
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_CHALLENGE_SELECT, false, false, true);
                }
                break;
        }


        // メニュー閉じる
        MainMenuManager.Instance.CloseGlobalMenu();

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }
}
