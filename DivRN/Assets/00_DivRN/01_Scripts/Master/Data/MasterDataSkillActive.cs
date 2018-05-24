using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：スキル関連：アクティブスキル
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataSkillActive : Master
{
    public uint timing_public { get; set; }              //!< 一般公開タイミング

    public string name { get; set; }                     //!< スキル情報：名前
    public string detail { get; set; }                       //!< スキル情報：詳細テキスト

    public MasterDataDefineLabel.BoolType always { get; set; }//!< スキル情報：常時発動フラグ
    public MasterDataDefineLabel.ElementType skill_element { get; set; }               //!< スキル情報：属性

    public MasterDataDefineLabel.ElementType cost1 { get; set; }                       //!< スキル情報：コスト0
    public MasterDataDefineLabel.ElementType cost2 { get; set; }                       //!< スキル情報：コスト1
    public MasterDataDefineLabel.ElementType cost3 { get; set; }                       //!< スキル情報：コスト2
    public MasterDataDefineLabel.ElementType cost4 { get; set; }                       //!< スキル情報：コスト3
    public MasterDataDefineLabel.ElementType cost5 { get; set; }                       //!< スキル情報：コスト4

    public MasterDataDefineLabel.UIEffectType effect { get; set; }                      //!< スキル情報：エフェクト番号

    public MasterDataDefineLabel.BoolType skill_active { get; set; }                //!< 汎用スキル効果：有効無効
    public MasterDataDefineLabel.SkillType skill_type { get; set; }                  //!< 汎用スキル効果：タイプ
    public int skill_value { get; set; }             //!< 汎用スキル効果：効果値
    public int skill_value_rand { get; set; }            //!< 汎用スキル効果：効果値振れ幅

    public int skill_poison_active { get; set; }     //!< 毒スキル効果：有効無効
    public int skill_poison_turn_min { get; set; }       //!< 毒スキル効果：持続ターン最小
    public int skill_poison_turn_max { get; set; }       //!< 毒スキル効果：持続ターン最大
    public int skill_poison_scale { get; set; }          //!< 毒スキル効果：効果倍率

    public int skill_coerce_active { get; set; }     //!< 威圧スキル効果：有効無効
    public int skill_coerce_turn_min { get; set; }       //!< 威圧スキル効果：付加ターン最小
    public int skill_coerce_turn_max { get; set; }       //!< 威圧スキル効果：付加ターン最大

    public int skill_guard_active { get; set; }          //!< 被ダメ軽減スキル効果：有効無効
    public int skill_guard_turn_min { get; set; }        //!< 被ダメ軽減スキル効果：持続ターン最小
    public int skill_guard_turn_max { get; set; }        //!< 被ダメ軽減スキル効果：持続ターン最大
    public int skill_guard_rate { get; set; }            //!< 被ダメ軽減スキル効果：軽減倍率

    public int skill_week_active { get; set; }           //!< 敵キャラ軟化スキル効果：有効無効
    public int skill_week_turn_min { get; set; }     //!< 敵キャラ軟化スキル効果：持続ターン最小
    public int skill_week_turn_max { get; set; }     //!< 敵キャラ軟化スキル効果：持続ターン最大
    public int skill_week_rate { get; set; }         //!< 敵キャラ軟化スキル効果：与ダメ倍率

    public int skill_change_active { get; set; }     //!< コスト変化スキル効果：有効無効
    public int skill_change_elem_prev { get; set; }      //!< コスト変化スキル効果：変化前コスト属性
    public int skill_change_elem_after { get; set; } //!< コスト変化スキル効果：変化後コスト属性

    public int skill_ct_add_active { get; set; }     //!< カウントダウン秒数増加スキル効果：有効無効
    public int skill_ct_add_second { get; set; }     //!< カウントダウン秒数増加スキル効果：付加秒数

    public int skill_drain_active { get; set; }          //!< 吸血スキル効果：有効無効
    public int skill_drain_scale { get; set; }           //!< 吸血スキル効果：攻撃倍率

    public int skill_critical_odds { get; set; }     //!< クリティカル発生率

    public string skill_boost_name { get; set; }         //!< ブーストスキル情報：表示名称
    public MasterDataDefineLabel.ElementType skill_boost_element { get; set; }     //!< ブーストスキル情報：属性
    public MasterDataDefineLabel.UIEffectType skill_boost_effect { get; set; }          //!< ブーストスキル情報：エフェクト
    public uint skill_boost_id { get; set; }             //!< ブーストスキル情報：ID

    public string skill_link_name { get; set; }          //!< リンクスキル情報：名前
    public string skill_link_detail { get; set; }            //!< リンクスキル情報：詳細テキスト
    public int skill_link_odds { get; set; }         //!< リンクスキル情報：発動率

    public MasterDataDefineLabel.ActiveAbilityType ability { get; set; }                 //!< 特性情報：特性タイプ
    public MasterDataDefineLabel.UIEffectType ability_effect { get; set; }              //!< 特性情報：置換：エフェクトタイプ
    public MasterDataDefineLabel.SkillType ability_type { get; set; }                //!< 特性情報：置換：スキルタイプ
    public int ability_value { get; set; }               //!< 特性情報：置換：スキル効果値
    public int ability_value_rand { get; set; }          //!< 特性情報：置換：スキル効果値振れ幅
    public int ability_critical { get; set; }            //!< 特性情報：置換：クリティカル発動率

    public int hate_value { get; set; } //ヘイト増減値

    public int param_00 { get; set; }                    //!< 特性情報：汎用パラメータ00
    public int param_01 { get; set; }                    //!< 特性情報：汎用パラメータ01
    public int param_02 { get; set; }                    //!< 特性情報：汎用パラメータ02
    public int param_03 { get; set; }                    //!< 特性情報：汎用パラメータ03
    public int param_04 { get; set; }                    //!< 特性情報：汎用パラメータ04
    public int param_05 { get; set; }                    //!< 特性情報：汎用パラメータ05
    public int param_06 { get; set; }                    //!< 特性情報：汎用パラメータ06
    public int param_07 { get; set; }                    //!< 特性情報：汎用パラメータ07


    public bool Is_skill_active()
    {
        bool ret_val = (skill_active == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    public string Get_skill_boost_name()
    {
        return skill_boost_name;
    }

    public string Get_skill_link_name()
    {
        return skill_link_name;
    }


    //ACTIVE_ABILITY_AILMENT_TARGET	条件：状態異常：判定対象
    public MasterDataDefineLabel.TargetType Get_AILMENT_TARGET()
    {
        return (MasterDataDefineLabel.TargetType)param_00;
    }

    //ACTIVE_ABILITY_AILMENT_GROUP_TYPE1	条件：状態異常：判定グループタイプ1
    public MasterDataDefineLabel.AilmentGroup Get_AILMENT_GROUP_TYPE1()
    {
        return (MasterDataDefineLabel.AilmentGroup)param_01;
    }

    //ACTIVE_ABILITY_HANDS_NUM_MIN	条件：HANDS数：判定HANDS数最小値
    public int Get_HANDS_NUM_MIN()
    {
        return param_00;
    }

    //ACTIVE_ABILITY_HANDS_NUM_MAX	条件：HANDS数：判定HANDS数最大値
    public int Get_HANDS_NUM_MAX()
    {
        return param_01;
    }

    //ACTIVE_ABILITY_RATE_NUM_MIN	条件：Rate数：判定Rate数最小値
    public int Get_RATE_NUM_MIN()
    {
        return param_00;
    }

    //ACTIVE_ABILITY_RATE_NUM_MAX	条件：Rate数：判定Rate数最大値
    public int Get_RATE_NUM_MAX()
    {
        return param_01;
    }

    private int[] m_CostPerElement = null;
    private bool m_IsInitCostPerElement = false;
    //属性別の必要枚数
    public int[] GetCostPerElement()
    {
        if (m_IsInitCostPerElement == false)
        {
            m_IsInitCostPerElement = true;

            m_CostPerElement = new int[(int)MasterDataDefineLabel.ElementType.MAX];
            m_CostPerElement[(int)cost1]++;
            m_CostPerElement[(int)cost2]++;
            m_CostPerElement[(int)cost3]++;
            m_CostPerElement[(int)cost4]++;
            m_CostPerElement[(int)cost5]++;
        }

        return m_CostPerElement;
    }

    private ResurrectInfo m_ResurrectInfo = null;
    /// <summary>
    /// 復活スキル情報を取得
    /// </summary>
    /// <returns></returns>
    public ResurrectInfo getResurrectInfo()
    {
        if (m_ResurrectInfo == null)
        {
            if (skill_type == MasterDataDefineLabel.SkillType.RESURRECT)
            {
                m_ResurrectInfo = new ResurrectInfo();
                m_ResurrectInfo.m_FixCount = param_00;
                m_ResurrectInfo.m_AddCount = param_01;
                m_ResurrectInfo.m_AddChancePercent = param_02;
                m_ResurrectInfo.m_HpPercent = param_03;
                m_ResurrectInfo.m_AddSpUse = param_04;

                m_ResurrectInfo.m_FixSpUse = m_ResurrectInfo.m_AddSpUse;
            }
        }

        return m_ResurrectInfo;
    }

    /// <summary>
    /// 汎用復活スキルかどうか
    /// </summary>
    /// <returns></returns>
    public bool isAlwaysResurrectSkill()
    {
        if (always == MasterDataDefineLabel.BoolType.ENABLE)
        {
            if (getResurrectInfo() != null)
            {
                return true;
            }
        }

        return false;
    }
};
