/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	LocalSaveManager.cs
	@brief	ローカルセーブ管理
	@author Developer
	@date 	2012/11/28

	UserDataAdminクラスを経由してセーブロードを行う。
	各オブジェクトを参照してセーブを実行すると、破棄順によって満足にセーブが達成できないことがある。
	UserDataAdminを経由することで破棄順の制御の対応が容易になる。
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/

using UnityEngine;
using System;
using System.Text;
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
	@brief	ローカルセーブ管理クラス
*/
//----------------------------------------------------------------------------
public class LocalSaveManager : SingletonComponent<LocalSaveManager>
{
    public const string SAVE_SCHEME = "jp.example.divinegate";

    //ファイルセーブ化
    public const string LOCALSAVE_GOES_TO_QUEST2_RESTORE = "0001";  //!< ローカルセーブ識別名：インゲーム復帰関連：中断復帰パラメータ(新探索)
    public const string LOCALSAVE_GOES_TO_MENU_RESULT = "0002"; //!< ローカルセーブ識別名：メインメニュー復帰関連：クエストリザルト
    public const string LOCALSAVE_GOES_TO_MENU_RETIRE = "0003"; //!< ローカルセーブ識別名：メインメニュー復帰関連：クエストリタイア
    public const string LOCALSAVE_GOES_TO_QUEST2_START = "0004";    //!< ローカルセーブ識別名：インゲーム復帰関連：クエスト開始パラメータ(新探索)
    public const string LOCALSAVE_INGAME_CONTINUE = "0005"; //!< ローカルセーブ識別名：インゲーム課金情報：コンティニュー回数
    public const string LOCALSAVE_INGAME_RESET = "0006";    //!< ローカルセーブ識別名：インゲーム課金情報：リセット回数

    //OSの機能でセーブ

    public const string LOCALSAVE_UPDATE_COUNTER = "divinegate_UPDATE_COUNTER";         //!< ローカルセーブ識別名：マスターデータ更新カウンタ
    public const string LOCALSAVE_UPDATE_COUNTER_MISSION = "divinegate_UPDATE_COUNTER_MISSION"; //!< ローカルセーブ識別名：マスターデータ更新カウンタ(アチーブメント専用)
    public const string LOCALSAVE_UPDATE_CHANGE_DATE_TIME = "divinegate_UPDATE_CHANGE_DATE_TIME";   //!< ローカルセーブ識別名：日付変更のタイトル戻しの時間変更
    public const string LOCALSAVE_UUID = "divinegate_USER_UUID";                //!< ローカルセーブ識別名：UUID保存
    public const string LOCALSAVE_TITLE_UUID = "divinegate_TITLE_UUID";                //!< ローカルセーブ識別名：UUIDタイトルログイン
    public const string LOCALSAVE_USER_ID = "divinegate_USER_ID";					//!< ローカルセーブ識別名：ユーザーID仮置き
    public const string LOCALSAVE_USER_ID_TRANSFER = "divinegate_USER_ID_TRANSFER";		//!< ローカルセーブ識別名：データ移行時ユーザが入力した移行先のユーザIDを保持（移行の通信が失敗してもユーザ認証で取得したユーザIDと比較できる）
    public const string LOCALSAVE_SERVER_IP = "divinegate_API_TEST_IP";             //!< ローカルセーブ識別名：APIテストのIPアドレス
    public const string LOCALSAVE_SERVER_AWS_TYPE = "divinegate_AWS_TYPE";              //!< ローカルセーブ識別名：AWSでのAssetBundle置き場所タイプ
    public const string LOCALSAVE_QUALITY = "divinegate_QUALITY";                   //!< ローカルセーブ識別名：クオリティ
    public const string LOCALSAVE_LOGIN = "divinegate_LOGINPARAM";              //!< ローカルセーブ識別名：ログインボーナス情報
    public const string LOCALSAVE_TUTORIAL_GROUP = "divinegate_TUTORIAL_GROUP";         //!< ローカルセーブ識別名：チュートリアル進行フラグ
    public const string LOCALSAVE_TUTORIAL_SKIP = "divinegate_TUTORIAL_SKIP";           //!< ローカルセーブ識別名：チュートリアルスキップフラグ
    public const string LOCALSAVE_FAVORITE_FRIEND = "divinegate_FAVOTIRE_FRIEND";           //!< ローカルセーブ識別名：お気に入りフレンド
    public const string LOCALSAVE_FAVORITE_UNIT = "divinegate_FAVOTIRE_UNIT";           //!< ローカルセーブ識別名：お気に入りユニット
    public const string LOCALSAVE_OPTION = "divinegate_OPTION";                 //!< ローカルセーブ識別名：オプション設定
    public const string LOCALSAVE_MASTER_HASH = "divinegate_MASTER_HASH_LIST";      //!< ローカルセーブ識別名：マスターハッシュ ※ver2000で機能復活する際に構造変えたのでキーをちょっと改変。
    public const string LOCALSAVE_MASTER = "divinegate_MASTER_";                    //!< ローカルセーブ識別名：マスター実体接頭子
    public const string LOCALSAVE_FRIEND_USE = "divinegate_FRIEND_USE";             //!< ローカルセーブ識別名：フレンド使用情報
    public const string LOCALSAVE_NOTIFICATION_REQUEST = "divinegate_NOTIFICATION_REQUEST"; //!< ローカルセーブ識別名：ローカル通知リクエストイベントリスト

    //----------------------------------------------------------------------------
    // タイトルでの利用規約同意強制関連
    //----------------------------------------------------------------------------
    //	public	const 	string	LOCALSAVE_INFORMATION				= "divinegate_INFORMATION";				//!< ローカルセーブ識別名：利用規約同意済
    public const string LOCALSAVE_INFORMATION = "divinegate_INFORMATION_2nd";           //!< ローカルセーブ識別名：利用規約同意済（1.0.4.1の際に規約漏れの不具合が生じたのの対応としてキーを変更して更新時に再度規約ページを開く）
    public const string LOCALSAVE_INFORMATION_VER = "divinegate_INFORMATION_2nd_VER";       //!< ローカルセーブ識別名：利用規約同意済バージョン
    public const string LOCALSAVE_INFORMATION_POLICY = "divinegate_INFORMATION_POLICY";     //!< ローカルセーブ識別名：ポリシー同意済
    public const string LOCALSAVE_INFORMATION_POLICY_VER = "divinegate_INFORMATION_POLICY_VER";  //!< ローカルセーブ識別名：ポリシー同意済バージョン

    //----------------------------------------------------------------------------
    // 通知をその日一度だけ表示するための保存情報
    //----------------------------------------------------------------------------
    public const string LOCALSAVE_INFO_DATE = "divinegate_INFO_DATE";				//!< ローカルセーブ識別名：通知表示日
    public const string LOCALSAVE_INFO_NORMAL_LIST = "divinegate_INFO_NORMAL_LIST";		//!< ローカルセーブ識別名：通知表示IDリスト（通常）
    public const string LOCALSAVE_INFO_EMERGENCY_LIST = "divinegate_INFO_EMERGENCY_LIST";		//!< ローカルセーブ識別名：通知表示IDリスト（緊急）
    //----------------------------------------------------------------------------
    //
    //----------------------------------------------------------------------------
    public const string LOCALSAVE_FRIEND_POINT_ACTIVE = "divinegate_QUEST_FP_ACTIVE";           //!< ローカルセーブ識別名：クエストでのフレンドポイントアクティブフラグ

    public const string LOCALSAVE_SCRATCH = "divinegate_SCRATCH";                 //!< ローカルセーブ識別名：ガチャ引き情報
    public const string LOCALSAVE_SCRATCH_DETAIL_SKIP = "divinegate_SCRATCH_DETAIL_SKIP";     //!< ローカルセーブ識別名：ガチャ引き情報：演出

    public const string LOCALSAVE_SORT_FILTER_PARTY_FORM = "divinegate_SORT_FILTER_PARTY_FORM";      //!< ローカルセーブ識別名：ソート情報：パーティ編成
    public const string LOCALSAVE_SORT_FILTER_BUILD_UNIT = "divinegate_SORT_FILTER_BUILD_UNIT";      //!< ローカルセーブ識別名：ソート情報：強化合成ユニット
    public const string LOCALSAVE_SORT_FILTER_BUILD_FRIEND = "divinegate_SORT_FILTER_BUILD_FRIEND";    //!< ローカルセーブ識別名：ソート情報：強化合成フレンド
    public const string LOCALSAVE_SORT_FILTER_EVOLVE_UNIT = "divinegate_SORT_FILTER_EVOLVE_UNIT";     //!< ローカルセーブ識別名：ソート情報：進化合成ユニット
    public const string LOCALSAVE_SORT_FILTER_EVOLVE_FRIEND = "divinegate_SORT_FILTER_EVOLVE_FRIEND";   //!< ローカルセーブ識別名：ソート情報：進化合成フレンド
    public const string LOCALSAVE_SORT_FILTER_LINK_UNIT = "divinegate_SORT_FILTER_LINK_UNIT";       //!< ローカルセーブ識別名：ソート情報：リンクユニット
    public const string LOCALSAVE_SORT_FILTER_UNIT_SALE = "divinegate_SORT_FILTER_UNIT_SALE";       //!< ローカルセーブ識別名：ソート情報：ユニット売却
    public const string LOCALSAVE_SORT_FILTER_UNIT_LIST = "divinegate_SORT_FILTER_UNIT_LIST";       //!< ローカルセーブ識別名：ソート情報：ユニット一覧
    public const string LOCALSAVE_SORT_FILTER_FRIEND_LIST = "divinegate_SORT_FILTER_FRIEND_LIST";     //!< ローカルセーブ識別名：ソート情報：フレンド一覧
    public const string LOCALSAVE_SORT_FILTER_FRIEND_WAIT_ME = "divinegate_SORT_FILTER_FRIEND_WAIT_ME";  //!< ローカルセーブ識別名：ソート情報：フレンド申請受け中
    public const string LOCALSAVE_SORT_FILTER_FRIEND_WAIT_HIM = "divinegate_SORT_FILTER_FRIEND_WAIT_HIM"; //!< ローカルセーブ識別名：ソート情報：フレンド申請出し中
    public const string LOCALSAVE_SORT_FILTER_QUEST_FRIEND = "divinegate_SORT_FILTER_QUEST_FRIEND";    //!< ローカルセーブ識別名：ソート情報：クエスト前フレンド
    public const string LOCALSAVE_SORT_FILTER_ACHIEVEMENT = "divinegate_SORT_FILTER_ACHIEVEMENT";     //!< ローカルセーブ識別名：ソート情報：ミッション
    public const string LOCALSAVE_SORT_FILTER_UNIT_POINT_LO = "divinegate_SORT_FILTER_UNIT_POINT_LO";        //!< ローカルセーブ識別名：ソート情報：ポイントショップ限界突破
    public const string LOCALSAVE_SORT_FILTER_UNIT_POINT_EVOLVE = "divinegate_SORT_FILTER_UNIT_POINT_EVOLVE";     //!< ローカルセーブ識別名：ソート情報：ポイント
    public const string LOCALSAVE_SORT_FILTER_ACHIEVEMENT_GP = "divinegate_SORT_FILTER_ACHIEVEMENT_GP";  //!< ローカルセーブ識別名：ソート情報：ミッショングループ

    public const string LOCALSAVE_SAVE_RENEW_UUID = "divinegate_RENEW_UUID";                //!< ローカルセーブ識別名：データ再構築での遷移後期待UUID
    public const string LOCALSAVE_CALENDAR = "divinegate_LOGIN_CALENDAR";           //!< ローカルセーブ識別名：月間ログイン：最終ログイン日
    public const string LOCALSAVE_ACHIEVEMENT_ENSYUTSU = "divinegate_ACHIEVEMENT_ENSYUTSU"; //!< ローカルセーブ識別名：アチーブメント演出済み一覧
    public const string LOCALSAVE_PACKET_UNIQUE_ID = "divinegate_PACKET_UNIQUE_ID";        //!< ローカルセーブ識別名：パケットユニークID

    public const string LOCALSAVE_TUTORIAL_DIALOG = "divinegate_TUTORIAL_DIALOG";           //!< ローカルセーブ識別名：チュートリアルダイアログ表示フラグ

    public const string LOCALSAVE_TRANSFER_PASSWORD = "devinegate_TRANSFER_PASSWORD";       //!< ローカルセーブ識別名：機種移行パスワード
    public const string LOCALSAVE_MOVIE_FIRST = "devinegate_MOVIE_FIRST";               //!< ローカルセーブ識別名：動画初回再生

