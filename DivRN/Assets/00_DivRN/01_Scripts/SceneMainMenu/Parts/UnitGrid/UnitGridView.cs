using UnityEngine;
using System;
using System.Collections.Generic;
using M4u;
using DG.Tweening;

public class UnitGridView : MenuPartsBase
{
    /////////UnitGridViewBase
    public Action<UnitGridContext> ClickUnitAction = delegate { };
    public Action<UnitGridContext> LongPressUnitAction = delegate { };

    public Action<UnitGridContext> SetupUnitIconType = delegate { };
    public Action<UnitGridContext> SetupUnitSelected = delegate { };


    public delegate void UnitEventHandler(UnitGridContext unitGridContext);
    public event UnitEventHandler OnUnitGridContextUpdated;

    public delegate void EventHandler();
    public event EventHandler OnUnitGridSetUp;

    /// <summary>所持しているユニット用</summary>
    List<UnitGridContext> units = new List<UnitGridContext>();
    public List<UnitGridContext> Units
    {
        get { return units; }
        set
        {
            units = value;
        }
    }

    /// <summary>リスト表示用 model</summary>
    private List<ListItemModel> m_units = new List<ListItemModel>();
    /// <summary>リスト表示用 view model</summary>
    private List<UnitGridContext> m_unitViewModels = new List<UnitGridContext>();


    /// <summary>ステータス切り替え用のタイマー</summary>
    private float m_SwitchStatusUpdateTime;
    /// <summary>ステータスの切り替え回数</summary>
    private int m_SwitchStatusCount;

    private float m_gridViewHight = 0;
    public float GridViewHight { get { return m_gridViewHight; } set { m_gridViewHight = value; } }

    /////////

    [SerializeField]
    private UnitGridViewModelContainer m_listContainer = null;

    [SerializeField]
    private float m_gridWidth = 110;
    [SerializeField]
    private float m_gridHeight = 130;
    [SerializeField]
    private int m_horizontalCount = 5;
    [SerializeField]
    private int m_verticalCount = 16;

    private static readonly string GridViewPrefabPath = "Prefab/UnitGrid/UnitGridView";

    private UnitGridManager m_unitGridManager = null;

    private static UnityEngine.UI.ScrollRect m_scrollRect = null;

    //////

    void Update()
    {
        //---------------------------------------------
        // ユニットのステータス切り替え
        //---------------------------------------------
        m_SwitchStatusUpdateTime += Time.deltaTime;

        float displayCount = Mathf.Clamp01(m_SwitchStatusUpdateTime / 5.0f);
        if ((int)displayCount == 1)
        {
            ++m_SwitchStatusCount;
            m_SwitchStatusUpdateTime = 0.0f;

            for (int i = 0; i < m_unitViewModels.Count; i++)
            {
                var viewModel = m_unitViewModels[i];
                viewModel.ChangeSwitchStatus(m_SwitchStatusCount);
            }
        }
    }

    // ============================== factory functions

    public void Attach(GameObject parent)
    {
        var node = GetRoot();

        // keep default rect transform
        var rectTransform = node.GetComponent<RectTransform>();
        var position = rectTransform.anchoredPosition3D;
        var sizeDelta = rectTransform.sizeDelta;
        var scale = rectTransform.localScale;

        // change parent and apply default rect transform
        node.transform.SetParent(parent.transform);
        rectTransform.anchoredPosition3D = position;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.localScale = scale;
    }

    new public static UnitGridView Create()
    {
        var prefab = Resources.Load<GameObject>(GridViewPrefabPath);
        Debug.Assert(prefab != null, "prefab : " + GridViewPrefabPath + " not found");
        var node = GameObject.Instantiate(prefab);
        Debug.Assert(node != null, "failed to Instantiate prefab : " + GridViewPrefabPath);
        var view = node.GetComponent<UnitGridView>();
        Debug.Assert(view != null, "Component of UnitGridView not found");

        m_scrollRect = view.GetComponent<UnityEngine.UI.ScrollRect>();

        return view;
    }

