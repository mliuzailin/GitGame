/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	BattleReqManager.cs
	@brief	バトルリクエスト管理クラス
	@author Developer
	@date 	2012/10/08

	戦闘リクエストの管理クラス。
	ゲームマネージャがこのクラスを監視して、バトルリクエストがあればバトルを始めるように対応する。
	BattleManagerに直接開戦指示を出すのではなく、ここを一度経由することで、
	戦闘の発生契機パターンを問わず戦闘を行いやすくする。
	（パネルめくって敵出た時、ゴール触ってボス出た時、連戦やイベント戦闘等に対応しやすく）
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
	@brief	インゲーム戦闘リクエストクラス
*/
//----------------------------------------------------------------------------
[Serializable]
public class BattleReq {


	/*==========================================================================*/
	/*		var																	*/
	/*==========================================================================*/
	public PacketStructQuest2Build			m_QuestBuild       = null;
	public PacketStructQuest2BuildBattle		m_QuestBuildBattle = null;


	public	int			m_BattleUniqueID;				//!< 戦闘リクエスト情報：戦闘ユニークID

	public	int			m_BattleTurnOffset;				//!< 戦闘リクエスト情報：有利不利ターン


	public	bool		m_BattleEnemyBoss;				//!< 戦闘リクエスト情報：ボス戦か否か
	public	bool		m_BattleEnemyChain;				//!< 戦闘リクエスト情報：この戦闘が派生バトルかどうか
	// public	bool		m_BattleEnemyChainNext;			//!< 戦闘リクエスト情報：この戦闘に派生バトルが設定されているかどうか

	public	bool		m_BattleRequestAttached;		//!< 戦闘リクエスト情報：リクエスト実行済み


	//----------------------------------------------------------------------------
	//	@brief		コンストラクタ
	//----------------------------------------------------------------------------
	public BattleReq() {

		m_QuestBuild              = null;
		m_QuestBuildBattle        = null;

		m_BattleUniqueID          = 0;
		m_BattleTurnOffset        = 0;


		m_BattleEnemyBoss         = false;
		m_BattleEnemyChain        = false;

		m_BattleRequestAttached   = false;

	}

	/// <summary>
	/// Quest1 のデータを Quest2 のデータへ変換
	/// </summary>
	/// <param name="src_data"></param>
	/// <param name="dst_data"></param>
	public static void convQuestToQuest2(PacketStructQuestBuild src_data, PacketStructQuest2Build dst_data)
	{
		dst_data.boss = src_data.boss;
		dst_data.list_battle = new PacketStructQuest2BuildBattle[src_data.list_battle.Length];
		for (int idx = 0; idx < dst_data.list_battle.Length; idx++)
		{
			PacketStructQuestBuildBattle src_build_battle = src_data.list_battle[idx];
			PacketStructQuest2BuildBattle dst_build_battle = new PacketStructQuest2BuildBattle();

			convQuestToQuest2(src_build_battle, dst_build_battle);
		}

		dst_data.list_drop = new PacketStructQuest2BuildDrop[src_data.list_drop.Length];
		for (int idx = 0; idx < dst_data.list_drop.Length; idx++)
		{
			PacketStructQuestBuildDrop src_drop = src_data.list_drop[idx];
			PacketStructQuest2BuildDrop dst_drop = new PacketStructQuest2BuildDrop();

			dst_drop.unique_id = src_drop.unique_id;
			dst_drop.setKindType(PacketStructQuest2BuildDrop.KindType.UNIT);
			dst_drop.item_id = (int)src_drop.unit_id;
			dst_drop.plus_pow = src_drop.plus_pow;
			dst_drop.plus_hp = src_drop.plus_hp;

			dst_drop.num = 0;

			dst_drop.floor = src_drop.floor;
		}

		dst_data.list_used_item = src_data.list_used_item;

		dst_data.list_e_param = src_data.list_e_param;
		dst_data.list_e_acttable = src_data.list_e_acttable;
		dst_data.list_e_actparam = src_data.list_e_actparam;
	}

	/// <summary>
	/// Quest1 のデータを Quest2 のデータへ変換
	/// </summary>
	/// <param name="src_data"></param>
	/// <param name="dst_data"></param>
	public static void convQuestToQuest2(PacketStructQuestBuildBattle src_data, PacketStructQuest2BuildBattle dst_data)
	{
		dst_data.unique_id = src_data.unique_id;
		dst_data.enemy_list = src_data.enemy_list;
		dst_data.drop_list = src_data.drop_list;
		dst_data.chain = src_data.chain;
		dst_data.chain_turn_offset = src_data.chain_turn_offset;
		dst_data.hate = null;
		dst_data.floor = src_data.floor;
		dst_data.evol_direction = src_data.evol_direction;
		dst_data.bgm_id = 0;
	}

