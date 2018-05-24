using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using DG.Tweening;
using ServerDataDefine;
using System;

public class ScoreRewardWindow : MenuPartsBase, IRecyclableItemsScrollContentDataProvider
{
    private static readonly int TAB_HI_SCORE = 0;
    private static readonly int TAB_TOTAL_SCORE = 1;
    private static readonly float FadeShowAlpha = 0.5f;
    private static readonly float FadeHideAlpha = 0.0f;
    private static readonly float WindowShowScale = 1.0f;
    private static readonly float WindowHideScale = 0.0f;
    private static readonly float AnimationTime = 0.25f;

    public ScrollRect scrollRect = null;
    public RecyclableItemsScrollContent scrollContent = null;
    public GameObject itemPrefab = null;

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<int> tabIndex = new M4uProperty<int>();
    public int TabIndex { get { return tabIndex.Value; } set { tabIndex.Value = value; } }

    M4uProperty<string> tab0Title = new M4uProperty<string>();
    public string Tab0Title { get { return tab0Title.Value; } set { tab0Title.Value = value; } }

    M4uProperty<string> tab1Title = new M4uProperty<string>();
    public string Tab1Title { get { return tab1Title.Value; } set { tab1Title.Value = value; } }

    M4uProperty<bool> isActiveGetAll = new M4uProperty<bool>();
    public bool IsActiveGetAll { get { return isActiveGetAll.Value; } set { isActiveGetAll.Value = value; } }

    List<ScoreRewardContext> rewardList = new List<ScoreRewardContext>();
    public List<ScoreRewardContext> RewardList { get { return rewardList; } }

    List<ScoreRewardContext> viewItems = new List<ScoreRewardContext>();

    private bool m_Ready = false;
    private bool m_Show = false;
    public bool IsShow { get { return m_Show; } }

    public int DataCount
    {
        get
        {
            return RewardList.Count;
        }
    }

