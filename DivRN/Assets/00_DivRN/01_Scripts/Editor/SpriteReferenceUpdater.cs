/**
 *  @file   SpriteReferenceUpdater.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/08/01
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

public class SpriteReferenceUpdater : Editor
{
    public const string SCRITABLE_OBJECT_PATH = "Assets/00_DivRN/02_Resources/Resources/SpriteReference/{0}SpriteReference.asset";
    /// <summary>アトラス名</summary>
    static string[] AtlasNameArray = new string[] {
            "Battle",
            "Common",
            "Menu",
            "Title"
        };

    /// <summary>
    /// 全てのSpriteReferenceを更新する
    /// </summary>
    [MenuItem("Tools/SetUpSpriteReference")]
    public static void SetUpAllSpriteReference()
    {
        foreach (string typeStr in AtlasNameArray)
        {
            string atlasPath = "Assets/00_DivRN/02_Resources/Atlas/" + typeStr;
            string spriteReferencePath = string.Format(SCRITABLE_OBJECT_PATH, typeStr);
            SetUpSpriteReference(atlasPath, spriteReferencePath);
        }

        // ローディングのSpriteReferenceを更新
        SetUpSpriteReference("Assets/00_DivRN/02_Resources/UIData/Effect/Loading", "");

        EditorUtility.DisplayDialog(
                "確認",
                "SpriteReferenceの更新が完了しました。" + "\n"
                 + "Ctrl + S などをして状態を保存してください。",
                "OK"
                );
    }

    /// <summary>
    /// SpriteReferenceを更新する
    /// </summary>
    /// <param name="atlasFolderPath"></param>
    /// <param name="spriteReferencePath"></param>
    public static void SetUpSpriteReference(string atlasFolderPath, string spriteReferencePath)
    {
        if (!Directory.Exists(atlasFolderPath))
        {
            Debug.LogError("フォルダ:" + atlasFolderPath + "は存在しません");
            return;
        }
        string[] filePathArray = Directory.GetFiles(atlasFolderPath);
        List<Sprite> spriteList = new List<Sprite>();
        foreach (string filePath in filePathArray)
        {
            //---------------------------------------------
            // アトラスのSpriteを取得
            //---------------------------------------------
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (!fileName.StartsWith("sheet"))
            {
                continue;
            }
            if (fileName.EndsWith("_mask"))
            {
                continue;
            }
            if (fileName.EndsWith("_spriterenderer"))
            {
                continue;
            }

            UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath(filePath);
            if (asset != null)
            {
                List<Sprite> sheetSpriteList = new List<Sprite>();
                foreach (UnityEngine.Object item in asset)
                {
                    if (item.GetType() != typeof(Sprite))
                    {
                        continue;
                    }
                    sheetSpriteList.Add((Sprite)item);
                }

                sheetSpriteList.Sort((a, b) => string.Compare(a.name, b.name));
                if (sheetSpriteList.Count > 0)
                {
                    spriteList.AddRange(sheetSpriteList);

                    //---------------------------------------------
                    // アトラステクスチャごとのScriptableObjectを更新
                    //---------------------------------------------
                    string sheetScritableObjectPath = Path.GetDirectoryName(filePath) + "/" + fileName + ".asset";
                    if (File.Exists(sheetScritableObjectPath))
                    {
                        SpriteReference sheetScritableObject = AssetDatabase.LoadAssetAtPath<SpriteReference>(sheetScritableObjectPath);
                        if (sheetScritableObject != null)
                        {
                            sheetScritableObject.sprites = sheetSpriteList;
                            // 更新
                            EditorUtility.SetDirty(sheetScritableObject);
                        }
                    }
                    else
                    {
                        Debug.LogError(sheetScritableObjectPath + "は存在しません");
                    }
                }
            }

        }

        //---------------------------------------------
        // ScriptableObjectを更新
        //---------------------------------------------
        if (!spriteReferencePath.IsNullOrEmpty())
        {
            if (File.Exists(spriteReferencePath))
            {
                SpriteReference scritableObject = AssetDatabase.LoadAssetAtPath<SpriteReference>(spriteReferencePath);
                if (scritableObject != null)
                {
                    scritableObject.sprites = spriteList;
                    // 更新
                    EditorUtility.SetDirty(scritableObject);
                }
            }
            else
            {
                Debug.LogError(spriteReferencePath + "は存在しません");
            }
        }

    }
}
