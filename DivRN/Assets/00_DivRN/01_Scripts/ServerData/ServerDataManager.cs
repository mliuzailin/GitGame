#if BUILD_TYPE_DEBUG
//	#define DEF_API_CRASHPOINT		//!< サーバー通信時に強制クラッシュを行なうか否かのダイアログを表示
#endif

#define USE_DICTIONALY // @Change Developer 2015/11/06 warning除去。HashTableの使用は非推奨になっているため、Dictionaryを使用する。

/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	ServerDataManager.cs
	@brief	サーバーデータ管理クラス
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

using ServerDataDefine;

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@brief	通信パケット情報
*/
//----------------------------------------------------------------------------
public class PacketDataResult
{
    public uint m_PacketUniqueNum;      //!< パケット情報：パケット固有ユニークID
    public SERVER_API m_PacketAPI;          //!< パケット情報：パケットAPIタイプ

    //	public	API_RESULT		m_PacketResult;			//!< パケットリザルト情報：総合結果
    public API_CODE m_PacketCode;			//!< パケットリザルト情報：リザルトコード
};

//----------------------------------------------------------------------------
/*!
	@brief	APIリザルトコード区分情報
*/
//----------------------------------------------------------------------------
public class ResultCodeType
{
    public API_CODETYPE m_CodeType;         //!< APIリザルトコード区分情報：コード区分
    public API_CODE m_Code;             //!< APIリザルトコード区分情報：コード

    public ResultCodeType(API_CODE eCode, API_CODETYPE eCodeType)
    {
        m_Code = eCode;
        m_CodeType = eCodeType;
    }
};

//----------------------------------------------------------------------------
/*!
	@brief	APIリザルトコード区分情報：イレギュラー
	@note	特定のAPIの場合にイレギュラーでコード分岐
*/
//----------------------------------------------------------------------------
public class ResultCodeTypeIrregular
{
    public SERVER_API m_APIType;            //!< APIリザルトコード区分情報：APIタイプ
    public ResultCodeType m_ResultCodeType; //!< APIリザルトコード区分情報：コード区分

    public ResultCodeTypeIrregular(SERVER_API eAPI, API_CODE eCode, API_CODETYPE eCodeType)
    {
        m_APIType = eAPI;
        m_ResultCodeType = new ResultCodeType(eCode, eCodeType);
    }
};

//----------------------------------------------------------------------------
/*!
	@brief	通信パケット情報
*/
//----------------------------------------------------------------------------
public class PacketData
{
    public SERVER_API m_PacketAPI;          //!< パケット情報：APIタイプ
    public string m_PacketSendMessage;  //!< パケット情報：パケット送信文字列	※Send～の構造体のJSon化文字列
    public uint m_PacketUniqueNum;      //!< パケット情報：パケット固有ユニークID

    public bool m_CommunicateFinish;	//!< パケット動作：処理完遂フラグ
};

//----------------------------------------------------------------------------
/*!
	@brief	サーバーデータ管理クラス
*/
//----------------------------------------------------------------------------
public class ServerDataManager : SingletonComponent<ServerDataManager>
{
    const int COMMUNICATE_MAX = 100;                                    //!< 通信リクエスト保持最大数
                                                                        //	const		int					COMMUNICATE_MAX			= 5;									//!< 通信リクエスト保持最大数
                                                                        //	const		int					COMMUNICATE_RETRY_MAX	= 10;									//!< 通信リクエストリトライ回数上限
    const int COMMUNICATE_RETRY_MAX = 1;                                    //!< 通信リクエストリトライ回数上限

    const float DEF_LOADING_CLOSE_WAIT = 0.05f;                             //!< ダイアログを閉じるまでの待ち	※頻繁に開閉すると通信頻度が高く感じるので対応

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    private PacketData[] m_PacketList = new PacketData[COMMUNICATE_MAX];        //!< 通信リクエスト情報：リクエストID

    private uint m_CommunicateWorkInput = 1;                                        //!< 通信作業情報：入力する領域番号	※０番はエラーコードで使うので１から
    private uint m_CommunicateWorkAccess = 1;                                       //!< 通信作業情報：処理する領域番号	※０番はエラーコードで使うので１から
    private uint m_CommunicateWorkFinish = 1;                                       //!< 通信作業情報：処理した領域番号	※０番はエラーコードで使うので１から
                                                                                    //	private		uint				m_CommunicateUniID		= 0;										//!< 通信作業情報：パケットユニークID

    private PacketData m_CoroutinePacket = null;                                        //!< 通信作業情報：コルーチン処理中パケット

    private PacketDataResult[] m_PacketResult = new PacketDataResult[COMMUNICATE_MAX];  //!< 通信結果情報：リザルト情報
    private uint m_PacketResultInput = 0;                                       //!< 通信結果情報：リザルト情報アクセス番号

    private Dialog m_ServerDialog = null;                                       //!< 通信中ダイアログ
                                                                                //	private		SERVER_API			m_ServerDialogAPI		= SERVER_API.SERVER_API_MAX;
                                                                                //	private		float				m_ServerDialogStartTime	= 0.0f;
                                                                                //	private		float				m_ServerDialogCloseTime	= 0.0f;

    private TemplateList<ResultCodeType> m_ResultCodeType = null;
    private TemplateList<ResultCodeTypeIrregular> m_ResultCodeTypeIrregular = null;


    private bool m_LoadingClip = false;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();

        //--------------------------------
        // パケット領域確保
        //--------------------------------
        for (int i = 0; i < COMMUNICATE_MAX; i++)
        {
            m_PacketList[i] = new PacketData();
            m_PacketList[i].m_PacketUniqueNum = 0;
            m_PacketList[i].m_CommunicateFinish = false;
        }

        //--------------------------------
        // リザルト領域確保
        //--------------------------------
        for (int i = 0; i < COMMUNICATE_MAX; i++)
        {
            m_PacketResult[i] = new PacketDataResult();
            m_PacketResult[i].m_PacketUniqueNum = 0;
        }


