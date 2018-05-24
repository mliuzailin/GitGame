using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitIconImageProvider : SingletonComponent<UnitIconImageProvider>
{
    private static readonly int ATLUS_NUM = 4;

    private static readonly uint defaultIconAssetIndex = 0;
    private static readonly string defaultIconName = "chara_icon_question";
    public static string DefaultIconName { get { return defaultIconName; } }
    private Sprite m_DefaultIcon = null;

    public enum LoaderType
    {
        Atlus,
        Sprite,
        AssetBundle,
    }


    private LoaderType m_loaderType = LoaderType.AssetBundle;

    protected override void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        switch (m_loaderType)
        {
            case LoaderType.Atlus:
                m_loader = new UnitIconUIAtlasLoader();
                break;
            case LoaderType.Sprite:
                m_loader = new UnitIconSpriteLoader();
                break;
            case LoaderType.AssetBundle:
                m_loader = new UnitIconAssetLoader();
                break;
        }

        m_loader.SetCache(m_cache);
    }

    public void Tick()
    {
        m_loader.Tick();
    }

    public void SetLoaderType(LoaderType _type)
    {
        m_loaderType = _type;
        Initialize();
    }

    private UnitIconLoader m_loader = null;
    private UnitIconImageCache m_cache = new UnitIconImageCache();

    // deprecated (空アイコンはmenuのsheet1にあるっぽい)
    public void GetEmpty(System.Action<Sprite> callback, System.Action<Sprite> errorAction = null)
    {
        string icon_name = "chara_icon_empty";
        GetOldIcon(icon_name, sprite =>
        {
            //ユニットアイコンの遅延更新でアイテムアイコンを上書きされないように名前チェックする。
            if (icon_name.Equals(sprite.name) ||
                icon_name.Equals(UnitIconImageProvider.DefaultIconName))//ユニットアイコンのデフォルトアイコン名
            {
                Call(sprite, callback);
            }
            else
            {
                if (errorAction != null)
                {
                    errorAction(sprite);
                }
            }
        });
    }

    //
    public void GetEtc(string icon_name, System.Action<Sprite> callback, System.Action<Sprite> errorAction = null)
    {
        GetEtcIcon(icon_name, sprite =>
        {
            //ユニットアイコンの遅延更新でアイテムアイコンを上書きされないように名前チェックする。
            if (icon_name.Equals(sprite.name) ||
                icon_name.Equals(UnitIconImageProvider.DefaultIconName))//ユニットアイコンのデフォルトアイコン名
            {
                Call(sprite, callback);
            }
            else
            {
                if (errorAction != null)
                {
                    errorAction(sprite);
                }
            }
        });
    }

    public long Get(uint unCharaFixID, System.Action<Sprite> callback = null, bool hipriority = false)
    {
        string spriteName = string.Empty;
        return Get(unCharaFixID, ref spriteName, callback, hipriority);
    }

    public long Get(uint unCharaFixID, ref string spriteName, System.Action<Sprite> callback = null, bool hipriority = false)
    {
        if (spriteName == string.Empty)
        {
            spriteName = UnitIconImageCache.GetCharaIconSpriteName(unCharaFixID);
        }

        string name = spriteName;

        long requestId = GetCharaIcon(unCharaFixID, sprite =>
        {
            Call(sprite, callback);
        }, hipriority);

        return requestId;
    }

    private void Call(Sprite sprite, System.Action<Sprite> callback)
    {
        if (callback != null)
        {
            // アイコンの取得中に表示先のオブジェクトがDestroyされるとMissingReferenceExceptionエラーになるので例外処理追加
            try
            {
                callback(sprite);
            }
            catch (MissingReferenceException e)
            {
#if BUILD_TYPE_DEBUG
                Debug.LogError(e);
#endif
            }
        }
    }

    public void Stop()
    {
        m_loader.CancelLoadingAssetQueue();
    }

    // ================================ cache management
    public void ClearAllCache()
    {
        m_cache.Clear();
        m_atlasLocators.Clear();
        m_loader.ClearCache();
    }

    private void removeChache(string name)
    {
        switch (m_loaderType)
        {
            case LoaderType.Atlus:
                m_cache.RemoveAtlas(name);
                break;
            case LoaderType.Sprite:
                m_cache.RemoveSprite(name);
                break;
            case LoaderType.AssetBundle:
                break;
        }
    }

    public void Reset(uint fix_id)
    {
        if (m_cache != null)
        {
            m_cache.Reset(fix_id);
        }
    }

    public void ResetAll()
    {
        if (m_cache != null)
        {
            m_cache.ResetAll();
        }
    }

    // ================================ RequestId
    private long m_RequestCounter = 0;

    private long getRequestId()
    {
        m_RequestCounter++;

        if (m_RequestCounter < 0)
        {
            m_RequestCounter = 1;
        }

        return m_RequestCounter;
    }

    // ================================ Atlus List
    private static readonly string baseAssetBundleName = "icondivpackunit_base";
    private UnitIconAtlas[] m_AtlusList = new UnitIconAtlas[ATLUS_NUM];
    public UnitIconAtlas[] AtlusList { get { return m_AtlusList; } }

    public void MakeAtlus(Action action)
    {
        if (m_loaderType != LoaderType.AssetBundle)
        {
            action();
            return;
        }

        //
        m_cache.Clear();

        m_loader.SerialLoad(baseAssetBundleName, (AssetBundle assetbundle) =>
        {
            if (assetbundle == null)
            {
                action();
                return;
            }

            string[] names = assetbundle.GetAllAssetNames();
            if (names == null ||
                names.Length < ATLUS_NUM)
            {
                action();
                return;
            }

            for (int i = 0; i < ATLUS_NUM; i++)
            {
#if UNITY_STANDALONE_WIN //&& !UNITY_EDITOR
                m_AtlusList[i] = new UnitIconAtlasWindows();
#elif UNITY_IOS
                m_AtlusList[i] = new UnitIconAtlasPVRTC();
#else
                m_AtlusList[i] = new UnitIconAtlasETC();
#endif
                m_AtlusList[i].Initialize(assetbundle, names[i]);
            }

            // DG0-3521
            // チュートリアル開始のOnChangeSceneでMenuをロードするとアイコンが消えて
            // しまう問題の対処
            RawImage[] images = gameObject.transform.GetComponentsInChildren<RawImage>();
            if (images != null)
            {
                for (int i = 0; i < ATLUS_NUM; i++)
                {
                    images[i].texture = m_AtlusList[i].Texture;
                }
            }

            var name = FindIconPackAtlasName(defaultIconAssetIndex);

            //デフォルトアイコンロード
            m_loader.SerialLoad(name, (AssetBundle assetbundle2) =>
            {
                if (assetbundle == null)
                {
                    action();
                    return;
                }

                m_DefaultIcon = assetbundle2.LoadAsset<Sprite>(defaultIconName);
                for (int i = 0; i < ATLUS_NUM; i++)
                {
                    m_AtlusList[i].DefaultIcon = m_DefaultIcon;
                }

                action();
            });

            //debugObjLoad();
        });
    }

    public void hiddenCanvas(bool enable = false)
    {
        // DG0-3521
        // チュートリアル開始のOnChangeSceneでMenuをロードするとアイコンが消えて
        // しまう問題の対処
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.enabled = enable;
    }

    private GameObject debugObj = null;
    /// <summary>
    /// デバッグ表示オブジェクトロード
    /// </summary>
    public void debugObjLoad()
    {
        if (debugObj != null)
        {
            return;
        }

        GameObject tmpObj = Resources.Load("UnitIconData/DebugIconAtlas") as GameObject;
        if (tmpObj == null)
        {
            return;
        }

        debugObj = GameObject.Instantiate(tmpObj) as GameObject;
        if (debugObj == null)
        {
            return;
        }

        RawImage[] images = debugObj.GetComponentsInChildren<RawImage>();
        if (images != null)
        {
            for (int i = 0; i < ATLUS_NUM; i++)
            {
                images[i].texture = m_AtlusList[i].Texture;
            }
        }
    }

    /// <summary>
    /// キャッシュからスプライト取得
    /// </summary>
    /// <param name="unCharaFixID"></param>
    /// <returns>スプライト</returns>
    private Sprite GetCharaIconFromChash(SpriteCacheInfo info)
    {
        if (info == null)
        {
            return null;
        }

        return m_AtlusList[info.atlusIndex].Sprites[info.spriteIndex];
    }


    public void SetCharaIconReady(string icon_name)
    {
        SpriteCacheInfo info = m_cache.GetSpriteInfo(icon_name);
        if (info != null)
        {
            info.ready = true;
        }
    }

    /// <summary>
    /// 未使用のスプライト情報を取得
    /// </summary>
    /// <returns>スプライト情報</returns>
    private SpriteCacheInfo GetFreeSpriteInfo()
    {
        int atlusIndex = -1;
        int spriteIndex = -1;
        for (int i = 0; i < ATLUS_NUM; i++)
        {
            spriteIndex = m_AtlusList[i].GetFreeIndex();
            if (spriteIndex != -1)
            {
                atlusIndex = i;
                break;
            }
        }

        if (spriteIndex == -1)
        {
            return null;
        }

        SpriteCacheInfo info = new SpriteCacheInfo(atlusIndex, spriteIndex);
        return info;
    }

    private Sprite GetSprite(AssetBundle assetBundle, long requestId, SpriteCacheInfo info, Action<Sprite> callback)
    {
        Sprite sprite = m_AtlusList[info.atlusIndex].SetSprite(assetBundle, requestId, info, callback);
        m_cache.Register(info.name, info);
        return sprite;
    }

    /// <summary>
    /// キャッシュ済みスプライト情報取得
    /// </summary>
    /// <returns>スプライト情報</returns>
    private SpriteCacheInfo GetCacheSpriteInfo(string key)
    {
        SpriteCacheInfo info = m_cache.GetSpriteInfo(key);

        if (info == null)
        {
            return null;
        }

        return info;
    }
    /// <summary>
    /// 再利用するスプライト情報取得
    /// </summary>
    /// <returns>スプライト情報</returns>
    private SpriteCacheInfo GetReplaceSpriteInfo()
    {
        SpriteCacheInfo info = m_cache.GetSpriteInfoOldItem();

        if (info == null)
        {
            return null;
        }

        return info;
    }

    private void RemoveAtlusRequest(long requestId)
    {
        if (requestId == 0)
        {
            return;
        }

        for (int i = 0; i < ATLUS_NUM; i++)
        {
            m_AtlusList[i].RemoveRequest(requestId);
        }
    }

    // ================================ icon pack
    private List<UnitIconAtlasLocator> m_atlasLocators = new List<UnitIconAtlasLocator>();

    public bool IsIconPackListEmpty()
    {
        return m_atlasLocators.IsNullOrEmpty();
    }

    private void FindIconPackAtlasFromName(string name, System.Action<UIAtlas> callback)
    {
        var atlasLocator = m_atlasLocators
            .FirstOrDefault(locator => locator.name.Equals(name));

        var atrasName = atlasLocator != null
            ? atlasLocator.name
            : "";

        if (atrasName == "")
        {
            callback(null);
            return;
        }

        m_loader.SerialLoad(atrasName, callback);
    }

    // ================================ take sprite from icon pack

    // TODO : refactor
    private long GetCharaIcon(uint unCharaFixID, System.Action<Sprite> callback = null, bool hipriority = false)
    {
        long requestId = 0;
        if (unCharaFixID == 0)
        {
            Call(m_DefaultIcon, callback);
            return requestId;
        }

        requestId = getRequestId();

        switch (m_loaderType)
        {
            case LoaderType.Atlus:
                {
                    GetCharaIconAtlas(unCharaFixID, atlas =>
                    {
                        var strSpriteName = UnitIconImageCache.GetCharaIconSpriteName(unCharaFixID);

                        if (atlas == null)
                        {
                            Debug.LogError("Atlas None! - " + " , charaID:" + unCharaFixID);
                            Call(m_DefaultIcon, callback);

                            return;
                        }

                        var sprire = atlas.GetSprite(strSpriteName);
                        if (sprire == null)
                        {
                            Debug.LogError("Atlas Inside None! - " + strSpriteName);
                        }

                        Call(sprire, callback);
                    });
                }
                break;
            case LoaderType.Sprite:
                {
                    GetCharaIconSprite(unCharaFixID, callback);
                }
                break;
            case LoaderType.AssetBundle:
                {
                    string spritename = UnitIconImageCache.GetCharaIconSpriteName(unCharaFixID);

                    //すでにキャッシュされている
                    SpriteCacheInfo cacheInfo = m_cache.GetSpriteInfo(spritename);
                    if (cacheInfo != null)
                    {
                        if (hipriority)
                        {
                            cacheInfo.HiPriority = true;
                        }

                        if (cacheInfo.ready)
                        {
                            //準備完了
                            Sprite chashSprite = GetCharaIconFromChash(cacheInfo);
                            if (chashSprite != null)
                            {
                                Call(chashSprite, callback);
                            }
                        }
                        else
                        {
                            //まだリクエスト中

                            //リクエストにアクション追加
                            m_AtlusList[cacheInfo.atlusIndex].AddRequestCallBack(cacheInfo, callback);

                            //デフォルトアイコンを設定
                            Call(m_DefaultIcon, callback);
                        }

                        return 0;
                    }

                    //デフォルトアイコンを設定
                    Call(m_DefaultIcon, callback);

                    //アセットバンドルから読み込んで追加
                    GetCharaIconAssetBundle(unCharaFixID, assetbundle =>
                    {
                        //アセットバンドルがない
                        if (assetbundle == null)
                        {
                            Debug.LogError("AssetBundle None! - " + " , charaID:" + unCharaFixID);
                            Call(m_DefaultIcon, callback);

                            return;
                        }

                        //すでにキャッシュ済みになっていないか？
                        SpriteCacheInfo nowInfo = GetCacheSpriteInfo(spritename);
                        if (nowInfo != null)
                        {
                            if (hipriority)
                            {
                                nowInfo.HiPriority = true;
                            }

                            GetSprite(assetbundle, requestId, nowInfo, callback);
                            return;
                        }

                        //アトラスに空きがあるかどうか
                        SpriteCacheInfo freeInfo = GetFreeSpriteInfo();
                        if (freeInfo != null)
                        {
                            //ある場合は追加
                            freeInfo.chara_fix_id = unCharaFixID;
                            freeInfo.name = spritename;
                            freeInfo.HiPriority = hipriority;

                            GetSprite(assetbundle, requestId, freeInfo, callback);

                            return;
                        }

                        //ない場合はキャッシュから使用率が低いものを取得して置き換え
                        SpriteCacheInfo replaceInfo = GetReplaceSpriteInfo();
                        if (replaceInfo != null)
                        {
                            replaceInfo.chara_fix_id = unCharaFixID;
                            replaceInfo.name = spritename;
                            replaceInfo.ready = false;
                            replaceInfo.HiPriority = hipriority;

                            GetSprite(assetbundle, requestId, replaceInfo, callback);

                            return;
                        }
                    });
                }
                break;
        }

        return requestId;
    }

    private void GetOldIcon(string icon_name, System.Action<Sprite> action)
    {
        switch (m_loaderType)
        {
            case LoaderType.Atlus:
                FindIconPackAtlas(1, atlus =>
                {
                    action(atlus.GetSprite(icon_name));
                });
                break;
            case LoaderType.Sprite:
                break;
            case LoaderType.AssetBundle:
                GetCharaIconAssetBundle(1, assetBundle =>
                {
                    action(assetBundle.LoadAsset<Sprite>(icon_name));
                });
                break;
        }
    }

    private static readonly string etcAssetBundleName = "icondivpack_etc";

    private void GetEtcIcon(string icon_name, System.Action<Sprite> action)
    {
        switch (m_loaderType)
        {
            case LoaderType.Atlus:
                m_loader.Get("iconpack_etc", (UIAtlas atlus) =>
                {
                    if (atlus == null)
                    {
                        return;
                    }

                    Sprite sprite = atlus.GetSprite(icon_name);
                    if (sprite == null)
                    {
                        return;
                    }

                    Call(sprite, action);
                });
                break;
            case LoaderType.Sprite:
                break;
            case LoaderType.AssetBundle:
                {
                    m_loader.Get(etcAssetBundleName, (AssetBundle asset) =>
                   {
                       if (asset == null)
                       {
                           Debug.LogError("AssetBundle None! - " + " , name:" + etcAssetBundleName);
                           return;
                       }

                       Sprite sprite = asset.LoadAsset<Sprite>(icon_name);
                       if (sprite == null)
                       {
                           Debug.LogError("Sprite None! - " + " , pack:" + etcAssetBundleName + " , name:" + icon_name);
                           return;
                       }

                       Call(sprite, action);
                   });
                }
                break;
        }
    }

    private void GetCharaIconAtlas(uint unCharaID, System.Action<UIAtlas> callback)
    {
        FindIconPackAtlas(unCharaID, callback);
    }

    private void GetCharaIconSprite(uint unCharaID, System.Action<Sprite> callback)
    {
        var name = UnitIconImageCache.GetCharaIconSpriteName(unCharaID);

        if (name == "")
        {
            Call(m_DefaultIcon, callback);
            return;
        }

        m_loader.SerialLoad(name, callback);
    }

    private void FindIconPackAtlas(uint charaId, System.Action<UIAtlas> callback)
    {
        var name = FindIconPackAtlasName(charaId);

        if (name == "")
        {
            callback(null);
            return;
        }

        m_loader.SerialLoad(name, callback);
    }

    private string FindIconPackAtlasName(uint charaId)
    {
        var atlasLocator = m_atlasLocators
            .FirstOrDefault(locator => locator.InRange((int)charaId));

        return atlasLocator != null
            ? atlasLocator.name
            : "";
    }

    private void GetCharaIconAssetBundle(uint unCharaID, System.Action<AssetBundle> callback)
    {
        var name = FindIconPackAtlasName(unCharaID);

        if (name == "")
        {
            callback(null);
            return;
        }

        m_loader.SerialLoad(name, callback);
    }

    // ====================================== loading resources

    // preload
    // called by a coroutine function only.
    public IEnumerator LoadIconPacks(AssetBundlerMultiplier mlutiplier)
    {
        List<MasterDataAssetBundlePath> assetBundlePathList = MasterFinder<MasterDataAssetBundlePath>.Instance.
                                                                SelectWhere(" where category = ? ", MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.ICONDIVPACK);
        yield return null;

        for (int i = 0; i < assetBundlePathList.Count; i++)
        {
            var name = assetBundlePathList[i].name.ToLower();
            AssetBundler assetBundler = null;
            switch (m_loaderType)
            {
                case LoaderType.Atlus:
                    assetBundler = m_loader.LoadAsset(name, (UIAtlas atlas) =>
                    {
                        if (atlas != null)
                        {
                            m_atlasLocators.Add(new UnitIconAtlasLocator(name));
                        }
                    });
                    break;
                case LoaderType.Sprite:
                    assetBundler = m_loader.LoadAsset(name, (Sprite sprite) =>
                    {
                    });
                    break;
                case LoaderType.AssetBundle:
                    assetBundler = m_loader.LoadAsset(name, (AssetBundle asset) =>
                     {
                         if (asset != null)
                         {
                             m_atlasLocators.Add(new UnitIconAtlasLocator(name));
                         }
                     });
                    break;
            }

            if (assetBundler != null)
            {
                mlutiplier.Add(assetBundler);
            }

            yield return null;
        };
    }
}



