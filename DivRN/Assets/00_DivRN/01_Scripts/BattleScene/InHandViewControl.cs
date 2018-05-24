using UnityEngine;
using System.Collections;
using BattleScene;

public class InHandViewControl : MonoBehaviour
{
    private InHandArea m_InHandArea = null;

    public GameObject m_InHandPrefab = null;

    private GameObject m_InHandObject = null;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_InHandArea != null)
        {
            float hand_z = BattleTouchInput.Instance.getScreenPosition(transform.position).z;
            Vector3 hand_world_position = BattleTouchInput.Instance.getWorldPosition(hand_z);
            m_InHandObject.transform.position = hand_world_position;

            for (int card_idx = 0; card_idx < m_InHandArea.getCardCount(); card_idx++)
            {
                BattleScene.BattleCard battle_card = m_InHandArea.getCard(card_idx);
                if (battle_card != null)
                {
                    if (battle_card.isChangePhase())
                    {
                        battle_card.resetChangePhase();
                        battle_card.m_ViewControl.gameObject.SetActive(true);
                        battle_card.m_ViewControl.setSnapToParent(getBattleCardTransform(card_idx), 0.0f);
                    }
                }
            }
        }
    }

    public void init(InHandArea in_hand_area)
    {
        m_InHandArea = in_hand_area;

        m_InHandObject = Instantiate(m_InHandPrefab);
        BattleSceneUtil.setRide(m_InHandObject.transform, gameObject.transform);
    }

    public Transform getBattleCardTransform(int card_index)
    {
        return m_InHandObject.transform.GetChild(card_index);
    }
}
