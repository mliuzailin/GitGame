using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using M4u;
using ServerDataDefine;
using UnityEngine.UI;
using TMPro;

public class Present : MenuPartsBase
{
    public enum Category : int
    {
        Present = 0,
        PresentLog
    }

    // カラーパレット
    // "#E03F6CFF", "#FFBF00FF", "#82B312FF"
    public Color[] IconColor;
    // "#FFA6C1C0", "#F1E990C0", "#BDF291C0"
    public Color[] FrameColor;

    // ボタン
    M4uProperty<string> naviText0 = new M4uProperty<string>("");
    M4uProperty<string> naviText1 = new M4uProperty<string>("");
    public string NaviText0 { get { return naviText0.Value; } set { naviText0.Value = value; } }
    public string NaviText1 { get { return naviText1.Value; } set { naviText1.Value = value; } }

    // レコードの追加先
    M4uProperty<List<PresentRecordListItemContex>> records0 = new M4uProperty<List<PresentRecordListItemContex>>(new List<PresentRecordListItemContex>());
    M4uProperty<List<PresentRecordListItemContex>> records1 = new M4uProperty<List<PresentRecordListItemContex>>(new List<PresentRecordListItemContex>());

    public List<PresentRecordListItemContex> Records0 { get { return records0.Value; } set { records0.Value = value; } }
    public List<PresentRecordListItemContex> Records1 { get { return records1.Value; } set { records1.Value = value; } }


    // レコード数
    M4uProperty<int> recordCount0 = new M4uProperty<int>(0);
    M4uProperty<int> recordCount1 = new M4uProperty<int>(0);
    M4uProperty<int> recordCount2 = new M4uProperty<int>(0);
    public int RecordCount0 { get { return recordCount0.Value; } set { recordCount0.Value = value; } }
    public int RecordCount1 { get { return recordCount1.Value; } set { recordCount1.Value = value; } }
    public int RecordCount2 { get { return recordCount2.Value; } set { recordCount2.Value = value; } }

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

    M4uProperty<string> emptyLabel = new M4uProperty<string>();
    public string EmptyLabel { get { return emptyLabel.Value; } set { emptyLabel.Value = value; } }

    [SerializeField]
    private Toggle[] m_toggles;

    private Category m_Category;

    public TextMeshProUGUI receivedItemDisplayLimtText;

