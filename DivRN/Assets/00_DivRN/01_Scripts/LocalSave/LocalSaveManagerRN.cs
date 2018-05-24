using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QualitySetting
{
    NORMAL,
    HIGH
}

public static class QualitySettingExtension
{
    public static string GetAssetBundleSuffix(this QualitySetting set)
    {
        return "quality-" + set.ToString().ToLower();
    }
}

public class LocalSaveManagerRN : PP
{
    private static string QUALITY_SETTING_KEY = "LocalSaveManagerRN_QUALITY_SETTING";
    private static string PATCHER_COUNTER_KEY = "LocalSaveManagerRN_PATCHER_COUNTER";
    private static string TUTORIAL_DIALOG_ISSHOW_KEY_FORMAT = "LocalSaveManagerRN_TUTORIAL_DIALOG_ISSHOW_KEY_{0}";

    private static LocalSaveManagerRN instance = new LocalSaveManagerRN();

    public bool GetIsShowTutorialDialog(TutorialDialog.FLAG_TYPE t)
    {
        return GetBool(string.Format(TUTORIAL_DIALOG_ISSHOW_KEY_FORMAT, t));
    }

    public void SetIsShowTutorialDialog(TutorialDialog.FLAG_TYPE t, bool v)
    {
        SetBool(string.Format(TUTORIAL_DIALOG_ISSHOW_KEY_FORMAT, t), v);
    }

    public int PatcherCounter
    {
        get
        {
            return GetInt(PATCHER_COUNTER_KEY, 0);
        }
        set
        {
            SetInt(PATCHER_COUNTER_KEY, value);
        }
    }


    public static LocalSaveManagerRN Instance
    {
        get
        {
            return instance;
        }
    }


    public QualitySetting QualitySetting
    {
        get
        {
            return (QualitySetting)GetInt(QUALITY_SETTING_KEY);
        }
        set
        {
            SetInt(QUALITY_SETTING_KEY, (int)value);
        }
    }
}
