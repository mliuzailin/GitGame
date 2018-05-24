using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;
using DG.Tweening;

/// <summary>
///
/// </summary>
public class MainMenuResultInfo : MainMenuSeq
{
    static readonly float WAIT_INTERVAL_SECOND = 0.1f;

    private const int QUEST_STATUS_COIN = 0;
    private const int QUEST_STATUS_TICKET = 1;
    private const int QUEST_STATUS_EXP = 2;
    private const int QUEST_STATUS_LEVEL_UP_CHECK = 3;
    private const int QUEST_STATUS_GRADE_UP_CHECK = 4;
    private const int QUEST_STATUS_GRADE_UP_WAIT = 5;
    private const int QUEST_STATUS_END = 6;

    private const int SCORE_BASE = 0;
    private const int SCORE_QUEST_BONUS = 1;
    private const int SCORE_SPECIAL_BONUS = 2;
    private const int SCORE_PENALTY = 3;
    private const int SCORE_TOTAL = 4;
    private const int SCORE_HI_SCORE_CHECK = 5;
    private const int SCORE_WAIT = 6;
    private const int SCORE_MISSION_CLEAR = 7;
    private const int SCORE_END = 8;

    enum ResultStatus
    {
        None = 0,               //< 開始前
        QuestStatus,            //< クエスト情報
        Drops,                  //< 取得情報
        MissionClear,           //< クリアミッション情報
        ChallengeReward,        //< 成長ボス報酬
        Score,                  //< スコア情報
        FriendInfo,             //< フレンド情報
        ParamCheck,             //< パラメータチェック
        WebView,
    }

    private QuestResult m_QuestResult = null;
    private MasterDataArea areaMaster;
    private ResultStatus m_ResultStatus = ResultStatus.None;
    private int m_AnimUnitCount = 0;
    private int m_AnimBonusCount = 0;
    private int m_Status = 0;
    private Tweener m_Tweener = null;
    private int m_UseExp = 0;
    private int m_StartRank = 0;
    private int m_StartExp = 0;
    private WaitTimer m_WaitTimer = null;

    private PacketStructResultScore m_ResultScore = null;

    private MasterDataQuest2 m_QuestMaster = null;
    private MasterDataArea m_AreaMaster = null;

    private GradeUpEffect m_gradeUpEffect = null;
    private HiScoreEffect m_HiScoreEffect = null;

    private QuestResultStepModel m_step = null;
    private List<DropIconListItemModel> m_obainedUnits = new List<DropIconListItemModel>();
    private List<DropIconListItemModel> m_FloorItemModels = new List<DropIconListItemModel>();

