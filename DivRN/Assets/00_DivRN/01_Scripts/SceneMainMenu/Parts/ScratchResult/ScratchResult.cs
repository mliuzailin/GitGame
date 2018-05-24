using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class ScratchResult : MenuPartsBase
{
    [SerializeField]
    ButtonView m_ReturnButton;

    public ScratchPanel[] scratchPanelList = null;
    public CanvasGroup omakeBG = null;

    M4uProperty<Sprite> mainPanel = new M4uProperty<Sprite>();
    public Sprite MainPanel { get { return mainPanel.Value; } set { mainPanel.Value = value; } }

    M4uProperty<bool> isViewSubPanel = new M4uProperty<bool>();
    public bool IsViewSubPanel { get { return isViewSubPanel.Value; } set { isViewSubPanel.Value = value; } }

    M4uProperty<Sprite> subPanel = new M4uProperty<Sprite>();
    public Sprite SubPanel { get { return subPanel.Value; } set { subPanel.Value = value; } }

    M4uProperty<Sprite> subLabel = new M4uProperty<Sprite>();
    public Sprite SubLabel { get { return subLabel.Value; } set { subLabel.Value = value; } }

    M4uProperty<List<OmakeDataContext>> omakeList = new M4uProperty<List<OmakeDataContext>>();
    public List<OmakeDataContext> OmakeList { get { return omakeList.Value; } set { omakeList.Value = value; } }

    M4uProperty<string> buttonLabel = new M4uProperty<string>();
    public string ButtonLabel { get { return buttonLabel.Value; } set { buttonLabel.Value = value; } }

    M4uProperty<bool> isViewSkipButton = new M4uProperty<bool>();
    public bool IsViewSkipButton { get { return isViewSkipButton.Value; } set { isViewSkipButton.Value = value; } }

    M4uProperty<bool> isUnitDetail = new M4uProperty<bool>();
    public bool IsUnitDetail { get { return isUnitDetail.Value; } set { isUnitDetail.Value = value; } }

    M4uProperty<string> unitDetailLabel = new M4uProperty<string>();
    public string UnitDetailLabel { get { return unitDetailLabel.Value; } set { unitDetailLabel.Value = value; } }

    M4uProperty<bool> isViewReturnButton = new M4uProperty<bool>(false);
    public bool IsViewReturnButton { get { return isViewReturnButton.Value; } set { isViewReturnButton.Value = value; } }

    public System.Action DidSelectSkip = delegate { };
    public System.Action DidSelectReturn = delegate { };

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        OmakeList = new List<OmakeDataContext>();
        IsViewSubPanel = false;
        IsViewSkipButton = false;
        SetUpButtons();
    }

    private void Start()
    {
        if (SafeAreaControl.HasInstance)
        {
            GameObject go = GameObject.Find(name);
            SafeAreaControl.Instance.addLocalYPos(go.transform);
        }
    }

    void SetUpButtons()
    {
        var returnButtonModel = new ButtonModel();
        m_ReturnButton.SetModel(returnButtonModel);

        returnButtonModel.OnClicked += () =>
        {
            OnSelectReturn();
        };

        returnButtonModel.Appear();
        returnButtonModel.SkipAppearing();
    }

    public void OnSelectSkip()
    {
        DidSelectSkip();
    }

    void OnSelectReturn()
    {
        if (DidSelectReturn != null)
        {
            DidSelectReturn();
        }
    }
}
