/**
 *  @file   WebView.cs
 *  @brief
 *  @author Developer
 *  @date   2016/12/20
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WebView : SingletonComponent<WebView>
{
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
    /// <summary>
    /// UniWebViewでのページを戻る処理はボタンを押した一つ前に戻るので二つのページを行き来する動作になるもよう。
    /// Plugin/iOS/UniWebView.mmのshouldStartLoadWithRequestでnavigationTypeがUIWebViewNavigationTypeBackForward時は
    /// 何もせずreturn YESで戻るように修正で通常動作になった。
    /// </summary>
    UniWebView m_WebView;
#endif
    [SerializeField]
    private GameObject m_TopMask;
    [SerializeField]
    private GameObject m_BottomMask;

    [SerializeField]
    RectTransform m_ButtomRect;
    [SerializeField]
    Button m_CloseButton;
    [SerializeField]
    Button m_ReturnButton;
    [SerializeField]
    RectTransform m_BackGroundRect;
    [SerializeField]
    RectTransform m_PopupButtomRect;
    [SerializeField]
    Button m_PopupCloseButton;
    [SerializeField]
    Button m_PopupConfirmationButton;
    [SerializeField]
    TextMeshProUGUI m_Message;

    GameObject lastDialog;

    Action m_CloseAction;

    private BGMPlayData[] m_TmpPrevPlayDatas;
    /// <summary>止めたBGMを再生するかどうか</summary>
    private bool m_ReturnableBGM = true;

    private uint m_WebViewFixID = 0;
    private bool m_WebViewOpen = false;

    public bool isOpen { get { return m_WebViewOpen; } }

    static WebView Create()
    {
        GameObject _tmpObj = Resources.Load<GameObject>("Prefab/WebView/WebView");
        if (_tmpObj == null)
        {
            return null;
        }
        GameObject _newObj = Instantiate(_tmpObj);
        if (_newObj == null)
        {
            return null;
        }
        WebView webView = _newObj.GetComponent<WebView>();

        return webView;
    }

    /// <summary>
    /// インスタンスの取得または生成
    /// </summary>
    /// <returns></returns>
    static WebView GetInstance()
    {
        if (!HasInstance)
        {
            return Create();
        }

        return Instance;
    }

    /// <summary>
    /// WebViewを開きます
    /// </summary>
    /// <param name="url"></param>
    /// <param name="closeAction">閉じたときのアクション</param>
    public static WebView OpenWebView(string url, Action closeAction, uint btype = 0)
    {
        return GetInstance()._OpenWebView(url, closeAction, btype);
    }

    /// <summary>
    /// WebViewを開きます
    /// </summary>
    /// <param name="url"></param>
    public static WebView OpenWebView(string url, uint btype = 0)
    {
        return GetInstance()._OpenWebView(url, null, btype);
    }


    /// <summary>
    /// WebViewを閉じます
    /// </summary>
    public static void CloseWebView()
    {
        GetInstance()._CloseWebView();
    }

    protected override void Awake()
    {
        base.Awake();
        m_CloseButton.colors = ColorBlockUtil.BUTTON_WHITE;
        m_CloseButton.onClick.AddListener(_CloseWebView);

        m_CloseButton.GetComponentInChildren<TextMeshProUGUI>().text = UnityUtil.GetText("common_button1");

        m_ReturnButton.onClick.AddListener(_ReturnWebView);

        m_ReturnButton.GetComponentInChildren<TextMeshProUGUI>().text = UnityUtil.GetText("common_button6");

        m_PopupCloseButton.onClick.AddListener(_CloseWebView);

        m_PopupCloseButton.GetComponentInChildren<TextMeshProUGUI>().text = UnityUtil.GetText("common_button1");

        m_PopupConfirmationButton.onClick.AddListener(_ConfirmationWebView);

        m_PopupConfirmationButton.GetComponentInChildren<TextMeshProUGUI>().text = UnityUtil.GetText("BTN_RECON");

        m_Message.text = UnityUtil.GetText("popup_check");
    }

    protected override void Start()
    {
        base.Start();

        lastDialog = GameObject.Find("Dialog(Clone)");
        if (lastDialog != null)
        {
            lastDialog.SetActive(false);
        }


        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.enebleMask(m_TopMask, null);

            int bottom_space_height = SafeAreaControl.Instance.bottom_space_height;
            if (bottom_space_height > 0)
            {
                m_TopMask.transform.AddLocalPositionY(SafeAreaControl.Instance.bar_height);
                m_ButtomRect.sizeDelta = new Vector2(m_ButtomRect.sizeDelta.x, m_ButtomRect.sizeDelta.y + bottom_space_height);
                m_PopupButtomRect.sizeDelta = new Vector2(m_PopupButtomRect.sizeDelta.x, m_PopupButtomRect.sizeDelta.y + bottom_space_height);
                m_BackGroundRect.offsetMax = new Vector2(m_BackGroundRect.offsetMax.x, -SafeAreaControl.Instance.bar_height);
                m_BottomMask.SetActive(true);
            }
        }
    }

    WebView _OpenWebView(string url, Action closeAction, uint fixid)
    {
        m_CloseAction = closeAction;
        m_WebViewFixID = fixid;
        m_WebViewOpen = true;

        if (BGMManager.HasInstance)
        {
            // 再生しているBGMを退避する
            m_TmpPrevPlayDatas = BGMManager.Instance.GetPlayingBGM();
        }
        SoundUtil.StopBGM(false);

#if UNITY_EDITOR_OSX
        //MacでWebを表示すると固まるのでスキップ
        _ButtonSetting();
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8
        // UniWebView: Main Page
        // http://uniwebview.onevcat.com/reference/class_uni_web_view.html

        if (m_WebView == null)
        {
            m_WebView = gameObject.AddComponent<UniWebView>();
        }

        m_WebView.immersiveMode = false;

        _ButtonSetting();

        m_WebView.OnLoadBegin += OnLoadBeginDelegate;
        m_WebView.OnLoadComplete += OnLoadComplete;
        m_WebView.OnReceivedMessage += OnReceivedMessageDelegate;
        m_WebView.InsetsForScreenOreitation += InsetsForScreenOreitation;
        m_WebView.OnWebViewShouldClose += OnWebViewShouldClose;

        UniWebView.SetHomeDomain(GlobalDefine.IN_BROWSER_DOMAIN);
        List<string> urls = Patcher.Instance.GetInBrowserUrls();
        UniWebView.SetInBrowserUrls(string.Join(",", urls.ToArray()));

#if UNITY_ANDROID || UNITY_WP8
        setSchemes(true);
#endif
        m_WebView.url = url;

        m_WebView.Load();
#endif

        return this;
    }

    /// <summary>
    /// WebViewを閉じる
    /// </summary>
    void _ButtonSetting()
    {
        if (m_WebViewFixID == 0)
        {
            UnityUtil.SetObjectEnabled(m_ButtomRect.gameObject, true);
            UnityUtil.SetObjectEnabled(m_PopupButtomRect.gameObject, false);
        }
        else
        {
            UnityUtil.SetObjectEnabled(m_ButtomRect.gameObject, false);
            UnityUtil.SetObjectEnabled(m_PopupButtomRect.gameObject, true);
            //            m_PopupToggle.isOn = MainMenuWebViewShowChk.GetViewCheck(m_WebViewFixID);
            MasterDataWebView master = MasterFinder<MasterDataWebView>.Instance.Find((int)m_WebViewFixID);
            if (master != null)
            {
                if ((MasterDataDefineLabel.WebviewTransitionType)master.webview_param_4 == MasterDataDefineLabel.WebviewTransitionType.NO_TRANSITION)
                {
                    // 遷移先が無いとき
                    UnityUtil.SetObjectEnabled(m_PopupConfirmationButton.gameObject, false);
                    var pos = m_PopupCloseButton.gameObject.GetComponent<RectTransform>().localPosition;
                    pos.x = 0;
                    m_PopupCloseButton.gameObject.GetComponent<RectTransform>().localPosition = pos;
                }
            }
        }
    }

    /// <summary>
    /// WebViewを閉じる
    /// </summary>
    void _CloseWebView()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        WebViewEnd();
    }

    /// <summary>
    /// WebViewを戻る
    /// </summary>
    void _ReturnWebView()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_RET);
#if UNITY_EDITOR_OSX
        //MacでWebを表示すると固まるのでスキップ
        return;
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8
        if (m_WebView.CanGoBack() == true)
        {
            m_WebView.GoBack();
        }
#endif
    }

    /// <summary>
    /// WebViewから別ページに遷移
    /// </summary>
    void _ConfirmationWebView()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        MasterDataWebView master = MasterFinder<MasterDataWebView>.Instance.Find((int)m_WebViewFixID);
        if (master != null)
        {
            switch ((MasterDataDefineLabel.WebviewTransitionType)master.webview_param_4)
            {
                case MasterDataDefineLabel.WebviewTransitionType.AREAMAP:// areamap
                    MainMenuParam.m_RegionID = MasterDataUtil.GetRegionIDFromCategory(MasterDataDefineLabel.REGION_CATEGORY.STORY);
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_STORY, false, false);
                    break;
                case MasterDataDefineLabel.WebviewTransitionType.SCRATCH:// scratch
                    MasterDataGacha[] scratchMaster = MasterDataUtil.GetActiveGachaMaster();
                    if (scratchMaster.IsNullOrEmpty() == false)
                    {
                        MainMenuParam.m_GachaMaster = scratchMaster.FirstOrDefault(g => g.fix_id == master.webview_param_5);
                    }
                    else
                    {
                        MainMenuParam.m_GachaMaster = null;
                    }
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_GACHA_MAIN, false, false);
                    break;
                case MasterDataDefineLabel.WebviewTransitionType.AREA:// area
                    MainMenuParam.SetQuestSelectParam(master.webview_param_5, master.webview_param_6);
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT, false, false, false);
                    break;
                case MasterDataDefineLabel.WebviewTransitionType.CHIP:// chip
                    StoreDialogManager.Instance.OpenBuyStone();
                    break;
                case MasterDataDefineLabel.WebviewTransitionType.POINTSHOP:// point shop
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_SHOP_POINT, false, false);
                    break;
                default:
                    break;
            }
        }

        WebViewEnd();
    }

    /// <summary>
    /// WebView終了処理
    /// </summary>
    void WebViewEnd()
    {
        // 止めていたBGMの再生
        if (m_TmpPrevPlayDatas != null && m_ReturnableBGM)
        {
            foreach (BGMPlayData playData in m_TmpPrevPlayDatas)
            {
                SoundUtil.PlayBGM(playData);
            }
        }

        if (lastDialog != null)
        {
            lastDialog.SetActive(true);
            lastDialog = null;
        }

#if UNITY_EDITOR_OSX
        //MacでWebを表示すると固まるのでスキップ
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8
        m_WebView.Hide();
        m_WebView.CleanCache();
#endif

        if (m_CloseAction != null)
        {
            m_CloseAction();
        }

        DestroyObject(gameObject);

        if (m_WebViewFixID != 0)
        {
            MainMenuWebViewShowChk.SetViewCheck(m_WebViewFixID, true);
        }
        m_WebViewOpen = false;
    }

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8

    void OnReceivedMessageDelegate(UniWebView webView, UniWebViewMessage message)
    {
        Uri uri = null;
        try
        {
            uri = new Uri(message.rawMessage);
        }
        catch (Exception e)
        {
            return;
        }


        if (URLManager.CheckLoadURL(uri))
        {
            webView.Load(message.rawMessage);
        }
        else
        {
            Application.OpenURL(message.rawMessage);
        }

#if BUILD_TYPE_DEBUG
        Debug.Log("OnReceivedMessageDelegate: message.rawMessage" + message.rawMessage + " / " + webView.openLinksInExternalBrowser);
#endif
    }

    void OnLoadBeginDelegate(UniWebView webView, string loadingUrl)
    {
    }

    void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
    {
        if (success)
        {
            webView.Show();
        }
        else
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("Something wrong in webview loading: " + errorMessage);
#endif
        }
    }

    public void DisableReturnableBGM()
    {
        m_ReturnableBGM = false;
    }

    UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation)
    {
        float webViewTop = 0;   // マージン：上
        float widthMarzine = 0; // マージン：横
        float webViewBottom = m_ButtomRect.sizeDelta.y; // マージン：下
        if (m_WebViewFixID != 0)
        {
            webViewBottom = m_PopupButtomRect.sizeDelta.y; // マージン：下
        }

        float screenWidth = UniWebViewHelper.screenWidth;  // 画面サイズ：横幅
        float screenHeight = UniWebViewHelper.screenHeight;    // 画面サイズ：高さ

        Canvas canvas = GetComponentInChildren<Canvas>();
        float canvasHeight = 0;  // Canvasサイズ：高さ
        float canvasWidth = 0; // Canvasサイズ：横幅
        if (canvas != null)
        {
            RectTransform rect = canvas.GetComponent<RectTransform>();
            canvasHeight = rect.rect.height;
            canvasWidth = rect.rect.width;
        }

        // Canvasの基準解像度：横の縦からの割合
        float baseWidthRatio = GlobalDefine.SCREEN_SIZE_W / canvasHeight;

        // 起動デバイスの解像度：横の縦からの割合
        float revWidthRatio = screenWidth / screenHeight;

        if (baseWidthRatio < revWidthRatio)
        {
            // 両サイドのマージン領域を求める。
            widthMarzine = (canvasWidth - GlobalDefine.SCREEN_SIZE_W) / 2.0f;
        }

        // 基準サイズとスクリーンサイズの横幅の比率を求める
        float rate = screenHeight / canvasHeight;
        webViewBottom *= rate;
        widthMarzine *= rate;

        // トップのマージンはAndroid実機の場合Activityからとってくる(ステータスバー対策)
#if !UNITY_EDITOR
        webViewTop = SafeAreaControl.Instance.getNativeTop();
#endif

        return new UniWebViewEdgeInsets((int)webViewTop, (int)widthMarzine, (int)webViewBottom, (int)widthMarzine);
    }

    bool OnWebViewShouldClose(UniWebView webView)
    {
        _CloseWebView();
        return true;
    }

    void setSchemes(bool setscheme)
    {
        if (setscheme)
        {
            m_WebView.AddUrlScheme("http");
            m_WebView.AddUrlScheme("https");
        }
        else
        {
            m_WebView.RemoveUrlScheme("http");
            m_WebView.RemoveUrlScheme("https");
        }
    }
#endif
}
