using System;
using System.Linq;
using ServerDataDefine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using Prime31;
using UnityEngine.UI;
using TMPro;


public class Mission : MenuPartsBase
{
    [SerializeField]
    GameObject m_FilterButtonRoot;
    public GameObject recevieAllButton;
    public GameObject recevieAllButton_off;
    public GameObject filterButton;
    public TextMeshProUGUI receivedItemDisplayLimtText;

    public MissionPager pager;

    public SortDialog.MissionFilterInfo m_FilterInfo;

    M4uProperty<string> emptyLabel = new M4uProperty<string>();
    public string EmptyLabel { get { return emptyLabel.Value; } set { emptyLabel.Value = value; } }

    private MissionGroup FindMissionGroup(ACHIEVEMENT_CATEGORY_TYPE t)
    {
        return MissionGroupList.FirstOrDefault(g => g.missionGroupType == t);
    }

    public void SetBadgeNum(int num01, int num02, int num03)
    {
        FindMissionGroup(ACHIEVEMENT_CATEGORY_TYPE.NORMAL).SetBadge(num01);
        FindMissionGroup(ACHIEVEMENT_CATEGORY_TYPE.EVENT).SetBadge(num02);
        FindMissionGroup(ACHIEVEMENT_CATEGORY_TYPE.DAILY).SetBadge(num03);
        //メニュー用フラグ更新
        bool bNormal = (num01 != 0) ? true : false;
        UserDataAdmin.Instance.SetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionNormal, bNormal);
        bool bEvent = (num02 != 0) ? true : false;
        UserDataAdmin.Instance.SetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionEvent, bEvent);
        bool bDaily = (num03 != 0) ? true : false;
        UserDataAdmin.Instance.SetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionDaily, bDaily);
    }


    private List<MissionGroup> MissionGroupList
    {
        get
        {
            return transform.GetComponentsInChildren<MissionGroup>().ToList();
        }
    }

    public MissionGroup CurrentMissionGroup
    {
        get;
        set;
    }


    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    void Start()
    {
    }

    public void Initialize()
    {
        m_FilterInfo = new SortDialog.MissionFilterInfo();
        m_FilterInfo.InitParam();

        MissionGroupList.ForEach(g => g.Deactivate());
        ACHIEVEMENT_CATEGORY_TYPE missionGroupType = (CurrentMissionGroup == null) ? ACHIEVEMENT_CATEGORY_TYPE.DAILY : CurrentMissionGroup.missionGroupType;

        if (MainMenuParam.m_AchievementShowData != null)
        {
            //アチーブメント指定がある場合
            missionGroupType = (ACHIEVEMENT_CATEGORY_TYPE)MainMenuParam.m_AchievementShowData.achievement_category_id;
            MainMenuParam.m_AchievementShowData = null;
        }
        else
        {
            //達成したアチーブがあるページを指定
            if (UserDataAdmin.Instance.GetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionDaily) == true)
            {
                missionGroupType = ACHIEVEMENT_CATEGORY_TYPE.DAILY;
            }
            else if (UserDataAdmin.Instance.GetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionEvent) == true)
            {
                missionGroupType = ACHIEVEMENT_CATEGORY_TYPE.EVENT;
            }
            else if (UserDataAdmin.Instance.GetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionNormal) == true)
            {
                missionGroupType = ACHIEVEMENT_CATEGORY_TYPE.NORMAL;
            }

        }
        MissionGroup missionGroup = FindMissionGroup(missionGroupType);
        missionGroup.SetSelectTab();
        missionGroup.Activate(1);

        SetUpButtons();
    }

    private void SetUpButtons()
    {
        var filterButton = new ButtonModel();
        MissionFilterButton.Attach(m_FilterButtonRoot).SetModel(filterButton);
        filterButton.OnClicked += () =>
        {
            OnClickFilterButton();
        };

        filterButton.Appear();
        filterButton.SkipAppearing();
    }

    //一括受取ボタン
    public void OnClickReceiveAllButton()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALLOnClickReceiveAllButton");
