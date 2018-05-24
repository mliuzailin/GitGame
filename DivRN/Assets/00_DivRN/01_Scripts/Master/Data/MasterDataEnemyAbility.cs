using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：敵特性
	@note	敵定義に設定され、戦闘中に影響する情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataEnemyAbility : Master
{
    //public uint fix_id{get;set;}                 //!< 情報固有固定ID
    public uint timing_public { get; set; }          //!< 一般公開タイミング

    public string name { get; set; }         //!< 基本情報：名前
    public string detail { get; set; }                   //!< 基本情報：詳細テキスト
    public string icon { get; set; }                 //!< 基本情報：使用アイコンリソース名
                                                     //public int category{get;set;}                //!< 基本情報：効果カテゴリ
    public MasterDataDefineLabel.EnemyAbilityType category { get; set; }                //!< 基本情報：効果カテゴリ

    public int param_00 { get; set; }                //!< 特性情報：汎用パラメータ00
    public int param_01 { get; set; }                //!< 特性情報：汎用パラメータ01
    public int param_02 { get; set; }                //!< 特性情報：汎用パラメータ02
    public int param_03 { get; set; }                //!< 特性情報：汎用パラメータ03
    public int param_04 { get; set; }                //!< 特性情報：汎用パラメータ04
    public int param_05 { get; set; }                //!< 特性情報：汎用パラメータ05
    public int param_06 { get; set; }                //!< 特性情報：汎用パラメータ06
    public int param_07 { get; set; }                //!< 特性情報：汎用パラメータ07
    public int param_08 { get; set; }                //!< 特性情報：汎用パラメータ08
    public int param_09 { get; set; }                //!< 特性情報：汎用パラメータ09
    public int param_10 { get; set; }                //!< 特性情報：汎用パラメータ10
    public int param_11 { get; set; }                //!< 特性情報：汎用パラメータ11
    public int param_12 { get; set; }                //!< 特性情報：汎用パラメータ12
    public int param_13 { get; set; }                //!< 特性情報：汎用パラメータ13
    public int param_14 { get; set; }                //!< 特性情報：汎用パラメータ14
    public int param_15 { get; set; }                //!< 特性情報：汎用パラメータ15



    //ENEMY_ABILITY_DMG_AILMENT_TARGET	ダメ補正：状態異常：判定対象
    public MasterDataDefineLabel.TargetType Get_DMG_AILMENT_TARGET()
    {
        return (MasterDataDefineLabel.TargetType)param_00;
    }

    //ENEMY_ABILITY_DMG_AILMENT_TYPE1	ダメ補正：状態異常：判定グループタイプ1
    public MasterDataDefineLabel.AilmentGroup Get_DMG_AILMENT_TYPE1()
    {
        return (MasterDataDefineLabel.AilmentGroup)param_01;
    }

    //ENEMY_ABILITY_DMG_AILMENT_TYPE2	ダメ補正：状態異常：判定グループタイプ2
    public MasterDataDefineLabel.AilmentGroup Get_DMG_AILMENT_TYPE2()
    {
        return (MasterDataDefineLabel.AilmentGroup)param_02;
    }

    //ENEMY_ABILITY_DMG_AILMENT_TYPE3	ダメ補正：状態異常：判定グループタイプ3
    public MasterDataDefineLabel.AilmentGroup Get_DMG_AILMENT_TYPE3()
    {
        return (MasterDataDefineLabel.AilmentGroup)param_03;
    }

    //ENEMY_ABILITY_DMG_AILMENT_TYPE4	ダメ補正：状態異常：判定グループタイプ4
    public MasterDataDefineLabel.AilmentGroup Get_DMG_AILMENT_TYPE4()
    {
        return (MasterDataDefineLabel.AilmentGroup)param_04;
    }

    //ENEMY_ABILITY_DMG_AILMENT_SUBJECT	ダメ補正：状態異常：補正対象
    public MasterDataDefineLabel.TargetType Get_DMG_AILMENT_SUBJECT()
    {
        return (MasterDataDefineLabel.TargetType)param_05;
    }

    //ENEMY_ABILITY_DMG_AILMENT_ON_RATE	ダメ補正：状態異常：付与時倍率(％)
    public int Get_DMG_AILMENT_ON_RATE()
    {
        return param_06;
    }

    //ENEMY_ABILITY_DMG_AILMENT_OFF_RATE	ダメ補正：状態異常：非付与時倍率(％)
    public int Get_DMG_AILMENT_OFF_RATE()
    {
        return param_07;
    }

    //ENEMY_ABILITY_DMG_CRITICAL_ON_RATE	敵特性：ダメ補正：クリティカル：クリティカル時倍率(％)
    public int Get_DMG_CRITICAL_ON_RATE()
    {
        return param_00;
    }

    //ENEMY_ABILITY_DMG_CRITICAL_OFF_RATE	敵特性：ダメ補正：クリティカル：非クリティカル時倍率(％)
    public int Get_DMG_CRITICAL_OFF_RATE()
    {
        return param_01;
    }

    //ENEMY_ABILITY_DMG_CRITICAL_NORMAL	敵特性：ダメ補正：クリティカル：ノーマルスキル	：有効無効 v400 add
    public bool Get_DMG_CRITICAL_NORMAL()
    {
        bool ret_val = ((MasterDataDefineLabel.BoolType)param_02 == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    //ENEMY_ABILITY_DMG_CRITICAL_LINK	敵特性：ダメ補正：クリティカル：リンクスキル	：有効無効 v400 add
    public bool Get_DMG_CRITICAL_LINK()
    {
        bool ret_val = ((MasterDataDefineLabel.BoolType)param_03 == MasterDataDefineLabel.BoolType.ENABLE); ;
        return ret_val;
    }

    //ENEMY_ABILITY_DMG_NORMAL_TYPE	敵特性：ダメ補正：ノーマルスキル：判定タイプ
    public MasterDataDefineLabel.SkillType Get_DMG_NORMAL_TYPE()
    {
        return (MasterDataDefineLabel.SkillType)param_00;
    }

    //ENEMY_ABILITY_DMG_NORMAL_RATE	敵特性：ダメ補正：ノーマルスキル：倍率(％)
    public int Get_DMG_NORMAL_RATE()
    {
        return param_01;
    }

    //ENEMY_ABILITY_DMG_ACTIVE_ELEMENT	敵特性：ダメ補正：アクティブスキル：判定：属性
    public MasterDataDefineLabel.ElementType Get_DMG_ACTIVE_ELEMENT()
    {
        return (MasterDataDefineLabel.ElementType)param_00;
    }

    //ENEMY_ABILITY_DMG_ACTIVE_TYPE	敵特性：ダメ補正：アクティブスキル：判定：タイプ
    public MasterDataDefineLabel.SkillType Get_DMG_ACTIVE_TYPE()
    {
        return (MasterDataDefineLabel.SkillType)param_01;
    }

    //ENEMY_ABILITY_DMG_ACTIVE_CATEGORY	敵特性：ダメ補正：アクティブスキル：判定：カテゴリ
    public MasterDataDefineLabel.SkillCategory Get_DMG_ACTIVE_CATEGORY()
    {
        return (MasterDataDefineLabel.SkillCategory)param_02;
    }

    //ENEMY_ABILITY_DMG_ACTIVE_ATK_RATE		= 3;	//!< 敵特性：ダメ補正：アクティブスキル：判定：攻撃力倍率
    //ENEMY_ABILITY_DMG_ACTIVE_ATK_FIX		= 4;	//!< 敵特性：ダメ補正：アクティブスキル：判定：固定値
    //ENEMY_ABILITY_DMG_ACTIVE_ATK_HP			= 5;	//!< 敵特性：ダメ補正：アクティブスキル：判定：対象HP割合
    //ENEMY_ABILITY_DMG_ACTIVE_ABSORB			= 6;	//!< 敵特性：ダメ補正：アクティブスキル：判定：吸血

    //ENEMY_ABILITY_DMG_ACTIVE_AILMENT	敵特性：ダメ補正：アクティブスキル：判定：状態グループ
    public MasterDataDefineLabel.AilmentGroup Get_DMG_ACTIVE_AILMENT()
    {
        return (MasterDataDefineLabel.AilmentGroup)param_07;
    }

    //ENEMY_ABILITY_DMG_ACTIVE_RATE	敵特性：ダメ補正：アクティブスキル：倍率(％)
    public int Get_DMG_ACTIVE_RATE()
    {
        return param_08;
    }

    //ENEMY_ABILITY_DMG_BOOST_RATE	敵特性：ダメ補正：ブーストスキル：倍率(％)
    public int Get_DMG_BOOST_RATE()
    {
        return param_00;
    }

    //ENEMY_ABILITY_DMG_COST_NUM	敵特性：ダメ補正：指定コスト：コスト数
    public int Get_DMG_COST_NUM()
    {
        return param_00;
    }

    //ENEMY_ABILITY_DMG_COST_OVER_RATE	敵特性：ダメ補正：指定コスト：指定コスト以上倍率(％)
    public int Get_DMG_COST_OVER_RATE()
    {
        return param_01;
    }

    //ENEMY_ABILITY_DMG_COST_LESS_RATE	敵特性：ダメ補正：指定コスト：指定コスト以下倍率(％)
    public int Get_DMG_COST_LESS_RATE()
    {
        return param_02;
    }

    //ENEMY_ABILITY_DMG_COLOR_NUM	敵特性：ダメ補正：指定色：色数
    public int Get_DMG_COLOR_NUM()
    {
        return param_00;
    }

    //ENEMY_ABILITY_DMG_COLOR_OVER_RATE	敵特性：ダメ補正：指定色：指定色以上倍率(％)
    public int Get_DMG_COLOR_OVER_RATE()
    {
        return param_01;
    }

    //ENEMY_ABILITY_DMG_COLOR_LESS_RATE	敵特性：ダメ補正：指定色：指定色以下倍率(％)
    public int Get_DMG_COLOR_LESS_RATE()
    {
        return param_02;
    }

    //ENEMY_ABILITY_DMG_HANDS_NUM	敵特性：ダメ補正：指定HANDS：HANDS数
    public int Get_DMG_HANDS_NUM()
    {
        return param_00;
    }

    //ENEMY_ABILITY_DMG_HANDS_OVER_RATE	敵特性：ダメ補正：指定HANDS：HANDS以上倍率(％)
    public int Get_DMG_HANDS_OVER_RATE()
    {
        return param_01;
    }

    //ENEMY_ABILITY_DMG_HANDS_LESS_RATE	敵特性：ダメ補正：指定HANDS：HANDS以下倍率(％)
    public int Get_DMG_HANDS_LESS_RATE()
    {
        return param_02;
    }

    //ENEMY_ABILITY_DMG_RATE_VALUE	敵特性：ダメ補正：指定Rate：Rate値
    public int Get_DMG_RATE_VALUE()
    {
        return param_00;
    }

    //ENEMY_ABILITY_DMG_RATE_OVER_RATE	敵特性：ダメ補正：指定Rate：Rate以上倍率(％)
    public int Get_DMG_RATE_OVER_RATE()
    {
        return param_01;
    }

    //ENEMY_ABILITY_DMG_RATE_LESS_RATE	敵特性：ダメ補正：指定Rate：Rate以下倍率(％)
    public int Get_DMG_RATE_LESS_RATE()
    {
        return param_02;
    }

    //ENEMY_ABILITY_SPECIFIC_ACTION_ELEMENT	敵特性：特定アクション：判定：属性
    public MasterDataDefineLabel.ElementType Get_SPECIFIC_ACTION_ELEMENT()
    {
        return (MasterDataDefineLabel.ElementType)param_00;
    }

    //ENEMY_ABILITY_SPECIFIC_ACTION_TYPE	敵特性：特定アクション：判定：タイプ
    public MasterDataDefineLabel.SkillType Get_SPECIFIC_ACTION_TYPE()
    {
        return (MasterDataDefineLabel.SkillType)param_01;
    }

    //ENEMY_ABILITY_SPECIFIC_ACTION_CATEGORY	敵特性：特定アクション：判定：カテゴリ
    public MasterDataDefineLabel.SkillCategory Get_SPECIFIC_ACTION_CATEGORY()
    {
        return (MasterDataDefineLabel.SkillCategory)param_02;
    }

    //ENEMY_ABILITY_SPECIFIC_ACTION_ATK_RATE	敵特性：特定アクション：判定：攻撃力倍率
    public bool Get_SPECIFIC_ACTION_ATK_RATE()
    {
        bool ret_val = ((MasterDataDefineLabel.BoolType)param_03 == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    //ENEMY_ABILITY_SPECIFIC_ACTION_ATK_FIX	敵特性：特定アクション：判定：固定値
    public bool Get_SPECIFIC_ACTION_ATK_FIX()
    {
        bool ret_val = ((MasterDataDefineLabel.BoolType)param_04 == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    //ENEMY_ABILITY_SPECIFIC_ACTION_ATK_HP	敵特性：特定アクション：判定：対象HP割合
    public bool Get_SPECIFIC_ACTION_ATK_HP()
    {
        bool ret_val = ((MasterDataDefineLabel.BoolType)param_05 == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    //ENEMY_ABILITY_SPECIFIC_ACTION_ABSORB	敵特性：特定アクション：判定：吸血
    public bool Get_SPECIFIC_ACTION_ABSORB()
    {
        bool ret_val = ((MasterDataDefineLabel.BoolType)param_06 == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    //ENEMY_ABILITY_SPECIFIC_ACTION_AILMENT	敵特性：特定アクション：判定：状態グループ
    public MasterDataDefineLabel.AilmentGroup Get_SPECIFIC_ACTION_AILMENT()
    {
        return (MasterDataDefineLabel.AilmentGroup)param_07;
    }

    //ENEMY_ABILITY_SPECIFIC_ACTION_INVOKE	敵特性：特定アクション：発動行動
    public int Get_SPECIFIC_ACTION_INVOKE()
    {
        return param_08;
    }

    //ENEMY_ABILITY_INVALID_TYPE1	敵特性：無効化：無効タイプ1
    public MasterDataDefineLabel.SkillCategory Get_INVALID_TYPE1()
    {
        return (MasterDataDefineLabel.SkillCategory)param_00;
    }

    //ENEMY_ABILITY_INVALID_TYPE2	敵特性：無効化：無効タイプ2
    public MasterDataDefineLabel.SkillCategory Get_INVALID_TYPE2()
    {
        return (MasterDataDefineLabel.SkillCategory)param_01;
    }

    //ENEMY_ABILITY_INVALID_TYPE3	敵特性：無効化：無効タイプ3
    public MasterDataDefineLabel.SkillCategory Get_INVALID_TYPE3()
    {
        return (MasterDataDefineLabel.SkillCategory)param_02;
    }

    //ENEMY_ABILITY_INVALID_TYPE4	敵特性：無効化：無効タイプ4
    public MasterDataDefineLabel.SkillCategory Get_INVALID_TYPE4()
    {
        return (MasterDataDefineLabel.SkillCategory)param_03;
    }

    //ENEMY_ABILITY_DMG_SKILL_RATE	敵特性：ダメ補正：指定スキル：倍率(％)
    public int Get_DMG_SKILL_RATE()
    {
        return param_00;
    }

    //ENEMY_ABILITY_DMG_SKILL_TARGET	敵特性：ダメ補正：指定スキル：補正適用可能スキルフラグ群
    public uint Get_DMG_SKILL_TARGET()
    {
        return (uint)param_01;
    }

    // MasterDataDefineLabel.EnemyAbilityType.KONJO で根性が発動するHP%（これ以上HPがあるときに発動）
    public int Get_KONJO_TRIGER_HP_RATE()
    {
        return param_00;
    }

    // MasterDataDefineLabel.EnemyAbilityType.KONJO で根性が発動した際に残るHP%
    public int Get_KONJO_KEEP_HP_RATE()
    {
        return param_01;
    }
};
