
/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MainMenuSeqQuestFix.cs
	@brief	メインメニューシーケンス：クエスト枠：クエスト確定
	@author Developer
	@date 	2012/11/27
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@brief	メインメニューシーケンス：クエスト枠：クエスト確定
*/
//----------------------------------------------------------------------------
public class MainMenuQuestServerSend : MainMenuSeq
{
    const int LINKER_CHARA_0 = 0;       //!< リンカID：キャラ：0番
    const int LINKER_CHARA_1 = 1;       //!< リンカID：キャラ：1番
    const int LINKER_CHARA_2 = 2;       //!< リンカID：キャラ：2番
    const int LINKER_CHARA_3 = 3;       //!< リンカID：キャラ：3番
    const int LINKER_CHARA_4 = 4;       //!< リンカID：キャラ：4番
    const int LINKER_CHARA_MAX = 5;     //!< リンカID：キャラ上限
    const int LINKER_BACK = 6;      //!< リンカID：戻る
    const int LINKER_FIX = 7;       //!< リンカID：確定
    const int LINKER_MAX = 8;       //!< リンカID：

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    //	private		GameObject[]	m_LinkerObject	= new GameObject[ MainMenuSeqQuestFix.LINKER_MAX ];

    //	private		uint			m_PacketUniqueID = 0;		//!< サーバー通信ユニーク番号

