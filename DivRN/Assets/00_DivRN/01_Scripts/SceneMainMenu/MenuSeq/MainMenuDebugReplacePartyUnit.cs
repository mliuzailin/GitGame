/**
 *  @file   MainMenuDebugReplacePartyUnit.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/14
 */

using UnityEngine;
using System.Collections;

public class MainMenuDebugReplacePartyUnit : MainMenuSeq
{

    DebugReplacePartyUnit m_PartyUnit;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

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

        m_PartyUnit = GetComponentInChildren<DebugReplacePartyUnit>();
        if (m_PartyUnit != null)
        {
            m_PartyUnit.CreateList();
        }
    }
}
