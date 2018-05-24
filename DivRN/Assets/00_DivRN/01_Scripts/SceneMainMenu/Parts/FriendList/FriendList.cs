using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using System;

public class FriendList : MenuPartsBase, IRecyclableItemsScrollContentDataProvider
{
    public Action OnClickSortButtonAction = delegate { };
    public Action OnClickReloadButtonAction = delegate { };
    [SerializeField]
    RecyclableItemsScrollContent scrollContent = null;
    [SerializeField]
    GameObject itemPrefab = null;
    [SerializeField]
    ScrollRect scrollRect = null;

    //[SerializeField]
    //private GameObject m_switchButtonRoot;
    [SerializeField]
    private GameObject m_sortButtonRoot;
    [SerializeField]
    private GameObject m_reloadButtonRoot;

    private static readonly string SwitchButtonPrefabPath = "Prefab/QuestDetail/QuestDetailSwitchButton";
    private static readonly string SortButtonPrefabPath = "Prefab/FriendList/FriendListSortButton";
    private static readonly string ReloadButtonPrefabPath = "Prefab/FriendList/FriendReloadButton";

    SortList<FriendDataSetting> friendBaseList = new SortList<FriendDataSetting>();
    public SortList<FriendDataSetting> FriendBaseList { get { return friendBaseList; } }

    List<FriendDataSetting> friendDataList = new List<FriendDataSetting>();
    //public List<FriendDataSetting> FriendDataList { get { return friendDataList; } set { friendDataList = value; } }

    private List<FriendDataItem> itemList = new List<FriendDataItem>();

    M4uProperty<bool> isViewParam = new M4uProperty<bool>();
    public bool IsViewParam { get { return isViewParam.Value; } set { isViewParam.Value = value; } }

    M4uProperty<string> paramValue = new M4uProperty<string>();
    public string ParamValue { get { return paramValue.Value; } set { paramValue.Value = value; } }

    M4uProperty<bool> isViewEmpty = new M4uProperty<bool>();
    public bool IsViewEmpty { get { return isViewEmpty.Value; } set { isViewEmpty.Value = value; } }

    M4uProperty<string> emptyLabel = new M4uProperty<string>();
    public string EmptyLabel { get { return emptyLabel.Value; } set { emptyLabel.Value = value; } }

    M4uProperty<string> statusTypeText = new M4uProperty<string>();
    /// <summary>ステータスの種類表示</summary>
    public string StatusTypeText { get { return statusTypeText.Value; } set { statusTypeText.Value = value; } }

    M4uProperty<string> filterText = new M4uProperty<string>();
    /// <summary>絞り込み表示</summary>
    public string FilterText { get { return filterText.Value; } set { filterText.Value = value; } }

    M4uProperty<bool> isViewReload = new M4uProperty<bool>();
    public bool IsViewReload { get { return isViewReload.Value; } set { isViewReload.Value = value; } }

    public bool CheckLock { get; set; }

    private FriendListSortButton m_SortButton;
    LocalSaveSortInfo m_SortInfo;

    public FriendReloadButton m_ReloadButton;

    public int DataCount
    {
        get
        {
            return friendDataList.Count;
        }
    }

