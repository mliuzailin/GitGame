/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MainMenuDefine.cs
	@brief	メインメニュー関連定義
	@author Developer
	@date 	2012/11/27
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
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@brief	基本リザルトコード
*/
//----------------------------------------------------------------------------
public enum MAINMENU_RESULT
{
    NONE,       //!< 基本リザルトコード：初期状態
    DOING,      //!< 基本リザルトコード：作業中
    FINISH,     //!< 基本リザルトコード：作業終了
};


//----------------------------------------------------------------------------
/*!
	@brief	メインメニューシーケンス
*/
//----------------------------------------------------------------------------
public enum MAINMENU_SEQ : int
{
    SEQ_NONE = -1,
    SEQ_HOME_MENU = 100,                    //!< ホーム画面

    SEQ_UNIT_MENU,                          //!< ユニットTOP
    SEQ_SHOP_MENU,                          //!< ショップTOP
    SEQ_HELP_MENU,                          //!< ヘルプTOP
    SEQ_FRIEND_MENU,                        //!< フレンドTOP
    SEQ_DEBUG_MENU,                         //!< デバッグTOP

    SEQ_LOGIN_BONUS,                        //!< ログインボーナス
    SEQ_BEGINNER_BOOST,                     //!< 初心者ブースト
    SEQ_DATE_CHANGE,                        //!< 日付変更

    SEQ_FRIEND_LIST = 200,                  //!< メインメニューシーケンス：フレンド枠：成立フレンドリスト
    SEQ_FRIEND_LIST_WAIT_ME,                //!< メインメニューシーケンス：フレンド枠：成立前フレンドリスト（申請受け中）
    SEQ_FRIEND_LIST_WAIT_HIM,               //!< メインメニューシーケンス：フレンド枠：成立前フレンドリスト（申請出し中）
    SEQ_FRIEND_SEARCH,                      //!< メインメニューシーケンス：フレンド枠：フレンド検索

    SEQ_HERO_FORM = 300,                    //!< 主人公選択
    SEQ_HERO_DETAIL,                        //!< 主人公選択：データ表示
    SEQ_HERO_PREVIEW,                       //!< 主人公選択：キャラ表示

    SEQ_UNIT_SELECT = 400,                  //!< メインメニューシーケンス：ユニット操作枠：共通ユニット選択画面
    SEQ_UNIT_PARTY_SELECT,                  //!< メインメニューシーケンス：ユニット操作枠：パーティ選択
    SEQ_UNIT_PARTY_FORM,                    //!< メインメニューシーケンス：ユニット操作枠：パーティ編成
    SEQ_UNIT_EVOLVE,                        //!< メインメニューシーケンス：ユニット操作枠：進化合成
    SEQ_UNIT_EVOLVE_RESULT,                 //!< メインメニューシーケンス：ユニット操作枠：進化合成演出
    SEQ_UNIT_BUILDUP,                       //!< メインメニューシーケンス：ユニット操作枠：強化合成
    SEQ_UNIT_BUILDUP_RESULT,                //!< メインメニューシーケンス：ユニット操作枠：強化合成演出
    SEQ_UNIT_LINK,                          //!< メインメニューシーケンス：ユニット操作枠：リンク
    SEQ_UNIT_LINK_RESULT,                   //!< メインメニューシーケンス：ユニット操作枠：リンク演出
    SEQ_UNIT_SALE,                          //!< メインメニューシーケンス：ユニット操作枠：売却
    SEQ_UNIT_LIST,                          //!< メインメニューシーケンス：ユニット操作枠：リスト
    SEQ_UNIT_CATALOG,                       //!< メインメニューシーケンス：ユニット操作枠：図鑑

    SEQ_QUEST_SELECT_OLD_AREA = 500,        //!< メインメニューシーケンス：クエスト枠：クエスト選択：旧エリア選択画面
    SEQ_QUEST_SELECT_AREA_STORY,            //!< メインメニューシーケンス：クエスト枠：クエスト選択：ストーリー(v300)
    SEQ_QUEST_SELECT_AREA_EVENT,            //!< メインメニューシーケンス：クエスト枠：クエスト選択：イベント(v300)
    SEQ_QUEST_SELECT_AREA_SCHOOL,           //!< メインメニューシーケンス：クエスト枠：クエスト選択：学校エリア選択画面（素材関連予定）
    SEQ_QUEST_SELECT,                       //!< メインメニューシーケンス：クエスト枠：クエスト選択：(v300)
    SEQ_QUEST_SELECT_DETAIL,                //!< メインメニューシーケンス：クエスト枠：クエスト詳細
    SEQ_QUEST_SELECT_FRIEND,                //!< メインメニューシーケンス：クエスト枠：フレンド選択
    SEQ_QUEST_SELECT_PARTY,                 //!< メインメニューシーケンス：クエスト枠：パーティ選択
    SEQ_QUEST_SERVER_SEND,                  //!< メインメニューシーケンス：クエスト枠：クエスト開始サーバーリクエスト

