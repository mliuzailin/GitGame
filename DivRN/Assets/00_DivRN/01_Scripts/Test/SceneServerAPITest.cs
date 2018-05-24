using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using M4u;
using ServerDataDefine;
using ServerApiTest;
using LitJson;
using System;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class SceneServerAPITest : Scene<SceneServerAPITest>, M4uContextInterface
{
    public GameObject apiAutoTesterPrefab;

    //
    const long UNIT_UNIQUE_INDEX_BASE = 10000000000000;


    //
    public EMASTERDATA[] getMasterDataList = null;

    public List<uint> getMasterDataTagIdList;
    public bool masterDataOutput = false;
    public string renameData = "";
    public DefaultPartyType defaultParty = DefaultPartyType.Red;
    public int tutorialStep = 0;
    public EditUserTransfar userTransfar;
    public string[] friendList;
    public string searchFriendId = "";
    public long[] presentSerialID;
    public EditPartyAssign party;
    public EditUnitBuildup unitbuildup;
    public EditUnitLink unitlink;
    public long[] saleUnit;
    public EditQuestStart questStart;
    public EditEvolQuest evolQuestStart;
    public EditAchievementGroup achievementGroup;
    public EditAchievement achievement;
    public EditAchievementOpen achievementOpen;
    public uint useItemId;
    public EditGacha gacha;
    public EditShopLimitOver pointShop;
    public EditPlayerParam debugPlayerParam;
    public EditGetUnitParam debugUnitGet;
    public int heroId;
    public int uniqueId;
    public bool is_premium;
    public EditEvolveUnit evolveUnit;
    public int[] guerrilla_boss_area_id;
    public int get_gacha_lineup_gacha_id = 308200;
    public PacketStructGachaLineup[] recvGachaLineup;
    public PacketStructPlayer debugStructPlayer;
    public RecvGetTopicInfoValue recvGetTopicInfoValue;
    public RecvMasterDataAchievementValue recvMasterDataAchievementValueTest;
    public RecvGetPresentOpenLogValue recvGetPresentOpenLogValue;

    private ServerApi serverApi = null;
    private string serverUUID = "";

    M4uProperty<string> address = new M4uProperty<string>();

    public string Address
    {
        get
        {
            return address.Value;
        }
        set
        {
            address.Value = value;
        }
    }

    M4uProperty<string> uuid = new M4uProperty<string>();

    public string Uuid
    {
        get
        {
            return uuid.Value;
        }
        set
        {
            uuid.Value = value;
        }
    }

    M4uProperty<string> player_name = new M4uProperty<string>();

    public string Player_name
    {
        get
        {
            return player_name.Value;
        }
        set
        {
            player_name.Value = value;
        }
    }

    M4uProperty<List<ServerApiMenuItem>> menulist = new M4uProperty<List<ServerApiMenuItem>>();

    public List<ServerApiMenuItem> Menulist
    {
        get
        {
            return menulist.Value;
        }
        set
        {
            menulist.Value = value;
        }
    }


    public void OnClickAutoTest()
    {
        APIAutoTester tester = Instantiate(apiAutoTesterPrefab).GetComponent<APIAutoTester>();

        tester.Play(
            () =>
            {
                Debug.Log("FINISH_AUTOTEST");
            });
    }


    protected override void Awake()
    {
        base.Awake();
        gameObject.GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        Address = LocalSaveManager.Instance.LoadFuncServerAddressIP();
        Uuid = LocalSaveManager.Instance.LoadFuncUUID();

        OnChangeMenu(0);
    }

    public override void OnInitialized()
    {
        base.OnInitialized();
        updateUser(null);
    }

    // Update is called once per frame
    void Update()
    {
#if false
//----------------------------------------
// サーバー処理が完遂してるかチェック
//----------------------------------------
		bool bServerFinish = ServerDataUtilSend.CheckPacketFinishAll();
		if( bServerFinish == false )
		{
			return;
		}

		//----------------------------------------
		// 完遂してるなら結果を取得
		//----------------------------------------
		PacketDataResult cPacketResult = ServerDataUtilSend.GetPacketResult( serverResuestId );
		if( cPacketResult == null )
		{
//			Debug.LogError( "PacketResult None!" );
			return;
		}
		if (cPacketResult.m_PacketCode == API_CODE.API_CODE_SUCCESS)
		{
			switch(cPacketResult.m_PacketAPI)
			{
				case SERVER_API.SERVER_API_USER_CREATE:
					Debug.Log( "USER_CREATE:Success" );
					LocalSaveManager.Instance.SaveFuncUUID(serverUUID);

					UserDataAdmin.Instance.m_StructPlayer = ServerDataParam.m_RecvPacketCreateUser.result.player;
					UserDataAdmin.Instance.m_StructSystem = ServerDataParam.m_RecvPacketCreateUser.result.system;
					UserDataAdmin.Instance.ConvertPartyAssing();
					ServerDataParam.m_RecvPacketCreateUser = null;

					return;
				case SERVER_API.SERVER_API_USER_AUTHENTICATION:
					Debug.Log( "USER_AUTHENTICATION:Success" );

					updateUser(ServerDataParam.m_RecvPacketUserAuthentication.result.player);
					UserDataAdmin.Instance.m_StructSystem = ServerDataParam.m_RecvPacketUserAuthentication.result.system;
					UserDataAdmin.Instance.m_StructQuest = ServerDataParam.m_RecvPacketUserAuthentication.result.quest;
					ServerDataParam.m_RecvPacketUserAuthentication = null;

					//----------------------------------------
					// ユーザー認証APIでのユーザー認証要請のイレギュラー対応を除去。
					// ここで外しておかないと他のAPIのセッション切れがスルーされるので確実に切っておく
					//----------------------------------------
					ServerDataManager.Instance.ResultCodeDelIrregular( SERVER_API.SERVER_API_USER_AUTHENTICATION , API_CODE.API_CODE_USER_AUTH_REQUIRED		);
					ServerDataManager.Instance.ResultCodeDelIrregular( SERVER_API.SERVER_API_USER_AUTHENTICATION , API_CODE.API_CODE_USER_AUTH_REQUIRED2	);
					break;
				case SERVER_API.SERVER_API_MASTER_HASH_GET:
					Debug.Log( "MASTER_HASH_GET:Success" );
					ServerDataParam.m_RecvGetMasterHash = null;
					break;
				case SERVER_API.SERVER_API_MASTER_DATA_GET_ALL:
					Debug.Log( "MASTER_DATA_GET_ALL:Success" );
					if(masterDataOutput)outputMasterData(ServerDataParam.m_RecvPacketMasterDataAll.result);
					ServerDataParam.m_RecvPacketMasterDataAll = null;
					break;
				case SERVER_API.SERVER_API_GET_LOGIN_PACK:
					Debug.Log( "GET_LOGIN_PACK:Success" );
					UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist( ServerDataParam.m_RecvPacketLoginPack.result.result_friend.friend );
					UserDataAdmin.Instance.m_StructHelperList = UserDataAdmin.FriendListClipMe( ServerDataParam.m_RecvPacketLoginPack.result.result_helper.friend );
					ServerDataParam.m_RecvPacketLoginPack = null;
					break;
				case SERVER_API.SERVER_API_USER_RENAME:
					Debug.Log( "USER_RENAME:Success" );
					UserDataAdmin.Instance.m_StructPlayer.user.user_name = ServerDataParam.m_RecvPacketRenameUser.result.user_name;
					updateUser(null);
					ServerDataParam.m_RecvPacketRenameUser = null;
					break;
				case SERVER_API.SERVER_API_USER_SELECT_DEF_PARTY:
					Debug.Log( "USER_SELECT_DEF_PARTY:Success" );
					updateUser(ServerDataParam.m_RecvPacketSelectDefParty.result.player);
					ServerDataParam.m_RecvPacketSelectDefParty = null;
					break;
				case SERVER_API.SERVER_API_TUTORIAL_SKIP:
					Debug.Log( "TUTORIAL_SKIP:Success" );
					updateUser(ServerDataParam.m_RecvPacketSkipTutorial.result.player);
					ServerDataParam.m_RecvPacketSkipTutorial = null;
					break;
				case SERVER_API.SERVER_API_TUTORIAL:
					Debug.Log( "TUTORIAL:Success" );
					ServerDataParam.m_RecvPacketTutorialStep = null;
					break;
				case SERVER_API.SERVER_API_TRANSFER_ORDER:
					Debug.Log( "TRANSFER_ORDER:Success password:" + ServerDataParam.m_RecvPacketTransferOrder.result.password );
					ServerDataParam.m_RecvPacketTransferOrder = null;
					break;
				case SERVER_API.SERVER_API_TRANSFER_EXEC:
					Debug.Log( "TRANSFER_EXEC:Success" );
					ServerDataParam.m_RecvPacketTransferExec = null;
					break;
				case SERVER_API.SERVER_API_GET_SNS_ID:
					Debug.Log( "GET_SNS_ID:Success" );
					userTransfar.snsId = ServerDataParam.m_RecvPacketGetSnsID.result.sns_id;
					ServerDataParam.m_RecvPacketGetSnsID = null;
					break;
				case SERVER_API.SERVER_API_SET_SNS_LINK:
					Debug.Log( "SET_SNS_LINK:Success" );
					ServerDataParam.m_RecvPacketSetSnsLink = null;
					break;
				case SERVER_API.SERVER_API_CHECK_SNS_LINK:
					Debug.Log( "CHECK_SNS_LINK:Success" );
					ServerDataParam.m_RecvPacketCheckSnsLink = null;
					break;
				case SERVER_API.SERVER_API_MOVE_SNS_SAVE_DATA:
					Debug.Log( "SNS_SAVE_DATA:Success" );
					ServerDataParam.m_RecvPacketMoveSnsSaveData = null;
					break;

				///
				///
				///
				case SERVER_API.SERVER_API_FRIEND_LIST_GET:
					Debug.Log( "FRIEND_LIST_GET:Success" );
					UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist( ServerDataParam.m_RecvPacketFriendListGet.result.friend );
					ServerDataParam.m_RecvPacketFriendListGet = null;
					break;
				case SERVER_API.SERVER_API_FRIEND_REQUEST:
					Debug.Log( "FRIEND_REQUEST:Success" );
					UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist( ServerDataParam.m_RecvPacketFriendRequest.result.friend );
					ServerDataParam.m_RecvPacketFriendRequest = null;
					break;
				case SERVER_API.SERVER_API_FRIEND_CONSENT:
					Debug.Log( "FRIEND_CONSENT:Success" );
					UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist( ServerDataParam.m_RecvPacketFriendConsent.result.friend );
					ServerDataParam.m_RecvPacketFriendConsent = null;
					break;
				case SERVER_API.SERVER_API_FRIEND_REFUSAL:
					Debug.Log( "FRIEND_REFUSAL:Success" );
					UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist( ServerDataParam.m_RecvPacketFriendRefusal.result.friend );
					ServerDataParam.m_RecvPacketFriendRefusal = null;
					break;
				case SERVER_API.SERVER_API_FRIEND_SEARCH:
					Debug.Log( "FRIEND_SEARCH:Success [" + ServerDataParam.m_RecvPacketFriendSearch.result.friend.user_name + "]"  );
					ServerDataParam.m_RecvPacketFriendSearch = null;
					break;
				case SERVER_API.SERVER_API_PRESENT_LIST_GET:
					Debug.Log( "PRESENT_LIST_GET:Success" );
					OnInfoPresent(ServerDataParam.m_RecvPacketPresentListGet.result);
					ServerDataParam.m_RecvPacketPresentListGet = null;
					break;
				case SERVER_API.SERVER_API_PRESENT_OPEN:
					Debug.Log( "PRESENT_OPEN:Success" );
					updateUser(ServerDataParam.m_RecvPacketPresentOpen.result.player);
					ServerDataParam.m_RecvPacketPresentOpen = null;
					break;

				///
				///
				///
				case SERVER_API.SERVER_API_UNIT_PARTY_ASSIGN:
					Debug.Log( "UNIT_PARTY_ASSIGN:Success" );
					updateUser(ServerDataParam.m_RecvPacketUnitPartyAssign.result.player);
					ServerDataParam.m_RecvPacketUnitPartyAssign = null;
					break;
				case SERVER_API.SERVER_API_UNIT_BLEND_BUILDUP:
					Debug.Log( "UNIT_BLEND_BUILDUP:Success" );
					updateUser(ServerDataParam.m_RecvPacketUnitBlendBuildUp.result.player);
					ServerDataParam.m_RecvPacketUnitBlendBuildUp = null;
					break;
				case SERVER_API.SERVER_API_QUEST_HELPER_GET_BUILD:
					Debug.Log( "QUEST_HELPER_GET_BUILD:Success" );
					ServerDataParam.m_RecvPacketQuestHelperGetBuild = null;
					break;
				case SERVER_API.SERVER_API_UNIT_SALE:
					Debug.Log( "UNIT_SALE:Success" );
					updateUser(ServerDataParam.m_RecvPacketUnitSale.result.player);
					ServerDataParam.m_RecvPacketUnitSale = null;
					break;
				case SERVER_API.SERVER_API_UNIT_LINK_CREATE:
					Debug.Log( "UNIT_LINK_CREATE:Success" );
					updateUser(ServerDataParam.m_RecvPacketUnitLink.result.player);
					ServerDataParam.m_RecvPacketUnitLink = null;
					break;
				case SERVER_API.SERVER_API_UNIT_LINK_DELETE:
					Debug.Log( "UNIT_LINK_DELETE:Success" );
					updateUser(ServerDataParam.m_RecvPacketUnitLink.result.player);
					ServerDataParam.m_RecvPacketUnitLink = null;
					break;

				///
				///
				///
				case SERVER_API.SERVER_API_QUEST_START:
					Debug.Log( "QUEST_START:Success" );
					ServerDataParam.m_RecvPacketQuestStart = null;
					break;
				case SERVER_API.SERVER_API_QUEST_CLEAR:
					Debug.Log( "QUEST_CLEAR:Success" );
					updateUser(ServerDataParam.m_RecvPacketQuestClear.result.player);
					ServerDataParam.m_RecvPacketQuestClear = null;
					break;
				case SERVER_API.SERVER_API_QUEST_RETIRE:
					Debug.Log( "QUEST_RETIRE:Success" );
					ServerDataParam.m_RecvPacketQuestRetire = null;
					break;
				case SERVER_API.SERVER_API_QUEST_CONTINUE:
					Debug.Log( "QUEST_CONTINUE:Success" );
					ServerDataParam.m_RecvPacketQuestContinue = null;
					break;
				case SERVER_API.SERVER_API_QUEST_RESET:
					Debug.Log( "QUEST_RESET:Success" );
					ServerDataParam.m_RecvPacketQuestReset = null;
					break;
				case SERVER_API.SERVER_API_QUEST_HELPER_GET_EVOL:
					Debug.Log( "QUEST_HELPER_GET_EVOL:Success" );
					ServerDataParam.m_RecvPacketQuestHelperGetEvol = null;
					break;
				case SERVER_API.SERVER_API_EVOL_QUEST_START:
					Debug.Log( "EVOL_QUEST_START:Success" );
					ServerDataParam.m_RecvPacketEvolQuestStart = null;
					break;
				case SERVER_API.SERVER_API_EVOL_QUEST_CLEAR:
					Debug.Log( "EVOL_QUEST_CLEAR:Success" );
					updateUser(ServerDataParam.m_RecvPacketEvolQuestClear.result.player);
					ServerDataParam.m_RecvPacketEvolQuestClear = null;
					break;
				case SERVER_API.SERVER_API_GET_ACHIEVEMENT_GP:
					Debug.Log( "GET_ACHIEVEMENT_GP:Success" );
					ServerDataParam.m_RecvPacketAchievementGroup = null;
					break;
				case SERVER_API.SERVER_API_GET_ACHIEVEMENT:
					Debug.Log( "GET_ACHIEVEMENT:Success" );
					ServerDataParam.m_RecvPacketMasterDataAchievement = null;
					break;
				case SERVER_API.SERVER_API_ACHIEVEMENT_OPEN:
					Debug.Log( "ACHIEVEMENT_OPEN:Success" );
					ServerDataParam.m_RecvPacketOpenAchievdement = null;
					break;

				///
				///
				///
				case SERVER_API.SERVER_API_GACHA_PLAY:
					Debug.Log( "GACHA_PLAY:Success" );
					updateUser(ServerDataParam.m_RecvPacketGachaPlay.result.player);
					ServerDataParam.m_RecvPacketGachaPlay = null;
					break;
				case SERVER_API.SERVER_API_GACHA_TICKET_PLAY:
					Debug.Log( "GACHA_TICKET_PLAY:Success" );
					updateUser(ServerDataParam.m_RecvPacketGachaTicketPlay.result.player);
					ServerDataParam.m_RecvPacketGachaTicketPlay = null;
					break;
				case SERVER_API.SERVER_API_STONE_USE_STAMINA:
					Debug.Log( "STONE_USE_STAMINA:Success" );
					updateUser(ServerDataParam.m_RecvPacketStoneUsedStamina.result.player);
					ServerDataParam.m_RecvPacketStoneUsedStamina = null;
					break;
				case SERVER_API.SERVER_API_STONE_USE_UNIT:
					Debug.Log( "STONE_USE_UNIT:Success" );
					updateUser(ServerDataParam.m_RecvPacketStoneUsedUnit.result.player);
					ServerDataParam.m_RecvPacketStoneUsedUnit = null;
					break;
				case SERVER_API.SERVER_API_STONE_USE_FRIEND:
					Debug.Log( "STONE_USE_FRIEND:Success" );
					updateUser(ServerDataParam.m_RecvPacketStoneUsedFriend.result.player);
					ServerDataParam.m_RecvPacketStoneUsedFriend = null;
					break;
				case SERVER_API.SERVER_API_GET_POINT_SHOP_PRODUCT:
					Debug.Log( "GET_POINT_SHOP_PRODUCT:Success" );
					OnInfoPointShop(ServerDataParam.m_RecvPacketGetPointShopProduct.result);
					ServerDataParam.m_RecvPacketGetPointShopProduct = null;
					break;

				///
				///
				///
#if BUILD_TYPE_DEBUG
				case SERVER_API.SERVER_API_DEBUG_EDIT_USER:
					Debug.Log( "DEBUG_EDIT_USER:Success" );
					updateUser(ServerDataParam.m_RecvPacketDebugEditUser.result.player);
					ServerDataParam.m_RecvPacketDebugEditUser = null;
					break;
#endif
#if BUILD_TYPE_DEBUG
				case SERVER_API.SERVER_API_DEBUG_UNIT_GET:
					Debug.Log( "DEBUG_UNIT_GET:Success" );
					updateUser(ServerDataParam.m_RecvPacketDebugUnitGet.result.player);
					ServerDataParam.m_RecvPacketDebugUnitGet = null;
					break;
#endif
				default:
					break;
			}
		}else
		{
			API_CODE code = (API_CODE)cPacketResult.m_PacketCode;
			Debug.Log( "API:Faled to code:" + code.ToString() );
		}
		serverResuestId = 0xffff;
#endif
    }

    public void errorCodeLog(API_CODE _code)
    {
        Debug.Log("API:Faled to code:" + _code.ToString());
    }

    /// <summary>
    /// ユーザー情報更新
    /// </summary>
    /// <param name="_player"></param>
    public void updateUser(PacketStructPlayer _player)
    {
        if (_player != null)
        {
            UserDataAdmin.Instance.m_StructPlayer = _player;
            UserDataAdmin.Instance.ConvertPartyAssing();
        }

        Uuid = LocalSaveManager.Instance.LoadFuncUUID();

        PacketStructPlayer player = UserDataAdmin.Instance.m_StructPlayer;
        if (player == null)
        {
            Debug.Log("player is null");
            return;
        }

        string _user_id = UnityUtil.CreateDrawUserID(UserDataAdmin.Instance.m_StructPlayer.user.user_id);
        Player_name = player.user.user_name + "(" + _user_id + ")";

        renameData = player.user.user_name;
#if false
		debugPlayerParam.Rank = (int)player.rank;
		debugPlayerParam.GameMoney = (int)player.have_money;
		debugPlayerParam.FriendPoint = (int)player.have_friend_pt;
		debugPlayerParam.FreePaidPoint = (int)player.have_stone_free;
		debugPlayerParam.TicketCasino = (int)player.have_ticket;
		debugPlayerParam.UnitPoint = (int)player.have_unit_point;
		debugPlayerParam.BuyMaxUnitCount = (int)player.extend_unit;
		debugPlayerParam.BuyMaxFriendCount = (int)player.extend_friend;
		debugPlayerParam.AllQuestClear = false;
#endif
        party.currentParty = player.unit_party_current;
        party.assign = player.unit_party_assign;
    }

    /// <summary>
    /// メニューボタン変更
    /// </summary>
    /// <param name="_select"></param>
    public void OnChangeMenu(int _select)
    {
        if (Menulist == null)
        {
            Menulist = new List<ServerApiMenuItem>();
        }
        else
        {
            Menulist.Clear();
        }
        switch (_select)
        {
            case 0: //管理
                Menulist.Add(new ServerApiMenuItem("ユーザー作成", SERVER_API.SERVER_API_USER_CREATE, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ユーザー認証", SERVER_API.SERVER_API_USER_AUTHENTICATION, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("マスターデータハッシュ取得", SERVER_API.SERVER_API_MASTER_HASH_GET, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("マスターデータ取得", SERVER_API.SERVER_API_MASTER_DATA_GET_ALL, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("マスターデータ取得2", SERVER_API.SERVER_API_MASTER_DATA_GET_ALL2, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ログインパック", SERVER_API.SERVER_API_GET_LOGIN_PACK, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("名前変更", SERVER_API.SERVER_API_USER_RENAME, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("初期パーティ選択", SERVER_API.SERVER_API_USER_SELECT_DEF_PARTY, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("チュートリアルスキップ", SERVER_API.SERVER_API_TUTORIAL_SKIP, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("チュートリアル", SERVER_API.SERVER_API_TUTORIAL, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("リニューアルチュートリアル", SERVER_API.SERVER_API_RENEW_TUTORIAL, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("引き継ぎパスワード発行", SERVER_API.SERVER_API_TRANSFER_ORDER, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("引き継ぎ実行", SERVER_API.SERVER_API_TRANSFER_EXEC, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("SNSID発行", SERVER_API.SERVER_API_GET_SNS_ID, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("SNSID紐付け", SERVER_API.SERVER_API_SET_SNS_LINK, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("SNSID紐付け確認", SERVER_API.SERVER_API_CHECK_SNS_LINK, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("SNSID引き継ぎ実行", SERVER_API.SERVER_API_MOVE_SNS_SAVE_DATA, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("主人公選択", SERVER_API.SERVER_API_SET_CURRENT_HERO, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("トピックニュース情報取得", SERVER_API.SERVER_API_GET_TOPIC_INFO, OnSendApi));
                break;
            case 1: //フレンド
                Menulist.Add(new ServerApiMenuItem("フレンドリスト取得", SERVER_API.SERVER_API_FRIEND_LIST_GET, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("フレンド申請", SERVER_API.SERVER_API_FRIEND_REQUEST, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("フレンド申請承認", SERVER_API.SERVER_API_FRIEND_CONSENT, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("フレンド解除", SERVER_API.SERVER_API_FRIEND_REFUSAL, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("フレンド検索", SERVER_API.SERVER_API_FRIEND_SEARCH, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("プレゼントリスト取得", SERVER_API.SERVER_API_PRESENT_LIST_GET, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("プレゼント開封ログ取得", SERVER_API.SERVER_API_GET_PRESENT_OPEN_LOG, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("プレゼント取得", SERVER_API.SERVER_API_PRESENT_OPEN, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("チケットガチャ", SERVER_API.SERVER_API_GACHA_TICKET_PLAY, OnSendApi));
                break;
            case 2: //ユニット
                Menulist.Add(new ServerApiMenuItem("パーティ編成", SERVER_API.SERVER_API_UNIT_PARTY_ASSIGN, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ユニット強化合成", SERVER_API.SERVER_API_UNIT_BLEND_BUILDUP, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("プレミアム強化フレンド取得", SERVER_API.SERVER_API_QUEST_HELPER_GET_BUILD, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ユニット売却", SERVER_API.SERVER_API_UNIT_SALE, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ユニットリンク実行", SERVER_API.SERVER_API_UNIT_LINK_CREATE, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ユニットリンク解除", SERVER_API.SERVER_API_UNIT_LINK_DELETE, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ユニット進化合成", SERVER_API.SERVER_API_EVOLVE_UNIT, OnSendApi));
                break;
            case 3: //クエスト
                Menulist.Add(new ServerApiMenuItem("クエスト開始", SERVER_API.SERVER_API_QUEST_START, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("クエストクリア", SERVER_API.SERVER_API_QUEST_CLEAR, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("クエストリタイア", SERVER_API.SERVER_API_QUEST_RETIRE, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("クエストコンテニュー", SERVER_API.SERVER_API_QUEST_CONTINUE, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("クエストリトライ", SERVER_API.SERVER_API_QUEST_RESET, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("プレミアム進化フレンド取得", SERVER_API.SERVER_API_QUEST_HELPER_GET_EVOL, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("進化クエスト開始", SERVER_API.SERVER_API_EVOL_QUEST_START, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("進化クエストクリア", SERVER_API.SERVER_API_EVOL_QUEST_CLEAR, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ミッショングループ取得", SERVER_API.SERVER_API_GET_ACHIEVEMENT_GP, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ミッションリスト取得", SERVER_API.SERVER_API_GET_ACHIEVEMENT, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ミッション報酬取得", SERVER_API.SERVER_API_ACHIEVEMENT_OPEN, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("アイテム使用", SERVER_API.SERVER_API_USE_ITEM, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ゲリラボス情報", SERVER_API.SERVER_API_GET_GUERRILLA_BOSS_INFO, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("新クエスト開始", SERVER_API.SERVER_API_QUEST2_START, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("新クエストクリア", SERVER_API.SERVER_API_QUEST2_CLEAR, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("新クエスト情報取得", SERVER_API.SERVER_API_QUEST2_ORDER_GET, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("新クエスト中断データ破棄通知", SERVER_API.SERVER_API_QUEST2_CESSAION_QUEST, OnSendApi));
                break;
            case 4: //ガチャショップ
                Menulist.Add(new ServerApiMenuItem("通常ガチャ", SERVER_API.SERVER_API_GACHA_PLAY, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("スタミナ回復", SERVER_API.SERVER_API_STONE_USE_STAMINA, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ユニット拡張", SERVER_API.SERVER_API_STONE_USE_UNIT, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("フレンド拡張", SERVER_API.SERVER_API_STONE_USE_FRIEND, OnSendApi));
                //Menulist.Add(new ServerApiMenuItem("魔法石購入", SERVER_API.SERVER_API_STONE_PAY_PREV_ANDROID, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ポイントショップリスト取得", SERVER_API.SERVER_API_GET_POINT_SHOP_PRODUCT, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("アイテム購入", SERVER_API.SERVER_API_POINT_SHOP_PURCHASE, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("限界突破", SERVER_API.SERVER_API_POINT_SHOP_LIMITOVER, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ガチャラインナップ詳細", SERVER_API.SERVER_API_GET_GACHA_LINEUP, OnSendApi));
                break;
#if BUILD_TYPE_DEBUG
            case 5: //デバッグ
                Menulist.Add(new ServerApiMenuItem("ユーザーパラメータ変更", SERVER_API.SERVER_API_DEBUG_EDIT_USER, OnSendApi));
                Menulist.Add(new ServerApiMenuItem("ユニット取得", SERVER_API.SERVER_API_DEBUG_UNIT_GET, OnSendApi));
                break;
#endif
            default:
                break;
        }
    }


    /// <summary>
    /// ユーザー削除確認
    /// </summary>
    public void OnDeleteUser()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogText(DialogTextType.Title, "ユーザ削除");
        newDialog.SetDialogText(DialogTextType.MainText, "ユーザーを削除します。\nよろしいですか？");
        newDialog.SetDialogText(DialogTextType.YesText, "はい");
        newDialog.SetDialogText(DialogTextType.NoText, "いいえ");
        newDialog.SetDialogEvent(
            DialogButtonEventType.YES,
            new System.Action(
                () =>
                {
                    deleteUser();
                }));
        newDialog.Show();
    }

    /// <summary>
    /// ユーザー削除
    /// </summary>
    private void deleteUser()
    {
        //----------------------------------------
        // ローカルセーブを破棄して再構築
        //----------------------------------------
        LocalSaveManager.LocalSaveRenew(true, true);

        Uuid = LocalSaveManager.Instance.LoadFuncUUID();
        Player_name = "";
    }

    /// <summary>
    /// ユーザー情報表示
    /// </summary>
    public void OnInfoUser()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogText(DialogTextType.Title, "ユーザ詳細");
        {
            string mainText = "";

            PacketStructPlayer player = UserDataAdmin.Instance.m_StructPlayer;
            debugStructPlayer = UserDataAdmin.Instance.m_StructPlayer;

            mainText += "ランク：" + player.rank.ToString() + "\n";
            mainText += "経験値：" + player.exp.ToString() + "\n";
            mainText += "スタミナ：" + player.stamina_now.ToString() + "/" + player.stamina_max.ToString() + "\n";
            mainText += "お金：" + player.have_money.ToString() + "\n";
            mainText += "チケット：" + player.have_ticket.ToString() + "\n";
            mainText += "魔法石：" + player.have_stone.ToString() + "\n";
            mainText += "ユニットポイント：" + player.have_unit_point.ToString() + "\n";
            mainText += "ユニット枠拡張：" + player.extend_unit.ToString() + "\n";
            mainText += "フレンド枠拡張：" + player.extend_friend.ToString() + "\n";
            foreach (PacketStructHero Hero in UserDataAdmin.Instance.m_StructHeroList)
            {
                mainText += "Hero_hero_id：" + Hero.hero_id.ToString() + "\n";
                mainText += "Hero_unique_id：" + Hero.unique_id.ToString() + "\n";
            }

            newDialog.SetDialogText(DialogTextType.MainText, mainText);
            newDialog.SetTextAlignment(DialogTextType.MainText, TMPro.TextAlignmentOptions.TopLeft);
        }

        newDialog.SetDialogText(DialogTextType.OKText, "＞確認");
        newDialog.Show();
    }

    /// <summary>
    /// ユニット情報表示
    /// </summary>
    public void OnInfoUnit()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogText(DialogTextType.Title, "ユニット詳細");
        {
            string mainText = "";
            string[] linkinfoText =
            {
                "―",
                "親",
                "子"
            };

            PacketStructPlayer player = UserDataAdmin.Instance.m_StructPlayer;

            foreach (PacketStructUnit unit in player.unit_list)
            {
                MasterDataParamChara _chara = MasterFinder<MasterDataParamChara>.Instance.Find((int)unit.id);
                if (_chara != null)
                {
                    mainText += string.Format("[{0,6}][{1:0000}][{4}]{2} LV:{3} SLV:{5} LO:{6}\n", unit.unique_id - UNIT_UNIQUE_INDEX_BASE, unit.id, _chara.name, unit.level, linkinfoText[unit.link_info], unit.limitbreak_lv, unit.limitover_lv);
                }
            }

            newDialog.SetDialogText(DialogTextType.MainText, mainText);
            newDialog.SetTextAlignment(DialogTextType.MainText, TMPro.TextAlignmentOptions.TopLeft);
        }

        newDialog.SetDialogText(DialogTextType.OKText, "＞確認");
        newDialog.Show();
    }

    /// <summary>
    /// フレンド情報表示
    /// </summary>
    public void OnInfoFriend()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogText(DialogTextType.Title, "フレンド詳細");
        {
            string[] stateList =
            {
                "フレンド",
                "申請中",
                "承認待ち",
                "無関係",
                "プレミアム"
            };
            string mainText = "";

            PacketStructFriend[] frineds = UserDataAdmin.Instance.m_StructFriendList;

            foreach (PacketStructFriend friend in frineds)
            {
                if (friend == null)
                    continue;
                string user_id = UnityUtil.CreateDrawUserID(friend.user_id);
                mainText += "[" + stateList[friend.friend_state] + "] ID:" + user_id + "  Name:" + friend.user_name + "\n";
            }

            newDialog.SetDialogText(DialogTextType.MainText, mainText);
            newDialog.SetTextAlignment(DialogTextType.MainText, TMPro.TextAlignmentOptions.TopLeft);
        }

        newDialog.SetDialogText(DialogTextType.OKText, "＞確認");
        newDialog.Show();
    }

    /// <summary>
    /// パーティ情報表示
    /// </summary>
    public void OnInfoParty()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogText(DialogTextType.Title, "パーティ詳細");
        {
            string mainText = "";
            PacketStructPlayer player = UserDataAdmin.Instance.m_StructPlayer;

            PacketStructUnit[][] partyList = UserDataAdmin.Instance.m_StructPartyAssign;
            for (int partyid = 0; partyid < partyList.Length; partyid++)
            {
                mainText += ((player.unit_party_current == partyid) ? "*" : " ") + "Party" + partyid.ToString();
                for (int uid = 0; uid < partyList[partyid].Length; uid++)
                {
                    if (partyList[partyid][uid] != null)
                    {
                        mainText += "[" + partyList[partyid][uid].id + "]";
                    }
                    else
                    {
                        mainText += "[-]";
                    }
                }
                mainText += "\n";
            }

            newDialog.SetDialogText(DialogTextType.MainText, mainText);
            newDialog.SetTextAlignment(DialogTextType.MainText, TMPro.TextAlignmentOptions.TopLeft);
        }

        newDialog.SetDialogText(DialogTextType.OKText, "＞確認");
        newDialog.Show();
    }

    public void OnInfoPresent(RecvPresentListGetValue _data)
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogText(DialogTextType.Title, "プレゼントリスト");
        {
            string mainText = "";
            for (int i = 0; i < _data.present.Length; i++)
            {
                if (_data.present[i] != null)
                {
                    mainText += "[" + _data.present[i].serial_id.ToString() + "] " + _data.present[i].message + "\n";
                }
            }
            newDialog.SetDialogText(DialogTextType.MainText, mainText);
            newDialog.SetTextAlignment(DialogTextType.MainText, TMPro.TextAlignmentOptions.TopLeft);
        }
        newDialog.SetDialogText(DialogTextType.OKText, "＞確認");
        newDialog.Show();
    }

    public void OnInfoItem()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogText(DialogTextType.Title, "アイテム詳細");
        {
            string mainText = "";

            PacketStructPlayer player = UserDataAdmin.Instance.m_StructPlayer;

            for (int i = 0; i < player.item_list.Length; i++)
            {
                if (player.item_list[i] == null)
                    continue;
                MasterDataUseItem item = MasterDataUtil.GetMasterDataUseItemFromID(player.item_list[i].item_id);
                mainText += "[" + player.item_list[i].item_id.ToString() + "] " + item.item_name + "\n";
            }

            newDialog.SetDialogText(DialogTextType.MainText, mainText);
            newDialog.SetTextAlignment(DialogTextType.MainText, TMPro.TextAlignmentOptions.TopLeft);
        }
        newDialog.SetDialogText(DialogTextType.OKText, "＞確認");
        newDialog.Show();
    }

    public void OnInfoPointShop(RecvGetPointShopProductValue result)
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogText(DialogTextType.Title, "ポイントショップ");
        {
            string mainText = "";

            foreach (MasterDataPointShopProduct product in result.shop_product)
            {
                if (product == null)
                    continue;
                mainText += "[" + product.fix_id.ToString() + "] " + product.product_name + ": " + product.price.ToString() + "\n";
            }

            newDialog.SetDialogText(DialogTextType.MainText, mainText);
            newDialog.SetTextAlignment(DialogTextType.MainText, TMPro.TextAlignmentOptions.TopLeft);
        }
        newDialog.SetDialogText(DialogTextType.OKText, "＞確認");
        newDialog.Show();
    }

    /// <summary>
    /// ServerAPI呼び出し
    /// </summary>
    /// <param name="_api"></param>
    public void OnSendApi(SERVER_API _api)
    {
        serverApi = null;
        switch (_api)
        {
            case SERVER_API.SERVER_API_USER_CREATE:
                OnSendUserAuth();
                break;
            case SERVER_API.SERVER_API_USER_AUTHENTICATION:
                OnSendUserAuth();
                break;
            case SERVER_API.SERVER_API_MASTER_HASH_GET:
                OnSendGetMasterHash();
                break;
            case SERVER_API.SERVER_API_MASTER_DATA_GET_ALL:
                OnSendGetMasterDataAll();
                break;
#if BUILD_TYPE_DEBUG
            case SERVER_API.SERVER_API_DEBUG_MASTER_DATA_GET_ALL2:
                OnSendGetMasterDataAll2();
                break;
#endif
            case SERVER_API.SERVER_API_MASTER_DATA_GET_ALL2:
                OnSendGetMasterDataAll2();
                break;
            case SERVER_API.SERVER_API_GET_LOGIN_PACK:
                OnSendLoginPack();
                break;
            case SERVER_API.SERVER_API_USER_RENAME:
                OnSendRenameUser();
                break;
            case SERVER_API.SERVER_API_USER_SELECT_DEF_PARTY:
                OnSendSelectDefParty();
                break;
            case SERVER_API.SERVER_API_TUTORIAL_SKIP:
                OnSendSkipTutorial();
                break;
            case SERVER_API.SERVER_API_TUTORIAL:
                OnSendTutorial();
                break;
            case SERVER_API.SERVER_API_TRANSFER_ORDER:
                OnSendTransferOrder();
                break;
            case SERVER_API.SERVER_API_TRANSFER_EXEC:
                OnSendTransferExec();
                break;
            case SERVER_API.SERVER_API_GET_SNS_ID:
                OnSendGetSnsID();
                break;
            case SERVER_API.SERVER_API_SET_SNS_LINK:
                OnSendSnsLink();
                break;
            case SERVER_API.SERVER_API_CHECK_SNS_LINK:
                OnSendCheckSnsLink();
                break;
            case SERVER_API.SERVER_API_MOVE_SNS_SAVE_DATA:
                OnSendSnsSaveData();
                break;

            ///
            ///
            ///
            case SERVER_API.SERVER_API_FRIEND_LIST_GET:
                OnSendFriendListGet();
                break;
            case SERVER_API.SERVER_API_FRIEND_REQUEST:
                OnSendFriendRequest();
                break;
            case SERVER_API.SERVER_API_FRIEND_CONSENT:
                OnSendFriendConsent();
                break;
            case SERVER_API.SERVER_API_FRIEND_SEARCH:
                OnSendFriendSearch();
                break;
            case SERVER_API.SERVER_API_FRIEND_REFUSAL:
                OnSendFriendRefusal();
                break;
            case SERVER_API.SERVER_API_PRESENT_LIST_GET:
                OnSendPresentListGet();
                break;
            case SERVER_API.SERVER_API_PRESENT_OPEN:
                OnSendPresentOpen();
                break;
            case SERVER_API.SERVER_API_GACHA_TICKET_PLAY:
                OnSendGachaTicketOlay();
                break;

            ///
            ///
            ///
            case SERVER_API.SERVER_API_UNIT_PARTY_ASSIGN:
                OnSendUnitPartyAssign();
                break;
            case SERVER_API.SERVER_API_UNIT_BLEND_BUILDUP:
                OnSendUnitBlendBuildup();
                break;
            case SERVER_API.SERVER_API_QUEST_HELPER_GET_BUILD:
                OnSendHelperGetBuild();
                break;
            case SERVER_API.SERVER_API_UNIT_SALE:
                OnSendUnitSale();
                break;
            case SERVER_API.SERVER_API_UNIT_LINK_CREATE:
                OnSendUnitLinkCreate();
                break;
            case SERVER_API.SERVER_API_UNIT_LINK_DELETE:
                OnSendUnitLinkDelete();
                break;

            ///
            ///
            ///
            #region ===旧API===
            case SERVER_API.SERVER_API_QUEST_START:
                OnSendQuestStart();
                break;
            case SERVER_API.SERVER_API_QUEST_CLEAR:
                OnSendQuestClear();
                break;
            case SERVER_API.SERVER_API_QUEST_CONTINUE:
                OnSendQuestContinue();
                break;
            case SERVER_API.SERVER_API_QUEST_RETIRE:
                OnSendQuestRetire();
                break;
            case SERVER_API.SERVER_API_QUEST_RESET:
                OnSendQuestReset();
                break;
            case SERVER_API.SERVER_API_QUEST_HELPER_GET_EVOL:
                OnSendQuestHelperGetEvol();
                break;
            case SERVER_API.SERVER_API_EVOL_QUEST_START:
                OnSendEvolQuestStart();
                break;
            case SERVER_API.SERVER_API_EVOL_QUEST_CLEAR:
                OnSendEvolQuestClear();
                break;
            #endregion
            case SERVER_API.SERVER_API_GET_ACHIEVEMENT_GP:
                OnSendGetAchivementGroup();
                break;
            case SERVER_API.SERVER_API_GET_ACHIEVEMENT:
                OnSendGetAchivement();
                break;
            case SERVER_API.SERVER_API_ACHIEVEMENT_OPEN:
                OnSendAchivementOpen();
                break;
            case SERVER_API.SERVER_API_USE_ITEM:
                OnSendUseItem();
                break;
            case SERVER_API.SERVER_API_GET_GUERRILLA_BOSS_INFO:
                OnSendGuerrillaBossInfo();
                break;
            case SERVER_API.SERVER_API_QUEST2_START:
                OnSendQuest2Start();
                break;
            case SERVER_API.SERVER_API_QUEST2_CLEAR:
                OnSendQuest2Clear();
                break;
            case SERVER_API.SERVER_API_QUEST2_ORDER_GET:
                OnSendQuest2OrderGet();
                break;
            case SERVER_API.SERVER_API_QUEST2_CESSAION_QUEST:
                OnSendQuest2CessaionQuest();
                break;
            ///
            ///
            ///
            case SERVER_API.SERVER_API_GACHA_PLAY:
                OnSendGachaPlay();
                break;
            case SERVER_API.SERVER_API_STONE_USE_STAMINA:
                OnSendStoneUseStamina();
                break;
            case SERVER_API.SERVER_API_STONE_USE_UNIT:
                OnSendStoneUseUnit();
                break;
            case SERVER_API.SERVER_API_STONE_USE_FRIEND:
                OnSendStoneUseFriend();
                break;
            case SERVER_API.SERVER_API_STONE_PAY_PREV_ANDROID:
                break;
            case SERVER_API.SERVER_API_GET_POINT_SHOP_PRODUCT:
                OnSendGetPointShopProduct();
                break;
            case SERVER_API.SERVER_API_POINT_SHOP_PURCHASE:
                OnSendPointShopPurchase();
                break;
            case SERVER_API.SERVER_API_POINT_SHOP_LIMITOVER:
                OnSendPointShopLimitover();
                break;

            ///
            ///
            ///
#if BUILD_TYPE_DEBUG
            case SERVER_API.SERVER_API_DEBUG_EDIT_USER:
                OnSendDebugEditUser();
                break;
#endif
#if BUILD_TYPE_DEBUG

            case SERVER_API.SERVER_API_DEBUG_UNIT_GET:
                OnSendDebugUnitGet();
                break;
#endif
            case SERVER_API.SERVER_API_SET_CURRENT_HERO:
                OnSendSetCurrentHero();
                break;
            case SERVER_API.SERVER_API_EVOLVE_UNIT:
                OnSendEvolveUnit();
                break;
            case SERVER_API.SERVER_API_GET_GACHA_LINEUP:
                OnSendGetGachaLineup();
                break;
            case SERVER_API.SERVER_API_RENEW_TUTORIAL:
                OnSendRenewTutorial();
                break;
            case SERVER_API.SERVER_API_GET_TOPIC_INFO:
                OnSendGetTopicInfo();
                break;
            case SERVER_API.SERVER_API_GET_PRESENT_OPEN_LOG:
                OnSendGetPresentOpenLog();
                break;
            default:
                Debug.LogError("未対応:" + _api.ToString());
                break;
        }
        if (serverApi != null)
        {
            serverApi.setSuccessAction(successAction).
                setErrorAction(
                    _data =>
                    {
                        errorCodeLog(_data.m_PacketCode);
                    }).
                SendStart();
        }
    }

    private void successAction(ServerApi.ResultData _data)
    {
        switch (_data.m_PacketAPI)
        {
            case SERVER_API.SERVER_API_USER_CREATE:
                Debug.Log("USER_CREATE:Success");
                break;
            case SERVER_API.SERVER_API_USER_AUTHENTICATION:
                Debug.Log("USER_AUTHENTICATION:Success");
                break;
            case SERVER_API.SERVER_API_MASTER_HASH_GET:
                Debug.Log("MASTER_HASH_GET:Success");
                break;
            case SERVER_API.SERVER_API_MASTER_DATA_GET_ALL:
                Debug.Log("MASTER_DATA_GET_ALL:Success");
                break;
            case SERVER_API.SERVER_API_GET_LOGIN_PACK:
                Debug.Log("GET_LOGIN_PACK:Success");
                break;
            case SERVER_API.SERVER_API_USER_RENAME:
                Debug.Log("USER_RENAME:Success");
                UserDataAdmin.Instance.m_StructPlayer.user.user_name = _data.GetResult<RecvRenameUser>().result.user_name;
                updateUser(null);
                break;
            case SERVER_API.SERVER_API_USER_SELECT_DEF_PARTY:
                Debug.Log("USER_SELECT_DEF_PARTY:Success");
                updateUser(_data.UpdateStructPlayer<RecvSelectDefParty>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;
            case SERVER_API.SERVER_API_TUTORIAL_SKIP:
                Debug.Log("TUTORIAL_SKIP:Success");
                break;
            case SERVER_API.SERVER_API_TUTORIAL:
                Debug.Log("TUTORIAL:Success");
                ServerDataParam.m_RecvPacketTutorialStep = null;
                break;
            case SERVER_API.SERVER_API_TRANSFER_ORDER:
                Debug.Log("TRANSFER_ORDER:Success password:" + _data.GetResult<RecvTransferOrder>().result.password);
                break;
            case SERVER_API.SERVER_API_TRANSFER_EXEC:
                Debug.Log("TRANSFER_EXEC:Success");
                break;
            case SERVER_API.SERVER_API_GET_SNS_ID:
                Debug.Log("GET_SNS_ID:Success");
                userTransfar.snsId = _data.GetResult<RecvGetSnsID>().result.sns_id;
                break;
            case SERVER_API.SERVER_API_SET_SNS_LINK:
                Debug.Log("SET_SNS_LINK:Success");
                break;
            case SERVER_API.SERVER_API_CHECK_SNS_LINK:
                Debug.Log("CHECK_SNS_LINK:Success");
                break;
            case SERVER_API.SERVER_API_MOVE_SNS_SAVE_DATA:
                Debug.Log("SNS_SAVE_DATA:Success");
                break;

            ///
            ///
            ///
            case SERVER_API.SERVER_API_FRIEND_LIST_GET:
                Debug.Log("FRIEND_LIST_GET:Success");
                UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(_data.GetResult<RecvFriendListGet>().result.friend);
                break;
            case SERVER_API.SERVER_API_FRIEND_REQUEST:
                Debug.Log("FRIEND_REQUEST:Success");
                UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(_data.GetResult<RecvFriendRequest>().result.friend);
                break;
            case SERVER_API.SERVER_API_FRIEND_CONSENT:
                Debug.Log("FRIEND_CONSENT:Success");
                UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(_data.GetResult<RecvFriendConsent>().result.friend);
                break;
            case SERVER_API.SERVER_API_FRIEND_REFUSAL:
                Debug.Log("FRIEND_REFUSAL:Success");
                UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(_data.GetResult<RecvFriendRefusal>().result.friend);
                break;
            case SERVER_API.SERVER_API_FRIEND_SEARCH:
                Debug.Log("FRIEND_SEARCH:Success [" + _data.GetResult<RecvFriendSearch>().result.friend.user_name + "]");
                break;
            case SERVER_API.SERVER_API_PRESENT_LIST_GET:
                Debug.Log("PRESENT_LIST_GET:Success");
                OnInfoPresent(_data.GetResult<RecvPresentListGet>().result);
                break;
            case SERVER_API.SERVER_API_PRESENT_OPEN:
                Debug.Log("PRESENT_OPEN:Success");
                updateUser(_data.UpdateStructPlayer<RecvPresentOpen>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;

            ///
            ///
            ///
            case SERVER_API.SERVER_API_UNIT_PARTY_ASSIGN:
                Debug.Log("UNIT_PARTY_ASSIGN:Success");
                updateUser(_data.UpdateStructPlayer<RecvUnitPartyAssign>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;
            case SERVER_API.SERVER_API_UNIT_BLEND_BUILDUP:
                Debug.Log("UNIT_BLEND_BUILDUP:Success");
                updateUser(_data.UpdateStructPlayer<RecvUnitBlendBuildUp>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;
            case SERVER_API.SERVER_API_QUEST_HELPER_GET_BUILD:
                Debug.Log("QUEST_HELPER_GET_BUILD:Success");
                break;
            case SERVER_API.SERVER_API_UNIT_SALE:
                Debug.Log("UNIT_SALE:Success");
                updateUser(_data.UpdateStructPlayer<RecvUnitSale>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;
            case SERVER_API.SERVER_API_UNIT_LINK_CREATE:
                Debug.Log("UNIT_LINK_CREATE:Success");
                updateUser(_data.UpdateStructPlayer<RecvUnitLink>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;
            case SERVER_API.SERVER_API_UNIT_LINK_DELETE:
                Debug.Log("UNIT_LINK_DELETE:Success");
                updateUser(_data.UpdateStructPlayer<RecvUnitLink>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;

            ///
            ///
            ///
            case SERVER_API.SERVER_API_QUEST_START:
                Debug.Log("QUEST_START:Success");
                break;
            case SERVER_API.SERVER_API_QUEST_CLEAR:
                Debug.Log("QUEST_CLEAR:Success");
                updateUser(_data.UpdateStructPlayer<RecvQuestClear>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;
            case SERVER_API.SERVER_API_QUEST_RETIRE:
                Debug.Log("QUEST_RETIRE:Success");
                break;
            case SERVER_API.SERVER_API_QUEST_CONTINUE:
                Debug.Log("QUEST_CONTINUE:Success");
                break;
            case SERVER_API.SERVER_API_QUEST_RESET:
                Debug.Log("QUEST_RESET:Success");
                break;
            case SERVER_API.SERVER_API_QUEST_HELPER_GET_EVOL:
                Debug.Log("QUEST_HELPER_GET_EVOL:Success");
                break;
            case SERVER_API.SERVER_API_EVOL_QUEST_START:
                Debug.Log("EVOL_QUEST_START:Success");
                break;
            case SERVER_API.SERVER_API_EVOL_QUEST_CLEAR:
                Debug.Log("EVOL_QUEST_CLEAR:Success");
                updateUser(_data.UpdateStructPlayer<RecvEvolQuestClear>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;
            case SERVER_API.SERVER_API_GET_ACHIEVEMENT_GP:
                Debug.Log("GET_ACHIEVEMENT_GP:Success");
                break;
            case SERVER_API.SERVER_API_GET_ACHIEVEMENT:
                Debug.Log("GET_ACHIEVEMENT:Success");
                break;
            case SERVER_API.SERVER_API_ACHIEVEMENT_OPEN:
                Debug.Log("ACHIEVEMENT_OPEN:Success");
                break;
            case SERVER_API.SERVER_API_USE_ITEM:
                Debug.Log("SERVER_API_USE_ITEM:Success");
                break;
            case SERVER_API.SERVER_API_GET_GUERRILLA_BOSS_INFO:
                Debug.Log("SERVER_API_GET_GUERRILLA_BOSS_INFO:Success");
                {
                    RecvGetGuerrillaBossInfoValue _infoValue = _data.GetResult<RecvGetGuerrillaBossInfo>().result;
                    if (_infoValue.guerrilla_boss_list != null)
                    {
                        for (int i = 0; i < _infoValue.guerrilla_boss_list.Length; i++)
                        {
                            Debug.Log("area_id:" + _infoValue.guerrilla_boss_list[i].area_id.ToString() + " boss_count:" + _infoValue.guerrilla_boss_list[i].boss_id_list.Length.ToString());
                        }
                    }
                }
                break;
            case SERVER_API.SERVER_API_QUEST2_START:
                Debug.Log("QUEST2_START:Success");
                break;
            case SERVER_API.SERVER_API_QUEST2_CLEAR:
                Debug.Log("QUEST2_CLEAR:Success");
                break;
            case SERVER_API.SERVER_API_QUEST2_ORDER_GET:
                Debug.Log("QUEST2_ORDER_GET:Success");
                break;
            case SERVER_API.SERVER_API_QUEST2_CESSAION_QUEST:
                Debug.Log("QUEST2_CESSAION_QUEST_GET:Success");
                break;
            ///
            ///
            ///
            case SERVER_API.SERVER_API_GACHA_PLAY:
                Debug.Log("GACHA_PLAY:Success");
                updateUser(_data.GetResult<RecvDebugUnitGet>().result.player);
                break;
            case SERVER_API.SERVER_API_GACHA_TICKET_PLAY:
                Debug.Log("GACHA_TICKET_PLAY:Success");
                updateUser(_data.UpdateStructPlayer<RecvGachaTicketPlay>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;
            case SERVER_API.SERVER_API_STONE_USE_STAMINA:
                Debug.Log("STONE_USE_STAMINA:Success");
                updateUser(_data.UpdateStructPlayer<RecvStoneUsedStamina>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;
            case SERVER_API.SERVER_API_STONE_USE_UNIT:
                Debug.Log("STONE_USE_UNIT:Success");
                updateUser(_data.UpdateStructPlayer<RecvStoneUsedUnit>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;
            case SERVER_API.SERVER_API_STONE_USE_FRIEND:
                Debug.Log("STONE_USE_FRIEND:Success");
                updateUser(_data.UpdateStructPlayer<RecvStoneUsedFriend>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                break;
            case SERVER_API.SERVER_API_GET_POINT_SHOP_PRODUCT:
                Debug.Log("GET_POINT_SHOP_PRODUCT:Success");
                OnInfoPointShop(_data.GetResult<RecvGetPointShopProduct>().result);
                break;

            ///
            ///
            ///
#if BUILD_TYPE_DEBUG
            case SERVER_API.SERVER_API_DEBUG_EDIT_USER:
                Debug.Log("DEBUG_EDIT_USER:Success");
                updateUser(_data.GetResult<RecvDebugEditUser>().result.player);
                ServerDataParam.m_RecvPacketDebugEditUser = null;
                break;
#endif
#if BUILD_TYPE_DEBUG
            case SERVER_API.SERVER_API_DEBUG_UNIT_GET:
                Debug.Log("DEBUG_UNIT_GET:Success");
                updateUser(_data.GetResult<RecvDebugUnitGet>().result.player);
                ServerDataParam.m_RecvPacketDebugUnitGet = null;
                break;
#endif
            case SERVER_API.SERVER_API_EVOLVE_UNIT:
                Debug.Log("SERVER_API_EVOLVE_UNIT:Success");
                break;
            case SERVER_API.SERVER_API_RENEW_TUTORIAL:
                Debug.Log("RENEW_TUTORIAL:Success");
                ServerDataParam.m_RecvPacketRenewTutorialStep = null;
                break;
            case SERVER_API.SERVER_API_GET_TOPIC_INFO:
                Debug.Log("SERVER_API_GET_TOPIC_INFO:Success");
                ServerDataParam.m_RecvPacketGetTopicInfo = null;
                break;
            case SERVER_API.SERVER_API_GET_PRESENT_OPEN_LOG:
                Debug.Log("SERVER_API_GET_PRESENT_OPEN_LOG:Success");
                ServerDataParam.m_RecvGetPresentOpenLog = null;
                break;

            default:
                break;
        }
    }

    private void OnSendDebugUnitGet()
    {
        PacketStructUnitGetDebug[] getUnits = makeDebugUnitArray();
        if (getUnits != null &&
            getUnits.Length >= 1)
        {
            serverApi = ServerDataUtilSend.SendPacketAPI_DebugUnitGet(getUnits);
        }
    }

    private void OnSendDebugEditUser()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_DebugEditUser(
            debugPlayerParam.GameMoney,
            debugPlayerParam.FreePaidPoint,
            debugPlayerParam.FriendPoint,
            debugPlayerParam.Rank,
            debugPlayerParam.BuyMaxUnitCount,
            debugPlayerParam.BuyMaxFriendCount,
            debugPlayerParam.TicketCasino,
            debugPlayerParam.UnitPoint,
            0, 0,
            (debugPlayerParam.AllQuestClear ? 1 : 0),
            0
        );
    }

    private void OnSendPointShopLimitover()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_PointShopLimitOver(
            pointShop.productId,
            pointShop.unitUniqueId,
            pointShop.limitOverCount
        );
    }

    private void OnSendPointShopPurchase()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_PointShopPurchase(pointShop.productId);
    }

    private void OnSendGetPointShopProduct()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_GetPointShopProduct();
    }

    private void OnSendStoneUseFriend()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_StoneUsedFriend();
    }

    private void OnSendStoneUseUnit()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_StoneUsedUnit();
    }

    private void OnSendStoneUseStamina()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_StoneUsedStamina();
    }

    private void OnSendGachaPlay()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_GachaPlay(
            gacha.Id,
            gacha.Count,
            0
        );
    }

    private void OnSendUseItem()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_ItemUse(useItemId);
    }

    private void OnSendAchivementOpen()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_AchievementOpen(
            achievementOpen.AchievementIdArray,
            achievementOpen.AchievementGroupIdArray
        );
    }

    private void OnSendGetAchivement()
    {
        ServerDataUtilSend.SendPacketAPI_GetMasterDataAchievement(
            achievement.GroupId,
            achievement.Page,
            achievement.SortType
        ).
            setSuccessAction(
                _data =>
                {
                    recvMasterDataAchievementValueTest = (RecvMasterDataAchievementValue)_data.GetResult<RecvMasterDataAchievement>().result;
                }).
            setErrorAction(
                _data =>
                {
                    errorCodeLog(_data.m_PacketCode);
                }).
            SendStart();
    }

    private void OnSendGetAchivementGroup()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_GetAchievementGroup(
            achievementGroup.Category,
            achievementGroup.Page,
            achievementGroup.SortType
        );
    }

    private void OnSendEvolQuestClear()
    {
        PacketStructUnitGet[] getUnit = new PacketStructUnitGet[1];
        //int[] openPanel = new int[25]{ 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25};
        int[] openPanel = new int[1]
        {
            3
        };
        serverApi = ServerDataUtilSend.SendPacketAPI_QuestEvolClear(
            evolQuestStart.unQuestID,
            0,
            0,
            getUnit,
            openPanel,
            0
        );
    }

    private void OnSendEvolQuestStart()
    {
        PacketStructFriend cHelper = getHelper();
        if (cHelper == null)
        {
            Debug.LogError("Helper not found!!");
            return;
        }
        evolQuestStart.unEvolUnitUniqueIDBase = CheckUnitUniqueId(evolQuestStart.unEvolUnitUniqueIDBase);
        for (int i = 0; i < evolQuestStart.aunEvolUnitUniqueIDParts.Length; i++)
        {
            evolQuestStart.aunEvolUnitUniqueIDParts[i] = CheckUnitUniqueId(evolQuestStart.aunEvolUnitUniqueIDParts[i]);
        }

        PacketStructUnit baseUnit = getUnitFromUniqueId(evolQuestStart.unEvolUnitUniqueIDBase);
        if (baseUnit == null)
        {
            Debug.LogError("Unit not found!!");
            return;
        }

        MasterDataParamCharaEvol evol = MasterDataUtil.GetCharaEvolParamFromCharaID(baseUnit.id);
        if (evol == null)
        {
            Debug.LogError("Evol not found!!");
            return;
        }

        evolQuestStart.unQuestID = evol.quest_id;

        serverApi = ServerDataUtilSend.SendPacketAPI_QuestEvolStart(
            evolQuestStart.unQuestID,
            evolQuestStart.unQuestState,
            cHelper.user_id,
            cHelper.unit,
            false,
            false,
            evolQuestStart.unEvolUnitUniqueIDBase,
            evolQuestStart.aunEvolUnitUniqueIDParts,
            evol.fix_id,
            evol.unit_id_after,
            null,
            evolQuestStart.unEventFP,
            0
        );
    }

    private void OnSendEvolveUnit()
    {
        evolveUnit.unEvolUnitUniqueIDBase = CheckUnitUniqueId(evolveUnit.unEvolUnitUniqueIDBase);
        for (int i = 0; i < evolveUnit.aunEvolUnitUniqueIDParts.Length; i++)
        {
            evolveUnit.aunEvolUnitUniqueIDParts[i] = CheckUnitUniqueId(evolveUnit.aunEvolUnitUniqueIDParts[i]);
        }

        PacketStructUnit baseUnit = getUnitFromUniqueId(evolveUnit.unEvolUnitUniqueIDBase);
        if (baseUnit == null)
        {
            Debug.LogError("Unit not found!!");
            return;
        }

        MasterDataParamCharaEvol evol = MasterDataUtil.GetCharaEvolParamFromCharaID(baseUnit.id);
        if (evol == null)
        {
            Debug.LogError("Evol not found!!");
            return;
        }
        serverApi = ServerDataUtilSend.SendPacketAPI_Evolve_Unit(
            evolveUnit.unEvolUnitUniqueIDBase,
            evolveUnit.aunEvolUnitUniqueIDParts,
            evol.fix_id,
            evol.unit_id_after,
            evolveUnit.nBeginnerBoostID
        );
    }

    private void OnSendQuestHelperGetEvol()
    {
        evolQuestStart.unEvolUnitUniqueIDBase = CheckUnitUniqueId(evolQuestStart.unEvolUnitUniqueIDBase);
        PacketStructUnit baseUnit = getUnitFromUniqueId(evolQuestStart.unEvolUnitUniqueIDBase);
        if (baseUnit == null)
        {
            Debug.LogError("Unit not found!!");
            return;
        }

        MasterDataParamCharaEvol evol = MasterDataUtil.GetCharaEvolParamFromCharaID(baseUnit.id);
        if (evol == null)
        {
            Debug.LogError("Evol not found!!");
            return;
        }
        serverApi = ServerDataUtilSend.SendPacketAPI_QuestHelperGetEvol(
            evolQuestStart.unEvolUnitUniqueIDBase,
            evol.fix_id
        );
    }

    private void OnSendQuestReset()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_QuestReset(1, 1);
    }

    private void OnSendQuestRetire()
    {
        bool is_auto_play = false;
        serverApi = ServerDataUtilSend.SendPacketAPI_QuestRetire(questStart.unQuestID, is_auto_play);
    }

    private void OnSendQuestContinue()
    {
        bool is_auto_play = false;
        serverApi = ServerDataUtilSend.SendPacketAPI_QuestContinue(1, is_auto_play);
    }

    private void OnSendQuestClear()
    {
#if false// TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
        PacketStructUnitGet[] getUnit = new PacketStructUnitGet[1];
        //int[] openPanel = new int[25]{ 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25};
        int[] openPanel = new int[1]
        {
            3
        };
        MasterDataQuest quest = MasterFinder<MasterDataQuest>.Instance.Find((int)questStart.unQuestID);
        if (quest == null)
            return;
        serverApi = ServerDataUtilSend.SendPacketAPI_QuestClear(
            questStart.unQuestID,
            (uint)quest.clear_exp,
            (uint)quest.clear_money,
            getUnit,
            openPanel
        );
#endif
    }

    private void OnSendQuestStart()
    {
        //助っ人設定
        PacketStructFriend cHelper = getHelper();
        if (cHelper == null)
        {
            Debug.LogError("Helper not found!!");
            return;
        }

        serverApi = ServerDataUtilSend.SendPacketAPI_QuestStart(
            questStart.unQuestID,
            questStart.unQuestState,
            cHelper.user_id,
            cHelper.unit,
            false,
            UserDataAdmin.Instance.m_StructPlayer.unit_party_current,
            null,
            0,
            0,
            null,
            cHelper.unit_link
        );
    }

    private void OnSendQuest2Clear()
    {
        SendEnemyKill[] enemy_kill_list = null;
        bool no_damage = false;
        int max_damage = 0;
        SendSkillExecCount[] active_skill_execute_count = null;
        int hero_skill_execute_count = 0;
        ServerDataDefine.PlayScoreInfo play_score_info = new PlayScoreInfo();
        bool is_auto_play = false;
        serverApi = ServerDataUtilSend.SendPacketAPI_Quest2Clear(
            questStart.unQuestID,
            enemy_kill_list,
            no_damage,
            max_damage,
            active_skill_execute_count,
            hero_skill_execute_count,
            play_score_info,
            is_auto_play
        );
    }
    private void OnSendQuest2OrderGet()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_Quest2OrderGet(
            questStart.orderGetDetail
        );
    }

    private void OnSendQuest2CessaionQuest()
    {
        bool is_auto_play = false;
        serverApi = ServerDataUtilSend.SendPacketAPI_Quest2CessaionQuest(is_auto_play);
    }
    private void OnSendQuest2Start()
    {
        //助っ人設定
        PacketStructFriend cHelper = getHelper();
        if (cHelper == null)
        {
            Debug.LogError("Helper not found!!");
            return;
        }

        serverApi = ServerDataUtilSend.SendPacketAPI_Quest2Start(
            questStart.unQuestID,
            questStart.unQuestState,
            cHelper.user_id,
            cHelper.unit,
            false,
            UserDataAdmin.Instance.m_StructPlayer.unit_party_current,
            0,
            0,
            null,
            false
        );
    }

    private void OnSendUnitLinkDelete()
    {
        unitlink.unUnitUniqueIDBase = CheckUnitUniqueId(unitlink.unUnitUniqueIDBase);
        long[] parts = new long[1]; //いまのところお金しか必要じゃない
        serverApi = ServerDataUtilSend.SendPacketAPI_UnitLinkDelete(
            unitlink.unUnitUniqueIDBase,
            parts,
            0
        );
    }

    private void OnSendUnitLinkCreate()
    {
        unitlink.unUnitUniqueIDBase = CheckUnitUniqueId(unitlink.unUnitUniqueIDBase);
        unitlink.unUnitUniqueIDLink = CheckUnitUniqueId(unitlink.unUnitUniqueIDLink);
        for (int i = 0; i < unitlink.aunUnitUniqueIDParts.Length; i++)
        {
            unitlink.aunUnitUniqueIDParts[i] = CheckUnitUniqueId(unitlink.aunUnitUniqueIDParts[i]);
        }
        serverApi = ServerDataUtilSend.SendPacketAPI_UnitLinkCreate(
            unitlink.unUnitUniqueIDBase,
            unitlink.unUnitUniqueIDLink,
            unitlink.aunUnitUniqueIDParts,
            0
        );
    }

    private void OnSendUnitSale()
    {
        for (int i = 0; i < saleUnit.Length; i++)
        {
            saleUnit[i] = CheckUnitUniqueId(saleUnit[i]);
        }
        serverApi = ServerDataUtilSend.SendPacketAPI_UnitSale(saleUnit);
    }

    private void OnSendHelperGetBuild()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_QuestHelperGetBuild(unitbuildup.unUnitUniqueIDBase);
    }

    private void OnSendUnitBlendBuildup()
    {
        unitbuildup.unUnitUniqueIDBase = CheckUnitUniqueId(unitbuildup.unUnitUniqueIDBase);
        for (int i = 0; i < unitbuildup.aunUnitUniqueIDParts.Length; i++)
            unitbuildup.aunUnitUniqueIDParts[i] = CheckUnitUniqueId(unitbuildup.aunUnitUniqueIDParts[i]);
        serverApi = ServerDataUtilSend.SendPacketAPI_UnitBlendBuildUp(
            unitbuildup.unUnitUniqueIDBase,
            unitbuildup.aunUnitUniqueIDParts,
            unitbuildup.unEventSLV,
            unitbuildup.nBeginnerBoostID,
            (unitbuildup.bTutorialActive ? 1 : 0),
            is_premium
        );
    }

    private void OnSendUnitPartyAssign()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_UnitPartyAssign(party.assign, party.currentParty);
    }

    private void OnSendGachaTicketOlay()
    {
        if (presentSerialID.Length > 0)
        {
            for (int i = 0; i < presentSerialID.Length; i++)
                presentSerialID[i] = CheckUnitUniqueId(presentSerialID[i]);
            serverApi = ServerDataUtilSend.SendPacketAPI_GachaTicketPlay(presentSerialID[0]);
        }
    }

    private void OnSendPresentOpen()
    {
        if (presentSerialID.Length > 0)
        {
            for (int i = 0; i < presentSerialID.Length; i++)
                presentSerialID[i] = CheckUnitUniqueId(presentSerialID[i]);
            serverApi = ServerDataUtilSend.SendPacketAPI_PresentOpen(presentSerialID);
        }
    }

    private void OnSendPresentListGet()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_PresentListGet();
    }

    private void OnSendFriendRefusal()
    {
        if (friendList != null &&
            friendList.Length >= 1)
        {
            uint[] ulist = new uint[friendList.Length];
            for (int i = 0; i < friendList.Length; i++)
            {
                ulist[i] = UnityUtil.CreateFriendUserID(friendList[i]);
            }
            serverApi = ServerDataUtilSend.SendPacketAPI_FriendRefusal(ulist);
        }
        else
        {
            Debug.LogError("FriendList not found!!");
        }
    }

    private void OnSendFriendSearch()
    {
        uint searchId = UnityUtil.CreateFriendUserID(searchFriendId);
        serverApi = ServerDataUtilSend.SendPacketAPI_FriendSearch(searchId);
    }

    private void OnSendFriendConsent()
    {
        if (friendList != null &&
            friendList.Length >= 1)
        {
            uint[] ulist = new uint[friendList.Length];
            for (int i = 0; i < friendList.Length; i++)
            {
                ulist[i] = UnityUtil.CreateFriendUserID(friendList[i]);
            }
            serverApi = ServerDataUtilSend.SendPacketAPI_FriendConsent(ulist);
        }
        else
        {
            Debug.LogError("FriendList not found!!");
        }
    }

    private void OnSendFriendRequest()
    {
        if (friendList != null &&
            friendList.Length >= 1)
        {
            uint[] ulist = new uint[friendList.Length];
            for (int i = 0; i < friendList.Length; i++)
            {
                ulist[i] = UnityUtil.CreateFriendUserID(friendList[i]);
            }
            serverApi = ServerDataUtilSend.SendPacketAPI_FriendRequest(ulist);
        }
        else
        {
            Debug.LogError("FriendList not found!!");
        }
    }

    private void OnSendFriendListGet()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_FriendListGet();
    }

    private void OnSendSnsSaveData()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_MoveSnsSaveData(userTransfar.snsId);
    }

    private void OnSendCheckSnsLink()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_CheckSnsLink(userTransfar.snsId);
    }

    private void OnSendSnsLink()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_SetSnsLink(userTransfar.snsId);
    }

    private void OnSendGetSnsID()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_GetSnsID();
    }

    private void OnSendTransferExec()
    {
        uint userid = UnityUtil.CreateFriendUserID(userTransfar.userId);
        serverApi = ServerDataUtilSend.SendPacketAPI_TransferExec(userid,
                                                                  userTransfar.password,
                                                                  LocalSaveManager.Instance.LoadFuncUUID());
    }

    private void OnSendTransferOrder()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_TransferOrder();
    }

    private void OnSendTutorial()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_Tutorial((uint)tutorialStep);
    }
    private void OnSendRenewTutorial()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_RenewTutorial((uint)tutorialStep);
    }

    private void OnSendSelectDefParty()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_SelectDefParty((uint)defaultParty);
    }

    private void OnSendRenameUser()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_RenameUser(renameData);
    }

    private void OnSendSetCurrentHero()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_SetCurrentHero(heroId, uniqueId);
    }

    private void OnSendGuerrillaBossInfo()
    {
        serverApi = ServerDataUtilSend.SendPacketAPI_GetGuerrillaBossInfo(guerrilla_boss_area_id);
    }

    private void OnSendGetGachaLineup()
    {
        ServerDataUtilSend.SendPacketAPI_GetGachaLineup(get_gacha_lineup_gacha_id, 0, 0).
            setSuccessAction(
                _data =>
                {
                    recvGachaLineup = _data.GetResult<RecvGetGachaLineup>().result.gacha_assign_unit_list;
                }).
            setErrorAction(
                _data =>
                {
                    errorCodeLog(_data.m_PacketCode);
                }).
            SendStart();
    }

    /// <summary>
    /// ユーザー認証
    /// </summary>
    public void OnSendUserAuth()
    {
        //----------------------------------------
        // ユーザー認証APIでのユーザー認証要請のイレギュラー対応を追加。
        //----------------------------------------
        ServerDataManager.Instance.ResultCodeAddIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED, API_CODETYPE.API_CODETYPE_USUALLY);
        ServerDataManager.Instance.ResultCodeAddIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED2, API_CODETYPE.API_CODETYPE_USUALLY);

        bool bUUIDCheck = false;
        if (LocalSaveManager.Instance != null)
        {
            bUUIDCheck = LocalSaveManager.Instance.CheckUUID();
        }

        //----------------------------------------
        // ユーザー認証リクエスト発行
        //----------------------------------------
        if (bUUIDCheck == false)
        {
            ServerDataUtilSend.SendPacketAPI_UserCreate(ref serverUUID).
                setSuccessAction(
                    _data =>
                    {
                        Debug.Log("USER_CREATE:Success");
                        LocalSaveManager.Instance.SaveFuncUUID(serverUUID);

                        updateUser(_data.UpdateStructPlayer<RecvCreateUser>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                        UserDataAdmin.Instance.m_StructSystem = _data.GetResult<RecvCreateUser>().result.system;
                        UserDataAdmin.Instance.m_StructHeroList = _data.GetResult<RecvCreateUser>().result.hero_list;
                    }).
                setErrorAction(
                    _data =>
                    {
                        errorCodeLog(_data.m_PacketCode);
                    }).
                SendStart();
        }
        else
        {
            ServerDataUtilSend.SendPacketAPI_UserAuthentication().
                setSuccessAction(
                    (_data) =>
                    {
                        Debug.Log("USER_AUTHENTICATION:Success");

                        updateUser(_data.UpdateStructPlayer<RecvUserAuthentication>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
                        UserDataAdmin.Instance.m_StructSystem = _data.GetResult<RecvUserAuthentication>().result.system;
                        UserDataAdmin.Instance.m_StructQuest = _data.GetResult<RecvUserAuthentication>().result.quest;
                        UserDataAdmin.Instance.m_StructHeroList = _data.GetResult<RecvUserAuthentication>().result.hero_list;
                    }).
                setErrorAction(
                    (_data) =>
                    {
                        errorCodeLog(_data.m_PacketCode);
                    }).
                SendStart();
        }
    }

    private void OnSendGetMasterHash()
    {
        ServerDataUtilSend.SendPacketAPI_GetMasterHash().
            setSuccessAction(
                _data =>
                {
                    //_data.GetResult<RecvGetMasterHash>().result;
                    Debug.Log("MASTER_HASH_GET:Success");
                }).
            setErrorAction(
                _data =>
                {
                    errorCodeLog(_data.m_PacketCode);
                }).
            SendStart();
    }

    private void OnSendGetMasterDataAll2()
    {
        Dictionary<EMASTERDATA, uint> dict = new Dictionary<EMASTERDATA, uint>();

        for (int i = 0; i < getMasterDataList.Length; ++i)
        {
            dict.Add(getMasterDataList[i], getMasterDataTagIdList[i]);
        }

        ServerDataUtilSend.SendPacketAPI_GetMasterDataAll2(dict).
            setSuccessAction(
                _data =>
                {
                    Debug.Log("MASTER_DATA_GET_ALL2:Success");

                    RecvMasterDataAll2Value v = _data.GetResult<RecvMasterDataAll2>().result;
                    RecvMasterDataAll2Value result = v;
                    //                    Debug.LogError("C:" + v.master_array_default_party.upd_list.Count());
                    //                    Debug.LogError("V:" + v.master_array_default_party.upd_list[0].party_chara0_id);

                    List<BaseMasterDiff<Master>> list = new List<BaseMasterDiff<Master>>();



                    foreach (FieldInfo fi in typeof(RecvMasterDataAll2Value).GetFields())
                    {
                        //                        Debug.LogError("FI:" + fi.ToString());
                        object obj = fi.GetValue(result);
                        //                        Debug.LogError("OBJ:" + obj);
                        if (obj == null)
                        {
                            continue;
                        }

                        //                        if(typeof(Master).GetType().get

                        //                        Debug.LogError("TYP:" + obj.GetType());
                        //                        Debug.LogError("TYP2:" + typeof(BaseMasterDiff<Master>));



                        if (obj.GetType() == typeof(MasterDataDefaultPartyDiff))
                        {
                            //MasterDataDefaultPartyDiff o = obj as MasterDataDefaultPartyDiff;
                            Debug.LogError("HIT2");
                            //                            SQLiteClient.Instance.UpdateOrCreate(o);

                        }
                        if (obj.GetType() == typeof(MasterDataDefaultPartyDiff))
                        {
                            Debug.LogError("HIT2");
                        }

                        if (obj.GetType().IsSubclassOf(typeof(BaseMasterDiff<Master>)))
                        {
                            Debug.LogError("HIT");
                            Debug.LogError("TYPE:" + obj.GetType().ToString());
                            list.Add((BaseMasterDiff<Master>)obj);
                        }
                    }

                    //			if(masterDataOutput)outputMkwasterData(_data.GetResult<RecvMasterDataAll>().result);
                }).
            setErrorAction(
                _data =>
                {
                    errorCodeLog(_data.m_PacketCode);
                }).
            SendStart();
    }

    private void OnSendGetMasterDataAll()
    {
        List<EMASTERDATA> masterList = new List<EMASTERDATA>();
        foreach (EMASTERDATA _data in getMasterDataList)
        {
            masterList.Add(_data);
        }
        ServerDataUtilSend.SendPacketAPI_GetMasterDataAll(masterList).
            setSuccessAction(
                _data =>
                {
                    Debug.Log("MASTER_DATA_GET_ALL:Success");
                    //			if(masterDataOutput)outputMasterData(_data.GetResult<RecvMasterDataAll>().result);
                }).
            setErrorAction(
                _data =>
                {
                    errorCodeLog(_data.m_PacketCode);
                }).
            SendStart();
    }

    private void OnSendLoginPack()
    {
        ServerDataUtilSend.SendPacketAPI_LoginPack(true, true, true, false).
            setSuccessAction(
                _data =>
                {
                    Debug.Log("GET_LOGIN_PACK:Success");
                    UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(_data.GetResult<RecvLoginPack>().result.result_friend.friend);
                    UserDataAdmin.Instance.m_StructHelperList = UserDataAdmin.FriendListClipMe(_data.GetResult<RecvLoginPack>().result.result_helper.friend);
                    MainMenuUtil.setupLoginPack(_data.GetResult<RecvLoginPack>().result);
                }).
            setErrorAction(
                _data =>
                {
                    errorCodeLog(_data.m_PacketCode);
                }).
            SendStart();
    }

    private void OnSendSkipTutorial()
    {
        ServerDataUtilSend.SendPacketAPI_SkipTutorial((uint)defaultParty).
            setSuccessAction(
                _data =>
                {
                    Debug.Log("TUTORIAL_SKIP:Success");
                    updateUser(_data.GetResult<RecvSkipTutorial>().result.player);
                }).
            setErrorAction(
                _data =>
                {
                    errorCodeLog(_data.m_PacketCode);
                }).
            SendStart();
    }
    private void OnSendGetTopicInfo()
    {
        ServerDataUtilSend.SendPacketAPI_GetTopicInfo()
        .setSuccessAction(
                _data =>
                {
                    Debug.Log("SERVER_API_GET_TOPIC_INFO:Success");
                    recvGetTopicInfoValue = _data.GetResult<RecvGetTopicInfo>().result;
                }).
            setErrorAction(
                _data =>
                {
                    errorCodeLog(_data.m_PacketCode);
                }).
            SendStart();

    }
    private void OnSendGetPresentOpenLog()
    {
        ServerDataUtilSend.SendPacketAPI_GetPresentOpenLog()
        .setSuccessAction(
                _data =>
                {
                    Debug.Log("SERVER_API_GET_PRESENT_OPEN_LOG:Success");
                    recvGetPresentOpenLogValue = _data.GetResult<RecvGetPresentOpenLog>().result;
                }).
            setErrorAction(
                _data =>
                {
                    errorCodeLog(_data.m_PacketCode);
                }).
            SendStart();

    }

    /// <summary>
    /// ユニット取得リスト作成
    /// </summary>
    /// <returns></returns>
    public PacketStructUnitGetDebug[] makeDebugUnitArray()
    {
        List<PacketStructUnitGetDebug> units = new List<PacketStructUnitGetDebug>();

        MasterDataParamChara chara = MasterFinder<MasterDataParamChara>.Instance.Find(debugUnitGet.id);
        if (chara != null)
        {
            //base Unit
            PacketStructUnitGetDebug newUnit = new PacketStructUnitGetDebug();
            newUnit.id = (uint)debugUnitGet.id;
            newUnit.add_hp = 0;
            newUnit.add_pow = 0;
            newUnit.add_def = 0;
            switch (debugUnitGet.status)
            {
                case GetUnitStatus.Normal:
                    newUnit.level = 1;
                    break;
                case GetUnitStatus.LevelMax:
                    newUnit.level = (uint)chara.level_max;
                    break;
            }
            newUnit.limitbreak_lv = 0;
            newUnit.limitover_lv = 0;
            units.Add(newUnit);

            //進化ユニット取得
            if (debugUnitGet.evolve)
            {
                MasterDataParamCharaEvol cEvolMasterData = MasterDataUtil.GetCharaEvolParamFromCharaID((uint)debugUnitGet.id);
                if (cEvolMasterData != null)
                {
                    if (cEvolMasterData.unit_id_parts1 != 0)
                    {
                        PacketStructUnitGetDebug cUnitGet = new PacketStructUnitGetDebug();
                        cUnitGet.id = cEvolMasterData.unit_id_parts1;
                        cUnitGet.limitbreak_lv = (uint)1;
                        cUnitGet.level = (uint)1;
                        cUnitGet.add_hp = (uint)0;
                        cUnitGet.add_pow = (uint)0;
                        units.Add(cUnitGet);
                    }
                    if (cEvolMasterData.unit_id_parts2 != 0)
                    {
                        PacketStructUnitGetDebug cUnitGet = new PacketStructUnitGetDebug();
                        cUnitGet.id = cEvolMasterData.unit_id_parts2;
                        cUnitGet.limitbreak_lv = (uint)1;
                        cUnitGet.level = (uint)1;
                        cUnitGet.add_hp = (uint)0;
                        cUnitGet.add_pow = (uint)0;
                        units.Add(cUnitGet);
                    }
                    if (cEvolMasterData.unit_id_parts3 != 0)
                    {
                        PacketStructUnitGetDebug cUnitGet = new PacketStructUnitGetDebug();
                        cUnitGet.id = cEvolMasterData.unit_id_parts3;
                        cUnitGet.limitbreak_lv = (uint)1;
                        cUnitGet.level = (uint)1;
                        cUnitGet.add_hp = (uint)0;
                        cUnitGet.add_pow = (uint)0;
                        units.Add(cUnitGet);
                    }
                }
            }

            //リンクユニット取得
            if (debugUnitGet.link)
            {
                MasterDataParamChara cCLinkharaMasterData = MasterDataUtil.GetCharaParamFromID((uint)debugUnitGet.id);
                if (cCLinkharaMasterData != null)
                {
                    if (cCLinkharaMasterData.link_unit_id_parts1 != 0)
                    {
                        PacketStructUnitGetDebug cUnitGet = new PacketStructUnitGetDebug();
                        cUnitGet.id = cCLinkharaMasterData.link_unit_id_parts1;
                        cUnitGet.limitbreak_lv = (uint)1;
                        cUnitGet.level = (uint)1;
                        cUnitGet.add_hp = (uint)0;
                        cUnitGet.add_pow = (uint)0;
                        units.Add(cUnitGet);
                    }
                    if (cCLinkharaMasterData.link_unit_id_parts2 != 0)
                    {
                        PacketStructUnitGetDebug cUnitGet = new PacketStructUnitGetDebug();
                        cUnitGet.id = cCLinkharaMasterData.link_unit_id_parts2;
                        cUnitGet.limitbreak_lv = (uint)1;
                        cUnitGet.level = (uint)1;
                        cUnitGet.add_hp = (uint)0;
                        cUnitGet.add_pow = (uint)0;
                        units.Add(cUnitGet);
                    }
                    if (cCLinkharaMasterData.link_unit_id_parts3 != 0)
                    {
                        PacketStructUnitGetDebug cUnitGet = new PacketStructUnitGetDebug();
                        cUnitGet.id = cCLinkharaMasterData.link_unit_id_parts3;
                        cUnitGet.limitbreak_lv = (uint)1;
                        cUnitGet.level = (uint)1;
                        cUnitGet.add_hp = (uint)0;
                        cUnitGet.add_pow = (uint)0;
                        units.Add(cUnitGet);
                    }
                }
            }
        }
        return units.ToArray();
    }

    /// <summary>
    /// ユニットユニークIDチェック
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    private long CheckUnitUniqueId(long _id)
    {
        if (_id == 0)
            return _id;
        if (_id < UNIT_UNIQUE_INDEX_BASE)
        {
            _id += UNIT_UNIQUE_INDEX_BASE;
        }
        return _id;
    }

    /// <summary>
    /// ヘルパー取得
    /// </summary>
    /// <returns></returns>
    private PacketStructFriend getHelper()
    {
        PacketStructFriend cHelper = null;
        if (UserDataAdmin.Instance.m_StructFriendList.Length > 0 &&
            UserDataAdmin.Instance.m_StructFriendList[0] != null)
        {
            cHelper = UserDataAdmin.Instance.m_StructFriendList[0];
        }
        else if (UserDataAdmin.Instance.m_StructHelperList.Length > 0 &&
                 UserDataAdmin.Instance.m_StructHelperList[0] != null)
        {
            cHelper = UserDataAdmin.Instance.m_StructHelperList[0];
        }
        return cHelper;
    }

    /// <summary>
    /// ユニットクラス取得
    /// </summary>
    /// <param name="unitid"></param>
    /// <returns></returns>
    private PacketStructUnit getUnitFromUniqueId(long unitid)
    {
        foreach (PacketStructUnit unit in UserDataAdmin.Instance.m_StructPlayer.unit_list)
        {
            if (unit.unique_id == unitid)
                return unit;
        }
        return null;
    }
}