        //----------------------------------------------------------------------------
        // リザルトコード区分表を生成
        //----------------------------------------------------------------------------
        {
            //----------------------------------------------------------------------------
            // 成功
            //----------------------------------------------------------------------------
            ResultCodeAdd(API_CODE.API_CODE_SUCCESS, API_CODETYPE.API_CODETYPE_USUALLY);    // 汎用系：成功

            //----------------------------------------------------------------------------
            // 自動再送系
            //----------------------------------------------------------------------------								  
            ResultCodeAdd(API_CODE.API_CODE_USER_AUTH_REQUIRED, API_CODETYPE.API_CODETYPE_AUTH_REQUIRED);   // ユーザー認証：再認証が必要
            ResultCodeAdd(API_CODE.API_CODE_USER_AUTH_REQUIRED2, API_CODETYPE.API_CODETYPE_AUTH_REQUIRED);  // ユーザー認証：再認証が必要

            //----------------------------------------------------------------------------
            // 自動再送系
            // デッドロックにユーザー認証は必要ないが、処理分岐を簡易的に済ますために再送処理に乗っける
            //----------------------------------------------------------------------------								  
            ResultCodeAdd(API_CODE.API_CODE_WIDE_DEAD_LOCK, API_CODETYPE.API_CODETYPE_AUTH_REQUIRED);   // ユーザー認証：APIデッドロック

            //----------------------------------------------------------------------------
            // アプリ強制停止系（復旧不可）
            // ※サーバー内部でダイアログを出してボタン押したら強制終了
            //----------------------------------------------------------------------------																  
            ResultCodeAdd(API_CODE.API_CODE_WIDE_ERROR_SYSTEM, API_CODETYPE.API_CODETYPE_RESTART);  // 汎用エラー：システムエラー系
            ResultCodeAdd(API_CODE.API_CODE_WIDE_ERROR_DB, API_CODETYPE.API_CODETYPE_RESTART);  // 汎用エラー：データベースエラー
            ResultCodeAdd(API_CODE.API_CODE_WIDE_ERROR_MASTER, API_CODETYPE.API_CODETYPE_RESTART);  // 汎用エラー：Masterエラー
            ResultCodeAdd(API_CODE.API_CODE_WIDE_ERROR_SERVER, API_CODETYPE.API_CODETYPE_RESTART);  // 汎用エラー：サーバーイレギュラーエラー
            ResultCodeAdd(API_CODE.API_CODE_CLIENT_PARSE_FAILED, API_CODETYPE.API_CODETYPE_RESTART);    // 汎用エラー：返信解析エラー
            ResultCodeAdd(API_CODE.API_CODE_CLIENT_PARSE_FAILED_CODE, API_CODETYPE.API_CODETYPE_RESTART);   // 汎用エラー：返信解析エラー（リザルトコード不正）
            ResultCodeAdd(API_CODE.API_CODE_CLIENT_CSUM_ERROR, API_CODETYPE.API_CODETYPE_RESTART);  // 汎用エラー：チェックサム不一致


            //----------------------------------------------------------------------------
            // 不正ユーザーにつき停止
            //----------------------------------------------------------------------------																	  
            ResultCodeAdd(API_CODE.API_CODE_WIDE_ERROR_USER_INVALID, API_CODETYPE.API_CODETYPE_INVALID_USER);   // 汎用エラー：不正ユーザー認定

            //----------------------------------------------------------------------------
            // ユーザー一時凍結中
            //----------------------------------------------------------------------------																	  
            ResultCodeAdd(API_CODE.API_CODE_WIDE_ERROR_USER_FREEZE, API_CODETYPE.API_CODETYPE_INVALID_USER);    // 汎用エラー：一時凍結

            //----------------------------------------------------------------------------
            // バージョンアップ示唆
            //----------------------------------------------------------------------------										  
            ResultCodeAdd(API_CODE.API_CODE_WIDE_ERROR_VERSION, API_CODETYPE.API_CODETYPE_VERSION_UP);  // 汎用エラー：バージョン不一致
            ResultCodeAdd(API_CODE.API_CODE_WIDE_ERROR_VERSION2, API_CODETYPE.API_CODETYPE_VERSION_UP); // 汎用エラー：バージョン不一致

            //----------------------------------------------------------------------------
            // アプリ強制停止系（任意タイミング復帰）
            // ※サーバー内部でダイアログを出してボタン入力待ち
            //----------------------------------------------------------------------------						  
            ResultCodeAdd(API_CODE.API_CODE_WIDE_ERROR_MENTENANCE, API_CODETYPE.API_CODETYPE_MENTENANCE);   // 汎用エラー：サーバーメンテナンス中

            //----------------------------------------------------------------------------
            // イベントストップ
            //----------------------------------------------------------------------------
            ResultCodeAdd(API_CODE.API_CODE_STOP_BUY_TIP, API_CODETYPE.API_CODETYPE_USUALLY);   // イベントストップ：チップ購入ストップステータス(停止)
            ResultCodeAdd(API_CODE.API_CODE_STOP_USE_TIP_CONTINUE, API_CODETYPE.API_CODETYPE_USUALLY);  // イベントストップ：チップ消費(コンティニュー)ストップステータス(停止)
            ResultCodeAdd(API_CODE.API_CODE_STOP_USE_TIP_RETRY, API_CODETYPE.API_CODETYPE_USUALLY); // イベントストップ：チップ消費(リトライ)ストップステータス(停止)
            ResultCodeAdd(API_CODE.API_CODE_STOP_USE_TIP_UNIT, API_CODETYPE.API_CODETYPE_USUALLY);  // イベントストップ：チップ消費(ユニット拡張)ストップステータス(停止)
            ResultCodeAdd(API_CODE.API_CODE_STOP_USE_TIP_FRIEND, API_CODETYPE.API_CODETYPE_USUALLY);    // イベントストップ：チップ消費(フレンド拡張)ストップステータス(停止)
            ResultCodeAdd(API_CODE.API_CODE_STOP_USE_TIP_STAMINA, API_CODETYPE.API_CODETYPE_USUALLY);   // イベントストップ：チップ消費(スタミナ回復)ストップステータス(停止)
            ResultCodeAdd(API_CODE.API_CODE_STOP_USE_TIP_PREMIUM, API_CODETYPE.API_CODETYPE_USUALLY);   // イベントストップ：チップ消費(プレミアム合成)ストップステータス(停止)
            ResultCodeAdd(API_CODE.API_CODE_STOP_USE_TIP_EVOL, API_CODETYPE.API_CODETYPE_USUALLY);  // イベントストップ：チップ消費(進化)ストップステータス(停止)
            ResultCodeAdd(API_CODE.API_CODE_STOP_AREA, API_CODETYPE.API_CODETYPE_USUALLY);  // イベントストップ：エリアストップステータス(停止)

            //----------------------------------------------------------------------------
            // API固有分岐系（複数API参照）
            //----------------------------------------------------------------------------
            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_SHORT_FP, API_CODETYPE.API_CODETYPE_USUALLY);  // 対価不足（フレンドポイント）
            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_SHORT_TIP, API_CODETYPE.API_CODETYPE_USUALLY); // 対価不足（チップ）
            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_SHORT_TIP2, API_CODETYPE.API_CODETYPE_USUALLY);    // 対価不足（チップ）
            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_SHORT_COIN, API_CODETYPE.API_CODETYPE_USUALLY);    // 対価不足（コイン）
            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_SHORT_STAMINA, API_CODETYPE.API_CODETYPE_USUALLY); // 対価不足（スタミナ）
            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_SHORT_STAMINA_MAX, API_CODETYPE.API_CODETYPE_USUALLY); // 対価不足（スタミナ最大値）
            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_SHORT_TICKET, API_CODETYPE.API_CODETYPE_USUALLY);  // 対価不足（チケット）

            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_UNIT_NONE, API_CODETYPE.API_CODETYPE_USUALLY); // [ クエスト , 進化 , 強化 , パーティ編成 , 売却	]：選択ユニットが存在しない
            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_UNIT_OWNER_ERR, API_CODETYPE.API_CODETYPE_USUALLY);    // [ クエスト , 進化 , 強化 , パーティ編成 , 売却	]：選択ユニットが他人のモノ
            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_UNIT_SAME, API_CODETYPE.API_CODETYPE_USUALLY); // [ クエスト , 進化 , 強化 , パーティ編成 , 売却	]：同一ユニットが複数選択されている
            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_UNIT_PARTY_ASSIGNED, API_CODETYPE.API_CODETYPE_USUALLY);   // [		  , 進化 , 強化 ,			   , 売却	]：選択ユニットがパーティアサインされている
            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_UNIT_EXCESS, API_CODETYPE.API_CODETYPE_USUALLY);   // [		  , 進化 , 強化 ,			   , 売却	]：選択ユニットが過剰に多い

            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_EVENT_ERR_FP, API_CODETYPE.API_CODETYPE_USUALLY);  // [ クエスト , 進化 , 強化 ,				,		]：一部イベントが終了している（フレンドポイント関連）
            ResultCodeAdd(API_CODE.API_CODE_WIDE_API_EVENT_ERR_SLV, API_CODETYPE.API_CODETYPE_USUALLY); // [		   ,      , 強化 ,				,		]：一部イベントが終了している（スキルレベルアップ関連）

            //----------------------------------------------------------------------------
            // API固有分岐系（一部API限定）
            //----------------------------------------------------------------------------
            ResultCodeAdd(API_CODE.API_CODE_USER_CREATE_UUID_ERR, API_CODETYPE.API_CODETYPE_USUALLY);   // ユーザー生成：UUID重複

            ResultCodeAdd(API_CODE.API_CODE_USER_RENAME_NG_WORD, API_CODETYPE.API_CODETYPE_USUALLY);    // ユーザー名称変更：禁止文字列使用

            ResultCodeAdd(API_CODE.API_CODE_FRIEND_SEARCH_NONE, API_CODETYPE.API_CODETYPE_USUALLY); // フレンド検索：該当ユーザー無し	

            ResultCodeAdd(API_CODE.API_CODE_GACHA_EVENT_ERR, API_CODETYPE.API_CODETYPE_USUALLY);    // ガチャ：ガチャイベント期間外
            ResultCodeAdd(API_CODE.API_CODE_GACHA_EVENT_ALREADY, API_CODETYPE.API_CODETYPE_USUALLY);    // ガチャ：既に引いている（ランチタイムガチャ想定）


            ResultCodeAdd(API_CODE.API_CODE_GACHA_STOP_STATUS_STOP, API_CODETYPE.API_CODETYPE_USUALLY); // ガチャ：ガチャストップステータス(停止)
            ResultCodeAdd(API_CODE.API_CODE_GACHA_STOP_STATUS_DISABLE, API_CODETYPE.API_CODETYPE_USUALLY);  // ガチャ：ガチャストップステータス(無効)
            ResultCodeAdd(API_CODE.API_CODE_GACHA_EVENT_OUTSIDE, API_CODETYPE.API_CODETYPE_USUALLY);    // ガチャ：ガチャ期間外
            ResultCodeAdd(API_CODE.API_CODE_GACHA_FIXID_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);  // ガチャ：ガチャIDエラー
            ResultCodeAdd(API_CODE.API_CODE_GACHA_INVALID_RANK_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);   // ガチャ：ガチャ不正対策
            ResultCodeAdd(API_CODE.API_CODE_STEP_UP_GACHA_REST_TIME_NOW, API_CODETYPE.API_CODETYPE_USUALLY);     //!< ガチャ：ステップ数がリセットされた

            ResultCodeAdd(API_CODE.API_CODE_FRIEND_REQUEST_ALREADY_STATE, API_CODETYPE.API_CODETYPE_USUALLY);   // フレンド申請：申請済で申請不要
            ResultCodeAdd(API_CODE.API_CODE_FRIEND_REQUEST_ALREADY_STATE2, API_CODETYPE.API_CODETYPE_USUALLY);  // フレンド申請：申請済で申請不要
            ResultCodeAdd(API_CODE.API_CODE_FRIEND_REQUEST_ALREADY_FRIEND, API_CODETYPE.API_CODETYPE_USUALLY);  // フレンド申請：成立済で申請不要
            ResultCodeAdd(API_CODE.API_CODE_FRIEND_REQUEST_FULL_ME, API_CODETYPE.API_CODETYPE_USUALLY); // フレンド申請：自分がフレンド枠上限
            ResultCodeAdd(API_CODE.API_CODE_FRIEND_REQUEST_FULL_HIM, API_CODETYPE.API_CODETYPE_USUALLY);    // フレンド申請：相手がフレンド枠上限
            ResultCodeAdd(API_CODE.API_CODE_FRIEND_REQUEST_RECEIVED, API_CODETYPE.API_CODETYPE_USUALLY);    // フレンド申請：既に相手から申請されていた	→ 成功でいいんじゃね？
            ResultCodeAdd(API_CODE.API_CODE_FRIEND_REQUEST_CANCELED, API_CODETYPE.API_CODETYPE_USUALLY);    // フレンド関連：既に相手からの申請が取り下げられている

            ResultCodeAdd(API_CODE.API_CODE_FRIEND_SELECT_NOT, API_CODETYPE.API_CODETYPE_USUALLY);  // フレンド関連：相手ユーザーが選択されていない
            ResultCodeAdd(API_CODE.API_CODE_FRIEND_SELECT_NOT2, API_CODETYPE.API_CODETYPE_USUALLY); // フレンド関連：相手ユーザーが選択されていない(破棄用？)
            ResultCodeAdd(API_CODE.API_CODE_FRIEND_SELECT_USER_NONE, API_CODETYPE.API_CODETYPE_USUALLY);    // フレンド関連：相手ユーザーが存在しない
            ResultCodeAdd(API_CODE.API_CODE_FRIEND_SELECT_ME, API_CODETYPE.API_CODETYPE_USUALLY);   // フレンド関連：フレンドに自分を指定

            ResultCodeAdd(API_CODE.API_CODE_QUEST_START_HELPER_ME, API_CODETYPE.API_CODETYPE_USUALLY);  // クエスト開始：自身を助っ人にはできない
            ResultCodeAdd(API_CODE.API_CODE_QUEST_START_OVER_UNIT, API_CODETYPE.API_CODETYPE_USUALLY);  // クエスト開始：所持ユニット数オーバー
            ResultCodeAdd(API_CODE.API_CODE_QUEST_START_OVER_UNIT2, API_CODETYPE.API_CODETYPE_USUALLY); // クエスト開始：所持ユニット数オーバー
            ResultCodeAdd(API_CODE.API_CODE_QUEST_START_OVER_PARTY_COST, API_CODETYPE.API_CODETYPE_USUALLY);    // クエスト開始：パーティコストオーバー
            ResultCodeAdd(API_CODE.API_CODE_QUEST_START_MASTER_NONE, API_CODETYPE.API_CODETYPE_USUALLY);    // クエスト開始：クエストマスターが存在しない
            ResultCodeAdd(API_CODE.API_CODE_QUEST_START_BAD_CONDITION, API_CODETYPE.API_CODETYPE_USUALLY);  // クエスト開始：クエストが受託できない			→ ナニコレ
            ResultCodeAdd(API_CODE.API_CODE_QUEST_START_INVALID, API_CODETYPE.API_CODETYPE_USUALLY);    // クエスト開始：無効状態のクエスト

            ResultCodeAdd(API_CODE.API_CODE_QUEST_RESULT_INJUSTICE, API_CODETYPE.API_CODETYPE_USUALLY); // クエストリザルト：クリアパラメータ上限超え	※チート
            ResultCodeAdd(API_CODE.API_CODE_QUEST_RESULT_ORDER_NONE, API_CODETYPE.API_CODETYPE_USUALLY);    // クエストリザルト：クエスト受諾情報無し		※チート
            ResultCodeAdd(API_CODE.API_CODE_QUEST_RESULT_ORDER_ERR, API_CODETYPE.API_CODETYPE_USUALLY); // クエストリザルト：クエスト受諾情報エラー	※チート？

            ResultCodeAdd(API_CODE.API_CODE_QUEST_INGAME_RESET_CT_ZERO, API_CODETYPE.API_CODETYPE_USUALLY); // クエスト中：リセット回数指定が不正（０な場合）
            ResultCodeAdd(API_CODE.API_CODE_QUEST_INGAME_RESET_CT_ERR, API_CODETYPE.API_CODETYPE_USUALLY);  // クエスト中：リセット回数比較エラー
            ResultCodeAdd(API_CODE.API_CODE_QUEST_INGAME_CONTINUE_CT_ZERO, API_CODETYPE.API_CODETYPE_USUALLY);  // クエスト中：コンティニュー回数指定が不正（０な場合）
            ResultCodeAdd(API_CODE.API_CODE_QUEST_INGAME_CONTINUE_CT_ERR, API_CODETYPE.API_CODETYPE_USUALLY);   // クエスト中：コンティニュー回数比較エラー

            ResultCodeAdd(API_CODE.API_CODE_EVOL_UNIT_LEVEL_SHORT, API_CODETYPE.API_CODETYPE_USUALLY);  // 進化クエスト開始：ベースユニットレベル不足
            ResultCodeAdd(API_CODE.API_CODE_EVOL_UNIT_LEVEL_SHORT2, API_CODETYPE.API_CODETYPE_USUALLY); // 進化クエスト開始：ベースユニットレベル不足
            ResultCodeAdd(API_CODE.API_CODE_EVOL_UNIT_AFTER_NONE, API_CODETYPE.API_CODETYPE_USUALLY);   // 進化クエスト開始：ベースユニット進化先が無い

            ResultCodeAdd(API_CODE.API_CODE_PAY_IOS_RECEIPT_ERR, API_CODETYPE.API_CODETYPE_USUALLY);    // 課金(iOS)：レシート情報不正
            ResultCodeAdd(API_CODE.API_CODE_PAY_IOS_RECEIPT_PURCHASE, API_CODETYPE.API_CODETYPE_USUALLY);   // 課金(iOS)：レシート使用済
            ResultCodeAdd(API_CODE.API_CODE_PAY_IOS_PRODUCT_ERR, API_CODETYPE.API_CODETYPE_USUALLY);    // 課金(iOS)：商品ID情報不正

            ResultCodeAdd(API_CODE.API_CODE_PAY_ANDROID_RECEIPT_ERR, API_CODETYPE.API_CODETYPE_USUALLY);    // 課金(Android)：レシート情報不正
            ResultCodeAdd(API_CODE.API_CODE_PAY_ANDROID_SIGNED_ERR, API_CODETYPE.API_CODETYPE_USUALLY); // 課金(Android)：サイン情報不正
                                                                                                        //			ResultCodeAdd( API_CODE.API_CODE_PAY_ANDROID_PRODUCT_ERR		, API_CODETYPE.API_CODETYPE_USUALLY			);	// 課金(Android)：商品ID情報不正					→ないっぽい

            ResultCodeAdd(API_CODE.API_CODE_PAY_WIDE_TIP_MAX, API_CODETYPE.API_CODETYPE_USUALLY);   // 課金共通：有料分の最大数超過
            ResultCodeAdd(API_CODE.API_CODE_PAY_WIDE_ERROR, API_CODETYPE.API_CODETYPE_USUALLY); // 課金共通：購入失敗
            ResultCodeAdd(API_CODE.API_CODE_PAY_WIDE_USER_NOT_MATCH, API_CODETYPE.API_CODETYPE_USUALLY);    // 課金共通：ユーザーIDエラー
            ResultCodeAdd(API_CODE.API_CODE_PAY_WIDE_APP_STORE_503, API_CODETYPE.API_CODETYPE_RETRY);   // 課金共通：AppleStoreへの問い合わせが繋がらなかった
            ResultCodeAdd(API_CODE.API_CODE_PAY_WIDE_AGE_BUDGET_OVER, API_CODETYPE.API_CODETYPE_USUALLY);   // 課金共通：年齢区分別の課金額上限突破


            ResultCodeAdd(API_CODE.API_CODE_STORE_MAX_UNIT, API_CODETYPE.API_CODETYPE_USUALLY); // ストア関連：拡張不可能（ユニット枠）
            ResultCodeAdd(API_CODE.API_CODE_STORE_MAX_FRIEND, API_CODETYPE.API_CODETYPE_USUALLY);   // ストア関連：拡張不可能（フレンド枠）
            ResultCodeAdd(API_CODE.API_CODE_STORE_MAX_STAMINA, API_CODETYPE.API_CODETYPE_USUALLY);  // ストア関連：スタミナ全快


            ResultCodeAdd(API_CODE.API_CODE_PAY_WIDE_PRODUCT_ERR, API_CODETYPE.API_CODETYPE_USUALLY);   // ストア関連：プロダクトIDが定義されていない
            ResultCodeAdd(API_CODE.API_CODE_PAY_WIDE_PRODUCT_TIMEOUT, API_CODETYPE.API_CODETYPE_USUALLY);   // ストア関連：プロダクトIDが期間外
            ResultCodeAdd(API_CODE.API_CODE_PAY_WIDE_PRODUCT_EVENT_NONE, API_CODETYPE.API_CODETYPE_USUALLY);    // ストア関連：イベントIDに紐付いた商品IDがない
            ResultCodeAdd(API_CODE.API_CODE_PAY_WIDE_PRODUCT_EVENT_ALREADY, API_CODETYPE.API_CODETYPE_USUALLY); // ストア関連：イベントIDに紐付いた商品を既に購入している


            ResultCodeAdd(API_CODE.API_CODE_PRESENT_NONE, API_CODETYPE.API_CODETYPE_USUALLY);   // プレゼント関連：プレゼントが存在しない
            ResultCodeAdd(API_CODE.API_CODE_PRESENT_SERIAL_ERR, API_CODETYPE.API_CODETYPE_USUALLY); // プレゼント関連：プレゼントシリアル番号が間違っている
            ResultCodeAdd(API_CODE.API_CODE_PRESENT_OPEN_NG, API_CODETYPE.API_CODETYPE_USUALLY);    // プレゼント関連：プレゼントが開けない

            ResultCodeAdd(API_CODE.API_CODE_SALE_COIN_MAX, API_CODETYPE.API_CODETYPE_USUALLY);  // ユニット売却：所持金最大

            ResultCodeAdd(API_CODE.API_CODE_REVIEW_ALREADY, API_CODETYPE.API_CODETYPE_USUALLY); // レビュー関連：既にレビューしている

            ResultCodeAdd(API_CODE.API_CODE_TRANSFER_USER_ID, API_CODETYPE.API_CODETYPE_USUALLY);   // セーブ移行関連：ユーザーID不一致
            ResultCodeAdd(API_CODE.API_CODE_TRANSFER_PASSWORD, API_CODETYPE.API_CODETYPE_USUALLY);  // セーブ移行関連：パスワード不一致
            ResultCodeAdd(API_CODE.API_CODE_TRANSFER_TIME_LIMIT, API_CODETYPE.API_CODETYPE_USUALLY);    // セーブ移行関連：パスワード発行後の移行期限切れ
            ResultCodeAdd(API_CODE.API_CODE_TRANSFER_MOVED, API_CODETYPE.API_CODETYPE_TRANSFER_MOVED);  // セーブ移行関連：セーブ移行発生

            ResultCodeAdd(API_CODE.API_CODE_RENEW_ERR_UUID_INVALID, API_CODETYPE.API_CODETYPE_USUALLY); // セーブ移行関連：セーブ移行発生
            ResultCodeAdd(API_CODE.API_CODE_RENEW_ERR_USER_ID_BEFORE, API_CODETYPE.API_CODETYPE_USUALLY);   // セーブ移行関連：セーブ移行発生

            ResultCodeAdd(API_CODE.API_CODE_BEGINNER_BOOST_FIXID_ERROR, API_CODETYPE.API_CODETYPE_USUALLY); // 初心者ブースト：ID不正エラー
            ResultCodeAdd(API_CODE.API_CODE_BEGINNER_BOOST_RANK_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);  // 初心者ブースト：指定IDのブーストはランク的に適用されない
            ResultCodeAdd(API_CODE.API_CODE_BEGINNER_BOOST_PERIOD_OUTSIDE, API_CODETYPE.API_CODETYPE_USUALLY);  // 初心者ブースト：イベントが終了している

            ResultCodeAdd(API_CODE.API_CODE_REQUIRE_QUEST_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);    // クエスト入場条件エラー

            ResultCodeAdd(API_CODE.API_CODE_MAKEPASSWORD_INVALID_RANK_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);    // セーブ移行関連：パスワード発行後の移行期限切れ

            ResultCodeAdd(API_CODE.API_CODE_AREA_AMEND_FIXID_ERROR, API_CODETYPE.API_CODETYPE_USUALLY); // エリア補正関連：エリア補正ID不正エラー
            ResultCodeAdd(API_CODE.API_CODE_AREA_AMEND_AREA_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);  // エリア補正関連：エリア補正IDと対象エリアが相違している
            ResultCodeAdd(API_CODE.API_CODE_AREA_AMEND_PERIOD_OUTSIDE, API_CODETYPE.API_CODETYPE_USUALLY);  // エリア補正関連：エリア補正IDイベントが終了している

            ResultCodeAdd(API_CODE.API_CODE_QUEST_CONTINUE_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);   // コンティニュー不可エラー
            ResultCodeAdd(API_CODE.API_CODE_QUEST_RETRY_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);  // リトライ不可エラー

            ResultCodeAdd(API_CODE.API_CODE_LINKSYSTEM_SALE_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);  // ユニット売却		：リンクありにより売却できない
            ResultCodeAdd(API_CODE.API_CODE_LINKSYSTEM_COMPOSE_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);   // ユニット進化合成	：リンクありにより強化できない
            ResultCodeAdd(API_CODE.API_CODE_LINKSYSTEM_FORMUNIT_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);  // ユニット編成		：リンクありにより編成できない
            ResultCodeAdd(API_CODE.API_CODE_LINKSYSTEM_UNIT_DISABLE_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);  // リンク作成画面	：リンク不可ユニット
            ResultCodeAdd(API_CODE.API_CODE_LINKSYSTEM_EVOL_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);  // ユニット進化合成	：リンクありにより進化できない

            ResultCodeAdd(API_CODE.API_CODE_ACHIEVEMENT_REWARD_MAX, API_CODETYPE.API_CODETYPE_USUALLY); // アチーブメント：受け取り上限エラー（一部）
            ResultCodeAdd(API_CODE.API_CODE_ACHIEVEMENT_REWARD_MAX_ALL, API_CODETYPE.API_CODETYPE_USUALLY); // アチーブメント：受け取り上限エラー

            ResultCodeAdd(API_CODE.API_CODE_ACHIEVEMENT_REWARD_ERR_1_KEY, API_CODETYPE.API_CODETYPE_USUALLY);   // アチーブメント：一部を受け取れなかった＋有効期限切れのキーが存在した
            ResultCodeAdd(API_CODE.API_CODE_ACHIEVEMENT_REWARD_ERR_2_KEY, API_CODETYPE.API_CODETYPE_USUALLY);   // アチーブメント：全部を受け取れなかった＋有効期限切れのキーが存在した
            ResultCodeAdd(API_CODE.API_CODE_ACHIEVEMENT_REWARD_ERR_INVALID_KEY, API_CODETYPE.API_CODETYPE_USUALLY); // アチーブメント：報酬受け取りに有効でないキーが存在した(その他は受け取れた)

            ResultCodeAdd(API_CODE.API_CODE_MASTERDATA_HASH_DIFFERENT, API_CODETYPE.API_CODETYPE_USUALLY);      //!< APIリザルトコード：ポイントショップ商品一覧：ハッシュコードが最新ではない

            ResultCodeAdd(API_CODE.API_CODE_POINT_SHOP_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);               //!< APIリザルトコード：ポイントショップ：購入失敗
            ResultCodeAdd(API_CODE.API_CODE_POINT_SHOP_TIMING_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);        //!< APIリザルトコード：ポイントショップ：購入期間外

            ResultCodeAdd(API_CODE.API_CODE_ITEM_NOTHING_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);     //!< APIリザルトコード：消費アイテム：アイテムが存在しない
            ResultCodeAdd(API_CODE.API_CODE_ITEM_OVERLAP_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);     //!< APIリザルトコード：消費アイテム：効果重複

            ResultCodeAdd(API_CODE.API_CODE_POINT_SHOP_LO_UNIT_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);       //!< APIリザルトコード：ポイントショップ：指定ユニットは限界突破出来ない
            ResultCodeAdd(API_CODE.API_CODE_POINT_SHOP_LO_COUNT_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);      //!< APIリザルトコード：ポイントショップ：限界突破指定回数オーバー
            ResultCodeAdd(API_CODE.API_CODE_POINT_SHOP_LO_POINT_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);      //!< APIリザルトコード：ポイントショップ：ユニットポイント不足

            ResultCodeAdd(API_CODE.API_CODE_QUEST_KEY_COUNT_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);      //!< APIリザルトコード：クエストキー：クエストキーが足りない
            ResultCodeAdd(API_CODE.API_CODE_QUEST_KEY_AREA_CLOSE, API_CODETYPE.API_CODETYPE_USUALLY);       //!< APIリザルトコード：クエストキー：キークエストのエリアが終了状態

            ResultCodeAdd(API_CODE.API_CODE_ACHIEVEMENT_GP_FIXID_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);     //!< APIリザルトコード：アチーブメントリスト：アチーブメントグループIDが存在しない

            ResultCodeAdd(API_CODE.API_CODE_SCORE_REWARD_INVALID_EVENT, API_CODETYPE.API_CODETYPE_USUALLY);     //!< APIリザルトコード：スコア報酬受け取り：無効なスコアイベント

            ResultCodeAdd(API_CODE.API_CODE_POST_LOG_UPLOAD_ERROR, API_CODETYPE.API_CODETYPE_USUALLY);     //!< APIリザルトコード：postlog禁止環境エラー

            ResultCodeAdd(API_CODE.API_CODE_GACHA_NOT_BOXGACHA, API_CODETYPE.API_CODETYPE_USUALLY);     //!< APIリザルトコード：スクラッチ：BOXスクラッチではない

            ResultCodeAdd(API_CODE.API_CODE_WIDE_ERROR_BAD_REQUEST, API_CODETYPE.API_CODETYPE_USUALLY);     //!< APIリザルトコード：スクラッチ：BOXスクラッチではない
            ResultCodeAdd(API_CODE.API_CODE_WIDE_ERROR_BAD_REQUEST2, API_CODETYPE.API_CODETYPE_USUALLY);     //!< APIリザルトコード：スクラッチ：BOXスクラッチではない

            //----------------------------------------------------------------------------
            // デバッグ用
            //----------------------------------------------------------------------------
            ResultCodeAdd(API_CODE.API_CODE_DEBUG_ERROR_PERMISSION, API_CODETYPE.API_CODETYPE_USUALLY);     //!< APIリザルトコード：デバッグAPI：リクエストしたユーザーが「開発ユーザー」ではない

            //----------------------------------------------------------------------------
            // API固有分岐系（一部API限定）
            //----------------------------------------------------------------------------
            ResultCodeAdd(API_CODE.API_CODE_CLIENT_RETRY, API_CODETYPE.API_CODETYPE_RETRY); // リトライ関連：リトライ

        }

        //----------------------------------------------------------------------------
        // APIタイプに依存したイレギュラーなリザルトコード区分表を生成
        //----------------------------------------------------------------------------
        {
            //--------------------------------
            // ユーザー認証で「ユーザー認証してね」が帰る場合は
            // ユーザー情報がサーバー上に存在しない
            // →破損ダイアログを出して生成からやり直す
            //--------------------------------
            ResultCodeAddIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED, API_CODETYPE.API_CODETYPE_USUALLY);
            ResultCodeAddIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED2, API_CODETYPE.API_CODETYPE_USUALLY);

            //--------------------------------
            // チュートリアルAPI
            // レスポンスを待って進める
            // エラーは無視する
            //--------------------------------
            ResultCodeAddIrregular(SERVER_API.SERVER_API_TUTORIAL, API_CODE.API_CODE_USER_AUTH_REQUIRED, API_CODETYPE.API_CODETYPE_USUALLY);
            ResultCodeAddIrregular(SERVER_API.SERVER_API_TUTORIAL, API_CODE.API_CODE_USER_AUTH_REQUIRED2, API_CODETYPE.API_CODETYPE_USUALLY);
            ResultCodeAddIrregular(SERVER_API.SERVER_API_TUTORIAL, API_CODE.API_CODE_CLIENT_RETRY, API_CODETYPE.API_CODETYPE_USUALLY);

            ResultCodeAddIrregular(SERVER_API.SERVER_API_RENEW_TUTORIAL, API_CODE.API_CODE_USER_AUTH_REQUIRED, API_CODETYPE.API_CODETYPE_USUALLY);
            ResultCodeAddIrregular(SERVER_API.SERVER_API_RENEW_TUTORIAL, API_CODE.API_CODE_USER_AUTH_REQUIRED2, API_CODETYPE.API_CODETYPE_USUALLY);
            ResultCodeAddIrregular(SERVER_API.SERVER_API_RENEW_TUTORIAL, API_CODE.API_CODE_CLIENT_RETRY, API_CODETYPE.API_CODETYPE_USUALLY);
        }

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リザルトコード一覧の生成
	*/
    //----------------------------------------------------------------------------
    public void ResultCodeAdd(API_CODE eCode, API_CODETYPE eCodeType)
    {
        if (m_ResultCodeType == null)
        {
            m_ResultCodeType = new TemplateList<ResultCodeType>();
        }
        m_ResultCodeType.Add(new ResultCodeType(eCode, eCodeType));
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	イレギュラーリザルトコード一覧の生成
	*/
    //----------------------------------------------------------------------------
    public void ResultCodeAddIrregular(SERVER_API eAPI, API_CODE eCode, API_CODETYPE eCodeType)
    {
        if (m_ResultCodeTypeIrregular == null)
        {
            m_ResultCodeTypeIrregular = new TemplateList<ResultCodeTypeIrregular>();
        }
        m_ResultCodeTypeIrregular.Add(new ResultCodeTypeIrregular(eAPI, eCode, eCodeType));
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	イレギュラーリザルトコード一覧の破棄
	*/
    //----------------------------------------------------------------------------
    public void ResultCodeDelIrregular(SERVER_API eAPI, API_CODE eCode)
    {
        for (int i = 0; i < m_ResultCodeTypeIrregular.m_BufferSize; i++)
        {
            if (m_ResultCodeTypeIrregular[i] == null
            || m_ResultCodeTypeIrregular[i].m_APIType != eAPI
            || m_ResultCodeTypeIrregular[i].m_ResultCodeType.m_Code != eCode
            ) continue;

            m_ResultCodeTypeIrregular[i] = null;
            return;
        }
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	リザルトコードからコードタイプを取得
	*/
    //----------------------------------------------------------------------------
    public ResultCodeType ResultCodeToCodeType(SERVER_API eAPI, API_CODE eCode)
    {
        //--------------------------------
        // APIに依存したイレギュラーなコードタイプを優先判定
        //--------------------------------
        if (m_ResultCodeTypeIrregular != null)
        {
            for (int i = 0; i < m_ResultCodeTypeIrregular.m_BufferSize; i++)
            {
                if (m_ResultCodeTypeIrregular[i] == null ||
                    m_ResultCodeTypeIrregular[i].m_APIType != eAPI ||
                    m_ResultCodeTypeIrregular[i].m_ResultCodeType.m_Code != eCode)
                {
                    continue;
                }

                return m_ResultCodeTypeIrregular[i].m_ResultCodeType;
            }
        }

        //--------------------------------
        // APIに依存しない汎用的なコードタイプを判定
        //--------------------------------
        if (m_ResultCodeType != null)
        {
            for (int i = 0; i < m_ResultCodeType.m_BufferSize; i++)
            {
                if (m_ResultCodeType[i].m_Code != eCode)
                {
                    continue;
                }

                return m_ResultCodeType[i];
            }
        }

#if DEBUG_LOG
		Debug.LogError( "Result Code To Code Type Error! - " + eAPI + " , " + eCode );
#endif
        return null;
    }



    //----------------------------------------------------------------------------
    /*!
		@brief	サーバー通信中ダイアログの表示切り替え
	*/
    //----------------------------------------------------------------------------
    public void ServerDialogUpdate(SERVER_API eAPI)
    {
        //--------------------------------
        // ダイアログ表示中などにローディング表示を強制的にオフる
        //--------------------------------
        if (m_LoadingClip == true)
        {
            LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.SERVER_API);
            return;
        }
        //--------------------------------
        // サーバー通信中はLoading表示
        //--------------------------------
        if (LoadingManager.Instance != null)
        {
            //--------------------------------
            // 特定のサーバーAPIはスルーを想定してダイアログを表示しない
            //--------------------------------
            if (eAPI == SERVER_API.SERVER_API_TUTORIAL || eAPI == SERVER_API.SERVER_API_RENEW_TUTORIAL)
            {
                return;
            }
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.SERVER_API);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	通信操作：通信リクエスト追加
	*/
    //----------------------------------------------------------------------------
    public uint AddCommunicateRequest(SERVER_API eAPIType, string strSendMessage, uint unUniqueID)
    {
        //--------------------------------
        // 送信情報を最低限チェック
        //--------------------------------
        if (strSendMessage.Length == 0)
        {
            Debug.LogError("strSendMessage NG!");
            return 0;
        }

        //--------------------------------
        // 通信リクエストのバッファ溢れチェック
        //--------------------------------
        uint nCommunicateTotal = m_CommunicateWorkInput - m_CommunicateWorkAccess;
        if (nCommunicateTotal + 1 >= COMMUNICATE_MAX)
        {
            Debug.LogError("Communicate Buffer Over!");
            return 0;
        }

        //--------------------------------
        // 通信リクエスト受理。
        // バッファに積んでおいて他の処理が終わり次第通信を行う。
        //--------------------------------
        m_CommunicateWorkInput++;

        uint nInputNum = m_CommunicateWorkInput % COMMUNICATE_MAX;
        m_PacketList[nInputNum].m_PacketAPI = eAPIType;
        m_PacketList[nInputNum].m_PacketSendMessage = strSendMessage;
        m_PacketList[nInputNum].m_PacketUniqueNum = unUniqueID;
        m_PacketList[nInputNum].m_CommunicateFinish = false;

#if BUILD_TYPE_DEBUG && DEBUG_LOG
		Debug.Log( "---------------------------\n" + eAPIType + "\n" + m_PacketList[ nInputNum ].m_PacketSendMessage + "\n---------------------------" );
#endif

        return m_PacketList[nInputNum].m_PacketUniqueNum;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	通信操作：完全完遂チェック
	*/
    //----------------------------------------------------------------------------
    public bool ChkCommunicateFinishAll()
    {
        return (m_CommunicateWorkInput == m_CommunicateWorkFinish);
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	通信操作：通信完遂チェック
		@note	AddCommunicateRequestの返り値を入れると、そのリクエストが完遂したか判断できる
	*/
    //----------------------------------------------------------------------------
    public bool ChkCommunicateFinish(uint unPacketUniqueID)
    {
        //--------------------------------
        // 
        //--------------------------------
        for (int i = 0; i < m_PacketList.Length; i++)
        {
            if (m_PacketList[i].m_PacketUniqueNum != unPacketUniqueID)
                continue;

            if (m_PacketList[i].m_CommunicateFinish == false)
            {
                return false;
            }
            else
            {
                if (m_PacketList[i] == m_CoroutinePacket)
                {
                    return false;
                }
                return true;
            }
        }
        return true;
    }


#if BUILD_TYPE_DEBUG
    //----------------------------------------------------------------------------
    /*!
		@brief	パケットの状況をログ出力
	*/
    //----------------------------------------------------------------------------
    public void DebugLogPacketList(uint unPacketUniqueID)
    {
        //--------------------------------
        // 
        //--------------------------------
        string strPacketLog = "";
        strPacketLog += "DebugPacketList Log -WorkNow------------\n";
        if (m_CoroutinePacket != null)
        {
            if (m_CoroutinePacket.m_PacketUniqueNum == unPacketUniqueID)
            {
                strPacketLog += m_CoroutinePacket.m_PacketUniqueNum + " , " + m_CoroutinePacket.m_PacketAPI + " , " + m_CoroutinePacket.m_CommunicateFinish + "!!!!!!!!!!!!!!!!!!!!!!!!!!!\n";
            }
            else
            {
                strPacketLog += m_CoroutinePacket.m_PacketUniqueNum + " , " + m_CoroutinePacket.m_PacketAPI + " , " + m_CoroutinePacket.m_CommunicateFinish + "\n";
            }
        }

        strPacketLog += "DebugPacketList Log ---------------\n";
        for (int i = 0; i < m_PacketList.Length; i++)
        {
            if (m_PacketList[i] == null)
                continue;

            if (m_PacketList[i].m_PacketUniqueNum == unPacketUniqueID)
            {
                strPacketLog += i + " , " + m_PacketList[i].m_PacketUniqueNum + " , " + m_PacketList[i].m_PacketAPI + " , " + m_PacketList[i].m_CommunicateFinish + "!!!!!!!!!!!!!!!!!!!!!!!!!!!\n";
            }
            else
            {
                strPacketLog += i + " , " + m_PacketList[i].m_PacketUniqueNum + " , " + m_PacketList[i].m_PacketAPI + " , " + m_PacketList[i].m_CommunicateFinish + "\n";
            }
        }

        Debug.Log(strPacketLog);
    }
#endif

    //----------------------------------------------------------------------------
    /*!
		@brief	リザルト操作：リザルト追加
		@note	リングバッファでリザルトデータを保持。古いデータは勝手に消えるので注意
	*/
    //----------------------------------------------------------------------------
    private bool AddPacketResult(uint unPacketUniqueID, SERVER_API eAPI, API_CODE eAPICode)
    {
        //--------------------------------
        // 
        //--------------------------------
        uint unAccess = m_PacketResultInput % (uint)m_PacketResult.Length;
        m_PacketResult[unAccess].m_PacketAPI = eAPI;
        m_PacketResult[unAccess].m_PacketCode = eAPICode;
        //		m_PacketResult[ unAccess ].m_PacketResult	= eAPIResult;
        m_PacketResult[unAccess].m_PacketUniqueNum = unPacketUniqueID;
        m_PacketResultInput++;

        //		Debug.LogError( "" + m_PacketResult[ unAccess ].m_PacketAPI + " , " + m_PacketResult[ unAccess ].m_PacketUniqueNum + " , " + unAccess );

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リザルト操作：リザルト取得
	*/
    //----------------------------------------------------------------------------
    public PacketDataResult GetPacketResult(uint unPacketUniqueID)
    {
        //--------------------------------
        // 
        //--------------------------------
        //		string strPacketResultList = "";
        for (int i = 0; i < m_PacketResult.Length; i++)
        {
            //			strPacketResultList += "" + i + " , " + m_PacketResult[i].m_PacketUniqueNum + "\n";

            if (m_PacketResult[i].m_PacketUniqueNum != unPacketUniqueID)
                continue;

            return m_PacketResult[i];
        }
        //		Debug.LogError( strPacketResultList );
        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リザルト操作：リザルト削除
		@note	一部のAPIで固定のユニーク番号を使用するものがあり、それの再送時に不具合が発生するため、ユニークIDを潰して情報が見つからないようにする
	*/
    //----------------------------------------------------------------------------
    public bool DelPacketResult(uint unPacketUniqueID)
    {
        //--------------------------------
        // 
        //--------------------------------
        for (int i = 0; i < m_PacketResult.Length; i++)
        {
            if (m_PacketResult[i].m_PacketUniqueNum != unPacketUniqueID)
                continue;

            m_PacketResult[i].m_PacketUniqueNum = 0xffffff;
            return true;
        }
        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	通信実行処理：サーバーにAPIを送信
	*/
    //----------------------------------------------------------------------------
    private void SendPacketAPI()
    {
        Debug.Assert(false, "ServerDataManager.SendPacketAPI() is deprecated");
    }

#if DEF_API_CRASHPOINT
	//----------------------------------------------------------------------------
	/*!
		@brief	強制アプリクラッシュダイアログ：
		@note	「ここでアプリをクラッシュさせますか？」のダイアログを出してYESならクラッシュ操作。
				・サーバーに送信したものが届いてない場合
				・サーバーに送信したものが届いてクライアントが返信受け取る前
				でのクラッシュを想定したポイントでダイアログを出し、ダイアログの選択次第でアプリを強制終了させる補助機能として用意
	*/
	//----------------------------------------------------------------------------
	IEnumerator DebugServerAPIClashDialog( string strDialogTitle , string strDialogMsg )
	{
		Dialog cCrashDialog = DialogManager.Open2B2_Direct( strDialogTitle , strDialogMsg , "BTN_YES" , "BTN_NO" , true, true );
		if( cCrashDialog != null )
		{
			while( true )
			{
				if( cCrashDialog.m_DialogPushButton != EDIALOG_BTN.eDIALOG_BTN_NONE )
				{
					m_ServerAPICrashPointDialogBtn = cCrashDialog.m_DialogPushButton;
					break;
				}
				yield return null;
			}
		}
	}
	
	//----------------------------------------------------------------------------
	/*!
		@brief	アプリ強制クラッシュ
	*/
	//----------------------------------------------------------------------------
	IEnumerator DebugServerAPIClash()
	{
		Application.Quit();
		while( true )
		{
			yield return null;
		}
	}
#endif
}

