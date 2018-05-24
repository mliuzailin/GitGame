using System.Collections.Generic;

public enum SEID : int
{
    SE_NONE = 0,        //!< SE:なし
    SE_STARGE_START,        //!< SE：システムSE：ジングル：ゲーム開始
    SE_STARGE_CLEAR,        //!< SE：システムSE：ジングル：ゲームクリア
    SE_STAGE_CLEAR_UI,      //!< SE：システムSE：ジングル：ゲームクリアUI
    SE_STARGE_GAMEOVER,     //!< SE：システムSE：ジングル：ゲームオーバー
    SE_BATLE_WINDOW_OPEN,       //!< SE：バトルSE：バトルウィンドウ：開く
    SE_BATLE_WINDOW_CLOSE,      //!< SE：バトルSE：バトルウィンドウ：閉じる
    SE_BATLE_COST_PUT,      //!< SE：バトルSE：コストを追加成立
    SE_BATLE_SKILL_EXEC,        //!< SE：バトルSE：スキルが発動
    SE_BATLE_SKILL_REPLACE,     //!< SE：バトルSE：スキルが置き換えられた
    SE_BATLE_ATTACK_EN_NORMAL,      //!< SE：バトルSE：攻撃音：敵側：デフォルト
    SE_BATLE_ATTACK_PC_NORMAL,      //!< SE：バトルSE：攻撃音：PC側：デフォルト
    SE_BATLE_COUNTDOWN_1,       //!< SE：システムSE：カウントダウン：1
    SE_BATLE_COUNTDOWN_2,       //!< SE：システムSE：カウントダウン：2
    SE_BATLE_COUNTDOWN_3,       //!< SE：システムSE：カウントダウン：3
    SE_BATLE_COUNTDOWN_4,       //!< SE：システムSE：カウントダウン：4
    SE_BATLE_COUNTDOWN_5,       //!< SE：システムSE：カウントダウン：5
    SE_BATLE_COUNTDOWN_6,       //!< SE：システムSE：カウントダウン：6
    SE_BATLE_COUNTDOWN_7,       //!< SE：システムSE：カウントダウン：7
    SE_BATLE_COUNTDOWN_8,       //!< SE：システムSE：カウントダウン：8
    SE_BATLE_COUNTDOWN_9,       //!< SE：システムSE：カウントダウン：9
    SE_BATLE_COST_PLUS_1,       //!< SE：システムSE：コスト吸着
    SE_BATLE_COST_PLUS_2,       //!< SE：システムSE：コスト吸着
    SE_BATLE_COST_IN,       //!< SE：システムSE：コスト配り
    SE_BATLE_SKILL_HANDS,       //!< SE：システムSE：～HANDSの音
    SE_BATLE_SKILL_CUTIN,       //!< SE：システムSE：スキルカットイン
    SE_BATLE_SKILL_CAPTION,     //!< SE：システムSE：スキルキャプション出現音
    SE_BATLE_SKILL_LIMITBREAK_CUTIN,        //!< SE：システムSE：LBSカットイン
    SE_BATLE_SKILL_LIMITBREAK_IMPACT,       //!< SE：システムSE：LBSインパクト
    SE_BATLE_ENEMY_TURN,        //!< SE：システムSE：敵ターン経過
    SE_BATLE_UI_OPEN,           //!< SE：システムSE：戦闘ウィンドウ開く
    SE_INGAME_PANEL_SELECT,     //!< SE：インゲームSE：パネル操作：選択音
    SE_INGAME_PANEL_MEKURI,     //!< SE：インゲームSE：パネル操作：めくり
    SE_INGAME_PANEL_MEKURI_NORMAL,      //!< SE:インゲームSE：パネル操作：めくり経過通常
    SE_INGAME_PANEL_MEKURI_SPECIAL,     //!< SE:インゲームSE：パネル操作：めくり経過特殊
    SE_INGAME_PANEL_SHOCK,      //!< SE：インゲームSE：パネル操作：叩きつけ音
    SE_INGAME_PANEL_MEKURI_S,       //!< SE：インゲームSE：パネル操作：めくり（無地パネル用）
    SE_INGAME_ACTIVITY_KEY,     //!< SE：インゲームSE：パネル発動音：鍵
    SE_INGAME_ACTIVITY_ITEM,        //!< SE：インゲームSE：パネル発動音：宝箱
    SE_INGAME_ACTIVITY_TRAP,        //!< SE：インゲームSE：パネル発動音：虎ばさみ
    SE_INGAME_ACTIVITY_BOMB,        //!< SE：インゲームSE：パネル発動音：地雷
    SE_INGAME_ACTIVITY_PITFALL,     //!< SE：インゲームSE：パネル発動音：落とし穴
    SE_INGAME_ACTIVITY_ENEMY,       //!< SE：インゲームSE：パネル発動音：敵
    SE_INGAME_ACTIVITY_TICKET,      //!< SE：インゲームSE：パネル発動音：チケット
    SE_INGAME_DOOR_OPEN,        //!< SE：インゲームSE：ドア作動音：開く
    SE_INGAME_DOOR_BOSS_TATAKI,     //!< SE：インゲームSE：ドア作動音：ボスドア：叩く
    SE_INGAME_DOOR_BOSS_OPEN,       //!< SE：インゲームSE：ドア作動音：ボスドア：開く
    SE_INGAME_PATH_PLUS,        //!< SE：インゲームSE：パス追加音
    SE_INGAME_QUEST_START_00,       //!< SE:インゲームSE：ReadyTo
    SE_INGAME_QUEST_START_01,       //!< SE:インゲームSE：MoveOn
    SE_INGAME_QUEST_START_02,       //!< SE:インゲームSE:UI退場音
    SE_MENU_OK,     //!< SE：メニューSE：肯定
    SE_MENU_OK2,        //!< SE：メニューSE：肯定（強）
    SE_MENU_NG,     //!< SE：メニューSE：否定
    SE_MENU_RET,        //!< SE：メニューSE：戻る
    SE_MAINMENU_BLEND_SELECT,       //!< SE：メインメニューSE：合成選択
    SE_MAINMENU_BLEND_SELECT_NG,        //!< SE：メインメニューSE：合成選択失敗
    SE_MAINMENU_BLEND_EXEC,     //!< SE：メインメニューSE：合成実行
    SE_MAINMENU_BLEND_CLEAR,        //!< SE：メインメニューSE：合成選択クリア
    SE_BATTLE_ENEMYDEATH,       //!< SE：バトルSE：敵死亡
    SE_BATTLE_SKILL_HEAL,       //!< SE：バトルSE：スキルによるHP回復
    SE_TITLE_CALL_W,        //!< SE：タイトルコール：女
    SE_TITLE_CALL_M,        //!< SE：タイトルコール：男 4.0.0ファイルがない // 番号がずれると問題があるので復活させました
    SE_SHUTTER_OPEN,        //!< SE：シャッター：開く
    SE_SHUTTER_CLOSE,       //!< SE：シャッター：閉じる
    SE_TRAP_CANCEL,     //!< SE：罠解除
    SE_TRAP_LUCK,       //!< SE：よい効果
    SE_TRAP_BAD,        //!< SE：わるい効果
    SE_BATTLE_ATTACK_FIRE,      //!< SE：炎属性攻撃
    SE_BATTLE_ATTACK_WATER,     //!< SE：水属性攻撃
    SE_BATTLE_ATTACK_WIND,      //!< SE：風属性攻撃
    SE_BATTLE_ATTACK_NAUGHT,        //!< SE：無属性攻撃
    SE_BATTLE_ATTACK_LIGHT,     //!< SE：光属性攻撃
    SE_BATTLE_ATTACK_DARK,      //!< SE：闇属性攻撃
    SE_BATTLE_ATTACK_HEAL,      //!< SE：回復属性攻撃
    SE_BATTLE_BOSS_ALERT,       //!< SE：ボスアラート
    SE_BATTLE_BOSS_APPEAR,      //!< SE：ボス登場
    SE_BATTLE_ATTACK_FIRST,     //!< SE：先制攻撃
    SE_BATTLE_ATTACK_BACK,      //!< SE：不意打ち攻撃
    SE_BATTLE_BUFF,     //!< SE：BUFFスキル
    SE_BATTLE_DEBUFF,       //!< SE：DEBUFFスキル
    SE_INGAME_LEADERSKILL,      //!< SE：リーダースキルパワーアップ
    SE_SKILL_COMBO_00,      //!< SE:スキルコンボ：00
    SE_SKILL_COMBO_01,      //!< SE:スキルコンボ：01
    SE_SKILL_COMBO_02,      //!< SE:スキルコンボ：02
    SE_SKILL_COMBO_03,      //!< SE:スキルコンボ：03
    SE_SKILL_COMBO_04,      //!< SE:スキルコンボ：04
    SE_SKILL_COMBO_05,      //!< SE:スキルコンボ：05
    SE_SKILL_COMBO_06,      //!< SE:スキルコンボ：06
    SE_SKILL_COMBO_07,      //!< SE:スキルコンボ：07
    SE_SKILL_COMBO_08,      //!< SE:スキルコンボ：08
    SE_SKILL_COMBO_MORE_THAN_08,      //!< SE:スキルコンボ：09
    SE_SKILL_COMBO_FINISH_WORD,     //!< SE：スキルコンボ：フィニッシュ
    SE_CHESS_FALL,      //!< SE:チェス駒：落下音
    SE_CHESS_MOVE,      //!< SE:チェス駒：移動音
    SE_DOOR_OPEN_NORMAL,        //!< SE:チェス駒：ドア開き：ノーマル
    SE_DOOR_OPEN_BOSS,      //!< SE:チェス駒：ドア開き：ボス
    SE_SPLIMITOVER,     //!< SE:SP切れUI

