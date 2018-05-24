using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using DG.Tweening;
using ServerDataDefine;

public class ScoreEventDialog : MenuPartsBase
{
    private static readonly float FadeShowAlpha = 0.5f;
    private static readonly float FadeHideAlpha = 0.0f;
    private static readonly float WindowShowScale = 1.0f;
    private static readonly float WindowHideScale = 0.0f;
    private static readonly float AnimationTime = 0.25f;

    public MenuPartsBase Window = null;
    public GameObject ShadowPanel = null;
    public ScoreRewardWindow RewardWindow = null;

    M4uProperty<List<ScoreEventContext>> eventList = new M4uProperty<List<ScoreEventContext>>(new List<ScoreEventContext>());
    public List<ScoreEventContext> EventList { get { return eventList.Value; } set { eventList.Value = value; } }

    M4uProperty<bool> isViewTab = new M4uProperty<bool>();
    public bool IsViewTab { get { return isViewTab.Value; } set { isViewTab.Value = value; } }

    private bool m_Ready = false;
    private bool m_Show = false;
    private System.Action hideAction = null;

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        IsViewTab = false;
    }

    public void setCamera(Camera camera)
    {
        GetComponentInChildren<Canvas>().worldCamera = camera;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Show(System.Action _hideAction = null)
    {
        if (m_Show)
        {
            return;
        }
        m_Show = true;
        IsViewTab = true;

        hideAction = _hideAction;

        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを登録
            AndroidBackKeyManager.Instance.StackPush(gameObject, OnClose);
        }

        Window.SetPositionAjustStatusBar(new Vector2(604.5f, 0.0f), new Vector2(-71.0f, -400.0f));

        ShadowPanel.GetComponent<Image>().DOFade(FadeShowAlpha, AnimationTime);

        //Window.transform.DOScaleY(WindowShowScale, AnimationTime).OnComplete(() =>
        Window.GetComponent<RectTransform>().DOAnchorPosX(32, AnimationTime).OnComplete(() =>
        {
            m_Ready = true;
        });
    }

    public void Hide()
    {
        if (m_Ready == false)
        {
            return;
        }
        m_Ready = false;

        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを解除
            AndroidBackKeyManager.Instance.StackPop(gameObject);
        }

        ShadowPanel.GetComponent<Image>().DOFade(FadeHideAlpha, AnimationTime);

        //Window.transform.DOScaleY(WindowHideScale, AnimationTime).OnComplete(() =>
        Window.GetComponent<RectTransform>().DOAnchorPosX(601, AnimationTime).OnComplete(() =>
        {
            if (hideAction != null)
            {
                hideAction();
            }
            DestroyObject(gameObject);
        });
    }

    public void OnClose()
    {
        if (m_Ready == false)
        {
            return;
        }

        Hide();
    }

    public void addScoreInfo(PacketStructUserScoreInfo scoreInfo, MasterDataScoreEvent scoreEventMaster)
    {

        ScoreEventContext scoreEvent = new ScoreEventContext();
        //スコア情報
        scoreEvent.ScoreInfo = scoreInfo;
        //タイトル
        scoreEvent.Title = scoreEventMaster.title;
        //期間
        string timeFormat = GameTextUtil.GetText("score_time_format");
        if (TimeEventManager.Instance.ChkEventActive(scoreEventMaster.event_id) == true)
        {
            //開催中
            MasterDataEvent eventData = MasterDataUtil.GetMasterDataEventFromID(scoreEventMaster.event_id);
            DateTime startTime = TimeUtil.GetDateTime(eventData.timing_start);
            if (eventData.timing_end != 0)
            {
                DateTime endTime = TimeUtil.GetDateTime(eventData.timing_end);
                endTime = endTime.SubtractAMinute();
                scoreEvent.Time = string.Format(GameTextUtil.GetText("score_period"), startTime.ToString(timeFormat), endTime.ToString(timeFormat));
            }
            else
            {
                //終了期限なし
                scoreEvent.Time = string.Format(GameTextUtil.GetText("score_period_infinite"), startTime.ToString(timeFormat), "");
            }
            scoreEvent.IsTimeEnd = false;
        }
        else
        {
            //イベント終了
            if (scoreEventMaster.receiving_end != 0)
            {
                DateTime endTime = TimeUtil.GetDateTime(scoreEventMaster.receiving_end);
                endTime = endTime.SubtractAMinute();
                string kikanFormat = GameTextUtil.GetText("Score_period_01");
                scoreEvent.TimeEnd = string.Format(kikanFormat, endTime.ToString(timeFormat));
            }
            else
            {
                scoreEvent.TimeEnd = GameTextUtil.GetText("Score_period_02");
            }
            scoreEvent.IsTimeEnd = true;
        }
        //エリア情報
        scoreEvent.AreaMessage = GameTextUtil.GetText("score_notice");
        //ハイスコア
        scoreEvent.HiScoreLabel = GameTextUtil.GetText("score_subtitle_highscore");
        scoreEvent.HiScore = string.Format(GameTextUtil.GetText("score_entity_01"), scoreInfo.hi_score);
        scoreEvent.HiScorePt = GameTextUtil.GetText("score_entity_02");
        //累積スコア
        scoreEvent.TotalScoreLabel = GameTextUtil.GetText("score_subtitle_cumulative"); ;
        scoreEvent.TotalScore = string.Format(GameTextUtil.GetText("score_entity_01"), scoreInfo.total_score);
        scoreEvent.TotalScorePt = GameTextUtil.GetText("score_entity_02");
        //報酬情報更新
        updateScoreReward(scoreEvent, scoreInfo);

        //アイコン
        MasterDataPresent imagePresent = null;
        if (scoreEventMaster.image_present_id != 0)
        {
            imagePresent = MasterDataUtil.GetPresentParamFromID(scoreEventMaster.image_present_id);
        }
        if (imagePresent != null)
        {
            MainMenuUtil.GetPresentIcon(imagePresent, (sprite) =>
             {
                 scoreEvent.IconImage = sprite;
             });
        }
        else
        {
            UnitIconImageProvider.Instance.Get(
                1,
                (sprite) =>
                {
                    scoreEvent.IconImage = sprite;
                });
        }

        scoreEvent.DidSelectItem = OnSelectItem;

        EventList.Add(scoreEvent);
    }

    private void updateScoreReward(ScoreEventContext scoreEvent, PacketStructUserScoreInfo scoreInfo)
    {
        scoreEvent.IsHiScoreReward = false;
        int nextHiScore = calcNextScore(scoreInfo.hi_score, (int)MasterDataDefineLabel.ScoreRewardType.HI_SCORE, scoreInfo.reward_list);
        if (nextHiScore == 0)
        {
            scoreEvent.NextHiLabel = GameTextUtil.GetText("score_notification_allget");
            scoreEvent.NextHiScore = "";
        }
        else
        {
            int nextScore = nextHiScore - scoreInfo.total_score;
            if (nextScore <= 0)
            {
                scoreEvent.IsHiScoreReward = true;
                scoreEvent.NextHiLabel = GameTextUtil.GetText("score_notification_getreward");
                scoreEvent.NextHiScore = "";
            }
            else
            {
                scoreEvent.NextHiLabel = GameTextUtil.GetText("score_notification_nextreward");
                scoreEvent.NextHiScore = string.Format(GameTextUtil.GetText("score_entity"), nextScore);
            }
        }

        scoreEvent.IsTotalScoreReward = false;
        int nextTotalScore = calcNextScore(scoreInfo.total_score, (int)MasterDataDefineLabel.ScoreRewardType.TOTAL_SCORE, scoreInfo.reward_list);
        if (nextTotalScore == 0)
        {
            scoreEvent.NextTotalLabel = GameTextUtil.GetText("score_notification_allget"); ;
            scoreEvent.NextTotalScore = "";
        }
        else
        {
            int nextScore = nextTotalScore - scoreInfo.total_score;
            if (nextScore <= 0)
            {
                scoreEvent.IsTotalScoreReward = true;
                scoreEvent.NextTotalLabel = GameTextUtil.GetText("score_notification_getreward");
                scoreEvent.NextTotalScore = "";
            }
            else
            {
                scoreEvent.NextTotalLabel = GameTextUtil.GetText("score_notification_nextreward");
                scoreEvent.NextTotalScore = string.Format(GameTextUtil.GetText("score_entity"), nextScore);
            }
        }
    }

    private int calcNextScore(int score, int type, PacketStructUserScoreReward[] rewards)
    {
        List<PacketStructUserScoreReward> rewardList = new List<PacketStructUserScoreReward>();
        for (int i = 0; i < rewards.Length; i++)
        {
            if (rewards[i].type != type)
            {
                continue;
            }

            rewardList.Add(rewards[i]);
        }

        rewardList.Sort((a, b) => a.score - b.score);

        for (int i = 0; i < rewardList.Count; i++)
        {
            return rewardList[i].score;
        }

        return 0;
    }

    public void OnSelectItem(ScoreEventContext item)
    {
        if (RewardWindow.IsShow)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        RewardWindow.setup(item.ScoreInfo);
        RewardWindow.Show(info =>
        {
            item.ScoreInfo = info;
            updateScoreReward(item, info);
        });
    }

    /*-------------------------------------------------------------------------------------*/
    /*                                                                                     */
    /*                                                                                     */
    /*                                                                                     */
    /*-------------------------------------------------------------------------------------*/
    public static ScoreEventDialog Create(Camera camera)
    {
        GameObject _tmpObj = Resources.Load("Prefab/ScoreEventDialog/ScoreEventDialog") as GameObject;
        if (_tmpObj == null)
        {
            return null;
        }

        GameObject _newObj = Instantiate(_tmpObj) as GameObject;
        if (_newObj == null)
        {
            return null;
        }
        UnityUtil.SetObjectEnabledOnce(_newObj, true);

        ScoreEventDialog dlg = _newObj.GetComponent<ScoreEventDialog>();
        dlg.setCamera(camera);

        return dlg;
    }
}
