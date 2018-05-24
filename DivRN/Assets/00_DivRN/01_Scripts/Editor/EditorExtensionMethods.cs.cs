using UnityEngine;
using UnityEditor;

/// <summary>
/// UnityEditorを使用する拡張メソッド
/// </summary>
public static class EditorExtensionMethods
{
    /// <summary>
    /// プレハブかどうかを判定
    /// </summary>
    /// <param name="self">対象</param>
    /// <returns>プレハブならtrue, otherwise false</returns>
    /// <remarks>
    /// taken from: http://answers.unity3d.com/questions/218429/how-to-know-if-a-gameobject-is-a-prefab.html 
    /// GetPrefabParent() = A, GetPrefabObject() = Bとすると、
    ///                                          |   A      |     B     |
    /// 1. projectビューにあるプレハブ            |   null    | non-null |
    /// 2. hierarchyにインスタンス化されたプレハブ | non-null | non-null  |
    /// 3. hierarchyにある通常のGameObject        |   null   |   null   |
    /// になっている模様
    /// </remarks>
    public static bool IsPrefab(this GameObject self)
    {
        return PrefabUtility.GetPrefabParent(self) == null && PrefabUtility.GetPrefabObject(self) != null;
    }

    public static bool IsPrefabInstance(this GameObject self)
    {
        return PrefabUtility.GetPrefabParent(self) != null && PrefabUtility.GetPrefabObject(self) != null;
    }
}