    SEQ_CHALLENGE_SELECT = 520,             //!< メインメニューシーケンス：クエスト枠：成長ボス選択

    SEQ_GACHA_MAIN = 600,                   //!< メインメニューシーケンス：ガチャ枠：ガチャ画面
    SEQ_GACHA_RESULT,                       //!< メインメニューシーケンス：ガチャ枠：ガチャ演出

    SEQ_SHOP_POINT = 700,                   //!< メインメニューシーケンス：ショップ枠：ポイントショップ
    SEQ_SHOP_POINT_LIMITOVER,               //!< メインメニューシーケンス：ショップ枠：ポイントショップ限界突破
    SEQ_SHOP_POINT_EVOLVE,                  //!< メインメニューシーケンス：ショップ枠：ポイントショップ進化

    SEQ_OTHERS_WEB = 800,                   //!< メインメニューシーケンス：アザーズ枠：リンク
    SEQ_OTHERS_HELP,                        //!< メインメニューシーケンス：アザーズ枠：ヘルプ
    SEQ_OTHERS_KIYAKU,                      //!< メインメニューシーケンス：アザーズ枠：規約/その他
    SEQ_OTHERS_USER,                        //!< メインメニューシーケンス：アザーズ枠：ユーザー操作
    SEQ_OTHERS_SUPPORT,                     //!< メインメニューシーケンス：アザーズ枠：サポート
    SEQ_OTHERS_MOVIE,                       //!< メインメニューシーケンス：アザーズ枠：ムービー
    SEQ_OTHERS_USER_RENAME = 820,           //!< メインメニューシーケンス：アザーズ枠：ユーザー操作：名称変更
    SEQ_OTHERS_TIP,                         //!< メインメニューシーケンス：アザーズ枠：購入履歴：チップを確認
    SEQ_OTHERS_USER_RENEW,                  //!< メインメニューシーケンス：アザーズ枠：データ削除
    SEQ_OTHERS_TRANSFER,                    //!< メインメニューシーケンス：アザーズ枠：バックアップ

    SEQ_RESULT_SERVER_SEND = 900,          //!< メインメニューシーケンス：リザルト枠：リザルト前サーバー反映処理
    SEQ_RESULT_RETIRE,                      //!< メインメニューシーケンス：リザルト枠：リタイア時リザルト処理
    SEQ_RESULT_INFO,                        //!< メインメニューシーケンス：リザルト枠：インフォメーション

    SEQ_TUTORIAL_HERO_SELECT = 1000,        //!< メインメニューシーケンス：チュートリアル枠：主人公選択
    SEQ_TUTORIAL_NAME,                      //!< メインメニューシーケンス：チュートリアル枠：名前入力

    SEQ_DEBUG_UNIT_GET = 2000,              //!< メインメニューシーケンス：デバッグ枠：ユニット取得
    SEQ_DEBUG_REPLACE_PARTY_UNIT,           //!< メインメニューシーケンス：デバッグ枠：1stパーティユニット置き換え
    SEQ_DEBUG_CHARA_CUTIN_CHK,              //!< メインメニューシーケンス：デバッグ枠：キャラカットイン確認
    SEQ_DEBUG_PARTY_OFFSET,                 //!< メインメニューシーケンス：デバッグ枠：PTオフセット調整
    SEQ_DEBUG_CHARA_MESH_CHK,               //!< メインメニューシーケンス：デバッグ枠：キャラメッシュ確認
    SEQ_DEBUG_DISP_ENEMY_CHK,               //!< メインメニューシーケンス：デバッグ枠：エネミー表示調整
    SEQ_DEBUG_MASTERDATA_CHK,               //!< メインメニューシーケンス：デバッグ枠：マスターデータ確認
    SEQ_DEBUG_QUEST_CLEAR,                  //!< メインメニューシーケンス：デバッグ枠：クエストクリア情報改変
    SEQ_DEBUG_USER_RANKUP,                  //!< メインメニューシーケンス：デバッグ枠：ユーザー情報変更
    SEQ_DEBUG_STORY_VIEW_OFFSET,            //!< メインメニューシーケンス：デバッグ枠：ストーリー画面オフセット調整
    SEQ_DEBUG_FORCE_FRIEND_UNIT,            //!< メインメニューシーケンス：デバッグ枠：フレンドユニット置き換え
    SEQ_DEBUG_BG_VIEW,                      //!< メインメニューシーケンス：デバッグ枠：エリア選択背景閲覧
    SEQ_DEBUG_QUEST_BG_VIEW,                //!< メインメニューシーケンス：デバッグ枠：バトル背景閲覧
    SEQ_DEBUG_MAP_ICON_VIEW,                //!< メインメニューシーケンス：デバッグ枠：マップアイコン閲覧
    SEQ_DEBUG_GENERAL_WINDOW_VIEW,          //!< メインメニューシーケンス：デバッグ枠：汎用ウィンドウ確認
    SEQ_DEBUG_RICH_TEXT,                    //!< メインメニューシーケンス：デバッグ枠：リッチテキスト確認


