using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using System;
using DG.Tweening;
using TMPro;

public class GeneralWindowDialog : MenuPartsBase
{

    public enum ButtonEventType
    {
        NONE = -1,
        CLOSE = 0,
        YES,
        NO,
        MAX
    };

    private static readonly float FadeShowAlpha = 0.5f;
    private static readonly float FadeHideAlpha = 0.0f;
    private static readonly float AnimationTime = 0.25f;
    private static readonly float TextWidthMax = 540f;

    [SerializeField]
    MenuPartsBase m_Window = null;
    [SerializeField]
    GameObject m_ShadowPanel = null;
    [SerializeField]
    Canvas m_Canvas = null;
    [SerializeField]
    Image m_MessageImage = null;
    [SerializeField]
    GameObject[] m_CharaImageRoot = null;
    [SerializeField]
    GeneralWindowButtonView m_YesButton = null;
    [SerializeField]
    GeneralWindowButtonView m_NoButton = null;
    [SerializeField]
    GeneralWindowButtonView m_ReturnButton = null;
    [SerializeField]
    GeneralWindowButtonView m_NextButton = null;
    [SerializeField]
    GeneralWindowButtonView m_CloseButton = null;
    [SerializeField]
    TextMeshProUGUI m_MessageText = null;

    M4uProperty<bool> isViewTitle = new M4uProperty<bool>();
    public bool IsViewTitle { get { return isViewTitle.Value; } set { isViewTitle.Value = value; } }

    M4uProperty<string> messageText = new M4uProperty<string>();
    public string MessageText { get { return messageText.Value; } set { messageText.Value = value; } }

    M4uProperty<string> pageNum = new M4uProperty<string>();
    public string PageNum { get { return pageNum.Value; } set { pageNum.Value = value; } }

    M4uProperty<string> pageMax = new M4uProperty<string>();
    public string PageMax { get { return pageMax.Value; } set { pageMax.Value = value; } }

    M4uProperty<bool> isViewPageCount = new M4uProperty<bool>();
    public bool IsViewPageCount { get { return isViewPageCount.Value; } set { isViewPageCount.Value = value; } }

    public RectTransform m_WindowRect;

    private bool m_Ready = false;
    private bool m_Show = false;

    private Action m_HideAction = null;

    private MasterDataGeneralWindow[] m_GeneralWindowMasterArray = null;
    private MasterDataGeneralWindow m_CurrentGeneralWindowMaster = null;
    int m_CurrentLine = 0;

    private Action[] m_ButtunActionList = new System.Action[(int)ButtonEventType.MAX];

    GeneralWindowCharacterView m_Character = null;

    public static GeneralWindowDialog Create(Camera camera = null)
    {
        GameObject _tmpObj = Resources.Load("Prefab/GeneralWindowDialog/GeneralWindowDialog") as GameObject;
        if (_tmpObj == null)
        {
            return null;
        }

        GameObject _newObj = Instantiate(_tmpObj) as GameObject;
        if (_newObj == null)
        {
            return null;
        }
        UnityUtil.SetObjectEnabledOnce(_newObj, true);

        GeneralWindowDialog dialog = _newObj.GetComponent<GeneralWindowDialog>();

        if (camera != null && dialog != null)
        {
            dialog.SetCamera(camera);
        }
        return dialog;
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;

        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを登録
            AndroidBackKeyManager.Instance.StackPush(gameObject, OnSelectReturn);
        }

        GameObject charParent = null;
        if (m_CharaImageRoot.Length > 0)
        {
            charParent = m_CharaImageRoot[0];
        }
        m_Character = GeneralWindowCharacterView.Attach(charParent);

        for (int i = 0; i < (int)ButtonEventType.MAX; ++i)
        {
            m_ButtunActionList[i] = null;
        }

        SetUpButtons();

        m_WindowRect = m_Window.GetComponent<RectTransform>();
        m_Window.SetPosition(new Vector2(m_WindowRect.rect.width, m_WindowRect.anchoredPosition.y));
        m_Window.transform.localScale = new Vector3(0, 0, 0);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetUpButtons()
    {
        // YesButton
        var yesButtonModel = new ButtonModel();
        m_YesButton.SetModel(yesButtonModel);
        m_YesButton.SetDefaultText(GameTextUtil.GetText("common_button4"));
        yesButtonModel.OnClicked += () =>
        {
            OnClickYesButton();
        };
        yesButtonModel.Appear();
        yesButtonModel.SkipAppearing();

        // NoButton
        var noButtonModel = new ButtonModel();
        m_NoButton.SetModel(noButtonModel);
        m_NoButton.SetDefaultText(GameTextUtil.GetText("common_button5"));
        noButtonModel.OnClicked += () =>
        {
            OnClickNoButton();
        };
        noButtonModel.Appear();
        noButtonModel.SkipAppearing();

        // NextButton
        var nextButtonModel = new ButtonModel();
        m_NextButton.SetModel(nextButtonModel);
        m_NextButton.SetDefaultText("次へ");
        nextButtonModel.OnClicked += () =>
        {
            OnClickNextButton();
        };
        nextButtonModel.Appear();
        nextButtonModel.SkipAppearing();


        // ReturnButton
        var returnButtonModel = new ButtonModel();
        m_ReturnButton.SetModel(returnButtonModel);
        returnButtonModel.OnClicked += () =>
        {
            OnSelectReturn();
        };
        returnButtonModel.Appear();
        returnButtonModel.SkipAppearing();

        // CloseButton
        var closeButtonModel = new ButtonModel();
        m_CloseButton.SetModel(closeButtonModel);
        m_CloseButton.SetDefaultText(GameTextUtil.GetText("common_button1"));
        closeButtonModel.OnClicked += () =>
        {
            OnClose();
        };
        closeButtonModel.Appear();
        closeButtonModel.SkipAppearing();
    }

