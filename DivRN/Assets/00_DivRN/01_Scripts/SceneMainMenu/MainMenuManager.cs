using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ServerDataDefine;
using UniExtensions;

//----------------------------------------------------------------------------
/*!
	@brief	パッチテキストアップデートリクエストstep.
*/
//----------------------------------------------------------------------------
public enum EMAINMENU_PATCHUPDATE_REQ : byte
{
    NONE = 0, // 無し
    REQUEST = 1, // リクエスト
    WAIT = 2, // パッチの完了待ち
    GOTITLE = 3, // タイトルに戻る.

    MAX,
};

public class MainMenuPageParam
{
    public const int LINKER_DRAW = (1 << 1);
    public const int LINKER_ACTIVE = (1 << 2);
    public const int HEADER_DRAW = (1 << 3);
    public const int HEADER_ACTIVE = (1 << 4);
    public const int RETURN = (1 << 5);

    public int m_LinkerState; //!< ページ表示時のメインメニュー下部リンカの動作制限
    public bool m_RefreshResource; //!< ページ閉じ時のリソース破棄処理有無

    public string m_OriginGameObjectPath; //!< 複製元情報：オリジナルオブジェクトパス
    public GameObject m_OriginGameObject; //!< 複製元情報：オリジナルオブジェクト
    public Type m_OriginComponent; //!< 複製元情報：接続コンポーネント

    public GameObject[] m_OriginPartsObjArray;
    public GameObject[] m_PartsObjArray;

    public GameObject m_GameObject; //!< 実働ページ：ページルートオブジェクト
    public MainMenuSeq m_MainMenuSeq; //!< 実働ページ：コンポーネント基底クラス
    public MasterMainMenuSeq m_MasterMainMenuSeq;

    public MAINMENU_SEQ m_MainMenuType = MAINMENU_SEQ.SEQ_MAX;
};

public class MainMenuManager : SingletonComponent<MainMenuManager>
{
    const int SWITCH_STATE_REQUEST_CHK = 0; //!< 切り替え状態：リクエスト判断

    public static ulong s_LastLoginTime = 0; //!< 最後にログイン処理を投げた時間
    //		※インゲームを経由して戻っても情報があって欲しいのでstatic
    //		※起動直後は確実に現在時と比較して日付が変化していると見なせるはず。

    public int m_MainMenuStartOK = 0;

    private Dictionary<MAINMENU_SEQ, MainMenuPageParam> mainMenuPageParamDict = new Dictionary<MAINMENU_SEQ, MainMenuPageParam>();

    private Queue<MainMenuSwitchReq> switchRequestQueue = new Queue<MainMenuSwitchReq>();
    private MainMenuFooter m_MainMenuFooter = null;
    private GameObject m_MainMenuFooterObj = null;
    private MainMenuHeader m_MainMenuHeader = null;
    private GameObject m_MainMenuHeaderObj = null;
    private MainMenuSubTab m_MainMenuSubTab = null;
    private GameObject m_MainMenuSubTabObj = null;
    private int m_WorkSwitchState = SWITCH_STATE_REQUEST_CHK; //!< ページ切り替え作業情報：切り替え状態
    private MAINMENU_SEQ m_WorkSwitchPagePrev = MAINMENU_SEQ.SEQ_MAX; //!< ページ切り替え作業情報：最後に開いてたページ
    private MAINMENU_SEQ m_WorkSwitchPageNow = MAINMENU_SEQ.SEQ_MAX; //!< ページ切り替え作業情報：現在開いているページ

    private static EMAINMENU_PATCHUPDATE_REQ m_PatchUpdateRequestStep = EMAINMENU_PATCHUPDATE_REQ.NONE; //!< Patcherのファイル更新リクエスト.

    [HideInInspector]
    public bool m_ResumePatchUpdateRequest = true; //!< レジューム時のパッチリクエストフラグ外部からの設定版(個別にシーン中はロックする場合にstart辺りでfalseにする).

    private static bool m_ResumePatchUpdateSeqLock = false; //!< レジューム時のパッチリクエストフラグクラス共通版(遷移中の動作ふさぎなど).

    private bool m_PatchReqChkDialog = false; //!< @add Developer 2016/04/18 ダイアログ中のリクエストをチェックするかフラグ
    private bool backkey = true;

    public EMAINMENU_PATCHUPDATE_REQ patchUpdateRequestStep
    {
        get
        {
            return (m_PatchUpdateRequestStep);
        }
    }

    private bool m_isUnitDitailCloseActive = true;
    private bool m_isUnitDitailSaveReturn = false;

    private MAINMENU_CATEGORY m_carrentCategory = MAINMENU_CATEGORY.NONE;
    public MAINMENU_CATEGORY currentCategory
    {
        get
        {
            return m_carrentCategory;
        }
        set
        {
            m_carrentCategory = value;

            if (m_onCategoryChanged != null)
            {
                m_onCategoryChanged(m_carrentCategory);
            }
        }
    }
    private Action<MAINMENU_CATEGORY> m_onCategoryChanged = null;
    public void RegisterOnCategoryChangedCallback(Action<MAINMENU_CATEGORY> callback)
    {
        m_onCategoryChanged = callback;
    }


    public MainMenuFooter Footer
    {
        get
        {
            return m_MainMenuFooter;
        }
    }


    public MainMenuHeader Header
    {
        get
        {
            return m_MainMenuHeader;
        }
    }


    public MainMenuSubTab SubTab
    {
        get
        {
            return m_MainMenuSubTab;
        }
    }


    public MAINMENU_SEQ WorkSwitchPageNow
    {
        get
        {
            return m_WorkSwitchPageNow;
        }
    }