	//----------------------------------------------------------------------------
	//	@brief		データ設定
	//----------------------------------------------------------------------------
	public bool SetupBattleReq(	int uniqueID,    bool attach,
								int turn_offset, bool boss,   bool chain,
								PacketStructQuestBuild       quest_param,
								PacketStructQuestBuildBattle battle_param ) {

		// Quest2 系へ変換
		PacketStructQuest2Build dst_quest_param = new PacketStructQuest2Build();
		convQuestToQuest2(quest_param, dst_quest_param);

		PacketStructQuest2BuildBattle dst_battle_param = new PacketStructQuest2BuildBattle();
		convQuestToQuest2(battle_param, dst_battle_param);

		return SetupBattleReq(uniqueID, attach, turn_offset, boss, chain, dst_quest_param, dst_battle_param);
	}

	public bool SetupBattleReq(	int uniqueID,    bool attach,
								int turn_offset, bool boss,   bool chain,
								PacketStructQuest2Build       quest_param,
								PacketStructQuest2BuildBattle battle_param ) {

		//--------------------------------
		//	管理用パラメータ設定
		//--------------------------------
		m_BattleUniqueID        = uniqueID;
		m_BattleRequestAttached = attach;


		//--------------------------------
		//	戦闘情報パラメータ設定
		//--------------------------------
		m_BattleTurnOffset = turn_offset;
		m_BattleEnemyBoss  = boss;

		m_BattleEnemyChain = chain;


		//--------------------------------
		//	戦闘情報を保存
		//--------------------------------
		m_QuestBuild       = quest_param;
		m_QuestBuildBattle = battle_param;

		return (m_QuestBuildBattle != null);
	}


	//----------------------------------------------------------------------------
	//	@brief		この戦闘がボス戦闘か？
	//----------------------------------------------------------------------------
	public bool isBoss {
		get {
			return m_BattleEnemyBoss;
		}
	}


	//----------------------------------------------------------------------------
	//	@brief		この戦闘が連鎖戦闘か？
	//----------------------------------------------------------------------------
	public bool isChain {
		get {
			return m_BattleEnemyChain;
		}
	}


	//----------------------------------------------------------------------------
	//	@brief		この戦闘に連鎖戦闘が設定されているか？
	//----------------------------------------------------------------------------
	public bool isChainNextBattle {
		get {
			if ( m_QuestBuildBattle == null ) {
				return false;
			}

			return ( m_QuestBuildBattle.chain != 0 ) ? true : false;
		}
	}


	//----------------------------------------------------------------------------
	//	@brief		この戦闘に設定されている連鎖戦闘を取得
	//----------------------------------------------------------------------------
	public int chainNextBattle {
		get {
			if ( m_QuestBuildBattle == null ) {
				return 0;
			}

			return m_QuestBuildBattle.chain;
		}
	}


	//----------------------------------------------------------------------------
	//	@brief		この戦闘に設定されている連鎖戦闘を取得
	//----------------------------------------------------------------------------
	public int chainNextBattleTurnOffset {
		get {
			if ( m_QuestBuildBattle == null ) {
				return 0;
			}

			return m_QuestBuildBattle.chain_turn_offset;
		}
	}

	/// <summary>
	/// ヘイトが有効かどうか
	/// </summary>
	/// <returns></returns>
	public bool isEnableHate()
	{
		bool ret_val = (BattleParam.IsKobetsuHP && m_QuestBuildBattle != null && m_QuestBuildBattle.hate != null);
		return ret_val;
	}

