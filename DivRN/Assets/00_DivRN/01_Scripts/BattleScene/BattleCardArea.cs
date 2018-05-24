using UnityEngine;
using System.Collections;
using BattleScene;

namespace BattleScene
{
    /// <summary>
    /// カード
    /// </summary>
    public class BattleCard
    {
        public enum Phase
        {
            UNUSED, // 未使用
            NEXT_AREA,  // 次手札領域上に置かれている
            HAND_AREA,  // 手札領域上に置かれている
            IN_HAND,    // 手に持たれている
            FIELD_AREA, // 場札領域上に置かれている
        }

        /// <summary>
        /// 状態変化の理由
        /// </summary>
        public enum ChangeCause
        {
            NONE,
            NEW_CARD,   // 新規に出現
            SKILL,      // スキルで変化した.
        }

        private Phase m_Phase = Phase.UNUSED;    // カードの状態.
        private bool m_IsChangePhase;
        private MasterDataDefineLabel.ElementType m_ElementType = MasterDataDefineLabel.ElementType.NONE;    // カードの属性
        private ChangeCause m_ElementChangeCause = ChangeCause.NONE;
        public bool m_IsOn = true;          // スキル発動に必要なカードかどうか.
        public int m_HandCardIndex = -1;    // 手札エリアのどこに置かれていたかを記憶.
        public BattleCardViewContorl m_ViewControl = null;
        private _BattleCardManager m_BattleCardManager = null;
        public float m_HoldMoveTime = 0.0f; // 時間切れで手に持っていたカードをしばらくその場に残すための時間

        public BattleCard(_BattleCardManager battle_card_manager)
        {
            m_BattleCardManager = battle_card_manager;
        }

        public void setPhase(Phase phase)
        {
            m_Phase = phase;
            m_IsChangePhase = true;
        }

        public Phase getPhase()
        {
            return m_Phase;
        }

        public bool isChangePhase()
        {
            return m_IsChangePhase;
        }

        public void resetChangePhase()
        {
            m_IsChangePhase = false;
            m_HoldMoveTime = 0.0f;
        }

        public void setElementType(MasterDataDefineLabel.ElementType element_type, ChangeCause element_change_cause)
        {
            m_ElementType = element_type;
            m_ElementChangeCause = element_change_cause;
            m_IsOn = m_BattleCardManager.isSkillCard(element_type);
        }

        public MasterDataDefineLabel.ElementType getElementType()
        {
            return m_ElementType;
        }

        public ChangeCause getChangeCause()
        {
            return m_ElementChangeCause;
        }

        public void resetChageCause()
        {
            m_ElementChangeCause = ChangeCause.NONE;
        }
    }

    /// <summary>
    /// 手札エリア
    /// </summary>
    public class HandArea
    {
        public const int MAX_CARD_COUNT = 5;
        private BattleCard[] m_BattleCards = null;

        public HandArea(int max_card_count = MAX_CARD_COUNT)
        {
            m_BattleCards = new BattleCard[max_card_count];
        }

        public void setCard(int index, BattleCard battle_card)
        {
            if (m_BattleCards[index] != null)
            {
                int free_index = -1;
                int dist_min = m_BattleCards.Length;
                for (int idx = 0; idx < m_BattleCards.Length; idx++)
                {
                    if (m_BattleCards[idx] == null)
                    {
                        int dist = Mathf.Abs(idx - index);
                        if (dist < dist_min)
                        {
                            dist_min = dist;
                            free_index = idx;
                        }
                    }
                }

                if (free_index >= 0)
                {
                    int step = (index > free_index) ? 1 : -1;
                    for (int idx = free_index; idx != index; idx += step)
                    {
                        m_BattleCards[idx] = m_BattleCards[idx + step];
                        m_BattleCards[idx].m_HandCardIndex = idx;
                        m_BattleCards[idx].setPhase(BattleCard.Phase.HAND_AREA);
                    }
                    m_BattleCards[index] = null;
                }
            }

            battle_card.setPhase(BattleCard.Phase.HAND_AREA);
            battle_card.m_HandCardIndex = index;
            m_BattleCards[index] = battle_card;
        }

        public void removeCard(BattleCard battle_card, bool is_unuse_card)
        {
            for (int idx = 0; idx < m_BattleCards.Length; idx++)
            {
                if (m_BattleCards[idx] == battle_card)
                {
                    m_BattleCards[idx] = null;
                    if (is_unuse_card)
                    {
                        battle_card.setPhase(BattleCard.Phase.UNUSED);
                        battle_card.m_ViewControl.gameObject.SetActive(false);
                    }
                    break;
                }
            }
        }

