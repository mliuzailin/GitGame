using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;

public class MainMenuQuestSelectFriend : MainMenuSeq
{
    private FriendList m_FriendList = null;
    private PageTitle m_PageTitle = null;

    private PacketStructFriend m_SelectFriend = null;

    private bool m_FixFriendUnit = false;
    private bool m_FixFriendLinkUnit = false;
    /// <summary>フレンドポイント取得可能なクエストかどうか</summary>
    bool m_GetQuestFriendPointEnabel = false;

    protected override void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        if (m_FriendList == null)
        {
            m_FriendList = m_CanvasObj.GetComponentInChildren<FriendList>();
            m_FriendList.SetPositionAjustStatusBar(new Vector2(0, 26), new Vector2(-60, -422));
            m_FriendList.CheckLock = true;//お気に入りチェックON
            m_FriendList.IsViewReload = true; // 更新ボタン表示
        }
        if (m_PageTitle == null)
        {
            m_PageTitle = m_CanvasObj.GetComponentInChildren<PageTitle>();
            m_PageTitle.SetPositionAjustStatusBar(new Vector2(0, -152));
        }


        //---------------------------------------
        // フレンドポイント取得可能なクエストかどうか
        //---------------------------------------
        m_GetQuestFriendPointEnabel = MasterDataUtil.GetQuest2FriendPointEnabel(MainMenuParam.m_QuestSelectMissionID);

        setupFriend();

