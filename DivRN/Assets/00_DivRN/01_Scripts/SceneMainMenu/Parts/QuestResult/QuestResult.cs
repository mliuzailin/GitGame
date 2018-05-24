using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class QuestResult : MenuPartsBase
{
    [SerializeField]
    private GameObject m_TopMask;
    [SerializeField]
    private GameObject m_BottomMask;

    [SerializeField]
    private GameObject m_particleRoot = null;
    [SerializeField]
    private GameObject m_levelUpRoot = null;
    [SerializeField]
    private GameObject m_gradeUpRoot = null;
    [SerializeField]
    private GameObject m_hiscoreRoot = null;


    [SerializeField]
    private ScrollRect m_dropItemsScroll;
    private MoveTo m_moveTo = null;

    [SerializeField]
    private Sprite m_BackGroundImage = null;

    private static readonly string AppearAnimationName = "quest_result_appear";
    private static readonly string AppearDropsAnimationName = "quest_result_drops_appear";
    private static readonly string AppearMissionAnimationName = "quest_result_mission_appear";
    private static readonly string DefaultAnimationName = "quest_result_loop";

    public AssetAutoSetEpisodeBackgroundTexture assetAutoSetEpisodeBackgroundTexture;
    public MasterDataArea m_AreaMaster;

    public GameObject m_StepViewRoot = null;

    //    // BG
    //    M4uProperty<Sprite> backGroundImage = new M4uProperty<Sprite>();
    //    public Sprite BackGroundImage { get { return backGroundImage.Value; } set { backGroundImage.Value = value; } }

    // エリアカテゴリ名
    M4uProperty<string> areaCategoryName = new M4uProperty<string>("");
    public string AreaCategoryName { get { return areaCategoryName.Value; } set { areaCategoryName.Value = value; } }
    // エリア名
    M4uProperty<string> areaNameText = new M4uProperty<string>("");
    public string AreaNameText { get { return areaNameText.Value; } set { areaNameText.Value = value; } }
    // クエスト名称
    M4uProperty<string> questNameText = new M4uProperty<string>("");
    public string QuestNameText { get { return questNameText.Value; } set { questNameText.Value = value; } }

    /// <summary>
    /// クリア情報
    /// </summary>

    // クリア情報表示フラグ
    M4uProperty<bool> isClearStatus = new M4uProperty<bool>();
    public bool IsClearStatus { get { return isClearStatus.Value; } set { isClearStatus.Value = value; } }

    // コインラベル
    M4uProperty<string> coinLabel = new M4uProperty<string>();
    public string CoinLabel { get { return coinLabel.Value; } set { coinLabel.Value = value; } }
    /// <summary>キーの保存用</summary>
    string coinTextKey = "";
    // コイン数
    int coin = 0;
    public int Coin
    {
        get { return coin; }
        set
        {
            coin = value;
            if (coinTextKey.IsNullOrEmpty())
            {
                coinTextKey = GameTextUtil.GetText("value_colorset");
            }
            CoinText = string.Format(coinTextKey, value);
        }
    }
    M4uProperty<string> coinText = new M4uProperty<string>("");
    // コイン数のテキスト
    private string CoinText { get { return coinText.Value; } set { coinText.Value = value; } }

    // チケットラベル
    M4uProperty<string> ticketLabel = new M4uProperty<string>();
    public string TicketLabel { get { return ticketLabel.Value; } set { ticketLabel.Value = value; } }
    /// <summary>キーの保存用</summary>
    string ticketTextKey = "";
    // チケット数
    int ticket = 0;
    public int Ticket
    {
        get { return ticket; }
        set
        {
            ticket = value;
            if (ticketTextKey.IsNullOrEmpty())
            {
                ticketTextKey = GameTextUtil.GetText("value_colorset");
            }
            TicketText = string.Format(ticketTextKey, value);
        }
    }
    M4uProperty<string> ticketText = new M4uProperty<string>();
    // チケット数のテキスト
    private string TicketText { get { return ticketText.Value; } set { ticketText.Value = value; } }

    // 経験値
    M4uProperty<int> exp = new M4uProperty<int>(0);
    public int Exp
    {
        get { return exp.Value; }
        set
        {
            exp.Value = value;
            //OnExpUpdated();
        }
    }
    // 経験値ラベル
    M4uProperty<string> expLabel = new M4uProperty<string>();
    public string ExpLabel { get { return expLabel.Value; } set { expLabel.Value = value; } }

    // 次のランクまでの必要経験値
    M4uProperty<int> nextRank = new M4uProperty<int>(0);
    public int NextRank { get { return nextRank.Value; } set { nextRank.Value = value; } }
    // 次のランクまでの必要経験値ラベル
    M4uProperty<string> nextRankLabel = new M4uProperty<string>();
    public string NextRankLabel { get { return nextRankLabel.Value; } set { nextRankLabel.Value = value; } }

    // 次のランクまでの必要経験値(全体の％)
    M4uProperty<float> nextRankPercent = new M4uProperty<float>(0.5f);
    public float NextRankPercent
    {
        get { return nextRankPercent.Value; }
        set
        {
            nextRankPercent.Value = value;
        }
    }

    // ランクアップ表示フラグ
    M4uProperty<bool> isRankUp = new M4uProperty<bool>();
    public bool IsRankUp
    {
        get { return isRankUp.Value; }
        set
        {
            isRankUp.Value = value;

            if (value)
                OnGotLevelUp();
        }
    }

    // ランクアップメッセージ
    M4uProperty<string> rankUpMessage = new M4uProperty<string>();
    public string RankUpMessage { get { return rankUpMessage.Value; } set { rankUpMessage.Value = value; } }


    /// <summary>
    /// ドロップ関連
    /// </summary>

    // ドロップ情報表示フラグ
    M4uProperty<bool> isDrops = new M4uProperty<bool>();
    public bool IsDrops
    {
        get { return isDrops.Value; }
        set
        {
            if (value == true
                && isDrops.Value == false)
            {
                ShowDrops();
            }

            isDrops.Value = value;
        }
    }

    // 通常ドロップ報酬
    M4uProperty<List<DropIconContex>> dropIcons = new M4uProperty<List<DropIconContex>>(new List<DropIconContex>());
    public List<DropIconContex> DropIcons { get { return dropIcons.Value; } set { dropIcons.Value = value; } }

    // TODO : 全体的にこのクラスの持っている役割が不十分なのでほかと合わせて整理
    private List<DropIconListItemModel> m_units = new List<DropIconListItemModel>();
    public DropIconListItemModel GetUnitAt(int index)
    {
        return m_units[index];
    }
    public void AddUnit(DropIconListItemModel unit)
    {
        unit.OnAppeared += () =>
        {
            ShiftDropItemScroll();
        };

        m_units.Add(unit);
    }
    public void ClearUnits()
    {
        m_units.Clear();
    }

    private List<DropIconListItemModel> m_floorBonuses = new List<DropIconListItemModel>();
    public DropIconListItemModel GetFloorBonusAt(int index)
    {
        return m_floorBonuses[index];
    }
    public void AddFloorBonus(DropIconListItemModel unit)
    {
        m_floorBonuses.Add(unit);
    }
    public void ClearFloorBonuses()
    {
        m_floorBonuses.Clear();
    }


    // ドロップラベル
    M4uProperty<string> dropLabel = new M4uProperty<string>();
    public string DropLabel { get { return dropLabel.Value; } set { dropLabel.Value = value; } }

    M4uProperty<string> dropGuideText = new M4uProperty<string>();
    /// <summary>ドロップのガイド</summary>
    public string DropGuideText { get { return dropGuideText.Value; } set { dropGuideText.Value = value; } }

    M4uProperty<bool> isViewDropGuid = new M4uProperty<bool>();
    /// <summary>ドロップのガイドの表示・非表示</summary>
    public bool IsViewDropGuid { get { return isViewDropGuid.Value; } set { isViewDropGuid.Value = value; } }

    // フロアボーナス
    M4uProperty<List<DropIconContex>> floorBonus = new M4uProperty<List<DropIconContex>>(new List<DropIconContex>());
    public List<DropIconContex> FloorBonus { get { return floorBonus.Value; } set { floorBonus.Value = value; } }

    // フロアボーナスラベル
    M4uProperty<string> floorBonusLabel = new M4uProperty<string>();
    public string FloorBonusLabel { get { return floorBonusLabel.Value; } set { floorBonusLabel.Value = value; } }

    /// <summary>
    /// ミッション関連
    /// </summary>

    // ミッション情報表示フラグ
    M4uProperty<bool> isMissionClear = new M4uProperty<bool>();
    public bool IsMissionClear
    {
        get { return isMissionClear.Value; }
        set
        {
            if (value == true
                && isMissionClear.Value == false)
            {
                ShowMission();
            }

            isMissionClear.Value = value;
        }
    }

    // ミッションリスト
    M4uProperty<List<QuestMissionContext>> missionList = new M4uProperty<List<QuestMissionContext>>(new List<QuestMissionContext>());
    public List<QuestMissionContext> MissionList { get { return missionList.Value; } set { missionList.Value = value; } }

    // ミッションラベル
    M4uProperty<string> missionLabel = new M4uProperty<string>();
    public string MissionLabel { get { return missionLabel.Value; } set { missionLabel.Value = value; } }

    M4uProperty<string> missionGuideText = new M4uProperty<string>();
    /// <summary>ミッションのガイド</summary>
    public string MissionGuideText { get { return missionGuideText.Value; } set { missionGuideText.Value = value; } }

    // ミッションフラグ
    M4uProperty<bool> isMissionFlag = new M4uProperty<bool>();
    public bool IsMissionFlag { get { return isMissionFlag.Value; } set { isMissionFlag.Value = value; } }

    /// <summary>
    /// マスター関連
    /// </summary>

    // マスター情報表示フラグ
    M4uProperty<bool> isMasterGradeUp = new M4uProperty<bool>();
    public bool IsMasterGradeUp { get { return isMasterGradeUp.Value; } set { isMasterGradeUp.Value = value; } }

    // グレードアップメッセージ
    M4uProperty<string> gradeUpMessage = new M4uProperty<string>();
    public string GradeUpMessage { get { return gradeUpMessage.Value; } set { gradeUpMessage.Value = value; } }

    /// <summary>
    /// スコア関連
    /// </summary>

    // スコア情報表示フラグ
    M4uProperty<bool> isScoreInfo = new M4uProperty<bool>();
    public bool IsScoreInfo { get { return isScoreInfo.Value; } set { isScoreInfo.Value = value; } }

    //スコアタイトル
    M4uProperty<string> scoreTitle = new M4uProperty<string>();
    public string ScoreTitle { get { return scoreTitle.Value; } set { scoreTitle.Value = value; } }

    //基礎スコアラベル
    M4uProperty<string> baseScoreLabel = new M4uProperty<string>();
    public string BaseScoreLabel { get { return baseScoreLabel.Value; } set { baseScoreLabel.Value = value; } }

    //基礎スコア
    M4uProperty<int> baseScore = new M4uProperty<int>();
    public int BaseScore { get { return baseScore.Value; } set { baseScore.Value = value; } }

    //クエストボーナスラベル
    M4uProperty<string> questBonusLabel = new M4uProperty<string>();
    public string QuestBonusLabel { get { return questBonusLabel.Value; } set { questBonusLabel.Value = value; } }

    //クエストボーナス
    M4uProperty<float> questBonus = new M4uProperty<float>();
    public float QuestBonus { get { return questBonus.Value; } set { questBonus.Value = value; } }

    //特効ボーナスラベル
    M4uProperty<string> specialBonusLabel = new M4uProperty<string>();
    public string SpecialBonusLabel { get { return specialBonusLabel.Value; } set { specialBonusLabel.Value = value; } }

    //特効ボーナス
    M4uProperty<float> specialBonus = new M4uProperty<float>();
    public float SpecialBonus { get { return specialBonus.Value; } set { specialBonus.Value = value; } }

    //ペナルティラベル
    M4uProperty<string> penaltyLabel = new M4uProperty<string>();
    public string PenaltyLabel { get { return penaltyLabel.Value; } set { penaltyLabel.Value = value; } }

    //ペナルティ
    M4uProperty<int> penalty = new M4uProperty<int>();
    public int Penalty { get { return penalty.Value; } set { penalty.Value = value; } }

    //獲得スコアラベル
    M4uProperty<string> totalScoreLabel = new M4uProperty<string>();
    public string TotalScoreLabel { get { return totalScoreLabel.Value; } set { totalScoreLabel.Value = value; } }

    //獲得スコア
    M4uProperty<int> totalScore = new M4uProperty<int>();
    public int TotalScore { get { return totalScore.Value; } set { totalScore.Value = value; } }

    // スコア報酬表示フラグ
    M4uProperty<bool> isScoreReward = new M4uProperty<bool>();
    public bool IsScoreReward { get { return isScoreReward.Value; } set { isScoreReward.Value = value; } }

    //スコア報酬ラベル
    M4uProperty<string> scoreRewardLabel = new M4uProperty<string>();
    public string ScoreRewardLabel { get { return scoreRewardLabel.Value; } set { scoreRewardLabel.Value = value; } }

    //スコア報酬メッセージ
    M4uProperty<string> scoreRewardMessage = new M4uProperty<string>();
    public string ScoreRewardMessage { get { return scoreRewardMessage.Value; } set { scoreRewardMessage.Value = value; } }

    //スコア報酬リスト
    M4uProperty<List<ScoreRewardContext>> scoreRewardList = new M4uProperty<List<ScoreRewardContext>>(new List<ScoreRewardContext>());
    public List<ScoreRewardContext> ScoreRewardList { get { return scoreRewardList.Value; } set { scoreRewardList.Value = value; } }

    /// <summary>
    /// 成長ボス関連
    /// </summary>
    // 成長ボス報酬表示フラグ
    M4uProperty<bool> isChallengeReward = new M4uProperty<bool>();
    public bool IsChallengeReward { get { return isChallengeReward.Value; } set { isChallengeReward.Value = value; } }

    //成長ボス報酬ラベル
    M4uProperty<string> challengeRewardLabel = new M4uProperty<string>();
    public string ChallengeRewardLabel { get { return challengeRewardLabel.Value; } set { challengeRewardLabel.Value = value; } }

    //成長ボス報酬メッセージ
    M4uProperty<string> challengeRewardMessage = new M4uProperty<string>();
    public string ChallengeRewardMessage { get { return challengeRewardMessage.Value; } set { challengeRewardMessage.Value = value; } }

    //成長ボス報酬リスト
    M4uProperty<List<ChallengeRewardContext>> challengeRewardList = new M4uProperty<List<ChallengeRewardContext>>(new List<ChallengeRewardContext>());
    public List<ChallengeRewardContext> ChallengeRewardList { get { return challengeRewardList.Value; } set { challengeRewardList.Value = value; } }

    /// <summary>
    /// その他
    /// </summary>

    // ボタンON/OFF
    M4uProperty<bool> isEnableButton = new M4uProperty<bool>();
    public bool IsEnableButton { get { return isEnableButton.Value; } set { isEnableButton.Value = value; } }

    public System.Action DidSelectCheck = delegate { };

    void Awake()
    {
        IsClearStatus = false;
        IsDrops = false;
        IsViewDropGuid = false;
        IsMissionClear = false;
        IsMasterGradeUp = false;
        IsScoreInfo = false;
        IsScoreReward = false;
        IsChallengeReward = false;
        QuestNameText = "";
        RankUpMessage = "";
        GradeUpMessage = "";

        DropLabel = GameTextUtil.GetText("result_text1");
        DropGuideText = GameTextUtil.GetText("result_text17");
        FloorBonusLabel = GameTextUtil.GetText("result_text15");
        MissionLabel = GameTextUtil.GetText("result_text2");
        MissionGuideText = GameTextUtil.GetText("result_text18");

        CoinLabel = GameTextUtil.GetText("result_text5");
        TicketLabel = GameTextUtil.GetText("result_text6");
        ExpLabel = GameTextUtil.GetText("result_text7");
        NextRankLabel = GameTextUtil.GetText("result_text9");
        GetComponent<M4uContextRoot>().Context = this;

        //		BackGroundImage = m_BackGroundImage;
    }

    void Start()
    {
        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.enebleMask(m_TopMask, m_BottomMask);

            int bottom_space_height = SafeAreaControl.Instance.bottom_space_height;
            if (bottom_space_height > 0)
            {
                int bar_height = SafeAreaControl.Instance.bar_height;

                RectTransform rect = m_TopMask.GetComponent<RectTransform>();
                rect.AddLocalPositionY(bar_height);

                rect = m_BottomMask.GetComponent<RectTransform>();
                rect.AddLocalPositionY(bottom_space_height * -1);
            }
        }
    }

    void Update()
    {
        if (m_moveTo != null)
        {
            m_moveTo.Tick(Time.deltaTime);
        }

        if (m_moveTo != null)
        {
            m_dropItemsScroll.normalizedPosition = new Vector2(m_moveTo.GetCurrentX(), 0);
        }
    }

    public void Show(System.Action callback = null)
    {
        Action action = () =>
        {
            PlayAnimation(AppearAnimationName, () =>
            {
                if (callback != null)
                {
                    callback();
                }
                PlayAnimation(DefaultAnimationName);
            });
        };

        switch (MasterDataUtil.GetQuestType(MainMenuParam.m_ResultQuestID))
        {
            case MasterDataDefineLabel.QuestType.NORMAL:
                assetAutoSetEpisodeBackgroundTexture.Create(m_AreaMaster.area_cate_id, action, action).Load();
                break;
            case MasterDataDefineLabel.QuestType.CHALLENGE:
                {
                    MasterDataChallengeEvent eventMaster = MasterDataUtil.GetChallengeEventFromQuestID(MainMenuParam.m_ResultQuestID);
                    if (eventMaster != null)
                    {
                        assetAutoSetEpisodeBackgroundTexture.Create(eventMaster, action, action).Load();
                    }
                }
                break;
            default:
                assetAutoSetEpisodeBackgroundTexture.Create(m_AreaMaster.area_cate_id, action, action).Load();
                break;
        }
    }

    public void ShowDrops(System.Action callback = null)
    {
        PlayAnimation(AppearDropsAnimationName, () =>
        {
            if (!IsNoUnitDrop())
            {
                SoundUtil.PlaySE(SEID.SE_MM_B04_RARE_START);
                SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_UNIT_GET);
            }

            if (callback != null)
            {
                callback();
            }

            PlayAnimation(DefaultAnimationName);

            if (m_units.Count > 0)
            {
                m_units[0].Appear();
            }
            else
            {
                foreach (var floorBonus in m_floorBonuses)
                {
                    floorBonus.Appear();
                }
            }
        });
    }

    private bool IsNoUnitDrop()
    {
        foreach (var floorBonus in m_floorBonuses)
        {
            if (floorBonus.isUnit)
            {
                return false;

            }
        }

        return m_units.Count == 0;
    }

    public void ShowMission(System.Action callback = null)
    {
        PlayAnimation(AppearMissionAnimationName, () =>
        {
            if (callback != null)
            {
                callback();
            }
            PlayAnimation(DefaultAnimationName);
        });
    }

    public void SkipShowMission()
    {
        // 短いのでとりあえずなし
    }

    public GradeUpEffect ShowGradeUp(System.Action callback = null)
    {
        return GradeUpEffect.Attach(m_gradeUpRoot).Show(callback);
    }

    public HiScoreEffect ShowHiScore(System.Action callback = null)
    {
        return HiScoreEffect.Attach(m_hiscoreRoot).Show(callback);
    }

    public void LoopUnits()
    {
        foreach (var unit in m_units)
        {
            unit.LoopStart();
        }
    }
    public void LoopFloorBonuses()
    {
        foreach (var floorBonus in m_floorBonuses)
        {
            floorBonus.LoopStart();
        }
    }

    public void ShowParticle()
    {
        while (m_particleRoot.transform.childCount > 0)
        {
            var child = m_particleRoot.transform.GetChild(0);
            GameObject.Destroy(child);
        }
        SplashParticleView.Attach(m_particleRoot);
    }

    public void ShiftDropItemScroll()
    {
        float duration = 0.2f;

        m_moveTo = new MoveTo(m_dropItemsScroll.normalizedPosition)
            .SetDestination(new Vector2
                                (
                                    1,
                                    m_dropItemsScroll.normalizedPosition.y
                                ))
            .SetDuration(duration)
            .RegisterOnFinishedCallback(() =>
            {
                m_dropItemsScroll.normalizedPosition = new Vector2(1, 0);
                m_moveTo = null;
            });

    }

    // シーン読み込みの最後に呼び出す
    public void PostSceneStart()
    {
    }

    public GameObject GetSetpViewRoot()
    {
        return m_StepViewRoot;
    }


    public void OnSelectCheck()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("OnSelectCheck()");