    SE_MM_A01_CHECK,        //!< SE:全体リソース：決定音
    SE_MM_A02_CHECK2,       //!< SE:全体リソース：決定音（大）
    SE_MM_A03_TAB,      //!< SE:全体リソース：タブ切り替え
    SE_MM_A04_BACK,     //!< SE:全体リソース：キャンセル(バック)音
    SE_MM_B01_EXP_GAUGE,        //!< SE:クエストリザルト：ゲージが伸びる音
    SE_MM_B02_RANKUP,       //!< SE:クエストリザルト：ランクアップ
    SE_MM_B04_RARE_START,       //!< SE:クエストリザルト：ユニットゲットレア度のUI
    SE_MM_B05_RARE_STAR_PUT,        //!< SE:クエストリザルト：星のはまる音
    SE_MM_B06_RARE_END,     //!< SE:クエストリザルト：レア度UIの最後に
    SE_MM_C01_SCRATCH_1_3,      //!< SE:スクラッチ：☆１～３ゲット
    SE_MM_C02_SCRATCH_4,        //!< SE:スクラッチ：☆４ゲット
    SE_MM_C03_SCRATCH_5_6,      //!< SE:スクラッチ：☆５～６ゲット
    SE_MM_C04_SCRATCH_RARE,     //!< SE:スクラッチ：レアめくり
    SE_MM_D01_FRIEND_UNIT,      //!< SE:強化合成：フレンドユニット
    SE_MM_D02_MATERIAL_UNIT,        //!< SE:強化合成：マテリアルユニット
    SE_MM_D04_LEVEL_UP,     //!< SE:強化合成：レベルアップ
    SE_MM_D09_EVOLVE_ROLL,      //!< SE:進化合成：演出回転
    SE_MM_D10_EVOLVE_COMP,      //!< SE:進化合成：進化後遷移
    SE_MM_D08_SALE,     //!< SE:売却：売却演出音

