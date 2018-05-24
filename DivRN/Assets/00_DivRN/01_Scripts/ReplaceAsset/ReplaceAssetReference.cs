using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// アセットの差し替え内容を設定するためのクラスです。
/// 「ReplaceAsset_???」という名前のゲームオブジェクトを作りそのコンポーネントにこのクラスを追加してください。
/// コンポーネントの各項目に必要事項を設定します。
/// このゲームオブジェクトをプレハブ化し、Asset Labels にアセットバンドル名を追加設定してください（このプレハブにだけ「Asset Labels」に値を設定すればＯＫ）
/// AssetBundles-Build を実行するとアセットバンドルが作られます（各プラットフォーム向けに作る必要があります）
///
/// プレハブ名の命名規則
///   ReplaceAsset_Menu  メニュー（バトル外）での差し替え設定
///   ReplaceAsset_BattleAll  バトル全体での差し替え設定（通常敵戦とボス戦とで差し替え内容が同じ場合はこれを使用）
///   ReplaceAsset_BattleNormal  バトル（通常敵戦）での差し替え設定（ReplaceAssetBattleAll とは同時には使用できません）
///   ReplaceAsset_BattleBoss  バトル（ボス敵戦）での差し替え設定（ReplaceAssetBattleAll とは同時には使用できません）
/// </summary>
[Serializable]
public class ReplaceAssetReference : MonoBehaviour
{
       public enum ChangeTimingType
    {
              NONE = 0,
              MENU = 1,   // メニュー
              BATTLE_NORMAL = 2,  // 通常敵戦
              BATTLE_BOSS = 3,    // ボス敵戦
    }

       private static readonly string[] ASSET_NAMES =
    {
        null,
        "ReplaceAsset_Menu",
        "ReplaceAsset_BattleNormal",
        "ReplaceAsset_BattleBoss",
    };
       private static readonly string ASSET_NAME_BATTLE_ALL = "ReplaceAsset_BattleAll";

       public ReplaceAssetReferenceEffect m_Effect = new ReplaceAssetReferenceEffect();
       public ReplaceAssetReferenceSE m_SE1 = new ReplaceAssetReferenceSE();
       public ReplaceAssetReferenceSE m_SE2 = new ReplaceAssetReferenceSE();
       public ReplaceAssetReferenceSE m_SE3 = new ReplaceAssetReferenceSE();
       public ReplaceAssetReferenceSprite m_Sprite = new ReplaceAssetReferenceSprite();

       public static ReplaceAssetReference getReplaceAssetReference(AssetBundle src_assetbundle, ReplaceAssetReference.ChangeTimingType timing_type)
    {
              string[] asset_names = src_assetbundle.GetAllAssetNames();
              for (int idx = 0; idx < asset_names.Length; idx++)
        {
                     asset_names[idx] = Path.GetFileName(asset_names[idx]);
        }

              string asset_name = searchName(asset_names, ASSET_NAMES[(int)timing_type]);

              if (asset_name == null)
        {
                     if (timing_type == ChangeTimingType.BATTLE_NORMAL
                            || timing_type == ChangeTimingType.BATTLE_BOSS
            )
            {
                            asset_name = searchName(asset_names, ASSET_NAME_BATTLE_ALL);
            }
        }

              if (asset_name != null)
        {
                     string load_name = Path.GetFileNameWithoutExtension(asset_name).ToLower();
                     GameObject replace_asset_prefab = src_assetbundle.LoadAsset<GameObject>(load_name);
                     if (replace_asset_prefab != null)
            {
                            ReplaceAssetReference ret_val = replace_asset_prefab.GetComponent<ReplaceAssetReference>();
                //Destroy(replace_asset_prefab);
                //replace_asset_prefab = null;
                            return ret_val;
            }
        }

              return null;
    }

       private static string searchName(string[] names, string name)
    {
              name += ".prefab";
              name = name.ToLower();
              for (int idx = 0; idx < names.Length; idx++)
        {
                     if (names[idx] == name)
            {
                            string ret_val = names[idx];
                            return ret_val;
            }
        }

              return null;
       }
}