// ======================================== loaders
public class UnitIconLoader
{
    virtual public AssetBundler LoadAsset(string name, System.Action<UIAtlas> callback)
    {
        return null;
    }
    virtual public AssetBundler LoadAsset(string name, System.Action<Sprite> callback)
    {
        return null;
    }
    virtual public AssetBundler LoadAsset(string name, System.Action<AssetBundle> callback)
    {
        return null;
    }

    // 一番最初に定義されていないとiOSでビルドエラーになる
    virtual public void SerialLoad(string name, System.Action<AssetBundle> callback)
    {
    }
    virtual public void SerialLoad(string name, System.Action<UIAtlas> callback)
    {
    }
    virtual public void SerialLoad(string name, System.Action<Sprite> callback)
    {
    }

    virtual public void Get(string name, System.Action<AssetBundle> callback)
    {
    }
    virtual public void Get(string name, System.Action<UIAtlas> callback)
    {
    }
    virtual public void Get(string name, System.Action<Sprite> callback)
    {
    }


    virtual public void ClearCache()
    {

    }

    virtual public void Tick()
    {

    }

    protected UnitIconImageCache m_cache = null;
    public void SetCache(UnitIconImageCache cache)
    {
        m_cache = cache;
    }

    public void CancelLoadingAssetQueue()
    {
        m_loadingAssetQueue.Clear();
        m_loadingAssetPackageCancel = true;
        m_loadingAssetBlock = false;
    }

