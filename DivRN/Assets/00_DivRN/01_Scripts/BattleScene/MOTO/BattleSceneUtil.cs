using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



//----------------------------------------------------------------------------
//	class
//----------------------------------------------------------------------------
//----------------------------------------------------------------------------
/*!
    @class		BattleUtil
    @brief		戦闘関連のユーティリティー
*/
//----------------------------------------------------------------------------
public static class BattleSceneUtil
{
    public const float MAX_ASPECT = 640.0f / 960.0f; //これより横長の画面の場合左右に黒帯が出て横長画面にならないようになっている

    //------------------------------------------------------------------------
    /*!
        @brief			アクティブスキルエフェクト取得			<static>
        @param[in]		int				(effLabel)		エフェクトラベル
        @param[in]		int				(out_anchor)	エフェクトの表示位置ID
        @param[in]		GameObject		(out_effect)	エフェクト
    */
    //------------------------------------------------------------------------
    static public GameObject GetSkillEffectPrefab(MasterDataDefineLabel.UIEffectType effLabel,
                                        ref InGameDefine.Effect2DAnchorType out_anchor)
    {

        InGameDefine.Effect2DAnchorType anchor_id = 0;
        GameObject eff = null;


        //--------------------------------
        //	ラベルに対応するエフェクトを選択
        //--------------------------------
        switch (effLabel)
        {
            case MasterDataDefineLabel.UIEffectType.NAUGHT_MA_00: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Naught_MA_00; break;
            case MasterDataDefineLabel.UIEffectType.NAUGHT_MA_01: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Naught_MA_01; break;
            case MasterDataDefineLabel.UIEffectType.NAUGHT_MM_00: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Naught_MM_00; break;
            case MasterDataDefineLabel.UIEffectType.NAUGHT_MM_01: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Naught_MM_01; break;
            case MasterDataDefineLabel.UIEffectType.NAUGHT_SA_00: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Naught_SA_00; break;
            case MasterDataDefineLabel.UIEffectType.NAUGHT_SA_01: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Naught_SA_01; break;
            case MasterDataDefineLabel.UIEffectType.NAUGHT_SA_02: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Naught_SA_02; break;
            case MasterDataDefineLabel.UIEffectType.NAUGHT_SM_00: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Naught_SM_00; break;
            case MasterDataDefineLabel.UIEffectType.NAUGHT_SM_01: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Naught_SM_01; break;
            case MasterDataDefineLabel.UIEffectType.NAUGHT_SM_02: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Naught_SM_02; break;

            case MasterDataDefineLabel.UIEffectType.FIRE_MA_00: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Fire_MA_00; break;
            case MasterDataDefineLabel.UIEffectType.FIRE_MA_01: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Fire_MA_01; break;
            case MasterDataDefineLabel.UIEffectType.FIRE_MM_00: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Fire_MM_00; break;
            case MasterDataDefineLabel.UIEffectType.FIRE_MM_01: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Fire_MM_01; break;
            case MasterDataDefineLabel.UIEffectType.FIRE_SA_00: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Fire_SA_00; break;
            case MasterDataDefineLabel.UIEffectType.FIRE_SA_01: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Fire_SA_01; break;
            case MasterDataDefineLabel.UIEffectType.FIRE_SA_02: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Fire_SA_02; break;
            case MasterDataDefineLabel.UIEffectType.FIRE_SM_00: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Fire_SM_00; break;
            case MasterDataDefineLabel.UIEffectType.FIRE_SM_01: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Fire_SM_01; break;
            case MasterDataDefineLabel.UIEffectType.FIRE_SM_02: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Fire_SM_02; break;

            case MasterDataDefineLabel.UIEffectType.WATER_MA_00: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Water_MA_00; break;
            case MasterDataDefineLabel.UIEffectType.WATER_MA_01: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Water_MA_01; break;
            case MasterDataDefineLabel.UIEffectType.WATER_MM_00: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Water_MM_00; break;
            case MasterDataDefineLabel.UIEffectType.WATER_MM_01: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Water_MM_01; break;
            case MasterDataDefineLabel.UIEffectType.WATER_SA_00: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Water_SA_00; break;
            case MasterDataDefineLabel.UIEffectType.WATER_SA_01: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Water_SA_01; break;
            case MasterDataDefineLabel.UIEffectType.WATER_SA_02: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Water_SA_02; break;
            case MasterDataDefineLabel.UIEffectType.WATER_SM_00: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Water_SM_00; break;
            case MasterDataDefineLabel.UIEffectType.WATER_SM_01: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Water_SM_01; break;
            case MasterDataDefineLabel.UIEffectType.WATER_SM_02: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Water_SM_02; break;

            case MasterDataDefineLabel.UIEffectType.WIND_MA_00: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Wind_MA_00; break;
            case MasterDataDefineLabel.UIEffectType.WIND_MA_01: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Wind_MA_01; break;
            case MasterDataDefineLabel.UIEffectType.WIND_MM_00: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Wind_MM_00; break;
            case MasterDataDefineLabel.UIEffectType.WIND_MM_01: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Wind_MM_01; break;
            case MasterDataDefineLabel.UIEffectType.WIND_SA_00: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Wind_SA_00; break;
            case MasterDataDefineLabel.UIEffectType.WIND_SA_01: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Wind_SA_01; break;
            case MasterDataDefineLabel.UIEffectType.WIND_SA_02: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Wind_SA_02; break;
            case MasterDataDefineLabel.UIEffectType.WIND_SM_00: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Wind_SM_00; break;
            case MasterDataDefineLabel.UIEffectType.WIND_SM_01: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Wind_SM_01; break;
            case MasterDataDefineLabel.UIEffectType.WIND_SM_02: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Wind_SM_02; break;

            case MasterDataDefineLabel.UIEffectType.LIGHT_MA_00: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Light_MA_00; break;
            case MasterDataDefineLabel.UIEffectType.LIGHT_MA_01: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Light_MA_01; break;
            case MasterDataDefineLabel.UIEffectType.LIGHT_MM_00: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Light_MM_00; break;
            case MasterDataDefineLabel.UIEffectType.LIGHT_MM_01: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Light_MM_01; break;
            case MasterDataDefineLabel.UIEffectType.LIGHT_SA_00: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Light_SA_00; break;
            case MasterDataDefineLabel.UIEffectType.LIGHT_SA_01: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Light_SA_01; break;
            case MasterDataDefineLabel.UIEffectType.LIGHT_SA_02: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Light_SA_02; break;
            case MasterDataDefineLabel.UIEffectType.LIGHT_SM_00: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Light_SM_00; break;
            case MasterDataDefineLabel.UIEffectType.LIGHT_SM_01: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Light_SM_01; break;
            case MasterDataDefineLabel.UIEffectType.LIGHT_SM_02: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Light_SM_02; break;

            case MasterDataDefineLabel.UIEffectType.DARK_MA_00: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Dark_MA_00; break;
            case MasterDataDefineLabel.UIEffectType.DARK_MA_01: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Dark_MA_01; break;
            case MasterDataDefineLabel.UIEffectType.DARK_MM_00: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Dark_MM_00; break;
            case MasterDataDefineLabel.UIEffectType.DARK_MM_01: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Dark_MM_01; break;
            case MasterDataDefineLabel.UIEffectType.DARK_SA_00: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Dark_SA_00; break;
            case MasterDataDefineLabel.UIEffectType.DARK_SA_01: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Dark_SA_01; break;
            case MasterDataDefineLabel.UIEffectType.DARK_SA_02: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Dark_SA_02; break;
            case MasterDataDefineLabel.UIEffectType.DARK_SM_00: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Dark_SM_00; break;
            case MasterDataDefineLabel.UIEffectType.DARK_SM_01: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Dark_SM_01; break;
            case MasterDataDefineLabel.UIEffectType.DARK_SM_02: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Dark_SM_02; break;

            case MasterDataDefineLabel.UIEffectType.HEAL_1: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_00; break;
            case MasterDataDefineLabel.UIEffectType.HEAL_2: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_01; break;
            case MasterDataDefineLabel.UIEffectType.HEAL_3: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_02; break;
            case MasterDataDefineLabel.UIEffectType.HEAL_4: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_03; break;
            case MasterDataDefineLabel.UIEffectType.SP_HEAL: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_SP; break;
            case MasterDataDefineLabel.UIEffectType.BLOOD: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Blood; break;
            case MasterDataDefineLabel.UIEffectType.POISON: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Poison; break;

            case MasterDataDefineLabel.UIEffectType.BUFF: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Buff; break;
            case MasterDataDefineLabel.UIEffectType.DEBUFF: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Debuff; break;

            case MasterDataDefineLabel.UIEffectType.ENEMY_SKILL_FIRE: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_EnemySkillFire00; break;
            case MasterDataDefineLabel.UIEffectType.ENEMY_SKILL_WIND: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_EnemySkillWind00; break;
            case MasterDataDefineLabel.UIEffectType.ENEMY_SKILL_WATER: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_EnemySkillWater00; break;
            case MasterDataDefineLabel.UIEffectType.ENEMY_SKILL_LIGHT: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_EnemySkillLight00; break;
            case MasterDataDefineLabel.UIEffectType.ENEMY_SKILL_DARK: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_EnemySkillDark00; break;
            case MasterDataDefineLabel.UIEffectType.ENEMY_SKILL_NAUGHT: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_EnemySkillNaught00; break;

            case MasterDataDefineLabel.UIEffectType.ENEMY_SKILL_HEAL_S: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_EnemySkillHeal_S; break;
            case MasterDataDefineLabel.UIEffectType.ENEMY_SKILL_HEAL_M: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_EnemySkillHeal_M; break;

            case MasterDataDefineLabel.UIEffectType.ENEMY_SKILL_BUFF: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_EnemySkillBuff; break;
            case MasterDataDefineLabel.UIEffectType.ENEMY_SKILL_DEBUFF: anchor_id = InGameDefine.Effect2DAnchorType.CENTER; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_EnemySkillDebuff; break;

            case MasterDataDefineLabel.UIEffectType.HM_M_FIRE: anchor_id = InGameDefine.Effect2DAnchorType.CENTER2; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HM_M_Fire; break;
            case MasterDataDefineLabel.UIEffectType.HM_M_WATER: anchor_id = InGameDefine.Effect2DAnchorType.CENTER2; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HM_M_Water; break;
            case MasterDataDefineLabel.UIEffectType.HM_M_WIND: anchor_id = InGameDefine.Effect2DAnchorType.CENTER2; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HM_M_Wind; break;
            case MasterDataDefineLabel.UIEffectType.HM_M_LIGHT: anchor_id = InGameDefine.Effect2DAnchorType.CENTER2; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HM_M_Light; break;
            case MasterDataDefineLabel.UIEffectType.HM_M_DARK: anchor_id = InGameDefine.Effect2DAnchorType.CENTER2; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HM_M_Dark; break;
            case MasterDataDefineLabel.UIEffectType.HM_M_NAUGHT: anchor_id = InGameDefine.Effect2DAnchorType.CENTER2; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HM_M_Naught; break;

            case MasterDataDefineLabel.UIEffectType.HM_S_FIRE: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HM_S_Fire; break;
            case MasterDataDefineLabel.UIEffectType.HM_S_WATER: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HM_S_Water; break;
            case MasterDataDefineLabel.UIEffectType.HM_S_WIND: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HM_S_Wind; break;
            case MasterDataDefineLabel.UIEffectType.HM_S_LIGHT: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HM_S_Light; break;
            case MasterDataDefineLabel.UIEffectType.HM_S_DARK: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HM_S_Dark; break;
            case MasterDataDefineLabel.UIEffectType.HM_S_NAUGHT: anchor_id = InGameDefine.Effect2DAnchorType.ENEMY_ONCE; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HM_S_Naught; break;

            case MasterDataDefineLabel.UIEffectType.PLAYER_SKILL_BUFF: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_PlayerSkillBuff; break;
            case MasterDataDefineLabel.UIEffectType.PLAYER_SKILL_DEBUFF: anchor_id = InGameDefine.Effect2DAnchorType.BOTTOM; eff = SceneObjReferGameMain.Instance.m_EffectPrefab.m_PlayerSkillDebuff; break;


            default: eff = null; break;
        }


        //--------------------------------
        //	出力
        //--------------------------------
        out_anchor = anchor_id;

        return eff;
    }

