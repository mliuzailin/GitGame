/// <summary>
/// ノーマルスキル成立判定クラス
/// </summary>
public class BattleSkillFormingCheck
{
    public struct ReachInfo
    {
        public int m_FieldIndex;
        public MasterDataDefineLabel.ElementType m_Element;
    }

    private bool m_IsAllocatedWorkArea = false; // 作業領域を確保済みかどうか

    private BattleSkillReq[] m_ResultFormedSkills = null;   //成立条件を満たしたスキル（結果）
    private int m_ResultFormedSkillsCount = 0;              //成立条件を満たしたスキル数（結果）
    private BattleSkillReq[] m_WorkFormedSkills = null;     //成立条件を満たしたスキル（作業用）
    private int m_WorkFormedSkillsCount = 0;                //成立条件を満たしたスキル数（作業用）

    private ReachInfo[] m_ResultReachInfos = null;          //リーチライン情報（結果）
    private int m_ResultReachInfosCount = 0;                //リーチライン情報数（結果）
    private ReachInfo[] m_WorkReachInfos = null;            //リーチライン情報（作業用）
    private int m_WorkReachInfosCount = 0;                  //リーチライン情報数（作業用）

    private bool[] m_WorkHandcardCostPerElement = null;     //手に持っているカードの種類（リーチ判定用）
    private int[,] m_WorkGeneralFieldCostInfo = null;       //汎用回復用残りコスト
    private int[][,] m_WorkMemberFieldCostInfo = null;      //各メンバー別残りコスト
    private CharaOnce[] m_WorkActiveSkillMembers = null;    //パーティメンバー
    private bool[] m_WorkIsAliveActiveSkillMembers = null;  //パーティメンバーが生存しているかどうか

    private MasterDataSkillActive[] m_OrderedSkillList = null;      // 優先順に並べられたスキル一覧
    private bool[] m_IsBoosts = null;                               // フィールドの「BOOST」情報
    private bool[] m_IsFulls = null;                                // フィールドの「FULL」情報

    private SkillRequestParam.SkillFilterType[] m_FilterTypes;  //前半スキルを判定するかしないか（ターン開始時に死亡しているメンバーは前半スキルを判定しない）

    private const int PHASE_WAIT = -1;
    private const int PHASE_MAX = 3;
    private int m_Phase = PHASE_WAIT;
    private bool m_IsCheck = false; // 成立チェックをする必要があるかどうか
    private bool m_IsFixResult = false;

    public BattleSkillFormingCheck(int BATTLE_SKILL_TOTAL_MAX)
    {
        m_ResultFormedSkills = new BattleSkillReq[BATTLE_SKILL_TOTAL_MAX];
        m_WorkFormedSkills = new BattleSkillReq[BATTLE_SKILL_TOTAL_MAX];
        m_WorkReachInfos = new ReachInfo[BATTLE_SKILL_TOTAL_MAX];
        m_ResultReachInfos = new ReachInfo[BATTLE_SKILL_TOTAL_MAX];
    }

    public BattleSkillReq[] skills
    {
        get
        {
            return m_ResultFormedSkills;
        }
    }
    public int skills_count
    {
        get
        {
            return m_ResultFormedSkillsCount;
        }
    }
    public ReachInfo[] reach_infos
    {
        get
        {
            return m_ResultReachInfos;
        }
    }
    public int reach_infos_count
    {
        get
        {
            return m_ResultReachInfosCount;
        }
    }

    public void reset()
    {
        m_Phase = PHASE_WAIT;
        m_ResultFormedSkillsCount = 0;
        m_ResultReachInfosCount = 0;
    }

    /// <summary>
    /// 成立チェックを起動
    /// </summary>
    public void setCheck()
    {
        m_IsCheck = true;
    }

    /// <summary>
    /// 結果が更新されたかどうか（結果が更新されたフレームのみtrue）
    /// </summary>
    /// <returns></returns>
    public bool isUpdateResult()
    {
        return m_IsFixResult;
    }

