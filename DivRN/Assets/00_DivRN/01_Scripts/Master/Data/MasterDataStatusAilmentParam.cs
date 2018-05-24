using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：状態異常整理用定義
	@note	状態異常パラメータ
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataStatusAilmentParam : Master
{
    public StatusAilment.GoodOrBad good_or_bad { get; set; }         //!< 良い、悪い			（MasterDataDefineLabel.BOOL_NONE）
    public MasterDataDefineLabel.BoolType clear_type { get; set; }              //!< クリアタイプ
    public MasterDataDefineLabel.AilmentType category { get; set; }                //!< 状態異常の種類		（MasterDataDefineLabel.AILMENT_TYPE_NONE）
    public int duration { get; set; }                //!< 継続ターン数

    public string icon { get; set; }                 //!< 状態アイコンリソース名
    public string name { get; set; }         //!< 名前
    public string detail { get; set; }                   //!< 詳細テキスト

    public MasterDataDefineLabel.BoolType update_move { get; set; }         //!< 移動中ターン経過	（MasterDataDefineLabel.BOOL_NONE）
    public MasterDataDefineLabel.BoolType update_battle { get; set; }           //!< 戦闘中ターン経過	（MasterDataDefineLabel.BOOL_NONE）

    public int param01 { get; set; }             //!< 状態異常の汎用パラメータ
    public int param02 { get; set; }             //!< 状態異常の汎用パラメータ
                                                 /*未使用*/
    public int param03 { get; set; }             //!< 状態異常の汎用パラメータ
                                                 /*未使用*/
    public int param04 { get; set; }             //!< 状態異常の汎用パラメータ
                                                 /*未使用*/
    public int param05 { get; set; }             //!< 状態異常の汎用パラメータ
                                                 /*未使用*/
    public int param06 { get; set; }             //!< 状態異常の汎用パラメータ
                                                 /*未使用*/
    public int param07 { get; set; }             //!< 状態異常の汎用パラメータ
                                                 /*未使用*/
    public int param08 { get; set; }             //!< 状態異常の汎用パラメータ
                                                 /*未使用*/
    public int param09 { get; set; }             //!< 状態異常の汎用パラメータ
                                                 /*未使用*/
    public int param10 { get; set; }             //!< 状態異常の汎用パラメータ
                                                 /*未使用*/
    public int param11 { get; set; }             //!< 状態異常の汎用パラメータ
                                                 /*未使用*/
    public int param12 { get; set; }             //!< 状態異常の汎用パラメータ

    //SKILLPARAM_ABSTATE_LATE_TURN_MIN	状態異常（ターンカウント遅延）：付加ターン最小	[ 0 ～ ]
    public int Get_ABSTATE_LATE_TURN_MIN()
    {
        return param01;
    }

    //SKILLPARAM_ABSTATE_LATE_TURN_MAX	状態異常（ターンカウント遅延）：付加ターン最大	[ 0 ～ ]
    public int Get_ABSTATE_LATE_TURN_MAX()
    {
        // ここはゼロしか入っていない
        return param02;
    }

    public MasterDataDefineLabel.AutoPlaySkillType Get_AUTO_PLAY_SKILL_TYPE()
    {
        MasterDataDefineLabel.AutoPlaySkillType ret_val = (MasterDataDefineLabel.AutoPlaySkillType)param01;
        return ret_val;
    }

    //効果値（割合）
    public int Get_VALUE_RATE()
    {
        int ret_val = 0;
        switch (category)
        {
            case MasterDataDefineLabel.AilmentType.TIMER:
                break;

            case MasterDataDefineLabel.AilmentType.SKILL_HEALING:
                if ((MasterDataDefineLabel.BoolType)param01 != MasterDataDefineLabel.BoolType.ENABLE)
                {
                    ret_val = param02;
                }
                break;

            default:
                ret_val = param01;
                break;
        }
        return ret_val;
    }

    //効果値（固定値）
    public int Get_VALUE_FIX()
    {
        int ret_val = 0;
        switch (category)
        {
            case MasterDataDefineLabel.AilmentType.TIMER:
                ret_val = param01;
                break;

            case MasterDataDefineLabel.AilmentType.SKILL_HEALING:
                if ((MasterDataDefineLabel.BoolType)param01 == MasterDataDefineLabel.BoolType.ENABLE)
                {
                    ret_val = param02;
                }
                break;

            default:
                break;
        }
        return ret_val;
    }
};