    public static void setRide(Transform my_transform, Transform parent_transform)
    {
        my_transform.SetParent(parent_transform, false);
    }

    /// <summary>
    /// リミットブレイクスキルが発動可能であるかどうかをチェック
    /// 元あった場所： InGameSkillWindows.cs の 740行付近の skillAble を設定する辺り.
    /// </summary>
    /// <returns></returns>
    public static bool checkLimitBreak(CharaParty player_party, GlobalDefine.PartyCharaIndex caster_index, BattleEnemy[] enemys, int target_enemy_index = -1)
    {
        // チュートリアル中の発動禁止処理.
        if (BattleParam.IsTutorial())
        {
            if (BattleSceneManager.Instance.isTutorialForbidLimitBreak(caster_index))
            {
                return false;
            }
        }

        MasterDataSkillLimitBreak limit_break_skill = null;

        if (caster_index != GlobalDefine.PartyCharaIndex.HERO)
        {
            CharaOnce caster_chara = player_party.getPartyMember(caster_index, CharaParty.CharaCondition.SKILL_LIMITBREAK);
            if (caster_chara == null)
            {
                return false;
            }

            //--------------------------------
            // スキル発動に必要なターン数が経過しているか
            //--------------------------------
            if (caster_chara.ChkCharaLimitBreak() == false)
            {
                return false;
            }

            limit_break_skill = BattleParam.m_MasterDataCache.useSkillLimitBreak(caster_chara.m_CharaMasterDataParam.skill_limitbreak);
        }
        else
        {
            if (BattleParam.m_PlayerParty.m_BattleHero.checkHeroSkillTurn() == false)
            {
                return false;
            }

            return true;    //Developer 今までチェックしてなかったようなので今はチェックせず発動可能に
        }

        if (limit_break_skill == null)
        {
            return false;
        }

        //--------------------------------
        //	SPコストチェック
        //--------------------------------
        int sp_val = player_party.GetSP();
        if (sp_val < limit_break_skill.use_sp)
        {
            return false;
        }


        //--------------------------------
        // スキル使用禁止状態
        //--------------------------------
        if (player_party.m_Ailments.getAilment(caster_index).GetSkillNotUse() == true)
        {
            return false;
        }


        bool ailmentChk;

        //--------------------------------
        // 同じタイプの状態異常がかかっていたら使用することができない
        //--------------------------------
        switch (limit_break_skill.status_ailment_target)
        {
            case MasterDataDefineLabel.TargetType.SELF:     // 自分
            case MasterDataDefineLabel.TargetType.FRIEND:   // 使用者の味方全員
                ailmentChk = ChkLimitBreakAilmentStatusPlayer(limit_break_skill, player_party, false);
                if (ailmentChk == true)
                {
                    return false;
                }
                break;

            case MasterDataDefineLabel.TargetType.OTHER:    // 相手
            case MasterDataDefineLabel.TargetType.ENEMY:    // 使用者の敵全員
            case MasterDataDefineLabel.TargetType.ENE_N_1:
            case MasterDataDefineLabel.TargetType.ENE_1N_1:
            case MasterDataDefineLabel.TargetType.ENE_R_N:
            case MasterDataDefineLabel.TargetType.ENE_1_N:
                ailmentChk = ChkLimitbreakAilmentStatusEnemy(limit_break_skill, enemys);
                if (ailmentChk == true)
                {
                    return false;
                }
                break;

            case MasterDataDefineLabel.TargetType.ALL:      // 全員
                ailmentChk = ChkLimitbreakAilmentStatusEnemy(limit_break_skill, enemys);
                if (ailmentChk == true)
                {
                    return false;
                }

                ailmentChk = ChkLimitBreakAilmentStatusPlayer(limit_break_skill, player_party, false);
                if (ailmentChk == true)
                {
                    return false;
                }
                break;
        }

        //--------------------------------
        // 指定都合状態異常全クリアは、クリアできる状態異常にかかっていないと使用することができない
        //--------------------------------
        if (limit_break_skill.skill_cate == MasterDataDefineLabel.SkillCategory.SUPPORT_SPESTATE_CLEAR)
        {
            if (ChkLimitbreakAilmentStatusCondition(limit_break_skill, player_party, enemys) == false)
            {
                return false;
            }
        }

        //--------------------------------
        // 自己瀕死効果系はHP残量が１より多くなければ使用することができない
        //--------------------------------
        if (limit_break_skill.skill_cate == MasterDataDefineLabel.SkillCategory.ATK_ANKOKU
        || limit_break_skill.skill_cate == MasterDataDefineLabel.SkillCategory.DAMAGE_ANKOKU)
        {
            if (player_party.m_HPCurrent.getValue(caster_index) <= 1)
            {
                return false;
            }
        }


        //--------------------------------
        // パネル変化系スキルは対象パネルがないと発動できない
        //--------------------------------
        if (limit_break_skill.skill_cate == MasterDataDefineLabel.SkillCategory.COST_CHANGE_ELEM)
        {
            MasterDataDefineLabel.ElementType elem = limit_break_skill.Get_COST_CHANGE_PREV();
            // 戦闘中のみチェック
            if (BattleParam.isActiveBattle())
            {
                bool changeCard = false;
                // 手札をすべて見て、対象属性の手札があるかを調べる
                for (int i = 0; i < BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCardMaxCount(); i++)
                {
                    BattleScene.BattleCard battle_card = BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.m_HandArea.getCard(i);
                    if (battle_card == null)
                    {
                        continue;
                    }

                    if (battle_card.getElementType() == elem)
                    {
                        changeCard = true;
                        break;
                    }
                }

                if (changeCard == false)
                {
                    return false;
                }
            }
        }

        //--------------------------------
        // 復活系スキルは誰も死亡していないと使用することができない
        //--------------------------------
        if (limit_break_skill.skill_cate == MasterDataDefineLabel.SkillCategory.SUPPORT_RESURRECT)
        {
            CharaOnce[] dead_members = BattleParam.m_PlayerParty.getPartyMembers(CharaParty.CharaCondition.DEAD);
            if (dead_members.Length <= 0)
            {
                return false;
            }
        }

        //--------------------------------
        // HP回復効果系スキルはHP残量が最大時には使用することができない
        //--------------------------------
        if (limit_break_skill.skill_cate == MasterDataDefineLabel.SkillCategory.RECOVERY_HP
        || limit_break_skill.skill_cate == MasterDataDefineLabel.SkillCategory.RECOVERY_HP_RATE)
        {
            BattleSceneUtil.MultiInt hp_max_alive = new BattleSceneUtil.MultiInt(player_party.m_HPCurrent); //生存者の最大ＨＰ合計
            hp_max_alive.minValue(GlobalDefine.PartyCharaIndex.MAX, 1);
            hp_max_alive.mulValue(GlobalDefine.PartyCharaIndex.MAX, player_party.m_HPMax);
            if (player_party.m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MAX) >= hp_max_alive.getValue(GlobalDefine.PartyCharaIndex.MAX))
            {
                return false;
            }
        }


