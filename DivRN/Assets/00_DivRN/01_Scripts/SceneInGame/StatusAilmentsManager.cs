using UnityEngine;
using System;
using System.Collections;

//============================================================================
//	class
//============================================================================
//----------------------------------------------------------------------------
/*!
    @class		StatusAilment
    @brief		管理用の状態異常情報クラス
    @note		ユーザー端末に保存されているデータなので変更禁止
*/
//----------------------------------------------------------------------------
[Serializable]
public class StatusAilment
{

    //------------------------------------------------
    // ※※※※※※※※※※※※※※※※※※※※※※
    // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
    // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
    // ※※※※※※※※※※※※※※※※※※※※※※
    //------------------------------------------------
    public int nMasterDataStatusAilmentID;                                          //!< 状態異常ID

    public GoodOrBad nGoodOrBad;                                                            //!< 状態異常良い効果か悪い効果か
    public MasterDataDefineLabel.AilmentType nType;                                                             //!< 状態異常タイプ
    public MasterDataDefineLabel.BoolType nUpdateTurnMove;                                                  //!< 移動中にターンの更新を行うか
    public MasterDataDefineLabel.BoolType nUpdateTurnBattle;                                                    //!< 戦闘中にターンの更新を行うか
    public int nLife;                                                               //!< 経過ターン
    public int nBaseAtk;                                                            //!< 基本攻撃力	//「攻撃者側」の攻撃力らしい
    public int nBaseHPMax;                                                          //!< 基本体力	//「被攻撃者側」の体力らしい
    public bool bUsed;                                                              //!< 使用中フラグ
    public bool bEdge;                                                              //!< 効果発動ターンフラグ

    public int nValueRate;  // 状態異常効果値（割合）
    public int nValueFix;   // 状態異常効果値（固定値）

    public enum GoodOrBad
    {
        NONE = 0,
        BAD = 1,
        GOOD = 2,
    };

    //------------------------------------------------------------------------
    //	@brief		コンストラクタ
    //------------------------------------------------------------------------
    public StatusAilment()
    {

        //--------------------------------
        // データクリア
        //--------------------------------
        ClearData();

    }


    //------------------------------------------------------------------------
    //	@brief		データクリア
    //------------------------------------------------------------------------
    public void ClearData()
    {

        nMasterDataStatusAilmentID = 0;
        nGoodOrBad = 0;
        nType = 0;
        nUpdateTurnMove = 0;
        nUpdateTurnBattle = 0;
        nLife = 0;
        nBaseAtk = 0;
        nBaseHPMax = 0;
        bUsed = false;
        bEdge = false;

        nValueRate = 0;
        nValueFix = 0;

    }


    //------------------------------------------------------------------------
    //	@brief		データ設定
    //------------------------------------------------------------------------
    public void SetupData(StatusAilment status_ailment)
    {
        nMasterDataStatusAilmentID = status_ailment.nMasterDataStatusAilmentID;
        nGoodOrBad = status_ailment.nGoodOrBad;
        nType = status_ailment.nType;
        nUpdateTurnMove = status_ailment.nUpdateTurnMove;
        nUpdateTurnBattle = status_ailment.nUpdateTurnBattle;
        nLife = status_ailment.nLife;
        nBaseAtk = status_ailment.nBaseAtk;
        nBaseHPMax = status_ailment.nBaseHPMax;
        bUsed = status_ailment.bUsed;
        bEdge = status_ailment.bEdge;

        nValueRate = status_ailment.nValueRate;
        nValueFix = status_ailment.nValueFix;

    }


    //------------------------------------------------------------------------
    //	@brief		データ設定
    //------------------------------------------------------------------------
    public void SetupData(StatusAilment.GoodOrBad good_or_bad,
                              MasterDataDefineLabel.AilmentType category,
                              MasterDataDefineLabel.BoolType updateTurnMove,
                              MasterDataDefineLabel.BoolType updateTurnBattle,
                            int duration_turn,
                            int base_attack,
                            int base_hpmax,
                            /*int[] status_ailment_param*/ int value_rate, int value_fix)
    {

        nGoodOrBad = good_or_bad;
        nType = category;
        nUpdateTurnMove = updateTurnMove;
        nUpdateTurnBattle = updateTurnBattle;
        nLife = duration_turn;
        nBaseAtk = base_attack;
        nBaseHPMax = base_hpmax;

        nValueRate = value_rate;
        nValueFix = value_fix;
    }


    /// <summary>
    /// データ設定
    /// </summary>
    /// <param name="status_ailment_param">状態異常データ</param>
    /// <param name="attacker_base_attack">攻撃者側の攻撃力</param>
    /// <param name="defender_base_hpmax">被攻撃者側の体力</param>
    public void _SetupData(MasterDataStatusAilmentParam status_ailment_param, int attacker_base_attack, int defender_base_hpmax)
    {

        //--------------------------------
        //	エラーチェック
        //--------------------------------
        if (status_ailment_param == null)
        {
            return;
        }


        //--------------------------------
        // データクリア
        //--------------------------------
        ClearData();


        //--------------------------------
        //	パラメータを入力
        //--------------------------------
        nMasterDataStatusAilmentID = (int)status_ailment_param.fix_id;
        nGoodOrBad = status_ailment_param.good_or_bad;
        nType = status_ailment_param.category;
        nLife = status_ailment_param.duration;
        nUpdateTurnMove = status_ailment_param.update_move;
        nUpdateTurnBattle = status_ailment_param.update_battle;
        nBaseAtk = attacker_base_attack;
        nBaseHPMax = defender_base_hpmax;

        nValueRate = status_ailment_param.Get_VALUE_RATE();
        nValueFix = status_ailment_param.Get_VALUE_FIX();
    }


    /// <summary>
    /// データ設定
    /// </summary>
    /// <param name="status_ailment_id">状態異常ＩＤ</param>
    /// <param name="attacker_base_attack">攻撃者側の攻撃力</param>
    /// <param name="defender_base_hpmax">被攻撃者側の体力</param>
    public void _SetupData(int status_ailment_id, int attacker_base_attack, int defender_base_hpmax)
    {

        MasterDataStatusAilmentParam status_ailment_param = BattleParam.m_MasterDataCache.useAilmentParam((uint)status_ailment_id);
        if (status_ailment_param == null)
        {
            return;
        }


        _SetupData(status_ailment_param, attacker_base_attack, defender_base_hpmax);
    }


    /// <summary>
    /// 状態異常ＩＤの状態異常の状態異常グループを取得.
    /// </summary>
    /// <param name="ailmentFixID"></param>
    /// <returns></returns>
    static public MasterDataDefineLabel.AilmentGroup getAilmentGroupFromAilmentID(int ailmentFixID)
    {
        MasterDataStatusAilmentParam ailmentMaster = null;
        MasterDataDefineLabel.AilmentGroup resultGroup = MasterDataDefineLabel.AilmentGroup.NONE;

        //------------------------------
        // マスターデータを取得
        //------------------------------
        ailmentMaster = BattleParam.m_MasterDataCache.useAilmentParam((uint)ailmentFixID);
        if (ailmentMaster == null)
        {
            return (resultGroup);
        }

        //------------------------------
        // グループを取得
        //------------------------------
        resultGroup = getAilmentGroupFromAilmentType(ailmentMaster.category);

        return (resultGroup);
    }


