/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MainMenuSeqResultServerSend.cs
	@brief	メインメニューシーケンス：リザルト枠：リザルト前サーバー反映処理
	@author Developer
	@date 	2013/02/07
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
	@brief	メインメニューシーケンス：リザルト枠：リザルト前サーバー反映処理
*/
//----------------------------------------------------------------------------
public class MainMenuResultServerSend : MainMenuSeq
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    private uint m_QuestID = 0;
    private uint m_AreaID = 0;
    private bool m_PageSwitchResultInfo = false;

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

        //----------------------------------------
        // ページ固有の表示要素インスタンスを保持
        //----------------------------------------

        //----------------------------------------
        // パッチ処理を行わないようにする.
        //----------------------------------------
        MainMenuManager.Instance.m_ResumePatchUpdateRequest = false;
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

        if (m_PageSwitchResultInfo)
        {
            if (MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_RESULT_INFO, false, false))
            {
                m_PageSwitchResultInfo = false;
            }
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

        //--------------------------------
        // ゲームからメインメニューに渡されたリザルトをサーバーに送るクラスなので、
        // ここで情報が存在しないことはあり得ない。
        //
        //--------------------------------
        if (SceneGoesParam.Instance == null
        || SceneGoesParam.Instance.m_SceneGoesParamToMainMenu == null
        )
        {
            Debug.LogError("SceneGoesParam.Instance.m_SceneGoesParamToMainMenu Instance None!");
            return;
        }

        m_QuestID = SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_QuestID;
        m_AreaID = SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_AreaID;

        //--------------------------------
        // リザルト情報を反映する前に項目を記録しておく
        //--------------------------------
#if BUILD_TYPE_DEBUG
        Debug.Log("MainMenuResultServerSend - SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_Quest2:" + SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_Quest2);
        Debug.Log("MainMenuResultServerSend - SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_PartyFriend:" + SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_PartyFriend);
        Debug.Log("MainMenuResultServerSend - UserDataAdmin.Instance.m_StructPlayer.unit_list:" + UserDataAdmin.Instance.m_StructPlayer.unit_list);
        Debug.Log("MainMenuResultServerSend - UserDataAdmin.Instance.m_StructPlayer.rank:" + UserDataAdmin.Instance.m_StructPlayer.rank);
        Debug.Log("MainMenuResultServerSend - UserDataAdmin.Instance.m_StructPlayer.exp:" + UserDataAdmin.Instance.m_StructPlayer.exp);
        Debug.Log("MainMenuResultServerSend - UserDataAdmin.Instance.m_StructPlayer.have_money:" + UserDataAdmin.Instance.m_StructPlayer.have_money);
        Debug.Log("MainMenuResultServerSend - UserDataAdmin.Instance.m_StructPlayer.have_stone:" + UserDataAdmin.Instance.m_StructPlayer.have_stone);
        Debug.Log("MainMenuResultServerSend - UserDataAdmin.Instance.m_StructPlayer.have_ticket:" + UserDataAdmin.Instance.m_StructPlayer.have_ticket);
        Debug.Log("MainMenuResultServerSend - UserDataAdmin.Instance.m_StructPlayer.have_friend_pt:" + UserDataAdmin.Instance.m_StructPlayer.have_friend_pt);
        Debug.Log("MainMenuResultServerSend - UserDataAdmin.Instance.m_StructPlayer.flag_unit_get:" + UserDataAdmin.Instance.m_StructPlayer.flag_unit_get);
        Debug.Log("MainMenuResultServerSend - UserDataAdmin.Instance.getCurrentHero():" + UserDataAdmin.Instance.getCurrentHero());