    /// <summary>
    /// 判定処理を実行（１フレーム分）
    /// </summary>
    /// <param name="battle_card_manager"></param>
    /// <param name="ordered_skill_list"></param>
    /// <param name="boosts"></param>
    /// <returns>チェック中かどうか</returns>
    public bool checkAsync(BattleScene._BattleCardManager battle_card_manager, MasterDataSkillActive[] ordered_skill_list, bool[] boosts, SkillRequestParam.SkillFilterType[] filter_types)
    {
        m_IsFixResult = false;
        if (m_Phase == PHASE_WAIT)
        {
            if (m_IsCheck)
            {
                m_IsCheck = false;

                // ハート回復・復活を判定
                _setupPanelAndField(battle_card_manager, ordered_skill_list, boosts, filter_types);
                _checkAlwaysSkill();
                m_Phase = 0;

                return true;
            }

            return false;
        }

        // ノーマルスキルを判定
        _checkNormalSkill();

        m_Phase++;

        if (m_Phase >= PHASE_MAX)
        {
            _fixResult();
            m_IsFixResult = true;
            m_Phase = PHASE_WAIT;
        }

        return true;
    }

    /// <summary>
    /// 判定処理を実行（判定が確定するまで実行）
    /// </summary>
    /// <param name="battle_card_manager"></param>
    /// <param name="ordered_skill_list"></param>
    /// <param name="boosts"></param>
    public void check(BattleScene._BattleCardManager battle_card_manager, MasterDataSkillActive[] ordered_skill_list, bool[] boosts, SkillRequestParam.SkillFilterType[] filter_types)
    {
        while (true)
        {
            bool is_checking = checkAsync(battle_card_manager, ordered_skill_list, boosts, filter_types);
            if (is_checking == false)
            {
                break;
            }
        }

        m_IsFixResult = true;
    }

    /// <summary>
    /// 作業領域を確保
    /// </summary>
    /// <param name="field_area_count"></param>
    private void _allocateWorkArea(int field_area_count)
    {
        for (int idx = 0; idx < m_WorkFormedSkills.Length; idx++)
        {
            m_ResultFormedSkills[idx] = new BattleSkillReq();
            m_WorkFormedSkills[idx] = new BattleSkillReq();
        }

        m_WorkActiveSkillMembers = new CharaOnce[BattleParam.m_PlayerParty.getPartyMemberMaxCount()];
        m_WorkIsAliveActiveSkillMembers = new bool[BattleParam.m_PlayerParty.getPartyMemberMaxCount()];

        m_WorkHandcardCostPerElement = new bool[(int)MasterDataDefineLabel.ElementType.MAX];

        m_WorkGeneralFieldCostInfo = new int[field_area_count, (int)MasterDataDefineLabel.ElementType.MAX];
        m_WorkMemberFieldCostInfo = new int[BattleParam.m_PlayerParty.getPartyMemberMaxCount()][,];
        for (int member_idx = 0; member_idx < m_WorkMemberFieldCostInfo.Length; member_idx++)
        {
            m_WorkMemberFieldCostInfo[member_idx] = new int[field_area_count, (int)MasterDataDefineLabel.ElementType.MAX];
        }

        m_IsBoosts = new bool[field_area_count];

        m_IsFulls = new bool[field_area_count];

        m_FilterTypes = new SkillRequestParam.SkillFilterType[BattleParam.m_PlayerParty.getPartyMemberMaxCount()];
    }

    /// <summary>
    /// パネル・場の状態を設定
    /// </summary>
    /// <param name="battle_card_manager"></param>
    /// <param name="ordered_skill_list"></param>
    /// <param name="boosts"></param>
    private void _setupPanelAndField(BattleScene._BattleCardManager battle_card_manager, MasterDataSkillActive[] ordered_skill_list, bool[] boosts, SkillRequestParam.SkillFilterType[] filter_types)
    {
        if (m_IsAllocatedWorkArea == false)
        {
            m_IsAllocatedWorkArea = true;
            _allocateWorkArea(battle_card_manager.m_FieldAreas.getFieldAreaCountMax());
        }

        m_WorkFormedSkillsCount = 0;
        m_WorkReachInfosCount = 0;

        for (int idx = 0; idx < m_WorkActiveSkillMembers.Length; idx++)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.EXIST);
            m_WorkActiveSkillMembers[idx] = chara_once;

