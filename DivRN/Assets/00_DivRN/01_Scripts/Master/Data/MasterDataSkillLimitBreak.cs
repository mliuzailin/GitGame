using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：スキル関連：リミットブレイクスキル
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataSkillLimitBreak : Master
{
    public uint timing_public { get; set; }                  //!< 一般公開タイミング

    public string name { get; set; }                     //!< スキル情報：名前
    public string detail { get; set; }                       //!< スキル情報：詳細テキスト

    public int add_fix_id { get; set; }                    //!< 追加リミブレfix_id

    public int use_turn { get; set; }                    //!< 発動対価：ターン数
    public int use_sp { get; set; }                      //!< 発動対価：SP

    public int level_max { get; set; }                   //!< スキルレベル：最大
    public int level_up_rate { get; set; }               //!< スキルレベル：レベルアップ確率

    public MasterDataDefineLabel.LBSkillPhase phase { get; set; }                       //!< 発動可能フェーズ


    public int subject_type { get; set; }                //!< 発動可能条件タイプ
    public int subject_value { get; set; }               //!< 発動可能条件値

    public MasterDataDefineLabel.ElementType skill_elem { get; set; }                  //!< スキル共通情報：属性
    public MasterDataDefineLabel.SkillType skill_type { get; set; }                  //!< スキル共通情報：タイプ
    public MasterDataDefineLabel.SkillCategory skill_cate { get; set; }//!< スキル共通情報：効果タイプ
    public MasterDataDefineLabel.UIEffectType skill_effect { get; set; }                //!< スキル共通情報：エフェクト

    public MasterDataDefineLabel.BoolType skill_damage_enable { get; set; }     //!< スキルダメージ有効無効
    public int skill_power { get; set; }             //!< スキル攻撃力(%)																NEW v2000
    public int skill_power_fix { get; set; }         //!< スキル攻撃力(固定)																NEW v2000
    public int skill_power_hp_rate { get; set; }     //!< スキル攻撃力(対象のHPの割合)													NEW v2000
    public int skill_absorb { get; set; }                //!< スキル吸収力(%)																NEW v2000
    public int skill_kickback { get; set; }              //!< スキル反動量(%)																NEW v2000
    public int skill_kickback_fix { get; set; }          //!< スキル反動量(固定)																NEW v2000

    public MasterDataDefineLabel.BoolType skill_chk_atk_affinity { get; set; }      //!< スキル攻撃側：属性相性チェック														NEW v2000
    public MasterDataDefineLabel.BoolType skill_chk_atk_leader { get; set; }        //!< スキル攻撃側：リーダーチェック														NEW v2000
    public MasterDataDefineLabel.BoolType skill_chk_atk_passive { get; set; }       //!< スキル攻撃側：パッシブチェック														NEW v2000
    public MasterDataDefineLabel.BoolType skill_chk_atk_ailment { get; set; }       //!< スキル攻撃側：状態異常チェック														NEW v2000

    public MasterDataDefineLabel.BoolType skill_chk_def_defence { get; set; }       //!< スキル防御チェック																NEW v2000
    public MasterDataDefineLabel.BoolType skill_chk_def_ailment { get; set; }       //!< スキル状態異常チェック															NEW v2000
    public MasterDataDefineLabel.BoolType skill_chk_def_barrier { get; set; }       //!< スキル状態異常バリアチェック													NEW v3400

    public MasterDataDefineLabel.TargetType status_ailment_target { get; set; }       //!< スキル効果対象						（MasterDataDefineLabel.TARGET_NONE）		NEW v2000
    public int status_ailment1 { get; set; }         //!< 状態変化ID１							（StatusAilmentParam.fix_id）				NEW v2000
    public int status_ailment2 { get; set; }         //!< 状態変化ID２							（StatusAilmentParam.fix_id）				NEW v2000
    public int status_ailment3 { get; set; }         //!< 状態変化ID３							（StatusAilmentParam.fix_id）				NEW v2000
    public int status_ailment4 { get; set; }         //!< 状態変化ID４							（StatusAilmentParam.fix_id）				NEW v2000

    public int hate_value { get; set; } //ヘイト増減値

    public int value0 { get; set; }                      //!< スキル汎用領域：
    public int value1 { get; set; }                      //!< スキル汎用領域：
    public int value2 { get; set; }                      //!< スキル汎用領域：
    public int value3 { get; set; }                      //!< スキル汎用領域：
    public int value4 { get; set; }                      //!< スキル汎用領域：
    public int value5 { get; set; }                      //!< スキル汎用領域：
    public int value6 { get; set; }                      //!< スキル汎用領域：
    public int value7 { get; set; }                      //!< スキル汎用領域：
    public int value8 { get; set; }                      //!< スキル汎用領域：
    public int value9 { get; set; }                      //!< スキル汎用領域：
    public int value10 { get; set; }                 //!< スキル汎用領域：
    public int value11 { get; set; }                 //!< スキル汎用領域：
    public int value12 { get; set; }                 //!< スキル汎用領域：
    public int value13 { get; set; }                 //!< スキル汎用領域：
    public int value14 { get; set; }                 //!< スキル汎用領域：
    public int value15 { get; set; }                 //!< スキル汎用領域：


    //-------------------------
    // データコンバート用領域
    //
    // マスターデータは配列として持ってこれないので、
    // インポートの後にこの領域に変数をまとめて配列化する
    //-------------------------
    //	public int value_turn_min{get;set;}              //!< スキル情報コンバート領域：スキル効果ターン：最低
    //	public int value_turn_max{get;set;}              //!< スキル情報コンバート領域：スキル効果ターン：最大

    /// <summary>
    /// コピー
    /// </summary>
    /// <param name="cSrc"></param>
    public void Copy(MasterDataSkillLimitBreak src)
    {
        timing_public = src.timing_public;
        name = src.name;
        detail = src.detail;
        add_fix_id = src.add_fix_id;
        use_turn = src.use_turn;
        use_sp = src.use_sp;
        level_max = src.level_max;
        level_up_rate = src.level_up_rate;
        phase = src.phase;
        subject_type = src.subject_type;
        subject_value = src.subject_value;
        skill_elem = src.skill_elem;
        skill_type = src.skill_type;
        skill_cate = src.skill_cate;
        skill_effect = src.skill_effect;
        skill_damage_enable = src.skill_damage_enable;
        skill_power = src.skill_power;
        skill_power_fix = src.skill_power_fix;
        skill_power_hp_rate = src.skill_power_hp_rate;
        skill_absorb = src.skill_absorb;
        skill_kickback = src.skill_kickback;
        skill_kickback_fix = src.skill_kickback_fix;
        skill_chk_atk_affinity = src.skill_chk_atk_affinity;
        skill_chk_atk_leader = src.skill_chk_atk_leader;
        skill_chk_atk_passive = src.skill_chk_atk_passive;
        skill_chk_atk_ailment = src.skill_chk_atk_ailment;
        skill_chk_def_defence = src.skill_chk_def_defence;
        skill_chk_def_ailment = src.skill_chk_def_ailment;
        skill_chk_def_barrier = src.skill_chk_def_barrier;
        status_ailment_target = src.status_ailment_target;
        status_ailment1 = src.status_ailment1;
        status_ailment2 = src.status_ailment2;
        status_ailment3 = src.status_ailment3;
        status_ailment4 = src.status_ailment4;
        hate_value = src.hate_value;
        value0 = src.value0;
        value1 = src.value1;
        value2 = src.value2;
        value3 = src.value3;
        value4 = src.value4;
        value5 = src.value5;
        value6 = src.value6;
        value7 = src.value7;
        value8 = src.value8;
        value9 = src.value9;
        value10 = src.value10;
        value11 = src.value11;
        value12 = src.value12;
        value13 = src.value13;
        value14 = src.value14;
        value15 = src.value15;
    }


    public bool Is_skill_damage_enable()
    {
        bool ret_val = (skill_damage_enable == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }



    //SKILLPARAM_COST_CHANGE_PREV	コスト変換（属性変換）：変化前コスト属性		[ ELEMENT_NONE ～ ELEMENT_MAX ]
    public MasterDataDefineLabel.ElementType Get_COST_CHANGE_PREV()
    {
        return (MasterDataDefineLabel.ElementType)value0;
    }

    //SKILLPARAM_COST_CHANGE_AFTER	コスト変換（属性変換）：変化後コスト属性		[ ELEMENT_NONE ～ ELEMENT_MAX ]
    public MasterDataDefineLabel.ElementType Get_COST_CHANGE_AFTER()
    {
        return (MasterDataDefineLabel.ElementType)value1;
    }

    //SKILLPARAM_FIX_DMG_ELEM_T_ELEM	固定ダメージ（対象属性限定）：対象属性			[ ELEMENT_NONE ～ ELEMENT_MAX ]
    public MasterDataDefineLabel.ElementType Get_FIX_DMG_ELEM_T_ELEM()
    {
        return (MasterDataDefineLabel.ElementType)value1;
    }

    //SKILLPARAM_ATK_ELEM_TARGET_ELEM	攻撃（対象属性限定）：属性
    public MasterDataDefineLabel.ElementType Get_ATK_ELEM_TARGET_ELEM()
    {
        return (MasterDataDefineLabel.ElementType)value0;
    }

    //SKILLPARAM_SUPPORT_SPESTATE_CLEAR_TARGET	指定都合状態異常全クリア：対象
    public MasterDataDefineLabel.TargetType Get_SUPPORT_SPESTATE_CLEAR_TARGET()
    {
        return (MasterDataDefineLabel.TargetType)value0;
    }

    //SKILLPARAM_SUPPORT_SPESTATE_CLEAR_GOOD_BAD	指定都合状態異常全クリア：良い悪い
    public StatusAilment.GoodOrBad Get_SUPPORT_SPESTATE_CLEAR_GOOD_BAD()
    {
        return (StatusAilment.GoodOrBad)value1;
    }

    //SKILLPARAM_ABSTATE_POISON_TURN_MIN	状態異常（毒）：持続ターン最小					[ 0 ～ ]
    public int Get_ABSTATE_POISON_TURN_MIN()
    {
        _debug_IsUsingField(value0);
        return value0;
    }

    //SKILLPARAM_ABSTATE_POISON_TURN_MAX	状態異常（毒）：持続ターン最大					[ 0 ～ ]
    public int Get_ABSTATE_POISON_TURN_MAX()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_ABSTATE_POISON_RATE	状態異常（毒）：効果倍率						[ 0 ～ ]
    public int Get_ABSTATE_POISON_RATE()
    {
        _debug_IsUsingField(value2);
        return value2;
    }

    //SKILLPARAM_ABSTATE_GB_TURN_MIN	状態異常（ガードブレイク）：持続ターン最小		[ 0 ～ ]
    public int Get_ABSTATE_GB_TURN_MIN()
    {
        _debug_IsUsingField(value0);
        return value0;
    }

    //SKILLPARAM_ABSTATE_GB_TURN_MAX	状態異常（ガードブレイク）：持続ターン最大		[ 0 ～ ]
    public int Get_ABSTATE_GB_TURN_MAX()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_ABSTATE_GB_RATE	状態異常（ガードブレイク）：防御倍率			[ 0 ～ ]
    public int Get_ABSTATE_GB_RATE()
    {
        _debug_IsUsingField(value2);
        return value2;
    }

    //SKILLPARAM_ABSTATE_LATE_TURN_MIN	状態異常（ターンカウント遅延）：付加ターン最小	[ 0 ～ ]
    public int Get_ABSTATE_LATE_TURN_MIN()
    {
        _debug_IsUsingField(value0);
        return value0;
    }

    //SKILLPARAM_ABSTATE_LATE_TURN_MAX	状態異常（ターンカウント遅延）：付加ターン最大	[ 0 ～ ]
    public int Get_ABSTATE_LATE_TURN_MAX()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_SUPPORT_GUARD_TURN_MIN	補助（ダメージ軽減）：持続ターン最小			[ 0 ～ ]
    public int Get_SUPPORT_GUARD_TURN_MIN()
    {
        _debug_IsUsingField(value0);
        return value0;
    }

    //SKILLPARAM_SUPPORT_GUARD_TURN_MAX	補助（ダメージ軽減）：持続ターン最大			[ 0 ～ ]
    public int Get_SUPPORT_GUARD_TURN_MAX()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_SUPPORT_GUARD_RATE	補助（ダメージ軽減）：軽減倍率（％）			[ 0 ～ ]
    public int Get_SUPPORT_GUARD_RATE()
    {
        _debug_IsUsingField(value2);
        return value2;
    }

    //SKILLPARAM_SUPPORT_POWUP_TURN_MIN	補助（パワーアップ）：持続ターン最小			[ 0 ～ ]
    public int Get_SUPPORT_POWUP_TURN_MIN()
    {
        _debug_IsUsingField(value0);
        return value0;
    }

    //SKILLPARAM_SUPPORT_POWUP_TURN_MAX	補助（パワーアップ）：持続ターン最大			[ 0 ～ ]
    public int Get_SUPPORT_POWUP_TURN_MAX()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_SUPPORT_POWUP_RATE	補助（パワーアップ）：パワーアップ倍率（％）	[ 0 ～ ]
    public int Get_SUPPORT_POWUP_RATE()
    {
        _debug_IsUsingField(value2);
        return value2;
    }

    //SKILLPARAM_SUPPORT_ELEMUP_TURN_MIN	補助（属性パワーアップ）：持続ターン最小		[ 0 ～ ]
    public int Get_SUPPORT_ELEMUP_TURN_MIN()
    {
        _debug_IsUsingField(value0);
        return value0;
    }

    //SKILLPARAM_SUPPORT_ELEMUP_TURN_MAX	補助（属性パワーアップ）：持続ターン最大		[ 0 ～ ]
    public int Get_SUPPORT_ELEMUP_TURN_MAX()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_SUPPORT_ELEMUP_ELEM	補助（属性パワーアップ）：属性					[ ELEMENT_NONE ～ ELEMENT_MAX ]
    public MasterDataDefineLabel.ElementType Get_SUPPORT_ELEMUP_ELEM()
    {
        return (MasterDataDefineLabel.ElementType)value2;
    }

    //SKILLPARAM_SUPPORT_ELEMUP_RATE	補助（属性パワーアップ）：パワーアップ倍率(％)	[ 0 ～ ]
    public int Get_SUPPORT_ELEMUP_RATE()
    {
        _debug_IsUsingField(value3);
        return value3;
    }

    //SKILLPARAM_SUPPORT_CTUP_TURN_MIN	補助（カウントダウン秒数増加）：持続ターン最小	[ 0 ～ ]
    public int Get_SUPPORT_CTUP_TURN_MIN()
    {
        _debug_IsUsingField(value0);
        return value0;
    }

    //SKILLPARAM_SUPPORT_CTUP_TURN_MAX	補助（カウントダウン秒数増加）：持続ターン最大	[ 0 ～ ]
    public int Get_SUPPORT_CTUP_TURN_MAX()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_SUPPORT_CTUP_SECOND	補助（カウントダウン秒数増加）：付加秒数		[ 0 ～ ]
    public int Get_SUPPORT_CTUP_SECOND()
    {
        _debug_IsUsingField(value2);
        return value2;
    }

    //SKILLPARAM_SUPPORT_INV_TURN_MIN	補助（属性攻撃無効化）：持続ターン最小			[ 0 ～ ]
    public int Get_SUPPORT_INV_TURN_MIN()
    {
        _debug_IsUsingField(value0);
        return value0;
    }

    //SKILLPARAM_SUPPORT_INV_TURN_MAX	補助（属性攻撃無効化）：持続ターン最大			[ 0 ～ ]
    public int Get_SUPPORT_INV_TURN_MAX()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_SUPPORT_INV_ELEM	補助（属性攻撃無効化）：属性					[ ELEMENT_NONE ～ ELEMENT_MAX ]
    public int Get_SUPPORT_INV_ELEM()
    {
        _debug_IsUsingField(value2);
        return value2;
    }

    //SKILLPARAM_SUPPORT_RETATK_TURN_MIN	補助（カウンター）：持続ターン最小				[ 0 ～ ]
    public int Get_SUPPORT_RETATK_TURN_MIN()
    {
        _debug_IsUsingField(value0);
        return value0;
    }

    //SKILLPARAM_SUPPORT_RETATK_TURN_MAX	補助（カウンター）：持続ターン最大				[ 0 ～ ]
    public int Get_SUPPORT_RETATK_TURN_MAX()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_SUPPORT_RETATK_ELEM	補助（カウンター）：属性						[ ELEMENT_NONE ～ ELEMENT_MAX ]
    public int Get_SUPPORT_RETATK_ELEM()
    {
        _debug_IsUsingField(value2);
        return value2;
    }

    //SKILLPARAM_SUPPORT_RETATK_RATE	補助（カウンター）：攻撃倍率(％)				[ 0 ～ ]
    public int Get_SUPPORT_RETATK_RATE()
    {
        _debug_IsUsingField(value3);
        return value3;
    }

    //SKILLPARAM_SUPPORT_C_CNG_TURN_MIN	補助（継続コスト変換）：持続ターン最小			[ 0 ～ ]
    public int Get_SUPPORT_C_CNG_TURN_MIN()
    {
        _debug_IsUsingField(value0);
        return value0;
    }

    //SKILLPARAM_SUPPORT_C_CNG_TURN_MAX	補助（継続コスト変換）：持続ターン最大			[ 0 ～ ]
    public int Get_SUPPORT_C_CNG_TURN_MAX()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_SUPPORT_C_CNG_PREV	補助（継続コスト変換）：変化前コスト属性		[ ELEMENT_NONE ～ ELEMENT_MAX ]
    public int Get_SUPPORT_C_CNG_PREV()
    {
        _debug_IsUsingField(value2);
        return value2;
    }

    //SKILLPARAM_SUPPORT_C_CNG_AFTER	補助（継続コスト変換）：変化後コスト属性		[ ELEMENT_NONE ～ ELEMENT_MAX ]
    public int Get_SUPPORT_C_CNG_AFTER()
    {
        _debug_IsUsingField(value2);
        return value2;
    }

    //SKILLPARAM_SUPPORT_C_FIX_HANDCARD	補助（コスト固定化）：手札変化有無			[ xxxx ]
    public bool Get_SUPPORT_C_FIX_HANDCARD()
    {
        _debug_IsUsingField(value0);
        bool ret_val = (MasterDataDefineLabel.BoolType)value0 == MasterDataDefineLabel.BoolType.ENABLE;
        return ret_val;
    }

    //SKILLPARAM_SUPPORT_C_FIX_TURN_MIN	補助（コスト固定化）：持続ターン最小			[ 0 ～ ]
    public int Get_SUPPORT_C_FIX_TURN_MIN()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_SUPPORT_C_FIX_TURN_MAX	補助（コスト固定化）：持続ターン最大			[ 0 ～ ]
    public int Get_SUPPORT_C_FIX_TURN_MAX()
    {
        _debug_IsUsingField(value2);
        return value2;
    }

    //SKILLPARAM_SUPPORT_C_FIX_ELEM	補助（コスト固定化）：コスト属性				[ ELEMENT_NONE ～ ELEMENT_MAX ]
    public MasterDataDefineLabel.ElementType Get_SUPPORT_C_FIX_ELEM()
    {
        _debug_IsUsingField(value3);
        return (MasterDataDefineLabel.ElementType)value3;
    }

    //SKILLPARAM_SUPPORT_GUARD_ELEM_ELEMENT	指定属性からのダメージを軽減:属性
    public MasterDataDefineLabel.ElementType Get_SUPPORT_GUARD_ELEM_ELEMENT()
    {
        return (MasterDataDefineLabel.ElementType)value0;
    }

    //SKILLPARAM_SUPPORT_GUARD_ELEM_TURN_MIN	指定属性からのダメージを軽減:継続ターン数最小
    public int Get_SUPPORT_GUARD_ELEM_TURN_MIN()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_SUPPORT_GUARD_ELEM_TURN_MAX	指定属性からのダメージを軽減:継続ターン数最大
    public int Get_SUPPORT_GUARD_ELEM_TURN_MAX()
    {
        _debug_IsUsingField(value2);
        return value2;
    }

    //SKILLPARAM_SUPPORT_GUARD_ELEM_RATE	指定属性からのダメージを軽減：軽減倍率
    public int Get_SUPPORT_GUARD_ELEM_RATE()
    {
        _debug_IsUsingField(value3);
        return value3;
    }

    //SKILLPARAM_SUPPORT_KIND_POWUP_TURN_MIN	指定種族の攻撃力を増加させる：継続ターン数最小
    public int Get_SUPPORT_KIND_POWUP_TURN_MIN()
    {
        _debug_IsUsingField(value0);
        return value0;
    }

    //SKILLPARAM_SUPPORT_KIND_POWUP_TURN_MAX	指定種族の攻撃力を増加させる：継続ターン数最大
    public int Get_SUPPORT_KIND_POWUP_TURN_MAX()
    {
        _debug_IsUsingField(value1);
        return value1;
    }

    //SKILLPARAM_SUPPORT_KIND_POWUP_KIND	指定種族の攻撃力を増加させる：種族
    public MasterDataDefineLabel.KindType Get_SUPPORT_KIND_POWUP_KIND()
    {
        return (MasterDataDefineLabel.KindType)value2;
    }

    //SKILLPARAM_SUPPORT_KIND_POWUP_RATE	指定種族の攻撃力を増加させる：上昇倍率
    public int Get_SUPPORT_KIND_POWUP_RATE()
    {
        _debug_IsUsingField(value3);
        return value3;
    }




    //SKILLPARAM_ATK_DEATH_RATE	攻撃（即死）：即死確率（％）					[ 0 ～ ]
    public int Get_ATK_DEATH_RATE()
    {
        return value0;
    }

    //SKILLPARAM_SUPPORT_TELEPORT_RANDOM	テレポート：ランダム座標化ON/OFF
    public bool Get_SUPPORT_TELEPORT_RANDOM()
    {
        bool ret_val = (MasterDataDefineLabel.BoolType)value0 == MasterDataDefineLabel.BoolType.ENABLE;
        return ret_val;
    }
    //SKILLPARAM_SUPPORT_TELEPORT_POS_X	テレポート：X座標
    public int Get_SUPPORT_TELEPORT_POS_X()
    {
        return value1;
    }

    //SKILLPARAM_SUPPORT_TELEPORT_POS_Y	テレポート：Y座標
    public int Get_SUPPORT_TELEPORT_POS_Y()
    {
        return value2;
    }

    //SKILLPARAM_COST_CHANGE_ALL_ELEMENT_0	全コストを指定属性に変換
    public MasterDataDefineLabel.ElementType[] Get_COST_CHANGE_ALL_ELEMENT()
    {
        MasterDataDefineLabel.ElementType[] ret_val =
        {
            (MasterDataDefineLabel.ElementType)value0,
            (MasterDataDefineLabel.ElementType)value1,
            (MasterDataDefineLabel.ElementType)value2,
            (MasterDataDefineLabel.ElementType)value3,
            (MasterDataDefineLabel.ElementType)value4,
        };

        return ret_val;
    }

    //SKILLPARAM_RECOVERY_HP_VALUE	回復（HP割合)：回復倍率（％）					[ 0 ～ ]
    public int Get_RECOVERY_HP_VALUE()
    {
        //Developer TODO:スキル発動タイミングではこの値は使用していないかもしれない.
        return value0;
    }
    //SKILLPARAM_RECOVERY_HP_VALUE_RAND	回復（HP割合)：回復倍率振れ幅（％）			[ 0 ～ ]
    public int Get_RECOVERY_HP_VALUE_RAND()
    {
        //Developer TODO:スキル発動タイミングではこの値は使用していないかもしれない.
        return value1;
    }

    //SKILLPARAM_RECOVERY_FIX_HP_VALUE	回復（HP固定値）：回復値						[ 0 ～ ]
    public int Get_RECOVERY_FIX_HP_VALUE()
    {
        return value0;
    }

    //SKILLPARAM_RECOVERY_SP_VALUE	回復（SP)：回復値								[ 0 ～ ]
    public int Get_RECOVERY_SP_VALUE()
    {
        return value0;
    }

    //SKILLPARAM_SUPPORT_LBS_TURN_FAST_TURN	使用者以外のLBS必要ターン数を短縮する：短縮ターン数
    public int Get_SUPPORT_LBS_TURN_FAST_TURN()
    {
        return value0;
    }

    //手札変換パラメータ
    public BattleFieldPanelChangeParam GetBattleFieldPanelChangeParam()
    {
        int[] param =
        {
            (int)MasterDataDefineLabel.BoolType.NONE,
            100,
            value0,
            value1,
            value2,
            value3,
            value4,
            value5,
            value6,
            value7,
            value8,
            value9,
            value10,
            value11,
            value12,
            value13,
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
            ret_val.m_FixCount = value0;
            ret_val.m_AddCount = value1;
            ret_val.m_AddChancePercent = value2;
            ret_val.m_HpPercent = value3;
            ret_val.m_AddSpUse = value4;

            ret_val.m_FixSpUse = 0; // リミットブレイクスキルは固定復活ではＳＰ消費しない
        }

        return ret_val;
    }

    public int getProvokeTurn()
    {
        int ret_val = 0;
        if (skill_cate == MasterDataDefineLabel.SkillCategory.SUPPORT_PROVOKE)
        {
            ret_val = value0;
        }

        return ret_val;
    }

    // ソースコード上には残っているが実質的に未使用と思われるエリアが本当に未使用かどうかをチェック
    private void _debug_IsUsingField(int value)
    {
        if (value != 0)
        {
            Debug.LogError("未使用領域ではない？");
        }
    }

};