        public int getCardMaxCount()
        {
            return m_BattleCards.Length;
        }

        public BattleCard getCard(int index)
        {
            return m_BattleCards[index];
        }

        public MasterDataDefineLabel.ElementType getCardElement(int index)
        {
            if (m_BattleCards[index] != null)
            {
                return m_BattleCards[index].getElementType();
            }

            return MasterDataDefineLabel.ElementType.NONE;
        }

        public void reset()
        {
            for (int idx = 0; idx < m_BattleCards.Length; idx++)
            {
                if (m_BattleCards[idx] != null)
                {
                    m_BattleCards[idx].setPhase(BattleCard.Phase.UNUSED);
                    m_BattleCards[idx].m_ViewControl.gameObject.SetActive(false);
                    m_BattleCards[idx] = null;
                }
            }
        }

        /// <summary>
        /// 手札内に指定属性のカードが何枚あるかを計算
        /// </summary>
        /// <param name="element_type"></param>
        /// <returns></returns>
        public int countElement(MasterDataDefineLabel.ElementType element_type)
        {
            int ret_val = 0;
            for (int idx = 0; idx < m_BattleCards.Length; idx++)
            {
                if (m_BattleCards[idx] != null)
                {
                    if (m_BattleCards[idx].getElementType() == element_type)
                    {
                        ret_val++;
                    }
                }
            }

            return ret_val;
        }
    }

    /// <summary>
    /// 場札エリア（単体）
    /// </summary>
    public class FieldArea
    {
        public const int MAX_CARD_COUNT = 5;
        private BattleCard[] m_BattleCards = null;
        public bool m_IsBoost = false;

        private int[] m_CostPerElement = new int[(int)MasterDataDefineLabel.ElementType.MAX];

        private int m_CardCount = 0;

        public FieldArea(int max_card_count = FieldArea.MAX_CARD_COUNT)
        {
            m_BattleCards = new BattleCard[max_card_count];
            for (int idx = 0; idx < m_BattleCards.Length; idx++)
            {
                m_BattleCards[idx] = null;
            }
            m_CardCount = 0;
        }

        public void reset()
        {
            m_IsBoost = false;
            for (int idx = 0; idx < m_BattleCards.Length; idx++)
            {
                if (m_BattleCards[idx] != null)
                {
                    m_BattleCards[idx].setPhase(BattleCard.Phase.UNUSED);
                    m_BattleCards[idx].m_ViewControl.gameObject.SetActive(false);
                    m_BattleCards[idx] = null;
                }
            }

            for (int idx = 0; idx < m_CostPerElement.Length; idx++)
            {
                m_CostPerElement[idx] = 0;
            }

            m_CardCount = 0;
        }

        public bool addCard(BattleCard battle_card)
        {
            if (isFull() == false)
            {
                battle_card.setPhase(BattleCard.Phase.FIELD_AREA);
                m_BattleCards[m_CardCount] = battle_card;
                m_CardCount++;
                return true;
            }

            return false;
        }

        public bool removeCard(BattleCard battle_card, bool is_unuse_card)
        {
            for (int idx = 0; idx < m_BattleCards.Length; idx++)
            {
                if (m_BattleCards[idx] == battle_card)
                {
                    m_CardCount--;
                    for (; idx < m_CardCount; idx++)
                    {
                        m_BattleCards[idx] = m_BattleCards[idx + 1];
                        m_BattleCards[idx].setPhase(BattleCard.Phase.FIELD_AREA);   // 描画位置を更新
                    }
                    m_BattleCards[idx] = null;

                    if (is_unuse_card)
                    {
                        battle_card.setPhase(BattleCard.Phase.UNUSED);
                        battle_card.m_ViewControl.gameObject.SetActive(false);
                    }
                    return true;
                }
            }

            return false;
        }

        public bool isFull()
        {
            return (m_CardCount >= m_BattleCards.Length);
        }

        public int getCardMaxCount()
        {
            return m_BattleCards.Length;
        }

        public int getCardCount()
        {
            return m_CardCount;
        }

        public BattleCard getCard(int index)
        {
            return m_BattleCards[index];
        }

        public MasterDataDefineLabel.ElementType getCardElement(int index)
        {
            if (m_BattleCards[index] != null)
            {
                return m_BattleCards[index].getElementType();
            }

            return MasterDataDefineLabel.ElementType.NONE;
        }

