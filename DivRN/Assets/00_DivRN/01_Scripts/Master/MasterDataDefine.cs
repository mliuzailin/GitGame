/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MasterDataDefine.cs
	@brief	マスターデータ関連定義
	@author Developer
	@date 	2012/10/03
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//using ServerDataDefine;

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@brief	マスターデータタイプ
*/
//----------------------------------------------------------------------------

public static class MasterDataDefine
{
    public static List<EMASTERDATA> SQLiteHoldList()
    {
        List<EMASTERDATA> result = new List<EMASTERDATA>();

        foreach (EMASTERDATA d in Enum.GetValues(typeof(EMASTERDATA)))
        {
            if (d == EMASTERDATA.eMASTERDATA_MAX)
            {
                continue;
            }
            result.Add(d);
        }
        return result;
    }

    public static EMASTERDATA_SERVER Convert(this EMASTERDATA emasterdata)
    {
        switch (emasterdata)
        {
            case EMASTERDATA.eMASTERDATA_PARAM_ENEMY:
                return EMASTERDATA_SERVER.ENEMY;
            case EMASTERDATA.eMASTERDATA_ASSET_BUNDLE_PATH:
                return EMASTERDATA_SERVER.ASSET_PATH;
            case EMASTERDATA.eMASTERDATA_PARAM_CHARA:
                return EMASTERDATA_SERVER.CHARA;
            case EMASTERDATA.eMASTERDATA_PARAM_CHARA_EVOL:
                return EMASTERDATA_SERVER.CHARA_EVOL;
            default:
                return emasterdata.ToString().Replace("eMASTERDATA_", "").ToEnum<EMASTERDATA_SERVER>();
        }
    }

    public static EMASTERDATA Convert(this EMASTERDATA_SERVER emasterdataServer)
    {
        return ("eMASTERDATA_" + emasterdataServer.ToString()).ToEnum<EMASTERDATA>();
    }

    public static string GetTableName(this EMASTERDATA emasterdata)
    {
        return DivRNUtil.GetTableName(emasterdata.ToString().Replace("eMASTERDATA_", ""));
    }
}