    protected class AssetBundlerPackage
    {
        public string name = "";
        public System.Action<UIAtlas> uiAtlasCallback = null;
        public System.Action<Sprite> spriteCallback = null;
        public System.Action<AssetBundle> assetCallback = null;
    }

    protected Queue<AssetBundlerPackage> m_loadingAssetQueue = new Queue<AssetBundlerPackage>();
    protected bool m_loadingAssetBlock = false;
    protected bool m_loadingAssetPackageCancel = false;
}

public class UnitIconUIAtlasLoader : UnitIconLoader
{
    override public AssetBundler LoadAsset(string name, System.Action<UIAtlas> callback)
    {
        Debug.Assert(m_cache != null, "m_cache not set");

        var cachedAtlas = m_cache.GetAtlas(name);
        if (cachedAtlas != null)
        {
            callback(cachedAtlas);
            return null;
        }

        return AssetBundler.Create().Set(name, typeof(Sprite), (operation) =>
        {
            UIAtlas atlas = operation.GetUIAtlas();
            callback(atlas);
            if (atlas != null)
            {
                m_cache.Register(name, atlas);
            }
        });
    }

    public override void Get(string name, Action<UIAtlas> callback)
    {
        var cachedAtlas = m_cache.GetAtlas(name);
        if (cachedAtlas != null)
        {
            callback(cachedAtlas);
            return;
        }

        SerialLoad(name, (UIAtlas atlus) =>
        {
            callback(atlus);
        });
    }

