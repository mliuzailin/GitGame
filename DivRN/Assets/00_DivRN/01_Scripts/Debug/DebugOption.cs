using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UniExtensions;
using System;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
public class DebugOption : SingletonComponent<DebugOption>
{
    [System.Serializable]
    public class TutorialDebugOption
    {
        public TutorialPart forceTutorialPart;
        //public bool disable;
        public bool skip;
    }

    public TutorialDebugOption tutorialDO;

    public bool disalbeDebugMenu;

    public bool noneNextVersion;

    public bool featureQuest;	// 未来クエスト出現（この値が変化したときは、TimeEventManager.Instance.TimeEventUpdateRequest()を呼ぶ）

#if UNITY_EDITOR && BUILD_TYPE_DEBUG 

    public bool AutoLoginUser = false;

    public bool deleteAllPlayerPrefs;

    //<! SceneCommonでログイン処理をする。
    [System.Serializable]
    public class AssetBundleDebugOption
    {
        public bool alwaysLoadFromCache;
        public bool disableCache;
    }

    protected override void Start()
    {
        if (deleteAllPlayerPrefs)
        {
            PlayerPrefs.DeleteAll();
        }
    }

    [System.Serializable]
    public class PatcherdebugOption
    {
        public bool alwaysFail;
        public bool alwaysCompitableAppVersion = false;

    }

    public PatcherdebugOption patcherDO;
    public AssetBundleDebugOption assetBundleDO;

    public bool skipInitialMask;

    public bool enableLog;

#if UNITY_EDITOR

    [CustomEditor(typeof(DebugOption))]
    public class DebugOptionSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayoutOption option = GUILayout.Width(300f);
            DrawDefaultInspector();
            DebugOption instance = target as DebugOption;
            if (GUILayout.Button("UnloadUnusedAssets", option))
            {
                Resources.UnloadUnusedAssets();
            }
        }
    }
#endif
#endif
}
#endif