/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	GlobalDefine.cs
	@brief	共通定義関連一元化クラス
	@author Developer
	@date 	2012/10/09

	複数スクリプトで参照する可能性のある各種定義の一元化クラス。
	Unityは [ #define ] が使えないため、static変数を使用する。
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using StrOpe = StringOperationUtil.OptimizedStringOperation;

#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG
using SimpleSQL;
#endif

public enum ApiEnv
{
    NONE,
    API_TEST_ADDRESS_ONLINE,
    API_TEST_ADDRESS_DEVELOP_1_NEW_GOE,
    API_TEST_ADDRESS_DEVELOP_1_IP_GOE,
    API_TEST_ADDRESS_DEVELOP_0_GOE,
    API_TEST_ADDRESS_STAGING_0_NEW_GOE,
    API_TEST_ADDRESS_STAGING_0_IP_GOE,
    API_TEST_ADDRESS_STAGING_1_NEW_GOE,
    API_TEST_ADDRESS_STAGING_1_IP_GOE,
    API_TEST_ADDRESS_STAGING_2a_GOE,
    API_TEST_ADDRESS_STAGING_2b_GOE,
    API_TEST_ADDRESS_STAGING_2c_GOE,
    API_TEST_ADDRESS_STAGING_3a_GOE,
    API_TEST_ADDRESS_STAGING_3b_GOE,
    API_TEST_ADDRESS_STAGING_3c_GOE,
    API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE,
    API_TEST_ADDRESS_STAGING_REVIEW_IP_GOE,
    API_TEST_ADDRESS_STAGING_REHEARSAL_GOE,
    API_TEST_ADDRESS_LOCAL_GOE
}

public static class ApiEnvExtension
{
    public static string GetIpAddress(this ApiEnv env)
    {
#if _MASTER_BUILD
        return GlobalDefine.API_TEST_ADDRESS_ONLINE; //!< APIテスト用アドレス：本番用
#else
        switch (env)
        {
            case ApiEnv.API_TEST_ADDRESS_ONLINE:
                return GlobalDefine.API_TEST_ADDRESS_ONLINE; //!< APIテスト用アドレス：本番用

            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_NEW_GOE:
                return GlobalDefine.API_TEST_ADDRESS_DEVELOP_1_NEW_GOE;
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_IP_GOE:
                return GlobalDefine.API_TEST_ADDRESS_DEVELOP_1_IP_GOE;
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_0_GOE:
                return GlobalDefine.API_TEST_ADDRESS_DEVELOP_0_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_0_NEW_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_0_NEW_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_0_IP_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_0_IP_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_1_NEW_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_1_NEW_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_1_IP_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_1_IP_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_2a_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_2a_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_2b_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_2b_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_2c_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_2c_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_3a_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_3a_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_3b_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_3b_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_3c_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_3c_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_IP_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_REVIEW_IP_GOE;
            case ApiEnv.API_TEST_ADDRESS_STAGING_REHEARSAL_GOE:
                return GlobalDefine.API_TEST_ADDRESS_STAGING_REHEARSAL_GOE;

            case ApiEnv.API_TEST_ADDRESS_LOCAL_GOE:
                return GlobalDefine.API_TEST_ADDRESS_LOCAL_GOE; //!< APIテスト用アドレス：ローカル
        }
        return "";
#endif
    }
}

//----------------------------------------------------------------------------
static public class GlobalDefine
{
    public static int USER_NAME_MAX_LENGTH = 10;

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
        @brief	WebViewでの表示先リンク
    */
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    // AppleStore アプリページ
    //----------------------------------------------------------------------------
    public static string WEB_LINK_APPSTORE = "https://example.com"; //!< AppleStore アプリページ


    //----------------------------------------------------------------------------
    // GooglePlay アプリページ
    //----------------------------------------------------------------------------
    public static string WEB_LINK_PLAYSTORE = "https://example.com"; //!< GooglePlay アプリページ


    //----------------------------------------------------------------------------
    //
    //----------------------------------------------------------------------------
    public static string IN_BROWSER_DOMAIN = "example.com";

    //----------------------------------------------------------------------------
    // サポートホームページ(MasterDataGlobalParamのfix_id)
    //----------------------------------------------------------------------------
    public static uint WEB_LINK_SUPPORT = 13;

    //----------------------------------------------------------------------------
    // 資金決済法(MasterDataGlobalParamのfix_id)
    //----------------------------------------------------------------------------
    public static uint WEB_LINK_SHIKIN_KESSAI = 12;

    //----------------------------------------------------------------------------
    // 特定商取引法(MasterDataGlobalParamのfix_id)
    //----------------------------------------------------------------------------
    public static uint WEB_LINK_TOKUTEI_TORIHIKI = 11;

    //----------------------------------------------------------------------------
    // WebViewのURL(MasterDataGlobalParamのfix_id)
    //----------------------------------------------------------------------------
    public static uint WEB_LINK_INFORMATION = 5;
    public static uint WEB_LINK_HP_DIVINE = 15;
    public static uint WEB_LINK_MR_DIVINE = 6;

    public static string StrVersion()
    {
        return Application.version;
    }

    private static int intVersion = 0;

    public static int AppVerToInt()
    {
        if (intVersion <= 0)
        {
            intVersion = StrVersion().Replace(".", "").ToInt(0);
        }

        return intVersion;
    }

    public const string SERVER_API_OLD_VER = "5.4.0"; //!< APIバージョン 旧バージョン ※サーバー接続バージョン
    public const string SERVER_API_CURRENT_VER = "5.5.0"; //!< APIバージョン 現状バージョン ※サーバー接続バージョン
    public const string SERVER_API_NEXT_VER = "5.6.0"; //!< APIバージョン 次のバージョン ※サーバー接続バージョン

    public const string CF_PROD_BASE_ADDRESS = "https://example.com";
    public const string S3_PROD_BASE_ADDRESS = "https://example.com";
#if _MASTER_BUILD
#else
    public const string CF_DEV_BASE_ADDRESS = "https://example.com";
    public const string S3_DEV_BASE_ADDRESS = "http://45.77.21.37";
    public const string S3_DEV_BASE_HTTP_ADDRESS = "http://45.77.21.37";
#endif

    public const string SERVER_COOKIE_SESSIONID = "PQDMSESSID"; //!< セッションIDフィールド名

    public const string SERVER_USER_AGENT = "PQDM-Client-App"; //!<

    public const string API_TEST_ADDRESS_ONLINE = "dg.example.com"; //!< APIテスト用アドレス：本番用
    public const string API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE = "review.example.com";

#if _MASTER_BUILD
    public enum SERVER_TYPE : int
    {
        prod = 0,    // 本番
        review = 1, 
        none = 99999,   // NG
    }
#else
    public enum SERVER_TYPE : int
    {
        prod = 0,    // 本番
        review = 1,
        stg0 = 2,
        stg1 = 3,
        stg2a = 4,
        stg2b = 5,
        stg2c = 6,
        stg3a = 7,
        stg3b = 8,
        stg3c = 9,
        dev0 = 10,
        dev1 = 11,
        rehearsal = 99,
        local = 100,
        none = 99999,   // NG
    };

    public const string API_TEST_ADDRESS_DEVELOP_1_NEW_GOE = "dev1.example.com";
    public const string API_TEST_ADDRESS_DEVELOP_1_IP_GOE = "104.238.161.24";
    public const string API_TEST_ADDRESS_DEVELOP_0_GOE = "192.0.2.2";
    public const string API_TEST_ADDRESS_STAGING_0_NEW_GOE = "stg0.example.com";
    public const string API_TEST_ADDRESS_STAGING_0_IP_GOE = "192.0.2.3";
    public const string API_TEST_ADDRESS_STAGING_1_NEW_GOE = "stg1.example.com";
    public const string API_TEST_ADDRESS_STAGING_1_IP_GOE = "192.0.2.4";
    public const string API_TEST_ADDRESS_STAGING_2a_GOE = "192.0.2.5";
    public const string API_TEST_ADDRESS_STAGING_2b_GOE = "192.0.2.6";
    public const string API_TEST_ADDRESS_STAGING_2c_GOE = "192.0.2.7";
    public const string API_TEST_ADDRESS_STAGING_3a_GOE = "192.0.2.8";
    public const string API_TEST_ADDRESS_STAGING_3b_GOE = "192.0.2.9";
    public const string API_TEST_ADDRESS_STAGING_3c_GOE = "192.0.2.10";
    public const string API_TEST_ADDRESS_STAGING_REVIEW_IP_GOE = "192.0.2.11";
    public const string API_TEST_ADDRESS_STAGING_REHEARSAL_GOE = "reha.dg.example.com";

    public const string API_TEST_ADDRESS_LOCAL_GOE = "pqdm_local"; //!< APIテスト用アドレス：ローカル
#endif

    //----------------------------------------------------------------------------
    /*!
        @brief	マスターデータ内部ID
    */
    //----------------------------------------------------------------------------
    public const uint TUTORIAL_AREA_1 = (1000000); //!< エリアマスターID：チュートリアルエリア
    public const uint TUTORIAL_QUEST_1 = (100001); //!< クエストマスターID：チュートリアルクエスト
    public const uint TUTORIAL_SCRATCH = (100); //!< ユニットID：チュートリアル専用スクラッチ

    public const uint TUTORIAL_STORY01 = (100000);
    public const uint TUTORIAL_STORY02 = (100001);
    public const uint TUTORIAL_STORY03 = (100002);

    public const int RENEW_QUEST_OFFSET = 100000;        //!< 新クエストIDオフセット
    public const int CHALLENGE_QUEST_OFFSET = 100000000; //!< 成長ボスクエストIDオフセット
    //----------------------------------------------------------------------------
    /*!
        @brief	各種最大値
    */
    //----------------------------------------------------------------------------
    public const uint VALUE_MAX_COIN = 4200000000;      //!< 各種最大値：所持金

    public const int VALUE_MAX_FRIEND_PT = 999999;      //!< 各種最大値：フレンドポイント
    public const int VALUE_MAX_POW = 999999;            //!< 各種最大値：攻撃力
    public const int VALUE_MAX_HP = 999999;             //!< 各種最大値：体力
    public const int VALUE_MAX_RANK = 9999;             //!< 各種最大値：ランク
    public const int VALUE_MAX_LEVEL = 100;             //!< 各種最大値：最大レベル
    public const int VALUE_MAX_PLUS = 9999;             //!< 各種最大値：プラス値総合
    public const int VALUE_MAX_LIMIT_OVER = 9999;       //!< 各種最大値：限界突破

    public const int VALUE_MAX_TICKET = 99999;          //!< 各種最大値：チケット

    public const int VALUE_VIEW_MAX_STONE = 999999;     //!< 各種最大値：チップ表示限界(無料・有料合計の含)
    public const int VALUE_BUT_MAX_STONE = 999;         //!< 各種最大値：チップ購入限界(無料・有料合計がこの値以上だとshopから購入できなくなる)
    public const int VALUE_VIEW_MAX_INFO = 99;          //!< 各種最大値：通知表示限界

    public const int VALUE_MAX_ITEM = 999;              //!< 各種最大値：各消費アイテム
    public const int VALUE_MAX_ITEM_GACHA = 999999999;  //!< 各種最大値：各消費アイテム(ガチャチケット)
    public const int VALUE_MAX_QUEST_KEY = 999;         //!< 各種最大値：各クエストキー
    public const int VALUE_MAX_CHARM = 1000;            //!< 各種最大値：CHARM
    public const int VALUE_MAX_ACHIEVEMENT = 999;       //!< 各種最大値：各ミッションの表示上最大値
    public const int VALUE_VIEW_MAX_SAME_UNIT = 999;    //!< 各種最大値：同名ユニットの所持数表示最大値

    //----------------------------------------------------------------------------
    /*!
        @brief	各種最大値（所持限界値）
    */
    //----------------------------------------------------------------------------
    public const long VALUE_HAVE_MAX_COIN = 4200000000;         //!< 各種最大値：所持金

    public const int VALUE_HAVE_MAX_FRIEND_PT = 99999;          //!< 各種最大値：フレンドポイント
    public const int VALUE_HAVE_MAX_TICKET = 99999;             //!< 各種最大値：チケット
    public const int VALUE_HAVE_MAX_STONE = 199998;             //!< 各種最大値：チップ(無料・有料合計)
    public const int VALUE_HAVE_MAX_STONE_PAID = 99999;         //!< 各種最大値：有料チップ
    public const int VALUE_HAVE_MAX_STONE_FREE = VALUE_HAVE_MAX_STONE - VALUE_HAVE_MAX_STONE_PAID; //!< 各種最大値：無料チップ
    public const long VALUE_HAVE_MAX_UNIT_POINT = 999999999;    //!< 各種最大値：ユニットポイント
    public const int VALUE_HAVE_MAX_ITEM = 999;                 //!< 各種最大値：各消費アイテム
    public const int VALUE_HAVE_MAX_ITEM_GACHA = 999999999;     //!< 各種最大値：各消費アイテム(ガチャチケット)
    public const int VALUE_HAVE_MAX_QUEST_KEY = 999;            //!< 各種最大値：各クエストキー

    //----------------------------------------------------------------------------
    /*!
        @brief	プラス値関連
    */
    //----------------------------------------------------------------------------
    public const int PLUS_RATE_POW = 5;             //!< プラス値ステータス倍率：攻撃力

    public const int PLUS_RATE_HP = 10;             //!< プラス値ステータス倍率：体力
    public const int PLUS_LINK_RATE_POW = 1;        //!< プラス値ステータスリンク倍率：攻撃力
    public const int PLUS_LINK_RATE_HP = 2;         //!< プラス値ステータスリンク倍率：体力

    public const uint PLUS_SALE_COIN = 10000;       //!< プラス値売却価格

    public const uint PLUS_BUILD_COIN = 0;          //!< プラス値合成費用 ※1.0.5対応。合成費用からプラス値の計算を除外

    public const uint PLUS_MAX = 99;                //!< プラス値最大

    //----------------------------------------------------------------------------
    /*!
        @brief	限界突破関連
    */
    //----------------------------------------------------------------------------
    public const uint GLOBALPARAMS_LIMITOVER_BOUNS_SALE = 3;    //!< 限界突破ユニットの売却時のボーナス

    public const uint GLOBALPARAMS_LIMITOVER_BOUNS_BUILDUP = 4; //!< 限界突破ユニットの強化時のボーナス

    //----------------------------------------------------------------------------
    /*!
        @brief	機種情報
    */
    //----------------------------------------------------------------------------
    public const int PLATFORM_IOS = 0;      //!< プラットフォーム：iOS
    public const int PLATFORM_ANDROID = 1;  //!< プラットフォーム：Android
    public const int PLATFORM_xxx = 2;      //!< プラットフォーム：その他

    //----------------------------------------------------------------------------
    /*!
        @brief	クオリティタイプ
    */
    //----------------------------------------------------------------------------
    public const int QUALITY_NONE = -1; //!< クオリティタイプ：

    public const int QUALITY_1_1 = 0; //!< クオリティタイプ：高（テクスチャ等が倍サイズ）
    public const int QUALITY_1_2 = 1; //!< クオリティタイプ：中（テクスチャ等が普通サイズ）
    public const int QUALITY_1_4 = 2; //!< クオリティタイプ：低（テクスチャ等が半分サイズ）


    //----------------------------------------------------------------------------
    /*!
        @brief	進化合成判定用ID
        @note	外部にラベルを出してないとこなんで、IDを決め打ちで入力
    */
    //----------------------------------------------------------------------------
    public const int EVOLVE_EVENT_ID_FIRE = 100200; //!< 進化合成判定用ID：炎

    public const int EVOLVE_EVENT_ID_WATER = 100300; //!< 進化合成判定用ID：水
    public const int EVOLVE_EVENT_ID_WIND = 100400; //!< 進化合成判定用ID：風
    public const int EVOLVE_EVENT_ID_LIGHT = 100500; //!< 進化合成判定用ID：光
    public const int EVOLVE_EVENT_ID_DARK = 100100; //!< 進化合成判定用ID：闇
    public const int EVOLVE_EVENT_ID_NAUGHT = 100600; //!< 進化合成判定用ID：無
    public const int EVOLVE_EVENT_ID_ALL = 100800; //!< 進化合成判定用ID：全属性可能

    //----------------------------------------------------------------------------
    /*!
        @brief	特殊イベントID
        @note	外部にラベルを出してないとこなんで、IDを決め打ちで入力
    */
    //----------------------------------------------------------------------------
    public const int FP_EVENT_ID_x0150 = 110100; //!< フレンドポイント増加イベントID：1.5倍

    public const int FP_EVENT_ID_x0200 = 110200; //!< フレンドポイント増加イベントID：2.0倍
    public const int FP_EVENT_ID_x0250 = 110300; //!< フレンドポイント増加イベントID：2.5倍
    public const int FP_EVENT_ID_x0300 = 110400; //!< フレンドポイント増加イベントID：3.0倍
    public const int FP_EVENT_ID_x0400 = 110500; //!< フレンドポイント増加イベントID：4.0倍
    public const int FP_EVENT_ID_x0500 = 110600; //!< フレンドポイント増加イベントID：5.0倍
    public const int FP_EVENT_ID_x1000 = 110700; //!< フレンドポイント増加イベントID：10.0倍

    public const int SLV_EVENT_ID_x0150 = 111100; //!< スキルレベルアップ確率増加イベントID：1.5倍
    public const int SLV_EVENT_ID_x0200 = 111200; //!< スキルレベルアップ確率増加イベントID：2.0倍
    public const int SLV_EVENT_ID_x0250 = 111300; //!< スキルレベルアップ確率増加イベントID：2.5倍
    public const int SLV_EVENT_ID_x0300 = 111400; //!< スキルレベルアップ確率増加イベントID：3.0倍
    public const int SLV_EVENT_ID_x0400 = 111500; //!< スキルレベルアップ確率増加イベントID：4.0倍
    public const int SLV_EVENT_ID_x0500 = 111600; //!< スキルレベルアップ確率増加イベントID：5.0倍
    public const int SLV_EVENT_ID_x1000 = 111700; //!< スキルレベルアップ確率増加イベントID：10.0倍

    //----------------------------------------------------------------------------
    /*!
        @brief	ログインボーナスタイプ
    */
    //----------------------------------------------------------------------------

    public enum PartyCharaIndex
    {
        ERROR = -1,
        LEADER = 0, //!< パーティーキャラ：リーダー枠
        MOB_1 = 1, //!< パーティーキャラ：アサインキャラ
        MOB_2 = 2, //!< パーティーキャラ：アサインキャラ
        MOB_3 = 3, //!< パーティーキャラ：アサインキャラ
        FRIEND = 4, //!< パーティーキャラ：フレンド枠
        MAX = 5, //!< パーティーキャラ：最大
        GENERAL = 6, // スキルは発動するが、発動者がまだ決まっていないとき
        HERO = 7, // ヒーロースキル発動時のスキル情報用
    }

    //----------------------------------------------------------------------------
    /*!
        @brief
    */
    //----------------------------------------------------------------------------
    public const float FRIEND_CYCLE_HOURS = 14; //!< フレンドがもう一度使えるようになるまでの時間

    public const uint FRIEND_DUMMY_ID = 0; //!< フレンドダミーID

    //----------------------------------------------------------------------------
    /*!
        @brief
    */
    //----------------------------------------------------------------------------
    public const float STAMINA_RECOVERY_SEC = 180.0f; //!< スタミナが１回復するまでのサイクル（秒数）

    //----------------------------------------------------------------------------
    /*!
        @brief	ガチャ関連
    */
    //----------------------------------------------------------------------------
    // タブ用定義( メインメニューの下メニュー「GACHA」を押した際に出てくる数でもある
    public const int GACHA_TAB1 = 0; //!< ガチャタブ1

    public const int GACHA_TAB2 = 1; //!< ガチャタブ2
    public const int GACHA_TAB3 = 2; //!< ガチャタブ3

    public const int GACHA_TAB_MAX = 3; //!< ガチャタブ最大数

    public const int GACHA_PLAY_MAX = 9; //!<

    public const int GACHA_PROPER_PRICE_CHIP = 5; //!< 通常、1回で消費するチップの量

    //----------------------------------------------------------------------------
    /*!
        @brief	ユニット保持数情報
    */
    //----------------------------------------------------------------------------
    public const uint GLOBALPARAMS_UNIT_MAX_EXTEND = 1; //!< ユニット保持最大数：MasterDataGlobalParamsのユニットボックス拡張購入可能回数

    public const int SHOP_BUY_UNIT_ADD = 5; //!< ユニット保持数：購入による増加数

    //----------------------------------------------------------------------------
    /*!
        @brief	フレンド登録最大数情報
    */
    //----------------------------------------------------------------------------
    public const uint GLOBALPARAMS_FRIEND_MAX_EXTEND = 2; //!< フレンド最大数：MasterDataGlobalParamsのフレンド枠拡張購入可能回数

    public const int SHOP_BUY_FRIEND_ADD = 5; //!< フレンド数：購入による増加数

    public const int FRIEND_MAX_WAIT_ME = 50; //!< フレンド数最大数	フレンド申請受中リストのクリップに使用
    public const int FRIEND_MAX_WAIT_HIM = 50; //!< フレンド数最大数	フレンド申請出中リストのクリップに使用

    //----------------------------------------------------------------------------
    /*!
        @brief	ユーザーデータ
    */
    //----------------------------------------------------------------------------
    public const int INGAME_UNIT_MAX = 100; //!< ゲーム中獲得ユニット最大数

    public const int INGAME_MONEY_MAX = 100; //!< ゲーム中獲得コイン最大数
    public const int INGAME_TICKET_MAX = 100; //!< ゲーム中獲得チケット最大数

    //----------------------------------------------------------------------------
    /*!
        @brief
    */
    //----------------------------------------------------------------------------
    public const float CELL_SIZE_X = 1.0f; //!< セル情報：サイズX方向（横）

    public const float CELL_SIZE_Z = 1.0f; //!< セル情報：サイズZ方向（奥）
    public const int CELL_MAX_X = 5; //!< セル情報：総数X方向（横）
    public const int CELL_MAX_Z = 5; //!< セル情報：総数Z方向（奥）
    public const int CELL_MAX_TOTAL = CELL_MAX_X * CELL_MAX_Z; //!< セル情報：総数

    public const float PLAYER_MOVE_SPEED = 3.0f; //!< インゲーム情報：プレイヤー移動速度

    public const int PLAYER_MOVE_REQ_MAX = 30; //!< インゲーム情報：プレイヤー移動リクエストストック最大

    public const float INPUT_LONGPRESS = 0.35f; //!< インプット情報：長押し境界時間（秒）

    public const float ELEMENTRATE_NORMAL = 1.0f; //!< 通常倍率
    public const float ELEMENTRATE_WEEK = 2.0f; //!< 弱点倍率
    public const float ELEMENTRATE_GUARD = 0.5f; //!< 半減倍率

    public const float GAME_SPEED_UP_OFF = 1.0f; //!< オプション設定：ゲーム中速度：通常
    public const float GAME_SPEED_UP_ON = 1.5f; //!< オプション設定：ゲーム中速度：上昇

    //----------------------------------------------------------------------------
    /*!
        @brief	リソースパス
    */
    //----------------------------------------------------------------------------
    public const string DATAPATH_SOUND_SE = "Sound/SE/"; //!< リソースパス：サウンド

    public const string DATAPATH_SOUND_BGM = "Sound/BGM/";

    //----------------------------------------------------------------------------
    /*!
        @brief	カットインメッシュサイズ
    */
    //----------------------------------------------------------------------------
    public const int SKILL_LIMITBREAK_MAX = 10; //!< リミットブレイクスキルのコスト最大数
    public const int SKILL_LIMITBREAK_MIN = 0; //!< リミットブレイクスキルのコスト最小数

    public const int SKILL_LIMITBREAK_TURN_ADD = 1; //!< リミットブレイクスキルの１ターンでのコスト増加量

    static public Vector3 VECTOR_ZERO = new Vector3(0.0f, 0.0f, 0.0f); //!< 共通で使う０で初期化されたVECTOR3
    static public Vector3 VECTOR_ONE = new Vector3(1.0f, 1.0f, 1.0f); //!< 共通で使う１で初期化されたVECTOR3


    //----------------------------------------------------------------------------
    /*!
        @brief	クエストID
    */
    //----------------------------------------------------------------------------
    public const int INFINITY_DUNGEON_QUEST_001 = 233; //!< 無限ダンジョンのクエストID：初級

    public const int INFINITY_DUNGEON_QUEST_002 = 234; //!< 無限ダンジョンのクエストID：中級
    public const int INFINITY_DUNGEON_QUEST_003 = 235; //!< 無限ダンジョンのクエストID：上級
    public const int INFINITY_DUNGEON_QUEST_004 = 236; //!< 無限ダンジョンのクエストID：超級
    public const int INFINITY_DUNGEON_QUEST_005 = 237; //!< 無限ダンジョンのクエストID：神級

    //----------------------------------------------------------------------------
    /*!
        @brief	画面関係
    */
    //----------------------------------------------------------------------------

    public const int SCREEN_SIZE_W = 640; //!< 想定画面サイズ：幅
    public const int SCREEN_SIZE_H = 960; //!< 想定画面サイズ：高さ


    //----------------------------------------------------------------------------
    /*!
        @brief	ユニット情報
    */
    //----------------------------------------------------------------------------
    public const float UNITDETAIL_EFFECT_OFFSET_Z = -0.01f; //!< エフェクト表示オフセット


    //----------------------------------------------------------------------------
    /*!
        @brief	強化画面でのLimitBreakSkill関連
    */
    //----------------------------------------------------------------------------
    public const int BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL1_LOW = 1;    //!< リミットブレイクスキルのレベル差条件1の下限値
    public const int BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL1_HIGH = 10;  //!< リミットブレイクスキルのレベル差条件1の上限値
    public const int BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL2_LOW = 11;   //!< リミットブレイクスキルのレベル差条件2の下限値
    public const int BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL2_HIGH = 16;  //!< リミットブレイクスキルのレベル差条件2の上限値
    public const int BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL3_LOW = 17;   //!< リミットブレイクスキルのレベル差条件3の下限値
    public const int BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL3_HIGH = 18;  //!< リミットブレイクスキルのレベル差条件3の上限値
    public const int BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL4_LOW = 19;   //!< リミットブレイクスキルのレベル差条件4の下限値
    public const int BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL4_HIGH = 99;  //!< リミットブレイクスキルのレベル差条件4の上限値

    public const int BUILDUP_LIMITBREAKSKILL_BASE_RATE_1 = 10;          //!< リミットブレイクスキルのレベル差条件1の基本値
    public const int BUILDUP_LIMITBREAKSKILL_BASE_RATE_2 = 20;          //!< リミットブレイクスキルのレベル差条件2の基本値
    public const int BUILDUP_LIMITBREAKSKILL_BASE_RATE_3 = 30;          //!< リミットブレイクスキルのレベル差条件3の基本値
    public const int BUILDUP_LIMITBREAKSKILL_BASE_RATE_4 = 40;          //!< リミットブレイクスキルのレベル差条件4の基本値

    public const float BUILDUP_SKILLUP_EVENT_RATE_x0150 = 1.5f; //!< スキルレベルアップ確率増加倍率：1.5倍
    public const float BUILDUP_SKILLUP_EVENT_RATE_x0200 = 2.0f; //!< スキルレベルアップ確率増加倍率：2.0倍
    public const float BUILDUP_SKILLUP_EVENT_RATE_x0250 = 2.5f; //!< スキルレベルアップ確率増加倍率：2.5倍
    public const float BUILDUP_SKILLUP_EVENT_RATE_x0300 = 3.0f; //!< スキルレベルアップ確率増加倍率：3.0倍
    public const float BUILDUP_SKILLUP_EVENT_RATE_x0400 = 4.0f; //!< スキルレベルアップ確率増加倍率：4.0倍
    public const float BUILDUP_SKILLUP_EVENT_RATE_x0500 = 5.0f; //!< スキルレベルアップ確率増加倍率：5.0倍
    public const float BUILDUP_SKILLUP_EVENT_RATE_x1000 = 10.0f; //!< スキルレベルアップ確率増加倍率：10.0倍

    public const int BUILDUP_SAMESKILL_SKILL_RATE_MAX = 100; //!< 同一スキルのスキルアップレートの最大値
    public const int BUILDUP_SKILLEGG_RATE_MAX = 10000;      //!< スキルエッグのスキルアップレートの最大値

    //----------------------------------------------------------------------------
    /*!
        @brief	PATH
    */
    //----------------------------------------------------------------------------

    private static string ResourceBasePath = null;
    private static string S3BasePath = null;

    private static string BaseBannerUrl = null;
    private static string TapBannerListUrl = null;
    private static string MainMenuBannerListUrl = null;
    private static string AssetBundleUrl = null;
    private static string PatchUrl = null;
    private static string ScratchIconhUrl = null;
    private static string ScratchBoadhUrl = null;
    private static string MasterResourcesUrl = null;

    public static void ResetPaths()
    {
        S3BasePath = null;
        PatchUrl = null;

        ResourceBasePath = null;
        BaseBannerUrl = null;
        TapBannerListUrl = null;
        MainMenuBannerListUrl = null;
        AssetBundleUrl = null;
        ScratchIconhUrl = null;
        ScratchBoadhUrl = null;
        MasterResourcesUrl = null;
    }

    public static string GetResourceBasePath()
    {
        if (ResourceBasePath != null)
        {
            return ResourceBasePath;
        }

        // Patchでreviewへの切り替え
        // DVGAN-2130 参照
        if (Patcher.Instance.isNextVersion())
        {
            ResourceBasePath = S3_PROD_BASE_ADDRESS + "/review";
            return ResourceBasePath;
        }

#if _MASTER_BUILD
        ResourceBasePath = CF_PROD_BASE_ADDRESS + "/prod";
#else
        switch (GetApiEnv())
        {
            case ApiEnv.API_TEST_ADDRESS_ONLINE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_REHEARSAL_GOE:
                ResourceBasePath = CF_PROD_BASE_ADDRESS + "/prod";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_IP_GOE:
                ResourceBasePath = S3_PROD_BASE_ADDRESS + "/review";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_0_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_0_IP_GOE:
                ResourceBasePath = CF_DEV_BASE_ADDRESS + "/stg0";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_1_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_1_IP_GOE:
                ResourceBasePath = CF_DEV_BASE_ADDRESS + "/stg1";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_2a_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_2b_GOE:
                ResourceBasePath = S3_DEV_BASE_HTTP_ADDRESS + "/stg2a";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_2c_GOE:
                ResourceBasePath = S3_DEV_BASE_HTTP_ADDRESS + "/stg2c";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_3a_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_3b_GOE:
            case ApiEnv.API_TEST_ADDRESS_LOCAL_GOE:
                ResourceBasePath = S3_DEV_BASE_HTTP_ADDRESS + "/stg3a";

                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_3c_GOE:
                ResourceBasePath = S3_DEV_BASE_HTTP_ADDRESS + "/stg3c";
                break;

            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_NEW_GOE:
                ResourceBasePath = CF_DEV_BASE_ADDRESS + "/dev1";
                break;

            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_IP_GOE:
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_0_GOE:
                ResourceBasePath = S3_DEV_BASE_HTTP_ADDRESS + "/dev1";
                break;
        }
#endif
        return ResourceBasePath;
    }

    public static string GetS3BasePath()
    {
        if (S3BasePath != null)
        {
            return S3BasePath;
        }

        // Patchでreviewへの切り替えは禁止
        // 初回は対象サーバのpatchを読む
        // ２回目以降はReviewpatchを読む
        // DVGAN-2130 参照
        if (Patcher.Instance.isNextVersion())
        {
            S3BasePath = S3_PROD_BASE_ADDRESS + "/review";
            return S3BasePath;
        }

#if _MASTER_BUILD
        S3BasePath = S3_PROD_BASE_ADDRESS + "/prod";
#else
        switch (GetApiEnv())
        {
            case ApiEnv.API_TEST_ADDRESS_ONLINE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_REHEARSAL_GOE:
                S3BasePath = S3_PROD_BASE_ADDRESS + "/prod";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_IP_GOE:
                S3BasePath = S3_PROD_BASE_ADDRESS + "/review";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_0_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_0_IP_GOE:
                S3BasePath = S3_DEV_BASE_ADDRESS + "/stg0";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_1_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_1_IP_GOE:
                S3BasePath = S3_DEV_BASE_ADDRESS + "/stg1";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_2a_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_2b_GOE:
                S3BasePath = S3_DEV_BASE_HTTP_ADDRESS + "/stg2a";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_2c_GOE:
                S3BasePath = S3_DEV_BASE_HTTP_ADDRESS + "/stg2c";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_3a_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_3b_GOE:
            case ApiEnv.API_TEST_ADDRESS_LOCAL_GOE:
                S3BasePath = S3_DEV_BASE_HTTP_ADDRESS + "/stg3a";
                break;

            case ApiEnv.API_TEST_ADDRESS_STAGING_3c_GOE:
                S3BasePath = S3_DEV_BASE_HTTP_ADDRESS + "/stg3c";
                break;

            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_NEW_GOE:
                S3BasePath = S3_DEV_BASE_ADDRESS + "/dev1";
                break;

            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_IP_GOE:
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_0_GOE:
                S3BasePath = S3_DEV_BASE_HTTP_ADDRESS + "/dev1";
                break;
        }
#endif
        return S3BasePath;
    }

    public static string GetBaseBannerUrl()
    {
        if (BaseBannerUrl != null)
        {
            return BaseBannerUrl;
        }

#if _MASTER_BUILD
        BaseBannerUrl = GetResourceBasePath() + "/banner";
#else
        switch (GetApiEnv())
        {
            default:
                BaseBannerUrl = GetResourceBasePath() + "/banner";
                break;
        }
#endif

        return BaseBannerUrl;
    }


    public static string GetTapBannerListUrl()
    {
        if (TapBannerListUrl != null)
        {
            return TapBannerListUrl;
        }

#if _MASTER_BUILD
        TapBannerListUrl = GetBaseBannerUrl() + "/web_bannerlist.json";
#else
        switch (GetApiEnv())
        {
            default:
                TapBannerListUrl = GetBaseBannerUrl() + "/web_bannerlist.json";
                break;
        }
#endif

        return TapBannerListUrl;
    }

    public static string GetMainMenuBannerListUrl()
    {
        if (MainMenuBannerListUrl != null)
        {
            return MainMenuBannerListUrl;
        }

#if _MASTER_BUILD
        MainMenuBannerListUrl = GetBaseBannerUrl() + "/client_bannerlist.json";
#else
        switch (GetApiEnv())
        {
            default:
                MainMenuBannerListUrl = GetBaseBannerUrl() + "/client_bannerlist.json";
                break;
        }
#endif

        return MainMenuBannerListUrl;
    }

    public static string GetAssetBundleUrl()
    {
        if (AssetBundleUrl != null)
        {
            return AssetBundleUrl;
        }

#if _MASTER_BUILD
        AssetBundleUrl = GetResourceBasePath() + "/v5_0_0/{0}/";
#else
        switch (GetApiEnv())
        {
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_IP_GOE:
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_0_GOE:
                //case ApiEnv.API_TEST_ADDRESS_LOCAL_GOE:
                AssetBundleUrl = GetResourceBasePath() + "/v5_0_0/{0}/";
                break;
            default:
                AssetBundleUrl = GetResourceBasePath() + "/v5_0_0/{0}/";
                break;
        }
#endif

        return AssetBundleUrl;
    }

    public static string GetPatchUrl()
    {
        string straddtime = "?time=" + DateTime.Now.Ticks;

        if (PatchUrl != null)
        {
            return PatchUrl + straddtime;
        }

#if _MASTER_BUILD
        PatchUrl = GetS3BasePath() + "/patch/500.json";
#else
        switch (GetApiEnv())
        {
            default:
                PatchUrl = GetS3BasePath() + "/patch/500.json";
                break;
        }
#endif

        return PatchUrl + straddtime;
    }

    public static string GetScratchIconhUrl()
    {
        if (ScratchIconhUrl != null)
        {
            return ScratchIconhUrl;
        }

#if _MASTER_BUILD
        ScratchIconhUrl = GetResourceBasePath() + "/scratch/{0}/icon.png";
#else
        switch (GetApiEnv())
        {
            default:
                ScratchIconhUrl = GetResourceBasePath() + "/scratch/{0}/icon.png";
                break;
        }
#endif

        return ScratchIconhUrl;
    }

    public static string GetScratchBoadhUrl()
    {
        if (ScratchBoadhUrl != null)
        {
            return ScratchBoadhUrl;
        }

#if _MASTER_BUILD
        ScratchBoadhUrl = GetResourceBasePath() + "/scratch/{0}/scratchbanner.png";
#else
        switch (GetApiEnv())
        {
            default:
                ScratchBoadhUrl = GetResourceBasePath() + "/scratch/{0}/scratchbanner.png";
                break;
        }
#endif

        return ScratchBoadhUrl;
    }

    /// <summary>
    /// 汎用マスタ画像のURLを取得する
    /// </summary>
    /// <returns>urlの文字列「http～/{0}(ディレクトリ名)/{1}(識別子)/{2}(画像ファイル名)」</returns>
    public static string GetMasterResourcesUrl()
    {
        if (MasterResourcesUrl != null)
        {
            return MasterResourcesUrl;
        }

#if _MASTER_BUILD
        MasterResourcesUrl = GetResourceBasePath() + "/{0}/{1}/{2}";
#else
        switch (GetApiEnv())
        {
            default:
                MasterResourcesUrl = GetResourceBasePath() + "/{0}/{1}/{2}";
                break;
        }
#endif

        return MasterResourcesUrl;
    }

    /// <summary>
    /// イベントスケジュールのバナーのURLを取得する
    /// </summary>
    /// <returns></returns>
    public static string GetEventScheduleBannerUrl()
    {
        string url = GetBaseBannerUrl() + "/topic_images/";
        return url;
    }

    public static string GetSqliteUrl()
    {
        string strUrl = null;
        string currentBasePath = "/sqlite/" + SERVER_API_CURRENT_VER + "/master.p2.bytes";

        // Patchでreviewへの切り替え
        // DVGAN-2130 参照
        if (Patcher.Instance.isNextVersion())
        {
            strUrl = S3_PROD_BASE_ADDRESS + "/review" + currentBasePath;
            return strUrl;
        }

#if _MASTER_BUILD
        strUrl = S3_PROD_BASE_ADDRESS + "/prod" + currentBasePath;
#else
        string oldBasePath = "/sqlite/" + SERVER_API_OLD_VER + "/master.p2.bytes";
        string nextBasePath = "/sqlite/" + SERVER_API_NEXT_VER + "/master.p2.bytes";

        switch (GetApiEnv())
        {
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_IP_GOE:
                strUrl = S3_DEV_BASE_ADDRESS + "/dev1" + currentBasePath;
                break;
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_0_GOE:
                strUrl = S3_DEV_BASE_HTTP_ADDRESS + "/dev0" + currentBasePath;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_0_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_0_IP_GOE:
                strUrl = S3_DEV_BASE_ADDRESS + "/stg0" + currentBasePath;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_1_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_1_IP_GOE:
                strUrl = S3_DEV_BASE_ADDRESS + "/stg1" + currentBasePath;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_2a_GOE:
                strUrl = S3_DEV_BASE_HTTP_ADDRESS + "/stg2a" + currentBasePath;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_2b_GOE:
                strUrl = S3_DEV_BASE_HTTP_ADDRESS + "/stg2b" + currentBasePath;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_2c_GOE:
                strUrl = S3_DEV_BASE_HTTP_ADDRESS + "/stg2c" + currentBasePath;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_3a_GOE:
            case ApiEnv.API_TEST_ADDRESS_LOCAL_GOE:
                strUrl = S3_DEV_BASE_HTTP_ADDRESS + "/stg3a" + currentBasePath;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_3b_GOE:
                strUrl = S3_DEV_BASE_HTTP_ADDRESS + "/stg3b" + currentBasePath;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_3c_GOE:
                strUrl = S3_DEV_BASE_HTTP_ADDRESS + "/stg3c" + currentBasePath;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_IP_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_REHEARSAL_GOE:
                strUrl = S3_PROD_BASE_ADDRESS + "/review" + currentBasePath;
                break;
            case ApiEnv.API_TEST_ADDRESS_ONLINE:
                strUrl = S3_PROD_BASE_ADDRESS + "/prod" + currentBasePath;
                break;
        }
#endif
        return strUrl;
    }

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    public static string GetApiVersion()
    {
        string strVersion = null;
#if _MASTER_BUILD
        strVersion = SERVER_API_CURRENT_VER;
#else
        switch (GetApiEnv())
        {
            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_IP_GOE:
            case ApiEnv.API_TEST_ADDRESS_ONLINE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_0_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_0_IP_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_1_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_1_IP_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_2a_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_2b_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_2c_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_REHEARSAL_GOE:
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_0_GOE:
            case ApiEnv.API_TEST_ADDRESS_LOCAL_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_3a_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_3b_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_3c_GOE:
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_IP_GOE:
                strVersion = SERVER_API_CURRENT_VER;
                break;
        }
#endif
        return strVersion;
    }

    public static SERVER_TYPE GetServerType()
    {
        SERVER_TYPE type = SERVER_TYPE.prod;

        if (Patcher.Instance.isNextVersion())
        {
            type = SERVER_TYPE.review;
        }

#if _MASTER_BUILD

#else
        switch (GetApiEnv())
        {
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_IP_GOE:
                type = SERVER_TYPE.dev1;
                break;
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_0_GOE:
                type = SERVER_TYPE.dev0;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_0_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_0_IP_GOE:
                type = SERVER_TYPE.stg0;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_1_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_1_IP_GOE:
                type = SERVER_TYPE.stg1;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_2a_GOE:
                type = SERVER_TYPE.stg2a;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_2b_GOE:
                type = SERVER_TYPE.stg2b;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_2c_GOE:
                type = SERVER_TYPE.stg2c;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_3a_GOE:
                type = SERVER_TYPE.stg3a;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_3b_GOE:
                type = SERVER_TYPE.stg3b;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_3c_GOE:
                type = SERVER_TYPE.stg3c;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_IP_GOE:
                type = SERVER_TYPE.review;
                break;
            case ApiEnv.API_TEST_ADDRESS_ONLINE:
                type = SERVER_TYPE.prod;
                break;
            case ApiEnv.API_TEST_ADDRESS_STAGING_REHEARSAL_GOE:
                type = SERVER_TYPE.rehearsal;
                break;

            case ApiEnv.API_TEST_ADDRESS_LOCAL_GOE:
                type = SERVER_TYPE.local;
                break;
        }
#endif
        return type;
    }

    public static ApiEnv GetApiEnv()
    {
        return GetServerApiEnv(LocalSaveManager.Instance.LoadFuncServerAddressIP());
    }

    public static ApiEnv GetServerApiEnv(string servername)
    {
        foreach (ApiEnv env in Enum.GetValues(typeof(ApiEnv)))
        {
            if (env.GetIpAddress().Equals(servername))
            {
                return env;
            }
        }
        return ApiEnv.NONE;
    }

    public static string GetApplicationStatus(bool isTag)
    {
        string status = "";

#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG

        if (DebugOption.Instance.disalbeDebugMenu == true)
        {
            return status;
        }

        string apiversion = GlobalDefine.GetApiVersion();
        SQLiteClient.Instance.UpdateDbClientVersion();

        //サーバタイプ
        string createServer = SQLiteClient.Instance.GetDbServerType().ToString();

        //作成時間
        double unixTime = SQLiteClient.Instance.GetDbCreateTime();
        DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime date = unixStart.AddSeconds(unixTime).ToLocalTime();
        string createDate = " : " + date.ToString("yyyy/MM/dd HH:mm");

        string sizestart = isTag == true ? "<size=22>" : "";
        if (Patcher.Instance.isNextVersion())
        {
            sizestart = isTag == true ? "<size=26>" : "";
        }
        string sizeend = isTag == true ? "</size>" : "";
        string space = isTag == true ? "\n" : " / ";
        string colorstart = isTag == true ? "<color=#0000ff>" : "";
        if (Patcher.Instance.isNextVersion())
        {
            colorstart = isTag == true ? "<color=#ff0000>" : "";
        }
        string colorend = isTag == true ? "</color>" : "";

        string dbtext = createServer + createDate;
        if (isTag == true)
        {
            dbtext += "\n" + GetSqliteUrl();
        }

        Uri uri = new Uri(GetResourceBasePath());
        status = StrOpe.i + sizestart +
                                Application.productName +
                                sizeend +
                                space +
                                sizestart +
                                colorstart + ServerDataUtil.GetServerName() + colorend +
                                sizeend +
                                "\nAPI: " + apiversion +
                                "\nAsset: " + uri.Scheme + "://" + uri.Host +
                                "" + uri.AbsolutePath +
                                "\ndb: " + dbtext +
                                "\nuuid: " + LocalSaveManager.Instance.LoadFuncUUID();
#endif

        return status;
    }
}
