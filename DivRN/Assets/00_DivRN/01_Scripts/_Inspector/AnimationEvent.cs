/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	AnimationEvent.cs
	@brief	アニメーションイベント
	@author Developer
	@date 	2012/11/06

 	アニメーションイベントクラス
	[ Animation ]のコンポーネントが付いたオブジェクトに対してアサインすることで、
	アニメーションイベントとしてこのスクリプト内のメソッドが指定できるようになる
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
	@brief	アニメーションイベント
*/
//----------------------------------------------------------------------------
public class AnimationEvent : MonoBehaviour
{
    //----------------------------------------------------------------------------
    /*!
		@brief
	*/
    //----------------------------------------------------------------------------
    // オープニング用SE
    public void AnimEvent_PlaySE_Opening_TapChange()
    {
        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
    }

    // オープニング用SE
    public void AnimEvent_PlaySE_Opening_ExpGauge()
    {
        SoundUtil.PlaySE(SEID.SE_MM_B01_EXP_GAUGE);
    }

    // オープニング用SE
    public void AnimEvent_PlaySE_Opening_LevelUP()
    {
        SoundUtil.PlaySE(SEID.SE_MM_D04_LEVEL_UP);
    }

    // オープニング用SE
    public void AnimEvent_PlaySE_Opening_UIKieru()
    {
        SoundUtil.PlaySE(SEID.SE_INGAME_QUEST_START_02);
    }

    // オープニング用SE
    public void AnimEvent_PlaySE_Opening_Mekure()
    {
        SoundUtil.PlaySE(SEID.SE_INGAME_PANEL_MEKURI);
    }

    // オープニング用SE
    public void AnimEvent_PlaySE_Opening_MekuriNormal()
    {
        SoundUtil.PlaySE(SEID.SE_INGAME_PANEL_MEKURI_NORMAL);
    }

    // オープニング用SE
    public void AnimEvent_PlaySE_Opening_MekuriSpecial()
    {
        SoundUtil.PlaySE(SEID.SE_INGAME_PANEL_MEKURI_SPECIAL);
    }


    public void AnimEvent_TitleCall() { SoundUtil.PlaySE(SEID.SE_TITLE_CALL_W); }   //!< アニメーションイベント：SE再生指示：タイトルコール
    public void AnimEvent_PlaySE_Mekuri() { SoundUtil.PlaySE(SEID.SE_INGAME_PANEL_MEKURI); }    //!< アニメーションイベント：SE再生指示：めくり
    public void AnimEvent_PlaySE_Mekuri_Keika_Normal() { SoundUtil.PlaySE(SEID.SE_INGAME_PANEL_MEKURI_NORMAL); }    //!< アニメーションイベント：SE再生指示：めくり経過通常
    public void AnimEvent_PlaySE_Mekuri_keika_Special() { SoundUtil.PlaySE(SEID.SE_INGAME_PANEL_MEKURI_SPECIAL); }  //!< アニメーションイベント：SE再生指示：めくり経過特殊
    public void AnimEvent_PlaySE_Tatakitsuke() { SoundUtil.PlaySE(SEID.SE_INGAME_PANEL_SHOCK); }    //!< アニメーションイベント：SE再生指示：叩きつけ

    // アニメーションイベント：SE再生指示：ドア作動音：開く
    public void AnimEvent_PlaySE_DoorOpen()
    {
        SoundUtil.PlaySE(SEID.SE_INGAME_DOOR_OPEN);
    }

    public void AnimEvent_PlaySE_DoorBossTataki() { SoundUtil.PlaySE(SEID.SE_INGAME_DOOR_BOSS_TATAKI); }    //!< アニメーションイベント：SE再生指示：ドア作動音：ボスドア：叩く
    public void AnimEvent_PlaySE_DoorBossOpen() { SoundUtil.PlaySE(SEID.SE_INGAME_DOOR_BOSS_OPEN); }    //!< アニメーションイベント：SE再生指示：ドア作動音：ボスドア：開く

    // アニメーションイベント：SE再生指示：ボス登場
    public void AnimEvent_PlaySE_BossAppear()
    {
        SoundUtil.PlaySE(SEID.SE_BATTLE_BOSS_APPEAR);
    }