        //--------------------------------
        // SP回復効果系スキルはSP残量が最大時には使用することができない
        //--------------------------------
        if (limit_break_skill.skill_cate == MasterDataDefineLabel.SkillCategory.RECOVERY_SP)
        {
            if (player_party.m_PartyTotalSP >= player_party.m_PartyTotalSPMax)
            {
                return false;
            }
        }

        //--------------------------------
        // 属性指定効果は対象属性がいないと使用することができない
        //--------------------------------
        if (limit_break_skill.skill_cate == MasterDataDefineLabel.SkillCategory.DAMAGE_ELEM)
        {
            MasterDataDefineLabel.ElementType elem = limit_break_skill.Get_FIX_DMG_ELEM_T_ELEM();
            if (ChkSpecificElementEnemy(elem, enemys, target_enemy_index) == false)
            {
                return false;
            }
        }


        //--------------------------------
        // 属性指定効果は対象属性がいないと使用することができない
        //--------------------------------
        if (limit_break_skill.skill_cate == MasterDataDefineLabel.SkillCategory.ATK_ELEM)
        {
            MasterDataDefineLabel.ElementType elem = limit_break_skill.Get_ATK_ELEM_TARGET_ELEM();
            if (ChkSpecificElementEnemy(elem, enemys, target_enemy_index) == false)
            {
                return false;
            }
        }


        //--------------------------------
        // 属性固定化スキルは１つの属性のみ発動することができる
        //--------------------------------
        if (limit_break_skill.skill_cate == MasterDataDefineLabel.SkillCategory.SUPPORT_COST_FIX)
        {
            MasterDataDefineLabel.AilmentType nType;

            StatusAilmentChara party_ailment = player_party.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX);

            // 炎
            {
                nType = StatusAilment.getAilmentTypeFromLimitBreakSkillCategory(limit_break_skill.skill_cate, MasterDataDefineLabel.ElementType.FIRE, MasterDataDefineLabel.KindType.NONE);
                if (party_ailment.IsHavingAilment(nType) == true)
                {
                    return false;
                }
            }

            // 水
            {
                nType = StatusAilment.getAilmentTypeFromLimitBreakSkillCategory(limit_break_skill.skill_cate, MasterDataDefineLabel.ElementType.WATER, MasterDataDefineLabel.KindType.NONE);
                if (party_ailment.IsHavingAilment(nType) == true)
                {
                    return false;
                }
            }

            // 光
            {
                nType = StatusAilment.getAilmentTypeFromLimitBreakSkillCategory(limit_break_skill.skill_cate, MasterDataDefineLabel.ElementType.LIGHT, MasterDataDefineLabel.KindType.NONE);
                if (party_ailment.IsHavingAilment(nType) == true)
                {
                    return false;
                }
            }

            // 闇
            {
                nType = StatusAilment.getAilmentTypeFromLimitBreakSkillCategory(limit_break_skill.skill_cate, MasterDataDefineLabel.ElementType.DARK, MasterDataDefineLabel.KindType.NONE);
                if (party_ailment.IsHavingAilment(nType) == true)
                {
                    return false;
                }
            }

            // 風
            {
                nType = StatusAilment.getAilmentTypeFromLimitBreakSkillCategory(limit_break_skill.skill_cate, MasterDataDefineLabel.ElementType.WIND, MasterDataDefineLabel.KindType.NONE);
                if (party_ailment.IsHavingAilment(nType) == true)
                {
                    return false;
                }
            }