    public MainMenuSeq MainMenuSeqPageNow
    {
        get
        {
            if (!mainMenuPageParamDict.ContainsKey(m_WorkSwitchPageNow)) return null;
            return mainMenuPageParamDict[m_WorkSwitchPageNow].m_MainMenuSeq;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        SceneObjReferMainMenu mainmenu = SceneObjReferMainMenu.Instance;
        if (mainmenu.m_MainMenuSubTab != null)
        {
            m_MainMenuSubTabObj = Instantiate(mainmenu.m_MainMenuSubTab) as GameObject;
            m_MainMenuSubTabObj.transform.SetParent(mainmenu.m_MainMenuTopAnchor.transform, false);
            m_MainMenuSubTab = m_MainMenuSubTabObj.GetComponent<MainMenuSubTab>();
        }

        //--------------------------------
        // 固定レイヤーにスクリプトアサイン
        //--------------------------------
        ShowHeader();

        //--------------------------------
        // ページの設定
        //--------------------------------
        //
        foreach (MasterMainMenuSeq master in Resources.LoadAll<MasterMainMenuSeq>("MasterData/MainMenuSeq"))
        {
            MAINMENU_SEQ nMainMenuSeqNum = master.Sequence;
            mainMenuPageParamDict[nMainMenuSeqNum] = new MainMenuPageParam();
            mainMenuPageParamDict[nMainMenuSeqNum].m_OriginGameObjectPath = "";
            mainMenuPageParamDict[nMainMenuSeqNum].m_OriginComponent = System.Type.GetType(master.SequenceName);
            mainMenuPageParamDict[nMainMenuSeqNum].m_RefreshResource = master.RefreshResource;
            mainMenuPageParamDict[nMainMenuSeqNum].m_GameObject = null;
            mainMenuPageParamDict[nMainMenuSeqNum].m_MainMenuSeq = null;
            mainMenuPageParamDict[nMainMenuSeqNum].m_LinkerState = 0;
            mainMenuPageParamDict[nMainMenuSeqNum].m_MasterMainMenuSeq = master;
            if (master.FooterActive)
            {
                mainMenuPageParamDict[nMainMenuSeqNum].m_LinkerState |= MainMenuPageParam.LINKER_ACTIVE;
            }
            if (master.FooterDraw)
            {
                mainMenuPageParamDict[nMainMenuSeqNum].m_LinkerState |= MainMenuPageParam.LINKER_DRAW;
            }
            if (master.HeaderActive)
            {
                mainMenuPageParamDict[nMainMenuSeqNum].m_LinkerState |= MainMenuPageParam.HEADER_ACTIVE;
            }
            if (master.HeaderDraw)
            {
                mainMenuPageParamDict[nMainMenuSeqNum].m_LinkerState |= MainMenuPageParam.HEADER_DRAW;
            }
            if (master.Return)
            {
                mainMenuPageParamDict[nMainMenuSeqNum].m_LinkerState |= MainMenuPageParam.RETURN;
            }
        }

        if (SafeAreaControl.Instance &&
            SceneObjReferMainMenu.Instance)
        {
            int bottom_space_height = SafeAreaControl.Instance.bottom_space_height;
            if (bottom_space_height > 0)
            {
                GameObject goTop = SceneObjReferMainMenu.Instance.m_TopDecoNoMenu;
                goTop.transform.AddLocalPositionY(SafeAreaControl.Instance.bar_height);
                SceneObjReferMainMenu.Instance.m_MainMenuBottomAnchor.transform.AddLocalPositionY(bottom_space_height);
            }
        }

        AndroidBackKeyManager.Instance.StackPush(gameObject, OnSelectBackKey);

        m_PatchUpdateRequestStep = EMAINMENU_PATCHUPDATE_REQ.NONE;
    }

    protected override void OnDestroy()
    {
        //--------------------------------
        // 基底呼出し
        //--------------------------------
        base.OnDestroy();


        //--------------------------------
        // 固定レイヤー破棄
        //--------------------------------
        if (m_MainMenuFooter != null && AndroidBackKeyManager.HasInstance)
        {
            AndroidBackKeyManager.Instance.StackPop(m_MainMenuFooter.gameObject);
        }
        HideFooter();
        HideHeader();


        mainMenuPageParamDict.Clear();

        //
        if (AndroidBackKeyManager.HasInstance)
        {
            AndroidBackKeyManager.Instance.StackPop(gameObject);
        }
    }

    public void Update()
    {
        //----------------------------------------
        // パッチの取得待ち
        //----------------------------------------
        // パッチの読み込みリクエストが入っていたらパッチの読み込みリクエストを行う.
        if (m_PatchUpdateRequestStep == EMAINMENU_PATCHUPDATE_REQ.REQUEST)
        {
            // @change Developer 2016/04/18 ダイアログ中のリクエストをチェックしないフラグを追加
            if (m_ResumePatchUpdateRequest == false ||                                  // kinshisi-nnhanozoku
                m_ResumePatchUpdateSeqLock == false ||
                (m_PatchReqChkDialog == false && Dialog.HasDialog() == true) ||        // 別ダイアログが開いていたらやらない.
                ServerApi.IsExists ||
                AssetBundler.HasAssetBundler() == true
                )
            {
                return;
            }

            // すでに現在読み込み終了待ちならロードリクエストを行わない.
            // CheckRequestFinishText == false ロード中.
            if (Patcher.Instance.IsLoaded == true)
            {
                Patcher.Instance.IsUpdateMoveTitle(
                    () =>
                    {
                        m_PatchUpdateRequestStep = EMAINMENU_PATCHUPDATE_REQ.GOTITLE;
                    },
                    () =>
                    {
                        m_PatchUpdateRequestStep = EMAINMENU_PATCHUPDATE_REQ.NONE;
                    });

                // パッチの完了待ち
                m_PatchUpdateRequestStep = EMAINMENU_PATCHUPDATE_REQ.WAIT;
            }
            else
            {
                // 読み込み中なのでそこで終了
                m_PatchUpdateRequestStep = EMAINMENU_PATCHUPDATE_REQ.NONE;
            }
        }
    }

    /// ==================== PLAYMAKER ACTION START ==================================
    public void OnReady()
    {
        MainMenuManagerFSM.Instance.SendFsmNextEvent();
    }

    void OnInTutorial()
    {
#if BUILD_TYPE_DEBUG
        if (DebugOption.Instance.tutorialDO.skip)
        {
            MainMenuManagerFSM.Instance.SendFsmEvent("SKIP_TUTORIAL");
            return;
        }
#endif

        TutorialPart part = TutorialManager.GetNextTutorialPart();
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL SceneMainMenu#OnInTutorial:" + part.ToString());
#endif
        if (part == TutorialPart.LAST ||
            part == TutorialPart.NONE)
        {
            if (TutorialManager.IsExists)
            {
                TutorialManager.Instance.Destroy();
            }
            MainMenuManagerFSM.Instance.SendFsmNegativeEvent();
            return;
        }

        MainMenuManagerFSM.Instance.SendFsmPositiveEvent();
    }

    void OnTutorialNext()
    {
        TutorialPart part = TutorialManager.GetNextTutorialPart();
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL SceneMainMenu#OnTutorialNext:" + part.ToString());
#endif
        if (!TutorialManager.IsExists)
        {
            TutorialManager.Create();
        }
        MainMenuManagerFSM.Instance.SendFsmNextEvent();
        this.ExecuteLater(
            0,
            () =>
            {
                TutorialFSM.Instance.SendFsmNextEvent();
            });
    }

    public IEnumerator OnWaitForSeqAllReady()
    {
        bool bAllSeqStartOK = false;
        while (!bAllSeqStartOK)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("mainMenuPageParamDict count:" + mainMenuPageParamDict.Count);
#endif
            bAllSeqStartOK = true;

            foreach (MainMenuPageParam p in mainMenuPageParamDict.NonNullValues())
            {
                if (p.m_MainMenuSeq == null)
                {
#if BUILD_TYPE_DEBUG
                    Debug.Log("OnSeqAllReady Wait:" + p);
#endif
                    continue;
                }

                if (!p.m_MainMenuSeq.m_MainMenuSeqStartOK)
                {
#if BUILD_TYPE_DEBUG
                    Debug.Log("OnSeqAllReady Wait:" + p.m_MainMenuSeq.name);
#endif
                    bAllSeqStartOK = false;
                }
            }

            yield return null;
        }

        //フェードイン
        SceneCommon.Instance.StartFadeIn();

        MainMenuManagerFSM.Instance.SendFsmNextEvent();
    }

    void OnStartMenuBGM()
    {
        //BGM再生
        SoundUtil.PlayBGM(BGMManager.EBGM_ID.eBGM_2_1, false);

        MainMenuManagerFSM.Instance.SendFsmNextEvent();
    }

    void OnIsServerConnectOK()
    {
        if (ServerDataParam.m_ServerConnectOK)
        {
            MainMenuManagerFSM.Instance.SendFsmPositiveEvent();
            return;
        }

        MainMenuManagerFSM.Instance.SendFsmNegativeEvent();
    }