[Serializable]
public class ReplaceAssetReferenceEffect
{
       public GameObject m_NAUGHT_MA_00;   //全体：無：物理：弱
       public GameObject m_NAUGHT_MA_01;   //全体：無：物理：強
       public GameObject m_NAUGHT_SA_00;   //単体：無：物理：弱
       public GameObject m_NAUGHT_SA_01;   //単体：無：物理：中
       public GameObject m_NAUGHT_SA_02;   //単体：無：物理：強
       public GameObject m_FIRE_MA_00; //全体：火：物理：弱
       public GameObject m_FIRE_MA_01; //全体：火：物理：強
       public GameObject m_FIRE_SA_00; //単体：火：物理：弱
       public GameObject m_FIRE_SA_01; //単体：火：物理：中
       public GameObject m_FIRE_SA_02; //単体：火：物理：強
       public GameObject m_WATER_MA_00;    //全体：水：物理：弱
       public GameObject m_WATER_MA_01;    //全体：水：物理：強
       public GameObject m_WATER_SA_00;    //単体：水：物理：弱
       public GameObject m_WATER_SA_01;    //単体：水：物理：中
       public GameObject m_WATER_SA_02;    //単体：水：物理：強
       public GameObject m_WIND_MA_00; //全体：風：物理：弱
       public GameObject m_WIND_MA_01; //全体：風：物理：強
       public GameObject m_WIND_SA_00; //単体：風：物理：弱
       public GameObject m_WIND_SA_01; //単体：風：物理：中
       public GameObject m_WIND_SA_02; //単体：風：物理：強
       public GameObject m_LIGHT_MA_00;    //全体：光：物理：弱
       public GameObject m_LIGHT_MA_01;    //全体：光：物理：強
       public GameObject m_LIGHT_SA_00;    //単体：光：物理：弱
       public GameObject m_LIGHT_SA_01;    //単体：光：物理：中
       public GameObject m_LIGHT_SA_02;    //単体：光：物理：強
       public GameObject m_DARK_MA_00; //全体：闇：物理：弱
       public GameObject m_DARK_MA_01; //全体：闇：物理：強
       public GameObject m_DARK_SA_00; //単体：闇：物理：弱
       public GameObject m_DARK_SA_01; //単体：闇：物理：中
       public GameObject m_DARK_SA_02; //単体：闇：物理：強
       public GameObject m_HM_S_FIRE;  //音符：単体：炎
       public GameObject m_HM_S_WATER; //音符：単体：水
       public GameObject m_HM_S_WIND;  //音符：単体：風
       public GameObject m_HM_S_LIGHT; //音符：単体：光
       public GameObject m_HM_S_DARK;  //音符：単体：闇
       public GameObject m_HM_S_NAUGHT;    //音符：単体：無
       public GameObject m_HM_M_FIRE;  //音符：全体：炎
       public GameObject m_HM_M_WATER; //音符：全体：水
       public GameObject m_HM_M_WIND;  //音符：全体：風
       public GameObject m_HM_M_LIGHT; //音符：全体：光
       public GameObject m_HM_M_DARK;  //音符：全体：闇
       public GameObject m_HM_M_NAUGHT;    //音符：全体：無
       public GameObject m_HEAL_1; //回復：小
       public GameObject m_SP_HEAL;    //回復：SP
       public GameObject m_BLOOD;  //吸血
       public GameObject m_POISON; //毒
       public GameObject m_BUFF;   //バフ
       public GameObject m_DEBUFF; //デバフ
       public GameObject m_ENEMY_SKILL_BUFF;   //敵スキル：バフ
       public GameObject m_ENEMY_SKILL_DEBUFF; //敵スキル：デバフ
       public GameObject m_ENEMY_SKILL_FIRE;   //敵スキル：大砲：炎
       public GameObject m_ENEMY_SKILL_WIND;   //敵スキル：大砲：風
       public GameObject m_ENEMY_SKILL_WATER;  //敵スキル：大砲：水
       public GameObject m_ENEMY_SKILL_LIGHT;  //敵スキル：大砲：光
       public GameObject m_ENEMY_SKILL_DARK;   //敵スキル：大砲：闇
       public GameObject m_ENEMY_SKILL_NAUGHT; //敵スキル：大砲：無
       public GameObject m_ENEMY_SKILL_HEAL_S; //敵スキル：回復：単体
       public GameObject m_ENEMY_SKILL_HEAL_M; //敵スキル：回復：全体
       public GameObject m_PLAYER_SKILL_BUFF;  //プレイヤー側に表示：バフ
       public GameObject m_PLAYER_SKILL_DEBUFF;    //プレイヤー側に表示：デバフ