    // アニメーションイベント：SE再生指示：クエスト開始：ReadyTo
    public void AnimEvent_PlaySE_QuestStart_ReadyTo()
    {
        SoundUtil.PlaySE(SEID.SE_INGAME_QUEST_START_00);
        SoundUtil.PlaySE(SEID.VOICE_INGAME_QUEST_READYTO);
    }

    // アニメーションイベント：SE再生指示：クエスト開始：MoveOn
    public void AnimEvent_PlaySE_QuestStart_MoveOn()
    {
        SoundUtil.PlaySE(SEID.VOICE_INGAME_QUEST_MOVEON);
    }

    // アニメーションイベント：SE再生指示：リトライイベントウィンドウの音
    public void AnimEvent_PlaySE_RetryWindow()
    {
        SoundUtil.PlaySE(SEID.SE_INGAME_ACTIVITY_ITEM);
    }

    public void AnimEvent_PlaySE_LBS_CutIn() { SoundUtil.PlaySE(SEID.SE_BATLE_SKILL_LIMITBREAK_CUTIN); }
    public void AnimEvent_PlaySE_LBS_Impact() { SoundUtil.PlaySE(SEID.SE_BATLE_SKILL_LIMITBREAK_IMPACT); }
    public void AnimEvent_PlaySE_QuestStart_Out() { SoundUtil.PlaySE(SEID.SE_INGAME_QUEST_START_02); }
    public void AnimEvent_PlaySE_CostIn() { SoundUtil.PlaySE(SEID.SE_BATLE_COST_IN); }  //!< アニメーションイベント：SE再生指示：カード配り
    public void Animevent_PlaySE_Hands() { SoundUtil.PlaySE(SEID.SE_BATLE_SKILL_HANDS); }
    public void AnimEvent_PlaySE_SkillCutin() { SoundUtil.PlaySE(SEID.SE_BATLE_SKILL_CUTIN); }
    public void AnimEvent_PlaySE_SkillCaption() { SoundUtil.PlaySE(SEID.SE_BATLE_SKILL_CAPTION); }
    public void AnimEvent_PlaySE_BattleUIClose() { SoundUtil.PlaySE(SEID.SE_MENU_RET); }

    // アニメーションイベント：SE再生指示：システムSE：ジングル：ゲーム開始
    public void AnimEvent_PlaySE_StageStart()
    {
        //		SoundUtil.PlaySE( SoundManager.SE_STARGE_START );
    }

    // アニメーションイベント：SE再生指示：システムSE：ジングル：ゲームクリア
    public void AnimEvent_PlaySE_StageClear()
    {
        SoundUtil.PlaySE(SEID.VOICE_INGAME_QUEST_QUESTCLEAR);
    }

    // アニメーションイベント：SE再生指示：システムSE：ジングル：ゲームクリアUI
    public void AnimEvent_PlaySE_StageClearUI()
    {
        SoundUtil.PlaySE(SEID.SE_STAGE_CLEAR_UI);
    }

    // アニメーションイベント：SE再生指示：システムSE：ジングル：ゲームオーバー
    public void AnimEvent_PlaySE_StageGameOver()
    {
        SoundUtil.PlaySE(SEID.SE_STARGE_GAMEOVER);
    }

    // アニメーションイベント：SE再生指示：バトルSE：バトルウィンドウ：開く
    public void AnimEvent_PlaySE_BattleWindowOpen()
    {
        SoundUtil.PlaySE(SEID.SE_BATLE_WINDOW_OPEN);
    }

    public void AnimEvent_PlaySE_BattleWindowClose() { SoundUtil.PlaySE(SEID.SE_BATLE_WINDOW_CLOSE); }  //!< アニメーションイベント：SE再生指示：バトルSE：バトルウィンドウ：閉じる
    public void AnimEvent_PlaySE_BattleCostPut() { SoundUtil.PlaySE(SEID.SE_BATLE_COST_PUT); }  //!< アニメーションイベント：SE再生指示：バトルSE：コスト追加成立
    public void AnimEvent_PlaySE_BattleSkillFix() { }
    // アニメーションイベント：SE再生指示：バトルSE：スキルが成立
    public void AnimEvent_PlaySE_BattleSkillFix2()
    {
    }

    // アニメーションイベント：SE再生指示：バトルSE：スキル置き換え
    public void AnimEvent_PlaySE_BatleSkillReplace()
    {
        SoundUtil.PlaySE(SEID.SE_BATLE_SKILL_REPLACE);
    }

