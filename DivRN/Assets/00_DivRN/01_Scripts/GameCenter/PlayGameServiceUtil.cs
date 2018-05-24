#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define GAMECENTER_WINDOWS_ANDROID
#elif UNITY_ANDROID
#define GAMECENTER_ANDROID
#elif UNITY_IPHONE
//	#define GAMECENTER_IPHONE
#else
#define GAMECENTER_WINDOWS_ANDROID
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if GAMECENTER_ANDROID || GAMECENTER_IPHONE
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif
using UnityEngine.SocialPlatforms;

//----------------------------------------------------------------------------
/*!
	@brief		リーダーボードID
*/
//----------------------------------------------------------------------------
public enum ELEADERBORAD
{
    eCLEAR_AREA_COUNT,                  //!< クリアしたエリア数
    eTOTAL_DAMAGE,                      //!< 瞬間火力
    eINFINITY_DUNGEON_LV0,              //!< 無限ダンジョン到達フロア-初級
    eINFINITY_DUNGEON_LV1,              //!< 無限ダンジョン到達フロア-中級
    eINFINITY_DUNGEON_CLEAR_TURN_LV0,   //!< 無限ダンジョンクリア記録-初級
    eINFINITY_DUNGEON_CLEAR_TURN_LV1,	//!< 無限ダンジョンクリア記録-中級
}

//----------------------------------------------------------------------------
/*!
	@brief		実績ID定義
*/
//----------------------------------------------------------------------------
public enum EACHIEVEMENT
{
    eHANDS_05,                          //!< 05HANDS
    eHANDS_10,                          //!< 10HANDS
    eHANDS_15,                          //!< 15HANDS
    eHANDS_20,                          //!< 20HANDS
    eHANDS_25,                          //!< 25HANDS
    eCOLLECT_010,                       //!< 図鑑010
    eCOLLECT_050,                       //!< 図鑑050
    eCOLLECT_100,                       //!< 図鑑100
    eCOLLECT_150,                       //!< 図鑑150
    eCOLLECT_200,                       //!< 図鑑200
    eCOLLECT_250,                       //!< 図鑑250
    eCOLLECT_300,                       //!< 図鑑300
    eCOLLECT_350,                       //!< 図鑑350
    eINFINITY_DUNGEON_LV0,              //!< 無限ダンジョンクリア-初級
    eINFINITY_DUNGEON_LV1,              //!< 無限ダンジョンクリア-中級
    eINFINITY_DUNGEON_LV2,              //!< 無限ダンジョンクリア-上級
    eINFINITY_DUNGEON_LV3,              //!< 無限ダンジョンクリア-超級
    eINFINITY_DUNGEON_LV4,				//!< 無限ダンジョンクリア-神級
}

//----------------------------------------------------------------------------
/*!
	@brief		SavedDataステータス.
*/
//----------------------------------------------------------------------------
public enum ESAVED_DATA_STATUS
{
    NONE,                       // 何もしていない.
    DATA_DURING_SAVE,           // セーブ処理中.
    DATA_LOADING,               // ロード中.
                                // 終了時.
    DATA_SAVE_FAILED,           // セーブ失敗.
    DATA_LOAD_FAILED,           // ロード失敗.
    DATA_SAVE_SUCCESS,          // セーブ成功.
    DATA_LOAD_SUCCESS,          // ロード成功.
    TIME_OUT_ERROR,				// 接続タイムアウト.
}

//----------------------------------------------------------------------------
/*!
	@class		PlayGameServiceUtil
	@brief		
*/
//----------------------------------------------------------------------------
public class PlayGameServiceUtil
{
    // ロード終了判定.
    private static bool m_LoginFinished = false;
#if GAMECENTER_ANDROID || GAMECENTER_IPHONE // @Change Developer 2015/11/04 warning除去。
	// セーブデータ一時添付用.
	private static byte[]				m_SaveDataTemp = null;
#endif // GAMECENTER_ANDROID || GAMECENTER_IPHONE
    // ロードデータ一時添付用.
    private static byte[] m_LoadDataTemp = null;
    // セーブ処理ステータス.
    private static ESAVED_DATA_STATUS m_SavedDataStatus = ESAVED_DATA_STATUS.NONE;

    // 初回作業呼び出し確認.
    private static bool m_CallInit = false;
    // オートサインイン呼び出し確認.
    private static bool m_CallAutoSignIn = false;

