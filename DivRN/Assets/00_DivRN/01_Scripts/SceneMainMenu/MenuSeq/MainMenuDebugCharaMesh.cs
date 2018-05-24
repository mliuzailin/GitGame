/**
 *  @file   MainMenuDebugCharaMesh.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/05/02
 */

using UnityEngine;
using System.Collections;

public class MainMenuDebugCharaMesh : MainMenuSeq
{

    DebugCharaMesh m_DebugCharaMesh;

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

        m_DebugCharaMesh = m_CanvasObj.GetComponentInChildren<DebugCharaMesh>();
        m_DebugCharaMesh.Create();
        m_DebugCharaMesh.SetUpParamChara(0);
    }

}
