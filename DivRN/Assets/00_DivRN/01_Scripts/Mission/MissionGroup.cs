using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.Serialization;
using ServerDataDefine;
using UnityEngine.UI;
using M4u;
using Prime31;
using TMPro;

public class MissionGroup : M4uContextMonoBehaviour
{
    private static uint REWARDED_COUNT_MAX = 30;

    [SerializeField]
    MissionTab m_MissionTab;

    public ACHIEVEMENT_CATEGORY_TYPE missionGroupType;
    public TextMeshProUGUI emptyMessage;

    public Mission mission;

    private bool m_isReword = false;

    private MissionPager Pager
    {
        get
        {
            return mission.pager;
        }
    }

    public M4uProperty<List<MasterDataAchievementConverted>> collection = new M4uProperty<List<MasterDataAchievementConverted>>();

    public List<MasterDataAchievementConverted> Collection
    {
        get
        {
            return collection.Value;
        }
        set
        {
            collection.Value = value;
        }
    }


    public bool IsActive
    {
        get
        {
            return emptyMessage.enabled || Collection.Count > 0;
        }
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        Collection = new List<MasterDataAchievementConverted>();
        emptyMessage.enabled = false;

        //ページ表示初期化
        Pager.Refresh(0, 0);
    }

    void Start()
    {
        string titleKey = "";
        switch (missionGroupType)
        {
            case ACHIEVEMENT_CATEGORY_TYPE.NORMAL:
                titleKey = "mission_tab3";
                break;
            case ACHIEVEMENT_CATEGORY_TYPE.EVENT:
                titleKey = "mission_tab2";
                break;
            case ACHIEVEMENT_CATEGORY_TYPE.DAILY:
                titleKey = "mission_tab1";
                break;
            case ACHIEVEMENT_CATEGORY_TYPE.QUEST:
                titleKey = "mission_tab4";
                break;
            case ACHIEVEMENT_CATEGORY_TYPE.REWARDED:
                titleKey = "mission_tab4";
                break;
        }

        if (m_MissionTab != null && titleKey.IsNullOrEmpty() == false)
        {
            m_MissionTab.TitleText = GameTextUtil.GetText(titleKey);
        }
    }

    public void Prev(Action finishAction = null)
    {
        if (GlobalMenuManager.Instance.IsPageClosing() == true)
        {
            return;
        }

        //連打防止
        if (ServerApi.IsExists)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);

        Activate(Pager.PageNo - 1, () =>
        {
            finishAction();
        });
    }

    public void Next(Action finishAction = null)
    {
        if (GlobalMenuManager.Instance.IsPageClosing() == true)
        {
            return;
        }

        //連打防止
        if (ServerApi.IsExists)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);

        Activate(Pager.PageNo + 1, () =>
        {
            finishAction();
        });
    }

    public void OnSelectTab(bool focus)
    {
        if (!focus)
        {
            return;
        }
        if (GlobalMenuManager.Instance.IsPageClosing() == true)
        {
            return;
        }

#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnSelectTab:" + missionGroupType);
#endif
        //選択中の場合
        if (mission.CurrentMissionGroup != null && mission.CurrentMissionGroup.Equals(this))
        {
            return;
        }
        //連打防止
        if (ServerApi.IsExists)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);


        if (mission.CurrentMissionGroup != null)
        {
            mission.CurrentMissionGroup.Deactivate();
        }
        Activate(1);
    }

    public void SetBadge(int num)
    {
        if (m_MissionTab != null && m_MissionTab.m_MissionBadge != null)
        {
            m_MissionTab.m_MissionBadge.Set(num);
        }
    }

    public void SetSelectTab()
    {
        if (m_MissionTab != null && m_MissionTab.m_Toggle != null)
        {
            m_MissionTab.m_Toggle.isOn = true;
        }
    }

    public void Activate(int pageNo = 1, Action finishAction = null)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("Call MissionGroup#Activate:" + missionGroupType + ":" + pageNo);
