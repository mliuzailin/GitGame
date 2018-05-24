/**
 *  @file   MissonGroupListItemContext.cs
 *  @brief  クエストのミッショングループリストアイテムのM4uContext
 *  @author Developer
 *  @date   2016/11/14
 */

using UnityEngine;
using System;
using System.Collections;
using ServerDataDefine;
using M4u;

public class MissonGroupListItemContext : M4uContext
{
    /// <summary>
    /// アイテムを選択したときのアクション
    /// </summary>
    public Action<MissonGroupListItemContext> DidSelectItem = delegate { };

    M4uProperty<PacketStructAchievementGroup> groupListData = new M4uProperty<PacketStructAchievementGroup>();
    /// <summary>
    /// ミッショングループデータ
    /// </summary>
    public PacketStructAchievementGroup GroupListData
    {
        get
        {
            return groupListData.Value;
        }
        set
        {
            groupListData.Value = value;
        }
    }

    M4uProperty<string> detailText = new M4uProperty<string>();
    /// <summary>
    /// グループ名
    /// </summary>
    public string DetailText
    {
        get
        {
            return detailText.Value;
        }
        set
        {
            detailText.Value = value;
        }
    }

    /// <summary>
    /// 取得などの情報
    /// </summary>
    M4uProperty<MasterDataDefineLabel.AchievementState> achievementState = new M4uProperty<MasterDataDefineLabel.AchievementState>();
    public MasterDataDefineLabel.AchievementState AchievementState
    {
        get
        {
            return achievementState.Value;
        }
        set
        {
            achievementState.Value = value;
        }
    }

    M4uProperty<float> progress = new M4uProperty<float>();
    /// <summary>
    /// ミッション進捗度
    /// </summary>
    public float Progress
    {
        get
        {
            return progress.Value;
        }
        set
        {
            TextColor = ((int)value < 1) ? ColorUtil.COLOR_WHITE : ColorUtil.COLOR_GRAY;
            progress.Value = value;
        }
    }

    M4uProperty<string> progressText = new M4uProperty<string>();
    /// <summary>
    /// ミッション進捗度
    /// </summary>
    public string ProgressText
    {
        get
        {
            return progressText.Value;
        }
        set
        {
            progressText.Value = value;
        }
    }

    M4uProperty<int> missonMaxCount = new M4uProperty<int>();
    /// <summary>
    /// ミッション総数
    /// </summary>
    public int MissonMaxCount
    {
        get
        {
            return missonMaxCount.Value;
        }
        set
        {
            Progress = (value > 0) ? (MissonClearCount.ToFloat() / value.ToFloat()) : 0.0f;
            missonMaxCount.Value = value;
        }
    }

    M4uProperty<int> missonClearCount = new M4uProperty<int>();
    /// <summary>
    /// ミッションクリア数
    /// </summary>
    public int MissonClearCount
    {
        get
        {
            return missonClearCount.Value;
        }
        set
        {
            Progress = (MissonMaxCount > 0) ? (MissonClearCount.ToFloat() / MissonMaxCount.ToFloat()) : 0.0f;
            missonClearCount.Value = value;
        }
    }

    /// <summary>
    /// テキストの色
    /// </summary>
    M4uProperty<Color> textColor = new M4uProperty<Color>(ColorUtil.COLOR_WHITE);
    public Color TextColor
    {
        get
        {
            return textColor.Value;
        }
        set
        {
            textColor.Value = value;
        }
    }


    M4uProperty<string> achievementStateText = new M4uProperty<string>();
    /// <summary>
    /// ミッションステータス
    /// </summary>
    public string AchievementStateText
    {
        get
        {
            return achievementStateText.Value;
        }
        set
        {
            achievementStateText.Value = value;
        }
    }


    M4uProperty<Color> achievementStateColor = new M4uProperty<Color>();
    /// <summary>
    /// ミッションステータスの色
    /// </summary>
    public Color AchievementStateColor
    {
        get
        {
            return achievementStateColor.Value;
        }
        set
        {
            achievementStateColor.Value = value;
        }
    }
}
