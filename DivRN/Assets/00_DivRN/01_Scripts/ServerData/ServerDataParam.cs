/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	ServerDataParam.cs
	@brief	サーバー通信関連データ保持クラス
	@author Developer
	@date 	2013/02/05
	
	@note	このクラスは ServerDataManager , ServerDataUtil からのみ更新されることを想定する。
 
			サーバーから受け取ったデータは本来、必要としている領域に書き込みが実行される。
			負荷だけを考えると受信データ解析の際に直接反映する方が良いが、
			仕様追加等によってServerDataManager内部のコードが煩雑になるし、
			APIを設計する段階で受け取ったデータを受け取る領域をちゃんと設計しないと作り直しにより工数が肥大化する。
			
			このクラスでデータを一旦保持することで
			外部クラスとServerDataManagerの関連性を下げ、拡張性,流用性を増す。
			また、サーバーAPI担当者とAPI受信データを使った実装担当者の実装タイミングのラグの許容も可能になる。
			
			-------------------------------------------------------
			実際のフローは以下な感じを想定。
			
			[ 各処理にてサーバーへの通信が必要になる ]
			　↓　↓　↓　↓　↓　↓　↓　
			[ ServerDataManager.AddCommunicateRequest で必要なAPIリクエストを飛ばす。管理番号として返り値は保持 ]
			　↓　↓　↓　↓　↓　↓　↓　
			[ ServerDataManager.ChkCommunicateFinishAll でサーバー通信の完遂をチェック ]
			　↓　↓　↓　↓　↓　↓　↓　
			[ ServerDataManager.GetPacketResult に管理番号を指定してリクエストをかけたAPI通信の結果を取得 ]
			　↓　↓　↓　↓　↓　↓　↓　
			[ 結果をもとに処理分岐。結果を反映したい場合は ServerDataParam にアクセスして必要な情報を取得する ]
			
			
			-------------------------------------------------------
			
			この実装では、同じ領域を更新するAPI通信が連続して発生した場合に、必ず後の通信によって情報が上書きされる。
			通信待ちが入るので連続発生の状況はイレギュラーケースと判断してシングルバッファにしているが、
			連続発生を許容したい場合にはバッファを用意しておき、何が起因で書き換えられたかの情報を保持する必要がある。
			工数と費用対効果とメモリ使用量等々の観点から今回は対応しない。
			
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
	@brief	サーバー通信関連データ保持クラス
