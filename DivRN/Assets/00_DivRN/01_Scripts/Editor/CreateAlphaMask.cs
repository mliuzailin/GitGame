using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

class CreateAlphaMask : AssetPostprocessor
{
    /// <summary>除外するパス</summary>
    static string[] m_ExcludePaths = new string[] {
        "Assets/00_DivRN/02_Resources/Resources/Font"
    };

    static TextureFormat CompressionFormat
    {
        get
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    return TextureFormat.ETC_RGB4;
                case BuildTarget.iOS:
                    return TextureFormat.PVRTC_RGB4;
                default:
                    return TextureFormat.DXT1;
            }
        }
    }

    private string Platform
    {
        get
        {
#if UNITY_ANDROID
            return "Android";
#elif UNITY_iOS
            return "iPhone";
#else
            return NOUtil.Platform;
#endif
        }
    }

    void OnPostprocessTexture(Texture2D texture)
    {
        //------------------------------------
        // 除外するパスに指定してある場合は、はじく
        //------------------------------------
        for (int i = 0; i < m_ExcludePaths.Length; i++)
        {
            if (assetPath.StartsWith(m_ExcludePaths[i]))
            {
                Debug.Log("AssetPath:" + assetPath);
                return;
            }
        }

        TextureImporter importer = (assetImporter as TextureImporter);
        string format = importer.GetAutomaticFormat(Platform).ToString();

        if (!assetPath.EndsWith(".png"))
        {
            Debug.Log("AssetPath:" + assetPath);
            return;
        }

        if (assetPath.EndsWith("_mask.png"))
        {
            Debug.Log("AssetPath:" + assetPath);
            return;
        }

#if true
        // アルファ抜きの輪郭部分の画質改善
        if (texture.format == TextureFormat.RGBA32
            && importer.textureType == TextureImporterType.Sprite
            && importer.spriteImportMode == SpriteImportMode.Multiple
        )
        {
            Debug.Log("Fill transparent..");
            Texture2D texture2 = _fillTransparent(texture);
            byte[] texture2_bytes = texture2.EncodeToPNG();
            File.WriteAllBytes(assetPath, texture2_bytes);
            return;
        }
#endif

        if (!format.Equals("Alpha8"))
        {
            Debug.Log("FORMAT:" + format);
            return;
        }

        Debug.Log("Create Mask..");
        Texture2D mask = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
        mask.wrapMode = TextureWrapMode.Clamp;

        // Convert the source image into a mask.
        var pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            var a = pixels[i].a;
            pixels[i] = new Color(a, a, a, a);
        }
        mask.SetPixels(pixels);

        byte[] bytes = mask.EncodeToPNG();

        //        File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
        //        EditorUtility.CompressTexture(mask, CompressionFormat, TextureCompressionQuality.Best);

        //        var maskPath = assetPath.Replace(".png", string.Format("_mask_{0}.asset",CompressionFormat.ToString()));
        var maskPath = assetPath.Replace(".png", "_mask.png");
        File.WriteAllBytes(maskPath, bytes);

        AssetDatabase.Refresh();

        //        var maskAsset = AssetDatabase.LoadAssetAtPath(maskPath, typeof(Texture2D)) as Texture2D;
        //        if (maskAsset == null)
        //        {
        //            AssetDatabase.CreateAsset(mask, maskPath);
        //        }
        //        else
        //        {
        //            EditorUtility.CopySerialized(mask, maskAsset);
        //        }

    }


    /// <summary>
    /// 不透明部分のふちの色で透明部分を塗る
    /// </summary>
    /// <param name="src_texture">元のテクスチャ（RGBA32である必要がある）</param>
    /// <param name="solid_alpha">不透明と判定する最低アルファ値</param>
    /// <param name="fat_size">ふちから何ドット分塗るか（大きくしすぎるとインポートに時間がかかる）</param>
    /// <returns></returns>
    private static Texture2D _fillTransparent(Texture2D src_texture, int solid_alpha = 17, int fat_size = 8)
    {
        int width = src_texture.width;
        int height = src_texture.height;

        Color32[] src_pixels = src_texture.GetPixels32();
        bool[] src_fixs = new bool[src_pixels.Length];
        bool[] update_fixs = new bool[src_pixels.Length];

        for (int idx = 0; idx < src_pixels.Length; idx++)
        {
            if (src_pixels[idx].a >= solid_alpha)
            {
                src_fixs[idx] = true;
            }
        }

        int[] offs = { 1, width, -1, -width };

        for (int loop = 0; loop < fat_size; loop++)
        {
            for (int idx = 0; idx < src_pixels.Length; idx++)
            {
                if (src_fixs[idx] == false)
                {
                    int idx_x = idx % width;

                    for (int off_idx = 0; off_idx < offs.Length; off_idx++)
                    {
                        int count = 0;
                        int r = 0;
                        int g = 0;
                        int b = 0;

                        int idx2 = idx + offs[off_idx];
                        if (idx2 >= 0 && idx2 < src_fixs.Length)
                        {
                            int idx2_x = idx2 % width;
                            int dif_x = idx_x - idx2_x;
                            if (dif_x >= -1 && dif_x <= 1)
                            {
                                if (src_fixs[idx2])
                                {
                                    count++;
                                    r += src_pixels[idx2].r;
                                    g += src_pixels[idx2].g;
                                    b += src_pixels[idx2].b;
                                }
                            }
                        }

                        if (count > 0)
                        {
                            r = Mathf.Min(255, (r * 2 + 1) / (count * 2));
                            g = Mathf.Min(255, (g * 2 + 1) / (count * 2));
                            b = Mathf.Min(255, (b * 2 + 1) / (count * 2));

                            src_pixels[idx].r = (byte)r;
                            src_pixels[idx].g = (byte)g;
                            src_pixels[idx].b = (byte)b;
                            //src_pixels[idx].a = 0;

                            update_fixs[idx] = true;
                        }
                    }
                }
            }

            bool is_update = false;
            for (int idx = 0; idx < update_fixs.Length; idx++)
            {
                if (update_fixs[idx])
                {
                    src_fixs[idx] = true;
                    update_fixs[idx] = false;
                    is_update = true;
                }
            }

            if (is_update == false)
            {
                break;
            }
        }

        // 塗りつぶされなかったところは透明な灰色で埋めておく
        for (int idx = 0; idx < src_pixels.Length; idx++)
        {
            if (src_fixs[idx] == false)
            {
                src_pixels[idx].r = 128;
                src_pixels[idx].g = 128;
                src_pixels[idx].b = 128;
                src_pixels[idx].a = 0;
            }
        }

        Texture2D dest_tex = new Texture2D(src_texture.width, src_texture.height, TextureFormat.RGBA32, false);
        dest_tex.SetPixels32(src_pixels);
        dest_tex.Apply();
        dest_tex.alphaIsTransparency = src_texture.alphaIsTransparency;
        dest_tex.anisoLevel = src_texture.anisoLevel;
        dest_tex.filterMode = src_texture.filterMode;
        dest_tex.hideFlags = src_texture.hideFlags;
        dest_tex.name = src_texture.name;
        dest_tex.wrapMode = src_texture.wrapMode;

        return dest_tex;
    }
}
