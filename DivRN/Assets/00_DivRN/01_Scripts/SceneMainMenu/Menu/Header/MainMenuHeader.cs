using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;

public class MainMenuHeader : View
{
    [SerializeField]
    private GameObject m_menuButtonRoot;

    /*==========================================================================*/
    /*      define                                                              */
    /*==========================================================================*/

    //----------------------------------------------------------------------------
    /*!
        @brief タイトル部分のユニット表示タイプ
    */
    //----------------------------------------------------------------------------
    public enum CAPTION_CUTIN_TYPE
    {
        CUTIN_NONE,         //!< 表示なし
        CUTIN_LEADER,       //!< 現在のアクティブパーティリーダー
        CUTIN_SELECT,       //!< 指定したユニット

        CUTIN_MAX,          //!< 最大値
    }


    /// <summary>
    /// 情報ウインドウ
    /// </summary>
    M4uProperty<bool> isViewInfoWindow = new M4uProperty<bool>();
    public bool IsViewInfoWindow { get { return isViewInfoWindow.Value; } set { isViewInfoWindow.Value = value; } }

    //M4uProperty<string> nextRankLabel = new M4uProperty<string>();
    //public string NextRankLabel { get { return nextRankLabel.Value; } set { nextRankLabel.Value = value; } }

    M4uProperty<string> nextRankExp = new M4uProperty<string>();
    public string NextRankExp { get { return nextRankExp.Value; } set { nextRankExp.Value = value; } }

    M4uProperty<string> staminaTimeValue = new M4uProperty<string>();
    public string StaminaTimeValue { get { return staminaTimeValue.Value; } set { staminaTimeValue.Value = value; } }

    M4uProperty<string> staminaValue = new M4uProperty<string>();
    public string StaminaValue { get { return staminaValue.Value; } set { staminaValue.Value = value; } }

    M4uProperty<string> statusdValue = new M4uProperty<string>();
    public string StatusdValue { get { return statusdValue.Value; } set { statusdValue.Value = value; } }

    /// <summary>
    /// アイテム効果ウインドウ
    /// </summary>
    M4uProperty<bool> isViewEffectWindow = new M4uProperty<bool>();
    public bool IsViewEffectWindow { get { return isViewEffectWindow.Value; } set { isViewEffectWindow.Value = value; } }

    M4uProperty<List<ItemEffectContext>> itemEffectList = new M4uProperty<List<ItemEffectContext>>(new List<ItemEffectContext>());
    public List<ItemEffectContext> ItemEffectList { get { return itemEffectList.Value; } set { itemEffectList.Value = value; } }

    M4uProperty<bool> isActiveNoItem = new M4uProperty<bool>();
    public bool IsActiveNoItem { get { return isActiveNoItem.Value; } set { isActiveNoItem.Value = value; } }

    /// <summary>
    /// メニューボタンフラグ
    /// </summary>
    M4uProperty<bool> isActiveMenuFlag = new M4uProperty<bool>();
    public bool IsActiveMenuFlag { get { return isActiveMenuFlag.Value; } set { isActiveMenuFlag.Value = value; } }

    /// <summary>
    /// テロップ
    /// </summary>
    M4uProperty<string> messageValue = new M4uProperty<string>();
    public string MessageValue { get { return messageValue.Value; } set { messageValue.Value = value; } }

    // MessagePosXは毎フレームアクセスするのでちょっとでも軽くなるようにm4u経由せずに直接変更する
    public float MessagePosX
    {
        get { return m_MessageValueObjectRectTransform.anchoredPosition.x; }
        set
        {
            var position = m_MessageValueObjectRectTransform.anchoredPosition;
            m_MessageValueObjectRectTransform.anchoredPosition = new Vector2
                (
                    value,
                    position.y
                );
        }
    }

    M4uProperty<Color> messageColor = new M4uProperty<Color>(Color.white);
    public Color MessageColor { get { return messageColor.Value; } set { messageColor.Value = value; } }

