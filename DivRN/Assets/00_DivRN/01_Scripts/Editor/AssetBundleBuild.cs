using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExportAssetBundles
{
    [MenuItem("AssetBundles/Build")]
    static void BuildAssetBundles()
    {
        string outputPath = "AssetBundles";
        BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression;
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget; ;
        BuildPipeline.BuildAssetBundles(outputPath, options, target);
    }
}