#endif

        if (GlobalMenuManager.Instance.IsPageClosing() == true)
        {
            return;
        }

        //連打防止
        if (ServerApi.IsExists)
        {
            return;
        }

        if (GlobalMenuManager.Instance.IsCangeTime())
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        List<MasterDataAchievementConverted> tmpOpenCollection = CurrentMissionGroup.Collection.FindAll((v) => v.IsState_Achieve || v.IsState_AchieveEffect);
        List<MasterDataAchievementConverted> openCollection = new List<MasterDataAchievementConverted>();

        //--------------------------------------
        // フィルター設定に該当するミッションを抽出する
        //--------------------------------------
        MasterDataDefineLabel.AchievementReceiveType filterType = MasterDataDefineLabel.AchievementReceiveType.NONE;
        if (m_FilterInfo != null)
        {
            filterType = m_FilterInfo.m_receive_type;
        }
        for (int i = 0; i < tmpOpenCollection.Count; ++i)
        {
            if (MasterDataUtil.CheckReceivePresentType(tmpOpenCollection[i].present_ids, filterType) == true)
            {
                openCollection.Add(tmpOpenCollection[i]);
            }
        }

        uint[] present = openCollection.Select((v) => v.fix_id).ToArray();  // 現在のタブで、報酬があるアチーブメントの配列

        // リクエスト
        ServerDataUtilSend.SendPacketAPI_AchievementOpen(present, new uint[]
            {
                (uint) CurrentMissionGroup.missionGroupType
            }).
            setSuccessAction((_data) =>
            {
                //ユーザー情報更新
                UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvOpenAchievement>(UserDataAdmin.Instance.m_StructPlayer);
                UserDataAdmin.Instance.ConvertPartyAssing();

                RecvOpenAchievementValue result = _data.GetResult<RecvOpenAchievement>().result;
                MissionAllOpenResultMessage(result);
#if BUILD_TYPE_DEBUG
                Debug.Log("FINISH:" + result.achievement_opened);
#endif
                CurrentMissionGroup.SetEnaleReceiveButtonAll(false); // 開封したミッションリストのボタンを押せなくする
                CurrentMissionGroup.Activate();

                // 開封した新規達成アチーブメントを削除
                ResidentParam.DelAchievementClear(result.achievement_opened);

                MainMenuManager.Instance.UpdateUserStatusFromGlobalMenu();
            }).
            setErrorAction((_data) =>
            {
                Debug.LogError("ERROR:" + _data.m_PacketCode);
                CurrentMissionGroup.SetEnaleReceiveButtonAll(true);
            }).
            SendStart();
    }

    /// <summary>
    /// 全部受け取りした時のメッセージ
    /// </summary>
    void MissionAllOpenResultMessage(RecvOpenAchievementValue result)
    {
        if (result == null) { return; }

        //--------------------------------------------------------------
        // 受取エラーの数を取得
        // 1: 受け取りアイテム（ユニット）が上限に到達　2: クエストキー期限切れ　３：開封有効期限切れ
        //--------------------------------------------------------------
        int error_num_1 = 0;
        int error_num_2 = 0;
        int error_num_3 = 0;
        if (result.error != null)
        {
            for (int i = 0; i < result.error.Length; ++i)
            {
                switch ((PRESENT_OPEN_ERROR_TYPE)result.error[i])
                {
                    case PRESENT_OPEN_ERROR_TYPE.COUNT_LIMIT:
                        ++error_num_1;
                        break;
                    case PRESENT_OPEN_ERROR_TYPE.QUEST_KEY_EXPIRED:
                        ++error_num_2;
                        break;
                    case PRESENT_OPEN_ERROR_TYPE.RECEIVE_EXPIRED:
                        ++error_num_3;
                        break;
                }
            }
        }

        //--------------------------------------
        // エラーテキストの取得
        //--------------------------------------
        string error_msg_1 = ""; // 所持上限テキスト
        string error_msg_2 = ""; // 有効期限切れテキスト
        string error_msg_3 = ""; // 受取期限超過テキスト
        if (error_num_1 > 0)
        {
            error_msg_1 = Environment.NewLine + string.Format(GameTextUtil.GetText("mt36q_content1"), error_num_1) + Environment.NewLine;
        }
        if (error_num_2 > 0)
        {
            error_msg_2 = Environment.NewLine + string.Format(GameTextUtil.GetText("mt36q_content2"), error_num_2) + Environment.NewLine;
        }
        if (error_num_3 > 0)
        {
            error_msg_3 = Environment.NewLine + string.Format(GameTextUtil.GetText("mt36q_content3"), error_num_3) + Environment.NewLine;
        }

        int rewardOpenCount = (result.achievement_opened != null) ? result.achievement_opened.Length : 0; // 開封個数

        //--------------------------------------
        // ダイアログの表示
        //--------------------------------------
        if (rewardOpenCount > 0)
        {
            // n個のプレゼントを受け取りました。
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("mm36q_title"));
            newDialog.SetDialogText(DialogTextType.MainText, string.Format(GameTextUtil.GetText("mm36q_content"), rewardOpenCount, error_msg_1, error_msg_2, error_msg_3));
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.Show();
        }
        else
        {
            // 全て受け取れませんでした。
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("mm36q_title"));
            newDialog.SetDialogText(DialogTextType.MainText, string.Format(GameTextUtil.GetText("mt36q_content0"), error_msg_1, error_msg_3));
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.Show();
        }
    }

    public void OnClickFilterButton()
    {
        if (GlobalMenuManager.Instance.IsPageClosing() == true)
        {
            return;
        }

        if (ServerApi.IsExists)
        {
            return;
        }

        if (SortDialog.IsExists == true)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        SortDialog dialog = SortDialog.Create();
        dialog.SetDialogType(SortDialog.DIALOG_TYPE.MISSION);
        dialog.m_MissionFilterData = m_FilterInfo.Clone();
        dialog.OnCloseMissionSortAction = OnClickSortCloseButton;
    }

    void OnClickSortCloseButton(SortDialog.MissionFilterInfo sortInfo)
    {
        SortDialog.MissionFilterInfo prevFilterInfo = (m_FilterInfo != null) ? m_FilterInfo.Clone() : new SortDialog.MissionFilterInfo();
        if (sortInfo != null)
        {
            m_FilterInfo = sortInfo.Clone();
        }

        if (CurrentMissionGroup != null)
        {
            if (prevFilterInfo.m_filter_type != sortInfo.m_filter_type)
            {
                CurrentMissionGroup.Deactivate();
                CurrentMissionGroup.Activate(1);
            }

            if (prevFilterInfo.m_receive_type != sortInfo.m_receive_type)
            {
                CurrentMissionGroup.UpdateGetReword();
            }
        }
    }
}