public enum EMASTERDATA : int
{
    eMASTERDATA_DEFAULT_PARTY, //!< マスターデータタイプ：デフォルト関連：パーティ編成
    eMASTERDATA_AREA, //!< マスターデータタイプ：クエスト関連：エリア情報
    //eMASTERDATA_QUEST, //!< マスターデータタイプ：クエスト関連：クエスト情報
    //eMASTERDATA_QUEST_FLOOR, //!< マスターデータタイプ：クエスト関連：クエスト内フロア情報
    //eMASTERDATA_FLOOR_CATEGORY, //!< マスターデータタイプ：クエスト関連：階層内ランダムカテゴリ分布パターン
    //eMASTERDATA_FLOOR_EXPECT, //!< マスターデータタイプ：クエスト関連：階層内期待値分布パターン
    eMASTERDATA_PARAM_CHARA, //!< マスターデータタイプ：単体パラメータ関連：キャラ
    //  eMASTERDATA_PARAM_PANEL, //!< マスターデータタイプ：単体パラメータ関連：パネル効果
    //  eMASTERDATA_PARAM_PANEL_GROUP, //!< マスターデータタイプ：単体パラメータ関連：パネル効果グループ
    eMASTERDATA_PARAM_ENEMY, //!< マスターデータタイプ：単体パラメータ関連：敵キャラ
    eMASTERDATA_ENEMY_GROUP, //!< マスターデータタイプ：敵集団出現のグループデータ
    eMASTERDATA_SKILL_ACTIVE, //!< マスターデータタイプ：スキル関連：アクティブスキル
    eMASTERDATA_SKILL_LEADER, //!< マスターデータタイプ：スキル関連：リーダースキル
    eMASTERDATA_SKILL_PASSIVE, //!< マスターデータタイプ：スキル関連：パッシブスキル
    eMASTERDATA_SKILL_LIMITBREAK, //!< マスターデータタイプ：スキル関連：リミットブレイクスキル
    eMASTERDATA_SKILL_BOOST, //!< マスターデータタイプ：スキル関連：ブーストスキル
    eMASTERDATA_EVENT, //!< マスターデータタイプ：期間限定イベント
    eMASTERDATA_AREA_AMEND, //!< マスターデータタイプ：期間限定エリア補正
    eMASTERDATA_NOTIFICATION, //!< マスターデータタイプ：ローカル通知
    eMASTERDATA_GUERRILLA_BOSS, //!< マスターデータタイプ：ゲリラボス
    eMASTERDATA_GACHA, //!< マスターデータタイプ：ガチャ関連：ガチャ定義
                       //  eMASTERDATA_GACHA_ASSIGN, //!< マスターデータタイプ：ガチャ関連：ガチャ詳細アサイン
    eMASTERDATA_GACHA_GROUP, //!< マスターデータタイプ：ガチャ関連：ガチャグループ定義
    eMASTERDATA_PARAM_CHARA_EVOL, //!< マスターデータタイプ：キャラ関連：キャラ進化情報
    eMASTERDATA_USER_RANK, //!< マスターデータタイプ：ユーザー関連：ランク
    //  eMASTERDATA_LOGIN_CHAIN, //!< マスターデータタイプ：ログインボーナス関連：連続ログイン
    //  eMASTERDATA_LOGIN_TOTAL, //!< マスターデータタイプ：ログインボーナス関連：通算ログイン
    //  eMASTERDATA_LOGIN_EVENT, //!< マスターデータタイプ：ログインボーナス関連：期間限定ログイン
    //  eMASTERDATA_LOGIN_MONTHLY, //!< マスターデータタイプ：ログインボーナス関連：月間ログイン
    eMASTERDATA_PRESENT, //!< マスターデータタイプ：プレゼント定義
    eMASTERDATA_PRESENT_GROUP, //!< マスターデータタイプ：プレゼントグループ定義
    //  eMASTERDATA_ACHIEVEMENT, //!< マスターデータタイプ：アチーブメント定義
    eMASTERDATA_STORE_PRODUCT, //!< マスターデータタイプ：ストア商品
                               //  eMASTERDATA_STORE_PRODUCT_EVENT, //!< マスターデータタイプ：ストア商品(イベント用)
    eMASTERDATA_ASSET_BUNDLE_PATH, //!< マスターデータタイプ：AssetBundleパス
    eMASTERDATA_INFORMATION, //!< マスターデータタイプ：運営のお知らせ
    eMASTERDATA_ENEMY_ABILITY, //!< マスターデータタイプ：敵特性
    eMASTERDATA_ENEMY_ACTION_TABLE, //!< マスターデータタイプ：敵行動パターン定義
    eMASTERDATA_ENEMY_ACTION_PARAM, //!< マスターデータタイプ：敵行動定義
    eMASTERDATA_STATUS_AILMENT, //!< マスターデータタイプ：状態異常整理用定義
    eMASTERDATA_BEGINNER_BOOST, //!< マスターデータタイプ：初心者ブースト
    eMASTERDATA_QUEST_REQUIREMENT, //!< マスターデータタイプ：クエスト入場条件
    eMASTERDATA_AREA_CATEGORY, //!< マスターデータタイプ：エリアカテゴリ情報
    eMASTERDATA_TEXT_DEFINITION, //!< マスターデータタイプ：テキスト定義情報
    eMASTERDATA_LINK_SYSTEM, //!< マスターデータタイプ：リンクシステム
    //eMASTERDATA_TOPPAGE, //!< マスターデータタイプ：トップページ定義
    //eMASTERDATA_AUDIODATA, //!< マスターデータタイプ：サウンド再生情報定義
    //eMASTERDATA_POINTSHOP_PRODUCT, //!< マスターデータタイプ：ポイントショップ商品定義
    eMASTERDATA_GACHA_TICKET, //!< マスターデータタイプ：ガチャチケット
    eMASTERDATA_LIMIT_OVER, //!< マスターデータタイプ：限界突破
    eMASTERDATA_USE_ITEM, //!< マスターデータタイプ：消費アイテム
    eMASTERDATA_GLOBAL_PARAMS, //!< マスターデータタイプ：共通定義
    eMASTERDATA_QUEST_KEY, //!< マスターデータタイプ：クエストキー

    //  eMASTERDATA_ACHIEVEMENT_GROUP,
    eMASTERDATA_WEB_VIEW,           //!< WebView表示※
    eMASTERDATA_HERO,               //!< 主人公※
    eMASTERDATA_HERO_LEVEL,         //!< 主人公レベル※
    //  eMASTERDATA_SKILL_ACTIVE_HERO,
    eMASTERDATA_HERO_ADD_EFFECT_RATE,//!< 主人公付与効果※

    //  eMASTERDATA_TOPIC_INFORMATION,
    eMASTERDATA_STORY,              //!< ストーリー※

