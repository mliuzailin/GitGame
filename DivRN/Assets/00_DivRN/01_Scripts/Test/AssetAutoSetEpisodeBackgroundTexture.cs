using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetAutoSetEpisodeBackgroundTexture : MonoBehaviour
{
    public bool SetNativeSize = false;
    private Image image;
    private RawImage rawImage;

    void Awake()
    {
        image = GetComponent<Image>();
        rawImage = GetComponent<RawImage>();
    }

    void Start()
    {
        if (image != null)
        {
            image.enabled = false;
        }
        else if (rawImage != null)
        {
            rawImage.enabled = false;
        }
    }

    public Color Color
    {
        get
        {
            if (image != null)
            {
                return image.color;
            }
            else if (rawImage != null)
            {
                return rawImage.color;
            }
            return Color.white;
        }

        set
        {
            if (image != null)
            {
                image.color = value;
            }
            else if (rawImage != null)
            {
                rawImage.color = value;
            }
        }
    }

    public AssetBundler Create(uint areaCategoyFixId, Action finish = null, Action fail = null)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL AssetAutoSetEpisodeBackgroundTexture#Load:" + areaCategoyFixId);
#endif
        MasterDataAreaCategory master = MasterDataUtil.GetAreaCategoryParamFromID(areaCategoyFixId);
        int background = master == null ? 0 : master.background;
        string assetbundleName = MasterDataUtil.GetMasterDataAreamapBackgroundName(background);

        return Create(assetbundleName, finish, fail);
    }

    public AssetBundler Create(MasterDataChallengeEvent eventMaster, Action finish = null, Action fail = null)
    {
        string assetbundleName = (eventMaster.bg_assetbundle_name == null) ? "" : eventMaster.bg_assetbundle_name;
        if (MasterDataUtil.GetMasterDataAssetBundlePath(assetbundleName) == null)
        {
            assetbundleName = "Areamap_default";
        }
        return Create(assetbundleName, finish, fail);

    }

    public AssetBundler Create(string assetbundleName, Action finish = null, Action fail = null)
    {
        return AssetBundler.Create().Set(assetbundleName, "Background",
            (o) =>
            {
                if (image != null)
                {
                    image.overrideSprite = o.AssetBundle.LoadAsset<Sprite>("Background");
                    image.enabled = true;
                    if (SetNativeSize == true)
                    {
                        image.SetNativeSize();
                    }
                }
                else if (rawImage != null)
                {
                    rawImage.texture = o.GetTexture("Background", TextureWrapMode.Clamp);
                    rawImage.enabled = true;
                    if (SetNativeSize == true)
                    {
                        rawImage.SetNativeSize();
                    }
                }

                if (finish != null)
                {
                    finish();
                }
            },
            (error) =>
            {
                if (image != null)
                {
                    image.overrideSprite = Resources.Load<Sprite>("Default/BG_color");
                    image.enabled = true;
                    if (SetNativeSize == true)
                    {
                        image.SetNativeSize();
                    }
                }
                else if (rawImage != null)
                {
                    rawImage.texture = Resources.Load("Default/BG_color") as Texture;
                    rawImage.enabled = true;
                    if (SetNativeSize == true)
                    {
                        rawImage.SetNativeSize();
                    }
                }

                if (fail != null)
                {
                    fail();
                }
            }
        );
    }
}
