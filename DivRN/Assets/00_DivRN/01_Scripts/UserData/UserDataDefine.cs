/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	UserDataDefine.cs
	@brief	ユーザーデータ管理関連定義
	@author Developer
	@date 	2012/11/28
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
using M4u;

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
	@brief	ユーザーデータ：プレイヤー情報
*/
//----------------------------------------------------------------------------
public class UserDataPlayerParam
{
    //!<ユーザーID
    M4uProperty<int> user_id = new M4uProperty<int>();
    public int User_id { get { return user_id.Value; } set { user_id.Value = value; } }
    //!<ユーザー名
    M4uProperty<string> user_name = new M4uProperty<string>();
    public string User_name { get { return user_name.Value; } set { user_name.Value = value; } }
    //!<ユーザーグループ
    M4uProperty<int> user_group = new M4uProperty<int>();
    public int User_group { get { return user_group.Value; } set { user_group.Value = value; } }

    //!<ランク
    M4uProperty<uint> rank = new M4uProperty<uint>();
    public uint Rank { get { return rank.Value; } set { rank.Value = value; } }
    //!<累積経験値
    M4uProperty<uint> exp = new M4uProperty<uint>();
    public uint Exp { get { return exp.Value; } set { exp.Value = value; } }
    //!<
    M4uProperty<float> expRatio = new M4uProperty<float>();
    public float ExpRatio { get { return expRatio.Value; } set { expRatio.Value = value; } }

    //!< 所持物：お金
    M4uProperty<uint> have_money = new M4uProperty<uint>();
    public uint Have_money
    {
        get { return have_money.Value; }
        set
        {
            have_money.Value = value;
            if (value > GlobalDefine.VALUE_MAX_COIN)
            {
                View_money = GlobalDefine.VALUE_MAX_COIN;
            }
            else
            {
                View_money = value;
            }
        }
    }
    M4uProperty<uint> view_money = new M4uProperty<uint>();
    public uint View_money { get { return view_money.Value; } set { view_money.Value = value; } }

    //!< 所持物：魔法石（有料分）
    M4uProperty<uint> have_stone_pay = new M4uProperty<uint>();
    public uint Have_stone_pay { get { return have_stone_pay.Value; } set { have_stone_pay.Value = value; } }

    //!< 所持物：魔法石（無料分）
    M4uProperty<uint> have_stone_free = new M4uProperty<uint>();
    public uint Have_stone_free { get { return have_stone_free.Value; } set { have_stone_free.Value = value; } }

    //!< 所持物：魔法石
    M4uProperty<uint> have_stone = new M4uProperty<uint>();
    public uint Have_stone
    {
        get { return have_stone.Value; }
        set
        {
            have_stone.Value = value;
            if (value > GlobalDefine.VALUE_VIEW_MAX_STONE)
            {
                View_stone = GlobalDefine.VALUE_VIEW_MAX_STONE;
            }
            else
            {
                View_stone = value;
            }
        }
    }
    M4uProperty<uint> view_stone = new M4uProperty<uint>();
    public uint View_stone { get { return view_stone.Value; } set { view_stone.Value = value; } }

    //!< 所持物：チケット
    M4uProperty<uint> have_ticket = new M4uProperty<uint>();
    public uint Have_ticket
    {
        get { return have_ticket.Value; }
        set
        {
            have_ticket.Value = value;
            if (value > GlobalDefine.VALUE_MAX_TICKET)
            {
                View_ticket = GlobalDefine.VALUE_MAX_TICKET;
            }
            else
            {
                View_ticket = value;
            }
        }
    }
    M4uProperty<uint> view_ticket = new M4uProperty<uint>();
    public uint View_ticket { get { return view_ticket.Value; } set { view_ticket.Value = value; } }

    //!< 所持物：フレンドポイント
    M4uProperty<uint> have_friend_pt = new M4uProperty<uint>();
    public uint Have_friend_pt { get { return have_friend_pt.Value; } set { have_friend_pt.Value = value; } }
    //!< 所持物：ユニットポイント
    M4uProperty<uint> have_unit_point = new M4uProperty<uint>();
    public uint Have_unit_point { get { return have_unit_point.Value; } set { have_unit_point.Value = value; } }


    //!< スタミナ：現在値 
    M4uProperty<uint> stamina_now = new M4uProperty<uint>();
    public uint Stamina_now { get { return stamina_now.Value; } set { stamina_now.Value = value; } }
    //!< スタミナ：最大値
    M4uProperty<uint> stamina_max = new M4uProperty<uint>();
    public uint Stamina_max { get { return stamina_max.Value; } set { stamina_max.Value = value; } }
    //!< スタミナ：比率
    M4uProperty<float> stamina_ratio = new M4uProperty<float>();
    public float Stamina_ratio { get { return stamina_ratio.Value; } set { stamina_ratio.Value = value; } }
    //!< スタミナ：回復時刻		※ローカルのカウントダウンはこれをもとにシミュレートする
    M4uProperty<uint> stamina_recover = new M4uProperty<uint>();
    public uint Stamina_recover { get { return stamina_recover.Value; } set { stamina_recover.Value = value; } }
    //!< スタミナ：
    M4uProperty<string> stamina_text = new M4uProperty<string>();
    public string Stamina_text { get { return stamina_text.Value; } set { stamina_text.Value = value; } }

    //!< 課金補正込最大数：ユニット枠最大数
    M4uProperty<uint> total_unit = new M4uProperty<uint>();
    public uint Total_unit { get { return total_unit.Value; } set { total_unit.Value = value; } }
    //!< 課金補正込最大数：フレンド枠最大数
    M4uProperty<uint> total_friend = new M4uProperty<uint>();
    public uint Total_friend { get { return total_friend.Value; } set { total_friend.Value = value; } }
    //!< 課金補正込最大数：パーティコスト最大数}
    M4uProperty<uint> total_party = new M4uProperty<uint>();
    public uint Total_party { get { return total_party.Value; } set { total_party.Value = value; } }

