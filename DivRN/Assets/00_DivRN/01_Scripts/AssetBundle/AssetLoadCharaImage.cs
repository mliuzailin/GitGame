/**
 *  @file   AssetLoadCharaImage.cs
 *  @brief  アセットバンドルのキャラクタをSpriteで取得するためのクラス
 *  @author Developer
 *  @date   2017/03/18
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AssetLoadCharaImage
{
    public uint m_CharID = 0;
    /// <summary>失敗したかどうか</summary>
    public bool IsFail { get; private set; }
    public bool IsLoading { get; private set; }

    Texture2D m_Texture = null;

    public AssetLoadCharaImage()
    {
        IsFail = false;
        IsLoading = false;
    }

    public void SetCharaID(uint unCharaID)
    {
        if (unCharaID == 0)
        {
            IsLoading = true;
            return;
        }

        m_CharID = unCharaID;
        AssetBundler.Create().SetAsUnitTexture(unCharaID,
                    (o) =>
                    {
                        m_Texture = o.GetTexture2D(TextureWrapMode.Clamp);
                        IsLoading = true;
                    },
                    (str) =>
                    {
                        IsLoading = true;
                        IsFail = true;
                    })
                    .Load();
    }

    public Texture2D GetTexture()
    {
        return m_Texture;
    }

    public Sprite GetSprite()
    {
        return GetSprite(new Rect(0, 0, m_Texture.GetUnitTextureWidth(), m_Texture.GetUnitTextureHeight()), Vector2.zero);
    }

    public Sprite GetSprite(Rect rect, Vector2 pivot)
    {
        if (m_Texture == null) { return null; }
        Sprite sprite = Sprite.Create(m_Texture, rect, pivot);
        return sprite;
    }
}
