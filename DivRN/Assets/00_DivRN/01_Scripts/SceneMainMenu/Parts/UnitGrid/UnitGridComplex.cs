using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using DG.Tweening;
using M4u;

public class UnitGridComplex : MenuPartsBase
{
    [SerializeField]
    private GameObject m_gridViewRoot;
    [SerializeField]
    private GameObject m_SortButtonRoot;

    private static readonly string SortButtonPrefabPath = "Prefab/UnitGrid/UnitGridSortButton";

    public Action OnClickSortButtonAction = delegate { };

    bool m_IsTutorialSortData = false;

    M4uProperty<string> unitCountText = new M4uProperty<string>();
    /// <summary>
    /// ユニット数等の表示
    /// </summary>
    public string UnitCountText { get { return unitCountText.Value; } set { unitCountText.Value = value; } }

    /// <summary>
    /// ソートボタンの有効／無効
    /// </summary>
    M4uProperty<bool> isActiveSortButton = new M4uProperty<bool>(true);
    public bool IsActiveSortButton { get { return isActiveSortButton.Value; } set { isActiveSortButton.Value = value; } }

    M4uProperty<string> statusTypeText = new M4uProperty<string>();
    /// <summary>ステータスの種類表示</summary>
    public string StatusTypeText { get { return statusTypeText.Value; } set { statusTypeText.Value = value; } }

    M4uProperty<string> filterText = new M4uProperty<string>();
    /// <summary>絞り込み表示</summary>
    public string FilterText { get { return filterText.Value; } set { filterText.Value = value; } }

    /// <summary>ユニット表示ステータス更新用</summary>
    MAINMENU_SORT_SEQ m_SortType = MAINMENU_SORT_SEQ.SEQ_INIT;
    MAINMENU_SORT_SEQ[] m_FavoriteSortTypes = null;
    LocalSaveSortInfo m_SortInfo = null;

    private UnitGridView m_unitGridView = null;
    public UnitGridView GridView { get { return m_unitGridView; } }

    // ソート用
    private SortList<UnitGridContext> m_unitBaseList = new SortList<UnitGridContext>();

    private bool m_bOpen = false;

    private float m_scrollHeight;

    private UnitGridSortButton m_SortButton;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    void Start()
    {
        SetUpButtons();
    }


    public UnitGridComplex AttchUnitGrid<T>(T unitGridView) where T : UnitGridView
    {
        m_unitGridView = unitGridView;
        m_unitGridView.Attach(m_gridViewRoot);
        m_unitGridView.OnUnitGridContextUpdated += unitGridContext =>
        {
            unitGridContext.SetStatus(m_SortType, m_FavoriteSortTypes);
        };
        m_unitGridView.OnUnitGridSetUp += () =>
        {
            SelectStatusDisplayType(m_SortType, m_FavoriteSortTypes);
        };
        RectTransform prect = m_unitGridView.GetComponent<ScrollRect>().content.parent.GetComponent<RectTransform>();
        m_scrollHeight = prect.rect.height;

        unitGridView.GridViewHight = m_scrollHeight;

        return this;
    }


    /// <summary>
    /// ユニット所持数の設定
    /// </summary>
    public void UpdateUnitCount()
    {
        int unitMax = (int)UserDataAdmin.Instance.m_StructPlayer.total_unit;
        int unitTotal = 0;

        if (UserDataAdmin.Instance.m_StructPlayer.unit_list != null)
        {
            unitTotal = UserDataAdmin.Instance.m_StructPlayer.unit_list.Length;
        }

        if (unitTotal <= unitMax)
        {
            UnitCountText = string.Format(GameTextUtil.GetText("unit_list_count1"), unitTotal, unitMax);
        }
        else
        {
            UnitCountText = string.Format(GameTextUtil.GetText("unit_list_count2"), unitTotal, unitMax);
        }
    }

    #region Delegate
    /// <summary>
    /// ソートボタンが押されたとき
    /// </summary>
    public void OnClickSortButton()
    {
        Vector2 rt = m_gridViewRoot.GetComponent<RectTransform>().offsetMax;
        OnClickSortButtonAction();
    }
    #endregion

