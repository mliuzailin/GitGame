/*==========================================================================*/
/*!
	@file		InGameDefine.cs
	@brief		インゲーム定義
*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//----------------------------------------------------------------------------
/*!
	@brief	インゲーム処理ステップ
*/
//----------------------------------------------------------------------------
public enum EINGAME_STEP : int {
	eINGAME_STEP_READY,										//!< インゲーム処理ステップ：初期化：諸々のしがらみ待ち
	eINGAME_STEP_READY_CREATE_PARTY,						//!< インゲーム処理ステップ：初期化：パーティー生成
	eINGAME_STEP_READY_CREATE_BATTLE,						//!< インゲーム処理ステップ：初期化：バトル関連生成

	eINGAME_STEP_INIT,										//!< インゲーム処理ステップ：初期化：
	eINGAME_STEP_INIT_FLOOR_SWITCH,							//!< インゲーム処理ステップ：初期化：フロア切り替え
	eINGAME_STEP_INIT_FLOOR_CREATE,							//!< インゲーム処理ステップ：初期化：フロア生成
	eINGAME_STEP_INIT_PARTY_REVISE,							//!< インゲーム処理ステップ：初期化：パーティ情報の補正
	eINGAME_STEP_INIT_FIRSTPOS,								//!< インゲーム処理ステップ：初期化：初期位置
	eINGAME_STEP_INIT_PLAYER_APPEAR,						//!< インゲーム処理ステップ：初期化：プレイヤー登場
	eINGAME_STEP_INIT_GAMESTART,							//!< インゲーム処理ステップ：初期化：ゲームスタート演出
	eINGAME_STEP_INIT_GAMESTART_WAIT,						//!< インゲーム処理ステップ：初期化：ゲームスタート演出終了待ち
	eINGAME_STEP_GAME_LEADERSKILL,							//!< インゲーム処理ステップ：ゲーム：リーダースキルによるパワーアップ演出
	eINGAME_STEP_GAME_LEADERSKILL_WAIT,						//!< インゲーム処理ステップ：ゲーム：リーダースキルによるパワーアップ演出
	eINGAME_STEP_GAME_PASSIVESKILL,							//!< インゲーム処理ステップ：ゲーム：パッシブスキルによるパワーアップ演出
	eINGAME_STEP_GAME_PASSIVESKILL_WAIT,					//!< インゲーム処理ステップ：ゲーム：パッシブスキルによるパワーアップ演出待ち
	eINGAME_STEP_GAME_LINKPASSIVE,							//!< インゲーム処理ステップ：ゲーム：リンクパッシブスキルによるパワーアップ演出
	eINGAME_STEP_GAME_LINKPASSIVE_WAIT,						//!< インゲーム処理ステップ：ゲーム：リンクパッシブスキルによるパワーアップ演出待ち
	eINGAME_STEP_INIT_MOVEON,								//!< インゲーム処理ステップ：初期化：ゲームスタート演出
	eINGAME_STEP_INIT_MOVEON_WAIT,							//!< インゲーム処理ステップ：初期化：ゲームスタート演出終了待ち

	eINGAME_STEP_GAME_INPUT,								//!< インゲーム処理ステップ：ゲーム：移動入力待ち
	eINGAME_STEP_GAME_MOVE_FIX,								//!< インゲーム処理ステップ：ゲーム：移動反映
	eINGAME_STEP_GAME_MOVE,									//!< インゲーム処理ステップ：ゲーム：移動中
	eINGAME_STEP_GAME_PANEL_START,							//!< インゲーム処理ステップ：ゲーム：パネル動作開始
	eINGAME_STEP_GAME_PANEL_ACT,							//!< インゲーム処理ステップ：ゲーム：パネル動作中
	eINGAME_STEP_GAME_GAMEOVER,								//!< インゲーム処理ステップ：ゲーム：ゲームオーバー
	eINGAME_STEP_GAME_CONTINUE,								//!< インゲーム処理ステップ：ゲーム：コンティニュー
	eINGAME_STEP_GAME_RETIRE,								//!< インゲーム処理ステップ：ゲーム：リタイア

