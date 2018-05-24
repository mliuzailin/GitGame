using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using DG.Tweening;
using System;
using ServerDataDefine;

public class ChallengeRewardDialog : MenuPartsBase, IRecyclableItemsScrollContentDataProvider
{
    private static readonly int TAB_REWARD_LIST = 0;
    private static readonly int TAB_REWARD_GET = 1;
    private static readonly float FadeShowAlpha = 0.5f;
    private static readonly float FadeHideAlpha = 0.0f;
    private static readonly float WindowShowScale = 1.0f;
    private static readonly float WindowHideScale = 0.0f;
    private static readonly float AnimationTime = 0.25f;

    public MenuPartsBase Window = null;
    public GameObject ShadowPanel = null;

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

    M4uProperty<bool> isViewGetAll = new M4uProperty<bool>();
    public bool IsViewGetAll { get { return isViewGetAll.Value; } set { isViewGetAll.Value = value; } }

    M4uProperty<bool> isViewFlag = new M4uProperty<bool>();
    public bool IsViewFlag { get { return isViewFlag.Value; } set { isViewFlag.Value = value; } }

    List<ChallengeRewardContext>[] rewardList = new List<ChallengeRewardContext>[2];
    public List<ChallengeRewardContext> RewardList { get { return rewardList[TabIndex]; } }

    List<ChallengeRewardListItem> viewItems = new List<ChallengeRewardListItem>();

    private PacketStructChallengeInfo m_ChallengeInfo = null;
    private bool m_Ready = false;
    private bool m_Show = false;
    private System.Action hideAction = null;

