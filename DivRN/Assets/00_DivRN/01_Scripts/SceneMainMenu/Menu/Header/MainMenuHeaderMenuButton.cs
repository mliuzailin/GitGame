using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;

public class MainMenuHeaderMenuButton : ButtonView
{
    [SerializeField]
    private Image m_buttonImage = null;
    [SerializeField]
    private Button m_button = null;

    public static readonly string PrefabPath = "Prefab/MainMenu/Header/MainMenuHeaderMenuButton";



    public static MainMenuHeaderMenuButton Attach(GameObject parent)
    {
        return ButtonView.Attach<MainMenuHeaderMenuButton>(PrefabPath, parent);
    }


    private ButtonModel m_model = null;


    public MainMenuHeaderMenuButton SetModel(ButtonModel model)
    {
        m_model = model;

        m_model.OnUpdated += () =>
        {
            m_button.interactable = m_model.isEnabled;
            SetTextures();
        };

        base.SetModel<ButtonModel>(m_model);

        return this;
    }

    void Awake()
    {
    }

    void Start()
    {
        SetTextures();
        Show();
        if (m_model != null) m_model.SkipAppearing();//TODO Developer　クエストからもどったときに有効化されないバグの対処
    }


    private void SetTextures()
    {

    }

    new public void Click()
    {
        if (MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch())
            return;

        base.Click();
    }
}