    //旧ページ遷移宣言（消去したい）
    SEQ_SWITCH_PAGE,                        //!< メインメニューシーケンス：特別枠：次の遷移先分岐ページ
    SEQ_ACHIEVEMENT_CLEAR,                  //!< メインメニューシーケンス：アチーブメント関連：新規達成
    SEQ_QUEST_SELECT_ACHIEVEMENT_GP_LIST,   //!< メインメニューシーケンス：クエスト枠：ミッショングループ(v350)

    SEQ_MAX,                                //!< メインメニューシーケンス：
};


//----------------------------------------------------------------------------
/*!
	@brief	メニューカテゴリ
*/
//----------------------------------------------------------------------------
public enum MAINMENU_CATEGORY : int
{
    NONE = -1,
    UNIT,
    FRIEND,
    SHOP,
    HELP,
    HOME,
    GACHA,
    QUEST,

    DEBUG,

    MAX,
};

//----------------------------------------------------------------------------
/*!
	@brief	メニューボタンタイプ
*/
//----------------------------------------------------------------------------
public enum MAINMENU_BUTTON : int
{
    NONE = -1,

    UNIT_PARTY_ASSIGN = 100,
    UNIT_HERO_SELECT,
    UNIT_BUILDUP,
    UNIT_EVOLUTION,
    UNIT_LINK,
    UNIT_SALE,
    UNIT_LIST,
    UNIT_BOOK,
    UNIT_POINT_LIMITOVER,
    UNIT_POINT_EVOLUTION,

    FRIEND_LIST = 200,
    FRIEND_LIST_WAIT_HIM,
    FRIEND_LIST_WAIT_ME,
    FRIEND_SEARCH,

    SHOP_CHIP = 300,
    SHOP_UNIT_EXTEND,
    SHOP_FRIEND_EXTEND,
    SHOP_STAMINA_RECOVERY,
    SHOP_POINT,

    HELP_WEBSITE = 400,
    HELP_TECHNIQUE,
    HELP_KIYAKU,
    HELP_SUPPORT,
    HELP_USER,
    HELP_MOVIE,
    HELP_DEBUG,

    DEBUB_CHANGE_USER_PARAM = 900,
    DEBUG_GET_UNIT,
    DEBUG_FORCE_PARTY,
    DEBUG_CHARA_CUTIN_CHK,
    DEBUG_PARTY_OFFSET,
    DEBUG_CHARA_MESH_CHK,
    DEBUG_DISP_ENEMY_CHK,

    DEBUG_MASTERDATA_CHK,
    DEBUG_QUEST_CLEAR,
    DEBUG_USER_RANK_UP,
    DEBUG_STORY_VIEW_OFFSET,
    DEBUG_FORCE_FRIEND_UNIT,
    DEBUG_BG_VIEW,
    DEBUG_QUEST_BG_VIEW,
    DEBUG_MAP_ICON_VIEW,
    DEBUG_GENERAL_WINDOW_VIEW,
    DEBUG_RICH_TEXT,
};

//----------------------------------------------------------------------------
/*!
	@brief	ソート項目
*/
//----------------------------------------------------------------------------
public enum MAINMENU_SORT_SEQ : int
{
    SEQ_INIT,                               //!< ソートタイプ：初期化枠

    SEQ_SORT_TYPE_FILTER,                   //!< ソートタイプ：お好み（フィルタ）

