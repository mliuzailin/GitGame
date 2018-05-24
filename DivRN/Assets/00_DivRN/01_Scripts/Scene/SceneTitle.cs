using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;
using UnityEngine;
using ServerDataDefine;
using M4u;
using UniExtensions;
using UnityEngine.UI;
using StrOpe = StringOperationUtil.OptimizedStringOperation;
using TMPro;
using DG.Tweening;

public class SceneTitle : Scene<SceneTitle>
{
    private static readonly string OPENING_MOVIE_FILE_NAME = "OpeningMovie.mp4";
    private static readonly string START_BUTTON_PREFAB_PATH = "Prefab/Title/TitleStartButotn";
    private static readonly string MENU_BUTTON_PREFAB_PATH = "Prefab/Title/TitleMenuButotn";
#if !UNITY_IOS
    private static readonly string GOOGLE_SINGIN_PREFAB_PATH = "Prefab/Title/TitleGoogleSignInButotn";
    private static readonly string GOOGLE_SINGOUT_PREFAB_PATH = "Prefab/Title/TitleGoogleSignOutButotn";
#endif

    [SerializeField]
    private GameObject m_TopMask;
    [SerializeField]
    private GameObject m_BottomMask;

    [SerializeField]
    private GameObject m_startButtonRoot;
    [SerializeField]
    private GameObject m_menuButtonRoot;
#if !UNITY_IOS
    [SerializeField]
    private GameObject m_btn_google_signin;
    [SerializeField]
    private GameObject m_btn_google_signout;
#endif
    [SerializeField]
    private GameObject m_btn_api_debug;
    [SerializeField]
    private GameObject m_btn_getdb;
    [SerializeField]
    private GameObject m_btn_brokendb;
    [SerializeField]
    private GameObject m_btn_brokenuuid;
    [SerializeField]
    private GameObject m_btn_safeareamask;
    [SerializeField]
    private TextMeshProUGUI textClientVersion = null;
    [SerializeField]
    private TextMeshProUGUI textDebugInfo = null;

    [SerializeField]
    private GameObject DispSize;
    [SerializeField]
    private GameObject m_btn_debugoffbutton;
    [SerializeField]
    private GameObject tutorialToggle;
    [SerializeField]
    private GameObject nextVersionToggle;

    [SerializeField]
    private TextMeshProUGUI m_UseridText;                           //!<
    [SerializeField]
    private TextMeshProUGUI m_Copyright;                            //!<

    [SerializeField]
    private GameObject m_BottomAnchor;


    GameObject m_MoviePrefab;

    [SerializeField]
    private Banner m_Banner = null;

    private bool transfer = false;
    private bool google_signin_chk = false;
    private bool m_firstPolicy = true;
    private bool m_firstAgreement = true;

    private Semaphore m_semaphore = new Semaphore();


    protected override void Start()
    {
        base.Start();

        //ローカルに保存してあるWebリソースを全削除
        WebResource.Instance.RemoveAll();

        //画面表示OFF
        GetComponentInChildren<CanvasGroup>().alpha = 0.0f;
    }

    protected override void Update()
    {
        base.Update();

        m_semaphore.Tick();

#if !UNITY_IOS
        if (google_signin_chk == false)
        {
            return;
        }

        //----------------------------------------
        //
        //----------------------------------------
        if (UnityUtil.ChkObjectEnabled(m_btn_google_signin) == true)
        {
            //----------------------------------------
            // ボタン表示中にサインインが完遂したなら表示を消す
            //----------------------------------------
            if (PlayGameServiceUtil.isSignedIn() == true)
            {
#if !UNITY_IPHONE
                UnityUtil.SetObjectEnabled(m_btn_google_signin, false);
#endif
#if BUILD_TYPE_DEBUG && UNITY_ANDROID && !UNITY_EDITOR
				UnityUtil.SetObjectEnabled(m_btn_google_signout, true);
#endif
            }
        }
        else
        {
            //----------------------------------------
            // ボタン非表示中にサインアウトが成立したなら表示を出す
            //----------------------------------------
            if (PlayGameServiceUtil.isSignedIn() == false)
            {
#if !UNITY_IPHONE
                UnityUtil.SetObjectEnabled(m_btn_google_signin, true);
#endif
#if BUILD_TYPE_DEBUG && UNITY_ANDROID && !UNITY_EDITOR
				UnityUtil.SetObjectEnabled(m_btn_google_signout, false);
#endif
            }
        }
#endif
    }

    public override SceneType GetSceneType()
    {
        return SceneType.SceneTitle;
    }

    public override void OnInitialized()
    {
        base.OnInitialized();

        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.enebleMask(m_TopMask, m_BottomMask);

            SafeAreaControl.Instance.addLocalYPos(m_BottomAnchor.transform);
            SafeAreaControl.Instance.addLocalYPos(m_Banner.transform);
        }

        m_Banner.LoadAndShow(Banner.Type.Title);

        UpdateClientVersion();

        //デバッグのときに表示
#if API_SELECT_DEBUG
        UnityUtil.SetObjectEnabled(m_btn_api_debug, true);
#else
        Destroy(m_btn_api_debug);
#endif

#if BUILD_TYPE_DEBUG
        UnityUtil.SetObjectEnabled(m_btn_getdb, true);
        UnityUtil.SetObjectEnabled(m_btn_brokendb, true);
        UnityUtil.SetObjectEnabled(m_btn_brokenuuid, true);
        UnityUtil.SetObjectEnabled(m_btn_safeareamask, true);
        UnityUtil.SetObjectEnabled(tutorialToggle, true);
        UnityUtil.SetObjectEnabled(nextVersionToggle, true);

        if (nextVersionToggle != null)
        {
            nextVersionToggle.GetComponent<Toggle>().isOn = DebugOption.Instance.noneNextVersion;
        }
#else
        Destroy(m_btn_getdb);
        Destroy(m_btn_brokendb);
        Destroy(m_btn_brokenuuid);
        Destroy(m_btn_safeareamask);
        Destroy(tutorialToggle);
        Destroy(nextVersionToggle);
#endif

#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
        UnityUtil.SetObjectEnabled(m_btn_debugoffbutton, true);
#else
        UnityUtil.SetObjectEnabled(m_btn_debugoffbutton, false);
#endif

        InputLayer.Attach();

        SetUpButtons();
        OnDebugOffButtonUpdate();

        //
        AndroidBackKeyManager.Instance.StackPush(gameObject, OnSelectBackKey);

        if (ResidentParam.m_IsGoToTileWithApiError == true && LoadingManager.Instance != null)
        {
            // クエスト受注などでサーバーエラーでタイトルに戻された時、
            // インジケーターが表示されたままになってしまうので非表示
            LoadingManager.Instance.RequestLoadingFinish();
        }

        ResidentParam.PramResetTitle();
    }

    private void UpdateUserID()
    {
        uint uid = LocalSaveManager.Instance.LoadFuncUserID();
        if (uid == 0)
        {
            m_UseridText.enabled = false;
        }
        else
        {
            m_UseridText.enabled = true;
            m_UseridText.text = string.Format(GameTextUtil.GetText("pp13p_display1"), UnityUtil.CreateDrawUserID(uid));
        }
    }

    private void UpdateClientVersion()
    {
        if (textClientVersion != null)
        {
            UnityUtil.SetObjectEnabled(textClientVersion.gameObject, true);

#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
            textDebugInfo.text = GlobalDefine.GetApplicationStatus(true);
#endif
            textClientVersion.text = string.Format(GameTextUtil.GetText("pp13p_display3"), GlobalDefine.StrVersion());
        }
    }

    private void OnSelectBackKey()
    {
        Dialog _newDialog = Dialog.Create(DialogType.DialogYesNo);
        _newDialog.SetDialogText(DialogTextType.Title, "アプリケーション終了");
        _newDialog.SetDialogText(DialogTextType.MainText, "ゲームを終了しますか？");
        _newDialog.SetDialogText(DialogTextType.YesText, "はい");
        _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            //アプリ終了
            Application.Quit();
        });
        _newDialog.SetDialogText(DialogTextType.NoText, "いいえ");
        _newDialog.DisableCancelButton();
        _newDialog.Show();
    }

    private void SetUpButtons()
    {
        var startButtonModel = new ButtonModel();
        var menuButtonModel = new ButtonModel();
        var googleSignInButtonModel = new ButtonModel();
        var googleSignOutButtonModel = new ButtonModel();

        ButtonView
            .Attach<ButtonView>(START_BUTTON_PREFAB_PATH, m_startButtonRoot)
            .SetModel<ButtonModel>(startButtonModel);
        ButtonView
            .Attach<ButtonView>(MENU_BUTTON_PREFAB_PATH, m_menuButtonRoot)
            .SetModel<ButtonModel>(menuButtonModel);
#if !UNITY_IOS
        ButtonView
            .Attach<ButtonView>(GOOGLE_SINGIN_PREFAB_PATH, m_btn_google_signin)
            .SetModel<ButtonModel>(googleSignInButtonModel);
        ButtonView
            .Attach<ButtonView>(GOOGLE_SINGOUT_PREFAB_PATH, m_btn_google_signout)
            .SetModel<ButtonModel>(googleSignOutButtonModel);
#endif

        startButtonModel.OnClicked += () =>
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            OnClickScreen();
        };
        menuButtonModel.OnClicked += () =>
        {
            OnClickMenu();
        };
#if !UNITY_IOS
        googleSignInButtonModel.OnClicked += () =>
        {
            OnClickGoogle();
        };
        googleSignOutButtonModel.OnClicked += () =>
        {
            OnClickGoogleSignout();
        };
#endif

        // TODO : 演出を入れるならそこに移動
        startButtonModel.Appear();
        startButtonModel.SkipAppearing();
        menuButtonModel.Appear();
        menuButtonModel.SkipAppearing();
#if !UNITY_IOS
        googleSignInButtonModel.Appear();
        googleSignInButtonModel.SkipAppearing();
        googleSignOutButtonModel.Appear();
        googleSignOutButtonModel.SkipAppearing();
#endif
    }

    public void OnDebugOffButton()
    {
#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
        DebugOption.Instance.disalbeDebugMenu = !DebugOption.Instance.disalbeDebugMenu;
        Debug.Log("CALL disalbeDebugMenu: " + (DebugOption.Instance.disalbeDebugMenu ? "true" : "false"));
        OnDebugOffButtonUpdate();
#endif
    }

    private void OnDebugOffButtonUpdate()
    {
#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
        if (DebugOption.Instance.disalbeDebugMenu == true)
        {
            UnityUtil.SetObjectEnabled(m_btn_getdb, false);
            UnityUtil.SetObjectEnabled(m_btn_brokendb, false);
            UnityUtil.SetObjectEnabled(m_btn_brokenuuid, false);
            UnityUtil.SetObjectEnabled(m_btn_safeareamask, false);
            UnityUtil.SetObjectEnabled(tutorialToggle, false);
            UnityUtil.SetObjectEnabled(nextVersionToggle, false);
            UnityUtil.SetObjectEnabled(textDebugInfo.gameObject, false);
            UnityUtil.SetObjectEnabled(DispSize, false);
#if API_SELECT_DEBUG
            UnityUtil.SetObjectEnabled(m_btn_api_debug, false);
#endif
        }
        else
        {
            UpdateClientVersion();

            UnityUtil.SetObjectEnabled(m_btn_getdb, true);
            UnityUtil.SetObjectEnabled(m_btn_brokendb, true);
            UnityUtil.SetObjectEnabled(m_btn_brokenuuid, true);
            UnityUtil.SetObjectEnabled(m_btn_safeareamask, true);
            UnityUtil.SetObjectEnabled(tutorialToggle, true);
            UnityUtil.SetObjectEnabled(nextVersionToggle, true);
            UnityUtil.SetObjectEnabled(textDebugInfo.gameObject, true);
            UnityUtil.SetObjectEnabled(DispSize, true);

#if API_SELECT_DEBUG
            UnityUtil.SetObjectEnabled(m_btn_api_debug, true);
#endif
        }
#endif
    }

