﻿/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	InGameStruct.cs
	@brief	インゲーム関連構造体定義
	@author Developer
	@date 	2012/12/03
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
	@brief	ゲーム中入手ユニット情報
*/
//----------------------------------------------------------------------------
[Serializable]
public class InGameAcquireUnit
{
	//------------------------------------------------
	// ※※※※※※※※※※※※※※※※※※※※※※
	// ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
	// 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
	// ※※※※※※※※※※※※※※※※※※※※※※
	//------------------------------------------------
	public	uint	m_UnitID;					//!< 入手ユニット情報：ユニットID
	public	int		m_UnitLevel;				//!< 入手ユニット情報：ユニットレベル
	public	int		m_UnitFloor;				//!< 入手ユニット情報：入手フロア階層
	public	int		m_UnitAddPow;				//!< 入手ユニット情報：プラス値：攻撃力
	public	int		m_UnitAddHP;				//!< 入手ユニット情報：プラス値：HP
	
	/*==========================================================================*/
	/*		func																*/
	/*==========================================================================*/
	//----------------------------------------------------------------------------
	/*!
		@brief	コンストラクタ
	*/
	//----------------------------------------------------------------------------
	public InGameAcquireUnit()
	{
		m_UnitID		= 0;		// 入手ユニット情報：ユニットID
		m_UnitLevel		= 1;		// 入手ユニット情報：ユニットレベル
		m_UnitFloor		= 0;		// 入手ユニット情報：入手フロア階層
	}
	
	//----------------------------------------------------------------------------
	/*!
		@brief	コンストラクタ
	*/
	//----------------------------------------------------------------------------
	public InGameAcquireUnit( InGameAcquireUnit cSrc )
	{
		m_UnitID		= cSrc.m_UnitID;		// 入手ユニット情報：ユニットID
		m_UnitLevel		= cSrc.m_UnitLevel;		// 入手ユニット情報：ユニットレベル
		m_UnitFloor		= cSrc.m_UnitFloor;		// 入手ユニット情報：入手フロア階層
	}

	public void Clear() {
		m_UnitID		= 0;		// 入手ユニット情報：ユニットID
		m_UnitLevel		= 1;		// 入手ユニット情報：ユニットレベル
		m_UnitFloor		= 0;		// 入手ユニット情報：入手フロア階層
	}

};


//============================================================================
//	class
//============================================================================
//----------------------------------------------------------------------------
/*!
	@class		InGameAcquireExp
	@brief		ゲーム中入手経験値情報
*/
//----------------------------------------------------------------------------
[Serializable]
public class InGameAcquireExp {
	private const int		DEFAULT_VALUE = 0;
	private const int		DEFAULT_FLOOR = 0;

	public int		m_Value;				//!< 入手経験値
	public int		m_Floor;				//!< 入手フロア階層

	//------------------------------------------------------------------------
	/*!
		@brief		コンストラクタ
	*/
	//------------------------------------------------------------------------
	public InGameAcquireExp() {
		m_Value = InGameAcquireExp.DEFAULT_VALUE;
		m_Floor = InGameAcquireExp.DEFAULT_FLOOR;
	}

	//------------------------------------------------------------------------
	/*!
		@brief		値のクリア
	*/
	//------------------------------------------------------------------------
	public void ClsExp() {
		m_Value = InGameAcquireExp.DEFAULT_VALUE;
		m_Floor = InGameAcquireExp.DEFAULT_FLOOR;
	}

	//------------------------------------------------------------------------
	/*!
		@brief		空きかどうかの確認
		@retval		bool		[空き/空いてない]
	*/
	//------------------------------------------------------------------------
	public bool IsEmpty() {
		if ( m_Floor != DEFAULT_FLOOR ) {
			return false;
		}
		return true;
	}

}; // class InGameAcquireExp



//============================================================================
//	class
//============================================================================
//----------------------------------------------------------------------------
/*!
	@class	InGameAcquireMoney
	@brief	ゲーム中入手コイン情報
*/
//----------------------------------------------------------------------------
[Serializable]
public class InGameAcquireMoney
{
	//------------------------------------------------
	// ※※※※※※※※※※※※※※※※※※※※※※
	// ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
	// 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
	// ※※※※※※※※※※※※※※※※※※※※※※
	//------------------------------------------------
	public	int		m_MoneyValue;				//!< 入手コイン情報：入手額
	public	int		m_MoneyFloor;				//!< 入手コイン情報：入手フロア階層
	
