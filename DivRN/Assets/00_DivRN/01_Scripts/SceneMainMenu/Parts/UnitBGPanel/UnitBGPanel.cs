using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;
using M4u;

public class UnitBGPanel : MenuPartsBase
{
    [SerializeField]
    private GameObject m_executeButtonRoot;
    [SerializeField]
    private GameObject m_returnButtonRoot;

    private static readonly string ExecuteButtonPrefabPath = "Prefab/UnitBGPanel/UnitPanelExecuteButton";
    private static readonly string ReturnButtonPrefabPath = "Prefab/UnitBGPanel/UnitPanelReturnButton";

    [SerializeField]
    GameObject m_unitIconRoot;
    [SerializeField]
    GameObject m_evolUnitIconRoot;

    M4uProperty<bool> isViewPanel = new M4uProperty<bool>();
    public bool IsViewPanel { get { return isViewPanel.Value; } set { isViewPanel.Value = value; } }

    M4uProperty<bool> isViewExecButton = new M4uProperty<bool>();
    public bool IsViewExecButton { get { return isViewExecButton.Value; } set { isViewExecButton.Value = value; } }

    M4uProperty<bool> isActiveExecButton = new M4uProperty<bool>();
    public bool IsActiveExecButton { get { return isActiveExecButton.Value; } set { isActiveExecButton.Value = value; } }

    M4uProperty<bool> isViewReturnButton = new M4uProperty<bool>();
    public bool IsViewReturnButton { get { return isViewReturnButton.Value; } set { isViewReturnButton.Value = value; } }

    M4uProperty<bool> isViewIcon = new M4uProperty<bool>();
    public bool IsViewIcon { get { return isViewIcon.Value; } set { isViewIcon.Value = value; } }

    M4uProperty<bool> isViewResetButton = new M4uProperty<bool>();
    public bool IsViewResetButton { get { return isViewResetButton.Value; } set { isViewResetButton.Value = value; } }

    M4uProperty<bool> isEnableResetButton = new M4uProperty<bool>();
    public bool IsEnableResetButton { get { return isEnableResetButton.Value; } set { isEnableResetButton.Value = value; } }

    M4uProperty<int> money = new M4uProperty<int>();
    public int Money { get { return money.Value; } set { money.Value = value; } }

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<Sprite> execButtonImage = new M4uProperty<Sprite>();
    public Sprite ExecButtonImage { get { return execButtonImage.Value; } set { execButtonImage.Value = value; } }

    M4uProperty<bool> isViewEvolve = new M4uProperty<bool>();
    public bool IsViewEvolve { get { return isViewEvolve.Value; } set { isViewEvolve.Value = value; } }

    M4uProperty<bool> isActiveLink = new M4uProperty<bool>();
    public bool IsActiveLink { get { return isActiveLink.Value; } set { isActiveLink.Value = value; } }

    M4uProperty<bool> isViewSale = new M4uProperty<bool>();
    public bool IsViewSale { get { return isViewSale.Value; } set { isViewSale.Value = value; } }

    M4uProperty<int> saleCount = new M4uProperty<int>();
    public int SaleCount { get { return saleCount.Value; } set { saleCount.Value = value; } }

    M4uProperty<int> saleCountMax = new M4uProperty<int>();
    public int SaleCountMax { get { return saleCountMax.Value; } set { saleCountMax.Value = value; } }

    M4uProperty<int> point = new M4uProperty<int>();
    public int Point { get { return point.Value; } set { point.Value = value; } }

    M4uProperty<string> message = new M4uProperty<string>();
    public string Message { get { return message.Value; } set { message.Value = value; } }

    M4uProperty<bool> isViewPointEvolve = new M4uProperty<bool>();
    public bool IsViewPointEvolve { get { return isViewPointEvolve.Value; } set { isViewPointEvolve.Value = value; } }

    M4uProperty<int> totalPoint = new M4uProperty<int>();
    public int TotalPoint { get { return totalPoint.Value; } set { totalPoint.Value = value; } }

    M4uProperty<string> totalTitle = new M4uProperty<string>();
    public string TotalTitle { get { return totalTitle.Value; } set { totalTitle.Value = value; } }

    M4uProperty<string> evolve_arrow = new M4uProperty<string>();
    public string Evolve_arrow { get { return evolve_arrow.Value; } set { evolve_arrow.Value = value; } }

    M4uProperty<bool> isDetailButton = new M4uProperty<bool>();
    public bool IsDetailButton { get { return isDetailButton.Value; } set { isDetailButton.Value = value; } }

	M4uProperty<bool> isLinkStatus = new M4uProperty<bool>();
	public bool IsLinkStatus { get { return isLinkStatus.Value; } set { isLinkStatus.Value = value; } }

	M4uProperty<Sprite> linkStatusImage = new M4uProperty<Sprite>();
	public Sprite LinkStatusImage { get { return linkStatusImage.Value; } set { linkStatusImage.Value = value; } }

	M4uProperty<Sprite> linkSkillImage = new M4uProperty<Sprite>();
	public Sprite LinkSkillImage { get { return linkSkillImage.Value; } set { linkSkillImage.Value = value; } }

	public UnitBGPanelUnitIcon UnitIcon;
    public UnitBGPanelUnitIcon EvolUnitIcon;

    public System.Action DidSelect = delegate { };
    public System.Action DidReset = delegate { };
    public System.Action DidReturn = delegate { };
    public System.Action DidSelectIcon = delegate { };
    public System.Action DidSelectIconLongpress = delegate { };
    public System.Action DidSelectEvolveIcon = delegate { };
    public System.Action DidSelectEvolveIconLongpress = delegate { };
    public System.Action OnClickDetailAction = delegate { };
	public System.Action OnClickLinkStatusAction = delegate { };
	public System.Action OnClickLinkSkillAction = delegate { };