    /// <summary>
    /// 状態異常のカテゴリから状態異常グループを取得
    /// </summary>
    /// <param name="ailmentID"></param>
    /// <returns></returns>
    static public MasterDataDefineLabel.AilmentGroup getAilmentGroupFromAilmentType(MasterDataDefineLabel.AilmentType ailmentID)
    {
        MasterDataDefineLabel.AilmentGroup resultGroup = MasterDataDefineLabel.AilmentGroup.NONE;


        //------------------------------
        // カテゴリによる分岐
        //------------------------------
        switch (ailmentID)
        {
            //------------------------------
            // 未設定
            //------------------------------
            case MasterDataDefineLabel.AilmentType.NONE:
                break;

            //------------------------------
            // 毒
            //------------------------------
            case MasterDataDefineLabel.AilmentType.POISON:              // 毒
            case MasterDataDefineLabel.AilmentType.POISON_MAXHP:        // 毒[最大HP割合]
                resultGroup = MasterDataDefineLabel.AilmentGroup.POISON;
                break;

            //------------------------------
            // 暗闇
            //------------------------------
            case MasterDataDefineLabel.AilmentType.DARK:                // 暗闇
                resultGroup = MasterDataDefineLabel.AilmentGroup.DARK;
                break;

            //------------------------------
            // 防御力ダウン
            //------------------------------
            case MasterDataDefineLabel.AilmentType.DEF_DOWN:            // 防御力ダウン
                resultGroup = MasterDataDefineLabel.AilmentGroup.DEF_DOWN;
                break;

            //------------------------------
            // 遅延
            //------------------------------
            case MasterDataDefineLabel.AilmentType.FEAR:                // 遅延
                resultGroup = MasterDataDefineLabel.AilmentGroup.DELAY;
                break;

            //------------------------------
            // 操作時間延長
            //------------------------------
            case MasterDataDefineLabel.AilmentType.TIMER:               // カウントダウン時間変化
                resultGroup = MasterDataDefineLabel.AilmentGroup.TIME_PLUS;
                break;

            //------------------------------
            // 攻撃力アップ
            //------------------------------
            case MasterDataDefineLabel.AilmentType.ATK_UP:              // 攻撃力アップ
            case MasterDataDefineLabel.AilmentType.ATK_UP_FIRE:     // 攻撃力アップ[炎属性]
            case MasterDataDefineLabel.AilmentType.ATK_UP_WATER:        // 攻撃力アップ[水属性]
            case MasterDataDefineLabel.AilmentType.ATK_UP_LIGHT:        // 攻撃力アップ[光属性]
            case MasterDataDefineLabel.AilmentType.ATK_UP_DARK:     // 攻撃力アップ[闇属性]
            case MasterDataDefineLabel.AilmentType.ATK_UP_WIND:     // 攻撃力アップ[風属性]
            case MasterDataDefineLabel.AilmentType.ATK_UP_HEAL:     // 攻撃力アップ[回復属性]
            case MasterDataDefineLabel.AilmentType.ATK_UP_NAUGHT:       // 攻撃力アップ[無属性]
            case MasterDataDefineLabel.AilmentType.ATK_UP_HUMAN:        // 攻撃力アップ[人間]
            case MasterDataDefineLabel.AilmentType.ATK_UP_DRAGON:       // 攻撃力アップ[ドラゴン]
            case MasterDataDefineLabel.AilmentType.ATK_UP_GOD:          // 攻撃力アップ[神]
            case MasterDataDefineLabel.AilmentType.ATK_UP_DEMON:        // 攻撃力アップ[魔物]
            case MasterDataDefineLabel.AilmentType.ATK_UP_CREATURE: // 攻撃力アップ[妖精]
            case MasterDataDefineLabel.AilmentType.ATK_UP_BEAST:        // 攻撃力アップ[獣]
            case MasterDataDefineLabel.AilmentType.ATK_UP_MACHINE:      // 攻撃力アップ[機械]
            case MasterDataDefineLabel.AilmentType.ATK_UP_EGG:          // 攻撃力アップ[強化合成用]
                resultGroup = MasterDataDefineLabel.AilmentGroup.ATK_UP;
                break;

            //------------------------------
            // 攻撃力ダウン
            //------------------------------
            case MasterDataDefineLabel.AilmentType.ATK_DOWN:            // 攻撃力ダウン
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_FIRE:       // 攻撃力ダウン[炎属性]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_WATER:      // 攻撃力ダウン[水属性]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_LIGHT:      // 攻撃力ダウン[光属性]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DARK:       // 攻撃力ダウン[闇属性]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_WIND:       // 攻撃力ダウン[風属性]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_HEAL:       // 攻撃力ダウン[回復属性]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_NAUGHT: // 攻撃力ダウン[無属性]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_GOD:        // 攻撃力ダウン[神]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_BEAST:      // 攻撃力ダウン[獣]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DEMON:      // 攻撃力ダウン[魔物]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DRAGON: // 攻撃力ダウン[ドラゴン]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_FAIRY:      // 攻撃力ダウン[妖精]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_HUMAN:      // 攻撃力ダウン[人間]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_MACHINE:    // 攻撃力ダウン[機械]
                resultGroup = MasterDataDefineLabel.AilmentGroup.ATK_DOWN;
                break;

            //------------------------------
            // 被ダメージアップ
            //------------------------------
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP:          // 被ダメアップ
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_FIRE: // 被ダメアップ[炎属性]
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_WATER:    // 被ダメアップ[水属性]
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_LIGHT:    // 被ダメアップ[光属性]
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_DARK: // 被ダメアップ[闇属性]
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_WIND: // 被ダメアップ[風属性]
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_HEAL: // 被ダメアップ[回復属性]
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_NAUGHT:   // 被ダメアップ[無属性]
                resultGroup = MasterDataDefineLabel.AilmentGroup.DAMAGE_TAKEN_UP;
                break;

            //------------------------------
            // バリア
            //------------------------------
            case MasterDataDefineLabel.AilmentType.DEF_UP:              // バリア
            case MasterDataDefineLabel.AilmentType.DEF_UP_FIRE:     // 防御力アップ[炎属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_WATER:        // 防御力アップ[水属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_LIGHT:        // 防御力アップ[光属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_DARK:     // 防御力アップ[闇属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_WIND:     // 防御力アップ[風属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_HEAL:     // 防御力アップ[回復属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_NAUGHT:       // 防御力アップ[無属性]
                resultGroup = MasterDataDefineLabel.AilmentGroup.BARRIER;
                break;

            //------------------------------
            // AS封印
            //------------------------------
            case MasterDataDefineLabel.AilmentType.SKILL_NOTUSE:        // スキル使用禁止
                resultGroup = MasterDataDefineLabel.AilmentGroup.SEALED;
                break;

            //------------------------------
            // 手札出現確率変動
            //------------------------------
            case MasterDataDefineLabel.AilmentType.HANDCARD_DEFAULT:    // 手札出現率デフォルト
                resultGroup = MasterDataDefineLabel.AilmentGroup.HAND_CHANCE;
                break;

            //------------------------------
            // 回復不可
            //------------------------------
            case MasterDataDefineLabel.AilmentType.NON_RECOVERY_ALL:    // 回復不可[全]
                resultGroup = MasterDataDefineLabel.AilmentGroup.NON_RECOVERY;
                break;

            //------------------------------
            // その他
            //------------------------------
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_FIRE:       // パネル属性完全固定[炎属性]
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_WATER:      // パネル属性完全固定[水属性]
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_LIGHT:      // パネル属性完全固定[光属性]
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_DARK:       // パネル属性完全固定[闇属性]
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_WIND:       // パネル属性完全固定[風属性]
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_HEAL:       // パネル属性完全固定[回復属性]
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_NAUGHT: // パネル属性完全固定[無属性]
            case MasterDataDefineLabel.AilmentType.PANIC:               // 混乱
            case MasterDataDefineLabel.AilmentType.HAPPY_TRESURE:       // ハッピートレジャー
            case MasterDataDefineLabel.AilmentType.BATTLE_MONEY_UP: // バトルマネーアップ
            case MasterDataDefineLabel.AilmentType.TORCH:               // たいまつ
            case MasterDataDefineLabel.AilmentType.ALARM:               // 警報
            case MasterDataDefineLabel.AilmentType.SKILL_HEALING:       // リジェネ
            case MasterDataDefineLabel.AilmentType.AUTO_PLAY_SKILL:

            case MasterDataDefineLabel.AilmentType.MAX:
            default:
                break;
        }

        return (resultGroup);
    }