    private WebView m_WebView = null;
    private bool m_nextSe = true;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }


    /// <summary>
    ///
    /// </summary>
	public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }

        bool bUpdateEnd = false;
        switch (m_ResultStatus)
        {
            case ResultStatus.QuestStatus: bUpdateEnd = UpdateQuestStatus(); break;
            case ResultStatus.Drops: bUpdateEnd = UpdateDrops(); break;
            case ResultStatus.MissionClear: bUpdateEnd = UpdateMissionClear(); break;
            case ResultStatus.ChallengeReward: bUpdateEnd = UpdateChallengeReward(); break;
            case ResultStatus.Score: bUpdateEnd = UpdateScore(); break;
            default:
                break;
        }

        if (bUpdateEnd)
        {
            m_step.End();
        }

        if (m_WaitTimer != null)
        {
            m_WaitTimer.Tick(Time.deltaTime);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="bActive"></param>
    /// <param name="bBack"></param>
    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        if (m_QuestResult == null)
        {
            m_QuestResult = m_CanvasObj.GetComponentInChildren<QuestResult>();
        }

        //---------------------
        // 遊んだクエストのマスターデータを取得し、
        // クエスト名称欄に名前を設定
        //---------------------
        {
            m_QuestMaster = MasterDataUtil.GetQuest2ParamFromID(MainMenuParam.m_ResultQuestID);
            if (m_QuestMaster != null)
            {
                m_AreaMaster = MasterDataUtil.GetAreaParamFromID(m_QuestMaster.area_id);
            }

            if (m_QuestMaster != null)
            {
                m_QuestResult.QuestNameText = m_QuestMaster.quest_name;
            }
            if (m_AreaMaster != null)
            {
                m_QuestResult.m_AreaMaster = m_AreaMaster;
                m_QuestResult.AreaNameText = m_AreaMaster.area_name;
            }
#if false//未使用
            uint unQuestSerialNum = 0;
            uint unQuestSerialTotal = 0;
            if (m_QuestMaster != null
            && m_AreaMaster != null
            && MasterDataUtil.GetQuestSerialNumber(m_QuestMaster.fix_id, m_AreaMaster.fix_id, ref unQuestSerialNum, ref unQuestSerialTotal)
            )
            {
                unQuestSerialNum += 1; // ※表示上は１からの連番にしておく
            }
#endif
        }

        m_QuestResult.SetTopAndBottomAjustStatusBar(Vector2.zero);
        Initialize();
        AndroidBackKeyManager.Instance.EnableBackKey();
        AndroidBackKeyManager.Instance.StackPush(gameObject, OnSelectBackKey);
        m_nextSe = true;
    }

    private void OnSelectBackKey()
    {
    }

    public override bool PageSwitchEventDisableBefore()
    {
        base.PageSwitchEventDisableBefore();
        AndroidBackKeyManager.Instance.StackPop(gameObject);
        return false;
    }

    private void Initialize()
    {
        InitQuestStatus();
        InitStep();

        m_obainedUnits.Clear();
        m_FloorItemModels.Clear();

        m_ResultStatus = ResultStatus.None;
    }

    private void InitStep()
    {
        if (m_step != null)
            return;

        QuestResultStepView view = null;

        m_step = new QuestResultStepModel();
        m_step.OnStepStarted += () =>
        {
            InputLayer.Instance.OnAnyTouchBeganCallbackOnce = (Vector3 position) =>
            {
                SkipStep();
            };
        };
        m_step.OnStepEnded += () =>
        {
            InputLayer.Instance.CancelAnyTouchCallback();

            if (view != null)
                return;

            view = QuestResultStepView.Attach(m_QuestResult.GetSetpViewRoot(), m_step);
        };
        m_step.OnStepChanged += () =>
        {
            nextResultStatus();

            if (AllStatesFinished())
                m_step = null;
            else
                m_step.Start();

            if (view == null)
                return;

            if (m_nextSe == true)
            {
                SoundUtil.PlaySE(SEID.SE_MENU_OK);
            }

            view.Detach();
            view = null;
            m_nextSe = true;
        };
    }

    private void SkipStep()
    {
        m_WaitTimer = null;

        if (m_step.IsProgressing == false)
        {
            return;
        }

        // TODO : 整理
        switch (m_ResultStatus)
        {
            case ResultStatus.QuestStatus:
#if false//こちらを有効にするとステータス演出やランクアップ演出もスキップできるようになる。
                {
                    if (m_gradeUpEffect == null)
                    {
                        if (SkipQuestStatus() == false)
                        {
                            m_WaitTimer = new WaitTimer(WAIT_INTERVAL_SECOND, SkipStep);
                            return;
                        }
                        m_Status = QUEST_STATUS_END;
                    }
                    else
                    {
                        m_gradeUpEffect.SkipShowAnimation();
                        m_Status = QUEST_STATUS_END;
                        return;
                    }
                }
                break;
#else
                return;
#endif
            case ResultStatus.Drops:
                //アイコンの準備完了待ち
                if (isDropIconAllReady() == false)
                {
                    m_WaitTimer = new WaitTimer(WAIT_INTERVAL_SECOND, SkipStep);
                    return;
                }
                if (!m_QuestResult.DropIcons.IsNullOrEmpty())
                {
                    // Newフラグのチェック
                    for (int i = 0; i < m_QuestResult.DropIcons.Count; i++)
                    {
                        m_QuestResult.DropIcons[i].CheckViewFlag();
                    }
                }
                foreach (var icon in m_obainedUnits)
                    icon.SkipAppearing();
                foreach (var icon in m_FloorItemModels)
                    icon.SkipAppearing();
                m_QuestResult.LoopUnits();
                m_QuestResult.LoopFloorBonuses();
                break;
            case ResultStatus.MissionClear:
                m_QuestResult.SkipShowMission();
                break;
            case ResultStatus.Score:
                SkipScore();
                break;
            case ResultStatus.ChallengeReward:
                break;
            default:
                return;
        }

        m_Status = int.MaxValue;
        m_step.End();
    }

    private bool isDropIconAllReady()
    {
        for (int i = 0; i < m_QuestResult.DropIcons.Count; i++)
        {
            if (m_QuestResult.DropIcons[i].m_bReady == false) return false;
        }
        for (int i = 0; i < m_QuestResult.FloorBonus.Count; i++)
        {
            if (m_QuestResult.FloorBonus[i].m_bReady == false) return false;
        }
        return true;
    }

    // ===============================================
    // TODO : QuestResultStepModelにまとめる
    private void TickState()
    {
        m_ResultStatus = (ResultStatus)((int)m_ResultStatus + 1);
    }
    private bool AllStatesFinished()
    {
        return (int)m_ResultStatus >= (int)ResultStatus.WebView;
    }
    private void StartState()
    {
        m_step.Next();
    }
    // ===============================================

    /// <summary>
    /// パラメータ上限チェック
    /// </summary>
    /// <returns></returns>
	private bool ParamCheck()
    {
        //----------------------------------------
        // パラメータリミットチェック
        //----------------------------------------
        //チェック対象：コイン、チケット
        PRM_LIMIT_ERROR_TYPE ret = MainMenuUtil.ChkPrmLimit(MainMenuParam.m_ResultMoney, MainMenuParam.m_ResultTicket, 0, 0, 0);
        MainMenuUtil.ShowParamLimitDialog(ret, DialogType.DialogOK);
        return false;
    }

    private bool willMoveNextPage;

    /// <summary>
    /// 次のページへ
    /// </summary>
	private void moveNextPage()
    {
        willMoveNextPage = true;

        if (TutorialManager.IsExists)
        {
            TutorialFSM.Instance.SendEvent_FinishQuestResult();

        }
        else if (MainMenuManager.s_LastLoginTime != 0)
        {
            //初心者ブーストが変更されたかもしれないので再設定
            MainMenuParam.m_BeginnerBoost = MasterDataUtil.GetMasterDataBeginnerBoost();
            UserDataAdmin.Instance.Player.isUpdateItem = true;
            switch (MasterDataUtil.GetQuestType(MainMenuParam.m_ResultQuestID))
            {
                case MasterDataDefineLabel.QuestType.NORMAL:
                    {
                        uint nextArea = MainMenuUtil.ChkActiveNextArea(MainMenuParam.m_ResultAreaID);
                        if (MainMenuParam.m_ResultNextArea == 0
                        && nextArea != 0)
                        {// 新しいエピソードが初めて解放された時
                            if (MainMenuUtil.ChkActiveStory(m_AreaMaster.fix_id) == true)
                            {// クリアしたエピソードの最後がストーリーの時
                                MainMenuParam.SetQuestSelectParam(m_AreaMaster.area_cate_id, m_AreaMaster.fix_id);
                            }
                            else
                            {
                                MainMenuParam.SetQuestSelectParam(m_AreaMaster.area_cate_id, nextArea);
                            }
                        }
                        else
                        {
                            MainMenuParam.SetQuestSelectParam(m_AreaMaster.area_cate_id, m_AreaMaster.fix_id);
                        }
                        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT, false, false, true);
                    }
                    break;
                case MasterDataDefineLabel.QuestType.CHALLENGE:
                    {
                        //成長ボス選択に戻る
                        MainMenuParam.SetChallengeSelectParam(MainMenuParam.m_ResultQuestID);
                        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_CHALLENGE_SELECT, false, false);
                    }
                    break;
                default:
                    {
                        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false);
                    }
                    break;

            }
        }
        else
        {
            //クエスト中断から開始してログインパックを取得していないときは
            //ログインパック取得遷移へ
            MainMenuParam.m_DateChangeType = DATE_CHANGE_TYPE.LOGIN;
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_DATE_CHANGE, false, false);
        }

        //---------------------
        // メインメニューのユニットに関する実績解除
        //---------------------
        MainMenuAchievement.GameCenterUnlockUnit();

        //---------------------
        // メインメニューのクエストに関する実績解除
        //---------------------
        MainMenuAchievement.GameCenterUnlockQuest(MainMenuParam.m_ResultQuestID);

        //---------------------
        // メインメニューのクエストに関する実績解除
        //---------------------
        MainMenuAchievement.GameCenterUnlockQuestRanking();
    }

    /// <summary>
    /// 次のシーケンス設定
    /// </summary>
	private void nextResultStatus()
    {
        if (AllStatesFinished())
        {
            return;
        }

        TickState();

        m_QuestResult.IsEnableButton = false;

        switch (m_ResultStatus)
        {
            case ResultStatus.QuestStatus:
                break;
            case ResultStatus.Drops:
                m_QuestResult.IsMasterGradeUp = false;
                InitDrops();
                break;
            case ResultStatus.MissionClear:
                if (InitMissionClear())
                {
                    m_step.ForceNext();
                }
                else
                {
                    ExitDrops();
                }
                break;
            case ResultStatus.ChallengeReward:
                if (InitChallengeReward())
                {
                    m_step.ForceNext();
                }
                else
                {
                    ExitDrops();
                    m_QuestResult.IsMissionClear = false;
                }
                break;
            case ResultStatus.Score:
                if (InitScore())
                {
                    m_step.ForceNext();
                }
                else
                {
                    ExitDrops();
                    m_QuestResult.IsMissionClear = false;
                    m_QuestResult.IsChallengeReward = false;
                }
                break;
            case ResultStatus.FriendInfo:
                if (TutorialManager.IsExists)
                {
                    m_step.ForceNext();
                }
                else
                {
                    if (InitFriendInfo())
                    {
                        m_step.ForceNext();
                    }
                }
                break;
            case ResultStatus.ParamCheck:
                ParamCheck();
                m_step.ForceNext();
                break;
            case ResultStatus.WebView:
                PopupWebView();
                break;
        }
    }

    /// <summary>
    /// クエスト情報初期化
    /// </summary>
	private void InitQuestStatus()
    {
        m_QuestResult.IsClearStatus = true;

        //---------------------
        // 各種パラメータ
        //---------------------
        m_QuestResult.Coin = 0;
        m_QuestResult.Ticket = 0;
        m_QuestResult.Exp = 0;

        //----------------------------------------
        // 次のランクまでの経験値ゲージの見た目を初期化
        // 現在のランクと次のランクのマスターデータを取得
        //----------------------------------------
        m_StartRank = (int)MainMenuParam.m_ResultPrevRank;
        m_StartExp = (int)MainMenuParam.m_ResultPrevExp;
        calcNextExp(true);
        m_QuestResult.IsRankUp = false;
        m_QuestResult.RankUpMessage = "";
        m_QuestResult.Exp = 0;
        m_Status = QUEST_STATUS_COIN;

        //クリアミッションフラグ
        m_QuestResult.IsMissionFlag = false;
        PacketAchievement[] clearList = ResidentParam.GetQuestAchivementClearList(MainMenuParam.m_ResultQuestID);
        if (clearList != null && clearList.Length > 0) m_QuestResult.IsMissionFlag = true;

        m_QuestResult.Show(() =>
        {
            StartState();
        });
    }

    /// <summary>
    /// クエスト情報更新
    /// </summary>
    /// <returns></returns>
    private bool UpdateQuestStatus()
    {
        switch (m_Status)
        {
            case QUEST_STATUS_COIN:
                {
                    m_Tweener = DOTween.To(() => m_QuestResult.Coin, coin => m_QuestResult.Coin = coin, (int)MainMenuParam.m_ResultMoney, 0.3f);
                    m_Status++;
                }
                break;
            case QUEST_STATUS_TICKET:
                {
                    if (m_Tweener != null && !m_Tweener.IsPlaying())
                    {
                        m_Tweener.Kill();
                        m_Tweener = null;

                        m_Tweener = DOTween.To(() => m_QuestResult.Ticket, ticket => m_QuestResult.Ticket = ticket, (int)MainMenuParam.m_ResultTicket, 0.3f);
                        m_Status++;
                    }
                }
                break;
            case QUEST_STATUS_EXP:
                {
                    if (m_Tweener != null && !m_Tweener.IsPlaying())
                    {
                        m_Tweener.Kill();
                        m_Tweener = null;

                        m_Tweener = DOTween.To(() => m_QuestResult.Exp, exp => m_QuestResult.Exp = exp, (int)MainMenuParam.m_ResultExp, 0.3f);
                        m_Status++;

                        SoundUtil.PlaySE(SEID.SE_MM_B01_EXP_GAUGE);
                    }
                }
                break;
            case QUEST_STATUS_LEVEL_UP_CHECK:
                {
                    //
                    updateNextExp();

                    if (m_Tweener != null && !m_Tweener.IsPlaying())
                    {
                        m_Tweener.Kill();
                        m_Tweener = null;
                        m_Status++;
                        makeRankUpMessage();
                    }
                }
                break;
            case QUEST_STATUS_GRADE_UP_CHECK:
                {
                    if (InitMasterGradeUp())
                    {
                        m_Status = QUEST_STATUS_END;
                    }
                    else
                    {
                        m_Status++;
                    }
                }
                break;
            case QUEST_STATUS_GRADE_UP_WAIT:
                {
                    if (UpdateMasterGradeUp())
                    {
                        m_Status++;
                    }
                }
                break;
            case QUEST_STATUS_END:
                return true;
        }
        return false;
    }

    private bool SkipQuestStatus()
    {
        if (m_Tweener != null)
        {
            m_Tweener.Complete();
            m_Tweener.Kill();
            m_Tweener = null;
        }

        PacketStructPlayer player = UserDataAdmin.Instance.m_StructPlayer;

        //リザルト
        m_QuestResult.Coin = (int)MainMenuParam.m_ResultMoney;
        m_QuestResult.Ticket = (int)MainMenuParam.m_ResultTicket;
        m_QuestResult.Exp = (int)MainMenuParam.m_ResultExp;

        //RankUp
        if (m_StartRank != player.rank &&
            m_QuestResult.IsRankUp == false)
        {
            m_QuestResult.IsRankUp = true;
        }
        makeRankUpMessage();

        //NextExp
        MasterDataUserRank cUserRankNext = MasterFinder<MasterDataUserRank>.Instance.Find((int)player.rank + 1);
        if (cUserRankNext != null)
        {
            float fEXPFilledSpriteRate = 1.0f - ((float)(cUserRankNext.exp_next_total - player.exp)
                                                / (float)(cUserRankNext.exp_next));
            m_QuestResult.NextRankPercent = fEXPFilledSpriteRate;
            m_QuestResult.NextRank = (int)(cUserRankNext.exp_next_total - player.exp);
        }

        return InitMasterGradeUp();
    }

    /// <summary>
    /// ネクスト経験値更新
    /// </summary>
    private void updateNextExp()
    {
        int addExp = (m_QuestResult.Exp - m_UseExp);
        m_StartExp += addExp;
        calcNextExp(false);
        m_UseExp = m_QuestResult.Exp;
    }

    /// <summary>
    /// ネクスト経験値計算
    /// </summary>
    /// <param name="bClearUseExp"></param>
    private void calcNextExp(bool bClearUseExp)
    {
        MasterDataUserRank cUserRankNext = MasterFinder<MasterDataUserRank>.Instance.Find(m_StartRank + 1);
        if (cUserRankNext != null
        )
        {
            if (m_StartExp >= cUserRankNext.exp_next_total)
            {
                m_StartRank++;
                if (m_QuestResult.IsRankUp == false)
                {
                    m_QuestResult.IsRankUp = true;
                }
                calcNextExp(false);
            }
            else
            {
                float fEXPFilledSpriteRate = 1.0f - ((float)(cUserRankNext.exp_next_total - m_StartExp)
                                                    / (float)(cUserRankNext.exp_next)
                                                    );
                m_QuestResult.NextRankPercent = fEXPFilledSpriteRate;
                m_QuestResult.NextRank = (int)(cUserRankNext.exp_next_total - m_StartExp);
                if (bClearUseExp) m_UseExp = 0;

            }
        }
        else
        {
            m_QuestResult.NextRankPercent = 1.0f;
            m_QuestResult.NextRank = 0;
        }
    }

    private void makeRankUpMessage()
    {
        if (MainMenuParam.m_ResultPrevRank == UserDataAdmin.Instance.m_StructPlayer.rank)
        {
            return;
        }

        MasterDataUserRank cPreRank = MasterFinder<MasterDataUserRank>.Instance.Find((int)MainMenuParam.m_ResultPrevRank);
        MasterDataUserRank cNowRank = MasterFinder<MasterDataUserRank>.Instance.Find((int)UserDataAdmin.Instance.m_StructPlayer.rank);
        string rankUpMessage = GameTextUtil.GetText("result_text11");
        uint up_stamina = cNowRank.stamina - cPreRank.stamina;
        if (up_stamina != 0)
        {
            string staminaFormat = GameTextUtil.GetText("result_text13");
            rankUpMessage += "\n\r" + string.Format(staminaFormat, up_stamina, cNowRank.stamina);
        }
        uint up_friend = cNowRank.friend_max - cPreRank.friend_max;
        if (up_friend != 0)
        {
            string friendFormat = GameTextUtil.GetText("result_text14");
            rankUpMessage += "\n\r" + string.Format(friendFormat, up_friend, UserDataAdmin.Instance.m_StructPlayer.total_friend);
        }
        uint up_cost = cNowRank.party_cost - cPreRank.party_cost;
        if (up_cost != 0)
        {
            string costFormat = GameTextUtil.GetText("result_text12");
            rankUpMessage += "\n\r" + string.Format(costFormat, up_cost, cNowRank.party_cost);
        }
        m_QuestResult.RankUpMessage = rankUpMessage;

    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private bool InitScore()
    {
        bool bRet = true;

        //スコア情報判定
        if (MainMenuParam.m_ResultScores != null &&
            MainMenuParam.m_ResultScores.Length != 0)
        {
            m_ResultScore = MainMenuParam.m_ResultScores[0];

            m_QuestResult.IsScoreInfo = true;

            m_QuestResult.ScoreTitle = "獲得スコア";
            m_QuestResult.BaseScoreLabel = "基礎スコア";
            m_QuestResult.BaseScore = 0;
            m_QuestResult.QuestBonusLabel = "クエストボーナス";
            m_QuestResult.QuestBonus = 0.0f;
            m_QuestResult.SpecialBonusLabel = "特効ボーナス";
            m_QuestResult.SpecialBonus = 0.0f;
            m_QuestResult.PenaltyLabel = "ターンペナルティ";
            m_QuestResult.Penalty = 0;
            m_QuestResult.TotalScoreLabel = "獲得スコア";
            m_QuestResult.TotalScore = 0;

            m_Status = SCORE_BASE;

            bRet = false;
        }
        return bRet;
    }

    private bool UpdateScore()
    {
        switch (m_Status)
        {
            case SCORE_BASE:
                {
                    //m_Tweener = DOTween.To(() => m_QuestResult.BaseScore, score => m_QuestResult.BaseScore = score, (int)m_ResultScore.base_score, 0.2f);
                    //m_Status = SCORE_QUEST_BONUS;
                    m_Tweener = DOTween.To(() => m_QuestResult.TotalScore, total => m_QuestResult.TotalScore = total, (int)m_ResultScore.result_score, 0.2f);
                    m_Status = SCORE_HI_SCORE_CHECK;
                }
                break;
            case SCORE_QUEST_BONUS:
                {
                    if (m_Tweener != null && !m_Tweener.IsPlaying())
                    {
                        m_Tweener.Kill();
                        m_Tweener = null;

                        m_Tweener = DOTween.To(() => m_QuestResult.QuestBonus, q_bonus => m_QuestResult.QuestBonus = q_bonus, ((float)m_ResultScore.quest_amend_value / 100.0f), 0.2f);
                        //m_Status = SCORE_SPECIAL_BONUS;
                        m_Status = SCORE_PENALTY;
                    }
                }
                break;
            case SCORE_SPECIAL_BONUS:
                {
                    if (m_Tweener != null && !m_Tweener.IsPlaying())
                    {
                        m_Tweener.Kill();
                        m_Tweener = null;

                        m_Tweener = DOTween.To(() => m_QuestResult.SpecialBonus, s_bonus => m_QuestResult.SpecialBonus = s_bonus, ((float)m_ResultScore.unit_special_value / 100.0f), 0.2f);
                        m_Status = SCORE_PENALTY;
                    }
                }
                break;
            case SCORE_PENALTY:
                {
                    if (m_Tweener != null && !m_Tweener.IsPlaying())
                    {
                        m_Tweener.Kill();
                        m_Tweener = null;

                        m_Tweener = DOTween.To(() => m_QuestResult.Penalty, penalty => m_QuestResult.Penalty = penalty, (int)m_ResultScore.penalty, 0.2f);
                        m_Status = SCORE_TOTAL;
                    }
                }
                break;
            case SCORE_TOTAL:
                {
                    if (m_Tweener != null && !m_Tweener.IsPlaying())
                    {
                        m_Tweener.Kill();
                        m_Tweener = null;

                        m_Tweener = DOTween.To(() => m_QuestResult.TotalScore, total => m_QuestResult.TotalScore = total, (int)m_ResultScore.result_score, 0.2f);
                        m_Status = SCORE_HI_SCORE_CHECK;
                    }
                }
                break;
            case SCORE_HI_SCORE_CHECK:
                {
                    if (m_Tweener != null && !m_Tweener.IsPlaying())
                    {
                        m_Tweener.Kill();
                        m_Tweener = null;
#if false
                        if (m_ResultScore.hi_score)
                        {
                            m_HiScoreEffect = m_QuestResult.ShowHiScore();

                            m_Status = SCORE_WAIT;
                        }
                        else
#endif
                        {
                            m_Status = SCORE_MISSION_CLEAR;
                        }
                    }
                }
                break;
            case SCORE_WAIT:
                {
                    if (m_HiScoreEffect != null && m_HiScoreEffect.isFinished)
                    {
                        m_Status = SCORE_MISSION_CLEAR;
                    }
                }
                break;
            case SCORE_MISSION_CLEAR:
                {
                    //スコア報酬初期化
                    InitScoreReward();

                    m_Status = SCORE_END;
                }
                break;
            case SCORE_END:
                return true;
        }
        return false;
    }

    private void SkipScore()
    {
        m_QuestResult.TotalScore = (int)m_ResultScore.result_score;
        //m_QuestResult.BaseScore = (int)m_ResultScore.base_score;
        //m_QuestResult.QuestBonus = (float)m_ResultScore.quest_amend_value / 100.0f;
        //m_QuestResult.SpecialBonus = (float)m_ResultScore.unit_special_value / 100.0f;
        //m_QuestResult.Penalty = (int)m_ResultScore.penalty;

        if (m_QuestResult.IsScoreReward == false)
        {
            //スコア報酬初期化
            InitScoreReward();
        }

        m_Status = SCORE_END;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private bool InitScoreReward()
    {
        bool bRet = true;
        if (MainMenuParam.m_ResultScores != null)
        {
            m_QuestResult.ScoreRewardLabel = GameTextUtil.GetText("result_text19");
            m_QuestResult.ScoreRewardMessage = GameTextUtil.GetText("result_text20");
            m_QuestResult.ScoreRewardList.Clear();

            for (int i = 0; i < MainMenuParam.m_ResultScores.Length; i++)
            {
                if (MainMenuParam.m_ResultScores[i] == null)
                {
                    continue;
                }

                if (MainMenuParam.m_ResultScores[i].reward_list == null)
                {
                    continue;
                }
                for (int j = 0; j < MainMenuParam.m_ResultScores[i].reward_list.Length; j++)
                {
                    if (MainMenuParam.m_ResultScores[i].reward_list[j] == null)
                    {
                        continue;
                    }

                    PacketStructUserScoreReward reward = MainMenuParam.m_ResultScores[i].reward_list[j];
                    MasterDataScoreEvent scoreEvent = MasterFinder<MasterDataScoreEvent>.Instance.SelectFirstWhere("where event_id = ? ", reward.event_id);

                    ScoreRewardContext scoreReward = new ScoreRewardContext();
                    scoreReward.setup(MainMenuParam.m_ResultScores[i].reward_list[j], ScoreRewardContext.REWARD_CLEAR);
                    if (scoreEvent != null)
                    {
                        scoreReward.IsViewEventName = true;
                        scoreReward.EventName = scoreEvent.title;
                    }
                    m_QuestResult.ScoreRewardList.Add(scoreReward);
                }
            }

            if (m_QuestResult.ScoreRewardList.Count != 0)
            {
                m_QuestResult.IsScoreReward = true;
                if (m_HiScoreEffect != null)
                {
                    DestroyObject(m_HiScoreEffect.gameObject);
                    m_HiScoreEffect = null;
                }
                bRet = false;
            }
        }
        return bRet;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private bool InitChallengeReward()
    {
        bool bRet = true;
        if (MainMenuParam.m_ResultChallenge != null &&
            MainMenuParam.m_ResultChallenge.get_list != null &&
            MainMenuParam.m_ResultChallenge.get_list.Length != 0)
        {
            PacketStructResultChallenge result = MainMenuParam.m_ResultChallenge;

            m_QuestResult.ChallengeRewardLabel = GameTextUtil.GetText("growth_boss_33");
            m_QuestResult.ChallengeRewardMessage = GameTextUtil.GetText("growth_boss_34");
            m_QuestResult.ChallengeRewardList.Clear();

            for (int i = 0; i < result.get_list.Length; i++)
            {
                if (result.get_list[i] == null)
                {
                    continue;
                }

                MasterDataChallengeReward master = MasterFinder<MasterDataChallengeReward>.Instance.Find(result.get_list[i].fix_id);
                if (master == null)
                {
                    continue;
                }

                ChallengeRewardContext newData = new ChallengeRewardContext();
                newData.SetData(result.get_list[i], master);
                newData.setupGet(ChallengeRewardContext.REWARD_CLEAR);

                m_QuestResult.ChallengeRewardList.Add(newData);
            }

            if (m_QuestResult.ChallengeRewardList.Count != 0)
            {
                m_QuestResult.IsChallengeReward = true;
                bRet = false;

            }

        }
        return bRet;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private bool UpdateChallengeReward()
    {
        return true;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private bool InitDrops()
    {
        InitDropUnits();
        InitFloorBonus();

        m_QuestResult.IsDrops = true;
        m_Status = 0;
        return false;
    }

    private void ExitDrops()
    {
        m_QuestResult.IsDrops = false;
        foreach (var icon in m_obainedUnits)
            icon.Close();
        foreach (var icon in m_FloorItemModels)
            icon.Close();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private bool UpdateDrops()
    {
        switch (m_Status)
        {
            case 0:
                if (UpdateDropUnits())
                {
                    m_Status = 1;
                }
                break;
            case 1:
                if (UpdateFloorBonus())
                {
                    m_Status = 2;
                }
                break;
            case 2:
                return true;
        }
        return false;
    }

    /// <summary>
    /// 取得ユニット初期化
    /// </summary>
    /// <returns></returns>
    private bool InitDropUnits()
    {
        //---------------------
        // 取得ユニット一覧
        //---------------------
        if (MainMenuParam.m_ResultUnit != null
        && MainMenuParam.m_ResultUnit.Length > 0
        )
        {
            // 要素の有った場合
            for (int i = 0; i < MainMenuParam.m_ResultUnit.Length; i++)
            {
                int index = i;

                ServerDataDefine.PacketStructUnit getUnit = UserDataAdmin.Instance.SearchChara(MainMenuParam.m_ResultUnit[i]);
                MasterDataParamChara _charaMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)getUnit.id);

                var model = new DropIconListItemModel((uint)index);

                model.OnShowedNext += () =>
                {
                    if (index >= MainMenuParam.m_ResultUnit.Length - 1)
                        return;

                    m_QuestResult.GetUnitAt(index + 1).Appear();
                };

                model.OnAppeared += () =>
                {
                    m_AnimUnitCount++;

                    if (index >= MainMenuParam.m_ResultUnit.Length - 1)
                    {
                        m_QuestResult.LoopUnits();

                        foreach (var floorBonus in m_FloorItemModels)
                            floorBonus.Appear();
                    }
                };

                model.OnClicked += () =>
                {
                    SoundUtil.PlaySE(SEID.SE_MENU_OK2);
                    openUnitDetailInfo(getUnit);
                };


                var contex = new DropIconContex(_charaMaster, false, false, model);
                m_QuestResult.DropIcons.Add(contex);
                m_QuestResult.AddUnit(model);


                m_obainedUnits.Add(model);
            }

            m_QuestResult.IsViewDropGuid = true;
        }
        else
        {
            // 要素がないときはメッセージ表示
            m_QuestResult.IsViewDropGuid = false;
        }

        //
        m_AnimUnitCount = 0;

        return false;
    }

    /// <summary>
    /// 取得ユニット更新
    /// </summary>
    /// <returns></returns>
    private bool UpdateDropUnits()
    {
        if (m_QuestResult.DropIcons.Count == 0) return true;

        for (int i = 0; i < m_AnimUnitCount; i++)
        {
            m_QuestResult.DropIcons[i].CheckViewFlag();
        }

        return m_AnimUnitCount >= m_QuestResult.DropIcons.Count;
    }

    /// <summary>
    /// フロアボーナス初期化
    /// </summary>
    /// <returns></returns>
    private bool InitFloorBonus()
    {
        bool bRet = true;
        if (MainMenuParam.m_ResultFloorBonus != null &&
            MainMenuParam.m_ResultFloorBonus.Length > 0)
        {
            for (int i = 0; i < MainMenuParam.m_ResultFloorBonus.Length; i++)
            {
                int index = i;
                var model = new DropIconListItemModel((uint)index);
                model.OnAppeared += () =>
                {
                    m_QuestResult.FloorBonus[index].IsViewCaption = true;

                    m_AnimBonusCount++;

                    if (index >= MainMenuParam.m_ResultFloorBonus.Length - 1)
                        m_QuestResult.LoopFloorBonuses();
                };




                PacketStructFloorBonus bonus = MainMenuParam.m_ResultFloorBonus[i];
                Sprite _item = null;
                Sprite _rare = null;
                int _count = 0;
                MasterDataParamChara _masterChara = null;
                MasterDataUseItem _masterItem = null;

                model.isUnit = bonus.type_id == MasterDataDefineLabel.FloorBonusType.UNIT;

                switch (bonus.type_id)
                {
                    case MasterDataDefineLabel.FloorBonusType.ITEM:
                        {
                            _masterItem = MasterDataUtil.GetMasterDataUseItemFromID((uint)bonus.param);
                            _rare = ResourceManager.Instance.Load("floor_bonus_item_icon");
                            _count = (int)bonus.param2;
                        }
                        break;
                    case MasterDataDefineLabel.FloorBonusType.MONEY:
                        {
                            string strSpriteName = "coin_icon";

                            _item = ResourceManager.Instance.Load(strSpriteName);
                            _rare = ResourceManager.Instance.Load("floor_bonus_item_icon");
                            _count = (int)bonus.param;
                        }
                        break;
                    case MasterDataDefineLabel.FloorBonusType.UNIT:
                        {
                            PacketStructUnit getUnit = UserDataAdmin.Instance.SearchChara(bonus.param);
                            _masterChara = MasterFinder<MasterDataParamChara>.Instance.Find((int)getUnit.id);
                            _count = (int)bonus.param2;

                            model.OnClicked += () =>
                            {
                                SoundUtil.PlaySE(SEID.SE_MENU_OK2);
                                openUnitDetailInfo(getUnit);
                            };
                        }
                        break;
                    case MasterDataDefineLabel.FloorBonusType.TICKET:
                        {
                            string strSpriteName = "casino_ticket";

                            _item = ResourceManager.Instance.Load(strSpriteName);
                            _rare = ResourceManager.Instance.Load("floor_bonus_item_icon");
                            _count = (int)bonus.param;
                        }
                        break;
                    case MasterDataDefineLabel.FloorBonusType.QUEST_KEY:
                        {
                            string strSpriteName = "mm_quest_key";

                            _item = ResourceManager.Instance.Load(strSpriteName);
                            _rare = ResourceManager.Instance.Load("floor_bonus_item_icon");
                            _count = (int)bonus.param2;
                        }
                        break;
                    case MasterDataDefineLabel.FloorBonusType.FRIEND_PT:
                        {
                            string strSpriteName = "friend_point_icon";

                            _item = ResourceManager.Instance.Load(strSpriteName);
                            _rare = ResourceManager.Instance.Load("floor_bonus_item_icon");
                            _count = (int)bonus.param;
                        }
                        break;
                    case MasterDataDefineLabel.FloorBonusType.SCRATCH:
                        {
                            string strSpriteName = "mm_item_scrachcard";

                            _item = ResourceManager.Instance.Load(strSpriteName);
                            _rare = ResourceManager.Instance.Load("floor_bonus_item_icon");
                            _count = (int)bonus.param;
                        }
                        break;
                    default:
                        continue;
                }

                m_FloorItemModels.Add(model);
                m_QuestResult.AddFloorBonus(model);

                switch (bonus.type_id)
                {
                    case MasterDataDefineLabel.FloorBonusType.UNIT:
                        {
                            var contex = new DropIconContex(_masterChara, false, false, model);
                            contex.IsViewCaption = false;
                            //contex.CaptionText = bonus.floor_count;
                            contex.CountText = (_count > 1) ? _count.ToString() : "";
                            contex.IsSelectEnable = true;
                            m_QuestResult.FloorBonus.Add(contex);
                        }
                        break;
                    case MasterDataDefineLabel.FloorBonusType.ITEM:
                        {
                            var contex = new DropIconContex(_masterItem, _rare, model);
                            contex.IsViewCaption = false;
                            //contex.CaptionText = bonus.floor_count;
                            contex.CountText = (_count != 0) ? _count.ToString() : "";
                            contex.IsSelectEnable = false;
                            m_QuestResult.FloorBonus.Add(contex);
                        }
                        break;
                    default:
                        {
                            var contex = new DropIconContex(_item, _rare, model);
                            contex.IsViewCaption = false;
                            //contex.CaptionText = bonus.floor_count;
                            contex.CountText = (_count != 0) ? _count.ToString() : "";
                            contex.IsSelectEnable = false;
                            m_QuestResult.FloorBonus.Add(contex);
                        }
                        break;
                }

            }

            bRet = false;
        }
        if (!bRet)
        {
            m_AnimBonusCount = 0;
        }
        return bRet;
    }

    /// <summary>
    /// フロアボーナス更新
    /// </summary>
    /// <returns></returns>
    private bool UpdateFloorBonus()
    {
        if (m_QuestResult.FloorBonus.Count == 0) return true;

        return m_AnimBonusCount >= m_QuestResult.FloorBonus.Count;
    }

    /// <summary>
    /// ミッションクリア初期化
    /// </summary>
    /// <returns></returns>
    private bool InitMissionClear()
    {
        bool bRet = true;
        if (MainMenuParam.m_ResultQuest2)
        {
            PacketAchievement[] _clearList = ResidentParam.GetQuestAchivementClearList(MainMenuParam.m_ResultQuestID);
            if (_clearList != null && _clearList.Length != 0)
            {
                m_QuestResult.MissionList.Clear();

                if (_clearList != null && _clearList.Length != 0)
                {
                    for (int i = 0; i < _clearList.Length; ++i)
                    {
                        if (_clearList[i].present_ids.IsNullOrEmpty() == false)
                        {
                            m_QuestResult.MissionList.Add(new QuestMissionContext(_clearList[i]));
                        }
                    }
                }
                if (m_QuestResult.MissionList.Count != 0)
                {
                    m_QuestResult.IsMissionClear = true;
                    if (MainMenuParam.m_ResultRewardLimit != null)
                    {
                        Dialog dlg = Dialog.Create(DialogType.DialogOK);
                        dlg.SetDialogTextFromTextkey(DialogTextType.Title, "mm38q_title2");
                        dlg.SetDialogTextFromTextkey(DialogTextType.MainText, "mm37q_content9");
                        dlg.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                        dlg.EnableFadePanel();
                        dlg.Show();
                    }
                    bRet = false;
                }
            }
        }
        return bRet;
    }

    /// <summary>
    /// ミッションクリア更新
    /// </summary>
    /// <returns></returns>
    private bool UpdateMissionClear()
    {
        return true;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private bool InitMasterGradeUp()
    {
        bool bRet = true;
        PacketStructHero preHero = MainMenuParam.m_ResultPrevHero;
        if (preHero == null) return bRet;
        PacketStructHero nowHero = UserDataAdmin.Instance.getCurrentHero();
        if (nowHero == null) return bRet;
        MasterDataHero heroMaster = MasterFinder<MasterDataHero>.Instance.Find((int)nowHero.hero_id);
        if (heroMaster == null) return bRet;
        MasterDataHeroLevel nextHeroLevelMaster = MasterFinder<MasterDataHeroLevel>.Instance.Find((int)nowHero.level + 1);
        if (preHero.level == nowHero.level) return bRet;

        string messageFormat = GameTextUtil.GetText("result_text16");
        int nextExp = 0;
        if (nextHeroLevelMaster != null)
        {
            nextExp = nextHeroLevelMaster.exp_next_total - nowHero.exp;
        }
        m_QuestResult.GradeUpMessage = string.Format(messageFormat, heroMaster.name, nowHero.level, nextExp);
        m_QuestResult.IsMasterGradeUp = true;

        m_gradeUpEffect = m_QuestResult.ShowGradeUp();

        bRet = false;

        return bRet;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private bool UpdateMasterGradeUp()
    {
        bool bRet = true;
        if (m_gradeUpEffect == null)
        {
            return bRet;
        }

        bRet = m_gradeUpEffect.isFinished;
        return bRet;
    }

    /// <summary>
    /// フレンドダイアログ表示
    /// </summary>
    /// <returns></returns>
	private bool InitFriendInfo()
    {
        bool bRet = true;

        if (MainMenuParam.m_ResultPrevFriend != null)
        {
            PacketStructFriend friend = MainMenuParam.m_ResultPrevFriend;
            //--------------------------------
            // フレンド成立状況でフレンドページの分岐
            //--------------------------------
            bool bFriendHasBeen = false;
            switch (friend.friend_state)
            {
                case (int)FRIEND_STATE.FRIEND_STATE_SUCCESS: bFriendHasBeen = true; break;
                case (int)FRIEND_STATE.FRIEND_STATE_WAIT_ME: bFriendHasBeen = false; break;
                case (int)FRIEND_STATE.FRIEND_STATE_WAIT_HIM: bFriendHasBeen = false; break;
                case (int)FRIEND_STATE.FRIEND_STATE_UNRELATED: bFriendHasBeen = false; break;
                case (int)FRIEND_STATE.FRIEND_STATE_PREMIUM: bFriendHasBeen = false; break;
            }
            int getfp = (int)MainMenuParam.m_ResultFriendPt;

            Dialog _newDialog = null;
            _newDialog = Dialog.Create(DialogType.DialogFriend);
            _newDialog.SetFriendInfo(friend, true);
            if (bFriendHasBeen == false)
            {
                _newDialog.SetStrongYes();

                _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "kk114q_title");

                string underFormat = GameTextUtil.GetText("kk114q_content");
                //フレンドポイントが０の時
                if (getfp == 0)
                {
                    //フレンドポイントがもらえる設定かどうか
                    if (m_QuestMaster.enable_friendpoint == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        //すでにもらっているのでもらえなかった
                        underFormat = GameTextUtil.GetText("kk114q_content2");
                    }
                    else
                    {
                        //フレンドポイントもらえないクエスト
                        underFormat = GameTextUtil.GetText("kk114q_content3");
                    }
                }
                _newDialog.SetDialogObjectEnabled(DialogObjectType.UnderText, true);
                _newDialog.SetDialogText(DialogTextType.UnderText, string.Format(underFormat, getfp));
                _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
                _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
                _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
                {
                    m_nextSe = false;
                    sendFriendRequest();
                });
                _newDialog.SetDialogEvent(DialogButtonEventType.NO, () =>
                {
                    m_nextSe = false;
                    nextResultStatus();
                });
            }
            else
            {
                _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "kk115q_title");

                string underFormat = GameTextUtil.GetText("kk115q_content");
                //フレンドポイントが０の時
                if (getfp == 0)
                {
                    //フレンドポイントがもらえる設定かどうか
                    if (m_QuestMaster.enable_friendpoint == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        //すでにもらっているのでもらえなかった
                        underFormat = GameTextUtil.GetText("kk115q_content2");
                    }
                    else
                    {
                        //フレンドポイントもらえないクエスト
                        underFormat = GameTextUtil.GetText("kk115q_content3");
                    }
                }
                _newDialog.SetDialogObjectEnabled(DialogObjectType.UnderText, true);
                _newDialog.SetDialogText(DialogTextType.UnderText, string.Format(underFormat, getfp));

                _newDialog.SetDialogObjectEnabled(DialogObjectType.OneButton, true);
                _newDialog.SetDialogObjectEnabled(DialogObjectType.TwoButton, false);
                _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                _newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
                {
                    m_nextSe = false;
                    nextResultStatus();
                });

            }
            _newDialog.EnableFadePanel();
            _newDialog.DisableCancelButton();
            _newDialog.Show();
            bRet = false;
        }

        return bRet;
    }

    /// <summary>
    /// フレンドリクエスト通信
    /// </summary>
	private void sendFriendRequest()
    {
        uint[] useridArray = new uint[1];
        useridArray[0] = MainMenuParam.m_ResultPrevFriend.user_id;
        ServerDataUtilSend.SendPacketAPI_FriendRequest(useridArray)
        .setSuccessAction(_data =>
        {
            UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(_data.GetResult<RecvFriendRequest>().result.friend);
            nextResultStatus();
        })
        .setErrorAction(_data =>
        {
            MainMenuUtil.openFriendRequestErrorDialog(_data.m_PacketCode, nextResultStatus);
        })
        .SendStart();

    }


    /// <summary>
    /// ユニット詳細画面（お気に入りあり）
    /// </summary>
    /// <param name="_unit"></param>
    private void openUnitDetailInfo(PacketStructUnit baseUnit)
    {
        if (MainMenuManager.HasInstance)
        {
            bool closeActive = false;
            var unitDetailView = MainMenuManager.Instance.OpenUnitDetailInfoPlayer(baseUnit, null, closeActive);

            if (unitDetailView == null)
                return;

            unitDetailView.SetCloseAction(() =>
            {
                m_QuestResult.LoopUnits();
                m_QuestResult.LoopFloorBonuses();
            });
        }
    }

    /// <summary>
    /// パラメータ上限チェック
    /// </summary>
    /// <returns></returns>
    private void PopupWebView()
    {
        var corutine = MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.QuestClear,
                                                                MainMenuParam.m_ResultQuestID,
                                                                PopupWebViewCallback,
                                                                PopupWebViewCloseAction);
        StartCoroutine(corutine);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    void PopupWebViewCloseAction()
    {
        PacketAchievement[] _clearList = ResidentParam.GetQuestAchivementClearList(MainMenuParam.m_ResultQuestID);
        if (_clearList != null &&
            _clearList.Length != 0)
        {
            for (int i = 0; i < _clearList.Length; i++)
            {
                MasterDataPresent presentData = (!_clearList[i].present_ids.IsNullOrEmpty())
                                                ? MasterDataUtil.GetPresentParamFromID(_clearList[i].present_ids[0])
                                                : null;
                if (presentData != null)
                {
                    ResidentParam.DelAchievementClear(_clearList[i].fix_id);
                }
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    void PopupWebViewCallback()
    {
        moveNextPage();
    }
}