    VOICE_INGAME_QUEST_READYTO,     //!< Voice:A:ReadyTo
    VOICE_INGAME_QUEST_MOVEON,      //!< Voice:A:MoveOn
    VOICE_INGAME_QUEST_BOSSAPPEAR,      //!< Voice:A:BossAppear
    VOICE_INGAME_QUEST_QUESTCLEAR,      //!< Voice:A:QuestClear
    VOICE_INGAME_QUEST_GAMEOVER,        //!< Voice:A:GameOver
    VOICE_INGAME_QUEST_GETKEY,      //!< Voice:A:GETKEY
    VOICE_INGAME_QUEST_NICE,        //!< Voice:A:戦闘評価：NICE
    VOICE_INGAME_QUEST_GREAT,       //!< Voice:A:戦闘評価：GREAT
    VOICE_INGAME_QUEST_BEAUTY,      //!< Voice:A:戦闘評価：BEAUTY
    VOICE_INGAME_QUEST_EXCELLENT,       //!< Voice:A:戦闘評価：EXCELLENT
    VOICE_INGAME_QUEST_COOL,        //!< Voice:A:戦闘評価：COOL
    VOICE_INGAME_QUEST_UNBELIEVABLE,        //!< Voice:A:戦闘評価：UNBELIEVABLE
    VOICE_INGAME_QUEST_MARVELOUS,       //!< Voice:A:戦闘評価：MARVELOUS
    VOICE_INGAME_QUEST_DIVINE,      //!< Voice:A:戦闘評価：DIVINE
    VOICE_INGAME_QUEST_FIRSTATTACK,     //!< Voice:A：FIRSTATTACK
    VOICE_INGAME_QUEST_BACKATTACK,      //!< Voice:A：BACKATTACK
    VOICE_INGAME_QUEST_HANDCARD_SET,        //!< Voice:A:手札配り
    VOICE_INGAME_QUEST_STANDREADY,      //!< Voice:A:LBS発動
    VOICE_INGAME_QUEST_SPLIMIT,     //!< Voice:A:SPLimit

    VOICE_INGAME_MM_EVOLVE,     //!< Voice:A:進化
    VOICE_INGAME_MM_FOOT_FRIEND,        //!< Voice:A:フッタ：フレンド
    VOICE_INGAME_MM_FOOT_OTHERS,        //!< Voice:A:フッタ：その他
    VOICE_INGAME_MM_FOOT_QUEST,     //!< Voice:A:フッタ：クエスト
    VOICE_INGAME_MM_FOOT_SCRATCH,       //!< Voice:A:フッタ：スクラッチ
    VOICE_INGAME_MM_FOOT_SHOP,      //!< Voice:A:フッタ：ショップ
    VOICE_INGAME_MM_FOOT_UNIT,      //!< Voice:A:フッタ：ユニット
    VOICE_INGAME_MM_LEVELUP,        //!< Voice:A:レベルアップ
    VOICE_INGAME_MM_RANKUP,     //!< Voice:A:ランクアップ
    VOICE_INGAME_MM_SKILLUP,        //!< Voice:A:スキルアップ
    VOICE_INGAME_MM_UNIT_GET,       //!< Voice:A:ユニット取得：
    VOICE_INGAME_MM_UNIT_GET_1,     //!< Voice:A:ユニット取得：レア１
    VOICE_INGAME_MM_UNIT_GET_2,     //!< Voice:A:ユニット取得：レア２
    VOICE_INGAME_MM_UNIT_GET_3,     //!< Voice:A:ユニット取得：レア３
    VOICE_INGAME_MM_UNIT_GET_4,     //!< Voice:A:ユニット取得：レア４
    VOICE_INGAME_MM_UNIT_GET_5,     //!< Voice:A:ユニット取得：レア５
    VOICE_INGAME_MM_UNIT_GET_6,     //!< Voice:A:ユニット取得：レア６
    VOICE_INGAME_MM_UNIT_GET_7,     //!< Voice:A:ユニット取得：レア７

    VOICE_INGAME_MM_LINK_ON,        //!< Voice:A:リンクオン
    VOICE_INGAME_MM_HOME,       //!< Voice:A:ホーム
    VOICE_INGAME_MM_LIMITOVER,      //!< Voice:A:リミットオーバー

    SE_MAX,		//!< SE：総数
}

