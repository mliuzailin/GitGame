/**
 *  @file   MainMenuDebugDispEnemy.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/05/02
 */

using UnityEngine;
using System.Collections;

public class MainMenuDebugDispEnemy : MainMenuSeq
{

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
}
