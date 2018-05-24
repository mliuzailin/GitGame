using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalTopMenu : GlobalMenuSeq
{
    private int m_UpdateCount = 0;
    public int UpdateCount { get { return m_UpdateCount; } set { m_UpdateCount = value; } }

    protected override void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        if (m_UpdateCount != 0)
        {
            m_UpdateCount--;
            if (m_UpdateCount < 0) m_UpdateCount = 0;
            updateLayout();
        }
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }

    protected override void PageSwitchSetting(bool bActive, bool bBack)
    {
        base.PageSwitchSetting(bActive, bBack);

        //--------------------------------
        // 以下は有効になったタイミングでの処理なので、
        // フェードアウト指示の場合にはスルー
        //--------------------------------
        if (bActive == false) { return; }

        //--------------------------------
        // 戻り処理の場合は再構築スルー
        //--------------------------------
        if (bBack == true) { return; }

        //ページ初期化処理
    }

    /// <summary>
    /// レイアウト更新
    /// </summary>
    public void updateLayout()
    {
        LayoutGroup[] layoutGroups = GetComponentsInChildren<LayoutGroup>();
        for (int i = 0; i < layoutGroups.Length; i++)
        {
            LayoutRebuilder.MarkLayoutForRebuild(layoutGroups[i].transform as RectTransform);
        }
    }
}