        //------------------------------------
        // エピソード名・ストーリー名の設定
        //------------------------------------
        m_PageTitle.Title = GameTextUtil.GetText("questfriend_title");

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.QUEST;
    }

    public override bool PageSwitchEventEnableAfter()
    {
        //戻るアクション設定
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.SetMenuReturnAction(true, SelectReturn);
        }

        return base.PageSwitchEventEnableAfter();
    }


    public override bool PageSwitchEventDisableBefore()
    {
        //戻るアクション解除
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.SetMenuReturnAction(false, null);
        }
        return base.PageSwitchEventDisableBefore();
    }

    private void startQuest()
    {
        if (m_SelectFriend == null)
        {
            return;
        }
#if BUILD_TYPE_DEBUG
        Debug.Log("startQuest");
#endif
        //ヘルパー設定
        PacketStructFriend helper = new PacketStructFriend();
        helper.user_id = m_SelectFriend.user_id;
        helper.user_name = m_SelectFriend.user_name;
        helper.user_rank = m_SelectFriend.user_rank;
        helper.friend_state = m_SelectFriend.friend_state;
        helper.last_play = m_SelectFriend.last_play;
        helper.unit = new PacketStructUnit();
        helper.unit.Copy(m_SelectFriend.unit);
        helper.unit_link.Copy(m_SelectFriend.unit_link);
#if BUILD_TYPE_DEBUG
        //フレンドユニット置き換え処理
        if (ForceFriendUnitParam.Enable &&
            !m_FixFriendUnit &&
            !m_FixFriendLinkUnit)
        {
            bool replace = false;
            if (ForceFriendUnitParam.BaseUnit != null)
            {
                helper.unit.Copy(ForceFriendUnitParam.BaseUnit);
                replace = true;
            }
            if (ForceFriendUnitParam.LinkUnit != null)
            {
                helper.unit_link.Copy(ForceFriendUnitParam.LinkUnit);
                if (ForceFriendUnitParam.BaseUnit == null)
                {
                    helper.unit.link_info = (int)CHARALINK_TYPE.CHARALINK_TYPE_BASE;
                    helper.unit.link_point = ForceFriendUnitParam.LinkUnit.link_point;
                }
                replace = true;
            }
            if (replace)
            {
                helper.user_name = "user";
                helper.friend_state = (int)FRIEND_STATE.FRIEND_STATE_SUCCESS;
            }
        }
#endif
        MainMenuParam.m_QuestHelper = helper;

        //クエスト開始
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_PARTY, false, false);
    }

    private void setupFriend()
    {
        List<FriendDataSetting> friendList = new List<FriendDataSetting>();

        MasterDataQuest2 cQuestMasterData = MasterDataUtil.GetQuest2ParamFromID(MainMenuParam.m_QuestSelectMissionID);
        uint requirement_id = 0;
        if (cQuestMasterData != null)
        {
            requirement_id = cQuestMasterData.quest_requirement_id;
        }

        CreateFriendList(ref friendList, requirement_id);

        setupFriendList(ref friendList);

        m_FriendList.FriendBaseList.Body = friendList;

        m_FriendList.SetUpSortData(LocalSaveManager.Instance.LoadFuncSortFilterQuestFriend());
        m_FriendList.OnClickSortButtonAction = OnClockSortButton;
        m_FriendList.OnClickReloadButtonAction = OnClickReloadButton;
        m_FriendList.Init();
    }

    private void setupFriendList(ref List<FriendDataSetting> friend_list)
    {
        for (int i = 0; i < friend_list.Count; i++)
        {
            friend_list[i].CharaOnce = MainMenuUtil.CreateFriendCharaOnce(friend_list[i].FriendData);
            //ソート用パラメータ設定
            friend_list[i].setSortParamFriend(friend_list[i].FriendData, friend_list[i].CharaOnce, friend_list[i].MasterData);
            friend_list[i].IsEnableButton = true;
            friend_list[i].DidSelectFriend += OnSelectFriend;
            friend_list[i].DidSelectIcon += OnSelectUnitIcon;
            switch (friend_list[i].FriendData.friend_state)
            {
                case (uint)FRIEND_STATE.FRIEND_STATE_SUCCESS:
                    friend_list[i].m_Flag = FriendDataItem.FlagType.FRIEND;
                    break;
                case (uint)FRIEND_STATE.FRIEND_STATE_UNRELATED:
                    friend_list[i].m_Flag = FriendDataItem.FlagType.HELPER;
                    break;
                default:
                    break;
            }

            //	クエストの設定を見てフレンドポイントを付与するかどうかを決める
            if (m_GetQuestFriendPointEnabel == false)
            {
                friend_list[i].IsViewFriendPoint = false;
            }
            else
            {
                friend_list[i].IsViewFriendPoint = (LocalSaveManager.Instance.GetLocalSaveUseFriend(friend_list[i].FriendData.user_id) == null);
            }
        }
    }

    public void OnSelectFriend(FriendDataItem _unit)
    {
        if (!m_FixFriendUnit ||
            m_FixFriendLinkUnit)
        {
            int _index = UserDataAdmin.Instance.SearchHelperIndex(_unit.FriendData.user_id);
            if (_index == -1)
            {
                return;
            }
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        m_SelectFriend = _unit.FriendData;

        Dialog dlg = Dialog.Create(DialogType.DialogUnit);
        dlg.setUnitInfo(m_SelectFriend);
        dlg.SetDialogText(DialogTextType.Title, m_SelectFriend.user_name);
        dlg.SetDialogText(DialogTextType.SubTitle, string.Format(GameTextUtil.GetText("questfriend_text1"), m_SelectFriend.user_rank.ToString()));
        dlg.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button7");
        dlg.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            startQuest();
        });
        dlg.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button1");
        dlg.Show();
    }

    public void OnSelectUnitIcon(FriendDataItem _friend)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.OpenUnitDetailInfoFriend(_friend.FriendData);
        }
    }

    public void SelectReturn()
    {
        if (!MainMenuManager.HasInstance)
        {
            return;
        }

        // エリア移動
        if (MasterDataUtil.GetQuestType(MainMenuParam.m_QuestSelectMissionID) == MasterDataDefineLabel.QuestType.CHALLENGE)
        {
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_CHALLENGE_SELECT, false, true);
        }
        else
        {
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_DETAIL, false, true);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	フレンドリストの生成処理
	*/
    //----------------------------------------------------------------------------
    private void CreateFriendList(ref List<FriendDataSetting> friend_list, uint requirement_id)
    {
        if (friend_list == null)
        {
            return;
        }

        m_FixFriendUnit = false;
        m_FixFriendLinkUnit = false;

        //--------------------------------
        //	フレンド固定の指定があれば処理
        //--------------------------------
        PacketStructUnit unit = null;
        PacketStructUnit linkUnit = null;
        MasterDataQuestRequirement quest_requirement = null;

        uint unLinkPoint = 0;
        //bool bLinkUnitOnlyFix = false;

        if (requirement_id != 0) quest_requirement = MasterDataUtil.GetMasterDataQuestRequirementFromID(requirement_id);
        if (quest_requirement != null)
        {
            // リンクポイントを取得
            unLinkPoint = (uint)quest_requirement.fix_unit_04_link_point;

            // ベースユニット強制置き換え
            if (quest_requirement.fix_unit_04_enable == MasterDataDefineLabel.BoolType.ENABLE)
            {
                if (quest_requirement.fix_unit_04_id != 0)
                {
                    unit = MainMenuUtil.CreateDummyFriendUnit(quest_requirement.fix_unit_04_id,
                                                               (uint)quest_requirement.fix_unit_04_lv,
                                                               (uint)quest_requirement.fix_unit_04_plus_atk,
                                                               (uint)quest_requirement.fix_unit_04_plus_hp,
                                                                     0,
                                                               (uint)quest_requirement.fix_unit_04_lv_lbs,
                                                               (uint)quest_requirement.fix_unit_04_lv_lo,
                                                                     unLinkPoint);
                    m_FixFriendUnit = true;
                }
            }

            // リンクユニット強制置き換え
            switch (quest_requirement.fix_unit_04_link_enable)
            {
                case MasterDataDefineLabel.BoolType.ENABLE:
                    if (quest_requirement.fix_unit_04_link_id != 0)
                    {
                        linkUnit = MainMenuUtil.CreateDummyUnit(quest_requirement.fix_unit_04_link_id,
                                                                 (uint)quest_requirement.fix_unit_04_link_lv,
                                                                       0,
                                                                 (uint)quest_requirement.fix_unit_04_link_lv_lo,
                                                                 (uint)quest_requirement.fix_unit_04_link_plus_atk,
                                                                 (uint)quest_requirement.fix_unit_04_link_plus_hp,
                                                                       0,
                                                                 (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK);
                    }
                    // 強制リンク外し
                    else
                    {
                        linkUnit = MainMenuUtil.CreateDummyUnit(0, 0, 0, 0, 0, 0, 0);
                    }

                    // ベースユニットも固定の場合
                    if (unit != null)
                    {
                        if (linkUnit != null)
                        {
                            unit.link_info = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE;
                            unit.link_unique_id = 1;
                        }
                        else
                        {
                            unit.link_info = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE;
                            unit.link_unique_id = 0;
                        }
                    }
                    // リンクユニットのみ固定の場合
                    else
                    {
                        m_FixFriendLinkUnit = true;
                    }
                    break;

                // 強制リンク外し
                case MasterDataDefineLabel.BoolType.DISABLE:
                    linkUnit = MainMenuUtil.CreateDummyUnit(0, 0, 0, 0, 0, 0, 0);
                    // ベースユニットが固定の場合
                    if (unit != null)
                    {
                        unit.link_info = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE;
                        unit.link_unique_id = 0;
                    }
                    // リンクユニットのみ固定の場合
                    else
                    {
                        m_FixFriendLinkUnit = true;
                    }
                    break;

                case MasterDataDefineLabel.BoolType.NONE:
                    break;
            }
        }
        //--------------------------------
        //	現在のフレンドリストを削除
        //--------------------------------　
        m_SelectFriend = null;
        friend_list.Clear();

        List<MasterDataParamChara> charaMasterList = MasterFinder<MasterDataParamChara>.Instance.FindAll();

        //--------------------------------
        //	新しいフレンドリストの作成
        //--------------------------------
        if (unit == null)
        {
            //--------------------------------
            //	固定フレンドの指定がない
            //--------------------------------

            // 助っ人の総数を取得
            int nFriendListTotal = 0;
            if (UserDataAdmin.Instance.m_StructHelperList != null)
            {
                nFriendListTotal += UserDataAdmin.Instance.m_StructHelperList.Length;
            }
            if (UserDataAdmin.Instance.m_StructFriendList != null)
            {
                nFriendListTotal += UserDataAdmin.Instance.m_StructFriendList.Length;
            }

            //	フレンド登録ユーザーリストの取得
            TemplateList<uint> cFavoriteFriendList = LocalSaveManager.Instance.LoadFuncAddFavoriteFriend();

            #region ==== 助っ人をフレンドリストに追加 =====
            //	助っ人をフレンドリストに追加
            //			friend_list.Alloc( nFriendListTotal );
            if (UserDataAdmin.Instance.m_StructHelperList != null)
            {
                for (int i = 0; i < UserDataAdmin.Instance.m_StructHelperList.Length; i++)
                {
                    if (UserDataAdmin.Instance.m_StructHelperList[i] == null)
                    {
                        continue;
                    }

                    FriendDataSetting cFriendParam = new FriendDataSetting();

                    if (m_FixFriendLinkUnit == false)
                    {
                        cFriendParam.FriendData = UserDataAdmin.Instance.m_StructHelperList[i];
                    }
                    // リンクユニットのみ固定の場合
                    else
                    {
                        // ヘルパーデータをコピー
                        cFriendParam.FriendData = new PacketStructFriend();
                        cFriendParam.FriendData.user_id = UserDataAdmin.Instance.m_StructHelperList[i].user_id;
                        cFriendParam.FriendData.user_name = UserDataAdmin.Instance.m_StructHelperList[i].user_name;
                        cFriendParam.FriendData.user_rank = UserDataAdmin.Instance.m_StructHelperList[i].user_rank;
                        cFriendParam.FriendData.last_play = UserDataAdmin.Instance.m_StructHelperList[i].last_play;
                        cFriendParam.FriendData.friend_point = UserDataAdmin.Instance.m_StructHelperList[i].friend_point;
                        cFriendParam.FriendData.friend_state = UserDataAdmin.Instance.m_StructHelperList[i].friend_state;
                        cFriendParam.FriendData.friend_state_update = UserDataAdmin.Instance.m_StructHelperList[i].friend_state_update;

                        unit = UserDataAdmin.Instance.m_StructHelperList[i].unit;
                        // リンク固定がある場合
                        if (linkUnit != null)
                        {
                            cFriendParam.FriendData.unit = MainMenuUtil.CreateDummyFriendUnit(unit.id, unit.level, unit.add_pow, unit.add_hp, 0, unit.limitbreak_lv,
                                                                                               unit.limitover_lv, unLinkPoint, (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE);
                            cFriendParam.FriendData.unit_link = linkUnit;
                        }
                        else
                        {
                            cFriendParam.FriendData.unit = MainMenuUtil.CreateDummyFriendUnit(unit.id, unit.level, unit.add_pow, unit.add_hp, 0, unit.limitbreak_lv,
                                                                                               unit.limitover_lv, unLinkPoint, (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE);
                        }
                    }

                    cFriendParam.MasterData = charaMasterList.Find((v) => v.fix_id == cFriendParam.FriendData.unit.id);

                    friend_list.Add(cFriendParam);
                }
            }
            #endregion

            #region ==== フレンド登録をしたフレンドを追加 =====
            //	フレンド登録をしたフレンドを追加
            if (UserDataAdmin.Instance.m_StructFriendList != null)
            {
                for (int i = 0; i < UserDataAdmin.Instance.m_StructFriendList.Length; i++)
                {
                    if (UserDataAdmin.Instance.m_StructFriendList[i] == null)
                    {
                        continue;
                    }

                    FriendDataSetting cFriendParam = new FriendDataSetting();

                    if (m_FixFriendLinkUnit == false)
                    {
                        cFriendParam.FriendData = UserDataAdmin.Instance.m_StructFriendList[i];
                    }
                    // リンクユニット固定の場合
                    else
                    {
                        // フレンドデータをコピー
                        cFriendParam.FriendData = new PacketStructFriend();
                        cFriendParam.FriendData.user_id = UserDataAdmin.Instance.m_StructFriendList[i].user_id;
                        cFriendParam.FriendData.user_name = UserDataAdmin.Instance.m_StructFriendList[i].user_name;
                        cFriendParam.FriendData.user_rank = UserDataAdmin.Instance.m_StructFriendList[i].user_rank;
                        cFriendParam.FriendData.last_play = UserDataAdmin.Instance.m_StructFriendList[i].last_play;
                        cFriendParam.FriendData.friend_point = UserDataAdmin.Instance.m_StructFriendList[i].friend_point;
                        cFriendParam.FriendData.friend_state = UserDataAdmin.Instance.m_StructFriendList[i].friend_state;
                        cFriendParam.FriendData.friend_state_update = UserDataAdmin.Instance.m_StructFriendList[i].friend_state_update;

                        unit = UserDataAdmin.Instance.m_StructFriendList[i].unit;
                        if (linkUnit != null)
                        {
                            cFriendParam.FriendData.unit = MainMenuUtil.CreateDummyFriendUnit(unit.id, unit.level, unit.add_pow, unit.add_hp, 0, unit.limitbreak_lv,
                                                                                               unLinkPoint, (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE);
                            cFriendParam.FriendData.unit_link = linkUnit;
                        }
                        else
                        {
                            cFriendParam.FriendData.unit = MainMenuUtil.CreateDummyFriendUnit(unit.id, unit.level, unit.add_pow, unit.add_hp, 0, unit.limitbreak_lv,
                                                                                               unLinkPoint, (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE);
                        }
                    }

                    cFriendParam.MasterData = charaMasterList.Find((v) => v.fix_id == cFriendParam.FriendData.unit.id);

                    if (cFavoriteFriendList != null)
                    {
                        cFriendParam.IsActiveLock = cFavoriteFriendList.ChkInside(TemplateListSort.ChkInsideUint, cFriendParam.FriendData.user_id);
                    }
                    else
                    {
                        cFriendParam.IsActiveLock = false;
                    }


                    friend_list.Add(cFriendParam);
                }
            }
            #endregion

            // フレンドのメンバーを条件で間引き
            friend_list = FriendListThinning(friend_list);
        }
        else
        {
            //--------------------------------
            //	固定フレンドの指定があった
            //--------------------------------
            MasterDataParamChara param_chara = charaMasterList.Find((v) => v.fix_id == unit.id);
            if (param_chara != null)
            {
                PacketStructFriend friend = new PacketStructFriend();
                friend.unit = unit;
                friend.user_id = 0;
                friend.user_name = param_chara.name;
                friend.user_rank = 999;
                friend.last_play = unit.get_time;
                friend.friend_point = 0;
                friend.friend_state = (uint)FRIEND_STATE.FRIEND_STATE_SUCCESS;

                // リンク固定がある場合
                if (linkUnit != null)
                {
                    friend.unit_link = linkUnit;
                }

                FriendDataSetting param_friend = new FriendDataSetting();
                param_friend.FriendData = friend;
                param_friend.MasterData = param_chara;

                friend_list.Add(param_friend);
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ソート処理：フレンド：ユニットID順
	*/
    //----------------------------------------------------------------------------
    private List<FriendDataSetting> FriendListThinning(List<FriendDataSetting> cFriendList)
    {
        List<FriendDataSetting> cRetFriendList = new List<FriendDataSetting>();
        List<FriendDataSetting> cWorkFriendList = new List<FriendDataSetting>();

        LocalSaveManager.Instance.LoadFuncUseFriend();

        //----------------------------------------
        // 手順１。ログイン時間が一定期間内のフレンドで最近使ってない子を選定
        //----------------------------------------
        {
            for (int i = 0; i < cFriendList.Count; i++)
            {
                //----------------------------------------
                // フレンド成立済な子のみ判定
                //----------------------------------------
                if (cFriendList[i] == null
                || cFriendList[i].FriendData.friend_state != (int)FRIEND_STATE.FRIEND_STATE_SUCCESS
                )
                {
                    continue;
                }

                //----------------------------------------
                // 指定フレンドが最近使った子かチェック
                //----------------------------------------
                LocalSaveFriendUse sFriendUseParam = LocalSaveManager.Instance.GetLocalSaveUseFriend(cFriendList[i].FriendData.user_id);
                if (sFriendUseParam != null)
                {
                    continue;
                }

                //----------------------------------------
                // 優先度 = ( 14時間 - ログインからの経過時間 )
                //----------------------------------------
                TimeSpan cTimeSpan = TimeManager.Instance.m_TimeNow - TimeUtil.ConvertServerTimeToLocalTime(cFriendList[i].FriendData.last_play);
                float fPriority = (GlobalDefine.FRIEND_CYCLE_HOURS - (float)cTimeSpan.TotalHours);
                if (fPriority < 0)
                {
                    continue;
                }

                //----------------------------------------
                // 出てきても良いフレンドとして選定
                // 後でソートするのでログインタイミングによる優先度をソートキーとして設定
                //----------------------------------------
                cWorkFriendList.Add(cFriendList[i]);
            }
            //----------------------------------------
            // 優先度から見て先着順でリストに登録
            //----------------------------------------
            for (int i = 0; i < cWorkFriendList.Count; i++)
            {
                //if( cRetFriendList.m_BufferSize >= GlobalDefine.FRIEND_DRAW_MAX_FRIEND )
                //    break;
                cRetFriendList.Add(cWorkFriendList[i]);
            }

            //----------------------------------------
            // 次の要素を追加で積むので作業用リストは破棄
            //----------------------------------------
            cWorkFriendList.Clear();
        }

        //----------------------------------------
        // 手順２。ログイン時間が一定期間内のフレンドで使用済な子を選定
        //----------------------------------------
        {
            for (int i = 0; i < cFriendList.Count; i++)
            {
                //----------------------------------------
                // フレンド成立済な子のみ判定
                //----------------------------------------
                if (cFriendList[i] == null
                || cFriendList[i].FriendData.friend_state != (int)FRIEND_STATE.FRIEND_STATE_SUCCESS
                )
                {
                    continue;
                }

                //----------------------------------------
                // 指定フレンドが最近使った子かチェック
                // こちらでは使用済の子のみ扱う
                //----------------------------------------
                LocalSaveFriendUse sFriendUseParam = LocalSaveManager.Instance.GetLocalSaveUseFriend(cFriendList[i].FriendData.user_id);
                if (sFriendUseParam == null)
                {
                    continue;
                }
#if true// このコードをコメントアウトするとフレンドが無限につかえるようになる。
                {
                    //----------------------------------------
                    // 連続で使用できないように１時間以内に使用した子は拒否
                    //----------------------------------------
                    DateTime cFriendUseTime = TimeUtil.ConvertServerTimeToLocalTime(sFriendUseParam.m_FriendUseTime);
                    TimeSpan cFriendUseSpan = TimeManager.Instance.m_TimeNow - cFriendUseTime;
                    if (cFriendUseSpan.TotalHours <= 1.0f)
                    {
                        continue;
                    }
                }
#endif
                //----------------------------------------
                // 優先度 = ( 14時間 - ログインからの経過時間 )
                //----------------------------------------
                TimeSpan cTimeSpan = TimeManager.Instance.m_TimeNow - TimeUtil.ConvertServerTimeToLocalTime(cFriendList[i].FriendData.last_play);
                float fPriority = (GlobalDefine.FRIEND_CYCLE_HOURS - (float)cTimeSpan.TotalHours);
                if (fPriority < 0)
                {
                    continue;
                }

                //----------------------------------------
                // 出てきても良いフレンドとして選定
                // 後でソートするのでログインタイミングによる優先度をソートキーとして設定
                //----------------------------------------
                cWorkFriendList.Add(cFriendList[i]);
            }
            //----------------------------------------
            // 優先度から見て先着順でリストに登録
            //----------------------------------------
            for (int i = 0; i < cWorkFriendList.Count; i++)
            {
                //if( cRetFriendList.m_BufferSize >= GlobalDefine.FRIEND_DRAW_MAX_FRIEND + GlobalDefine.FRIEND_DRAW_MAX_FRIEND_USE )
                //    break;
                cRetFriendList.Add(cWorkFriendList[i]);
            }

            //----------------------------------------
            // 次の要素を追加で積むので作業用リストは破棄
            //----------------------------------------
            cWorkFriendList.Clear();
        }

        //----------------------------------------
        // 手順３。助っ人をテキトーに選定
        //----------------------------------------
        {
            for (int i = 0; i < cFriendList.Count; i++)
            {
                //----------------------------------------
                // 無関係な子のみ判定
                //----------------------------------------
                if (cFriendList[i] == null
                || cFriendList[i].FriendData.friend_state != (int)FRIEND_STATE.FRIEND_STATE_UNRELATED
                )
                {
                    continue;
                }

#if true// このコードをコメントアウトすると助っ人が無限につかえるようになる。
                //----------------------------------------
                // 指定フレンドが最近使った子かチェック
                //----------------------------------------
                LocalSaveFriendUse sFriendUseParam = LocalSaveManager.Instance.GetLocalSaveUseFriend(cFriendList[i].FriendData.user_id);
                if (sFriendUseParam != null)
                {
                    //----------------------------------------
                    // 連続で使用できないように１時間以内に使用した子は拒否
                    //----------------------------------------
                    DateTime cFriendUseTime = TimeUtil.ConvertServerTimeToLocalTime(sFriendUseParam.m_FriendUseTime);
                    TimeSpan cFriendUseSpan = TimeManager.Instance.m_TimeNow - cFriendUseTime;
                    if (cFriendUseSpan.TotalHours <= 1.0f)
                    {
                        continue;
                    }
                }
#endif

                //----------------------------------------
                // サーバー側でフレンドリストに含まれる子を助っ人リストに入れてしまうことがあるらしい
                // フレンドに同じユーザーIDの子がいるならスルー
                //----------------------------------------
                bool bSameAssign = false;
                for (int j = 0; j < cFriendList.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (cFriendList[j] == null
                    || cFriendList[j].FriendData.user_id != cFriendList[i].FriendData.user_id
                    )
                    {
                        continue;
                    }

                    bSameAssign = true;
                    break;
                }
                if (bSameAssign == true)
                {
                    continue;
                }

                //----------------------------------------
                // テキトーな優先度でランダムに選定
                //----------------------------------------
                cWorkFriendList.Add(cFriendList[i]);
            }

            //----------------------------------------
            // 優先度から見て先着順でリストに登録
            //----------------------------------------
            for (int i = 0; i < cWorkFriendList.Count; i++)
            {
                //if( cRetFriendList.m_BufferSize >= GlobalDefine.FRIEND_DRAW_MAX )
                //    break;
                cRetFriendList.Add(cWorkFriendList[i]);
            }

            //----------------------------------------
            // 次の要素を追加で積むので作業用リストは破棄
            //----------------------------------------
            cWorkFriendList.Clear();
        }

        return cRetFriendList;
    }

    /// <summary>
    /// ソートダイアログを開く
    /// </summary>
    void OnClockSortButton()
    {
        if (SortDialog.IsExists == true)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        SortDialog dialog = SortDialog.Create();
        dialog.SetDialogType(SortDialog.DIALOG_TYPE.FRIEND);
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterQuestFriend());
        dialog.OnCloseAction = OnClickSortCloseButton;
    }

    /// <summary>
    /// リストの更新
    /// </summary>
    void OnClickReloadButton()
    {
        if (ServerApi.IsExists == true)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        ServerDataUtilSend.SendPacketAPI_LoginPack(false, true, true, false).
            setSuccessAction(
                _data =>
                {
                    RecvLoginPackValue cLoginPack = _data.GetResult<RecvLoginPack>().result;

                    //----------------------------------------
                    // 助っ人情報保持
                    //----------------------------------------
                    if (cLoginPack.result_helper != null
                    && cLoginPack.result_helper.friend != null
                    )
                    {
                        UserDataAdmin.Instance.m_StructHelperList = UserDataAdmin.FriendListClipMe(cLoginPack.result_helper.friend);
                    }

                    //----------------------------------------
                    // フレンド情報保持
                    //----------------------------------------
                    if (cLoginPack.result_friend != null
                    && cLoginPack.result_friend.friend != null
                    )
                    {
                        UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(cLoginPack.result_friend.friend);
                        LocalSaveManager.Instance.SaveFuncAddFavoriteFriendClip(UserDataAdmin.Instance.m_StructFriendList);
                    }

                    // 更新ボタンの無効化
                    MainMenuParam.m_IsEnableQuestFriendReload = false;
                    m_FriendList.m_ReloadButton.SetEnable(false);

                    // 更新
                    setupFriend();
                }).
            setErrorAction(
                _data =>
                {

                }).
            SendStart();
    }

    /// <summary>
    /// ソートダイアログを閉じたとき
    /// </summary>
    void OnClickSortCloseButton(LocalSaveSortInfo sortInfo)
    {
        //--------------------------------
        // データ保存
        //--------------------------------
        LocalSaveManager.Instance.SaveFuncSortFilterQuestFriend(sortInfo);

        //TODO ソートテスト
        //m_FriendList.FriendBaseList.AddSortInfo(SORT_PARAM.RANK, SORT_ORDER.DESCENDING);
        //m_FriendList.FriendBaseList.AddSortInfo(SORT_PARAM.ELEMENT, SORT_ORDER.ASCENDING);
        //m_FriendList.FriendBaseList.AddSortInfo(SORT_PARAM.LOGIN_TIME, SORT_ORDER.DESCENDING);

        //TODO フィルタテスト
        //m_FriendList.FriendBaseList.AddFilter<int>(SORT_PARAM.ELEMENT, new int[] { (int)MasterDataDefineLabel.ElementType.FIRE });

        m_FriendList.ExecSort(sortInfo);
    }
}