#endif
        if (missionGroupType == ACHIEVEMENT_CATEGORY_TYPE.REWARDED)
        {
            mission.filterButton.SetActive(false);
            mission.receivedItemDisplayLimtText.gameObject.SetActive(true);
            mission.receivedItemDisplayLimtText.text = GameTextUtil.GetText("RECEIVED_ITEM_DISPLAY_LIMIT");
        }
        else
        {
            mission.filterButton.SetActive(true);
            mission.receivedItemDisplayLimtText.gameObject.SetActive(false);
        }

        Pager.onClickLeftArrow = () =>
        {
            Prev();
        };
        Pager.onClickRightArrow = () =>
        {
            Next();
        };

        //---------------------------------------
        // 一括受け取りボタンの表示設定
        //---------------------------------------
        if (CheckViewRecevieAllButtonType(missionGroupType) == false)
        {
            SetEnableRecevieAllButton(MasterDataDefineLabel.BoolType.NONE);
        }
        else
        {
            SetEnableRecevieAllButton((m_isReword) ? MasterDataDefineLabel.BoolType.ENABLE : MasterDataDefineLabel.BoolType.DISABLE);
        }

        uint p = (uint)missionGroupType;
        if (missionGroupType == ACHIEVEMENT_CATEGORY_TYPE.REWARDED)
        {
            p = (uint)ACHIEVEMENT_CATEGORY_TYPE.NORMAL;
        }

        uint filterType = (uint)MasterDataDefineLabel.AchievementFilterType.ALL;
        if (mission.m_FilterInfo != null)
        {
            filterType = (uint)mission.m_FilterInfo.m_filter_type;
        }

        StartCoroutine(WaitSentStart(() =>
        {
            ServerDataUtilSend.SendPacketAPI_GetMasterDataAchievement(p, (uint)pageNo, 0, 0, filterType).
                setSuccessAction(_data =>
                {
#if BUILD_TYPE_DEBUG
                    Debug.Log("FINISH");
#endif
                    mission.CurrentMissionGroup = this;
                    RecvMasterDataAchievementValue response = (RecvMasterDataAchievementValue)_data.GetResult<RecvMasterDataAchievement>().result;

                    if (missionGroupType == ACHIEVEMENT_CATEGORY_TYPE.REWARDED)
                    {
                        Collection = response.clear_array_achievement.ToList();
                    }
                    else
                    {
                        Collection = response.master_array_achievement.ToList();
                    }

                    UpdateGetReword();

                    //タブについているバッジの更新
                    mission.SetBadgeNum(response.achievement_category_clear_num.clear_num_1, response.achievement_category_clear_num.clear_num_2, response.achievement_category_clear_num.clear_num_3);

                    if (Collection.Count == 0)
                    {
                        emptyMessage.enabled = true;
                        Pager.Refresh(0, 0);
                        return;
                    }

                    //Pagerの更新
                    if (missionGroupType == ACHIEVEMENT_CATEGORY_TYPE.REWARDED)
                    {
                        Pager.Refresh(pageNo, REWARDED_COUNT_MAX);
                    }
                    else
                    {
                        Pager.Refresh(pageNo, response.achievement_all_num);
                    }
                }).
                setErrorAction(_data =>
                {
                    Debug.LogError("ERROR:" + _data.m_PacketCode);
                    SetEnaleReceiveButtonAll(true);
                }).
                SendStart();
        }));
    }

    /// <summary>
    /// 通信を待機する
    /// </summary>
    /// <param name="finishAction"></param>
    /// <returns></returns>
    IEnumerator WaitSentStart(Action finishAction)
    {
        while (MainMenuManager.Instance.patchUpdateRequestStep == EMAINMENU_PATCHUPDATE_REQ.WAIT)
        {
            yield return null;
        }

        if (finishAction != null)
        {
            finishAction();
        }
    }

    /// <summary>
    /// 一括受け取りボタンのON/OFF状態の更新
    /// </summary>
    public void UpdateGetReword()
    {
        m_isReword = IsGetReword();
        if (CheckViewRecevieAllButtonType(missionGroupType) == true)
        {
            SetEnableRecevieAllButton((m_isReword) ? MasterDataDefineLabel.BoolType.ENABLE : MasterDataDefineLabel.BoolType.DISABLE);
        }
        else
        {
            SetEnableRecevieAllButton(MasterDataDefineLabel.BoolType.NONE);
        }
    }

    /// <summary>
    /// 一括受け取りボタンのON/OFF状態の設定
    /// </summary>
    /// <param name="boolType"></param>
    public void SetEnableRecevieAllButton(MasterDataDefineLabel.BoolType boolType)
    {
        switch (boolType)
        {
            case MasterDataDefineLabel.BoolType.DISABLE:
                // 押せない
                mission.recevieAllButton.gameObject.SetActive(false);
                mission.recevieAllButton_off.gameObject.SetActive(true);
                break;
            case MasterDataDefineLabel.BoolType.ENABLE:
                // 押せる
                mission.recevieAllButton.gameObject.SetActive(true);
                mission.recevieAllButton_off.gameObject.SetActive(false);
                break;
            default:
                // 非表示にする
                mission.recevieAllButton.gameObject.SetActive(false);
                mission.recevieAllButton_off.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// リストに一括受け取り可能なものがあるかチェックする
    /// </summary>
    /// <returns></returns>
    public bool IsGetReword()
    {
        if (Collection.IsNullOrEmpty())
        {
            return false;
        }
        MasterDataDefineLabel.AchievementReceiveType receiveType = MasterDataDefineLabel.AchievementReceiveType.NONE;
        if (mission.m_FilterInfo != null)
        {
            receiveType = mission.m_FilterInfo.m_receive_type;
        }

        for (int index = 0; index < Collection.Count; index++)
        {
            if ((Collection[index].IsState_Achieve || Collection[index].IsState_AchieveEffect) == false)
            {
                continue;
            }
            if (MasterDataUtil.CheckReceivePresentType(Collection[index].present_ids, receiveType) == false)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    public void Deactivate()
    {
        emptyMessage.enabled = false;
        Pager.Refresh(0, 0); //ページ表示初期化
        Collection.Clear();
    }

    private GameObject CollectionGO
    {
        get
        {
            return transform.GetChild(0).gameObject;
        }
    }

    public void SetEnaleReceiveButtonAll(bool isEnale)
    {
        MissionListItem[] items = CollectionGO.GetComponentsInChildren<MissionListItem>();
        if (items == null)
        {
            return;
        }
        Array.ConvertAll(items, (v) => v.ReceiveButton.interactable = isEnale);
    }

    /// <summary>
    /// 一括受け取りボタンを表示するミッションタイプか調べる
    /// </summary>
    /// <param name="type"></param>
    /// <returns>true:表示 false:非表示</returns>
    public static bool CheckViewRecevieAllButtonType(ACHIEVEMENT_CATEGORY_TYPE type)
    {
        return (type != ACHIEVEMENT_CATEGORY_TYPE.REWARDED
            && type != ACHIEVEMENT_CATEGORY_TYPE.QUEST);
    }
}