	/*==========================================================================*/
	/*		func																*/
	/*==========================================================================*/
	//----------------------------------------------------------------------------
	/*!
		@brief		コンストラクタ
	*/
	//----------------------------------------------------------------------------
	public InGameAcquireMoney()
	{
		m_MoneyValue	= 0;		// 入手コイン情報：入手額
		m_MoneyFloor	= 0;		// 入手コイン情報：入手フロア階層
	}
	
	//----------------------------------------------------------------------------
	/*!
		@brief		コンストラクタ
	*/
	//----------------------------------------------------------------------------
	public InGameAcquireMoney( InGameAcquireMoney cSrc )
	{
		m_MoneyValue	= cSrc.m_MoneyValue;		// 入手コイン情報：入手額
		m_MoneyFloor	= cSrc.m_MoneyFloor;		// 入手コイン情報：入手フロア階層
	}

	//----------------------------------------------------------------------------
	/*!
		@brief		クリア
	*/
	//----------------------------------------------------------------------------
	public void Clear() {
		m_MoneyValue = 0;
		m_MoneyFloor = 0;
	}

}; // class InGameAcquireMoney



//----------------------------------------------------------------------------
/*!
	@class		InGameAcquireTicket
	@brief		ゲーム中入手チケット情報
*/
//----------------------------------------------------------------------------
[Serializable]
public class InGameAcquireTicket {
	
	//------------------------------------------------
	// ※※※※※※※※※※※※※※※※※※※※※※
	// ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
	// 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
	// ※※※※※※※※※※※※※※※※※※※※※※
	//------------------------------------------------
	public int		m_TicketFloor;			//!< 入手チケット情報：入手フロア
	public int		m_TicketValue;			//!< 入手チケット情報：入手枚数

	public InGameAcquireTicket() {
		m_TicketFloor = 0;
		m_TicketValue = 0;
	}

	public InGameAcquireTicket( InGameAcquireTicket cSrc ) {
		m_TicketFloor = cSrc.m_TicketFloor;
		m_TicketValue = cSrc.m_TicketValue;
	}

	public void Clear() {
		m_TicketFloor = 0;
		m_TicketValue = 0;
	}

} // class InGameAcquireTicket


//----------------------------------------------------------------------------
/*!
	@brief	インゲームフロア切り替えパラメータ
	@note	次のフロアに切り替わる際の参照パラメータ
*/
//----------------------------------------------------------------------------
public class InGameFloorSwitch
{
	public	const int		RECOVERY_NONE		= 0;	//!< 回復パターン：回復しない
	public	const int		RECOVERY_1_1		= 1;	//!< 回復パターン：全体量の 1/1 回復
	public	const int		RECOVERY_1_2		= 2;	//!< 回復パターン：全体量の 1/2 回復
	public	const int		RECOVERY_1_4		= 3;	//!< 回復パターン：全体量の 1/4 回復

	/*==========================================================================*/
	/*		var																	*/
	/*==========================================================================*/
	public		int			m_FloorNum			= 0;							//!< フロア番号

	public		int			m_PlayerRecoveryHP	= 0;							//!< 遷移時プレイヤー効果：HP回復
	public		int			m_PlayerRecoverySP	= 0;							//!< 遷移時プレイヤー効果：SP回復

	/*==========================================================================*/
	/*		func																*/
	/*==========================================================================*/
	//----------------------------------------------------------------------------
	//	コンストラクタ
	//----------------------------------------------------------------------------
	public InGameFloorSwitch() {

		m_FloorNum         = 0;
		m_PlayerRecoveryHP = 0;
		m_PlayerRecoverySP = 0;

	}


	public void SetupData( int floor, int hp_recovery, int sp_recovery ) {

		m_FloorNum = floor;
		m_PlayerRecoveryHP = hp_recovery;
		m_PlayerRecoverySP = sp_recovery;

	}

}


/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/