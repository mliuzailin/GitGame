using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class QuestDetailTabContext : M4uContext
{
    private QuestDetailTabModel m_model = null;
    public QuestDetailTabContext(QuestDetailTabModel model)
    {
        m_model = model;
    }

    public QuestDetailTabModel model { get { return m_model; } }

    public string TitleOff = "";
    public string TitleOn = "";
    M4uProperty<string> title = new M4uProperty<string>();
    private string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<bool> isSelected = new M4uProperty<bool>();
    public bool IsSelected
    {
        get { return m_model.isSelected; }
        set
        {
            m_model.isSelected = value;
            isSelected.Value = value;
            if (value)
            {
                Title = TitleOn;
            }
            else
            {
                Title = TitleOff;
            }
        }
    }

    public QuestDetailModel.TabType m_Type;
    public int m_Index = 0;
    public System.Action<QuestDetailTabContext> DidSelectTab = delegate { };
}