        // 属性別のカード枚数を取得
        public int[] getCostPerElenet()
        {
            for (int idx = 0; idx < m_CostPerElement.Length; idx++)
            {
                m_CostPerElement[idx] = 0;
            }
            for (int idx = 0; idx < m_CardCount; idx++)
            {
                BattleCard battle_card = m_BattleCards[idx];
                m_CostPerElement[(int)battle_card.getElementType()]++;
            }
            return m_CostPerElement;
        }

    }

    /// <summary>
    /// 場札エリア
    /// </summary>
    public class FieldAreas
    {
        public const int MAX_FIELD_AREA_COUNT = 5;
        private FieldArea[] m_FieldAreas = null;
        public bool m_IsResurrectMode = false;

        public FieldAreas(int max_field_area_count = MAX_FIELD_AREA_COUNT, int max_card_count = FieldArea.MAX_CARD_COUNT)
        {
            m_FieldAreas = new FieldArea[max_field_area_count];
            for (int idx = 0; idx < m_FieldAreas.Length; idx++)
            {
                m_FieldAreas[idx] = new FieldArea(max_card_count);
            }
        }

        public FieldArea getFieldArea(int index)
        {
            return m_FieldAreas[index];
        }

        public int getFieldAreaCountMax()
        {
            return m_FieldAreas.Length;
        }

        public void reset()
        {
            for (int idx = 0; idx < m_FieldAreas.Length; idx++)
            {
                m_FieldAreas[idx].reset();
            }
        }

        public bool addCard(int index, BattleCard battle_card)
        {
            return m_FieldAreas[index].addCard(battle_card);
        }

        public void removeCard(BattleCard battle_card, bool is_unuse_card)
        {
            for (int idx = 0; idx < m_FieldAreas.Length; idx++)
            {
                bool is_delete = m_FieldAreas[idx].removeCard(battle_card, is_unuse_card);
                if (is_delete)
                {
                    break;
                }
            }
        }

        public void setResurrectMode(bool is_resurrect_mode)
        {
            m_IsResurrectMode = is_resurrect_mode;
        }
    }

    /// <summary>
    /// 手に持っているカード
    /// </summary>
    public class InHandArea
    {
        private BattleCard[] m_BattleCards = null;
        private int m_CardCount = 0;

        public InHandArea(int max_card_count = HandArea.MAX_CARD_COUNT)
        {
            m_BattleCards = new BattleCard[max_card_count];
            m_CardCount = 0;
        }

        public void reset()
        {
            for (int idx = 0; idx < m_BattleCards.Length; idx++)
            {
                if (m_BattleCards[idx] != null)
                {
                    m_BattleCards[idx].setPhase(BattleCard.Phase.UNUSED);
                    m_BattleCards[idx].m_ViewControl.gameObject.SetActive(false);
                    m_BattleCards[idx] = null;
                }
            }
            m_CardCount = 0;
        }

        public void addCard(BattleCard battle_card)
        {
            if (m_CardCount < m_BattleCards.Length)
            {
                battle_card.setPhase(BattleCard.Phase.IN_HAND);
                m_BattleCards[m_CardCount] = battle_card;
                m_CardCount++;
            }
        }

        public void removeCard(BattleCard battle_card, bool is_unuse_card)
        {
            for (int idx = 0; idx < m_BattleCards.Length; idx++)
            {
                if (m_BattleCards[idx] == battle_card)
                {
                    m_CardCount--;
                    for (; idx < m_CardCount; idx++)
                    {
                        m_BattleCards[idx] = m_BattleCards[idx + 1];
                    }
                    m_BattleCards[idx] = null;

                    if (is_unuse_card)
                    {
                        battle_card.setPhase(BattleCard.Phase.UNUSED);
                        battle_card.m_ViewControl.gameObject.SetActive(false);
                    }
                    break;
                }
            }
        }

        public int getCardMaxCount()
        {
            return m_BattleCards.Length;
        }

        public int getCardCount()
        {
            return m_CardCount;
        }

        public BattleCard getCard(int index)
        {
            return m_BattleCards[index];
        }
    }

    /// <summary>
    /// カード制御
    /// </summary>
    public class _BattleCardManager
    {
        public HandArea m_NextArea = null;
        public HandArea m_HandArea = null;
        public FieldAreas m_FieldAreas = null;
        public InHandArea m_InHandArea = null;
        public BattleCard[] m_BattleCards = null;
        public bool m_IsStartCountDown = false;

        // スキル発動に必要なカード属性.
        private bool[] m_IsSkillCardInfo = new bool[(int)MasterDataDefineLabel.ElementType.MAX];
        private bool m_IsUpdateSkillCardInfo = false;