    SEQ_SORT_TYPE_ATTACK,                   //!< ソートタイプ：攻撃力
    SEQ_SORT_TYPE_FAVORITE,                 //!< ソートタイプ：お気に入り
    SEQ_SORT_TYPE_COST,                     //!< ソートタイプ：コスト
    SEQ_SORT_TYPE_HP,                       //!< ソートタイプ：体力
    SEQ_SORT_TYPE_ELEMENT,                  //!< ソートタイプ：属性
    SEQ_SORT_TYPE_ID,                       //!< ソートタイプ：ID
    SEQ_SORT_TYPE_GET,                      //!< ソートタイプ：入手順
    SEQ_SORT_TYPE_KIND,                     //!< ソートタイプ：種族
    SEQ_SORT_TYPE_PLUS,                     //!< ソートタイプ：プラス値
    SEQ_SORT_TYPE_DEFAULT,                  //!< ソートタイプ：デフォルト
    SEQ_SORT_TYPE_LOGIN_TIME,               //!< ソートタイプ：ログイン順
    SEQ_SORT_TYPE_RANK,                     //!< ソートタイプ：ランク
    SEQ_SORT_TYPE_WAIT_HIM,                 //!< ソートタイプ：申請出順
    SEQ_SORT_TYPE_WAIT_ME,                  //!< ソートタイプ：申請受順
    SEQ_SORT_TYPE_LEVEL,                    //!< ソートタイプ：レベル
    SEQ_SORT_TYPE_RARE,                     //!< ソートタイプ：レア度
    SEQ_SORT_TYPE_MISSION_CLEAR,            //!< ソートタイプ：ミッション達成済み（v300ミッション用）
    SEQ_SORT_TYPE_MISSION_CHALLENGING,      //!< ソートタイプ：ミッション挑戦中（v300ミッション用）
    SEQ_SORT_TYPE_MISSION_ACQUIRED,         //!< ソートタイプ：ミッション報酬取得済み（v300ミッション用）
    SEQ_SORT_TYPE_LIMIT_OVER,               //!< ソートタイプ：限界突破(v340限界突破用)
    SEQ_SORT_TYPE_MISSION_GP_GET_REWARD,    //!< ソートタイプ：GET REWARD：ミッション報酬未取得あり（v360ミッショングループ用）
    SEQ_SORT_TYPE_MISSION_GP_NEW,           //!< ソートタイプ：NEW：ミッション未達成ミッションあり（v360ミッショングループ用）
    SEQ_SORT_TYPE_MISSION_GP_ACQUIRED,      //!< ソートタイプ：CLEAR：ミッション全て報酬取得済み（v360ミッショングループ用）
    SEQ_SORT_TYPE_CHARM,                    //!< ソートタイプ：チャーム

    SEQ_SORT_TYPE_FAVORITE_SORT,            //!< ソートタイプ：お好みソート

    SEQ_SORT_TYPE_MAX,                      //!< ソートタイプ：MAX
}

public enum MAINMENU_FILTER_TYPE
{
    FILTER_INIT,
    FILTER_RARE,
    FILTER_ELEMENT,
    FILTER_KIND,
    FILTER_MAX,
}

//----------------------------------------------------------------------------
/*!
	@brief	パラメータリミットエラータイプ
*/
//----------------------------------------------------------------------------
public enum PRM_LIMIT_ERROR_TYPE
{
    PRM_LIMIT_CHECK_OK,                     //!< パラメータリミットエラー：エラーなし
    PRM_LIMIT_ERR_COIN,                     //!< パラメータリミットエラー：コイン
    PRM_LIMIT_ERR_TICKET,                   //!< パラメータリミットエラー：チケット
    PRM_LIMIT_ERR_FP,                       //!< パラメータリミットエラー：フレンドポイント
    PRM_LIMIT_ERR_TIP,                      //!< パラメータリミットエラー：チップ
    PRM_LIMIT_ERR_UNITPOINT,                //!< パラメータリミットエラー：ユニットポイント
    PRM_LIMIT_ERR_ITEM,                     //!< パラメータリミットエラー：アイテム
    PRM_LIMIT_ERR_QUEST_KEY,                //!< パラメータリミットエラー：クエストキー
    PRM_LIMIT_ERR_OTHER,                    //!< パラメータリミットエラー：複数
};

