using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：スキル関連：パッシブスキル
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataSkillPassive : Master
{
    public uint timing_public { get; set; }                  //!< 一般公開タイミング

    public string name { get; set; }                         //!< パッシブスキル情報：名称和名
    public string detail { get; set; }                           //!< パッシブスキル情報：詳細説明

    public int add_fix_id { get; set; }                    //!< 追加パッシブfix_id

    //public MasterDataDefineLabel.BoolType skill_trap_pass_active { get; set; }          //!< パッシブスキル情報：移動中トラップ無効効果：効果有無
    //public MasterDataDefineLabel.TrapType skill_trap_pass_type { get; set; }            //!< パッシブスキル情報：移動中トラップ無効効果：トラップタイプ

    public MasterDataDefineLabel.BoolType skill_powup_kind_active { get; set; }     //!< パッシブスキル情報：敵種族別ステータス倍率効果：効果有無
    public MasterDataDefineLabel.KindType skill_powup_kind_type { get; set; }           //!< パッシブスキル情報：敵種族別ステータス倍率効果：種族
    public MasterDataDefineLabel.StatusType skill_powup_kind_status { get; set; }     //!< パッシブスキル情報：敵種族別ステータス倍率効果：変動ステータス
    public int skill_powup_kind_rate { get; set; }           //!< パッシブスキル情報：敵種族別ステータス倍率効果：倍率

    public MasterDataDefineLabel.BoolType skill_counter_atk_active { get; set; }        //!< パッシブスキル情報：カウンター効果：効果有無
    public MasterDataDefineLabel.ElementType skill_counter_atk_element { get; set; }       //!< パッシブスキル情報：カウンター効果：属性
    public int skill_counter_atk_odds { get; set; }          //!< パッシブスキル情報：カウンター効果：発動確率
    public int skill_counter_atk_scale { get; set; }     //!< パッシブスキル情報：カウンター効果：与ダメ倍率
    public MasterDataDefineLabel.UIEffectType skill_counter_atk_effect { get; set; }        //!< パッシブスキル情報：カウンター効果：エフェクトタイプ

    public MasterDataDefineLabel.BoolType skill_damage_recovery_active { get; set; }    //!< パッシブスキル情報：被ダメ時回復効果：効果有無
    public int skill_damage_recovery_odds { get; set; }      //!< パッシブスキル情報：被ダメ時回復効果：発動確率
    public int skill_damage_recovery_rate { get; set; }      //!< パッシブスキル情報：被ダメ時回復効果：回復割合

    public MasterDataDefineLabel.BoolType skill_hp_full_powup_active { get; set; }      //!< パッシブスキル情報：体力最大時攻撃力アップ効果：効果有無
    public int skill_hp_full_powup_scale { get; set; }       //!< パッシブスキル情報：体力最大時攻撃力アップ効果：倍率

    public MasterDataDefineLabel.BoolType skill_dying_powup_active { get; set; }        //!< パッシブスキル情報：瀕死時攻撃力アップ効果：効果有無
    public int skill_dying_powup_border { get; set; }        //!< パッシブスキル情報：瀕死時攻撃力アップ効果：体力しきい値
    public int skill_dying_powup_scale { get; set; }     //!< パッシブスキル情報：瀕死時攻撃力アップ効果：倍率

    public MasterDataDefineLabel.BoolType skill_backatk_pass_active { get; set; }       //!< パッシブスキル情報：バックアタック発生確率上書き：効果有無
    public int skill_backatk_pass_rate { get; set; }     //!< パッシブスキル情報：バックアタック発生確率上書き：確率

    public MasterDataDefineLabel.BoolType skill_decline_dmg_elem_active { get; set; }   //!< パッシブスキル情報：属性ダメージ軽減：効果有無
    public MasterDataDefineLabel.ElementType skill_decline_dmg_elem_elem { get; set; } //!< パッシブスキル情報：属性ダメージ軽減：属性
    public int skill_decline_dmg_elem_rate { get; set; } //!< パッシブスキル情報：属性ダメージ軽減：軽減倍率

    public MasterDataDefineLabel.BoolType skill_decline_dmg_kind_active { get; set; }   //!< パッシブスキル情報：種族ダメージ軽減：効果有無
    public MasterDataDefineLabel.KindType skill_decline_dmg_kind_kind { get; set; } //!< パッシブスキル情報：種族ダメージ軽減：種族
    public int skill_decline_dmg_kind_rate { get; set; } //!< パッシブスキル情報：種族ダメージ軽減：軽減倍率

    public MasterDataDefineLabel.BoolType skill_boost_chance_active { get; set; }       //!< パッシブスキル情報：ブーストパネル抽選回数増加：効果有無
    public int skill_boost_chance_count { get; set; }        //!< パッシブスキル情報：ブーストパネル抽選回数増加：回数

    public MasterDataDefineLabel.PassiveSkillCategory skill_type { get; set; }                      //!< パッシブスキル情報：スキルタイプ

    public int skill_param_00 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ00
    public int skill_param_01 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ01
    public int skill_param_02 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ02
    public int skill_param_03 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ03
    public int skill_param_04 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ04
    public int skill_param_05 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ05
    public int skill_param_06 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ06
    public int skill_param_07 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ07
    public int skill_param_08 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ08
    public int skill_param_09 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ09
    public int skill_param_10 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ10
    public int skill_param_11 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ11
    public int skill_param_12 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ12
    public int skill_param_13 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ13
    public int skill_param_14 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ14
    public int skill_param_15 { get; set; }                  //!< パッシブスキル情報：汎用パラメータ15



    private MasterDataDefineLabel.AutoPlaySkillType _Get_AutoPlaySkillType()
    {
        MasterDataDefineLabel.AutoPlaySkillType ret_val = MasterDataDefineLabel.AutoPlaySkillType.NONE;

        if (skill_type == MasterDataDefineLabel.PassiveSkillCategory.AUTO_PLAY_SKILL)
        {
            ret_val = (MasterDataDefineLabel.AutoPlaySkillType)skill_param_00;
        }

        return ret_val;
    }

    public bool Is_skill_powup_kind_active()
    {
        bool ret_val = (skill_powup_kind_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_counter_atk_active()
    {
        bool ret_val = (skill_counter_atk_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_damage_recovery_active()
    {
        bool ret_val = (skill_damage_recovery_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    private bool _Is_skill_hp_full_powup_active()
    {
        bool ret_val = (skill_hp_full_powup_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    private bool _Is_skill_dying_powup_active()
    {
        bool ret_val = (skill_dying_powup_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    private bool _Is_skill_backatk_pass_active()
    {
        bool ret_val = (skill_backatk_pass_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    private bool _Is_skill_decline_dmg_elem_active()
    {
        bool ret_val = (skill_decline_dmg_elem_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    private bool _Is_skill_decline_dmg_kind_active()
    {
        bool ret_val = (skill_decline_dmg_kind_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public bool Is_skill_boost_chance_active()
    {
        bool ret_val = (skill_boost_chance_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }



    //PASSIVESKILL_PARAM_FOLLOW_ATK_ELEM	追撃：属性
    private MasterDataDefineLabel.ElementType _Get_FOLLOW_ATK_ELEM()
    {
        return (MasterDataDefineLabel.ElementType)skill_param_00;
    }

    //PASSIVESKILL_PARAM_FOLLOW_ATK_SCALE	追撃：倍率
    private int _Get_FOLLOW_ATK_SCALE()
    {
        return skill_param_01;
    }

    //PASSIVESKILL_PARAM_FOLLOW_ATK_EFFECT	追撃：エフェクトタイプ
    private MasterDataDefineLabel.UIEffectType _Get_FOLLOW_ATK_EFFECT()
    {
        return (MasterDataDefineLabel.UIEffectType)skill_param_02;
    }

    //PASSIVESKILL_PARAM_HANDS_BARRIER_ELEM	手札バリア：属性
    public MasterDataDefineLabel.ElementType Get_HANDS_BARRIER_ELEM()
    {
        return (MasterDataDefineLabel.ElementType)skill_param_00;
    }

    //PASSIVESKILL_PARAM_HANDS_BARRIER_RATE	手札バリア：軽減倍率
    public int Get_HANDS_BARRIER_RATE()
    {
        return skill_param_01;
    }

    //PASSIVESKILL_PARAM_CARD_CHANCE_UP_FIRE	手札出現率調整（属性別）
    public int Get_CARD_CHANCE_UP(MasterDataDefineLabel.ElementType element_type)
    {
#if true
        // GC回避のためメモリ確保しないバージョン
        int ret_val = 0;
        switch (element_type)
        {
            case MasterDataDefineLabel.ElementType.NAUGHT:
                ret_val = skill_param_05;
                break;

            case MasterDataDefineLabel.ElementType.FIRE:
                ret_val = skill_param_00;
                break;

            case MasterDataDefineLabel.ElementType.WATER:
                ret_val = skill_param_01;
                break;

            case MasterDataDefineLabel.ElementType.LIGHT:
                ret_val = skill_param_03;
                break;

            case MasterDataDefineLabel.ElementType.DARK:
                ret_val = skill_param_04;
                break;

            case MasterDataDefineLabel.ElementType.WIND:
                ret_val = skill_param_02;
                break;

            case MasterDataDefineLabel.ElementType.HEAL:
                ret_val = skill_param_06;
                break;
        }

        return ret_val;
#else
		int[] table =
		{
			0,	//NONE
			skill_param_05,	//NAUGHT
			skill_param_00,	//FIRE
			skill_param_01,	//WATER
			skill_param_03,	//LIGHT
			skill_param_04,	//DARK
			skill_param_02,	//WIND
			skill_param_06,	//HEAL
		};

		return table[(int)element_type];
#endif
    }

    //PASSIVESKILL_PARAM_HANDS_HE_OVER	指定HANDS数以上でHP回復：以上を許容するか
    private bool _Get_HANDS_HE_OVER()
    {
        bool ret_val = (MasterDataDefineLabel.BoolType)skill_param_00 == MasterDataDefineLabel.BoolType.ENABLE;
        return ret_val;
    }

    //PASSIVESKILL_PARAM_HANDS_HE_COUNT	指定HANDS数以上でHP回復：指定HAND数
    private int _Get_HANDS_HE_COUNT()
    {
        return skill_param_01;
    }

    //PASSIVESKILL_PARAM_HANDS_HE_VALUE_ACTIVE	指定HANDS数以上でHP回復：固定回復か割合回復か
    private bool _Get_HANDS_HE_VALUE_ACTIVE()
    {
        bool ret_val = (MasterDataDefineLabel.BoolType)skill_param_02 == MasterDataDefineLabel.BoolType.ENABLE;
        return ret_val;
    }

    //PASSIVESKILL_PARAM_HANDS_HE_VALUE	指定HANDS数以上でHP回復：効果値
    private int _Get_HANDS_HE_VALUE()
    {
        return skill_param_03;
    }

    //PASSIVESKILL_PARAM_COUNTDOWN_SECOND	カウントダウン秒数の増減：付加秒数
    private int _Get_COUNTDOWN_SECOND()
    {
        return skill_param_00;
    }

    //PASSIVESKILL_PARAM_HANDS_RATE_VALUE	HANDS時のRate数増減：効果値（％）
    public int Get_HANDS_RATE_VALUE()
    {
        return skill_param_00;
    }

    //PASSIVESKILL_PARAM_GUARD_FUNBARI_BORDER	ふんばり：体力しきい値
    private int _Get_GUARD_FUNBARI_BORDER()
    {
        return skill_param_00;
    }

    //PASSIVESKILL_PARAM_GUARD_FULL_HP_RATE	体力最大時被ダメ軽減：軽減割合(％)
    public int Get_GUARD_FULL_HP_RATE()
    {
        return skill_param_00;
    }

    //PASSIVESKILL_PARAM_HEAL_HP_BATTLE_RATE	バトルリジェネ：割合
    public int Get_HEAL_HP_BATTLE_RATE()
    {
        return skill_param_00;
    }

    //PASSIVESKILL_PARAM_CNG_HAND_ELEM_ROOT	指定属性を指定属性の手札に変換：変換前
    public MasterDataDefineLabel.ElementType Get_CNG_HAND_ELEM_ROOT()
    {
        return (MasterDataDefineLabel.ElementType)skill_param_00;
    }

    //PASSIVESKILL_PARAM_CNG_HAND_ELEM_DEST	指定属性を指定属性の手札に変換：変換後
    public MasterDataDefineLabel.ElementType Get_CNG_HAND_ELEM_DEST()
    {
        return (MasterDataDefineLabel.ElementType)skill_param_01;
    }

    //PASSIVESKILL_PARAM_DMGUP_FLOOR_PNL_BASENUM	フロアパネルめくった枚数に応じてダメージUP：発動枚数
    public int Get_DMGUP_FLOOR_PNL_BASENUM()
    {
        return skill_param_00;
    }

    //PASSIVESKILL_PARAM_DMGUP_FLOOR_PNL_BASEVAL	フロアパネルめくった枚数に応じてダメージUP：基礎値(％)
    public int Get_DMGUP_FLOOR_PNL_BASEVAL()
    {
        return skill_param_01;
    }

    //PASSIVESKILL_PARAM_DMGUP_FLOOR_PNL_RISENUM	フロアパネルめくった枚数に応じてダメージUP：上昇枚数
    public int Get_DMGUP_FLOOR_PNL_RISENUM()
    {
        return skill_param_02;
    }

    //PASSIVESKILL_PARAM_DMGUP_FLOOR_PNL_RISEVAL	フロアパネルめくった枚数に応じてダメージUP：上昇値(％)
    public int Get_DMGUP_FLOOR_PNL_RISEVAL()
    {
        return skill_param_03;
    }

    //PASSIVESKILL_PARAM_DMGUP_ELEM_NUM_CNT	指定色以上で攻撃した際にダメージUP：色数
    public int Get_DMGUP_ELEM_NUM_CNT()
    {
        return skill_param_00;
    }

    //PASSIVESKILL_PARAM_DMGUP_ELEM_NUM_SCALE	指定色以上で攻撃した際にダメージUP：倍率
    public int Get_DMGUP_ELEM_NUM_SCALE()
    {
        return skill_param_01;
    }

    //PASSIVESKILL_PARAM_DMGUP_HANDS_NUM_CNT	指定HANDS以上で攻撃した際にダメージUP：HANDS数
    private int _Get_DMGUP_HANDS_NUM_CNT()
    {
        return skill_param_00;
    }

    //PASSIVESKILL_PARAM_DMGUP_HANDS_NUM_SCALE	指定HANDS以上で攻撃した際にダメージUP：倍率
    private int _Get_DMGUP_HANDS_NUM_SCALE()
    {
        return skill_param_01;
    }

    //PASSIVESKILL_PARAM_HEAL_HP_MOVE_RATE	移動リジェネ：割合
    public int Get_HEAL_HP_MOVE_RATE()
    {
        return skill_param_00;
    }

    //手札変換パラメータ
    public BattleFieldPanelChangeParam GetBattleFieldPanelChangeParam()
    {
        int[] param =
        {
            skill_param_00,
            skill_param_01,
            skill_param_02,
            skill_param_03,
            skill_param_04,
            skill_param_05,
            skill_param_06,
            skill_param_07,
            skill_param_08,
            skill_param_09,
            skill_param_10,
            skill_param_11,
            skill_param_12,
            skill_param_13,
            skill_param_14,
            skill_param_15,
        };

        BattleFieldPanelChangeParam ret_val = new BattleFieldPanelChangeParam();
        ret_val.init(param);

        return ret_val;
    }

    //-------------------------------------------
    // スキル効果の連結に対応した版の関数

    // 追撃効果情報
    public class FollowAttackInfo
    {
        public int m_AttackPercent; // 攻撃力倍率（％）
        public MasterDataDefineLabel.ElementType m_ElementType; // 属性
        public MasterDataDefineLabel.UIEffectType m_EffectType; // エフェクト種類
    }
    // 追撃効果の情報を取得
    public FollowAttackInfo Get_FollowAttackInfo()
    {
        MasterDataSkillPassive wrk_master = this;
        while (wrk_master != null)
        {
            if (wrk_master.skill_type == MasterDataDefineLabel.PassiveSkillCategory.FOLLOW_ATK)
            {
                FollowAttackInfo ret_val = new FollowAttackInfo();
                ret_val.m_AttackPercent = wrk_master._Get_FOLLOW_ATK_SCALE();
                ret_val.m_ElementType = wrk_master._Get_FOLLOW_ATK_ELEM();
                ret_val.m_EffectType = wrk_master._Get_FOLLOW_ATK_EFFECT();
                return ret_val;
            }

            wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
        }

        return null;
    }

    // フルライフアタック情報（体力最大時攻撃力アップ効果）
    public class FullLifeAttacUpkInfo
    {
        public int m_AttackPercent; // 攻撃力倍率（％）
    }
    // フルライフアタック情報（体力最大時攻撃力アップ効果）を取得
    public FullLifeAttacUpkInfo Get_FullLifeAttackUpInfo()
    {
        MasterDataSkillPassive wrk_master = this;
        while (wrk_master != null)
        {
            if (wrk_master._Is_skill_hp_full_powup_active())
            {
                FullLifeAttacUpkInfo ret_val = new FullLifeAttacUpkInfo();
                ret_val.m_AttackPercent = wrk_master.skill_hp_full_powup_scale;
                return ret_val;
            }

            wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
        }

        return null;
    }

    // 底力（瀕死時攻撃力アップ）情報
    public class DyingAttackUpInfo
    {
        public int m_AttackPercent; // 攻撃力倍率（％）
        public int m_HpBorder;      // 体力しきい値
    }
    // 底力（瀕死時攻撃力アップ）情報を取得
    public DyingAttackUpInfo Get_DyingAttackUpInfo()
    {
        MasterDataSkillPassive wrk_master = this;
        while (wrk_master != null)
        {
            if (wrk_master._Is_skill_dying_powup_active())
            {
                DyingAttackUpInfo ret_val = new DyingAttackUpInfo();
                ret_val.m_AttackPercent = wrk_master.skill_dying_powup_scale;
                ret_val.m_HpBorder = wrk_master.skill_dying_powup_border;
                return ret_val;
            }

            wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
        }

        return null;
    }

    // 根性（ふんばり・即死回避）情報
    public class FunbariInfo
    {
        public int m_HpBorder;      // 体力しきい値
    }
    // 根性（ふんばり・即死回避）情報を取得
    public FunbariInfo Get_FunbariInfo()
    {
        MasterDataSkillPassive wrk_master = this;
        while (wrk_master != null)
        {
            if (wrk_master.skill_type == MasterDataDefineLabel.PassiveSkillCategory.GUARD_FUNBARI)
            {
                FunbariInfo ret_val = new FunbariInfo();
                ret_val.m_HpBorder = wrk_master._Get_GUARD_FUNBARI_BORDER();
                return ret_val;
            }

            wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
        }

        return null;
    }

    // カウントダウン秒数の増減情報
    public class CountDownTimeInfo
    {
        public int m_Second;    // 増減する秒数
    }
    // カウントダウン秒数の増減情報を取得
    public CountDownTimeInfo Get_CountDownTimeInfo()
    {
        MasterDataSkillPassive wrk_master = this;
        while (wrk_master != null)
        {
            if (wrk_master.skill_type == MasterDataDefineLabel.PassiveSkillCategory.COUNTDOWN)
            {
                CountDownTimeInfo ret_val = new CountDownTimeInfo();
                ret_val.m_Second = wrk_master._Get_COUNTDOWN_SECOND();
                return ret_val;
            }

            wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
        }

        return null;
    }

    // バックアタック情報
    public class BackAttackInfo
    {
        public int m_BackAttackPercent; //バックアタック発生率（上書き）（単位は％）
    }
    // バックアタック情報を取得
    public BackAttackInfo Get_BackAttackInfo()
    {
        MasterDataSkillPassive wrk_master = this;
        while (wrk_master != null)
        {
            if (wrk_master._Is_skill_backatk_pass_active())
            {
                BackAttackInfo ret_val = new BackAttackInfo();
                ret_val.m_BackAttackPercent = wrk_master.skill_backatk_pass_rate;
                return ret_val;
            }

            wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
        }

        return null;
    }

    // 指定HANDS以上でダメージUP情報
    public class HandsAttackUpInfo
    {
        public int m_HandsCount;    // HANDS数
        public int m_AttackPercent; // 攻撃力倍率（％）
    }

    // 指定HANDS以上でダメージUP情報を取得
    public HandsAttackUpInfo Get_HandsAttackUpInfo()
    {
        MasterDataSkillPassive wrk_master = this;
        while (wrk_master != null)
        {
            if (wrk_master.skill_type == MasterDataDefineLabel.PassiveSkillCategory.DMGUP_HANDS_NUM)
            {
                HandsAttackUpInfo ret_val = new HandsAttackUpInfo();
                ret_val.m_HandsCount = wrk_master._Get_DMGUP_HANDS_NUM_CNT();
                ret_val.m_AttackPercent = wrk_master._Get_DMGUP_HANDS_NUM_SCALE();
                return ret_val;
            }

            wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
        }

        return null;
    }

    // 指定HANDSでHP回復情報
    public class HandsHealInfo
    {
        public bool m_IsOver;   // ture:指定HANDS以上の時 false:指定HANDSの時
        public int m_HandsCount;    // HANDS数
        public int m_HealValuePercent;  // 割合回復量（％）
        public int m_HealValueFix;  // 固定回復量
    }
    // 指定HANDSでHP回復情報を取得
    public HandsHealInfo Get_HandsHealInfo()
    {
        MasterDataSkillPassive wrk_master = this;
        while (wrk_master != null)
        {
            if (wrk_master.skill_type == MasterDataDefineLabel.PassiveSkillCategory.HANDS_HEAL)
            {
                HandsHealInfo ret_val = new HandsHealInfo();
                ret_val.m_IsOver = wrk_master._Get_HANDS_HE_OVER();
                ret_val.m_HandsCount = wrk_master._Get_HANDS_HE_COUNT();
                if (wrk_master._Get_HANDS_HE_VALUE_ACTIVE())
                {
                    ret_val.m_HealValuePercent = 0;
                    ret_val.m_HealValueFix = wrk_master._Get_HANDS_HE_VALUE();
                }
                else
                {
                    ret_val.m_HealValuePercent = wrk_master._Get_HANDS_HE_VALUE();
                    ret_val.m_HealValueFix = 0;
                }
                return ret_val;
            }

            wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
        }

        return null;
    }

    // 属性ダメージ軽減情報
    public class DeclineElementDamageInfo
    {
        public int m_DamagePercent;
    }
    // 属性ダメージ軽減情報を取得
    public DeclineElementDamageInfo Get_DeclineElementDamageInfo(MasterDataDefineLabel.ElementType element_type)
    {
        // 個別属性指定と全属性が同時に存在するときは個別属性指定を優先する

        {
            MasterDataSkillPassive wrk_master = this;
            while (wrk_master != null)
            {
                if (wrk_master._Is_skill_decline_dmg_elem_active()
                    && element_type == wrk_master.skill_decline_dmg_elem_elem
                )
                {
                    DeclineElementDamageInfo ret_val = new DeclineElementDamageInfo();
                    ret_val.m_DamagePercent = wrk_master.skill_decline_dmg_elem_rate;
                    return ret_val;
                }

                wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
            }
        }

        // 全属性（マスターデータに属性指定が無いものは全属性に対して効果あり）
        {
            MasterDataSkillPassive wrk_master = this;
            while (wrk_master != null)
            {
                if (wrk_master._Is_skill_decline_dmg_elem_active()
                    && MasterDataDefineLabel.ElementType.NONE == wrk_master.skill_decline_dmg_elem_elem
                )
                {
                    DeclineElementDamageInfo ret_val = new DeclineElementDamageInfo();
                    ret_val.m_DamagePercent = wrk_master.skill_decline_dmg_elem_rate;
                    return ret_val;
                }

                wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
            }
        }

        return null;
    }

    // 種族ダメージ軽減情報
    public class DeclineKindDamageInfo
    {
        public int m_DamagePercent;
    }
    // 種族ダメージ軽減情報を取得
    public DeclineKindDamageInfo Get_DeclineKindDamageInfo(MasterDataDefineLabel.KindType kind_type)
    {
        // 個別種族指定と全種族が同時に存在するときは個別種族指定を優先する

        {
            MasterDataSkillPassive wrk_master = this;
            while (wrk_master != null)
            {
                if (wrk_master._Is_skill_decline_dmg_kind_active()
                    && kind_type == wrk_master.skill_decline_dmg_kind_kind
                )
                {
                    DeclineKindDamageInfo ret_val = new DeclineKindDamageInfo();
                    ret_val.m_DamagePercent = wrk_master.skill_decline_dmg_kind_rate;
                    return ret_val;
                }

                wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
            }
        }

        // 全種族（マスターデータに種族指定が無いものは全種族に対して効果あり）
        {
            MasterDataSkillPassive wrk_master = this;
            while (wrk_master != null)
            {
                if (wrk_master._Is_skill_decline_dmg_kind_active()
                    && MasterDataDefineLabel.KindType.NONE == wrk_master.skill_decline_dmg_kind_kind
                )
                {
                    DeclineKindDamageInfo ret_val = new DeclineKindDamageInfo();
                    ret_val.m_DamagePercent = wrk_master.skill_decline_dmg_kind_rate;
                    return ret_val;
                }

                wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
            }
        }

        return null;
    }

    // 指定したタイプのオートプレイスキルを持っているかどうか
    public bool Check_AutoPlaySkillType(MasterDataDefineLabel.AutoPlaySkillType skill_type)
    {
        MasterDataSkillPassive wrk_master = this;
        while (wrk_master != null)
        {
            if (wrk_master._Get_AutoPlaySkillType() == skill_type)
            {
                return true;
            }

            wrk_master = BattleParam.m_MasterDataCache.useSkillPassive((uint)wrk_master.add_fix_id);
        }

        return false;
    }
};
