/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	ServerDataUtilSend.cs
	@brief	サーバー通信関連ユーティリティ：送信処理特化
	@author Developer
	@date 	2013/02/07

	送信ユーティリティと受信ユーティリティが同じソース内にあると、物量が膨大になるので分割してるだけ。

	外部クラスはこのユーティリティ関数を呼び出すことでパケット送信処理を行う
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
using System.Linq;
using FFmpeg.AutoGen;
using LitJson;
using StrOpe = StringOperationUtil.OptimizedStringOperation;

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
	@brief	サーバー通信関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class ServerDataUtilSend
{
    public const string DUMMY_UUID = "abaopiujagknvsd";     //!< ダミーUUID

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
#if BUILD_TYPE_DEBUG
    // デバッグ用：AID表示フラグ
    public static bool m_AidOutput = false;
#endif

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット状況チェック：単体終了
	*/
    //----------------------------------------------------------------------------
    static public bool CheckPacketFinish(uint unPacketUniqueID)
    {
        if (ServerDataManager.Instance == null)
        {
            Debug.LogError("Manager Instance None!");
            return false;
        }
        return ServerDataManager.Instance.ChkCommunicateFinish(unPacketUniqueID);
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット状況チェック：完遂
	*/
    //----------------------------------------------------------------------------
    static public bool CheckPacketFinishAll()
    {
        if (ServerDataManager.Instance == null)
        {
            Debug.LogError("Manager Instance None!");
            return false;
        }
        return ServerDataManager.Instance.ChkCommunicateFinishAll();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット状況チェック：リザルト取得
	*/
    //----------------------------------------------------------------------------
    static public PacketDataResult GetPacketResult(uint unPacketUniqueID)
    {
        if (ServerDataManager.Instance == null)
        {
            Debug.LogError("Manager Instance None!");
            return null;
        }
        return ServerDataManager.Instance.GetPacketResult(unPacketUniqueID);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット状況チェック：リザルト削除
	*/
    //----------------------------------------------------------------------------
    static public bool DelPacketResult(uint unPacketUniqueID)
    {
        if (ServerDataManager.Instance == null)
        {
            Debug.LogError("Manager Instance None!");
            return false;
        }
        return ServerDataManager.Instance.DelPacketResult(unPacketUniqueID);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セキュリティキー作成
		@note	クエスト関連のAPIを捏造して送受信すると好きなキャラを取得できるらしい。対応としてテキトーな独自計算で求めた数値を添付し、サーバー上で求めた数値と一致するかのチェックを挟む
	*/
    //----------------------------------------------------------------------------
    static public uint CreateSecurityKey_QuestClear(
        uint unQuestID
    , uint unGetExp
    , uint unGetMoney
    , PacketStructUnitGet[] asGetUnit
    )
    {
        return 0;
    }


    //============================================================================
    //============================================================================
    //============================================================================
    //============================================================================
    //============================================================================

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユーザー管理：ユーザー新規生成
	*/
    /*
	 * リザルト：RecvCreateUser
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_UserCreate(ref string strGetUUID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendCreateUser cSendAPI = new SendCreateUser();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.terminal = CreateStructTerminal();
        cSendAPI.uuid = Guid.NewGuid().ToString();

        //--------------------------------
        // 成功した場合にローカル保存するために保持しておく
        //--------------------------------
        strGetUUID = cSendAPI.uuid;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_USER_CREATE,
                                JsonMapper.ToJson(cSendAPI),
                                cSendAPI.header.packet_unique_id,
                                strGetUUID);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユーザー管理：ユーザー承認
		@param	string uuid uuid指定
	*/
    /*
	 * リザルト：RecvUserAuthentication
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_UserAuthentication(string uuid = null)
    {
        //--------------------------------
        // UUID取得
        //
        // とりあえず暫定対応としてローカルセーブから参照。
        // 本来はローカルストレージに保存してるのを参照する。
        //--------------------------------
        string strUUID = "";

        if (uuid != null)
        {
            strUUID = uuid;
        }
        else if (LocalSaveManager.Instance != null)
        {
            strUUID = LocalSaveManager.Instance.LoadFuncUUID();
        }

        if (strUUID.Length <= 0)
        {
            Debug.LogError("UUID Error!");
            strUUID = DUMMY_UUID;
        }

        //--------------------------------
        // データ構築
        //--------------------------------
        SendUserAuthentication cSendAPI = new SendUserAuthentication();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.terminal = CreateStructTerminal();
        cSendAPI.uuid = strUUID;
        cSendAPI.boot = null;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_USER_AUTHENTICATION,
                                JsonMapper.ToJson(cSendAPI),
                                cSendAPI.header.packet_unique_id,
                                strUUID);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユーザー管理：ユーザー名称変更
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_RenameUser(string strUserName)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendRenameUser cSendAPI = new SendRenameUser();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.user_name = strUserName;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_USER_RENAME
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユーザー管理：ユーザー初期設定
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_SelectDefParty(uint unSelectPartyNum)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendSelectDefParty cSendAPI = new SendSelectDefParty();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.select_party = unSelectPartyNum;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_USER_SELECT_DEF_PARTY
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユーザー管理：ユーザー再構築
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_UserRenew(ref string strGetUUID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendRenewUser cSendAPI = new SendRenewUser();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.terminal = CreateStructTerminal();
        cSendAPI.uuid = Guid.NewGuid().ToString();

        //--------------------------------
        // 成功した場合にローカル保存するために保持しておく
        //--------------------------------
        strGetUUID = cSendAPI.uuid;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_USER_RENEW
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユーザー管理：ユーザー再構築情報問い合わせ
	*/
    /*
	 * リザルト：RecvCheckRenewUser
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_UserRenewCheck(string strUUID, uint unBeforeUserID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendCheckRenewUser cSendAPI = new SendCheckRenewUser();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.uuid_after = strUUID;
        cSendAPI.user_id_before = unBeforeUserID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                        SERVER_API.SERVER_API_USER_RENEW_CHECK,
                        JsonMapper.ToJson(cSendAPI),
                        cSendAPI.header.packet_unique_id
                        );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：マスターデータ操作：マスターデータハッシュ取得
	*/
    /*
	 * リザルト：RecvGetMasterHash
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetMasterHash()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetMasterHash cSendAPI = new SendGetMasterHash();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                        SERVER_API.SERVER_API_MASTER_HASH_GET
                    , JsonMapper.ToJson(cSendAPI)
                    , cSendAPI.header.packet_unique_id
                    );

    }

#if false
	//----------------------------------------------------------------------------
	/*!
		@brief	APIパケット送信：マスターデータ操作：マスターデータデータ取得
	*/
	//----------------------------------------------------------------------------
	static	public uint SendPacketAPI_GetMasterData( EMASTERDATA eMasterType )
	{
		//--------------------------------
		// データ構築
		//--------------------------------
		SendGetMasterData cSendAPI = new SendGetMasterData();

		cSendAPI.header			= CreateStructHeader();
		cSendAPI.master_type	= (uint)eMasterType;

		//--------------------------------
		// API送信リクエスト発行
		//--------------------------------
		return ServerDataManager.Instance.AddCommunicateRequest(
								SERVER_API.SERVER_API_MASTER_DATA_GET
							,	JsonMapper.ToJson( cSendAPI )
							,	cSendAPI.header.packet_unique_id
							);
	}
#endif
    static public ServerApi SendPacketAPI_Debug_GetMasterDataAll2(Dictionary<EMASTERDATA, uint> masterTagIdDict)
    {
#if BUILD_TYPE_DEBUG
        SendGetMasterDataAll cSendAPI = new SendGetMasterDataAll();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // マスターハッシュのローカル情報配列を生成
        //
        // クラサバ間で受け渡すか否かで選別せずに、全種のマスターについてとりあえず情報を送信。
        // どう扱うかはサーバーに委ねる。
        //--------------------------------
        cSendAPI.hash = new PacketStructMasterHash[(int)masterTagIdDict.Keys.Count];
        List<EMASTERDATA> keyList = masterTagIdDict.Keys.ToList();
        for (int i = 0; i < cSendAPI.hash.Length; i++)
        {
            EMASTERDATA m = keyList[i];
            cSendAPI.hash[i] = new PacketStructMasterHash();
            cSendAPI.hash[i].hash = "";
            cSendAPI.hash[i].timing = 0;
            cSendAPI.hash[i].type = (uint)m.Convert();
            cSendAPI.hash[i].tag_id = masterTagIdDict[m];
#if BUILD_TYPE_DEBUG
            Debug.Log("MASTER:" + m.ToString() + " TAG_ID:" + masterTagIdDict[m]);
#endif
        }

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_DEBUG_MASTER_DATA_GET_ALL2
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
#else
		return null;
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：マスターデータ操作：マスターデータデータ取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetMasterDataAll2(Dictionary<EMASTERDATA, uint> masterTagIdDict)
    {
        SendGetMasterDataAll cSendAPI = new SendGetMasterDataAll();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // マスターハッシュのローカル情報配列を生成
        //
        // クラサバ間で受け渡すか否かで選別せずに、全種のマスターについてとりあえず情報を送信。
        // どう扱うかはサーバーに委ねる。
        //--------------------------------
        cSendAPI.hash = new PacketStructMasterHash[(int)masterTagIdDict.Keys.Count];
        List<EMASTERDATA> keyList = masterTagIdDict.Keys.ToList();
        for (int i = 0; i < cSendAPI.hash.Length; i++)
        {
            EMASTERDATA m = keyList[i];
            cSendAPI.hash[i] = new PacketStructMasterHash();
            cSendAPI.hash[i].hash = "";
            cSendAPI.hash[i].timing = 0;
            cSendAPI.hash[i].type = (uint)m.Convert();
            cSendAPI.hash[i].tag_id = masterTagIdDict[m];
#if BUILD_TYPE_DEBUG
            Debug.Log("MASTER:" + m.ToString() + " TAG_ID:" + masterTagIdDict[m]);
#endif
        }

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_MASTER_DATA_GET_ALL2
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }
    static public ServerApi SendPacketAPI_GetMasterDataAll(List<EMASTERDATA> cMasterList)
    {
        SendGetMasterDataAll cSendAPI = new SendGetMasterDataAll();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // マスターハッシュのローカル情報配列を生成
        //
        // クラサバ間で受け渡すか否かで選別せずに、全種のマスターについてとりあえず情報を送信。
        // どう扱うかはサーバーに委ねる。
        //--------------------------------
        cSendAPI.hash = new PacketStructMasterHash[(int)cMasterList.Count];
        for (int i = 0; i < cSendAPI.hash.Length; i++)
        {
            cSendAPI.hash[i] = new PacketStructMasterHash();
            cSendAPI.hash[i].hash = "";
            cSendAPI.hash[i].timing = 0;
            cSendAPI.hash[i].type = (uint)cMasterList[i].Convert();
        }

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_MASTER_DATA_GET_ALL
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：アチーブメント操作：アチーブメントグループ情報取得 v350
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetAchievementGroup(uint unGategory, uint unPageNum, int nSortType)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendAchievementGroup cSendAPI = new SendAchievementGroup();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // アチーブメントの演出確認済み一覧をサーバーへ送信
        //--------------------------------
        cSendAPI.achievement_viewed = LocalSaveManager.Instance.LoadFuncAchievementEnsyutsu();

        //--------------------------------
        // パラメータ
        //--------------------------------
        cSendAPI.achievement_gp_cate = unGategory;
        cSendAPI.achievement_gp_page_no = unPageNum;
        cSendAPI.achievement_gp_sort_type = (uint)nSortType;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GET_ACHIEVEMENT_GP
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：アチーブメント操作：アチーブメント情報取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetMasterDataAchievement(uint unListGpId,
                                                                uint unPageNum,
                                                                int nSortType,
                                                                uint quest_id = 0,
                                                                uint unFilter = (uint)MasterDataDefineLabel.AchievementFilterType.ALL)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetMasterDataAchievement cSendAPI = new SendGetMasterDataAchievement();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // アチーブメントの演出確認済み一覧をサーバーへ送信
        //--------------------------------
        cSendAPI.achievement_viewed = LocalSaveManager.Instance.LoadFuncAchievementEnsyutsu();

        //--------------------------------
        // パラメータ
        //--------------------------------
        cSendAPI.achievement_gp_id = unListGpId;
        cSendAPI.achievement_page_no = unPageNum;
        cSendAPI.achievement_sort_type = (uint)nSortType;
        cSendAPI.quest_id = quest_id;
        cSendAPI.filter = unFilter;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GET_ACHIEVEMENT
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：マスターデータ操作：ストアイベント一覧取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetStoreProductEvent()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetStoreProductEvent cSendAPI = new SendGetStoreProductEvent();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GET_STORE_EVENT
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ログイン情報：ログインパック情報取得
	*/
    /*
	 * リザルト：RecvLoginPack
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_LoginPack(bool bGetLogin, bool bGetFriend, bool bGetHelper, bool bGetPresent)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendLoginPack cSendAPI = new SendLoginPack();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.get_login = (bGetLogin == true) ? 1 : 0;
        cSendAPI.get_friend = (bGetFriend == true) ? 1 : 0;
        cSendAPI.get_helper = (bGetHelper == true) ? 1 : 0;
        cSendAPI.get_present = (bGetPresent == true) ? 1 : 0;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                    SERVER_API.SERVER_API_GET_LOGIN_PACK,
                    JsonMapper.ToJson(cSendAPI),
                    cSendAPI.header.packet_unique_id
                    );
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ログイン情報：ログイン情報取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_LoginParam()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendLoginParam cSendAPI = new SendLoginParam();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GET_LOGIN_PARAM
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

#if false
	//----------------------------------------------------------------------------
	/*!
		@brief	APIパケット送信：プレイヤー情報：プレイヤー基本情報取得
	*/
	//----------------------------------------------------------------------------
	static	public uint SendPacketAPI_GetPlayerParam()
	{
		//--------------------------------
		// データ構築
		//--------------------------------
		SendGetPlayerParam cSendAPI = new SendGetPlayerParam();

		cSendAPI.header	= CreateStructHeader();

		//--------------------------------
		// API送信リクエスト発行
		//--------------------------------
		return ServerDataManager.Instance.AddCommunicateRequest(
								SERVER_API.SERVER_API_GET_PLAYER_PARAM
							,	JsonMapper.ToJson( cSendAPI )
							,	cSendAPI.header.packet_unique_id
							);
	}
#endif

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：フレンド操作：フレンド一覧取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_FriendListGet()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendFriendListGet cSendAPI = new SendFriendListGet();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_FRIEND_LIST_GET
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：フレンド操作：フレンド申請
	*/
    /*
	 * リザルト：RecvFriendRequest
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_FriendRequest(uint[] aunUserID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendFriendRequest cSendAPI = new SendFriendRequest();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.user_id = aunUserID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_FRIEND_REQUEST
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：フレンド操作：フレンド申請承認
	*/
    /*
	 * リザルト：RecvFriendConsent
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_FriendConsent(uint[] aunUserID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendFriendConsent cSendAPI = new SendFriendConsent();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.user_id = aunUserID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_FRIEND_CONSENT
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：フレンド操作：フレンド解除
	*/
    /*
	 * リザルト：RecvFriendRefusal
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_FriendRefusal(uint[] aunUserID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendFriendRefusal cSendAPI = new SendFriendRefusal();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.user_id = aunUserID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_FRIEND_REFUSAL
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：フレンド操作：フレンドユーザー検索
	*/
    /*
	 * リザルト：RecvFriendSearch
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_FriendSearch(uint unUserID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendFriendSearch cSendAPI = new SendFriendSearch();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.user_id = unUserID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_FRIEND_SEARCH
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユニット操作：ユニットパーティ編成設定
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_UnitPartyAssign(
            PacketStructPartyAssign[] acPartyAssign
        , int nCurrentParty
    )
    {
        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (acPartyAssign == null
        //		||	acPartyAssign.Length != GlobalDefine.PARTY_PATTERN_MAX
        || acPartyAssign.Length != UserDataAdmin.Instance.m_StructPartyAssign.Length
        )
        {
            Debug.LogError("PartyAssign Size NG!");
        }

        //--------------------------------
        // データ構築
        //--------------------------------
        SendUnitPartyAssign cSendAPI = new SendUnitPartyAssign();

        cSendAPI.header = CreateStructHeader();

        cSendAPI.party_assign = acPartyAssign;
        cSendAPI.party_current = nCurrentParty;


        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_UNIT_PARTY_ASSIGN
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユニット操作：ユニット売却
	*/
    /*
	 * リザルト：RecvUnitSale
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_UnitSale(long[] aunUnitUniqueID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendUnitSale cSendAPI = new SendUnitSale();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.unit_unique_id = aunUnitUniqueID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_UNIT_SALE
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユニット操作：ユニット強化合成
	*/
    /*
	 * リザルト：RecvUnitBlendBuildUp
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_UnitBlendBuildUp(
        long unUnitUniqueIDBase
    , long[] aunUnitUniqueIDParts
    , uint unEventSLV
    , int nBeginnerBoostID
    , int nTutorialActive
    , bool is_premium

    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendUnitBlendBuildUp cSendAPI = new SendUnitBlendBuildUp();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.unique_id_base = unUnitUniqueIDBase;
        cSendAPI.unique_id_parts = aunUnitUniqueIDParts;
        cSendAPI.event_skilllv = unEventSLV;
        cSendAPI.beginner_boost_id = nBeginnerBoostID;
        cSendAPI.buildup_tutorial = (uint)nTutorialActive;
        cSendAPI.is_premium = is_premium;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_UNIT_BLEND_BUILDUP
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユニット操作：ユニット進化合成
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_UnitBlendEvol(
        uint unUnitUniqueIDBase
    , uint[] aunUnitUniqueIDParts
    , uint unBlendPattern
    , uint unBlendUnitID
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendUnitBlendEvol cSendAPI = new SendUnitBlendEvol();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.blend_unit_id = unBlendUnitID;
        cSendAPI.blend_pattern = unBlendPattern;
        cSendAPI.unique_id_base = unUnitUniqueIDBase;
        cSendAPI.unique_id_parts = aunUnitUniqueIDParts;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_UNIT_BLEND_EVOL
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：助っ人一覧取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_QuestHelperGet()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuestHelperGet cSendAPI = new SendQuestHelperGet();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST_HELPER_GET
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：助っ人一覧取得（進化合成向け）
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_QuestHelperGetEvol(long unBaseCharaUniqueID, uint unBlendPatternID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuestHelperGetEvol cSendAPI = new SendQuestHelperGetEvol();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.blend_base_unique_id = unBaseCharaUniqueID;
        cSendAPI.blend_pattern = unBlendPatternID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST_HELPER_GET_EVOL
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：助っ人一覧取得（強化合成向け）
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_QuestHelperGetBuild(long unBaseCharaUniqueID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuestHelperGetBuild cSendAPI = new SendQuestHelperGetBuild();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.blend_base_unique_id = unBaseCharaUniqueID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST_HELPER_GET_BUILD
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：クエスト開始
	*/
    /*
	 * リザルト：RecvQuestStart
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_QuestStart(
        uint unQuestID
    , uint unQuestState
    , uint unHelperUserID
    , PacketStructUnit sHelperUnit
    , bool bHelperPointActive
    , int nPartyCurrent
    , uint[] aunDropUnitList

    , uint unEventFP

    , int nBeginnerBoostID
    , uint[] aunareaAmendID
    , PacketStructUnit sHelperLinkUnit
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuestStart cSendAPI = new SendQuestStart();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = unQuestID;
        cSendAPI.quest_state = unQuestState;
        cSendAPI.helper_unit = sHelperUnit;
        cSendAPI.helper_user_id = unHelperUserID;
        cSendAPI.helper_point_ok = (bHelperPointActive == true) ? 1 : 0;
        cSendAPI.party_current = nPartyCurrent;
        cSendAPI.all_unit_kind = aunDropUnitList;
        cSendAPI.event_friendpoint = unEventFP;
        cSendAPI.beginner_boost_id = nBeginnerBoostID;
        cSendAPI.area_amend_id = aunareaAmendID;
        cSendAPI.helper_unit_link = sHelperLinkUnit;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST_START
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：クエストクリア
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_QuestClear(
        uint unQuestID
    , uint unGetExp
    , uint unGetMoney
    , PacketStructUnitGet[] asGetUnit
    , int[] anOpenQuest
    )
    {

        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuestClear cSendAPI = new SendQuestClear();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = unQuestID;
        cSendAPI.get_exp = unGetExp;
        cSendAPI.get_money = unGetMoney;
        cSendAPI.get_unit = null;
        cSendAPI.security_key = 0;

        cSendAPI.panel_unique = anOpenQuest;


        //--------------------------------
        // 無駄データが挟まらないようにユニットが入力されてる部分だけのバッファを作る
        //--------------------------------
        uint unUnitTotal = 0;
        for (int i = 0; i < asGetUnit.Length; i++)
        {
            if (asGetUnit[i] == null
            || asGetUnit[i].id == 0
            ) continue;

            unUnitTotal++;
        }

        if (unUnitTotal != 0)
        {
            cSendAPI.get_unit = new PacketStructUnitGet[unUnitTotal];

            unUnitTotal = 0;
            for (int i = 0; i < asGetUnit.Length; i++)
            {
                if (asGetUnit[i] == null
                || asGetUnit[i].id == 0
                ) continue;

                cSendAPI.get_unit[unUnitTotal] = asGetUnit[i];
                unUnitTotal++;
            }
        }

#if true
        //--------------------------------
        // チート対策のために適当なセキュリティキー情報を送る。
        //
        // 同じ計算式を使ってサーバー上でキーを作成して比較することで、
        // 単純なパケット捏造によるユニットの付与が行われても弾けるようにしていく
        //--------------------------------
        cSendAPI.security_key = CreateSecurityKey_QuestClear(unQuestID, unGetExp, unGetMoney, cSendAPI.get_unit);
#endif

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST_CLEAR
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：進化クエスト開始
	*/
    /*
	 * リザルト：RecvEvolQuestStart
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_QuestEvolStart(
        uint unQuestID
    , uint unQuestState
    , uint unHelperUserID
    , PacketStructUnit sHelperUnit
    , bool bHelperPremium
    , bool bHelperPointActive

    , long unEvolUnitUniqueIDBase
    , long[] aunEvolUnitUniqueIDParts
    , uint unBlendPattern
    , uint unBlendUnitID

    , uint[] aunDropUnitList

    , uint unEventFP

    , int nBeginnerBoostID
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendEvolQuestStart cSendAPI = new SendEvolQuestStart();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = unQuestID;
        cSendAPI.quest_state = unQuestState;

        cSendAPI.unique_id_base = unEvolUnitUniqueIDBase;
        cSendAPI.unique_id_parts = aunEvolUnitUniqueIDParts;
        cSendAPI.blend_unit_id = unBlendUnitID;
        cSendAPI.blend_pattern = unBlendPattern;

        cSendAPI.helper_unit = sHelperUnit;
        cSendAPI.helper_user_id = unHelperUserID;
        cSendAPI.helper_premium = (bHelperPremium == true) ? (uint)1 : (uint)0;
        cSendAPI.helper_point_ok = (bHelperPointActive == true) ? 1 : 0;

        cSendAPI.all_unit_kind = aunDropUnitList;

        cSendAPI.event_friendpoint = unEventFP;

        cSendAPI.beginner_boost_id = nBeginnerBoostID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_EVOL_QUEST_START
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：進化クエストクリア
	*/
    /*
	 * リザルト：RecvEvolQuestClear
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_QuestEvolClear(
        uint unQuestID
    , uint unGetExp
    , uint unGetMoney
    , PacketStructUnitGet[] asGetUnit
    , int[] anOpenQuest
    , int nTutorialActive
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendEvolQuestClear cSendAPI = new SendEvolQuestClear();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = unQuestID;
        cSendAPI.get_exp = unGetExp;
        cSendAPI.get_money = unGetMoney;
        cSendAPI.get_unit = null;
        cSendAPI.security_key = 0;

        cSendAPI.panel_unique = anOpenQuest;

        cSendAPI.evol_tutorial = (uint)nTutorialActive;

        //--------------------------------
        // 無駄データが挟まらないようにユニットが入力されてる部分だけのバッファを作る
        //--------------------------------
        TemplateList<PacketStructUnitGet> acUnitList = new TemplateList<PacketStructUnitGet>();
        for (int i = 0; i < asGetUnit.Length; i++)
        {
            if (asGetUnit[i] == null
            || asGetUnit[i].id == 0
            ) continue;

            acUnitList.Add(asGetUnit[i]);
        }
        if (acUnitList.m_BufferSize > 0)
        {
            cSendAPI.get_unit = acUnitList.ToArray();
        }

#if false
		if( cSendAPI.get_unit.Length > 0 )
		{
			for( int i = 0;i < cSendAPI.get_unit.Length;i++ )
			{
				Debug.LogError( "GetUnit - " + cSendAPI.get_unit[ i ].id );
			}
		}
#endif

#if true
        //--------------------------------
        // チート対策のために適当なセキュリティキー情報を送る。
        //
        // 同じ計算式を使ってサーバー上でキーを作成して比較することで、
        // 単純なパケット捏造によるユニットの付与が行われても弾けるようにしていく
        //--------------------------------
        cSendAPI.security_key = CreateSecurityKey_QuestClear(unQuestID, unGetExp, unGetMoney, cSendAPI.get_unit);
#endif

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_EVOL_QUEST_CLEAR
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    #region === 新クエスト関連 ===

    static public ServerApi SendPacketAPI_Quest2StartTutorial(
        uint unQuestID
    , uint unQuestState
    , uint unHelperUserID
    , PacketStructUnit sHelperUnit
    , bool bHelperPointActive
    , int nPartyCurrent
    , uint unEventFP
    , int nBeginnerBoostID
    , uint[] aunareaAmendID
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuest2Start cSendAPI = new SendQuest2Start();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = unQuestID;
        cSendAPI.quest_state = unQuestState;
        cSendAPI.helper_unit = sHelperUnit;
        cSendAPI.helper_user_id = unHelperUserID;
        cSendAPI.helper_point_ok = (bHelperPointActive == true) ? 1 : 0;
        cSendAPI.party_current = nPartyCurrent;
        cSendAPI.event_friendpoint = unEventFP;
        cSendAPI.beginner_boost_id = nBeginnerBoostID;
        cSendAPI.area_amend_id = aunareaAmendID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST2_START_TUTORIAL
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：クエスト開始
	*/
    /*
	 * リザルト：RecvQuestStart
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_Quest2Start(
        uint unQuestID
    , uint unQuestState
    , uint unHelperUserID
    , PacketStructUnit sHelperUnit
    , bool bHelperPointActive
    , int nPartyCurrent
    , uint unEventFP
    , int nBeginnerBoostID
    , uint[] aunareaAmendID
    , bool is_auto_play
    , PacketStructUnit sHelperLinkUnit = null
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuest2Start cSendAPI = new SendQuest2Start();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = unQuestID;
        cSendAPI.quest_state = unQuestState;
        cSendAPI.helper_unit = sHelperUnit;
        cSendAPI.helper_user_id = unHelperUserID;
        cSendAPI.helper_point_ok = (bHelperPointActive == true) ? 1 : 0;
        cSendAPI.party_current = nPartyCurrent;
        cSendAPI.event_friendpoint = unEventFP;
        cSendAPI.beginner_boost_id = nBeginnerBoostID;
        cSendAPI.area_amend_id = aunareaAmendID;
        cSendAPI.is_auto_play = is_auto_play;
        cSendAPI.helper_unit_link = sHelperLinkUnit;

#if BUILD_TYPE_DEBUG
        // デバッグ機能：期間外のクエストを開始する（開始タイミングが期間内であることにしてAPIを呼ぶ）
        if (DebugOption.Instance.featureQuest)
        {
            MasterDataQuest2 master_data_quest = MasterDataUtil.GetQuest2ParamFromID(cSendAPI.quest_id);
            if (master_data_quest != null)
            {
                MasterDataArea master_data_area = MasterFinder<MasterDataArea>.Instance.Find((int)master_data_quest.area_id);
                if (master_data_area != null)
                {
                    MasterDataEvent master_data_event = MasterFinder<MasterDataEvent>.Instance.Find((int)master_data_area.event_id);
                    if (master_data_event != null)
                    {
                        DateTime start_date = TimeUtil.GetDateTime(master_data_event.timing_start);
                        if (master_data_event.period_type != MasterDataDefineLabel.PeriodType.CYCLE)
                        {
                            if (start_date.CompareTo(DateTime.Now) >= 0)
                            {
                                cSendAPI.header.local_time = TimeUtil.ConvertLocalTimeToServerTime(start_date);
                            }
                        }
                        else
                        {
                            if (master_data_event.timing_end == 0
                                || TimeUtil.GetDateTime(master_data_event.timing_end).CompareTo(DateTime.Now) >= 0
                            )
                            {
                                // 開催曜日まで日付を進める
                                for (int idx = 0; idx < 7; idx++)
                                {
                                    DayOfWeek week = start_date.DayOfWeek;
                                    int week_idx = 0;
                                    switch (week)
                                    {
                                        case DayOfWeek.Sunday:
                                            week_idx = TimeUtil.BIT_SUNDAY;
                                            break;

                                        case DayOfWeek.Monday:
                                            week_idx = TimeUtil.BIT_MONDAY;
                                            break;

                                        case DayOfWeek.Tuesday:
                                            week_idx = TimeUtil.BIT_TUESDAY;
                                            break;

                                        case DayOfWeek.Wednesday:
                                            week_idx = TimeUtil.BIT_WEDNESDAY;
                                            break;

                                        case DayOfWeek.Thursday:
                                            week_idx = TimeUtil.BIT_THURSDAY;
                                            break;

                                        case DayOfWeek.Friday:
                                            week_idx = TimeUtil.BIT_FRIDAY;
                                            break;

                                        case DayOfWeek.Saturday:
                                            week_idx = TimeUtil.BIT_SATURDAY;
                                            break;
                                    }

                                    int week_bit = (1 << (int)week_idx);

                                    if ((master_data_event.cycle_date_type & week_bit) != 0)
                                    {
                                        break;
                                    }

                                    start_date = start_date.AddADay();
                                }

                                if (start_date.DayOfWeek != DateTime.Now.DayOfWeek)
                                {
                                    cSendAPI.header.local_time = TimeUtil.ConvertLocalTimeToServerTime(start_date);
                                }
                            }
                        }
                    }
                }
            }
        }
#endif //BUILD_TYPE_DEBUG

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST2_START
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：クエストクリア
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_Quest2Clear(
        uint unQuestID,
        SendEnemyKill[] enemy_kill_list,
        bool no_damage,
        int max_damag,
        SendSkillExecCount[] active_skill_execute_count,
        int hero_skill_execute_count,
        PlayScoreInfo play_score_info,
        bool is_auto_play
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuest2Clear cSendAPI = new SendQuest2Clear();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = unQuestID;
        cSendAPI.enemy_kill_list = enemy_kill_list;
        cSendAPI.no_damage = no_damage;
        cSendAPI.max_damage = max_damag;
        cSendAPI.active_skill_execute_count = active_skill_execute_count;
        cSendAPI.hero_skill_execute_count = hero_skill_execute_count;
        cSendAPI.play_score_info = play_score_info;
        cSendAPI.is_auto_play = is_auto_play;

#if false
        //--------------------------------
        // チート対策のために適当なセキュリティキー情報を送る。
        //
        // 同じ計算式を使ってサーバー上でキーを作成して比較することで、
        // 単純なパケット捏造によるユニットの付与が行われても弾けるようにしていく
        //--------------------------------
        cSendAPI.security_key = CreateSecurityKey_QuestClear(unQuestID, unGetExp, unGetMoney, cSendAPI.get_unit);
#endif

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST2_CLEAR
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }
    #endregion

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：UNIT進化
	*/
    /*
	 * リザルト：RecvUnitEvoltOnSendEvolveUnit
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_Evolve_Unit(
     long unEvolUnitUniqueIDBase
    , long[] aunEvolUnitUniqueIDParts
    , uint chara_evol_id
    , uint after_unit_chara_id
    , int nBeginnerBoostID
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendEvolveUnit cSendAPI = new SendEvolveUnit();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.evolve_base_unit_id = unEvolUnitUniqueIDBase;
        cSendAPI.material_unit_list = aunEvolUnitUniqueIDParts;
        cSendAPI.chara_evol_id = chara_evol_id;
        cSendAPI.after_unit_chara_id = after_unit_chara_id;
        cSendAPI.beginner_boost_id = nBeginnerBoostID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_EVOLVE_UNIT
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：クエストリタイア
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_QuestRetire(uint unQuestID, bool is_auto_play)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuestRetire cSendAPI = new SendQuestRetire();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = unQuestID;
        cSendAPI.is_auto_play = is_auto_play;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST_RETIRE
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：クエスト受託情報取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_QuestOrderGet(int nDetailFlag)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuestOrderGet cSendAPI = new SendQuestOrderGet();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.detail_flag = nDetailFlag;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST_ORDER_GET
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：クエスト受託情報取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_Quest2OrderGet(int nDetailFlag)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuest2OrderGet cSendAPI = new SendQuest2OrderGet();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.detail_flag = nDetailFlag;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST2_ORDER_GET
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  APIパケット送信：クエスト操作：ストーリクエスト開始
    */
    /*
     * リザルト：
     */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_Quest2StoryStart(uint unQuestID)
    {

        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuest2StoryStart cSendAPI = new SendQuest2StoryStart();
        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = (int)unQuestID;


        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST2_STORY_START
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  APIパケット送信：クエスト操作：ストーリクエストクリア
    */
    /*
     * リザルト：
     */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_Quest2StoryClear(uint unQuestID)
    {

        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuest2StoryClear cSendAPI = new SendQuest2StoryClear();
        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = (int)unQuestID;


        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST2_STORY_CLEAR
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：インゲーム中：コンティニュー
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_QuestContinue(uint unContinueCt, bool is_auto_play)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuestContinue cSendAPI = new SendQuestContinue();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.continue_ct = unContinueCt;
        cSendAPI.is_auto_play = is_auto_play;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST_CONTINUE
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト操作：新クエスト中断データ破棄通知
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_Quest2CessaionQuest(bool is_auto_play)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuest2CessaionQuest cSendAPI = new SendQuest2CessaionQuest();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.is_auto_play = is_auto_play;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                              SERVER_API.SERVER_API_QUEST2_CESSAION_QUEST
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：インゲーム中：リセット
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_QuestReset(uint unResetCt, int nFloor)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuestReset cSendAPI = new SendQuestReset();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.reset_ct = unResetCt;
        cSendAPI.floor = nFloor;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_QUEST_RESET
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：不正検出関連：不正検出送信
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_Injustice(uint[] aunInjusticeFlag)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendInjustice cSendAPI = new SendInjustice();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.injustice_flag = aunInjusticeFlag;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_INJUSTICE
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：チュートリアル関連：進行集計
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_Tutorial(uint unTutorialStep)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendTutorialStep cSendAPI = new SendTutorialStep();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.tutorial_step = unTutorialStep;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_TUTORIAL
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：リニューアルチュートリアル関連：進行集計
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_RenewTutorial(uint unTutorialStep)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendRenewTutorialStep cSendAPI = new SendRenewTutorialStep();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.tutorial_step = unTutorialStep;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_RENEW_TUTORIAL
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：魔法石使用：ユニット枠増設
	*/
    /*
	 * リザルト：RecvStoneUsedUnit
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_StoneUsedUnit()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendStoneUsedUnit cSendAPI = new SendStoneUsedUnit();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_STONE_USE_UNIT
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：魔法石使用：フレンド枠増設
	*/
    /*
	 * リザルト：RecvStoneUsedFriend
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_StoneUsedFriend()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendStoneUsedFriend cSendAPI = new SendStoneUsedFriend();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_STONE_USE_FRIEND
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

#if false
	//----------------------------------------------------------------------------
	/*!
		@brief	APIパケット送信：魔法石使用：パーティコスト増加
	*/
	//----------------------------------------------------------------------------
	static	public uint SendPacketAPI_StoneUsedPartyCost()
	{
		//--------------------------------
		// データ構築
		//--------------------------------
		SendStoneUsedPartyCost cSendAPI = new SendStoneUsedPartyCost();

		cSendAPI.header	= CreateStructHeader();

		//--------------------------------
		// API送信リクエスト発行
		//--------------------------------
		return ServerDataManager.Instance.AddCommunicateRequest(
								SERVER_API.SERVER_API_STONE_USE_PARTY_COST
							,	JsonMapper.ToJson( cSendAPI )
							,	cSendAPI.header.packet_unique_id
							);
	}
#endif

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：魔法石使用：スタミナ回復
	*/
    /*
	 * リザルト：RecvStoneUsedStamina
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_StoneUsedStamina()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendStoneUsedStamina cSendAPI = new SendStoneUsedStamina();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_STONE_USE_STAMINA
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ガチャ操作：ガチャ実行
	*/
    /*
	 * リザルト：RecvGachaPlay
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GachaPlay(
        uint unGachaID
    , uint unGachaCt
    , uint unTutorialFlag
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGachaPlay cSendAPI = new SendGachaPlay();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.gacha_id = unGachaID;
        cSendAPI.gacha_ct = unGachaCt;
        cSendAPI.gacha_tutorial = unTutorialFlag;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GACHA_PLAY
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ガチャ操作：ガチャチケット実行
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GachaTicketPlay(
        long unPresentSerialID
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGachaTicketPlay cSendAPI = new SendGachaTicketPlay();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.present_serial_id = unPresentSerialID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GACHA_TICKET_PLAY
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：課金操作：魔法石購入直前処理( iOS … AppStore )
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_StorePayPrev_ios(
            string strProductID
        , uint unEventID
        , int nAgeType
    )
    {

        //--------------------------------
        // データ構築
        //--------------------------------
        SendStorePayPrev_ios cSendAPI = new SendStorePayPrev_ios();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.product_id = strProductID;
        cSendAPI.event_id = unEventID;
        cSendAPI.age_type = nAgeType;

#if BUILD_TYPE_DEBUG
        Debug.Log("StoreID:" + strProductID + " , EventID:" + unEventID);
#endif
        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_STONE_PAY_PREV_IOS
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：課金操作：魔法石購入直前処理( Android … GooglePlay )
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_StorePayPrev_android(
            string strProductID
        , uint unEventID
        , int nAgeType
    )
    {

        //--------------------------------
        // データ構築
        //--------------------------------
        SendStorePayPrev_android cSendAPI = new SendStorePayPrev_android();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.product_id = strProductID;
        cSendAPI.event_id = unEventID;
        cSendAPI.age_type = nAgeType;

#if BUILD_TYPE_DEBUG
        Debug.Log("StoreID:" + strProductID + " , EventID:" + unEventID);
#endif
        //		cSendAPI.reserve0		= "";
        //		cSendAPI.reserve1		= "";

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------

        return ServerApi.Create(
                                SERVER_API.SERVER_API_STONE_PAY_PREV_ANDROID
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：課金操作：魔法石購入反映処理( iOS … AppStore )
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_StorePay_ios(
        string strReceipt
    )
    {

        //--------------------------------
        // データ構築
        //--------------------------------
        SendStorePay_ios cSendAPI = new SendStorePay_ios();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.receipt = strReceipt;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_STONE_PAY_IOS
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：課金操作：魔法石購入反映処理( Android … GooglePlay )
	*/
    /*
	 * リザルト：RecvStorePay_android
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_StorePay_android(
        string strReceipt
    , string strReceiptSign
    )
    {

        //--------------------------------
        // データ構築
        //--------------------------------
        SendStorePay_android cSendAPI = new SendStorePay_android();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.receipt = strReceipt;
        cSendAPI.receipt_sign = strReceiptSign;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_STONE_PAY_ANDROID
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }



    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：課金操作：魔法石購入キャンセル処理
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_StorePayCancel(
        string strProductID
        , uint unEventID
    )
    {

        //--------------------------------
        // データ構築
        //--------------------------------
        SendStorePayCancel cSendAPI = new SendStorePayCancel();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.product_id = strProductID;
        cSendAPI.event_id = unEventID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_STORE_PAY_CANCEL
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユーザーレビュー関連：レビュー遷移報酬
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_ReviewPresent()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendReviewPresent cSendAPI = new SendReviewPresent();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_REVIEW_PRESENT
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：プレゼント関連：プレゼント一覧取得
	*/
    /*
	 * リザルト：RecvPresentListGet
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_PresentListGet()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendPresentListGet cSendAPI = new SendPresentListGet();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_PRESENT_LIST_GET
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：プレゼント関連：プレゼント開封
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_PresentOpen(
        long[] aunPresentSerialID
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendPresentOpen cSendAPI = new SendPresentOpen();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.present_serial_id = aunPresentSerialID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_PRESENT_OPEN
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：セーブ移行：パスワード発行
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_TransferOrder()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendTransferOrder cSendAPI = new SendTransferOrder();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_TRANSFER_ORDER
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：セーブ移行：移行実行
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_TransferExec(uint unUserID, string strPassword, string uuid)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendTransferExec cSendAPI = new SendTransferExec();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.move_user_id = unUserID;
        cSendAPI.password = strPassword;


        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_TRANSFER_EXEC
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            , uuid);
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：アチーブメント開封：開封実行
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_AchievementOpen(uint[] aunAchievementOpen, uint[] aunAchievementGroupOpen)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendOpenAchievement cSendAPI = new SendOpenAchievement();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.achievement_open = aunAchievementOpen;
        cSendAPI.achievement_viewed = LocalSaveManager.Instance.LoadFuncAchievementEnsyutsu();

        cSendAPI.achievement_gp_open = aunAchievementGroupOpen;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_ACHIEVEMENT_OPEN
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：SNSアカウント紐付け確認：送信
		@param[in]	sns_id		検索するSNSID
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_CheckSnsLink(string sns_id)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendCheckSnsLink cSendAPI = new SendCheckSnsLink();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.sns_id = sns_id;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
            SERVER_API.SERVER_API_CHECK_SNS_LINK
            , JsonMapper.ToJson(cSendAPI)
            , cSendAPI.header.packet_unique_id
            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：SNSアカウント紐付け確認：送信
		@param[in]	sns_id		紐付けるSNSID
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_SetSnsLink(string sns_id)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendSetSnsLink cSendAPI = new SendSetSnsLink();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.sns_id = sns_id;
        // 自分しか紐付けないようにここで入れとく.
        cSendAPI.user_id = UserDataAdmin.Instance.m_StructPlayer.user.user_id;

        //--------------------------------
        // アチーブメントの演出確認済み一覧をサーバーへ送信
        //--------------------------------
        cSendAPI.achievement_viewed = LocalSaveManager.Instance.LoadFuncAchievementEnsyutsu();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
            SERVER_API.SERVER_API_SET_SNS_LINK
            , JsonMapper.ToJson(cSendAPI)
            , cSendAPI.header.packet_unique_id
            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：SNSアカウント紐付け確認：送信
		@param[in]	sns_id		以降するSNSID
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_MoveSnsSaveData(string sns_id)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendMoveSnsSaveData cSendAPI = new SendMoveSnsSaveData();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.sns_id = sns_id;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
            SERVER_API.SERVER_API_MOVE_SNS_SAVE_DATA
            , JsonMapper.ToJson(cSendAPI)
            , cSendAPI.header.packet_unique_id
            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：SNSID作成：送信
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetSnsID()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetSnsID cSendAPI = new SendGetSnsID();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
            SERVER_API.SERVER_API_GET_SNS_ID
            , JsonMapper.ToJson(cSendAPI)
            , cSendAPI.header.packet_unique_id
            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIタイプ：ポイントショップ：ショップ商品情報を取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetPointShopProduct()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetPointShopProduct cSendAPI = new SendGetPointShopProduct();

        cSendAPI.header = CreateStructHeader();
        //--------------------------------
        // 2016/04/19 通信前のパッチ取得対応に伴いハッシュチェックが不要と判断された為
        // 現段階ではコメントアウトし、サーバ側にハッシュを送らないようになっています
        // サーバー側もハッシュチェックを外していただいています

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
            SERVER_API.SERVER_API_GET_POINT_SHOP_PRODUCT
            , JsonMapper.ToJson(cSendAPI)
            , cSendAPI.header.packet_unique_id
            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIタイプ：ポイントショップ：商品購入
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_PointShopPurchase(uint unProductID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendPointShopPurchase cSendAPI = new SendPointShopPurchase();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.point_shop_product_id = unProductID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
            SERVER_API.SERVER_API_POINT_SHOP_PURCHASE
            , JsonMapper.ToJson(cSendAPI)
            , cSendAPI.header.packet_unique_id
            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIタイプ：ポイントショップ：限界突破
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_PointShopLimitOver(uint unProductID, long unUnitUniqueIDBase, uint unLimitOverCount)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendPointShopLimitOver cSendAPI = new SendPointShopLimitOver();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.point_shop_product_id = unProductID;
        cSendAPI.unit_unique_id = unUnitUniqueIDBase;
        cSendAPI.limit_over_count = unLimitOverCount;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
            SERVER_API.SERVER_API_POINT_SHOP_LIMITOVER
            , JsonMapper.ToJson(cSendAPI)
            , cSendAPI.header.packet_unique_id
            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIタイプ：ポイントショップ：進化
	*/
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_PointShopEvol(uint unProductID, long unUnitUniqueIDBase, uint unBlendUnitId, uint unBlendPattern)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendPointShopEvol cSendAPI = new SendPointShopEvol();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.point_shop_product_id = unProductID;
        cSendAPI.unit_unique_id = unUnitUniqueIDBase;
        cSendAPI.blend_unit_id = unBlendUnitId;
        cSendAPI.blend_pattern = unBlendPattern;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
            SERVER_API.SERVER_API_POINT_SHOP_EVOL
            , JsonMapper.ToJson(cSendAPI)
            , cSendAPI.header.packet_unique_id
            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユニット操作：ユニットリンク実行
	*/
    /*
	 * リザルト：RecvUnitLink
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_UnitLinkCreate(long unUnitUniqueIDBase,
                                                      long unUnitUniqueIDLink,
                                                      long[] aunUnitUniqueIDParts,
                                                      int nBeginnerBoostID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendUnitLinkCreate cSendAPI = new SendUnitLinkCreate();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.unique_id_base = unUnitUniqueIDBase;
        cSendAPI.unique_id_link = unUnitUniqueIDLink;
        cSendAPI.unique_id_parts = aunUnitUniqueIDParts;
        cSendAPI.beginner_boost_id = nBeginnerBoostID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_UNIT_LINK_CREATE
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ユニット操作：ユニットリンク解除
	*/
    /*
	 * リザルト：RecvUnitLink
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_UnitLinkDelete(long unUnitUniqueIDBase,
                                                      long[] aunUnitUniqueIDParts,
                                                      int nBeginnerBoostID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendUnitLinkDelete cSendAPI = new SendUnitLinkDelete();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.unique_id_base = unUnitUniqueIDBase;
        cSendAPI.unique_id_parts = aunUnitUniqueIDParts;
        cSendAPI.beginner_boost_id = nBeginnerBoostID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_UNIT_LINK_DELETE
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：消費アイテム操作：アイテム使用
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_ItemUse(uint unItemId)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendItemUse cSendAPI = new SendItemUse();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.item_id = unItemId;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
            SERVER_API.SERVER_API_USE_ITEM
            , JsonMapper.ToJson(cSendAPI)
            , cSendAPI.header.packet_unique_id
            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：BOXガチャ操作：在庫状況取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetBoxGachaStock(uint uGachaID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetBoxGachaStock cSendAPI = new SendGetBoxGachaStock();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.fix_id = uGachaID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
            SERVER_API.SERVER_API_GET_BOX_GACHA_STOCK
            , JsonMapper.ToJson(cSendAPI)
            , cSendAPI.header.packet_unique_id
            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：BOXガチャ操作：在庫状況リセット
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_ResetBoxGachaStock(uint uGachaID)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendResetBoxGachaStock cSendAPI = new SendResetBoxGachaStock();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.fix_id = uGachaID;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
            SERVER_API.SERVER_API_RESET_BOX_GACHA_STOCK
            , JsonMapper.ToJson(cSendAPI)
            , cSendAPI.header.packet_unique_id
            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：主人公選択：主人公選択
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_SetCurrentHero(int heroId, int unique_id)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendSetCurrentHero cSendAPI = new SendSetCurrentHero();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.hero_id = heroId;
        cSendAPI.unique_id = unique_id;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_SET_CURRENT_HERO
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：クエスト：ゲリラボス情報取得
	*/
    /*
	 * リザルト：RecvGetGuerrillaBossInfo
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetGuerrillaBossInfo(int[] area_id_list)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetGuerrillaBossInfo cSendAPI = new SendGetGuerrillaBossInfo();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.area_id_list = area_id_list;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GET_GUERRILLA_BOSS_INFO
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ガチャ：ガチャラインナップ詳細
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetGachaLineup(int gachaId, uint step_id, uint gacha_assign_id)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetGachaLineup cSendAPI = new SendGetGachaLineup();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.gacha_id = gachaId;
        cSendAPI.step_id = step_id;
        cSendAPI.assign_id = gacha_assign_id;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                            SERVER_API.SERVER_API_GET_GACHA_LINEUP
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：定期データ更新(デバイストークン)
	*/
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_PeriodicUpdate(string sDeviceToken)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendPeriodicUpdate cSendAPI = new SendPeriodicUpdate();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.device_token = sDeviceToken;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                            SERVER_API.SERVER_API_PERIODIC_UPDATE
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：デバッグ機能関連：ユーザーデータ更新
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_DebugEditUser(
        int nGameMoney          //!< 入手情報：お金
    , int nFreePaidPoint        //!< 入手情報：無料チップ
    , int nFriendPoint      //!< 入手情報：フレンドポイント
    , int nRank             //!< 入手情報：ランク
    , int nBuyMaxUnitCount  //!< 入手情報：ユニット枠購入数
    , int nBuyMaxFriendCount    //!< 入手情報：フレンド枠購入数
    , int nTicketCasino     //!< 入手情報：カジノチケット
    , int nUnitPoint            //!< 入手情報：ユニットポイント
    , int nEventPointId     //!< 入手情報：イベントポイントID
    , int nEventPointCount  //!< 入手情報：ユニットポイント数
    , int nAllQuestClear        //!< 入手情報：クエストクリア情報
    , int nResetGachaID    //!< 入手情報:リセットするガチャID
    )
    {
#if BUILD_TYPE_DEBUG
        //--------------------------------
        // データ構築
        //--------------------------------
        SendDebugEditUser cSendAPI = new SendDebugEditUser();
        // 共通ヘッダ
        cSendAPI.header = CreateStructHeader();
        // 編集項目
        cSendAPI.game_money = nGameMoney;           //!< 入手情報：お金
        cSendAPI.free_paid_point = nFreePaidPoint;      //!< 入手情報：無料チップ
        cSendAPI.friend_point = nFriendPoint;           //!< 入手情報：フレンドポイント
        cSendAPI.rank = nRank;              //!< 入手情報：ランク
        cSendAPI.buy_max_unit_count = nBuyMaxUnitCount;     //!< 入手情報：ユニット枠購入数
        cSendAPI.buy_max_friend_count = nBuyMaxFriendCount; //!< 入手情報：フレンド枠購入数
        cSendAPI.ticket_casino = nTicketCasino;     //!< 入手情報：カジノチケット
        cSendAPI.unit_point = nUnitPoint;           //!< 入手情報：ユニットポイント
        cSendAPI.event_point_id = nEventPointId;        //!< 入手情報：イベントポイントID
        cSendAPI.event_point_count = nEventPointCount;      //!< 入手情報：イベントポイント数
        cSendAPI.all_quest_clear = nAllQuestClear;      //!< 入手情報：クエストクリア情報
        cSendAPI.reset_gacha_id = nResetGachaID;  //!< 入手情報:リセットするガチャID

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_DEBUG_EDIT_USER
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
#else
		return null;
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：デバッグ機能関連：成長曲線検証
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_DebugCalculationData(AppliCalculationData[] asCalculationData, int nSeqID = 0)
    {
#if BUILD_TYPE_DEBUG
        //--------------------------------
        // データ構築
        //--------------------------------
        SendDebugCalculationData cSendAPI = new SendDebugCalculationData();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.calculation_data = null;

        //--------------------------------
        // 無駄データが挟まらないように、データが入力されてる部分だけのバッファを作る
        //--------------------------------
        TemplateList<AppliCalculationData> acUnitList = new TemplateList<AppliCalculationData>();
        int nCalculationDataMax = asCalculationData.Length;
        for (int num = 0; num < nCalculationDataMax; ++num)
        {
            if (asCalculationData[num] == null)
            {
                continue;
            }

            acUnitList.Add(asCalculationData[num]);
        }

        if (acUnitList.m_BufferSize > 0)
        {
            cSendAPI.calculation_data = acUnitList.ToArray();
        }

        // シーケンスID設定
        if (nSeqID == 0)
        {
            cSendAPI.seq_id = (int)cSendAPI.header.local_time;
        }
        else
        {
            cSendAPI.seq_id = nSeqID;
        }

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_DEBUG_GROWTH_CURVE,
                                JsonMapper.ToJson(cSendAPI),
                                cSendAPI.header.packet_unique_id);
#else
		return null;
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：デバッグ機能関連：ユニット取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_DebugUnitGet(PacketStructUnitGetDebug[] asGetUnit)
    {
#if BUILD_TYPE_DEBUG
        //--------------------------------
        // データ構築
        //--------------------------------
        SendDebugUnitGet cSendAPI = new SendDebugUnitGet();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.get_unit = null;

        //--------------------------------
        // 無駄データが挟まらないようにユニットが入力されてる部分だけのバッファを作る
        //--------------------------------
        TemplateList<PacketStructUnitGetDebug> acUnitList = new TemplateList<PacketStructUnitGetDebug>();
        for (int i = 0; i < asGetUnit.Length; i++)
        {
            if (asGetUnit[i] == null)
                continue;
            acUnitList.Add(asGetUnit[i]);
        }
        if (acUnitList.m_BufferSize > 0)
        {
            cSendAPI.get_unit = acUnitList.ToArray();
        }

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_DEBUG_UNIT_GET
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
#else
		return null;
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：デバッグ機能関連：クエストクリア情報改変
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_DebugQuestClear(int nFlag)
    {
#if BUILD_TYPE_DEBUG
        //--------------------------------
        // データ構築
        //--------------------------------
        SendDebugQuestClear cSendAPI = new SendDebugQuestClear();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.flag = nFlag;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_DEBUG_QUEST_CLEAR
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
#else
		return null;
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：デバッグ機能関連：バトルログアップロード
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_DebugBattleLog(int type, string log)
    {
#if BUILD_TYPE_DEBUG
        //--------------------------------
        // データ構築
        //--------------------------------
        SendDebugBattleLog cSendAPI = new SendDebugBattleLog();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.uuid = LocalSaveManager.Instance.LoadFuncUUID();
        cSendAPI.log_type = type;
        cSendAPI.log_data = log;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_DEBUG_SEND_BATTLE_LOG
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
#else
		return null;
#endif
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：ホームページのトピック : ニュース情報取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetTopicInfo()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetTopicInfo cSendAPI = new SendGetTopicInfo();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GET_TOPIC_INFO
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：プレゼント関連：プレゼント開封ログ取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetPresentOpenLog()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetPresentOpenLog cSendAPI = new SendGetPresentOpenLog();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GET_PRESENT_OPEN_LOG
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：スコア関連：ユーザースコア情報取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetUserScoreInfo(int[] event_ids)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetUserScoreInfo cSendAPI = new SendGetUserScoreInfo();
        cSendAPI.header = CreateStructHeader();
        cSendAPI.event_ids = event_ids;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GET_USER_SCORE_INFO
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：スコア関連：スコア報酬取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetScoreReward(int event_id, int[] reward_ids)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetScoreReward cSendAPI = new SendGetScoreReward();
        cSendAPI.header = CreateStructHeader();
        cSendAPI.event_id = event_id;
        cSendAPI.reward_id_list = reward_ids;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GET_SCORE_REWARD
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：成長ボス関連：成長ボス情報取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetChallengeInfo(int[] event_ids)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetChallengeInfo cSendAPI = new SendGetChallengeInfo();
        cSendAPI.header = CreateStructHeader();
        cSendAPI.event_ids = event_ids;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GET_CHALLENGE_INFO
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：成長ボス関連：成長ボス報酬取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_GetChallengeReward(int event_id, int[] reward_ids, int[] loop_cnts)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendGetChallengeReward cSendAPI = new SendGetChallengeReward();
        cSendAPI.header = CreateStructHeader();
        cSendAPI.event_id = event_id;
        cSendAPI.reward_id_list = reward_ids;
        cSendAPI.loop_cnt_list = loop_cnts;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_GET_CHALLENGE_REWARD
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：成長ボス関連：クエスト開始
	*/
    /*
	 * リザルト：RecvQuestStart
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_ChallengeQuestStart(
        uint unQuestID
    , uint unLevel
    , bool bSkip
    , uint unQuestState
    , uint unHelperUserID
    , PacketStructUnit sHelperUnit
    , bool bHelperPointActive
    , int nPartyCurrent
    , uint unEventFP
    , int nBeginnerBoostID
    , uint[] aunareaAmendID
    , bool is_auto_play
    , PacketStructUnit sHelperLinkUnit = null
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendChallengeQuestStart cSendAPI = new SendChallengeQuestStart();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = unQuestID;
        cSendAPI.level = unLevel;
        cSendAPI.skip = bSkip;
        cSendAPI.quest_state = unQuestState;
        cSendAPI.helper_unit = sHelperUnit;
        cSendAPI.helper_user_id = unHelperUserID;
        cSendAPI.helper_point_ok = (bHelperPointActive == true) ? 1 : 0;
        cSendAPI.party_current = nPartyCurrent;
        cSendAPI.event_friendpoint = unEventFP;
        cSendAPI.beginner_boost_id = nBeginnerBoostID;
        cSendAPI.area_amend_id = aunareaAmendID;
        cSendAPI.is_auto_play = is_auto_play;
        cSendAPI.helper_unit_link = sHelperLinkUnit;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_CHALLENGE_QUEST_START
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：成長ボス関連：クエストクリア
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_ChallengeQuestClear(
        uint unQuestID,
        SendEnemyKill[] enemy_kill_list,
        bool no_damage,
        int max_damag,
        SendSkillExecCount[] active_skill_execute_count,
        int hero_skill_execute_count,
        PlayScoreInfo play_score_info,
        bool is_auto_play
    )
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendChallengeQuestClear cSendAPI = new SendChallengeQuestClear();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = unQuestID;
        cSendAPI.enemy_kill_list = enemy_kill_list;
        cSendAPI.no_damage = no_damage;
        cSendAPI.max_damage = max_damag;
        cSendAPI.active_skill_execute_count = active_skill_execute_count;
        cSendAPI.hero_skill_execute_count = hero_skill_execute_count;
        cSendAPI.play_score_info = play_score_info;
        cSendAPI.is_auto_play = is_auto_play;

#if false
        //--------------------------------
        // チート対策のために適当なセキュリティキー情報を送る。
        //
        // 同じ計算式を使ってサーバー上でキーを作成して比較することで、
        // 単純なパケット捏造によるユニットの付与が行われても弾けるようにしていく
        //--------------------------------
        cSendAPI.security_key = CreateSecurityKey_QuestClear(unQuestID, unGetExp, unGetMoney, cSendAPI.get_unit);
#endif

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_CHALLENGE_QUEST_CLEAR
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：成長ボス関連：クエストリタイア
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_ChallengeQuestRetire(uint unQuestID, bool is_auto_play)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuestRetire cSendAPI = new SendQuestRetire();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.quest_id = unQuestID;
        cSendAPI.is_auto_play = is_auto_play;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_CHALLENGE_QUEST_RETIRE
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：成長ボス関連：クエスト受託情報取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_ChallengeQuestOrderGet(int nDetailFlag)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuest2OrderGet cSendAPI = new SendQuest2OrderGet();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.detail_flag = nDetailFlag;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_CHALLENGE_QUEST_ORDER_GET
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：成長ボス関連：コンティニュー
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_ChallengeQuestContinue(uint unContinueCt, bool is_auto_play)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuestContinue cSendAPI = new SendQuestContinue();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.continue_ct = unContinueCt;
        cSendAPI.is_auto_play = is_auto_play;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                                SERVER_API.SERVER_API_CHALLENGE_QUEST_CONTINUE
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：成長ボス関連：クエスト中断データ破棄通知
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_ChallengeQuestCessaionQuest(bool is_auto_play)
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendQuest2CessaionQuest cSendAPI = new SendQuest2CessaionQuest();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.is_auto_play = is_auto_play;

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                              SERVER_API.SERVER_API_CHALLENGE_QUEST_CESSAION_QUEST
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：日付変更情報取得
	*/
    /*
	 * リザルト：
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_DayStraddle()
    {
        //--------------------------------
        // データ構築
        //--------------------------------
        SendDayStraddle cSendAPI = new SendDayStraddle();

        cSendAPI.header = CreateStructHeader();

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                              SERVER_API.SERVER_API_DAY_STRADDLE
                            , JsonMapper.ToJson(cSendAPI)
                            , cSendAPI.header.packet_unique_id
                            );
    }

#if false
	//----------------------------------------------------------------------------
	/*!
		@brief	APIパケット送信：プレゼント関連：プレゼント送信
	*/
	//----------------------------------------------------------------------------
	static	public uint SendPacketAPI_PresentThrow(
		long	unThrowUnitUniqueID
	,	uint	unThrowUserID
	)
	{
		//--------------------------------
		// データ構築
		//--------------------------------
		SendPresentThrow cSendAPI = new SendPresentThrow();

		cSendAPI.header				= CreateStructHeader();
		cSendAPI.unit_unique_id		= unThrowUnitUniqueID;
		cSendAPI.user_id			= unThrowUserID;

		//--------------------------------
		// API送信リクエスト発行
		//--------------------------------
		return ServerDataManager.Instance.AddCommunicateRequest(
								SERVER_API.SERVER_API_PRESENT_THROW
							,	JsonMapper.ToJson( cSendAPI )
							,	cSendAPI.header.packet_unique_id
							);
	}
#endif

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信：チュートリアルスキップ関連：送信
	*/
    /*
	 * リザルト：RecvSkipTutorial
	 */
    //----------------------------------------------------------------------------
    static public ServerApi SendPacketAPI_SkipTutorial(uint unSelectPartyNum)
    {
        //--------------------------------
        //	データ構築
        //--------------------------------
        SendSkipTutorial cSendAPI = new SendSkipTutorial();

        cSendAPI.header = CreateStructHeader();
        cSendAPI.select_party = unSelectPartyNum;


        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerApi.Create(
                        SERVER_API.SERVER_API_TUTORIAL_SKIP,
                        JsonMapper.ToJson(cSendAPI),
                        cSendAPI.header.packet_unique_id
                        );
    }

    //============================================================================
    //============================================================================
    //============================================================================
    //============================================================================
    //============================================================================


    //----------------------------------------------------------------------------
    /*!
		@brief	送信パケット固定情報構築：ヘッダ
	*/
    //----------------------------------------------------------------------------
    static public PacketStructHeaderSend CreateStructHeader()
    {
        PacketStructHeaderSend cHeader = new PacketStructHeaderSend();


        {
            cHeader.api_version = GlobalDefine.GetApiVersion();
        }

        //--------------------------------
        // アプリ再起動後でIDが重複することで不具合が生じる可能性があるため
        // パケットユニークIDはインストール後からの通算を送るように対応。
        //--------------------------------
        cHeader.packet_unique_id = LocalSaveManager.Instance.LoadFuncPacketUniqueID();
        if (cHeader.packet_unique_id >= 0x3FFFFFFF)
        {
            LocalSaveManager.Instance.SaveFuncPacketUniqueID(1);
        }
        else
        {
            LocalSaveManager.Instance.SaveFuncPacketUniqueID(cHeader.packet_unique_id + 1);
        }

        cHeader.terminal = CreateStructTerminal();                               // v2.4.0対応 ヘッダーに端末情報付与

        // UserDataAdminは端末未保存なので保証できない(サーバ側はrank=0値の時DB検索で補完).
        cHeader.rank = UserDataAdmin.Instance.SendServerPacketStructPlayerRunk();  // v2.4.0対応 サーバログ用 DB検索回数を減らす対応.

        cHeader.ad_id = "";
        cHeader.ad_flag = 0;

        //--------------------------------
        // 端末時間を取得
        // @add Developer 2016/04/08 v340 端末時間信用機能対応
        //--------------------------------
#if UNITY_EDITOR || UNITY_STANDALONE
        cHeader.local_time = TimeUtil.ConvertLocalTimeToServerTime(TimeManager.Instance.m_TimeNow);
#else
		cHeader.local_time = TimeUtil.ConvertLocalTimeToServerTime( DateTime.Now );
#endif

        return cHeader;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	送信パケット固定情報構築：端末情報
	*/
    //----------------------------------------------------------------------------
    static private PacketStructTerminal CreateStructTerminal()
    {
        PacketStructTerminal cTerminal = new PacketStructTerminal();

#if UNITY_IPHONE
        cTerminal.platform = GlobalDefine.PLATFORM_IOS;
        cTerminal.name = UnityEngine.iOS.Device.generation.ToString();
        //		iOSUtil deeleted. Developer
        //      cTerminal.name      = iOSUtil.GetDeviceGeneration().ToString("g");      // 2014/12/24 iPhone6世代が取得できず Developerプラグインを使う
        cTerminal.os = SystemInfo.operatingSystem;
#elif UNITY_ANDROID
        cTerminal.platform = GlobalDefine.PLATFORM_ANDROID;
        cTerminal.name = SystemInfo.deviceModel;
        cTerminal.os = SystemInfo.operatingSystem;
#else
		cTerminal.platform	= GlobalDefine.PLATFORM_xxx;
		cTerminal.name		= SystemInfo.deviceModel;
		cTerminal.os		= SystemInfo.operatingSystem;
#endif

        return cTerminal;
    }

#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG || BUILD_HTTPS_SELECT
    static public bool GetSecureServer(ApiEnv env)
    {
        bool secure = false;

        switch (env)
        {
            case ApiEnv.API_TEST_ADDRESS_ONLINE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_REHEARSAL_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_REVIEW_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_0_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_STAGING_1_NEW_GOE:
            case ApiEnv.API_TEST_ADDRESS_DEVELOP_1_NEW_GOE:
                secure = true;
                break;
            default:
                secure = false;
                break;
        }

        return secure;
    }
#endif

    //----------------------------------------------------------------------------
    /*!
		@brief	サーバーアドレス生成
	*/
    //----------------------------------------------------------------------------
    static public string CreateAddress(SERVER_API ePacketAPI, string strAddressIP)
    {
        //----------------------------------------
        // 各APIに対応したPHPの名称を取得
        //----------------------------------------
        string strPHPAddress = "";
        switch (ePacketAPI)
        {
            case SERVER_API.SERVER_API_USER_CREATE: strPHPAddress += "create_user"; break;  // APIタイプ：ユーザー管理：ユーザー新規生成
            case SERVER_API.SERVER_API_USER_AUTHENTICATION: strPHPAddress += "auth_user"; break;    // APIタイプ：ユーザー管理：ユーザー承認
            case SERVER_API.SERVER_API_USER_RENAME: strPHPAddress += "change_user_name"; break; // APIタイプ：ユーザー管理：ユーザー名称変更
            case SERVER_API.SERVER_API_USER_SELECT_DEF_PARTY: strPHPAddress += "renew_user_initial_setting"; break; // APIタイプ：ユーザー管理：ユーザー初期パーティ設定
            case SERVER_API.SERVER_API_USER_RENEW: strPHPAddress += "user_renew"; break;    // APIタイプ：ユーザー管理：ユーザー再構築
            case SERVER_API.SERVER_API_USER_RENEW_CHECK: strPHPAddress += "user_renew_check"; break;	// APIタイプ：ユーザー管理：ユーザー再構築情報問い合わせ
            case SERVER_API.SERVER_API_MASTER_HASH_GET: strPHPAddress += "get_master_hash"; break;  // APIタイプ：マスターデータ操作：マスターデータハッシュ取得
                                                                                                    //			case SERVER_API.SERVER_API_MASTER_DATA_GET:			strPHPAddress += "xxxxxxxxxxxxxxxxxxxxxxxxxx"	;	break;	// APIタイプ：マスターデータ操作：マスターデータ実体取得
            case SERVER_API.SERVER_API_MASTER_DATA_GET_ALL: strPHPAddress += "get_master"; break;   // APIタイプ：マスターデータ操作：マスターデータ実体取得（全種）
            case SERVER_API.SERVER_API_MASTER_DATA_GET_ALL2: strPHPAddress += "get_master2"; break; // APIタイプ：マスターデータ操作：マスターデータ実体取得（差分）
            case SERVER_API.SERVER_API_GET_ACHIEVEMENT_GP: strPHPAddress += "get_mission_gp_data"; break;   // APIタイプ：アチーブメント操作：アチーブメントグループ実体取得
            case SERVER_API.SERVER_API_GET_ACHIEVEMENT: strPHPAddress += "get_mission_data"; break; // APIタイプ：アチーブメント操作：アチーブメント実体取得
            case SERVER_API.SERVER_API_GET_LOGIN_PACK: strPHPAddress += "login_pack"; break;    // APIタイプ：ログイン情報：ログインパック情報取得
            case SERVER_API.SERVER_API_GET_LOGIN_PARAM: strPHPAddress += "login"; break;    // APIタイプ：ログイン情報：ログイン情報取得
                                                                                            //			case SERVER_API.SERVER_API_GET_PLAYER_PARAM:		strPHPAddress += "xxxxxxxxxxxxxxxxxxxxxxxxxx"	;	break;	// APIタイプ：プレイヤー情報：プレイヤー基本情報取得
            case SERVER_API.SERVER_API_FRIEND_LIST_GET: strPHPAddress += "get_friends"; break;  // APIタイプ：フレンド操作：フレンド一覧取得
            case SERVER_API.SERVER_API_FRIEND_REQUEST: strPHPAddress += "send_friend_request"; break;   // APIタイプ：フレンド操作：フレンド申請
            case SERVER_API.SERVER_API_FRIEND_CONSENT: strPHPAddress += "accept_friend_request"; break; // APIタイプ：フレンド操作：フレンド申請承認
            case SERVER_API.SERVER_API_FRIEND_REFUSAL: strPHPAddress += "delete_friend"; break; // APIタイプ：フレンド操作：フレンド解除
            case SERVER_API.SERVER_API_FRIEND_SEARCH: strPHPAddress += "select_user_code"; break;   // APIタイプ：フレンド操作：フレンドユーザー検索
            case SERVER_API.SERVER_API_UNIT_PARTY_ASSIGN: strPHPAddress += "form_units"; break; // APIタイプ：ユニット操作：ユニットパーティ編成設定
            case SERVER_API.SERVER_API_UNIT_SALE: strPHPAddress += "sale_unit"; break;  // APIタイプ：ユニット操作：ユニット売却
            case SERVER_API.SERVER_API_UNIT_BLEND_BUILDUP: strPHPAddress += "compose_unit"; break;  // APIタイプ：ユニット操作：ユニット強化合成
            case SERVER_API.SERVER_API_UNIT_BLEND_EVOL: strPHPAddress += "evolve_unit"; break;  // APIタイプ：ユニット操作：ユニット進化合成
            case SERVER_API.SERVER_API_UNIT_LINK_CREATE: strPHPAddress += "create_unit_link"; break;    // APIタイプ：ユニット操作：ユニットリンク実行
            case SERVER_API.SERVER_API_UNIT_LINK_DELETE: strPHPAddress += "delete_unit_link"; break;    // APIタイプ：ユニット操作：ユニットリンク解除
            case SERVER_API.SERVER_API_QUEST_HELPER_GET: strPHPAddress += "get_helpers"; break; // APIタイプ：クエスト操作：助っ人一覧取得
            case SERVER_API.SERVER_API_QUEST_HELPER_GET_EVOL: strPHPAddress += "get_helpers_evol"; break;   // APIタイプ：クエスト操作：助っ人一覧取得（進化合成向け）
            case SERVER_API.SERVER_API_QUEST_HELPER_GET_BUILD: strPHPAddress += "get_helpers_compose"; break;   // APIタイプ：クエスト操作：助っ人一覧取得（強化合成向け）
            case SERVER_API.SERVER_API_QUEST_START: strPHPAddress += "order_quest"; break;  // APIタイプ：クエスト操作：クエスト開始
            case SERVER_API.SERVER_API_QUEST_CLEAR: strPHPAddress += "clear_quest"; break;  // APIタイプ：クエスト操作：クエストクリア
            case SERVER_API.SERVER_API_QUEST_RETIRE: strPHPAddress += "renew_retire_quest"; break;  // APIタイプ：クエスト操作：クエストリタイア
            case SERVER_API.SERVER_API_QUEST_ORDER_GET: strPHPAddress += "get_quest_info"; break;   // APIタイプ：クエスト操作：クエスト受託情報取得
            case SERVER_API.SERVER_API_QUEST_CONTINUE: strPHPAddress += "renew_continue_quest"; break;  // APIタイプ：インゲーム中：コンティニュー
            case SERVER_API.SERVER_API_QUEST_RESET: strPHPAddress += "reset_quest"; break;  // APIタイプ：インゲーム中：リセット
            case SERVER_API.SERVER_API_EVOL_QUEST_START: strPHPAddress += "order_quest_evol"; break;    // APIタイプ：クエスト操作：進化クエスト開始
            case SERVER_API.SERVER_API_EVOL_QUEST_CLEAR: strPHPAddress += "clear_quest_evol"; break;  // APIタイプ：クエスト操作：進化クエストクリア

            case SERVER_API.SERVER_API_QUEST2_START: strPHPAddress += "renew_order_quest"; break;   // APIタイプ：クエスト操作：新クエスト開始
            case SERVER_API.SERVER_API_QUEST2_START_TUTORIAL: strPHPAddress += "renew_tutorial_order_quest"; break; // APIタイプ：クエスト操作：新クエスト開始（チュートリアル）
            case SERVER_API.SERVER_API_QUEST2_CLEAR: strPHPAddress += "renew_clear_quest"; break;  // APIタイプ：クエスト操作：新クエストクリア
            case SERVER_API.SERVER_API_QUEST2_ORDER_GET: strPHPAddress += "get_renew_quest_info"; break;  // APIタイプ：クエスト操作：新クエスト受託情報取得
            case SERVER_API.SERVER_API_QUEST2_STORY_START: strPHPAddress += "renew_order_story_quest"; break;   // APIタイプ：クエスト操作：ストーリクエスト開始
            case SERVER_API.SERVER_API_QUEST2_STORY_CLEAR: strPHPAddress += "renew_clear_story_quest"; break;   // APIタイプ：クエスト操作：ストーリクエストクリア
            case SERVER_API.SERVER_API_QUEST2_CESSAION_QUEST: strPHPAddress += "renew_cessation_quest"; break;  // APIタイプ：クエスト操作：新クエスト中断データ破棄通知

            case SERVER_API.SERVER_API_INJUSTICE: strPHPAddress += "injustice"; break;  // APIタイプ：不正検出関連：不正検出送信
            case SERVER_API.SERVER_API_TUTORIAL: strPHPAddress += "tutorial"; break;    // APIタイプ：チュートリアル関連：進行集計
            case SERVER_API.SERVER_API_STONE_USE_UNIT: strPHPAddress += "buy_max_unit"; break;  // APIタイプ：魔法石使用：ユニット枠増設
            case SERVER_API.SERVER_API_STONE_USE_FRIEND: strPHPAddress += "buy_max_friend"; break;  // APIタイプ：魔法石使用：フレンド枠増設
                                                                                                    //			case SERVER_API.SERVER_API_STONE_USE_PARTY_COST:	strPHPAddress += "buy_max_cost"					;	break;	// APIタイプ：魔法石使用：パーティコスト増加
            case SERVER_API.SERVER_API_STONE_USE_STAMINA: strPHPAddress += "buy_recover_ap"; break; // APIタイプ：魔法石使用：スタミナ回復
            case SERVER_API.SERVER_API_GACHA_PLAY: strPHPAddress += "buy_gacha"; break; // APIタイプ：ガチャ操作：ガチャ実行
            case SERVER_API.SERVER_API_GACHA_TICKET_PLAY: strPHPAddress += "buy_gacha_ticket"; break;   // APIタイプ：ガチャ操作：ガチャチケット実行
            case SERVER_API.SERVER_API_STONE_PAY_PREV_IOS: strPHPAddress += "app_store_purchase_begin"; break;  // APIタイプ：課金操作：魔法石購入直前処理( iOS … AppStore )
            case SERVER_API.SERVER_API_STONE_PAY_PREV_ANDROID: strPHPAddress += "google_play_bill_begin"; break;    // APIタイプ：課金操作：魔法石購入直前処理( Android … GooglePlay )
            case SERVER_API.SERVER_API_STONE_PAY_IOS: strPHPAddress += "app_store_purchase_point"; break;   // APIタイプ：課金操作：魔法石購入反映処理( iOS … AppStore )
            case SERVER_API.SERVER_API_STONE_PAY_ANDROID: strPHPAddress += "google_play_bill_point"; break; // APIタイプ：課金操作：魔法石購入反映処理( Android … GooglePlay )
            case SERVER_API.SERVER_API_REVIEW_PRESENT: strPHPAddress += "post_review_present"; break;   // APIタイプ：ユーザーレビュー関連：レビュー遷移報酬
            case SERVER_API.SERVER_API_PRESENT_LIST_GET: strPHPAddress += "get_presents"; break;    // APIタイプ：プレゼント関連：プレゼント一覧取得
            case SERVER_API.SERVER_API_PRESENT_OPEN: strPHPAddress += "open_present"; break;    // APIタイプ：プレゼント関連：プレゼント開封
                                                                                                //			case SERVER_API.SERVER_API_PRESENT_THROW:			strPHPAddress += "send_present" 				;	break;	// APIタイプ：プレゼント関連：プレゼント送信

            case SERVER_API.SERVER_API_TRANSFER_ORDER: strPHPAddress += "make_password"; break; // APIタイプ：セーブ移行関連：パスワード発行
            case SERVER_API.SERVER_API_TRANSFER_EXEC: strPHPAddress += "move_save_data"; break; // APIタイプ：セーブ移行関連：移行実行

#if BUILD_TYPE_DEBUG
            case SERVER_API.SERVER_API_DEBUG_EDIT_USER: strPHPAddress += "debug_edit_user"; break;  // APIタイプ：デバッグ機能関連：ユーザーデータ更新
            case SERVER_API.SERVER_API_DEBUG_UNIT_GET: strPHPAddress += "debug_add_units"; break;   // APIタイプ：デバッグ機能関連：ユニット取得
            case SERVER_API.SERVER_API_DEBUG_GROWTH_CURVE: strPHPAddress += "debug_calculation_data"; break;	// APIタイプ：デバッグ機能関連：成長曲線検証
            case SERVER_API.SERVER_API_DEBUG_QUEST_CLEAR: strPHPAddress += "aaaaaaaaaaaaaa"; break; // APIタイプ：デバッグ機能関連：クエストクリア情報改変
            case SERVER_API.SERVER_API_DEBUG_SEND_BATTLE_LOG: strPHPAddress += "post_log"; break;   // APIタイプ：デバッグ機能関連：ユーザーデータ更新
            case SERVER_API.SERVER_API_DEBUG_MASTER_DATA_GET_ALL2: strPHPAddress += "debug_get_master2"; break; // APIタイプ：マスターデータ操作：マスターデータ実体取得（差分）
#endif

            case SERVER_API.SERVER_API_GET_STORE_EVENT: strPHPAddress += "get_store_product_event"; break;  // APIタイプ：イベントストア一覧取得
            case SERVER_API.SERVER_API_STORE_PAY_CANCEL: strPHPAddress += "purchase_cancel"; break; // APIタイプ：課金操作：魔法石購入キャンセル

            case SERVER_API.SERVER_API_ACHIEVEMENT_OPEN: strPHPAddress += "get_achievement"; break; // APIタイプ：アチーブメント開封

            case SERVER_API.SERVER_API_CHECK_SNS_LINK: strPHPAddress += "check_sns_link"; break;    //!< APIタイプ：SNSIDとの紐付け確認
            case SERVER_API.SERVER_API_SET_SNS_LINK: strPHPAddress += "set_sns_link"; break;    //!< APIタイプ：SNSIDとの紐付け
            case SERVER_API.SERVER_API_MOVE_SNS_SAVE_DATA: strPHPAddress += "move_sns_save_data"; break;    //!< APIタイプ：SNSIDを使用したデータ移行

            case SERVER_API.SERVER_API_TUTORIAL_SKIP: strPHPAddress += "debug_skip_renew_tutorial"; break;  //!< APIタイプ：チュートリアルスキップ

            case SERVER_API.SERVER_API_GET_SNS_ID: strPHPAddress += "get_snsid"; break; //!< APIタイプ：SNSID作成

            case SERVER_API.SERVER_API_GET_POINT_SHOP_PRODUCT: strPHPAddress += "get_point_shop_product"; break;    //!< APIタイプ：ポイントショップ：ショップ商品情報を取得		get_point_shop_product
            case SERVER_API.SERVER_API_POINT_SHOP_PURCHASE: strPHPAddress += "point_shop_purchase"; break;  //!< APIタイプ：ポイントショップ：商品購入					point_shop_purchase
            case SERVER_API.SERVER_API_POINT_SHOP_LIMITOVER: strPHPAddress += "point_shop_limit_over"; break;   //!< APIタイプ：ポイントショップ：限界突破
            case SERVER_API.SERVER_API_POINT_SHOP_EVOL: strPHPAddress += "point_shop_evolve"; break; //!< APIタイプ：ポイントショップ：進化

            case SERVER_API.SERVER_API_USE_ITEM: strPHPAddress += "use_item"; break;    //!< APIタイプ：消費アイテム関連：アイテム使用

            case SERVER_API.SERVER_API_GET_BOX_GACHA_STOCK: strPHPAddress += "get_box_gacha_stock"; break;  //!< APIタイプ：BOXガチャ在庫状況取得
            case SERVER_API.SERVER_API_RESET_BOX_GACHA_STOCK: strPHPAddress += "reset_box_gacha_stock"; break;	//!< APIタイプ：BOXガチャ在庫状況リセット
            case SERVER_API.SERVER_API_SET_CURRENT_HERO: strPHPAddress += "set_current_hero"; break;  // APIタイプ：主人公選択：主人公選択
            case SERVER_API.SERVER_API_EVOLVE_UNIT: strPHPAddress += "evolve_unit"; break;  // APIタイプ：進化実行：進化実行
            case SERVER_API.SERVER_API_GET_GUERRILLA_BOSS_INFO: strPHPAddress += "get_guerrilla_boss_info"; break;  // APIタイプ：ゲリラボス情報取得
            case SERVER_API.SERVER_API_GET_GACHA_LINEUP: strPHPAddress += "get_gacha_lineup"; break;  // APIタイプ：ガチャ：ガチャラインナップ詳細
            case SERVER_API.SERVER_API_RENEW_TUTORIAL: strPHPAddress += "renew_tutorial"; break;	// APIタイプ：チュートリアル関連：進行集計
            case SERVER_API.SERVER_API_GET_TOPIC_INFO: strPHPAddress += "get_topic_info"; break;	// APIタイプ：ホームページのトピック : ニュース情報取得
            case SERVER_API.SERVER_API_GET_PRESENT_OPEN_LOG: strPHPAddress += "get_present_open_log"; break;    // APIタイプ：プレゼント関連 : プレゼント開封ログ取得
            case SERVER_API.SERVER_API_PERIODIC_UPDATE: strPHPAddress += "periodic_update"; break;	//!< APIタイプ：定期データ更新(デバイストークン)
            case SERVER_API.SERVER_API_GET_USER_SCORE_INFO: strPHPAddress += "get_user_score"; break;    //!< APIタイプ：ユーザースコア情報取得
            case SERVER_API.SERVER_API_GET_SCORE_REWARD: strPHPAddress += "get_score_reward"; break;    //!< APIタイプ：スコア報酬取得

            case SERVER_API.SERVER_API_GET_CHALLENGE_INFO: strPHPAddress += "get_user_challenge"; break;                    //!< APIタイプ：成長ボスイベント情報取得
            case SERVER_API.SERVER_API_GET_CHALLENGE_REWARD: strPHPAddress += "get_challenge_reward"; break;                //!< APIタイプ：成長ボスイベント報酬取得
            case SERVER_API.SERVER_API_CHALLENGE_QUEST_START: strPHPAddress += "challenge_order_quest"; break;              //!< APIタイプ：クエスト操作：成長ボスクエスト開始
            case SERVER_API.SERVER_API_CHALLENGE_QUEST_CLEAR: strPHPAddress += "challenge_clear_quest"; break;              //!< APIタイプ：クエスト操作：成長ボスクエストクリア
            case SERVER_API.SERVER_API_CHALLENGE_QUEST_ORDER_GET: strPHPAddress += "get_challenge_quest_info"; break;       //!< APIタイプ：クエスト操作：成長ボスクエスト受託情報取得
            case SERVER_API.SERVER_API_CHALLENGE_QUEST_CESSAION_QUEST: strPHPAddress += "challenge_cessation_quest"; break; //!< APIタイプ：クエスト操作：成長ボスクエスト中断データ破棄通知
            case SERVER_API.SERVER_API_CHALLENGE_QUEST_CONTINUE: strPHPAddress += "challenge_continue_quest"; break;        //!< APIタイプ：クエスト操作：コンティニュー
            case SERVER_API.SERVER_API_CHALLENGE_QUEST_RETIRE: strPHPAddress += "challenge_retire_quest"; break;            //!< APIタイプ：クエスト操作：クエストリタイア

            case SERVER_API.SERVER_API_DAY_STRADDLE: strPHPAddress += "day_straddle"; break;            //!< APIタイプ：日付変更情報取得
        }

        bool bSecurity = false;

#if API_SELECT_DEBUG || BUILD_TYPE_DEBUG || BUILD_HTTPS_SELECT
        bSecurity = GetSecureServer(GlobalDefine.GetApiEnv());
#else
		bSecurity = true;
#endif

        //----------------------------------------
        // セキュリティの有無で [ http ][ https ] を使い分ける
        //----------------------------------------
        string strServerAddress = "";
        if (bSecurity == true)
        {
            strServerAddress = StrOpe.i + "https://" + strAddressIP + "/pqdm/" + GlobalDefine.GetApiVersion() + "/api/" + strPHPAddress + ".php";
        }
        else
        {
            strServerAddress = StrOpe.i + "http://" + strAddressIP + "/pqdm/" + GlobalDefine.GetApiVersion() + "/api/" + strPHPAddress + ".php";
        }

        return strServerAddress;
    }
}
/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