            // 回復
            {
                nType = StatusAilment.getAilmentTypeFromLimitBreakSkillCategory(limit_break_skill.skill_cate, MasterDataDefineLabel.ElementType.HEAL, MasterDataDefineLabel.KindType.NONE);
                if (party_ailment.IsHavingAilment(nType) == true)
                {
                    return false;
                }
            }

            // 無
            {
                nType = StatusAilment.getAilmentTypeFromLimitBreakSkillCategory(limit_break_skill.skill_cate, MasterDataDefineLabel.ElementType.NAUGHT, MasterDataDefineLabel.KindType.NONE);
                if (party_ailment.IsHavingAilment(nType) == true)
                {
                    return false;
                }
            }
        }

        return true;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		プレイヤーのLBSパラメータで状態異常効果チェック
        @param[in]	int		(charaIdx)		スキル所持者ID
        @param[in]	int		(is_check_full)		チェックモード。false:一人でも状態異常がいればtrueが返る true:全員が状態の時のみtrueが返る
        @retval		bool	[効果中/効果中でない]
        元あった場所：InGameUtil.cs 内の ChkLimitBreakAilmentStatusPlayer()
    */
    //------------------------------------------------------------------------
    static public bool ChkLimitBreakAilmentStatusPlayer(MasterDataSkillLimitBreak limit_break_skill, CharaParty player_party, bool is_check_full)
    {
        if (limit_break_skill == null
        || player_party == null)
        {
            return false;
        }

        MasterDataDefineLabel.AilmentType nType = StatusAilment.getAilmentTypeFromLimitBreakSkillCategory(limit_break_skill);

        int have_count = 0;
        int check_count = 0;
        for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
        {
            bool has = player_party.m_Ailments.getAilment((GlobalDefine.PartyCharaIndex)idx).IsHavingAilment(nType);
            if (has)
            {
                have_count++;
            }
            check_count++;
        }

        if (is_check_full == false)
        {
            if (have_count > 0)
            {
                return true;
            }
        }
        else
        {
            if (have_count == check_count)
            {
                return true;
            }
        }

        return false;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		敵のLBSパラメータで状態異常効果チェック
        @param[in]	int		(charaIdx)		スキル所持者ID
        @retval		bool	[効果中/効果中の敵はいない]
        元あった場所：InGameUtil.cs 内の ChkLimitbreakAilmentStatusEnemy()
    */
    //------------------------------------------------------------------------
    static public bool ChkLimitbreakAilmentStatusEnemy(MasterDataSkillLimitBreak limit_break_skill, BattleEnemy[] enemys)
    {

        if (limit_break_skill == null
        || enemys == null)
        {
            return false;
        }

        MasterDataDefineLabel.AilmentType nType = StatusAilment.getAilmentTypeFromLimitBreakSkillCategory(limit_break_skill);

        for (int idx = 0; idx < enemys.Length; idx++)
        {
            BattleEnemy enemy = enemys[idx];
            if (enemy != null
                && enemy.isDead() == false
                && enemy.m_StatusAilmentChara != null
            )
            {
                if (enemy.m_StatusAilmentChara.IsHavingAilment(nType))
                {
                    return true;
                }
            }
        }

        return false;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		状態異常効果チェック：指定都合（消せる状態異常があるかどうかを調べている？）
        @param[in]	int		(charaIdx)		スキル所持者ID	※MasterDataSkillLimitBreakでもいい気がするが、上記のチェック仕様に合わす
        @retval		bool	[効果中/効果中の敵はいない]
        元あった場所：InGameUtil.cs 内の ChkLimitbreakAilmentStatusCondition()
    */
    //------------------------------------------------------------------------
    static public bool ChkLimitbreakAilmentStatusCondition(MasterDataSkillLimitBreak limit_break_skill, CharaParty player_party, BattleEnemy[] enemys)
    {

        bool result = false;

        //------------------------------
        //	エラーチェック
        //------------------------------
        if (limit_break_skill == null
        || player_party == null
        || enemys == null)
        {
            return (result);
        }

        // 都合が良い/悪いが未設定の場合（現状処理しない	2015/04/15）
        if (limit_break_skill.Get_SUPPORT_SPESTATE_CLEAR_GOOD_BAD() == StatusAilment.GoodOrBad.NONE)
        {
            return (result);
        }


        switch (limit_break_skill.Get_SUPPORT_SPESTATE_CLEAR_TARGET())
        {
            case MasterDataDefineLabel.TargetType.SELF:     // 自分
            case MasterDataDefineLabel.TargetType.FRIEND:   // 使用者の味方全員
                                                            // プレイヤーの指定都合状態異常をチェック
                result = player_party.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX).ChkStatusAilmentClearType(limit_break_skill.Get_SUPPORT_SPESTATE_CLEAR_GOOD_BAD());
                break;

            case MasterDataDefineLabel.TargetType.OTHER:    // 相手
            case MasterDataDefineLabel.TargetType.ENEMY:    // 使用者の敵全員
            case MasterDataDefineLabel.TargetType.ENE_N_1:
            case MasterDataDefineLabel.TargetType.ENE_1N_1:
            case MasterDataDefineLabel.TargetType.ENE_R_N:
            case MasterDataDefineLabel.TargetType.ENE_1_N:
                // 敵全サーチ
                for (int num = 0; num < enemys.Length; ++num)
                {
                    if (enemys[num] == null)
                    {
                        continue;
                    }

                    // 敵の指定都合状態異常をチェック
                    result = enemys[num].m_StatusAilmentChara.ChkStatusAilmentClearType(limit_break_skill.Get_SUPPORT_SPESTATE_CLEAR_GOOD_BAD());
                    // 1体でも該当するならチェック終了
                    if (result == true)
                    {
                        break;
                    }
                }
                break;

            case MasterDataDefineLabel.TargetType.ALL:      // 全員
                                                            // プレイヤーの指定都合状態異常をチェック
                result = player_party.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX).ChkStatusAilmentClearType(limit_break_skill.Get_SUPPORT_SPESTATE_CLEAR_GOOD_BAD());
                if (result == true)
                {
                    break;
                }

                // 敵全サーチ
                for (int num = 0; num < enemys.Length; ++num)
                {
                    if (enemys[num] == null)
                    {
                        continue;
                    }

                    // 敵の指定都合状態異常をチェック(1体でも該当するなら発動可能)
                    result = enemys[num].m_StatusAilmentChara.ChkStatusAilmentClearType(limit_break_skill.Get_SUPPORT_SPESTATE_CLEAR_GOOD_BAD());
                    // 1体でも該当するならチェック終了
                    if (result == true)
                    {
                        break;
                    }
                }
                break;
        }

        return (result);
    }

    //------------------------------------------------------------------------
    /*!
        @brief		戦闘中の敵に指定属性の敵が含まれているかチェック
        @param[in]	int			(element)		指定属性
        @param[in]	int			(target)		ターゲット
        @retval		bool		[指定属性の敵が含まれている/いない]
        元あった場所：InGameUtil.cs 内の ChkSpecificElementEnemy()
    */
    //------------------------------------------------------------------------
    public static bool ChkSpecificElementEnemy(MasterDataDefineLabel.ElementType element, BattleEnemy[] enemys, int target)
    {
        bool ret = false;

        if (enemys == null)
        {
            return ret;
        }

        if (target == InGameDefine.SELECT_NONE)
        {
            //----------------------------------------
            //	全体を対象として判定(ターゲット指定なし)
            //----------------------------------------
            for (int i = 0; i < enemys.Length; i++)
            {
                BattleEnemy battleEnemy = enemys[i];
                if (battleEnemy == null)
                {
                    continue;
                }

                if (battleEnemy.isDead() == true)
                {
                    continue;
                }

                if (battleEnemy.getMasterDataParamChara() == null)
                {
                    continue;
                }

                if (battleEnemy.getMasterDataParamChara().element != element)
                {
                    continue;
                }

                ret = true;
                break;
            }
        }
        else
        {
            //----------------------------------------
            //	指定対象で判定
            //----------------------------------------
            BattleEnemy battleEnemy = enemys[target];
            if (battleEnemy != null)
            {
                if (battleEnemy.isDead() != true)
                {
                    if (battleEnemy.getMasterDataParamChara() != null)
                    {
                        if (battleEnemy.getMasterDataParamChara().element == element)
                        {
                            ret = true;
                        }
                    }
                }
            }
        }
        return ret;
    }

    [Serializable]
    public class MultiInt
    {
        // 各メンバーの値（publicになっていますがここへは直接はアクセスしないでください（publicでないとJsonにできないため。[SerializeField]が効かない…LitJsonだから？））
        public int[] m_MemberValues;

        public MultiInt()
        {
            m_MemberValues = new int[(int)GlobalDefine.PartyCharaIndex.MAX];
        }

        public MultiInt(int value)
        {
            m_MemberValues = new int[(int)GlobalDefine.PartyCharaIndex.MAX];
            setValue(GlobalDefine.PartyCharaIndex.MAX, value);
        }

        public MultiInt(MultiInt value)
        {
            m_MemberValues = new int[(int)GlobalDefine.PartyCharaIndex.MAX];
            setValue(GlobalDefine.PartyCharaIndex.MAX, value);
        }

        public MultiFloat toMultiFloat()
        {
            MultiFloat ret_val = new MultiFloat();
            for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
            {
                ret_val.setValue((GlobalDefine.PartyCharaIndex)idx, m_MemberValues[idx]);
            }
            return ret_val;
        }


        public int getMemberCount()
        {
            return m_MemberValues.Length;
        }

        public void setValue(GlobalDefine.PartyCharaIndex member_index, int value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] = value;
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] = value;
                }
            }
        }

        public void setValue(GlobalDefine.PartyCharaIndex member_index, MultiInt value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] = value.m_MemberValues[idx];
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] = value.m_MemberValues[(int)member_index];
                }
            }
        }

        public void minValue(GlobalDefine.PartyCharaIndex member_index, int value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    if (m_MemberValues[idx] > value)
                    {
                        m_MemberValues[idx] = value;
                    }
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    if (m_MemberValues[(int)member_index] > value)
                    {
                        m_MemberValues[(int)member_index] = value;
                    }
                }
            }
        }

        public void minValue(GlobalDefine.PartyCharaIndex member_index, MultiInt value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < m_MemberValues.Length; idx++)
                {
                    if (m_MemberValues[idx] > value.m_MemberValues[idx])
                    {
                        m_MemberValues[idx] = value.m_MemberValues[idx];
                    }
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    if (m_MemberValues[(int)member_index] > value.m_MemberValues[(int)member_index])
                    {
                        m_MemberValues[(int)member_index] = value.m_MemberValues[(int)member_index];
                    }
                }
            }
        }

        public void maxValue(GlobalDefine.PartyCharaIndex member_index, int value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    if (m_MemberValues[idx] < value)
                    {
                        m_MemberValues[idx] = value;
                    }
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    if (m_MemberValues[(int)member_index] < value)
                    {
                        m_MemberValues[(int)member_index] = value;
                    }
                }
            }
        }

        public void maxValue(GlobalDefine.PartyCharaIndex member_index, MultiInt value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < m_MemberValues.Length; idx++)
                {
                    if (m_MemberValues[idx] < value.m_MemberValues[idx])
                    {
                        m_MemberValues[idx] = value.m_MemberValues[idx];
                    }
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    if (m_MemberValues[(int)member_index] < value.m_MemberValues[(int)member_index])
                    {
                        m_MemberValues[(int)member_index] = value.m_MemberValues[(int)member_index];
                    }
                }
            }
        }

        public int getValue(GlobalDefine.PartyCharaIndex member_index)
        {
            if (BattleParam.IsKobetsuHP == false)
            {
                return m_MemberValues[0];
            }

            if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
            {
                return m_MemberValues[(int)member_index];
            }

            int ret_val = 0;
            for (int idx = 0; idx < m_MemberValues.Length; idx++)
            {
                ret_val += m_MemberValues[idx];
            }
            return ret_val;
        }

        public void addValue(GlobalDefine.PartyCharaIndex member_index, int value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] += value;
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] += value;
                }
            }
        }

        public void addValue(GlobalDefine.PartyCharaIndex member_index, MultiInt value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] += value.m_MemberValues[idx];
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] += value.m_MemberValues[(int)member_index];
                }
            }
        }

        /// <summary>
        /// 加算（加算される方がゼロ以上の場合のみ加算。生存者のみを回復したい等の用途で使用）
        /// </summary>
        /// <param name="member_index"></param>
        /// <param name="value"></param>
        public void addValueAliveOnly(GlobalDefine.PartyCharaIndex member_index, int value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    if (m_MemberValues[idx] > 0)
                    {
                        m_MemberValues[idx] += value;
                    }
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    if (m_MemberValues[(int)member_index] > 0)
                    {
                        m_MemberValues[(int)member_index] += value;
                    }
                }
            }
        }

        /// <summary>
        /// 加算（加算される方がゼロ以上の場合のみ加算。生存者のみを回復したい等の用途で使用）
        /// </summary>
        /// <param name="member_index"></param>
        /// <param name="value"></param>
        public void addValueAliveOnly(GlobalDefine.PartyCharaIndex member_index, MultiInt value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    if (m_MemberValues[idx] > 0)
                    {
                        m_MemberValues[idx] += value.m_MemberValues[idx];
                    }
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    if (m_MemberValues[(int)member_index] > 0)
                    {
                        m_MemberValues[(int)member_index] += value.m_MemberValues[(int)member_index];
                    }
                }
            }
        }

        public void subValue(GlobalDefine.PartyCharaIndex member_index, int value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] -= value;
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] -= value;
                }
            }
        }

        public void subValue(GlobalDefine.PartyCharaIndex member_index, MultiInt value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] -= value.m_MemberValues[idx];
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] -= value.m_MemberValues[(int)member_index];
                }
            }
        }

        public void mulValue(GlobalDefine.PartyCharaIndex member_index, int value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] *= value;
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] *= value;
                }
            }
        }

        public void mulValue(GlobalDefine.PartyCharaIndex member_index, MultiInt value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] *= value.getValue((GlobalDefine.PartyCharaIndex)idx);
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] *= value.getValue(member_index);
                }
            }
        }

        public void mulValueF(GlobalDefine.PartyCharaIndex member_index, float value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] = (int)InGameUtilBattle.AvoidErrorMultiple(m_MemberValues[idx], value);
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] = (int)InGameUtilBattle.AvoidErrorMultiple(m_MemberValues[(int)member_index], value);
                }
            }
        }

        public void mulValueF(GlobalDefine.PartyCharaIndex member_index, MultiFloat value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] = (int)InGameUtilBattle.AvoidErrorMultiple(m_MemberValues[idx], value.getValue((GlobalDefine.PartyCharaIndex)idx));
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] = (int)InGameUtilBattle.AvoidErrorMultiple(m_MemberValues[(int)member_index], value.getValue(member_index));
                }
            }
        }

        public string getDebugString(GlobalDefine.PartyCharaIndex member_index)
        {
#if BUILD_TYPE_DEBUG
            string ret_val = "";
            if (member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                ret_val += "[";
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    ret_val += ((GlobalDefine.PartyCharaIndex)idx).ToString()
                        + ":" + m_MemberValues[idx].ToString() + " ";
                }
                ret_val += "]";
            }
            else
            {
                ret_val += member_index.ToString()
                    + ":" + m_MemberValues[(int)member_index].ToString();
            }

            return ret_val;
