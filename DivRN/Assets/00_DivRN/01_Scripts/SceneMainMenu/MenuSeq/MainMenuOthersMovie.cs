/**
 *  @file   MainMenuOthersMovie.cs
 *  @brief
 *  @author Developer
 *  @date   2017/03/06
 */

using UnityEngine;
using System.Collections;

public class MainMenuOthersMovie : MainMenuSeq
{
    OthersInfo m_OthersInfo;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }

    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        m_OthersInfo = GetComponentInChildren<OthersInfo>();
        m_OthersInfo.SetTopAndBottomAjustStatusBar(new Vector2(-8, -348));
        m_OthersInfo.LabelText = GameTextUtil.GetText("he182p_title");

        m_OthersInfo.Infos.Clear();
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateMovieInfo("he182p_button2", "OpeningMovie.mp4", null));

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.NONE;
    }
}