        private BattleSkillReachInfo m_BattleSkillReachInfo = null;

#if BUILD_TYPE_DEBUG
        public MasterDataDefineLabel.ElementType[] m_DebugNextCardFix = null;
#endif //BUILD_TYPE_DEBUG

        public _BattleCardManager()
        {
            m_NextArea = new HandArea(5);
            m_HandArea = new HandArea(5);
#if BUILD_TYPE_DEBUG
            m_DebugNextCardFix = new MasterDataDefineLabel.ElementType[5];
#endif //BUILD_TYPE_DEBUG
            m_FieldAreas = new FieldAreas(5, 5);
            m_InHandArea = new InHandArea(m_HandArea.getCardMaxCount());
            m_BattleCards = new BattleCard[
                ((m_NextArea != null) ? m_NextArea.getCardMaxCount() : 0)
                + m_HandArea.getCardMaxCount()
                + m_FieldAreas.getFieldAreaCountMax() * m_FieldAreas.getFieldArea(0).getCardMaxCount()];

            for (int idx = 0; idx < m_BattleCards.Length; idx++)
            {
                m_BattleCards[idx] = new BattleCard(this);
            }
        }

        /// <summary>
        /// カードをリセット
        /// </summary>
        /// <param name="is_keep_hand_area">手札の状態を維持するかどうか</param>
        public void reset(bool is_keep_hand_area)
        {
            putInHandCard(-1, -1);
            if (is_keep_hand_area == false)
            {
                if (m_NextArea != null)
                {
                    m_NextArea.reset();
                }
                m_HandArea.reset();
            }
            m_FieldAreas.reset();
            m_IsStartCountDown = false;
        }

        /// <summary>
        /// プールの中から未使用カードを取得
        /// </summary>
        /// <returns></returns>
        public BattleCard getUnusedCard()
        {
            for (int idx = 0; idx < m_BattleCards.Length; idx++)
            {
                BattleCard battle_card = m_BattleCards[idx];
                if (battle_card.getPhase() == BattleCard.Phase.UNUSED)
                {
                    return battle_card;
                }
            }

            return null;
        }

        /// <summary>
        /// 場などからカードを取り省く
        /// </summary>
        /// <param name="battle_card"></param>
        /// <param name="is_unuse_card"></param>
        public void removeCard(BattleCard battle_card, bool is_unuse_card)
        {
            switch (battle_card.getPhase())
            {
                case BattleCard.Phase.NEXT_AREA:
                    m_NextArea.removeCard(battle_card, is_unuse_card);
                    break;

                case BattleCard.Phase.HAND_AREA:
                    m_HandArea.removeCard(battle_card, is_unuse_card);
                    break;

                case BattleCard.Phase.IN_HAND:
                    m_InHandArea.removeCard(battle_card, is_unuse_card);
                    break;

                case BattleCard.Phase.FIELD_AREA:
                    m_FieldAreas.removeCard(battle_card, is_unuse_card);
                    break;
            }
        }

        public void setCardToNextArea(int index, BattleCard battle_card)
        {
            if (m_NextArea != null)
            {
                removeCard(battle_card, false);
                m_NextArea.setCard(index, battle_card);
                battle_card.setPhase(BattleCard.Phase.NEXT_AREA);
            }
        }

        /// <summary>
        /// 手札エリアにカードをセット
        /// </summary>
        /// <param name="index"></param>
        /// <param name="battle_card"></param>
        public void setCardToHandArea(int index, BattleCard battle_card)
        {
            removeCard(battle_card, false);
            m_HandArea.setCard(index, battle_card);
        }

        /// <summary>
        /// 手持ちエリアにカードをセット（同時に手札エリアから取り省かれる）
        /// </summary>
        /// <param name="battle_card"></param>
        public void setCardToInHand(BattleCard battle_card)
        {
            removeCard(battle_card, false);
            m_InHandArea.addCard(battle_card);
        }

        /// <summary>
        /// 場エリアにカードをセット（同時に手持ちエリアから取り省かれる）
        /// </summary>
        /// <param name="index"></param>
        /// <param name="battle_card"></param>
        /// <returns></returns>
        public bool setCardToFieldArea(int index, BattleCard battle_card)
        {
            removeCard(battle_card, false);
            return m_FieldAreas.addCard(index, battle_card);
        }

        /// <summary>
        /// プレイヤーによる操作中かどうか
        /// </summary>
        /// <returns></returns>
        public bool isCatchingCardByPlayer()
        {
            return (m_InHandArea.getCardCount() > 0);
        }

