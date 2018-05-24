using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using System;

public class AreaSelect : MenuPartsBase
{
    [SerializeField]
    private GameObject[] m_switchButtonRoots;
    public List<Sprite> backgroundSprites;

    [SerializeField]
    ScrollRect m_ScrollRect = null;
    public GameObject RegionButtonRoot;

    M4uProperty<Sprite> backGroundImage = new M4uProperty<Sprite>();
    public Sprite BackGroundImage { get { return backGroundImage.Value; } set { backGroundImage.Value = value; } }

    M4uProperty<Sprite> bannerImage = new M4uProperty<Sprite>();
    public Sprite BannerImage { get { return bannerImage.Value; } set { bannerImage.Value = value; } }

    M4uProperty<Sprite> switchImage0 = new M4uProperty<Sprite>();
    public Sprite SwitchImage0 { get { return switchImage0.Value; } set { switchImage0.Value = value; } }

    M4uProperty<string> switchTitle0 = new M4uProperty<string>("コウイキMAP");
    public string SwitchTitle0
    {
        get { return switchTitle0.Value; }
        set
        {
            m_switchButtons[0].labelText = value;
        }
    }

    M4uProperty<Sprite> switchImage1 = new M4uProperty<Sprite>();
    public Sprite SwitchImage1 { get { return switchImage1.Value; } set { switchImage1.Value = value; } }

    M4uProperty<string> switchTitle1 = new M4uProperty<string>("ガクエンMAP");
    public string SwitchTitle1
    {
        get { return switchTitle1.Value; }
        set
        {
            m_switchButtons[1].labelText = value;
        }
    }

    M4uProperty<Sprite> switchImage2 = new M4uProperty<Sprite>();
    public Sprite SwitchImage2 { get { return switchImage2.Value; } set { switchImage2.Value = value; } }

    M4uProperty<string> switchTitle2 = new M4uProperty<string>("イベントMAP");
    public string SwitchTitle2
    {
        get { return switchTitle2.Value; }
        set
        {
            m_switchButtons[2].labelText = value;
        }
    }

    M4uProperty<List<AreaDataContext>> areaDataList = new M4uProperty<List<AreaDataContext>>();
    public List<AreaDataContext> AreaDataList { get { return areaDataList.Value; } set { areaDataList.Value = value; } }

	List<GameObject> areaDataItemList = new List<GameObject>();
	public List<GameObject> AreaDataItemList { get { return areaDataItemList; } set { areaDataItemList = value; } }

	M4uProperty<bool> isActiveScroll = new M4uProperty<bool>(true);
    public bool IsActiveScroll
    {
        get { return isActiveScroll.Value; }
        set
        {
            isActiveScroll.Value = value;
            if (m_ScrollRect != null)
            {
                m_ScrollRect.enabled = value;
            }
        }
    }

    M4uProperty<bool> isViewRegionButton = new M4uProperty<bool>();
    public bool IsViewRegionButton { get { return isViewRegionButton.Value; } set { isViewRegionButton.Value = value; } }

    public System.Action DidSelectBanner = delegate { };

    public System.Action DidSelectAdvent = delegate { };

    public System.Action DidSelectSwitch0 = delegate { };

    public System.Action DidSelectSwitch1 = delegate { };

    public System.Action DidSelectSwitch2 = delegate { };

    public System.Action DidSelectMap = delegate { };

    private MasterDataDefineLabel.AreaCategory m_currentCategory = MasterDataDefineLabel.AreaCategory.RN_STORY;

	private bool m_buttonAnimationFinish = false;
	public bool m_iconAnimationFinish = false;

    public MasterDataDefineLabel.AreaCategory currentCategory
    {
        get
        {
            return m_currentCategory;
        }
        set
        {

            m_currentCategory = value;

            // TODO : 整理
            m_switchButtons[0].isSelected = m_currentCategory == MasterDataDefineLabel.AreaCategory.RN_STORY;
            m_switchButtons[1].isSelected = m_currentCategory == MasterDataDefineLabel.AreaCategory.RN_SCHOOL;
            m_switchButtons[2].isSelected = m_currentCategory == MasterDataDefineLabel.AreaCategory.RN_EVENT;
        }
    }


    private List<AreaSelectSwitchButtonModel> m_switchButtons = new List<AreaSelectSwitchButtonModel>();


    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;