#else
            return null;
#endif
        }
    }

    [Serializable]
    public class MultiFloat
    {
        [SerializeField]
        private float[] m_MemberValues;

        public MultiFloat()
        {
            m_MemberValues = new float[(int)GlobalDefine.PartyCharaIndex.MAX];
        }

        public MultiFloat(float value)
        {
            m_MemberValues = new float[(int)GlobalDefine.PartyCharaIndex.MAX];
            for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
            {
                m_MemberValues[idx] = value;
            }
        }

        public MultiFloat(MultiFloat value)
        {
            m_MemberValues = new float[(int)GlobalDefine.PartyCharaIndex.MAX];
            setValue(GlobalDefine.PartyCharaIndex.MAX, value);
        }

        public MultiInt toMultiInt()
        {
            MultiInt ret_val = new MultiInt();
            for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
            {
                ret_val.setValue((GlobalDefine.PartyCharaIndex)idx, (int)m_MemberValues[idx]);
            }
            return ret_val;
        }

        public int getMemberCount()
        {
            return m_MemberValues.Length;
        }

        public void setValue(GlobalDefine.PartyCharaIndex member_index, float value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] = value;
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] = value;
                }
            }
        }

        public void setValue(GlobalDefine.PartyCharaIndex member_index, MultiFloat value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] = value.m_MemberValues[idx];
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] = value.m_MemberValues[(int)member_index];
                }
            }
        }

        public void minValue(GlobalDefine.PartyCharaIndex member_index, float value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    if (m_MemberValues[idx] > value)
                    {
                        m_MemberValues[idx] = value;
                    }
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    if (m_MemberValues[(int)member_index] > value)
                    {
                        m_MemberValues[(int)member_index] = value;
                    }
                }
            }
        }

        public void minValue(GlobalDefine.PartyCharaIndex member_index, MultiFloat value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    if (m_MemberValues[idx] > value.m_MemberValues[idx])
                    {
                        m_MemberValues[idx] = value.m_MemberValues[idx];
                    }
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    if (m_MemberValues[(int)member_index] > value.m_MemberValues[(int)member_index])
                    {
                        m_MemberValues[(int)member_index] = value.m_MemberValues[(int)member_index];
                    }
                }
            }
        }

        public void maxValue(GlobalDefine.PartyCharaIndex member_index, float value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    if (m_MemberValues[idx] < value)
                    {
                        m_MemberValues[idx] = value;
                    }
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    if (m_MemberValues[(int)member_index] < value)
                    {
                        m_MemberValues[(int)member_index] = value;
                    }
                }
            }
        }

        public void maxValue(GlobalDefine.PartyCharaIndex member_index, MultiFloat value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    if (m_MemberValues[idx] < value.m_MemberValues[idx])
                    {
                        m_MemberValues[idx] = value.m_MemberValues[idx];
                    }
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    if (m_MemberValues[(int)member_index] < value.m_MemberValues[(int)member_index])
                    {
                        m_MemberValues[(int)member_index] = value.m_MemberValues[(int)member_index];
                    }
                }
            }
        }

        public float getValue(GlobalDefine.PartyCharaIndex member_index)
        {
            if (BattleParam.IsKobetsuHP == false)
            {
                return m_MemberValues[0];
            }

            if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
            {
                return m_MemberValues[(int)member_index];
            }

            float ret_val = 0.0f;
            for (int idx = 0; idx < m_MemberValues.Length; idx++)
            {
                ret_val += m_MemberValues[idx];
            }
            return ret_val;
        }

        public void addValue(GlobalDefine.PartyCharaIndex member_index, float value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] += value;
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] += value;
                }
            }
        }

        public void addValue(GlobalDefine.PartyCharaIndex member_index, MultiFloat value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] += value.m_MemberValues[idx];
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] += value.m_MemberValues[(int)member_index];
                }
            }
        }

        public void subValue(GlobalDefine.PartyCharaIndex member_index, float value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] -= value;
                    InGameUtilBattle.AvoidError(ref m_MemberValues[idx]);
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] -= value;
                    InGameUtilBattle.AvoidError(ref m_MemberValues[(int)member_index]);
                }
            }
        }

        public void subValue(GlobalDefine.PartyCharaIndex member_index, MultiFloat value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] -= value.m_MemberValues[idx];
                    InGameUtilBattle.AvoidError(ref m_MemberValues[idx]);
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] -= value.m_MemberValues[(int)member_index];
                    InGameUtilBattle.AvoidError(ref m_MemberValues[(int)member_index]);
                }
            }
        }

        public void mulValue(GlobalDefine.PartyCharaIndex member_index, float value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] = InGameUtilBattle.AvoidErrorMultiple(m_MemberValues[idx], value);
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] = InGameUtilBattle.AvoidErrorMultiple(m_MemberValues[(int)member_index], value);
                }
            }
        }

        public void mulValue(GlobalDefine.PartyCharaIndex member_index, MultiFloat value)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    m_MemberValues[idx] = InGameUtilBattle.AvoidErrorMultiple(m_MemberValues[idx], value.getValue((GlobalDefine.PartyCharaIndex)idx));
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index] = InGameUtilBattle.AvoidErrorMultiple(m_MemberValues[(int)member_index], value.getValue(member_index));
                }
            }
        }