public static class SESettings
{
    public static Dictionary<SEID, string> pathMap = new Dictionary<SEID, string>
    {
        {SEID.SE_STARGE_START , GlobalDefine.DATAPATH_SOUND_SE + "seb10_start"}, // SE：システムSE：ジングル：ゲーム開始
        {SEID.SE_STARGE_CLEAR , GlobalDefine.DATAPATH_SOUND_SE + "E17_quest_clear"},	// SE：システムSE：ジングル：ゲームクリア
        {SEID.SE_STAGE_CLEAR_UI , GlobalDefine.DATAPATH_SOUND_SE + "E18_quest_clear"},	// SE：システムSE：ジングル：ゲームクリアUI
        {SEID.SE_STARGE_GAMEOVER , GlobalDefine.DATAPATH_SOUND_SE + "F43_gameover"},	// SE：システムSE：ジングル：ゲームオーバー
        {SEID.SE_BATLE_WINDOW_OPEN , GlobalDefine.DATAPATH_SOUND_SE + "F01_enemy_window"},	// SE：バトルSE：バトルウィンドウ：開く
        {SEID.SE_BATLE_WINDOW_CLOSE , GlobalDefine.DATAPATH_SOUND_SE + "seb07_windowclose"},	// SE：バトルSE：バトルウィンドウ：閉じる
        {SEID.SE_BATLE_COST_PUT , GlobalDefine.DATAPATH_SOUND_SE + "seb02_ecardput"},	// SE：バトルSE：コストを追加成立
        {SEID.SE_BATLE_SKILL_EXEC , GlobalDefine.DATAPATH_SOUND_SE + "hatudou"},	// SE：バトルSE：スキルが発動
        {SEID.SE_BATLE_SKILL_REPLACE , GlobalDefine.DATAPATH_SOUND_SE + "seiritu"},
        {SEID.SE_BATLE_ATTACK_EN_NORMAL , GlobalDefine.DATAPATH_SOUND_SE + "seb04_en_attack"},	// SE：バトルSE：攻撃音：敵側：デフォルト
        {SEID.SE_BATLE_COUNTDOWN_1 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_count_1"},	// SE：システムSE：カウントダウン：1
        {SEID.SE_BATLE_COUNTDOWN_2 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_count_2"},	// SE：システムSE：カウントダウン：2
        {SEID.SE_BATLE_COUNTDOWN_3 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_count_3"},	// SE：システムSE：カウントダウン：3
        {SEID.SE_BATLE_COUNTDOWN_4 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_count_4"},	// SE：システムSE：カウントダウン：4
        {SEID.SE_BATLE_COUNTDOWN_5 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_count_5"},	// SE：システムSE：カウントダウン：5
        {SEID.SE_BATLE_COUNTDOWN_6 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_count_6"},	// SE：システムSE：カウントダウン：6
        {SEID.SE_BATLE_COUNTDOWN_7 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_count_7"},	// SE：システムSE：カウントダウン：7
        {SEID.SE_BATLE_COUNTDOWN_8 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_count_8"},	// SE：システムSE：カウントダウン：8
        {SEID.SE_BATLE_COUNTDOWN_9 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_count_9"},	// SE：システムSE：カウントダウン：9
        {SEID.SE_BATLE_COST_PLUS_1 , GlobalDefine.DATAPATH_SOUND_SE + "kyutyaku01"},	// SE：システムSE：コスト吸着
        {SEID.SE_BATLE_COST_PLUS_2 , GlobalDefine.DATAPATH_SOUND_SE + "E06_nazoru"},	// SE：システムSE：コスト吸着
        {SEID.SE_BATLE_COST_IN , GlobalDefine.DATAPATH_SOUND_SE + "F07_tefuda"},	// SE：インゲームSE：パネル操作：手札配り
        {SEID.SE_BATLE_SKILL_HANDS , GlobalDefine.DATAPATH_SOUND_SE + "F55_hands"},	// SE：インゲームSE：～handsの音
        {SEID.SE_BATLE_SKILL_CUTIN , GlobalDefine.DATAPATH_SOUND_SE + "seb08_doornock"},
        {SEID.SE_BATLE_SKILL_CAPTION , GlobalDefine.DATAPATH_SOUND_SE + "F09_skill"},	// SE:インゲームSE:
        {SEID.SE_BATLE_SKILL_LIMITBREAK_CUTIN , GlobalDefine.DATAPATH_SOUND_SE + "H02_power_up"},	// SE:インゲームSE:LBSカットイン：カットイン
        {SEID.SE_BATLE_SKILL_LIMITBREAK_IMPACT , GlobalDefine.DATAPATH_SOUND_SE + "F57_active_impact"},	// SE:インゲームSE:LBSカットイン：バン
        {SEID.SE_BATLE_ENEMY_TURN , GlobalDefine.DATAPATH_SOUND_SE + "F58_enemy_next"},	// SE:インゲームSE:敵ターン経過
        {SEID.SE_BATLE_UI_OPEN , GlobalDefine.DATAPATH_SOUND_SE + "Z01_shutter_open"},
        {SEID.SE_INGAME_PANEL_MEKURI , GlobalDefine.DATAPATH_SOUND_SE + "E07_mekure"},	// SE：インゲームSE：パネル操作：めくり
        {SEID.SE_INGAME_PANEL_MEKURI_NORMAL , GlobalDefine.DATAPATH_SOUND_SE + "E07a_mekure_keika_normal"},	// SE：インゲームSE：パネル操作：めくり経過通常
        {SEID.SE_INGAME_PANEL_MEKURI_SPECIAL , GlobalDefine.DATAPATH_SOUND_SE + "E07b_mekure_keika_normalspecial"},	// SE：インゲームSE：パネル操作：めくり経過特殊
        {SEID.SE_INGAME_PANEL_SHOCK , GlobalDefine.DATAPATH_SOUND_SE + "E08_bashin"},	// SE：インゲームSE：パネル操作：叩きつけ音
        {SEID.SE_INGAME_ACTIVITY_ITEM , GlobalDefine.DATAPATH_SOUND_SE + "se08_takarabako"}, // SE：インゲームSE：パネル発動音：宝箱
        {SEID.SE_INGAME_DOOR_OPEN , GlobalDefine.DATAPATH_SOUND_SE + "se07_opendoor"},	// SE：インゲームSE：ドア作動音：開く
        {SEID.SE_INGAME_DOOR_BOSS_TATAKI , GlobalDefine.DATAPATH_SOUND_SE + "seb08_doornock"},	// SE：インゲームSE：ドア作動音：ボスドア：叩く
        {SEID.SE_INGAME_DOOR_BOSS_OPEN , GlobalDefine.DATAPATH_SOUND_SE + "seb09_dooropen"},	// SE：インゲームSE：ドア作動音：ボスドア：開く
        {SEID.SE_INGAME_QUEST_START_00 , GlobalDefine.DATAPATH_SOUND_SE + "E02a_ready_to"},	// SE：インゲームSE：クエスト開始UI音
        {SEID.SE_INGAME_QUEST_START_02 , GlobalDefine.DATAPATH_SOUND_SE + "E05_ui_kieru"},	// SE：インゲームSE：クエスト開始UI退場音

        {SEID.SE_MENU_OK , GlobalDefine.DATAPATH_SOUND_SE + "A01_check"},	// SE：メニューSE：肯定
        {SEID.SE_MENU_OK2 , GlobalDefine.DATAPATH_SOUND_SE + "A02_check2"},	// SE：メニューSE：肯定
        {SEID.SE_MENU_NG , GlobalDefine.DATAPATH_SOUND_SE + "seb04_en_attack"},	// SE：メニューSE：否定
        {SEID.SE_MENU_RET , GlobalDefine.DATAPATH_SOUND_SE + "A04_back"},	// SE：メニューSE：戻る
        {SEID.SE_MAINMENU_BLEND_EXEC , GlobalDefine.DATAPATH_SOUND_SE + "A02_check2"},	// SE：メインメニューSE：合成実行
        {SEID.SE_BATTLE_ENEMYDEATH , GlobalDefine.DATAPATH_SOUND_SE + "F41_enemy_kieru"},	// SE：バトルSE：敵死亡
        {SEID.SE_BATTLE_SKILL_HEAL , GlobalDefine.DATAPATH_SOUND_SE + "F33_kaifuk"},	// SE：バトルSE：スキルによるHP回復
        {SEID.SE_TITLE_CALL_W , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_divinegate_zero"},	// SE：タイトルコール：女
        //{SEID.SE_TITLE_CALL_M , GlobalDefine.DATAPATH_SOUND_SE + "se_title_m_d"},	// SE：タイトルコール：男
        {SEID.SE_TRAP_LUCK , GlobalDefine.DATAPATH_SOUND_SE + "G13_trap_luck"},	// SE：良い罠
        {SEID.SE_TRAP_BAD , GlobalDefine.DATAPATH_SOUND_SE + "G05_trap_bad"},	// SE：悪い罠
        {SEID.SE_BATTLE_ATTACK_FIRE , GlobalDefine.DATAPATH_SOUND_SE + "F34_attack_fire"},	// SE：属性攻撃音：炎
        {SEID.SE_BATTLE_ATTACK_WATER , GlobalDefine.DATAPATH_SOUND_SE + "F35_attack_aqua"},	// SE：属性攻撃音：水
        {SEID.SE_BATTLE_ATTACK_WIND , GlobalDefine.DATAPATH_SOUND_SE + "F36_attack_wind"},	// SE：属性攻撃音：風
        {SEID.SE_BATTLE_ATTACK_LIGHT , GlobalDefine.DATAPATH_SOUND_SE + "F37_attack_light"},	// SE：属性攻撃音：光
        {SEID.SE_BATTLE_ATTACK_DARK , GlobalDefine.DATAPATH_SOUND_SE + "F38_attack_dark"},	// SE：属性攻撃音：闇
        {SEID.SE_BATTLE_ATTACK_NAUGHT , GlobalDefine.DATAPATH_SOUND_SE + "F39_attack_none"},	// SE：属性攻撃音：無
        {SEID.SE_BATTLE_BOSS_ALERT , GlobalDefine.DATAPATH_SOUND_SE + "E14_boss_window"},	// SE：ボスウィンドウ
        {SEID.SE_BATTLE_BOSS_APPEAR , GlobalDefine.DATAPATH_SOUND_SE + "E15_boss_apper"},	// SE：ボス登場
        {SEID.SE_BATTLE_ATTACK_FIRST , GlobalDefine.DATAPATH_SOUND_SE + "F04_firstattack"},	// SE：先制攻撃
        {SEID.SE_BATTLE_ATTACK_BACK , GlobalDefine.DATAPATH_SOUND_SE + "F02_backattack"},	// SE：不意打ち攻撃
        {SEID.SE_BATTLE_BUFF , GlobalDefine.DATAPATH_SOUND_SE + "H08_buff_skill"},	// SE：BUFFスキル
        {SEID.SE_BATTLE_DEBUFF , GlobalDefine.DATAPATH_SOUND_SE + "H05_debuff_skill"},	// SE：DEBUFFスキル
        {SEID.SE_INGAME_LEADERSKILL , GlobalDefine.DATAPATH_SOUND_SE + "H02_power_up"},	// SE：リーダースキルパワーアップ
        {SEID.SE_SKILL_COMBO_00 , GlobalDefine.DATAPATH_SOUND_SE + "F22_skill_01"},	// SE：スキルコンボ：00
        {SEID.SE_SKILL_COMBO_01 , GlobalDefine.DATAPATH_SOUND_SE + "F23_skill_02"},	// SE：スキルコンボ：01
        {SEID.SE_SKILL_COMBO_02 , GlobalDefine.DATAPATH_SOUND_SE + "F24_skill_03"},	// SE：スキルコンボ：02
        {SEID.SE_SKILL_COMBO_03 , GlobalDefine.DATAPATH_SOUND_SE + "F25_skill_04"},	// SE：スキルコンボ：03
        {SEID.SE_SKILL_COMBO_04 , GlobalDefine.DATAPATH_SOUND_SE + "F26_skill_05"},	// SE：スキルコンボ：04
        {SEID.SE_SKILL_COMBO_05 , GlobalDefine.DATAPATH_SOUND_SE + "F27_skill_06"},	// SE：スキルコンボ：05
        {SEID.SE_SKILL_COMBO_06 , GlobalDefine.DATAPATH_SOUND_SE + "F28_skill_07"},	// SE：スキルコンボ：06
        {SEID.SE_SKILL_COMBO_07 , GlobalDefine.DATAPATH_SOUND_SE + "F29_skill_08"},	// SE：スキルコンボ：07
        {SEID.SE_SKILL_COMBO_08 , GlobalDefine.DATAPATH_SOUND_SE + "F30_skill_09"},	// SE：スキルコンボ：08
        {SEID.SE_SKILL_COMBO_MORE_THAN_08 , GlobalDefine.DATAPATH_SOUND_SE + "F30_skill_09"},	// SE：スキルコンボ：09以上
        {SEID.SE_SKILL_COMBO_FINISH_WORD , GlobalDefine.DATAPATH_SOUND_SE + "F59_finish_word"},	// SE：スキルコンボ：フィニッシュ
        {SEID.SE_CHESS_FALL , GlobalDefine.DATAPATH_SOUND_SE + "X07_chessfall"},	// SE：チェス駒：落下音
        {SEID.SE_CHESS_MOVE , GlobalDefine.DATAPATH_SOUND_SE + "X08_chesswalk"},	// SE：チェス駒：移動音
        {SEID.SE_DOOR_OPEN_NORMAL , GlobalDefine.DATAPATH_SOUND_SE + "I01_door_normal"},	// SE：ドア開き：通常
        {SEID.SE_DOOR_OPEN_BOSS , GlobalDefine.DATAPATH_SOUND_SE + "I02_door_boss"},	// SE：ドア開き：ボス
        {SEID.SE_SPLIMITOVER , GlobalDefine.DATAPATH_SOUND_SE + "E21_SPlimit"},	// SE：SPLimitOver

        {SEID.SE_MM_A03_TAB , GlobalDefine.DATAPATH_SOUND_SE + "A03_tab_change"},	// SE:全体リソース：タブ切り替え
        {SEID.SE_MM_B01_EXP_GAUGE , GlobalDefine.DATAPATH_SOUND_SE + "B01_exp_gage"},	// SE:クエストリザルト：ゲージが伸びる音
        {SEID.SE_MM_B02_RANKUP , GlobalDefine.DATAPATH_SOUND_SE + "B02_rank_lv_up"},	// SE:クエストリザルト：ランクアップ
        {SEID.SE_MM_B04_RARE_START , GlobalDefine.DATAPATH_SOUND_SE + "B04_rera_ui_start"},	// SE:クエストリザルト：ユニットゲットレア度のUI
        {SEID.SE_MM_C01_SCRATCH_1_3 , GlobalDefine.DATAPATH_SOUND_SE + "C01_rera_1_3_v2"},	// SE:スクラッチ：☆１～３ゲット
        {SEID.SE_MM_C02_SCRATCH_4 , GlobalDefine.DATAPATH_SOUND_SE + "C02_rera_4_v2"},	// SE:スクラッチ：☆４ゲット
        {SEID.SE_MM_C03_SCRATCH_5_6 , GlobalDefine.DATAPATH_SOUND_SE + "C03_rera_5_6_v2"},	// SE:スクラッチ：☆５～６ゲット
        {SEID.SE_MM_C04_SCRATCH_RARE , GlobalDefine.DATAPATH_SOUND_SE + "C04_rera_5_6_mekure"},	// SE:スクラッチ：レアめくり
        {SEID.SE_MM_D01_FRIEND_UNIT , GlobalDefine.DATAPATH_SOUND_SE + "D01_friend_unit"},	// SE:強化合成：フレンドユニット
        {SEID.SE_MM_D02_MATERIAL_UNIT , GlobalDefine.DATAPATH_SOUND_SE + "D02_material_unit"},	// SE:強化合成：マテリアルユニット
        {SEID.SE_MM_D04_LEVEL_UP , GlobalDefine.DATAPATH_SOUND_SE + "D04_levelup"},	// SE:強化合成：レベルアップ
        {SEID.SE_MM_D09_EVOLVE_ROLL , GlobalDefine.DATAPATH_SOUND_SE + "D09_evolve_roll"},	// SE:進化合成：演出回転
        {SEID.SE_MM_D10_EVOLVE_COMP , GlobalDefine.DATAPATH_SOUND_SE + "D10_evolve_comp"},	// SE:進化合成：進化後遷移
        {SEID.SE_MM_D08_SALE , GlobalDefine.DATAPATH_SOUND_SE + "D08_unit_sell"},	// SE:売却：売却演出音

        {SEID.VOICE_INGAME_QUEST_READYTO , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_foot_queststart"},	// VOICE:インゲーム:クエスト:ReadyTo
        {SEID.VOICE_INGAME_QUEST_MOVEON , GlobalDefine.DATAPATH_SOUND_SE + "a_07"},	// VOICE:インゲーム:クエスト:MoveOn
        {SEID.VOICE_INGAME_QUEST_BOSSAPPEAR , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_bossbattle"},	// VOICE:インゲーム：クエスト:BossAppear
        {SEID.VOICE_INGAME_QUEST_QUESTCLEAR , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_foot_questclear"},	// VOICE:インゲーム：クエスト:GameClear
        {SEID.VOICE_INGAME_QUEST_GAMEOVER , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_foot_gameover"},	// VOICE:インゲーム：クエスト:GameOver
        {SEID.VOICE_INGAME_QUEST_GETKEY , GlobalDefine.DATAPATH_SOUND_SE + "a_11"},	// VOICE:インゲーム：クエスト:GetKey
        {SEID.VOICE_INGAME_QUEST_NICE , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_nice"},	// VOICE:インゲーム：クエスト:戦闘評価:NICE
        {SEID.VOICE_INGAME_QUEST_GREAT , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_great"},	// VOICE:インゲーム：クエスト:戦闘評価:GREA
        {SEID.VOICE_INGAME_QUEST_BEAUTY , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_beauty"},	// VOICE:インゲーム：クエスト:戦闘評価:BEAUTY
        {SEID.VOICE_INGAME_QUEST_EXCELLENT , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_excellent"},	// VOICE:インゲーム：クエスト:戦闘評価:EXCELLENT
        {SEID.VOICE_INGAME_QUEST_COOL , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_cool"},	// VOICE:インゲーム：クエスト:戦闘評価:NICE
        {SEID.VOICE_INGAME_QUEST_UNBELIEVABLE , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_unbelievable"},	// VOICE:インゲーム：クエスト:戦闘評価:GREAT
        {SEID.VOICE_INGAME_QUEST_MARVELOUS , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_marvelous"},	// VOICE:インゲーム：クエスト:戦闘評価:BEAUTY
        {SEID.VOICE_INGAME_QUEST_DIVINE , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_divine"},	// VOICE:インゲーム：クエスト:戦闘評価:EXCELLENT
        {SEID.VOICE_INGAME_QUEST_FIRSTATTACK , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_firstattack"},	// VOICE:インゲーム：クエスト:先制攻撃
        {SEID.VOICE_INGAME_QUEST_BACKATTACK , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_backattack"},	// VOICE:インゲーム：クエスト:不意打ち
        {SEID.VOICE_INGAME_QUEST_HANDCARD_SET , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_handcard_set"},	// VOICE:インゲーム：クエスト：手札配り
        {SEID.VOICE_INGAME_QUEST_STANDREADY , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_battle_standready"},	// VOICE:インゲーム：クエスト：LBS
        {SEID.VOICE_INGAME_QUEST_SPLIMIT , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_splimit"},	// VOICE:インゲーム：クエスト：SPLimit

        {SEID.VOICE_INGAME_MM_EVOLVE , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_evolve"},	// Voice:A:進化
        {SEID.VOICE_INGAME_MM_FOOT_QUEST , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_foot_quest"},	// Voice:A:フッタ：クエスト
        {SEID.VOICE_INGAME_MM_FOOT_SCRATCH , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_foot_scratch"},	// Voice:A:フッタ：スクラッチ
        {SEID.VOICE_INGAME_MM_FOOT_SHOP , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_foot_shop"},	// Voice:A:フッタ：ショップ
        {SEID.VOICE_INGAME_MM_FOOT_UNIT , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_foot_unit"},	// Voice:A:フッタ：ユニット
        {SEID.VOICE_INGAME_MM_LEVELUP , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_levelup"},	// Voice:A:レベルアップ
        {SEID.VOICE_INGAME_MM_RANKUP , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_rank_up"},	// Voice:A:ランクアップ
        {SEID.VOICE_INGAME_MM_SKILLUP , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_skillup"},	// Voice:A:スキルアップ
        {SEID.VOICE_INGAME_MM_UNIT_GET , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_unitget"},	// Voice:A:ユニット取得：
        {SEID.VOICE_INGAME_MM_LINK_ON , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_link_on"},	// Voice:A:リンクオン
        {SEID.VOICE_INGAME_MM_HOME , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_home"},	// Voice:A:ホーム
        {SEID.VOICE_INGAME_MM_LIMITOVER , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_limitover"},	// Voice:A:リミットオーバー

        // 以下、2017/5/26時点で使われていないSE→このまま使わないなら音ファイルごと削除
        {SEID.SE_BATLE_ATTACK_PC_NORMAL , GlobalDefine.DATAPATH_SOUND_SE + "seb05_pc_attack"},	// SE：バトルSE：攻撃音：PC側：デフォルト
        {SEID.SE_INGAME_PANEL_SELECT , GlobalDefine.DATAPATH_SOUND_SE + "se02_panelselect"},	// SE：インゲームSE：パネル操作：選択音
        {SEID.SE_INGAME_ACTIVITY_KEY , GlobalDefine.DATAPATH_SOUND_SE + "se06_keyget"},	// SE：インゲームSE：パネル発動音：鍵
        {SEID.SE_INGAME_ACTIVITY_PITFALL , GlobalDefine.DATAPATH_SOUND_SE + "se11_pitfall"},	// SE：インゲームSE：パネル発動音：落とし穴
        {SEID.SE_INGAME_ACTIVITY_ENEMY , GlobalDefine.DATAPATH_SOUND_SE + "E11_enemy_panel"},	// SE：インゲームSE：パネル発動音：敵
        {SEID.SE_INGAME_ACTIVITY_TICKET , GlobalDefine.DATAPATH_SOUND_SE + "E22_ticket_casino"},	// SE：インゲームSE：パネル発動音：チケット
        {SEID.SE_INGAME_PATH_PLUS , GlobalDefine.DATAPATH_SOUND_SE + "kyutyaku02"},	// SE：インゲームSE：ドア作動音：ボスドア：開く
        {SEID.SE_INGAME_QUEST_START_01 , GlobalDefine.DATAPATH_SOUND_SE + "E02b_move_on"},	// SE：インゲームSE：クエスト開始UI音
        {SEID.SE_SHUTTER_CLOSE , GlobalDefine.DATAPATH_SOUND_SE + "Z02_shutter_close"},	// SE：シャッター：閉じる
        {SEID.SE_TRAP_CANCEL , GlobalDefine.DATAPATH_SOUND_SE + "G03_trap_cancel"},	// SE：罠解除
        {SEID.SE_MM_B05_RARE_STAR_PUT , GlobalDefine.DATAPATH_SOUND_SE + "B05_rera_ui_star"},	// SE:クエストリザルト：星のはまる音
        {SEID.SE_MM_B06_RARE_END , GlobalDefine.DATAPATH_SOUND_SE + "B06_rera_ui_end"},	// SE:クエストリザルト：レア度UIの最後に
        {SEID.VOICE_INGAME_MM_FOOT_FRIEND , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_foot_friend"},	// Voice:A:フッタ：フレンド
        {SEID.VOICE_INGAME_MM_FOOT_OTHERS , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_foot_others"},	// Voice:A:フッタ：その他
        {SEID.VOICE_INGAME_MM_UNIT_GET_1 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_unitget_1"},	// Voice:A:ユニット取得：レア１
        {SEID.VOICE_INGAME_MM_UNIT_GET_2 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_unitget_2"},	// Voice:A:ユニット取得：レア２
        {SEID.VOICE_INGAME_MM_UNIT_GET_3 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_unitget_3"},	// Voice:A:ユニット取得：レア３
        {SEID.VOICE_INGAME_MM_UNIT_GET_4 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_unitget_4"},	// Voice:A:ユニット取得：レア４
        {SEID.VOICE_INGAME_MM_UNIT_GET_5 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_unitget_5"},	// Voice:A:ユニット取得：レア５
        {SEID.VOICE_INGAME_MM_UNIT_GET_6 , GlobalDefine.DATAPATH_SOUND_SE + "voice_a_mm_unitget_6"},	// Voice:A:ユニット取得：レア６
   };

    // --------------- 各シーンごとの使用するSE
    // TODO : もうちょっといい感じにする

    public static List<SEID> SceneSplashSEs = new List<SEID>
    {
        SEID.SE_MENU_OK,
        SEID.SE_MENU_OK2,
        SEID.SE_MENU_NG,
        SEID.SE_MENU_RET,
    };

    public static List<SEID> SceneTitleSEs = new List<SEID>
    {
        SEID.SE_MENU_OK,
        SEID.SE_MENU_OK2,
        SEID.SE_MENU_NG,
        SEID.SE_MENU_RET,
        SEID.SE_TITLE_CALL_W,
        //SEID.SE_TITLE_CALL_M,
    };

    public static List<SEID> SceneMainMenuSEs = new List<SEID>
    {
        SEID.SE_MENU_RET,
        SEID.SE_MENU_OK,
        SEID.SE_MENU_OK2,
        SEID.SE_MENU_NG,
        SEID.SE_MAINMENU_BLEND_EXEC,
        SEID.SE_MM_A03_TAB, // 旧オープニングで使ってたらしい→必要ないなら削除
        SEID.SE_MM_B01_EXP_GAUGE,
        SEID.SE_MM_B02_RANKUP,
        SEID.SE_MM_B04_RARE_START,
        SEID.SE_MM_C01_SCRATCH_1_3,
        SEID.SE_MM_C02_SCRATCH_4,
        SEID.SE_MM_C03_SCRATCH_5_6,
        SEID.SE_MM_C04_SCRATCH_RARE,
        SEID.SE_MM_D01_FRIEND_UNIT,
        SEID.SE_MM_D02_MATERIAL_UNIT,
        SEID.SE_MM_D04_LEVEL_UP,
        SEID.SE_MM_D09_EVOLVE_ROLL,
        SEID.SE_MM_D10_EVOLVE_COMP,
        SEID.SE_MM_D08_SALE,

        // INGAMEってついてるけどガチャで使ってる
        SEID.SE_INGAME_PANEL_MEKURI,
        SEID.SE_INGAME_PANEL_MEKURI_NORMAL,
        SEID.SE_INGAME_PANEL_MEKURI_SPECIAL,

        // ログインボーナスで使ってる
        SEID.SE_INGAME_QUEST_START_00,

        SEID.VOICE_INGAME_MM_EVOLVE,
        SEID.VOICE_INGAME_MM_FOOT_QUEST,
        SEID.VOICE_INGAME_MM_FOOT_SCRATCH,
        SEID.VOICE_INGAME_MM_FOOT_SHOP,
        SEID.VOICE_INGAME_MM_FOOT_UNIT,
        SEID.VOICE_INGAME_MM_LEVELUP,
        SEID.VOICE_INGAME_MM_RANKUP,
        SEID.VOICE_INGAME_MM_SKILLUP,
        SEID.VOICE_INGAME_MM_UNIT_GET,
        SEID.VOICE_INGAME_MM_LINK_ON,
        SEID.VOICE_INGAME_MM_HOME,
        SEID.VOICE_INGAME_MM_LIMITOVER,
    };

    public static List<SEID> SceneQuest2SEs = new List<SEID>
    {
        SEID.SE_MENU_OK,
        SEID.SE_MENU_OK2,
        SEID.SE_MENU_RET,
        SEID.SE_MM_A03_TAB,
        SEID.SE_MM_D10_EVOLVE_COMP,
        SEID.VOICE_INGAME_MM_EVOLVE,
        SEID.SE_STARGE_CLEAR,
        SEID.SE_STARGE_GAMEOVER,
        SEID.SE_BATTLE_ENEMYDEATH,
        SEID.SE_BATTLE_SKILL_HEAL,
        SEID.SE_BATLE_COST_PUT,
        SEID.SE_BATLE_SKILL_EXEC,
        SEID.SE_BATLE_ATTACK_EN_NORMAL,
        SEID.SE_BATLE_COUNTDOWN_1,
        SEID.SE_BATLE_COUNTDOWN_2,
        SEID.SE_BATLE_COUNTDOWN_3,
        SEID.SE_BATLE_COUNTDOWN_4,
        SEID.SE_BATLE_COUNTDOWN_5,
        SEID.SE_BATLE_COUNTDOWN_6,
        SEID.SE_BATLE_COUNTDOWN_7,
        SEID.SE_BATLE_COUNTDOWN_8,
        SEID.SE_BATLE_COUNTDOWN_9,
        SEID.SE_BATLE_COST_PLUS_1,
        SEID.SE_BATLE_COST_PLUS_2,
        SEID.SE_BATLE_SKILL_CUTIN,
        SEID.SE_BATLE_ENEMY_TURN,
        SEID.SE_BATLE_UI_OPEN,
        SEID.SE_INGAME_PANEL_MEKURI,
        SEID.SE_INGAME_PANEL_MEKURI_NORMAL,
        SEID.SE_INGAME_PANEL_MEKURI_SPECIAL,
        SEID.SE_TRAP_LUCK, // 敵行動マスターデータに使用設定あり
        SEID.SE_TRAP_BAD, // 敵行動マスターデータに使用設定あり
        SEID.SE_BATTLE_ATTACK_FIRE,
        SEID.SE_BATTLE_ATTACK_WATER,
        SEID.SE_BATTLE_ATTACK_WIND,
        SEID.SE_BATTLE_ATTACK_LIGHT,
        SEID.SE_BATTLE_ATTACK_DARK,
        SEID.SE_BATTLE_ATTACK_NAUGHT,
        SEID.SE_BATTLE_BOSS_ALERT,
        SEID.SE_BATTLE_ATTACK_FIRST,
        SEID.SE_BATTLE_ATTACK_BACK,
        SEID.SE_BATTLE_BUFF,
        SEID.SE_BATTLE_DEBUFF,
        SEID.SE_INGAME_LEADERSKILL,
        SEID.SE_SKILL_COMBO_00,
        SEID.SE_SKILL_COMBO_01,
        SEID.SE_SKILL_COMBO_02,
        SEID.SE_SKILL_COMBO_03,
        SEID.SE_SKILL_COMBO_04,
        SEID.SE_SKILL_COMBO_05,
        SEID.SE_SKILL_COMBO_06,
        SEID.SE_SKILL_COMBO_07,
        SEID.SE_SKILL_COMBO_08,
        SEID.SE_SKILL_COMBO_MORE_THAN_08,
        SEID.VOICE_INGAME_QUEST_NICE,
        SEID.VOICE_INGAME_QUEST_GREAT,
        SEID.VOICE_INGAME_QUEST_BEAUTY, // SoundManager->SceneObjReferGameMain経由で使ってるぽい？→必要ないなら削除
        SEID.VOICE_INGAME_QUEST_EXCELLENT,
        SEID.VOICE_INGAME_QUEST_COOL,
        SEID.VOICE_INGAME_QUEST_UNBELIEVABLE,
        SEID.VOICE_INGAME_QUEST_MARVELOUS,
        SEID.VOICE_INGAME_QUEST_DIVINE,
        SEID.VOICE_INGAME_QUEST_FIRSTATTACK,
        SEID.VOICE_INGAME_QUEST_BACKATTACK,
        SEID.VOICE_INGAME_QUEST_HANDCARD_SET,

        // 以下AnimationEventで使ってるぽい→必要ないなら削除
        SEID.SE_STAGE_CLEAR_UI,
        SEID.SE_BATLE_WINDOW_OPEN,
        SEID.SE_BATLE_WINDOW_CLOSE,
        SEID.SE_BATLE_SKILL_REPLACE,
        SEID.SE_BATLE_COST_IN,
        SEID.SE_BATLE_SKILL_HANDS,
        SEID.SE_BATLE_SKILL_CAPTION,
        SEID.SE_BATLE_SKILL_LIMITBREAK_CUTIN,
        SEID.SE_BATLE_SKILL_LIMITBREAK_IMPACT,
        SEID.SE_INGAME_PANEL_SHOCK,
        SEID.SE_INGAME_ACTIVITY_ITEM,
        SEID.SE_INGAME_DOOR_OPEN,
        SEID.SE_INGAME_DOOR_BOSS_TATAKI,
        SEID.SE_INGAME_DOOR_BOSS_OPEN,
        SEID.SE_INGAME_QUEST_START_00,
        SEID.SE_INGAME_QUEST_START_02,
        SEID.SE_BATTLE_BOSS_APPEAR,
        SEID.SE_SKILL_COMBO_FINISH_WORD,
        SEID.SE_CHESS_FALL,
        SEID.SE_CHESS_MOVE,
        SEID.SE_DOOR_OPEN_NORMAL,
        SEID.SE_DOOR_OPEN_BOSS,
        SEID.SE_SPLIMITOVER,
        SEID.VOICE_INGAME_QUEST_READYTO,
        SEID.VOICE_INGAME_QUEST_MOVEON,
        SEID.VOICE_INGAME_QUEST_BOSSAPPEAR,
        SEID.VOICE_INGAME_QUEST_QUESTCLEAR,
        SEID.VOICE_INGAME_QUEST_GAMEOVER,
        SEID.VOICE_INGAME_QUEST_GETKEY,
        SEID.VOICE_INGAME_QUEST_STANDREADY,
        SEID.VOICE_INGAME_QUEST_SPLIMIT,
    };
}
