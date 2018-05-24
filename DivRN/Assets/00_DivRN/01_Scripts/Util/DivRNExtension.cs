using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DivRNExtension
{
    public static Sprite CreateSprite(this Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    public static int GetUnitTextureWidth(this Texture2D texture)
    {
        if (LocalSaveManagerRN.Instance.QualitySetting == QualitySetting.HIGH)
        {
            return texture.width / 2;
        }

        return texture.width;
    }

    public static int GetUnitTextureHeight(this Texture2D texture)
    {
        if (LocalSaveManagerRN.Instance.QualitySetting == QualitySetting.HIGH)
        {
            return texture.height / 2;
        }

        return texture.height;
    }
}