    // アニメーションイベント：SE再生指示：スキルコンボフィニッシュ
    public void AnimEvent_PlaySE_BattleSkillComboFinish()
    {
        SoundUtil.PlaySE(SEID.SE_SKILL_COMBO_FINISH_WORD);
    }

    public void AnimEvent_PlaySE_BattleSkillExec() { }

    // アニメーションイベント：SE再生指示：チェス落下音
    public void AnimEvent_PlaySE_ChessFall()
    {
        SoundUtil.PlaySE(SEID.SE_CHESS_FALL);
    }

    // アニメーションイベント：SE再生指示：ドア開き：通常
    public void AnimEvent_PlaySE_DoorOpen_Normal()
    {
        SoundUtil.PlaySE(SEID.SE_DOOR_OPEN_NORMAL);
    }

    // アニメーションイベント：SE再生指示：ドア開き：ボス
    public void AnimEvent_PlaySE_DoorOpen_Boss()
    {
        SoundUtil.PlaySE(SEID.SE_DOOR_OPEN_BOSS);
    }

    // アニメーションイベント：SE再生指示：SP切れUI
    public void AnimEvent_PlaySE_SPLimitOver()
    {
        SoundUtil.PlaySE(SEID.SE_SPLIMITOVER);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		VOICE:アニメーションイベント：SE再生指示：鍵取得時の音
	*/
    //----------------------------------------------------------------------------
    public void AnimEvent_PlayVoice_DoorOpen()
    {
        SoundUtil.PlaySE(SEID.VOICE_INGAME_QUEST_GETKEY);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		VOICE:ボス登場
	*/
    //----------------------------------------------------------------------------
    public void AnimEvent_PlayVoice_BossAppear()
    {
        SoundUtil.PlaySE(SEID.VOICE_INGAME_QUEST_BOSSAPPEAR);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		VOICE:ゲームオーバー
	*/
    //----------------------------------------------------------------------------
    public void AnimEvent_PlayVoice_StageGameOver()
    {
        SoundUtil.PlaySE(SEID.VOICE_INGAME_QUEST_GAMEOVER);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		VOICE:LBS発動時ボイス
	*/
    //----------------------------------------------------------------------------
    public void AnimEvent_PlayVoice_StandReady()
    {
        SoundUtil.PlaySE(SEID.VOICE_INGAME_QUEST_STANDREADY);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		VOICE:SPlimit
	*/
    //----------------------------------------------------------------------------
    public void AnimEvent_PlayVoice_SPLimit()
    {
        SoundUtil.PlaySE(SEID.VOICE_INGAME_QUEST_SPLIMIT);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーションイベント：SE再生指示：カウントダウン
	*/
    //----------------------------------------------------------------------------
    public void AnimEvent_PlaySE_CountDown()
    {
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーションイベント：エフェクト再生指示：レアカード演出
	*/
    //----------------------------------------------------------------------------
    public void AnimEvent_EffectCardRare()
    {
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーションイベント：エフェクト再生指示：カード反転土煙演出
	*/
    //----------------------------------------------------------------------------
    public void AnimEvent_EffectCardSmoke()
    {
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーションイベント：エフェクト再生指示：カード発動エフェクト
	*/
    //----------------------------------------------------------------------------
    public void AnimEvent_EffectCardActivity()
    {
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーションイベント：エフェクト再生指示：カード発動エフェクト
	*/
    //----------------------------------------------------------------------------
    public void AnimEvent_EffectCardSummon()
    {
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーションイベント：戦闘カットイン特化処理：次のカットイン入場を許可
	*/
    //----------------------------------------------------------------------------
    private void AnimEvent_BattleCutinNextOK()
    {
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーションイベント：戦闘カットイン特化処理：カットイン効能発動
	*/
    //----------------------------------------------------------------------------
    private void AnimEvent_BattleCutinExecOK()
    {
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーションイベント：バトルダメージタイミング
	*/
    //----------------------------------------------------------------------------
    private void AnimEvent_BattleDamage()
    {
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーションイベント：ゴール扉の煙
	*/
    //----------------------------------------------------------------------------
    private void AnimEvent_DoorSmoke()
    {
    }

}

