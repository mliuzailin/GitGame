using UnityEngine;
using System.Collections;

using ServerDataDefine;

namespace ServerApiTest
{
	/// <summary>
	/// デフォルトパーティタイプ
	/// </summary>
	public enum DefaultPartyType
	{
		Red=1,
		Blue,
		Green,
	}

	public enum GetUnitStatus
	{
		Normal=0,
		LevelMax,
	}


	/// <summary>
	/// プレイヤー変更パラメータ
	/// </summary>
	[System.Serializable]
	public class EditPlayerParam
	{
		public int Rank;               //!< 入手情報：ランク
		public int GameMoney;          //!< 入手情報：お金
		public int FreePaidPoint;      //!< 入手情報：無料チップ
		public int FriendPoint;        //!< 入手情報：フレンドポイント
		public int TicketCasino;       //!< 入手情報：カジノチケット
		public int UnitPoint;          //!< 入手情報：ユニットポイント
		public int BuyMaxUnitCount;    //!< 入手情報：ユニット枠購入数
		public int BuyMaxFriendCount;   //!< 入手情報：フレンド枠購入数
		public bool AllQuestClear;		//!< 入手情報：クエストクリア情報
	}

	[System.Serializable]
	public class EditGetUnitParam
	{
		public int id;
		public GetUnitStatus status;
		public bool evolve;
		public bool link;
	}

	[System.Serializable]
	public class EditPartyAssign
	{
		public int currentParty = 0;
		public PacketStructPartyAssign[] assign;
	}

	[System.Serializable]
	public class EditUnitBuildup
	{
		public long unUnitUniqueIDBase;
		public long[] aunUnitUniqueIDParts;
		public bool bFriendPointActive;
		public uint unEventFP;
		public uint unEventSLV;
		public int nBeginnerBoostID;
		public bool bTutorialActive;
	}

	[System.Serializable]
	public class EditUnitLink
	{
		public long unUnitUniqueIDBase;
		public long unUnitUniqueIDLink;
		public long[] aunUnitUniqueIDParts;
	}

	[System.Serializable]
	public class EditQuestStart
	{
		public uint unQuestID;
		public uint unQuestState;
		public bool bHelperPointActive;
		public uint unEventFP;
        public int  orderGetDetail;
	}

	[System.Serializable]
	public class EditEvolQuest
	{
		public uint unQuestID;
		public uint unQuestState;
		public long unEvolUnitUniqueIDBase;
		public long[] aunEvolUnitUniqueIDParts;
		public bool bHelperPointActive;
		public uint unEventFP;
	}
	[System.Serializable]
    public class EditEvolveUnit
    {
        public long unEvolUnitUniqueIDBase;
        public long[] aunEvolUnitUniqueIDParts;
        public int nBeginnerBoostID;
    }

    [System.Serializable]
	public class EditAchievementGroup
	{
		public uint Category;
		public uint Page;
		public int SortType;
	}

	[System.Serializable]
	public class EditAchievement
	{
		public uint GroupId;
		public uint Page;
		public int SortType;
	}
	[System.Serializable]
	public class EditAchievementOpen
	{
		public uint[] AchievementIdArray;
		public uint[] AchievementGroupIdArray;
	}

	[System.Serializable]
	public class EditGacha
	{
		public uint Id;
		public uint Count;
	}

	[System.Serializable]
	public class EditShopLimitOver
	{
		public uint productId;
		public uint unitUniqueId;
		public uint limitOverCount;
	}

	[System.Serializable]
	public class EditUserTransfar
	{
		public string userId;
		public string password;
		public string snsId;
	}
}
