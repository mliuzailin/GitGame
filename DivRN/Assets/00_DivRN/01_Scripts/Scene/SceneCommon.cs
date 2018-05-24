using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using M4u;
using System.Configuration;
using MiniJSON;
using DG.Tweening;
using ServerDataDefine;

public enum SceneType
{
    NONE,
    SceneOutset,
    SceneTitle,
    SceneMainMenu,
    SceneQuest2,

    //
    SceneRefresh,
    SceneSplash,
}

public class SceneCommon : SingletonComponent<SceneCommon>
{
    private SceneType changeSceneType = SceneType.NONE;
    private SceneType lastSceneType = SceneType.NONE;
    private bool isAsyncLoad = true;


    public SceneType CurrentSceneType
    {
        get
        {
            return CurrentScene.GetSceneType();
        }
    } //!< 現在のシーンタイプ

    public SceneType LastSceneType
    {
        get
        {
            return lastSceneType;
        }
    } //!< 前回のシーンタイプ

    private bool isLoadingScene = true;

    public bool IsLoadingScene
    {
        get
        {
            return isLoadingScene;
        }
    }

    public M4uProperty<string> buildAt = new M4uProperty<string>();

    public string BuildAt
    {
        get
        {
            return buildAt.Value;
        }
        set
        {
            buildAt.Value = value;
        }
    }

    public M4uProperty<string> serverName = new M4uProperty<string>();

    public string ServerName
    {
        get
        {
            return serverName.Value;
        }
        set
        {
            serverName.Value = value;
        }
    }

    static public void initalizeMenuFps()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
    }

    static public void initalizeGameFps()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    protected override void Awake()
    {
        base.Awake();

        if (Patcher.Instance == null)
        {
            gameObject.AddComponent<Patcher>();
        }
        if (LocalSaveManager.Instance == null)
        {
            gameObject.AddComponent<LocalSaveManager>();
        }
        if (ServerDataManager.Instance == null)
        {
            gameObject.AddComponent<ServerDataManager>();
        }

        initalizeMenuFps();

        /**
         * DOTween初期化
        **/
        DOTween.Init(true, false, LogBehaviour.ErrorsOnly);
    }

    void OnShowPatchUpdatedDialog()
    {
        Dialog.Create(DialogType.DialogOK).
            SetMainFromTextKey("PATCH_UPDATED").
            SetDialogEvent(
                DialogButtonEventType.OK,
                () =>
                {
                    LocalSaveManagerRN.Instance.PatcherCounter = Patcher.Instance.GetUpdateCounter();
                    LocalSaveManagerRN.Instance.Save();
                    CommonFSM.Instance.SendFsmNextEvent();
                }).
            Show();
    }

    void OnIsPatchUpdated()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnIsPatchUpdated Local:" + LocalSaveManagerRN.Instance.PatcherCounter + " REMOTE:" + Patcher.Instance.GetUpdateCounter());
#endif
        //patchのバージョンが上がっていた場合
        if (LocalSaveManagerRN.Instance.PatcherCounter < Patcher.Instance.GetUpdateCounter())
        {
            CommonFSM.Instance.SendFsmPositiveEvent();
            return;
        }

        CommonFSM.Instance.SendFsmNegativeEvent();
    }

    void OnGoSplash()
    {
        ChangeScene(SceneType.SceneSplash);
    }

    void OnGoTitle()
    {
        ChangeScene(SceneType.SceneTitle);
    }

    public void GameToTitle()
    {
        SoundUtil.StopBGM(false);
        ChangeScene(SceneType.SceneTitle);

        //DG0-2813 チュートリアル中にエラーでタイトルに戻された場合の対応
        if (TutorialManager.IsExists)
        {
            TutorialManager.Instance.Destroy();
        }
    }

    protected override void Start()
    {
        base.Start();

#if UNITY_EDITOR
        BuildAt = "";
        ServerName = ServerDataUtil.GetServerName();
#else
        BuildAt = "";
        ServerName = "";
#endif
        //アイコンアトラス更新関数開始
        StartCoroutine(UpdateAtlusList());
    }


    void OnInitialize()
    {
        //初期起動シーン取得
        changeSceneType = CurrentScene.GetSceneType();

        CommonFSM.Instance.SendFsmEvent("INITIALIZED");
    }

    void OnShouldUserAutoLogin()
    {
#if UNITY_EDITOR && BUILD_TYPE_DEBUG
        if (DebugOption.Instance.AutoLoginUser)
        {
            CommonFSM.Instance.SendFsmPositiveEvent();
            return;
        }
#endif
        CommonFSM.Instance.SendFsmNegativeEvent();
    }

    void OnShouldFadeOutWait()
    {
        CommonFSM.Instance.SendFsmPositiveEvent();
    }


    void OnCallSceneInitialized()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL SceneCommon#OnCallSceneInitialized:" + CurrentScene.GetSceneType());
