using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using System.Linq;
using ServerDataDefine;

public class SortObjectBase : M4uContext
{
    static public System.Type[] paramType = new System.Type[(int)SORT_PARAM.MAX]
    {
        typeof(int),        //!< ID
        typeof(int),        //!< LEVEL
        typeof(int),        //!< HP
        typeof(int),        //!< STR
        typeof(int),        //!< PLUS
        typeof(int),        //!< COST
        typeof(int),        //!< RARITY
        typeof(int),        //!< ELEMENT
        typeof(int),        //!< SUB_ELEMENT
        typeof(int),        //!< KIND
        typeof(int),        //!< SUB_KIND
        typeof(int),        //!< LIMIT_OVER
        typeof(double),     //!< CHARM
        typeof(long),       //!< GET_TIME
        typeof(long),       //!< UNIQUE_ID

        typeof(bool),       //!< FAVORITE
        typeof(bool),       //!< PARTY

        typeof(int),        //!< RANK
        typeof(long),       //!< LOGIN_TIME
        typeof(int),        //!< FRIEND_STATE

        typeof(int),        //!< LIMITED
        typeof(int),        //!< RATIO_UP

		typeof(int),        //!< GROUP_ID
    };


    private bool isActive = true;
    public bool IsActive { get { return isActive; } set { isActive = value; } }
    private List<SortParamBase> ParamList = new List<SortParamBase>();

    public UnitGridParam UnitParam;