    //------------------------------------------------------------------------
    /*!
		@brief		初回作業		<static>
	*/
    //------------------------------------------------------------------------
    public static void InitPlayGameService()
    {
#if GAMECENTER_ANDROID || GAMECENTER_IPHONE
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate();

		m_CallInit = true;
#endif
    }

    //------------------------------------------------------------------------
    /*!
		@brief		オートログイン		<static>
	*/
    //------------------------------------------------------------------------
    public static void AutoSignIn()
    {
        // オートログインが呼び出されていたらスル―.
        if (m_CallAutoSignIn == true)
        {
            return;
        }
        // 初回処理が呼び出されていないなら戻る.
        if (m_CallInit == false)
        {
            return;
        }
#if GAMECENTER_ANDROID || GAMECENTER_IPHONE
		((PlayGamesPlatform)Social.Active).Authenticate( ( bool success ) =>
		{
			if( success )
			{
				// サインイン成功.
				authenticationSucceededEvent();
				Debug.LogErrorFormat("GooglePlayGame Singin Success");
			}
			else
			{
				// サインイン失敗.
				authenticationFailedEvent();
				Debug.LogErrorFormat("GooglePlayGame Singin Error");
			}
		}, true );
#endif
        m_CallAutoSignIn = true;
    }

    //------------------------------------------------------------------------
    /*!
		@brief		サインイン		<static>
	*/
    //------------------------------------------------------------------------
    public static void SignIn()
    {
#if GAMECENTER_ANDROID || GAMECENTER_IPHONE
		m_LoginFinished = false;

		//--------------------------------
		// サインイン通信中はLoading表示
		//--------------------------------
		LoadingManager.Instance.RequestLoadingStart( LOADING_TYPE.PLAYGAME_SERVICES );

		Social.localUser.Authenticate( ( bool success ) =>
		{
			if( success )
			{
				// サインイン成功.
				authenticationSucceededEvent();
				Debug.LogErrorFormat("GooglePlayGame Singin Success");
			}
			else
			{
				// サインイン失敗.
				authenticationFailedEvent();
				Debug.LogErrorFormat("GooglePlayGame Singin Error");
			}
		} );
#endif
    }

    //------------------------------------------------------------------------
    /*!
		@brief		サインイン終了判定		<static>
	*/
    //------------------------------------------------------------------------
    public static bool SignInFinished()
    {
        return m_LoginFinished;
    }

    //------------------------------------------------------------------------
    /*!
		@brief		サインアウト		<static>
	*/
    //------------------------------------------------------------------------
    public static void SignOut()
    {
#if GAMECENTER_ANDROID || GAMECENTER_IPHONE
		if( Social.Active != null )
		{
			PlayGamesPlatform.Instance.SignOut();
		}
#endif
    }

    //------------------------------------------------------------------------
    /*!
		@brief		サインイン中か確認
		@retval		bool		[サイン中/サインアウト中]
	*/
    //------------------------------------------------------------------------
    public static bool isSignedIn()
    {
        bool result = true;

#if GAMECENTER_ANDROID || GAMECENTER_IPHONE
		result = Social.localUser.authenticated;
#endif
        return result;
    }

    //------------------------------------------------------------------------
    /*!
		@brief		実績閲覧
	*/
    //------------------------------------------------------------------------
    public static void ShowAchievements()
    {
#if GAMECENTER_ANDROID
		Social.ShowAchievementsUI();
#endif
    }

    //------------------------------------------------------------------------
    /*!
		@brief		リーダーボード閲覧
	*/
    //------------------------------------------------------------------------
    public static void ShowAllLeaderBoard()
    {
#if GAMECENTER_ANDROID
		Social.ShowLeaderboardUI();
#endif
    }

    //------------------------------------------------------------------------
    /*!
		@brief		実績解除				<static>
		@param[in]	EACHIEVEMENT		(eID)		実績ID
	*/
    //------------------------------------------------------------------------
    public static void UnlockAchievement(EACHIEVEMENT eID)
    {
#if GAMECENTER_ANDROID
		string key = GetAchievementKey( eID );
		((PlayGamesPlatform)Social.Active).IncrementAchievement( key, 10000, (bool success) =>
		{
			if( success )
			{
				// 登録成功.
#if BUILD_TYPE_DEBUG
				Debug.Log( "UnlockAchievement["+ key +"] : Success" );
#endif
			}
			else
			{
				// 登録失敗.
#if BUILD_TYPE_DEBUG
				Debug.Log( "UnlockAchievement["+ key +"] : Failed" );
#endif
			}
		} );
#endif
    }