#endif

        MainMenuParam.m_ResultQuest2 = SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_Quest2;
        MainMenuParam.m_ResultPrevFriend = SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_PartyFriend;
        MainMenuParam.m_ResultPrevUnit = UserDataAdmin.Instance.m_StructPlayer.unit_list;
        MainMenuParam.m_ResultPrevRank = UserDataAdmin.Instance.m_StructPlayer.rank;
        MainMenuParam.m_ResultPrevExp = UserDataAdmin.Instance.m_StructPlayer.exp;
        MainMenuParam.m_ResultPrevMoney = UserDataAdmin.Instance.m_StructPlayer.have_money;
        MainMenuParam.m_ResultPrevStone = UserDataAdmin.Instance.m_StructPlayer.have_stone;
        MainMenuParam.m_ResultPrevTicket = UserDataAdmin.Instance.m_StructPlayer.have_ticket;
        MainMenuParam.m_ResultPrevFriendPoint = UserDataAdmin.Instance.m_StructPlayer.have_friend_pt;
        MainMenuParam.m_ResultPrevUnitGetFlag = UserDataAdmin.Instance.m_StructPlayer.flag_unit_get;
        MainMenuParam.m_ResultPrevHero = UserDataAdmin.Instance.getCurrentHero();

        MainMenuParam.m_ResultNextArea = SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_NextAreaCleard;

        if (TutorialManager.IsExists &&
             TutorialManager.PP.IsNewUser)
        {
            MainMenuParam.m_ResultPrevRank = 1;
            MainMenuParam.m_ResultPrevExp = 0;
        }


        {
            SendEnemyKill[] enemy_kill_list = null;
            bool no_damage = false;
            int max_damage = 0;
            SendSkillExecCount[] active_skill_execute_count = null;
            int hero_skill_execute_count = 0;
            ServerDataDefine.PlayScoreInfo play_score_info = new PlayScoreInfo();
            bool is_auto_play = false;

            if (!TutorialManager.IsExists)
            {
                SceneGoesParamToMainMenu _data = SceneGoesParam.Instance.m_SceneGoesParamToMainMenu;

                //プレイ情報を設定
                no_damage = _data.m_NoDamagePlayer;
                max_damage = _data.m_MaxDamageToEnemy;
                hero_skill_execute_count = _data.m_HeroSkillUseCount;

                enemy_kill_list = new SendEnemyKill[_data.m_EnemyKillCountList.Length];
                for (int i = 0; i < _data.m_EnemyKillCountList.Length; i++)
                {
                    enemy_kill_list[i] = new SendEnemyKill();
                    enemy_kill_list[i].enemy_id = (uint)_data.m_EnemyKillCountList[i].m_EnemyFixID;
                    enemy_kill_list[i].value = (uint)_data.m_EnemyKillCountList[i].m_KillCount;
                }

                active_skill_execute_count = new SendSkillExecCount[_data.m_LimitBreakUseList.Length];
                for (int i = 0; i < _data.m_LimitBreakUseList.Length; i++)
                {
                    active_skill_execute_count[i] = new SendSkillExecCount();
                    active_skill_execute_count[i].skill_id = (uint)_data.m_LimitBreakUseList[i].m_LimitBreakSkillID;
                    active_skill_execute_count[i].value = (uint)_data.m_LimitBreakUseList[i].m_UseCount;
                }

                //スコア情報を設定
                play_score_info = _data.m_PlayScoreInfo;

                is_auto_play = _data.m_IsUsedAutoPlay;
            }

            //--------------------------------
            // API通信リクエスト発行
            //--------------------------------
            switch (MasterDataUtil.GetQuestType(m_QuestID))
            {
                case MasterDataDefineLabel.QuestType.NORMAL:
                    {
                        //--------------------------------
                        // 新クエスト用のリザルトAPIを発行
                        //--------------------------------
                        ServerDataUtilSend.SendPacketAPI_Quest2Clear(
                                            m_QuestID,
                                            enemy_kill_list,
                                            no_damage,
                                            max_damage,
                                            active_skill_execute_count,
                                            hero_skill_execute_count,
                                            play_score_info,
                                            is_auto_play
                                        )
                        .setSuccessAction(_data =>
                        {
                            quest2ClearSuccess(_data);
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
                        //--------------------------------
                        // 成長ボスクエスト用のリザルトAPIを発行
                        //--------------------------------
                        ServerDataUtilSend.SendPacketAPI_ChallengeQuestClear(
                                            m_QuestID,
                                            enemy_kill_list,
                                            no_damage,
                                            max_damage,
                                            active_skill_execute_count,
                                            hero_skill_execute_count,
                                            play_score_info,
                                            is_auto_play
                                        )
                        .setSuccessAction(_data =>
                        {
                            challengeQuestClearSuccess(_data);
                        })
                        .setErrorAction(_data =>
                        {
                            requestError(_data);
                        })
                        .SendStart();
                    }
                    break;
            }
        }
        m_PageSwitchResultInfo = false;

        //--------------------------------
        // 一度発行したらもう不要なので削除
        //--------------------------------
        SceneGoesParam.Instance.m_SceneGoesParamToMainMenu = null;

    }

    private void quest2ClearSuccess(ServerApi.ResultData _data)
    {
        RecvQuest2Clear quest2Clear = _data.GetResult<RecvQuest2Clear>();
        //--------------------------------
        // 通常クエストなら通常クエスト用のリザルトAPI解析
        // DG0-2733 Tutorial時、StructPlayer.renew_tutorial_step == 217 に更新される
        //--------------------------------
        UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvQuest2Clear>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
        UserDataAdmin.Instance.m_StructHeroList = quest2Clear.result.hero_list;
        UserDataAdmin.Instance.ConvertPartyAssing();

        settingResultData(quest2Clear.result);

        m_PageSwitchResultInfo = true;
    }

    private void challengeQuestClearSuccess(ServerApi.ResultData _data)
    {
        //--------------------------------
        // 成長ボスクエスト用のリザルトAPI解析
        //--------------------------------
        RecvChallengeQuestClear challengeQquestClear = _data.GetResult<RecvChallengeQuestClear>();

        UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvChallengeQuestClear>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
        UserDataAdmin.Instance.m_StructHeroList = challengeQquestClear.result.hero_list;
        UserDataAdmin.Instance.ConvertPartyAssing();

        settingResultData(challengeQquestClear.result);

        //成長ボスクエストのリザルトデータ
        MainMenuParam.m_ResultChallenge = challengeQquestClear.result.result_challenge;

        m_PageSwitchResultInfo = true;
    }

    /// <summary>
    /// リザルト演出用データ設定
    /// </summary>
    /// <param name="result"></param>
    private void settingResultData(RecvQuest2ClearValue result)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("MainMenuResultServerSend - quest2Clear.result.get_exp:" + result.get_exp);
        Debug.Log("MainMenuResultServerSend - quest2Clear.result.get_money:" + result.get_money);
        Debug.Log("MainMenuResultServerSend - quest2Clear.result.get_unit:" + result.get_unit);
        Debug.Log("MainMenuResultServerSend - quest2Clear.result.get_stone:" + result.get_stone);
        Debug.Log("MainMenuResultServerSend - quest2Clear.result.get_ticket:" + result.get_ticket);
        Debug.Log("MainMenuResultServerSend - m_QuestID:" + m_QuestID);
        Debug.Log("MainMenuResultServerSend - quest2Clear.result.get_clear_unit:" + result.get_clear_unit);
        Debug.Log("MainMenuResultServerSend - quest2Clear.result.get_clear_item:" + result.get_clear_item);
        Debug.Log("MainMenuResultServerSend - quest2Clear.result.get_clear_key:" + result.get_clear_key);
        Debug.Log("MainMenuResultServerSend - quest2Clear.result.get_bonus:" + result.get_bonus);
#endif

        MainMenuParam.m_ResultParamActive = 1;
        MainMenuParam.m_ResultExp = result.get_exp;
        MainMenuParam.m_ResultMoney = result.get_money;
        MainMenuParam.m_ResultUnit = result.get_unit;
        MainMenuParam.m_ResultStone = result.get_stone;
        MainMenuParam.m_ResultTicket = result.get_ticket;
        MainMenuParam.m_ResultFriendPt = result.get_friend_pt;
        MainMenuParam.m_ResultQuestID = m_QuestID;
        MainMenuParam.m_ResultClearUnit = result.get_clear_unit;
        MainMenuParam.m_ResultClearItem = result.get_clear_item;
        MainMenuParam.m_ResultClearQuestKey = result.get_clear_key;
        MainMenuParam.m_ResultFloorBonus = result.get_bonus;
        MainMenuParam.m_ResultRewardLimit = result.reward_limit_list;
        MainMenuParam.m_ResultScores = result.result_scores;
        MainMenuParam.m_ResultChallenge = null;
        MainMenuParam.m_ResultAreaID = m_AreaID;

        if (TutorialManager.IsExists)
        {
            //チュートリアルの取得ユニットなどはリザルトで帰ってこないのでここで生成
            MainMenuParam.m_ResultUnit = new List<long> { UserDataAdmin.Instance.SearchCharaByUnitId(TutorialManager.MaterialUnitCharaId).unique_id }.ToArray();

            if (TutorialManager.PP.IsNewUser)
            {
                //新規ユーザーの取得経験値を設定
                MainMenuParam.m_ResultExp = UserDataAdmin.Instance.m_StructPlayer.exp;
            }
        }
        //--------------------------------
        // ログインAPIが先に走ることでフレンドポイントが適用されないらしいので、
        // ここで上書きすることで補正をかけている
        //--------------------------------
        if (result.player != null)
        {
            MainMenuParam.m_LoginFriendPointNow = result.player.have_friend_pt;
        }
        //--------------------------------
        // ローカルセーブにあるリザルト情報を破棄
        //
        // ここより前で破棄するとデータ消失の恐れがある。
        // ここで破棄すると通信中に意図的にアプリ終了させるとリザルトが重複して送れる可能性があるが、
        // サーバー側で「クエスト受理中クリア前」のフラグを持っているのでそのまま重複して送ってもエラー出して抜けられるはず。
        //--------------------------------
        LocalSaveManager.Instance.SaveFuncGoesToMenuResult(null);
        LocalSaveManager.Instance.SaveFuncGoesToMenuRetire(null);

    }

    private void requestError(ServerApi.ResultData _data)
    {
        if (_data.m_PacketCode == API_CODE.API_CODE_WIDE_ERROR_MENTENANCE) return;
        //--------------------------------
        // ローカルセーブにあるリザルト情報を破棄
        //
        // ここより前で破棄するとデータ消失の恐れがある。
        // ここで破棄すると通信中に意図的にアプリ終了させるとリザルトが重複して送れる可能性があるが、
        // サーバー側で「クエスト受理中クリア前」のフラグを持っているのでそのまま重複して送ってもエラー出して抜けられるはず。
        //--------------------------------
        LocalSaveManager.Instance.SaveFuncGoesToMenuResult(null);
        LocalSaveManager.Instance.SaveFuncGoesToMenuRetire(null);
    }
}

