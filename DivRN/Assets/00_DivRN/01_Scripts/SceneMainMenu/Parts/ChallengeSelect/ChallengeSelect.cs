using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using M4u;
using DG.Tweening;
using ServerDataDefine;

public class ChallengeSelect : MenuPartsBase
{
    private readonly float BOSS_POS_OFFSET = 410.0f;
    private readonly float BOSS_POS_PLAY = 50.0f;
    private readonly float BOSS_POS_ANIMATION_TIME = 0.25f;
    private readonly float BOSS_PANEL_ANIMATION_TIME = 0.25f;

    public class EventData
    {
        public MasterDataChallengeEvent eventMaster;
        public MasterDataChallengeQuest questMaster;
        public PacketStructChallengeInfo info;
        public bool bSkip;
        public int SkipLevel;
        public int UseTicket;

        public EventData()
        {
            eventMaster = null;
            questMaster = null;
            info = null;
            bSkip = false;
            SkipLevel = 0;
            UseTicket = 0;
        }
    };

    public ButtonView[] ButtonList = null;
    public GameObject BossRoot = null;
    public AssetAutoSetEpisodeBackgroundTexture assetAutoSetEpisodeBackgroundTexture = null;
    public RectTransform Panel = null;
    public Image BossPanelImage = null;

    private string ChallengeBossPrefabName = "Prefab/ChallengeSelect/ChallengeBoss";
    private ChallengeBoss Boss = null;

    //
    private M4uProperty<List<BossIconContext>> bossIconList = new M4uProperty<List<BossIconContext>>();
    public List<BossIconContext> BossIconList { get { return bossIconList.Value; } set { bossIconList.Value = value; } }

    //ボス画像リスト
    private M4uProperty<List<BossSpriteContext>> bossSpriteList = new M4uProperty<List<BossSpriteContext>>();
    public List<BossSpriteContext> BossSpriteList { get { return bossSpriteList.Value; } set { bossSpriteList.Value = value; } }

    //イベントタイトル
    private M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    //開催期間
    private M4uProperty<string> time = new M4uProperty<string>();
    public string Time { get { return time.Value; } set { time.Value = value; } }

    //ボスアイコン
    private M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    //属性イメージ
    private M4uProperty<Sprite> elementImage = new M4uProperty<Sprite>();
    public Sprite ElementImage { get { return elementImage.Value; } set { elementImage.Value = value; } }

    //レベル
    private M4uProperty<string> level = new M4uProperty<string>();
    public string Level { get { return level.Value; } set { level.Value = value; } }

    /*
     * ボタンラベル
     */
    private M4uProperty<string> rewardLabel = new M4uProperty<string>();
    public string RewardLabel { get { return rewardLabel.Value; } set { rewardLabel.Value = value; } }

    private M4uProperty<string> ruleLabel = new M4uProperty<string>();
    public string RuleLabel { get { return ruleLabel.Value; } set { ruleLabel.Value = value; } }

    private M4uProperty<string> attrLabel = new M4uProperty<string>();
    public string AttrLabel { get { return attrLabel.Value; } set { attrLabel.Value = value; } }

    /*
     * ボタンフラグ
     */
    private M4uProperty<bool> isViewSkipButton = new M4uProperty<bool>();
    public bool IsViewSkipButton { get { return isViewSkipButton.Value; } set { isViewSkipButton.Value = value; } }

    private M4uProperty<Color> skipButtonColor = new M4uProperty<Color>();
    public Color SkipButtonColor { get { return skipButtonColor.Value; } set { skipButtonColor.Value = value; } }