#if !UNITY_IOS
    public void OnClickGoogle()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL SceneTitle#OnClickGoogle");
        Debug.Log("PlayGameService Sign In!!");
#endif
        //----------------------------------------
        // サインイン実行
        //----------------------------------------
        PlayGameServiceUtil.SignIn();

        //----------------------------------------
        // サインインが済んだら表示オフ
        //----------------------------------------
#if !UNITY_IPHONE
        UnityUtil.SetObjectEnabled(m_btn_google_signin, false);
#endif
#if BUILD_TYPE_DEBUG && UNITY_ANDROID && !UNITY_EDITOR
		UnityUtil.SetObjectEnabled(m_btn_google_signout, true);
#endif

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    public void OnClickGoogleSignout()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL SceneTitle#OnClickGoogleSignout");
        Debug.Log("PlayGameService Sign Out!!");
#endif
        //----------------------------------------
        // サインイン実行
        //----------------------------------------
        PlayGameServiceUtil.SignOut();

        //----------------------------------------
        // サインインが済んだら表示オフ
        //----------------------------------------
#if !UNITY_IPHONE
        UnityUtil.SetObjectEnabled(m_btn_google_signin, true);
#endif
#if BUILD_TYPE_DEBUG && UNITY_ANDROID && !UNITY_EDITOR
		UnityUtil.SetObjectEnabled(m_btn_google_signout, false);
#endif

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }
#endif

    /////////////////
    // MENU
    /////////////////

    readonly string mt14q_button_x = "データの整合性チェック";

    public void OnClickMenu()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL SceneTitle#OnClickMenu");
#endif

        Dialog newDialog = Dialog.Create(DialogType.DialogMenu);
        List<DialogMenuItem> menuList = new List<DialogMenuItem>();
        menuList.Add(new DialogMenuItem("", UnityUtil.GetText("mt14q_button_1"), OnSelectQualitySwitch));
        menuList.Add(new DialogMenuItem("", UnityUtil.GetText("mt14q_button_2"), OnSelectDataTransfer));
        menuList.Add(new DialogMenuItem("", mt14q_button_x, OnSelectBrokenDbCheck));
        menuList.Add(new DialogMenuItem("", UnityUtil.GetText("mt14q_button_3"), OnSelectChashClear));
        menuList.Add(new DialogMenuItem("", UnityUtil.GetText("mt14q_button_4"), OnSelectHelp));
        newDialog.SetMeneList(menuList);
        newDialog.SetDialogObjectEnabled(DialogObjectType.OneButton, true);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mt14q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    public void OnSelectQualitySwitch(DialogMenuItem _item)
    {
        QualitySetting qualitySetting = LocalSaveManagerRN.Instance.QualitySetting;
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnSelectQualitySwitch");
#endif
        Dialog newDialog = Dialog.Create(DialogType.DialogSelectQuality);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mt15q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "mt15q_content_1");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button6");
        newDialog.SetDialogEvent(DialogButtonEventType.QUALITYHIGH, new System.Action(() =>
        {
            if (newDialog.changeQualitySwitch() == true)
            {
                qualitySetting = QualitySetting.HIGH;
            }
            else
            {
                qualitySetting = QualitySetting.NORMAL;
            }
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
            LocalSaveManagerRN.Instance.QualitySetting = qualitySetting;
            LocalSaveManagerRN.Instance.Save();

            //AssetBundleキャッシュの破棄
            LocalAssetBundleClearExec();

            newDialog.Hide();
            OnClickMenu();
        }));
        if (qualitySetting == QualitySetting.NORMAL)
            newDialog.changeQualitySwitch(true);
        else
            newDialog.changeQualitySwitch(false);
        newDialog.DisableAutoHide();
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    public void OnSelectChashClear(DialogMenuItem _item)
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mt19q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "mt19q_content");
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CashDelete!!");
#endif
            ChashClear();
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CashDelete!!");
#endif
            OnClickMenu();
        }));
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    public void OnSelectDataTransfer(DialogMenuItem _item)
    {
        DataTransferExec();
    }

    private string DataTransferUuid = null;
    public void DataTransferExec(string strInputUserID = null, string strInputPassword = null)
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogTransferPassword);

        TMP_InputField[] inputField = newDialog.objectArray[(int)DialogObjectType.TransferPassword].GetChildrenComponent<TMP_InputField>();
        if (strInputUserID != null &&
            strInputUserID.Length > 0)
        {
            inputField[0].text = strInputUserID;
        }

        if (strInputPassword != null &&
            strInputPassword.Length > 0)
        {
            inputField[1].text = strInputPassword;
        }

        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mt16q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button7");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button6");
        newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
            DataTransfer(inputField[0].text, inputField[1].text);
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
        {
            OnClickMenu();
        }));
        newDialog.DisableCancelButton();
        newDialog.EnableFadePanel();
        newDialog.enableInputField();
        newDialog.Show();
    }

    private void DataTransfer(string strInputUserID, string strInputPassword)
    {
        Dialog error_dialog = null;

        if (strInputUserID.Length <= 0)
        {
            error_dialog = DialogManager.Open1B("mt18q_title",
                                                 "mt18q_content1",
                                                 "common_button1",
                                                 true, true);
            error_dialog.DisableCancelButton();
            error_dialog.SetOkEvent(() =>
            {
                DataTransferExec();
            });

            return;
        }

        if (strInputPassword.Length <= 0)
        {
            error_dialog = DialogManager.Open1B("mt18q_title",
                                                 "mt18q_content2",
                                                 "common_button1",
                                                 true, true);
            error_dialog.DisableCancelButton();
            error_dialog.SetOkEvent(() =>
            {
                DataTransferExec(strInputUserID);
            });

            return;
        }

        uint unInputSearchID = UnityUtil.CreateFriendUserID(strInputUserID);
        if (unInputSearchID == 0)
        {
            error_dialog = DialogManager.Open1B("mt18q_title",
                                                 "mt18q_content1",
                                                 "common_button1",
                                                 true, true);
            error_dialog.DisableCancelButton();
            error_dialog.SetOkEvent(() =>
            {
                DataTransferExec(strInputUserID, strInputPassword);
            });

            return;
        }

        LocalSaveManager.Instance.DiffTitleUUID("SceneTitle/DataTransfer");

        new SerialProcess()
            .Add(next =>
            {
                if (DataTransferUuid != null)
                {
                    next();
                    return;
                }

                CreateUserSerialProcess((string uuid) =>
                {
                    DataTransferUuid = uuid;
                },
                (ServerApi.ResultData data) =>
                {
                    next();
                });
            })
            .Add(next =>
            {
                UserAuthenticationSerialProcess(DataTransferUuid,
                                                (ServerApi.ResultData data) =>
                                                {
                                                    next();
                                                });
            })
            .Add(next =>
            {
                ServerDataUtilSend.SendPacketAPI_TransferExec(unInputSearchID,
                                                              strInputPassword,
                                                              DataTransferUuid)
                .setSuccessAction(_data =>
                {
                    //----------------------------------------
                    // 移行前のデータを破棄
                    //----------------------------------------
                    LocalSaveManager.RefreshTransfer();
                    LocalSaveManager.Instance.SaveFuncInformationOK(LocalSaveManager.AGREEMENT.FOX_CALLED);
                    LocalSaveManager.Instance.SaveFuncInformationPolicyOK(LocalSaveManager.AGREEMENT.FOX_CALLED);

                    //----------------------------------------
                    // メインメニューパラメータクリア
                    //----------------------------------------
                    MainMenuHeader.ParamReset();
                    ResidentParam.ParamResetUserRenew();

                    //----------------------------------------
                    // UUID保存
                    //----------------------------------------
                    LocalSaveManager.Instance.SaveFuncUUID(DataTransferUuid);
                    DataTransferUuid = null;

                    LocalSaveManager.Instance.SaveFuncTitleUUID();

                    //useridをセット
                    LocalSaveManager.Instance.SaveFuncUserID(unInputSearchID);
                    UpdateUserID();

                    //----------------------------------------
                    // セーブ移行成功！
                    //----------------------------------------
                    Dialog dialog = DialogManager.Open1B("mt17q_title",
                                                                "mt17q_content",
                                                                "common_button1",
                                                                true, true);
                    dialog.DisableCancelButton();
                    dialog.SetOkEvent(() =>
                    {

                    });
                })
                .setErrorAction(data =>
                {
                    DataTransferExec(strInputUserID, strInputPassword);
                })
                .SendStart();
            })
            .Flush();
    }

    readonly string BrokendbCheck_ButtonText = "確認";

    private void SelectBrokenDbCheckNGDialog(Action action = null)
    {
        LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.SERVER_API);
        StartCoroutine(SQLiteClient.Instance.LocalSqlite3ClearExec(() =>
        {
            LoadingManager.Instance.RequestLoadingFinish();

            Dialog dialog = Dialog.Create(DialogType.DialogOK);
            dialog.SetDialogText(DialogTextType.Title, mt14q_button_x);
            dialog.SetDialogText(DialogTextType.MainText,
                                "データの不整合が発生していました。\n" +
                                "アプリ内キャッシュを修復しました。\n\n" +
                                "アプリケーションを終了します。\n" +
                                "終了後にアプリケーションを立ち上げて\n" +
                                "プレイしてください。\n"
                                 );
            dialog.SetDialogText(DialogTextType.OKText, BrokendbCheck_ButtonText);
            dialog.SetDialogEvent(DialogButtonEventType.OK, () =>
            {
                if (action != null)
                {
                    action();
                }
            });
            dialog.DisableCancelButton();
            dialog.Show();
        }));
    }

    private void SelectBrokenDbCheckOkDialog(Action action = null)
    {
        Dialog dialog = Dialog.Create(DialogType.DialogOK);
        dialog.SetDialogText(DialogTextType.Title, mt14q_button_x);
        dialog.SetDialogText(DialogTextType.MainText, "問題ありませんでした。");
        dialog.SetDialogText(DialogTextType.OKText, BrokendbCheck_ButtonText);
        dialog.SetDialogEvent(DialogButtonEventType.OK, () =>
        {
            if (action != null)
            {
                action();
            }
        });
        dialog.DisableCancelButton();
        dialog.Show();
    }

    public void OnSelectBrokenDbCheck(DialogMenuItem _item)
    {
        if (SQLiteClient.Instance.BrokenDb())
        {
            SelectBrokenDbCheckNGDialog(() =>
            {
                //アプリ終了
                Application.Quit();
            });
        }
        else
        {
            SelectBrokenDbCheckOkDialog();
        }
    }

    private void ChashClear()
    {
        LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.SERVER_API);

        // キャッシュの削除
        LocalAssetBundleClearExec();
        LocalWebViewCheckClearExec();

        //Sqliteの削除とコピー
        StartCoroutine(SQLiteClient.Instance.LocalSqlite3ClearExec(() =>
        {
            //Sqliteキャッシュダウンロードを行わなくする設定
            //SQLiteClient.Instance.zerotoOneClientVersion();

            UpdateUserID();
            UpdateClientVersion();
            LoadingManager.Instance.RequestLoadingFinish();
            finishChashClear();
        }));
    }

    public void LocalAssetBundleClearExec()
    {
        string path = LocalSaveUtilToInstallFolder.GetSavePathAssetBundle();
        DirectoryInfo di = new DirectoryInfo(path);

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }

        AssetBundlerResponse.clearAssetBundleChash();

        AssetBundlerPlayerPrefs.ResetCache();

        UnitIconImageProvider.Instance.ClearAllCache();
    }

    public void LocalWebViewCheckClearExec()
    {
        LocalSaveUtilToInstallFolder.RemoveWebviewCheck();
        MainMenuWebViewShowChk.ResetCache();
    }

    private void finishChashClear()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mt20q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "mt20q_content");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {

#if BUILD_TYPE_DEBUG
            Debug.Log("CashDelete!!");
#endif
        }));
        newDialog.DisableCancelButton();
        newDialog.EnableFadePanel();
        newDialog.Show();
    }

    public void OnSelectHelp(DialogMenuItem _item)
    {
        uint uid = LocalSaveManager.Instance.LoadFuncUserID();
        string userIdText = string.Format(GameTextUtil.GetText("mt21q_content"), UnityUtil.CreateDrawUserID(uid));
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mt21q_title");
        newDialog.SetDialogText(DialogTextType.MainText, userIdText);
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
            //DG0-1944　お問い合わせは強制的に外部ブラウザーで表示する
            string support_url = MasterDataUtil.GetMasterDataGlobalParamTextFromID(GlobalDefine.WEB_LINK_SUPPORT);
            URLManager.OpenURL(support_url, true);
        }));
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    /////////////////
    // デバッグ系（サーバとSqlite）
    /////////////////

