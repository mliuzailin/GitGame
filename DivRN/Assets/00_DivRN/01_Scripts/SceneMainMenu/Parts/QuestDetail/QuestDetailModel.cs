using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetailModel
{
    private static float InfoPanelHeight = 150;
    private static float MissionPanelHeight = 410;
    private static float MessagePanelHeight = 410;

    public enum TabType
    {
        Info,
        Mission,
        Rule,
        Boss,
    }

    private TabType m_currentType = TabType.Info;
    public TabType currentType
    {
        get { return m_currentType; }
        set { m_currentType = value; }
    }

    private Dictionary<TabType, float> m_typeWindowHeightMap = new Dictionary<TabType, float>
        {
            { TabType.Info , InfoPanelHeight},
            { TabType.Mission , MissionPanelHeight},
            { TabType.Rule , MessagePanelHeight},
            { TabType.Boss , MessagePanelHeight},
        };

    public void SetInfoPanelHeight(float height)
    {
        m_typeWindowHeightMap[TabType.Info] = height;
    }

    public void SetMissionPanelHeightDelta(float heightDelta)
    {
        m_typeWindowHeightMap[TabType.Mission] = heightDelta;
    }
    public void SetMessagePanelHeight(float height)
    {
        m_typeWindowHeightMap[TabType.Rule] = height;
        m_typeWindowHeightMap[TabType.Boss] = height;
    }


    public float GetBgHeight()
    {
        return m_typeWindowHeightMap.ContainsKey(currentType)
            ? m_typeWindowHeightMap[currentType]
            : m_typeWindowHeightMap[TabType.Info];
    }
}