    /// <summary>
    /// ソート＆フィルタ情報の設定
    /// </summary>
    /// <param name="sortInfo"></param>
    public void SetUpSortData(LocalSaveSortInfo sortInfo)
    {
        m_IsTutorialSortData = false;
        if (sortInfo != null && sortInfo.m_SortType == (int)MAINMENU_SORT_SEQ.SEQ_INIT)
        {
            sortInfo = SortDialog.SetDefaultSortInfo(sortInfo, SortDialog.DIALOG_TYPE.UNIT);
        }

        m_unitBaseList.SetUpSortData(sortInfo);
        m_SortInfo = sortInfo;
        if (sortInfo != null)
        {
            m_SortType = (MAINMENU_SORT_SEQ)sortInfo.m_SortType;
            if (sortInfo.m_FavoriteSorts != null)
            {
                m_FavoriteSortTypes = new MAINMENU_SORT_SEQ[sortInfo.m_FavoriteSorts.Length];
                for (int i = 0; i < sortInfo.m_FavoriteSorts.Length; i++)
                {
                    m_FavoriteSortTypes[i] = (MAINMENU_SORT_SEQ)sortInfo.m_FavoriteSorts[i].m_SortType;
                }
            }

            m_unitBaseList.AddSortInfo(SORT_PARAM.ID, (sortInfo.m_SortIsAscOrder) ? SORT_ORDER.ASCENDING : SORT_ORDER.DESCENDING);
            m_unitBaseList.AddSortInfo(SORT_PARAM.LEVEL, (sortInfo.m_SortIsAscOrder) ? SORT_ORDER.ASCENDING : SORT_ORDER.DESCENDING);
        }
        m_unitBaseList.AddSortInfo(SORT_PARAM.UNIQUE_ID, SORT_ORDER.ASCENDING);
    }

    /// <summary>
    /// ソート＆フィルタ情報の設定(チュートリアル用)
    /// </summary>
    /// <param name="sortInfo"></param>
    public void SetUpTutorialSortData()
    {
        m_IsTutorialSortData = true;
        m_unitBaseList.ClearSortInfo();
        m_unitBaseList.ClearFilter();
        m_unitBaseList.AddSortInfo(SORT_PARAM.UNIQUE_ID, SORT_ORDER.DESCENDING);
    }

    /// <summary>
    /// ソート実行（データのみ）
    /// </summary>
    /// <param name="sortInfo"></param>
    public void ExecSortOnly(LocalSaveSortInfo sortInfo)
    {
        SetUpSortData(sortInfo);
        Units = m_unitBaseList.Exec(SORT_OBJECT_TYPE.UNIT_LIST);
    }

    /// <summary>
    /// ソート内容をリストに適用
    /// </summary>
    /// <param name="sortInfo"></param>
    public void ExecSortBuild(LocalSaveSortInfo sortInfo)
    {
        SelectStatusDisplayType(m_SortType, m_FavoriteSortTypes);
        SetStatusType(m_SortType, m_FavoriteSortTypes);
        SetFilterText(sortInfo);
        m_unitGridView.CreateList();
    }

    private void SetUpButtons()
    {
        var sortButtonModel = new ButtonModel();
        var fixButtonModel = new ButtonModel();

        m_SortButton = ButtonView.Attach<UnitGridSortButton>(SortButtonPrefabPath, m_SortButtonRoot);
        m_SortButton.SetModel<ButtonModel>(sortButtonModel);

        sortButtonModel.OnClicked += () =>
        {
            OnClickSortButton();
        };

        sortButtonModel.Appear();
        sortButtonModel.SkipAppearing();
    }

    // =========================================== transfer functions

    /// <summary>
    /// ユニットアイテムリスト
    /// </summary>
    public List<UnitGridContext> Units
    {
        get { return m_unitGridView.Units; }
        private set
        {
            m_unitGridView.Units = value;
        }
    }

    public Action<UnitGridContext> ClickUnitAction
    {
        get { return m_unitGridView.ClickUnitAction; }
        set { m_unitGridView.ClickUnitAction = value; }
    }

    public Action<UnitGridContext> LongPressUnitAction
    {
        get { return m_unitGridView.LongPressUnitAction; }
        set { m_unitGridView.LongPressUnitAction = value; }
    }

    public Action<UnitGridContext> SetupUnitIconType
    {
        get { return m_unitGridView.SetupUnitIconType; }
        set { m_unitGridView.SetupUnitIconType = value; }
    }
    public Action<UnitGridContext> SetupUnitSelected
    {
        get { return m_unitGridView.SetupUnitSelected; }
        set { m_unitGridView.SetupUnitSelected = value; }
    }