#if BUILD_TYPE_DEBUG
        public string getDebugString(GlobalDefine.PartyCharaIndex member_index, int scale)
        {
            string ret_val = "";
            if (member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                ret_val += "[";
                for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                {
                    float val = m_MemberValues[idx] * scale;
                    InGameUtilBattle.AvoidError(ref val);
                    ret_val += ((GlobalDefine.PartyCharaIndex)idx).ToString()
                        + ":" + ((int)val).ToString() + " ";
                }
                ret_val += "]";
            }
            else
            {
                float val = m_MemberValues[(int)member_index] * scale;
                InGameUtilBattle.AvoidError(ref val);
                ret_val += member_index.ToString()
                    + ":" + ((int)val).ToString();
            }

            return ret_val;
        }
#endif
    }

    /// <summary>
    /// パーティメンバーの状態異常をまとめたもの。
    /// ※この構造体はローカルセーブでセーブされます
    /// </summary>
    [Serializable]
    public class MultiAilment
    {
        public StatusAilmentChara[] m_MemberValues; // パーティメンバー個別の値
        private StatusAilmentChara m_AilmentAllMember = new StatusAilmentChara(StatusAilmentChara.OwnerType.PLAYER);    //全員分をまとめたもの

        public MultiAilment()
        {
            m_MemberValues = new StatusAilmentChara[(int)GlobalDefine.PartyCharaIndex.MAX];
            for (int idx = 0; idx < m_MemberValues.Length; idx++)
            {
                m_MemberValues[idx] = new StatusAilmentChara(StatusAilmentChara.OwnerType.PLAYER);
            }
        }

        public StatusAilmentChara getAilment(GlobalDefine.PartyCharaIndex member_index)
        {
            if (BattleParam.IsKobetsuHP == false)
            {
                return m_MemberValues[0];
            }

            if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
            {
                return m_MemberValues[(int)member_index];
            }

            // すべての状態異常をまとめたものを返す.
            m_AilmentAllMember.ClearChara();
            for (int member_idx = 0; member_idx < m_MemberValues.Length; member_idx++)
            {
                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)member_idx, CharaParty.CharaCondition.ALIVE);
                if (chara_once != null)
                {
                    for (int ailment_idx = 0; ailment_idx < m_MemberValues[member_idx].GetAilmentCount(); ailment_idx++)
                    {
                        StatusAilment ailment = m_MemberValues[member_idx].GetAilment(ailment_idx);
                        if (ailment != null)
                        {
                            m_AilmentAllMember.AddStatusAilment(ailment
                                , null
#if BUILD_TYPE_DEBUG
                                , false //デバッグログが出ないようにする
#endif //BUILD_TYPE_DEBUG
                            );
                        }
                    }
                }
            }
            return m_AilmentAllMember;
        }

        public void UpdateTurnOnce(StatusAilmentChara.UpdateTiming eTiming)
        {
            for (int idx = 0; idx < m_MemberValues.Length; idx++)
            {
                m_MemberValues[idx].UpdateTurnOnce(eTiming);
            }

            updatePartyAilment();
        }

        /// <summary>
        /// パーティにかかる状態異常は、だれか一人でも持っていたら全員にコピーする.
        /// </summary>
        public void updatePartyAilment()
        {
            // パーティにかかる状態異常一覧
            MasterDataDefineLabel.AilmentType[] party_ailment_types =
            {
                MasterDataDefineLabel.AilmentType.TIMER,
                MasterDataDefineLabel.AilmentType.HANDCARD_DEFAULT,
                MasterDataDefineLabel.AilmentType.AUTO_PLAY_SKILL,
            };

            List<StatusAilment> party_ailments = new List<StatusAilment>();

            for (int member_idx = 0; member_idx < m_MemberValues.Length; member_idx++)
            {
                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)member_idx, CharaParty.CharaCondition.ALIVE);
                if (chara_once != null)
                {
                    for (int ailment_idx = 0; ailment_idx < m_MemberValues[member_idx].GetAilmentCount(); ailment_idx++)
                    {
                        StatusAilment ailment = m_MemberValues[member_idx].GetAilment(ailment_idx);
                        if (ailment != null)
                        {
                            for (int ailment_type_idx = 0; ailment_type_idx < party_ailment_types.Length; ailment_type_idx++)
                            {
                                MasterDataDefineLabel.AilmentType party_ailment_type = party_ailment_types[ailment_type_idx];
                                if (ailment.nType == party_ailment_type)
                                {
                                    party_ailments.Add(ailment);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            foreach (StatusAilment party_ailment in party_ailments)
            {
                for (int member_idx = 0; member_idx < m_MemberValues.Length; member_idx++)
                {
                    CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)member_idx, CharaParty.CharaCondition.ALIVE);
                    if (chara_once != null)
                    {
                        m_MemberValues[member_idx].AddStatusAilment(party_ailment, chara_once.m_CharaMasterDataParam);
                    }
                }
            }
        }

        public void DelAllStatusAilment(GlobalDefine.PartyCharaIndex member_index)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < m_MemberValues.Length; idx++)
                {
                    m_MemberValues[idx].DelAllStatusAilment();
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    m_MemberValues[(int)member_index].DelAllStatusAilment();
                }
            }
        }

        /// <summary>
        /// プレイヤーパーティに状態異常を与える
        /// </summary>
        /// <param name="member_index">対象パーティメンバー</param>
        /// <param name="status_ailment_id">状態異常ＩＤ</param>
        /// <param name="attacker_base_attack">攻撃者の攻撃力</param>
        /// <param name="defender_base_hpmax">プレイヤーパーティのＨＰ</param>
        public void AddStatusAilmentToPlayerParty(GlobalDefine.PartyCharaIndex member_index,
            int status_ailment_id, int attacker_base_attack, BattleSceneUtil.MultiInt defender_base_hpmax)
        {
            if (BattleParam.IsKobetsuHP == false || member_index == GlobalDefine.PartyCharaIndex.MAX)
            {
                for (int idx = 0; idx < m_MemberValues.Length; idx++)
                {
                    CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.ALIVE);
                    if (chara_once != null)
                    {
                        m_MemberValues[idx].AddStatusAilment(status_ailment_id, attacker_base_attack, defender_base_hpmax.getValue((GlobalDefine.PartyCharaIndex)idx), chara_once.m_CharaMasterDataParam);
                    }
                }
            }
            else
            {
                if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
                {
                    CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)member_index, CharaParty.CharaCondition.ALIVE);
                    if (chara_once != null)
                    {
                        m_MemberValues[(int)member_index].AddStatusAilment(status_ailment_id, attacker_base_attack, defender_base_hpmax.getValue((GlobalDefine.PartyCharaIndex)member_index), chara_once.m_CharaMasterDataParam);
                    }
                }
            }

            updatePartyAilment();
        }

        public void DelStatusAilmentCondition(StatusAilment.GoodOrBad ailment_goodOrbad)
        {
            for (int idx = 0; idx < m_MemberValues.Length; idx++)
            {
                m_MemberValues[idx].DelStatusAilmentCondition(ailment_goodOrbad);
            }
        }

        public void restoreFromSaveData()
        {
            for (int idx = 0; idx < m_MemberValues.Length; idx++)
            {
                m_MemberValues[idx].restoreFromSaveData();
            }
        }
    }

    /// <summary>
    /// 体力に応じて攻撃力ＵＰ
    /// </summary>
    /// <param name="border_rate_min">攻撃力ＵＰする体力の最小割合</param>
    /// <param name="border_rate_max">攻撃力ＵＰする体力の最大割合</param>
    /// <param name="full_life_rate">攻撃力の倍率</param>
    /// <returns></returns>
    public static BattleSceneUtil.MultiFloat getLifeRatePow(float border_rate_min, float border_rate_max, float full_life_rate)
    {
        BattleSceneUtil.MultiFloat ret_val = new BattleSceneUtil.MultiFloat(1.0f);

        for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
        {
            int hp = BattleParam.m_PlayerParty.m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx);
            int hp_max = BattleParam.m_PlayerParty.m_HPMax.getValue((GlobalDefine.PartyCharaIndex)idx);
            if (hp >= (int)(hp_max * border_rate_min) && hp <= (int)(hp_max * border_rate_max))
            {
                ret_val.setValue((GlobalDefine.PartyCharaIndex)idx, full_life_rate);
            }
        }

        return ret_val;
    }


    /// <summary>
    /// パーセント判定
    /// </summary>
    /// <param name="percent">判定したい確率（％）</param>
    /// <param name="percent_max">確率の最大値（パーセントなら100）</param>
    /// <returns></returns>
    public static bool checkChancePercent(int percent, int percent_max = 100)
    {
        uint rand_val = RandManager.GetRand(0, (uint)percent_max);
        bool ret_val = (percent > rand_val);
        return ret_val;
    }

    /// <summary>
    /// スキルの発動率判定
    /// </summary>
    /// <param name="percent">判定したい確率（％）</param>
    /// <param name="percent_max">確率の最大値（パーセントなら100）</param>
    /// <returns></returns>
    public static bool checkChancePercentSkill(int percent, int percent_max = 100)
    {
#if BUILD_TYPE_DEBUG
        // スキル発動率を１００％に
        if (BattleParam.m_DebugForce100PercentSkill)
        {
            return true;
        }
#endif
        return checkChancePercent(percent, percent_max);
    }

    /// <summary>
    /// アスペクト比によってカメラ情報や敵領域の高さを計算
    /// </summary>
    public static void getAdjustScreenInfo(ref float camera_fov, ref Quaternion camera_rotation)
    {
        float[] x_params = { 640.0f / 1280.0f, 640.0f / 1136.0f, 640.0f / 960.0f };
        float[] fovs = { 37.8f, 33.8f, 28.6f };
        float[] degs = { -4.84f, -2.79f, -0.14f };

        float asprect = Screen.width / (float)Screen.height;

        if (asprect > MAX_ASPECT)
        {
            asprect = MAX_ASPECT;
        }

        for (int idx = 0; idx <= 1; idx++)
        {
            float t = (asprect - x_params[idx]) / (x_params[idx + 1] - x_params[idx]);

            if ((idx == 0 && t <= 1.0f)
                || (idx == 1)
            )
            {
                camera_fov = (fovs[idx + 1] - fovs[idx]) * t + fovs[idx];

                float deg = (degs[idx + 1] - degs[idx]) * t + degs[idx];
                camera_rotation = Quaternion.Euler(new Vector3(deg, 0.0f, 0.0f));

                break;
            }
        }
    }
} // class BattleUtil