    private const string message_split_text = "**";

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        receivedItemDisplayLimtText.gameObject.SetActive(false);
    }

    void Start()
    {
    }

    // 戻るボタン
    public void OnClickedBackButton()
    {
    }

    // ナビゲーションボタンのフィードバック
    public void OnClickedNaviButton(int _category)
    {
        if (!m_toggles[_category].isOn
            || m_Category == (Category)_category)
            return;

        m_Category = (Category)_category;
        receivedItemDisplayLimtText.gameObject.SetActive(false);
        if (m_Category == Category.PresentLog)
        {

            receivedItemDisplayLimtText.gameObject.SetActive(true);
            receivedItemDisplayLimtText.text = GameTextUtil.GetText("RECEIVED_ITEM_DISPLAY_LIMIT");
        }
        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
    }

    // 「すべて受領」ボタンのフィードバック
    public void OnClickdeTakeAllButton()
    {
        if (ServerApi.IsExists == true)
        {
            return;
        }

        if (GlobalMenuManager.Instance.IsPageClosing() == true)
        {
            return;
        }

        if (GlobalMenuManager.Instance.IsCangeTime())
        {
            return;
        }

        long[] ids = Records0.Where(p => p.NoticeEnable == false).ToList().Select(p => p.FixId).ToArray();
        presentOpen(ids, true);
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    // レコードの受領ボタンのフィードバック
    public void OnClickedRecordButton(PresentRecordListItemContex contex)
    {
        if (ServerApi.IsExists == true)
        {
            return;
        }

        if (GlobalMenuManager.Instance.IsPageClosing() == true)
        {
            return;
        }

        if (GlobalMenuManager.Instance.IsCangeTime())
        {
            return;
        }

        long[] ids = { contex.FixId };
        if (contex.NoticeEnable == false)
        {
            presentOpen(ids, false);
        }
        else
        {
            Dialog newDialog = Dialog.Create(DialogType.DialogScrollInfo);
            newDialog.SetDialogText(DialogTextType.Title, contex.CaptionText01);
            newDialog.SetDialogText(DialogTextType.OKText, GameTextUtil.GetText("BTN_RECON"));
            newDialog.AddScrollInfoText(contex.NoticeText);
            newDialog.SetDialogEvent(DialogButtonEventType.OK, new Action(() =>
             {
                 if (contex.Category == 0)
                 {
                     presentOpen(ids, false, true);
                 }
             }));
            newDialog.Show();
        }
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    // レコードの移動
    private bool moveRecord(long present_id)
    {
        var contex = Records0.Find(p => p.FixId == present_id);
        if (contex == null)
            return false;

        contex.IconColor = IconColor[1];    // 色変更
        contex.FrameColor = FrameColor[1];
        contex.CaptionText02 = "";          // 残り時間非表示
        contex.Category = (int)Category.PresentLog;

        Records1.Insert(0, contex);
        if (Records1.Count > 30)
        {
            Records1.RemoveAt(Records1.Count - 1);
        }
        Records0.Remove(contex);
        return true;
    }

    // 確認ダイアログボックス表示
    private void openDialog(string title, string mainText, string okText, bool popupcheck = false)
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogText(DialogTextType.Title, title);
        newDialog.SetDialogText(DialogTextType.MainText, mainText);
        newDialog.SetDialogText(DialogTextType.OKText, okText);
        if (popupcheck == true)
        {
            newDialog.SetOkEvent(() =>
            {
                StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
            });
        }
        newDialog.Show();
    }

    // SceneStart後に実行
    public void initPresent()
    {
        // ナビゲーションバー
        NaviText0 = GameTextUtil.GetText("present_tab1");
        NaviText1 = GameTextUtil.GetText("present_tab2");

        EmptyLabel = GameTextUtil.GetText("common_not_list");

        ViewPage = 0;

        updatePresentList();

        UpdatePage(ViewPage);
    }

    /// <summary>
    /// プレゼントリストの更新
    /// </summary>
    /// <param name="isUpdateLog">受け取り済みのリストの更新も行う</param>
    public void updatePresentList(bool isUpdateLog = true)
    {
        // プレゼント（受領前）
        Records0.Clear();
        var present = UserDataAdmin.Instance.m_StructPresentList;
        if (present != null)
        {
            for (int i = 0; i < present.Length; i++)
            {
                // プレゼントリストの追加
                AddRecord(0, present[i]);
            }
            // レコード数の更新
            RecordCount0 = Records0.Count;
            RecordCount2 = Records0.Where(p => p.NoticeEnable == false).ToList().Count;
        }

        // プレゼント（受領済み）
        if (isUpdateLog)
        {
            StartCoroutine(WaitSentStart(() =>
            {
                var serverApi = ServerDataUtilSend.SendPacketAPI_GetPresentOpenLog();

                // SendStartの成功時の振る舞い
                serverApi.setSuccessAction(_data =>
                {
                    Records1.Clear();
                    RecvGetPresentOpenLogValue result = _data.GetResult<RecvGetPresentOpenLog>().result;
                    if (result == null || result.present == null)
                    {
                        return;
                    }
                    for (int i = 0; i < result.present.Length; i++)
                    {
                        // プレゼントリストの追加
                        AddRecord(1, result.present[i]);
                    }

                    // レコード数の更新
                    RecordCount1 = Records1.Count;
                });

                // SendStartの失敗時の振る舞い
                serverApi.setErrorAction(_date =>
                {
#if BUILD_TYPE_DEBUG
                    Debug.Log("MASTER_HASH_GET:Error");
#endif
                });

                serverApi.SendStart();
            }));
        }
    }

    /// <summary>
    /// 通信を待機する
    /// </summary>
    /// <param name="finishAction"></param>
    /// <returns></returns>
    IEnumerator WaitSentStart(Action finishAction)
    {
        while (MainMenuManager.Instance.patchUpdateRequestStep == EMAINMENU_PATCHUPDATE_REQ.WAIT)
        {
            yield return null;
        }

        if (finishAction != null)
        {
            finishAction();
        }
    }

    // レコードの追加
    public void AddRecord(int category, ServerDataDefine.PacketStructPresent data)
    {
        var contex = new PresentRecordListItemContex();

        // 共通
        contex.FixId = data.serial_id;
        contex.CaptionText00 = MainMenuUtil.GetPresentCount(data).ToString();
        contex.CaptionText01 = data.message;
        contex.NoticeText = "";
        contex.NoticeEnable = false;
        if ((MasterDataDefineLabel.PresentType)data.present_type == MasterDataDefineLabel.PresentType.NOTICE)
        {
            string[] notice = data.message.Split(new String[] { message_split_text }, StringSplitOptions.None);
            if (notice.Length >= 2)
            {
                contex.CaptionText00 = "1";
                contex.CaptionText01 = notice[0];
                contex.NoticeText = notice[1];
                contex.NoticeEnable = true;
            }
        }
        contex.Category = category;

        // 画像はカテゴリ別に差し替え可能にする
        image(data, sprite => { contex.IconImage = sprite; });

        // ボタンフィードバック
        contex.DidSelectItem += OnClickedRecordButton;

        // 枠色変更
        contex.IconColor = IconColor[contex.Category];
        contex.FrameColor = FrameColor[contex.Category];

        // レコードの追加先を指定
        switch ((Category)contex.Category)
        {
            case Category.Present:
                // アイテム受領期間までの残り時間
                if (data.delete_timing == 1)
                {
                    // 無期限
                    //contex.CaptionText02 = "ENDLESS";
                }
                else
                {
                    DateTime date = TimeUtil.ConvertServerTimeToLocalTime((ulong)data.delete_timing);
                    contex.CaptionText02 = contex.TimeLeftText(date);
                }
                contex.Caption01_H = 110;

                Records0.Add(contex);
                break;

            case Category.PresentLog:
                contex.CaptionText02 = "";
                contex.Caption01_H = 80;
                Records1.Add(contex);
                break;
        }
    }

    private uint getFixId(ServerDataDefine.PacketStructPresent data)
    {
        uint fix_id;
        // present_type で引き数の意味が異なる
        switch ((MasterDataDefineLabel.PresentType)data.present_type)
        {
            case MasterDataDefineLabel.PresentType.UNIT:
            case MasterDataDefineLabel.PresentType.SCRATCH:
            case MasterDataDefineLabel.PresentType.ITEM:
            case MasterDataDefineLabel.PresentType.QUEST_KEY:
                fix_id = data.present_value0;
                break;
            default:
                fix_id = 0;
                break;
        }
        return fix_id;
    }

    // アイコンのSpriteを取得
    private void image(PacketStructPresent presentData, System.Action<Sprite> callback)
    {
        MasterDataPresent presentMaster = new MasterDataPresent();
        presentMaster.present_type = (MasterDataDefineLabel.PresentType)presentData.present_type;
        presentMaster.present_param1 = (int)presentData.present_value0;
        presentMaster.present_param2 = (int)presentData.present_value1;
        presentMaster.present_param3 = (int)presentData.present_value2;
        presentMaster.present_param4 = (int)presentData.present_value3;
        presentMaster.present_param5 = (int)presentData.present_value4;
        presentMaster.present_param6 = (int)presentData.present_value5;
        presentMaster.present_param7 = (int)presentData.present_value6;
        presentMaster.present_param8 = (int)presentData.present_value7;
        presentMaster.present_param9 = (int)presentData.present_value8;
        presentMaster.present_param10 = (int)presentData.present_value9;

        MainMenuUtil.GetPresentIcon(presentMaster, sprite =>
        {
            callback(sprite);
        });
    }

    /// <summary>
    /// プレゼントを開く
    /// </summary>
    /// <param name="present_ids"></param>
    /// <param name="isAll">全部受け取りかどうか</param>
    private void presentOpen(long[] present_ids, bool isAll = false, bool isNoticeEnable = false)
    {
        var serverApi = ServerDataUtilSend.SendPacketAPI_PresentOpen(present_ids);

        // SendStartの成功時の振る舞い
        serverApi.setSuccessAction(_data =>
        {
            UserDataAdmin.Instance.m_StructPlayer =
                _data.UpdateStructPlayer<RecvPresentOpen>(UserDataAdmin.Instance.m_StructPlayer);
            UserDataAdmin.Instance.ConvertPartyAssing();

            // 開封結果を取得
            var presentOpen = _data.GetResult<RecvPresentOpen>().result;

            // プレゼントリストを更新
            UserDataAdmin.Instance.m_StructPresentList = UserDataAdmin.PresentListClipTimeLimit(presentOpen.present);

            // プレゼント開封個数
            var presentOpenArray = presentOpen.open_serial_id;

            for (int i = 0; i < presentOpenArray.Length; ++i)
            {
                // レコードcontextを、DoneRecordsへ移動
                moveRecord(presentOpenArray[i]);
            }

            updatePresentList(false);

            // レコード数の更新
            RecordCount0 = Records0.Count;
            RecordCount1 = Records1.Count;
            RecordCount2 = Records0.Where(p => p.NoticeEnable == false).ToList().Count;

            //----------------------------------------------------
            // 開封結果ダイアログの表示
            //----------------------------------------------------
            if (isAll)
            {
                PresentAllOpenResultMessage(presentOpen);
            }
            else
            {
                PresentOneOpenResultMessage(presentOpen, isNoticeEnable);
            }

            // ページ表示の更新
            //StartCoroutine(PageUpdate());
        });

        // SendStartの失敗時の振る舞い
        serverApi.setErrorAction(_data =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("MASTER_HASH_GET:Error");
#endif
        });

        serverApi.SendStart();
    }

    /// <summary>
    /// 単品受取したときのメーッセージ
    /// </summary>
    private void PresentOneOpenResultMessage(RecvPresentOpenValue result, bool isNoticeEnable)
    {
        if (result == null) { return; }

        string result_msg = "";
        bool check = false;

        //--------------------------------------------------------------
        // エラーテキストの取得
        // 1: 受け取りアイテム（ユニット）が上限に到達　2: クエストキー期限切れ　３：開封有効期限切れ
        //--------------------------------------------------------------
        if (result.error != null)
        {
            for (int i = 0; i < result.error.Length; ++i)
            {
                switch ((PRESENT_OPEN_ERROR_TYPE)result.error[i])
                {
                    case PRESENT_OPEN_ERROR_TYPE.COUNT_LIMIT:
                        result_msg += Environment.NewLine + GameTextUtil.GetText("mt41q_content1");
                        break;
                    case PRESENT_OPEN_ERROR_TYPE.QUEST_KEY_EXPIRED:
                        result_msg += Environment.NewLine + GameTextUtil.GetText("mt41q_content2");
                        break;
                    case PRESENT_OPEN_ERROR_TYPE.RECEIVE_EXPIRED:
                        result_msg += Environment.NewLine + GameTextUtil.GetText("mt41q_content3");
                        break;
                }
            }
        }

        int presentOpenCount = (result.open_serial_id != null) ? result.open_serial_id.Length : 0; // 開封個数
        if (presentOpenCount > 0 && isNoticeEnable == false)
        {
            // 先頭に受け取りメッセージを追加
            result_msg = GameTextUtil.GetText("mt41q_content0") + Environment.NewLine + result_msg;
            check = true;
        }
        else
        {
            // 先頭の改行を消す
            if (!result_msg.IsNullOrEmpty())
            {
                result_msg = result_msg.Remove(0, Environment.NewLine.Length);
            }
        }


        if (!result_msg.IsNullOrEmpty())
        {
            openDialog(GameTextUtil.GetText("mt40q_title"),
                    string.Format(GameTextUtil.GetText("mt41q_content"), result_msg),
                    GameTextUtil.GetText("common_button1"), check);
        }
    }

    /// <summary>
    /// 全部受け取りした時のメッセージ
    /// </summary>
    private void PresentAllOpenResultMessage(RecvPresentOpenValue result)
    {
        if (result == null) { return; }

        //--------------------------------------------------------------
        // 受取エラーの数を取得
        // 1: 受け取りアイテム（ユニット）が上限に到達　2: クエストキー期限切れ　３：開封有効期限切れ
        //--------------------------------------------------------------
        int error_num_1 = 0;
        int error_num_2 = 0;
        int error_num_3 = 0;
        if (result.error != null)
        {
            for (int i = 0; i < result.error.Length; ++i)
            {
                switch ((PRESENT_OPEN_ERROR_TYPE)result.error[i])
                {
                    case PRESENT_OPEN_ERROR_TYPE.COUNT_LIMIT:
                        ++error_num_1;
                        break;
                    case PRESENT_OPEN_ERROR_TYPE.QUEST_KEY_EXPIRED:
                        ++error_num_2;
                        break;
                    case PRESENT_OPEN_ERROR_TYPE.RECEIVE_EXPIRED:
                        ++error_num_3;
                        break;
                }
            }
        }

        //--------------------------------------
        // エラーテキストの取得
        //--------------------------------------
        string error_msg_1 = ""; // 所持上限テキスト
        string error_msg_2 = ""; // 有効期限切れテキスト
        string error_msg_3 = ""; // 受取期限超過テキスト
        if (error_num_1 > 0)
        {
            error_msg_1 = Environment.NewLine + string.Format(GameTextUtil.GetText("mt40q_content1"), error_num_1) + Environment.NewLine;
        }
        if (error_num_2 > 0)
        {
            error_msg_2 = Environment.NewLine + string.Format(GameTextUtil.GetText("mt40q_content2"), error_num_2) + Environment.NewLine;
        }
        if (error_num_3 > 0)
        {
            error_msg_3 = Environment.NewLine + string.Format(GameTextUtil.GetText("mt40q_content3"), error_num_3) + Environment.NewLine;
        }

        int presentOpenCount = (result.open_serial_id != null) ? result.open_serial_id.Length : 0; // 開封個数

        //--------------------------------------
        // ダイアログの表示
        //--------------------------------------
        if (presentOpenCount > 0)
        {
            // n個のプレゼントを受け取りました。
            openDialog(GameTextUtil.GetText("mt40q_title"),
                        string.Format(GameTextUtil.GetText("mt40q_content"), presentOpenCount, error_msg_1, error_msg_2, error_msg_3),
                        GameTextUtil.GetText("common_button1"), true);
        }
        else
        {
            // 全て受け取れませんでした。
            openDialog(GameTextUtil.GetText("mt40q_title"),
                        string.Format(GameTextUtil.GetText("mt40q_content0"), error_msg_1, error_msg_3),
                        GameTextUtil.GetText("common_button1"));
        }
    }

    // SceneStart後に実行
    public void PostSceneStart()
    {
        StartCoroutine(PageUpdate());
    }

    private IEnumerator PageUpdate()
    {
        RecordCount0 = Records0.Count;
        RecordCount1 = Records1.Count;
        RecordCount2 = Records0.Where(p => p.NoticeEnable == false).ToList().Count;

        yield return new WaitForSeconds(0.1f);

        ViewPage = 0;
        UpdatePage(0);
    }

    public void UpdatePage(int inc = 0)
    {
        for (int i = 0; i < Records0.Count; i++)
        {
            Records0[i].Active = true;
        }
    }
}