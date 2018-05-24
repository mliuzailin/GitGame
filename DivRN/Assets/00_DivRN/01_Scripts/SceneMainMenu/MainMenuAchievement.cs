/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MainMenuAchievement.cs
	@brief	メインメニュー関連実績
	@author Developer
	@date 	2013/10/17
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
	@brief	メインメニューシーケンス：デバッグ枠：カットイン調整
*/
//----------------------------------------------------------------------------
static public class MainMenuAchievement
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /*
        const	int		INFINITY_DUNGEON_QUEST_001	= 233;		//!< 無限ダンジョンのクエストID：初級
        const	int		INFINITY_DUNGEON_QUEST_002	= 234;		//!< 無限ダンジョンのクエストID：中級
        const	int		INFINITY_DUNGEON_QUEST_003	= 235;		//!< 無限ダンジョンのクエストID：上級
        const	int		INFINITY_DUNGEON_QUEST_004	= 236;		//!< 無限ダンジョンのクエストID：超級
        const	int		INFINITY_DUNGEON_QUEST_005	= 237;		//!< 無限ダンジョンのクエストID：神級
    */
    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	GameCenter解除
	*/
    //----------------------------------------------------------------------------
    static public void GameCenterUnlockQuest(uint unQuestID)
    {
#if BUILD_TYPE_DEBUG
        //		Debug.Log( "Achievement QuestID = " + unQuestID );
#endif
        switch (unQuestID)
        {
            case GlobalDefine.INFINITY_DUNGEON_QUEST_001: PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eINFINITY_DUNGEON_LV0); break;
            case GlobalDefine.INFINITY_DUNGEON_QUEST_002: PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eINFINITY_DUNGEON_LV1); break;
            case GlobalDefine.INFINITY_DUNGEON_QUEST_003: PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eINFINITY_DUNGEON_LV2); break;
            case GlobalDefine.INFINITY_DUNGEON_QUEST_004: PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eINFINITY_DUNGEON_LV3); break;
            case GlobalDefine.INFINITY_DUNGEON_QUEST_005: PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eINFINITY_DUNGEON_LV4); break;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	GameCenter解除
	*/
    //----------------------------------------------------------------------------
    static public void GameCenterUnlockUnit()
    {
        if (UserDataAdmin.Instance == null
        || UserDataAdmin.Instance.m_StructPlayer == null
        || UserDataAdmin.Instance.m_StructPlayer.flag_unit_get == null
        )
        {
            return;
        }

        uint unUnitGetCt = ServerDataUtil.GetBitFlagCt(ref UserDataAdmin.Instance.m_StructPlayer.flag_unit_get);
#if BUILD_TYPE_DEBUG
        //		Debug.Log( "Achievement UnitGetCt = " + unUnitGetCt );
#endif

        if (unUnitGetCt >= 10)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eCOLLECT_010);
        }

        if (unUnitGetCt >= 50)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eCOLLECT_050);
        }

        if (unUnitGetCt >= 100)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eCOLLECT_100);
        }

        if (unUnitGetCt >= 150)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eCOLLECT_150);
        }

        if (unUnitGetCt >= 200)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eCOLLECT_200);
        }

        if (unUnitGetCt >= 250)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eCOLLECT_250);
        }

        if (unUnitGetCt >= 300)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eCOLLECT_300);
        }

        if (unUnitGetCt >= 350)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eCOLLECT_350);
        }

    }




    //----------------------------------------------------------------------------
    /*!
		@brief	GameCenter解除
	*/
    //----------------------------------------------------------------------------
    static public void GameCenterUnlockQuestRanking()
    {
        if (UserDataAdmin.Instance == null
        || UserDataAdmin.Instance.m_StructPlayer == null
        || UserDataAdmin.Instance.m_StructPlayer.flag_quest_clear == null
        )
        {
            return;
        }

        uint unQuestClearCt = ServerDataUtil.GetBitFlagCt(ref UserDataAdmin.Instance.m_StructPlayer.flag_quest_clear);
#if BUILD_TYPE_DEBUG
        //		Debug.Log( "Achievement QuestClearCt = " + unQuestClearCt );
#endif
        PlayGameServiceUtil.SubmitScore(ELEADERBORAD.eCLEAR_AREA_COUNT, (long)unQuestClearCt);
    }

}

