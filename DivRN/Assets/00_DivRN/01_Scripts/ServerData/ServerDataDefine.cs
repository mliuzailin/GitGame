/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	SaverDataDefine.cs
	@brief	テンプレートクラス
	@author Developer
	@date 	2012/10/04
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

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
namespace ServerDataDefine
{
    /*==========================================================================*/
    /*		define																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	APIタイプ
		@note	サーバー通信に使用するAPIのバリエーション
	*/
    //----------------------------------------------------------------------------
    public enum SERVER_API : int
    {
        SERVER_API_NONE = -1,
        SERVER_API_USER_CREATE,                //!< APIタイプ：ユーザー管理：ユーザー新規生成									RecvCreateUser
        SERVER_API_USER_AUTHENTICATION,        //!< APIタイプ：ユーザー管理：ユーザー承認										RecvUserAuthentication
        SERVER_API_USER_RENAME,                //!< APIタイプ：ユーザー管理：ユーザー名称変更									RecvRenameUser
        SERVER_API_USER_SELECT_DEF_PARTY,      //!< APIタイプ：ユーザー管理：ユーザー初期設定									RecvSelectDefParty
        SERVER_API_USER_RENEW,                 //!< APIタイプ：ユーザー管理：ユーザー再構築									RecvRenewUser
        SERVER_API_USER_RENEW_CHECK,           //!< APIタイプ：ユーザー管理：ユーザー再構築情報問い合わせ						RecvCheckRenewUser
        SERVER_API_MASTER_HASH_GET,            //!< APIタイプ：マスターデータ操作：マスターデータハッシュ取得					RecvGetMasterHash
        SERVER_API_MASTER_DATA_GET_ALL,        //!< APIタイプ：マスターデータ操作：マスターデータ実体取得（全種）				RecvMasterDataAll
        SERVER_API_GET_ACHIEVEMENT_GP,         //!< APIタイプ：マスターデータ操作：アチーブメントグループ実体取得				RecvAchievementGroup
        SERVER_API_GET_ACHIEVEMENT,            //!< APIタイプ：マスターデータ操作：アチーブメント実体取得						RecvMasterDataAchievement
        SERVER_API_GET_LOGIN_PACK,             //!< APIタイプ：ログイン情報：ログインパック情報取得							RecvLoginPack
        SERVER_API_GET_LOGIN_PARAM,            //!< APIタイプ：ログイン情報：ログイン情報取得									RecvLoginParam

        SERVER_API_FRIEND_LIST_GET,            //!< APIタイプ：フレンド操作：フレンド一覧取得									RecvFriendListGet
        SERVER_API_FRIEND_REQUEST,             //!< APIタイプ：フレンド操作：フレンド申請										RecvFriendRequest
        SERVER_API_FRIEND_CONSENT,             //!< APIタイプ：フレンド操作：フレンド申請承認									RecvFriendConsent
        SERVER_API_FRIEND_REFUSAL,             //!< APIタイプ：フレンド操作：フレンド解除										RecvFriendRefusal
        SERVER_API_FRIEND_SEARCH,              //!< APIタイプ：フレンド操作：フレンドユーザー検索								RecvFriendSearch

        SERVER_API_UNIT_PARTY_ASSIGN,          //!< APIタイプ：ユニット操作：ユニットパーティ編成設定							RecvUnitPartyAssign
        SERVER_API_UNIT_SALE,                  //!< APIタイプ：ユニット操作：ユニット売却										RecvUnitSale
        SERVER_API_UNIT_BLEND_BUILDUP,         //!< APIタイプ：ユニット操作：ユニット強化合成									RecvUnitBlendBuildUp
        SERVER_API_UNIT_BLEND_EVOL,            //!< APIタイプ：ユニット操作：ユニット進化合成									RecvUnitBlendEvol
        SERVER_API_UNIT_LINK_CREATE,           //!< APIタイプ：ユニット操作：ユニットリンク実行								RecvUnitLink
        SERVER_API_UNIT_LINK_DELETE,           //!< APIタイプ：ユニット操作：ユニットリンク解除								RecvUnitLink

        SERVER_API_QUEST_HELPER_GET,           //!< APIタイプ：クエスト操作：助っ人一覧取得									RecvQuestHelperGet
        SERVER_API_QUEST_HELPER_GET_EVOL,      //!< APIタイプ：クエスト操作：助っ人一覧取得（進化合成用）						RecvQuestHelperGetEvol
        SERVER_API_QUEST_HELPER_GET_BUILD,     //!< APIタイプ：クエスト操作：助っ人一覧取得（強化合成用）						RecvQuestHelperGetBuild
        SERVER_API_QUEST_START,                //!< APIタイプ：クエスト操作：クエスト開始										RecvQuestStart
        SERVER_API_QUEST_CLEAR,                //!< APIタイプ：クエスト操作：クエストクリア									RecvQuestClear
        SERVER_API_QUEST_RETIRE,               //!< APIタイプ：クエスト操作：クエストリタイア									RecvQuestRetire
        SERVER_API_QUEST_ORDER_GET,            //!< APIタイプ：クエスト操作：クエスト受託情報取得								RecvQuestOrderGet
        SERVER_API_QUEST_CONTINUE,             //!< APIタイプ：インゲーム中：コンティニュー									RecvQuestContinue
        SERVER_API_QUEST_RESET,                //!< APIタイプ：インゲーム中：リセット											RecvQuestReset
        SERVER_API_EVOL_QUEST_START,           //!< APIタイプ：クエスト操作：進化クエスト開始									RecvEvolQuestStart
        SERVER_API_EVOL_QUEST_CLEAR,           //!< APIタイプ：クエスト操作：進化クエストクリア								RecvEvolQuestClear


        SERVER_API_QUEST2_START,               //!< APIタイプ：クエスト操作：新クエスト開始
        SERVER_API_QUEST2_CLEAR,               //!< APIタイプ：クエスト操作：新クエストクリア
        SERVER_API_QUEST2_ORDER_GET,           //!< APIタイプ：クエスト操作：新クエスト受託情報取得
        SERVER_API_QUEST2_CESSAION_QUEST,      //!< APIタイプ：クエスト操作：新クエスト中断データ破棄通知

        SERVER_API_INJUSTICE,                  //!< APIタイプ：不正検出関連：不正検出送信										RecvInjustice
        SERVER_API_TUTORIAL,                   //!< APIタイプ：チュートリアル関連：進行集計									RecvTutorialStep

        SERVER_API_STONE_USE_UNIT,             //!< APIタイプ：魔法石使用：ユニット枠増設										RecvStoneUsedUnit
        SERVER_API_STONE_USE_FRIEND,           //!< APIタイプ：魔法石使用：フレンド枠増設										RecvStoneUsedFriend
        SERVER_API_STONE_USE_STAMINA,          //!< APIタイプ：魔法石使用：スタミナ回復										RecvStoneUsedStamina
        SERVER_API_GACHA_PLAY,                 //!< APIタイプ：ガチャ操作：ガチャ実行											RecvGachaPlay
        SERVER_API_GACHA_TICKET_PLAY,          //!< APIタイプ：ガチャ操作：ガチャチケット実行									RecvGachaTicketPlay
        SERVER_API_STONE_PAY_PREV_IOS,         //!< APIタイプ：課金操作：魔法石購入直前処理( iOS … AppStore )					RecvStorePayPrev_ios
        SERVER_API_STONE_PAY_PREV_ANDROID,     //!< APIタイプ：課金操作：魔法石購入直前処理( Android … GooglePlay )			RecvStorePayPrev_android
        SERVER_API_STONE_PAY_IOS,              //!< APIタイプ：課金操作：魔法石購入反映処理( iOS … AppStore )					RecvStorePay_ios
        SERVER_API_STONE_PAY_ANDROID,          //!< APIタイプ：課金操作：魔法石購入反映処理( Android … GooglePlay )			RecvStorePay_android

        SERVER_API_REVIEW_PRESENT,             //!< APIタイプ：ユーザーレビュー関連：レビュー遷移報酬							RecvReviewPresent
        SERVER_API_PRESENT_LIST_GET,           //!< APIタイプ：プレゼント関連：プレゼント一覧取得								RecvPresentListGet
        SERVER_API_PRESENT_OPEN,               //!< APIタイプ：プレゼント関連：プレゼント開封									RecvPresentOpen

        SERVER_API_TRANSFER_ORDER,             //!< APIタイプ：セーブ移行関連：パスワード発行
        SERVER_API_TRANSFER_EXEC,              //!< APIタイプ：セーブ移行関連：移行実行

#if BUILD_TYPE_DEBUG
        SERVER_API_DEBUG_EDIT_USER,            //!< APIタイプ：デバッグ機能関連：ユーザーランクアップ
        SERVER_API_DEBUG_UNIT_GET,             //!< APIタイプ：デバッグ機能関連：ユニット取得
        SERVER_API_DEBUG_GROWTH_CURVE,         //!< APIタイプ：デバッグ機能関連：成長曲線検証
        SERVER_API_DEBUG_QUEST_CLEAR,          //!< APIタイプ：デバッグ機能関連：クエストクリア情報改変
        SERVER_API_DEBUG_SEND_BATTLE_LOG,      //!< APIタイプ：デバッグ機能；バトルログアップロード
        SERVER_API_DEBUG_MASTER_DATA_GET_ALL2, //!< APIタイプ：デバッグ機能；バトルログアップロード
#endif
        SERVER_API_GET_STORE_EVENT,            //!< APIタイプ：マスターデータ操作：ストアイベント一覧取得
        SERVER_API_STORE_PAY_CANCEL,           //!< APIタイプ：課金操作：魔法石購入キャンセル

        SERVER_API_ACHIEVEMENT_OPEN,           //!< APIタイプ：アチーブメント開封
        SERVER_API_CHECK_SNS_LINK,             //!< APIタイプ：SNSIDとの紐付け確認
        SERVER_API_SET_SNS_LINK,               //!< APIタイプ：SNSIDとの紐付け
        SERVER_API_MOVE_SNS_SAVE_DATA,         //!< APIタイプ：SNSIDを使用したデータ移行

        SERVER_API_TUTORIAL_SKIP,              //!< APIタイプ：チュートリアルスキップ

        SERVER_API_GET_SNS_ID,                 //!< APIタイプ：SNSID作成

        SERVER_API_GET_POINT_SHOP_PRODUCT,     //!< APIタイプ：ポイントショップ：ショップ商品情報を取得		get_point_shop_product
        SERVER_API_POINT_SHOP_PURCHASE,        //!< APIタイプ：ポイントショップ：商品購入					point_shop_purchase
        SERVER_API_POINT_SHOP_LIMITOVER,       //!< APIタイプ：ポイントショップ：限界突破
        SERVER_API_POINT_SHOP_EVOL,            //!< APIタイプ：ポイントショップ：進化

        SERVER_API_USE_ITEM,                   //!< APIタイプ：アイテム使用

        SERVER_API_GET_BOX_GACHA_STOCK,        //!< APIタイプ：BOXガチャ在庫状況取得
        SERVER_API_RESET_BOX_GACHA_STOCK,      //!< APIタイプ：BOXガチャ在庫状況リセット
        SERVER_API_SET_CURRENT_HERO,           //!< APIタイプ：主人公選択：主人公選択			                     set_current_hero
        SERVER_API_EVOLVE_UNIT,                //!< APIタイプ：ユニット操作：ユニット進化		                     evolve_unit
        SERVER_API_GET_GACHA_LINEUP,           //!< APIタイプ：ユニット操作：ユニット進化		                     get_gacha_lineup

        SERVER_API_GET_GUERRILLA_BOSS_INFO,    //!< APIタイプ：ゲリラボス情報取得
        SERVER_API_MASTER_DATA_GET_ALL2,       //!< APIタイプ：マスターデータ操作：マスターデータ実体取得（全種）				RecvMasterDataAll
        SERVER_API_RENEW_TUTORIAL,             //!< APIタイプ：リニューアルチュートリアル関連：進行集計									RecvTutorialStep
        SERVER_API_GET_TOPIC_INFO,             //!< APIタイプ：ホームページのトピック : ニュース情報取得									RecvGetTopicInfo

        SERVER_API_PERIODIC_UPDATE,            //!< APIタイプ：定期データ更新(デバイストークン)
        SERVER_API_GET_PRESENT_OPEN_LOG,       //!< APIタイプ：プレゼント開封ログ取得
        SERVER_API_QUEST2_START_TUTORIAL,

        SERVER_API_QUEST2_STORY_CLEAR,         //!< APIタイプ：クエスト操作：ストーリクエストクリア
        SERVER_API_GET_USER_SCORE_INFO,        //!< APIタイプ：ユーザースコア情報取得
        SERVER_API_GET_SCORE_REWARD,           //!< APIタイプ：スコア報酬取得

        SERVER_API_GET_CHALLENGE_INFO,              //!< APIタイプ：成長ボスイベント情報取得
        SERVER_API_GET_CHALLENGE_REWARD,            //!< APIタイプ：成長ボスイベント報酬取得
        SERVER_API_CHALLENGE_QUEST_START,           //!< APIタイプ：クエスト操作：成長ボスクエスト開始
        SERVER_API_CHALLENGE_QUEST_CLEAR,           //!< APIタイプ：クエスト操作：成長ボスクエストクリア
        SERVER_API_CHALLENGE_QUEST_ORDER_GET,       //!< APIタイプ：クエスト操作：成長ボスクエスト受託情報取得
        SERVER_API_CHALLENGE_QUEST_CESSAION_QUEST,  //!< APIタイプ：クエスト操作：成長ボスクエスト中断データ破棄通知
        SERVER_API_CHALLENGE_QUEST_CONTINUE,        //!< APIタイプ：クエスト操作：コンティニュー
        SERVER_API_CHALLENGE_QUEST_RETIRE,          //!< APIタイプ：クエスト操作：クエストリタイア

        SERVER_API_DAY_STRADDLE,               //!< APIタイプ：日付変更情報取得

        SERVER_API_QUEST2_STORY_START,              //!< APIタイプ：クエスト操作：ストーリクエスト開始

        SERVER_API_MAX,                        //!< APIタイプ：
    };


    //----------------------------------------------------------------------------
    /*!
		@brief	APIリザルトコード
		@note	サーバーからの返信に含まれるリザルトコード。
				パケット固有のものだったり汎用的なものだったり。
				このコードはエラー時にユーザーに見える形で表示する。
	*/
    //----------------------------------------------------------------------------
    public enum API_CODE
    {
        //----------------------------------------------------------------------------
        // コードを追加したら下記の資料を更新すること
        // APIエラー一覧（クライアント定義）
        //----------------------------------------------------------------------------

        API_CODE_SUCCESS = 0x1000,      //!< APIリザルトコード：汎用系：成功

        //----------------------------------------------------------------------------
        // 自動再送系
        //----------------------------------------------------------------------------
        //		API_CODE_USER_UUID_ERR					= 0xaaaa, 		//!< APIリザルトコード：ユーザー認証：UUIDが不正				→ないっぽい
        API_CODE_USER_AUTH_REQUIRED = 0x2003,       //!< APIリザルトコード：ユーザー認証：再認証が必要
        API_CODE_USER_AUTH_REQUIRED2 = 0x2004,      //!< APIリザルトコード：ユーザー認証：再認証が必要

        API_CODE_WIDE_DEAD_LOCK = 0x2600,       //!< APIリザルトコード：一部APIでのデッドロック発生

        //----------------------------------------------------------------------------
        // アプリ強制停止系（復旧不可）
        // ※サーバー内部でダイアログを出してボタン押したら強制終了
        //----------------------------------------------------------------------------
        API_CODE_WIDE_ERROR_SYSTEM = 0x3000,        //!< APIリザルトコード：汎用エラー：システムエラー系
        API_CODE_WIDE_ERROR_DB = 0x3001,        //!< APIリザルトコード：汎用エラー：データベースエラー		※データ不整合が起きた時等。
        API_CODE_WIDE_ERROR_MASTER = 0x3002,        //!< APIリザルトコード：汎用エラー：マスターデータの不整合（クライアントの動作が保証出来ない）
        API_CODE_WIDE_ERROR_SERVER = 0x2000,        //!< APIリザルトコード：汎用エラー：サーバーイレギュラーエラー

        //----------------------------------------------------------------------------
        // 不正ユーザーにつき停止
        //----------------------------------------------------------------------------
        API_CODE_WIDE_ERROR_USER_INVALID = 0x2007,      //!< APIリザルトコード：汎用エラー：不正ユーザー認定

        //----------------------------------------------------------------------------
        // バージョンアップ示唆
        //----------------------------------------------------------------------------
        API_CODE_WIDE_ERROR_VERSION = 0x2001,       //!< APIリザルトコード：汎用エラー：バージョン不一致
        API_CODE_WIDE_ERROR_VERSION2 = 0x3101,      //!< APIリザルトコード：汎用エラー：バージョン不一致

        //----------------------------------------------------------------------------
        // アプリ強制停止系（任意タイミング復帰）
        // ※サーバー内部でダイアログを出してボタン入力待ち
        //----------------------------------------------------------------------------
        API_CODE_WIDE_ERROR_MENTENANCE = 0x3100,        //!< APIリザルトコード：汎用エラー：サーバーメンテナンス中

        //----------------------------------------------------------------------------
        // API固有分岐系（複数API参照）
        //----------------------------------------------------------------------------
        API_CODE_WIDE_API_SHORT_FP = 0x2027,        //!< APIリザルトコード：対価不足（フレンドポイント）
        API_CODE_WIDE_API_SHORT_TIP = 0x2028,       //!< APIリザルトコード：対価不足（チップ）
        API_CODE_WIDE_API_SHORT_TIP2 = 0x2301,      //!< APIリザルトコード：対価不足（チップ）
        API_CODE_WIDE_API_SHORT_COIN = 0x2026,      //!< APIリザルトコード：対価不足（コイン）
        API_CODE_WIDE_API_SHORT_STAMINA = 0x2501,       //!< APIリザルトコード：対価不足（スタミナ）
        API_CODE_WIDE_API_SHORT_STAMINA_MAX = 0x2502,       //!< APIリザルトコード：対価不足（スタミナMAX）
        API_CODE_WIDE_API_SHORT_TICKET = 0x2069,        //!< APIリザルトコード：対価不足（チケット）


        API_CODE_WIDE_API_UNIT_NONE = 0x2037,       //!< APIリザルトコード：[ クエスト , 進化 , 強化 , パーティ編成 , 売却	]：選択ユニットが存在しない
        API_CODE_WIDE_API_UNIT_OWNER_ERR = 0x2038,      //!< APIリザルトコード：[ クエスト , 進化 , 強化 , パーティ編成 , 売却	]：選択ユニットが他人のモノ
        API_CODE_WIDE_API_UNIT_SAME = 0x2042,       //!< APIリザルトコード：[ クエスト , 進化 , 強化 , パーティ編成 , 売却	]：同一ユニットが複数選択されている
        API_CODE_WIDE_API_UNIT_PARTY_ASSIGNED = 0x2043,     //!< APIリザルトコード：[		   , 進化 , 強化 ,				, 売却	]：選択ユニットがパーティアサインされている
        API_CODE_WIDE_API_UNIT_EXCESS = 0x2045,     //!< APIリザルトコード：[		   , 進化 , 強化 ,				, 売却	]：選択ユニットが過剰に多い


        API_CODE_WIDE_API_EVENT_ERR_FP = 0x2071,        //!< APIリザルトコード：[ クエスト , 進化 , 強化 ,				,		]：一部イベントが終了している（フレンドポイント関連）
        API_CODE_WIDE_API_EVENT_ERR_SLV = 0x2072,       //!< APIリザルトコード：[		   ,      , 強化 ,				,		]：一部イベントが終了している（スキルレベルアップ関連）

        //----------------------------------------------------------------------------
        // API固有分岐系（一部API限定）
        //----------------------------------------------------------------------------
        API_CODE_USER_CREATE_UUID_ERR = 0x2101,     //!< APIリザルトコード：ユーザー生成：UUID重複

        API_CODE_USER_RENAME_NG_WORD = 0x2100,      //!< APIリザルトコード：ユーザー名称変更：禁止文字列使用

        API_CODE_FRIEND_SEARCH_NONE = 0x2008,       //!< APIリザルトコード：フレンド検索：該当ユーザー無し

        API_CODE_GACHA_EVENT_ERR = 0x2073,      //!< APIリザルトコード：ガチャ：イベント期間終了( ランチタイムのイベント期間 )
        API_CODE_GACHA_EVENT_ALREADY = 0x2074,      //!< APIリザルトコード：ガチャ：既に引いている（ランチタイムガチャ想定）

        API_CODE_GACHA_STOP_STATUS_STOP = 0x2079,       //!< APIリザルトコード：ガチャ：ガチャストップステータス(停止)
        API_CODE_GACHA_STOP_STATUS_DISABLE = 0x2080,        //!< APIリザルトコード：ガチャ：ガチャストップステータス(無効)
        API_CODE_GACHA_EVENT_OUTSIDE = 0x2081,      //!< APIリザルトコード：ガチャ：ガチャ期間外
        API_CODE_GACHA_FIXID_ERROR = 0x2082,        //!< APIリザルトコード：ガチャ：ガチャIDエラー
        API_CODE_GACHA_INVALID_RANK_ERROR = 0x2093,     //!< APIリザルトコード：ガチャ：ガチャ不正対策

        API_CODE_STOP_BUY_TIP = 0x2083,     //!< APIリザルトコード：チップ購入ストップステータス(停止)
        API_CODE_STOP_USE_TIP_CONTINUE = 0x2084,        //!< APIリザルトコード：チップ消費(コンティニュー)ストップステータス(停止)
        API_CODE_STOP_USE_TIP_RETRY = 0x2085,       //!< APIリザルトコード：チップ消費(リトライ)ストップステータス(停止)
        API_CODE_STOP_USE_TIP_UNIT = 0x2086,        //!< APIリザルトコード：チップ消費(ユニット拡張)ストップステータス(停止)
        API_CODE_STOP_USE_TIP_FRIEND = 0x2087,      //!< APIリザルトコード：チップ消費(フレンド拡張)ストップステータス(停止)
        API_CODE_STOP_USE_TIP_STAMINA = 0x2088,       //!< APIリザルトコード：チップ消費(スタミナ回復)ストップステータス(停止)
        API_CODE_STOP_USE_TIP_PREMIUM = 0x2089,     //!< APIリザルトコード：チップ消費(プレミアム合成)ストップステータス(停止)
        API_CODE_STOP_USE_TIP_EVOL = 0x2090,        //!< APIリザルトコード：チップ消費(進化)ストップステータス(停止)
        API_CODE_STOP_AREA = 0x2091,        //!< APIリザルトコード：エリアストップステータス(停止)

        API_CODE_FRIEND_REQUEST_ALREADY_STATE = 0x2005,     //!< APIリザルトコード：フレンド申請：申請済で申請不要
        API_CODE_FRIEND_REQUEST_ALREADY_STATE2 = 0x2057,        //!< APIリザルトコード：フレンド申請：申請済で申請不要
        API_CODE_FRIEND_REQUEST_ALREADY_FRIEND = 0x2056,        //!< APIリザルトコード：フレンド申請：成立済で申請不要
        API_CODE_FRIEND_REQUEST_FULL_ME = 0x2200,       //!< APIリザルトコード：フレンド申請：自分がフレンド枠上限
        API_CODE_FRIEND_REQUEST_FULL_HIM = 0x2201,      //!< APIリザルトコード：フレンド申請：相手がフレンド枠上限
        API_CODE_FRIEND_REQUEST_RECEIVED = 0x2202,      //!< APIリザルトコード：フレンド申請：既に相手から申請されていた		→ 成功でいいんじゃね？
        API_CODE_FRIEND_REQUEST_CANCELED = 0x2203,      //!< APIリザルトコード：フレンド申請：承認しようとしたら相手からキャンセルされてた

        API_CODE_FRIEND_SELECT_NOT = 0x2022,        //!< APIリザルトコード：フレンド関連：相手ユーザーが選択されていない
        API_CODE_FRIEND_SELECT_NOT2 = 0x2024,       //!< APIリザルトコード：フレンド関連：相手ユーザーが選択されていない(破棄用？)
        API_CODE_FRIEND_SELECT_USER_NONE = 0x2023,      //!< APIリザルトコード：フレンド関連：相手ユーザーが存在しない
        API_CODE_FRIEND_SELECT_ME = 0x2021,     //!< APIリザルトコード：フレンド関連：フレンドに自分を指定