#if API_SELECT_DEBUG
    public DialogMenuItem createMenuItem(string servername)
    {
        string title = ServerDataUtil.GetServerName(servername);
        bool Security = ServerDataUtilSend.GetSecureServer(GlobalDefine.GetServerApiEnv(servername));

        title = title + (Security == true ? " (https)" : " (http)");

        return new DialogMenuItem(servername, title, OnSelectServer);
    }
#endif

    public void OnClickDebug()
    {
#if API_SELECT_DEBUG
        Debug.Log("CALL SceneTitle#OnClickDebu");
        //--------------------------------
        // ページを開いた初回はパネルの位置関係をリセットするよう強制
        //--------------------------------
        Dialog _newDialog = Dialog.Create(DialogType.DialogScrollMenu_YESNO);
        _newDialog.SetDialogText(DialogTextType.Title, "API接続先設定");

        List<DialogMenuItem> menuList = new List<DialogMenuItem>();
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_0_NEW_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_0_IP_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_1_NEW_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_1_IP_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_2a_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_2b_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_2c_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_3a_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_3b_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_3c_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_REVIEW_IP_GOE));
#if DEBUG_EXPORT_BATTLE_LOG
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_LOCAL_GOE));
#else
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_ONLINE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_STAGING_REHEARSAL_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_LOCAL_GOE));
#endif
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_DEVELOP_1_NEW_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_DEVELOP_1_IP_GOE));
        menuList.Add(createMenuItem(GlobalDefine.API_TEST_ADDRESS_DEVELOP_0_GOE));

        _newDialog.SetMeneList(menuList);
        _newDialog.SetDialogText(DialogTextType.YesText, "DBダウンロード");
        _newDialog.SetDialogText(DialogTextType.NoText, "閉じる");
        _newDialog.Show();

        _newDialog.SetYesEvent(() =>
        {
            SelectGetDb();
        }).
        SetNoEvent(() =>
        {
        });

#endif
    }

#if API_SELECT_DEBUG
    private void OnSelectServer(DialogMenuItem item)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        DialogManager.Open2B_Direct("Select API Test Mode", "( " + item.Title + " ) ", "common_button4", "common_button5", true, false).
            SetYesEvent(() =>
            {
                LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.SERVER_API);

                //----------------------------------------
                // ローカルセーブを破棄して再構築
                //----------------------------------------
                LocalSaveManager.LocalSaveRenew(true, true);

                //----------------------------------------
                // FoxSDKの仕様による進行不可回避
                //----------------------------------------
                Debug.Log("FoxSDK Safety");
                LocalSaveManager.Instance.SaveFuncInformationOK(LocalSaveManager.AGREEMENT.FOX_CALLED);

                LocalSaveManager.Instance.SaveFuncServerAddressIP(item.Title);
#if UNITY_EDITOR
                SceneCommon.Instance.ServerName = ServerDataUtil.GetServerName();
#endif
#if BUILD_TYPE_DEBUG
                UpdateClientVersion();
#endif
                //----------------------------------------
                // セーブが消えた状態で諸々再取得しとく
                //----------------------------------------
                LocalSaveManager.Instance.LoadFuncLocalData();
                LocalSaveManager.Instance.LoadFuncRestore();
                LocalSaveManager.Instance.LoadFuncServerAddressIP();
                LocalSaveManager.Instance.LoadFuncUUID();
                LocalSaveManager.Instance.SaveFuncTitleUUID();

                //----------------------------------------
                // メインメニューパラメータクリア
                //----------------------------------------
                MainMenuHeader.ParamReset();
                ResidentParam.ParamResetUserRenew();

                Action patcher = () =>
                {
                    Patcher.Instance.clear();
                    Patcher.Instance.Load(
                        () =>
                        {
                            LocalSaveManagerRN.Instance.PatcherCounter = Patcher.Instance.GetUpdateCounter();
                            LocalSaveManagerRN.Instance.Save();
                            LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.SERVER_API);

                            // Patchでreviewへの切り替え
                            // DVGAN-2130 参照
                            GlobalDefine.ResetPaths();
                            UpdateClientVersion();

                            if (Patcher.Instance.IsCompitableVersion(GlobalDefine.StrVersion()) == false)
                            {
                                ShowVersionUpDialog();
                            }
                        },
                        (error) =>
                        {
                            Debug.LogError("ERROR:" + error);
                            LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.SERVER_API);

                            Uri uri = new Uri(GlobalDefine.GetPatchUrl());
                            string text = StrOpe.i + "パッチファイルをダウンロードできませんでした\n" +
                                            "プランナーさんにパッチファイルのアップロードを\n" +
                                            "お願いしてください。\n\n" +
                                            "URL: " + uri.Scheme + "://" + uri.Host + "\n" +
                                            uri.AbsolutePath + "\n";
                            ;

                            DialogManager.Open1B_Direct("Patch Download Error",
                                                        text,
                                                        "common_button7",
                                                         true, true).
                            SetOkEvent(() =>
                            {

                            });
                        });
                };

                ChangeServerClear(patcher);
            }).
            SetNoEvent();
    }
#endif


#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
    private bool bClickGetDb = false;
#endif
    public void OnClickGetDb()
    {
#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
        DialogManager.Open2B_Direct("Sqlite",
                            "空のクライアントキャッシュDBを\nコピーしますか？\n\n主な用途\n・サーバのMASTERデータの整合性が取れない\n・開発者モードで時間を過去に戻す場合",
                            "common_button4",
                            "common_button5",
                            true, false).
                            SetYesEvent(() =>
                            {
                                LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.SERVER_API);
                                //Sqliteの削除とコピー
                                StartCoroutine(SQLiteClient.Instance.LocalSqlite3ClearExec(() =>
                                {
                                    SQLiteClient.Instance.zerotoOneClientVersion();
                                    LoadingManager.Instance.RequestLoadingFinish();
                                    DialogManager.Open1B_Direct("Sqlite",
                                        "空のクライアントキャッシュDB\nをコピーしました",
                                        "common_button7",
                                        true, true).
                                                SetOkEvent(() =>
                                                {
                                                    UpdateClientVersion();
                                                });
                                }));
                            }).
                            SetNoEvent(() =>
                            {
                            });
#endif
    }

    public void SelectGetDb()
    {
#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
        if (bClickGetDb)
        {
            return;
        }
        bClickGetDb = true;

        DialogManager.Open2B_Direct("Sqliteダウンロード",
                                    ServerDataUtil.GetServerName() + " 向けの\nクライアントキャッシュDBを\nダウンロードしますか？",
                                    "common_button4",
                                    "common_button5",
                                    true, false).
                                    SetYesEvent(() =>
                                    {
                                        startDowloadSqliteDb();
                                        bClickGetDb = false;
                                    }).
                                    SetNoEvent(() =>
                                    {
                                        bClickGetDb = false;
                                    });
#endif
    }

    private IEnumerator DowloadSqliteDb(Action<WWW> action, Action<float> progress = null)
    {
        WaitForSeconds waitseconds = new WaitForSeconds(0.2f);

        yield return waitseconds;

        // ファイルのクローズ
        SQLiteClient.Instance.CloseDb();

        yield return waitseconds;

        // ファイルの削除
        SQLiteClient.Instance.RemoveDbFiles();

        yield return waitseconds;

        // ダウンロード
        string path = LocalSaveUtilToInstallFolder.GetSavePathSqlite() + "/" + SQLiteClient.Instance.GetFilename();

        string url = GlobalDefine.GetSqliteUrl();
#if BUILD_TYPE_DEBUG
        Debug.Log("DowloadSqliteDb GlobalDefine.GetSqliteUrl: " + url);
#endif
        WWW www = new WWW(url);
        //タイムアウト設定あり
        float startTime = Time.time;
        float timeout = 150.0f;
        while (!www.isDone)
        {
            if (timeout > 0 && (Time.time - startTime) >= timeout)
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("DowloadSqliteDb timeout " + url);
#endif
                www.Dispose();
                www = null;
                break;
            }
            else
            {
                if (progress != null)
                {
                    progress(www.progress);
                }
                yield return waitseconds;
            }
        }

        if (progress != null)
        {
            progress(1.0f);
        }

        bool isSucess = (www != null && string.IsNullOrEmpty(www.error));

        if (isSucess == true)
        {
            File.WriteAllBytes(path, www.bytes);
        }
        else
        {
            string errortext = www == null ? "www == null timeout" : www.error;
            Debug.Log("Error: " + errortext);
        }

        yield return waitseconds;

        // DBオープン
        SQLiteClient.Instance.OpenDB();

        if (isSucess == false)
        {
            SQLiteClient.Instance.zerotoOneClientVersion();
        }

        yield return waitseconds;

        action(www);

        yield return waitseconds;
    }

#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
    private void startDowloadSqliteDb()
    {
        LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.SERVER_API);

        StartCoroutine(DowloadSqliteDb((WWW www) =>
        {
            if (www != null && string.IsNullOrEmpty(www.error))
            {
                // ダウンロードが正常に完了した
                DialogManager.Open1B_Direct("Sqlite",
                                            ServerDataUtil.GetServerName() + "\nDBをダウンロードしました",
                                            "common_button7",
                                            true, true).
                SetOkEvent(() =>
                {
                });
            }
            else
            {
                // ダウンロードでエラーが発生した
                DowloadSqliteDbError(www);
            }

            UpdateClientVersion();
            LoadingManager.Instance.RequestLoadingFinish();
        }));
    }

    private void DowloadSqliteDbError(WWW www, Action sendevent = null)
    {
        string errortext = www == null ? "www == null timeout" : www.error;
        Uri uri = new Uri(www.url);
        string text = StrOpe.i + "SqliteDBがダウンロードできませんでした\n" +
                                "正しく生成が行えていないようです\n" +
                                "クライアントプログラマーに報告してください\n" +
                                "アップロード対応orコピー対応サーバの場合は\n" +
                                "プランナーさんに報告してください\n" +
                                "確認時はこの画面を見せてください\n" +
                                "\n" +
                                errortext + "\n\n" +
                                ServerDataUtil.GetServerName() + "\n" +
                                "アプリバージョン: " + GlobalDefine.StrVersion() + "\n" +
                                "URL: " + uri.Scheme + "://" + uri.Host;
        foreach (string s in uri.Segments)
        {
            text += "\n" + s;
        }

        DialogManager.Open1B_Direct("Sqlite Download Error",
                    text,
                    "common_button7",
                    true, true).
        SetOkEvent(() =>
        {
            if (sendevent != null)
            {
                sendevent();
            }
        });
    }

    private void ChangeServerClear(System.Action finishAction = null)
    {
        // キャッシュの削除
        LocalAssetBundleClearExec();

        //Sqliteの削除とコピー
        StartCoroutine(SQLiteClient.Instance.LocalSqlite3ClearExec(() =>
        {
            UpdateUserID();
            UpdateClientVersion();

            if (finishAction != null)
            {
                finishAction();
            }
        }));
    }