    M4uProperty<Color> messageBGColor = new M4uProperty<Color>(Color.clear);
    public Color MessageBGColor { get { return messageBGColor.Value; } set { messageBGColor.Value = value; } }

    //----------------------------------------------------------------------------
    /*!
        @brief
    */
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    /*!
        @brief  メッセージバー関連
    */
    //----------------------------------------------------------------------------

    /// <summary>
    /// 通常表示以外メッセージ表示クラス
    /// </summary>
    class HeaderMessage
    {
        public enum MESSAGE_TYPE
        {
            NONE = -1,
            NORMAL,
            ACHIEVEMENT_CLEAR,
            ACHIEVEMENT_NEW,
            MAX,
        }

        public uint fix_id;
        public string message;       //!< メッセージ
        public Color bg_color;       //!< 背景の色
        public Color message_color;  //!< 文字の色
        public MESSAGE_TYPE type;    //!< メッセージのタイプ
        public bool deleted = false; //!< メッセージを表示し終わったら削除する
        public int priority = 0;     //!< ソートの並び順

        static public int SortCompare(HeaderMessage a, HeaderMessage b)
        {
            // タイプで比較
            if ((int)a.type < (int)b.type)
            {
                return -1;
            }
            else if ((int)a.type > (int)b.type)
            {
                return 1;
            }

            // プライオリティで比較
            if (a.priority < b.priority)
            {
                return -1;
            }
            else if (a.priority > b.priority)
            {
                return 1;
            }

            return 0;
        }
    }

    static private int m_MsgBarRequestInputIndex = 0;       //!< メッセージバー：リクエスト入力番号
    static private bool m_MsgBarResetText = false;
    static private string m_MsgBarRequestInputMsg = "";      //!< メッセージバー：リクエスト入力文字列
    static private string m_PreMsgBarRequestInputMsg = "";      //!< メッセージバー：リクエスト入力文字列(一つ前)
    static private List<HeaderMessage> m_MsgBarOtherMessageList = null;    //!< 通常表示以外メッセージリスト
    static private bool m_IsViewMsgBarOtherMessage = false;
    static private HeaderMessage m_CurrentMessageValue = null;

    private float m_WorkMessageScrollTime = 0.0f;    //!< メッセージバー動作：
    private string m_WorkMessage = "";      //!< メッセージバー動作：

    private GameObject m_MessageValueObject = null;
    private RectTransform m_MessageValueObjectRectTransform = null;

    private GameObject statusObj = null;    //< ステータス関連オブジェクト

    private GlobalMenu globalMenu = null;

    private bool m_ActiveHeaderTouch = false;

    static public void ParamReset()
    {
        m_MsgBarRequestInputIndex = 0;      // メッセージバー：リクエスト入力番号
        m_MsgBarResetText = false;
        m_MsgBarRequestInputMsg = "";       // メッセージバー：リクエスト入力文字列
        m_PreMsgBarRequestInputMsg = "";    // メッセージバー：リクエスト入力文字列(一つ前)
        m_MsgBarOtherMessageList = null;    // 通常表示以外メッセージリスト
        m_IsViewMsgBarOtherMessage = false;
        m_CurrentMessageValue = null;
    }


    /// <summary>
    /// 起動処理
    /// </summary>
    void Awake()
    {
        {
            if (!UserDataAdmin.HasInstance)
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("UserDataAdmin.Instance is null");
#endif
                return;
            }
            statusObj = UnityUtil.GetChildNode(gameObject, "Status");
            statusObj.GetComponent<M4uContextRoot>().Context = UserDataAdmin.Instance;

            m_MessageValueObject = UnityUtil.GetChildNode(gameObject, "MessageValue");
            Debug.Assert(m_MessageValueObject != null, "MessageValue not found.");
            m_MessageValueObjectRectTransform = m_MessageValueObject.GetComponent<RectTransform>();
            Debug.Assert(m_MessageValueObjectRectTransform != null, "MessageValue has not RectTransform.");
            MainMenuHeader.UnderMsgRequest("");

            GetComponent<M4uContextRoot>().Context = this;

        }