    /// <summary>
    /// LBSスキルカテゴリIDを状態異常IDに変換
    /// </summary>
    /// <param name="category"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    static public MasterDataDefineLabel.AilmentType getAilmentTypeFromLimitBreakSkillCategory(MasterDataDefineLabel.SkillCategory category, MasterDataDefineLabel.ElementType element_type, MasterDataDefineLabel.KindType kind_type)
    {
        MasterDataDefineLabel.AilmentType ailmentType = 0;

        switch (category)
        {
            case MasterDataDefineLabel.SkillCategory.ABSTATE_POISON:
                // 毒系効果
                ailmentType = MasterDataDefineLabel.AilmentType.POISON;
                break;

            case MasterDataDefineLabel.SkillCategory.SUPPORT_POWUP:
                // 与ダメアップ
                ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP;
                break;

            case MasterDataDefineLabel.SkillCategory.SUPPORT_POWUP_ELEM:
                // 与ダメアップ
                {
                    switch (element_type)
                    {
                        case MasterDataDefineLabel.ElementType.NAUGHT:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_NAUGHT;
                            break;
                        case MasterDataDefineLabel.ElementType.FIRE:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_FIRE;
                            break;
                        case MasterDataDefineLabel.ElementType.WATER:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_WATER;
                            break;
                        case MasterDataDefineLabel.ElementType.LIGHT:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_LIGHT;
                            break;
                        case MasterDataDefineLabel.ElementType.DARK:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_DARK;
                            break;
                        case MasterDataDefineLabel.ElementType.WIND:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_WIND;
                            break;
                        case MasterDataDefineLabel.ElementType.HEAL:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_HEAL;
                            break;

                        case MasterDataDefineLabel.ElementType.NONE:
                        default:
                            break;
                    }
                }
                break;

            case MasterDataDefineLabel.SkillCategory.SUPPORT_ELEM_GUARD:
                // 指定属性からのダメージを軽減
                {
                    switch (element_type)
                    {
                        case MasterDataDefineLabel.ElementType.NAUGHT:
                            ailmentType = MasterDataDefineLabel.AilmentType.DEF_UP_NAUGHT;
                            break;
                        case MasterDataDefineLabel.ElementType.FIRE:
                            ailmentType = MasterDataDefineLabel.AilmentType.DEF_UP_FIRE;
                            break;
                        case MasterDataDefineLabel.ElementType.WATER:
                            ailmentType = MasterDataDefineLabel.AilmentType.DEF_UP_WATER;
                            break;
                        case MasterDataDefineLabel.ElementType.LIGHT:
                            ailmentType = MasterDataDefineLabel.AilmentType.DEF_UP_LIGHT;
                            break;
                        case MasterDataDefineLabel.ElementType.DARK:
                            ailmentType = MasterDataDefineLabel.AilmentType.DEF_UP_DARK;
                            break;
                        case MasterDataDefineLabel.ElementType.WIND:
                            ailmentType = MasterDataDefineLabel.AilmentType.DEF_UP_WIND;
                            break;
                        case MasterDataDefineLabel.ElementType.HEAL:
                            ailmentType = MasterDataDefineLabel.AilmentType.DEF_UP_HEAL;
                            break;

                        case MasterDataDefineLabel.ElementType.NONE:
                        default:
                            break;
                    }
                }
                break;

            case MasterDataDefineLabel.SkillCategory.SUPPORT_KIND_POWUP:
                // 指定種族の与ダメアップ
                {
                    switch (kind_type)
                    {
                        case MasterDataDefineLabel.KindType.HUMAN:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_HUMAN;
                            break;

                        case MasterDataDefineLabel.KindType.DRAGON:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_DRAGON;
                            break;

                        case MasterDataDefineLabel.KindType.GOD:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_GOD;
                            break;

                        case MasterDataDefineLabel.KindType.DEMON:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_DEMON;
                            break;

                        case MasterDataDefineLabel.KindType.CREATURE:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_CREATURE;
                            break;

                        case MasterDataDefineLabel.KindType.BEAST:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_BEAST;
                            break;

                        case MasterDataDefineLabel.KindType.MACHINE:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_MACHINE;
                            break;

                        case MasterDataDefineLabel.KindType.EGG:
                            ailmentType = MasterDataDefineLabel.AilmentType.ATK_UP_EGG;
                            break;

                        case MasterDataDefineLabel.KindType.NONE:
                        default:
                            break;
                    }
                }
                break;

            case MasterDataDefineLabel.SkillCategory.SUPPORT_GUARD:
                // 防御上昇効果
                ailmentType = MasterDataDefineLabel.AilmentType.DEF_UP;
                break;

            case MasterDataDefineLabel.SkillCategory.ABSTATE_GUARD_BREAK:
                // 防御低減効果
                ailmentType = MasterDataDefineLabel.AilmentType.DEF_DOWN;
                break;

            case MasterDataDefineLabel.SkillCategory.ABSTATE_TURN_LATE:
                // 行動遅延
                ailmentType = MasterDataDefineLabel.AilmentType.FEAR;
                break;

            case MasterDataDefineLabel.SkillCategory.SUPPORT_QUICK:
                // カウントダウン時間変化
                ailmentType = MasterDataDefineLabel.AilmentType.TIMER;
                break;

            case MasterDataDefineLabel.SkillCategory.ABSTATE_TORCH:
                // たいまつ
                ailmentType = MasterDataDefineLabel.AilmentType.TORCH;
                break;

            case MasterDataDefineLabel.SkillCategory.ABSTATE_DARK:
                // 暗闇
                ailmentType = MasterDataDefineLabel.AilmentType.DARK;
                break;

            case MasterDataDefineLabel.SkillCategory.ABSTATE_PANIC:
                // 混乱
                ailmentType = MasterDataDefineLabel.AilmentType.PANIC;
                break;

            case MasterDataDefineLabel.SkillCategory.ABSTATE_HAPPYTRESURE:
                // ハッピートレジャー
                ailmentType = MasterDataDefineLabel.AilmentType.HAPPY_TRESURE;
                break;

            case MasterDataDefineLabel.SkillCategory.ABSTATE_BATTLEMONEYUP:
                // バトルマネーアップ
                ailmentType = MasterDataDefineLabel.AilmentType.BATTLE_MONEY_UP;
                break;

            case MasterDataDefineLabel.SkillCategory.ABSTATE_ALARM:
                // 警報
                ailmentType = MasterDataDefineLabel.AilmentType.ALARM;
                break;

            case MasterDataDefineLabel.SkillCategory.SUPPORT_COST_FIX:
                {
                    // パネル属性固定
                    switch (element_type)
                    {
                        case MasterDataDefineLabel.ElementType.FIRE: ailmentType = MasterDataDefineLabel.AilmentType.PNL_HOLD_FIRE; break;
                        case MasterDataDefineLabel.ElementType.WATER: ailmentType = MasterDataDefineLabel.AilmentType.PNL_HOLD_WATER; break;
                        case MasterDataDefineLabel.ElementType.LIGHT: ailmentType = MasterDataDefineLabel.AilmentType.PNL_HOLD_LIGHT; break;
                        case MasterDataDefineLabel.ElementType.DARK: ailmentType = MasterDataDefineLabel.AilmentType.PNL_HOLD_DARK; break;
                        case MasterDataDefineLabel.ElementType.WIND: ailmentType = MasterDataDefineLabel.AilmentType.PNL_HOLD_WIND; break;
                        case MasterDataDefineLabel.ElementType.HEAL: ailmentType = MasterDataDefineLabel.AilmentType.PNL_HOLD_HEAL; break;
                        case MasterDataDefineLabel.ElementType.NAUGHT: ailmentType = MasterDataDefineLabel.AilmentType.PNL_HOLD_NAUGHT; break;
                        default: break;
                    }
                }
                break;

            case MasterDataDefineLabel.SkillCategory.ABSTATE_POISON_MAXHP:
                // 毒[最大HP割合]
                ailmentType = MasterDataDefineLabel.AilmentType.POISON_MAXHP;
                break;

            case MasterDataDefineLabel.SkillCategory.NON_RECOVERY_ALL:
                // 回復不可[全]
                ailmentType = MasterDataDefineLabel.AilmentType.NON_RECOVERY_ALL;
                break;

            case MasterDataDefineLabel.SkillCategory.SUPPORT_COST_CHANGE_ALL:
            case MasterDataDefineLabel.SkillCategory.SUPPORT_LBS_TURN_FAST:
            case MasterDataDefineLabel.SkillCategory.MOVE_TELEPORT:
            case MasterDataDefineLabel.SkillCategory.MOVE_UNLOCK_DOOR:
            default:
                break;
        }

        return ailmentType;
    }

    static public MasterDataDefineLabel.AilmentType getAilmentTypeFromLimitBreakSkillCategory(MasterDataSkillLimitBreak master_data_skill_limit_break)
    {
        MasterDataDefineLabel.ElementType element_type = MasterDataDefineLabel.ElementType.NONE;
        MasterDataDefineLabel.KindType kind_type = MasterDataDefineLabel.KindType.NONE;

        switch (master_data_skill_limit_break.skill_cate)
        {
            case MasterDataDefineLabel.SkillCategory.SUPPORT_POWUP_ELEM:
                element_type = master_data_skill_limit_break.Get_SUPPORT_ELEMUP_ELEM();
                break;

            case MasterDataDefineLabel.SkillCategory.SUPPORT_ELEM_GUARD:
                element_type = master_data_skill_limit_break.Get_SUPPORT_GUARD_ELEM_ELEMENT();
                break;

            case MasterDataDefineLabel.SkillCategory.SUPPORT_KIND_POWUP:
                kind_type = master_data_skill_limit_break.Get_SUPPORT_KIND_POWUP_KIND();
                break;

            case MasterDataDefineLabel.SkillCategory.SUPPORT_COST_FIX:
                element_type = master_data_skill_limit_break.Get_SUPPORT_C_FIX_ELEM();
                break;

            default:
                break;
        }

        return getAilmentTypeFromLimitBreakSkillCategory(master_data_skill_limit_break.skill_cate, element_type, kind_type);
    }

    /// <summary>
    /// ユニットに付与して効果がある状態変化かどうかを調べる
    /// 付与されるユニットの属性・種族の影響で効果の有無が決まるのは「攻撃アップ」「攻撃ダウン」のみ（防御系はユニットの属性ではなく攻撃の属性）
    /// </summary>
    /// <param name="ailment_type">状態変化種類</param>
    /// <param name="element_type">付与されるユニットの属性</param>
    /// <param name="kind_type1">付与されるユニットの種族１</param>
    /// <param name="kind_type2">付与されるユニットの種族２</param>
    /// <returns></returns>
    public static bool isWorkAilmentForUnit(MasterDataDefineLabel.AilmentType ailment_type, MasterDataParamChara owner_chara_master)
    {
        if (owner_chara_master == null)
        {
            return true;
        }

        MasterDataDefineLabel.ElementType target_element_type = MasterDataDefineLabel.ElementType.NONE;
        MasterDataDefineLabel.KindType target_kind_type = MasterDataDefineLabel.KindType.NONE;

        switch (ailment_type)
        {
            case MasterDataDefineLabel.AilmentType.ATK_UP_FIRE:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_FIRE:
                target_element_type = MasterDataDefineLabel.ElementType.FIRE;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_UP_WATER:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_WATER:
                target_element_type = MasterDataDefineLabel.ElementType.WATER;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_UP_LIGHT:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_LIGHT:
                target_element_type = MasterDataDefineLabel.ElementType.LIGHT;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_UP_DARK:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DARK:
                target_element_type = MasterDataDefineLabel.ElementType.DARK;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_UP_WIND:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_WIND:
                target_element_type = MasterDataDefineLabel.ElementType.WIND;
                break;

#if false // 回復属性キャラは存在しない
            case MasterDataDefineLabel.AilmentType.ATK_UP_HEAL:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_HEAL:
                target_element_type = MasterDataDefineLabel.ElementType.HEAL;
                break;
#endif

            case MasterDataDefineLabel.AilmentType.ATK_UP_NAUGHT:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_NAUGHT:
                target_element_type = MasterDataDefineLabel.ElementType.NAUGHT;
                break;


            case MasterDataDefineLabel.AilmentType.ATK_UP_HUMAN:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_HUMAN:
                target_kind_type = MasterDataDefineLabel.KindType.HUMAN;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_UP_DRAGON:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DRAGON:
                target_kind_type = MasterDataDefineLabel.KindType.DRAGON;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_UP_GOD:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_GOD:
                target_kind_type = MasterDataDefineLabel.KindType.GOD;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_UP_DEMON:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DEMON:
                target_kind_type = MasterDataDefineLabel.KindType.DEMON;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_UP_CREATURE:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_FAIRY:
                target_kind_type = MasterDataDefineLabel.KindType.CREATURE;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_UP_BEAST:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_BEAST:
                target_kind_type = MasterDataDefineLabel.KindType.BEAST;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_UP_MACHINE:
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_MACHINE:
                target_kind_type = MasterDataDefineLabel.KindType.MACHINE;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_UP_EGG:
                target_kind_type = MasterDataDefineLabel.KindType.EGG;
                break;
        }

        bool ret_val = true;

        if (target_element_type != MasterDataDefineLabel.ElementType.NONE)
        {
            if (owner_chara_master.element != target_element_type)
            {
                ret_val = false;
            }
        }

        if (target_kind_type != MasterDataDefineLabel.KindType.NONE)
        {
            if (owner_chara_master.kind != target_kind_type
                && owner_chara_master.sub_kind != target_kind_type
            )
            {
                ret_val = false;
            }
        }

        return ret_val;
    }
} // class StatusAilment