    public const string LOCALSAVE_GROSSING_TYPE = "divinegate_GROSSING_TYPE";           //!< ローカルセーブ識別名：総付スクラッチ：タイプ
    public const string LOCALSAVE_GROSSING_ID = "divinegate_GROSSING_ID";               //!< ローカルセーブ識別名：総付スクラッチ：ID
    public const string LOCALSAVE_GROSSING_NUM = "divinegate_GROSSING_NUM";         //!< ローカルセーブ識別名：総付スクラッチ：個数

#if SAVE_VERSIONUP_TEST
	//----------------------------------------
	//		※この定義を有効化してセーブを作成し、無効化した正規版で読み込む。
	//		※バージョンアップフラグの制御が正常動作してるなら構造体化の前にバージョン判定によるデータ破棄が走るので問題ないはず。
	//		※逆にこの定義による解析エラーが発生するのであれば、構造体の内容変化によって解析エラーが発生するフローが残っていると判断できる。
	//----------------------------------------
	public	const 	string	LOCALSAVE_RESTORE_VERSION			= "divinegate_RESTORE_VER";			//!< ローカルセーブ識別名：中断復帰情報のバージョン
	public	const	string	LOCALSAVE_RESTORE_VERSION_ID		= "107after";						//!< ローカルセーブ識別名：中断復帰情報のバージョン（これを変えるとローカルの中断復帰情報が破棄される）
	public	const	string	LOCALSAVE_RESULT_VERSION			= "divinegate_RESULT_VER_DMY";		//!< ローカルセーブ識別名：クエストリザルト情報のバージョン	※もともと107時代には無いパラメータなのでキーを適当に変えている
	public	const	string	LOCALSAVE_RESULT_VERSION_ID			= "107after";						//!< ローカルセーブ識別名：クエストリザルト情報のバージョン（これを変えるとローカルのクエストクリア情報が破棄される）
#else
    public const string LOCALSAVE_RESTORE_VERSION = "divinegate_RESTORE_VER";           //!< ローカルセーブ識別名：中断復帰情報のバージョン
    public const string LOCALSAVE_RESTORE_VERSION_ID = "500after";                      //!< ローカルセーブ識別名：中断復帰情報のバージョン（これを変えるとローカルの中断復帰情報が破棄される）
    public const string LOCALSAVE_RESULT_VERSION = "divinegate_RESULT_VER";         //!< ローカルセーブ識別名：クエストリザルト情報のバージョン
    public const string LOCALSAVE_RESULT_VERSION_ID = "500after";                       //!< ローカルセーブ識別名：クエストリザルト情報のバージョン（これを変えるとローカルのクエストクリア情報が破棄される）
#endif

    private LocalSaveMasterHash m_CacheMasterHash = null;                                       //!< ハッシュ値のjsonデコードが負荷になっているためキャッシュ化



    //----------------------------------------------------------------------------
    /*!
		@brief	AssetBundle初期化タイプ
	*/
    //----------------------------------------------------------------------------
    public enum ASSETBUNDLE_INIT
    {
        ASSETBUNDLE_INIT_FIRST = 0      //!< AssetBundle初期化タイプ：インストール直後
    , ASSETBUNDLE_INIT_RESET = 1        //!< AssetBundle初期化タイプ：データ更新有りでリセット発生
    , ASSETBUNDLE_INIT_CACHE = 2        //!< AssetBundle初期化タイプ：データ更新無しでキャッシュ比較のみ
    };

    //----------------------------------------------------------------------------
    /*!
		@brief	FoxSDK関連
	*/
    //----------------------------------------------------------------------------
    public enum AGREEMENT
    {
        NONE = 0        //!< FoxSDK関連：何もしてない状態
    , FOX_CALLED = 1        //!< FoxSDK関連：FoxSDKによる集計ブラウザ開き済
    , AGREE_OK = 2      //!< FoxSDK関連：利用規約への同意済
    };

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public SceneGoesParamToQuest2Restore m_SaveGoesDataToQuest2Restore = null;      //!< 中断復帰データ：インゲーム復帰関連：中断復帰パラメータ
    public SceneGoesParamToQuest2 m_SaveGoesDataToQuest2 = null;        //!< 中断復帰データ：インゲーム復帰関連：クエスト開始パラメータ
    private SceneGoesParamToMainMenu m_SaveGoesDataToMainMenuResult = null;     //!< 中断復帰データ：メインメニュー復帰関連：クエストリザルト
    private SceneGoesParamToMainMenuRetire m_SaveGoesDataToMainMenuRetire = null;       //!< 中断復帰データ：メインメニュー復帰関連：クエストリタイア


    private TemplateList<long> m_LocalSaveFavoriteUnit = null;      //!< お気に入りユニット情報キャッシュ
    private TemplateList<uint> m_LocalSaveFavoriteFriend = null;        //!< お気に入りフレンド情報キャッシュ
    private LocalSaveOption m_LocalSaveOption = null;       //!< オプション設定キャッシュ
                                                            //private			int											m_LocalSaveTutorialFlag			= -1;		//!< チュートリアル進行フラグ
    private TemplateList<LocalSaveFriendUse> m_LocalSaveFriendUse = null;       //!< フレンド使用
    private TemplateList<LocalSaveEventNotification> m_LocalSaveNotificationRequest = null;     //!< ローカル通知リクエストリスト
                                                                                                //private			byte[]										m_LocalSaveTutorialDialofFlags	= null;		//!< チュートリアルダイアログ表示フラグ


    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	ローカルセーブ再構築処理：セーブ移行時
	*/
    //----------------------------------------------------------------------------
    static public void RefreshTransfer()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log(" SaveData Transfer Refresh.");
#endif

        //--------------------------------
        // セーブデータの部分破棄
        //--------------------------------

        //--------------------------------
        // 中断復帰情報破棄
        //--------------------------------
        LocalSaveUtilToInstallFolder.RemoveBattleSetting(LOCALSAVE_GOES_TO_MENU_RESULT);

        LocalSaveUtilToInstallFolder.RemoveBattleSetting(LOCALSAVE_GOES_TO_MENU_RETIRE);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_FRIEND_POINT_ACTIVE);

        LocalSaveUtilToInstallFolder.RemoveBattleSetting(LOCALSAVE_INGAME_CONTINUE);
        LocalSaveUtilToInstallFolder.RemoveBattleSetting(LOCALSAVE_INGAME_RESET);

        LocalSaveUtil.ExecDataRemove(LOCALSAVE_RESTORE_VERSION);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_RESULT_VERSION);

        //--------------------------------
        // ログイン情報破棄
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_LOGIN);

        //--------------------------------
        // 表示用ユーザーID情報破棄
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_USER_ID);

        //--------------------------------
        // ガチャ引き情報破棄
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SCRATCH);

        //--------------------------------
        // アチーブメント演出情報破棄
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_ACHIEVEMENT_ENSYUTSU);

        //--------------------------------
        // カレンダー演出情報破棄
        //
        // ※カレンダー演出をクリアすると、「受け取った後なのに発生したよ演出が表示される」危険性がある。
        // ※無駄に期待させないように、「受け取る前だけど演出が表示されない（プレゼントはもらえる）」
        //--------------------------------
        //		LocalSaveUtil.ExecDataRemove( LOCALSAVE_CALENDAR );

        //--------------------------------
        // チュートリアルダイアログ破棄
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_TUTORIAL_DIALOG);

        //--------------------------------
        // お気に入り情報破棄
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_FAVORITE_FRIEND);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_FAVORITE_UNIT);

        //--------------------------------
        // フレンド使用情報破棄
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_FRIEND_USE);

        //--------------------------------
        // データ移行用パスワード情報破棄
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_TRANSFER_PASSWORD);

        //--------------------------------
        // ローカル通知リクエストイベントリスト
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_NOTIFICATION_REQUEST);

        //--------------------------------
        // 利用規約・プライバシー・ポリシー同意情報
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_INFORMATION_VER);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_INFORMATION_POLICY_VER);

        //--------------------------------
        // 通知をその日一度だけ表示するための保存情報
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_INFO_DATE);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_INFO_NORMAL_LIST);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_INFO_EMERGENCY_LIST);

        //--------------------------------
        // ページ別ソート情報
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_PARTY_FORM);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_BUILD_UNIT);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_BUILD_FRIEND);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_EVOLVE_UNIT);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_EVOLVE_FRIEND);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_LINK_UNIT);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_UNIT_SALE);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_UNIT_LIST);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_FRIEND_LIST);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_FRIEND_WAIT_ME);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_FRIEND_WAIT_HIM);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_QUEST_FRIEND);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_ACHIEVEMENT);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_UNIT_POINT_LO);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_UNIT_POINT_EVOLVE);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_SORT_FILTER_ACHIEVEMENT_GP);

        //--------------------------------
        // Other設定情報
        //--------------------------------
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_OPTION);
        LocalSaveUtil.ExecDataRemove(LOCALSAVE_NOTIFICATION_REQUEST);

        //--------------------------------
        // 諸々キャッシュ情報を一旦クリア
        //--------------------------------
        LocalSaveManager.Instance.m_LocalSaveFavoriteUnit = null;       // お気に入りユニット情報キャッシュ
        LocalSaveManager.Instance.m_LocalSaveFavoriteFriend = null;     // お気に入りフレンド情報キャッシュ
        LocalSaveManager.Instance.m_LocalSaveOption = null;     // オプション設定キャッシュ
        LocalSaveManager.Instance.m_LocalSaveFriendUse = null;      // フレンド使用状況

        LocalSaveManager.Instance.m_SaveGoesDataToQuest2Restore = null; // 中断復帰データ：インゲーム復帰関連：中断復帰パラメータ
        LocalSaveManager.Instance.m_SaveGoesDataToQuest2 = null;    // 中断復帰データ：インゲーム復帰関連：クエスト開始パラメータ
        LocalSaveManager.Instance.m_SaveGoesDataToMainMenuResult = null;    // 中断復帰データ：メインメニュー復帰関連：クエストリザルト
        LocalSaveManager.Instance.m_SaveGoesDataToMainMenuRetire = null; // 中断復帰データ：メインメニュー復帰関連：クエストリタイア
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ローカルセーブ再構築処理
	*/
    //----------------------------------------------------------------------------
    static public void LocalSaveRenew(bool bUUIDRenew, bool bServerAddressRenew)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log(" SaveData Deleted.");