        SetUpBttons();
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
        IsViewInfoWindow = false;
        IsViewEffectWindow = false;

    }


    // ---------------------------------------- setting up

    private ButtonModel m_menuButtonModel = null;
    private void SetUpBttons()
    {
        m_menuButtonModel = new ButtonModel();
        m_menuButtonModel.OnClicked += () =>
        {

            OnOpenGlobalMenu();
        };
        MainMenuHeaderMenuButton.
                            Attach(m_menuButtonRoot).
                            SetModel(m_menuButtonModel);
    }


    //----------------------------------------------------------------------------
    /*!
		@brief			ヘッダーへの操作有効無効設定
		@param[in]		bool		(enable)		有効/無効
	*/
    //----------------------------------------------------------------------------
    public void SetHeaderActive(bool enable)
    {
        //メニューボタン ON/OFF
        if (m_menuButtonModel != null
            && m_menuButtonModel.isReady) m_menuButtonModel.isEnabled = enable;

        //メニューが開いてたら　非表示/表示
        if (globalMenu != null)
        {
            if (globalMenu.isShowed)
            {
                UnityUtil.SetObjectEnabledOnce(globalMenu.gameObject, enable);
            }
            else
            {
                // アニメーション途中なら開いてないとみなして削除
                Destroy(globalMenu);
                globalMenu = null;
            }
        }

        //タッチ判定 ON/OFF
        SetTouchActive(enable);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief			ヘッダーへのタッチ操作の有効無効設定
		@param[in]		bool		(enable)		有効/無効
	*/
    //----------------------------------------------------------------------------
    public void SetTouchActive(bool enable)
    {
        //タッチ判定 ON/OFF
        if (m_ActiveHeaderTouch != enable)
        {
            m_ActiveHeaderTouch = enable;

            //無効時にウインドウが表示されていたら消す
            if (m_ActiveHeaderTouch == false)
            {
                IsViewEffectWindow = false;
                IsViewInfoWindow = false;
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	シーンの遷移が行えるかチェックする
	*/
    //----------------------------------------------------------------------------
    private bool CheckSwitchRequest()
    {
        // フッターメニューが操作できない場合はアウトにしておく
        //----------------------------------------
        // 管理クラスの状態チェック(ページ遷移中などはアウト)
        //----------------------------------------
        if (MainMenuManager.HasInstance == false
        || MainMenuManager.Instance.CheckMenuControlNG() == true
        ) return false;
        //----------------------------------------
        // 通信中はアウト
        //----------------------------------------
        if (ServerDataManager.HasInstance == true
        && ServerDataManager.Instance.ChkCommunicateFinishAll() == false
        ) return false;

        //----------------------------------------
        // 常駐ダイアログ表示中はアウト
        //----------------------------------------
        if (Dialog.HasDialog() == true)
            return false;

        return true;
    }

    /// <summary>
    /// グローバルメニュー開く
    /// </summary>
	public void OnOpenGlobalMenu(GLOBALMENU_SEQ _seq = GLOBALMENU_SEQ.TOP_MENU)
    {
        if (!CheckSwitchRequest())
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        var tag = "HeaderOpenGlobalMenu";
        ButtonBlocker.Instance.Block(tag);

        if (globalMenu == null)
        {
            globalMenu = GlobalMenu.Create(GLOBALMENU_TYPE.MAIN_MENU, SceneObjReferMainMenu.Instance.m_MainMenuGroupCamera.GetComponent<Camera>());
            if (_seq == GLOBALMENU_SEQ.TOP_MENU)
            {
                globalMenu.Show(() =>
                {
                    ButtonBlocker.Instance.Unblock(tag);
                });
            }
            else
            {
                globalMenu.ShowPage(_seq, () =>
                {
                    ButtonBlocker.Instance.Unblock(tag);
                });
            }

            if (MainMenuManager.HasInstance)
            {
                MainMenuManager.Instance.SetMenuFooterActive(false);
            }
        }
        else
        {
            globalMenu.Hide(() =>
            {
                ButtonBlocker.Instance.Unblock(tag);
            });
            globalMenu = null;
            if (MainMenuManager.HasInstance)
            {
                MainMenuManager.Instance.SetMenuFooterActive(true);
            }
        }
    }

    /// <summary>
    /// ページを指定してグローバルメニューを開く
    /// </summary>
    /// <param name="type"></param>
    public void OpenGlobalMenu(GLOBALMENU_SEQ seq)
    {
        if (m_menuButtonModel.isEnabled == false)
        {
            return;
        }

        OnOpenGlobalMenu(seq);
    }

    /// <summary>
    /// グローバルメニューが開ききるまで待機
    /// </summary>
    /// <param name="finishAction"></param>
    /// <returns></returns>
    IEnumerator WiatFinishOpenGlobalMenu(Action finishAction)
    {
        while (globalMenu != null &&
            globalMenu.isShowed == false)
        {
            yield return null;
        }
        while (globalMenu != null && globalMenu.CheckFinishButtonAnim() == false)
        {
            yield return null;
        }


        if (finishAction != null)
        {
            finishAction();
        }
    }

    /// <summary>
    /// グローバルメニュー閉じる
    /// </summary>
	public void CloseGlobalMenu()
    {
        if (globalMenu == null)
        {
            return;
        }
        globalMenu.Hide();
        globalMenu = null;
        if (MainMenuManager.HasInstance) MainMenuManager.Instance.SetMenuFooterActive(true);
    }

    public bool HasGlobalMenu()
    {
        return globalMenu != null;
    }

    public void OnInfoPointerEnter()
    {
        if (m_ActiveHeaderTouch == false)
        {
            return;
        }
        IsViewInfoWindow = true;
        settingInfoWindow();
    }

    public void OnInfoPointerExt()
    {
        IsViewInfoWindow = false;
    }

    public void OnEffectPointerEnter()
    {
        if (m_ActiveHeaderTouch == false)
        {
            return;
        }
        if (ItemEffectList.Count == 0)
        {
            return;
        }
        IsViewEffectWindow = true;
    }

    public void OnEffectPointerExt()
    {
        IsViewEffectWindow = false;
    }

    /// <summary>
    /// 情報ウインドウ設定
    /// </summary>
    private void settingInfoWindow()
    {
        PacketStructPlayer _player = UserDataAdmin.Instance.m_StructPlayer;
        //次ランクまで経験値計算
        MasterDataUserRank nextRank = MasterFinder<MasterDataUserRank>.Instance.Find((int)_player.rank + 1);
        if (nextRank != null)
        {
            //NextRankLabel = "次のランクまであと";
            int nextExp = (int)nextRank.exp_next_total - (int)_player.exp;
            NextRankExp = string.Format(GameTextUtil.GetText("head_subtext3"), nextExp);
        }
        else
        {
            //NextRankLabel = "ランクは最大です";
            NextRankExp = GameTextUtil.GetText("rankmax_caution");
        }

        updateStaminaTime();

#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
        StatusdValue = GlobalDefine.GetApplicationStatus(false);
#endif
    }

    /// <summary>
    /// スタミナ回復時間更新
    /// </summary>
    private void updateStaminaTime()
    {
        PacketStructPlayer _player = UserDataAdmin.Instance.m_StructPlayer;

        int leftStamina = (int)_player.stamina_max - (int)_player.stamina_now;
        if (leftStamina > 0)
        {
            DateTime cTimeStaminaRecovery = TimeUtil.ConvertServerTimeToLocalTime(UserDataAdmin.Instance.m_StructPlayer.stamina_recover);
            DateTime cTimeStaminaRecoveryNext = cTimeStaminaRecovery.AddSeconds(GlobalDefine.STAMINA_RECOVERY_SEC * leftStamina);
            DateTime cTimeNow = TimeManager.Instance.m_TimeNow;
            TimeSpan RecoverySpan = cTimeStaminaRecoveryNext - cTimeNow;
            int leftHour = RecoverySpan.Hours + 24 * RecoverySpan.Days;
            string leftTime = string.Format("{0:00}:{1:00}:{2:00}", leftHour, RecoverySpan.Minutes, RecoverySpan.Seconds);
            StaminaTimeValue = string.Format(GameTextUtil.GetText("head_subtext2"), leftTime);
        }
        else
        {
            StaminaTimeValue = "";
        }
        StaminaValue = string.Format(GameTextUtil.GetText("head_subtext1"), _player.stamina_now, _player.stamina_max);
    }

    /// <summary>
    /// アイテム効果更新
    /// </summary>
    private void updateItemEffect()
    {
        //プレイヤーのデータが更新されたらアイテム効果も更新する
        if (!UserDataAdmin.Instance.Player.isUpdateItem)
        {
            return;
        }

        UserDataAdmin.Instance.Player.isUpdateItem = false;

        PacketStructUseItem[] item_list = UserDataAdmin.Instance.m_StructPlayer.item_list;

        ItemEffectList.Clear();

        if (item_list != null)
        {
            for (int i = 0; i < item_list.Length; i++)
            {
                MasterDataUseItem itemMaster = MasterFinder<MasterDataUseItem>.Instance.Find((int)item_list[i].item_id);
                //マスターない
                if (itemMaster == null)
                {
                    continue;
                }

                //効果時間なし
                if (itemMaster.effect_span_m == 0)
                {
                    continue;
                }

                //効果時間内？
                if (!MainMenuUtil.IsItemEffectValid(itemMaster))
                {
                    continue;
                }

                ItemEffectContext newEffect = new ItemEffectContext();
                newEffect.ItemMaster = itemMaster;
                newEffect.IsViewIcon = true;

                if (itemMaster.coin_amend != 0) newEffect.IconImage = ResourceManager.Instance.Load("coin_b_2");
                if (itemMaster.exp_amend != 0) newEffect.IconImage = ResourceManager.Instance.Load("exp_b_2");
                if (itemMaster.fp_amend != 0) newEffect.IconImage = ResourceManager.Instance.Load("friend_b_2");
                if (itemMaster.link_amend != 0) newEffect.IconImage = ResourceManager.Instance.Load("charm_b_2");
                if (itemMaster.tk_amend != 0) newEffect.IconImage = ResourceManager.Instance.Load("ticket_b_2");

                newEffect.Time = MainMenuUtil.GetItemEffectLeftTimeString(itemMaster);

                ItemEffectList.Add(newEffect);
            }
        }
        //初心者ブーストがあるときは表示（アイテム効果が４つ未満のとき）
        if (MainMenuParam.m_BeginnerBoost != null)
        {
            ItemEffectContext newEffect = new ItemEffectContext();
            newEffect.ItemMaster = null;
            newEffect.IconImage = ResourceManager.Instance.Load("beginner_b");
            newEffect.IsViewIcon = true;
            if (ItemEffectList.Count > 3) newEffect.IsViewIcon = false;
            newEffect.Time = "";
            ItemEffectList.Add(newEffect);
        }
        IsActiveNoItem = (ItemEffectList.Count == 0) ? true : false;
    }

    /// <summary>
    /// アイテム効果時間更新
    /// </summary>
    private void updateItemEffectTime()
    {
        if (ItemEffectList.Count == 0)
        {
            return;
        }

        //
        bool bDelData = false;
        List<ItemEffectContext> tmpList = new List<ItemEffectContext>();
        for (int i = 0; i < ItemEffectList.Count; i++)
        {
            if (ItemEffectList[i].ItemMaster == null)
            {
                //マスターがないのは初心者ブースト表示
                tmpList.Add(ItemEffectList[i]);
                continue;
            }
            //効果時間内？
            if (MainMenuUtil.IsItemEffectValid(ItemEffectList[i].ItemMaster))
            {
                //残り時間更新
                ItemEffectList[i].Time = MainMenuUtil.GetItemEffectLeftTimeString(ItemEffectList[i].ItemMaster);
                //仮リスト追加
                tmpList.Add(ItemEffectList[i]);
            }
            else
            {
                //削除データあり
                bDelData = true;
            }
        }
        if (bDelData)
        {
            ItemEffectList.Clear();
            for (int i = 0; i < tmpList.Count; i++)
            {
                if (tmpList[i].ItemMaster == null && tmpList.Count < 4)
                {
                    tmpList[i].IsViewIcon = true;
                }
                ItemEffectList.Add(tmpList[i]);
            }
            IsActiveNoItem = (ItemEffectList.Count == 0) ? true : false;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	メッセージ更新リクエスト
		@note	nullが入ってきた場合表示をしないようにする
	*/
    //----------------------------------------------------------------------------
    static public void UnderMsgRequest(string strMsg, bool backKeep = false)
    {
        bool isReset = true;

        if (strMsg == "")
        {
            if (m_PreMsgBarRequestInputMsg != "")
            {
                strMsg = m_PreMsgBarRequestInputMsg;
                m_PreMsgBarRequestInputMsg = "";
            }
        }
        else if (m_CurrentMessageValue != null)
        {
            isReset = false;
        }

        if (backKeep == true)
        {
            m_PreMsgBarRequestInputMsg = m_MsgBarRequestInputMsg;
        }
        m_MsgBarRequestInputMsg = strMsg;

        if (isReset == true)
        {
            m_MsgBarRequestInputIndex = 0;
            m_MsgBarResetText = true;
        }
    }

    /// <summary>
    /// マーキーの表示・非表示
    /// </summary>
    /// <param name="isShow"></param>
    static public void IsViewOtherUnderMessage(bool isShow)
    {
        m_MsgBarResetText = true;
        m_IsViewMsgBarOtherMessage = isShow;
    }

    /// <summary>
    /// ミッションクリアのマーキー表示
    /// </summary>
    static public void SetupMissionClearMessage()
    {
        ClearMissionClearMessage();

        //----------------------------------------
        // 新規達成演出前のアチーブメントがあるならメッセージ表示
        //----------------------------------------
        if (ResidentParam.m_AchievementClear != null && ResidentParam.m_AchievementClear.m_BufferSize > 0)
        {
            m_MsgBarResetText = true;
            m_MsgBarRequestInputIndex = 0;

            for (int i = 0; i < ResidentParam.m_AchievementClear.m_BufferSize; ++i)
            {
                //----------------------------------------
                // アチーブメントが演出が必要なステータス状況かチェック
                //----------------------------------------
                PacketAchievement cAchievement = ResidentParam.m_AchievementClear[i];
                if (cAchievement == null)
                {
                    continue;
                }
                if (cAchievement.server_state != (int)ServerDataDefine.ACHIEVEMENT_STATE.ACHIEVEMENT_STATE_S1_T1_E0_S0)
                {
                    continue;
                }
                // クエストミッションはミッション画面に表示されず、直接アイテムを受け取るので弾く
                if (cAchievement.achievement_category_id == (int)ServerDataDefine.ACHIEVEMENT_CATEGORY_TYPE.QUEST)
                {
                    continue;
                }

                HeaderMessage message = new HeaderMessage();
                message.fix_id = cAchievement.fix_id;
                message.message = string.Format(GameTextUtil.GetText("achievement_complete"), TextUtil.RemoveNewLine(cAchievement.draw_msg));
                message.message_color = Color.white;
                message.bg_color = HexColor.ToColor("#f90974");
                message.type = HeaderMessage.MESSAGE_TYPE.ACHIEVEMENT_CLEAR;
                message.priority = ResidentParam.m_AchievementClear.m_BufferSize - i;
                m_MsgBarOtherMessageList.Add(message);
            }
            m_MsgBarOtherMessageList.Sort(HeaderMessage.SortCompare);
        }
    }

    /// <summary>
    /// ミッションクリアのマーキーを削除
    /// </summary>
    static private void ClearMissionClearMessage()
    {
        if (m_MsgBarOtherMessageList == null)
        {
            m_MsgBarOtherMessageList = new List<HeaderMessage>();
        }

        int removeCount = m_MsgBarOtherMessageList.RemoveAll((v) => v.type == HeaderMessage.MESSAGE_TYPE.ACHIEVEMENT_CLEAR);
        if (removeCount > 0)
        {
            m_MsgBarResetText = true;
            m_MsgBarRequestInputIndex = 0;
        }
    }

    /// <summary>
    /// ミッション発生のマーキー表示
    /// </summary>
    static public void SetupMissionNewMessae()
    {
        ClearMissionNewMessage();

        //----------------------------------------
        // 新規発生ミッションがあるならメッセージ表示
        //----------------------------------------
        if (ResidentParam.m_AchievementNewCnt > 0)
        {
            m_MsgBarResetText = true;
            m_MsgBarRequestInputIndex = 0;

            HeaderMessage message = new HeaderMessage();
            message.message = GameTextUtil.GetText("achievement_occurrence");
            message.message_color = Color.cyan;
            message.bg_color = Color.clear;
            message.type = HeaderMessage.MESSAGE_TYPE.ACHIEVEMENT_NEW;
            m_MsgBarOtherMessageList.Add(message);

            m_MsgBarOtherMessageList.Sort(HeaderMessage.SortCompare);
        }
    }

    /// <summary>
    /// ミッション発生のマーキーを削除
    /// </summary>
    static private void ClearMissionNewMessage()
    {
        if (m_MsgBarOtherMessageList == null)
        {
            m_MsgBarOtherMessageList = new List<HeaderMessage>();
        }

        int removeCount = m_MsgBarOtherMessageList.RemoveAll((v) => v.type == HeaderMessage.MESSAGE_TYPE.ACHIEVEMENT_NEW);
        if (removeCount > 0)
        {
            m_MsgBarResetText = true;
            m_MsgBarRequestInputIndex = 0;
        }
    }

    /// <summary>
    /// アイテム効果時間更新
    /// </summary>
    public void UpdateScrollMessage()
    {
        //----------------------------------------
        // 表示しているメッセージをリクエストに合わせて変更
        //----------------------------------------
        bool bScrollReset = false;
        if (m_MsgBarResetText == true)
        {
            m_CurrentMessageValue = null;
            m_MsgBarResetText = false;
            m_WorkMessageScrollTime = 0.0f;

            if (m_IsViewMsgBarOtherMessage == true
                && m_MsgBarRequestInputMsg.IsNullOrEmpty() == false
                && m_MsgBarOtherMessageList != null
                && m_MsgBarOtherMessageList.Count > 0)
            {
                // m_MessageValueListの中身が有ってもメインのメッセージが空の場合は表示しない

                if (m_MsgBarRequestInputIndex > m_MsgBarOtherMessageList.Count)
                {
                    m_MsgBarRequestInputIndex = 0;
                }

                if (m_MsgBarRequestInputIndex == m_MsgBarOtherMessageList.Count)
                {
                    // 通常の文言を表示する
                    m_WorkMessage = m_MsgBarRequestInputMsg;
                    MessageBGColor = Color.clear;
                    MessageColor = Color.white;
                }
                else
                {
                    HeaderMessage message = m_MsgBarOtherMessageList[m_MsgBarRequestInputIndex];
                    m_CurrentMessageValue = message;
                    m_WorkMessage = message.message;
                    MessageBGColor = message.bg_color;
                    MessageColor = message.message_color;
                }
            }
            else
            {
                m_WorkMessage = m_MsgBarRequestInputMsg;
                MessageBGColor = Color.clear;
                MessageColor = Color.white;
            }

            if (m_WorkMessage == null)
            {
                // メッセージを消す
                MessageValue = "";
            }
            else
            {
                // メッセージ表示してテキストを変更
                MessageValue = m_WorkMessage;
            }

            bScrollReset = true;
        }

        //----------------------------------------
        // 以下RTDから持ってきた動作部分
        //----------------------------------------
        // メッセージが存在するなら処理を行う
        if (m_WorkMessage != null)
        {
            float fDeltaRate = Time.deltaTime / (1.0f / 30.0f);
            //----------------------------------------
            // 位置情報更新
            //
            // 右から入ってきて左に抜けるため、
            // X座標を移動の判定に使用しているらしい
            //----------------------------------------
            if (bScrollReset == true)
            {
                //----------------------------------------
                // ステップ０． 初期化なら右に持っていく
                //----------------------------------------
                MessagePosX = 640.0f;
            }
            else if (MessagePosX >= 20)
            {
                //----------------------------------------
                // ステップ１． 最初の方は左に向かって早めに動く。
                //              一定位置まで行ったらステップ２へ
                //----------------------------------------
                MessagePosX -= (40.0f * fDeltaRate);

                m_WorkMessageScrollTime = 0.0f;
            }
            else if (m_WorkMessageScrollTime < 1.0f)
            {
                //----------------------------------------
                // ステップ２． 一定時間止まる
                //              時間経過でステップ３へ
                //----------------------------------------
                m_WorkMessageScrollTime += Time.deltaTime;
                MessagePosX = 0;
            }
            else
            {
                //----------------------------------------
                // ステップ３． 左にゆっくり抜ける
                //              しばらく進んだらステップ１へ戻る
                //----------------------------------------
                MessagePosX -= (3.0f * fDeltaRate);

                float fFontSize = m_MessageValueObjectRectTransform.sizeDelta.x * m_MessageValueObjectRectTransform.localScale.x;
                if (MessagePosX < -(fFontSize))
                {
                    // メーセージの削除
                    // 終わったら次の表示
                    m_MsgBarResetText = true;
                    if (m_IsViewMsgBarOtherMessage == true && m_MsgBarRequestInputMsg.IsNullOrEmpty() == false && m_MsgBarOtherMessageList != null && m_MsgBarOtherMessageList.Count > 0)
                    {
                        bool isRemove = false;

                        if (m_MsgBarRequestInputIndex >= 0 && m_MsgBarRequestInputIndex < m_MsgBarOtherMessageList.Count)
                        {
                            HeaderMessage message = m_MsgBarOtherMessageList[m_MsgBarRequestInputIndex];
                            if (message.deleted == true)
                            {
                                m_MsgBarOtherMessageList.Remove(message);
                                isRemove = true;
                            }
                        }
                        if (isRemove == false)
                        {
                            ++m_MsgBarRequestInputIndex;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// マーキーを選択したとき
    /// </summary>
    public void OnClickMessage()
    {
        if (m_ActiveHeaderTouch == false)
        {
            return;
        }

        if (m_CurrentMessageValue != null)
        {
            if (m_CurrentMessageValue.type == HeaderMessage.MESSAGE_TYPE.ACHIEVEMENT_CLEAR)
            {
                MainMenuParam.m_AchievementShowData = ResidentParam.GetAchievementClear(m_CurrentMessageValue.fix_id);
            }
            OpenGlobalMenu(GLOBALMENU_SEQ.MISSION);
        }
    }

    private void Update()
    {
        //スタミナ回復時間更新
        if (IsViewInfoWindow) updateStaminaTime();

        //アイテム効果更新
        updateItemEffect();

        //アイテム効果時間更新
        updateItemEffectTime();

        // スクロールメッセージの更新
        UpdateScrollMessage();
    }
}
