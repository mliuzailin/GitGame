using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class CreateMenuScriptableObject
{

    public static void CreateScriptableObject<T>() where T : ScriptableObject
    {
        if (Selection.assetGUIDs == null || Selection.assetGUIDs.Length == 0)
        {
            return;
        }

        string selectGuid = (Selection.assetGUIDs)[0];
        string path = AssetDatabase.GUIDToAssetPath(selectGuid);
        Debug.Log(path);

        T newData = ScriptableObject.CreateInstance<T>();

        int _index = 0;

        string dataPath = string.Format("{0}/{1}{2:0000}.asset", path, typeof(T).ToString(), _index);
        while (File.Exists(dataPath))
        {
            _index++;
            dataPath = string.Format("{0}/{1}{2:0000}.asset", path, typeof(T).ToString(), _index);
        }
        Debug.Log(dataPath);

        AssetDatabase.CreateAsset(newData, dataPath);
        AssetDatabase.SaveAssets();

    }

    [MenuItem("Assets/DivRN/Create/MenuButtonItem")]
    public static void CreateFooterSubMenuItem()
    {
        CreateScriptableObject<MasterMenuButtonItem>();
    }

    [MenuItem("Assets/DivRN/Create/MainMenuSeq")]
    public static void CreateMainMenuSeq()
    {
        CreateScriptableObject<MasterMainMenuSeq>();
    }

    [MenuItem("Assets/DivRN/Create/GlobalMenuSeq")]
    public static void CreateGlobalMenuSeq()
    {
        CreateScriptableObject<MasterGlobalMenuSeq>();
    }
}
