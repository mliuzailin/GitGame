using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleHeroParam
{
    [Tooltip("レベル")]
    public int m_Level = 1;

    [Tooltip("必殺技ＩＤ（リミットブレイクＩＤ）\nリミットブレイクスキルの「攻撃の固定値」にレベルを掛けたダメージを与えます")]
    public int m_HeroSkillID = 0;
}

public class BattleHero
{
    private const int HERO_SKILL_ID = 0x7ffffffd;   //ヒーロースキルのＩＤ（リミットブレイクスキルＩＤと被らなそうな適当な数値）
    private const int HERO_SKILL_ATK_ONLY_ID = 0x7ffffffe;  // 追加を無くしたヒーロースキルのＩＤ（リミットブレイクスキルＩＤと被らなそうな適当な数値）

    private int m_Level = 0;    // ヒーローのレベル（１～９９）
    private uint m_HeroSkillID = 0; //　必殺技（リミットブレイクスキルＩＤ）
    private int m_SkillTurn = 0;    // 必殺技使用までの残りターン数
    private int m_RecastTurn = 0;   // 必殺技リキャストターン数
    private int m_MaxAddSkillTurnPerBattleTurn; // １戦闘ターンあたりに消化できる最大スキルターン数

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="hero_level">ヒーローのレベル</param>
    /// <param name="limit_break_skill_id">必殺技ＩＤ（リミットブレイクスキルＩＤ）</param>
    public BattleHero(int hero_level, uint limit_break_skill_id)
    {
        m_Level = hero_level;
        m_HeroSkillID = limit_break_skill_id;
        m_SkillTurn = 0;
        m_RecastTurn = 0;
    }

    /// <summary>
    /// 追加効果発動確率（％）
    /// </summary>
    /// <returns></returns>
    public int getAdditionalEffectPercent()
    {
        int ret_val = (((m_Level - 1) / 10) + 1) * 5;
        return ret_val;
    }

    public void resetBattleTurn(int max_add_trun = 4)
    {
        m_MaxAddSkillTurnPerBattleTurn = max_add_trun;
    }

    /// <summary>
    /// スキルターン数をリセット（スキル使用後のリセット）
    /// </summary>
    public void resetSkillTurn()
    {
        m_SkillTurn = m_RecastTurn;
    }

    /// <summary>
    /// スキルターンを消化
    /// </summary>
    /// <param name="skill_turn"></param>
    public void addSkillTrun(int skill_turn, bool is_limit_by_max_add_skill_turn)
    {
        if (is_limit_by_max_add_skill_turn)
        {
            if (skill_turn > m_MaxAddSkillTurnPerBattleTurn)
            {
                skill_turn = m_MaxAddSkillTurnPerBattleTurn;
                m_MaxAddSkillTurnPerBattleTurn = 0;
            }
            else
            {
                m_MaxAddSkillTurnPerBattleTurn -= skill_turn;
            }
        }

        m_SkillTurn -= skill_turn;
        if (m_SkillTurn < 0)
        {
            m_SkillTurn = 0;
        }
    }

    /// <summary>
    /// スキル発動可能になるまでのターン数
    /// </summary>
    /// <returns></returns>
    public int getSkillTurn()
    {
        return m_SkillTurn;
    }

    /// <summary>
    /// スキル発動可能になるまでのターン数
    /// </summary>
    /// <returns></returns>
    public int getSkillTurnMax()
    {
        return m_RecastTurn;
    }

    /// <summary>
    /// ヒーロースキル発動可能ターン数に達しているか
    /// </summary>
    /// <returns></returns>
    public bool checkHeroSkillTurn()
    {
        bool ret_val = (m_RecastTurn > 0 && m_SkillTurn <= 0);
        return ret_val;
    }

    /// <summary>
    /// ヒーロースキルが使用可能かどうか調べる
    /// </summary>
    /// <returns></returns>
    public bool checkHeroSkill()
    {
        return BattleParam.IsEnableLBS(GlobalDefine.PartyCharaIndex.HERO);
    }

    /// <summary>
    /// ヒーロースキルのfix_idを取得
    /// </summary>
    /// <param name="is_atk_only_skill"></param>
    /// <returns></returns>
    public uint getHeroSkillFixId(bool is_atk_only_skill)
    {
        if (is_atk_only_skill)
        {
            return HERO_SKILL_ATK_ONLY_ID;
        }
        else
        {
            return HERO_SKILL_ID;
        }
    }