    public void SetCamera(Camera camera)
    {
        m_Canvas.worldCamera = camera;
    }

    public GeneralWindowDialog SetGroupID(uint group_id)
    {
        m_CurrentLine = 0;
        m_GeneralWindowMasterArray = MasterFinder<MasterDataGeneralWindow>.Instance.SelectWhere(" where group_id = ? ORDER BY fix_id ASC", group_id).ToArray();

        return this;
    }

    public GeneralWindowDialog SetIndex(int index)
    {
        if (m_GeneralWindowMasterArray == null || m_GeneralWindowMasterArray.IsRange(index) == false)
        {
            // エラーだった場合、閉じるボタンを表示しておく
            GeneralWindowButtonView[] buttons = GetComponentsInChildren<GeneralWindowButtonView>();
            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].IsView = (buttons[i] == m_CloseButton);
            }
            return this;
        }

        m_CurrentLine = index;
        m_CurrentGeneralWindowMaster = m_GeneralWindowMasterArray[index];

        int fix_id = (int)m_CurrentGeneralWindowMaster.fix_id;

        // タイトルの表示
        if (m_CurrentGeneralWindowMaster.title != string.Empty)
        {
            SetTitleText(m_CurrentGeneralWindowMaster.title);
        }

        // キャラクター表示
        m_Character.SetSprite(null);
        if (m_CurrentGeneralWindowMaster.char_img != string.Empty)
        {
            m_Character.SetUpData(m_CurrentGeneralWindowMaster);

            // 表示位置場所の変更
            if (m_CharaImageRoot.Length > (int)m_CurrentGeneralWindowMaster.char_type)
            {
                m_Character.SetParent<GeneralWindowCharacterView>(m_CharaImageRoot[(int)m_CurrentGeneralWindowMaster.char_type]);
            }
        }

        // 本文の表示
        MessageText = "";
        SetMessageSprite(null);
        if (m_CurrentGeneralWindowMaster.message_type == MasterDataDefineLabel.GeneralWindowMessageType.TEXT)
        {
            // テキスト
            MessageText = m_CurrentGeneralWindowMaster.message;
        }
        else if (m_CurrentGeneralWindowMaster.message_type == MasterDataDefineLabel.GeneralWindowMessageType.IMAGE)
        {
            // 画像
            MessageText = "";

            string assetBundleName = "general_window_" + m_CurrentGeneralWindowMaster.group_id.ToString();
            string assetName = m_CurrentGeneralWindowMaster.message;
            AssetBundler asset = AssetBundler.Create().Set(assetBundleName,
                                    (o) =>
                                    {
                                        SetMessageSprite(o.GetTexture2D(assetName, TextureWrapMode.Clamp),
                                                        o.GetTexture(assetName + "_mask", TextureWrapMode.Clamp),
                                                        fix_id);
                                    },
                                    (str) =>
                                    {

                                    })
                                    .Load();
        }

        // ページ数の表示
        if (m_CurrentGeneralWindowMaster.button_type == MasterDataDefineLabel.GeneralWindowButtonType.PAGE
            && m_GeneralWindowMasterArray.Length > 1)
        {
            IsViewPageCount = true;
            PageNum = (index + 1).ToString();
            PageMax = m_GeneralWindowMasterArray.Length.ToString();
        }
        else
        {
            IsViewPageCount = false;
        }

        // ボタンの表示
        if (m_CurrentGeneralWindowMaster.button_type == MasterDataDefineLabel.GeneralWindowButtonType.YES_NO)
        {
            m_YesButton.SetText(m_CurrentGeneralWindowMaster.button_01);
            m_NoButton.SetText(m_CurrentGeneralWindowMaster.button_02);

            m_YesButton.IsView = true;
            m_NoButton.IsView = true;

            m_NextButton.IsView = false;
            m_ReturnButton.IsView = false;
            m_CloseButton.IsView = false;
        }
        else
        {
            m_YesButton.IsView = false;
            m_NoButton.IsView = false;


            if (m_GeneralWindowMasterArray.Length <= 1)
            {
                // ページが1つ以下
                m_NextButton.IsView = false;
                m_ReturnButton.IsView = false;
                m_CloseButton.IsView = true;

                m_CloseButton.SetText(m_CurrentGeneralWindowMaster.button_01);
            }
            else if (m_CurrentLine == 0)
            {
                // ページが先頭
                m_NextButton.IsView = true;
                m_ReturnButton.IsView = true;
                m_CloseButton.IsView = false;

                m_NextButton.SetText(m_CurrentGeneralWindowMaster.button_02);
            }
            else if (m_CurrentLine == m_GeneralWindowMasterArray.Length - 1)
            {
                // ページが最後尾
                m_NextButton.IsView = false;
                m_ReturnButton.IsView = true;
                m_CloseButton.IsView = true;
            }
            else
            {
                m_NextButton.IsView = true;
                m_ReturnButton.IsView = true;
                m_CloseButton.IsView = false;

                m_NextButton.SetText(m_CurrentGeneralWindowMaster.button_02);
            }

            // 戻るボタンの画像変更
            if (m_CurrentLine == 0)
            {
                m_ReturnButton.SetSprite(ResourceManager.Instance.Load("gw_btn_close", ResourceType.Common));
            }
            else
            {
                m_ReturnButton.SetSprite(ResourceManager.Instance.Load("gw_btn_back", ResourceType.Common));
            }
        }

        return this;
    }

    void SetMessageSprite(Texture2D texture, Texture mask = null, int fix_id = -1)
    {
        if (m_MessageImage == null)
        {
            return;
        }

        if (fix_id >= 0 && fix_id != m_CurrentGeneralWindowMaster.fix_id)
        {
            return;
        }
        m_MessageImage.sprite = null;

        if (texture == null)
        {
            m_MessageImage.color = m_MessageImage.color.WithAlpha(0);
        }
        else
        {
            m_MessageImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            m_MessageImage.SetNativeSize();
            m_MessageImage.color = m_MessageImage.color.WithAlpha(1);
        }

        if (mask != null)
        {
            m_MessageImage.material = new Material(Resources.Load<Material>("Material/AlphaMaskMaterial"));
            m_MessageImage.material.SetTexture("_AlphaTex", mask);
        }
        else
        {
            m_MessageImage.material = null;
        }
    }

    void SetTitleText(string str)
    {
        if (m_MessageText == null)
        {
            return;
        }

        m_MessageText.text = str;
        RectTransform rect = m_MessageText.GetComponent<RectTransform>();

        float width = m_MessageText.preferredWidth;
        if (m_MessageText.preferredWidth >= TextWidthMax)
        {
            width = TextWidthMax;
        }

        rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);

        IsViewTitle = (str.IsNullOrEmpty() == false);
    }

    /// <summary>
    /// ボタンイベント設定
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="action"></param>
    public GeneralWindowDialog SetDialogEvent(ButtonEventType eventType, System.Action action)
    {
        if ((int)eventType >= (int)ButtonEventType.MAX)
        {
            return this;
        }
        if (eventType == ButtonEventType.NONE)
        {
            return this;
        }

        m_ButtunActionList[(int)eventType] = action;
        return this;
    }

    void SendDialogEvent(ButtonEventType eventType)
    {
        //アクション起動
        Action action = m_ButtunActionList[(int)eventType];
        if (action != null)
        {
            action();
        }
    }

    void OnSelectReturn()
    {
        if (m_YesButton.IsView == true)
        {
            return;
        }

        if (m_CurrentLine == 0)
        {
            OnClose();
        }
        else
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            SetIndex(m_CurrentLine - 1);
        }
    }

    void OnClickYesButton()
    {
        SendDialogEvent(ButtonEventType.YES);
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        Hide();
    }

    void OnClickNoButton()
    {
        SendDialogEvent(ButtonEventType.NO);
        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        Hide();
    }

    void OnClickNextButton()
    {
        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
        SetIndex(m_CurrentLine + 1);
    }

    public void OnClickShadowPanel()
    {
        if (m_NextButton.IsView == true)
        {
            OnClickNextButton();
        }
        else if (m_CloseButton.IsView == true)
        {
            OnClose();
        }

    }

    public void Show(Action hideAction = null)
    {
        if (m_Show)
        {
            return;
        }
        m_Show = true;

        m_HideAction = hideAction;

        m_Window.SetPositionAjustStatusBar(new Vector2(0, m_WindowRect.anchoredPosition.y));
        m_Window.transform.localScale = new Vector3(1, 0, 1);

        m_ShadowPanel.GetComponent<Image>().DOFade(FadeShowAlpha, AnimationTime);

        m_Window.transform.DOScaleY(1f, AnimationTime).OnComplete(() =>
        {
            SetIndex(0);
            m_Ready = true;
        });

        return;
    }

    public void Hide()
    {
        if (m_Ready == false)
        {
            return;
        }

        m_Show = false;

        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを解除
            AndroidBackKeyManager.Instance.StackPop(gameObject);
        }

        m_ShadowPanel.GetComponent<Image>().DOFade(FadeHideAlpha, AnimationTime);

        m_Window.transform.DOScaleY(0f, AnimationTime).OnComplete(() =>
        {
            if (m_HideAction != null)
            {
                m_HideAction();
            }
            DestroyObject(gameObject);
        });
    }


    public void OnClose()
    {
        if (m_Ready == false || m_Show == false)
        {
            return;
        }

        SendDialogEvent(ButtonEventType.CLOSE);
        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        Hide();
    }
}