//----------------------------------------------------------------------------
/*!
    @class		StatusAilmentChara
    @brief		管理用の状態異常キャラ情報クラス
*/
//----------------------------------------------------------------------------
[Serializable]
public class StatusAilmentChara
{
    //----------------------------------------------------------------------------
    //	@brief		更新タイミング定義
    //----------------------------------------------------------------------------
    public enum UpdateTiming : int
    {
        eFIELD,                     //!< 移動フェイズ
        eBATTLE,                    //!< 戦闘フェイズ
    }

    public enum PoisonType
    {
        NORMAL = 0, //!< 毒タイプ：通常
        MAXHP = 1,  //!< 毒タイプ：最大HP割合

        MAX = 2 //!< 毒タイプ：
    }

    public enum OwnerType
    {
        PLAYER,
        ENEMY,
    }

    // この項目はセーブされるので public にしています
    public StatusAilment[] cAilment;                                                               //!< 状態異常
    public OwnerType m_OwnerType;


    private const int STATUSAILMENT_LIFE_MIN = 0;       //!< 効果寿命最少
    private const int STATUSAILMENT_LIFE_MAX = 99;      //!< 効果寿命最大
    private const int STATUSAILMENT_MAX = 20;       //!< 状態異常管理最大数

    /// <summary>
    /// 状態異常管理最大数を取得(public const はLitJsonだと問題があるので関数経由で取得するようにした)
    /// </summary>
    /// <returns></returns>
    public static int get_STATUSAILMENT_MAX()
    {
        return STATUSAILMENT_MAX;
    }

    private bool m_IsExistOffenceAilment;                                               //!< 攻撃力変化の状態異常にかかっているかどうか
    private bool m_IsExistDefenceAilment;                                               //!< 防御力変化の状態異常にかかっているかどうか
    private float m_OffenceRate;                                                            //!< 攻撃力変化倍率
    private float m_DefenceDamageDownRate;                                              //!< 被ダメ軽減倍率
    private float m_DefenceDamageUpRate;                                                    //!< 被ダメ増加倍率			// @add Developer 2016/04/05 v340 バリア無視対応、変数追加
    private float m_DefenceArmorRate;                                                       //!< 防御力減衰倍率

    private float[] m_OffenceRateElem = new float[(int)MasterDataDefineLabel.ElementType.MAX];  //!< 攻撃力変化倍率[属性]
    private float[] m_OffenceRateKind = new float[(int)MasterDataDefineLabel.KindType.MAX]; //!< 攻撃力変化倍率[種族]
    private float[] m_DefenceDamageDownRateElem = new float[(int)MasterDataDefineLabel.ElementType.MAX];    //!< 被ダメ変化倍率[属性]
    private float[] m_DefenceDamageUpRateElem = new float[(int)MasterDataDefineLabel.ElementType.MAX];  //!< 被ダメ増加倍率[属性]	// @add Developer 2016/04/05 v340 バリア無視対応、変数追加
    private float[] m_DefenceDanageRateKind = new float[(int)MasterDataDefineLabel.KindType.MAX];   //!< 被ダメ変化倍率[種族]

    private float[] fPoisonDamage = new float[(int)PoisonType.MAX]; //!< 毒ダメージ
    private float fHealValue = 0.0f;                                            //!< 回復量

    private int nCountDown;                                                             //!< カウントダウン変化
    private MasterDataDefineLabel.ElementType nFixElement;                                                          //!< 属性固定

    private bool[] bStatus = new bool[(int)MasterDataDefineLabel.AilmentType.MAX];          //!< 状態異常フラグ

    public StatusAilmentChara()
    {
        _init(OwnerType.PLAYER);
    }

    public StatusAilmentChara(OwnerType owner_type)
    {
        _init(owner_type);
    }