	private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        IsViewPanel = false;
        IsViewExecButton = false;
        IsActiveExecButton = true;
        IsViewReturnButton = false;
        IsViewEvolve = false;
        IsViewIcon = true;
        IsViewResetButton = true;
        IsEnableResetButton = false;
        IsActiveLink = false;
        IsViewSale = false;
		IsLinkStatus = false;
		Title = "";
        ExecButtonImage = null;

        SetUpButtons();

        //-----------------------------------------------------
        // ユニットアイコンの生成
        //-----------------------------------------------------
        UnitIcon = UnitBGPanelUnitIcon.Create(m_unitIconRoot.transform);
        UnitIcon.IsActiveIcon = false;
        UnitIcon.IconColor = new Color(1, 1, 1, 1);
        UnitIcon.DidSelectIcon = OnSelectIcon;
        UnitIcon.DidSelectIconLongpress = OnSelectIconLongpress;

        EvolUnitIcon = UnitBGPanelUnitIcon.Create(m_evolUnitIconRoot.transform);
        EvolUnitIcon.IsActiveIcon = false;
        EvolUnitIcon.DidSelectIcon = OnSelectEvolveIcon;
        EvolUnitIcon.DidSelectIconLongpress = OnSelectEvolveIconLongpress;

		LinkStatusImage = ResourceManager.Instance.Load("btn_status_off", ResourceType.Common);
		LinkSkillImage = ResourceManager.Instance.Load("btn_skill_off", ResourceType.Common);
	}

	public void setupBaseUnit(MasterDataParamChara _master, PacketStructUnit _unit_data)
    {
        if (_master == null)
        {
            return;
        }

        if (_unit_data == null)
        {
            return;
        }

        UnitIcon.IsActiveIcon = true;
        UnitIconImageProvider.Instance.Get(
            _master.fix_id,
            sprite =>
            {
                UnitIcon.IconImage = sprite;
            },
            true);
        UnitIcon.LinkIcon = MainMenuUtil.GetLinkMark(_unit_data);
        UnitIcon.chara_fix_id = _master.fix_id;
        UnitIcon.CharaNo = _master.draw_id;
        UnitIcon.Attribute_circle = MainMenuUtil.GetElementCircleSprite(_master.element);
	}

	public void resetBaseUnit()
    {
        UnitIconImageProvider.Instance.Reset(UnitIcon.chara_fix_id);

        UnitIcon.chara_fix_id = 0;
        UnitIcon.IsActiveIcon = false;
        UnitIcon.IconImage = null;
        UnitIcon.LinkIcon = null;
	}

	public void setupEvolveUnit(MasterDataParamChara _master, PacketStructUnit _unit_data = null)
    {
        if (_master == null)
        {
            return;
        }

        EvolUnitIcon.IsActiveIcon = true;
        UnitIconImageProvider.Instance.Get(
            _master.fix_id,
            sprite =>
            {
                EvolUnitIcon.IconImage = sprite;
            },
            true);

        if (_unit_data != null)
        {
            EvolUnitIcon.LinkIcon = MainMenuUtil.GetLinkMark(_unit_data);
        }
        EvolUnitIcon.CharaNo = _master.draw_id;
        EvolUnitIcon.chara_fix_id = _master.fix_id;

        EvolUnitIcon.Attribute_circle = MainMenuUtil.GetElementCircleSprite(_master.element);
    }

    public void resetEvolveUnit()
    {
        UnitIconImageProvider.Instance.Reset(EvolUnitIcon.chara_fix_id);

        EvolUnitIcon.chara_fix_id = 0;
        EvolUnitIcon.IsActiveIcon = false;
        EvolUnitIcon.IconImage = null;
        EvolUnitIcon.LinkIcon = null;
    }

    public void OnSelect()
    {
        DidSelect();
    }

    public void OnReset()
    {
        DidReset();
    }

    public void OnRetuen()
    {
        DidReturn();
    }

    private void OnSelectIcon()
    {
        DidSelectIcon();
    }

    private void OnSelectIconLongpress()
    {
        DidSelectIconLongpress();
    }

    private void OnSelectEvolveIcon()
    {
        DidSelectEvolveIcon();
    }
    private void OnSelectEvolveIconLongpress()
    {
        DidSelectEvolveIconLongpress();
    }


    private void SetUpButtons()
    {
        var executeButtonModel = new ButtonModel();
        var returnButtonModel = new ButtonModel();

        ButtonView
            .Attach<ButtonView>(ExecuteButtonPrefabPath, m_executeButtonRoot)
            .SetModel<ButtonModel>(executeButtonModel);
        ButtonView
            .Attach<ButtonView>(ReturnButtonPrefabPath, m_returnButtonRoot)
            .SetModel<ButtonModel>(returnButtonModel);

        executeButtonModel.OnClicked += () =>
        {
            OnSelect();
        };
        returnButtonModel.OnClicked += () =>
        {
            OnRetuen();
        };


        // TODO : 演出を入れるならそこに移動
        executeButtonModel.Appear();
        executeButtonModel.SkipAppearing();
        returnButtonModel.Appear();
        returnButtonModel.SkipAppearing();
    }

    public void OnClickDetailButton()
    {
        if (OnClickDetailAction != null)
        {
            OnClickDetailAction();
        }
    }

	public void OnClickLinkStatusButton()
	{
		if (OnClickLinkStatusAction != null)
		{
			OnClickLinkStatusAction();
		}
	}

	public void OnClickLinkSkillButton()
	{
		if (OnClickLinkSkillAction != null)
		{
			OnClickLinkSkillAction();
		}
	}
}