        SetUpSwitchButtons();
    }


    private void SetUpSwitchButtons()
    {
        var indexActionMap = new List<System.Action>
        {
            OnSelectSwitch0,
            OnSelectSwitch1,
            OnSelectSwitch2,
        };

        var tabName = new List<string>
        {
            GameTextUtil.GetText("map_tab1"),
            GameTextUtil.GetText("map_tab2"),
            GameTextUtil.GetText("map_tab3"),
        };

		var areaCategoryLabel = new List<MasterDataDefineLabel.AreaCategory>
		{
			MasterDataDefineLabel.AreaCategory.RN_STORY,
			MasterDataDefineLabel.AreaCategory.RN_SCHOOL,
			MasterDataDefineLabel.AreaCategory.RN_EVENT,
		};

		m_buttonAnimationFinish = false;

		int size = m_switchButtonRoots.Length;
        for (int i = 0; i < size; i++)
        {
            int index = i;
			bool bufEvent = MainMenuUtil.checkHelpBufEvent(areaCategoryLabel[index]);

            var model = new AreaSelectSwitchButtonModel(index);
            model.OnClicked += () =>
            {
				if (model.isSelected)
					return;

                indexActionMap[index]();
            };
            model.OnShowedNext += () =>
            {
				if (index == size - 1)
				{
					m_buttonAnimationFinish = true;
					return;
				}

                m_switchButtons[index + 1].Appear();
            };
            model.labelText = tabName[index];

            AreaSelectSwitchButton
                .Attach(m_switchButtonRoots[index])
                .SetModel(model, bufEvent);

            m_switchButtons.Add(model);
        }

        {
            var model = new ButtonModel();
            model.OnClicked += () =>
            {
                OnSelectMapButton();
            };

            RegionSwitchButton
                .Attach(RegionButtonRoot)
                .SetModel(model);
        }
    }

    /// <summary>
    /// 先頭リージョンボタンのアニメーション開始を行う.
    /// </summary>
    public void AppearSwitchButton()
    {
        if (m_switchButtons[0] != null)
        {
            m_switchButtons[0].Appear();
        }
    }

    public void OnSelectBanner()
    {
        DidSelectBanner();
    }

    public void OnSelectAdvent()
    {
        DidSelectAdvent();
    }

    public void OnSelectSwitch0()
    {
        DidSelectSwitch0();
    }

    public void OnSelectSwitch1()
    {
        DidSelectSwitch1();
    }

    public void OnSelectSwitch2()
    {
        DidSelectSwitch2();
    }

    public void OnSelectMapButton()
    {
        DidSelectMap();
    }

    public void AddAreaData(uint _index, string _title, Sprite _image, Vector2 _pos, System.Action<uint> _action)
    {
        var model = new AreaSelectListItemModel(_index);
        model.OnClicked += () =>
        {
            _action(model.index);
        };

        AreaDataContext newArea = new AreaDataContext(model);
        newArea.m_AreaIndex = _index;
        newArea.Title = _title;
        newArea.IconImage = _image;
        newArea.PosX = _pos.x;
        newArea.PosY = _pos.y;

        AddAreaData(newArea);
    }

    public void AddAreaData(AreaDataContext newArea)
    {
        if (AreaDataList == null)
        {
            AreaDataList = new List<AreaDataContext>();
        }

        AreaDataList.Add(newArea);
    }

    public void ClearAreaData()
    {
        if (AreaDataList == null)
        {
            return;
        }
        AreaDataList.Clear();
		m_iconAnimationFinish = false;
    }

	public void checkAnimationFinish()
	{
		StartCoroutine(WaitAnimationFinish());
	}

	IEnumerator WaitAnimationFinish()
	{
		while(AreaDataItemList.Count < AreaDataList.Count)
		{
			yield return null;
		}
		while(m_buttonAnimationFinish == false || m_iconAnimationFinish == false)
		{
			yield return null;
		}
		for (int i = 0; i < m_switchButtonRoots.Length; ++i)
		{
			AreaSelectSwitchButton button = m_switchButtonRoots[i].GetComponentInChildren<AreaSelectSwitchButton>();
			if (button != null)
			{
				button.m_bufEventAnimationStart = true;
				if (m_switchButtons[i].isSelected == false)
				{
					button.setBufEvent(false);
					button.setBufEvent(true, MainMenuManager.Instance.Footer.getFooterBufAnimationTime());
				}
				else
				{
					button.setBufEvent(false);
				}
			}
		}
		for(int i = 0;i < AreaDataItemList.Count;++i)
		{
			AreaDataListItem item = AreaDataItemList[i].GetComponent<AreaDataListItem>();
			if (item != null)
			{
				item.setBufEvent(true, MainMenuManager.Instance.Footer.getFooterBufAnimationTime());
			}
		}
	}
}
