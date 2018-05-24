using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneSplash : SceneMode<SceneSplash>
{
    public GameObject m_LogObject = null;
    public Animation m_LogoAnimation = null;
    public GameObject m_MessageObject = null;
    public GameObject m_ScreenClick = null;
    public Image m_FadeScreen = null;
    MovieManager m_MovieManager;
    GameObject m_MoviePrefab;
    const string OPENING_MOVIE_FILE_NAME = "DG_teaser_master_full.mp4";

    private float m_WaitTime;
    private Animation m_MessageAnimation = null;
    private TextMeshProUGUI m_MessageText = null;
    private bool m_firstPolicy = true;
    private bool m_firstAgreement = true;

    protected override void InitlaizePrefab()
    {
        //Scene<T>.Awakeで初期化されないようにするための処置
    }

    protected override void PrepareSEs()
    {
        //Scene<T>.Startの呼び出し順序調整
    }

    protected override void Awake()
    {
        base.Awake();

        SceneCommon.initalizeMenuFps();
    }

    protected override void Start()
    {
        base.Start();

    }

    void OnQuitApplication()
    {
        DivRNUtil.ShowQuitApplicationDialog();
    }

    void OnPatchUpdate()
    {
        Patcher.Instance.Load(
            () =>
            {
#if BUILD_TYPE_DEBUG
                string format_text = Patcher.Instance.checkFormat();
                if (format_text.Length > 0)
                {
                    Dialog dialog = Dialog.Create(DialogType.DialogScrollInfo);
                    dialog.SetDialogText(DialogTextType.Title, "Patcher");
                    dialog.AddScrollInfoText(format_text);
                    dialog.SetDialogText(DialogTextType.OKText, GameTextUtil.GetText("common_button7"));
                    dialog.EnableFadePanel();
                    dialog.DisableAutoHide();
                    dialog.SetOkEvent(() =>
                    {
                        SceneSplashFSM.Instance.SendFsmEvent_FailRetry();
                    });
                    dialog.Show();
                    return;
                }
#endif
                LocalSaveManagerRN.Instance.PatcherCounter = Patcher.Instance.GetUpdateCounter();
                LocalSaveManagerRN.Instance.Save();
                if (LocalSaveManager.Instance.LoadFuncInformationVer() != "")
                {
                    m_firstAgreement = false;
                }
                if (LocalSaveManager.Instance.LoadFuncInformationPolicyVer() != "")
                {
                    m_firstPolicy = false;
                }
                SceneSplashFSM.Instance.SendFsmEvent_Success();
            },
            (error) =>
            {
                Debug.LogError("ERROR:" + error);
                SceneSplashFSM.Instance.SendFsmEvent_FailQuit();
                /*
                                DivRNUtil.ShowRetryDialog(
                                    () =>
                                    {
                #if BUILD_TYPE_DEBUG
                                        DialogManager.Open1B_Direct("Patcher",
                                                        "Patchファイルの取得エラー\n" +
                                                        "通信をOFFにしていると場合にPatcが取得できず\n" +
                                                        "このエラーがが発生します\n\n" +
                                                        "Patchファイルのフォーマットエラー\n" +
                                                        "パッチファイルの構造が壊れている可能性が\n" +
                                                        "あります。確認及び修正してください\n\n" +
                                                        "HTTPS接続時に端末時間を変更すると\n" +
                                                        "このエラーになる場合があります\n " +
                                                        "端末時間を現在時間に修正してアクセスしてください",
                                                        "common_button7", true, true).
                                        SetOkEvent(() =>
                                        {
                                            SceneSplashFSM.Instance.SendFsmEvent_FailRetry();
                                        });
                #else
                                        SceneSplashFSM.Instance.SendFsmEvent_FailRetry();
                #endif
                                });
                */
            });
    }

    void OnStartWait()
    {
        SceneSplashFSM.Instance.SendFsmNextEvent();
    }

    void OnLogoFadeIn()
    {
        if (m_MessageObject != null)
        {
            m_MessageAnimation = m_MessageObject.GetComponent<Animation>();
            m_MessageText = m_MessageObject.GetComponent<TextMeshProUGUI>();
        }

        // iOS 7、8のロゴ崩れ問題対応
        // PTUnityAppController.mm 参照
        Invoke("startLog", 1);
    }

    void startLog()
    {
        UnityUtil.SetObjectEnabled(m_LogObject, true);

        m_LogoAnimation.Play("SplashFadeIn");
        SceneSplashFSM.Instance.SendFsmNextEvent();
        m_WaitTime = 0;

    }

    IEnumerator OnWaitInitialize()
    {
        yield return new WaitForSeconds(1.5f);

        base.InitlaizePrefab();
        base.OnEnable();
        base.PrepareSEs();
        base.OnInitialized();

        SceneSplashFSM.Instance.SendFsmNextEvent();
    }

    void OnMessageFadeIn()
    {
        if (m_LogoAnimation.isPlaying == true)
        {
            return;
        }

        if (m_WaitTime >= 1.5)
        {
            m_MessageText.text = GameTextUtil.GetText("pp2p_display");
            m_MessageAnimation.Play("SplashMsgFadeIn");
            SceneSplashFSM.Instance.SendFsmNextEvent();
            m_WaitTime = 0;
        }
        else
        {
            m_WaitTime += Time.deltaTime;
        }
    }

    void OnMessageSkipCheck()
    {
        if (m_MessageAnimation.isPlaying == true)
        {
            return;
        }

        if (UnityUtil.ChkObjectEnabled(m_ScreenClick) == false)
        {
            UnityUtil.SetObjectEnabled(m_ScreenClick, true);
        }

        if (m_WaitTime >= 1.5)
        {
            OnScreenClick();
        }
        else
        {
            m_WaitTime += Time.deltaTime;
        }
    }

    void OnScreenClick()
    {
        UnityUtil.SetObjectEnabled(m_ScreenClick, false);
        m_MessageAnimation.Play("SplashMsgFadeOut");
        SceneSplashFSM.Instance.SendFsmNextEvent();
    }

    void OnMessageFadeOut()
    {
        if (m_MessageAnimation.isPlaying == true)
        {
            return;
        }

        SceneSplashFSM.Instance.SendFsmNextEvent();
    }


    void OnShouldShowPrivacyPolicyDialog()
    {

        int nAgreementOK = LocalSaveManager.Instance.LoadFuncInformationPolicyOK();
        if (nAgreementOK == (int)LocalSaveManager.AGREEMENT.AGREE_OK)
        {
            SceneSplashFSM.Instance.SendFsmNegativeEvent();
            return;
        }
        SceneSplashFSM.Instance.SendFsmPositiveEvent();
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
            SceneSplashFSM.Instance.SendFsmPositiveEvent();
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
        {
            newDialog.Hide();
            SceneSplashFSM.Instance.SendFsmNegativeEvent();
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
        SceneSplashFSM.Instance.SendFsmPositiveEvent();
    }

    void OnShowPrivacyPolicyNeedPositiveDialog()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "pp9q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "pp9q_content");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button6");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
        {
            SceneSplashFSM.Instance.SendFsmNextEvent();
        });
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    void OnShouldShowAgreementDialog()
    {
        int nAgreementOK = LocalSaveManager.Instance.LoadFuncInformationOK();
        if (nAgreementOK == (int)LocalSaveManager.AGREEMENT.AGREE_OK)
        {
            SceneSplashFSM.Instance.SendFsmNegativeEvent();
            m_WaitTime = 0.25f;
            return;
        }
        SceneSplashFSM.Instance.SendFsmPositiveEvent();
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
            SceneSplashFSM.Instance.SendFsmPositiveEvent();
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
        {
            newDialog.Hide();
            SceneSplashFSM.Instance.SendFsmNegativeEvent();
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
        m_WaitTime = 0.25f;
        SceneSplashFSM.Instance.SendFsmPositiveEvent();
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
            SceneSplashFSM.Instance.SendFsmNextEvent();
        });
        newDialog.Show();
    }

    void OnScreenFade()
    {
        m_WaitTime -= Time.deltaTime;
        if (m_WaitTime <= 0)
        {
            m_WaitTime = 0;
            SceneSplashFSM.Instance.SendFsmNextEvent();
        }
        Color col = m_FadeScreen.color;
        col.a = 1 - (m_WaitTime / 0.25f);
        m_FadeScreen.color = col;
    }

    void OnShouldPlayMovie()
    {
        if (LocalSaveManager.Instance.LoadFuncMovieFirst() == false)
        {
            // 初回起動時のみ再生
            GameObject _obj = Resources.Load("Prefab/MoviePrefab") as GameObject;
            if (_obj != null)
            {
                m_MoviePrefab = Instantiate(_obj) as GameObject;
                m_MovieManager = m_MoviePrefab.GetComponent<MovieManager>();
            }
            SceneSplashFSM.Instance.SendFsmPositiveEvent();
            return;
        }
        SceneSplashFSM.Instance.SendFsmNegativeEvent();
    }

    //動画再生
    void OnPlayMovie()
    {
        if (m_MovieManager != null)
        {
            //			UnityUtil.SetObjectEnabledOnce(m_MoviePrefab, true);
            m_MovieManager.play(OPENING_MOVIE_FILE_NAME, false, false);
        }
        SceneSplashFSM.Instance.SendFsmEvent("FINISH_MOVIE");
    }

    //動画完了
    void OnStopMovie()
    {
        if (m_MovieManager.isMoviePlay == false)
        {
            LocalSaveManager.Instance.SaveFuncMovieFirst(true);

            SceneSplashFSM.Instance.SendFsmNextEvent();
        }
    }

    void OnExit()
    {
        SceneCommon.Instance.ChangeScene(SceneType.SceneTitle);
    }
}
