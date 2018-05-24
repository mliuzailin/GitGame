/**
 *  @file   MissonGroupListTabSwitcherContext.cs
 *  @brief  クエストのミッショングループのタブ切り替えのM4uContext
 *  @author Developer
 *  @date   2016/11/14
 */

using UnityEngine;
using System.Collections;
using M4u;

public class MissonGroupListTabSwitcherContext : M4uContext
{

    public MasterDataDefineLabel.AchievementCategory m_AchievementCategory;
    M4uProperty<MasterDataDefineLabel.AchievementCategory> achievementCategory = new M4uProperty<MasterDataDefineLabel.AchievementCategory>();
    /// <summary>
    /// タブカテゴリ
    /// Toggleでインターフェース上で変えると値が変わらないので、
    /// 値を取得するときはm_AchievementCategoryを使う
    /// </summary>
    public MasterDataDefineLabel.AchievementCategory AchievementCategory
    {
        get
        {
            //if (achievementCategory.Value != m_AchievementCategory) {
            //    return m_AchievementCategory;
            //} else {
            //    return achievementCategory.Value;
            //}

            return achievementCategory.Value;
        }
        set
        {
            m_AchievementCategory = value;
            achievementCategory.Value = value;
        }
    }

    /// <summary>
    /// デフォルトのタブの文字の色
    /// </summary>
    M4uProperty<Color> defaultTextColor = new M4uProperty<Color>();
    public Color DefaultTextColor
    {
        get
        {
            return defaultTextColor.Value;
        }
        set
        {
            defaultTextColor.Value = value;
        }
    }

    /// <summary>
    /// 期間限定のタブの文字の色
    /// </summary>
    M4uProperty<Color> limitedTimeTextColor = new M4uProperty<Color>();
    public Color LimitedTimeTextColor
    {
        get
        {
            return limitedTimeTextColor.Value;
        }
        set
        {
            limitedTimeTextColor.Value = value;
        }
    }

}
