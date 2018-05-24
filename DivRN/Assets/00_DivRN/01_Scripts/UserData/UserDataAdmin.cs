/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	UserDataAdmin.cs
	@brief	ユーザーデータ管理
	@author Developer
	@date 	2012/11/30

	サーバーから送られてきたプレイヤー情報はここで保持される。
	基本的には、サーバーから送られてきたプレイヤー情報を保持するだけ。

	サーバーからの受信データでの上書きのみを想定し、ローカルでの部分的な書き込みは想定しない。
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
	@brief	ユーザーデータ管理：プレイヤー情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class UserDataAdmin : SingletonComponent<UserDataAdmin>
{
    public enum UserFlagType
    {
        GlobalFriend,           //!< フレンド(承認・申請)
        GlobalMissionDaily,     //!< ミッションのデイリー(達成済みで未受け取り)
        GlobalMissionEvent,     //!< ミッションのイベント(達成済みで未受け取り)
        GlobalMissionNormal,    //!< ミッションのノーマル(達成済みで未受け取り)
        GlobalPresent,          //!< プレゼント(未受け取り)
        Max,
    }

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public PacketStructSystem m_StructSystem;                       //!< 受信情報：システム情報
    private PacketStructPlayer m_structPlayer;
    public PacketStructPlayer m_StructPlayer
    {
        get { return m_structPlayer; }
        set
        {
            m_structPlayer = value;
            UpdateUnitGridParam();

            //通信でスレットから呼ばれるのでメインスレッドで更新
            //Player.calcExpRatio()がMasterFinder<MasterDataUserRank>を呼び出しているため
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                updatePlayerParam();
            });
        }
    }

    public PacketStructQuest m_StructQuest;                         //!< 受信情報：クエスト受注情報
    public PacketStructFriend[] m_StructHelperList;                 //!< 受信情報：助っ人情報
    private PacketStructFriend[] m_structFriendList;                //!< 受信情報：フレンド情報
    public PacketStructFriend[] m_StructFriendList { get { return m_structFriendList; } set { m_structFriendList = value; updateFriend(); } }

    private PacketStructPresent[] m_structPresentList;              //!< 受信情報：プレゼント一覧
    public PacketStructPresent[] m_StructPresentList { get { return m_structPresentList; } set { m_structPresentList = value; updatePresent(); } }
    public PacketStructLoginMonthly m_StructLoginMonthly;           //!< 受信情報：月間ログインボーナス一覧
    public PacketStructPeriodLogin[] m_StructPeriodLogin;           //!< 受信情報：期間限定ログインボーナス一覧
    public string m_PeriodLoginResourceRoot;                        //!< 受信情報：期間限定ログインボーナスの画像が格納されているディレクトリ

    public PacketStructOthersPresent m_StructOthersPresent;         //!< 受信情報：その他プレゼント

    public PacketStructMasterHash[] m_StructMasterHashList;         //!< 受信情報：マスターハッシュリスト
    public uint[] m_StructMasterHashChanged;                        //!< 受信情報：マスターハッシュ変更フラグ
    public PacketStructUnit[][] m_StructPartyAssign = null;         //!< 解析情報：パーティアサインメンバー
    public MasterDataStoreProductEvent[] m_StructProductEvent = null;//!< 解析情報：イベントストア一覧取得

    private PacketStructHero[] m_structHeroList = null;             //!< 受信情報：HEROリスト情報
    public PacketStructHero[] m_StructHeroList
    {
        get { return m_structHeroList; }
        set
        {
            if (value == null)
            {
                return;
            }

            m_structHeroList = value;
        }
    }

    private bool[] m_FlagList = new bool[(int)UserFlagType.Max];

    public UnitGridParam[] m_UnitGridParamList = null;              //!< 解析情報：ユニットパラメータ一覧取得
    public long[] m_add_unit_uni_id_list;                           //!< 解析情報：ユニット追加情報
    public long[] m_update_unit_uni_id_list;                        //!< 解析情報：ユニット更新情報
    public long[] m_delete_unit_uni_id_list;                        //!< 解析情報：ユニット削除情報
    public bool m_cureate_unit_list;                                //!< 解析情報：ユニットリストの再作成フラグ
    public bool m_bThreadUnitParam = false;                         //!< ユニットパラメータが作成中？
    public System.Object m_SyncObj = new System.Object();

    public List<long> m_addUnitList = new List<long>();             //!< 生成中に追加されたユニット情報
    public List<long> m_updateUnitList = new List<long>();          //!< 生成中に更新されたユニット情報
    public List<long> m_deleteUnitList = new List<long>();          //!< 生成中に削除されたユニット情報

    private TemplateList<PacketStructGachaStatus> m_GachaStatus = null;			//!< ガチャ情報

    //プレイヤー情報(M4u参照用)
    [SerializeField]
    private UserDataPlayerParam player = new UserDataPlayerParam();
    public UserDataPlayerParam Player
    {
        get { return player; }
        set { player = value; }
    }

    private List<MasterDataParamChara> charaMasterArray = null;
    private List<MasterDataParamCharaEvol> evolMasterArray = null;

    private bool updateUserData = false;
    private int updateUserDataCount = 0;
    public bool IsUpdateUserData { get { return updateUserData; } }


    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	パラメータ破棄
	*/
    //----------------------------------------------------------------------------
    public void ParamReset()
    {
        m_StructSystem = null;
        m_StructQuest = null;
        m_StructHelperList = null;
        m_StructFriendList = null;
        m_StructPresentList = null;

        m_StructMasterHashList = null;
        m_StructMasterHashChanged = null;

        m_StructPartyAssign = null;

        m_StructProductEvent = null;

        m_structHeroList = null;

        m_add_unit_uni_id_list = null;
        m_update_unit_uni_id_list = null;
        m_delete_unit_uni_id_list = null;
        m_cureate_unit_list = false;

        ResetGachaStatus();
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
    */
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();
#if true //テストデータ
        Player.User_name = "てすとの名前";
        Player.Rank = 15;
        Player.Have_money = 5896324;
        Player.Have_stone = 32;
        Player.Have_ticket = 236;
        Player.Stamina_max = 128;
        Player.Stamina_now = 89;
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public void Update()
    {
        //----------------------------------------
        // スタミナ回復シミュレート
        // ※スタミナが全快でない場合限定処理
        //----------------------------------------
        if (m_StructPlayer != null
        && m_StructPlayer.stamina_now < m_StructPlayer.stamina_max
        )
        {
            //----------------------------------------
            // スタミナ回復に関連する時間系データを算出
            //----------------------------------------
            DateTime cTimeStaminaRecovery = TimeUtil.ConvertServerTimeToLocalTime(m_StructPlayer.stamina_recover);
            DateTime cTimeStaminaRecoveryNext = cTimeStaminaRecovery.AddSeconds(GlobalDefine.STAMINA_RECOVERY_SEC);
            DateTime cTimeNow = TimeManager.Instance.m_TimeNow;

            //----------------------------------------
            // 現在時間が次の回復予定時間を越えている場合、
            // 処理が実行されてスタミナが回復する
            //----------------------------------------
            //			Debug.LogError( cTimeNow + " , " + cTimeStaminaRecoveryNext );
            if (cTimeNow > cTimeStaminaRecoveryNext)
            {
                m_StructPlayer.stamina_now++;
                m_StructPlayer.stamina_recover = TimeUtil.ConvertLocalTimeToServerTime(cTimeStaminaRecoveryNext);
                // @add Developer 2015/12/02 v310 ローカル通知用に保存(ステータスバーの更新タイミングだけでいいかもしれないが、念のため)
                LocalNotificationUtil.m_StaminaNow = m_StructPlayer.stamina_now;
                updatePlayerParam();
            }
        }

        //ユーザーデータ更新フラグ
        if (updateUserData)
        {
            updateUserDataCount--;
            if (updateUserDataCount < 0)
            {
                updateUserData = false;
                updateUserDataCount = 0;
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	情報解析：パーティアサイン解析
	*/
    //----------------------------------------------------------------------------
    public void ConvertPartyAssing()
    {
        //----------------------------------------
        // エラーチェック
        //----------------------------------------
        if (m_StructPlayer == null
        || m_StructPlayer.unit_list == null
        || m_StructPlayer.unit_list.Length <= 0
        )
        {
            Debug.LogError("unit_list is null or empty");
            return;
        }

        //----------------------------------------
        // 領域確保
        //----------------------------------------
        //		m_StructPartyAssign = new PacketStructUnit[ GlobalDefine.PARTY_PATTERN_MAX ][];
        m_StructPartyAssign = new PacketStructUnit[m_StructPlayer.unit_party_assign.Length][];
        for (int i = 0; i < m_StructPartyAssign.Length; i++)
        {
            m_StructPartyAssign[i] = new PacketStructUnit[(int)GlobalDefine.PartyCharaIndex.MAX];
        }

        //----------------------------------------
        // パーティ編成に合わせたアサイン情報を構築
        //----------------------------------------
        for (int i = 0; i < m_StructPlayer.unit_party_assign.Length; i++)
        {
            m_StructPartyAssign[i][0] = SearchChara(m_StructPlayer.unit_party_assign[i].unit0_unique_id);
            m_StructPartyAssign[i][1] = SearchChara(m_StructPlayer.unit_party_assign[i].unit1_unique_id);
            m_StructPartyAssign[i][2] = SearchChara(m_StructPlayer.unit_party_assign[i].unit2_unique_id);
            m_StructPartyAssign[i][3] = SearchChara(m_StructPlayer.unit_party_assign[i].unit3_unique_id);
            m_StructPartyAssign[i][4] = null;
        }

        //----------------------------------------
        // ユニットリストのパラメータ更新
        //----------------------------------------
        if (m_UnitGridParamList != null)
        {
            for (int i = 0; i < m_UnitGridParamList.Length; ++i)
            {
                m_UnitGridParamList[i].party_assign = MainMenuUtil.ChkUnitPartyAssign(m_UnitGridParamList[i].unique_id);
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	情報解析：キャラクター検索：キャラID
	*/
    //----------------------------------------------------------------------------
    public PacketStructUnit SearchCharaFromID(uint unUnitID)
    {
        if (unUnitID == 0)
        {
            return null;
        }

        if (m_StructPlayer == null
        || m_StructPlayer.unit_list == null
        || m_StructPlayer.unit_list.Length <= 0
        )
        {
            return null;
        }

        for (int i = 0; i < m_StructPlayer.unit_list.Length; i++)
        {
            if (m_StructPlayer.unit_list[i].id != unUnitID)
            {
                continue;
            }

            return m_StructPlayer.unit_list[i];
        }
        return null;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	情報解析：キャラクター検索：ユニークID
	*/
    //----------------------------------------------------------------------------
    public PacketStructUnit SearchCharaByUnitId(long unitId)
    {
        if (unitId == 0)
        {
            return null;
        }

        if (m_StructPlayer == null
        || m_StructPlayer.unit_list == null
        || m_StructPlayer.unit_list.Length <= 0
        )
        {
            return null;
        }

        return m_structPlayer.unit_list.FirstOrDefault(u => u.id == unitId);
    }

    public PacketStructUnit SearchChara(long unUniqueID)
    {
        if (unUniqueID == 0)
        {
            return null;
        }

        if (m_StructPlayer == null
        || m_StructPlayer.unit_list == null
        || m_StructPlayer.unit_list.Length <= 0
        )
        {
            return null;
        }

        for (int i = 0; i < m_StructPlayer.unit_list.Length; i++)
        {
            if (m_StructPlayer.unit_list[i].unique_id != unUniqueID)
            {
                continue;
            }

            return m_StructPlayer.unit_list[i];
        }
        return null;
    }

    public PacketStructUnit SearchLinkUnit(PacketStructUnit _baseUnit)
    {
        if (_baseUnit == null)
        {
            return null;
        }

        if (_baseUnit.link_info != (uint)CHARALINK_TYPE.CHARALINK_TYPE_BASE)
        {
            return null;
        }

        return SearchChara(_baseUnit.link_unique_id);
    }

    public PacketStructUseItem SearchUseItem(uint _id)
    {
        if (m_StructPlayer == null)
        {
            return null;
        }

        for (int i = 0; i < m_StructPlayer.item_list.Length; i++)
        {
            if (m_StructPlayer.item_list[i].item_id == _id)
            {
                return m_StructPlayer.item_list[i];
            }
        }

        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	情報解析：キャラクター検索：ユニークID
	*/
    //----------------------------------------------------------------------------
    public int SearchCharaIndex(long unUniqueID)
    {
        if (unUniqueID == 0)
        {
            return -1;
        }

        if (m_StructPlayer == null
        || m_StructPlayer.unit_list == null
        || m_StructPlayer.unit_list.Length <= 0
        )
        {
            return -1;
        }

        for (int i = 0; i < m_StructPlayer.unit_list.Length; i++)
        {
            if (m_StructPlayer.unit_list[i].unique_id != unUniqueID)
            {
                continue;
            }

            return i;
        }
        return -1;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	情報解析：助っ人検索
	*/
    //----------------------------------------------------------------------------
    public PacketStructFriend SearchHelper(uint unUserID)
    {
        //----------------------------------------
        // 助っ人一覧を検索
        //----------------------------------------
        for (int i = 0; i < m_StructHelperList.Length; i++)
        {
            if (m_StructHelperList[i] == null
            || m_StructHelperList[i].user_id != unUserID
            )
            {
                continue;
            }

            return m_StructHelperList[i];
        }

        //----------------------------------------
        // フレンド一覧を検索
        //----------------------------------------
        for (int i = 0; i < m_StructFriendList.Length; i++)
        {
            if (m_StructFriendList[i] == null
            || m_StructFriendList[i].user_id != unUserID
            )
            {
                continue;
            }
            return m_StructFriendList[i];
        }

        //----------------------------------------
        // 該当無し
        //----------------------------------------
        return null;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="unUserID"></param>
    /// <returns></returns>
    public PacketStructFriend SearchHelperOnly(uint unUserID)
    {
        //----------------------------------------
        // 助っ人一覧を検索
        //----------------------------------------
        for (int i = 0; i < m_StructHelperList.Length; i++)
        {
            if (m_StructHelperList[i] == null
            || m_StructHelperList[i].user_id != unUserID
            )
            {
                continue;
            }

            return m_StructHelperList[i];
        }

        //----------------------------------------
        // 該当無し
        //----------------------------------------
        return null;
    }

    /// <summary>
    /// 使用できる助っ人数を取得
    /// </summary>
    /// <param name="userId"></param>
    public int CheckHelperCount()
    {
        int count = 0;

        //----------------------------------------
        // 助っ人一覧を検索
        //----------------------------------------
        for (int i = 0; i < m_StructHelperList.Length; i++)
        {
            PacketStructFriend helper = m_StructHelperList[i];
            if (helper == null)
            {
                continue;
            }

            //----------------------------------------
            // 指定フレンドが最近使った子かチェック
            //----------------------------------------
            LocalSaveFriendUse sFriendUseParam = LocalSaveManager.Instance.GetLocalSaveUseFriend(helper.user_id);
            if (sFriendUseParam != null)
            {
                //----------------------------------------
                // 連続で使用できないように１時間以内に使用した子は拒否
                //----------------------------------------
                DateTime cFriendUseTime = TimeUtil.ConvertServerTimeToLocalTime(sFriendUseParam.m_FriendUseTime);
                TimeSpan cFriendUseSpan = TimeManager.Instance.m_TimeNow - cFriendUseTime;
                if (cFriendUseSpan.TotalHours <= 1.0f)
                {
                    continue;
                }
            }

            if (m_StructFriendList != null)
            {
                //----------------------------------------
                // サーバー側でフレンドリストに含まれる子を助っ人リストに入れてしまうことがあるらしい
                // フレンドに同じユーザーIDの子がいるならスルー
                //----------------------------------------
                bool bSameAssign = false;
                for (int j = 0; j < m_StructFriendList.Length; j++)
                {
                    PacketStructFriend friend = m_StructFriendList[j];
                    if (friend == null ||
                        friend.friend_state != (int)FRIEND_STATE.FRIEND_STATE_SUCCESS)
                    {
                        continue;
                    }

                    if (friend.user_id != helper.user_id)
                    {
                        continue;
                    }

                    bSameAssign = true;
                    break;
                }

                if (bSameAssign == true)
                {
                    continue;
                }
            }

            count++;
        }

        return count;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	情報解析：助っ人検索
	*/
    //----------------------------------------------------------------------------
    public int SearchHelperIndex(uint unUserID)
    {
        //----------------------------------------
        // 助っ人一覧を検索
        //----------------------------------------
        if (m_StructHelperList != null)
        {
            for (int i = 0; i < m_StructHelperList.Length; i++)
            {
                if (m_StructHelperList[i] == null
                || m_StructHelperList[i].user_id != unUserID
                )
                {
                    continue;
                }

                return i;
            }
        }

        //----------------------------------------
        // フレンド一覧を検索
        //----------------------------------------
        if (m_StructFriendList != null)
        {
            for (int i = 0; i < m_StructFriendList.Length; i++)
            {
                if (m_StructFriendList[i] == null
                || m_StructFriendList[i].user_id != unUserID
                )
                {
                    continue;
                }

                return i;
            }
        }

        //----------------------------------------
        // 該当無し
        //----------------------------------------
        return -1;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	フレンドリストから存在しないキャラを除外
		@note	アカネあたりに置き換える → 進化プレミアムなどで不整合が起きるので領域破棄へ
	*/
    //----------------------------------------------------------------------------
    static public PacketStructFriend[] FriendListClipNotExist(PacketStructFriend[] acFriendList)
    {
        if (acFriendList == null)
        {
            return null;
        }

        //----------------------------------------
        // フレンド加工実行
        //
        // マスターに存在しないユニットがフレンドで送られてきているかチェック。
        // 可能な限りnewしたくないんで最初はチェックだけ。
        //----------------------------------------
        bool bCharaError = false;
        for (int i = 0; i < acFriendList.Length; i++)
        {
            if (acFriendList[i] == null
            || acFriendList[i].unit == null
            )
            {
                continue;
            }

            if (MasterDataUtil.GetCharaParamFromID(acFriendList[i].unit.id) != null)
            {
                continue;
            }

            bCharaError = true;
            break;
        }
        if (bCharaError == false)
        {
            //----------------------------------------
            // 不審なユニットがいないならそのままでOK
            //----------------------------------------
            return acFriendList;
        }

        //----------------------------------------
        // ここまで来たら不審なユニットがいる
        // →リストを再構築
        //----------------------------------------
        {
            TemplateList<PacketStructFriend> cFriendListNew = new TemplateList<PacketStructFriend>();
            for (int i = 0; i < acFriendList.Length; i++)
            {
                if (acFriendList[i] == null
                || acFriendList[i].unit == null
                )
                {
                    continue;
                }

                if (MasterDataUtil.GetCharaParamFromID(acFriendList[i].unit.id) == null)
                {
                    Debug.LogError("Friend Error ... " + acFriendList[i].unit.id + " , " + acFriendList[i].user_name);
                    continue;
                }

                cFriendListNew.Add(acFriendList[i]);
            }
            return cFriendListNew.ToArray();
        }
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	フレンドリストから自分を除外
		@note
	*/
    //----------------------------------------------------------------------------
    static public PacketStructFriend[] FriendListClipMe(PacketStructFriend[] acFriendList)
    {
        //----------------------------------------
        // サーバーが存在しないキャラIDを送ってくることがあるので、
        // クライアント側で間引き処理を行う
        //----------------------------------------
        acFriendList = FriendListClipNotExist(acFriendList);
        if (acFriendList == null)
        {
            return null;
        }

        //----------------------------------------
        // 自分がフレンドリストに内包されているかチェック
        //----------------------------------------
        bool bSelfInsideChk = false;
        uint unSelfID = UserDataAdmin.Instance.m_StructPlayer.user.user_id;
        for (int i = 0; i < acFriendList.Length; i++)
        {
            if (acFriendList[i] == null
            || acFriendList[i].user_id != unSelfID
            )
            {
                continue;
            }

            bSelfInsideChk = true;
            break;
        }
        if (bSelfInsideChk == false)
        {
            return acFriendList;
        }
        Debug.LogError("FriendList Hit Me!");

        //----------------------------------------
        // 自分がフレンドリストに内包されている場合、
        // リストから自分を除外して再構築
        //----------------------------------------
        TemplateList<PacketStructFriend> cFriendList = new TemplateList<PacketStructFriend>();
        for (int i = 0; i < acFriendList.Length; i++)
        {
            if (acFriendList[i] == null
            || acFriendList[i].user_id == unSelfID
            )
            {
                continue;
            }

            cFriendList.Add(acFriendList[i]);
        }
        return cFriendList.ToArray();
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	プレゼントリストから期限切れを除外
		@note
	*/
    //----------------------------------------------------------------------------
    static public PacketStructPresent[] PresentListClipTimeLimit(PacketStructPresent[] acPresentList)
    {
        //----------------------------------------
        // 要素無しならスルー
        //----------------------------------------
        if (acPresentList == null
        || acPresentList.Length <= 0
        )
        {
            return null;
        }

        //----------------------------------------
        // 期限切れを間引き
        //----------------------------------------
        TemplateList<PacketStructPresent> cPresentClipList = new TemplateList<PacketStructPresent>();
        DateTime cTimeNow = TimeManager.Instance.m_TimeNow;
        ulong unTimeNow = TimeUtil.ConvertLocalTimeToServerTime(cTimeNow);
        for (int i = 0; i < acPresentList.Length; i++)
        {
            if (acPresentList[i] == null)
                continue;

            //----------------------------------------
            // 無期限ならそのまま適用
            //----------------------------------------
            if (acPresentList[i].delete_timing == (long)PRESENT_TIMELIMIT.PRESENT_TIMELIMIT_FREE)
            {
                cPresentClipList.Add(acPresentList[i]);
                continue;
            }

            //----------------------------------------
            // 30日制限ならプレゼント発生から30日チェック
            //
            // サーバー側のセーフティのため、30日後の終日で制限
            //----------------------------------------
            if (acPresentList[i].delete_timing == (long)PRESENT_TIMELIMIT.PRESENT_TIMELIMIT_30)
            {
                DateTime cTimeBase = TimeUtil.ConvertServerTimeToLocalTime(acPresentList[i].send_timing);
                DateTime cTimeBaseAfter30 = cTimeBase.AddDays(30);
                DateTime cTimeLimit = new DateTime(cTimeBaseAfter30.Year, cTimeBaseAfter30.Month, cTimeBaseAfter30.Day, 23, 59, 59);
                ulong unTimeLimit = TimeUtil.ConvertLocalTimeToServerTime(cTimeLimit);
                if (unTimeLimit > unTimeNow)
                {
                    //----------------------------------------
                    // 計算ミスとかで表示と判定の誤差が出るのがイヤなので
                    // リミット時間を上書きしておく
                    //----------------------------------------
                    acPresentList[i].delete_timing = (long)unTimeLimit;

                    cPresentClipList.Add(acPresentList[i]);
#if BUILD_TYPE_DEBUG
                    Debug.Log("Present Active! " + acPresentList[i].present_type + " , " + cTimeLimit);
#endif
                }
                else
                {
                    Debug.LogError("Present Clip30 ... " + acPresentList[i].present_type + " , " + cTimeLimit + " ... " + cTimeBaseAfter30);
                }
                continue;
            }

            //----------------------------------------
            // 30日制限でも無制限でもない場合は、
            // サーバー時間でリミットが指定されているのでそのまま判定
            //----------------------------------------
            {
                DateTime cTimeBase = TimeUtil.ConvertServerTimeToLocalTime((ulong)acPresentList[i].delete_timing);
                // 指定日の0時00分00秒に削除されるので、指定日の前日の23時59分59秒に変更する
                DateTime cTimeLimit = new DateTime(cTimeBase.Year, cTimeBase.Month, cTimeBase.Day, 0, 0, 0);
                ulong unTimeLimit = TimeUtil.ConvertLocalTimeToServerTime(cTimeLimit) - 1;
                if (unTimeLimit > unTimeNow)
                {
                    //----------------------------------------
                    // 計算ミスとかで表示と判定の誤差が出るのがイヤなので
                    // リミット時間を上書きしておく
                    //----------------------------------------
                    acPresentList[i].delete_timing = (long)unTimeLimit;

                    cPresentClipList.Add(acPresentList[i]);
#if BUILD_TYPE_DEBUG
                    //					Debug.Log( "Present Active! " + acPresentList[i].present_type + " , " + cTimeLimit );
#endif
                    continue;
                }
                else
                {
                    Debug.LogError("Present Clip ... " + acPresentList[i].present_type + " , " + cTimeLimit + " ... " + cTimeBase);
                }
            }
        }
        return cPresentClipList.ToArray();
    }

    public PacketStructPresent SearchPresent(long _serial_id)
    {
        if (m_StructPresentList == null) return null;
        for (int i = 0; i < m_StructPresentList.Length; i++)
        {
            if (m_StructPresentList[i].serial_id == _serial_id) return m_StructPresentList[i];

        }
        return null;
    }

    // メニュー内遷移でのプレゼントAPI呼び出しタイミング調整カウント
    // MainMenuManager→OnUserStateUpdate
    public int m_PresentRequestWaitCount = 0;

    //----------------------------------------------------------------------------
    /*!
		@brief	サーバ送信用のユーザランク
		@note   サーバ側のアクセスログ作成時のDB検索回数を減らす為にランク情報を提供しているが、
				サーバから初回ユーザ情報を受取るまではデータが無く保証できない。
				サーバ側との相談の結果、それでも十分DB検索回数は減るとのことで、
				クライアント側は、値を持ってない場合は0値を送ることにした。
				サーバ側はAPIで判断し、CreateUser,AuthUserは保証出来ないとしてこのメソッドの戻り値を使わない。
	*/
    //----------------------------------------------------------------------------
    public uint SendServerPacketStructPlayerRunk()
    {
        uint runk = 0;

        // ユーザ情報があったら送る
        if (m_StructPlayer != null)
        {
            runk = m_StructPlayer.rank;
        }
        return runk;
    }

    /// <summary>
    /// プレイヤー情報更新
    /// </summary>
    public void updatePlayerParam()
    {
        Player.User_name = m_structPlayer.user.user_name;

        Player.Rank = m_structPlayer.rank;
        Player.Exp = m_structPlayer.exp;
        Player.calcExpRatio();

        Player.Stamina_now = m_structPlayer.stamina_now;
        Player.Stamina_max = m_structPlayer.stamina_max;
        Player.calcStaminaRatio();

        Player.Have_money = m_structPlayer.have_money;
        Player.Have_stone = m_structPlayer.have_stone;
        Player.Have_ticket = m_structPlayer.have_ticket;

        Player.isUpdateItem = true;

        updateUserData = true;
        updateUserDataCount = 2;
    }

    /// <summary>
    /// アイテムポイント取得
    /// </summary>
    /// <param name="event_id"></param>
    /// <returns></returns>
    public uint GetItemPoint(uint item_id)
    {
        if (m_StructPlayer == null ||
            m_StructPlayer.item_list == null)
        {
            return 0;
        }

        MasterDataUseItem itemMaster = MasterFinder<MasterDataUseItem>.Instance.Find((int)item_id);
        if (itemMaster == null)
        {
            return 0;
        }

        for (int i = 0; i < m_StructPlayer.item_list.Length; i++)
        {
            PacketStructUseItem item = m_StructPlayer.item_list[i];
            if (item == null)
            {
                continue;
            }

            if (itemMaster.fix_id == item.item_id)
            {
                return m_StructPlayer.item_list[i].item_cnt;
            }
        }
        return 0;
    }

    public MasterDataDefaultParty getCurrentHeroDefaultParty()
    {
        return MasterFinder<MasterDataDefaultParty>.Instance.Find(getCurrentHeroMaster().default_party_id);
    }

    public int HeroId
    {
        get
        {
            return getCurrentHero().hero_id;
        }
    }

    public MasterDataHero getCurrentHeroMaster()
    {
        int heroId = getCurrentHero().hero_id;
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL getCurrentHeroMaster:" + heroId);
#endif
        return MasterFinder<MasterDataHero>.Instance.Find(heroId);
    }

    /// <summary>
    /// 現在選択されているヒーロー情報
    /// </summary>
    /// <returns></returns>
    public PacketStructHero getCurrentHero()
    {
        if (m_StructPlayer != null && m_StructHeroList != null)
        {
            for (int idx = 0; idx < m_StructHeroList.Length; idx++)
            {
                PacketStructHero packet_struct_hero = m_StructHeroList[idx];
                if (packet_struct_hero != null
                    && packet_struct_hero.unique_id == m_StructPlayer.current_hero_id
                )
                {
                    return packet_struct_hero;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// スタミナが最大を超えているか？
    /// </summary>
    /// <returns></returns>
    public bool IsStaminaMax()
    {
        if (m_StructPlayer.stamina_now >= m_StructPlayer.stamina_max)
        {
            return true;
        }
        return false;
    }

    public bool IsUnitOver()
    {
        int max_unit = (int)m_StructPlayer.total_unit;
        int now_unit = m_StructPlayer.unit_list.Length;
        if (max_unit < now_unit)
        {
            return true;
        }
        return false;
    }

    public void SetUserFlag(UserFlagType _type, bool bFlag)
    {
        m_FlagList[(int)_type] = bFlag;

        UpdateHeader();
    }

    public bool GetUserFlag(UserFlagType _type)
    {
        return m_FlagList[(int)_type];
    }

    public void UpdateHeader()
    {
        if (!MainMenuManager.HasInstance)
        {
            return;
        }

        if (m_FlagList[(int)UserFlagType.GlobalFriend] ||
            m_FlagList[(int)UserFlagType.GlobalMissionDaily] ||
            m_FlagList[(int)UserFlagType.GlobalMissionEvent] ||
            m_FlagList[(int)UserFlagType.GlobalMissionNormal] ||
            m_FlagList[(int)UserFlagType.GlobalPresent]
            )
        {
            MainMenuManager.Instance.Header.IsActiveMenuFlag = true;
            if (GlobalMenu.GetGlobalMainMenu() != null) GlobalMenu.GetGlobalMainMenu().Context.IsActiveFlag = true;
        }
        else
        {
            MainMenuManager.Instance.Header.IsActiveMenuFlag = false;
            if (GlobalMenu.GetGlobalMainMenu() != null) GlobalMenu.GetGlobalMainMenu().Context.IsActiveFlag = false;
        }

        //ホームを開いていたら
        if (MainMenuManager.Instance.WorkSwitchPageNow == MAINMENU_SEQ.SEQ_HOME_MENU)
        {
            MainMenuHome home = (MainMenuHome)MainMenuManager.Instance.MainMenuSeqPageNow;
            bool bPresent = false;
            if (m_FlagList[(int)UserFlagType.GlobalPresent])
            {
                bPresent = true;
            }

            bool bMission = false;
            if (m_FlagList[(int)UserFlagType.GlobalMissionDaily] ||
                m_FlagList[(int)UserFlagType.GlobalMissionEvent] ||
                m_FlagList[(int)UserFlagType.GlobalMissionNormal])
            {
                bMission = true;
            }
            home.setFlag(bPresent, bMission);
        }
    }

    private void updatePresent()
    {
        bool bFlag = (m_structPresentList != null && m_structPresentList.Length != 0) ? true : false;
        SetUserFlag(UserFlagType.GlobalPresent, bFlag);
    }

    private void updateFriend()
    {
        bool bFlag = false;

        if (m_structFriendList != null)
        {
            uint totalWaitMe = 0;
            uint totalWaitHim = 0;
            GetFriendInfo(ref totalWaitMe, ref totalWaitHim);
            bFlag = (totalWaitMe != 0) ? true : false;
        }

        SetUserFlag(UserFlagType.GlobalFriend, bFlag);
    }

    public void GetFriendInfo(ref uint totalWaitMe, ref uint totalWaitHim)
    {
        totalWaitMe = 0;     // フレンド申請受中
        totalWaitHim = 0;    // フレンド申請出中
        if (m_structFriendList != null)
        {
            for (int i = 0; i < m_structFriendList.Length; i++)
            {
                if (m_structFriendList[i] == null)
                {
                    continue;
                }

                switch (m_structFriendList[i].friend_state)
                {
                    case (int)FRIEND_STATE.FRIEND_STATE_SUCCESS: break;
                    case (int)FRIEND_STATE.FRIEND_STATE_WAIT_ME: totalWaitMe++; break;
                    case (int)FRIEND_STATE.FRIEND_STATE_WAIT_HIM: totalWaitHim++; break;
                    case (int)FRIEND_STATE.FRIEND_STATE_UNRELATED: break;
                    case (int)FRIEND_STATE.FRIEND_STATE_PREMIUM: break;
                }
            }
        }

    }

    /// <summary>
    /// ユニットパラメータリストの更新
    /// </summary>
    public void UpdateUnitGridParam()
    {
        //パラメータが作成されるまでは更新処理しない
        if (m_UnitGridParamList == null)
        {
            return;
        }

        lock (m_SyncObj)
        {
            //パラメータ生成中
            if (m_bThreadUnitParam)
            {
                //生成中に更新がきたらIDを保存する。

                if (m_add_unit_uni_id_list != null)
                {
                    for (int i = 0; i < m_add_unit_uni_id_list.Length; i++)
                    {
                        m_addUnitList.Add(m_add_unit_uni_id_list[i]);
                    }
                    m_add_unit_uni_id_list = null;
                }
                if (m_update_unit_uni_id_list != null)
                {
                    for (int i = 0; i < m_update_unit_uni_id_list.Length; i++)
                    {
                        m_updateUnitList.Add(m_update_unit_uni_id_list[i]);
                    }
                    m_update_unit_uni_id_list = null;
                }
                if (m_delete_unit_uni_id_list != null)
                {
                    for (int i = 0; i < m_delete_unit_uni_id_list.Length; i++)
                    {
                        m_deleteUnitList.Add(m_delete_unit_uni_id_list[i]);
                    }
                    m_delete_unit_uni_id_list = null;
                }

                return;
            }
        }

        // 再作成
        if (m_cureate_unit_list)
        {
            CreateUnitGridParam();
            return;
        }

        // 追加
        if (m_add_unit_uni_id_list != null)
        {
            addUnitGridParam(m_add_unit_uni_id_list);
            m_add_unit_uni_id_list = null;
        }

        // 更新
        if (m_update_unit_uni_id_list != null)
        {
            updateUnitGridParam(m_update_unit_uni_id_list);
            m_update_unit_uni_id_list = null;
        }

        // 削除
        if (m_delete_unit_uni_id_list != null)
        {
            deleteUnitGridParam(m_delete_unit_uni_id_list);
            m_delete_unit_uni_id_list = null;
        }
    }

    private void addUnitGridParam(long[] add_unit_list)
    {
        List<UnitGridParam> paramList = new List<UnitGridParam>(m_UnitGridParamList);
        for (int i = 0; i < add_unit_list.Length; i++)
        {
            UnitGridParam tempParam = Array.Find(m_UnitGridParamList, (v) => v.unique_id == add_unit_list[i]);
            if (tempParam != null)
            {
                continue;
            }

            PacketStructUnit unitData = Array.Find(m_StructPlayer.unit_list, (v) => v.unique_id == add_unit_list[i]);
            if (unitData == null)
            {
                continue;
            }

            MasterDataParamChara master = GetCharaParamFromID(unitData.id);
            if (master == null)
            {
                continue;
            }

            UnitGridParam param = new UnitGridParam(unitData, master);
            paramList.Add(param);
        }

        m_UnitGridParamList = paramList.ToArray();
    }

    private void updateUnitGridParam(long[] update_unit_list)
    {
        for (int i = 0; i < update_unit_list.Length; i++)
        {
            PacketStructUnit unitData = Array.Find(m_StructPlayer.unit_list, (v) => v.unique_id == update_unit_list[i]);
            if (unitData == null)
            {
                continue;
            }

            updateUnitGridParam(unitData);

            // update_unit_listにリンク相手のidが入っていない場合、更新する
            if (unitData.link_unique_id > 0 && update_unit_list.Contains(unitData.link_unique_id) == false)
            {
                PacketStructUnit linkUnitData = CharaLinkUtil.GetLinkUnit(unitData.link_unique_id);
                updateUnitGridParam(linkUnitData);
            }
        }
    }

    private void updateUnitGridParam(PacketStructUnit unitData)
    {
        if (unitData == null)
        {
            return;
        }

        int index = Array.FindIndex(m_UnitGridParamList, (v) => v.unique_id == unitData.unique_id);
        if (index < 0)
        {
            return;
        }

        MasterDataParamChara master = GetCharaParamFromID(unitData.id);
        if (master == null)
        {
            return;
        }

        m_UnitGridParamList[index].setSortParamUnit(unitData, master);
    }

    private void deleteUnitGridParam(long[] delete_unit_list)
    {
        List<UnitGridParam> paramList = new List<UnitGridParam>(m_UnitGridParamList);

        for (int i = 0; i < delete_unit_list.Length; i++)
        {
            paramList.RemoveAll((v) => v.unique_id == delete_unit_list[i]);
        }
        m_UnitGridParamList = paramList.ToArray();
    }

    /// <summary>
    /// ユニットパラメータリストの作成
    /// </summary>
    public bool CreateUnitGridParam()
    {
        PacketStructUnit[] unit_list = m_structPlayer.unit_list;

        if (unit_list == null)
        {
            return false;
        }

        m_cureate_unit_list = false;
        m_add_unit_uni_id_list = null;
        m_update_unit_uni_id_list = null;
        m_delete_unit_uni_id_list = null;

        createUnitGridParamAll(unit_list);

        return true;
    }

    private void createUnitGridParamAll(PacketStructUnit[] unitList)
    {
        List<UnitGridParam> sortParamList = new List<UnitGridParam>();

        for (int i = 0; i < unitList.Length; i++)
        {
            MasterDataParamChara master = charaMasterArray.Find((v) => v.fix_id == unitList[i].id);
            if (master == null)
            {
                continue;
            }

            UnitGridParam sortParam = new UnitGridParam();
            sortParam.setSortParamUnit(unitList[i], master, false);
            sortParam.evolve = (evolMasterArray.Find((v) => v.unit_id_pre == unitList[i].id) != null) ? true : false;
            sortParamList.Add(sortParam);
        }
        m_UnitGridParamList = sortParamList.ToArray();
    }

    private System.Threading.Thread th = null;
    public void CreateThreadUnitGridParam()
    {
        m_UnitGridParamList = new UnitGridParam[0];
        PacketStructUnit[] unitList = m_structPlayer.unit_list;

        if (unitList == null)
        {
            return;
        }

        m_cureate_unit_list = false;
        m_add_unit_uni_id_list = null;
        m_update_unit_uni_id_list = null;
        m_delete_unit_uni_id_list = null;

        m_addUnitList.Clear();
        m_updateUnitList.Clear();
        m_deleteUnitList.Clear();

        // UnitGridParamの生成中にPlayerPrefasを使っているのでメインスレッドでキャッシュを作っておく
        LocalSaveManager.Instance.LoadFuncAddFavoriteUnit();

        m_bThreadUnitParam = true;

        //生成中にunit_listを変更されてしまうのでコピーして使う
        PacketStructUnit[] copyList = new PacketStructUnit[unitList.Length];
        for (int i = 0; i < unitList.Length; i++)
        {
            copyList[i] = new PacketStructUnit();
            copyList[i].Copy(unitList[i]);
        }

        th = new System.Threading.Thread(() =>
        {
            //マスターデータのキャッシュを作る
            charaMasterArray = null;
            evolMasterArray = null;
            List<MasterDataLimitOver> limitOverMasterArray = null;
            List<MasterDataLinkSystem> linkSystemMasterArray = null;
            List<MasterDataSkillLimitBreak> skillLimitBreakMasterArray = null;

            //メインスレッドで取得
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                charaMasterArray = MasterFinder<MasterDataParamChara>.Instance.FindAll();
                evolMasterArray = MasterFinder<MasterDataParamCharaEvol>.Instance.FindAll();
                limitOverMasterArray = MasterFinder<MasterDataLimitOver>.Instance.FindAll();
                linkSystemMasterArray = MasterFinder<MasterDataLinkSystem>.Instance.FindAll();
                skillLimitBreakMasterArray = MasterFinder<MasterDataSkillLimitBreak>.Instance.FindAll();
            });

            //Master取得待ち
            while (charaMasterArray == null ||
                  evolMasterArray == null ||
                  limitOverMasterArray == null ||
                  linkSystemMasterArray == null ||
                  skillLimitBreakMasterArray == null)
            {
                System.Threading.Thread.Sleep(1);
            }

            //パラメータ生成
            createUnitGridParamAll(copyList);

            //生成中に更新処理があったときの反映処理
            lock (m_SyncObj)
            {
                if (m_addUnitList.Count != 0)
                {
                    addUnitGridParam(m_addUnitList.ToArray());
                }
                if (m_updateUnitList.Count != 0)
                {
                    updateUnitGridParam(m_updateUnitList.ToArray());
                }
                if (m_deleteUnitList.Count != 0)
                {
                    deleteUnitGridParam(m_deleteUnitList.ToArray());
                }

                m_bThreadUnitParam = false;
            }

            m_addUnitList.Clear();
            m_updateUnitList.Clear();
            m_deleteUnitList.Clear();
        });
        th.Start();
    }

    public UnitGridParam SearchUnitGridParam(long unique_id)
    {
        if (m_UnitGridParamList == null)
        {
            return null;
        }

        UnitGridParam ret = Array.Find(m_UnitGridParamList, (v) => v.unique_id == unique_id);
        return ret;
    }

    public MasterDataParamChara GetCharaParamFromID(uint unID)
    {
        if (charaMasterArray == null)
        {
            return null;
        }

        if (unID == 0)
        {
            return null;
        }

        return charaMasterArray.FirstOrDefault(t => t.fix_id == unID);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	パラメータ操作：ガチャ情報：プレゼント付きガチャのプレゼント付与情報設定
		@return	データに更新があったか
	*/
    //----------------------------------------------------------------------------
    public bool UpdateGachaStatusList(PacketStructGachaStatus[] aResultStatus)
    {
        //　更新有無
        bool bRet = false;
        // 保持データが無い場合は作成する
        if (m_GachaStatus == null)
        {
            m_GachaStatus = new TemplateList<PacketStructGachaStatus>(aResultStatus);


            // 総付プレゼント情報を保存する
            String strPresentType = "";
            String strPresentId = "";
            String strPresentNum = "";

            if (m_GachaStatus[0].grossing_present_list != null)
            {
                for (int gacha_present_count = 0; gacha_present_count < m_GachaStatus[0].grossing_present_list.Length; gacha_present_count++)
                {
                    strPresentType += m_GachaStatus[0].grossing_present_list[gacha_present_count].grossing_present_type;
                    strPresentId += m_GachaStatus[0].grossing_present_list[gacha_present_count].grossing_present_id;
                    strPresentNum += m_GachaStatus[0].grossing_present_list[gacha_present_count].grossing_present_num;

                    if (gacha_present_count < m_GachaStatus[0].grossing_present_list.Length - 1)
                    {
                        strPresentType += ",";
                        strPresentId += ",";
                        strPresentNum += ",";
                    }
                }
                LocalSaveManager.Instance.SaveFuncGrossingPresent(strPresentType, strPresentId, strPresentNum);
            }

            // 更新あり
            bRet = true;
        }
        else
        {
            // 保持データがある場合
            // 帰ってきたデータを更新する
            for (int result_present_count = 0; result_present_count < aResultStatus.Length; result_present_count++)
            {
                // 保持データとの比較
                bool bCheck = false;
                for (int gacha_present_count = 0; gacha_present_count < m_GachaStatus.m_BufferSize; gacha_present_count++)
                {
                    // お互いのfix_idが同じか判定
                    if (aResultStatus[result_present_count].fix_id == m_GachaStatus[gacha_present_count].fix_id)
                    {
                        // 保持データが存在する
                        bCheck = true;
                        // 値が違えば値を更新
                        if (aResultStatus[result_present_count].present_count != m_GachaStatus[gacha_present_count].present_count)
                        {
                            m_GachaStatus[gacha_present_count].present_count = aResultStatus[result_present_count].present_count;
                            // 更新あり
                            bRet = true;
                        }
                        // 値が違えば値を更新
                        if (aResultStatus[result_present_count].date != m_GachaStatus[gacha_present_count].date)
                        {
                            m_GachaStatus[gacha_present_count].date = aResultStatus[result_present_count].date;
                            // 更新あり
                            bRet = true;
                        }
                        // 値が違えば値を更新
                        if (aResultStatus[result_present_count].paid_tip_only_count != m_GachaStatus[gacha_present_count].paid_tip_only_count)
                        {
                            m_GachaStatus[gacha_present_count].paid_tip_only_count = aResultStatus[result_present_count].paid_tip_only_count;
                            // 更新あり
                            bRet = true;
                        }
                        // 値が違えば値を更新
                        if (aResultStatus[result_present_count].grossing_only_count != m_GachaStatus[gacha_present_count].grossing_only_count)
                        {
                            m_GachaStatus[gacha_present_count].grossing_only_count = aResultStatus[result_present_count].grossing_only_count;
                            // 更新あり
                            bRet = true;
                        }
                        // 値が違えば値を更新
                        if (aResultStatus[result_present_count].step_num != m_GachaStatus[gacha_present_count].step_num)
                        {
                            m_GachaStatus[gacha_present_count].step_num = aResultStatus[result_present_count].step_num;
                            // 更新あり
                            bRet = true;
                        }
                        // 値が違えば値を更新
                        if (aResultStatus[result_present_count].is_first_time != m_GachaStatus[gacha_present_count].is_first_time)
                        {
                            m_GachaStatus[gacha_present_count].is_first_time = aResultStatus[result_present_count].is_first_time;
                            // 更新あり
                            bRet = true;

                        }
                        //break;
                    }

                    // 値が違えば値を更新
                    if (aResultStatus[result_present_count].grossing_present_list != m_GachaStatus[gacha_present_count].grossing_present_list
                        && aResultStatus[result_present_count].grossing_present_list != null)
                    {
                        m_GachaStatus[gacha_present_count].grossing_present_list = aResultStatus[result_present_count].grossing_present_list;

                        String strPresentType = "";
                        String strPresentId = "";
                        String strPresentNum = "";
                        //
                        for (int i = 0; i < m_GachaStatus[gacha_present_count].grossing_present_list.Length; i++)
                        {
                            strPresentType += m_GachaStatus[gacha_present_count].grossing_present_list[i].grossing_present_type;
                            strPresentId += m_GachaStatus[gacha_present_count].grossing_present_list[i].grossing_present_id;
                            strPresentNum += m_GachaStatus[gacha_present_count].grossing_present_list[i].grossing_present_num;

                            if (i < m_GachaStatus[gacha_present_count].grossing_present_list.Length - 1)
                            {
                                strPresentType += ",";
                                strPresentId += ",";
                                strPresentNum += ",";
                            }
                        }
                        LocalSaveManager.Instance.SaveFuncGrossingPresent(strPresentType, strPresentId, strPresentNum);

                        // 更新あり
                        bRet = true;
                    }
                }
                // データが無くループを抜けた場合は、新しく追加する
                if (bCheck == false)
                {
                    m_GachaStatus.Add(aResultStatus[result_present_count]);
                    // 更新あり
                    bRet = true;
                }
            }
        }

        return bRet;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	パラメータ操作：ガチャ情報：プレゼント付きガチャのプレゼント付与情報設定
		@return	データに更新があったか
	*/
    //----------------------------------------------------------------------------
    public PacketStructGachaStatus GetGachaStatus(uint fix_id)
    {
        // 保持データが無い場合はnull
        if (m_GachaStatus == null)
        {
            return null;
        }
        // 保持データがある場合
        for (int gacha_present_count = 0; gacha_present_count < m_GachaStatus.m_BufferSize; gacha_present_count++)
        {
            if (fix_id == m_GachaStatus[gacha_present_count].fix_id)
            {
                return m_GachaStatus[gacha_present_count];
            }
        }
        return null;
    }

    public void ResetGachaStatus()
    {
        if (m_GachaStatus == null)
        {
            return;
        }

        m_GachaStatus.Release();
        m_GachaStatus = null;
    }

    /// <summary>
    /// アイテムを使ったときのスタミナ値
    /// </summary>
    /// <param name="_master"></param>
    /// <returns></returns>
    public int GetUseItemStamina(MasterDataUseItem _master)
    {
        int result = (int)m_StructPlayer.stamina_now;
        if (!MasterDataUtil.ChkUseItemTypeStaminaRecovery(_master))
        {
            return result;
        }

        //端数切り上げ（サーバー準拠）
        int recover = (int)Math.Ceiling(((double)m_structPlayer.stamina_max * _master.stamina_recovery) / 100);
        result = (int)m_structPlayer.stamina_now + recover;

        return result;
    }
}

