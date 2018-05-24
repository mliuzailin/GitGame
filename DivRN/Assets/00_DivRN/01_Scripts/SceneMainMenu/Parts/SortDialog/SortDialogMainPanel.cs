/**
 *  @file   SortDialogMainPanel.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/18
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M4u;

public class SortDialogMainPanel : MenuPartsBase
{
    public Action<MAINMENU_FILTER_TYPE> OnClickFilterButtonAction;
    public Action<SortDialogTextButtonListContext> OnClickSortButtonAction;
    public Action<bool> OnClickOrderButton;

    [SerializeField]
    SortDialogTextButtonView AscendingButton;
    [SerializeField]
    SortDialogTextButtonView DescendingButton;
    [SerializeField]
    SortDialogTextButtonView ResetButton;
    [SerializeField]
    SortDialogTextButtonView RareButton;
    [SerializeField]
    SortDialogTextButtonView KindButton;
    [SerializeField]
    SortDialogTextButtonView ElementAllOnButton;
    [SerializeField]
    SortDialogTextButtonView ElementAllOffButton;

    M4uProperty<string> titleText = new M4uProperty<string>();
    public string TitleText { get { return titleText.Value; } set { titleText.Value = value; } }

    M4uProperty<string> filterTitleText = new M4uProperty<string>();
    /// <summary>フィルタのタイトルテキスト</summary>
    public string FilterTitleText
    {
        get
        {
            return filterTitleText.Value;
        }
        set
        {
            filterTitleText.Value = value;
        }
    }

    M4uProperty<string> sortTitleText = new M4uProperty<string>();
    /// <summary>ソートのタイトルテキスト</summary>
    public string SortTitleText
    {
        get
        {
            return sortTitleText.Value;
        }
        set
        {
            sortTitleText.Value = value;
        }
    }

    M4uProperty<List<SortDialogTextButtonListContext>> sortButtons = new M4uProperty<List<SortDialogTextButtonListContext>>(new List<SortDialogTextButtonListContext>());
    /// <summary>ソートボタン</summary>
    public List<SortDialogTextButtonListContext> SortButtons
    {
        get
        {
            return sortButtons.Value;
        }
        set
        {
            sortButtons.Value = value;
        }
    }

    M4uProperty<List<SortDialogElementListContext>> elements = new M4uProperty<List<SortDialogElementListContext>>(new List<SortDialogElementListContext>());
    public List<SortDialogElementListContext> Elements { get { return elements.Value; } set { elements.Value = value; } }

    bool isAscOrder = true;
    /// <summary>昇順かどうか</summary>
    public bool IsAscOrder
    {
        get { return isAscOrder; }
        set
        {
            isAscOrder = value;
            SetupOrder(value);
        }
    }

    SortDialog m_SortDialog;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        SetUpButtons();
    }

    // Use this for initialization
    void Start()
    {
        m_SortDialog = GetComponentInParent<SortDialog>();

        string titleColor = GameTextUtil.GetText("title_Color");
        TitleText = string.Format(titleColor, GameTextUtil.GetText("filter_title1"));
        SortTitleText = string.Format(titleColor, GameTextUtil.GetText("filter_title2"));
        FilterTitleText = string.Format(titleColor, GameTextUtil.GetText("filter_title5"));
        SetUpSortList();
        SetUpElement();
        UpdateSort(m_SortDialog.m_SortData);
        UpdateFilter(m_SortDialog.m_SortData);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetUpButtons()
    {
        // 昇順
        var ascendingButtonModel = new ButtonModel();
        AscendingButton.SetModel<ButtonModel>(ascendingButtonModel);
        AscendingButton.m_OnText = string.Format(GameTextUtil.GetText("filter_choice"), GameTextUtil.GetText("filter_text19"));
        AscendingButton.m_OffText = GameTextUtil.GetText("filter_text19");
        ascendingButtonModel.OnClicked += () =>
        {
            OnClickAscButton();
        };
        ascendingButtonModel.Appear();
        ascendingButtonModel.SkipAppearing();

        // 降順
        var descendingButtonModel = new ButtonModel();
        DescendingButton.SetModel<ButtonModel>(descendingButtonModel);
        DescendingButton.m_OnText = string.Format(GameTextUtil.GetText("filter_choice"), GameTextUtil.GetText("filter_text20"));
        DescendingButton.m_OffText = GameTextUtil.GetText("filter_text20");
        descendingButtonModel.OnClicked += () =>
        {
            OnClickDescButton();
        };
        descendingButtonModel.Appear();
        descendingButtonModel.SkipAppearing();

        // 初期化
        var resetButtonModel = new ButtonModel();
        ResetButton.SetModel<ButtonModel>(resetButtonModel);
        ResetButton.m_OnText = GameTextUtil.GetText("filter_text51");
        resetButtonModel.OnClicked += () =>
        {
            OnClickResetButton();
        };
        resetButtonModel.Appear();
        resetButtonModel.SkipAppearing();

        // レア度
        var rareButtonModel = new ButtonModel();
        RareButton.SetModel<ButtonModel>(rareButtonModel);
        RareButton.m_OnText = string.Format(GameTextUtil.GetText("filter_choice"), GameTextUtil.GetSortDialogFilterText(MAINMENU_FILTER_TYPE.FILTER_RARE));
        RareButton.m_OffText = GameTextUtil.GetSortDialogFilterText(MAINMENU_FILTER_TYPE.FILTER_RARE);
        rareButtonModel.OnClicked += () =>
        {
            OnClickFilterButton(MAINMENU_FILTER_TYPE.FILTER_RARE);
        };
        rareButtonModel.Appear();
        rareButtonModel.SkipAppearing();

        // 種族
        var kindButtonModel = new ButtonModel();
        KindButton.SetModel<ButtonModel>(kindButtonModel);
        KindButton.m_OnText = string.Format(GameTextUtil.GetText("filter_choice"), GameTextUtil.GetSortDialogFilterText(MAINMENU_FILTER_TYPE.FILTER_KIND));
        KindButton.m_OffText = GameTextUtil.GetSortDialogFilterText(MAINMENU_FILTER_TYPE.FILTER_KIND);
        kindButtonModel.OnClicked += () =>
        {
            OnClickFilterButton(MAINMENU_FILTER_TYPE.FILTER_KIND);
        };
        kindButtonModel.Appear();
        kindButtonModel.SkipAppearing();

        // 一括ON
        var elementAllOnButtonModel = new ButtonModel();
        ElementAllOnButton.SetModel<ButtonModel>(elementAllOnButtonModel);
        ElementAllOnButton.m_OnText = GameTextUtil.GetText("filter_text58");
        elementAllOnButtonModel.OnClicked += () =>
        {
            OnClickElementAllOnButton();
        };
        elementAllOnButtonModel.Appear();
        elementAllOnButtonModel.SkipAppearing();

        // 一括OFF
        var elementAllOffButtonModel = new ButtonModel();
        ElementAllOffButton.SetModel<ButtonModel>(elementAllOffButtonModel);
        ElementAllOffButton.m_OnText = GameTextUtil.GetText("filter_text57");
        elementAllOffButtonModel.OnClicked += () =>
        {
            OnClickElementAllOffButton();
        };
        elementAllOffButtonModel.Appear();
        elementAllOffButtonModel.SkipAppearing();

    }

    /// <summary>
    /// フィルタの選択状態をチェック
    /// </summary>
    /// <param name="filterType"></param>
    /// <returns></returns>
    bool CheckSelectFilter(MAINMENU_FILTER_TYPE filterType, LocalSaveSortInfo sortInfo)
    {
        if (sortInfo == null) { return false; }
        if (filterType == MAINMENU_FILTER_TYPE.FILTER_RARE)
        {
            return SortUtil.CheckFilterWorking(sortInfo.m_FilterRares);
        }
        else if (filterType == MAINMENU_FILTER_TYPE.FILTER_ELEMENT)
        {
            return SortUtil.CheckFilterWorking(sortInfo.m_FilterElements);
        }
        else if (filterType == MAINMENU_FILTER_TYPE.FILTER_KIND)
        {
            return SortUtil.CheckFilterWorking(sortInfo.m_FilterKinds);
        }

        return false;
    }

    /// <summary>
    /// フィルタの選択状態をチェック
    /// </summary>
    /// <param name="elementType"></param>
    /// <returns></returns>
    bool CheckSelectElementFilter(MasterDataDefineLabel.ElementType elementType, LocalSaveSortInfo sortInfo)
    {
        if (sortInfo == null) { return false; }
        if (sortInfo.m_FilterElements == null || sortInfo.m_FilterElements.Length == 0) { return false; }
        return (Array.IndexOf(sortInfo.m_FilterElements, (int)elementType) >= 0);
    }


    void SetUpSortList()
    {
        SortButtons.Clear();
        if (m_SortDialog.m_MainSortList != null)
        {
            for (int i = 0; i < m_SortDialog.m_MainSortList.Length; i++)
            {
                AddSortData(m_SortDialog.m_MainSortList[i]);
            }
        }
    }

    void SetUpElement()
    {
        Elements.Clear();
        AddElement(MasterDataDefineLabel.ElementType.FIRE);
        AddElement(MasterDataDefineLabel.ElementType.WATER);
        AddElement(MasterDataDefineLabel.ElementType.WIND);
        AddElement(MasterDataDefineLabel.ElementType.LIGHT);
        AddElement(MasterDataDefineLabel.ElementType.DARK);
        AddElement(MasterDataDefineLabel.ElementType.NAUGHT);
    }

    void AddSortData(MAINMENU_SORT_SEQ sortType)
    {
        SortDialogTextButtonListContext sortButton = new SortDialogTextButtonListContext();
        sortButton.SortType = sortType;
        sortButton.OffNameText = GameTextUtil.GetSortDialogSortText(sortType);
        sortButton.OnNameText = string.Format(GameTextUtil.GetText("filter_choice"), GameTextUtil.GetSortDialogSortText(sortType));
        sortButton.OffTextColor = ColorUtil.COLOR_WHITE;
        sortButton.OnTextColor = ColorUtil.COLOR_YELLOW;
        sortButton.DidSelectItem = OnClickSortButton;
        SortButtons.Add(sortButton);
    }

    public void AddElement(MasterDataDefineLabel.ElementType elementType)
    {
        SortDialogElementListContext filterSwitch = new SortDialogElementListContext();
        filterSwitch.IconImage = GetElementPanel(elementType);
        filterSwitch.ElementType = elementType;
        filterSwitch.IsSelect = CheckSelectElement(elementType, m_SortDialog.m_SortData);
        filterSwitch.DidSelectItem = OnClickElementButton;
        Elements.Add(filterSwitch);
    }

    public Sprite GetElementPanel(MasterDataDefineLabel.ElementType elementType)
    {
        string sprite_name = "";

        switch (elementType)
        {
            case MasterDataDefineLabel.ElementType.NAUGHT:
                sprite_name = "mu";
                break;
            case MasterDataDefineLabel.ElementType.FIRE:
                sprite_name = "hi";
                break;
            case MasterDataDefineLabel.ElementType.WATER:
                sprite_name = "mizu";
                break;
            case MasterDataDefineLabel.ElementType.LIGHT:
                sprite_name = "hikari";
                break;
            case MasterDataDefineLabel.ElementType.DARK:
                sprite_name = "yami";
                break;
            case MasterDataDefineLabel.ElementType.WIND:
                sprite_name = "kaze";
                break;
            case MasterDataDefineLabel.ElementType.HEAL:
                sprite_name = "kaifuku";
                break;
        }

        if (sprite_name.IsNullOrEmpty()) { return null; }

        return ResourceManager.Instance.Load(sprite_name, ResourceType.Battle);

    }


    /// <summary>
    /// フィルタの選択状態をチェック
    /// </summary>
    /// <param name="elementType"></param>
    /// <returns></returns>
    bool CheckSelectElement(MasterDataDefineLabel.ElementType elementType, LocalSaveSortInfo sortInfo)
    {
        if (sortInfo == null) { return false; }
        if (sortInfo.m_FilterElements == null || sortInfo.m_FilterElements.Length == 0) { return false; }
        return (Array.IndexOf(sortInfo.m_FilterElements, (int)elementType) >= 0);
    }


    /// <summary>
    /// ソートの選択状態を更新
    /// </summary>
    /// <param name="sortInfo"></param>
    public void UpdateSort(LocalSaveSortInfo sortInfo)
    {
        if (sortInfo != null && SortButtons != null)
        {
            for (int i = 0; i < SortButtons.Count; i++)
            {
                SortButtons[i].IsSelect = (sortInfo.m_SortType == (int)SortButtons[i].SortType);
            }

            IsAscOrder = sortInfo.m_SortIsAscOrder;
        }
    }

    /// <summary>
    /// フィルターの選択状態を更新
    /// </summary>
    /// <param name="sortInfo"></param>
    public void UpdateFilter(LocalSaveSortInfo sortInfo)
    {
        RareButton.SetSelect(CheckSelectFilter(MAINMENU_FILTER_TYPE.FILTER_RARE, sortInfo));
        KindButton.SetSelect(CheckSelectFilter(MAINMENU_FILTER_TYPE.FILTER_KIND, sortInfo));

        for (int i = 0; i < Elements.Count; i++)
        {
            Elements[i].IsSelect = CheckSelectElementFilter(Elements[i].ElementType, sortInfo);
        }
    }

    private void SetupOrder(bool is_asc_order)
    {
        AscendingButton.SetSelect(is_asc_order == true);
        DescendingButton.SetSelect(is_asc_order == false);
    }

    public void OnClickAscButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        IsAscOrder = true;
    }

    public void OnClickDescButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        IsAscOrder = false;
    }

    void OnClickFilterButton(MAINMENU_FILTER_TYPE filterType)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        if (OnClickFilterButtonAction != null)
        {
            OnClickFilterButtonAction(filterType);
        }
    }


    void OnClickSortButton(SortDialogTextButtonListContext sortButton)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (OnClickSortButtonAction != null)
        {
            OnClickSortButtonAction(sortButton);
        }
    }

    void OnClickElementButton(SortDialogElementListContext elementButton)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    /// <summary>
    /// 初期状態ボタンを押したとき
    /// </summary>
    public void OnClickResetButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (m_SortDialog != null)
        {
            m_SortDialog.ResetSortParam();
        }
    }

    /// <summary>
    /// 一括ONボタンを押したとき
    /// </summary>
    public void OnClickElementAllOnButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (Elements != null)
        {
            Elements.ConvertAll(v => v.IsSelect = true);
        }
    }

    /// <summary>
    /// 一括OFFボタンを押したとき
    /// </summary>
    public void OnClickElementAllOffButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (Elements != null)
        {
            Elements.ConvertAll(v => v.IsSelect = false);
        }
    }

}