#endif
        CurrentScene.OnInitialized();
        isLoadingScene = false;
        CommonFSM.Instance.SendFsmNextEvent();
    }

    public void ChangeScene(SceneType sceneType, bool is_async_load = true)
    {
        lastSceneType = CurrentSceneType;
        changeSceneType = sceneType;
        isAsyncLoad = is_async_load;
        isLoadingScene = true;
        CommonFSM.Instance.SendFsmEvent("CHANGE_SCENE");
    }

    public void StartFadeIn()
    {
        LoadingManager.Instance.RequestLoadingFinish();

        CommonFSM.Instance.SendFsmEvent("FADE_IN");
    }

    IEnumerator OnChangeScene()
    {
        AndroidBackKeyManager.Instance.StackClear();

        if (isAsyncLoad)
        {
            // 非同期ロード
            //参照が切れているリソースを確実に開放するために、シーン切替時に空シーンを挟む
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("SceneEmpty");

            while (!asyncOperation.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();

            Resources.UnloadUnusedAssets();

            asyncOperation = SceneManager.LoadSceneAsync(changeSceneType.ToString());

            while (!asyncOperation.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            // 同期ロード（画面を動かすため何回か yield return null している）
            //参照が切れているリソースを確実に開放するために、シーン切替時に空シーンを挟む
            SceneManager.LoadScene("SceneEmpty");

            Resources.UnloadUnusedAssets();

            yield return null;

            SceneManager.LoadScene(changeSceneType.ToString());
        }

        if (TutorialManager.IsExists)
        {
            TutorialFSM.Instance.SendFsmEvent("CHANGED_SCENE");
        }

        CommonFSM.Instance.SendFsmNextEvent();
    }

    IEnumerator OnCheckBGMFade()
    {
        if (BGMManager.HasInstance)
        {
            while (BGMManager.Instance.isFadeOut)
            {
                yield return null;
            }
        }
        CommonFSM.Instance.SendFsmNextEvent();
    }

    void OnFadeOutWait()
    {
        switch (changeSceneType)
        {
            case SceneType.SceneOutset:
                ResourceManager.Instance.ClearCacheExcept(ResourceType.Title);
                break; // シーン：発端
            case SceneType.SceneTitle:
                ResourceManager.Instance.ClearCacheExcept(ResourceType.Title);
                break; // シーン：タイトルメニュー
            case SceneType.SceneMainMenu:
                ResourceManager.Instance.ClearCacheExcept(ResourceType.Menu);
                LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.TO_HOME);
                break; // シーン：メインメニュー
            case SceneType.SceneRefresh:
                ResourceManager.Instance.ClearCacheExcept(ResourceType.Common);
                LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.TO_HOME);
                break; // シーン：リフレッシュシーン
            case SceneType.SceneQuest2:
                ResourceManager.Instance.ClearCacheExcept(ResourceType.Battle);
                LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.TO_BATTLE);
                LoadingManager.Instance.SetText(GetQuestTitle());
                break; // シーン：新クエスト
            default:
                break; // シーン：その他
        }

        CommonFSM.Instance.SendFsmNextEvent();
    }

    void OnFadeInWait()
    {
        CommonFSM.Instance.SendFsmNextEvent();
    }

    void OnDisableInitialMaskIfNeed()
    {
        changeSceneType = SceneType.NONE;
        CommonFSM.Instance.SendFsmNextEvent();
    }


    /// <summary>
    /// ユーザー作成チェック
    /// </summary>
    public void CheckCreateUser()
    {
        bool bUUIDCheck = false;
        if (LocalSaveManager.Instance != null)
        {
            bUUIDCheck = LocalSaveManager.Instance.CheckUUID();
        }

        //----------------------------------------
        // ユーザー認証APIでのユーザー認証要請のイレギュラー対応を追加。
        //----------------------------------------
        ServerDataManager.Instance.ResultCodeAddIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED, API_CODETYPE.API_CODETYPE_USUALLY);
        ServerDataManager.Instance.ResultCodeAddIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED2, API_CODETYPE.API_CODETYPE_USUALLY);

        if (!bUUIDCheck)
        {
            CommonFSM.Instance.SendFsmEvent("DO_POSITIVE");
        }
        else
        {
            CommonFSM.Instance.SendFsmEvent("DO_NEGATIVE");
        }
    }

    /// <summary>
    /// ユーザー作成
    /// </summary>
    public void OnCreateUser()
    {
        string uuid = "";

        ServerDataUtilSend.SendPacketAPI_UserCreate(ref uuid).
            setSuccessAction(
                _data =>
                {
                    //----------------------------------------
                    // ユーザー認証APIでのユーザー認証要請のイレギュラー対応を除去。
                    // ここで外しておかないと他のAPIのセッション切れがスルーされるので確実に切っておく
                    //----------------------------------------
                    ServerDataManager.Instance.ResultCodeDelIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED);
                    ServerDataManager.Instance.ResultCodeDelIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED2);

                    LocalSaveManager.Instance.SaveFuncUUID(uuid);
                    UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvCreateUser>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                    UserDataAdmin.Instance.m_StructHeroList = _data.GetResult<RecvCreateUser>().result.hero_list;
                    UserDataAdmin.Instance.ConvertPartyAssing();
                    CommonFSM.Instance.SendFsmNextEvent();
                }).
            setErrorAction(
                _data =>
                {
                    //----------------------------------------
                    // ユーザー認証APIでのユーザー認証要請のイレギュラー対応を除去。
                    // ここで外しておかないと他のAPIのセッション切れがスルーされるので確実に切っておく
                    //----------------------------------------
                    ServerDataManager.Instance.ResultCodeDelIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED);
                    ServerDataManager.Instance.ResultCodeDelIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED2);

                    CommonFSM.Instance.SendFsmEvent("DO_ERROR");
                }).
            SendStart();
    }

    /// <summary>
    /// デフォルトパーティ設定
    /// </summary>
    public void OnSelectDefaultParty()
    {
        //デフォルトパーティ
        ServerDataUtilSend.SendPacketAPI_SkipTutorial(1).
            setSuccessAction(
                _data =>
                {
                    UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvSkipTutorial>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                    UserDataAdmin.Instance.ConvertPartyAssing();
                    CommonFSM.Instance.SendFsmNextEvent();
                }).
            setErrorAction(
                _data =>
                {
                    CommonFSM.Instance.SendFsmEvent("DO_ERROR");
                }).
            SendStart();
    }

    /// <summary>
    /// ユーザー認証
    /// </summary>
    public void OnAuthUser()
    {
        ServerDataUtilSend.SendPacketAPI_UserAuthentication().
            setSuccessAction(
                _data =>
                {
                    //----------------------------------------
                    // ユーザー認証APIでのユーザー認証要請のイレギュラー対応を除去。
                    // ここで外しておかないと他のAPIのセッション切れがスルーされるので確実に切っておく
                    //----------------------------------------
                    ServerDataManager.Instance.ResultCodeDelIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED);
                    ServerDataManager.Instance.ResultCodeDelIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED2);

                    UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvUserAuthentication>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                    UserDataAdmin.Instance.m_StructHeroList = _data.GetResult<RecvUserAuthentication>().result.hero_list;
                    UserDataAdmin.Instance.ConvertPartyAssing();
                    CommonFSM.Instance.SendFsmNextEvent();
                }).
            setErrorAction(
                _data =>
                {
                    //----------------------------------------
                    // ユーザー認証APIでのユーザー認証要請のイレギュラー対応を除去。
                    // ここで外しておかないと他のAPIのセッション切れがスルーされるので確実に切っておく
                    //----------------------------------------
                    ServerDataManager.Instance.ResultCodeDelIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED);
                    ServerDataManager.Instance.ResultCodeDelIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED2);

                    CommonFSM.Instance.SendFsmEvent("DO_ERROR");
                }).
            SendStart();
    }

    /// <summary>
    /// ログインパック取得
    /// </summary>
    public void OnLoginPack()
    {
        ServerDataUtilSend.SendPacketAPI_LoginPack(true, true, true, true).
            setSuccessAction(
                _data =>
                {
                    MainMenuUtil.setupLoginPack(_data.GetResult<RecvLoginPack>().result);
                    CommonFSM.Instance.SendFsmNextEvent();
                }).
            setErrorAction(
                _data =>
                {
                    CommonFSM.Instance.SendFsmNextEvent();
                }).
            SendStart();
    }

    void OnShouldLoadResidentResource()
    {
        if (CurrentScene.GetSceneType() == SceneType.SceneTitle)
        {
            CommonFSM.Instance.SendFsmNegativeEvent();
            return;
        }

        if (!UnitIconImageProvider.Instance.IsIconPackListEmpty())
        {
            CommonFSM.Instance.SendFsmNegativeEvent();
            return;
        }
        CommonFSM.Instance.SendFsmPositiveEvent();
    }

    void OnLoadResidentResource()
    {
        LoadResidentResource(
            () =>
            {
                CommonFSM.Instance.SendFsmNextEvent();
            });
    }

    IEnumerator CreateLoadResource(Action<AssetBundlerMultiplier> finishAction, List<int> categories = null)
    {
        AssetBundlerMultiplier mlutiplier = AssetBundlerMultiplier.Create();

        if (categories != null && categories.Count > 0)
        {
            for (int i = 0; i < categories.Count; i++)
            {
                List<MasterDataAssetBundlePath> assetBundlePathList = MasterFinder<MasterDataAssetBundlePath>.Instance.
                                                                SelectWhere(" where category = ? ", categories[i]);
#if BUILD_TYPE_DEBUG
                Debug.Log("CALL SceneTitle#LoadTitleDownload Count:" + assetBundlePathList.Count);
#endif
                yield return null;

                for (int j = 0; j < assetBundlePathList.Count; j++)
                {
                    AssetBundler aber = AssetBundler.Create().Set(assetBundlePathList[j].name.ToLower()).SkipCreateCache();
                    mlutiplier.Add(aber);
                    yield return null;
                };
            }

            yield return null;
        }

        yield return UnitIconImageProvider.Instance.LoadIconPacks(mlutiplier);

        yield return null;

        yield return ReplaceAssetManager.Instance.downloadReplaceAssetbundle(mlutiplier);

        yield return null;

        yield return AreaBackgroundProvider.Instance.PreLoadAreaBackground(mlutiplier);

        yield return null;

        yield return GeneralWindowProvider.Instance.PreLoad(mlutiplier);

        yield return null;


        mlutiplier.RegisterProgressAction(
            (float progress) =>
            {
                float progressPercent = progress * 100;
                LoadingManager.Instance.Progress(progressPercent);
                if (MovieManager.Instance != null)
                {
                    MovieManager.Instance.setPercent(progressPercent);
                }
            });

        finishAction(mlutiplier);
    }

    public void LoadResidentResource(Action finishAction, List<int> categories = null)
    {
        StartCoroutine(CreateLoadResource((mlutiplier) =>
        {
            mlutiplier.Load(
                () =>
                {
                    finishAction();
                    LoadingManager.Instance.Progress(100);
                    if (MovieManager.Instance != null)
                    {
                        MovieManager.Instance.setPercent(100);
                    }
                },
                () =>
                {
                    finishAction();
                    LoadingManager.Instance.Progress(100);
                    if (MovieManager.Instance != null)
                    {
                        MovieManager.Instance.setPercent(100);
                    }
                });
        },
            categories
        ));
    }


    public IScene CurrentScene
    {
        get;
        set;
    }

    // TODO : いろいろ整理しないと...
    private string GetQuestTitle()
    {
        uint questId = 0;

        if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore != null &&
            SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_QuestMissionID != 0)
        {
            questId = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_QuestMissionID;
        }
        if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2 != null &&
            SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestMissionID != 0)
        {
            questId = SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestMissionID;
        }
        if (MainMenuParam.m_QuestSelectMissionID != 0)
        {
            questId = MainMenuParam.m_QuestSelectMissionID;
        }
#if false// TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
        var quest1 = MasterFinder<MasterDataQuest>.Instance.Find((int)questId);
        if (quest1 != null &&
            quest1.quest_name.Length > 0)
        {
            return quest1.quest_name;
        }
#endif
        var quest2 = MasterDataUtil.GetQuest2ParamFromID(questId);
        if (quest2 != null &&
            quest2.quest_name.Length > 0)
        {
            return quest2.quest_name;
        }

        return "";
    }

    private static readonly int UnloadSpriteCount = 300;
    private int updateSpriteCount = 0;


    public IEnumerator UpdateAtlusList()
    {
        UnitIconAtlas[] atlusArray = UnitIconImageProvider.Instance.AtlusList;

        while (true)
        {
            yield return null;

            for (int i = 0; i < atlusArray.Length; i++)
            {
                if (atlusArray[i] == null)
                {
                    continue;
                }

                updateSpriteCount += atlusArray[i].UpdateAtlas();
            }

            if (updateSpriteCount > UnloadSpriteCount)
            {
                yield return null;

                updateSpriteCount = 0;
                Resources.UnloadUnusedAssets();
            }
        }
    }
}
