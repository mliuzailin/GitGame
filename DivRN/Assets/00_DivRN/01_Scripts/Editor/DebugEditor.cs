using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

// using LinqTools;
using System.Reflection;
using SqlCipher4Unity3D;
using UnityEditor;


// %	command
// #	shift
// &	optio
public class DebugEditor : EditorWindow
{
    [MenuItem("Debug/OpenScene_Title &#%0")]
    public static void OpenScene_Title()
    {
        OpenScene("Title");
    }

    [MenuItem("Debug/OpenScene_MainMenu &#%1")]
    public static void OpenScene_MainMenu()
    {
        OpenScene("MainMenu");
    }

    [MenuItem("Debug/OpenScene_GameMain &#%3")]
    public static void OpenScene_GameMain()
    {
        OpenScene("GameMain");
    }

    [MenuItem("Debug/OpenScene_Quest2 &#%4")]
    public static void OpenScene_Quest2()
    {
        OpenScene("Quest2");
    }

    private static void OpenScene(string scene)
    {
        EditorApplication.OpenScene(string.Format("Assets/00_DivRN/00_Scenes/Scene{0}.unity", scene));
    }

    // PrefabのApply
    [MenuItem("KeyRemap/PrefabApply &a")]
    static void KeyRemapPrefabApply()
    {
        CommonExecuteMenuItem("GameObject/Apply Changes To Prefab");
    }

    static void CommonExecuteMenuItem(string iStr)
    {
        EditorApplication.ExecuteMenuItem(iStr);
    }
}