    private bool CheckType(SORT_PARAM _type, System.Type _paramType)
    {
        if (paramType.IsOverRange((int)_type))
        {
            Debug.LogError(string.Format("OVERRANGE_TYPE:{0}", _type));
            return false;
        }

        if (_paramType != paramType[(int)_type])
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 比較処理（ユニットリスト専用）
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="_obj"></param>
    /// <param name="_order"></param>
    /// <returns></returns>
    private int CompareToLong(long a, long b)
    {
        if (a < b)
        {
            return 1;
        }

        if (a > b)
        {
            return -1;
        }

        return 0;
    }

    private int CompareToDouble(double a, double b)
    {
        if (a < b)
        {
            return 1;
        }

        if (a > b)
        {
            return -1;
        }

        return 0;
    }

    private int CompareToBool(bool a, bool b)
    {
        if (a == true && b == false)
        {
            return 1;
        }

        if (a == false && b == true)
        {
            return -1;
        }

        return 0;
    }

    public int CompareUnitTo(SortInfo info, SortObjectBase _obj)
    {
        int _ret = 0;

        switch (info.m_Type)
        {
            case SORT_PARAM.ID:
                _ret = _obj.UnitParam.draw_id - UnitParam.draw_id;
                break;
            case SORT_PARAM.LEVEL:
                _ret = _obj.UnitParam.level - UnitParam.level;
                break;
            case SORT_PARAM.HP:
                _ret = _obj.UnitParam.hp - UnitParam.hp;
                break;
            case SORT_PARAM.POW:
                _ret = _obj.UnitParam.pow - UnitParam.pow;
                break;
            case SORT_PARAM.PLUS:
                _ret = _obj.UnitParam.plus - UnitParam.plus;
                break;
            case SORT_PARAM.COST:
                _ret = _obj.UnitParam.cost - UnitParam.cost;
                break;
            case SORT_PARAM.RARITY:
                _ret = _obj.UnitParam.rare - UnitParam.rare;
                break;
            case SORT_PARAM.ELEMENT:
                _ret = _obj.UnitParam.element_int - UnitParam.element_int;
                break;
            case SORT_PARAM.KIND:
                _ret = _obj.UnitParam.kind_int - UnitParam.kind_int;
                break;
            case SORT_PARAM.SUB_KIND:
                _ret = _obj.UnitParam.sub_kind_int - UnitParam.sub_kind_int;
                break;
            case SORT_PARAM.LIMMIT_OVER:
                _ret = _obj.UnitParam.limitover_lv - UnitParam.limitover_lv;
                break;
            case SORT_PARAM.CHARM: //double
                _ret = CompareToDouble(UnitParam.charm, _obj.UnitParam.charm);
                break;
            case SORT_PARAM.PARTY: //bool
                _ret = CompareToBool(UnitParam.party_assign, _obj.UnitParam.party_assign);
                break;
            case SORT_PARAM.FAVORITE: //bool
                _ret = CompareToBool(UnitParam.favorite, _obj.UnitParam.favorite);
                break;
            case SORT_PARAM.GET_TIME: //long
                _ret = CompareToLong(UnitParam.get_time, _obj.UnitParam.get_time);
                break;
            case SORT_PARAM.UNIQUE_ID://long
                _ret = CompareToLong(UnitParam.unique_id, _obj.UnitParam.unique_id);
                break;

            case SORT_PARAM.RANK:
                _ret = _obj.UnitParam.rank - UnitParam.rank;
                break;
            case SORT_PARAM.LOGIN_TIME:
                _ret = CompareToLong(UnitParam.login_time, _obj.UnitParam.login_time);
                break;
            case SORT_PARAM.FRIEND_STATE:
                _ret = _obj.UnitParam.friend_state - UnitParam.friend_state;
                break;

            case SORT_PARAM.RATIO_UP:
                _ret = _obj.UnitParam.ratio_up - UnitParam.ratio_up;
                break;
            case SORT_PARAM.LIMITED:
                _ret = _obj.UnitParam.limited - UnitParam.limited;
                break;
            case SORT_PARAM.GROUP_ID:
                _ret = _obj.UnitParam.group_id - UnitParam.group_id;
                break;

            default:
                Debug.Assert(true, "CompareUnitTo no tyoe");
                break;
        }

        if (info.m_Order == SORT_ORDER.ASCENDING)
        {
            _ret *= -1;
        }

        return _ret;
    }

    /// <summary>
    /// フィルタ処理（ユニット）
    /// </summary>
    /// <param name="_filter"></param>
    public void FilterUnitTo(SortFilterBase _filter)
    {
        bool bActive = false;

        for (int i = 0; i < _filter.m_Types.Length; i++)
        {
            SORT_PARAM param = _filter.m_Types[i];

            if (bActive)
            {
                break;
            }

            switch (param)
            {
                case SORT_PARAM.ID:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.draw_id);
                    break;
                case SORT_PARAM.LEVEL:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.level);
                    break;
                case SORT_PARAM.HP:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.hp);
                    break;
                case SORT_PARAM.POW:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.pow);
                    break;
                case SORT_PARAM.PLUS:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.plus);
                    break;
                case SORT_PARAM.COST:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.cost);
                    break;
                case SORT_PARAM.RARITY:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.rare);
                    break;
                case SORT_PARAM.ELEMENT:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.element_int);
                    break;
                case SORT_PARAM.KIND:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.kind_int);
                    break;
                case SORT_PARAM.SUB_KIND:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.sub_kind_int);
                    break;
                case SORT_PARAM.LIMMIT_OVER:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.limitover_lv);
                    break;
                case SORT_PARAM.CHARM: //double
                    bActive = ((SortFilter<double>)_filter).FilterTo(UnitParam.charm);
                    break;
                case SORT_PARAM.PARTY: //bool
                    bActive = ((SortFilter<bool>)_filter).FilterTo(UnitParam.party_assign);
                    break;
                case SORT_PARAM.FAVORITE: //bool
                    bActive = ((SortFilter<bool>)_filter).FilterTo(UnitParam.favorite);
                    break;
                case SORT_PARAM.GET_TIME: //long
                    bActive = ((SortFilter<long>)_filter).FilterTo(UnitParam.get_time);
                    break;
                case SORT_PARAM.UNIQUE_ID://long
                    bActive = ((SortFilter<long>)_filter).FilterTo(UnitParam.unique_id);
                    break;

                case SORT_PARAM.RANK:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.rank);
                    break;
                case SORT_PARAM.LOGIN_TIME://long
                    bActive = ((SortFilter<long>)_filter).FilterTo(UnitParam.login_time);
                    break;
                case SORT_PARAM.FRIEND_STATE:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.friend_state);
                    break;

                case SORT_PARAM.RATIO_UP:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.ratio_up);
                    break;
                case SORT_PARAM.LIMITED:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.limited);
                    break;
                case SORT_PARAM.GROUP_ID:
                    bActive = ((SortFilter<int>)_filter).FilterTo(UnitParam.group_id);
                    break;

                default:
                    Debug.Assert(true, "FilterUnitTo no tyoe");
                    break;
            }
        }

        isActive = bActive;
    }

    /// <summary>
    /// ソートパラメータ設定（ユニット）
    /// </summary>
    public virtual void setSortParamUnit(UnitGridParam unit_param)
    {
        unit_param.element_int = SortUtil.GetSortNumElement(unit_param.element);
        unit_param.kind_int = SortUtil.GetSortNumKind(unit_param.kind);
        unit_param.sub_kind_int = SortUtil.GetSortNumKind(unit_param.sub_kind);
        UnitParam = unit_param;
    }

    /// <summary>
    /// ソートパラメータ設定（フレンド）
    /// </summary>
    /// <param name="_friend"></param>
    public void setSortParamFriend(
        PacketStructFriend _friend,
        CharaOnce _baseChara,
        MasterDataParamChara _master
        )
    {
        PacketStructUnit _sub = null;
        if (_friend.unit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE)
        {
            _sub = _friend.unit_link;
        }
        int _cost = _master.party_cost;
        if (_sub != null)
        {
            _cost += CharaLinkUtil.GetLinkUnitCost(_sub.id);
        }
        UnitParam = new UnitGridParam();
        UnitParam.draw_id = (int)_master.draw_id;
        UnitParam.level = (int)_friend.unit.level;
        UnitParam.hp = _baseChara.m_CharaHP;
        UnitParam.pow = _baseChara.m_CharaPow;
        UnitParam.plus = _baseChara.m_CharaPlusHP + _baseChara.m_CharaPlusPow;
        UnitParam.cost = _cost;
        UnitParam.rare = (int)_master.rare;
        UnitParam.element_int = SortUtil.GetSortNumElement(_master.element);
        UnitParam.kind_int = SortUtil.GetSortNumKind(_master.kind);
        UnitParam.sub_kind_int = SortUtil.GetSortNumKind(_master.sub_kind);
        UnitParam.limitover_lv = (int)_friend.unit.limitover_lv;
        UnitParam.charm = _baseChara.m_CharaCharm;
        UnitParam.rank = (int)_friend.user_rank;
        UnitParam.login_time = SortUtil.GetSortNumLoginTime(_friend.last_play);
        UnitParam.favorite = MainMenuUtil.ChkFavoridFriend(_friend.user_id);
        UnitParam.friend_state = (int)_friend.friend_state;
    }

    /// <summary>
    /// ソートパラメータ設定（ガチャラインナップ）
    /// </summary>
    /// <param name="_lineup"></param>
    public void setSortParamLineUp(PacketStructGachaLineup _lineup, MasterDataParamChara _master)
    {
        UnitParam = new UnitGridParam();
        UnitParam.ratio_up = _lineup.rate_up_icon;
        UnitParam.limited = _lineup.limit_icon;
        UnitParam.group_id = _lineup.lineup_sort_group_id;
        UnitParam.rare = (int)_master.rare;
        UnitParam.draw_id = _lineup.id;
    }
}
