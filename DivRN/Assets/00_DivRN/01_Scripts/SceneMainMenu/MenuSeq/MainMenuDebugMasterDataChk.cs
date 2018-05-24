using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDebugMasterDataChk : MainMenuSeq
{
    DebugMasterDisplayOverlay m_MasterDisplayOverlay;

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

        // TODO: ページ解説テキスト設定

        // TODO: タイトル設定

        m_MasterDisplayOverlay = GetComponentInChildren<DebugMasterDisplayOverlay>();
        m_MasterDisplayOverlay.setup();
    }
}
