using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SORT_PARAM
{
    NONE = -1,

    ID,           //!< ID順
    LEVEL,        //!< レベル順
    HP,           //!< 体力順
    POW,          //!< 高家気力順
    PLUS,         //!< プラス値順
    COST,         //!< コスト順
    RARITY,       //!< レア度順
    ELEMENT,      //!< 属性順
    SUB_ELEMENT,  //!< 副属性順
    KIND,         //!< 種族順
    SUB_KIND,     //!< 副種族順
    LIMMIT_OVER,  //!< 限界突破レベル順
    CHARM,        //!< チャーム順
    GET_TIME,     //!< 入手順
    UNIQUE_ID,    //!< ユニークID

    FAVORITE,     //!< お気に入り順
    PARTY,        //!< パーティ順

    RANK,         //!< ランク順
    LOGIN_TIME,   //!< ログイン時間順
    FRIEND_STATE, //!< フレンドステータス順

    LIMITED,      //!< 限定順
    RATIO_UP,     //!< 確率UP順

    GROUP_ID,     //!< グループID

    MAX,
}

public enum SORT_ORDER
{
    NONE = -1,
    ASCENDING,      //!< 昇順
    DESCENDING,     //!< 降順
}

public enum SORT_OBJECT_TYPE
{
    NONE = -1,
    UNIT_LIST,       //!< ユニットリスト
    LINUP_LIST,     //!<　ガチャラインナップ
    FRIEND_LIST,     //!<　フレンドリスト
}

public class SortInfo
{
    public SORT_PARAM m_Type;
    public SORT_ORDER m_Order;

    public SortInfo(SORT_PARAM _type, SORT_ORDER _order)
    {
        m_Type = _type;
        m_Order = _order;
    }
}