#endif
        //--------------------------------
        // 引き継ぎたい情報を取得しておく
        //--------------------------------
        string strSavedUUID = LocalSaveManager.Instance.LoadFuncUUID();
        string strSavedServerAddress = LocalSaveManager.Instance.LoadFuncServerAddressIP();

        QualitySetting eSavedAssetBundleQualityLevel = LocalSaveManagerRN.Instance.QualitySetting;
        uint unPacketUniqueID = LocalSaveManager.Instance.LoadFuncPacketUniqueID();

        //--------------------------------
        // セーブデータの全破棄
        //--------------------------------
        LocalSaveUtil.ExecDataDelete();

        //--------------------------------
        // 諸々キャッシュ情報を一旦クリア
        //--------------------------------
        LocalSaveManager.Instance.m_LocalSaveFavoriteUnit = null;   // お気に入りユニット情報キャッシュ
        LocalSaveManager.Instance.m_LocalSaveFavoriteFriend = null; // お気に入りフレンド情報キャッシュ
        LocalSaveManager.Instance.m_LocalSaveOption = null;         // オプション設定キャッシュ
        LocalSaveManager.Instance.m_LocalSaveFriendUse = null;      // フレンド使用状況

        LocalSaveManager.Instance.m_SaveGoesDataToQuest2Restore = null; // 中断復帰データ：インゲーム復帰関連：中断復帰パラメータ
        LocalSaveManager.Instance.m_SaveGoesDataToQuest2 = null;        // 中断復帰データ：インゲーム復帰関連：クエスト開始パラメータ
        LocalSaveManager.Instance.m_SaveGoesDataToMainMenuResult = null;// 中断復帰データ：メインメニュー復帰関連：クエストリザルト
        LocalSaveManager.Instance.m_SaveGoesDataToMainMenuRetire = null;// 中断復帰データ：メインメニュー復帰関連：クエストリタイア

        //--------------------------------
        // 引き継ぎたい情報の復元
        //--------------------------------
        if (bUUIDRenew == false)
        {
            LocalSaveManager.Instance.SaveFuncUUID(strSavedUUID);
        }

        if (bServerAddressRenew == false)
        {
            LocalSaveManager.Instance.SaveFuncServerAddressIP(strSavedServerAddress);
        }

        LocalSaveManagerRN.Instance.QualitySetting = eSavedAssetBundleQualityLevel;
        LocalSaveManager.Instance.SaveFuncPacketUniqueID(unPacketUniqueID);

        //--------------------------------
        // セーブ消した結果が残らないように諸々再読み込み
        //--------------------------------
        if (LocalSaveManager.Instance != null)
        {
            LocalSaveManager.Instance.LoadFuncLocalData();
            LocalSaveManager.Instance.LoadFuncRestore();
            LocalSaveManager.Instance.LoadFuncUUID();
        }
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	セーブデータバージョンセーフティ
	*/
    //----------------------------------------------------------------------------
    static public void SaveVersionSafety()
    {
        //bool bSaveVersionOK = true;// @Change Developer 2015/11/04 warning除去。
        /*
                //--------------------------------
                // 文字列を取得
                //--------------------------------
                string strLoadString = LocalSaveUtil.GetValueString( SAVE_VERSION_KEY , "" );
                if( strLoadString.Length <= 0 )
                {
                    Debug.LogError( "SaveData VersionKey None! ... " + strLoadString );
                    bSaveVersionOK = false;
                }

                //--------------------------------
                // 文字列をパラメータに変換してバージョンチェック
                //--------------------------------
                if( bSaveVersionOK == true )
                {
                    LocalSaveVersion sLocalSaveVersion = LitJson.JsonMapper.ToObject< LocalSaveVersion >( strLoadString );
                    if( sLocalSaveVersion == null
                    ||	sLocalSaveVersion.m_SaveVertsion != SAVE_VERSION
                    )
                    {
                        Debug.LogError( "SaveData VersionUp! All Clear!" );
                        bSaveVersionOK = false;
                    }
                }
        */
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：ログイン情報
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncLoginParam(RecvLoginParamValue cLoginParam)
    {
        string strSaveString = LitJson.JsonMapper.ToJson(cLoginParam);

        LocalSaveUtil.SetValueString(LOCALSAVE_LOGIN, strSaveString);
        LocalSaveUtil.ExecDataSave();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ログイン情報
	*/
    //----------------------------------------------------------------------------
    public ServerDataDefine.RecvLoginParamValue LoadFuncLoginParam()
    {
        string strSaveString = LocalSaveUtil.GetValueString(LOCALSAVE_LOGIN, "");
        if (strSaveString.Length <= 0)
        {
            return null;
        }
        return LitJson.JsonMapper.ToObject<RecvLoginParamValue>(strSaveString);
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：アチーブメント演出済み一覧
	*/
    //----------------------------------------------------------------------------
    public uint[] LoadFuncAchievementEnsyutsu()
    {
        //--------------------------------
        // 領域から読み込み
        //--------------------------------
        string strAchievementIDs = LocalSaveUtil.GetValueString(LOCALSAVE_ACHIEVEMENT_ENSYUTSU, "");
        if (strAchievementIDs.Length <= 0)
        {
            return null;
        }

        //--------------------------------
        // 文字列をパラメータに変換
        //--------------------------------
        uint[] aunAchievementIDList = LitJson.JsonMapper.ToObject<uint[]>(strAchievementIDs);
        if (aunAchievementIDList == null)
        {
            return null;
        }
        return aunAchievementIDList;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：お気に入りフレンドを間引き

		@note	自分はお気に入り登録していても、相手の方からフレンド解除されることはある。
		@note	フレンド解除された場合にはフレンドリストから除外されるので、それをチェックしてお気に入りを自動解除する
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncAddFavoriteFriendClip(PacketStructFriend[] acFriendList)
    {
        //--------------------------------
        // フレンドお気に入り情報に追加
        //--------------------------------
        TemplateList<uint> cFavoriteFriendList = LoadFuncAddFavoriteFriend();
        if (cFavoriteFriendList == null)
        {
            return;
        }

        //--------------------------------
        // フレンドリストでお気に入りを間引き
        //--------------------------------
        TemplateList<uint> cSubList = new TemplateList<uint>();
        for (int i = 0; i < cFavoriteFriendList.m_BufferSize; i++)
        {
            bool bFriendHit = false;
            for (int j = 0; j < acFriendList.Length; j++)
            {
                if (acFriendList[j] == null
                || acFriendList[j].friend_state != (uint)FRIEND_STATE.FRIEND_STATE_SUCCESS
                || acFriendList[j].user_id != cFavoriteFriendList[i]
                ) continue;

                bFriendHit = true;
                break;
            }
            if (bFriendHit == true)
            {
                cSubList.Add(cFavoriteFriendList[i]);
            }
        }
        cFavoriteFriendList = cSubList;

        //--------------------------------
        // パラメータを文字列化
        //--------------------------------
        string strSaveString = LitJson.JsonMapper.ToJson(cFavoriteFriendList);

        LocalSaveUtil.SetValueString(LOCALSAVE_FAVORITE_FRIEND, strSaveString);
        LocalSaveUtil.ExecDataSave();

        //--------------------------------
        // 保持しているデータを更新
        //--------------------------------
        m_LocalSaveFavoriteFriend = cFavoriteFriendList;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：サーバー送受信に使用するパケットユニークID
		@note	通算になったので端末セーブ。「可能な限り前回送信情報と重複しない」が重要なので、データ移行等でカウントが０になってもそれはそれで動く
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncPacketUniqueID(uint unPacketID)
    {
        //--------------------------------
        // ulong保存すればいいけど、Unityのセーブ機能はuintのセーブができないので構造体化して文字列化
        //--------------------------------
        LocalSavePacketUniqueID cPacketUniqueID = new LocalSavePacketUniqueID();
        cPacketUniqueID.m_PacketUniqueID = unPacketID;

        //--------------------------------
        // パラメータを文字列化
        //--------------------------------
        string strSaveString = LitJson.JsonMapper.ToJson(cPacketUniqueID);

        LocalSaveUtil.SetValueString(LOCALSAVE_PACKET_UNIQUE_ID, strSaveString);
        LocalSaveUtil.ExecDataSave();
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：サーバー送受信に使用するパケットユニークID
	*/
    //----------------------------------------------------------------------------
    public uint LoadFuncPacketUniqueID()
    {
        //--------------------------------
        // 領域から読み込み
        //--------------------------------
        string strPacketUniqueID = LocalSaveUtil.GetValueString(LOCALSAVE_PACKET_UNIQUE_ID, "");
        if (strPacketUniqueID.Length <= 0)
        {
            // ０番は初期化用なのでIDは１番から振る
            return 1;
        }

        //--------------------------------
        // 文字列をパラメータに変換
        //--------------------------------
        LocalSavePacketUniqueID cPacketUniqueID = LitJson.JsonMapper.ToObject<LocalSavePacketUniqueID>(strPacketUniqueID);
        if (cPacketUniqueID == null)
        {
            return 1;
        }
        return cPacketUniqueID.m_PacketUniqueID;

    }
    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：お気に入りフレンド登録
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncAddFavoriteFriend(uint unUserID, bool bAdd, bool bSub)
    {
        //--------------------------------
        // お気に入り情報取得
        //--------------------------------
        TemplateList<uint> cFavoriteFriendList = LoadFuncAddFavoriteFriend();


        //--------------------------------
        // 要望が追加か削除かで処理分岐
        //--------------------------------
        if (bAdd == true)
        {
            //--------------------------------
            // お気に入り追加処理
            //--------------------------------

            //--------------------------------
            // リストが存在しないなら新規作成
            //--------------------------------
            if (cFavoriteFriendList == null)
            {
                cFavoriteFriendList = new TemplateList<uint>();
            }
            //--------------------------------
            // 既にお気に入り登録されていないかチェック
            //--------------------------------
            for (int i = 0; i < cFavoriteFriendList.m_BufferSize; i++)
            {
                if (cFavoriteFriendList[i] != unUserID)
                {
                    continue;
                }
                return;
            }

            //--------------------------------
            // フレンドお気に入り情報に追加
            //--------------------------------
            cFavoriteFriendList.Add(unUserID);
        }
        else
        if (bSub == true)
        {
            //--------------------------------
            // リストが存在しないならスルー
            //--------------------------------
            if (cFavoriteFriendList == null)
            {
                return;
            }

            //--------------------------------
            // お気に入り登録されているなら破棄
            //--------------------------------
            TemplateList<uint> cSubList = new TemplateList<uint>();
            for (int i = 0; i < cFavoriteFriendList.m_BufferSize; i++)
            {
                if (cFavoriteFriendList[i] == unUserID)
                    continue;
                cSubList.Add(cFavoriteFriendList[i]);
            }
            if (cFavoriteFriendList.m_BufferSize == cSubList.m_BufferSize)
            {
                return;
            }

            cFavoriteFriendList = cSubList;
        }

        //--------------------------------
        // パラメータを文字列化
        //--------------------------------
        string strSaveString = LitJson.JsonMapper.ToJson(cFavoriteFriendList);

        LocalSaveUtil.SetValueString(LOCALSAVE_FAVORITE_FRIEND, strSaveString);
        LocalSaveUtil.ExecDataSave();

        //--------------------------------
        // 保持しているデータを更新
        //--------------------------------
        m_LocalSaveFavoriteFriend = cFavoriteFriendList;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：お気に入りフレンド取得
	*/
    //----------------------------------------------------------------------------
    public TemplateList<uint> LoadFuncAddFavoriteFriend()
    {
        //--------------------------------
        // 一度読み込み済ならキャッシュを返す
        //--------------------------------
        if (m_LocalSaveFavoriteFriend != null)
        {
            return m_LocalSaveFavoriteFriend;
        }

        //--------------------------------
        // 領域から読み込み
        //--------------------------------
        string strFriendList = LocalSaveUtil.GetValueString(LOCALSAVE_FAVORITE_FRIEND, "");
        if (strFriendList.Length <= 0)
        {
            return null;
        }

        //--------------------------------
        // 文字列をパラメータに変換
        //--------------------------------
        TemplateList<uint> cFriendList = LitJson.JsonMapper.ToObject<TemplateList<uint>>(strFriendList);
        if (cFriendList == null)
        {
            return null;
        }

        //--------------------------------
        // 次回以降使いまわすので保持しておく
        //--------------------------------
        m_LocalSaveFavoriteFriend = cFriendList;
        return cFriendList;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：お気に入りユニット登録
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncAddFavoriteUnit(long unUnitID, bool bAdd, bool bSub)
    {
        //--------------------------------
        // ユニットお気に入り情報を取得
        //--------------------------------
        TemplateList<long> cFavoriteUnitList = LoadFuncAddFavoriteUnit();

        //--------------------------------
        // 要望が追加か削除かで処理分岐
        //--------------------------------
        if (bAdd == true)
        {
            //--------------------------------
            // お気に入り追加処理
            //--------------------------------

            //--------------------------------
            // リストが存在しないなら新規作成
            //--------------------------------
            if (cFavoriteUnitList == null)
            {
                cFavoriteUnitList = new TemplateList<long>();
            }
            //--------------------------------
            // 既にお気に入り登録されていないかチェック
            //--------------------------------
            for (int i = 0; i < cFavoriteUnitList.m_BufferSize; i++)
            {
                if (cFavoriteUnitList[i] != unUnitID)
                {
                    continue;
                }
                return;
            }

            //--------------------------------
            // お気に入り情報に追加
            //--------------------------------
            cFavoriteUnitList.Add(unUnitID);
        }
        else
        if (bSub == true)
        {
            //--------------------------------
            // リストが存在しないならスルー
            //--------------------------------
            if (cFavoriteUnitList == null)
            {
                return;
            }

            //--------------------------------
            // お気に入り登録されているなら破棄
            //--------------------------------
            TemplateList<long> cSubList = new TemplateList<long>();
            for (int i = 0; i < cFavoriteUnitList.m_BufferSize; i++)
            {
                if (cFavoriteUnitList[i] == unUnitID)
                    continue;
                cSubList.Add(cFavoriteUnitList[i]);
            }
            if (cFavoriteUnitList.m_BufferSize == cSubList.m_BufferSize)
            {
                return;
            }

            cFavoriteUnitList = cSubList;
        }

        //--------------------------------
        // パラメータを文字列化
        //--------------------------------
        string strSaveString = LitJson.JsonMapper.ToJson(cFavoriteUnitList);

        LocalSaveUtil.SetValueString(LOCALSAVE_FAVORITE_UNIT, strSaveString);
        LocalSaveUtil.ExecDataSave();

        //--------------------------------
        // 次回から使いまわすので保持しておく
        //--------------------------------
        m_LocalSaveFavoriteUnit = cFavoriteUnitList;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：お気に入りユニット取得
	*/
    //----------------------------------------------------------------------------
    public TemplateList<long> LoadFuncAddFavoriteUnit()
    {
        //--------------------------------
        // 既に読み込んでるならキャッシュを返す
        //--------------------------------
        if (m_LocalSaveFavoriteUnit != null)
        {
            return m_LocalSaveFavoriteUnit;
        }

        //--------------------------------
        // 領域からデータ読み込み
        //--------------------------------
        string strFavoriteList = LocalSaveUtil.GetValueString(LOCALSAVE_FAVORITE_UNIT, "");
        if (strFavoriteList.Length <= 0)
        {
            m_LocalSaveFavoriteUnit = new TemplateList<long> { };
            return null;
        }

        //--------------------------------
        // 文字列をパラメータに変換
        //--------------------------------
        TemplateList<long> cFavoriteList = LitJson.JsonMapper.ToObject<TemplateList<long>>(strFavoriteList);
        if (cFavoriteList == null)
        {
            m_LocalSaveFavoriteUnit = new TemplateList<long> { };
            return null;
        }

        //--------------------------------
        // 保持して次回以降使いまわす
        //--------------------------------
        m_LocalSaveFavoriteUnit = cFavoriteList;
        return cFavoriteList;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：フレンド使用情報
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncUseFriend(uint unUserID)
    {
        //--------------------------------
        //	ダミーフレンドの使用情報は保存しない
        //--------------------------------
        if (unUserID == GlobalDefine.FRIEND_DUMMY_ID)
        {
            return;
        }

        //--------------------------------
        // フレンド使用情報を取得
        //--------------------------------
        TemplateList<LocalSaveFriendUse> cUseFriendList = LoadFuncUseFriend();
        if (cUseFriendList == null)
        {
            cUseFriendList = new TemplateList<LocalSaveFriendUse>();
        }

        bool bFriendAssigned = false;

        //--------------------------------
        // フレンドリストに既にそのユーザーがいるなら使いまわし
        //--------------------------------
        if (bFriendAssigned == false)
        {
            for (int i = 0; i < cUseFriendList.m_BufferSize; i++)
            {
                if (cUseFriendList[i] == null
                || cUseFriendList[i].m_FriendID != unUserID
                ) continue;

                cUseFriendList[i].m_FriendUseTime = TimeUtil.ConvertLocalTimeToServerTime(TimeManager.Instance.m_TimeNow);
                bFriendAssigned = true;
                break;
            }
        }

        //--------------------------------
        // 既に有効期限の切れた人がいるならその領域使いまわし
        //--------------------------------
        if (bFriendAssigned == false)
        {
            for (int i = 0; i < cUseFriendList.m_BufferSize; i++)
            {
                if (cUseFriendList[i] == null)
                    continue;

                DateTime cTime = TimeUtil.ConvertServerTimeToLocalTime(cUseFriendList[i].m_FriendUseTime);
                TimeSpan cTimeSpan = TimeManager.Instance.m_TimeNow - cTime;

                if (cTimeSpan.TotalHours < GlobalDefine.FRIEND_CYCLE_HOURS)
                    continue;

                cUseFriendList[i].m_FriendID = unUserID;
                cUseFriendList[i].m_FriendUseTime = TimeUtil.ConvertLocalTimeToServerTime(TimeManager.Instance.m_TimeNow);
                bFriendAssigned = true;
                break;
            }
        }

        //--------------------------------
        // まだ解決しないなら領域追加
        //--------------------------------
        if (bFriendAssigned == false)
        {
            LocalSaveFriendUse cLocalSaveFriendUse = new LocalSaveFriendUse();

            cLocalSaveFriendUse.m_FriendID = unUserID;
            cLocalSaveFriendUse.m_FriendUseTime = TimeUtil.ConvertLocalTimeToServerTime(TimeManager.Instance.m_TimeNow);

            cUseFriendList.Add(cLocalSaveFriendUse);

            bFriendAssigned = true;
        }



        //--------------------------------
        // パラメータを文字列化
        //--------------------------------
        string strSaveString = LitJson.JsonMapper.ToJson(cUseFriendList);

        LocalSaveUtil.SetValueString(LOCALSAVE_FRIEND_USE, strSaveString);
        LocalSaveUtil.ExecDataSave();

        //--------------------------------
        // 次回以降使いまわすので保持しておく
        //--------------------------------
        m_LocalSaveFriendUse = cUseFriendList;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：フレンド使用情報
	*/
    //----------------------------------------------------------------------------
    public TemplateList<LocalSaveFriendUse> LoadFuncUseFriend()
    {
        //--------------------------------
        // 一度読み込み済ならキャッシュを返す
        //--------------------------------
        if (m_LocalSaveFriendUse != null)
        {
            return m_LocalSaveFriendUse;
        }

        //--------------------------------
        // 領域から読み込み
        //--------------------------------
        string strUseFriendList = LocalSaveUtil.GetValueString(LOCALSAVE_FRIEND_USE, "");
        if (strUseFriendList.Length <= 0)
        {
            return null;
        }

        //--------------------------------
        // 文字列をパラメータに変換
        //--------------------------------
        TemplateList<LocalSaveFriendUse> cUseFriendList = LitJson.JsonMapper.ToObject<TemplateList<LocalSaveFriendUse>>(strUseFriendList);
        if (cUseFriendList == null)
        {
            return null;
        }

        //--------------------------------
        // 次回以降使いまわすので保持しておく
        //--------------------------------
        m_LocalSaveFriendUse = cUseFriendList;
        return cUseFriendList;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	判定実行：フレンド使用情報
	*/
    //----------------------------------------------------------------------------
    public LocalSaveFriendUse GetLocalSaveUseFriend(uint unUserID)
    {
        //--------------------------------
        // フレンド使用情報を取得
        //--------------------------------
        TemplateList<LocalSaveFriendUse> cUseFriendList = LoadFuncUseFriend();
        if (cUseFriendList == null)
        {
            return null;
        }

        //--------------------------------
        // フレンド使用情報を取得
        //--------------------------------
        for (int i = 0; i < cUseFriendList.m_BufferSize; i++)
        {
            if (cUseFriendList[i] == null
            || cUseFriendList[i].m_FriendID != unUserID
            ) continue;

            //--------------------------------
            // 固定時間経過で時効になるので
            // 時効だったらnull返す
            //--------------------------------
            DateTime cTime = TimeUtil.ConvertServerTimeToLocalTime(cUseFriendList[i].m_FriendUseTime);
            TimeSpan cTimeSpan = TimeManager.Instance.m_TimeNow - cTime;
            if (cTimeSpan.TotalHours > GlobalDefine.FRIEND_CYCLE_HOURS)
            {
                return null;
            }
            return cUseFriendList[i];
        }
        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：ローカル通知リクエストフラグリスト
		@param	unRequestEventID	(uint)		リクエストするイベントID
		@param	bNotification		(bool)		通知有無(falseの時は削除する)
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncNotificationRequest(uint unRequestEventID, bool bNotification)
    {
        //--------------------------------
        // フレンド使用情報を取得
        //--------------------------------
        TemplateList<LocalSaveEventNotification> cNotificationRequestList = LoadFuncNotificationRequest();
        if (cNotificationRequestList == null)
        {
            cNotificationRequestList = new TemplateList<LocalSaveEventNotification>();
        }
        // 更新終了フラグ
        bool bFinishUpdate = false;
        //--------------------------------
        // リスト内にリクエストIDが存在するかチェック
        //--------------------------------
        for (int i = 0; i < cNotificationRequestList.m_BufferSize; i++)
        {
            if (cNotificationRequestList[i] == null
            || cNotificationRequestList[i].m_FixID != unRequestEventID
            ) continue;

            bFinishUpdate = true;
            // 値を入れる
            cNotificationRequestList[i].m_Push = bNotification;
            break;
        }
        // 値が無かった場合
        //--------------------------------
        // 追加する
        //--------------------------------
        if (bFinishUpdate == false)
        {
            bFinishUpdate = true;
            LocalSaveEventNotification cAddObject = new LocalSaveEventNotification();
            cAddObject.m_FixID = unRequestEventID;
            cAddObject.m_Push = bNotification;

            cNotificationRequestList.Add(cAddObject);
        }



        //--------------------------------
        // パラメータを文字列化
        //--------------------------------
        string strSaveString = LitJson.JsonMapper.ToJson(cNotificationRequestList);

        LocalSaveUtil.SetValueString(LOCALSAVE_NOTIFICATION_REQUEST, strSaveString);
        LocalSaveUtil.ExecDataSave();

        //--------------------------------
        // 次回以降使いまわすので保持しておく
        //--------------------------------
        m_LocalSaveNotificationRequest = cNotificationRequestList;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ローカル通知リクエストフラグリスト
	*/
    //----------------------------------------------------------------------------
    public TemplateList<LocalSaveEventNotification> LoadFuncNotificationRequest()
    {
        //--------------------------------
        // 一度読み込み済ならキャッシュを返す
        //--------------------------------
        if (m_LocalSaveNotificationRequest != null)
        {
            return m_LocalSaveNotificationRequest;
        }

        //--------------------------------
        // 領域から読み込み
        //--------------------------------
        string strNotificationRequest = LocalSaveUtil.GetValueString(LOCALSAVE_NOTIFICATION_REQUEST, "");
        if (strNotificationRequest.Length <= 0)
        {
            return null;
        }

        //--------------------------------
        // 文字列をパラメータに変換
        //--------------------------------
        TemplateList<LocalSaveEventNotification> cNotificationRequestList = LitJson.JsonMapper.ToObject<TemplateList<LocalSaveEventNotification>>(strNotificationRequest);
        if (cNotificationRequestList == null)
        {
            return null;
        }

        //--------------------------------
        // 次回以降使いまわすので保持しておく
        //--------------------------------
        m_LocalSaveNotificationRequest = cNotificationRequestList;
        return cNotificationRequestList;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	判定実行：ローカル通知リクエストフラグリスト
	*/
    //----------------------------------------------------------------------------
    public LocalSaveEventNotification CheckFuncNotificationRequest(uint unRequestEventID)
    {
        //--------------------------------
        // フレンド使用情報を取得
        //--------------------------------
        TemplateList<LocalSaveEventNotification> cNotificationRequestList = LoadFuncNotificationRequest();
        if (cNotificationRequestList == null)
        {
            return null;
        }

        //--------------------------------
        // リスト内にリクエストIDが存在するかチェック
        //--------------------------------
        for (int i = 0; i < cNotificationRequestList.m_BufferSize; i++)
        {
            if (cNotificationRequestList[i] == null
            || cNotificationRequestList[i].m_FixID != unRequestEventID
            ) continue;

            // リクエストIDと一致するものがある
            return cNotificationRequestList[i];
        }

        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	削除実行：ローカル通知リクエストフラグリスト
	*/
    //----------------------------------------------------------------------------
    public void RemoveFuncNotificationRequest(uint unRequestEventID)
    {
        //--------------------------------
        // フレンド使用情報を取得
        //--------------------------------
        TemplateList<LocalSaveEventNotification> cNotificationRequestList = LoadFuncNotificationRequest();
        if (cNotificationRequestList == null)
        {
            return;
        }

        //--------------------------------
        // リスト内にリクエストIDが存在するかチェック
        //--------------------------------
        for (int i = 0; i < cNotificationRequestList.m_BufferSize; i++)
        {
            if (cNotificationRequestList[i] == null
            || cNotificationRequestList[i].m_FixID != unRequestEventID
            ) continue;

            // リクエストIDと一致するものがある
            cNotificationRequestList.Remove(cNotificationRequestList[i]);
            break;
        }

        //--------------------------------
        // 次回以降使いまわすので保持しておく
        //--------------------------------
        m_LocalSaveNotificationRequest = cNotificationRequestList;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	保存実行：現行のデータを保存する
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncNotificationRequest()
    {
        //--------------------------------
        // パラメータを文字列化
        //--------------------------------
        if (m_LocalSaveNotificationRequest != null)
        {
            string strSaveString = LitJson.JsonMapper.ToJson(m_LocalSaveNotificationRequest);

            LocalSaveUtil.SetValueString(LOCALSAVE_NOTIFICATION_REQUEST, strSaveString);
            LocalSaveUtil.ExecDataSave();
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：スクラッチ引き情報：
		@param	入手ユニットユニークID

		@note	ガチャめくり直前に落ちても演出だけやれるように例外対応
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncScratch(LocalSaveScratch sScratchParam)
    {
        if (sScratchParam == null)
        {
            LocalSaveUtil.SetValueString(LOCALSAVE_SCRATCH, "");
            LocalSaveUtil.ExecDataSave();

#if BUILD_TYPE_DEBUG
            Debug.Log("Scratch Param Clear!");
#endif
        }
        else
        {
            string strSaveString = LitJson.JsonMapper.ToJson(sScratchParam);

            LocalSaveUtil.SetValueString(LOCALSAVE_SCRATCH, strSaveString);
            LocalSaveUtil.ExecDataSave();

#if BUILD_TYPE_DEBUG
            Debug.Log("Scratch Param Save ... ");
#endif
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：スクラッチ引き情報：演出スキップ
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncScratchDetailSkip(bool sw)
    {
        LocalSaveUtil.SetValueBool(LOCALSAVE_SCRATCH_DETAIL_SKIP, sw);
        LocalSaveUtil.ExecDataSave();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：スクラッチ引き情報：演出スキップ
	*/
    //----------------------------------------------------------------------------
    public bool LoadFuncScratchDetailSkip()
    {
        bool sw = LocalSaveUtil.GetValueBool(LOCALSAVE_SCRATCH_DETAIL_SKIP, true);
        return sw;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：インゲーム課金情報：コンティニュー回数
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncInGameContinue(LocalSaveContinue cContinueParam)
    {
        if (cContinueParam == null)
        {
            LocalSaveUtilToInstallFolder.RemoveBattleSetting(LOCALSAVE_INGAME_CONTINUE);

#if BUILD_TYPE_DEBUG
            Debug.Log("Continue Param Clear!");
#endif
        }
        else
        {
            string strSaveString = LitJson.JsonMapper.ToJson(cContinueParam);

            LocalSaveUtilToInstallFolder.SaveBattleSetting(LOCALSAVE_INGAME_CONTINUE, strSaveString);


#if BUILD_TYPE_DEBUG
            Debug.Log("Continue Param Save ... " + cContinueParam.nContinueCt + " , " + cContinueParam.nContinueNext);
#endif
        }
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：インゲーム課金情報：コンティニュー回数
	*/
    //----------------------------------------------------------------------------
    public LocalSaveContinue LoadFuncInGameContinue()
    {
        string strSaveString = LocalSaveUtilToInstallFolder.LoadBattleSetting(LOCALSAVE_INGAME_CONTINUE);
        if (strSaveString.Length <= 0)
        {
            // ※回数が０だとサーバーに弾かれるので１から始める
            LocalSaveContinue cContinue = new LocalSaveContinue();
            cContinue.nContinueCt = 1;
            cContinue.nContinueNext = 1;

            SaveFuncInGameContinue(cContinue);
            return cContinue;
        }
        return LitJson.JsonMapper.ToObject<LocalSaveContinue>(strSaveString);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：インゲーム課金情報：リセット回数
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncInGameReset(LocalSaveReset cResetParam)
    {
        if (cResetParam == null)
        {
            LocalSaveUtilToInstallFolder.RemoveBattleSetting(LOCALSAVE_INGAME_RESET);

#if BUILD_TYPE_DEBUG
            Debug.Log("Reset Param Clear!");
#endif
        }
        else
        {
            string strSaveString = LitJson.JsonMapper.ToJson(cResetParam);

            LocalSaveUtilToInstallFolder.SaveBattleSetting(LOCALSAVE_INGAME_RESET, strSaveString);

#if BUILD_TYPE_DEBUG
            Debug.Log("Reset Param Save ... " + cResetParam.nResetCt + " , " + cResetParam.nResetNext);
#endif
        }
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：インゲーム課金情報：リセット回数
	*/
    //----------------------------------------------------------------------------
    public LocalSaveReset LoadFuncInGameReset()
    {
        string strSaveString = LocalSaveUtilToInstallFolder.LoadBattleSetting(LOCALSAVE_INGAME_RESET);
        if (strSaveString.Length <= 0)
        {
            // ※回数が０だとサーバーに弾かれるので１から始める
            LocalSaveReset cResetParam = new LocalSaveReset();
            cResetParam.nResetCt = 1;
            cResetParam.nResetNext = 1;

            SaveFuncInGameReset(cResetParam);

            return cResetParam;
        }
        return LitJson.JsonMapper.ToObject<LocalSaveReset>(strSaveString);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：クエスト開始前フレンドポイント付与フラグ保存
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncQuestFriendPointActive(uint unFriendPoint, uint unFriendPointHave)
    {
        {
            LocalSaveQuestFriend cQuestFriendPt = new LocalSaveQuestFriend();
            cQuestFriendPt.m_FriendPt = unFriendPoint;
            cQuestFriendPt.m_FriendPtHave = unFriendPointHave;

            string strSaveString = LitJson.JsonMapper.ToJson(cQuestFriendPt);

            LocalSaveUtil.SetValueString(LOCALSAVE_FRIEND_POINT_ACTIVE, strSaveString);
            LocalSaveUtil.ExecDataSave();

#if BUILD_TYPE_DEBUG
            Debug.Log("Quest FriendPoint Save ... " + strSaveString);
#endif
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：ユーザーID
		@note	タイトルでのエラー表示用
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncUserID(uint unUserID)
    {
        LocalSaveUtil.SetValueString(LOCALSAVE_USER_ID, unUserID.ToString());
        LocalSaveUtil.ExecDataSave();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ユーザーID
		@note	タイトルでのエラー表示用
	*/
    //----------------------------------------------------------------------------
    public uint LoadFuncUserID()
    {
        string strUserID = LocalSaveUtil.GetValueString(LOCALSAVE_USER_ID, "0");
        uint unUserID = 0;
        if (uint.TryParse(strUserID, out unUserID) == true)
        {
            return unUserID;
        }
        return 0;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：UUID
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncUUID(string strUUID)
    {
        LocalSaveUtil.SetValueString(LOCALSAVE_UUID, strUUID);
        LocalSaveUtil.ExecDataSave();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：UUID
	*/
    //----------------------------------------------------------------------------
    public string LoadFuncUUID()
    {
        return LocalSaveUtil.GetValueString(LOCALSAVE_UUID, "");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	存在チェック：UUID
	*/
    //----------------------------------------------------------------------------
    public bool CheckUUID()
    {
        string strUUID = LocalSaveUtil.GetValueString(LOCALSAVE_UUID, "");
        if (strUUID.Length > 0)
        {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：UUIDチェック用
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncTitleUUID()
    {
        LocalSaveUtil.SetValueString(LOCALSAVE_TITLE_UUID, LoadFuncUUID());
        LocalSaveUtil.ExecDataSave();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	存在チェック：UUIDチェック用
	*/
    //----------------------------------------------------------------------------
    public void DiffTitleUUID(string path)
    {
        string baseUUID = LoadFuncUUID();
        string titleUUID = LocalSaveUtil.GetValueString(LOCALSAVE_TITLE_UUID, "");

        if (titleUUID.Length <= 0)
        {
            return;
        }

        if (titleUUID == baseUUID)
        {
            return;
        }

        return;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：利用規約の同意済
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncInformationOK(AGREEMENT eAgree)
    {
        LocalSaveUtil.SetValueInt(LOCALSAVE_INFORMATION, (int)eAgree);
        LocalSaveUtil.ExecDataSave();
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：利用規約の同意済
	*/
    //----------------------------------------------------------------------------
    public string LoadFuncInformationVer()
    {
        return LocalSaveUtil.GetValueString(LOCALSAVE_INFORMATION_VER, "");
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：利用規約の同意済
	*/
    //----------------------------------------------------------------------------
    public int LoadFuncInformationOK()
    {
        //--------------------------------
        // 同意ページのバージョンが変わっている場合、
        // 再度規約を表示させるためにステータスをクリア
        //--------------------------------
        string strInformationVer = LocalSaveUtil.GetValueString(LOCALSAVE_INFORMATION_VER, "");

        if (strInformationVer != Patcher.Instance.GetPolicy())
        {
            //--------------------------------
            // 現状のフラグをもとに進行状況を加工。
            // 初期状態の場合はFoxSDKの通信を行いたいので、規約の処理を一度でも実行しているかどうかでフラグ分岐。
            //--------------------------------
            int nNowState = LocalSaveUtil.GetValueInt(LOCALSAVE_INFORMATION, 0);
            switch (nNowState)
            {
                case (int)AGREEMENT.NONE: PlayerPrefs.SetInt("agree_secondTime_kiyaku", 0); break;
                case (int)AGREEMENT.FOX_CALLED: SaveFuncInformationOK(AGREEMENT.FOX_CALLED); PlayerPrefs.SetInt("agree_ok", 0); PlayerPrefs.SetInt("agree_secondTime_kiyaku", 0); break;
                case (int)AGREEMENT.AGREE_OK: SaveFuncInformationOK(AGREEMENT.FOX_CALLED); PlayerPrefs.SetInt("agree_ok", 0); PlayerPrefs.SetInt("agree_secondTime_kiyaku", 1); break;
            }

            //--------------------------------
            // バージョン更新を検地したことをセーブ。
            //--------------------------------
            LocalSaveUtil.SetValueString(LOCALSAVE_INFORMATION_VER, Patcher.Instance.GetPolicy());
            LocalSaveUtil.ExecDataSave();

            //Debug.LogError( "InformationVer - " + strInformationVer + " -> " + GlobalDefine.INFORMATION_VER_KIYAKU );
        }

        //--------------------------------
        // 現状のステータスを返す
        //--------------------------------
        return LocalSaveUtil.GetValueInt(LOCALSAVE_INFORMATION, 0);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：ポリシーの同意済
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncInformationPolicyOK(AGREEMENT eAgree)
    {
        LocalSaveUtil.SetValueInt(LOCALSAVE_INFORMATION_POLICY, (int)eAgree);
        LocalSaveUtil.ExecDataSave();
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ポリシーの同意済
	*/
    //----------------------------------------------------------------------------
    public string LoadFuncInformationPolicyVer()
    {
        return LocalSaveUtil.GetValueString(LOCALSAVE_INFORMATION_POLICY_VER, "");
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ポリシーの同意済
	*/
    //----------------------------------------------------------------------------
    public int LoadFuncInformationPolicyOK()
    {
        //--------------------------------
        // 同意ページのバージョンが変わっている場合、
        // 再度規約を表示させるためにステータスをクリア
        //--------------------------------
        string strInformationVer = LocalSaveUtil.GetValueString(LOCALSAVE_INFORMATION_POLICY_VER, "");


        if (strInformationVer != Patcher.Instance.GetPrivacyPolicy())
        {
            //--------------------------------
            // 現状のフラグをもとに進行状況を加工。
            // 初期状態の場合はFoxSDKの通信を行いたいので、規約の処理を一度でも実行しているかどうかでフラグ分岐。
            //--------------------------------
            int nNowState = LocalSaveUtil.GetValueInt(LOCALSAVE_INFORMATION_POLICY, 0);
            switch (nNowState)
            {
                case (int)AGREEMENT.NONE: PlayerPrefs.SetInt("agree_secondTime_policy", 0); break;
                case (int)AGREEMENT.FOX_CALLED: SaveFuncInformationPolicyOK(AGREEMENT.FOX_CALLED); PlayerPrefs.SetInt("agree_ok_policy", 0); PlayerPrefs.SetInt("agree_secondTime_policy", 0); break;
                case (int)AGREEMENT.AGREE_OK: SaveFuncInformationPolicyOK(AGREEMENT.FOX_CALLED); PlayerPrefs.SetInt("agree_ok_policy", 0); PlayerPrefs.SetInt("agree_secondTime_policy", 1); break;
            }

            //--------------------------------
            // バージョン更新を検地したことをセーブ。
            //--------------------------------
            LocalSaveUtil.SetValueString(LOCALSAVE_INFORMATION_POLICY_VER, Patcher.Instance.GetPrivacyPolicy());
            LocalSaveUtil.ExecDataSave();

            //Debug.LogError( "InformationPolicyVer - " + strInformationVer + " -> " + GlobalDefine.INFORMATION_VER_POLICY );
        }

        //--------------------------------
        // 現状のステータスを返す
        //--------------------------------
        return LocalSaveUtil.GetValueInt(LOCALSAVE_INFORMATION_POLICY, 0);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：お好みソート情報
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncSortFilterState(string strFilterKey, LocalSaveSortInfo cInfo)
    {
        //--------------------------------
        // パラメータを文字列化してセーブ
        //--------------------------------
        string strSaveString = LitJson.JsonMapper.ToJson(cInfo);
        LocalSaveUtil.SetValueString(strFilterKey, strSaveString);
        LocalSaveUtil.ExecDataSave();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ソート情報
	*/
    //----------------------------------------------------------------------------
    public LocalSaveSortInfo LoadFuncSortFilterState(string strSortFilterKey)
    {
        //--------------------------------
        // 領域からデータ読み込み
        //--------------------------------
        string strSortFilter = LocalSaveUtil.GetValueString(strSortFilterKey, "");
        if (strSortFilter.Length <= 0)
        {
            //--------------------------------
            // 初期設定でセーブを作成しておく
            //--------------------------------
            LocalSaveSortInfo cLocalSave = new LocalSaveSortInfo();
            cLocalSave.InitParam();
            SaveFuncSortFilterState(strSortFilterKey, cLocalSave);
            strSortFilter = LocalSaveUtil.GetValueString(strSortFilterKey, "");
            if (strSortFilter.Length <= 0)
            {
                Debug.LogError("SaveData FavoriteSort Create Error!");
                return null;
            }
        }

        //--------------------------------
        // 文字列をパラメータに変換
        //--------------------------------
        LocalSaveSortInfo cInfo = LitJson.JsonMapper.ToObject<LocalSaveSortInfo>(strSortFilter);
        if (cInfo == null)
        {
            Debug.LogError("SaveData FavoriteSort ToObject Error!");
            return null;
        }

        return cInfo;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncSortFilterPartyForm(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_PARTY_FORM, cInfo); }                            // ソートセーブロード：パーティ編成
    public void SaveFuncSortFilterBuildUnit(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_BUILD_UNIT, cInfo); }                            // ソートセーブロード：強化合成ユニット
    public void SaveFuncSortFilterBuildFriend(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_BUILD_FRIEND, cInfo); }                        // ソートセーブロード：強化合成フレンド
    public void SaveFuncSortFilterEvolveUnit(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_EVOLVE_UNIT, cInfo); }                          // ソートセーブロード：進化合成ユニット
    public void SaveFuncSortFilterEvolveFriend(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_EVOLVE_FRIEND, cInfo); }                      // ソートセーブロード：進化合成フレンド
    public void SaveFuncSortFilterLinkUnit(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_LINK_UNIT, cInfo); }                              // ソートセーブロード：リンクユニット
    public void SaveFuncSortFilterUnitSale(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_UNIT_SALE, cInfo); }                              // ソートセーブロード：ユニット売却
    public void SaveFuncSortFilterUnitList(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_UNIT_LIST, cInfo); }                              // ソートセーブロード：ユニット一覧
    public void SaveFuncSortFilterFriendList(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_FRIEND_LIST, cInfo); }                          // ソートセーブロード：フレンド一覧
    public void SaveFuncSortFilterFriendWaitMe(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_FRIEND_WAIT_ME, cInfo); }                     // ソートセーブロード：フレンド申請受け中
    public void SaveFuncSortFilterFriendWaitHim(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_FRIEND_WAIT_HIM, cInfo); }                   // ソートセーブロード：フレンド申請出し中
    public void SaveFuncSortFilterQuestFriend(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_QUEST_FRIEND, cInfo); }                        // ソートセーブロード：クエスト前フレンド
    public void SaveFuncSortFilterAchievement(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_ACHIEVEMENT, cInfo); }                         // ソートセーブロード：ミッション
    public void SaveFuncSortFilterUnitPointLO(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_UNIT_POINT_LO, cInfo); }       // ソートセーブロード：ポイントショップ限界突破
    public void SaveFuncSortFilterUnitPointEvolve(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_UNIT_POINT_EVOLVE, cInfo); }       // ソートセーブロード：ポイントショップ限界突破
    public void SaveFuncSortFilterAchievementGp(LocalSaveSortInfo cInfo) { SaveFuncSortFilterState(LOCALSAVE_SORT_FILTER_ACHIEVEMENT_GP, cInfo); }                    // ソートセーブロード：ミッショングループ

    public LocalSaveSortInfo LoadFuncSortFilterPartyForm() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_PARTY_FORM); }                                      // ソートセーブロード：パーティ編成
    public LocalSaveSortInfo LoadFuncSortFilterBuildUnit() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_BUILD_UNIT); }                                      // ソートセーブロード：強化合成ユニット
    public LocalSaveSortInfo LoadFuncSortFilterBuildFriend() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_BUILD_FRIEND); }                                  // ソートセーブロード：強化合成フレンド
    public LocalSaveSortInfo LoadFuncSortFilterEvolveUnit() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_EVOLVE_UNIT); }                                    // ソートセーブロード：進化合成ユニット
    public LocalSaveSortInfo LoadFuncSortFilterEvolveFriend() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_EVOLVE_FRIEND); }                                // ソートセーブロード：進化合成フレンド
    public LocalSaveSortInfo LoadFuncSortFilterLinkUnit() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_LINK_UNIT); }                                        // ソートセーブロード：リンクユニット
    public LocalSaveSortInfo LoadFuncSortFilterUnitSale() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_UNIT_SALE); }                                        // ソートセーブロード：ユニット売却
    public LocalSaveSortInfo LoadFuncSortFilterUnitList() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_UNIT_LIST); }                                        // ソートセーブロード：ユニット一覧
    public LocalSaveSortInfo LoadFuncSortFilterFriendList() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_FRIEND_LIST); }                                    // ソートセーブロード：フレンド一覧
    public LocalSaveSortInfo LoadFuncSortFilterFriendWaitMe() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_FRIEND_WAIT_ME); }                               // ソートセーブロード：フレンド申請受け中
    public LocalSaveSortInfo LoadFuncSortFilterFriendWaitHim() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_FRIEND_WAIT_HIM); }                             // ソートセーブロード：フレンド申請出し中
    public LocalSaveSortInfo LoadFuncSortFilterQuestFriend() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_QUEST_FRIEND); }                                  // ソートセーブロード：クエスト前フレンド
    public LocalSaveSortInfo LoadFuncSortFilterAchievement() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_ACHIEVEMENT); }                                   // ソートセーブロード：ミッション
    public LocalSaveSortInfo LoadFuncSortFilterUnitPointLO() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_UNIT_POINT_LO); }                                 // ソートセーブロード：ポイントショップ限界突破
    public LocalSaveSortInfo LoadFuncSortFilterUnitPointEvolve() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_UNIT_POINT_EVOLVE); }                                    // ソートセーブロード：進化合成ユニット
    public LocalSaveSortInfo LoadFuncSortFilterAchievementGp() { return LoadFuncSortFilterState(LOCALSAVE_SORT_FILTER_ACHIEVEMENT_GP); }                              // ソートセーブロード：ミッショングループ

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：API検証用アドレス
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncServerAddressIP(string strAddress)
    {
        LocalSaveUtil.SetValueString(LOCALSAVE_SERVER_IP, strAddress);
        LocalSaveUtil.ExecDataSave();
        GlobalDefine.ResetPaths();
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：API検証用アドレス
	*/
    //----------------------------------------------------------------------------
    public string LoadFuncServerAddressIP()
    {
        // Patchでreviewへの切り替え
        // DVGAN-2130 参照
        if (Patcher.Instance.isNextVersion())
        {
            return GlobalDefine.API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE; 
        }

#if _MASTER_BUILD
		return GlobalDefine.API_TEST_ADDRESS_ONLINE;
#elif ADDRESS_DEVELOP_1
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_DEVELOP_1_NEW_GOE );
#elif ADDRESS_DEVELOP_1_IP
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_DEVELOP_1_IP_GOE );
#elif ADDRESS_DEVELOP_0
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_DEVELOP_0_GOE );
#elif ADDRESS_STAGING_0
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_0_NEW_GOE );
#elif ADDRESS_STAGING_IP_0
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_0_IP_GOE );
#elif ADDRESS_STAGING_1
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_1_NEW_GOE );
#elif ADDRESS_STAGING_IP_1
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_1_IP_GOE );
#elif ADDRESS_STAGING_2a
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_2a_GOE );
#elif ADDRESS_STAGING_2b
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_2b_GOE );
#elif ADDRESS_STAGING_2c
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_2c_GOE );
#elif ADDRESS_STAGING_3a
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_3a_GOE );
#elif ADDRESS_STAGING_3b
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_3b_GOE );
#elif ADDRESS_STAGING_3c
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_3c_GOE );
#elif ADDRESS_STAGING_REVIEW
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE );
#elif ADDRESS_STAGING_REVIEW_IP
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_REVIEW_IP_GOE );
#elif ADDRESS_ONLINE
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_ONLINE );
#elif ADDRESS_STAGING_REHEARSAL
		return LocalSaveUtil.GetValueString( LOCALSAVE_SERVER_IP , GlobalDefine.API_TEST_ADDRESS_STAGING_REHEARSAL_GOE );