#endif

    public void OnClickBrokenDb()
    {
#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
        DialogManager.Open2B_Direct("Sqlite",
                            "キャッシュDBを破壊しますか？\n\n主な用途\n・タイトルログイン時の修復\n・MENUのデータの整合性チェック用",
                            "common_button4",
                            "common_button5",
                            true, false).
                            SetYesEvent(() =>
                            {
                                try
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        SQLiteClient.Instance.CloseDb();

                                        string path = SQLiteClient.Instance.GetDbPath();
                                        using (BinaryWriter w = new BinaryWriter(File.OpenWrite(path)))
                                        {
                                            w.Seek(1024, SeekOrigin.Begin);
                                            w.Write(new byte[] { (byte)0x01, (byte)0x23, (byte)0x45, (byte)0x67, });
                                            w.Write((int)123456789);
                                            w.Write((float)3.14159);
                                        }

                                        SQLiteClient.Instance.OpenDB();
                                    }
                                }
                                catch (Exception e)
                                {
                                    Debug.Log("e= " + e.StackTrace);
                                }
                            }).
                            SetNoEvent(() =>
                            {
                            });
#endif
    }

    public void OnClickBrokenUUID()
    {
#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
        DialogManager.Open2B_Direct("UUID",
                            "UUIDを変更しますか？\n\n用途\n・UUIDが書き換わったケースの確認",
                            "common_button4",
                            "common_button5",
                            true, false).
                            SetYesEvent(() =>
                            {
#if false
                                //Odinテスト用コード
                                int zero = 0;
                                int n = 1000 / zero;
                                Debug.Log("n= " + n);
#endif
                                LocalSaveManager.Instance.SaveFuncUUID(Guid.NewGuid().ToString());
                                UpdateClientVersion();
                            }).
                            SetNoEvent(() =>
                            {
                            });
#endif
    }

    public void OnClickChangeMask()
    {
#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
        if (SafeAreaControl.HasInstance == false)
        {
            return;
        }
        bool ismask = SafeAreaControl.Instance.safe_area_mask;

        string message = string.Format("iPhone10確認用\n\n用途\n・マスク有無の状態変更\n\n現在:{0}", ismask ? "あり" : "なし");
        DialogManager.Open2B_Direct("上下のMask表示状態変更",
                            message,
                            "common_button4",
                            "common_button5",
                            true, false).
                            SetYesEvent(() =>
                            {
                                SafeAreaControl.Instance.setMask(!ismask);
                            }).
                            SetNoEvent(() =>
                            {
                            });

#endif
    }
    /////////////////
    //Title FMS
    /////////////////

    void OnReady()
    {
        TitleFSM.Instance.SendFsmNextEvent();
    }


    void OnShouldCopySQLite()
    {
        TitleFSM.Instance.SendFsmPositiveEvent();
    }

    void OnCopySQLite()
    {
        TitleFSM.Instance.SendFsmNextEvent();
    }

    void OnFadeWait()
    {
        //フェードイン
        GetComponentInChildren<CanvasGroup>().DOFade(1.0f, 0.4f).SetDelay(0.1f)
        .OnComplete(() =>
        {
            TitleFSM.Instance.SendFsmNextEvent();
        });
    }

    public void OnPlayGameSigninCheck()
    {
        google_signin_chk = true;
        TitleFSM.Instance.SendFsmNextEvent();
    }

    void OnChangeNextVersion()
    {
#if BUILD_TYPE_DEBUG
        if (nextVersionToggle != null)
        {
            bool isON = nextVersionToggle.GetComponent<Toggle>().isOn;
            if (DebugOption.Instance.noneNextVersion == isON)
            {
                return;
            }

            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.SERVER_API);

            DebugOption.Instance.noneNextVersion = isON;

            Patcher.Instance.Load(
            () =>
            {
                LocalSaveManagerRN.Instance.PatcherCounter = Patcher.Instance.GetUpdateCounter();
                LocalSaveManagerRN.Instance.Save();

                LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.SERVER_API);

                GlobalDefine.ResetPaths();
                UpdateClientVersion();
            },
            (error) =>
            {
                Uri uri = new Uri(GlobalDefine.GetPatchUrl());
                string servername = ServerDataUtil.GetServerName(LocalSaveManager.Instance.LoadFuncServerAddressIP());

                Debug.LogError("ERROR:" + error);
                string text = StrOpe.i + "パッチファイルの取得に失敗しました。\n\n" +
                            "サーバ:" + servername + "\n" +
                            "URL: " + uri.Scheme + "://" + uri.Host + "\n" +
                            uri.AbsolutePath + "\n" +
                            "アプリバージョン: " + GlobalDefine.StrVersion() + "\n";

                DialogManager.Open1B_Direct("Patch NG!!!!",
                                            text,
                                            "common_button7", true, true).
                SetOkEvent(() =>
                {

                });

                LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.SERVER_API);
            });
        }

