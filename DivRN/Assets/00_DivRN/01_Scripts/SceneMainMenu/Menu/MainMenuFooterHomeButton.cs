using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;

public class MainMenuFooterHomeButton : ButtonView
{
    [SerializeField]
    protected Image m_buttonBaseImage;

    public static readonly string PrefabPath = "Prefab/MainMenu/MainMenuFooterHomeButton";
    public static readonly string BaseImagePath = "btn_home";

    private MainMenuFooterButtonModel m_model = null;


    public static MainMenuFooterHomeButton Attach(GameObject parent)
    {
        return ButtonView.Attach<MainMenuFooterHomeButton>(PrefabPath, parent);
    }


    public MainMenuFooterHomeButton SetModel(MainMenuFooterButtonModel model)
    {
        m_model = model;

        m_model.OnUpdated += () =>
        {
            SetTextures();
        };


        base.SetModel<MainMenuFooterButtonModel>(m_model);

        return this;
    }

    void Awake()
    {
        AppearAnimationName = "mainmenu_footer_button_appear";
        DefaultAnimationName = "mainmenu_footer_home_button_loop";
        ClickAnimationName = "mainmenu_footer_button_click";
    }

    void Start()
    {
        SetTextures();
    }

    new public void Click()
    {
        if (MainMenuManager.Instance.CheckMenuControlNG() ||
             MainMenuManager.Instance.IsPageSwitch())
        {
            return;
        }

        base.Click();
    }

    private void SetTextures()
    {
        string imagePath = BaseImagePath;
        if (m_model.isEnabled == false)
        {
            imagePath += "_off";
        }
        else if (m_model.isSelected)
        {
            imagePath += "_down";
        }

        m_buttonBaseImage.sprite = ResourceManager.Instance.Load(imagePath);
    }
}