    /// <summary>
    /// ヒーロースキルを作成（リミットブレイクスキルの一種として動作させてみる）
    /// </summary>
    public void createHeroSkill()
    {
        m_RecastTurn = 0;
        MasterDataSkillLimitBreak base_skill = BattleParam.m_MasterDataCache.useSkillLimitBreak(m_HeroSkillID);
        if (base_skill != null)
        {
            m_RecastTurn = base_skill.use_turn;

            MasterDataSkillLimitBreak hero_skill = new MasterDataSkillLimitBreak();   // 追加効果ありスキル
            hero_skill.Copy(base_skill);
            MasterDataSkillLimitBreak hero_skill_atk_only = new MasterDataSkillLimitBreak();  // 攻撃のみスキル
            hero_skill_atk_only.Copy(hero_skill);

            hero_skill.fix_id = HERO_SKILL_ID;
            hero_skill_atk_only.fix_id = HERO_SKILL_ATK_ONLY_ID;

            if (hero_skill.skill_absorb != 0
                || hero_skill.skill_kickback != 0
                || hero_skill.skill_kickback_fix != 0
                || hero_skill.skill_cate != MasterDataDefineLabel.SkillCategory.NONE
            )
            {
                // 追加効果がある場合
                hero_skill.name = hero_skill.name + "+追加効果";

                // 攻撃のみスキルの攻撃以外の効果を消す
                hero_skill_atk_only.skill_absorb = 0;   // 吸血効果
                hero_skill_atk_only.skill_kickback = 0; // 反動ダメージ
                hero_skill_atk_only.skill_kickback_fix = 0;  // 反動ダメージ

                hero_skill_atk_only.skill_cate = MasterDataDefineLabel.SkillCategory.NONE;
                hero_skill_atk_only.status_ailment1 = 0;
                hero_skill_atk_only.status_ailment2 = 0;
                hero_skill_atk_only.status_ailment3 = 0;
                hero_skill_atk_only.status_ailment4 = 0;
            }

            BattleParam.m_MasterDataCache.addSkillLimitBreak(hero_skill);
            BattleParam.m_MasterDataCache.addSkillLimitBreak(hero_skill_atk_only);
        }
    }

    /// <summary>
    /// ヒーロースキルのスキルアクティビティを取得
    /// </summary>
    /// <returns></returns>
    public BattleSkillActivity getHeroSkillActivity()
    {
        //--------------------------------
        //	追加効果の有無
        //--------------------------------
        uint hero_skill_id = HERO_SKILL_ATK_ONLY_ID;
        if (BattleSceneUtil.checkChancePercentSkill(getAdditionalEffectPercent()))
        {
            hero_skill_id = HERO_SKILL_ID;
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "主人公スキル追加効果：あり(追加効果発動確率:" + getAdditionalEffectPercent().ToString() + "%)");
        }
        else
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "主人公スキル追加効果：なし(追加効果発動確率:" + getAdditionalEffectPercent().ToString() + "%)");
        }

        MasterDataSkillLimitBreak param = BattleParam.m_MasterDataCache.useSkillLimitBreak(hero_skill_id);
        if (param == null)
        {
            return null;
        }

        //--------------------------------
        //	スキル発動情報
        //--------------------------------
        BattleSkillActivity activity = new BattleSkillActivity();
        activity.m_SkillParamOwnerNum = GlobalDefine.PartyCharaIndex.HERO;
        activity.m_SkillParamFieldID = InGameDefine.SELECT_NONE;
        activity.m_SkillParamSkillID = hero_skill_id;
        activity.m_Effect = param.skill_effect;
        activity.m_Type = param.skill_type;
        activity.m_Element = param.skill_elem;
        activity.m_SkillType = ESKILLTYPE.eLIMITBREAK;
        activity.m_Category_SkillCategory_PROPERTY = param.skill_cate;
        activity.m_SkillParamTarget = null;


        activity.m_skill_power = param.skill_power;
        activity.m_skill_power_fix = param.skill_power_fix;
        activity.m_skill_power_hp_rate = param.skill_power_hp_rate;

        activity.m_skill_absorb = param.skill_absorb;
        activity.m_skill_kickback = param.skill_kickback;
        activity.m_skill_kickback_fix = param.skill_kickback_fix;

        activity.m_skill_chk_atk_affinity = param.skill_chk_atk_affinity;
        activity.m_skill_chk_atk_leader = param.skill_chk_atk_leader;
        activity.m_skill_chk_atk_passive = param.skill_chk_atk_passive;
        activity.m_skill_chk_atk_ailment = param.skill_chk_atk_ailment;
        activity.m_skill_chk_atk_combo = MasterDataDefineLabel.BoolType.DISABLE;            // コンボレートはのらない

        activity.m_skill_chk_def_defence = param.skill_chk_def_defence;
        activity.m_skill_chk_def_ailment = param.skill_chk_def_ailment;
        activity.m_skill_chk_def_barrier = param.skill_chk_def_barrier;

        activity.m_statusAilment_target = param.status_ailment_target;
        activity.m_statusAilment = new int[] {   param.status_ailment1,
                                                            param.status_ailment2,
                                                            param.status_ailment3,
                                                            param.status_ailment4 };

        activity._setParam(param);

        return activity;
    }

    public void restoreSkillTurn(int skill_turn)
    {
        m_SkillTurn = skill_turn;
    }

    public int getLevel()
    {
        return m_Level;
    }

    public int getAtk()
    {
        // 攻撃力はＬＶそのまま
        return m_Level;
    }
}