       public GameObject m_BATTLE_COST_PLUS;   //手札２枚目持った時のエフェクト
       public GameObject m_UNIT_DROP;  //アイテムドロップ
       public GameObject m_PLAYER_DAMAGE_NAUGHT;   //プレイヤー側ダメージ：無
       public GameObject m_PLAYER_DAMAGE_FIRE; //プレイヤー側ダメージ：炎
       public GameObject m_PLAYER_DAMAGE_WATER;    //プレイヤー側ダメージ：水
       public GameObject m_PLAYER_DAMAGE_LIGHT;    //プレイヤー側ダメージ：光
       public GameObject m_PLAYER_DAMAGE_DARK; //プレイヤー側ダメージ：闇
       public GameObject m_PLAYER_DAMAGE_WIND; //プレイヤー側ダメージ：風
       public GameObject m_SKILL_COMP; //スキル成立：場に表示
       public GameObject m_SKILL_SETUP_NAUGHT; //スキル成立：プレイヤーへ飛んでいく
       public GameObject m_TURN_CHANGE;    //敵スキルターン変化
       public GameObject m_GAME_CLEAR; //ゲームクリア
       public GameObject m_ENEMY_EVOL1;    //敵進化：前半
       public GameObject m_ENEMY_EVOL2;    //敵進化：後半
       public GameObject m_ENEMY_DEATH;    //ザコ敵死亡
       public GameObject m_ENEMT_DEATH_BOSS;   //ボス敵死亡
       public GameObject m_HAND_CARD_CHANGE;   //手札変化エフェクト
       public GameObject m_HAND_CARD_DESTROY;  //手札削除エフェクト
}

