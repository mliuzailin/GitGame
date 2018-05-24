/**
 *  @file   SortUtil.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/25
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ServerDataDefine;
using System;

public static class SortUtil
{
    /// <summary>
    /// ソートタイプの変換
    /// </summary>
    /// <param name="sortType"></param>
    /// <returns></returns>
    public static SORT_PARAM GetSortParam(MAINMENU_SORT_SEQ sortType)
    {
        SORT_PARAM sortParam = SORT_PARAM.NONE;

        switch (sortType)
        {
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ATTACK:
                sortParam = SORT_PARAM.POW;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE:
                sortParam = SORT_PARAM.FAVORITE;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_COST:
                sortParam = SORT_PARAM.COST;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_HP:
                sortParam = SORT_PARAM.HP;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ELEMENT:
                sortParam = SORT_PARAM.ELEMENT;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ID:
                sortParam = SORT_PARAM.ID;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_GET:
                sortParam = SORT_PARAM.GET_TIME;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_KIND:
                sortParam = SORT_PARAM.KIND;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_PLUS:
                sortParam = SORT_PARAM.PLUS;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LOGIN_TIME:
                sortParam = SORT_PARAM.LOGIN_TIME;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RANK:
                sortParam = SORT_PARAM.RANK;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LEVEL:
                sortParam = SORT_PARAM.LEVEL;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RARE:
                sortParam = SORT_PARAM.RARITY;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LIMIT_OVER:
                sortParam = SORT_PARAM.LIMMIT_OVER;
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_CHARM:
                sortParam = SORT_PARAM.CHARM;
                break;
            default:
#if BUILD_TYPE_DEBUG
                Debug.LogError("SortType NG!! - " + sortType);
#endif
                break;
        }

        return sortParam;
    }

    /// <summary>
    /// フィルタが設定されているかどうか調べる
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static bool CheckFilterWorking(int[] filters)
    {
        if (filters == null || filters.Length == 0) { return false; }

        return filters.Any((v) => v < 0);
    }

    /// <summary>
    /// 属性のソート順補正
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static int[] GetFilterNumElements(int[] filters)
    {
        List<int> datalist = new List<int>();
        for (int i = 0; i < filters.Length; i++)
        {
            if (filters[i] > 0)
            {
                datalist.Add(SortUtil.GetSortNumElement((MasterDataDefineLabel.ElementType)filters[i]));
            }
        }

        return datalist.ToArray();
    }

    /// <summary>
    /// 種族のソート順補正
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static int[] GetFilterNumKindss(int[] filters)
    {
        List<int> datalist = new List<int>();
        for (int i = 0; i < filters.Length; i++)
        {
            if (filters[i] >= 0)
            {
                datalist.Add(SortUtil.GetSortNumKind((MasterDataDefineLabel.KindType)filters[i]));
            }
        }

        return datalist.ToArray();
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  お気に入りのソート順補正
    */
    //----------------------------------------------------------------------------
    static public byte GetSortNumFavorite(bool bFavorite)
    {
        return (bFavorite == true) ? (byte)1 : (byte)2;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  パーティアサインのソート順補正
    */
    //----------------------------------------------------------------------------
    static public byte GetSortNumPartyAssign(bool bAssign)
    {
        return (bAssign == true) ? (byte)1 : (byte)2;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  種族のソート順補正
    */
    //----------------------------------------------------------------------------
    static public byte GetSortNumKind(MasterDataDefineLabel.KindType nKind)
    {
        switch (nKind)
        {
            case MasterDataDefineLabel.KindType.NONE: return 0;  // 種族タイプ： - 
            case MasterDataDefineLabel.KindType.HUMAN: return 1;  // 種族タイプ：種族：人間
            case MasterDataDefineLabel.KindType.DRAGON: return 2;  // 種族タイプ：種族：ドラゴン
            case MasterDataDefineLabel.KindType.GOD: return 3;  // 種族タイプ：種族：神
            case MasterDataDefineLabel.KindType.DEMON: return 4;  // 種族タイプ：種族：魔物
            case MasterDataDefineLabel.KindType.CREATURE: return 5;  // 種族タイプ：種族：妖精
            case MasterDataDefineLabel.KindType.BEAST: return 6;  // 種族タイプ：種族：獣
            case MasterDataDefineLabel.KindType.MACHINE: return 7;  // 種族タイプ：種族：機械
            case MasterDataDefineLabel.KindType.EGG: return 8;  // 種族タイプ：種族：強化合成用
            default:
#if BUILD_TYPE_DEBUG
                Debug.LogWarning("Kind NG!! - " + nKind);
#endif
                break;
        }
        return 0;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  属性のソート順補正
    */
    //----------------------------------------------------------------------------
    static public byte GetSortNumElement(MasterDataDefineLabel.ElementType nElement)
    {
        switch (nElement)
        {
            case MasterDataDefineLabel.ElementType.NAUGHT: return 6;  // スキルカットイン：無
            case MasterDataDefineLabel.ElementType.FIRE: return 1;  // スキルカットイン：炎
            case MasterDataDefineLabel.ElementType.WATER: return 2;  // スキルカットイン：水  
            case MasterDataDefineLabel.ElementType.LIGHT: return 4;  // スキルカットイン：光
            case MasterDataDefineLabel.ElementType.DARK: return 5;  // スキルカットイン：闇
            case MasterDataDefineLabel.ElementType.WIND: return 3;  // スキルカットイン：緑
            default:
#if BUILD_TYPE_DEBUG
                Debug.LogWarning("Element NG!! - " + nElement);
#endif
                break;
        }
        return 0;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  フレンドステータスのソート順補正
    */
    //----------------------------------------------------------------------------
    static public byte GetSortNumFriendState(FRIEND_STATE friendState)
    {
        switch (friendState)
        {
            case FRIEND_STATE.FRIEND_STATE_SUCCESS: return 1;
            case FRIEND_STATE.FRIEND_STATE_WAIT_ME: return 2;
            case FRIEND_STATE.FRIEND_STATE_WAIT_HIM: return 2;
            case FRIEND_STATE.FRIEND_STATE_UNRELATED: return 3;
            case FRIEND_STATE.FRIEND_STATE_PREMIUM: return 3;
        }
        return 0;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  ログイン時間のソート順補正
    */
    //----------------------------------------------------------------------------
    static public long GetSortNumLoginTime(ulong unLoginTime)
    {
        TimeSpan cTimeSpan = TimeManager.Instance.m_TimeNow - TimeUtil.ConvertServerTimeToLocalTime(unLoginTime);
        long nMinutes = (long)cTimeSpan.TotalMinutes;
        return nMinutes;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  ステータス変化時間のソート順補正
    */
    //----------------------------------------------------------------------------
    static public ushort GetSortNumFriendStateUpdate(ulong unStateUpdateTime)
    {
        TimeSpan cTimeSpan = TimeManager.Instance.m_TimeNow - TimeUtil.ConvertServerTimeToLocalTime(unStateUpdateTime);
        int nMinutes = (int)cTimeSpan.TotalMinutes;

        if (nMinutes > (0xffff))
        {
            return 0xffff;
        }
        return (ushort)nMinutes;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  ユニット取得時間のソート順補正
    */
    //----------------------------------------------------------------------------
    static public uint GetSortNumUnitGetTime(ulong unUnitGetTime)
    {
        TimeSpan cTimeSpan = TimeManager.Instance.m_TimeNow - TimeUtil.ConvertServerTimeToLocalTime(unUnitGetTime);
        uint nMinutes = (uint)cTimeSpan.TotalMinutes;

        if (nMinutes > (0xffffffff))
        {
            return 0xffffffff;
        }
        return (uint)nMinutes;
    }
}