    private MasterDataGuerrillaBoss m_GuerrillaBoss = null;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();


    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public new void Update()
    {
        //----------------------------------------
        // 固定処理
        // 管理側からの更新許可やフェード待ち含む。
        //----------------------------------------
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	基底継承：MainMenuSeq：ページ切り替えにより有効化された際に呼ばれる関数
		@note	ページのレイアウト再構築を兼ねる
	*/
    //----------------------------------------------------------------------------
    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //----------------------------------------
        // ゲリラボス情報を取得
        //----------------------------------------
        m_GuerrillaBoss = MasterDataUtil.GetGuerrillaBossParamFromQuestID(MainMenuParam.m_QuestSelectMissionID);

        //----------------------------------------
        // ゲーム本編に引き渡す情報を色々設定
        //----------------------------------------
        {
            //----------------------------------------
            // 選択されている助っ人を取得
            //----------------------------------------
            PacketStructFriend cHelper = MainMenuParam.m_QuestHelper;
            if (cHelper == null)
            {
                Debug.LogError("SelectFriendNone");
            }

            //----------------------------------------
            //
            //----------------------------------------
            int nPartyCurrent = UserDataAdmin.Instance.m_StructPlayer.unit_party_current;
            PacketStructUnit[] acUnitStruct = {
                                    UserDataAdmin.Instance.m_StructPartyAssign[ nPartyCurrent ][0]
                                ,   UserDataAdmin.Instance.m_StructPartyAssign[ nPartyCurrent ][1]
                                ,   UserDataAdmin.Instance.m_StructPartyAssign[ nPartyCurrent ][2]
                                ,   UserDataAdmin.Instance.m_StructPartyAssign[ nPartyCurrent ][3]
                                ,   cHelper.unit
                                };

            // @add Developer 2015/09/07 ver300
            PacketStructUnit cLinkUnitStruct = null;

            UserDataUnitParam[] acUnitParam = new UserDataUnitParam[acUnitStruct.Length];
            for (int i = 0; i < acUnitStruct.Length; i++)
            {
                if (acUnitStruct[i] == null)
                    continue;

                acUnitParam[i] = new UserDataUnitParam();
                acUnitParam[i].m_UnitDataID = acUnitStruct[i].id;
                acUnitParam[i].m_UnitParamLevel = (int)acUnitStruct[i].level;
                acUnitParam[i].m_UnitParamEXP = (int)acUnitStruct[i].exp;
                acUnitParam[i].m_UnitParamUniqueID = acUnitStruct[i].unique_id;
                acUnitParam[i].m_UnitParamLimitBreakLV = (int)acUnitStruct[i].limitbreak_lv;
                acUnitParam[i].m_UnitParamLimitOverLV = (int)acUnitStruct[i].limitover_lv;
                acUnitParam[i].m_UnitParamPlusPow = (int)acUnitStruct[i].add_pow;
                acUnitParam[i].m_UnitParamPlusHP = (int)acUnitStruct[i].add_hp;

                // @add Developer 2015/09/07 ver300
                // リンクユニットを取得
                if (i != acUnitStruct.Length - 1)
                {
                    cLinkUnitStruct = CharaLinkUtil.GetLinkUnit(acUnitStruct[i].link_unique_id);
                }
                else
                {
                    cLinkUnitStruct = cHelper.unit_link;
                }

                // リンクユニットのパラメータ設定
                if (cLinkUnitStruct == null)
                {
                    continue;
                }
                acUnitParam[i].m_UnitParamLinkID = cLinkUnitStruct.id;
                acUnitParam[i].m_UnitParamLinkLv = (int)cLinkUnitStruct.level;
                acUnitParam[i].m_UnitParamLinkPlusPow = (int)cLinkUnitStruct.add_pow;
                acUnitParam[i].m_UnitParamLinkPlusHP = (int)cLinkUnitStruct.add_hp;
                acUnitParam[i].m_UnitParamLinkPoint = (int)acUnitStruct[i].link_point;
                acUnitParam[i].m_UnitParamLinkLimitOverLV = (int)cLinkUnitStruct.limitover_lv;
            }


            //----------------------------------------
            // 固定パーティ情報でパーティ情報を上書き
            //----------------------------------------
            FixPartyAssign(ref acUnitParam, MainMenuParam.m_QuestSelectMissionID);

            {
                SceneGoesParamToQuest2 cSceneGoesParamToQuest2 = new SceneGoesParamToQuest2();

                cSceneGoesParamToQuest2.m_QuestAreaID = MainMenuParam.m_QuestSelectAreaID;
                cSceneGoesParamToQuest2.m_QuestAreaAmendCoin = MainMenuParam.m_QuestSelectAreaAmendCoin;      // ※パーセント表記
                cSceneGoesParamToQuest2.m_QuestAreaAmendDrop = MainMenuParam.m_QuestSelectAreaAmendDrop;      // ※パーセント表記
                cSceneGoesParamToQuest2.m_QuestAreaAmendExp = MainMenuParam.m_QuestSelectAreaAmendEXP;        // ※パーセント表記
                cSceneGoesParamToQuest2.m_QuestMissionID = MainMenuParam.m_QuestSelectMissionID;
                cSceneGoesParamToQuest2.m_QuestRandSeed = RandManager.GetRand();
                cSceneGoesParamToQuest2.m_IsUsedAutoPlay = false;
                cSceneGoesParamToQuest2.m_QuestGuerrillaBoss = m_GuerrillaBoss;

                cSceneGoesParamToQuest2.m_PartyFriend = cHelper;
                cSceneGoesParamToQuest2.m_PartyChara0Param = acUnitParam[0];
                cSceneGoesParamToQuest2.m_PartyChara1Param = acUnitParam[1];
                cSceneGoesParamToQuest2.m_PartyChara2Param = acUnitParam[2];
                cSceneGoesParamToQuest2.m_PartyChara3Param = acUnitParam[3];
                cSceneGoesParamToQuest2.m_PartyChara4Param = acUnitParam[4];

                cSceneGoesParamToQuest2.m_NextAreaCleard = MainMenuUtil.ChkActiveNextArea(MainMenuParam.m_QuestSelectAreaID);

                SceneGoesParam.Instance.m_SceneGoesParamToQuest2 = cSceneGoesParamToQuest2;

                //----------------------------------------
                // ゲーム開始情報をローカル保存。
                //
                // 通信前に保存しているため、通信中にアプリを終了させるとサーバー側の諸々消費を飛ばして次回起動時に開始情報が残ってしまう。
                // そのままクエストに入られるとサーバー上でコスト消費が行われずにクエストに移行する可能性があるがので、
                // タイトル画面で「サーバーにクエスト開始が届いているか」をチェックするAPIを投げて届いている場合のみ適用することで対応する。
                //----------------------------------------
                if (LocalSaveManager.Instance != null)
                {
                    LocalSaveManager.Instance.SaveFuncGoesToMenuResult(null);
                    LocalSaveManager.Instance.SaveFuncGoesToMenuRetire(null);
                    LocalSaveManager.Instance.SaveFuncGoesToQuest2Restore(null);
                    LocalSaveManager.Instance.SaveFuncGoesToQuest2Start(cSceneGoesParamToQuest2);
                    LocalSaveManager.Instance.SaveFuncInGameContinue(null);
                    LocalSaveManager.Instance.SaveFuncInGameReset(null);
                }
            }
        }
        //----------------------------------------
        // 通信発行
        //----------------------------------------
        ServerCommunicate();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	通信処理
	*/
    //----------------------------------------------------------------------------
    private void ServerCommunicate()
    {
        //--------------------------------
        // サーバーに送る情報を構築
        //--------------------------------
        uint unQuestID = (uint)MainMenuParam.m_QuestSelectMissionID;
        uint unQuestState = 0;
        if (MainMenuParam.m_QuestSelectAreaAmendStamina != 100) { unQuestState = (int)MasterDataDefineLabel.AmendType.STAMINA; }    // エリア補正タイプ：補正：スタミナ半減
        else if (MainMenuParam.m_QuestSelectAreaAmendEXP != 100) { unQuestState = (int)MasterDataDefineLabel.AmendType.EXP; }   // エリア補正タイプ：補正：経験値アップ
        else if (MainMenuParam.m_QuestSelectAreaAmendCoin != 100) { unQuestState = (int)MasterDataDefineLabel.AmendType.MONEY; }    // エリア補正タイプ：補正：コイン
        else if (MainMenuParam.m_QuestSelectAreaAmendDrop != 100) { unQuestState = (int)MasterDataDefineLabel.AmendType.DROP; }	// エリア補正タイプ：補正：ドロップ率
        else if (MainMenuParam.m_QuestSelectAreaAmendTicket != 100) { unQuestState = (int)MasterDataDefineLabel.AmendType.TICKET; } // エリア補正タイプ：補正：チケット

        //----------------------------------------
        // 選択されている助っ人を取得
        //----------------------------------------
        //		PacketStructFriend cHelper = UserDataAdmin.Instance.SearchHelper( MainMenuParam.m_QuestHelperUserID );
        PacketStructFriend cHelper = MainMenuParam.m_QuestHelper;
        if (cHelper == null)
        {
            Debug.LogError("SelectFriendNone");
            return;
        }

        //----------------------------------------
        // フレンド使用のサイクルをもとにFP発生判定。
        // ここでも使用宣言が走るので使用情報を書き込んでおく
        //----------------------------------------
        // 指定のフレンドがフレンドポイント取得可能かどうか
        // bugweb3907対応、以前はクエストの取得可能情報も得ていたが、サーバーサイドに移動し、フレンドのみ見るように
        bool bFriendPointActive = (LocalSaveManager.Instance.GetLocalSaveUseFriend(cHelper.user_id) == null);

        //----------------------------------------
        // フレンドポイント付与情報をセーブ。
        //----------------------------------------
        uint unGetFriendPt = 0;
        if (bFriendPointActive == true)
        {
            // フレンドポイントを取得
            unGetFriendPt = MainMenuUtil.GetSelectFriendPoint(cHelper, MainMenuParam.m_QuestEventFP);
        }
        LocalSaveManager.Instance.SaveFuncQuestFriendPointActive(unGetFriendPt, UserDataAdmin.Instance.m_StructPlayer.have_friend_pt);

        //----------------------------------------
        //エリア補正リストを取得
        //----------------------------------------
        TemplateList<MasterDataAreaAmend> AreaAmendList = MainMenuParam.m_QuestAreaAmendList;
        uint[] aunAreaAmendID = new uint[AreaAmendList.m_BufferSize];

        for (int cnt = 0; cnt < AreaAmendList.m_BufferSize; cnt++)
        {
            aunAreaAmendID[cnt] = AreaAmendList[cnt].fix_id;
        }


        bool is_auto_play = false;
        MasterDataQuest2 master = MasterDataUtil.GetQuest2ParamFromID(unQuestID);
        if (master.enable_autoplay != MasterDataDefineLabel.BoolType.ENABLE)
        {
            LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
            is_auto_play = (cOption.m_OptionAutoPlayEnable == (int)LocalSaveDefine.OptionAutoPlayEnable.ON);
        }

        //--------------------------------
        // API通信リクエスト発行
        //--------------------------------
        switch (MasterDataUtil.GetQuestType(unQuestID))
        {
            case MasterDataDefineLabel.QuestType.NORMAL:
                {

                    ServerDataUtilSend.SendPacketAPI_Quest2Start(
                                                  unQuestID
                                                , unQuestState
                                                , cHelper.user_id
                                                , cHelper.unit
                                                , bFriendPointActive
                                                , UserDataAdmin.Instance.m_StructPlayer.unit_party_current
                                                , MainMenuParam.m_QuestEventFP
                                                , (MainMenuParam.m_BeginnerBoost != null) ? (int)MainMenuParam.m_BeginnerBoost.fix_id : 0
                                                , aunAreaAmendID
                                                , is_auto_play
                                                , cHelper.unit_link
                                                )
                    .setSuccessAction(_data =>
                    {
                        requestSuccessQuest2(_data);
                    })
                    .setErrorAction(_data =>
                    {
                        requestError(_data);
                    })
                    .SendStart();
                }
                break;
            case MasterDataDefineLabel.QuestType.CHALLENGE:
                {
                    ServerDataUtilSend.SendPacketAPI_ChallengeQuestStart(
                                                    unQuestID
                                                , (uint)MainMenuParam.m_ChallengeQuestLevel
                                                , MainMenuParam.m_bChallengeQuestSkip
                                                , unQuestState
                                                , cHelper.user_id
                                                , cHelper.unit
                                                , bFriendPointActive
                                                , UserDataAdmin.Instance.m_StructPlayer.unit_party_current
                                                , MainMenuParam.m_QuestEventFP
                                                , (MainMenuParam.m_BeginnerBoost != null) ? (int)MainMenuParam.m_BeginnerBoost.fix_id : 0
                                                , aunAreaAmendID
                                                , is_auto_play
                                                , cHelper.unit_link
                                                )
                    .setSuccessAction(_data =>
                    {
                        requestSuccessChallengeQuest(_data);
                    })
                    .setErrorAction(_data =>
                    {
                        requestError(_data);
                    })
                    .SendStart();
                }
                break;
            case MasterDataDefineLabel.QuestType.NONE:
                break;
            default:
                break;
        }
    }

    private void requestSuccessQuest2(ServerApi.ResultData _data)
    {

        //----------------------------------------
        // 使用したフレンド情報を記憶しておく
        //----------------------------------------
        LocalSaveManager.Instance.SaveFuncUseFriend(MainMenuParam.m_QuestHelper.user_id);

        //
        settingQuestStart(_data.GetResult<RecvQuest2Start>().result);

        //----------------------------------------
        // BGM停止処理：暫定対応
        //----------------------------------------
        SoundUtil.StopBGM(false);

        //----------------------------------------
        // サーバーに受理されたのでゲームへ移行
        //----------------------------------------
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        SceneCommon.Instance.ChangeScene(SceneType.SceneQuest2, false);
    }

    private void requestSuccessChallengeQuest(ServerApi.ResultData _data)
    {

        //----------------------------------------
        // 使用したフレンド情報を記憶しておく
        //----------------------------------------
        LocalSaveManager.Instance.SaveFuncUseFriend(MainMenuParam.m_QuestHelper.user_id);

        //
        settingQuestStart(_data.GetResult<RecvChallengeQuestStart>().result);

        //----------------------------------------
        // BGM停止処理：暫定対応
        //----------------------------------------
        SoundUtil.StopBGM(false);

        //----------------------------------------
        // サーバーに受理されたのでゲームへ移行
        //----------------------------------------
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        SceneCommon.Instance.ChangeScene(SceneType.SceneQuest2, false);
    }

    private void settingQuestStart(RecvQuest2StartValue result)
    {
        //----------------------------------------
        // スタミナ＆チケット減少反映
        //
        //----------------------------------------
        {
            UserDataAdmin.Instance.m_StructPlayer.stamina_now = result.stamina_now;
            UserDataAdmin.Instance.m_StructPlayer.stamina_recover = result.stamina_recover;
            UserDataAdmin.Instance.m_StructPlayer.have_ticket = result.ticket_now;
            UserDataAdmin.Instance.updatePlayerParam();


            // @add Developer 2015/12/02 v310 ローカル通知用に保存
            LocalNotificationUtil.m_StaminaNow = result.stamina_now;
        }

        //----------------------------------------
        // サーバーで求めたクエスト開始情報を反映
        //----------------------------------------
        SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build = new SceneGoesParamToQuest2Build();
        SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild = result.quest;
    }

    private void requestError(ServerApi.ResultData _data)
    {
        //		if( _data.m_PacketCode == API_CODE.API_CODE_WIDE_API_EVENT_ERR_FP )
        //		{
        //			//----------------------------------------
        //			// フレンドポイント関連のイベントが終了している
        //			//----------------------------------------
        //			MainMenuManager.Instance.AddSwitchRequest( MAINMENU_SEQ.SEQ_QUEST_SELECT_FRIEND , false , true );
        //			MainMenuManager.Instance.AddSwitchRequest( MAINMENU_SEQ.SEQ_QUEST_SELECT_FRIEND , false , false );
        //		}
        //----------------------------------------
        // 受けれなかったならクエスト開始情報を破棄
        //----------------------------------------
        LocalSaveManager.Instance.SaveFuncGoesToQuest2Start(null);

        //----------------------------------------
        // サーバー時間とクライアント時間のズレで再表示されないように、
        // エラー発生時には端末側の時間情報を更新することで確実にイベント終了タイミングまで進める
        //----------------------------------------
        if (ServerDataParam.m_ServerTime != 0)
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.SetupServerTime(ServerDataParam.m_ServerTime);
            }
            if (UserDataAdmin.Instance != null
            && UserDataAdmin.Instance.m_StructSystem != null
            )
            {
                UserDataAdmin.Instance.m_StructSystem.server_time = ServerDataParam.m_ServerTime;
            }
        }

