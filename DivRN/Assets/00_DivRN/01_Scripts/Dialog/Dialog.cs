using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using DG.Tweening;
using ServerDataDefine;
using TMPro;

/// <summary>
/// ダイアログタイプ
/// </summary>
public enum DialogType : int
{
    NONE = -1,
    DialogOK = 0,               //< OKダイアログ
    DialogYesNo,                //< YesNoダイアログ
    DialogMenu,                 //< Menuダイアログ
    DialogUnit,					//< Unitダイアログ
    DialogSort,                 //< ソートダイアログ
    DialogShopSelect,           //<
    DialogShopAgeVerification,  //<
    DialogScroll,				//< スクロールダイアログ
    DialogFriend,               //< フレンドダイアログ
    DialogScrollInfo,           //< スクロール情報ダイアログ
    DialogTransferPassword,     //< 機種移行ダイアログ
    DialogSelectQuality,        //< ダイアログ
    DialogScrollMenu,           //< スクロールMenuダイアログ
    DialogScrollMenu_YESNO,		//< スクロールMenuダイアログ
    DialogIconList,             //< アイコンリストダイアログ
    DialogMissionList,          //< ミッションリストダイアログ
    MAX
};

/// <summary>
/// ダイアログテキストタイプ
/// </summary>
public enum DialogTextType : int
{
    Title = 0,              //< タイトル
    SubTitle,               //< サブタイトル
    MainText,               //< メインテキスト
    OKText,                 //< OKボタンテキスト
    YesText,                //< Yesボタンテキスト
    NoText,                 //< Noボタンテキスト
    CancelText,             //< Cancelボタンテキスト
    UnderText,              //< アンダーテキスト
    InButtonText,           //<
    QualityHighText,        //<
    MAX
};
/// <summary>
/// ダイアログイベントタイプ
/// </summary>
public enum DialogButtonEventType : int
{
    NONE = -1,              //< 無効イベント
    OK = 0,                 //< OKボタンイベント
    YES,                    //< YESボタンイベント
    NO,                     //< NOボタンイベント
    INBUTTON,               //< メニュー内ボタンイベント
    QUALITYHIGH,            //<
    CANCEL,                 //< CANCELボタンイベント
    MULTI01,                //< マルチボタン(1)イベント
    MULTI02,                //< マルチボタン(2)イベント
    MULTI03,                //< マルチボタン(3)イベント
    MULTI04,                //< マルチボタン(4)イベント
    MULTI05,                //< マルチボタン(5)イベント
    MULTI06,                //< マルチボタン(6)イベント
    MAX
};

/// <summary>
/// ダイアログタイトルタイプ
/// </summary>
public enum DialogTitleType : int
{
    NONE = -1,
    SINGLE = 0,
    DOUBLE,
    FRIEND,
};

/// <summary>
/// ダイアログメインタイプ
/// </summary>
public enum DialogMainType : int
{
    NONE = -1,
    TEXT = 0,
    MENU,
    UNIT,
    SORT,
    SHOPSELECT,
    SHOPAGE,
    SCROLL,
    FRIEND,
    SCROLL_INFO,
    TRANSFER_PASSWORD,
    MENUINBUTTON,
    SELECTQUALITY,
    SCROLL_MENU,
    ICON_LIST,
    MISSION_LIST,
};

/// <summary>
/// ダイアログボタンタイプ
/// </summary>
public enum DialogButtonType : int
{
    NONE = -1,
    OK,
    YESNO,
};

/// <summary>
/// ダイアログオブジェクトタイプ
/// </summary>
public enum DialogObjectType : int
{
    NONE = -1,
    Title = 0,
    DoubleTitle = 1,
    MainText = 2,
    Menu = 3,
    OneButton = 4,
    TwoButton = 5,
    Cancel = 6,
    UnitInfo = 7,
    Sort = 8,
    ShopSelect = 9,
    ShopAgeVerification = 10,
    UnderText = 11,
    ScrollText = 12,
    FriendInfo = 13,
    ScrollInfo = 14,
    TransferPassword = 15,
    MenuInButton = 16,
    SelectQuality = 17,
    ScrollMenu = 18,
    UnserButtonList = 19,
    FriendTitle = 20,
    IconList = 21,
    VerticalButtonList = 22,
    MissionList = 23,

    MAX
};

/// <summary>
/// ダイアログユニット情報タイプ
/// </summary>
public enum DialogUnitInfoType : int
{
    NONE = -1,
    PLAYER,
    FRIEND,
    HELPER,
    MAX
}

/// <summary>
/// ダイアログクラス
/// </summary>
public class Dialog : M4uContextMonoBehaviour
{
    public GameObject dialogBG = null;
    public GameObject fadePanel = null;
    public GameObject fadePanel2 = null;
    public GameObject scrollInfoContent = null;

    public GameObject[] objectArray;

    public Button cancel_button = null;

    [SerializeField]
    private GameObject _ok_button_root = null;
    [SerializeField]
    private GameObject _yes_button_root = null;
    [SerializeField]
    private GameObject _no_button_root = null;
    [SerializeField]
    private GameObject _cancel_button_root = null;
    [SerializeField]
    private GameObject _inmenu_button_root = null;
    [SerializeField]
    private GameObject _quality_high_button_root = null;

    [SerializeField]
    private Button _fade_panel_touch_reciever = null;

    public static readonly string OkButtonPrefabPath = "Prefab/Dialog/Parts/DialogOkButton";
    public static readonly string YesButtonPrefabPath = "Prefab/Dialog/Parts/DialogYesButton";
    public static readonly string NoButtonPrefabPath = "Prefab/Dialog/Parts/DialogNoButton";
    public static readonly string CancelButtonPrefabPath = "Prefab/Dialog/Parts/DialogCancelButton";
    public static readonly string InmenuButtonPrefabPath = "Prefab/Dialog/Parts/DialogInmenuButton";
    public static readonly string QualitySwitchButtonPrefabPath = "Prefab/Dialog/Parts/DialogQualityButton";
    public static readonly string TransitionButtonPrefabPath = "Prefab/Dialog/Parts/DialogTransitionButton";
    public static readonly string CloseButtonPrefabPath = "Prefab/Dialog/Parts/DialogCloseButton";

