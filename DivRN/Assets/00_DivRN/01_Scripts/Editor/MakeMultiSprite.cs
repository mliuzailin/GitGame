using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class MakeMultiSprite
{
    [MenuItem("Assets/DivRN/Make/MakeMultiSpriteUnitIcon")]
    public static void MakeMultiSpriteUnitIcon()
    {
        if (Selection.assetGUIDs == null ||
            Selection.assetGUIDs.Length == 0)
        {
            return;
        }

        var imagePath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

        var importer = (TextureImporter)TextureImporter.GetAtPath(imagePath);
        if (importer.textureType != TextureImporterType.Sprite)
        {
            return;
        }

        object[] args = new object[2] { 0, 0 };
        var method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(importer, args);
        if ((int)args[0] != 2048 &&
            (int)args[1] != 2048)
        {
            return;
        }

        List<SpriteMetaData> metas = new List<SpriteMetaData>();
        for (int count = 0; count < 225; count++)
        {
            SpriteMetaData meta = new SpriteMetaData();
            int cx = count % 15;
            int cy = count / 15;
            int x = (8 + cx * 128) + (cx * 8);
            int y = (8 + cy * 128) + (cy * 8);
            int w = 128;
            int h = 128;
            meta.name = "sprite" + (count + 1).ToString();
            meta.rect = new Rect(x, y, w, h);
            meta.pivot = new Vector2(0.5f, 0.5f);
            metas.Add(meta);
        }

        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritesheet = metas.ToArray();

        // appry
        EditorUtility.SetDirty(importer);
        AssetDatabase.ImportAsset(imagePath, ImportAssetOptions.ForceUncompressedImport);
    }
}
