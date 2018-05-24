/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MainMenuUtil.cs
	@brief	メインメニュー関連ユーティリティ
	@author Developer
	@date 	2013/08/01
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
	@brief	メインメニュー関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class MainMenuUtil
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	ダミーユニット生成
	*/
    //----------------------------------------------------------------------------
    static public PacketStructUnit CreateDummyUnit(uint unUnitID, uint unUnitLevel, uint unUnitLevelLBS, uint unUnitLevelLO, uint unUnitAddAtk, uint unUnitAddHP,
                                                    uint unLinkPoint = 0, uint unLinkType = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE)
    {
        #region ==== 最大レベルチェック ====
        MasterDataParamChara chara_param = MasterDataUtil.GetCharaParamFromID(unUnitID);
        if (chara_param == null)
        {
            return null;
        }

        //	LBS最大レベルチェック
        MasterDataSkillLimitBreak lbs_param = MasterDataUtil.GetLimitBreakSkillParamFromID(chara_param.skill_limitbreak);
        if (lbs_param != null)
        {
            if (unUnitLevelLBS > lbs_param.level_max)
            {
                unUnitLevelLBS = (uint)lbs_param.level_max;
            }
        }
        #endregion

        PacketStructUnit cDummyUnit = new PacketStructUnit();
        cDummyUnit.id = unUnitID;
        cDummyUnit.level = unUnitLevel;
        cDummyUnit.limitbreak_lv = unUnitLevelLBS;
        cDummyUnit.limitover_lv = unUnitLevelLO;
        cDummyUnit.add_pow = unUnitAddAtk;
        cDummyUnit.add_hp = unUnitAddHP;
        cDummyUnit.add_def = 0;
        cDummyUnit.get_time = TimeUtil.ConvertLocalTimeToServerTime(TimeManager.Instance.m_TimeNow);
        cDummyUnit.unique_id = 1;

        if (chara_param != null)
        {
            cDummyUnit.exp = (uint)CharaUtil.GetStatusValue(chara_param, (int)unUnitLevel, CharaUtil.VALUE.EXP);
        }
        else
        {
            cDummyUnit.exp = 0;
        }

        cDummyUnit.link_info = unLinkType;
        switch (unLinkType)
        {
            case (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE:
                cDummyUnit.link_point = 0;
                cDummyUnit.link_unique_id = 0;
                break;

            case (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE:
            case (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK:
                cDummyUnit.link_point = unLinkPoint;
                cDummyUnit.link_unique_id = 1;
                break;
        }

        return cDummyUnit;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ダミーフレンド生成
	*/
    //----------------------------------------------------------------------------
    static public PacketStructFriend CreateDummyFriend(uint unUnitID, uint unUnitLevel, string strName = "Developer")
    {
        PacketStructFriend cDummyFriend = new PacketStructFriend();
        cDummyFriend.friend_point = 0;
        cDummyFriend.friend_state = 0;
        cDummyFriend.friend_state_update = 0;
        cDummyFriend.last_play = TimeUtil.ConvertLocalTimeToServerTime(TimeManager.Instance.m_TimeNow);
        cDummyFriend.user_id = 0;   // ※０番はサーバー側に許容してもらえる特殊ID
        cDummyFriend.user_name = strName;
        cDummyFriend.user_rank = 1;
        cDummyFriend.unit = new PacketStructUnit();
        cDummyFriend.unit.add_def = 0;
        cDummyFriend.unit.add_hp = 0;
        cDummyFriend.unit.add_pow = 0;
        cDummyFriend.unit.get_time = TimeUtil.ConvertLocalTimeToServerTime(TimeManager.Instance.m_TimeNow);
        cDummyFriend.unit.id = unUnitID;
        cDummyFriend.unit.level = unUnitLevel;
        cDummyFriend.unit.unique_id = 1;
        cDummyFriend.unit.limitbreak_lv = 0;
        cDummyFriend.unit.exp = (uint)CharaUtil.GetStatusValue(MasterDataUtil.GetCharaParamFromID(unUnitID), (int)unUnitLevel, CharaUtil.VALUE.EXP);

        return cDummyFriend;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	固定パーティ用のダミーフレンドユニットの生成
	*/
    //----------------------------------------------------------------------------
    static public PacketStructUnit CreateDummyFriendUnit(uint unit_id, uint unit_lv, uint add_atk, uint add_hp, uint add_def, uint lbs_lv, uint lo_lv,
                                                          uint unLinkPoint = 0, uint unLinkType = (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE)
    {
        MasterDataParamChara chara_param = MasterDataUtil.GetCharaParamFromID(unit_id);
        if (chara_param == null)
        {
            return null;
        }

        #region ==== 最大レベルチェック ====
        //	LBS最大レベルチェック
        MasterDataSkillLimitBreak lbs_param = MasterDataUtil.GetLimitBreakSkillParamFromID(chara_param.skill_limitbreak);
        if (lbs_param != null)
        {
            if (lbs_lv > lbs_param.level_max)
            {
                lbs_lv = (uint)lbs_param.level_max;
            }
        }
        #endregion

        #region ==== HP上限下限チェック ====
        //	HP上限下限チェック
        if (unit_lv > chara_param.level_max)
        {
            unit_lv = (uint)chara_param.level_max;
        }

        if (unit_lv < chara_param.level_min)
        {
            unit_lv = (uint)chara_param.level_min;
        }

        #endregion

        PacketStructUnit unit = new PacketStructUnit();
        if (unit == null)
        {
            return null;
        }
        unit.id = unit_id;
        unit.level = unit_lv;
        unit.add_pow = add_atk;
        unit.add_hp = add_hp;
        unit.add_def = add_def;
        unit.limitbreak_lv = lbs_lv;
        unit.limitover_lv = lo_lv;
        unit.get_time = TimeUtil.ConvertLocalTimeToServerTime(TimeManager.Instance.m_TimeNow);
        unit.unique_id = 1;

        //unit.exp			=	0;
        if (chara_param != null)
        {
            unit.exp = (uint)CharaUtil.GetStatusValue(chara_param, (int)unit_lv, CharaUtil.VALUE.EXP);
        }
        else
        {
            unit.exp = 0;
        }

        unit.link_info = unLinkType;
        switch (unLinkType)
        {
            case (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_NONE:
                unit.link_point = 0;
                unit.link_unique_id = 0;
                break;

            case (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE:
            case (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK:
                unit.link_point = unLinkPoint;
                unit.link_unique_id = 1;
                break;
        }

        return unit;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ランチタイムガチャの有効時間判定
		@note
	*/
    //----------------------------------------------------------------------------
    static public bool CheckLunchTimeGacha()
    {
        //--------------------------------
        //
        //--------------------------------
        if (UserDataAdmin.Instance == null
        || UserDataAdmin.Instance.m_StructPlayer == null
        )
        {
            return false;
        }

        if (TimeManager.Instance == null)
        {
            return false;
        }


        //--------------------------------
        // 最後にランチタイムガチャ引いた時と同日はもう引けない
        //--------------------------------
        if (UserDataAdmin.Instance.m_StructPlayer.lunch_play_time != 0)
        {
            DateTime cLunshGachaPlayTime = TimeUtil.ConvertServerTimeToLocalTime(UserDataAdmin.Instance.m_StructPlayer.lunch_play_time);
            DateTime cLunshGachaPlayDay = new DateTime(cLunshGachaPlayTime.Year, cLunshGachaPlayTime.Month, cLunshGachaPlayTime.Day);
            DateTime cNowDay = new DateTime(TimeManager.Instance.m_TimeNow.Year, TimeManager.Instance.m_TimeNow.Month, TimeManager.Instance.m_TimeNow.Day);
            if (cLunshGachaPlayDay == cNowDay)
            {
                return false;
            }
        }

        //--------------------------------
        // 開始タイミングと終了タイミングで不整合あるならはじく
        //--------------------------------
        if (MainMenuParam.m_LunchTimeStart >= MainMenuParam.m_LunchTimeEnd)
        {
            return false;
        }

        //--------------------------------
        // ランチタイム時間帯でないと引けない
        //--------------------------------
        {
            int nLunchTimeStartHour = MainMenuParam.m_LunchTimeStart / 100;
            int nLunchTimeStartMinute = MainMenuParam.m_LunchTimeStart % 100;
            int nLunchTimeEndHour = MainMenuParam.m_LunchTimeEnd / 100;
            int nLunchTimeEndMinute = MainMenuParam.m_LunchTimeEnd % 100;
            DateTime cLunchStart = new DateTime(TimeManager.Instance.m_TimeNow.Year
                                                , TimeManager.Instance.m_TimeNow.Month
                                                , TimeManager.Instance.m_TimeNow.Day
                                                , 0
                                                , 0
                                                , 0
                                                );
            DateTime cLunchEnd = new DateTime(TimeManager.Instance.m_TimeNow.Year
                                                , TimeManager.Instance.m_TimeNow.Month
                                                , TimeManager.Instance.m_TimeNow.Day
                                                , 0
                                                , 0
                                                , 0
                                                );
            cLunchStart = cLunchStart.AddHours(nLunchTimeStartHour);
            cLunchStart = cLunchStart.AddMinutes(nLunchTimeStartMinute);
            cLunchEnd = cLunchEnd.AddHours(nLunchTimeEndHour);
            cLunchEnd = cLunchEnd.AddMinutes(nLunchTimeEndMinute);

#if BUILD_TYPE_DEBUG
            //			Debug.Log( "LunchGacha - " + MainMenuParam.m_LunchTimeStart + " , " + MainMenuParam.m_LunchTimeEnd + "," + cLunchStart + " , " + cLunchEnd + " , " + cLunchEnd.AddMinutes(-1)  );
#endif
            bool bLunchTimeOK = false;
            if (TimeManager.Instance.m_TimeNow >= cLunchStart
            && TimeManager.Instance.m_TimeNow < cLunchEnd
            )
            {
                bLunchTimeOK = true;
            }

            if (bLunchTimeOK == false)
            {
                return false;
            }
        }

        //--------------------------------
        // 条件OK！
        //--------------------------------
        return true;
    }

    /// <summary>
    /// エリアカテゴリの作成
    /// </summary>
    /// <param name="area_cate_id"></param>
    /// <returns></returns>
    static public AreaDataContext CreateRNAreaCategory(uint area_cate_id, AreaSelectListItemModel model)
    {
        //
        if (area_cate_id == 0)
        {
            return null;
        }

        //
        MasterDataAreaCategory areaCategory = MasterFinder<MasterDataAreaCategory>.Instance.Find((int)area_cate_id);
        if (areaCategory == null)
        {
            return null;
        }

        MasterDataArea[] areaArray = MasterFinder<MasterDataArea>.Instance.SelectWhere(" where area_cate_id = ? ", area_cate_id).ToArray();

        //----------------------------------------------
        // エリアの存在チェック
        //----------------------------------------------
        bool hasArea = false;
        bool hasQuestCompleted = true;
        bool hasQuestCleard = true;
        bool hasQuestNew = false;
        bool hasBufEvent = false;
        MasterDataGuerrillaBoss guerrillaBoss = null;

        for (int chkArea = 0; chkArea < areaArray.Length; ++chkArea)
        {
            MasterDataArea areaData = areaArray[chkArea];
            if (areaData == null)
            {
                continue;
            }


            bool hasQuest = ChkActiveArea(areaCategory, areaData, ref hasArea, ref hasQuestCompleted, ref hasQuestCleard, ref hasQuestNew, ref guerrillaBoss);

            if (hasBufEvent == false && hasQuest)
            {
                MainMenuQuestSelect.AreaAmendParam amend = CreateAreaParamAmend(areaData);
                if (amend != null)
                {

                    if (amend.m_FlagAmendCoin == true ||
                        amend.m_FlagAmendDrop == true ||
                        amend.m_FlagAmendEXP == true ||
                        amend.m_FlagAmendGuerrillaBoss == true ||
                        amend.m_FlagAmendLinkPoint == true ||
                        amend.m_FlagAmendScore == true ||
                        amend.m_FlagAmendStamina == true ||
                        amend.m_FlagAmendTicket == true)
                    {
                        hasBufEvent = true;
                    }
                }
            }
        }

        // エリアが存在していたらデータを返す
        if (hasArea)
        {
            AreaDataContext newArea = new AreaDataContext(model);
            newArea.m_AreaIndex = area_cate_id;
            newArea.IsViewFlag = (hasQuestCleard || hasQuestCompleted);
            if (hasQuestCompleted)
            {
                newArea.FlagImage = ResourceManager.Instance.Load("completed");
            }
            else if (hasQuestCleard)
            {
                newArea.FlagImage = ResourceManager.Instance.Load("clear");
            }
            newArea.IsAreaNew = hasQuestNew;
            newArea.m_bufEvent = hasBufEvent;
            return newArea;
        }

        return null;
    }

    static public bool ChkAreaCleard(uint pre_area)
    {
        if (pre_area == 0)
        {
            return true;
        }

        MasterDataQuest2[] preQuestMasterArray = MasterFinder<MasterDataQuest2>.Instance.SelectWhere(" where area_id = ? ", pre_area).ToArray();

        bool bPreAreaCleard = true;
        for (int nChkQuest = 0; nChkQuest < preQuestMasterArray.Length; nChkQuest++)
        {
            MasterDataQuest2 _masterQuest2 = preQuestMasterArray[nChkQuest];

            if (_masterQuest2.story != 0)
            {
                continue;
            }

            if (_masterQuest2.active != MasterDataDefineLabel.BoolType.ENABLE)
            {
                continue;
            }

            //
            if (ServerDataUtil.ChkRenewBitFlag(ref UserDataAdmin.Instance.m_StructPlayer.flag_renew_quest_clear, _masterQuest2.fix_id) == true)
            {
                continue;
            }

            bPreAreaCleard = false;
            break;
        }
        return bPreAreaCleard;
    }

    static public bool ChkActiveArea(
        MasterDataAreaCategory _masterAreaCategory,
        MasterDataArea _masterArea,
        ref bool hasArea,
        ref bool hasQuestCompleted,
        ref bool hasQuestCleard,
        ref bool hasQuestNew,
        ref MasterDataGuerrillaBoss cAreaGuerrillaBossData
        )
    {
        //----------------------------------------
        // 期間限定エリアの出現チェック
        //----------------------------------------
        if (_masterAreaCategory.area_cate_type != MasterDataDefineLabel.AreaCategory.KEY) // キークエストはイベント期間チェックしない
        {
            if (TimeEventManager.Instance == null ||
            TimeEventManager.Instance.ChkEventActive(_masterArea.event_id) == false)
            {
                return false;
            }
        }

        //----------------------------------------
        // クエストキーチェック
        //----------------------------------------
        if (_masterAreaCategory.area_cate_type == MasterDataDefineLabel.AreaCategory.KEY) // キークエストの場合
        {
            //対象のエリアを参照する所持中のクエストキー情報を全て取得
            PacketStructQuestKey cKeyInfo = MainMenuUtil.GetHaveQuestKeyNumFromArea(_masterArea.fix_id);
            if (cKeyInfo == null)
            {
                return false;
            }
        }

        //----------------------------------------
        // 事前クリア条件になってるエリアが指定されてるならクリア状況チェック
        //----------------------------------------
        if (ChkAreaCleard(_masterArea.pre_area) == false)
        {
            return false;
        }

        //----------------------------------------------
        // クエストの存在チェック
        //----------------------------------------------
        bool hasQuest = false;
        bool hasNotClearQuest = false;
        //MasterDataQuest2[] workQuestMasterArray = Array.FindAll(questMasterArray, (v) => v.area_id == areaData.fix_id);
        MasterDataQuest2[] workQuestMasterArray = MasterFinder<MasterDataQuest2>.Instance.SelectWhere(" where area_id = ? ", _masterArea.fix_id).ToArray();
        PacketStructPlayer structplayer = UserDataAdmin.Instance.m_StructPlayer;

        for (int chkQquest = 0; chkQquest < workQuestMasterArray.Length; ++chkQquest)
        {
            MasterDataQuest2 questData = workQuestMasterArray[chkQquest];
            if (questData == null ||
                questData.area_id != _masterArea.fix_id ||
                questData.active != MasterDataDefineLabel.BoolType.ENABLE)
            {
                continue;
            }

            //----------------------------------------
            // 一度クリアしたら二度と出現しないクエストの場合、
            // 表示から間引く
            //----------------------------------------
            if (questData.once == MasterDataDefineLabel.BoolType.ENABLE)
            {
                if (ServerDataUtil.ChkRenewBitFlag(ref structplayer.flag_renew_quest_clear, questData.fix_id) == true)
                {
                    continue;
                }
            }

            hasQuest = true;
            //----------------------------------------
            // ストーリークエストのチェック
            //----------------------------------------
            if (questData.story > 0)
            {
                continue;
            }

            //----------------------------------------
            // ゲリラボスの存在チェック
            //----------------------------------------
            if (cAreaGuerrillaBossData == null)
            {
                cAreaGuerrillaBossData = MasterDataUtil.GetGuerrillaBossParamFromQuestID(questData.fix_id);
            }

            //----------------------------------------
            // コンプリート済みかどうかのチェック
            //----------------------------------------
            if (ServerDataUtil.ChkRenewBitFlag(ref structplayer.flag_renew_quest_mission_complete, questData.fix_id) == true)
            {
                continue;
            }
            hasQuestCompleted = false;

            //----------------------------------------
            // クリア済みかどうかのチェック
            //----------------------------------------
            if (ServerDataUtil.ChkRenewBitFlag(ref structplayer.flag_renew_quest_clear, questData.fix_id) == true)
            {
                continue;
            }
            hasQuestCleard = false;

            // 現在のクエスト以前にクリアしていないクエストがある場合はスキップ
            if (hasNotClearQuest == true)
            {
                continue;
            }
            hasNotClearQuest = true;

            //----------------------------------------
            // Newかどうかのチェック
            //----------------------------------------
            if (ServerDataUtil.ChkRenewBitFlag(ref structplayer.flag_renew_quest_check, questData.fix_id) == true)
            {
                continue;
            }
            hasQuestNew = true;
        }

        if (hasQuest)
        {
            hasArea = true;
        }

        return hasQuest;
    }

    static public uint ChkActiveNextArea(uint areaID)
    {
        MasterDataArea[] workAreaMasterArray = MasterFinder<MasterDataArea>.Instance.SelectWhere(" where pre_area = ? ", areaID).ToArray();
        if (workAreaMasterArray.IsNullOrEmpty() == true)
        {
            return 0;
        }
        if (ChkAreaCleard(areaID) == false)
        {
            return 0;
        }

        return workAreaMasterArray[0].fix_id;
    }

    static public bool ChkActiveStory(uint areaID)
    {
        MasterDataQuest2[] workQuestMasterArray = MasterFinder<MasterDataQuest2>.Instance.SelectWhere(" where area_id = ? ", areaID).ToArray();
        if (workQuestMasterArray.Length == 0)
        {
            return false;
        }

        MasterDataQuest2 questData = workQuestMasterArray[workQuestMasterArray.Length - 1];
        if (questData == null || questData.area_id != areaID || questData.active != MasterDataDefineLabel.BoolType.ENABLE)
        {
            return false;
        }

        //----------------------------------------
        // ストーリークエストのチェック
        //----------------------------------------
        if (questData.story == 0)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 開催中のチャプターが入っているリュージョンリストを作成する
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    static public List<MasterDataRegion> CreateRegionList(MasterDataDefineLabel.REGION_CATEGORY category)
    {
        List<MasterDataRegion> regionList = new List<MasterDataRegion>();
        List<MasterDataRegion> tmpRegionList = MasterFinder<MasterDataRegion>.Instance.SelectWhere("where category = ?", category);
        if (tmpRegionList == null ||
            tmpRegionList.Count == 0)
        {
            return regionList;
        }

        for (int cntRegion = 0; cntRegion < tmpRegionList.Count; ++cntRegion)
        {
            MasterDataRegion regionMaster = tmpRegionList[cntRegion];
            List<MasterDataAreaCategory> areaCategoryMasterArray = MasterFinder<MasterDataAreaCategory>.Instance.SelectWhere("where region_id = ?", regionMaster.fix_id);
            bool hasArea = false;

            for (int cntAreaCate = 0; cntAreaCate < areaCategoryMasterArray.Count; ++cntAreaCate)
            {
                MasterDataAreaCategory areaCategoryMaster = areaCategoryMasterArray[cntAreaCate];

                MasterDataArea[] areaArray = MasterFinder<MasterDataArea>.Instance.SelectWhere(" where area_cate_id = ? ", areaCategoryMaster.fix_id).ToArray();

                //----------------------------------------------
                // エリアの存在チェック
                //----------------------------------------------
                bool hasQuestCompleted = true;
                bool hasQuestCleard = true;
                bool hasQuestNew = false;
                MasterDataGuerrillaBoss guerrillaBoss = null;

                for (int cntArea = 0; cntArea < areaArray.Length; ++cntArea)
                {
                    MasterDataArea areaData = areaArray[cntArea];
                    if (areaData == null)
                    {
                        continue;
                    }

                    ChkActiveArea(areaCategoryMaster, areaData, ref hasArea, ref hasQuestCompleted, ref hasQuestCleard, ref hasQuestNew, ref guerrillaBoss);

                    if (hasArea == true)
                    {
                        // エリアの有無がわかればいのでループを抜ける
                        break;
                    }
                }

                if (hasArea == true)
                {
                    // エリアの有無がわかればいのでループを抜ける
                    break;
                }
            }

            if (hasArea == true)
            {
                regionList.Add(regionMaster);
            }
        }

        regionList.Sort((a, b) => (int)a.sort - (int)b.sort);
        return regionList;
    }

    /// <summary>
    /// MasterDataRegionに含まれている有効なMasterDataAreaCategory.fix_idを取得します
    /// </summary>
    /// <param name="regionMaster"></param>
    /// <returns></returns>
    static public uint[] CreateRegionMasterContainedAreaCategoryIDs(MasterDataRegion regionMaster)
    {
        List<uint> categoryIDList = new List<uint>();

        List<MasterDataAreaCategory> areaCategoryMasterArray = MasterFinder<MasterDataAreaCategory>.Instance.SelectWhere("where region_id = ?", regionMaster.fix_id);
        for (int cntAreaCate = 0; cntAreaCate < areaCategoryMasterArray.Count; ++cntAreaCate)
        {
            MasterDataAreaCategory areaCategoryMaster = areaCategoryMasterArray[cntAreaCate];
            MasterDataArea[] areaArray = MasterFinder<MasterDataArea>.Instance.SelectWhere(" where area_cate_id = ? ", areaCategoryMaster.fix_id).ToArray();

            //----------------------------------------------
            // エリアの存在チェック
            //----------------------------------------------
            bool hasArea = false;
            bool hasQuestCompleted = true;
            bool hasQuestCleard = true;
            bool hasQuestNew = false;
            MasterDataGuerrillaBoss guerrillaBoss = null;
            for (int cntArea = 0; cntArea < areaArray.Length; ++cntArea)
            {
                MasterDataArea areaData = areaArray[cntArea];
                if (areaData == null)
                {
                    continue;
                }

                ChkActiveArea(areaCategoryMaster, areaData, ref hasArea, ref hasQuestCompleted, ref hasQuestCleard, ref hasQuestNew, ref guerrillaBoss);

                if (hasArea == true)
                {
                    // エリアの有無がわかればいのでループを抜ける
                    break;
                }
            }

            if (hasArea == true)
            {
                categoryIDList.Add(areaCategoryMaster.fix_id);
            }
        }

        return categoryIDList.ToArray();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	初心者ブースト、エリア補正チェック
		@note
	*/
    //----------------------------------------------------------------------------
    static public MainMenuQuestSelect.AreaAmendParam CreateAreaParamAmend(MasterDataArea cAreaMasterData)
    {
        MainMenuQuestSelect.AreaAmendParam cAreaParam = new MainMenuQuestSelect.AreaAmendParam();
        cAreaParam.m_AreaMasterDataAmendList = new TemplateList<MasterDataAreaAmend>();

        //----------------------------------------
        // このエリアに現状掛かっている補正を求めて表示。
        // 複数効果が同時発動する可能性を考慮して総当たりチェック
        //----------------------------------------
        cAreaParam.m_QuestSelectAreaAmendStamina = 100;
        cAreaParam.m_QuestSelectAreaAmendEXP = 100;
        cAreaParam.m_QuestSelectAreaAmendCoin = 100;
        cAreaParam.m_QuestSelectAreaAmendDrop = 100;
        cAreaParam.m_QuestSelectAreaAmendTicket = 100;
        cAreaParam.m_QuestSelectAreaAmendGuerrillaBoss = 100;
        cAreaParam.m_QuestSelectAreaAmendLinkPoint = 100;
        cAreaParam.m_QuestSelectAreaAmendScore = 100;

        cAreaParam.m_FlagAmendStamina = false;
        cAreaParam.m_FlagAmendEXP = false;
        cAreaParam.m_FlagAmendCoin = false;
        cAreaParam.m_FlagAmendDrop = false;
        cAreaParam.m_FlagAmendTicket = false;
        cAreaParam.m_FlagAmendGuerrillaBoss = false;
        cAreaParam.m_FlagAmendLinkPoint = false;
        cAreaParam.m_FlagAmendScore = false;

        MasterDataAreaAmend cAreaAmentChk = null;

        //----------------------------------------
        //	通常処理より初心者ブーストを優先的に補正効果を掛ける。
        //----------------------------------------
        if (MainMenuParam.m_BeginnerBoost != null)
        {
            //----------------------------------------
            // 初心者ブースト適用
            //----------------------------------------
            //----------------------------------------
            // 初心者ブーストの補正
            //----------------------------------------
            cAreaParam.m_QuestSelectAreaAmendStamina = MainMenuParam.m_BeginnerBoost.boost_stamina_use; // エリア補正タイプ：補正：スタミナ
            cAreaParam.m_QuestSelectAreaAmendEXP = MainMenuParam.m_BeginnerBoost.boost_exp;         // エリア補正タイプ：補正：経験値アップ
            cAreaParam.m_QuestSelectAreaAmendCoin = MainMenuParam.m_BeginnerBoost.boost_money;        // エリア補正タイプ：補正：コイン

            //----------------------------------------
            // エリア補正情報表示更新。
            //
            // ※初心者ブースト発動中はドロップ率上昇、チケット取得数増加以外のイベントは適用されない
            // 　フラグ判定は行う
            //----------------------------------------
            //	スタミナ補正
            cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.STAMINA);
            if (cAreaAmentChk != null)
            {
                cAreaParam.m_FlagAmendStamina = true;
            }

            //	経験地入手量補正
            cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.EXP);
            if (cAreaAmentChk != null)
            {
                cAreaParam.m_FlagAmendEXP = true;
            }

            //	お金入手量補正
            cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.MONEY);
            if (cAreaAmentChk != null)
            {
                cAreaParam.m_FlagAmendCoin = true;
            }
            //	ドロップ率補正
            cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.DROP);
            if (cAreaAmentChk != null)
            {
                cAreaParam.m_QuestSelectAreaAmendDrop = cAreaAmentChk.amend_value;
                cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                cAreaParam.m_FlagAmendDrop = true;
            }

            //	チケット入手量補正
            cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.TICKET);
            if (cAreaAmentChk != null)
            {
                cAreaParam.m_QuestSelectAreaAmendTicket = cAreaAmentChk.amend_value;
                cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                cAreaParam.m_FlagAmendTicket = true;
            }
            //	ゲリラボス出現率補正
            cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.GUERRILLABOSS);
            if (cAreaAmentChk != null)
            {
                cAreaParam.m_QuestSelectAreaAmendGuerrillaBoss = cAreaAmentChk.amend_value;
                cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                cAreaParam.m_FlagAmendGuerrillaBoss = true;
            }
            //	リンクポイント補正
            cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.LINKPOINT);
            if (cAreaAmentChk != null)
            {
                cAreaParam.m_QuestSelectAreaAmendLinkPoint = cAreaAmentChk.amend_value;
                cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                cAreaParam.m_FlagAmendLinkPoint = true;
            }
            //  スコア補正
            cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.SCORE);
            if (cAreaAmentChk != null)
            {
                cAreaParam.m_QuestSelectAreaAmendScore = cAreaAmentChk.amend_value;
                cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                cAreaParam.m_FlagAmendScore = true;
            }
        }
        else
        {
            //----------------------------------------
            // 通常処理
            //----------------------------------------
            {
                //	スタミナ補正
                cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.STAMINA);
                if (cAreaAmentChk != null)
                {
                    cAreaParam.m_QuestSelectAreaAmendStamina = cAreaAmentChk.amend_value;
                    cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                    cAreaParam.m_FlagAmendStamina = true;
                }

                //	経験地入手量補正
                cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.EXP);
                if (cAreaAmentChk != null)
                {
                    cAreaParam.m_QuestSelectAreaAmendEXP = cAreaAmentChk.amend_value;
                    cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                    cAreaParam.m_FlagAmendEXP = true;
                }

                //	お金入手量補正
                cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.MONEY);
                if (cAreaAmentChk != null)
                {
                    cAreaParam.m_QuestSelectAreaAmendCoin = cAreaAmentChk.amend_value;
                    cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                    cAreaParam.m_FlagAmendCoin = true;
                }

                //	チケット入手量補正
                cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.TICKET);
                if (cAreaAmentChk != null)
                {
                    cAreaParam.m_QuestSelectAreaAmendTicket = cAreaAmentChk.amend_value;
                    cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                    cAreaParam.m_FlagAmendTicket = true;
                }

                //	ドロップ率補正
                cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.DROP);
                if (cAreaAmentChk != null)
                {
                    cAreaParam.m_QuestSelectAreaAmendDrop = cAreaAmentChk.amend_value;
                    cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                    cAreaParam.m_FlagAmendDrop = true;
                }
                //	ゲリラボス出現率補正
                cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.GUERRILLABOSS);
                if (cAreaAmentChk != null)
                {
                    cAreaParam.m_QuestSelectAreaAmendGuerrillaBoss = cAreaAmentChk.amend_value;
                    cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                    cAreaParam.m_FlagAmendGuerrillaBoss = true;
                }
                //	リンクポイント補正
                cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.LINKPOINT);
                if (cAreaAmentChk != null)
                {
                    cAreaParam.m_QuestSelectAreaAmendLinkPoint = cAreaAmentChk.amend_value;
                    cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                    cAreaParam.m_FlagAmendLinkPoint = true;
                }
                //  スコア補正
                cAreaAmentChk = MasterDataUtil.GetAreaAmendParamFromAreaID(cAreaMasterData.fix_id, MasterDataDefineLabel.AmendType.SCORE);
                if (cAreaAmentChk != null)
                {
                    cAreaParam.m_QuestSelectAreaAmendScore = cAreaAmentChk.amend_value;
                    cAreaParam.m_AreaMasterDataAmendList.Add(cAreaAmentChk);
                    cAreaParam.m_FlagAmendScore = true;
                }
            }
        }

        return cAreaParam;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	パラメータリミットチェック
		@note	コイン、チケット、フレンドポイント、チップ、ユニットポイント
	*/
    //----------------------------------------------------------------------------
    static public PRM_LIMIT_ERROR_TYPE ChkPrmLimit(uint cAddCoin, uint cAddTicket, uint cAddFp, uint cAddTip, uint cAddUnitPoint)
    {
        PRM_LIMIT_ERROR_TYPE errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_CHECK_OK;
        bool bChk = true;

        //コイン
        if (cAddCoin > 0)
        {
            uint iHaveCoinAfter = UserDataAdmin.Instance.m_StructPlayer.have_money + cAddCoin;
            if (iHaveCoinAfter > GlobalDefine.VALUE_HAVE_MAX_COIN)
            {
                //所持限界！
                errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_COIN;
                bChk = false;
            }
        }

        //チケット
        if (cAddTicket > 0)
        {
            uint iHaveTicketAfter = UserDataAdmin.Instance.m_StructPlayer.have_ticket + cAddTicket;
            if (iHaveTicketAfter > GlobalDefine.VALUE_HAVE_MAX_TICKET)
            {
                //所持限界！
                if (bChk == false)
                {
                    errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_OTHER;
                    return errorType;
                }

                errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_TICKET;
                bChk = false;
            }
        }

        //フレンドポイント
        if (cAddFp > 0)
        {
            uint iHaveFpAfter = UserDataAdmin.Instance.m_StructPlayer.have_friend_pt + cAddFp;
            if (iHaveFpAfter > GlobalDefine.VALUE_HAVE_MAX_FRIEND_PT)
            {
                //所持限界！
                if (bChk == false)
                {
                    errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_OTHER;
                    return errorType;
                }

                errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_FP;
                bChk = false;
            }
        }

        //チップ
        if (cAddTip > 0)
        {
            uint iHaveTipAfter = UserDataAdmin.Instance.m_StructPlayer.have_stone + cAddTip;
            if (iHaveTipAfter > GlobalDefine.VALUE_HAVE_MAX_STONE)
            {
                //所持限界！
                if (bChk == false)
                {
                    errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_OTHER;
                    return errorType;
                }

                errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_TIP;
                bChk = false;
            }
        }

        //ユニットポイント
        if (cAddUnitPoint > 0)
        {
            uint iHaveUPAfter = UserDataAdmin.Instance.m_StructPlayer.have_unit_point + cAddUnitPoint;
            if (iHaveUPAfter > GlobalDefine.VALUE_HAVE_MAX_UNIT_POINT)
            {
                //所持限界！
                if (bChk == false)
                {
                    errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_OTHER;
                    return errorType;
                }

                errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_UNITPOINT;
                bChk = false;
            }
        }

        return errorType;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	パラメータリミットチェック２
		@note	消費アイテムチェック用
		@param1	[unAddItem]			増加個数
		@param2	[nItemId]			消費アイテムID：0以下で指定なしの場合は全チェック
		@param3	[cBfChkErrorType]	事前のパラメータチェック結果
	*/
    //----------------------------------------------------------------------------
    static public PRM_LIMIT_ERROR_TYPE ChkPrmLimitItem(uint unAddItem, int nItemId, PRM_LIMIT_ERROR_TYPE cBfChkErrorType)
    {
        PRM_LIMIT_ERROR_TYPE errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_CHECK_OK;
        bool bChk = true;

        //----------------------------------------
        // 事前にパラメータチェックでエラーだった際は、チェックフラグを切り替えておく
        //----------------------------------------
        if (cBfChkErrorType != PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_CHECK_OK)
        {
            bChk = false;
            errorType = cBfChkErrorType;
        }

        //----------------------------------------
        // 所持しているアイテムリストを取得
        //----------------------------------------
        PacketStructUseItem[] cItemList = UserDataAdmin.Instance.m_StructPlayer.item_list;

        if (cItemList == null
        || cItemList.Length <= 0)
        {
            return errorType;
        }

        uint iHaveItemAfter = 0;

        for (int i = 0; i < cItemList.Length; i++)
        {
            if (cItemList[i] == null)
            {
                continue;
            }

            //ID指定ありの場合は、IDチェック
            if (nItemId > 0)
            {
                if (cItemList[i].item_id != (uint)nItemId)
                {
                    continue;
                }
            }

            //--------------------------------
            // 消費アイテムマスターデータ取得
            //--------------------------------
            MasterDataUseItem cMasterDataUseItem = MasterDataUtil.GetMasterDataUseItemFromID(cItemList[i].item_id);

            if (cMasterDataUseItem == null)
            {
                Debug.LogError("UseItem MasterData None! - " + cItemList[i].item_id);
                //マスターない！おかしい！
                return errorType;
            }

            int haveItemMax;
            if (MasterDataUtil.ChkUseItemTypeStaminaRecovery(cMasterDataUseItem) || MasterDataUtil.ChkUseItemTypeAmend(cMasterDataUseItem))
            {
                // 消費アイテム
                haveItemMax = GlobalDefine.VALUE_HAVE_MAX_ITEM;
            }
            else
            {
                // スクラッチチケット(ポイント)
                haveItemMax = GlobalDefine.VALUE_HAVE_MAX_ITEM_GACHA;
            }

            iHaveItemAfter = cItemList[i].item_cnt + unAddItem;

            //所持限界！
            if (iHaveItemAfter > haveItemMax)
            {
                if (bChk == false)
                {
                    errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_OTHER;
                    break;
                }

                errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_ITEM;
                break;
            }

            //ID指定ありの場合は終了
            if (nItemId > 0)
            {
                break;
            }
        }

        return errorType;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	パラメータリミットチェック３
		@note	クエストキーチェック用
		@param1	[unAddKey]			増加個数
		@param2	[nKeyId]			クエストキーID：0以下で指定なしの場合は全チェック
		@param3	[cBfChkErrorType]	事前のパラメータチェック結果
	*/
    //----------------------------------------------------------------------------
    static public PRM_LIMIT_ERROR_TYPE ChkPrmLimitQuestKey(uint unAddKey, int nKeyId, PRM_LIMIT_ERROR_TYPE cBfChkErrorType)
    {
        PRM_LIMIT_ERROR_TYPE errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_CHECK_OK;
        bool bChk = true;

        //----------------------------------------
        // 事前にパラメータチェックでエラーだった際は、チェックフラグを切り替えておく
        //----------------------------------------
        if (cBfChkErrorType != PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_CHECK_OK)
        {
            bChk = false;
            errorType = cBfChkErrorType;
        }

        //----------------------------------------
        // 所持しているクエストキーリストを取得
        //----------------------------------------
        PacketStructQuestKey[] cHaveKeyList = UserDataAdmin.Instance.m_StructPlayer.quest_key_list;

        if (cHaveKeyList == null
        || cHaveKeyList.Length <= 0)
        {
            return errorType;
        }

        uint iHaveKeyAfter = 0;

        for (int i = 0; i < cHaveKeyList.Length; i++)
        {
            if (cHaveKeyList[i] == null)
            {
                continue;
            }

            //ID指定ありの場合は、IDチェック
            if (nKeyId > 0)
            {
                if (cHaveKeyList[i].quest_key_id != (uint)nKeyId)
                {
                    continue;
                }
            }

            //--------------------------------
            // クエストキーマスターデータ取得
            //--------------------------------
            MasterDataQuestKey cMasterDataQuestKey = MasterDataUtil.GetMasterDataQuestKeyFromID(cHaveKeyList[i].quest_key_id);

            if (cMasterDataQuestKey == null)
            {
                Debug.LogError("UseItem MasterData None! - " + cHaveKeyList[i].quest_key_id);
                //マスターない！おかしい！
                return errorType;
            }

            //ID指定なしの場合は、期限外を間引く
            if (nKeyId <= 0)
            {
                if (cMasterDataQuestKey.timing_end > 0)
                {
                    //期限チェック
                    bool bCheckWithinTime = TimeManager.Instance.CheckWithinTime(cMasterDataQuestKey.timing_end);
                    if (bCheckWithinTime == false)
                    {
                        //キーが期限を過ぎていたので間引く
                        continue;
                    }
                }
            }

            iHaveKeyAfter = cHaveKeyList[i].quest_key_cnt + unAddKey;

            //所持限界！
            if (iHaveKeyAfter > GlobalDefine.VALUE_HAVE_MAX_QUEST_KEY)
            {
                if (bChk == false)
                {
                    errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_OTHER;
                    break;
                }

                errorType = PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_QUEST_KEY;
                break;
            }

            //ID指定ありの場合は終了
            if (nKeyId > 0)
            {
                break;
            }
        }

        return errorType;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	フレンドポイント取得処理
		@param[in]	PacketStructFriend	(cHelper)		助っ人情報
		@param[in]	uint				(uEventFP)		発生しているイベントID
		@return		取得できるフレンドポイント
	*/
    //----------------------------------------------------------------------------
    static public uint GetSelectFriendPoint(PacketStructFriend cHelper, uint uEventFP)
    {
        uint unGetFriendPt = 0;

        if (cHelper == null)
        {
            Debug.LogError("SelectFriendNone");
            return 0;
        }

        //----------------------------------------
        // フレンド使用のサイクルをもとにFP発生判定。
        // ここでも使用宣言が走るので使用情報を書き込んでおく
        //----------------------------------------
        bool bFriendPointActive = (LocalSaveManager.Instance.GetLocalSaveUseFriend(cHelper.user_id) == null);
        if (bFriendPointActive == true)
        {
            switch (cHelper.friend_state)
            {
                case (int)FRIEND_STATE.FRIEND_STATE_SUCCESS: unGetFriendPt = 20; break;
                case (int)FRIEND_STATE.FRIEND_STATE_WAIT_ME: unGetFriendPt = 5; break;
                case (int)FRIEND_STATE.FRIEND_STATE_WAIT_HIM: unGetFriendPt = 5; break;
                case (int)FRIEND_STATE.FRIEND_STATE_UNRELATED: unGetFriendPt = 5; break;
                case (int)FRIEND_STATE.FRIEND_STATE_PREMIUM: unGetFriendPt = 50; break;
            }

            //----------------------------------------
            // 発生しているイベントによって倍率変動
            // ※切り上げ計算はfloat誤差怖いんで自力計算してる
            //----------------------------------------
            float fEventPoint = unGetFriendPt;
            switch (uEventFP)
            {
                case GlobalDefine.FP_EVENT_ID_x0150: fEventPoint *= 1.5f; break;    // フレンドポイント増加イベントID：1.5倍
                case GlobalDefine.FP_EVENT_ID_x0200: fEventPoint *= 2.0f; break;    // フレンドポイント増加イベントID：2.0倍
                case GlobalDefine.FP_EVENT_ID_x0250: fEventPoint *= 2.5f; break;    // フレンドポイント増加イベントID：2.5倍
                case GlobalDefine.FP_EVENT_ID_x0300: fEventPoint *= 3.0f; break;    // フレンドポイント増加イベントID：3.0倍
                case GlobalDefine.FP_EVENT_ID_x0400: fEventPoint *= 4.0f; break;    // フレンドポイント増加イベントID：4.0倍
                case GlobalDefine.FP_EVENT_ID_x0500: fEventPoint *= 5.0f; break;    // フレンドポイント増加イベントID：5.0倍
                case GlobalDefine.FP_EVENT_ID_x1000: fEventPoint *= 10.0f; break;   // フレンドポイント増加イベントID：10.0倍
            }

            //----------------------------------------
            // アイテム：フレンドポイントブーストの効果チェック
            //	事前チェック、リザルトのフレンドポイント表示用に計算
            //	実際の補正計算はサーバー側で行っている。
            //----------------------------------------
            // プレイヤー情報に持っているアイテムリストを取得
            PacketStructUseItem[] cItemList = UserDataAdmin.Instance.m_StructPlayer.item_list;
            if (cItemList != null)
            {
                for (int i = 0; i < cItemList.Length; i++)
                {
                    if (cItemList[i] == null)
                    {
                        continue;
                    }

                    // 消費アイテムマスターデータ取得
                    MasterDataUseItem cMasterDataUseItem = MasterDataUtil.GetMasterDataUseItemFromID(cItemList[i].item_id);

                    //item_masterにアイテムが登録されていない
                    if (cMasterDataUseItem == null)
                    {
                        continue;
                    }

                    //フレンドポイント補正効果がないやつはスルー
                    if (cMasterDataUseItem.fp_amend <= 0)
                    {
                        continue;
                    }

                    //アイテム使用中かチェック
                    if (cItemList[i].use_timing > 0 && cMasterDataUseItem.effect_span_m > 0)
                    {
                        //--------------------------------
                        // 時間変換
                        //--------------------------------
                        //現在時間
                        DateTime cTimeNow = TimeManager.Instance.m_TimeNow;
                        //アイテム使用開始時間
                        DateTime cItemUseStart = TimeUtil.GetDateTimeSec(cItemList[i].use_timing);
                        //アイテム使用トータル時間
                        TimeSpan cItemUseSpan = cTimeNow - cItemUseStart;
                        //マスタに設定されている効果時間
                        TimeSpan cEffectSpan = TimeSpan.FromMinutes(cMasterDataUseItem.effect_span_m);

                        //--------------------------------
                        // 効果時間チェック
                        //--------------------------------
                        // 効果時間内の場合のみ補正倍率反映
                        if (cItemUseSpan < cEffectSpan)
                        {
                            //--------------------------------
                            //フレンドポイント補正アイテム使用中なので
                            //補正倍率を適用する
                            //--------------------------------
                            fEventPoint = (cMasterDataUseItem.fp_amend * fEventPoint) / 100;
                        }

                        //基本的に効果が重複するアイテムは二重で使用できないので、ここまできたらループせずに終了
                        break;
                    }
                }
            }

            //小数点以下切り上げ計算
            unGetFriendPt = (uint)fEventPoint;
            if ((fEventPoint * 10) % 10 > 0)
            {
                unGetFriendPt += 1;
            }
        }

        return unGetFriendPt;
    }

    /// <summary>
    /// 属性ごとのサークルスプライト取得
    /// </summary>
    /// <param name="elementType"></param>
    /// <returns></returns>
    static public Sprite GetElementCircleSprite(MasterDataDefineLabel.ElementType elementType)
    {
        string sprite_name = "";
        switch (elementType)
        {
            case MasterDataDefineLabel.ElementType.FIRE:
                sprite_name = "attribute_circle_red";
                break;
            case MasterDataDefineLabel.ElementType.WATER:
                sprite_name = "attribute_circle_bl";
                break;
            case MasterDataDefineLabel.ElementType.WIND:
                sprite_name = "attribute_circle_gr";
                break;
            case MasterDataDefineLabel.ElementType.LIGHT:
                sprite_name = "attribute_circle_ye";
                break;
            case MasterDataDefineLabel.ElementType.DARK:
                sprite_name = "attribute_circle_pu";
                break;
            case MasterDataDefineLabel.ElementType.NAUGHT:
                sprite_name = "attribute_circle_grnot";
                break;
        }

        if (sprite_name.IsNullOrEmpty()) { return null; }

        return ResourceManager.Instance.Load(sprite_name, ResourceType.Common);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	クエストキー所持チェック
		@note	対象のクエストキーが現在有効かをチェック
	*/
    //----------------------------------------------------------------------------
    static public bool ChkQuestKeyPlayableFromId(MasterDataQuestKey key_master)
    {
        if (key_master == null)
        {
            return false;
        }

        //----------------------------------------
        // 期限チェック
        //----------------------------------------
        if (key_master.timing_end > 0)
        {
            //開始タイミングはないため固定日付指定
            bool bCheckWithinTime = TimeManager.Instance.CheckWithinTime(key_master.timing_end);
            if (bCheckWithinTime == false)
            {
                return false;
            }
        }

        //エリアマスタ取得
        MasterDataArea[] cAreaMasters = MasterFinder<MasterDataArea>.Instance.SelectWhere(" where area_cate_id = ? ", key_master.key_area_category_id).ToArray();
        if (cAreaMasters == null)
        {
            return false;
        }

        //一つでも有効なエリアがあれば有効
        for (int i = 0; i < cAreaMasters.Length; i++)
        {
            //----------------------------------------
            // 事前クリア条件になってるエリアが指定されてるならクリア状況チェック
            // 通常チェックの場合のみ
            //----------------------------------------
            if (cAreaMasters[i].pre_area != 0)
            {
                bool bPreAreaCleard = true;
                MasterDataQuest2[] questArray = MasterFinder<MasterDataQuest2>.Instance.SelectWhere(" where area_id = ? ", cAreaMasters[i].pre_area).ToArray();
                for (int nChkQuest = 0; nChkQuest < questArray.Length; nChkQuest++)
                {
                    MasterDataQuest2 _masterQuest2 = questArray[nChkQuest];
                    if (_masterQuest2.active != MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        continue;
                    }

                    if (ServerDataUtil.ChkRenewBitFlag(ref UserDataAdmin.Instance.m_StructPlayer.flag_renew_quest_clear, _masterQuest2.fix_id) == true)
                    {
                        continue;
                    }

                    bPreAreaCleard = false;
                    break;
                }
                if (bPreAreaCleard) return true;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	現在所持しているクエストキー情報取得
		@note	現在有効なクエストキー情報取得を所持しているかチェック（クエストキーマスタ順でチェック）
	*/
    //----------------------------------------------------------------------------
    static public PacketStructQuestKey GetHaveQuestKeyNumFromArea(uint unAreaId)
    {
        //エリアマスタ取得
        MasterDataArea cAreaMaster = MasterDataUtil.GetAreaParamFromID(unAreaId);
        if (cAreaMaster == null)
        {
            return null;
        }

        //----------------------------------------
        // 現在所持しているキーリストを取得
        //----------------------------------------
        PacketStructQuestKey[] cKeyList = UserDataAdmin.Instance.m_StructPlayer.quest_key_list;
        if (cKeyList == null
        && cKeyList.Length <= 0)
        {
            return null;
        }

        //----------------------------------------
        // クエストキーマスタ順に、現在有効な対象エリアのキーを所持しているかチェック
        //----------------------------------------
        MasterDataQuestKey[] questkeys = MasterFinder<MasterDataQuestKey>.Instance.GetAll();

        for (int iKey = 0; iKey < questkeys.Length; iKey++)
        {
            //----------------------------------------
            // エリアIDチェック
            //----------------------------------------
            MasterDataQuestKey cQuestKeyMaster = questkeys[iKey];
            if (cQuestKeyMaster == null
            || cQuestKeyMaster.key_area_category_id != cAreaMaster.fix_id)
            {
                continue;
            }

            //----------------------------------------
            // 所持中のキーとのチェック
            //----------------------------------------
            for (int iHaveKey = 0; iHaveKey < cKeyList.Length; iHaveKey++)
            {
                //----------------------------------------
                // クエストキーIDと個数チェック
                //----------------------------------------
                if (cKeyList[iHaveKey] == null
                || cKeyList[iHaveKey].quest_key_cnt <= 0
                || cKeyList[iHaveKey].quest_key_id != cQuestKeyMaster.fix_id)
                {
                    continue;
                }

                //----------------------------------------
                // 期限チェック
                //----------------------------------------
                if (cQuestKeyMaster.timing_end > 0)
                {
                    //開始タイミングはないため固定日付指定
                    bool bCheckWithinTime = TimeManager.Instance.CheckWithinTime(cQuestKeyMaster.timing_end);
                    if (bCheckWithinTime == false)
                    {
                        continue;
                    }
                }

                return cKeyList[iHaveKey];
            }
        }

        return null;
    }


    /// <summary>
    /// フレンドリストの情報作成
    /// </summary>
    static public void CreateFriendList(ref FriendList friendList, FRIEND_STATE _state, Action<FriendDataItem> selectIcon = null, Action<FriendDataItem> selectFriend = null)
    {
        PacketStructFriend[] friendDataList = new PacketStructFriend[0];
        if (!UserDataAdmin.Instance.m_StructFriendList.IsNullOrEmpty())
        {
            friendDataList = Array.FindAll(UserDataAdmin.Instance.m_StructFriendList,
                                                            value => value != null && value.friend_state == (uint)_state); // フレンドを抽出
        }

        List<FriendDataSetting> friends = new List<FriendDataSetting>();
        List<MasterDataParamChara> charaMasterList = MasterFinder<MasterDataParamChara>.Instance.FindAll();
        FriendDataSetting friend;

        for (int i = 0; i < friendDataList.Length; i++)
        {
            PacketStructFriend friendData = friendDataList[i];
            friend = new FriendDataSetting();
            friend.FriendData = friendData;
            friend.MasterData = charaMasterList.Find((v) => v.fix_id == friendData.unit.id);
            friend.CharaOnce = CreateFriendCharaOnce(friendData);
            friend.m_Flag = FriendDataItem.FlagType.NONE;
            friend.IsEnableButton = true;

            // デリゲート設定
            if (selectIcon != null)
            {
                friend.DidSelectIcon += selectIcon;
            }
            if (selectFriend != null)
            {
                friend.DidSelectFriend += selectFriend;
            }

            friends.Add(friend);
        }

        //
        friendList.FriendBaseList.Body.Clear();
        for (int i = 0; i < friends.Count; i++)
        {
            //ソート用パラメータ設定
            friends[i].setSortParamFriend(friends[i].FriendData, friends[i].CharaOnce, friends[i].MasterData);
            friendList.FriendBaseList.Body.Add(friends[i]);
        }
    }

    public static CharaOnce CreateFriendCharaOnce(PacketStructFriend friendData)
    {
        CharaOnce friendCharaOnce = new CharaOnce();

        PacketStructUnit _sub = null;
        if (friendData.unit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE)
        {
            _sub = friendData.unit_link;
        }

        if (friendData.unit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE && _sub != null)
        {
            friendCharaOnce.CharaSetupFromID(
                friendData.unit.id,
                (int)friendData.unit.level,
                (int)friendData.unit.limitbreak_lv,
                (int)friendData.unit.limitover_lv,
                (int)friendData.unit.add_pow,
                (int)friendData.unit.add_hp,
                _sub.id,
                (int)_sub.level,
                (int)_sub.add_pow,
                (int)_sub.add_hp,
                (int)friendData.unit.link_point,
                (int)_sub.limitover_lv
                );
        }
        else
        {
            friendCharaOnce.CharaSetupFromID(
                friendData.unit.id,
                (int)friendData.unit.level,
                (int)friendData.unit.limitbreak_lv,
                (int)friendData.unit.limitover_lv,
                (int)friendData.unit.add_pow,
                (int)friendData.unit.add_hp,
                0,
                0,
                0,
                0,
                0,
                0
                );
        }

        return friendCharaOnce;
    }

    public static bool ChkFavoridFriend(uint user_id)
    {
        bool FavoriteNow = false;
        //----------------------------------------
        // 現状のお気に入り状況を判断
        //----------------------------------------
        TemplateList<uint> cFriendFavoriteList = LocalSaveManager.Instance.LoadFuncAddFavoriteFriend();
        if (cFriendFavoriteList == null)
        {
            FavoriteNow = false;
        }
        else
        {
            FavoriteNow = false;
            for (int i = 0; i < cFriendFavoriteList.m_BufferSize; i++)
            {
                if (cFriendFavoriteList[i] != user_id)
                {
                    continue;
                }

                FavoriteNow = true;
                break;
            }
        }

        return FavoriteNow;
    }

    /// <summary>
    /// ユニットがお気に入りされているかどうかチェックする
    /// </summary>
    /// <param name="unique_id"></param>
    /// <returns></returns>
    public static bool ChkFavoritedUnit(long unique_id)
    {
        bool isFavorite = false;
        TemplateList<long> cFavoriteUnitList = LocalSaveManager.Instance.LoadFuncAddFavoriteUnit();

        if (cFavoriteUnitList != null)
        {
            isFavorite = cFavoriteUnitList.ChkInside(TemplateListSort.ChkInsideLong, unique_id);
        }

        return isFavorite;
    }

    public static void openFriendRequestErrorDialog(API_CODE errorCode, System.Action _action = null)
    {
        if (_action != null)
        {
            _action();
        }
    }

    public static bool ChkUnitPartyAssign(long _unique_id)
    {
        PacketStructUnit[][] partyAssign = UserDataAdmin.Instance.m_StructPartyAssign;
        if (partyAssign == null)
        {
            return false;
        }

        for (int i = 0; i < partyAssign.Length; i++)
        {
            for (int j = 0; j < partyAssign[i].Length; j++)
            {
                if (partyAssign[i][j] == null)
                {
                    continue;
                }

                if (partyAssign[i][j].unique_id == _unique_id)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsLoggedIn
    {
        get;
        set;
    }

    public static void setupLoginPack(RecvLoginPackValue cLoginPack)
    {

        //----------------------------------------
        // パックに内包されてる情報を各処理用に割り振り
        //----------------------------------------
        bool bLoginParamRenew = false;
        if (cLoginPack == null)
        {
            Debug.LogError("cLoginPack is null");
            return;
        }

        IsLoggedIn = true;
        //----------------------------------------
        // ログイン系情報保持
        //----------------------------------------

        //----------------------------------------
        // ランチタイム情報を取得
        //----------------------------------------
        MainMenuParam.m_LunchTimeStart = cLoginPack.lunch_st;
        MainMenuParam.m_LunchTimeEnd = cLoginPack.lunch_end;

        //----------------------------------------
        // プレゼント付きガチャのプレゼント付与情報を取得
        //----------------------------------------
        if (UserDataAdmin.HasInstance)
        {
            // login時はサーバーデータに合わせるため、現状のデータを破棄
            UserDataAdmin.Instance.ResetGachaStatus();

            // データがあった場合作成する
            if (cLoginPack.gacha_status != null)
            {
                UserDataAdmin.Instance.UpdateGachaStatusList(cLoginPack.gacha_status);
            }
        }

        //----------------------------------------
        // 通算ログイン回数がセーブデータと一致する場合、
        // 表示を加算してセーブして反映。
        //----------------------------------------
        RecvLoginParamValue cLoginParam = cLoginPack.result_login_param;
        RecvLoginParamValue cLoginParamPrev = LocalSaveManager.Instance.LoadFuncLoginParam();

        //----------------------------------------
        // ログイン情報が以前のものと今回のもの両方存在する場合、
        // 前回取得分を考慮しつつ今回のものと足し合わせて表示する
        //----------------------------------------
        if (cLoginParam != null
        && cLoginParamPrev != null
        )
        {
            if (cLoginParam.login_total != cLoginParamPrev.login_total)
            {
                //----------------------------------------
                // 通算ログイン回数がセーブデータと一致しない場合、
                // 日付が変わってログイン情報が更新されたのでそのまま反映
                //----------------------------------------
                bLoginParamRenew = true;

                //----------------------------------------
                // フレンドポイントが更新されているので反映
                //----------------------------------------
                UserDataAdmin.Instance.m_StructPlayer.have_friend_pt = cLoginParam.friend_point_now;
#if BUILD_TYPE_DEBUG
                Debug.Log("LoginParam Change Day");
#endif
            }
            else
            {
                //----------------------------------------
                // 今回の更新分でパラメータに何らかの変化が生じたならログイン情報として表示する
                //----------------------------------------
                if (cLoginParam.friend_point_get > 0)
                {
                    //----------------------------------------
                    // フレンドポイントが更新されているので反映
                    //----------------------------------------
                    UserDataAdmin.Instance.m_StructPlayer.have_friend_pt = cLoginParam.friend_point_now;
                }
                else
                {
                    cLoginParam.friend_help_ct = 0;
                    cLoginParam.friend_point_get = 0;
                    cLoginParam.friend_point_now = cLoginParamPrev.friend_point_now;
                }

                //----------------------------------------
                // ログインボーナスはその日の初回しか取れない。
                // 初回以外は変な情報が帰ってくるらしいので、例外的に弾く
                //----------------------------------------
                if (cLoginParam.login_bonus != null
                && cLoginParam.login_bonus.Length > 0
                && cLoginParam.login_bonus[0].type != (uint)LOGINBONUS_KIND.LOGINBONUS_KIND_NONE
                )
                {
                    bLoginParamRenew = true;

                    TemplateList<PacketStructLoginBonus> cLoginBonusList = new TemplateList<PacketStructLoginBonus>(cLoginParam.login_bonus, cLoginParamPrev.login_bonus);
                    cLoginParam.login_bonus = cLoginBonusList.ToArray();
#if BUILD_TYPE_DEBUG
                    Debug.Log("LoginBonus Update ... " + LitJson.JsonMapper.ToJson(cLoginParam.login_bonus));
#endif
                }
                else
                {
                    cLoginParam.login_bonus = cLoginParamPrev.login_bonus;
#if BUILD_TYPE_DEBUG
                    Debug.Log("LoginBonus No Change ");
#endif
                }
#if BUILD_TYPE_DEBUG
                Debug.Log("LoginParam Same Day");
#endif
            }

            LocalSaveManager.Instance.SaveFuncLoginParam(cLoginParam);
        }
        else
        if (cLoginParamPrev != null)
        {
            //----------------------------------------
            // セーブデータだけ存在する場合
            // そのままセーブデータを表示に使用する
            //----------------------------------------
            cLoginParam = cLoginParamPrev;

            //----------------------------------------
            // 一度見せた後なはずなんで
            // フレンドポイント部分のみ削除して反映。
            //----------------------------------------
            cLoginParam.friend_help_ct = 0;
            cLoginParam.friend_point_get = 0;
            cLoginParam.friend_point_now = 0;

#if BUILD_TYPE_DEBUG
            Debug.Log("LoginParam Prev Day");
#endif
        }
        else
        if (cLoginParam != null)
        {
            //----------------------------------------
            // 取得データだけ存在する場合、
            // 新しいデータとしてそのままセーブして表示に使用する
            //----------------------------------------
            bLoginParamRenew = true;
            LocalSaveManager.Instance.SaveFuncLoginParam(cLoginParam);
        }

        //----------------------------------------
        // この時点で表示用のログインパラメータが確定。
        //
        // 新規にログインボーナス情報が更新されている場合はログインボーナスダイアログを有効化。
        // 新規にフレンドポイントが発生している場合はフレンドポイントダイアログを有効化。
        //----------------------------------------
        if (cLoginParam != null)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("LoginParam Update ---- \n" + LitJson.JsonMapper.ToJson(cLoginParam));
#endif
            MainMenuParam.m_LoginTotal = cLoginParam.login_total;       // ログイン情報：総合ログイン日数
            MainMenuParam.m_LoginChain = cLoginParam.login_chain;       // ログイン情報：連続ログイン日数
            MainMenuParam.m_LoginBonus = cLoginParam.login_bonus;       // ログイン情報：ログインボーナス
            MainMenuParam.m_LoginFriendPointNow = cLoginParam.friend_point_now; // ログイン情報：フレンド：フレンドポイント：現在値
            MainMenuParam.m_LoginFriendPointGet = cLoginParam.friend_point_get; // ログイン情報：フレンド：フレンドポイント：今回取得分
            MainMenuParam.m_LoginFriendHelpCt = cLoginParam.friend_help_ct;     // ログイン情報：フレンド：助っ人として助けた人数

            //----------------------------------------
            // プレゼントの通知ダイアログを表示するために通知済フラグをオフ
            // ※フレンドポイントの発生はログインボーナスダイアログではなくプレゼントダイアログへ移行した
            //----------------------------------------
            if (MainMenuParam.m_LoginFriendPointGet > 0)
            {
                ResidentParam.m_StartingFirstPresent = false;
            }

            //----------------------------------------
            // 起動後初回かログインパラメータに更新が入った場合のみ
            // ログインボーナスダイアログを有効化
            //----------------------------------------
            if (bLoginParamRenew == true
            || ResidentParam.m_StartingFirstLoginBonus == false
            )
            {
                MainMenuParam.m_LoginActive = true;
            }
            else
            {
                MainMenuParam.m_LoginActive = false;
            }
        }
        else
        {
            MainMenuParam.m_LoginActive = false;
        }

        //----------------------------------------
        // プレゼント系情報保持
        // cLoginPack.result_present 自体が null な場合は
        // 「プレゼント取得処理は重いのでスキップした」
        // 場合を兼ねる。
        // nullならリストを更新しないようにすることで、起動直後に取得した情報を残している
        //----------------------------------------
        if (cLoginPack.result_present != null)
        {
            UserDataAdmin.Instance.m_StructPresentList = UserDataAdmin.PresentListClipTimeLimit(cLoginPack.result_present.present);
        }

        //----------------------------------------
        //
        // 月間ログインボーナス情報保持
        //
        //----------------------------------------
        if (cLoginPack.result_monthly != null)
        {
            UserDataAdmin.Instance.m_StructLoginMonthly = cLoginPack.result_monthly;

            if (UserDataAdmin.Instance.m_StructLoginMonthly == null)
            {
                Debug.LogError("UserDataAdmin.Instance.m_StructLoginMonthly is Null!!!!!!!!!!!!!!!!!!!!!!!!");
            }
        }

        //----------------------------------------
        //
        // 期間限定ログインボーナス情報保持
        //
        //----------------------------------------
        UserDataAdmin.Instance.m_StructPeriodLogin = cLoginPack.result_period_login;
        UserDataAdmin.Instance.m_PeriodLoginResourceRoot = cLoginPack.period_login_resource_root;

        //----------------------------------------
        //
        // その他プレゼント情報保持
        //
        //----------------------------------------
        if (cLoginPack.others_present != null)
        {
            UserDataAdmin.Instance.m_StructOthersPresent = cLoginPack.others_present;
        }

        //----------------------------------------
        // 助っ人情報保持
        //
        // 本来助っ人0人は本来ありえないが、
        // サーバー構築中でダミーユーザーの追加が難しい時期らしいので、
        // 暫定的にローカルでゲームが進行するようにダミーユーザーを捏造して使用できるようにしておく
        //----------------------------------------
        if (cLoginPack.result_helper != null
        && cLoginPack.result_helper.friend != null
        )
        {
            UserDataAdmin.Instance.m_StructHelperList = UserDataAdmin.FriendListClipMe(cLoginPack.result_helper.friend);
        }
        //----------------------------------------
        // フレンド情報保持
        //
        // お気に入りに含まれるフレンドと関連が切れているかチェックして
        // 切れてるならお気に入りから除外する
        //----------------------------------------
        if (cLoginPack.result_friend != null
        && cLoginPack.result_friend.friend != null
        )
        {
            UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(cLoginPack.result_friend.friend);
            LocalSaveManager.Instance.SaveFuncAddFavoriteFriendClip(UserDataAdmin.Instance.m_StructFriendList);
        }

        //----------------------------------------
        // フラグ情報
        //
        //----------------------------------------
        bool flagDaily = false;
        bool flagEvent = false;
        bool flagNormal = false;
        if (cLoginPack.new_notice_flags != null)
        {
            flagDaily = (cLoginPack.new_notice_flags.achievement_daily != 0) ? true : false;
            flagEvent = (cLoginPack.new_notice_flags.achievement_event != 0) ? true : false;
            flagNormal = (cLoginPack.new_notice_flags.achievement_normal != 0) ? true : false;
        }
        UserDataAdmin.Instance.SetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionDaily, flagDaily);
        UserDataAdmin.Instance.SetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionEvent, flagEvent);
        UserDataAdmin.Instance.SetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionNormal, flagNormal);

    }

    /// <summary>
    /// アイテム効果が有効かどうか？
    /// </summary>
    /// <param name="_itemMaster"></param>
    /// <returns></returns>
    static public bool IsItemEffectValid(MasterDataUseItem _itemMaster)
    {
        //効果時間が設定されていない
        if (_itemMaster.effect_span_m == 0)
        {
            return false;
        }

        //アイテムを所持したことがない
        PacketStructUseItem _item = UserDataAdmin.Instance.SearchUseItem(_itemMaster.fix_id);
        if (_item == null)
        {
            return false;
        }
        //アイテムを使用していない
        if (_item.use_timing == 0)
        {
            return false;
        }

        //--------------------------------
        // 時間変換
        //--------------------------------
        //現在時間
        DateTime cTimeNow = TimeManager.Instance.m_TimeNow;
        //アイテム使用開始時間
        DateTime cItemUseStart = TimeUtil.GetDateTimeSec(_item.use_timing);
        //アイテム使用トータル時間
        TimeSpan cItemUseSpan = cTimeNow - cItemUseStart;
        //マスタに設定されている効果時間
        TimeSpan cEffectSpan = TimeSpan.FromMinutes(_itemMaster.effect_span_m);

        //--------------------------------
        // 効果時間チェック
        //--------------------------------
        // 効果時間が切れていた
        if (cItemUseSpan >= cEffectSpan)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// アイテム効果の残り時間を取得
    /// </summary>
    /// <param name="_itemMaster"></param>
    /// <returns></returns>
    static public string GetItemEffectLeftTimeString(MasterDataUseItem _itemMaster)
    {
        string _ret = "00:00";
        if (!IsItemEffectValid(_itemMaster))
        {
            return _ret;
        }

        PacketStructUseItem _item = UserDataAdmin.Instance.SearchUseItem(_itemMaster.fix_id);

        //--------------------------------
        // 時間変換
        //--------------------------------
        //現在時間
        DateTime cTimeNow = TimeManager.Instance.m_TimeNow;
        //アイテム使用開始時間
        DateTime cItemUseStart = TimeUtil.GetDateTimeSec(_item.use_timing);
        //アイテム使用トータル時間
        TimeSpan cItemUseSpan = cTimeNow - cItemUseStart;
        //マスタに設定されている効果時間
        TimeSpan cEffectSpan = TimeSpan.FromMinutes(_itemMaster.effect_span_m);

        TimeSpan leftTime = cEffectSpan - cItemUseSpan;
        _ret = string.Format("{0:00}:{1:00}", leftTime.Minutes, leftTime.Seconds);
        return _ret;
    }

    static public bool IsWriteIcon(ref string spriteName, Sprite sprite)
    {
        if (sprite == null)
        {
            return false;
        }

        return spriteName.Equals(sprite.name) ||
                sprite.name.Equals(UnitIconImageProvider.DefaultIconName) ? true : false;
    }

    static public void GetPresentIcon(MasterDataPresent _master, System.Action<Sprite> callback)
    {
        string spriteName = string.Empty;
        GetPresentIcon(_master, ref spriteName, callback);
    }

    static public void GetPresentIcon(MasterDataPresent _master, ref string spriteName, System.Action<Sprite> callback)
    {
        Sprite result = null;
        string strIconName = MasterDataUtil.DEF_ITEM_ICON_NAME;

        if (_master != null)
        {
            //ユニットの場合、ユニットアイコンを返す
            if (_master.present_type == MasterDataDefineLabel.PresentType.UNIT)
            {
                UnitIconImageProvider.Instance.Get((uint)_master.present_param1, ref spriteName, callback, false);
                return;
            }
            //アイテムの場合
            else if (_master.present_type == MasterDataDefineLabel.PresentType.ITEM)
            {
                string item_name = String.Empty;

                MasterDataUseItem cItemMaster = MasterDataUtil.GetMasterDataUseItemFromID((uint)_master.present_param1);
                if (cItemMaster == null)
                {
                    //デフォルト設定
                    item_name = spriteName = MasterDataUtil.DEF_ITEM_ICON_NAME;
                    callback(ResourceManager.Instance.Load(spriteName, ResourceType.Common));
                }
                else
                {
                    item_name = spriteName = cItemMaster.item_icon;
                    MasterDataUtil.GetItemIcon(cItemMaster, callback);
                }

                return;
            }

            //----------------------------------------
            // アイコンの絵を報酬タイプに合わせて加工
            //----------------------------------------
            ResourceType resourceType = ResourceType.Common;
            switch (_master.present_type)
            {
                case MasterDataDefineLabel.PresentType.NONE:
                    strIconName = MasterDataUtil.DEF_ITEM_ICON_NAME;
                    break;
                case MasterDataDefineLabel.PresentType.MONEY:
                    strIconName = "coin_icon";
                    break;
                case MasterDataDefineLabel.PresentType.FP:
                    strIconName = "friend_point_icon";
                    break;
                case MasterDataDefineLabel.PresentType.STONE:
                    strIconName = "tip_icon";
                    break;
                case MasterDataDefineLabel.PresentType.STONE_PAY:
                    strIconName = "tip_icon";
                    break;
                case MasterDataDefineLabel.PresentType.MSG:
                    strIconName = "divine1";
                    break;
                case MasterDataDefineLabel.PresentType.TICKET:
                    strIconName = "casino_ticket";
                    break;
                case MasterDataDefineLabel.PresentType.PARTY_PRESET:
                    strIconName = "divine1";
                    break;
                case MasterDataDefineLabel.PresentType.SCRATCH:
                    strIconName = "mm_item_scrachcard";
                    break;
                case MasterDataDefineLabel.PresentType.QUEST_KEY:
                    strIconName = "mm_quest_key";
                    resourceType = ResourceType.Menu;
                    break;
                case MasterDataDefineLabel.PresentType.UNIT_POINT:
                    strIconName = "mm_item_unitpoint";
                    break;
                case MasterDataDefineLabel.PresentType.NOTICE:
                    strIconName = "unei_news";
                    break;
            }

            if (strIconName.IsNullOrEmpty() == false)
            {
                if (resourceType == ResourceType.NONE)
                {
                    result = ResourceManager.Instance.Load(strIconName);
                }
                else
                {
                    result = ResourceManager.Instance.Load(strIconName, resourceType);
                }
            }
            else
            {
#if BUILD_TYPE_DEBUG
                Debug.LogError("Not Found IconData::MasterDataPresent::fix_id:" + _master.fix_id);
#endif
            }
        }

        if (result == null)
        {
            strIconName = MasterDataUtil.DEF_ITEM_ICON_NAME;
            result = ResourceManager.Instance.Load(strIconName, ResourceType.Common);
        }

        spriteName = strIconName;

        callback(result);
    }

    /// <summary>
    /// プレゼント個数取得
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    static public uint GetPresentCount(PacketStructPresent data)
    {
        uint count;
        switch ((MasterDataDefineLabel.PresentType)data.present_type)
        {
            case MasterDataDefineLabel.PresentType.MONEY:
            case MasterDataDefineLabel.PresentType.FP:
            case MasterDataDefineLabel.PresentType.STONE:
            case MasterDataDefineLabel.PresentType.STONE_PAY:
            case MasterDataDefineLabel.PresentType.TICKET:
            case MasterDataDefineLabel.PresentType.UNIT_POINT:
                count = data.present_value0;
                break;
            case MasterDataDefineLabel.PresentType.UNIT:
                {
                    // 旧仕様でvalue2に0が設定される場合があるので
                    // １をデフォルトにしてvalue2が２以上だったときに反映する
                    count = 1;
                    if (data.present_value2 > 1)
                    {
                        count = data.present_value2;
                    }
                }
                break;
            case MasterDataDefineLabel.PresentType.ITEM:
            case MasterDataDefineLabel.PresentType.QUEST_KEY:
                count = data.present_value1;
                break;
            case MasterDataDefineLabel.PresentType.SCRATCH:
                count = 1; // 付与数の値がないのでとりあえず1と表示
                break;
            default:
                count = 0;
                break;
        }
        return count;
    }

    static public bool IsRenewQuest(MasterDataAreaCategory _master)
    {
        if (_master.area_cate_type == MasterDataDefineLabel.AreaCategory.RN_STORY
         || _master.area_cate_type == MasterDataDefineLabel.AreaCategory.RN_SCHOOL
         || _master.area_cate_type == MasterDataDefineLabel.AreaCategory.RN_EVENT
         || _master.area_cate_type == MasterDataDefineLabel.AreaCategory.RN_KEY)
        {
            return true;
        }
        return false;
    }

    static public uint GetEventTimingEnd(uint event_id)
    {
        uint unTimingStart = 0;
        uint unTimingEnd = 0;
        MasterDataEvent eventMaster = MasterDataUtil.GetMasterDataEventFromID(event_id);
        if (eventMaster != null)
        {
            switch (eventMaster.period_type)
            {
                // 指定(従来通り)
                default:
                case MasterDataDefineLabel.PeriodType.DESIGNATION:
                    unTimingStart = eventMaster.timing_start;
                    unTimingEnd = eventMaster.timing_end;
                    break;

                // サイクル
                case MasterDataDefineLabel.PeriodType.CYCLE:
                    if (TimeEventManager.Instance != null)
                    {
                        CycleParam cCycleParam = TimeEventManager.Instance.GetEventCycleParam(eventMaster.event_id);
                        if (cCycleParam != null)
                        {
                            unTimingStart = cCycleParam.timingStart;
                            unTimingEnd = cCycleParam.timingEnd;
                        }
                    }
                    break;
            }

        }
        return unTimingEnd;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化時のイベント毎のスキルレベルアップ上昇確率を返す.
		@note
	*/
    //----------------------------------------------------------------------------
    static public float GetBuildUpEventRate()
    {
        switch (MainMenuParam.m_BlendBuildEventSLV)
        {
            case GlobalDefine.SLV_EVENT_ID_x0150:
                return GlobalDefine.BUILDUP_SKILLUP_EVENT_RATE_x0150;
            case GlobalDefine.SLV_EVENT_ID_x0200:
                return GlobalDefine.BUILDUP_SKILLUP_EVENT_RATE_x0200;
            case GlobalDefine.SLV_EVENT_ID_x0250:
                return GlobalDefine.BUILDUP_SKILLUP_EVENT_RATE_x0250;
            case GlobalDefine.SLV_EVENT_ID_x0300:
                return GlobalDefine.BUILDUP_SKILLUP_EVENT_RATE_x0300;
            case GlobalDefine.SLV_EVENT_ID_x0400:
                return GlobalDefine.BUILDUP_SKILLUP_EVENT_RATE_x0400;
            case GlobalDefine.SLV_EVENT_ID_x0500:
                return GlobalDefine.BUILDUP_SKILLUP_EVENT_RATE_x0500;
            case GlobalDefine.SLV_EVENT_ID_x1000:
                return GlobalDefine.BUILDUP_SKILLUP_EVENT_RATE_x1000;
        }
        return 1.0f;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	有料チップ限定ガチャの有効時間判定
		@note
	*/
    //----------------------------------------------------------------------------
    static public bool CheckPaidTipOnlyGacha(MasterDataGacha cGachaMaster)
    {
        //--------------------------------
        //
        //--------------------------------
        if (UserDataAdmin.Instance == null
        || UserDataAdmin.Instance.m_StructPlayer == null)
        {
            return false;
        }
        if (TimeManager.Instance == null)
        {
            return false;
        }

        //--------------------------------
        // 最後にそのガチャを引いた時から敷居時間を超えていなければ引けない
        //--------------------------------
        // 保持データがある場合は、ガチャ引き情報を持ってくる
        PacketStructGachaStatus GachaDate = UserDataAdmin.Instance.GetGachaStatus(cGachaMaster.fix_id);
        // データが無い(=そのガチャを始めて引いた)場合は引ける
        if (GachaDate == null)
        {
            return true;
        }

        DateTime cGachaPlayTime = new DateTime(1970, 1, 1, 0, 0, 0);            // ガチャを引いた時間
        DateTime cGachaPlayTimeNextReset = new DateTime(1970, 1, 1, 0, 0, 0);   // 次回のリセット時間
        int nResetTime = 0;     // リセット時間
        MasterDataDefineLabel.PaidTipResetType nResetType = 0;     // リセットタイプ
        int ResetMonth = 0;     // リセット時間:月
        int ResetDay = 0;       // リセット時間:日
        int ResetHour = 0;      // リセット時間:時
        int ResetMin = 0;       // リセット時間:分
        uint unResetWeek = 0;       // リセットする曜日(bit)

        // リセットタイプを代入
        nResetType = cGachaMaster.reset_type;
        // リセットする曜日(bit)
        unResetWeek = cGachaMaster.reset_week;
        // 有料チップガチャのリセット時間(月)
        ResetMonth = cGachaMaster.reset_month;
        // 有料チップガチャのリセット時間(日)
        ResetDay = cGachaMaster.reset_day;
        // 有料チップガチャのリセット時間(時分)
        nResetTime = cGachaMaster.reset_time;
        // ガチャを引いた時間
        cGachaPlayTime = TimeUtil.ConvertServerTimeToLocalTime(GachaDate.date);

        /*
		// デバッグ用
		// リセットタイプを代入
		nResetType = MasterDataDefineLabel.PAIDTIP_RESET_TYPE_MONTH;
		// リセットする曜日(bit)
		unResetWeek = 128;
		// 有料チップガチャのリセット時間(月)
		ResetMonth = 12;
		// 有料チップガチャのリセット時間(日)
		ResetDay = 1;
		// 有料チップガチャのリセット時間
		nResetTime = 1840;
		// ガチャを引いた時間
		cGachaPlayTime = new DateTime( 2016, 4, 23, 19, 30, 0 );
		*/

        ResetHour = nResetTime / 100;
        ResetMin = (nResetTime - ResetHour * 100);

        switch (nResetType)
        {
            // 月/日/時/分
            case MasterDataDefineLabel.PaidTipResetType.MONTH:

                cGachaPlayTimeNextReset = new DateTime(cGachaPlayTime.Year, ResetMonth, ResetDay, ResetHour, ResetMin, 0);

                // 以前引いた時間が、reset_timeより後なら追加する
                if (cGachaPlayTime >= cGachaPlayTimeNextReset)
                {
                    cGachaPlayTimeNextReset = cGachaPlayTimeNextReset.AddYears(1);
                }
                break;
            // 日/時/分
            case MasterDataDefineLabel.PaidTipResetType.DAY:

                cGachaPlayTimeNextReset = new DateTime(cGachaPlayTime.Year, cGachaPlayTime.Month, ResetDay, ResetHour, ResetMin, 0);

                // 以前引いた時間が、reset_timeより後なら一日追加する
                if (cGachaPlayTime >= cGachaPlayTimeNextReset)
                {
                    cGachaPlayTimeNextReset = cGachaPlayTimeNextReset.AddMonths(1);
                }
                break;
            // 時/分
            case MasterDataDefineLabel.PaidTipResetType.HOUR:

                cGachaPlayTimeNextReset = new DateTime(cGachaPlayTime.Year, cGachaPlayTime.Month, cGachaPlayTime.Day, ResetHour, ResetMin, 0);

                // 以前引いた時間が、reset_timeより後なら一日追加する
                if (cGachaPlayTime >= cGachaPlayTimeNextReset)
                {
                    cGachaPlayTimeNextReset = cGachaPlayTimeNextReset.AddDays(1);
                }
                break;
            // 時/分/曜日
            case MasterDataDefineLabel.PaidTipResetType.WEEK:

                cGachaPlayTimeNextReset = new DateTime(cGachaPlayTime.Year, cGachaPlayTime.Month, cGachaPlayTime.Day, ResetHour, ResetMin, 0);

                //--------------------------------
                // 曜日を取得
                // データ順：月火水木金土日空 →下位bit
                //--------------------------------
                int nBitFlag = 0;
                int nResetWeek = 0;

                for (int bitNum = 0; bitNum < TimeUtil.BIT_DAY_OF_WEEK_MAX; ++bitNum)
                {
                    nBitFlag = 1 << bitNum;

                    //--------------------------------
                    // フラグチェック
                    //--------------------------------
                    if ((unResetWeek & nBitFlag) == 0)
                    {
                        continue;
                    }

                    nResetWeek = bitNum;
                }

                //--------------------------------
                // 曜日チェック
                //--------------------------------

                // ガチャをまわした曜日を取得
                int nPlayWeek = TimeUtil.GetGachaPlayDayOfWeekToBitValue(cGachaPlayTime);

                // 加算する日数
                int nPlusDay = 0;

                // リセットのされる曜日と引いた曜日が同じ場合
                if (nResetWeek == nPlayWeek
                && cGachaPlayTimeNextReset <= cGachaPlayTime)
                {
                    nPlayWeek -= 1;
                    nPlusDay++;
                }

                for (int i = 1; i < TimeUtil.BIT_DAY_OF_WEEK_MAX; i++)
                {
                    // リセットされるの曜日とまわした時の曜日が同じになるまで
                    if (nResetWeek == nPlayWeek)
                    {
                        break;
                    }

                    if (nPlayWeek > 1)
                    {
                        nPlayWeek -= 1;
                    }
                    else
                    {
                        nPlayWeek = TimeUtil.BIT_DAY_OF_WEEK_MAX - 1;
                    }
                    nPlusDay++;
                }

                cGachaPlayTimeNextReset = cGachaPlayTimeNextReset.AddDays(nPlusDay);
                break;
            case MasterDataDefineLabel.PaidTipResetType.NONE:
                return false;
        }

        // 現在時間 - 次の区切り時間を行い、時間が残っている = 現在時間が後なら引ける
        TimeSpan cGachaSpan = TimeManager.Instance.m_TimeNow - cGachaPlayTimeNextReset;
        if (cGachaSpan.TotalMilliseconds > 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// お気に入りチェック
    /// </summary>
    /// <param name="_unique_id"></param>
    /// <returns></returns>
    static public bool ChkUnitFavorite(long _unique_id)
    {
        //--------------------------
        // 選択したユニットをが既にお気に入り登録されているかチェック
        //--------------------------
        TemplateList<long> cFavoriteUnitList = LocalSaveManager.Instance.LoadFuncAddFavoriteUnit();

        bool bAlreadyFavorite = false;
        if (cFavoriteUnitList != null)
        {
            bAlreadyFavorite = cFavoriteUnitList.ChkInside(TemplateListSort.ChkInsideLong, _unique_id);
        }

        return bAlreadyFavorite;
    }

    /// <summary>
    /// リンクユニット属性のアイコンを取得する
    /// </summary>
    /// <param name="_unitData"></param>
    /// <param name="_linkUnitData">nullの場合は自分のユニットリストから、リンクユニットを取得される</param>
    /// <returns></returns>
    public static Sprite GetLinkMark(PacketStructUnit _unitData, PacketStructUnit _linkUnitData = null)
    {
        // リンク状態による分岐
        if (_unitData.link_info == (uint)CHARALINK_TYPE.CHARALINK_TYPE_NONE)
        {
            return null;
        }

        PacketStructUnit linkUnitData = null;
        if (_linkUnitData == null)
        { // 自分のユニットのリンク情報を取得
            linkUnitData = CharaLinkUtil.GetLinkUnit(_unitData.link_unique_id);
        }
        else
        { // フレンドのユニットのリンク情報を取得
            // フレンド、助っ人で一覧に来る場合はベースが来るはず
            linkUnitData = _linkUnitData;
        }

        if (linkUnitData == null)
        {
            return null;
        }

        //スプライト名
        string sprite_name = "link_";

        //属性判定
        MasterDataParamChara master = null;
        master = MasterFinder<MasterDataParamChara>.Instance.Find((int)linkUnitData.id);
        if (master != null)
        {
            switch (master.element)
            {
                case MasterDataDefineLabel.ElementType.NAUGHT:
                    sprite_name += "nothing";
                    break;
                case MasterDataDefineLabel.ElementType.FIRE:
                    sprite_name += "fire";
                    break;
                case MasterDataDefineLabel.ElementType.WATER:
                    sprite_name += "water";
                    break;
                case MasterDataDefineLabel.ElementType.LIGHT:
                    sprite_name += "light";
                    break;
                case MasterDataDefineLabel.ElementType.DARK:
                    sprite_name += "dark";
                    break;
                case MasterDataDefineLabel.ElementType.WIND:
                    sprite_name += "wind";
                    break;
            }
        }

        //親子判定
        if (linkUnitData.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK)
        {
            sprite_name += "_1";
        }
        else
        {
            sprite_name += "_2";
        }

        Sprite sprite = ResourceManager.Instance.Load(sprite_name, ResourceType.Menu);

        return sprite;
    }

    /// <summary>
    /// 属性のテキスト画像を取得する
    /// </summary>
    static public Sprite GetTextElementSprite(MasterDataDefineLabel.ElementType element_type)
    {
        string sprite_name = "";

        switch (element_type)
        {
            case MasterDataDefineLabel.ElementType.NAUGHT:
                sprite_name = "txt_element_naught";
                break;
            case MasterDataDefineLabel.ElementType.FIRE:
                sprite_name = "txt_element_fire";
                break;
            case MasterDataDefineLabel.ElementType.WATER:
                sprite_name = "txt_element_water";
                break;
            case MasterDataDefineLabel.ElementType.LIGHT:
                sprite_name = "txt_element_light";
                break;
            case MasterDataDefineLabel.ElementType.DARK:
                sprite_name = "txt_element_dark";
                break;
            case MasterDataDefineLabel.ElementType.WIND:
                sprite_name = "txt_element_wind";
                break;
        }

        if (sprite_name.IsNullOrEmpty())
        {
            return null;
        }

        return ResourceManager.Instance.Load(sprite_name, ResourceType.Common);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="kind_type"></param>
    /// <param name="is_omission_egg">true:"強化" false:"強化合成用"</param>
    /// <returns></returns>
    static public Sprite GetTextKindSprite(MasterDataDefineLabel.KindType kind_type, bool is_omission_egg)
    {
        string sprite_name = "";

        switch (kind_type)
        {
            case MasterDataDefineLabel.KindType.HUMAN:
                sprite_name = "txt_kind_human";
                break;
            case MasterDataDefineLabel.KindType.DRAGON:
                sprite_name = "txt_kind_dragon";
                break;
            case MasterDataDefineLabel.KindType.GOD:
                sprite_name = "txt_kind_god";
                break;
            case MasterDataDefineLabel.KindType.DEMON:
                sprite_name = "txt_kind_demon";
                break;
            case MasterDataDefineLabel.KindType.CREATURE:
                sprite_name = "txt_kind_creature";
                break;
            case MasterDataDefineLabel.KindType.BEAST:
                sprite_name = "txt_kind_beast";
                break;
            case MasterDataDefineLabel.KindType.MACHINE:
                sprite_name = "txt_kind_machine";
                break;
            case MasterDataDefineLabel.KindType.EGG:
                sprite_name = (is_omission_egg) ? "txt_kind_egg_2" : "txt_kind_egg_1";
                break;
        }

        if (sprite_name.IsNullOrEmpty())
        {
            return null;
        }

        return ResourceManager.Instance.Load(sprite_name, ResourceType.Common);
    }

    private static UnitGridContext CreateUnitContext(UnitGridParam param, uint index, Dictionary<long, bool> party_assign, PacketStructUnit[] party_units = null)
    {
        var model = new ListItemModel(index);
        UnitGridContext unit = new UnitGridContext(model);

        PacketStructUnit unitdata = param.unit;
        unit.UnitData = unitdata;
        unit.CharaMasterData = param.master;
        unit.UnitIconType = MasterDataDefineLabel.UnitIconType.NONE;
        unit.IsSelectedUnit = false;

        unit.setSortParamUnit(param);


        //アイコン構築
        //パーティ
        if (party_units == null)
        {
            //ChkUnitPartyAssign
            unit.IsActivePartyAssign = party_assign.ContainsKey(unitdata.unique_id);
        }
        else
        {
            for (int i = 0; i < party_units.Length; ++i)
            {
                if (party_units[i] == null)
                {
                    continue;
                }

                if (party_units[i].unique_id == unitdata.unique_id)
                {
                    unit.IsActivePartyAssign = true;
                    if (i == 0)
                    {
                        unit.IsActiveLeader = true;
                    }
                }
            }
        }

        //リンク
        if (unitdata.link_info != (int)CHARALINK_TYPE.CHARALINK_TYPE_NONE)
        {
            unit.LinkMark = GetLinkMark(unitdata, null);
        }

        //お気に入り
        unit.IsActiveFavoriteImage = ChkUnitFavorite(unitdata.unique_id);

        //Param
        unit.Plus = unitdata.add_hp + unitdata.add_pow;
        unit.Level = unitdata.level;
        unit.setParamValue();

        return unit;
    }

    private static Dictionary<long, bool> CreatePartyAssign()
    {
        Dictionary<long, bool> dict = new Dictionary<long, bool>();
        PacketStructUnit[][] partyAssign = UserDataAdmin.Instance.m_StructPartyAssign;
        if (partyAssign == null)
        {
            return dict;
        }

        for (int i = 0; i < partyAssign.Length; i++)
        {
            for (int j = 0; j < partyAssign[i].Length; j++)
            {
                if (partyAssign[i][j] == null)
                {
                    continue;
                }

                long unique_id = partyAssign[i][j].unique_id;
                dict[unique_id] = true;
            }
        }

        return dict;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public static List<UnitGridContext> MakeUnitGridContextList(PacketStructUnit[] party_units = null)
    {
        UnitGridParam[] unitlist = UserDataAdmin.Instance.m_UnitGridParamList;
        if (unitlist == null)
        {
            Debug.LogError("unitlist is null");
            return null;
        }

        List<UnitGridContext> list = new List<UnitGridContext>();
        Dictionary<long, bool> party_assign = CreatePartyAssign();

        for (int i = 0; i < unitlist.Length; i++)
        {
            UnitGridContext unitcontext = CreateUnitContext(unitlist[i],
                                                     (uint)i,
                                                     party_assign,
                                                     party_units);
            list.Add(unitcontext);
        }

        return list;
    }

    /// <summary>
	///
	/// </summary>
	/// <returns></returns>
	public static List<UnitGridContext> MakeUnitGridContextListUnitPoint(PacketStructUnit[] party_units = null)
    {
        UnitGridParam[] unitlist = UserDataAdmin.Instance.m_UnitGridParamList;
        if (unitlist == null)
        {
            Debug.LogError("unitlist is null");
            return null;
        }

        List<UnitGridContext> list = new List<UnitGridContext>();
        Dictionary<long, bool> party_assign = CreatePartyAssign();

        for (int i = 0; i < unitlist.Length; i++)
        {
            UnitGridParam unit = unitlist[i];
            if (unit.master == null)
            {
                continue;
            }
            //--------------------------------
            // 進化マスター取得
            //--------------------------------
            MasterDataParamCharaEvol cEvol = MasterDataUtil.GetCharaEvolParamFromCharaID(unit.master.fix_id);
            // 進化先がないなら表示しない
            if (cEvol == null)
            {
                continue;
            }

            // 指定ポイントが0以下の場合
            if (unit.master.evol_unitpoint <= 0)
            {
                continue;
            }

            UnitGridContext unitcontext = CreateUnitContext(unitlist[i],
                                                     (uint)i,
                                                     party_assign,
                                                     party_units);
            list.Add(unitcontext);
        }

        return list;
    }

    /// <summary>
	///
	/// </summary>
	/// <returns></returns>
	public static List<UnitGridContext> MakeUnitGridContextListLimitOver(PacketStructUnit[] party_units = null)
    {
        UnitGridParam[] unitlist = UserDataAdmin.Instance.m_UnitGridParamList;
        if (unitlist == null)
        {
            Debug.LogError("unitlist is null");
            return null;
        }

        List<UnitGridContext> list = new List<UnitGridContext>();
        Dictionary<long, bool> party_assign = CreatePartyAssign();

        for (int i = 0; i < unitlist.Length; i++)
        {
            UnitGridParam unit = unitlist[i];

            if (unit.master == null)
            {
                continue;
            }

            //--------------------------------
            // 限界突破マスターも取得
            //--------------------------------
            MasterDataLimitOver cLimitOver = MasterDataUtil.GetLimitOverFromID(unit.master.limit_over_type);
            // 限界突破マスターが無いならとりあえず表示しない
            if (cLimitOver == null)
            {
                continue;
            }

            //--------------------------------
            // 条件を満たすユニットはリストに表示しない
            //--------------------------------
            // 限界突破マスターの最大レベルが0以下の場合
            if (cLimitOver.limit_over_max <= 0)
            {
                continue;
            }

            // 最大まで限界突破している場合
            if (unit.limitover_lv >= cLimitOver.limit_over_max)
            {
                continue;
            }

            UnitGridContext unitcontext = CreateUnitContext(unitlist[i],
                                                     (uint)i,
                                                     party_assign,
                                                     party_units);
            list.Add(unitcontext);
        }

        return list;
    }

    public static Sprite GetSkillElementIcon(MasterDataDefineLabel.ElementType _type)
    {
        string _sprite_name = "";
        switch (_type)
        {
            case MasterDataDefineLabel.ElementType.NAUGHT:
                _sprite_name = "_k";
                break;
            case MasterDataDefineLabel.ElementType.FIRE:
                _sprite_name = "_r";
                break;
            case MasterDataDefineLabel.ElementType.WATER:
                _sprite_name = "_b";
                break;
            case MasterDataDefineLabel.ElementType.LIGHT:
                _sprite_name = "_y";
                break;
            case MasterDataDefineLabel.ElementType.DARK:
                _sprite_name = "_v";
                break;
            case MasterDataDefineLabel.ElementType.WIND:
                _sprite_name = "_g";
                break;
            case MasterDataDefineLabel.ElementType.HEAL:
                _sprite_name = "_p";
                break;
        }

        if (_sprite_name.IsNullOrEmpty())
        {
            return null;
        }

        return ResourceManager.Instance.Load("icon_zokusei" + _sprite_name, ResourceType.Common);
    }

    public static Sprite GetSkillElementColor(string sno, MasterDataDefineLabel.ElementType _type)
    {
        string _sprite_name = "_n";
        switch (_type)
        {
            case MasterDataDefineLabel.ElementType.NAUGHT:
                _sprite_name = "_k";
                break;
            case MasterDataDefineLabel.ElementType.FIRE:
                _sprite_name = "_r";
                break;
            case MasterDataDefineLabel.ElementType.WATER:
                _sprite_name = "_b";
                break;
            case MasterDataDefineLabel.ElementType.LIGHT:
                _sprite_name = "_y";
                break;
            case MasterDataDefineLabel.ElementType.DARK:
                _sprite_name = "_v";
                break;
            case MasterDataDefineLabel.ElementType.WIND:
                _sprite_name = "_g";
                break;
            case MasterDataDefineLabel.ElementType.HEAL:
                _sprite_name = "_p";
                break;
        }

        return ResourceManager.Instance.Load(sno + _sprite_name, ResourceType.Common);
    }

    public static void SetPartySelectUnitData(ref PartyMemberUnitContext unit, PacketStructUnit unitData, PacketStructUnit subUnit, PacketStructUnit[] partyUnits)
    {
        if (unit == null)
        {
            return;
        }
        if (unitData == null || unitData.id == 0)
        {
            return;
        }

        if (unit.CharaMaster == null)
        {
            return;
        }

        if (unit.CharaMaster.skill_active0 > 0)
        {
            MasterDataSkillActive skill1 = MasterFinder<MasterDataSkillActive>.Instance.Find((int)unit.CharaMaster.skill_active0);
            if (skill1 != null)
            {
                unit.IsActiveSkill1Empty = false;
                unit.Skill1Cost1 = MainMenuUtil.GetSkillElementIcon(skill1.cost1);
                unit.Skill1Cost2 = MainMenuUtil.GetSkillElementIcon(skill1.cost2);
                unit.Skill1Cost3 = MainMenuUtil.GetSkillElementIcon(skill1.cost3);
                unit.Skill1Cost4 = MainMenuUtil.GetSkillElementIcon(skill1.cost4);
                unit.Skill1Cost5 = MainMenuUtil.GetSkillElementIcon(skill1.cost5);
                unit.Skill1Color = MainMenuUtil.GetSkillElementColor("S1", skill1.skill_element);
            }
        }

        if (unit.CharaMaster.skill_active1 > 0)
        {
            MasterDataSkillActive skill2 = MasterFinder<MasterDataSkillActive>.Instance.Find((int)unit.CharaMaster.skill_active1);
            if (skill2 != null)
            {
                unit.IsActiveSkill2Empty = false;
                unit.Skill2Cost1 = MainMenuUtil.GetSkillElementIcon(skill2.cost1);
                unit.Skill2Cost2 = MainMenuUtil.GetSkillElementIcon(skill2.cost2);
                unit.Skill2Cost3 = MainMenuUtil.GetSkillElementIcon(skill2.cost3);
                unit.Skill2Cost4 = MainMenuUtil.GetSkillElementIcon(skill2.cost4);
                unit.Skill2Cost5 = MainMenuUtil.GetSkillElementIcon(skill2.cost5);
                unit.Skill2Color = MainMenuUtil.GetSkillElementColor("S2", skill2.skill_element);
            }
        }


        unit.ParamText = (unitData.level >= unit.CharaMaster.level_max) ? GameTextUtil.GetText("uniticon_flag1")
                                    : string.Format(GameTextUtil.GetText("uniticon_flag2"), unitData.level); // レベル
        uint plusPoint = unitData.add_hp + unitData.add_pow; // プラス値の計算
        if (plusPoint != 0)
        {
            unit.ParamText += string.Format(GameTextUtil.GetText("uniticon_flag3"), plusPoint);
        }

        unit.LinkIcon = MainMenuUtil.GetLinkMark(unitData, subUnit);
        unit.OutSideCircleImage = MainMenuUtil.GetElementCircleSprite(unit.CharaMaster.element);
        if (unit.LinkCharaMaster != null)
        {
            unit.LinkOutSideCircleImage = MainMenuUtil.GetElementCircleSprite(unit.LinkCharaMaster.element);
        }
        SetUpPartySelectCharaData(ref unit, unitData, subUnit, false, partyUnits);
    }

    private static void SetUpPartySelectCharaData(ref PartyMemberUnitContext item, PacketStructUnit _mainUnit, PacketStructUnit _subUnit, bool dispCharm, PacketStructUnit[] partyUnits)
    {
        CharaOnce baseChara = new CharaOnce();

        if (_mainUnit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE &&
            _subUnit != null)
        {
            baseChara.CharaSetupFromID(
                _mainUnit.id,
                (int)_mainUnit.level,
                (int)_mainUnit.limitbreak_lv,
                (int)_mainUnit.limitover_lv,
                (int)_mainUnit.add_pow,
                (int)_mainUnit.add_hp,
                _subUnit.id,
                (int)_subUnit.level,
                (int)_subUnit.add_pow,
                (int)_subUnit.add_hp,
                (int)_mainUnit.link_point,
                (int)_subUnit.limitover_lv
                );
        }
        else
        {
            baseChara.CharaSetupFromID(
                _mainUnit.id,
                (int)_mainUnit.level,
                (int)_mainUnit.limitbreak_lv,
                (int)_mainUnit.limitover_lv,
                (int)_mainUnit.add_pow,
                (int)_mainUnit.add_hp,
                0,
                0,
                0,
                0,
                0,
                0
                );
        }

        var changeHp = (int)(baseChara.m_CharaHP * GetLeaderSkillHpRate(baseChara, partyUnits));
        var statusText = string.Format(GameTextUtil.GetText("questlast_text4"), changeHp);
        if (changeHp > baseChara.m_CharaHP)
        {
            statusText = string.Format(GameTextUtil.GetText("questlast_text8"), changeHp);
        }
        else if (changeHp < baseChara.m_CharaHP)
        {
            statusText = string.Format(GameTextUtil.GetText("questlast_text9"), changeHp);
        }
        item.HpText = statusText;

        var changePow = (int)(baseChara.m_CharaPow * GetLeaderSkillDamageRate(baseChara, GetPartyCharaOnceArray(partyUnits)));
        statusText = string.Format(GameTextUtil.GetText("questlast_text4"), changePow);
        if (changePow > baseChara.m_CharaPow)
        {
            statusText = string.Format(GameTextUtil.GetText("questlast_text8"), changePow);
        }
        else if (changePow < baseChara.m_CharaPow)
        {
            statusText = string.Format(GameTextUtil.GetText("questlast_text9"), changePow);
        }
        item.AtkText = statusText;
    }

    /// <summary>
    /// PacketStructUnit配列からCharaOnce配列を作成して返す.
    /// </summary>
    /// <param name="partyUnits">現在のパーティーのユニット情報</param>
    /// <returns>パーティーのキャラステータス配列</returns>
    public static CharaOnce[] GetPartyCharaOnceArray(PacketStructUnit[] partyUnits)
    {
        var charaArray = new CharaOnce[(int)GlobalDefine.PartyCharaIndex.MAX];
        for (var i = 0; i < partyUnits.Length; i++)
        {
            if (partyUnits[i] == null)
            {
                continue;
            }
            charaArray[i] = GetPartyCharaOnce(partyUnits[i]);
        }

        return charaArray;
    }

    /// <summary>
    /// PacketStructUnitからCharaOnceを作成して返す.
    /// </summary>
    /// <param name="packetStructUnit">キャラ情報</param>
    /// <returns>キャラステータス情報</returns>
    private static CharaOnce GetPartyCharaOnce(PacketStructUnit packetStructUnit)
    {
        var charaOnce = new CharaOnce();
        var unit = packetStructUnit;
        if (unit != null)
        {
            charaOnce.CharaSetupFromID(
                unit.id,
                (int)unit.level,
                (int)unit.limitbreak_lv,
                (int)unit.limitover_lv,
                (int)unit.add_pow,
                (int)unit.add_hp,
                0,
                0,
                0,
                0,
                0,
                0
                );
        }

        return charaOnce;
    }

    /// <summary>
    /// リーダースキルのHP倍率を返す.
    /// </summary>
    /// <param name="baseChara">対象キャラ</param>
    /// <param name="partyUnits"></param>
    /// <returns></returns>
    public static float GetLeaderSkillHpRate(CharaOnce baseChara, PacketStructUnit[] partyUnits)
    {
        var rate = 1.0f;
        var partyCharaOnce = GetPartyCharaOnceArray(partyUnits);
        // リーダースキルのHP倍率を反映.
        rate = InGameUtilBattle.AvoidErrorMultiple(rate, InGameUtilBattle.GetLeaderSkillHPRate(partyCharaOnce, GlobalDefine.PartyCharaIndex.LEADER, baseChara));
        // フレンドスキルのHP倍率を反映.
        rate = InGameUtilBattle.AvoidErrorMultiple(rate, InGameUtilBattle.GetLeaderSkillHPRate(partyCharaOnce, GlobalDefine.PartyCharaIndex.FRIEND, baseChara));

        return rate;
    }

    /// <summary>
    /// リーダースキルのダメージ倍率を返す.
    /// </summary>
    /// <param name="baseChara"></param>
    /// <param name="partyCharaOnce"></param>
    /// <returns></returns>
    public static float GetLeaderSkillDamageRate(CharaOnce baseChara, CharaOnce[] partyCharaOnce)
    {
        var rate = 1.0f;
        var party = new CharaParty();
        party.PartySetup(partyCharaOnce, false);
        // ダメージ計算時にBattleParam.m_PlayerPartyを使用するため一時的に代入する.
        BattleParam.m_PlayerParty = party;

        rate = InGameUtilBattle.GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex.LEADER, baseChara);
        rate = InGameUtilBattle.AvoidErrorMultiple(rate, InGameUtilBattle.GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex.FRIEND, baseChara));

        // ダメージ計算が完了したので,BattleParam.m_PlayerPartyにNULLを入れておく.
        BattleParam.m_PlayerParty = null;

        return rate;
    }

    /// <summary>
    /// バフイベント有無チェック.
    /// </summary>
    /// <param name="category">チェックするエリアカテゴリタイプ(NONEなら全部)/param>
    /// <param name="areaCategoryId">チェックするカテゴリID(0なら全部)</param>
    /// <returns>バフイベント有無(true:有/false:無)</returns>
    public static bool checkHelpBufEvent(MasterDataDefineLabel.AreaCategory category = MasterDataDefineLabel.AreaCategory.NONE)
    {
        if (TutorialManager.IsExists == true)
        {
            return false;
        }

        //保存された結果がある場合はそれを使用する。
        if (IsAmendFlagCheckResult(category))
        {
            return GetAmendFlagCheckResult(category);
        }

        bool bCheckChallenge = false;

        MasterDataDefineLabel.REGION_CATEGORY[] region_category;
        switch (category)
        {
            case MasterDataDefineLabel.AreaCategory.RN_STORY:
                region_category = new MasterDataDefineLabel.REGION_CATEGORY[] { MasterDataDefineLabel.REGION_CATEGORY.STORY };
                break;

            case MasterDataDefineLabel.AreaCategory.RN_SCHOOL:
                region_category = new MasterDataDefineLabel.REGION_CATEGORY[] { MasterDataDefineLabel.REGION_CATEGORY.MATERIAL };
                break;

            case MasterDataDefineLabel.AreaCategory.RN_EVENT:
                region_category = new MasterDataDefineLabel.REGION_CATEGORY[] { MasterDataDefineLabel.REGION_CATEGORY.EVENT };
                bCheckChallenge = true;
                break;

            default:
                region_category = new MasterDataDefineLabel.REGION_CATEGORY[] { MasterDataDefineLabel.REGION_CATEGORY.STORY, MasterDataDefineLabel.REGION_CATEGORY.MATERIAL, MasterDataDefineLabel.REGION_CATEGORY.EVENT };
                bCheckChallenge = true;
                break;
        }

        for (int i = 0; i < region_category.Length; ++i)
        {
            uint region_id = MasterDataUtil.GetRegionIDFromCategory(region_category[i]);
            MasterDataRegion regionMaster = MasterFinder<MasterDataRegion>.Instance.Find((int)region_id);

            if (regionMaster == null)
            {
                continue;
            }

            MasterDataAreaCategory[] areaCategoryMasterArray = MasterFinder<MasterDataAreaCategory>.Instance.SelectWhere("where region_id = ?", region_id).ToArray();
            if (areaCategoryMasterArray == null)
            {
                continue;
            }

            for (int j = 0; j < areaCategoryMasterArray.Length; ++j)
            {
                if (isCheckAmendFlag(areaCategoryMasterArray[j]))
                {
                    //リザルトデータを保存する。
                    SaveAmendFlagCheckResult(category, true);
                    return true;
                }
            }
        }

        //成長ボスのバフイベントチェック
        if (bCheckChallenge)
        {
            if (IsCheckChallengeAmend())
            {
                //リザルトデータを保存する。
                SaveAmendFlagCheckResult(category, true);
                return true;
            }
        }

        //リザルトデータを保存する。
        SaveAmendFlagCheckResult(category, false);
        return false;
    }

    private static bool isCheckAmendFlag(MasterDataAreaCategory areaCategoryMaster)
    {
        if (areaCategoryMaster == null)
        {
            return false;
        }
        bool hasArea = false;
        bool hasQuestCleard = true;
        bool hasQuestCompleted = true;
        bool hasQuestNew = false;
        MasterDataGuerrillaBoss guerrillaBoss = null;

        MasterDataArea[] masterAreaArray = MasterFinder<MasterDataArea>.Instance.SelectWhere(" where area_cate_id = ? ", areaCategoryMaster.fix_id).ToArray();

        for (int i = 0; i < masterAreaArray.Length; ++i)
        {
            ChkActiveArea(areaCategoryMaster, masterAreaArray[i], ref hasArea, ref hasQuestCompleted, ref hasQuestCleard, ref hasQuestNew, ref guerrillaBoss);
            if (!hasArea)
            {
                continue;
            }

            MainMenuQuestSelect.AreaAmendParam amend = CreateAreaParamAmend(masterAreaArray[i]);
            if (amend == null)
            {
                continue;

            }

            if (amend.m_FlagAmendCoin == true ||
                amend.m_FlagAmendDrop == true ||
                amend.m_FlagAmendEXP == true ||
                amend.m_FlagAmendGuerrillaBoss == true ||
                amend.m_FlagAmendLinkPoint == true ||
                amend.m_FlagAmendScore == true ||
                amend.m_FlagAmendStamina == true ||
                amend.m_FlagAmendTicket == true)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 成長ボスのバフイベントチェック
    /// </summary>
    /// <returns></returns>
    public static bool IsCheckChallengeAmend()
    {
        List<MasterDataChallengeEvent> events = MasterDataUtil.GetActiveChallengeEvent();
        for (int i = 0; i < events.Count; i++)
        {
            MasterDataArea areaData = MasterFinder<MasterDataArea>.Instance.SelectFirstWhere("where event_id = ?", events[i].event_id);
            if (areaData == null)
            {
                continue;
            }
            MainMenuQuestSelect.AreaAmendParam amend = CreateAreaParamAmend(areaData);
            if (amend == null)
            {
                continue;

            }

            if (amend.m_FlagAmendCoin == true ||
                amend.m_FlagAmendDrop == true ||
                amend.m_FlagAmendEXP == true ||
                amend.m_FlagAmendGuerrillaBoss == true ||
                amend.m_FlagAmendLinkPoint == true ||
                amend.m_FlagAmendScore == true ||
                amend.m_FlagAmendStamina == true ||
                amend.m_FlagAmendTicket == true)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// エリア補間フラグチェック結果があるかどうか？
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    static private bool IsAmendFlagCheckResult(MasterDataDefineLabel.AreaCategory category)
    {
        int index = GetAmendFlagCheckIndex(category);

        //現在の時間
        int NowHour = TimeManager.Instance.m_TimeNow.Hour;

        //保存データが有効かどうか？
        if (MainMenuParam.m_AmendFlagCheckTime[index] == NowHour)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// エリア補間フラグインデックス取得
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    static private int GetAmendFlagCheckIndex(MasterDataDefineLabel.AreaCategory category)
    {
        int index = 0;
        switch (category)
        {
            case MasterDataDefineLabel.AreaCategory.RN_STORY:
                index = 1;
                break;
            case MasterDataDefineLabel.AreaCategory.RN_SCHOOL:
                index = 2;
                break;
            case MasterDataDefineLabel.AreaCategory.RN_EVENT:
                index = 3;
                break;
            default:
                break;
        }
        return index;
    }

    /// <summary>
    /// エリア補間チェックリザルト取得
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    static private bool GetAmendFlagCheckResult(MasterDataDefineLabel.AreaCategory category)
    {
        int index = GetAmendFlagCheckIndex(category);
        return MainMenuParam.m_AmendFlagCheckResult[index];
    }

    /// <summary>
    /// エリア補間フラグチェック結果保存
    /// </summary>
    /// <param name="category"></param>
    /// <param name="result"></param>
    static private void SaveAmendFlagCheckResult(MasterDataDefineLabel.AreaCategory category, bool result)
    {
        int index = GetAmendFlagCheckIndex(category);
        int NowHour = TimeManager.Instance.m_TimeNow.Hour;
        MainMenuParam.m_AmendFlagCheckTime[index] = NowHour;
        MainMenuParam.m_AmendFlagCheckResult[index] = result;
    }

    /// <summary>
    /// リージョンカテゴリ取得
    /// </summary>
    /// <param name="area_cate_id"></param>
    /// <returns></returns>
    static public MasterDataDefineLabel.REGION_CATEGORY GetRegionCategory(uint area_cate_id)
    {
        MasterDataDefineLabel.REGION_CATEGORY ret = MasterDataDefineLabel.REGION_CATEGORY.STORY;

        if (area_cate_id == 0)
        {
            return ret;
        }

        //
        MasterDataAreaCategory areaCategory = MasterFinder<MasterDataAreaCategory>.Instance.Find((int)area_cate_id);
        if (areaCategory == null)
        {
            return ret;
        }

        MasterDataRegion region = MasterFinder<MasterDataRegion>.Instance.Find((int)areaCategory.region_id);
        if (region == null)
        {
            return ret;
        }

        return region.category;
    }

    /// <summary>
    /// エリアカテゴリの存在チェック
    /// </summary>
    /// <param name="area_cate_id"></param>
    /// <returns></returns>
    static public bool CheckRNAreaCategory(uint area_cate_id)
    {
        //
        if (area_cate_id == 0)
        {
            return false;
        }

        //
        MasterDataAreaCategory areaCategory = MasterFinder<MasterDataAreaCategory>.Instance.Find((int)area_cate_id);
        if (areaCategory == null)
        {
            return false;
        }

        MasterDataArea[] areaArray = MasterFinder<MasterDataArea>.Instance.SelectWhere(" where area_cate_id = ? ", area_cate_id).ToArray();

        //----------------------------------------------
        // エリアの存在チェック
        //----------------------------------------------
        bool hasArea = false;
        bool hasQuestCompleted = true;
        bool hasQuestCleard = true;
        bool hasQuestNew = false;
        MasterDataGuerrillaBoss guerrillaBoss = null;

        for (int chkArea = 0; chkArea < areaArray.Length; ++chkArea)
        {
            MasterDataArea areaData = areaArray[chkArea];
            if (areaData == null)
            {
                continue;
            }


            ChkActiveArea(areaCategory, areaData, ref hasArea, ref hasQuestCompleted, ref hasQuestCleard, ref hasQuestNew, ref guerrillaBoss);

            //有効なエリアが見つかった。
            if (hasArea) return true;
        }

        return false;
    }

    /// <summary>
    /// パラメータが上限値に達している場合はダイアログ出して注意喚起。処理は続行
    /// </summary>
    /// <param name="error_type"></param>
    /// <param name="dialogType"></param>
    /// <param name="finishAction"></param>
    static public void ShowParamLimitDialog(PRM_LIMIT_ERROR_TYPE error_type, DialogType dialogType, Action<bool> finishAction = null)
    {
        if (error_type == PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_CHECK_OK)
        {
            if (finishAction != null)
            {
                finishAction(true);
            }
        }
        else
        {
            string strTitle = "";
            string strMsg = "";

            switch (error_type)
            {
                case PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_COIN:
                    //コイン
                    strTitle = "ERROR_QUEST_LIMIT_COIN_TITLE";
                    strMsg = "ERROR_QUEST_AF_LIMIT_COIN";
                    break;

                case PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_TICKET:
                    //チケット
                    strTitle = "ERROR_QUEST_LIMIT_TICKET_TITLE";
                    strMsg = "ERROR_QUEST_AF_LIMIT_TICKET";
                    break;

                case PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_OTHER:
                    //複数エラー
                    strTitle = "ERROR_QUEST_LIMIT_OTHER_TITLE";
                    strMsg = "ERROR_QUEST_AF_LIMIT_OTHER";
                    break;

                default:
                    //上記以外でエラーが帰ってきたら一応エラーをだしておく（基本ここは通らない）
                    strTitle = "ERROR_QUEST_LIMIT_OTHER_TITLE";
                    strMsg = "ERROR_QUEST_AF_LIMIT_OTHER";
                    break;
            }

            Dialog _newDialog = Dialog.Create(dialogType);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, strTitle);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, strMsg);

            switch (dialogType)
            {
                case DialogType.DialogOK:
                    _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button7");
                    _newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
                    {
                        if (finishAction != null)
                        {
                            finishAction(true);
                        }
                    });
                    break;
                case DialogType.DialogYesNo:
                    _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button7");
                    _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button6");
                    _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
                    {
                        if (finishAction != null)
                        {
                            finishAction(true);
                        }
                    });
                    _newDialog.SetDialogEvent(DialogButtonEventType.NO, () =>
                    {
                        if (finishAction != null)
                        {
                            finishAction(false);
                        }
                    });
                    break;
            }

            _newDialog.EnableFadePanel();
            _newDialog.Show();
            SoundUtil.PlaySE(SEID.SE_MENU_OK);
        }
    }
}
