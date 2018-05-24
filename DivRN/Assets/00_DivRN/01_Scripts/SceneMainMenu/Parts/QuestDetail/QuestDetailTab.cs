using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class QuestDetailTab : MenuPartsBase
{
    [SerializeField]
    private GameObject m_switchButtonRoot;
    [SerializeField]
    private GameObject m_tabListRoot;

    private static readonly string AppearAnimationName = "quest_detail_tab_appear";
    private static readonly string DefaultAnimationName = "quest_detail_tab_loop";


    public System.Action<QuestDetailTabContext> DidTabChenged = delegate { };

    M4uProperty<List<QuestDetailTabContext>> tabList = new M4uProperty<List<QuestDetailTabContext>>();
    public List<QuestDetailTabContext> TabList { get { return tabList.Value; } set { tabList.Value = value; } }


    private bool m_isShowed = false;

    private int tabIndex = 0;
    private int tabMax = 1;

    private List<QuestDetailTabModel> m_tabs = new List<QuestDetailTabModel>();


    public int currentIndex { get { return tabIndex; } }
    public bool m_IsReady = false;

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        TabList = new List<QuestDetailTabContext>();

        tabIndex = 0;
    }

    private void Start()
    {
        if (SafeAreaControl.Instance)
        {
            SafeAreaControl.Instance.addLocalYPos(transform);
        }
    }

    public void Clear()
    {
        tabIndex = 0;
        TabList.Clear();
    }

    public void AddTab(string _title, QuestDetailModel.TabType _type)
    {
        int _index = TabList.Count;

        var model = new QuestDetailTabModel((uint)_index);
        model.OnShowedNext += () =>
        {
            if (_index + 1 >= m_tabs.Count)
                return;

            m_tabs[_index + 1].Appear();
        };

        QuestDetailTabContext newTab = new QuestDetailTabContext(model);
        newTab.TitleOn = string.Format(GameTextUtil.GetText("stmina_bahutext"), _title);
        newTab.TitleOff = _title;
        newTab.m_Type = _type;
        newTab.m_Index = _index;
        newTab.DidSelectTab += OnSelectTab;
        if (_index == tabIndex)
        {
            newTab.IsSelected = true;
        }
        else
        {
            newTab.IsSelected = false;
        }
        TabList.Add(newTab);
        tabMax = TabList.Count;

        m_tabs.Add(model);
    }


    public void Show()
    {
        if (m_isShowed)
            return;
        m_isShowed = true;


        PlayAnimation(AppearAnimationName, () =>
        {
            if (m_tabs.Count > 0)
                m_tabs[0].Appear();

            PlayAnimation(DefaultAnimationName);
        });
    }

    public void Hide()
    {
        m_isShowed = false;
        m_switchButtonRoot.SetActive(false);
        m_tabListRoot.SetActive(false);

        tabIndex = 0;

        if (ButtonBlocker.Instance.IsActive())
            ButtonBlocker.Instance.Unblock();
    }

    private void ChengeTabIndex()
    {
        for (int i = 0; i < TabList.Count; i++)
        {
            if (i == tabIndex)
            {
                TabList[i].IsSelected = true;
            }
            else
            {
                TabList[i].IsSelected = false;
            }
        }
    }

    public void OnSelectTab(QuestDetailTabContext _tab)
    {
        if (m_IsReady == false)
        {
            return;
        }

        if (ServerApi.IsExists)
        {
            return;
        }

        if (ButtonBlocker.Instance.IsActive())
        {
            return;
        }
        ButtonBlocker.Instance.Block();

        tabIndex = _tab.m_Index;
        ChengeTabIndex();
        DidTabChenged(_tab);
    }

    public void OnChengedTab()
    {
        if (m_IsReady == false)
        {
            return;
        }

        if (ServerApi.IsExists)
        {
            return;
        }

        if (ButtonBlocker.Instance.IsActive())
        {
            return;
        }
        ButtonBlocker.Instance.Block();

        if ((tabIndex + 1) >= tabMax)
        {
            tabIndex = 0;
        }
        else
        {
            tabIndex++;
        }

        DidTabChenged(TabList[tabIndex]);
    }

    public void ActivateButton()
    {
        ButtonBlocker.Instance.Unblock();
    }
}
