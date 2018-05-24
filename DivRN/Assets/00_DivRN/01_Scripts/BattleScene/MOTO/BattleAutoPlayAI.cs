using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAutoPlayAI : BattleAutoPlay.AI
{
    private const int PARTY_MEMBER_NUM = 5;
    private const int FIELD_AREA_NUM = 5;
    private const int HAND_AREA_NUM = 5;

    private const int PANEL_PUT_COUNT_BASE = 9;
    private int m_HandIndex = 0;    // 手札を左から順番に使用するための変数
    private CostInfo[] m_SkillOrder = null; // 優先的に置きたいスキル順
    private SkillWorksInfo m_SkillWorksInfo = new SkillWorksInfo(); // スキル効果

    private bool m_IsUseLimitBreakSkill = false;    // リミブレスキルを自動使用するかどうか
    private bool m_IsStopOnEntryBossBattle = false; // ボス戦突入時にオートプレイを停止するかどうか

    private int[] m_LimitBreakSkillTurnID = new int[PARTY_MEMBER_NUM];

    // スキルの優先順位は、リーダー・ＭＯＢ１・ＭＯＢ２・ＭＯＢ３・フレンドの順
    private readonly GlobalDefine.PartyCharaIndex[] MEMBER_ORDER = {
        GlobalDefine.PartyCharaIndex.LEADER,
        GlobalDefine.PartyCharaIndex.MOB_1,
        GlobalDefine.PartyCharaIndex.MOB_2,
        GlobalDefine.PartyCharaIndex.MOB_3,
        GlobalDefine.PartyCharaIndex.FRIEND,
    };

    // オートプレイ停止チェック用変数
    private bool m_IsBossBattle = false;

    public override int getPanelPutCount()
    {
        return m_SkillWorksInfo.getPanelPutCount();
    }

    public override void startAutoPlay()
    {
        setSkillInfo();

        for (int member_idx = 0; member_idx < m_LimitBreakSkillTurnID.Length; member_idx++)
        {
            m_LimitBreakSkillTurnID[member_idx] = 0;
        }

        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
        m_IsUseLimitBreakSkill = (cOption.m_OptionAutoPlayUseAS == (int)LocalSaveDefine.OptionAutoPlayUseAS.ON);
        m_IsStopOnEntryBossBattle = (cOption.m_OptionAutoPlayStopBoss == (int)LocalSaveDefine.OptionAutoPlayStopBoss.ON);

        isStopAutoPlay();
    }

    public override void initThink(PanelInfo panel_info)
    {
        setSkillInfo();

        m_HandIndex = -1;

        m_SkillOrder = m_SkillWorksInfo.getElementOrder(panel_info, MEMBER_ORDER);
    }

    public override void execThink(ref int dest_hand_index, ref int dest_field_index, PanelInfo panel_info)
    {
        m_HandIndex = (m_HandIndex + 1) % panel_info.m_HandElements.Length;

        int[] work_field_cost_empty_count = new int[panel_info.m_FieldElements.Length];
        for (int field_idx = 0; field_idx < work_field_cost_empty_count.Length; field_idx++)
        {
            work_field_cost_empty_count[field_idx] = panel_info.m_FieldElements[field_idx].m_CostCountMax - panel_info.m_FieldElements[field_idx].m_CostCountUsing;
        }

        // 置きたい属性などの指定が何もない場合は、右側から埋めていく
        if (m_SkillOrder.IsNullOrEmpty())
        {
            // 置きたい属性が無い場合は、優先順位が低い場（右側優先）に適当なパネルを置く
            for (int field_idx = panel_info.m_FieldElements.Length - 1; field_idx >= 0; field_idx--)
            {
                CostInfo field_cost_info = panel_info.m_FieldElements[field_idx];
                if (field_cost_info.m_CostCountUsing < field_cost_info.m_CostCountMax)
                {
                    dest_hand_index = m_HandIndex;
                    dest_field_index = field_idx;
                    return;
                }
            }
        }

        // 優先的に置きたい属性があればそれを置く
        for (int skill_order_idx = 0; skill_order_idx < m_SkillOrder.Length; skill_order_idx++)
        {
            CostInfo skill_cost_info = m_SkillOrder[skill_order_idx];
            if (skill_cost_info != null && skill_cost_info.m_CostCountUsing > 0)
            {
                // このスキルが既に成立済みかチェックし、成立済みならスキップ(TODO:同じスキルを２回以上成立させたい場合はどうするか)
                bool is_formed = false;
                for (int field_idx = 0; field_idx < panel_info.m_FieldElements.Length; field_idx++)
                {
                    CostInfo field_cost_info = panel_info.m_FieldElements[field_idx];
                    bool is_formed2 = true;
                    for (MasterDataDefineLabel.ElementType element_idx = MasterDataDefineLabel.ElementType.NONE + 1; element_idx < MasterDataDefineLabel.ElementType.MAX; element_idx++)
                    {
                        int field_cost_count = field_cost_info.getCostCount(element_idx);
                        int skill_cost_count = skill_cost_info.getCostCount(element_idx);

                        if (field_cost_count < skill_cost_count)
                        {
                            is_formed2 = false;
                            break;
                        }
                    }

                    if (is_formed2)
                    {
                        is_formed = true;
                        break;
                    }
                }

                if (is_formed)
                {
                    continue;
                }

                // まだこのスキルは成立していないのでどこにパネルを出すか決める
                // スキル成立までの手数が一番少ないところに出す。
                CostInfo[] skill_request_cost_infos = new CostInfo[panel_info.m_FieldElements.Length];    // 各場でスキルが後何枚パネルを必要としているかの情報
                int min_skill_request_cost_count = 999; // 最小の必要枚数
                {
                    for (int field_idx = 0; field_idx < panel_info.m_FieldElements.Length; field_idx++)
                    {
                        CostInfo field_cost_info = panel_info.m_FieldElements[field_idx];
                        if (field_cost_info.m_CostCountUsing < field_cost_info.m_CostCountMax)
                        {
                            CostInfo skill_request_cost_info = new CostInfo(skill_cost_info);

                            for (MasterDataDefineLabel.ElementType element_idx = MasterDataDefineLabel.ElementType.NONE + 1; element_idx < MasterDataDefineLabel.ElementType.MAX; element_idx++)
                            {
                                int field_cost_count = field_cost_info.getCostCount(element_idx);
                                skill_request_cost_info.addCost(element_idx, -field_cost_count);

                                skill_request_cost_infos[field_idx] = skill_request_cost_info;
                            }

                            if (skill_request_cost_info.m_CostCountUsing >= 0
                                && skill_request_cost_info.m_CostCountUsing < min_skill_request_cost_count)
                            {
                                min_skill_request_cost_count = skill_request_cost_info.m_CostCountUsing;
                            }
                        }
                    }
                }

                for (int boost_idx = 0; boost_idx <= 1; boost_idx++)
                {
                    bool is_boost = (boost_idx == 0) ? true : false; // BOOST場を優先
                    for (int field_idx = 0; field_idx < panel_info.m_FieldElements.Length; field_idx++)
                    {
                        if (panel_info.m_IsBoosts[field_idx] == is_boost)
                        {
                            CostInfo field_cost_info = panel_info.m_FieldElements[field_idx];
                            if (field_cost_info.m_CostCountUsing < field_cost_info.m_CostCountMax)
                            {
                                CostInfo skill_request_cost_info = skill_request_cost_infos[field_idx];
                                if (skill_request_cost_info != null
                                    && skill_request_cost_info.m_CostCountUsing == min_skill_request_cost_count
                                )
                                {
                                    if (work_field_cost_empty_count[field_idx] >= skill_request_cost_info.m_CostCountUsing)    // スキルを成立させるために場に十分に空きがあるかどうか
                                    {
                                        // 必要枚数が一番多い属性を優先的に出す
                                        for (int element_count = 5; element_count >= 1; element_count--)
                                        {
                                            for (MasterDataDefineLabel.ElementType element_idx = MasterDataDefineLabel.ElementType.NONE + 1; element_idx < MasterDataDefineLabel.ElementType.MAX; element_idx++)
                                            {
                                                if (skill_request_cost_info.getCostCount(element_idx) == element_count)
                                                {
                                                    for (int hand_idx = 0; hand_idx < panel_info.m_HandElements.Length; hand_idx++)
                                                    {
                                                        int wrk_hand_idx = (m_HandIndex + hand_idx) % panel_info.m_HandElements.Length;
                                                        if (panel_info.m_HandElements[wrk_hand_idx] == element_idx)
                                                        {
                                                            dest_hand_index = wrk_hand_idx;
                                                            dest_field_index = field_idx;
                                                            return;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        // パネルを置ける領域数を、このスキルが使用する予定数分減らしておく
                                        {
                                            int max_reserve_cost_count = 5 - skill_order_idx;   // 優先順位が高いスキルほど多く予約できる
                                            if (max_reserve_cost_count > 0)
                                            {
                                                int reserve_cost_count = skill_cost_info.m_CostCountUsing;
                                                if (reserve_cost_count > max_reserve_cost_count)
                                                {
                                                    reserve_cost_count = max_reserve_cost_count;
                                                }
                                                work_field_cost_empty_count[field_idx] -= reserve_cost_count;
                                            }
                                        }

                                        // 二重ループから一気に抜ける
                                        boost_idx = 2;
                                        field_idx = panel_info.m_FieldElements.Length;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // 置ける属性が無かった場合、優先順位が低い場（ブースト以外優先、右側優先）に適当なパネルを置く
        for (int boost_idx = 0; boost_idx <= 1; boost_idx++)
        {
            bool is_boost = (boost_idx == 0) ? false : true; // BOOST以外を優先
            for (int field_idx = panel_info.m_FieldElements.Length - 1; field_idx >= 0; field_idx--)
            {
                if (panel_info.m_IsBoosts[field_idx] == is_boost)
                {
                    CostInfo field_cost_info = panel_info.m_FieldElements[field_idx];
                    if (field_cost_info.m_CostCountUsing < field_cost_info.m_CostCountMax)
                    {
                        dest_hand_index = m_HandIndex;
                        dest_field_index = field_idx;
                        return;
                    }

                }
            }
        }
    }

    public override GlobalDefine.PartyCharaIndex getLimitBreakSkillCaster()
    {
        // リミブレスキル使用禁止でないかチェック
        if (m_IsUseLimitBreakSkill)
        {
            int battle_turn_id = getBattleTurnID();

            for (int member_idx = 0; member_idx < MEMBER_ORDER.Length; member_idx++)
            {
                GlobalDefine.PartyCharaIndex member_type = MEMBER_ORDER[member_idx];
                // このターンでまだリミブレスキルを使用していないかチェック（１ターン中に何度も発動できるリミブレスキル対策）
                if (m_LimitBreakSkillTurnID[(int)member_type] != battle_turn_id)
                {
                    // リミブレスキルが使用可能かチェック
                    if (BattleParam.IsEnableLBS(member_type))
                    {
                        m_LimitBreakSkillTurnID[(int)member_type] = battle_turn_id;
                        return member_type;
                    }
                }
            }
        }

        return GlobalDefine.PartyCharaIndex.ERROR;
    }

    public override bool isSkipCountDown()
    {
        int skip_on_hands = m_SkillWorksInfo.getSkipOnHands();
        if (skip_on_hands > 0)
        {
            int hands_count = 0;
            for (GlobalDefine.PartyCharaIndex member_idx = GlobalDefine.PartyCharaIndex.LEADER; member_idx < GlobalDefine.PartyCharaIndex.MAX; member_idx++)
            {
                hands_count += BattleParam.getPartyMemberHands(member_idx);
            }

            if (hands_count >= skip_on_hands)
            {
                return true;
            }
        }

        for (int skill_idx = 0; skill_idx < 2; skill_idx++)
        {
            for (GlobalDefine.PartyCharaIndex member_idx = GlobalDefine.PartyCharaIndex.LEADER; member_idx < GlobalDefine.PartyCharaIndex.MAX; member_idx++)
            {
                bool is_skip = m_SkillWorksInfo.isSkipOnSkillFormed(skill_idx, member_idx);
                if (is_skip)
                {
                    int skill_count = BattleParam.getFormedSkillCounts(skill_idx, (GlobalDefine.PartyCharaIndex)member_idx);
                    if (skill_count > 0)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public override bool isStopAutoPlay()
    {
        bool ret_val = false;

        bool is_boss_battle = BattleParam.m_BattleRequest.isBoss;

        if (m_IsStopOnEntryBossBattle)
        {
            // ボス戦突入か
            if (m_IsBossBattle == false && is_boss_battle)
            {
                ret_val = true;
            }
        }

        m_IsBossBattle = is_boss_battle;

        return ret_val;
    }


    private int getBattleTurnID()
    {
        int ret_val = 1 + BattleParam.m_BattleRequest.m_BattleUniqueID + BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleTotalTurn * 1000;
        return ret_val;
    }

    private void setSkillInfo()
    {
        int panel_put_count_base = PANEL_PUT_COUNT_BASE;
#if BUILD_TYPE_DEBUG
        // パネル配置数をデバッグメニューの値で上書き
        {
            int debug_panel_put_count = BattleDebugMenu.getAutoPlayPanelPutCount();
            if (debug_panel_put_count > 0)
            {
                panel_put_count_base = debug_panel_put_count;
            }
        }
#endif //BUILD_TYPE_DEBUG

        m_SkillWorksInfo.init(panel_put_count_base);

        // オートプレイスキルの効果を集約
        bool is_applied_ailment = false;
        for (int member_idx = 0; member_idx < MEMBER_ORDER.Length; member_idx++)
        {
            GlobalDefine.PartyCharaIndex member_type = MEMBER_ORDER[member_idx];

            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember(member_type, CharaParty.CharaCondition.SKILL_PASSIVE);
            if (chara_once != null)
            {
#if BUILD_TYPE_DEBUG
                // デバッグメニューで設定されたスキルを適用
                {
                    MasterDataDefineLabel.AutoPlaySkillType[] debug_skills = BattleDebugMenu.getAutoPlaySkill(member_type);
                    if (debug_skills.IsNullOrEmpty() == false)
                    {
                        for (int debug_skill_idx = 0; debug_skill_idx < debug_skills.Length; debug_skill_idx++)
                        {
                            MasterDataDefineLabel.AutoPlaySkillType debug_skill_type = debug_skills[debug_skill_idx];
                            m_SkillWorksInfo.applySkill(debug_skill_type, member_type, false);
                        }
                    }
                }
#endif //BUILD_TYPE_DEBUG

                // メインユニットのパッシブスキル
                {
                    MasterDataSkillPassive passive_skill_master = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
                    if (passive_skill_master != null)
                    {
                        m_SkillWorksInfo.applySkill(passive_skill_master, member_type, false);
                    }
                }

                // リンクユニットのパッシブスキル
                if (chara_once.m_LinkParam != null
                    && chara_once.m_LinkParam.m_CharaID != 0
                )
                {
                    MasterDataParamChara link_chara_master = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
                    if (link_chara_master != null)
                    {
                        MasterDataSkillPassive link_passive_skill_master = BattleParam.m_MasterDataCache.useSkillPassive(link_chara_master.link_skill_passive);
                        if (link_passive_skill_master != null)
                        {
                            m_SkillWorksInfo.applySkill(link_passive_skill_master, member_type, true);
                        }
                    }
                }

                // 状態異常にあるオートプレイ用スキルを反映
                if (is_applied_ailment == false)
                {
                    is_applied_ailment = true;
                    StatusAilmentChara status_ailment_chara = BattleParam.m_PlayerParty.m_Ailments.getAilment(member_type);
                    if (status_ailment_chara != null)
                    {
                        for (int ailment_idx = 0; ailment_idx < status_ailment_chara.GetAilmentCount(); ailment_idx++)
                        {
                            StatusAilment status_ailment = status_ailment_chara.GetAilment(ailment_idx);
                            if (status_ailment != null)
                            {
                                if (status_ailment.nType == MasterDataDefineLabel.AilmentType.AUTO_PLAY_SKILL)
                                {
                                    MasterDataStatusAilmentParam ailment_param = BattleParam.m_MasterDataCache.useAilmentParam((uint)status_ailment.nMasterDataStatusAilmentID);
                                    if (ailment_param != null)
                                    {
                                        MasterDataDefineLabel.AutoPlaySkillType auto_play_skill_type = ailment_param.Get_AUTO_PLAY_SKILL_TYPE();
                                        m_SkillWorksInfo.applySkill(auto_play_skill_type, member_type, false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        m_SkillWorksInfo.fix();
    }


    /// <summary>
    /// スキル効果
    /// </summary>
    private class SkillWorksInfo
    {
        private class SkillCost
        {
            public CostInfo m_CostInfo; // スキルのコスト情報
        }
        private SkillCost[/*main_or_link*/, /*member_index*/, /*skill_index*/] m_SkillCosts; // 各メンバーの集めたい属性
        private int m_PanelPutCount; // パネル配置枚数
        private bool m_IsTryResurrect;   // 復活を試みるかどうか
        private int m_SkipOnHands;   // 指定HANDS以上スキル成立でスキップ
        private bool[/*skill_index*/,/*member_index*/] m_SkipOnSkillFormeds; // スキル成立でスキップ（スキル別、メンバー別）

        public int getPanelPutCount()
        {
            return m_PanelPutCount;
        }

        public int getSkipOnHands()
        {
            return m_SkipOnHands;
        }

        public bool isSkipOnSkillFormed(int skill_index, GlobalDefine.PartyCharaIndex member_index)
        {
            return m_SkipOnSkillFormeds[skill_index, (int)member_index];
        }

        /// <summary>
        /// スキル効果を初期化
        /// </summary>
        public void init(int panel_put_count_base)
        {
            if (m_SkillCosts == null)
            {
                m_SkillCosts = new SkillCost[2, BattleParam.m_PlayerParty.getPartyMemberMaxCount(), 4];
                m_SkipOnSkillFormeds = new bool[2, BattleParam.m_PlayerParty.getPartyMemberMaxCount()];
            }

            for (int idx0 = 0; idx0 < m_SkillCosts.GetLength(0); idx0++)
            {
                for (int idx1 = 0; idx1 < m_SkillCosts.GetLength(1); idx1++)
                {
                    for (int idx2 = 0; idx2 < m_SkillCosts.GetLength(2); idx2++)
                    {
                        m_SkillCosts[idx0, idx1, idx2] = null;
                    }
                }
            }

            for (int idx0 = 0; idx0 < m_SkipOnSkillFormeds.GetLength(0); idx0++)
            {
                for (int idx1 = 0; idx1 < m_SkipOnSkillFormeds.GetLength(1); idx1++)
                {
                    m_SkipOnSkillFormeds[idx0, idx1] = false;
                }
            }

            m_PanelPutCount = panel_put_count_base;
            m_IsTryResurrect = false;
            m_SkipOnHands = 0;
        }


        /// <summary>
        /// スキル効果を確定
        /// </summary>
        public void fix()
        {
            // パネル配置枚数の上限、下限
            m_PanelPutCount = Mathf.Max(m_PanelPutCount, 1);
            m_PanelPutCount = Mathf.Min(m_PanelPutCount, 25);
        }

        /// <summary>
        /// 属性配置優先順を取得
        /// </summary>
        /// <param name="member_order"></param>
        /// <returns></returns>
        public CostInfo[] getElementOrder(PanelInfo panel_info, GlobalDefine.PartyCharaIndex[] member_order)
        {
            List<CostInfo> cost_info_order = new List<CostInfo>();

            // 復活があるときは回復を最優先で置く
            if (m_IsTryResurrect)
            {
                int sp = BattleParam.m_PlayerParty.GetSP();
                if (sp > 0)
                {
                    CharaOnce[] dead_charas = BattleParam.m_PlayerParty.getPartyMembers(CharaParty.CharaCondition.DEAD);
                    if (dead_charas.Length > 0)
                    {
                        for (int field_idx = 0; field_idx < panel_info.m_IsBoosts.Length; field_idx++)
                        {
                            if (panel_info.m_IsBoosts[field_idx])
                            {
                                cost_info_order.Add(new CostInfo(MasterDataDefineLabel.ElementType.HEAL, MasterDataDefineLabel.ElementType.HEAL, MasterDataDefineLabel.ElementType.HEAL, MasterDataDefineLabel.ElementType.HEAL, MasterDataDefineLabel.ElementType.HEAL));
                                break;
                            }
                        }
                    }
                }
            }

            // メインユニットのLEADER,MOB_1,MOB2,MOB3,FRIEND,リンクユニットのLEADER,MOB_1,MOB2,MOB3,FRIENDの順に並べる
            for (int skill_idx = 0; skill_idx < m_SkillCosts.GetLength(2); skill_idx++)
            {
                for (int link_idx = 0; link_idx < m_SkillCosts.GetLength(0); link_idx++)
                {
                    for (int member_idx = 0; member_idx < member_order.Length; member_idx++)
                    {
                        GlobalDefine.PartyCharaIndex member_type = member_order[member_idx];
                        SkillCost skill_cost = m_SkillCosts[link_idx, (int)member_type, skill_idx];
                        if (skill_cost != null
                            && skill_cost.m_CostInfo != null
                        )
                        {
                            bool is_enable_skill = true;
                            {
                                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember(member_type, CharaParty.CharaCondition.ALIVE);
                                if (chara_once == null)
                                {
                                    is_enable_skill = false;
                                }
                            }

                            if (is_enable_skill)
                            {
                                cost_info_order.Add(skill_cost.m_CostInfo);
                            }
                        }
                    }
                }
            }

            CostInfo[] ret_val = cost_info_order.ToArray();

            return ret_val;
        }

        /// <summary>
        /// スキル効果を適用
        /// </summary>
        /// <param name="passive_skill_master"></param>
        /// <param name="member_index"></param>
        /// <param name="is_link_member"></param>
        /// <param name="dest_param_ai"></param>
        public void applySkill(MasterDataSkillPassive passive_skill_master, GlobalDefine.PartyCharaIndex member_index, bool is_link_member)
        {
            for (MasterDataDefineLabel.AutoPlaySkillType skill_type = MasterDataDefineLabel.AutoPlaySkillType.NONE + 1; skill_type < MasterDataDefineLabel.AutoPlaySkillType.MAX; skill_type++)
            {
                bool is_exist = passive_skill_master.Check_AutoPlaySkillType(skill_type);
                if (is_exist)
                {
                    applySkill(skill_type, member_index, is_link_member);
                }
            }
        }

        /// <summary>
        /// スキル効果を適用
        /// </summary>
        /// <param name="skill_type"></param>
        /// <param name="passive_skill_master"></param>
        /// <param name="member_index"></param>
        /// <param name="is_link_member"></param>
        /// <param name="dest_param_ai"></param>
        public void applySkill(MasterDataDefineLabel.AutoPlaySkillType skill_type, GlobalDefine.PartyCharaIndex member_index, bool is_link_member)
        {
            CostInfo cost_info = null;
            int work_panel_plus = 0;
            bool work_is_resurrect = false;
            int skip_on_hands = 0;

            switch (skill_type)
            {
                case MasterDataDefineLabel.AutoPlaySkillType.SKILL1_SELF:
                    cost_info = _getActiveSkillCost(member_index, 0);
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.SKILL1_LEADER:
                    cost_info = _getActiveSkillCost(GlobalDefine.PartyCharaIndex.LEADER, 0);
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.SKILL2_SELF:
                    cost_info = _getActiveSkillCost(member_index, 1);
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.SKILL2_LEADER:
                    cost_info = _getActiveSkillCost(GlobalDefine.PartyCharaIndex.LEADER, 1);
                    break;


                case MasterDataDefineLabel.AutoPlaySkillType.TOGETHER_NAUGHT:
                    cost_info = new CostInfo(MasterDataDefineLabel.ElementType.NAUGHT, MasterDataDefineLabel.ElementType.NAUGHT, MasterDataDefineLabel.ElementType.NAUGHT, MasterDataDefineLabel.ElementType.NAUGHT, MasterDataDefineLabel.ElementType.NAUGHT);
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.TOGETHER_FIRE:
                    cost_info = new CostInfo(MasterDataDefineLabel.ElementType.FIRE, MasterDataDefineLabel.ElementType.FIRE, MasterDataDefineLabel.ElementType.FIRE, MasterDataDefineLabel.ElementType.FIRE, MasterDataDefineLabel.ElementType.FIRE);
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.TOGETHER_WATER:
                    cost_info = new CostInfo(MasterDataDefineLabel.ElementType.WATER, MasterDataDefineLabel.ElementType.WATER, MasterDataDefineLabel.ElementType.WATER, MasterDataDefineLabel.ElementType.WATER, MasterDataDefineLabel.ElementType.WATER);
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.TOGETHER_LIGHT:
                    cost_info = new CostInfo(MasterDataDefineLabel.ElementType.LIGHT, MasterDataDefineLabel.ElementType.LIGHT, MasterDataDefineLabel.ElementType.LIGHT, MasterDataDefineLabel.ElementType.LIGHT, MasterDataDefineLabel.ElementType.LIGHT);
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.TOGETHER_DARK:
                    cost_info = new CostInfo(MasterDataDefineLabel.ElementType.DARK, MasterDataDefineLabel.ElementType.DARK, MasterDataDefineLabel.ElementType.DARK, MasterDataDefineLabel.ElementType.DARK, MasterDataDefineLabel.ElementType.DARK);
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.TOGETHER_WIND:
                    cost_info = new CostInfo(MasterDataDefineLabel.ElementType.WIND, MasterDataDefineLabel.ElementType.WIND, MasterDataDefineLabel.ElementType.WIND, MasterDataDefineLabel.ElementType.WIND, MasterDataDefineLabel.ElementType.WIND);
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.TOGETHER_HEAL:
                    cost_info = new CostInfo(MasterDataDefineLabel.ElementType.HEAL, MasterDataDefineLabel.ElementType.HEAL, MasterDataDefineLabel.ElementType.HEAL, MasterDataDefineLabel.ElementType.HEAL, MasterDataDefineLabel.ElementType.HEAL);
                    break;


                case MasterDataDefineLabel.AutoPlaySkillType.TRY_RESURRECT:
                    work_is_resurrect = true;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.SKIP_ON_SKILL1_SELF:
                    m_SkipOnSkillFormeds[0, (int)member_index] = true;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.SKIP_ON_SKILL1_LEADER:
                    m_SkipOnSkillFormeds[0, (int)GlobalDefine.PartyCharaIndex.LEADER] = true;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.SKIP_ON_SKILL2_SELF:
                    m_SkipOnSkillFormeds[1, (int)member_index] = true;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.SKIP_ON_SKILL2_LEADER:
                    m_SkipOnSkillFormeds[1, (int)GlobalDefine.PartyCharaIndex.LEADER] = true;
                    break;


                case MasterDataDefineLabel.AutoPlaySkillType.SKIP_ON_HANDS_1:
                    skip_on_hands = 1;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.SKIP_ON_HANDS_2:
                    skip_on_hands = 2;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.SKIP_ON_HANDS_3:
                    skip_on_hands = 3;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.SKIP_ON_HANDS_4:
                    skip_on_hands = 4;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.SKIP_ON_HANDS_5:
                    skip_on_hands = 5;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.SKIP_ON_HANDS_6:
                    skip_on_hands = 6;
                    break;


                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_PLUS_1:
                    work_panel_plus = 1;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_PLUS_2:
                    work_panel_plus = 2;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_PLUS_3:
                    work_panel_plus = 3;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_PLUS_4:
                    work_panel_plus = 4;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_PLUS_5:
                    work_panel_plus = 5;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_PLUS_6:
                    work_panel_plus = 6;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_PLUS_7:
                    work_panel_plus = 7;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_PLUS_8:
                    work_panel_plus = 8;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_PLUS_9:
                    work_panel_plus = 9;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_PLUS_10:
                    work_panel_plus = 10;
                    break;


                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_DOWN_1:
                    work_panel_plus = -1;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_DOWN_2:
                    work_panel_plus = -2;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_DOWN_3:
                    work_panel_plus = -3;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_DOWN_4:
                    work_panel_plus = -4;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_DOWN_5:
                    work_panel_plus = -5;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_DOWN_6:
                    work_panel_plus = -6;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_DOWN_7:
                    work_panel_plus = -7;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_DOWN_8:
                    work_panel_plus = -8;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_DOWN_9:
                    work_panel_plus = -9;
                    break;

                case MasterDataDefineLabel.AutoPlaySkillType.PANEL_DOWN_10:
                    work_panel_plus = -10;
                    break;
            }

            if (cost_info != null)
            {
                int link_idx = (is_link_member ? 1 : 0);
                for (int skill_idx = 0; skill_idx < m_SkillCosts.GetLength(2); skill_idx++)
                {
                    if (m_SkillCosts[link_idx, (int)member_index, skill_idx] == null)
                    {
                        SkillCost skill_cost = new SkillCost();
                        skill_cost.m_CostInfo = cost_info;
                        m_SkillCosts[link_idx, (int)member_index, skill_idx] = skill_cost;
                        break;
                    }
                }
            }

            m_PanelPutCount += work_panel_plus;

            m_IsTryResurrect |= work_is_resurrect;

            if (m_SkipOnHands == 0)
            {
                m_SkipOnHands = skip_on_hands;
            }
        }

        private CostInfo _getActiveSkillCost(GlobalDefine.PartyCharaIndex member_index, int active_skill_no)
        {
            CostInfo ret_val = null;

            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember(member_index, CharaParty.CharaCondition.EXIST);
            if (chara_once != null)
            {
                uint active_skill_fix_id = (active_skill_no == 0) ? chara_once.m_CharaMasterDataParam.skill_active0 : chara_once.m_CharaMasterDataParam.skill_active1;
                MasterDataSkillActive master_data_skill_active = BattleParam.m_MasterDataCache.useSkillActive(active_skill_fix_id);
                if (master_data_skill_active != null)
                {
                    ret_val = new CostInfo(master_data_skill_active.cost1, master_data_skill_active.cost2, master_data_skill_active.cost3, master_data_skill_active.cost4, master_data_skill_active.cost5);
                }
            }

            return ret_val;
        }
    }
}