//----------------------------------------------------------------------------
/*!
	@brief	使用アイテム種類
*/
//----------------------------------------------------------------------------
public enum MAINMENU_USE_ITEM : int
{
    USE_ITEM_INIT,                      //!< 使用アイテム：初期化枠
    USE_ITEM_STAMINA,                   //!< 使用アイテム：スタミナ回復関連アイテム

    USE_ITEM_MAX,                       //!< ソートタイプ：MAX
}

//----------------------------------------------------------------------------
/*!
	@brief	アトラス種類
*/
//----------------------------------------------------------------------------
public enum MAINMENU_ATLAS_TYPE : int
{
    MAINMENU,

    MAX,
}

//----------------------------------------------------------------------------
/*!
	@brief	ユニット選択タイプ
*/
//----------------------------------------------------------------------------
public enum MAINMENU_UNIT_SELECT_TYPE : int
{
    NONE = -1,
    BILDUP = 0,
    EVOLVE,
    LINK_BASE,
    LINK_TARGET,

    MAX,
}

//----------------------------------------------------------------------------
/*!
	@brief	日付変更タイプ
*/
//----------------------------------------------------------------------------
public enum DATE_CHANGE_TYPE : int
{
    NONE = -1,
    RETURN_TITLE = 0,       //!< タイトルへ戻る
    LOGIN = 1,              //!< ログイン処理
    DAY_STRADDLE = 2,       //!< 日跨ぎ処理

    MAX,
}


/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
static public class MainMenuDefine
{

    //----------------------------------------------------------------------------
    /*!
		@brief	ページ表示範囲（スクロール範囲）定義
	*/
    //----------------------------------------------------------------------------
    public const int PAGE_OTHERS_HEIGHT = 420;      //!< スクロール範囲：縦：Othersページ
    public const int PAGE_OTHERS_EVENTLIST_HEIGHT = 420;        //!< スクロール範囲：縦：Others：イベントスケジュール表
    public const int PAGE_QUEST_LIST_HEIGHT = 250;      //!< スクロール範囲：縦：Questページ（エリアカテゴリ、エリア、クエスト）
    public const int PAGE_QUEST_MISSION_GP_HEIGHT = 420;        //!< スクロール範囲：縦：Quest：ミッショングループページ
    public const int PAGE_QUEST_MISSION_HEIGHT = 420;       //!< スクロール範囲：縦：Quest：ミッションページ
    public const int PAGE_QUEST_ITEM_HEIGHT = 420;      //!< スクロール範囲：縦：Quest：アイテムページ
    public const int PAGE_QUEST_FRIENDLIST_HEIGHT = 220;        //!< スクロール範囲：縦：Quest：フレンド選択ページ
    public const int PAGE_FRIEND_LIST_HEIGHT = 420;     //!< スクロール範囲：縦：Friendページ
    public const int PAGE_UNIT_LIST_HEIGHT = 220;       //!< スクロール範囲：縦：Unitページ：ユニットリスト
    public const int PAGE_UNIT_LIST_HEIGHT2 = 420;      //!< スクロール範囲：縦：Unitページ：ユニットリスト２
    public const int PAGE_UNIT_FRIENDLIST_HEIGHT = 220;     //!< スクロール範囲：縦：Unitページ：フレンドリスト
    public const int PAGE_RESULT_LIST_HEIGHT = 220;     //!< スクロール範囲：縦：Resultページ：取得ユニットリスト
    public const int PAGE_UNIT_LIST_SCRATCH = 420;      //!< スクロール範囲：縦：Scratchページ
    public const int PAGE_SHOP_POINTSHOPLIST_HEIGHT = 420;      //!< スクロール範囲：縦：Shopページ：ポイントショップリスト
    public const int PAGE_SHOP_POINTSHOPLO_HEIGHT = 348;        //!< スクロール範囲：縦：Shopページ：ポイントショップ限界突破ユニットリスト
    public const int PAGE_QUEST_ITEM_EFFECT_HEIGHT = 210;       //!< スクロール範囲：縦：Quest：アイテム効果ダイアログ

    public const int PAGE_FOOTER_CHILD_MENU = 624;      //!< スクロール範囲：横：フッターメニュー

