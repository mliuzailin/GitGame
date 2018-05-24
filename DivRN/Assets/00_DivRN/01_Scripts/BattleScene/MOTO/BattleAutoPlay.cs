using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAutoPlay
{
    /// <summary>
    /// オートプレイの思考ルーチン
    /// </summary>
    public abstract class AI
    {
        public class CostInfo
        {
            public int m_CostCountMax = 5;
            public int m_CostCountUsing = 0;
            public int[] m_ElementCounts = new int[(int)MasterDataDefineLabel.ElementType.MAX];

            public CostInfo(int cost_count_max)
            {
                m_CostCountMax = cost_count_max;
                resetCosts();
            }

            public CostInfo(
                MasterDataDefineLabel.ElementType cost0,
                MasterDataDefineLabel.ElementType cost1 = MasterDataDefineLabel.ElementType.NONE,
                MasterDataDefineLabel.ElementType cost2 = MasterDataDefineLabel.ElementType.NONE,
                MasterDataDefineLabel.ElementType cost3 = MasterDataDefineLabel.ElementType.NONE,
                MasterDataDefineLabel.ElementType cost4 = MasterDataDefineLabel.ElementType.NONE
            )
            {
                m_CostCountMax = 5;
                resetCosts();

                addCost(cost0);
                addCost(cost1);
                addCost(cost2);
                addCost(cost3);
                addCost(cost4);
            }

            public CostInfo(CostInfo cost_info)
            {
                m_CostCountMax = cost_info.m_CostCountMax;
                m_CostCountUsing = cost_info.m_CostCountUsing;

                for (MasterDataDefineLabel.ElementType element_idx = MasterDataDefineLabel.ElementType.NONE; element_idx < MasterDataDefineLabel.ElementType.MAX; element_idx++)
                {
                    m_ElementCounts[(int)element_idx] = cost_info.m_ElementCounts[(int)element_idx];
                }
            }

            public void resetCosts()
            {
                m_CostCountUsing = 0;
                for (MasterDataDefineLabel.ElementType element_idx = MasterDataDefineLabel.ElementType.NONE; element_idx < MasterDataDefineLabel.ElementType.MAX; element_idx++)
                {
                    m_ElementCounts[(int)element_idx] = 0;
                }
            }

            public void addCost(MasterDataDefineLabel.ElementType element_type, int value = 1)
            {
                if (element_type != MasterDataDefineLabel.ElementType.NONE)
                {
                    if (m_CostCountUsing + value > m_CostCountMax)
                    {
                        value = m_CostCountMax - m_CostCountUsing;
                    }

                    if (m_ElementCounts[(int)element_type] + value < 0)
                    {
                        value = -m_ElementCounts[(int)element_type];
                    }

                    m_ElementCounts[(int)element_type] += value;
                    {
                        m_CostCountUsing += value;
                    }
                }
            }

            public int getCostCount(MasterDataDefineLabel.ElementType element_type)
            {
                int ret_val = m_ElementCounts[(int)element_type];
                return ret_val;
            }
        }

        /// <summary>
        /// 思考ルーチンで使用するパネルの情報
        /// </summary>
        public class PanelInfo
        {
            public MasterDataDefineLabel.ElementType[/*hand_idx*/] m_HandElements;
            public MasterDataDefineLabel.ElementType[/*hand_idx*/] m_NextElements;
            public CostInfo[/*field_idx*/] m_FieldElements;
            public int[/*field_idx*/] m_FieldCosts;
            public bool[/*field_idx*/] m_IsBoosts;
            public bool m_IsBossBattle;
        }

        /// <summary>
        /// ５秒間で何枚のパネルを出すか.
        /// </summary>
        /// <returns></returns>
        public virtual int getPanelPutCount()
        {
            return 1;
        }

        /// <summary>
        /// オートプレイ開始時に呼ばれる関数
        /// </summary>
        public virtual void startAutoPlay()
        {
        }

        /// <summary>
        /// 思考を初期化
        /// </summary>
        public virtual void initThink(PanelInfo panel_info)
        {
        }

        /// <summary>
        /// 思考を実行
        /// オートプレイは２枚以上のパネルは手に持たない（アルゴリズムを簡単にするため）
        /// </summary>
        /// <param name="dest_hand_index">どこのパネルを持つか</param>
        /// <param name="dest_field_index">どこにパネルを置くか</param>
        /// <returns></returns>
        public virtual void execThink(ref int dest_hand_index, ref int dest_field_index, PanelInfo panel_info)
        {
        }

        /// <summary>
        /// 発動させたいリミットブレイクスキルがある場合はこの関数で発動者を返すとオートプレイがリミブレスキルを発動します。
        /// </summary>
        /// <returns></returns>
        public virtual GlobalDefine.PartyCharaIndex getLimitBreakSkillCaster()
        {
            return GlobalDefine.PartyCharaIndex.ERROR;
        }

        /// <summary>
        /// カウントダウンをスキップしたい場合trueを返すとスキップされます。
        /// </summary>
        /// <returns></returns>
        public virtual bool isSkipCountDown()
        {
            return false;
        }

        /// <summary>
        /// オートプレイを停止したいときにtureを返してください。
        /// </summary>
        /// <returns></returns>
        public virtual bool isStopAutoPlay()
        {
            return false;
        }
    }

    /// <summary>
    /// デフォルトのＡＩ（ランダムにパネルを出す）
    /// </summary>
    private class AI_Default : BattleAutoPlay.AI
    {
        public override int getPanelPutCount()
        {
            int ret_val = UnityEngine.Random.Range(10, 20 + 1);
            return ret_val;
        }

        public override void initThink(PanelInfo panel_info)
        {
        }

        public override void execThink(ref int dest_hand_index, ref int dest_field_index, PanelInfo panel_info)
        {
            // 完全ランダム
            dest_hand_index = UnityEngine.Random.Range(0, 5);
            dest_field_index = UnityEngine.Random.Range(0, 5);
        }
    }

    private const float CARD_MOVE_TIME_MAX = 0.2f;  // 手に持ってから場に置くまでの時間の最大値
    private const float COUNT_DOWN_TIME = 5.0f;  // カウントダウン時間
    private const float CARD_PUT_TIME = 3.0f / 60.0f;  // カードを場に置く処理にかかる時間

    private BattleScene._BattleCardManager m_BattleCardManager = null;

    private bool m_IsPlaying = false;
    private bool m_IsDebugStopMode = false;
    private AI m_AI = null;

    private enum AutoPlayPhase
    {
        NOT_INPUT_PHASE,	// ユーザー入力受付中ではない
        START_WAIT,	// オートプレイ開始待ち
        CARD_GET,	// カード掴み
        CARD_MOVE,	// カード移動中
        CARD_PUT,  // カード置く
    }
    private AutoPlayPhase m_AutoPlayPhase = AutoPlayPhase.NOT_INPUT_PHASE;
    private float m_Timer = 0.0f;
    private float m_Duration = 0.0f;
    private Vector2 m_StartPosition;
    private Vector2 m_GoalPosition;
    private bool m_IsFullField = false; // 場がすべて埋まっていたらそれ以上パネルを置かない
    private float m_PanelPutTimer = 0.0f;   // パネルを操作開始してからの経過時間（カウントダウン開始前のパネルを手に持った瞬間からの時間）
    private int m_PanelPutCountGoal = 0;    // ５秒間でパネルを何枚出すか
    private int m_PanelPutCountCurrent = 0; // カウントダウンが始まってから何枚パネルを出したか
    private float m_PanelPutInterval = 0.0f;

    private AI.PanelInfo m_PanelInfo;   // 現在のパネル情報

    public void init(BattleScene._BattleCardManager battle_card_manager)
    {
        m_BattleCardManager = battle_card_manager;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="is_active"></param>
    /// <param name="ai"></param>
    /// <param name="is_debug_stop_mode">ユーザーが画面タッチしたらオートモードを解除するかどうか</param>
    public void startAutoPlay(AI ai = null, bool is_debug_stop_mode = false)
    {
        m_IsPlaying = true;
        m_IsDebugStopMode = is_debug_stop_mode;
        if (m_IsDebugStopMode)
        {
            BattleParam.setAutoPlayState(BattleParam.AutoPlayState.ON);
        }

        if (ai != null)
        {
            m_AI = ai;
        }
        else
        {
            m_AI = new AI_Default();
        }
        m_AI.startAutoPlay();

        m_Timer = 0.0f;
        m_AutoPlayPhase = AutoPlayPhase.START_WAIT;
    }

    public void stopAutoPlay()
    {
        m_IsPlaying = false;
        m_AutoPlayPhase = AutoPlayPhase.NOT_INPUT_PHASE;
    }

    public bool isPlaying()
    {
        return m_IsPlaying;
    }

    public void update(float delta_time)
    {
        if (m_IsPlaying)
        {
            if (m_IsDebugStopMode)
            {
                bool mouse_button = Input.GetMouseButton(0);
                if (mouse_button)
                {
                    BattleParam.setAutoPlayState(BattleParam.AutoPlayState.CANCEL);
                    m_IsPlaying = false;
                    m_AutoPlayPhase = AutoPlayPhase.NOT_INPUT_PHASE;
                    return;
                }
            }

            BattleParam.BattlePhase battle_phase = BattleParam.getBattlePhase();
            if (battle_phase == BattleParam.BattlePhase.INPUT
                || battle_phase == BattleParam.BattlePhase.INPUT_HANDLING
            )
            {
                if (m_IsFullField)
                {
                    return;
                }

                Vector2 hand_pos = new Vector2(2.0f, 5.0f); //画面外の見えない位置に初期化
                bool is_touching = false;

                switch (m_AutoPlayPhase)
                {
                    case AutoPlayPhase.NOT_INPUT_PHASE:
                        {
                            // オートプレイの停止判定
                            if (m_AI.isStopAutoPlay())
                            {
                                BattleParam.setAutoPlayState(BattleParam.AutoPlayState.CANCEL);
                                m_IsPlaying = false;
                                m_AutoPlayPhase = AutoPlayPhase.NOT_INPUT_PHASE;
                                m_IsFullField = false;
                                return;
                            }

                            // プレイヤーフェイズに戻ってきたときにここに来る.
                            m_Timer = 1.0f;
                            m_AutoPlayPhase = AutoPlayPhase.START_WAIT;
                        }
                        break;

                    case AutoPlayPhase.START_WAIT:
                        {
                            m_Timer -= delta_time;
                            if (m_Timer <= 0.0f)
                            {
                                m_PanelInfo = new AI.PanelInfo();
                                m_PanelInfo.m_HandElements = new MasterDataDefineLabel.ElementType[m_BattleCardManager.m_HandArea.getCardMaxCount()];
                                m_PanelInfo.m_NextElements = new MasterDataDefineLabel.ElementType[m_BattleCardManager.m_NextArea.getCardMaxCount()];
                                m_PanelInfo.m_FieldElements = new AI.CostInfo[m_BattleCardManager.m_FieldAreas.getFieldAreaCountMax()];
                                for (int field_idx = 0; field_idx < m_PanelInfo.m_FieldElements.Length; field_idx++)
                                {
                                    m_PanelInfo.m_FieldElements[field_idx] = new AI.CostInfo(m_BattleCardManager.m_FieldAreas.getFieldArea(field_idx).getCardMaxCount());
                                }
                                m_PanelInfo.m_FieldCosts = new int[m_PanelInfo.m_FieldElements.Length];
                                m_PanelInfo.m_IsBoosts = new bool[m_PanelInfo.m_FieldElements.Length];

                                // 思考ルーチン初期化
                                m_AI.initThink(_updatePanelInfo());

                                // リミブレスキルをチェック
                                GlobalDefine.PartyCharaIndex limit_break_skill_caster = m_AI.getLimitBreakSkillCaster();
                                if (BattleParam.IsEnableLBS(limit_break_skill_caster) == false)
                                {
                                    limit_break_skill_caster = GlobalDefine.PartyCharaIndex.ERROR;
                                }

                                if (limit_break_skill_caster >= GlobalDefine.PartyCharaIndex.LEADER
                                    && limit_break_skill_caster <= GlobalDefine.PartyCharaIndex.FRIEND
                                )
                                {
                                    // リミブレスキル発動
                                    BattleParam.RequestLBS(limit_break_skill_caster);
                                    m_AutoPlayPhase = AutoPlayPhase.NOT_INPUT_PHASE;
                                    m_IsFullField = false;
                                }
                                else
                                {
                                    // パネル操作開始
                                    m_PanelPutTimer = 0.0f;
                                    m_PanelPutCountGoal = m_AI.getPanelPutCount();
                                    m_PanelPutCountCurrent = 0;

                                    float count_down_time = BattleSceneManager.Instance.PRIVATE_FIELD.calcCountDownTime();
                                    if (count_down_time < COUNT_DOWN_TIME)
                                    {
                                        count_down_time = COUNT_DOWN_TIME;
                                    }

                                    m_PanelPutInterval = count_down_time / m_PanelPutCountGoal * (1.0f + 1.0f / m_PanelPutCountGoal);
                                    m_AutoPlayPhase = AutoPlayPhase.CARD_GET;
                                }
                            }
                        }
                        break;

                    case AutoPlayPhase.CARD_GET:
                        {
                            m_PanelPutTimer += delta_time;

                            float card_get_time = m_PanelPutCountCurrent * m_PanelPutInterval;    // カードを掴む時刻

                            if (m_PanelPutTimer >= card_get_time)
                            {
                                if (m_BattleCardManager != null)
                                {
                                    if (m_BattleCardManager.m_FieldAreas.getFieldArea(0).isFull()
                                        && m_BattleCardManager.m_FieldAreas.getFieldArea(1).isFull()
                                        && m_BattleCardManager.m_FieldAreas.getFieldArea(2).isFull()
                                        && m_BattleCardManager.m_FieldAreas.getFieldArea(3).isFull()
                                        && m_BattleCardManager.m_FieldAreas.getFieldArea(4).isFull()
                                    )
                                    {
                                        BattleSceneManager.Instance.PRIVATE_FIELD.skipCountDown();
                                        m_IsFullField = true;
                                        m_AutoPlayPhase = AutoPlayPhase.NOT_INPUT_PHASE;
                                        return;
                                    }
                                }

                                // カウントダウンスキップ判定
                                if (m_AI.isSkipCountDown())
                                {
                                    BattleSceneManager.Instance.PRIVATE_FIELD.skipCountDown();
                                    m_AutoPlayPhase = AutoPlayPhase.NOT_INPUT_PHASE;
                                    m_IsFullField = false;
                                    return;
                                }

                                // 思考ルーチン呼び出し
                                int hand_index = 0;
                                int field_index = 0;
                                m_AI.execThink(ref hand_index, ref field_index, _updatePanelInfo());

                                m_StartPosition = new Vector2(hand_index, 1.0f - 0.3f);
                                m_GoalPosition = new Vector2(field_index, 0.0f - 0.5f);

                                hand_pos = m_StartPosition;
                                is_touching = true;

                                float card_put_time = (m_PanelPutCountCurrent + 1) * m_PanelPutInterval - CARD_PUT_TIME;    // カードを置く時刻
                                m_Duration = card_put_time - m_PanelPutTimer;
                                m_Duration = Mathf.Max(m_Duration, 0.0f);
                                m_Duration = Mathf.Min(m_Duration, CARD_MOVE_TIME_MAX);

                                m_Timer = 0.0f;
                                m_AutoPlayPhase = AutoPlayPhase.CARD_MOVE;
                            }
                        }
                        break;

                    case AutoPlayPhase.CARD_MOVE:
                        {
                            m_PanelPutTimer += delta_time;
                            m_Timer += delta_time;
                            if (m_Timer < m_Duration)
                            {
                                float t = m_Timer / m_Duration;

                                Vector2 vec = m_GoalPosition - m_StartPosition;
                                vec.x *= t * t; // 横方向は動き始めをゆっくりにする（２枚以上を掴まないようにするため）
                                vec.y *= t;
                                hand_pos = m_StartPosition + vec;
                                is_touching = true;
                            }
                            else
                            {
                                hand_pos = m_GoalPosition;
                                is_touching = true;

                                m_AutoPlayPhase = AutoPlayPhase.CARD_PUT;
                            }
                        }
                        break;

                    case AutoPlayPhase.CARD_PUT:    // タッチしていないフレームが最低１フレームはあるようにするための処理
                        {
                            m_PanelPutTimer += delta_time;

                            m_PanelPutCountCurrent++;

                            hand_pos = m_GoalPosition;
                            is_touching = false;

                            m_AutoPlayPhase = AutoPlayPhase.CARD_GET;
                        }
                        break;
                }

                BattleSceneManager.Instance.setOverrideTouchMode(BattleSceneManager.Instance.getCardFieldScreenPos(hand_pos.x, hand_pos.y), is_touching);
            }
            else
            {
                m_AutoPlayPhase = AutoPlayPhase.NOT_INPUT_PHASE;
                m_IsFullField = false;
            }
        }
        else
        {
            m_AutoPlayPhase = AutoPlayPhase.NOT_INPUT_PHASE;
            m_IsFullField = false;
        }
    }

    private AI.PanelInfo _updatePanelInfo()
    {
        // 手札の情報を取得
        for (int hand_idx = 0; hand_idx < m_PanelInfo.m_HandElements.Length; hand_idx++)
        {
            MasterDataDefineLabel.ElementType element_type = m_BattleCardManager.m_HandArea.getCardElement(hand_idx);
            m_PanelInfo.m_HandElements[hand_idx] = element_type;
        }

        // 次札の情報を取得
        for (int next_idx = 0; next_idx < m_PanelInfo.m_NextElements.Length; next_idx++)
        {
            MasterDataDefineLabel.ElementType element_type = m_BattleCardManager.m_NextArea.getCardElement(next_idx);
            m_PanelInfo.m_NextElements[next_idx] = element_type;
        }

        // 場の情報を取得
        for (int field_idx = 0; field_idx < m_PanelInfo.m_FieldElements.Length; field_idx++)
        {
            AI.CostInfo field_cost_info = m_PanelInfo.m_FieldElements[field_idx];
            field_cost_info.resetCosts();

            int cost_count = m_BattleCardManager.m_FieldAreas.getFieldArea(field_idx).getCardCount();
            for (int cost_idx = 0; cost_idx < cost_count; cost_idx++)
            {
                MasterDataDefineLabel.ElementType element_type = m_BattleCardManager.m_FieldAreas.getFieldArea(field_idx).getCardElement(cost_idx);
                field_cost_info.addCost(element_type);
            }
            m_PanelInfo.m_FieldCosts[field_idx] = cost_count;
            m_PanelInfo.m_IsBoosts[field_idx] = m_BattleCardManager.m_FieldAreas.getFieldArea(field_idx).m_IsBoost;
        }

        m_PanelInfo.m_IsBossBattle = BattleParam.m_BattleRequest.isBoss;

        return m_PanelInfo;
    }
}