        API_CODE_QUEST_START_HELPER_ME = 0x2009,        //!< APIリザルトコード：クエスト開始：自身を助っ人にはできない
        API_CODE_QUEST_START_OVER_UNIT = 0x2010,        //!< APIリザルトコード：クエスト開始：所持ユニット数オーバー
        API_CODE_QUEST_START_OVER_UNIT2 = 0x2025,       //!< APIリザルトコード：クエスト開始：所持ユニット数オーバー
        API_CODE_QUEST_START_OVER_PARTY_COST = 0x2012,      //!< APIリザルトコード：クエスト開始：パーティコストオーバー
        API_CODE_QUEST_START_MASTER_NONE = 0x2013,      //!< APIリザルトコード：クエスト開始：クエストマスターが存在しない
        API_CODE_QUEST_START_BAD_CONDITION = 0x2500,        //!< APIリザルトコード：クエスト開始：クエストが期間外で受託できない
        API_CODE_QUEST_START_INVALID = 0x2055,      //!< APIリザルトコード：クエスト開始：無効状態のクエスト

        API_CODE_QUEST_RESULT_INJUSTICE = 0x2014,       //!< APIリザルトコード：クエストリザルト：クリアパラメータ上限超え	※チート
        API_CODE_QUEST_RESULT_ORDER_NONE = 0x2015,      //!< APIリザルトコード：クエストリザルト：クエスト受諾情報無し		※チート
        API_CODE_QUEST_RESULT_ORDER_ERR = 0x2016,       //!< APIリザルトコード：クエストリザルト：クエスト受諾情報エラー	※チート？

        API_CODE_QUEST_INGAME_RESET_CT_ZERO = 0x2017,       //!< APIリザルトコード：クエスト中：リセット回数指定が不正（０な場合）
        API_CODE_QUEST_INGAME_RESET_CT_ERR = 0x2018,        //!< APIリザルトコード：クエスト中：リセット回数比較エラー
        API_CODE_QUEST_INGAME_CONTINUE_CT_ZERO = 0x2019,        //!< APIリザルトコード：クエスト中：コンティニュー回数指定が不正（０な場合）
        API_CODE_QUEST_INGAME_CONTINUE_CT_ERR = 0x2020,     //!< APIリザルトコード：クエスト中：コンティニュー回数比較エラー

        API_CODE_EVOL_UNIT_LEVEL_SHORT = 0x2046,        //!< APIリザルトコード：進化クエスト開始：ベースユニットレベル不足
        API_CODE_EVOL_UNIT_LEVEL_SHORT2 = 0x2049,       //!< APIリザルトコード：進化クエスト開始：ベースユニットレベル不足
        API_CODE_EVOL_UNIT_AFTER_NONE = 0x2047,     //!< APIリザルトコード：進化クエスト開始：ベースユニット進化先が無い

        API_CODE_PAY_IOS_RECEIPT_ERR = 0x2029,      //!< APIリザルトコード：課金(iOS)：レシート情報不正
        API_CODE_PAY_IOS_RECEIPT_PURCHASE = 0x2031,     //!< APIリザルトコード：課金(iOS)：レシート使用済
        API_CODE_PAY_IOS_PRODUCT_ERR = 0x2030,      //!< APIリザルトコード：課金(iOS)：商品ID情報不正

        API_CODE_PAY_ANDROID_RECEIPT_ERR = 0x2032,      //!< APIリザルトコード：課金(Android)：レシート情報不正
        API_CODE_PAY_ANDROID_SIGNED_ERR = 0x2033,       //!< APIリザルトコード：課金(Android)：サイン情報不正
        API_CODE_PAY_ANDROID_PRODUCT_ERR = 0x2134,      //!< APIリザルトコード：課金(Android)：商品ID情報不正					→ないっぽい


        API_CODE_PAY_WIDE_TIP_MAX = 0x2300,     //!< APIリザルトコード：課金共通：有料分の最大数超過
        API_CODE_PAY_WIDE_ERROR = 0x2310,       //!< APIリザルトコード：課金共通：購入失敗
        API_CODE_PAY_WIDE_USER_NOT_MATCH = 0x2311,      //!< APIリザルトコード：課金共通：ユーザーIDエラー
        API_CODE_PAY_WIDE_APP_STORE_503 = 0x2312,       //!< APIリザルトコード：課金共通：AppleStoreへの問い合わせが繋がらなかった
        API_CODE_PAY_WIDE_AGE_BUDGET_OVER = 0x2075,     //!< APIリザルトコード：課金共通：年齢区分別の課金額上限突破

        API_CODE_PAY_WIDE_PRODUCT_ERR = 0x2065,     //!< APIリザルトコード：課金共通：プロダクトIDが定義されていない
        API_CODE_PAY_WIDE_PRODUCT_TIMEOUT = 0x2066,     //!< APIリザルトコード：課金共通：プロダクトIDが期間外
        API_CODE_PAY_WIDE_PRODUCT_EVENT_NONE = 0x2067,      //!< APIリザルトコード：課金共通：イベントIDに紐付いた商品IDがない
        API_CODE_PAY_WIDE_PRODUCT_EVENT_ALREADY = 0x2068,       //!< APIリザルトコード：課金共通：イベントIDに紐付いた商品を既に購入している

        API_CODE_STORE_MAX_UNIT = 0x2034,       //!< APIリザルトコード：ストア関連：拡張不可能（ユニット枠）
        API_CODE_STORE_MAX_FRIEND = 0x2401,     //!< APIリザルトコード：ストア関連：拡張不可能（フレンド枠）
        API_CODE_STORE_MAX_STAMINA = 0x2400,        //!< APIリザルトコード：ストア関連：スタミナ全快

        API_CODE_PRESENT_NONE = 0x2036,     //!< APIリザルトコード：プレゼント関連：プレゼントが存在しない
        API_CODE_PRESENT_SERIAL_ERR = 0x2041,       //!< APIリザルトコード：プレゼント関連：プレゼントシリアル番号が間違っている
        API_CODE_PRESENT_OPEN_NG = 0x2142,      //!< APIリザルトコード：プレゼント関連：プレゼントを開けると上限突破　※サーバーに定義が無いっぽい


        API_CODE_SALE_COIN_MAX = 0x2044,        //!< APIリザルトコード：ユニット売却：所持金最大

        API_CODE_REVIEW_ALREADY = 0x2054,       //!< APIリザルトコード：レビュー関連：既にレビューしている

        API_CODE_TRANSFER_USER_ID = 0x2058,     //!< APIリザルトコード：セーブ移行関連：ユーザーID不一致
        API_CODE_TRANSFER_PASSWORD = 0x2059,        //!< APIリザルトコード：セーブ移行関連：パスワード不一致
        API_CODE_TRANSFER_MOVED = 0x2061,       //!< APIリザルトコード：セーブ移行関連：移行済で継続不可
        API_CODE_TRANSFER_TIME_LIMIT = 0x2062,      //!< APIリザルトコード：セーブ移行関連：移行期限切れ

        API_CODE_RENEW_ERR_UUID_INVALID = 0x2063,       //!< APIリザルトコード：データ再構築エラー：UUIDの移行先が見つからない
        API_CODE_RENEW_ERR_USER_ID_BEFORE = 0x2064,     //!< APIリザルトコード：データ再構築エラー：UUIDの移行先が指す前回ユーザーIDが不一致

        API_CODE_BEGINNER_BOOST_FIXID_ERROR = 0x2076,       //!< APIリザルトコード：初心者ブースト：ID不正エラー
        API_CODE_BEGINNER_BOOST_RANK_ERROR = 0x2077,        //!< APIリザルトコード：初心者ブースト：指定IDのブーストはランク的に適用されない
        API_CODE_BEGINNER_BOOST_PERIOD_OUTSIDE = 0x2078,        //!< APIリザルトコード：初心者ブースト：イベントが終了している

        API_CODE_REQUIRE_QUEST_ERROR = 0x2092,      //!< APIリザルトコード：クエスト入場制限：条件を満たしていない

        API_CODE_MAKEPASSWORD_INVALID_RANK_ERROR = 0x2094,  //!< APIリザルトコード：パスワード発行：不正対策

        API_CODE_AREA_AMEND_FIXID_ERROR = 0x2095,       //!< APIリザルトコード：エリア補正：エリア補正ID不正エラー。
        API_CODE_AREA_AMEND_AREA_ERROR = 0x2096,        //!< APIリザルトコード：エリア補正：エリア補正IDと対象エリアが相違している。
        API_CODE_AREA_AMEND_PERIOD_OUTSIDE = 0x2097,        //!< APIリザルトコード：エリア補正：エリア補正IDイベントが終了している。

        API_CODE_QUEST_CONTINUE_ERROR = 0x2098,     //!< APIリザルトコード：コンティニュー不可エラー
        API_CODE_QUEST_RETRY_ERROR = 0x2099,        //!< APIリザルトコード：リトライ不可エラー

        //v300 リンクユニット関連
        API_CODE_LINKSYSTEM_SALE_ERROR = 0x2740,        //!< APIリザルトコード：ユニット売却		：リンクありにより売却できない
        API_CODE_LINKSYSTEM_COMPOSE_ERROR = 0x2741,     //!< APIリザルトコード：ユニット進化合成	：リンクありにより強化できない
        API_CODE_LINKSYSTEM_FORMUNIT_ERROR = 0x2742,        //!< APIリザルトコード：ユニット編成		：リンクありにより編成できない
        API_CODE_LINKSYSTEM_UNIT_DISABLE_ERROR = 0x2743,        //!< APIリザルトコード：リンク作成画面		：リンク不可ユニット
        API_CODE_LINKSYSTEM_EVOL_ERROR = 0x2744,        //!< APIリザルトコード：ユニット進化合成	：リンクありにより進化できない

        API_CODE_ACHIEVEMENT_REWARD_MAX = 0x2710,       //!< APIリザルトコード：アチーブメント：受け取り上限エラー（一部）
        API_CODE_ACHIEVEMENT_REWARD_MAX_ALL = 0x2711,       //!< APIリザルトコード：アチーブメント：受け取り上限エラー
        API_CODE_ACHIEVEMENT_REWARD_ERR_1_KEY = 0x2712, //!< APIリザルトコード：アチーブメント：一部を受け取れなかった＋有効期限切れのキーが存在した
        API_CODE_ACHIEVEMENT_REWARD_ERR_2_KEY = 0x2713, //!< APIリザルトコード：アチーブメント：全部を受け取れなかった＋有効期限切れのキーが存在した
        API_CODE_ACHIEVEMENT_REWARD_ERR_INVALID_KEY = 0x2714,   //!< APIリザルトコード：アチーブメント：報酬受け取りに有効でないキーが存在した(その他は受け取れた)

        SNS_MAX_LENGTH = 0x2720,        //!< APIリザルトコード：SNS機種移行：SNSアカウントの文字列長が最大値を超えている
        MOVE_SNS_SAVE_NONE_USER = 0x2725,       //!< APIリザルトコード：SNS機種移行：SNS_IDに紐付いたユーザーIDが存在しない

        GENERATE_SNS_ID_ERROR = 0x2730,     //!< APIリザルトコード：SNS機種移行：SNS_ID作成失敗(作成したIDのconflict10回).

        API_CODE_MASTERDATA_HASH_DIFFERENT = 0x2751,        //!< APIリザルトコード：ポイントショップ商品一覧：ハッシュコードが最新ではない

        API_CODE_POINT_SHOP_ERROR = 0x2752,     //!< APIリザルトコード：ポイントショップ：購入失敗
        API_CODE_POINT_SHOP_TIMING_ERROR = 0x2753,      //!< APIリザルトコード：ポイントショップ：購入期間外

        API_CODE_ITEM_NOTHING_ERROR = 0x2754,       //!< APIリザルトコード：消費アイテム：アイテムが存在しない
        API_CODE_ITEM_OVERLAP_ERROR = 0x2755,       //!< APIリザルトコード：消費アイテム：効果重複エラー

        API_CODE_POINT_SHOP_LO_UNIT_ERROR = 0x2756,     //!< APIリザルトコード：ポイントショップ：指定ユニットは限界突破出来ない
        API_CODE_POINT_SHOP_LO_COUNT_ERROR = 0x2757,        //!< APIリザルトコード：ポイントショップ：限界突破指定回数オーバー
        API_CODE_POINT_SHOP_LO_POINT_ERROR = 0x2758,        //!< APIリザルトコード：ポイントショップ：ユニットポイント不足

        API_CODE_QUEST_KEY_COUNT_ERROR = 0x2759,        //!< APIリザルトコード：クエスト開始：クエストキーが足りない
        API_CODE_QUEST_KEY_AREA_CLOSE = 0x2760,     //!< APIリザルトコード：クエスト開始：キークエストのエリアが終了状態

        API_CODE_GACHA_NOT_BOXGACHA = 0x2761,       //!< APIリザルトコード：ボックスガチャ在庫：ボックスガチャではない

        API_CODE_ACHIEVEMENT_GP_FIXID_ERROR = 0x2762,       //!< APIリザルトコード：アチーブメントリスト：アチーブメントグループIDが存在しない

        //v510 スコア関連
        API_CODE_SCORE_REWARD_INVALID_EVENT = 0x2780,       //!< APIリザルトコード：スコア報酬受け取り：無効なスコアイベント

        //v530　バトルログ
        API_CODE_POST_LOG_UPLOAD_ERROR = 0x2772,       //!< APIリザルトコード：postlog禁止環境エラー

        // v530 ステップアップガチャ
        API_CODE_STEP_UP_GACHA_REST_TIME_NOW = 0x2790, //!< APIリザルトコード：ガチャ：ステップ数がリセットされた

        //----------------------------------------------------------------------------
        // ユーザー凍結
        //----------------------------------------------------------------------------
        API_CODE_WIDE_ERROR_USER_FREEZE = 0x2750,       //!< APIリザルトコード：汎用エラー：ユーザー凍結中

        //----------------------------------------------------------------------------
        // デバッグ用
        //----------------------------------------------------------------------------
        API_CODE_DEBUG_ERROR_PERMISSION = 0x2006,       //!< APIリザルトコード：デバッグAPI：リクエストしたユーザーが「開発ユーザー」ではない

        //----------------------------------------------------------------------------
        // 対応要らないらしいやつ
        //----------------------------------------------------------------------------

        API_CODE_WIDE_ERROR_BAD_REQUEST = 0x2002,   // APIリザルトコード：汎用エラー：リクエストパラメータ不正
        API_CODE_WIDE_ERROR_BAD_REQUEST2 = 0x2051,  // APIリザルトコード：汎用エラー：リクエストパラメータ不正

        //		SERVER_ERROR				= 0x2000,	// 汎用的なエラーコード、現在はほぼ使っていない。
        //		PERMISSION_ERROR			= 0x2006,	// パーミッションエラー。ユーザー名検索でのエラーなので、対応はいりません。
        //		MASTER_DATA_CHANGED			= 0x2011,	// マスタデータが変更されている。アセットバンドル化したので、使用していません。
        //		NEED_POINT_ERROR			= 0x2048,	// 素材ポイント不足。これはrtdの仕様なので使用していません。
        //		UNIT_MASTER_ERROR			= 0x2039,	// ユニットマスターがない。rtdの名残っぽいです、フレンド間のプレゼントはないので対応はいりません。
        //		PRESENT_ERROR				= 0x2040,	// プレゼントできないユニット。rtdの名残っぽいです、フレンド間のプレゼントはないので対応はいりません。
        //		HASH_ERROR					= 0x2050,	// hash値エラー。アセットバンドル化によってこれも現在使用してません。
        //		VALIDATION_ERROR			= 0x2052,	// バリデーションエラー。rtdでは使用していましたが、エラーコードの細分化によって使用しなくなりましたので、対応はいりません。
        //		MASTER_DATA_INJUSTICE		= 0x2053,	// マスターデータが不正。アセットバンドル化によってこれも現在使用してません。


        //----------------------------------------------------------------------------
        // 以下はクライアント専用
        // サーバーからは送られてこないが、クライアント側で何らかの不具合が検出された場合の対処として準備
        //----------------------------------------------------------------------------
        //		API_CODE_CLIENT_ERROR					= 0x9000, 		// APIリザルトコード：クライアントエラー：クライアントエラー系。サーバからは返却されない。
        API_CODE_CLIENT_RETRY = 0x9001,         // APIリザルトコード：クライアントエラー：時間切れによる再送処理
        API_CODE_CLIENT_PARSE_FAILED = 0x9002,      // APIリザルトコード：クライアントエラー：レスポンスデータの解析に失敗した。
        API_CODE_CLIENT_PARSE_FAILED_CODE = 0x9003,         // APIリザルトコード：クライアントエラー：レスポンスデータのコード解析に失敗した。
        API_CODE_CLIENT_CSUM_ERROR = 0x9010,        // APIリザルトコード：クライアントエラー：チェックサム不一致

        //		API_CODE_CLIENT_TRANSFER				= 0x9004, 		// APIリザルトコード：クライアントエラー：セーブ移行発生

        API_CODE_Android_QueryInventoryFailed_ERROR = 0xFFFFFA1,        // CB_Android_QueryInventoryFailed
        API_CODE_Android_ConsumePurchaseFailed_ERROR = 0xFFFFFA2,       // CB_Android_ConsumePurchaseFailed
        API_CODE_Android_OnPurchaseReceipt_ERROR = 0xFFFFFA3,           // OnPurchaseReceipt

        API_CODE_OnStoreListWait_ERROR = 0xFFFFFB1,          		// OnStoreListWait
        API_CODE_OnPurchaseRestoreWait_ERROR = 0xFFFFFB2,           // OnPurchaseRestoreWait
        API_CODE_OnPurchaseBefor_ERROR = 0xFFFFFB3,                 // OnPurchaseBefor
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	API総合結果タイプ
		@note	サーバーのAPIリザルトコードはおおよそ以下に区分できる
					・アプリのバージョンアップが必須
					・アプリの再起動が必須
					・メンテナンス中なので一定時間後（ユーザー任意タイミング）に再送したい
					・接続が成立しないので一定時間後（ユーザー任意タイミング）に再送したい
					・API固有リザルトコード
				API固有リザルトコード以外は共通化できるので、結果タイプをもとにServerDataManager側でケアする
	*/
    //----------------------------------------------------------------------------
    public enum API_CODETYPE
    {
        API_CODETYPE_USUALLY,           //!< ServerDataManager外部判断：API固有の処理分岐で解決
        API_CODETYPE_INVALID_USER,      //!< ServerDataManager内部完結：「不正ユーザーが検出されました。起動できません」→復旧不可
        API_CODETYPE_VERSION_UP,        //!< ServerDataManager内部完結：「新しいバージョンがあるからストアで落としてね」→復旧不可
        API_CODETYPE_RESTART,           //!< ServerDataManager内部完結：「修復不可能なエラーが発生したから再起動してね」→復旧不可　※ダイアログ上にエラーコードを表示
        API_CODETYPE_MENTENANCE,        //!< ServerDataManager内部完結：「サーバーメンテナンス中なんでまた後で繋げてね」→ボタンおしたら再送
        API_CODETYPE_RETRY,             //!< ServerDataManager内部完結：「サーバーにつながりにくいから後でまた繋げてね」→ボタンおしたら再送
        API_CODETYPE_AUTH_REQUIRED,     //!< ServerDataManager内部完結：「再承認必須」
        API_CODETYPE_RETRY_AUTO,        //!< ServerDataManager内部完結：「自動再送」
        API_CODETYPE_TRANSFER_MOVED,    //!< ServerDataManager内部完結：「セーブ移行発生」
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	フレンド申請状況
	*/
    //----------------------------------------------------------------------------
    public enum FRIEND_STATE
    {
        FRIEND_STATE_SUCCESS = 0,       //!< フレンド申請状況：成立
        FRIEND_STATE_WAIT_HIM = 1,      //!< フレンド申請状況：自分から申請中。相手の判断待ち
        FRIEND_STATE_WAIT_ME = 2,       //!< フレンド申請状況：相手から申請中。自分の判断待ち
        FRIEND_STATE_UNRELATED = 3,         //!< フレンド申請状況：無関係
        FRIEND_STATE_PREMIUM = 4,       //!< フレンド申請状況：プレミアム枠
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	ログインボーナス起因タイプ
	*/
    //----------------------------------------------------------------------------
    public enum LOGINBONUS_TYPE
    {
        LOGINBONUS_TYPE_TOTAL = 0,      //!< 通算ログインボーナス
        LOGINBONUS_TYPE_CHAIN = 1,      //!< 連続ログインボーナス
        LOGINBONUS_TYPE_EVENT = 2,      //!< 期間限定ログインボーナス
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	ログインボーナス種類
	*/
    //----------------------------------------------------------------------------
    public enum LOGINBONUS_KIND
    {
        LOGINBONUS_KIND_NONE = 0,       //!< ログインボーナス種類：
        LOGINBONUS_KIND_MONEY = 1,      //!< ログインボーナス種類：お金
        LOGINBONUS_KIND_FP = 2,         //!< ログインボーナス種類：フレンドポイント
        LOGINBONUS_KIND_STONE = 3,      //!< ログインボーナス種類：魔法石
        LOGINBONUS_KIND_STONE_PAY = 5,      //!< ログインボーナス種類：魔法石（有料）
        LOGINBONUS_KIND_UNIT = 7,       //!< ログインボーナス種類：ユニット
        LOGINBONUS_KIND_TICKET = 8,         //!< ログインボーナス種類：チケット
    };


    //----------------------------------------------------------------------------
    /*!
		@brief	年齢区分
	*/
    //----------------------------------------------------------------------------
    public enum AGE_TYPE
    {
        AGE_TYPE_NONE = 0,      //!< 年齢区分：
        AGE_TYPE_00_15 = 1,         //!< 年齢区分：16歳未満
        AGE_TYPE_16_19 = 2,         //!< 年齢区分：16歳～19歳
        AGE_TYPE_20_xx = 3,         //!< 年齢区分：20歳以上
    };


    //----------------------------------------------------------------------------
    /*!
		@brief	プレゼント種類
	*/
    //----------------------------------------------------------------------------
    public enum PRESENT_TIMELIMIT
    {
        PRESENT_TIMELIMIT_30 = 0,       //!< プレゼント開封期限：30日
        PRESENT_TIMELIMIT_FREE = 1,         //!< プレゼント開封期限：無制限
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	強化合成成功タイプ
	*/
    //----------------------------------------------------------------------------
    public enum BUILDUP_TYPE
    {
        BUILDUP_TYPE_RATE_1_00 = 0,         //!< 成功パターン：成功
        BUILDUP_TYPE_RATE_1_25 = 1,         //!< 成功パターン：大成功
        BUILDUP_TYPE_RATE_1_50 = 2,     //!< 成功パターン：超成功
    };


    //----------------------------------------------------------------------------
    /*!
		@brief	アチーブメントステータスタイプ
	*/
    //----------------------------------------------------------------------------
    public enum ACHIEVEMENT_STATE
    {
        ACHIEVEMENT_STATE_NONE = 0,         //!< 出現(　) , 達成(　) , 演出(　) , 取得(　)　→　出現前
        ACHIEVEMENT_STATE_S1_T0_E0_S0 = 1,      //!< 出現(○) , 達成(　) , 演出(　) , 取得(　)　→　出現後
        ACHIEVEMENT_STATE_S1_T1_E0_S0 = 2,      //!< 出現(○) , 達成(○) , 演出(　) , 取得(　)　→　出現後 , 達成後
        ACHIEVEMENT_STATE_S1_T1_E1_S0 = 3,      //!< 出現(○) , 達成(○) , 演出(○) , 取得(　)　→　出現後 , 達成後 , 演出後
        ACHIEVEMENT_STATE_S1_T1_E1_S1 = 4,      //!< 出現(○) , 達成(○) , 演出(○) , 取得(○)　→　出現後 , 達成後 , 演出後 , 取得後
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	リンクシステム：リンクタイプ
	*/
    //----------------------------------------------------------------------------
    public enum CHARALINK_TYPE
    {
        CHARALINK_TYPE_NONE = 0,        //!< リンクタイプ：なし
        CHARALINK_TYPE_BASE = 1,        //!< リンクタイプ：ベースユニット
        CHARALINK_TYPE_LINK = 2,        //!< リンクタイプ：リンクユニット
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	プッシュ通知タイプ
	*/
    //----------------------------------------------------------------------------
    public enum NOTIFICATION_TYPE
    {
        NONE = 0,       //!< なし
        EVENT = 1,      //!< イベント定義からFIxIDを参照
        GACHA = 2,      //!< ガチャ定義からFixIDを参照
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	ミッション報酬・プレゼント開封エラータイプ
	*/
    //----------------------------------------------------------------------------
    public enum PRESENT_OPEN_ERROR_TYPE
    {
        NONE = 0,                  //!< なし
        COUNT_LIMIT = 1,           //!< 受け取りアイテム（ユニット）が上限に到達
        QUEST_KEY_EXPIRED = 2,     //!< クエストキー期限切れ
        RECEIVE_EXPIRED            //!< 開封有効期限切れ
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アチーブメントカテゴリタイプ
	*/
    //----------------------------------------------------------------------------
    public enum ACHIEVEMENT_CATEGORY_TYPE
    {
        NONE = 0,
        NORMAL = 1,
        EVENT = 2,
        DAILY = 3,
        QUEST = 4,
        REWARDED,
    }