#endif
        DidSelectCheck();
    }


    private void OnExpUpdated()
    {
        var ratio = NextRank == 0
            ? 1.0f
            : 1.0f * (NextRank - Exp) / (NextRank);

        if (ratio < 0)
        {
            ratio = 0;
        }

        if (ratio > 1)
        {
            ratio = 1;
        }

        nextRankPercent.Value = ratio;
    }

    private LevelUpEffect m_levelUpEffect = null;
    private void OnGotLevelUp()
    {
        if (m_levelUpEffect == null)
        {
            m_levelUpEffect = LevelUpEffect.Attach(m_levelUpRoot);
        }

        m_levelUpEffect.Show();
    }

    public void OnClickNextButton()
    {
        QuestResultStepView view = GetComponentInChildren<QuestResultStepView>();
        if (view != null)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("OnClickNextButton");
#endif
            view.ClickNextButton();
        }
    }

    // TODO : 別ファイルに
    class MoveTo
    {
        private Vector2 m_original;
        private Vector2 m_destination;
        private float m_duration;
        private float m_elaspedTime = 0;
        private System.Action m_onFinished = null;

        public MoveTo(Vector2 from)
        {
            m_original = m_destination = from;
        }

        public MoveTo SetDestination(Vector2 to)
        {
            m_destination = to;
            return this;
        }

        public MoveTo SetDuration(float duration)
        {
            m_duration = duration;
            return this;
        }

        public MoveTo RegisterOnFinishedCallback(System.Action callback)
        {
            m_onFinished = callback;

            return this;
        }

        public void Tick(float delta)
        {
            if (m_elaspedTime >= m_duration)
            {
                return;
            }

            m_elaspedTime += delta;

            if (m_elaspedTime >= m_duration)
            {
                if (m_onFinished != null)
                {
                    m_onFinished();
                }
            }
        }

        public float GetCurrentX()
        {
            float ratio = m_elaspedTime / m_duration;
            return (m_original + (m_destination - m_original) * ratio).x;
        }
    }
}
