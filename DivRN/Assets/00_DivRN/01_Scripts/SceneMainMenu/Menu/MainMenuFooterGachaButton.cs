using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;

public class MainMenuFooterGachaButton : ButtonView
{
    [SerializeField]
    protected Image m_buttonBaseImage;
    [SerializeField]
    protected Image m_FlagImage;

    public static readonly string PrefabPath = "Prefab/MainMenu/MainMenuFooterGachaButton";
    public static readonly string BaseImagePath = "btn_scratch";

    private MainMenuFooterButtonModel m_model = null;


    public static MainMenuFooterGachaButton Attach(GameObject parent)
    {
        return ButtonView.Attach<MainMenuFooterGachaButton>(PrefabPath, parent);
    }


    public MainMenuFooterGachaButton SetModel(MainMenuFooterButtonModel model)
    {
        m_model = model;

        m_model.OnUpdated += () =>
        {
            SetTextures();
        };

        base.SetModel<MainMenuFooterButtonModel>(m_model);

        if (m_FlagImage != null)
        {
            m_FlagImage.enabled = false;
        }

        return this;
    }

    void Awake()
    {
        AppearAnimationName = "mainmenu_footer_button_appear";
        DefaultAnimationName = "mainmenu_footer_gacha_button_loop";
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

    public void SetFlag(bool bFlag)
    {
        if (m_FlagImage != null)
        {
            m_FlagImage.enabled = bFlag;
        }
    }
}