    //------------------------------------------------------------------------
    /*!
		@brief		ランキングボードへのスコア登録		<static>
		@param[in]	ELEADERBORAD				(eID)		リーダーボードID
	*/
    //------------------------------------------------------------------------
    public static void SubmitScore(ELEADERBORAD eID, System.Int64 score)
    {
#if GAMECENTER_ANDROID
		string key = GetLeaderboardKey( eID );
		Social.ReportScore( score, key, (bool success) =>
		{
			if( success )
			{
				// 登録成功.
#if BUILD_TYPE_DEBUG
				Debug.Log( "SubmitScore["+ key +"] : Success" );
#endif
			}
			else
			{
				// 登録失敗.
#if BUILD_TYPE_DEBUG
				Debug.Log( "SubmitScore["+ key +"] : Failed" );
#endif
			}
		} );
#endif
    }

    //------------------------------------------------------------------------
    /*!
		@brief		プレイヤーID取得				<static>
		@return		string						[プレイヤーID]
		@note		取得失敗時はnullが帰る.
	*/
    //------------------------------------------------------------------------
    public static string GetPlayerID()
    {
        string retVal = null;
#if GAMECENTER_ANDROID || GAMECENTER_IPHONE
		if( isSignedIn() )
		{
			// サインイン済みの場合.
			retVal = Social.localUser.id;
		}
#endif
        return retVal;
    }

    //------------------------------------------------------------------------
    /*!	
		@brief		実績解除キー取得
		@param[in]	EACHIEVEMENT		(eID)		実績ID
		@return		string				[実績キー]
	*/
    //------------------------------------------------------------------------
    private static string GetAchievementKey(EACHIEVEMENT eID)
    {
        int id = (int)eID;

        string[] key = {
#if PROV_JP_EXAMPLE_DGRN
			"empty",		//hands05
			"empty",		//hands10
			"empty",		//hands15
			"empty",		//hands20
			"empty",		//hands25
			"empty",		//図鑑10
			"empty",		//図鑑50
			"empty",		//図鑑100
			"empty",		//図鑑150
			"empty",		//図鑑200
			"empty",		//図鑑250
			"empty",		//図鑑300
			"empty",		//図鑑350
			"empty",		//無限ダンジョン初級
			"empty"		//無限ダンジョン中級
#elif PROV_JP_EXAMPLE_DGRN_DEV
			"empty",		//hands05
			"empty",		//hands10
			"empty",		//hands15
			"empty",		//hands20
			"empty",		//hands25
			"empty",		//図鑑10
			"empty",		//図鑑50
			"empty",		//図鑑100
			"empty",		//図鑑150
			"empty",		//図鑑200
			"empty",		//図鑑250
			"empty",		//図鑑300
			"empty",		//図鑑350
			"empty",		//無限ダンジョン初級
			"empty"		//無限ダンジョン中級
#elif PROV_JP_EXAMPLE_DGRN_STABLE
			"empty",		//hands05
			"empty",		//hands10
			"empty",		//hands15
			"empty",		//hands20
			"empty",		//hands25
			"empty",		//図鑑10
			"empty",		//図鑑50
			"empty",		//図鑑100
			"empty",		//図鑑150
			"empty",		//図鑑200
			"empty",		//図鑑250
			"empty",		//図鑑300
			"empty",		//図鑑350
			"empty",		//無限ダンジョン初級
			"empty"		//無限ダンジョン中級
#else //jp.example.dg
			"empty",		//hands05
			"empty",		//hands10
			"empty",		//hands15
			"empty",		//hands20
			"empty",		//hands25
			"empty",		//図鑑10
			"empty",		//図鑑50
			"empty",		//図鑑100
			"empty",		//図鑑150
			"empty",		//図鑑200
			"empty",		//図鑑250
			"empty",		//図鑑300
			//"empty",		//図鑑350 本番にはない
			"empty",		//無限ダンジョン初級
			"empty"		//無限ダンジョン中級
#endif
		};

        if (id < 0 && id >= key.Length)
        {
            return "";
        }

        return key[id];
    }

