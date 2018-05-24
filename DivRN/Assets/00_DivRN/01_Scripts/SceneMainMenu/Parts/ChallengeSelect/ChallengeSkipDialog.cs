using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using DG.Tweening;

public class ChallengeSkipDialog : MenuPartsBase, IRecyclableItemsScrollContentDataProvider
{
    private static readonly float FadeShowAlpha = 0.5f;
    private static readonly float FadeHideAlpha = 0.0f;
    private static readonly float WindowShowScale = 1.0f;
    private static readonly float WindowHideScale = 0.0f;
    private static readonly float AnimationTime = 0.25f;

    private static readonly int MIN_LEVEL = 1;

    public MenuPartsBase WindowRoot = null;
    public MenuPartsBase Window = null;
    public GameObject ShadowPanel = null;

    public ScrollRect scrollRect = null;
    public RecyclableItemsScrollContent scrollContent = null;
    public GameObject itemPrefab = null;

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<string> levelLabel = new M4uProperty<string>();
    public string LevelLabel { get { return levelLabel.Value; } set { levelLabel.Value = value; } }

    M4uProperty<string> levelValue = new M4uProperty<string>();
    public string LevelValue { get { return levelValue.Value; } set { levelValue.Value = value; } }

    M4uProperty<string> ticketLabel = new M4uProperty<string>();
    public string TicketLabel { get { return ticketLabel.Value; } set { ticketLabel.Value = value; } }

    M4uProperty<string> ticketValue = new M4uProperty<string>();
    public string TicketValue { get { return ticketValue.Value; } set { ticketValue.Value = value; } }

    M4uProperty<string> skipLabel = new M4uProperty<string>();
    public string SkipLabel { get { return skipLabel.Value; } set { skipLabel.Value = value; } }

    M4uProperty<string> skipValue = new M4uProperty<string>();
    public string SkipValue { get { return skipValue.Value; } set { skipValue.Value = value; } }

    M4uProperty<string> dontSkipLabel = new M4uProperty<string>();
    public string DontSkipLabel { get { return dontSkipLabel.Value; } set { dontSkipLabel.Value = value; } }

    M4uProperty<bool> isActiveDontSkip = new M4uProperty<bool>();
    public bool IsActiveDontSkip { get { return isActiveDontSkip.Value; } set { isActiveDontSkip.Value = value; } }

    M4uProperty<bool> isActiveCancel = new M4uProperty<bool>();
    public bool IsActiveCancel { get { return isActiveCancel.Value; } set { isActiveCancel.Value = value; } }


    List<ChallengeSkipContext> skipList = new List<ChallengeSkipContext>();
    public List<ChallengeSkipContext> SkipList { get { return skipList; } }

    List<ChallengeSkipListItem> viewItems = new List<ChallengeSkipListItem>();

    private bool m_Ready = false;
    private bool m_Show = false;
    private int m_UseTicket = 0;
    private System.Action hideAction = null;

    private ChallengeSelect.EventData m_EventData = null;