    public static GameObject CreateUnitListItem()
    {
        var root = GameObject.Find("/ListItemPool");
        if (root == null)
        {
            var prefab = Resources.Load<GameObject>(GridViewPrefabPath);
            Debug.Assert(prefab != null, "prefab : " + GridViewPrefabPath + " not found");
            var node = GameObject.Instantiate(prefab);
            Debug.Assert(node != null, "failed to Instantiate prefab : " + GridViewPrefabPath);
            var view = node.GetComponent<UnitGridView>();
            Debug.Assert(view != null, "Component of UnitGridView not found");
            List<UnitGridContext> unitList = MainMenuUtil.MakeUnitGridContextList();
            if (unitList.Count > 0)
            {
                root = new GameObject("ListItemPool");
                for (int i = 0; i < view.m_horizontalCount * view.m_verticalCount; i++)
                {
                    var go = Instantiate(Resources.Load("Prefab/UnitGrid/UnitListItem"), root.transform) as GameObject;
                    var cc = go.GetComponent<M4uContextRoot>();
                    cc.Context = (M4uContextInterface)unitList[0];
                    go.name = "UnitListItem";
                }
            }
            Destroy(node);
        }
        return root;
    }

    // ===================================== public functions

    private bool m_initialized = false;
    public void Initialize()
    {
        m_initialized = true;

        if (m_unitGridManager == null)
        {
            m_unitGridManager = new UnitGridManager(
                m_gridWidth,
                m_gridHeight,
                m_horizontalCount,
                m_verticalCount);
        }

        SetUpListItems();
    }

    public void UpdateList()
    {
        // memo DG0-3225
        // UnitIconImageProvider.Instance.Stop()を呼び出すとアイコンが？になる問題がある
        // UnitBGPanelなどリスト外でアイコンを表示するものなど

        for (int i = 0; i < m_units.Count; i++)
        {
            var unit = m_units[i];
            var index = (int)unit.index;
            ApplyUnitData(m_unitGridManager.GetModifiedIndex(index), m_unitViewModels[index]);
        }

        UnitIconImageProvider.Instance.Tick();
    }

    public void UpdateItem(UnitGridContext item)
    {
        int index = (int)item.model.index;
        int modifiedIndex = m_unitGridManager.GetModifiedIndex(index);
        if (modifiedIndex >= 0 &&
            modifiedIndex < Units.Count)
        {
            var unit = Units[modifiedIndex];
            unit.Copy(item);
        }
    }

    public void SelectStatusDisplayType(MAINMENU_SORT_SEQ sortType, MAINMENU_SORT_SEQ[] favoriteSortTypes)
    {
        m_SwitchStatusCount = 0;
        m_SwitchStatusUpdateTime = 0;

        for (int i = 0; i < m_unitViewModels.Count; i++)
        {
            var viewModel = m_unitViewModels[i];
            viewModel.SetStatus(sortType, favoriteSortTypes);
        }
    }

