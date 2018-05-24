using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class BeginnerBoost : MenuPartsBase
{
    // テキスト
    M4uProperty<string> title = new M4uProperty<string>("");
    M4uProperty<string> content0 = new M4uProperty<string>("");
    M4uProperty<string> button1 = new M4uProperty<string>("");
    public string Title { get { return title.Value; } set { title.Value = value; } }
    public string Content0 { get { return content0.Value; } set { content0.Value = value; } }
    public string Button1 { get { return button1.Value; } set { button1.Value = value; } }

    int m_LastUpdateCount = 0;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    void Start()
    {
        AndroidBackKeyManager.Instance.StackPush(gameObject, OnClickedButton);
    }

    void LateUpdate()
    {
        if (m_LastUpdateCount != 0)
        {
            m_LastUpdateCount--;
            if (m_LastUpdateCount < 0)
            {
                m_LastUpdateCount = 0;
            }
            updateLayout();
        }
    }

    // ボタン
    public void OnClickedButton()
    {
        SoundManager.Instance.PlaySE(SEID.SE_MENU_RET);
        SceneExit();
    }

    // シーン終了
    private void SceneExit()
    {
        AndroidBackKeyManager.Instance.StackPop(gameObject);
        //ホーム画面へ遷移
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, true, false);
    }

    // SceneStart後に実行
    public void PostSceneStart()
    {
        var boost = MasterDataUtil.GetMasterDataBeginnerBoost();
        if (boost == null)
        {
            //ホーム画面へ遷移
            SceneExit();
            return;
        }

        // 初心者ブースト
        Title = GameTextUtil.GetText("pp5q_title");

        // ランク｛0｝を超えるまでは、
        Content0 = string.Format(GameTextUtil.GetText("pp5q0_content"), boost.rank_max);

        // スタミナ消費｛0｝％！
        if (boost.boost_stamina_use != 100)
        {
            Content0 += Environment.NewLine + string.Format(GameTextUtil.GetText("pp5q1_content"), boost.boost_stamina_use);
        }

        // 必要強化費用｛0｝％！
        if (boost.boost_build_money != 100)
        {
            Content0 += Environment.NewLine + string.Format(GameTextUtil.GetText("pp5q3_content"), boost.boost_build_money);
        }

        // 取得経験値｛0｝倍！
        if (boost.boost_exp != 100)
        {
            Content0 += Environment.NewLine + string.Format(GameTextUtil.GetText("pp5q2_content"), (float)boost.boost_exp / 100.0f);
        }

        // コインドロップ｛0｝倍！
        if (boost.boost_money != 100)
        {
            Content0 += Environment.NewLine + string.Format(GameTextUtil.GetText("pp5q4_content"), (float)boost.boost_money / 100.0f);
        }

        // 閉じる
        Button1 = GameTextUtil.GetText("common_button1");


        m_LastUpdateCount = 5;
    }
}