    private M4uProperty<bool> isActiveSkipButton = new M4uProperty<bool>();
    public bool IsActiveSkipButton
    {
        get
        {
            return isActiveSkipButton.Value;
        }
        set
        {
            SkipButtonColor = (value ? new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1));
            isActiveSkipButton.Value = value;
        }
    }

    private M4uProperty<bool> isActiveOkButton = new M4uProperty<bool>();
    public bool IsActiveOkButton { get { return isActiveOkButton.Value; } set { isActiveOkButton.Value = value; } }

    private M4uProperty<bool> isViewRightButton = new M4uProperty<bool>();
    public bool IsViewRightButton { get { return isViewRightButton.Value; } set { isViewRightButton.Value = value; } }

    private M4uProperty<bool> isViewLeftButton = new M4uProperty<bool>();
    public bool IsViewLeftButton { get { return isViewLeftButton.Value; } set { isViewLeftButton.Value = value; } }

    /*
     * フラグ関連
     */
    private M4uProperty<bool> isViewFlag = new M4uProperty<bool>();
    public bool IsViewFlag { get { return isViewFlag.Value; } set { isViewFlag.Value = value; } }

    private M4uProperty<Sprite> flagImage = new M4uProperty<Sprite>();
    public Sprite FlagImage { get { return flagImage.Value; } set { flagImage.Value = value; } }

    private M4uProperty<string> flagValue = new M4uProperty<string>();
    public string FlagValue { get { return flagValue.Value; } set { flagValue.Value = value; } }

    private M4uProperty<bool> isViewRewardFlag = new M4uProperty<bool>();
    public bool IsViewRewardFlag { get { return isViewRewardFlag.Value; } set { isViewRewardFlag.Value = value; } }

    /*
     * イベントコールバック
     */
    public System.Action<EventData> OnChengedBoss = delegate { };

    public System.Action<EventData> OnSelectOk = delegate { };
    public System.Action<EventData> OnSelectReward = delegate { };
    public System.Action<EventData> OnSelectRule = delegate { };
    public System.Action<EventData> OnSelectBossAtr = delegate { };
    public System.Action<EventData> OnSelectSkip = delegate { };

    //イベントリスト
    private List<EventData> m_Events = new List<EventData>();
    public List<EventData> Events { get { return m_Events; } }

    /*
     *
     */
    private bool m_bReady = false;
    public bool IsReady { get { return m_bReady; } }
    private int m_SelectIndex = 0;
    private Vector2 m_StartPos = Vector2.zero;
    private Vector2 m_BossPos = Vector2.zero;
    private float m_MinPosX = 0.0f;
    private float m_MaxPosX = 0.0f;
    private List<ButtonModel> m_buttonModels = new List<ButtonModel>();
    private AssetBundlerMultiplier m_AssetBundleMulti = null;

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;

        BossIconList = new List<BossIconContext>();
        BossSpriteList = new List<BossSpriteContext>();

        Title = "";
        Time = "";
        IconImage = null;
        ElementImage = MainMenuUtil.GetElementCircleSprite(MasterDataDefineLabel.ElementType.NAUGHT);

        RewardLabel = GameTextUtil.GetText("growth_boss_10");
        RuleLabel = GameTextUtil.GetText("growth_boss_11");
        AttrLabel = GameTextUtil.GetText("growth_boss_12");

        IsViewSkipButton = true;
        IsActiveSkipButton = false;
        IsActiveOkButton = false;

        IsViewFlag = false;
        FlagImage = null;
        FlagValue = "";

        IsViewRewardFlag = false;

        BossPanelImage.color = new Color(1, 1, 1, 0);
    }

    public void ClearAll()
    {
        m_Events.Clear();
    }

    public void AddEventData(MasterDataChallengeEvent eventMaster, PacketStructChallengeInfo info, MasterDataChallengeQuest questMaster = null)
    {
        if (questMaster == null)
        {
            questMaster = MasterDataUtil.GetChallengeQuestMaster(eventMaster.event_id, info.challenge_level);
        }

        EventData newData = new EventData();
        newData.eventMaster = eventMaster;
        newData.questMaster = questMaster;
        newData.info = info;
        m_Events.Add(newData);
    }

    public void setup(uint select_event_id = 0)
    {
        m_AssetBundleMulti = AssetBundlerMultiplier.Create();

        //3DTest
        //initChallengeBoss(boss_ids);
        initButton();

        //
        initBossData(select_event_id);

        if (assetAutoSetEpisodeBackgroundTexture != null)
        {

            m_AssetBundleMulti
                .Add(LoadBackgroundTexture());
        }

        m_AssetBundleMulti.Load(
            () =>
            {
                //
                float movePos = BOSS_POS_OFFSET * (BossIconList.Count - m_SelectIndex - 1);
                RectTransform trans = BossRoot.GetComponent<RectTransform>();
                trans.anchoredPosition = new Vector2(movePos, trans.anchoredPosition.y);

                BossPanelImage.DOColor(new Color(1, 1, 1, 1), BOSS_PANEL_ANIMATION_TIME);

                Panel.DOScaleY(1.0f, BOSS_PANEL_ANIMATION_TIME)
                .OnComplete(() =>
                {
                    //ボタン有効化
                    for (int i = 0; i < BossIconList.Count; i++)
                    {
                        BossIconList[i].model.Appear();
                    }
                    for (int i = 0; i < m_buttonModels.Count; i++)
                    {
                        m_buttonModels[i].Appear();
                    }

                    updateButton();

                    //ボス変更コールバック
                    OnChengedBoss(m_Events[m_SelectIndex]);

                    m_bReady = true;
                });
            });
    }

    private AssetBundler LoadBackgroundTexture()
    {
        AssetBundler asset = assetAutoSetEpisodeBackgroundTexture.Create(m_Events[m_SelectIndex].eventMaster,
            () =>
            {
                assetAutoSetEpisodeBackgroundTexture.Color = new Color(1, 1, 1, 1);
            });
        return asset;
    }