/// <summary>
/// バトル中の実績用情報を集計（クエスト終了時に集計）
/// ※バトルアチーブとは別物
/// ※クエスト側で既に持っている情報は集計しない
/// </summary>
[Serializable]
public class AchievementTotalingInBattle
{
    // 中断復帰用にセーブする必要がありそうなので public
    public int m_PlayerDamageTotal; // プレイヤーが受けたダメージ合計
    public int m_EnemyDamageMax;    // 敵が受けたダメージ最大値
    public int m_EnemyDamageTotal;  // 敵が受けたダメージ合計
    public int[] m_KillEnemyFixIdArray = null;  // 倒した敵の配列(fix_idが入っている)
    public int[] m_UseLimitBreakFixIdArray = null;  // 使用したアクティブスキルの配列(fix_idが入っている)
    public int m_HeroSkillUseCount; // 主人公スキルの使用回数.


    public class EnemyKillCountInfo
    {
        public int m_EnemyFixID;    // 敵ＩＤ
        public int m_KillCount; // 撃破数
    }

    public class LimitBreakSkillUseInfo
    {
        public int m_LimitBreakSkillID; // アクティブ（リミブレ）スキルのFixID
        public int m_UseCount;  // 使用回数
    }

    /// <summary>
    /// クエスト開始時に呼ぶ（バトル開始時ではない）
    /// </summary>
    public void initQuset()
    {
        m_PlayerDamageTotal = 0;
        m_EnemyDamageMax = 0;
        m_EnemyDamageTotal = 0;
        m_KillEnemyFixIdArray = new int[0];
        m_UseLimitBreakFixIdArray = new int[0];
        m_HeroSkillUseCount = 0;
    }

