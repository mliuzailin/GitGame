using UnityEngine;
using System.Collections;
using BattleScene;

public class HandAreaViewControl : MonoBehaviour
{
    public enum TransformType
    {
        HAND_AREA,  // 手札位置
        NEXT_AREA,  // 次札位置
        APPEAR_AREA,    // 出現位置

        MAX
    }


    private HandArea m_HandArea = null;
    private HandArea m_NextArea = null;

    public GameObject m_HandAreaPrefab = null;

    private GameObject[] m_HandAreaObjects = null;
    private FieldCollision[] m_HandAreaObjectsCollision = null;
    private Transform[,] m_HandAreaTransforms = null;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_HandArea != null)
        {
            for (int idx = 0; idx < m_HandArea.getCardMaxCount(); idx++)
            {
                BattleScene.BattleCard battle_card = m_HandArea.getCard(idx);
                if (battle_card != null)
                {
                    if (battle_card.getChangeCause() == BattleScene.BattleCard.ChangeCause.NEW_CARD)
                    {
                        battle_card.resetChageCause();
                        battle_card.setPhase(BattleScene.BattleCard.Phase.HAND_AREA);
                        battle_card.m_ViewControl.transform.SetParent(getBattleCardTransform(idx, TransformType.NEXT_AREA), false);
                        battle_card.m_ViewControl.transform.localPosition = Vector3.zero;
                        battle_card.m_ViewControl.transform.localRotation = Quaternion.identity;
                        battle_card.m_ViewControl.transform.localScale = Vector3.one;
                    }

                    if (battle_card.isChangePhase())
                    {
                        float hold_time = battle_card.m_HoldMoveTime;
                        battle_card.resetChangePhase();
                        battle_card.m_ViewControl.gameObject.SetActive(true);
                        battle_card.m_ViewControl.setSnapToParent(getBattleCardTransform(idx, TransformType.HAND_AREA), 0.1f, hold_time);
                    }
                }
            }
        }

        if (m_NextArea != null)
        {
            for (int idx = 0; idx < m_NextArea.getCardMaxCount(); idx++)
            {
                BattleScene.BattleCard battle_card = m_NextArea.getCard(idx);
                if (battle_card != null)
                {
                    if (battle_card.getChangeCause() == BattleScene.BattleCard.ChangeCause.NEW_CARD)
                    {
                        battle_card.resetChageCause();
                        battle_card.setPhase(BattleScene.BattleCard.Phase.NEXT_AREA);
                        battle_card.m_ViewControl.transform.SetParent(getBattleCardTransform(idx, TransformType.APPEAR_AREA), false);
                        battle_card.m_ViewControl.transform.localPosition = Vector3.zero;
                        battle_card.m_ViewControl.transform.localRotation = Quaternion.identity;
                        battle_card.m_ViewControl.transform.localScale = Vector3.one;
                    }

                    if (battle_card.isChangePhase())
                    {
                        battle_card.resetChangePhase();
                        battle_card.m_ViewControl.gameObject.SetActive(true);
                        battle_card.m_ViewControl.setSnapToParent(getBattleCardTransform(idx, TransformType.NEXT_AREA), 0.1f);
                    }
                }
            }
        }
    }

    public void init(HandArea hand_area, HandArea next_area)
    {
        m_HandArea = hand_area;
        m_NextArea = next_area;
        m_HandAreaObjects = new GameObject[m_HandArea.getCardMaxCount()];
        m_HandAreaObjectsCollision = new FieldCollision[m_HandArea.getCardMaxCount()];
        m_HandAreaTransforms = new Transform[m_HandArea.getCardMaxCount(), (int)TransformType.MAX];
        for (int idx = 0; idx < m_HandAreaObjects.Length; idx++)
        {
            GameObject hand_card_area_object = Instantiate(m_HandAreaPrefab);
            hand_card_area_object.transform.SetParent(gameObject.transform, false);
            hand_card_area_object.transform.localPosition = new Vector3(idx - (m_HandAreaObjects.Length - 1) * 0.5f, 0.0f, 0.0f);
            m_HandAreaObjects[idx] = hand_card_area_object;
            m_HandAreaObjectsCollision[idx] = hand_card_area_object.GetComponent<FieldCollision>();

            m_HandAreaTransforms[idx, (int)TransformType.HAND_AREA] = hand_card_area_object.transform.GetChild(0);
            m_HandAreaTransforms[idx, (int)TransformType.NEXT_AREA] = m_HandAreaTransforms[idx, (int)TransformType.HAND_AREA].GetChild(0);
            m_HandAreaTransforms[idx, (int)TransformType.APPEAR_AREA] = m_HandAreaTransforms[idx, (int)TransformType.NEXT_AREA].GetChild(0);

            // 臨時で位置調整
            if (m_HandArea != null)
            {
                switch (idx)
                {
                    case 0:
                        m_HandAreaTransforms[idx, (int)TransformType.NEXT_AREA].localPosition = new Vector3(-0.122f, -1.013f, 0.0f);
                        m_HandAreaTransforms[idx, (int)TransformType.NEXT_AREA].localScale = new Vector3(0.54f, 0.54f, 0.54f);
                        break;

                    case 1:
                        m_HandAreaTransforms[idx, (int)TransformType.NEXT_AREA].localPosition = new Vector3(-0.058f, -1.257f, 0.0f);
                        m_HandAreaTransforms[idx, (int)TransformType.NEXT_AREA].localScale = new Vector3(0.54f, 0.54f, 0.54f);
                        break;

                    case 2:
                        m_HandAreaTransforms[idx, (int)TransformType.NEXT_AREA].localPosition = new Vector3(0.0f, -1.381f, 0.0f);
                        m_HandAreaTransforms[idx, (int)TransformType.NEXT_AREA].localScale = new Vector3(0.52f, 0.52f, 0.52f);
                        break;

                    case 3:
                        m_HandAreaTransforms[idx, (int)TransformType.NEXT_AREA].localPosition = new Vector3(0.058f, -1.257f, 0.0f);
                        m_HandAreaTransforms[idx, (int)TransformType.NEXT_AREA].localScale = new Vector3(0.54f, 0.54f, 0.54f);
                        break;

                    case 4:
                        m_HandAreaTransforms[idx, (int)TransformType.NEXT_AREA].localPosition = new Vector3(0.122f, -1.013f, 0.0f);
                        m_HandAreaTransforms[idx, (int)TransformType.NEXT_AREA].localScale = new Vector3(0.54f, 0.54f, 0.54f);
                        break;
                }
            }
        }
    }

    public Transform getBattleCardTransform(int area_index, TransformType transform_type)
    {
        return m_HandAreaTransforms[area_index, (int)transform_type];
    }

    /// <summary>
    /// マウスカーソルがカード置き場に乗っているかを調べ乗っているカード置き場の番号を返す
    /// </summary>
    /// <returns></returns>
    public int getTouchIndex()
    {
        for (int idx = 0; idx < m_HandAreaObjectsCollision.Length; idx++)
        {
            FieldCollision field_collision = m_HandAreaObjectsCollision[idx];
            if (field_collision != null)
            {
                if (field_collision.IsMouseOver)
                {
                    return idx;
                }
            }
        }

        return -1;
    }
}