[Serializable]
public class ReplaceAssetReferenceSE
{
       public AudioClip m_SE_STARGE_START; //!< SE：システムSE：ジングル：ゲーム開始
       public AudioClip m_SE_STARGE_CLEAR; //!< SE：システムSE：ジングル：ゲームクリア
       public AudioClip m_SE_STAGE_CLEAR_UI;   //!< SE：システムSE：ジングル：ゲームクリアUI
       public AudioClip m_SE_STARGE_GAMEOVER;    //!< SE：システムSE：ジングル：ゲームオーバー
       public AudioClip m_SE_BATLE_WINDOW_OPEN;        //!< SE：バトルSE：バトルウィンドウ：開く
       public AudioClip m_SE_BATLE_WINDOW_CLOSE;      //!< SE：バトルSE：バトルウィンドウ：閉じる
       public AudioClip m_SE_BATLE_COST_PUT;      //!< SE：バトルSE：コストを追加成立
       public AudioClip m_SE_BATLE_SKILL_EXEC;      //!< SE：バトルSE：スキルが発動
       public AudioClip m_SE_BATLE_SKILL_REPLACE;    //!< SE：バトルSE：スキルが置き換えられた
       public AudioClip m_SE_BATLE_ATTACK_EN_NORMAL;      //!< SE：バトルSE：攻撃音：敵側：デフォルト
       public AudioClip m_SE_BATLE_ATTACK_PC_NORMAL;      //!< SE：バトルSE：攻撃音：PC側：デフォルト
       public AudioClip m_SE_BATLE_COUNTDOWN_1;        //!< SE：システムSE：カウントダウン：1
       public AudioClip m_SE_BATLE_COUNTDOWN_2;        //!< SE：システムSE：カウントダウン：2
       public AudioClip m_SE_BATLE_COUNTDOWN_3;        //!< SE：システムSE：カウントダウン：3
       public AudioClip m_SE_BATLE_COUNTDOWN_4;        //!< SE：システムSE：カウントダウン：4
       public AudioClip m_SE_BATLE_COUNTDOWN_5;        //!< SE：システムSE：カウントダウン：5
       public AudioClip m_SE_BATLE_COUNTDOWN_6;        //!< SE：システムSE：カウントダウン：6
       public AudioClip m_SE_BATLE_COUNTDOWN_7;        //!< SE：システムSE：カウントダウン：7
       public AudioClip m_SE_BATLE_COUNTDOWN_8;        //!< SE：システムSE：カウントダウン：8
       public AudioClip m_SE_BATLE_COUNTDOWN_9;        //!< SE：システムSE：カウントダウン：9
       public AudioClip m_SE_BATLE_COST_PLUS_1;        //!< SE：システムSE：コスト吸着
       public AudioClip m_SE_BATLE_COST_PLUS_2;        //!< SE：システムSE：コスト吸着
       public AudioClip m_SE_BATLE_COST_IN;        //!< SE：システムSE：コスト配り
       public AudioClip m_SE_BATLE_SKILL_HANDS;        //!< SE：システムSE：～HANDSの音
       public AudioClip m_SE_BATLE_SKILL_CUTIN;        //!< SE：システムSE：スキルカットイン
       public AudioClip m_SE_BATLE_SKILL_CAPTION;    //!< SE：システムSE：スキルキャプション出現音
       public AudioClip m_SE_BATLE_SKILL_LIMITBREAK_CUTIN;      //!< SE：システムSE：LBSカットイン
       public AudioClip m_SE_BATLE_SKILL_LIMITBREAK_IMPACT;        //!< SE：システムSE：LBSインパクト
       public AudioClip m_SE_BATLE_ENEMY_TURN;      //!< SE：システムSE：敵ターン経過
       public AudioClip m_SE_BATLE_UI_OPEN;            //!< SE：システムSE：戦闘ウィンドウ開く
       public AudioClip m_SE_INGAME_PANEL_SELECT;    //!< SE：インゲームSE：パネル操作：選択音
       public AudioClip m_SE_INGAME_PANEL_MEKURI;    //!< SE：インゲームSE：パネル操作：めくり
       public AudioClip m_SE_INGAME_PANEL_MEKURI_NORMAL;      //!< SE:インゲームSE：パネル操作：めくり経過通常
       public AudioClip m_SE_INGAME_PANEL_MEKURI_SPECIAL;    //!< SE:インゲームSE：パネル操作：めくり経過特殊
       public AudioClip m_SE_INGAME_PANEL_SHOCK;      //!< SE：インゲームSE：パネル操作：叩きつけ音
       public AudioClip m_SE_INGAME_PANEL_MEKURI_S;        //!< SE：インゲームSE：パネル操作：めくり（無地パネル用）
       public AudioClip m_SE_INGAME_ACTIVITY_KEY;    //!< SE：インゲームSE：パネル発動音：鍵
       public AudioClip m_SE_INGAME_ACTIVITY_ITEM;      //!< SE：インゲームSE：パネル発動音：宝箱
       public AudioClip m_SE_INGAME_ACTIVITY_TRAP;      //!< SE：インゲームSE：パネル発動音：虎ばさみ
       public AudioClip m_SE_INGAME_ACTIVITY_BOMB;      //!< SE：インゲームSE：パネル発動音：地雷
       public AudioClip m_SE_INGAME_ACTIVITY_PITFALL;    //!< SE：インゲームSE：パネル発動音：落とし穴
       public AudioClip m_SE_INGAME_ACTIVITY_ENEMY;        //!< SE：インゲームSE：パネル発動音：敵
       public AudioClip m_SE_INGAME_ACTIVITY_TICKET;      //!< SE：インゲームSE：パネル発動音：チケット
       public AudioClip m_SE_INGAME_DOOR_OPEN;      //!< SE：インゲームSE：ドア作動音：開く
       public AudioClip m_SE_INGAME_DOOR_BOSS_TATAKI;    //!< SE：インゲームSE：ドア作動音：ボスドア：叩く
       public AudioClip m_SE_INGAME_DOOR_BOSS_OPEN;        //!< SE：インゲームSE：ドア作動音：ボスドア：開く
       public AudioClip m_SE_INGAME_PATH_PLUS;      //!< SE：インゲームSE：パス追加音
       public AudioClip m_SE_INGAME_QUEST_START_00;        //!< SE:インゲームSE：ReadyTo
       public AudioClip m_SE_INGAME_QUEST_START_01;        //!< SE:インゲームSE：MoveOn
       public AudioClip m_SE_INGAME_QUEST_START_02;        //!< SE:インゲームSE:UI退場音
       public AudioClip m_SE_MENU_OK;    //!< SE：メニューSE：肯定
       public AudioClip m_SE_MENU_OK2;      //!< SE：メニューSE：肯定（強）
       public AudioClip m_SE_MENU_NG;    //!< SE：メニューSE：否定
       public AudioClip m_SE_MENU_RET;      //!< SE：メニューSE：戻る
       public AudioClip m_SE_MAINMENU_BLEND_SELECT;        //!< SE：メインメニューSE：合成選択
       public AudioClip m_SE_MAINMENU_BLEND_SELECT_NG;      //!< SE：メインメニューSE：合成選択失敗
       public AudioClip m_SE_MAINMENU_BLEND_EXEC;    //!< SE：メインメニューSE：合成実行
       public AudioClip m_SE_MAINMENU_BLEND_CLEAR;      //!< SE：メインメニューSE：合成選択クリア
       public AudioClip m_SE_BATTLE_ENEMYDEATH;        //!< SE：バトルSE：敵死亡
       public AudioClip m_SE_BATTLE_SKILL_HEAL;        //!< SE：バトルSE：スキルによるHP回復
       public AudioClip m_SE_TITLE_CALL_W;      //!< SE：タイトルコール：女
       public AudioClip m_SE_TITLE_CALL_M;      //!< SE：タイトルコール：男 4.0.0ファイルがない // 番号がずれると問題があるので復活させました
       public AudioClip m_SE_SHUTTER_OPEN;      //!< SE：シャッター：開く
       public AudioClip m_SE_SHUTTER_CLOSE;        //!< SE：シャッター：閉じる
       public AudioClip m_SE_TRAP_CANCEL;    //!< SE：罠解除
       public AudioClip m_SE_TRAP_LUCK;        //!< SE：よい効果
       public AudioClip m_SE_TRAP_BAD;      //!< SE：わるい効果
       public AudioClip m_SE_BATTLE_ATTACK_FIRE;      //!< SE：炎属性攻撃
       public AudioClip m_SE_BATTLE_ATTACK_WATER;    //!< SE：水属性攻撃
       public AudioClip m_SE_BATTLE_ATTACK_WIND;      //!< SE：風属性攻撃
       public AudioClip m_SE_BATTLE_ATTACK_NAUGHT;      //!< SE：無属性攻撃
       public AudioClip m_SE_BATTLE_ATTACK_LIGHT;    //!< SE：光属性攻撃
       public AudioClip m_SE_BATTLE_ATTACK_DARK;      //!< SE：闇属性攻撃
       public AudioClip m_SE_BATTLE_ATTACK_HEAL;      //!< SE：回復属性攻撃
       public AudioClip m_SE_BATTLE_BOSS_ALERT;        //!< SE：ボスアラート
       public AudioClip m_SE_BATTLE_BOSS_APPEAR;      //!< SE：ボス登場
       public AudioClip m_SE_BATTLE_ATTACK_FIRST;    //!< SE：先制攻撃
       public AudioClip m_SE_BATTLE_ATTACK_BACK;      //!< SE：不意打ち攻撃
       public AudioClip m_SE_BATTLE_BUFF;    //!< SE：BUFFスキル
       public AudioClip m_SE_BATTLE_DEBUFF;        //!< SE：DEBUFFスキル
       public AudioClip m_SE_INGAME_LEADERSKILL;      //!< SE：リーダースキルパワーアップ
       public AudioClip m_SE_SKILL_COMBO_00;      //!< SE:スキルコンボ：00
       public AudioClip m_SE_SKILL_COMBO_01;      //!< SE:スキルコンボ：01
       public AudioClip m_SE_SKILL_COMBO_02;      //!< SE:スキルコンボ：02
       public AudioClip m_SE_SKILL_COMBO_03;      //!< SE:スキルコンボ：03
       public AudioClip m_SE_SKILL_COMBO_04;      //!< SE:スキルコンボ：04
       public AudioClip m_SE_SKILL_COMBO_05;      //!< SE:スキルコンボ：05
       public AudioClip m_SE_SKILL_COMBO_06;      //!< SE:スキルコンボ：06
       public AudioClip m_SE_SKILL_COMBO_07;      //!< SE:スキルコンボ：07
       public AudioClip m_SE_SKILL_COMBO_08;      //!< SE:スキルコンボ：08
       public AudioClip m_SE_SKILL_COMBO_MORE_THAN_08;    //!< SE:スキルコンボ：09
       public AudioClip m_SE_SKILL_COMBO_FINISH_WORD;    //!< SE：スキルコンボ：フィニッシュ
       public AudioClip m_SE_CHESS_FALL;      //!< SE:チェス駒：落下音
       public AudioClip m_SE_CHESS_MOVE;      //!< SE:チェス駒：移動音
       public AudioClip m_SE_DOOR_OPEN_NORMAL;      //!< SE:チェス駒：ドア開き：ノーマル
       public AudioClip m_SE_DOOR_OPEN_BOSS;      //!< SE:チェス駒：ドア開き：ボス
       public AudioClip m_SE_SPLIMITOVER;    //!< SE:SP切れUI