    public int DataCount
    {
        get
        {
            return RewardList.Count;
        }
    }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;

        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを登録
            AndroidBackKeyManager.Instance.StackPush(gameObject, OnClose);
        }

        rewardList[0] = new List<ChallengeRewardContext>();
        rewardList[1] = new List<ChallengeRewardContext>();

        IsViewFlag = false;
    }

    public void setCamera(Camera camera)
    {
        GetComponentInChildren<Canvas>().worldCamera = camera;
    }

    // Use this for initialization
    void Start()
    {
        Title = GameTextUtil.GetText("growth_boss_15");
        Tab0Title = GameTextUtil.GetText("growth_boss_16");
        Tab1Title = GameTextUtil.GetText("growth_boss_17");
        TabIndex = 0;
        IsViewGetAll = false;
        IsActiveGetAll = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setup(PacketStructChallengeInfo info)
    {
        m_ChallengeInfo = info;
        updateRewardList();
    }

    private void updateRewardList()
    {
        //報酬リスト更新
        updateInfoRewardList(m_ChallengeInfo.reward_list);

        //受取リスト更新
        updateGetRewardList(m_ChallengeInfo.get_list);

        //リスト更新
        scrollContent.Initialize(this);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="infoRewards"></param>
    private void updateInfoRewardList(PacketStructChallengeInfoReward[] infoRewards)
    {
        if (infoRewards != null ||
            infoRewards.Length == 0)
        {
            rewardList[TAB_REWARD_LIST].Clear();

            int mode = 0;
            for (int i = 0; i < infoRewards.Length; i++)
            {
                ChallengeRewardContext newData = new ChallengeRewardContext();
                newData.SetData(infoRewards[i]);
                switch (mode)
                {
                    case 0:
                        {
                            if (infoRewards[i].type < 4)
                            {
                                newData.SetTitleBar(GameTextUtil.GetText("growth_boss_18"));
                                mode = 1;
                            }
                            else
                            {
                                newData.SetTitleBar(GameTextUtil.GetText("growth_boss_19"));
                                mode = 2;
                            }
                        }
                        break;
                    case 1:
                        {
                            if (infoRewards[i].type >= 4)
                            {
                                newData.SetTitleBar(GameTextUtil.GetText("growth_boss_19"));
                                mode = 2;
                            }
                        }
                        break;
                    default:
                        break;
                }

                rewardList[TAB_REWARD_LIST].Add(newData);
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="getRewards"></param>
    private void updateGetRewardList(PacketStructChallengeGetReward[] getRewards)
    {
        if (getRewards != null ||
            getRewards.Length == 0)
        {
            IsViewFlag = false;
            rewardList[TAB_REWARD_GET].Clear();

            for (int i = 0; i < getRewards.Length; i++)
            {
                MasterDataChallengeReward master = MasterFinder<MasterDataChallengeReward>.Instance.Find(getRewards[i].fix_id);
                if (master == null)
                {
                    continue;
                }

                ChallengeRewardContext newData = new ChallengeRewardContext();


                newData.SetData(getRewards[i], master);

                rewardList[TAB_REWARD_GET].Add(newData);
            }

            if (rewardList[TAB_REWARD_GET].Count != 0)
            {
                IsViewFlag = true;
            }
        }
    }

    public void Show(System.Action _hideAction = null)
    {
        if (m_Show)
        {
            return;
        }
        m_Show = true;

        hideAction = _hideAction;

        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを設定
            AndroidBackKeyManager.Instance.StackPush(gameObject, OnClose);
        }

        Window.SetPositionAjustStatusBar(new Vector2(0.0f, 20.0f), new Vector2(-40.0f, -155.0f));

        ShadowPanel.GetComponent<Image>().DOFade(FadeShowAlpha, AnimationTime);

        setScrollTop();

        Window.transform.DOScaleY(WindowShowScale, AnimationTime)
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

        ShadowPanel.GetComponent<Image>().DOFade(FadeHideAlpha, AnimationTime);

        Window.transform.DOScaleY(WindowHideScale, AnimationTime).OnComplete(() =>
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

    public void OnSelectTab(int index)
    {
        if (TabIndex == index)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        TabIndex = index;

        switch (TabIndex)
        {
            case 0:
                IsViewGetAll = false;
                break;
            case 1:
                {
                    IsViewGetAll = true;
                    IsActiveGetAll = (RewardList.Count != 0) ? true : false;
                }
                break;
        }

        setScrollTop();
        scrollContent.resetCurrtent();
        scrollContent.Initialize(this);
    }

    public void OnSelectItem(ChallengeRewardContext data)
    {
        if (data.GetReward == null)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        int event_id = m_ChallengeInfo.event_id;
        int[] reward_ids = new int[1];
        reward_ids[0] = data.GetReward.fix_id;
        int[] loop_cnts = new int[1];
        loop_cnts[0] = data.GetReward.loop_count;

        SendGetChallengeReward(event_id, reward_ids, loop_cnts);
    }

    public void OnSelectGetAll()
    {
        if (TabIndex != 1 ||
            RewardList.Count == 0)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        int event_id = m_ChallengeInfo.event_id;
        int[] reward_ids = new int[RewardList.Count];
        int[] loop_cnts = new int[RewardList.Count];
        for (int i = 0; i < RewardList.Count; i++)
        {
            reward_ids[i] = RewardList[i].GetReward.fix_id;
            loop_cnts[i] = RewardList[i].GetReward.loop_count;
        }

        SendGetChallengeReward(event_id, reward_ids, loop_cnts);
    }

    private void SendGetChallengeReward(int event_id, int[] reward_ids, int[] loop_cnts)
    {
        ServerDataUtilSend.SendPacketAPI_GetChallengeReward(event_id, reward_ids, loop_cnts)
        .setSuccessAction((data) =>
        {
            RecvGetChallengeRewardValue result = data.GetResult<RecvGetChallengeReward>().result;
            if (result != null)
            {
                //プレゼント情報更新
                UserDataAdmin.Instance.m_StructPresentList = UserDataAdmin.PresentListClipTimeLimit(result.present);

                //受取報酬リスト更新
                m_ChallengeInfo.get_list = result.get_list;
                updateGetRewardList(result.get_list);

                scrollContent.Initialize(this);

                openGetRewardSuccesDialog();
            }
        })
        .setErrorAction((data) =>
        {

        })
        .SendStart();
    }

    private void openGetRewardSuccesDialog()
    {
        Dialog dlg = Dialog.Create(DialogType.DialogOK);
        dlg.SetDialogTextFromTextkey(DialogTextType.Title, "growth_boss_20");
        dlg.SetDialogTextFromTextkey(DialogTextType.MainText, "growth_boss_21");
        dlg.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        dlg.Show();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public float GetItemScale(int index)
    {
        if (RewardList[index].IsActiveTitleBar)
        {
            return 164.0f;
        }
        else
        {
            return 120.0f;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private ChallengeRewardListItem GetViewListItem(int index)
    {
        for (int i = 0; i < viewItems.Count; i++)
        {
            if (viewItems[i].itemIndex == index)
            {
                return viewItems[i];
            }
        }
        return null;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="index"></param>
    /// <param name="recyclableItem"></param>
    /// <returns></returns>
    public RectTransform GetItem(int index, RectTransform recyclableItem)
    {
        if (null == recyclableItem)
        {
            // 初回ロード時はinstantateItemCountで指定した回数分だけitemがnullで来るので、ここで生成してあげる
            // 以降はitemが再利用されるため、Reflesh()しない限りnullは来ない
            recyclableItem = Instantiate(itemPrefab).GetComponent<RectTransform>();

            ChallengeRewardContext item = new ChallengeRewardContext();
            item.DidSelectItem = OnSelectItem;

            recyclableItem.gameObject.GetComponent<M4uContextRoot>().Context = item;

            ChallengeRewardListItem listItem = recyclableItem.gameObject.GetComponent<ChallengeRewardListItem>();

            viewItems.Add(listItem);

            //位置調整
            RectTransform offset = recyclableItem.gameObject.GetComponent<ChallengeRewardListItem>().Offset;
            float posX = recyclableItem.sizeDelta.x * -0.5f;
            offset.anchoredPosition = new Vector2(posX, offset.anchoredPosition.y);
        }

        {
            if (RewardList.Count > index)
            {
                ChallengeRewardListItem listItem = recyclableItem.gameObject.GetComponent<ChallengeRewardListItem>();
                listItem.itemIndex = index;


                ChallengeRewardContext item = listItem.Context;
                item.CopyData(RewardList[index]);
                switch (TabIndex)
                {
                    case 0:
                        item.setupInfo(m_ChallengeInfo);
                        break;
                    case 1:
                        item.setupGet(ChallengeRewardContext.REWARD_ACHIIEVE);
                        break;
                }
            }
        }

        return recyclableItem;
    }

    /*-------------------------------------------------------------------------------------*/
    /*                                                                                     */
    /*                                                                                     */
    /*                                                                                     */
    /*-------------------------------------------------------------------------------------*/
    public static ChallengeRewardDialog Create(Camera camera)
    {
        GameObject _tmpObj = Resources.Load("Prefab/ChallengeSelect/ChallengeRewardDialog") as GameObject;
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

        _newObj.GetComponentInChildren<Canvas>().worldCamera = camera;

        ChallengeRewardDialog dlg = _newObj.GetComponent<ChallengeRewardDialog>();

        return dlg;
    }
}
