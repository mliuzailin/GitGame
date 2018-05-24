using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssetBundlerResponse
{
    private static Dictionary<string, AssetBundle> cache = new Dictionary<string, AssetBundle>();

    public static void clearAssetBundleChash()
    {
        foreach (KeyValuePair<string, AssetBundle> data in cache)
        {
            data.Value.Unload(false);
        }
        cache.Clear();
    }

    public AssetBundle AssetBundle
    {
        get
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CALL AssetBundlerResponse#AssetBundle get: " + assetBundleName);
#endif
            if (!cache.ContainsKey(assetBundleName))
            {
                string path = LocalSaveUtilToInstallFolder.LoadAsssetBundlePath(assetBundleName);
#if BUILD_TYPE_DEBUG
                Debug.Log("CALL AssetBundlerResponse#AssetBundle get: " + path);
#endif
                AssetBundle ab = AssetBundle.LoadFromFile(path);

                if (ab == null)
                {
                    return null;
                }

                cache.Add(assetBundleName, ab);
            }

            return cache[assetBundleName];
        }
    }

    public bool isCached()
    {
        if (cache.ContainsKey(assetBundleName))
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CALL AssetBundlerResponse#LoadCacheAsync is cached:" + assetBundleName);
#endif
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator LoadCacheAsync(System.Action finishAction = null)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL AssetBundlerResponse#LoadCacheAsync get:" + assetBundleName);
#endif

        if (isCached())
        {
            finishAction();
            yield break;
        }

        string path = LocalSaveUtilToInstallFolder.LoadAsssetBundlePath(assetBundleName);
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL AssetBundlerResponse#LoadCacheAsync get: " + path);
#endif
        AssetBundleCreateRequest baseCommonRequest = AssetBundle.LoadFromFileAsync(path);
        yield return baseCommonRequest;

        if (baseCommonRequest.assetBundle == null)
        {
            finishAction();
            yield break;
        }

        cache.Add(assetBundleName, baseCommonRequest.assetBundle);

        finishAction();
        yield break;
    }

    public Texture GetHeroThumbnailSprliteMask(int heroId)
    {
        return GetAsset<Texture>(string.Format("Hero_{0:D2}_mask", heroId));
    }

    public Sprite GetHeroThumbnailSprite(int heroId)
    {
        return GetAsset<Sprite>(string.Format("Hero_{0:D2}", heroId));
    }

    public T GetAsset<T>(string assetName = null)
        where T : UnityEngine.Object
    {
        if (assetName != null)
        {
            return AssetBundle.LoadAsset<T>(assetName);
        }
        return AssetBundle.LoadAllAssets().FirstOrDefault(t => t.GetType() == typeof(T)) as T;
    }

    public T[] GetAssetAll<T>()
        where T : UnityEngine.Object
    {
        return AssetBundle.LoadAllAssets<T>();
    }

    private string assetBundleName;

    public static AssetBundlerResponse Create(string assetBundleName)
    {
        AssetBundlerResponse response = new AssetBundlerResponse();
        response.assetBundleName = assetBundleName;

        return response;
    }

    public UIAtlas GetUIAtlas()
    {
        AssetBundle assetbundle = AssetBundle;

        if (assetbundle == null)
        {
            return null;
        }

        UIAtlas atlas = new UIAtlas();
        atlas.spriteList = assetbundle.LoadAllAssets<Sprite>();
        atlas.Name = assetbundle.name.Replace(AssetBundler.URLSuffix, "");

        return atlas;
    }

    public AudioClip GetAudioClip(string audioId)
    {
        return AssetBundle.LoadAsset<AudioClip>(audioId);
    }

    public List<AudioClip> GetAudioClipList()
    {
        return AssetBundle.LoadAllAssets<AudioClip>().ToList();
    }


    public Sprite GetSprite()
    {
        return AssetBundle.LoadAsset<Sprite>(assetBundleName);
    }

    public string[] GetAllAssetNames()
    {
        return AssetBundle.GetAllAssetNames();
    }

    public Texture2D GetTexture2D()
    {
        string[] names = AssetBundle.GetAllAssetNames();
        return AssetBundle.LoadAsset<Texture2D>(names[0]);
    }

    public Texture2D GetTexture2D(TextureWrapMode textureWrapMode)
    {
        Texture2D tex = GetTexture2D();
        if (tex != null)
        {
            tex.wrapMode = textureWrapMode;
        }
        return tex;
    }

    public Texture2D GetTexture2D(string name, TextureWrapMode textureWrapMode)
    {
        Texture2D tex = AssetBundle.LoadAsset<Texture2D>(name);
        if (tex != null)
        {
            tex.wrapMode = textureWrapMode;
        }

        return tex;
    }

    public Texture GetTexture(string name, TextureWrapMode textureWrapMode)
    {
        Texture tex = AssetBundle.LoadAsset<Texture>(name);
        if (tex != null)
        {
            tex.wrapMode = textureWrapMode;
        }

        return tex;
    }

    public Texture2D GetUncompressedTexture2D()
    {
        string[] names = AssetBundle.GetAllAssetNames();
        string name = names.FirstOrDefault(s => s.Contains("uncompressed"));
        if (name == null) name = names[0];
        Texture2D tex = AssetBundle.LoadAsset<Texture2D>(name);
        if (tex != null)
        {
            tex.wrapMode = TextureWrapMode.Clamp;
        }
        return tex;
    }


    public Sprite[] GetSprites()
    {
        return AssetBundle.LoadAllAssets<Sprite>();
    }
}

