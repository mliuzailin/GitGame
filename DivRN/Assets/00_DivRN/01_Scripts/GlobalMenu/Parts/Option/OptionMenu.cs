using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MenuPartsBase
{
    [SerializeField]
    GameObject m_Cointent;
    [SerializeField]
    ScrollRect m_ScrollRect;

    public enum ItemType
    {
        NONE = -1,
        BGM = 0,
        SE,
        GUIDE,
        TIPS,
        VOICE,
        SPEED,
        SKILL_TURN,
        CONFIRM_AS,
        SKILL_COST,
        BATTLE_ACHIEVE,
        QUEST_END_TIPS,
        AUTO_PLAY_STOP_BOSS,
        AUTO_PLAY_USE_AS,

        NOTIFICATION,

        NOT_EVENT,
        NOT_STAMINA,
        //NOT_CASINO,
        //NOT_SEISEKI,
        //NOT_BONUS,

        MAX,
    }

    List<OptionMenuItem> OptionItem = null;
    private int m_LastUpdateCount = 0;

    private void Awake()
    {
        OptionItem = new List<OptionMenuItem>();
    }

    void LateUpdate()
    {
        if (m_LastUpdateCount != 0)
        {
            m_LastUpdateCount--;
            if (m_LastUpdateCount < 0)
            {
                m_LastUpdateCount = 0;
            }
            updateLayout();
            SetScrollPosition();

        }
    }

    void SetScrollPosition()
    {
        if (m_ScrollRect == null)
        {
            return;
        }

        LayoutGroup layoutGroup = m_ScrollRect.GetComponentInChildren<LayoutGroup>();
        if (layoutGroup == null)
        {
            return;
        }

        float height = m_ScrollRect.viewport.rect.height;
        float contentHeight = m_ScrollRect.content.rect.height;
        if (contentHeight <= height)
        {
            // コンテンツよりスクロールエリアのほうが広いので、スクロールしなくてもすべて表示されている
            m_ScrollRect.verticalNormalizedPosition = 1;
            m_ScrollRect.vertical = false;
            return;
        }

        m_ScrollRect.vertical = true;
    }

    public OptionMenuItem GetOptionItem(ItemType _type)
    {
        return OptionItem.Find((v) => v.m_Type == _type);
    }

    public OptionMenuItem AddItem()
    {
        OptionMenuItem item = Attach<OptionMenuItem>("Prefab/OptionMenu/OptionMenuItem", m_Cointent);
        OptionItem.Add(item);
        m_LastUpdateCount = 5;
        return item;
    }

    public OptionMenuSpace AddSpace()
    {
        OptionMenuSpace item = Attach<OptionMenuSpace>("Prefab/OptionMenu/OptionMenuSpace", m_Cointent);
        m_LastUpdateCount = 5;
        return item;
    }
}