       public AudioClip m_SE_MM_A01_CHECK;      //!< SE:全体リソース：決定音
       public AudioClip m_SE_MM_A02_CHECK2;        //!< SE:全体リソース：決定音（大）
       public AudioClip m_SE_MM_A03_TAB;      //!< SE:全体リソース：タブ切り替え
       public AudioClip m_SE_MM_A04_BACK;    //!< SE:全体リソース：キャンセル(バック)音
       public AudioClip m_SE_MM_B01_EXP_GAUGE;      //!< SE:クエストリザルト：ゲージが伸びる音
       public AudioClip m_SE_MM_B02_RANKUP;        //!< SE:クエストリザルト：ランクアップ
       public AudioClip m_SE_MM_B04_RARE_START;        //!< SE:クエストリザルト：ユニットゲットレア度のUI
       public AudioClip m_SE_MM_B05_RARE_STAR_PUT;      //!< SE:クエストリザルト：星のはまる音
       public AudioClip m_SE_MM_B06_RARE_END;    //!< SE:クエストリザルト：レア度UIの最後に
       public AudioClip m_SE_MM_C01_SCRATCH_1_3;      //!< SE:スクラッチ：☆１～３ゲット
       public AudioClip m_SE_MM_C02_SCRATCH_4;      //!< SE:スクラッチ：☆４ゲット
       public AudioClip m_SE_MM_C03_SCRATCH_5_6;      //!< SE:スクラッチ：☆５～６ゲット
       public AudioClip m_SE_MM_C04_SCRATCH_RARE;    //!< SE:スクラッチ：レアめくり
       public AudioClip m_SE_MM_D01_FRIEND_UNIT;      //!< SE:強化合成：フレンドユニット
       public AudioClip m_SE_MM_D02_MATERIAL_UNIT;      //!< SE:強化合成：マテリアルユニット
       public AudioClip m_SE_MM_D04_LEVEL_UP;    //!< SE:強化合成：レベルアップ
       public AudioClip m_SE_MM_D09_EVOLVE_ROLL;      //!< SE:進化合成：演出回転
       public AudioClip m_SE_MM_D10_EVOLVE_COMP;      //!< SE:進化合成：進化後遷移
       public AudioClip m_SE_MM_D08_SALE;    //!< SE:売却：売却演出音