    // プレイヤーがダメージを受けた時に呼ぶ（ノーダメージでクエストクリアしたかを判定するのが目的）
    public void damagePlayer(int damage)
    {
        m_PlayerDamageTotal += damage;
    }

    // 敵がダメージを受けた時に呼ぶ（最大値を集計するのが目的）(クエスト中の合計ダメージ)
    public void damageEnemy(int damage)
    {
        m_EnemyDamageMax = Mathf.Max(m_EnemyDamageMax, damage);
        m_EnemyDamageTotal += damage;
    }

    // 敵を撃破したときに呼ぶ（敵撃破数の集計のため）
    public void killEnemy(int enemy_fix_id)
    {
        int[] old_array = m_KillEnemyFixIdArray;
        m_KillEnemyFixIdArray = new int[old_array.Length + 1];

        for (int idx = 0; idx < old_array.Length; idx++)
        {
            m_KillEnemyFixIdArray[idx] = old_array[idx];
        }

        m_KillEnemyFixIdArray[m_KillEnemyFixIdArray.Length - 1] = enemy_fix_id;
    }

    // アクティブ（リミブレ）スキルを使用したときに呼ぶ
    public void useLimitBreakSkill(int active_skill_id)
    {
        int[] old_array = m_UseLimitBreakFixIdArray;
        m_UseLimitBreakFixIdArray = new int[old_array.Length + 1];

        for (int idx = 0; idx < old_array.Length; idx++)
        {
            m_UseLimitBreakFixIdArray[idx] = old_array[idx];
        }

        m_UseLimitBreakFixIdArray[m_UseLimitBreakFixIdArray.Length - 1] = active_skill_id;
    }

    // ヒーロスキルを使用したときに呼ぶ
    public void useHeroSkill()
    {
        m_HeroSkillUseCount++;
    }


    /// <summary>
    /// 敵撃破数を取得
    /// </summary>
    /// <returns></returns>
    public EnemyKillCountInfo[] getEnemyKillCount()
    {
        Dictionary<int, int> info_array = new Dictionary<int, int>();

        for (int idx = 0; idx < m_KillEnemyFixIdArray.Length; idx++)
        {
            int fix_id = m_KillEnemyFixIdArray[idx];

            if (info_array.ContainsKey(fix_id))
            {
                info_array[fix_id]++;
            }
            else
            {
                info_array[fix_id] = 1;
            }
        }

        EnemyKillCountInfo[] ret_val = new EnemyKillCountInfo[info_array.Count];
        int index = 0;
        foreach (KeyValuePair<int, int> pair in info_array)
        {
            EnemyKillCountInfo wrk_info = new EnemyKillCountInfo();
            wrk_info.m_EnemyFixID = pair.Key;
            wrk_info.m_KillCount = pair.Value;

            ret_val[index] = wrk_info;
            index++;
        }

        return ret_val;
    }

    /// <summary>
    /// プレイヤーがノーダメージか
    /// </summary>
    /// <returns></returns>
    public bool isNoDamagePlayer()
    {
        bool ret_val = (m_PlayerDamageTotal == 0);
        return ret_val;
    }

    /// <summary>
    /// 敵に与えた最大ダメージを取得
    /// </summary>
    /// <returns></returns>
    public int getMaxDamageToEnemy()
    {
        return m_EnemyDamageMax;
    }

    /// <summary>
    /// アクティブ（リミブレ）スキル使用回数を取得
    /// </summary>
    /// <returns></returns>
    public LimitBreakSkillUseInfo[] getLimitBreakSkillUse()
    {
        Dictionary<int, int> info_array = new Dictionary<int, int>();

        for (int idx = 0; idx < m_UseLimitBreakFixIdArray.Length; idx++)
        {
            int fix_id = m_UseLimitBreakFixIdArray[idx];

            if (info_array.ContainsKey(fix_id))
            {
                info_array[fix_id]++;
            }
            else
            {
                info_array[fix_id] = 1;
            }
        }

        LimitBreakSkillUseInfo[] ret_val = new LimitBreakSkillUseInfo[info_array.Count];
        int index = 0;
        foreach (KeyValuePair<int, int> pair in info_array)
        {
            LimitBreakSkillUseInfo wrk_info = new LimitBreakSkillUseInfo();
            wrk_info.m_LimitBreakSkillID = pair.Key;
            wrk_info.m_UseCount = pair.Value;

            ret_val[index] = wrk_info;
            index++;
        }

        return ret_val;
    }

    /// <summary>
    /// ヒーロースキル使用回数
    /// </summary>
    /// <returns></returns>
    public int getHeroSkillUseCount()
    {
        return m_HeroSkillUseCount;
    }
}