    public int DataCount
    {
        get
        {
            return SkipList.Count;
        }
    }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
        Title = "";
        LevelLabel = "";
        TicketLabel = GameTextUtil.GetText("growth_boss_28");
        SkipLabel = GameTextUtil.GetText("growth_boss_29");
        DontSkipLabel = GameTextUtil.GetText("growth_boss_30");
    }

    public void setup(ChallengeSelect.EventData data)
    {
        m_EventData = data;
        IsActiveCancel = data.bSkip;
        updateSkipList();
    }

    private void updateSkipList()
    {
        int now_level = m_EventData.bSkip ? m_EventData.SkipLevel : m_EventData.info.challenge_level;
        int skip_level = m_EventData.eventMaster.skip_level;
        int max_level = (m_EventData.info.max_level <= m_EventData.eventMaster.skip_max ? m_EventData.info.max_level : m_EventData.eventMaster.skip_max);
        uint boss_chara_id = m_EventData.questMaster.boss_chara_id;

        int skip_cnt = m_EventData.info.skip_cnt;
        m_UseTicket = m_EventData.eventMaster.skip_base_ticket_num + (m_EventData.eventMaster.skip_stepup_ticket_num * skip_cnt);

        //基本情報
        LevelValue = string.Format("{0}", now_level);
        TicketValue = string.Format(GameTextUtil.GetText("growth_boss_32"), UserDataAdmin.Instance.m_StructPlayer.have_ticket);
        SkipValue = string.Format(GameTextUtil.GetText("growth_boss_32"), m_UseTicket);

        //リストクリア
        SkipList.Clear();

        int start_level = (max_level / skip_level) * skip_level;
        if (now_level == MIN_LEVEL &&
            now_level == start_level)
        {
            //選択できるレベルがない
            IsActiveDontSkip = true;
            return;
        }

        bool bSkipMinLV = false;

        if (start_level >= skip_level)
        {
            for (int level = start_level; level > 0; level -= skip_level)
            {
                //現在レベルにはスキップできない
                if (level == now_level)
                {
                    continue;
                }

                //カレントレベルにはスキップできない
                if (level == m_EventData.info.challenge_level)
                {
                    continue;
                }

                if (level == MIN_LEVEL)
                {
                    //最少LVが追加された
                    bSkipMinLV = true;
                }

                ChallengeSkipContext data = new ChallengeSkipContext();
                data.SetData(level);

                SkipList.Add(data);
            }
        }

        if (bSkipMinLV == false &&
            now_level != MIN_LEVEL &&
            m_EventData.info.challenge_level != MIN_LEVEL)
        {
            //最少LVには常にスキップできる
            ChallengeSkipContext data = new ChallengeSkipContext();
            data.SetData(MIN_LEVEL);

            SkipList.Add(data);
        }

        if (SkipList.Count == 0)
        {
            //選択できるレベルがない
            IsActiveDontSkip = true;
            return;
        }

        scrollContent.Initialize(this);
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

        WindowRoot.SetPositionAjustStatusBar(new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f));

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

        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        Hide();
    }

    public void OnSelectItem(ChallengeSkipContext data)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        m_EventData.bSkip = true;
        m_EventData.SkipLevel = data.SkipLevel;
        m_EventData.UseTicket = m_UseTicket;
        m_EventData.questMaster = MasterDataUtil.GetChallengeQuestMaster(m_EventData.eventMaster.event_id, data.SkipLevel);

        Hide();
    }

    public void OnCalcel()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        m_EventData.bSkip = false;
        m_EventData.SkipLevel = m_EventData.info.challenge_level;
        m_EventData.UseTicket = 0;
        m_EventData.questMaster = MasterDataUtil.GetChallengeQuestMaster(m_EventData.eventMaster.event_id, m_EventData.info.challenge_level);

        Hide();
    }

    public float GetItemScale(int index)
    {
        return itemPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private ChallengeSkipListItem GetViewListItem(int index)
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

    public RectTransform GetItem(int index, RectTransform recyclableItem)
    {
        if (null == recyclableItem)
        {
            // 初回ロード時はinstantateItemCountで指定した回数分だけitemがnullで来るので、ここで生成してあげる
            // 以降はitemが再利用されるため、Reflesh()しない限りnullは来ない
            recyclableItem = Instantiate(itemPrefab).GetComponent<RectTransform>();

            ChallengeSkipContext item = new ChallengeSkipContext();
            item.DidSelectItem = OnSelectItem;

            recyclableItem.gameObject.GetComponent<M4uContextRoot>().Context = item;

            ChallengeSkipListItem listItem = recyclableItem.gameObject.GetComponent<ChallengeSkipListItem>();

            viewItems.Add(listItem);

            //位置調整
            //RectTransform offset = recyclableItem.gameObject.GetComponent<ChallengeSkipListItem>().Offset;
            //float posX = recyclableItem.sizeDelta.x * -0.5f;
            //offset.anchoredPosition = new Vector2(posX, offset.anchoredPosition.y);
        }

        {
            if (SkipList.Count > index)
            {
                ChallengeSkipListItem listItem = recyclableItem.gameObject.GetComponent<ChallengeSkipListItem>();
                listItem.itemIndex = index;


                ChallengeSkipContext item = listItem.Context;
                item.Copy(SkipList[index]);
                item.setup();
            }
        }

        return recyclableItem;
    }

    /*-------------------------------------------------------------------------------------*/
    /*                                                                                     */
    /*                                                                                     */
    /*                                                                                     */
    /*-------------------------------------------------------------------------------------*/
    public static ChallengeSkipDialog Create(Camera camera)
    {
        GameObject _tmpObj = Resources.Load("Prefab/ChallengeSelect/ChallengeSkipDialog") as GameObject;
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

        ChallengeSkipDialog dlg = _newObj.GetComponent<ChallengeSkipDialog>();

        return dlg;
    }
}