    private void _init(OwnerType owner_type)
    {
        cAilment = null;

        m_IsExistOffenceAilment = false;
        m_IsExistDefenceAilment = false;
        m_OffenceRate = 1.0f;
        m_DefenceDamageDownRate = 1.0f;
        m_DefenceDamageUpRate = 1.0f;
        m_DefenceArmorRate = 1.0f;

        for (int num = 0; num < (int)PoisonType.MAX; ++num)
        {
            fPoisonDamage[num] = 0.0f;
        }
        fHealValue = 0.0f;

        nCountDown = 0;
        nFixElement = 0;

        m_OwnerType = owner_type;

        for (int i = 0; i < bStatus.Length; i++)
        {
            bStatus[i] = false;
        }

        for (int i = 0; i < m_OffenceRateElem.Length; i++)
        {
            m_OffenceRateElem[i] = 1.0f;
        }

        for (int i = 0; i < m_OffenceRateKind.Length; i++)
        {
            m_OffenceRateKind[i] = 1.0f;
        }

        for (int i = 0; i < m_DefenceDamageDownRateElem.Length; i++)
        {
            m_DefenceDamageDownRateElem[i] = 1.0f;
        }

        for (int i = 0; i < m_DefenceDamageUpRateElem.Length; i++)
        {
            m_DefenceDamageUpRateElem[i] = 1.0f;
        }

        for (int i = 0; i < m_DefenceDanageRateKind.Length; i++)
        {
            m_DefenceDanageRateKind[i] = 1.0f;
        }

        cAilment = new StatusAilment[STATUSAILMENT_MAX];
        for (int n = 0; n < cAilment.Length; n++)
        {
            cAilment[n] = new StatusAilment();
            cAilment[n].bUsed = false;
        }
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		指定都合のステータスがかかっているかチェック
        @param[in]	int			(goodOrbad)		良い/悪い(MasterDataDefineLabel.BOOL_ENABLE)
        @retval		bool		[かかっている/かかっていない]
    */
    //----------------------------------------------------------------------------
    public bool ChkStatusCondition(StatusAilment.GoodOrBad goodOrbad)
    {

        bool retVal = false;


        //--------------------------------
        //	状態変化全てをチェック
        //--------------------------------
        for (int i = 0; i < cAilment.Length; i++)
        {

            if (cAilment[i] == null)
            {
                continue;
            }

            if (cAilment[i].nGoodOrBad != goodOrbad)
            {
                continue;
            }

            retVal = true;
            break;
        }


        return retVal;
    }


    //------------------------------------------------------------------------
    /*!
        @brief		状態異常ターン更新個別更新
        @param[in]	EAILMENT_UPDATE_TIMING		(eTiming)				タイミング指示
    */
    //------------------------------------------------------------------------
    public void UpdateTurnOnce(UpdateTiming eTiming)
    {

        //--------------------------------
        //	各種効果倍率をクリアする
        //	@change Developer 2016/04/05 v340 バリア無視対応、変数追加
        //--------------------------------
        m_IsExistOffenceAilment = false;
        m_IsExistDefenceAilment = false;
        m_OffenceRate = 1.0f;
        m_DefenceDamageDownRate = 1.0f;
        m_DefenceDamageUpRate = 1.0f;
        m_DefenceArmorRate = 1.0f;
        nCountDown = 0;
        nFixElement = MasterDataDefineLabel.ElementType.NONE;
        fHealValue = 0.0f;

        for (int n = 0; n < (int)StatusAilmentChara.PoisonType.MAX; ++n)
        {
            fPoisonDamage[n] = 0.0f;
        }

        for (int n = 0; n < m_OffenceRateElem.Length; n++)
        {
            m_OffenceRateElem[n] = 1.0f;
        }

        for (int n = 0; n < m_OffenceRateKind.Length; n++)
        {
            m_OffenceRateKind[n] = 1.0f;
        }

        for (int n = 0; n < m_DefenceDamageDownRateElem.Length; n++)
        {
            m_DefenceDamageDownRateElem[n] = 1.0f;
        }

        for (int n = 0; n < m_DefenceDamageUpRateElem.Length; n++)
        {
            m_DefenceDamageUpRateElem[n] = 1.0f;
        }

        for (int n = 0; n < m_DefenceDanageRateKind.Length; n++)
        {
            m_DefenceDanageRateKind[n] = 1.0f;
        }


        StatusAilment ailment;
        //--------------------------------
        //	ターン経過処理を状態異常管理に対して行う
        //--------------------------------
        for (int n = 0; n < cAilment.Length; n++)
        {
            // 未使用領域はスキップ
            ailment = cAilment[n];
            if (ailment.bUsed == false)
            {
                continue;
            }

            //--------------------------------
            //	更新タイミングのチェック
            //--------------------------------
            switch (eTiming)
            {
                case UpdateTiming.eBATTLE:
                    {
                        //--------------------------------
                        //	戦闘中
                        //--------------------------------
                        //	戦闘中の更新が有効になっていない場合中止
                        if (ailment.nUpdateTurnBattle != MasterDataDefineLabel.BoolType.ENABLE)
                        {
                            continue;
                        }
                    }
                    break;

                case UpdateTiming.eFIELD:
                    {
                        //--------------------------------
                        //	移動中
                        //--------------------------------
                        //	移動中の更新が有効になっていない場合中止
                        if (ailment.nUpdateTurnMove != MasterDataDefineLabel.BoolType.ENABLE)
                        {
                            continue;
                        }
                    }
                    break;

                default:
                    break;
            }


            // 発動ターンフラグをおろす
            ailment.bEdge = false;

            // ターン数を１減らす
            ailment.nLife += -1;
            if (ailment.nLife < STATUSAILMENT_LIFE_MIN)
            {
                ailment.nLife = STATUSAILMENT_LIFE_MIN;
            }


            // 効果継続時間が０になった場合は状態異常の解除を行う
            if (ailment.nLife <= STATUSAILMENT_LIFE_MIN)
            {

                MasterDataDefineLabel.AilmentType nType = ailment.nType;



                // 威嚇スキルはここでは解除しない
                if (nType == MasterDataDefineLabel.AilmentType.FEAR)
                {
                    continue;
                }

                // 状態異常の解除
                if (nType != MasterDataDefineLabel.AilmentType.NONE)
                {
                    bStatus[(int)nType] = false;
                }

                // 未使用領域に設定
                ailment.bUsed = false;


                continue;

            }
            else
            {

                // 効果発揮(倍率更新)
                EffectiveStatusAilment(ailment);

            }
        }
    }

    //------------------------------------------------------------------------
    /*!
        @brief		状態異常効果発揮
        @param[in]	StatusAilment		(ailment)		状態異常効果
    */
    //------------------------------------------------------------------------
    public void EffectiveStatusAilment(StatusAilment ailment)
    {
        //--------------------------------
        //	エラーチェック
        //--------------------------------
        if (ailment == null)
        {
            return;
        }

        //--------------------------------
        // 各効果を反映
        //--------------------------------
        MasterDataDefineLabel.AilmentType nType = ailment.nType;
        if (nType != MasterDataDefineLabel.AilmentType.NONE
            && nType != MasterDataDefineLabel.AilmentType.POISON
            && nType != MasterDataDefineLabel.AilmentType.POISON_MAXHP)
        {
            bStatus[(int)nType] = true;
        }

        switch (nType)
        {
            case MasterDataDefineLabel.AilmentType.NONE:
                break;

            //--------------------------------
            //	毒
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.POISON:
                // 毒は上書き不可
                {
                    if (bStatus[(int)nType] == false)
                    {
                        bStatus[(int)nType] = true;
                    }

                    float rate = 0.0f;
                    rate = InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //AILMENTPARAM_POISON_RATE
                    rate = InGameUtilBattle.AvoidErrorMultiple(ailment.nBaseAtk, rate);

                    fPoisonDamage[(int)PoisonType.NORMAL] = rate;
                }
                break;


            //--------------------------------
            //	ダメージ軽減倍率
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.DEF_UP:
                // ダメージ軽減倍率
                m_DefenceDamageDownRate *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //AILMENTPARAM_BARRIER_RATE
                m_IsExistDefenceAilment = true;
                break;


            //--------------------------------
            //	防御値減衰倍率
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.DEF_DOWN:
                // 防御値減衰倍率
                m_DefenceArmorRate *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_GUARDBREAK_RATE
                m_IsExistDefenceAilment = true;
                break;


            //--------------------------------
            //	カウントダウン変化
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.TIMER:
                // カウントダウン変化
                nCountDown += ailment.nValueFix;    //AILMENTPARAM_COUNTDOWN_COUNT
                break;


            //--------------------------------
            //	攻撃力アップ
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.ATK_UP:
                // 攻撃力アップ
                m_OffenceRate *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);   //AILMENTPARAM_ATK_UP_RATE
                m_IsExistOffenceAilment = true;
                break;


            //--------------------------------
            //	攻撃力アップ【属性】
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.ATK_UP_FIRE:
                // 攻撃力アップ[炎属性]
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.FIRE] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_ATK_UP_ELEM_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_WATER:
                // 攻撃力アップ[水属性]
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.WATER] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //AILMENTPARAM_ATK_UP_ELEM_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_LIGHT:
                // 攻撃力アップ[光属性]
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.LIGHT] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //AILMENTPARAM_ATK_UP_ELEM_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_DARK:
                // 攻撃力アップ[闇属性]
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.DARK] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_ATK_UP_ELEM_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_WIND:
                // 攻撃力アップ[風属性]
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.WIND] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_ATK_UP_ELEM_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_HEAL:
                // 攻撃力アップ[回復属性]
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.HEAL] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_ATK_UP_ELEM_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_NAUGHT:
                // 攻撃力アップ[無属性]
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.NAUGHT] = InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //AILMENTPARAM_ATK_UP_ELEM_RATE
                m_IsExistOffenceAilment = true;
                break;


            //--------------------------------
            //	エナジーパネル属性固定
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_FIRE:
                nFixElement = MasterDataDefineLabel.ElementType.FIRE;
                break;

            case MasterDataDefineLabel.AilmentType.PNL_HOLD_WATER:
                nFixElement = MasterDataDefineLabel.ElementType.WATER;
                break;

            case MasterDataDefineLabel.AilmentType.PNL_HOLD_LIGHT:
                nFixElement = MasterDataDefineLabel.ElementType.LIGHT;
                break;

            case MasterDataDefineLabel.AilmentType.PNL_HOLD_DARK:
                nFixElement = MasterDataDefineLabel.ElementType.DARK;
                break;

            case MasterDataDefineLabel.AilmentType.PNL_HOLD_WIND:
                nFixElement = MasterDataDefineLabel.ElementType.WIND;
                break;

            case MasterDataDefineLabel.AilmentType.PNL_HOLD_HEAL:
                nFixElement = MasterDataDefineLabel.ElementType.HEAL;
                break;

            case MasterDataDefineLabel.AilmentType.PNL_HOLD_NAUGHT:
                nFixElement = MasterDataDefineLabel.ElementType.NAUGHT;
                break;

            //--------------------------------
            //	防御力アップ【属性】
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.DEF_UP_FIRE:
                // 防御力アップ[炎属性]
                m_DefenceDamageDownRateElem[(int)MasterDataDefineLabel.ElementType.FIRE] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);    //AILMENTPARAM_BARRIER_RATE
                m_IsExistDefenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.DEF_UP_WATER:
                // 防御力アップ[水属性]
                m_DefenceDamageDownRateElem[(int)MasterDataDefineLabel.ElementType.WATER] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);   //AILMENTPARAM_BARRIER_RATE
                m_IsExistDefenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.DEF_UP_LIGHT:
                // 防御力アップ[光属性]
                m_DefenceDamageDownRateElem[(int)MasterDataDefineLabel.ElementType.LIGHT] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);   //AILMENTPARAM_BARRIER_RATE
                m_IsExistDefenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.DEF_UP_DARK:
                // 防御力アップ[闇属性]
                m_DefenceDamageDownRateElem[(int)MasterDataDefineLabel.ElementType.DARK] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);    //AILMENTPARAM_BARRIER_RATE
                m_IsExistDefenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.DEF_UP_WIND:
                // 防御力アップ[風属性]
                m_DefenceDamageDownRateElem[(int)MasterDataDefineLabel.ElementType.WIND] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);    //AILMENTPARAM_BARRIER_RATE
                m_IsExistDefenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.DEF_UP_HEAL:
                // 防御力アップ[回復属性]
                m_DefenceDamageDownRateElem[(int)MasterDataDefineLabel.ElementType.HEAL] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);    //AILMENTPARAM_BARRIER_RATE
                m_IsExistDefenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.DEF_UP_NAUGHT:
                // 防御力アップ[無属性]
                m_DefenceDamageDownRateElem[(int)MasterDataDefineLabel.ElementType.NAUGHT] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_BARRIER_RATE
                m_IsExistDefenceAilment = true;
                break;


            //--------------------------------
            //	攻撃力アップ【種族】
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.ATK_UP_HUMAN:
                // 攻撃力アップ[人間]
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.HUMAN] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);    //AILMENTPARAM_ATK_UP_KIND_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_DRAGON:
                // 攻撃力アップ[ドラゴン]
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.DRAGON] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);   //AILMENTPARAM_ATK_UP_KIND_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_GOD:
                // 攻撃力アップ[神]
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.GOD] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_ATK_UP_KIND_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_DEMON:
                // 攻撃力アップ[魔物]
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.DEMON] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);    //AILMENTPARAM_ATK_UP_KIND_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_CREATURE:
                // 攻撃力アップ[妖精]
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.CREATURE] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //AILMENTPARAM_ATK_UP_KIND_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_BEAST:
                // 攻撃力アップ[獣]
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.BEAST] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);    //AILMENTPARAM_ATK_UP_KIND_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_MACHINE:
                // 攻撃力アップ[機械]
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.MACHINE] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_ATK_UP_KIND_RATE
                m_IsExistOffenceAilment = true;
                break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_EGG:
                // 攻撃力アップ[強化合成用]
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.EGG] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_ATK_UP_KIND_RATE
                m_IsExistOffenceAilment = true;
                break;


            //--------------------------------
            //	スキル封印
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.SKILL_NOTUSE:
                // スキル使用禁止
                break;


            //--------------------------------
            //	リジェネ
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.SKILL_HEALING:
                // リジェネ
                {
                    if (ailment.nValueFix != 0)
                    {

                        //--------------------------------
                        //	固定値
                        //--------------------------------
                        fHealValue = ailment.nValueFix; //SKILLPARAM_HEALING_VALUE
                    }
                    else
                    {

                        //--------------------------------
                        //	割合
                        //--------------------------------
                        float param_rate = InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //
                        fHealValue = (int)InGameUtilBattle.AvoidErrorMultiple(ailment.nBaseHPMax, param_rate);

                    }
                }
                break;


            //--------------------------------
            //	攻撃力ダウン
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.ATK_DOWN:
                // 攻撃力ダウン
                m_OffenceRate *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);   //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;


            //--------------------------------
            //	攻撃力ダウン【属性】
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_FIRE:
                // 攻撃力ダウン【炎】
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.FIRE] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_WATER:
                // 攻撃力ダウン【水】
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.WATER] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_LIGHT:
                // 攻撃力ダウン【光】
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.LIGHT] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DARK:
                // 攻撃力ダウン【闇】
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.DARK] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_WIND:
                // 攻撃力ダウン【風】
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.WIND] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_NAUGHT:
                // 攻撃力ダウン【無】
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.NAUGHT] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);    //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_HEAL:
                // 攻撃力ダウン【回復】
                m_OffenceRateElem[(int)MasterDataDefineLabel.ElementType.HEAL] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;


            //--------------------------------
            //	攻撃力ダウン【種族】
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_GOD:
                //	神
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.GOD] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_BEAST:
                //	獣
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.BEAST] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);    //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DEMON:
                //	魔物
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.DEMON] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);    //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DRAGON:
                //	ドラゴン
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.DRAGON] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);   //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_FAIRY:
                //	妖精
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.CREATURE] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_HUMAN:
                //	人間
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.HUMAN] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);    //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_MACHINE:
                //	機械
                m_OffenceRateKind[(int)MasterDataDefineLabel.KindType.MACHINE] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_SUPPORT_POWDOWN_RATE
                m_IsExistOffenceAilment = true;
                break;


            //--------------------------------
            //	被ダメージアップ
            //	@change Developer 2016/04/05 v340 バリア無視対応、変数変更
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP:
                //	被ダメアップ
                m_DefenceDamageUpRate *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);   //AILMENTPARAM_DEF_DMG_UP_RATE
                m_IsExistDefenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_FIRE:
                //	被ダメアップ【炎】
                m_DefenceDamageUpRateElem[(int)MasterDataDefineLabel.ElementType.FIRE] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_DEF_DMG_UP_RATE
                m_IsExistDefenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_WATER:
                //	被ダメアップ【水】
                m_DefenceDamageUpRateElem[(int)MasterDataDefineLabel.ElementType.WATER] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //AILMENTPARAM_DEF_DMG_UP_RATE
                m_IsExistDefenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_WIND:
                //	被ダメアップ【風】
                m_DefenceDamageUpRateElem[(int)MasterDataDefineLabel.ElementType.WIND] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_DEF_DMG_UP_RATE
                m_IsExistDefenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_LIGHT:
                //	被ダメアップ【光】
                m_DefenceDamageUpRateElem[(int)MasterDataDefineLabel.ElementType.LIGHT] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //AILMENTPARAM_DEF_DMG_UP_RATE
                m_IsExistDefenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_DARK:
                //	被ダメアップ【闇】
                m_DefenceDamageUpRateElem[(int)MasterDataDefineLabel.ElementType.DARK] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_DEF_DMG_UP_RATE
                m_IsExistDefenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_NAUGHT:
                //	被ダメアップ【無】
                m_DefenceDamageUpRateElem[(int)MasterDataDefineLabel.ElementType.NAUGHT] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);    //AILMENTPARAM_DEF_DMG_UP_RATE
                m_IsExistDefenceAilment = true;
                break;

            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_HEAL:
                //	被ダメアップ【回復】
                m_DefenceDamageUpRateElem[(int)MasterDataDefineLabel.ElementType.HEAL] *= InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate);  //AILMENTPARAM_DEF_DMG_UP_RATE
                m_IsExistDefenceAilment = true;
                break;


            //--------------------------------
            //	手札出現率デフォルト
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.HANDCARD_DEFAULT:
                break;

            //--------------------------------
            //	毒[最大HP割合]
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.POISON_MAXHP:
                // 毒は上書き不可
                {
                    if (bStatus[(int)nType] == false)
                    {
                        bStatus[(int)nType] = true;
                    }

                    float rate = 0.0f;
                    rate = InGameUtilBattle.GetDBRevisionValue(ailment.nValueRate); //AILMENTPARAM_POISON_MAXHP_RATE
                    rate = InGameUtilBattle.AvoidErrorMultiple(ailment.nBaseHPMax, rate);

                    fPoisonDamage[(int)PoisonType.MAXHP] = rate;
                }
                break;

            //--------------------------------
            //	回復不可[全]
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.NON_RECOVERY_ALL:
                break;

            //--------------------------------
            //	暗闇
            //--------------------------------
            case MasterDataDefineLabel.AilmentType.DARK:
                break;

            case MasterDataDefineLabel.AilmentType.AUTO_PLAY_SKILL:
                break;

            default:
                break;
        }
    }

    //------------------------------------------------------------------------
    /*!
        @brief		状態異常の追加
        @param[in]	StatusAilment		(param)		状態異常パラメータ
        @retval		[true=登録完了/false=登録失敗]
    */
    //------------------------------------------------------------------------
    public bool AddStatusAilment(StatusAilment ailmentParam
        , MasterDataParamChara owner_chara_master
#if BUILD_TYPE_DEBUG
        , bool is_output_log = true
#endif //BUILD_TYPE_DEBUG
    )
    {
        //--------------------------------
        //	エラーチェック
        //--------------------------------

        if (ailmentParam == null)
        {
            return false;
        }

        int idx = -1;


        //--------------------------------
        //	既に同じ状態異常が登録されていないかチェック
        //--------------------------------
        for (int n = 0; n < cAilment.Length; n++)
        {
            // ｎｕｌｌチェック
            StatusAilment ailment = cAilment[n];
            if (ailment == null)
            {
                continue;
            }

            // ついでに状態異常管理バッファの空き検索を行っておく
            if (ailment.bUsed == false)
            {
                idx = n;
                continue;
            }

            // 同じスキルにかかっていれば終了
            if (ailment.nType == ailmentParam.nType)
            {
#if BUILD_TYPE_DEBUG
                if (is_output_log)
                {
                    DebugBattleLog.outputAilmentChara(this, "へ状態変化付与無効（同スキルが付与済み）");
                }
#endif //BUILD_TYPE_DEBUG
                return false;
            }
        }

        // 付与対象ユニットに対して効果がなければ付与しない（属性・種族で判定）
        if (StatusAilment.isWorkAilmentForUnit(ailmentParam.nType, owner_chara_master) == false)
        {
            return false;
        }

        //--------------------------------
        //	状態異常管理バッファに空きがなかった
        //--------------------------------
        if (idx == -1)
        {
#if BUILD_TYPE_DEBUG
            if (is_output_log)
            {
                DebugBattleLog.outputAilmentChara(this, "へ状態変化付与無効（付与数上限）");
            }
#endif //BUILD_TYPE_DEBUG
            return false;
        }


        //--------------------------------
        //	状態異常管理バッファに空きがあったため、
        //	パラメータを元に新たな状態異常を登録
        //--------------------------------
        //		cAilment[ idx ]       = ailmentParam;
        cAilment[idx].SetupData(ailmentParam);
        cAilment[idx].bUsed = true;
        cAilment[idx].bEdge = true;

#if BUILD_TYPE_DEBUG
        if (is_output_log)
        {
            MasterDataStatusAilmentParam master_data = BattleParam.m_MasterDataCache.useAilmentParam((uint)ailmentParam.nMasterDataStatusAilmentID);
            if (master_data != null)
            {
                DebugBattleLog.outputAilmentChara(this, "へ状態変化付与:"
                    + master_data.name
                    + " fixid:" + ailmentParam.nMasterDataStatusAilmentID.ToString()
                    + " 付与者のATK:" + ailmentParam.nBaseAtk.ToString()
                    + " 被付与者のMaxHP:" + ailmentParam.nBaseHPMax.ToString()
                );
            }
            else
            {
                DebugBattleLog.outputAilmentChara(this, "へ状態変化付与エラー マスターデータ不明 fixid:" + ailmentParam.nMasterDataStatusAilmentID.ToString());
            }
        }
#endif //BUILD_TYPE_DEBUG

        //--------------------------------
        //	初回の効果発動を行う
        //--------------------------------
        EffectiveStatusAilment(cAilment[idx]);


        return true;
    }


    //------------------------------------------------------------------------
    /// <summary>
    /// 状態異常追加
    /// </summary>
    /// <param name="status_ailment_id">状態異常データID</param>
    /// <param name="attacker_base_attack">攻撃者側の攻撃力</param>
    /// <param name="defender_base_hpmax">被攻撃者側の体力</param>
    /// <returns></returns>
    public bool AddStatusAilment(int status_ailment_id, int attacker_base_attack, int defender_base_hpmax, MasterDataParamChara owner_chara_master)
    {
        if (status_ailment_id == 0)
        {
            return false;
        }

        StatusAilment statusAilment = new StatusAilment();
        if (statusAilment == null)
        {
            return false;
        }

        statusAilment._SetupData(status_ailment_id, attacker_base_attack, defender_base_hpmax);


        //--------------------------------
        //	現状のシステムを残しつつ対応するため、
        //	状態異常パラメータを構造体に格納し、
        //	旧版の状態異常リクエストを使用して発行する
        //--------------------------------
        return AddStatusAilment(statusAilment, owner_chara_master);
    }

    //------------------------------------------------------------------------
    /*!
        @brief		キャラ情報の完全消去
    */
    //------------------------------------------------------------------------
    public void ClearChara()
    {

        //--------------------------------
        //	使用中フラグの初期化
        //--------------------------------
        for (int n = 0; n < cAilment.Length; n++)
        {
            cAilment[n].bUsed = false;
        }

        //--------------------------------
        //	ステータスのクリア
        //--------------------------------
        DelAllStatusAilment();
    }

    //------------------------------------------------------------------------
    /*!
        @brief		状態異常の効果再チェック
        @param[in]	int		(id)		管理ID
    */
    //------------------------------------------------------------------------
    public void CheckEffectiveStatusAilment()
    {
        for (int num = 0; num < cAilment.Length; ++num)
        {
            if (cAilment[num].bUsed == false
            && cAilment[num].nLife <= STATUSAILMENT_LIFE_MIN)
            {
                continue;
            }
            else
            {
                // 効果発揮(倍率更新)
                EffectiveStatusAilment(cAilment[num]);
            }
        }
    }



    //------------------------------------------------------------------------
    /*!
        @brief		ステータスの取得				<static>
        @param[in]	int			(nStatus)	ステータス
    */
    //------------------------------------------------------------------------
    public bool[] GetStatus()
    {
        return bStatus;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		攻撃力倍率取得			<static>
        @return		float		[攻撃力倍率]
    */
    //------------------------------------------------------------------------
    public float GetOffenceRate()
    {
        return m_OffenceRate;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		属性攻撃力倍率取得			<static>
        @param[in]	int			(elem)		属性
        @return		float		[攻撃力倍率]
    */
    //------------------------------------------------------------------------
    public float GetOffenceElementRate(MasterDataDefineLabel.ElementType elem)
    {
        return m_OffenceRateElem[(int)elem];
    }

    //------------------------------------------------------------------------
    /*!
        @brief		種族攻撃力倍率取得			<static>
        @param[in]	int			(kind)		種族
        @return		float		[攻撃力倍率]
    */
    //------------------------------------------------------------------------
    public float GetOffenceKindRate(MasterDataDefineLabel.KindType kind)
    {
        if (kind < MasterDataDefineLabel.KindType.NONE ||
            kind >= MasterDataDefineLabel.KindType.MAX)
        {
            return 1.0f;
        }

        return m_OffenceRateKind[(int)kind];
    }

    //------------------------------------------------------------------------
    /*!
        @brief		ダメージ軽減倍率取得(属性限定)
        @param[in]	int			(elem)			属性
        @return		float		[ダメージ軽減倍率]
    */
    //------------------------------------------------------------------------
    public float GetDeffenceElementRate(MasterDataDefineLabel.ElementType elem)
    {
        if (elem < MasterDataDefineLabel.ElementType.NONE ||
            elem >= MasterDataDefineLabel.ElementType.MAX)
        {
            return 1.0f;
        }

        return m_DefenceDamageDownRateElem[(int)elem];
    }

    //------------------------------------------------------------------------
    /*!
        @brief		被ダメージ増加倍率取得(属性限定)
        @param[in]	int			(elem)			属性
        @return		float		[被ダメージ増加倍率]
        @note		Developer 2016/04/05 v340 バリア無視対応
    */
    //------------------------------------------------------------------------
    public float GetDeffenceElementRateGain(MasterDataDefineLabel.ElementType elem)
    {
        if (elem < MasterDataDefineLabel.ElementType.NONE
        || elem >= MasterDataDefineLabel.ElementType.MAX)
        {
            return 1.0f;
        }

        return m_DefenceDamageUpRateElem[(int)elem];
    }

    //------------------------------------------------------------------------
    /*!
        @brief		ダメージ軽減倍率取得			<static>
        @return		float		[ダメージ軽減倍率]
    */
    //------------------------------------------------------------------------
    public float GetDefenceRate()
    {
        return m_DefenceDamageDownRate;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		被ダメージ増加倍率取得		<static>
        @return		float		[被ダメージ増加倍率]
        @note		Developer 2016/04/05 v340 バリア無視対応
    */
    //------------------------------------------------------------------------
    public float GetDefenceRateGain()
    {
        return m_DefenceDamageUpRate;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		防御減衰倍率取得				<static>
        @return		float		[防御減衰倍率]
    */
    //----------------------------------------------------------------------------
    public float GetDefenceWeekRate()
    {
        return m_DefenceArmorRate;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		毒ダメージ取得					<static>
        @return		float	[ダメージ量(%)]
        @change		Developer 2016/02/26 v330 毒複数対応
    */
    //------------------------------------------------------------------------
    public float GetPoisonDamage(PoisonType type)
    {
        return fPoisonDamage[(int)type];
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		カウントダウン変化				<static>
        @return		int		[カウントダウン変化量]
    */
    //----------------------------------------------------------------------------
    public int GetCountDown()
    {
        return nCountDown;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		暗闇状態かを調べる				<static>
        @retval		bool		[暗闇である/ない]
    */
    //------------------------------------------------------------------------
    public bool GetDark()
    {
        return bStatus[(int)MasterDataDefineLabel.AilmentType.DARK];
    }

    //------------------------------------------------------------------------
    /*!
        @brief		パネル属性の固定				<static>
        @retval		int		[固定された属性]
    */
    //------------------------------------------------------------------------
    public MasterDataDefineLabel.ElementType GetPanelElemFix()
    {
        return nFixElement;
    }


    //------------------------------------------------------------------------
    /*!
        @brief		スキル使用禁止					<static>
        @retval		bool	[スキル使用不可/可]
    */
    //------------------------------------------------------------------------
    public bool GetSkillNotUse()
    {
        return bStatus[(int)MasterDataDefineLabel.AilmentType.SKILL_NOTUSE];
    }


    //------------------------------------------------------------------------
    /*!
        @brief		リジェネ効果取得				<static>
        @return		int		[リジェネ効果量]
    */
    //------------------------------------------------------------------------
    public BattleSceneUtil.MultiInt GetHealValue()
    {
        BattleSceneUtil.MultiInt heal_value = new BattleSceneUtil.MultiInt();
        heal_value.setValue(GlobalDefine.PartyCharaIndex.MAX, (int)fHealValue);
        return heal_value;
    }


    //------------------------------------------------------------------------
    /*!
        @brief		手札出現率デフォルト		<static>
        @retval		bool	[デフォルト効果あり/なし]
    */
    //------------------------------------------------------------------------
    public bool GetHandCardDefault()
    {
        return bStatus[(int)MasterDataDefineLabel.AilmentType.HANDCARD_DEFAULT];
    }


    //------------------------------------------------------------------------
    /*!
        @brief		回復不可[全]					<static>
        @retval		bool	[スキル使用不可/可]
    */
    //------------------------------------------------------------------------
    public bool GetNonRecoveryAll()
    {
        return bStatus[(int)MasterDataDefineLabel.AilmentType.NON_RECOVERY_ALL];
    }

    /// <summary>
    /// 攻撃力変化バフの合計値を取得（画面表示用なので、ロジックで使っても問題ないかまでは確認していません）
    /// </summary>
    /// <param name="dest_rate"></param>
    /// <param name="element_type"></param>
    /// <param name="kind_type"></param>
    /// <param name="sub_kind_type"></param>
    /// <returns></returns>
    public bool GetOffenceRateTotal(out float dest_rate, MasterDataDefineLabel.ElementType element_type, MasterDataDefineLabel.KindType kind_type, MasterDataDefineLabel.KindType sub_kind_type)
    {
        dest_rate = 1.0f;
        // 攻撃力倍率
        dest_rate = InGameUtilBattle.AvoidErrorMultiple(dest_rate, GetOffenceRate());
        // 属性攻撃力倍率
        // ※属性エンハンス系、属性逆エンハンス系の計算処理。属性参照はユニット単位で行うこと！
        dest_rate = InGameUtilBattle.AvoidErrorMultiple(dest_rate, GetOffenceElementRate(element_type));
        // 種族攻撃力倍率(メイン
        dest_rate = InGameUtilBattle.AvoidErrorMultiple(dest_rate, GetOffenceKindRate(kind_type));
        // 種族攻撃力倍率(サブ)
        if (sub_kind_type != MasterDataDefineLabel.KindType.NONE)
        {
            dest_rate = InGameUtilBattle.AvoidErrorMultiple(dest_rate, GetOffenceKindRate(sub_kind_type));
        }

        return m_IsExistOffenceAilment;
    }

    /// <summary>
    /// 防御変化バフの最大値を取得（画面表示用なので、ロジックで使っても問題ないかまでは確認していません）
    /// プレイヤー側でのみ使用可能（敵側キャラには防御力があるので計算できない。プレイヤーキャラは防御力は常にゼロ）
    /// </summary>
    /// <param name="dest_rate"></param>
    /// <returns></returns>
    public bool GetDefenceDamageRateMaxForPlayer(out float dest_rate)
    {
        dest_rate = 1.0f;

        float damage_up_max = 1.0f;
        float damage_down_max = 1.0f;
        for (int idx = 0; idx < cAilment.Length; idx++)
        {
            float damage_up_base = GetDefenceRateGain();
            float damage_down_base = GetDefenceRate();

            for (MasterDataDefineLabel.ElementType elem_idx = MasterDataDefineLabel.ElementType.NONE + 1; elem_idx < MasterDataDefineLabel.ElementType.MAX - 1; elem_idx++)
            {
                float damage_up_elem = GetDeffenceElementRateGain(elem_idx);
                float damage_down_elem = GetDeffenceElementRate(elem_idx);

                float damage_up = damage_up_base * damage_up_elem;
                float damage_down = damage_down_base * damage_down_elem;

                if (damage_up > damage_up_max)
                {
                    damage_up_max = damage_up;
                }

                if (damage_down < damage_down_max)
                {
                    damage_down_max = damage_down;
                }
            }
        }

        dest_rate = damage_up_max * damage_down_max;

        return m_IsExistDefenceAilment;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		特定状態異常のクリア
        @parma[in]	int		(type)		スキルカテゴリ
    */
    //------------------------------------------------------------------------
    public void DelStatusAilment(MasterDataDefineLabel.AilmentType category)
    {
        //--------------------------------
        //	状態異常管理バッファを洗い、該当の状態異常をクリアする
        //--------------------------------
        StatusAilment ailment = null;
        for (int n = 0; n < cAilment.Length; n++)
        {
            ailment = cAilment[n];

            // 未使用
            if (ailment.bUsed == false)
            {
                continue;
            }

            // カテゴリチェック
            if (ailment.nType != category)
            {
                continue;
            }


            // ステータスの解除
            MasterDataDefineLabel.AilmentType nType = category;
            if (nType >= MasterDataDefineLabel.AilmentType.NONE
                && nType < MasterDataDefineLabel.AilmentType.MAX)
            {

                bStatus[(int)nType] = false;

                // 未使用領域に変更
                ailment.bUsed = false;

            }
            else
            {

                continue;

            }
        }
    }

    //------------------------------------------------------------------------
    /*!
        @brief		特定状態異常のクリア
        @param[in]	int		(id)		管理ID
        @parma[in]	int		(type)		スキルカテゴリ
        @note		既存のでは正常にクリアできていないようだったので、改良版を作成(2015/03/06)
    */
    //------------------------------------------------------------------------
    private void DelStatusAilment2(MasterDataDefineLabel.AilmentType type)
    {
        //--------------------------------
        //	エラーチェック(idチェック、遅延チェック)
        //--------------------------------
        if (type == MasterDataDefineLabel.AilmentType.FEAR)
        {
            return;
        }

        //--------------------------------
        //	状態異常管理バッファを洗い、該当の状態異常をクリアする
        //--------------------------------
        //		MasterDataStatusAilmentParam	master	= null;
        StatusAilment ailment = null;
        for (int n = 0; n < cAilment.Length; ++n)
        {
            //			master	= MasterDataUtil.GetMasterDataStatusAilmentParam( cAilment[ n ].nMasterDataStatusAilmentID );
            ailment = cAilment[n];
            if (ailment == null
            || ailment.bUsed == false)
            {
                continue;
            }

            // カテゴリチェック
            if (ailment.nType != type)
            {
                continue;
            }

            // ステータスの解除
            if (type >= MasterDataDefineLabel.AilmentType.NONE
               && type < MasterDataDefineLabel.AilmentType.MAX)
            {

                bStatus[(int)type] = false;

                //--------------------------------
                //	状態異常のクリア
                //--------------------------------
                ailment.ClearData();
            }
        }

    }

    //------------------------------------------------------------------------
    /*!
        @brief		状態異常のクリア		<static>
    */
    //------------------------------------------------------------------------
    public void DelAllStatusAilment()
    {
        //--------------------------------
        //	状態異常管理バッファのクリア
        //--------------------------------
        for (int i = 0; i < bStatus.Length; i++)
        {

            //--------------------------------
            //	ステータスを解除
            //--------------------------------
            bStatus[i] = false;

        }


        //--------------------------------
        //	状態異常のクリア
        //--------------------------------
        for (int i = 0; i < cAilment.Length; i++)
        {
            if (cAilment[i] == null)
            {
                continue;
            }

            cAilment[i].ClearData();
        }


        //--------------------------------
        //	各種パラメータの変化をクリア
        //	@change Developer 2016/04/05 v340 バリア無視対応、変数追加
        //--------------------------------
        m_IsExistOffenceAilment = false;
        m_IsExistDefenceAilment = false;
        m_OffenceRate = 1.0f;
        m_DefenceDamageDownRate = 1.0f;
        m_DefenceDamageUpRate = 1.0f;
        m_DefenceArmorRate = 1.0f;
        nCountDown = 0;

        nFixElement = MasterDataDefineLabel.ElementType.NONE;

        for (int i = 0; i < (int)PoisonType.MAX; ++i)
        {
            fPoisonDamage[i] = 0.0f;
        }

        for (int i = 0; i < m_OffenceRateElem.Length; i++)
        {
            m_OffenceRateElem[i] = 1.0f;
        }

        for (int i = 0; i < m_OffenceRateKind.Length; i++)
        {
            m_OffenceRateKind[i] = 1.0f;
        }

        for (int i = 0; i < m_DefenceDamageDownRateElem.Length; i++)
        {
            m_DefenceDamageDownRateElem[i] = 1.0f;
        }

        for (int i = 0; i < m_DefenceDamageUpRateElem.Length; i++)
        {
            m_DefenceDamageUpRateElem[i] = 1.0f;
        }

        for (int i = 0; i < m_DefenceDanageRateKind.Length; i++)
        {
            m_DefenceDanageRateKind[i] = 1.0f;
        }
    }

    //------------------------------------------------------------------------
    /*!
        @brief		状態異常の全クリア：指定都合
        @param[in]	int		(ailment_id)			状態異常ID
        @param[in]	int		(ailment_goodOrbad)		都合が良い/悪い(MasterDataDefineLabel.BOOL_ENABLE)
    */
    //------------------------------------------------------------------------
    public void DelStatusAilmentCondition(StatusAilment.GoodOrBad ailment_goodOrbad)
    {
        //------------------------------
        //	エラーチェック
        //------------------------------
        if (ailment_goodOrbad == StatusAilment.GoodOrBad.NONE)
        {
            return;
        }

        MasterDataStatusAilmentParam ailmentMaster = null;
        bool clearFlag = false;

        for (int num = 0; num < cAilment.Length; ++num)
        {

            // 使用していない、マスターデータがない場合
            if (cAilment[num] == null
               || cAilment[num].bUsed == false
               || cAilment[num].nMasterDataStatusAilmentID == 0)
            {
                continue;
            }

            // マスターデータを取得
            ailmentMaster = BattleParam.m_MasterDataCache.useAilmentParam((uint)cAilment[num].nMasterDataStatusAilmentID);

            // 条件確認(指定都合が合っているか、消すことができる状態異常か)
            if (ailmentMaster.good_or_bad != ailment_goodOrbad
               || ailmentMaster.clear_type != MasterDataDefineLabel.BoolType.ENABLE)
            {
                continue;
            }

            //--------------------------------
            //	各種パラメータの変化をクリア
            //	@change Developer 2016/04/05 v340 バリア無視対応、変数追加
            //--------------------------------
            if (clearFlag == false)
            {
                m_IsExistOffenceAilment = false;
                m_IsExistDefenceAilment = false;
                m_OffenceRate = 1.0f;
                m_DefenceDamageDownRate = 1.0f;
                m_DefenceDamageUpRate = 1.0f;
                m_DefenceArmorRate = 1.0f;
                nCountDown = 0;
                nFixElement = MasterDataDefineLabel.ElementType.NONE;

                for (int i = 0; i < (int)StatusAilmentChara.PoisonType.MAX; ++i)
                {
                    fPoisonDamage[i] = 0.0f;
                }

                for (int i = 0; i < m_OffenceRateElem.Length; ++i)
                {
                    m_OffenceRateElem[i] = 1.0f;
                }

                for (int i = 0; i < m_OffenceRateKind.Length; ++i)
                {
                    m_OffenceRateKind[i] = 1.0f;
                }

                for (int i = 0; i < m_DefenceDamageDownRateElem.Length; ++i)
                {
                    m_DefenceDamageDownRateElem[i] = 1.0f;
                }

                for (int i = 0; i < m_DefenceDamageUpRateElem.Length; ++i)
                {
                    m_DefenceDamageUpRateElem[i] = 1.0f;
                }

                for (int i = 0; i < m_DefenceDanageRateKind.Length; ++i)
                {
                    m_DefenceDanageRateKind[i] = 1.0f;
                }

                clearFlag = true;
            }

            // 指定都合の状態異常のクリア
            DelStatusAilment2(cAilment[num].nType);
            //			DelStatusAilment( ailment_id, cAilment[ num ].nType );				// ←こちらでは正常にクリアできず、チェック判定がおかしくなる
        }

        // 状態異常に変化があれば、効果などを再チェック
        if (clearFlag == true)
        {
            CheckEffectiveStatusAilment();
        }

    }

    //------------------------------------------------------------------------
    /*!
        @brief		指定管理番号のユニットに指定都合の状態変化がかかっているかを確認
        @param[in]	int		(ailment_goodOrbad)		都合が良い/悪い(MasterDataDefineLebel.BOOL_ENABLE)
    */
    //------------------------------------------------------------------------
    public bool ChkStatusAilmentCondition(StatusAilment.GoodOrBad ailment_goodOrbad)
    {
        return ChkStatusCondition(ailment_goodOrbad);
    }


    //------------------------------------------------------------------------
    /*!
        @brief		消せる状態異常があるか確認
        @param[in]	int		(ailment_id)			状態異常ID
        @param[in]	int		(ailment_goodOrbad)		都合が良い/悪い(MasterDataDefineLebel.BOOL_ENABLE)
        @retval		bool	[true:有り/false:無し]
    */
    //------------------------------------------------------------------------
    public bool ChkStatusAilmentClearType(StatusAilment.GoodOrBad ailment_goodOrbad)
    {

        bool result = false;

        for (int num = 0; num < cAilment.Length; ++num)
        {

            // 使用していない、マスターデータがない場合
            if (cAilment[num] == null
               || cAilment[num].bUsed == false
               || cAilment[num].nMasterDataStatusAilmentID == 0)
            {
                continue;
            }

            // 各種取得
            MasterDataStatusAilmentParam ailmentMaster = BattleParam.m_MasterDataCache.useAilmentParam((uint)cAilment[num].nMasterDataStatusAilmentID);

            // 条件確認(指定都合が合っているか、消すことができる状態異常か)
            if (ailmentMaster.good_or_bad == ailment_goodOrbad
               && ailmentMaster.clear_type == MasterDataDefineLabel.BoolType.ENABLE)
            {
                // 1つでもあれば発動できる
                result = true;
                break;
            }
        }


        return (result);
    }

    public int GetAilmentCount()
    {
        return STATUSAILMENT_MAX;
    }

    // このキャラが現在かかっている状態異常を取得.
    public StatusAilment GetAilment(int idx)
    {
        if (idx < 0
        || idx >= STATUSAILMENT_MAX
        || cAilment[idx] == null
        || cAilment[idx].bUsed == false)
        {
            return null;
        }

        return cAilment[idx];
    }

    // このキャラが指定の型の状態異常にかかっているかを調べる.
    public bool IsHavingAilment(MasterDataDefineLabel.AilmentType ailment_type)
    {
        return bStatus[(int)ailment_type];
    }

    // この状態異常の所有者の種類
    public OwnerType GetOwnerType()
    {
        return m_OwnerType;
    }

    /// <summary>
    /// セーブデータから復帰
    /// </summary>
    public void restoreFromSaveData()
    {
        StatusAilment[] wrk_ailments = cAilment;
        if (wrk_ailments != null)
        {
            OwnerType wrk_owner_type = m_OwnerType;

            // 一旦初期化
            _init(wrk_owner_type);


            for (int idx = wrk_ailments.Length - 1; idx >= 0; idx--)
            {
                StatusAilment wrk_ailment = wrk_ailments[idx];
                if (wrk_ailment != null
                    && wrk_ailment.bUsed
                    && wrk_ailment.nMasterDataStatusAilmentID != 0
                )
                {
                    AddStatusAilment(wrk_ailment, null);
                }
            }
        }
    }
} // class StatusAilmentmChara


