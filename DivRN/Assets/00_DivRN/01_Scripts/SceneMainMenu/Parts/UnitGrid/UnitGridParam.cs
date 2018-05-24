/**
 *  @file   UnitGridParam.cs
 *  @brief
 *  @author Developer
 *  @date   2017/06/06
 */

using UnityEngine;
using System.Collections;
using ServerDataDefine;

public class UnitGridParam
{
    public uint unit_id;
    public long unique_id;

    public bool party_assign;
    public bool favorite;
    public bool evolve;

    public long get_time;
    public int draw_id;
    public int level;
    public int hp;
    public int pow;
    public int plus;
    public int cost;
    public int rare;
    public MasterDataDefineLabel.ElementType element;
    public int element_int;
    public MasterDataDefineLabel.KindType kind;
    public int kind_int;
    public MasterDataDefineLabel.KindType sub_kind;
    public int sub_kind_int;
    public int limitover_lv;
    public double charm;
    public int rank;
    public long login_time;
    public int friend_state;
    public int ratio_up;
    public int limited;
    public int group_id;

    public PacketStructUnit unit = null;
    public MasterDataParamChara master = null;

    public UnitGridParam()
    {
        unit_id = 0;
    }

    public UnitGridParam(PacketStructUnit _main, MasterDataParamChara _mainMaster)
    {
        unit_id = 0;
        setSortParamUnit(_main, _mainMaster);
    }

    public void Copy(UnitGridParam _data)
    {
        unit_id = _data.unit_id;
        unique_id = _data.unique_id;

        party_assign = _data.party_assign;
        favorite = _data.favorite;
        evolve = _data.evolve;

        get_time = _data.get_time;
        draw_id = _data.draw_id;
        level = _data.level;
        hp = _data.hp;
        pow = _data.pow;
        plus = _data.plus;
        cost = _data.cost;
        rare = _data.rare;
        element = _data.element;
        kind = _data.kind;
        sub_kind = _data.sub_kind;
        limitover_lv = _data.limitover_lv;
        charm = _data.charm;

        unit = _data.unit;
        master = _data.master;
    }

    /// <summary>
    /// ソートパラメータ設定（ユニット）
    /// </summary>
    /// <param name="_main"></param>
    /// <param name="_mainMaster"></param>
    /// <param name="_sub"></param>
    public void setSortParamUnit(PacketStructUnit _main, MasterDataParamChara _mainMaster, bool bEvolCheck = true)
    {
        unit_id = _main.id;
        unique_id = _main.unique_id;
        unit = _main;
        master = _mainMaster;

        setSortParamUnit(_main, _mainMaster, CharaLinkUtil.GetLinkUnit(_main.link_unique_id));

        party_assign = MainMenuUtil.ChkUnitPartyAssign(_main.unique_id);
        favorite = MainMenuUtil.ChkFavoritedUnit(_main.unique_id);
        get_time = (long)_main.get_time;

        if (bEvolCheck)
        {
            MasterDataParamCharaEvol evol = MasterFinder<MasterDataParamCharaEvol>.Instance.SelectFirstWhere(" where unit_id_pre = ? ", _mainMaster.fix_id);
            evolve = (evol != null) ? true : false;
        }
    }

    /// <summary>
    /// ソートパラメータ設定（ユニット共通）
    /// </summary>
    /// <param name="_main"></param>
    /// <param name="_mainMaster"></param>
    /// <param name="_sub"></param>
    private void setSortParamUnit(PacketStructUnit _main, MasterDataParamChara _mainMaster, PacketStructUnit _sub)
    {
        CharaOnce baseChara = new CharaOnce();

        if (_main.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE &&
            _sub != null)
        {
            baseChara.CharaSetupFromParam(
                _mainMaster,
                (int)_main.level,
                (int)_main.limitbreak_lv,
                (int)_main.limitover_lv,
                (int)_main.add_pow,
                (int)_main.add_hp,
                _sub.id,
                (int)_sub.level,
                (int)_sub.add_pow,
                (int)_sub.add_hp,
                (int)_main.link_point,
                (int)_sub.limitover_lv
                );
        }
        else
        {
            baseChara.CharaSetupFromParam(
                _mainMaster,
                (int)_main.level,
                (int)_main.limitbreak_lv,
                (int)_main.limitover_lv,
                (int)_main.add_pow,
                (int)_main.add_hp,
                0,
                0,
                0,
                0,
                0,
                0
                );
        }

        draw_id = (int)_mainMaster.draw_id;
        level = (int)_main.level;
        hp = baseChara.m_CharaHP;
        pow = baseChara.m_CharaPow;
        plus = baseChara.m_CharaPlusHP + baseChara.m_CharaPlusPow;
        cost = _mainMaster.party_cost;
        if (_main.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE && _sub != null)
        {
            cost += CharaLinkUtil.GetLinkUnitCost(_sub.id);
        }
        rare = (int)_mainMaster.rare;
        element = _mainMaster.element;
        kind = _mainMaster.kind;
        sub_kind = _mainMaster.sub_kind;
        limitover_lv = (int)_main.limitover_lv;
        charm = baseChara.m_CharaCharm;
    }
}
