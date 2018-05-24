using UnityEngine;
using System.Collections;
using BattleScene;

public class FieldAreaViewControl : MonoBehaviour
{
    private FieldAreas m_FieldAreas = null;

    public GameObject m_FieldAreaPrefab = null;

    private class ViewObject
    {
        public GameObject m_Root = null;
        public GameObject[] m_Cards = null;
        public GameObject m_Boost = null;
        public GameObject m_Full = null;
        public GameObject m_PanelBase = null;
        public GameObject m_PanelSelect = null;
        public GameObject m_PanelRBase = null;
        public GameObject m_PanelRSelect = null;
        public FieldCollision m_Collision = null;
    }

    private ViewObject[] m_ViewObjects = null;

    private int m_BasePanelUpdateCounter = 0;
    private bool m_IsResurrectMode = false;
    private int m_SelectedField = -1;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_FieldAreas != null)
        {
            bool is_update_field_view = false;

            // 復活可能かどうか更新
            if (m_FieldAreas.m_IsResurrectMode != m_IsResurrectMode)
            {
                m_IsResurrectMode = m_FieldAreas.m_IsResurrectMode;
                is_update_field_view = false;
            }

            // 定期的更新の確認
            m_BasePanelUpdateCounter--;
            if (m_BasePanelUpdateCounter <= 0)
            {
                is_update_field_view = true;
            }

            // ＢＯＯＳＴ・ＦＵＬＬが更新されたかを更新
            if (is_update_field_view == false)
            {
                for (int field_idx = 0; field_idx < m_FieldAreas.getFieldAreaCountMax(); field_idx++)
                {
                    FieldArea field_area = m_FieldAreas.getFieldArea(field_idx);
                    if (field_area != null)
                    {
                        if (m_ViewObjects[field_idx].m_Boost.IsActive() != field_area.m_IsBoost
                            || m_ViewObjects[field_idx].m_Full.IsActive() != field_area.isFull()
                        )
                        {
                            is_update_field_view = true;
                            break;
                        }
                    }
                }
            }

            if (is_update_field_view)
            {
                m_BasePanelUpdateCounter = 30;
            }

            for (int field_idx = 0; field_idx < m_FieldAreas.getFieldAreaCountMax(); field_idx++)
            {
                FieldArea field_area = m_FieldAreas.getFieldArea(field_idx);
                if (field_area != null)
                {
                    // カード表示を更新
                    for (int card_idx = 0; card_idx < field_area.getCardCount(); card_idx++)
                    {
                        BattleScene.BattleCard battle_card = field_area.getCard(card_idx);
                        if (battle_card != null)
                        {
                            if (battle_card.isChangePhase())
                            {
                                battle_card.resetChangePhase();
                                float duration = 0.05f;
                                if (battle_card.m_ViewControl.gameObject.IsActive() == false)
                                {
                                    battle_card.m_ViewControl.gameObject.SetActive(true);
                                    duration = 0.0f;
                                }
                                battle_card.m_ViewControl.setSnapToParent(getBattleCardTransform(field_idx, card_idx), duration);
                            }
                        }
                    }

                    // 下地パネル・ＢＯＯＳＴ・ＦＵＬＬ表示を更新
                    if (is_update_field_view)
                    {
                        if (m_SelectedField != field_idx)
                        {
                            if (m_IsResurrectMode
                                && field_area.m_IsBoost
                            )
                            {
                                m_ViewObjects[field_idx].m_PanelBase.SetActive(false);
                                m_ViewObjects[field_idx].m_PanelSelect.SetActive(false);
                                m_ViewObjects[field_idx].m_PanelRBase.SetActive(true);
                                m_ViewObjects[field_idx].m_PanelRSelect.SetActive(false);
                            }
                            else
                            {
                                m_ViewObjects[field_idx].m_PanelBase.SetActive(true);
                                m_ViewObjects[field_idx].m_PanelSelect.SetActive(false);
                                m_ViewObjects[field_idx].m_PanelRBase.SetActive(false);
                                m_ViewObjects[field_idx].m_PanelRSelect.SetActive(false);
                            }
                        }
                        else
                        {
                            if (m_IsResurrectMode
                                && field_area.m_IsBoost
                            )
                            {
                                m_ViewObjects[field_idx].m_PanelBase.SetActive(false);
                                m_ViewObjects[field_idx].m_PanelSelect.SetActive(false);
                                m_ViewObjects[field_idx].m_PanelRBase.SetActive(false);
                                m_ViewObjects[field_idx].m_PanelRSelect.SetActive(true);
                            }
                            else
                            {
                                m_ViewObjects[field_idx].m_PanelBase.SetActive(false);
                                m_ViewObjects[field_idx].m_PanelSelect.SetActive(true);
                                m_ViewObjects[field_idx].m_PanelRBase.SetActive(false);
                                m_ViewObjects[field_idx].m_PanelRSelect.SetActive(false);
                            }
                        }

                        m_ViewObjects[field_idx].m_Boost.SetActive(field_area.m_IsBoost);
                        m_ViewObjects[field_idx].m_Full.SetActive(field_area.isFull());
                    }
                }
            }
        }
    }

    public void init(FieldAreas field_areas)
    {
        m_FieldAreas = field_areas;

        m_ViewObjects = new ViewObject[m_FieldAreas.getFieldAreaCountMax()];
        for (int field_idx = 0; field_idx < m_ViewObjects.Length; field_idx++)
        {
            GameObject field_area_object = Instantiate(m_FieldAreaPrefab);
            BattleSceneUtil.setRide(field_area_object.transform, gameObject.transform);
            field_area_object.transform.localPosition = new Vector3(field_idx - (m_ViewObjects.Length - 1) * 0.5f, 0.0f, 0.0f);

            m_ViewObjects[field_idx] = new ViewObject();
            m_ViewObjects[field_idx].m_Root = field_area_object;
            int card_count = m_FieldAreas.getFieldArea(0).getCardMaxCount();
            m_ViewObjects[field_idx].m_Cards = new GameObject[card_count];
            for (int card_idx = 0; card_idx < card_count; card_idx++)
            {
                m_ViewObjects[field_idx].m_Cards[card_idx] = field_area_object.transform.GetChild(card_idx).gameObject;
            }
            m_ViewObjects[field_idx].m_Boost = field_area_object.transform.Find("Boost").gameObject;
            m_ViewObjects[field_idx].m_Full = field_area_object.transform.Find("Full").gameObject;
            m_ViewObjects[field_idx].m_PanelBase = field_area_object.transform.Find("PanelBase").gameObject;
            m_ViewObjects[field_idx].m_PanelSelect = field_area_object.transform.Find("PanelSelect").gameObject;
            m_ViewObjects[field_idx].m_PanelRBase = field_area_object.transform.Find("PanelRBase").gameObject;
            m_ViewObjects[field_idx].m_PanelRSelect = field_area_object.transform.Find("PanelRSelect").gameObject;
            m_ViewObjects[field_idx].m_Collision = field_area_object.GetComponent<FieldCollision>();
        }
    }

    public Transform getFieldTransform(int field_index)
    {
        return m_ViewObjects[field_index].m_Root.transform;
    }

    public Transform getBattleCardTransform(int field_index, int card_index)
    {
        return m_ViewObjects[field_index].m_Cards[card_index].transform;
    }

    public int getTouchIndex()
    {
        for (int field_idx = 0; field_idx < m_ViewObjects.Length; field_idx++)
        {
            FieldCollision field_collision = m_ViewObjects[field_idx].m_Collision;
            if (field_collision != null)
            {
                if (field_collision.IsMouseOver)
                {
                    return field_idx;
                }
            }
        }

        return -1;
    }

    public void setViewSelect(int field_index)
    {
        if (m_SelectedField != field_index)
        {
            m_SelectedField = field_index;
            m_BasePanelUpdateCounter = 0;
        }
    }
}