#endif
    }

    void OnPatchUpdateRequest()
    {
        Patcher.Instance.Load(
            () =>
            {
                LocalSaveManagerRN.Instance.PatcherCounter = Patcher.Instance.GetUpdateCounter();
                LocalSaveManagerRN.Instance.Save();

                // Patchでreviewへの切り替え
                // DVGAN-2130 参照
                if (Patcher.Instance.isNextVersion())
                {
                    GlobalDefine.ResetPaths();
                    UpdateClientVersion();
                }

                TitleFSM.Instance.SendFsmEvent_Success();
            },
            (error) =>
            {
                Debug.LogError("ERROR:" + error);
                TitleFSM.Instance.SendFsmEvent_FailQuit();

                /*
                DivRNUtil.ShowRetryDialog(
                    () =>
                    {
                        TitleFSM.Instance.SendFsmEvent_FailRetry();
                    });
                */
            });
    }

    void OnQuitApplication()
    {
        DivRNUtil.ShowQuitApplicationDialog();
    }

    //DO REPEATH
    void OnWaitForPatchUpdateFinish()
    {
        //-------------------------
        // PlayGameServicesオートログイン処理を呼ぶ.
        // 実際に呼ぶかの判定は関数内で行う.
        //-------------------------
        PlayGameServiceUtil.AutoSignIn();

        LocalSaveManager.Instance.DiffTitleUUID("SceneTitle/OnWaitForPatchUpdateFinish");

        TitleFSM.Instance.SendFsmNextEvent();
    }

    void OnNgForPatchUpdateFinish()
    {
        //-------------------------
        // PlayGameServicesオートログイン処理を呼ぶ.
        // 実際に呼ぶかの判定は関数内で行う.
        //-------------------------
        PlayGameServiceUtil.AutoSignIn();

        LocalSaveManager.Instance.DiffTitleUUID("SceneTitle/OnWaitForPatchUpdateFinish");

        TitleFSM.Instance.SendFsmNextEvent();
    }

    void OnShouldShowVersionUpDialog()
    {
        if (Patcher.Instance.IsCompitableVersion(GlobalDefine.StrVersion()) == false)
        {
            TitleFSM.Instance.SendFsmPositiveEvent();
            return;
        }

        TitleFSM.Instance.SendFsmNegativeEvent();
    }

    void OnShowVersionUpDialog()
    {
        ShowVersionUpDialog(() =>
        {
            TitleFSM.Instance.SendFsmNextEvent();
        });
    }

    void ShowVersionUpDialog(Action nextAction = null)
    {
        Dialog dialog = DialogManager.Open1B("ERROR_VERSION_UP_TITLE", "ERROR_VERSION_UP", "common_button7", true, true);
        dialog.SetDialogEvent(DialogButtonEventType.OK, () =>
        {
#if UNITY_IPHONE
            Application.OpenURL(GlobalDefine.WEB_LINK_APPSTORE);
#elif UNITY_ANDROID
            Application.OpenURL(GlobalDefine.WEB_LINK_PLAYSTORE);
#else
            Application.OpenURL( GlobalDefine.WEB_LINK_PLAYSTORE );
#endif
        });

#if BUILD_TYPE_DEBUG
        dialog.SetDialogEvent(DialogButtonEventType.CANCEL, () =>
        {
            dialog.Hide();

            string servername = ServerDataUtil.GetServerName(LocalSaveManager.Instance.LoadFuncServerAddressIP());

            Uri uri = new Uri(GlobalDefine.GetPatchUrl());

            string versions = "";
            List<string> list = Patcher.Instance.GetAppVersionMulti();
            for (int i = 0; i < list.Count; i++)
            {
                versions += "\n" + list[i];
            }

            string text = StrOpe.i + "パッチに設定してある動作バージョンを無視します。\n" +
                                        "動作に問題が発生する場合があります。\n\n" +
                                        "正常動作することが確認されている場合は\n" +
                                        "プランナーさんに画面か画面キャプチャー\n" +
                                        "見せてを対応をお願いしてください。\n\n" +
                                        "サーバ:" + servername + "\n" +
                                        "URL: " + uri.Scheme + "://" + uri.Host + "\n" +
                                        uri.AbsolutePath + "\n" +
                                        "アプリバージョン: " + GlobalDefine.StrVersion() + "\n" +
                                        "APP_VERSION_PATCH_MULTI: " + versions;

            DialogManager.Open1B_Direct("Skip Patch Version",
                                        text,
                                        "common_button7", true, true).
            SetOkEvent(() =>
            {
                if (nextAction != null)
                {
                    nextAction();
                }
            });
        })
        .EnableCancel();
#endif
    }

    void OnExitInitialState()
    {
        UpdateUserID();

        m_Copyright.text = GameTextUtil.GetText("pp13p_display2");

        //BGM
        SoundUtil.PlayBGM(BGMManager.EBGM_ID.eBGM_1_1, false);
        SoundUtil.PlaySE(SEID.SE_TITLE_CALL_W);

        if (LocalSaveManager.Instance.LoadFuncInformationVer() != "")
        {
            m_firstAgreement = false;
        }

        if (LocalSaveManager.Instance.LoadFuncInformationPolicyVer() != "")
        {
            m_firstPolicy = false;
        }

        //----------------------------------------
        // ローカル通知予約を破棄
        //----------------------------------------
        LocalNotificationUtil.CancelNotification();

        TitleFSM.Instance.SendFsmNextEvent();
    }

    void OnSqliteCheck()
    {
        if (SQLiteClient.Instance.BrokenDb())
        {
            SelectBrokenDbCheckNGDialog(() =>
            {
                //アプリ終了
                Application.Quit();
            });
        }
        else
        {
            TitleFSM.Instance.SendFsmNextEvent();
        }
    }

    void PatchCheckTapToStart(Action action)
    {
        if (Patcher.Instance.IsLoaded)
        {
            action();
        }
        else
        {
            Patcher.Instance.Load(
                () =>
                {
                    LocalSaveManagerRN.Instance.PatcherCounter = Patcher.Instance.GetUpdateCounter();
                    LocalSaveManagerRN.Instance.Save();

                    // Patchでreviewへの切り替え
                    // DVGAN-2130 参照
                    if (Patcher.Instance.isNextVersion())
                    {
                        GlobalDefine.ResetPaths();
                        UpdateClientVersion();
                    }

                    action();
                },
                (error) =>
                {
                    Debug.LogError("ERROR:" + error);

                    DivRNUtil.ShowRetryDialog(
                        () =>
                        {
                        });
                });
        }
    }

    void OnClickScreen()
    {
        PatchCheckTapToStart(() =>
        {

#if BUILD_TYPE_DEBUG
            Debug.Log("CALL SceneTitle#OnClickScreen");
#endif

#if BUILD_TYPE_DEBUG
            if (tutorialToggle != null)
            {
                DebugOption.Instance.tutorialDO.skip = tutorialToggle.GetComponent<Toggle>().isOn;
            }

            if (nextVersionToggle != null)
            {
                DebugOption.Instance.noneNextVersion = nextVersionToggle.GetComponent<Toggle>().isOn;
            }
#endif

            AndroidBackKeyManager.Instance.StackPop(gameObject);

            TitleFSM.Instance.SendFsmEvent("CLICK_SCREEN");
        });
    }

    void OnShouldShowAgreementDialog()
    {
        int nAgreementOK = LocalSaveManager.Instance.LoadFuncInformationOK();
        if (nAgreementOK == (int)LocalSaveManager.AGREEMENT.AGREE_OK)
        {
            TitleFSM.Instance.SendFsmNegativeEvent();
            return;
        }
        TitleFSM.Instance.SendFsmPositiveEvent();
    }

    void OnShowAgreementDialog()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "pp6q_title");
        if (m_firstAgreement == true)
        {
            // 初回時
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "pp6q_content");
            Debug.LogError("Agreement first");
        }
        else
        {
            // バージョン更新時
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "pp6q_content2");
            Debug.LogError("Agreement ver update");
        }
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button2");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button3");
        newDialog.SetMenuInButton(true);
        newDialog.SetDialogTextFromTextkey(DialogTextType.InButtonText, "pp6q_button");
        newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
            //----------------------------------------
            // 利用規約に同意したことをセーブ
            //----------------------------------------
            LocalSaveManager.Instance.SaveFuncInformationOK(LocalSaveManager.AGREEMENT.AGREE_OK);
            newDialog.Hide();
            TitleFSM.Instance.SendFsmPositiveEvent();
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
        {
            newDialog.Hide();
            TitleFSM.Instance.SendFsmNegativeEvent();
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.INBUTTON, new System.Action(() =>
        {
            string url = Patcher.Instance.GetAgreementUrl();
            URLManager.OpenURL(url);
        }));
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.DisableAutoHide();
        newDialog.Show();
    }

    void OnShowConfirmReadAgreementDialog()
    {
        //TODO
        TitleFSM.Instance.SendFsmPositiveEvent();
    }

    void OnShowAgreementNeedPositiveDialog()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "pp7q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "pp7q_content");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button6");
        newDialog.DisableCancelButton();
        newDialog.EnableFadePanel();
        newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
        {
            TitleFSM.Instance.SendFsmNextEvent();
        });
        newDialog.Show();
    }

    void OnShouldShowPrivacyPolicyDialog()
    {
        int nAgreementOK = LocalSaveManager.Instance.LoadFuncInformationPolicyOK();
        if (nAgreementOK == (int)LocalSaveManager.AGREEMENT.AGREE_OK)
        {
            TitleFSM.Instance.SendFsmNegativeEvent();
            return;
        }
        TitleFSM.Instance.SendFsmPositiveEvent();
    }

    void OnShowPrivacyPolicyDialog()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "pp8q_title");
        if (m_firstPolicy == true)
        {
            // 初回時
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "pp8q_content");
            Debug.LogError("Policy first");
        }
        else
        {
            // バージョン更新時
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "pp8q_content2");
            Debug.LogError("Policy ver update");
        }
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button2");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button3");
        newDialog.SetMenuInButton(true);
        newDialog.SetDialogTextFromTextkey(DialogTextType.InButtonText, "pp8q_button");
        newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
            //----------------------------------------
            // プライバシーポリシーに同意したことをセーブ
            //----------------------------------------
            LocalSaveManager.Instance.SaveFuncInformationPolicyOK(LocalSaveManager.AGREEMENT.AGREE_OK);
            newDialog.Hide();
            TitleFSM.Instance.SendFsmPositiveEvent();
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
        {
            newDialog.Hide();
            TitleFSM.Instance.SendFsmNegativeEvent();
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.INBUTTON, new System.Action(() =>
        {

            string url = Patcher.Instance.GetPolicyUrl();
            URLManager.OpenURL(url);
        }));
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.DisableAutoHide();
        newDialog.Show();
    }

    void OnShowConfirmReadPrivacyPolicyDialog()
    {
        //TODO
        TitleFSM.Instance.SendFsmPositiveEvent();
    }

    void OnShowPrivacyPolicyNeedPositiveDialog()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "pp9q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "pp9q_content");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button6");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
        {
            TitleFSM.Instance.SendFsmNextEvent();
        });
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    public void OnMoviPlayCheck()
    {
        if (LocalSaveManager.Instance.LoadFuncMovieFirst() == false)
        {

            // 初回起動時のみ再生
            GameObject _obj = Resources.Load("Prefab/MoviePrefab") as GameObject;
            if (_obj != null)
            {
                m_MoviePrefab = Instantiate(_obj) as GameObject;
                MovieManager.Instance.play(OPENING_MOVIE_FILE_NAME, false, true, true, false);
            }
        }
        else
        {
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.TO_HOME);
        }

        TitleFSM.Instance.SendFsmNextEvent();
    }

    void OnShouldUserCreate()
    {
        if (LocalSaveManager.Instance.CheckUUID())
        {
            TitleFSM.Instance.SendFsmNegativeEvent();
            return;
        }

        TitleFSM.Instance.SendFsmPositiveEvent();
    }

    private void authError(API_CODE _code, uint uniqueNo)
    {
        Debug.LogError("CALL authError:" + _code.ToString() + " UNIQ:" + uniqueNo + " STATE:" + TitleFSM.Instance.ActiveStateName);
    }

    private void CreateUserSerialProcess(Action<string> uuidsucsess = null,
                            Action<ServerApi.ResultData> datasucsess = null)
    {
        string m_ServerUUID = "";
        ServerApi.ResultData data = null;

        new SerialProcess()
            .Add(next =>
            {
                ServerDataUtilSend.SendPacketAPI_UserCreate(ref m_ServerUUID).
                    setSuccessAction(_data =>
                    {
                        data = _data;
                        next();
                    }).
                    setErrorAction(_data =>
                    {
                        LoadingManager.Instance.RequestLoadingFinish();
                        authError(_data.m_PacketCode, _data.m_PacketUniqueNum);
                    }).
                    SendStart();
            })
            .Add(next =>
            {
                Debug.Assert(data != null, "ServerDataUtilSend.SendPacketAPI_UserCreate() failed.");

                if (uuidsucsess != null)
                {
                    uuidsucsess(m_ServerUUID);
                }

                if (datasucsess != null)
                {
                    datasucsess(data);
                }
            })
            .Flush();
    }

    private void UserAuthenticationSerialProcess(string uuid,
                                                 Action<ServerApi.ResultData> datasucsess = null)
    {
        ServerApi.ResultData data = null;

        new SerialProcess()
            .Add(next =>
            {
                ServerDataUtilSend.SendPacketAPI_UserAuthentication(uuid).
                setSuccessAction(_data =>
                {
                    data = _data;
                    next();
                }).
                setErrorAction(_data =>
                {
                    LoadingManager.Instance.RequestLoadingFinish();
                    authError(_data.m_PacketCode, _data.m_PacketUniqueNum);
                }).
                SendStart();
            })
            .Add(next =>
            {
                Debug.Assert(data != null, "ServerDataUtilSend.SendPacketAPI_UserCreate() failed.");

                if (datasucsess != null)
                {
                    datasucsess(data);
                }
            })
            .Flush();
    }

    void OnUserCreate()
    {
        CreateUserSerialProcess((string uuid) =>
        {
            //----------------------------------------
            // ユーザー生成が成立
            // →UUIDを保持してローカル情報を構築する
            //----------------------------------------
            LocalSaveManager.Instance.SaveFuncUUID(uuid);

        },
        (ServerApi.ResultData data) =>
        {
            m_semaphore.Lock(() =>
            {
                TitleFSM.Instance.SendFsmNextEvent();
            });
            new System.Threading.Thread(() =>
            {
                try
                {
                    // UserDataAdminの構築が重すぎてメインスレッドが止まる時があるので非同期で処理
                    UserDataAdmin.Instance.m_StructPlayer = data.UpdateStructPlayer<RecvCreateUser>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                    UserDataAdmin.Instance.m_StructSystem = data.GetResult<RecvCreateUser>().result.system;
                    UserDataAdmin.Instance.m_StructHeroList = data.GetResult<RecvCreateUser>().result.hero_list;
                    UserDataAdmin.Instance.ConvertPartyAssing();
                }
                catch (Exception e)
                {
                    Debug.LogError("ERROR:" + e.Message);
                }
                finally
                {
                    m_semaphore.Unlock();
                }
            }).Start();
        });
    }

    void OnUserAuthentication()
    {
        UserAuthenticationSerialProcess(null,
        (ServerApi.ResultData data) =>
        {
            LocalSaveManager.Instance.DiffTitleUUID("SceneTitle/OnUserAuthentication");

            // UnitGridParamの生成中にPlayerPrefasを使っているのでメインスレッドでキャッシュを作っておく
            LocalSaveManager.Instance.LoadFuncAddFavoriteUnit();

            m_semaphore.Lock(() =>
            {
                SaveUserAuthenticationInfos();
            });
            new System.Threading.Thread(() =>
            {
                try
                {
                    // UserDataAdminの構築が重すぎてメインスレッドが止まる時があるので非同期で処理
                    UserDataAdmin.Instance.m_StructPlayer = data.UpdateStructPlayer<RecvUserAuthentication>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                    UserDataAdmin.Instance.m_StructHeroList = data.GetResult<RecvUserAuthentication>().result.hero_list;
                    UserDataAdmin.Instance.m_StructSystem = data.GetResult<RecvUserAuthentication>().result.system;
                    UserDataAdmin.Instance.m_StructQuest = data.GetResult<RecvUserAuthentication>().result.quest;
                    UserDataAdmin.Instance.ConvertPartyAssing();
                }
                catch (Exception e)
                {
                    Debug.LogError("ERROR:" + e.Message);
                }
                finally
                {
                    m_semaphore.Unlock();
                }
            }).Start();
        });
    }

    //----------------------------------------
    // ユーザー認証が成立
    // →ローカル情報を構築する
    //----------------------------------------
    private void SaveUserAuthenticationInfos()
    {
        // @add Developer 2015/12/02 v310 ローカル通知用に保存(中断復帰時にも対応するため、初めにサーバから受け取るタイミングで保存)
        LocalNotificationUtil.m_StaminaNow = UserDataAdmin.Instance.m_StructPlayer.stamina_now;

        //----------------------------------------
        // サーバー時間を設定
        //----------------------------------------
        if (TimeManager.Instance != null)
        {
            if (UserDataAdmin.Instance != null && UserDataAdmin.Instance.m_StructSystem != null)
            {
                TimeManager.Instance.SetupServerTime(UserDataAdmin.Instance.m_StructSystem.server_time);

                //--------------------------------
                // サーバー時間の検出
                //--------------------------------
                DateTime cServerTime = TimeUtil.ConvertServerTimeToLocalTime(UserDataAdmin.Instance.m_StructSystem.server_time);
#if BUILD_TYPE_DEBUG
                Debug.Log(cServerTime.ToString("cServerTime:yyyy-MM-dd,HH:mm:ss") + " ... " + UserDataAdmin.Instance.m_StructSystem.server_time);
#endif
            }
        }

        //----------------------------------------
        // ユーザーIDを記録。
        // 今後の認証時エラー等に使用
        //----------------------------------------
        LocalSaveManager.Instance.SaveFuncUserID(UserDataAdmin.Instance.m_StructPlayer.user.user_id);
        LocalSaveManager.Instance.SaveFuncTitleUUID();

        TitleFSM.Instance.SendFsmPositiveEvent();

        //チュートリアルステップをチェック
        TutorialManager.CheckTutorialStep();
    }

    private PacketStructQuest2Build m_RestoreQuest2Build = null;    //!< 中断復帰時のサーバー側保持クエスト構築情報
    private uint m_RestoreQuestMissionID = 0;                       //!< 復帰するクエストのID
    private SceneGoesParamToQuest2Restore m_Quest2Restore = null;   //!< 中断復帰情報
    private SceneGoesParamToQuest2 m_Quest2Start = null;            //!< クエスト開始情報

    void OnShouldShowRestoreCheckDialog()
    {
        m_RestoreQuest2Build = null;
        m_Quest2Restore = null;
        m_Quest2Start = null;
        m_RestoreQuestMissionID = 0;

        //----------------------------------------
        // クエストクリア情報のバージョン不一致で破棄
        //----------------------------------------
        if (LocalSaveManager.Instance.LoadFuncResultVersionCheck() == false)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("Quest Result Version NG!");
#endif
            LocalSaveManager.Instance.SaveFuncGoesToMenuResult(null);
            LocalSaveManager.Instance.SaveFuncResultVersion();
        }

        //保存情報取得
        m_Quest2Restore = LocalSaveManager.Instance.LoadFuncGoesToQuest2Restore();
        if (m_Quest2Restore == null)
        {
            m_Quest2Start = LocalSaveManager.Instance.LoadFuncGoesToQuest2Start();
        }

        //クエストID取得
        if (m_Quest2Restore != null)
        {
            m_RestoreQuestMissionID = m_Quest2Restore.m_QuestMissionID;
        }
        if (m_Quest2Start != null)
        {
            m_RestoreQuestMissionID = m_Quest2Start.m_QuestMissionID;
        }

        //クエストIDチェック
        if (m_RestoreQuestMissionID != 0)
        {
            TitleFSM.Instance.SendFsmPositiveEvent();

            return;
        }

        TitleFSM.Instance.SendFsmNegativeEvent();
    }


    void OnCheckRestoreCheckId()
    {
        switch (MasterDataUtil.GetQuestType(m_RestoreQuestMissionID))
        {
            case MasterDataDefineLabel.QuestType.NORMAL:
                {
                    ServerDataUtilSend.SendPacketAPI_Quest2OrderGet(0)
                        .setSuccessAction((data) =>
                        {
                            //----------------------------------------
                            // 通信完了したので結果を反映
                            //----------------------------------------
                            successRestoreCheck(data);
                        })
                        .setErrorAction(_data =>
                        {
                            //----------------------------------------
                            // この時点でクエスト受諾情報がサーバーから返ってきていない場合、
                            // そもそもクエスト受諾が成立していないのでダイアログも出さずにスルー
                            //----------------------------------------
                            errorRestoreCheck(_data);
                        })
                        .SendStart();
                }
                break;
            case MasterDataDefineLabel.QuestType.CHALLENGE:
                {
                    ServerDataUtilSend.SendPacketAPI_ChallengeQuestOrderGet(0)
                        .setSuccessAction((data) =>
                        {
                            //----------------------------------------
                            // 通信完了したので結果を反映
                            //----------------------------------------
                            successRestoreCheck(data);
                        })
                        .setErrorAction(_data =>
                        {
                            //----------------------------------------
                            // この時点でクエスト受諾情報がサーバーから返ってきていない場合、
                            // そもそもクエスト受諾が成立していないのでダイアログも出さずにスルー
                            //----------------------------------------
                            errorRestoreCheck(_data);
                        })
                        .SendStart();
                }
                break;
            default:
                //
                TitleFSM.Instance.SendFsmNegativeEvent();
                break;
        }
    }

    private void successRestoreCheck(ServerApi.ResultData data)
    {
        RecvQuest2OrderGet serverOrderQuest = data.GetResult<RecvQuest2OrderGet>();
        //----------------------------------------
        // この時点でクエスト受諾情報がサーバーから返ってきていない場合、
        // そもそもクエスト受諾が成立していないのでダイアログも出さずにスルー
        //----------------------------------------
        if (serverOrderQuest.result.quest_id == 0)
        {
            TitleFSM.Instance.SendFsmNegativeEvent();
            return;
        }

        //----------------------------------------
        // 中断復帰情報を作成した時と今で、
        // セーブデータに互換性がなかったり引き継ぎたくない場合はバージョン変えて中断復帰情報を破棄する
        //----------------------------------------
        bool bLocalRestoreSaveVersionCheck = LocalSaveManager.Instance.LoadFuncRestoreVersionCheck();
        if (bLocalRestoreSaveVersionCheck == false)
        {
            TitleFSM.Instance.SendFsmNegativeEvent();
            return;
        }
        TitleFSM.Instance.SendFsmPositiveEvent();
    }

    private void errorRestoreCheck(ServerApi.ResultData data)
    {
        TitleFSM.Instance.SendFsmNegativeEvent();
        Debug.LogError("PacketResult Code Error! - " + data.m_PacketCode);
    }

    void OnShowRestoreCheckDialog()
    {
        bool is_auto_play = false;
        if (LocalSaveManager.Instance.m_SaveGoesDataToQuest2Restore != null)
        {
            is_auto_play = LocalSaveManager.Instance.m_SaveGoesDataToQuest2Restore.m_IsUsedAutoPlay;
        }


        Dialog newDialog = null;
        newDialog = DialogManager.Open2B("BATTLE_RETURN_TITLE",
                                         "BATTLE_RETURN",
                                        "common_button4", "common_button5",
                                         true, false).
            SetDialogEvent(DialogButtonEventType.YES, () =>
            {

                // 復帰した場合
                newDialog.Hide();
                // 中断復帰データのキャッシュを破棄
                LocalSaveManager.Instance.m_SaveGoesDataToQuest2Restore = null;
                TitleFSM.Instance.SendFsmPositiveEvent();
            }).
            SetDialogEvent(DialogButtonEventType.NO, () =>
            {

                // 復帰しなかった場合
                SendCessaionQuest(newDialog, is_auto_play);
            });
        newDialog.DisableCancelButton();
    }

    private void SendCessaionQuest(Dialog dialog, bool is_auto_play)
    {
        switch (MasterDataUtil.GetQuestType(m_RestoreQuestMissionID))
        {
            case MasterDataDefineLabel.QuestType.NORMAL:
                {
                    ServerDataUtilSend.SendPacketAPI_Quest2CessaionQuest(is_auto_play).
                    setSuccessAction((data) =>
                    {
                        dialog.Hide();
                        TitleFSM.Instance.SendFsmNegativeEvent();
                    }).
                    setErrorAction(_data =>
                    {
                        dialog.Hide();
                        TitleFSM.Instance.SendFsmNegativeEvent();
                    }).
                    SendStart();
                }
                break;
            case MasterDataDefineLabel.QuestType.CHALLENGE:
                {
                    ServerDataUtilSend.SendPacketAPI_ChallengeQuestCessaionQuest(is_auto_play).
                    setSuccessAction((data) =>
                    {
                        dialog.Hide();
                        TitleFSM.Instance.SendFsmNegativeEvent();
                    }).
                    setErrorAction(_data =>
                    {
                        dialog.Hide();
                        TitleFSM.Instance.SendFsmNegativeEvent();
                    }).
                    SendStart();
                }
                break;
            default:
                dialog.Hide();
                TitleFSM.Instance.SendFsmNegativeEvent();
                break;
        }
    }

    void OnExitRestore()
    {
        LocalSaveManager.Instance.SaveFuncGoesToQuest2Restore(null);
        LocalSaveManager.Instance.SaveFuncGoesToQuest2Start(null);
        TitleFSM.Instance.SendFsmNextEvent();
    }

    void OnRestoreFailed()
    {
        Debug.LogError("CALL OnRestoreFailed");
    }

    void OnRestoreFinish()
    {
        SceneCommon.Instance.LoadResidentResource(() =>
        {
            SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore = null;
            SceneGoesParam.Instance.m_SceneGoesParamToQuest2 = null;

            if (m_Quest2Restore != null)
            {
                SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore = m_Quest2Restore;
            }
            else if (m_Quest2Start != null)
            {
                SceneGoesParam.Instance.m_SceneGoesParamToQuest2 = m_Quest2Start;
            }


            //----------------------------------------
            // サーバーで求めたクエスト開始情報を反映
            //----------------------------------------
            {
                SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build = new SceneGoesParamToQuest2Build();
                SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild = m_RestoreQuest2Build;
            }

            SceneCommon.Instance.ChangeScene(SceneType.SceneQuest2, false);
        });
    }

    void OnRestore()
    {
        switch (MasterDataUtil.GetQuestType(m_RestoreQuestMissionID))
        {
            case MasterDataDefineLabel.QuestType.NORMAL:
                {
                    ServerDataUtilSend.SendPacketAPI_Quest2OrderGet(1).
                        setSuccessAction((data) =>
                        {
                            //----------------------------------------
                            // 通信完了したので結果を反映
                            //----------------------------------------
                            successRestore(data);
                        }).
                        setErrorAction(_data =>
                        {
                            //----------------------------------------
                            // この時点でクエスト受諾情報がサーバーから返ってきていない場合、
                            // そもそもクエスト受諾が成立していないのでダイアログも出さずにスルー
                            //----------------------------------------
                            errorRestore(_data);
                        }).
                        SendStart();
                }
                break;
            case MasterDataDefineLabel.QuestType.CHALLENGE:
                {
                    ServerDataUtilSend.SendPacketAPI_ChallengeQuestOrderGet(1).
                        setSuccessAction((data) =>
                        {
                            //----------------------------------------
                            // 通信完了したので結果を反映
                            //----------------------------------------
                            successRestore(data);
                        }).
                        setErrorAction(_data =>
                        {
                            //----------------------------------------
                            // この時点でクエスト受諾情報がサーバーから返ってきていない場合、
                            // そもそもクエスト受諾が成立していないのでダイアログも出さずにスルー
                            //----------------------------------------
                            errorRestore(_data);
                        }).
                        SendStart();
                }
                break;
            default:
                TitleFSM.Instance.SendFsmEvent_DoError();
                break;

        }
    }

    private void successRestore(ServerApi.ResultData data)
    {
        RecvQuest2OrderGet serverOrderQuestDetail = data.GetResult<RecvQuest2OrderGet>();
        //----------------------------------------
        // この時点でクエスト受諾情報がサーバーから返ってきていない場合、
        // そもそもクエスト受諾が成立していないのでダイアログも出さずにスルー
        //----------------------------------------
        if (serverOrderQuestDetail.result.quest_id == 0)
        {
            //                          m_RestoreSeq = RESTORE_SEQ.SEQ_CHK_DIALOG_ERROR;
            TitleFSM.Instance.SendFsmEvent_DoError();
            return;
        }

        //----------------------------------------
        // 取得した情報とクライアント側の情報が整合が取れているか確認
        //----------------------------------------
        uint nQuestContinue = 0;
        uint nQuestReset = 0;
        {
            bool bQuestOrderOK = false;
            if (serverOrderQuestDetail.result.quest != null)
            {
                if (m_Quest2Restore != null && m_Quest2Restore.m_QuestMissionID == serverOrderQuestDetail.result.quest_id)
                {
#if BUILD_TYPE_DEBUG
                    Debug.Log("Quest2 Restore - " + m_Quest2Restore.m_QuestMissionID + " , " + serverOrderQuestDetail.result.quest_id);
#endif
                    bQuestOrderOK = true;
                    nQuestContinue = serverOrderQuestDetail.result.continue_ct;
                    nQuestReset = serverOrderQuestDetail.result.reset_ct;
                }
                else if (m_Quest2Start != null && m_Quest2Start.m_QuestMissionID == serverOrderQuestDetail.result.quest_id)
                {
#if BUILD_TYPE_DEBUG
                    Debug.Log("Quest2 Restore Start - " + m_Quest2Start.m_QuestMissionID + " , " + serverOrderQuestDetail.result.quest_id);
#endif
                    bQuestOrderOK = true;
                }
            }

            //----------------------------------------
            // クエスト受諾情報がそのまま復元できない場合
            //----------------------------------------
            if (bQuestOrderOK == false)
            {
                {
                    //----------------------------------------
                    // 通常クエストのクエスト構成情報が無い場合、
                    // ユーザーに中断復帰の同意もらった後なので、ダイアログを出して謝る
                    //
                    // 勝手にクエスト受けなおすわけにもいかないので、復元を認めずエラー処理へ移行
                    //----------------------------------------
                    //                                  m_RestoreSeq = RESTORE_SEQ.SEQ_CHK_DIALOG_ERROR;
                    TitleFSM.Instance.SendFsmEvent_DoError();
                    return;
                }
            }
        }


        //----------------------------------------
        // ここまで来たら何の問題もなく中断復帰可能！
        //----------------------------------------
        {
            //----------------------------------------
            // サーバーから取得したクエストのコンティニューとリセット回数がローカルの回数と一致する場合、
            // サーバー側が先行して成立しているけどローカルがそれを受け取る前にアプリを落としたケースと考えられる。
            // ※ローカルに保存されている回数情報は「次のコンティニューの時に送るカウント」なので本来は１多いはず
            //
            // インゲーム内部で判断して無料でコンティニューやリセットができるように、
            // ここでフラグを立ててインゲームへ送り出す
            //----------------------------------------
            ResidentParam.m_QuestRestoreContinue = false;
            ResidentParam.m_QuestRestoreReset = false;
            LocalSaveContinue cLocalSaveContinue = LocalSaveManager.Instance.LoadFuncInGameContinue();
            LocalSaveReset cLocalSaveReset = LocalSaveManager.Instance.LoadFuncInGameReset();
            if (cLocalSaveContinue != null && cLocalSaveContinue.nContinueCt == nQuestContinue && nQuestContinue != 0)
            {
                ResidentParam.m_QuestRestoreContinue = true;
            }
            else if (cLocalSaveReset != null && cLocalSaveReset.nResetCt == nQuestReset && nQuestReset != 0)
            {
                ResidentParam.m_QuestRestoreReset = true;
            }

            //----------------------------------------
            // サーバーのクエスト構築情報を後で受け渡すために保持しておく
            //----------------------------------------
            m_RestoreQuest2Build = serverOrderQuestDetail.result.quest;

            //----------------------------------------
            // サーバー上のクエスト受諾情報が正常なら中断復帰できる。
            //----------------------------------------
            TitleFSM.Instance.SendFsmNextEvent();
        }
    }

    private void errorRestore(ServerApi.ResultData data)
    {
        //                      m_RestoreSeq = RESTORE_SEQ.SEQ_CHK_DIALOG_ERROR;
        TitleFSM.Instance.SendFsmEvent_DoError();
        Debug.LogError("PacketResult Code Error! - " + data.m_PacketCode);
    }

    void OnINIT()
    {
        LocalSaveManager.Instance.DiffTitleUUID("SceneTitle/OnINIT");

        TimeManager tm = TimeManager.Instance;
        if (tm != null)
        {
            int nChangeTimeHour = TimeManager.Instance.ChangeTimeHour();

            if (tm.m_TimeNow.Hour >= nChangeTimeHour)
            {
                DateTime nextDay = tm.m_TimeNow.AddDays(1);
                MainMenuParam.m_ReturnTitleTime = new DateTime(nextDay.Year,
                                                                nextDay.Month,
                                                                nextDay.Day,
                                                                nChangeTimeHour,
                                                                0,
                                                                0);
            }
            else
            {
                MainMenuParam.m_ReturnTitleTime = new DateTime(tm.m_TimeNow.Year,
                                                                 tm.m_TimeNow.Month,
                                                                 tm.m_TimeNow.Day,
                                                                 nChangeTimeHour,
                                                                 0,
                                                                 0);
            }

        }

        //アイコンキャッシュ設定
        AssetBundlerResponse.clearAssetBundleChash();
        UnitIconImageProvider.Instance.ClearAllCache();
        UnitIconImageProvider.Instance.SetLoaderType(UnitIconImageProvider.LoaderType.AssetBundle);

        //
        UnitIconImageProvider.Instance.hiddenCanvas(true);

        TitleFSM.Instance.SendFsmNextEvent();

        // スリープしないようにする
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void OnDiskSizeLimit()
    {

        if (LocalSaveUtilToInstallFolder.ChkSavePossible() == true)
        {
            TitleFSM.Instance.SendFsmNextEvent();
            return;
        }

        string text = GameTextUtil.GetText("storage_caution");

#if BUILD_TYPE_DEBUG
        text = String.Format("{0}\n\n[デバッグ]\n端末の空き容量:{1}MB\n警告を出す容量:{2}MB",
                            text,
                            LocalSaveUtilToInstallFolder.GetSaveDiskFreeSpace(),
                            LocalSaveUtilToInstallFolder.GetRequestFreeSpace());
#endif

        DialogManager.Open1B_Direct(GameTextUtil.GetText("error_reject_common_title"),
                                    text,
                                    "common_button7",
                                    true, true).
        SetOkEvent(() =>
        {
            TitleFSM.Instance.SendFsmNextEvent();
        });
    }

    private GetMaster2er getMaster2er;

    private float DownloadMasterCount;
    private float DownloadMasterMax;

    void SqliteUpdateProgressPercent(float parcent)
    {
        if (MovieManager.Instance != null)
        {
            MovieManager.Instance.setPercent(parcent * 100);
        }
        else
        {
            LoadingManager.Instance.Progress(parcent * 100);
        }
    }

    void SqliteUpdateProgressFile()
    {
        DownloadMasterCount++;
        if (DownloadMasterCount > DownloadMasterMax)
        {
            DownloadMasterCount = DownloadMasterMax;
        }

        if (MovieManager.Instance != null)
        {
            MovieManager.Instance.setProgressFiles(DownloadMasterCount, DownloadMasterMax);
        }
        else
        {
            LoadingManager.Instance.ProgressFiles(DownloadMasterCount, DownloadMasterMax);
        }
    }

    IEnumerator OnDownloadSqlite()
    {
        // ダウンロードするか判定
        // 空DBコピー、ダウンロード済みのDBの場合はスキップする
        SQLiteClient.Instance.UpdateDbClientVersion();
        double unixTime = SQLiteClient.Instance.GetDbCreateTime();
        if (unixTime > SQLiteClient.EmptyDbUnixTime)
        {
#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
            Debug.Log("CALL OnDownloadSqlite skip");
#endif
            TitleFSM.Instance.SendFsmNextEvent();
            yield break;
        }

        if (MovieManager.Instance == null)
        {
            LoadingManager.Instance.RequestLoadingFinish();
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.DATA_DOWNLOAD);
        }


        yield return DowloadSqliteDb((WWW www) =>
        {
#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
            Debug.Log("CALL OnDownloadSqlite Finish");
            if (www != null && string.IsNullOrEmpty(www.error))
            {
                // ダウンロードが正常に完了した
                TitleFSM.Instance.SendFsmNextEvent();
            }
            else
            {
                // ダウンロードでエラーが発生した
                DowloadSqliteDbError(www,
                                     () =>
                                     {
                                         TitleFSM.Instance.SendFsmNextEvent();
                                     });
            }
#else
            TitleFSM.Instance.SendFsmNextEvent();
#endif
        },
        (float progress) =>
        {
            SqliteUpdateProgressPercent(progress);
        });
    }