#else
        return LocalSaveUtil.GetValueString(LOCALSAVE_SERVER_IP, GlobalDefine.API_TEST_ADDRESS_DEVELOP_1_IP_GOE);
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：基本パラメータ
		@note	PlayerPrefs にはロードは必要ないので各機能にパラメータを反映するだけ
	*/
    //----------------------------------------------------------------------------
    public LocalSaveStruct LoadFuncLocalData()
    {
        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：オプションパラメータ
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncOption(LocalSaveOption cLocalSaveOption)
    {
        //--------------------------------
        // パラメータを文字列化してセーブ
        //--------------------------------
        string strSaveString = LitJson.JsonMapper.ToJson(cLocalSaveOption);
        LocalSaveUtil.SetValueString(LOCALSAVE_OPTION, strSaveString);
        LocalSaveUtil.ExecDataSave();

        //--------------------------------
        // 次回から使いまわすので保持しておく
        //--------------------------------
        m_LocalSaveOption = cLocalSaveOption;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：オプションパラメータ
		@note	PlayerPrefs にはロードは必要ないので各機能にパラメータを反映するだけ
	*/
    //----------------------------------------------------------------------------
    public LocalSaveOption LoadFuncOption()
    {
        //--------------------------------
        // 既に読み込んでるならキャッシュを返す
        //--------------------------------
        if (m_LocalSaveOption != null)
        {
            return m_LocalSaveOption;
        }

        //--------------------------------
        // 領域からデータ読み込み
        //--------------------------------
        string strOption = LocalSaveUtil.GetValueString(LOCALSAVE_OPTION, "");
        if (strOption.Length <= 0)
        {
            //--------------------------------
            // 初期設定でセーブを作成しておく
            //--------------------------------
            LocalSaveOption cLocalSaveOption = new LocalSaveOption();
            cLocalSaveOption.m_OptionBGM = (int)LocalSaveDefine.OptionBGM.ON;
            cLocalSaveOption.m_OptionGuide = (int)LocalSaveDefine.OptionGuide.ON;
            cLocalSaveOption.m_OptionSE = (int)LocalSaveDefine.OptionSE.ON;
            cLocalSaveOption.m_OptionTIPS = (int)LocalSaveDefine.OptionTips.ON;
            cLocalSaveOption.m_OptionVoice = (int)LocalSaveDefine.OptionVoice.ON;
            cLocalSaveOption.m_OptionSpeed = (int)LocalSaveDefine.OptionSpeed.OFF;
            cLocalSaveOption.m_OptionBattleSkillTurn = (int)LocalSaveDefine.OptionBattleSkillTurn.OFF;
            cLocalSaveOption.m_OptionConfirmAS = (int)LocalSaveDefine.OptionConfirmAS.ON;
            cLocalSaveOption.m_OptionBattleSkillCost = (int)LocalSaveDefine.OptionBattleSkillCost.OFF;
            cLocalSaveOption.m_OptionBattleAchieve = (int)LocalSaveDefine.OptionBattleAchieve.OFF;
            cLocalSaveOption.m_OptionQuestEndTips = (int)LocalSaveDefine.OptionQuestEndTips.ON;
            cLocalSaveOption.m_OptionAutoPlayEnable = (int)LocalSaveDefine.OptionAutoPlayEnable.OFF;
            cLocalSaveOption.m_OptionAutoPlayStopBoss = (int)LocalSaveDefine.OptionAutoPlayStopBoss.OFF;
            cLocalSaveOption.m_OptionAutoPlayUseAS = (int)LocalSaveDefine.OptionAutoPlayUseAS.ON;
            cLocalSaveOption.m_OptionNotification = (int)LocalSaveDefine.OptionNotification.ON;
            cLocalSaveOption.m_NotificationCasino = (int)LocalSaveDefine.OptionNotificationCasino.ON;
            cLocalSaveOption.m_NotificationEvent = (int)LocalSaveDefine.OptionNotificationEvent.ON;
            cLocalSaveOption.m_NotificationSeisekiden = (int)LocalSaveDefine.OptionNotificationSeisekiden.ON;
            SaveFuncOption(cLocalSaveOption);
            strOption = LocalSaveUtil.GetValueString(LOCALSAVE_OPTION, "");
            if (strOption.Length <= 0)
            {
                Debug.LogError("SaveData Option Create Error!");
                return null;
            }
        }

        //--------------------------------
        // 文字列をパラメータに変換
        //--------------------------------
        LocalSaveOption cOption = LitJson.JsonMapper.ToObject<LocalSaveOption>(strOption);
        if (cOption == null)
        {
            Debug.LogError("SaveData Option ToObject Error!");
            return null;
        }

        //--------------------------------
        // 保持して次回以降使いまわす
        //--------------------------------
        m_LocalSaveOption = cOption;
        return m_LocalSaveOption;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ローカルセーブ識別名：インゲーム復帰関連：中断復帰パラメータバージョンが破棄対象でないかチェック
	*/
    //----------------------------------------------------------------------------
    public bool LoadFuncRestoreVersionCheck()
    {
        string strLoadString = LocalSaveUtil.GetValueString(LOCALSAVE_RESTORE_VERSION, "");
        if (strLoadString != LOCALSAVE_RESTORE_VERSION_ID)
        {
            Debug.LogError("Version Restore Delete!");
            return false;
        }
        return true;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ローカルセーブ識別名：インゲーム復帰関連：中断復帰パラメータバージョン
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncRestoreVersion()
    {
        Debug.LogError("Version Restore Save! - " + LOCALSAVE_RESTORE_VERSION_ID);
        LocalSaveUtil.SetValueString(LOCALSAVE_RESTORE_VERSION, LOCALSAVE_RESTORE_VERSION_ID);
        LocalSaveUtil.ExecDataSave();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ローカルセーブ識別名：インゲーム復帰関連：中断復帰パラメータ
	*/
    //----------------------------------------------------------------------------
    public bool LoadFuncGoesToQuest2RestoreChk()
    {
        //--------------------------------
        // 文字列を取得してパラメータ変換して返す
        //--------------------------------
        string strLoadString = LocalSaveUtilToInstallFolder.LoadBattleSetting(LOCALSAVE_GOES_TO_QUEST2_RESTORE);
        if (strLoadString != null
        && strLoadString.Length > 0
        )
        {
            return true;
        }
        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ローカルセーブ識別名：インゲーム復帰関連：中断復帰パラメータ
	*/
    //----------------------------------------------------------------------------
    public SceneGoesParamToQuest2Restore LoadFuncGoesToQuest2Restore()
    {
        //--------------------------------
        // キャッシュしてるならそれを返す
        //--------------------------------
        if (m_SaveGoesDataToQuest2Restore != null)
        {
            return m_SaveGoesDataToQuest2Restore;
        }
        //--------------------------------
        // 文字列を取得してパラメータ変換して返す
        //--------------------------------
        string strLoadString = LocalSaveUtilToInstallFolder.LoadBattleSetting(LOCALSAVE_GOES_TO_QUEST2_RESTORE);
        if (strLoadString.Length <= 0)
        {
            return null;
        }
        m_SaveGoesDataToQuest2Restore = LitJson.JsonMapper.ToObject<SceneGoesParamToQuest2Restore>(strLoadString);
        return m_SaveGoesDataToQuest2Restore;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：ローカルセーブ識別名：インゲーム復帰関連：中断復帰パラメータ
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncGoesToQuest2Restore(SceneGoesParamToQuest2Restore cGoesParam)
    {
        //--------------------------------
        // パラメータを文字列化してセーブ
        //--------------------------------
        string strSaveString = "";
        if (cGoesParam != null)
        {
            strSaveString = LitJson.JsonMapper.ToJson(cGoesParam);
        }
        LocalSaveUtilToInstallFolder.SaveBattleSetting(LOCALSAVE_GOES_TO_QUEST2_RESTORE, strSaveString);

        m_SaveGoesDataToQuest2Restore = cGoesParam;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ローカルセーブ識別名：インゲーム復帰関連：クエスト開始パラメータ
	*/
    //----------------------------------------------------------------------------
    public bool LoadFuncGoesToQuest2StartChk()
    {
        //--------------------------------
        // 文字列を取得してパラメータ変換して返す
        //--------------------------------
        string strLoadString = LocalSaveUtilToInstallFolder.LoadBattleSetting(LOCALSAVE_GOES_TO_QUEST2_START);

        if (strLoadString != null
        && strLoadString.Length > 0
        )
        {
            return true;
        }
        return false;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ローカルセーブ識別名：インゲーム復帰関連：クエスト開始パラメータ
	*/
    //----------------------------------------------------------------------------
    public SceneGoesParamToQuest2 LoadFuncGoesToQuest2Start()
    {
        //--------------------------------
        // キャッシュしてるならそれを返す
        //--------------------------------
        if (m_SaveGoesDataToQuest2 != null)
        {
            return m_SaveGoesDataToQuest2;
        }

        //--------------------------------
        // 文字列を取得してパラメータ変換して返す
        //--------------------------------
        string strLoadString = LocalSaveUtilToInstallFolder.LoadBattleSetting(LOCALSAVE_GOES_TO_QUEST2_START);
        if (strLoadString.Length <= 0)
        {
            return null;
        }
        m_SaveGoesDataToQuest2 = LitJson.JsonMapper.ToObject<SceneGoesParamToQuest2>(strLoadString);
        return m_SaveGoesDataToQuest2;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：ローカルセーブ識別名：インゲーム復帰関連：クエスト開始パラメータ
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncGoesToQuest2Start(SceneGoesParamToQuest2 cGoesParam)
    {
        //--------------------------------
        // パラメータを文字列化してセーブ
        //--------------------------------
        string strSaveString = "";
        if (cGoesParam != null)
        {
            strSaveString = LitJson.JsonMapper.ToJson(cGoesParam);
        }
        LocalSaveUtilToInstallFolder.SaveBattleSetting(LOCALSAVE_GOES_TO_QUEST2_START, strSaveString);

        m_SaveGoesDataToQuest2 = cGoesParam;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ローカルセーブ識別名：クエストリザルトパラメータバージョンが破棄対象でないかチェック
	*/
    //----------------------------------------------------------------------------
    public bool LoadFuncResultVersionCheck()
    {
        string strLoadString = LocalSaveUtil.GetValueString(LOCALSAVE_RESULT_VERSION, "");
        if (strLoadString != LOCALSAVE_RESULT_VERSION_ID)
        {
            Debug.LogError("Version Result Delete!");
            return false;
        }
        return true;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ローカルセーブ識別名：クエストリザルトパラメータバージョン
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncResultVersion()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("Version Result Save! - " + LOCALSAVE_RESULT_VERSION_ID);
#endif
        LocalSaveUtil.SetValueString(LOCALSAVE_RESULT_VERSION, LOCALSAVE_RESULT_VERSION_ID);
        LocalSaveUtil.ExecDataSave();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ローカルセーブ識別名：メインメニュー復帰関連：クエストリザルト
	*/
    //----------------------------------------------------------------------------
    public SceneGoesParamToMainMenu LoadFuncGoesToMenuResult()
    {
        //--------------------------------
        // キャッシュしてるならそれを返す
        //--------------------------------
        if (m_SaveGoesDataToMainMenuResult != null)
        {
            return m_SaveGoesDataToMainMenuResult;
        }

        //--------------------------------
        // 文字列を取得してパラメータ変換して返す
        //--------------------------------
        string strLoadString = LocalSaveUtilToInstallFolder.LoadBattleSetting(LOCALSAVE_GOES_TO_MENU_RESULT);

        if (strLoadString.Length <= 0)
        {
            return null;
        }
        m_SaveGoesDataToMainMenuResult = LitJson.JsonMapper.ToObject<SceneGoesParamToMainMenu>(strLoadString);
        return m_SaveGoesDataToMainMenuResult;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：ローカルセーブ識別名：メインメニュー復帰関連：クエストリザルト
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncGoesToMenuResult(SceneGoesParamToMainMenu cGoesParam)
    {
        //--------------------------------
        // パラメータを文字列化してセーブ
        //--------------------------------
        string strSaveString = "";
        if (cGoesParam != null)
        {
            strSaveString = LitJson.JsonMapper.ToJson(cGoesParam);
        }
        LocalSaveUtilToInstallFolder.SaveBattleSetting(LOCALSAVE_GOES_TO_MENU_RESULT, strSaveString);

        m_SaveGoesDataToMainMenuResult = cGoesParam;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：ローカルセーブ識別名：メインメニュー復帰関連：クエストリタイア
	*/
    //----------------------------------------------------------------------------
    public SceneGoesParamToMainMenuRetire LoadFuncGoesToMenuRetire()
    {
        //--------------------------------
        // キャッシュしてるならそれを返す
        //--------------------------------
        if (m_SaveGoesDataToMainMenuRetire != null)
        {
            return m_SaveGoesDataToMainMenuRetire;
        }

        //--------------------------------
        // 文字列を取得してパラメータ変換して返す
        //--------------------------------
        string strLoadString = LocalSaveUtilToInstallFolder.LoadBattleSetting(LOCALSAVE_GOES_TO_MENU_RETIRE);

        if (strLoadString.Length <= 0)
        {
            return null;
        }
        m_SaveGoesDataToMainMenuRetire = LitJson.JsonMapper.ToObject<SceneGoesParamToMainMenuRetire>(strLoadString);
        return m_SaveGoesDataToMainMenuRetire;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：ローカルセーブ識別名：メインメニュー復帰関連：クエストリザルト
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncGoesToMenuRetire(SceneGoesParamToMainMenuRetire cGoesParam)
    {
        //--------------------------------
        // パラメータを文字列化してセーブ
        //--------------------------------
        string strSaveString = "";
        if (cGoesParam != null)
        {
            strSaveString = LitJson.JsonMapper.ToJson(cGoesParam);
        }
        LocalSaveUtilToInstallFolder.SaveBattleSetting(LOCALSAVE_GOES_TO_MENU_RETIRE, strSaveString);

        m_SaveGoesDataToMainMenuRetire = cGoesParam;
    }



    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：中断復帰パラメータ
		@note	PlayerPrefs にはロードは必要ないので各機能にパラメータを反映するだけ
	*/
    //----------------------------------------------------------------------------
    public SceneGoesParamToQuest2Restore LoadFuncRestore()
    {
        return LoadFuncGoesToQuest2Restore();
    }



    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：中断復帰パラメータ
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncRestoreQuest2()
    {
#if true
#if UNITY_EDITOR
        Debug.Log("Save Quest2 Restore Data >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
#endif // UNITY_EDITOR

        SceneGoesParamToQuest2Restore cParam = new SceneGoesParamToQuest2Restore();
        if (cParam == null)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log(" LocalSaveError! (Quest2)");
#endif
            return;
        }

        //--------------------------------
        // セーブするパラメータを更新。
        //--------------------------------
        if (SceneModeContinuousBattle.Instance != null
        && SceneModeContinuousBattle.Instance.m_PlayerPartyChara != null)
        {
            //--------------------------------
            // InGame側の進行状態保存
            //--------------------------------
            for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
            {
                if (SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i] == null)
                {
                    continue;
                }

                if (SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_CharaMasterDataParam == null)
                {
                    continue;
                }

                cParam.m_PlayerPartyRestore.m_PartyCharaID[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_CharaMasterDataParam.fix_id;
                cParam.m_PlayerPartyRestore.m_PartyCharaLevel[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_CharaLevel;
                cParam.m_PlayerPartyRestore.m_PartyCharaLBSLv[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_CharaLBSLv;
                cParam.m_PlayerPartyRestore.m_PartyCharaPlusPow[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_CharaPlusPow;
                cParam.m_PlayerPartyRestore.m_PartyCharaPlusHP[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_CharaPlusHP;
                cParam.m_PlayerPartyRestore.m_PartyCharaLimitBreak[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_CharaLimitBreak;
                cParam.m_PlayerPartyRestore.m_PartyCharaLimitOver[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_CharaLimitOver;
                cParam.m_PlayerPartyRestore.m_PartyCharaLinkID[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_LinkParam.m_CharaID;
                cParam.m_PlayerPartyRestore.m_PartyCharaLinkLv[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_LinkParam.m_CharaLv;
                cParam.m_PlayerPartyRestore.m_PartyCharaLinkPlusPow[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_LinkParam.m_CharaPlusPow;
                cParam.m_PlayerPartyRestore.m_PartyCharaLinkPlusHP[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_LinkParam.m_CharaPlusHP;
                cParam.m_PlayerPartyRestore.m_PartyCharaLinkPoint[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_LinkParam.m_CharaLinkPoint;
                cParam.m_PlayerPartyRestore.m_PartyCharaLinkLimitOver[i] = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[i].m_LinkParam.m_CharaLOLevel;
            }

            //--------------------------------
            // 発行クエスト情報保存
            //--------------------------------
            // クエスト情報の保存
            cParam.m_QuestAreaID = SceneModeContinuousBattle.Instance.m_QuestAreaID;
            cParam.m_QuestMissionID = SceneModeContinuousBattle.Instance.m_QuestMissionID;
            cParam.m_NextAreaCleard = SceneModeContinuousBattle.Instance.m_NextAreaCleard;

            // 瞬間火力記録
            //			cParam.m_QuestDamageMax = SceneModeContinuousBattle.Instance.m_QuestDamageMax;

            // クリアまでのターン数
            cParam.m_QuestTotalTurn = SceneModeContinuousBattle.Instance.m_QuestTotalTurn;


            //--------------------------------
            // 取得ユニット保存
            //--------------------------------
            for (int i = 0; i < GlobalDefine.INGAME_UNIT_MAX; i++)
            {
                cParam.m_AcquireUnit[i] = InGameQuestData.Instance.m_AcquireUnit[i];
            }


            //--------------------------------
            // 取得金保存
            //--------------------------------
            for (int i = 0; i < GlobalDefine.INGAME_MONEY_MAX; i++)
            {
                cParam.m_AcquireMoney[i] = InGameQuestData.Instance.m_AcquireMoney[i];
            }


            //--------------------------------
            // チケット
            //--------------------------------
            for (int i = 0; i < GlobalDefine.INGAME_TICKET_MAX; i++)
            {
                cParam.m_AcquireTicket[i] = InGameQuestData.Instance.m_AcquireTicket[i];
            }


            //--------------------------------
            // 取得ユニット数保存
            //--------------------------------
            for (int i = 0; i < (int)MasterDataDefineLabel.RarityType.MAX; i++)
            {
                cParam.m_AcquireUnitRareNum[i] = InGameQuestData.Instance.m_AcquireUnitRareNum[i];
            }


            cParam.m_QuestRandSeed = SceneModeContinuousBattle.Instance.m_QuestRandSeed;
            cParam.m_IsUsedAutoPlay = SceneModeContinuousBattle.Instance.isUsedAutoPlay;
            cParam.m_BattleCount = SceneModeContinuousBattle.Instance.battleCount;
        }

        // プレイヤーパーティ情報を保存
        CharaParty chara_party = null;
        if (SceneModeContinuousBattle.Instance != null)
        {
            chara_party = SceneModeContinuousBattle.Instance.m_PlayerParty;
        }

        if (chara_party != null)
        {
            cParam.m_PlayerPartyRestore.m_IsKobetsuHP = BattleParam.IsKobetsuHP;

            // プレイヤーステータスを保存
            cParam.m_PlayerPartyRestore.m_QuestHP = chara_party.m_HPCurrent;
            cParam.m_PlayerPartyRestore.m_QuestSP = chara_party.m_PartyTotalSP;
            cParam.m_PlayerPartyRestore.m_QuestHPMax = chara_party.m_HPMax;
            cParam.m_PlayerPartyRestore.m_QuestSPMax = chara_party.m_PartyTotalSPMax;

            cParam.m_PlayerPartyRestore.m_PartyAilments = chara_party.m_Ailments;

            for (int i = 0; i < chara_party.getPartyMemberMaxCount(); i++)
            {
                CharaOnce chara_once = chara_party.getPartyMember((GlobalDefine.PartyCharaIndex)i, CharaParty.CharaCondition.EXIST);
                if (chara_once == null)
                {
                    continue;
                }

                cParam.m_PlayerPartyRestore.m_PartyCharaID[i] = chara_once.m_CharaMasterDataParam.fix_id;
                cParam.m_PlayerPartyRestore.m_PartyCharaLevel[i] = chara_once.m_CharaLevel;
                cParam.m_PlayerPartyRestore.m_PartyCharaLBSLv[i] = chara_once.m_CharaLBSLv;
                cParam.m_PlayerPartyRestore.m_PartyCharaPlusPow[i] = chara_once.m_CharaPlusPow;
                cParam.m_PlayerPartyRestore.m_PartyCharaPlusHP[i] = chara_once.m_CharaPlusHP;
                cParam.m_PlayerPartyRestore.m_PartyCharaLimitBreak[i] = chara_once.m_CharaLimitBreak;
                cParam.m_PlayerPartyRestore.m_PartyCharaLimitOver[i] = chara_once.m_CharaLimitOver;
                cParam.m_PlayerPartyRestore.m_PartyCharaLinkID[i] = chara_once.m_LinkParam.m_CharaID;
                cParam.m_PlayerPartyRestore.m_PartyCharaLinkLv[i] = chara_once.m_LinkParam.m_CharaLv;
                cParam.m_PlayerPartyRestore.m_PartyCharaLinkPlusPow[i] = chara_once.m_LinkParam.m_CharaPlusPow;
                cParam.m_PlayerPartyRestore.m_PartyCharaLinkPlusHP[i] = chara_once.m_LinkParam.m_CharaPlusHP;
                cParam.m_PlayerPartyRestore.m_PartyCharaLinkPoint[i] = chara_once.m_LinkParam.m_CharaLinkPoint;
                cParam.m_PlayerPartyRestore.m_PartyCharaLinkLimitOver[i] = chara_once.m_LinkParam.m_CharaLOLevel;
            }

            cParam.m_PlayerPartyRestore.m_Hate = chara_party.m_Hate;
            cParam.m_PlayerPartyRestore.m_Hate_ProvokeTurn = chara_party.m_Hate_ProvokeTurn;
            cParam.m_PlayerPartyRestore.m_Hate_ProvokeOrder = chara_party.m_Hate_ProvokeOrder;
            cParam.m_PlayerPartyRestore.m_HeroSkillTurn = chara_party.m_BattleHero.getSkillTurn();
            cParam.m_PlayerPartyRestore.m_BattleAchive = chara_party.m_BattleAchive;
        }

        {
            {
                //--------------------------------
                //	戦闘リクエスト情報
                //--------------------------------
                cParam.m_BattleRestore.m_BattleUniqueID = BattleParam.m_BattleRequest.m_QuestBuildBattle.unique_id;
                cParam.m_BattleRestore.m_Chain = BattleParam.m_BattleRequest.m_BattleEnemyChain;
                cParam.m_BattleRestore.m_Boss = BattleParam.m_BattleRequest.m_BattleEnemyBoss;
                cParam.m_BattleRestore.m_BattleTurnOffset = BattleParam.m_BattleRequest.m_BattleTurnOffset;


                //--------------------------------
                //	経過ターン数
                //--------------------------------
                cParam.m_BattleRestore.m_BattleTotalTurn = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleTotalTurn;


                //--------------------------------
                //	プレイヤー状態異常更新フラグ
                //--------------------------------
                cParam.m_BattleRestore.m_UpdateStatusAilmentPlayer = BattleSceneManager.Instance.PRIVATE_FIELD.m_UpdatedStatusAilmentPlayer;


#if true // @Change Developer 2015/08/21 v300状態異常の遅延発動対応。
                //---------------------------------------------------------
                // 状態異常遅延発動。
                //---------------------------------------------------------
                for (int i = 0; i < cParam.m_BattleRestore.m_StatusAilmentDelay.Length; ++i)
                {
                    //-----------------------------------------------------
                    // 情報入力。
                    //-----------------------------------------------------
                    if (null != BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleLogicAilment.m_acDelayAilment[i])
                    {
                        //cParam.m_StatusAilmentDelay[i].m_bUse					= true;
                        cParam.m_BattleRestore.m_StatusAilmentDelay[i].m_nSkillParamOwnerNum = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleLogicAilment.m_acDelayAilment[i].m_SkillParamOwnerNum;
                        cParam.m_BattleRestore.m_StatusAilmentDelay[i].m_nStatusAilmentTarget = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleLogicAilment.m_acDelayAilment[i].m_statusAilment_target;

                        for (int j = 0; j < BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleLogicAilment.m_acDelayAilment[i].m_SkillParamTarget.Length; ++j)
                        {
                            cParam.m_BattleRestore.m_StatusAilmentDelay[i].m_acSkillParamTarget[j] = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleLogicAilment.m_acDelayAilment[i].m_SkillParamTarget[j];
                        }
                        for (int j = 0; j < BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleLogicAilment.m_acDelayAilment[i].m_statusAilment.Length; ++j)
                        {
                            cParam.m_BattleRestore.m_StatusAilmentDelay[i].SetAilment(j, BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleLogicAilment.m_acDelayAilment[i].m_statusAilment[j]);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < cParam.m_BattleRestore.m_StatusAilmentDelay[i].m_acSkillParamTarget.Length; ++j)
                        {
                            cParam.m_BattleRestore.m_StatusAilmentDelay[i].m_acSkillParamTarget[j] = new BattleSkillTarget();
                        }
                    }
                }
#endif


                //--------------------------------
                //	敵情報
                //--------------------------------
                cParam.m_BattleRestore.m_BattleEnemy = BattleParam.m_EnemyParam;
                cParam.m_BattleRestore.m_EnemyActionSwitch = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleLogicEnemy.m_abEnemyActionSwitch;

                for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
                {
                    BattleEnemy battle_enemy = BattleParam.m_EnemyParam[i];

                    if (battle_enemy == null)
                    {
                        continue;
                    }

                    //--------------------------------
                    //	行動テーブル管理情報の保存
                    //--------------------------------
                    EnemyActionTableControl tableControl = battle_enemy.getEnemyActionTableControl();
                    if (tableControl != null)
                    {


                        //--------------------------------
                        //	敵行動テーブルランダム行動時の乱数シードを保存
                        //--------------------------------
                        cParam.m_BattleRestore.m_BattleEnemyRandSeed[i] = tableControl.m_RandomSeed;


                        //--------------------------------
                        //	敵行動テーブル進行度を保存
                        //--------------------------------
                        cParam.m_BattleRestore.m_BattleEnemyActStep[i] = tableControl.m_ActionStep;
                    }
                }

                //--------------------------------
                // 敵リアクション：判定スキルIDの保存
                //--------------------------------
                cParam.m_BattleRestore.m_EnemyReactChkSkillID = BattleSceneManager.Instance.PRIVATE_FIELD.m_EnemyReactChkSkillID;
                cParam.m_BattleRestore.m_NextLimitBreakSkillCaster = BattleSceneManager.Instance.PRIVATE_FIELD.m_NextLimitBreakSkillCaster;
                cParam.m_BattleRestore.m_NextLimitBreakSkillFixID = BattleSceneManager.Instance.PRIVATE_FIELD.m_NextLimitBreakSkillFixID;

                //--------------------------------
                // 手札の保存
                //--------------------------------
                cParam.m_BattleRestore.m_BattleHandCardElem = new MasterDataDefineLabel.ElementType[BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCardMaxCount()];
                cParam.m_BattleRestore.m_BattleNextCardElem = new MasterDataDefineLabel.ElementType[cParam.m_BattleRestore.m_BattleHandCardElem.Length];
                for (int idx = 0; idx < cParam.m_BattleRestore.m_BattleHandCardElem.Length; idx++)
                {
                    cParam.m_BattleRestore.m_BattleHandCardElem[idx] = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCardElement(idx);
                    cParam.m_BattleRestore.m_BattleNextCardElem[idx] = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_NextArea.getCardElement(idx);
                }

                //--------------------------------
                // 場の保存
                // @add Developer 2015/10/26 v300 新スキル対応
                //--------------------------------
                int field_count = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldAreaCountMax();
                int card_count = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(0).getCardMaxCount();
                cParam.m_BattleRestore.m_BattleFieldPanelElem = new MasterDataDefineLabel.ElementType[field_count * card_count];
                for (int field_idx = 0; field_idx < field_count; field_idx++)
                {
                    for (int card_idx = 0; card_idx < BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(field_idx).getCardCount(); card_idx++)
                    {
                        MasterDataDefineLabel.ElementType element_type = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_FieldAreas.getFieldArea(field_idx).getCardElement(card_idx);
                        cParam.m_BattleRestore.m_BattleFieldPanelElem[field_idx * card_count + card_idx] = element_type;
                    }
                }

                //--------------------------------
                //	ブーストパネルの保存
                //--------------------------------
                cParam.m_BattleRestore.m_BattleBoost = BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField;

                //--------------------------------
                //	フラグの保存
                //--------------------------------
                // 初回ターンの処理を行ったかどうかのフラグ
                cParam.m_BattleRestore.m_ProcStep = (int)BattleSceneManager.Instance.PRIVATE_FIELD.m_ProcStep;
                cParam.m_BattleRestore.m_ProcStep_player = (int)BattleSceneManager.Instance.PRIVATE_FIELD.m_ProcStep_player;

                //--------------------------------
                //	実績集計情報の保存
                //--------------------------------
                cParam.m_BattleRestore.m_AchievementTotaling = BattleParam.m_AchievementTotalingInBattle;
            }
            if (SceneModeContinuousBattle.Instance != null)
            {
                for (int num = 0; num < (int)GlobalDefine.PartyCharaIndex.MAX; ++num)
                {
                    cParam.m_Balloon_active[num] = SceneModeContinuousBattle.Instance.m_Balloon_active[num];
                }

                cParam.m_BossChainCount = SceneModeContinuousBattle.Instance.m_BossChainCount;
            }
        }
#endif

        //--------------------------------
        // セーブ実行
        //--------------------------------
        SaveFuncGoesToQuest2Restore(cParam);

#if UNITY_EDITOR
        Debug.Log("Save Quest2 Restore Data  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
#endif // UNITY_EDITOR
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：機種移行パスワード
	*/
    //----------------------------------------------------------------------------
    public bool LoadFuncTransferPasswordChk()
    {
        //--------------------------------
        // 文字列を取得してパラメータ変換して返す
        //--------------------------------
        string strLoadString = LocalSaveUtil.GetValueString(LOCALSAVE_TRANSFER_PASSWORD, "");
        if (strLoadString != null
        && strLoadString.Length > 0
        )
        {
            LocalSaveTransferPassword saveTransferPassword = LitJson.JsonMapper.ToObject<LocalSaveTransferPassword>(strLoadString);
            int today = TimeManager.Instance.m_TimeNow.ToString("yyyyMMdd").ToInt(0);
            if (today > saveTransferPassword.m_TimeLimit)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：機種移行パスワード
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncTransferPassword(LocalSaveTransferPassword cTransferPassword)
    {
        //--------------------------------
        // パラメータを文字列化してセーブ
        //--------------------------------
        string strSaveString = "";
        if (cTransferPassword != null)
        {
            strSaveString = LitJson.JsonMapper.ToJson(cTransferPassword);
        }
        LocalSaveUtil.SetValueString(LOCALSAVE_TRANSFER_PASSWORD, strSaveString);
        LocalSaveUtil.ExecDataSave();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：機種移行パスワード
	*/
    //----------------------------------------------------------------------------
    public LocalSaveTransferPassword LoadFuncTransferPassword()
    {
        //--------------------------------
        // 文字列を取得してパラメータ変換して返す
        //--------------------------------
        string strLoadString = LocalSaveUtil.GetValueString(LOCALSAVE_TRANSFER_PASSWORD, "");
        if (strLoadString.Length <= 0)
        {
            return null;
        }
        LocalSaveTransferPassword saveTransferPassword = LitJson.JsonMapper.ToObject<LocalSaveTransferPassword>(strLoadString);
        return saveTransferPassword;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：動画初回再生
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncMovieFirst(bool bSwitch)
    {
        LocalSaveUtil.SetValueBool(LOCALSAVE_MOVIE_FIRST, bSwitch);
        LocalSaveUtil.ExecDataSave();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：動画初回再生
	*/
    //----------------------------------------------------------------------------
    public bool LoadFuncMovieFirst()
    {
        return LocalSaveUtil.GetValueBool(LOCALSAVE_MOVIE_FIRST, false);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：総付プレゼント情報
	*/
    //----------------------------------------------------------------------------
    public void SaveFuncGrossingPresent(string strGrossingType, string strGrossingId, string strGrossingNum)
    {
        LocalSaveUtil.SetValueString(LOCALSAVE_GROSSING_TYPE, strGrossingType);
        LocalSaveUtil.SetValueString(LOCALSAVE_GROSSING_ID, strGrossingId);
        LocalSaveUtil.SetValueString(LOCALSAVE_GROSSING_NUM, strGrossingNum);
        LocalSaveUtil.ExecDataSave();
    }

    public void CheckInfoTime(DateTime _now)
    {
        string strTime;
        strTime = LocalSaveUtil.GetValueString(LOCALSAVE_INFO_DATE, string.Empty);
        if (strTime != string.Empty)
        {
            DateTime infoTime = DateTime.Parse(strTime);
            if (infoTime.Year != _now.Year ||
                infoTime.Month != _now.Month ||
                infoTime.Day != _now.Day)
            {
                updateInfoTime(_now);
            }
        }
        else
        {
            updateInfoTime(_now);
        }
    }

    private void updateInfoTime(DateTime _updateTime)
    {
        string upTime = _updateTime.ToString();
        LocalSaveUtil.SetValueString(LOCALSAVE_INFO_DATE, upTime);
        TemplateList<uint> resett = new TemplateList<uint>();
        SaveFuncUintList(LOCALSAVE_INFO_NORMAL_LIST, resett, false);
        SaveFuncUintList(LOCALSAVE_INFO_EMERGENCY_LIST, resett, false);
        LocalSaveUtil.ExecDataSave();
    }

    public TemplateList<uint> LoadFuncUintList(string strKeyName)
    {
        TemplateList<uint> ret = null;
        string data = LocalSaveUtil.GetValueString(strKeyName, string.Empty);
        if (data != string.Empty)
        {
            ret = LitJson.JsonMapper.ToObject<TemplateList<uint>>(data);
        }
        else
        {
            ret = new TemplateList<uint>();
        }
        return ret;
    }

    public void SaveFuncUintList(string strKeyName, TemplateList<uint> list, bool bSave = true)
    {
        string data = LitJson.JsonMapper.ToJson(list);
        LocalSaveUtil.SetValueString(strKeyName, data);
        if (bSave) LocalSaveUtil.ExecDataSave();
    }
}