	eINGAME_STEP_BATTLE_START,								//!< インゲーム処理ステップ：バトル：バトル開始
	eINGAME_STEP_BATTLE,									//!< インゲーム処理ステップ：バトル：バトル中
	eINGAME_STEP_BATTLE_GAMEOVER,							//!< インゲーム処理ステップ：バトル：ゲームオーバー
	eINGAME_STEP_BATTLE_END,								//!< インゲーム処理ステップ：バトル：バトル終了
	eINGAME_STEP_BATTLE_END_CHK,							//!< インゲーム処理ステップ：バトル：バトル連鎖チェック
	eINGAME_STEP_BATTLE_CHAIN,								//!< インゲーム処理ステップ：バトル：バトル連鎖
	eINGAME_STEP_BATTLE_EVOL_START,							//!< インゲーム処理ステップ：バトル：バトル派生
	eINGAME_STEP_BATTLE_EVOL_END,							//!< インゲーム処理ステップ：バトル：バトル派生

	eINGAME_STEP_CLEAR_BOSS_BATTLE,							//!< インゲーム処理ステップ：クリア：ドア開放
	eINGAME_STEP_CLEAR_STAGE,								//!< インゲーム処理ステップ：クリア：ステージクリア
	eINGAME_STEP_CLEAR_AREA,								//!< インゲーム処理ステップ：クリア：エリアクリア
	eINGAME_STEP_CLEAR_ANSWER,								//!< インゲーム処理ステップ：クリア：答え合わせ
	eINGAME_STEP_CLEAR_ANSWER_DONE,							//!< インゲーム処理ステップ：クリア：答え合わせ入力待ち
	eINGAME_STEP_CLEAR_RESTART_WAIT,						//!< インゲーム処理ステップ：クリア：リセット課金入力待ち
	eINGAME_STEP_CLEAR_RESTART_WAIT_DONE,					//!< インゲーム処理ステップ：クリア：リセット課金確認待ち
	eINGAME_STEP_CLEAR_NEXT_WAIT,							//!< インゲーム処理ステップ：クリア：次エリアへの切り替え待ち

	eINGAME_STEP_LIMITBREAK_TITLE,							//!< インゲーム処理ステップ：リミットブレイク：演出
	eINGAME_STEP_LIMITBREAK_TITLE_WAIT,						//!< インゲーム処理ステップ：リミットブレイク：演出終了待ち
	eINGAME_STEP_LIMITBREAK_TITLE_END,						//!< インゲーム処理ステップ：リミットブレイク：演出終了

	eINGAME_STEP_GO_RESULT,									//!< インゲーム処理ステップ：遷移：リザルトへ
	eINGAME_STEP_GO_NEXT,									//!< インゲーム処理ステップ：遷移：次エリアへ

	eINGAME_STEP_ERROR,										//!< インゲーム処理ステップ：エラー：

	eINGAME_STEP_MAX,										//!< インゲーム処理ステップ：
};


//----------------------------------------------------------------------------
/*!
	@brief		扉モーション定義
*/
//----------------------------------------------------------------------------
public enum EINGAME_DOOR_ANIM : int {
	eINGAME_DOOR_ANIM_OPEN,									//!< 鍵取得時の扉開き
	eINGAME_DOOR_ANIM_TATAKI,								//!< 定期ボスドア叩き
	eINGAME_DOOR_ANIM_CLOSE,								//!< ドア閉まり
	eINGAME_DOOR_ANIM_OPEN_BOSS,							//!< ボスドア開き
};

//----------------------------------------------------------------------------
/*!
	@brief		パネル状態定義
*/
//----------------------------------------------------------------------------
public enum EINGAME_PANEL_STATE : int {
	eINGAME_PANEL_STATE_NONE,								//!< 裏
	eINGAME_PANEL_STATE_USED,								//!< 表
};

//----------------------------------------------------------------------------
/*!
	@brief		SPアイコン定義
*/
//----------------------------------------------------------------------------
public enum ESP_ID : int {
	eSP_INIT = 0,											//!< SPアイコン_数字
	eSP_00 = eSP_INIT,
	eSP_01,
	eSP_02,
	eSP_03,
	eSP_04,
	eSP_05,
	eSP_06,
	eSP_07,
	eSP_08,
	eSP_09,
	eSP_10,
	eSP_11,
	eSP_12,
	eSP_13,
	eSP_14,
	eSP_15,
	eSP_16,
	eSP_17,
	eSP_18,
	eSP_19,
	eSP_MAX,
};