        /// <summary>
        /// 手札エリアに新しいカードを配る
        /// </summary>
        /// <param name="hand_area_index"></param>
        /// <param name="element_type"></param>
        public void addNewCardInHandArea(int hand_area_index, MasterDataDefineLabel.ElementType element_type)
        {
            BattleCard battle_card = getUnusedCard();
            if (battle_card != null)
            {
                MasterDataDefineLabel.ElementType wrk_element_type = element_type;
#if BUILD_TYPE_DEBUG
                if (m_DebugNextCardFix[hand_area_index] != MasterDataDefineLabel.ElementType.NONE)
                {
                    wrk_element_type = m_DebugNextCardFix[hand_area_index];
                }
#endif //BUILD_TYPE_DEBUG

                battle_card.setElementType(wrk_element_type, BattleCard.ChangeCause.NEW_CARD);

                if (m_NextArea != null)
                {
                    BattleCard next_card = m_NextArea.getCard(hand_area_index);
                    if (next_card != null)
                    {
                        setCardToHandArea(hand_area_index, next_card);
                    }

                    setCardToNextArea(hand_area_index, battle_card);
                }
                else
                {
                    setCardToHandArea(hand_area_index, battle_card);
                }
            }
        }

        /// <summary>
        /// 手札エリアのカードを手に持つ
        /// </summary>
        /// <param name=""></param>
        public void catchHandAreaCard(int hand_area_touch_index)
        {
            BattleCard battle_card = m_HandArea.getCard(hand_area_touch_index);
            if (battle_card != null)
            {
                if (m_InHandArea.getCardCount() > 0)
                {
                    BattleCard in_hand_card = m_InHandArea.getCard(0);
                    if (battle_card.getElementType() != in_hand_card.getElementType())
                    {
                        battle_card = null;
                    }
                    else
                    {
                        int step = (hand_area_touch_index > in_hand_card.m_HandCardIndex) ? 1 : -1;
                        for (int idx = in_hand_card.m_HandCardIndex; idx != hand_area_touch_index; idx += step)
                        {
                            BattleCard temp_battle_card = m_HandArea.getCard(idx + step);
                            if (temp_battle_card != null && temp_battle_card.getElementType() != in_hand_card.getElementType())
                            {
                                battle_card = null;
                                break;
                            }
                        }
                    }
                }

                if (battle_card != null)
                {
                    setCardToInHand(battle_card);
                }
            }
        }