#if false//3DTest
    private void initChallengeBoss( uint[] boss_ids )
    {
        //
        GameObject tmpObj = Resources.Load(ChallengeBossPrefabName) as GameObject;
        if (tmpObj == null)
        {
            return;
        }

        GameObject insObj = Instantiate(tmpObj);
        if (insObj == null)
        {
            return;
        }

        UnityUtil.SetObjectEnabledOnce(insObj, true);

        Boss = insObj.GetComponent<ChallengeBoss>();
        if (Boss != null)
        {
            Boss.AddBossData(boss_ids);
        }
    }
#endif

    private void initBossData(uint select_event_id)
    {
        BossIconList.Clear();
        BossSpriteList.Clear();

        m_Events.Reverse();

        for (int i = 0; i < m_Events.Count; i++)
        {
            uint event_id = m_Events[i].eventMaster.event_id;
            uint boss_id = m_Events[i].questMaster.boss_chara_id;
            {
                int index = i;
                BossIconContext iconContext = new BossIconContext();
                iconContext.boss_id = boss_id;

                iconContext.model = new ButtonModel();
                iconContext.model.OnClicked += () =>
                {
                    OnSelectBoss(index);
                };

                iconContext.IsSelect = false;
                if (event_id == select_event_id)
                {
                    iconContext.IsSelect = true;
                    m_SelectIndex = index;
                }

                BossIconList.Add(iconContext);
            }

            {
                BossSpriteContext spriteContext = new BossSpriteContext();
                AssetBundler asset = spriteContext.setup(boss_id);
                if (asset != null)
                {
                    m_AssetBundleMulti.Add(asset);
                }
                BossSpriteList.Add(spriteContext);
            }
        }

        m_MinPosX = -BOSS_POS_PLAY;
        m_MaxPosX = (BOSS_POS_OFFSET * (float)(BossSpriteList.Count - 1)) + BOSS_POS_PLAY;

    }

    private void initButton()
    {
        var elementActoinMap = new List<System.Action<EventData>>
        {
            OnSelectOk,
            OnSelectReward,
            OnSelectRule,
            OnSelectBossAtr,
            OnSelectSkip,
            OnSelectRight,
            OnSelectLeft,
        };

        for (int i = 0; i < ButtonList.Length; i++)
        {
            int index = i;
            ButtonModel model = new ButtonModel();
            model.OnClicked += () =>
            {
                if (m_bReady)
                {
                    elementActoinMap[index](m_Events[m_SelectIndex]);
                }
            };

            ButtonList[index].SetModel(model);

            m_buttonModels.Add(model);
        }
    }

    private void OnSelectBoss(int index)
    {
        if (index < 0 ||
            m_bReady == false)
        {
            return;
        }

        if (m_SelectIndex == index)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);

        m_bReady = false;

        m_SelectIndex = index;

        float movePos = BOSS_POS_OFFSET * (m_Events.Count - index - 1);

        moveBossPos(movePos, () =>
        {
            //ボタン状態更新
            updateButton();

            //背景BG更新
            LoadBackgroundTexture().Load();

            //ボス変更コールバック
            OnChengedBoss(m_Events[m_SelectIndex]);

            m_bReady = true;
        });
    }

    private void moveBossPos(float posX, System.Action endAction = null)
    {
        BossRoot.GetComponent<RectTransform>()
            .DOAnchorPosX(posX, BOSS_POS_ANIMATION_TIME)
            .OnComplete(() =>
            {
                if (endAction != null)
                {
                    endAction();
                }

            });
    }

    private void updateButton()
    {
        //選択状態更新
        for (int i = 0; i < BossIconList.Count; i++)
        {
            BossIconList[i].IsSelect = (i == m_SelectIndex ? true : false);
        }

        //サイドボタン更新
        if (m_Events.Count <= 1)
        {
            IsViewRightButton = false;
            IsViewLeftButton = false;
        }
        else if (m_SelectIndex == 0)
        {
            IsViewRightButton = true;
            IsViewLeftButton = false;
        }
        else if (m_SelectIndex == (m_Events.Count - 1))
        {
            IsViewRightButton = false;
            IsViewLeftButton = true;
        }
        else
        {
            IsViewRightButton = true;
            IsViewLeftButton = true;
        }
    }

    private void OnSelectRight(EventData data)
    {
        if (m_bReady == false)
        {
            return;
        }

        if (m_SelectIndex < (m_Events.Count - 1))
        {
            OnSelectBoss(m_SelectIndex + 1);
        }
    }

    private void OnSelectLeft(EventData data)
    {
        if (m_bReady == false)
        {
            return;
        }

        if (m_SelectIndex != 0 &&
            m_Events.Count != 0)
        {
            OnSelectBoss(m_SelectIndex - 1);
        }

    }

    /// <summary>
    /// ドラッグ開始
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(BaseEventData eventData)
    {
        if (m_bReady == false ||
            m_Events.Count <= 1)
        {
            return;
        }

        ButtonBlocker.Instance.Block();

        PointerEventData point = (PointerEventData)eventData;
        //Debug.Log("OnBiginDrag:" + point.position.ToString());
        m_StartPos = point.position;
        m_BossPos = BossRoot.transform.localPosition;
    }

    /// <summary>
    /// ドラッグ終了
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(BaseEventData eventData)
    {
        if (m_bReady == false ||
            m_Events.Count <= 1)
        {
            return;
        }

        int index = (m_Events.Count - 1) - Mathf.FloorToInt(BossRoot.transform.localPosition.x / BOSS_POS_OFFSET + 0.5f);
        if (index < 0)
        {
            index = 0;
        }
        else if (index >= m_Events.Count)
        {
            index = m_Events.Count - 1;
        }
        float movePos = BOSS_POS_OFFSET * (m_Events.Count - index - 1);

        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);

        moveBossPos(movePos, () =>
        {
            if (index != m_SelectIndex)
            {
                m_SelectIndex = index;

                //ボタン状態更新
                updateButton();

                //背景BG更新
                LoadBackgroundTexture().Load();

                //ボス変更コールバック
                OnChengedBoss(m_Events[m_SelectIndex]);
            }

            ButtonBlocker.Instance.Unblock();
        });

        //Debug.Log("OnEndDrag:" + point.position.ToString());
    }

    /// <summary>
    /// ドラッグ中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(BaseEventData eventData)
    {
        if (m_bReady == false ||
            m_Events.Count <= 1)
        {
            return;
        }

        PointerEventData point = (PointerEventData)eventData;

        float move = m_StartPos.x - point.position.x;
        //Debug.Log("OnDrag:" + move.ToString());
        float posX = Mathf.Clamp(m_BossPos.x - move, m_MinPosX, m_MaxPosX);
        BossRoot.transform.localPosition = new Vector3(posX, 0, 0);
    }
}