    //  eMASTERDATA_EVENT_POINT,
    //  eMASTERDATA_GROSSING_PRESENT,
    eMASTERDATA_ENEMY_HATE,         //!< 敵ヘイト※
    eMASTERDATA_RENEW_QUEST,        //!< 新クエスト※
    eMASTERDATA_NPC,                //!< NPC※
    eMASTERDATA_STORY_CHARA,        //!< ストーリーキャラ情報※
    eMASTERDATA_ILLUSTRATOR,        //!< 絵師情報※
    eMASTERDATA_REGION,             //!< リージョン※
    eMASTERDATA_RENEW_QUEST_SCORE,  //!< クエストスコア
    eMASTERDATA_PLAY_SCORE,         //!< プレイスコア
    eMASTERDATA_SCORE_EVENT,        //!< イベントスコア
    eMASTERDATA_SCORE_REWARD,       //!< スコア報酬
    eMASTERDATA_QUEST_APPEARANCE,   //!< クエスト演出差替え情報※
    eMASTERDATA_STEP_UP_GACHA,      //!< ステップアップガチャ情報
    eMASTERDATA_STEP_UP_GACHA_MANAGE, //!< ステップアップガチャ管理情報
    eMASTERDATA_CHALLENGE_QUEST,    //!< 成長ボスクエスト
    eMASTERDATA_CHALLENGE_EVENT,    //!< 成長ボスイベント
    eMASTERDATA_CHALLENGE_REWARD,   //!< 成長ボス報酬
    eMASTERDATA_GACHA_TEXT,          //!< ガチャテキスト管理
    eMASTERDATA_GACHA_TEXT_REF,      //!< ガチャテキスト参照管理
    eMASTERDATA_GENERAL_WINDOW,      //!< 汎用ウインドウ管理
    eMASTERDATA_MAX //!< マスターデータタイプ：
};

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータタイプ（サーバー側ID）
	@note	数値はServer版の管理しているWiki参照
*/
//----------------------------------------------------------------------------
public enum EMASTERDATA_SERVER : int
{
    USER_RANK = 10, // マスターデータタイプ（サーバー側ID）：ユーザーランク
    DEFAULT_PARTY = 11, // マスターデータタイプ（サーバー側ID）：デフォルトパーティー
    CHARA = 20, // マスターデータタイプ（サーバー側ID）：キャラパラメータ
    CHARA_EVOL = 21, // マスターデータタイプ（サーバー側ID）：キャラクター進化情報
    SKILL_LEADER = 22, // マスターデータタイプ（サーバー側ID）：リーダースキル
    SKILL_ACTIVE = 23, // マスターデータタイプ（サーバー側ID）：アクティブスキル
    SKILL_PASSIVE = 24, // マスターデータタイプ（サーバー側ID）：パッシブスキル
    SKILL_LIMITBREAK = 25, // マスターデータタイプ（サーバー側ID）：リミットブレイクスキル
    SKILL_BOOST = 26, // マスターデータタイプ（サーバー側ID）：ブーストスキル
    ENEMY = 30, // マスターデータタイプ（サーバー側ID）：エネミーパラメータ
    ENEMY_GROUP = 31, // マスターデータタイプ（サーバー側ID）：エネミーグループ
    GUERRILLA_BOSS = 32, // マスターデータタイプ（サーバー側ID）：ゲリラボス
    ENEMY_ACTION_TABLE = 33, // マスターデータタイプ（サーバー側ID）：敵行動テーブル
    ENEMY_ACTION_PARAM = 34, // マスターデータタイプ（サーバー側ID）：敵行動定義
    STATUS_AILMENT = 35, // マスターデータタイプ（サーバー側ID）：状態異常整理用定義
    AREA = 40, // マスターデータタイプ（サーバー側ID）：エリア情報
    QUEST = 41, // マスターデータタイプ（サーバー側ID）：クエスト情報
    //  QUEST_FLOOR = 42, // マスターデータタイプ（サーバー側ID）：クエスト内フロア情報
    //  CATEGORY_PATTERN = 43, // マスターデータタイプ（サーバー側ID）：階層内ランダムカテゴリ分布パターン
    //  EXPECT_PATTERN = 44, // マスターデータタイプ（サーバー側ID）：階層内期待値分布パターン
    //  PANEL = 45, // マスターデータタイプ（サーバー側ID）：パネル効果パラメータ
    //  PANEL_GROUP = 46, // マスターデータタイプ（サーバー側ID）：パネル分岐グループ
    QUEST_REQUIREMENT = 47, // マスターデータタイプ（サーバー側ID）：クエスト入場条件
    GACHA = 60, // マスターデータタイプ（サーバー側ID）：ガチャ定義
    //GACHA_ASSIGN = 61, // マスターデータタイプ（サーバー側ID）：ガチャ詳細アサイン
    GACHA_GROUP = 62, // マスターデータタイプ（サーバー側ID）：ガチャグループ定義
    LOGIN_TOTAL = 70, // マスターデータタイプ（サーバー側ID）：通算ログイン
    //LOGIN_CHAIN = 71, // マスターデータタイプ（サーバー側ID）：連続ログイン
    //LOGIN_EVENT = 72, // マスターデータタイプ（サーバー側ID）：期間限定ログイン
    EVENT = 73, // マスターデータタイプ（サーバー側ID）：期間限定イベント
    AREA_AMEND = 74, // マスターデータタイプ（サーバー側ID）：期間限定エリア補正
    PRESENT = 75, // マスターデータタイプ（サーバー側ID）：プレゼント定義
    //ACHIEVEMENT_LIST = 76, // マスターデータタイプ（サーバー側ID）：アチーブメント
    PRESENT_GROUP = 77, // マスターデータタイプ（サーバー側ID）：プレゼントグループ定義
    STORE_PRODUCT = 80, // マスターデータタイプ（サーバー側ID）：ストア商品一覧
    ASSET_PATH = 81, // マスターデータタイプ（サーバー側ID）：AssetBundleパス
    INFORMATION = 82, // マスターデータタイプ（サーバー側ID）：運営通知
    STORE_PRODUCT_EVENT = 83, // マスターデータタイプ（サーバー側ID）：ストアイベント情報
    //LOGIN_MONTHLY = 84, // マスターデータタイプ（サーバー側ID）：月間ログイン
    NOTIFICATION = 85, // マスターデータタイプ（サーバー側ID）：ローカル通知
    BEGINNER_BOOST = 86, // マスターデータタイプ（サーバー側ID）：初心者ブースト
    AREA_CATEGORY = 87, // マスターデータタイプ（サーバー側ID）：エリアカテゴリ情報
    //INVITATION = 88, // マスターデータタイプ（サーバー側ID）：招待イベント定義
    ENEMY_ABILITY = 89, // マスターデータタイプ（サーバー側ID）：エネミー特性
    TEXT_DEFINITION = 90, // マスターデータタイプ（サーバー側ID）：テキスト定義
    LINK_SYSTEM = 91, // マスターデータタイプ（サーバー側ID）：リンクシステム
    TOPPAGE = 92, // マスターデータタイプ（サーバー側ID）：トップページ
    AUDIO_DATA = 93, // マスターデータタイプ（サーバー側ID）：オーディオ再生情報
    //POINT_SHOP_PRODUCT = 94, // マスターデータタイプ（サーバー側ID）：ポイントショップ商品定義
    GACHA_TICKET = 95, // マスターデータタイプ（サーバー側ID）：ガチャチケット
    LIMIT_OVER = 96, // マスターデータタイプ（サーバー側ID）：限界突破
    USE_ITEM = 97, // マスターデータタイプ（サーバー側ID）：消費アイテム
    GLOBAL_PARAMS = 98, // マスターデータタイプ（サーバー側ID）：マスターデータタイプ：共通定義
    QUEST_KEY = 100, // マスターデータタイプ（サーバー側ID）：クエストキー

