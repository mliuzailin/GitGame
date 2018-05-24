using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Configuration;
using System.Linq;
using UnityEngine;

public static class AssetBundlerPlayerPrefs
{
    private static Dictionary<string, uint> VALUES = null;

    public static bool isSaveDisk = true;

    public static void ResetCache()
    {
        VALUES = null;
    }

    public static uint GetVersion(string assetBundleName)
    {
        if (VALUES == null)
        {
            VALUES = LocalSaveUtilToInstallFolder.LoadAssetBundleVersions();
        }

        uint value = 0;

        try
        {
            value = VALUES[assetBundleName];
        }
        catch (Exception e)
        {
            Debug.Log(">Exception : " + e);
        }

        return value;
    }

    public static void SetVersion(string assetBundleName, uint version = 0)
    {
        if (VALUES == null)
        {
            VALUES = LocalSaveUtilToInstallFolder.LoadAssetBundleVersions();
        }

        VALUES[assetBundleName] = version;

        Save();
    }

    public static void Save()
    {
        if (isSaveDisk == true && VALUES != null)
        {
            LocalSaveUtilToInstallFolder.SaveAssetBundleVersions(VALUES);
        }
    }

    public static bool DelKey(string assetBundleName)
    {
        if (VALUES == null)
        {
            VALUES = LocalSaveUtilToInstallFolder.LoadAssetBundleVersions();
        }

        if (VALUES.ContainsKey(assetBundleName) == true)
        {
            VALUES.Remove(assetBundleName);
            return true;
        }
        return false;
    }
}

public class AssetBundler : BaseAssetBundler
{
    private AssetBundlerFSM fsm;
    private Action<AssetBundlerResponse> finishAction;
    private Action<string> errorAction;
    private Action<string> failAction;
    private System.Type type;
    private string assetBundleName;

    private MasterDataAssetBundlePath assetbundlepath;

    private string assetName;

    public AssetBundler DisableAutoDestoryOnSuccess()
    {
        IsAutoDestroyOnSuccess = false;
        return this;
    }


    public AssetBundler DisableAutoDestoryOnFail()
    {
        IsAutoDestroyOnFail = false;
        return this;
    }

    protected override void Awake()
    {
        base.Awake();

        DisableAutoDestoryOnFail();
        failAction = ((str) =>
        {
            DivRNUtil.ShowRetryDialog(
                () =>
                {
                    Retry();
                });
        });

        fsm = GetComponent<AssetBundlerFSM>();

    }

    public static AssetBundler Create()
    {
        GameObject go = Instantiate(Resources.Load("Prefab/AssetBundler") as GameObject);
        return go.GetComponent<AssetBundler>();
    }

    public AssetBundler Set(string assetBundleName, Action<AssetBundlerResponse> finishAction = null, Action<string> error = null)
    {
        return Set(assetBundleName, assetBundleName, typeof(GameObject), finishAction, error);
    }

    public AssetBundler Set(string assetBundleName, System.Type type, Action<AssetBundlerResponse> finishAction, Action<string> error = null)
    {
        return Set(assetBundleName, assetBundleName, type, finishAction, error);
    }

    public AssetBundler SetAsAudioClipPack(string packName, Action<List<AudioClip>> finishAction, Action<string> error = null)
    {
        return Set(
            packName,
            packName,
            typeof(AudioClip[]),
            (o) =>
            {
                finishAction(o.GetAudioClipList());
            },
            error);
    }

    public AssetBundler SetAsAudioClip(string packName, string audioId, Action<AudioClip> finishAction, Action<string> error = null)
    {
        return Set(
            packName,
            packName,
            typeof(AudioClip[]),
            (o) =>
            {
                finishAction(o.GetAudioClip(audioId));
            },
            error);
    }

    public AssetBundler SetAsUnitTexture(uint unCharaID, Action<AssetBundlerResponse> finishAction = null, Action<string> error = null)
    {
        int charaId = (int)unCharaID;
        string assetBundleName = string.Format("unit_{0}", DivRNUtil.UnitIdToString((uint)charaId));
        string assetname = string.Format("Unit_{0}", DivRNUtil.UnitIdToString((uint)charaId));

        return Set(assetBundleName, assetname, typeof(Texture2D), finishAction, error);
    }

    public AssetBundler Set(string assetBundleName, string assetName, Action<AssetBundlerResponse> finishAction = null, Action<string> error = null)
    {
        return Set(assetBundleName, assetName, typeof(GameObject), finishAction, error);
    }

    public AssetBundler Set(string assetBundleName, string assetName, System.Type type, Action<AssetBundlerResponse> finishAction = null, Action<string> error = null)
    {
        setAction(finishAction, error);

        this.assetBundleName = assetBundleName.ToLower();
        this.assetName = assetName;
        this.type = type;

        assetbundlepath = MasterDataUtil.GetMasterDataAssetBundlePath(this.assetBundleName, false);

#if BUILD_TYPE_DEBUG
        gameObject.name = "AssetBundler::" + assetBundleName + "::" + assetName + "::" + type.ToString();
        Debug.Log("CALL AssetBundler#Set:" + gameObject.name);
#endif
        return this;
    }