    private PacketStructUserScoreInfo scoreInfo = null;
    private List<PacketStructUserScoreReward> achieveList = new List<PacketStructUserScoreReward>();
    private System.Action<PacketStructUserScoreInfo> hideAction = null;

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        Title = GameTextUtil.GetText("scorereward_title");
        TabIndex = TAB_HI_SCORE;
        Tab0Title = GameTextUtil.GetText("scorereward_tub_01");
        Tab1Title = GameTextUtil.GetText("scorereward_tub_02");
        IsActiveGetAll = false;
    }

    private void Start()
    {
        
    }

    public void setup(PacketStructUserScoreInfo _info)
    {
        scoreInfo = _info;
        TabIndex = TAB_TOTAL_SCORE;
        updateRewardList();
    }

    private void updateRewardList()
    {
        if (scoreInfo == null) return;

        RewardList.Clear();
        achieveList.Clear();

        List<PacketStructUserScoreReward> viewList = new List<PacketStructUserScoreReward>();
        List<PacketStructUserScoreReward> getList = new List<PacketStructUserScoreReward>();

        if (scoreInfo.reward_list != null)
        {
            for (int i = 0; i < scoreInfo.reward_list.Length; i++)
            {
                if (scoreInfo.reward_list[i] == null)
                {
                    continue;
                }
                if (scoreInfo.reward_list[i].type - 1 != TabIndex)
                {
                    continue;
                }

                bool bAchieve = false;
                switch (scoreInfo.reward_list[i].type)
                {
                    case 1:
                        {
                            if(scoreInfo.hi_score >= scoreInfo.reward_list[i].score)
                            {
                                bAchieve = true;
                            }
                        }
                        break;
                    case 2:
                        {
                            if (scoreInfo.total_score >= scoreInfo.reward_list[i].score)
                            {
                                bAchieve = true;
                            }
                        }
                        break;
                }

                if (bAchieve)
                {
                    achieveList.Add(scoreInfo.reward_list[i]);
                }else
                {
                    viewList.Add(scoreInfo.reward_list[i]);
                }
            }
        }
        if (scoreInfo.get_list != null)
        {
            for (int i = 0; i < scoreInfo.get_list.Length; i++)
            {
                if (scoreInfo.get_list[i] == null)
                {
                    continue;
                }
                if (scoreInfo.get_list[i].type - 1 != TabIndex)
                {
                    continue;
                }
                getList.Add(scoreInfo.get_list[i]);
            }
        }

        IsActiveGetAll = false;

        if (achieveList.Count != 0)
        {
            achieveList.Sort((a, b) => a.score - b.score);
            for (int i = 0; i < achieveList.Count; i++)
            {
                ScoreRewardContext scoreReward = new ScoreRewardContext();
                scoreReward.setData(achieveList[i], ScoreRewardContext.REWARD_ACHIIEVE);
                RewardList.Add(scoreReward);
            }

            IsActiveGetAll = true;
        }
        if (viewList.Count != 0)
        {
            viewList.Sort((a, b) => a.score - b.score);
            for (int i = 0; i < viewList.Count; i++)
            {
                ScoreRewardContext scoreReward = new ScoreRewardContext();
                scoreReward.setData(viewList[i], ScoreRewardContext.REWARD_SHOW);
                RewardList.Add(scoreReward);
            }
        }
        if(getList.Count != 0)
        {
            getList.Sort((a, b) => a.score - b.score);
            for (int i = 0; i < getList.Count; i++)
            {
                ScoreRewardContext scoreReward = new ScoreRewardContext();
                scoreReward.setData(getList[i], ScoreRewardContext.REWARD_GET);
                RewardList.Add(scoreReward);
            }
        }
        scrollContent.Initialize(this);
    }

    public void Show( System.Action<PacketStructUserScoreInfo> action )
    {
        if (m_Show)
        {
            return;
        }
        m_Show = true;

        UnityUtil.SetObjectEnabledOnce(gameObject, true);

        hideAction = action;

        SetPositionAjustStatusBar(new Vector2(0, 6), new Vector2(-40, -247));

        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを登録
            AndroidBackKeyManager.Instance.StackPush(gameObject, OnClose);
        }

        setScrollTop();

        gameObject.transform.DOScaleY(WindowShowScale, AnimationTime)
        .OnUpdate(() =>
        {
            setScrollTop();
        })
        .OnComplete(() =>
        {
            setScrollTop();
            m_Ready = true;
        });
    }

    private void setScrollTop()
    {
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1.0f;
        }
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

        if (hideAction != null)
        {
            hideAction(scoreInfo);
        }

        gameObject.transform.DOScaleY(WindowHideScale, AnimationTime).OnComplete(() =>
        {
            RewardList.Clear();
            UnityUtil.SetObjectEnabledOnce(gameObject, false);
            m_Show = false;
        });
    }

    public void OnSelectTab( int index )
    {
        if (TabIndex == index)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        TabIndex = index;

        updateRewardList();
    }

    public void OnClose()
    {
        if (m_Ready == false)
        {
            return;
        }

        Hide();
    }

    public void OnSelectGetItem(ScoreRewardContext item)
    {
        if ( m_Ready == false ||
             ServerApi.IsExists )
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        int[] reward_ids = new int[1];
        reward_ids[0] = item.Reward.fix_id;
        SendGetScoreReward(reward_ids);
    }

    public void OnSelectGetAll()
    {
        if ( m_Ready == false ||
             ServerApi.IsExists ||
             achieveList.Count == 0)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        int[] reward_ids = new int[achieveList.Count];
        for(int i=0;i< achieveList.Count; i++)
        {
            reward_ids[i] = achieveList[i].fix_id;
        }
        SendGetScoreReward(reward_ids);
    }

    private void SendGetScoreReward( int[] reward_ids )
    {
        ServerDataUtilSend.SendPacketAPI_GetScoreReward(scoreInfo.event_id, reward_ids)
        .setSuccessAction(data =>
        {
            RecvGetScoreReward getReward = data.GetResult<RecvGetScoreReward>();
            if( getReward != null&&
                getReward.result!=null)
            {
                //スコア情報
                scoreInfo = getReward.result.score_info;

                //報酬リスト更新
                updateRewardList();

                //プレゼント更新
                if (getReward.result.present != null)
                {
                    UserDataAdmin.Instance.m_StructPresentList = UserDataAdmin.PresentListClipTimeLimit(getReward.result.present);
                }

                openGetScoreRewardDialog();
            }
        })
        .setErrorAction(data =>
        {

        })
        .SendStart();
    }

    private void openGetScoreRewardDialog()
    {
        Dialog dlg = Dialog.Create(DialogType.DialogOK);
        dlg.SetDialogText(DialogTextType.Title, "報酬受け取り成功");
        dlg.SetDialogText(DialogTextType.MainText, "報酬は運営BOXに送られました。");
        dlg.SetDialogText(DialogTextType.OKText, "閉じる");
        //dlg.DisableFadePanel();
        dlg.Show();
    }

    public float GetItemScale(int index)
    {
        return itemPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }

    public RectTransform GetItem(int index, RectTransform recyclableItem)
    {
        if (null == recyclableItem)
        {
            // 初回ロード時はinstantateItemCountで指定した回数分だけitemがnullで来るので、ここで生成してあげる
            // 以降はitemが再利用されるため、Reflesh()しない限りnullは来ない
            recyclableItem = Instantiate(itemPrefab).GetComponent<RectTransform>();

            ScoreRewardContext item = new ScoreRewardContext();
            item.DidSelectItem = OnSelectGetItem;

            recyclableItem.gameObject.GetComponent<M4uContextRoot>().Context = item;

            viewItems.Add(item);

            //位置調整
            RectTransform offset = recyclableItem.gameObject.GetComponent<ScoreRewardListItem>().Offset.GetComponent<RectTransform>();
            float posX = recyclableItem.sizeDelta.x * -1.0f;
            offset.anchoredPosition = new Vector2(posX, offset.anchoredPosition.y);
        }

        {
            if(RewardList.Count > index)
            {
                ScoreRewardContext item = recyclableItem.gameObject.GetComponent<ScoreRewardListItem>().Context;
                item.setup(RewardList[index].Reward, RewardList[index].RewardType);
                //Debug.LogWarning("ScoreReward index=" + index);
            }
        }

        return recyclableItem;
    }
}
