using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class QuestDetailMission : MenuPartsBase
{
    private QuestDetailModel m_model = null;
    public void SetModel(QuestDetailModel model)
    {
        m_model = model;
        SetUpLayout();
    }

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<int> count = new M4uProperty<int>();
    public int Count { get { return count.Value; } set { count.Value = value; } }

    M4uProperty<int> countMax = new M4uProperty<int>();
    public int CountMax { get { return countMax.Value; } set { countMax.Value = value; } }

    M4uProperty<List<QuestMissionContext>> missionList = new M4uProperty<List<QuestMissionContext>>();
    public List<QuestMissionContext> MissionList { get { return missionList.Value; } set { missionList.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        MissionList = new List<QuestMissionContext>();
    }

    private void Start()
    {
        if (SafeAreaControl.Instance)
        {
            SafeAreaControl.Instance.addLocalYPos(transform);
        }
    }

    private void SetUpLayout()
    {
        Debug.Assert(m_model != null, "model not set.");
        var rectTransform = GetComponent<RectTransform>();
        Debug.Assert(rectTransform != null, "RectTransform not set.");
        m_model.SetMissionPanelHeightDelta(rectTransform.sizeDelta.y);
    }
}