    public AssetBundler setAction(Action<AssetBundlerResponse> finishAction = null, Action<string> error = null, Action<string> failAction = null)
    {
        this.finishAction += finishAction;
        this.errorAction += error;
        this.failAction += failAction;

        return this;
    }

    public AssetBundler Load()
    {
        fsm.SendFsmNextEvent();
        return this;
    }

    private AssetBundlerResponse response;

    public AssetBundlerResponse Response
    {
        get
        {
            return response;
        }
    }

    void OnCheckAssetBundlePathMaster()
    {
#if BUILD_TYPE_DEBUG
        if (assetbundlepath == null)
        {
            DialogManager.Open1B_Direct("No Set Asset Bundle Path Master",
                                            "Asset Bundle Path Masterに登録されていない\nファイルをダウンロードしています。\nプランナーさんに画面を見せるか\n画面キャプチャーして報告してください。\n\n" +
                                            "assetBundleName: " + assetBundleName,
                                            "common_button7", true, true).
            SetOkEvent(() =>
            {
                fsm.SendFsmNextEvent();
            });
            return;
        }
        else if (assetbundlepath.title_dl == MasterDataDefineLabel.ASSETBUNDLE_TITLE_DL.DELETE)
        {
            DialogManager.Open1B_Direct("Delete Asset Bundle",
                                            "削除対象ファイルをダウンロードします。\nプランナーさんにマスターデータ設定が\n間違っていないか確認しください。\n\n不明な場合はクライアントプログラマに画面を見せるか\n画面キャプチャーして報告してください。\n\n" +
                                            "fix_id: " + assetbundlepath.fix_id + "\n" +
                                            "assetBundleName: " + assetBundleName,
                                            "common_button7", true, true).
            SetOkEvent(() =>
            {
                fsm.SendFsmNextEvent();
            });
            return;
        }
#endif

        fsm.SendFsmNextEvent();
    }

    void OnExistsCompatibleCache()
    {
#if UNITY_EDITOR && BUILD_TYPE_DEBUG 
        if (DebugOption.Instance.assetBundleDO.alwaysLoadFromCache)
        {
            fsm.SendFsmPositiveEvent();
            return;
        }

        if (DebugOption.Instance.assetBundleDO.disableCache)
        {
            fsm.SendFsmNegativeEvent();
            return;
        }
#endif

        //キャッシュしているバージョンがAssetBundlePathListに設定しているバージョンよりも低い場合
        if (assetbundlepath == null ||
            AssetBundlerPlayerPrefs.GetVersion(assetBundleName) < assetbundlepath.version)
        {
            fsm.SendFsmNegativeEvent();
            return;
        }

        fsm.SendFsmPositiveEvent();
    }

    public static string BaseURL
    {
        get
        {
            return string.Format(GlobalDefine.GetAssetBundleUrl(), NOUtil.Platform);
        }
    }

    public static string URLSuffix
    {
        get
        {
            string platform = NOUtil.Platform.ToLower();
            return string.Format("-{0}_quality-{1}", platform, LocalSaveManagerRN.Instance.QualitySetting.ToString().ToLower());
        }
    }

    void OnDownload()
    {
        string url = Path.Combine(BaseURL, string.Format("{0}{1}", assetBundleName, URLSuffix));

#if BUILD_TYPE_DEBUG
        Debug.Log("CALL AssetBundler#OnDownload:" + url);
#endif

        www = new WWW(url);
        www.threadPriority = ThreadPriority.Low;
        response = AssetBundlerResponse.Create(assetBundleName);

        fsm.SendFsmEvent("DO_WAIT");
    }

    void OnLoadFromCache()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL AssetBundler#OnLoadFromCache: " + assetBundleName);
#endif
        response = AssetBundlerResponse.Create(assetBundleName);

        if (response.AssetBundle == null)
        {
            fsm.SendFsmEvent("FAIL");
            return;
        }

        fsm.SendFsmEvent("SUCCESS");
    }

    private string error;

    IEnumerator OnWait()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL AssetBundler#WAIT:" + www.progress + " URL:" + www.url);