    override public void SerialLoad(string name, System.Action<UIAtlas> callback)
    {
        if (m_loadingAssetPackageCancel)
        {
            m_loadingAssetPackageCancel = false;
        }

        m_loadingAssetQueue.Enqueue(new AssetBundlerPackage
        {
            name = name,
            uiAtlasCallback = atlas =>
            {
                callback(atlas);
            }
        });
    }

    override public void Tick()
    {
        while (UpdateLoadingAssetQueue()) { };
    }

    // キャッシュから読み込んだときにtrueをかえす
    private bool UpdateLoadingAssetQueue()
    {
        if (m_loadingAssetQueue.Count == 0
            || m_loadingAssetBlock)
        {
            return false;
        }

        var package = m_loadingAssetQueue.Dequeue();
        var name = package.name;
        var assetBundler = LoadAsset(name, atlas =>
        {
            if (m_loadingAssetPackageCancel)
            {
                return;
            }

            m_loadingAssetBlock = false;

            package.uiAtlasCallback(atlas);
        });

        if (assetBundler != null)
        {
            m_loadingAssetBlock = true;
            assetBundler.Load();
        }

        return assetBundler == null;
    }
}

public class UnitIconSpriteLoader : UnitIconLoader
{
    override public AssetBundler LoadAsset(string name, System.Action<Sprite> callback)
    {
        Debug.Assert(m_cache != null, "m_cache not set");

        var cachedSprite = m_cache.GetSprite(name);
        if (cachedSprite != null)
        {
            callback(cachedSprite);
            return null;
        }

        return AssetBundler.Create().Set(name, typeof(Sprite), (operation) =>
        {
            var sprite = operation.GetSprite();
            callback(sprite);
            if (sprite != null)
            {
                m_cache.Register(name, sprite);
            }
        });
    }