       public AudioClip m_VOICE_INGAME_QUEST_READYTO;    //!< Voice:A:ReadyTo
       public AudioClip m_VOICE_INGAME_QUEST_MOVEON;      //!< Voice:A:MoveOn
       public AudioClip m_VOICE_INGAME_QUEST_BOSSAPPEAR;      //!< Voice:A:BossAppear
       public AudioClip m_VOICE_INGAME_QUEST_QUESTCLEAR;      //!< Voice:A:QuestClear
       public AudioClip m_VOICE_INGAME_QUEST_GAMEOVER;      //!< Voice:A:GameOver
       public AudioClip m_VOICE_INGAME_QUEST_GETKEY;      //!< Voice:A:GETKEY
       public AudioClip m_VOICE_INGAME_QUEST_NICE;      //!< Voice:A:戦闘評価：NICE
       public AudioClip m_VOICE_INGAME_QUEST_GREAT;        //!< Voice:A:戦闘評価：GREAT
       public AudioClip m_VOICE_INGAME_QUEST_BEAUTY;      //!< Voice:A:戦闘評価：BEAUTY
       public AudioClip m_VOICE_INGAME_QUEST_EXCELLENT;        //!< Voice:A:戦闘評価：EXCELLENT
       public AudioClip m_VOICE_INGAME_QUEST_COOL;      //!< Voice:A:戦闘評価：COOL
       public AudioClip m_VOICE_INGAME_QUEST_UNBELIEVABLE;      //!< Voice:A:戦闘評価：UNBELIEVABLE
       public AudioClip m_VOICE_INGAME_QUEST_MARVELOUS;        //!< Voice:A:戦闘評価：MARVELOUS
       public AudioClip m_VOICE_INGAME_QUEST_DIVINE;      //!< Voice:A:戦闘評価：DIVINE
       public AudioClip m_VOICE_INGAME_QUEST_FIRSTATTACK;    //!< Voice:A：FIRSTATTACK
       public AudioClip m_VOICE_INGAME_QUEST_BACKATTACK;      //!< Voice:A：BACKATTACK
       public AudioClip m_VOICE_INGAME_QUEST_HANDCARD_SET;      //!< Voice:A:手札配り
       public AudioClip m_VOICE_INGAME_QUEST_STANDREADY;      //!< Voice:A:LBS発動
       public AudioClip m_VOICE_INGAME_QUEST_SPLIMIT;    //!< Voice:A:SPLimit