#if BUILD_TYPE_DEBUG
    System.Diagnostics.Stopwatch master2erStopwatch = new System.Diagnostics.Stopwatch();
#endif

    void OnCallGetMaster2API()
    {
        if (getMaster2er == null)
        {
#if BUILD_TYPE_DEBUG
            master2erStopwatch.Start();
#endif
            getMaster2er = new GetMaster2er();

            if (MovieManager.Instance == null)
            {
                LoadingManager.Instance.RequestLoadingFinish();
                LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.DATA_DOWNLOAD);
            }

            DownloadMasterCount = 0;
            // +1はその他取得していないMasterの分
            DownloadMasterMax = getMaster2er.TargetListCount + 1;
        }

        SqliteUpdateProgressFile();
#if BUILD_TYPE_DEBUG
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
#endif

        Dictionary<EMASTERDATA, uint> targetDict = getMaster2er.Next();
        ServerDataUtilSend.SendPacketAPI_GetMasterDataAll2(targetDict).
            setSuccessAction(_data =>
            {
                result = _data.GetResult<RecvMasterDataAll2>().result;

#if BUILD_TYPE_DEBUG
                stopwatch.Stop();
                Debug.Log("stopwatch SendPacketAPI_GetMasterDataAll2: " + stopwatch.Elapsed.TotalSeconds);
#endif

                TitleFSM.Instance.SendFsmNextEvent();
            }).
            setErrorAction(_data =>
            {
                Debug.LogError("ERROR");
            }).
            SendStart();
    }

    private RecvMasterDataAll2Value result;

    IEnumerator OnGetAndReflectAllData()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnGetAndReflectAllData");