    void OnAddSwitchRequest_Home()
    {
        AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, true, false);
    }

    public IEnumerator OnFadeOutBefore()
    {
        while (!PageSwitchSeqOldFadeOutBefore())
        {
            yield return null;
        }

        MainMenuHeader.UnderMsgRequest("");

        //フェード開始処理
        MainMenuSwitchReq cSwitchReq = PeekSwitchRequest();

        if (cSwitchReq == null)
        {
            MainMenuManagerFSM.Instance.SendFsmNextEvent();
            yield break;
        }


        //--------------------------------
        // フェードアウト後にリクエストページのフェードインを実行することを期待する
        //--------------------------------
        if (mainMenuPageParamDict.ContainsKey(WorkSwitchPageNow)
            && mainMenuPageParamDict[WorkSwitchPageNow] != null
            && mainMenuPageParamDict[WorkSwitchPageNow].m_MainMenuSeq != null)
            mainMenuPageParamDict[WorkSwitchPageNow].m_MainMenuSeq.ClosePage(cSwitchReq.m_SwitchRequstFast);



        //--------------------------------
        // リンカーの有無によってフェード呼出し
        //--------------------------------
        if (cSwitchReq.m_SwitchRequstLinkerDraw == false)
        {
            HideFooter();
        }
        SetMenuFooterActive(cSwitchReq.m_SwitchRequstLinkerActive);

        //--------------------------------
        //	ヘッダーオン/オフ切り替え
        //--------------------------------
        SetMenuHeaderDraw(cSwitchReq.m_SwitchRequstLinkerHeaderDraw);
        SetMenuHeaderActive(cSwitchReq.m_SwitchRequstLinkerHeaderActive);

        //--------------------------------
        //	戻るボタンオン/オフ切り替え
        //--------------------------------
        SetMenuReturn(cSwitchReq.m_SwitchRequstReturn);

        //--------------------------------
        // 現在開いているページを設定
        // ※フェード中は例外コード
        //--------------------------------
        m_WorkSwitchPagePrev = m_WorkSwitchPageNow;
        m_WorkSwitchPageNow = MAINMENU_SEQ.SEQ_MAX;

        MainMenuManagerFSM.Instance.SendFsmNextEvent();
    }

    public IEnumerator OnFadeOutWait()
    {
        while (!PageSwitchSeqOldFadeOut())
        {
            yield return null;
        }

        MainMenuManagerFSM.Instance.SendFsmNextEvent();
    }

    public IEnumerator OnFadeOutAfter()
    {
        while (!PageSwitchSeqOldFadeOutAfter())
        {
            yield return null;
        }

        if (m_WorkSwitchPagePrev < MAINMENU_SEQ.SEQ_MAX)
        {
            if (mainMenuPageParamDict[m_WorkSwitchPagePrev].m_MainMenuSeq != null)
                mainMenuPageParamDict[m_WorkSwitchPagePrev].m_MainMenuSeq.OnPageFinished();
        }

        //上書き禁止アイコン設定をクリアする
        UnitIconImageProvider.Instance.ResetAll();

        MainMenuManagerFSM.Instance.SendFsmNextEvent();
    }

    void OnHasSwitchRequest()
    {
        if (HasSwitchRequest)
        {
            MainMenuManagerFSM.Instance.SendFsmPositiveEvent();
            return;
        }

        MainMenuManagerFSM.Instance.SendFsmNegativeEvent();
    }

    void OnUserStateUpdate()
    {
        if (TutorialManager.IsExists)
        {
            MainMenuManagerFSM.Instance.SendFsmNextEvent();
            return;
        }

        int m_PresentRequestWaitCount = UserDataAdmin.Instance.m_PresentRequestWaitCount;
        m_PresentRequestWaitCount++;

        if (m_PresentRequestWaitCount < Patcher.Instance.GetPresentRequestWaitNum())
        {
            UserDataAdmin.Instance.m_PresentRequestWaitCount = m_PresentRequestWaitCount;

            MainMenuManagerFSM.Instance.SendFsmNextEvent();

            return;
        }

        UserDataAdmin.Instance.m_PresentRequestWaitCount = 0;

        ServerDataUtilSend.SendPacketAPI_PresentListGet()
                .setSuccessAction(_data =>
                {
                    //----------------------------------------
                    // 情報反映
                    //----------------------------------------
                    var list = UserDataAdmin.PresentListClipTimeLimit(_data.GetResult<RecvPresentListGet>().result.present);
                    UserDataAdmin.Instance.m_StructPresentList = list;

                    MainMenuManagerFSM.Instance.SendFsmNextEvent();
                })
                .setErrorAction(_data =>
                {
                    MainMenuManagerFSM.Instance.SendFsmNextEvent();
                })
                .SendStart();
    }

    public IEnumerator OnFadeInBefore()
    {
        while (!PageSwitchSeqNewFadeInBefore())
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("wait for PageSwitchSeqNewFadeInBefore");
#endif
            yield return null;
        }


        MainMenuSwitchReq cSwitchReq = switchRequestQueue.DequeueOrNull();

        MAINMENU_SEQ nOpenPageID = MAINMENU_SEQ.SEQ_HOME_MENU;
        if (cSwitchReq != null)
        {
            nOpenPageID = (MAINMENU_SEQ)cSwitchReq.m_SwitchRequstNextSeq;
        }
        else
        {
            //--------------------------------
            // セーフティのためアクティブなページがないなら強制的にホーム画面をアクティブ化
            //--------------------------------
#if BUILD_TYPE_DEBUG
            Debug.Log("Switch Page Request Error!");
#endif
        }


        //-------------------------------------------------
        // チュートリアルダイアログの表示
        //-------------------------------------------------
        if (!TutorialManager.IsExists)
        {
            MAINMENU_SEQ mainmenuSeq = (MAINMENU_SEQ)nOpenPageID;
            TutorialDialog.FLAG_TYPE flagType = mainmenuSeq.ConvertToTutorialFlagType();


            if (flagType != TutorialDialog.FLAG_TYPE.NONE && flagType != TutorialDialog.FLAG_TYPE.ALL)
            {
                bool isTutorial = (LocalSaveManagerRN.Instance.GetIsShowTutorialDialog(flagType) == false);
                if (isTutorial)
                {
#if BUILD_TYPE_DEBUG
                    Debug.LogError(string.Format("チュートリアルを表示する FLAG_TYPE:{0}", flagType.ToString()));
#endif
                    TutorialDialog.Create().SetTutorialType(flagType).Show(() =>
                    {
                        isTutorial = false;
                    });

                }
                while (isTutorial)
                {
#if BUILD_TYPE_DEBUG
                    Debug.Log("IN_TUTORIAL");
#endif
                    yield return null;
                }
            }
        }

        //--------------------------------
        // 開く対象の場合には有効化操作呼出し
        //--------------------------------
        MainMenuPageParam target = mainMenuPageParamDict[nOpenPageID];

        target.m_MainMenuSeq.PageSwitchTriger(cSwitchReq.m_SwitchRequstFast);

        //----------------------------------------
        // 参照していたテクスチャやメッシュ等を完全破棄
        // 切り替えのたびに呼ぶのは少し重すぎる処理だけど、単体破棄が存在しないので…
        //----------------------------------------
        if (target.m_RefreshResource)
        {
            UnityUtil.ResourceRefresh();
        }

        //--------------------------------
        // リンカーの有無によってフェード呼出し
        //--------------------------------
        if (cSwitchReq.m_SwitchRequstLinkerDraw)
        {
            if (m_MainMenuFooter == null)
            {
                ShowFooter();
            }
        }
        SetMenuFooterActive(cSwitchReq.m_SwitchRequstLinkerActive);

        //--------------------------------
        //	ヘッダーオン/オフ切り替え
        //--------------------------------
        SetMenuHeaderDraw(cSwitchReq.m_SwitchRequstLinkerHeaderDraw);

        //--------------------------------
        //	ヘッダーサブタブ設定
        //--------------------------------
        m_MainMenuSubTab.SetSubTab((MAINMENU_SEQ)nOpenPageID);


        m_WorkSwitchPageNow = (MAINMENU_SEQ)nOpenPageID;

        //--------------------------------
        // ミッションのマーキー表示設定
        //--------------------------------
        setMissionMessage();

        MainMenuManagerFSM.Instance.SendFsmNextEvent();
    }

    public IEnumerator OnFadeInWait()
    {
        while (!PageSwitchSeqNewFadeIn())
        {
            yield return null;
        }

        MainMenuManagerFSM.Instance.SendFsmNextEvent();
    }

    public IEnumerator OnFadeInAfter()
    {
        while (!PageSwitchSeqNewFadeInAfter())
        {
            yield return null;
        }

        setSubtitle();

        // TODO : MainMenuManagerを整理したらシーン開始(切り替え演出あけ)に移動させる
        // 注) MainMenuManagerのフェードイン処理がAnimationを邪魔するのでこのタイミングより前だとアニメーションが動かない
        // ホーム画面用
        EffectProcessor.Instance.PlayOrderly(
            new List<string>
            {
                "HomeMenu",
                "MenuBanner",
                "MainMenuFooter",
            });
        // バトル→ホーム画面用(フッターだけinitializeが走らないので TODO : シーンをまたいでフッター再表示時にInitialize()走るようにする)
        EffectProcessor.Instance.PlayOrderly(
            new List<string>
            {
                "HomeMenu",
                "MenuBanner"
            });
        // クエスト画面用
        EffectProcessor.Instance.PlayOrderly(
            new List<string>
            {
                "MenuBanner"
            });
        // ガチャ画面(ガチャ演出から戻ってきたとき)用
        EffectProcessor.Instance.PlayOrderly(
            new List<string>
            {
                "MainMenuFooter"
            });
        // クエスト詳細画面用
        EffectProcessor.Instance.PlayOrderly(
            new List<string>
            {
                "MainMenuQuestDetail"
            });
        // パーティ編成画面用
        EffectProcessor.Instance.PlayOrderly(
            new List<string>
            {
                "MainMenuUnitPartySelect"
            });
        m_SwitchPageFlag = false;

        MainMenuManagerFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    /// GlobalMenuからの通知
    /// </summary>
    public void UpdateUserStatusFromGlobalMenu(GLOBALMENU_SEQ selectseq = GLOBALMENU_SEQ.NONE)
    {
        GlobalMenuForMainMenu globalmenu = GlobalMenuForMainMenu.GetGlobalMainMenu();
        if (globalmenu != null)
        {
            GLOBALMENU_SEQ currentSeq = (selectseq != GLOBALMENU_SEQ.NONE) ? selectseq : globalmenu.CurrentSeq;
            MainMenuSeq seq = mainMenuPageParamDict[m_WorkSwitchPageNow].m_MainMenuSeq;
            seq.PageUpdateStatusFromMainMenu(currentSeq);
        }
    }

    private bool m_SwitchPageFlag = false;


    /// <summary>
    /// 初期パーティ設定チェック
    /// </summary>
    public void OnCheckFirstParty()
    {
        Debug.LogError("SELECT:" + UserDataAdmin.Instance.m_StructPlayer.renew_first_select_num);

        if (UserDataAdmin.Instance.m_StructPlayer.renew_first_select_num != -1 ||
            UserDataAdmin.Instance.m_StructPlayer.first_select_num != -1)
        {
            //選択済み
            MainMenuManagerFSM.Instance.SendFsmNextEvent();
            return;
        }

        //メニューBGM再生開始
        SoundUtil.PlayBGM(BGMManager.EBGM_ID.eBGM_2_1, false);

        //チュートリアルスキップ用ヒーロー選択ダイアログ
        Dialog selectDialog = Dialog.Create(DialogType.DialogMenu);
        List<DialogMenuItem> menuList = new List<DialogMenuItem>();
        menuList.Add(new DialogMenuItem("10", "カズシ", OnSelectDefaultParty));
        menuList.Add(new DialogMenuItem("20", "ココロ", OnSelectDefaultParty));
        menuList.Add(new DialogMenuItem("30", "シンク", OnSelectDefaultParty));
        menuList.Add(new DialogMenuItem("40", "リアン", OnSelectDefaultParty));
        menuList.Add(new DialogMenuItem("50", "ムサシ", OnSelectDefaultParty));
        menuList.Add(new DialogMenuItem("60", "ミカ", OnSelectDefaultParty));
        selectDialog.SetMeneList(menuList);
        selectDialog.SetDialogText(DialogTextType.Title, "ヒーロー選択");
        selectDialog.SetDialogObjectEnabled(DialogObjectType.OneButton, false);
        selectDialog.Show();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="_item"></param>
    private void OnSelectDefaultParty(DialogMenuItem _item)
    {
        uint default_party_id = (uint)_item.Title.ToInt(10);
        ServerDataUtilSend.SendPacketAPI_SkipTutorial(default_party_id).
            setSuccessAction(
                _data =>
                {
                    UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvSkipTutorial>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                    UserDataAdmin.Instance.m_StructHeroList = _data.GetResult<RecvSkipTutorial>().result.hero_list;
                    UserDataAdmin.Instance.ConvertPartyAssing();
                    MainMenuManagerFSM.Instance.SendFsmNextEvent();
                }).
            setErrorAction(
                _data =>
                {
                    MainMenuManagerFSM.Instance.SendFsmNextEvent();
                }).
            SendStart();
    }

    /// <summary>
    /// ログインパック取得
    /// タイトル、日付変更時のみ動作する
    /// 呼ばれることが多い場合はここを呼び出すようにしないこと
    /// 何か１回行ったら呼び出すなどしてはいけない
    /// </summary>
    public void OnSendLoginPack()
    {
        bool bGetLogin = true;
        bool bGetFriend = true;
        bool bGetHelper = true;
        bool bGetPresent = true;

        //クエストリザルトでこの値をみてログインパック取得しているか判別している。
        s_LastLoginTime = TimeUtil.ConvertLocalTimeToServerTime(TimeManager.Instance.m_TimeNow);

        //日跨ぎAPIを呼ぶ時間を設定
        DateTime nextDay = TimeManager.Instance.m_TimeNow.AddDays(1);
        MainMenuParam.m_DayStraddleTime = new DateTime(nextDay.Year,
                                                            nextDay.Month,
                                                            nextDay.Day,
                                                            0,
                                                            0,
                                                            0);
        //選択保存情報リセット
        MainMenuParam.ResetSaveSelect();

        //----------------------------------------
        // 現状の適用ブーストを設定
        //----------------------------------------
        MainMenuParam.m_BeginnerBoost = MasterDataUtil.GetMasterDataBeginnerBoost();
        UserDataAdmin.Instance.Player.isUpdateItem = true;

        ServerDataUtilSend.SendPacketAPI_LoginPack(bGetLogin, bGetFriend, bGetHelper, bGetPresent).
            setSuccessAction(
                _data =>
                {
                    MainMenuUtil.setupLoginPack(_data.GetResult<RecvLoginPack>().result);

                    MainMenuManagerFSM.Instance.SendFsmNextEvent();
                }).
            setErrorAction(
                _data =>
                {
                    MainMenuManagerFSM.Instance.SendFsmNextEvent();
                }).
            SendStart();
    }

    /// <summary>
    /// 緊急告知チェック
    /// </summary>
    public void OnCheckEmergency()
    {
        string _message = MasterDataUtil.GetInformationMessage(MasterDataDefineLabel.InfomationType.EMERGENCY, false);
        if (_message == string.Empty)
        {
            MainMenuManagerFSM.Instance.SendFsmNextEvent();
            return;
        }
        Dialog _newDialog = Dialog.Create(DialogType.DialogScrollInfo);
        _newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("kinkyu_dialog_title"));
        _newDialog.AddScrollInfoText(_message);
        _newDialog.SetDialogEvent(
            DialogButtonEventType.OK,
            () =>
            {
                MainMenuManagerFSM.Instance.SendFsmNextEvent();
            });
        _newDialog.Show();
    }

    /// <summary>
    /// お知らせチェック
    /// </summary>
    public void OnCheckInfomation()
    {
        string _message = MasterDataUtil.GetInformationMessage(MasterDataDefineLabel.InfomationType.NORMAL);
        if (_message == string.Empty)
        {
            MainMenuManagerFSM.Instance.SendFsmNextEvent();
            return;
        }
        Dialog _newDialog = Dialog.Create(DialogType.DialogScrollInfo);
        _newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("mm31q_title"));
        _newDialog.AddScrollInfoText(_message);

        _newDialog.SetDialogEvent(
            DialogButtonEventType.OK,
            () =>
            {
                MainMenuManagerFSM.Instance.SendFsmNextEvent();
            });
        _newDialog.Show();
    }

    void OnExistsLoginBonus()
    {
        if (LoginBonus.isCheck() == true)
        {
            MainMenuManagerFSM.Instance.SendFsmPositiveEvent();
            return;
        }
        MainMenuManagerFSM.Instance.SendFsmNegativeEvent();
    }

    void OnGoHome()
    {
        //ホームメニュー画面へ遷移
        AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false);
        MainMenuManagerFSM.Instance.SendFsmNextEvent();
    }

    void OnGoLoginBonus()
    {
        //ログインボーナス画面へ遷移
        AddSwitchRequest(MAINMENU_SEQ.SEQ_LOGIN_BONUS, false, false);
        MainMenuManagerFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    ///
    /// </summary>
    public void OnCheckLoginPack()
    {
        //----------------------------------------
        // 現状の適用ブーストを設定
        //----------------------------------------
        MainMenuParam.m_BeginnerBoost = MasterDataUtil.GetMasterDataBeginnerBoost();

        //メニュフラグ更新
        UserDataAdmin.Instance.UpdateHeader();

        //残りヘルパーが１５以下になったらログインパックで補充
        if (s_LastLoginTime != 0 && //中断から再開した場合は補充処理しない。
            UserDataAdmin.Instance.m_StructHelperList != null &&
            UserDataAdmin.Instance.CheckHelperCount() <= 15)
        {
            ServerDataUtilSend.SendPacketAPI_LoginPack(false, false, true, false).
                setSuccessAction(
                    _data =>
                    {
                        RecvLoginPackValue cLoginPack = _data.GetResult<RecvLoginPack>().result;
                        //----------------------------------------
                        // 助っ人情報保持
                        //----------------------------------------
                        if (cLoginPack.result_helper != null
                        && cLoginPack.result_helper.friend != null
                        )
                        {
                            UserDataAdmin.Instance.m_StructHelperList = UserDataAdmin.FriendListClipMe(cLoginPack.result_helper.friend);
                        }


                        MainMenuManagerFSM.Instance.SendFsmNextEvent();
                    }).
                setErrorAction(
                    _data =>
                    {
                        MainMenuManagerFSM.Instance.SendFsmNextEvent();
                    }).
                SendStart();

        }
        else
        {
            MainMenuManagerFSM.Instance.SendFsmNextEvent();
        }
    }

    /// <summary>
    /// クエストリザルトチェック
    /// </summary>
    public void OnCheckQuestResult()
    {
        if (SceneGoesParam.Instance.m_SceneGoesParamToMainMenu == null)
        {
            //クエストリザルトなし
            MainMenuManagerFSM.Instance.SendFsmNextEvent();
            return;
        }

        //リザルト通信へ
        AddSwitchRequest(MAINMENU_SEQ.SEQ_RESULT_SERVER_SEND, false, false);
    }

    /// <summary>
    /// クエストリタイアチェック
    /// </summary>
    public void OnCheckQuestRetire()
    {
        if (SceneGoesParam.Instance.m_SceneGoesParamToMainMenuRetire == null)
        {
            //リタイア情報がない（本来はありえない状態）ときはHOMEへ
            AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false);
            return;
        }

        //リタイア通信へ
        AddSwitchRequest(MAINMENU_SEQ.SEQ_RESULT_RETIRE, false, false);
    }

    /// <summary>
    /// 前回のシーンから遷移を選択
    /// </summary>
    public void OnCheckLastScene()
    {
        switch (SceneCommon.Instance.LastSceneType)
        {
            case SceneType.SceneTitle:
                if (SceneGoesParam.Instance.m_SceneGoesParamToMainMenu == null)
                    MainMenuManagerFSM.Instance.SendFsmEvent("LAST_SCENE_TITLE");
                else
                    MainMenuManagerFSM.Instance.SendFsmEvent("LAST_SCENE_GAME");
                break;
            case SceneType.SceneQuest2:
                MainMenuManagerFSM.Instance.SendFsmEvent("LAST_SCENE_GAME");
                break;
#if BUILD_TYPE_DEBUG
            case SceneType.NONE:
                //単体起動したとき
                MainMenuManagerFSM.Instance.SendFsmEvent("LAST_SCENE_TITLE");
                break;
#endif
            default:
                //タイトルやゲーム以外から来た（本来はありえない状態）ときはHOMEへ
                AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false);
                break;
        }
    }

    /// ==================== PLAYMAKER ACTION END ==================================

    //----------------------------------------------------------------------------
    /*!
        @brief	上部プレイヤー情報の表示/非表示
    */
    //----------------------------------------------------------------------------
    public void SetMenuHeaderDraw(bool enable)
    {
        UnityUtil.SetObjectEnabledOnce(m_MainMenuHeaderObj, enable);
        UnityUtil.SetObjectEnabled(SceneObjReferMainMenu.Instance.m_TopDecoNoMenu, !enable);
        UnityUtil.SetObjectEnabled(SceneObjReferMainMenu.Instance.m_TopDeco, enable);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	上部プレイヤー情報の判定ON/OFF
	*/
    //----------------------------------------------------------------------------
    public void SetMenuHeaderActive(bool enable)
    {
        if (m_MainMenuHeader != null)
        {
            m_MainMenuHeader.SetHeaderActive(enable);
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	下部ボタンの判定ON/OFF
    */
    //----------------------------------------------------------------------------
    public void SetMenuFooterActive(bool enable)
    {
        if (m_MainMenuFooter != null)
        {
            m_MainMenuFooter.setActiveFlag(enable);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	戻るボタンのON/OFF
	*/
    //----------------------------------------------------------------------------
    public void SetMenuReturn(bool enable)
    {
        if (m_MainMenuFooter != null)
        {
            if (enable && MainMenuParam.m_PageBack.Count == 0)
            {
                enable = false;
            }
            m_MainMenuFooter.IsActiveReturn = enable;

            //一旦登録を消す
            AndroidBackKeyManager.Instance.StackPop(m_MainMenuHeader.gameObject);

            if (enable)
            {
                AndroidBackKeyManager.Instance.StackPush(m_MainMenuHeader.gameObject, m_MainMenuFooter.OnSelectReturn);
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	戻るボタンのON/OFF(アクション設定)
	*/
    //----------------------------------------------------------------------------
    public void SetMenuReturnAction(bool enable, Action _action)
    {
        if (m_MainMenuFooter != null)
        {
            if (enable)
            {
                //設定
                m_MainMenuFooter.IsActiveReturn = true;
                m_MainMenuFooter.ReturnAction = _action;
                AndroidBackKeyManager.Instance.StackPush(m_MainMenuHeader.gameObject, m_MainMenuFooter.OnSelectReturn);
            }
            else
            {
                //解除
                m_MainMenuFooter.IsActiveReturn = false;
                m_MainMenuFooter.ReturnAction = null;
                AndroidBackKeyManager.Instance.StackPop(m_MainMenuHeader.gameObject);
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public void ShowFooter()
    {
        SceneObjReferMainMenu mainmenu = SceneObjReferMainMenu.Instance;
        if (mainmenu.m_MainMenuFooter != null)
        {
            m_MainMenuFooterObj = Instantiate(mainmenu.m_MainMenuFooter) as GameObject;
            m_MainMenuFooterObj.transform.SetParent(mainmenu.m_MainMenuBottomAnchor.transform, false);
            m_MainMenuFooter = m_MainMenuFooterObj.GetComponent<MainMenuFooter>();

            //--------------------------------
            // 最初にログイン系処理をやってる時にフェードしてるとこが見えてしまうので、
            // 違和感軽減のため生成時点で無効状態移行させておく
            //--------------------------------
            m_MainMenuFooter.Initialize();
            m_MainMenuFooter.GetModel().OnDisappeared += () =>
            {
                Destroy(m_MainMenuFooter);
                Destroy(m_MainMenuFooter.gameObject);
                m_MainMenuFooterObj = null;
                m_MainMenuFooter = null;
            };
        }
    }

    /// <summary>
    ///
    /// </summary>
    public void HideFooter()
    {
        if (m_MainMenuFooter == null)
            return;
        m_MainMenuFooter.setActiveFlag(false);
        m_MainMenuFooter.GetModel().Close();
    }

    /// <summary>
    ///
    /// </summary>
    public void ShowHeader()
    {
        SceneObjReferMainMenu mainmenu = SceneObjReferMainMenu.Instance;
        if (mainmenu.m_MainMenuHeader != null)
        {
            m_MainMenuHeaderObj = Instantiate(mainmenu.m_MainMenuHeader) as GameObject;
            m_MainMenuHeaderObj.transform.SetParent(mainmenu.m_MainMenuTopAnchor.transform, false);
            m_MainMenuHeader = m_MainMenuHeaderObj.GetComponent<MainMenuHeader>();
        }
    }

    /// <summary>
    ///
    /// </summary>
    public void HideHeader()
    {
        Destroy(m_MainMenuHeader);
        m_MainMenuHeaderObj = null;
        m_MainMenuHeader = null;
    }


    private GameObject m_ListItemPool = null;
    //----------------------------------------------------------------------------
    /*!
		@brief	ページの読み込み処理
		@note	シーン読み込み直後に全ページを一括で読み込むと待ち時間が長くなるので、必要なタイミングで読み込むように対応
		@retval[ true	]	:ページの準備完了
		@retval[ false	]	:ページの見た目複製と初期化中
	*/
    //----------------------------------------------------------------------------
    private bool PageInstantiate(MAINMENU_SEQ eMainMenuSeq)
    {
        MainMenuPageParam target = mainMenuPageParamDict[eMainMenuSeq];
        target.m_MainMenuType = eMainMenuSeq;

        //--------------------------------
        // インスタンスを生成してないなら生成実行
        //
        // オリジナルオブジェクト(Prefab)を複製して実体化する
        //--------------------------------
        if (target.m_GameObject == null)
        {
            target.m_GameObject = Instantiate(SceneObjReferMainMenu.Instance.m_MainMenuSeqObj) as GameObject;

            //--------------------------------
            // オブジェクトの親としてNGUI階層を設定
            //--------------------------------
            if (SceneObjReferMainMenu.Instance.m_MainMenuRoot)
            {
                target.m_GameObject.transform.SetParent(SceneObjReferMainMenu.Instance.m_MainMenuRoot.transform, false);
            }
            else
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("GroupAnchor Is Null!");
#endif
            }

            GameObject objCanvas = UnityUtil.GetChildNode(target.m_GameObject, "Canvas");
            if (objCanvas != null)
            {
                foreach (MasterMainMenuSeq.SequenceObj _obj in target.m_MasterMainMenuSeq.SequenceObjArray)
                {
                    string object_path = "Prefab/" + _obj.object_name;
                    GameObject originObj = Resources.Load(object_path) as GameObject;
                    if (originObj != null)
                    {
                        GameObject _insObj = Instantiate(originObj) as GameObject;
                        _insObj.transform.SetParent(objCanvas.transform, false);
                    }
                }
            }

            //--------------------------------
            // 初期化中のレイアウトを見られたくないので
            // 一時的に表示しないレイヤーに設定する
            //--------------------------------
            UnityUtil.SetObjectLayer(target.m_GameObject, LayerMask.NameToLayer("DRAW_CLIP"));

            //--------------------------------
            // 有効化しないとStart関数まで行きつかないらしい。
            //
            // 表示レイヤーが違うことで準備中のものが描画されることはないので一旦無条件で有効化しておく
            //--------------------------------
            UnityUtil.SetObjectEnabledOnce(target.m_GameObject, true);
        }

        if (target.m_MainMenuSeq == null)
        {
            if (!TutorialManager.IsExists)
            {
                m_ListItemPool = UnitGridView.CreateUnitListItem();
            }
            var mainMenuSeq = target.m_GameObject.AddComponent(target.m_OriginComponent) as MainMenuSeq;
            Debug.Assert(mainMenuSeq != null, "Failed to Add MainMenuSeq");
            target.m_MainMenuSeq = mainMenuSeq;
            mainMenuSeq.RegisterOnFadeOutFinishedCallback(() =>
            {
                if (MainMenuParam.FindPageBack(target.m_MainMenuType))
                {
                    return;
                }
                if (m_ListItemPool != null)
                {
                    poolItem(target.m_GameObject.transform);
                }
                Destroy(target.m_GameObject);
                target.m_GameObject = null;
                target.m_MainMenuSeq = null;
            });
            target.m_GameObject.name = target.m_MasterMainMenuSeq.SequenceName;
        }

        return true;
    }

    void poolItem(Transform root)
    {
        var children = root.Children();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == "UnitListItem")
            {
                children[i].SetParent(m_ListItemPool.transform);
            }
        }
        for (int i = 0; i < root.childCount; i++)
        {
            poolItem(root.GetChild(i));
        }
    }


    //----------------------------------------------------------------------------
    /*!
        @brief	ページ切り替え処理：古いページのフェードアウト前イベント
        @retval[ true	] : 処理完遂
        @retval[ false	] : 処理中
    */
    //----------------------------------------------------------------------------
    public bool PageSwitchSeqOldFadeOutBefore()
    {
        //--------------------------------
        // ページ閉じ前のイベント実行
        //--------------------------------

        if (m_WorkSwitchPageNow == MAINMENU_SEQ.SEQ_MAX)
        {
            return true;
        }

        MainMenuSeq seq = mainMenuPageParamDict[m_WorkSwitchPageNow].m_MainMenuSeq;

        if (seq == null)
        {
            return true;
        }

        m_ResumePatchUpdateSeqLock = false; //!< レジューム時のパッチリクエストフラグを無効にする.

        if (seq.PageSwitchEventDisableBefore())
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("PageSwitchEventDisableBefore:" + seq.name);
#endif
            return false;
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	ページ切り替え処理：古いページがフェードアウト処理中
        @retval[ true	] : 処理完遂
        @retval[ false	] : 処理中
    */
    //----------------------------------------------------------------------------
    public bool PageSwitchSeqOldFadeOut()
    {
        //--------------------------------
        // フェード処理の完遂待ち
        //--------------------------------
        bool bSwitchFinish = true;
        foreach (MainMenuPageParam p in mainMenuPageParamDict.NonNullValues())
        {
            if (p.m_MainMenuSeq == null)
            {
                continue;
            }

            bSwitchFinish &= p.m_MainMenuSeq.PageSwitchFinishCheck();
        }
        if (bSwitchFinish == false)
        {
            return false;
        }

        //--------------------------------
        // 処理完遂
        //--------------------------------
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	ページ切り替え処理：古いページのフェードアウト後イベント
        @note	この時点では画面上にページが一切表示されていない
        @retval[ true	] : 処理完遂
        @retval[ false	] : 処理中
    */
    //----------------------------------------------------------------------------
    public bool PageSwitchSeqOldFadeOutAfter()
    {
        //--------------------------------
        // ページ閉じ後のイベント実行
        //--------------------------------
        bool bEventPlaying = false;
        MainMenuSeq sequence = null;

        if (m_WorkSwitchPagePrev != MAINMENU_SEQ.SEQ_MAX)
        {
            sequence = mainMenuPageParamDict[m_WorkSwitchPagePrev].m_MainMenuSeq;
            if (sequence != null)
            {
                MainMenuSwitchReq cSwitchReq = PeekSwitchRequest();
                MAINMENU_SEQ eMextPageID = MAINMENU_SEQ.SEQ_HOME_MENU;
                if (cSwitchReq != null)
                {
                    eMextPageID = (MAINMENU_SEQ)cSwitchReq.m_SwitchRequstNextSeq;
                }

                if (mainMenuPageParamDict[m_WorkSwitchPagePrev].m_MainMenuSeq.PageSwitchEventDisableAfter(eMextPageID))
                {
                    bEventPlaying = true;
                }
            }
        }
        if (bEventPlaying == true)
        {
            return false;
        }

        if (sequence != null)
        {
            sequence.RunOnFadeOutFinishedCallback();
        }

        //--------------------------------
        // 処理完遂
        //--------------------------------
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	ページ切り替え処理：新しいページのフェードイン前イベント
        @note	この時点では画面上にページが一切表示されていない
        @retval[ true	] : 処理完遂
        @retval[ false	] : 処理中
    */
    //----------------------------------------------------------------------------
    public bool PageSwitchSeqNewFadeInBefore()
    {
        //--------------------------------
        // 次に開くページを取得
        //--------------------------------
        MainMenuSwitchReq cSwitchReq = PeekSwitchRequest();
        MAINMENU_SEQ nOpenPageID = MAINMENU_SEQ.SEQ_HOME_MENU;
        if (cSwitchReq != null)
        {
            nOpenPageID = (MAINMENU_SEQ)cSwitchReq.m_SwitchRequstNextSeq;
        }
        else
        {
            //--------------------------------
            // セーフティのためアクティブなページがないなら強制的にホーム画面をアクティブ化
            //--------------------------------
#if BUILD_TYPE_DEBUG
            Debug.Log("Switch Page Request Error!");
#endif
        }

        MainMenuPageParam target = mainMenuPageParamDict[nOpenPageID];

        //--------------------------------
        // 新しいページのオブジェクト存在チェック。
        // ページの実体がまだ作られていないなら実体作成
        //--------------------------------
        if (target.m_GameObject == null)
        {
            PageInstantiate((MAINMENU_SEQ)nOpenPageID);
#if BUILD_TYPE_DEBUG
            Debug.Log("target.m_GameObject is null");
#endif
            return false;
        }

        if (target.m_GameObject.layer != LayerMask.NameToLayer("DRAW_CLIP"))
        {
            UnityUtil.SetObjectEnabledOnce(target.m_GameObject, true);
        }

        if (target.m_MainMenuSeq != null &&
            target.m_MainMenuSeq.m_MainMenuSeqStartOK == false
        )
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("target.m_MainMenuSeq  ");
#endif
            return false;
        }

        if (target.m_MainMenuSeq == null)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("target.m_MainMenuSeq is null");
#endif
            return false;
        }

        bool bBack = false;
        if (cSwitchReq != null)
        {
            bBack = cSwitchReq.m_SwitchRequstBack;
        }

        return !target.m_MainMenuSeq.PageSwitchEventEnableBefore(bBack);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	ページ切り替え処理：新しいページのフェードイン処理中
        @retval[ true	] : 処理完遂
        @retval[ false	] : 処理中
    */
    //----------------------------------------------------------------------------
    public bool PageSwitchSeqNewFadeIn()
    {
        //--------------------------------
        // ページ切り替え実行中
        // 切り替えの完遂チェック
        //--------------------------------
        bool bSwitchFinish = true;
        foreach (MainMenuPageParam p in mainMenuPageParamDict.NonNullValues())
        {
            if (p.m_MainMenuSeq == null)
            {
                continue;
            }

            bool bRet = p.m_MainMenuSeq.PageSwitchFinishCheck();
            bSwitchFinish &= bRet;
        }

        if (bSwitchFinish == false)
        {
            return false;
        }

        //--------------------------------
        // 処理完遂
        //--------------------------------
        return true;
    }

    void OnShouldSendLoginPack()
    {
        if (MainMenuUtil.IsLoggedIn)
        {
            MainMenuManagerFSM.Instance.SendFsmNegativeEvent();
            return;
        }

        MainMenuManagerFSM.Instance.SendFsmPositiveEvent();
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	ページ切り替え処理：新しいページのフェードイン後イベント
        @retval[ true	] : 処理完遂
        @retval[ false	] : 処理中
    */
    //----------------------------------------------------------------------------
    public bool PageSwitchSeqNewFadeInAfter()
    {
        MainMenuPageParam target = mainMenuPageParamDict[m_WorkSwitchPageNow];

        if (m_WorkSwitchPageNow == MAINMENU_SEQ.SEQ_MAX)
        {
            return true;
        }

        if (target.m_MainMenuSeq == null)
        {
            return true;
        }

        m_ResumePatchUpdateSeqLock = true; //!< レジューム時のパッチリクエストフラグを有効にする.

        if (target.m_MainMenuSeq.PageSwitchEventEnableAfter())
        {
            return false;
        }
        return true;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief	次ページの置き換え
        @note	日付変更があった場合に、日時変更呼び出しに置き換える。
        @retval[MAINMENU_SEQ] : 置き換えページ
    */
    //----------------------------------------------------------------------------
    public MAINMENU_SEQ getReplaceNextPage(MAINMENU_SEQ eNextPage)
    {
#if UNITY_EDITOR
        //--------------------------------
        // エディタで作業中に日付変化によるタイトル戻しが面倒なので、
        // エディタでの起動時にはそのままページ遷移を認める
        //--------------------------------
        return eNextPage;
#else

        MAINMENU_SEQ eReplaceNextPage = eNextPage;
        try
        {
            //タイトルへ戻る時間のチェック
            if (MainMenuParam.m_ReturnTitleTime != DateTime.MaxValue &&
                TimeManager.Instance != null &&
                TimeManager.Instance.m_TimeNow != null)
            {
                //--------------------------------
                // 次ページがQuest , Friends , Scratch , Shop , Others , Unitsで
                // タイトルログイン時に設定された時間をすぎたらタイトルに戻す
                //--------------------------------
                if (
                    (TimeManager.Instance.m_TimeNow >= MainMenuParam.m_ReturnTitleTime)
                    &&
                    // 運営通知で 更新 or 一括取得を行った後、運営通知に戻る処理が有るのでチェック
                    (eNextPage == MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_STORY ||
                     eNextPage == MAINMENU_SEQ.SEQ_FRIEND_LIST ||
                     eNextPage == MAINMENU_SEQ.SEQ_GACHA_MAIN ||
                     eNextPage == MAINMENU_SEQ.SEQ_HOME_MENU)
                )
                {
                    //ひとまず日付変更ページへ
                    eReplaceNextPage = MAINMENU_SEQ.SEQ_DATE_CHANGE;
                    MainMenuParam.m_DateChangeType = DATE_CHANGE_TYPE.RETURN_TITLE; //タイトル戻るルート
                }
            }
            //０時またぎ通信をするかチェック
            if(MainMenuParam.m_DayStraddleTime != DateTime.MaxValue &&
                TimeManager.Instance != null &&
                TimeManager.Instance.m_TimeNow != null)
            {
                if (
                    (TimeManager.Instance.m_TimeNow >= MainMenuParam.m_DayStraddleTime)
                    &&
                    // 運営通知で 更新 or 一括取得を行った後、運営通知に戻る処理が有るのでチェック
                    (eNextPage == MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_STORY ||
                     eNextPage == MAINMENU_SEQ.SEQ_FRIEND_LIST ||
                     eNextPage == MAINMENU_SEQ.SEQ_GACHA_MAIN ||
                     eNextPage == MAINMENU_SEQ.SEQ_HOME_MENU)
                )
                {
                    //ひとまず日付変更ページへ
                    eReplaceNextPage = MAINMENU_SEQ.SEQ_DATE_CHANGE;
                    MainMenuParam.m_DateChangeType = DATE_CHANGE_TYPE.DAY_STRADDLE;//０時またぎAPIを呼ぶルート
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        return eReplaceNextPage;
#endif // UNITY_EDITOR
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替えリクエスト関連：リクエスト発行
	*/
    //----------------------------------------------------------------------------
    public bool AddSwitchRequest(MAINMENU_SEQ eNextPage, bool bFast, bool bBack, bool bNotBack = false)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL AddSwitchRequest:" + eNextPage + " bFast:" + bFast + " bBack:" + bBack + " bNotBack:" + bNotBack);
#endif
        if (eNextPage >= MAINMENU_SEQ.SEQ_MAX)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CANCEL");
#endif
            return false;
        }
        if (IsPageSwitch())
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CANCEL");
#endif
            return false;
        }
        MAINMENU_SEQ nNextPage = getReplaceNextPage(eNextPage);

        if (switchRequestQueue.Where(r => r.m_SwitchRequstNextSeq == (int)nNextPage).Count() > 0)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CANCEL");
#endif
            m_SwitchPageFlag = true;
            MainMenuManagerFSM.Instance.SendEvent_ChangeSeq();
            return false;
        }

#if BUILD_TYPE_DEBUG
        Debug.Log("AddSwitchRequest");
#endif

        if (eNextPage != MAINMENU_SEQ.SEQ_UNIT_PARTY_FORM)
        {
            MainMenuParam.m_PartyAssignPrevPage = 0;
        }

        MainMenuSwitchReq req = new MainMenuSwitchReq();
        // ここでエラーになる場合、MainMenuSeqのScriptableObjectがちゃんと設定されていない場合がある
        req.m_SwitchRequstNextSeq = (int)nNextPage;
        req.m_SwitchRequstFast = bFast;
        req.m_SwitchRequstBack = bBack;
        req.m_SwitchRequstLinkerDraw = ((mainMenuPageParamDict[nNextPage].m_LinkerState & MainMenuPageParam.LINKER_DRAW) != 0);
        req.m_SwitchRequstLinkerActive = ((mainMenuPageParamDict[nNextPage].m_LinkerState & MainMenuPageParam.LINKER_ACTIVE) != 0);
        req.m_SwitchRequstLinkerHeaderDraw = ((mainMenuPageParamDict[nNextPage].m_LinkerState & MainMenuPageParam.HEADER_DRAW) != 0);
        req.m_SwitchRequstLinkerHeaderActive = ((mainMenuPageParamDict[nNextPage].m_LinkerState & MainMenuPageParam.HEADER_ACTIVE) != 0);
        req.m_SwitchRequstReturn = ((mainMenuPageParamDict[nNextPage].m_LinkerState & MainMenuPageParam.RETURN) != 0);

        if (TutorialManager.IsExists)
        {
            req.m_SwitchRequstLinkerHeaderActive = false;
            req.m_SwitchRequstLinkerActive = false;
        }

        switchRequestQueue.Enqueue(req);

        //戻るボタンがあるページに行くときは現在のページをもどるページに追加する
        if (req.m_SwitchRequstReturn)
        {
            //戻るボタンでもどってきた＆戻ってこないでフラグの時は追加しない
            if (!bBack &&
                !bNotBack)
            {
                MainMenuParam.m_PageBack.Push(m_WorkSwitchPageNow);
            }
        }
        else
        {
            //戻るボタンがないページにいったらすべてクリアする

            MAINMENU_SEQ[] array_sequence = MainMenuParam.m_PageBack.ToArray();

            MainMenuParam.m_PageBack.Clear();

            for (int i = 0; i < array_sequence.Count(); i++)
            {
                MainMenuSeq sequence = mainMenuPageParamDict[array_sequence[i]].m_MainMenuSeq;
                if (sequence != null)
                {
                    sequence.RunOnFadeOutFinishedCallback();
                }
            }
        }

        m_SwitchPageFlag = true;

        MainMenuManagerFSM.Instance.SendEvent_ChangeSeq();
        return true;
    }

    public bool HasSwitchRequest
    {
        get
        {
            return switchRequestQueue.Count > 0;
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	ページ切り替えリクエスト関連：リクエスト取得
    */
    //----------------------------------------------------------------------------
    public MainMenuSwitchReq PeekSwitchRequest()
    {
        if (!HasSwitchRequest)
        {
            return null;
        }

        return switchRequestQueue.Peek();
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	メニュー操作禁止状態チェック
    */
    //----------------------------------------------------------------------------
    public bool CheckMenuControlNG()
    {
        //--------------------------------
        // シーン切り替え中ならNG
        //--------------------------------
        if (SceneCommon.Instance != null &&
            SceneCommon.Instance.IsLoadingScene
        )
        {
            return true;
        }

        //--------------------------------
        // ページ切り替え中ならNG
        //--------------------------------
        if (m_WorkSwitchState != SWITCH_STATE_REQUEST_CHK)
        {
            return true;
        }

        //--------------------------------
        // ページ切り替えリクエストが存在するならNG
        //--------------------------------
        if (PeekSwitchRequest() != null)
        {
            return true;
        }

        return false;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief	ページアクティブチェック
    */
    //----------------------------------------------------------------------------
    public bool CheckPageActive(MAINMENU_SEQ ePageID)
    {
        return UnityUtil.ChkObjectEnabled(mainMenuPageParamDict[ePageID].m_GameObject);
    }

    /// <summary>
    /// グローバルメニュー閉じる
    /// </summary>
    public void CloseGlobalMenu()
    {
        m_MainMenuHeader.CloseGlobalMenu();
    }

    /// <summary>
    /// ユニット詳細画面開く
    /// </summary>
    /// <returns></returns>
    private UnitDetailInfo OpenUnitDetailInfoBase(bool _closeActive = true,
                                                  bool _charaScreen = false,
                                                  Action closeAction = null)
    {
        if (UnitDetailInfo.GetUnitDetailInfo() != null  // すでに表示中？
        || IsPageSwitch()                               // ページ切り替え中？
        || m_MainMenuHeader.HasGlobalMenu()             // グローバルメニュー表示中？
        )
        {
            return null;
        }

        //いま開いているページ＆メニューを非表示
        UnityUtil.SetObjectEnabledOnce(mainMenuPageParamDict[m_WorkSwitchPageNow].m_GameObject, false);

        //サブタブ非表示
        UnityUtil.SetObjectEnabledOnce(m_MainMenuSubTab.gameObject, false);

        //ヘッダーを非アクティブに
        SetMenuHeaderActive(false);

        //戻るボタンフラグを保存
        if (m_MainMenuFooter != null)
        {
            m_isUnitDitailSaveReturn = m_MainMenuFooter.IsActiveReturn;
            //フッターを非アクティブに
            m_MainMenuFooter.setActiveFlag(false);
        }

        //ダイアログをすべて非表示
        Dialog.SetDialogEnableAll(false);

        m_isUnitDitailCloseActive = _closeActive;

        var info = UnitDetailInfo.Create(SceneObjReferMainMenu.Instance.m_MainMenuGroupCamera.GetComponent<Camera>(), _charaScreen);
        if (info == null)
        {
            if (closeAction != null)
            {
                closeAction();
            }
        }

        //クローズするときに呼ばれる関数を設定
        if (closeAction != null)
        {
            info.SetCloseAction(closeAction);
        }

        return info;
    }

    /// <summary>
    /// ユニット詳細画面開く
    /// </summary>
    /// <returns></returns>
    public UnitDetailInfo OpenUnitDetailInfo(bool _closeActive = true, bool _charaScreen = false)
    {
        var info = OpenUnitDetailInfoBase(_closeActive, _charaScreen, CloseUnitDetailInfo);

        if (info == null)
        {
            return null;
        }

        MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("undefo_description"), true);
        MainMenuHeader.IsViewOtherUnderMessage(false);

        return info;
    }

    /// <summary>
    /// ユニット詳細画面開く
    /// </summary>
    /// <returns></returns>
    public UnitDetailInfo OpenUnitDetailInfoTutorial(bool _closeActive = true)
    {
        var info = OpenUnitDetailInfoBase(_closeActive, false, CloseUnitDetailInfoTutorial);

        if (info == null)
        {
            return null;
        }

        MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("undefo_description"), true);

        return info;
    }

    /// <summary>
    /// ユニット詳細画面開く（フレンド）
    /// </summary>
    /// <param name="_friend"></param>
    public void OpenUnitDetailInfoFriend(PacketStructFriend _friend)
    {
        var info = OpenUnitDetailInfo();
        if (info != null)
        {
            if (_friend.unit_link.id != 0)
            {
                info.SetUnit(_friend.unit, _friend.unit_link);
            }
            else
            {
                info.SetUnit(_friend.unit, null);
            }

            info.IsViewCharaCount = false;
        }
    }

    /// <summary>
    /// ユニット詳細画面開く（プレイヤー）
    /// </summary>
    /// <param name="_unit"></param>
    public UnitDetailInfo OpenUnitDetailInfoPlayer(PacketStructUnit _unit, bool _closeActive = true, bool _isViewCharaCount = true)
    {
        var info = OpenUnitDetailInfo(_closeActive);
        if (info != null)
        {
            PacketStructUnit _subUnit = UserDataAdmin.Instance.SearchLinkUnit(_unit);
            info.SetUnit(_unit, _subUnit);
            info.IsViewCharaCount = _isViewCharaCount;
        }

        return info;
    }

    /// <summary>
    /// ユニット詳細画面開く（プレイヤー）
    /// </summary>
    /// <param name="_unit"></param>
    public void OpenUnitDetailInfoPlayerTutorial(PacketStructUnit _unit, bool _closeActive = true)
    {
        var info = OpenUnitDetailInfoTutorial(_closeActive);
        if (info != null)
        {
            PacketStructUnit _subUnit = UserDataAdmin.Instance.SearchLinkUnit(_unit);
            info.SetUnit(_unit, _subUnit);
            info.IsViewCharaCount = true;
        }

        SetMenuHeaderDraw(true);
        SetMenuHeaderActive(false);
    }

    /// <summary>
    /// ユニット詳細画面開く（プレイヤー）
    /// </summary>
    /// <param name="_unit"></param>
    public UnitDetailInfo OpenUnitDetailInfoPlayer(PacketStructUnit _unit, PacketStructUnit _subUnit, bool _closeActive = true, bool _isViewCharaCount = true)
    {
        var info = OpenUnitDetailInfo(_closeActive);
        if (info != null)
        {
            if (_subUnit != null && _subUnit.id > 0 && _unit.link_info == (int)CHARALINK_TYPE.CHARALINK_TYPE_BASE)
            {
                info.SetUnit(_unit, _subUnit);
            }
            else
            {
                info.SetUnit(_unit, null);
            }
            info.IsViewCharaCount = _isViewCharaCount;
        }
        return info;
    }

    /// <summary>
    /// ユニット詳細画面開く（カタログ）
    /// </summary>
    /// <param name="_id"></param>
    public UnitDetailInfo OpenUnitDetailInfoCatalog(uint _id, bool _closeActive = true, bool _isViewCharaCount = true)
    {
        var info = OpenUnitDetailInfo(_closeActive);
        if (info != null)
        {
            info.SetCharaID(_id);
            info.IsViewCharaCount = _isViewCharaCount;
        }

        return info;
    }

    /// <summary>
    /// ユニット詳細画面閉じる
    /// </summary>
    public void CloseUnitDetailInfo()
    {
        //開いていたページ＆メニューを表示
        UnityUtil.SetObjectEnabledOnce(mainMenuPageParamDict[m_WorkSwitchPageNow].m_GameObject, true);
        //サブタブ表示
        UnityUtil.SetObjectEnabledOnce(m_MainMenuSubTab.gameObject, true);

        if (m_isUnitDitailCloseActive)
        {
            //
            if (m_MainMenuFooter != null)
            {
                m_MainMenuFooter.IsActiveReturn = m_isUnitDitailSaveReturn;
                m_MainMenuFooter.setActiveFlag(true);
            }

            //ヘッダーをアクティブへ
            SetMenuHeaderActive(true);
        }

        //ダイアログをすべて表示
        Dialog.SetDialogEnableAll(true);

        MainMenuHeader.UnderMsgRequest("");
        setMissionMessage();
    }

    /// <summary>
    /// ユニット詳細画面閉じる
    /// </summary>
    public void CloseUnitDetailInfoTutorial()
    {
        //開いていたページ＆メニューを表示
        UnityUtil.SetObjectEnabledOnce(mainMenuPageParamDict[m_WorkSwitchPageNow].m_GameObject, true);
        //サブタブ表示
        UnityUtil.SetObjectEnabledOnce(m_MainMenuSubTab.gameObject, true);

        SetMenuHeaderDraw(false);

        MainMenuHeader.UnderMsgRequest("");
    }

    /// <summary>
    /// ページ変更中かどうか
    /// </summary>
    /// <returns></returns>
    public bool IsPageSwitch()
    {
        return m_SwitchPageFlag;
    }

    /// <summary>
    /// バックキー
    /// </summary>
    private void OnSelectBackKey()
    {
        if (!backkey)
        {
            return;
        }

        Dialog _newDialog = Dialog.Create(DialogType.DialogYesNo);
        if (m_WorkSwitchPageNow == MAINMENU_SEQ.SEQ_HOME_MENU)
        {
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mm55q_title");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "mm55q_content");
            _newDialog.SetDialogEvent(
                DialogButtonEventType.YES,
                () =>
                {
                    //タイトルへ戻る
                    SceneCommon.Instance.GameToTitle();
                });
        }
        else
        {
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "backkey_home_title");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "backkey_home_main");
            _newDialog.SetDialogEvent(
                DialogButtonEventType.YES,
                () =>
                {
                    //HOMEへ戻る
                    AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false);
                });
        }
        _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _newDialog.DisableCancelButton();
        _newDialog.Show();
    }

    public void DisableBackKey()
    {
        backkey = false;
    }

    public void EnableBackKey()
    {
        backkey = true;
    }

    private void setSubtitle()
    {
        switch (m_WorkSwitchPageNow)
        {
            case MAINMENU_SEQ.SEQ_HOME_MENU: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("ho22p_description")); break;
            case MAINMENU_SEQ.SEQ_HERO_FORM: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("ho26p_description")); break;
            case MAINMENU_SEQ.SEQ_HERO_DETAIL: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("ho28p_description")); break;
            case MAINMENU_SEQ.SEQ_UNIT_PARTY_SELECT: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un70p_description")); break;
            case MAINMENU_SEQ.SEQ_UNIT_PARTY_FORM: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un71p_description")); break;
            case MAINMENU_SEQ.SEQ_UNIT_BUILDUP: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un74p_description")); break;
            case MAINMENU_SEQ.SEQ_UNIT_BUILDUP_RESULT: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un80p_description")); break;
            case MAINMENU_SEQ.SEQ_UNIT_EVOLVE: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un81p_description")); break;
            case MAINMENU_SEQ.SEQ_UNIT_EVOLVE_RESULT: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un85p_description")); break;
            case MAINMENU_SEQ.SEQ_UNIT_LINK: break;
            case MAINMENU_SEQ.SEQ_UNIT_LINK_RESULT: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un91p_description")); break;
            case MAINMENU_SEQ.SEQ_UNIT_SALE: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un95p_description")); break;
            case MAINMENU_SEQ.SEQ_UNIT_LIST: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un98p_description")); break;
            case MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_STORY: break;
            case MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_EVENT: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk100p_description")); break;
            case MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_SCHOOL: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk101p_description")); break;
            case MAINMENU_SEQ.SEQ_QUEST_SELECT: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk103p_description")); break;
            case MAINMENU_SEQ.SEQ_QUEST_SELECT_FRIEND: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk111p_description")); break;
            case MAINMENU_SEQ.SEQ_QUEST_SELECT_PARTY: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk112p_description")); break;
            case MAINMENU_SEQ.SEQ_SHOP_POINT: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("sh125p_description")); break;
            case MAINMENU_SEQ.SEQ_GACHA_MAIN: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("sc142p_description")); break;
            case MAINMENU_SEQ.SEQ_FRIEND_LIST: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("fr151p_description")); break;
            case MAINMENU_SEQ.SEQ_FRIEND_LIST_WAIT_ME: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("fr155p_description")); break;
            case MAINMENU_SEQ.SEQ_FRIEND_LIST_WAIT_HIM: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("fr160p_description")); break;
            case MAINMENU_SEQ.SEQ_FRIEND_SEARCH: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("fr162p_description")); break;
            case MAINMENU_SEQ.SEQ_OTHERS_WEB: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("he168p_description")); break;
            case MAINMENU_SEQ.SEQ_OTHERS_HELP: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("he169p_description")); break;
            case MAINMENU_SEQ.SEQ_OTHERS_KIYAKU: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("he172p_description")); break;
            case MAINMENU_SEQ.SEQ_OTHERS_USER: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("he173p_description")); break;
            case MAINMENU_SEQ.SEQ_OTHERS_USER_RENAME: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("he174p_description")); break;
            case MAINMENU_SEQ.SEQ_OTHERS_SUPPORT: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("he181p_description")); break;
            case MAINMENU_SEQ.SEQ_OTHERS_MOVIE: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("he182p_description")); break;
            case MAINMENU_SEQ.SEQ_UNIT_CATALOG: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("he184p_description")); break;
            case MAINMENU_SEQ.SEQ_QUEST_SELECT_DETAIL: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk104f_description")); break;
            case MAINMENU_SEQ.SEQ_SHOP_POINT_EVOLVE: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("collabo_description")); break;
            case MAINMENU_SEQ.SEQ_SHOP_POINT_LIMITOVER: MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("limitover_description")); break;
            case MAINMENU_SEQ.SEQ_CHALLENGE_SELECT: break;
            default:
                MainMenuHeader.UnderMsgRequest("");
                break;
        }
    }

    private void setMissionMessage()
    {
        if (TutorialManager.IsExists == false)
        {
            MainMenuHeader.IsViewOtherUnderMessage(true);
        }
        else
        {
            MainMenuHeader.IsViewOtherUnderMessage(false);
        }

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	パッチのデータ更新リクエスト.
		@note	外部からの呼び出しを可にしておく.
	*/
    //----------------------------------------------------------------------------
    public void RequestPatchUpdate(bool bChkDialog = false)
    {
        // すでにロードリクエスト中の場合は動作させない.
        if (m_PatchUpdateRequestStep == EMAINMENU_PATCHUPDATE_REQ.NONE)
        {
            m_PatchUpdateRequestStep = EMAINMENU_PATCHUPDATE_REQ.REQUEST; //!< Patcherのファイル更新リクエスト開始.
        }

        m_PatchReqChkDialog = bChkDialog;
    }

    public void SetResetSubTabFlag()
    {
        if (SubTab != null)
        {
            SubTab.SetResetFlag();
        }
    }
}
