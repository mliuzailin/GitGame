/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	UserDataAdminQuest.cs
	@brief	ユーザーデータ管理：クエスト状況
	@author Developer
	@date 	2012/11/28
	@note	クエスト中の一時的なパラメータ
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
	@brief	ユーザーデータ管理：クエスト状況
*/
//----------------------------------------------------------------------------
public class InGameQuestData : SingletonComponent<InGameQuestData>
{
	/*==========================================================================*/
	/*		var																	*/
	/*==========================================================================*/
	public	bool					m_InGameClear			= false;														//!< クリアフラグ
	public	bool					m_InGameRetire			= false;														//!< リタイアフラグ

	public	int						m_InGameEXP				= 0;															//!< 獲得クエスト経験値
	public	int						m_InGameMoney			= 0;															//!< 獲得クエスト金
	public	int						m_InGameTicket			= 0;															//!< 獲得クエストチケット

	public	InGameAcquireUnit[]		m_AcquireUnit;
	public	InGameAcquireMoney[]	m_AcquireMoney;
	public	InGameAcquireTicket[]	m_AcquireTicket;

#if FLOOR_EXP
	public	InGameAcquireExp[]		m_AcquireExp			= new InGameAcquireExp[ GlobalDefine.INGAME_MONEY_MAX ];		//!< 獲得経験値
#endif // #if FLOOR_EXP

	public int[]					m_AcquireUnitRareNum;

