using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetAutoSetCharaImage : AssetAutoSetCharaTexture
{
    public bool useUncompressed;
    public bool useSetWidthOnly;

    private float m_ImageScale = 1.0f;
    public float ImageScale { get { return m_ImageScale; } set { m_ImageScale = value; } }

    protected override void SetTexture(Texture2D cAssetBundleTexture)
    {
        GetComponent<Image>().sprite = Sprite.Create(cAssetBundleTexture, new Rect(0, 0, cAssetBundleTexture.width, cAssetBundleTexture.height), Vector2.zero);
        GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        if (useSetWidthOnly)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(cAssetBundleTexture.GetUnitTextureWidth() * m_ImageScale, GetComponent<RectTransform>().sizeDelta.y);
        }
        else
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(cAssetBundleTexture.GetUnitTextureWidth() * m_ImageScale, cAssetBundleTexture.GetUnitTextureHeight() * m_ImageScale);
        }
    }

    protected override Texture2D GetTexture(AssetBundlerResponse o)
    {
        if (!useUncompressed)
        {
            return base.GetTexture(o);
        }

        //非圧縮テクスチャの取得
        string[] names = o.GetAllAssetNames();

        if (names.Length == 1)
        {
            Debug.LogError("NORMAL:" + base.GetTexture(o).name);
            return base.GetTexture(o);
        }

        Texture2D uncompressedTexture = o.GetTexture2D(names[1], TextureWrapMode.Clamp);

        if (uncompressedTexture == null)
        {
            Debug.LogError("NORMAL:" + base.GetTexture(o).name);
            return base.GetTexture(o);
        }
        Debug.LogError("HITHIT:" + uncompressedTexture.name);
        return uncompressedTexture;
    }

    public bool IsSetTexture
    {
        get
        {
            return GetComponent<Image>().sprite != null;
        }
    }

    protected override void ResetTexture()
    {
        GetComponent<Image>().sprite = null;
        GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }
}