    /*==========================================================================*/
    /*		macro																*/
    /*==========================================================================*/
    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		汎用構造体は、複数タイプの通信パケットに内包されることを想定。
		通信の回数削減のために一つのパケット内に複数情報を持たせるケースが多いが、
		バラバラに定義して一つ一つのパケットで処理分岐するのが面倒なので、汎用的な物を準備して使用する。
		送信情報はまとめるほどでもないので、概ね受信用に特化。
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：送信共通ヘッダ：
		@note	全ての送信パケットにはこれが接頭される。
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructHeaderSend
    {
        public string api_version;          //!< 送信ヘッダ：サーバーAPIバージョン
                                            //	※クライアント側が接続するAPIバージョンを送ることで、サーバーAPI側の拡張がある時にエラーを返してもらう

        public uint packet_unique_id;       //!< 送信ヘッダ：パケットユニークID
                                            //	※パケット管理番号。管理番号はパケット生成のたびに加算されていき、再送の際には更新されない。
                                            //	※受信したものの管理番号と処理済みの管理番号を比較して、新しいものだけ処理すればOK
        public PacketStructTerminal terminal;               //!< 送信共通ヘッダ：端末情報構造体
        public uint rank;                   //!< プレイヤー情報：ランク

        public string ad_id;                    //!< 広告ID
        public int ad_flag;             //!< 広告制限フラグ

        public long ott;                    //!< 送信ヘッダ：セキュリティコード（サーバー乱数）	※サーバーから届いたものをそのまま送り返す

        public ulong local_time;                //!< 送信ヘッダ：ローカル時間
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：受信共通ヘッダ：
		@note	全ての受信パケットにはこれが接頭される。
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructHeaderRecv
    {
        public int code;                    //!< 受信ヘッダ：リザルトコード	※受信データのエラーコード含む。パターンはパケット固有
        public string session_id;               //!< 受信ヘッダ：セッションID	※サーバー側のログイン情報のようなもの
        public string api_version;          //!< 受信ヘッダ：サーバーAPIバージョン

        public uint packet_unique_id;       //!< 受信ヘッダ：パケットユニークID　※送信で送ったものがそのまま帰ってくる想定

        public int system_flg;              //!< ※※未使用※※

        //		public	string		security_code;			//!< 送信ヘッダ：セキュリティコード（チェックサム）
        public string csum;                 //!< 送信ヘッダ：セキュリティコード（チェックサム）

        public long ott;                    //!< 送信ヘッダ：セキュリティコード（サーバー乱数）

        public ulong server_time;           //!< 受信ヘッダ：サーバー時間

        //----------------------------------------

        public uint[] achievement_new;      //!< 受信ヘッダ：アチーブメント関連：新規発生	    整数配列	※ [ 0 ～ ] ※MasterDataAchievement.fix_id と一致。通信により発生したアチーブメントの一覧を返してもらう
        public uint[] achievement_clear;        //!< 受信ヘッダ：アチーブメント関連：条件クリア	    整数配列	※ [ 0 ～ ] ※MasterDataAchievement.fix_id と一致。通信により達成したアチーブメントの一覧を返してもらう
        public RecvAchievementCount[] achievement_counter;  //!< 受信ヘッダ：アチーブメント関連：条件カウント	構造体
                                                            //----------------------------------------
                                                            // V320アチーブメントマスター取得タイミング変更対応
        public PacketAchievement[] achievement_clear_data;      //!< 受信ヘッダ：アチーブメント関連：条件クリアしたアチーブメントのマスターデータをもらう

        public int achievement_reward_count;    //!< 受信ヘッダ：アチーブメント関連：達成報酬を獲得していないアチーブメントの数(=メインメニュー「ミッション」の表示バッチ数)

        public int achievement_new_count;       //!< 受信ヘッダ：アチーブメント関連：新規発生数
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：アチーブメント関連
		@note	MasterDataAchievementConvertedの取得タイミング変更対応の為、
				ダイアログ表示に必要な部分のみ取得する
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketAchievement
    {
        public uint fix_id;                 //!< 固有ID
        public int category;                //!< カテゴリ
        public int achievement_category_id; //!< アチーブメントカテゴリID
        public int achievement_gp_id;       //!< アチーブメントグループID
        public string draw_msg;             //!< 表示用文言
        public uint[] present_ids;          //!< プレゼントID１				（MasterDataPresent.fix_id）
        public int server_state;            //!< サーバー側のステータス		※これはマスター出力の際には含まれない。サーバーからクライアントに情報を送る際に付与
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：端末情報：
		@note	ユーザーが持つ端末の情報。主にサービス分岐や運営時の情報収集に使用。
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructTerminal
    {
        public uint platform;       //!< 端末情報：プラットフォーム		※ [ 0:iOS ][ 1:Android ][ 2:その他 ]
        public string name;         //!< 端末情報：機種名
        public string os;               //!< 端末情報：OS名
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：アプリ起動情報：
		@note	アプリ起動時にカスタムURLスキーム対応として送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructBoot
    {
        public string scheme;           //!< 起動情報：アプリ起動元のホスト名
    };


    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：AssetBundle最新のパス情報
		@note	アプリ起動時にAssetBundle取得に使用
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructAssetPath
    {
        public string filename;     //!< ファイル名
        public string folder;           //!< バージョンパス
        public int version;     //!< 更新番号
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ユーザー基本情報：
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructUser
    {
        public uint user_id;        //!< ユーザー情報：ユーザー固有ID	※UUIDを元にサーバーが割り当てた固有ID。
        public string user_name;        //!< ユーザー情報：ユーザー名
        public uint user_group;     //!< ユーザー情報：ユーザーグループ	※付加軽減のため期間限定効果を時間差で行ったりする場合のグループ分け

        M4u.M4uProperty<string> username = new M4u.M4uProperty<string>();
        public string Username
        {
            get
            {
                Username = user_name;
                return username.Value;
            }
            set
            {
                username.Value = value;
            }
        }
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：システム情報：
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructSystem
    {
        public ulong server_time;   //!< システム情報：サーバー時間		※Unityは端末の設定時間を参照するためローカル情報はあてにならない。サーバーから時間を貰うことである程度整合
                                    //		public	uint		money_rate;		//!< システム情報：為替レート		※アプリ内でストア商品の表示をする際の単価表示に使用想定。
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ユニット単体情報：
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructUnit
    {
        //------------------------------------------------
        // ※※※※※※※※※※※※※※※※※※※※※※
        // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
        // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
        // ※※※※※※※※※※※※※※※※※※※※※※
        //------------------------------------------------
        public uint id;             //!< ユニット情報：キャラID		※マスターデータに含まれるキャラに紐付いたID。「カゼポックルは○○番」とかでキャラ種類依存の数値。
        public uint exp;            //!< ユニット情報：蓄積経験値
        public uint level;          //!< ユニット情報：レベル
                                    //		public	uint		unique_id;		//!< ユニット情報：ユニークID	※ユーザーが持つユニットのユニークID。
        public long unique_id;      //!< ユニット情報：ユニークID	※ユーザーが持つユニットのユニークID。
                                    //								※手持ちユニットに割り振って重複しない数値が入る。

        public uint add_pow;        //!< ユニット情報：付加ステータス：攻撃
        public uint add_def;        //!< ユニット情報：付加ステータス：防御
        public uint add_hp;         //!< ユニット情報：付加ステータス：体力

        public uint limitbreak_lv;  //!< ユニット情報：リミットブレイクスキルレベル
        public uint limitover_lv;   //!< ユニット情報：限界突破レベル

        // @add Developer 2015/09/03 ver300
        public long link_unique_id; //!< ユニット情報：リンク紐付け用ユニークID		※ユーザーが持つユニットのユニークID
        public uint link_point;     //!< ユニット情報：リンクポイント(リンクスキル発動率に影響 0 ~ 10000)
        public uint link_info;      //!< ユニット情報：(無 or 親 or 子)

        public ulong get_time;      //!< ユニット情報：入手時間


        public void Copy(PacketStructUnit cSrc)
        {
            id = cSrc.id;               // ユニット情報：キャラID
            exp = cSrc.exp;             // ユニット情報：蓄積経験値
            level = cSrc.level;         // ユニット情報：レベル
            unique_id = cSrc.unique_id;     // ユニット情報：ユニークID

            add_pow = cSrc.add_pow;         // ユニット情報：付加ステータス：攻撃
            add_def = cSrc.add_def;         // ユニット情報：付加ステータス：防御
            add_hp = cSrc.add_hp;           // ユニット情報：付加ステータス：体力

            limitbreak_lv = cSrc.limitbreak_lv; // ユニット情報：リミットブレイクスキルレベル
            limitover_lv = cSrc.limitover_lv;    // ユニット情報：限界突破レベル

            get_time = cSrc.get_time;       // ユニット情報：入手時間

            // @add Developer 2015/09/03 ver300
            link_unique_id = cSrc.link_unique_id;   // ユニット情報：リンク紐付け用ユニークID
            link_point = cSrc.link_point;       // ユニット情報：リンクポイント（リンクスキル発動率に影響）
            link_info = cSrc.link_info;     // ユニット情報：(無 or 親 or 子)
        }
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ユニット取得情報：
		@note	ローカル環境でユニットを取得した場合にサーバーに対して送信。リザルトでのゲーム中取得ユニット通知想定。
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructUnitGet
    {
        //------------------------------------------------
        // ※※※※※※※※※※※※※※※※※※※※※※
        // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
        // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
        // ※※※※※※※※※※※※※※※※※※※※※※
        //------------------------------------------------
        public uint id;             //!< ユニット情報：キャラID
        public uint level;          //!< ユニット情報：レベル

        public uint add_pow;        //!< ユニット情報：付加ステータス：攻撃
        public uint add_def;        //!< ユニット情報：付加ステータス：防御
        public uint add_hp;         //!< ユニット情報：付加ステータス：体力
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ユニット取得情報：
		@note	ローカル環境でユニットを取得した場合にサーバーに対して送信。デバッグメニュー用に定義。
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructUnitGetDebug
    {
        public uint id;             //!< ユニット情報：キャラID
        public uint limitbreak_lv;  //!< ユニット情報：リミットブレイクスキルレベル
        public uint limitover_lv;   //!< ユニット情報：限界突破レベル
        public uint level;          //!< ユニット情報：レベル
        public uint add_pow;        //!< ユニット情報：付加ステータス：攻撃
        public uint add_def;        //!< ユニット情報：付加ステータス：防御
        public uint add_hp;         //!< ユニット情報：付加ステータス：体力

        public void Copy(PacketStructUnitGetDebug cSrc)
        {
            id = cSrc.id;
            limitbreak_lv = cSrc.limitbreak_lv;
            limitover_lv = cSrc.limitover_lv;
            level = cSrc.level;
            add_pow = cSrc.add_pow;
            add_def = cSrc.add_def;
            add_hp = cSrc.add_hp;
        }

    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：プレイヤー情報：
        @description	注) ここのメンバはGetField()でアクセスしている場所があるのでpublic以外にしないでください
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructPlayer
    {
        public PacketStructUser user;               //!< ユーザー情報

        public int first_select_num;            //!< 初回ユニット選択パターン
        public int renew_first_select_num;      //!< リニューアルユニット選択パターン

        public uint rank;               //!< プレイヤー成長：ランク
        public uint exp;                //!< プレイヤー成長：蓄積経験値

        public int review;              //!< レビュー済フラグ ※[ 0:レビュー前 ][ 1:レビュー済 ]

        public uint pay_total;          //!< 課金額：総額
        public uint pay_month;          //!< 課金額：月間

        public uint have_money;         //!< 所持物：お金
        public uint have_stone_pay;     //!< 所持物：魔法石（有料分）
        public uint have_stone_free;    //!< 所持物：魔法石（無料分）
        public uint have_stone;         //!< 所持物：魔法石
        public uint have_ticket;        //!< 所持物：チケット
        public uint have_friend_pt;     //!< 所持物：フレンドポイント
        public uint have_unit_point;    //!< 所持物：ユニットポイント

        public uint stamina_now;        //!< スタミナ：現在値
        public uint stamina_max;        //!< スタミナ：最大値
        public ulong stamina_recover;   //!< スタミナ：回復時刻		※ローカルのカウントダウンはこれをもとにシミュレートする

        public uint total_unit;         //!< 課金補正込最大数：ユニット枠最大数
        public uint total_friend;       //!< 課金補正込最大数：フレンド枠最大数
        public uint total_party;        //!< 課金補正込最大数：パーティコスト最大数

        public byte[] flag_quest_check; //!< クエストフラグ：確認済み
        public byte[] flag_quest_clear; //!< クエストフラグ：クリア済み

        public byte[] flag_renew_quest_check; //!< 新クエストフラグ：確認済み
        public byte[] flag_renew_quest_clear; //!< 新クエストフラグ：クリア済み
        public byte[] flag_renew_quest_mission_complete; //!< 新クエストフラグ：ミッション全達成

        public byte[] flag_unit_check;  //!< ユニットフラグ：確認済み
        public byte[] flag_unit_get;        //!< ユニットフラグ：取得済み

        public byte[] flag_tutorial;        //!< チュートリアルフラグ：達成済み
        public int renew_tutorial_step;     //!< リニューアルチュートリアルステップ

        public PacketStructPartyAssign[] unit_party_assign; //!< ユニット関連：パーティ編成		※保持ユニットのユニークIDのみ保持
        public int unit_party_current;  //!< ユニット関連：パーティカレント	※選択してるパーティの番号
        public PacketStructUnit[] unit_list;                    //!< ユニット関連：保持ユニット

        public PacketStructUnit[] add_unit_list;                //!< ユニット関連：APIレシーブ用（追加ユニット）
        public PacketStructUnit[] update_unit_list;             //!< ユニット関連：APIレシーブ用（更新ユニット）
        public PacketStructUnit[] delete_unit_list;             //!< ユニット関連：APIレシーブ用（削除ユニット）

        public int extend_unit;     //!< 有料拡張：ユニット枠
        public int extend_friend;       //!< 有料拡張：フレンド枠

        public ulong lunch_play_time;   //!< ランチタイムガチャを最後に引いたサーバー時間

        public PacketStructUseItem[] item_list;     //!< 消費アイテムリスト
        public PacketStructQuestKey[] quest_key_list;   //!< クエストキーリスト
        public int current_hero_id;                 //!< サーバー側で保持しているヒーロー情報（hero_listのunique_idの値）

    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：フレンド情報：
		@note	他ユーザーの参照情報。実際はフレンド限定でなく、ランダムなユーザー情報が入ったりもする。
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructFriend
    {
        //------------------------------------------------
        // ※※※※※※※※※※※※※※※※※※※※※※
        // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
        // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
        // ※※※※※※※※※※※※※※※※※※※※※※
        //------------------------------------------------
        public uint user_id;                //!< フレンド情報：ユーザーID
        public string user_name;                //!< フレンド情報：ユーザー名称
        public uint user_rank;              //!< フレンド情報：ユーザーランク

        public ulong last_play;             //!< 最終プレイタイミング

        public uint friend_state;           //!< フレンド情報：フレンドステータス	※ [ 0:フレンド成立 ][ 1:申請中 ][ 2:承認待ち ][ 3:無関係 ][ 4:プレミアム枠 ]
        public ulong friend_state_update;   //!< フレンド情報：フレンドステータス更新時間
        public uint friend_point;           //!< フレンド情報：フレンドポイント

        public PacketStructUnit unit;                   //!< リーダーユニット情報
        public PacketStructUnit unit_link_s;            //!< リンクユニット情報実態
        public PacketStructUnit unit_link               //!< リンクユニット情報
        {
            get
            {
                if (unit_link_s == null)
                {
                    unit_link_s = new PacketStructUnit();
                }
                return unit_link_s;
            }
            set
            {
                unit_link_s = value;
            }
        }

        public void Copy(PacketStructFriend cSrc)
        {
            user_id = cSrc.user_id;
            user_name = cSrc.user_name;
            user_rank = cSrc.user_rank;

            last_play = cSrc.last_play;

            friend_state = cSrc.friend_state;
            friend_state_update = cSrc.friend_state_update;
            friend_point = cSrc.friend_point;

            unit = new PacketStructUnit();
            unit.Copy(cSrc.unit);

            unit_link = new PacketStructUnit();
            unit_link.Copy(cSrc.unit_link);
        }
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：クエスト受注情報：
		@note	クエスト受注中の場合、サーバーに最低限のデータを保持して、チート対策として使用する
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuest
    {
        public uint quest_id;               //!< クエスト情報：クエストID
        public uint quest_status;           //!< クエスト情報：クエストステータス
        public uint continue_ct;            //!< クエスト情報：コンティニュー回数
        public uint reset_ct;               //!< クエスト情報：リセット回数
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：マスターハッシュ情報：
		@note	マスターデータの取得は比較的重いので、ハッシュデータのみを取得してローカルと比較して変更されたマスターデータのみを再取得する。
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructMasterHash
    {
        public uint type;           //!< マスターハッシュ情報：マスターデータタイプ
        public string hash;         //!< マスターハッシュ情報：マスターデータハッシュ
        public uint timing;         //!< マスターハッシュ情報：マスターデータ前回取得タイミング
        public uint tag_id;         //!< マスターハッシュ情報：タグID

        //------------------------------------------------
        // ※※※※※※※※※※※※※※※※※※※※※※
        // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
        // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
        // ※※※※※※※※※※※※※※※※※※※※※※
        //------------------------------------------------
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ログインボーナス情報：
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructLoginBonus
    {
        public uint type;           //!< ボーナス情報：タイプ
        public uint value;          //!< ボーナス情報：数値
        public int bonus_type;      //!< ボーナス情報：ボーナスタイプ	※ [ 0：通算ログインボーナス ][ 1：連続ログインボーナス ][ 2：期間限定ログインボーナス ]
    };


    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：月間ログインボーナス情報：
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructLoginMonthly
    {
        public MasterDataLoginMonthlyConverted[] monthly_list;               //!< マスターデータ：月間ログイン
        public uint[] login_list;                                            //!< 月間ログインボーナス情報：ログインした日付の一覧
        public uint login_date;                                 //!< ログインした日付
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：期間ログインボーナス情報：
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructPeriodLogin
    {
        public MasterDataPeriodLoginConverted[] period_login;
        public uint login_count;
        public string event_name;
        public uint timing_start;
        public uint timing_end;
        public string resource_store_name;
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：その他プレゼント情報：
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructOthersPresent
    {
        public long[] serial_numbers;
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：プレゼント情報：
		@note
	*/
    //----------------------------------------------------------------------------
    //	[Serializable]
    public class PacketStructPresent
    {
        public long serial_id;          //!< プレゼント情報：シリアルID
        public ulong send_timing;       //!< プレゼント情報：発行タイミング
        public string message;          //!< プレゼント情報：添付メッセージ	※プレゼントタイプ：スクラッチの場合はjson

        public uint present_type;       //!< プレゼント情報：プレゼントタイプ
        public uint present_value0;     //!< プレゼント情報：プレゼント数値	※プレゼントタイプごとに汎用的に使用
        public uint present_value1;     //!< プレゼント情報：プレゼント数値	※プレゼントタイプごとに汎用的に使用
        public uint present_value2;     //!< プレゼント情報：プレゼント数値	※プレゼントタイプごとに汎用的に使用 ※サーバー側のプラス対応のため変数追加(v2000)
        public uint present_value3;     //!< プレゼント情報：プレゼント数値	※プレゼントタイプごとに汎用的に使用 ※サーバー側のプラス対応のため変数追加(v2000)
        public uint present_value4;     //!< プレゼント情報：プレゼント数値	※プレゼントタイプごとに汎用的に使用 ※サーバー側のプラス対応のため変数追加(v2000)
        public uint present_value5;     //!< プレゼント情報：プレゼント数値	※プレゼントタイプごとに汎用的に使用 ※サーバー側のプラス対応のため変数追加(v2700)
        public uint present_value6;     //!< プレゼント情報：プレゼント数値	※プレゼントタイプごとに汎用的に使用 ※サーバー側のプラス対応のため変数追加(v2700)
        public uint present_value7;     //!< プレゼント情報：プレゼント数値	※プレゼントタイプごとに汎用的に使用 ※サーバー側のプラス対応のため変数追加(v2700)
        public uint present_value8;     //!< プレゼント情報：プレゼント数値	※プレゼントタイプごとに汎用的に使用 ※サーバー側のプラス対応のため変数追加(v2700)
        public uint present_value9;     //!< プレゼント情報：プレゼント数値	※プレゼントタイプごとに汎用的に使用 ※サーバー側のプラス対応のため変数追加(v2700)

        public long delete_timing;      //!< プレゼント情報：破棄タイミング	※ [ 0:30日 ][ 1:無制限 ][ その他:サーバー時間 ]
        public int message_fix;     //!< プレゼント情報：文言固定フラグ	※ [ 0:固定無し ][ 1:固定 ]

        //		public PacketStructPresent()
        //		{
        //			Debug.LogError( StackTraceUtility.ExtractStackTrace() );
        //			Debug.LogError( "PacketStructPresent New!" );
        //		}
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：パーティアサイン情報
		@note	１パーティ分のユニットアサイン情報。１ユーザーは複数のパーティを保持する
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructPartyAssign
    {
        public long unit0_unique_id;        //!< ユニットユニークID
        public long unit1_unique_id;        //!< ユニットユニークID
        public long unit2_unique_id;        //!< ユニットユニークID
        public long unit3_unique_id;        //!< ユニットユニークID
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：イベント用アイテム購入の構造体
		@note	これまで購入したイベント用アイテム情報
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructEventPurchase
    {
        public string product_id;           //!< 製品ID
        public uint event_id;           //!< イベント固有ID
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：消費アイテム情報
		@note	現在ユーザーが所持している消費アイテム情報
	*/
    //----------------------------------------------------------------------------
    //	[Serializable]
    public class PacketStructUseItem
    {
        public uint item_id;            //!< 消費アイテム情報：消費アイテムID
        public uint item_cnt;           //!< 消費アイテム情報：個数
        public long use_timing;         //!< 消費アイテム情報：使用開始時間
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：クエストキー情報
		@note	現在ユーザーが所持しているクエストキー情報
	*/
    //----------------------------------------------------------------------------
    //	[Serializable]
    public class PacketStructQuestKey
    {
        public uint quest_key_id;       //!< クエストキー情報：クエストキーID
        public uint quest_key_cnt;      //!< クエストキー情報：キー数
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：クエストリザルト情報：
		@note	クエストのドロップ隠蔽のためにゲーム中で開いたパネル情報を一覧で送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuestResult
    {
        public int[] panel_unique;          //!< 開いたパネルのユニークID
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：クエスト構築情報：
		@note	クエストのドロップ隠蔽のためにゲーム中のフィールド情報はサーバーから取得
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuestBuild
    {
        //		public	PacketStructQuestBuildPanel[][]	panel;			//!< パネル汎用情報
        public PacketStructQuestBuildPanel[] panel;         //!< パネル汎用情報	※階層ごとに配列
        public int[] boss;          //!< ボス情報（エネミー情報構造体のユニークID）
        public PacketStructQuestBuildBattle[] list_battle;  //!< 乱数分岐が終わってFIXした戦闘１回分の情報を全てリスト化したもの
        public PacketStructQuestBuildItem[] list_item;      //!< 乱数分岐が終わってFIXしたアイテム１つ分の情報を全てリスト化したもの
        public PacketStructQuestBuildTrap[] list_trap;      //!< 乱数分岐が終わってFIXしたトラップ１つ分の情報を全てリスト化したもの
        public PacketStructQuestBuildDrop[] list_drop;      //!< ドロップユニット情報を全てリスト化したもの

        public int floor_count; //!< 階層カウント：階層総数
        public int under;           //!< 階層カウント：地下階層総数
        public int enemy_chain; //!< アイテム情報：エネミー連鎖パネル無効[ 0:効果なし][ 1:効果中 ]
        public uint[] list_used_item;   //!< アイテム情報：使用中アイテムのFixIdリスト

        public MasterDataParamEnemy[] list_e_param;     //!< 出現する敵ユニットのマスターデータ
        public MasterDataEnemyActionTable[] list_e_acttable;    //!< 出現する敵ユニットの行動テーブル
        public MasterDataEnemyActionParam[] list_e_actparam;    //!< 出現する敵ユニットの行動パラメータ

    };


    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：クエスト構築情報：パネル情報
		@note	戦闘１回分のパラメータ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuestBuildPanel
    {
        public PacketStructQuestBuildPanelDetail[] panel_detail;    // フロア内のパネル。１枚ごとに配列
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：クエスト構築情報：パネル情報
		@note	戦闘１回分のパラメータ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuestBuildPanelDetail
    {
        public int unique_id;           // パネル汎用情報：パネルユニークID（「どのパネルを開けたか」の送信用ユニークID。1以上の連番でOK)
        public int star;                // パネル汎用情報：パネルの星の数（1,2,3,4,5,6,!,?）
        public int risk;                // パネル汎用情報：パネルのリスク（赤,青,黄）
        public int type;                // パネル汎用情報：パネルのカテゴリ(戦闘,トラップ,アイテム,カラ,鍵)
        public int type_unique;     // パネル汎用情報：パネルのカテゴリ別ユニークID（エネミーリスト or トラップリスト or アイテムリスト ）
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：クエスト構築情報：戦闘情報
		@note	戦闘１回分のパラメータ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuestBuildBattle
    {
        public int unique_id;           // データユニークID( このクエスト受諾情報中で固有のエネミー情報番号 。1以上の連番でOK)
        public uint[] enemy_list;           // エネミーマスター参照用IDリスト( 左から右に向かってエネミーFixIDをリスト化 )
        public int[] drop_list;         // ドロップ情報参照リスト( 左から右に向かってドロップ情報構造体のユニークIDをリスト化 )
        public int chain;               // 戦闘連鎖先( StructBattleのunique_idで連鎖 )
        public int chain_turn_offset;       // 戦闘連鎖時ターンオフセット

        public int floor;               // パネル汎用情報：フロア番号
        public MasterDataDefineLabel.EvoluveEffectType evol_direction;      // 進化演出
    };


    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：クエスト構築情報：アイテム情報
		@note	戦闘１回分のパラメータ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuestBuildItem
    {
        public int unique_id;           // データユニークID( このクエスト受諾情報中で固有のアイテム情報番号。1以上の連番でOK )
        public uint item_id;            // アイテムマスター参照用FixID
        public int param_0;         // 取得金額 ※お金２倍イベント中等を考慮してマスターからの直接参照は避ける
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：クエスト構築情報：トラップ情報
		@note	戦闘１回分のパラメータ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuestBuildTrap
    {
        public int unique_id;           // データユニークID( このクエスト受諾情報中で固有のトラップ情報番号。1以上の連番でOK )
        public uint trap_id;            // トラップマスター参照用FixID
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：クエスト構築情報：ドロップ情報
		@note	戦闘１回分のパラメータ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuestBuildDrop
    {
        public int unique_id;               // データユニークID( このクエスト受諾情報中で固有のドロップ情報番号。1以上の連番でOK )
        public uint unit_id;                // ドロップするユニットFixID
        public int plus_pow;                // ドロップ時の攻撃プラス値
        public int plus_hp;             // ドロップ時の体力プラス値

        public int floor;                   // パネル汎用情報：フロア番号 ※
    };

    #region === 新クエスト関連構造体定義 ===
    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：新クエスト構築情報：
		@note	クエストのドロップ隠蔽のためにゲーム中のフィールド情報はサーバーから取得
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuest2Build
    {
        public int[] boss;          //!< ボス情報（エネミー情報構造体のユニークID）
        public PacketStructQuest2BuildBattle[] list_battle;  //!< 乱数分岐が終わってFIXした戦闘１回分の情報を全てリスト化したもの
        public PacketStructQuest2BuildDrop[] list_drop;      //!< ドロップユニット情報を全てリスト化したもの

        public uint[] list_used_item;   //!< アイテム情報：使用中アイテムのFixIdリスト

        public MasterDataParamEnemy[] list_e_param;     //!< 出現する敵ユニットのマスターデータ
        public MasterDataEnemyActionTable[] list_e_acttable;    //!< 出現する敵ユニットの行動テーブル
        public MasterDataEnemyActionParam[] list_e_actparam;    //!< 出現する敵ユニットの行動パラメータ

    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：新クエスト構築情報：戦闘情報
		@note	戦闘１回分のパラメータ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuest2BuildBattle
    {
        public int unique_id;               // データユニークID( このクエスト受諾情報中で固有のエネミー情報番号 。1以上の連番でOK)
        public uint[] enemy_list;           // エネミーマスター参照用IDリスト( 左から右に向かってエネミーFixIDをリスト化 )
        public int[] drop_list;             // ドロップ情報参照リスト( 左から右に向かってドロップ情報構造体のユニークIDをリスト化 )
        public int chain;                   // 戦闘連鎖先( StructBattleのunique_idで連鎖 )
        public int chain_turn_offset;       // 戦闘連鎖時ターンオフセット

        public PacketStructQuest2Hate hate; // ヘイト情報

        public int floor;               // パネル汎用情報：フロア番号
        public MasterDataDefineLabel.EvoluveEffectType evol_direction;      // 進化演出

        public int bgm_id;              // BGM ID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：新クエスト構築情報：ドロップ情報
		@note	戦闘１回分のパラメータ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuest2BuildDrop
    {
        public int unique_id;               // データユニークID( このクエスト受諾情報中で固有のドロップ情報番号。1以上の連番でOK )

        public uint unit_id;                // ユニットID（削除します）

        public int kind;                    // 種類　0 なし,1 カジノチケット,2 ゲーム内マネ,3 アイテム,4 ユニット,5 クエストキー
        public int item_id;                 // アイテムID,ユニットID,クエストキーID兼用
        public int num;                     // 個数

        public int plus_pow;                // ドロップ時の攻撃プラス値
        public int plus_hp;                 // ドロップ時の体力プラス値

        public int floor;                   // パネル汎用情報：フロア番号 ※


        public enum KindType
        {
            NONE = 0,       // なし
            TICKET = 1,     // カジノチケット
            MONEY = 2,      // ゲーム内マネ
            ITEM = 3,       // アイテム
            UNIT = 4,       // ユニット
            QUEST_KEY = 5,  // クエストキー
            MAX
        }

        public KindType getKindType()
        {
            return (KindType)kind;
        }

        public void setKindType(KindType kind_type)
        {
            kind = (int)kind_type;
        }
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：新クエスト構築情報：ヘイト情報
		@note	戦闘１回分のパラメータ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructQuest2Hate
    {
        public int fix_id;
        public int hate_initial;
        public int hate_given_damage1;
        public int hate_given_damage2;
        public int hate_given_damage3;
        public int hate_given_damage4;
        public int hate_given_damage5;
        public int hate_heal1;
        public int hate_heal2;
        public int hate_heal3;
        public int hate_heal4;
        public int hate_heal5;
        public int hate_rate_fire;
        public int hate_rate_water;
        public int hate_rate_wind;
        public int hate_rate_light;
        public int hate_rate_dark;
        public int hate_rate_naught;
        public int hate_rate_race1;
        public int hate_rate_race2;
        public int hate_rate_race3;
        public int hate_rate_race4;
        public int hate_rate_race5;
        public int hate_rate_race6;
        public int hate_rate_race7;
        public int hate_rate_race8;
        public int hate_rate_race9;
        public int hate_rate_race10;
    };
    #endregion

    //----------------------------------------------------------------------------
    /*!
		@brief	フロアボーナス　獲得ユニット情報
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructFloorBonus
    {
        //public int type_id;             // [ 0 ]コイン　[ 1 ]　チケット　[ 2 ]ユニットのインスタンスID
        public MasterDataDefineLabel.FloorBonusType type_id;             // [ 0 ]コイン　[ 1 ]　チケット　[ 2 ]ユニットのインスタンスID
        public long param;                  // [ 0 ]コイン枚数　[ 1 ]　チケット枚数　[ 2 ]獲得したユニットのインスタンスID
        public long param2;                 // [ 0 ]未対応　[ 1 ]未対応　[ 2 ]獲得したユニットの数量
        public string floor_count;          // 獲得フロア情報
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	ガチャ　BOXガチャの在庫情報
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructBoxGachaStock
    {
        public uint fix_id;             // MasterDataParamChara.fix_idと同じ値が入る
        public int n;                   // 現在在庫数
        public int m;                   // 最大在庫数
                                        // アクセサ
        public int now_stock            // 現在在庫数
        {
            get
            {
                return n;
            }
        }
        public int max_stock            // 最大在庫数
        {
            get
            {
                return m;
            }
        }
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	ガチャ　ガチャ情報
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructGachaStatus
    {
        public uint fix_id;                 // MasterDataGacha.fix_idと同じ値が入る
        public int present_count;           // プレゼントが付与された回数
        public ulong date;                  // 最後に引いた時間（タイムスタンプ）
        public int paid_tip_only_count; // 有料チップの引いた回数
        public int grossing_only_count; // 総付けの引いた回数
        public uint step_num;           // 現在のステップ
        public int is_first_time;       // 初回無料ガチャフラグ
        public PacketStructGrossingPresent[] grossing_present_list;     //総付プレゼントリスト
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：総付プレゼント情報
		@note	ユーザー入手したプレゼントのリスト情報
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructGrossingPresent
    {
        public int grossing_present_type;           // 総付プレゼント情報：タイプ
        public int grossing_present_id;         // 総付プレゼント情報：ID(fixid)
        public int grossing_present_num;            // 総付プレゼント情報：プレゼント数
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：総付プレゼントカウント情報
		@note	ユーザーが引いた回数のリスト情報
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructGrossingCount
    {
        public int grossing_present_id;         // 総付プレゼント情報：ID(fixid)
        public int grossing_present_count;          // 総付プレゼント情報：引いた回数
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	アチーブメントグループ：
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructAchievementGroup
    {
        public int fix_id;                  // MasterDataAchievementGroup.fix_idと同じ値が入る
        public string draw_msg;             // アチーブメントグループ：表示文言
        public int clear_cnt;               // アチーブメントグループ：グループ内ミッション達成数
        public int list_cnt;                // アチーブメントグループ：グループ内ミッション総数
        public int clear_unacquired_cnt;    // アチーブメントグループ：グループ内の ミッション達成済かつ報酬未取得ステータスのミッション数
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	スコア情報：
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructUserScoreInfo
    {
        public int event_id;                // MasterDataEventのevent_id
        public int hi_score;                // ハイスコア
        public int total_score;             // 累計スコア
        public PacketStructUserScoreReward[] reward_list;   // 褒賞リスト
        public PacketStructUserScoreReward[] get_list;      // 褒賞リスト（取得済み）
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	スコア情報：
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructUserScoreReward
    {
        public int fix_id;
        public int event_id;
        public int type;
        public int score;
        public int[] present_ids;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	スコア情報：
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructResultScore
    {
        public int event_id;
        public int base_score;
        public int quest_amend_value;
        public int unit_special_value;
        public int penalty;
        public int result_score;
        public bool hi_score;
        public PacketStructUserScoreReward[] reward_list;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	成長ボス情報：
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructChallengeInfo
    {
        public int event_id;
        public int challenge_level;
        public int max_level;
        public int skip_cnt;
        public int event_cnt;
        public int reward_challenge_cnt;
        public int reward_clear_cnt;
        public PacketStructChallengeInfoReward[] reward_list;
        public PacketStructChallengeGetReward[] get_list;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	成長ボス情報：リザルト情報
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructResultChallenge
    {
        public PacketStructChallengeGetReward[] get_list;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	成長ボス情報：報酬情報
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructChallengeInfoReward
    {
        public int type;
        public int start;
        public int end;
        public int loop_cnt;
        public int clear_param;
        public int status;
        public int[] present_ids;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	成長ボス情報：報酬情報
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructChallengeGetReward
    {
        public int fix_id;
        public int loop_count;
        public int[] present_ids;
    }

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
        @brief	通信パケット：情報解析用：
    */
    //----------------------------------------------------------------------------
    public class RecvDataBase
    {
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：情報解析用：
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvHeaderChecker
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ読み込み経由：ヘッダのみ返す用
		@note	Forgery処理で分岐が面倒なので用意
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvMasterDataWideUse
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public PacketStructMasterHash master_hash;  //!< 受信共通ヘッダ
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー生成：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー生成：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendCreateUser
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ

        public PacketStructTerminal terminal;       //!< 申請情報：端末情報
        public string uuid;         //!< 申請情報：UUID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー生成：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvCreateUser : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvCreateUserValue result;          //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー生成：受信実データ
	*/
    //----------------------------------------------------------------------------
    public class RecvCreateUserValue
    {
        public PacketStructPlayer player;           //!< 受信情報：プレイヤー情報
        public PacketStructSystem system;           //!< 受信情報：システム情報
        public PacketStructHero[] hero_list;       //!< 受信情報：HEROリスト
    };



    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー認証：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー認証：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendUserAuthentication
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ
        public PacketStructTerminal terminal;       //!< 申請情報：端末情報

        public PacketStructBoot boot;           //!< 申請情報：起動情報	※再送時のユーザー認証ではnull

        public string uuid;         //!< 申請情報：UUID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー認証：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvUserAuthentication : RecvDataBase
    {
        public PacketStructHeaderRecv header;       //!< 受信共通ヘッダ
        public RecvUserAuthenticationValue result;      //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー認証：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvUserAuthenticationValue
    {
        public PacketStructPlayer player;           //!< 受信情報：プレイヤー情報
        public PacketStructSystem system;           //!< 受信情報：システム情報
        public PacketStructQuest quest;         //!< 受信情報：クエスト受注情報
        public PacketStructHero[] hero_list;       //!< 受信情報：HEROリスト
    };



    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：AssetBundle最新のパス取得
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：AssetBundle最新のパス取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendAssetBundlePath
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：AssetBundle最新のパス取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvAssetBundlePath
    {
        public PacketStructHeaderRecv header;       //!< 受信共通ヘッダ
        public RecvAssetBundlePathValue result;     //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：AssetBundle最新のパス取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvAssetBundlePathValue
    {
        public PacketStructAssetPath[] asset_path;  //!< AssetBundleパスリスト
    };


    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー名称変更：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー名称変更：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendRenameUser
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ

        public string user_name;        //!< 申請情報：ユーザー名称
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー名称変更：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvRenameUser : RecvDataBase
    {
        public PacketStructHeaderRecv header;       //!< 受信共通ヘッダ
        public RecvRenameUserValue result;      //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー名称変更：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvRenameUserValue
    {
        public string user_name;        //!< 申請情報：ユーザー名称
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー初期パーティ選択：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー初期パーティ選択：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendSelectDefParty
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ

        public uint select_party;   //!< 申請情報：初期パーティ選択番号
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー初期パーティ選択：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvSelectDefParty : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvSelectDefPartyValue result;          //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー初期パーティ選択：受信実データ
	*/
    //----------------------------------------------------------------------------
    public class RecvSelectDefPartyValue
    {
        public PacketStructPlayer player;           //!< 受信情報：プレイヤー情報
        public PacketStructHero[] hero_list;            //!< 受信情報：HEROリスト
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー再構築：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー再構築：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendRenewUser
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ

        public PacketStructTerminal terminal;       //!< 申請情報：端末情報
        public string uuid;         //!< 申請情報：UUID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー再構築：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvRenewUser : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvRenewUserValue result;           //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー再構築：受信実データ
	*/
    //----------------------------------------------------------------------------
    public class RecvRenewUserValue
    {
        public PacketStructPlayer player;           //!< 受信情報：プレイヤー情報
        public PacketStructSystem system;           //!< 受信情報：システム情報
    };


    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー再構築情報問い合わせ：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー再構築情報問い合わせ：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendCheckRenewUser
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
        public string uuid_after;           //!< 問い合わせ情報：データ再構築でサーバーに送ったUUID
        public uint user_id_before;     //!< 問い合わせ情報：データ再構築する前のユーザーID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー再構築情報問い合わせ：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvCheckRenewUser : RecvDataBase
    {
        public PacketStructHeaderRecv header;       //!< 受信共通ヘッダ
        public RecvCheckRenewUserValue result;      //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユーザー再構築情報問い合わせ：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvCheckRenewUserValue
    {
        public string uuid_new; //!< 再構築後UUID
    };


    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターデータ情報取得（全種類）
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターデータ情報取得（全種類）：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGetMasterDataAll
    {
        public PacketStructHeaderSend header;                   //!< 送信共通ヘッダ

        public PacketStructMasterHash[] hash;                   //!< 送信情報：マスターハッシュ情報

        public uint[] achievement_viewed;       //!< 送信情報：アチーブメント関連：演出確認済み	※MasterDataAchievement.fix_idと一致。クライアント側が演出済みなアチーブメント一覧を送る
    };

    [Serializable]
    public class SendGetMasterDataAll2
    {
        public PacketStructHeaderSend header;                   //!< 送信共通ヘッダ

        public PacketStructMasterHash[] hash;                   //!< 送信情報：マスターハッシュ情報

        public uint[] achievement_viewed;       //!< 送信情報：アチーブメント関連：演出確認済み	※MasterDataAchievement.fix_idと一致。クライアント側が演出済みなアチーブメント一覧を送る
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターデータ情報取得（全種類）：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvMasterDataAll : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvMasterDataAllValue result;               //!< 受信実データ

    };
    [Serializable]
    public class RecvMasterDataAll2 : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvMasterDataAll2Value result;               //!< 受信実データ

    };

    public abstract class BaseMasterDiff<T> where T : Master, new()
    {
        public int[] del_list;
        public T[] upd_list;
    }

    public class MasterDataDefaultPartyDiff : BaseMasterDiff<MasterDataDefaultParty> { }
    public class MasterDataAssetBundlePathDiff : BaseMasterDiff<MasterDataAssetBundlePath> { }
    public class MasterDataInformationDiff : BaseMasterDiff<MasterDataInformation> { }
    public class MasterDataGachaDiff : BaseMasterDiff<MasterDataGacha> { }
    public class MasterDataGachaGroupDiff : BaseMasterDiff<MasterDataGachaGroup> { }
    public class MasterDataPresentDiff : BaseMasterDiff<MasterDataPresent> { }
    public class MasterDataAchievementConvertedDiff : BaseMasterDiff<MasterDataAchievementConverted> { }
    public class MasterDataEventDiff : BaseMasterDiff<MasterDataEvent> { }
    public class MasterDataNotificationDiff : BaseMasterDiff<MasterDataNotification> { }
    public class MasterDataUserRankDiff : BaseMasterDiff<MasterDataUserRank> { }
    public class MasterDataStoreProductDiff : BaseMasterDiff<MasterDataStoreProduct> { }
    public class MasterDataAreaAmendDiff : BaseMasterDiff<MasterDataAreaAmend> { }
    public class MasterDataBeginnerBoostDiff : BaseMasterDiff<MasterDataBeginnerBoost> { }
    public class MasterDataGuerrillaBossDiff : BaseMasterDiff<MasterDataGuerrillaBoss> { }
    public class MasterDataStatusAilmentParamDiff : BaseMasterDiff<MasterDataStatusAilmentParam> { }
    public class MasterDataEnemyGroupDiff : BaseMasterDiff<MasterDataEnemyGroup> { }
    public class MasterDataAreaCategoryDiff : BaseMasterDiff<MasterDataAreaCategory> { }
    public class MasterDataLinkSystemDiff : BaseMasterDiff<MasterDataLinkSystem> { }
    public class MasterDataLimitOverDiff : BaseMasterDiff<MasterDataLimitOver> { }
    public class MasterDataUseItemDiff : BaseMasterDiff<MasterDataUseItem> { }
    public class MasterDataGlobalParamsDiff : BaseMasterDiff<MasterDataGlobalParams> { }
    public class MasterDataAreaDiff : BaseMasterDiff<MasterDataArea> { }
    //public class MasterDataQuestDiff : BaseMasterDiff<MasterDataQuest> { }// TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
    public class MasterDataParamCharaDiff : BaseMasterDiff<MasterDataParamChara> { }
    public class MasterDataParamCharaEvolDiff : BaseMasterDiff<MasterDataParamCharaEvol> { }
    public class MasterDataSkillActiveDiff : BaseMasterDiff<MasterDataSkillActive> { }
    public class MasterDataSkillPassiveDiff : BaseMasterDiff<MasterDataSkillPassive> { }
    public class MasterDataSkillLimitBreakDiff : BaseMasterDiff<MasterDataSkillLimitBreak> { }
    public class MasterDataSkillLeaderDiff : BaseMasterDiff<MasterDataSkillLeader> { }
    public class MasterDataSkillBoostDiff : BaseMasterDiff<MasterDataSkillBoost> { }
    public class MasterDataParamEnemyDiff : BaseMasterDiff<MasterDataParamEnemy> { }
    public class MasterDataEnemyAbilityDiff : BaseMasterDiff<MasterDataEnemyAbility> { }
    public class MasterDataEnemyActionTableDiff : BaseMasterDiff<MasterDataEnemyActionTable> { }
    public class MasterDataEnemyActionParamDiff : BaseMasterDiff<MasterDataEnemyActionParam> { }
    public class MasterDataQuestRequirementDiff : BaseMasterDiff<MasterDataQuestRequirement> { }
    public class MasterDataTextDefinitionDiff : BaseMasterDiff<MasterDataTextDefinition> { }
    public class MasterDataTopPageDiff : BaseMasterDiff<MasterDataTopPage> { }
    public class MasterDataAudioDataDiff : BaseMasterDiff<MasterDataAudioData> { }
    public class MasterDataGachaTicketDiff : BaseMasterDiff<MasterDataGachaTicket> { }
    public class MasterDataQuestKeyDiff : BaseMasterDiff<MasterDataQuestKey> { }
    public class MasterDataStoreProductEventDiff : BaseMasterDiff<MasterDataStoreProductEvent> { }
    public class MasterDataHeroDiff : BaseMasterDiff<MasterDataHero> { }
    public class MasterDataHeroAddEffectRateDiff : BaseMasterDiff<MasterDataHeroAddEffectRate> { }
    public class MasterDataHeroLevelDiff : BaseMasterDiff<MasterDataHeroLevel> { }

    public class MasterDataLoginMonthlyDiff : BaseMasterDiff<MasterDataLoginMonthly> { }
    public class MasterDataGachaAssignDiff : BaseMasterDiff<MasterDataGachaAssign> { }
    public class MasterDataLoginEventDiff : BaseMasterDiff<MasterDataLoginEvent> { }
    public class MasterDataCategoryPatternDiff : BaseMasterDiff<MasterDataCategoryPattern> { }
    public class MasterDataExpectPatternDiff : BaseMasterDiff<MasterDataExpectPattern> { }
    public class MasterDataLoginChainDiff : BaseMasterDiff<MasterDataLoginChain> { }
    public class MasterDataLoginTotalDiff : BaseMasterDiff<MasterDataLoginTotal> { }
    public class MasterDataStoryDiff : BaseMasterDiff<MasterDataStory> { }
    public class MasterDataQuest2Diff : BaseMasterDiff<MasterDataQuest2> { }
    public class MasterDataNpcDiff : BaseMasterDiff<MasterDataNpc> { }
    public class MasterDataStoryCharaDiff : BaseMasterDiff<MasterDataStoryChara> { }
    public class MasterDataIllustratorDiff : BaseMasterDiff<MasterDataIllustrator> { }
    public class MasterDataWebViewDiff : BaseMasterDiff<MasterDataWebView> { }
    public class MasterDataRegionDiff : BaseMasterDiff<MasterDataRegion> { }
    public class MasterDataRenewQuestScoreDiff : BaseMasterDiff<MasterDataRenewQuestScore> { }
    public class MasterDataPlayScoreDiff : BaseMasterDiff<MasterDataPlayScore> { }
    public class MasterDataScoreEventDiff : BaseMasterDiff<MasterDataScoreEvent> { }
    public class MasterDataScoreRewardDiff : BaseMasterDiff<MasterDataScoreReward> { }
    public class MasterDataQuestAppearanceDiff : BaseMasterDiff<MasterDataQuestAppearance> { }
    public class MasterDataStepUpGachaDiff : BaseMasterDiff<MasterDataStepUpGacha> { }
    public class MasterDataStepUpGachaManageDiff : BaseMasterDiff<MasterDataStepUpGachaManage> { }
    public class MasterDataChallengeQuestDiff : BaseMasterDiff<MasterDataChallengeQuest> { }
    public class MasterDataChallengeEventDiff : BaseMasterDiff<MasterDataChallengeEvent> { }
    public class MasterDataChallengeRewardDiff : BaseMasterDiff<MasterDataChallengeReward> { }
    public class MasterDataGachaTextDiff : BaseMasterDiff<MasterDataGachaText> { }
    public class MasterDataGachaTextRefDiff : BaseMasterDiff<MasterDataGachaTextRef> { }
    public class MasterDataPresentGroupDiff : BaseMasterDiff<MasterDataPresentGroup> { }
    public class MasterDataGeneralWindowDiff : BaseMasterDiff<MasterDataGeneralWindow> { }


    [Serializable]
    public class RecvMasterDataAll2Value
    {

        public PacketStructMasterHash[] master_hash;                        //!< マスターデータハッシュ
                                                                            //----------------------------------------------------
                                                                            // もともとAssetBundleがある時からGetMasterでサーバー取得していたもの
                                                                            //----------------------------------------------------
        public MasterDataAssetBundlePathDiff master_array_asset_path;         //!< マスターデータ実体：AssetBundleパス
        public MasterDataInformationDiff master_array_information;            //!< マスターデータ実体：運営のお知らせ				※公開タイミングの概念あり
        public MasterDataGachaDiff master_array_gacha;                    //!< マスターデータ実体：ガチャ関連：ガチャ定義		※公開タイミングの概念あり
        public MasterDataGachaGroupDiff master_array_gacha_group;                    //!< マスターデータ実体：ガチャ関連：ガチャ定義グループ		※公開タイミングの概念あり
        public MasterDataPresentDiff master_array_present;                //!< マスターデータ実体：プレゼント定義
        public MasterDataAchievementConvertedDiff master_array_achievement;           //!< マスターデータ実体：アチーブメント定義			※公開タイミングの概念あり
        public MasterDataEventDiff master_array_event;                    //!< マスターデータ実体：期間限定イベント
        public MasterDataNotificationDiff master_array_notification;          //!< マスターデータ実体：プッシュ通知

        //----------------------------------------------------
        // 間引きとかせずに普通に渡してもらうもの
        //----------------------------------------------------
        public MasterDataUserRankDiff master_array_rank;                      //!< マスターデータ実体：ユーザー関連：ランク
        public MasterDataDefaultPartyDiff master_array_default_party;         //!< マスターデータ実体：デフォルト関連：パーティ編成
        public MasterDataStoreProductDiff master_array_store_product;         //!< マスターデータ実体：ストア商品
        public MasterDataAreaAmendDiff master_array_area_amend;               //!< マスターデータ実体：期間限定エリア補正
        public MasterDataBeginnerBoostDiff master_array_begginer_boost;       //!< マスターデータ実体：初心者ブースト
        public MasterDataGuerrillaBossDiff master_array_guerrilla_boss;       //!< マスターデータ実体：ゲリラボス
        public MasterDataStatusAilmentParamDiff master_array_status_ailment;  //!< マスターデータ実体：状態異常整理用定義
        public MasterDataEnemyGroupDiff master_array_enemy_group;             //!< マスターデータ実体：単体パラメータ関連：エネミーグループ
        public MasterDataAreaCategoryDiff master_array_area_category;         //!< マスターデータ実体：クエスト図鑑関連：エリアカテゴリ情報
        public MasterDataLinkSystemDiff master_array_link_system;             //!< マスターデータ実体：リンクシステム
        public MasterDataLimitOverDiff master_array_limit_over;               //!< マスターデータ実体：限界突破情報
        public MasterDataUseItemDiff master_array_item;                       //!< マスターデータ実体：消費アイテム
        public MasterDataGlobalParamsDiff master_array_global_params;         //!< マスターデータ実体：共通定義
        public MasterDataGeneralWindowDiff master_array_general_window;       //!< マスターデータ実体：汎用ウィンドウ

        //----------------------------------------------------
        // 一般公開タイミングを考慮しつつ渡してもらうもの
        //----------------------------------------------------
        public MasterDataAreaDiff master_array_area;                  //!< マスターデータ実体：クエスト関連：エリア情報
                                                                      // TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
                                                                      //public MasterDataQuestDiff master_array_quest;                    //!< マスターデータ実体：クエスト関連：クエスト情報
        public MasterDataParamCharaDiff master_array_chara;                   //!< マスターデータ実体：単体パラメータ関連：キャラ
        public MasterDataParamCharaEvolDiff master_array_chara_evol;          //!< マスターデータ実体：単体パラメータ関連：キャラ進化情報
        public MasterDataSkillActiveDiff master_array_skill_active;           //!< マスターデータ実体：スキル関連：アクティブスキル
        public MasterDataSkillPassiveDiff master_array_skill_passive;         //!< マスターデータ実体：スキル関連：パッシブスキル
        public MasterDataSkillLimitBreakDiff master_array_skill_limitbreak;       //!< マスターデータ実体：スキル関連：リミットブレイクスキル
        public MasterDataSkillLeaderDiff master_array_skill_leader;           //!< マスターデータ実体：スキル関連：リーダースキル
        public MasterDataSkillBoostDiff master_array_skill_boost;         //!< マスターデータ実体：スキル関連：ブーストスキル
        public MasterDataParamEnemyDiff master_array_enemy;                   //!< マスターデータ実体：単体パラメータ関連：エネミー
        public MasterDataEnemyAbilityDiff master_array_enemy_ability;         //!< マスターデータ実体：敵特性
        public MasterDataEnemyActionTableDiff master_array_enemy_action_table;    //!< マスターデータ実体：敵行動パターン定義
        public MasterDataEnemyActionParamDiff master_array_enemy_action_param;    //!< マスターデータ実体：敵行動定義
        public MasterDataQuestRequirementDiff master_array_quest_requirement;     //!< マスターデータ実体：クエスト関連：クエスト入場条件
        public MasterDataTextDefinitionDiff master_array_text_definition;     //!< マスターデータ実体：テキスト定義情報
        public MasterDataTopPageDiff master_array_toppage;                //!< マスターデータ実体：トップページ定義
        public MasterDataAudioDataDiff master_array_audiodata;                //!< マスターデータ実体：オーディオ再生情報
        public MasterDataGachaTicketDiff master_array_gachaticket;            //!< マスターデータ実体：ガチャチケット
        public MasterDataQuestKeyDiff master_array_quest_key;             //!< マスターデータ実体：クエストキー
        public MasterDataRegionDiff master_array_region;                //!< マスターデータ実体：リージョン
        public MasterDataStepUpGachaDiff master_array_step_up_gacha;              //!< マスターデータ実体：ステップアップガチャ情報
        public MasterDataStepUpGachaManageDiff master_array_step_up_gacha_manage; //!< マスターデータ実体：ステップアップガチャ管理情報

        //----------------------------------------------------
        // GetMaster以外のAPIで最新取得を行うもの
        // ※サーバー側がNull送るでも定義無しでも対応できるように変数定義だけは置いておく。
        //----------------------------------------------------
        public MasterDataStoreProductEventDiff master_array_store_product_event;  //!< マスターデータ実体：ストア商品イベント					→ ストアを開いたタイミングで最新取得
        public MasterDataHeroDiff master_array_hero;  //!<
        public MasterDataHeroAddEffectRateDiff master_array_hero_add_effect_rate;  //!<
        public MasterDataHeroLevelDiff master_array_hero_level;  //!<
        public MasterDataLoginMonthlyDiff master_array_login_monthly;         //!< マスターデータ実体：ログインボーナス関連：月間ログイン → ログインパックAPIで最新取得

        //----------------------------------------------------
        // そもそもクライアントからデータを隠蔽するもの or 渡す必要のないもの
        // ※サーバー側がNull送るでも定義無しでも対応できるように変数定義だけは置いておく。
        //----------------------------------------------------
        public MasterDataGachaAssignDiff master_array_gacha_assign;           //!< マスターデータ実体：ガチャアサイン
        public MasterDataLoginEventDiff master_array_login_event;         //!< マスターデータ実体：ログインボーナス関連：期間限定ログイン
        public MasterDataCategoryPatternDiff master_array_category_pattern;       //!< マスターデータ実体：クエスト関連：階層内ランダムカテゴリ分布パターン
        public MasterDataExpectPatternDiff master_array_expect_pattern;       //!< マスターデータ実体：クエスト関連：階層内期待値分布パターン

        //----------------------------------------------------
        // 仕様廃止で要らなくなったもの
        // ※サーバー側がNull送るでも定義無しでも対応できるように変数定義だけは置いておく。
        //----------------------------------------------------
        public MasterDataLoginChainDiff master_array_login_chain;         //!< マスターデータ実体：ログインボーナス関連：連続ログイン
        public MasterDataLoginTotalDiff master_array_login_total;         //!< マスターデータ実体：ログインボーナス関連：通算ログイン

        public MasterDataQuest2Diff master_array_renew_quest;            //!< マスターデータ実体：単体パラメータ関連：パネル効果グループ
        public MasterDataStoryDiff master_array_story;
        public MasterDataNpcDiff master_array_npc;
        public MasterDataStoryCharaDiff master_array_story_chara;

        public MasterDataIllustratorDiff master_array_illustrator;
        public MasterDataWebViewDiff master_array_webview;
        public MasterDataRenewQuestScoreDiff master_array_renew_quest_score;    //!< マスターデータ実体：クエストスコア
        public MasterDataPlayScoreDiff master_array_play_score;                 //!< マスターデータ実体：プレイスコア
        public MasterDataScoreEventDiff master_array_score_event;               //!< マスターデータ実体：イベントスコア
        public MasterDataScoreRewardDiff master_array_score_reward;             //!< マスターデータ実体：スコア報酬
        public MasterDataQuestAppearanceDiff master_array_quest_appearance;     //!< マスターデータ実体：クエスト演出差し替え情報

        public MasterDataChallengeQuestDiff master_array_challenge_quest;       //!< マスターデータ実体：成長ボスクエスト
        public MasterDataChallengeEventDiff master_array_challenge_event;       //!< マスターデータ実体：成長ボスイベント
        public MasterDataChallengeRewardDiff master_array_challenge_reward;     //!< マスターデータ実体：成長ボス報酬

        public MasterDataGachaTextDiff master_array_gacha_text;     //!< マスターデータ実体：ガチャテキスト管理
        public MasterDataGachaTextRefDiff master_array_gacha_text_ref;     //!< マスターデータ実体：ガチャテキスト参照管理
        public MasterDataPresentGroupDiff master_array_present_group;   //!< マスターデータ実体：プレゼントグループ
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターデータ情報取得（全種類）：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvMasterDataAllValue
    {
        public PacketStructMasterHash[] master_hash;                        //!< マスターデータハッシュ

        //----------------------------------------------------
        // もともとAssetBundleがある時からGetMasterでサーバー取得していたもの
        //----------------------------------------------------
        public MasterDataAssetBundlePath[] master_array_asset_path;         //!< マスターデータ実体：AssetBundleパス
        public MasterDataInformation[] master_array_information;            //!< マスターデータ実体：運営のお知らせ				※公開タイミングの概念あり
        public MasterDataGacha[] master_array_gacha;                    //!< マスターデータ実体：ガチャ関連：ガチャ定義		※公開タイミングの概念あり
        public MasterDataGachaGroup[] master_array_gacha_group;                    //!< マスターデータ実体：ガチャ関連：ガチャ定義グループ		※公開タイミングの概念あり
        public MasterDataPresent[] master_array_present;                //!< マスターデータ実体：プレゼント定義
        public MasterDataAchievementConverted[] master_array_achievement;           //!< マスターデータ実体：アチーブメント定義			※公開タイミングの概念あり
        public MasterDataEvent[] master_array_event;                    //!< マスターデータ実体：期間限定イベント
        public MasterDataNotification[] master_array_notification;          //!< マスターデータ実体：プッシュ通知

        //----------------------------------------------------
        // 間引きとかせずに普通に渡してもらうもの
        //----------------------------------------------------
        public MasterDataUserRank[] master_array_rank;                      //!< マスターデータ実体：ユーザー関連：ランク
        public MasterDataDefaultParty[] master_array_default_party;         //!< マスターデータ実体：デフォルト関連：パーティ編成
        public MasterDataStoreProduct[] master_array_store_product;         //!< マスターデータ実体：ストア商品
        public MasterDataAreaAmend[] master_array_area_amend;               //!< マスターデータ実体：期間限定エリア補正
        public MasterDataBeginnerBoost[] master_array_begginer_boost;       //!< マスターデータ実体：初心者ブースト
        public MasterDataGuerrillaBoss[] master_array_guerrilla_boss;       //!< マスターデータ実体：ゲリラボス
        public MasterDataStatusAilmentParam[] master_array_status_ailment;  //!< マスターデータ実体：状態異常整理用定義
        public MasterDataEnemyGroup[] master_array_enemy_group;             //!< マスターデータ実体：単体パラメータ関連：エネミーグループ
        public MasterDataAreaCategory[] master_array_area_category;         //!< マスターデータ実体：クエスト図鑑関連：エリアカテゴリ情報
        public MasterDataLinkSystem[] master_array_link_system;             //!< マスターデータ実体：リンクシステム
        public MasterDataLimitOver[] master_array_limit_over;               //!< マスターデータ実体：限界突破情報
        public MasterDataUseItem[] master_array_item;                       //!< マスターデータ実体：消費アイテム
        public MasterDataGlobalParams[] master_array_global_params;         //!< マスターデータ実体：共通定義
        public MasterDataGeneralWindow[] master_array_general_window;       //!< マスターデータ実体：汎用ウィンドウ

        //----------------------------------------------------
        // 一般公開タイミングを考慮しつつ渡してもらうもの
        //----------------------------------------------------
        public MasterDataArea[] master_array_area;                  //!< マスターデータ実体：クエスト関連：エリア情報
                                                                    // TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
                                                                    //public MasterDataQuest[] master_array_quest;                    //!< マスターデータ実体：クエスト関連：クエスト情報
        public MasterDataParamChara[] master_array_chara;                   //!< マスターデータ実体：単体パラメータ関連：キャラ
        public MasterDataParamCharaEvol[] master_array_chara_evol;          //!< マスターデータ実体：単体パラメータ関連：キャラ進化情報
        public MasterDataSkillActive[] master_array_skill_active;           //!< マスターデータ実体：スキル関連：アクティブスキル
        public MasterDataSkillPassive[] master_array_skill_passive;         //!< マスターデータ実体：スキル関連：パッシブスキル
        public MasterDataSkillLimitBreak[] master_array_skill_limitbreak;       //!< マスターデータ実体：スキル関連：リミットブレイクスキル
        public MasterDataSkillLeader[] master_array_skill_leader;           //!< マスターデータ実体：スキル関連：リーダースキル
        public MasterDataSkillBoost[] master_array_skill_boost;         //!< マスターデータ実体：スキル関連：ブーストスキル
        public MasterDataParamEnemy[] master_array_enemy;                   //!< マスターデータ実体：単体パラメータ関連：エネミー
        public MasterDataEnemyAbility[] master_array_enemy_ability;         //!< マスターデータ実体：敵特性
        public MasterDataEnemyActionTable[] master_array_enemy_action_table;    //!< マスターデータ実体：敵行動パターン定義
        public MasterDataEnemyActionParam[] master_array_enemy_action_param;    //!< マスターデータ実体：敵行動定義
        public MasterDataQuestRequirement[] master_array_quest_requirement;     //!< マスターデータ実体：クエスト関連：クエスト入場条件
        public MasterDataTextDefinition[] master_array_text_definition;     //!< マスターデータ実体：テキスト定義情報
        public MasterDataTopPage[] master_array_toppage;                //!< マスターデータ実体：トップページ定義
        public MasterDataAudioData[] master_array_audiodata;                //!< マスターデータ実体：オーディオ再生情報
        public MasterDataGachaTicket[] master_array_gachaticket;            //!< マスターデータ実体：ガチャチケット
        public MasterDataQuestKey[] master_array_quest_key;             //!< マスターデータ実体：クエストキー
        public MasterDataStepUpGacha[] master_array_step_up_gacha;              //!< マスターデータ実体：ステップアップガチャ情報
        public MasterDataStepUpGachaManage[] master_array_step_up_gacha_manage; //!< マスターデータ実体：ステップアップガチャ管理情報

        //----------------------------------------------------
        // GetMaster以外のAPIで最新取得を行うもの
        // ※サーバー側がNull送るでも定義無しでも対応できるように変数定義だけは置いておく。
        //----------------------------------------------------
        public MasterDataStoreProductEvent[] master_array_store_product_event;  //!< マスターデータ実体：ストア商品イベント					→ ストアを開いたタイミングで最新取得
        public MasterDataHero[] master_array_hero;                           //!<
        public MasterDataHeroAddEffectRate[] master_array_hero_add_effect_rate;  //!<
        public MasterDataHeroLevel[] master_array_hero_level;                //!<
        public MasterDataLoginMonthly[] master_array_login_monthly;         //!< マスターデータ実体：ログインボーナス関連：月間ログイン → ログインパックAPIで最新取得

        //----------------------------------------------------
        // そもそもクライアントからデータを隠蔽するもの or 渡す必要のないもの
        // ※サーバー側がNull送るでも定義無しでも対応できるように変数定義だけは置いておく。
        //----------------------------------------------------
        public MasterDataGachaAssign[] master_array_gacha_assign;           //!< マスターデータ実体：ガチャアサイン
        public MasterDataLoginEvent[] master_array_login_event;         //!< マスターデータ実体：ログインボーナス関連：期間限定ログイン
        public MasterDataCategoryPattern[] master_array_category_pattern;       //!< マスターデータ実体：クエスト関連：階層内ランダムカテゴリ分布パターン
        public MasterDataExpectPattern[] master_array_expect_pattern;       //!< マスターデータ実体：クエスト関連：階層内期待値分布パターン

        //----------------------------------------------------
        // 仕様廃止で要らなくなったもの
        // ※サーバー側がNull送るでも定義無しでも対応できるように変数定義だけは置いておく。
        //----------------------------------------------------
        public MasterDataLoginChain[] master_array_login_chain;         //!< マスターデータ実体：ログインボーナス関連：連続ログイン
        public MasterDataLoginTotal[] master_array_login_total;         //!< マスターデータ実体：ログインボーナス関連：通算ログイン


        public MasterDataStory[] master_array_story;                    //!<
        public MasterDataStoryChara[] master_array_story_chara;         //!<

        public MasterDataIllustrator[] master_array_illustrator;            //!<
        public MasterDataWebView[] master_array_webview;
        public MasterDataRenewQuestScore[] master_array_renew_quest_score;  //!< マスターデータ実体：クエストスコア
        public MasterDataPlayScore[] master_array_play_score;               //!< マスターデータ実体：プレイスコア
        public MasterDataScoreEvent[] master_array_score_event;             //!< マスターデータ実体：イベントスコア
        public MasterDataScoreReward[] master_array_score_reward;           //!< マスターデータ実体：スコア報酬
        public MasterDataQuestAppearance[] master_array_quest_appearance;   //!< マスターデータ実体：クエスト演出差し替え情報

        public MasterDataChallengeQuest[] master_array_challenge_quest;     //!< マスターデータ実体：成長ボスクエスト
        public MasterDataChallengeEvent[] master_array_challenge_event;     //!< マスターデータ実体：成長ボスイベント
        public MasterDataChallengeReward[] master_array_challenge_reward;   //!< マスターデータ実体：成長ボス報酬

        public MasterDataGachaText[] master_array_gacha_text;   //!< マスターデータ実体：ガチャテキスト管理
        public MasterDataGachaTextRef[] master_array_gacha_text_ref;   //!< マスターデータ実体：ガチャテキスト参照管理
        public MasterDataPresentGroup[] master_array_present_group;   //!< マスターデータ実体：プレゼントグループ
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターデータ情報取得（アチーブメントグループ）
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターデータ情報取得（アチーブメントグループ）：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendAchievementGroup
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint[] achievement_viewed;       //!< 送信情報：アチーブメント関連：演出確認済み	※MasterDataAchievement.fix_idと一致。クライアント側が演出済みなアチーブメント一覧を送る

        public uint achievement_gp_cate;        //!< 送信情報：アチーブメントグループ：カテゴリ
        public uint achievement_gp_page_no;     //!< 送信情報：アチーブメントグループ：ページ番号
        public uint achievement_gp_sort_type;   //!< 送信情報：アチーブメントグループ：ソート番号
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターデータ情報取得（アチーブメントグループ）：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvAchievementGroup : RecvDataBase
    {
        public PacketStructHeaderRecv header;       //!< 受信共通ヘッダ

        public RecvAchievementGroupValue result;        //!< 受信情報：情報実体
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターデータ情報取得（アチーブメントグループ）：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvAchievementGroupValue
    {
        public PacketStructAchievementGroup[] achievement_gp_list;  //!< 受信実データ：アチーブメントグループ定義			※公開タイミングの概念あり
        public uint achievement_gp_all_num; //!< 受信実データ：アチーブメントグループリスト総数
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターデータ情報取得（アチーブメント）
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターデータ情報取得（アチーブメント）：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGetMasterDataAchievement
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint[] achievement_viewed;       //!< 送信情報：アチーブメント関連：演出確認済み	※MasterDataAchievement.fix_idと一致。クライアント側が演出済みなアチーブメント一覧を送る

        //public	uint					achievement_cate;		//!< 送信情報：アチーブメント：リストカテゴリ
        public uint achievement_gp_id;          //!< 送信情報：アチーブメント：アチーブメントグループID
        public uint achievement_page_no;        //!< 送信情報：アチーブメント：ページ番号
        public uint achievement_sort_type;      //!< 送信情報：アチーブメント：ソート番号
        public uint quest_id;                   //!< 送信情報：アチーブメント：クエストＩＤ
        public uint filter;                     //!< 送信情報：アチーブメント：フィルタ設定
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターデータ情報取得（アチーブメント）：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvMasterDataAchievement : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ

        public RecvMasterDataAchievementValue result;           //!< マスターデータ実体：アチーブメント定義			※公開タイミングの概念あり
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターデータ情報取得（アチーブメント）：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvMasterDataAchievementValue
    {
        public uint achievement_all_num;                //!< アチーブメント総数
        public RecvAchievementCategoryClearNumValue achievement_category_clear_num; //!<  各（ノーマル、イベント、デイリー）の達成件数。clear_num_{$achievement_category_id}: 達成件数。 各ミッションカテゴリに1件もない場合は空の配列
        public MasterDataAchievementConverted[] master_array_achievement;           //!< ミッションデータ(最大30件)			※公開タイミングの概念あり
        public MasterDataAchievementConverted[] clear_array_achievement;           //!< 達成(ミッション報酬受取り済み)データ(最大30件)
        public uint achievement_gp_cate;                //!< アチーブメント：カテゴリ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：各（ノーマル、イベント、デイリー）の達成件数）：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvAchievementCategoryClearNumValue
    {
        public int clear_num_1;                //!< ノーマルカテゴリ達成件数
        public int clear_num_2;                //!< イベントカテゴリ達成件数
        public int clear_num_3;                //!< デイリーカテゴリ達成件数
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ストアイベント一覧データ取得
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ストアイベント一覧データ取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGetStoreProductEvent
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ストアイベント一覧データ取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetStoreProductEvent : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvGetStoreProductEventValue result;                //!< 受信実データ

    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ストアイベント一覧データ取得：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetStoreProductPlayerValue
    {
        public uint pay_month;                              //!< 月間課金額
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ストアイベント一覧データ取得：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetStoreProductEventValue
    {
        public RecvGetStoreProductPlayerValue player;   //!< プレイヤー情報（月間課金額のみ）
        public MasterDataStoreProductEvent[] store_product_event;   //!< マスターデータ実体：イベントストア
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ログイン時パケット統合情報：

		@note	ログインタイミングで複数のAPIが呼ばれてたので、統合して１つにしたもの。
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ログイン時パケット統合情報：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendLoginPack
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ
        public int get_login;       //!< 情報取得指定：ログイン情報
        public int get_friend;      //!< 情報取得指定：フレンド情報
        public int get_helper;      //!< 情報取得指定：助っ人情報
        public int get_present; //!< 情報取得指定：プレゼント取得情報
                                //		public	int						get_monthly;	//!< 情報取得指定：月間ログインボーナス情報
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ログイン時パケット統合情報：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvLoginPack : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvLoginPackValue result;           //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ログイン時パケット統合情報：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvLoginPackValue
    {
        public RecvLoginParamValue result_login_param;      //!< 受信実データ：
        public RecvFriendListGetValue result_friend;            //!< 受信実データ：
        public RecvQuestHelperGetValue result_helper;           //!< 受信実データ：
        public RecvPresentListGetValue result_present;          //!< 受信実データ：
        public PacketStructLoginMonthly result_monthly;         //!< 受信実データ：月間ログインボーナス
        public PacketStructPeriodLogin[] result_period_login;	//!< 受信実データ：期間限定ログインボーナス
        public string period_login_resource_root;               //!< 受信実データ：期間限定ログインボーナスの画像が格納されているディレクトリ
        public PacketStructOthersPresent others_present;        //!< 受信実データ：

        public int lunch_st;                //!< ランチタイム時間制限：開始時間　※時分のみ（1130）
        public int lunch_end;               //!< ランチタイム時間制限：終了時間　※時分のみ（1330）

        public PacketStructGachaStatus[] gacha_status;          //!< ガチャ情報：
        public RecvFlagValue new_notice_flags;                 //!< フラグ情報
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ログイン時パケット統合情報：フラグデータ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvFlagValue
    {
        public int achievement_normal;
        public int achievement_event;
        public int achievement_daily;
        public int friend;
        public int present;
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ログイン情報：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ログイン情報：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendLoginParam
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ログイン情報：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvLoginParam : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvLoginParamValue result;          //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ログイン情報：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvLoginParamValue
    {
        public uint login_total;            //!< ログイン：総合ログイン日数
        public uint login_chain;            //!< ログイン：連続ログイン日数
        public PacketStructLoginBonus[] login_bonus;            //!< ログイン：ログインボーナス		※効果の重複発生を考慮して配列化

        public uint friend_point_now;       //!< フレンド：フレンドポイント：現在値
        public uint friend_point_get;       //!< フレンド：フレンドポイント：今回取得分
        public uint friend_help_ct;         //!< フレンド：助っ人として助けた人数

        //------------------------------------------------
        // ※※※※※※※※※※※※※※※※※※※※※※
        // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
        // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
        // ※※※※※※※※※※※※※※※※※※※※※※
        //------------------------------------------------
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
        @brief	通信パケット：フレンド一覧取得：
    */
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
        @brief	通信パケット：フレンド一覧取得：送信
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendFriendListGet
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド一覧取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvFriendListGet : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvFriendListGetValue result;           //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド一覧取得：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvFriendListGetValue
    {
        public PacketStructFriend[] friend;         //!< 受信情報：フレンド一覧
    }

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド申請：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド申請：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendFriendRequest
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ

        public uint[] user_id;      //!< フレンドにしたいユーザーID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド申請：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvFriendRequest : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvFriendRequestValue result;           //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド申請：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvFriendRequestValue
    {
        public PacketStructFriend[] friend;     //!< 受信情報：フレンド一覧
    };


    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド申請承認：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド申請承認：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendFriendConsent
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ

        public uint[] user_id;      //!< フレンドユーザーID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド申請承認：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvFriendConsent : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvFriendConsentValue result;           //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド申請承認：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvFriendConsentValue
    {
        public PacketStructFriend[] friend;         //!< 受信情報：フレンド一覧
    }


    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド解除：
		@note	以下の動作も兼ねる→[ フレンド申請拒否 , 申請取り下げ ]
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド解除：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendFriendRefusal
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ

        public uint[] user_id;      //!< フレンドユーザーID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド解除：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvFriendRefusal : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvFriendRefusalValue result;           //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド解除：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvFriendRefusalValue
    {
        public PacketStructFriend[] friend;         //!< 受信情報：フレンド一覧
    }


    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド検索：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド検索：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendFriendSearch
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ

        public uint user_id;        //!< フレンドユーザーID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド検索：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvFriendSearch : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvFriendSearchValue result;            //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド検索：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvFriendSearchValue
    {
        public PacketStructFriend friend;           //!< 受信情報：フレンド一覧
    }

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：パーティ編集通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：パーティ編集通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendUnitPartyAssign
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ

        public PacketStructPartyAssign[] party_assign;  //!< パーティアサイン
        public int party_current;   //!< 選択しているパーティ番号
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：パーティ編集通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvUnitPartyAssign : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvUnitPartyAssignValue result;         //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：パーティ編集通知：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvUnitPartyAssignValue
    {
        public PacketStructPlayer player;           //!< 受信情報：プレイヤー情報
    }

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット売却：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット売却：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendUnitSale
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public long[] unit_unique_id;       //!< ユニット情報：ユニークID	※ユーザーが持つユニットのユニークID。
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット売却：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvUnitSale : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvUnitSaleValue result;            //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット売却：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvUnitSaleValue
    {
        public PacketStructPlayer player;           //!< 受信情報：プレイヤー情報
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット強化合成：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット強化合成：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendUnitBlendBuildUp
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public long unique_id_base;     //!< 強化合成情報：ユニークID：ベース	※ユーザーが持つユニットのユニークID。
        public long[] unique_id_parts;  //!< 強化合成情報：ユニークID：素材		※ユーザーが持つユニットのユニークID。
        public uint event_skilllv;      //!< 関連イベント開催情報：スキルレベルアップ確率上昇イベント
        public int beginner_boost_id;   //!< 初心者ブーストFixID
        public uint buildup_tutorial;   //!< チュートリアルフラグ	※[ 0:チュートリアルでない ][ 1:チュートリアルである ]
        public bool is_premium;          //!< 助っ人情報：プレミアムフラグ            ※[ 0:プレミアムでない ][ 1:プレミアムである ]
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット強化合成：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvUnitBlendBuildUp : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvUnitBlendBuildUpValue result;            //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット強化合成：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvUnitBlendBuildUpValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報

        public uint blend_pattern;      //!< 受信情報：成功パターン	※ [ 0:成功 ][ 1:大成功 ][ 2:超成功 ]
        public uint blend_exp;          //!< 受信情報：経験値合計
        public long blend_unit_unique;  //!< 受信情報：合成後ユニットユニークID
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニットリンク：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニットリンク実行：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendUnitLinkCreate
    {
        public PacketStructHeaderSend header;                   //!< 送信共通ヘッダ

        public long unique_id_base;         //!< リンク情報：ユニークID：ベース	※ユーザーが持つユニットのユニークID
        public long unique_id_link;         //!< リンク情報：ユニークID：リンク	※ユーザーが持つユニットのユニークID
        public long[] unique_id_parts;      //!< リンク情報：ユニークID：素材	※ユーザーが持つユニットのユニークID

        public int beginner_boost_id;       //!< 初心者ブーストFixID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニットリンク解除：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendUnitLinkDelete
    {
        public PacketStructHeaderSend header;                   //!< 送信共通ヘッダ

        public long unique_id_base;         //!< リンク情報：ユニークID：ベース	※ユーザーが持つユニットのユニークID
        public long[] unique_id_parts;      //!< リンク情報：ユニークID：素材	※ユーザーが持つユニットのユニークID

        public int beginner_boost_id;       //!< 初心者ブーストFixID
    };
    //----------------------------------------------------------------------------
    /*
		@brief	通信パケット：ユニットリンク：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvUnitLink : RecvDataBase
    {
        public PacketStructHeaderRecv header;                   //!< 受信共通ヘッダ
        public RecvUnitLinkValue result;                    //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニットリンク：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvUnitLinkValue
    {
        public PacketStructPlayer player;                   //!< 受信情報：プレイヤー情報
    };


    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：助っ人一覧取得：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：助っ人一覧取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuestHelperGet
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：助っ人一覧取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestHelperGet : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuestHelperGetValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：助っ人一覧取得：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestHelperGetValue
    {
        public PacketStructFriend[] friend;             //!< 受信情報：助っ人一覧
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：助っ人一覧取得（進化合成向け）：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：助っ人一覧取得（進化合成向け）：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuestHelperGetEvol
    {
        public PacketStructHeaderSend header;                       //!< 送信共通ヘッダ

        public long blend_base_unique_id;       //!< 進化合成情報：ユニットユニークID
        public uint blend_pattern;              //!< 進化合成情報：進化パターンID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：助っ人一覧取得（進化合成向け）：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestHelperGetEvol : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuestHelperGetEvolValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：助っ人一覧取得（進化合成向け）：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestHelperGetEvolValue
    {
        public PacketStructFriend[] friend;             //!< 受信情報：助っ人一覧
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：助っ人一覧取得（強化合成向け）：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：助っ人一覧取得（強化合成向け）：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuestHelperGetBuild
    {
        public PacketStructHeaderSend header;                       //!< 送信共通ヘッダ

        public long blend_base_unique_id;       //!< 進化合成情報：ユニットユニークID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：助っ人一覧取得（強化合成向け）：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestHelperGetBuild : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuestHelperGetBuildValue result;             //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：助っ人一覧取得（強化合成向け）：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestHelperGetBuildValue
    {
        public PacketStructFriend[] friend;             //!< 受信情報：助っ人一覧
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエスト開始通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエスト開始通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuestStart
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint quest_id;           //!< クエスト情報：クエストID
        public uint quest_state;        //!< クエスト情報：補正情報 ※20160328 サーバー側未使用

        public uint helper_user_id;     //!< 助っ人情報：ユーザーID
        public PacketStructUnit helper_unit;        //!< 助っ人情報：ユニット
        public int helper_point_ok; //!< 助っ人情報：フレンドポイントの付与有無	※[ 0:フレンドポイントを付与しない ][ 1:フレンドポイントを付与 ]

        public int party_current;       //!< 選択しているパーティ番号

        public uint[] all_unit_kind;        //!< 出現する可能性のあるユニットID

        //		public	uint[]						event_list;			//!< 関連イベント開催情報：
        public uint event_friendpoint;  //!< 関連イベント開催情報：フレンドポイント増加イベント

        public int beginner_boost_id;   //!< 初心者ブーストFixID

        public uint[] area_amend_id;        //!< エリア補正IDリスト
        public PacketStructUnit helper_unit_link;   //!< 助っ人情報：リンクユニット ※開発ユーザーの場合のみ使用(CHARM値反映のため)
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエスト開始通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestStart : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuestStartValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエスト開始通知：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestStartValue
    {
        public uint stamina_now;        //!< スタミナ：現在値
        public ulong stamina_recover;   //!< スタミナ：回復時刻
        public uint ticket_now;         //!< チケット：現在値

        public PacketStructQuestBuild quest;    //!< クエスト構成詳細情報	※ドロップ隠蔽対応でのサーバー側作成フロア情報
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストクリア通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストクリア通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuestClear
    {
        public PacketStructHeaderSend header;                   //!< 送信共通ヘッダ

        public uint quest_id;               //!< クエスト情報：クエストID

        public uint get_money;              //!< 取得物情報：お金
        public uint get_exp;                //!< 取得物情報：経験値
        public PacketStructUnitGet[] get_unit;              //!< 取得物情報：ユニット

        public uint security_key;           //!< チート防止用のキー情報

        public int[] panel_unique;          //!< 開いたパネル一覧	※ 1.0.8.0以降対応


        public uint achievement_combo;      //!< アチーブメント：コンボ数
        public uint achievement_damage;     //!< アチーブメント：ダメージ値

    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストクリア通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestClear : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuestClearValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストクリア通知：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestClearValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報

        public uint get_money;          //!< 取得物情報：お金
        public uint get_exp;            //!< 取得物情報：経験値
        public long[] get_unit;         //!< 取得物情報：ユニットユニークID
        public uint get_ticket;         //!< 取得物情報：チケット

        public uint get_stone;          //!< エリア報酬：魔法石

        public long get_clear_unit;     //!< クエストクリア報酬：ユニット付与
        public uint get_clear_item;     //!< クエストクリア報酬：アイテム付与
        public uint get_clear_key;      //!< クエストクリア報酬：クエストキー付与

        public PacketStructFloorBonus[] get_bonus;      //!< 取得物情報：フロアボーナス ユニットユニークID
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストリタイア通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストリタイア通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuestRetire
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint quest_id;           //!< クエスト情報：クエストID
        public bool is_auto_play;   //!< オートプレイを使用したかどうか
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストリタイア通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestRetire : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuestRetireValue result;             //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストリタイア通知：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestRetireValue
    {
        public PacketStructPlayer player;
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエスト受注情報取得：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエスト受注情報取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuestOrderGet
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
        public int detail_flag;     //!< クエスト構成詳細情報の取得有無
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエスト受注情報取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestOrderGet : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuestOrderGetValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエスト受注情報取得：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestOrderGetValue
    {
        public uint quest_id;           //!< クエスト受注情報：クエストID
        public uint continue_ct;        //!< クエスト受注情報：コンティニュー回数
        public uint reset_ct;           //!< クエスト受注情報：リセット回数

        public PacketStructQuestBuild quest;                //!< クエスト構成詳細情報	※ドロップ隠蔽対応でのサーバー側作成フロア情報
    };


    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストコンティニュー通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストコンティニュー通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuestContinue
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint continue_ct;        //!< コンティニュー回数
        public bool is_auto_play;   //!< オートプレイを使用したかどうか
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストコンティニュー通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestContinue : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuestContinueValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストコンティニュー通知：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestContinueValue
    {
        public int dummy;               //!< ダミー

        public PacketStructQuestBuild quest;                //!< クエスト構成詳細情報	※ドロップ隠蔽対応でのサーバー側作成フロア情報
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストリセット通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストリセット通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuestReset
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint reset_ct;           //!< リセット回数
        public int floor;               //!< リセットしたフロア
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストリセット通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestReset : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuestResetValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：クエストリセット通知：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuestResetValue
    {
        public int dummy;               //!< ダミー

        public PacketStructQuestBuild quest;                //!< クエスト構成詳細情報	※ドロップ隠蔽対応でのサーバー側作成フロア情報
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：進化クエスト開始通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：進化クエスト開始通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendEvolQuestStart
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint quest_id;           //!< クエスト情報：クエストID
        public uint quest_state;        //!< クエスト情報：補正情報 ※20160328 サーバー側未使用

        public long unique_id_base;     //!< 進化対象ユニットユニークID
        public long[] unique_id_parts;  //!< 進化素材ユニットユニークID

        public uint blend_unit_id;      //!< 進化合成情報：進化後キャラID
        public uint blend_pattern;      //!< 進化合成情報：進化パターンID

        public uint helper_user_id;     //!< 助っ人情報：ユーザーID
        public PacketStructUnit helper_unit;        //!< 助っ人情報：ユニット
        public uint helper_premium;     //!< 助っ人情報：プレミアムフラグ			※[ 0:プレミアムでない ][ 1:プレミアムである ]
        public int helper_point_ok; //!< 助っ人情報：フレンドポイントの付与有無	※[ 0:フレンドポイントを付与しない ][ 1:フレンドポイントを付与 ]

        public uint[] all_unit_kind;        //!< 出現する可能性のあるユニットID

        public uint event_friendpoint;  //!< 関連イベント開催情報：フレンドポイント増加イベント

        public int beginner_boost_id;   //!< 初心者ブーストFixID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：進化クエスト開始通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvEvolQuestStart : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvEvolQuestStartValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：進化クエスト開始通知：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvEvolQuestStartValue
    {
        public uint stamina_now;        //!< スタミナ：現在値
        public ulong stamina_recover;   //!< スタミナ：回復時刻

        public PacketStructQuestBuild quest;                //!< クエスト構成詳細情報	※ドロップ隠蔽対応でのサーバー側作成フロア情報
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：進化クエストクリア通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：進化クエストクリア通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendEvolQuestClear
    {
        public PacketStructHeaderSend header;                   //!< 送信共通ヘッダ

        public uint quest_id;               //!< クエスト情報：クエストID

        public uint get_money;              //!< 取得物情報：お金
        public uint get_exp;                //!< 取得物情報：経験値
        public PacketStructUnitGet[] get_unit;              //!< 取得物情報：ユニット

        //		public	long						security_key;			//!< チート防止用のキー情報
        public uint security_key;           //!< チート防止用のキー情報

        public int[] panel_unique;          //!< 開いたパネル一覧	※ 1.0.8.0以降対応

        public uint achievement_combo;      //!< アチーブメント：コンボ数
        public uint achievement_damage;     //!< アチーブメント：ダメージ値

        public uint evol_tutorial;      //!< チュートリアルフラグ	※[ 0:チュートリアルでない ][ 1:チュートリアルである ]
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：進化クエストクリア通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvEvolQuestClear : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvEvolQuestClearValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：進化クエストクリア通知：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvEvolQuestClearValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報

        public uint get_money;          //!< 取得物情報：お金
        public uint get_exp;            //!< 取得物情報：経験値
        public long[] get_unit;         //!< 取得物情報：ユニットユニークID
        public uint get_ticket;         //!< 取得物情報：チケット

        public uint get_stone;          //!< エリア報酬：魔法石

        public long unit_unique;        //!< 合成後ユニットユニークID

        public PacketStructFloorBonus[] get_bonus;          //!< 取得物情報：フロアボーナス
    };

    #region === 新クエスト関連通信パケット ===
    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエスト開始通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエスト開始通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuest2Start
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint quest_id;                 //!< クエスト情報：クエストID

        public int helper_point_ok;           //!< 助っ人情報：フレンドポイントの付与有無	※[ 0:フレンドポイントを付与しない ][ 1:フレンドポイントを付与 ]
        public int beginner_boost_id;         //!< 初心者ブーストFixID
        public uint quest_state;              //!< クエスト情報：補正情報 ※20160328 サーバー側未使用
        public uint[] area_amend_id;          //!< エリア補正IDリスト
        public int party_current;             //!< 選択しているパーティ番号
        public uint helper_user_id;           //!< 助っ人情報：ユーザーID
        public uint event_friendpoint;        //!< 関連イベント開催情報：フレンドポイント増加イベント
        public PacketStructUnit helper_unit;  //!< 助っ人情報：ユニット
        public PacketStructUnit helper_unit_link;  //!< 助っ人情報：リンクユニット
        public bool is_auto_play;             //!< オートプレイを使用したかどうか
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエスト開始通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuest2Start : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuest2StartValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエスト開始通知：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuest2StartValue
    {
        public uint stamina_now;        //!< スタミナ：現在値
        public ulong stamina_recover;   //!< スタミナ：回復時刻
        public uint ticket_now;         //!< チケット：現在値

        public PacketStructQuest2Build quest;    //!< クエスト構成詳細情報	※ドロップ隠蔽対応でのサーバー側作成フロア情報
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエストクリア通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエストクリア通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuest2Clear
    {
        public PacketStructHeaderSend header;                   //!< 送信共通ヘッダ

        public uint quest_id;               //!< クエスト２クリア情報：クエストID
        public SendEnemyKill[] enemy_kill_list;       //!< クエスト２クリア情報：倒した敵リスト
        public bool no_damage;              //!< クエスト２クリア情報：ノーダメージフラグ
        public int max_damage;              //!< クエスト２クリア情報：最大ダメージ
        public SendSkillExecCount[] active_skill_execute_count;    //!< クエスト２クリア情報：スキル使用回数
        public int hero_skill_execute_count;        //!< クエスト２クリア情報：ヒーロースキル使用回数
        public PlayScoreInfo play_score_info;   //!< プレイスコア計算用情報
        public bool is_auto_play;   //!< オートプレイを使用したかどうか
    };

    [Serializable]
    public class SendEnemyKill
    {
        public uint enemy_id;
        public uint value;
    }

    [Serializable]
    public class SendSkillExecCount
    {
        public uint skill_id;
        public uint value;
    }

    [Serializable]
    public class PlayScoreInfo
    {
        public bool is_enable_score;    //スコアが有効かどうか（中断復帰していないかどうか）
        public int play_score;          //プレイスコア（(1)と(2)の合計にクエストスコア補正を掛けたもの）
        public uint check_info;          //チート対策用比較値（独自ハッシュ等？ 今はゼロ固定）
        public int[] battle_achieve_count;  //バトルアチーブ達成数（バトル毎）
        public int[] turn_count;    //経過ターン数（バトル毎）
        public int[] skill_info;    //スキル成立情報（通信容量削減＆難読化のためいくつかの数値をまとめる）（ターン毎）
                                    //内訳：１パネルスキルが成立したかどうか * 1
                                    //     + ２パネルスキルが成立したかどうか * 2
                                    //     + ３パネルスキルが成立したかどうか * 4
                                    //     + ４パネルスキルが成立したかどうか * 8
                                    //     + ５パネルスキルが成立したかどうか * 16
                                    //     + ハンズ数 * 32

        private MasterDataPlayScore[] m_MasterDataPlayScoreArray = null;
        private int m_QuestBaseScore = 0;
        private int m_QUestPlayScoreRate = 0;
        private bool m_IsPlayScoreQuest = false;    // プレイスコア対象のクエストかどうか

        // コンストラクタ
        public PlayScoreInfo()
        {
            is_enable_score = false;
            battle_achieve_count = new int[0];
            turn_count = new int[0];
            skill_info = new int[0];

            m_MasterDataPlayScoreArray = new MasterDataPlayScore[0];

            m_QuestBaseScore = 0;
            m_QUestPlayScoreRate = 0;
            m_IsPlayScoreQuest = false;
        }

        // プレイスコア計算対象クエストかどうかを設定
        public void setPlayScoreQuestFlag(bool is_play_score_quest)
        {
            m_IsPlayScoreQuest = is_play_score_quest;
        }

        // プレイスコア計算対象クエストかどうかを取得
        public bool isPlayScoreQuest()
        {
            return m_IsPlayScoreQuest;
        }

        // クエスト開始時の初期化
        public void init(MasterDataPlayScore[] master_data_play_score_array, int quest_base_score, int quest_play_score_rate)
        {
            is_enable_score = true;
            battle_achieve_count = new int[0];
            turn_count = new int[0];
            skill_info = new int[0];

            m_MasterDataPlayScoreArray = master_data_play_score_array;

            m_QuestBaseScore = quest_base_score;
            m_QUestPlayScoreRate = quest_play_score_rate;
        }

        // バトル終了毎に呼ぶ
        public void endBattle(int param_battle_achieve_count)
        {
            if (is_enable_score == false)
            {
                return;
            }

            int[] new_battle_achieve_count = new int[battle_achieve_count.Length + 1];
            int[] new_turn_count = new int[turn_count.Length + 1];

            int total_turn = 0;
            for (int idx = 0; idx < battle_achieve_count.Length; idx++)
            {
                new_battle_achieve_count[idx] = battle_achieve_count[idx];
                new_turn_count[idx] = turn_count[idx];
                total_turn += turn_count[idx];
            }

            new_battle_achieve_count[battle_achieve_count.Length] = param_battle_achieve_count;

            int now_battle_turn_count = skill_info.Length - total_turn;
            new_turn_count[turn_count.Length] = now_battle_turn_count;

            battle_achieve_count = new_battle_achieve_count;
            turn_count = new_turn_count;

            calcValue();
        }

        /// <summary>
        /// ノーマルスキルのスキルカットイン前に呼ぶ
        /// </summary>
        public void startTurn()
        {
            if (is_enable_score == false)
            {
                return;
            }

            int[] new_skill_info = new int[skill_info.Length + 1];

            for (int idx = 0; idx < skill_info.Length; idx++)
            {
                new_skill_info[idx] = skill_info[idx];
            }

            new_skill_info[skill_info.Length] = 0;

            skill_info = new_skill_info;
        }

        /// <summary>
        /// ノーマルスキルのスキルカットイン時に呼ぶ
        /// </summary>
        /// <param name="is_1panel"></param>
        /// <param name="is_2panel"></param>
        /// <param name="is_3panel"></param>
        /// <param name="is_4panel"></param>
        /// <param name="is_5panel"></param>
        /// <param name="add_hands"></param>
        public void addHand(bool is_1panel, bool is_2panel, bool is_3panel, bool is_4panel, bool is_5panel, int add_hands = 1)
        {
            if (is_enable_score == false)
            {
                return;
            }

            int val = skill_info[skill_info.Length - 1];
            int hands = val / 32;

            hands += add_hands;

            int new_val = (hands * 32)
                + ((is_1panel) ? 1 : 0)
                + ((is_2panel) ? 2 : 0)
                + ((is_3panel) ? 4 : 0)
                + ((is_4panel) ? 8 : 0)
                + ((is_5panel) ? 16 : 0);

            skill_info[skill_info.Length - 1] = new_val;

            calcValue();
        }

        /// <summary>
        /// 得点を計算しなおす
        /// </summary>
        public void calcValue()
        {
            play_score = _getScore();
            check_info = _getCheckInfo();
        }

        /// <summary>
        /// 得点を計算
        /// </summary>
        /// <returns></returns>
        private int _getScore()
        {
            int wrk_play_score = 0;

            // バトルアチーブの得点
            {
                int param_count = _getScoreParam(MasterDataDefineLabel.PlayScoreType.BATTLE_ACHIEVE_COUNT);
                int param_score = _getScoreParam(MasterDataDefineLabel.PlayScoreType.BATTLE_ACHIEVE_SCORE);

                if (param_count > 0 && param_score > 0)
                {
                    int total_achieve = 0;
                    for (int idx = 0; idx < battle_achieve_count.Length; idx++)
                    {
                        total_achieve += battle_achieve_count[idx];
                    }

                    int achieve_score = total_achieve / param_count;
                    achieve_score *= param_score;

                    wrk_play_score += achieve_score;
                }
            }

            // ハンズ数・成立パネル得点
            {
                int score_1panel = _getScoreParam(MasterDataDefineLabel.PlayScoreType.SKILL_1_PANEL_SCORE);
                int score_2panel = _getScoreParam(MasterDataDefineLabel.PlayScoreType.SKILL_2_PANEL_SCORE);
                int score_3panel = _getScoreParam(MasterDataDefineLabel.PlayScoreType.SKILL_3_PANEL_SCORE);
                int score_4panel = _getScoreParam(MasterDataDefineLabel.PlayScoreType.SKILL_4_PANEL_SCORE);
                int score_5panel = _getScoreParam(MasterDataDefineLabel.PlayScoreType.SKILL_5_PANEL_SCORE);

                for (int idx = 0; idx < skill_info.Length; idx++)
                {
                    int skill_info_value = skill_info[idx];
                    int hands = skill_info_value / 32;
                    int is_1panel = (skill_info_value / 1) & 1;
                    int is_2panel = (skill_info_value / 2) & 1;
                    int is_3panel = (skill_info_value / 4) & 1;
                    int is_4panel = (skill_info_value / 8) & 1;
                    int is_5panel = (skill_info_value / 16) & 1;

                    int score = hands *
                        (
                            (score_1panel * is_1panel)
                            + (score_2panel * is_2panel)
                            + (score_3panel * is_3panel)
                            + (score_4panel * is_4panel)
                            + (score_5panel * is_5panel)
                        );

                    wrk_play_score += score;
                }
            }

            // クエストスコア補正
            wrk_play_score = (int)(((long)wrk_play_score * m_QuestBaseScore * m_QUestPlayScoreRate) / 100);

            return wrk_play_score;
        }

        /// <summary>
        /// 得点計算用パラメータ取得
        /// </summary>
        /// <param name="play_score_type"></param>
        /// <returns></returns>
        private int _getScoreParam(MasterDataDefineLabel.PlayScoreType play_score_type)
        {
            int ret_val = 0;
            for (int idx = 0; idx < m_MasterDataPlayScoreArray.Length; idx++)
            {
                MasterDataPlayScore play_score = m_MasterDataPlayScoreArray[idx];
                if (play_score != null)
                {
                    if (play_score.score_type == play_score_type)
                    {
                        ret_val = play_score.score_param;
                        break;
                    }
                }
            }

            return ret_val;
        }

        /// <summary>
        /// チート対策用の何かの値
        /// </summary>
        /// <returns></returns>
        private uint _getCheckInfo()
        {
            return 0;
        }
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエストクリア通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuest2Clear : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuest2ClearValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエストクリア通知：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuest2ClearValue
    {
        public PacketStructHero[] hero_list;            //!< 受信情報：HEROリスト
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報

        public uint get_money;          //!< 取得物情報：お金
        public uint get_exp;            //!< 取得物情報：経験値
        public long[] get_unit;         //!< 取得物情報：ユニットユニークID
        public uint get_ticket;         //!< 取得物情報：チケット
        public uint get_friend_pt;      //!< 取得物情報：フレンドポイント

        public uint get_stone;          //!< エリア報酬：魔法石

        public long get_clear_unit;     //!< クエストクリア報酬：ユニット付与
        public uint get_clear_item;     //!< クエストクリア報酬：アイテム付与
        public uint get_clear_key;      //!< クエストクリア報酬：クエストキー付与
        public PacketStructFloorBonus[] get_bonus;      //!< 取得物情報：フロアボーナス ユニットユニークID

        public int[] reward_limit_list; //!< アチーブメント警告：報酬超過アチーブメントID

        public PacketStructResultScore[] result_scores;     //!< スコア情報
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエスト受注情報取得：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエスト受注情報取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuest2OrderGet
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
        public int detail_flag;     //!< クエスト構成詳細情報の取得有無
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエスト受注情報取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuest2OrderGet : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuest2OrderGetValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエスト受注情報取得：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuest2OrderGetValue
    {
        public uint quest_id;           //!< クエスト受注情報：クエストID
        public uint continue_ct;        //!< クエスト受注情報：コンティニュー回数
        public uint reset_ct;           //!< クエスト受注情報：リセット回数

        public PacketStructQuest2Build quest;                //!< クエスト構成詳細情報	※ドロップ隠蔽対応でのサーバー側作成フロア情報
    };

    /*==========================================================================*/
    /*        class                                                             */
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
        @brief  通信パケット：ストーリクエスト開始通知：
    */
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
        @brief  通信パケット：ストーリクエスト開始通知：送信
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuest2StoryStart
    {
        public PacketStructHeaderSend header; //!< 送信共通ヘッダ

        public int quest_id;                  //!< クエスト２クリア情報：クエストID
    };

    //----------------------------------------------------------------------------
    /*!
        @brief  通信パケット：ストーリクエスト開始通知：受信
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuest2StoryStart : RecvDataBase
    {
        public PacketStructHeaderRecv header;    //!< 受信共通ヘッダ
        public RecvQuest2StoryStartValue result; //!< 受信情報：情報実体    ※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
        @brief  通信パケット：ストーリクエスト開始通知：受信実データ
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuest2StoryStartValue
    {

    };

    /*==========================================================================*/
    /*        class                                                             */
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
        @brief  通信パケット：ストーリクエストクリア通知：
    */
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
        @brief  通信パケット：ストーリクエストクリア通知：送信
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuest2StoryClear
    {
        public PacketStructHeaderSend header; //!< 送信共通ヘッダ

        public int quest_id;                  //!< クエスト２クリア情報：クエストID
    };

    //----------------------------------------------------------------------------
    /*!
        @brief  通信パケット：ストーリクエストクリア通知：受信
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuest2StoryClear : RecvDataBase
    {
        public PacketStructHeaderRecv header;    //!< 受信共通ヘッダ
        public RecvQuest2StoryClearValue result; //!< 受信情報：情報実体    ※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
        @brief  通信パケット：ストーリクエストクリア通知：受信実データ
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuest2StoryClearValue
    {
        public PacketStructPlayer player;   //!< 受信情報：プレイヤー情報
        public int[] reward_limit_list; //!< アチーブメント警告：報酬超過アチーブメントID
    };

    /*==========================================================================*/
    /*        class                                                             */
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
        @brief  通信パケット：	新クエスト中断データ破棄通知：
    */
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
        @brief  通信パケット：	新クエスト中断データ破棄通知：送信
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendQuest2CessaionQuest
    {
        public PacketStructHeaderSend header; //!< 送信共通ヘッダ
        public bool is_auto_play;   //!< オートプレイを使用したかどうか
    };

    //----------------------------------------------------------------------------
    /*!
        @brief  通信パケット：	新クエスト中断データ破棄通知：受信
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuest2CessaionQuest : RecvDataBase
    {
        public PacketStructHeaderRecv header;    //!< 受信共通ヘッダ
        public RecvQuest2CessaionQuestValue result; //!< 受信情報：情報実体    ※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
        @brief  通信パケット：	新クエスト中断データ破棄通知：受信実データ
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvQuest2CessaionQuestValue
    {
    };

    #endregion

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：不正検出通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：不正検出通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendInjustice
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint[] injustice_flag;       //!< 不正ID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：不正検出通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvInjustice : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvInjusticeValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：不正検出通知：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvInjusticeValue
    {
        public int dummy;               //!< ダミー
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：チュートリ進行状況通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：チュートリ進行状況通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendTutorialStep
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint tutorial_step;      //!< チュートリアル進行番号
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：チュートリ進行状況通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvTutorialStep : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvTutorialStepValue result;                //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：チュートリ進行状況通知：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvTutorialStepValue
    {
        public int dummy;               //!< ダミー
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：リニュアルチュートリ進行状況通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：リニュアルチュートリ進行状況通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendRenewTutorialStep
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint tutorial_step;      //!< チュートリアル進行番号
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：チュートリ進行状況通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvRenewTutorialStep : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvRenewTutorialStepValue result;                //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：チュートリ進行状況通知：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvRenewTutorialStepValue
    {
        public int dummy;               //!< ダミー
    };





    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット枠増設
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット枠増設：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendStoneUsedUnit
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット枠増設：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStoneUsedUnit : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvStoneUsedUnitValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット枠増設：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStoneUsedUnitValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド枠増設
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド枠増設：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendStoneUsedFriend
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド枠増設：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStoneUsedFriend : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvStoneUsedFriendValue result;             //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：フレンド枠増設：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStoneUsedFriendValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：パーティコスト増設
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：パーティコスト増設：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendStoneUsedPartyCost
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：パーティコスト増設：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStoneUsedPartyCost
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvStoneUsedPartyCostValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：パーティコスト増設：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStoneUsedPartyCostValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：スタミナ回復
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：スタミナ回復：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendStoneUsedStamina
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：スタミナ回復：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStoneUsedStamina : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvStoneUsedStaminaValue result;                //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：スタミナ回復：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStoneUsedStaminaValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ガチャ実行：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ガチャ実行：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGachaPlay
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint gacha_id;           //!< ガチャ情報：ガチャID
        public uint gacha_tutorial;     //!< ガチャ情報：チュートリアルフラグ　※[ 0:チュートリアルでない ][ 1:チュートリアルである ]
        public uint gacha_ct;           //!< ガチャ情報：ガチャ連続回数
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ガチャ実行：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGachaPlay : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvGachaPlayValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ガチャ実行：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGachaPlayValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報

        public uint[] gacha_bonus_data;      //!< おまけプレゼントID

        public long[] unit_unique_id;       //!< 入手ユニットユニークID

        public uint[] blank_unit_id;        //!< ハズレユニット情報：キャラID

        public PacketStructGachaStatus[] gacha_status;      //!< ガチャ情報：

        public RecvPresentListGetValue result_present;          //!< 受信実データ：
    };



    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入直前処理( iOS ... AppStore )：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入直前処理( iOS ... AppStore )：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendStorePayPrev_ios
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public string product_id;           //!< 購入情報：製品ID
        public uint event_id;           //!< 購入情報：イベント固有ID
        public int age_type;            //!< 購入情報：年齢区分 [ 0:未設定 ][ 1:16歳未満 ][ 2:16歳～19歳 ][ 3:20歳以上 ]
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入直前処理( iOS ... AppStore )：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStorePayPrev_ios : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvStorePayPrev_ios_Value result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入直前処理( iOS ... AppStore )：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStorePayPrev_ios_Value
    {
        public uint dummy;              //!< ダミー
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入直前処理( Android ... GooglePlay )：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入直前処理( Android ... GooglePlay )：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendStorePayPrev_android
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public string product_id;           //!< 購入情報：製品ID
        public uint event_id;           //!< 購入情報：イベント固有ID
        public int age_type;            //!< 購入情報：年齢区分 [ 0:未設定 ][ 1:16歳未満 ][ 2:16歳～19歳 ][ 3:20歳以上 ]
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入直前処理( Android ... GooglePlay )：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStorePayPrev_android : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvStorePayPrev_android_Value result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入直前処理( Android ... GooglePlay )：受信実体
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStorePayPrev_android_Value
    {
        public uint dummy;              //!< ダミー
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入反映処理( iOS ... AppStore )：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入反映処理( iOS ... AppStore )：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendStorePay_ios
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public string receipt;          //!< レシート情報
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入反映処理( iOS ... AppStore )：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStorePay_ios : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvStorePayValue_ios result;                //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入反映処理( iOS ... AppStore )：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStorePayValue_ios
    {
        public uint stone_ct;                   //!< 手持ち魔法石総数
        public uint stone_ct_pay;               //!< 手持ち魔法石総数：有料
        public uint stone_ct_free;              //!< 手持ち魔法石総数：無料
        public uint pay_total;                  //!< 課金額：総額
        public uint pay_month;                  //!< 課金額：月間

        //		public	PacketStructEventPurchase[]	event_purchase_history;		//!< イベント用アイテム購入：イベント用アイテム購入履歴
        public PacketStructEventPurchase[] event_purchase;              //!< イベント用アイテム購入時情報

        public uint present_add;                //!< プレゼント付与情報[0:付与していない][1:付与している]
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入反映処理( Android ... GooglePlay )：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入反映処理( Android ... GooglePlay )：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendStorePay_android
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public string receipt;          //!< レシート情報
        public string receipt_sign;     //!< レシート情報：署名
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入反映処理( Android ... GooglePlay )：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStorePay_android : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvStorePayValue_android result;                //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入反映処理( Android ... GooglePlay )：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStorePayValue_android
    {
        public uint stone_ct;                   //!< 手持ち魔法石総数
        public uint stone_ct_pay;               //!< 手持ち魔法石総数：有料
        public uint stone_ct_free;              //!< 手持ち魔法石総数：無料
        public uint pay_total;                  //!< 課金額：総額
        public uint pay_month;                  //!< 課金額：月間

        //		public	PacketStructEventPurchase[]	event_purchase_history;		//!< イベント用アイテム購入：イベント用アイテム購入履歴
        public PacketStructEventPurchase[] event_purchase;              //!< イベント用アイテム購入時情報

        public uint present_add;                //!< プレゼント付与情報[0:付与していない][1:付与している]
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入キャンセル処理
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入キャンセル処理：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendStorePayCancel
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public string product_id;           //!< 購入情報：製品ID
        public uint event_id;           //!< 購入情報：イベント固有ID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入キャンセル処理：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStorePayCancel : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvStorePayCancelValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：魔法石購入キャンセル処理：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvStorePayCancelValue
    {
        public int dummy;           //!<
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：レビュー遷移報酬
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：レビュー遷移報酬：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendReviewPresent
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：レビュー遷移報酬：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvReviewPresent : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvReviewPresentValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：レビュー遷移報酬：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvReviewPresentValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報
    };


    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：プレゼント一覧取得
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：プレゼント一覧取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendPresentListGet
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：プレゼント一覧取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvPresentListGet : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvPresentListGetValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：プレゼント一覧取得：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvPresentListGetValue
    {
        public PacketStructPresent[] present;           //!< 受信情報：プレゼント一覧
    };



    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：プレゼント一覧取得
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：プレゼント一覧取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendLoginMonthlyGet
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：プレゼント一覧取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvLoginMonthlyGet
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvLoginMonthlyGetValue result;             //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：プレゼント一覧取得：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvLoginMonthlyGetValue
    {
        public PacketStructLoginMonthly monthly_present;            //!< 受信情報：プレゼント一覧
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：プレゼント開封通知
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：プレゼント開封通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendPresentOpen
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public long[] present_serial_id;    //!< プレゼントシリアルID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：プレゼント開封通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvPresentOpen : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvPresentOpenValue result;             //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：プレゼント開封通知：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvPresentOpenValue
    {
        public int[] error;
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報
        public long[] unit_unique_id;       //!< 受信情報：ユニットユニークID	※プレゼントがユニットだった場合の対象ユニークID
        public long[] open_serial_id;       //!< 受信情報：リクエストにより開封されたシリアルIDの一覧

        public PacketStructPresent[] present;           //!< 受信情報：プレゼント一覧
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：セーブ移行
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：セーブ移行：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendTransferOrder
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：セーブ移行：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvTransferOrder : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvTransferOrderValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：セーブ移行：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvTransferOrderValue
    {
        public string password;         //!< パスワード
        public int remaining_time;      //!< パスワード有効期限(日付を表す8桁の整数 年月日)
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：セーブ移行：移行実行
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：セーブ移行：移行実行：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendTransferExec
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint move_user_id;       //!<
        public string password;         //!<
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：セーブ移行：移行実行：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvTransferExec : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvTransferExecValue result;                //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：セーブ移行：移行実行：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvTransferExecValue
    {
        public int dummy;           //!<
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：アチーブメント開封：開封実行
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：アチーブメント開封：開封実行：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendOpenAchievement
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint[] achievement_open;             //!< アチーブメント関連：開封対象リスト	※MasterDataAchievement.fix_idと一致。一括開封できるように配列化
        public uint[] achievement_viewed;               //!< アチーブメント関連：演出確認済み	※MasterDataAchievement.fix_idと一致。クライアント側が演出済みなアチーブメント一覧を送る

        public uint[] achievement_gp_open;          //!< アチーブメント関連：開封対象リスト	※MasterDataAchievementGroup.fix_idと一致。一括開封できるように配列化
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：アチーブメント開封：開封実行：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvOpenAchievement : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvOpenAchievementValue result;             //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：アチーブメント開封：開封実行：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvOpenAchievementValue
    {
        public PacketStructPlayer player;                   //!< 受信情報：プレイヤー情報
        public uint[] achievement_opened;       //!< 受信情報：アチーブメント関連：実際開封したもの	整数配列	※MasterDataAchievement.fix_id と一致。実際に開封したもののリストを返す
        public int needs_update_presents;   //!< 受信情報：プレゼントリスト更新が必要か[ 0：なし ][ 1：あり ]
        public int[] error;                 //!< 受信情報：報酬受け取り出来ないものの状態
    };


    //----------------------------------------------------------------------------
    /*!
		@brief	アチーブメント構造体：受信実データ
		@note	アチーブメントで達成条件に特定行動の回数がある際に利用
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvAchievementCount
    {
        public uint achievement_id;     //!< MasterDataAchievement.fix_id と 一致
        public uint counter;            //!< achievement_id　に関連する数値データ　達成条件次第で討伐数や複数体の入手をまとめたビット値など様々な用途　詳細表示用途
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：ユーザーデータ編集
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：ユーザーデータ編集：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendDebugEditUser
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public int game_money;              //!< 入手情報：お金
        public int free_paid_point;         //!< 入手情報：無料チップ
        public int friend_point;            //!< 入手情報：フレンドポイント
        public int rank;                    //!< 入手情報：ランク
        public int buy_max_unit_count;      //!< 入手情報：ユニット枠購入数
        public int buy_max_friend_count;    //!< 入手情報：フレンド枠購入数
        public int ticket_casino;           //!< 入手情報：カジノチケット
        public int unit_point;              //!< 入手情報：ユニットポイント
        public int all_quest_clear;         //!< 入手情報：クエストクリア情報
        public int event_point_id;          //!< 入手情報：イベントポイント(アイテムマスタ管理のID指定)
        public int event_point_count;       //!< 入手情報：イベントポイント付与個数
        public int reset_gacha_id;          //!< 入手情報：指定IDのガチャログをリセット
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：ユーザーランクアップ：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvDebugEditUser : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvDebugEditUserValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：ユーザーランクアップ：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvDebugEditUserValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報
        public PacketStructGachaStatus[] gacha_status; //!< ガチャ情報：
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：成長曲線検証
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：成長曲線検証：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendDebugCalculationData
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
        public AppliCalculationData[] calculation_data; //!< 成長曲線検証データ
        public int seq_id;              //!< アプリ側でボタンを押したtimestamp
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：成長曲線検証：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvDebugCalculationData
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public AppliCalculationData[] result;               //!< クライアントでは特に何もしないので、nullを返してくる
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	デバッグ機能：デバッグ機能関連：成長曲線検証：受信実データ
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class AppliCalculationData
    {
        public uint unit_id;            // ユニットID
        public uint unit_level;     // ユニットレベル
        public uint exp;                // 対象レベル時のNEXT経験値
        public uint material_exp;       // 対象レベル時の素材経験値
        public uint sale_gold;      // 対象レベル時の売却価格
        public LimitOverCalculationData[] limitover;        // 限界突破計算
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	デバッグ機能：デバッグ機能関連：成長曲線検証：限界突破データ
		@note
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class LimitOverCalculationData
    {
        public uint limitover_lv;           // 限界突破レベル
        public uint material_exp;           // 対象限界突破レベル時の素材経験値
        public uint sale_gold;              // 対象限界突破レベル時の売却価格
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：ユニット取得
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：ユニット取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendDebugUnitGet
    {
        public PacketStructHeaderSend header;       //!< 送信共通ヘッダ
        public PacketStructUnitGetDebug[] get_unit; //!< 取得ユニット
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：ユニット取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvDebugUnitGet : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvDebugUnitGetValue result;                //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：ユニット取得：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvDebugUnitGetValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報
    };


    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：クエストクリア情報改変
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：クエストクリア情報改変：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendDebugQuestClear
    {
        public PacketStructHeaderSend header;       //!< 送信共通ヘッダ
        public int flag;        //!< クエストクリア改変フラグ	※[ 0:完全未クリア化 ][ 1:完全クリア化 ]
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：クエストクリア情報改変：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvDebugQuestClear : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvDebugQuestClearValue result;             //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：クエストクリア情報改変：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvDebugQuestClearValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報
    };

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：クエストクリア情報改変
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：クエストクリア情報改変：送信
	*/
    //----------------------------------------------------------------------------

    [Serializable]
    public class SendDebugBattleLog
    {
        public PacketStructHeaderSend header;       //!< 送信共通ヘッダ
        public string uuid;                         //!< UUID
        public int log_type;                            // !< ログタイプ ※現在 1(バトルログ) 固定
        public string log_data;                         // !< ログデータ
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：クエストクリア情報改変：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvDebugBattleLog : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvDebugBattleLogValue result;             //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：デバッグ機能関連：クエストクリア情報改変：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvDebugBattleLogValue
    {
        public string url;                                  //!< アップロードしたログファイルのURL
    };


    // 廃止だけどビルドエラーが邪魔なんで一旦有効化
    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット進化合成：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット進化合成：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendUnitBlendEvol
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint unique_id_base;     //!< 進化合成情報：ユニークID：ベース	※ユーザーが持つユニットのユニークID。
        public uint[] unique_id_parts;  //!< 進化合成情報：ユニークID：素材		※ユーザーが持つユニットのユニークID。

        public uint blend_unit_id;      //!< 進化合成情報：進化後キャラID
        public uint blend_pattern;      //!< 進化合成情報：進化パターン番号		※データベース内のアクセス番号。サーバー負荷軽減のために添付。
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット進化合成：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvUnitBlendEvol : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvUnitBlendEvolValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット進化合成：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvUnitBlendEvolValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報

        public long unit_unique;        //!< 受信情報：合成後ユニットユニークID
    };


    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット進化：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット進化：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendEvolveUnit
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public long evolve_base_unit_id;     //!< 進化合成情報：ユニークID：ベース	※ユーザーが持つユニットのユニークID。
        public long[] material_unit_list;  //!< 進化合成情報：ユニークID：素材		※ユーザーが持つユニットのユニークID。

        public uint chara_evol_id;      //!< 進化合成情報：進化後キャラID
        public uint after_unit_chara_id;      //!< 進化合成情報：進化パターン番号		※データベース内のアクセス番号。サーバー負荷軽減のために添付。
        public int beginner_boost_id;   //!< 初心者ブーストFixID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット進化：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvEvolveUnit : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvEvolveUnitValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ユニット進化：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvEvolveUnitValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報
    };



    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターハッシュ情報取得
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターハッシュ情報取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGetMasterHash
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ
        public PacketStructMasterHash[] hash;           //!< 送信情報：ローカルのマスターハッシュ情報
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターハッシュ情報取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetMasterHash : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvGetMasterHashValue result;           //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：マスターハッシュ情報取得：受信実体
	*/
    //----------------------------------------------------------------------------

    [Serializable]
    public class RecvGetMasterHashValue
    {
        public uint[] changed;      //!< 受信情報：マスターデータ更新フラグ
        public PacketStructMasterHash[] master_hash;        //!< 受信情報：マスターハッシュ情報
        public PacketStructMasterHash[] hash;           //!< 受信情報：マスターハッシュ情報
    };

    #region ==== SNSアカウント紐付け確認 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：SNSアカウント紐付け確認：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendCheckSnsLink
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public string sns_id;               //!< 調査するSNSID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：SNSアカウント紐付け確認：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvCheckSnsLink : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvCheckSnsLinkValue result;                //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：SNSアカウント紐付け確認：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvCheckSnsLinkValue
    {
        public PacketStructPlayer player;               //!< 受信情報：SNSIDに紐付いたプレイヤー情報
    };
    #endregion

    #region ==== SNSアカウント紐付け ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：SNSアカウント紐付け：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendSetSnsLink
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public string sns_id;               //!< 調査するSNSID
        public uint user_id;            //!< 確認用移行元ユーザーID

        public uint[] achievement_viewed;   //!< 送信情報：アチーブメント関連：演出確認済み	※MasterDataAchievement.fix_idと一致。クライアント側が演出済みなアチーブメント一覧を送る
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：SNSアカウント紐付け：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvSetSnsLink : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvSetSnsLinkValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：SNSアカウント紐付け：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvSetSnsLinkValue
    {
        public int dummy;               //!< ダミーコード
    };
    #endregion

    #region ==== SNSアカウントでの機種移行 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：SNSアカウントでの機種移行：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendMoveSnsSaveData
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public string sns_id;               //!< 調査するSNSID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：SNSアカウントでの機種移行：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvMoveSnsSaveData : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvMoveSnsSaveDataValue result;             //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：SNSアカウントでの機種移行：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvMoveSnsSaveDataValue
    {
        public int dummy;               //!< ダミーコード
    };
    #endregion

    #region ==== チュートリアルスキップ ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：チュートリアルスキップ
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：チュートリアルスキップ：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendSkipTutorial
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ

        public uint select_party;   //!< 申請情報：初期パーティ選択番号
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：チュートリアルスキップ：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvSkipTutorial : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvSkipTutorialValue result;            //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：チュートリアルスキップ：受信実データ
	*/
    //----------------------------------------------------------------------------
    public class RecvSkipTutorialValue
    {
        public PacketStructPlayer player;           //!< 受信情報：プレイヤー情報
        public PacketStructHero[] hero_list;            //!< 受信情報：HEROリスト
    };
    #endregion

    #region ==== SNSID作成 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：SNSID作成：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGetSnsID
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：SNSID作成：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetSnsID : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvGetSnsIDValue result;                //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：SNSID作成：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetSnsIDValue
    {
        public string sns_id;               //!< 作成したSNSID
    };
    #endregion

    #region ==== ガチャチケットによるガチャ実行 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ガチャチケット実行：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGachaTicketPlay
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public long present_serial_id;  //!< プレゼントシリアルID
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ガチャチケット実行：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGachaTicketPlay : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvGachaTicketPlayValue result;             //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ガチャチケット実行：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGachaTicketPlayValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報

        public long[] unit_unique_id;       //!< 受信情報：入手ユニットユニークID
        public uint[] blank_unit_id;        //!< 受信情報：ハズレユニット情報：キャラID

        public uint gacha_id;           //!< 受信情報：ガチャ情報：ガチャID
        public PacketStructPresent[] present;           //!< 受信情報：プレゼント一覧

        public PacketStructGachaStatus[] gacha_status;      //!< ガチャ情報：
    };
    #endregion

    #region ==== ポイントショップ商品情報を取得 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ポイントショップ商品情報を取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGetPointShopProduct
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public PacketStructMasterHash[] hash;               //!< 送信情報：マスターハッシュ情報
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ポイントショップ商品情報を取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetPointShopProduct : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvGetPointShopProductValue result;             //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ポイントショップ商品情報を取得：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetPointShopProductValue
    {
        public MasterDataPointShopProduct[] shop_product;           //!< 受信情報：商品情報
    };
    #endregion

    #region ==== ポイントショップ商品購入 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ポイントショップ商品購入：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendPointShopPurchase
    {
        public PacketStructHeaderSend header;                   //!< 送信共通ヘッダ
        public long point_shop_product_id;  //!< 購入プロダクトID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ポイントショップ商品購入：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvPointShopPurchase : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvPointShopPurchaseValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ポイントショップ商品購入：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvPointShopPurchaseValue
    {
        public PacketStructPlayer player;           //!< 受信情報：プレイヤー情報
    };
    #endregion

    #region ==== ポイントショップ限界突破 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ポイントショップ限界突破：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendPointShopLimitOver
    {
        public PacketStructHeaderSend header;                   //!< 送信共通ヘッダ

        public long point_shop_product_id;  //!< 購入プロダクトID
        public long unit_unique_id;         //!< 限界突破を行うユニットユニークID
        public uint limit_over_count;       //!< 限界突破を行う回数
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ポイントショップ限界突破：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvPointShopLimitOver : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvPointShopLimitOverValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ポイントショップ限界突破：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvPointShopLimitOverValue
    {
        public PacketStructPlayer player;           //!< 受信情報：プレイヤー情報
    };
    #endregion

    #region ==== ポイントショップ進化 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ポイントショップ限界突破：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendPointShopEvol
    {
        public PacketStructHeaderSend header;                   //!< 送信共通ヘッダ

        public long point_shop_product_id;  //!< 購入プロダクトID
        public long unit_unique_id;         //!< 限界突破を行うユニットユニークID
        public uint blend_unit_id;          //!< 進化合成情報：進化後キャラID
        public uint blend_pattern;          //!< 進化合成情報：進化パターンID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ポイントショップ進化：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvPointShopEvol : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvPointShopEvolValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ポイントショップ進化：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvPointShopEvolValue
    {
        public PacketStructPlayer player;           //!< 受信情報：プレイヤー情報
    };
    #endregion

    #region ==== 消費アイテム使用 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：消費アイテム使用
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：消費アイテム使用：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendItemUse
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint item_id;            //!< 消費アイテムID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：消費アイテム使用：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvItemUse : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ

        public RecvItemUseValue result;         //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：消費アイテム使用：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvItemUseValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報
    };
    #endregion

    #region ==== BOXガチャ在庫状況取得 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：BOXガチャ在庫状況取得
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：BOXガチャ在庫状況取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGetBoxGachaStock
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint fix_id;             //!< 取得ガチャID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：BOXガチャ在庫状況取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetBoxGachaStock : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ

        public RecvGetBoxGachaStockValue result;            //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：BOXガチャ在庫状況取得：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetBoxGachaStockValue
    {
        public PacketStructBoxGachaStock[] stock_list;      //!< 受信情報：ボックスの在庫情報
    };
    #endregion

    #region ==== BOXガチャ在庫状況リセット ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：BOXガチャ在庫状況リセット
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：BOXガチャ在庫状況リセット：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendResetBoxGachaStock
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public uint fix_id;             //!< 取得ガチャID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：BOXガチャ在庫状況リセット：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvResetBoxGachaStock : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ

        public RecvResetBoxGachaStockValue result;          //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：BOXガチャ在庫状況リセット：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvResetBoxGachaStockValue
    {
        public PacketStructBoxGachaStock[] stock_list;      //!< 受信情報：ボックスの在庫情報
    };
    #endregion

    #region ==== HERO情報変更 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：HERO情報変更：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：HERO情報変更：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendSetCurrentHero
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ
        public int hero_id;             //!< 主人公ID
        public int unique_id;           //!< 主人公のユニークID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：HERO情報変更：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvSetCurrentHero : RecvDataBase
    {
        public PacketStructHeaderRecv header;       //!< 受信共通ヘッダ
        public RecvSetCurrentHeroValue result;      //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：HERO情報変更：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvSetCurrentHeroValue
    {
        public PacketStructHero[] hero_list;       //!< 受信情報：HEROリスト
        public PacketStructPlayer player;          //!< 受信情報：プレイヤー情報
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：HERO情報：
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructHero
    {
        public int unique_id;               // 一意なID
        public int hero_id;                 // 主人公ID
        public int level;                   // 主人公LV
        public int exp;                     // 総獲得経験値
        public int current_skill_id;        // 主人公のスキルID
        public int kind;                    //  1：生徒 2: 先生
        public int gemder;                  //  1：人間男性 2: 人間女性
        public int current_wear_id;         // 選択中の服装ID デフォルト（初期服装）: 0
    };

    #endregion

    #region ==== ゲリラボス情報取得 ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ゲリラボス情報取得：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ゲリラボス情報取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGetGuerrillaBossInfo
    {
        public PacketStructHeaderSend header;           //!< 送信共通ヘッダ
        public int[] area_id_list;             //!< エリアID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ゲリラボス情報取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetGuerrillaBossInfo : RecvDataBase
    {
        public PacketStructHeaderRecv header;       //!< 受信共通ヘッダ
        public RecvGetGuerrillaBossInfoValue result;      //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ゲリラボス情報取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetGuerrillaBossInfoValue
    {
        public PacketStructGuerrillaBossInfo[] guerrilla_boss_list;
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ゲリラボス情報：
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructGuerrillaBossInfo
    {
        public int area_id;                 // エリアID
        public int[] boss_id_list;          // ボスID
    };
    #endregion

    #region ==== ガチャラインナップ詳細 ====

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ガチャラインナップ詳細：
		@brief	通信パケット：ガチャラインナップ詳細：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGetGachaLineup
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public int gacha_id;     //!< ガチャID情報：ガチャID
        public uint step_id;     //!< ラインナップ(確率)を確認したい各ステップのID ステップアップガチャ以外は「0」で固定
        public uint assign_id; //!< ステップアップごとのアサインID ステップアップガチャ以外は「0」で固定
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ガチャラインナップ詳細：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetGachaLineup : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvGetGachaLineupValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ガチャラインナップ詳細：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetGachaLineupValue
    {
        public PacketStructGachaLineup[] gacha_assign_unit_list;               //!< 受信情報：ガチャラインナップ詳細情報
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ガチャラインナップ詳細情報：
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructGachaLineup
    {
        public int id;                         // ユニットID（chara_masterのfix_id）
        public int limit_icon;                 // 現在限定表記の有無 0: なし　1：あり値
        public int lineup_sort_group_id;       // グループID
        public int rate_up_icon;               // 超絶UP表記の有無 0: なし　1：1.5倍　2：2倍　... 備考で補足します
        public string rate;                    // 排出確率（webview参照等の場合：空文字を返す）
    };

    #endregion

    #region ==== ホームページのトピック ニュース情報取得====

    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：トピックニュース情報取得：
		@brief	通信パケット：トピックニュース情報取得：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGetTopicInfo
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public int gacha_id;     //!< ガチャID情報：ガチャID
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ニュース情報取得：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetTopicInfo : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvGetTopicInfoValue result;               //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：ニュース情報取得詳細：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetTopicInfoValue
    {
        public PacketStructPickup pickup;               //!< 受信情報：お得情報
        public PacketStructGuerrilla guerrilla;         //!< 受信情報：ゲリラ情報
        public PacketStructNewUnit new_unit;            //!< 受信情報：新ユニット情報
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ニュース情報取得情報：お得情報
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructPickup
    {
        public PacketStructPickupImage[] image;             //!< 受信情報：お得情報 トップ画像情報
        public PacketStructPickupText[] text;               //!< 受信情報：お得情報 text
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ニュース情報取得情報：お得情報 トップ画像情報
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructPickupImage
    {
        public uint timing_start;                   //!< 受信情報：表示開始日時
        public uint timing_end;                     //!< 受信情報：表示終了日時
        public string url;                            //!< 受信情報：お得情報 画像URL
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ニュース情報取得情報：お得情報 text
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructPickupText
    {
        public uint timing_start;                 //!< 受信情報：表示開始日時
        public uint timing_end;                   //!< 受信情報：表示終了日時
        public string message;                      //!< 受信情報：お得情報文章
        public int event_flag;                   //!< 受信情報：お得情報の飾り表示用フラグ(0=OFF,1=ON)
    };


    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ニュース情報取得情報：ゲリラ情報
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructGuerrilla
    {
        public PacketStructGuerrillaImage[] image;             //!< 受信情報：ゲリラ情報 トップ画像情報
        public PacketStructGuerrillaText[] text;               //!< 受信情報：ゲリラ情報 text
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ニュース情報取得情報：ゲリラ情報 トップ画像情報
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructGuerrillaImage
    {
        public uint timing_start;                   //!< 受信情報：表示開始日時
        public uint timing_end;                     //!< 受信情報：表示終了日時
        public string url;                            //!< 受信情報：ゲリラ情報 画像URL
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ニュース情報取得情報：ゲリラ情報 text
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructGuerrillaText
    {
        public uint timing_start;                 //!< 受信情報：表示開始日時
        public uint timing_end;                   //!< 受信情報：表示終了日時
        public string message;                      //!< 受信情報：ゲリラ情報文章
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ニュース情報取得情報：新ユニット情報
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructNewUnit
    {
        public PacketStructNewUnitImage[] image;             //!< 受信情報：新ユニット情報 トップ画像情報
        public PacketStructNewUnitIcon[] icon;               //!< 受信情報：新ユニット情報 ユニットアイコンブロック情報
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ニュース情報取得情報：新ユニット情報 トップ画像情報
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructNewUnitImage
    {
        public uint timing_start;                   //!< 受信情報：表示開始日時
        public uint timing_end;                     //!< 受信情報：表示終了日時
        public string url;                            //!< 受信情報：ゲリラ情報 画像URL
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	汎用構造体：ニュース情報取得情報：新ユニット情報 icon
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class PacketStructNewUnitIcon
    {
        public uint timing_start;                 //!< 受信情報：表示開始日時
        public uint timing_end;                   //!< 受信情報：表示終了日時
        public string title;                      //!< 受信情報：ユニットアイコンブロックのタイトル
        public uint[] ids;                      //!< 受信情報：ユニットIDの配列
    };

    #endregion

    #region ==== 定期データ更新(デバイストークン) ====
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：定期データ更新(デバイストークン)
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：定期データ更新(デバイストークン)：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendPeriodicUpdate
    {
        public PacketStructHeaderSend header;               //!< 送信共通ヘッダ

        public string device_token;     //!< 端末ID：GCM/APNSからデバイストークンを貰いディバゲゲームサーバに送信
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：定期データ更新(デバイストークン)：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvPeriodicUpdate : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ

        public RecvPeriodicUpdateValue result;          //!< 受信情報：情報実体	※リザルトコードが不正の場合はNull
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：定期データ更新(デバイストークン)：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvPeriodicUpdateValue
    {
        public uint dummy;          //!< ダミーパラメータ：※ [ 0 ～ ] ※リザルトフォーマットを保つためのダミー変数
    };
    #endregion

    #region ==== プレゼント開封ログ取得 ====
    //----------------------------------------------------------------------------
    /*!
        @brief	プレゼント関連：プレゼント開封ログ取得
    */
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
        @brief	プレゼント関連：プレゼント開封ログ取得：送信
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendGetPresentOpenLog
    {
        public PacketStructHeaderSend header;           //!< 受信共通ヘッダ
    };
    //----------------------------------------------------------------------------
    /*!
        @brief	プレゼント関連：プレゼント開封ログ取得：受信
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetPresentOpenLog : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvGetPresentOpenLogValue result;          //!< 受信情報：情報実体	※リザルトコードが不正の場合はNull
    };
    //----------------------------------------------------------------------------
    /*!
        @brief	プレゼント関連：プレゼント開封ログ取得：受信実データ
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvGetPresentOpenLogValue
    {
        public PacketStructPresent[] present;           //!< 受信情報：プレゼント一覧
    };

    #endregion

    #region ==== ユーザースコア情報取得 ====

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class SendGetUserScoreInfo
    {
        public PacketStructHeaderSend header;           //!< 受信共通ヘッダ
        public int[] event_ids;                         //!< イベントID配列
    }

    [Serializable]
    public class RecvGetUserScoreInfo : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvGetUserScoreInfoValue result;
    }

    [Serializable]
    public class RecvGetUserScoreInfoValue
    {
        public PacketStructUserScoreInfo[] score_infos;
    }

    #endregion

    #region ==== スコア報酬取得 ====

    [Serializable]
    public class SendGetScoreReward
    {
        public PacketStructHeaderSend header;           //!< 受信共通ヘッダ
        public int event_id;                            //!< イベントID
        public int[] reward_id_list;                    //!< 報酬IDリスト
    }

    [Serializable]
    public class RecvGetScoreReward : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvGetScoreRewardValue result;
    }

    [Serializable]
    public class RecvGetScoreRewardValue
    {
        public PacketStructUserScoreInfo score_info;    //!< 受信情報：スコア情報
        public PacketStructPresent[] present;           //!< 受信情報：プレゼント一覧
    }

    #endregion

    #region ==== 成長ボス情報取得 ====

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class SendGetChallengeInfo
    {
        public PacketStructHeaderSend header;           //!< 受信共通ヘッダ
        public int[] event_ids;                         //!< イベントID配列
    }

    [Serializable]
    public class RecvGetChallengeInfo : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvGetChallengeInfoValue result;
    }

    [Serializable]
    public class RecvGetChallengeInfoValue
    {
        public PacketStructChallengeInfo[] challenge_infos;
    }

    #endregion

    #region ==== 成長ボス報酬取得 ====
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class SendGetChallengeReward
    {
        public PacketStructHeaderSend header;           //!< 受信共通ヘッダ
        public int event_id;                            //!< イベントID
        public int[] reward_id_list;                    //!< 報酬ID配列
        public int[] loop_cnt_list;                     //!< ループ回数配列
    }

    [Serializable]
    public class RecvGetChallengeReward : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvGetChallengeRewardValue result;
    }

    [Serializable]
    public class RecvGetChallengeRewardValue
    {
        public PacketStructChallengeGetReward[] get_list;   //!< 受信情報：報酬リスト
        public PacketStructPresent[] present;               //!< 受信情報：プレゼント一覧
    }
    #endregion

    #region === 成長ボスクエスト関連通信パケット ===
    /*==========================================================================*/
    /*		class																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエスト開始通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエスト開始通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendChallengeQuestStart
    {
        public PacketStructHeaderSend header;      //!< 送信共通ヘッダ

        public uint quest_id;                 //!< クエスト情報：クエストID
        public uint level;                    //!< クエスト情報：レベル
        public bool skip;                     //!< クエスト情報：スキップフラグ

        public int helper_point_ok;           //!< 助っ人情報：フレンドポイントの付与有無	※[ 0:フレンドポイントを付与しない ][ 1:フレンドポイントを付与 ]
        public int beginner_boost_id;         //!< 初心者ブーストFixID
        public uint quest_state;              //!< クエスト情報：補正情報 ※20160328 サーバー側未使用
        public uint[] area_amend_id;          //!< エリア補正IDリスト
        public int party_current;             //!< 選択しているパーティ番号
        public uint helper_user_id;                 //!< 助っ人情報：ユーザーID
        public uint event_friendpoint;              //!< 関連イベント開催情報：フレンドポイント増加イベント
        public PacketStructUnit helper_unit;        //!< 助っ人情報：ユニット
        public PacketStructUnit helper_unit_link;   //!< 助っ人情報：リンクユニット
        public bool is_auto_play;                   //!< オートプレイを使用したかどうか

    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエスト開始通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvChallengeQuestStart : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvQuest2StartValue result;              //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエストクリア通知：
	*/
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエストクリア通知：送信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class SendChallengeQuestClear
    {
        public PacketStructHeaderSend header;                   //!< 送信共通ヘッダ

        public uint quest_id;                                   //!< 成長ボスクエストクリア情報：クエストID
        public SendEnemyKill[] enemy_kill_list;                 //!< 成長ボスクエストクリア情報：倒した敵リスト
        public bool no_damage;                                  //!< 成長ボスクエストクリア情報：ノーダメージフラグ
        public int max_damage;                                  //!< 成長ボスクエストクリア情報：最大ダメージ
        public SendSkillExecCount[] active_skill_execute_count; //!< 成長ボスクエストクリア情報：スキル使用回数
        public int hero_skill_execute_count;                    //!< 成長ボスクエストクリア情報：ヒーロースキル使用回数
        public PlayScoreInfo play_score_info;                   //!< プレイスコア計算用情報
        public bool is_auto_play;                               //!< オートプレイを使用したかどうか
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエストクリア通知：受信
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvChallengeQuestClear : RecvDataBase
    {
        public PacketStructHeaderRecv header;               //!< 受信共通ヘッダ
        public RecvChallengeQuestClearValue result;         //!< 受信情報：情報実体	※リザルトコードがエラーの場合にnullになる可能性がある
    };
    //----------------------------------------------------------------------------
    /*!
		@brief	通信パケット：新クエストクリア通知：受信実データ
	*/
    //----------------------------------------------------------------------------
    [Serializable]
    public class RecvChallengeQuestClearValue : RecvQuest2ClearValue
    {
        public PacketStructResultChallenge result_challenge;     //!< 成長ボスリザルト情報
    };

    #endregion

    #region ==== 日付変更情報取得 ====
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class SendDayStraddle
    {
        public PacketStructHeaderSend header;           //!< 受信共通ヘッダ
    }

    [Serializable]
    public class RecvDayStraddle : RecvDataBase
    {
        public PacketStructHeaderRecv header;           //!< 受信共通ヘッダ
        public RecvDayStraddleValue result;
    }

    [Serializable]
    public class RecvDayStraddleValue
    {
        public PacketStructPlayer player;               //!< 受信情報：プレイヤー情報
        public PacketStructGachaStatus[] gacha_status;  //!< 受信情報：ガチャ情報
    }

    #endregion
    //----------------------------------------------------------------------------
    /*!
        @brief	汎用構造体：ダイヤログ文言：
        @note	エラーダイヤログ文言に使用。
    */
    //----------------------------------------------------------------------------
    [Serializable]
    public class ApiErrorDialogTextKey
    {
        public string title_key { get; set; }       //< タイトル
        public string contents_key { get; set; }    //< メインテキスト
        public string button_key1 { get; set; }      //< OKボタンテキスト
        public string button_key2 { get; set; }      //< OKボタンテキスト
        public CHANGE_SCENE change_scene { get; set; }         //< 挙動（どこに遷移するか）
        public DIALOG_TYPE dialog_type { get; set; }      //< ダイアログタイプ
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	シーン変更
		@note
	*/
    //----------------------------------------------------------------------------
    public enum CHANGE_SCENE
    {
        CHANGE_SCENE_HOME,             //!< ホーム画面：
        CHANGE_SCENE_TITLE,            //!< タイトル：
        CHANGE_SCENE_STAY,             //!< そのまま：
        CHANGE_SCENE_STAY_OR_ACTION,                //!< ：
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	ダイアログタイプ
		@note
	*/
    //----------------------------------------------------------------------------
    public enum DIALOG_TYPE
    {
        DIALOG_TYPE_1 = 1,               //!< ダイアログタイプ1：
        DIALOG_TYPE_2 = 2                //!< ダイアログタイプ2：
    };


}