	private int getHateRatePercent(MasterDataDefineLabel.ElementType element_type, MasterDataDefineLabel.KindType main_kind, MasterDataDefineLabel.KindType sub_kind)
	{
		int ret_val = 100;

		if (isEnableHate())
		{
			int element_percent = 100;
			int main_kind_percent = 100;
			int sub_kind_percent = 100;

			switch (element_type)
			{
				case MasterDataDefineLabel.ElementType.FIRE:
					element_percent = m_QuestBuildBattle.hate.hate_rate_fire;
					break;

				case MasterDataDefineLabel.ElementType.WATER:
					element_percent = m_QuestBuildBattle.hate.hate_rate_water;
					break;

				case MasterDataDefineLabel.ElementType.WIND:
					element_percent = m_QuestBuildBattle.hate.hate_rate_wind;
					break;

				case MasterDataDefineLabel.ElementType.LIGHT:
					element_percent = m_QuestBuildBattle.hate.hate_rate_light;
					break;

				case MasterDataDefineLabel.ElementType.DARK:
					element_percent = m_QuestBuildBattle.hate.hate_rate_dark;
					break;

				case MasterDataDefineLabel.ElementType.NAUGHT:
					element_percent = m_QuestBuildBattle.hate.hate_rate_naught;
					break;

				default:
					break;
			}

			for (int idx = 0; idx < 2; idx++)
			{
				MasterDataDefineLabel.KindType wrk_kind = MasterDataDefineLabel.KindType.NONE;
				switch (idx)
				{
					case 0:
						wrk_kind = main_kind;
						break;

					case 1:
						wrk_kind = sub_kind;
						break;
				}

				int wrk_kind_percent = 100;
				switch (wrk_kind)
				{
					case MasterDataDefineLabel.KindType.HUMAN:
						wrk_kind_percent = m_QuestBuildBattle.hate.hate_rate_race1;
						break;

					case MasterDataDefineLabel.KindType.DRAGON:
						wrk_kind_percent = m_QuestBuildBattle.hate.hate_rate_race1;
						break;

					case MasterDataDefineLabel.KindType.GOD:
						wrk_kind_percent = m_QuestBuildBattle.hate.hate_rate_race1;
						break;

					case MasterDataDefineLabel.KindType.DEMON:
						wrk_kind_percent = m_QuestBuildBattle.hate.hate_rate_race1;
						break;

					case MasterDataDefineLabel.KindType.CREATURE:
						wrk_kind_percent = m_QuestBuildBattle.hate.hate_rate_race1;
						break;

					case MasterDataDefineLabel.KindType.BEAST:
						wrk_kind_percent = m_QuestBuildBattle.hate.hate_rate_race1;
						break;

					case MasterDataDefineLabel.KindType.MACHINE:
						wrk_kind_percent = m_QuestBuildBattle.hate.hate_rate_race1;
						break;

					case MasterDataDefineLabel.KindType.EGG:
						wrk_kind_percent = m_QuestBuildBattle.hate.hate_rate_race1;
						break;

					default:
						break;
				}

				switch (idx)
				{
					case 0:
						main_kind_percent = wrk_kind_percent;
						break;

					case 1:
						sub_kind_percent = wrk_kind_percent;
						break;
				}
			}

			if (sub_kind == MasterDataDefineLabel.KindType.NONE)
			{
				ret_val = element_percent * main_kind_percent / 100;
			}
			else
			{
				// サブ種族があるキャラは種族の倍率はメインとサブの平均
				ret_val = element_percent * (main_kind_percent + sub_kind_percent) / 2 / 100;
			}
		}

		return ret_val;
	}

	/// <summary>
	/// ヘイト初期値を取得
	/// </summary>
	/// <param name="element_type">キャラの属性</param>
	/// <returns></returns>
	public int getInitialHate(MasterDataDefineLabel.ElementType element_type, MasterDataDefineLabel.KindType main_kind, MasterDataDefineLabel.KindType sub_kind)
	{
		int ret_val = 0;

		if (isEnableHate())
		{
			int element_rate = getHateRatePercent(element_type, main_kind, sub_kind);
			ret_val = m_QuestBuildBattle.hate.hate_initial * element_rate / 100;
		}

		return ret_val;
	}

	/// <summary>
	/// 与ダメージヘイト値を取得
	/// </summary>
	/// <param name="element_type">キャラの属性</param>
	/// <param name="rank">順位（１～５）</param>
	/// <returns></returns>
	public int getGivenDamageHate(MasterDataDefineLabel.ElementType element_type, MasterDataDefineLabel.KindType main_kind, MasterDataDefineLabel.KindType sub_kind, int rank)
	{
		int ret_val = 0;

		if (isEnableHate())
		{
			int damage_hate = 0;
			switch (rank)
			{
				case 1:
					damage_hate = m_QuestBuildBattle.hate.hate_given_damage1;
					break;

				case 2:
					damage_hate = m_QuestBuildBattle.hate.hate_given_damage2;
					break;

				case 3:
					damage_hate = m_QuestBuildBattle.hate.hate_given_damage3;
					break;

				case 4:
					damage_hate = m_QuestBuildBattle.hate.hate_given_damage4;
					break;

				case 5:
					damage_hate = m_QuestBuildBattle.hate.hate_given_damage5;
					break;
			}

			int element_rate = getHateRatePercent(element_type, main_kind, sub_kind);
			ret_val = damage_hate * element_rate / 100;
		}

		return ret_val;
	}