        /// <summary>
        /// 手持ちカードをどこかに置く（どこに置いたかで挙動が変わる）
        /// </summary>
        /// <param name="hand_area_touch_index"></param>
        /// <param name="field_area_touch_index"></param>
        public void putInHandCard(int hand_area_touch_index, int field_area_touch_index)
        {
            if (m_InHandArea.getCardCount() > 0)
            {
                if (hand_area_touch_index >= 0)
                {
                    // 手札エリアに置いた
                    for (int idx = 0; idx < m_InHandArea.getCardCount(); idx++)
                    {
                        BattleCard battle_card = m_InHandArea.getCard(idx);
                        if (battle_card != null)
                        {
                            setCardToHandArea(hand_area_touch_index, battle_card);
                            idx--;
                        }
                    }
                }
                else if (field_area_touch_index >= 0)
                {
                    // 場札エリアに置いた
                    for (int idx = 0; idx < m_InHandArea.getCardCount(); idx++)
                    {
                        BattleCard battle_card = m_InHandArea.getCard(idx);
                        if (battle_card != null)
                        {
                            bool is_success = setCardToFieldArea(field_area_touch_index, battle_card);
                            if (is_success)
                            {
                                // 新しいカードを手札エリアに
                                m_IsStartCountDown = true;
                            }
                            else
                            {
                                // 場札エリアに置けなかったので手札エリアへ戻す
                                setCardToHandArea(battle_card.m_HandCardIndex, battle_card);
                            }
                            idx--;
                        }
                    }
                }
                else
                {
                    // 手札エリアでも場札エリアでもないところに置いた
                    for (int idx = 0; idx < m_InHandArea.getCardCount(); idx++)
                    {
                        BattleCard battle_card = m_InHandArea.getCard(idx);
                        if (battle_card != null)
                        {
                            setCardToHandArea(battle_card.m_HandCardIndex, battle_card);
                            idx--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 時間切れで手に持っているカードを手札位置に戻す
        /// </summary>
        public void putInHandCardByTimeOver()
        {
            for (int idx = 0; idx < m_InHandArea.getCardCount(); idx++)
            {
                BattleCard battle_card = m_InHandArea.getCard(idx);
                if (battle_card != null)
                {
                    setCardToHandArea(battle_card.m_HandCardIndex, battle_card);
                    battle_card.m_HoldMoveTime = 0.5f;  // すぐに戻さないでしばらくその場にとどめる。
                    idx--;
                }
            }
        }

        /// <summary>
        /// アクティブスキルに使用されるカードかどうかのフラグをリセット
        /// </summary>
        public void resetSkillCardInfo()
        {
            for (int idx = 0; idx < m_IsSkillCardInfo.Length; idx++)
            {
                m_IsSkillCardInfo[idx] = false;
            }
            m_IsUpdateSkillCardInfo = true;
        }

        /// <summary>
        /// アクティブスキルに使用されるカードであることをセット
        /// </summary>
        /// <param name="element_type"></param>
        public void setSkillCardInfo(MasterDataDefineLabel.ElementType element_type)
        {
            if (element_type > MasterDataDefineLabel.ElementType.NONE && element_type < MasterDataDefineLabel.ElementType.MAX)
            {
                m_IsSkillCardInfo[(int)element_type] = true;
            }
            m_IsUpdateSkillCardInfo = true;
        }

        /// <summary>
        /// アクティブスキルに使用されるカードであるかどうか調べる
        /// </summary>
        /// <param name="element_type"></param>
        /// <returns></returns>
        public bool isSkillCard(MasterDataDefineLabel.ElementType element_type)
        {
            if (element_type > MasterDataDefineLabel.ElementType.NONE && element_type < MasterDataDefineLabel.ElementType.MAX)
            {
                return m_IsSkillCardInfo[(int)element_type];
            }
            return false;
        }

        /// <summary>
        /// アクティブスキルに使用されるカードの種類に増減・変化があったかどうか調べる
        /// </summary>
        /// <param name="is_clear_flag"></param>
        /// <returns></returns>
        public bool isUpdateSkillCard(bool is_clear_flag)
        {
            bool ret_val = m_IsUpdateSkillCardInfo;
            if (is_clear_flag)
            {
                m_IsUpdateSkillCardInfo = false;
            }
            return ret_val;
        }

        /// <summary>
        /// リーチ情報をセット
        /// </summary>
        /// <param name="reach_info"></param>
        public void setReachInfo(BattleSkillReachInfo reach_info)
        {
            m_BattleSkillReachInfo = reach_info;
        }

        /// <summary>
        /// リーチ情報を取得
        /// </summary>
        /// <returns></returns>
        public BattleSkillReachInfo getReachInfo()
        {
            return m_BattleSkillReachInfo;
        }

        /// <summary>
        /// カード周りのエフェクト情報
        /// </summary>
        public struct EffectInfo
        {
            /// <summary>
            /// エフェクトの発生位置
            /// </summary>
            public enum EffectPosition
            {
                HAND_CARD_AREA,
                FIELD_AREA,
            }

            /// <summary>
            /// エフェクトの種類
            /// </summary>
            public enum EffectType
            {
                CARD_CHANGE,    // カード変化
                CARD_DESTROY,   // カード破壊
                SKILL_FORMED,   // スキル成立
            }

            public EffectPosition m_EffectPosition;
            public int m_PositionIndex;
            public EffectType m_EffectType;
        }
        private int m_EffectCount = 0;
        private EffectInfo[] m_EffectInfos = new EffectInfo[10];

        /// <summary>
        /// エフェクト情報をリセット
        /// </summary>
        public void resetEffectInfo()
        {
            m_EffectCount = 0;
        }

        /// <summary>
        /// エフェクト情報を追加
        /// </summary>
        /// <param name="effect_position"></param>
        /// <param name="index"></param>
        /// <param name="effect_type"></param>
        public void addEffectInfo(EffectInfo.EffectPosition effect_position, int index, EffectInfo.EffectType effect_type)
        {
            if (m_EffectCount < m_EffectInfos.Length)
            {
                m_EffectInfos[m_EffectCount].m_EffectPosition = effect_position;
                m_EffectInfos[m_EffectCount].m_PositionIndex = index;
                m_EffectInfos[m_EffectCount].m_EffectType = effect_type;
                m_EffectCount++;
            }
        }

        public int getEffectInfoCount()
        {
            return m_EffectCount;
        }

        public EffectInfo getEffectInfo(int index)
        {
            return m_EffectInfos[index];
        }
    }
}

public class BattleCardArea : MonoBehaviour
{
    public BattleScene._BattleCardManager m_BattleCardManager = new BattleScene._BattleCardManager();

    public HandAreaViewControl m_HandAreaViewControl = null;
    public FieldAreaViewControl m_FieldAreaViewControl = null;
    public InHandViewControl m_InHandViewControl = null;
    public BattleReachLineViewControl m_BattleReachLineViewControl = null;
    public GameObject m_BattleCardPrefab = null;

    private BattleCardViewContorl[] m_BattleCardContorl = null;

    private int m_HandAreaTouchIndex = -1;
    private int m_FieldAreaTouchIndex = -1;

    private int m_InHandCardCount = 0;

    // Use this for initialization
    void Start()
    {
        m_HandAreaViewControl.init(m_BattleCardManager.m_HandArea, m_BattleCardManager.m_NextArea);
        m_FieldAreaViewControl.init(m_BattleCardManager.m_FieldAreas);
        m_InHandViewControl.init(m_BattleCardManager.m_InHandArea);

        m_BattleCardContorl = new BattleCardViewContorl[m_BattleCardManager.m_BattleCards.Length];
        for (int idx = 0; idx < m_BattleCardContorl.Length; idx++)
        {
            GameObject battle_card_object = Instantiate(m_BattleCardPrefab);
            battle_card_object.transform.parent = transform;
            battle_card_object.SetActive(false);
            BattleCardViewContorl battle_card_control = battle_card_object.GetComponent<BattleCardViewContorl>();
            battle_card_control.init(m_BattleCardManager.m_BattleCards[idx]);
            m_BattleCardContorl[idx] = battle_card_control;
        }
        m_BattleCardManager.reset(false);
    }

    // Update is called once per frame
    void Update()
    {
        BattleTouchInput battle_touch_input = BattleTouchInput.Instance;
        if (battle_touch_input.isTouching())
        {
            if (battle_touch_input.isOverrideTouchMode() == false)
            {
                m_HandAreaTouchIndex = m_HandAreaViewControl.getTouchIndex();
                m_FieldAreaTouchIndex = m_FieldAreaViewControl.getTouchIndex();
            }
            else
            {
                m_HandAreaTouchIndex = battle_touch_input.getOverrideTouchMode_HandAreaTouchIndex();
                m_FieldAreaTouchIndex = battle_touch_input.getOverrideTouchMode_FieldAreaTouchIndex();
            }

            if (m_HandAreaTouchIndex >= 0)
            {
                // 手札エリアに触れているのでカードがあれば拾う.
                m_BattleCardManager.catchHandAreaCard(m_HandAreaTouchIndex);
            }
        }
        else
        {
            if (battle_touch_input.isWorking())
            {
                if (m_BattleCardManager.isCatchingCardByPlayer())
                {
                    // 手を放したのでカードを置く.
                    m_BattleCardManager.putInHandCard(m_HandAreaTouchIndex, m_FieldAreaTouchIndex);
                }
            }
            else
            {
                // 入力が中断した場合はカードを元の位置に戻す.
                m_BattleCardManager.putInHandCardByTimeOver();
            }

            m_HandAreaTouchIndex = -1;
            m_FieldAreaTouchIndex = -1;
        }

        if (m_BattleCardManager.m_InHandArea.getCardCount() > 0)
        {
            m_FieldAreaViewControl.setViewSelect(m_FieldAreaTouchIndex);
        }
        else
        {
            m_FieldAreaViewControl.setViewSelect(-1);
        }

        // カードの明暗表示を切り替え
        if (m_BattleCardManager.isUpdateSkillCard(true))
        {
            for (int idx = 0; idx < m_BattleCardManager.m_BattleCards.Length; idx++)
            {
                m_BattleCardManager.m_BattleCards[idx].m_IsOn = m_BattleCardManager.isSkillCard(m_BattleCardManager.m_BattleCards[idx].getElementType());
            }
        }

        // リーチラインの描画更新
        BattleSkillReachInfo reach_info = m_BattleCardManager.getReachInfo();
        if (reach_info != null)
        {
            m_BattleReachLineViewControl.clearLine();
            m_BattleReachLineViewControl.setCameraPosition(battle_touch_input.getCamera().transform.position);
            for (int hand_index = 0; hand_index < m_BattleCardManager.m_HandArea.getCardMaxCount(); hand_index++)
            {
                Vector3 hand_position;
                BattleCard battle_card = m_BattleCardManager.m_HandArea.getCard(hand_index);
                if (battle_card != null)
                {
                    hand_position = m_HandAreaViewControl.getBattleCardTransform(hand_index, HandAreaViewControl.TransformType.HAND_AREA).position;
                }
                else
                {
                    battle_card = m_BattleCardManager.m_InHandArea.getCard(0);
                    if (battle_card == null)
                    {
                        // 場にパネルを置いた瞬間：直前の手の位置に表示
                        hand_position = m_InHandViewControl.getBattleCardTransform(0).position;
                    }
                    else if (battle_card.isChangePhase())
                    {
                        // パネルを手に持った瞬間：手の位置がまだ更新されていないので、手札領域の座標を使用
                        hand_position = m_HandAreaViewControl.getBattleCardTransform(hand_index, HandAreaViewControl.TransformType.HAND_AREA).position;
                    }
                    else
                    {
                        hand_position = m_InHandViewControl.getBattleCardTransform(0).position;
                    }
                }

                for (int field_index = 0; field_index < m_BattleCardManager.m_FieldAreas.getFieldAreaCountMax(); field_index++)
                {
                    MasterDataDefineLabel.ElementType element_type = reach_info.getReachInfo(field_index, hand_index);
                    if (element_type != MasterDataDefineLabel.ElementType.NONE)
                    {
                        Vector3 field_position = m_FieldAreaViewControl.getFieldTransform(field_index).position;
                        m_BattleReachLineViewControl.addLine(hand_position, field_position, element_type);
                    }
                }
            }
        }
        else
        {
            m_BattleReachLineViewControl.clearLine();
        }

        int in_hand_card_count = m_BattleCardManager.m_InHandArea.getCardCount();
        if (in_hand_card_count != m_InHandCardCount)
        {
            if (in_hand_card_count == 1 && m_InHandCardCount == 0)
            {
                //１枚目のカードを手に持った時のＳＥ
                SoundUtil.PlaySE(SEID.SE_BATLE_COST_PLUS_1);
            }
            else if (in_hand_card_count > 1)
            {
                //２枚目以上のカードを手に持った時のＳＥ
                SoundUtil.PlaySE(SEID.SE_BATLE_COST_PLUS_2);

                // エフェクト
                EffectManager.Instance.playEffect(SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DBattleCostPlus, Vector3.zero, Vector3.zero, m_InHandViewControl.getBattleCardTransform(0), null, 5.0f);
            }
            else if (in_hand_card_count == 0)
            {
                bool is_put_field = false;  // 札を場に置いたかどうか
                for (int idx = 0; idx < m_BattleCardManager.m_HandArea.getCardMaxCount(); idx++)
                {
                    if (m_BattleCardManager.m_HandArea.getCard(idx) == null)
                    {
                        // 手札に空きがあるので場に置いたと判断
                        is_put_field = true;
                        break;
                    }
                }

                if (is_put_field)
                {
                    // 場に札を置いた時のＳＥ
                    SoundUtil.PlaySE(SEID.SE_BATLE_COST_PUT);
                }
            }

            m_InHandCardCount = in_hand_card_count;
        }

        // 場・カード関連のエフェクト
        for (int idx = 0; idx < m_BattleCardManager.getEffectInfoCount(); idx++)
        {
            _BattleCardManager.EffectInfo effect_info = m_BattleCardManager.getEffectInfo(idx);

            Transform trans = getEffectPosition(effect_info.m_PositionIndex, effect_info.m_EffectPosition);

            if (trans != null)
            {
                if (SceneObjReferGameMain.Instance != null)
                {
                    GameObject effect_prefab = null;
                    switch (effect_info.m_EffectType)
                    {
                        case _BattleCardManager.EffectInfo.EffectType.CARD_CHANGE:
                            effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HandCardTransform;
                            break;

                        case _BattleCardManager.EffectInfo.EffectType.CARD_DESTROY:
                            effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_HandCardDestroy;
                            break;

                        case _BattleCardManager.EffectInfo.EffectType.SKILL_FORMED:
                            effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DSkill;
                            break;
                    }

                    if (effect_prefab != null)
                    {
                        EffectManager.Instance.playEffect(effect_prefab, Vector3.zero, Vector3.zero, trans, null, 5.0f);
                    }
                }
            }
        }
        m_BattleCardManager.resetEffectInfo();
    }

    public Transform getEffectPosition(int field_index, _BattleCardManager.EffectInfo.EffectPosition effect_position)
    {
        Transform trans = null;
        switch (effect_position)
        {
            case _BattleCardManager.EffectInfo.EffectPosition.HAND_CARD_AREA:
                trans = m_HandAreaViewControl.getBattleCardTransform(field_index, HandAreaViewControl.TransformType.HAND_AREA);
                break;

            case _BattleCardManager.EffectInfo.EffectPosition.FIELD_AREA:
                trans = m_FieldAreaViewControl.getFieldTransform(field_index);
                break;
        }

        return trans;
    }
}