            bool is_alive = false;
            if (chara_once != null)
            {
                CharaOnce chara_once_active_skill = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.SKILL_ACTIVE);
                if (chara_once_active_skill != null)
                {
                    is_alive = true;
                }
            }
            m_WorkIsAliveActiveSkillMembers[idx] = is_alive;
        }

        for (int idx = 0; idx < m_WorkHandcardCostPerElement.Length; idx++)
        {
            m_WorkHandcardCostPerElement[idx] = false;
        }
        for (int idx = 0; idx < battle_card_manager.m_HandArea.getCardMaxCount(); idx++)
        {
            BattleScene.BattleCard battle_card = battle_card_manager.m_HandArea.getCard(idx);
            if (battle_card != null)
            {
                m_WorkHandcardCostPerElement[(int)battle_card.getElementType()] = true;
            }
        }
        for (int idx = 0; idx < battle_card_manager.m_InHandArea.getCardMaxCount(); idx++)
        {
            BattleScene.BattleCard battle_card = battle_card_manager.m_InHandArea.getCard(idx);
            if (battle_card != null)
            {
                m_WorkHandcardCostPerElement[(int)battle_card.getElementType()] = true;
            }
        }

        // 各フィールドに置かれている属性別のカード枚数を保存（常駐スキルと各パーティメンバーそれぞれへ保存）
        for (int field_idx = 0; field_idx < battle_card_manager.m_FieldAreas.getFieldAreaCountMax(); field_idx++)
        {
            BattleScene.FieldArea field_area = battle_card_manager.m_FieldAreas.getFieldArea(field_idx);
            int[] field_cost_per_element = field_area.getCostPerElenet();

            // 常駐スキルへ保存
            for (int element_idx = (int)MasterDataDefineLabel.ElementType.NONE + 1; element_idx < (int)MasterDataDefineLabel.ElementType.MAX; element_idx++)
            {
                m_WorkGeneralFieldCostInfo[field_idx, element_idx] = field_cost_per_element[element_idx];
            }

            // 全パーティメンバーへ保存
            for (int member_idx = 0; member_idx < m_WorkMemberFieldCostInfo.Length; member_idx++)
            {
                int[,] wrk_member_field_cost_info = m_WorkMemberFieldCostInfo[member_idx];
                for (int element_idx = (int)MasterDataDefineLabel.ElementType.NONE + 1; element_idx < (int)MasterDataDefineLabel.ElementType.MAX; element_idx++)
                {
                    wrk_member_field_cost_info[field_idx, element_idx] = field_cost_per_element[element_idx];
                }
            }
        }

        m_OrderedSkillList = ordered_skill_list;

        boosts.CopyTo(m_IsBoosts, 0);
        filter_types.CopyTo(m_FilterTypes, 0);

        for (int idx = 0; idx < battle_card_manager.m_FieldAreas.getFieldAreaCountMax(); idx++)
        {
            m_IsFulls[idx] = battle_card_manager.m_FieldAreas.getFieldArea(idx).isFull();
        }
    }

    /// <summary>
    /// 常駐スキルの成立を判定
    /// </summary>
    private void _checkAlwaysSkill()
    {
        //常駐スキル
        for (int skill_idx = 0; skill_idx < m_OrderedSkillList.Length; skill_idx++)
        {
            MasterDataSkillActive current_skill_active = m_OrderedSkillList[skill_idx];

            if (current_skill_active.always == MasterDataDefineLabel.BoolType.ENABLE)
            {
                //復活スキル以外を判定
                if (current_skill_active.getResurrectInfo() == null)
                {
                    uint current_skill_id = current_skill_active.fix_id;
                    int[] current_skill_cost = current_skill_active.GetCostPerElement();
                    _checkActiveSkillFormingConditionSub(m_WorkGeneralFieldCostInfo, current_skill_cost, m_WorkHandcardCostPerElement, GlobalDefine.PartyCharaIndex.GENERAL, current_skill_id);
                }
            }
        }
        // 常駐回復スキルと同条件で成立する常駐復活スキルを追加
        int always_recov_skill_count = m_WorkFormedSkillsCount;
        for (int idx = 0; idx < always_recov_skill_count; idx++)
        {
            BattleSkillReq recov_skill_req = m_WorkFormedSkills[idx];
            if (m_IsBoosts[recov_skill_req.m_SkillParamFieldNum] != false)
            {
                for (int skill_idx = 0; skill_idx < m_OrderedSkillList.Length; skill_idx++)
                {
                    MasterDataSkillActive current_skill_active = m_OrderedSkillList[skill_idx];

                    if (current_skill_active.isAlwaysResurrectSkill())
                    {
                        MasterDataSkillActive recov_skill = BattleParam.m_MasterDataCache.useSkillActive(recov_skill_req.m_SkillParamSkillID);
                        if (current_skill_active.cost1 == recov_skill.cost1
                            && current_skill_active.cost2 == recov_skill.cost2
                            && current_skill_active.cost3 == recov_skill.cost3
                            && current_skill_active.cost4 == recov_skill.cost4
                            && current_skill_active.cost5 == recov_skill.cost5
                        )
                        {
                            if (m_WorkFormedSkillsCount < m_WorkFormedSkills.Length)
                            {
                                BattleSkillReq resurr_skill_req = m_WorkFormedSkills[m_WorkFormedSkillsCount];
                                resurr_skill_req.m_SkillReqState = BattleSkillReq.State.REQUESTED;
                                resurr_skill_req.m_SkillParamCharaNum = recov_skill_req.m_SkillParamCharaNum;
                                resurr_skill_req.m_SkillParamFieldNum = recov_skill_req.m_SkillParamFieldNum;
                                resurr_skill_req.m_SkillParamSkillID = current_skill_active.fix_id;

                                m_WorkFormedSkillsCount++;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// ノーマルスキルの成立を判定
    /// </summary>
    /// <param name="index"></param>
    private void _checkNormalSkill()
    {
        //通常のアクティブスキル
        int start_index = (m_OrderedSkillList.Length * m_Phase) / PHASE_MAX;
        int end_index = (m_OrderedSkillList.Length * (m_Phase + 1)) / PHASE_MAX;

        for (int skill_idx = start_index; skill_idx < end_index; skill_idx++)
        {
            MasterDataSkillActive current_skill_active = m_OrderedSkillList[skill_idx];
            uint current_skill_id = current_skill_active.fix_id;
            int[] current_skill_cost = current_skill_active.GetCostPerElement();

            if (current_skill_active.always != MasterDataDefineLabel.BoolType.ENABLE)
            {
                for (int member_idx = 0; member_idx < m_WorkActiveSkillMembers.Length; member_idx++)
                {
                    CharaOnce chara_once = m_WorkActiveSkillMembers[member_idx];
                    if (chara_once != null
                        && m_WorkIsAliveActiveSkillMembers[member_idx]
                    )
                    {
                        if (chara_once.m_CharaMasterDataParam.skill_active0 == current_skill_id
                            || chara_once.m_CharaMasterDataParam.skill_active1 == current_skill_id
                        )
                        {
                            if (SkillRequestParam.filterSkill(current_skill_active, m_FilterTypes[member_idx]))
                            {
                                int[,] wrk_member_field_cost_info = m_WorkMemberFieldCostInfo[member_idx];
                                _checkActiveSkillFormingConditionSub(wrk_member_field_cost_info, current_skill_cost, m_WorkHandcardCostPerElement, chara_once.m_PartyCharaIndex, current_skill_id);
                            }
                        }
                    }
                }
            }
        }
    }

    private void _checkActiveSkillFormingConditionSub(int[,] field_cost_per_element, int[] skill_cost_per_element, bool[] handcard_cost_per_element, GlobalDefine.PartyCharaIndex chara_index, uint skill_id)
    {
        for (int field_idx = 0; field_idx < m_IsFulls.Length; field_idx++)
        {
            while (true)    //ひとつのフィールドで同じスキルが複数回成立する可能性があるのでループ
            {
                // スキル発動条件成立チェック
                bool is_success = true;
                for (int element_idx = (int)MasterDataDefineLabel.ElementType.NONE + 1; element_idx < (int)MasterDataDefineLabel.ElementType.MAX; element_idx++)
                {
                    if (field_cost_per_element[field_idx, element_idx] < skill_cost_per_element[element_idx])
                    {
                        is_success = false;
                        break;
                    }
                }

                if (is_success)
                {
                    // 発動条件成立したスキルを登録
                    if (m_WorkFormedSkillsCount < m_WorkFormedSkills.Length)
                    {
                        BattleSkillReq skill_req = m_WorkFormedSkills[m_WorkFormedSkillsCount];
                        skill_req.m_SkillReqState = BattleSkillReq.State.REQUESTED;
                        skill_req.m_SkillParamCharaNum = chara_index;
                        skill_req.m_SkillParamFieldNum = field_idx;
                        skill_req.m_SkillParamSkillID = skill_id;

                        m_WorkFormedSkillsCount++;
                    }

                    // 場のカード消費
                    for (int element_idx = (int)MasterDataDefineLabel.ElementType.NONE + 1; element_idx < (int)MasterDataDefineLabel.ElementType.MAX; element_idx++)
                    {
                        field_cost_per_element[field_idx, element_idx] -= skill_cost_per_element[element_idx];
                    }
                }
                else
                {
                    // リーチチェック
                    if (m_IsFulls[field_idx] == false)
                    {
                        bool is_reach_success = true;
                        bool is_use_handcard = false;
                        for (int element_idx = (int)MasterDataDefineLabel.ElementType.NONE + 1; element_idx < (int)MasterDataDefineLabel.ElementType.MAX; element_idx++)
                        {
                            if (handcard_cost_per_element[element_idx])
                            {
                                if (field_cost_per_element[field_idx, element_idx] + 1 < skill_cost_per_element[element_idx])
                                {
                                    is_reach_success = false;
                                    break;
                                }

                                if (field_cost_per_element[field_idx, element_idx] + 1 == skill_cost_per_element[element_idx])
                                {
                                    if (is_use_handcard)
                                    {
                                        // 手札を２枚以上使わないと成立しない場合はリーチではない.
                                        is_reach_success = false;
                                        break;
                                    }
                                    is_use_handcard = true;
                                }
                            }
                            else
                            {
                                if (field_cost_per_element[field_idx, element_idx] < skill_cost_per_element[element_idx])
                                {
                                    is_reach_success = false;
                                    break;
                                }
                            }
                        }

                        if (is_reach_success)
                        {
                            MasterDataDefineLabel.ElementType reach_element = MasterDataDefineLabel.ElementType.NONE;
                            for (int element_idx = (int)MasterDataDefineLabel.ElementType.NONE + 1; element_idx < (int)MasterDataDefineLabel.ElementType.MAX; element_idx++)
                            {
                                if (field_cost_per_element[field_idx, element_idx] < skill_cost_per_element[element_idx])
                                {
                                    reach_element = (MasterDataDefineLabel.ElementType)element_idx;
                                    break;
                                }
                            }

                            //################								addSkillReachInfo(field_idx, reach_element);
                            m_WorkReachInfos[m_WorkReachInfosCount].m_FieldIndex = field_idx;
                            m_WorkReachInfos[m_WorkReachInfosCount].m_Element = reach_element;
                            m_WorkReachInfosCount++;
                        }
                    }

                    break;
                }
            }
        }
    }

    /// <summary>
    /// 判定結果を確定
    /// </summary>
    private void _fixResult()
    {
        BattleSkillReq[] temp_skills = m_ResultFormedSkills;
        m_ResultFormedSkills = m_WorkFormedSkills;
        m_WorkFormedSkills = temp_skills;

        m_ResultFormedSkillsCount = m_WorkFormedSkillsCount;

        ReachInfo[] temp_reachs = m_ResultReachInfos;
        m_ResultReachInfos = m_WorkReachInfos;
        m_WorkReachInfos = temp_reachs;

        m_ResultReachInfosCount = m_WorkReachInfosCount;
    }
}