#endif
        foreach (EMASTERDATA d in MasterDataDefine.SQLiteHoldList())
        {
            yield return null;
        }

        TitleFSM.Instance.SendFsmNextEvent();
    }

    void OnExistsRestMasterData()
    {
        if (getMaster2er.IsDone)
        {
#if BUILD_TYPE_DEBUG
            master2erStopwatch.Stop();
            Debug.Log("stopwatch OnExistsRestMasterData: " + master2erStopwatch.Elapsed.TotalSeconds);
#endif
            LoadingManager.Instance.RequestLoadingFinish();
            TitleFSM.Instance.SendFsmNegativeEvent();
            return;
        }
        TitleFSM.Instance.SendFsmPositiveEvent();
    }

    void OnReflectMasterDiff()
    {
        if (result == null)
        {
            TitleFSM.Instance.SendFsmNextEvent();
            return;
        }

#if BUILD_TYPE_DEBUG
        var debug_db_error_text = new StringBuilder();

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
#endif

        m_semaphore.Lock(() =>
        {
#if BUILD_TYPE_DEBUG
            stopwatch.Stop();
            Debug.Log("stopwatch OnReflectMasterDiff: " + stopwatch.Elapsed.TotalSeconds);

            if (debug_db_error_text.Length > 0)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Dialog dialog = Dialog.Create(DialogType.DialogScrollInfo);
                    dialog.SetDialogText(DialogTextType.Title, "Sqlite3: ERROR:");
                    dialog.AddScrollInfoText("Sqlite（キャッシュ）にデータが格納できませんでした。\n" +
                                             "サーバから送られてきたデータがクライアントで受け取れない状況です。\n" +
                                             "クライアントもしくはサーバに画面を見せるか全てのページをキャプチャして報告してください\n\n" +
                                             GlobalDefine.GetApplicationStatus(false) + "\n\n" +
                                              debug_db_error_text);
                    dialog.SetDialogText(DialogTextType.OKText, GameTextUtil.GetText("common_button7"));
                    dialog.EnableFadePanel();
                    dialog.DisableAutoHide();
                    dialog.SetOkEvent(() =>
                    {
                        TitleFSM.Instance.SendFsmNextEvent();
                    });
                    dialog.Show();
                });
            }
            else
            {
                TitleFSM.Instance.SendFsmNextEvent();
            }