    M4uProperty<string> test = new M4uProperty<string>();
    public string Test { get { return test.Value; } set { test.Value = value; } }


    public void calcStaminaRatio()
    {
        Stamina_text = string.Format("{0}/{1}", Stamina_now, Stamina_max);
        Stamina_ratio = (float)Stamina_now / Stamina_max;
        if (Stamina_ratio < 0.0f) Stamina_ratio = 0.0f;
        if (Stamina_ratio >= 1.0f) Stamina_ratio = 1.0f;
    }

    public void calcExpRatio()
    {
        int nextRankNum = (int)Rank + 1;
        // DG0-2697 
        // SceneTitleのOnLoadResidentResourceのMaster取得完了処理でキャッシュする
        // SceneTitleのOnUserAuthentication でも呼び出されるがMaster取得前にキャッシュするとまずい。
        // キャッシュのみ取得する関数に変更
        MasterDataUserRank nextRank = MasterFinder<MasterDataUserRank>.Instance.FindCache(nextRankNum);
        if (nextRank != null)
        {
            int nextExp = (int)nextRank.exp_next;
            int needExp = (int)nextRank.exp_next_total - (int)UserDataAdmin.Instance.m_StructPlayer.exp;
            ExpRatio = 1.0f - ((float)needExp / (float)nextExp);
        }
        else
        {
            ExpRatio = 1.0f;
        }
    }

    public bool isUpdateItem { get; set; }
}

//----------------------------------------------------------------------------
/*!
	@brief	ユーザーデータ：ユニット情報
*/
//----------------------------------------------------------------------------
public class UserDataUnitParam
{
    //------------------------------------------------
    // ※※※※※※※※※※※※※※※※※※※※※※
    // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
    // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
    // ※※※※※※※※※※※※※※※※※※※※※※
    //------------------------------------------------
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public uint m_UnitDataID = 0;       //!< ユニット情報：共通データ：キャラID

    public int m_UnitParamEXP = 0;      //!< ユニット情報：単体固有：蓄積経験値
    public int m_UnitParamLevel = 1;        //!< ユニット情報：単体固有：レベル
    public long m_UnitParamUniqueID = 0;        //!< ユニット情報：単体固有：ユニークID

    public int m_UnitParamLimitBreakLV = 0;     //!< ユニット情報：単体固有：リミットブレイクスキルレベル
    public int m_UnitParamLimitOverLV = 0;      //!< ユニット情報：単体固有：限界突破レベル
    public int m_UnitParamPlusPow = 0;      //!< ユニット情報：単体固有：プラス値：攻撃
    public int m_UnitParamPlusHP = 0;       //!< ユニット情報：単体固有：プラス値：体力

    public uint m_UnitParamLinkID = 0;      //!< ユニット情報：共通データ：キャラID
    public int m_UnitParamLinkLv = 0;       //!< ユニット情報：単体固有：レベル
    public int m_UnitParamLinkPlusPow = 0;      //!< ユニット情報：単体固有：プラス値：攻撃
    public int m_UnitParamLinkPlusHP = 0;       //!< ユニット情報：単体固有：プラス値：体力
    public int m_UnitParamLinkPoint = 0;        //!< ユニット情報：単体固有：リンクポイント(リンクスキル発動率に影響)
    public int m_UnitParamLinkLimitOverLV = 0;      //!< ユニット情報：単体固有：限界突破レベル
}


//----------------------------------------------------------------------------
/*!
	@brief	ユーザーデータ：フレンド情報
*/
//----------------------------------------------------------------------------
public class UserDataFriendParam
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public string m_FriendUserName = "";        //!< フレンド情報：ユーザー名称
    public uint m_FriendUserID = 0;     //!< フレンド情報：ユーザーID
    public int m_FriendUserRank = 0;        //!< フレンド情報：ランク
    public int m_FriendPoint = 0;       //!< フレンド情報：フレンドポイント
    public int m_FriendState = 0;       //!< フレンド情報：ステータス
    public UserDataUnitParam m_FriendUnit = null;       //!< フレンド情報：ユニット情報

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	コンストラクタ
	*/
    //----------------------------------------------------------------------------
    public UserDataFriendParam()
    {
        m_FriendUserName = "";      // フレンド情報：ユーザー名称
        m_FriendUserID = 0;     // フレンド情報：ユーザーID
        m_FriendUserRank = 0;       // フレンド情報：ランク
        m_FriendPoint = 0;      // フレンド情報：フレンドポイント
        m_FriendState = 0;      // フレンド情報：ステータス
        m_FriendUnit = null;        // フレンド情報：ユニット情報
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	コンストラクタ
	*/
    //----------------------------------------------------------------------------
    public UserDataFriendParam(UserDataFriendParam cSrc)
    {
        m_FriendUserName = cSrc.m_FriendUserName;   // フレンド情報：ユーザー名称
        m_FriendUserID = cSrc.m_FriendUserID;       // フレンド情報：ユーザーID
        m_FriendUserRank = cSrc.m_FriendUserRank;   // フレンド情報：ランク
        m_FriendPoint = cSrc.m_FriendPoint;     // フレンド情報：フレンドポイント
        m_FriendState = cSrc.m_FriendState;     // フレンド情報：ステータス
        m_FriendUnit = cSrc.m_FriendUnit;       // フレンド情報：ユニット情報
    }
}