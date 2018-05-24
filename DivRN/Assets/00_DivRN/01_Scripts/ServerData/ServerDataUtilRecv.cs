/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	ServerDataUtilRecv.cs
	@brief	サーバー通信関連ユーティリティ：受信処理特化
	@author Developer
	@date 	2013/01/31
	
	送信ユーティリティと受信ユーティリティが同じソース内にあると、物量が膨大になるので分割してるだけ。
 
	外部クラスはこのユーティリティの呼び出しは行わない。
	ここにまとまっているのはAPI受信データ解析処理用ユーティリティのみ
	
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
	@brief	サーバー通信関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class ServerDataUtilRecv
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット受信情報からヘッダ情報を抜き出し
	*/
    //----------------------------------------------------------------------------
    static public RecvHeaderChecker CreateRecvHeader(string strSrcMessage)
    {
        //--------------------------------
        // 最初の構造体の終端を取得
        // ※最初の構造体はヘッダ情報
        //--------------------------------
        if (strSrcMessage[0] != '{')
        {
            return null;
        }

        // ヘッダーにachievement_counterを追加した際にachievement_counter構造体が複数時、"}"の場所では構造体を途中から切り捨てる不具合が発生したので対応
        //int nIndex = strSrcMessage.IndexOf( "}" , 0 );
        int nIndex = strSrcMessage.IndexOf("},\"result\"", 0);
        if (nIndex < 0)
        {
            return null;
        }

        //--------------------------------
        // ヘッダ情報までで文字列を切り離して、構造を終わるように終端を加える
        //--------------------------------
        string strDstMessage = strSrcMessage.Substring(0, nIndex + 1) + "}";

        //--------------------------------
        // ヘッダ情報のみの文字列ができたので構造体化
        //--------------------------------
        return JsonMapper.ToObject<RecvHeaderChecker>(strDstMessage);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	APIパケット受信情報からヘッダ情報を抜き出し
	*/
    //----------------------------------------------------------------------------
    static public RecvMasterDataWideUse CreateRecvHeaderMasterData(string strSrcMessage)
    {
        //--------------------------------
        // 最初の構造体の終端を取得
        // ※最初の構造体はヘッダ情報
        //--------------------------------
        int nIndex0 = strSrcMessage.IndexOf("}", 0);
        if (nIndex0 < 0)
        {
            return null;
        }

        int nIndex1 = strSrcMessage.IndexOf("}", nIndex0 + 1);
        if (nIndex1 < 0)
        {
            return null;
        }

        //--------------------------------
        // ヘッダ情報までで文字列を切り離して、構造を終わるように終端を加える
        //--------------------------------
        string strDstMessage = strSrcMessage.Substring(0, nIndex1 + 1) + "}";

        //--------------------------------
        // ヘッダ情報のみの文字列ができたので構造体化
        //--------------------------------
        return JsonMapper.ToObject<RecvMasterDataWideUse>(strDstMessage);
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	API受信情報解析
	*/
    //----------------------------------------------------------------------------
    static public ResultCodeType RecvPacketAPI(ref PacketData rcPacketData, string strRecvText, string strRecvError, bool bParse = true)
    {
        //--------------------------------
        // 送受信エラーを検出
        //--------------------------------
        if (strRecvError != null)
        {
            //--------------------------------
            // 再送指示。
            // そもそもこの関数に入る前に弾いているのでここに届くことはないはず。
            //--------------------------------
            return ServerDataManager.Instance.ResultCodeToCodeType(rcPacketData.m_PacketAPI, API_CODE.API_CODE_CLIENT_RETRY);
        }

        //--------------------------------
        // ヘッダ情報のみ抜出
        //--------------------------------
        RecvHeaderChecker cPacketHeader = CreateRecvHeader(strRecvText);

        //--------------------------------
        // セッションIDを更新
        // ヘッダが破損していてもデータがあれば適用するので先行して設定
        //--------------------------------
        if (cPacketHeader != null
        && cPacketHeader.header.session_id != null
        && cPacketHeader.header.session_id.Length > 0
        )
        {
            ServerDataParam.m_ServerSessionID = cPacketHeader.header.session_id;
        }

        //--------------------------------
        // サーバーから届いた現在時間を保持
        //--------------------------------
        if (cPacketHeader != null
        && cPacketHeader.header != null
        )
        {
            //--------------------------------
            // 
            //--------------------------------
            if (ServerDataParam.m_ServerTime != 0
            && cPacketHeader.header.server_time == 0
            )
            {
                //--------------------------------
                // サーバーから時間情報が返ってくるケースと返ってこないケースがあると処理が破綻する。
                // 返ってくるAPIと返ってこないAPIがあっても一応動作するようにセーフティを入れておく
                //--------------------------------
                Debug.LogError("ServerTime Error! - " + rcPacketData.m_PacketAPI);
                if (TimeManager.Instance != null)
                {
                    TimeUtil.ConvertLocalTimeToServerTime(TimeManager.Instance.m_TimeNow);
                }
            }
            else
            {
                //--------------------------------
                // サーバーから届いた時間情報を使用
                //--------------------------------
                ServerDataParam.m_ServerTime = cPacketHeader.header.server_time;
            }
        }

        //--------------------------------
        // セキュリティコードを更新
        //--------------------------------
        if (cPacketHeader != null
        && cPacketHeader.header.csum != null
        && cPacketHeader.header.csum.Length > 0
        )
        {
            ServerDataParam.m_ServerAPICheckSum = cPacketHeader.header.csum;
#if BUILD_TYPE_DEBUG
            Debug.Log("API CheckSum - Recv " + ServerDataParam.m_ServerAPICheckSum);
#endif
        }

        if (cPacketHeader != null
        && cPacketHeader.header != null
        )
        {
            ServerDataParam.m_ServerAPICheckRandOtt = cPacketHeader.header.ott;
        }

        //--------------------------------
        // ヘッダ情報はフォーマット固定なのでヘッダの取得を行い不具合がないかチェックする
        //--------------------------------
        if (cPacketHeader == null)
        {
            Debug.LogError("RecvPacket Header Broken! : " + strRecvText);
            return ServerDataManager.Instance.ResultCodeToCodeType(rcPacketData.m_PacketAPI, API_CODE.API_CODE_CLIENT_PARSE_FAILED);
        }

        if (Enum.IsDefined(typeof(API_CODE), cPacketHeader.header.code) == false)
        {
            Debug.LogError("RecvPacket Header Broken! Error Code : " + strRecvText);
            return ServerDataManager.Instance.ResultCodeToCodeType(rcPacketData.m_PacketAPI, API_CODE.API_CODE_CLIENT_PARSE_FAILED_CODE);
        }

        //--------------------------------
        // リクエストに対する返信ではない場合、
        // 再送依頼してもう一度取り直す
        //--------------------------------
        if (rcPacketData.m_PacketUniqueNum != cPacketHeader.header.packet_unique_id)
        {
            Debug.LogError("RecvPacket UniqueID Not Match! " + "send:" + rcPacketData.m_PacketUniqueNum + " , recv:" + cPacketHeader.header.packet_unique_id);
            return ServerDataManager.Instance.ResultCodeToCodeType(rcPacketData.m_PacketAPI, API_CODE.API_CODE_CLIENT_RETRY);
        }

        //--------------------------------
        // リザルトデータが存在しない場合に
        // サーバー班が result = null を仕込むのが工数がかかるようなので、
        // ここで例外的にresultが0配列の場合にnullに置き換える。
        // 
        // 基本的にエラー発生時にのみコレが引っかかるので、
        // 処理が発生する頻度自体は低いはず…
        //--------------------------------
        if (cPacketHeader.header.code != (int)API_CODE.API_CODE_SUCCESS
        && strRecvText[strRecvText.Length - 3] == '['
        && strRecvText[strRecvText.Length - 2] == ']'
        && strRecvText[strRecvText.Length - 1] == '}'
        )
        {
            Debug.LogError("Packet Result Format Safety!");
            strRecvText = strRecvText.Substring(0, strRecvText.Length - 3);
            strRecvText += "null}";
        }

        //--------------------------------
        // 届いたリザルトコードがAPI呼び出し元に渡さないタイプならそのまま終了
        //--------------------------------
        ResultCodeType cResultCodeType = null;
        {
            //--------------------------------
            // APIとリザルトコードのペアからコードタイプを求める
            //--------------------------------
            cResultCodeType = ServerDataManager.Instance.ResultCodeToCodeType(rcPacketData.m_PacketAPI, (API_CODE)cPacketHeader.header.code);
        }

        if (cResultCodeType != null)
        {
            bool bError = false;
            switch (cResultCodeType.m_CodeType)
            {
                case API_CODETYPE.API_CODETYPE_USUALLY: bError = false; break;
                case API_CODETYPE.API_CODETYPE_INVALID_USER: bError = true; break;
                case API_CODETYPE.API_CODETYPE_VERSION_UP: bError = true; break;
                case API_CODETYPE.API_CODETYPE_RESTART: bError = true; break;
                case API_CODETYPE.API_CODETYPE_MENTENANCE: bError = true; break;
                case API_CODETYPE.API_CODETYPE_RETRY: bError = true; break;
                case API_CODETYPE.API_CODETYPE_AUTH_REQUIRED: bError = true; break;
                case API_CODETYPE.API_CODETYPE_TRANSFER_MOVED: bError = true; break;
            }

            if (bError == true)
            {
                return cResultCodeType;
            }
        }
        else
        {
            Debug.LogError("Packet Result CodeType None! - " + cPacketHeader.header.code);
            return null;
        }

        if (bParse)
        {
            //--------------------------------
            // APIタイプで分岐して解析処理へ
            //--------------------------------
            switch (rcPacketData.m_PacketAPI)
            {
                case SERVER_API.SERVER_API_USER_CREATE: RecvPacketAPIConvert(JsonMapper.ToObject<RecvCreateUser>(strRecvText)); break;  // APIタイプ：ユーザー管理：ユーザー新規生成								
                case SERVER_API.SERVER_API_USER_AUTHENTICATION: RecvPacketAPIConvert(JsonMapper.ToObject<RecvUserAuthentication>(strRecvText)); break;  // APIタイプ：ユーザー管理：ユーザー承認									
                case SERVER_API.SERVER_API_USER_RENAME: RecvPacketAPIConvert(JsonMapper.ToObject<RecvRenameUser>(strRecvText)); break;  // APIタイプ：ユーザー管理：ユーザー名称変更								
                case SERVER_API.SERVER_API_USER_SELECT_DEF_PARTY: RecvPacketAPIConvert(JsonMapper.ToObject<RecvSelectDefParty>(strRecvText)); break;    // APIタイプ：ユーザー管理：ユーザー初期設定
                case SERVER_API.SERVER_API_USER_RENEW: RecvPacketAPIConvert(JsonMapper.ToObject<RecvRenewUser>(strRecvText)); break;    // APIタイプ：ユーザー管理：ユーザー再構築
                case SERVER_API.SERVER_API_USER_RENEW_CHECK: RecvPacketAPIConvert(JsonMapper.ToObject<RecvCheckRenewUser>(strRecvText)); break; // APIタイプ：ユーザー管理：ユーザー再構築情報問い合わせ
                case SERVER_API.SERVER_API_MASTER_HASH_GET: RecvPacketAPIConvert(JsonMapper.ToObject<RecvGetMasterHash>(strRecvText)); break;   // APIタイプ：マスターハッシュ情報取得
                case SERVER_API.SERVER_API_MASTER_DATA_GET_ALL: RecvPacketAPIConvert(JsonMapper.ToObject<RecvMasterDataAll>(strRecvText)); break;   // APIタイプ：マスターデータ操作：マスターデータ実体取得					
                case SERVER_API.SERVER_API_MASTER_DATA_GET_ALL2: RecvPacketAPIConvert(JsonMapper.ToObject<RecvMasterDataAll2>(strRecvText)); break;   // APIタイプ：マスターデータ操作：マスターデータ実体取得
                case SERVER_API.SERVER_API_GET_ACHIEVEMENT_GP: RecvPacketAPIConvert(JsonMapper.ToObject<RecvAchievementGroup>(strRecvText)); break; // APIタイプ：アチーブメント操作：アチーブメントグループ実体取得
                case SERVER_API.SERVER_API_GET_ACHIEVEMENT: RecvPacketAPIConvert(JsonMapper.ToObject<RecvMasterDataAchievement>(strRecvText)); break;   // APIタイプ：アチーブメント操作：アチーブメント実体取得
                case SERVER_API.SERVER_API_GET_LOGIN_PACK: RecvPacketAPIConvert(JsonMapper.ToObject<RecvLoginPack>(strRecvText)); break;    // APIタイプ：ログイン情報：ログインパック情報取得
                case SERVER_API.SERVER_API_GET_LOGIN_PARAM: RecvPacketAPIConvert(JsonMapper.ToObject<RecvLoginParam>(strRecvText)); break;  // APIタイプ：ログイン情報：ログイン情報取得
                case SERVER_API.SERVER_API_FRIEND_LIST_GET: RecvPacketAPIConvert(JsonMapper.ToObject<RecvFriendListGet>(strRecvText)); break;   // APIタイプ：フレンド操作：フレンド一覧取得								
                case SERVER_API.SERVER_API_FRIEND_REQUEST: RecvPacketAPIConvert(JsonMapper.ToObject<RecvFriendRequest>(strRecvText)); break;    // APIタイプ：フレンド操作：フレンド申請									
                case SERVER_API.SERVER_API_FRIEND_CONSENT: RecvPacketAPIConvert(JsonMapper.ToObject<RecvFriendConsent>(strRecvText)); break;    // APIタイプ：フレンド操作：フレンド申請承認								
                case SERVER_API.SERVER_API_FRIEND_REFUSAL: RecvPacketAPIConvert(JsonMapper.ToObject<RecvFriendRefusal>(strRecvText)); break;    // APIタイプ：フレンド操作：フレンド解除									
                case SERVER_API.SERVER_API_FRIEND_SEARCH: RecvPacketAPIConvert(JsonMapper.ToObject<RecvFriendSearch>(strRecvText)); break;  // APIタイプ：フレンド操作：フレンドユーザー検索							
                case SERVER_API.SERVER_API_UNIT_PARTY_ASSIGN: RecvPacketAPIConvert(JsonMapper.ToObject<RecvUnitPartyAssign>(strRecvText)); break;   // APIタイプ：ユニット操作：ユニットパーティ編成設定						
                case SERVER_API.SERVER_API_UNIT_SALE: RecvPacketAPIConvert(JsonMapper.ToObject<RecvUnitSale>(strRecvText)); break;  // APIタイプ：ユニット操作：ユニット売却									
                case SERVER_API.SERVER_API_UNIT_BLEND_BUILDUP: RecvPacketAPIConvert(JsonMapper.ToObject<RecvUnitBlendBuildUp>(strRecvText)); break; // APIタイプ：ユニット操作：ユニット強化合成								
                case SERVER_API.SERVER_API_UNIT_BLEND_EVOL: RecvPacketAPIConvert(JsonMapper.ToObject<RecvUnitBlendEvol>(strRecvText)); break;   // APIタイプ：ユニット操作：ユニット進化合成								
                case SERVER_API.SERVER_API_UNIT_LINK_CREATE: RecvPacketAPIConvert(JsonMapper.ToObject<RecvUnitLink>(strRecvText)); break;   // APIタイプ：ユニット操作：ユニットリンク実行
                case SERVER_API.SERVER_API_UNIT_LINK_DELETE: RecvPacketAPIConvert(JsonMapper.ToObject<RecvUnitLink>(strRecvText)); break;   // APIタイプ：ユニット操作：ユニットリンク解除
                case SERVER_API.SERVER_API_QUEST_HELPER_GET: RecvPacketAPIConvert(JsonMapper.ToObject<RecvQuestHelperGet>(strRecvText)); break; // APIタイプ：クエスト操作：助っ人一覧取得								
                case SERVER_API.SERVER_API_QUEST_HELPER_GET_EVOL: RecvPacketAPIConvert(JsonMapper.ToObject<RecvQuestHelperGetEvol>(strRecvText)); break;    // APIタイプ：クエスト操作：助っ人一覧取得（進化合成向き）
                case SERVER_API.SERVER_API_QUEST_HELPER_GET_BUILD: RecvPacketAPIConvert(JsonMapper.ToObject<RecvQuestHelperGetBuild>(strRecvText)); break;  // APIタイプ：クエスト操作：助っ人一覧取得（強化合成向き）
                case SERVER_API.SERVER_API_QUEST_START: RecvPacketAPIConvert(JsonMapper.ToObject<RecvQuestStart>(strRecvText)); break;  // APIタイプ：クエスト操作：クエスト開始									
                case SERVER_API.SERVER_API_QUEST_CLEAR: RecvPacketAPIConvert(JsonMapper.ToObject<RecvQuestClear>(strRecvText)); break;  // APIタイプ：クエスト操作：クエストクリア								
                case SERVER_API.SERVER_API_QUEST_RETIRE: RecvPacketAPIConvert(JsonMapper.ToObject<RecvQuestRetire>(strRecvText)); break;    // APIタイプ：クエスト操作：クエストリタイア
                case SERVER_API.SERVER_API_QUEST_ORDER_GET: RecvPacketAPIConvert(JsonMapper.ToObject<RecvQuestOrderGet>(strRecvText)); break;   // APIタイプ：クエスト操作：クエスト受託情報取得							
                case SERVER_API.SERVER_API_QUEST_CONTINUE: RecvPacketAPIConvert(JsonMapper.ToObject<RecvQuestContinue>(strRecvText)); break;    // APIタイプ：インゲーム中：コンティニュー								
                case SERVER_API.SERVER_API_QUEST_RESET: RecvPacketAPIConvert(JsonMapper.ToObject<RecvQuestReset>(strRecvText)); break;  // APIタイプ：インゲーム中：リセット										
                case SERVER_API.SERVER_API_EVOL_QUEST_START: RecvPacketAPIConvert(JsonMapper.ToObject<RecvEvolQuestStart>(strRecvText)); break; // APIタイプ：クエスト操作：進化クエスト開始										
                case SERVER_API.SERVER_API_EVOL_QUEST_CLEAR: RecvPacketAPIConvert(JsonMapper.ToObject<RecvEvolQuestClear>(strRecvText)); break; // APIタイプ：クエスト操作：進化クエストクリア									
                case SERVER_API.SERVER_API_INJUSTICE: RecvPacketAPIConvert(JsonMapper.ToObject<RecvInjustice>(strRecvText)); break; // APIタイプ：不正検出関連：不正検出送信									
                case SERVER_API.SERVER_API_TUTORIAL: RecvPacketAPIConvert(JsonMapper.ToObject<RecvTutorialStep>(strRecvText)); break;   // APIタイプ：チュートリアル関連：進行集計
                case SERVER_API.SERVER_API_STONE_USE_UNIT: RecvPacketAPIConvert(JsonMapper.ToObject<RecvStoneUsedUnit>(strRecvText)); break;    // APIタイプ：魔法石使用：ユニット枠増設									
                case SERVER_API.SERVER_API_STONE_USE_FRIEND: RecvPacketAPIConvert(JsonMapper.ToObject<RecvStoneUsedFriend>(strRecvText)); break;    // APIタイプ：魔法石使用：フレンド枠増設									
                case SERVER_API.SERVER_API_STONE_USE_STAMINA: RecvPacketAPIConvert(JsonMapper.ToObject<RecvStoneUsedStamina>(strRecvText)); break;  // APIタイプ：魔法石使用：スタミナ回復									
                case SERVER_API.SERVER_API_GACHA_PLAY: RecvPacketAPIConvert(JsonMapper.ToObject<RecvGachaPlay>(strRecvText)); break;    // APIタイプ：ガチャ操作：ガチャ実行										
                case SERVER_API.SERVER_API_GACHA_TICKET_PLAY: RecvPacketAPIConvert(JsonMapper.ToObject<RecvGachaTicketPlay>(strRecvText)); break;   // APIタイプ：ガチャ操作：ガチャチケット実行										
                case SERVER_API.SERVER_API_STONE_PAY_PREV_IOS: RecvPacketAPIConvert(JsonMapper.ToObject<RecvStorePayPrev_ios>(strRecvText)); break; // APIタイプ：課金操作：魔法石購入直前処理( iOS … AppStore )				
                case SERVER_API.SERVER_API_STONE_PAY_PREV_ANDROID: RecvPacketAPIConvert(JsonMapper.ToObject<RecvStorePayPrev_android>(strRecvText)); break; // APIタイプ：課金操作：魔法石購入直前処理( Android … GooglePlay )		
                case SERVER_API.SERVER_API_STONE_PAY_IOS: RecvPacketAPIConvert(JsonMapper.ToObject<RecvStorePay_ios>(strRecvText)); break;  // APIタイプ：課金操作：魔法石購入反映処理( iOS … AppStore )				
                case SERVER_API.SERVER_API_STONE_PAY_ANDROID: RecvPacketAPIConvert(JsonMapper.ToObject<RecvStorePay_android>(strRecvText)); break;  // APIタイプ：課金操作：魔法石購入反映処理( Android … GooglePlay )		
                case SERVER_API.SERVER_API_REVIEW_PRESENT: RecvPacketAPIConvert(JsonMapper.ToObject<RecvReviewPresent>(strRecvText)); break;    // APIタイプ：ユーザーレビュー関連：レビュー遷移報酬						
                case SERVER_API.SERVER_API_PRESENT_LIST_GET: RecvPacketAPIConvert(JsonMapper.ToObject<RecvPresentListGet>(strRecvText)); break; // APIタイプ：プレゼント関連：プレゼント一覧取得							
                case SERVER_API.SERVER_API_PRESENT_OPEN: RecvPacketAPIConvert(JsonMapper.ToObject<RecvPresentOpen>(strRecvText)); break;    // APIタイプ：プレゼント関連：プレゼント開封								
                case SERVER_API.SERVER_API_TRANSFER_ORDER: RecvPacketAPIConvert(JsonMapper.ToObject<RecvTransferOrder>(strRecvText)); break;    // APIタイプ：セーブ移行関連：パスワード発行
                case SERVER_API.SERVER_API_TRANSFER_EXEC: RecvPacketAPIConvert(JsonMapper.ToObject<RecvTransferExec>(strRecvText)); break;  // APIタイプ：セーブ移行関連：移行実行
#if BUILD_TYPE_DEBUG
                case SERVER_API.SERVER_API_DEBUG_EDIT_USER: RecvPacketAPIConvert(JsonMapper.ToObject<RecvDebugEditUser>(strRecvText)); break;   // APIタイプ：デバッグ機能関連：ユーザーデータ更新
                case SERVER_API.SERVER_API_DEBUG_UNIT_GET: RecvPacketAPIConvert(JsonMapper.ToObject<RecvDebugUnitGet>(strRecvText)); break; // APIタイプ：デバッグ機能関連：ユニット取得
                case SERVER_API.SERVER_API_DEBUG_QUEST_CLEAR: RecvPacketAPIConvert(JsonMapper.ToObject<RecvDebugQuestClear>(strRecvText)); break;   // APIタイプ：デバッグ機能関連：クエストクリア情報改変
                case SERVER_API.SERVER_API_DEBUG_SEND_BATTLE_LOG: RecvPacketAPIConvert(JsonMapper.ToObject<RecvDebugBattleLog>(strRecvText)); break;   // APIタイプ：デバッグ機能関連：バトルログアップロード
                case SERVER_API.SERVER_API_DEBUG_MASTER_DATA_GET_ALL2: RecvPacketAPIConvert(JsonMapper.ToObject<RecvMasterDataAll2>(strRecvText)); break;   // APIタイプ：マスターデータ操作：マスターデータ実体取得
#endif
                case SERVER_API.SERVER_API_GET_STORE_EVENT: RecvPacketAPIConvert(JsonMapper.ToObject<RecvGetStoreProductEvent>(strRecvText)); break;    // APIタイプ：イベントストア一覧取得
                case SERVER_API.SERVER_API_STORE_PAY_CANCEL: RecvPacketAPIConvert(JsonMapper.ToObject<RecvStorePayCancel>(strRecvText)); break; // APIタイプ：課金操作：魔法石購入キャンセル
                case SERVER_API.SERVER_API_ACHIEVEMENT_OPEN: RecvPacketAPIConvert(JsonMapper.ToObject<RecvOpenAchievement>(strRecvText)); break;    // APIタイプ：アチーブメント開封
                case SERVER_API.SERVER_API_CHECK_SNS_LINK: RecvPacketAPIConvert(JsonMapper.ToObject<RecvCheckSnsLink>(strRecvText)); break; //!< APIタイプ：SNSIDとの紐付け確認
                case SERVER_API.SERVER_API_SET_SNS_LINK: RecvPacketAPIConvert(JsonMapper.ToObject<RecvSetSnsLink>(strRecvText)); break; //!< APIタイプ：SNSIDとの紐付け
                case SERVER_API.SERVER_API_MOVE_SNS_SAVE_DATA: RecvPacketAPIConvert(JsonMapper.ToObject<RecvMoveSnsSaveData>(strRecvText)); break;  //!< APIタイプ：SNSIDを使用したデータ移行

                case SERVER_API.SERVER_API_TUTORIAL_SKIP: RecvPacketAPIConvert(JsonMapper.ToObject<RecvSkipTutorial>(strRecvText)); break;  //!< APIタイプ：チュートリアルスキップ

                case SERVER_API.SERVER_API_GET_SNS_ID: RecvPacketAPIConvert(JsonMapper.ToObject<RecvGetSnsID>(strRecvText)); break; //!< APIタイプ：SNSID作成

                case SERVER_API.SERVER_API_GET_POINT_SHOP_PRODUCT: RecvPacketAPIConvert(JsonMapper.ToObject<RecvGetPointShopProduct>(strRecvText)); break;  //!< APIタイプ：ポイントショップ：ショップ商品情報を取得		get_point_shop_product			
                case SERVER_API.SERVER_API_POINT_SHOP_PURCHASE: RecvPacketAPIConvert(JsonMapper.ToObject<RecvPointShopPurchase>(strRecvText)); break;   //!< APIタイプ：ポイントショップ：商品購入					point_shop_purchase
                case SERVER_API.SERVER_API_POINT_SHOP_LIMITOVER: RecvPacketAPIConvert(JsonMapper.ToObject<RecvPointShopLimitOver>(strRecvText)); break; //!< APIタイプ：ポイントショップ：限界突破
                case SERVER_API.SERVER_API_POINT_SHOP_EVOL: RecvPacketAPIConvert(JsonMapper.ToObject<RecvPointShopEvol>(strRecvText)); break;   //!< APIタイプ：ポイントショップ：進化

                case SERVER_API.SERVER_API_USE_ITEM: RecvPacketAPIConvert(JsonMapper.ToObject<RecvItemUse>(strRecvText)); break;    //!< APIタイプ：消費アイテム：アイテム使用

                case SERVER_API.SERVER_API_GET_BOX_GACHA_STOCK: RecvPacketAPIConvert(JsonMapper.ToObject<RecvGetBoxGachaStock>(strRecvText)); break;    //!< APIタイプ：BOXガチャ在庫状況取得
                case SERVER_API.SERVER_API_RESET_BOX_GACHA_STOCK: RecvPacketAPIConvert(JsonMapper.ToObject<RecvResetBoxGachaStock>(strRecvText)); break;    //!< APIタイプ：BOXガチャ在庫状況リセット
                case SERVER_API.SERVER_API_EVOLVE_UNIT: RecvPacketAPIConvert(JsonMapper.ToObject<RecvEvolveUnit>(strRecvText)); break;  // APIタイプ：クエスト操作：進化クエスト開始										
                case SERVER_API.SERVER_API_GET_GUERRILLA_BOSS_INFO: RecvPacketAPIConvert(JsonMapper.ToObject<RecvGetGuerrillaBossInfo>(strRecvText)); break;    //!< APIタイプ：ゲリラボス情報
                case SERVER_API.SERVER_API_GET_GACHA_LINEUP: RecvPacketAPIConvert(JsonMapper.ToObject<RecvGetGachaLineup>(strRecvText)); break;  // APIタイプ：ガチャ：ガチャラインナップ詳細										
                case SERVER_API.SERVER_API_RENEW_TUTORIAL: RecvPacketAPIConvert(JsonMapper.ToObject<RecvRenewTutorialStep>(strRecvText)); break;   // APIタイプ：チュートリアル関連：進行集計
                case SERVER_API.SERVER_API_GET_TOPIC_INFO: RecvPacketAPIConvert(JsonMapper.ToObject<RecvGetTopicInfo>(strRecvText)); break;   // APIタイプ：ホームページのトピック : ニュース情報取得

                case SERVER_API.SERVER_API_PERIODIC_UPDATE: RecvPacketAPIConvert(JsonMapper.ToObject<RecvPeriodicUpdate>(strRecvText)); break;  //!< APIタイプ：定期データ更新(デバイストークン)
                case SERVER_API.SERVER_API_GET_PRESENT_OPEN_LOG: RecvPacketAPIConvert(JsonMapper.ToObject<RecvGetPresentOpenLog>(strRecvText)); break;   // APIタイプ：ホームページのトピック : ニュース情報取得
            }
        }

        //--------------------------------
        // アチーブメントの共通情報を加算しておく
        //--------------------------------
        if (cPacketHeader != null)
        {
            ResidentParam.AddAchievementClear(cPacketHeader.header.achievement_clear_data);
            // 値が0以上の場合のみバッチの値を更新
            if (cPacketHeader.header.achievement_reward_count >= 0)
            {
                ResidentParam.m_AchievementRewardCnt = cPacketHeader.header.achievement_reward_count;
            }

            // 新規アチーブメントカウント数を更新
            ResidentParam.AddAchievementNewCnt(cPacketHeader.header.achievement_new_count);
        }

        return cResultCodeType;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	サーバーパラメータに反映
	*/
    //----------------------------------------------------------------------------
    static private void RecvPacketAPIConvert(RecvCreateUser cRecvAPI) { ServerDataParam.m_RecvPacketCreateUser = cRecvAPI; ServerDataParam.m_ServerConnectOK = true; }  // サーバー受信情報：ユーザー管理：ユーザー新規生成
    static private void RecvPacketAPIConvert(RecvUserAuthentication cRecvAPI) { ServerDataParam.m_RecvPacketUserAuthentication = cRecvAPI; ServerDataParam.m_ServerConnectOK = true; }  // サーバー受信情報：ユーザー管理：ユーザー承認
    static private void RecvPacketAPIConvert(RecvRenameUser cRecvAPI) { ServerDataParam.m_RecvPacketRenameUser = cRecvAPI; }    // サーバー受信情報：ユーザー管理：ユーザー名称変更
    static private void RecvPacketAPIConvert(RecvSelectDefParty cRecvAPI) { ServerDataParam.m_RecvPacketSelectDefParty = cRecvAPI; }    // サーバー受信情報：ユーザー管理：ユーザー初期設定
    static private void RecvPacketAPIConvert(RecvRenewUser cRecvAPI) { ServerDataParam.m_RecvPacketRenewUser = cRecvAPI; ServerDataParam.m_ServerConnectOK = true; }    // サーバー受信情報：ユーザー管理：ユーザー再構築
    static private void RecvPacketAPIConvert(RecvCheckRenewUser cRecvAPI) { ServerDataParam.m_RecvPacketRenewUserCheck = cRecvAPI; }    // サーバー受信情報：ユーザー管理：ユーザー再構築情報問い合わせ
    static private void RecvPacketAPIConvert(RecvLoginPack cRecvAPI) { ServerDataParam.m_RecvPacketLoginPack = cRecvAPI; }  // サーバー受信情報：ログイン情報：ログインパック情報取得
    static private void RecvPacketAPIConvert(RecvLoginParam cRecvAPI) { ServerDataParam.m_RecvPacketLoginParam = cRecvAPI; }    // サーバー受信情報：ログイン情報：ログイン情報取得
    static private void RecvPacketAPIConvert(RecvFriendListGet cRecvAPI) { ServerDataParam.m_RecvPacketFriendListGet = cRecvAPI; }  // サーバー受信情報：フレンド操作：フレンド一覧取得
    static private void RecvPacketAPIConvert(RecvFriendRequest cRecvAPI) { ServerDataParam.m_RecvPacketFriendRequest = cRecvAPI; }  // サーバー受信情報：フレンド操作：フレンド申請
    static private void RecvPacketAPIConvert(RecvFriendConsent cRecvAPI) { ServerDataParam.m_RecvPacketFriendConsent = cRecvAPI; }  // サーバー受信情報：フレンド操作：フレンド申請承認
    static private void RecvPacketAPIConvert(RecvFriendRefusal cRecvAPI) { ServerDataParam.m_RecvPacketFriendRefusal = cRecvAPI; }  // サーバー受信情報：フレンド操作：フレンド解除
    static private void RecvPacketAPIConvert(RecvFriendSearch cRecvAPI) { ServerDataParam.m_RecvPacketFriendSearch = cRecvAPI; }    // サーバー受信情報：フレンド操作：フレンドユーザー検索
    static private void RecvPacketAPIConvert(RecvUnitPartyAssign cRecvAPI) { ServerDataParam.m_RecvPacketUnitPartyAssign = cRecvAPI; }  // サーバー受信情報：ユニット操作：ユニットパーティ編成設定
    static private void RecvPacketAPIConvert(RecvUnitSale cRecvAPI) { ServerDataParam.m_RecvPacketUnitSale = cRecvAPI; }    // サーバー受信情報：ユニット操作：ユニット売却
    static private void RecvPacketAPIConvert(RecvUnitBlendBuildUp cRecvAPI) { ServerDataParam.m_RecvPacketUnitBlendBuildUp = cRecvAPI; }    // サーバー受信情報：ユニット操作：ユニット強化合成
    static private void RecvPacketAPIConvert(RecvUnitBlendEvol cRecvAPI) { ServerDataParam.m_RecvPacketUnitBlendEvol = cRecvAPI; }  // サーバー受信情報：ユニット操作：ユニット進化合成
    static private void RecvPacketAPIConvert(RecvUnitLink cRecvAPI) { ServerDataParam.m_RecvPacketUnitLink = cRecvAPI; }    // サーバー受信情報：ユニット操作：ユニットリンク
    static private void RecvPacketAPIConvert(RecvQuestHelperGet cRecvAPI) { ServerDataParam.m_RecvPacketQuestHelperGet = cRecvAPI; }    // サーバー受信情報：クエスト操作：助っ人一覧取得
    static private void RecvPacketAPIConvert(RecvQuestHelperGetEvol cRecvAPI) { ServerDataParam.m_RecvPacketQuestHelperGetEvol = cRecvAPI; }    // サーバー受信情報：クエスト操作：助っ人一覧取得（進化合成向け）
    static private void RecvPacketAPIConvert(RecvQuestHelperGetBuild cRecvAPI) { ServerDataParam.m_RecvPacketQuestHelperGetBuild = cRecvAPI; }  // サーバー受信情報：クエスト操作：助っ人一覧取得（強化合成向け）
    static private void RecvPacketAPIConvert(RecvQuestStart cRecvAPI) { ServerDataParam.m_RecvPacketQuestStart = cRecvAPI; }    // サーバー受信情報：クエスト操作：クエスト開始
    static private void RecvPacketAPIConvert(RecvQuestClear cRecvAPI) { ServerDataParam.m_RecvPacketQuestClear = cRecvAPI; }    // サーバー受信情報：クエスト操作：クエストクリア
    static private void RecvPacketAPIConvert(RecvQuestRetire cRecvAPI) { ServerDataParam.m_RecvPacketQuestRetire = cRecvAPI; }  // サーバー受信情報：クエスト操作：クエストリタイア
    static private void RecvPacketAPIConvert(RecvQuestOrderGet cRecvAPI) { ServerDataParam.m_RecvPacketQuestOrderGet = cRecvAPI; }  // サーバー受信情報：クエスト操作：クエスト受託情報取得
    static private void RecvPacketAPIConvert(RecvEvolQuestStart cRecvAPI) { ServerDataParam.m_RecvPacketEvolQuestStart = cRecvAPI; }    // サーバー受信情報：クエスト操作：進化クエスト開始
    static private void RecvPacketAPIConvert(RecvEvolQuestClear cRecvAPI) { ServerDataParam.m_RecvPacketEvolQuestClear = cRecvAPI; }    // サーバー受信情報：クエスト操作：進化クエストクリア
    static private void RecvPacketAPIConvert(RecvQuestContinue cRecvAPI) { ServerDataParam.m_RecvPacketQuestContinue = cRecvAPI; }  // サーバー受信情報：インゲーム中：コンティニュー
    static private void RecvPacketAPIConvert(RecvQuestReset cRecvAPI) { ServerDataParam.m_RecvPacketQuestReset = cRecvAPI; }    // サーバー受信情報：インゲーム中：リセット
    static private void RecvPacketAPIConvert(RecvInjustice cRecvAPI) { ServerDataParam.m_RecvPacketInjustice = cRecvAPI; }  // サーバー受信情報：不正検出関連：不正検出送信
    static private void RecvPacketAPIConvert(RecvTutorialStep cRecvAPI) { ServerDataParam.m_RecvPacketTutorialStep = cRecvAPI; }    // サーバー受信情報：チュートリアル関連：進行集計
    static private void RecvPacketAPIConvert(RecvStoneUsedUnit cRecvAPI) { ServerDataParam.m_RecvPacketStoneUsedUnit = cRecvAPI; }  // サーバー受信情報：魔法石使用：ユニット枠増設
    static private void RecvPacketAPIConvert(RecvStoneUsedFriend cRecvAPI) { ServerDataParam.m_RecvPacketStoneUsedFriend = cRecvAPI; }  // サーバー受信情報：魔法石使用：フレンド枠増設
    static private void RecvPacketAPIConvert(RecvStoneUsedStamina cRecvAPI) { ServerDataParam.m_RecvPacketStoneUsedStamina = cRecvAPI; }    // サーバー受信情報：魔法石使用：スタミナ回復
    static private void RecvPacketAPIConvert(RecvGachaPlay cRecvAPI) { ServerDataParam.m_RecvPacketGachaPlay = cRecvAPI; }  // サーバー受信情報：ガチャ操作：ガチャ実行
    static private void RecvPacketAPIConvert(RecvGachaTicketPlay cRecvAPI) { ServerDataParam.m_RecvPacketGachaTicketPlay = cRecvAPI; }  // サーバー受信情報：ガチャ操作：ガチャチケット実行
    static private void RecvPacketAPIConvert(RecvStorePayPrev_ios cRecvAPI) { ServerDataParam.m_RecvPacketStorePayPrev_ios = cRecvAPI; }    // サーバー受信情報：課金操作：魔法石購入直前処理( iOS … AppStore )
    static private void RecvPacketAPIConvert(RecvStorePayPrev_android cRecvAPI) { ServerDataParam.m_RecvPacketStorePayPrev_android = cRecvAPI; }    // サーバー受信情報：課金操作：魔法石購入直前処理( Android … GooglePlay )
    static private void RecvPacketAPIConvert(RecvStorePay_ios cRecvAPI) { ServerDataParam.m_RecvPacketStorePay_ios = cRecvAPI; }    // サーバー受信情報：課金操作：魔法石購入反映処理( iOS … AppStore )
    static private void RecvPacketAPIConvert(RecvStorePay_android cRecvAPI) { ServerDataParam.m_RecvPacketStorePay_android = cRecvAPI; }    // サーバー受信情報：課金操作：魔法石購入反映処理( Android … GooglePlay )
    static private void RecvPacketAPIConvert(RecvReviewPresent cRecvAPI) { ServerDataParam.m_RecvPacketReviewPresent = cRecvAPI; }  // サーバー受信情報：ユーザーレビュー関連：レビュー遷移報酬
    static private void RecvPacketAPIConvert(RecvPresentListGet cRecvAPI) { ServerDataParam.m_RecvPacketPresentListGet = cRecvAPI; }    // サーバー受信情報：プレゼント関連：プレゼント一覧取得
    static private void RecvPacketAPIConvert(RecvPresentOpen cRecvAPI) { ServerDataParam.m_RecvPacketPresentOpen = cRecvAPI; }  // サーバー受信情報：プレゼント関連：プレゼント開封
    static private void RecvPacketAPIConvert(RecvTransferOrder cRecvAPI) { ServerDataParam.m_RecvPacketTransferOrder = cRecvAPI; }  // サーバー受信情報：セーブ移行関連：パスワード発行
    static private void RecvPacketAPIConvert(RecvTransferExec cRecvAPI) { ServerDataParam.m_RecvPacketTransferExec = cRecvAPI; }    // サーバー受信情報：セーブ移行関連：移行実行
    static private void RecvPacketAPIConvert(RecvDebugEditUser cRecvAPI) { ServerDataParam.m_RecvPacketDebugEditUser = cRecvAPI; }  // サーバー受信情報：デバッグ機能関連：ユーザーランクアップ
    static private void RecvPacketAPIConvert(RecvDebugUnitGet cRecvAPI) { ServerDataParam.m_RecvPacketDebugUnitGet = cRecvAPI; }    // サーバー受信情報：デバッグ機能関連：ユニット取得
    static private void RecvPacketAPIConvert(RecvDebugQuestClear cRecvAPI) { ServerDataParam.m_RecvPacketDebugQuestClear = cRecvAPI; }  // サーバー受信情報：デバッグ機能関連：クエストクリア情報改変
    static private void RecvPacketAPIConvert(RecvDebugBattleLog cRecvAPI) { ServerDataParam.m_RecvPacketDebugBattleLog = cRecvAPI; }    // サーバー受信情報：デバッグ機能関連：クエストクリア情報改変
    static private void RecvPacketAPIConvert(RecvMasterDataAll cRecvAPI) { ServerDataParam.m_RecvPacketMasterDataAll = cRecvAPI; }  // サーバー受信情報：マスターデータ実体：全種
    static private void RecvPacketAPIConvert(RecvMasterDataAll2 cRecvAPI) { ServerDataParam.m_RecvPacketMasterDataAll2 = cRecvAPI; }    // サーバー受信情報：マスターデータ実体：差分
    static private void RecvPacketAPIConvert(RecvGetMasterHash cRecvAPI) { ServerDataParam.m_RecvGetMasterHash = cRecvAPI; }    // サーバー受信情報：マスターデータハッシュ実体：全種
    static private void RecvPacketAPIConvert(RecvAchievementGroup cRecvAPI) { ServerDataParam.m_RecvPacketAchievementGroup = cRecvAPI; }    // サーバー受信情報：アチーブメント実体：アチーブメントグループ
    static private void RecvPacketAPIConvert(RecvMasterDataAchievement cRecvAPI) { ServerDataParam.m_RecvPacketMasterDataAchievement = cRecvAPI; }  // サーバー受信情報：アチーブメント実体：アチーブメント
    static private void RecvPacketAPIConvert(RecvGetStoreProductEvent cRecvAPI) { ServerDataParam.m_RecvPacketStoreProductEvent = cRecvAPI; }   // サーバー受信情報：イベントストア一覧取得
    static private void RecvPacketAPIConvert(RecvStorePayCancel cRecvAPI) { ServerDataParam.m_RecvPacketStorePayCancel = cRecvAPI; }    // サーバー受信情報：課金操作：魔法石購入キャンセル
    static private void RecvPacketAPIConvert(RecvOpenAchievement cRecvAPI) { ServerDataParam.m_RecvPacketOpenAchievdement = cRecvAPI; } // サーバー受信情報：アチーブメント開封
    static private void RecvPacketAPIConvert(RecvCheckSnsLink cRecvAPI) { ServerDataParam.m_RecvPacketCheckSnsLink = cRecvAPI; }    // サーバー受信情報：SNSIDとの紐付け確認
    static private void RecvPacketAPIConvert(RecvSetSnsLink cRecvAPI) { ServerDataParam.m_RecvPacketSetSnsLink = cRecvAPI; }    // サーバー受信情報：SNSIDとの紐付け
    static private void RecvPacketAPIConvert(RecvMoveSnsSaveData cRecvAPI) { ServerDataParam.m_RecvPacketMoveSnsSaveData = cRecvAPI; }  // サーバー受信情報：SNSIDを使用したデータ移行

    static private void RecvPacketAPIConvert(RecvSkipTutorial cRecvAPI) { ServerDataParam.m_RecvPacketSkipTutorial = cRecvAPI; }    // サーバー受信情報：チュートリアルスキップ

    static private void RecvPacketAPIConvert(RecvGetSnsID cRecvAPI) { ServerDataParam.m_RecvPacketGetSnsID = cRecvAPI; }    // サーバー受信情報：SNSID取得

    static private void RecvPacketAPIConvert(RecvGetPointShopProduct cRecvAPI) { ServerDataParam.m_RecvPacketGetPointShopProduct = cRecvAPI; }  //!< APIタイプ：ポイントショップ：ショップ商品情報を取得		get_point_shop_product			
    static private void RecvPacketAPIConvert(RecvPointShopPurchase cRecvAPI) { ServerDataParam.m_RecvPacketPointShopPurchase = cRecvAPI; }  //!< APIタイプ：ポイントショップ：商品購入					point_shop_purchase
    static private void RecvPacketAPIConvert(RecvPointShopLimitOver cRecvAPI) { ServerDataParam.m_RecvPacketPointShopLimitOver = cRecvAPI; }    //!< APIタイプ：ポイントショップ：限界突破
    static private void RecvPacketAPIConvert(RecvPointShopEvol cRecvAPI) { ServerDataParam.m_RecvPacketPointShopEvol = cRecvAPI; }  //!< APIタイプ：ポイントショップ：進化

    static private void RecvPacketAPIConvert(RecvItemUse cRecvAPI) { ServerDataParam.m_RecvPacketItemUse = cRecvAPI; }  //!< サーバー受信情報：消費アイテム：アイテム使用

    static private void RecvPacketAPIConvert(RecvGetBoxGachaStock cRecvAPI) { ServerDataParam.m_RecvPacketGetBoxGachaStock = cRecvAPI; }    //!< APIタイプ：BOXガチャ在庫状況取得
    static private void RecvPacketAPIConvert(RecvResetBoxGachaStock cRecvAPI) { ServerDataParam.m_RecvPacketResetBoxGachaStock = cRecvAPI; }    //!< APIタイプ：BOXガチャ在庫状況リセット
    static private void RecvPacketAPIConvert(RecvSetCurrentHero cRecvAPI) { ServerDataParam.m_RecvPacketSetCurrentHero = cRecvAPI; }    //!< APIタイプ：HERO選択セーブ
    static private void RecvPacketAPIConvert(RecvEvolveUnit cRecvAPI) { ServerDataParam.m_RecvPacketEvolveUnit = cRecvAPI; }    //!< APIタイプ：ユニット強化
    static private void RecvPacketAPIConvert(RecvGetGuerrillaBossInfo cRecvAPI) { ServerDataParam.m_RecvPacketGetGuerrillaBossInfo = cRecvAPI; }    //!< APIタイプ：ゲリラボス情報取得
    static private void RecvPacketAPIConvert(RecvGetGachaLineup cRecvAPI) { ServerDataParam.m_RecvGetGachaLineup = cRecvAPI; }  //!< APIタイプ：ガチャラインナップ詳細
    static private void RecvPacketAPIConvert(RecvRenewTutorialStep cRecvAPI) { ServerDataParam.m_RecvPacketRenewTutorialStep = cRecvAPI; }  // サーバー受信情報：リニューアルチュートリアル関連：進行集計
    static private void RecvPacketAPIConvert(RecvGetTopicInfo cRecvAPI) { ServerDataParam.m_RecvPacketGetTopicInfo = cRecvAPI; } // サーバー受信情報：ホームページのトピック : ニュース情報取得

    static private void RecvPacketAPIConvert(RecvPeriodicUpdate cRecvAPI) { ServerDataParam.m_RecvPeriodicUpdate = cRecvAPI; }  //!< APIタイプ：定期データ更新(デバイストークン)
    static private void RecvPacketAPIConvert(RecvGetPresentOpenLog cRecvAPI) { ServerDataParam.m_RecvGetPresentOpenLog = cRecvAPI; }    //!< APIタイプ：プレゼント関連：プレゼント開封ログ取得
};


/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