    public static readonly string CONFIRM_BUTTON_TITLE = "確認";

    private Dictionary<int, DialogButtonModel> m_buttons = new Dictionary<int, DialogButtonModel>();


    public VerticalLayoutGroup buttonList = null;

    //private int dialogID = -1;
    private DialogType dialogType = DialogType.NONE;
    private bool autoHide = true;
    private bool cancel = true;
    private bool backkey = true;
    private bool show = false;
    private bool hide = false;
    private System.Action[] buttunActionList = new System.Action[(int)DialogButtonEventType.MAX];
    private CanvasSetting canvasSetting = null;
    private bool fadePanelFlag = false;
    private int updateLayoutCount = 0;
    private bool isUseCloseButton = false;

    private DialogButtonEventType pushButton = DialogButtonEventType.NONE;
    public DialogButtonEventType PushButton { get { return pushButton; } }

    private DialogUnitInfo unitInfo = null;

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<string> subtitle = new M4uProperty<string>();
    public string Subtitle { get { return subtitle.Value; } set { subtitle.Value = value; } }

    M4uProperty<string> main_text = new M4uProperty<string>();
    public string Main_text { get { return main_text.Value; } set { main_text.Value = value; } }

    M4uProperty<bool> isActiveTitleBorder = new M4uProperty<bool>();
    public bool IsActiveTitleBorder { get { return isActiveTitleBorder.Value; } set { isActiveTitleBorder.Value = value; } }

    M4uProperty<string> under_text = new M4uProperty<string>();
    public string Under_text { get { return under_text.Value; } set { under_text.Value = value; } }

    M4uProperty<List<DialogMenuItem>> menu_list = new M4uProperty<List<DialogMenuItem>>();
    public List<DialogMenuItem> Menu_list { get { return menu_list.Value; } set { menu_list.Value = value; } }

    M4uProperty<List<DialogTipItem>> tip_list = new M4uProperty<List<DialogTipItem>>();
    public List<DialogTipItem> Tip_list { get { return tip_list.Value; } set { tip_list.Value = value; } }

    M4uProperty<List<DialogAgeItem>> age_list = new M4uProperty<List<DialogAgeItem>>();
    public List<DialogAgeItem> Age_list { get { return age_list.Value; } set { age_list.Value = value; } }

    M4uProperty<List<DialogSortItem>> sort_list = new M4uProperty<List<DialogSortItem>>(new List<DialogSortItem>());
    public List<DialogSortItem> Sort_list { get { return sort_list.Value; } set { sort_list.Value = value; } }

    M4uProperty<string> order_id_text = new M4uProperty<string>();
    public string Order_id_text { get { return order_id_text.Value; } set { order_id_text.Value = value; } }

    M4uProperty<string> password_text = new M4uProperty<string>();
    public string Password_text { get { return password_text.Value; } set { password_text.Value = value; } }

    M4uProperty<string> transfer_detail_text = new M4uProperty<string>();
    public string Transfer_detail_text { get { return transfer_detail_text.Value; } set { transfer_detail_text.Value = value; } }

    M4uProperty<string> in_button_text = new M4uProperty<string>();
    public string In_button_text { get { return in_button_text.Value; } set { in_button_text.Value = value; } }

    M4uProperty<string> normal_text = new M4uProperty<string>();
    public string Normal_text { get { return normal_text.Value; } set { normal_text.Value = value; } }

    M4uProperty<string> high_text = new M4uProperty<string>();
    public string High_text { get { return high_text.Value; } set { high_text.Value = value; } }

    M4uProperty<bool> isActiveScrollText = new M4uProperty<bool>(false);
    public bool IsActiveScrollText { get { return isActiveScrollText.Value; } set { isActiveScrollText.Value = value; } }

    M4uProperty<List<QuestMissionContext>> missionList = new M4uProperty<List<QuestMissionContext>>(new List<QuestMissionContext>());
    public List<QuestMissionContext> MissionList { get { return missionList.Value; } set { missionList.Value = value; } }

    private bool m_isStrongYes = false;
    public Dialog SetStrongYes()
    {
        m_isStrongYes = true;
        return this;
    }

    M4uProperty<List<DialogUnderButtonContext>> under_button_list = new M4uProperty<List<DialogUnderButtonContext>>(new List<DialogUnderButtonContext>());
    public List<DialogUnderButtonContext> Under_button_list { get { return under_button_list.Value; } set { under_button_list.Value = value; } }

    M4uProperty<List<DialogIconItem>> iconList = new M4uProperty<List<DialogIconItem>>(new List<DialogIconItem>());
    public List<DialogIconItem> IconList { get { return iconList.Value; } set { iconList.Value = value; } }