    public void CreateList(List<UnitGridContext> unitList)
    {
        m_unitBaseList.Body = unitList;

        Units = m_unitBaseList.Exec(SORT_OBJECT_TYPE.UNIT_LIST);

        SelectStatusDisplayType(m_SortType, m_FavoriteSortTypes);
        SetStatusType(m_SortType, m_FavoriteSortTypes);
        SetFilterText(m_SortInfo);
        UpdateUnitCount();

        m_unitGridView.CreateList();
    }

    public void UpdateList()
    {
        m_unitGridView.UpdateList();
    }

    /// <summary>
    /// 指定アイテムが表示しているアイテムの場合更新する
    /// </summary>
    /// <param name="item">指定アイテム</param>
    public void UpdateItem(UnitGridContext item)
    {
        m_unitGridView.UpdateItem(item);
    }

    /// <summary>
    /// ソート元データ更新
    /// </summary>
    /// <param name="item"></param>
    public void UpdateBaseItem(UnitGridContext item)
    {
        UnitGridContext orignal = m_unitBaseList.Body.Find(a => a.UnitData.unique_id == item.UnitData.unique_id);
        if (orignal != null)
        {
            orignal.Copy(item);
            orignal.updateSortParam();
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="unique_id"></param>
    /// <returns></returns>
    public UnitGridContext SearchUnitBaseItem(long unique_id)
    {
        return m_unitBaseList.Body.Find(a => a.UnitData.unique_id == unique_id);
    }

    /// <summary>
    /// アイコン下部のステータスを表示するタイプを設定する
    /// </summary>
    /// <param name="sortType">ソートタイプ</param>
    /// <returns></returns>
    public void SelectStatusDisplayType(MAINMENU_SORT_SEQ sortType, MAINMENU_SORT_SEQ[] favoriteSortTypes)
    {
        m_unitGridView.SelectStatusDisplayType(sortType, favoriteSortTypes);
    }

    /// <summary>
    /// ソートの種別表示の設定
    /// </summary>
    /// <param name="item"></param>
    /// <param name="sortType"></param>
    void SetStatusType(MAINMENU_SORT_SEQ sortType, MAINMENU_SORT_SEQ[] favoriteSortTypes)
    {
        if (TutorialManager.IsExists && m_IsTutorialSortData == true)
        {
            StatusTypeText = GameTextUtil.GetText("tutorial_sort_items01");
        }
        else
        {
            if (sortType == MAINMENU_SORT_SEQ.SEQ_INIT)
            {
                StatusTypeText = GameTextUtil.GetText("filter_text18");
            }
            else
            {
                StatusTypeText = GameTextUtil.GetSortDialogSortText(sortType);
            }
        }

        StatusTypeText = string.Format(GameTextUtil.GetText("sort_colorset"), StatusTypeText); // カラーコード設定
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

    public void changeGridWindowSize(bool bFlag, float moveHeight)
    {
        if (m_bOpen == bFlag)
        {
            return;
        }

        if (bFlag == true)
        {
            DOTween.To(
                    () => m_gridViewRoot.GetComponent<RectTransform>().offsetMax,
                    vec => m_gridViewRoot.GetComponent<RectTransform>().offsetMax = vec,
                    new Vector2(0, -40 - moveHeight),
                    0.3f
                ).Play();
        }
        else
        {
            RectTransform prect = m_unitGridView.GetComponent<ScrollRect>().content.parent.GetComponent<RectTransform>();
            RectTransform rtContent = m_unitGridView.GetComponent<ScrollRect>().content;
            UnitGridView ugv = m_unitGridView.gameObject.GetComponent<UnitGridView>();
            DOTween.To(
                () => m_gridViewRoot.GetComponent<RectTransform>().offsetMax,
                vec => m_gridViewRoot.GetComponent<RectTransform>().offsetMax = vec,
                new Vector2(0, -40),
                0.3f
                ).OnUpdate(() =>
                {
                    ugv.checkNormalizedPosition(m_scrollHeight > rtContent.rect.height);
                }).OnComplete(() =>
                {
                    ugv.checkNormalizedPosition(m_scrollHeight > rtContent.rect.height);
                }).Play();
        }
        m_bOpen = bFlag;
    }
}