       public AudioClip m_VOICE_INGAME_MM_EVOLVE;    //!< Voice:A:進化
       public AudioClip m_VOICE_INGAME_MM_FOOT_FRIEND;      //!< Voice:A:フッタ：フレンド
       public AudioClip m_VOICE_INGAME_MM_FOOT_OTHERS;      //!< Voice:A:フッタ：その他
       public AudioClip m_VOICE_INGAME_MM_FOOT_QUEST;    //!< Voice:A:フッタ：クエスト
       public AudioClip m_VOICE_INGAME_MM_FOOT_SCRATCH;        //!< Voice:A:フッタ：スクラッチ
       public AudioClip m_VOICE_INGAME_MM_FOOT_SHOP;      //!< Voice:A:フッタ：ショップ
       public AudioClip m_VOICE_INGAME_MM_FOOT_UNIT;      //!< Voice:A:フッタ：ユニット
       public AudioClip m_VOICE_INGAME_MM_LEVELUP;      //!< Voice:A:レベルアップ
       public AudioClip m_VOICE_INGAME_MM_RANKUP;    //!< Voice:A:ランクアップ
       public AudioClip m_VOICE_INGAME_MM_SKILLUP;      //!< Voice:A:スキルアップ
       public AudioClip m_VOICE_INGAME_MM_UNIT_GET;        //!< Voice:A:ユニット取得：
       public AudioClip m_VOICE_INGAME_MM_UNIT_GET_1;    //!< Voice:A:ユニット取得：レア１
       public AudioClip m_VOICE_INGAME_MM_UNIT_GET_2;    //!< Voice:A:ユニット取得：レア２
       public AudioClip m_VOICE_INGAME_MM_UNIT_GET_3;    //!< Voice:A:ユニット取得：レア３
       public AudioClip m_VOICE_INGAME_MM_UNIT_GET_4;    //!< Voice:A:ユニット取得：レア４
       public AudioClip m_VOICE_INGAME_MM_UNIT_GET_5;    //!< Voice:A:ユニット取得：レア５
       public AudioClip m_VOICE_INGAME_MM_UNIT_GET_6;    //!< Voice:A:ユニット取得：レア６
       public AudioClip m_VOICE_INGAME_MM_UNIT_GET_7;    //!< Voice:A:ユニット取得：レア７

       public AudioClip m_VOICE_INGAME_MM_LINK_ON;      //!< Voice:A:リンクオン
       public AudioClip m_VOICE_INGAME_MM_HOME;        //!< Voice:A:ホーム
       public AudioClip m_VOICE_INGAME_MM_LIMITOVER;      //!< Voice:A:リミットオーバー
}


[Serializable]
public class ReplaceAssetReferenceSprite
{
       [Serializable]
       public class SpriteReplaceInfo
    {
              public string m_BaseName;
              public Sprite m_Sprite;
    }

       public SpriteReplaceInfo[] m_SpriteReplaceInfo;
}
