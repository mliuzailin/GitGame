/**
 *  @file   AssetPathCopyer.cs
 *  @brief  Asset以降のパスをクリップボードの貼り付ける
 *  @author Developer
 *  @date   2016/12/07
 */

using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

/// <summary>
/// Assets フォルダに存在するアセットの相対パスを Project ビュー上でコピーするためのクラス
/// </summary>
public static class AssetPathCopyer
{
    private enum Priority   // 優先度
    {
        FROM_ASSETS = 10000,                // Assets フォルダからの相対パスをコピーするコマンド
        FROM_ASSETS_WITH_DOUBLE_QUATES,     // Assets フォルダからの相対パスを二重引用符付きでコピーするコマンド
        FROM_RESOURCES,                     // Resources フォルダからの相対パスをコピーするコマンド
        FROM_RESOURCES_WITH_DOUBLE_QUATES,  // Resources フォルダからの相対パスを二重引用符付きでコピーするコマンド
        FROM_PREFAB,                        // Prefab フォルダからの相対パスをコピーするコマンド
        FROM_SPRITE_NAME,                        // Sprite名をコピーするコマンド
    }

    private const string FROM_ASSETS_ITEM_NAME = @"Assets/Copy Path/From Assets";              // Assets フォルダからの相対パスをコピーするコマンドの名前
    private const string FROM_ASSETS_WITH_DOUBLE_QUATES_ITEM_NAME = @"Assets/Copy Path/From Assets With """"";    // Assets フォルダからの相対パスを二重引用符付きでコピーするコマンドの名前
    private const string FROM_RESOURCES_ITEM_NAME = @"Assets/Copy Path/From Resources";           // Resources フォルダからの相対パスをコピーするコマンドの名前
    private const string FROM_RESOURCES_WITH_DOUBLE_QUATES_ITEM_NAME = @"Assets/Copy Path/From Resources With """""; // Resources フォルダからの相対パスを二重引用符付きでコピーするコマンドの名前
    private const string FROM_PREFAB_ITEM_NAME = @"Assets/Copy Path/From Prefab";           // Prefab フォルダからの相対パスをコピーするコマンドの名前
    private const string FROM_SPRITE_NAME = @"Assets/Copy Path/Sprite Name";           // Sprite名をコピーするコマンドの名前

    private static string SelectAssetPath { get { return AssetDatabase.GetAssetPath(Selection.objects[0]); } } // 選択中のオブジェクトの Assets フォルダからの相対パスを取得します
    private static bool IsSelectAsset { get { return Selection.objects != null && 0 < Selection.objects.Length; } } // 選択中のオブジェクトが存在するかどうかを返します

    /// <summary>
    /// Assets フォルダからの相対パスをコピーします
    /// </summary>
    [MenuItem(FROM_ASSETS_ITEM_NAME, false, (int)Priority.FROM_ASSETS)]
    private static void FromAssets()
    {
        EditorGUIUtility.systemCopyBuffer = SelectAssetPath;
    }

    /// <summary>
    /// Assets フォルダからの相対パスを二重引用符付きでコピーします
    /// </summary>
    [MenuItem(FROM_ASSETS_WITH_DOUBLE_QUATES_ITEM_NAME, false, (int)Priority.FROM_ASSETS_WITH_DOUBLE_QUATES)]
    private static void FromAssetsWithDoubleQuates()
    {
        EditorGUIUtility.systemCopyBuffer = AddDoubleQuates(SelectAssetPath);
    }

    /// <summary>
    /// Resources フォルダからの相対パスをコピーします
    /// </summary>
    [MenuItem(FROM_RESOURCES_ITEM_NAME, false, (int)Priority.FROM_RESOURCES)]
    private static void FromResources()
    {
        EditorGUIUtility.systemCopyBuffer = ToPathFromResources(SelectAssetPath);
    }

    /// <summary>
    /// Resources フォルダからの相対パスを二重引用符付きでコピーします
    /// </summary>
    [MenuItem(FROM_RESOURCES_WITH_DOUBLE_QUATES_ITEM_NAME, false, (int)Priority.FROM_RESOURCES_WITH_DOUBLE_QUATES)]
    private static void FromResourcesWithDoubleQuates()
    {
        EditorGUIUtility.systemCopyBuffer = AddDoubleQuates(ToPathFromResources(SelectAssetPath));
    }

    /// <summary>
    /// Resources フォルダからの相対パスをコピーします
    /// </summary>
    [MenuItem(FROM_PREFAB_ITEM_NAME, false, (int)Priority.FROM_PREFAB)]
    private static void FromPrefab()
    {
        EditorGUIUtility.systemCopyBuffer = ToPathFromPrefab(SelectAssetPath);
    }

    /// <summary>
    /// スプライト名をコピーします
    /// </summary>
    [MenuItem(FROM_SPRITE_NAME, false, (int)Priority.FROM_SPRITE_NAME)]
    private static void FormSpriteName()
    {
        UnityEngine.Object obj = Selection.objects[0];
        if (obj.GetType() == typeof(UnityEngine.Sprite))
        {
            EditorGUIUtility.systemCopyBuffer = obj.name;
        }
    }

    /// <summary>
    /// アセットのパスをコピーできるかどうかを確認します
    /// </summary>
    [MenuItem(FROM_ASSETS_ITEM_NAME, true)]
    [MenuItem(FROM_ASSETS_WITH_DOUBLE_QUATES_ITEM_NAME, true)]
    [MenuItem(FROM_RESOURCES_ITEM_NAME, true)]
    [MenuItem(FROM_RESOURCES_WITH_DOUBLE_QUATES_ITEM_NAME, true)]
    [MenuItem(FROM_PREFAB_ITEM_NAME, true)]
    [MenuItem(FROM_SPRITE_NAME, true)]
    private static bool Validate()
    {
        return IsSelectAsset;
    }

    /// <summary>
    /// 指定された文字列を Resources フォルダからの相対パスに変換します
    /// </summary>
    private static string ToPathFromResources(string str)
    {
        str = Regex.Replace(str, @"^.*Resources/", ""); // Resourcesフォルダまでのパスを削除します
        str = string.Format("{0}/{1}", Path.GetDirectoryName(str), Path.GetFileNameWithoutExtension(str)); // 拡張子を削除します
        str = str.StartsWith(@"/") ? str.Remove(0, 1) : str;
        return str;
    }

    /// <summary>
    /// 指定された文字列を Prefab フォルダからの相対パスに変換します
    /// </summary>
    private static string ToPathFromPrefab(string str)
    {
        str = Regex.Replace(str, @"^.*Prefab/", ""); // Resourcesフォルダまでのパスを削除します
        str = string.Format("{0}/{1}", Path.GetDirectoryName(str), Path.GetFileNameWithoutExtension(str)); // 拡張子を削除します
        str = str.StartsWith(@"/") ? str.Remove(0, 1) : str;
        return str;
    }


    /// <summary>
    /// 指定された文字列に二重引用符を追加します
    /// </summary>
    private static string AddDoubleQuates(string str)
    {
        return string.Format(@"""{0}""", str);
    }
}