*/
//----------------------------------------------------------------------------
static public class ServerDataParam
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    static public ulong m_ServerTime = 0;					//!< 最後の通信処理でサーバーから返って来た時間情報

    static public string m_ServerSessionID = "";            //!< 
    static public string m_ServerSessionIDCookie = "";     	//!< 
    static public bool m_ServerConnectOK = false;       	//!< サーバー接続完了フラグ
    static public string m_ServerAPICheckSum = "";          //!< サーバーセキュリティコード（チェックサム）
    static public long m_ServerAPICheckRandOtt = 0;         //!< サーバーセキュリティコード（サーバー乱数）

    static public RecvCreateUser m_RecvPacketCreateUser;						//!< サーバー受信情報：ユーザー管理：ユーザー新規生成
    static public RecvUserAuthentication m_RecvPacketUserAuthentication;  		//!< サーバー受信情報：ユーザー管理：ユーザー承認
    static public RecvRenameUser m_RecvPacketRenameUser;                        //!< サーバー受信情報：ユーザー管理：ユーザー名称変更
    static public RecvSelectDefParty m_RecvPacketSelectDefParty;          		//!< サーバー受信情報：ユーザー管理：ユーザー初期設定
    static public RecvRenewUser m_RecvPacketRenewUser;                      	//!< サーバー受信情報：ユーザー管理：ユーザー再構築
    static public RecvCheckRenewUser m_RecvPacketRenewUserCheck;              	//!< サーバー受信情報：ユーザー管理：ユーザー再構築情報問い合わせ
    static public RecvLoginPack m_RecvPacketLoginPack;                      	//!< サーバー受信情報：ログイン情報：ログインパック情報取得
    static public RecvLoginParam m_RecvPacketLoginParam;                        //!< サーバー受信情報：ログイン情報：ログイン情報取得
    static public RecvFriendListGet m_RecvPacketFriendListGet;                  //!< サーバー受信情報：フレンド操作：フレンド一覧取得
    static public RecvFriendRequest m_RecvPacketFriendRequest;                  //!< サーバー受信情報：フレンド操作：フレンド申請
    static public RecvFriendConsent m_RecvPacketFriendConsent;                  //!< サーバー受信情報：フレンド操作：フレンド申請承認
    static public RecvFriendRefusal m_RecvPacketFriendRefusal;                  //!< サーバー受信情報：フレンド操作：フレンド解除
    static public RecvFriendSearch m_RecvPacketFriendSearch;                    //!< サーバー受信情報：フレンド操作：フレンドユーザー検索
    static public RecvUnitPartyAssign m_RecvPacketUnitPartyAssign;              //!< サーバー受信情報：ユニット操作：ユニットパーティ編成設定
    static public RecvUnitSale m_RecvPacketUnitSale;                        	//!< サーバー受信情報：ユニット操作：ユニット売却
    static public RecvUnitBlendBuildUp m_RecvPacketUnitBlendBuildUp;         	//!< サーバー受信情報：ユニット操作：ユニット強化合成
    static public RecvUnitBlendEvol m_RecvPacketUnitBlendEvol;                  //!< サーバー受信情報：ユニット操作：ユニット進化合成
    static public RecvUnitLink m_RecvPacketUnitLink;                        	//!< サーバー受信情報：ユニット操作：ユニットリンク
    static public RecvQuestHelperGet m_RecvPacketQuestHelperGet;            	//!< サーバー受信情報：クエスト操作：助っ人一覧取得
    static public RecvQuestHelperGetEvol m_RecvPacketQuestHelperGetEvol;     	//!< サーバー受信情報：クエスト操作：助っ人一覧取得（進化合成向け）
    static public RecvQuestHelperGetBuild m_RecvPacketQuestHelperGetBuild;		//!< サーバー受信情報：クエスト操作：助っ人一覧取得（強化合成向け）
    static public RecvQuestStart m_RecvPacketQuestStart;                        //!< サーバー受信情報：クエスト操作：クエスト開始
    static public RecvQuestClear m_RecvPacketQuestClear;                        //!< サーバー受信情報：クエスト操作：クエストクリア
    static public RecvQuestRetire m_RecvPacketQuestRetire;                  	//!< サーバー受信情報：クエスト操作：クエストリタイア
    static public RecvQuestOrderGet m_RecvPacketQuestOrderGet;                  //!< サーバー受信情報：クエスト操作：クエスト受託情報取得
    static public RecvEvolQuestStart m_RecvPacketEvolQuestStart;             	//!< サーバー受信情報：クエスト操作：進化クエスト開始
    static public RecvEvolQuestClear m_RecvPacketEvolQuestClear;         		//!< サーバー受信情報：クエスト操作：進化クエストクリア
    static public RecvQuestContinue m_RecvPacketQuestContinue;                  //!< サーバー受信情報：インゲーム中：コンティニュー
    static public RecvQuestReset m_RecvPacketQuestReset;                        //!< サーバー受信情報：インゲーム中：リセット
    static public RecvInjustice m_RecvPacketInjustice;                      	//!< サーバー受信情報：不正検出関連：不正検出送信
    static public RecvTutorialStep m_RecvPacketTutorialStep;                    //!< サーバー受信情報：チュートリアル関連：進行集計
    static public RecvStoneUsedUnit m_RecvPacketStoneUsedUnit;                  //!< サーバー受信情報：魔法石使用：ユニット枠増設
    static public RecvStoneUsedFriend m_RecvPacketStoneUsedFriend;              //!< サーバー受信情報：魔法石使用：フレンド枠増設
    static public RecvStoneUsedStamina m_RecvPacketStoneUsedStamina;       		//!< サーバー受信情報：魔法石使用：スタミナ回復
    static public RecvGachaPlay m_RecvPacketGachaPlay;                      	//!< サーバー受信情報：ガチャ操作：ガチャ実行
    static public RecvGachaTicketPlay m_RecvPacketGachaTicketPlay;              //!< サーバー受信情報：ガチャ操作：ガチャチケット実行
    static public RecvStorePayPrev_ios m_RecvPacketStorePayPrev_ios;         	//!< サーバー受信情報：課金操作：魔法石購入直前処理( iOS … AppStore )
    static public RecvStorePayPrev_android m_RecvPacketStorePayPrev_android;	//!< サーバー受信情報：課金操作：魔法石購入直前処理( Android … GooglePlay )
    static public RecvStorePay_ios m_RecvPacketStorePay_ios;                    //!< サーバー受信情報：課金操作：魔法石購入反映処理( iOS … AppStore )
    static public RecvStorePay_android m_RecvPacketStorePay_android;    		//!< サーバー受信情報：課金操作：魔法石購入反映処理( Android … GooglePlay )
    static public RecvReviewPresent m_RecvPacketReviewPresent;                  //!< サーバー受信情報：ユーザーレビュー関連：レビュー遷移報酬
    static public RecvPresentListGet m_RecvPacketPresentListGet;            	//!< サーバー受信情報：プレゼント関連：プレゼント一覧取得
    static public RecvPresentOpen m_RecvPacketPresentOpen;                      //!< サーバー受信情報：プレゼント関連：プレゼント開封

    //	static	public	RecvPresentThrow				m_RecvPacketPresentThrow;					//!< サーバー受信情報：プレゼント関連：プレゼント送信

    static public RecvTransferOrder m_RecvPacketTransferOrder;                  //!< サーバー受信情報：セーブ移行関連：パスワード発行
    static public RecvTransferExec m_RecvPacketTransferExec;                    //!< サーバー受信情報：セーブ移行関連：移行実行

    static public RecvDebugEditUser m_RecvPacketDebugEditUser;                  //!< サーバー受信情報：デバッグ機能関連：ユーザーデータ編集
    static public RecvDebugUnitGet m_RecvPacketDebugUnitGet;                    //!< サーバー受信情報：デバッグ機能関連：ユニット取得
    static public RecvDebugQuestClear m_RecvPacketDebugQuestClear;              //!< サーバー受信情報：デバッグ機能関連：クエストクリア情報改変
    static public RecvDebugBattleLog m_RecvPacketDebugBattleLog;                //!< サーバー受信情報：デバッグ機能関連：バトルログアップロード

    static public RecvMasterDataAll m_RecvPacketMasterDataAll;                  //!< サーバー受信情報：マスターデータ実体：全種
    static public RecvMasterDataAll2 m_RecvPacketMasterDataAll2;              	//!< サーバー受信情報：マスターデータ実体：全種
    static public RecvAchievementGroup m_RecvPacketAchievementGroup;          	//!< サーバー受信情報：アチーブメント：アチーブメントグループ
    static public RecvMasterDataAchievement m_RecvPacketMasterDataAchievement;	//!< サーバー受信情報：アチーブメント：アチーブメント
    static public RecvGetStoreProductEvent m_RecvPacketStoreProductEvent;     	//!< サーバー受信情報：イベントストア一覧取得
    static public RecvStorePayCancel m_RecvPacketStorePayCancel;               	//!< サーバー受信情報：課金操作：魔法石購入キャンセル

    static public RecvOpenAchievement m_RecvPacketOpenAchievdement;             //!< サーバー受信情報：アチーブメント開封
    static public RecvAchievementCount[] m_RecvAchievementCount;               	//!< サーバー受信情報：アチーブメント達成条件に回数指定有り
    static public RecvGetMasterHash m_RecvGetMasterHash;                        //!< サーバー受信情報：
    static public RecvCheckSnsLink m_RecvPacketCheckSnsLink;                    //!< サーバー受信情報：SNSIDとの紐付け確認
    static public RecvSetSnsLink m_RecvPacketSetSnsLink;                        //!< サーバー受信情報：SNSIDとの紐付け
    static public RecvMoveSnsSaveData m_RecvPacketMoveSnsSaveData;              //!< サーバー受信情報：SNSIDを使用したデータ移行

    static public RecvSkipTutorial m_RecvPacketSkipTutorial;                    //!< サーバー受信情報：チュートリアルスキップ

    static public RecvGetSnsID m_RecvPacketGetSnsID;                       		//!< サーバー受信情報：SNSID取得
    static public RecvGetPointShopProduct m_RecvPacketGetPointShopProduct;		//!< サーバー受信情報：ポイントショップ：ショップ商品情報を取得		get_point_shop_product			
    static public RecvPointShopPurchase m_RecvPacketPointShopPurchase;         	//!< サーバー受信情報：ポイントショップ：商品購入					point_shop_purchase
    static public RecvPointShopLimitOver m_RecvPacketPointShopLimitOver;   		//!< サーバー受信情報：ポイントショップ：限界突破
    static public RecvPointShopEvol m_RecvPacketPointShopEvol;                  //!< サーバー受信情報：ポイントショップ：進化
    static public RecvItemUse m_RecvPacketItemUse;                      		//!< サーバー受信情報：アイテム使用

    static public RecvGetBoxGachaStock m_RecvPacketGetBoxGachaStock;        	//!< サーバー受信情報：ボックスガチャ在庫取得
    static public RecvResetBoxGachaStock m_RecvPacketResetBoxGachaStock;		//!< サーバー受信情報：ボックスガチャ在庫リセット
    static public RecvSetCurrentHero m_RecvPacketSetCurrentHero;               	//!< サーバー受信情報：HERO選択セーブ
    static public RecvEvolveUnit m_RecvPacketEvolveUnit;                      	//!< サーバー受信情報：ユニット進化
    static public RecvGetGuerrillaBossInfo m_RecvPacketGetGuerrillaBossInfo;	//!< サーバー受信情報：ゲリラボス情報
    static public RecvGetGachaLineup m_RecvGetGachaLineup;                     	//!< サーバー受信情報：ガチャラインナップ
    static public RecvRenewTutorialStep m_RecvPacketRenewTutorialStep;        	//!< サーバー受信情報：リニューアルチュートリアル関連：進行集計
    static public RecvGetTopicInfo m_RecvPacketGetTopicInfo;                    //!< サーバー受信情報：ホームページのトピック : ニュース情報取得取得

    static public RecvPeriodicUpdate m_RecvPeriodicUpdate;                      //!< サーバー受信情報：定期データ更新(デバイストークン)
    static public RecvGetPresentOpenLog m_RecvGetPresentOpenLog;         		//!< サーバー受信情報：プレゼント関連： プレゼント開封ログ取得

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
};


/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
