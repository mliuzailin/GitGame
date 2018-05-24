using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMissionMenu : GlobalMenuSeq
{
    private Mission m_Mission = null;



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
        if (m_Mission == null)
        {
            m_Mission = GetComponentInChildren<Mission>();
            m_Mission.SetPositionAjustStatusBar(new Vector2(0, -6), new Vector2(-20, -346));

            m_Mission.EmptyLabel = GameTextUtil.GetText("common_not_list");
        }

        // ナビゲーションバー
        //        string[] navi_text = {"デイリー","イベント","ノーマル","達成済み"};
        //
        //		m_Mission.NaviText0 = navi_text[0];
        //		m_Mission.NaviText1 = navi_text[1];
        //		m_Mission.NaviText2 = navi_text[2];
        //		m_Mission.NaviText3 = navi_text[3];

        // レコード
        //		int id = 0;
        //        for (int i = 0; i < 200; i++)
        //        {
        //            m_Mission.AddRecord(id++ ,i%3, id + "：title", id*3, new DateTime(2017,2,7), new DateTime(2017,2,28), i%30, 20, 1, i, false, true);
        //        }
        //		
        // シーンの最後に呼び出す
        m_Mission.Initialize();

        // 新規発生アチーブメント情報を削除
        ResidentParam.DelAchievementNewCnt();
    }
}
