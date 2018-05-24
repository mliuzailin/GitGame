/**
 *  @file   HeroSelectButtonPanel.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/21
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;

public class HeroSelectButtonPanel : MenuPartsBase
{
    [SerializeField]
    private GameObject m_nextButtonRoot;
    [SerializeField]
    private GameObject m_returnButtonRoot;

    private static readonly string NextButtonPrefabPath = "Prefab/HeroSelect/ButtonPanel/HeroSelectPanelNextButton";
    private static readonly string ReturnButtonPrefabPath = "Prefab/HeroSelect/ButtonPanel/HeroSelectPanelReturnButton";


    private ButtonModel m_nextButton = null;
    private ButtonModel m_returnButton = null;


    /// <summary>次へを選択したときのアクション</summary>
    public Action NextAction = delegate { };
    /// <summary>戻るを選択したときのアクション</summary>
    public Action ReturnAction = delegate { };

    M4uProperty<bool> isActiveNext = new M4uProperty<bool>(true);
    /// <summary>次へボタンを表示するかどうか</summary>
    public bool IsActiveNext
    {
        get
        {
            return isActiveNext.Value;
        }
        set
        {
            isActiveNext.Value = value;
        }
    }

    M4uProperty<bool> isActiveReturn = new M4uProperty<bool>(true);
    /// <summary>戻るボタンを表示するかどうか</summary>
    public bool IsActiveReturn
    {
        get
        {
            return isActiveReturn.Value;
        }
        set
        {
            isActiveReturn.Value = value;
        }
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;

        SetUpButtons();
    }

    // Use this for initialization
    void Start()
    {
        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.addLocalYPos(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickNextButton()
    {
        NextAction();
    }

    public void OnClickReturnButton()
    {
        ReturnAction();
    }

    /// <summary>
    /// 下のアンカーのY座標の設定
    /// </summary>
    /// <param name="posY"></param>
    public void SetBottomPositionY(float posY)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, posY);
    }


    private void SetUpButtons()
    {
        m_nextButton = new ButtonModel();
        m_returnButton = new ButtonModel();

        ButtonView
            .Attach<ButtonView>(NextButtonPrefabPath, m_nextButtonRoot)
            .SetModel<ButtonModel>(m_nextButton);
        ButtonView
            .Attach<ButtonView>(ReturnButtonPrefabPath, m_returnButtonRoot)
            .SetModel<ButtonModel>(m_returnButton);


        m_nextButton.OnClicked += () =>
        {
            OnClickNextButton();
        };
        m_returnButton.OnClicked += () =>
        {
            OnClickReturnButton();
        };

        // TODO : 演出を入れるならその場所に移動
        m_nextButton.Appear();
        m_nextButton.SkipAppearing();
        m_returnButton.Appear();
        m_returnButton.SkipAppearing();
    }
}