    //------------------------------------------------------------------------
    /*!
		@brief		リーダーボードのキー取得
		@param[in]	ELEADERBOARD		(eID)		リーダーボードID
		@return		string				[リーダーボードキー]
	*/
    //------------------------------------------------------------------------
    private static string GetLeaderboardKey(ELEADERBORAD eID)
    {
        int id = (int)eID;

        string[] key = {
#if PROV_JP_EXAMPLE_DGRN
			"empty",		//クリアしたエリア数
			"empty",		//瞬間火力
			"empty",		//無限ダンジョン階数-初級
			"empty",		//無限ダンジョン階数-中級
			"empty",		//無限ダンジョンクリアターン-初級
			"empty",		//無限ダンジョンクリアターン-中級
#elif PROV_JP_EXAMPLE_DGRN_DEV
			"empty",	//クリアしたエリア数
			"empty",	//瞬間火力
			"empty",	//無限ダンジョン階数-初級
			"empty",	//無限ダンジョン階数-中級
			"empty",	//無限ダンジョンクリアターン-初級
			"empty",	//無限ダンジョンクリアターン-中級
#elif PROV_JP_EXAMPLE_DGRN_STABLE
			"empty",	//クリアしたエリア数
			"empty",	//瞬間火力
			"empty",	//無限ダンジョン階数-初級
			"empty",	//無限ダンジョン階数-中級
			"empty",	//無限ダンジョンクリアターン-初級
			"empty",	//無限ダンジョンクリアターン-中級
#else //jp.example.dg
			"empty",	//クリアしたエリア数
			"empty",	//瞬間火力
			"empty",	//無限ダンジョン階数-初級
			"empty",	//無限ダンジョン階数-中級
			"empty",	//無限ダンジョンクリアターン-初級
			"empty",	//無限ダンジョンクリアターン-中級
#endif
		};

        if (id < 0 && id >= key.Length)
        {
            return "";
        }

        return key[id];
    }