    private FriendDataItem.ParamType m_SelectParamType = FriendDataItem.ParamType.NAME;

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        IsViewParam = false;
        IsViewEmpty = false;
        IsViewReload = false;
        ParamValue = "";
        EmptyLabel = GameTextUtil.GetText("friend_not_list");
        CheckLock = false;
    }

    public void Init()
    {
        //ソート＆フィルタ実行
        friendDataList = friendBaseList.Exec(SORT_OBJECT_TYPE.FRIEND_LIST);
        IsViewEmpty = (friendDataList.Count == 0) ? true : false;

        if (scrollContent != null)
        {
            scrollContent.Initialize(this);
        }

        SetUpButtons();
        SetStatusType(m_SortInfo);
        SetFilterText(m_SortInfo);
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void OnChengeInfo()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        switch (m_SelectParamType)
        {
            case FriendDataItem.ParamType.NAME:
                m_SelectParamType = FriendDataItem.ParamType.SKILL;
                break;
            case FriendDataItem.ParamType.SKILL:
                m_SelectParamType = FriendDataItem.ParamType.NAME;
                break;
        }
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].changeInfo(m_SelectParamType);
        }
    }

    public float GetItemScale(int index)
    {
        return itemPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }

    public RectTransform GetItem(int index, RectTransform recyclableItem)
    {
        // indexの位置にあるセルを読み込む処理

        if (null == recyclableItem)
        {
            // 初回ロード時はinstantateItemCountで指定した回数分だけitemがnullで来るので、ここで生成してあげる
            // 以降はitemが再利用されるため、Reflesh()しない限りnullは来ない
            recyclableItem = Instantiate(itemPrefab).GetComponent<RectTransform>();
            FriendDataItem _item = recyclableItem.GetComponent<FriendDataItem>();
            itemList.Add(_item);
        }

        // セルの内容書き換え
        {
            FriendDataItem _item = recyclableItem.GetComponent<FriendDataItem>();
            _item.setup(index, friendDataList[index], m_SelectParamType, CheckLock);
        }

        return recyclableItem;
    }



    private void SetUpButtons()
    {
        var sortButtonModel = new ButtonModel();
        m_SortButton = ButtonView.Attach<FriendListSortButton>(SortButtonPrefabPath, m_sortButtonRoot);
        m_SortButton.SetModel<ButtonModel>(sortButtonModel);
        sortButtonModel.OnClicked += () =>
        {
            OnClickSortButton();
        };

        // TODO : 演出を入れるならその場所に移動
        sortButtonModel.Appear();
        sortButtonModel.SkipAppearing();

        var reloadButtonModel = new ButtonModel();
        m_ReloadButton = ButtonView.Attach<FriendReloadButton>(ReloadButtonPrefabPath, m_reloadButtonRoot);
        m_ReloadButton.SetReloadButtonModel(reloadButtonModel);
        reloadButtonModel.isEnabled = MainMenuParam.m_IsEnableQuestFriendReload;
        reloadButtonModel.OnClicked += () =>
        {
            OnClickReloadButton();
        };
        reloadButtonModel.Appear();
        reloadButtonModel.SkipAppearing();
    }

    public void SetUpSortData(LocalSaveSortInfo sortInfo)
    {
        if (sortInfo != null && sortInfo.m_SortType == (int)MAINMENU_SORT_SEQ.SEQ_INIT)
        {
            sortInfo = SortDialog.SetDefaultSortInfo(sortInfo, SortDialog.DIALOG_TYPE.FRIEND);
        }

        friendBaseList.SetUpSortData(sortInfo);
        m_SortInfo = sortInfo;
        if (sortInfo != null)
        {
            friendBaseList.AddSortInfo(SORT_PARAM.FRIEND_STATE, (sortInfo.m_SortIsAscOrder) ? SORT_ORDER.ASCENDING : SORT_ORDER.DESCENDING);
            friendBaseList.AddSortInfo(SORT_PARAM.LOGIN_TIME, (sortInfo.m_SortIsAscOrder) ? SORT_ORDER.ASCENDING : SORT_ORDER.DESCENDING);
            friendBaseList.AddSortInfo(SORT_PARAM.ID, (sortInfo.m_SortIsAscOrder) ? SORT_ORDER.ASCENDING : SORT_ORDER.DESCENDING);
        }

    }

    /// <summary>
    /// ソート＆フィルタの実行
    /// </summary>
    /// <param name="sortInfo"></param>
    public void ExecSort(LocalSaveSortInfo sortInfo)
    {
        SetUpSortData(sortInfo);
        friendDataList = friendBaseList.Exec(SORT_OBJECT_TYPE.FRIEND_LIST);
        IsViewEmpty = (friendDataList.Count == 0) ? true : false;
        if (scrollContent != null)
        {
            scrollContent.Initialize(this);
        }
        SetStatusType(sortInfo);
        SetFilterText(sortInfo);
    }

    /// <summary>
    /// ソートの種別表示の設定
    /// </summary>
    /// <param name="sortInfo"></param>
    void SetStatusType(LocalSaveSortInfo sortInfo)
    {
        StatusTypeText = "";
        if (sortInfo != null)
        {
            MAINMENU_SORT_SEQ sortType = (MAINMENU_SORT_SEQ)sortInfo.m_SortType;
            if (sortType == MAINMENU_SORT_SEQ.SEQ_INIT)
            {
                StatusTypeText = GameTextUtil.GetText("filter_text18");
            }
            else
            {
                StatusTypeText = GameTextUtil.GetSortDialogSortText(sortType);
            }

            StatusTypeText = string.Format(GameTextUtil.GetText("sort_colorset"), StatusTypeText); // カラーコード設定
        }
    }

    /// <summary>
    /// 絞り込みの表示設定
    /// </summary>
    /// <param name="sortInfo"></param>
    void SetFilterText(LocalSaveSortInfo sortInfo)
    {
        FilterText = "";
        string resourcePath = "config";
        if (sortInfo != null)
        {
            if (SortUtil.CheckFilterWorking(sortInfo.m_FilterRares) == true
                || SortUtil.CheckFilterWorking(sortInfo.m_FilterElements) == true
                || SortUtil.CheckFilterWorking(sortInfo.m_FilterKinds) == true)
            {
                FilterText = GameTextUtil.GetText("filter_text59");
                resourcePath = resourcePath + "_active";
            }
        }

        if (m_SortButton != null)
        {
            m_SortButton.m_ButtonImage.sprite = ResourceManager.Instance.Load(resourcePath);
        }
    }

    void OnClickSortButton()
    {
        if (ServerApi.IsExists == true)
        {
            return;
        }

        if (OnClickSortButtonAction != null)
        {
            OnClickSortButtonAction();
        }
    }

    void OnClickReloadButton()
    {
        if (ServerApi.IsExists == true)
        {
            return;
        }

        if (OnClickReloadButtonAction != null)
        {
            OnClickReloadButtonAction();
        }
    }
}
