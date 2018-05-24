/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	GameTextUtil.cs
	@brief	テキストユーティリティ
	@author Developer
	@date 	2013/05/27
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
	@brief	テキストユーティリティ
*/
//----------------------------------------------------------------------------
static public class GameTextUtil
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	キーに割り当てられたテキストを返す
	*/
    //----------------------------------------------------------------------------
    static public string GetText(string strKey)
    {
        return UnityUtil.GetText(strKey);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キーに割り当てられたテキストを返す
	*/
    //----------------------------------------------------------------------------
    static public bool GetTextTry(string strKey, ref string strGetMsg)
    {
        return UnityUtil.GetTextTry(strKey, ref strGetMsg);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief			種族ID->種族ゲームテキストキー
		@param[in]		int		(kind)		種族ID
		@return			string	[対応したゲームテキストキー]
	*/
    //----------------------------------------------------------------------------
    static public string GetKindToTextKey(MasterDataDefineLabel.KindType kind)
    {
        string strRaceMsgKey = "KIND_HUMAN";
        switch (kind)
        {
            case MasterDataDefineLabel.KindType.HUMAN: strRaceMsgKey = "KIND_HUMAN"; break;
            case MasterDataDefineLabel.KindType.DRAGON: strRaceMsgKey = "KIND_DRAGON"; break;
            case MasterDataDefineLabel.KindType.GOD: strRaceMsgKey = "KIND_GOD"; break;
            case MasterDataDefineLabel.KindType.DEMON: strRaceMsgKey = "KIND_DEMON"; break;
            case MasterDataDefineLabel.KindType.CREATURE: strRaceMsgKey = "KIND_CREATURE"; break;
            case MasterDataDefineLabel.KindType.BEAST: strRaceMsgKey = "KIND_BEAST"; break;
            case MasterDataDefineLabel.KindType.MACHINE: strRaceMsgKey = "KIND_MACHINE"; break;
            case MasterDataDefineLabel.KindType.EGG: strRaceMsgKey = "KIND_EGG"; break;
            default: Debug.LogError("Unit Kind Error!: kind:" + kind); break;
        }
        return strRaceMsgKey;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief			種族ID->種族ゲームテキスト->表示種族テキスト
		@param[in]		int		(kind)		種族ID
		@param[in]		int		(kind)		副種族ID
		@return			string	[対応したゲームテキスト]
	*/
    //----------------------------------------------------------------------------
    static public string GetKindToText(MasterDataDefineLabel.KindType kind, MasterDataDefineLabel.KindType sub_kind)
    {
        //----------------------------------------
        // キャラ種族入力
        //----------------------------------------
        string strRaceMsgKey = "KIND_HUMAN";
        switch (kind)
        {
            case MasterDataDefineLabel.KindType.HUMAN: strRaceMsgKey = "KIND_HUMAN"; break;
            case MasterDataDefineLabel.KindType.DRAGON: strRaceMsgKey = "KIND_DRAGON"; break;
            case MasterDataDefineLabel.KindType.GOD: strRaceMsgKey = "KIND_GOD"; break;
            case MasterDataDefineLabel.KindType.DEMON: strRaceMsgKey = "KIND_DEMON"; break;
            case MasterDataDefineLabel.KindType.CREATURE: strRaceMsgKey = "KIND_CREATURE"; break;
            case MasterDataDefineLabel.KindType.BEAST: strRaceMsgKey = "KIND_BEAST"; break;
            case MasterDataDefineLabel.KindType.MACHINE: strRaceMsgKey = "KIND_MACHINE"; break;
            case MasterDataDefineLabel.KindType.EGG: strRaceMsgKey = "KIND_EGG"; break;
            default: Debug.LogError("Unit Kind Error!: kind:" + kind); break;
        }
        //----------------------------------------
        // キャラ副種族入力
        //----------------------------------------
        string strSubRaceMsgKey = "";
        switch (sub_kind)
        {
            case MasterDataDefineLabel.KindType.HUMAN: strSubRaceMsgKey = "KIND_HUMAN"; break;
            case MasterDataDefineLabel.KindType.DRAGON: strSubRaceMsgKey = "KIND_DRAGON"; break;
            case MasterDataDefineLabel.KindType.GOD: strSubRaceMsgKey = "KIND_GOD"; break;
            case MasterDataDefineLabel.KindType.DEMON: strSubRaceMsgKey = "KIND_DEMON"; break;
            case MasterDataDefineLabel.KindType.CREATURE: strSubRaceMsgKey = "KIND_CREATURE"; break;
            case MasterDataDefineLabel.KindType.BEAST: strSubRaceMsgKey = "KIND_BEAST"; break;
            case MasterDataDefineLabel.KindType.MACHINE: strSubRaceMsgKey = "KIND_MACHINE"; break;
            case MasterDataDefineLabel.KindType.EGG: strSubRaceMsgKey = "KIND_EGG"; break;
        }

        if (strSubRaceMsgKey == "")
        {
            // 副種族が存在しないキャラであればベース種族のみ返す
            return UnityUtil.GetText(strRaceMsgKey);
        }
        else
        {
            // 副種族の存在するキャラならば副種族を後ろにつけて返す
            return UnityUtil.GetText(strRaceMsgKey) + " / " + UnityUtil.GetText(strSubRaceMsgKey);
        }
    }
    //----------------------------------------------------------------------------
    /*!
		@brief			レア度->レア度ゲームテキストキー
		@param[in]		int		(rare)		レア度
		@return			string	[対応したゲームテキストキー]
	*/
    //----------------------------------------------------------------------------
    static public string GetRareToTextKey(MasterDataDefineLabel.RarityType rare)
    {
        string strRareMsgKey = "RR_01";
        switch (rare)
        {
            case MasterDataDefineLabel.RarityType.STAR_1: strRareMsgKey = "RR_01"; break;
            case MasterDataDefineLabel.RarityType.STAR_2: strRareMsgKey = "RR_02"; break;
            case MasterDataDefineLabel.RarityType.STAR_3: strRareMsgKey = "RR_03"; break;
            case MasterDataDefineLabel.RarityType.STAR_4: strRareMsgKey = "RR_04"; break;
            case MasterDataDefineLabel.RarityType.STAR_5: strRareMsgKey = "RR_05"; break;
            case MasterDataDefineLabel.RarityType.STAR_6: strRareMsgKey = "RR_06"; break;
            case MasterDataDefineLabel.RarityType.STAR_7: strRareMsgKey = "RR_07"; break;
            default: Debug.LogError("Unit Kind Error!: rare:" + rare); break;
        }
        return strRareMsgKey;
    }

    //----------------------------------------------------------------------------
    /*!
			@brief			属性->属性テキストキー
		@param[in]		int		(elem)		属性
		@return			string	[対応したゲームテキストキー]
	*/
    //----------------------------------------------------------------------------
    static public string GetElemToTextKey(MasterDataDefineLabel.ElementType elem)
    {
        string strElementMsgKey = "ELEM_NAUGHT";
        switch (elem)
        {
            case MasterDataDefineLabel.ElementType.NONE: strElementMsgKey = "ELEM_NONE"; break;
            case MasterDataDefineLabel.ElementType.FIRE: strElementMsgKey = "ELEM_FIRE"; break;
            case MasterDataDefineLabel.ElementType.WATER: strElementMsgKey = "ELEM_WATER"; break;
            case MasterDataDefineLabel.ElementType.WIND: strElementMsgKey = "ELEM_WIND"; break;
            case MasterDataDefineLabel.ElementType.DARK: strElementMsgKey = "ELEM_DARK"; break;
            case MasterDataDefineLabel.ElementType.LIGHT: strElementMsgKey = "ELEM_LIGHT"; break;
            case MasterDataDefineLabel.ElementType.NAUGHT: strElementMsgKey = "ELEM_NAUGHT"; break;
            case MasterDataDefineLabel.ElementType.HEAL: strElementMsgKey = "ELEM_HEAL"; break;
            default: Debug.LogError("Unit Kind Error!: elem:" + elem); break;
        }
        return strElementMsgKey;
    }

    /// <summary>
    /// ソート項目のテキストを取得
    /// </summary>
    /// <param name="sortType"></param>
    /// <returns></returns>
    public static string GetSortText(MAINMENU_SORT_SEQ sortType)
    {
        return GameTextUtil.GetText(GameTextUtil.GetSortToTextKey(sortType));
    }

    /// <summary>
    /// ソート項目のテキストキーを取得
    /// </summary>
    /// <param name="sortType">ソートタイプ</param>
    /// <returns>対応したゲームテキストキー</returns>
    public static string GetSortToTextKey(MAINMENU_SORT_SEQ sortType)
    {
        string strSortTypeKey = "";

        switch (sortType)
        {
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FILTER:
                strSortTypeKey = "SORT_TYPE_FILTER";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ATTACK:
                strSortTypeKey = "SORT_TYPE_ATK";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_COST:
                strSortTypeKey = "WORD_COST";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ELEMENT:
                strSortTypeKey = "SORT_TYPE_ELEMENT";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_HP:
                strSortTypeKey = "SORT_TYPE_HP";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ID:
                strSortTypeKey = "SORT_TYPE_ID";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_GET:
                strSortTypeKey = "SORT_TYPE_GET";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_KIND:
                strSortTypeKey = "SORT_TYPE_KIND";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_PLUS:
                strSortTypeKey = "SORT_TYPE_PLUS";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_DEFAULT:
                strSortTypeKey = "SORT_TYPE_DEFAULT";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LOGIN_TIME:
                strSortTypeKey = "SORT_TYPE_LOGIN_TIME";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RANK:
                strSortTypeKey = "SORT_TYPE_RANK";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_WAIT_HIM:
                strSortTypeKey = "SORT_TYPE_WAIT_HIM";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_WAIT_ME:
                strSortTypeKey = "SORT_TYPE_WAIT_ME";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE:
                strSortTypeKey = "SORT_TYPE_FAVORITE";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LEVEL:
                strSortTypeKey = "SORT_TYPE_LEVEL";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RARE:
                strSortTypeKey = "SORT_TYPE_RARE";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MISSION_CLEAR:
                strSortTypeKey = "SORT_TYPE_MISSION_CLEAR";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MISSION_CHALLENGING:
                strSortTypeKey = "SORT_TYPE_MISSION_CHALLENGING";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MISSION_ACQUIRED:
                strSortTypeKey = "SORT_TYPE_MISSION_ACQUIRED";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LIMIT_OVER:
                strSortTypeKey = "SORT_TYPE_LIMIT_OVER";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MISSION_GP_GET_REWARD:
                strSortTypeKey = "SORT_TYPE_MISSION_GP_GET_REWARD";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MISSION_GP_NEW:
                strSortTypeKey = "SORT_TYPE_MISSION_GP_NEW";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MISSION_GP_ACQUIRED:
                strSortTypeKey = "SORT_TYPE_MISSION_GP_CLEAR";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_CHARM:
                strSortTypeKey = "SORT_TYPE_CHARM";
                break;
        }

        return strSortTypeKey;
    }

    /// <summary>
    /// フィルター項目のテキストを取得
    /// </summary>
    /// <param name="filterType"></param>
    /// <returns></returns>
    public static string GetSortDialogFilterText(MAINMENU_FILTER_TYPE filterType)
    {
        string textkey = "";
        switch (filterType)
        {
            case MAINMENU_FILTER_TYPE.FILTER_RARE:
                textkey = "filter_text1";
                break;
            case MAINMENU_FILTER_TYPE.FILTER_ELEMENT:
                textkey = "filter_text2";
                break;
            case MAINMENU_FILTER_TYPE.FILTER_KIND:
                textkey = "filter_text3";
                break;
        }

        return GameTextUtil.GetText(textkey);
    }

    /// <summary>
    /// ソート項目のテキストを取得
    /// </summary>
    /// <param name="sortType"></param>
    /// <returns></returns>
    public static string GetSortDialogSortText(MAINMENU_SORT_SEQ sortType)
    {
        string textkey = "";
        switch (sortType)
        {
            case MAINMENU_SORT_SEQ.SEQ_INIT:
                textkey = "filter_text49";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FILTER:
                textkey = "filter_text17";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ATTACK:
                textkey = "filter_text12";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE:
                textkey = "filter_text4";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_COST:
                textkey = "filter_text6";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_HP:
                textkey = "filter_text11";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ELEMENT:
                textkey = "filter_text8";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ID:
                textkey = "filter_text13";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_GET:
                textkey = "filter_text5";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_KIND:
                textkey = "filter_text9";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_PLUS:
                textkey = "filter_text16";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_DEFAULT:
                textkey = "filter_text18";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LOGIN_TIME:
                textkey = "filter_text53";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RANK:
                textkey = "filter_text52";
                break;
            //case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_WAIT_HIM:
            //    textkey = "";
            //    break;
            //case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_WAIT_ME:
            //    textkey = "";
            //    break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LEVEL:
                textkey = "filter_text10";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RARE:
                textkey = "filter_text7";
                break;
            //case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MISSION_CLEAR:
            //    textkey = "";
            //    break;
            //case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MISSION_CHALLENGING:
            //    textkey = "";
            //    break;
            //case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MISSION_ACQUIRED:
            //    textkey = "";
            //    break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LIMIT_OVER:
                textkey = "filter_text14";
                break;
            //case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MISSION_GP_GET_REWARD:
            //    textkey = "";
            //    break;
            //case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MISSION_GP_NEW:
            //    textkey = "";
            //    break;
            //case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_MISSION_GP_ACQUIRED:
            //    textkey = "";
            //    break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_CHARM:
                textkey = "filter_text15";
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE_SORT:
                textkey = "filter_text17";
                break;
        }

        return GameTextUtil.GetText(textkey);
    }

    /// <summary>
    /// ソートの種族テキストを取得
    /// </summary>
    /// <param name="kindType"></param>
    /// <returns></returns>
    public static string GetSortDialogKindText(MasterDataDefineLabel.KindType kindType)
    {
        string textKey = "";
        switch (kindType)
        {
            case MasterDataDefineLabel.KindType.HUMAN:
                textKey = "filter_text35";
                break;
            case MasterDataDefineLabel.KindType.DRAGON:
                textKey = "filter_text36";
                break;
            case MasterDataDefineLabel.KindType.GOD:
                textKey = "filter_text37";
                break;
            case MasterDataDefineLabel.KindType.DEMON:
                textKey = "filter_text38";
                break;
            case MasterDataDefineLabel.KindType.CREATURE:
                textKey = "filter_text39";
                break;
            case MasterDataDefineLabel.KindType.BEAST:
                textKey = "filter_text40";
                break;
            case MasterDataDefineLabel.KindType.MACHINE:
                textKey = "filter_text41";
                break;
            case MasterDataDefineLabel.KindType.EGG:
                textKey = "filter_text42";
                break;
        }
        return GameTextUtil.GetText(textKey);
    }

    /// <summary>
    /// ソートの属性テキストを取得
    /// </summary>
    /// <param name="elementType"></param>
    /// <returns></returns>
    public static string GetSortDialogElementText(MasterDataDefineLabel.ElementType elementType)
    {
        string textKey = "";
        switch (elementType)
        {
            case MasterDataDefineLabel.ElementType.NAUGHT:
                textKey = "filter_text34";
                break;
            case MasterDataDefineLabel.ElementType.FIRE:
                textKey = "filter_text29";
                break;
            case MasterDataDefineLabel.ElementType.WATER:
                textKey = "filter_text30";
                break;
            case MasterDataDefineLabel.ElementType.LIGHT:
                textKey = "filter_text32";
                break;
            case MasterDataDefineLabel.ElementType.DARK:
                textKey = "filter_text33";
                break;
            case MasterDataDefineLabel.ElementType.WIND:
                textKey = "filter_text31";
                break;
        }
        return GameTextUtil.GetText(textKey);
    }

    /// <summary>
    /// ソートのレア度テキストを取得
    /// </summary>
    /// <param name="rareType"></param>
    /// <returns></returns>
    public static string GetSortDialogRareText(MasterDataDefineLabel.RarityType rareType)
    {
        string textKey = "";
        switch (rareType)
        {
            case MasterDataDefineLabel.RarityType.STAR_1:
                textKey = "filter_text21";
                break;
            case MasterDataDefineLabel.RarityType.STAR_2:
                textKey = "filter_text22";
                break;
            case MasterDataDefineLabel.RarityType.STAR_3:
                textKey = "filter_text23";
                break;
            case MasterDataDefineLabel.RarityType.STAR_4:
                textKey = "filter_text24";
                break;
            case MasterDataDefineLabel.RarityType.STAR_5:
                textKey = "filter_text25";
                break;
            case MasterDataDefineLabel.RarityType.STAR_6:
                textKey = "filter_text26";
                break;
            case MasterDataDefineLabel.RarityType.STAR_7:
                textKey = "filter_text27";
                break;
        }
        return GameTextUtil.GetText(textKey);
    }

    public static string GetGachaPlayText(int count, MasterDataGacha _master)
    {
        string _ret = "";
        string payItem = "";
        uint price = (uint)(_master.price * count);
        MasterDataDefineLabel.BoolType paid_tip_only = _master.paid_tip_only;

        switch (_master.type)
        {
            case MasterDataDefineLabel.GachaType.NONE:
                //Money使用
                payItem = "Money";
                break;
            case MasterDataDefineLabel.GachaType.RARE:
            case MasterDataDefineLabel.GachaType.EVENT:
            case MasterDataDefineLabel.GachaType.TUTORIAL:
                // チップ使用
                payItem = GameTextUtil.GetText("common_text6");
                //有料チップ専用なら有料チップ数を設定
                if (paid_tip_only == MasterDataDefineLabel.BoolType.ENABLE)
                {
                    payItem = GameTextUtil.GetText("common_text13");
                }
                break;
            case MasterDataDefineLabel.GachaType.NORMAL:
            case MasterDataDefineLabel.GachaType.LUNCH:
                // フレンドポイント使用
                payItem = GameTextUtil.GetText("common_text1");
                break;
            case MasterDataDefineLabel.GachaType.TICKET:
                break;
            case MasterDataDefineLabel.GachaType.EVENT_POINT:
                break;
            case MasterDataDefineLabel.GachaType.ITEM_POINT:
                // アイテムポイント使用
                payItem = MasterDataUtil.GetItemNameFromGachaMaster(_master);
                break;
            case MasterDataDefineLabel.GachaType.STEP_UP:
                MasterDataStepUpGacha stepMaster = MasterDataUtil.GetMasterDataStepUpGachaFromGachaID(_master.fix_id);
                paid_tip_only = stepMaster.paid_tip_only;

                // チップ使用
                payItem = GameTextUtil.GetText("common_text6");
                //有料チップ専用なら有料チップ数を設定
                if (paid_tip_only == MasterDataDefineLabel.BoolType.ENABLE)
                {
                    payItem = GameTextUtil.GetText("common_text13");
                }
                break;
            default:
                break;
        }
        uint itemVal = 0;
        //--------------------------------
        // ガチャタイプにあわせて引ける回数を計算
        //--------------------------------
        switch (_master.type)
        {
            // 友情ガチャ
            case MasterDataDefineLabel.GachaType.NORMAL:
                {
                    itemVal = UserDataAdmin.Instance.m_StructPlayer.have_friend_pt;
                }
                break;
            // レア
            case MasterDataDefineLabel.GachaType.RARE:
            // イベント(コラボ)
            case MasterDataDefineLabel.GachaType.EVENT:
            // チュートリアル
            case MasterDataDefineLabel.GachaType.TUTORIAL:
            // イベント2
            case MasterDataDefineLabel.GachaType.EVENT_2:
                {
                    itemVal = UserDataAdmin.Instance.m_StructPlayer.have_stone;
                    //有料チップ専用なら有料チップ数を設定
                    if (paid_tip_only == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        itemVal = UserDataAdmin.Instance.m_StructPlayer.have_stone_pay;
                    }
                }
                break;
            // ランチ
            case MasterDataDefineLabel.GachaType.LUNCH:
                {
                    itemVal = 1;
                }
                break;
            case MasterDataDefineLabel.GachaType.EVENT_POINT:
                {
                    itemVal = 1;
                }
                break;
            case MasterDataDefineLabel.GachaType.ITEM_POINT:
                {
                    itemVal = UserDataAdmin.Instance.GetItemPoint(_master.cost_item_id);
                }
                break;
            // ステップアップ
            case MasterDataDefineLabel.GachaType.STEP_UP:
                {
                    MasterDataStepUpGachaManage stepManageMaster = MasterDataUtil.GetCurrentStepUpGachaManageMaster(_master.fix_id);
                    price = stepManageMaster.price;
                    itemVal = UserDataAdmin.Instance.m_StructPlayer.have_stone;
                    //有料チップ専用なら有料チップ数を設定
                    if (paid_tip_only == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        itemVal = UserDataAdmin.Instance.m_StructPlayer.have_stone_pay;
                    }
                }
                break;
        }
        if (count == 1)
        {
            string mainFormat = GetText("sc146q_content1");
            _ret = string.Format(mainFormat, payItem, price, itemVal);
        }
        else
        {
            string mainFormat = GetText("sc147q_content");
            _ret = string.Format(mainFormat, payItem, price, itemVal, count);
        }

#if BUILD_TYPE_DEBUG
        if (DebugOption.Instance.disalbeDebugMenu == false)
        {
            _ret += "\n\r" + "\n\r<color=#FF0000>デバッグ表示</color>\n\r";
            _ret += "スクラッチID[ " + _master.fix_id + " ]";
        }
#endif

        return _ret;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	表示用残り時間の取得
	*/
    //----------------------------------------------------------------------------
    static public string GetRemainStr(TimeSpan ts, string format)
    {
        string RemainDay = null;

        if (ts.TotalDays > 1.0)
        {
            RemainDay = format + string.Format(GameTextUtil.GetText("general_time_03"), (int)ts.TotalDays);
        }
        else
        if (ts.TotalHours > 1.0)
        {
            RemainDay = format + string.Format(GameTextUtil.GetText("general_time_04"), (int)ts.TotalHours);
        }
        else
        if (ts.TotalMinutes > 1.0)
        {
            RemainDay = format + string.Format(GameTextUtil.GetText("general_time_05"), (int)ts.TotalMinutes);
        }
        else
        if (ts.TotalSeconds >= 1.0)
        {
            RemainDay = format + string.Format(GameTextUtil.GetText("general_time_06"), (int)ts.TotalSeconds);
        }

        return RemainDay;

    }

    /// <summary>
    /// ミッションの表示フィルタ項目のテキスト取得
    /// </summary>
    /// <param name="filterType"></param>
    /// <returns></returns>
    static public string GetMissonDrawFilterText(MasterDataDefineLabel.AchievementFilterType filterType)
    {

        string textKey = "";
        switch (filterType)
        {
            case MasterDataDefineLabel.AchievementFilterType.ALL:
                textKey = "filter_display_all";
                break;
            case MasterDataDefineLabel.AchievementFilterType.NOT_ACHIEVED:
                textKey = "filter_display_not_achieved";
                break;
            case MasterDataDefineLabel.AchievementFilterType.UNACQUIRED:
                textKey = "filter_display_unacquired";
                break;
        }

        return GameTextUtil.GetText(textKey);
    }

    /// <summary>
    /// ミッションの種別受け取のフィルタ項目のテキスト取得
    /// </summary>
    /// <param name="filterType"></param>
    /// <returns></returns>
    static public string GetMissonReceiveFilterText(MasterDataDefineLabel.AchievementReceiveType filterType)
    {
        string textKey = "";
        switch (filterType)
        {
            case MasterDataDefineLabel.AchievementReceiveType.NONE:
                textKey = "filter_display_all";
                break;
            case MasterDataDefineLabel.AchievementReceiveType.UNIT:
                textKey = "filter_get_unit";
                break;
            case MasterDataDefineLabel.AchievementReceiveType.COIN:
                textKey = "filter_get_coin";
                break;
            case MasterDataDefineLabel.AchievementReceiveType.CHIP:
                textKey = "filter_get_chip";
                break;
            case MasterDataDefineLabel.AchievementReceiveType.OTHER:
                textKey = "filter_get_etc";
                break;
            default:
                break;
        }

        return GameTextUtil.GetText(textKey);
    }

}


