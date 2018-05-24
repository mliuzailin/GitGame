/**
 *  @file   MissonGroupListTabSwitcher.cs
 *  @brief  クエストのミッショングループのタブ切り替え
 *  @author Developer
 *  @date   2016/11/14
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using M4u;

public class MissonGroupListTabSwitcher : MonoBehaviour
{
    public ToggleGroup m_ToggleGroup;

    MissonGroupListTabSwitcherContext Context = new MissonGroupListTabSwitcherContext();
    void Awake()
    {
        gameObject.GetComponent<M4uContextRoot>().Context = Context;
    }

    // Use this for initialization
    void Start()
    {
        Context.AchievementCategory = MasterDataDefineLabel.AchievementCategory.BASE;

        Context.DefaultTextColor = ColorUtil.COLOR_PURPLE;
        Context.LimitedTimeTextColor = ColorUtil.COLOR_YELLOW;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// デフォルトのタブを押した時
    /// </summary>
    /// <param name="value"></param>
    public void OnValueChangedDefault(bool value)
    {
        Context.DefaultTextColor = (value) ? ColorUtil.COLOR_PURPLE : ColorUtil.COLOR_YELLOW;
        if (value)
        {
            Context.m_AchievementCategory = MasterDataDefineLabel.AchievementCategory.BASE;
        }
    }

    /// <summary>
    /// 期間限定のタブを押した時
    /// </summary>
    /// <param name="value"></param>
    public void OnValueChangedLimitedTime(bool value)
    {
        Context.LimitedTimeTextColor = (value) ? ColorUtil.COLOR_PURPLE : ColorUtil.COLOR_YELLOW;
        if (value)
        {
            Context.m_AchievementCategory = MasterDataDefineLabel.AchievementCategory.EVENT;
        }
    }

}
