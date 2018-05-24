using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using System; //Exception
using System.Text; //Encoding

//using Jemast.LocalCache;
public class BuildEditor : EditorWindow
{

    [MenuItem("Tools/ReverseSceneOrder")]
    public static void ReverseSceneOrder()
    {
        List<EditorBuildSettingsScene> list = EditorBuildSettings.scenes.ToList();
        list.Reverse();

        EditorBuildSettings.scenes = list.ToArray();
    }


    private static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }

    [MenuItem("Tools/Build/Build_iOS")]
    public static void Build_iOS()
    {
        PlayerSettings.applicationIdentifier = "jp.example.dgrn.dev";

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenePaths();
        buildPlayerOptions.locationPathName = "iOS";
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.AcceptExternalModificationsToPlayer;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    [MenuItem("Tools/Build/Build_Android")]
    public static void Build_Android()
    {
        Build_Setting(BuildTarget.Android);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenePaths();
        buildPlayerOptions.locationPathName = "Android.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    [MenuItem("Tools/Build/Build_Windows")]
    public static void Build_Windows()
    {
        PlayerSettings.defaultIsFullScreen = false;
        PlayerSettings.defaultScreenHeight = 960;
        PlayerSettings.defaultScreenWidth = 640;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
        PlayerSettings.resizableWindow = true;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenePaths();
        buildPlayerOptions.locationPathName = "Windows/dgrn.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows;
        buildPlayerOptions.options = BuildOptions.Development;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    [MenuItem("Tools/Clean/DeleteAll_Playerprefs")]
    public static void Reset_Playerprefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Tools/Path/TemporaryCachePath")]
    public static void Log_TemporaryCachePath()
    {
        Debug.Log("temporaryCachePath: " + Application.temporaryCachePath);
    }

    [MenuItem("Tools/Path/PersistentDataPath")]
    public static void Log_PersistentDataPath()
    {
        Debug.Log("persistentDataPath: " + Application.persistentDataPath);
    }

    [MenuItem("Tools/Path/DataPath")]
    public static void Log_DataPath()
    {
        Debug.Log("dataPath: " + Application.dataPath);
    }

    public static void Build_iOS_Batch_ScriptOnly()
    {
        Build_Setting(BuildTarget.iOS);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenePaths();
        buildPlayerOptions.locationPathName = "iOS";
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.BuildScriptsOnly;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    public static void Build_iOS_Batch()
    {
        Build_Setting(BuildTarget.iOS);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenePaths();
        buildPlayerOptions.locationPathName = "iOS";
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.AcceptExternalModificationsToPlayer;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    public static void Build_Android_Batch()
    {
        Build_Setting(BuildTarget.Android);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenePaths();
        buildPlayerOptions.locationPathName = "Android.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    public static bool JenkinsBuild = false;

    private static void User_Build_Setting(BuildTarget target)
    {
        //アプリケーション設定
        string appPath = System.IO.Directory.GetCurrentDirectory();
        string[] settings = OnLoadSettong("user.txt");

        if (settings.Length == 5 || settings.Length == 6)
        {
            PlayerSettings.applicationIdentifier = settings[0];

            if (target == BuildTarget.iOS)
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, settings[0]);

                if (PlayerSettings.applicationIdentifier == "jp.example.dg")
                {
                    PlayerSettings.iOS.allowHTTPDownload = false;
                }
            }

            if (target == BuildTarget.Android)
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, settings[0]);

                PlayerSettings.Android.keystoreName = appPath + "/user.keystore";
                PlayerSettings.Android.keystorePass = settings[1];
                PlayerSettings.Android.keyaliasName = settings[2];
                PlayerSettings.Android.keyaliasPass = settings[3];
            }

            PlayerSettings.productName = settings[4];

            if (settings.Length == 6)
            {
                if (target == BuildTarget.Android)
                {
                    PlayerSettings.Android.bundleVersionCode = Int32.Parse(settings[5]);
                }
                else if (target == BuildTarget.iOS)
                {
                    PlayerSettings.iOS.buildNumber = settings[5];
                }
            }
        }
        else
        {
            Debug.Log("Error user.txt: " + settings.Length);
        }
    }

    public static void Build_Setting(BuildTarget target)
    {
        JenkinsBuild = true;

        User_Build_Setting(target);

        if (target == BuildTarget.Android)
        {
            //AndroidManifest.xmlの書き換え
            OnSettingAndroidManifestXml(PlayerSettings.applicationIdentifier);
        }

        BuildTargetGroup targetgrout = BuildTargetGroup.Unknown;
        switch (target)
        {
            case BuildTarget.iOS:
                targetgrout = BuildTargetGroup.iOS;
                break;

            case BuildTarget.Android:
                targetgrout = BuildTargetGroup.Android;
                break;

            default:
                break;
        }
#if false
		Debug.Log("keystoreName:" + PlayerSettings.Android.keystoreName);
		Debug.Log("bundleIdentifier:" + PlayerSettings.bundleIdentifier);
		Debug.Log("keystorePass:" + PlayerSettings.Android.keystorePass);
		Debug.Log("keyaliasName:" + PlayerSettings.Android.keyaliasName);
		Debug.Log("keyaliasPass:" + PlayerSettings.Android.keyaliasPass);
#endif

        //ビルド設定
        string[] buildsettings = OnLoadSettong("build.txt");
        string definesymbols = "";
        if (buildsettings.Length == 2)
        {
            if (targetgrout == BuildTargetGroup.Android)
            {
                definesymbols = buildsettings[0];
            }
            else if (targetgrout == BuildTargetGroup.iOS)
            {
                definesymbols = buildsettings[1];
            }
        }
        else
        {
            Debug.Log("Error build.txt: " + buildsettings.Length);
        }

        // 複数設定時はセミコロンを付ける
        // BUILD_TYPE_DEBUG;DEF_XXXX
        if (definesymbols.Length > 0)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetgrout, definesymbols);
        }

        if (targetgrout != BuildTargetGroup.Unknown)
        {
            Debug.Log("GetScriptingDefineSymbolsForGroup: " + PlayerSettings.GetScriptingDefineSymbolsForGroup(targetgrout));
        }
    }

    private static void OnSettingAndroidManifestXml(string bundleIdentifier)
    {
        string appPath = System.IO.Directory.GetCurrentDirectory();
        Debug.Log("path:" + appPath);

        // PUSH com.google.android.c2dm.intent.RECEIVE　and C2D_MESSAGE のパッケージ名書き換え
        string baseIdentifier = "jp.example.dgrn";
        string pathAndroidxml = appPath + "/Assets/Plugins/Android/AndroidManifest.xml";
        Debug.Log("pathAndroidxml: " + pathAndroidxml);

        StreamReader sr = new StreamReader(pathAndroidxml, Encoding.GetEncoding("UTF-8"));
        string inSetting = sr.ReadToEnd();
        sr.Close();
        //Debug.Log("pathinSetting " + inSetting);

        string outSetting = inSetting.Replace(baseIdentifier, bundleIdentifier);
        //Debug.Log("pathoutSetting: " + outSetting);

        StreamWriter sw = new StreamWriter(pathAndroidxml, false, Encoding.GetEncoding("UTF-8"));
        sw.Write(outSetting);
        sw.Close();
    }

    public static string[] OnLoadSettong(string filename)
    {
        string appPath = System.IO.Directory.GetCurrentDirectory();
        Debug.Log("path:" + appPath);

        string[] settings = new string[0];
        FileInfo fi = new FileInfo(appPath + "/" + filename);
        try
        {
            // 一行毎読み込み
            string tempLine;
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                tempLine = sr.ReadToEnd();
            }

            tempLine = tempLine.Replace(Environment.NewLine, "\r");
            tempLine = tempLine.Trim('\r');
            settings = tempLine.Split('\r');
        }
        catch (Exception e)
        {
            Debug.Log("Exception:" + e.ToString());
        }

        return settings;
    }
}