//----------------------------------------------------------------------------
/*!
	@brief		トラップの演出定義
*/
//----------------------------------------------------------------------------
public enum REQ_TRAPEFFECT : int {
	NONE,													//!< なし
	GOOD,													//!< 良い
	BAD,													//!< 悪い
};

//----------------------------------------------------------------------------
/*!
	@brief		警戒レベル定義
*/
//----------------------------------------------------------------------------
public enum ECAUTION_LEVEL : int {
	eCAUTION_MIN		= 0,
	eCAUTION_NONE		= ECAUTION_LEVEL.eCAUTION_MIN,		//!< 無
	eCAUTION_BLUE,											//!< 青
	eCAUTION_YELLOW,										//!< 黄
	eCAUTION_RED,											//!< 赤
	eCAUTION_ATTACK,										//!< 奇襲
	eCAUTION_MAX,											//!< 最大値
};

//----------------------------------------------------------------------------
/*!
	@brief		コンボステータス
*/
//----------------------------------------------------------------------------
public enum ECOMBO_STATE : int {
	eNONE,
	eIN,													//!< 入場
	eIDLE,													//!< 待機
	eOUT,													//!< 退場
};

//----------------------------------------------------------------------------
/*!
	@brief		ターンを示すキャプションのID定義
*/
//----------------------------------------------------------------------------
public enum ECAPTION : int {
	eNONE,
	eACTIVESKILL,
	ePASSIVESKILL,
	eLEADERSKILL,
	eLBSSKILL,
	eBOOSTSKILL,
	eLINKSKILL,
	eLINKPASSIVE,
	ePLAYERPHASE,
	eENEMYPHASE,
}

//----------------------------------------------------------------------------
/*!
	@brief		更新回数定義
*/
//----------------------------------------------------------------------------
public enum EBATTLE_UPDATE_STEP : int {
	eSTART = 0,
	eUPDATE,
}

//----------------------------------------------------------------------------
/*!
	@brief	ダメージタイプ
*/
//----------------------------------------------------------------------------
public enum EDAMAGE_TYPE
{
	eDAMAGE_TYPE_NORMAL,			//!< ダメージタイプ：通常
	eDAMAGE_TYPE_WEEK,				//!< ダメージタイプ：弱点
	eDAMAGE_TYPE_GUARD,				//!< ダメージタイプ：半減
	eDAMAGE_TYPE_HEAL,				//!< ダメージタイプ：回復
	eDAMAGE_TYPE_CRITICAL,			//!< ダメージタイプ：クリティカル

	eDAMAGE_TYPE_MAX,				//!< ダメージタイプ：
}

//----------------------------------------------------------------------------
/*!
	@class		InGameDefine
	@brief		インゲーム定義
*/
//----------------------------------------------------------------------------
static public class InGameDefine {
	public const string			RESOURCE_STAGEEFFECT_PATH		= "CardDM/Effect/Prefabs/stage/";				//!< ステージエフェクトパス
	public const string			RESOURCE_MOTION_TITLESLIDE_IN	= "TitleSlide_00_in";
	public const string			RESOURCE_MOTION_TITLESLIDE_OUT	= "TitleSlide_00_out";
	public const string			ANIM_CAPTION_IN					= "TitleSlide_00_in";
	public const string			ANIM_CAPTION_OUT				= "TitleSlide_00_out";
	public const string			ANIM_COUNTDOWN					= "CountDownAnim";
	public const string			ANIM_COUNTDOWNBACK				= "CountDownBackAnim";
	public const int			BATTLE_ATTACK_JUDGE_LOOP		= 6;											//!< スキル判定の1フレでのループ数
	public const int			BATTLE_ENEMY_MAX				= 8;                                            //!< 同時出現敵最大数
	public const int			BATTLE_SKILL_FIELD_MAX			= (BattleLogic.BATTLE_FIELD_COST_MAX + 1) * (int)GlobalDefine.PartyCharaIndex.MAX/*50*/;									//!< 同時可能攻撃最大数 // @Change Developer 2015/10/14 処理負荷対策。１パネ攻撃＊５人＝２５個と、回復パネルでの発動をバッファ（＋５）として、５０->３０に修正。
	public const int			BATTLE_SKILL_TOTAL_MAX			= BATTLE_SKILL_FIELD_MAX * BattleLogic.BATTLE_FIELD_MAX;	//!< 同時可能攻撃最大数
	public const int			BATTLE_SKILL_REACH_MAX			= BATTLE_SKILL_TOTAL_MAX;                       //!< 同時可能リーチ最大数