    public override void Get(string name, Action<Sprite> callback)
    {
        var cachedSprite = m_cache.GetSprite(name);
        if (cachedSprite != null)
        {
            callback(cachedSprite);
            return;
        }

        SerialLoad(name, (Sprite sprite) =>
        {
            callback(sprite);
        });
    }

    override public void SerialLoad(string name, System.Action<Sprite> callback)
    {
        if (m_loadingAssetPackageCancel)
        {
            m_loadingAssetPackageCancel = false;
        }

        m_loadingAssetQueue.Enqueue(new AssetBundlerPackage
        {
            name = name,
            spriteCallback = sprite =>
            {
                callback(sprite);
            }
        });
    }

    override public void Tick()
    {
        while (UpdateLoadingAssetQueue()) { };
    }

    // キャッシュから読み込んだときにtrueをかえす
    private bool UpdateLoadingAssetQueue()
    {
        if (m_loadingAssetQueue.Count == 0
            || m_loadingAssetBlock)
        {
            return false;
        }

        var package = m_loadingAssetQueue.Dequeue();
        var name = package.name;
        var assetBundler = LoadAsset(name, sprite =>
        {
            if (m_loadingAssetPackageCancel)
            {
                return;
            }

            m_loadingAssetBlock = false;

            package.spriteCallback(sprite);
        });

        if (assetBundler != null)
        {
            m_loadingAssetBlock = true;
            assetBundler.Load();
        }

        return assetBundler == null;
    }
}