    //----------------------------------------------------------------------------
    /*!
		@brief	リスト：1ページに表示する要素数定義
	*/
    //----------------------------------------------------------------------------
    public const int PAGE_ACHIEVEMENT_GP_NUM = 50;      //!< ミッショングループ数 ※この件数はサーバーと連動しているので注意。修正する場合サーバー対応必要
    public const int PAGE_ACHIEVEMENT_NUM = 50;     //!< ミッション数 ※この件数はサーバーと連動しているので注意。修正する場合サーバー対応必要
    public const int PAGE_ITEM_NUM = 50;        //!< アイテム数
    public const int PAGE_UNIT_NUM = 100;       //!< ユニット数
    public const int PAGE_FRIEND_NUM = 100;     //!< フレンド数
    public const int PAGE_POINTSHOP_PRODUCT_NUM = 50;       //!< ポイントショップ商品数


    //----------------------------------------------------------------------------
    /*!
		@brief	テキストの表示限界文字数定義(全角での文字数を設定)
	*/
    //----------------------------------------------------------------------------
    public const int TEXT_LIMIT_NUM_AREACATE_NAME = 6;      //!< 表示限界文字数：クエスト選択：エリアカテゴリ名
    public const int TEXT_LIMIT_NUM_AREA_NAME = 9;      //!< 表示限界文字数：クエスト選択：エリア名
    public const int TEXT_LIMIT_NUM_QUEST_NAME = 9;     //!< 表示限界文字数：クエスト選択：クエスト名
                                                        //public const int TEXT_LIMIT_NUM_MISSION_MSG			= 18;		//!< 表示限界文字数：ミッション：ミッションラベル

    //----------------------------------------------------------------------------
    /*!
		@brief	ソートタイプ
	*/
    //----------------------------------------------------------------------------
    public const int SORTTYPE_FILTER = 99;      //!< ソートタイプ：お好み（フィルタ）

    //----------------------------------------------------------------------------
    /*!
		@brief	ソート：お好み(フィルタ)の優先項目
	*/
    //----------------------------------------------------------------------------
    public const int SORT_FILTER_PRIORITY_ATK = 1;      //!< ソート：フィルタ優先：攻撃力
    public const int SORT_FILTER_PRIORITY_HP = 2;       //!< ソート：フィルタ優先：体力
    public const int SORT_FILTER_PRIORITY_LV = 3;       //!< ソート：フィルタ優先：レベル
    public const int SORT_FILTER_PRIORITY_RARE = 4;     //!< ソート：フィルタ優先：レア度
    public const int SORT_FILTER_PRIORITY_MAX = 4;      //!< ソート：フィルタ優先：

    // 使わないのでコメントアウト 代わりにMasterDataDefineLabel.ElementTypeを使用する Developer 2016/11/07
    ///// <summary>
    ///// ページ固有フィルタの種類
    ///// </summary>
    //public enum FILTER_ELEMENT_TYPE {
    //    FIRE = 0,   //!< 属性：火
    //    WATER,      //!< 属性：水
    //    WIND,       //!< 属性：風
    //    LIGHT,      //!< 属性：光
    //    DARK,       //!< 属性：闇
    //    NAUGHT,     //!< 属性：無
    //    ALL,        //!< ALL
    //    MAX,        //!<
    //}

    //----------------------------------------------------------------------------
    /*!
        @brief	演出：強化、進化、リンク
    */
    //----------------------------------------------------------------------------
    public const string ANIM_FIX_UNIT_IN = ("CutinBaseCharaIn");        //!< 固定キャラカットイン：アニメーション：開始
    public const string ANIM_FIX_UNIT_OUT = ("CutinBaseCharaOut");      //!< 固定キャラカットイン：アニメーション：終了
    public const string ANIM_COST_UNIT_IN = ("CutinLinkCharaIn");       //!< 消費キャラカットイン：アニメーション：開始
    public const string ANIM_COST_UNIT_OUT = ("CutinLinkCharaOut");     //!< 消費キャラカットイン：アニメーション：終了
    public const float UPDATE_WAIT_TIME = (0.3f);                       //!< 更新待ち時間
    public const int CUTIN_OBJ_TYPE_FIX = 0;                            //!< オブジェクトタイプ：固定キャラ
    public const int CUTIN_OBJ_TYPE_COST = 1;                           //!< オブジェクトタイプ：消費キャラ
    public const string UNIT_DECIDE_BUTTON_BLOCK_TAG = ("UnitDecide");  //!<

    //----------------------------------------------------------------------------
    /*!
		@brief	演出：スクラッチ
	*/
    //----------------------------------------------------------------------------
    public const int GACHA_TITLE_CHANGE_RARE5_OVER_NUM = 7;         //!< ガチャ演出条件：タイトル変更条件：レア度5以上のユニット排出数
}