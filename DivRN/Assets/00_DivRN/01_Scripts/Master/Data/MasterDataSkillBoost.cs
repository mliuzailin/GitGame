using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：スキル関連：ブーストスキル
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataSkillBoost : Master
{
    public uint timing_public { get; set; }              //!< 一般公開タイミング

    public MasterDataDefineLabel.SkillType skill_type { get; set; }                  //!< 基本情報：スキルタイプ
    public MasterDataDefineLabel.BoostSkillCategory skill_cate { get; set; }                  //!< 基本情報：効果カテゴリ

    public MasterDataDefineLabel.BoolType skill_damage_enable { get; set; }     //!< スキル情報：ダメージ有効無効
    public int skill_power { get; set; }             //!< スキル情報：効果値(％)
    public int skill_power_fix { get; set; }         //!< スキル情報：効果値(固定)
    public int skill_power_hp_rate { get; set; }     //!< スキル情報：効果値(対象HPの割合)
    public int skill_absorb { get; set; }                //!< スキル情報：吸収量(％)

    public MasterDataDefineLabel.BoolType skill_chk_atk_affinity { get; set; }      //!< 効果情報：攻撃側：属性相性チェック
    public MasterDataDefineLabel.BoolType skill_chk_atk_leader { get; set; }        //!< 効果情報：攻撃側：リーダースキルチェック
    public MasterDataDefineLabel.BoolType skill_chk_atk_passive { get; set; }       //!< 効果情報：攻撃側：パッシブスキルチェック
    public MasterDataDefineLabel.BoolType skill_chk_atk_ailment { get; set; }       //!< 効果情報：攻撃側：状態変化チェック

    public MasterDataDefineLabel.BoolType skill_chk_def_defence { get; set; }       //!< 効果情報：防御側：防御無視チェック
    public MasterDataDefineLabel.BoolType skill_chk_def_ailment { get; set; }       //!< 効果情報：防御側：状態変化チェック
    public MasterDataDefineLabel.BoolType skill_chk_def_barrier { get; set; }       //!< 効果情報：防御側：状態バリアチェック

    public MasterDataDefineLabel.TargetType status_ailment_target { get; set; }       //!< 状態変化対象
#if true // @Change Developer 2015/08/21 v300状態異常の遅延発動対応。
    public int status_ailment_delay { get; set; }        //!< 状態変化遅延。
#endif
    public int status_ailment1 { get; set; }         //!< 状態変化ID1
    public int status_ailment2 { get; set; }         //!< 状態変化ID2
    public int status_ailment3 { get; set; }         //!< 状態変化ID3
    public int status_ailment4 { get; set; }         //!< 状態変化ID4

    public int hate_value { get; set; } //ヘイト増減値

    public int skill_param_00 { get; set; }              //!< スキル情報：汎用パラメータ00
    public int skill_param_01 { get; set; }              //!< スキル情報：汎用パラメータ01
    public int skill_param_02 { get; set; }              //!< スキル情報：汎用パラメータ02
    public int skill_param_03 { get; set; }              //!< スキル情報：汎用パラメータ03
    public int skill_param_04 { get; set; }              //!< スキル情報：汎用パラメータ04
    public int skill_param_05 { get; set; }              //!< スキル情報：汎用パラメータ05
    public int skill_param_06 { get; set; }              //!< スキル情報：汎用パラメータ06
    public int skill_param_07 { get; set; }              //!< スキル情報：汎用パラメータ07
    public int skill_param_08 { get; set; }              //!< スキル情報：汎用パラメータ08
    public int skill_param_09 { get; set; }              //!< スキル情報：汎用パラメータ09
    public int skill_param_10 { get; set; }              //!< スキル情報：汎用パラメータ10
    public int skill_param_11 { get; set; }              //!< スキル情報：汎用パラメータ11
    public int skill_param_12 { get; set; }              //!< スキル情報：汎用パラメータ12
    public int skill_param_13 { get; set; }              //!< スキル情報：汎用パラメータ13
    public int skill_param_14 { get; set; }              //!< スキル情報：汎用パラメータ14
    public int skill_param_15 { get; set; }              //!< スキル情報：汎用パラメータ15


    public bool Is_skill_damage_enable()
    {
        bool ret_val = (skill_damage_enable == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }


    //BOOSTSKILL_HEAL_SP_VALUE	SP回復：効果値
    public int Get_HEAL_SP_VALUE()
    {
        return skill_param_00;
    }

    //BOOSTSKILL_HEAL_HP_RATE_VALUE	HP回復(割合)：効果倍率(%)
    public int Get_HEAL_HP_RATE_VALUE()
    {
        return skill_param_00;
    }

    //BOOSTSKILL_HEAL_HP_FIX_VALUE	HP回復(固定値)：効果値
    public int Get_HEAL_HP_FIX_VALUE()
    {
        return skill_param_00;
    }

    //BOOSTSKILL_HAND_CNG_PANEL_LEFT	手札変換：パネル指定：左　 →　変換先
    public MasterDataDefineLabel.ElementType[] Get_HAND_CNG_PANEL()
    {
        MasterDataDefineLabel.ElementType[] ret_val =
        {
            (MasterDataDefineLabel.ElementType)skill_param_00,
            (MasterDataDefineLabel.ElementType)skill_param_01,
            (MasterDataDefineLabel.ElementType)skill_param_02,
            (MasterDataDefineLabel.ElementType)skill_param_03,
            (MasterDataDefineLabel.ElementType)skill_param_04,
        };
        return ret_val;
    }

    //BOOSTSKILL_HAND_CNG_ELEM_FIRE	手札変換：属性指定：炎　 →　変換先
    public MasterDataDefineLabel.ElementType[] Get_HAND_CNG_ELEM()
    {
        MasterDataDefineLabel.ElementType[] ret_val =
        {
            MasterDataDefineLabel.ElementType.NONE,							// -	→	ランダム
			(MasterDataDefineLabel.ElementType)skill_param_05,	// 無 　→　変換先
			(MasterDataDefineLabel.ElementType)skill_param_00,	// 炎　 →　変換先
			(MasterDataDefineLabel.ElementType)skill_param_01,	// 水　 →　変換先
			(MasterDataDefineLabel.ElementType)skill_param_03,	// 光　 →　変換先
			(MasterDataDefineLabel.ElementType)skill_param_04,	// 闇　 →　変換先
			(MasterDataDefineLabel.ElementType)skill_param_02,	// 風　 →　変換先
			(MasterDataDefineLabel.ElementType)skill_param_06, 	// 回復 →　変換先
		};
        return ret_val;
    }

    //BOOSTSKILL_LBS_TURN_REDUCE_TARGET	LBS必要ターン数を短縮：対象
    public MasterDataDefineLabel.TargetType Get_LBS_TURN_REDUCE_TARGET()
    {
        return (MasterDataDefineLabel.TargetType)skill_param_00;
    }

    //BOOSTSKILL_LBS_TURN_REDUCE_VALUE	LBS必要ターン数を短縮：効果値
    public int Get_LBS_TURN_REDUCE_VALUE()
    {
        return skill_param_01;
    }

    //手札変換パラメータ
    public BattleFieldPanelChangeParam GetBattleFieldPanelChangeParam()
    {
        int[] param =
        {
            (int)MasterDataDefineLabel.BoolType.NONE,
            100,
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
        };

        BattleFieldPanelChangeParam ret_val = new BattleFieldPanelChangeParam();
        ret_val.init(param);

        return ret_val;
    }

    /// <summary>
    /// 復活スキル情報を取得
    /// </summary>
    /// <returns></returns>
    public ResurrectInfo getResurrectInfo()
    {
        ResurrectInfo ret_val = null;
        if (skill_type == MasterDataDefineLabel.SkillType.RESURRECT)
        {
            ret_val = new ResurrectInfo();
            ret_val.m_FixCount = skill_param_00;
            ret_val.m_AddCount = skill_param_01;
            ret_val.m_AddChancePercent = skill_param_02;
            ret_val.m_HpPercent = skill_param_03;
            ret_val.m_AddSpUse = skill_param_04;

            ret_val.m_FixSpUse = ret_val.m_AddSpUse;
        }

        return ret_val;
    }
};
