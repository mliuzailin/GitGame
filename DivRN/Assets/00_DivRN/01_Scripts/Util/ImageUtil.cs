/**
 * 	@file	ImageUtil.cs
 *	@brief	画像関連の処理
 *	@author Developer
 *	@date	2016/11/10
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class ImageUtil
{
    /// <summary>
    /// スプライトの読み込み
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="spriteName">スプライトの名前</param>
    /// <returns></returns>
    public static Sprite[] LoadSprites(string fileName, string[] spriteNames)
    {
        if (spriteNames == null && spriteNames.Length == 0)
        {
            return Resources.LoadAll<Sprite>(fileName);
        }
        else
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(fileName);
            return Array.FindAll(sprites, s => s.name.Equals(spriteNames));
        }
    }

    /// <summary>
    /// スプライトの配列から指定の名前のスプライトを取得する
    /// </summary>
    /// <param name="sprites">スプライトの配列</param>
    /// <param name="spriteName">スプライトの名前</param>
    /// <returns></returns>
    public static Sprite GetSprite(Sprite[] sprites, string spriteName)
    {
        if (sprites.Length == 0)
        {
            return null;
        }

        return Array.Find(sprites, s => s.name.Equals(spriteName));
    }


    private static Dictionary<string, Material> s_materialCache = new Dictionary<string, Material>();
    public static void GetMaterial(string path, System.Action<Material> callback)
    {
        var material = Resources.Load(path, typeof(Material)) as Material;
        if (material == null)
        {
            callback(null);
            return;
        }


        if (!s_materialCache.ContainsKey(path))
            s_materialCache[path] = material;

        callback(s_materialCache[path]);
    }
}