#else
             TitleFSM.Instance.SendFsmNextEvent();
#endif
        });
        new Thread(() =>
        {
            foreach (FieldInfo fi in typeof(RecvMasterDataAll2Value).GetFields())
            {
                object obj = fi.GetValue(result);
                try
                {
                    List<int> l = SQLiteClient.Instance.Reflect(obj);
#if BUILD_TYPE_DEBUG
                    if (l.Count >= 2 && l[1] > 0)
                    {
                        Debug.Log("Sqlite3: " + string.Format("REFLECT {0} u_or_i:{1}件 d:{2}件 \n", DivRNUtil.GetTableName(obj.GetType().ToString()), l[0], l[1]));
                    }
#endif
                }
                catch (Exception e)
                {
#if BUILD_TYPE_DEBUG
                    Debug.LogError("Sqlite3: ERROR:" + e.Message + "::" + obj.GetType().ToString());
                    debug_db_error_text.Append(e.Message + "::" + obj.GetType().ToString() + "\n\n");
#endif
                }
            }

            m_semaphore.Unlock();
        }
        ).Start();
    }

    // タイトルでダウンロードするカテゴリ
    private readonly MasterDataDefineLabel.ASSETBUNDLE_CATEGORY[] TITLE_DOWNLOAD_CATEGORY =
    {
        MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.HERO,
        MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.TUTORIAL,
        MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.STORYBG,
        MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.NPC,
        MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.BATTLEBG,
        MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.AREAMAPICON,
        MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.AREAMAP,
        MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.PACKBGM,
        MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.PACKSE,
        MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.REPLACEPKG,
        MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.MAPBG,
        MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.GENERALWINDOW,
    };

    void OnLoadResidentResource()
    {
        // 動画再生中にディスク書き込みしないようにするための対応
        AssetBundlerPlayerPrefs.isSaveDisk = false;

        List<int> categorys = null;

        // タイトル→ホームはすぐに暗転してアセットダウンロード時にローディング画面を切り替える
        if (MovieManager.Instance == null)
        {
            LoadingManager.Instance.RequestLoadingFinish();
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.DATA_DOWNLOAD);
        }
        else
        {
            //タイトルでキャッシュするAssetBundle登録
            categorys = new List<int>();
            categorys.Add((int)MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.TITLE);

            // タイトルでダウンロードするカテゴリを追加
            for (var i = 0; i < TITLE_DOWNLOAD_CATEGORY.Length; i++)
            {
                categorys.Add((int)TITLE_DOWNLOAD_CATEGORY[i]);
            }
        }

        SceneCommon.Instance.LoadResidentResource(() =>
        {
            // DG0-2697
            //　calcExpRatioのMasterDataUserRanknの参照をSqliteから行うとクラッシュしているケースがある
            // (iOS版のクラッシュログ)
            // 取得後にキャッシュに載せることでSqliteを行わないようにする
            // PlayerParamでステータス更新を行なっておく
            MasterFinder<MasterDataUserRank>.Instance.FindAll();
            UserDataAdmin.Instance.updatePlayerParam();

            //RegisterNotificationで直接取得せずにマスター更新後にキャッシュにのせる
            MasterDataUtil.GetMasterDataNotification();
            TitleFSM.Instance.SendFsmNextEvent();
        },
        categorys);
    }

    void onstoreListSuccess(bool success)
    {
        if (success)
        {
            TitleFSM.Instance.SendFsmNextEvent();
        }
        else
        {
            TitleFSM.Instance.SendFsmEvent("DO_ERROR", 0);
        }
    }

    void OnStoreProductList()
    {
        if (StoreManagerFSM.Instance.IsPurchaseWait)
        {
            TitleFSM.Instance.SendFsmNextEvent();
            return;
        }

        StoreManager.Instance.startStoreList(onstoreListSuccess);
    }

    void OnStoreProductListError()
    {
        DialogManager.Open1B_Direct("ERROR_PAY_ITUNES_TITLE",
                                    "ERROR_PAY_ITUNES",
                                    "common_button7",
                                    true, false).
                SetOkEvent(() =>
                {
                    TitleFSM.Instance.SendFsmNextEvent();
                });
    }

    void onpurchaseRestoreSucess(bool success)
    {
        TitleFSM.Instance.SendFsmNextEvent();
    }

    void OnStoreProductRestore()
    {
        if (StoreManagerFSM.Instance.IsPurchaseWait)
        {
            TitleFSM.Instance.SendFsmNextEvent();
            return;
        }

        StoreManager.Instance.startPurchaseRestore(onpurchaseRestoreSucess);
    }

    void OnEND()
    {
        // デフォルトのスリープ設定に戻す
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        //----------------------------------------
        // 今後作られる中断復帰情報はアプリ内バージョンで保存されるようにしておく
        //----------------------------------------
        LocalSaveManager.Instance.SaveFuncRestoreVersion();


        if (MovieManager.Instance != null)
        {
            if (MovieManager.Instance.isMoviePlay == true)
            {
                MovieManager.Instance.setLoop(false);
                MovieManager.Instance.setSkip(true);
            }
        }

        TitleFSM.Instance.SendFsmNextEvent();
    }

    void OnMovieCheck()
    {
        if (MovieManager.Instance != null)
        {
            if (MovieManager.Instance.isMoviePlay == true)
            {
                return;
            }
        }

        TitleFSM.Instance.SendFsmNextEvent();
    }

    void OnAssetBundleDeleteCheck()
    {
        List<MasterDataAssetBundlePath> assetBundlePathList = MasterFinder<MasterDataAssetBundlePath>.Instance.SelectWhere(" where title_dl = ? ", MasterDataDefineLabel.ASSETBUNDLE_TITLE_DL.DELETE);
        if (assetBundlePathList != null)
        {
            for (int i = 0; i < assetBundlePathList.Count; i++)
            {
                string lowerName = assetBundlePathList[i].name.ToLower();
                AssetBundlerPlayerPrefs.DelKey(lowerName);
                LocalSaveUtilToInstallFolder.RemoveAsssetBundle(lowerName);
            }
        }

        TitleFSM.Instance.SendFsmNextEvent();
    }

    void OnAssetBundleVersionSaveDisk()
    {
        //　ディスク書き込み復帰
        AssetBundlerPlayerPrefs.isSaveDisk = true;
        AssetBundlerPlayerPrefs.Save();

        TitleFSM.Instance.SendFsmNextEvent();
    }

    private void OnCreateAtlus()
    {
        //アトラス生成
        UnitIconImageProvider.Instance.MakeAtlus(() =>
        {
            TitleFSM.Instance.SendFsmNextEvent();
        });
    }

    private void OnCacheUnit()
    {
#if true
        //ユニットパラメータ作成開始するが待たない
        UserDataAdmin.Instance.CreateThreadUnitGridParam();

        TitleFSM.Instance.SendFsmNextEvent();
#else
        //ユニットパラメータ作成できるまでタイトルで待つ

        // CreateUnitGridParam()でPlayerPrefasを使っているのでメインスレッドでキャッシュを作っておく
        LocalSaveManager.Instance.LoadFuncAddFavoriteUnit();

        m_semaphore.Lock(() => {
            TitleFSM.Instance.SendFsmNextEvent();
        });
        new System.Threading.Thread(() =>
        {
            //ユニットパラメータ作成
            UserDataAdmin.Instance.CreateUnitGridParam();
            m_semaphore.Unlock();
        }).Start();
#endif
    }

    void OnShouldStartTutorial()
    {
        if (LocalSaveManager.Instance.LoadFuncMovieFirst() == false)
        {
            LocalSaveManager.Instance.SaveFuncMovieFirst(true);
        }

#if UNITY_EDITOR && BUILD_TYPE_DEBUG
        if (DebugOption.Instance.tutorialDO.forceTutorialPart != TutorialPart.NONE)
        {

            TitleFSM.Instance.SendFsmPositiveEvent();
            return;
        }
#endif

        if (TutorialManager.GetNextTutorialPart() == TutorialPart.LAST)
        {
            UnitIconImageProvider.Instance.hiddenCanvas();

            TitleFSM.Instance.SendFsmNegativeEvent();
            return;
        }

        TitleFSM.Instance.SendFsmPositiveEvent();
    }

    void OnStartTutorial()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnStartTutorial");
#endif

        TutorialPart part = TutorialManager.GetNextTutorialPart();

        if (
#if BUILD_TYPE_DEBUG
         DebugOption.Instance.tutorialDO.skip == false &&
#endif
        (part == TutorialPart.NONE || part == TutorialPart.NORMAL01 || part == TutorialPart.BATTLE))
        {
            LoadingManager.Instance.setOverLayMask(true);
        }

#if UNITY_EDITOR && BUILD_TYPE_DEBUG
        if (DebugOption.Instance.tutorialDO.forceTutorialPart != TutorialPart.NONE)
        {
            ServerDataUtilSend.SendPacketAPI_SelectDefParty(10).
            setSuccessAction(_data =>
            {
                UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvSelectDefParty>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                UserDataAdmin.Instance.m_StructHeroList = _data.GetResult<RecvSelectDefParty>().result.hero_list;
                UserDataAdmin.Instance.ConvertPartyAssing();
                SceneCommon.Instance.ChangeScene(SceneType.SceneMainMenu);
            }).
            setErrorAction(data =>
            {
                Debug.LogError("ERROR");
                SceneCommon.Instance.ChangeScene(SceneType.SceneMainMenu);
            }).
            SendStart();
        }
        else
#endif
        {
            SceneCommon.Instance.ChangeScene(SceneType.SceneMainMenu);
        }

        SoundUtil.StopBGM(false);
    }

    void OnShouldStartRestore()
    {
        if (m_RestoreQuest2Build == null)
        {
            TitleFSM.Instance.SendFsmNegativeEvent();
        }
        else
        {
            TitleFSM.Instance.SendFsmPositiveEvent();
        }
    }

    void OnChangeScene_MainMenu()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL SceneTitle#OnChangeScene_MainMenu");
#endif
        //----------------------------------------
        // 中断復帰を適用しない場合は変な情報を拾わないようにnull入れておく
        //----------------------------------------
        SceneGoesParam.Instance.m_SceneGoesParamToQuest2 = null;
        SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build = null;
        SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore = null;

        //BGM停止
        SoundUtil.StopBGM(false);
        SceneCommon.Instance.ChangeScene(SceneType.SceneMainMenu);
    }
}