    public void CreateList()
    {
        if (m_initialized)
        {
            UpdateListItems();
            return;
        }

        Initialize();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="unique_id"></param>
    /// <returns></returns>
    public UnitGridContext GetItem(long unique_id)
    {
        return Units.Find(a => a.UnitData.unique_id == unique_id);
    }

    // ================================= private functions

    private void SetUpListItems()
    {
        Debug.Assert(m_units.Count == 0, "SetUpListItems called twice.");
        Debug.Assert(m_unitViewModels.Count == 0, "SetUpListItems called twice.");

        int unitCount = Units.Count;
        m_unitGridManager.UpdateElementCount(unitCount);
        m_listContainer.SetHeight(m_unitGridManager.GetScrollRectHeight());

        System.Func<bool> IsAllStarted = () =>
        {
            if (m_units.Count < m_unitGridManager.GetGridCount())
            {
                return false;
            }

            for (int i = 0; i < m_units.Count; i++)
            {
                var unit = m_units[i];
                if (!unit.isStarted)
                {
                    return false;
                }
            }
            return true;
        };

        UnitIconImageProvider.Instance.Stop();

        for (int i = 0; i < m_unitGridManager.GetGridCount(); i++)
        {
            int index = i;
            var model = new ListItemModel((uint)i);
            var viewModel = new UnitGridContext(model);

            m_listContainer.Items.Add(viewModel);

            m_unitViewModels.Add(viewModel);

            model.OnStarted += () =>
            {
                model.SetPosition(m_unitGridManager.GetInitialPosition(index));

                ApplyUnitData(m_unitGridManager.GetModifiedIndex(index), viewModel);

                if (IsAllStarted())
                {
                    AllStarted();
                }
            };

            model.OnClicked += () =>
            {
                if (ClickUnitAction != null)
                {
                    var tag = "UnitGridOpenUnitDetail";
                    ButtonBlocker.Instance.Block(tag);
                    ClickUnitAction(viewModel);
                    ButtonBlocker.Instance.Unblock(tag);
                }
            };

            model.OnLongPressed += () =>
            {
                if (LongPressUnitAction != null)
                {
                    LongPressUnitAction(viewModel);
                }
            };

            m_units.Add(model);
        }

        UnitIconImageProvider.Instance.Tick();
    }

    private void UpdateListItems()
    {
        var oldScrollHeight = m_unitGridManager.GetScrollRectHeight();

        int unitCount = Units.Count;
        m_unitGridManager.UpdateElementCount(unitCount);

        var newScrollHeight = m_unitGridManager.GetScrollRectHeight();
        m_listContainer.UpdateHeight(newScrollHeight);

        UnitIconImageProvider.Instance.Stop();

        for (int i = 0; i < m_units.Count; i++)
        {
            var unit = m_units[i];
            var index = (int)unit.index;

            ApplyUnitData(m_unitGridManager.GetModifiedIndex(index), m_unitViewModels[index]);

            if (oldScrollHeight != newScrollHeight)
            {
                unit.MoveBy(Vector2.up * (newScrollHeight - oldScrollHeight) / 2);
            }
        }

        UnitIconImageProvider.Instance.Tick();

        if (oldScrollHeight > newScrollHeight)
        {
            if (m_scrollRect.verticalNormalizedPosition < 0)
            {
                m_scrollRect.verticalNormalizedPosition = 0;
            }
            else if (newScrollHeight < GridViewHight)
            {
                m_scrollRect.verticalNormalizedPosition = 1;
            }
        }
    }

    private void ApplyUnitData(int modifiedIndex, UnitGridContext viewModel)
    {
        if (modifiedIndex >= 0
            && modifiedIndex < Units.Count)
        {
            var unit = Units[modifiedIndex];
            viewModel.Copy(unit);
            viewModel.IsView = true;

            //
            if (unit.UnitData != null &&
                unit.UnitData.id != 0)
            {
                viewModel.m_SpriteName = string.Empty;
                UnitIconImageProvider.Instance.Get(
                    viewModel.UnitData.id,
                    ref viewModel.m_SpriteName,
                    sprite =>
                    {
                        if (MainMenuUtil.IsWriteIcon(ref viewModel.m_SpriteName, sprite))
                        {
                            viewModel.SetIconImageDirectly(sprite);
                        }
                    });
            }

            SetupUnitIconType(viewModel);
            SetupUnitSelected(viewModel);

            UnitGridContextUpdated(viewModel);
        }
        else
        {
            viewModel.IsView = false;
        }
    }


    private float m_oldScrolledValue = 0;
    // ================================= called by ScrollRect component
    public void OnScrolled()
    {
        var currentScrolledValue = m_unitGridManager.GetScrollRectHeight() * (1 - m_scrollRect.verticalNormalizedPosition);

        if (currentScrolledValue != m_oldScrolledValue)
        {
            UnitIconImageProvider.Instance.Stop();

            m_unitGridManager.Shift(currentScrolledValue,
                (index, delta) =>
                {
                    if (delta == 0)
                    {
                        return;
                    }

                    float height = m_unitGridManager.GetGridsRectHeight();
                    Vector2 vec2 = delta > 0
                        ? Vector2.down * height
                        : Vector2.up * height;
                    m_units[index].MoveBy(vec2);
                },
                (index, delta) =>
                {
                    ApplyUnitData(m_unitGridManager.GetModifiedIndex(index), m_unitViewModels[index]);
                });
        }

        m_oldScrolledValue = currentScrolledValue;
    }

    public void checkNormalizedPosition(bool bTop)
    {
        if (m_scrollRect.verticalNormalizedPosition < 0 ||
            (bTop == true && m_scrollRect.content.offsetMax.y > 0))
        {
            if (bTop == false)
            {
                m_scrollRect.verticalNormalizedPosition = 0;
            }
            else
            {
                m_scrollRect.verticalNormalizedPosition = 1;
            }
        }

        if (m_scrollRect.verticalNormalizedPosition > 1)
        {
            m_scrollRect.verticalNormalizedPosition = 1;
        }
    }

    // ===================================== rotected functions

    protected void AllStarted()
    {
        if (OnUnitGridSetUp != null)
        {
            OnUnitGridSetUp();
        }
    }

    protected void UnitGridContextUpdated(UnitGridContext unitGridContext)
    {
        if (OnUnitGridContextUpdated != null)
        {
            OnUnitGridContextUpdated(unitGridContext);
        }
    }
}