    //  ACHIEVEMENT_GROUP =101,
    WEB_VIEW = 102,             //!< WebView表示※
    HERO = 103,                 //!< 主人公※
    HERO_LEVEL = 104,           //!< 主人公レベル※
    //  SKILL_ACTIVE_HERO = 105,
    HERO_ADD_EFFECT_RATE = 106, //!< 主人公付与効果※

    //  TOPIC_INFORMATION=107,
    STORY = 108,                //!< ストーリー※

    //  EVENT_POINT=109,
    //  GROSSING_PRESENT=110,
    ENEMY_HATE = 111,           //!< 敵ヘイト※
    RENEW_QUEST = 112,          //!< 新クエスト※
    NPC = 113,                  //!< NPC※
    STORY_CHARA = 114,          //!< ストーリーキャラ情報※
    ILLUSTRATOR = 115,          //!< 絵師情報※
    REGION = 118,               //!< リージョン※
    RENEW_QUEST_SCORE = 119,    //!< クエストスコア
    PLAY_SCORE = 120,           //!< プレイスコア
    SCORE_EVENT = 121,          //!< イベントスコア
    SCORE_REWARD = 122,         //!< スコア報酬
    QUEST_APPEARANCE = 123,     //!< クエスト演出差し替え情報

    STEP_UP_GACHA = 125,        //!< ステップアップガチャ情報
    STEP_UP_GACHA_MANAGE = 126, //!< ステップアップガチャ管理情報

    CHALLENGE_QUEST = 127,      //!< 成長ボスクエスト
    CHALLENGE_EVENT = 128,      //!< 成長ボスイベント
    CHALLENGE_REWARD = 129,     //!< 成長ボス報酬

    GACHA_TEXT = 131,           //!< ガチャテキスト管理
    GACHA_TEXT_REF = 132,       //!< ガチャテキスト参照管理
    GENERAL_WINDOW = 133,       //!< 汎用ウインドウ管理

};
