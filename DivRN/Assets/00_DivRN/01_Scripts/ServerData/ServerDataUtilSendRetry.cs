/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	ServerDataUtilSendRetry.cs
	@brief	サーバー通信関連ユーティリティ：送信リトライ処理特化
	@author Developer
	@date 	2013/02/07
 
	パケット送信時にユーザー再認証が必要になったケースの対応として、
	文字列をパケットに直して一部情報を書き換えて発行しなおすことに特化したクラス。
	
	主にServerDataManagerからの再送処理発生時のイレギュラー対応を想定
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

using LitJson;

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
	@brief	サーバー通信関連ユーティリティ：送信リトライ処理特化
*/
//----------------------------------------------------------------------------
static public class ServerDataUtilSendRetry
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /*==========================================================================*/
    /*		prototype															*/
    /*==========================================================================*/
    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット送信リトライ：
		@note	この処理は通信の結果「端末とサーバーの関連切れてるからユーザー認証しなおしてね」が発生した際に送りなおすことに特化している。
				再送元のパケットは一度サーバーに届いて正式に拒否られているので、パケットユニークIDを別にしてから再度送りなおすことで受理を期待する
	*/
    //----------------------------------------------------------------------------
    static public uint SendPacketAPIRetry(SERVER_API eAPIType, string strAPIData, uint unPacketUniqueID)
    {

#if true
        //-------------------------
        // 再送時にパケットにユニークIDを変えない方がサーバー側が対応しやすいらしい。
        // そのまま前に送ったものをもう一度送る形で対応する
        //-------------------------
        Debug.LogError("Packet Retry - " + eAPIType);
#else
		//-------------------------
		// 再送時にはヘッダのパケットユニークIDだけを更新してから送る
		//-------------------------
		PacketStructHeaderSend cSendHeader = ServerDataUtilSend.CreateStructHeader();
		switch( eAPIType )
		{
			//-------------------------
			// APIタイプ：ユーザー管理：ユーザー新規生成
			//-------------------------
			case SERVER_API.SERVER_API_USER_CREATE:
				{
					SendCreateUser cSendPacket = JsonMapper.ToObject< SendCreateUser >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：ユーザー管理：ユーザー承認													
			//-------------------------
			case SERVER_API.SERVER_API_USER_AUTHENTICATION:
				{
					SendUserAuthentication cSendPacket = JsonMapper.ToObject< SendUserAuthentication >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：ユーザー管理：ユーザー名称変更														
			//-------------------------
			case SERVER_API.SERVER_API_USER_RENAME:		
				{
					SendRenameUser cSendPacket = JsonMapper.ToObject< SendRenameUser >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：ユーザー管理：ユーザー初期設定												
			//-------------------------
			case SERVER_API.SERVER_API_USER_SELECT_DEF_PARTY:
				{
					SendSelectDefParty cSendPacket = JsonMapper.ToObject< SendSelectDefParty >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：ユーザー管理：ユーザー再構築										
			//-------------------------
			case SERVER_API.SERVER_API_USER_RENEW:		
				{
					SendRenewUser cSendPacket = JsonMapper.ToObject< SendRenewUser >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：マスターデータ操作：マスターデータ実体取得（全種）				
			//-------------------------
			case SERVER_API.SERVER_API_MASTER_DATA_GET_ALL:
				{
					SendGetMasterDataAll cSendPacket = JsonMapper.ToObject< SendGetMasterDataAll >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：ログイン情報：ログインパック情報取得								
			//-------------------------
			case SERVER_API.SERVER_API_GET_LOGIN_PACK:	
				{
					SendLoginPack cSendPacket = JsonMapper.ToObject< SendLoginPack >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：ログイン情報：ログイン情報取得									
			//-------------------------
			case SERVER_API.SERVER_API_GET_LOGIN_PARAM:	
				{
					SendLoginParam cSendPacket = JsonMapper.ToObject< SendLoginParam >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：フレンド操作：フレンド一覧取得													
			//-------------------------
			case SERVER_API.SERVER_API_FRIEND_LIST_GET:	
				{
					SendFriendListGet cSendPacket = JsonMapper.ToObject< SendFriendListGet >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：フレンド操作：フレンド申請														
			//-------------------------
			case SERVER_API.SERVER_API_FRIEND_REQUEST:	
				{
					SendFriendRequest cSendPacket = JsonMapper.ToObject< SendFriendRequest >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;	
			//-------------------------
			// APIタイプ：フレンド操作：フレンド申請承認													
			//-------------------------
			case SERVER_API.SERVER_API_FRIEND_CONSENT:	
				{
					SendFriendConsent cSendPacket = JsonMapper.ToObject< SendFriendConsent >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：フレンド操作：フレンド解除														
			//-------------------------
			case SERVER_API.SERVER_API_FRIEND_REFUSAL:	
				{
					SendFriendRefusal cSendPacket = JsonMapper.ToObject< SendFriendRefusal >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：フレンド操作：フレンドユーザー検索												
			//-------------------------
			case SERVER_API.SERVER_API_FRIEND_SEARCH:	
				{
					SendFriendSearch cSendPacket = JsonMapper.ToObject< SendFriendSearch >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：ユニット操作：ユニットパーティ編成設定											
			//-------------------------
			case SERVER_API.SERVER_API_UNIT_PARTY_ASSIGN:
				{
					SendUnitPartyAssign cSendPacket = JsonMapper.ToObject< SendUnitPartyAssign >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：ユニット操作：ユニット売却															
			//-------------------------
			case SERVER_API.SERVER_API_UNIT_SALE:		
				{
					SendUnitSale cSendPacket = JsonMapper.ToObject< SendUnitSale >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：ユニット操作：ユニット強化合成												
			//-------------------------
			case SERVER_API.SERVER_API_UNIT_BLEND_BUILDUP:
				{
					SendUnitBlendBuildUp cSendPacket = JsonMapper.ToObject< SendUnitBlendBuildUp >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：ユニット操作：ユニット進化合成													
			//-------------------------
			case SERVER_API.SERVER_API_UNIT_BLEND_EVOL:	
				{
					SendUnitBlendEvol cSendPacket = JsonMapper.ToObject< SendUnitBlendEvol >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：クエスト操作：助っ人一覧取得														
			//-------------------------
			case SERVER_API.SERVER_API_QUEST_HELPER_GET:
				{
					SendQuestHelperGet cSendPacket = JsonMapper.ToObject< SendQuestHelperGet >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：クエスト操作：助っ人一覧取得（進化合成用）						
			//-------------------------
			case SERVER_API.SERVER_API_QUEST_HELPER_GET_EVOL:
				{
					SendQuestHelperGetEvol cSendPacket = JsonMapper.ToObject< SendQuestHelperGetEvol >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：クエスト操作：助っ人一覧取得（強化合成用）						
			//-------------------------
			case SERVER_API.SERVER_API_QUEST_HELPER_GET_BUILD:
				{
					SendQuestHelperGetBuild cSendPacket = JsonMapper.ToObject< SendQuestHelperGetBuild >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：クエスト操作：クエスト開始															
			//-------------------------
			case SERVER_API.SERVER_API_QUEST_START:		
				{
					SendQuestStart cSendPacket = JsonMapper.ToObject< SendQuestStart >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;	
			//-------------------------
			// APIタイプ：クエスト操作：クエストクリア													
			//-------------------------
			case SERVER_API.SERVER_API_QUEST_CLEAR:		
				{
					SendQuestClear cSendPacket = JsonMapper.ToObject< SendQuestClear >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------

			//-------------------------
			// APIタイプ：クエスト操作：クエストリタイア									
			//-------------------------
			//-------------------------
			case SERVER_API.SERVER_API_QUEST_RETIRE:	
				{
					SendQuestRetire cSendPacket = JsonMapper.ToObject< SendQuestRetire >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：クエスト操作：クエスト受託情報取得												
			//-------------------------
			case SERVER_API.SERVER_API_QUEST_ORDER_GET:	
				{
					SendQuestOrderGet cSendPacket = JsonMapper.ToObject< SendQuestOrderGet >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：インゲーム中：コンティニュー														
			//-------------------------
			case SERVER_API.SERVER_API_QUEST_CONTINUE:	
				{
					SendQuestContinue cSendPacket = JsonMapper.ToObject< SendQuestContinue >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：インゲーム中：リセット																
			//-------------------------
			case SERVER_API.SERVER_API_QUEST_RESET:		
				{
					SendQuestReset cSendPacket = JsonMapper.ToObject< SendQuestReset >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：クエスト操作：進化クエスト開始									
			//-------------------------
			case SERVER_API.SERVER_API_EVOL_QUEST_START:
				{
					SendEvolQuestStart cSendPacket = JsonMapper.ToObject< SendEvolQuestStart >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：クエスト操作：進化クエストクリア									
			//-------------------------
			case SERVER_API.SERVER_API_EVOL_QUEST_CLEAR:
				{
					SendEvolQuestClear cSendPacket = JsonMapper.ToObject< SendEvolQuestClear >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：不正検出関連：不正検出送信															
			//-------------------------
			case SERVER_API.SERVER_API_INJUSTICE:		
				{
					SendInjustice cSendPacket = JsonMapper.ToObject< SendInjustice >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			//-------------------------
			//-------------------------
			// APIタイプ：チュートリアル関連：進行集計										
			//-------------------------
			case SERVER_API.SERVER_API_TUTORIAL:	
				{
					SendTutorialStep cSendPacket = JsonMapper.ToObject< SendTutorialStep >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;	
			//-------------------------
			// APIタイプ：魔法石使用：ユニット枠増設														
			//-------------------------
			case SERVER_API.SERVER_API_STONE_USE_UNIT:	
				{
					SendStoneUsedUnit cSendPacket = JsonMapper.ToObject< SendStoneUsedUnit >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：魔法石使用：フレンド枠増設														
			//-------------------------
			case SERVER_API.SERVER_API_STONE_USE_FRIEND:
				{
					SendStoneUsedFriend cSendPacket = JsonMapper.ToObject< SendStoneUsedFriend >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：魔法石使用：スタミナ回復														
			//-------------------------
			case SERVER_API.SERVER_API_STONE_USE_STAMINA:
				{
					SendStoneUsedStamina cSendPacket = JsonMapper.ToObject< SendStoneUsedStamina >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：ガチャ操作：ガチャ実行																
			//-------------------------
			case SERVER_API.SERVER_API_GACHA_PLAY:		
				{
					SendGachaPlay cSendPacket = JsonMapper.ToObject< SendGachaPlay >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：課金操作：魔法石購入直前処理( iOS … AppStore )								
			//-------------------------
			case SERVER_API.SERVER_API_STONE_PAY_PREV_IOS:
				{
					SendStorePayPrev_ios cSendPacket = JsonMapper.ToObject< SendStorePayPrev_ios >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：課金操作：魔法石購入直前処理( Android … GooglePlay )						
			//-------------------------
			case SERVER_API.SERVER_API_STONE_PAY_PREV_ANDROID:
				{
					SendStorePayPrev_android cSendPacket = JsonMapper.ToObject< SendStorePayPrev_android >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：課金操作：魔法石購入反映処理( iOS … AppStore )									
			//-------------------------
			case SERVER_API.SERVER_API_STONE_PAY_IOS:	
				{
					SendStorePay_ios cSendPacket = JsonMapper.ToObject< SendStorePay_ios >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：課金操作：魔法石購入反映処理( Android … GooglePlay )							
			//-------------------------
			case SERVER_API.SERVER_API_STONE_PAY_ANDROID:
				{
					SendStorePay_android cSendPacket = JsonMapper.ToObject< SendStorePay_android >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：ユーザーレビュー関連：レビュー遷移報酬											
			//-------------------------
			case SERVER_API.SERVER_API_REVIEW_PRESENT:	
				{
					SendReviewPresent cSendPacket = JsonMapper.ToObject< SendReviewPresent >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：プレゼント関連：プレゼント一覧取得												
			//-------------------------
			case SERVER_API.SERVER_API_PRESENT_LIST_GET:
				{
					SendPresentListGet cSendPacket = JsonMapper.ToObject< SendPresentListGet >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：プレゼント関連：プレゼント開封														
			//-------------------------
			case SERVER_API.SERVER_API_PRESENT_OPEN:	
				{
					SendPresentOpen cSendPacket = JsonMapper.ToObject< SendPresentOpen >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：デバッグ機能関連：ユーザーランクアップ
			//-------------------------
#if BUILD_TYPE_DEBUG
			case SERVER_API.SERVER_API_DEBUG_RANKUP:	
				{
					SendDebugUserRankUp cSendPacket = JsonMapper.ToObject< SendDebugUserRankUp >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
#endif
			//-------------------------
			// APIタイプ：デバッグ機能関連：ユニット取得
			//-------------------------
#if BUILD_TYPE_DEBUG
			case SERVER_API.SERVER_API_DEBUG_UNIT_GET:	
				{
					SendDebugUnitGet cSendPacket = JsonMapper.ToObject< SendDebugUnitGet >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
			//-------------------------
			// APIタイプ：デバッグ機能関連：クエストクリア情報改変
			//-------------------------
#endif
#if BUILD_TYPE_DEBUG
			case SERVER_API.SERVER_API_DEBUG_QUEST_CLEAR:
				{
					SendDebugQuestClear cSendPacket = JsonMapper.ToObject< SendDebugQuestClear >( strAPIData );
					cSendPacket.header = cSendHeader;
					strAPIData = JsonMapper.ToJson( cSendPacket );
				}
				break;
#endif
			//-------------------------
			// 
			//-------------------------
		}
#endif

        //--------------------------------
        // API送信リクエスト発行
        //--------------------------------
        return ServerDataManager.Instance.AddCommunicateRequest(
                                eAPIType
                            , strAPIData
                            , unPacketUniqueID
                            );
    }

};


/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