    //------------------------------------------------------------------------
    /*!
		@brief		サインイン終了;成功
	*/
    //------------------------------------------------------------------------
    public static void authenticationSucceededEvent()
    {
        // 成功失敗にかかわらず終了する.
        m_LoginFinished = true;

#if BUILD_TYPE_DEBUG
        Debug.Log("authenticationSucceededEvent");
#endif
        //--------------------------------
        // サインイン通信中のLoading表示を閉じる
        //--------------------------------
        LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.PLAYGAME_SERVICES);
    }

    //------------------------------------------------------------------------
    /*!
		@brief		サインイン終了;失敗
	*/
    //------------------------------------------------------------------------
    public static void authenticationFailedEvent()
    {
        // 成功失敗にかかわらず終了する.
        m_LoginFinished = true;

#if BUILD_TYPE_DEBUG
        Debug.Log("authenticationFailedEvent");
#endif

        //--------------------------------
        // サインイン通信中のLoading表示を閉じる
        //--------------------------------
        LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.PLAYGAME_SERVICES);
    }

    //------------------------------------------------------------------------
    /*!
		@brief		セーブ処理ステータス	<static>
	*/
    //------------------------------------------------------------------------
    public static ESAVED_DATA_STATUS IsSavedDataStatus()
    {
        return m_SavedDataStatus;
    }

    //------------------------------------------------------------------------
    /*!
		@brief		最新のロードデータ取得	<static>
	*/
    //------------------------------------------------------------------------
    public static byte[] GetLastLoadData()
    {
        return m_LoadDataTemp;
    }

    //------------------------------------------------------------------------
    /*!
		@brief		データセーブ.
		@param[in]	string				(strSaveName)		セーブするファイル名.
		@param[in]	byte[]				(aSaveData)			セーブするバイトデータ.
		@return		セーブ処理を開始したか判定.
	*/
    //------------------------------------------------------------------------
    public static bool SavedGameDataSave(string strSaveName, byte[] aSaveData)
    {
#if GAMECENTER_ANDROID || GAMECENTER_IPHONE
		// 受け付けない状態はfalseで返す.
		// すでにロードorセーブ中 もしくはデータが不正な時.
		if( m_SavedDataStatus == ESAVED_DATA_STATUS.DATA_DURING_SAVE || m_SavedDataStatus == ESAVED_DATA_STATUS.DATA_LOADING ) return false;
		if( strSaveName == null || aSaveData == null ) return false;
		// 保存データはまだ使用しないので一時的に保持.
		m_SaveDataTemp = aSaveData;
		// ステータスをセーブ処理中に.
		m_SavedDataStatus = ESAVED_DATA_STATUS.DATA_DURING_SAVE;
		// SavedDataデータ取得開始.
		((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
			strSaveName,									// 参照するセーブファイル名.
			DataSource.ReadNetworkOnly,						// ネットワーク上のデータのみ取得する.
			ConflictResolutionStrategy.UseUnmerged,			// コンフリクトじのマージをしない.
			SavedGameSaveOpened								// 処理終了時に呼び出すコールバック.
		);

		//--------------------------------
		// SavedGames通信中はLoading表示
		//--------------------------------
		LoadingManager.Instance.RequestLoadingStart( LOADING_TYPE.PLAYGAME_SERVICES );

		return true;
#else
        return false;
#endif
    }
#if GAMECENTER_ANDROID || GAMECENTER_IPHONE
	//------------------------------------------------------------------------
	/*!
		@brief		セーブデータスロット取得コールバック(セーブ用).
		@param[in]	SavedGameRequestStatus	(status)		呼び出された時のステータス.
		@param[in]	ISavedGameMetadata		(game)			セーブデータのメタデータ.
	*/
	//------------------------------------------------------------------------
	public static void SavedGameSaveOpened( SavedGameRequestStatus status, ISavedGameMetadata game )
	{
		if( status == SavedGameRequestStatus.Success )
		{
			// 正常取得時.
			// 特にメタデータを更新はしないでデータを保存.
			SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
			SavedGameMetadataUpdate updatedMetadata = builder.Build();
			((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate( 
				game,										// メタデータ.
				updatedMetadata,							// メタデータ更新内容.
				m_SaveDataTemp,								// 保存するデータ.
				SavedGameWritten							// 処理終了時に呼び出すコールバック.
			);
		}
		else if( status == SavedGameRequestStatus.TimeoutError )
		{
			// 接続タイムアウト.
			// 保存データは消しておく.
			m_SaveDataTemp = null;
			// ステータスをタイムアウト.
			m_SavedDataStatus = ESAVED_DATA_STATUS.TIME_OUT_ERROR;

			//--------------------------------
			// SavedGames通信中のLoading表示を閉じる
			//--------------------------------
			LoadingManager.Instance.RequestLoadingFinish( LOADING_TYPE.PLAYGAME_SERVICES );
		}
		else
		{
			// データ取得失敗時.
			DebugLogger.StatAdd( "Failed SaveOpened : " + status );
			// 保存データは消しておく.
			m_SaveDataTemp = null;
			// ステータスをセーブ失敗に.
			m_SavedDataStatus = ESAVED_DATA_STATUS.DATA_SAVE_FAILED;

			//--------------------------------
			// SavedGames通信中のLoading表示を閉じる
			//--------------------------------
			LoadingManager.Instance.RequestLoadingFinish( LOADING_TYPE.PLAYGAME_SERVICES );
		}
	}

	//------------------------------------------------------------------------
	/*!
		@brief		セーブデータ保存完了コールバック.
		@param[in]	SavedGameRequestStatus	(status)		呼び出された時のステータス.
		@param[in]	ISavedGameMetadata		(game)			セーブデータのメタデータ.
	*/
	//------------------------------------------------------------------------
	public static void SavedGameWritten( SavedGameRequestStatus status, ISavedGameMetadata game )
	{
		if( status == SavedGameRequestStatus.Success )
		{
			// 保存成功.
			// 保存データは消しておく.
			m_SaveDataTemp = null;
			// ステータスをセーブ成功に.
			m_SavedDataStatus = ESAVED_DATA_STATUS.DATA_SAVE_SUCCESS;
		}
		else if( status == SavedGameRequestStatus.TimeoutError )
		{
			// 接続タイムアウト.
			// 保存データは消しておく.
			m_SaveDataTemp = null;
			// ステータスをタイムアウト.
			m_SavedDataStatus = ESAVED_DATA_STATUS.TIME_OUT_ERROR;
		}
		else
		{
			// データ取得失敗時.
			// 保存データは消しておく.
			m_SaveDataTemp = null;
			// ステータスをセーブ失敗に.
			m_SavedDataStatus = ESAVED_DATA_STATUS.DATA_SAVE_FAILED;
		}
	
		//--------------------------------
		// SavedGames通信中のLoading表示を閉じる
		//--------------------------------
		LoadingManager.Instance.RequestLoadingFinish( LOADING_TYPE.PLAYGAME_SERVICES );
	}
#endif

    //------------------------------------------------------------------------
    /*!
		@brief		データロード
		@param[in]	string				(strLoadName)		ロードするファイル名.
		@return		セーブ処理を開始したか判定.
	*/
    //------------------------------------------------------------------------
    public static bool SavedGameDataLoadRequest(string strLoadName)
    {
#if GAMECENTER_ANDROID || GAMECENTER_IPHONE
		// 受け付けない状態はfalseで返す.
		// すでにロードorセーブ中 もしくはデータが不正な時.
		if( m_SavedDataStatus == ESAVED_DATA_STATUS.DATA_DURING_SAVE || m_SavedDataStatus == ESAVED_DATA_STATUS.DATA_LOADING ) return false;
		if( strLoadName == null ) return false;
		// ロードデータをnullに.
		m_LoadDataTemp = null;
		// ステータスをセーブ処理中に.
		m_SavedDataStatus = ESAVED_DATA_STATUS.DATA_LOADING;
		// SavedDataデータ取得開始.
		((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
			strLoadName,									// 参照するセーブファイル名.
			DataSource.ReadNetworkOnly,						// ネットワーク上のデータのみ取得する.
			ConflictResolutionStrategy.UseUnmerged,			// コンフリクトじのマージをしない.
			SavedGameLoadOpened								// 処理終了時に呼び出すコールバック.
		);

		//--------------------------------
		// SavedGames通信中はLoading表示
		//--------------------------------
		LoadingManager.Instance.RequestLoadingStart( LOADING_TYPE.PLAYGAME_SERVICES );

		return true;
#else
        return false;
#endif
    }

#if GAMECENTER_ANDROID || GAMECENTER_IPHONE
	//------------------------------------------------------------------------
	/*!
		@brief		セーブデータスロット取得コールバック(ロード用).
		@param[in]	SavedGameRequestStatus	(status)		呼び出された時のステータス.
		@param[in]	ISavedGameMetadata		(game)			セーブデータのメタデータ.
	*/
	//------------------------------------------------------------------------
	public static void SavedGameLoadOpened( SavedGameRequestStatus status, ISavedGameMetadata game )
	{
		if( status == SavedGameRequestStatus.Success )
		{
			// 正常取得時.
			// ロード処理開始.
			((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData( game, SaveGameLoaded );
		}
		else if( status == SavedGameRequestStatus.TimeoutError )
		{
			// 接続タイムアウト.
			// ロードデータをnullに.
			m_LoadDataTemp = null;
			// ステータスをタイムアウト.
			m_SavedDataStatus = ESAVED_DATA_STATUS.TIME_OUT_ERROR;

			//--------------------------------
			// SavedGames通信中のLoading表示を閉じる
			//--------------------------------
			LoadingManager.Instance.RequestLoadingFinish( LOADING_TYPE.PLAYGAME_SERVICES );
		}
		else
		{
			// データ取得失敗時.
			// ロードデータをnullに.
			m_LoadDataTemp = null;
			// ステータスをロード失敗に.
			m_SavedDataStatus = ESAVED_DATA_STATUS.DATA_LOAD_FAILED;

			//--------------------------------
			// SavedGames通信中のLoading表示を閉じる
			//--------------------------------
			LoadingManager.Instance.RequestLoadingFinish( LOADING_TYPE.PLAYGAME_SERVICES );
		}
	}

	//------------------------------------------------------------------------
	/*!
		@brief		セーブデータ読み込み完了コールバック.
		@param[in]	SavedGameRequestStatus	(status)		呼び出された時のステータス.
		@param[in]	ISavedGameMetadata		(game)			セーブデータのメタデータ.
	*/
	//------------------------------------------------------------------------
	public static void SaveGameLoaded( SavedGameRequestStatus status, byte[] data )
	{
		if( status == SavedGameRequestStatus.Success )
		{
			// 正常取得時.
			// ロードデータをコピー.
			m_LoadDataTemp = new byte[data.Length];
			data.CopyTo( m_LoadDataTemp, 0 );
			// ステータスをロード成功に.
			m_SavedDataStatus = ESAVED_DATA_STATUS.DATA_LOAD_SUCCESS;
		}
		else if( status == SavedGameRequestStatus.TimeoutError )
		{
			// 接続タイムアウト.
			// ロードデータをnullに.
			m_LoadDataTemp = null;
			// ステータスをタイムアウト.
			m_SavedDataStatus = ESAVED_DATA_STATUS.TIME_OUT_ERROR;
		}
		else
		{
			// データ取得失敗時.
			// ロードデータをnullに.
			m_LoadDataTemp = null;
			// ステータスをロード失敗に.
			m_SavedDataStatus = ESAVED_DATA_STATUS.DATA_LOAD_FAILED;
		}

		//--------------------------------
		// SavedGames通信中のLoading表示を閉じる
		//--------------------------------
		LoadingManager.Instance.RequestLoadingFinish( LOADING_TYPE.PLAYGAME_SERVICES );
	}
#endif
} // class PlayGameServiceUtil