public class UnitIconAssetLoader : UnitIconLoader
{
    private Dictionary<string, AssetBundle> m_assetbundles = new Dictionary<string, AssetBundle>();

    override public AssetBundler LoadAsset(string name, System.Action<AssetBundle> callback)
    {
        Debug.Assert(m_cache != null, "m_cache not set");

        if (m_assetbundles.ContainsKey(name))
        {
            callback(m_assetbundles[name]);
            return null;
        }
#if true

        return AssetBundler.Create().Set(name, (response) =>
        {
            m_assetbundles[name] = response.AssetBundle;
            callback(m_assetbundles[name]);
        });
#else
#if UNITY_IOS
        string path = Application.streamingAssetsPath + "/UnitIconPack/iOS/" + name;
#else
        string path = Application.streamingAssetsPath + "/UnitIconPack/Android/" + name;
#endif
        AssetBundle assetbundle = AssetBundle.LoadFromFile(path);
        if (assetbundle == null)
        {
            Debug.LogError("AssetBundle None! - " + " , path:" + path);
        }
        else
        {
            m_assetbundles[name] = assetbundle;
            callback(m_assetbundles[name]);
        }

        return null;
#endif
    }

    public override void Get(string name, Action<AssetBundle> callback)
    {
        if (m_assetbundles.ContainsKey(name))
        {
            callback(m_assetbundles[name]);
            return;
        }

        SerialLoad(name, (AssetBundle assetBundle) =>
        {
            callback(assetBundle);
        });
    }

    override public void SerialLoad(string name, System.Action<AssetBundle> callback)
    {
        if (m_loadingAssetPackageCancel)
        {
            m_loadingAssetPackageCancel = false;
        }

        m_loadingAssetQueue.Enqueue(new AssetBundlerPackage
        {
            name = name,
            assetCallback = asset =>
            {
                callback(asset);
            }
        });
    }

    override public void ClearCache()
    {
        m_assetbundles.Clear();
    }

    override public void Tick()
    {
        while (UpdateLoadingAssetQueue()) { };
    }

    // キャッシュから読み込んだときにtrueをかえす
    private bool UpdateLoadingAssetQueue()
    {
        if (m_loadingAssetQueue.Count == 0
            || m_loadingAssetBlock)
        {
            return false;
        }

        var package = m_loadingAssetQueue.Dequeue();
        var name = package.name;
        LoadAsset(name, (AssetBundle asset) =>
        {
            if (m_loadingAssetPackageCancel)
            {
                return;
            }

            m_loadingAssetBlock = false;

            package.assetCallback(asset);
        });

        return !m_loadingAssetBlock;
    }
}