	/*==========================================================================*/
	/*		func																*/
	/*==========================================================================*/
	//------------------------------------------------------------------------
	/*!
		@brief		起動時呼出し
	*/
	//------------------------------------------------------------------------
	protected override void Awake() {
		base.Awake();

#if FLOOR_EXP
		for ( int i=0; i<m_AcquireExp.Length; i++ ) {
			m_AcquireExp[ i ] = new InGameAcquireExp();
		}
#endif // #if FLOOR_EXP

		m_AcquireMoney = new InGameAcquireMoney[ GlobalDefine.INGAME_MONEY_MAX ];
		for ( int i=0; i<m_AcquireMoney.Length; i++ ) {
			m_AcquireMoney[ i ] = new InGameAcquireMoney();
		}

		m_AcquireUnit = new InGameAcquireUnit[ GlobalDefine.INGAME_UNIT_MAX ];
		for ( int i=0; i<m_AcquireUnit.Length; i++ ) {
			m_AcquireUnit[ i ] = new InGameAcquireUnit();
		}

		m_AcquireTicket = new InGameAcquireTicket[ GlobalDefine.INGAME_TICKET_MAX ];
		for ( int i=0; i<m_AcquireTicket.Length; i++ ) {
			m_AcquireTicket[ i ] = new InGameAcquireTicket();
		}

		m_AcquireUnitRareNum = new int[(int)MasterDataDefineLabel.RarityType.MAX];
		for (int i = 0; i < m_AcquireUnitRareNum.Length; i++) {
			m_AcquireUnitRareNum[i] = 0;
		}
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
	//----------------------------------------------------------------------------
	protected override void Start()
	{
		base.Start();
	}

	//------------------------------------------------------------------------
	/*!
		@brief		破棄時呼出し
	*/
	//------------------------------------------------------------------------
	protected override void OnDestroy() {
		base.OnDestroy();

#if FLOOR_EXP
		for ( int i=0; i<m_AcquireExp.Length; i++ ) {
			m_AcquireExp[ i ] = null;
		}
		m_AcquireExp = null;
#endif // #if FLOOR_EXP
	}

#if false// TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
    //----------------------------------------------------------------------------
    /*!
		@brief		クエスト情報セットアップ
		@param[in]	uint		(unQuestID)		クエストID
		@retval		bool		[正常終了/エラー]
	*/
    //----------------------------------------------------------------------------
    public bool QuestSetup( uint unQuestID )
	{
		MasterDataQuest assign = MasterDataUtil.GetQuestParamFromID( unQuestID );
		if ( assign == null ) {
			return false;
		}

		// クエストクリア時の獲得データ情報を受け取る
		m_InGameEXP    = assign.clear_exp;
		m_InGameMoney  = assign.clear_money;

		// フラグ等のリセット
		m_InGameClear  = false;
		m_InGameRetire = false;

		return true;
	}
#endif
    //----------------------------------------------------------------------------
    /*!
		@brief	クエスト完遂フラグ入力
	*/
    //----------------------------------------------------------------------------
    public void QuestComplete() {
		m_InGameClear	= true;
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	クエストリタイアフラグ入力
	*/
	//----------------------------------------------------------------------------
	public void QuestRetire() {
		m_InGameRetire = true;
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	クエスト情報クリア
	*/
	//----------------------------------------------------------------------------
	public void QuestClear() {
		m_InGameClear	= false;
		m_InGameRetire	= false;
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	ゲーム中入手ユニット操作：追加
	*/
	//----------------------------------------------------------------------------
//	public bool AddAcquireUnitParam( InGameAcquireUnit cUnitParam ) {
	public bool AddAcquireUnitParam( uint unitID, int level, int floor, int addPow, int addHP ) {
		//------------------------
		// 空き領域チェック
		//------------------------
		int nFreeAccess = -1;
		for ( int i=0; i<m_AcquireUnit.Length; i++ ) {
			// ユニットIDが０なら使ってない
			if( m_AcquireUnit[ i ].m_UnitID != 0 ) {
				continue;
			}

			nFreeAccess = i;
			break;
		}

		if ( nFreeAccess == -1 ) {
			Debug.LogError( "Add Acquire Unit Buffer Over!" );
			return false;
		}
		
		//------------------------
		// ユニット追加
		//------------------------
		m_AcquireUnit[ nFreeAccess ].m_UnitID     = unitID;
		m_AcquireUnit[ nFreeAccess ].m_UnitLevel  = level;
		m_AcquireUnit[ nFreeAccess ].m_UnitFloor  = floor;
		m_AcquireUnit[ nFreeAccess ].m_UnitAddPow = addPow;
		m_AcquireUnit[ nFreeAccess ].m_UnitAddHP  = addHP;

		// 新探索用に追加 Developer
		MasterDataParamChara param = BattleParam.m_MasterDataCache.useCharaParam(unitID);
		if (param != null)
		{
			++m_AcquireUnitRareNum[(int)param.rare];
		}

		return true;
	}
	//----------------------------------------------------------------------------
	/*!
		@brief	ゲーム中入手ユニット操作：取得
	*/
	//----------------------------------------------------------------------------
	public InGameAcquireUnit GetAcquireUnitParamFromNum( int nAccessNum )
	{
		if( m_AcquireUnit.Length < nAccessNum )
			return null;
		return m_AcquireUnit[ nAccessNum ];
	}
	//----------------------------------------------------------------------------
	/*!
		@brief		ゲーム中入手ユニット操作：総数取得
		@return		int		[取得ユニット総数]
	*/
	//----------------------------------------------------------------------------
	public uint GetAcquireUnitTotal() {
		uint unitTotal = 0;
		for ( int i=0; i<m_AcquireUnit.Length; i++ ) {
			if ( m_AcquireUnit[ i ].m_UnitID == 0 ) {
				continue;
			}
			unitTotal += 1;
		}

		return unitTotal;
	}
	//----------------------------------------------------------------------------
	/*!
		@brief		ゲーム中入手ユニット操作：レア毎数取得
		@return		int		[取得ユニットレア毎数]
	*/
	//----------------------------------------------------------------------------
	public int GetAcquireUnitRare(MasterDataDefineLabel.RarityType rare)
	{
		return m_AcquireUnitRareNum[(int)rare];
	}
	//----------------------------------------------------------------------------
	/*!
		@brief	ゲーム中入手ユニット操作：破棄
	*/
	//----------------------------------------------------------------------------
	public void ClrAcquireUnitParam()
	{
		for ( int i=0; i<m_AcquireUnit.Length; i++ ) {
//			m_AcquireUnit[i] = null;
			m_AcquireUnit[ i ].Clear();
		}
	}
	//----------------------------------------------------------------------------
	/*!
		@brief	ゲーム中入手ユニット操作：階層指定ユニット消去
	*/
	//----------------------------------------------------------------------------
	public void DelAcquireUnitFloor( int nFloorNum )
	{
		//------------------------
		// 指定階層で手に入れたユニット情報を全て破棄する
		//------------------------
		for ( int i=0; i<m_AcquireUnit.Length; i++ ) {
			if ( m_AcquireUnit[ i ].m_UnitID == 0 ) {
				continue;
			}

			if ( m_AcquireUnit[ i ].m_UnitFloor != nFloorNum ) {
				continue;
			}

			m_AcquireUnit[ i ].Clear();
		}
	}
	
#if FLOOR_EXP // 経験値がフロア毎に取得できるような仕様がはいった場合に使用
	//------------------------------------------------------------------------
	/*!
		@brief		経験値入手
		@param[in]	int			(exp)		経験値
		@param[in]	int			(floor)		階層
		@retval		bool		[正常終了/エラー]
	*/
	//------------------------------------------------------------------------
	public bool AddExp( int exp, int floor ) {
		int free = -1;
		for ( int i=0; i<m_AcquireExp.Length; i++ ) {
			if ( m_AcquireExp[ i ] == null ) {
				continue;
			}

			if ( m_AcquireExp[ i ].IsEmpty() == false ) {
				continue;
			}

			free = i;
		}

		if ( free == -1 ) {
			return false;
		}

		m_AcquireExp[ free ].m_Value = exp;
		m_AcquireExp[ free ].m_Floor = floor;

		return true;
	}

	//------------------------------------------------------------------------
	/*!
		@brief		経験値総合値取得
	*/
	//------------------------------------------------------------------------
	public int GetTotalExp() {
		int total = 0;
		for ( int i=0; i<m_AcquireExp.Length; i++ ) {
			if ( m_AcquireExp[ i ] == null ) {
				continue;
			}

			total += m_AcquireExp[ i ].m_Value;
		}

		return total;
	}

	//------------------------------------------------------------------------
	/*!
		@brief		経験値クリア
	*/
	//------------------------------------------------------------------------
	public void ClsExp() {
		for ( int i=0; i<m_AcquireExp.Length; i++ ) {
			if ( m_AcquireExp[ i ] == null ) {
				continue;
			}

			m_AcquireExp[ i ].ClsExp();
		}
	}

	//------------------------------------------------------------------------
	/*!
		@brief		経験値クリア(指定フロアのみ)
		@param[in]	int			(floor)		指定フロア
	*/
	//------------------------------------------------------------------------
	public void ClsExp( int floor ) {
		for ( int i=0; i<m_AcquireExp.Length; i++ ) {
			if ( m_AcquireExp[ i ] == null ) {
				continue;
			}

			if ( m_AcquireExp[ i ].m_Floor != floor ) {
				continue;
			}

			m_AcquireExp[ i ].ClsExp();
		}
	}
#endif // #if FLOOR_EXP

	//----------------------------------------------------------------------------
	/*!
		@brief		ゲーム中入手コイン操作：追加
		@param[i]	int		(value)		価格
		@param[i]	int		(floor)		階層
	*/
	//----------------------------------------------------------------------------
	public bool AddAcquireMoneyParam( int value, int floor ) {
		//------------------------
		// 空き領域チェック
		//------------------------
		int nFreeAccess = -1;
		for ( int i=0; i<m_AcquireMoney.Length; i++ ) {
			// 価格が０であれば未使用
			if ( m_AcquireMoney[ i ].m_MoneyValue != 0 ) {
				continue;
			}

			nFreeAccess = i;
			break;
		}

		if ( nFreeAccess == -1 ) {
			Debug.LogError( "Add Acquire Money Buffer Over!" );
			return false;
		}
		
		//------------------------
		// ユニット追加
		//------------------------
		m_AcquireMoney[ nFreeAccess ].m_MoneyValue = value;
		m_AcquireMoney[ nFreeAccess ].m_MoneyFloor = floor;

		return true;
	}
	//----------------------------------------------------------------------------
	/*!
		@brief	ゲーム中入手コイン操作：総額取得
	*/
	//----------------------------------------------------------------------------
	public uint GetAcquireMoneyTotal() {
		uint unTotalMoney = 0;
		for ( int i=0; i<m_AcquireMoney.Length; i++ ) {
			if ( m_AcquireMoney[ i ].m_MoneyValue == 0 ) {
				continue;
			}

			unTotalMoney += (uint)m_AcquireMoney[i].m_MoneyValue;
		}

		return unTotalMoney;
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	ゲーム中入手コイン操作：階層指定合計取得
	*/
	//----------------------------------------------------------------------------
	public uint GetAcquireMoneyTotalFloor( int floor ) {
		uint unTotalMoney = 0;
		for( int i=0; i<m_AcquireMoney.Length; i++ ) {
			if ( m_AcquireMoney[ i ].m_MoneyValue == 0 ) {
				continue;
			}

			if ( m_AcquireMoney[ i ].m_MoneyFloor != floor ) {
				continue;
			}

			unTotalMoney += (uint)m_AcquireMoney[ i ].m_MoneyValue;
		}
		return unTotalMoney;
	}


	//----------------------------------------------------------------------------
	/*!
		@brief	ゲーム中入手コイン操作：破棄
	*/
	//----------------------------------------------------------------------------
	public void ClrAcquireMoneyParam() {
		for( int i=0; i<m_AcquireMoney.Length; i++ ) {
			m_AcquireMoney[ i ].Clear();
		}
	}
	//----------------------------------------------------------------------------
	/*!
		@brief	ゲーム中入手コイン操作：階層指定コイン消去
	*/
	//----------------------------------------------------------------------------
	public void DelAcquireMoneyFloor( int nFloorNum ) {
		//------------------------
		// 指定階層で手に入れたコイン情報を全て破棄する
		//------------------------
		for ( int i=0; i<m_AcquireMoney.Length; i++ ) {
			if ( m_AcquireMoney[ i ].m_MoneyValue == 0 ) {
				continue;
			}

			if ( m_AcquireMoney[ i ].m_MoneyFloor != nFloorNum ) {
				continue;
			}

			m_AcquireMoney[ i ].Clear();
		}
	}


	//----------------------------------------------------------------------------
	/*!
		@brief		ゲーム中入手チケット操作：チケット入手
	*/
	//----------------------------------------------------------------------------
	public bool AddAcquireTicket( int floor, int value ) {

		//------------------------
		//	エラーチェック
		//------------------------
		if ( m_AcquireTicket == null ) {
			return false;
		}


		int access = -1;
		for ( int i=0; i<m_AcquireTicket.Length; i++ ) {

			//------------------------
			//	エラーチェック
			//------------------------
			if ( m_AcquireTicket[ i ] == null ) {
				continue;
			}

			//------------------------
			//	０を未使用として扱う
			//------------------------
			if ( m_AcquireTicket[ i ].m_TicketValue != 0 ) {
				continue;
			}

			access = i;
			break;
		}

		//------------------------
		//	空き領域がない
		//------------------------
		if ( access == -1 ) {
			return false;
		}


		m_AcquireTicket[ access ].m_TicketFloor = floor;
		m_AcquireTicket[ access ].m_TicketValue = value;


		return true;
	}


	//----------------------------------------------------------------------------
	/*!
		@brief		ゲーム中入手チケット操作：総数取得
	*/
	//----------------------------------------------------------------------------
	public int GetAcquireTicketTotal() {

		//------------------------
		//	エラーチェック
		//------------------------
		if ( m_AcquireTicket == null ) {
			return 0;
		}


		int ticket_total = 0;

		//------------------------
		//	総数計算
		//------------------------
		for ( int i=0; i<m_AcquireTicket.Length; i++ ) {

			//------------------------
			//	エラーチェック
			//------------------------
			if ( m_AcquireTicket[ i ] == null ) {
				continue;
			}


			//------------------------
			//	枚数０を未使用と判定する
			//------------------------
			if ( m_AcquireTicket[ i ].m_TicketValue == 0 ) {
				continue;
			}


			ticket_total = ticket_total + m_AcquireTicket[ i ].m_TicketValue;
		}


		return ticket_total;
	}


	//----------------------------------------------------------------------------
	/*!
		@brief		ゲーム中入手チケット操作：階層指定総数取得
	*/
	//----------------------------------------------------------------------------
	public int GetAcquireTicketTotalFloor( int floor ) {

		//------------------------
		//	エラーチェック
		//------------------------
		if ( m_AcquireTicket == null ) {
			return 0;
		}

		int ticket_total = 0;

		//------------------------
		//	総数計算
		//------------------------
		for ( int i=0; i<m_AcquireTicket.Length; i++ ) {

			//------------------------
			//	エラーチェック
			//------------------------
			if ( m_AcquireTicket[ i ] == null ) {
				continue;
			}


			//------------------------
			//	枚数０を未使用と判定する
			//------------------------
			if ( m_AcquireTicket[ i ].m_TicketValue == 0 ) {
				continue;
			}


			//------------------------
			//	指定階層かどうかチェック
			//------------------------
			if ( m_AcquireTicket[ i ].m_TicketFloor == floor ) {
				continue;
			}



			ticket_total = ticket_total + m_AcquireTicket[ i ].m_TicketValue;
		}


		return ticket_total;
	}



	//----------------------------------------------------------------------------
	/*!
		@brief		ゲーム中入手チケット操作：チケット情報のクリア
	*/
	//----------------------------------------------------------------------------
	public void ClrAcquireTicketParam() {

		if ( m_AcquireTicket == null ) {
			return;
		}


		//------------------------
		//	総数計算
		//------------------------
		for ( int i=0; i<m_AcquireTicket.Length; i++ ) {

			//------------------------
			//	エラーチェック
			//------------------------
			if ( m_AcquireTicket[ i ] == null ) {
				continue;
			}


			m_AcquireTicket[ i ].Clear();
		}
	}


	//----------------------------------------------------------------------------
	/*!
		@brief		ゲーム中入手チケット操作：チケット情報
	*/
	//----------------------------------------------------------------------------
	public void DelAcquireTicketFloor( int floor ) {

		if ( m_AcquireTicket == null ) {
			return;
		}


		//------------------------
		//	総数計算
		//------------------------
		for ( int i=0; i<m_AcquireTicket.Length; i++ ) {

			//------------------------
			//	エラーチェック
			//------------------------
			if ( m_AcquireTicket[ i ] == null ) {
				continue;
			}


			//------------------------
			//	指定階層かどうかチェック
			//------------------------
			if ( m_AcquireTicket[ i ].m_TicketFloor != floor ) {
				continue;
			}


			m_AcquireTicket[ i ].Clear();
		}
	}



	//----------------------------------------------------------------------------
	/*!
		@brief		クエスト内での獲得データ情報のクリア
	*/
	//----------------------------------------------------------------------------
	public void ClrAcquireQuest() {

		ClrAcquireMoneyParam();
		ClrAcquireUnitParam();
		ClrAcquireTicketParam();


		m_InGameEXP    = 0;
		m_InGameMoney  = 0;
		m_InGameTicket = 0;
	}

	//----------------------------------------------------------------------------
	/*!
		@brief		フロア内での獲得データ情報のクリア
		@param[in]	int		(nFloor)		フロア数
	*/
	//----------------------------------------------------------------------------
	public void ClrAcquireFloor( int nFloor ) {

		DelAcquireMoneyFloor( nFloor );
		DelAcquireUnitFloor( nFloor );
		DelAcquireTicketFloor( nFloor );

	}

	//----------------------------------------------------------------------------
	/*!
		@brief		新クエスト情報セットアップ
		@param[in]	uint		(unQuestID)		クエストID
		@retval		bool		[正常終了/エラー]
	*/
	//----------------------------------------------------------------------------
	public bool Quest2Setup(uint unQuestID)
	{
		MasterDataQuest2 assign = MasterDataUtil.GetQuest2ParamFromID(unQuestID);
		if (assign == null)
		{
			return false;
		}

		// クエストクリア時の獲得データ情報を受け取る
		m_InGameEXP = assign.clear_exp;
		m_InGameMoney = assign.clear_money;
		//		m_InGameTicket = assign.clear_ticket;

		// フラグ等のリセット
		m_InGameClear = false;
		m_InGameRetire = false;

		return true;
	}

} // class InGameQuestData