	public const int			PLAYERSTART_POS_X				= 2;                                            //!< 開始座標X
	public const int			PLAYERSTART_POS_Z				= 4;                                            //!< 開始座標Z
	public const int			PLAYERSTART_POS_NONE			= -1;                                           //!< 未選択

	public const int			SP_VALUE_STOP					= 0;											//!< 歩き停止のSP量
	public const int			SP_VALUE_USE					= 1;											//!< 歩いた時のSP減少量
	public const int			SP_VALUE_MIN					= 0;                                            //!< SP最少値
	public const float			DEFAULT_SCREEN_SCALE			= 480.0f;										//!< DefaultScreenScale(iPhone 640*960)
	public const float			PARTY_CHARA_ICON_OFFSET			= 0.6f;											//!< スキルウィンドウ表示時のUI飛び出し量
	public const float			PARTY_CHARA_LBSEFF_OFFSET		= 330.0f;										//!< スキルウィンドウ表示時のLBS使用可能エフェクト飛び出し量

	public const float			CAUTION_DANGER_RATE				= 0.3f;                                         //!< 危険と判断するHPの残量
	public const int			CAUTION_LEVEL_UP				=  1;											//!< 警戒レベル悪化量
	public const int			CAUTION_LEVEL_DOWN				= -1;                                           //!< 警戒レベル回復量
	public const int			TURN_OFFSET_RATE_CAUTION_RED	= 30;											//!< 先攻後攻ターン変化発生率
	public const int			TURN_OFFSET_RATE_CAUTION_ATTACK	=  0;											//!< 先攻後攻ターン変化発生率
	public const int			TURN_OFFSET_00					=  1;											//!< 後攻時のターン変化量
	public const int			TURN_OFFSET_01					= -1;											//!< 先攻時のターン変化量
	public const int			TURN_ENEMY_MIN					=  1;											//!< 敵のターン数：最低
	public const int			TURN_ENEMY_MAX					= 99;											//!< 敵のターン数：最大
	public const float			COUNTDOWN_PLAY_TIME_MIN			= 1.0f;											//!< カウントダウン行動の最少
	public const float			COUNTDOWN_TIME_MIN				= 0.0f;											//!< カウントダウン最少
	public const float			COUNTDOWN_TIME_MAX				= 9.0f;											//!< カウントダウン最大
	public const float			COUNTDOWN_TIME_DEFAULT			= 5.0f;											//!< カウントダウン通常時
	public const float			BOOST_RATE_MIN					= 1.0f;											//!< ブースト倍率最少
	public const float			BOOST_RATE_MAX					= 1.5f;											//!< ブースト倍率最大
	public const float			CRITICAL_RATE					= 1.5f;											//!< クリティカル倍率
	public const float			HANDS_RATE_MIN					= 0.01f;										//!< Rate数最少
	public const float			HANDS_RATE_MAX					= 10.0f;										//!< Rate数最大
	public const float			HANDS_RATE_DEFAULT				= 0.25f;										//!< Rate数通常時
	public const int			SELECT_NONE						= -1;                                           //!< 非選択番号

	public enum Effect2DAnchorType
	{
		CENTER				= 0,											//!< 画面中央基準位置
		BOTTOM				= 1,											//!< 画面下部基準位置
		ENEMY_ONCE			= 2,											//!< 敵一体
		CENTER2				= 3,											//!< 画面中央基準位置2（一部のエフェクトの補正用）
	}


	static public Vector3		VECTOR_EMPHASIS					= new Vector3(  0.0f,  0.2f,  0.0f);
	static public Vector3		VECTOR_SHAKE_LARGE				= new Vector3(  0.5f,  0.5f,  0.5f );
	static public Vector3		VECTOR_SHAKE_MIDDLE				= new Vector3( 0.35f, 0.35f, 0.35f );

	public const int			SE_ASSETARRAY_NUM				= 5;											//!< ランダム再生を考慮したSEバッファサイズ

}; // class InGameDefine

