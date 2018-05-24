/**
 *  @file   SortDialog.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/17
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;
using DG.Tweening;

public class SortDialog : MenuPartsBase
{
    public const MAINMENU_SORT_SEQ DEFAULT_UNIT_SORT_TYPE = MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_GET;
    public const MAINMENU_SORT_SEQ DEFAULT_FRIEND_SORT_TYPE = MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_DEFAULT;

    public const bool DEFAULT_UNIT_ASC_ORDER = false;
    public const bool DEFAULT_FRIEND_ASC_ORDER = true;

    public class MissionFilterInfo
    {
        /// <summary>表示フィルタータイプ</summary>
        public MasterDataDefineLabel.AchievementFilterType m_filter_type;
        /// <summary>種別受け取りタイプ</summary>
        public MasterDataDefineLabel.AchievementReceiveType m_receive_type;

        public void InitParam()
        {
            m_filter_type = MasterDataDefineLabel.AchievementFilterType.ALL;
            m_receive_type = MasterDataDefineLabel.AchievementReceiveType.NONE;
        }

        public MissionFilterInfo Clone()
        {
            MissionFilterInfo _data = new MissionFilterInfo();

            _data.m_filter_type = this.m_filter_type;
            _data.m_receive_type = this.m_receive_type;

            return _data;
        }
    }

    public enum DIALOG_TYPE
    {
        NONE = 0,
        UNIT,
        FRIEND,
        MISSION,
    }

    // TODO CloseActionの引数のクラスを1つにまとめたい
    public Action<LocalSaveSortInfo> OnCloseAction = delegate { };
    public Action<LocalSaveSortInfo> OnCloseThreadAction = null;

    public Action<MissionFilterInfo> OnCloseMissionSortAction = delegate { };

    [SerializeField]
    GameObject DialogRoot = null;
    [SerializeField]
    GameObject ButtonPanelRoot = null;

    /// <summary>ソート画面</summary>
    public SortDialogMainPanel m_MainPanel;
    /// <summary>レア度フィルタ画面</summary>
    public SortDialogRareFilterPanel m_RareFilterPanel;
    /// <summary>種族フィルタ画面</summary>
    public SortDialogKindFilterPanel m_KindFilterPanel;
    /// <summary>お好みソート画面</summary>
    public SortDialogFavoriteSortPanel m_FavoriteSortPanel;
    /// <summary>お好みソートの選択画面</summary>
    public SortDialogFavoriteSortSelectGridPanel m_FavoriteSortSelectGridPanel;
    /// <summary>ミッションフィルター画面</summary>
    public SortDialogMissionFilterPanel m_SortDialogMissionFilterPanel;

    public LocalSaveSortInfo m_SortData;
    public MissionFilterInfo m_MissionFilterData;

    public MAINMENU_SORT_SEQ[] m_MainSortList = null;
    public MAINMENU_SORT_SEQ[] m_FavoriteSortList = null;

    public bool m_IsCloseSE = true;

    public DIALOG_TYPE m_DialogType = DIALOG_TYPE.NONE;

    M4uProperty<bool> isActiveOneButton = new M4uProperty<bool>(true);
    public bool IsActiveOneButton
    {
        get
        {
            return isActiveOneButton.Value;
        }
        set
        {
            isActiveOneButton.Value = value;
        }
    }

    M4uProperty<bool> isActiveTwoButton = new M4uProperty<bool>(true);
    public bool IsActiveTwoButton
    {
        get
        {
            return isActiveTwoButton.Value;
        }
        set
        {
            isActiveTwoButton.Value = value;
        }
    }

    M4uProperty<string> closeButtonText = new M4uProperty<string>();
    public string CloseButtonText
    {
        get
        {
            return closeButtonText.Value;
        }
        set
        {
            closeButtonText.Value = value;
        }
    }

    M4uProperty<string> returnButtonText = new M4uProperty<string>();
    public string ReturnButtonText
    {
        get
        {
            return returnButtonText.Value;
        }
        set
        {
            returnButtonText.Value = value;
        }
    }

    M4uProperty<string> decisionButtonText = new M4uProperty<string>();
    public string DecisionButtonText
    {
        get
        {
            return decisionButtonText.Value;
        }
        set
        {
            decisionButtonText.Value = value;
        }
    }

    private Semaphore m_Semapho = new Semaphore();

    public static bool IsExists
    {
        get
        {
            GameObject obj = GameObject.FindGameObjectWithTag("SortDialog");
            if (obj == null)
            {
                return false;
            }
            SortDialog comp = obj.GetComponent<SortDialog>();
            if (comp == null)
            {
                return false;
            }

            return true;
        }
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
        CloseButtonText = GameTextUtil.GetText("common_button7");
        ReturnButtonText = GameTextUtil.GetText("common_button6");
        DecisionButtonText = GameTextUtil.GetText("common_button7");

        AndroidBackKeyManager.Instance.StackPush(gameObject, OnClickCloseButton);
    }

    // Update is called once per frame
    void Update()
    {
        m_Semapho.Tick();
    }

    void CreateSortDialogPanel()
    {
        m_MainPanel = Attach<SortDialogMainPanel>("Prefab/SortDialog/SortDialogMainPanel", DialogRoot);
        m_RareFilterPanel = Attach<SortDialogRareFilterPanel>("Prefab/SortDialog/SortDialogRareFilterPanel", DialogRoot);
        m_KindFilterPanel = Attach<SortDialogKindFilterPanel>("Prefab/SortDialog/SortDialogKindFilterPanel", DialogRoot);
        m_FavoriteSortPanel = Attach<SortDialogFavoriteSortPanel>("Prefab/SortDialog/SortDialogFavoriteSortPanel", DialogRoot);
        m_FavoriteSortSelectGridPanel = Attach<SortDialogFavoriteSortSelectGridPanel>("Prefab/SortDialog/SortDialogFavoriteSortSelectGridPanel", DialogRoot);

        m_MainPanel.IsAscOrder = true;
        m_MainPanel.OnClickFilterButtonAction = OnClickMainPanelFilterButton;
        m_MainPanel.OnClickSortButtonAction = OnClickMainPanelSortButton;
        m_FavoriteSortPanel.OnClickFavoriteSortItemButtonAction = OnClickFavoriteSortItemButton;
        m_FavoriteSortSelectGridPanel.OnClickSortButtonAction = OnClickFavoriteSortSelectPanelSortButton;

        ButtonPanelRoot.transform.SetAsLastSibling();
    }

    void CreateMissionFilterPanel()
    {
        m_SortDialogMissionFilterPanel = Attach<SortDialogMissionFilterPanel>("Prefab/SortDialog/SortDialogMissionFilterPanel", DialogRoot);
        m_SortDialogMissionFilterPanel.gameObject.SetActive(true);

        ButtonPanelRoot.transform.SetAsLastSibling();
    }

    void InitMainMenu()
    {
        IsActiveOneButton = true;
        IsActiveTwoButton = false;
        m_MainPanel.gameObject.SetActive(true);
        m_RareFilterPanel.gameObject.SetActive(false);
        m_KindFilterPanel.gameObject.SetActive(false);
        m_FavoriteSortPanel.gameObject.SetActive(false);
        m_FavoriteSortSelectGridPanel.gameObject.SetActive(false);
    }

    public void SetDialogType(DIALOG_TYPE dialogType)
    {
        m_DialogType = dialogType;
        switch (dialogType)
        {
            case DIALOG_TYPE.UNIT:
                SetUpUnitSortList();
                CreateSortDialogPanel();
                InitMainMenu();
                break;
            case DIALOG_TYPE.FRIEND:
                SetUpFriendSortList();
                CreateSortDialogPanel();
                InitMainMenu();
                m_FavoriteSortPanel.IsActiveUsePriority = false;
                break;
            case DIALOG_TYPE.MISSION:
                CreateMissionFilterPanel();
                IsActiveOneButton = true;
                IsActiveTwoButton = false;
                break;
        }
    }

    public void SetSortData(LocalSaveSortInfo sortData)
    {
        m_SortData = SetDefaultSortInfo(sortData, m_DialogType);
    }

    /// <summary>
    /// フレンドリストのソート項目設定
    /// </summary>
    void SetUpFriendSortList()
    {
        m_MainSortList = new MAINMENU_SORT_SEQ[] {
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_DEFAULT,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LOGIN_TIME,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RANK,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ELEMENT,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_KIND,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ATTACK,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_HP,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LEVEL,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RARE,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LIMIT_OVER,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_CHARM,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE_SORT
        };

        m_FavoriteSortList = new MAINMENU_SORT_SEQ[] {
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LOGIN_TIME,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RANK,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ELEMENT,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_KIND,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ATTACK,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_HP,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LEVEL,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RARE,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LIMIT_OVER,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_CHARM,
        };
    }

    /// <summary>
    /// ユニットリストのソート項目設定
    /// </summary>
    void SetUpUnitSortList()
    {
        m_MainSortList = new MAINMENU_SORT_SEQ[] {
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_GET,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_COST,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RARE,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ELEMENT,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_KIND,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LEVEL,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_HP,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ATTACK,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ID,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LIMIT_OVER,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_CHARM,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_PLUS,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE_SORT,
        };

        m_FavoriteSortList = new MAINMENU_SORT_SEQ[] {
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_GET,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_COST,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RARE,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ELEMENT,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_KIND,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LEVEL,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_HP,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ATTACK,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ID,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LIMIT_OVER,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_CHARM,
            MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_PLUS
        };
    }

    /// <summary>
    /// ソート設定を確定させる
    /// </summary>
    void ConfirmSortType()
    {
        if (m_DialogType == DIALOG_TYPE.UNIT || m_DialogType == DIALOG_TYPE.FRIEND)
        {
            if (m_SortData != null)
            {
                SortDialogTextButtonListContext item = m_MainPanel.SortButtons.Find((v) => v.IsSelect == true);
                if (item != null)
                {
                    m_SortData.m_SortType = (int)item.SortType;
                }
                else
                {
                    m_SortData = SetDefaultSortInfo(m_SortData, m_DialogType);
                }
                m_SortData.m_SortIsAscOrder = m_MainPanel.IsAscOrder;

                // 属性フィルタの設定
                m_SortData.m_FilterElements = new int[m_MainPanel.Elements.Count];
                m_SortData.m_FilterElements = Array.ConvertAll(m_SortData.m_FilterElements, (v) => -1);
                int count = 0;
                for (int i = 0; i < m_MainPanel.Elements.Count; i++)
                {
                    if (m_MainPanel.Elements[i].IsSelect == true)
                    {
                        m_SortData.m_FilterElements[count] = (int)m_MainPanel.Elements[i].ElementType;
                        ++count;
                    }
                }
                if (count == 0)
                {
                    m_SortData.m_FilterElements = null;
                }
            }
        }
        else if (m_DialogType == DIALOG_TYPE.MISSION)
        {

        }
    }

    /// <summary>
    /// フィルタの項目を選択したとき
    /// </summary>
    /// <param name="filterButton"></param>
    void OnClickMainPanelFilterButton(MAINMENU_FILTER_TYPE filterType)
    {
        ConfirmSortType(); // ソート設定を確定させる
        IsActiveOneButton = false;
        IsActiveTwoButton = true;
        m_MainPanel.gameObject.SetActive(false);

        switch (filterType)
        {
            case MAINMENU_FILTER_TYPE.FILTER_RARE:
                AndroidBackKeyManager.Instance.StackPush(m_RareFilterPanel.gameObject, OnBackKeyRareFilterPanel);
                m_RareFilterPanel.gameObject.SetActive(true);
                m_RareFilterPanel.UpdateData(m_SortData);
                break;
            case MAINMENU_FILTER_TYPE.FILTER_KIND:
                AndroidBackKeyManager.Instance.StackPush(m_KindFilterPanel.gameObject, OnBackKeyKindFilterPanel);
                m_KindFilterPanel.gameObject.SetActive(true);
                m_KindFilterPanel.UpdateData(m_SortData);
                break;
        }
    }

    /// <summary>
    /// ソートの項目を選択したとき
    /// </summary>
    /// <param name="sortButton"></param>
    void OnClickMainPanelSortButton(SortDialogTextButtonListContext sortButton)
    {
        if (sortButton.SortType == MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE_SORT)
        {
            ConfirmSortType(); // ソート設定を確定させる
            AndroidBackKeyManager.Instance.StackPush(m_FavoriteSortPanel.gameObject, OnBackKeyFavoriteSortPanel);
            IsActiveOneButton = false;
            IsActiveTwoButton = true;
            m_MainPanel.gameObject.SetActive(false);
            m_FavoriteSortPanel.gameObject.SetActive(true);
            m_FavoriteSortPanel.UpdateData(m_SortData);
        }
        else
        {
            LocalSaveSortInfo sort = new LocalSaveSortInfo();
            sort.m_SortType = (int)sortButton.SortType;
            sort.m_SortIsAscOrder = m_MainPanel.IsAscOrder;
            m_MainPanel.UpdateSort(sort);
        }
    }

    /// <summary>
    /// お好みソートのセルの項目ボタンを押したとき
    /// </summary>
    /// <param name="favoriteSort"></param>
    void OnClickFavoriteSortItemButton(SortDialogFavoriteSortListContext favoriteSort)
    {
        AndroidBackKeyManager.Instance.StackPush(m_FavoriteSortSelectGridPanel.gameObject, OnBackKeyFavoriteSortSelectGridPanel);
        m_FavoriteSortPanel.gameObject.SetActive(false);
        m_FavoriteSortSelectGridPanel.gameObject.SetActive(true);
        m_FavoriteSortSelectGridPanel.SetUpSelected(favoriteSort.SortType, m_FavoriteSortPanel.SortCells);
        IsActiveOneButton = false;
        IsActiveTwoButton = false;
    }

    /// <summary>
    /// お好みソートの項目を選択したとき
    /// </summary>
    /// <param name="sortButton"></param>
    void OnClickFavoriteSortSelectPanelSortButton(SortDialogTextButtonListContext sortButton)
    {
        m_FavoriteSortPanel.SetSortType(sortButton.SortType, m_FavoriteSortPanel.SelectIndex);
        m_FavoriteSortPanel.gameObject.SetActive(true);
        m_FavoriteSortSelectGridPanel.gameObject.SetActive(false);
        IsActiveOneButton = false;
        IsActiveTwoButton = true;
    }

    private void OnBackKeyRareFilterPanel()
    {
        AndroidBackKeyManager.Instance.StackPop(m_RareFilterPanel.gameObject);
        OnClickReturnButton();
    }

    private void OnBackKeyKindFilterPanel()
    {
        AndroidBackKeyManager.Instance.StackPop(m_KindFilterPanel.gameObject);
        OnClickReturnButton();
    }

    private void OnBackKeyFavoriteSortPanel()
    {
        AndroidBackKeyManager.Instance.StackPop(m_FavoriteSortPanel.gameObject);
        OnClickReturnButton();
    }

    private void OnBackKeyFavoriteSortSelectGridPanel()
    {
        AndroidBackKeyManager.Instance.StackPop(m_FavoriteSortSelectGridPanel.gameObject);
        m_FavoriteSortPanel.gameObject.SetActive(true);
        m_FavoriteSortSelectGridPanel.gameObject.SetActive(false);
        IsActiveOneButton = false;
        IsActiveTwoButton = true;
    }

    /// <summary>
    /// 閉じるボタンを押したとき
    /// </summary>
    public void OnClickCloseButton()
    {
        AndroidBackKeyManager.Instance.StackPop(gameObject);

        ConfirmSortType();

        if (m_IsCloseSE)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        }
        if (OnCloseThreadAction != null)
        {
            Tweener tw = DialogRoot.transform.DOScaleY(0f, 0.25f);
            m_Semapho.Lock(() =>
            {
                if (OnCloseAction != null)
                {
                    OnCloseAction(m_SortData);
                }
                if (OnCloseMissionSortAction != null)
                {
                    OnCloseMissionSortAction(m_MissionFilterData);
                }

                if (tw.IsActive() &&
                    tw.IsComplete() == false)
                {
                    tw.OnComplete(() =>
                    {
                        Hide();
                    });
                }
                else
                {
                    Hide();
                }
            });
            new System.Threading.Thread(() =>
            {
                OnCloseThreadAction(m_SortData);
                m_Semapho.Unlock();
            })
            .Start();
        }
        else
        {
            if (OnCloseAction != null)
            {
                OnCloseAction(m_SortData);
            }
            if (OnCloseMissionSortAction != null)
            {
                OnCloseMissionSortAction(m_MissionFilterData);
            }

            DialogRoot.transform.DOScaleY(0f, 0.25f).OnComplete(() =>
            {
                Hide();
            });

        }
    }

    /// <summary>
    /// 戻るボタンを押したとき
    /// </summary>
    public void OnClickReturnButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        InitMainMenu();
    }

    /// <summary>
    /// 決定ボタンを押したとき
    /// </summary>
    public void OnClickDecisionButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        //--------------------------------------------------
        // 設定の確定
        //--------------------------------------------------
        if (m_RareFilterPanel.gameObject.IsActive())
        {
            if (m_SortData != null)
            {
                m_SortData.m_FilterRares = new int[m_RareFilterPanel.FilterSwitches.Count];
                m_SortData.m_FilterRares = Array.ConvertAll(m_SortData.m_FilterRares, (v) => -1);
                int count = 0;
                for (int i = 0; i < m_RareFilterPanel.FilterSwitches.Count; i++)
                {
                    if (m_RareFilterPanel.FilterSwitches[i].IsSelect == true)
                    {
                        m_SortData.m_FilterRares[count] = (int)m_RareFilterPanel.FilterSwitches[i].RarityType;
                        ++count;
                    }
                }
                if (count == 0)
                {
                    m_SortData.m_FilterRares = null;
                }
            }

        }
        else if (m_KindFilterPanel.gameObject.IsActive())
        {
            if (m_SortData != null)
            {
                m_SortData.m_FilterKinds = new int[m_KindFilterPanel.FilterSwitches.Count];
                m_SortData.m_FilterKinds = Array.ConvertAll(m_SortData.m_FilterKinds, (v) => -1);
                int count = 0;
                for (int i = 0; i < m_KindFilterPanel.FilterSwitches.Count; i++)
                {
                    if (m_KindFilterPanel.FilterSwitches[i].IsSelect == true)
                    {
                        m_SortData.m_FilterKinds[count] = (int)m_KindFilterPanel.FilterSwitches[i].KindType;
                        ++count;
                    }
                }
                m_SortData.m_FilterIsIncludeKindsSub = m_KindFilterPanel.IsIncludeSubKind;
                if (count == 0)
                {
                    m_SortData.m_FilterKinds = null;
                }
            }
        }
        else if (m_FavoriteSortPanel.gameObject.IsActive())
        {
            if (m_SortData != null)
            {
                m_SortData.m_FavoriteSorts = new LocalSaveSort[m_FavoriteSortPanel.SortCells.Count];
                for (int i = 0; i < m_FavoriteSortPanel.SortCells.Count; i++)
                {
                    m_SortData.m_FavoriteSorts[i] = new LocalSaveSort();
                    m_SortData.m_FavoriteSorts[i].m_SortType = (int)m_FavoriteSortPanel.SortCells[i].SortType;
                    m_SortData.m_FavoriteSorts[i].m_IsAscOrder = (m_FavoriteSortPanel.SortCells[i].SortType != MAINMENU_SORT_SEQ.SEQ_INIT) ?
                                                                 m_FavoriteSortPanel.SortCells[i].IsAscOrder : true;
                }

                m_SortData.m_FavoriteSortIsUsePriority = m_FavoriteSortPanel.IsOnUsePriority;
            }

            if (m_SortData != null)
            {
                m_SortData.m_SortType = (int)MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE_SORT;
            }
        }

        m_MainPanel.UpdateSort(m_SortData);
        m_MainPanel.UpdateFilter(m_SortData);
        InitMainMenu();
    }

    /// <summary>
    /// ソート設定の初期化
    /// </summary>
    public void ResetSortParam()
    {
        m_SortData = null;
        m_SortData = SetDefaultSortInfo(m_SortData, m_DialogType);

        m_MainPanel.UpdateSort(m_SortData);
        m_MainPanel.UpdateFilter(m_SortData);
        m_RareFilterPanel.UpdateData(m_SortData);
        m_KindFilterPanel.UpdateData(m_SortData);
        m_FavoriteSortPanel.UpdateData(m_SortData);
    }

    public static SortDialog Create()
    {
        GameObject _tmpObj = Resources.Load<GameObject>("Prefab/SortDialog/SortDialog");
        if (_tmpObj == null) return null;
        GameObject _newObj = Instantiate(_tmpObj);
        if (_newObj == null) return null;
        UnityUtil.SetObjectEnabledOnce(_newObj, true);
        SortDialog sortDialog = _newObj.GetComponent<SortDialog>();

        return sortDialog;
    }

    /// <summary>
    /// ダイアログを閉じる
    /// </summary>
    public void Hide()
    {
        DestroyObject(gameObject);
    }

    public static LocalSaveSortInfo SetDefaultSortInfo(LocalSaveSortInfo sortInfo, DIALOG_TYPE dialogType)
    {
        LocalSaveSortInfo _sortInfo = sortInfo;
        if (_sortInfo == null)
        {
            _sortInfo = new LocalSaveSortInfo();
            _sortInfo.InitParam();
        }

        if (_sortInfo.m_SortType != (int)MAINMENU_SORT_SEQ.SEQ_INIT)
        {
            return _sortInfo;
        }

        switch (dialogType)
        {
            case DIALOG_TYPE.UNIT:
                _sortInfo.m_SortType = (int)DEFAULT_UNIT_SORT_TYPE;
                _sortInfo.m_SortIsAscOrder = DEFAULT_UNIT_ASC_ORDER;
                break;
            case DIALOG_TYPE.FRIEND:
                _sortInfo.m_SortType = (int)DEFAULT_FRIEND_SORT_TYPE;
                _sortInfo.m_SortIsAscOrder = DEFAULT_FRIEND_ASC_ORDER;
                break;
            default:
                break;
        }

        return _sortInfo;
    }
}