        return;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	固定パーティ情報で上書き
	*/
    //----------------------------------------------------------------------------
    private void FixPartyAssign(ref UserDataUnitParam[] acUnitParam, uint unMissonID)
    {
        MasterDataQuest2 quest_param = MasterDataUtil.GetQuest2ParamFromID(unMissonID);
        if (quest_param == null)
        {
            return;
        }

        MasterDataQuestRequirement requirement_param = MasterDataUtil.GetMasterDataQuestRequirementFromID(quest_param.quest_requirement_id);
        if (requirement_param == null)
        {
            return;
        }

        //	書き換え：リーダーユニット
        FixUnitAssign(ref acUnitParam[(int)GlobalDefine.PartyCharaIndex.LEADER], GlobalDefine.PartyCharaIndex.LEADER, requirement_param);
        FixUnitAssign(ref acUnitParam[(int)GlobalDefine.PartyCharaIndex.MOB_1], GlobalDefine.PartyCharaIndex.MOB_1, requirement_param);
        FixUnitAssign(ref acUnitParam[(int)GlobalDefine.PartyCharaIndex.MOB_2], GlobalDefine.PartyCharaIndex.MOB_2, requirement_param);
        FixUnitAssign(ref acUnitParam[(int)GlobalDefine.PartyCharaIndex.MOB_3], GlobalDefine.PartyCharaIndex.MOB_3, requirement_param);
        FixUnitAssign(ref acUnitParam[(int)GlobalDefine.PartyCharaIndex.FRIEND], GlobalDefine.PartyCharaIndex.FRIEND, requirement_param);
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	固定パーティ情報で上書き：単体
	*/
    //----------------------------------------------------------------------------
    private void FixUnitAssign(ref UserDataUnitParam unit, GlobalDefine.PartyCharaIndex party_index, MasterDataQuestRequirement requirement_param)
    {
        MasterDataDefineLabel.BoolType fix_enable = MasterDataDefineLabel.BoolType.NONE;
        uint fix_unit_id;
        int fix_unit_lv;
        int fix_unit_lv_lbs;
        int fix_unit_lv_lo;
        int fix_unit_add_hp;
        int fix_unit_add_atk;
        MasterDataDefineLabel.BoolType fix_link_enable = MasterDataDefineLabel.BoolType.NONE;
        uint fix_unit_link_id;
        int fix_unit_link_lv;
        int fix_unit_link_add_hp;
        int fix_unit_link_add_atk;
        int fix_unit_link_point;
        int fix_unit_link_lv_lo;

        switch (party_index)
        {
            default:
            case GlobalDefine.PartyCharaIndex.LEADER:
                //	情報格納：リーダー
                fix_enable = requirement_param.fix_unit_00_enable;
                fix_unit_id = requirement_param.fix_unit_00_id;
                fix_unit_lv = requirement_param.fix_unit_00_lv;
                fix_unit_lv_lbs = requirement_param.fix_unit_00_lv_lbs;
                fix_unit_lv_lo = requirement_param.fix_unit_00_lv_lo;
                fix_unit_add_hp = requirement_param.fix_unit_00_plus_hp;
                fix_unit_add_atk = requirement_param.fix_unit_00_plus_atk;
                fix_link_enable = requirement_param.fix_unit_00_link_enable;
                fix_unit_link_id = requirement_param.fix_unit_00_link_id;
                fix_unit_link_lv = requirement_param.fix_unit_00_link_lv;
                fix_unit_link_add_hp = requirement_param.fix_unit_00_link_plus_hp;
                fix_unit_link_add_atk = requirement_param.fix_unit_00_link_plus_atk;
                fix_unit_link_point = requirement_param.fix_unit_00_link_point;
                fix_unit_link_lv_lo = requirement_param.fix_unit_00_link_lv_lo;
                break;
            case GlobalDefine.PartyCharaIndex.MOB_1:
                //	情報格納：サブ１
                fix_enable = requirement_param.fix_unit_01_enable;
                fix_unit_id = requirement_param.fix_unit_01_id;
                fix_unit_lv = requirement_param.fix_unit_01_lv;
                fix_unit_lv_lbs = requirement_param.fix_unit_01_lv_lbs;
                fix_unit_lv_lo = requirement_param.fix_unit_01_lv_lo;
                fix_unit_add_hp = requirement_param.fix_unit_01_plus_hp;
                fix_unit_add_atk = requirement_param.fix_unit_01_plus_atk;
                fix_link_enable = requirement_param.fix_unit_01_link_enable;
                fix_unit_link_id = requirement_param.fix_unit_01_link_id;
                fix_unit_link_lv = requirement_param.fix_unit_01_link_lv;
                fix_unit_link_add_hp = requirement_param.fix_unit_01_link_plus_hp;
                fix_unit_link_add_atk = requirement_param.fix_unit_01_link_plus_atk;
                fix_unit_link_point = requirement_param.fix_unit_01_link_point;
                fix_unit_link_lv_lo = requirement_param.fix_unit_01_link_lv_lo;
                break;
            case GlobalDefine.PartyCharaIndex.MOB_2:
                //	情報格納：サブ２
                fix_enable = requirement_param.fix_unit_02_enable;
                fix_unit_id = requirement_param.fix_unit_02_id;
                fix_unit_lv = requirement_param.fix_unit_02_lv;
                fix_unit_lv_lbs = requirement_param.fix_unit_02_lv_lbs;
                fix_unit_lv_lo = requirement_param.fix_unit_02_lv_lo;
                fix_unit_add_hp = requirement_param.fix_unit_02_plus_hp;
                fix_unit_add_atk = requirement_param.fix_unit_02_plus_atk;
                fix_link_enable = requirement_param.fix_unit_02_link_enable;
                fix_unit_link_id = requirement_param.fix_unit_02_link_id;
                fix_unit_link_lv = requirement_param.fix_unit_02_link_lv;
                fix_unit_link_add_hp = requirement_param.fix_unit_02_link_plus_hp;
                fix_unit_link_add_atk = requirement_param.fix_unit_02_link_plus_atk;
                fix_unit_link_point = requirement_param.fix_unit_02_link_point;
                fix_unit_link_lv_lo = requirement_param.fix_unit_02_link_lv_lo;
                break;
            case GlobalDefine.PartyCharaIndex.MOB_3:
                //	情報格納：サブ３
                fix_enable = requirement_param.fix_unit_03_enable;
                fix_unit_id = requirement_param.fix_unit_03_id;
                fix_unit_lv = requirement_param.fix_unit_03_lv;
                fix_unit_lv_lbs = requirement_param.fix_unit_03_lv_lbs;
                fix_unit_lv_lo = requirement_param.fix_unit_03_lv_lo;
                fix_unit_add_hp = requirement_param.fix_unit_03_plus_hp;
                fix_unit_add_atk = requirement_param.fix_unit_03_plus_atk;
                fix_link_enable = requirement_param.fix_unit_03_link_enable;
                fix_unit_link_id = requirement_param.fix_unit_03_link_id;
                fix_unit_link_lv = requirement_param.fix_unit_03_link_lv;
                fix_unit_link_add_hp = requirement_param.fix_unit_03_link_plus_hp;
                fix_unit_link_add_atk = requirement_param.fix_unit_03_link_plus_atk;
                fix_unit_link_point = requirement_param.fix_unit_03_link_point;
                fix_unit_link_lv_lo = requirement_param.fix_unit_03_link_lv_lo;
                break;
            case GlobalDefine.PartyCharaIndex.FRIEND:
                //	情報格納：フレンド
                fix_enable = requirement_param.fix_unit_04_enable;
                fix_unit_id = requirement_param.fix_unit_04_id;
                fix_unit_lv = requirement_param.fix_unit_04_lv;
                fix_unit_lv_lbs = requirement_param.fix_unit_04_lv_lbs;
                fix_unit_lv_lo = requirement_param.fix_unit_04_lv_lo;
                fix_unit_add_hp = requirement_param.fix_unit_04_plus_hp;
                fix_unit_add_atk = requirement_param.fix_unit_04_plus_atk;
                fix_link_enable = requirement_param.fix_unit_04_link_enable;
                fix_unit_link_id = requirement_param.fix_unit_04_link_id;
                fix_unit_link_lv = requirement_param.fix_unit_04_link_lv;
                fix_unit_link_add_hp = requirement_param.fix_unit_04_link_plus_hp;
                fix_unit_link_add_atk = requirement_param.fix_unit_04_link_plus_atk;
                fix_unit_link_point = requirement_param.fix_unit_04_link_point;
                fix_unit_link_lv_lo = requirement_param.fix_unit_04_link_lv_lo;
                break;
        }

        // ベースユニット強制置き換え
        switch (fix_enable)
        {
            case MasterDataDefineLabel.BoolType.ENABLE:
                {
                    // 上書き
                    if (unit == null)
                    {
                        // ベースユニットに設定がない場合
                        if (fix_unit_id == 0)
                        {
                            break;
                        }

                        unit = new UserDataUnitParam();
                    }

                    // ベースユニットに設定がある場合
                    if (fix_unit_id != 0)
                    {
                        unit.m_UnitDataID = fix_unit_id;
                        unit.m_UnitParamLevel = fix_unit_lv;
                        unit.m_UnitParamLimitBreakLV = fix_unit_lv_lbs;
                        unit.m_UnitParamLimitOverLV = fix_unit_lv_lo;
                        unit.m_UnitParamPlusHP = fix_unit_add_hp;
                        unit.m_UnitParamPlusPow = fix_unit_add_atk;
                        unit.m_UnitParamLinkPoint = fix_unit_link_point;
                    }
                    else
                    {
                        // リーダー、フレンドは空にしない
                        if (party_index != GlobalDefine.PartyCharaIndex.LEADER
                        && party_index != GlobalDefine.PartyCharaIndex.FRIEND)
                        {
                            // 空にする
                            unit = null;
                        }
                    }
                }
                break;

            case MasterDataDefineLabel.BoolType.DISABLE:
                {
                    // リーダー、フレンドは空にしない
                    if (party_index != GlobalDefine.PartyCharaIndex.LEADER
                    && party_index != GlobalDefine.PartyCharaIndex.FRIEND)
                    {
                        // 空にする
                        unit = null;
                    }
                }
                break;

            case MasterDataDefineLabel.BoolType.NONE:
                // 維持
                break;
        }

        // パーティにベースユニットが設定されていない場合
        if (unit == null)
        {
            return;
        }

        // リンクユニット強制置き換え
        switch (fix_link_enable)
        {
            case MasterDataDefineLabel.BoolType.ENABLE:
                // リンクユニットに設定がある場合
                if (fix_unit_link_id != 0)
                {
                    unit.m_UnitParamLinkID = fix_unit_link_id;
                    unit.m_UnitParamLinkLv = fix_unit_link_lv;
                    unit.m_UnitParamLinkPlusPow = fix_unit_link_add_hp;
                    unit.m_UnitParamLinkPlusHP = fix_unit_link_add_atk;
                    unit.m_UnitParamLinkPoint = fix_unit_link_point;
                    unit.m_UnitParamLinkLimitOverLV = fix_unit_link_lv_lo;
                }
                else
                {
                    unit.m_UnitParamLinkID = 0;
                    unit.m_UnitParamLinkLv = 0;
                    unit.m_UnitParamLinkPlusPow = 0;
                    unit.m_UnitParamLinkPlusHP = 0;
                    unit.m_UnitParamLinkPoint = 0;
                    unit.m_UnitParamLinkLimitOverLV = 0;
                }
                break;

            case MasterDataDefineLabel.BoolType.DISABLE:
                unit.m_UnitParamLinkID = 0;
                unit.m_UnitParamLinkLv = 0;
                unit.m_UnitParamLinkPlusPow = 0;
                unit.m_UnitParamLinkPlusHP = 0;
                unit.m_UnitParamLinkPoint = 0;
                unit.m_UnitParamLinkLimitOverLV = 0;
                break;

            case MasterDataDefineLabel.BoolType.NONE:
                break;
        }
    }

}