#endif
        while (www.isDone == false)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogError("CALL AssetBundler# NetworkReachability.NotReachable.WWW url: " + www.url + " error: " + www.error);
                break;
            }

            yield return null;
        }

        int code = responseCode;
        if (Application.internetReachability == NetworkReachability.NotReachable ||
            code == NG_RESPONSE_CODE)
        {
            Debug.LogError("CALL AssetBundler#www url: " + www.url + " error: " + www.error);
            this.error = www.error;
            fsm.SendFsmEvent("FAIL");
            yield break;
        }

        if (code >= 400)
        {
            Debug.LogError("CALL AssetBundler#www url: " + www.url + " error: " + www.error);
            this.error = www.error;
            fsm.SendFsmEvent("ERROR");
            yield break;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("CALL AssetBundler#www url: " + www.url + " error: " + www.error);
            this.error = www.error;
            fsm.SendFsmEvent("FAIL");
            yield break;
        }

        LocalSaveUtilToInstallFolder.SaveAsssetBundle(assetBundleName, www.bytes);

        if (response.AssetBundle == null)
        {
            Debug.LogError("CALL AssetBundler#ASSETBUNDLE_IS_NULL:" + www.url);
            fsm.SendFsmEvent("FAIL");
            yield break;
        }

        fsm.SendFsmEvent("SUCCESS");
    }

    void OnNoAssetBundle()
    {
#if BUILD_TYPE_DEBUG
#if UNITY_STANDALONE || UNITY_EDITOR
        fsm.SendFsmEvent("ERROR");
#else
        Uri url = new Uri(www.url);
        DialogManager.Open1B_Direct("No Asset Bundle",
                                        "ファイルがサーバにアップロードされていない\nもしくは、アクセスできません\nアップロード担当者に問い合わせてください\n\n" +
                                        "host: " + url.Host + "\n" +
                                        "Path: " + url.AbsolutePath.Replace("/", "/\n") + "\n" +
                                        "error: " + www.error,
                                        "common_button7", true, true).
        SetOkEvent(() =>
        {
            fsm.SendFsmEvent("ERROR");
        });
#endif
#else
        fsm.SendFsmEvent("ERROR");
#endif
    }

    public override string ActiveStateName
    {
        get
        {
            return fsm.ActiveStateName;
        }
    }

    public bool skipCreateCache;

    public AssetBundler SkipCreateCache()
    {
        skipCreateCache = true;
        return this;
    }

    void OnCreateCache()
    {
        if (skipCreateCache)
        {
            fsm.SendFsmNextEvent();
        }
        else if (response.isCached())
        {
            fsm.SendFsmNextEvent();
        }
        else
        {
            StartCoroutine(response.LoadCacheAsync(() =>
            {
                fsm.SendFsmNextEvent();
            }));
        }
    }

    void OnSuccess()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL AssetBundler#OnSuccess:");
#endif

        uint version = 0;
        if (assetbundlepath != null)
        {
            version = assetbundlepath.version;
        }

        AssetBundlerPlayerPrefs.SetVersion(assetBundleName, version);

        if (finishAction != null)
        {
            finishAction(response);
        }

        wwwRelece();

        if (IsAutoDestroyOnSuccess)
        {
            Destroy(gameObject);
        }
    }

    void OnError()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL AssetBundler#ERROR:" + error);
#endif

        if (errorAction != null)
        {
            errorAction(error);
        }

        wwwRelece();

        if (IsAutoDestroyOnSuccess)
        {
            Destroy(gameObject);
        }
    }

    void OnFail()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL AssetBundler#FAIL:" + error);
#endif

        if (failAction != null)
        {
            failAction(error);
        }

        if (IsAutoDestroyOnFail)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (response != null)
        {
            response = null;
        }
    }

    // disableRetryDialogはsetActionより先に呼ばないとFailが呼ばれない
    public void disableRetryDialog()
    {
        failAction = null;
    }

    public void Retry()
    {
        fsm.SendFsmEvent("RETRY");
    }


    private WWW www;

    public WWW WWW
    {
        get
        {
            return www;
        }
    }

    private void wwwRelece()
    {
        if (www != null)
        {
            www.Dispose();
            www = null;
        }
    }

    public int responseCode
    {
        get
        {
            if (www == null)
            {
                return NG_RESPONSE_CODE;
            }
            else
            {
                return getResponseCode(www);
            }
        }
    }

    static public int NG_RESPONSE_CODE = 0;

    public static int getResponseCode(WWW request)
    {
        if (request == null ||
            request.isDone == false)
        {
            Debug.LogError("no www.");

            return NG_RESPONSE_CODE;
        }

        if (request.responseHeaders == null)
        {
            Debug.LogError("no response headers.");

            return NG_RESPONSE_CODE;
        }

        if (!request.responseHeaders.ContainsKey("STATUS"))
        {
            Debug.LogError("response headers has no STATUS.");

            return NG_RESPONSE_CODE;
        }

        string statusLine = request.responseHeaders["STATUS"];
        string[] components = statusLine.Split(' ');
        if (components.Length < 3)
        {
            Debug.LogError("invalid response status: " + statusLine);
            return NG_RESPONSE_CODE;
        }

        int ret = NG_RESPONSE_CODE;
        if (!int.TryParse(components[1], out ret))
        {
            Debug.LogError("invalid response code: " + components[1]);
        }

        return ret;
    }
}