    /// <summary>
    ///
    /// </summary>
    void Awake()
    {
        gameObject.GetComponentInChildren<Canvas>().enabled = false;

        gameObject.GetComponent<M4uContextRoot>().Context = this;
        Title = "タイトル";
        Main_text = "サンプルテキスト";
        Under_text = "サンプル";
        Order_id_text = GameTextUtil.GetText("mt16q_content_1");
        Password_text = GameTextUtil.GetText("mt16q_content_2");
        Transfer_detail_text = GameTextUtil.GetText("mt16q_content_3");
        Normal_text = GameTextUtil.GetText("mt15q_content_2");
        High_text = GameTextUtil.GetText("mt15q_content_3");

        GameObject transObj = getObject(DialogObjectType.TransferPassword);
        TMP_InputField[] fields = transObj.GetComponentsInChildren<TMP_InputField>();
        if (fields != null &&
            fields.Length > 0)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i].enabled = false;
            }
        }

        for (int i = 0; i < (int)DialogButtonEventType.MAX; i++)
        {
            buttunActionList[i] = null;
        }

        canvasSetting = gameObject.GetComponentInChildren<CanvasSetting>();

        setButtonAction(_fade_panel_touch_reciever, DialogButtonEventType.CANCEL);

        SetUpButtons();


        dialogBG.transform.localScale = new Vector3(1, 0, 1);
        Color tmpCol = fadePanel.GetComponent<Image>().color;
        fadePanel.GetComponent<Image>().color = new Color(tmpCol.r, tmpCol.g, tmpCol.b, 0f);

        //バックキーが押された時のアクションを登録
        AndroidBackKeyManager.Instance.StackPush(gameObject, OnBackKeyAction);
    }


    /// <summary>
    ///
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    ///
    /// </summary>
    void Update()
    {
        if (updateLayoutCount != 0)
        {
            updateLayoutCount--;
            if (updateLayoutCount < 0)
            {
                updateLayoutCount = 0;
            }
            updateLayout();
        }
    }

    private void SetUpButtons()
    {
        var buttonTypePrefabMap = new Dictionary<int, string>
        {
            { (int)DialogButtonEventType.OK,             OkButtonPrefabPath},
            { (int)DialogButtonEventType.YES,            YesButtonPrefabPath},
            { (int)DialogButtonEventType.NO,             NoButtonPrefabPath},
            { (int)DialogButtonEventType.CANCEL,         CancelButtonPrefabPath},
            { (int)DialogButtonEventType.INBUTTON,       InmenuButtonPrefabPath},
            { (int)DialogButtonEventType.QUALITYHIGH,    QualitySwitchButtonPrefabPath},
        };

        var closeButtonTypePrefabMap = new Dictionary<int, string>
        {
            { (int)DialogButtonEventType.OK,             CloseButtonPrefabPath},
        };

        var buttonTypeRootMap = new Dictionary<int, GameObject>
        {
            { (int)DialogButtonEventType.OK,             _ok_button_root},
            { (int)DialogButtonEventType.YES,            _yes_button_root},
            { (int)DialogButtonEventType.NO,             _no_button_root},
            { (int)DialogButtonEventType.CANCEL,         _cancel_button_root},
            { (int)DialogButtonEventType.INBUTTON,       _inmenu_button_root},
            { (int)DialogButtonEventType.QUALITYHIGH,    _quality_high_button_root},
        };
        var buttonTypeSeMap = new Dictionary<int, SEID>
        {
            { (int)DialogButtonEventType.OK,             SEID.SE_MENU_OK},
            { (int)DialogButtonEventType.YES,            SEID.SE_MENU_OK},
            { (int)DialogButtonEventType.NO,             SEID.SE_MENU_RET},
            { (int)DialogButtonEventType.CANCEL,         SEID.SE_MENU_RET},
            { (int)DialogButtonEventType.INBUTTON,       SEID.SE_MENU_OK},
            { (int)DialogButtonEventType.QUALITYHIGH,    SEID.SE_MENU_OK}
        };

        isUseCloseButton = false;

        for (int i = 0; i < buttonTypePrefabMap.Count - 1; i++)
        {
            int index = i;
            var type = (DialogButtonEventType)index;
            var buttonModel = new DialogButtonModel();
            var view = DialogButtonView.
                Attach<DialogButtonView>(buttonTypePrefabMap[(int)type], buttonTypeRootMap[(int)type]);
            view.SetModel<ButtonModel>(buttonModel);

            buttonModel.OnClicked += () =>
            {
                OnPushButton(type,
                    IsCloseOrReturnButton(buttonModel.text)
                    ? SEID.SE_MENU_RET
                    : buttonTypeSeMap[(int)type]);
            };
            buttonModel.OnUpdated += () =>
            {
                if (IsCloseButton(buttonModel.text) == true
                && type == DialogButtonEventType.OK)
                {
                    if (isUseCloseButton == false)
                    {
                        view.Detach();
                        view = DialogButtonView.
                               Attach<DialogButtonView>(closeButtonTypePrefabMap[(int)type], buttonTypeRootMap[(int)type]);
                        view.SetModel<ButtonModel>(buttonModel);
                        isUseCloseButton = true;
                    }
                }
                else if (isUseCloseButton == true)
                {
                    view.Detach();
                    view = DialogButtonView.
                           Attach<DialogButtonView>(buttonTypePrefabMap[(int)type], buttonTypeRootMap[(int)type]);
                    view.SetModel<ButtonModel>(buttonModel);
                    isUseCloseButton = false;
                }
                view.SetText(buttonModel.text);
                view.SetSwitch(buttonModel._switch);
            };

            m_buttons[index] = buttonModel;

            // TODO : 演出あれば入れる
            buttonModel.Appear();
            buttonModel.SkipAppearing();
        }
    }

    public Dialog SettingDialog(DialogType _type)
    {
        dialogType = _type;
        //
        EnableFadePanel();
        //すべてOFF
        setAllObjectDisable();

        switch (dialogType)
        {
            case DialogType.DialogOK:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.TEXT);
                SettingDialogButton(DialogButtonType.OK);
                break;
            case DialogType.DialogYesNo:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.TEXT);
                SettingDialogButton(DialogButtonType.YESNO);
                break;
            case DialogType.DialogMenu:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.MENU);
                SettingDialogButton(DialogButtonType.NONE);
                break;
            case DialogType.DialogUnit:
                SettingDialogTitle(DialogTitleType.FRIEND);
                SettingDialogMain(DialogMainType.UNIT);
                SettingDialogButton(DialogButtonType.YESNO);
                break;
            case DialogType.DialogSort:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.SORT);
                SettingDialogButton(DialogButtonType.NONE);
                break;
            case DialogType.DialogShopSelect:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.SHOPSELECT);
                SettingDialogButton(DialogButtonType.NONE);
                break;
            case DialogType.DialogShopAgeVerification:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.SHOPAGE);
                SettingDialogButton(DialogButtonType.NONE);
                break;
            case DialogType.DialogScroll:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.SCROLL);
                SettingDialogButton(DialogButtonType.NONE);
                break;
            case DialogType.DialogFriend:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.FRIEND);
                SettingDialogButton(DialogButtonType.YESNO);
                break;
            case DialogType.DialogScrollInfo:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.SCROLL_INFO);
                SettingDialogButton(DialogButtonType.NONE);
                break;
            case DialogType.DialogTransferPassword:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.TRANSFER_PASSWORD);
                SettingDialogButton(DialogButtonType.YESNO);
                break;
            case DialogType.DialogSelectQuality:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.SELECTQUALITY);
                SettingDialogButton(DialogButtonType.OK);
                break;
            case DialogType.DialogScrollMenu:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.SCROLL_MENU);
                SettingDialogButton(DialogButtonType.OK);
                break;
            case DialogType.DialogScrollMenu_YESNO:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.SCROLL_MENU);
                SettingDialogButton(DialogButtonType.YESNO);
                break;
            case DialogType.DialogIconList:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.ICON_LIST);
                SettingDialogButton(DialogButtonType.YESNO);
                break;
            case DialogType.DialogMissionList:
                SettingDialogTitle(DialogTitleType.SINGLE);
                SettingDialogMain(DialogMainType.MISSION_LIST);
                SettingDialogButton(DialogButtonType.OK);
                break;
        }
        return this;
    }

    public void SetDialogObjectEnabled(DialogObjectType _type, bool _flag)
    {
        UnityUtil.SetObjectEnabledOnce(getObject(_type), _flag);
    }

    private void SettingDialogTitle(DialogTitleType _titleType)
    {
        switch (_titleType)
        {
            case DialogTitleType.NONE:
                IsActiveTitleBorder = false;
                break;
            case DialogTitleType.SINGLE:
                IsActiveTitleBorder = true;
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.Title), true);
                break;
            case DialogTitleType.DOUBLE:
                IsActiveTitleBorder = true;
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.DoubleTitle), true);
                break;
            case DialogTitleType.FRIEND:
                IsActiveTitleBorder = true;
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.FriendTitle), true);
                break;
        }
    }

    private void SettingDialogMain(DialogMainType _mainType)
    {
        switch (_mainType)
        {
            case DialogMainType.NONE:
                break;
            case DialogMainType.TEXT:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.MainText), true);
                break;
            case DialogMainType.MENU:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.Menu), true);
                break;
            case DialogMainType.UNIT:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.UnitInfo), true);
                unitInfo = getObject(DialogObjectType.UnitInfo).GetComponent<DialogUnitInfo>();
                break;
            case DialogMainType.SORT:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.Sort), true);
                break;
            case DialogMainType.SHOPSELECT:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.ShopSelect), true);
                break;
            case DialogMainType.SHOPAGE:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.MainText), true);
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.ShopAgeVerification), true);
                break;
            case DialogMainType.SCROLL:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.ScrollText), true);
                break;
            case DialogMainType.FRIEND:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.FriendInfo), true);
                break;
            case DialogMainType.SCROLL_INFO:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.ScrollInfo), true);
                break;
            case DialogMainType.TRANSFER_PASSWORD:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.TransferPassword), true);
                break;
            case DialogMainType.SELECTQUALITY:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.MainText), true);
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.SelectQuality), true);
                break;
            case DialogMainType.SCROLL_MENU:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.ScrollMenu), true);
                break;
            case DialogMainType.ICON_LIST:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.IconList), true);
                break;
            case DialogMainType.MISSION_LIST:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.MissionList), true);
                break;

        }
    }

    private void SettingDialogButton(DialogButtonType _buttonType)
    {
        DisableCancelButton();
        switch (_buttonType)
        {
            case DialogButtonType.NONE:
                SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.OneButton), true);
                break;
            case DialogButtonType.OK:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.OneButton), true);
                break;
            case DialogButtonType.YESNO:
                UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.TwoButton), true);
                break;
        }
    }

    /// <summary>
    /// ダイアログテキスト設定
    /// </summary>
    /// <param name="textType"></param>
    /// <param name="text"></param>
    public Dialog SetDialogText(DialogTextType textType, string text)
    {
        string setText = checkLimitString(text);

        switch (textType)
        {
            case DialogTextType.Title:
                Title = setText;
                break;
            case DialogTextType.SubTitle:
                Subtitle = setText;
                break;
            case DialogTextType.MainText:
                Main_text = setText;
                break;
            case DialogTextType.OKText:
                m_buttons[(int)DialogButtonEventType.OK].text = setText;
                break;
            case DialogTextType.YesText:
                m_buttons[(int)DialogButtonEventType.YES].text = setText;
                break;
            case DialogTextType.NoText:
                m_buttons[(int)DialogButtonEventType.NO].text = setText;
                break;
            case DialogTextType.CancelText:
                m_buttons[(int)DialogButtonEventType.CANCEL].text = setText;
                break;
            case DialogTextType.UnderText:
                Under_text = setText;
                break;
            case DialogTextType.InButtonText:
                m_buttons[(int)DialogButtonEventType.INBUTTON].text = setText;
                break;
            default:
                break;
        }
        return this;
    }


    public Dialog SetMainFromTextKey(string textKey)
    {
        SetDialogText(DialogTextType.MainText, GameTextUtil.GetText(textKey));
        return this;
    }

    public Dialog SetTitleFromTextKey(string textKey)
    {
        SetDialogText(DialogTextType.Title, GameTextUtil.GetText(textKey));
        return this;
    }

    /// <summary>
    /// ダイアログテキスト設定
    /// </summary>
    /// <param name="textType"></param>
    /// <param name="textKey"></param>
    public Dialog SetDialogTextFromTextkey(DialogTextType textType, string textKey)
    {
        SetDialogText(textType, GameTextUtil.GetText(textKey));
        return this;
    }

    /// <summary>
    /// テキストアラインメント設定
    /// </summary>
    /// <param name="textType"></param>
    /// <param name="align"></param>
    public void SetTextAlignment(DialogTextType textType, TextAlignmentOptions align)
    {
        GameObject _textObj = null;
        switch (textType)
        {
            case DialogTextType.MainText:
                {
                    if (dialogType == DialogType.DialogScroll)
                    {
                        _textObj = UnityUtil.GetChildNode(getObject(DialogObjectType.ScrollText), textType.ToString());
                    }
                    else
                    {
                        _textObj = UnityUtil.GetChildNode(gameObject, textType.ToString());
                    }
                }
                break;
            default:
                _textObj = UnityUtil.GetChildNode(gameObject, textType.ToString());
                break;
        }

        if (_textObj == null)
        {
            return;
        }

        TextMeshProUGUI tmpText = _textObj.GetComponent<TextMeshProUGUI>();
        tmpText.alignment = align;

        if (dialogType == DialogType.DialogScroll) { return; }

        RectTransform trans = _textObj.GetComponent<RectTransform>();

        switch (align)
        {
            case TextAlignmentOptions.Bottom:
                trans.pivot = new Vector2(0.5f, 0);
                break;
            case TextAlignmentOptions.BottomLeft:
                trans.pivot = new Vector2(0, 0);
                break;
            case TextAlignmentOptions.BottomRight:
                trans.pivot = new Vector2(1, 0);
                break;
            case TextAlignmentOptions.Center:
                trans.pivot = new Vector2(0.5f, 0.5f);
                break;
            case TextAlignmentOptions.MidlineLeft:
                trans.pivot = new Vector2(0, 0.5f);
                break;
            case TextAlignmentOptions.MidlineRight:
                trans.pivot = new Vector2(1, 0.5f);
                break;
            case TextAlignmentOptions.Top:
                trans.pivot = new Vector2(0.5f, 1);
                break;
            case TextAlignmentOptions.TopLeft:
                trans.pivot = new Vector2(0, 1);
                break;
            case TextAlignmentOptions.TopRight:
                trans.pivot = new Vector2(1, 1);
                break;
        }

    }

    public void SetButtonAlignment(TextAnchor align)
    {
        if (buttonList == null)
        {
            return;
        }
        buttonList.childAlignment = align;
    }

    public Dialog SetOkEvent(System.Action action, bool autoHide = true)
    {
        SetDialogEvent(DialogButtonEventType.OK, () =>
        {
            action();

            if (autoHide)
            {
                Hide();
            }

        });
        return this;
    }

    public Dialog SetYesEvent(System.Action action, bool autoHide = true)
    {
        SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            action();

            if (autoHide)
            {
                Hide();
            }

        });
        return this;
    }

    public Dialog SetNoEvent(System.Action action = null, bool autoHide = true)
    {
        SetDialogEvent(DialogButtonEventType.NO, () =>
        {
            if (action != null)
            {
                action();
            }

            if (autoHide)
            {
                Hide();
            }
        });
        return this;
    }

    /// <summary>
    /// チップ購入
    /// </summary>
    public Dialog SetTipEvent(List<DialogTipItem> tiplist,
                              System.Action<DialogTipItem> action = null,
                              bool autoHide = true)
    {
        for (int i = 0; i < tiplist.Count; i++)
        {
            System.Action<DialogTipItem> selectAction = (DialogTipItem item) =>
            {
                SoundUtil.PlaySE(SEID.SE_MENU_OK);

                if (action != null)
                {
                    action(item);
                }

                if (autoHide)
                {
                    Hide();
                }
            };
            tiplist[i].DidSelectItem += selectAction;
        }

        Tip_list = tiplist;

        updateLayoutCount = 5;

        return this;
    }

    /// <summary>
    /// 年齢確認リスト設定
    /// </summary>
    public Dialog SetAgeEvent(List<DialogAgeItem> agelist,
                              System.Action<DialogButtonEventType> action = null,
                              bool autoHide = true)
    {
        for (int i = 0; i < agelist.Count; i++)
        {
            System.Action<DialogButtonEventType> selectAction = (DialogButtonEventType type) =>
            {
                SoundUtil.PlaySE(SEID.SE_MENU_OK);
                if (action != null)
                {
                    action(type);
                }

                if (autoHide)
                {
                    Hide();
                }
            };
            agelist[i].DidSelectItem += selectAction;
        }

        Age_list = agelist;

        return this;
    }

    /// <summary>
    /// 水平の端に到達したときのテキストのラッピングモード設定
    /// </summary>
    /// <param name="textType"></param>
    /// <param name="mode"></param>
    public void SetHorizontalOverflow(DialogTextType textType, HorizontalWrapMode mode)
    {
        GameObject _textObj = UnityUtil.GetChildNode(gameObject, textType.ToString());
        if (_textObj == null)
        {
            return;
        }

        Text tmpText = _textObj.GetComponent<Text>();
        tmpText.horizontalOverflow = mode;
    }

    /// <summary>
    /// ボタンイベント設定
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="action"></param>
    public Dialog SetDialogEvent(DialogButtonEventType eventType, System.Action action)
    {
        if ((int)eventType >= (int)DialogButtonEventType.MAX)
        {
            return this;
        }
        if (eventType == DialogButtonEventType.NONE)
        {
            return this;
        }

        buttunActionList[(int)eventType] = action;
        return this;
    }
    /// <summary>
    /// メニューリスト設定
    /// </summary>
    /// <param name="list"></param>
    public void SetMeneList(List<DialogMenuItem> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].DidSelectAction += OnAction;
        }
        Menu_list = list;
    }

    public void SetSortListCurrentSortType(MAINMENU_SORT_SEQ[] sortTypeList, MAINMENU_SORT_SEQ selectSortTypeNow, System.Action<MAINMENU_SORT_SEQ> action)
    {
        for (int i = 0; i < sortTypeList.Length; i++)
        {
            DialogSortItem item = new DialogSortItem();
            item.DetailText = GameTextUtil.GetText("BTN_FORMAT_ZENKAKU") + GameTextUtil.GetText(GameTextUtil.GetSortToTextKey(sortTypeList[i]));
            item.SortType = sortTypeList[i];
            item.ButtonColorBlock = (sortTypeList[i] == selectSortTypeNow) ? ColorBlockUtil.BUTTON_PURPLE : ColorBlockUtil.BUTTON_WHITE;
            item.DelSelectDialogSort += action;
            Sort_list.Add(item);
        }
    }

    public Dialog SetMissionList(List<QuestMissionContext> mission_list)
    {
        MissionList.Clear();
        MissionList = mission_list;

        return this;
    }

    /// <summary>
    /// キャンセルボタン無効
    /// </summary>
    public void DisableCancelButton()
    {
        if (getObject(DialogObjectType.Cancel) != null)
        {
            UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.Cancel), false);
        }
        cancel = false;
    }

    /// <summary>
    /// キャンセルボタン無効
    /// </summary>
    public void EnableCancel()
    {
        cancel = true;
    }

    /// <summary>
    /// 自動消去無効
    /// </summary>
    public void DisableAutoHide()
    {
        autoHide = false;
    }

    /// <summary>
    /// フェードパネル有効
    /// </summary>
    public void EnableFadePanel()
    {
        fadePanelFlag = true;
    }

    /// <summary>
    /// フェードパネル無効
    /// </summary>
    public void DisableFadePanel()
    {
        fadePanelFlag = false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="_type"></param>
    public void OnPushButton(DialogButtonEventType _type, SEID seId = SEID.SE_NONE)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("Push:" + _type.ToString());
#endif
        //キャンセル無効
        if (!cancel && _type == DialogButtonEventType.CANCEL)
        {
            return;
        }


        if (seId == SEID.SE_NONE)
        {
            if (_type == DialogButtonEventType.NO ||
                _type == DialogButtonEventType.CANCEL ||
                IsCloseOrReturnButton(m_buttons[(int)_type].text))
            {
                seId = SEID.SE_MENU_RET;
            }
            else
            {
                seId = SEID.SE_MENU_OK2;
            }
        }

        if (seId == SEID.SE_MENU_OK
            && _type != DialogButtonEventType.INBUTTON
            && m_isStrongYes)
        {
            seId = SEID.SE_MENU_OK2;
        }

        if (hide == false)
        {
            SoundUtil.PlaySE(seId);
        }

        //アクション起動
        System.Action action = buttunActionList[(int)_type];
        if (action != null)
        {
            action();
        }

        pushButton = _type;

        //自動消去
        if (autoHide)
        {
            Hide();
        }
    }

    private bool IsCloseOrReturnButton(string _text)
    {
        if (_text == GameTextUtil.GetText("common_button1") ||
            _text == GameTextUtil.GetText("common_button6"))
        {
            return true;
        }
        return false;
    }

    private bool IsCloseButton(string _text)
    {
        if (_text == GameTextUtil.GetText("common_button1"))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// バックキーアクション(Android用)
    /// </summary>
    public void OnBackKeyAction()
    {
        if (!backkey)
        {
            return;
        }

        if (checkObject(DialogObjectType.Cancel) && cancel)
        {
            //キャンセルありはキャンセル
            OnPushButton(DialogButtonEventType.CANCEL);
        }
        else if (checkObject(DialogObjectType.OneButton))
        {
            //OKのときはOKボタン
            OnPushButton(DialogButtonEventType.OK);
        }
        else if (checkObject(DialogObjectType.TwoButton))
        {
            //YesNoのときはNOボタン
            OnPushButton(DialogButtonEventType.NO);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public void OnAction()
    {
        //自動消去
        if (autoHide)
        {
            Hide();
        }
    }

    /// <summary>
    /// ダイアログ表示開始
    /// </summary>
    public void Show()
    {
        if (show)
        {
            return;
        }

        show = true;

        gameObject.GetComponentInChildren<Canvas>().enabled = true;

        if (fadePanelFlag)
        {
            fadePanel.GetComponent<Image>().DOFade(0.70f, 0.25f);
        }
        else
        {
            fadePanel2.GetComponent<Image>().DOFade(0.70f, 0.25f);
        }

        dialogBG.transform.DOScaleY(1, 0.25f).OnComplete(() =>
        {
            IsActiveScrollText = true;
            if (unitInfo != null)
            {
                unitInfo.setViewScroll();
            }
        });

        if (dialogType == DialogType.DialogScrollInfo)
        {
            // ScrollRectで勝手にスクロールする時があるので強制的にトップ位置にする
            StartCoroutine(WaitScrollContent());
        }
    }

    IEnumerator WaitScrollContent()
    {
        yield return null;
        ScrollRect srect = getObject(DialogObjectType.ScrollInfo).GetComponent<ScrollRect>();
        if (srect == null) yield break;
        float height = srect.content.transform.parent.GetComponent<RectTransform>().sizeDelta.y;
        float contentHeight = srect.content.sizeDelta.y;
        if (contentHeight <= height) yield break;
        srect.verticalNormalizedPosition = 1;
    }


    /// <summary>
    /// ダイアログを閉じる
    /// </summary>
    public void Hide(System.Action action = null)
    {
        if (hide)
        {
            return;
        }

        hide = true;

        //バックキーが押された時のアクションを解除
        AndroidBackKeyManager.Instance.StackPop(gameObject);

        if (fadePanelFlag)
        {
            fadePanel.GetComponent<Image>().DOFade(0.0f, 0.25f);
        }
        else
        {
            fadePanel2.GetComponent<Image>().DOFade(0.0f, 0.25f);
        }

        dialogBG.transform.DOScaleY(0f, 0.25f).OnComplete(() =>
        {
            if (action != null)
            {
                action();
            }
            DestroyObject(gameObject);
        });
    }

    /// <summary>
    /// ボタンアクション設定
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="_type"></param>
    private void setButtonAction(Button btn, DialogButtonEventType _type)
    {
        if (btn == null)
        {
            return;
        }

        btn.onClick.AddListener(() =>
        {
            DialogButtonEventType tmp = _type;
            OnPushButton(tmp);
        });
    }

    /// <summary>
    /// オブジェクト取得
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    private GameObject getObject(DialogObjectType _type)
    {
        return objectArray[(int)_type];
    }

    private bool checkObject(DialogObjectType _type)
    {
        if (getObject(_type) == null)
        {
            return false;
        }

        return UnityUtil.ChkObjectEnabled(getObject(_type));
    }

    /// <summary>
    /// すべてのオブジェクトの表示を消す
    /// </summary>
    private void setAllObjectDisable()
    {
        for (int i = 0; i < objectArray.Length; i++)
        {
            //キャンセルは除外
            if (i == (int)DialogObjectType.Cancel)
            {
                continue;
            }

            if (objectArray[i] == null)
            {
                continue;
            }

            UnityUtil.SetObjectEnabledOnce(objectArray[i], false);
        }
    }

    /// <summary>
    /// ユニット情報設定
    /// </summary>
    /// <param name="_unitInfoType"></param>
    /// <param name="_index"></param>
    public void setUnitInfo(DialogUnitInfoType _unitInfoType, int _index)
    {
        if (unitInfo == null)
        {
            return;
        }

        if (!UserDataAdmin.HasInstance)
        {
            return;
        }

        PacketStructUnit mainUnit = null;
        PacketStructUnit subUnit = null;
        bool dispCharm = false;

        switch (_unitInfoType)
        {
            case DialogUnitInfoType.PLAYER:
                {
                    mainUnit = UserDataAdmin.Instance.m_StructPlayer.unit_list[_index];
                    subUnit = CharaLinkUtil.GetLinkUnit(mainUnit.link_unique_id);
                }
                break;
            case DialogUnitInfoType.FRIEND:
                {
                    PacketStructFriend friend = UserDataAdmin.Instance.m_StructFriendList[_index];
                    mainUnit = friend.unit;
                    if (mainUnit.link_info == (uint)CHARALINK_TYPE.CHARALINK_TYPE_BASE)
                    {
                        subUnit = friend.unit_link;
                    }
                }
                break;
            case DialogUnitInfoType.HELPER:
                {
                    PacketStructFriend helper = UserDataAdmin.Instance.m_StructHelperList[_index];
                    mainUnit = helper.unit;
                    if (mainUnit.link_info == (uint)CHARALINK_TYPE.CHARALINK_TYPE_BASE)
                    {
                        subUnit = helper.unit_link;
                    }
                }
                break;
        }

        if (mainUnit == null)
        {
            return;
        }

        unitInfo.setupChara(mainUnit, subUnit, dispCharm);
    }

    /// <summary>
    /// ユニット情報設定
    /// </summary>
    /// <param name="_friend"></param>
    public void setUnitInfo(PacketStructFriend _friend)
    {
        PacketStructUnit mainUnit = null;
        PacketStructUnit subUnit = null;
        bool dispCharm = false;

        mainUnit = _friend.unit;
        if (mainUnit.link_info == (uint)CHARALINK_TYPE.CHARALINK_TYPE_BASE)
        {
            subUnit = _friend.unit_link;
        }

        if (mainUnit == null)
        {
            return;
        }

        unitInfo.setupChara(mainUnit, subUnit, dispCharm);
    }

    /// <summary>
    /// フレンド情報設定
    /// </summary>
    /// <param name="_friend"></param>
    public void SetFriendInfo(PacketStructFriend _friend, bool bCheckLock = false)
    {
        GameObject _obj = loadPrefab("Prefab/FriendList/FriendDataItem");
        if (_obj == null)
        {
            return;
        }

        FriendDataItem _friendItem = _obj.GetComponent<FriendDataItem>();
        if (_friendItem == null)
        {
            return;
        }

        GameObject _friendInfoObj = getObject(DialogObjectType.FriendInfo);
        _friendItem.transform.SetParent(_friendInfoObj.transform, false);

        FriendDataSetting _setting = new FriendDataSetting();
        _setting.FriendData = _friend;
        _setting.MasterData = MasterFinder<MasterDataParamChara>.Instance.Find((int)_friend.unit.id);
        _setting.CharaOnce = MainMenuUtil.CreateFriendCharaOnce(_friend);
        _friendItem.setup(0, _setting, FriendDataItem.ParamType.NAME, bCheckLock);
    }

    public void AddScrollInfoImage(string _url)
    {
        GameObject _obj = loadPrefab("Prefab/Dialog/ScrollInfo/DialogInfoImage");
        if (_obj == null)
        {
            return;
        }

        DialogInfoImage _infoImage = _obj.GetComponent<DialogInfoImage>();
        if (_infoImage == null)
        {
            return;
        }
        _infoImage.setup(_url);
    }

    public void AddScrollInfoText(string _message)
    {
        GameObject _obj = loadPrefab("Prefab/Dialog/ScrollInfo/DialogInfoText");
        if (_obj == null)
        {
            return;
        }
        DialogInfoText _infoText = _obj.GetComponent<DialogInfoText>();
        if (_infoText == null)
        {
            return;
        }
        _infoText.Message = checkLimitString(_message);
    }

    public void AddScrollInfoIconList(string _title, uint[] icon_ids)
    {
        GameObject _obj = loadPrefab("Prefab/Dialog/ScrollInfo/DialogInfoIconList");
        if (_obj == null)
        {
            return;
        }
        DialogInfoIconList _infoIconList = _obj.GetComponent<DialogInfoIconList>();
        if (_infoIconList == null)
        {
            return;
        }
        _infoIconList.setup(_title, icon_ids, SelectInfoIcon);
        updateLayoutCount = 5;
    }

    private void SelectInfoIcon(uint _char_id)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.OpenUnitDetailInfoCatalog(_char_id);
        }
    }

    private GameObject loadPrefab(string prefabName)
    {
        GameObject _tmpObj = Resources.Load(prefabName) as GameObject;
        if (_tmpObj == null)
        {
            return null;
        }

        GameObject _insObj = Instantiate(_tmpObj) as GameObject;
        if (_insObj == null)
        {
            return null;
        }

        _insObj.transform.SetParent(scrollInfoContent.transform, false);
        return _insObj;

    }
    public void updateLayout()
    {
        LayoutGroup[] layoutGroups = GetComponentsInChildren<LayoutGroup>();
        for (int i = 0; i < layoutGroups.Length; i++)
        {
            LayoutRebuilder.MarkLayoutForRebuild(layoutGroups[i].transform as RectTransform);
        }
    }

    public void SetDialogEnable(bool bFlag)
    {
        canvasSetting.SetCanvasEnable(bFlag);
    }

    public void SetMenuInButton(bool bFlag)
    {
        UnityUtil.SetObjectEnabledOnce(getObject(DialogObjectType.MenuInButton), bFlag);
    }

    public void DisableBackKey()
    {
        backkey = false;
    }

    public void changeQualitySwitch(bool sw)
    {
        if (sw == true)
        {
            m_buttons[(int)DialogButtonEventType.QUALITYHIGH]._switch = false;
        }
        else
        {
            m_buttons[(int)DialogButtonEventType.QUALITYHIGH]._switch = true;
        }
    }

    public bool changeQualitySwitch()
    {
        if (m_buttons[(int)DialogButtonEventType.QUALITYHIGH]._switch == true)
        {
            m_buttons[(int)DialogButtonEventType.QUALITYHIGH]._switch = false;
        }
        else
        {
            m_buttons[(int)DialogButtonEventType.QUALITYHIGH]._switch = true;
        }
        return m_buttons[(int)DialogButtonEventType.QUALITYHIGH]._switch;
    }

    /// <summary>
    /// 表示文字数チェック
    /// </summary>
    /// <param name="_text"></param>
    /// <returns></returns>
    public string checkLimitString(string _text)
    {
        // Unityでは15000文字以上は表示できないらしいので
        // 安全のため14000文字以上は削除して表示
        string retText = "";
        if (_text.Length > 14000)
        {
            retText = _text.Substring(0, 14000);
        }
        else
        {
            retText = _text;
        }
        return retText;
    }

    public void addUnderButton(string title, System.Action action)
    {
        DialogUnderButtonContext context = new DialogUnderButtonContext();
        context.Title = title;
        context.DidSelectItem = action;
        Under_button_list.Add(context);
    }

    public void setupUnderKiyaku()
    {
        SetDialogObjectEnabled(DialogObjectType.UnserButtonList, true);
        addUnderButton(GameTextUtil.GetText("he172p_buttontitle2"), () =>
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            string kessai_url = MasterDataUtil.GetMasterDataGlobalParamTextFromID(GlobalDefine.WEB_LINK_SHIKIN_KESSAI);
            URLManager.OpenURL(kessai_url);
        });
        addUnderButton(GameTextUtil.GetText("he172p_buttontitle3"), () =>
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            string torihiki_url = MasterDataUtil.GetMasterDataGlobalParamTextFromID(GlobalDefine.WEB_LINK_TOKUTEI_TORIHIKI);
            URLManager.OpenURL(torihiki_url);
        });
    }

    public void addVerticalButton(string title, System.Action closeAction, System.Action hideAction = null)
    {
        var buttonModel = new DialogButtonModel();
        var view = DialogButtonView.Attach<DialogButtonView>(TransitionButtonPrefabPath, getObject(DialogObjectType.VerticalButtonList));
        view.SetModel<ButtonModel>(buttonModel);
        view.SetText(title);

        buttonModel.OnClicked += () =>
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK);
            if (closeAction != null)
            {
                closeAction();
            }

            if (autoHide)
            {
                Hide(hideAction);
            }
        };

        // TODO : 演出あれば入れる
        buttonModel.Appear();
        buttonModel.SkipAppearing();
    }

    public Dialog enableInputField()
    {
        GameObject transObj = getObject(DialogObjectType.TransferPassword);
        TMP_InputField[] fields = transObj.GetComponentsInChildren<TMP_InputField>();
        if (fields != null &&
            fields.Length > 0)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i].enabled = true;
            }
        }
        return this;
    }

    /*-------------------------------------------------------------------------------------*/
    /*                                                                                     */
    /*                                                                                     */
    /*                                                                                     */
    /*-------------------------------------------------------------------------------------*/

    /// <summary>
    /// ダイアログ生成カウンタ
    /// </summary>
    public static int dialogCounter = 0;

    /// <summary>
    /// ダイアログ生成
    /// </summary>
    /// <param name="dialogType"></param>
    /// <returns>ダイアログクラス</returns>
    public static Dialog Create(DialogType dialogType)
    {

        GameObject _tmpObj = Resources.Load("Prefab/Dialog/Dialog") as GameObject;
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

        Dialog newDialog = _newObj.GetComponent<Dialog>();
        if (newDialog == null)
        {
            return null;
        }
        newDialog.SettingDialog(dialogType);

        int dlg_count = GetDialog().Length;
        if (dlg_count != 0)
        {
            newDialog.canvasSetting.ChangeSortingOrder(dlg_count);
        }

        //newDialog.dialogID = dialogCounter;
        dialogCounter++;

        return newDialog;
    }

    public static Dialog[] GetDialog()
    {
        GameObject[] dlgArray = GameObject.FindGameObjectsWithTag("Dialog");
        Dialog[] _ret = new Dialog[dlgArray.Length];
        for (int i = 0; i < dlgArray.Length; i++)
        {
            _ret[i] = dlgArray[i].GetComponent<Dialog>();
        }
        return _ret;
    }

    public static void HideAll()
    {
        Dialog[] dlgArray = GetDialog();
        foreach (Dialog dlg in dlgArray)
        {
            if (dlg != null) dlg.Hide();
        }
    }

    public static bool HasDialog()
    {
        Dialog[] dlgArray = GetDialog();
        if (dlgArray.Length == 0)
        {
            return false;
        }

        return true;
    }

    public static void SetDialogEnableAll(bool bFlag)
    {
        Dialog[] dlgArray = GetDialog();
        for (int i = 0; i < dlgArray.Length; i++)
        {
            dlgArray[i].SetDialogEnable(bFlag);
        }
    }

}
