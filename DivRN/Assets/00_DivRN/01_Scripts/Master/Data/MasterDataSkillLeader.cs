using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：スキル関連：リーダースキル
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataSkillLeader : Master
{
    public uint timing_public { get; set; }                  //!< 一般公開タイミング

    public string name { get; set; }                         //!< リーダースキル情報：名称和名
    public string detail { get; set; }                           //!< リーダースキル情報：詳細説明

    public uint add_fix_id { get; set; }                     //!< リーダースキル情報：追加リーダースキル

    public MasterDataDefineLabel.BoolType skill_powup_elem_active { get; set; }     //!< リーダースキル情報：味方属性別ステータス倍率効果：効果有無
    public MasterDataDefineLabel.ElementType skill_powup_elem_type { get; set; }           //!< リーダースキル情報：味方属性別ステータス倍率効果：属性
    public MasterDataDefineLabel.StatusType skill_powup_elem_status { get; set; }     //!< リーダースキル情報：味方属性別ステータス倍率効果：ステータスタイプ
    public int skill_powup_elem_rate { get; set; }           //!< リーダースキル情報：味方属性別ステータス倍率効果：倍率

    public MasterDataDefineLabel.BoolType skill_powup_kind_active { get; set; }     //!< リーダースキル情報：味方種族別ステータス倍率効果：効果有無
    public MasterDataDefineLabel.KindType skill_powup_kind_type { get; set; }           //!< リーダースキル情報：味方種族別ステータス倍率効果：種族
    public MasterDataDefineLabel.StatusType skill_powup_kind_status { get; set; }     //!< リーダースキル情報：味方種族別ステータス倍率効果：ステータスタイプ
    public int skill_powup_kind_rate { get; set; }           //!< リーダースキル情報：味方種族別ステータス倍率効果：倍率

    public MasterDataDefineLabel.BoolType skill_follow_atk_active { get; set; }     //!< リーダースキル情報：追いうち効果：効果有無
    public MasterDataDefineLabel.ElementType skill_follow_atk_element { get; set; }        //!< リーダースキル情報：追いうち効果：属性
    public int skill_follow_atk_rate { get; set; }           //!< リーダースキル情報：追いうち効果：倍率
    public MasterDataDefineLabel.UIEffectType skill_follow_atk_effect { get; set; }     //!< リーダースキル情報：追いうち効果：エフェクトタイプ

    public MasterDataDefineLabel.BoolType skill_decline_dmg_active { get; set; }        //!< リーダースキル情報：敵属性別被ダメ軽減効果：効果有無
    public MasterDataDefineLabel.ElementType skill_decline_dmg_element { get; set; }       //!< リーダースキル情報：敵属性別被ダメ軽減効果：属性
    public int skill_decline_dmg_rate { get; set; }          //!< リーダースキル情報：敵属性別被ダメ軽減効果：倍率

    //-------------------------

    public MasterDataDefineLabel.BoolType skill_recovery_move_active { get; set; }      //!< リーダースキル情報：移動リジェネ：効果有無
    public int skill_recovery_move_rate { get; set; }        //!< リーダースキル情報：移動リジェネ：倍率

    public MasterDataDefineLabel.BoolType skill_recovery_battle_active { get; set; }    //!< リーダースキル情報：バトルリジェネ：効果有無
    public int skill_recovery_battle_rate { get; set; }      //!< リーダースキル情報：バトルリジェネ：倍率

    public MasterDataDefineLabel.BoolType skill_quick_time_active { get; set; }     //!< リーダースキル情報：戦闘時秒数補正効果：効果有無
    public int skill_quick_time_second { get; set; }     //!< リーダースキル情報：戦闘時秒数補正効果：加算秒数

    public MasterDataDefineLabel.BoolType skill_recovery_support_active { get; set; }   //!< リーダースキル情報：回復量アップ効果：効果有無
    public int skill_recovery_support_rate { get; set; } //!< リーダースキル情報：回復量アップ効果：倍率

    public MasterDataDefineLabel.BoolType skill_recovery_atk_active { get; set; }       //!< リーダースキル情報：攻撃時回復効果：効果有無
    public int skill_recovery_atk_rate { get; set; }     //!< リーダースキル情報：攻撃時回復効果：倍率

    public MasterDataDefineLabel.BoolType skill_hpfull_powup_active { get; set; }       //!< リーダースキル情報：体力最大時攻撃力倍化効果：効果有無
    public int skill_hpfull_powup_rate { get; set; }     //!< リーダースキル情報：体力最大時攻撃力倍化効果：倍率

    public MasterDataDefineLabel.BoolType skill_hpdown_powup_active { get; set; }       //!< リーダースキル情報：体力減少時攻撃力倍化効果：効果有無
    public int skill_hpdown_powup_border { get; set; }       //!< リーダースキル情報：体力減少時攻撃力倍化効果：体力しきい値
    public int skill_hpdown_powup_rate { get; set; }     //!< リーダースキル情報：体力減少時攻撃力倍化効果：倍率

    public MasterDataDefineLabel.BoolType skill_mekuri_powup_active { get; set; }       //!< リーダースキル情報：パネルめくり数比例攻撃力倍化効果：効果有無

    public MasterDataDefineLabel.BoolType skill_funbari_active { get; set; }            //!< リーダースキル情報：ふんばり効果：効果有無
    public int skill_funbari_border { get; set; }            //!< リーダースキル情報：ふんばり効果：体力しきい値

    public MasterDataDefineLabel.BoolType skill_hpfull_guard_active { get; set; }       //!< リーダースキル情報：体力最大時被ダメ減衰効果：効果有無
    public int skill_hpfull_guard_rate { get; set; }     //!< リーダースキル情報：体力最大時被ダメ減衰効果：倍率

    public MasterDataDefineLabel.BoolType skill_initiative_atk_active { get; set; } //!< リーダースキル情報：先制攻撃発動割合変動効果：効果有無
    public int skill_initiative_atk_b_0 { get; set; }        //!< リーダースキル情報：先制攻撃発動割合変動効果：青パネル：先制
    public int skill_initiative_atk_b_1 { get; set; }        //!< リーダースキル情報：先制攻撃発動割合変動効果：青パネル：通常
    public int skill_initiative_atk_b_2 { get; set; }        //!< リーダースキル情報：先制攻撃発動割合変動効果：青パネル：不意打ち
    public int skill_initiative_atk_y_0 { get; set; }        //!< リーダースキル情報：先制攻撃発動割合変動効果：黄パネル：先制
    public int skill_initiative_atk_y_1 { get; set; }        //!< リーダースキル情報：先制攻撃発動割合変動効果：黄パネル：通常
    public int skill_initiative_atk_y_2 { get; set; }        //!< リーダースキル情報：先制攻撃発動割合変動効果：黄パネル：不意打ち
    public int skill_initiative_atk_r_0 { get; set; }        //!< リーダースキル情報：先制攻撃発動割合変動効果：赤パネル：先制
    public int skill_initiative_atk_r_1 { get; set; }        //!< リーダースキル情報：先制攻撃発動割合変動効果：赤パネル：通常
    public int skill_initiative_atk_r_2 { get; set; }        //!< リーダースキル情報：先制攻撃発動割合変動効果：赤パネル：不意打ち

    public MasterDataDefineLabel.BoolType skill_transform_card_active { get; set; } //!< リーダースキル情報：指定属性の手札を指定属性のカードに変換：効果有無
    public MasterDataDefineLabel.ElementType skill_transform_card_root { get; set; }       //!< リーダースキル情報：指定属性の手札を指定属性のカードに変換：変換前
    public MasterDataDefineLabel.ElementType skill_transform_card_dest { get; set; }       //!< リーダースキル情報：指定属性の手札を指定属性のカードに変換：変換後

    public MasterDataDefineLabel.BoolType skill_damageup_color_active { get; set; } //!< リーダースキル情報：指定色以上で攻撃をした際にダメージUP：効果有無
    public int skill_damageup_color_count { get; set; }      //!< リーダースキル情報：指定色以上で攻撃をした際にダメージUP：変換前属性
    public int skill_damageup_color_rate { get; set; }       //!< リーダースキル情報：指定色以上で攻撃をした際にダメージUP：変換後属性

    public MasterDataDefineLabel.BoolType skill_damageup_hands_active { get; set; } //!< リーダースキル情報：指定HANDS以上で攻撃をした際にダメージUP：効果有無
    public int skill_damageup_hands_count { get; set; }      //!< リーダースキル情報：指定HANDS以上で攻撃をした際にダメージUP：変換前属性
    public int skill_damageup_hands_rate { get; set; }       //!< リーダースキル情報：指定HANDS以上で攻撃をした際にダメージUP：変換後属性

    public MasterDataDefineLabel.LeaderSkillCategory skill_type { get; set; }                      //!< リーダースキル情報：スキル効果

    public int skill_value_00 { get; set; }                  //!< リーダースキル情報：パラメータ_00
    public int skill_value_01 { get; set; }                  //!< リーダースキル情報：パラメータ_01
    public int skill_value_02 { get; set; }                  //!< リーダースキル情報：パラメータ_02
    public int skill_value_03 { get; set; }                  //!< リーダースキル情報：パラメータ_03
    public int skill_value_04 { get; set; }                  //!< リーダースキル情報：パラメータ_04
    public int skill_value_05 { get; set; }                  //!< リーダースキル情報：パラメータ_05
    public int skill_value_06 { get; set; }                  //!< リーダースキル情報：パラメータ_06
    public int skill_value_07 { get; set; }                  //!< リーダースキル情報：パラメータ_07
    public int skill_value_08 { get; set; }                  //!< リーダースキル情報：パラメータ_08
    public int skill_value_09 { get; set; }                  //!< リーダースキル情報：パラメータ_09
    public int skill_value_10 { get; set; }                  //!< リーダースキル情報：パラメータ_10
    public int skill_value_11 { get; set; }                  //!< リーダースキル情報：パラメータ_11
    public int skill_value_12 { get; set; }                  //!< リーダースキル情報：パラメータ_12
    public int skill_value_13 { get; set; }                  //!< リーダースキル情報：パラメータ_13
    public int skill_value_14 { get; set; }                  //!< リーダースキル情報：パラメータ_14
    public int skill_value_15 { get; set; }                  //!< リーダースキル情報：パラメータ_15



    public bool Is_skill_powup_elem_active()
    {
        bool ret_val = (skill_powup_elem_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_powup_kind_active()
    {
        bool ret_val = (skill_powup_kind_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_follow_atk_active()
    {
        bool ret_val = (skill_follow_atk_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_decline_dmg_active()
    {
        bool ret_val = (skill_decline_dmg_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_recovery_move_active()
    {
        bool ret_val = (skill_recovery_move_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_recovery_battle_active()
    {
        bool ret_val = (skill_recovery_battle_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_quick_time_active()
    {
        bool ret_val = (skill_quick_time_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_recovery_support_active()
    {
        bool ret_val = (skill_recovery_support_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_recovery_atk_active()
    {
        bool ret_val = (skill_recovery_atk_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_hpfull_powup_active()
    {
        bool ret_val = (skill_hpfull_powup_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_hpdown_powup_active()
    {
        bool ret_val = (skill_hpdown_powup_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_funbari_active()
    {
        bool ret_val = (skill_funbari_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_hpfull_guard_active()
    {
        bool ret_val = (skill_hpfull_guard_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_initiative_atk_active()
    {
        bool ret_val = (skill_initiative_atk_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_transform_card_active()
    {
        bool ret_val = (skill_transform_card_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_damageup_color_active()
    {
        bool ret_val = (skill_damageup_color_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_damageup_hands_active()
    {
        bool ret_val = (skill_damageup_hands_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }



    //LEADERSKILL_PARAM_HP_ATK_POWUP_AND_CHK	バラシフト：条件チェックをANDで行うか
    public bool Get_HP_ATK_POWUP_AND_CHK()
    {
        bool ret_val = ((MasterDataDefineLabel.BoolType)skill_value_00 == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    //LEADERSKILL_PARAM_HP_ATK_POWUP_KIND_00	バラシフト：種族指定
    public MasterDataDefineLabel.KindType[] GetArray_HP_ATK_POWUP_KIND()
    {
        MasterDataDefineLabel.KindType[] table =
        {
            (MasterDataDefineLabel.KindType)skill_value_06,
            (MasterDataDefineLabel.KindType)skill_value_07,
            (MasterDataDefineLabel.KindType)skill_value_08,
            (MasterDataDefineLabel.KindType)skill_value_09,
            (MasterDataDefineLabel.KindType)skill_value_10,
        };

        return table;
    }

    //LEADERSKILL_PARAM_HP_ATK_POWUP_ELEM_00	バラシフト：属性指定
    public MasterDataDefineLabel.ElementType[] GetArray_HP_ATK_POWUP_ELEM()
    {
        MasterDataDefineLabel.ElementType[] table =
        {
            (MasterDataDefineLabel.ElementType)skill_value_01,
            (MasterDataDefineLabel.ElementType)skill_value_02,
            (MasterDataDefineLabel.ElementType)skill_value_03,
            (MasterDataDefineLabel.ElementType)skill_value_04,
            (MasterDataDefineLabel.ElementType)skill_value_05,
        };

        return table;
    }

    //LEADERSKILL_PARAM_HP_ATK_POWUP_HP	バラシフト：体力倍率
    public int Get_HP_ATK_POWUP_HP()
    {
        return skill_value_11;
    }

    //LEADERSKILL_PARAM_HP_ATK_POWUP_ATK	バラシフト：攻撃力倍率
    public int Get_HP_ATK_POWUP_ATK()
    {
        return skill_value_12;
    }

    //LEADERSKILL_PARAM_DMGUP_CONDI_COST_1_COST	ダメ補正：コスト条件：条件：コスト
    public MasterDataDefineLabel.SkillCostType[] GetArray_DMGUP_CONDI_COST_COST()
    {
        MasterDataDefineLabel.SkillCostType[] table =
        {
            (MasterDataDefineLabel.SkillCostType)skill_value_00,
            (MasterDataDefineLabel.SkillCostType)skill_value_02,
            (MasterDataDefineLabel.SkillCostType)skill_value_04,
            (MasterDataDefineLabel.SkillCostType)skill_value_06,
            (MasterDataDefineLabel.SkillCostType)skill_value_08,
            (MasterDataDefineLabel.SkillCostType)skill_value_10,
            (MasterDataDefineLabel.SkillCostType)skill_value_12,
        };

        return table;
    }

    //LEADERSKILL_PARAM_DMGUP_CONDI_COST_1_COND	コスト条件：条件：条件
    public MasterDataDefineLabel.ConditionType[] GetArray_DMGUP_CONDI_COST_COND()
    {
        MasterDataDefineLabel.ConditionType[] table =
        {
            (MasterDataDefineLabel.ConditionType)skill_value_01,
            (MasterDataDefineLabel.ConditionType)skill_value_03,
            (MasterDataDefineLabel.ConditionType)skill_value_05,
            (MasterDataDefineLabel.ConditionType)skill_value_07,
            (MasterDataDefineLabel.ConditionType)skill_value_09,
            (MasterDataDefineLabel.ConditionType)skill_value_11,
            (MasterDataDefineLabel.ConditionType)skill_value_13,
        };

        return table;
    }

    //LEADERSKILL_PARAM_DMGUP_CONDI_COST_OR_NUM	コスト条件：条件：OR数
    public int Get_DMGUP_CONDI_COST_OR_NUM()
    {
        return skill_value_14;
    }

    //LEADERSKILL_PARAM_DMGUP_CONDI_COST_RATE	コスト条件：倍率(％)
    public int Get_DMGUP_CONDI_COST_RATE()
    {
        return skill_value_15;
    }

    //LEADERSKILL_PARAM_DMG_ENEMY_ELEM_ELEMENT	被ダメ補正：属性
    public MasterDataDefineLabel.ElementType Get_DMG_ENEMY_ELEM_ELEMENT()
    {
        return (MasterDataDefineLabel.ElementType)skill_value_00;
    }

    //LEADERSKILL_PARAM_DMG_ENEMY_ELEM_RATE	被ダメ補正：倍率
    public int Get_DMG_ENEMY_ELEM_RATE()
    {
        return skill_value_01;
    }

    //LEADERSKILL_PARAM_BFPNL_BTLSTART	場にパネルを配置：戦闘開始時
    public bool Get_BFPNL_BTLSTART()
    {
        bool ret_val = ((MasterDataDefineLabel.BoolType)skill_value_00 == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    //手札変換パラメータ
    public BattleFieldPanelChangeParam GetBattleFieldPanelChangeParam()
    {
        int[] param =
        {
            skill_value_00,
            skill_value_01,
            skill_value_02,
            skill_value_03,
            skill_value_04,
            skill_value_05,
            skill_value_06,
            skill_value_07,
            skill_value_08,
            skill_value_09,
            skill_value_10,
            skill_value_11,
            skill_value_12,
            skill_value_13,
            skill_value_14,
            skill_value_15,
        };

        BattleFieldPanelChangeParam ret_val = new BattleFieldPanelChangeParam();
        ret_val.init(param);

        return ret_val;
    }
};
