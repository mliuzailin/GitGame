/**
 *  @file   MainMenuOthersHelp.cs
 *  @brief
 *  @author Developer
 *  @date   2017/03/06
 */

using UnityEngine;
using System.Collections;

public class MainMenuOthersHelp : MainMenuSeq
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

        m_OthersInfo.LabelText = GameTextUtil.GetText("he169p_title1");

        CreateInfoList();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.HELP;
    }

    void CreateInfoList()
    {
        m_OthersInfo.Infos.Clear();

        // 基本ルール
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle1", TutorialDialog.FLAG_TYPE.HOW_TO_PLAY, null));           // ディバインゲート零とは
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle17", TutorialDialog.FLAG_TYPE.HOME_MENU, null));            // ホーム
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle18", TutorialDialog.FLAG_TYPE.BATTLE1, null));              // バトル1
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle19", TutorialDialog.FLAG_TYPE.BATTLE2, null));              // バトル2
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle20", TutorialDialog.FLAG_TYPE.BATTLE3, null));              // バトル3
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle21", TutorialDialog.FLAG_TYPE.BATTLE4, null));              // バトル4
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle5", TutorialDialog.FLAG_TYPE.UNIT_PARTY_SELECT, null));     // パーティー編成
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle3", TutorialDialog.FLAG_TYPE.UNIT_STATUS, null));           // ユニット
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle15", TutorialDialog.FLAG_TYPE.ELEMENT_DAMAGE, null));       // 属性相性
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle6", TutorialDialog.FLAG_TYPE.UNIT_PARTY_BUILDUP, null));    // 強化合成
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle14", TutorialDialog.FLAG_TYPE.UNIT_LIMIT_OVER, null));      // 限界突破とCHARM
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle7", TutorialDialog.FLAG_TYPE.UNIT_EVOLVE, null));           // ユニット進化
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle8", TutorialDialog.FLAG_TYPE.UNIT_LINK, null));             // リンク機能
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle9", TutorialDialog.FLAG_TYPE.UNIT_SALE, null));             // ユニット売却
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle2", TutorialDialog.FLAG_TYPE.UNIT_HERO, null));             // マスター
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle12", TutorialDialog.FLAG_TYPE.ACHIEVEMENT_GP_LIST, null));  // ミッション
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle11", TutorialDialog.FLAG_TYPE.EVENT_SCHEDULE_LIST, null));  // イベントスケジュール
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle10", TutorialDialog.FLAG_TYPE.GACHA, null));                // スクラッチ
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle13", TutorialDialog.FLAG_TYPE.FRIEND_LIST, null));          // フレンド
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle16", TutorialDialog.FLAG_TYPE.UNIT_CATALOG, null));         // 図鑑
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_button2title50", TutorialDialog.FLAG_TYPE.SCORE, null));               // スコア
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle22", TutorialDialog.FLAG_TYPE.CHALLENGE, null));            // 魔影回廊
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateTutorialDialog("he169p_buttontitle23", TutorialDialog.FLAG_TYPE.AUTO_PLAY, null));            // オートプレイ

        // 基本ルール
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title1", "he170q_content1", null));      // ログインボーナス
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title2", "he170q_content2", null));      // 友情ポイント
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title3", "he170q_content3", null));      // スキルレベルアップ
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title4", "he170q_content4", null));      // スキル確認方法
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title5", "he170q_content5", null));      // お気に入り登録
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title6", "he170q_content6", null));      // リーチライン
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title7", "he170q_content7", null));      // 制限時間
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title8", "he170q_content8", null));      // ユニットのHP
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title9", "he170q_content9", null));      // ターゲット
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title46", "he170q_content46", null));    // ノーマルスキル
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title47", "he170q_content47", null));    // アクティブスキル
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title48", "he170q_content48", null));    // リーダースキル
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title49", "he170q_content49", null));    // マスタースキル
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title10", "he170q_content10", null));    // BOOST
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title11", "he170q_content11", null));    // パッシブスキル
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title12", "he170q_content12", null));    // SP
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title13", "he170q_content13", null));    // ファーストアタック
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title14", "he170q_content14", null));    // バックアタック
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title15", "he170q_content15", null));    // クエストリタイア
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title16", "he170q_content16", null));    // ランクアップ
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title17", "he170q_content17", null));    // あなたのID
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title18", "he170q_content18", null));    // アプリの削除
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title19", "he170q_content19", null));    // 理力界
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title20", "he170q_content20", null));    // 魔力界
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title21", "he170q_content21", null));    // 共時世界
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title22", "he170q_content22", null));    // 蝕
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title23", "he170q_content23", null));    // 相克
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title24", "he170q_content24", null));    // 魔影蝕
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title25", "he170q_content25", null));    // 侵蝕
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title26", "he170q_content26", null));    // 侵蝕者
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title27", "he170q_content27", null));    // エクステ
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title28", "he170q_content28", null));    // エクステM
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title29", "he170q_content29", null));    // リボークモード
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title30", "he170q_content30", null));    // 超越理論
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title31", "he170q_content31", null));    // 大消失
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title32", "he170q_content32", null));    // 幻影界
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title33", "he170q_content33", null));    // 海守学院都市
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title34", "he170q_content34", null));    // 海守学院
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title35", "he170q_content35", null));    // 海守学院音楽研究会
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title36", "he170q_content36", null));    // 覚醒
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title37", "he170q_content37", null));    // 旅団
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title38", "he170q_content38", null));    // 魔震獣
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title39", "he170q_content39", null));    // エネミー
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title40", "he170q_content40", null));    // 魔影蝕発生装置
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title41", "he170q_content41", null));    // 疑似魔影蝕発生装置
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title42", "he170q_content42", null));    // マテリアルマップ
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title43", "he170q_content43", null));    // 招願
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateDialogInfo("he169p_button2title44", "he170q_content44", null));    // 種族（加護）

    }
}