	/// <summary>
	/// 回復ヘイト値を取得
	/// </summary>
	/// <param name="element_type">キャラの属性</param>
	/// <param name="rank">順位（１～５）</param>
	/// <returns></returns>
	public int getHealHate(MasterDataDefineLabel.ElementType element_type, MasterDataDefineLabel.KindType main_kind, MasterDataDefineLabel.KindType sub_kind, int rank)
	{
		int ret_val = 0;

		if (isEnableHate())
		{
			int heal_hate = 0;
			switch (rank)
			{
				case 1:
					heal_hate = m_QuestBuildBattle.hate.hate_heal1;
					break;

				case 2:
					heal_hate = m_QuestBuildBattle.hate.hate_heal2;
					break;

				case 3:
					heal_hate = m_QuestBuildBattle.hate.hate_heal3;
					break;

				case 4:
					heal_hate = m_QuestBuildBattle.hate.hate_heal4;
					break;

				case 5:
					heal_hate = m_QuestBuildBattle.hate.hate_heal5;
					break;
			}

			int element_rate = getHateRatePercent(element_type, main_kind, sub_kind);

			ret_val = heal_hate * element_rate / 100;
		}

		return ret_val;
	}

	/// <summary>
	/// 被ダメージヘイト値を取得
	/// </summary>
	/// <returns></returns>
	public int getDamageHate()
	{
		return 0;
	}
}

//----------------------------------------------------------------------------
/*!
	@brief	バトルリクエスト管理クラス
*/
//----------------------------------------------------------------------------
public class BattleReqManager : SingletonComponent<BattleReqManager>
{
	const		int						BATTLE_REQ_MAX			= 10;		//!< 戦闘リクエスト受理最大数
	/*==========================================================================*/
	/*		var																	*/
	/*==========================================================================*/
	public		int						m_BattleUniqueID		= 0;		//!< 戦闘ユニークID

	public		BattleReq[]				m_BattleRequest			= null;		//!< 戦闘リクエスト：リクエスト情報
	public		bool					m_BattleRequestActive	= false;	//!< 戦闘リクエスト：リクエスト有無フラグ
	public		int						m_BattleRequestAccess	= 0;		//!< 戦闘リクエスト：リングバッファアクセス番号
	public		int						m_BattleRequestInput	= 0;		//!< 戦闘リクエスト：リングバッファ入力番号


