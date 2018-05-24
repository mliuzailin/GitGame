﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;

public class MainMenuFooterUnitsButton : ButtonView
{
    [SerializeField]
    protected Image m_buttonBaseImage;
    [SerializeField]
    private Animation m_bufEventAnimation;

    public static readonly string PrefabPath = "Prefab/MainMenu/MainMenuFooterUnitsButton";
    public static readonly string BaseImagePath = "btn_unit";
    private readonly string BufAnimationName = "mainmenu_footer_buf_button_loop";

    private MainMenuFooterButtonModel m_model = null;
    private bool m_IsBufEvent = false;
    public bool IsBufEvent { set { m_IsBufEvent = value; } }


    public static MainMenuFooterUnitsButton Attach(GameObject parent)
    {
        return ButtonView.Attach<MainMenuFooterUnitsButton>(PrefabPath, parent);
    }


    public MainMenuFooterUnitsButton SetModel(MainMenuFooterButtonModel model, bool bufEvent)
    {
        m_model = model;
        m_IsBufEvent = bufEvent;

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
        DefaultAnimationName = "mainmenu_footer_units_button_loop";
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
        if (!m_model.isEnabled)
        {
            imagePath += "_off";
            setBufEvent(false);
        }
        else if (m_model.isSelected)
        {
            imagePath += "_down";
            setBufEvent(false);
        }
        else
        {
            if (MainMenuManager.Instance != null
            && MainMenuManager.Instance.currentCategory == MAINMENU_CATEGORY.UNIT)
            {
                setBufEvent(false);
            }
            else
            {
                setBufEvent(true, true);
            }
        }

        m_buttonBaseImage.sprite = ResourceManager.Instance.Load(imagePath);
    }

    public void setBufEvent(bool isAnimation, bool startshift = false)
    {
        if (MainMenuManager.Instance == null ||
            MainMenuManager.Instance.Footer == null)
        {
            return;
        }

        MainMenuFooter footer = MainMenuManager.Instance.Footer;

        if (footer.isFooterAppeared == false)
        {
            return;
        }

        if (m_IsBufEvent)
        {
            footer.IsUnitBufEvent = isAnimation;

            if (isAnimation)
            {
                if (m_bufEventAnimation.isPlaying == false)
                {
                    float starttime = 0;

                    if (startshift)
                    {
                        starttime = footer.getHelpButtanAnimationTime();
                    }

                    m_bufEventAnimation[BufAnimationName].time = starttime;
                    m_bufEventAnimation.Play(BufAnimationName);
                }
            }
            else
            {
                m_bufEventAnimation.Stop();
            }
        }
        else
        {
            footer.IsUnitBufEvent = false;
            m_bufEventAnimation.Stop();
        }
    }

    public float getAnimationTime()
    {
        if (m_bufEventAnimation == null)
        {
            return 0;
        }

        if (m_bufEventAnimation.isPlaying == false)
        {
            return 0;
        }

        return m_bufEventAnimation[BufAnimationName].time;
    }
}