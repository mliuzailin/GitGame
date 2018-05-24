/**
 *  @file   MissingListWindow.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/08/03
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class MissingListWindow : EditorWindow
{
    public class AssetParameterData
    {
        public UnityEngine.Object obj { get; set; } //!< アセットのObject自体  
        public string path { get; set; }            //!< アセットのパス
        public string name { get; set; }            //!< プロパティーの表示名
    }

    /// <summary>検索するファイルの拡張子</summary>
    private static string[] m_Extensions = { ".prefab",
                                            ".mat",
                                            ".controller",
                                            ".cs",
                                            ".shader",
                                            ".mask",
                                            ".asset"
                                        };

    private static List<AssetParameterData> m_MissingList = new List<AssetParameterData>();
    private Vector2 m_ScrollPos;

    /// <summary>  
    /// Missingがあるアセットを検索してそのリストを表示する  
    /// </summary>  
    [MenuItem("Tools/SearchMissingList")]
    private static void ShowMissingList()
    {
        // Missingがあるアセットを検索  
        Search();

        // ウィンドウを表示  
        var window = GetWindow<MissingListWindow>();
        window.minSize = new Vector2(900, 300);
    }

    /// <summary>  
    /// Missingがあるアセットを検索  
    /// </summary>  
    private static void Search()
    {
        m_MissingList.Clear();

        // 全てのアセットのファイルパスを取得  
        string[] allPaths = AssetDatabase.GetAllAssetPaths();
        int length = allPaths.Length;

        for (int i = 0; i < length; i++)
        {
            // プログレスバーを表示  
            EditorUtility.DisplayProgressBar("Search Missing", string.Format("{0}/{1}", i + 1, length), (float)i / length);

            // Missing状態のプロパティを検索  
            if (m_Extensions.Contains(Path.GetExtension(allPaths[i])))
            {
                SearchMissing(allPaths[i]);
            }
        }

        // プログレスバーを消す  
        EditorUtility.ClearProgressBar();
    }

    /// <summary>  
    /// 指定アセットにMissingのプロパティがあれば、それをmissingListに追加する  
    /// </summary>  
    /// <param name="path">Path.</param>  
    private static void SearchMissing(string path)
    {
        // 指定パスのアセットを全て取得  
        IEnumerable<UnityEngine.Object> assets = AssetDatabase.LoadAllAssetsAtPath(path);

        // 各アセットについて、Missingのプロパティがあるかチェック  
        foreach (UnityEngine.Object obj in assets)
        {
            if (obj == null)
            {
                continue;
            }
            if (obj.name == "Deprecated EditorExtensionImpl")
            {
                continue;
            }

            // SerializedObjectを通してアセットのプロパティを取得する  
            SerializedObject sobj = new SerializedObject(obj);
            SerializedProperty property = sobj.GetIterator();

            while (property.Next(true))
            {
                // プロパティの種類がオブジェクト（アセット）への参照で、  
                // その参照がnullなのにもかかわらず、参照先インスタンスIDが0でないものはMissing状態！  
                if (property.propertyType == SerializedPropertyType.ObjectReference &&
                    property.objectReferenceValue == null &&
                    property.objectReferenceInstanceIDValue != 0)
                {
                    // Missing状態のプロパティリストに追加する  
                    AssetParameterData assetParameterData = new AssetParameterData();
                    assetParameterData.obj = obj;
                    assetParameterData.path = path;
                    assetParameterData.name = property.displayName;

                    m_MissingList.Add(assetParameterData);
                }
            }
        }
    }

    /// <summary>  
    /// Missingのリストを表示  
    /// </summary>  
    private void OnGUI()
    {
        // 列見出し  
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Asset", GUILayout.Width(200));
        EditorGUILayout.LabelField("Name", GUILayout.Width(200));
        EditorGUILayout.LabelField("Path");
        EditorGUILayout.EndHorizontal();

        // リスト表示  
        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

        foreach (AssetParameterData data in m_MissingList)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(data.obj, data.obj.GetType(), true, GUILayout.Width(200));
            EditorGUILayout.TextField(data.name, GUILayout.Width(200));
            EditorGUILayout.TextField(data.path);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.LabelField("Count: " + m_MissingList.Count, GUILayout.Width(200));
    }
}