	/*==========================================================================*/
	/*		func																*/
	/*==========================================================================*/
	//----------------------------------------------------------------------------
	/*!
		@brief	Unity固有処理：初期化処理	※インスタンス生成時呼出し
	*/
	//----------------------------------------------------------------------------
	protected override void Awake()
	{
		base.Awake();
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
	//----------------------------------------------------------------------------
	protected override void Start()
	{
		base.Start();

		//--------------------------------
		// リクエスト受付バッファを固定サイズ準備
		//--------------------------------
		m_BattleRequest = new BattleReq[ BATTLE_REQ_MAX ];
		for( int i = 0;i < m_BattleRequest.Length;i++ )
		{
			m_BattleRequest[i] = new BattleReq();
		}
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	戦闘リクエスト操作：リクエスト消去
	 */
	//----------------------------------------------------------------------------
	public	void BattleRequestClear()
	{
		//--------------------------------
		// アクセス情報のみクリア
		// バッファ作り直す必要はないので負荷優先で無視
		//--------------------------------
		m_BattleRequestActive	= false;
		m_BattleRequestAccess	= 0;
		m_BattleRequestInput	= 0;
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	戦闘リクエスト操作：リクエスト発行
		@param[in]		PacketStructQuestBuild			(quest_param)		クエスト構築情報
		@param[in]		PacketStructQuestBuildBattle	(battle_param)		戦闘構築情報
		@param[in]		bool							(boss)				この戦闘はボス戦闘か？
		@param[in]		bool							(chain)				この戦闘は連鎖戦闘か？
		@param[in]		int								(turn_offset)		有利不利ターン
	*/
	//----------------------------------------------------------------------------
	public bool BattleRequestAdd(	PacketStructQuestBuild       quest_param,
									PacketStructQuestBuildBattle battle_param,
									bool boss, bool chain, int turn_offset ) {

		//--------------------------------
		// バッファに空きがあるかチェック
		//--------------------------------
		if( (m_BattleRequestInput - m_BattleRequestAccess - 1) > m_BattleRequest.Length )
		{
			Debug.LogError( "BattleRequest Buffer Over!!" );
			return false;
		}


		//--------------------------------
		// バッファに空きがある
		// →リクエスト情報を受理
		//--------------------------------
		int nInputIndex = m_BattleRequestInput % m_BattleRequest.Length;
		m_BattleRequestInput	= m_BattleRequestInput + 1;
		m_BattleRequestActive	= true;


		//--------------------------------
		//	リクエストデータ設定
		//--------------------------------
		m_BattleRequest[ nInputIndex ].SetupBattleReq(	m_BattleUniqueID++, false,
														turn_offset, boss, chain,
														quest_param, battle_param );


		return true;
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	戦闘リクエスト操作：リクエスト発行
		@param[in]		PacketStructQuestBuild			(quest_param)		クエスト構築情報
		@param[in]		PacketStructQuestBuildBattle	(battle_param)		戦闘構築情報
		@param[in]		bool							(boss)				この戦闘はボス戦闘か？
		@param[in]		bool							(chain)				この戦闘は連鎖戦闘か？
		@param[in]		int								(turn_offset)		有利不利ターン
	*/
	//----------------------------------------------------------------------------
	public bool BattleRequestAdd(	PacketStructQuest2Build quest_param,
									PacketStructQuest2BuildBattle battle_param,
									bool boss, bool chain, int turn_offset)
	{

		//--------------------------------
		// バッファに空きがあるかチェック
		//--------------------------------
		if ((m_BattleRequestInput - m_BattleRequestAccess - 1) > m_BattleRequest.Length)
		{
			Debug.LogError("BattleRequest Buffer Over!!");
			return false;
		}


		//--------------------------------
		// バッファに空きがある
		// →リクエスト情報を受理
		//--------------------------------
		int nInputIndex = m_BattleRequestInput % m_BattleRequest.Length;
		m_BattleRequestInput = m_BattleRequestInput + 1;
		m_BattleRequestActive = true;


		//--------------------------------
		//	リクエストデータ設定
		//--------------------------------
		m_BattleRequest[nInputIndex].SetupBattleReq(	m_BattleUniqueID++, false,
														turn_offset, boss, chain,
														quest_param, battle_param);


		return true;
	}


	//----------------------------------------------------------------------------
	/*!
		@brief	戦闘リクエスト操作：リクエスト取得
	*/
	//----------------------------------------------------------------------------
	public BattleReq GetBattleRequest() {
		if( m_BattleRequestActive == false ) {
			return null;
		}

		int nAccessIndex = m_BattleRequestAccess % m_BattleRequest.Length;
		return m_BattleRequest[ nAccessIndex ];
	}
	//----------------------------------------------------------------------------
	/*!
		@brief	戦闘リクエスト操作：リクエスト消去
	*/
	//----------------------------------------------------------------------------
	public	bool ErsBattleRequest()
	{
		if( m_BattleRequestActive == false )
		{
			Debug.LogError( "DelBattleRequest Current Battle Is Null" );
			return false;
		}

		int nAccessIndex = m_BattleRequestAccess % m_BattleRequest.Length;

		//--------------------------------
		// 実行済みバトルリクエスト以外の消去は認めない
		//--------------------------------
		if( m_BattleRequest[ nAccessIndex ].m_BattleRequestAttached == false )
		{
			Debug.LogError( "DelBattleRequest Current Battle Attached == false" );
			return false;
		}

		//--------------------------------
		// カレントバトルリクエストと消去対象が一致
		// →情報をクリアしてアクセス番号を次へ進める
		//--------------------------------
		m_BattleRequest[ nAccessIndex ].m_BattleUniqueID = -1;
		m_BattleRequestAccess++;
		if( m_BattleRequestAccess == m_BattleRequestInput )
		{
			m_BattleRequestActive = false;
		}

		return true;
	}
	//----------------------------------------------------------------------------
	/*!
		@brief	戦闘リクエスト操作：リクエスト有無チェック
	*/
	//----------------------------------------------------------------------------
	public	bool ChkBattleRequest()
	{
		return m_BattleRequestActive;
	}
}
