using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：敵行動定義
	@note	１匹の敵が戦闘中にやる行動１つ分の情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataEnemyActionParam : Master
{
    //public int fix_id;                  //!<

    public uint timing_public { get; set; }          //!< 一般公開タイミング

    public string skill_name { get; set; }               //!< 表示用のテキスト
    public int add_fix_id { get; set; }              //!< 追加行動(連続行動)					 (MasterDataEnemyActionParam.fix_id)
                                                     //public int attack_target{get;set;}           //!< 攻撃対象							（MasterDataDefineLabel.TARGET_NONE）
    public MasterDataDefineLabel.TargetType attack_target { get; set; }           //!< 攻撃対象							（MasterDataDefineLabel.TARGET_NONE）
    public int attack_target_num { get; set; }           //!< 攻撃対象数
    public int se_id { get; set; }                   //!< SEID(ラベルID)						（MasterDataDefineLabel.ENEMY_SE_NONE）
                                                     //public int effect_id{get;set;}               //!< エフェクトID(名前?タイプ?)			（MasterDataDefineLabel.UIEFFECT_NONE）
    public MasterDataDefineLabel.UIEffectType effect_id { get; set; }               //!< エフェクトID(名前?タイプ?)			（MasterDataDefineLabel.UIEFFECT_NONE）
                                                                                    //public int attack_motion{get;set;}           //!< 攻撃モーション(ON/OFF)				（MasterDataDefineLabel.ENEMY_MOTION_NONE）
    public MasterDataDefineLabel.EnemyMotionType attack_motion { get; set; }           //!< 攻撃モーション(ON/OFF)				（MasterDataDefineLabel.ENEMY_MOTION_NONE）
                                                                                       //public int shake_screen{get;set;}            //!< 画面揺れ(ON/OFF)					（MasterDataDefineLabel.ENEMY_ATKSHAKE_NONE）
    public MasterDataDefineLabel.EnemyATKShakeType shake_screen { get; set; }            //!< 画面揺れ(ON/OFF)					（MasterDataDefineLabel.ENEMY_ATKSHAKE_NONE）
                                                                                         //public int damage_effect{get;set;}           //!< ダメージエフェクト(ON/OFF)			（MasterDataDefineLabel.BOOL_NONE）
    public MasterDataDefineLabel.BoolType damage_effect { get; set; }           //!< ダメージエフェクト(ON/OFF)			（MasterDataDefineLabel.BOOL_NONE）
                                                                                //public int damage_draw{get;set;}         //!< ダメージ数値(ON/OFF)				（MasterDataDefineLabel.BOOL_NONE）
    public MasterDataDefineLabel.BoolType damage_draw { get; set; }         //!< ダメージ数値(ON/OFF)				（MasterDataDefineLabel.BOOL_NONE）

    public int wait_time { get; set; }

    //public int status_ailment_target{get;set;}   //!< 状態変化対象						（MasterDataDefineLabel.TARGET_NON）
    public MasterDataDefineLabel.TargetType status_ailment_target { get; set; }   //!< 状態変化対象						（MasterDataDefineLabel.TARGET_NON）
    public int status_ailment1 { get; set; }     //!< 状態変化ID１							（StatusAilmentParam.fix_id）
    public int status_ailment2 { get; set; }     //!< 状態変化ID２							（StatusAilmentParam.fix_id）
    public int status_ailment3 { get; set; }     //!< 状態変化ID３							（StatusAilmentParam.fix_id）
    public int status_ailment4 { get; set; }     //!< 状態変化ID４							（StatusAilmentParam.fix_id）

    //	public in		cutin{get;set;}					//!< カットイン(ON/OFF)

    //public int skill_type{get;set;}              //!< スキルタイプ						（MasterDataDefineLabel.ENEMY_SKILL_NONE）
    public MasterDataDefineLabel.EnemySkillCategory skill_type { get; set; }              //!< スキルタイプ						（MasterDataDefineLabel.ENEMY_SKILL_NONE）
    public int skill_param1 { get; set; }            //!< スキル汎用パラメータ
    public int skill_param2 { get; set; }            //!< スキル汎用パラメータ
    public int skill_param3 { get; set; }            //!< スキル汎用パラメータ
    public int skill_param4 { get; set; }            //!< スキル汎用パラメータ
    public int skill_param5 { get; set; }            //!< スキル汎用パラメータ
    public int skill_param6 { get; set; }            //!< スキル汎用パラメータ
    public int skill_param7 { get; set; }            //!< スキル汎用パラメータ
    public int skill_param8 { get; set; }            //!< スキル汎用パラメータ
    public int skill_param9 { get; set; }            //!< スキル汎用パラメータ
    public int skill_param10 { get; set; }           //!< スキル汎用パラメータ
    public int skill_param11 { get; set; }           //!< スキル汎用パラメータ
    public int skill_param12 { get; set; }           //!< スキル汎用パラメータ
    public int skill_param13 { get; set; }           //!< スキル汎用パラメータ
    public int skill_param14 { get; set; }           //!< スキル汎用パラメータ
    public int skill_param15 { get; set; }           //!< スキル汎用パラメータ
    public int skill_param16 { get; set; }           //!< スキル汎用パラメータ

    public uint audio_data_id { get; set; }          //!< 行動時のボイス指定


    //SKILLPARAM_ATK_NORMAL_POW_RATE	リミブレスキルパラメータ：攻撃：攻撃倍率（％）							[ 0 ～ ]
    public int Get_ATK_NORMAL_POW_RATE()
    {
        return skill_param1;
    }

    //SKILLPARAM_ATK_NORMAL_POW_RATE_R	リミブレスキルパラメータ：攻撃：攻撃倍率振れ幅（％）						[ 0 ～ ]
    public int Get_ATK_NORMAL_POW_RATE_R()
    {
        return skill_param2;
    }

    //SKILLPARAM_ATK_ELEM_POW_RATE	リミブレスキルパラメータ：攻撃（対象属性限定）：攻撃倍率（％）			[ 0 ～ ]
    public int Get_ATK_ELEM_POW_RATE()
    {
        return skill_param2;
    }

    //SKILLPARAM_ATK_ELEM_TARGET_ELEM	リミブレスキルパラメータ：攻撃（対象属性限定）：属性
    public MasterDataDefineLabel.ElementType Get_ATK_ELEM_TARGET_ELEM()
    {
        return (MasterDataDefineLabel.ElementType)skill_param1;
    }

    //ENEMYSKILL_PARAM_HANDCARD_REMAKE_RANDOM	敵スキルパラメータ：ランダム
    public bool Get_HANDCARD_REMAKE_RANDOM()
    {
        bool ret_val = ((MasterDataDefineLabel.BoolType)skill_param1 == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    //ENEMYSKILL_PARAM_HANDCARD_REMAKE_FIRE	敵スキルパラメータ：炎変換先
    public MasterDataDefineLabel.ElementType Get_HANDCARD_REMAKE_FIRE()
    {
        return (MasterDataDefineLabel.ElementType)skill_param2;
    }

    //ENEMYSKILL_PARAM_HANDCARD_REMAKE_WATER	敵スキルパラメータ：水変換先
    public MasterDataDefineLabel.ElementType Get_HANDCARD_REMAKE_WATER()
    {
        return (MasterDataDefineLabel.ElementType)skill_param3;
    }

    //ENEMYSKILL_PARAM_HANDCARD_REMAKE_WIND	敵スキルパラメータ：風変換先
    public MasterDataDefineLabel.ElementType Get_HANDCARD_REMAKE_WIND()
    {
        return (MasterDataDefineLabel.ElementType)skill_param4;
    }

    //ENEMYSKILL_PARAM_HANDCARD_REMAKE_LIGHT	敵スキルパラメータ：光変換先
    public MasterDataDefineLabel.ElementType Get_HANDCARD_REMAKE_LIGHT()
    {
        return (MasterDataDefineLabel.ElementType)skill_param5;
    }

    //ENEMYSKILL_PARAM_HANDCARD_REMAKE_DARK	敵スキルパラメータ：闇変換先
    public MasterDataDefineLabel.ElementType Get_HANDCARD_REMAKE_DARK()
    {
        return (MasterDataDefineLabel.ElementType)skill_param6;
    }

    //ENEMYSKILL_PARAM_HANDCARD_REMAKE_NAUGHT	敵スキルパラメータ：無変換先
    public MasterDataDefineLabel.ElementType Get_HANDCARD_REMAKE_NAUGHT()
    {
        return (MasterDataDefineLabel.ElementType)skill_param7;
    }

    //ENEMYSKILL_PARAM_HANDCARD_REMAKE_HEAL	敵スキルパラメータ：回変換先
    public MasterDataDefineLabel.ElementType Get_HANDCARD_REMAKE_HEAL()
    {
        return (MasterDataDefineLabel.ElementType)skill_param8;
    }

    //ENEMYSKILL_PARAM_ATKHP_RATE_CHK_HPMAX	敵スキルパラメータ：最大HPで判定を行うか
    public bool Get_ATKHP_RATE_CHK_HPMAX()
    {
        bool ret_val = ((MasterDataDefineLabel.BoolType)skill_param1 == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    //ENEMYSKILL_PARAM_ATKHP_RATE_RATE	敵スキルパラメータ：ダメージ割合
    public int Get_ATKHP_RATE_RATE()
    {
        return skill_param2;
    }

    //ENEMYSKILL_PARAM_HEAL_TARGET	敵スキルパラメータ：対象
    public MasterDataDefineLabel.TargetType Get_HEAL_TARGET()
    {
        return (MasterDataDefineLabel.TargetType)skill_param1;
    }

    //ENEMYSKILL_PARAM_HEAL_RATE_MODE	敵スキルパラメータ：割合回復：有効無効
    public bool Get_HEAL_RATE_MODE()
    {
        bool ret_val = ((MasterDataDefineLabel.BoolType)skill_param2 == MasterDataDefineLabel.BoolType.ENABLE);
        return ret_val;
    }

    //ENEMYSKILL_PARAM_HEAL_VALUE	敵スキルパラメータ：固定回復量
    public int Get_HEAL_VALUE()
    {
        return skill_param3;
    }

    //ENEMYSKILL_PARAM_HEAL_VALUE_RATE	敵スキルパラメータ：割合回復量
    public int Get_HEAL_VALUE_RATE()
    {
        return skill_param4;
    }

    //ENEMYSKILL_PARAM_DEATH_RATE	敵スキルパラメータ：即死攻撃：発動率
    public int Get_DEATH_RATE()
    {
        return skill_param1;
    }

    public uint Get_ENEMY_ABILITY_FIX_ID()
    {
        return (uint)skill_param1;
    }

    public int Get_ENEMY_ABILITY_TURN()
    {
        return skill_param2;
    }


    public BattleFieldPanelChangeParam GetBattleFieldPanelChangeParam()
    {
        int[] param =
        {
            (int)MasterDataDefineLabel.BoolType.NONE,
            100,
            skill_param1,
            skill_param2,
            skill_param3,
            skill_param4,
            skill_param5,
            skill_param6,
            skill_param7,
            skill_param8,
            skill_param9,
            skill_param10,
            skill_param11,
            skill_param12,
            skill_param13,
            skill_param14,
        };

        BattleFieldPanelChangeParam ret_val = new BattleFieldPanelChangeParam();
        ret_val.init(param);

        return ret_val;
    }
};
